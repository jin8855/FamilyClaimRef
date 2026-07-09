using System.Windows;
using FamilyClaimRef.App.Services.Localization;
using Xunit;

namespace FamilyClaimRef.App.Tests.Services.Localization;

public sealed class ResourceUiTextProviderTests
{
    [Fact]
    public void Get_known_key_returns_expected_string()
    {
        var provider = CreateProvider();

        var text = provider.Get(UiTextKeys.AppTitle);

        Assert.Equal("FamilyClaimRef", text);
    }

    [Fact]
    public void Get_missing_key_returns_deterministic_fallback()
    {
        var provider = CreateProvider();

        var text = provider.Get("Ui.Missing.Key");

        Assert.Equal("[[Ui.Missing.Key]]", text);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Get_null_or_blank_key_rejects_clearly(string? key)
    {
        var provider = CreateProvider();

        var exception = Record.Exception(() => provider.Get(key!));

        Assert.NotNull(exception);
        Assert.IsType<ArgumentException>(exception);
    }

    [Fact]
    public void Format_known_key_formats_using_resource_value()
    {
        var resources = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["Ui.Test.Format"] = "Selected: {0}"
        };
        var provider = new ResourceUiTextProvider(resources);

        var text = provider.Format("Ui.Test.Format", "file.png");

        Assert.Equal("Selected: file.png", text);
    }

    [Fact]
    public void Pilot_keys_exist_in_UiTextKeys()
    {
        var keys = new[]
        {
            UiTextKeys.AppTitle,
            UiTextKeys.DocumentSourceFileSection,
            UiTextKeys.ActionSelectFile,
            UiTextKeys.StatusRegistrationSection,
            UiTextKeys.DevHarnessWarningLocalMvpValidation
        };

        Assert.All(keys, key => Assert.False(string.IsNullOrWhiteSpace(key)));
        Assert.Equal(keys.Length, keys.Distinct(StringComparer.Ordinal).Count());
    }

    [Fact]
    public void Runtime_message_keys_exist_in_UiTextKeys()
    {
        var keys = RuntimeMessageKeys;

        Assert.All(keys, key => Assert.False(string.IsNullOrWhiteSpace(key)));
        Assert.Equal(24, keys.Length);
        Assert.Equal(keys.Length, keys.Distinct(StringComparer.Ordinal).Count());
    }

    [Fact]
    public void Runtime_message_keys_resolve_from_UiStrings()
    {
        var resources = LoadUiStrings();
        var provider = new ResourceUiTextProvider(resources);

        Assert.All(RuntimeMessageKeys, key => Assert.False(string.IsNullOrWhiteSpace(provider.Get(key))));
    }

    [Fact]
    public void ResourceDictionary_source_returns_string_values()
    {
        var resources = new ResourceDictionary
        {
            [UiTextKeys.ActionSelectFile] = "파일 선택"
        };
        var provider = new ResourceUiTextProvider(resources);

        var text = provider.Get(UiTextKeys.ActionSelectFile);

        Assert.Equal("파일 선택", text);
    }

    [Theory]
    [InlineData(UiTextKeys.DocumentSourceFileSection, "원본 파일")]
    [InlineData(UiTextKeys.DocumentSelectedFileLabel, "선택한 파일")]
    [InlineData(UiTextKeys.DocumentMetadataSection, "문서 정보")]
    [InlineData(UiTextKeys.DocumentTypeLabel, "문서 유형")]
    [InlineData(UiTextKeys.DocumentDisplayTitleLabel, "표시 제목")]
    [InlineData(UiTextKeys.DocumentReferenceDateLabel, "기준일")]
    [InlineData(UiTextKeys.TargetSelectionSection, "저장 대상 선택")]
    [InlineData(UiTextKeys.TargetKindLabel, "대상 유형")]
    [InlineData(UiTextKeys.PolicyTargetLabel, "보험 대상")]
    [InlineData(UiTextKeys.ClaimTargetLabel, "청구 대상")]
    [InlineData(UiTextKeys.ActionSelectFile, "파일 선택")]
    [InlineData(UiTextKeys.ActionRegisterDocument, "등록")]
    [InlineData(UiTextKeys.ValidationSectionLabel, "입력 확인")]
    [InlineData(UiTextKeys.StatusRegistrationSection, "등록 상태")]
    [InlineData(UiTextKeys.StatusLabel, "상태")]
    [InlineData(UiTextKeys.StatusLastRegistrationSummaryLabel, "마지막 등록 요약")]
    [InlineData(UiTextKeys.DocumentRegistrationMessageNoActiveClaim, "선택할 수 있는 활성 청구 대상이 없습니다.")]
    [InlineData(UiTextKeys.DocumentRegistrationMessageNoActivePolicy, "선택할 수 있는 활성 보험 대상이 없습니다.")]
    [InlineData(UiTextKeys.DocumentRegistrationValidationSelectClaimBeforeRegister, "문서를 등록하기 전에 청구 대상을 선택해 주세요.")]
    [InlineData(UiTextKeys.DocumentRegistrationValidationSelectPolicyBeforeRegister, "문서를 등록하기 전에 보험 대상을 선택해 주세요.")]
    [InlineData(UiTextKeys.DocumentRegistrationValidationSelectTarget, "저장할 대상을 선택해 주세요.")]
    public void Approved_korean_copy_values_resolve_from_UiStrings(string key, string expected)
    {
        var resources = LoadUiStrings();
        var provider = new ResourceUiTextProvider(resources);

        var text = provider.Get(key);

        Assert.Equal(expected, text);
    }

    [Fact]
    public void ResourceDictionary_non_string_value_rejects_clearly()
    {
        var resources = new ResourceDictionary
        {
            ["Ui.Test.NonString"] = 123
        };
        var provider = new ResourceUiTextProvider(resources);

        var exception = Record.Exception(() => provider.Get("Ui.Test.NonString"));

        Assert.NotNull(exception);
        Assert.IsType<InvalidOperationException>(exception);
    }

    private static ResourceUiTextProvider CreateProvider()
    {
        return new ResourceUiTextProvider(new Dictionary<string, string>(StringComparer.Ordinal)
        {
            [UiTextKeys.AppTitle] = "FamilyClaimRef",
            [UiTextKeys.DocumentSourceFileSection] = "원본 파일",
            [UiTextKeys.ActionSelectFile] = "파일 선택",
            [UiTextKeys.StatusRegistrationSection] = "등록 상태",
            [UiTextKeys.DevHarnessWarningLocalMvpValidation] = "Local MVP validation screen."
        });
    }

    private static IReadOnlyDictionary<string, string> LoadUiStrings()
    {
        var path = Path.Combine(FindProjectRoot(), "app", "FamilyClaimRef.App", "Resources", "UiStrings.xaml");
        var document = System.Xml.Linq.XDocument.Load(path);
        var keyName = System.Xml.Linq.XName.Get("Key", "http://schemas.microsoft.com/winfx/2006/xaml");

        return document
            .Descendants()
            .Where(element => element.Attribute(keyName) is not null)
            .ToDictionary(
                element => element.Attribute(keyName)!.Value,
                element => element.Value,
                StringComparer.Ordinal);
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

    private static string[] RuntimeMessageKeys =>
    [
        UiTextKeys.DocumentRegistrationStatusCleanupFailed,
        UiTextKeys.DocumentRegistrationMessageNoActiveClaim,
        UiTextKeys.DocumentRegistrationMessageNoActivePolicy,
        UiTextKeys.DocumentRegistrationStatusFailed,
        UiTextKeys.DocumentRegistrationStatusCompleted,
        UiTextKeys.DocumentRegistrationValidationSelectClaimBeforeRegister,
        UiTextKeys.DocumentRegistrationValidationSelectPolicyBeforeRegister,
        UiTextKeys.DocumentRegistrationStatusFileSelected,
        UiTextKeys.DocumentRegistrationValidationSelectFile,
        UiTextKeys.DocumentRegistrationValidationSelectTargetKind,
        UiTextKeys.DocumentRegistrationValidationSelectTarget,
        UiTextKeys.DocumentRegistrationValidationSelectDocumentType,
        UiTextKeys.DocumentRegistrationValidationEnterDisplayTitle,
        UiTextKeys.DocumentRegistrationValidationSelectReferenceDate,
        UiTextKeys.ClaimManagementMessageCreated,
        UiTextKeys.ClaimManagementMessageDisabled,
        UiTextKeys.ClaimManagementValidationTitleRequired,
        UiTextKeys.PolicyManagementMessageCreated,
        UiTextKeys.PolicyManagementMessageDisabled,
        UiTextKeys.PolicyManagementValidationDisableBlockedByActiveClaims,
        UiTextKeys.ClaimManagementValidationSelectPolicyBeforeCreate,
        UiTextKeys.PolicyManagementValidationTitleRequired,
        UiTextKeys.ClaimManagementValidationSelectClaimTarget,
        UiTextKeys.PolicyManagementValidationSelectPolicyTarget
    ];
}
