using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using FamilyClaimRef.App.Models.Storage;
using FamilyClaimRef.App.Services.Localization;
using FamilyClaimRef.App.Services.Storage;

namespace FamilyClaimRef.App.ViewModels;

public sealed class ClaimPaymentManagementViewModel : INotifyPropertyChanged
{
    private readonly IClaimPaymentStorageService storageService;
    private readonly IUiTextProvider uiTextProvider;
    private readonly SemaphoreSlim operationGate = new(1, 1);
    private readonly Dictionary<string, ClaimPaymentRecord> recordsById =
        new(StringComparer.Ordinal);

    private IReadOnlyList<ClaimPaymentListItemViewModel> payments = [];
    private IReadOnlyList<ClaimPaymentStatusOptionViewModel> availableStatusOptions = [];
    private string? selectedSubmissionId;
    private string? selectedPaymentId;
    private string? selectedStatus;
    private DateTime? paidDate;
    private string? paidAmountText;
    private string? paidCoverageDisplayName;
    private string? denyReason;
    private string? reductionReason;
    private string? additionalDocumentsMemo;
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

    public ClaimPaymentManagementViewModel(
        IClaimPaymentStorageService storageService,
        IUiTextProvider uiTextProvider)
    {
        this.storageService = storageService
            ?? throw new ArgumentNullException(nameof(storageService));
        this.uiTextProvider = uiTextProvider
            ?? throw new ArgumentNullException(nameof(uiTextProvider));
        AvailableStatusOptions = CreateStatusOptions(ClaimPaymentValues.StatusPending);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public IReadOnlyList<ClaimPaymentListItemViewModel> Payments
    {
        get => payments;
        private set
        {
            if (SetProperty(ref payments, value))
            {
                OnPropertyChanged(nameof(IsPaymentListEmpty));
            }
        }
    }

    public IReadOnlyList<ClaimPaymentStatusOptionViewModel> AvailableStatusOptions
    {
        get => availableStatusOptions;
        private set => SetProperty(ref availableStatusOptions, value);
    }

    public string? SelectedSubmissionId
    {
        get => selectedSubmissionId;
        private set
        {
            if (SetProperty(ref selectedSubmissionId, value))
            {
                OnCommandStateChanged();
            }
        }
    }

    public string? SelectedPaymentId
    {
        get => selectedPaymentId;
        private set
        {
            if (SetProperty(ref selectedPaymentId, value))
            {
                OnCommandStateChanged();
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
                OnFieldStateChanged();
            }
        }
    }

    public DateTime? PaidDate
    {
        get => paidDate;
        set
        {
            if (SetProperty(ref paidDate, value))
            {
                MarkEditorChanged();
            }
        }
    }

    public string? PaidAmountText
    {
        get => paidAmountText;
        set
        {
            if (SetProperty(ref paidAmountText, value))
            {
                MarkEditorChanged();
            }
        }
    }

    public string? PaidCoverageDisplayName
    {
        get => paidCoverageDisplayName;
        set
        {
            if (SetProperty(ref paidCoverageDisplayName, value))
            {
                MarkEditorChanged();
            }
        }
    }

    public string? DenyReason
    {
        get => denyReason;
        set
        {
            if (SetProperty(ref denyReason, value))
            {
                MarkEditorChanged();
            }
        }
    }

    public string? ReductionReason
    {
        get => reductionReason;
        set
        {
            if (SetProperty(ref reductionReason, value))
            {
                MarkEditorChanged();
            }
        }
    }

    public string? AdditionalDocumentsMemo
    {
        get => additionalDocumentsMemo;
        set
        {
            if (SetProperty(ref additionalDocumentsMemo, value))
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
                OnFieldStateChanged();
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

    public bool IsPaymentListEmpty => Payments.Count == 0;

    public bool CanCreate =>
        !IsBusy
        && !IsEditMode
        && !string.IsNullOrWhiteSpace(SelectedSubmissionId);

    public bool CanSave =>
        !IsBusy
        && IsEditMode
        && HasUnsavedChanges
        && !ClaimPaymentValues.IsTerminal(editingStatus ?? string.Empty);

    public bool CanEditDetails =>
        !IsBusy
        && !ClaimPaymentValues.IsTerminal(editingStatus ?? string.Empty);

    public bool CanNavigateAway => !IsBusy && !HasUnsavedChanges;

    public bool IsPaidResultEnabled =>
        CanEditDetails
        && (string.Equals(SelectedStatus, ClaimPaymentValues.StatusPending, StringComparison.Ordinal)
            || string.Equals(SelectedStatus, ClaimPaymentValues.StatusPaid, StringComparison.Ordinal)
            || string.Equals(SelectedStatus, ClaimPaymentValues.StatusPartiallyPaid, StringComparison.Ordinal));

    public bool IsDenyReasonEnabled =>
        CanEditDetails
        && (string.Equals(SelectedStatus, ClaimPaymentValues.StatusPending, StringComparison.Ordinal)
            || string.Equals(SelectedStatus, ClaimPaymentValues.StatusDenied, StringComparison.Ordinal));

    public bool IsReductionReasonEnabled =>
        CanEditDetails
        && (string.Equals(SelectedStatus, ClaimPaymentValues.StatusPending, StringComparison.Ordinal)
            || string.Equals(SelectedStatus, ClaimPaymentValues.StatusPartiallyPaid, StringComparison.Ordinal));

    public bool IsAdditionalDocumentsEnabled =>
        CanEditDetails
        && !string.Equals(SelectedStatus, ClaimPaymentValues.StatusCancelled, StringComparison.Ordinal);

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

    public async Task<bool> LoadForSubmissionAsync(
        string claimSubmissionId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(claimSubmissionId))
        {
            ClearContext();
            return false;
        }

        var normalizedId = claimSubmissionId.Trim();
        if (!CanNavigateAway
            && !string.Equals(SelectedSubmissionId, normalizedId, StringComparison.Ordinal))
        {
            return false;
        }

        if (!await operationGate.WaitAsync(0, cancellationToken))
        {
            return false;
        }

        try
        {
            IsBusy = true;
            ClearMessages();
            SelectedSubmissionId = normalizedId;
            var records = await storageService.GetBySubmissionAsync(normalizedId, cancellationToken);
            recordsById.Clear();
            foreach (var record in records)
            {
                recordsById.Add(record.Id, record);
            }

            RebuildList();
            if (!string.IsNullOrWhiteSpace(SelectedPaymentId)
                && recordsById.TryGetValue(SelectedPaymentId, out var selected))
            {
                ApplyRecord(selected);
            }
            else
            {
                StartNew();
            }

            return true;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch
        {
            ApplyEditorState(() =>
            {
                SelectedSubmissionId = normalizedId;
                recordsById.Clear();
                Payments = [];
                ResetEditor();
            });
            OperationMessage = uiTextProvider.Get(UiTextKeys.ProductClaimPaymentOperationFailedMessage);
            return false;
        }
        finally
        {
            IsBusy = false;
            operationGate.Release();
        }
    }

    public void ClearContext()
    {
        ApplyEditorState(() =>
        {
            SelectedSubmissionId = null;
            recordsById.Clear();
            Payments = [];
            ResetEditor();
        });
        ClearMessages();
        OnCommandStateChanged();
    }

    public void StartNew()
    {
        ClearMessages();
        ApplyEditorState(ResetEditor);
        OnCommandStateChanged();
    }

    public bool LoadSelectedPayment()
    {
        return SelectPayment(SelectedPaymentId);
    }

    public bool SelectPayment(string? paymentId)
    {
        var normalizedId = paymentId?.Trim();
        if (string.IsNullOrWhiteSpace(normalizedId)
            || !recordsById.TryGetValue(normalizedId, out var record))
        {
            return false;
        }

        if (string.Equals(SelectedPaymentId, normalizedId, StringComparison.Ordinal)
            && HasUnsavedChanges)
        {
            return true;
        }

        if (!CanNavigateAway)
        {
            return false;
        }

        ClearMessages();
        ApplyRecord(record);
        return true;
    }

    public async Task<bool> CreatePendingAsync(CancellationToken cancellationToken = default)
    {
        if (!await operationGate.WaitAsync(0, cancellationToken))
        {
            return false;
        }

        try
        {
            IsBusy = true;
            ClearMessages();
            if (!TryCreateDraft(ClaimPaymentValues.StatusPending, out var draft))
            {
                SetValidationMessage();
                return false;
            }

            var created = await storageService.CreateAsync(draft, cancellationToken);
            await ApplyCommittedRecordAsync(created);
            OperationMessage = uiTextProvider.Get(UiTextKeys.ProductClaimPaymentCreatedMessage);
            return true;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (ClaimPaymentLegacyReviewRequiredException)
        {
            SetLegacyReviewMessage();
            return false;
        }
        catch (ClaimPaymentReferenceException)
        {
            SetReferenceMessage();
            return false;
        }
        catch (ClaimPaymentTransitionException)
        {
            SetTransitionMessage();
            return false;
        }
        catch (ArgumentException)
        {
            SetValidationMessage();
            return false;
        }
        catch
        {
            OperationMessage = uiTextProvider.Get(UiTextKeys.ProductClaimPaymentOperationFailedMessage);
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
            || string.IsNullOrWhiteSpace(SelectedPaymentId)
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

            var updated = await storageService.UpdateAsync(
                SelectedPaymentId,
                editingRevision.Value,
                draft,
                cancellationToken);
            await ApplyCommittedRecordAsync(updated);
            OperationMessage = uiTextProvider.Get(UiTextKeys.ProductClaimPaymentSavedMessage);
            return true;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (ClaimPaymentConcurrencyException)
        {
            ConflictMessage = uiTextProvider.Get(UiTextKeys.ProductClaimPaymentConflictMessage);
            return false;
        }
        catch (ClaimPaymentLegacyReviewRequiredException)
        {
            SetLegacyReviewMessage();
            return false;
        }
        catch (ClaimPaymentReferenceException)
        {
            SetReferenceMessage();
            return false;
        }
        catch (ClaimPaymentTransitionException)
        {
            SetTransitionMessage();
            return false;
        }
        catch (ArgumentException)
        {
            SetValidationMessage();
            return false;
        }
        catch
        {
            OperationMessage = uiTextProvider.Get(UiTextKeys.ProductClaimPaymentOperationFailedMessage);
            return false;
        }
        finally
        {
            IsBusy = false;
            operationGate.Release();
        }
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

    private async Task ApplyCommittedRecordAsync(ClaimPaymentRecord record)
    {
        recordsById[record.Id] = record;
        RebuildList();
        ApplyRecord(record);

        try
        {
            var records = await storageService.GetBySubmissionAsync(
                record.ClaimSubmissionId,
                CancellationToken.None);
            recordsById.Clear();
            foreach (var refreshed in records)
            {
                recordsById.Add(refreshed.Id, refreshed);
            }

            RebuildList();
            if (recordsById.TryGetValue(record.Id, out var selected))
            {
                ApplyRecord(selected);
            }
        }
        catch
        {
            // Preserve the durable mutation result if a post-write refresh fails.
        }
    }

    private void RebuildList()
    {
        Payments = recordsById.Values
            .OrderByDescending(record => record.UpdatedAt)
            .ThenBy(record => record.Id, StringComparer.Ordinal)
            .Select(record => new ClaimPaymentListItemViewModel(
                record.Id,
                GetStatusDisplay(record.Status),
                record.PaidAmount?.ToString("N0", CultureInfo.CurrentCulture)
                    ?? uiTextProvider.Get(UiTextKeys.ProductClaimPaymentNotEnteredValue),
                record.PaidDate?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)
                    ?? uiTextProvider.Get(UiTextKeys.ProductClaimPaymentNotEnteredValue),
                record.UpdatedAt.ToLocalTime().ToString("yyyy-MM-dd HH:mm", CultureInfo.CurrentCulture)))
            .ToList();
    }

    private void ApplyRecord(ClaimPaymentRecord record)
    {
        ApplyEditorState(() =>
        {
            SelectedSubmissionId = record.ClaimSubmissionId;
            SelectedPaymentId = record.Id;
            editingRevision = record.Revision;
            editingStatus = record.Status;
            SelectedStatus = record.Status;
            PaidDate = record.PaidDate?.ToDateTime(TimeOnly.MinValue);
            PaidAmountText = record.PaidAmount?.ToString("N0", CultureInfo.CurrentCulture);
            PaidCoverageDisplayName = record.PaidCoverageDisplayName;
            DenyReason = record.DenyReason;
            ReductionReason = record.ReductionReason;
            AdditionalDocumentsMemo = record.AdditionalDocumentsMemo;
            Memo = record.Memo;
            AvailableStatusOptions = CreateStatusOptions(record.Status);
            HasUnsavedChanges = false;
        });
        OnCommandStateChanged();
        OnFieldStateChanged();
    }

    private void ResetEditor()
    {
        SelectedPaymentId = null;
        editingRevision = null;
        editingStatus = null;
        SelectedStatus = ClaimPaymentValues.StatusPending;
        PaidDate = null;
        PaidAmountText = null;
        PaidCoverageDisplayName = null;
        DenyReason = null;
        ReductionReason = null;
        AdditionalDocumentsMemo = null;
        Memo = null;
        AvailableStatusOptions = CreateStatusOptions(ClaimPaymentValues.StatusPending);
        HasUnsavedChanges = false;
    }

    private IReadOnlyList<ClaimPaymentStatusOptionViewModel> CreateStatusOptions(string currentStatus)
    {
        return new[] { currentStatus }
            .Concat(ClaimPaymentValues.GetAllowedTargets(currentStatus))
            .Distinct(StringComparer.Ordinal)
            .Select(status => new ClaimPaymentStatusOptionViewModel(status, GetStatusDisplay(status)))
            .ToList();
    }

    private string GetStatusDisplay(string status)
    {
        var key = status switch
        {
            ClaimPaymentValues.StatusPending => UiTextKeys.ProductClaimPaymentStatusPending,
            ClaimPaymentValues.StatusPaid => UiTextKeys.ProductClaimPaymentStatusPaid,
            ClaimPaymentValues.StatusPartiallyPaid => UiTextKeys.ProductClaimPaymentStatusPartiallyPaid,
            ClaimPaymentValues.StatusDenied => UiTextKeys.ProductClaimPaymentStatusDenied,
            ClaimPaymentValues.StatusCancelled => UiTextKeys.ProductClaimPaymentStatusCancelled,
            _ => UiTextKeys.ProductClaimPaymentNotEnteredValue
        };
        return uiTextProvider.Get(key);
    }

    private bool TryCreateDraft(string? status, out ClaimPaymentDraft draft)
    {
        if (string.IsNullOrWhiteSpace(SelectedSubmissionId)
            || string.IsNullOrWhiteSpace(status)
            || !TryParseAmount(PaidAmountText, out var amount))
        {
            draft = null!;
            return false;
        }

        draft = new ClaimPaymentDraft(
            SelectedSubmissionId,
            status,
            PaidDate is null ? null : DateOnly.FromDateTime(PaidDate.Value),
            amount,
            PaidCoverageDisplayName,
            DenyReason,
            ReductionReason,
            AdditionalDocumentsMemo,
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
            && parsed > 0)
        {
            amount = parsed;
            return true;
        }

        amount = null;
        return false;
    }

    private void SetValidationMessage()
    {
        ValidationMessage = uiTextProvider.Get(UiTextKeys.ProductClaimPaymentValidationMessage);
    }

    private void SetLegacyReviewMessage()
    {
        LegacyReviewMessage = uiTextProvider.Get(UiTextKeys.ProductClaimPaymentLegacyReviewMessage);
    }

    private void SetReferenceMessage()
    {
        ReferenceMessage = uiTextProvider.Get(UiTextKeys.ProductClaimPaymentReferenceMessage);
    }

    private void SetTransitionMessage()
    {
        TransitionMessage = uiTextProvider.Get(UiTextKeys.ProductClaimPaymentTransitionMessage);
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

    private void OnFieldStateChanged()
    {
        OnPropertyChanged(nameof(IsPaidResultEnabled));
        OnPropertyChanged(nameof(IsDenyReasonEnabled));
        OnPropertyChanged(nameof(IsReductionReasonEnabled));
        OnPropertyChanged(nameof(IsAdditionalDocumentsEnabled));
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

public sealed record ClaimPaymentListItemViewModel(
    string Id,
    string StatusDisplay,
    string PaidAmountDisplay,
    string PaidDateDisplay,
    string UpdatedAtDisplay);

public sealed record ClaimPaymentStatusOptionViewModel(string Value, string DisplayText);
