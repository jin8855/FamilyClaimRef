using System.Windows;
using System.Windows.Controls;
using FamilyClaimRef.App.ViewModels;

namespace FamilyClaimRef.App.Views;

public partial class ProductDocumentRegistrationView : UserControl
{
    internal const double StackedLayoutThreshold = 1050;

    public ProductDocumentRegistrationView()
    {
        InitializeComponent();
        ApplyResponsiveLayout(ActualWidth);
    }

    internal static bool ShouldUseStackedLayout(double availableWidth) =>
        availableWidth < StackedLayoutThreshold;

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
            var targetKind = viewModel.TargetKind;
            var targetPolicyId = viewModel.SelectedPolicyId;
            var registrationSucceeded = await viewModel.RegisterAsync();
            if (registrationSucceeded
                && string.Equals(
                    targetKind,
                    DocumentRegistrationViewModel.PolicyTargetKind,
                    StringComparison.Ordinal)
                && !string.IsNullOrWhiteSpace(targetPolicyId)
                && Window.GetWindow(this)?.DataContext is ProductShellViewModel shell)
            {
                await shell.NavigateToInsurancePolicyEditAsync(targetPolicyId);
            }
        }
    }

    private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        ApplyResponsiveLayout(e.NewSize.Width);
    }

    private void ApplyResponsiveLayout(double availableWidth)
    {
        if (ShouldUseStackedLayout(availableWidth))
        {
            WorkspaceFileColumn.Width = new GridLength(1, GridUnitType.Star);
            WorkspaceReviewColumn.Width = new GridLength(0);
            Grid.SetRow(FileConnectionPanel, 0);
            Grid.SetColumn(FileConnectionPanel, 0);
            FileConnectionPanel.Margin = new Thickness(0);
            Grid.SetRow(ContentReviewPanel, 1);
            Grid.SetColumn(ContentReviewPanel, 0);
            ContentReviewPanel.Margin = new Thickness(0, 14, 0, 0);
            return;
        }

        WorkspaceFileColumn.Width = new GridLength(1, GridUnitType.Star);
        WorkspaceReviewColumn.Width = new GridLength(1, GridUnitType.Star);
        Grid.SetRow(FileConnectionPanel, 0);
        Grid.SetColumn(FileConnectionPanel, 0);
        FileConnectionPanel.Margin = new Thickness(0, 0, 7, 0);
        Grid.SetRow(ContentReviewPanel, 0);
        Grid.SetColumn(ContentReviewPanel, 1);
        ContentReviewPanel.Margin = new Thickness(7, 0, 0, 0);
    }
}
