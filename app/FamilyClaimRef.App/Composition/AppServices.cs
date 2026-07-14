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
        string runtimeRootPath,
        string metadataRootPath,
        string attachmentRootPath)
    {
        MainWindowViewModel = mainWindowViewModel
            ?? throw new ArgumentNullException(nameof(mainWindowViewModel));
        RuntimeRootPath = runtimeRootPath
            ?? throw new ArgumentNullException(nameof(runtimeRootPath));
        MetadataRootPath = metadataRootPath
            ?? throw new ArgumentNullException(nameof(metadataRootPath));
        AttachmentRootPath = attachmentRootPath
            ?? throw new ArgumentNullException(nameof(attachmentRootPath));
    }

    public MainWindowViewModel MainWindowViewModel { get; }

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
        var documentRegistrationViewModel = new DocumentRegistrationViewModel(
            registrationWorkflow,
            filePickerService,
            policyClaimStorageService,
            uiTextProvider);
        var policyClaimManagementViewModel = new PolicyClaimManagementViewModel(
            policyClaimStorageService,
            uiTextProvider);
        var mainWindowViewModel = new MainWindowViewModel(
            documentRegistrationViewModel,
            policyClaimManagementViewModel);

        return new AppServices(
            mainWindowViewModel,
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
            [UiTextKeys.ClaimManagementMessageCreated] = "Claim target was created.",
            [UiTextKeys.ClaimManagementMessageDisabled] = "Claim target was disabled.",
            [UiTextKeys.ClaimManagementValidationTitleRequired] = "Claim target title is required.",
            [UiTextKeys.PolicyManagementMessageCreated] = "Policy target was created.",
            [UiTextKeys.PolicyManagementMessageDisabled] = "Policy target was disabled.",
            [UiTextKeys.PolicyManagementValidationDisableBlockedByActiveClaims] =
                "Policy target has active claim targets. Disable claim targets first.",
            [UiTextKeys.ClaimManagementValidationSelectPolicyBeforeCreate] =
                "Select an active policy target before creating a claim target.",
            [UiTextKeys.PolicyManagementValidationTitleRequired] = "Policy target title is required.",
            [UiTextKeys.ClaimManagementValidationSelectClaimTarget] = "Select a claim target.",
            [UiTextKeys.PolicyManagementValidationSelectPolicyTarget] = "Select a policy target."
        });
    }
}
