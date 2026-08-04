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
    private readonly StartupDiagnosticSession startupDiagnostics;

    public App()
    {
        startupDiagnostics = StartupDiagnosticSession.CreateFromEnvironment();
        startupDiagnostics.Record(
            "App",
            "app_constructor.body_enter",
            "enter",
            "started",
            "FamilyClaimRef.App.App..ctor");
        startupDiagnostics.RegisterHandlers(this);
        startupDiagnostics.Record(
            "App",
            "app_constructor.body_ready",
            "return",
            "completed",
            "FamilyClaimRef.App.App..ctor");
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        startupDiagnostics.Record(
            "App",
            "app_on_startup.enter",
            "enter",
            "started",
            "FamilyClaimRef.App.App.OnStartup");

        try
        {
            startupDiagnostics.Record(
                "App",
                "base_on_startup",
                "begin",
                "started",
                "FamilyClaimRef.App.App.OnStartup");
            base.OnStartup(e);
            startupDiagnostics.Record(
                "App",
                "base_on_startup",
                "end",
                "completed",
                "FamilyClaimRef.App.App.OnStartup");

            var startupMode = StartupWindowModeSelector.Select(e.Args);
            startupDiagnostics.Record(
                "App",
                "startup_mode.selection",
                "decision",
                startupMode == StartupWindowMode.ProductShellPreview
                    ? "product_shell_preview"
                    : "default",
                "FamilyClaimRef.App.App.OnStartup");

            AppServices services;
            startupDiagnostics.Record(
                "App",
                "app_services_create_default",
                "begin",
                "started",
                "FamilyClaimRef.App.App.OnStartup");
            try
            {
                services = AppServices.CreateDefault();
                startupDiagnostics.Record(
                    "App",
                    "app_services_create_default",
                    "end",
                    "completed",
                    "FamilyClaimRef.App.App.OnStartup");
            }
            catch (Exception exception)
            {
                startupDiagnostics.RecordException(
                    "App",
                    "app_services_create_default",
                    "end",
                    "failed",
                    exception,
                    "FamilyClaimRef.App.App.OnStartup");
                throw;
            }

            startupDiagnostics.Record(
                "App",
                "product_shell_window.construction",
                "begin",
                "started",
                "FamilyClaimRef.App.App.OnStartup");
            Window selectedWindow = startupMode switch
            {
                StartupWindowMode.MainWindow or StartupWindowMode.ProductShellPreview =>
                    CreateProductShellWindow(services),
                _ => throw new ArgumentOutOfRangeException(
                    nameof(startupMode),
                    startupMode,
                    null)
            };
            startupDiagnostics.Record(
                "App",
                "product_shell_window.construction",
                "end",
                "completed",
                "FamilyClaimRef.App.App.OnStartup");

            MainWindow = selectedWindow;
            startupDiagnostics.Record(
                "App",
                "application.main_window_assignment",
                "end",
                "completed",
                "FamilyClaimRef.App.App.OnStartup");
            startupDiagnostics.Record(
                "App",
                "product_shell_window.show",
                "begin",
                "started",
                "FamilyClaimRef.App.App.OnStartup");
            selectedWindow.Show();
            startupDiagnostics.Record(
                "App",
                "product_shell_window.show",
                "return",
                "completed",
                "FamilyClaimRef.App.App.OnStartup");

            if (selectedWindow is ProductShellWindow productShellWindow)
            {
                productShellWindow.ScheduleStartupDispatcherObservation();
            }
        }
        catch (Exception exception)
        {
            startupDiagnostics.RecordException(
                "App",
                "app_on_startup.exception",
                "end",
                "failed",
                exception,
                "FamilyClaimRef.App.App.OnStartup");
            throw;
        }
    }

    protected override void OnExit(ExitEventArgs e)
    {
        startupDiagnostics.Record(
            "App",
            "app_on_exit",
            "enter",
            "started",
            "FamilyClaimRef.App.App.OnExit");
        try
        {
            base.OnExit(e);
            startupDiagnostics.Record(
                "App",
                "app_on_exit",
                "return",
                "completed",
                "FamilyClaimRef.App.App.OnExit");
        }
        catch (Exception exception)
        {
            startupDiagnostics.RecordException(
                "App",
                "app_on_exit",
                "return",
                "failed",
                exception,
                "FamilyClaimRef.App.App.OnExit");
            throw;
        }
        finally
        {
            startupDiagnostics.Dispose();
        }
    }

    private ProductShellWindow CreateProductShellWindow(AppServices services)
    {
        return new ProductShellWindow(
            services.ProductShellViewModel,
            startupDiagnostics);
    }
}

