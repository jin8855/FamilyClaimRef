using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using FamilyClaimRef.App.Models.Storage;
using FamilyClaimRef.App.Services.Localization;
using FamilyClaimRef.App.Services.Storage;

namespace FamilyClaimRef.App.ViewModels;

public sealed class HomeDashboardViewModel : INotifyPropertyChanged
{
    private static readonly HashSet<string> InProgressSubmissionStatuses =
    [
        ClaimSubmissionValues.StatusPreparing,
        ClaimSubmissionValues.StatusSubmitted,
        ClaimSubmissionValues.StatusAdditionalDocumentsRequested,
        ClaimSubmissionValues.StatusReviewing
    ];

    private readonly IClaimHistoryStorageReader historyStorageReader;
    private readonly IClaimSubmissionHistoryStorageReader submissionHistoryStorageReader;
    private readonly IClaimPaymentHistoryStorageReader paymentHistoryStorageReader;
    private readonly IFamilyMemberStorageService familyMemberStorageService;
    private readonly IUiTextProvider uiTextProvider;
    private readonly SemaphoreSlim operationGate = new(1, 1);
    private IReadOnlyList<HomeDashboardRecentActivityViewModel> recentActivities = [];
    private int inProgressClaimCount;
    private int noSubmissionClaimCount;
    private int paymentResultPendingCount;
    private bool isBusy;
    private bool hasLoadedProjection;
    private string? stateMessage;

    public HomeDashboardViewModel(
        IClaimHistoryStorageReader historyStorageReader,
        IClaimSubmissionHistoryStorageReader submissionHistoryStorageReader,
        IClaimPaymentHistoryStorageReader paymentHistoryStorageReader,
        IFamilyMemberStorageService familyMemberStorageService,
        IUiTextProvider uiTextProvider)
    {
        this.historyStorageReader = historyStorageReader
            ?? throw new ArgumentNullException(nameof(historyStorageReader));
        this.submissionHistoryStorageReader = submissionHistoryStorageReader
            ?? throw new ArgumentNullException(nameof(submissionHistoryStorageReader));
        this.paymentHistoryStorageReader = paymentHistoryStorageReader
            ?? throw new ArgumentNullException(nameof(paymentHistoryStorageReader));
        this.familyMemberStorageService = familyMemberStorageService
            ?? throw new ArgumentNullException(nameof(familyMemberStorageService));
        this.uiTextProvider = uiTextProvider
            ?? throw new ArgumentNullException(nameof(uiTextProvider));
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public int InProgressClaimCount
    {
        get => inProgressClaimCount;
        private set => SetProperty(ref inProgressClaimCount, value);
    }

    public int NoSubmissionClaimCount
    {
        get => noSubmissionClaimCount;
        private set => SetProperty(ref noSubmissionClaimCount, value);
    }

    public int PaymentResultPendingCount
    {
        get => paymentResultPendingCount;
        private set => SetProperty(ref paymentResultPendingCount, value);
    }

    public IReadOnlyList<HomeDashboardRecentActivityViewModel> RecentActivities
    {
        get => recentActivities;
        private set
        {
            if (SetProperty(ref recentActivities, value))
            {
                OnPropertyChanged(nameof(HasRecentActivities));
            }
        }
    }

    public bool HasRecentActivities => RecentActivities.Count > 0;

    public bool IsBusy
    {
        get => isBusy;
        private set => SetProperty(ref isBusy, value);
    }

    public bool HasLoadedProjection
    {
        get => hasLoadedProjection;
        private set => SetProperty(ref hasLoadedProjection, value);
    }

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

    public bool HasStateMessage => !string.IsNullOrWhiteSpace(StateMessage);

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

            var claims = await historyStorageReader.GetAllClaimCasesForHistoryAsync(cancellationToken);
            var policies = await historyStorageReader.GetAllPoliciesForHistoryAsync(cancellationToken);
            var families = await familyMemberStorageService.GetFamilyMembersAsync(cancellationToken);
            var submissions = await submissionHistoryStorageReader
                .GetAllSubmissionsForHistoryAsync(cancellationToken);
            var payments = await paymentHistoryStorageReader
                .GetAllPaymentsForHistoryAsync(cancellationToken);

            if (!TryBuildDictionary(claims, claim => claim.Id, out var claimById)
                || !TryBuildDictionary(policies, policy => policy.Id, out var policyById)
                || !TryBuildDictionary(families, family => family.Id, out var familyById)
                || !TryBuildDictionary(submissions, submission => submission.Id, out var submissionById)
                || !TryBuildDictionary(payments, payment => payment.Id, out _))
            {
                return SetFailure(UiTextKeys.ProductHistoryReferenceMessage);
            }

            if (!TryValidateGraph(
                    claims,
                    policies,
                    submissions,
                    payments,
                    claimById,
                    policyById,
                    familyById,
                    submissionById,
                    out var validationMessageKey))
            {
                return SetFailure(validationMessageKey);
            }

            var submissionsByClaim = submissions.ToLookup(
                submission => submission.ClaimCaseId,
                StringComparer.Ordinal);
            var paymentsBySubmission = payments.ToLookup(
                payment => payment.ClaimSubmissionId,
                StringComparer.Ordinal);

            InProgressClaimCount = claims.Count(claim =>
                IsActiveSavedClaim(claim, familyById)
                && submissionsByClaim[claim.Id].Any(submission =>
                    IsActiveSubmission(submission, claim, policyById)
                    && InProgressSubmissionStatuses.Contains(submission.Status)));
            NoSubmissionClaimCount = claims.Count(claim =>
                IsActiveSavedClaim(claim, familyById)
                && !submissionsByClaim[claim.Id].Any());
            PaymentResultPendingCount = submissions.Count(submission =>
            {
                var claim = claimById[submission.ClaimCaseId];
                return IsActiveSavedClaim(claim, familyById)
                    && IsActiveSubmission(submission, claim, policyById)
                    && string.Equals(
                        submission.Status,
                        ClaimSubmissionValues.StatusCompleted,
                        StringComparison.Ordinal)
                    && !paymentsBySubmission[submission.Id].Any(payment =>
                        ClaimPaymentValues.IsTerminal(payment.Status));
            });

            RecentActivities = submissions
                .Select(submission => CreateProjection(
                    claimById[submission.ClaimCaseId],
                    familyById[claimById[submission.ClaimCaseId].FamilyMemberId!],
                    policyById[submission.PolicyId],
                    submission,
                    paymentsBySubmission[submission.Id].ToArray()))
                .OrderByDescending(projection => projection.UpdatedAt)
                .ThenByDescending(projection => projection.TreatmentDate)
                .ThenBy(projection => projection.SortKey, StringComparer.Ordinal)
                .Take(5)
                .Select(projection => projection.Activity)
                .ToArray();

            HasLoadedProjection = true;
            StateMessage = HasRecentActivities
                ? null
                : uiTextProvider.Get(UiTextKeys.ProductHistoryEmptyMessage);
            return true;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch
        {
            ResetProjection();
            StateMessage = uiTextProvider.Get(UiTextKeys.ProductHistoryLoadFailedMessage);
            return false;
        }
        finally
        {
            IsBusy = false;
            operationGate.Release();
        }
    }

    private static bool TryValidateGraph(
        IReadOnlyList<ClaimRecord> claims,
        IReadOnlyList<PolicyRecord> policies,
        IReadOnlyList<ClaimSubmissionRecord> submissions,
        IReadOnlyList<ClaimPaymentRecord> payments,
        IReadOnlyDictionary<string, ClaimRecord> claimById,
        IReadOnlyDictionary<string, PolicyRecord> policyById,
        IReadOnlyDictionary<string, FamilyMemberRecord> familyById,
        IReadOnlyDictionary<string, ClaimSubmissionRecord> submissionById,
        out string messageKey)
    {
        foreach (var claim in claims)
        {
            if (!IsKnownClaimStatus(claim.CaseStatus))
            {
                messageKey = UiTextKeys.ProductHistoryUnknownStatusMessage;
                return false;
            }

            if (string.IsNullOrWhiteSpace(claim.FamilyMemberId))
            {
                messageKey = UiTextKeys.ProductHistoryLegacyReviewMessage;
                return false;
            }

            if (!familyById.ContainsKey(claim.FamilyMemberId))
            {
                messageKey = UiTextKeys.ProductHistoryReferenceMessage;
                return false;
            }
        }

        foreach (var policy in policies)
        {
            if (string.IsNullOrWhiteSpace(policy.FamilyMemberId))
            {
                messageKey = UiTextKeys.ProductHistoryLegacyReviewMessage;
                return false;
            }

            if (!familyById.ContainsKey(policy.FamilyMemberId))
            {
                messageKey = UiTextKeys.ProductHistoryReferenceMessage;
                return false;
            }
        }

        foreach (var submission in submissions)
        {
            if (!claimById.TryGetValue(submission.ClaimCaseId, out var claim)
                || !policyById.TryGetValue(submission.PolicyId, out var policy))
            {
                messageKey = UiTextKeys.ProductHistoryReferenceMessage;
                return false;
            }

            if (!ClaimSubmissionValues.Statuses.Contains(submission.Status, StringComparer.Ordinal))
            {
                messageKey = UiTextKeys.ProductHistoryUnknownStatusMessage;
                return false;
            }

            if (!string.Equals(claim.CaseStatus, ClaimCaseValues.StatusSaved, StringComparison.Ordinal))
            {
                messageKey = UiTextKeys.ProductHistoryReferenceMessage;
                return false;
            }

            if (!string.Equals(claim.FamilyMemberId, policy.FamilyMemberId, StringComparison.Ordinal))
            {
                messageKey = UiTextKeys.ProductHistoryOwnershipMessage;
                return false;
            }
        }

        foreach (var payment in payments)
        {
            if (!submissionById.ContainsKey(payment.ClaimSubmissionId))
            {
                messageKey = UiTextKeys.ProductHistoryReferenceMessage;
                return false;
            }

            if (!ClaimPaymentValues.Statuses.Contains(payment.Status, StringComparer.Ordinal))
            {
                messageKey = UiTextKeys.ProductHistoryUnknownStatusMessage;
                return false;
            }
        }

        messageKey = string.Empty;
        return true;
    }

    private HomeDashboardProjection CreateProjection(
        ClaimRecord claim,
        FamilyMemberRecord family,
        PolicyRecord policy,
        ClaimSubmissionRecord submission,
        IReadOnlyList<ClaimPaymentRecord> payments)
    {
        var updatedAt = payments.Aggregate(
            submission.UpdatedAt,
            static (latest, payment) => payment.UpdatedAt > latest ? payment.UpdatedAt : latest);
        var parentState = claim.DisabledAt is not null
            || family.DisabledAt is not null
            || policy.DisabledAt is not null
            ? uiTextProvider.Get(UiTextKeys.ProductHistoryDisabledState)
            : uiTextProvider.Get(UiTextKeys.ProductHistoryActiveState);

        return new HomeDashboardProjection(
            submission.Id,
            claim.ReferenceDate,
            updatedAt,
            new HomeDashboardRecentActivityViewModel(
                family.DisplayName,
                claim.ReferenceDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                DisplayOrFallback(claim.HospitalName),
                DisplayOrFallback(policy.InsurerName),
                policy.DisplayTitle,
                GetSubmissionStatusDisplay(submission.Status),
                CreatePaymentSummary(payments),
                updatedAt.ToLocalTime().ToString("yyyy-MM-dd HH:mm", CultureInfo.CurrentCulture),
                parentState));
    }

    private static bool IsActiveSavedClaim(
        ClaimRecord claim,
        IReadOnlyDictionary<string, FamilyMemberRecord> familyById)
    {
        return string.Equals(claim.CaseStatus, ClaimCaseValues.StatusSaved, StringComparison.Ordinal)
            && claim.DisabledAt is null
            && familyById[claim.FamilyMemberId!].DisabledAt is null;
    }

    private static bool IsActiveSubmission(
        ClaimSubmissionRecord submission,
        ClaimRecord claim,
        IReadOnlyDictionary<string, PolicyRecord> policyById)
    {
        var policy = policyById[submission.PolicyId];
        return policy.DisabledAt is null
            && string.Equals(claim.FamilyMemberId, policy.FamilyMemberId, StringComparison.Ordinal);
    }

    private static bool IsKnownClaimStatus(string? status)
    {
        return string.Equals(status, ClaimCaseValues.StatusDraft, StringComparison.Ordinal)
            || string.Equals(status, ClaimCaseValues.StatusSaved, StringComparison.Ordinal);
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
            _ => uiTextProvider.Get(UiTextKeys.ProductHistoryUnknownStatusMessage)
        };
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

    private static int CountPayments(IReadOnlyList<ClaimPaymentRecord> payments, string status)
    {
        return payments.Count(payment => string.Equals(payment.Status, status, StringComparison.Ordinal));
    }

    private string DisplayOrFallback(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? uiTextProvider.Get(UiTextKeys.ProductClaimCompleteNotEnteredValue)
            : value;
    }

    private bool SetFailure(string messageKey)
    {
        ResetProjection();
        StateMessage = uiTextProvider.Get(messageKey);
        return false;
    }

    private void ResetProjection()
    {
        InProgressClaimCount = 0;
        NoSubmissionClaimCount = 0;
        PaymentResultPendingCount = 0;
        RecentActivities = [];
        HasLoadedProjection = false;
        StateMessage = null;
    }

    private static bool TryBuildDictionary<T>(
        IReadOnlyList<T> records,
        Func<T, string> keySelector,
        out IReadOnlyDictionary<string, T> result)
    {
        var dictionary = new Dictionary<string, T>(StringComparer.Ordinal);
        foreach (var record in records)
        {
            var key = keySelector(record);
            if (string.IsNullOrWhiteSpace(key) || !dictionary.TryAdd(key, record))
            {
                result = new Dictionary<string, T>(StringComparer.Ordinal);
                return false;
            }
        }

        result = dictionary;
        return true;
    }

    private bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
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

    private sealed record HomeDashboardProjection(
        string SortKey,
        DateOnly TreatmentDate,
        DateTimeOffset UpdatedAt,
        HomeDashboardRecentActivityViewModel Activity);
}

public sealed record HomeDashboardRecentActivityViewModel(
    string FamilyDisplayName,
    string TreatmentDateDisplay,
    string HospitalName,
    string InsurerDisplayName,
    string PolicyDisplayTitle,
    string SubmissionStatusDisplay,
    string PaymentSummaryDisplay,
    string UpdatedAtDisplay,
    string ParentStateDisplay);
