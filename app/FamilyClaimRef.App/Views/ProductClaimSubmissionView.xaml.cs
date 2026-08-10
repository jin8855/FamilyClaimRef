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

    private async void SubmissionList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count > 0
            && e.AddedItems[0] is ClaimSubmissionListItemViewModel selected
            && DataContext is ClaimSubmissionManagementViewModel viewModel)
        {
            viewModel.SelectedSubmissionId = selected.Id;
            await viewModel.LoadSelectedSubmissionAsync();
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

    private void PaymentList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count > 0
            && e.AddedItems[0] is ClaimPaymentListItemViewModel selected
            && DataContext is ClaimSubmissionManagementViewModel viewModel)
        {
            if (!viewModel.PaymentManagement.SelectPayment(selected.Id)
                && sender is DataGrid paymentList)
            {
                paymentList.SelectedItem = viewModel.PaymentManagement.Payments.FirstOrDefault(
                    item => string.Equals(
                        item.Id,
                        viewModel.PaymentManagement.SelectedPaymentId,
                        StringComparison.Ordinal));
            }
        }
    }

    private void NewPaymentButton_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is ClaimSubmissionManagementViewModel viewModel)
        {
            viewModel.PaymentManagement.StartNew();
        }
    }

    private async void CreatePaymentButton_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is ClaimSubmissionManagementViewModel viewModel)
        {
            await viewModel.PaymentManagement.CreatePendingAsync();
        }
    }

    private async void SavePaymentButton_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is ClaimSubmissionManagementViewModel viewModel)
        {
            await viewModel.PaymentManagement.SaveAsync();
        }
    }
}
