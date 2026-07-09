using System.ComponentModel;
using System.Runtime.CompilerServices;
using FamilyClaimRef.App.Models.Storage;
using FamilyClaimRef.App.Services.Localization;
using FamilyClaimRef.App.Services.Storage;

namespace FamilyClaimRef.App.ViewModels;

public sealed class PolicyClaimManagementViewModel : INotifyPropertyChanged
{
    private readonly IPolicyClaimStorageService policyClaimStorageService;
    private readonly IUiTextProvider uiTextProvider;

    private IReadOnlyList<PolicyRecord> availablePolicies = [];
    private IReadOnlyList<ClaimRecord> availableClaims = [];
    private string? selectedPolicyId;
    private string? selectedClaimId;
    private string? selectedPolicyForClaimId;
    private string? newPolicyDisplayTitle;
    private string? newClaimDisplayTitle;
    private string? managementMessage;

    public PolicyClaimManagementViewModel(
        IPolicyClaimStorageService policyClaimStorageService,
        IUiTextProvider uiTextProvider)
    {
        this.policyClaimStorageService = policyClaimStorageService
            ?? throw new ArgumentNullException(nameof(policyClaimStorageService));
        this.uiTextProvider = uiTextProvider
            ?? throw new ArgumentNullException(nameof(uiTextProvider));
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public IReadOnlyList<PolicyRecord> AvailablePolicies
    {
        get => availablePolicies;
        private set
        {
            if (SetProperty(ref availablePolicies, value))
            {
                OnPropertyChanged(nameof(HasAvailablePolicies));
                OnPropertyChanged(nameof(CanCreateClaim));
            }
        }
    }

    public IReadOnlyList<ClaimRecord> AvailableClaims
    {
        get => availableClaims;
        private set
        {
            if (SetProperty(ref availableClaims, value))
            {
                OnPropertyChanged(nameof(HasAvailableClaims));
            }
        }
    }

    public string? SelectedPolicyId
    {
        get => selectedPolicyId;
        set
        {
            if (SetProperty(ref selectedPolicyId, value))
            {
                OnPropertyChanged(nameof(CanDisablePolicy));
            }
        }
    }

    public string? SelectedClaimId
    {
        get => selectedClaimId;
        set
        {
            if (SetProperty(ref selectedClaimId, value))
            {
                OnPropertyChanged(nameof(CanDisableClaim));
            }
        }
    }

    public string? SelectedPolicyForClaimId
    {
        get => selectedPolicyForClaimId;
        set
        {
            if (SetProperty(ref selectedPolicyForClaimId, value))
            {
                OnPropertyChanged(nameof(CanCreateClaim));
            }
        }
    }

    public string? NewPolicyDisplayTitle
    {
        get => newPolicyDisplayTitle;
        set
        {
            if (SetProperty(ref newPolicyDisplayTitle, value))
            {
                OnPropertyChanged(nameof(CanCreatePolicy));
            }
        }
    }

    public string? NewClaimDisplayTitle
    {
        get => newClaimDisplayTitle;
        set
        {
            if (SetProperty(ref newClaimDisplayTitle, value))
            {
                OnPropertyChanged(nameof(CanCreateClaim));
            }
        }
    }

    public string? ManagementMessage
    {
        get => managementMessage;
        private set => SetProperty(ref managementMessage, value);
    }

    public bool HasAvailablePolicies => AvailablePolicies.Count > 0;

    public bool HasAvailableClaims => AvailableClaims.Count > 0;

    public bool CanCreatePolicy => !string.IsNullOrWhiteSpace(NewPolicyDisplayTitle);

    public bool CanCreateClaim =>
        !string.IsNullOrWhiteSpace(NewClaimDisplayTitle)
        && !string.IsNullOrWhiteSpace(SelectedPolicyForClaimId);

    public bool CanDisablePolicy => !string.IsNullOrWhiteSpace(SelectedPolicyId);

    public bool CanDisableClaim => !string.IsNullOrWhiteSpace(SelectedClaimId);

    public async Task LoadAsync(CancellationToken cancellationToken = default)
    {
        var policies = await policyClaimStorageService.GetPoliciesAsync(cancellationToken);
        var claims = await policyClaimStorageService.GetClaimsAsync(cancellationToken);

        AvailablePolicies = policies.ToList();
        AvailableClaims = claims.ToList();

        if (!AvailablePolicies.Any(policy => string.Equals(policy.Id, SelectedPolicyId, StringComparison.Ordinal)))
        {
            SelectedPolicyId = null;
        }

        if (!AvailableClaims.Any(claim => string.Equals(claim.Id, SelectedClaimId, StringComparison.Ordinal)))
        {
            SelectedClaimId = null;
        }

        if (!AvailablePolicies.Any(policy => string.Equals(policy.Id, SelectedPolicyForClaimId, StringComparison.Ordinal)))
        {
            SelectedPolicyForClaimId = AvailablePolicies.FirstOrDefault()?.Id;
        }
    }

    public async Task<bool> CreatePolicyAsync(CancellationToken cancellationToken = default)
    {
        var title = NormalizeOptionalTitle(NewPolicyDisplayTitle);
        if (title is null)
        {
            ManagementMessage = uiTextProvider.Get(UiTextKeys.PolicyManagementValidationTitleRequired);
            return false;
        }

        var policy = await policyClaimStorageService.AddPolicyAsync(
            new PolicyDraft(title, DateOnly.FromDateTime(DateTime.Today)),
            cancellationToken);

        NewPolicyDisplayTitle = null;
        await LoadAsync(cancellationToken);
        SelectedPolicyId = policy.Id;
        SelectedPolicyForClaimId = policy.Id;
        ManagementMessage = uiTextProvider.Get(UiTextKeys.PolicyManagementMessageCreated);

        return true;
    }

    public async Task<bool> CreateClaimAsync(CancellationToken cancellationToken = default)
    {
        var title = NormalizeOptionalTitle(NewClaimDisplayTitle);
        if (title is null)
        {
            ManagementMessage = uiTextProvider.Get(UiTextKeys.ClaimManagementValidationTitleRequired);
            return false;
        }

        if (string.IsNullOrWhiteSpace(SelectedPolicyForClaimId)
            || !AvailablePolicies.Any(policy => string.Equals(policy.Id, SelectedPolicyForClaimId, StringComparison.Ordinal)))
        {
            ManagementMessage = uiTextProvider.Get(UiTextKeys.ClaimManagementValidationSelectPolicyBeforeCreate);
            return false;
        }

        var claim = await policyClaimStorageService.AddClaimAsync(
            new ClaimDraft(
                SelectedPolicyForClaimId,
                title,
                DateOnly.FromDateTime(DateTime.Today)),
            cancellationToken);

        NewClaimDisplayTitle = null;
        await LoadAsync(cancellationToken);
        SelectedClaimId = claim.Id;
        ManagementMessage = uiTextProvider.Get(UiTextKeys.ClaimManagementMessageCreated);

        return true;
    }

    public async Task<bool> DisableSelectedPolicyAsync(CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(SelectedPolicyId))
        {
            ManagementMessage = uiTextProvider.Get(UiTextKeys.PolicyManagementValidationSelectPolicyTarget);
            return false;
        }

        var activeClaims = await policyClaimStorageService.GetClaimsByPolicyIdAsync(
            SelectedPolicyId,
            cancellationToken);
        if (activeClaims.Count > 0)
        {
            ManagementMessage = uiTextProvider.Get(UiTextKeys.PolicyManagementValidationDisableBlockedByActiveClaims);
            return false;
        }

        await policyClaimStorageService.DisablePolicyAsync(SelectedPolicyId, cancellationToken);
        await LoadAsync(cancellationToken);
        ManagementMessage = uiTextProvider.Get(UiTextKeys.PolicyManagementMessageDisabled);

        return true;
    }

    public async Task<bool> DisableSelectedClaimAsync(CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(SelectedClaimId))
        {
            ManagementMessage = uiTextProvider.Get(UiTextKeys.ClaimManagementValidationSelectClaimTarget);
            return false;
        }

        await policyClaimStorageService.DisableClaimAsync(SelectedClaimId, cancellationToken);
        await LoadAsync(cancellationToken);
        ManagementMessage = uiTextProvider.Get(UiTextKeys.ClaimManagementMessageDisabled);

        return true;
    }

    private static string? NormalizeOptionalTitle(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        return value.Trim();
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
}
