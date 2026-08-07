using System.Windows;
using System.Windows.Controls;
using FamilyClaimRef.App.ViewModels;

namespace FamilyClaimRef.App.Views;

public partial class ProductClaimSubmissionView : UserControl
{
    public ProductClaimSubmissionView()
    {
        InitializeComponent();
    }

    private async void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is ClaimSubmissionManagementViewModel viewModel)
        {
            await viewModel.LoadAsync();
        }
    }

    private async void ClaimCase_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count > 0
            && DataContext is ClaimSubmissionManagementViewModel viewModel)
        {
            await viewModel.LoadClaimContextAsync();
        }
    }

    private void SubmissionList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count > 0
            && e.AddedItems[0] is ClaimSubmissionListItemViewModel selected
            && DataContext is ClaimSubmissionManagementViewModel viewModel)
        {
            viewModel.SelectedSubmissionId = selected.Id;
            viewModel.LoadSelectedSubmission();
        }
    }

    private void NewButton_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is ClaimSubmissionManagementViewModel viewModel)
        {
            viewModel.StartNew();
        }
    }

    private async void CreateButton_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is ClaimSubmissionManagementViewModel viewModel)
        {
            await viewModel.CreatePreparingAsync();
        }
    }

    private async void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is ClaimSubmissionManagementViewModel viewModel)
        {
            await viewModel.SaveAsync();
        }
    }
}
