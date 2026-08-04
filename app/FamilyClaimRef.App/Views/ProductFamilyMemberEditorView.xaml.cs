using System.Windows;
using System.Windows.Controls;
using FamilyClaimRef.App.ViewModels;

namespace FamilyClaimRef.App.Views;

public partial class ProductFamilyMemberEditorView : UserControl
{
    public ProductFamilyMemberEditorView()
    {
        InitializeComponent();
    }

    private async void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        if (Window.GetWindow(this)?.DataContext is ProductShellViewModel shell)
        {
            await shell.SaveFamilyMemberAndReturnAsync();
        }
    }

    private async void DeactivateButton_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is FamilyMemberManagementViewModel viewModel)
        {
            await viewModel.DeactivateCurrentAsync();
        }
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        if (Window.GetWindow(this)?.DataContext is ProductShellViewModel shell)
        {
            shell.NavigateTo(ProductScreenRoutes.FamilyMembers);
        }
    }
}
