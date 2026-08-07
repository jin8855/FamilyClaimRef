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

        Assert.Equal(214, resources.Count);
        Assert.Equal(157, resources.Keys.Count(IsProductKey));
        Assert.Equal(157, ExpectedProductResources.Count);
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

        Assert.Equal(214, resourceEntries.Count);
        Assert.Equal(resourceKeys.Length, resourceKeys.Distinct(StringComparer.Ordinal).Count());
        Assert.Equal(214, constantValues.Count);
        Assert.Equal(constantValues.Count, constantValues.Distinct(StringComparer.Ordinal).Count());
        Assert.Equal(157, constantValues.Count(IsProductKey));
        Assert.Equal(
            resourceKeys.OrderBy(key => key, StringComparer.Ordinal),
            constantValues.OrderBy(key => key, StringComparer.Ordinal));
    }

    [Fact]
    public void Non_product_resource_values_match_reference_date_revision_baseline()
    {
        var existingResources = LoadUiStrings()
            .Where(resource => !IsProductKey(resource.Key))
            .ToDictionary(resource => resource.Key, resource => resource.Value, StringComparer.Ordinal);

        Assert.Equal(57, existingResources.Count);
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
        Assert.Equal(53, keys.Length);
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
    [InlineData(UiTextKeys.DocumentReferenceDateLabel, "문서 발급·조회 기준일")]
    [InlineData(UiTextKeys.DocumentReferenceDateHelp, "문서에 표시된 발급일 또는 보험정보 조회 기준일입니다. 보험 가입일과는 다릅니다. 문서에 날짜가 없으면 비워두세요.")]
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
    [InlineData(UiTextKeys.ProductInsurancePolicyFamilyLabel, "가족")]
    [InlineData(UiTextKeys.ProductInsurancePolicyInsurerLabel, "보험사")]
    [InlineData(UiTextKeys.ProductInsurancePolicyContractStatusLabel, "계약 상태")]
    [InlineData(UiTextKeys.ProductInsurancePolicyEnrollmentDateLabel, "가입일")]
    [InlineData(UiTextKeys.ProductInsurancePolicyCoveragePeriodLabel, "보험기간")]
    [InlineData(UiTextKeys.ProductInsurancePolicyPremiumPaymentPeriodLabel, "보험료 납입기간")]
    [InlineData(UiTextKeys.ProductInsurancePolicyTotalPlannedPremiumAmountLabel, "납입액")]
    [InlineData(UiTextKeys.ProductInsurancePolicyCurrencySuffix, "원")]
    [InlineData(UiTextKeys.ProductInsurancePolicyRenewalTypeLabel, "갱신 유형")]
    [InlineData(UiTextKeys.ProductInsurancePolicyRefundTypeLabel, "환급 유형")]
    [InlineData(UiTextKeys.ProductInsurancePolicyBusinessTypeLabel, "보험사 구분")]
    [InlineData(UiTextKeys.ProductInsurancePolicyProductCategoryLabel, "상품 구분")]
    [InlineData(UiTextKeys.ProductInsurancePolicyRegistrationSourceLabel, "등록 출처")]
    [InlineData(UiTextKeys.ProductInsurancePolicyBasicInformationSection, "기본정보")]
    [InlineData(UiTextKeys.ProductInsurancePolicyCoveragePaymentSection, "보장·납입정보")]
    [InlineData(UiTextKeys.ProductInsurancePolicyClassificationSection, "보험 분류")]
    [InlineData(UiTextKeys.ProductInsurancePolicyRegistrationInformationSection, "등록정보")]
    [InlineData(UiTextKeys.ProductInsurancePolicySelectionRequired, "선택 필요")]
    [InlineData(UiTextKeys.ProductInsurancePolicyLegacyValueReviewRequired, "기존 값 확인 필요")]
    [InlineData(UiTextKeys.ProductInsurancePolicyUnregisteredValue, "미등록")]
    [InlineData(UiTextKeys.ProductInsurancePolicyLoadFailedMessage, "보험 계약 목록을 불러오지 못했습니다. 다시 시도해 주세요.")]
    [InlineData(UiTextKeys.ProductInsurancePolicySavedMessage, "보험 계약 정보를 저장했습니다.")]
    [InlineData(UiTextKeys.ProductInsurancePolicyRequiredFieldsMessage, "보험 계약 정보를 모두 입력해 주세요.")]
    [InlineData(UiTextKeys.ProductInsurancePolicyTargetUnavailableMessage, "처리할 보험 계약을 찾을 수 없습니다. 목록을 다시 확인해 주세요.")]
    [InlineData(UiTextKeys.ProductInsurancePolicyFamilyUnavailableMessage, "연결된 가족 정보를 찾을 수 없습니다. 저장하려면 가족을 다시 선택해 주세요.")]
    [InlineData(UiTextKeys.ProductInsurancePolicyFamilyUnavailableValue, "연결 확인 필요")]
    [InlineData(UiTextKeys.ProductInsurancePolicyOperationFailedMessage, "보험 계약을 처리하지 못했습니다. 다시 시도해 주세요.")]
    [InlineData(UiTextKeys.ProductInsurancePolicyTemporarySaveAction, "임시저장")]
    [InlineData(UiTextKeys.ProductInsurancePolicySummarySectionTitle, "보험 요약 정보")]
    [InlineData(UiTextKeys.ProductInsurancePolicyLinkedDocumentsSectionTitle, "이 보험에 연결할 문서")]
    [InlineData(UiTextKeys.ProductInsurancePolicyLinkedDocumentsGuidance, "보험 문서는 보험 기본정보를 저장한 뒤 연결합니다. 약관, 계약서, 보험증권은 이 보험에 종속된 문서로 관리합니다.")]
    [InlineData(UiTextKeys.ProductInsurancePolicyDocumentTypeHeader, "문서 유형")]
    [InlineData(UiTextKeys.ProductInsurancePolicyDocumentStatusHeader, "상태")]
    [InlineData(UiTextKeys.ProductInsurancePolicyDocumentNextActionHeader, "다음 작업")]
    [InlineData(UiTextKeys.ProductInsurancePolicyDocumentCaptureType, "보험 조회 캡처")]
    [InlineData(UiTextKeys.ProductInsurancePolicyDocumentPolicyType, "보험증권/계약서")]
    [InlineData(UiTextKeys.ProductInsurancePolicyDocumentTermsType, "약관 PDF/DOCX")]
    [InlineData(UiTextKeys.ProductInsurancePolicyDocumentCreateStatus, "보험 저장 후 등록 가능")]
    [InlineData(UiTextKeys.ProductInsurancePolicyDocumentNotRegisteredStatus, "미등록")]
    [InlineData(UiTextKeys.ProductInsurancePolicyDocumentStatusUnavailable, "문서 상태를 불러오지 못했습니다.")]
    [InlineData(UiTextKeys.ProductInsurancePolicyDocumentStatusGuidance, "문서 유형별 활성 연결은 1건만 유지하며, 다시 등록하거나 연결을 해제해도 이전 이력과 파일은 보존됩니다.")]
    [InlineData(UiTextKeys.ProductInsurancePolicyDocumentRegisterAction, "문서 등록")]
    [InlineData(UiTextKeys.ProductInsurancePolicyDocumentOpenAction, "문서 열기")]
    [InlineData(UiTextKeys.ProductInsurancePolicyDocumentReplaceAction, "다시 등록")]
    [InlineData(UiTextKeys.ProductInsurancePolicyDocumentUnlinkAction, "연결 해제")]
    [InlineData(UiTextKeys.ProductInsurancePolicyDocumentUnlinkConfirmationTitle, "문서 연결 해제")]
    [InlineData(UiTextKeys.ProductInsurancePolicyDocumentUnlinkConfirmationMessage, "이 보험에서 선택한 문서 연결을 해제하시겠습니까? 문서 이력과 파일은 삭제되지 않습니다.")]
    [InlineData(UiTextKeys.ProductInsurancePolicyDocumentUnlinkedMessage, "문서 연결을 해제했습니다.")]
    [InlineData(UiTextKeys.ProductInsurancePolicyDocumentOpenFailedMessage, "문서를 열지 못했습니다. 다시 시도해 주세요.")]
    [InlineData(UiTextKeys.ProductInsurancePolicyDocumentUnlinkFailedMessage, "문서 연결을 해제하지 못했습니다. 다시 시도해 주세요.")]
    [InlineData(UiTextKeys.ProductInsurancePolicyDocumentHistoryHeaderFormat, "문서 이력 보기 ({0}건)")]
    [InlineData(UiTextKeys.ProductInsurancePolicyDocumentHistoryTitleHeader, "문서 제목")]
    [InlineData(UiTextKeys.ProductInsurancePolicyDocumentHistoryRegisteredAtHeader, "등록일시")]
    [InlineData(UiTextKeys.ProductInsurancePolicyDocumentHistoryCurrentStatus, "현재")]
    [InlineData(UiTextKeys.ProductInsurancePolicyDocumentHistoryArchivedStatus, "이력")]
    [InlineData(UiTextKeys.ProductInsurancePolicyCoverageCandidatesSectionTitle, "담보 후보 확인")]
    [InlineData(UiTextKeys.ProductInsurancePolicyCoverageCandidatesGuidance, "담보 후보는 약관 또는 계약서 문서를 연결하고 사용자가 확인한 뒤 표시합니다.")]
    [InlineData(UiTextKeys.ProductInsurancePolicyCoverageCandidateHeader, "담보/특약 후보")]
    [InlineData(UiTextKeys.ProductInsurancePolicyCareTypeCandidateHeader, "진료구분 후보")]
    [InlineData(UiTextKeys.ProductInsurancePolicyCostTypeCandidateHeader, "비용구분 후보")]
    [InlineData(UiTextKeys.ProductInsurancePolicyKeywordTagHeader, "키워드/태그")]
    [InlineData(UiTextKeys.ProductInsurancePolicyReviewRequiredHeader, "확인 필요")]
    [InlineData(UiTextKeys.ProductInsurancePolicyCoverageCandidatesEmptyMessage, "연결 문서를 확인한 뒤 담보 후보를 표시합니다. 현재 자동 분석은 실행하지 않습니다.")]
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
        UiTextKeys.ProductFamilyMemberSavedRefreshFailedMessage,
        UiTextKeys.ProductInsurancePolicyLoadFailedMessage,
        UiTextKeys.ProductInsurancePolicySavedMessage,
        UiTextKeys.ProductInsurancePolicyRequiredFieldsMessage,
        UiTextKeys.ProductInsurancePolicyTargetUnavailableMessage,
        UiTextKeys.ProductInsurancePolicyFamilyUnavailableMessage,
        UiTextKeys.ProductInsurancePolicyOperationFailedMessage
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
                "저장은 완료되었지만 목록을 새로고치지 못했습니다. 다시 불러와 주세요.",
            [UiTextKeys.ProductInsurancePolicyFamilyLabel] = "가족",
            [UiTextKeys.ProductInsurancePolicyInsurerLabel] = "보험사",
            [UiTextKeys.ProductInsurancePolicyContractStatusLabel] = "계약 상태",
            [UiTextKeys.ProductInsurancePolicyEnrollmentDateLabel] = "가입일",
            [UiTextKeys.ProductInsurancePolicyCoveragePeriodLabel] = "보험기간",
            [UiTextKeys.ProductInsurancePolicyPremiumPaymentPeriodLabel] = "보험료 납입기간",
            [UiTextKeys.ProductInsurancePolicyTotalPlannedPremiumAmountLabel] = "납입액",
            [UiTextKeys.ProductInsurancePolicyCurrencySuffix] = "원",
            [UiTextKeys.ProductInsurancePolicyRenewalTypeLabel] = "갱신 유형",
            [UiTextKeys.ProductInsurancePolicyRefundTypeLabel] = "환급 유형",
            [UiTextKeys.ProductInsurancePolicyBusinessTypeLabel] = "보험사 구분",
            [UiTextKeys.ProductInsurancePolicyProductCategoryLabel] = "상품 구분",
            [UiTextKeys.ProductInsurancePolicyRegistrationSourceLabel] = "등록 출처",
            [UiTextKeys.ProductInsurancePolicyBasicInformationSection] = "기본정보",
            [UiTextKeys.ProductInsurancePolicyCoveragePaymentSection] = "보장·납입정보",
            [UiTextKeys.ProductInsurancePolicyClassificationSection] = "보험 분류",
            [UiTextKeys.ProductInsurancePolicyRegistrationInformationSection] = "등록정보",
            [UiTextKeys.ProductInsurancePolicySelectionRequired] = "선택 필요",
            [UiTextKeys.ProductInsurancePolicyLegacyValueReviewRequired] = "기존 값 확인 필요",
            [UiTextKeys.ProductInsurancePolicyUnregisteredValue] = "미등록",
            [UiTextKeys.ProductInsurancePolicyLoadFailedMessage] =
                "보험 계약 목록을 불러오지 못했습니다. 다시 시도해 주세요.",
            [UiTextKeys.ProductInsurancePolicySavedMessage] = "보험 계약 정보를 저장했습니다.",
            [UiTextKeys.ProductInsurancePolicyRequiredFieldsMessage] =
                "보험 계약 정보를 모두 입력해 주세요.",
            [UiTextKeys.ProductInsurancePolicyTargetUnavailableMessage] =
                "처리할 보험 계약을 찾을 수 없습니다. 목록을 다시 확인해 주세요.",
            [UiTextKeys.ProductInsurancePolicyFamilyUnavailableMessage] =
                "연결된 가족 정보를 찾을 수 없습니다. 저장하려면 가족을 다시 선택해 주세요.",
            [UiTextKeys.ProductInsurancePolicyFamilyUnavailableValue] = "연결 확인 필요",
            [UiTextKeys.ProductInsurancePolicyOperationFailedMessage] =
                "보험 계약을 처리하지 못했습니다. 다시 시도해 주세요.",
            [UiTextKeys.ProductInsurancePolicyTemporarySaveAction] = "임시저장",
            [UiTextKeys.ProductInsurancePolicySummarySectionTitle] = "보험 요약 정보",
            [UiTextKeys.ProductInsurancePolicyLinkedDocumentsSectionTitle] = "이 보험에 연결할 문서",
            [UiTextKeys.ProductInsurancePolicyLinkedDocumentsGuidance] =
                "보험 문서는 보험 기본정보를 저장한 뒤 연결합니다. 약관, 계약서, 보험증권은 이 보험에 종속된 문서로 관리합니다.",
            [UiTextKeys.ProductInsurancePolicyDocumentTypeHeader] = "문서 유형",
            [UiTextKeys.ProductInsurancePolicyDocumentStatusHeader] = "상태",
            [UiTextKeys.ProductInsurancePolicyDocumentNextActionHeader] = "다음 작업",
            [UiTextKeys.ProductInsurancePolicyDocumentCaptureType] = "보험 조회 캡처",
            [UiTextKeys.ProductInsurancePolicyDocumentPolicyType] = "보험증권/계약서",
            [UiTextKeys.ProductInsurancePolicyDocumentTermsType] = "약관 PDF/DOCX",
            [UiTextKeys.ProductInsurancePolicyDocumentCreateStatus] = "보험 저장 후 등록 가능",
            [UiTextKeys.ProductInsurancePolicyDocumentNotRegisteredStatus] = "미등록",
            [UiTextKeys.ProductInsurancePolicyDocumentStatusUnavailable] =
                "문서 상태를 불러오지 못했습니다.",
            [UiTextKeys.ProductInsurancePolicyDocumentStatusGuidance] =
                "문서 유형별 활성 연결은 1건만 유지하며, 다시 등록하거나 연결을 해제해도 이전 이력과 파일은 보존됩니다.",
            [UiTextKeys.ProductInsurancePolicyDocumentRegisterAction] = "문서 등록",
            [UiTextKeys.ProductInsurancePolicyDocumentOpenAction] = "문서 열기",
            [UiTextKeys.ProductInsurancePolicyDocumentReplaceAction] = "다시 등록",
            [UiTextKeys.ProductInsurancePolicyDocumentUnlinkAction] = "연결 해제",
            [UiTextKeys.ProductInsurancePolicyDocumentUnlinkConfirmationTitle] = "문서 연결 해제",
            [UiTextKeys.ProductInsurancePolicyDocumentUnlinkConfirmationMessage] =
                "이 보험에서 선택한 문서 연결을 해제하시겠습니까? 문서 이력과 파일은 삭제되지 않습니다.",
            [UiTextKeys.ProductInsurancePolicyDocumentUnlinkedMessage] = "문서 연결을 해제했습니다.",
            [UiTextKeys.ProductInsurancePolicyDocumentOpenFailedMessage] =
                "문서를 열지 못했습니다. 다시 시도해 주세요.",
            [UiTextKeys.ProductInsurancePolicyDocumentUnlinkFailedMessage] =
                "문서 연결을 해제하지 못했습니다. 다시 시도해 주세요.",
            [UiTextKeys.ProductInsurancePolicyDocumentHistoryHeaderFormat] =
                "문서 이력 보기 ({0}건)",
            [UiTextKeys.ProductInsurancePolicyDocumentHistoryTitleHeader] = "문서 제목",
            [UiTextKeys.ProductInsurancePolicyDocumentHistoryRegisteredAtHeader] = "등록일시",
            [UiTextKeys.ProductInsurancePolicyDocumentHistoryCurrentStatus] = "현재",
            [UiTextKeys.ProductInsurancePolicyDocumentHistoryArchivedStatus] = "이력",
            [UiTextKeys.ProductInsurancePolicyCoverageCandidatesSectionTitle] = "담보 후보 확인",
            [UiTextKeys.ProductInsurancePolicyCoverageCandidatesGuidance] =
                "담보 후보는 약관 또는 계약서 문서를 연결하고 사용자가 확인한 뒤 표시합니다.",
            [UiTextKeys.ProductInsurancePolicyCoverageCandidateHeader] = "담보/특약 후보",
            [UiTextKeys.ProductInsurancePolicyCareTypeCandidateHeader] = "진료구분 후보",
            [UiTextKeys.ProductInsurancePolicyCostTypeCandidateHeader] = "비용구분 후보",
            [UiTextKeys.ProductInsurancePolicyKeywordTagHeader] = "키워드/태그",
            [UiTextKeys.ProductInsurancePolicyReviewRequiredHeader] = "확인 필요",
            [UiTextKeys.ProductInsurancePolicyCoverageCandidatesEmptyMessage] =
                "연결 문서를 확인한 뒤 담보 후보를 표시합니다. 현재 자동 분석은 실행하지 않습니다.",
            [UiTextKeys.ProductCategoryGuidance] =
                "분류는 전체에서 고유한 코드를 사용하며, 항목 코드는 같은 분류 안에서만 고유합니다. 선택한 행의 식별자와 현재 버전으로만 변경합니다.",
            [UiTextKeys.ProductCategoryCategoryListTitle] = "분류 목록",
            [UiTextKeys.ProductCategoryItemListTitle] = "선택한 분류의 항목 목록",
            [UiTextKeys.ProductCategoryNameLabel] = "분류명",
            [UiTextKeys.ProductCategoryItemNameLabel] = "항목명",
            [UiTextKeys.ProductCategoryCodeLabel] = "코드",
            [UiTextKeys.ProductCategoryStateLabel] = "상태",
            [UiTextKeys.ProductCategoryItemCountLabel] = "항목 수",
            [UiTextKeys.ProductCategoryActionsLabel] = "작업",
            [UiTextKeys.ProductCategorySortOrderLabel] = "정렬 순서",
            [UiTextKeys.ProductCategoryDescriptionLabel] = "설명",
            [UiTextKeys.ProductCategorySystemDefaultLabel] = "시스템 기본값",
            [UiTextKeys.ProductCategoryParentLabel] = "상위 분류",
            [UiTextKeys.ProductCategoryPolicySearchLabel] = "보험 검색에 사용",
            [UiTextKeys.ProductCategoryHistorySearchLabel] = "이력 검색에 사용",
            [UiTextKeys.ProductCategoryActiveValue] = "사용 중",
            [UiTextKeys.ProductCategoryInactiveValue] = "사용 중지",
            [UiTextKeys.ProductCategoryRegisterCategoryAction] = "분류 등록",
            [UiTextKeys.ProductCategoryRegisterItemAction] = "항목 등록",
            [UiTextKeys.ProductCategoryReactivateAction] = "다시 사용",
            [UiTextKeys.ProductCategoryLoadFailedMessage] =
                "분류와 항목 목록을 불러오지 못했습니다. 다시 시도해 주세요.",
            [UiTextKeys.ProductCategorySavedMessage] = "분류를 저장했습니다.",
            [UiTextKeys.ProductCategoryItemSavedMessage] = "분류 항목을 저장했습니다.",
            [UiTextKeys.ProductCategoryDeactivatedMessage] = "분류를 사용 중지했습니다.",
            [UiTextKeys.ProductCategoryReactivatedMessage] = "분류를 다시 사용하도록 변경했습니다.",
            [UiTextKeys.ProductCategoryItemDeactivatedMessage] = "분류 항목을 사용 중지했습니다.",
            [UiTextKeys.ProductCategoryItemReactivatedMessage] = "분류 항목을 다시 사용하도록 변경했습니다.",
            [UiTextKeys.ProductCategoryValidationMessage] =
                "이름, 코드와 0 이상의 정렬 순서를 확인해 주세요.",
            [UiTextKeys.ProductCategoryDuplicateCodeMessage] =
                "같은 범위에 이미 등록된 코드입니다. 다른 코드를 입력해 주세요.",
            [UiTextKeys.ProductCategoryConflictMessage] =
                "다른 변경이 먼저 저장되었습니다. 목록을 다시 불러온 뒤 시도해 주세요.",
            [UiTextKeys.ProductCategoryTargetUnavailableMessage] =
                "처리할 분류 또는 항목을 찾을 수 없습니다. 목록을 다시 확인해 주세요.",
            [UiTextKeys.ProductCategoryParentInactiveMessage] = "사용 중인 상위 분류를 선택해 주세요.",
            [UiTextKeys.ProductCategoryActiveItemsBlockMessage] =
                "사용 중인 항목이 있어 분류를 사용 중지할 수 없습니다. 항목을 먼저 사용 중지해 주세요.",
            [UiTextKeys.ProductCategoryOperationFailedMessage] =
                "분류 정보를 처리하지 못했습니다. 다시 시도해 주세요."
        };

    private const string ExistingResourceFingerprint =
        "F1A5F63834E61427024809910EFE70FE45D898F357DE6BFF5BF27E29F6E14E68";
}
