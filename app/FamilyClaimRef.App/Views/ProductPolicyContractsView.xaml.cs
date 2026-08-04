using System.Windows;
using System.Windows.Controls;
using FamilyClaimRef.App.Models.Storage;
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
            viewModel.ClearManagementMessage();
            await viewModel.LoadAsync();
        }
    }

    private async void CreateButton_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is PolicyClaimManagementViewModel viewModel)
        {
            await viewModel.CreatePolicyAsync();
        }
    }

    private async void DisableButton_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is PolicyClaimManagementViewModel viewModel)
        {
            await viewModel.DisableSelectedPolicyAsync();
        }
    }

    private void EditPolicyButton_Click(object sender, RoutedEventArgs e)
    {
        SelectPolicyAndNavigate(sender, ProductScreenRoutes.PolicyRegister);
    }

    private void AddPolicyDocumentButton_Click(object sender, RoutedEventArgs e)
    {
        SelectPolicyAndNavigate(sender, ProductScreenRoutes.PolicyDocumentRegister);
    }

    private void SelectPolicyAndNavigate(object sender, string routeId)
    {
        if (sender is not FrameworkElement { DataContext: PolicyRecord policy }
            || DataContext is not PolicyClaimManagementViewModel management
            || Window.GetWindow(this)?.DataContext is not ProductShellViewModel shell)
        {
            return;
        }

        management.SelectedPolicyId = policy.Id;
        shell.NavigateTo(routeId);
    }
}
