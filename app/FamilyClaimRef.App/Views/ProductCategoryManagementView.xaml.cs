using System.Windows;
using System.Windows.Controls;
using FamilyClaimRef.App.Models.Storage;
using FamilyClaimRef.App.ViewModels;

namespace FamilyClaimRef.App.Views;

public partial class ProductCategoryManagementView : UserControl
{
    public ProductCategoryManagementView()
    {
        InitializeComponent();
    }

    private async void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is CategoryManagementViewModel viewModel)
        {
            viewModel.ClearManagementMessage();
            await viewModel.LoadAsync();
        }
    }

    private void RegisterCategoryButton_Click(object sender, RoutedEventArgs e)
    {
        if (Window.GetWindow(this)?.DataContext is ProductShellViewModel shell)
        {
            shell.NavigateToCategoryCreate();
        }
    }

    private void RegisterItemButton_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is CategoryManagementViewModel { SelectedCategory: { } category }
            && Window.GetWindow(this)?.DataContext is ProductShellViewModel shell)
        {
            shell.NavigateToCategoryItemCreate(category.RowId);
        }
    }

    private void EditCategoryButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: CategoryRecord record }
            && Window.GetWindow(this)?.DataContext is ProductShellViewModel shell)
        {
            shell.NavigateToCategoryEdit(record.RowId, shell.CategoryManagement.AggregateVersion);
        }
    }

    private async void DeactivateCategoryButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: CategoryRecord record }
            && DataContext is CategoryManagementViewModel viewModel)
        {
            await viewModel.DeactivateCategoryAsync(record.RowId, viewModel.AggregateVersion);
        }
    }

    private async void ReactivateCategoryButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: CategoryRecord record }
            && DataContext is CategoryManagementViewModel viewModel)
        {
            await viewModel.ReactivateCategoryAsync(record.RowId, viewModel.AggregateVersion);
        }
    }

    private void EditItemButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: CategoryItemRecord record }
            && Window.GetWindow(this)?.DataContext is ProductShellViewModel shell)
        {
            shell.NavigateToCategoryItemEdit(
                record.ParentCategoryId,
                record.RowId,
                shell.CategoryManagement.AggregateVersion);
        }
    }

    private async void DeactivateItemButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: CategoryItemRecord record }
            && DataContext is CategoryManagementViewModel viewModel)
        {
            await viewModel.DeactivateItemAsync(
                record.ParentCategoryId,
                record.RowId,
                viewModel.AggregateVersion);
        }
    }

    private async void ReactivateItemButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: CategoryItemRecord record }
            && DataContext is CategoryManagementViewModel viewModel)
        {
            await viewModel.ReactivateItemAsync(
                record.ParentCategoryId,
                record.RowId,
                viewModel.AggregateVersion);
        }
    }
}
