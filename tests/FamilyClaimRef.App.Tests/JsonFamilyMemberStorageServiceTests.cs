using System.Text.Json;
using FamilyClaimRef.App.Models.Storage;
using FamilyClaimRef.App.Services.Storage;
using Xunit;

namespace FamilyClaimRef.App.Tests;

public sealed class JsonFamilyMemberStorageServiceTests
{
    [Fact]
    public async Task Empty_store_returns_no_active_members_without_creating_file()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var service = new JsonFamilyMemberStorageService(rootPath);

            Assert.Empty(await service.GetFamilyMembersAsync());
            Assert.Empty(await service.GetActiveFamilyMembersAsync());
            Assert.False(File.Exists(Path.Combine(rootPath, JsonFamilyMemberStorageService.StoreFileName)));
        });
    }

    [Fact]
    public async Task Create_trims_values_sets_version_one_and_persists_schema_one()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var service = new JsonFamilyMemberStorageService(rootPath);

            var created = await service.CreateFamilyMemberAsync(
                new FamilyMemberDraft(
                    "  synthetic family  ",
                    FamilyMemberRelationValues.Self,
                    "  synthetic memo  "));

            Assert.StartsWith("family_", created.Id, StringComparison.Ordinal);
            Assert.Equal("synthetic family", created.DisplayName);
            Assert.Equal(FamilyMemberRelationValues.Self, created.Relation);
            Assert.Equal("synthetic memo", created.Memo);
            Assert.Equal(1, created.Version);
            Assert.Equal(created.CreatedAt, created.UpdatedAt);
            Assert.Null(created.DisabledAt);

            var reloaded = Assert.Single(await service.GetActiveFamilyMembersAsync());
            Assert.Equal(created, reloaded);

            var json = await File.ReadAllTextAsync(
                Path.Combine(rootPath, JsonFamilyMemberStorageService.StoreFileName));
            using var document = JsonDocument.Parse(json);
            Assert.Equal(
                JsonFamilyMemberStorageService.StoreSchemaVersion,
                document.RootElement.GetProperty("schemaVersion").GetInt32());
            var persistedRelation = document.RootElement
                .GetProperty("items")[0]
                .GetProperty("relation")
                .GetString();
            Assert.Equal(FamilyMemberRelationValues.Self, persistedRelation);
            Assert.NotEqual("본인 후보", persistedRelation);
            Assert.NotEqual("가족 후보", persistedRelation);
        });
    }

    [Fact]
    public async Task Create_allows_duplicate_display_names_with_distinct_ids()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var service = new JsonFamilyMemberStorageService(rootPath);
            var draft = CreateDraft("same display");

            var first = await service.CreateFamilyMemberAsync(draft);
            var second = await service.CreateFamilyMemberAsync(draft);

            Assert.NotEqual(first.Id, second.Id);
            Assert.Equal(2, (await service.GetActiveFamilyMembersAsync()).Count);
        });
    }

    [Fact]
    public async Task Create_rejects_empty_display_name_and_unsupported_relation()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var service = new JsonFamilyMemberStorageService(rootPath);

            await Assert.ThrowsAsync<ArgumentException>(() =>
                service.CreateFamilyMemberAsync(
                    new FamilyMemberDraft(" ", FamilyMemberRelationValues.Mother, null)));
            await Assert.ThrowsAsync<ArgumentException>(() =>
                service.CreateFamilyMemberAsync(
                    new FamilyMemberDraft("synthetic", "unsupported", null)));
            await Assert.ThrowsAsync<ArgumentException>(() =>
                service.CreateFamilyMemberAsync(
                    new FamilyMemberDraft("synthetic", "본인 후보", null)));
            await Assert.ThrowsAsync<ArgumentException>(() =>
                service.CreateFamilyMemberAsync(
                    new FamilyMemberDraft("synthetic", "가족 후보", null)));
            Assert.Empty(await service.GetActiveFamilyMembersAsync());
        });
    }

    [Fact]
    public async Task Update_uses_explicit_id_preserves_immutable_fields_and_increments_version()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var service = new JsonFamilyMemberStorageService(rootPath);
            var created = await service.CreateFamilyMemberAsync(CreateDraft());

            var updated = await service.UpdateFamilyMemberAsync(
                created.Id,
                created.Version,
                new FamilyMemberDraft(
                    "updated synthetic",
                    FamilyMemberRelationValues.Self,
                    null));

            Assert.Equal(created.Id, updated.Id);
            Assert.Equal(created.CreatedAt, updated.CreatedAt);
            Assert.Null(updated.DisabledAt);
            Assert.Equal(2, updated.Version);
            Assert.Equal("updated synthetic", updated.DisplayName);
            Assert.Null(updated.Memo);
        });
    }

    [Fact]
    public async Task Stale_update_returns_version_conflict_and_does_not_write()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var service = new JsonFamilyMemberStorageService(rootPath);
            var created = await service.CreateFamilyMemberAsync(CreateDraft());
            var updated = await service.UpdateFamilyMemberAsync(
                created.Id,
                created.Version,
                CreateDraft("first update"));

            var exception = await Assert.ThrowsAsync<FamilyMemberStorageException>(() =>
                service.UpdateFamilyMemberAsync(
                    created.Id,
                    created.Version,
                    CreateDraft("stale update")));

            Assert.Equal(FamilyMemberStorageErrorCode.VersionConflict, exception.ErrorCode);
            Assert.Equal(updated, await service.GetFamilyMemberAsync(created.Id));
        });
    }

    [Fact]
    public async Task Deactivate_increments_version_and_excludes_record_from_active_query()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var service = new JsonFamilyMemberStorageService(rootPath);
            var created = await service.CreateFamilyMemberAsync(CreateDraft());

            var deactivated = await service.DeactivateFamilyMemberAsync(
                created.Id,
                created.Version);

            Assert.Equal(2, deactivated.Version);
            Assert.NotNull(deactivated.DisabledAt);
            Assert.Equal(deactivated.DisabledAt, deactivated.UpdatedAt);
            Assert.Equal(deactivated, Assert.Single(await service.GetFamilyMembersAsync()));
            Assert.Empty(await service.GetActiveFamilyMembersAsync());
            Assert.Equal(deactivated, await service.GetFamilyMemberAsync(created.Id));
        });
    }

    [Fact]
    public async Task Reactivate_clears_disabled_at_increments_version_and_restores_active_query()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var service = new JsonFamilyMemberStorageService(rootPath);
            var created = await service.CreateFamilyMemberAsync(CreateDraft());
            var deactivated = await service.DeactivateFamilyMemberAsync(
                created.Id,
                created.Version);

            var reactivated = await service.ReactivateFamilyMemberAsync(
                deactivated.Id,
                deactivated.Version);

            Assert.Equal(3, reactivated.Version);
            Assert.Null(reactivated.DisabledAt);
            Assert.True(reactivated.UpdatedAt >= deactivated.UpdatedAt);
            Assert.Equal(reactivated, Assert.Single(await service.GetFamilyMembersAsync()));
            Assert.Equal(reactivated, Assert.Single(await service.GetActiveFamilyMembersAsync()));
            Assert.Equal(reactivated, await service.GetFamilyMemberAsync(created.Id));
        });
    }

    [Fact]
    public async Task Stale_or_inactive_targets_are_non_write_failures()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var service = new JsonFamilyMemberStorageService(rootPath);
            var created = await service.CreateFamilyMemberAsync(CreateDraft());
            var activeReactivate = await Assert.ThrowsAsync<FamilyMemberStorageException>(() =>
                service.ReactivateFamilyMemberAsync(created.Id, created.Version));
            var updated = await service.UpdateFamilyMemberAsync(
                created.Id,
                created.Version,
                CreateDraft("updated before deactivate"));

            var staleDeactivate = await Assert.ThrowsAsync<FamilyMemberStorageException>(() =>
                service.DeactivateFamilyMemberAsync(created.Id, created.Version));
            var deactivated = await service.DeactivateFamilyMemberAsync(
                updated.Id,
                updated.Version);

            var inactiveDeactivate = await Assert.ThrowsAsync<FamilyMemberStorageException>(() =>
                service.DeactivateFamilyMemberAsync(deactivated.Id, deactivated.Version));
            var inactiveUpdate = await Assert.ThrowsAsync<FamilyMemberStorageException>(() =>
                service.UpdateFamilyMemberAsync(
                    created.Id,
                    deactivated.Version,
                    CreateDraft("not written")));

            Assert.Equal(
                FamilyMemberStorageErrorCode.TargetUnavailable,
                activeReactivate.ErrorCode);
            Assert.Equal(FamilyMemberStorageErrorCode.VersionConflict, staleDeactivate.ErrorCode);
            Assert.Equal(
                FamilyMemberStorageErrorCode.TargetUnavailable,
                inactiveDeactivate.ErrorCode);
            Assert.Equal(FamilyMemberStorageErrorCode.TargetUnavailable, inactiveUpdate.ErrorCode);
            Assert.Equal(deactivated, await service.GetFamilyMemberAsync(created.Id));
        });
    }

    [Fact]
    public async Task Provider_instances_for_same_store_share_process_gate_and_prevent_lost_update()
    {
        var rootPath = CreateUniqueRootPath();
        var created = CreateRecord();
        var sharedState = new SharedRecordStoreState([created]);
        var firstStore = new GateProbeRecordStore(sharedState, blockFirstLoad: true);
        var secondStore = new GateProbeRecordStore(sharedState, blockFirstLoad: false);
        var firstProvider = CreateProvider(rootPath, firstStore);
        var secondProvider = CreateProvider(rootPath, secondStore);

        var firstUpdate = CaptureAsync(() => firstProvider.UpdateFamilyMemberAsync(
            created.Id,
            created.Version,
            CreateDraft("first contender")));
        await firstStore.LoadEntered.Task.WaitAsync(TimeSpan.FromSeconds(5));

        var secondUpdate = CaptureAsync(() => secondProvider.UpdateFamilyMemberAsync(
            created.Id,
            created.Version,
            CreateDraft("second contender")));
        await Task.Delay(100);
        Assert.Equal(0, secondStore.LoadCalls);

        firstStore.ReleaseLoad.TrySetResult(true);
        var results = await Task.WhenAll(firstUpdate, secondUpdate);

        Assert.Single(results, result => result.Record is not null);
        var failure = Assert.Single(results, result => result.Exception is not null);
        var conflict = Assert.IsType<FamilyMemberStorageException>(failure.Exception);
        Assert.Equal(FamilyMemberStorageErrorCode.VersionConflict, conflict.ErrorCode);
        Assert.Equal(1, secondStore.LoadCalls);
        Assert.Equal(2, Assert.Single(sharedState.Snapshot()).Version);
    }

    [Fact]
    public async Task Cancellation_before_mutation_does_not_create_store_file()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var service = new JsonFamilyMemberStorageService(rootPath);
            using var cancellation = new CancellationTokenSource();
            cancellation.Cancel();

            await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
                service.CreateFamilyMemberAsync(CreateDraft(), cancellation.Token));

            Assert.False(File.Exists(Path.Combine(rootPath, JsonFamilyMemberStorageService.StoreFileName)));
        });
    }

    [Fact]
    public async Task Malformed_or_incompatible_envelope_fails_without_rewrite()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var filePath = Path.Combine(rootPath, JsonFamilyMemberStorageService.StoreFileName);
            await File.WriteAllTextAsync(filePath, "{ invalid json");
            var malformedBefore = await File.ReadAllTextAsync(filePath);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                new JsonFamilyMemberStorageService(rootPath).GetActiveFamilyMembersAsync());
            Assert.Equal(malformedBefore, await File.ReadAllTextAsync(filePath));

            await File.WriteAllTextAsync(
                filePath,
                """
                {
                  "schemaVersion": 2,
                  "savedAt": "2026-01-01T00:00:00Z",
                  "items": []
                }
                """);
            var incompatibleBefore = await File.ReadAllTextAsync(filePath);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                new JsonFamilyMemberStorageService(rootPath).GetActiveFamilyMembersAsync());
            Assert.Equal(incompatibleBefore, await File.ReadAllTextAsync(filePath));
        });
    }

    [Fact]
    public async Task Save_failure_preserves_previous_records_and_is_not_retried()
    {
        var rootPath = CreateUniqueRootPath();
        var existing = CreateRecord();
        var store = new StubRecordStore([existing])
        {
            SaveException = new IOException("synthetic save failure")
        };
        var service = new JsonFamilyMemberStorageService(
            rootPath,
            store,
            static () => new DateTimeOffset(2026, 8, 3, 1, 0, 0, TimeSpan.Zero),
            static () => "family_new");

        await Assert.ThrowsAsync<IOException>(() =>
            service.UpdateFamilyMemberAsync(
                existing.Id,
                existing.Version,
                CreateDraft("not persisted")));

        Assert.Equal(1, store.SaveCalls);
        Assert.Equal(existing, Assert.Single(store.Items));
    }

    [Fact]
    public async Task Atomic_move_failure_preserves_existing_json_and_removes_temp_file()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var service = new JsonFamilyMemberStorageService(rootPath);
            var created = await service.CreateFamilyMemberAsync(CreateDraft());
            var filePath = Path.Combine(rootPath, JsonFamilyMemberStorageService.StoreFileName);
            var before = await File.ReadAllBytesAsync(filePath);

            await using (var lockStream = new FileStream(
                filePath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read))
            {
                var exception = await Record.ExceptionAsync(() =>
                    service.UpdateFamilyMemberAsync(
                        created.Id,
                        created.Version,
                        CreateDraft("not persisted")));
                Assert.True(
                    exception is IOException or UnauthorizedAccessException,
                    exception?.GetType().FullName);

                Assert.Equal(before, await File.ReadAllBytesAsync(filePath));
                Assert.Empty(Directory.GetFiles(rootPath, "*.tmp", SearchOption.TopDirectoryOnly));
            }

            Assert.Equal(created, await service.GetFamilyMemberAsync(created.Id));
        });
    }

    [Fact]
    public async Task Structured_failures_do_not_include_raw_target_or_payload_values()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var service = new JsonFamilyMemberStorageService(rootPath);
            var rawId = "family_private_raw_id";
            var exception = await Assert.ThrowsAsync<FamilyMemberStorageException>(() =>
                service.UpdateFamilyMemberAsync(rawId, 1, CreateDraft("private display")));

            Assert.DoesNotContain(rawId, exception.Message, StringComparison.Ordinal);
            Assert.DoesNotContain("private display", exception.Message, StringComparison.Ordinal);
            Assert.DoesNotContain(rootPath, exception.Message, StringComparison.OrdinalIgnoreCase);
        });
    }

    private static FamilyMemberDraft CreateDraft(string displayName = "synthetic family")
    {
        return new FamilyMemberDraft(
            displayName,
            FamilyMemberRelationValues.Mother,
            "synthetic memo");
    }

    private static FamilyMemberRecord CreateRecord()
    {
        var createdAt = new DateTimeOffset(2026, 8, 3, 0, 0, 0, TimeSpan.Zero);
        return new FamilyMemberRecord(
            "family_existing",
            "existing synthetic",
            FamilyMemberRelationValues.Self,
            null,
            createdAt,
            createdAt,
            null,
            1);
    }

    private static JsonFamilyMemberStorageService CreateProvider(
        string rootPath,
        IFamilyMemberRecordStore store)
    {
        return new JsonFamilyMemberStorageService(
            rootPath,
            store,
            static () => new DateTimeOffset(2026, 8, 3, 1, 0, 0, TimeSpan.Zero),
            static () => "family_unused");
    }

    private static async Task<(FamilyMemberRecord? Record, Exception? Exception)> CaptureAsync(
        Func<Task<FamilyMemberRecord>> action)
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
            "FamilyMemberStorage",
            Guid.NewGuid().ToString("N"));
    }

    private sealed class StubRecordStore(IReadOnlyList<FamilyMemberRecord> items)
        : IFamilyMemberRecordStore
    {
        public IReadOnlyList<FamilyMemberRecord> Items { get; private set; } = items.ToList();

        public Exception? SaveException { get; init; }

        public int SaveCalls { get; private set; }

        public Task<JsonFileEnvelope<FamilyMemberRecord>> LoadAsync(
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(new JsonFileEnvelope<FamilyMemberRecord>
            {
                SchemaVersion = JsonFamilyMemberStorageService.StoreSchemaVersion,
                SavedAt = new DateTimeOffset(2026, 8, 3, 0, 0, 0, TimeSpan.Zero),
                Items = Items.ToList()
            });
        }

        public Task SaveAsync(
            IReadOnlyList<FamilyMemberRecord> updatedItems,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            SaveCalls++;
            if (SaveException is not null)
            {
                throw SaveException;
            }

            Items = updatedItems.ToList();
            return Task.CompletedTask;
        }
    }

    private sealed class SharedRecordStoreState(IReadOnlyList<FamilyMemberRecord> items)
    {
        private readonly object sync = new();
        private IReadOnlyList<FamilyMemberRecord> currentItems = items.ToList();

        public IReadOnlyList<FamilyMemberRecord> Snapshot()
        {
            lock (sync)
            {
                return currentItems.ToList();
            }
        }

        public void Replace(IReadOnlyList<FamilyMemberRecord> itemsToSave)
        {
            lock (sync)
            {
                currentItems = itemsToSave.ToList();
            }
        }
    }

    private sealed class GateProbeRecordStore(
        SharedRecordStoreState state,
        bool blockFirstLoad) : IFamilyMemberRecordStore
    {
        private int loadCalls;

        public TaskCompletionSource<bool> LoadEntered { get; } =
            new(TaskCreationOptions.RunContinuationsAsynchronously);

        public TaskCompletionSource<bool> ReleaseLoad { get; } =
            new(TaskCreationOptions.RunContinuationsAsynchronously);

        public int LoadCalls => Volatile.Read(ref loadCalls);

        public async Task<JsonFileEnvelope<FamilyMemberRecord>> LoadAsync(
            CancellationToken cancellationToken = default)
        {
            var call = Interlocked.Increment(ref loadCalls);
            if (blockFirstLoad && call == 1)
            {
                LoadEntered.TrySetResult(true);
                await ReleaseLoad.Task.WaitAsync(cancellationToken);
            }

            return new JsonFileEnvelope<FamilyMemberRecord>
            {
                SchemaVersion = JsonFamilyMemberStorageService.StoreSchemaVersion,
                SavedAt = new DateTimeOffset(2026, 8, 3, 0, 0, 0, TimeSpan.Zero),
                Items = state.Snapshot().ToList()
            };
        }

        public Task SaveAsync(
            IReadOnlyList<FamilyMemberRecord> itemsToSave,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            state.Replace(itemsToSave);
            return Task.CompletedTask;
        }
    }
}
