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
    public void Product_copy_resources_match_approved_contract()
    {
        var resources = LoadUiStrings();

        Assert.Equal(116, resources.Count);
        Assert.Equal(60, resources.Keys.Count(IsProductKey));
        Assert.Equal(60, ExpectedProductResources.Count);
        Assert.All(
            ExpectedProductResources,
            expected => Assert.Equal(expected.Value, resources[expected.Key]));
    }

    [Fact]
    public void UiTextKeys_match_resource_keys_without_duplicates_or_gaps()
    {
        var resourceEntries = LoadUiStringEntries();
        var resourceKeys = resourceEntries.Select(entry => entry.Key).ToArray();
        var constantValues = LoadUiTextKeyConstants();

        Assert.Equal(116, resourceEntries.Count);
        Assert.Equal(resourceKeys.Length, resourceKeys.Distinct(StringComparer.Ordinal).Count());
        Assert.Equal(116, constantValues.Count);
        Assert.Equal(constantValues.Count, constantValues.Distinct(StringComparer.Ordinal).Count());
        Assert.Equal(60, constantValues.Count(IsProductKey));
        Assert.Equal(
            resourceKeys.OrderBy(key => key, StringComparer.Ordinal),
            constantValues.OrderBy(key => key, StringComparer.Ordinal));
    }

    [Fact]
    public void Existing_resource_values_are_preserved()
    {
        var existingResources = LoadUiStrings()
            .Where(resource => !IsProductKey(resource.Key))
            .ToDictionary(resource => resource.Key, resource => resource.Value, StringComparer.Ordinal);

        Assert.Equal(56, existingResources.Count);
        Assert.Equal("보험 대상", existingResources[UiTextKeys.PolicyTargetLabel]);
        Assert.Equal("청구 대상", existingResources[UiTextKeys.ClaimTargetLabel]);
        Assert.Equal(ExistingResourceFingerprint, ComputeResourceFingerprint(existingResources));
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
        Assert.Equal(47, keys.Length);
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
    [InlineData(UiTextKeys.DocumentRegistrationMessageNoActiveClaim, "선택할 수 있는 청구 건이 없습니다.")]
    [InlineData(UiTextKeys.DocumentRegistrationMessageNoActivePolicy, "선택할 수 있는 보험 계약이 없습니다.")]
    [InlineData(UiTextKeys.DocumentRegistrationValidationSelectClaimBeforeRegister, "문서를 등록하기 전에 연결할 청구 건을 선택해 주세요.")]
    [InlineData(UiTextKeys.DocumentRegistrationValidationSelectPolicyBeforeRegister, "문서를 등록하기 전에 연결할 보험 계약을 선택해 주세요.")]
    [InlineData(UiTextKeys.DocumentRegistrationValidationSelectTargetKind, "연결 대상 유형을 선택해 주세요.")]
    [InlineData(UiTextKeys.DocumentRegistrationValidationSelectTarget, "연결할 대상을 선택해 주세요.")]
    [InlineData(UiTextKeys.ProductDocumentRegistrationTargetSelectionSection, "연결 대상 선택")]
    [InlineData(UiTextKeys.ProductDocumentRegistrationPolicyTargetLabel, "보험 계약")]
    [InlineData(UiTextKeys.ProductDocumentRegistrationClaimTargetLabel, "청구 건")]
    [InlineData(UiTextKeys.ProductDocumentRegistrationValidationUnsupportedFileType, "지원하지 않는 파일 형식입니다.")]
    [InlineData(UiTextKeys.ProductDocumentRegistrationValidationEmptyFile, "빈 파일은 등록할 수 없습니다.")]
    [InlineData(UiTextKeys.ProductDocumentRegistrationValidationFileTooLarge, "파일 크기는 25MB 이하여야 합니다.")]
    [InlineData(UiTextKeys.ProductDocumentRegistrationValidationSourceUnavailable, "선택한 파일을 읽을 수 없습니다. 다시 선택해 주세요.")]
    [InlineData(UiTextKeys.ProductDocumentRegistrationValidationSourceChanged, "선택 후 파일이 변경되었습니다. 다시 선택해 주세요.")]
    [InlineData(UiTextKeys.ProductDocumentRegistrationValidationDuplicateDocument, "같은 대상에 동일한 문서가 이미 등록되어 있습니다.")]
    [InlineData(UiTextKeys.ProductDocumentRegistrationStatusCanceled, "파일 선택을 취소했습니다.")]
    [InlineData(UiTextKeys.ProductDocumentRegistrationStatusRetryAvailable, "입력 내용을 유지했습니다. 확인 후 다시 시도해 주세요.")]
    [InlineData(UiTextKeys.ProductDocumentListLoadFailedMessage, "문서 목록을 불러오지 못했습니다.")]
    [InlineData(UiTextKeys.ProductFamilyMemberDisplayNameLabel, "표시명")]
    [InlineData(UiTextKeys.ProductFamilyMemberRelationLabel, "관계")]
    [InlineData(UiTextKeys.ProductFamilyMemberMemoLabel, "메모")]
    [InlineData(UiTextKeys.ProductFamilyMemberActiveStateLabel, "사용 여부")]
    [InlineData(UiTextKeys.ProductFamilyMemberActiveListLabel, "가족 목록")]
    [InlineData(UiTextKeys.ProductFamilyMemberEmptyMessage, "등록된 가족 정보가 없습니다.")]
    [InlineData(UiTextKeys.ProductFamilyMemberLoadFailedMessage, "가족 목록을 불러오지 못했습니다. 다시 시도해 주세요.")]
    [InlineData(UiTextKeys.ProductFamilyMemberSavedMessage, "가족 정보를 저장했습니다.")]
    [InlineData(UiTextKeys.ProductFamilyMemberDeactivatedMessage, "가족 정보를 사용 중지했습니다.")]
    [InlineData(UiTextKeys.ProductFamilyMemberReactivateAction, "다시 사용")]
    [InlineData(UiTextKeys.ProductFamilyMemberReactivatedMessage, "가족 정보를 다시 사용하도록 변경했습니다.")]
    [InlineData(UiTextKeys.ProductFamilyMemberDisplayNameRequiredMessage, "표시명을 입력해 주세요.")]
    [InlineData(UiTextKeys.ProductFamilyMemberRelationRequiredMessage, "관계를 선택해 주세요.")]
    [InlineData(UiTextKeys.ProductFamilyMemberConflictMessage, "다른 변경이 반영되었습니다. 목록을 다시 불러온 뒤 시도해 주세요.")]
    [InlineData(UiTextKeys.ProductFamilyMemberTargetUnavailableMessage, "처리할 가족 정보를 찾을 수 없습니다. 목록을 다시 확인해 주세요.")]
    [InlineData(UiTextKeys.ProductFamilyMemberOperationFailedMessage, "가족 정보를 처리하지 못했습니다. 다시 시도해 주세요.")]
    [InlineData(UiTextKeys.ProductFamilyMemberSavedRefreshFailedMessage, "저장은 완료되었지만 목록을 새로고치지 못했습니다. 다시 불러와 주세요.")]
    [InlineData(UiTextKeys.ClaimManagementMessageCreated, "청구 건을 등록했습니다.")]
    [InlineData(UiTextKeys.ClaimManagementMessageDisabled, "청구 건을 사용 중지했습니다.")]
    [InlineData(UiTextKeys.ClaimManagementValidationTitleRequired, "청구 건 이름을 입력해 주세요.")]
    [InlineData(UiTextKeys.PolicyManagementMessageCreated, "보험 계약을 등록했습니다.")]
    [InlineData(UiTextKeys.PolicyManagementMessageDisabled, "보험 계약을 사용 중지했습니다.")]
    [InlineData(UiTextKeys.PolicyManagementValidationDisableBlockedByActiveClaims, "활성 청구 건이 있어 보험 계약을 사용 중지할 수 없습니다. 청구 건을 먼저 사용 중지해 주세요.")]
    [InlineData(UiTextKeys.ClaimManagementValidationSelectPolicyBeforeCreate, "청구 건을 등록할 보험 계약을 선택해 주세요.")]
    [InlineData(UiTextKeys.PolicyManagementValidationTitleRequired, "보험 계약 이름을 입력해 주세요.")]
    [InlineData(UiTextKeys.ClaimManagementValidationSelectClaimTarget, "사용 중지할 청구 건을 선택해 주세요.")]
    [InlineData(UiTextKeys.PolicyManagementValidationSelectPolicyTarget, "사용 중지할 보험 계약을 선택해 주세요.")]
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
        return LoadUiStringEntries().ToDictionary(
            entry => entry.Key,
            entry => entry.Value,
            StringComparer.Ordinal);
    }

    private static IReadOnlyList<KeyValuePair<string, string>> LoadUiStringEntries()
    {
        var path = Path.Combine(FindProjectRoot(), "app", "FamilyClaimRef.App", "Resources", "UiStrings.xaml");
        var document = System.Xml.Linq.XDocument.Load(path);
        var keyName = System.Xml.Linq.XName.Get("Key", "http://schemas.microsoft.com/winfx/2006/xaml");

        return document
            .Descendants()
            .Where(element => element.Attribute(keyName) is not null)
            .Select(element => new KeyValuePair<string, string>(
                element.Attribute(keyName)!.Value,
                element.Value))
            .ToArray();
    }

    private static IReadOnlyList<string> LoadUiTextKeyConstants()
    {
        return typeof(UiTextKeys)
            .GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
            .Where(field => field.IsLiteral && !field.IsInitOnly && field.FieldType == typeof(string))
            .Select(field => (string)field.GetRawConstantValue()!)
            .ToArray();
    }

    private static string ComputeResourceFingerprint(IReadOnlyDictionary<string, string> resources)
    {
        var snapshot = string.Join(
            ((char)30).ToString(),
            resources
                .OrderBy(resource => resource.Key, StringComparer.Ordinal)
                .Select(resource => $"{resource.Key}{(char)31}{resource.Value}"));
        var bytes = System.Text.Encoding.UTF8.GetBytes(snapshot);

        using var sha256 = System.Security.Cryptography.SHA256.Create();
        return Convert.ToHexString(sha256.ComputeHash(bytes));
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
        UiTextKeys.PolicyManagementValidationSelectPolicyTarget,
        UiTextKeys.ProductManagementLoadFailedMessage,
        UiTextKeys.ProductPolicyContractsOperationFailedMessage,
        UiTextKeys.ProductClaimCasesOperationFailedMessage,
        UiTextKeys.ProductPolicyContractsDuplicateTitleMessage,
        UiTextKeys.ProductClaimCasesDuplicateTitleMessage
        ,
        UiTextKeys.ProductDocumentRegistrationValidationUnsupportedFileType,
        UiTextKeys.ProductDocumentRegistrationValidationEmptyFile,
        UiTextKeys.ProductDocumentRegistrationValidationFileTooLarge,
        UiTextKeys.ProductDocumentRegistrationValidationSourceUnavailable,
        UiTextKeys.ProductDocumentRegistrationValidationSourceChanged,
        UiTextKeys.ProductDocumentRegistrationValidationDuplicateDocument,
        UiTextKeys.ProductDocumentRegistrationStatusCanceled,
        UiTextKeys.ProductDocumentRegistrationStatusRetryAvailable
        ,
        UiTextKeys.ProductFamilyMemberLoadFailedMessage,
        UiTextKeys.ProductFamilyMemberSavedMessage,
        UiTextKeys.ProductFamilyMemberDeactivatedMessage,
        UiTextKeys.ProductFamilyMemberReactivatedMessage,
        UiTextKeys.ProductFamilyMemberDisplayNameRequiredMessage,
        UiTextKeys.ProductFamilyMemberRelationRequiredMessage,
        UiTextKeys.ProductFamilyMemberConflictMessage,
        UiTextKeys.ProductFamilyMemberTargetUnavailableMessage,
        UiTextKeys.ProductFamilyMemberOperationFailedMessage,
        UiTextKeys.ProductFamilyMemberSavedRefreshFailedMessage
    ];

    private static bool IsProductKey(string key) => key.StartsWith("Ui.Product.", StringComparison.Ordinal);

    private static IReadOnlyDictionary<string, string> ExpectedProductResources { get; } =
        new Dictionary<string, string>(StringComparer.Ordinal)
        {
            [UiTextKeys.ProductShellTitle] = "FamilyClaimRef",
            [UiTextKeys.ProductNavigationHome] = "홈",
            [UiTextKeys.ProductNavigationPolicyContracts] = "보험 계약",
            [UiTextKeys.ProductNavigationClaimCases] = "청구 건",
            [UiTextKeys.ProductNavigationDocumentRegistration] = "문서 등록",
            [UiTextKeys.ProductNavigationDocumentList] = "문서 목록",
            [UiTextKeys.ProductHomeTitle] = "홈",
            [UiTextKeys.ProductPolicyContractsTitle] = "보험 계약",
            [UiTextKeys.ProductClaimCasesTitle] = "청구 건",
            [UiTextKeys.ProductPolicyContractsEmptyMessage] = "등록된 보험 계약이 없습니다.",
            [UiTextKeys.ProductClaimCasesEmptyMessage] = "등록된 청구 건이 없습니다.",
            [UiTextKeys.ProductPolicyContractsCreationSection] = "보험 계약 등록",
            [UiTextKeys.ProductClaimCasesCreationSection] = "청구 건 등록",
            [UiTextKeys.ProductPolicyContractsActiveListLabel] = "보험 계약 목록",
            [UiTextKeys.ProductClaimCasesActiveListLabel] = "청구 건 목록",
            [UiTextKeys.ProductPolicyContractsDisplayTitleLabel] = "보험 계약 이름",
            [UiTextKeys.ProductClaimCasesDisplayTitleLabel] = "청구 건 이름",
            [UiTextKeys.ProductClaimCasesPolicyLabel] = "보험 계약",
            [UiTextKeys.ProductPolicyContractsCreateAction] = "보험 계약 등록",
            [UiTextKeys.ProductPolicyContractsDisableAction] = "보험 계약 사용 중지",
            [UiTextKeys.ProductClaimCasesCreateAction] = "청구 건 등록",
            [UiTextKeys.ProductClaimCasesDisableAction] = "청구 건 사용 중지",
            [UiTextKeys.ProductManagementStatusLabel] = "처리 결과",
            [UiTextKeys.ProductManagementLoadFailedMessage] =
                "목록을 불러오지 못했습니다. 다시 시도해 주세요.",
            [UiTextKeys.ProductPolicyContractsOperationFailedMessage] =
                "보험 계약을 처리하지 못했습니다. 다시 시도해 주세요.",
            [UiTextKeys.ProductClaimCasesOperationFailedMessage] =
                "청구 건을 처리하지 못했습니다. 다시 시도해 주세요.",
            [UiTextKeys.ProductPolicyContractsDuplicateTitleMessage] =
                "같은 이름의 활성 보험 계약이 이미 있습니다.",
            [UiTextKeys.ProductClaimCasesDuplicateTitleMessage] =
                "같은 이름의 활성 청구 건이 이미 있습니다.",
            [UiTextKeys.ProductDocumentRegistrationTitle] = "문서 등록",
            [UiTextKeys.ProductDocumentRegistrationTargetSelectionSection] = "연결 대상 선택",
            [UiTextKeys.ProductDocumentRegistrationPolicyTargetLabel] = "보험 계약",
            [UiTextKeys.ProductDocumentRegistrationClaimTargetLabel] = "청구 건",
            [UiTextKeys.ProductDocumentRegistrationValidationUnsupportedFileType] =
                "지원하지 않는 파일 형식입니다.",
            [UiTextKeys.ProductDocumentRegistrationValidationEmptyFile] =
                "빈 파일은 등록할 수 없습니다.",
            [UiTextKeys.ProductDocumentRegistrationValidationFileTooLarge] =
                "파일 크기는 25MB 이하여야 합니다.",
            [UiTextKeys.ProductDocumentRegistrationValidationSourceUnavailable] =
                "선택한 파일을 읽을 수 없습니다. 다시 선택해 주세요.",
            [UiTextKeys.ProductDocumentRegistrationValidationSourceChanged] =
                "선택 후 파일이 변경되었습니다. 다시 선택해 주세요.",
            [UiTextKeys.ProductDocumentRegistrationValidationDuplicateDocument] =
                "같은 대상에 동일한 문서가 이미 등록되어 있습니다.",
            [UiTextKeys.ProductDocumentRegistrationStatusCanceled] =
                "파일 선택을 취소했습니다.",
            [UiTextKeys.ProductDocumentRegistrationStatusRetryAvailable] =
                "입력 내용을 유지했습니다. 확인 후 다시 시도해 주세요.",
            [UiTextKeys.ProductDocumentListTitle] = "문서 목록",
            [UiTextKeys.ProductDocumentListEmptyMessage] = "등록된 문서가 없습니다.",
            [UiTextKeys.ProductDocumentListLoadFailedMessage] = "문서 목록을 불러오지 못했습니다.",
            [UiTextKeys.ProductFamilyMemberDisplayNameLabel] = "표시명",
            [UiTextKeys.ProductFamilyMemberRelationLabel] = "관계",
            [UiTextKeys.ProductFamilyMemberMemoLabel] = "메모",
            [UiTextKeys.ProductFamilyMemberActiveStateLabel] = "사용 여부",
            [UiTextKeys.ProductFamilyMemberActiveListLabel] = "가족 목록",
            [UiTextKeys.ProductFamilyMemberEmptyMessage] = "등록된 가족 정보가 없습니다.",
            [UiTextKeys.ProductFamilyMemberLoadFailedMessage] =
                "가족 목록을 불러오지 못했습니다. 다시 시도해 주세요.",
            [UiTextKeys.ProductFamilyMemberSavedMessage] = "가족 정보를 저장했습니다.",
            [UiTextKeys.ProductFamilyMemberDeactivatedMessage] = "가족 정보를 사용 중지했습니다.",
            [UiTextKeys.ProductFamilyMemberReactivateAction] = "다시 사용",
            [UiTextKeys.ProductFamilyMemberReactivatedMessage] =
                "가족 정보를 다시 사용하도록 변경했습니다.",
            [UiTextKeys.ProductFamilyMemberDisplayNameRequiredMessage] = "표시명을 입력해 주세요.",
            [UiTextKeys.ProductFamilyMemberRelationRequiredMessage] = "관계를 선택해 주세요.",
            [UiTextKeys.ProductFamilyMemberConflictMessage] =
                "다른 변경이 반영되었습니다. 목록을 다시 불러온 뒤 시도해 주세요.",
            [UiTextKeys.ProductFamilyMemberTargetUnavailableMessage] =
                "처리할 가족 정보를 찾을 수 없습니다. 목록을 다시 확인해 주세요.",
            [UiTextKeys.ProductFamilyMemberOperationFailedMessage] =
                "가족 정보를 처리하지 못했습니다. 다시 시도해 주세요.",
            [UiTextKeys.ProductFamilyMemberSavedRefreshFailedMessage] =
                "저장은 완료되었지만 목록을 새로고치지 못했습니다. 다시 불러와 주세요."
        };

    private const string ExistingResourceFingerprint =
        "3854B89745899CE5F331C3E4AD8A706F155F3001734B9A600D752253D44905D4";
}
