using FamilyClaimRef.App.Services.Localization;
using FamilyClaimRef.App.Services.Runtime;
using FamilyClaimRef.App.Services.Storage;
using FamilyClaimRef.App.Services.UI;
using FamilyClaimRef.App.ViewModels;
using System.Windows;

namespace FamilyClaimRef.App.Composition;

public sealed class AppServices
{
    private AppServices(
        MainWindowViewModel mainWindowViewModel,
        ProductShellViewModel productShellViewModel,
        string runtimeRootPath,
        string metadataRootPath,
        string attachmentRootPath)
    {
        MainWindowViewModel = mainWindowViewModel
            ?? throw new ArgumentNullException(nameof(mainWindowViewModel));
        ProductShellViewModel = productShellViewModel
            ?? throw new ArgumentNullException(nameof(productShellViewModel));
        RuntimeRootPath = runtimeRootPath
            ?? throw new ArgumentNullException(nameof(runtimeRootPath));
        MetadataRootPath = metadataRootPath
            ?? throw new ArgumentNullException(nameof(metadataRootPath));
        AttachmentRootPath = attachmentRootPath
            ?? throw new ArgumentNullException(nameof(attachmentRootPath));
    }

    public MainWindowViewModel MainWindowViewModel { get; }

    public ProductShellViewModel ProductShellViewModel { get; }

    public string RuntimeRootPath { get; }

    public string MetadataRootPath { get; }

    public string AttachmentRootPath { get; }

    public static AppServices CreateDefault()
    {
        return Create(new EnvironmentRuntimeRootProvider());
    }

    public static AppServices Create(IRuntimeRootProvider runtimeRootProvider)
    {
        ArgumentNullException.ThrowIfNull(runtimeRootProvider);

        var runtimeRootPaths = runtimeRootProvider.GetRuntimeRootPaths();
        var metadataRootPath = runtimeRootPaths.MetadataRootPath;
        var attachmentRootPath = runtimeRootPaths.AttachmentRootPath;

        IDocumentStorageService documentStorageService = new JsonDocumentStorageService(metadataRootPath);
        IFamilyMemberStorageService familyMemberStorageService =
            new JsonFamilyMemberStorageService(metadataRootPath);
        ICategoryAggregateStorageService categoryAggregateStorageService =
            new JsonCategoryAggregateStorageService(metadataRootPath);
        IPolicyClaimStorageService policyClaimStorageService =
            new JsonPolicyClaimStorageService(metadataRootPath, familyMemberStorageService);
        var claimCaseStorageService = (IClaimCaseStorageService)policyClaimStorageService;
        IClaimSubmissionStorageService claimSubmissionStorageService =
            new JsonClaimSubmissionStorageService(
                metadataRootPath,
                claimCaseStorageService,
                policyClaimStorageService,
                documentStorageService);
        IClaimPaymentStorageService claimPaymentStorageService =
            new JsonClaimPaymentStorageService(
                metadataRootPath,
                claimSubmissionStorageService,
                claimCaseStorageService,
                policyClaimStorageService);
        IFileAttachmentService fileAttachmentService = new LocalFileAttachmentService(attachmentRootPath);
        var fileValidationService = new DocumentFileValidationService();
        var attachmentCoordinator = new DocumentAttachmentCoordinator(
            documentStorageService,
            fileAttachmentService,
            fileValidationService);
        var linkCoordinator = new DocumentLinkCoordinator(
            documentStorageService,
            policyClaimStorageService);
        var registrationWorkflow = new DocumentRegistrationWorkflow(
            attachmentCoordinator,
            linkCoordinator,
            documentStorageService,
            fileAttachmentService,
            policyClaimStorageService);
        IFilePickerService filePickerService = new WpfFilePickerService(fileValidationService);
        var uiTextProvider = CreateUiTextProvider();
        var mainWindowDocumentRegistrationViewModel = new DocumentRegistrationViewModel(
            registrationWorkflow,
            filePickerService,
            policyClaimStorageService,
            uiTextProvider,
            fileValidationService);
        var mainWindowPolicyClaimManagementViewModel = new PolicyClaimManagementViewModel(
            policyClaimStorageService,
            uiTextProvider);
        var mainWindowViewModel = new MainWindowViewModel(
            mainWindowDocumentRegistrationViewModel,
            mainWindowPolicyClaimManagementViewModel);
        var productShellDocumentRegistrationViewModel = new DocumentRegistrationViewModel(
            registrationWorkflow,
            filePickerService,
            policyClaimStorageService,
            uiTextProvider,
            fileValidationService,
            familyMemberStorageService);
        var productDocumentListViewModel = new ProductDocumentListViewModel(
            documentStorageService,
            uiTextProvider);
        IManagedDocumentOpener managedDocumentOpener = new ManagedDocumentOpener(
            attachmentRootPath);
        var productShellPolicyClaimManagementViewModel = new PolicyClaimManagementViewModel(
            policyClaimStorageService,
            familyMemberStorageService,
            documentStorageService,
            managedDocumentOpener,
            uiTextProvider);
        var familyMemberManagementViewModel = new FamilyMemberManagementViewModel(
            familyMemberStorageService,
            uiTextProvider);
        var categoryManagementViewModel = new CategoryManagementViewModel(
            categoryAggregateStorageService,
            uiTextProvider);
        var claimPaymentManagementViewModel = new ClaimPaymentManagementViewModel(
            claimPaymentStorageService,
            uiTextProvider);
        var claimSubmissionManagementViewModel = new ClaimSubmissionManagementViewModel(
            claimSubmissionStorageService,
            claimCaseStorageService,
            documentStorageService,
            claimPaymentManagementViewModel,
            uiTextProvider);
        var claimCompleteSummaryViewModel = new ClaimCompleteSummaryViewModel(
            claimCaseStorageService,
            claimSubmissionStorageService,
            claimPaymentStorageService,
            policyClaimStorageService,
            familyMemberStorageService,
            uiTextProvider);
        var productShellViewModel = new ProductShellViewModel(
            uiTextProvider,
            productShellDocumentRegistrationViewModel,
            productDocumentListViewModel,
            productShellPolicyClaimManagementViewModel,
            familyMemberManagementViewModel,
            categoryManagementViewModel,
            claimSubmissionManagementViewModel,
            claimCompleteSummaryViewModel);

        return new AppServices(
            mainWindowViewModel,
            productShellViewModel,
            runtimeRootPaths.RuntimeRootPath,
            metadataRootPath,
            attachmentRootPath);
    }

    private static IUiTextProvider CreateUiTextProvider()
    {
        if (Application.Current is not null)
        {
            return new ResourceUiTextProvider(Application.Current.Resources);
        }

        return new ResourceUiTextProvider(new Dictionary<string, string>(StringComparer.Ordinal)
        {
            [UiTextKeys.ProductShellTitle] = "FamilyClaimRef",
            [UiTextKeys.ProductNavigationHome] = "홈",
            [UiTextKeys.ProductNavigationPolicyContracts] = "보험 계약",
            [UiTextKeys.ProductNavigationClaimCases] = "청구 건",
            [UiTextKeys.ProductNavigationDocumentRegistration] = "문서 등록",
            [UiTextKeys.ProductNavigationDocumentList] = "문서 목록",
            [UiTextKeys.ProductDocumentListTitle] = "문서 목록",
            [UiTextKeys.ProductDocumentListEmptyMessage] = "등록된 문서가 없습니다.",
            [UiTextKeys.ProductDocumentListLoadFailedMessage] = "문서 목록을 불러오지 못했습니다.",
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
            [UiTextKeys.ProductClaimCaseVisitTypeOutpatient] = "통원",
            [UiTextKeys.ProductClaimCaseVisitTypeInpatient] = "입원",
            [UiTextKeys.ProductClaimCaseValidationRequiredMessage] =
                "가족, 청구 건 이름, 진료일, 병원명, 진료 구분과 금액 입력을 확인해 주세요.",
            [UiTextKeys.ProductClaimCaseConflictMessage] =
                "다른 변경이 먼저 저장되었습니다. 목록을 다시 불러온 뒤 다시 시도해 주세요.",
            [UiTextKeys.ProductClaimCaseLegacyReviewRequiredMessage] =
                "기존 청구 건의 가족 연결을 확인할 수 없어 편집할 수 없습니다.",
            [UiTextKeys.ProductClaimCaseDraftCreatedMessage] =
                "청구 건 초안을 생성했습니다.",
            [UiTextKeys.ProductClaimCaseSavedMessage] =
                "청구 건 상세를 저장했습니다.",
            [UiTextKeys.ProductClaimCaseDisabledMessage] =
                "청구 건을 사용 중지했습니다.",
            [UiTextKeys.ProductClaimSubmissionClaimCaseLabel] = "청구 건",
            [UiTextKeys.ProductClaimSubmissionGuidance] =
                "저장된 청구 건과 같은 가족의 보험 계약을 선택해 보험사별 청구 진행 기록을 관리합니다.",
            [UiTextKeys.ProductClaimSubmissionUnsavedNavigationGuidance] =
                "변경 내용을 저장한 뒤 다음 단계로 이동할 수 있습니다.",
            [UiTextKeys.ProductClaimSubmissionListTitle] = "보험사별 청구 기록",
            [UiTextKeys.ProductClaimSubmissionPolicyLabel] = "보험 계약",
            [UiTextKeys.ProductClaimSubmissionStatusLabel] = "진행 상태",
            [UiTextKeys.ProductClaimSubmissionUpdatedAtLabel] = "최근 변경",
            [UiTextKeys.ProductClaimSubmissionEmptyMessage] = "등록된 보험사 청구 기록이 없습니다.",
            [UiTextKeys.ProductClaimSubmissionNewAction] = "새 보험사 청구",
            [UiTextKeys.ProductClaimSubmissionDetailTitle] = "청구 진행 상세",
            [UiTextKeys.ProductClaimSubmissionCoverageLabel] = "담보명",
            [UiTextKeys.ProductClaimSubmissionSubmittedDateLabel] = "청구일",
            [UiTextKeys.ProductClaimSubmissionSubmittedAmountLabel] = "청구 금액",
            [UiTextKeys.ProductClaimSubmissionDocumentLabel] = "제출 문서",
            [UiTextKeys.ProductClaimSubmissionMemoLabel] = "메모",
            [UiTextKeys.ProductClaimSubmissionCreateAction] = "준비중 기록 생성",
            [UiTextKeys.ProductClaimSubmissionSaveAction] = "진행 상태 저장",
            [UiTextKeys.ProductClaimSubmissionHistoryAction] = "청구 이력",
            [UiTextKeys.ProductClaimSubmissionPaymentFutureTitle] = "지급 결과",
            [UiTextKeys.ProductClaimSubmissionPaymentFutureMessage] =
                "지급·삭감·부지급 결과는 후속 ClaimPayment 기능에서 관리합니다.",
            [UiTextKeys.ProductClaimSubmissionValidationMessage] =
                "보험 계약, 담보명, 청구일, 청구 금액과 진행 상태를 확인해 주세요.",
            [UiTextKeys.ProductClaimSubmissionConflictMessage] =
                "다른 변경이 먼저 저장되었습니다. 기록을 다시 불러온 뒤 시도해 주세요.",
            [UiTextKeys.ProductClaimSubmissionLegacyReviewMessage] =
                "기존 청구 건 또는 보험 계약의 가족 연결을 확인할 수 없어 처리할 수 없습니다.",
            [UiTextKeys.ProductClaimSubmissionReferenceMessage] =
                "청구 건, 보험 계약 또는 제출 문서의 연결 상태를 확인해 주세요.",
            [UiTextKeys.ProductClaimSubmissionTransitionMessage] =
                "현재 진행 상태에서는 선택한 상태로 변경할 수 없습니다.",
            [UiTextKeys.ProductClaimSubmissionOperationFailedMessage] =
                "보험사 청구 기록을 처리하지 못했습니다. 다시 시도해 주세요.",
            [UiTextKeys.ProductClaimSubmissionCreatedMessage] = "보험사 청구 준비 기록을 생성했습니다.",
            [UiTextKeys.ProductClaimSubmissionSavedMessage] = "보험사 청구 진행 상태를 저장했습니다.",
            [UiTextKeys.ProductClaimSubmissionReferenceUnavailableValue] = "연결 확인 필요",
            [UiTextKeys.ProductClaimSubmissionNotEnteredValue] = "미입력",
            [UiTextKeys.ProductClaimSubmissionStatusPreparing] = "준비중",
            [UiTextKeys.ProductClaimSubmissionStatusSubmitted] = "청구 접수",
            [UiTextKeys.ProductClaimSubmissionStatusAdditionalDocumentsRequested] = "추가 서류 요청",
            [UiTextKeys.ProductClaimSubmissionStatusReviewing] = "심사중",
            [UiTextKeys.ProductClaimSubmissionStatusCancelled] = "취소",
            [UiTextKeys.ProductClaimSubmissionStatusCompleted] = "청구 처리 완료",
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
            [UiTextKeys.ProductInsurancePolicyLinkedDocumentsSectionTitle] =
                "이 보험에 연결할 문서",
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
            [UiTextKeys.DocumentRegistrationStatusCleanupFailed] =
                "등록 중 일부 정리가 실패했습니다. 다시 시도하거나 관리자에게 문의하세요.",
            [UiTextKeys.DocumentRegistrationMessageNoActiveClaim] = "No active claim is available for selection.",
            [UiTextKeys.DocumentRegistrationMessageNoActivePolicy] = "No active policy is available for selection.",
            [UiTextKeys.DocumentRegistrationStatusFailed] = "문서 등록에 실패했습니다.",
            [UiTextKeys.DocumentRegistrationStatusCompleted] = "문서 등록이 완료되었습니다.",
            [UiTextKeys.DocumentRegistrationValidationSelectClaimBeforeRegister] =
                "Select a claim before registering this document.",
            [UiTextKeys.DocumentRegistrationValidationSelectPolicyBeforeRegister] =
                "Select a policy before registering this document.",
            [UiTextKeys.DocumentRegistrationStatusFileSelected] = "파일을 선택했습니다.",
            [UiTextKeys.DocumentRegistrationValidationSelectFile] = "파일을 선택해 주세요.",
            [UiTextKeys.DocumentRegistrationValidationSelectTargetKind] = "연결 대상 유형을 선택해 주세요.",
            [UiTextKeys.DocumentRegistrationValidationSelectTarget] = "저장할 대상을 입력해 주세요.",
            [UiTextKeys.DocumentRegistrationValidationSelectDocumentType] = "문서 유형을 선택해 주세요.",
            [UiTextKeys.DocumentRegistrationValidationEnterDisplayTitle] = "표시 제목을 입력해 주세요.",
            [UiTextKeys.DocumentRegistrationValidationSelectReferenceDate] = "기준일을 선택해 주세요.",
            [UiTextKeys.DocumentReferenceDateLabel] = "문서 발급·조회 기준일",
            [UiTextKeys.DocumentReferenceDateHelp] =
                "문서에 표시된 발급일 또는 보험정보 조회 기준일입니다. 보험 가입일과는 다릅니다. 문서에 날짜가 없으면 비워두세요.",
            [UiTextKeys.ProductDocumentRegistrationValidationUnsupportedFileType] =
                "Unsupported file type.",
            [UiTextKeys.ProductDocumentRegistrationValidationEmptyFile] =
                "Empty files cannot be registered.",
            [UiTextKeys.ProductDocumentRegistrationValidationFileTooLarge] =
                "The file must be 25 MB or smaller.",
            [UiTextKeys.ProductDocumentRegistrationValidationSourceUnavailable] =
                "The selected file is unavailable.",
            [UiTextKeys.ProductDocumentRegistrationValidationSourceChanged] =
                "The selected file changed.",
            [UiTextKeys.ProductDocumentRegistrationValidationDuplicateDocument] =
                "The same document is already registered for this target.",
            [UiTextKeys.ProductDocumentRegistrationStatusCanceled] =
                "File selection canceled.",
            [UiTextKeys.ProductDocumentRegistrationStatusRetryAvailable] =
                "Inputs were retained for retry.",
            [UiTextKeys.ClaimManagementMessageCreated] = "청구 건을 등록했습니다.",
            [UiTextKeys.ClaimManagementMessageDisabled] = "청구 건을 사용 중지했습니다.",
            [UiTextKeys.ClaimManagementValidationTitleRequired] = "청구 건 이름을 입력해 주세요.",
            [UiTextKeys.PolicyManagementMessageCreated] = "보험 계약을 등록했습니다.",
            [UiTextKeys.PolicyManagementMessageDisabled] = "보험 계약을 사용 중지했습니다.",
            [UiTextKeys.PolicyManagementValidationDisableBlockedByActiveClaims] =
                "활성 청구 건이 있어 보험 계약을 사용 중지할 수 없습니다. 청구 건을 먼저 사용 중지해 주세요.",
            [UiTextKeys.ClaimManagementValidationSelectPolicyBeforeCreate] =
                "청구 건을 등록할 보험 계약을 선택해 주세요.",
            [UiTextKeys.PolicyManagementValidationTitleRequired] = "보험 계약 이름을 입력해 주세요.",
            [UiTextKeys.ClaimManagementValidationSelectClaimTarget] = "사용 중지할 청구 건을 선택해 주세요.",
            [UiTextKeys.PolicyManagementValidationSelectPolicyTarget] =
                "사용 중지할 보험 계약을 선택해 주세요.",
            [UiTextKeys.ProductClaimPaymentValidationMessage] =
                "지급 상태와 상태별 필수 항목 및 지급 금액 형식을 확인해 주세요.",
            [UiTextKeys.ProductClaimPaymentConflictMessage] =
                "다른 변경이 먼저 저장되었습니다. 지급 결과를 다시 불러온 뒤 시도해 주세요.",
            [UiTextKeys.ProductClaimPaymentLegacyReviewMessage] =
                "기존 청구 건의 가족 연결을 확인할 수 없어 지급 결과를 처리할 수 없습니다.",
            [UiTextKeys.ProductClaimPaymentReferenceMessage] =
                "보험사 청구, 청구 건 또는 보험 계약의 연결 상태를 확인해 주세요.",
            [UiTextKeys.ProductClaimPaymentTransitionMessage] =
                "현재 상태에서는 선택한 지급 결과로 변경할 수 없습니다.",
            [UiTextKeys.ProductClaimPaymentOperationFailedMessage] =
                "지급 결과를 처리하지 못했습니다. 다시 시도해 주세요.",
            [UiTextKeys.ProductClaimPaymentCreatedMessage] = "지급 대기 기록을 생성했습니다.",
            [UiTextKeys.ProductClaimPaymentSavedMessage] = "지급 결과를 저장했습니다.",
            [UiTextKeys.ProductClaimPaymentNotEnteredValue] = "미입력",
            [UiTextKeys.ProductClaimPaymentStatusPending] = "대기",
            [UiTextKeys.ProductClaimPaymentStatusPaid] = "지급",
            [UiTextKeys.ProductClaimPaymentStatusPartiallyPaid] = "부분 지급",
            [UiTextKeys.ProductClaimPaymentStatusDenied] = "부지급",
            [UiTextKeys.ProductClaimPaymentStatusCancelled] = "취소",
            [UiTextKeys.ProductClaimCompleteGuidance] = "선택한 청구 건의 보험사별 청구 진행과 지급 결과를 읽기 전용으로 확인합니다.",
            [UiTextKeys.ProductClaimCompleteBackToSubmissionAction] = "청구 진행으로 돌아가기",
            [UiTextKeys.ProductClaimCompleteClaimInfoSectionTitle] = "청구 건 기본 정보",
            [UiTextKeys.ProductClaimCompleteFamilyLabel] = "가족",
            [UiTextKeys.ProductClaimCompleteTreatmentDateLabel] = "진료일",
            [UiTextKeys.ProductClaimCompleteHospitalLabel] = "병원명",
            [UiTextKeys.ProductClaimCompleteDiagnosisLabel] = "진단",
            [UiTextKeys.ProductClaimCompleteVisitTypeLabel] = "진료 구분",
            [UiTextKeys.ProductClaimCompleteCaseStatusLabel] = "청구 건 상태",
            [UiTextKeys.ProductClaimCompleteCaseStatusSaved] = "저장됨",
            [UiTextKeys.ProductClaimCompleteSubmissionCountsSectionTitle] = "보험사 청구 현황",
            [UiTextKeys.ProductClaimCompleteSubmissionTotalLabel] = "전체",
            [UiTextKeys.ProductClaimCompleteSubmissionInProgressLabel] = "진행 중",
            [UiTextKeys.ProductClaimCompleteSubmissionCompletedLabel] = "처리 완료",
            [UiTextKeys.ProductClaimCompleteSubmissionCancelledLabel] = "취소",
            [UiTextKeys.ProductClaimCompletePaymentCountsSectionTitle] = "지급 결과 현황",
            [UiTextKeys.ProductClaimCompletePaymentPendingLabel] = "대기",
            [UiTextKeys.ProductClaimCompletePaymentPaidLabel] = "지급",
            [UiTextKeys.ProductClaimCompletePaymentPartiallyPaidLabel] = "부분 지급",
            [UiTextKeys.ProductClaimCompletePaymentDeniedLabel] = "부지급",
            [UiTextKeys.ProductClaimCompletePaymentCancelledLabel] = "취소",
            [UiTextKeys.ProductClaimCompleteSubmissionListTitle] = "보험사별 처리 요약",
            [UiTextKeys.ProductClaimCompletePolicyLabel] = "보험 계약",
            [UiTextKeys.ProductClaimCompleteSubmissionStatusLabel] = "청구 상태",
            [UiTextKeys.ProductClaimCompletePaymentSummaryLabel] = "지급 결과 요약",
            [UiTextKeys.ProductClaimCompleteUpdatedAtLabel] = "최근 변경",
            [UiTextKeys.ProductClaimCompleteSubmissionEmptyMessage] = "이 청구 건에 등록된 보험사 청구 기록이 없습니다.",
            [UiTextKeys.ProductClaimCompleteEmptyMessage] = "요약할 청구 건을 선택해 주세요.",
            [UiTextKeys.ProductClaimCompleteReferenceMessage] = "청구 건, 보험 계약 또는 지급 결과의 연결 상태를 확인해 주세요.",
            [UiTextKeys.ProductClaimCompleteLegacyReviewMessage] = "기존 청구 건 또는 보험 계약의 가족 연결을 확인할 수 없어 요약을 표시할 수 없습니다.",
            [UiTextKeys.ProductClaimCompleteLoadFailedMessage] = "청구 처리 요약을 불러오지 못했습니다. 다시 시도해 주세요.",
            [UiTextKeys.ProductClaimCompleteNoPaymentsValue] = "지급 결과 없음",
            [UiTextKeys.ProductClaimCompleteNotEnteredValue] = "미입력",
            [UiTextKeys.ProductClaimCompletePaymentSummaryFormat] = "대기 {0} · 지급 {1} · 부분 지급 {2} · 부지급 {3} · 취소 {4}",
        });
    }
}
