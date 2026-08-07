using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using FamilyClaimRef.App.ViewModels;

namespace FamilyClaimRef.App.Views;

public partial class ProductInsurancePolicyEditorView : UserControl
{
    internal const double StackedLayoutThreshold = 1100;

    public ProductInsurancePolicyEditorView()
    {
        InitializeComponent();
        ApplyResponsiveLayout(ActualWidth);
    }

    internal static bool ShouldUseStackedLayout(double availableWidth) =>
        availableWidth < StackedLayoutThreshold;

    internal static bool TryFormatPremiumAmountText(string? text, out string formatted)
    {
        var digits = (text ?? string.Empty).Replace(",", string.Empty, StringComparison.Ordinal);
        if (digits.Length == 0)
        {
            formatted = string.Empty;
            return true;
        }

        if (digits.Any(character => !char.IsAsciiDigit(character))
            || !decimal.TryParse(digits, NumberStyles.None, CultureInfo.InvariantCulture, out var amount))
        {
            formatted = string.Empty;
            return false;
        }

        formatted = amount.ToString("N0", CultureInfo.InvariantCulture);
        return true;
    }

    private async void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        if (Window.GetWindow(this)?.DataContext is ProductShellViewModel shell)
        {
            await shell.SaveInsurancePolicyAndReturnAsync();
        }
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        if (Window.GetWindow(this)?.DataContext is ProductShellViewModel shell)
        {
            shell.NavigateTo(ProductScreenRoutes.PolicyManage);
        }
    }

    private async void DocumentPrimaryButton_Click(object sender, RoutedEventArgs e)
    {
        if (!TryGetDocumentActionContext(sender, out var viewModel, out var shell, out var documentType))
        {
            return;
        }

        if (viewModel.HasInsurancePolicyDocumentType(documentType))
        {
            await viewModel.OpenInsurancePolicyDocumentAsync(documentType);
            return;
        }

        shell.NavigateToPolicyDocumentRegistration(documentType);
    }

    private void ReplaceDocumentButton_Click(object sender, RoutedEventArgs e)
    {
        if (TryGetDocumentActionContext(sender, out _, out var shell, out var documentType))
        {
            shell.NavigateToPolicyDocumentRegistration(documentType);
        }
    }

    private async void UnlinkDocumentButton_Click(object sender, RoutedEventArgs e)
    {
        if (!TryGetDocumentActionContext(sender, out var viewModel, out _, out var documentType))
        {
            return;
        }

        var result = MessageBox.Show(
            Window.GetWindow(this),
            (string)FindResource("Ui.Product.InsurancePolicy.DocumentUnlinkConfirmationMessage"),
            (string)FindResource("Ui.Product.InsurancePolicy.DocumentUnlinkConfirmationTitle"),
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning,
            MessageBoxResult.No);
        if (result == MessageBoxResult.Yes)
        {
            await viewModel.UnlinkInsurancePolicyDocumentAsync(documentType);
        }
    }

    private async void HistoryDocumentOpenButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button
            {
                DataContext: InsurancePolicyDocumentHistoryItemViewModel historyItem
            }
            && DataContext is PolicyClaimManagementViewModel viewModel)
        {
            await viewModel.OpenInsurancePolicyDocumentHistoryAsync(historyItem);
        }
    }

    private bool TryGetDocumentActionContext(
        object sender,
        out PolicyClaimManagementViewModel viewModel,
        out ProductShellViewModel shell,
        out string documentType)
    {
        viewModel = null!;
        shell = null!;
        documentType = string.Empty;
        if (sender is not Button { Tag: string tag }
            || string.IsNullOrWhiteSpace(tag)
            || DataContext is not PolicyClaimManagementViewModel
            {
                IsInsurancePolicyEditMode: true,
                SelectedPolicyId: not null
            } management
            || Window.GetWindow(this)?.DataContext is not ProductShellViewModel productShell)
        {
            return false;
        }

        viewModel = management;
        shell = productShell;
        documentType = tag;
        return true;
    }

    private void PremiumAmount_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        e.Handled = e.Text.Any(character => !char.IsAsciiDigit(character));
    }

    private void PremiumAmount_Pasting(object sender, DataObjectPastingEventArgs e)
    {
        if (!e.SourceDataObject.GetDataPresent(DataFormats.UnicodeText)
            || !TryFormatPremiumAmountText(
                e.SourceDataObject.GetData(DataFormats.UnicodeText) as string,
                out _))
        {
            e.CancelCommand();
        }
    }

    private void PremiumAmount_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (sender is not TextBox textBox
            || !TryFormatPremiumAmountText(textBox.Text, out var formatted)
            || string.Equals(textBox.Text, formatted, StringComparison.Ordinal))
        {
            return;
        }

        var digitsAfterCaret = textBox.Text[textBox.CaretIndex..]
            .Count(char.IsAsciiDigit);
        textBox.Text = formatted;
        textBox.CaretIndex = FindCaretIndexFromRight(formatted, digitsAfterCaret);
    }

    private static int FindCaretIndexFromRight(string text, int digitsAfterCaret)
    {
        var remaining = digitsAfterCaret;
        for (var index = text.Length; index > 0; index--)
        {
            if (char.IsAsciiDigit(text[index - 1]) && remaining-- == 0)
            {
                return index;
            }
        }

        return 0;
    }

    private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        ApplyResponsiveLayout(e.NewSize.Width);
    }

    private void ApplyResponsiveLayout(double availableWidth)
    {
        var useStackedLayout = ShouldUseStackedLayout(availableWidth);
        if (useStackedLayout)
        {
            HeaderTitleColumn.Width = new GridLength(1, GridUnitType.Star);
            HeaderCommandColumn.Width = new GridLength(0);
            HeaderTitleArea.Margin = new Thickness(0);
            Grid.SetRow(HeaderCommands, 1);
            Grid.SetColumn(HeaderCommands, 0);
            HeaderCommands.Margin = new Thickness(0, 8, 0, 0);
            SummaryColumn.Width = new GridLength(1, GridUnitType.Star);
            DocumentColumn.Width = new GridLength(0);
            Grid.SetRow(InsuranceSummaryPanel, 0);
            Grid.SetColumn(InsuranceSummaryPanel, 0);
            InsuranceSummaryPanel.Margin = new Thickness(0);
            Grid.SetRow(LinkedDocumentsPanel, 1);
            Grid.SetColumn(LinkedDocumentsPanel, 0);
            LinkedDocumentsPanel.Margin = new Thickness(0);
            return;
        }

        HeaderTitleColumn.Width = new GridLength(1, GridUnitType.Star);
        HeaderCommandColumn.Width = GridLength.Auto;
        HeaderTitleArea.Margin = new Thickness(0, 0, 20, 0);
        Grid.SetRow(HeaderCommands, 0);
        Grid.SetColumn(HeaderCommands, 1);
        HeaderCommands.Margin = new Thickness(0);
        SummaryColumn.Width = new GridLength(1, GridUnitType.Star);
        DocumentColumn.Width = new GridLength(1, GridUnitType.Star);
        Grid.SetRow(InsuranceSummaryPanel, 0);
        Grid.SetColumn(InsuranceSummaryPanel, 0);
        InsuranceSummaryPanel.Margin = new Thickness(0, 0, 7, 0);
        Grid.SetRow(LinkedDocumentsPanel, 0);
        Grid.SetColumn(LinkedDocumentsPanel, 1);
        LinkedDocumentsPanel.Margin = new Thickness(7, 0, 0, 0);
    }
}
