using FamilyClaimRef.App.Services.Runtime;
using FamilyClaimRef.App.Services.Storage;
using FamilyClaimRef.App.Services.UI;
using FamilyClaimRef.App.ViewModels;

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
        var documentRegistrationViewModel = new DocumentRegistrationViewModel(
            registrationWorkflow,
            filePickerService,
            policyClaimStorageService);
        var policyClaimManagementViewModel = new PolicyClaimManagementViewModel(policyClaimStorageService);
        var mainWindowViewModel = new MainWindowViewModel(
            documentRegistrationViewModel,
            policyClaimManagementViewModel);

        return new AppServices(
            mainWindowViewModel,
            runtimeRootPaths.RuntimeRootPath,
            metadataRootPath,
            attachmentRootPath);
    }
}
