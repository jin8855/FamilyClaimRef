using System.Windows;
using System.Windows.Controls;
using FamilyClaimRef.App.ViewModels;

namespace FamilyClaimRef.App.Views;

public partial class ProductPolicyContractsView : UserControl
{
    public ProductPolicyContractsView()
    {
        InitializeComponent();
    }

    private async void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is PolicyClaimManagementViewModel viewModel)
        {
            viewModel.ClearInsurancePolicyMessage();
            await viewModel.LoadInsurancePoliciesAsync();
        }
    }

    private async void RegisterButton_Click(object sender, RoutedEventArgs e)
    {
        if (Window.GetWindow(this)?.DataContext is ProductShellViewModel shell)
        {
            await shell.NavigateToInsurancePolicyCreateAsync();
        }
    }

    private async void DisableButton_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is PolicyClaimManagementViewModel viewModel
            && !string.IsNullOrWhiteSpace(viewModel.SelectedPolicyId))
        {
            await viewModel.DisableInsurancePolicyAsync(viewModel.SelectedPolicyId);
        }
    }

    private async void EditPolicyButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: InsurancePolicyListItemViewModel item }
            && Window.GetWindow(this)?.DataContext is ProductShellViewModel shell)
        {
            await shell.NavigateToInsurancePolicyEditAsync(item.Id);
        }
    }

    private void AddPolicyDocumentButton_Click(object sender, RoutedEventArgs e)
    {
        SelectPolicyAndNavigate(sender, ProductScreenRoutes.PolicyDocumentRegister);
    }

    private void SelectPolicyAndNavigate(object sender, string routeId)
    {
        if (sender is not FrameworkElement { DataContext: InsurancePolicyListItemViewModel item }
            || DataContext is not PolicyClaimManagementViewModel management
            || Window.GetWindow(this)?.DataContext is not ProductShellViewModel shell)
        {
            return;
        }

        management.SelectedPolicyId = item.Id;
        shell.NavigateTo(routeId);
    }
}
