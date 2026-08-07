using System.Collections.Concurrent;
using System.IO;
using System.Text.Json;
using FamilyClaimRef.App.Models.Storage;

namespace FamilyClaimRef.App.Services.Storage;

public sealed class JsonCategoryAggregateStorageService : ICategoryAggregateStorageService
{
    public const string StoreFileName = "categories.json";
    public const int StoreSchemaVersion = 1;

    private static readonly ConcurrentDictionary<string, SemaphoreSlim> StoreGates =
        new(StringComparer.OrdinalIgnoreCase);

    private readonly ICategoryAggregateRecordStore store;
    private readonly SemaphoreSlim storeGate;
    private readonly Func<DateTimeOffset> utcNow;
    private readonly Func<Guid> rowIdFactory;

    public JsonCategoryAggregateStorageService(string metadataRootPath)
        : this(
            metadataRootPath,
            new JsonCategoryAggregateRecordStore(metadataRootPath),
            static () => DateTimeOffset.UtcNow,
            static () => Guid.NewGuid())
    {
    }

    internal JsonCategoryAggregateStorageService(
        string metadataRootPath,
        ICategoryAggregateRecordStore store,
        Func<DateTimeOffset> utcNow,
        Func<Guid> rowIdFactory)
    {
        if (string.IsNullOrWhiteSpace(metadataRootPath))
        {
            throw new ArgumentException("Metadata root path is required.", nameof(metadataRootPath));
        }

        this.store = store ?? throw new ArgumentNullException(nameof(store));
        this.utcNow = utcNow ?? throw new ArgumentNullException(nameof(utcNow));
        this.rowIdFactory = rowIdFactory ?? throw new ArgumentNullException(nameof(rowIdFactory));

        var storeIdentity = Path.GetFullPath(Path.Combine(metadataRootPath, StoreFileName));
        storeGate = StoreGates.GetOrAdd(storeIdentity, static _ => new SemaphoreSlim(1, 1));
    }

    public async Task<CategoryAggregateSnapshot> LoadAsync(
        CancellationToken cancellationToken = default)
    {
        await storeGate.WaitAsync(cancellationToken);
        try
        {
            return ToSnapshot(await store.LoadAsync(cancellationToken));
        }
        finally
        {
            storeGate.Release();
        }
    }

    public Task<CategoryMutationResult<CategoryRecord>> CreateCategoryAsync(
        long expectedAggregateVersion,
        CategoryDraft draft,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(draft);
        var normalizedDraft = NormalizeCategoryDraft(draft);

        return MutateAsync(
            expectedAggregateVersion,
            envelope =>
            {
                EnsureCategoryCodeAvailable(envelope.Categories, normalizedDraft.Code, null);
                var rowId = CreateUniqueRowId(envelope);
                var timestamp = utcNow();
                var record = new CategoryRecord(
                    rowId,
                    normalizedDraft.Name,
                    normalizedDraft.Code,
                    normalizedDraft.SortOrder,
                    normalizedDraft.Description,
                    normalizedDraft.IsSystemDefault,
                    timestamp,
                    timestamp,
                    null,
                    []);
                envelope.Categories.Add(record);
                return record;
            },
            cancellationToken);
    }

    public Task<CategoryMutationResult<CategoryRecord>> UpdateCategoryAsync(
        Guid categoryRowId,
        long expectedAggregateVersion,
        CategoryDraft draft,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(draft);
        ValidateRowId(categoryRowId, nameof(categoryRowId));
        var normalizedDraft = NormalizeCategoryDraft(draft);

        return MutateAsync(
            expectedAggregateVersion,
            envelope =>
            {
                var index = FindCategoryIndex(envelope.Categories, categoryRowId);
                var current = RequireCategory(envelope.Categories, index);
                EnsureCategoryCodeAvailable(
                    envelope.Categories,
                    normalizedDraft.Code,
                    categoryRowId);

                var updated = current with
                {
                    Name = normalizedDraft.Name,
                    Code = normalizedDraft.Code,
                    SortOrder = normalizedDraft.SortOrder,
                    Description = normalizedDraft.Description,
                    IsSystemDefault = normalizedDraft.IsSystemDefault,
                    UpdatedAt = utcNow()
                };
                envelope.Categories[index] = updated;
                return updated;
            },
            cancellationToken);
    }

    public Task<CategoryMutationResult<CategoryRecord>> DeactivateCategoryAsync(
        Guid categoryRowId,
        long expectedAggregateVersion,
        CancellationToken cancellationToken = default)
    {
        ValidateRowId(categoryRowId, nameof(categoryRowId));

        return MutateAsync(
            expectedAggregateVersion,
            envelope =>
            {
                var index = FindCategoryIndex(envelope.Categories, categoryRowId);
                var current = RequireCategory(envelope.Categories, index);
                if (!current.IsActive)
                {
                    throw TargetUnavailable();
                }

                if (current.Items.Any(item => item.IsActive))
                {
                    throw new CategoryAggregateStorageException(
                        CategoryAggregateStorageErrorCode.ActiveItemsBlockDeactivation,
                        "Active category items block category deactivation.");
                }

                var timestamp = utcNow();
                var updated = current with { UpdatedAt = timestamp, DisabledAt = timestamp };
                envelope.Categories[index] = updated;
                return updated;
            },
            cancellationToken);
    }

    public Task<CategoryMutationResult<CategoryRecord>> ReactivateCategoryAsync(
        Guid categoryRowId,
        long expectedAggregateVersion,
        CancellationToken cancellationToken = default)
    {
        ValidateRowId(categoryRowId, nameof(categoryRowId));

        return MutateAsync(
            expectedAggregateVersion,
            envelope =>
            {
                var index = FindCategoryIndex(envelope.Categories, categoryRowId);
                var current = RequireCategory(envelope.Categories, index);
                if (current.IsActive)
                {
                    throw TargetUnavailable();
                }

                var updated = current with { UpdatedAt = utcNow(), DisabledAt = null };
                envelope.Categories[index] = updated;
                return updated;
            },
            cancellationToken);
    }

    public Task<CategoryMutationResult<CategoryItemRecord>> CreateItemAsync(
        Guid parentCategoryId,
        long expectedAggregateVersion,
        CategoryItemDraft draft,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(draft);
        ValidateRowId(parentCategoryId, nameof(parentCategoryId));
        var normalizedDraft = NormalizeItemDraft(draft);

        return MutateAsync(
            expectedAggregateVersion,
            envelope =>
            {
                var categoryIndex = FindCategoryIndex(envelope.Categories, parentCategoryId);
                var category = RequireCategory(envelope.Categories, categoryIndex);
                EnsureParentActive(category);
                EnsureItemCodeAvailable(category.Items, normalizedDraft.Code, null);

                var timestamp = utcNow();
                var record = new CategoryItemRecord(
                    CreateUniqueRowId(envelope),
                    category.RowId,
                    normalizedDraft.Name,
                    normalizedDraft.Code,
                    normalizedDraft.SortOrder,
                    normalizedDraft.Description,
                    normalizedDraft.UseForPolicySearch,
                    normalizedDraft.UseForHistorySearch,
                    timestamp,
                    timestamp,
                    null);
                var items = category.Items.ToList();
                items.Add(record);
                envelope.Categories[categoryIndex] = category with
                {
                    Items = items,
                    UpdatedAt = timestamp
                };
                return record;
            },
            cancellationToken);
    }

    public Task<CategoryMutationResult<CategoryItemRecord>> UpdateItemAsync(
        Guid parentCategoryId,
        Guid itemRowId,
        long expectedAggregateVersion,
        CategoryItemDraft draft,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(draft);
        ValidateRowId(parentCategoryId, nameof(parentCategoryId));
        ValidateRowId(itemRowId, nameof(itemRowId));
        var normalizedDraft = NormalizeItemDraft(draft);

        return MutateItemAsync(
            parentCategoryId,
            itemRowId,
            expectedAggregateVersion,
            requireActiveParent: false,
            (category, current) =>
            {
                EnsureItemCodeAvailable(category.Items, normalizedDraft.Code, itemRowId);
                return current with
                {
                    Name = normalizedDraft.Name,
                    Code = normalizedDraft.Code,
                    SortOrder = normalizedDraft.SortOrder,
                    Description = normalizedDraft.Description,
                    UseForPolicySearch = normalizedDraft.UseForPolicySearch,
                    UseForHistorySearch = normalizedDraft.UseForHistorySearch,
                    UpdatedAt = utcNow()
                };
            },
            cancellationToken);
    }

    public Task<CategoryMutationResult<CategoryItemRecord>> DeactivateItemAsync(
        Guid parentCategoryId,
        Guid itemRowId,
        long expectedAggregateVersion,
        CancellationToken cancellationToken = default)
    {
        ValidateRowId(parentCategoryId, nameof(parentCategoryId));
        ValidateRowId(itemRowId, nameof(itemRowId));

        return MutateItemAsync(
            parentCategoryId,
            itemRowId,
            expectedAggregateVersion,
            requireActiveParent: false,
            (_, current) =>
            {
                if (!current.IsActive)
                {
                    throw TargetUnavailable();
                }

                var timestamp = utcNow();
                return current with { UpdatedAt = timestamp, DisabledAt = timestamp };
            },
            cancellationToken);
    }

    public Task<CategoryMutationResult<CategoryItemRecord>> ReactivateItemAsync(
        Guid parentCategoryId,
        Guid itemRowId,
        long expectedAggregateVersion,
        CancellationToken cancellationToken = default)
    {
        ValidateRowId(parentCategoryId, nameof(parentCategoryId));
        ValidateRowId(itemRowId, nameof(itemRowId));

        return MutateItemAsync(
            parentCategoryId,
            itemRowId,
            expectedAggregateVersion,
            requireActiveParent: true,
            (_, current) =>
            {
                if (current.IsActive)
                {
                    throw TargetUnavailable();
                }

                return current with { UpdatedAt = utcNow(), DisabledAt = null };
            },
            cancellationToken);
    }

    private Task<CategoryMutationResult<CategoryItemRecord>> MutateItemAsync(
        Guid parentCategoryId,
        Guid itemRowId,
        long expectedAggregateVersion,
        bool requireActiveParent,
        Func<CategoryRecord, CategoryItemRecord, CategoryItemRecord> mutation,
        CancellationToken cancellationToken)
    {
        return MutateAsync(
            expectedAggregateVersion,
            envelope =>
            {
                var actualParent = envelope.Categories.FirstOrDefault(category =>
                    category.Items.Any(item => item.RowId == itemRowId));
                if (actualParent is null)
                {
                    throw TargetUnavailable();
                }

                if (actualParent.RowId != parentCategoryId)
                {
                    throw new CategoryAggregateStorageException(
                        CategoryAggregateStorageErrorCode.ParentMismatch,
                        "Category item parent does not match.");
                }

                var categoryIndex = FindCategoryIndex(envelope.Categories, parentCategoryId);
                var category = RequireCategory(envelope.Categories, categoryIndex);
                if (requireActiveParent)
                {
                    EnsureParentActive(category);
                }

                var items = category.Items.ToList();
                var itemIndex = items.FindIndex(item => item.RowId == itemRowId);
                var updated = mutation(category, items[itemIndex]);
                if (updated.ParentCategoryId != category.RowId)
                {
                    throw new CategoryAggregateStorageException(
                        CategoryAggregateStorageErrorCode.ParentMismatch,
                        "Category item reparenting is not allowed.");
                }

                items[itemIndex] = updated;
                envelope.Categories[categoryIndex] = category with
                {
                    Items = items,
                    UpdatedAt = updated.UpdatedAt
                };
                return updated;
            },
            cancellationToken);
    }

    private async Task<CategoryMutationResult<T>> MutateAsync<T>(
        long expectedAggregateVersion,
        Func<CategoryAggregateEnvelope, T> mutation,
        CancellationToken cancellationToken)
    {
        if (expectedAggregateVersion < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(expectedAggregateVersion),
                "Expected aggregate version cannot be negative.");
        }

        await storeGate.WaitAsync(cancellationToken);
        try
        {
            var current = await store.LoadAsync(cancellationToken);
            if (current.AggregateVersion != expectedAggregateVersion)
            {
                throw new CategoryAggregateStorageException(
                    CategoryAggregateStorageErrorCode.VersionConflict,
                    "Category aggregate version conflict.");
            }

            cancellationToken.ThrowIfCancellationRequested();
            var working = CloneEnvelope(current);
            var record = mutation(working);
            working = new CategoryAggregateEnvelope
            {
                SchemaVersion = StoreSchemaVersion,
                AggregateVersion = checked(current.AggregateVersion + 1),
                SavedAt = utcNow(),
                Categories = working.Categories
            };

            await store.SaveAsync(working, cancellationToken);
            return new CategoryMutationResult<T>(ToSnapshot(working), record);
        }
        finally
        {
            storeGate.Release();
        }
    }

    private Guid CreateUniqueRowId(CategoryAggregateEnvelope envelope)
    {
        var rowId = rowIdFactory();
        if (rowId == Guid.Empty
            || envelope.Categories.Any(category =>
                category.RowId == rowId
                || category.Items.Any(item => item.RowId == rowId)))
        {
            throw new InvalidOperationException("Generated category row id is invalid or duplicated.");
        }

        return rowId;
    }

    private static CategoryAggregateEnvelope CloneEnvelope(CategoryAggregateEnvelope source)
    {
        return new CategoryAggregateEnvelope
        {
            SchemaVersion = source.SchemaVersion,
            AggregateVersion = source.AggregateVersion,
            SavedAt = source.SavedAt,
            Categories = source.Categories
                .Select(category => category with { Items = category.Items.ToList() })
                .ToList()
        };
    }

    private static CategoryAggregateSnapshot ToSnapshot(CategoryAggregateEnvelope envelope)
    {
        return new CategoryAggregateSnapshot(
            envelope.SchemaVersion,
            envelope.AggregateVersion,
            envelope.Categories
                .OrderBy(category => category.SortOrder)
                .ThenBy(category => category.Name, StringComparer.OrdinalIgnoreCase)
                .ThenBy(category => category.RowId)
                .Select(category => category with
                {
                    Items = category.Items
                        .OrderBy(item => item.SortOrder)
                        .ThenBy(item => item.Name, StringComparer.OrdinalIgnoreCase)
                        .ThenBy(item => item.RowId)
                        .ToList()
                })
                .ToList());
    }

    private static CategoryDraft NormalizeCategoryDraft(CategoryDraft draft)
    {
        return new CategoryDraft(
            NormalizeRequired(draft.Name, nameof(draft.Name)),
            NormalizeRequired(draft.Code, nameof(draft.Code)),
            NormalizeSortOrder(draft.SortOrder),
            NormalizeOptional(draft.Description),
            draft.IsSystemDefault);
    }

    private static CategoryItemDraft NormalizeItemDraft(CategoryItemDraft draft)
    {
        return new CategoryItemDraft(
            NormalizeRequired(draft.Name, nameof(draft.Name)),
            NormalizeRequired(draft.Code, nameof(draft.Code)),
            NormalizeSortOrder(draft.SortOrder),
            NormalizeOptional(draft.Description),
            draft.UseForPolicySearch,
            draft.UseForHistorySearch);
    }

    private static void EnsureCategoryCodeAvailable(
        IReadOnlyList<CategoryRecord> categories,
        string code,
        Guid? excludedRowId)
    {
        if (categories.Any(category =>
            category.RowId != excludedRowId
            && string.Equals(category.Code, code, StringComparison.OrdinalIgnoreCase)))
        {
            throw DuplicateCode();
        }
    }

    private static void EnsureItemCodeAvailable(
        IReadOnlyList<CategoryItemRecord> items,
        string code,
        Guid? excludedRowId)
    {
        if (items.Any(item =>
            item.RowId != excludedRowId
            && string.Equals(item.Code, code, StringComparison.OrdinalIgnoreCase)))
        {
            throw DuplicateCode();
        }
    }

    private static int FindCategoryIndex(IReadOnlyList<CategoryRecord> categories, Guid rowId)
    {
        for (var index = 0; index < categories.Count; index++)
        {
            if (categories[index].RowId == rowId)
            {
                return index;
            }
        }

        return -1;
    }

    private static CategoryRecord RequireCategory(
        IReadOnlyList<CategoryRecord> categories,
        int index)
    {
        if (index < 0)
        {
            throw TargetUnavailable();
        }

        return categories[index];
    }

    private static void EnsureParentActive(CategoryRecord category)
    {
        if (!category.IsActive)
        {
            throw new CategoryAggregateStorageException(
                CategoryAggregateStorageErrorCode.ParentInactive,
                "Category parent is inactive.");
        }
    }

    private static CategoryAggregateStorageException DuplicateCode()
    {
        return new CategoryAggregateStorageException(
            CategoryAggregateStorageErrorCode.DuplicateCode,
            "Category code already exists in its uniqueness scope.");
    }

    private static CategoryAggregateStorageException TargetUnavailable()
    {
        return new CategoryAggregateStorageException(
            CategoryAggregateStorageErrorCode.TargetUnavailable,
            "Category aggregate target is unavailable.");
    }

    private static void ValidateRowId(Guid rowId, string parameterName)
    {
        if (rowId == Guid.Empty)
        {
            throw new ArgumentException("Row id is required.", parameterName);
        }
    }

    private static int NormalizeSortOrder(int value)
    {
        if (value < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(value), "Sort order cannot be negative.");
        }

        return value;
    }

    private static string NormalizeRequired(string value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Value is required.", parameterName);
        }

        return value.Trim();
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}

internal interface ICategoryAggregateRecordStore
{
    Task<CategoryAggregateEnvelope> LoadAsync(CancellationToken cancellationToken = default);

    Task SaveAsync(
        CategoryAggregateEnvelope envelope,
        CancellationToken cancellationToken = default);
}

internal sealed class JsonCategoryAggregateRecordStore : ICategoryAggregateRecordStore
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    private readonly string filePath;

    public JsonCategoryAggregateRecordStore(string metadataRootPath)
    {
        if (string.IsNullOrWhiteSpace(metadataRootPath))
        {
            throw new ArgumentException("Metadata root path is required.", nameof(metadataRootPath));
        }

        filePath = Path.GetFullPath(Path.Combine(
            metadataRootPath,
            JsonCategoryAggregateStorageService.StoreFileName));
    }

    public async Task<CategoryAggregateEnvelope> LoadAsync(
        CancellationToken cancellationToken = default)
    {
        if (!File.Exists(filePath))
        {
            return new CategoryAggregateEnvelope
            {
                SchemaVersion = JsonCategoryAggregateStorageService.StoreSchemaVersion,
                AggregateVersion = 0,
                Categories = []
            };
        }

        return await ReadAndValidateAsync(filePath, cancellationToken);
    }

    public async Task SaveAsync(
        CategoryAggregateEnvelope envelope,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(envelope);
        ValidateEnvelope(envelope, requireSavedAt: true);

        var directory = Path.GetDirectoryName(filePath)
            ?? throw new InvalidOperationException("Category storage directory is unavailable.");
        Directory.CreateDirectory(directory);

        var tempFilePath = $"{filePath}.{Guid.NewGuid():N}.tmp";
        try
        {
            await using (var stream = new FileStream(
                tempFilePath,
                FileMode.CreateNew,
                FileAccess.Write,
                FileShare.None,
                bufferSize: 4096,
                FileOptions.Asynchronous | FileOptions.WriteThrough))
            {
                await JsonSerializer.SerializeAsync(stream, envelope, JsonOptions, cancellationToken);
                await stream.FlushAsync(cancellationToken);
                stream.Flush(flushToDisk: true);
            }

            var verified = await ReadAndValidateAsync(tempFilePath, cancellationToken);
            if (verified.SchemaVersion != envelope.SchemaVersion
                || verified.AggregateVersion != envelope.AggregateVersion)
            {
                throw new InvalidOperationException("Category aggregate verification failed.");
            }

            if (File.Exists(filePath))
            {
                File.Replace(tempFilePath, filePath, $"{filePath}.bak", ignoreMetadataErrors: true);
            }
            else
            {
                File.Move(tempFilePath, filePath);
            }
        }
        finally
        {
            TryDeleteTempFile(tempFilePath);
        }
    }

    private static async Task<CategoryAggregateEnvelope> ReadAndValidateAsync(
        string path,
        CancellationToken cancellationToken)
    {
        CategoryAggregateEnvelope? envelope;
        try
        {
            await using var stream = new FileStream(
                path,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read,
                bufferSize: 4096,
                useAsync: true);
            envelope = await JsonSerializer.DeserializeAsync<CategoryAggregateEnvelope>(
                stream,
                JsonOptions,
                cancellationToken);
        }
        catch (JsonException exception)
        {
            throw new InvalidOperationException("Category aggregate JSON is invalid.", exception);
        }

        if (envelope is null)
        {
            throw new InvalidOperationException("Category aggregate JSON is empty or invalid.");
        }

        ValidateEnvelope(envelope, requireSavedAt: true);
        return envelope;
    }

    private static void ValidateEnvelope(
        CategoryAggregateEnvelope envelope,
        bool requireSavedAt)
    {
        if (envelope.SchemaVersion != JsonCategoryAggregateStorageService.StoreSchemaVersion)
        {
            throw new InvalidOperationException("Category aggregate schema version is unsupported.");
        }

        if (envelope.AggregateVersion < 0)
        {
            throw new InvalidOperationException("Category aggregate version is invalid.");
        }

        if (requireSavedAt && envelope.SavedAt == default)
        {
            throw new InvalidOperationException("Category aggregate savedAt is required.");
        }

        if (envelope.Categories is null)
        {
            throw new InvalidOperationException("Category aggregate categories are required.");
        }

        var rowIds = new HashSet<Guid>();
        var categoryCodes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var category in envelope.Categories)
        {
            ValidateCategory(category, rowIds, categoryCodes);
        }
    }

    private static void ValidateCategory(
        CategoryRecord category,
        ISet<Guid> rowIds,
        ISet<string> categoryCodes)
    {
        if (category.RowId == Guid.Empty || !rowIds.Add(category.RowId))
        {
            throw new InvalidOperationException("Category row id is invalid or duplicated.");
        }

        if (string.IsNullOrWhiteSpace(category.Name)
            || string.IsNullOrWhiteSpace(category.Code)
            || category.Name != category.Name.Trim()
            || category.Code != category.Code.Trim()
            || !categoryCodes.Add(category.Code)
            || category.SortOrder < 0
            || category.CreatedAt == default
            || category.UpdatedAt == default
            || category.Items is null)
        {
            throw new InvalidOperationException("Category aggregate category is invalid.");
        }

        if (!category.IsActive && category.Items.Any(item => item.IsActive))
        {
            throw new InvalidOperationException("Inactive category cannot contain active items.");
        }

        var itemCodes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var item in category.Items)
        {
            if (item.RowId == Guid.Empty
                || !rowIds.Add(item.RowId)
                || item.ParentCategoryId != category.RowId
                || string.IsNullOrWhiteSpace(item.Name)
                || string.IsNullOrWhiteSpace(item.Code)
                || item.Name != item.Name.Trim()
                || item.Code != item.Code.Trim()
                || !itemCodes.Add(item.Code)
                || item.SortOrder < 0
                || item.CreatedAt == default
                || item.UpdatedAt == default)
            {
                throw new InvalidOperationException("Category aggregate item is invalid.");
            }
        }
    }

    private static void TryDeleteTempFile(string path)
    {
        try
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
        catch (IOException)
        {
        }
        catch (UnauthorizedAccessException)
        {
        }
    }
}
