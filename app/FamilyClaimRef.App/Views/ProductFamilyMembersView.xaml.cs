using System.Windows;
using System.Windows.Controls;
using FamilyClaimRef.App.Models.Storage;
using FamilyClaimRef.App.ViewModels;

namespace FamilyClaimRef.App.Views;

public partial class ProductFamilyMembersView : UserControl
{
    public ProductFamilyMembersView()
    {
        InitializeComponent();
    }

    private async void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is FamilyMemberManagementViewModel viewModel)
        {
            viewModel.ClearManagementMessage();
            await viewModel.LoadAsync();
        }
    }

    private void RegisterButton_Click(object sender, RoutedEventArgs e)
    {
        if (Window.GetWindow(this)?.DataContext is ProductShellViewModel shell)
        {
            shell.NavigateToFamilyCreate();
        }
    }

    private async void EditButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: FamilyMemberRecord record }
            && record.DisabledAt is null
            && Window.GetWindow(this)?.DataContext is ProductShellViewModel shell)
        {
            await shell.NavigateToFamilyEditAsync(record.Id, record.Version);
        }
    }

    private async void DeactivateButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: FamilyMemberRecord record }
            && record.DisabledAt is null
            && DataContext is FamilyMemberManagementViewModel viewModel)
        {
            await viewModel.DeactivateAsync(record.Id, record.Version);
        }
    }

    private async void ReactivateButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: FamilyMemberRecord record }
            && record.DisabledAt is not null
            && DataContext is FamilyMemberManagementViewModel viewModel)
        {
            await viewModel.ReactivateAsync(record.Id, record.Version);
        }
    }
}
