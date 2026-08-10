using System.Xml.Linq;
using FamilyClaimRef.App.Models.Storage;
using FamilyClaimRef.App.Services.Localization;
using FamilyClaimRef.App.Services.Storage;
using FamilyClaimRef.App.ViewModels;
using Xunit;

namespace FamilyClaimRef.App.Tests;

public sealed class ClaimHistoryViewModelTests
{
    private static readonly DateTimeOffset Timestamp =
        new(2026, 8, 10, 10, 0, 0, TimeSpan.Zero);

    [Fact]
    public void Constructor_rejects_null_dependencies()
    {
        var fixture = new StubFixture();

        Assert.Throws<ArgumentNullException>(() => new ClaimHistoryViewModel(
            null!, fixture.Submissions, fixture.Payments, fixture.Families, fixture.UiText));
        Assert.Throws<ArgumentNullException>(() => new ClaimHistoryViewModel(
            fixture.History, null!, fixture.Payments, fixture.Families, fixture.UiText));
        Assert.Throws<ArgumentNullException>(() => new ClaimHistoryViewModel(
            fixture.History, fixture.Submissions, null!, fixture.Families, fixture.UiText));
        Assert.Throws<ArgumentNullException>(() => new ClaimHistoryViewModel(
            fixture.History, fixture.Submissions, fixture.Payments, null!, fixture.UiText));
        Assert.Throws<ArgumentNullException>(() => new ClaimHistoryViewModel(
            fixture.History, fixture.Submissions, fixture.Payments, fixture.Families, null!));
    }

    [Fact]
    public async Task Load_projects_one_row_per_submission_and_orders_by_treatment_then_update()
    {
        var fixture = new StubFixture();
        fixture.AddPolicy("policy_2", "family_1", "Policy Two", "Insurer Two");
        fixture.AddSubmission("submission_2", "claim_1", "policy_2", Timestamp.AddHours(2));
        fixture.AddFamily("family_2", "Family Two");
        fixture.AddPolicy("policy_3", "family_2", "Policy Three", "Insurer Three");
        fixture.AddClaim(
            "claim_2",
            "family_2",
            "Claim Two",
            new DateOnly(2026, 8, 11),
            ClaimCaseValues.VisitTypeInpatient,
            "Diagnosis Two");
        fixture.AddSubmission("submission_3", "claim_2", "policy_3", Timestamp.AddHours(1));
        var viewModel = fixture.CreateViewModel();

        Assert.True(await viewModel.LoadAsync());

        Assert.Equal(3, viewModel.Items.Count);
        Assert.Equal(
            ["submission_3", "submission_2", "submission_1"],
            viewModel.Items.Select(item => item.SubmissionKey));
        Assert.Equal(2, viewModel.Items.Count(item => item.ClaimDisplayTitle == "Claim One"));
        var firstPolicy = viewModel.Items.Single(item => item.SubmissionKey == "submission_1");
        Assert.Equal("pending 0 paid 1 partial 0 denied 1 cancelled 0", firstPolicy.PaymentSummaryDisplay);
        Assert.True(viewModel.SelectItem(firstPolicy));
        Assert.Equal(2, viewModel.SelectedDetail!.Payments.Count);
        Assert.Contains(viewModel.SelectedDetail.Payments, payment => payment.StatusDisplay == "paid");
        Assert.Contains(viewModel.SelectedDetail.Payments, payment => payment.StatusDisplay == "denied");
        Assert.Equal("no payments", viewModel.Items.Single(
            item => item.SubmissionKey == "submission_2").PaymentSummaryDisplay);
    }

    [Fact]
    public async Task Filters_and_reset_cover_family_insurer_dates_visit_type_and_display_text()
    {
        var fixture = new StubFixture();
        fixture.AddPolicy("policy_2", "family_1", "Policy Two", "Insurer Two");
        fixture.AddSubmission("submission_2", "claim_1", "policy_2", Timestamp.AddHours(2));
        fixture.AddFamily("family_2", "Family Two");
        fixture.AddPolicy("policy_3", "family_2", "Policy Three", "Insurer Three");
        fixture.AddClaim(
            "claim_2",
            "family_2",
            "Claim Two",
            new DateOnly(2026, 8, 11),
            ClaimCaseValues.VisitTypeInpatient,
            "Diagnosis Two");
        fixture.AddSubmission("submission_3", "claim_2", "policy_3", Timestamp.AddHours(1));
        var viewModel = fixture.CreateViewModel();
        Assert.True(await viewModel.LoadAsync());

        viewModel.SelectedFamilyFilter = viewModel.FamilyFilterOptions.Single(
            option => option.DisplayName == "Family One");
        Assert.True(viewModel.ApplyFilters());
        Assert.Equal(2, viewModel.Items.Count);

        viewModel.ResetFilters();
        viewModel.SelectedInsurerFilter = viewModel.InsurerFilterOptions.Single(
            option => option.DisplayName == "Insurer Three");
        Assert.True(viewModel.ApplyFilters());
        Assert.Equal("submission_3", Assert.Single(viewModel.Items).SubmissionKey);

        viewModel.ResetFilters();
        viewModel.TreatmentDateFrom = new DateTime(2026, 8, 11);
        viewModel.TreatmentDateTo = new DateTime(2026, 8, 11);
        Assert.True(viewModel.ApplyFilters());
        Assert.Equal("submission_3", Assert.Single(viewModel.Items).SubmissionKey);

        viewModel.ResetFilters();
        viewModel.SelectedVisitTypeFilter = viewModel.VisitTypeFilterOptions.Single(
            option => option.Value == ClaimCaseValues.VisitTypeInpatient);
        Assert.True(viewModel.ApplyFilters());
        Assert.Equal("submission_3", Assert.Single(viewModel.Items).SubmissionKey);

        viewModel.ResetFilters();
        viewModel.SearchText = "diagnosis two";
        Assert.True(viewModel.ApplyFilters());
        Assert.Equal("submission_3", Assert.Single(viewModel.Items).SubmissionKey);

        viewModel.TreatmentDateFrom = new DateTime(2026, 8, 12);
        viewModel.TreatmentDateTo = new DateTime(2026, 8, 11);
        Assert.False(viewModel.ApplyFilters());
        Assert.Empty(viewModel.Items);
        Assert.Equal("date range", viewModel.StateMessage);

        viewModel.ResetFilters();
        Assert.Equal(3, viewModel.Items.Count);
        Assert.Null(viewModel.StateMessage);
    }

    [Fact]
    public async Task Claim_scope_includes_disabled_parent_and_missing_scope_fails_closed()
    {
        var fixture = new StubFixture();
        fixture.History.Claims[0] = fixture.History.Claims[0] with { DisabledAt = Timestamp };
        fixture.AddFamily("family_2", "Family Two");
        fixture.AddPolicy("policy_2", "family_2", "Policy Two", "Insurer Two");
        fixture.AddClaim(
            "claim_2",
            "family_2",
            "Claim Two",
            new DateOnly(2026, 8, 11),
            ClaimCaseValues.VisitTypeInpatient,
            "Diagnosis Two");
        fixture.AddSubmission("submission_2", "claim_2", "policy_2", Timestamp.AddHours(1));
        var viewModel = fixture.CreateViewModel();

        viewModel.SetClaimCaseScope("claim_1", resetFilters: true);
        Assert.True(await viewModel.LoadAsync());
        var item = Assert.Single(viewModel.Items);
        Assert.Equal("disabled", item.ParentStateDisplay);

        viewModel.SetClaimCaseScope("missing_claim", resetFilters: true);
        Assert.False(await viewModel.LoadAsync());
        Assert.Empty(viewModel.Items);
        Assert.False(viewModel.HasDetail);
        Assert.Equal("reference", viewModel.StateMessage);
    }

    [Fact]
    public async Task Selection_filters_and_detail_survive_a_read_only_reload_when_still_visible()
    {
        var fixture = new StubFixture();
        var viewModel = fixture.CreateViewModel();
        Assert.True(await viewModel.LoadAsync());
        var item = Assert.Single(viewModel.Items);
        Assert.True(viewModel.SelectItem(item));
        viewModel.SearchText = "Claim One";
        Assert.True(viewModel.ApplyFilters());

        Assert.True(await viewModel.LoadAsync());

        Assert.Single(viewModel.Items);
        Assert.True(viewModel.HasDetail);
        Assert.Equal("Claim One", viewModel.SelectedDetail!.ClaimDisplayTitle);
        Assert.Equal("Claim One", viewModel.SearchText);
    }

    [Fact]
    public async Task Missing_legacy_ownership_and_unknown_status_states_fail_closed()
    {
        var missing = new StubFixture();
        missing.Families.Records.Clear();
        await AssertFailureAsync(missing, "reference");

        var legacy = new StubFixture();
        legacy.History.Claims[0] = legacy.History.Claims[0] with { FamilyMemberId = null };
        await AssertFailureAsync(legacy, "legacy");

        var ownership = new StubFixture();
        ownership.History.Policies[0] = ownership.History.Policies[0] with { FamilyMemberId = "family_2" };
        await AssertFailureAsync(ownership, "ownership");

        var unknownSubmission = new StubFixture();
        unknownSubmission.Submissions.Records[0] = unknownSubmission.Submissions.Records[0] with
        {
            Status = "unexpected_submission_status"
        };
        await AssertFailureAsync(unknownSubmission, "unknown");

        var unknownPayment = new StubFixture();
        unknownPayment.Payments.Records[0] = unknownPayment.Payments.Records[0] with
        {
            Status = "unexpected_payment_status"
        };
        await AssertFailureAsync(unknownPayment, "unknown");
    }

    [Fact]
    public async Task Load_failure_removes_previous_list_and_detail_without_diagnostic_disclosure()
    {
        var fixture = new StubFixture();
        var viewModel = fixture.CreateViewModel();
        Assert.True(await viewModel.LoadAsync());
        Assert.True(viewModel.SelectItem(Assert.Single(viewModel.Items)));
        fixture.History.LoadException = new IOException(
            @"C:\Users\local-user\claims.json contained private diagnostics");

        Assert.False(await viewModel.LoadAsync());

        Assert.Empty(viewModel.Items);
        Assert.False(viewModel.HasDetail);
        Assert.Equal("failed", viewModel.StateMessage);
        Assert.DoesNotContain("local-user", viewModel.StateMessage, StringComparison.Ordinal);
        Assert.DoesNotContain("claims.json", viewModel.StateMessage, StringComparison.Ordinal);
        Assert.DoesNotContain("private diagnostics", viewModel.StateMessage, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Real_json_history_projection_is_byte_for_byte_read_only()
    {
        var root = Path.Combine(
            Path.GetTempPath(),
            "FamilyClaimRef.App.Tests",
            nameof(ClaimHistoryViewModelTests),
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
                new FamilyMemberDraft("Synthetic Family", FamilyMemberRelationValues.Self, null));
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
            var viewModel = new ClaimHistoryViewModel(
                policies,
                submissions,
                payments,
                families,
                new FakeUiTextProvider());

            Assert.True(await viewModel.LoadAsync());
            Assert.Single(viewModel.Items);

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
    public void Dedicated_list_and_detail_views_are_read_only_and_do_not_expose_data_ids()
    {
        var root = FindProjectRoot();
        var list = XDocument.Load(Path.Combine(
            root,
            "app",
            "FamilyClaimRef.App",
            "Views",
            "ProductClaimHistoryListView.xaml"));
        var detail = XDocument.Load(Path.Combine(
            root,
            "app",
            "FamilyClaimRef.App",
            "Views",
            "ProductClaimHistoryDetailView.xaml"));
        var values = list.Descendants().SelectMany(element => element.Attributes())
            .Concat(detail.Descendants().SelectMany(element => element.Attributes()))
            .Select(attribute => attribute.Value)
            .ToArray();

        Assert.Contains("ProductScreen_10", values);
        Assert.Contains("ProductScreen_21", values);
        Assert.DoesNotContain(values, value => value.Contains("SubmissionKey", StringComparison.Ordinal));
        Assert.DoesNotContain(values, value => value.Contains(".Id}", StringComparison.Ordinal));
        Assert.DoesNotContain(values, value => value.Contains("SaveCommand", StringComparison.Ordinal));
        Assert.DoesNotContain(values, value => value.Contains("UpdateCommand", StringComparison.Ordinal));
        Assert.DoesNotContain(values, value => value.Contains("DisableCommand", StringComparison.Ordinal));
        var detailActionColumn = list.Descendants().Single(
            element => element.Name.LocalName == "DataGridTemplateColumn");
        var cellAutomationNameSetter = detailActionColumn.Descendants().Single(
            element => element.Name.LocalName == "Setter"
                && element.Attribute("Property")?.Value == "AutomationProperties.Name");
        Assert.Equal("{Binding ClaimDisplayTitle}", cellAutomationNameSetter.Attribute("Value")?.Value);

        var dataGrids = list.Descendants().Concat(detail.Descendants())
            .Where(element => element.Name.LocalName == "DataGrid")
            .ToArray();
        Assert.Equal(2, dataGrids.Length);
        Assert.All(dataGrids, grid => Assert.Equal("True", grid.Attribute("IsReadOnly")?.Value));
        Assert.All(dataGrids, grid => Assert.Equal("False", grid.Attribute("CanUserAddRows")?.Value));
        Assert.All(dataGrids, grid => Assert.Equal("False", grid.Attribute("CanUserDeleteRows")?.Value));
        Assert.All(dataGrids, grid => Assert.Equal("Disabled", grid.Attribute("ScrollViewer.HorizontalScrollBarVisibility")?.Value));

        var shell = XDocument.Load(Path.Combine(
            root,
            "app",
            "FamilyClaimRef.App",
            "ProductShell",
            "ProductShellWindow.xaml"));
        Assert.Contains(shell.Descendants(), element =>
            element.Attribute("Value")?.Value == ProductScreenRoutes.HistoryView);
        Assert.Contains(shell.Descendants(), element =>
            element.Attribute("Value")?.Value == ProductScreenRoutes.HistoryDetail);
        Assert.Contains(shell.Descendants(), element =>
            element.Attribute("Value")?.Value == "{StaticResource ClaimHistoryListContentTemplate}");
        Assert.Contains(shell.Descendants(), element =>
            element.Attribute("Value")?.Value == "{StaticResource ClaimHistoryDetailContentTemplate}");
    }

    private static async Task AssertFailureAsync(StubFixture fixture, string expectedMessage)
    {
        var viewModel = fixture.CreateViewModel();
        Assert.False(await viewModel.LoadAsync());
        Assert.Empty(viewModel.Items);
        Assert.False(viewModel.HasDetail);
        Assert.Equal(expectedMessage, viewModel.StateMessage);
    }

    private static IReadOnlyDictionary<string, byte[]> SnapshotFiles(string root)
    {
        return Directory.EnumerateFiles(root, "*", SearchOption.AllDirectories)
            .ToDictionary(
                path => Path.GetRelativePath(root, path),
                File.ReadAllBytes,
                StringComparer.Ordinal);
    }

    private static InsurancePolicyDraft PolicyDraft(string familyId)
    {
        return new InsurancePolicyDraft(
            "Synthetic Policy",
            familyId,
            "Synthetic Insurer",
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
            "Synthetic Claim",
            familyId,
            new DateOnly(2026, 8, 10),
            "Synthetic Hospital",
            "A01",
            "Synthetic Diagnosis",
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
            AddFamily("family_1", "Family One");
            AddPolicy("policy_1", "family_1", "Policy One", "Insurer One");
            AddClaim(
                "claim_1",
                "family_1",
                "Claim One",
                new DateOnly(2026, 8, 10),
                ClaimCaseValues.VisitTypeOutpatient,
                "Diagnosis One");
            AddSubmission("submission_1", "claim_1", "policy_1", Timestamp);
            Payments.Records.Add(Payment(
                "payment_1",
                "submission_1",
                ClaimPaymentValues.StatusPaid,
                Timestamp.AddMinutes(1)));
            Payments.Records.Add(Payment(
                "payment_2",
                "submission_1",
                ClaimPaymentValues.StatusDenied,
                Timestamp.AddMinutes(2)));
        }

        public StubHistoryReader History { get; } = new();
        public StubSubmissionStorage Submissions { get; } = new();
        public StubPaymentStorage Payments { get; } = new();
        public StubFamilyStorage Families { get; } = new();
        public IUiTextProvider UiText { get; } = new FakeUiTextProvider();

        public ClaimHistoryViewModel CreateViewModel() => new(
            History,
            Submissions,
            Payments,
            Families,
            UiText);

        public void AddFamily(string id, string displayName, DateTimeOffset? disabledAt = null)
        {
            Families.Records[id] = new FamilyMemberRecord(
                id,
                displayName,
                FamilyMemberRelationValues.Self,
                null,
                Timestamp,
                Timestamp,
                disabledAt,
                1);
        }

        public void AddPolicy(
            string id,
            string familyId,
            string displayTitle,
            string insurer,
            DateTimeOffset? disabledAt = null)
        {
            History.Policies.Add(new PolicyRecord(
                id,
                displayTitle,
                null,
                Timestamp,
                Timestamp,
                disabledAt,
                familyId,
                insurer));
        }

        public void AddClaim(
            string id,
            string familyId,
            string displayTitle,
            DateOnly treatmentDate,
            string visitType,
            string diagnosisName,
            DateTimeOffset? disabledAt = null)
        {
            History.Claims.Add(new ClaimRecord(
                id,
                null,
                displayTitle,
                treatmentDate,
                Timestamp,
                Timestamp,
                disabledAt,
                familyId,
                "Synthetic Hospital",
                "A01",
                diagnosisName,
                visitType,
                false,
                false,
                10_000,
                0,
                0,
                null,
                ClaimCaseValues.StatusSaved,
                2));
        }

        public void AddSubmission(
            string id,
            string claimId,
            string policyId,
            DateTimeOffset updatedAt)
        {
            Submissions.Records.Add(new ClaimSubmissionRecord(
                id,
                claimId,
                policyId,
                null,
                "Coverage",
                new DateOnly(2026, 8, 10),
                10_000,
                [],
                ClaimSubmissionValues.StatusCompleted,
                "Submission memo",
                1,
                Timestamp,
                updatedAt));
        }

        private static ClaimPaymentRecord Payment(
            string id,
            string submissionId,
            string status,
            DateTimeOffset updatedAt)
        {
            return new ClaimPaymentRecord(
                id,
                submissionId,
                status,
                new DateOnly(2026, 8, 10),
                5_000,
                "Coverage",
                null,
                null,
                null,
                null,
                1,
                Timestamp,
                updatedAt);
        }
    }

    private sealed class FakeUiTextProvider : IUiTextProvider
    {
        private static readonly IReadOnlyDictionary<string, string> Values =
            new Dictionary<string, string>(StringComparer.Ordinal)
            {
                [UiTextKeys.ProductHistoryAllOption] = "all",
                [UiTextKeys.ProductHistoryActiveState] = "active",
                [UiTextKeys.ProductHistoryDisabledState] = "disabled",
                [UiTextKeys.ProductHistoryEmptyMessage] = "empty",
                [UiTextKeys.ProductHistoryFilterEmptyMessage] = "filter empty",
                [UiTextKeys.ProductHistoryReferenceMessage] = "reference",
                [UiTextKeys.ProductHistoryLegacyReviewMessage] = "legacy",
                [UiTextKeys.ProductHistoryOwnershipMessage] = "ownership",
                [UiTextKeys.ProductHistoryUnknownStatusMessage] = "unknown",
                [UiTextKeys.ProductHistoryLoadFailedMessage] = "failed",
                [UiTextKeys.ProductHistoryDateRangeMessage] = "date range",
                [UiTextKeys.ProductHistoryClaimStatusDraft] = "draft",
                [UiTextKeys.ProductClaimCompleteCaseStatusSaved] = "saved",
                [UiTextKeys.ProductClaimCompleteNoPaymentsValue] = "no payments",
                [UiTextKeys.ProductClaimCompleteNotEnteredValue] = "not entered",
                [UiTextKeys.ProductClaimCompletePaymentSummaryFormat] =
                    "pending {0} paid {1} partial {2} denied {3} cancelled {4}",
                [UiTextKeys.ProductClaimSubmissionStatusPreparing] = "preparing",
                [UiTextKeys.ProductClaimSubmissionStatusSubmitted] = "submitted",
                [UiTextKeys.ProductClaimSubmissionStatusAdditionalDocumentsRequested] = "additional",
                [UiTextKeys.ProductClaimSubmissionStatusReviewing] = "reviewing",
                [UiTextKeys.ProductClaimSubmissionStatusCancelled] = "cancelled",
                [UiTextKeys.ProductClaimSubmissionStatusCompleted] = "completed",
                [UiTextKeys.ProductClaimPaymentStatusPending] = "pending",
                [UiTextKeys.ProductClaimPaymentStatusPaid] = "paid",
                [UiTextKeys.ProductClaimPaymentStatusPartiallyPaid] = "partial",
                [UiTextKeys.ProductClaimPaymentStatusDenied] = "denied",
                [UiTextKeys.ProductClaimPaymentStatusCancelled] = "cancelled",
                [UiTextKeys.ProductClaimCaseVisitTypeOutpatient] = "outpatient",
                [UiTextKeys.ProductClaimCaseVisitTypeInpatient] = "inpatient"
            };

        public string Get(string key) => Values.GetValueOrDefault(key, key);

        public string Format(string key, params object?[] args) => string.Format(Get(key), args);
    }

    private sealed class StubHistoryReader : IClaimHistoryStorageReader
    {
        public List<PolicyRecord> Policies { get; } = [];
        public List<ClaimRecord> Claims { get; } = [];
        public Exception? LoadException { get; set; }

        public Task<IReadOnlyList<PolicyRecord>> GetAllPoliciesForHistoryAsync(
            CancellationToken cancellationToken = default)
        {
            if (LoadException is not null)
            {
                throw LoadException;
            }

            return Task.FromResult<IReadOnlyList<PolicyRecord>>(Policies.ToArray());
        }

        public Task<IReadOnlyList<ClaimRecord>> GetAllClaimCasesForHistoryAsync(
            CancellationToken cancellationToken = default)
        {
            if (LoadException is not null)
            {
                throw LoadException;
            }

            return Task.FromResult<IReadOnlyList<ClaimRecord>>(Claims.ToArray());
        }
    }

    private sealed class StubSubmissionStorage : IClaimSubmissionStorageService
    {
        public List<ClaimSubmissionRecord> Records { get; } = [];

        public Task<IReadOnlyList<ClaimSubmissionRecord>> GetByClaimCaseAsync(
            string claimCaseId,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<ClaimSubmissionRecord>>(Records
                .Where(record => string.Equals(record.ClaimCaseId, claimCaseId, StringComparison.Ordinal))
                .ToArray());

        public Task<ClaimSubmissionRecord?> GetAsync(
            string id,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(Records.FirstOrDefault(record => record.Id == id));

        public Task<IReadOnlyList<PolicyRecord>> GetClaimablePoliciesAsync(
            string claimCaseId,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<PolicyRecord>>([]);

        public Task<ClaimSubmissionRecord> CreateAsync(
            ClaimSubmissionDraft draft,
            CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public Task<ClaimSubmissionRecord> UpdateAsync(
            string id,
            int expectedRevision,
            ClaimSubmissionDraft draft,
            CancellationToken cancellationToken = default) => throw new NotSupportedException();
    }

    private sealed class StubPaymentStorage : IClaimPaymentStorageService
    {
        public List<ClaimPaymentRecord> Records { get; } = [];

        public Task<IReadOnlyList<ClaimPaymentRecord>> GetBySubmissionAsync(
            string claimSubmissionId,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<ClaimPaymentRecord>>(Records
                .Where(record => string.Equals(
                    record.ClaimSubmissionId,
                    claimSubmissionId,
                    StringComparison.Ordinal))
                .ToArray());

        public Task<ClaimPaymentRecord?> GetAsync(
            string id,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(Records.FirstOrDefault(record => record.Id == id));

        public Task<ClaimPaymentRecord> CreateAsync(
            ClaimPaymentDraft draft,
            CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public Task<ClaimPaymentRecord> UpdateAsync(
            string id,
            int expectedRevision,
            ClaimPaymentDraft draft,
            CancellationToken cancellationToken = default) => throw new NotSupportedException();
    }

    private sealed class StubFamilyStorage : IFamilyMemberStorageService
    {
        public Dictionary<string, FamilyMemberRecord> Records { get; } = new(StringComparer.Ordinal);

        public Task<IReadOnlyList<FamilyMemberRecord>> GetFamilyMembersAsync(
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<FamilyMemberRecord>>(Records.Values.ToArray());

        public Task<IReadOnlyList<FamilyMemberRecord>> GetActiveFamilyMembersAsync(
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<FamilyMemberRecord>>(Records.Values
                .Where(record => record.DisabledAt is null)
                .ToArray());

        public Task<FamilyMemberRecord?> GetFamilyMemberAsync(
            string id,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(Records.GetValueOrDefault(id));

        public Task<FamilyMemberRecord> CreateFamilyMemberAsync(
            FamilyMemberDraft draft,
            CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public Task<FamilyMemberRecord> UpdateFamilyMemberAsync(
            string id,
            int expectedVersion,
            FamilyMemberDraft draft,
            CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public Task<FamilyMemberRecord> DeactivateFamilyMemberAsync(
            string id,
            int expectedVersion,
            CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public Task<FamilyMemberRecord> ReactivateFamilyMemberAsync(
            string id,
            int expectedVersion,
            CancellationToken cancellationToken = default) => throw new NotSupportedException();
    }
}
