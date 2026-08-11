using System.Windows;
using System.Windows.Controls;
using FamilyClaimRef.App.ViewModels;

namespace FamilyClaimRef.App.Views;

public partial class ProductPolicySearchView : UserControl
{
    public ProductPolicySearchView()
    {
        InitializeComponent();
    }

    private async void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is PolicyClaimManagementViewModel viewModel)
        {
            await viewModel.LoadPolicySearchAsync();
        }
    }

    private void ApplyButton_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is PolicyClaimManagementViewModel viewModel)
        {
            viewModel.ApplyPolicySearch();
        }
    }

    private void ResetButton_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is PolicyClaimManagementViewModel viewModel)
        {
            viewModel.ResetPolicySearchFilters();
        }
    }

    private void DetailButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: InsurancePolicyListItemViewModel item }
            || DataContext is not PolicyClaimManagementViewModel management
            || Window.GetWindow(this)?.DataContext is not ProductShellViewModel shell)
        {
            return;
        }

        management.SelectedPolicyId = item.Id;
        if (shell.NavigateCommand.CanExecute(ProductScreenRoutes.PolicyDetail))
        {
            shell.NavigateCommand.Execute(ProductScreenRoutes.PolicyDetail);
        }
    }
}
