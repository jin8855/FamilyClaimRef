using System.Xml.Linq;
using FamilyClaimRef.App.Models.Storage;
using FamilyClaimRef.App.Services.Localization;
using FamilyClaimRef.App.Services.Storage;
using FamilyClaimRef.App.ViewModels;
using Xunit;

namespace FamilyClaimRef.App.Tests;

public sealed class ClaimSubmissionManagementViewModelTests
{
    [Fact]
    public void Constructor_rejects_null_dependencies()
    {
        var fixture = new TestFixture();
        try
        {
            Assert.Throws<ArgumentNullException>(() => new ClaimSubmissionManagementViewModel(
                null!, fixture.PolicyClaims, fixture.Documents, fixture.UiText));
            Assert.Throws<ArgumentNullException>(() => new ClaimSubmissionManagementViewModel(
                fixture.Submissions, null!, fixture.Documents, fixture.UiText));
            Assert.Throws<ArgumentNullException>(() => new ClaimSubmissionManagementViewModel(
                fixture.Submissions, fixture.PolicyClaims, null!, fixture.UiText));
            Assert.Throws<ArgumentNullException>(() => new ClaimSubmissionManagementViewModel(
                fixture.Submissions, fixture.PolicyClaims, fixture.Documents, null!));
        }
        finally
        {
            fixture.Dispose();
        }
    }

    [Fact]
    public async Task Load_create_submit_and_reload_preserve_exact_claim_policy_and_revision()
    {
        await UsingFixtureAsync(async fixture =>
        {
            var family = await fixture.CreateFamilyAsync();
            var otherFamily = await fixture.CreateFamilyAsync("other family");
            var policy = await fixture.CreatePolicyAsync(family.Id, "selected policy");
            _ = await fixture.CreatePolicyAsync(otherFamily.Id, "excluded policy");
            var claim = await fixture.CreateSavedClaimAsync(family.Id);
            var document = await fixture.CreateClaimDocumentAsync(claim.Id);
            var viewModel = fixture.CreateViewModel();
            viewModel.SelectedClaimCaseId = claim.Id;

            Assert.True(await viewModel.LoadAsync());
            Assert.Equal(claim.Id, viewModel.SelectedClaimCaseId);
            Assert.Equal(policy.Id, Assert.Single(viewModel.AvailablePolicies).Id);
            Assert.Equal(document.Id, Assert.Single(viewModel.AvailableDocuments).Id);
            Assert.True(viewModel.CanCreate);

            Assert.True(await viewModel.CreatePreparingAsync());
            var createdId = Assert.IsType<string>(viewModel.SelectedSubmissionId);
            Assert.True(viewModel.IsEditMode);
            Assert.False(viewModel.HasUnsavedChanges);
            Assert.Equal("created", viewModel.OperationMessage);

            viewModel.CoverageDisplayName = "coverage";
            viewModel.SubmittedDate = new DateTime(2026, 8, 8);
            viewModel.SubmittedAmountText = "12,000";
            viewModel.SelectedStatus = ClaimSubmissionValues.StatusSubmitted;
            viewModel.Memo = "submitted memo";
            viewModel.AvailableDocuments.Single().IsSelected = true;
            Assert.False(viewModel.CanNavigateAway);
            Assert.True(viewModel.CanSave);

            Assert.True(await viewModel.SaveAsync());
            Assert.True(viewModel.CanNavigateAway);
            Assert.Equal("saved", viewModel.OperationMessage);

            var reloaded = fixture.CreateViewModel();
            reloaded.SelectedClaimCaseId = claim.Id;
            Assert.True(await reloaded.LoadAsync());
            reloaded.SelectedSubmissionId = createdId;
            Assert.True(reloaded.LoadSelectedSubmission());

            Assert.Equal(policy.Id, reloaded.SelectedPolicyId);
            Assert.Equal("coverage", reloaded.CoverageDisplayName);
            Assert.Equal(ClaimSubmissionValues.StatusSubmitted, reloaded.SelectedStatus);
            Assert.Equal("12,000", reloaded.SubmittedAmountText);
            Assert.True(reloaded.AvailableDocuments.Single().IsSelected);
            Assert.Equal(2, (await fixture.Submissions.GetAsync(createdId))!.Revision);
        });
    }

    [Fact]
    public async Task Load_excludes_disabled_saved_claim_case()
    {
        await UsingFixtureAsync(async fixture =>
        {
            var family = await fixture.CreateFamilyAsync();
            _ = await fixture.CreatePolicyAsync(family.Id);
            var active = await fixture.CreateSavedClaimAsync(family.Id, "active claim");
            var disabled = await fixture.CreateSavedClaimAsync(family.Id, "disabled claim");
            disabled = await fixture.PolicyClaims.DisableClaimCaseAsync(
                disabled.Id,
                disabled.Revision);
            var viewModel = fixture.CreateViewModel(
                claimCases: new StaticClaimCaseStorage([disabled, active]));
            viewModel.SelectedClaimCaseId = active.Id;

            Assert.True(await viewModel.LoadAsync());
            Assert.Equal(active.Id, Assert.Single(viewModel.AvailableClaimCases).Id);
            Assert.DoesNotContain(
                viewModel.AvailableClaimCases,
                option => string.Equals(option.Id, disabled.Id, StringComparison.Ordinal));
        });
    }

    [Fact]
    public async Task Blank_submitted_amount_saves_and_reloads_as_null()
    {
        await UsingFixtureAsync(async fixture =>
        {
            var family = await fixture.CreateFamilyAsync();
            var policy = await fixture.CreatePolicyAsync(family.Id);
            var claim = await fixture.CreateSavedClaimAsync(family.Id);
            var viewModel = fixture.CreateViewModel();
            viewModel.SelectedClaimCaseId = claim.Id;
            Assert.True(await viewModel.LoadAsync());
            Assert.True(await viewModel.CreatePreparingAsync());

            viewModel.CoverageDisplayName = "coverage";
            viewModel.SubmittedDate = new DateTime(2026, 8, 8);
            viewModel.SubmittedAmountText = " ";
            viewModel.SelectedStatus = ClaimSubmissionValues.StatusSubmitted;

            Assert.True(await viewModel.SaveAsync());
            var id = viewModel.SelectedSubmissionId!;
            Assert.Null((await fixture.Submissions.GetAsync(id))!.SubmittedAmount);

            var reloaded = fixture.CreateViewModel();
            reloaded.SelectedClaimCaseId = claim.Id;
            Assert.True(await reloaded.LoadAsync());
            reloaded.SelectedSubmissionId = id;
            Assert.True(reloaded.LoadSelectedSubmission());
            Assert.Null(reloaded.SubmittedAmountText);
            Assert.Equal(policy.Id, reloaded.SelectedPolicyId);
        });
    }

    [Fact]
    public async Task Invalid_input_stays_on_editor_and_uses_safe_validation_message()
    {
        await UsingFixtureAsync(async fixture =>
        {
            var family = await fixture.CreateFamilyAsync();
            _ = await fixture.CreatePolicyAsync(family.Id);
            var claim = await fixture.CreateSavedClaimAsync(family.Id);
            var viewModel = fixture.CreateViewModel();
            viewModel.SelectedClaimCaseId = claim.Id;
            Assert.True(await viewModel.LoadAsync());
            Assert.True(await viewModel.CreatePreparingAsync());

            var submissionId = viewModel.SelectedSubmissionId;
            viewModel.SubmittedAmountText = "not a number";

            Assert.False(await viewModel.SaveAsync());
            Assert.Equal("validation", viewModel.ValidationMessage);
            Assert.Equal(submissionId, viewModel.SelectedSubmissionId);
            Assert.True(viewModel.IsEditMode);
            Assert.True(viewModel.HasUnsavedChanges);
            Assert.DoesNotContain("submission_", viewModel.ValidationMessage, StringComparison.Ordinal);
        });
    }

    [Fact]
    public async Task Stale_revision_reports_conflict_without_overwriting_external_change()
    {
        await UsingFixtureAsync(async fixture =>
        {
            var family = await fixture.CreateFamilyAsync();
            var policy = await fixture.CreatePolicyAsync(family.Id);
            var claim = await fixture.CreateSavedClaimAsync(family.Id);
            var viewModel = fixture.CreateViewModel();
            viewModel.SelectedClaimCaseId = claim.Id;
            Assert.True(await viewModel.LoadAsync());
            Assert.True(await viewModel.CreatePreparingAsync());
            var id = viewModel.SelectedSubmissionId!;
            var current = (await fixture.Submissions.GetAsync(id))!;

            _ = await fixture.Submissions.UpdateAsync(
                id,
                current.Revision,
                CreatePreparingDraft(claim.Id, policy.Id) with { Memo = "external" });
            viewModel.Memo = "local";

            Assert.False(await viewModel.SaveAsync());
            Assert.Equal("conflict", viewModel.ConflictMessage);
            Assert.Equal("external", (await fixture.Submissions.GetAsync(id))!.Memo);
            Assert.True(viewModel.HasUnsavedChanges);
        });
    }

    [Fact]
    public async Task Forbidden_transition_reports_transition_message_and_keeps_record_unchanged()
    {
        await UsingFixtureAsync(async fixture =>
        {
            var family = await fixture.CreateFamilyAsync();
            _ = await fixture.CreatePolicyAsync(family.Id);
            var claim = await fixture.CreateSavedClaimAsync(family.Id);
            var viewModel = fixture.CreateViewModel();
            viewModel.SelectedClaimCaseId = claim.Id;
            Assert.True(await viewModel.LoadAsync());
            Assert.True(await viewModel.CreatePreparingAsync());
            var id = viewModel.SelectedSubmissionId!;

            viewModel.CoverageDisplayName = "coverage";
            viewModel.SubmittedDate = new DateTime(2026, 8, 8);
            viewModel.SelectedStatus = ClaimSubmissionValues.StatusReviewing;

            Assert.False(await viewModel.SaveAsync());
            Assert.Equal("transition", viewModel.TransitionMessage);
            Assert.Equal(
                ClaimSubmissionValues.StatusPreparing,
                (await fixture.Submissions.GetAsync(id))!.Status);
        });
    }

    [Fact]
    public async Task Terminal_target_can_be_saved_once_and_then_locks_editor()
    {
        await UsingFixtureAsync(async fixture =>
        {
            var family = await fixture.CreateFamilyAsync();
            _ = await fixture.CreatePolicyAsync(family.Id);
            var claim = await fixture.CreateSavedClaimAsync(family.Id);
            var viewModel = fixture.CreateViewModel();
            viewModel.SelectedClaimCaseId = claim.Id;
            Assert.True(await viewModel.LoadAsync());
            Assert.True(await viewModel.CreatePreparingAsync());

            viewModel.CoverageDisplayName = "coverage";
            viewModel.SubmittedDate = new DateTime(2026, 8, 8);
            viewModel.SubmittedAmountText = "12,000";
            viewModel.SelectedStatus = ClaimSubmissionValues.StatusSubmitted;
            Assert.True(await viewModel.SaveAsync());

            viewModel.SelectedStatus = ClaimSubmissionValues.StatusCompleted;
            Assert.True(viewModel.CanSave);
            Assert.True(viewModel.CanEditDetails);
            Assert.True(await viewModel.SaveAsync());

            var persisted = (await fixture.Submissions.GetAsync(
                viewModel.SelectedSubmissionId!))!;
            Assert.Equal(ClaimSubmissionValues.StatusCompleted, persisted.Status);
            Assert.Equal(3, persisted.Revision);
            Assert.False(viewModel.CanSave);
            Assert.False(viewModel.CanEditDetails);
            Assert.False(viewModel.HasUnsavedChanges);
        });
    }

    [Fact]
    public async Task Durable_mutation_survives_post_write_list_refresh_failure()
    {
        await UsingFixtureAsync(async fixture =>
        {
            var family = await fixture.CreateFamilyAsync();
            _ = await fixture.CreatePolicyAsync(family.Id);
            var claim = await fixture.CreateSavedClaimAsync(family.Id);
            var failingStorage = new RefreshFailingClaimSubmissionStorage(fixture.Submissions);
            var viewModel = fixture.CreateViewModel(failingStorage);
            viewModel.SelectedClaimCaseId = claim.Id;
            Assert.True(await viewModel.LoadAsync());

            failingStorage.FailNextListRead = true;
            Assert.True(await viewModel.CreatePreparingAsync());
            var id = viewModel.SelectedSubmissionId!;
            Assert.True(viewModel.IsEditMode);
            Assert.False(viewModel.CanCreate);
            Assert.False(viewModel.HasUnsavedChanges);
            Assert.Single(viewModel.Submissions);
            Assert.Equal(1, (await fixture.Submissions.GetAsync(id))!.Revision);

            viewModel.Memo = "updated after refresh failure";
            failingStorage.FailNextListRead = true;
            Assert.True(await viewModel.SaveAsync());

            var persisted = (await fixture.Submissions.GetAsync(id))!;
            Assert.Equal(2, persisted.Revision);
            Assert.Equal("updated after refresh failure", persisted.Memo);
            Assert.False(viewModel.HasUnsavedChanges);
            Assert.Equal("saved", viewModel.OperationMessage);
        });
    }
    [Fact]
    public void Dedicated_view_exposes_contract_fields_routes_and_no_raw_id_binding()
    {
        var root = FindProjectRoot();
        var path = Path.Combine(
            root,
            "app",
            "FamilyClaimRef.App",
            "Views",
            "ProductClaimSubmissionView.xaml");
        var document = XDocument.Load(path);
        XNamespace presentation = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";
        var values = document.Descendants()
            .SelectMany(element => element.Attributes())
            .Select(attribute => attribute.Value)
            .ToArray();

        Assert.Contains("ProductClaimSubmission_ClaimCase", values);
        Assert.Contains("ProductClaimSubmission_Policy", values);
        Assert.Contains("ProductClaimSubmission_Coverage", values);
        Assert.Contains("ProductClaimSubmission_Date", values);
        Assert.Contains("ProductClaimSubmission_Amount", values);
        Assert.Contains("ProductClaimSubmission_Documents", values);
        Assert.Contains("ProductClaimSubmission_Status", values);
        Assert.Contains("ProductClaimSubmission_Memo", values);
        Assert.Contains("ProductClaimSubmission_Create", values);
        Assert.Contains("ProductClaimSubmission_Save", values);
        Assert.Equal(6, values.Count(value => value == "{Binding CanEditDetails}"));
        Assert.Contains("{x:Static viewModels:ProductScreenRoutes.ClaimReferenceResult}", values);
        Assert.Contains("{x:Static viewModels:ProductScreenRoutes.ClaimComplete}", values);
        Assert.Contains("{x:Static viewModels:ProductScreenRoutes.HistoryView}", values);
        Assert.DoesNotContain(values, value => value.Contains("{Binding Id}", StringComparison.Ordinal));
        Assert.DoesNotContain(values, value => value.Contains("SelectedSubmissionId", StringComparison.Ordinal));
        Assert.DoesNotContain(values, value => value.Contains("PolicyCoverageId", StringComparison.Ordinal));
        var submissionRowStyle = Assert.Single(
            document.Descendants(presentation + "Style"),
            style => style.Attribute("TargetType")?.Value == "DataGridRow");
        Assert.Contains(
            submissionRowStyle.Elements(presentation + "Setter"),
            setter => setter.Attribute("Property")?.Value == "AutomationProperties.Name"
                && setter.Attribute("Value")?.Value == "{Binding PolicyDisplayTitle}");
    }

    private static ClaimSubmissionDraft CreatePreparingDraft(string claimId, string policyId)
    {
        return new ClaimSubmissionDraft(
            claimId,
            policyId,
            PolicyCoverageId: null,
            CoverageDisplayName: null,
            SubmittedDate: null,
            SubmittedAmount: null,
            SubmittedClaimDocumentIds: [],
            Status: ClaimSubmissionValues.StatusPreparing,
            Memo: null);
    }

    private static ClaimCaseDraft CreateClaimDraft(string familyMemberId, string title)
    {
        return new ClaimCaseDraft(
            title,
            familyMemberId,
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
            Memo: "synthetic memo");
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

        throw new InvalidOperationException("Project root was not found.");
    }

    private static async Task UsingFixtureAsync(Func<TestFixture, Task> action)
    {
        using var fixture = new TestFixture();
        await action(fixture);
    }

    private sealed class TestFixture : IDisposable
    {
        public TestFixture()
        {
            RootPath = Path.Combine(
                Path.GetTempPath(),
                "FamilyClaimRef.App.Tests",
                nameof(ClaimSubmissionManagementViewModelTests),
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
            UiText = new FakeUiTextProvider();
        }

        public string RootPath { get; }

        public JsonFamilyMemberStorageService Families { get; }

        public JsonPolicyClaimStorageService PolicyClaims { get; }

        public JsonDocumentStorageService Documents { get; }

        public JsonClaimSubmissionStorageService Submissions { get; }

        public IUiTextProvider UiText { get; }

        public ClaimSubmissionManagementViewModel CreateViewModel(
            IClaimSubmissionStorageService? submissions = null,
            IClaimCaseStorageService? claimCases = null)
        {
            return new ClaimSubmissionManagementViewModel(
                submissions ?? Submissions,
                claimCases ?? PolicyClaims,
                Documents,
                UiText);
        }

        public Task<FamilyMemberRecord> CreateFamilyAsync(string title = "synthetic family")
        {
            return Families.CreateFamilyMemberAsync(new FamilyMemberDraft(
                title,
                FamilyMemberRelationValues.Self,
                null));
        }

        public Task<PolicyRecord> CreatePolicyAsync(
            string familyMemberId,
            string title = "synthetic policy")
        {
            return PolicyClaims.CreateInsurancePolicyAsync(new InsurancePolicyDraft(
                title,
                familyMemberId,
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
        }

        public async Task<ClaimRecord> CreateSavedClaimAsync(
            string familyMemberId,
            string title = "synthetic claim")
        {
            var draft = CreateClaimDraft(familyMemberId, title);
            var created = await PolicyClaims.CreateClaimCaseAsync(draft);
            return await PolicyClaims.UpdateClaimCaseAsync(created.Id, created.Revision, draft);
        }

        public async Task<ClaimDocumentRecord> CreateClaimDocumentAsync(string claimId)
        {
            var physicalName = $"claim-{Guid.NewGuid():N}.pdf";
            var document = await Documents.AddDocumentAsync(new DocumentDraft(
                physicalName,
                "synthetic receipt",
                "pdf",
                $"claims/{claimId}/{physicalName}"));
            return await Documents.AddClaimDocumentAsync(new ClaimDocumentDraft(
                claimId,
                document.Id,
                "receipt"));
        }

        public void Dispose()
        {
            if (Directory.Exists(RootPath))
            {
                Directory.Delete(RootPath, recursive: true);
            }
        }
    }

    private sealed class StaticClaimCaseStorage(
        IReadOnlyList<ClaimRecord> claims) : IClaimCaseStorageService
    {
        public Task<IReadOnlyList<ClaimRecord>> GetClaimCasesAsync(
            CancellationToken cancellationToken = default) => Task.FromResult(claims);

        public Task<ClaimRecord?> GetClaimCaseAsync(
            string id,
            CancellationToken cancellationToken = default) => Task.FromResult(
                claims.SingleOrDefault(claim => string.Equals(
                    claim.Id,
                    id,
                    StringComparison.Ordinal)));

        public Task<ClaimRecord> CreateClaimCaseAsync(
            ClaimCaseDraft draft,
            CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public Task<ClaimRecord> UpdateClaimCaseAsync(
            string id,
            int expectedRevision,
            ClaimCaseDraft draft,
            CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public Task<ClaimRecord> DisableClaimCaseAsync(
            string id,
            int expectedRevision,
            CancellationToken cancellationToken = default) => throw new NotSupportedException();
    }

    private sealed class RefreshFailingClaimSubmissionStorage(
        IClaimSubmissionStorageService inner) : IClaimSubmissionStorageService
    {
        public bool FailNextListRead { get; set; }

        public Task<IReadOnlyList<ClaimSubmissionRecord>> GetByClaimCaseAsync(
            string claimCaseId,
            CancellationToken cancellationToken = default)
        {
            if (FailNextListRead)
            {
                FailNextListRead = false;
                throw new IOException("Synthetic post-write refresh failure.");
            }

            return inner.GetByClaimCaseAsync(claimCaseId, cancellationToken);
        }

        public Task<ClaimSubmissionRecord?> GetAsync(
            string id,
            CancellationToken cancellationToken = default)
        {
            return inner.GetAsync(id, cancellationToken);
        }

        public Task<IReadOnlyList<PolicyRecord>> GetClaimablePoliciesAsync(
            string claimCaseId,
            CancellationToken cancellationToken = default)
        {
            return inner.GetClaimablePoliciesAsync(claimCaseId, cancellationToken);
        }

        public Task<ClaimSubmissionRecord> CreateAsync(
            ClaimSubmissionDraft draft,
            CancellationToken cancellationToken = default)
        {
            return inner.CreateAsync(draft, cancellationToken);
        }

        public Task<ClaimSubmissionRecord> UpdateAsync(
            string id,
            int expectedRevision,
            ClaimSubmissionDraft draft,
            CancellationToken cancellationToken = default)
        {
            return inner.UpdateAsync(id, expectedRevision, draft, cancellationToken);
        }
    }
    private sealed class FakeUiTextProvider : IUiTextProvider
    {
        private static readonly IReadOnlyDictionary<string, string> Values =
            new Dictionary<string, string>(StringComparer.Ordinal)
            {
                [UiTextKeys.ProductClaimSubmissionValidationMessage] = "validation",
                [UiTextKeys.ProductClaimSubmissionConflictMessage] = "conflict",
                [UiTextKeys.ProductClaimSubmissionLegacyReviewMessage] = "legacy",
                [UiTextKeys.ProductClaimSubmissionReferenceMessage] = "reference",
                [UiTextKeys.ProductClaimSubmissionTransitionMessage] = "transition",
                [UiTextKeys.ProductClaimSubmissionOperationFailedMessage] = "failed",
                [UiTextKeys.ProductClaimSubmissionCreatedMessage] = "created",
                [UiTextKeys.ProductClaimSubmissionSavedMessage] = "saved",
                [UiTextKeys.ProductClaimSubmissionReferenceUnavailableValue] = "unavailable",
                [UiTextKeys.ProductClaimSubmissionNotEnteredValue] = "not entered",
                [UiTextKeys.ProductClaimSubmissionStatusPreparing] = "preparing",
                [UiTextKeys.ProductClaimSubmissionStatusSubmitted] = "submitted",
                [UiTextKeys.ProductClaimSubmissionStatusAdditionalDocumentsRequested] = "additional",
                [UiTextKeys.ProductClaimSubmissionStatusReviewing] = "reviewing",
                [UiTextKeys.ProductClaimSubmissionStatusCancelled] = "cancelled",
                [UiTextKeys.ProductClaimSubmissionStatusCompleted] = "completed"
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
}
