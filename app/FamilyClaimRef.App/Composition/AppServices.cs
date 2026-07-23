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
        IPolicyClaimStorageService policyClaimStorageService = new JsonPolicyClaimStorageService(metadataRootPath);
        IFileAttachmentService fileAttachmentService = new LocalFileAttachmentService(attachmentRootPath);
        var attachmentCoordinator = new DocumentAttachmentCoordinator(
            documentStorageService,
            fileAttachmentService);
        var linkCoordinator = new DocumentLinkCoordinator(
            documentStorageService,
            policyClaimStorageService);
        var registrationWorkflow = new DocumentRegistrationWorkflow(
            attachmentCoordinator,
            linkCoordinator,
            documentStorageService,
            fileAttachmentService);
        IFilePickerService filePickerService = new WpfFilePickerService();
        var uiTextProvider = CreateUiTextProvider();
        var mainWindowDocumentRegistrationViewModel = new DocumentRegistrationViewModel(
            registrationWorkflow,
            filePickerService,
            policyClaimStorageService,
            uiTextProvider);
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
            uiTextProvider);
        var productDocumentListViewModel = new ProductDocumentListViewModel(
            documentStorageService,
            uiTextProvider);
        var productShellPolicyClaimManagementViewModel = new PolicyClaimManagementViewModel(
            policyClaimStorageService,
            uiTextProvider);
        var productShellViewModel = new ProductShellViewModel(
            uiTextProvider,
            productShellDocumentRegistrationViewModel,
            productDocumentListViewModel,
            productShellPolicyClaimManagementViewModel);

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
                "사용 중지할 보험 계약을 선택해 주세요."
        });
    }
}
