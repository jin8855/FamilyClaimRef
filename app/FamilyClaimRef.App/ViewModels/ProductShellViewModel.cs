using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using FamilyClaimRef.App.Services.Localization;

namespace FamilyClaimRef.App.ViewModels;

public sealed class ProductShellViewModel : INotifyPropertyChanged
{
    private ProductNavigationItemViewModel selectedNavigationItem;

    public ProductShellViewModel(
        IUiTextProvider uiTextProvider,
        DocumentRegistrationViewModel documentRegistration,
        ProductDocumentListViewModel documentList,
        PolicyClaimManagementViewModel policyClaimManagement)
    {
        ArgumentNullException.ThrowIfNull(uiTextProvider);
        ArgumentNullException.ThrowIfNull(documentRegistration);
        ArgumentNullException.ThrowIfNull(documentList);
        ArgumentNullException.ThrowIfNull(policyClaimManagement);

        ShellTitle = uiTextProvider.Get(UiTextKeys.ProductShellTitle);
        DocumentRegistration = documentRegistration;
        DocumentList = documentList;
        PolicyClaimManagement = policyClaimManagement;
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
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public string ShellTitle { get; }

    public DocumentRegistrationViewModel DocumentRegistration { get; }

    public ProductDocumentListViewModel DocumentList { get; }

    public PolicyClaimManagementViewModel PolicyClaimManagement { get; }

    public ReadOnlyCollection<ProductNavigationItemViewModel> NavigationItems { get; }

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

            SetProperty(ref selectedNavigationItem, value);
        }
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
