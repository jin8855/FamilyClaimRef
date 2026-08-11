using FamilyClaimRef.App.Models.Storage;
using FamilyClaimRef.App.Services.Localization;

namespace FamilyClaimRef.App.ViewModels;

public sealed partial class PolicyClaimManagementViewModel
{
    private IReadOnlyList<InsurancePolicyListItemViewModel> policySearchSource = [];
    private IReadOnlyList<InsurancePolicyListItemViewModel> policySearchResults = [];
    private IReadOnlyList<PolicySearchFilterOptionViewModel> policySearchFamilyOptions = [];
    private IReadOnlyList<PolicySearchFilterOptionViewModel> policySearchInsurerOptions = [];
    private IReadOnlyList<PolicySearchFilterOptionViewModel> policySearchContractStatusOptions = [];
    private IReadOnlyList<PolicySearchFilterOptionViewModel> policySearchProductCategoryOptions = [];
    private PolicySearchFilterOptionViewModel? selectedPolicySearchFamily;
    private PolicySearchFilterOptionViewModel? selectedPolicySearchInsurer;
    private PolicySearchFilterOptionViewModel? selectedPolicySearchContractStatus;
    private PolicySearchFilterOptionViewModel? selectedPolicySearchProductCategory;
    private string policySearchKeyword = string.Empty;
    private string? policySearchMessage;
    private bool hasLoadedPolicySearchProjection;

    public IReadOnlyList<InsurancePolicyListItemViewModel> PolicySearchResults
    {
        get => policySearchResults;
        private set
        {
            if (SetProperty(ref policySearchResults, value))
            {
                OnPropertyChanged(nameof(HasPolicySearchResults));
                OnPropertyChanged(nameof(IsPolicySearchFilterEmpty));
            }
        }
    }

    public IReadOnlyList<PolicySearchFilterOptionViewModel> PolicySearchFamilyOptions
    {
        get => policySearchFamilyOptions;
        private set => SetProperty(ref policySearchFamilyOptions, value);
    }

    public IReadOnlyList<PolicySearchFilterOptionViewModel> PolicySearchInsurerOptions
    {
        get => policySearchInsurerOptions;
        private set => SetProperty(ref policySearchInsurerOptions, value);
    }

    public IReadOnlyList<PolicySearchFilterOptionViewModel> PolicySearchContractStatusOptions
    {
        get => policySearchContractStatusOptions;
        private set => SetProperty(ref policySearchContractStatusOptions, value);
    }

    public IReadOnlyList<PolicySearchFilterOptionViewModel> PolicySearchProductCategoryOptions
    {
        get => policySearchProductCategoryOptions;
        private set => SetProperty(ref policySearchProductCategoryOptions, value);
    }

    public PolicySearchFilterOptionViewModel? SelectedPolicySearchFamily
    {
        get => selectedPolicySearchFamily;
        set => SetProperty(ref selectedPolicySearchFamily, value);
    }

    public PolicySearchFilterOptionViewModel? SelectedPolicySearchInsurer
    {
        get => selectedPolicySearchInsurer;
        set => SetProperty(ref selectedPolicySearchInsurer, value);
    }

    public PolicySearchFilterOptionViewModel? SelectedPolicySearchContractStatus
    {
        get => selectedPolicySearchContractStatus;
        set => SetProperty(ref selectedPolicySearchContractStatus, value);
    }

    public PolicySearchFilterOptionViewModel? SelectedPolicySearchProductCategory
    {
        get => selectedPolicySearchProductCategory;
        set => SetProperty(ref selectedPolicySearchProductCategory, value);
    }

    public string PolicySearchKeyword
    {
        get => policySearchKeyword;
        set => SetProperty(ref policySearchKeyword, value ?? string.Empty);
    }

    public string? PolicySearchMessage
    {
        get => policySearchMessage;
        private set => SetProperty(ref policySearchMessage, value);
    }

    public bool HasLoadedPolicySearchProjection
    {
        get => hasLoadedPolicySearchProjection;
        private set
        {
            if (SetProperty(ref hasLoadedPolicySearchProjection, value))
            {
                OnPropertyChanged(nameof(IsPolicySearchFilterEmpty));
            }
        }
    }

    public bool HasPolicySearchResults => PolicySearchResults.Count > 0;

    public bool HasPolicySearchSource => policySearchSource.Count > 0;

    public bool IsPolicySearchFilterEmpty =>
        HasLoadedPolicySearchProjection
        && HasPolicySearchSource
        && !HasPolicySearchResults;

    public async Task<bool> LoadPolicySearchAsync(
        CancellationToken cancellationToken = default)
    {
        if (familyMemberStorageService is null)
        {
            ResetPolicySearchAfterFailure();
            PolicySearchMessage = uiTextProvider.Get(
                UiTextKeys.ProductInsurancePolicyLoadFailedMessage);
            return false;
        }

        await operationGate.WaitAsync(cancellationToken);
        try
        {
            var policies = await policyClaimStorageService.GetPoliciesAsync(cancellationToken);
            var familyMembers = await familyMemberStorageService.GetFamilyMembersAsync(
                cancellationToken);
            var activePolicies = policies
                .Where(policy => policy.DisabledAt is null)
                .ToList();
            var familyMembersById = familyMembers.ToDictionary(
                member => member.Id,
                StringComparer.Ordinal);

            AvailablePolicies = activePolicies;
            AvailablePolicyFamilyMembers = familyMembers
                .Where(member => member.DisabledAt is null)
                .ToList();
            AvailableInsurancePolicies = activePolicies
                .Select(policy => new InsurancePolicyListItemViewModel(
                    policy,
                    policy.FamilyMemberId is not null
                    && familyMembersById.TryGetValue(policy.FamilyMemberId, out var familyMember)
                        ? familyMember.DisplayName
                        : uiTextProvider.Get(
                            UiTextKeys.ProductInsurancePolicyFamilyUnavailableValue),
                    uiTextProvider.Get(UiTextKeys.ProductInsurancePolicyUnregisteredValue),
                    uiTextProvider.Get(
                        UiTextKeys.ProductInsurancePolicyLegacyValueReviewRequired)))
                .ToList();

            policySearchSource = AvailableInsurancePolicies;
            OnPropertyChanged(nameof(HasPolicySearchSource));
            SetPolicySearchOptions();
            ResetPolicySearchFiltersCore();
            HasLoadedPolicySearchProjection = true;
            PolicySearchMessage = HasPolicySearchSource
                ? null
                : uiTextProvider.Get(UiTextKeys.ProductPolicyContractsEmptyMessage);
            return true;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch
        {
            ResetPolicySearchAfterFailure();
            PolicySearchMessage = uiTextProvider.Get(
                UiTextKeys.ProductInsurancePolicyLoadFailedMessage);
            return false;
        }
        finally
        {
            operationGate.Release();
        }
    }

    public void ApplyPolicySearch()
    {
        var keyword = PolicySearchKeyword.Trim();
        PolicySearchKeyword = keyword;
        PolicySearchResults = policySearchSource
            .Where(item => MatchesOption(
                item.Policy.FamilyMemberId,
                SelectedPolicySearchFamily))
            .Where(item => MatchesOption(
                item.Policy.InsurerName,
                SelectedPolicySearchInsurer))
            .Where(item => MatchesOption(
                item.ContractStatus,
                SelectedPolicySearchContractStatus))
            .Where(item => MatchesOption(
                item.ProductCategory,
                SelectedPolicySearchProductCategory))
            .Where(item => keyword.Length == 0
                || item.DisplayTitle.Contains(keyword, StringComparison.OrdinalIgnoreCase))
            .ToList();

        ClearPolicySearchSelectionOutsideResults();
        PolicySearchMessage = !HasPolicySearchSource
            ? uiTextProvider.Get(UiTextKeys.ProductPolicyContractsEmptyMessage)
            : HasPolicySearchResults
                ? null
                : uiTextProvider.Get(UiTextKeys.ProductPolicySearchFilterEmptyMessage);
    }

    public void ResetPolicySearchFilters()
    {
        ResetPolicySearchFiltersCore();
        PolicySearchMessage = HasPolicySearchSource
            ? null
            : uiTextProvider.Get(UiTextKeys.ProductPolicyContractsEmptyMessage);
    }

    private void SetPolicySearchOptions()
    {
        PolicySearchFamilyOptions = CreatePolicySearchOptions(
            policySearchSource.Select(item => (
                Value: item.Policy.FamilyMemberId,
                DisplayName: (string?)item.FamilyDisplayName)));
        PolicySearchInsurerOptions = CreatePolicySearchOptions(
            policySearchSource.Select(item => (
                item.Policy.InsurerName,
                item.InsurerName)));
        PolicySearchContractStatusOptions = CreatePolicySearchOptions(
            policySearchSource.Select(item => (
                (string?)item.ContractStatus,
                (string?)item.ContractStatus)));
        PolicySearchProductCategoryOptions = CreatePolicySearchOptions(
            policySearchSource.Select(item => (
                (string?)item.ProductCategory,
                (string?)item.ProductCategory)));
    }

    private IReadOnlyList<PolicySearchFilterOptionViewModel> CreatePolicySearchOptions(
        IEnumerable<(string? Value, string? DisplayName)> values)
    {
        var options = values
            .Where(value => !string.IsNullOrWhiteSpace(value.Value)
                && !string.IsNullOrWhiteSpace(value.DisplayName))
            .GroupBy(value => value.Value!, StringComparer.Ordinal)
            .Select(group => new PolicySearchFilterOptionViewModel(
                group.Key,
                group.First().DisplayName!))
            .OrderBy(option => option.DisplayName, StringComparer.Ordinal)
            .ThenBy(option => option.Value, StringComparer.Ordinal)
            .ToList();

        return
        [
            new PolicySearchFilterOptionViewModel(
                null,
                uiTextProvider.Get(UiTextKeys.ProductHistoryAllOption)),
            .. options
        ];
    }

    private void ResetPolicySearchFiltersCore()
    {
        SelectedPolicySearchFamily = PolicySearchFamilyOptions.FirstOrDefault();
        SelectedPolicySearchInsurer = PolicySearchInsurerOptions.FirstOrDefault();
        SelectedPolicySearchContractStatus =
            PolicySearchContractStatusOptions.FirstOrDefault();
        SelectedPolicySearchProductCategory =
            PolicySearchProductCategoryOptions.FirstOrDefault();
        PolicySearchKeyword = string.Empty;
        PolicySearchResults = policySearchSource.ToList();
        if (!policySearchSource.Any(item => string.Equals(
                item.Id,
                SelectedPolicyId,
                StringComparison.Ordinal)))
        {
            SelectedPolicyId = null;
        }
    }

    private void ResetPolicySearchAfterFailure()
    {
        policySearchSource = [];
        AvailablePolicies = [];
        AvailableInsurancePolicies = [];
        AvailablePolicyFamilyMembers = [];
        PolicySearchResults = [];
        PolicySearchFamilyOptions = [];
        PolicySearchInsurerOptions = [];
        PolicySearchContractStatusOptions = [];
        PolicySearchProductCategoryOptions = [];
        SelectedPolicySearchFamily = null;
        SelectedPolicySearchInsurer = null;
        SelectedPolicySearchContractStatus = null;
        SelectedPolicySearchProductCategory = null;
        PolicySearchKeyword = string.Empty;
        SelectedPolicyId = null;
        HasLoadedPolicySearchProjection = false;
        OnPropertyChanged(nameof(HasPolicySearchSource));
    }

    private void ClearPolicySearchSelectionOutsideResults()
    {
        if (!PolicySearchResults.Any(item => string.Equals(
                item.Id,
                SelectedPolicyId,
                StringComparison.Ordinal)))
        {
            SelectedPolicyId = null;
        }
    }

    private static bool MatchesOption(
        string? value,
        PolicySearchFilterOptionViewModel? option)
    {
        return option?.Value is null
            || string.Equals(value, option.Value, StringComparison.Ordinal);
    }
}
