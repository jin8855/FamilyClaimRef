using System.ComponentModel;
using System.Runtime.CompilerServices;
using FamilyClaimRef.App.Models.Storage;
using FamilyClaimRef.App.Services.Localization;
using FamilyClaimRef.App.Services.Storage;

namespace FamilyClaimRef.App.ViewModels;

public sealed class FamilyMemberManagementViewModel : INotifyPropertyChanged
{
    private readonly IFamilyMemberStorageService storageService;
    private readonly IUiTextProvider uiTextProvider;
    private readonly SemaphoreSlim operationGate = new(1, 1);

    private IReadOnlyList<FamilyMemberRecord> availableMembers = [];
    private string? editingTargetId;
    private int expectedVersion;
    private string? displayName;
    private string? selectedRelation;
    private string? memo;
    private string? managementMessage;
    private bool isBusy;

    public FamilyMemberManagementViewModel(
        IFamilyMemberStorageService storageService,
        IUiTextProvider uiTextProvider)
    {
        this.storageService = storageService
            ?? throw new ArgumentNullException(nameof(storageService));
        this.uiTextProvider = uiTextProvider
            ?? throw new ArgumentNullException(nameof(uiTextProvider));
        RelationOptions = FamilyMemberRelationValues.All;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public IReadOnlyList<FamilyMemberRecord> AvailableMembers
    {
        get => availableMembers;
        private set
        {
            if (SetProperty(ref availableMembers, value))
            {
                OnPropertyChanged(nameof(HasAvailableMembers));
            }
        }
    }

    public IReadOnlyList<string> RelationOptions { get; }

    public bool HasAvailableMembers => AvailableMembers.Count > 0;

    public string? DisplayName
    {
        get => displayName;
        set
        {
            if (SetProperty(ref displayName, value))
            {
                OnCommandStateChanged();
            }
        }
    }

    public string? SelectedRelation
    {
        get => selectedRelation;
        set
        {
            if (SetProperty(ref selectedRelation, value))
            {
                OnCommandStateChanged();
            }
        }
    }

    public string? Memo
    {
        get => memo;
        set => SetProperty(ref memo, value);
    }

    public string? ManagementMessage
    {
        get => managementMessage;
        private set => SetProperty(ref managementMessage, value);
    }

    public bool IsBusy
    {
        get => isBusy;
        private set
        {
            if (SetProperty(ref isBusy, value))
            {
                OnCommandStateChanged();
            }
        }
    }

    public bool IsEditMode => editingTargetId is not null;

    public bool CanSave =>
        !IsBusy
        && !string.IsNullOrWhiteSpace(DisplayName)
        && SelectedRelation is not null
        && FamilyMemberRelationValues.IsSupported(SelectedRelation);

    public bool CanDeactivate =>
        !IsBusy
        && editingTargetId is not null
        && expectedVersion > 0;

    public bool CanDelete => false;

    public bool CanUseRowActions => !IsBusy;

    internal string? EditingTargetId => editingTargetId;

    internal int ExpectedVersion => expectedVersion;

    public void ClearManagementMessage()
    {
        ManagementMessage = null;
    }

    public void BeginCreate()
    {
        if (IsBusy)
        {
            return;
        }

        ResetEditor();
        ManagementMessage = null;
    }

    public async Task<bool> LoadAsync(CancellationToken cancellationToken = default)
    {
        if (!await TryEnterOperationAsync(cancellationToken))
        {
            return false;
        }

        try
        {
            AvailableMembers = await storageService.GetFamilyMembersAsync(cancellationToken);
            return true;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch
        {
            ManagementMessage = uiTextProvider.Get(UiTextKeys.ProductFamilyMemberLoadFailedMessage);
            return false;
        }
        finally
        {
            ExitOperation();
        }
    }

    public async Task<bool> PrepareEditAsync(
        string id,
        int version,
        CancellationToken cancellationToken = default)
    {
        if (!await TryEnterOperationAsync(cancellationToken))
        {
            return false;
        }

        try
        {
            var record = await storageService.GetFamilyMemberAsync(id, cancellationToken);
            if (record is null || record.DisabledAt is not null)
            {
                ManagementMessage = uiTextProvider.Get(
                    UiTextKeys.ProductFamilyMemberTargetUnavailableMessage);
                return false;
            }

            if (record.Version != version)
            {
                ManagementMessage = uiTextProvider.Get(
                    UiTextKeys.ProductFamilyMemberConflictMessage);
                return false;
            }

            editingTargetId = record.Id;
            expectedVersion = record.Version;
            DisplayName = record.DisplayName;
            SelectedRelation = record.Relation;
            Memo = record.Memo;
            ManagementMessage = null;
            OnEditorModeChanged();
            return true;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch
        {
            ManagementMessage = uiTextProvider.Get(
                UiTextKeys.ProductFamilyMemberOperationFailedMessage);
            return false;
        }
        finally
        {
            ExitOperation();
        }
    }

    public async Task<bool> SaveAsync(CancellationToken cancellationToken = default)
    {
        if (!await TryEnterOperationAsync(cancellationToken))
        {
            return false;
        }

        try
        {
            if (string.IsNullOrWhiteSpace(DisplayName))
            {
                ManagementMessage = uiTextProvider.Get(
                    UiTextKeys.ProductFamilyMemberDisplayNameRequiredMessage);
                return false;
            }

            if (SelectedRelation is null
                || !FamilyMemberRelationValues.IsSupported(SelectedRelation))
            {
                ManagementMessage = uiTextProvider.Get(
                    UiTextKeys.ProductFamilyMemberRelationRequiredMessage);
                return false;
            }

            var draft = new FamilyMemberDraft(DisplayName, SelectedRelation, Memo);
            FamilyMemberRecord saved;
            if (editingTargetId is null)
            {
                saved = await storageService.CreateFamilyMemberAsync(draft, cancellationToken);
            }
            else
            {
                saved = await storageService.UpdateFamilyMemberAsync(
                    editingTargetId,
                    expectedVersion,
                    draft,
                    cancellationToken);
            }

            editingTargetId = saved.Id;
            expectedVersion = saved.Version;
            DisplayName = saved.DisplayName;
            SelectedRelation = saved.Relation;
            Memo = saved.Memo;
            OnEditorModeChanged();

            return await RefreshAfterMutationAsync(
                UiTextKeys.ProductFamilyMemberSavedMessage,
                cancellationToken);
        }
        catch (FamilyMemberStorageException exception)
        {
            ManagementMessage = uiTextProvider.Get(
                exception.ErrorCode == FamilyMemberStorageErrorCode.VersionConflict
                    ? UiTextKeys.ProductFamilyMemberConflictMessage
                    : UiTextKeys.ProductFamilyMemberTargetUnavailableMessage);
            return false;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch
        {
            ManagementMessage = uiTextProvider.Get(
                UiTextKeys.ProductFamilyMemberOperationFailedMessage);
            return false;
        }
        finally
        {
            ExitOperation();
        }
    }

    public Task<bool> DeactivateCurrentAsync(CancellationToken cancellationToken = default)
    {
        if (editingTargetId is null || expectedVersion <= 0)
        {
            ManagementMessage = uiTextProvider.Get(
                UiTextKeys.ProductFamilyMemberTargetUnavailableMessage);
            return Task.FromResult(false);
        }

        return DeactivateAsync(editingTargetId, expectedVersion, cancellationToken);
    }

    public async Task<bool> DeactivateAsync(
        string id,
        int version,
        CancellationToken cancellationToken = default)
    {
        if (!await TryEnterOperationAsync(cancellationToken))
        {
            return false;
        }

        try
        {
            await storageService.DeactivateFamilyMemberAsync(id, version, cancellationToken);
            if (string.Equals(editingTargetId, id, StringComparison.Ordinal))
            {
                ResetEditor();
            }

            return await RefreshAfterMutationAsync(
                UiTextKeys.ProductFamilyMemberDeactivatedMessage,
                cancellationToken);
        }
        catch (FamilyMemberStorageException exception)
        {
            ManagementMessage = uiTextProvider.Get(
                exception.ErrorCode == FamilyMemberStorageErrorCode.VersionConflict
                    ? UiTextKeys.ProductFamilyMemberConflictMessage
                    : UiTextKeys.ProductFamilyMemberTargetUnavailableMessage);
            return false;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch
        {
            ManagementMessage = uiTextProvider.Get(
                UiTextKeys.ProductFamilyMemberOperationFailedMessage);
            return false;
        }
        finally
        {
            ExitOperation();
        }
    }

    public async Task<bool> ReactivateAsync(
        string id,
        int version,
        CancellationToken cancellationToken = default)
    {
        if (!await TryEnterOperationAsync(cancellationToken))
        {
            return false;
        }

        try
        {
            await storageService.ReactivateFamilyMemberAsync(id, version, cancellationToken);
            return await RefreshAfterMutationAsync(
                UiTextKeys.ProductFamilyMemberReactivatedMessage,
                cancellationToken);
        }
        catch (FamilyMemberStorageException exception)
        {
            ManagementMessage = uiTextProvider.Get(
                exception.ErrorCode == FamilyMemberStorageErrorCode.VersionConflict
                    ? UiTextKeys.ProductFamilyMemberConflictMessage
                    : UiTextKeys.ProductFamilyMemberTargetUnavailableMessage);
            return false;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch
        {
            ManagementMessage = uiTextProvider.Get(
                UiTextKeys.ProductFamilyMemberOperationFailedMessage);
            return false;
        }
        finally
        {
            ExitOperation();
        }
    }

    private async Task<bool> RefreshAfterMutationAsync(
        string successMessageKey,
        CancellationToken cancellationToken)
    {
        try
        {
            AvailableMembers = await storageService.GetFamilyMembersAsync(cancellationToken);
            ManagementMessage = uiTextProvider.Get(successMessageKey);
        }
        catch
        {
            ManagementMessage = uiTextProvider.Get(
                UiTextKeys.ProductFamilyMemberSavedRefreshFailedMessage);
        }

        return true;
    }

    private async Task<bool> TryEnterOperationAsync(CancellationToken cancellationToken)
    {
        var entered = await operationGate.WaitAsync(0, cancellationToken);
        if (!entered)
        {
            return false;
        }

        IsBusy = true;
        return true;
    }

    private void ExitOperation()
    {
        IsBusy = false;
        operationGate.Release();
    }

    private void ResetEditor()
    {
        editingTargetId = null;
        expectedVersion = 0;
        DisplayName = null;
        SelectedRelation = null;
        Memo = null;
        OnEditorModeChanged();
    }

    private void OnEditorModeChanged()
    {
        OnPropertyChanged(nameof(IsEditMode));
        OnPropertyChanged(nameof(CanDeactivate));
    }

    private void OnCommandStateChanged()
    {
        OnPropertyChanged(nameof(CanSave));
        OnPropertyChanged(nameof(CanDeactivate));
        OnPropertyChanged(nameof(CanDelete));
        OnPropertyChanged(nameof(CanUseRowActions));
    }

    private bool SetProperty<T>(
        ref T field,
        T value,
        [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return false;
        }

        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
