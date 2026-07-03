using System.Configuration;
using System.Data;
using System.Windows;
using FamilyClaimRef.App.Composition;

namespace FamilyClaimRef.App;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var services = AppServices.CreateDefault();
        var window = new MainWindow
        {
            DataContext = services.MainWindowViewModel
        };

        MainWindow = window;
        window.Show();
    }
}

