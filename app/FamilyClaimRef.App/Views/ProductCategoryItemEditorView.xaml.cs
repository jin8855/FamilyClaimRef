using System.Windows;
using System.Windows.Controls;
using FamilyClaimRef.App.ViewModels;

namespace FamilyClaimRef.App.Views;

public partial class ProductCategoryItemEditorView : UserControl
{
    public ProductCategoryItemEditorView()
    {
        InitializeComponent();
    }

    private async void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        if (Window.GetWindow(this)?.DataContext is ProductShellViewModel shell)
        {
            await shell.SaveCategoryItemAndReturnAsync();
        }
    }
}
