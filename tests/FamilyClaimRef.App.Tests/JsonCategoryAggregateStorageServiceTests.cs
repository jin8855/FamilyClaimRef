using System.Text.Json;
using FamilyClaimRef.App.Models.Storage;
using FamilyClaimRef.App.Services.Storage;
using Xunit;

namespace FamilyClaimRef.App.Tests;

public sealed class JsonCategoryAggregateStorageServiceTests
{
    [Fact]
    public async Task Missing_file_returns_empty_schema_one_version_zero_without_writing()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var service = new JsonCategoryAggregateStorageService(rootPath);

            var snapshot = await service.LoadAsync();

            Assert.Equal(1, snapshot.SchemaVersion);
            Assert.Equal(0, snapshot.AggregateVersion);
            Assert.Empty(snapshot.Categories);
            Assert.False(File.Exists(StorePath(rootPath)));
        });
    }

    [Fact]
    public async Task Category_create_trims_values_increments_version_and_reloads()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var service = new JsonCategoryAggregateStorageService(rootPath);

            var result = await service.CreateCategoryAsync(
                0,
                new CategoryDraft("  의료 분류  ", "  MEDICAL  ", 10, "  설명  ", true));

            Assert.Equal(1, result.Snapshot.AggregateVersion);
            Assert.NotEqual(Guid.Empty, result.Record.RowId);
            Assert.Equal("의료 분류", result.Record.Name);
            Assert.Equal("MEDICAL", result.Record.Code);
            Assert.Equal("설명", result.Record.Description);
            Assert.True(result.Record.IsActive);
            var reloaded = await service.LoadAsync();
            Assert.Equal(result.Snapshot.SchemaVersion, reloaded.SchemaVersion);
            Assert.Equal(result.Snapshot.AggregateVersion, reloaded.AggregateVersion);
            AssertCategoryEqual(result.Record, Assert.Single(reloaded.Categories));

            using var json = JsonDocument.Parse(await File.ReadAllTextAsync(StorePath(rootPath)));
            Assert.Equal(1, json.RootElement.GetProperty("schemaVersion").GetInt32());
            Assert.Equal(1, json.RootElement.GetProperty("aggregateVersion").GetInt64());
            Assert.Equal(1, json.RootElement.GetProperty("categories").GetArrayLength());
            Assert.Equal(0, json.RootElement.GetProperty("categories")[0]
                .GetProperty("items").GetArrayLength());
        });
    }

    [Fact]
    public async Task Category_code_is_unique_after_trim_and_ordinal_ignore_case()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var service = new JsonCategoryAggregateStorageService(rootPath);
            var created = await service.CreateCategoryAsync(0, CategoryDraft("first", "Alpha"));
            var before = await File.ReadAllBytesAsync(StorePath(rootPath));

            var exception = await Assert.ThrowsAsync<CategoryAggregateStorageException>(() =>
                service.CreateCategoryAsync(
                    created.Snapshot.AggregateVersion,
                    CategoryDraft("second", "  aLpHa  ")));

            Assert.Equal(CategoryAggregateStorageErrorCode.DuplicateCode, exception.ErrorCode);
            Assert.Equal(before, await File.ReadAllBytesAsync(StorePath(rootPath)));
            Assert.Equal(1, (await service.LoadAsync()).AggregateVersion);
        });
    }

    [Fact]
    public async Task Item_code_is_unique_within_parent_but_reusable_in_another_parent()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var service = new JsonCategoryAggregateStorageService(rootPath);
            var firstParent = await service.CreateCategoryAsync(0, CategoryDraft("first", "P1"));
            var secondParent = await service.CreateCategoryAsync(1, CategoryDraft("second", "P2"));
            var firstItem = await service.CreateItemAsync(
                firstParent.Record.RowId,
                2,
                ItemDraft("first item", "Shared"));

            var duplicate = await Assert.ThrowsAsync<CategoryAggregateStorageException>(() =>
                service.CreateItemAsync(
                    firstParent.Record.RowId,
                    firstItem.Snapshot.AggregateVersion,
                    ItemDraft("duplicate", " shared ")));
            Assert.Equal(CategoryAggregateStorageErrorCode.DuplicateCode, duplicate.ErrorCode);

            var secondItem = await service.CreateItemAsync(
                secondParent.Record.RowId,
                firstItem.Snapshot.AggregateVersion,
                ItemDraft("second item", "SHARED"));

            Assert.Equal(4, secondItem.Snapshot.AggregateVersion);
            var reloaded = await service.LoadAsync();
            Assert.Single(reloaded.Categories.Single(c => c.RowId == firstParent.Record.RowId).Items);
            Assert.Single(reloaded.Categories.Single(c => c.RowId == secondParent.Record.RowId).Items);
        });
    }

    [Fact]
    public async Task Updates_target_only_explicit_row_ids_and_preserve_identity_and_parent()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var service = new JsonCategoryAggregateStorageService(rootPath);
            var first = await service.CreateCategoryAsync(0, CategoryDraft("first", "C1"));
            var second = await service.CreateCategoryAsync(1, CategoryDraft("second", "C2"));
            var item = await service.CreateItemAsync(
                first.Record.RowId,
                2,
                ItemDraft("item", "I1"));

            var updatedCategory = await service.UpdateCategoryAsync(
                second.Record.RowId,
                3,
                CategoryDraft("second updated", "C2-UPDATED"));
            var updatedItem = await service.UpdateItemAsync(
                first.Record.RowId,
                item.Record.RowId,
                4,
                ItemDraft("item updated", "I1-UPDATED"));

            Assert.Equal(second.Record.RowId, updatedCategory.Record.RowId);
            Assert.Equal("first", updatedItem.Snapshot.Categories
                .Single(c => c.RowId == first.Record.RowId).Name);
            Assert.Equal("second updated", updatedItem.Snapshot.Categories
                .Single(c => c.RowId == second.Record.RowId).Name);
            Assert.Equal(item.Record.RowId, updatedItem.Record.RowId);
            Assert.Equal(first.Record.RowId, updatedItem.Record.ParentCategoryId);
        });
    }

    [Fact]
    public async Task Missing_row_and_parent_mismatch_are_explicit_non_write_failures()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var service = new JsonCategoryAggregateStorageService(rootPath);
            var first = await service.CreateCategoryAsync(0, CategoryDraft("first", "C1"));
            var second = await service.CreateCategoryAsync(1, CategoryDraft("second", "C2"));
            var item = await service.CreateItemAsync(
                first.Record.RowId,
                2,
                ItemDraft("item", "I1"));
            var before = await File.ReadAllBytesAsync(StorePath(rootPath));

            var missing = await Assert.ThrowsAsync<CategoryAggregateStorageException>(() =>
                service.UpdateCategoryAsync(
                    Guid.NewGuid(),
                    3,
                    CategoryDraft("missing", "MISSING")));
            var mismatch = await Assert.ThrowsAsync<CategoryAggregateStorageException>(() =>
                service.UpdateItemAsync(
                    second.Record.RowId,
                    item.Record.RowId,
                    3,
                    ItemDraft("reparent", "I2")));

            Assert.Equal(CategoryAggregateStorageErrorCode.TargetUnavailable, missing.ErrorCode);
            Assert.Equal(CategoryAggregateStorageErrorCode.ParentMismatch, mismatch.ErrorCode);
            Assert.Equal(before, await File.ReadAllBytesAsync(StorePath(rootPath)));
            Assert.Equal(3, (await service.LoadAsync()).AggregateVersion);
        });
    }

    [Fact]
    public async Task Inactive_parent_rejects_item_create_and_reactivate()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var service = new JsonCategoryAggregateStorageService(rootPath);
            var parent = await service.CreateCategoryAsync(0, CategoryDraft("parent", "P"));
            var item = await service.CreateItemAsync(
                parent.Record.RowId,
                1,
                ItemDraft("item", "I"));
            var inactiveItem = await service.DeactivateItemAsync(
                parent.Record.RowId,
                item.Record.RowId,
                2);
            var inactiveParent = await service.DeactivateCategoryAsync(
                parent.Record.RowId,
                3);
            var before = await File.ReadAllBytesAsync(StorePath(rootPath));

            var createFailure = await Assert.ThrowsAsync<CategoryAggregateStorageException>(() =>
                service.CreateItemAsync(
                    parent.Record.RowId,
                    4,
                    ItemDraft("new", "N")));
            var reactivateFailure = await Assert.ThrowsAsync<CategoryAggregateStorageException>(() =>
                service.ReactivateItemAsync(
                    parent.Record.RowId,
                    inactiveItem.Record.RowId,
                    4));

            Assert.Equal(CategoryAggregateStorageErrorCode.ParentInactive, createFailure.ErrorCode);
            Assert.Equal(CategoryAggregateStorageErrorCode.ParentInactive, reactivateFailure.ErrorCode);
            Assert.Equal(before, await File.ReadAllBytesAsync(StorePath(rootPath)));
            Assert.False(inactiveParent.Record.IsActive);
        });
    }

    [Fact]
    public async Task Active_item_blocks_parent_deactivation_until_all_items_are_inactive()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var service = new JsonCategoryAggregateStorageService(rootPath);
            var parent = await service.CreateCategoryAsync(0, CategoryDraft("parent", "P"));
            var item = await service.CreateItemAsync(
                parent.Record.RowId,
                1,
                ItemDraft("item", "I"));
            var before = await File.ReadAllBytesAsync(StorePath(rootPath));

            var blocked = await Assert.ThrowsAsync<CategoryAggregateStorageException>(() =>
                service.DeactivateCategoryAsync(parent.Record.RowId, 2));
            Assert.Equal(
                CategoryAggregateStorageErrorCode.ActiveItemsBlockDeactivation,
                blocked.ErrorCode);
            Assert.Equal(before, await File.ReadAllBytesAsync(StorePath(rootPath)));

            await service.DeactivateItemAsync(parent.Record.RowId, item.Record.RowId, 2);
            var deactivated = await service.DeactivateCategoryAsync(parent.Record.RowId, 3);
            var reactivated = await service.ReactivateCategoryAsync(parent.Record.RowId, 4);

            Assert.False(deactivated.Record.IsActive);
            Assert.True(reactivated.Record.IsActive);
            Assert.Equal(5, reactivated.Snapshot.AggregateVersion);
        });
    }

    [Fact]
    public async Task Stale_version_conflicts_without_changing_file()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var service = new JsonCategoryAggregateStorageService(rootPath);
            var created = await service.CreateCategoryAsync(0, CategoryDraft("first", "C1"));
            var before = await File.ReadAllBytesAsync(StorePath(rootPath));

            var conflict = await Assert.ThrowsAsync<CategoryAggregateStorageException>(() =>
                service.CreateCategoryAsync(0, CategoryDraft("stale", "C2")));

            Assert.Equal(CategoryAggregateStorageErrorCode.VersionConflict, conflict.ErrorCode);
            Assert.Equal(before, await File.ReadAllBytesAsync(StorePath(rootPath)));
            var reloaded = await service.LoadAsync();
            Assert.Equal(created.Snapshot.AggregateVersion, reloaded.AggregateVersion);
            AssertCategoryEqual(created.Record, Assert.Single(reloaded.Categories));
        });
    }

    [Fact]
    public async Task Concurrent_same_version_mutations_produce_one_success_and_one_conflict()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var first = new JsonCategoryAggregateStorageService(rootPath);
            var second = new JsonCategoryAggregateStorageService(rootPath);

            var results = await Task.WhenAll(
                CaptureAsync(() => first.CreateCategoryAsync(0, CategoryDraft("first", "C1"))),
                CaptureAsync(() => second.CreateCategoryAsync(0, CategoryDraft("second", "C2"))));

            Assert.Single(results, result => result.Result is not null);
            var failure = Assert.Single(results, result => result.Exception is not null);
            var conflict = Assert.IsType<CategoryAggregateStorageException>(failure.Exception);
            Assert.Equal(CategoryAggregateStorageErrorCode.VersionConflict, conflict.ErrorCode);
            var snapshot = await first.LoadAsync();
            Assert.Equal(1, snapshot.AggregateVersion);
            Assert.Single(snapshot.Categories);
        });
    }

    [Fact]
    public async Task Every_successful_mutation_increments_aggregate_version_exactly_once()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var service = new JsonCategoryAggregateStorageService(rootPath);
            var category = await service.CreateCategoryAsync(0, CategoryDraft("category", "C"));
            var item = await service.CreateItemAsync(category.Record.RowId, 1, ItemDraft("item", "I"));
            var updated = await service.UpdateItemAsync(
                category.Record.RowId,
                item.Record.RowId,
                2,
                ItemDraft("updated", "I2"));
            var disabled = await service.DeactivateItemAsync(
                category.Record.RowId,
                item.Record.RowId,
                3);

            Assert.Equal([1L, 2L, 3L, 4L], new[]
            {
                category.Snapshot.AggregateVersion,
                item.Snapshot.AggregateVersion,
                updated.Snapshot.AggregateVersion,
                disabled.Snapshot.AggregateVersion
            });
        });
    }

    [Fact]
    public async Task Malformed_or_unsupported_schema_fails_closed_without_rewrite()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var path = StorePath(rootPath);
            Directory.CreateDirectory(rootPath);
            await File.WriteAllTextAsync(path, "{ invalid json");
            var malformed = await File.ReadAllBytesAsync(path);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                new JsonCategoryAggregateStorageService(rootPath).LoadAsync());
            Assert.Equal(malformed, await File.ReadAllBytesAsync(path));

            await File.WriteAllTextAsync(
                path,
                """
                {
                  "schemaVersion": 2,
                  "aggregateVersion": 4,
                  "savedAt": "2026-08-07T00:00:00Z",
                  "categories": []
                }
                """);
            var unsupported = await File.ReadAllBytesAsync(path);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                new JsonCategoryAggregateStorageService(rootPath).LoadAsync());
            Assert.Equal(unsupported, await File.ReadAllBytesAsync(path));
        });
    }

    [Fact]
    public async Task Inactive_parent_with_active_item_fails_closed_without_rewrite()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var path = StorePath(rootPath);
            var parentRowId = Guid.NewGuid();
            var timestamp = new DateTimeOffset(2026, 8, 7, 0, 0, 0, TimeSpan.Zero);
            var envelope = new CategoryAggregateEnvelope
            {
                SchemaVersion = JsonCategoryAggregateStorageService.StoreSchemaVersion,
                AggregateVersion = 2,
                SavedAt = timestamp,
                Categories =
                [
                    new CategoryRecord(
                        parentRowId,
                        "inactive parent",
                        "PARENT",
                        0,
                        null,
                        false,
                        timestamp,
                        timestamp,
                        timestamp,
                        [
                            new CategoryItemRecord(
                                Guid.NewGuid(),
                                parentRowId,
                                "active child",
                                "CHILD",
                                0,
                                null,
                                true,
                                true,
                                timestamp,
                                timestamp,
                                null)
                        ])
                ]
            };
            Directory.CreateDirectory(rootPath);
            await File.WriteAllTextAsync(path, JsonSerializer.Serialize(envelope));
            var before = await File.ReadAllBytesAsync(path);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                new JsonCategoryAggregateStorageService(rootPath).LoadAsync());

            Assert.Equal(before, await File.ReadAllBytesAsync(path));
        });
    }

    [Fact]
    public async Task Successful_replace_preserves_previous_valid_copy_and_removes_temp_files()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var service = new JsonCategoryAggregateStorageService(rootPath);
            var created = await service.CreateCategoryAsync(0, CategoryDraft("category", "C"));
            await service.UpdateCategoryAsync(
                created.Record.RowId,
                1,
                CategoryDraft("updated", "C2"));
            await service.UpdateCategoryAsync(
                created.Record.RowId,
                2,
                CategoryDraft("updated again", "C3"));

            using var current = JsonDocument.Parse(await File.ReadAllTextAsync(StorePath(rootPath)));
            using var backup = JsonDocument.Parse(await File.ReadAllTextAsync($"{StorePath(rootPath)}.bak"));
            Assert.Equal(3, current.RootElement.GetProperty("aggregateVersion").GetInt64());
            Assert.Equal(2, backup.RootElement.GetProperty("aggregateVersion").GetInt64());
            Assert.Empty(Directory.GetFiles(rootPath, "*.tmp", SearchOption.TopDirectoryOnly));
        });
    }

    [Fact]
    public async Task Store_failure_preserves_original_snapshot_and_does_not_retry()
    {
        var rootPath = CreateUniqueRootPath();
        var envelope = Envelope(version: 7);
        var store = new StubAggregateStore(envelope)
        {
            SaveException = new IOException("synthetic replace failure")
        };
        var service = new JsonCategoryAggregateStorageService(
            rootPath,
            store,
            static () => new DateTimeOffset(2026, 8, 7, 1, 0, 0, TimeSpan.Zero),
            static () => Guid.Parse("10000000-0000-0000-0000-000000000001"));

        await Assert.ThrowsAsync<IOException>(() =>
            service.CreateCategoryAsync(7, CategoryDraft("not saved", "NEW")));

        Assert.Equal(1, store.SaveCalls);
        Assert.Equal(7, store.Current.AggregateVersion);
        Assert.Empty(store.Current.Categories);
    }

    [Fact]
    public async Task Locked_destination_failure_preserves_original_and_cleans_temp_file()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var service = new JsonCategoryAggregateStorageService(rootPath);
            var created = await service.CreateCategoryAsync(0, CategoryDraft("category", "C"));
            var path = StorePath(rootPath);
            var before = await File.ReadAllBytesAsync(path);

            await using (var locked = new FileStream(
                path,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read))
            {
                var failure = await Record.ExceptionAsync(() => service.UpdateCategoryAsync(
                    created.Record.RowId,
                    1,
                    CategoryDraft("not saved", "C2")));
                Assert.True(failure is IOException or UnauthorizedAccessException, failure?.GetType().Name);
                Assert.Equal(before, await File.ReadAllBytesAsync(path));
                Assert.Empty(Directory.GetFiles(rootPath, "*.tmp", SearchOption.TopDirectoryOnly));
            }

            Assert.Equal(1, (await service.LoadAsync()).AggregateVersion);
        });
    }

    [Fact]
    public async Task Validation_rejects_blank_codes_and_errors_do_not_disclose_values_or_paths()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var service = new JsonCategoryAggregateStorageService(rootPath);

            await Assert.ThrowsAsync<ArgumentException>(() =>
                service.CreateCategoryAsync(0, CategoryDraft("category", " ")));
            var created = await service.CreateCategoryAsync(0, CategoryDraft("category", "PRIVATE-CODE"));
            var failure = await Assert.ThrowsAsync<CategoryAggregateStorageException>(() =>
                service.UpdateCategoryAsync(
                    Guid.NewGuid(),
                    created.Snapshot.AggregateVersion,
                    CategoryDraft("private name", "PRIVATE-OTHER")));

            Assert.DoesNotContain("PRIVATE-CODE", failure.Message, StringComparison.Ordinal);
            Assert.DoesNotContain("PRIVATE-OTHER", failure.Message, StringComparison.Ordinal);
            Assert.DoesNotContain("private name", failure.Message, StringComparison.Ordinal);
            Assert.DoesNotContain(rootPath, failure.Message, StringComparison.OrdinalIgnoreCase);
        });
    }

    private static void AssertCategoryEqual(CategoryRecord expected, CategoryRecord actual)
    {
        Assert.Equal(expected.RowId, actual.RowId);
        Assert.Equal(expected.Name, actual.Name);
        Assert.Equal(expected.Code, actual.Code);
        Assert.Equal(expected.SortOrder, actual.SortOrder);
        Assert.Equal(expected.Description, actual.Description);
        Assert.Equal(expected.IsSystemDefault, actual.IsSystemDefault);
        Assert.Equal(expected.CreatedAt, actual.CreatedAt);
        Assert.Equal(expected.UpdatedAt, actual.UpdatedAt);
        Assert.Equal(expected.DisabledAt, actual.DisabledAt);
        Assert.Equal(expected.Items, actual.Items);
    }

    private static CategoryDraft CategoryDraft(string name, string code)
    {
        return new CategoryDraft(name, code, 0, null, false);
    }

    private static CategoryItemDraft ItemDraft(string name, string code)
    {
        return new CategoryItemDraft(name, code, 0, null, true, true);
    }

    private static string StorePath(string rootPath)
    {
        return Path.Combine(rootPath, JsonCategoryAggregateStorageService.StoreFileName);
    }

    private static async Task<(
        CategoryMutationResult<CategoryRecord>? Result,
        Exception? Exception)> CaptureAsync(
        Func<Task<CategoryMutationResult<CategoryRecord>>> action)
    {
        try
        {
            return (await action(), null);
        }
        catch (Exception exception)
        {
            return (null, exception);
        }
    }

    private static async Task UsingTempRootAsync(Func<string, Task> action)
    {
        var rootPath = CreateUniqueRootPath();
        Directory.CreateDirectory(rootPath);

        try
        {
            await action(rootPath);
        }
        finally
        {
            if (Directory.Exists(rootPath))
            {
                Directory.Delete(rootPath, recursive: true);
            }
        }
    }

    private static string CreateUniqueRootPath()
    {
        return Path.Combine(
            Path.GetTempPath(),
            "FamilyClaimRef.App.Tests",
            "CategoryAggregateStorage",
            Guid.NewGuid().ToString("N"));
    }

    private static CategoryAggregateEnvelope Envelope(long version)
    {
        return new CategoryAggregateEnvelope
        {
            SchemaVersion = JsonCategoryAggregateStorageService.StoreSchemaVersion,
            AggregateVersion = version,
            SavedAt = new DateTimeOffset(2026, 8, 7, 0, 0, 0, TimeSpan.Zero),
            Categories = []
        };
    }

    private sealed class StubAggregateStore(CategoryAggregateEnvelope initial)
        : ICategoryAggregateRecordStore
    {
        public CategoryAggregateEnvelope Current { get; private set; } = initial;

        public Exception? SaveException { get; init; }

        public int SaveCalls { get; private set; }

        public Task<CategoryAggregateEnvelope> LoadAsync(
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(Current);
        }

        public Task SaveAsync(
            CategoryAggregateEnvelope envelope,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            SaveCalls++;
            if (SaveException is not null)
            {
                throw SaveException;
            }

            Current = envelope;
            return Task.CompletedTask;
        }
    }
}
