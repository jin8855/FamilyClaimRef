using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using FamilyClaimRef.App.Models.Matching;
using FamilyClaimRef.App.Models.Storage;
using FamilyClaimRef.App.Services.Localization;
using FamilyClaimRef.App.Services.Matching;
using FamilyClaimRef.App.Services.Storage;

namespace FamilyClaimRef.App.ViewModels;

public sealed class ClaimReferenceResultViewModel : INotifyPropertyChanged
{
    private readonly IClaimReferenceMatchingEngine matchingEngine;
    private readonly IFamilyMemberStorageService familyMemberStorageService;
    private readonly IClaimHistoryStorageReader historyStorageReader;
    private readonly IPolicyCoverageStorageService policyCoverageStorageService;
    private readonly IClaimSubmissionHistoryStorageReader submissionHistoryStorageReader;
    private readonly IClaimPaymentHistoryStorageReader paymentHistoryStorageReader;
    private readonly IDocumentStorageService documentStorageService;
    private readonly IUiTextProvider uiTextProvider;
    private readonly SemaphoreSlim operationGate = new(1, 1);
    private string? selectedClaimCaseId;
    private bool isBusy;
    private ClaimReferenceResultState state = ClaimReferenceResultState.Initial;
    private string? stateMessage;
    private string? selectedClaimDisplayTitle;
    private IReadOnlyList<ClaimReferenceCoverageResultItemViewModel> coverageResults = [];
    private IReadOnlyList<ClaimReferenceSimilarClaimItemViewModel> similarClaims = [];
    private bool hasExcludedUnconfirmedCoverages;

    public ClaimReferenceResultViewModel(
        IClaimReferenceMatchingEngine matchingEngine,
        IFamilyMemberStorageService familyMemberStorageService,
        IClaimHistoryStorageReader historyStorageReader,
        IPolicyCoverageStorageService policyCoverageStorageService,
        IClaimSubmissionHistoryStorageReader submissionHistoryStorageReader,
        IClaimPaymentHistoryStorageReader paymentHistoryStorageReader,
        IDocumentStorageService documentStorageService,
        IUiTextProvider uiTextProvider)
    {
        this.matchingEngine = matchingEngine ?? throw new ArgumentNullException(nameof(matchingEngine));
        this.familyMemberStorageService = familyMemberStorageService ?? throw new ArgumentNullException(nameof(familyMemberStorageService));
        this.historyStorageReader = historyStorageReader ?? throw new ArgumentNullException(nameof(historyStorageReader));
        this.policyCoverageStorageService = policyCoverageStorageService ?? throw new ArgumentNullException(nameof(policyCoverageStorageService));
        this.submissionHistoryStorageReader = submissionHistoryStorageReader ?? throw new ArgumentNullException(nameof(submissionHistoryStorageReader));
        this.paymentHistoryStorageReader = paymentHistoryStorageReader ?? throw new ArgumentNullException(nameof(paymentHistoryStorageReader));
        this.documentStorageService = documentStorageService ?? throw new ArgumentNullException(nameof(documentStorageService));
        this.uiTextProvider = uiTextProvider ?? throw new ArgumentNullException(nameof(uiTextProvider));
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

    public ClaimReferenceResultState State
    {
        get => state;
        private set => SetProperty(ref state, value);
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

    public string? SelectedClaimDisplayTitle
    {
        get => selectedClaimDisplayTitle;
        private set => SetProperty(ref selectedClaimDisplayTitle, value);
    }

    public IReadOnlyList<ClaimReferenceCoverageResultItemViewModel> CoverageResults
    {
        get => coverageResults;
        private set
        {
            if (SetProperty(ref coverageResults, value))
            {
                OnPropertyChanged(nameof(HasCoverageResults));
                OnPropertyChanged(nameof(HasResults));
            }
        }
    }

    public IReadOnlyList<ClaimReferenceSimilarClaimItemViewModel> SimilarClaims
    {
        get => similarClaims;
        private set
        {
            if (SetProperty(ref similarClaims, value))
            {
                OnPropertyChanged(nameof(HasSimilarClaims));
                OnPropertyChanged(nameof(HasResults));
            }
        }
    }

    public bool HasCoverageResults => CoverageResults.Count > 0;

    public bool HasSimilarClaims => SimilarClaims.Count > 0;

    public bool HasResults => HasCoverageResults || HasSimilarClaims;

    public bool HasExcludedUnconfirmedCoverages
    {
        get => hasExcludedUnconfirmedCoverages;
        private set => SetProperty(ref hasExcludedUnconfirmedCoverages, value);
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
                State = ClaimReferenceResultState.Initial;
                StateMessage = uiTextProvider.Get(UiTextKeys.ProductClaimReferenceInitialMessage);
                return false;
            }

            var families = await familyMemberStorageService.GetFamilyMembersAsync(cancellationToken);
            var policies = await historyStorageReader.GetAllPoliciesForHistoryAsync(cancellationToken);
            var coverages = await policyCoverageStorageService.GetPolicyCoveragesAsync(cancellationToken);
            var claims = await historyStorageReader.GetAllClaimCasesForHistoryAsync(cancellationToken);
            var submissions = await submissionHistoryStorageReader.GetAllSubmissionsForHistoryAsync(cancellationToken);
            var payments = await paymentHistoryStorageReader.GetAllPaymentsForHistoryAsync(cancellationToken);
            var policyDocuments = await GetPolicyDocumentsAsync(policies, cancellationToken);
            var request = new ClaimReferenceMatchingRequest(
                SelectedClaimCaseId,
                AnchorPolicyCoverageId: null,
                families,
                policies,
                coverages,
                claims,
                submissions,
                payments,
                policyDocuments);
            var projection = matchingEngine.BuildProjection(request);
            var selectedClaim = claims.FirstOrDefault(claim => string.Equals(
                claim.Id,
                SelectedClaimCaseId,
                StringComparison.Ordinal));

            SelectedClaimDisplayTitle = selectedClaim?.DisplayTitle;
            CoverageResults = projection.CoverageResults
                .Select(CreateCoverageItem)
                .ToArray();
            SimilarClaims = projection.SimilarClaims
                .Select(CreateSimilarClaimItem)
                .ToArray();
            HasExcludedUnconfirmedCoverages = projection.HasExcludedUnconfirmedCoverages;
            State = HasResults
                ? ClaimReferenceResultState.Populated
                : ClaimReferenceResultState.Empty;
            StateMessage = State == ClaimReferenceResultState.Empty
                ? uiTextProvider.Get(UiTextKeys.ProductClaimReferenceEmptyMessage)
                : null;
            return true;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (ClaimReferenceMatchingException)
        {
            ResetProjection();
            State = ClaimReferenceResultState.DomainError;
            StateMessage = uiTextProvider.Get(UiTextKeys.ProductClaimReferenceDomainErrorMessage);
            return false;
        }
        catch
        {
            ResetProjection();
            State = ClaimReferenceResultState.UnexpectedError;
            StateMessage = uiTextProvider.Get(UiTextKeys.ProductClaimReferenceUnexpectedErrorMessage);
            return false;
        }
        finally
        {
            IsBusy = false;
            operationGate.Release();
        }
    }

    private async Task<IReadOnlyList<PolicyDocumentRecord>> GetPolicyDocumentsAsync(
        IReadOnlyList<PolicyRecord> policies,
        CancellationToken cancellationToken)
    {
        var documents = new List<PolicyDocumentRecord>();
        foreach (var policy in policies)
        {
            documents.AddRange(await documentStorageService.GetPolicyDocumentsAsync(
                policy.Id,
                cancellationToken));
        }

        return documents;
    }

    private ClaimReferenceCoverageResultItemViewModel CreateCoverageItem(
        ClaimReferenceCoverageResult result)
    {
        return new ClaimReferenceCoverageResultItemViewModel(
            result.PolicyDisplayName,
            result.CoverageDisplayName,
            GetResultGroupDisplay(result.ResultGroup),
            string.Join(
                ", ",
                result.RuleEvidence.Select(evidence => $"{GetRuleDisplay(evidence.RuleName)}: {GetOutcomeDisplay(evidence.Outcome)}")),
            result.HasSourcePolicyDocument
                ? uiTextProvider.Get(UiTextKeys.ProductClaimReferenceSourceDocumentAvailable)
                : uiTextProvider.Get(UiTextKeys.ProductClaimReferenceSourceDocumentUnavailable));
    }

    private ClaimReferenceSimilarClaimItemViewModel CreateSimilarClaimItem(
        ClaimReferenceSimilarClaim result)
    {
        return new ClaimReferenceSimilarClaimItemViewModel(
            result.PolicyDisplayName,
            uiTextProvider.Format(UiTextKeys.ProductClaimReferenceSimilarityFormat, result.SimilarityTier),
            result.TreatmentDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
            GetPaymentSummaryDisplay(result.TerminalPaymentFacts));
    }

    private string GetPaymentSummaryDisplay(IReadOnlyList<ClaimReferencePaymentFact> payments)
    {
        if (payments.Count == 0)
        {
            return uiTextProvider.Get(UiTextKeys.ProductClaimReferenceNoPaymentValue);
        }

        return string.Join(
            ", ",
            payments.Select(payment => GetPaymentStatusDisplay(payment.Status)));
    }

    private string GetResultGroupDisplay(string value) => value switch
    {
        ClaimReferenceMatchingValues.ResultGroupConditionMatch => uiTextProvider.Get(UiTextKeys.ProductClaimReferenceResultGroupMatch),
        ClaimReferenceMatchingValues.ResultGroupNeedsConfirmation => uiTextProvider.Get(UiTextKeys.ProductClaimReferenceResultGroupNeedsConfirmation),
        ClaimReferenceMatchingValues.ResultGroupCurrentInputMismatch => uiTextProvider.Get(UiTextKeys.ProductClaimReferenceResultGroupMismatch),
        _ => uiTextProvider.Get(UiTextKeys.ProductClaimReferenceResultGroupUnknown)
    };

    private string GetRuleDisplay(string value) => value switch
    {
        ClaimReferenceMatchingValues.RulePolicyStatus => uiTextProvider.Get(UiTextKeys.ProductClaimReferenceRulePolicyStatus),
        ClaimReferenceMatchingValues.RuleTreatmentDate => uiTextProvider.Get(UiTextKeys.ProductClaimReferenceRuleTreatmentDate),
        ClaimReferenceMatchingValues.RuleVisitType => uiTextProvider.Get(UiTextKeys.ProductClaimReferenceRuleVisitType),
        ClaimReferenceMatchingValues.RuleSurgery => uiTextProvider.Get(UiTextKeys.ProductClaimReferenceRuleSurgery),
        ClaimReferenceMatchingValues.RulePrescription => uiTextProvider.Get(UiTextKeys.ProductClaimReferenceRulePrescription),
        ClaimReferenceMatchingValues.RuleDiagnosisCode => uiTextProvider.Get(UiTextKeys.ProductClaimReferenceRuleDiagnosisCode),
        ClaimReferenceMatchingValues.RuleSourceDocument => uiTextProvider.Get(UiTextKeys.ProductClaimReferenceRuleSourceDocument),
        _ => uiTextProvider.Get(UiTextKeys.ProductClaimReferenceRuleUnknown)
    };

    private string GetOutcomeDisplay(string value) => value switch
    {
        ClaimReferenceMatchingValues.OutcomePassed => uiTextProvider.Get(UiTextKeys.ProductClaimReferenceOutcomePassed),
        ClaimReferenceMatchingValues.OutcomeNeedsConfirmation => uiTextProvider.Get(UiTextKeys.ProductClaimReferenceOutcomeNeedsConfirmation),
        ClaimReferenceMatchingValues.OutcomeMismatch => uiTextProvider.Get(UiTextKeys.ProductClaimReferenceOutcomeMismatch),
        ClaimReferenceMatchingValues.OutcomeNotApplicable => uiTextProvider.Get(UiTextKeys.ProductClaimReferenceOutcomeNotApplicable),
        _ => uiTextProvider.Get(UiTextKeys.ProductClaimReferenceOutcomeUnknown)
    };

    private string GetPaymentStatusDisplay(string value) => value switch
    {
        ClaimPaymentValues.StatusPaid => uiTextProvider.Get(UiTextKeys.ProductClaimPaymentStatusPaid),
        ClaimPaymentValues.StatusPartiallyPaid => uiTextProvider.Get(UiTextKeys.ProductClaimPaymentStatusPartiallyPaid),
        ClaimPaymentValues.StatusDenied => uiTextProvider.Get(UiTextKeys.ProductClaimPaymentStatusDenied),
        ClaimPaymentValues.StatusCancelled => uiTextProvider.Get(UiTextKeys.ProductClaimPaymentStatusCancelled),
        _ => uiTextProvider.Get(UiTextKeys.ProductClaimReferencePaymentUnknownValue)
    };

    private void ResetProjection()
    {
        State = ClaimReferenceResultState.Initial;
        StateMessage = null;
        SelectedClaimDisplayTitle = null;
        CoverageResults = [];
        SimilarClaims = [];
        HasExcludedUnconfirmedCoverages = false;
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
}

public enum ClaimReferenceResultState
{
    Initial,
    Populated,
    Empty,
    DomainError,
    UnexpectedError
}

public sealed record ClaimReferenceCoverageResultItemViewModel(
    string PolicyDisplayName,
    string CoverageDisplayName,
    string ResultGroupDisplay,
    string EvidenceDisplay,
    string SourceDocumentDisplay);

public sealed record ClaimReferenceSimilarClaimItemViewModel(
    string PolicyDisplayName,
    string SimilarityDisplay,
    string TreatmentDateDisplay,
    string PaymentSummaryDisplay);
