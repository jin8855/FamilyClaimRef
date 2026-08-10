using System.Xml.Linq;
using FamilyClaimRef.App.ViewModels;
using FamilyClaimRef.App.Views;
using Xunit;

namespace FamilyClaimRef.App.Tests;

public sealed class ProductWireframeRouteCoverageTests
{
    private static readonly XNamespace Presentation =
        "http://schemas.microsoft.com/winfx/2006/xaml/presentation";

    private static readonly XNamespace Xaml =
        "http://schemas.microsoft.com/winfx/2006/xaml";

    private static readonly string[] StructuredGenericRoutes =
    [
        ProductScreenRoutes.FamilyMembers,
        ProductScreenRoutes.PolicyList,
        ProductScreenRoutes.PolicyDetail,
        ProductScreenRoutes.OcrReview,
        ProductScreenRoutes.ClaimReferenceResult,
        ProductScreenRoutes.HistoryView,
        ProductScreenRoutes.PolicyRegister,
        ProductScreenRoutes.FamilyRegister,
        ProductScreenRoutes.ClaimComplete,
        ProductScreenRoutes.ManageHome,
        ProductScreenRoutes.CategoryManage,
        ProductScreenRoutes.CategoryRegister,
        ProductScreenRoutes.CategoryItemRegister,
        ProductScreenRoutes.HistoryDetail
    ];

    private static readonly string[] TableRoutes =
    [
        ProductScreenRoutes.FamilyMembers,
        ProductScreenRoutes.PolicyList,
        ProductScreenRoutes.HistoryView,
        ProductScreenRoutes.PolicyRegister,
        ProductScreenRoutes.CategoryManage
    ];

    [Fact]
    public void Approved_wireframe_directory_and_product_routes_have_exact_21_screen_parity()
    {
        var root = FindProjectRoot();
        var wireframes = Directory
            .EnumerateFiles(Path.Combine(root, "design", "wireframes"), "*.html")
            .Select(Path.GetFileNameWithoutExtension)
            .Where(name => name is not "index")
            .OfType<string>()
            .Order(StringComparer.Ordinal)
            .ToArray();

        Assert.Equal(21, wireframes.Length);
        Assert.Equal(ProductScreenRoutes.All, wireframes);
    }

    [Fact]
    public void Generic_routes_have_wireframe_specific_groups_and_expected_table_columns()
    {
        var resourcePath = Path.Combine(
            FindProjectRoot(),
            "app",
            "FamilyClaimRef.App",
            "Resources",
            "ProductScreenContent.xaml");
        var document = XDocument.Load(resourcePath);
        var resources = document.Root!
            .Elements()
            .Where(element => element.Attribute(Xaml + "Key") is not null)
            .ToDictionary(
                element => element.Attribute(Xaml + "Key")!.Value,
                element => element.Value,
                StringComparer.Ordinal);

        foreach (var route in StructuredGenericRoutes)
        {
            var titles = Split(resources[$"Ui.Product.Wireframe.{route}.GroupTitles"]);
            var bodies = Split(resources[$"Ui.Product.Wireframe.{route}.GroupBodies"]);
            var fieldGroups = SplitGroups(resources[$"Ui.Product.Wireframe.{route}.GroupFields"]);

            Assert.True(titles.Length >= 2, route);
            Assert.Equal(titles.Length, bodies.Length);
            Assert.Equal(titles.Length, fieldGroups.Length);
            Assert.All(fieldGroups, fields => Assert.True(fields.Length >= 2, route));
        }

        foreach (var route in TableRoutes)
        {
            Assert.True(
                Split(resources[$"Ui.Product.Wireframe.{route}.Columns"]).Length >= 3,
                route);
            Assert.False(
                string.IsNullOrWhiteSpace(resources[$"Ui.Product.Wireframe.{route}.TableTitle"]),
                route);
        }
    }

    [Fact]
    public void Generic_wireframe_view_renders_groups_table_headers_and_explicit_empty_state()
    {
        var viewPath = Path.Combine(
            FindProjectRoot(),
            "app",
            "FamilyClaimRef.App",
            "Views",
            "ProductWireframeScreenView.xaml");
        var document = XDocument.Load(viewPath);
        var bindings = document
            .Descendants()
            .SelectMany(element => element.Attributes())
            .Select(attribute => attribute.Value)
            .ToArray();

        Assert.Contains("{Binding Groups}", bindings);
        Assert.Contains("{Binding FieldLabels}", bindings);
        Assert.Contains("{Binding TableColumns}", bindings);
        Assert.Contains("{Binding TableTitle}", bindings);
        Assert.Contains("{Binding EmptyMessage}", bindings);
        Assert.Contains("{Binding Commands}", bindings);
        Assert.Contains("{Binding ClaimStepNumber}", bindings);
    }

    [Fact]
    public void Claim_flow_views_expose_all_five_steps_and_safe_shared_context()
    {
        var generic = XDocument.Load(Path.Combine(
            FindProjectRoot(),
            "app",
            "FamilyClaimRef.App",
            "Views",
            "ProductWireframeScreenView.xaml"));
        var entry = XDocument.Load(Path.Combine(
            FindProjectRoot(),
            "app",
            "FamilyClaimRef.App",
            "Views",
            "ProductClaimCasesView.xaml"));

        Assert.Equal(
            5,
            generic.Descendants(Presentation + "Button").Count(button =>
                AttributeValue(button, "AutomationId")?.StartsWith(
                    "ProductClaimFlow_Step",
                    StringComparison.Ordinal) == true));
        Assert.Equal(
            5,
            entry.Descendants(Presentation + "Button").Count(button =>
                AttributeValue(button, "AutomationId")?.StartsWith(
                    "ProductClaim_Step",
                    StringComparison.Ordinal) == true));
        Assert.Contains(
            generic.Descendants(Presentation + "Border"),
            border => AttributeValue(border, "AutomationId") == "ProductClaimFlow_Context");
        Assert.Contains(
            entry.Descendants(Presentation + "Border"),
            border => AttributeValue(border, "AutomationId") == "ProductClaimFlow_Context");
    }

    [Fact]
    public void Registration_view_keeps_real_commands_and_adds_display_only_target_summary()
    {
        var document = XDocument.Load(Path.Combine(
            FindProjectRoot(),
            "app",
            "FamilyClaimRef.App",
            "Views",
            "ProductDocumentRegistrationView.xaml"));
        var values = document
            .Descendants()
            .SelectMany(element => element.Attributes())
            .Select(attribute => attribute.Value)
            .ToArray();

        Assert.Contains("ProductRegistration_SelectFile", values);
        Assert.Contains("ProductRegistration_Register", values);
        Assert.Contains("ProductRegistration_PolicyTarget", values);
        Assert.Contains("ProductRegistration_ClaimTarget", values);
        Assert.Contains("{Binding SelectedPolicyFamilyDisplayName}", values);
        Assert.Contains("{Binding SelectedPolicyInsurerName}", values);
        Assert.DoesNotContain(values, value => value.Contains("TargetId", StringComparison.Ordinal));

        var commandBar = document.Descendants()
            .Single(element => AttributeValue(element, "AutomationId") ==
                "ProductRegistration_CommandBar");
        var summary = document.Descendants()
            .Single(element => AttributeValue(element, "AutomationId") ==
                "ProductRegistration_TargetSummaryPanel");
        var guidance = document.Descendants()
            .Single(element => AttributeValue(element, "AutomationId") ==
                "ProductRegistration_GuidancePanel");
        var workspace = document.Descendants()
            .Single(element => AttributeValue(element, "AutomationId") ==
                "ProductRegistration_Workspace");
        var fileConnection = document.Descendants()
            .Single(element => AttributeValue(element, "AutomationId") ==
                "ProductRegistration_FileConnectionPanel");
        var contentReview = document.Descendants()
            .Single(element => AttributeValue(element, "AutomationId") ==
                "ProductRegistration_ContentReviewPanel");

        Assert.Equal("0", AttributeValue(commandBar, "Grid.Row"));
        Assert.Equal("1", AttributeValue(summary, "Grid.Row"));
        Assert.Equal("2", AttributeValue(guidance, "Grid.Row"));
        Assert.Equal("3", AttributeValue(workspace, "Grid.Row"));
        Assert.Same(workspace, fileConnection.Parent);
        Assert.Same(workspace, contentReview.Parent);
        Assert.Equal("0", AttributeValue(fileConnection, "Grid.Column"));
        Assert.Equal("1", AttributeValue(contentReview, "Grid.Column"));
    }

    [Theory]
    [InlineData(1400, false)]
    [InlineData(1050, false)]
    [InlineData(1049, true)]
    [InlineData(760, true)]
    public void Registration_view_stacks_only_below_desktop_threshold(
        double availableWidth,
        bool expectedStacked)
    {
        Assert.Equal(
            expectedStacked,
            ProductDocumentRegistrationView.ShouldUseStackedLayout(availableWidth));
    }

    [Fact]
    public void Product_content_dictionary_has_title_subtitle_and_sections_for_every_route()
    {
        var resourcePath = Path.Combine(
            FindProjectRoot(),
            "app",
            "FamilyClaimRef.App",
            "Resources",
            "ProductScreenContent.xaml");
        var document = XDocument.Load(resourcePath);
        var keys = document.Root!
            .Elements()
            .Select(element => element.Attribute(Xaml + "Key")?.Value)
            .Where(key => key is not null)
            .ToHashSet(StringComparer.Ordinal);

        foreach (var route in ProductScreenRoutes.All)
        {
            Assert.Contains($"Ui.Product.Wireframe.{route}.Title", keys);
            Assert.Contains($"Ui.Product.Wireframe.{route}.Subtitle", keys);
            Assert.Contains($"Ui.Product.Wireframe.{route}.Primary", keys);
            Assert.Contains($"Ui.Product.Wireframe.{route}.Secondary", keys);
        }
    }

    [Fact]
    public void Shell_templates_cover_functional_routes_and_fall_back_to_generic_view()
    {
        var shellPath = Path.Combine(
            FindProjectRoot(),
            "app",
            "FamilyClaimRef.App",
            "ProductShell",
            "ProductShellWindow.xaml");
        var document = XDocument.Load(shellPath);
        var triggerValues = document
            .Descendants(Presentation + "DataTrigger")
            .Select(trigger => trigger.Attribute("Value")?.Value)
            .Where(value => value is not null)
            .ToHashSet(StringComparer.Ordinal);

        Assert.Contains(ProductScreenRoutes.HomeDashboard, triggerValues);
        Assert.Contains(ProductScreenRoutes.PolicyManage, triggerValues);
        Assert.Contains(ProductScreenRoutes.ClaimCase, triggerValues);
        Assert.Contains(ProductScreenRoutes.PolicyDocumentRegister, triggerValues);
        Assert.Contains(ProductScreenRoutes.ClaimDocumentRegister, triggerValues);
        Assert.Contains(ProductScreenRoutes.ClaimSubmission, triggerValues);
        Assert.Contains(ProductScreenRoutes.ClaimComplete, triggerValues);
        Assert.Contains(ProductScreenRoutes.DocumentBox, triggerValues);
        Assert.Contains(ProductScreenRoutes.FamilyMembers, triggerValues);
        Assert.Contains(ProductScreenRoutes.FamilyRegister, triggerValues);
        Assert.Contains(ProductScreenRoutes.PolicyRegister, triggerValues);
        Assert.Contains(
            document.Descendants(Presentation + "Setter"),
            setter => setter.Attribute("Value")?.Value
                == "{StaticResource WireframeContentTemplate}");
    }

    [Fact]
    public void Family_views_bind_explicit_persistence_commands_and_keep_delete_disabled()
    {
        var viewsRoot = Path.Combine(
            FindProjectRoot(),
            "app",
            "FamilyClaimRef.App",
            "Views");
        var list = XDocument.Load(Path.Combine(viewsRoot, "ProductFamilyMembersView.xaml"));
        var editor = XDocument.Load(Path.Combine(viewsRoot, "ProductFamilyMemberEditorView.xaml"));
        var values = list
            .Descendants()
            .Concat(editor.Descendants())
            .SelectMany(element => element.Attributes())
            .Select(attribute => attribute.Value)
            .ToArray();

        Assert.Contains("ProductFamily_Register", values);
        Assert.Contains("ProductFamily_Save", values);
        Assert.Contains("ProductFamily_Deactivate", values);
        Assert.Contains("ProductFamily_Reactivate", values);
        Assert.Contains("ProductFamily_Delete", values);
        Assert.DoesNotContain(values, value => value.Contains(".Id}", StringComparison.Ordinal));

        var deleteButton = editor
            .Descendants(Presentation + "Button")
            .Single(button => AttributeValue(button, "AutomationId") == "ProductFamily_Delete");
        Assert.Equal("{Binding CanDelete}", deleteButton.Attribute("IsEnabled")?.Value);

        var memo = editor
            .Descendants(Presentation + "TextBox")
            .Single(textBox => AttributeValue(textBox, "AutomationId") == "ProductFamily_Memo");
        Assert.Equal("True", memo.Attribute("AcceptsReturn")?.Value);
        Assert.Equal("Wrap", memo.Attribute("TextWrapping")?.Value);
        Assert.Equal("Left", memo.Attribute("HorizontalContentAlignment")?.Value);
        Assert.Equal("Top", memo.Attribute("VerticalContentAlignment")?.Value);

        var inactiveStatusTrigger = list
            .Descendants(Presentation + "DataTrigger")
            .Single(trigger =>
                trigger.Attribute("Binding")?.Value == "{Binding DisabledAt}"
                && trigger.Attribute("Value")?.Value == "{x:Null}"
                && trigger.Descendants(Presentation + "Setter").Any(setter =>
                    setter.Attribute("Property")?.Value == "Text"));
        Assert.Contains(
            inactiveStatusTrigger.Descendants(Presentation + "Setter"),
            setter => setter.Attribute("Property")?.Value == "Text"
                && setter.Attribute("Value")?.Value
                    == "{StaticResource Ui.Product.Wireframe.Common.ActiveValue}");
        Assert.True(
            list.Descendants(Presentation + "Condition").Count(condition =>
                condition.Attribute("Binding")?.Value == "{Binding DisabledAt}"
                && condition.Attribute("Value")?.Value == "{x:Null}") >= 2);

        var editorCodeBehind = File.ReadAllText(Path.Combine(
            viewsRoot,
            "ProductFamilyMemberEditorView.xaml.cs"));
        Assert.Contains(
            "await shell.SaveFamilyMemberAndReturnAsync();",
            editorCodeBehind,
            StringComparison.Ordinal);
        var listCodeBehind = File.ReadAllText(Path.Combine(
            viewsRoot,
            "ProductFamilyMembersView.xaml.cs"));
        Assert.True(
            listCodeBehind.Split("record.DisabledAt is null", StringSplitOptions.None).Length >= 3);
        Assert.Contains("record.DisabledAt is not null", listCodeBehind, StringComparison.Ordinal);
        Assert.Contains("ReactivateAsync", listCodeBehind, StringComparison.Ordinal);

        var content = XDocument.Load(Path.Combine(
            FindProjectRoot(),
            "app",
            "FamilyClaimRef.App",
            "Resources",
            "ProductScreenContent.xaml"));
        var resources = content.Root!
            .Elements()
            .Where(element => element.Attribute(Xaml + "Key") is not null)
            .ToDictionary(
                element => element.Attribute(Xaml + "Key")!.Value,
                element => element.Value,
                StringComparer.Ordinal);
        Assert.Equal(
            "저장된 가족 정보의 표시명, 관계, 사용 상태를 확인하는 목록입니다.",
            resources["Ui.Product.Wireframe.02_family_members.Primary"]);
        Assert.Equal(
            "저장한 가족 정보는 로컬 JSON 저장소에 유지되며 목록과 편집 화면에서 다시 확인할 수 있습니다. 고유식별정보는 입력하지 않습니다.",
            resources["Ui.Product.Wireframe.13_family_register.Secondary"]);
        Assert.DoesNotContain(
            resources.Values,
            value => value.Contains("가족 저장 모델이 없어", StringComparison.Ordinal));
        var familyUiText = string.Join(
            Environment.NewLine,
            resources
                .Where(resource => resource.Key.Contains("family", StringComparison.OrdinalIgnoreCase))
                .Select(resource => resource.Value));
        Assert.DoesNotContain("본인 후보", familyUiText, StringComparison.Ordinal);
        Assert.DoesNotContain("가족 후보", familyUiText, StringComparison.Ordinal);
    }

    [Fact]
    public void Insurance_policy_views_bind_seven_fields_and_keep_unsupported_commands_disabled()
    {
        var viewsRoot = Path.Combine(
            FindProjectRoot(),
            "app",
            "FamilyClaimRef.App",
            "Views");
        var list = XDocument.Load(Path.Combine(viewsRoot, "ProductPolicyContractsView.xaml"));
        var editor = XDocument.Load(Path.Combine(viewsRoot, "ProductInsurancePolicyEditorView.xaml"));
        var values = list
            .Descendants()
            .Concat(editor.Descendants())
            .SelectMany(element => element.Attributes())
            .Select(attribute => attribute.Value)
            .ToArray();

        var expectedFieldIds = new[]
        {
            "ProductInsurance_DisplayTitle",
            "ProductInsurance_Family",
            "ProductInsurance_Insurer",
            "ProductInsurance_ContractStatus",
            "ProductInsurance_EnrollmentDate",
            "ProductInsurance_CoveragePeriod",
            "ProductInsurance_RegistrationSource"
        };
        Assert.All(expectedFieldIds, id => Assert.Contains(id, values));
        Assert.Contains("ProductInsurance_Save", values);
        Assert.Contains("ProductPolicy_Register", values);
        Assert.Contains("ProductPolicy_List", values);
        Assert.DoesNotContain(values, value => value.Contains("FamilyMemberId}", StringComparison.Ordinal));

        var save = editor
            .Descendants(Presentation + "Button")
            .Single(button => AttributeValue(button, "AutomationId") == "ProductInsurance_Save");
        Assert.Equal("{Binding CanSaveInsurancePolicy}", save.Attribute("IsEnabled")?.Value);

        foreach (var commandId in new[]
                 {
                     "ProductInsurance_Hold",
                     "ProductInsurance_Delete",
                     "ProductInsurance_Disable"
                 })
        {
            var button = editor
                .Descendants(Presentation + "Button")
                .Single(candidate => AttributeValue(candidate, "AutomationId") == commandId);
            Assert.Equal("False", button.Attribute("IsEnabled")?.Value);
        }
    }

    [Fact]
    public void Product_views_hide_development_picker_and_keep_stable_route_action_ids()
    {
        var productRoot = Path.Combine(
            FindProjectRoot(),
            "app",
            "FamilyClaimRef.App");
        var allProductXaml = Directory
            .EnumerateFiles(productRoot, "Product*.xaml", SearchOption.AllDirectories)
            .Select(File.ReadAllText)
            .ToArray();
        var combined = string.Join(Environment.NewLine, allProductXaml);

        Assert.DoesNotContain("ProductScreenPicker", combined, StringComparison.Ordinal);
        Assert.DoesNotContain(
            "Ui.Product.Wireframe.Common.ScreenPickerLabel",
            combined,
            StringComparison.Ordinal);
        Assert.Contains(
            "AutomationProperties.AutomationId=\"{Binding AutomationId}\"",
            combined,
            StringComparison.Ordinal);
        Assert.Contains("ProductClaimFlow_Step1", combined, StringComparison.Ordinal);
        Assert.Contains("ProductClaimFlow_Step5", combined, StringComparison.Ordinal);
        Assert.DoesNotContain("Width=\"220\"", combined, StringComparison.Ordinal);
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

    private static string[] Split(string value)
    {
        return value.Split(
            '|',
            StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    }

    private static string[][] SplitGroups(string value)
    {
        return value
            .Split("||", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(Split)
            .ToArray();
    }

    private static string? AttributeValue(XElement element, string localName)
    {
        return element.Attributes().SingleOrDefault(attribute =>
            attribute.Name.LocalName.Equals(localName, StringComparison.Ordinal)
            || attribute.Name.LocalName.EndsWith($".{localName}", StringComparison.Ordinal))?.Value;
    }
}
