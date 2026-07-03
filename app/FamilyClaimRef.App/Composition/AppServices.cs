using System.IO;
using FamilyClaimRef.App.Services.Storage;
using FamilyClaimRef.App.Services.UI;
using FamilyClaimRef.App.ViewModels;

namespace FamilyClaimRef.App.Composition;

public sealed class AppServices
{
    private const string AppDataFolderName = "FamilyClaimRef";

    private AppServices(
        DocumentRegistrationViewModel documentRegistrationViewModel,
        string metadataRootPath,
        string attachmentRootPath)
    {
        DocumentRegistrationViewModel = documentRegistrationViewModel
            ?? throw new ArgumentNullException(nameof(documentRegistrationViewModel));
        MetadataRootPath = metadataRootPath
            ?? throw new ArgumentNullException(nameof(metadataRootPath));
        AttachmentRootPath = attachmentRootPath
            ?? throw new ArgumentNullException(nameof(attachmentRootPath));
    }

    public DocumentRegistrationViewModel DocumentRegistrationViewModel { get; }

    public string MetadataRootPath { get; }

    public string AttachmentRootPath { get; }

    public static AppServices CreateDefault()
    {
        var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var appDataRoot = Path.Combine(localAppData, AppDataFolderName);
        var metadataRootPath = Path.Combine(appDataRoot, "data", "local");
        var attachmentRootPath = Path.Combine(appDataRoot, "attachments");

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

        return new AppServices(
            documentRegistrationViewModel,
            metadataRootPath,
            attachmentRootPath);
    }
}
