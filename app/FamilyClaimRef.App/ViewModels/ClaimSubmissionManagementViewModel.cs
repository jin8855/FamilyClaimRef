using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using FamilyClaimRef.App.Models.Storage;
using FamilyClaimRef.App.Services.Localization;
using FamilyClaimRef.App.Services.Storage;

namespace FamilyClaimRef.App.ViewModels;

public sealed class ClaimSubmissionManagementViewModel : INotifyPropertyChanged
{
    private readonly IClaimSubmissionStorageService submissionStorageService;
    private readonly IClaimCaseStorageService claimCaseStorageService;
    private readonly IDocumentStorageService documentStorageService;
    private readonly IUiTextProvider uiTextProvider;
    private readonly SemaphoreSlim operationGate = new(1, 1);
    private readonly Dictionary<string, ClaimSubmissionRecord> recordsById =
        new(StringComparer.Ordinal);

    private IReadOnlyList<ClaimSubmissionClaimCaseOptionViewModel> availableClaimCases = [];
    private IReadOnlyList<ClaimSubmissionPolicyOptionViewModel> availablePolicies = [];
    private IReadOnlyList<ClaimSubmissionListItemViewModel> submissions = [];
    private IReadOnlyList<ClaimSubmissionDocumentOptionViewModel> availableDocuments = [];
    private IReadOnlyList<ClaimSubmissionStatusOptionViewModel> availableStatusOptions = [];
    private string? selectedClaimCaseId;
    private string? selectedPolicyId;
    private string? selectedSubmissionId;
    private string? coverageDisplayName;
    private DateTime? submittedDate;
    private string? submittedAmountText;
    private string? selectedStatus;
    private string? memo;
    private int? editingRevision;
    private string? editingStatus;
    private bool isBusy;
    private bool hasUnsavedChanges;
    private bool isApplying;
    private string? validationMessage;
    private string? conflictMessage;
    private string? legacyReviewMessage;
    private string? referenceMessage;
    private string? transitionMessage;
    private string? operationMessage;

    public ClaimSubmissionManagementViewModel(
        IClaimSubmissionStorageService submissionStorageService,
        IClaimCaseStorageService claimCaseStorageService,
        IDocumentStorageService documentStorageService,
        IUiTextProvider uiTextProvider)
    {
        this.submissionStorageService = submissionStorageService
            ?? throw new ArgumentNullException(nameof(submissionStorageService));
        this.claimCaseStorageService = claimCaseStorageService
            ?? throw new ArgumentNullException(nameof(claimCaseStorageService));
        this.documentStorageService = documentStorageService
            ?? throw new ArgumentNullException(nameof(documentStorageService));
        this.uiTextProvider = uiTextProvider
            ?? throw new ArgumentNullException(nameof(uiTextProvider));
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public IReadOnlyList<ClaimSubmissionClaimCaseOptionViewModel> AvailableClaimCases
    {
        get => availableClaimCases;
        private set => SetProperty(ref availableClaimCases, value);
    }

    public IReadOnlyList<ClaimSubmissionPolicyOptionViewModel> AvailablePolicies
    {
        get => availablePolicies;
        private set => SetProperty(ref availablePolicies, value);
    }

    public IReadOnlyList<ClaimSubmissionListItemViewModel> Submissions
    {
        get => submissions;
        private set
        {
            if (SetProperty(ref submissions, value))
            {
                OnPropertyChanged(nameof(IsSubmissionListEmpty));
            }
        }
    }

    public IReadOnlyList<ClaimSubmissionDocumentOptionViewModel> AvailableDocuments
    {
        get => availableDocuments;
        private set => SetProperty(ref availableDocuments, value);
    }

    public IReadOnlyList<ClaimSubmissionStatusOptionViewModel> AvailableStatusOptions
    {
        get => availableStatusOptions;
        private set => SetProperty(ref availableStatusOptions, value);
    }

    public string? SelectedClaimCaseId
    {
        get => selectedClaimCaseId;
        set
        {
            if (SetProperty(ref selectedClaimCaseId, value))
            {
                OnCommandStateChanged();
            }
        }
    }

    public string? SelectedPolicyId
    {
        get => selectedPolicyId;
        set
        {
            if (SetProperty(ref selectedPolicyId, value))
            {
                MarkEditorChanged();
            }
        }
    }

    public string? SelectedSubmissionId
    {
        get => selectedSubmissionId;
        set
        {
            if (SetProperty(ref selectedSubmissionId, value))
            {
                OnCommandStateChanged();
            }
        }
    }

    public string? CoverageDisplayName
    {
        get => coverageDisplayName;
        set
        {
            if (SetProperty(ref coverageDisplayName, value))
            {
                MarkEditorChanged();
            }
        }
    }

    public DateTime? SubmittedDate
    {
        get => submittedDate;
        set
        {
            if (SetProperty(ref submittedDate, value))
            {
                MarkEditorChanged();
            }
        }
    }

    public string? SubmittedAmountText
    {
        get => submittedAmountText;
        set
        {
            if (SetProperty(ref submittedAmountText, value))
            {
                MarkEditorChanged();
            }
        }
    }

    public string? SelectedStatus
    {
        get => selectedStatus;
        set
        {
            if (SetProperty(ref selectedStatus, value))
            {
                MarkEditorChanged();
            }
        }
    }

    public string? Memo
    {
        get => memo;
        set
        {
            if (SetProperty(ref memo, value))
            {
                MarkEditorChanged();
            }
        }
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

    public bool HasUnsavedChanges
    {
        get => hasUnsavedChanges;
        private set
        {
            if (SetProperty(ref hasUnsavedChanges, value))
            {
                OnCommandStateChanged();
            }
        }
    }

    public bool IsEditMode => editingRevision is not null;

    public bool IsSubmissionListEmpty => Submissions.Count == 0;

    public bool CanCreate =>
        !IsBusy
        && !IsEditMode
        && !string.IsNullOrWhiteSpace(SelectedClaimCaseId)
        && !string.IsNullOrWhiteSpace(SelectedPolicyId);

    public bool CanSave =>
        !IsBusy
        && IsEditMode
        && HasUnsavedChanges
        && !ClaimSubmissionValues.IsTerminal(editingStatus ?? string.Empty);

    public bool CanEditDetails =>
        !IsBusy
        && (editingStatus is null
            || !ClaimSubmissionValues.IsTerminal(editingStatus));

    public bool CanNavigateAway => !IsBusy && !HasUnsavedChanges;

    public string? ValidationMessage
    {
        get => validationMessage;
        private set => SetProperty(ref validationMessage, value);
    }

    public string? ConflictMessage
    {
        get => conflictMessage;
        private set => SetProperty(ref conflictMessage, value);
    }

    public string? LegacyReviewMessage
    {
        get => legacyReviewMessage;
        private set => SetProperty(ref legacyReviewMessage, value);
    }

    public string? ReferenceMessage
    {
        get => referenceMessage;
        private set => SetProperty(ref referenceMessage, value);
    }

    public string? TransitionMessage
    {
        get => transitionMessage;
        private set => SetProperty(ref transitionMessage, value);
    }

    public string? OperationMessage
    {
        get => operationMessage;
        private set => SetProperty(ref operationMessage, value);
    }

    public async Task<bool> LoadAsync(CancellationToken cancellationToken = default)
    {
        if (!await operationGate.WaitAsync(0, cancellationToken))
        {
            return false;
        }

        try
        {
            IsBusy = true;
            ClearMessages();
            var claims = await claimCaseStorageService.GetClaimCasesAsync(cancellationToken);
            AvailableClaimCases = claims
                .Where(claim => string.Equals(
                    claim.CaseStatus,
                    ClaimCaseValues.StatusSaved,
                    StringComparison.Ordinal))
                .OrderBy(claim => claim.DisplayTitle, StringComparer.Ordinal)
                .Select(claim => new ClaimSubmissionClaimCaseOptionViewModel(
                    claim.Id,
                    claim.DisplayTitle))
                .ToList();

            if (string.IsNullOrWhiteSpace(SelectedClaimCaseId))
            {
                SelectedClaimCaseId = AvailableClaimCases.FirstOrDefault()?.Id;
            }

            if (string.IsNullOrWhiteSpace(SelectedClaimCaseId)
                || !AvailableClaimCases.Any(option => string.Equals(
                    option.Id,
                    SelectedClaimCaseId,
                    StringComparison.Ordinal)))
            {
                ResetContext();
                SetReferenceMessage();
                return false;
            }

            await LoadClaimContextCoreAsync(cancellationToken);
            return true;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (ClaimSubmissionLegacyReviewRequiredException)
        {
            SetLegacyReviewMessage();
            return false;
        }
        catch (ClaimSubmissionReferenceException)
        {
            SetReferenceMessage();
            return false;
        }
        catch
        {
            OperationMessage = uiTextProvider.Get(
                UiTextKeys.ProductClaimSubmissionOperationFailedMessage);
            return false;
        }
        finally
        {
            IsBusy = false;
            operationGate.Release();
        }
    }

    public async Task<bool> LoadClaimContextAsync(
        CancellationToken cancellationToken = default)
    {
        if (!await operationGate.WaitAsync(0, cancellationToken))
        {
            return false;
        }

        try
        {
            IsBusy = true;
            ClearMessages();
            if (string.IsNullOrWhiteSpace(SelectedClaimCaseId))
            {
                ResetContext();
                return false;
            }

            await LoadClaimContextCoreAsync(cancellationToken);
            return true;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (ClaimSubmissionLegacyReviewRequiredException)
        {
            ResetContext();
            SetLegacyReviewMessage();
            return false;
        }
        catch (ClaimSubmissionReferenceException)
        {
            ResetContext();
            SetReferenceMessage();
            return false;
        }
        catch
        {
            ResetContext();
            OperationMessage = uiTextProvider.Get(
                UiTextKeys.ProductClaimSubmissionOperationFailedMessage);
            return false;
        }
        finally
        {
            IsBusy = false;
            operationGate.Release();
        }
    }

    public void StartNew()
    {
        ClearMessages();
        ApplyEditorState(() =>
        {
            SelectedSubmissionId = null;
            editingRevision = null;
            editingStatus = null;
            SelectedPolicyId = AvailablePolicies.FirstOrDefault()?.Id;
            CoverageDisplayName = null;
            SubmittedDate = null;
            SubmittedAmountText = null;
            SelectedStatus = ClaimSubmissionValues.StatusPreparing;
            Memo = null;
            foreach (var document in AvailableDocuments)
            {
                document.IsSelected = false;
            }

            AvailableStatusOptions = CreateStatusOptions(ClaimSubmissionValues.StatusPreparing);
            HasUnsavedChanges = false;
        });
        OnCommandStateChanged();
    }

    public bool LoadSelectedSubmission()
    {
        ClearMessages();
        if (string.IsNullOrWhiteSpace(SelectedSubmissionId)
            || !recordsById.TryGetValue(SelectedSubmissionId, out var record))
        {
            return false;
        }

        ApplyRecord(record);
        return true;
    }

    public async Task<bool> CreatePreparingAsync(
        CancellationToken cancellationToken = default)
    {
        if (!await operationGate.WaitAsync(0, cancellationToken))
        {
            return false;
        }

        try
        {
            IsBusy = true;
            ClearMessages();
            if (!TryCreateDraft(ClaimSubmissionValues.StatusPreparing, out var draft))
            {
                SetValidationMessage();
                return false;
            }

            var created = await submissionStorageService.CreateAsync(draft, cancellationToken);
            await ApplyCommittedRecordAsync(created);
            OperationMessage = uiTextProvider.Get(
                UiTextKeys.ProductClaimSubmissionCreatedMessage);
            return true;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (ClaimSubmissionLegacyReviewRequiredException)
        {
            SetLegacyReviewMessage();
            return false;
        }
        catch (ClaimSubmissionReferenceException)
        {
            SetReferenceMessage();
            return false;
        }
        catch (ArgumentException)
        {
            SetValidationMessage();
            return false;
        }
        catch
        {
            OperationMessage = uiTextProvider.Get(
                UiTextKeys.ProductClaimSubmissionOperationFailedMessage);
            return false;
        }
        finally
        {
            IsBusy = false;
            operationGate.Release();
        }
    }

    public async Task<bool> SaveAsync(CancellationToken cancellationToken = default)
    {
        if (editingRevision is null
            || string.IsNullOrWhiteSpace(SelectedSubmissionId)
            || !await operationGate.WaitAsync(0, cancellationToken))
        {
            return false;
        }

        try
        {
            IsBusy = true;
            ClearMessages();
            if (!TryCreateDraft(SelectedStatus, out var draft))
            {
                SetValidationMessage();
                return false;
            }

            var updated = await submissionStorageService.UpdateAsync(
                SelectedSubmissionId,
                editingRevision.Value,
                draft,
                cancellationToken);
            await ApplyCommittedRecordAsync(updated);
            OperationMessage = uiTextProvider.Get(
                UiTextKeys.ProductClaimSubmissionSavedMessage);
            return true;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (ClaimSubmissionConcurrencyException)
        {
            ConflictMessage = uiTextProvider.Get(
                UiTextKeys.ProductClaimSubmissionConflictMessage);
            return false;
        }
        catch (ClaimSubmissionLegacyReviewRequiredException)
        {
            SetLegacyReviewMessage();
            return false;
        }
        catch (ClaimSubmissionReferenceException)
        {
            SetReferenceMessage();
            return false;
        }
        catch (ClaimSubmissionTransitionException)
        {
            TransitionMessage = uiTextProvider.Get(
                UiTextKeys.ProductClaimSubmissionTransitionMessage);
            return false;
        }
        catch (ArgumentException)
        {
            SetValidationMessage();
            return false;
        }
        catch
        {
            OperationMessage = uiTextProvider.Get(
                UiTextKeys.ProductClaimSubmissionOperationFailedMessage);
            return false;
        }
        finally
        {
            IsBusy = false;
            operationGate.Release();
        }
    }

    private async Task LoadClaimContextCoreAsync(CancellationToken cancellationToken)
    {
        var claimCaseId = SelectedClaimCaseId!;
        var policies = await submissionStorageService.GetClaimablePoliciesAsync(
            claimCaseId,
            cancellationToken);
        AvailablePolicies = policies
            .Select(policy => new ClaimSubmissionPolicyOptionViewModel(
                policy.Id,
                policy.DisplayTitle,
                policy.InsurerName))
            .ToList();

        var links = (await documentStorageService.GetClaimDocumentsAsync(
                claimCaseId,
                cancellationToken))
            .Where(link => link.DisabledAt is null)
            .OrderBy(link => link.CreatedAt)
            .ToList();
        var documents = new List<ClaimSubmissionDocumentOptionViewModel>();
        foreach (var link in links)
        {
            var document = await documentStorageService.GetDocumentByIdAsync(
                link.DocumentId,
                cancellationToken);
            if (document is not null && document.DisabledAt is null)
            {
                documents.Add(new ClaimSubmissionDocumentOptionViewModel(
                    link.Id,
                    document.DisplayTitle,
                    link.DocumentType,
                    MarkEditorChanged));
            }
        }

        AvailableDocuments = documents;
        await RefreshSubmissionsAsync(cancellationToken);
        if (!string.IsNullOrWhiteSpace(SelectedSubmissionId)
            && recordsById.TryGetValue(SelectedSubmissionId, out var selected))
        {
            ApplyRecord(selected);
        }
        else
        {
            StartNew();
        }
    }

    private async Task RefreshSubmissionsAsync(CancellationToken cancellationToken)
    {
        var records = await submissionStorageService.GetByClaimCaseAsync(
            SelectedClaimCaseId!,
            cancellationToken);
        recordsById.Clear();
        foreach (var record in records)
        {
            recordsById.Add(record.Id, record);
        }

        RebuildSubmissionList();
    }

    private async Task ApplyCommittedRecordAsync(ClaimSubmissionRecord record)
    {
        recordsById[record.Id] = record;
        RebuildSubmissionList();
        ApplyRecord(record);

        try
        {
            await RefreshSubmissionsAsync(CancellationToken.None);
            if (recordsById.TryGetValue(record.Id, out var refreshed))
            {
                ApplyRecord(refreshed);
            }
        }
        catch
        {
            // The durable mutation succeeded; preserve its returned state when refresh fails.
        }
    }

    private void RebuildSubmissionList()
    {
        var policyTitles = AvailablePolicies.ToDictionary(
            option => option.Id,
            option => option.DisplayText,
            StringComparer.Ordinal);
        Submissions = recordsById.Values
            .OrderByDescending(record => record.UpdatedAt)
            .ThenBy(record => record.Id, StringComparer.Ordinal)
            .Select(record => new ClaimSubmissionListItemViewModel(
                record.Id,
                policyTitles.GetValueOrDefault(
                    record.PolicyId,
                    uiTextProvider.Get(UiTextKeys.ProductClaimSubmissionReferenceUnavailableValue)),
                record.CoverageDisplayName
                    ?? uiTextProvider.Get(UiTextKeys.ProductClaimSubmissionNotEnteredValue),
                GetStatusDisplay(record.Status),
                record.UpdatedAt.ToLocalTime().ToString("yyyy-MM-dd HH:mm", CultureInfo.CurrentCulture)))
            .ToList();
    }

    private void ApplyRecord(ClaimSubmissionRecord record)
    {
        ApplyEditorState(() =>
        {
            SelectedSubmissionId = record.Id;
            editingRevision = record.Revision;
            editingStatus = record.Status;
            SelectedPolicyId = record.PolicyId;
            CoverageDisplayName = record.CoverageDisplayName;
            SubmittedDate = record.SubmittedDate?.ToDateTime(TimeOnly.MinValue);
            SubmittedAmountText = record.SubmittedAmount?.ToString("N0", CultureInfo.CurrentCulture);
            SelectedStatus = record.Status;
            Memo = record.Memo;
            var selectedDocumentIds = record.SubmittedClaimDocumentIds.ToHashSet(StringComparer.Ordinal);
            foreach (var document in AvailableDocuments)
            {
                document.IsSelected = selectedDocumentIds.Contains(document.Id);
            }

            AvailableStatusOptions = CreateStatusOptions(record.Status);
            HasUnsavedChanges = false;
        });
        OnCommandStateChanged();
    }

    private IReadOnlyList<ClaimSubmissionStatusOptionViewModel> CreateStatusOptions(
        string currentStatus)
    {
        return new[] { currentStatus }
            .Concat(ClaimSubmissionValues.GetAllowedTargets(currentStatus))
            .Distinct(StringComparer.Ordinal)
            .Select(status => new ClaimSubmissionStatusOptionViewModel(
                status,
                GetStatusDisplay(status)))
            .ToList();
    }

    private string GetStatusDisplay(string status)
    {
        var key = status switch
        {
            ClaimSubmissionValues.StatusPreparing => UiTextKeys.ProductClaimSubmissionStatusPreparing,
            ClaimSubmissionValues.StatusSubmitted => UiTextKeys.ProductClaimSubmissionStatusSubmitted,
            ClaimSubmissionValues.StatusAdditionalDocumentsRequested =>
                UiTextKeys.ProductClaimSubmissionStatusAdditionalDocumentsRequested,
            ClaimSubmissionValues.StatusReviewing => UiTextKeys.ProductClaimSubmissionStatusReviewing,
            ClaimSubmissionValues.StatusCancelled => UiTextKeys.ProductClaimSubmissionStatusCancelled,
            ClaimSubmissionValues.StatusCompleted => UiTextKeys.ProductClaimSubmissionStatusCompleted,
            _ => UiTextKeys.ProductClaimSubmissionReferenceUnavailableValue
        };
        return uiTextProvider.Get(key);
    }

    private bool TryCreateDraft(string? status, out ClaimSubmissionDraft draft)
    {
        if (string.IsNullOrWhiteSpace(SelectedClaimCaseId)
            || string.IsNullOrWhiteSpace(SelectedPolicyId)
            || string.IsNullOrWhiteSpace(status)
            || !TryParseAmount(SubmittedAmountText, out var amount))
        {
            draft = null!;
            return false;
        }

        draft = new ClaimSubmissionDraft(
            SelectedClaimCaseId,
            SelectedPolicyId,
            PolicyCoverageId: null,
            CoverageDisplayName,
            SubmittedDate is null ? null : DateOnly.FromDateTime(SubmittedDate.Value),
            amount,
            AvailableDocuments.Where(document => document.IsSelected)
                .Select(document => document.Id)
                .ToArray(),
            status,
            Memo);
        return true;
    }

    private static bool TryParseAmount(string? value, out long? amount)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            amount = null;
            return true;
        }

        var styles = NumberStyles.Integer | NumberStyles.AllowThousands;
        if ((long.TryParse(value, styles, CultureInfo.CurrentCulture, out var parsed)
                || long.TryParse(value, styles, CultureInfo.InvariantCulture, out parsed))
            && parsed >= 0)
        {
            amount = parsed;
            return true;
        }

        amount = null;
        return false;
    }

    private void ResetContext()
    {
        ApplyEditorState(() =>
        {
            AvailablePolicies = [];
            AvailableDocuments = [];
            Submissions = [];
            recordsById.Clear();
            SelectedSubmissionId = null;
            editingRevision = null;
            editingStatus = null;
            SelectedPolicyId = null;
            CoverageDisplayName = null;
            SubmittedDate = null;
            SubmittedAmountText = null;
            SelectedStatus = ClaimSubmissionValues.StatusPreparing;
            Memo = null;
            AvailableStatusOptions = CreateStatusOptions(ClaimSubmissionValues.StatusPreparing);
            HasUnsavedChanges = false;
        });
        OnCommandStateChanged();
    }

    public void ClearMessages()
    {
        ValidationMessage = null;
        ConflictMessage = null;
        LegacyReviewMessage = null;
        ReferenceMessage = null;
        TransitionMessage = null;
        OperationMessage = null;
    }

    private void SetValidationMessage()
    {
        ValidationMessage = uiTextProvider.Get(
            UiTextKeys.ProductClaimSubmissionValidationMessage);
    }

    private void SetLegacyReviewMessage()
    {
        LegacyReviewMessage = uiTextProvider.Get(
            UiTextKeys.ProductClaimSubmissionLegacyReviewMessage);
    }

    private void SetReferenceMessage()
    {
        ReferenceMessage = uiTextProvider.Get(
            UiTextKeys.ProductClaimSubmissionReferenceMessage);
    }

    private void MarkEditorChanged()
    {
        if (!isApplying)
        {
            HasUnsavedChanges = true;
        }

        OnCommandStateChanged();
    }

    private void ApplyEditorState(Action action)
    {
        isApplying = true;
        try
        {
            action();
        }
        finally
        {
            isApplying = false;
        }
    }

    private void OnCommandStateChanged()
    {
        OnPropertyChanged(nameof(IsEditMode));
        OnPropertyChanged(nameof(CanCreate));
        OnPropertyChanged(nameof(CanSave));
        OnPropertyChanged(nameof(CanEditDetails));
        OnPropertyChanged(nameof(CanNavigateAway));
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

public sealed record ClaimSubmissionClaimCaseOptionViewModel(string Id, string DisplayTitle);

public sealed record ClaimSubmissionPolicyOptionViewModel(
    string Id,
    string DisplayTitle,
    string? InsurerName)
{
    public string DisplayText => string.IsNullOrWhiteSpace(InsurerName)
        ? DisplayTitle
        : $"{DisplayTitle} · {InsurerName}";
}

public sealed record ClaimSubmissionListItemViewModel(
    string Id,
    string PolicyDisplayTitle,
    string CoverageDisplayName,
    string StatusDisplay,
    string UpdatedAtDisplay);

public sealed record ClaimSubmissionStatusOptionViewModel(string Value, string DisplayText);

public sealed class ClaimSubmissionDocumentOptionViewModel : INotifyPropertyChanged
{
    private readonly Action changed;
    private bool isSelected;

    public ClaimSubmissionDocumentOptionViewModel(
        string id,
        string displayTitle,
        string documentType,
        Action changed)
    {
        Id = id;
        DisplayTitle = displayTitle;
        DocumentType = documentType;
        this.changed = changed ?? throw new ArgumentNullException(nameof(changed));
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public string Id { get; }

    public string DisplayTitle { get; }

    public string DocumentType { get; }

    public bool IsSelected
    {
        get => isSelected;
        set
        {
            if (isSelected == value)
            {
                return;
            }

            isSelected = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSelected)));
            changed();
        }
    }
}
