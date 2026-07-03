using System.Windows;
using FamilyClaimRef.App.ViewModels;

namespace FamilyClaimRef.App;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private async void Window_Loaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is DocumentRegistrationViewModel viewModel)
        {
            await viewModel.LoadTargetOptionsAsync();
        }
    }

    private async void SelectFileButton_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is DocumentRegistrationViewModel viewModel)
        {
            await viewModel.SelectFileAsync();
        }
    }

    private async void RegisterButton_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is DocumentRegistrationViewModel viewModel)
        {
            await viewModel.RegisterAsync();
        }
    }
}
