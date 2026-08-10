using System.Windows;
using System.Windows.Controls;
using FamilyClaimRef.App.ViewModels;

namespace FamilyClaimRef.App.Views;

public partial class ProductClaimCompleteSummaryView : UserControl
{
    public ProductClaimCompleteSummaryView()
    {
        InitializeComponent();
    }

    private async void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is ClaimCompleteSummaryViewModel viewModel)
        {
            await viewModel.LoadAsync();
        }
    }
}
