using System.Windows;
using System.Windows.Controls;
using FamilyClaimRef.App.ViewModels;

namespace FamilyClaimRef.App.Views;

public partial class ProductClaimReferenceResultView : UserControl
{
    public ProductClaimReferenceResultView()
    {
        InitializeComponent();
    }

    private async void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is ClaimReferenceResultViewModel viewModel)
        {
            await viewModel.LoadAsync();
        }
    }
}
