using System.Windows;
using System.Windows.Controls;
using FamilyClaimRef.App.ViewModels;

namespace FamilyClaimRef.App.Views;

public partial class ProductClaimHistoryListView : UserControl
{
    public ProductClaimHistoryListView()
    {
        InitializeComponent();
    }

    private async void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is ClaimHistoryViewModel viewModel)
        {
            await viewModel.LoadAsync();
        }
    }

    private void ApplyFilters_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is ClaimHistoryViewModel viewModel)
        {
            viewModel.ApplyFilters();
        }
    }

    private void ResetFilters_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is ClaimHistoryViewModel viewModel)
        {
            viewModel.ResetFilters();
        }
    }

    private void OpenDetail_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button { DataContext: ClaimHistoryListItemViewModel item }
            && DataContext is ClaimHistoryViewModel viewModel
            && viewModel.SelectItem(item)
            && Window.GetWindow(this)?.DataContext is ProductShellViewModel shell)
        {
            shell.NavigateTo(ProductScreenRoutes.HistoryDetail);
        }
    }
}
