using System.Globalization;
using System.Xml.Linq;
using FamilyClaimRef.App.Models.Storage;
using FamilyClaimRef.App.ViewModels;
using FamilyClaimRef.App.Views;
using Xunit;

namespace FamilyClaimRef.App.Tests;

public sealed class InsurancePolicyRevisionContractTests
{
    private static readonly XNamespace Presentation =
        "http://schemas.microsoft.com/winfx/2006/xaml/presentation";
    private static readonly XNamespace Xaml =
        "http://schemas.microsoft.com/winfx/2006/xaml";

    [Fact]
    public void Selection_contracts_have_exact_values_and_order()
    {
        Assert.Equal(
            ["유지", "만기", "보험료 납입면제"],
            InsurancePolicyValues.ContractStatuses);
        Assert.Equal(
            ["갱신형", "비갱신형(고정형)", "일부 갱신형"],
            InsurancePolicyValues.RenewalTypes);
        Assert.Equal(
            ["환급형", "해약환급금 미지급형"],
            InsurancePolicyValues.RefundTypes);
        Assert.Equal(
            ["생명보험", "손해보험"],
            InsurancePolicyValues.BusinessTypes);
        Assert.Equal(
            ["실손보험", "운전자보험", "암보험", "종합보험"],
            InsurancePolicyValues.ProductCategories);
        Assert.Equal(
            ["직접 입력", "보험 문서 등록"],
            InsurancePolicyValues.RegistrationSources);
    }

    [Fact]
    public void Editor_has_four_sections_five_combo_contracts_and_read_only_source()
    {
        var document = LoadView("ProductInsurancePolicyEditorView.xaml");
        var text = File.ReadAllText(ViewPath("ProductInsurancePolicyEditorView.xaml"));
        Assert.Contains("Ui.Product.InsurancePolicy.BasicInformationSection", text, StringComparison.Ordinal);
        Assert.Contains("Ui.Product.InsurancePolicy.CoveragePaymentSection", text, StringComparison.Ordinal);
        Assert.Contains("Ui.Product.InsurancePolicy.ClassificationSection", text, StringComparison.Ordinal);
        Assert.Contains("Ui.Product.InsurancePolicy.RegistrationInformationSection", text, StringComparison.Ordinal);

        var comboIds = document.Descendants(Presentation + "ComboBox")
            .Select(GetAutomationId)
            .Where(value => value?.StartsWith("ProductInsurance_", StringComparison.Ordinal) == true)
            .ToArray();
        Assert.Contains("ProductInsurance_ContractStatus", comboIds);
        Assert.Contains("ProductInsurance_RenewalType", comboIds);
        Assert.Contains("ProductInsurance_RefundType", comboIds);
        Assert.Contains("ProductInsurance_BusinessType", comboIds);
        Assert.Contains("ProductInsurance_ProductCategory", comboIds);

        var source = document.Descendants(Presentation + "TextBox")
            .Single(element => GetAutomationId(element) ==
                "ProductInsurance_RegistrationSource");
        Assert.Equal(
            "{Binding InsuranceRegistrationSource, Mode=OneWay}",
            source.Attribute("Text")?.Value);
        Assert.Equal("True", source.Attribute("IsReadOnly")?.Value);
        Assert.Equal("False", source.Attribute("IsTabStop")?.Value);
    }

    [Fact]
    public void Premium_amount_uses_numeric_right_aligned_auto_grouping_contract()
    {
        var document = LoadView("ProductInsurancePolicyEditorView.xaml");
        var resources = XDocument.Load(Path.Combine(
                FindProjectRoot(),
                "app",
                "FamilyClaimRef.App",
                "Resources",
                "UiStrings.xaml"))
            .Descendants()
            .Where(element => element.Attribute(Xaml + "Key") is not null)
            .ToDictionary(
                element => element.Attribute(Xaml + "Key")!.Value,
                element => element.Value,
                StringComparer.Ordinal);
        var amount = FindByAutomationId(
            document,
            "ProductInsurance_TotalPlannedPremiumAmount");

        Assert.Equal("납입액", resources["Ui.Product.InsurancePolicy.TotalPlannedPremiumAmountLabel"]);
        Assert.Equal("Right", amount.Attribute("TextAlignment")?.Value);
        Assert.Equal("PremiumAmount_PreviewTextInput", amount.Attribute("PreviewTextInput")?.Value);
        Assert.Equal("PremiumAmount_TextChanged", amount.Attribute("TextChanged")?.Value);
        Assert.Equal("PremiumAmount_Pasting", amount.Attributes()
            .Single(attribute => attribute.Name.LocalName.EndsWith("Pasting", StringComparison.Ordinal))
            .Value);
    }

    [Theory]
    [InlineData("", "")]
    [InlineData("1", "1")]
    [InlineData("1200", "1,200")]
    [InlineData("12,000,000", "12,000,000")]
    public void Premium_amount_formatter_adds_thousands_grouping(
        string input,
        string expected)
    {
        Assert.True(ProductInsurancePolicyEditorView.TryFormatPremiumAmountText(input, out var formatted));
        Assert.Equal(expected, formatted);
    }

    [Theory]
    [InlineData("-1")]
    [InlineData("1.5")]
    [InlineData("12a")]
    public void Premium_amount_formatter_rejects_non_digit_input(string input)
    {
        Assert.False(ProductInsurancePolicyEditorView.TryFormatPremiumAmountText(input, out _));
    }

    [Fact]
    public void Editor_preserves_approved_three_area_wireframe_structure()
    {
        var document = LoadView("ProductInsurancePolicyEditorView.xaml");
        var rootGrid = document.Descendants(Presentation + "Grid")
            .First(element => element.Ancestors(Presentation + "ScrollViewer").Any());
        var commandBar = FindByAutomationId(document, "ProductInsurance_CommandBar");
        var summaryPanel = FindByAutomationId(document, "ProductInsurance_SummaryPanel");
        var documentsPanel = FindByAutomationId(document, "ProductInsurance_LinkedDocumentsPanel");
        var coveragePanel = FindByAutomationId(document, "ProductInsurance_CoverageCandidatesPanel");

        Assert.Same(rootGrid, commandBar.Parent);
        Assert.Null(AttachedValue(commandBar, "Grid.Row"));
        Assert.Equal("24,16,24,24", rootGrid.Attribute("Margin")?.Value);
        Assert.Equal("0", AttachedValue(summaryPanel, "Grid.Row"));
        Assert.Equal("0", AttachedValue(summaryPanel, "Grid.Column"));
        Assert.Equal("0", AttachedValue(documentsPanel, "Grid.Row"));
        Assert.Equal("1", AttachedValue(documentsPanel, "Grid.Column"));
        Assert.Same(rootGrid, coveragePanel.Parent);
        Assert.Equal("2", AttachedValue(coveragePanel, "Grid.Row"));
        Assert.Null(AttachedValue(coveragePanel, "Grid.Column"));
        Assert.DoesNotContain("MaxWidth", rootGrid.Attributes().Select(attribute => attribute.Name.LocalName));
    }

    [Fact]
    public void Editor_command_bar_and_deferred_actions_match_approved_contract()
    {
        var document = LoadView("ProductInsurancePolicyEditorView.xaml");
        var commandBar = FindByAutomationId(document, "ProductInsurance_CommandBar");
        var buttons = commandBar.Descendants(Presentation + "Button")
            .ToDictionary(element => GetAutomationId(element)!, StringComparer.Ordinal);

        Assert.Equal(
            [
                "ProductInsurance_TemporarySave",
                "ProductInsurance_Save",
                "ProductInsurance_Hold",
                "ProductInsurance_Delete",
                "ProductInsurance_Disable",
                "ProductInsurance_Close"
            ],
            buttons.Keys);
        Assert.Equal("False", buttons["ProductInsurance_TemporarySave"].Attribute("IsEnabled")?.Value);
        Assert.Equal("{Binding CanSaveInsurancePolicy}", buttons["ProductInsurance_Save"].Attribute("IsEnabled")?.Value);
        Assert.Equal("False", buttons["ProductInsurance_Hold"].Attribute("IsEnabled")?.Value);
        Assert.Equal("False", buttons["ProductInsurance_Delete"].Attribute("IsEnabled")?.Value);
        Assert.Equal("False", buttons["ProductInsurance_Disable"].Attribute("IsEnabled")?.Value);
        Assert.Null(buttons["ProductInsurance_Close"].Attribute("IsEnabled"));
    }

    [Fact]
    public void Editor_keeps_linked_document_and_coverage_areas_honest_and_guarded()
    {
        var document = LoadView("ProductInsurancePolicyEditorView.xaml");
        var linkedDocuments = FindByAutomationId(document, "ProductInsurance_LinkedDocumentTable");
        var coverageCandidates = FindByAutomationId(document, "ProductInsurance_CoverageCandidateTable");
        var documentButtons = linkedDocuments.Descendants(Presentation + "Button").ToArray();

        Assert.Equal(9, documentButtons.Length);
        Assert.All(
            documentButtons,
            button => Assert.Equal(
                "{Binding IsInsurancePolicyEditMode}",
                button.Attribute("IsEnabled")?.Value));
        Assert.Equal(
            [
                "ProductInsurance_CaptureDocumentPrimary",
                "ProductInsurance_CaptureDocumentReplace",
                "ProductInsurance_CaptureDocumentUnlink",
                "ProductInsurance_PolicyDocumentPrimary",
                "ProductInsurance_PolicyDocumentReplace",
                "ProductInsurance_PolicyDocumentUnlink",
                "ProductInsurance_TermsDocumentPrimary",
                "ProductInsurance_TermsDocumentReplace",
                "ProductInsurance_TermsDocumentUnlink"
            ],
            documentButtons.Select(GetAutomationId));
        Assert.Equal(3, documentButtons.Count(button => button.Attribute("Tag")?.Value == "capture"));
        Assert.Equal(3, documentButtons.Count(button => button.Attribute("Tag")?.Value == "policy"));
        Assert.Equal(3, documentButtons.Count(button => button.Attribute("Tag")?.Value == "terms"));
        var viewText = File.ReadAllText(ViewPath("ProductInsurancePolicyEditorView.xaml"));
        Assert.Contains("DocumentPrimaryButton_Click", viewText, StringComparison.Ordinal);
        Assert.Contains("ReplaceDocumentButton_Click", viewText, StringComparison.Ordinal);
        Assert.Contains("UnlinkDocumentButton_Click", viewText, StringComparison.Ordinal);
        Assert.Contains("{Binding HasInsuranceCaptureDocument}", viewText, StringComparison.Ordinal);
        Assert.Contains("{Binding HasInsurancePolicyDocument}", viewText, StringComparison.Ordinal);
        Assert.Contains("{Binding HasInsuranceTermsDocument}", viewText, StringComparison.Ordinal);
        Assert.Contains(
            coverageCandidates.Descendants(Presentation + "TextBlock"),
            element => element.Attribute("Text")?.Value ==
                "{StaticResource Ui.Product.InsurancePolicy.CoverageCandidatesEmptyMessage}");
    }

    [Fact]
    public void Editor_binds_linked_document_statuses_and_registration_returns_to_policy()
    {
        var document = LoadView("ProductInsurancePolicyEditorView.xaml");
        var statusBindings = FindByAutomationId(document, "ProductInsurance_LinkedDocumentTable")
            .Descendants(Presentation + "TextBlock")
            .Select(element => element.Attribute("Text")?.Value)
            .Where(value => value?.StartsWith("{Binding Insurance", StringComparison.Ordinal) == true)
            .Select(value => value!)
            .ToArray();
        var registrationCode = File.ReadAllText(ViewCodePath("ProductDocumentRegistrationView.xaml.cs"));

        Assert.Equal(
            [
                "{Binding InsuranceCaptureDocumentStatus}",
                "{Binding InsurancePolicyDocumentStatus}",
                "{Binding InsuranceTermsDocumentStatus}"
            ],
            statusBindings);
        Assert.Contains("await viewModel.RegisterAsync()", registrationCode, StringComparison.Ordinal);
        Assert.Contains("NavigateToInsurancePolicyEditAsync", registrationCode, StringComparison.Ordinal);
    }

    [Fact]
    public void Editor_exposes_collapsed_open_only_document_history_without_extra_page_height()
    {
        var document = LoadView("ProductInsurancePolicyEditorView.xaml");
        var expander = FindByAutomationId(document, "ProductInsurance_DocumentHistoryExpander");
        var historyList = FindByAutomationId(document, "ProductInsurance_DocumentHistoryList");
        var historyOpen = document.Descendants(Presentation + "Button")
            .Single(element => GetAutomationId(element) == "ProductInsurance_DocumentHistoryOpen");
        var historyColumns = historyList.Descendants(Presentation + "GridViewColumn").ToArray();
        var historyTitle = historyColumns[1].Descendants(Presentation + "TextBlock").Single();
        var viewText = File.ReadAllText(ViewPath("ProductInsurancePolicyEditorView.xaml"));
        var code = File.ReadAllText(ViewCodePath("ProductInsurancePolicyEditorView.xaml.cs"));

        Assert.Equal("False", expander.Attribute("IsExpanded")?.Value);
        Assert.Equal(
            "{Binding InsurancePolicyDocumentHistoryTitle}",
            expander.Attribute("Header")?.Value);
        Assert.Contains("{Binding HasInsurancePolicyDocumentHistory}", viewText, StringComparison.Ordinal);
        Assert.Equal(
            "{Binding InsurancePolicyDocumentHistory}",
            historyList.Attribute("ItemsSource")?.Value);
        Assert.Equal("220", historyList.Attribute("MaxHeight")?.Value);
        Assert.Equal(
            "Disabled",
            historyList.Attribute("ScrollViewer.HorizontalScrollBarVisibility")?.Value);
        Assert.Equal(
            "Auto",
            historyList.Attribute("ScrollViewer.VerticalScrollBarVisibility")?.Value);
        Assert.Equal(
            ["115", "135", "125", "55", "100"],
            historyColumns.Select(column => column.Attribute("Width")?.Value));
        Assert.Equal("{Binding DisplayTitle}", historyTitle.Attribute("Text")?.Value);
        Assert.Equal("CharacterEllipsis", historyTitle.Attribute("TextTrimming")?.Value);
        Assert.Equal("{Binding DisplayTitle}", historyTitle.Attribute("ToolTip")?.Value);
        Assert.Equal(
            "HistoryDocumentOpenButton_Click",
            historyOpen.Attribute("Click")?.Value);
        Assert.Contains("{Binding DocumentTypeDisplayName}", viewText, StringComparison.Ordinal);
        Assert.Contains("{Binding DisplayTitle}", viewText, StringComparison.Ordinal);
        Assert.Contains("{Binding RegisteredAt}", viewText, StringComparison.Ordinal);
        Assert.Contains("{Binding Status}", viewText, StringComparison.Ordinal);
        Assert.Contains("OpenInsurancePolicyDocumentHistoryAsync", code, StringComparison.Ordinal);
        Assert.DoesNotContain("HistoryDocumentDelete", viewText, StringComparison.Ordinal);
        Assert.DoesNotContain("HistoryDocumentReactivate", viewText, StringComparison.Ordinal);
    }

    [Theory]
    [InlineData(1920, false)]
    [InlineData(1280, false)]
    [InlineData(1100, false)]
    [InlineData(1099, true)]
    [InlineData(960, true)]
    public void Editor_responsive_layout_uses_desktop_columns_until_threshold(
        double availableWidth,
        bool expectedStacked)
    {
        Assert.Equal(
            expectedStacked,
            ProductInsurancePolicyEditorView.ShouldUseStackedLayout(availableWidth));
    }

    [Fact]
    public void Editor_keeps_commands_right_of_title_on_desktop_and_stacks_them_when_narrow()
    {
        var document = LoadView("ProductInsurancePolicyEditorView.xaml");
        var commandBar = FindByAutomationId(document, "ProductInsurance_CommandBar");
        var commands = commandBar.Descendants(Presentation + "WrapPanel")
            .Single(element => element.Attribute(Xaml + "Name")?.Value == "HeaderCommands");
        var code = File.ReadAllText(ViewCodePath("ProductInsurancePolicyEditorView.xaml.cs"));

        Assert.Equal("1", AttachedValue(commands, "Grid.Column"));
        Assert.Contains("Grid.SetRow(HeaderCommands, 1)", code, StringComparison.Ordinal);
        Assert.Contains("Grid.SetColumn(HeaderCommands, 0)", code, StringComparison.Ordinal);
        Assert.Contains("Grid.SetRow(HeaderCommands, 0)", code, StringComparison.Ordinal);
        Assert.Contains("Grid.SetColumn(HeaderCommands, 1)", code, StringComparison.Ordinal);
    }

    [Fact]
    public void Policy_list_contains_required_summary_columns()
    {
        var text = File.ReadAllText(ViewPath("ProductPolicyContractsView.xaml"));

        Assert.Contains("{Binding DisplayTitle}", text, StringComparison.Ordinal);
        Assert.Contains("{Binding FamilyDisplayName}", text, StringComparison.Ordinal);
        Assert.Contains("{Binding InsurerName}", text, StringComparison.Ordinal);
        Assert.Contains("{Binding ProductCategory}", text, StringComparison.Ordinal);
        Assert.Contains("{Binding ContractStatus}", text, StringComparison.Ordinal);
        Assert.Contains("{Binding EnrollmentDate}", text, StringComparison.Ordinal);
        Assert.Contains("{Binding TotalPlannedPremiumAmount}", text, StringComparison.Ordinal);
    }

    [Theory]
    [InlineData("en-US")]
    [InlineData("ko-KR")]
    public void Policy_list_date_and_amount_are_culture_independent(string cultureName)
    {
        var originalCulture = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo(cultureName);
            var timestamp = DateTimeOffset.Parse("2026-08-05T00:00:00Z", CultureInfo.InvariantCulture);
            var policy = new PolicyRecord(
                Id: "policy_synthetic",
                DisplayTitle: "synthetic policy",
                ReferenceDate: null,
                CreatedAt: timestamp,
                UpdatedAt: timestamp,
                DisabledAt: null,
                FamilyMemberId: "family_synthetic",
                InsurerName: "synthetic insurer",
                ContractStatus: InsurancePolicyValues.ContractStatusActive,
                EnrollmentDate: new DateOnly(2026, 8, 5),
                CoveragePeriod: "2026-2036",
                RegistrationSource: InsurancePolicyValues.RegistrationSourceDirectInput,
                PremiumPaymentPeriod: "20년납",
                TotalPlannedPremiumAmount: 12_000_000m,
                RenewalType: InsurancePolicyValues.RenewalTypeFixed,
                RefundType: InsurancePolicyValues.RefundTypeRefundable,
                InsuranceBusinessType: InsurancePolicyValues.BusinessTypeLife,
                ProductCategory: InsurancePolicyValues.ProductCategoryCancer);
            var item = new InsurancePolicyListItemViewModel(
                policy,
                "synthetic family",
                "미등록",
                "기존 값 확인 필요");

            Assert.Equal("2026-08-05", item.EnrollmentDate);
            Assert.Equal("12,000,000원", item.TotalPlannedPremiumAmount);
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
        }
    }

    [Fact]
    public void Document_registration_uses_approved_reference_date_label_and_help()
    {
        var resourcePath = Path.Combine(
            FindProjectRoot(),
            "app",
            "FamilyClaimRef.App",
            "Resources",
            "UiStrings.xaml");
        var resources = XDocument.Load(resourcePath);
        var values = resources.Descendants()
            .Where(element => element.Attribute(Xaml + "Key") is not null)
            .ToDictionary(
                element => element.Attribute(Xaml + "Key")!.Value,
                element => element.Value,
                StringComparer.Ordinal);
        var viewText = File.ReadAllText(ViewPath("ProductDocumentRegistrationView.xaml"));

        Assert.Equal("문서 발급·조회 기준일", values["Ui.Document.ReferenceDateLabel"]);
        Assert.Equal(
            "문서에 표시된 발급일 또는 보험정보 조회 기준일입니다. 보험 가입일과는 다릅니다. 문서에 날짜가 없으면 비워두세요.",
            values["Ui.Document.ReferenceDateHelp"]);
        Assert.Equal("보험사 구분", values["Ui.Product.InsurancePolicy.BusinessTypeLabel"]);
        Assert.Contains("Ui.Document.ReferenceDateHelp", viewText, StringComparison.Ordinal);
    }

    private static XDocument LoadView(string fileName) => XDocument.Load(ViewPath(fileName));

    private static XElement FindByAutomationId(XDocument document, string automationId)
    {
        return document.Descendants()
            .Single(element => GetAutomationId(element) == automationId);
    }

    private static string? AttachedValue(XElement element, string localName)
    {
        return element.Attributes()
            .FirstOrDefault(attribute => attribute.Name.LocalName == localName)
            ?.Value;
    }

    private static string? GetAutomationId(XElement element)
    {
        return element.Attributes()
            .FirstOrDefault(attribute => attribute.Name.LocalName.EndsWith(
                ".AutomationId",
                StringComparison.Ordinal))
            ?.Value;
    }

    private static string ViewPath(string fileName)
    {
        return Path.Combine(
            FindProjectRoot(),
            "app",
            "FamilyClaimRef.App",
            "Views",
            fileName);
    }

    private static string ViewCodePath(string fileName)
    {
        return Path.Combine(
            FindProjectRoot(),
            "app",
            "FamilyClaimRef.App",
            "Views",
            fileName);
    }

    private static string FindProjectRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "FamilyClaimRef.sln")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException("FamilyClaimRef project root was not found.");
    }
}
