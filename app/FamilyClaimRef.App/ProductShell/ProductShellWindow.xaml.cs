using System.Windows;
using FamilyClaimRef.App.ViewModels;

namespace FamilyClaimRef.App.ProductShell;

public partial class ProductShellWindow : Window
{
    public ProductShellWindow(ProductShellViewModel viewModel)
    {
        ArgumentNullException.ThrowIfNull(viewModel);

        InitializeComponent();
        DataContext = viewModel;
    }
}
