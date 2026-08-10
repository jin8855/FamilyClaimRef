using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using FamilyClaimRef.App.Models.Storage;
using FamilyClaimRef.App.Services.Localization;
using FamilyClaimRef.App.Services.Storage;

namespace FamilyClaimRef.App.ViewModels;

public sealed class ClaimHistoryViewModel : INotifyPropertyChanged
{
    private readonly IClaimHistoryStorageReader historyStorageReader;
    private readonly IClaimSubmissionStorageService submissionStorageService;
    private readonly IClaimPaymentStorageService paymentStorageService;
    private readonly IFamilyMemberStorageService familyMemberStorageService;
    private readonly IUiTextProvider uiTextProvider;
    private readonly SemaphoreSlim operationGate = new(1, 1);
    private IReadOnlyList<ClaimHistoryProjection> allProjections = [];
    private IReadOnlyList<ClaimHistoryListItemViewModel> items = [];
    private IReadOnlyList<ClaimHistoryFilterOptionViewModel> familyFilterOptions = [];
    private IReadOnlyList<ClaimHistoryFilterOptionViewModel> insurerFilterOptions = [];
    private readonly IReadOnlyList<ClaimHistoryFilterOptionViewModel> visitTypeFilterOptions;
    private ClaimHistoryFilterOptionViewModel? selectedFamilyFilter;
    private ClaimHistoryFilterOptionViewModel? selectedInsurerFilter;
    private ClaimHistoryFilterOptionViewModel? selectedVisitTypeFilter;
    private DateTime? treatmentDateFrom;
    private DateTime? treatmentDateTo;
    private string? searchText;
    private string? claimCaseScopeId;
    private string? selectedSubmissionKey;
    private ClaimHistoryDetailViewModel? selectedDetail;
    private string? stateMessage;
    private bool isBusy;
    private bool hasLoadedProjection;

    public ClaimHistoryViewModel(
        IClaimHistoryStorageReader historyStorageReader,
        IClaimSubmissionStorageService submissionStorageService,
        IClaimPaymentStorageService paymentStorageService,
        IFamilyMemberStorageService familyMemberStorageService,
        IUiTextProvider uiTextProvider)
    {
        this.historyStorageReader = historyStorageReader
            ?? throw new ArgumentNullException(nameof(historyStorageReader));
        this.submissionStorageService = submissionStorageService
            ?? throw new ArgumentNullException(nameof(submissionStorageService));
        this.paymentStorageService = paymentStorageService
            ?? throw new ArgumentNullException(nameof(paymentStorageService));
        this.familyMemberStorageService = familyMemberStorageService
            ?? throw new ArgumentNullException(nameof(familyMemberStorageService));
        this.uiTextProvider = uiTextProvider
            ?? throw new ArgumentNullException(nameof(uiTextProvider));
        visitTypeFilterOptions =
        [
            new(null, uiTextProvider.Get(UiTextKeys.ProductHistoryAllOption)),
            new(ClaimCaseValues.VisitTypeOutpatient, uiTextProvider.Get(UiTextKeys.ProductClaimCaseVisitTypeOutpatient)),
            new(ClaimCaseValues.VisitTypeInpatient, uiTextProvider.Get(UiTextKeys.ProductClaimCaseVisitTypeInpatient))
        ];
        selectedVisitTypeFilter = visitTypeFilterOptions[0];
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public bool IsBusy
    {
        get => isBusy;
        private set => SetProperty(ref isBusy, value);
    }

    public bool HasItems => Items.Count > 0;

    public bool HasStateMessage => !string.IsNullOrWhiteSpace(StateMessage);

    public bool HasDetail => SelectedDetail is not null;

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

    public string? ClaimCaseScopeId
    {
        get => claimCaseScopeId;
        private set => SetProperty(ref claimCaseScopeId, value);
    }

    public IReadOnlyList<ClaimHistoryListItemViewModel> Items
    {
        get => items;
        private set
        {
            if (SetProperty(ref items, value))
            {
                OnPropertyChanged(nameof(HasItems));
            }
        }
    }

    public IReadOnlyList<ClaimHistoryFilterOptionViewModel> FamilyFilterOptions
    {
        get => familyFilterOptions;
        private set => SetProperty(ref familyFilterOptions, value);
    }

    public IReadOnlyList<ClaimHistoryFilterOptionViewModel> InsurerFilterOptions
    {
        get => insurerFilterOptions;
        private set => SetProperty(ref insurerFilterOptions, value);
    }

    public IReadOnlyList<ClaimHistoryFilterOptionViewModel> VisitTypeFilterOptions => visitTypeFilterOptions;

    public ClaimHistoryFilterOptionViewModel? SelectedFamilyFilter
    {
        get => selectedFamilyFilter;
        set => SetProperty(ref selectedFamilyFilter, value);
    }

    public ClaimHistoryFilterOptionViewModel? SelectedInsurerFilter
    {
        get => selectedInsurerFilter;
        set => SetProperty(ref selectedInsurerFilter, value);
    }

    public ClaimHistoryFilterOptionViewModel? SelectedVisitTypeFilter
    {
        get => selectedVisitTypeFilter;
        set => SetProperty(ref selectedVisitTypeFilter, value);
    }

    public DateTime? TreatmentDateFrom
    {
        get => treatmentDateFrom;
        set => SetProperty(ref treatmentDateFrom, value);
    }

    public DateTime? TreatmentDateTo
    {
        get => treatmentDateTo;
        set => SetProperty(ref treatmentDateTo, value);
    }

    public string? SearchText
    {
        get => searchText;
        set => SetProperty(ref searchText, value);
    }

    public ClaimHistoryDetailViewModel? SelectedDetail
    {
        get => selectedDetail;
        private set
        {
            if (SetProperty(ref selectedDetail, value))
            {
                OnPropertyChanged(nameof(HasDetail));
            }
        }
    }

    public void SetClaimCaseScope(string? claimCaseId, bool resetFilters)
    {
        ClaimCaseScopeId = string.IsNullOrWhiteSpace(claimCaseId) ? null : claimCaseId;
        if (resetFilters)
        {
            ResetFilterValues();
            selectedSubmissionKey = null;
            SelectedDetail = null;
        }
    }

    public async Task<bool> LoadAsync(CancellationToken cancellationToken = default)
    {
        if (!await operationGate.WaitAsync(0, cancellationToken))
        {
            return false;
        }

        var preservedSelection = selectedSubmissionKey;
        try
        {
            IsBusy = true;
            ResetProjection();

            var claims = await historyStorageReader.GetAllClaimCasesForHistoryAsync(cancellationToken);
            var policies = await historyStorageReader.GetAllPoliciesForHistoryAsync(cancellationToken);
            var families = await familyMemberStorageService.GetFamilyMembersAsync(cancellationToken);
            var policyById = policies.ToDictionary(policy => policy.Id, StringComparer.Ordinal);
            var familyById = families.ToDictionary(family => family.Id, StringComparer.Ordinal);

            if (ClaimCaseScopeId is not null
                && !claims.Any(claim => string.Equals(claim.Id, ClaimCaseScopeId, StringComparison.Ordinal)))
            {
                return SetFailure(UiTextKeys.ProductHistoryReferenceMessage);
            }

            var projections = new List<ClaimHistoryProjection>();
            foreach (var claim in claims.Where(IsInScope))
            {
                if (!IsKnownClaimStatus(claim.CaseStatus))
                {
                    return SetFailure(UiTextKeys.ProductHistoryUnknownStatusMessage);
                }

                if (string.IsNullOrWhiteSpace(claim.FamilyMemberId))
                {
                    return SetFailure(UiTextKeys.ProductHistoryLegacyReviewMessage);
                }

                if (!familyById.TryGetValue(claim.FamilyMemberId, out var family))
                {
                    return SetFailure(UiTextKeys.ProductHistoryReferenceMessage);
                }

                var submissions = await submissionStorageService.GetByClaimCaseAsync(
                    claim.Id,
                    cancellationToken);
                if (submissions.Count > 0
                    && !string.Equals(claim.CaseStatus, ClaimCaseValues.StatusSaved, StringComparison.Ordinal))
                {
                    return SetFailure(UiTextKeys.ProductHistoryReferenceMessage);
                }

                foreach (var submission in submissions)
                {
                    if (!string.Equals(submission.ClaimCaseId, claim.Id, StringComparison.Ordinal)
                        || string.IsNullOrWhiteSpace(submission.PolicyId))
                    {
                        return SetFailure(UiTextKeys.ProductHistoryReferenceMessage);
                    }

                    if (!ClaimSubmissionValues.Statuses.Contains(submission.Status, StringComparer.Ordinal))
                    {
                        return SetFailure(UiTextKeys.ProductHistoryUnknownStatusMessage);
                    }

                    if (!policyById.TryGetValue(submission.PolicyId, out var policy))
                    {
                        return SetFailure(UiTextKeys.ProductHistoryReferenceMessage);
                    }

                    if (string.IsNullOrWhiteSpace(policy.FamilyMemberId))
                    {
                        return SetFailure(UiTextKeys.ProductHistoryLegacyReviewMessage);
                    }

                    if (!string.Equals(policy.FamilyMemberId, claim.FamilyMemberId, StringComparison.Ordinal))
                    {
                        return SetFailure(UiTextKeys.ProductHistoryOwnershipMessage);
                    }

                    var payments = await paymentStorageService.GetBySubmissionAsync(
                        submission.Id,
                        cancellationToken);
                    if (payments.Any(payment => !string.Equals(
                        payment.ClaimSubmissionId,
                        submission.Id,
                        StringComparison.Ordinal)))
                    {
                        return SetFailure(UiTextKeys.ProductHistoryReferenceMessage);
                    }

                    if (payments.Any(payment => !ClaimPaymentValues.Statuses.Contains(
                        payment.Status,
                        StringComparer.Ordinal)))
                    {
                        return SetFailure(UiTextKeys.ProductHistoryUnknownStatusMessage);
                    }

                    projections.Add(CreateProjection(claim, family, policy, submission, payments));
                }
            }

            allProjections = projections
                .OrderByDescending(projection => projection.TreatmentDate)
                .ThenByDescending(projection => projection.SubmissionUpdatedAt)
                .ThenBy(projection => projection.SubmissionKey, StringComparer.Ordinal)
                .ToArray();
            CreateFilterOptions();
            hasLoadedProjection = true;
            ApplyFiltersCore(preservedSelection);
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
    public bool ApplyFilters()
    {
        if (!hasLoadedProjection)
        {
            return false;
        }

        return ApplyFiltersCore(selectedSubmissionKey);
    }

    public void ResetFilters()
    {
        ResetFilterValues();
        if (hasLoadedProjection)
        {
            ApplyFiltersCore(selectedSubmissionKey);
        }
    }

    public bool SelectItem(ClaimHistoryListItemViewModel item)
    {
        ArgumentNullException.ThrowIfNull(item);
        var projection = allProjections.FirstOrDefault(candidate => string.Equals(
            candidate.SubmissionKey,
            item.SubmissionKey,
            StringComparison.Ordinal));
        if (projection is null || !Items.Any(candidate => string.Equals(
            candidate.SubmissionKey,
            item.SubmissionKey,
            StringComparison.Ordinal)))
        {
            selectedSubmissionKey = null;
            SelectedDetail = null;
            return false;
        }

        selectedSubmissionKey = projection.SubmissionKey;
        SelectedDetail = projection.Detail;
        return true;
    }

    private bool ApplyFiltersCore(string? preservedSelection)
    {
        StateMessage = null;
        var from = TreatmentDateFrom is null
            ? (DateOnly?)null
            : DateOnly.FromDateTime(TreatmentDateFrom.Value.Date);
        var to = TreatmentDateTo is null
            ? (DateOnly?)null
            : DateOnly.FromDateTime(TreatmentDateTo.Value.Date);
        if (from is not null && to is not null && from > to)
        {
            Items = [];
            selectedSubmissionKey = null;
            SelectedDetail = null;
            StateMessage = uiTextProvider.Get(UiTextKeys.ProductHistoryDateRangeMessage);
            return false;
        }

        var normalizedSearch = SearchText?.Trim();
        var filtered = allProjections
            .Where(projection => SelectedFamilyFilter?.Value is null
                || string.Equals(projection.FamilyId, SelectedFamilyFilter.Value, StringComparison.Ordinal))
            .Where(projection => SelectedInsurerFilter?.Value is null
                || string.Equals(projection.InsurerName, SelectedInsurerFilter.Value, StringComparison.Ordinal))
            .Where(projection => SelectedVisitTypeFilter?.Value is null
                || string.Equals(projection.VisitType, SelectedVisitTypeFilter.Value, StringComparison.Ordinal))
            .Where(projection => from is null || projection.TreatmentDate >= from)
            .Where(projection => to is null || projection.TreatmentDate <= to)
            .Where(projection => string.IsNullOrWhiteSpace(normalizedSearch)
                || projection.SearchText.Contains(normalizedSearch, StringComparison.OrdinalIgnoreCase))
            .ToArray();
        Items = filtered.Select(projection => projection.ListItem).ToArray();

        var selected = filtered.FirstOrDefault(projection => string.Equals(
            projection.SubmissionKey,
            preservedSelection,
            StringComparison.Ordinal));
        selectedSubmissionKey = selected?.SubmissionKey;
        SelectedDetail = selected?.Detail;
        if (Items.Count == 0)
        {
            StateMessage = uiTextProvider.Get(
                allProjections.Count == 0
                    ? UiTextKeys.ProductHistoryEmptyMessage
                    : UiTextKeys.ProductHistoryFilterEmptyMessage);
        }

        return true;
    }

    private ClaimHistoryProjection CreateProjection(
        ClaimRecord claim,
        FamilyMemberRecord family,
        PolicyRecord policy,
        ClaimSubmissionRecord submission,
        IReadOnlyList<ClaimPaymentRecord> payments)
    {
        var submissionStatus = GetSubmissionStatusDisplay(submission.Status);
        var visitType = GetVisitTypeDisplay(claim.VisitType);
        var parentState = claim.DisabledAt is not null
            || family.DisabledAt is not null
            || policy.DisabledAt is not null
            ? uiTextProvider.Get(UiTextKeys.ProductHistoryDisabledState)
            : uiTextProvider.Get(UiTextKeys.ProductHistoryActiveState);
        var insurerName = DisplayOrFallback(policy.InsurerName);
        var paymentSummary = CreatePaymentSummary(payments);
        var treatmentDate = claim.ReferenceDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        var listItem = new ClaimHistoryListItemViewModel(
            submission.Id,
            treatmentDate,
            family.DisplayName,
            insurerName,
            policy.DisplayTitle,
            claim.DisplayTitle,
            visitType,
            submissionStatus,
            paymentSummary,
            parentState,
            submission.UpdatedAt.ToLocalTime().ToString("yyyy-MM-dd HH:mm", CultureInfo.CurrentCulture));
        var paymentItems = payments
            .OrderByDescending(payment => payment.UpdatedAt)
            .ThenBy(payment => payment.Id, StringComparer.Ordinal)
            .Select(payment => new ClaimHistoryPaymentItemViewModel(
                GetPaymentStatusDisplay(payment.Status),
                payment.PaidDate?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)
                    ?? DisplayOrFallback(null),
                FormatAmount(payment.PaidAmount),
                DisplayOrFallback(payment.PaidCoverageDisplayName),
                DisplayOrFallback(payment.DenyReason),
                DisplayOrFallback(payment.ReductionReason),
                DisplayOrFallback(payment.AdditionalDocumentsMemo),
                DisplayOrFallback(payment.Memo),
                payment.UpdatedAt.ToLocalTime().ToString("yyyy-MM-dd HH:mm", CultureInfo.CurrentCulture)))
            .ToArray();
        var detail = new ClaimHistoryDetailViewModel(
            claim.DisplayTitle,
            family.DisplayName,
            treatmentDate,
            DisplayOrFallback(claim.HospitalName),
            CreateDiagnosisDisplay(claim),
            visitType,
            GetClaimStatusDisplay(claim.CaseStatus),
            parentState,
            policy.DisplayTitle,
            insurerName,
            submissionStatus,
            submission.SubmittedDate?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)
                ?? DisplayOrFallback(null),
            FormatAmount(submission.SubmittedAmount),
            DisplayOrFallback(submission.CoverageDisplayName),
            DisplayOrFallback(submission.Memo),
            submission.UpdatedAt.ToLocalTime().ToString("yyyy-MM-dd HH:mm", CultureInfo.CurrentCulture),
            paymentItems);
        var searchText = string.Join(
            " ",
            family.DisplayName,
            insurerName,
            policy.DisplayTitle,
            claim.DisplayTitle,
            claim.HospitalName,
            claim.DiagnosisCode,
            claim.DiagnosisName,
            visitType,
            submissionStatus,
            paymentSummary,
            parentState,
            submission.CoverageDisplayName,
            submission.Memo);
        return new ClaimHistoryProjection(
            submission.Id,
            claim.FamilyMemberId!,
            insurerName,
            claim.ReferenceDate,
            claim.VisitType ?? string.Empty,
            submission.UpdatedAt,
            searchText,
            listItem,
            detail);
    }

    private void CreateFilterOptions()
    {
        var allLabel = uiTextProvider.Get(UiTextKeys.ProductHistoryAllOption);
        var selectedFamilyValue = SelectedFamilyFilter?.Value;
        var selectedInsurerValue = SelectedInsurerFilter?.Value;
        FamilyFilterOptions = new[] { new ClaimHistoryFilterOptionViewModel(null, allLabel) }
            .Concat(allProjections
                .GroupBy(projection => projection.FamilyId, StringComparer.Ordinal)
                .Select(group => new ClaimHistoryFilterOptionViewModel(
                    group.Key,
                    group.First().ListItem.FamilyDisplayName))
                .OrderBy(option => option.DisplayName, StringComparer.CurrentCulture))
            .ToArray();
        InsurerFilterOptions = new[] { new ClaimHistoryFilterOptionViewModel(null, allLabel) }
            .Concat(allProjections
                .Select(projection => projection.InsurerName)
                .Distinct(StringComparer.Ordinal)
                .Order(StringComparer.CurrentCulture)
                .Select(value => new ClaimHistoryFilterOptionViewModel(value, value)))
            .ToArray();
        SelectedFamilyFilter = FamilyFilterOptions.FirstOrDefault(option => string.Equals(
                option.Value,
                selectedFamilyValue,
                StringComparison.Ordinal))
            ?? FamilyFilterOptions[0];
        SelectedInsurerFilter = InsurerFilterOptions.FirstOrDefault(option => string.Equals(
                option.Value,
                selectedInsurerValue,
                StringComparison.Ordinal))
            ?? InsurerFilterOptions[0];
        SelectedVisitTypeFilter ??= VisitTypeFilterOptions[0];
    }

    private bool IsInScope(ClaimRecord claim)
    {
        return ClaimCaseScopeId is null
            || string.Equals(claim.Id, ClaimCaseScopeId, StringComparison.Ordinal);
    }

    private static bool IsKnownClaimStatus(string? status)
    {
        return string.Equals(status, ClaimCaseValues.StatusDraft, StringComparison.Ordinal)
            || string.Equals(status, ClaimCaseValues.StatusSaved, StringComparison.Ordinal);
    }

    private string GetClaimStatusDisplay(string? status)
    {
        return string.Equals(status, ClaimCaseValues.StatusDraft, StringComparison.Ordinal)
            ? uiTextProvider.Get(UiTextKeys.ProductHistoryClaimStatusDraft)
            : uiTextProvider.Get(UiTextKeys.ProductClaimCompleteCaseStatusSaved);
    }

    private string GetVisitTypeDisplay(string? visitType)
    {
        return visitType switch
        {
            ClaimCaseValues.VisitTypeOutpatient => uiTextProvider.Get(UiTextKeys.ProductClaimCaseVisitTypeOutpatient),
            ClaimCaseValues.VisitTypeInpatient => uiTextProvider.Get(UiTextKeys.ProductClaimCaseVisitTypeInpatient),
            _ => DisplayOrFallback(null)
        };
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

    private string GetPaymentStatusDisplay(string status)
    {
        return status switch
        {
            ClaimPaymentValues.StatusPending => uiTextProvider.Get(UiTextKeys.ProductClaimPaymentStatusPending),
            ClaimPaymentValues.StatusPaid => uiTextProvider.Get(UiTextKeys.ProductClaimPaymentStatusPaid),
            ClaimPaymentValues.StatusPartiallyPaid => uiTextProvider.Get(UiTextKeys.ProductClaimPaymentStatusPartiallyPaid),
            ClaimPaymentValues.StatusDenied => uiTextProvider.Get(UiTextKeys.ProductClaimPaymentStatusDenied),
            ClaimPaymentValues.StatusCancelled => uiTextProvider.Get(UiTextKeys.ProductClaimPaymentStatusCancelled),
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

    private string CreateDiagnosisDisplay(ClaimRecord claim)
    {
        var display = string.Join(
            " / ",
            new[] { claim.DiagnosisCode, claim.DiagnosisName }
                .Where(value => !string.IsNullOrWhiteSpace(value)));
        return string.IsNullOrWhiteSpace(display) ? DisplayOrFallback(null) : display;
    }

    private string DisplayOrFallback(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? uiTextProvider.Get(UiTextKeys.ProductClaimCompleteNotEnteredValue)
            : value;
    }

    private string FormatAmount(long? value)
    {
        return value?.ToString("N0", CultureInfo.CurrentCulture)
            ?? DisplayOrFallback(null);
    }

    private static int CountPayments(IEnumerable<ClaimPaymentRecord> payments, string status)
    {
        return payments.Count(payment => string.Equals(payment.Status, status, StringComparison.Ordinal));
    }

    private bool SetFailure(string messageKey)
    {
        ResetProjection();
        StateMessage = uiTextProvider.Get(messageKey);
        return false;
    }

    private void ResetProjection()
    {
        allProjections = [];
        Items = [];
        FamilyFilterOptions = [];
        InsurerFilterOptions = [];
        selectedSubmissionKey = null;
        SelectedDetail = null;
        StateMessage = null;
        hasLoadedProjection = false;
    }

    private void ResetFilterValues()
    {
        SelectedFamilyFilter = null;
        SelectedInsurerFilter = null;
        SelectedVisitTypeFilter = VisitTypeFilterOptions[0];
        TreatmentDateFrom = null;
        TreatmentDateTo = null;
        SearchText = null;
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

    private sealed record ClaimHistoryProjection(
        string SubmissionKey,
        string FamilyId,
        string InsurerName,
        DateOnly TreatmentDate,
        string VisitType,
        DateTimeOffset SubmissionUpdatedAt,
        string SearchText,
        ClaimHistoryListItemViewModel ListItem,
        ClaimHistoryDetailViewModel Detail);
}

public sealed record ClaimHistoryFilterOptionViewModel(
    string? Value,
    string DisplayName);

public sealed record ClaimHistoryListItemViewModel(
    string SubmissionKey,
    string TreatmentDateDisplay,
    string FamilyDisplayName,
    string InsurerDisplayName,
    string PolicyDisplayTitle,
    string ClaimDisplayTitle,
    string VisitTypeDisplay,
    string SubmissionStatusDisplay,
    string PaymentSummaryDisplay,
    string ParentStateDisplay,
    string UpdatedAtDisplay);

public sealed record ClaimHistoryDetailViewModel(
    string ClaimDisplayTitle,
    string FamilyDisplayName,
    string TreatmentDateDisplay,
    string HospitalName,
    string DiagnosisDisplay,
    string VisitTypeDisplay,
    string ClaimStatusDisplay,
    string ParentStateDisplay,
    string PolicyDisplayTitle,
    string InsurerDisplayName,
    string SubmissionStatusDisplay,
    string SubmittedDateDisplay,
    string SubmittedAmountDisplay,
    string CoverageDisplayName,
    string SubmissionMemo,
    string SubmissionUpdatedAtDisplay,
    IReadOnlyList<ClaimHistoryPaymentItemViewModel> Payments)
{
    public bool HasPayments => Payments.Count > 0;
}

public sealed record ClaimHistoryPaymentItemViewModel(
    string StatusDisplay,
    string PaidDateDisplay,
    string PaidAmountDisplay,
    string CoverageDisplayName,
    string DenyReason,
    string ReductionReason,
    string AdditionalDocumentsMemo,
    string Memo,
    string UpdatedAtDisplay);
