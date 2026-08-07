using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using FamilyClaimRef.App.Services.Localization;

namespace FamilyClaimRef.App.ViewModels;

public sealed class ProductShellViewModel : INotifyPropertyChanged
{
    private ProductNavigationItemViewModel selectedNavigationItem;
    private ProductScreenViewModel currentScreen;
    private bool isSynchronizingNavigation;
    private readonly string emptyDisplayValue;
    private readonly string claimContextInputState;
    private readonly string claimContextConfirmationState;

    public ProductShellViewModel(
        IUiTextProvider uiTextProvider,
        DocumentRegistrationViewModel documentRegistration,
        ProductDocumentListViewModel documentList,
        PolicyClaimManagementViewModel policyClaimManagement,
        FamilyMemberManagementViewModel familyMemberManagement)
    {
        ArgumentNullException.ThrowIfNull(uiTextProvider);
        ArgumentNullException.ThrowIfNull(documentRegistration);
        ArgumentNullException.ThrowIfNull(documentList);
        ArgumentNullException.ThrowIfNull(policyClaimManagement);
        ArgumentNullException.ThrowIfNull(familyMemberManagement);

        ShellTitle = uiTextProvider.Get(UiTextKeys.ProductShellTitle);
        emptyDisplayValue = uiTextProvider.Get(ProductScreenTextKeys.EmptyValue);
        claimContextInputState = uiTextProvider.Get(ProductScreenTextKeys.ClaimContextInputValue);
        claimContextConfirmationState =
            uiTextProvider.Get(ProductScreenTextKeys.ClaimContextConfirmationValue);
        DocumentRegistration = documentRegistration;
        DocumentList = documentList;
        PolicyClaimManagement = policyClaimManagement;
        FamilyMemberManagement = familyMemberManagement;
        NavigationItems = Array.AsReadOnly(
        [
            new ProductNavigationItemViewModel(
                "Home",
                uiTextProvider.Get(UiTextKeys.ProductNavigationHome)),
            new ProductNavigationItemViewModel(
                "PolicyContracts",
                uiTextProvider.Get(UiTextKeys.ProductNavigationPolicyContracts)),
            new ProductNavigationItemViewModel(
                "ClaimCases",
                uiTextProvider.Get(UiTextKeys.ProductNavigationClaimCases)),
            new ProductNavigationItemViewModel(
                "DocumentRegistration",
                uiTextProvider.Get(UiTextKeys.ProductNavigationDocumentRegistration)),
            new ProductNavigationItemViewModel(
                "DocumentList",
                uiTextProvider.Get(UiTextKeys.ProductNavigationDocumentList))
        ]);
        selectedNavigationItem = NavigationItems[0];

        Screens = Array.AsReadOnly(ProductScreenCatalog.Create(uiTextProvider).ToArray());
        ScreensById = Screens.ToDictionary(screen => screen.Id, StringComparer.Ordinal);
        currentScreen = ScreensById[ProductScreenRoutes.HomeDashboard];
        NavigateCommand = new ProductRouteCommand(NavigateTo, ScreensById.ContainsKey);
        PolicyClaimManagement.PropertyChanged += OnPolicyClaimManagementPropertyChanged;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public string ShellTitle { get; }

    public DocumentRegistrationViewModel DocumentRegistration { get; }

    public ProductDocumentListViewModel DocumentList { get; }

    public PolicyClaimManagementViewModel PolicyClaimManagement { get; }

    public FamilyMemberManagementViewModel FamilyMemberManagement { get; }

    public ReadOnlyCollection<ProductNavigationItemViewModel> NavigationItems { get; }

    public ReadOnlyCollection<ProductScreenViewModel> Screens { get; }

    public ICommand NavigateCommand { get; }

    public string ClaimContextPolicyDisplayTitle =>
        PolicyClaimManagement.AvailablePolicies
            .FirstOrDefault(policy => string.Equals(
                policy.Id,
                PolicyClaimManagement.SelectedPolicyForClaimId,
                StringComparison.Ordinal))
            ?.DisplayTitle
        ?? emptyDisplayValue;

    public string ClaimContextClaimDisplayTitle =>
        PolicyClaimManagement.AvailableClaims
            .FirstOrDefault(claim => string.Equals(
                claim.Id,
                PolicyClaimManagement.SelectedClaimId,
                StringComparison.Ordinal))
            ?.DisplayTitle
        ?? NormalizeDisplayTitle(PolicyClaimManagement.NewClaimDisplayTitle)
        ?? emptyDisplayValue;

    public string ClaimContextInputState => claimContextInputState;

    public string ClaimContextConfirmationState => claimContextConfirmationState;

    public ProductScreenViewModel CurrentScreen
    {
        get => currentScreen;
        private set
        {
            if (SetProperty(ref currentScreen, value))
            {
                OnPropertyChanged(nameof(CurrentRouteId));
            }
        }
    }

    public string CurrentRouteId
    {
        get => CurrentScreen.Id;
        set
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                NavigateTo(value);
            }
        }
    }

    public ProductNavigationItemViewModel? SelectedNavigationItem
    {
        get => selectedNavigationItem;
        set
        {
            if (value is null)
            {
                return;
            }

            if (!NavigationItems.Contains(value))
            {
                throw new ArgumentException("Selected navigation item must belong to the shell.", nameof(value));
            }

            if (SetProperty(ref selectedNavigationItem, value)
                && !isSynchronizingNavigation)
            {
                NavigateTo(MapNavigationToRoute(value.Id));
            }
        }
    }

    private IReadOnlyDictionary<string, ProductScreenViewModel> ScreensById { get; }

    public void NavigateTo(string routeId)
    {
        if (string.Equals(routeId, ProductScreenRoutes.FamilyRegister, StringComparison.Ordinal))
        {
            FamilyMemberManagement.BeginCreate();
        }

        NavigateCore(routeId);
    }

    public void NavigateToFamilyCreate()
    {
        FamilyMemberManagement.BeginCreate();
        NavigateCore(ProductScreenRoutes.FamilyRegister);
    }

    public async Task<bool> NavigateToFamilyEditAsync(
        string id,
        int version,
        CancellationToken cancellationToken = default)
    {
        if (!await FamilyMemberManagement.PrepareEditAsync(id, version, cancellationToken))
        {
            return false;
        }

        NavigateCore(ProductScreenRoutes.FamilyRegister);
        return true;
    }

    public async Task<bool> SaveFamilyMemberAndReturnAsync(
        CancellationToken cancellationToken = default)
    {
        if (!await FamilyMemberManagement.SaveAsync(cancellationToken))
        {
            return false;
        }

        NavigateCore(ProductScreenRoutes.FamilyMembers);
        return true;
    }

    public async Task<bool> NavigateToInsurancePolicyCreateAsync(
        CancellationToken cancellationToken = default)
    {
        if (!await PolicyClaimManagement.PrepareInsurancePolicyCreateAsync(cancellationToken))
        {
            return false;
        }

        NavigateCore(ProductScreenRoutes.PolicyRegister);
        return true;
    }

    public async Task<bool> NavigateToInsurancePolicyEditAsync(
        string id,
        CancellationToken cancellationToken = default)
    {
        if (!await PolicyClaimManagement.PrepareInsurancePolicyEditAsync(id, cancellationToken))
        {
            return false;
        }

        NavigateCore(ProductScreenRoutes.PolicyRegister);
        return true;
    }

    public bool NavigateToPolicyDocumentRegistration(string documentType)
    {
        if (string.IsNullOrWhiteSpace(documentType)
            || string.IsNullOrWhiteSpace(PolicyClaimManagement.SelectedPolicyId))
        {
            return false;
        }

        DocumentRegistration.TargetKind = DocumentRegistrationViewModel.PolicyTargetKind;
        DocumentRegistration.SelectedPolicyId = PolicyClaimManagement.SelectedPolicyId;
        DocumentRegistration.DocumentType = documentType;
        NavigateCore(ProductScreenRoutes.PolicyDocumentRegister);
        return true;
    }

    public async Task<bool> SaveInsurancePolicyAndReturnAsync(
        CancellationToken cancellationToken = default)
    {
        if (await PolicyClaimManagement.SaveInsurancePolicyAsync(cancellationToken) is null)
        {
            return false;
        }

        NavigateCore(ProductScreenRoutes.PolicyManage);
        return true;
    }

    private void NavigateCore(string routeId)
    {
        if (!ScreensById.TryGetValue(routeId, out var destination))
        {
            throw new ArgumentException("Unknown product screen route.", nameof(routeId));
        }

        ConfigureRegistrationTarget(routeId);
        CurrentScreen = destination;
        SynchronizeLegacyNavigation(routeId);
    }

    private void ConfigureRegistrationTarget(string routeId)
    {
        if (string.Equals(routeId, ProductScreenRoutes.PolicyDocumentRegister, StringComparison.Ordinal))
        {
            DocumentRegistration.TargetKind = DocumentRegistrationViewModel.PolicyTargetKind;
            if (!string.IsNullOrWhiteSpace(PolicyClaimManagement.SelectedPolicyId))
            {
                DocumentRegistration.SelectedPolicyId = PolicyClaimManagement.SelectedPolicyId;
            }

            return;
        }

        if (string.Equals(routeId, ProductScreenRoutes.ClaimDocumentRegister, StringComparison.Ordinal))
        {
            DocumentRegistration.TargetKind = DocumentRegistrationViewModel.ClaimTargetKind;
            if (!string.IsNullOrWhiteSpace(PolicyClaimManagement.SelectedClaimId))
            {
                DocumentRegistration.SelectedClaimId = PolicyClaimManagement.SelectedClaimId;
            }
        }
    }

    private void SynchronizeLegacyNavigation(string routeId)
    {
        var navigationId = routeId switch
        {
            ProductScreenRoutes.PolicyManage
                or ProductScreenRoutes.PolicyList
                or ProductScreenRoutes.PolicyDetail
                or ProductScreenRoutes.PolicyRegister => "PolicyContracts",
            ProductScreenRoutes.ClaimCase
                or ProductScreenRoutes.ClaimSubmission
                or ProductScreenRoutes.ClaimReferenceResult
                or ProductScreenRoutes.ClaimComplete
                or ProductScreenRoutes.OcrReview => "ClaimCases",
            ProductScreenRoutes.PolicyDocumentRegister
                or ProductScreenRoutes.ClaimDocumentRegister => "DocumentRegistration",
            ProductScreenRoutes.DocumentBox => "DocumentList",
            _ => "Home"
        };

        var item = NavigationItems.Single(candidate =>
            string.Equals(candidate.Id, navigationId, StringComparison.Ordinal));

        isSynchronizingNavigation = true;
        try
        {
            SelectedNavigationItem = item;
        }
        finally
        {
            isSynchronizingNavigation = false;
        }
    }

    private static string MapNavigationToRoute(string navigationId)
    {
        return navigationId switch
        {
            "PolicyContracts" => ProductScreenRoutes.PolicyManage,
            "ClaimCases" => ProductScreenRoutes.ClaimCase,
            "DocumentRegistration" => ProductScreenRoutes.PolicyDocumentRegister,
            "DocumentList" => ProductScreenRoutes.DocumentBox,
            _ => ProductScreenRoutes.HomeDashboard
        };
    }

    private void OnPolicyClaimManagementPropertyChanged(
        object? sender,
        PropertyChangedEventArgs eventArgs)
    {
        if (eventArgs.PropertyName is nameof(PolicyClaimManagementViewModel.AvailablePolicies)
            or nameof(PolicyClaimManagementViewModel.SelectedPolicyForClaimId))
        {
            OnPropertyChanged(nameof(ClaimContextPolicyDisplayTitle));
        }

        if (eventArgs.PropertyName is nameof(PolicyClaimManagementViewModel.AvailableClaims)
            or nameof(PolicyClaimManagementViewModel.SelectedClaimId)
            or nameof(PolicyClaimManagementViewModel.NewClaimDisplayTitle))
        {
            OnPropertyChanged(nameof(ClaimContextClaimDisplayTitle));
        }
    }

    private static string? NormalizeDisplayTitle(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
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
