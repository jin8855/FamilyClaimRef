using System.Xml.Linq;
using FamilyClaimRef.App.Models.Storage;
using FamilyClaimRef.App.Services.Localization;
using FamilyClaimRef.App.Services.Storage;
using FamilyClaimRef.App.ViewModels;
using Xunit;

namespace FamilyClaimRef.App.Tests;

public sealed class HomeDashboardViewModelTests
{
    private static readonly DateTimeOffset Timestamp =
        new(2026, 8, 10, 10, 0, 0, TimeSpan.Zero);

    [Fact]
    public void Constructor_rejects_null_dependencies()
    {
        var fixture = new StubFixture();

        Assert.Throws<ArgumentNullException>(() => new HomeDashboardViewModel(
            null!, fixture.Submissions, fixture.Payments, fixture.Families, fixture.UiText));
        Assert.Throws<ArgumentNullException>(() => new HomeDashboardViewModel(
            fixture.History, null!, fixture.Payments, fixture.Families, fixture.UiText));
        Assert.Throws<ArgumentNullException>(() => new HomeDashboardViewModel(
            fixture.History, fixture.Submissions, null!, fixture.Families, fixture.UiText));
        Assert.Throws<ArgumentNullException>(() => new HomeDashboardViewModel(
            fixture.History, fixture.Submissions, fixture.Payments, null!, fixture.UiText));
        Assert.Throws<ArgumentNullException>(() => new HomeDashboardViewModel(
            fixture.History, fixture.Submissions, fixture.Payments, fixture.Families, null!));
    }

    [Fact]
    public async Task Load_reads_the_full_graph_in_contract_order_and_calculates_summary_counts()
    {
        var fixture = new StubFixture();
        fixture.AddClaim("claim_no_submission", "family_1", "No submission", Timestamp.AddHours(1));
        fixture.AddClaim("claim_completed", "family_1", "Completed", Timestamp.AddHours(2));
        fixture.AddSubmission(
            "submission_completed",
            "claim_completed",
            "policy_1",
            ClaimSubmissionValues.StatusCompleted,
            Timestamp.AddHours(3));
        var viewModel = fixture.CreateViewModel();

        Assert.True(await viewModel.LoadAsync());

        Assert.Equal(["claims", "policies", "families", "submissions", "payments"], fixture.CallOrder);
        Assert.Equal(1, viewModel.InProgressClaimCount);
        Assert.Equal(1, viewModel.NoSubmissionClaimCount);
        Assert.Equal(1, viewModel.PaymentResultPendingCount);
        Assert.Equal(2, viewModel.RecentActivities.Count);
        Assert.True(viewModel.HasLoadedProjection);
        Assert.Null(viewModel.StateMessage);
    }

    [Fact]
    public async Task No_submission_count_requires_zero_submissions_including_cancelled_records()
    {
        var fixture = new StubFixture();
        fixture.Submissions.Records[0] = fixture.Submissions.Records[0] with
        {
            Status = ClaimSubmissionValues.StatusCancelled
        };
        fixture.AddClaim("claim_without_submission", "family_1", "No submission", Timestamp.AddHours(1));
        var viewModel = fixture.CreateViewModel();

        Assert.True(await viewModel.LoadAsync());

        Assert.Equal(1, viewModel.NoSubmissionClaimCount);
        Assert.Equal(0, viewModel.InProgressClaimCount);
    }

    [Fact]
    public async Task In_progress_count_is_distinct_by_claim_and_excludes_completed_and_cancelled()
    {
        var fixture = new StubFixture();
        fixture.AddSubmission(
            "submission_reviewing",
            "claim_1",
            "policy_1",
            ClaimSubmissionValues.StatusReviewing,
            Timestamp.AddHours(1));
        fixture.AddClaim("claim_completed", "family_1", "Completed", Timestamp.AddHours(2));
        fixture.AddSubmission(
            "submission_completed",
            "claim_completed",
            "policy_1",
            ClaimSubmissionValues.StatusCompleted,
            Timestamp.AddHours(3));
        fixture.AddClaim("claim_cancelled", "family_1", "Cancelled", Timestamp.AddHours(4));
        fixture.AddSubmission(
            "submission_cancelled",
            "claim_cancelled",
            "policy_1",
            ClaimSubmissionValues.StatusCancelled,
            Timestamp.AddHours(5));
        var viewModel = fixture.CreateViewModel();

        Assert.True(await viewModel.LoadAsync());

        Assert.Equal(1, viewModel.InProgressClaimCount);
    }

    [Fact]
    public async Task Payment_pending_requires_completed_submission_without_any_terminal_payment()
    {
        var fixture = new StubFixture();
        fixture.Submissions.Records[0] = fixture.Submissions.Records[0] with
        {
            Status = ClaimSubmissionValues.StatusCompleted
        };
        fixture.AddPayment("payment_pending", "submission_1", ClaimPaymentValues.StatusPending, Timestamp);
        var viewModel = fixture.CreateViewModel();

        Assert.True(await viewModel.LoadAsync());
        Assert.Equal(1, viewModel.PaymentResultPendingCount);

        fixture.AddPayment("payment_paid", "submission_1", ClaimPaymentValues.StatusPaid, Timestamp.AddHours(1));
        Assert.True(await viewModel.LoadAsync());
        Assert.Equal(0, viewModel.PaymentResultPendingCount);
    }

    [Fact]
    public async Task Recent_activity_uses_latest_payment_update_orders_deterministically_and_limits_to_five()
    {
        var fixture = new StubFixture();
        fixture.Submissions.Records.Clear();
        for (var index = 1; index <= 6; index++)
        {
            var suffix = index.ToString("00");
            fixture.AddPolicy($"policy_{suffix}", "family_1", $"Policy {suffix}", $"Insurer {suffix}");
            fixture.AddSubmission(
                $"submission_{suffix}",
                "claim_1",
                $"policy_{suffix}",
                ClaimSubmissionValues.StatusSubmitted,
                Timestamp);
        }

        fixture.AddPayment(
            "payment_latest",
            "submission_06",
            ClaimPaymentValues.StatusPending,
            Timestamp.AddDays(1));
        var viewModel = fixture.CreateViewModel();

        Assert.True(await viewModel.LoadAsync());

        Assert.Equal(5, viewModel.RecentActivities.Count);
        Assert.Equal("Policy 06", viewModel.RecentActivities[0].PolicyDisplayTitle);
        Assert.Equal(
            ["Policy 06", "Policy 01", "Policy 02", "Policy 03", "Policy 04"],
            viewModel.RecentActivities.Select(activity => activity.PolicyDisplayTitle));
    }

    [Fact]
    public async Task Disabled_parents_are_excluded_from_active_counts_but_remain_in_recent_activity()
    {
        var fixture = new StubFixture();
        fixture.Families.Records["family_1"] = fixture.Families.Records["family_1"] with
        {
            DisabledAt = Timestamp
        };
        var viewModel = fixture.CreateViewModel();

        Assert.True(await viewModel.LoadAsync());

        Assert.Equal(0, viewModel.InProgressClaimCount);
        Assert.Equal(0, viewModel.NoSubmissionClaimCount);
        Assert.Equal(0, viewModel.PaymentResultPendingCount);
        Assert.Equal("disabled", Assert.Single(viewModel.RecentActivities).ParentStateDisplay);
    }

    [Theory]
    [InlineData("missing", "reference")]
    [InlineData("legacy", "legacy")]
    [InlineData("ownership", "ownership")]
    [InlineData("unknown_claim", "unknown")]
    [InlineData("unknown_submission", "unknown")]
    [InlineData("unknown_payment", "unknown")]
    [InlineData("draft_parent", "reference")]
    public async Task Invalid_graph_states_fail_closed(string scenario, string expectedMessage)
    {
        var fixture = new StubFixture();
        switch (scenario)
        {
            case "missing":
                fixture.Families.Records.Clear();
                break;
            case "legacy":
                fixture.History.Claims[0] = fixture.History.Claims[0] with { FamilyMemberId = null };
                break;
            case "ownership":
                fixture.AddFamily("family_2", "Family Two");
                fixture.History.Policies[0] = fixture.History.Policies[0] with
                {
                    FamilyMemberId = "family_2"
                };
                break;
            case "unknown_claim":
                fixture.History.Claims[0] = fixture.History.Claims[0] with { CaseStatus = "unknown" };
                break;
            case "unknown_submission":
                fixture.Submissions.Records[0] = fixture.Submissions.Records[0] with { Status = "unknown" };
                break;
            case "unknown_payment":
                fixture.AddPayment("payment_1", "submission_1", "unknown", Timestamp);
                break;
            case "draft_parent":
                fixture.History.Claims[0] = fixture.History.Claims[0] with
                {
                    CaseStatus = ClaimCaseValues.StatusDraft
                };
                break;
        }

        var viewModel = fixture.CreateViewModel();
        Assert.False(await viewModel.LoadAsync());
        Assert.Equal(expectedMessage, viewModel.StateMessage);
        Assert.False(viewModel.HasLoadedProjection);
        Assert.Empty(viewModel.RecentActivities);
        Assert.Equal(0, viewModel.InProgressClaimCount);
        Assert.Equal(0, viewModel.NoSubmissionClaimCount);
        Assert.Equal(0, viewModel.PaymentResultPendingCount);
    }

    [Theory]
    [InlineData("orphan_submission")]
    [InlineData("orphan_payment")]
    [InlineData("duplicate_claim")]
    [InlineData("duplicate_policy")]
    [InlineData("duplicate_family")]
    [InlineData("duplicate_submission")]
    [InlineData("duplicate_payment")]
    public async Task Orphans_and_duplicate_keys_fail_closed_before_calculation(string scenario)
    {
        var fixture = new StubFixture();
        switch (scenario)
        {
            case "orphan_submission":
                fixture.AddSubmission(
                    "submission_orphan",
                    "missing_claim",
                    "policy_1",
                    ClaimSubmissionValues.StatusSubmitted,
                    Timestamp);
                break;
            case "orphan_payment":
                fixture.AddPayment("payment_orphan", "missing_submission", ClaimPaymentValues.StatusPending, Timestamp);
                break;
            case "duplicate_claim":
                fixture.History.Claims.Add(fixture.History.Claims[0]);
                break;
            case "duplicate_policy":
                fixture.History.Policies.Add(fixture.History.Policies[0]);
                break;
            case "duplicate_family":
                fixture.Families.DuplicateRecords.Add(fixture.Families.Records["family_1"]);
                break;
            case "duplicate_submission":
                fixture.Submissions.Records.Add(fixture.Submissions.Records[0]);
                break;
            case "duplicate_payment":
                fixture.AddPayment("payment_1", "submission_1", ClaimPaymentValues.StatusPending, Timestamp);
                fixture.Payments.Records.Add(fixture.Payments.Records[0]);
                break;
        }

        var viewModel = fixture.CreateViewModel();
        Assert.False(await viewModel.LoadAsync());
        Assert.Equal("reference", viewModel.StateMessage);
        Assert.Empty(viewModel.RecentActivities);
    }

    [Fact]
    public async Task Load_failure_clears_previous_projection_without_disclosing_diagnostics()
    {
        var fixture = new StubFixture();
        var viewModel = fixture.CreateViewModel();
        Assert.True(await viewModel.LoadAsync());
        fixture.History.LoadException = new IOException(
            @"C:\Users\local-user\claims.json contained private diagnostics");

        Assert.False(await viewModel.LoadAsync());

        Assert.False(viewModel.HasLoadedProjection);
        Assert.Empty(viewModel.RecentActivities);
        Assert.Equal("failed", viewModel.StateMessage);
        Assert.DoesNotContain("local-user", viewModel.StateMessage, StringComparison.Ordinal);
        Assert.DoesNotContain("claims.json", viewModel.StateMessage, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Empty_store_is_a_successful_loaded_projection_with_safe_notice()
    {
        var fixture = new StubFixture();
        fixture.Clear();
        var viewModel = fixture.CreateViewModel();

        Assert.True(await viewModel.LoadAsync());

        Assert.True(viewModel.HasLoadedProjection);
        Assert.Equal("empty", viewModel.StateMessage);
        Assert.Empty(viewModel.RecentActivities);
        Assert.Equal(0, viewModel.InProgressClaimCount);
        Assert.Equal(0, viewModel.NoSubmissionClaimCount);
        Assert.Equal(0, viewModel.PaymentResultPendingCount);
    }

    [Fact]
    public async Task Cancellation_is_rethrown_and_busy_state_and_gate_are_restored()
    {
        var fixture = new StubFixture { CancelClaimsRead = true };
        var viewModel = fixture.CreateViewModel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
            viewModel.LoadAsync(new CancellationToken(canceled: true)));

        Assert.False(viewModel.IsBusy);
        fixture.CancelClaimsRead = false;
        Assert.True(await viewModel.LoadAsync());
    }

    [Fact]
    public async Task Concurrent_load_is_rejected_without_duplicate_reader_execution()
    {
        var fixture = new StubFixture { BlockClaimsRead = true };
        var viewModel = fixture.CreateViewModel();
        var firstLoad = viewModel.LoadAsync();
        await fixture.ClaimsReadStarted.Task.WaitAsync(TimeSpan.FromSeconds(5));

        Assert.False(await viewModel.LoadAsync());
        Assert.Equal(1, fixture.History.ClaimReadCount);

        fixture.ReleaseClaimsRead.TrySetResult();
        Assert.True(await firstLoad);
        Assert.False(viewModel.IsBusy);
    }

    [Fact]
    public async Task Load_does_not_mutate_reader_records()
    {
        var fixture = new StubFixture();
        var claims = fixture.History.Claims.ToArray();
        var policies = fixture.History.Policies.ToArray();
        var families = fixture.Families.GetSnapshot();
        var submissions = fixture.Submissions.Records.ToArray();
        var payments = fixture.Payments.Records.ToArray();

        Assert.True(await fixture.CreateViewModel().LoadAsync());

        Assert.Equal(claims, fixture.History.Claims);
        Assert.Equal(policies, fixture.History.Policies);
        Assert.Equal(families, fixture.Families.GetSnapshot());
        Assert.Equal(submissions, fixture.Submissions.Records);
        Assert.Equal(payments, fixture.Payments.Records);
    }

    [Fact]
    public void Home_view_binds_only_the_dashboard_projection_and_preserves_navigation_ids()
    {
        var projectRoot = FindProjectRoot();
        var viewPath = Path.Combine(
            projectRoot,
            "app",
            "FamilyClaimRef.App",
            "Views",
            "ProductHomeView.xaml");
        var codePath = viewPath + ".cs";
        var viewText = File.ReadAllText(viewPath);
        var codeText = File.ReadAllText(codePath);
        var document = XDocument.Load(viewPath);
        var automationIds = document
            .Descendants()
            .SelectMany(element => element.Attributes())
            .Where(attribute => attribute.Name.LocalName.EndsWith(".AutomationId", StringComparison.Ordinal))
            .Select(attribute => attribute.Value)
            .ToArray();

        Assert.Contains("ProductHome_Claim", automationIds);
        Assert.Contains("ProductHome_Policy", automationIds);
        Assert.Contains("ProductHome_History", automationIds);
        Assert.Contains("ProductHome_Manage", automationIds);
        Assert.Contains("ProductHomeDashboard_Refresh", automationIds);
        Assert.Contains("ProductHomeDashboard_InProgressCount", automationIds);
        Assert.Contains("ProductHomeDashboard_NoSubmissionCount", automationIds);
        Assert.Contains("ProductHomeDashboard_PaymentPendingCount", automationIds);
        Assert.Contains("ProductHomeDashboard_RecentActivities", automationIds);
        Assert.Contains("ProductHomeDashboard_StateMessage", automationIds);
        Assert.DoesNotContain("AvailableClaims.Count", viewText, StringComparison.Ordinal);
        Assert.DoesNotContain("DocumentList.Items.Count", viewText, StringComparison.Ordinal);
        Assert.DoesNotContain("PolicyClaimManagement.LoadAsync", codeText, StringComparison.Ordinal);
        Assert.DoesNotContain("DocumentList.LoadAsync", codeText, StringComparison.Ordinal);
        Assert.Contains("HomeDashboard.LoadAsync", codeText, StringComparison.Ordinal);
        Assert.Contains("HorizontalScrollBarVisibility=\"Disabled\"", viewText, StringComparison.Ordinal);
        Assert.DoesNotContain(automationIds, value => value.Contains("{Binding", StringComparison.Ordinal));
    }

    private static string FindProjectRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "FamilyClaimRef.sln")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException("FamilyClaimRef project root was not found.");
    }

    private sealed class StubFixture
    {
        public StubFixture()
        {
            History.CallOrder = CallOrder;
            Submissions.CallOrder = CallOrder;
            Payments.CallOrder = CallOrder;
            Families.CallOrder = CallOrder;
            AddFamily("family_1", "Family One");
            AddPolicy("policy_1", "family_1", "Policy One", "Insurer One");
            AddClaim("claim_1", "family_1", "Claim One", Timestamp);
            AddSubmission(
                "submission_1",
                "claim_1",
                "policy_1",
                ClaimSubmissionValues.StatusSubmitted,
                Timestamp);
        }

        public List<string> CallOrder { get; } = [];
        public StubHistoryReader History { get; } = new();
        public StubSubmissionReader Submissions { get; } = new();
        public StubPaymentReader Payments { get; } = new();
        public StubFamilyStorage Families { get; } = new();
        public IUiTextProvider UiText { get; } = new FakeUiTextProvider();
        public TaskCompletionSource ClaimsReadStarted { get; } =
            new(TaskCreationOptions.RunContinuationsAsynchronously);
        public TaskCompletionSource ReleaseClaimsRead { get; } =
            new(TaskCreationOptions.RunContinuationsAsynchronously);

        public bool BlockClaimsRead
        {
            set
            {
                History.BlockClaimsRead = value;
                History.ClaimsReadStarted = ClaimsReadStarted;
                History.ReleaseClaimsRead = ReleaseClaimsRead;
            }
        }

        public bool CancelClaimsRead
        {
            set => History.CancelClaimsRead = value;
        }

        public HomeDashboardViewModel CreateViewModel() => new(
            History,
            Submissions,
            Payments,
            Families,
            UiText);

        public void AddFamily(string id, string displayName)
        {
            Families.Records[id] = new FamilyMemberRecord(
                id,
                displayName,
                FamilyMemberRelationValues.Self,
                null,
                Timestamp,
                Timestamp,
                null,
                1);
        }

        public void AddPolicy(string id, string familyId, string title, string insurer)
        {
            History.Policies.Add(new PolicyRecord(
                id,
                title,
                new DateOnly(2026, 8, 1),
                Timestamp,
                Timestamp,
                null,
                familyId,
                insurer));
        }

        public void AddClaim(string id, string familyId, string title, DateTimeOffset updatedAt)
        {
            History.Claims.Add(new ClaimRecord(
                id,
                null,
                title,
                new DateOnly(2026, 8, 10),
                Timestamp,
                updatedAt,
                null,
                familyId,
                "Hospital",
                "D01",
                "Diagnosis",
                ClaimCaseValues.VisitTypeOutpatient,
                CaseStatus: ClaimCaseValues.StatusSaved,
                Revision: 1));
        }

        public void AddSubmission(
            string id,
            string claimId,
            string policyId,
            string status,
            DateTimeOffset updatedAt)
        {
            Submissions.Records.Add(new ClaimSubmissionRecord(
                id,
                claimId,
                policyId,
                null,
                "Coverage",
                new DateOnly(2026, 8, 10),
                null,
                [],
                status,
                null,
                1,
                Timestamp,
                updatedAt));
        }

        public void AddPayment(string id, string submissionId, string status, DateTimeOffset updatedAt)
        {
            Payments.Records.Add(new ClaimPaymentRecord(
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
                updatedAt));
        }

        public void Clear()
        {
            CallOrder.Clear();
            History.Claims.Clear();
            History.Policies.Clear();
            Families.Records.Clear();
            Families.DuplicateRecords.Clear();
            Submissions.Records.Clear();
            Payments.Records.Clear();
        }
    }

    private sealed class StubHistoryReader : IClaimHistoryStorageReader
    {
        public List<PolicyRecord> Policies { get; } = [];
        public List<ClaimRecord> Claims { get; } = [];
        public List<string> CallOrder { get; set; } = [];
        public Exception? LoadException { get; set; }
        public bool BlockClaimsRead { get; set; }
        public bool CancelClaimsRead { get; set; }
        public TaskCompletionSource? ClaimsReadStarted { get; set; }
        public TaskCompletionSource? ReleaseClaimsRead { get; set; }
        public int ClaimReadCount { get; private set; }

        public Task<IReadOnlyList<PolicyRecord>> GetAllPoliciesForHistoryAsync(
            CancellationToken cancellationToken = default)
        {
            CallOrder.Add("policies");
            return Task.FromResult<IReadOnlyList<PolicyRecord>>(Policies.ToArray());
        }

        public async Task<IReadOnlyList<ClaimRecord>> GetAllClaimCasesForHistoryAsync(
            CancellationToken cancellationToken = default)
        {
            CallOrder.Add("claims");
            ClaimReadCount++;
            if (CancelClaimsRead)
            {
                throw new OperationCanceledException(cancellationToken);
            }

            if (LoadException is not null)
            {
                throw LoadException;
            }

            if (BlockClaimsRead)
            {
                ClaimsReadStarted!.TrySetResult();
                await ReleaseClaimsRead!.Task.WaitAsync(cancellationToken);
            }

            return Claims.ToArray();
        }
    }

    private sealed class StubSubmissionReader : IClaimSubmissionHistoryStorageReader
    {
        public List<ClaimSubmissionRecord> Records { get; } = [];
        public List<string> CallOrder { get; set; } = [];

        public Task<IReadOnlyList<ClaimSubmissionRecord>> GetAllSubmissionsForHistoryAsync(
            CancellationToken cancellationToken = default)
        {
            CallOrder.Add("submissions");
            return Task.FromResult<IReadOnlyList<ClaimSubmissionRecord>>(Records.ToArray());
        }
    }

    private sealed class StubPaymentReader : IClaimPaymentHistoryStorageReader
    {
        public List<ClaimPaymentRecord> Records { get; } = [];
        public List<string> CallOrder { get; set; } = [];

        public Task<IReadOnlyList<ClaimPaymentRecord>> GetAllPaymentsForHistoryAsync(
            CancellationToken cancellationToken = default)
        {
            CallOrder.Add("payments");
            return Task.FromResult<IReadOnlyList<ClaimPaymentRecord>>(Records.ToArray());
        }
    }

    private sealed class StubFamilyStorage : IFamilyMemberStorageService
    {
        public Dictionary<string, FamilyMemberRecord> Records { get; } = new(StringComparer.Ordinal);
        public List<FamilyMemberRecord> DuplicateRecords { get; } = [];
        public List<string> CallOrder { get; set; } = [];

        public Task<IReadOnlyList<FamilyMemberRecord>> GetFamilyMembersAsync(
            CancellationToken cancellationToken = default)
        {
            CallOrder.Add("families");
            return Task.FromResult<IReadOnlyList<FamilyMemberRecord>>(
                Records.Values.Concat(DuplicateRecords).ToArray());
        }

        public Task<IReadOnlyList<FamilyMemberRecord>> GetActiveFamilyMembersAsync(
            CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public Task<FamilyMemberRecord?> GetFamilyMemberAsync(
            string id,
            CancellationToken cancellationToken = default) => throw new NotSupportedException();

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

        public FamilyMemberRecord[] GetSnapshot() =>
            Records.Values.Concat(DuplicateRecords).OrderBy(record => record.Id, StringComparer.Ordinal).ToArray();
    }

    private sealed class FakeUiTextProvider : IUiTextProvider
    {
        private static readonly IReadOnlyDictionary<string, string> Values =
            new Dictionary<string, string>(StringComparer.Ordinal)
            {
                [UiTextKeys.ProductHistoryEmptyMessage] = "empty",
                [UiTextKeys.ProductHistoryReferenceMessage] = "reference",
                [UiTextKeys.ProductHistoryLegacyReviewMessage] = "legacy",
                [UiTextKeys.ProductHistoryOwnershipMessage] = "ownership",
                [UiTextKeys.ProductHistoryUnknownStatusMessage] = "unknown",
                [UiTextKeys.ProductHistoryLoadFailedMessage] = "failed",
                [UiTextKeys.ProductHistoryActiveState] = "active",
                [UiTextKeys.ProductHistoryDisabledState] = "disabled",
                [UiTextKeys.ProductClaimSubmissionStatusPreparing] = "preparing",
                [UiTextKeys.ProductClaimSubmissionStatusSubmitted] = "submitted",
                [UiTextKeys.ProductClaimSubmissionStatusAdditionalDocumentsRequested] = "additional",
                [UiTextKeys.ProductClaimSubmissionStatusReviewing] = "reviewing",
                [UiTextKeys.ProductClaimSubmissionStatusCancelled] = "cancelled",
                [UiTextKeys.ProductClaimSubmissionStatusCompleted] = "completed",
                [UiTextKeys.ProductClaimCompleteNoPaymentsValue] = "no payments",
                [UiTextKeys.ProductClaimCompleteNotEnteredValue] = "not entered",
                [UiTextKeys.ProductClaimCompletePaymentSummaryFormat] =
                    "pending {0} paid {1} partial {2} denied {3} cancelled {4}"
            };

        public string Get(string key) => Values.GetValueOrDefault(key, key);

        public string Format(string key, params object?[] args) => string.Format(Get(key), args);
    }
}
