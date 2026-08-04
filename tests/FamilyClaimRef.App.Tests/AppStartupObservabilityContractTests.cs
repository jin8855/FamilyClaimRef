using System.Reflection;
using System.Text.RegularExpressions;
using FamilyClaimRef.App.ProductShell;
using FamilyClaimRef.App.Startup;
using FamilyClaimRef.App.ViewModels;
using Xunit;

namespace FamilyClaimRef.App.Tests;

public sealed class AppStartupObservabilityContractTests
{
    [Fact]
    public void App_preserves_existing_startup_order_and_rethrows_exceptions()
    {
        var source = ReadProjectFile("App.xaml.cs");

        AssertOrdered(
            source,
            "base.OnStartup(e);",
            "StartupWindowModeSelector.Select(e.Args)",
            "AppServices.CreateDefault();",
            "CreateProductShellWindow(services)",
            "MainWindow = selectedWindow;",
            "selectedWindow.Show();",
            "productShellWindow.ScheduleStartupDispatcherObservation();");
        Assert.Contains(
            "app_services_create_default",
            source,
            StringComparison.Ordinal);
        Assert.Contains(
            "app_on_startup.exception",
            source,
            StringComparison.Ordinal);
        Assert.Contains("throw;", source, StringComparison.Ordinal);
        Assert.DoesNotContain(
            "throw exception;",
            source,
            StringComparison.Ordinal);
        Assert.Equal(1, CountOccurrences(source, "selectedWindow.Show();"));
        Assert.Equal(1, CountOccurrences(source, "new ProductShellWindow("));
    }

    [Fact]
    public void App_constructor_owns_activation_and_one_handler_registration_call()
    {
        var source = ReadProjectFile("App.xaml.cs");

        AssertOrdered(
            source,
            "public App()",
            "StartupDiagnosticSession.CreateFromEnvironment()",
            "\"app_constructor.body_enter\"",
            "startupDiagnostics.RegisterHandlers(this);",
            "\"app_constructor.body_ready\"");
        Assert.Equal(
            1,
            CountOccurrences(
                source,
                "startupDiagnostics.RegisterHandlers(this);"));
    }

    [Fact]
    public void Product_shell_retains_public_one_argument_constructor()
    {
        var constructor = typeof(ProductShellWindow).GetConstructor(
            BindingFlags.Public | BindingFlags.Instance,
            binder: null,
            [typeof(ProductShellViewModel)],
            modifiers: null);

        Assert.NotNull(constructor);
        Assert.Single(constructor.GetParameters());
    }

    [Fact]
    public void Product_shell_diagnostic_events_and_dispatcher_are_enabled_only()
    {
        var source = ReadProjectFile(
            "ProductShell",
            "ProductShellWindow.xaml.cs");

        Assert.Contains(
            "diagnostics is { IsEnabled: true }",
            source,
            StringComparison.Ordinal);
        Assert.Contains(
            "if (startupDiagnostics is not null)",
            source,
            StringComparison.Ordinal);
        Assert.Contains("Loaded += OnFirstLoaded;", source, StringComparison.Ordinal);
        Assert.Contains(
            "ContentRendered += OnFirstContentRendered;",
            source,
            StringComparison.Ordinal);
        Assert.Contains("Closed += OnClosed;", source, StringComparison.Ordinal);
        Assert.Equal(1, CountOccurrences(source, "Dispatcher.BeginInvoke("));
        Assert.Contains(
            "if (startupDiagnostics is null || dispatcherCallbackScheduled)",
            source,
            StringComparison.Ordinal);
    }

    [Fact]
    public void Runtime_handlers_observe_without_changing_exception_semantics()
    {
        var source = ReadProjectFile(
            "Startup",
            "StartupDiagnosticSession.cs");

        Assert.Equal(
            1,
            CountOccurrences(
                source,
                "AppDomain.CurrentDomain.UnhandledException += appDomainHandler;"));
        Assert.Equal(
            1,
            CountOccurrences(
                source,
                "application.DispatcherUnhandledException += dispatcherHandler;"));
        Assert.Equal(
            1,
            CountOccurrences(
                source,
                "TaskScheduler.UnobservedTaskException += taskSchedulerHandler;"));
        Assert.DoesNotContain("Handled = true", source, StringComparison.Ordinal);
        Assert.DoesNotContain(".SetObserved(", source, StringComparison.Ordinal);
        Assert.DoesNotContain("exception.Message", source, StringComparison.Ordinal);
        Assert.DoesNotContain("exception.ToString", source, StringComparison.Ordinal);
        Assert.DoesNotContain("StackTrace", source, StringComparison.Ordinal);
    }

    [Fact]
    public void Instrumentation_has_no_background_or_product_launch_mechanism()
    {
        var diagnosticSource = ReadProjectFile(
            "Startup",
            "StartupDiagnosticSession.cs");
        var appSource = ReadProjectFile("App.xaml.cs");
        var windowSource = ReadProjectFile(
            "ProductShell",
            "ProductShellWindow.xaml.cs");
        var combined = string.Concat(
            diagnosticSource,
            appSource,
            windowSource);

        Assert.DoesNotContain("Process.Start", combined, StringComparison.Ordinal);
        Assert.DoesNotContain("Task.Run", combined, StringComparison.Ordinal);
        Assert.DoesNotContain("new Thread", combined, StringComparison.Ordinal);
        Assert.DoesNotContain("new Timer", combined, StringComparison.Ordinal);
        Assert.DoesNotContain(
            "static void Main",
            combined,
            StringComparison.Ordinal);
    }

    [Fact]
    public void Instrumentation_does_not_reference_storage_or_registration_owners()
    {
        var source = ReadProjectFile(
            "Startup",
            "StartupDiagnosticSession.cs");

        Assert.DoesNotContain("Services.Storage", source, StringComparison.Ordinal);
        Assert.DoesNotContain("DocumentRecord", source, StringComparison.Ordinal);
        Assert.DoesNotContain("PolicyRecord", source, StringComparison.Ordinal);
        Assert.DoesNotContain("ClaimRecord", source, StringComparison.Ordinal);
        Assert.DoesNotContain("DocumentRegistrationWorkflow", source, StringComparison.Ordinal);
        Assert.DoesNotContain("JsonFileStore", source, StringComparison.Ordinal);
        Assert.DoesNotContain("AppServices", source, StringComparison.Ordinal);
    }

    [Fact]
    public void App_xaml_and_project_keep_generated_entrypoint_contract()
    {
        var appXaml = ReadProjectFile("App.xaml");
        var project = ReadProjectFile("FamilyClaimRef.App.csproj");

        Assert.DoesNotContain("StartupUri", appXaml, StringComparison.Ordinal);
        Assert.DoesNotContain(
            "StartupDiagnosticSession.cs",
            project,
            StringComparison.Ordinal);
        Assert.DoesNotContain("<Compile Include=", project, StringComparison.Ordinal);
        Assert.Contains("<OutputType>WinExe</OutputType>", project, StringComparison.Ordinal);
        Assert.Contains("<UseWPF>true</UseWPF>", project, StringComparison.Ordinal);
    }

    [Fact]
    public void App_services_owner_remains_unmodified_by_instrumentation()
    {
        var source = ReadProjectFile(
            "Composition",
            "AppServices.cs");

        Assert.DoesNotContain(
            "StartupDiagnosticSession",
            source,
            StringComparison.Ordinal);
        Assert.DoesNotContain(
            "FAMILYCLAIMREF_ENABLE_STARTUP_DIAGNOSTICS",
            source,
            StringComparison.Ordinal);
    }

    [Fact]
    public void Diagnostic_configuration_reads_environment_without_writing_it()
    {
        var source = ReadProjectFile(
            "Startup",
            "StartupDiagnosticSession.cs");

        Assert.Equal(
            2,
            Regex.Matches(
                source,
                @"Environment\.GetEnvironmentVariable\(",
                RegexOptions.CultureInvariant).Count);
        Assert.DoesNotContain(
            "Environment.SetEnvironmentVariable",
            source,
            StringComparison.Ordinal);
    }

    [Fact]
    public void Diagnostic_storage_uses_preexisting_handle_pinned_root_without_pathname_compensation()
    {
        var source = ReadProjectFile(
            "Startup",
            "StartupDiagnosticSession.cs");

        Assert.Contains(
            "!Directory.Exists(normalizedRoot)",
            source,
            StringComparison.Ordinal);
        Assert.DoesNotContain(
            "Directory.CreateDirectory(",
            source,
            StringComparison.Ordinal);
        Assert.DoesNotContain(
            "Directory.Delete(",
            source,
            StringComparison.Ordinal);
        Assert.DoesNotContain(
            "File.Delete(",
            source,
            StringComparison.Ordinal);
        Assert.Contains("SafeFileHandle", source, StringComparison.Ordinal);
        Assert.Contains("CreateFileW(", source, StringComparison.Ordinal);
        Assert.Contains(
            "FileFlagBackupSemantics",
            source,
            StringComparison.Ordinal);
        Assert.Contains(
            "FileFlagOpenReparsePoint",
            source,
            StringComparison.Ordinal);
        Assert.Contains(
            "FileShare.Read | FileShare.Write",
            source,
            StringComparison.Ordinal);
        Assert.DoesNotContain(
            "FileShare.Delete",
            source,
            StringComparison.Ordinal);
        Assert.Contains(
            "GetFileInformationByHandleEx",
            source,
            StringComparison.Ordinal);
        Assert.Contains(
            "GetFinalPathNameByHandleW",
            source,
            StringComparison.Ordinal);
        Assert.DoesNotContain(
            "Process.Start",
            source,
            StringComparison.Ordinal);
    }

    private static string ReadProjectFile(params string[] relativeSegments)
    {
        var segments = new[]
        {
            FindProjectRoot(),
            "app",
            "FamilyClaimRef.App"
        }.Concat(relativeSegments).ToArray();

        return File.ReadAllText(Path.Combine(segments));
    }

    private static void AssertOrdered(
        string source,
        params string[] fragments)
    {
        var previousIndex = -1;
        foreach (var fragment in fragments)
        {
            var index = source.IndexOf(fragment, StringComparison.Ordinal);
            Assert.True(
                index > previousIndex,
                $"Expected '{fragment}' after index {previousIndex}.");
            previousIndex = index;
        }
    }

    private static int CountOccurrences(
        string value,
        string fragment)
    {
        var count = 0;
        var startIndex = 0;
        while ((startIndex = value.IndexOf(
                   fragment,
                   startIndex,
                   StringComparison.Ordinal)) >= 0)
        {
            count++;
            startIndex += fragment.Length;
        }

        return count;
    }

    private static string FindProjectRoot()
    {
        var currentDirectory = new DirectoryInfo(AppContext.BaseDirectory);
        while (currentDirectory is not null)
        {
            if (File.Exists(Path.Combine(
                    currentDirectory.FullName,
                    "FamilyClaimRef.sln")))
            {
                return currentDirectory.FullName;
            }

            currentDirectory = currentDirectory.Parent;
        }

        throw new InvalidOperationException("Project root was not found.");
    }
}
