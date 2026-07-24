using System.Configuration;
using System.Data;
using System.Windows;
using FamilyClaimRef.App.Composition;
using FamilyClaimRef.App.ProductShell;
using FamilyClaimRef.App.Startup;

namespace FamilyClaimRef.App;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var startupMode = StartupWindowModeSelector.Select(e.Args);
        var services = AppServices.CreateDefault();
        Window selectedWindow = startupMode switch
        {
            StartupWindowMode.MainWindow or StartupWindowMode.ProductShellPreview =>
                CreateProductShellWindow(services),
            _ => throw new ArgumentOutOfRangeException(
                nameof(startupMode),
                startupMode,
                null)
        };

        MainWindow = selectedWindow;
        selectedWindow.Show();
    }

    private static ProductShellWindow CreateProductShellWindow(AppServices services)
    {
        return new ProductShellWindow(services.ProductShellViewModel);
    }
}

