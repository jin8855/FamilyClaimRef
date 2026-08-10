using System.Xml.Linq;
using FamilyClaimRef.App.Models.Storage;
using FamilyClaimRef.App.Services.Localization;
using FamilyClaimRef.App.Services.Storage;
using FamilyClaimRef.App.ViewModels;
using Xunit;

namespace FamilyClaimRef.App.Tests;

public sealed class ClaimPaymentManagementViewModelTests
{
    [Fact]
    public void Constructor_rejects_null_dependencies()
    {
        using var fixture = new TestFixture();

        Assert.Throws<ArgumentNullException>(() => new ClaimPaymentManagementViewModel(
            null!, fixture.UiText));
        Assert.Throws<ArgumentNullException>(() => new ClaimPaymentManagementViewModel(
            fixture.Payments, null!));
    }

    [Fact]
    public async Task Load_create_multiple_pending_and_exact_selection_are_preserved()
    {
        using var fixture = new TestFixture();
        var submission = await fixture.CreateSubmissionAsync(ClaimSubmissionValues.StatusSubmitted);
        var viewModel = fixture.CreateViewModel();

        Assert.True(await viewModel.LoadForSubmissionAsync(submission.Id));
        Assert.True(viewModel.CanCreate);
        Assert.True(await viewModel.CreatePendingAsync());
        var firstId = viewModel.SelectedPaymentId;
        viewModel.StartNew();
        Assert.True(await viewModel.CreatePendingAsync());
        var secondId = viewModel.SelectedPaymentId;

        Assert.NotEqual(firstId, secondId);
        Assert.Equal(2, viewModel.Payments.Count);
        Assert.True(viewModel.SelectPayment(firstId));
        Assert.Equal(firstId, viewModel.SelectedPaymentId);
        Assert.Equal(ClaimPaymentValues.StatusPending, viewModel.SelectedStatus);
    }

    [Fact]
    public async Task Completed_submission_accepts_paid_result_and_terminal_record_is_read_only()
    {
        using var fixture = new TestFixture();
        var submission = await fixture.CreateSubmissionAsync(ClaimSubmissionValues.StatusCompleted);
        var viewModel = fixture.CreateViewModel();
        await viewModel.LoadForSubmissionAsync(submission.Id);
        await viewModel.CreatePendingAsync();

        viewModel.SelectedStatus = ClaimPaymentValues.StatusPaid;
        viewModel.PaidDate = new DateTime(2026, 8, 10);
        viewModel.PaidAmountText = "1,250,000";
        viewModel.PaidCoverageDisplayName = "medical coverage";

        Assert.True(await viewModel.SaveAsync());
        Assert.False(viewModel.CanEditDetails);
        Assert.False(viewModel.CanSave);
        var stored = Assert.Single(await fixture.Payments.GetBySubmissionAsync(submission.Id));
        Assert.Equal(ClaimPaymentValues.StatusPaid, stored.Status);
        Assert.Equal(1_250_000, stored.PaidAmount);
        Assert.Equal(2, stored.Revision);
    }

    [Fact]
    public async Task Partially_paid_and_denied_state_fields_follow_exact_contract()
    {
        using var fixture = new TestFixture();
        var submission = await fixture.CreateSubmissionAsync(ClaimSubmissionValues.StatusCompleted);
        var partial = fixture.CreateViewModel();
        await partial.LoadForSubmissionAsync(submission.Id);
        await partial.CreatePendingAsync();
        partial.SelectedStatus = ClaimPaymentValues.StatusPartiallyPaid;

        Assert.True(partial.IsPaidResultEnabled);
        Assert.True(partial.IsReductionReasonEnabled);
        Assert.False(partial.IsDenyReasonEnabled);

        partial.PaidDate = new DateTime(2026, 8, 10);
        partial.PaidAmountText = "500000";
        partial.PaidCoverageDisplayName = "coverage A";
        partial.ReductionReason = "limit applied";
        Assert.True(await partial.SaveAsync());

        var denied = fixture.CreateViewModel();
        await denied.LoadForSubmissionAsync(submission.Id);
        denied.StartNew();
        await denied.CreatePendingAsync();
        denied.SelectedStatus = ClaimPaymentValues.StatusDenied;

        Assert.False(denied.IsPaidResultEnabled);
        Assert.True(denied.IsDenyReasonEnabled);
        Assert.False(denied.IsReductionReasonEnabled);
        denied.DenyReason = "not covered";
        Assert.True(await denied.SaveAsync());
        Assert.Equal(2, (await fixture.Payments.GetBySubmissionAsync(submission.Id)).Count);
    }

    [Fact]
    public async Task Invalid_amount_is_safe_validation_failure_and_does_not_write()
    {
        using var fixture = new TestFixture();
        var submission = await fixture.CreateSubmissionAsync(ClaimSubmissionValues.StatusCompleted);
        var viewModel = fixture.CreateViewModel();
        await viewModel.LoadForSubmissionAsync(submission.Id);
        await viewModel.CreatePendingAsync();
        var before = await File.ReadAllBytesAsync(fixture.PaymentPath);

        viewModel.SelectedStatus = ClaimPaymentValues.StatusPaid;
        viewModel.PaidDate = new DateTime(2026, 8, 10);
        viewModel.PaidAmountText = "not-a-number";
        viewModel.PaidCoverageDisplayName = "coverage";

        Assert.False(await viewModel.SaveAsync());
        Assert.Equal(UiTextKeys.ProductClaimPaymentValidationMessage, viewModel.ValidationMessage);
        Assert.Equal(before, await File.ReadAllBytesAsync(fixture.PaymentPath));
        Assert.DoesNotContain(fixture.RootPath, viewModel.ValidationMessage, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Stale_revision_sets_conflict_and_preserves_winning_value()
    {
        using var fixture = new TestFixture();
        var submission = await fixture.CreateSubmissionAsync(ClaimSubmissionValues.StatusSubmitted);
        var created = await fixture.Payments.CreateAsync(PendingDraft(submission.Id));
        var first = fixture.CreateViewModel();
        var second = fixture.CreateViewModel();
        await first.LoadForSubmissionAsync(submission.Id);
        await second.LoadForSubmissionAsync(submission.Id);
        Assert.True(first.SelectPayment(created.Id));
        Assert.True(second.SelectPayment(created.Id));

        first.Memo = "winner";
        second.Memo = "stale";
        Assert.True(await first.SaveAsync());
        Assert.False(await second.SaveAsync());

        Assert.Equal(UiTextKeys.ProductClaimPaymentConflictMessage, second.ConflictMessage);
        Assert.Equal("winner", (await fixture.Payments.GetAsync(created.Id))!.Memo);
    }

    [Fact]
    public async Task Unsaved_payment_blocks_submission_switch_and_parent_navigation()
    {
        using var fixture = new TestFixture();
        var firstSubmission = await fixture.CreateSubmissionAsync(
            ClaimSubmissionValues.StatusSubmitted,
            "first");
        var secondSubmission = await fixture.CreateSubmissionAsync(
            ClaimSubmissionValues.StatusSubmitted,
            "second");
        var payment = fixture.CreateViewModel();
        var parent = new ClaimSubmissionManagementViewModel(
            fixture.Submissions,
            fixture.PolicyClaims,
            fixture.Documents,
            payment,
            fixture.UiText);
        await payment.LoadForSubmissionAsync(firstSubmission.Id);
        await payment.CreatePendingAsync();
        payment.Memo = "unsaved";

        Assert.False(payment.CanNavigateAway);
        Assert.False(parent.CanNavigateAway);
        Assert.False(parent.CanSwitchSubmission);
        Assert.False(await payment.LoadForSubmissionAsync(secondSubmission.Id));
        Assert.Equal(firstSubmission.Id, payment.SelectedSubmissionId);
    }

    [Fact]
    public async Task Unsaved_payment_blocks_payment_switch_and_preserves_editor()
    {
        using var fixture = new TestFixture();
        var submission = await fixture.CreateSubmissionAsync(ClaimSubmissionValues.StatusSubmitted);
        var first = await fixture.Payments.CreateAsync(PendingDraft(submission.Id) with
        {
            Memo = "first"
        });
        var second = await fixture.Payments.CreateAsync(PendingDraft(submission.Id) with
        {
            Memo = "second"
        });
        var viewModel = fixture.CreateViewModel();
        Assert.True(await viewModel.LoadForSubmissionAsync(submission.Id));
        Assert.True(viewModel.SelectPayment(first.Id));
        viewModel.Memo = "unsaved editor";

        Assert.False(viewModel.SelectPayment(second.Id));

        Assert.Equal(first.Id, viewModel.SelectedPaymentId);
        Assert.Equal("unsaved editor", viewModel.Memo);
        Assert.True(viewModel.HasUnsavedChanges);
        Assert.False(viewModel.CanNavigateAway);
    }

    [Fact]
    public async Task Load_failure_clears_prior_payment_list_and_editor()
    {
        using var fixture = new TestFixture();
        var firstSubmission = await fixture.CreateSubmissionAsync(
            ClaimSubmissionValues.StatusSubmitted,
            "first");
        var secondSubmission = await fixture.CreateSubmissionAsync(
            ClaimSubmissionValues.StatusSubmitted,
            "second");
        var firstPayment = await fixture.Payments.CreateAsync(PendingDraft(firstSubmission.Id));
        var storage = new ToggleLoadClaimPaymentStorage(fixture.Payments);
        var viewModel = new ClaimPaymentManagementViewModel(storage, fixture.UiText);
        Assert.True(await viewModel.LoadForSubmissionAsync(firstSubmission.Id));
        Assert.True(viewModel.SelectPayment(firstPayment.Id));
        storage.FailLoads = true;

        Assert.False(await viewModel.LoadForSubmissionAsync(secondSubmission.Id));

        Assert.Equal(secondSubmission.Id, viewModel.SelectedSubmissionId);
        Assert.Empty(viewModel.Payments);
        Assert.Null(viewModel.SelectedPaymentId);
        Assert.False(viewModel.HasUnsavedChanges);
        Assert.Equal(
            UiTextKeys.ProductClaimPaymentOperationFailedMessage,
            viewModel.OperationMessage);
        Assert.DoesNotContain(
            fixture.RootPath,
            viewModel.OperationMessage,
            StringComparison.Ordinal);
    }

    [Fact]
    public async Task Parent_reference_failure_surfaces_safe_reference_message()
    {
        using var fixture = new TestFixture();
        var submission = await fixture.CreateSubmissionAsync(ClaimSubmissionValues.StatusSubmitted);
        var viewModel = fixture.CreateViewModel();
        await viewModel.LoadForSubmissionAsync(submission.Id);
        await viewModel.CreatePendingAsync();
        var current = await fixture.PolicyClaims.GetClaimCaseAsync(submission.ClaimCaseId);
        await fixture.PolicyClaims.DisableClaimCaseAsync(current!.Id, current.Revision);
        viewModel.Memo = "blocked";

        Assert.False(await viewModel.SaveAsync());
        Assert.Equal(UiTextKeys.ProductClaimPaymentReferenceMessage, viewModel.ReferenceMessage);
        Assert.DoesNotContain(submission.Id, viewModel.ReferenceMessage, StringComparison.Ordinal);
    }

    [Fact]
    public void Dedicated_view_uses_static_payment_locators_and_no_data_ids()
    {
        var root = FindProjectRoot();
        var path = Path.Combine(
            root,
            "app",
            "FamilyClaimRef.App",
            "Views",
            "ProductClaimSubmissionView.xaml");
        var document = XDocument.Load(path);
        var values = document.Descendants()
            .SelectMany(element => element.Attributes())
            .Select(attribute => attribute.Value)
            .ToArray();

        Assert.Contains("ProductClaimPayment_List", values);
        Assert.Contains("ProductClaimPayment_New", values);
        Assert.Contains("ProductClaimPayment_Create", values);
        Assert.Contains("ProductClaimPayment_Save", values);
        Assert.Contains("{Binding IsPaidResultEnabled}", values);
        Assert.Contains("{Binding IsDenyReasonEnabled}", values);
        Assert.Contains("{Binding IsReductionReasonEnabled}", values);
        Assert.Contains("{Binding CanNavigateAway}", values);
        Assert.DoesNotContain(values, value => value.Contains("ClaimSubmissionId", StringComparison.Ordinal));
        Assert.DoesNotContain(values, value => value.Contains("SelectedPaymentId", StringComparison.Ordinal));
        Assert.DoesNotContain(values, value => value.Contains("{Binding Id}", StringComparison.Ordinal));
    }

    private static ClaimPaymentDraft PendingDraft(string submissionId)
    {
        return new ClaimPaymentDraft(
            submissionId,
            ClaimPaymentValues.StatusPending,
            PaidDate: null,
            PaidAmount: null,
            PaidCoverageDisplayName: null,
            DenyReason: null,
            ReductionReason: null,
            AdditionalDocumentsMemo: null,
            Memo: null);
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

    private sealed class TestFixture : IDisposable
    {
        public TestFixture()
        {
            RootPath = Path.Combine(
                Path.GetTempPath(),
                "FamilyClaimRef.App.Tests",
                nameof(ClaimPaymentManagementViewModelTests),
                Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(RootPath);
            Families = new JsonFamilyMemberStorageService(RootPath);
            PolicyClaims = new JsonPolicyClaimStorageService(RootPath, Families);
            Documents = new JsonDocumentStorageService(RootPath);
            Submissions = new JsonClaimSubmissionStorageService(
                RootPath,
                PolicyClaims,
                PolicyClaims,
                Documents);
            Payments = new JsonClaimPaymentStorageService(
                RootPath,
                Submissions,
                PolicyClaims,
                PolicyClaims);
            UiText = new EchoUiTextProvider();
        }

        public string RootPath { get; }

        public string PaymentPath => Path.Combine(RootPath, "claim-payments.json");

        public JsonFamilyMemberStorageService Families { get; }

        public JsonPolicyClaimStorageService PolicyClaims { get; }

        public JsonDocumentStorageService Documents { get; }

        public JsonClaimSubmissionStorageService Submissions { get; }

        public JsonClaimPaymentStorageService Payments { get; }

        public IUiTextProvider UiText { get; }

        public ClaimPaymentManagementViewModel CreateViewModel()
        {
            return new ClaimPaymentManagementViewModel(Payments, UiText);
        }

        public async Task<ClaimSubmissionRecord> CreateSubmissionAsync(
            string status,
            string suffix = "primary")
        {
            var family = await Families.CreateFamilyMemberAsync(new FamilyMemberDraft(
                $"synthetic family {suffix}",
                FamilyMemberRelationValues.Self,
                null));
            var policy = await PolicyClaims.CreateInsurancePolicyAsync(new InsurancePolicyDraft(
                $"synthetic policy {suffix}",
                family.Id,
                "synthetic insurer",
                InsurancePolicyValues.ContractStatusActive,
                new DateOnly(2026, 8, 1),
                "synthetic coverage",
                "20 years",
                12_000_000m,
                InsurancePolicyValues.RenewalTypeFixed,
                InsurancePolicyValues.RefundTypeRefundable,
                InsurancePolicyValues.BusinessTypeLife,
                InsurancePolicyValues.ProductCategoryMedicalExpense));
            var claimDraft = new ClaimCaseDraft(
                $"synthetic claim {suffix}",
                family.Id,
                new DateOnly(2026, 8, 8),
                "synthetic hospital",
                "a12.3",
                "synthetic diagnosis",
                ClaimCaseValues.VisitTypeOutpatient,
                HasSurgery: false,
                HasPrescription: true,
                CoveredAmount: 1_000,
                NonCoveredAmount: 2_000,
                PrescriptionAmount: 3_000,
                Memo: null);
            var draftClaim = await PolicyClaims.CreateClaimCaseAsync(claimDraft);
            var claim = await PolicyClaims.UpdateClaimCaseAsync(
                draftClaim.Id,
                draftClaim.Revision,
                claimDraft);
            var submission = await Submissions.CreateAsync(new ClaimSubmissionDraft(
                claim.Id,
                policy.Id,
                PolicyCoverageId: null,
                CoverageDisplayName: null,
                SubmittedDate: null,
                SubmittedAmount: null,
                SubmittedClaimDocumentIds: [],
                ClaimSubmissionValues.StatusPreparing,
                Memo: null));

            if (!string.Equals(status, ClaimSubmissionValues.StatusPreparing, StringComparison.Ordinal))
            {
                submission = await Submissions.UpdateAsync(
                    submission.Id,
                    submission.Revision,
                    SubmissionDraft(claim.Id, policy.Id, ClaimSubmissionValues.StatusSubmitted));
            }

            if (string.Equals(status, ClaimSubmissionValues.StatusCompleted, StringComparison.Ordinal))
            {
                submission = await Submissions.UpdateAsync(
                    submission.Id,
                    submission.Revision,
                    SubmissionDraft(claim.Id, policy.Id, ClaimSubmissionValues.StatusReviewing));
                submission = await Submissions.UpdateAsync(
                    submission.Id,
                    submission.Revision,
                    SubmissionDraft(claim.Id, policy.Id, ClaimSubmissionValues.StatusCompleted));
            }

            return submission;
        }

        public void Dispose()
        {
            if (Directory.Exists(RootPath))
            {
                Directory.Delete(RootPath, recursive: true);
            }
        }

        private static ClaimSubmissionDraft SubmissionDraft(
            string claimId,
            string policyId,
            string status)
        {
            return new ClaimSubmissionDraft(
                claimId,
                policyId,
                PolicyCoverageId: null,
                CoverageDisplayName: "synthetic coverage",
                SubmittedDate: new DateOnly(2026, 8, 8),
                SubmittedAmount: null,
                SubmittedClaimDocumentIds: [],
                status,
                Memo: null);
        }
    }

    private sealed class ToggleLoadClaimPaymentStorage(IClaimPaymentStorageService inner)
        : IClaimPaymentStorageService
    {
        public bool FailLoads { get; set; }

        public Task<IReadOnlyList<ClaimPaymentRecord>> GetBySubmissionAsync(
            string claimSubmissionId,
            CancellationToken cancellationToken = default)
        {
            return FailLoads
                ? Task.FromException<IReadOnlyList<ClaimPaymentRecord>>(
                    new InvalidOperationException("Synthetic load failure."))
                : inner.GetBySubmissionAsync(claimSubmissionId, cancellationToken);
        }

        public Task<ClaimPaymentRecord?> GetAsync(
            string id,
            CancellationToken cancellationToken = default) =>
            inner.GetAsync(id, cancellationToken);

        public Task<ClaimPaymentRecord> CreateAsync(
            ClaimPaymentDraft draft,
            CancellationToken cancellationToken = default) =>
            inner.CreateAsync(draft, cancellationToken);

        public Task<ClaimPaymentRecord> UpdateAsync(
            string id,
            int expectedRevision,
            ClaimPaymentDraft draft,
            CancellationToken cancellationToken = default) =>
            inner.UpdateAsync(id, expectedRevision, draft, cancellationToken);
    }

    private sealed class EchoUiTextProvider : IUiTextProvider
    {
        public string Get(string key) => key;

        public string Format(string key, params object?[] args)
        {
            return string.Format(Get(key), args);
        }
    }
}
