using FamilyClaimRef.App.Models.Storage;
using FamilyClaimRef.App.Services.Localization;
using FamilyClaimRef.App.Services.Storage;
using FamilyClaimRef.App.ViewModels;
using Xunit;

namespace FamilyClaimRef.App.Tests;

public sealed class PolicySearchViewModelTests
{
    [Fact]
    public async Task Load_projects_all_active_policies_in_source_order_and_excludes_disabled()
    {
        var familyStore = new StubFamilyMemberStorage(
        [
            CreateFamily("family_a", "가족 A"),
            CreateFamily("family_b", "가족 B")
        ]);
        var policyStore = new StubPolicyStorage(
        [
            CreatePolicy("policy_b", "보험 계약 B", "family_b", "보험사 B"),
            CreatePolicy("policy_disabled", "보험 계약 제외", "family_a", "보험사 A", disabled: true),
            CreatePolicy("policy_a", "보험 계약 A", "family_a", "보험사 A")
        ]);
        var viewModel = CreateViewModel(policyStore, familyStore);

        Assert.True(await viewModel.LoadPolicySearchAsync());

        Assert.Equal(["policy_b", "policy_a"], viewModel.PolicySearchResults.Select(item => item.Id));
        Assert.True(viewModel.HasLoadedPolicySearchProjection);
        Assert.True(viewModel.HasPolicySearchSource);
        Assert.True(viewModel.HasPolicySearchResults);
        Assert.False(viewModel.IsPolicySearchFilterEmpty);
        Assert.Null(viewModel.PolicySearchMessage);
        Assert.Equal(0, policyStore.WriteCount);
        Assert.Equal(0, familyStore.WriteCount);
    }

    [Fact]
    public async Task Family_filter_uses_exact_id_even_when_display_names_are_equal()
    {
        var familyStore = new StubFamilyMemberStorage(
        [
            CreateFamily("family_a", "가족 A"),
            CreateFamily("family_b", "가족 A")
        ]);
        var policyStore = new StubPolicyStorage(
        [
            CreatePolicy("policy_a", "보험 계약 A", "family_a", "보험사 A"),
            CreatePolicy("policy_b", "보험 계약 B", "family_b", "보험사 B")
        ]);
        var viewModel = CreateViewModel(policyStore, familyStore);
        await viewModel.LoadPolicySearchAsync();

        Assert.Equal(
            2,
            viewModel.PolicySearchFamilyOptions.Count(option => option.DisplayName == "가족 A"));
        viewModel.SelectedPolicySearchFamily = Option(
            viewModel.PolicySearchFamilyOptions,
            "family_b");

        viewModel.ApplyPolicySearch();

        Assert.Equal("policy_b", Assert.Single(viewModel.PolicySearchResults).Id);
    }

    [Fact]
    public async Task Exact_insurer_status_and_category_filters_preserve_legacy_display_contract()
    {
        var familyStore = new StubFamilyMemberStorage([CreateFamily("family_a", "가족 A")]);
        var policyStore = new StubPolicyStorage(
        [
            CreatePolicy(
                "policy_a",
                "보험 계약 A",
                "family_a",
                "보험사 A",
                InsurancePolicyValues.LegacyContractStatusActive,
                InsurancePolicyValues.ProductCategoryCancer),
            CreatePolicy(
                "policy_b",
                "보험 계약 B",
                "family_a",
                "보험사 AB",
                InsurancePolicyValues.ContractStatusExpired,
                InsurancePolicyValues.ProductCategoryDriver)
        ]);
        var viewModel = CreateViewModel(policyStore, familyStore);
        await viewModel.LoadPolicySearchAsync();

        viewModel.SelectedPolicySearchInsurer = Option(
            viewModel.PolicySearchInsurerOptions,
            "보험사 A");
        viewModel.SelectedPolicySearchContractStatus = Option(
            viewModel.PolicySearchContractStatusOptions,
            InsurancePolicyValues.ContractStatusActive);
        viewModel.SelectedPolicySearchProductCategory = Option(
            viewModel.PolicySearchProductCategoryOptions,
            InsurancePolicyValues.ProductCategoryCancer);
        viewModel.ApplyPolicySearch();

        var result = Assert.Single(viewModel.PolicySearchResults);
        Assert.Equal("policy_a", result.Id);
        Assert.Equal(InsurancePolicyValues.ContractStatusActive, result.ContractStatus);
    }

    [Fact]
    public async Task Keyword_is_trimmed_case_insensitive_and_limited_to_display_title()
    {
        var familyStore = new StubFamilyMemberStorage([CreateFamily("family_a", "가족 A")]);
        var policyStore = new StubPolicyStorage(
        [
            CreatePolicy("raw_keyword", "보험 Contract A", "family_a", "보험사 A"),
            CreatePolicy("policy_b", "보험 계약 B", "family_a", "keyword-insurer") with
            {
                CoveragePeriod = "keyword memo document diagnosis"
            }
        ]);
        var viewModel = CreateViewModel(policyStore, familyStore);
        await viewModel.LoadPolicySearchAsync();

        viewModel.PolicySearchKeyword = "  contract  ";
        viewModel.ApplyPolicySearch();

        Assert.Equal("contract", viewModel.PolicySearchKeyword);
        Assert.Equal("raw_keyword", Assert.Single(viewModel.PolicySearchResults).Id);

        viewModel.PolicySearchKeyword = "keyword";
        viewModel.ApplyPolicySearch();

        Assert.Empty(viewModel.PolicySearchResults);
        Assert.True(viewModel.IsPolicySearchFilterEmpty);
        Assert.Equal("filter-empty", viewModel.PolicySearchMessage);
    }

    [Fact]
    public async Task Five_filters_are_combined_with_and_and_excluded_selection_is_cleared()
    {
        var familyStore = new StubFamilyMemberStorage(
        [
            CreateFamily("family_a", "가족 A"),
            CreateFamily("family_b", "가족 B")
        ]);
        var policyStore = new StubPolicyStorage(
        [
            CreatePolicy("policy_a", "보험 계약 Alpha", "family_a", "보험사 A"),
            CreatePolicy("policy_b", "보험 계약 Beta", "family_b", "보험사 B")
        ]);
        var viewModel = CreateViewModel(policyStore, familyStore);
        await viewModel.LoadPolicySearchAsync();
        viewModel.SelectedPolicyId = "policy_b";
        viewModel.SelectedPolicySearchFamily = Option(viewModel.PolicySearchFamilyOptions, "family_a");
        viewModel.SelectedPolicySearchInsurer = Option(viewModel.PolicySearchInsurerOptions, "보험사 A");
        viewModel.SelectedPolicySearchContractStatus = Option(
            viewModel.PolicySearchContractStatusOptions,
            InsurancePolicyValues.ContractStatusActive);
        viewModel.SelectedPolicySearchProductCategory = Option(
            viewModel.PolicySearchProductCategoryOptions,
            InsurancePolicyValues.ProductCategoryCancer);
        viewModel.PolicySearchKeyword = "alpha";

        viewModel.ApplyPolicySearch();

        Assert.Equal("policy_a", Assert.Single(viewModel.PolicySearchResults).Id);
        Assert.Null(viewModel.SelectedPolicyId);
    }

    [Fact]
    public async Task Apply_without_filters_and_reset_restore_all_active_results_without_writes()
    {
        var familyStore = new StubFamilyMemberStorage([CreateFamily("family_a", "가족 A")]);
        var policyStore = new StubPolicyStorage(
        [
            CreatePolicy("policy_a", "보험 계약 A", "family_a", "보험사 A"),
            CreatePolicy("policy_b", "보험 계약 B", "family_a", "보험사 B")
        ]);
        var viewModel = CreateViewModel(policyStore, familyStore);
        await viewModel.LoadPolicySearchAsync();

        viewModel.ApplyPolicySearch();
        Assert.Equal(2, viewModel.PolicySearchResults.Count);

        viewModel.SelectedPolicyId = "policy_a";
        viewModel.SelectedPolicySearchInsurer = Option(
            viewModel.PolicySearchInsurerOptions,
            "보험사 B");
        viewModel.PolicySearchKeyword = "B";
        viewModel.ApplyPolicySearch();
        Assert.Equal("policy_b", Assert.Single(viewModel.PolicySearchResults).Id);
        Assert.Null(viewModel.SelectedPolicyId);

        viewModel.ResetPolicySearchFilters();

        Assert.Equal(2, viewModel.PolicySearchResults.Count);
        Assert.Equal(string.Empty, viewModel.PolicySearchKeyword);
        Assert.All(
            new[]
            {
                viewModel.SelectedPolicySearchFamily,
                viewModel.SelectedPolicySearchInsurer,
                viewModel.SelectedPolicySearchContractStatus,
                viewModel.SelectedPolicySearchProductCategory
            },
            option => Assert.Null(option?.Value));
        Assert.Equal(0, policyStore.WriteCount);
        Assert.Equal(0, familyStore.WriteCount);
    }

    [Fact]
    public async Task Source_empty_and_filter_empty_have_distinct_state_and_messages()
    {
        var emptyViewModel = CreateViewModel(
            new StubPolicyStorage([]),
            new StubFamilyMemberStorage([]));

        Assert.True(await emptyViewModel.LoadPolicySearchAsync());
        Assert.False(emptyViewModel.HasPolicySearchSource);
        Assert.False(emptyViewModel.IsPolicySearchFilterEmpty);
        Assert.Equal("source-empty", emptyViewModel.PolicySearchMessage);

        var populatedViewModel = CreateViewModel(
            new StubPolicyStorage(
                [CreatePolicy("policy_a", "보험 계약 A", "family_a", "보험사 A")]),
            new StubFamilyMemberStorage([CreateFamily("family_a", "가족 A")]));
        await populatedViewModel.LoadPolicySearchAsync();
        populatedViewModel.PolicySearchKeyword = "없는 조건";
        populatedViewModel.ApplyPolicySearch();

        Assert.True(populatedViewModel.HasPolicySearchSource);
        Assert.True(populatedViewModel.IsPolicySearchFilterEmpty);
        Assert.Equal("filter-empty", populatedViewModel.PolicySearchMessage);
    }

    [Fact]
    public async Task Reload_failure_clears_previous_projection_and_exposes_only_safe_message()
    {
        var familyStore = new StubFamilyMemberStorage([CreateFamily("family_a", "가족 A")]);
        var policyStore = new StubPolicyStorage(
            [CreatePolicy("policy_sensitive", "보험 계약 A", "family_a", "보험사 A")]);
        var viewModel = CreateViewModel(policyStore, familyStore);
        await viewModel.LoadPolicySearchAsync();
        viewModel.SelectedPolicyId = "policy_sensitive";
        viewModel.PolicySearchKeyword = "stale";
        policyStore.ReadException = new InvalidOperationException(
            "C:\\private\\policies.json { json } policy_sensitive");

        Assert.False(await viewModel.LoadPolicySearchAsync());

        Assert.Empty(viewModel.PolicySearchResults);
        Assert.Empty(viewModel.PolicySearchFamilyOptions);
        Assert.Empty(viewModel.PolicySearchInsurerOptions);
        Assert.Empty(viewModel.PolicySearchContractStatusOptions);
        Assert.Empty(viewModel.PolicySearchProductCategoryOptions);
        Assert.Null(viewModel.SelectedPolicySearchFamily);
        Assert.Null(viewModel.SelectedPolicySearchInsurer);
        Assert.Null(viewModel.SelectedPolicySearchContractStatus);
        Assert.Null(viewModel.SelectedPolicySearchProductCategory);
        Assert.Equal(string.Empty, viewModel.PolicySearchKeyword);
        Assert.Null(viewModel.SelectedPolicyId);
        Assert.False(viewModel.HasLoadedPolicySearchProjection);
        Assert.Equal("safe-load-failed", viewModel.PolicySearchMessage);
        Assert.DoesNotContain("private", viewModel.PolicySearchMessage, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("json", viewModel.PolicySearchMessage, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("policy_sensitive", viewModel.PolicySearchMessage, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Cancellation_is_rethrown_and_not_converted_to_load_failure()
    {
        var policyStore = new StubPolicyStorage([])
        {
            ReadException = new OperationCanceledException()
        };
        var viewModel = CreateViewModel(policyStore, new StubFamilyMemberStorage([]));

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => viewModel.LoadPolicySearchAsync());
    }

    private static PolicyClaimManagementViewModel CreateViewModel(
        IPolicyClaimStorageService policyStore,
        IFamilyMemberStorageService familyStore)
    {
        return new PolicyClaimManagementViewModel(
            policyStore,
            familyStore,
            new DictionaryUiTextProvider(new Dictionary<string, string>(StringComparer.Ordinal)
            {
                [UiTextKeys.ProductInsurancePolicyDocumentCreateStatus] = "document-status",
                [UiTextKeys.ProductInsurancePolicyFamilyUnavailableValue] = "family-unavailable",
                [UiTextKeys.ProductInsurancePolicyUnregisteredValue] = "unregistered",
                [UiTextKeys.ProductInsurancePolicyLegacyValueReviewRequired] = "legacy-review",
                [UiTextKeys.ProductPolicyContractsEmptyMessage] = "source-empty",
                [UiTextKeys.ProductPolicySearchFilterEmptyMessage] = "filter-empty",
                [UiTextKeys.ProductInsurancePolicyLoadFailedMessage] = "safe-load-failed",
                [UiTextKeys.ProductHistoryAllOption] = "전체"
            }));
    }

    private static PolicySearchFilterOptionViewModel Option(
        IReadOnlyList<PolicySearchFilterOptionViewModel> options,
        string value)
    {
        return Assert.Single(options, option => string.Equals(
            option.Value,
            value,
            StringComparison.Ordinal));
    }

    private static FamilyMemberRecord CreateFamily(string id, string displayName)
    {
        var timestamp = new DateTimeOffset(2026, 8, 11, 0, 0, 0, TimeSpan.Zero);
        return new FamilyMemberRecord(
            id,
            displayName,
            FamilyMemberRelationValues.Self,
            null,
            timestamp,
            timestamp,
            null,
            1);
    }

    private static PolicyRecord CreatePolicy(
        string id,
        string displayTitle,
        string familyMemberId,
        string insurerName,
        string contractStatus = InsurancePolicyValues.ContractStatusActive,
        string productCategory = InsurancePolicyValues.ProductCategoryCancer,
        bool disabled = false)
    {
        var timestamp = new DateTimeOffset(2026, 8, 11, 0, 0, 0, TimeSpan.Zero);
        return new PolicyRecord(
            id,
            displayTitle,
            new DateOnly(2026, 8, 11),
            timestamp,
            timestamp,
            disabled ? timestamp : null,
            familyMemberId,
            insurerName,
            contractStatus,
            new DateOnly(2026, 8, 11),
            "10년",
            InsurancePolicyValues.RegistrationSourceDirectInput,
            "10년납",
            1_000_000m,
            InsurancePolicyValues.RenewalTypeFixed,
            InsurancePolicyValues.RefundTypeRefundable,
            InsurancePolicyValues.BusinessTypeLife,
            productCategory);
    }

    private sealed class StubPolicyStorage : IPolicyClaimStorageService
    {
        private readonly IReadOnlyList<PolicyRecord> policies;

        public StubPolicyStorage(IReadOnlyList<PolicyRecord> policies)
        {
            this.policies = policies;
        }

        public Exception? ReadException { get; set; }

        public int WriteCount { get; private set; }

        public Task<IReadOnlyList<PolicyRecord>> GetPoliciesAsync(
            CancellationToken cancellationToken = default)
        {
            if (ReadException is not null)
            {
                throw ReadException;
            }

            return Task.FromResult(policies);
        }

        public Task<PolicyRecord?> GetPolicyAsync(string id, CancellationToken cancellationToken = default) =>
            Task.FromResult(policies.FirstOrDefault(policy => policy.Id == id));

        public Task<PolicyRecord> AddPolicyAsync(PolicyDraft draft, CancellationToken cancellationToken = default) =>
            Write<PolicyRecord>();

        public Task<PolicyRecord> CreateInsurancePolicyAsync(InsurancePolicyDraft draft, CancellationToken cancellationToken = default) =>
            Write<PolicyRecord>();

        public Task<PolicyRecord> UpdateInsurancePolicyAsync(string id, InsurancePolicyDraft draft, CancellationToken cancellationToken = default) =>
            Write<PolicyRecord>();

        public Task<PolicyRecord> DisablePolicyAsync(string id, CancellationToken cancellationToken = default) =>
            Write<PolicyRecord>();

        public Task<IReadOnlyList<ClaimRecord>> GetClaimsAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<ClaimRecord>>([]);

        public Task<IReadOnlyList<ClaimRecord>> GetClaimsByPolicyIdAsync(string policyId, CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<ClaimRecord>>([]);

        public Task<ClaimRecord?> GetClaimAsync(string id, CancellationToken cancellationToken = default) =>
            Task.FromResult<ClaimRecord?>(null);

        public Task<ClaimRecord> AddClaimAsync(ClaimDraft draft, CancellationToken cancellationToken = default) =>
            Write<ClaimRecord>();

        public Task<ClaimRecord> DisableClaimAsync(string id, int expectedRevision, CancellationToken cancellationToken = default) =>
            Write<ClaimRecord>();

        public Task<bool> PolicyExistsAsync(string id, CancellationToken cancellationToken = default) =>
            Task.FromResult(policies.Any(policy => policy.Id == id));

        public Task<bool> ClaimExistsAsync(string id, CancellationToken cancellationToken = default) =>
            Task.FromResult(false);

        private Task<T> Write<T>()
        {
            WriteCount++;
            throw new InvalidOperationException("Search must not write.");
        }
    }

    private sealed class StubFamilyMemberStorage : IFamilyMemberStorageService
    {
        private readonly IReadOnlyList<FamilyMemberRecord> familyMembers;

        public StubFamilyMemberStorage(IReadOnlyList<FamilyMemberRecord> familyMembers)
        {
            this.familyMembers = familyMembers;
        }

        public int WriteCount { get; private set; }

        public Task<IReadOnlyList<FamilyMemberRecord>> GetFamilyMembersAsync(
            CancellationToken cancellationToken = default) =>
            Task.FromResult(familyMembers);

        public Task<IReadOnlyList<FamilyMemberRecord>> GetActiveFamilyMembersAsync(
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<FamilyMemberRecord>>(
                familyMembers.Where(member => member.DisabledAt is null).ToList());

        public Task<FamilyMemberRecord?> GetFamilyMemberAsync(string id, CancellationToken cancellationToken = default) =>
            Task.FromResult(familyMembers.FirstOrDefault(member => member.Id == id));

        public Task<FamilyMemberRecord> CreateFamilyMemberAsync(FamilyMemberDraft draft, CancellationToken cancellationToken = default) =>
            Write<FamilyMemberRecord>();

        public Task<FamilyMemberRecord> UpdateFamilyMemberAsync(string id, int expectedVersion, FamilyMemberDraft draft, CancellationToken cancellationToken = default) =>
            Write<FamilyMemberRecord>();

        public Task<FamilyMemberRecord> DeactivateFamilyMemberAsync(string id, int expectedVersion, CancellationToken cancellationToken = default) =>
            Write<FamilyMemberRecord>();

        public Task<FamilyMemberRecord> ReactivateFamilyMemberAsync(string id, int expectedVersion, CancellationToken cancellationToken = default) =>
            Write<FamilyMemberRecord>();

        private Task<T> Write<T>()
        {
            WriteCount++;
            throw new InvalidOperationException("Search must not write.");
        }
    }

    private sealed class DictionaryUiTextProvider : IUiTextProvider
    {
        private readonly IReadOnlyDictionary<string, string> values;

        public DictionaryUiTextProvider(IReadOnlyDictionary<string, string> values)
        {
            this.values = values;
        }

        public string Get(string key) => values[key];

        public string Format(string key, params object?[] args) =>
            string.Format(values[key], args);
    }
}
