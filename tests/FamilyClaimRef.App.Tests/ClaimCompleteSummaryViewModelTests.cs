using System.Xml.Linq;
using FamilyClaimRef.App.Models.Storage;
using FamilyClaimRef.App.Services.Localization;
using FamilyClaimRef.App.Services.Storage;
using FamilyClaimRef.App.ViewModels;
using Xunit;

namespace FamilyClaimRef.App.Tests;

public sealed class ClaimCompleteSummaryViewModelTests
{
    private static readonly DateTimeOffset Timestamp =
        new(2026, 8, 10, 10, 0, 0, TimeSpan.Zero);

    [Fact]
    public void Constructor_rejects_null_dependencies()
    {
        var fixture = new StubFixture();

        Assert.Throws<ArgumentNullException>(() => new ClaimCompleteSummaryViewModel(
            null!, fixture.Submissions, fixture.Payments, fixture.Policies, fixture.Families, fixture.UiText));
        Assert.Throws<ArgumentNullException>(() => new ClaimCompleteSummaryViewModel(
            fixture.Claims, null!, fixture.Payments, fixture.Policies, fixture.Families, fixture.UiText));
        Assert.Throws<ArgumentNullException>(() => new ClaimCompleteSummaryViewModel(
            fixture.Claims, fixture.Submissions, null!, fixture.Policies, fixture.Families, fixture.UiText));
        Assert.Throws<ArgumentNullException>(() => new ClaimCompleteSummaryViewModel(
            fixture.Claims, fixture.Submissions, fixture.Payments, null!, fixture.Families, fixture.UiText));
        Assert.Throws<ArgumentNullException>(() => new ClaimCompleteSummaryViewModel(
            fixture.Claims, fixture.Submissions, fixture.Payments, fixture.Policies, null!, fixture.UiText));
        Assert.Throws<ArgumentNullException>(() => new ClaimCompleteSummaryViewModel(
            fixture.Claims, fixture.Submissions, fixture.Payments, fixture.Policies, fixture.Families, null!));
    }

    [Fact]
    public async Task Load_aggregates_submissions_and_payments_by_exact_status()
    {
        var fixture = new StubFixture();
        fixture.AddSubmission("submission_preparing", "policy_1", ClaimSubmissionValues.StatusPreparing);
        fixture.AddSubmission("submission_reviewing", "policy_2", ClaimSubmissionValues.StatusReviewing);
        fixture.AddSubmission("submission_completed", "policy_3", ClaimSubmissionValues.StatusCompleted);
        fixture.AddSubmission("submission_cancelled", "policy_4", ClaimSubmissionValues.StatusCancelled);
        fixture.Payments.BySubmission["submission_reviewing"] =
        [
            Payment("payment_pending", "submission_reviewing", ClaimPaymentValues.StatusPending),
            Payment("payment_partial", "submission_reviewing", ClaimPaymentValues.StatusPartiallyPaid)
        ];
        fixture.Payments.BySubmission["submission_completed"] =
        [
            Payment("payment_paid", "submission_completed", ClaimPaymentValues.StatusPaid),
            Payment("payment_denied", "submission_completed", ClaimPaymentValues.StatusDenied)
        ];
        fixture.Payments.BySubmission["submission_cancelled"] =
        [
            Payment("payment_cancelled", "submission_cancelled", ClaimPaymentValues.StatusCancelled)
        ];
        var viewModel = fixture.CreateViewModel();

        Assert.True(await viewModel.LoadAsync());

        Assert.True(viewModel.HasSummary);
        Assert.True(viewModel.HasSubmissions);
        Assert.Equal("synthetic claim", viewModel.ClaimDisplayTitle);
        Assert.Equal("synthetic family", viewModel.FamilyDisplayName);
        Assert.Equal(4, viewModel.SubmissionTotalCount);
        Assert.Equal(2, viewModel.SubmissionInProgressCount);
        Assert.Equal(1, viewModel.SubmissionCompletedCount);
        Assert.Equal(1, viewModel.SubmissionCancelledCount);
        Assert.Equal(1, viewModel.PaymentPendingCount);
        Assert.Equal(1, viewModel.PaymentPaidCount);
        Assert.Equal(1, viewModel.PaymentPartiallyPaidCount);
        Assert.Equal(1, viewModel.PaymentDeniedCount);
        Assert.Equal(1, viewModel.PaymentCancelledCount);
        Assert.Equal(4, viewModel.Submissions.Count);
        Assert.Contains(
            viewModel.Submissions,
            item => item.PolicyDisplayTitle == "policy 1"
                && item.PaymentSummaryDisplay == "no payments");
        Assert.Contains(
            viewModel.Submissions,
            item => item.PolicyDisplayTitle == "policy 2"
                && item.PaymentSummaryDisplay == "pending 1 paid 0 partial 1 denied 0 cancelled 0");
    }

    [Fact]
    public async Task Saved_claim_without_submissions_is_a_valid_empty_summary()
    {
        var fixture = new StubFixture();
        var viewModel = fixture.CreateViewModel();

        Assert.True(await viewModel.LoadAsync());

        Assert.True(viewModel.HasSummary);
        Assert.False(viewModel.HasSubmissions);
        Assert.Empty(viewModel.Submissions);
        Assert.Equal(0, viewModel.SubmissionTotalCount);
        Assert.Null(viewModel.StateMessage);
    }

    [Fact]
    public async Task Missing_or_disabled_claim_is_safe_reference_failure()
    {
        var missing = new StubFixture();
        missing.Claims.Claim = null;
        var missingViewModel = missing.CreateViewModel();

        Assert.False(await missingViewModel.LoadAsync());
        Assert.Equal("reference", missingViewModel.StateMessage);
        Assert.False(missingViewModel.HasSummary);

        var disabled = new StubFixture();
        disabled.Claims.Claim = disabled.Claims.Claim! with { DisabledAt = Timestamp };
        var disabledViewModel = disabled.CreateViewModel();

        Assert.False(await disabledViewModel.LoadAsync());
        Assert.Equal("reference", disabledViewModel.StateMessage);
        Assert.False(disabledViewModel.HasSummary);
    }

    [Fact]
    public async Task Legacy_owner_is_reported_separately_without_projection()
    {
        var fixture = new StubFixture();
        fixture.Claims.Claim = fixture.Claims.Claim! with { FamilyMemberId = null };
        var viewModel = fixture.CreateViewModel();

        Assert.False(await viewModel.LoadAsync());

        Assert.Equal("legacy", viewModel.StateMessage);
        Assert.False(viewModel.HasSummary);
        Assert.Empty(viewModel.Submissions);
    }

    [Fact]
    public async Task Related_policy_reference_states_are_reported_without_projection()
    {
        var missing = new StubFixture();
        missing.AddSubmission("submission_missing_policy", "policy_missing", ClaimSubmissionValues.StatusSubmitted);
        missing.Policies.Records.Clear();
        var missingViewModel = missing.CreateViewModel();

        Assert.False(await missingViewModel.LoadAsync());
        Assert.Equal("reference", missingViewModel.StateMessage);
        Assert.False(missingViewModel.HasSummary);

        var disabled = new StubFixture();
        disabled.AddSubmission("submission_disabled_policy", "policy_disabled", ClaimSubmissionValues.StatusSubmitted);
        disabled.Policies.Records["policy_disabled"] =
            disabled.Policies.Records["policy_disabled"] with { DisabledAt = Timestamp };
        var disabledViewModel = disabled.CreateViewModel();

        Assert.False(await disabledViewModel.LoadAsync());
        Assert.Equal("reference", disabledViewModel.StateMessage);
        Assert.False(disabledViewModel.HasSummary);

        var legacy = new StubFixture();
        legacy.AddSubmission("submission_legacy_policy", "policy_legacy", ClaimSubmissionValues.StatusSubmitted);
        legacy.Policies.Records["policy_legacy"] =
            legacy.Policies.Records["policy_legacy"] with { FamilyMemberId = null };
        var legacyViewModel = legacy.CreateViewModel();

        Assert.False(await legacyViewModel.LoadAsync());
        Assert.Equal("legacy", legacyViewModel.StateMessage);
        Assert.False(legacyViewModel.HasSummary);
    }

    [Fact]
    public async Task Reference_mismatch_in_submission_or_payment_is_rejected()
    {
        var submissionMismatch = new StubFixture();
        submissionMismatch.AddSubmission(
            "submission_wrong_claim",
            "policy_1",
            ClaimSubmissionValues.StatusSubmitted,
            "another_claim");
        var submissionViewModel = submissionMismatch.CreateViewModel();

        Assert.False(await submissionViewModel.LoadAsync());
        Assert.Equal("reference", submissionViewModel.StateMessage);

        var paymentMismatch = new StubFixture();
        paymentMismatch.AddSubmission("submission_1", "policy_1", ClaimSubmissionValues.StatusCompleted);
        paymentMismatch.Payments.BySubmission["submission_1"] =
        [
            Payment("payment_wrong_submission", "another_submission", ClaimPaymentValues.StatusPaid)
        ];
        var paymentViewModel = paymentMismatch.CreateViewModel();

        Assert.False(await paymentViewModel.LoadAsync());
        Assert.Equal("reference", paymentViewModel.StateMessage);
    }

    [Fact]
    public async Task Load_failure_clears_previous_projection_and_hides_diagnostic_details()
    {
        var fixture = new StubFixture();
        fixture.AddSubmission("submission_1", "policy_1", ClaimSubmissionValues.StatusSubmitted);
        var viewModel = fixture.CreateViewModel();
        Assert.True(await viewModel.LoadAsync());
        fixture.Submissions.LoadException = new IOException(
            @"C:\Users\local-user\claims.json contained private diagnostics");

        Assert.False(await viewModel.LoadAsync());

        Assert.False(viewModel.HasSummary);
        Assert.Empty(viewModel.Submissions);
        Assert.Equal(0, viewModel.SubmissionTotalCount);
        Assert.Equal("failed", viewModel.StateMessage);
        Assert.DoesNotContain("local-user", viewModel.StateMessage, StringComparison.Ordinal);
        Assert.DoesNotContain("claims.json", viewModel.StateMessage, StringComparison.Ordinal);
        Assert.DoesNotContain("private diagnostics", viewModel.StateMessage, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Real_json_projection_is_byte_for_byte_read_only()
    {
        var root = Path.Combine(
            Path.GetTempPath(),
            "FamilyClaimRef.App.Tests",
            nameof(ClaimCompleteSummaryViewModelTests),
            Guid.NewGuid().ToString("N"));
        var metadataRoot = Path.Combine(root, "data", "local");

        try
        {
            var families = new JsonFamilyMemberStorageService(metadataRoot);
            var policies = new JsonPolicyClaimStorageService(metadataRoot, families);
            var documents = new JsonDocumentStorageService(metadataRoot);
            var submissions = new JsonClaimSubmissionStorageService(
                metadataRoot,
                policies,
                policies,
                documents);
            var payments = new JsonClaimPaymentStorageService(
                metadataRoot,
                submissions,
                policies,
                policies);
            var family = await families.CreateFamilyMemberAsync(
                new FamilyMemberDraft("synthetic family", FamilyMemberRelationValues.Self, null));
            var policy = await policies.CreateInsurancePolicyAsync(PolicyDraft(family.Id));
            var claimDraft = ClaimDraft(family.Id);
            var draftClaim = await policies.CreateClaimCaseAsync(claimDraft);
            var claim = await policies.UpdateClaimCaseAsync(
                draftClaim.Id,
                draftClaim.Revision,
                claimDraft);
            await submissions.CreateAsync(new ClaimSubmissionDraft(
                claim.Id,
                policy.Id,
                null,
                null,
                null,
                null,
                [],
                ClaimSubmissionValues.StatusPreparing,
                null));
            var before = SnapshotFiles(metadataRoot);
            var viewModel = new ClaimCompleteSummaryViewModel(
                policies,
                submissions,
                payments,
                policies,
                families,
                new FakeUiTextProvider())
            {
                SelectedClaimCaseId = claim.Id
            };

            Assert.True(await viewModel.LoadAsync());

            var after = SnapshotFiles(metadataRoot);
            Assert.Equal(before.Keys.Order(StringComparer.Ordinal), after.Keys.Order(StringComparer.Ordinal));
            foreach (var path in before.Keys)
            {
                Assert.Equal(before[path], after[path]);
            }
        }
        finally
        {
            if (Directory.Exists(root))
            {
                Directory.Delete(root, true);
            }
        }
    }

    [Fact]
    public void Dedicated_view_is_read_only_and_does_not_expose_data_ids()
    {
        var root = FindProjectRoot();
        var view = XDocument.Load(Path.Combine(
            root,
            "app",
            "FamilyClaimRef.App",
            "Views",
            "ProductClaimCompleteSummaryView.xaml"));
        var values = view
            .Descendants()
            .SelectMany(element => element.Attributes())
            .Select(attribute => attribute.Value)
            .ToArray();

        Assert.Contains("ProductScreen_14", values);
        Assert.Contains("ProductClaimComplete_SubmissionList", values);
        Assert.Contains("{x:Static viewModels:ProductScreenRoutes.ClaimSubmission}", values);
        Assert.Contains("{x:Static viewModels:ProductScreenRoutes.HistoryView}", values);
        Assert.DoesNotContain(values, value => value.Contains(".Id}", StringComparison.Ordinal));
        Assert.DoesNotContain(values, value => value.Contains("SelectedClaimCaseId", StringComparison.Ordinal));
        var mutationCommands = new[]
        {
            "SaveCommand",
            "CreateCommand",
            "UpdateCommand",
            "DisableCommand"
        };
        Assert.DoesNotContain(
            values,
            value => mutationCommands.Any(command => value.Contains(command, StringComparison.Ordinal)));
        var dataGrid = Assert.Single(
            view.Descendants(),
            element => element.Name.LocalName == "DataGrid");
        Assert.Equal("True", dataGrid.Attribute("IsReadOnly")?.Value);
        Assert.Equal("False", dataGrid.Attribute("CanUserAddRows")?.Value);
        Assert.Equal("False", dataGrid.Attribute("CanUserDeleteRows")?.Value);

        var shell = XDocument.Load(Path.Combine(
            root,
            "app",
            "FamilyClaimRef.App",
            "ProductShell",
            "ProductShellWindow.xaml"));
        Assert.Contains(
            shell.Descendants(),
            element => element.Attribute("Value")?.Value == ProductScreenRoutes.ClaimComplete);
        Assert.Contains(
            shell.Descendants(),
            element => element.Attribute("Value")?.Value == "{StaticResource ClaimCompleteContentTemplate}");
    }

    private static IReadOnlyDictionary<string, byte[]> SnapshotFiles(string root)
    {
        return Directory
            .EnumerateFiles(root, "*", SearchOption.AllDirectories)
            .ToDictionary(
                path => Path.GetRelativePath(root, path),
                File.ReadAllBytes,
                StringComparer.Ordinal);
    }

    private static ClaimPaymentRecord Payment(
        string id,
        string submissionId,
        string status)
    {
        return new ClaimPaymentRecord(
            id,
            submissionId,
            status,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            1,
            Timestamp,
            Timestamp);
    }

    private static InsurancePolicyDraft PolicyDraft(string familyId)
    {
        return new InsurancePolicyDraft(
            "synthetic policy",
            familyId,
            "synthetic insurer",
            InsurancePolicyValues.ContractStatusActive,
            new DateOnly(2026, 1, 1),
            "2026-2036",
            "10 years",
            1_000_000,
            InsurancePolicyValues.RenewalTypeFixed,
            InsurancePolicyValues.RefundTypeRefundable,
            InsurancePolicyValues.BusinessTypeLife,
            InsurancePolicyValues.ProductCategoryMedicalExpense);
    }

    private static ClaimCaseDraft ClaimDraft(string familyId)
    {
        return new ClaimCaseDraft(
            "synthetic claim",
            familyId,
            new DateOnly(2026, 8, 10),
            "synthetic hospital",
            "A01",
            "synthetic diagnosis",
            ClaimCaseValues.VisitTypeOutpatient,
            false,
            false,
            10_000,
            0,
            0,
            null);
    }

    private static string FindProjectRoot()
    {
        var current = new DirectoryInfo(AppContext.BaseDirectory);
        while (current is not null)
        {
            if (File.Exists(Path.Combine(current.FullName, "FamilyClaimRef.sln")))
            {
                return current.FullName;
            }

            current = current.Parent;
        }

        throw new DirectoryNotFoundException("FamilyClaimRef project root was not found.");
    }

    private sealed class StubFixture
    {
        public StubFixture()
        {
            Families.Records["family_1"] = new FamilyMemberRecord(
                "family_1",
                "synthetic family",
                FamilyMemberRelationValues.Self,
                null,
                Timestamp,
                Timestamp,
                null,
                1);
            Claims.Claim = new ClaimRecord(
                "claim_1",
                null,
                "synthetic claim",
                new DateOnly(2026, 8, 10),
                Timestamp,
                Timestamp,
                null,
                "family_1",
                "synthetic hospital",
                "A01",
                "synthetic diagnosis",
                ClaimCaseValues.VisitTypeOutpatient,
                false,
                false,
                10_000,
                0,
                0,
                null,
                ClaimCaseValues.StatusSaved,
                2);
        }

        public StubClaimCaseStorage Claims { get; } = new();

        public StubClaimSubmissionStorage Submissions { get; } = new();

        public StubClaimPaymentStorage Payments { get; } = new();

        public StubPolicyStorage Policies { get; } = new();

        public StubFamilyStorage Families { get; } = new();

        public IUiTextProvider UiText { get; } = new FakeUiTextProvider();

        public ClaimCompleteSummaryViewModel CreateViewModel()
        {
            return new ClaimCompleteSummaryViewModel(
                Claims,
                Submissions,
                Payments,
                Policies,
                Families,
                UiText)
            {
                SelectedClaimCaseId = "claim_1"
            };
        }

        public void AddSubmission(
            string id,
            string policyId,
            string status,
            string claimCaseId = "claim_1")
        {
            Policies.Records[policyId] = new PolicyRecord(
                policyId,
                policyId.Replace('_', ' '),
                null,
                Timestamp,
                Timestamp,
                null,
                "family_1");
            Submissions.Records.Add(new ClaimSubmissionRecord(
                id,
                claimCaseId,
                policyId,
                null,
                null,
                null,
                null,
                [],
                status,
                null,
                1,
                Timestamp,
                Timestamp.AddMinutes(Submissions.Records.Count)));
        }
    }

    private sealed class FakeUiTextProvider : IUiTextProvider
    {
        private static readonly IReadOnlyDictionary<string, string> Values =
            new Dictionary<string, string>(StringComparer.Ordinal)
            {
                [UiTextKeys.ProductClaimCompleteEmptyMessage] = "empty",
                [UiTextKeys.ProductClaimCompleteReferenceMessage] = "reference",
                [UiTextKeys.ProductClaimCompleteLegacyReviewMessage] = "legacy",
                [UiTextKeys.ProductClaimCompleteLoadFailedMessage] = "failed",
                [UiTextKeys.ProductClaimCompleteNoPaymentsValue] = "no payments",
                [UiTextKeys.ProductClaimCompleteNotEnteredValue] = "not entered",
                [UiTextKeys.ProductClaimCompletePaymentSummaryFormat] =
                    "pending {0} paid {1} partial {2} denied {3} cancelled {4}",
                [UiTextKeys.ProductClaimCompleteCaseStatusSaved] = "saved",
                [UiTextKeys.ProductClaimSubmissionStatusPreparing] = "preparing",
                [UiTextKeys.ProductClaimSubmissionStatusSubmitted] = "submitted",
                [UiTextKeys.ProductClaimSubmissionStatusAdditionalDocumentsRequested] = "additional",
                [UiTextKeys.ProductClaimSubmissionStatusReviewing] = "reviewing",
                [UiTextKeys.ProductClaimSubmissionStatusCancelled] = "cancelled",
                [UiTextKeys.ProductClaimSubmissionStatusCompleted] = "completed",
                [UiTextKeys.ProductClaimCaseVisitTypeOutpatient] = "outpatient",
                [UiTextKeys.ProductClaimCaseVisitTypeInpatient] = "inpatient"
            };

        public string Get(string key)
        {
            return Values.GetValueOrDefault(key, key);
        }

        public string Format(string key, params object?[] args)
        {
            return string.Format(Get(key), args);
        }
    }

    private sealed class StubClaimCaseStorage : IClaimCaseStorageService
    {
        public ClaimRecord? Claim { get; set; }

        public Task<IReadOnlyList<ClaimRecord>> GetClaimCasesAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IReadOnlyList<ClaimRecord>>(Claim is null ? [] : [Claim]);
        }

        public Task<ClaimRecord?> GetClaimCaseAsync(string id, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Claim);
        }

        public Task<ClaimRecord> CreateClaimCaseAsync(ClaimCaseDraft draft, CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<ClaimRecord> UpdateClaimCaseAsync(string id, int expectedRevision, ClaimCaseDraft draft, CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<ClaimRecord> DisableClaimCaseAsync(string id, int expectedRevision, CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();
    }

    private sealed class StubClaimSubmissionStorage : IClaimSubmissionStorageService
    {
        public List<ClaimSubmissionRecord> Records { get; } = [];

        public Exception? LoadException { get; set; }

        public Task<IReadOnlyList<ClaimSubmissionRecord>> GetByClaimCaseAsync(string claimCaseId, CancellationToken cancellationToken = default)
        {
            if (LoadException is not null)
            {
                throw LoadException;
            }

            return Task.FromResult<IReadOnlyList<ClaimSubmissionRecord>>(Records.ToArray());
        }

        public Task<ClaimSubmissionRecord?> GetAsync(string id, CancellationToken cancellationToken = default) =>
            Task.FromResult(Records.FirstOrDefault(record => string.Equals(record.Id, id, StringComparison.Ordinal)));

        public Task<IReadOnlyList<PolicyRecord>> GetClaimablePoliciesAsync(string claimCaseId, CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<PolicyRecord>>([]);

        public Task<ClaimSubmissionRecord> CreateAsync(ClaimSubmissionDraft draft, CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<ClaimSubmissionRecord> UpdateAsync(string id, int expectedRevision, ClaimSubmissionDraft draft, CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();
    }

    private sealed class StubClaimPaymentStorage : IClaimPaymentStorageService
    {
        public Dictionary<string, IReadOnlyList<ClaimPaymentRecord>> BySubmission { get; } =
            new(StringComparer.Ordinal);

        public Task<IReadOnlyList<ClaimPaymentRecord>> GetBySubmissionAsync(string claimSubmissionId, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(BySubmission.GetValueOrDefault(claimSubmissionId, []));
        }

        public Task<ClaimPaymentRecord?> GetAsync(string id, CancellationToken cancellationToken = default) =>
            Task.FromResult(BySubmission.Values.SelectMany(records => records).FirstOrDefault(record => record.Id == id));

        public Task<ClaimPaymentRecord> CreateAsync(ClaimPaymentDraft draft, CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<ClaimPaymentRecord> UpdateAsync(string id, int expectedRevision, ClaimPaymentDraft draft, CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();
    }

    private sealed class StubPolicyStorage : IPolicyClaimStorageService
    {
        public Dictionary<string, PolicyRecord> Records { get; } = new(StringComparer.Ordinal);

        public Task<IReadOnlyList<PolicyRecord>> GetPoliciesAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<PolicyRecord>>(Records.Values.ToArray());

        public Task<PolicyRecord?> GetPolicyAsync(string id, CancellationToken cancellationToken = default) =>
            Task.FromResult(Records.GetValueOrDefault(id));

        public Task<PolicyRecord> AddPolicyAsync(PolicyDraft draft, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<PolicyRecord> CreateInsurancePolicyAsync(InsurancePolicyDraft draft, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<PolicyRecord> UpdateInsurancePolicyAsync(string id, InsurancePolicyDraft draft, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<PolicyRecord> DisablePolicyAsync(string id, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<IReadOnlyList<ClaimRecord>> GetClaimsAsync(CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<ClaimRecord>>([]);
        public Task<IReadOnlyList<ClaimRecord>> GetClaimsByPolicyIdAsync(string policyId, CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<ClaimRecord>>([]);
        public Task<ClaimRecord?> GetClaimAsync(string id, CancellationToken cancellationToken = default) => Task.FromResult<ClaimRecord?>(null);
        public Task<ClaimRecord> AddClaimAsync(ClaimDraft draft, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<ClaimRecord> DisableClaimAsync(string id, int expectedRevision, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<bool> PolicyExistsAsync(string id, CancellationToken cancellationToken = default) => Task.FromResult(Records.ContainsKey(id));
        public Task<bool> ClaimExistsAsync(string id, CancellationToken cancellationToken = default) => Task.FromResult(false);
    }

    private sealed class StubFamilyStorage : IFamilyMemberStorageService
    {
        public Dictionary<string, FamilyMemberRecord> Records { get; } = new(StringComparer.Ordinal);

        public Task<IReadOnlyList<FamilyMemberRecord>> GetFamilyMembersAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<FamilyMemberRecord>>(Records.Values.ToArray());

        public Task<IReadOnlyList<FamilyMemberRecord>> GetActiveFamilyMembersAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<FamilyMemberRecord>>(Records.Values.Where(record => record.DisabledAt is null).ToArray());

        public Task<FamilyMemberRecord?> GetFamilyMemberAsync(string id, CancellationToken cancellationToken = default) =>
            Task.FromResult(Records.GetValueOrDefault(id));

        public Task<FamilyMemberRecord> CreateFamilyMemberAsync(FamilyMemberDraft draft, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<FamilyMemberRecord> UpdateFamilyMemberAsync(string id, int expectedVersion, FamilyMemberDraft draft, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<FamilyMemberRecord> DeactivateFamilyMemberAsync(string id, int expectedVersion, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<FamilyMemberRecord> ReactivateFamilyMemberAsync(string id, int expectedVersion, CancellationToken cancellationToken = default) => throw new NotSupportedException();
    }
}
