using System.Windows;
using System.Windows.Controls;
using FamilyClaimRef.App.ViewModels;

namespace FamilyClaimRef.App.Views;

public partial class ProductHomeView : UserControl
{
    public ProductHomeView()
    {
        InitializeComponent();
    }

    private async void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is not ProductShellViewModel viewModel)
        {
            return;
        }

        await viewModel.PolicyClaimManagement.LoadAsync();
        await viewModel.DocumentList.LoadAsync();
    }
}
