using System.Xml.Linq;
using Xunit;

namespace FamilyClaimRef.App.Tests;

public sealed class ProductPolicyClaimAccessibilityLayoutContractTests
{
    private static readonly XNamespace Presentation =
        "http://schemas.microsoft.com/winfx/2006/xaml/presentation";

    [Fact]
    public void Shell_exposes_stable_primary_navigation_without_development_screen_picker()
    {
        var document = LoadXaml(
            "app",
            "FamilyClaimRef.App",
            "ProductShell",
            "ProductShellWindow.xaml");
        var automationIds = document
            .Descendants(Presentation + "Button")
            .Select(button => AttributeValue(button, "AutomationId"))
            .Where(value => value is not null)
            .ToArray();

        Assert.Contains("ProductNav_Home", automationIds);
        Assert.Contains("ProductNav_Claim", automationIds);
        Assert.Contains("ProductNav_Policy", automationIds);
        Assert.Contains("ProductNav_History", automationIds);
        Assert.Contains("ProductNav_Manage", automationIds);

        Assert.DoesNotContain(
            document.Descendants(Presentation + "ComboBox"),
            element => AttributeValue(element, "AutomationId") == "ProductScreenPicker");
        Assert.DoesNotContain(
            document.Descendants(Presentation + "TextBlock"),
            element => AttributeValue(element, "Text")
                == "{StaticResource Ui.Product.Wireframe.Common.ScreenPickerLabel}");
    }

    [Fact]
    public void Shell_uses_responsive_approved_dimensions_without_fixed_left_navigation()
    {
        var document = LoadXaml(
            "app",
            "FamilyClaimRef.App",
            "ProductShell",
            "ProductShellWindow.xaml");

        Assert.Equal("1280", AttributeValue(document.Root!, "Width"));
        Assert.Equal("840", AttributeValue(document.Root!, "Height"));
        Assert.Equal("960", AttributeValue(document.Root!, "MinWidth"));
        Assert.Equal("680", AttributeValue(document.Root!, "MinHeight"));
        Assert.DoesNotContain(
            document.Descendants(Presentation + "ColumnDefinition"),
            column => AttributeValue(column, "Width") == "220");
        Assert.DoesNotContain(
            document.Descendants(Presentation + "ListBox"),
            element => AttributeValue(element, "ItemsSource") == "{Binding NavigationItems}");
    }

    [Fact]
    public void Policy_and_claim_controls_use_display_text_and_stable_automation_ids()
    {
        var policy = LoadXaml(
            "app",
            "FamilyClaimRef.App",
            "Views",
            "ProductPolicyContractsView.xaml");
        var policyGrid = Assert.Single(
            policy.Descendants(Presentation + "DataGrid"),
            element => AttributeValue(element, "ItemsSource")
                == "{Binding AvailableInsurancePolicies}");
        Assert.Equal("Id", AttributeValue(policyGrid, "SelectedValuePath"));
        Assert.Equal("ProductPolicy_List", AttributeValue(policyGrid, "AutomationId"));
        Assert.Contains(
            policyGrid.Descendants(Presentation + "DataGridTextColumn"),
            column => AttributeValue(column, "Binding") == "{Binding DisplayTitle}");

        var claim = LoadXaml(
            "app",
            "FamilyClaimRef.App",
            "Views",
            "ProductClaimCasesView.xaml");
        var familySelector = FindItemsControl(
            claim,
            "ComboBox",
            "{Binding AvailableClaimFamilyMembers}");
        var claimList = FindItemsControl(
            claim,
            "ListBox",
            "{Binding AvailableClaims}");

        Assert.Equal("DisplayName", AttributeValue(familySelector, "DisplayMemberPath"));
        Assert.Equal("DisplayTitle", AttributeValue(claimList, "DisplayMemberPath"));
        Assert.Equal("ProductClaim_Family", AttributeValue(familySelector, "AutomationId"));
        Assert.Equal("ProductClaim_List", AttributeValue(claimList, "AutomationId"));
    }

    [Fact]
    public void Policy_search_controls_are_read_only_and_use_only_stable_automation_ids()
    {
        var document = LoadXaml(
            "app",
            "FamilyClaimRef.App",
            "Views",
            "ProductPolicySearchView.xaml");
        var automationIds = document
            .Descendants()
            .Select(element => AttributeValue(element, "AutomationId"))
            .Where(value => value is not null)
            .ToArray();
        var expected = new[]
        {
            "ProductScreen_03",
            "ProductPolicySearch_Family",
            "ProductPolicySearch_Insurer",
            "ProductPolicySearch_ContractStatus",
            "ProductPolicySearch_ProductCategory",
            "ProductPolicySearch_Keyword",
            "ProductPolicySearch_Apply",
            "ProductPolicySearch_Reset",
            "ProductPolicySearch_Results",
            "ProductPolicySearch_StateMessage"
        };
        Assert.All(expected, id => Assert.Contains(id, automationIds));
        Assert.DoesNotContain(
            automationIds,
            value => value!.Contains("{Binding", StringComparison.Ordinal)
                || value.Contains("Id", StringComparison.Ordinal)
                || value.Contains("FamilyDisplayName", StringComparison.Ordinal)
                || value.Contains("InsurerName", StringComparison.Ordinal));
        var filterOptionStyleReference =
            "{StaticResource PolicySearchFilterOptionAutomationStyle}";
        var filterIds = expected.Skip(1).Take(4).ToHashSet(StringComparer.Ordinal);
        var filterControls = document
            .Descendants(Presentation + "ComboBox")
            .Where(element => filterIds.Contains(AttributeValue(element, "AutomationId") ?? string.Empty))
            .ToArray();
        Assert.Equal(4, filterControls.Length);
        Assert.All(
            filterControls,
            element => Assert.Equal(
                filterOptionStyleReference,
                AttributeValue(element, "ItemContainerStyle")));
        var filterOptionName = Assert.Single(
            document.Descendants(Presentation + "Style")
                .Where(style => AttributeValue(style, "Key") == "PolicySearchFilterOptionAutomationStyle")
                .SelectMany(style => style.Descendants(Presentation + "Setter")),
            setter => AttributeValue(setter, "Property") == "AutomationProperties.Name");
        Assert.Equal(
            "{StaticResource Ui.Product.Wireframe.03_policy_list.FilterSectionTitle}",
            AttributeValue(filterOptionName, "Value"));

        var results = Assert.Single(
            document.Descendants(Presentation + "DataGrid"),
            element => AttributeValue(element, "AutomationId") == "ProductPolicySearch_Results");
        Assert.Equal("True", AttributeValue(results, "IsReadOnly"));
        Assert.Equal("False", AttributeValue(results, "CanUserAddRows"));
        var rowName = Assert.Single(
            results
                .Elements(Presentation + "DataGrid.RowStyle")
                .Descendants(Presentation + "Setter"),
            setter => AttributeValue(setter, "Property") == "AutomationProperties.Name");
        Assert.Equal(
            "{StaticResource Ui.Product.Wireframe.03_policy_list.ResultRowAutomationName}",
            AttributeValue(rowName, "Value"));
        var cellName = Assert.Single(
            results
                .Elements(Presentation + "DataGrid.CellStyle")
                .Descendants(Presentation + "Setter"),
            setter => AttributeValue(setter, "Property") == "AutomationProperties.Name");
        Assert.Equal(
            "{StaticResource Ui.Product.Wireframe.03_policy_list.ResultRowAutomationName}",
            AttributeValue(cellName, "Value"));

        var text = document.ToString(SaveOptions.DisableFormatting);
        Assert.DoesNotContain("ProductPolicy_Register", text, StringComparison.Ordinal);
        Assert.DoesNotContain("EditPolicyButton_Click", text, StringComparison.Ordinal);
        Assert.DoesNotContain("DisableButton_Click", text, StringComparison.Ordinal);
        Assert.DoesNotContain("DocumentRegistration", text, StringComparison.Ordinal);
        Assert.DoesNotContain("PolicyCoverage", text, StringComparison.Ordinal);
        Assert.DoesNotContain("ClaimCase", text, StringComparison.Ordinal);
    }

    [Fact]
    public void Registration_controls_hide_paths_and_use_display_text()
    {
        var document = LoadXaml(
            "app",
            "FamilyClaimRef.App",
            "Views",
            "ProductDocumentRegistrationView.xaml");

        var policySelector = FindItemsControl(
            document,
            "ComboBox",
            "{Binding AvailablePolicies}");
        var claimSelector = FindItemsControl(
            document,
            "ComboBox",
            "{Binding AvailableClaims}");
        var documentTypeSelector = Assert.Single(
            document.Descendants(Presentation + "ComboBox"),
            element => AttributeValue(element, "AutomationId") == "ProductRegistration_DocumentType");

        Assert.Equal("DisplayTitle", AttributeValue(policySelector, "DisplayMemberPath"));
        Assert.Equal("DisplayTitle", AttributeValue(claimSelector, "DisplayMemberPath"));
        Assert.Equal("Label", AttributeValue(documentTypeSelector, "DisplayMemberPath"));
        Assert.DoesNotContain(
            document.Descendants(),
            element => AttributeValue(element, "Text") is "{Binding SelectedSourceFilePath}"
                or "{Binding TargetId}");
    }

    [Fact]
    public void Status_and_busy_controls_keep_stable_semantic_ids()
    {
        var files = new[]
        {
            "ProductPolicyContractsView.xaml",
            "ProductClaimCasesView.xaml",
            "ProductDocumentRegistrationView.xaml",
            "ProductDocumentListView.xaml"
        };
        var automationIds = files
            .Select(file => LoadXaml(
                "app",
                "FamilyClaimRef.App",
                "Views",
                file))
            .SelectMany(document => document.Descendants())
            .Select(element => AttributeValue(element, "AutomationId"))
            .Where(value => value is not null)
            .ToHashSet(StringComparer.Ordinal);

        Assert.Contains("ProductPolicy_Status", automationIds);
        Assert.Contains("ProductClaim_Status", automationIds);
        Assert.Contains("ProductRegistration_Validation", automationIds);
        Assert.Contains("ProductRegistration_Status", automationIds);
        Assert.Contains("ProductDocumentBox_List", automationIds);
    }

    [Fact]
    public void Empty_table_messages_render_below_data_grid_headers()
    {
        var expectations = new[]
        {
            (
                File: "ProductPolicyContractsView.xaml",
                Text: "{StaticResource Ui.Product.PolicyContracts.EmptyMessage}"),
            (
                File: "ProductDocumentListView.xaml",
                Text: "{Binding EmptyMessage}"),
            (
                File: "ProductDocumentListView.xaml",
                Text: "{Binding LoadFailedMessage}")
        };

        foreach (var expectation in expectations)
        {
            var document = LoadXaml(
                "app",
                "FamilyClaimRef.App",
                "Views",
                expectation.File);
            var message = Assert.Single(
                document.Descendants(Presentation + "TextBlock"),
                element => AttributeValue(element, "Text") == expectation.Text);

            Assert.Equal("8,38,8,0", AttributeValue(message, "Margin"));
            Assert.Equal("Top", AttributeValue(message, "VerticalAlignment"));
            Assert.Equal("1", AttributeValue(message, "ZIndex"));
        }
    }

    [Fact]
    public void Accessibility_metadata_does_not_bind_to_raw_internal_values()
    {
        var productRoot = Path.Combine(
            FindProjectRoot(),
            "app",
            "FamilyClaimRef.App");
        var paths = Directory
            .EnumerateFiles(productRoot, "Product*.xaml", SearchOption.AllDirectories)
            .Append(Path.Combine(productRoot, "ProductShell", "ProductShellWindow.xaml"));

        var accessibilityValues = paths
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Select(LoadXaml)
            .SelectMany(document => document.Root!.DescendantsAndSelf())
            .SelectMany(element => element.Attributes())
            .Where(attribute =>
                attribute.Name.LocalName is "Name" or "AutomationId")
            .Select(attribute => attribute.Value)
            .ToArray();

        Assert.DoesNotContain(
            accessibilityValues,
            value =>
                value is "{Binding}" or "{Binding .}"
                || value.Contains("TargetId", StringComparison.Ordinal)
                || value.Contains("SourceFilePath", StringComparison.Ordinal)
                || value.Contains("PolicyRecord", StringComparison.Ordinal)
                || value.Contains("ClaimRecord", StringComparison.Ordinal)
                || value.Contains("Exception", StringComparison.Ordinal));
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

    private static string? AttributeValue(XElement element, string localName)
    {
        return element.Attributes()
            .SingleOrDefault(attribute =>
                attribute.Name.LocalName == localName
                || attribute.Name.LocalName.EndsWith(
                    $".{localName}",
                    StringComparison.Ordinal))
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
