using System.Windows;
using System.Windows.Controls;
using FamilyClaimRef.App.ViewModels;

namespace FamilyClaimRef.App.Views;

public partial class ProductClaimCasesView : UserControl
{
    public ProductClaimCasesView()
    {
        InitializeComponent();
    }

    private async void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is PolicyClaimManagementViewModel viewModel)
        {
            viewModel.ClearClaimCaseMessages();
            await viewModel.LoadAsync();
        }
    }

    private void NewButton_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is PolicyClaimManagementViewModel viewModel)
        {
            viewModel.StartNewClaimCase();
        }
    }

    private async void CreateButton_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is PolicyClaimManagementViewModel viewModel)
        {
            await viewModel.CreateClaimCaseRecordAsync();
        }
    }

    private async void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is PolicyClaimManagementViewModel viewModel)
        {
            await viewModel.SaveClaimCaseAsync();
        }
    }

    private async void DisableButton_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is PolicyClaimManagementViewModel viewModel)
        {
            await viewModel.DisableSelectedClaimCaseAsync();
        }
    }

    private async void ClaimList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count > 0
            && DataContext is PolicyClaimManagementViewModel viewModel)
        {
            await viewModel.LoadSelectedClaimCaseAsync();
        }
    }
}
