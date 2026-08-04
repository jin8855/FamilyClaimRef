using System.Windows;
using System.Windows.Controls;
using FamilyClaimRef.App.ViewModels;

namespace FamilyClaimRef.App.Views;

public partial class ProductDocumentRegistrationView : UserControl
{
    public ProductDocumentRegistrationView()
    {
        InitializeComponent();
    }

    private async void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is DocumentRegistrationViewModel viewModel)
        {
            await viewModel.LoadTargetOptionsAsync();
        }
    }

    private async void SelectFileButton_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is DocumentRegistrationViewModel { IsBusy: false } viewModel)
        {
            await viewModel.SelectFileAsync();
        }
    }

    private async void RegisterButton_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is DocumentRegistrationViewModel { IsBusy: false } viewModel)
        {
            await viewModel.RegisterAsync();
        }
    }
}
