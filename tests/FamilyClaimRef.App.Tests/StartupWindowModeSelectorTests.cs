using FamilyClaimRef.App.Startup;
using Xunit;

namespace FamilyClaimRef.App.Tests;

public sealed class StartupWindowModeSelectorTests
{
    [Fact]
    public void Select_null_arguments_returns_main_window()
    {
        var result = StartupWindowModeSelector.Select(null);

        Assert.Equal(StartupWindowMode.MainWindow, result);
    }

    [Fact]
    public void Select_empty_arguments_returns_main_window()
    {
        var result = StartupWindowModeSelector.Select([]);

        Assert.Equal(StartupWindowMode.MainWindow, result);
    }

    [Fact]
    public void Select_unknown_arguments_returns_main_window()
    {
        var result = StartupWindowModeSelector.Select(["--unknown", null!]);

        Assert.Equal(StartupWindowMode.MainWindow, result);
    }

    [Theory]
    [InlineData("--product-shell-preview")]
    [InlineData("--PRODUCT-SHELL-PREVIEW")]
    public void Select_exact_preview_argument_returns_product_shell_preview(string argument)
    {
        Assert.Equal(
            "--product-shell-preview",
            StartupWindowModeSelector.ProductShellPreviewArgument);

        var result = StartupWindowModeSelector.Select([argument]);

        Assert.Equal(StartupWindowMode.ProductShellPreview, result);
    }

    [Fact]
    public void Select_preview_argument_among_unrelated_arguments_returns_product_shell_preview()
    {
        var result = StartupWindowModeSelector.Select(
            ["--unknown", "--product-shell-preview", "--another"]);

        Assert.Equal(StartupWindowMode.ProductShellPreview, result);
    }

    [Fact]
    public void Select_duplicate_preview_arguments_returns_product_shell_preview()
    {
        var result = StartupWindowModeSelector.Select(
            ["--product-shell-preview", "--product-shell-preview"]);

        Assert.Equal(StartupWindowMode.ProductShellPreview, result);
    }

    [Fact]
    public void Select_preview_token_prefix_returns_main_window()
    {
        var result = StartupWindowModeSelector.Select(["--product-shell-previe"]);

        Assert.Equal(StartupWindowMode.MainWindow, result);
    }

    [Theory]
    [InlineData("prefix--product-shell-preview")]
    [InlineData("--product-shell-preview=true")]
    public void Select_non_exact_preview_tokens_return_main_window(string argument)
    {
        var result = StartupWindowModeSelector.Select([argument]);

        Assert.Equal(StartupWindowMode.MainWindow, result);
    }

    [Fact]
    public void Select_is_deterministic_and_stateless()
    {
        string[] arguments = ["--unknown", "--product-shell-preview"];

        var first = StartupWindowModeSelector.Select(arguments);
        var second = StartupWindowModeSelector.Select(arguments);

        Assert.Equal(StartupWindowMode.ProductShellPreview, first);
        Assert.Equal(first, second);
    }
}
