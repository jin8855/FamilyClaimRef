using System.Windows;
using System.Windows.Controls;
using FamilyClaimRef.App.ViewModels;

namespace FamilyClaimRef.App.Views;

public partial class ProductDocumentListView : UserControl
{
    public ProductDocumentListView()
    {
        InitializeComponent();
    }

    private async void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is ProductDocumentListViewModel viewModel)
        {
            await viewModel.LoadAsync();
        }
    }
}
