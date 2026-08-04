using FamilyClaimRef.App.Models.Storage;
using FamilyClaimRef.App.Services.Localization;
using FamilyClaimRef.App.Services.Storage;
using FamilyClaimRef.App.ViewModels;
using Xunit;

namespace FamilyClaimRef.App.Tests;

public sealed class FamilyMemberManagementViewModelTests
{
    [Fact]
    public void Create_mode_requires_display_name_and_approved_relation()
    {
        var viewModel = CreateViewModel(new StubStorageService());

        Assert.False(viewModel.IsEditMode);
        Assert.False(viewModel.CanSave);
        Assert.False(viewModel.CanDeactivate);
        Assert.False(viewModel.CanDelete);
        Assert.Equal(
            [
                FamilyMemberRelationValues.Self,
                FamilyMemberRelationValues.Husband,
                FamilyMemberRelationValues.Son,
                FamilyMemberRelationValues.Daughter,
                FamilyMemberRelationValues.Father,
                FamilyMemberRelationValues.Mother,
                FamilyMemberRelationValues.YoungerSibling,
                FamilyMemberRelationValues.Grandmother,
                FamilyMemberRelationValues.Grandfather,
                FamilyMemberRelationValues.Other
            ],
            viewModel.RelationOptions);
        Assert.DoesNotContain("본인 후보", viewModel.RelationOptions);
        Assert.DoesNotContain("가족 후보", viewModel.RelationOptions);

        viewModel.DisplayName = "synthetic family";
        viewModel.SelectedRelation = "가족 후보";

        Assert.False(viewModel.CanSave);

        viewModel.SelectedRelation = FamilyMemberRelationValues.Mother;

        Assert.True(viewModel.CanSave);
        Assert.False(viewModel.CanDeactivate);
    }

    [Fact]
    public async Task Save_in_create_mode_uses_create_then_refreshes_without_display_name_identity_lookup()
    {
        var created = CreateRecord();
        var storage = new StubStorageService
        {
            CreateHandler = (draft, _) =>
            {
                Assert.Equal("synthetic family", draft.DisplayName);
                return Task.FromResult(created);
            },
            ActiveMembers = [created]
        };
        var viewModel = CreateViewModel(storage);
        SetValidInput(viewModel);

        Assert.True(await viewModel.SaveAsync());

        Assert.Equal(1, storage.CreateCalls);
        Assert.Equal(0, storage.UpdateCalls);
        Assert.True(viewModel.IsEditMode);
        Assert.Equal(created.Id, viewModel.EditingTargetId);
        Assert.Equal(created.Version, viewModel.ExpectedVersion);
        Assert.Equal("saved", viewModel.ManagementMessage);
        Assert.Equal(created, Assert.Single(viewModel.AvailableMembers));
    }

    [Fact]
    public async Task Prepare_edit_and_save_use_explicit_id_and_version()
    {
        var current = CreateRecord();
        var updated = current with
        {
            DisplayName = "updated synthetic",
            UpdatedAt = current.UpdatedAt.AddMinutes(1),
            Version = 2
        };
        var storage = new StubStorageService
        {
            MemberById = current,
            UpdateHandler = (id, version, _, _) =>
            {
                Assert.Equal(current.Id, id);
                Assert.Equal(current.Version, version);
                return Task.FromResult(updated);
            },
            ActiveMembers = [updated]
        };
        var viewModel = CreateViewModel(storage);

        Assert.True(await viewModel.PrepareEditAsync(current.Id, current.Version));
        viewModel.DisplayName = updated.DisplayName;
        Assert.True(await viewModel.SaveAsync());

        Assert.Equal(0, storage.CreateCalls);
        Assert.Equal(1, storage.UpdateCalls);
        Assert.Equal(updated.Version, viewModel.ExpectedVersion);
    }

    [Fact]
    public async Task Prepare_edit_rejects_stale_version_without_write()
    {
        var current = CreateRecord() with { Version = 2 };
        var viewModel = CreateViewModel(new StubStorageService { MemberById = current });

        Assert.False(await viewModel.PrepareEditAsync(current.Id, 1));

        Assert.False(viewModel.IsEditMode);
        Assert.Equal("conflict", viewModel.ManagementMessage);
    }

    [Fact]
    public async Task Version_conflict_maps_to_safe_message_and_is_non_write()
    {
        var current = CreateRecord();
        var storage = new StubStorageService
        {
            MemberById = current,
            UpdateHandler = (_, _, _, _) => throw new FamilyMemberStorageException(
                FamilyMemberStorageErrorCode.VersionConflict,
                "internal family_private_raw_id")
        };
        var viewModel = CreateViewModel(storage);
        await viewModel.PrepareEditAsync(current.Id, current.Version);

        Assert.False(await viewModel.SaveAsync());

        Assert.Equal("conflict", viewModel.ManagementMessage);
        Assert.DoesNotContain(current.Id, viewModel.ManagementMessage, StringComparison.Ordinal);
        Assert.Equal(1, storage.UpdateCalls);
    }

    [Fact]
    public async Task Deactivate_current_uses_explicit_target_and_resets_editor()
    {
        var current = CreateRecord();
        var deactivated = current with
        {
            DisabledAt = current.UpdatedAt.AddMinutes(1),
            UpdatedAt = current.UpdatedAt.AddMinutes(1),
            Version = 2
        };
        var storage = new StubStorageService
        {
            MemberById = current,
            DeactivateHandler = (id, version, _) =>
            {
                Assert.Equal(current.Id, id);
                Assert.Equal(current.Version, version);
                return Task.FromResult(deactivated);
            },
            ActiveMembers = [deactivated]
        };
        var viewModel = CreateViewModel(storage);
        await viewModel.PrepareEditAsync(current.Id, current.Version);

        Assert.True(await viewModel.DeactivateCurrentAsync());

        Assert.Equal(1, storage.DeactivateCalls);
        Assert.False(viewModel.IsEditMode);
        Assert.False(viewModel.CanDeactivate);
        var retained = Assert.Single(viewModel.AvailableMembers);
        Assert.Equal(deactivated, retained);
        Assert.NotNull(retained.DisabledAt);
        Assert.Equal("deactivated", viewModel.ManagementMessage);
    }

    [Fact]
    public async Task Reactivate_inactive_uses_explicit_target_and_refreshes_management_list()
    {
        var inactive = CreateRecord() with
        {
            DisabledAt = DateTimeOffset.UtcNow,
            Version = 2
        };
        var reactivated = inactive with
        {
            DisabledAt = null,
            UpdatedAt = inactive.UpdatedAt.AddMinutes(1),
            Version = 3
        };
        var storage = new StubStorageService
        {
            ReactivateHandler = (id, version, _) =>
            {
                Assert.Equal(inactive.Id, id);
                Assert.Equal(inactive.Version, version);
                return Task.FromResult(reactivated);
            },
            ActiveMembers = [reactivated]
        };
        var viewModel = CreateViewModel(storage);

        Assert.True(await viewModel.ReactivateAsync(inactive.Id, inactive.Version));

        Assert.Equal(1, storage.ReactivateCalls);
        Assert.Equal(reactivated, Assert.Single(viewModel.AvailableMembers));
        Assert.Equal("reactivated", viewModel.ManagementMessage);
    }

    [Fact]
    public async Task Busy_reentry_rejects_second_create_without_queueing_duplicate_write()
    {
        var entered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var created = CreateRecord();
        var storage = new StubStorageService
        {
            CreateHandler = async (_, _) =>
            {
                entered.SetResult();
                await release.Task;
                return created;
            },
            ActiveMembers = [created]
        };
        var viewModel = CreateViewModel(storage);
        SetValidInput(viewModel);

        var first = viewModel.SaveAsync();
        await entered.Task;
        var second = await viewModel.SaveAsync();
        release.SetResult();

        Assert.False(second);
        Assert.True(await first);
        Assert.Equal(1, storage.CreateCalls);
    }

    [Fact]
    public async Task Successful_create_with_refresh_failure_preserves_edit_token_and_retry_updates()
    {
        var created = CreateRecord();
        var updated = created with { Version = 2 };
        var storage = new StubStorageService
        {
            CreateHandler = (_, _) => Task.FromResult(created),
            UpdateHandler = (_, version, _, _) =>
            {
                Assert.Equal(created.Version, version);
                return Task.FromResult(updated);
            },
            ActiveLoadException = new IOException("synthetic refresh failure")
        };
        var viewModel = CreateViewModel(storage);
        SetValidInput(viewModel);

        Assert.True(await viewModel.SaveAsync());

        Assert.Equal(1, storage.CreateCalls);
        Assert.Equal(created.Id, viewModel.EditingTargetId);
        Assert.Equal(created.Version, viewModel.ExpectedVersion);
        Assert.Equal("saved-refresh-failed", viewModel.ManagementMessage);

        storage.ActiveLoadException = null;
        storage.ActiveMembers = [updated];
        Assert.True(await viewModel.SaveAsync());
        Assert.Equal(1, storage.CreateCalls);
        Assert.Equal(1, storage.UpdateCalls);
        Assert.Equal(updated.Version, viewModel.ExpectedVersion);
    }

    [Fact]
    public async Task Successful_update_with_refresh_failure_preserves_new_version_for_retry()
    {
        var current = CreateRecord();
        var firstUpdate = current with { Version = 2 };
        var secondUpdate = current with { Version = 3 };
        var seenVersions = new List<int>();
        var storage = new StubStorageService
        {
            MemberById = current,
            UpdateHandler = (_, version, _, _) =>
            {
                seenVersions.Add(version);
                return Task.FromResult(version == 1 ? firstUpdate : secondUpdate);
            }
        };
        var viewModel = CreateViewModel(storage);
        Assert.True(await viewModel.PrepareEditAsync(current.Id, current.Version));
        storage.ActiveLoadException = new IOException("synthetic refresh failure");

        Assert.True(await viewModel.SaveAsync());
        Assert.Equal(2, viewModel.ExpectedVersion);
        Assert.Equal("saved-refresh-failed", viewModel.ManagementMessage);

        storage.ActiveLoadException = null;
        storage.ActiveMembers = [secondUpdate];
        Assert.True(await viewModel.SaveAsync());
        Assert.Equal([1, 2], seenVersions);
        Assert.Equal(3, viewModel.ExpectedVersion);
    }

    [Fact]
    public async Task Successful_deactivate_with_refresh_failure_cannot_repeat_current_target()
    {
        var current = CreateRecord();
        var storage = new StubStorageService
        {
            MemberById = current,
            DeactivateHandler = (_, _, _) => Task.FromResult(current with
            {
                DisabledAt = current.UpdatedAt.AddMinutes(1),
                Version = 2
            })
        };
        var viewModel = CreateViewModel(storage);
        Assert.True(await viewModel.PrepareEditAsync(current.Id, current.Version));
        storage.ActiveLoadException = new IOException("synthetic refresh failure");

        Assert.True(await viewModel.DeactivateCurrentAsync());
        Assert.False(viewModel.IsEditMode);
        Assert.Equal("saved-refresh-failed", viewModel.ManagementMessage);
        Assert.False(await viewModel.DeactivateCurrentAsync());
        Assert.Equal(1, storage.DeactivateCalls);
    }

    [Fact]
    public async Task Cancellation_before_operation_does_not_call_storage()
    {
        var storage = new StubStorageService();
        var viewModel = CreateViewModel(storage);
        SetValidInput(viewModel);
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
            viewModel.SaveAsync(cancellation.Token));

        Assert.Equal(0, storage.CreateCalls);
        Assert.Equal(0, storage.UpdateCalls);
    }

    [Fact]
    public async Task General_failure_message_does_not_expose_exception_payload_or_paths()
    {
        var storage = new StubStorageService
        {
            CreateHandler = (_, _) => throw new IOException(
                @"C:\private\family_private_raw_id\synthetic memo")
        };
        var viewModel = CreateViewModel(storage);
        SetValidInput(viewModel);

        Assert.False(await viewModel.SaveAsync());

        Assert.Equal("operation-failed", viewModel.ManagementMessage);
        Assert.DoesNotContain("private", viewModel.ManagementMessage, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("synthetic memo", viewModel.ManagementMessage, StringComparison.Ordinal);
    }

    private static FamilyMemberManagementViewModel CreateViewModel(StubStorageService storage)
    {
        return new FamilyMemberManagementViewModel(storage, CreateTextProvider());
    }

    private static void SetValidInput(FamilyMemberManagementViewModel viewModel)
    {
        viewModel.DisplayName = "synthetic family";
        viewModel.SelectedRelation = FamilyMemberRelationValues.Mother;
        viewModel.Memo = "synthetic memo";
    }

    private static FamilyMemberRecord CreateRecord()
    {
        var timestamp = new DateTimeOffset(2026, 8, 3, 0, 0, 0, TimeSpan.Zero);
        return new FamilyMemberRecord(
            "family_synthetic_id",
            "synthetic family",
            FamilyMemberRelationValues.Mother,
            "synthetic memo",
            timestamp,
            timestamp,
            null,
            1);
    }

    private static IUiTextProvider CreateTextProvider()
    {
        return new ResourceUiTextProvider(new Dictionary<string, string>(StringComparer.Ordinal)
        {
            [UiTextKeys.ProductFamilyMemberLoadFailedMessage] = "load-failed",
            [UiTextKeys.ProductFamilyMemberSavedMessage] = "saved",
            [UiTextKeys.ProductFamilyMemberDeactivatedMessage] = "deactivated",
            [UiTextKeys.ProductFamilyMemberReactivatedMessage] = "reactivated",
            [UiTextKeys.ProductFamilyMemberDisplayNameRequiredMessage] = "display-required",
            [UiTextKeys.ProductFamilyMemberRelationRequiredMessage] = "relation-required",
            [UiTextKeys.ProductFamilyMemberConflictMessage] = "conflict",
            [UiTextKeys.ProductFamilyMemberTargetUnavailableMessage] = "target-unavailable",
            [UiTextKeys.ProductFamilyMemberOperationFailedMessage] = "operation-failed",
            [UiTextKeys.ProductFamilyMemberSavedRefreshFailedMessage] = "saved-refresh-failed"
        });
    }

    private sealed class StubStorageService : IFamilyMemberStorageService
    {
        public IReadOnlyList<FamilyMemberRecord> ActiveMembers { get; set; } = [];

        public FamilyMemberRecord? MemberById { get; init; }

        public Exception? ActiveLoadException { get; set; }

        public Func<FamilyMemberDraft, CancellationToken, Task<FamilyMemberRecord>>? CreateHandler { get; init; }

        public Func<string, int, FamilyMemberDraft, CancellationToken, Task<FamilyMemberRecord>>?
            UpdateHandler { get; init; }

        public Func<string, int, CancellationToken, Task<FamilyMemberRecord>>?
            DeactivateHandler { get; init; }

        public Func<string, int, CancellationToken, Task<FamilyMemberRecord>>?
            ReactivateHandler { get; init; }

        public int CreateCalls { get; private set; }

        public int UpdateCalls { get; private set; }

        public int DeactivateCalls { get; private set; }

        public int ReactivateCalls { get; private set; }

        public Task<IReadOnlyList<FamilyMemberRecord>> GetActiveFamilyMembersAsync(
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (ActiveLoadException is not null)
            {
                throw ActiveLoadException;
            }

            return Task.FromResult<IReadOnlyList<FamilyMemberRecord>>(
                ActiveMembers.Where(member => member.DisabledAt is null).ToList());
        }

        public Task<IReadOnlyList<FamilyMemberRecord>> GetFamilyMembersAsync(
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (ActiveLoadException is not null)
            {
                throw ActiveLoadException;
            }

            return Task.FromResult(ActiveMembers);
        }

        public Task<FamilyMemberRecord?> GetFamilyMemberAsync(
            string id,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(MemberById);
        }

        public Task<FamilyMemberRecord> CreateFamilyMemberAsync(
            FamilyMemberDraft draft,
            CancellationToken cancellationToken = default)
        {
            CreateCalls++;
            return CreateHandler is null
                ? Task.FromResult(CreateRecord())
                : CreateHandler(draft, cancellationToken);
        }

        public Task<FamilyMemberRecord> UpdateFamilyMemberAsync(
            string id,
            int expectedVersion,
            FamilyMemberDraft draft,
            CancellationToken cancellationToken = default)
        {
            UpdateCalls++;
            return UpdateHandler is null
                ? Task.FromResult(CreateRecord() with { Id = id, Version = expectedVersion + 1 })
                : UpdateHandler(id, expectedVersion, draft, cancellationToken);
        }

        public Task<FamilyMemberRecord> DeactivateFamilyMemberAsync(
            string id,
            int expectedVersion,
            CancellationToken cancellationToken = default)
        {
            DeactivateCalls++;
            return DeactivateHandler is null
                ? Task.FromResult(CreateRecord() with
                {
                    Id = id,
                    DisabledAt = DateTimeOffset.UtcNow,
                    Version = expectedVersion + 1
                })
                : DeactivateHandler(id, expectedVersion, cancellationToken);
        }

        public Task<FamilyMemberRecord> ReactivateFamilyMemberAsync(
            string id,
            int expectedVersion,
            CancellationToken cancellationToken = default)
        {
            ReactivateCalls++;
            return ReactivateHandler is null
                ? Task.FromResult(CreateRecord() with
                {
                    Id = id,
                    DisabledAt = null,
                    Version = expectedVersion + 1
                })
                : ReactivateHandler(id, expectedVersion, cancellationToken);
        }
    }
}
