using FamilyClaimRef.App.Models.Matching;
using FamilyClaimRef.App.Models.Storage;
using FamilyClaimRef.App.Services.Localization;
using FamilyClaimRef.App.Services.Matching;
using FamilyClaimRef.App.Services.Storage;
using FamilyClaimRef.App.ViewModels;
using Xunit;

namespace FamilyClaimRef.App.Tests;

public sealed class ClaimReferenceResultViewModelTests
{
    private static readonly DateTimeOffset Timestamp = new(2026, 8, 12, 0, 0, 0, TimeSpan.Zero);

    [Fact]
    public async Task LoadAsync_preserves_engine_order_and_displays_only_projection_fields()
    {
        var fixture = new Fixture();
        fixture.Engine.Projection = new ClaimReferenceProjection(
        [
            Coverage("policy-b", "coverage-b", ClaimReferenceMatchingValues.ResultGroupNeedsConfirmation),
            Coverage("policy-a", "coverage-a", ClaimReferenceMatchingValues.ResultGroupConditionMatch)
        ],
        [SimilarClaim("policy-history")],
        HasExcludedUnconfirmedCoverages: true);
        var viewModel = fixture.CreateViewModel();

        var loaded = await viewModel.LoadAsync();

        Assert.True(loaded);
        Assert.Equal(ClaimReferenceResultState.Populated, viewModel.State);
        Assert.Equal(["Policy B", "Policy A"], viewModel.CoverageResults.Select(item => item.PolicyDisplayName));
        Assert.Single(viewModel.SimilarClaims);
        Assert.Equal("current claim", viewModel.SelectedClaimDisplayTitle);
        Assert.True(viewModel.HasExcludedUnconfirmedCoverages);
        Assert.NotNull(fixture.Engine.LastRequest);
        Assert.Equal("claim-current", fixture.Engine.LastRequest!.SelectedClaimCaseId);
        Assert.Equal(fixture.Families.Records, fixture.Engine.LastRequest.FamilyMembers);
        Assert.Equal(fixture.History.Policies, fixture.Engine.LastRequest.Policies);
        Assert.Equal(fixture.Coverages.Records, fixture.Engine.LastRequest.PolicyCoverages);
        Assert.Equal(fixture.History.Claims, fixture.Engine.LastRequest.ClaimCases);
        Assert.Equal(fixture.Submissions.Records, fixture.Engine.LastRequest.ClaimSubmissions);
        Assert.Equal(fixture.Payments.Records, fixture.Engine.LastRequest.ClaimPayments);
    }

    [Fact]
    public async Task LoadAsync_with_empty_projection_sets_empty_not_error()
    {
        var fixture = new Fixture();
        fixture.Engine.Projection = new ClaimReferenceProjection([], [], false);
        var viewModel = fixture.CreateViewModel();

        var loaded = await viewModel.LoadAsync();

        Assert.True(loaded);
        Assert.Equal(ClaimReferenceResultState.Empty, viewModel.State);
        Assert.Empty(viewModel.CoverageResults);
        Assert.Empty(viewModel.SimilarClaims);
        Assert.Equal("empty", viewModel.StateMessage);
    }

    [Fact]
    public async Task LoadAsync_with_domain_failure_clears_prior_projection()
    {
        var fixture = new Fixture();
        fixture.Engine.Projection = new ClaimReferenceProjection([Coverage("policy-a", "coverage-a", ClaimReferenceMatchingValues.ResultGroupConditionMatch)], [], false);
        var viewModel = fixture.CreateViewModel();
        Assert.True(await viewModel.LoadAsync());
        fixture.Engine.Failure = new ClaimReferenceMatchingException(
            ClaimReferenceMatchingErrorCode.InvalidGraph,
            "internal graph detail");

        var loaded = await viewModel.LoadAsync();

        Assert.False(loaded);
        Assert.Equal(ClaimReferenceResultState.DomainError, viewModel.State);
        Assert.Empty(viewModel.CoverageResults);
        Assert.Empty(viewModel.SimilarClaims);
        Assert.Equal("domain", viewModel.StateMessage);
    }

    [Fact]
    public async Task LoadAsync_with_unexpected_failure_clears_prior_projection_and_hides_exception()
    {
        var fixture = new Fixture();
        fixture.Engine.Projection = new ClaimReferenceProjection([Coverage("policy-a", "coverage-a", ClaimReferenceMatchingValues.ResultGroupConditionMatch)], [], false);
        var viewModel = fixture.CreateViewModel();
        Assert.True(await viewModel.LoadAsync());
        fixture.Engine.Failure = new InvalidOperationException("C:\\private\\claim.json");

        var loaded = await viewModel.LoadAsync();

        Assert.False(loaded);
        Assert.Equal(ClaimReferenceResultState.UnexpectedError, viewModel.State);
        Assert.Empty(viewModel.CoverageResults);
        Assert.Empty(viewModel.SimilarClaims);
        Assert.Equal("unexpected", viewModel.StateMessage);
        Assert.DoesNotContain("private", viewModel.StateMessage!, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task LoadAsync_without_selected_claim_remains_initial_without_reading_sources()
    {
        var fixture = new Fixture();
        var viewModel = fixture.CreateViewModel();
        viewModel.SelectedClaimCaseId = null;

        var loaded = await viewModel.LoadAsync();

        Assert.False(loaded);
        Assert.Equal(ClaimReferenceResultState.Initial, viewModel.State);
        Assert.Equal("initial", viewModel.StateMessage);
        Assert.Equal(0, fixture.Families.ReadCount);
        Assert.Equal(0, fixture.History.ReadCount);
    }

    private static ClaimReferenceCoverageResult Coverage(string policyId, string coverageId, string group)
    {
        return new ClaimReferenceCoverageResult(
            policyId,
            coverageId,
            policyId == "policy-a" ? "Policy A" : "Policy B",
            coverageId == "coverage-a" ? "Coverage A" : "Coverage B",
            group,
            2,
            [new ClaimReferenceRuleEvidence(ClaimReferenceMatchingValues.RuleTreatmentDate, ClaimReferenceMatchingValues.OutcomePassed)],
            false);
    }

    private static ClaimReferenceSimilarClaim SimilarClaim(string policyId)
    {
        return new ClaimReferenceSimilarClaim(
            "claim-history",
            "submission-history",
            policyId,
            "History Policy",
            "active",
            true,
            ClaimReferenceMatchingValues.SimilarityTierA,
            new DateOnly(2026, 8, 1),
            ClaimCaseValues.VisitTypeOutpatient,
            Timestamp,
            [new ClaimReferencePaymentFact(ClaimPaymentValues.StatusPaid, null, null, null)]);
    }

    private sealed class Fixture
    {
        public StubEngine Engine { get; } = new();
        public StubFamilyStorage Families { get; } = new();
        public StubHistoryReader History { get; } = new();
        public StubCoverageStorage Coverages { get; } = new();
        public StubSubmissionReader Submissions { get; } = new();
        public StubPaymentReader Payments { get; } = new();
        public StubDocumentStorage Documents { get; } = new();

        public Fixture()
        {
            Families.Records =
            [
                new FamilyMemberRecord("family-current", "Family", "self", null, Timestamp, Timestamp, null, 1)
            ];
            History.Claims =
            [
                new ClaimRecord("claim-current", null, "current claim", new DateOnly(2026, 8, 10), Timestamp, Timestamp, null, "family-current", CaseStatus: ClaimCaseValues.StatusSaved)
            ];
            History.Policies =
            [
                new PolicyRecord("policy-a", "Policy A", null, Timestamp, Timestamp, null, "family-current")
            ];
            Coverages.Records =
            [
                new PolicyCoverageRecord("coverage-a", "policy-a", "Coverage A", PolicyCoverageValues.ReviewStatusUserConfirmed, null, null, PolicyCoverageValues.VisitTypeAny, PolicyCoverageValues.ConditionAny, PolicyCoverageValues.ConditionAny, PolicyCoverageValues.DiagnosisRuleAny, [], PolicyCoverageValues.SourceManual, null, null, null, 1, Timestamp, Timestamp, null)
            ];
        }

        public ClaimReferenceResultViewModel CreateViewModel()
        {
            return new ClaimReferenceResultViewModel(
                Engine,
                Families,
                History,
                Coverages,
                Submissions,
                Payments,
                Documents,
                new StubUiTextProvider())
            {
                SelectedClaimCaseId = "claim-current"
            };
        }
    }

    private sealed class StubEngine : IClaimReferenceMatchingEngine
    {
        public ClaimReferenceProjection Projection { get; set; } = new([], [], false);
        public Exception? Failure { get; set; }
        public ClaimReferenceMatchingRequest? LastRequest { get; private set; }

        public ClaimReferenceProjection BuildProjection(ClaimReferenceMatchingRequest request)
        {
            LastRequest = request;
            if (Failure is not null)
            {
                throw Failure;
            }

            return Projection;
        }
    }

    private sealed class StubFamilyStorage : IFamilyMemberStorageService
    {
        public IReadOnlyList<FamilyMemberRecord> Records { get; set; } = [];
        public int ReadCount { get; private set; }
        public Task<IReadOnlyList<FamilyMemberRecord>> GetFamilyMembersAsync(CancellationToken cancellationToken = default) { ReadCount++; return Task.FromResult(Records); }
        public Task<IReadOnlyList<FamilyMemberRecord>> GetActiveFamilyMembersAsync(CancellationToken cancellationToken = default) => Task.FromResult(Records);
        public Task<FamilyMemberRecord?> GetFamilyMemberAsync(string id, CancellationToken cancellationToken = default) => Task.FromResult(Records.FirstOrDefault(record => record.Id == id));
        public Task<FamilyMemberRecord> CreateFamilyMemberAsync(FamilyMemberDraft draft, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<FamilyMemberRecord> UpdateFamilyMemberAsync(string id, int expectedVersion, FamilyMemberDraft draft, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<FamilyMemberRecord> DeactivateFamilyMemberAsync(string id, int expectedVersion, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<FamilyMemberRecord> ReactivateFamilyMemberAsync(string id, int expectedVersion, CancellationToken cancellationToken = default) => throw new NotSupportedException();
    }

    private sealed class StubHistoryReader : IClaimHistoryStorageReader
    {
        public IReadOnlyList<PolicyRecord> Policies { get; set; } = [];
        public IReadOnlyList<ClaimRecord> Claims { get; set; } = [];
        public int ReadCount { get; private set; }
        public Task<IReadOnlyList<PolicyRecord>> GetAllPoliciesForHistoryAsync(CancellationToken cancellationToken = default) { ReadCount++; return Task.FromResult(Policies); }
        public Task<IReadOnlyList<ClaimRecord>> GetAllClaimCasesForHistoryAsync(CancellationToken cancellationToken = default) { ReadCount++; return Task.FromResult(Claims); }
    }

    private sealed class StubCoverageStorage : IPolicyCoverageStorageService
    {
        public IReadOnlyList<PolicyCoverageRecord> Records { get; set; } = [];
        public Task<IReadOnlyList<PolicyCoverageRecord>> GetPolicyCoveragesAsync(CancellationToken cancellationToken = default) => Task.FromResult(Records);
        public Task<IReadOnlyList<PolicyCoverageRecord>> GetActivePolicyCoveragesAsync(CancellationToken cancellationToken = default) => Task.FromResult(Records);
        public Task<PolicyCoverageRecord?> GetPolicyCoverageAsync(string id, CancellationToken cancellationToken = default) => Task.FromResult(Records.FirstOrDefault(record => record.PolicyCoverageId == id));
        public Task<PolicyCoverageRecord> CreatePolicyCoverageAsync(PolicyCoverageCreateDraft draft, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<PolicyCoverageRecord> UpdatePolicyCoverageAsync(string id, int expectedRevision, PolicyCoverageUpdateDraft draft, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<PolicyCoverageRecord> ChangePolicyCoverageReviewStatusAsync(string id, int expectedRevision, string targetReviewStatus, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<PolicyCoverageRecord> DisablePolicyCoverageAsync(string id, int expectedRevision, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<PolicyCoverageRecord> RestorePolicyCoverageAsync(string id, int expectedRevision, CancellationToken cancellationToken = default) => throw new NotSupportedException();
    }

    private sealed class StubSubmissionReader : IClaimSubmissionHistoryStorageReader
    {
        public IReadOnlyList<ClaimSubmissionRecord> Records { get; set; } = [];
        public Task<IReadOnlyList<ClaimSubmissionRecord>> GetAllSubmissionsForHistoryAsync(CancellationToken cancellationToken = default) => Task.FromResult(Records);
    }

    private sealed class StubPaymentReader : IClaimPaymentHistoryStorageReader
    {
        public IReadOnlyList<ClaimPaymentRecord> Records { get; set; } = [];
        public Task<IReadOnlyList<ClaimPaymentRecord>> GetAllPaymentsForHistoryAsync(CancellationToken cancellationToken = default) => Task.FromResult(Records);
    }

    private sealed class StubDocumentStorage : IDocumentStorageService
    {
        public Task<IReadOnlyList<DocumentRecord>> GetDocumentsAsync(CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<DocumentRecord>>([]);
        public Task<DocumentRecord?> GetDocumentByIdAsync(string documentId, CancellationToken cancellationToken = default) => Task.FromResult<DocumentRecord?>(null);
        public Task<DocumentRecord> AddDocumentAsync(DocumentDraft draft, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task DisableDocumentAsync(string documentId, DateTimeOffset disabledAt, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<IReadOnlyList<PolicyDocumentRecord>> GetPolicyDocumentsAsync(string policyId, CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<PolicyDocumentRecord>>([]);
        public Task<PolicyDocumentRecord> AddPolicyDocumentAsync(PolicyDocumentDraft draft, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task DisablePolicyDocumentAsync(string policyDocumentId, DateTimeOffset disabledAt, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<IReadOnlyList<ClaimDocumentRecord>> GetClaimDocumentsAsync(string claimId, CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<ClaimDocumentRecord>>([]);
        public Task<ClaimDocumentRecord> AddClaimDocumentAsync(ClaimDocumentDraft draft, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task DisableClaimDocumentAsync(string claimDocumentId, DateTimeOffset disabledAt, CancellationToken cancellationToken = default) => throw new NotSupportedException();
    }

    private sealed class StubUiTextProvider : IUiTextProvider
    {
        private static readonly IReadOnlyDictionary<string, string> Values = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            [UiTextKeys.ProductClaimReferenceInitialMessage] = "initial",
            [UiTextKeys.ProductClaimReferenceEmptyMessage] = "empty",
            [UiTextKeys.ProductClaimReferenceDomainErrorMessage] = "domain",
            [UiTextKeys.ProductClaimReferenceUnexpectedErrorMessage] = "unexpected",
            [UiTextKeys.ProductClaimReferenceSourceDocumentAvailable] = "document",
            [UiTextKeys.ProductClaimReferenceSourceDocumentUnavailable] = "no document",
            [UiTextKeys.ProductClaimReferenceResultGroupMatch] = "match",
            [UiTextKeys.ProductClaimReferenceResultGroupNeedsConfirmation] = "needs",
            [UiTextKeys.ProductClaimReferenceResultGroupMismatch] = "mismatch",
            [UiTextKeys.ProductClaimReferenceResultGroupUnknown] = "unknown",
            [UiTextKeys.ProductClaimReferenceRuleTreatmentDate] = "treatment",
            [UiTextKeys.ProductClaimReferenceOutcomePassed] = "passed",
            [UiTextKeys.ProductClaimReferenceNoPaymentValue] = "no payment",
            [UiTextKeys.ProductClaimReferenceSimilarityFormat] = "Tier {0}",
            [UiTextKeys.ProductClaimPaymentStatusPaid] = "paid"
        };

        public string Get(string key) => Values.GetValueOrDefault(key, key);
        public string Format(string key, params object?[] args) => string.Format(Get(key), args);
    }
}
