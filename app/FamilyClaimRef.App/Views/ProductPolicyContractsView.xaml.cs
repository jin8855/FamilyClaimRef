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
}
