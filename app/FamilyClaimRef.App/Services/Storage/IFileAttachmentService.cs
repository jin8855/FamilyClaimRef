namespace FamilyClaimRef.App.Services.Storage;

public interface IFileAttachmentService
{
    Task<FileAttachmentCopyResult> CopyDocumentFileAsync(
        string sourceFilePath,
        string physicalFileName,
        CancellationToken cancellationToken = default);

    Task DeleteDocumentFileIfExistsAsync(
        string relativePath,
        CancellationToken cancellationToken = default);

    Task<bool> DocumentFileExistsAsync(
        string relativePath,
        CancellationToken cancellationToken = default);
}
