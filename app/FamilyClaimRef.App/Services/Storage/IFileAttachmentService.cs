namespace FamilyClaimRef.App.Services.Storage;

public interface IFileAttachmentService
{
    Task<StagedFileAttachment> StageDocumentFileAsync(
        string sourceFilePath,
        CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException("Staged attachment operations are not supported.");
    }

    Task<FileAttachmentCopyResult> FinalizeStagedDocumentFileAsync(
        StagedFileAttachment stagedFile,
        string physicalFileName,
        CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException("Staged attachment operations are not supported.");
    }

    Task DeleteStagedFileIfExistsAsync(
        StagedFileAttachment stagedFile,
        CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException("Staged attachment operations are not supported.");
    }

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
