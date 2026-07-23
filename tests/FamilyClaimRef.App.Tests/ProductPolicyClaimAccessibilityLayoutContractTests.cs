using System.Xml.Linq;
using Xunit;

namespace FamilyClaimRef.App.Tests;

public sealed class ProductPolicyClaimAccessibilityLayoutContractTests
{
    private static readonly XNamespace Presentation =
        "http://schemas.microsoft.com/winfx/2006/xaml/presentation";

    private static readonly XNamespace Xaml =
        "http://schemas.microsoft.com/winfx/2006/xaml";

    [Fact]
    public void Navigation_items_use_single_selection_and_display_only_automation_names()
    {
        var document = LoadXaml(
            "app",
            "FamilyClaimRef.App",
            "ProductShell",
            "ProductShellWindow.xaml");
        var navigation = Assert.Single(
            document.Descendants(Presentation + "ListBox"),
            element => AttributeValue(element, "ItemsSource") == "{Binding NavigationItems}");

        Assert.Equal("Single", AttributeValue(navigation, "SelectionMode"));
        Assert.Equal(
            "{Binding SelectedNavigationItem, Mode=TwoWay}",
            AttributeValue(navigation, "SelectedItem"));
        Assert.Contains(
            navigation.Descendants(Presentation + "Setter"),
            setter =>
                AttributeValue(setter, "Property") == "AutomationProperties.Name"
                && AttributeValue(setter, "Value") == "{Binding DisplayText}");
    }

    [Fact]
    public void Policy_rows_use_display_title_for_automation_name()
    {
        var document = LoadXaml(
            "app",
            "FamilyClaimRef.App",
            "Views",
            "ProductPolicyContractsView.xaml");
        var policyList = FindItemsControl(document, "ListBox", "{Binding AvailablePolicies}");

        AssertDisplayOnlyAutomationName(policyList, "{Binding DisplayTitle}");
    }

    [Fact]
    public void Claim_policy_selector_and_rows_use_display_title_for_automation_name()
    {
        var document = LoadXaml(
            "app",
            "FamilyClaimRef.App",
            "Views",
            "ProductClaimCasesView.xaml");

        AssertDisplayOnlyAutomationName(
            FindItemsControl(document, "ComboBox", "{Binding AvailablePolicies}"),
            "{Binding DisplayTitle}");
        AssertDisplayOnlyAutomationName(
            FindItemsControl(document, "ListBox", "{Binding AvailableClaims}"),
            "{Binding DisplayTitle}");
    }

    [Fact]
    public void Registration_target_options_use_display_title_for_automation_name()
    {
        var document = LoadXaml(
            "app",
            "FamilyClaimRef.App",
            "Views",
            "ProductDocumentRegistrationView.xaml");

        AssertDisplayOnlyAutomationName(
            FindItemsControl(document, "ComboBox", "{Binding AvailablePolicies}"),
            "{Binding DisplayTitle}");
        AssertDisplayOnlyAutomationName(
            FindItemsControl(document, "ComboBox", "{Binding AvailableClaims}"),
            "{Binding DisplayTitle}");
    }

    [Fact]
    public void Registration_document_type_options_use_label_for_automation_name()
    {
        var document = LoadXaml(
            "app",
            "FamilyClaimRef.App",
            "Views",
            "ProductDocumentRegistrationView.xaml");
        var documentTypeSelector = Assert.Single(
            document.Descendants(Presentation + "ComboBox"),
            element => AttributeValue(element, "DisplayMemberPath") == "Label");

        AssertDisplayOnlyAutomationName(documentTypeSelector, "{Binding Label}");
    }

    [Fact]
    public void Management_result_regions_occupy_the_final_fixed_layout_row()
    {
        foreach (var fileName in new[]
                 {
                     "ProductPolicyContractsView.xaml",
                     "ProductClaimCasesView.xaml"
                 })
        {
            var document = LoadXaml(
                "app",
                "FamilyClaimRef.App",
                "Views",
                fileName);
            var rootGrid = Assert.Single(document.Root!.Elements(Presentation + "Grid"));
            var rowDefinitions = rootGrid
                .Element(Presentation + "Grid.RowDefinitions")!
                .Elements(Presentation + "RowDefinition")
                .Select(row => AttributeValue(row, "Height")!)
                .ToArray();
            var statusGroup = Assert.Single(
                rootGrid.Elements(Presentation + "GroupBox"),
                group =>
                    AttributeValue(group, "Header")
                    == "{StaticResource Ui.Product.Management.StatusLabel}");

            Assert.Equal(["Auto", "Auto", "*", "Auto"], rowDefinitions);
            Assert.Equal("3", AttributeValue(statusGroup, "Grid.Row"));
            Assert.Empty(rootGrid.Elements(Presentation + "ScrollViewer"));
        }
    }

    [Fact]
    public void Accessibility_names_do_not_bind_to_raw_objects_or_internal_fields()
    {
        var paths = new[]
        {
            Path.Combine(
                FindProjectRoot(),
                "app",
                "FamilyClaimRef.App",
                "ProductShell",
                "ProductShellWindow.xaml"),
            Path.Combine(
                FindProjectRoot(),
                "app",
                "FamilyClaimRef.App",
                "Views",
                "ProductPolicyContractsView.xaml"),
            Path.Combine(
                FindProjectRoot(),
                "app",
                "FamilyClaimRef.App",
                "Views",
                "ProductClaimCasesView.xaml"),
            Path.Combine(
                FindProjectRoot(),
                "app",
                "FamilyClaimRef.App",
                "Views",
                "ProductDocumentRegistrationView.xaml")
        };

        var automationNameValues = paths
            .Select(LoadXaml)
            .SelectMany(document => document.Root!.DescendantsAndSelf())
            .SelectMany(element => element.Attributes())
            .Where(attribute => attribute.Name.LocalName == "Name"
                                && attribute.Name.NamespaceName.Contains(
                                    "presentation",
                                    StringComparison.Ordinal))
            .Select(attribute => attribute.Value)
            .ToArray();

        Assert.DoesNotContain(
            automationNameValues,
            value =>
                value is "{Binding}" or "{Binding .}"
                || value.Contains("PolicyRecord", StringComparison.Ordinal)
                || value.Contains("ClaimRecord", StringComparison.Ordinal)
                || value.Contains(" Id", StringComparison.Ordinal)
                || value.Contains("CreatedAt", StringComparison.Ordinal)
                || value.Contains("UpdatedAt", StringComparison.Ordinal)
                || value.Contains("DisabledAt", StringComparison.Ordinal)
                || value.Contains("SortOrder", StringComparison.Ordinal));
    }

    [Fact]
    public void Product_shell_default_dimensions_are_unchanged()
    {
        var document = LoadXaml(
            "app",
            "FamilyClaimRef.App",
            "ProductShell",
            "ProductShellWindow.xaml");

        Assert.Equal("820", AttributeValue(document.Root!, "Width"));
        Assert.Equal("520", AttributeValue(document.Root!, "Height"));
        Assert.Null(document.Root!.Attribute("MinWidth"));
        Assert.Null(document.Root!.Attribute("MinHeight"));
        Assert.Null(document.Root!.Attribute("WindowState"));
    }

    private static XElement FindItemsControl(
        XDocument document,
        string elementName,
        string itemsSource)
    {
        return Assert.Single(
            document.Descendants(Presentation + elementName),
            element => AttributeValue(element, "ItemsSource") == itemsSource);
    }

    private static void AssertDisplayOnlyAutomationName(
        XElement itemsControl,
        string expectedBinding)
    {
        Assert.Contains(
            itemsControl.Descendants(Presentation + "Setter"),
            setter =>
                AttributeValue(setter, "Property") == "AutomationProperties.Name"
                && AttributeValue(setter, "Value") == expectedBinding);
    }

    private static string? AttributeValue(XElement element, string localName)
    {
        return element.Attributes()
            .SingleOrDefault(attribute =>
                attribute.Name.LocalName == localName
                || attribute.Name == Xaml + localName)
            ?.Value;
    }

    private static XDocument LoadXaml(params string[] pathSegments)
    {
        return LoadXaml(Path.Combine([FindProjectRoot(), .. pathSegments]));
    }

    private static XDocument LoadXaml(string path)
    {
        return XDocument.Load(path, LoadOptions.PreserveWhitespace);
    }

    private static string FindProjectRoot()
    {
        var currentDirectory = new DirectoryInfo(AppContext.BaseDirectory);
        while (currentDirectory is not null)
        {
            if (File.Exists(Path.Combine(currentDirectory.FullName, "FamilyClaimRef.sln")))
            {
                return currentDirectory.FullName;
            }

            currentDirectory = currentDirectory.Parent;
        }

        throw new InvalidOperationException("Project root was not found.");
    }
}
