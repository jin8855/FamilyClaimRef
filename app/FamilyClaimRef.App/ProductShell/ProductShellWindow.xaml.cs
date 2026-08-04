using System.Windows;
using System.Windows.Threading;
using FamilyClaimRef.App.Startup;
using FamilyClaimRef.App.ViewModels;

namespace FamilyClaimRef.App.ProductShell;

public partial class ProductShellWindow : Window
{
    public ProductShellWindow(ProductShellViewModel viewModel)
        : this(viewModel, null)
    {
    }

    internal ProductShellWindow(
        ProductShellViewModel viewModel,
        StartupDiagnosticSession? diagnostics)
    {
        ArgumentNullException.ThrowIfNull(viewModel);
        if (diagnostics is { IsEnabled: true })
        {
            startupDiagnostics = diagnostics;
            startupDiagnostics.Record(
                "ProductShellWindow",
                "product_shell_window.constructor",
                "enter",
                "started",
                "FamilyClaimRef.App.ProductShell.ProductShellWindow..ctor");
        }

        try
        {
            startupDiagnostics?.Record(
                "ProductShellWindow",
                "product_shell_window.initialize_component",
                "begin",
                "started",
                "FamilyClaimRef.App.ProductShell.ProductShellWindow..ctor");
            InitializeComponent();
            startupDiagnostics?.Record(
                "ProductShellWindow",
                "product_shell_window.initialize_component",
                "end",
                "completed",
                "FamilyClaimRef.App.ProductShell.ProductShellWindow..ctor");

            startupDiagnostics?.Record(
                "ProductShellWindow",
                "product_shell_window.data_context_assignment",
                "begin",
                "started",
                "FamilyClaimRef.App.ProductShell.ProductShellWindow..ctor");
            DataContext = viewModel;
            startupDiagnostics?.Record(
                "ProductShellWindow",
                "product_shell_window.data_context_assignment",
                "end",
                "completed",
                "FamilyClaimRef.App.ProductShell.ProductShellWindow..ctor");

            if (startupDiagnostics is not null)
            {
                Loaded += OnFirstLoaded;
                ContentRendered += OnFirstContentRendered;
                Closed += OnClosed;
            }

            startupDiagnostics?.Record(
                "ProductShellWindow",
                "product_shell_window.constructor",
                "return",
                "completed",
                "FamilyClaimRef.App.ProductShell.ProductShellWindow..ctor");
        }
        catch (Exception exception)
        {
            startupDiagnostics?.RecordException(
                "ProductShellWindow",
                "product_shell_window.constructor",
                "return",
                "failed",
                exception,
                "FamilyClaimRef.App.ProductShell.ProductShellWindow..ctor");
            throw;
        }
    }

    private readonly StartupDiagnosticSession? startupDiagnostics;
    private bool dispatcherCallbackScheduled;

    internal void ScheduleStartupDispatcherObservation()
    {
        if (startupDiagnostics is null || dispatcherCallbackScheduled)
        {
            return;
        }

        dispatcherCallbackScheduled = true;
        startupDiagnostics.Record(
            "ProductShellWindow",
            "product_shell_window.dispatcher_callback",
            "callback",
            "scheduled",
            "FamilyClaimRef.App.ProductShell.ProductShellWindow.ScheduleStartupDispatcherObservation");

        try
        {
            _ = Dispatcher.BeginInvoke(
                new Action(() =>
                    startupDiagnostics.Record(
                        "ProductShellWindow",
                        "product_shell_window.dispatcher_callback",
                        "callback",
                        "executed",
                        "FamilyClaimRef.App.ProductShell.ProductShellWindow.ScheduleStartupDispatcherObservation")),
                DispatcherPriority.ApplicationIdle);
        }
        catch (Exception exception)
        {
            startupDiagnostics.RecordException(
                "ProductShellWindow",
                "product_shell_window.dispatcher_callback",
                "callback",
                "failed",
                exception,
                "FamilyClaimRef.App.ProductShell.ProductShellWindow.ScheduleStartupDispatcherObservation");
        }
    }

    private void OnFirstLoaded(object sender, RoutedEventArgs e)
    {
        Loaded -= OnFirstLoaded;
        startupDiagnostics?.Record(
            "ProductShellWindow",
            "product_shell_window.loaded",
            "event",
            "observed",
            "FamilyClaimRef.App.ProductShell.ProductShellWindow..ctor");
    }

    private void OnFirstContentRendered(object? sender, EventArgs e)
    {
        ContentRendered -= OnFirstContentRendered;
        startupDiagnostics?.Record(
            "ProductShellWindow",
            "product_shell_window.content_rendered",
            "event",
            "observed",
            "FamilyClaimRef.App.ProductShell.ProductShellWindow..ctor");
    }

    private void OnClosed(object? sender, EventArgs e)
    {
        Loaded -= OnFirstLoaded;
        ContentRendered -= OnFirstContentRendered;
        Closed -= OnClosed;
        startupDiagnostics?.Record(
            "ProductShellWindow",
            "product_shell_window.closed",
            "event",
            "observed",
            "FamilyClaimRef.App.ProductShell.ProductShellWindow..ctor");
    }
}
