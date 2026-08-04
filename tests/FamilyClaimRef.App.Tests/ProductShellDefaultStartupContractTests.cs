using System.Text.RegularExpressions;
using System.Xml.Linq;
using FamilyClaimRef.App.Startup;
using Xunit;

namespace FamilyClaimRef.App.Tests;

public sealed class ProductShellDefaultStartupContractTests
{
    [Fact]
    public void App_maps_default_and_preview_modes_to_the_same_product_shell_factory()
    {
        var source = ReadAppSource();

        Assert.Matches(
            new Regex(
                @"StartupWindowMode\.MainWindow\s+or\s+" +
                @"StartupWindowMode\.ProductShellPreview\s*=>\s*" +
                @"CreateProductShellWindow\(services\)",
                RegexOptions.CultureInvariant),
            source);
    }

    [Fact]
    public void App_constructs_product_shell_once_and_never_constructs_legacy_main_window()
    {
        var source = ReadAppSource();

        Assert.Equal(1, CountOccurrences(source, "new ProductShellWindow("));
        Assert.Equal(0, CountOccurrences(source, "new MainWindow"));
    }

    [Fact]
    public void App_assigns_the_selected_product_shell_as_application_main_window_before_showing_it()
    {
        var source = ReadAppSource();
        var assignmentIndex = source.IndexOf(
            "MainWindow = selectedWindow;",
            StringComparison.Ordinal);
        var showIndex = source.IndexOf(
            "selectedWindow.Show();",
            StringComparison.Ordinal);

        Assert.True(assignmentIndex >= 0);
        Assert.True(showIndex > assignmentIndex);
    }

    [Fact]
    public void App_has_one_top_level_show_path()
    {
        var source = ReadAppSource();

        Assert.Equal(1, CountOccurrences(source, ".Show();"));
    }

    [Fact]
    public void Unknown_startup_modes_throw_instead_of_falling_back_to_legacy_main_window()
    {
        var source = ReadAppSource();

        Assert.Contains(
            "_ => throw new ArgumentOutOfRangeException(",
            source,
            StringComparison.Ordinal);
        Assert.DoesNotContain("new MainWindow", source, StringComparison.Ordinal);
    }

    [Fact]
    public void No_arguments_keep_the_selector_default_while_the_app_maps_it_to_product_shell()
    {
        var source = ReadAppSource();

        Assert.Equal(
            StartupWindowMode.MainWindow,
            StartupWindowModeSelector.Select([]));
        Assert.Contains(
            "StartupWindowMode.MainWindow or StartupWindowMode.ProductShellPreview",
            source,
            StringComparison.Ordinal);
    }

    [Fact]
    public void Preview_token_remains_exact_and_selectable()
    {
        Assert.Equal(
            "--product-shell-preview",
            StartupWindowModeSelector.ProductShellPreviewArgument);
        Assert.Equal(
            StartupWindowMode.ProductShellPreview,
            StartupWindowModeSelector.Select(["--product-shell-preview"]));
    }

    [Fact]
    public void Unknown_arguments_use_the_default_mode_that_the_app_maps_to_product_shell()
    {
        var source = ReadAppSource();

        Assert.Equal(
            StartupWindowMode.MainWindow,
            StartupWindowModeSelector.Select(["--unknown"]));
        Assert.Contains(
            "StartupWindowMode.MainWindow or StartupWindowMode.ProductShellPreview",
            source,
            StringComparison.Ordinal);
        Assert.DoesNotContain("new MainWindow", source, StringComparison.Ordinal);
    }

    [Fact]
    public void Product_shell_uses_approved_responsive_desktop_dimensions()
    {
        var xamlPath = Path.Combine(
            FindProjectRoot(),
            "app",
            "FamilyClaimRef.App",
            "ProductShell",
            "ProductShellWindow.xaml");
        var root = XDocument.Load(xamlPath).Root;

        Assert.NotNull(root);
        Assert.Equal("1280", root.Attribute("Width")?.Value);
        Assert.Equal("840", root.Attribute("Height")?.Value);
        Assert.Equal("960", root.Attribute("MinWidth")?.Value);
        Assert.Equal("680", root.Attribute("MinHeight")?.Value);
    }

    [Fact]
    public void Legacy_main_window_sources_remain_preserved()
    {
        var appProjectPath = Path.Combine(
            FindProjectRoot(),
            "app",
            "FamilyClaimRef.App");

        Assert.True(File.Exists(Path.Combine(appProjectPath, "MainWindow.xaml")));
        Assert.True(File.Exists(Path.Combine(appProjectPath, "MainWindow.xaml.cs")));
    }

    [Fact]
    public void App_does_not_add_persistent_environment_or_registry_startup_state()
    {
        var source = ReadAppSource();

        Assert.DoesNotContain(
            "Environment.SetEnvironmentVariable",
            source,
            StringComparison.Ordinal);
        Assert.DoesNotContain("Registry", source, StringComparison.Ordinal);
        Assert.DoesNotContain("FAMILYCLAIMREF_", source, StringComparison.Ordinal);
    }

    private static string ReadAppSource()
    {
        return File.ReadAllText(
            Path.Combine(
                FindProjectRoot(),
                "app",
                "FamilyClaimRef.App",
                "App.xaml.cs"));
    }

    private static int CountOccurrences(string value, string fragment)
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
