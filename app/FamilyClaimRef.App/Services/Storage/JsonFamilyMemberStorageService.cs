using System.Collections.Concurrent;
using System.IO;
using FamilyClaimRef.App.Models.Storage;

namespace FamilyClaimRef.App.Services.Storage;

public sealed class JsonFamilyMemberStorageService : IFamilyMemberStorageService
{
    public const string StoreFileName = "family-members.json";
    public const int StoreSchemaVersion = 1;

    private static readonly ConcurrentDictionary<string, SemaphoreSlim> StoreGates =
        new(StringComparer.OrdinalIgnoreCase);

    private readonly IFamilyMemberRecordStore store;
    private readonly SemaphoreSlim storeGate;
    private readonly Func<DateTimeOffset> utcNow;
    private readonly Func<string> idFactory;

    public JsonFamilyMemberStorageService(string metadataRootPath)
        : this(
            metadataRootPath,
            new JsonFamilyMemberRecordStore(metadataRootPath),
            static () => DateTimeOffset.UtcNow,
            static () => $"family_{Guid.NewGuid():N}")
    {
    }

    internal JsonFamilyMemberStorageService(
        string metadataRootPath,
        IFamilyMemberRecordStore store,
        Func<DateTimeOffset> utcNow,
        Func<string> idFactory)
    {
        if (string.IsNullOrWhiteSpace(metadataRootPath))
        {
            throw new ArgumentException("Metadata root path is required.", nameof(metadataRootPath));
        }

        this.store = store ?? throw new ArgumentNullException(nameof(store));
        this.utcNow = utcNow ?? throw new ArgumentNullException(nameof(utcNow));
        this.idFactory = idFactory ?? throw new ArgumentNullException(nameof(idFactory));

        var storeIdentity = Path.GetFullPath(Path.Combine(metadataRootPath, StoreFileName));
        storeGate = StoreGates.GetOrAdd(storeIdentity, static _ => new SemaphoreSlim(1, 1));
    }

    public async Task<IReadOnlyList<FamilyMemberRecord>> GetFamilyMembersAsync(
        CancellationToken cancellationToken = default)
    {
        await storeGate.WaitAsync(cancellationToken);
        try
        {
            var envelope = await store.LoadAsync(cancellationToken);
            return envelope.Items
                .OrderBy(member => member.CreatedAt)
                .ThenBy(member => member.Id, StringComparer.Ordinal)
                .ToList();
        }
        finally
        {
            storeGate.Release();
        }
    }

    public async Task<IReadOnlyList<FamilyMemberRecord>> GetActiveFamilyMembersAsync(
        CancellationToken cancellationToken = default)
    {
        await storeGate.WaitAsync(cancellationToken);
        try
        {
            var envelope = await store.LoadAsync(cancellationToken);
            return envelope.Items
                .Where(member => member.DisabledAt is null)
                .OrderBy(member => member.CreatedAt)
                .ThenBy(member => member.Id, StringComparer.Ordinal)
                .ToList();
        }
        finally
        {
            storeGate.Release();
        }
    }

    public async Task<FamilyMemberRecord?> GetFamilyMemberAsync(
        string id,
        CancellationToken cancellationToken = default)
    {
        await storeGate.WaitAsync(cancellationToken);
        try
        {
            var normalizedId = NormalizeRequiredValue(id, nameof(id));
            var envelope = await store.LoadAsync(cancellationToken);
            return envelope.Items.FirstOrDefault(member =>
                string.Equals(member.Id, normalizedId, StringComparison.Ordinal));
        }
        finally
        {
            storeGate.Release();
        }
    }

    public async Task<FamilyMemberRecord> CreateFamilyMemberAsync(
        FamilyMemberDraft draft,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(draft);

        await storeGate.WaitAsync(cancellationToken);
        try
        {
            var envelope = await store.LoadAsync(cancellationToken);
            var normalizedDraft = NormalizeDraft(draft);
            cancellationToken.ThrowIfCancellationRequested();

            var id = NormalizeRequiredValue(idFactory(), "generatedId");
            if (envelope.Items.Any(member => string.Equals(member.Id, id, StringComparison.Ordinal)))
            {
                throw new InvalidOperationException("Generated family member id already exists.");
            }

            var timestamp = utcNow();
            var record = new FamilyMemberRecord(
                id,
                normalizedDraft.DisplayName,
                normalizedDraft.Relation,
                normalizedDraft.Memo,
                timestamp,
                timestamp,
                null,
                1);

            var items = envelope.Items.ToList();
            items.Add(record);
            await store.SaveAsync(items, cancellationToken);
            return record;
        }
        finally
        {
            storeGate.Release();
        }
    }

    public async Task<FamilyMemberRecord> UpdateFamilyMemberAsync(
        string id,
        int expectedVersion,
        FamilyMemberDraft draft,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(draft);

        await storeGate.WaitAsync(cancellationToken);
        try
        {
            var envelope = await store.LoadAsync(cancellationToken);
            var normalizedId = NormalizeRequiredValue(id, nameof(id));
            var normalizedDraft = NormalizeDraft(draft);
            ValidateExpectedVersion(expectedVersion);
            cancellationToken.ThrowIfCancellationRequested();

            var items = envelope.Items.ToList();
            var index = items.FindIndex(member =>
                string.Equals(member.Id, normalizedId, StringComparison.Ordinal));
            var current = RequireActiveTarget(items, index);
            EnsureExpectedVersion(current, expectedVersion);

            var updated = current with
            {
                DisplayName = normalizedDraft.DisplayName,
                Relation = normalizedDraft.Relation,
                Memo = normalizedDraft.Memo,
                UpdatedAt = utcNow(),
                Version = checked(current.Version + 1)
            };
            items[index] = updated;

            await store.SaveAsync(items, cancellationToken);
            return updated;
        }
        finally
        {
            storeGate.Release();
        }
    }

    public async Task<FamilyMemberRecord> DeactivateFamilyMemberAsync(
        string id,
        int expectedVersion,
        CancellationToken cancellationToken = default)
    {
        await storeGate.WaitAsync(cancellationToken);
        try
        {
            var envelope = await store.LoadAsync(cancellationToken);
            var normalizedId = NormalizeRequiredValue(id, nameof(id));
            ValidateExpectedVersion(expectedVersion);
            cancellationToken.ThrowIfCancellationRequested();

            var items = envelope.Items.ToList();
            var index = items.FindIndex(member =>
                string.Equals(member.Id, normalizedId, StringComparison.Ordinal));
            var current = RequireActiveTarget(items, index);
            EnsureExpectedVersion(current, expectedVersion);

            var timestamp = utcNow();
            var deactivated = current with
            {
                UpdatedAt = timestamp,
                DisabledAt = timestamp,
                Version = checked(current.Version + 1)
            };
            items[index] = deactivated;

            await store.SaveAsync(items, cancellationToken);
            return deactivated;
        }
        finally
        {
            storeGate.Release();
        }
    }

    public async Task<FamilyMemberRecord> ReactivateFamilyMemberAsync(
        string id,
        int expectedVersion,
        CancellationToken cancellationToken = default)
    {
        await storeGate.WaitAsync(cancellationToken);
        try
        {
            var envelope = await store.LoadAsync(cancellationToken);
            var normalizedId = NormalizeRequiredValue(id, nameof(id));
            ValidateExpectedVersion(expectedVersion);
            cancellationToken.ThrowIfCancellationRequested();

            var items = envelope.Items.ToList();
            var index = items.FindIndex(member =>
                string.Equals(member.Id, normalizedId, StringComparison.Ordinal));
            var current = RequireInactiveTarget(items, index);
            EnsureExpectedVersion(current, expectedVersion);

            var reactivated = current with
            {
                UpdatedAt = utcNow(),
                DisabledAt = null,
                Version = checked(current.Version + 1)
            };
            items[index] = reactivated;

            await store.SaveAsync(items, cancellationToken);
            return reactivated;
        }
        finally
        {
            storeGate.Release();
        }
    }

    private static FamilyMemberDraft NormalizeDraft(FamilyMemberDraft draft)
    {
        var displayName = NormalizeRequiredValue(draft.DisplayName, nameof(draft.DisplayName));
        var relation = NormalizeRequiredValue(draft.Relation, nameof(draft.Relation));
        if (!FamilyMemberRelationValues.IsSupported(relation))
        {
            throw new ArgumentException("Relation is not supported.", nameof(draft.Relation));
        }

        var memo = string.IsNullOrWhiteSpace(draft.Memo) ? null : draft.Memo.Trim();
        return new FamilyMemberDraft(displayName, relation, memo);
    }

    private static FamilyMemberRecord RequireActiveTarget(
        IReadOnlyList<FamilyMemberRecord> items,
        int index)
    {
        if (index < 0 || items[index].DisabledAt is not null)
        {
            throw new FamilyMemberStorageException(
                FamilyMemberStorageErrorCode.TargetUnavailable,
                "Family member target is unavailable.");
        }

        return items[index];
    }

    private static FamilyMemberRecord RequireInactiveTarget(
        IReadOnlyList<FamilyMemberRecord> items,
        int index)
    {
        if (index < 0 || items[index].DisabledAt is null)
        {
            throw new FamilyMemberStorageException(
                FamilyMemberStorageErrorCode.TargetUnavailable,
                "Family member target is unavailable.");
        }

        return items[index];
    }

    private static void EnsureExpectedVersion(FamilyMemberRecord current, int expectedVersion)
    {
        if (current.Version != expectedVersion)
        {
            throw new FamilyMemberStorageException(
                FamilyMemberStorageErrorCode.VersionConflict,
                "Family member version conflict.");
        }
    }

    private static void ValidateExpectedVersion(int expectedVersion)
    {
        if (expectedVersion <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(expectedVersion),
                "Expected version must be positive.");
        }
    }

    private static string NormalizeRequiredValue(string value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Value is required.", parameterName);
        }

        return value.Trim();
    }
}

internal interface IFamilyMemberRecordStore
{
    Task<JsonFileEnvelope<FamilyMemberRecord>> LoadAsync(
        CancellationToken cancellationToken = default);

    Task SaveAsync(
        IReadOnlyList<FamilyMemberRecord> items,
        CancellationToken cancellationToken = default);
}

internal sealed class JsonFamilyMemberRecordStore : IFamilyMemberRecordStore
{
    private readonly JsonFileStore<FamilyMemberRecord> store;

    public JsonFamilyMemberRecordStore(string metadataRootPath)
    {
        store = new JsonFileStore<FamilyMemberRecord>(
            metadataRootPath,
            JsonFamilyMemberStorageService.StoreFileName,
            JsonFamilyMemberStorageService.StoreSchemaVersion);
    }

    public Task<JsonFileEnvelope<FamilyMemberRecord>> LoadAsync(
        CancellationToken cancellationToken = default)
    {
        return store.LoadAsync(cancellationToken);
    }

    public Task SaveAsync(
        IReadOnlyList<FamilyMemberRecord> items,
        CancellationToken cancellationToken = default)
    {
        return store.SaveAsync(items, cancellationToken);
    }
}
