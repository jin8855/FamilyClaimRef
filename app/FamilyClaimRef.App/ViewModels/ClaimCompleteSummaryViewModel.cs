using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using FamilyClaimRef.App.Models.Storage;
using FamilyClaimRef.App.Services.Localization;
using FamilyClaimRef.App.Services.Storage;

namespace FamilyClaimRef.App.ViewModels;

public sealed class ClaimCompleteSummaryViewModel : INotifyPropertyChanged
{
    private readonly IClaimCaseStorageService claimCaseStorageService;
    private readonly IClaimSubmissionStorageService claimSubmissionStorageService;
    private readonly IClaimPaymentStorageService claimPaymentStorageService;
    private readonly IPolicyClaimStorageService policyStorageService;
    private readonly IFamilyMemberStorageService familyMemberStorageService;
    private readonly IUiTextProvider uiTextProvider;
    private readonly SemaphoreSlim operationGate = new(1, 1);
    private string? selectedClaimCaseId;
    private bool isBusy;
    private bool hasSummary;
    private string? stateMessage;
    private string? claimDisplayTitle;
    private string? familyDisplayName;
    private string? treatmentDateDisplay;
    private string? hospitalName;
    private string? diagnosisDisplay;
    private string? visitTypeDisplay;
    private string? caseStatusDisplay;
    private IReadOnlyList<ClaimCompleteSubmissionItemViewModel> submissions = [];
    private int submissionTotalCount;
    private int submissionInProgressCount;
    private int submissionCompletedCount;
    private int submissionCancelledCount;
    private int paymentPendingCount;
    private int paymentPaidCount;
    private int paymentPartiallyPaidCount;
    private int paymentDeniedCount;
    private int paymentCancelledCount;

    public ClaimCompleteSummaryViewModel(
        IClaimCaseStorageService claimCaseStorageService,
        IClaimSubmissionStorageService claimSubmissionStorageService,
        IClaimPaymentStorageService claimPaymentStorageService,
        IPolicyClaimStorageService policyStorageService,
        IFamilyMemberStorageService familyMemberStorageService,
        IUiTextProvider uiTextProvider)
    {
        this.claimCaseStorageService = claimCaseStorageService
            ?? throw new ArgumentNullException(nameof(claimCaseStorageService));
        this.claimSubmissionStorageService = claimSubmissionStorageService
            ?? throw new ArgumentNullException(nameof(claimSubmissionStorageService));
        this.claimPaymentStorageService = claimPaymentStorageService
            ?? throw new ArgumentNullException(nameof(claimPaymentStorageService));
        this.policyStorageService = policyStorageService
            ?? throw new ArgumentNullException(nameof(policyStorageService));
        this.familyMemberStorageService = familyMemberStorageService
            ?? throw new ArgumentNullException(nameof(familyMemberStorageService));
        this.uiTextProvider = uiTextProvider
            ?? throw new ArgumentNullException(nameof(uiTextProvider));
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public string? SelectedClaimCaseId
    {
        get => selectedClaimCaseId;
        set => SetProperty(ref selectedClaimCaseId, value);
    }

    public bool IsBusy
    {
        get => isBusy;
        private set => SetProperty(ref isBusy, value);
    }

    public bool HasSummary
    {
        get => hasSummary;
        private set => SetProperty(ref hasSummary, value);
    }

    public bool HasSubmissions => Submissions.Count > 0;

    public bool HasStateMessage => !string.IsNullOrWhiteSpace(StateMessage);

    public string? StateMessage
    {
        get => stateMessage;
        private set
        {
            if (SetProperty(ref stateMessage, value))
            {
                OnPropertyChanged(nameof(HasStateMessage));
            }
        }
    }

    public string? ClaimDisplayTitle
    {
        get => claimDisplayTitle;
        private set => SetProperty(ref claimDisplayTitle, value);
    }

    public string? FamilyDisplayName
    {
        get => familyDisplayName;
        private set => SetProperty(ref familyDisplayName, value);
    }

    public string? TreatmentDateDisplay
    {
        get => treatmentDateDisplay;
        private set => SetProperty(ref treatmentDateDisplay, value);
    }

    public string? HospitalName
    {
        get => hospitalName;
        private set => SetProperty(ref hospitalName, value);
    }

    public string? DiagnosisDisplay
    {
        get => diagnosisDisplay;
        private set => SetProperty(ref diagnosisDisplay, value);
    }

    public string? VisitTypeDisplay
    {
        get => visitTypeDisplay;
        private set => SetProperty(ref visitTypeDisplay, value);
    }

    public string? CaseStatusDisplay
    {
        get => caseStatusDisplay;
        private set => SetProperty(ref caseStatusDisplay, value);
    }

    public IReadOnlyList<ClaimCompleteSubmissionItemViewModel> Submissions
    {
        get => submissions;
        private set
        {
            if (SetProperty(ref submissions, value))
            {
                OnPropertyChanged(nameof(HasSubmissions));
            }
        }
    }

    public int SubmissionTotalCount
    {
        get => submissionTotalCount;
        private set => SetProperty(ref submissionTotalCount, value);
    }

    public int SubmissionInProgressCount
    {
        get => submissionInProgressCount;
        private set => SetProperty(ref submissionInProgressCount, value);
    }

    public int SubmissionCompletedCount
    {
        get => submissionCompletedCount;
        private set => SetProperty(ref submissionCompletedCount, value);
    }

    public int SubmissionCancelledCount
    {
        get => submissionCancelledCount;
        private set => SetProperty(ref submissionCancelledCount, value);
    }

    public int PaymentPendingCount
    {
        get => paymentPendingCount;
        private set => SetProperty(ref paymentPendingCount, value);
    }

    public int PaymentPaidCount
    {
        get => paymentPaidCount;
        private set => SetProperty(ref paymentPaidCount, value);
    }

    public int PaymentPartiallyPaidCount
    {
        get => paymentPartiallyPaidCount;
        private set => SetProperty(ref paymentPartiallyPaidCount, value);
    }

    public int PaymentDeniedCount
    {
        get => paymentDeniedCount;
        private set => SetProperty(ref paymentDeniedCount, value);
    }

    public int PaymentCancelledCount
    {
        get => paymentCancelledCount;
        private set => SetProperty(ref paymentCancelledCount, value);
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
            ResetProjection();

            if (string.IsNullOrWhiteSpace(SelectedClaimCaseId))
            {
                StateMessage = uiTextProvider.Get(UiTextKeys.ProductClaimCompleteEmptyMessage);
                return true;
            }

            var claim = await claimCaseStorageService.GetClaimCaseAsync(
                SelectedClaimCaseId,
                cancellationToken);
            if (claim is null
                || claim.DisabledAt is not null
                || !string.Equals(claim.CaseStatus, ClaimCaseValues.StatusSaved, StringComparison.Ordinal))
            {
                SetReferenceMessage();
                return false;
            }

            if (string.IsNullOrWhiteSpace(claim.FamilyMemberId))
            {
                SetLegacyMessage();
                return false;
            }

            var family = await familyMemberStorageService.GetFamilyMemberAsync(
                claim.FamilyMemberId,
                cancellationToken);
            if (family is null || family.DisabledAt is not null)
            {
                SetReferenceMessage();
                return false;
            }

            var records = await claimSubmissionStorageService.GetByClaimCaseAsync(
                claim.Id,
                cancellationToken);
            var items = new List<ClaimCompleteSubmissionItemViewModel>(records.Count);
            var allPayments = new List<ClaimPaymentRecord>();

            foreach (var submission in records)
            {
                if (!string.Equals(submission.ClaimCaseId, claim.Id, StringComparison.Ordinal)
                    || string.IsNullOrWhiteSpace(submission.PolicyId)
                    || !ClaimSubmissionValues.Statuses.Contains(submission.Status, StringComparer.Ordinal))
                {
                    SetReferenceMessage();
                    return false;
                }

                var policy = await policyStorageService.GetPolicyAsync(
                    submission.PolicyId,
                    cancellationToken);
                if (policy is null || policy.DisabledAt is not null)
                {
                    SetReferenceMessage();
                    return false;
                }

                if (string.IsNullOrWhiteSpace(policy.FamilyMemberId))
                {
                    SetLegacyMessage();
                    return false;
                }

                if (!string.Equals(policy.FamilyMemberId, claim.FamilyMemberId, StringComparison.Ordinal))
                {
                    SetReferenceMessage();
                    return false;
                }

                var payments = await claimPaymentStorageService.GetBySubmissionAsync(
                    submission.Id,
                    cancellationToken);
                if (payments.Any(payment =>
                    !string.Equals(payment.ClaimSubmissionId, submission.Id, StringComparison.Ordinal)
                    || !ClaimPaymentValues.Statuses.Contains(payment.Status, StringComparer.Ordinal)))
                {
                    SetReferenceMessage();
                    return false;
                }

                allPayments.AddRange(payments);
                items.Add(new ClaimCompleteSubmissionItemViewModel(
                    policy.DisplayTitle,
                    GetSubmissionStatusDisplay(submission.Status),
                    CreatePaymentSummary(payments),
                    submission.UpdatedAt.ToLocalTime().ToString(
                        "yyyy-MM-dd HH:mm",
                        CultureInfo.CurrentCulture)));
            }

            ClaimDisplayTitle = claim.DisplayTitle;
            FamilyDisplayName = family.DisplayName;
            TreatmentDateDisplay = claim.ReferenceDate.ToString(
                "yyyy-MM-dd",
                CultureInfo.InvariantCulture);
            HospitalName = claim.HospitalName;
            DiagnosisDisplay = CreateDiagnosisDisplay(claim);
            VisitTypeDisplay = GetVisitTypeDisplay(claim.VisitType);
            CaseStatusDisplay = uiTextProvider.Get(UiTextKeys.ProductClaimCompleteCaseStatusSaved);
            Submissions = items
                .OrderByDescending(item => item.UpdatedAtDisplay, StringComparer.Ordinal)
                .ToArray();
            SubmissionTotalCount = records.Count;
            SubmissionInProgressCount = records.Count(record =>
                !ClaimSubmissionValues.IsTerminal(record.Status));
            SubmissionCompletedCount = records.Count(record => string.Equals(
                record.Status,
                ClaimSubmissionValues.StatusCompleted,
                StringComparison.Ordinal));
            SubmissionCancelledCount = records.Count(record => string.Equals(
                record.Status,
                ClaimSubmissionValues.StatusCancelled,
                StringComparison.Ordinal));
            PaymentPendingCount = CountPayments(allPayments, ClaimPaymentValues.StatusPending);
            PaymentPaidCount = CountPayments(allPayments, ClaimPaymentValues.StatusPaid);
            PaymentPartiallyPaidCount = CountPayments(allPayments, ClaimPaymentValues.StatusPartiallyPaid);
            PaymentDeniedCount = CountPayments(allPayments, ClaimPaymentValues.StatusDenied);
            PaymentCancelledCount = CountPayments(allPayments, ClaimPaymentValues.StatusCancelled);
            HasSummary = true;
            return true;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch
        {
            ResetProjection();
            StateMessage = uiTextProvider.Get(UiTextKeys.ProductClaimCompleteLoadFailedMessage);
            return false;
        }
        finally
        {
            IsBusy = false;
            operationGate.Release();
        }
    }

    private string CreatePaymentSummary(IReadOnlyList<ClaimPaymentRecord> payments)
    {
        if (payments.Count == 0)
        {
            return uiTextProvider.Get(UiTextKeys.ProductClaimCompleteNoPaymentsValue);
        }

        return uiTextProvider.Format(
            UiTextKeys.ProductClaimCompletePaymentSummaryFormat,
            CountPayments(payments, ClaimPaymentValues.StatusPending),
            CountPayments(payments, ClaimPaymentValues.StatusPaid),
            CountPayments(payments, ClaimPaymentValues.StatusPartiallyPaid),
            CountPayments(payments, ClaimPaymentValues.StatusDenied),
            CountPayments(payments, ClaimPaymentValues.StatusCancelled));
    }

    private string GetSubmissionStatusDisplay(string status)
    {
        return status switch
        {
            ClaimSubmissionValues.StatusPreparing => uiTextProvider.Get(UiTextKeys.ProductClaimSubmissionStatusPreparing),
            ClaimSubmissionValues.StatusSubmitted => uiTextProvider.Get(UiTextKeys.ProductClaimSubmissionStatusSubmitted),
            ClaimSubmissionValues.StatusAdditionalDocumentsRequested => uiTextProvider.Get(UiTextKeys.ProductClaimSubmissionStatusAdditionalDocumentsRequested),
            ClaimSubmissionValues.StatusReviewing => uiTextProvider.Get(UiTextKeys.ProductClaimSubmissionStatusReviewing),
            ClaimSubmissionValues.StatusCancelled => uiTextProvider.Get(UiTextKeys.ProductClaimSubmissionStatusCancelled),
            ClaimSubmissionValues.StatusCompleted => uiTextProvider.Get(UiTextKeys.ProductClaimSubmissionStatusCompleted),
            _ => uiTextProvider.Get(UiTextKeys.ProductClaimCompleteReferenceMessage)
        };
    }

    private string GetVisitTypeDisplay(string? visitType)
    {
        return visitType switch
        {
            ClaimCaseValues.VisitTypeOutpatient => uiTextProvider.Get(UiTextKeys.ProductClaimCaseVisitTypeOutpatient),
            ClaimCaseValues.VisitTypeInpatient => uiTextProvider.Get(UiTextKeys.ProductClaimCaseVisitTypeInpatient),
            _ => uiTextProvider.Get(UiTextKeys.ProductClaimCompleteNotEnteredValue)
        };
    }

    private string CreateDiagnosisDisplay(ClaimRecord claim)
    {
        var values = new[] { claim.DiagnosisCode, claim.DiagnosisName }
            .Where(value => !string.IsNullOrWhiteSpace(value));
        var display = string.Join(" / ", values);
        return string.IsNullOrWhiteSpace(display)
            ? uiTextProvider.Get(UiTextKeys.ProductClaimCompleteNotEnteredValue)
            : display;
    }

    private static int CountPayments(
        IEnumerable<ClaimPaymentRecord> payments,
        string status)
    {
        return payments.Count(payment => string.Equals(
            payment.Status,
            status,
            StringComparison.Ordinal));
    }

    private void SetReferenceMessage()
    {
        ResetProjection();
        StateMessage = uiTextProvider.Get(UiTextKeys.ProductClaimCompleteReferenceMessage);
    }

    private void SetLegacyMessage()
    {
        ResetProjection();
        StateMessage = uiTextProvider.Get(UiTextKeys.ProductClaimCompleteLegacyReviewMessage);
    }

    private void ResetProjection()
    {
        HasSummary = false;
        StateMessage = null;
        ClaimDisplayTitle = null;
        FamilyDisplayName = null;
        TreatmentDateDisplay = null;
        HospitalName = null;
        DiagnosisDisplay = null;
        VisitTypeDisplay = null;
        CaseStatusDisplay = null;
        Submissions = [];
        SubmissionTotalCount = 0;
        SubmissionInProgressCount = 0;
        SubmissionCompletedCount = 0;
        SubmissionCancelledCount = 0;
        PaymentPendingCount = 0;
        PaymentPaidCount = 0;
        PaymentPartiallyPaidCount = 0;
        PaymentDeniedCount = 0;
        PaymentCancelledCount = 0;
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

public sealed record ClaimCompleteSubmissionItemViewModel(
    string PolicyDisplayTitle,
    string SubmissionStatusDisplay,
    string PaymentSummaryDisplay,
    string UpdatedAtDisplay);
