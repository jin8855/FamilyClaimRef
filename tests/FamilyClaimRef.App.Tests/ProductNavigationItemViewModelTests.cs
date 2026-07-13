using FamilyClaimRef.App.ViewModels;
using Xunit;

namespace FamilyClaimRef.App.Tests;

public sealed class ProductNavigationItemViewModelTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_rejects_null_or_blank_id(string? id)
    {
        var exception = Record.Exception(() => new ProductNavigationItemViewModel(id!, "Home"));

        Assert.IsType<ArgumentException>(exception);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_rejects_null_or_blank_display_text(string? displayText)
    {
        var exception = Record.Exception(() => new ProductNavigationItemViewModel("Home", displayText!));

        Assert.IsType<ArgumentException>(exception);
    }

    [Fact]
    public void Constructor_preserves_id_and_display_text()
    {
        var item = new ProductNavigationItemViewModel("Home", "Home display");

        Assert.Equal("Home", item.Id);
        Assert.Equal("Home display", item.DisplayText);
    }

    [Fact]
    public void Public_contract_is_immutable_navigation_state_only()
    {
        var properties = typeof(ProductNavigationItemViewModel).GetProperties();

        Assert.Equal(
            [nameof(ProductNavigationItemViewModel.DisplayText), nameof(ProductNavigationItemViewModel.Id)],
            properties.Select(property => property.Name).OrderBy(name => name, StringComparer.Ordinal));
        Assert.All(properties, property => Assert.Null(property.SetMethod));
        Assert.Empty(typeof(ProductNavigationItemViewModel).GetInterfaces());
    }
}
