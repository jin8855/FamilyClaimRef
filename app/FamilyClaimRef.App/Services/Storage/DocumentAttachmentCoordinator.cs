using System.IO;
using FamilyClaimRef.App.Models.Storage;
using FamilyClaimRef.App.Services;

namespace FamilyClaimRef.App.Services.Storage;

public sealed class DocumentAttachmentCoordinator
{
    private const string DocumentIdToken = "document";
    private const int MaxDuplicateIndex = 999;

    private readonly IDocumentStorageService documentStorageService;
    private readonly IFileAttachmentService fileAttachmentService;

    public DocumentAttachmentCoordinator(
        IDocumentStorageService documentStorageService,
        IFileAttachmentService fileAttachmentService)
    {
        this.documentStorageService = documentStorageService
            ?? throw new ArgumentNullException(nameof(documentStorageService));
        this.fileAttachmentService = fileAttachmentService
            ?? throw new ArgumentNullException(nameof(fileAttachmentService));
    }

    public async Task<DocumentAttachmentResult> AttachDocumentAsync(
        DocumentAttachmentRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        cancellationToken.ThrowIfCancellationRequested();

        var sourceFilePath = NormalizeSourceFilePath(request.SourceFilePath);
        var documentScope = NormalizeRequiredValue(request.DocumentScope, nameof(request.DocumentScope));
        var documentType = NormalizeRequiredValue(request.DocumentType, nameof(request.DocumentType));
        var displayTitle = NormalizeRequiredValue(request.DisplayTitle, nameof(request.DisplayTitle));
        if (request.ReferenceDate == default)
        {
            throw new ArgumentException("Reference date is required.", nameof(request.ReferenceDate));
        }

        var sourceExtension = ExtractSourceExtension(sourceFilePath);
        var existingPhysicalFileNames = await GetExistingPhysicalFileNamesAsync(cancellationToken);

        for (var duplicateIndex = 1; duplicateIndex <= MaxDuplicateIndex; duplicateIndex++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var physicalFileName = FileNamePolicyService.CreatePhysicalFileName(
                documentScope,
                DocumentIdToken,
                request.ReferenceDate,
                documentType,
                sourceExtension,
                duplicateIndex);

            if (existingPhysicalFileNames.Contains(physicalFileName)
                || await TargetFileExistsAsync(physicalFileName, cancellationToken))
            {
                continue;
            }

            FileAttachmentCopyResult copyResult;
            try
            {
                copyResult = await fileAttachmentService.CopyDocumentFileAsync(
                    sourceFilePath,
                    physicalFileName,
                    cancellationToken);
            }
            catch (IOException) when (duplicateIndex < MaxDuplicateIndex)
            {
                continue;
            }

            try
            {
                var document = await documentStorageService.AddDocumentAsync(
                    new DocumentDraft(
                        copyResult.PhysicalFileName,
                        displayTitle,
                        copyResult.Extension,
                        copyResult.RelativePath),
                    cancellationToken);

                return new DocumentAttachmentResult(document, copyResult);
            }
            catch (Exception metadataException)
            {
                await CleanupCopiedFileAsync(copyResult.RelativePath, metadataException, cancellationToken);
                throw;
            }
        }

        throw new InvalidOperationException("Available duplicate index was not found.");
    }

    private async Task<HashSet<string>> GetExistingPhysicalFileNamesAsync(CancellationToken cancellationToken)
    {
        var documents = await documentStorageService.GetDocumentsAsync(cancellationToken);

        return documents
            .Select(document => document.PhysicalFileName)
            .Where(physicalFileName => !string.IsNullOrWhiteSpace(physicalFileName))
            .ToHashSet(StringComparer.Ordinal);
    }

    private Task<bool> TargetFileExistsAsync(string physicalFileName, CancellationToken cancellationToken)
    {
        return fileAttachmentService.DocumentFileExistsAsync($"documents/{physicalFileName}", cancellationToken);
    }

    private async Task CleanupCopiedFileAsync(
        string relativePath,
        Exception metadataException,
        CancellationToken cancellationToken)
    {
        try
        {
            await fileAttachmentService.DeleteDocumentFileIfExistsAsync(relativePath, cancellationToken);
        }
        catch (Exception cleanupException)
        {
            throw new AggregateException(
                "Document metadata save failed and copied file cleanup also failed.",
                metadataException,
                cleanupException);
        }
    }

    private static string NormalizeSourceFilePath(string sourceFilePath)
    {
        var normalizedSourceFilePath = NormalizeRequiredValue(sourceFilePath, nameof(sourceFilePath));
        var sourceFullPath = Path.GetFullPath(normalizedSourceFilePath);
        if (!File.Exists(sourceFullPath))
        {
            throw new FileNotFoundException("Source file was not found.", sourceFullPath);
        }

        return sourceFullPath;
    }

    private static string NormalizeRequiredValue(string value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Value is required.", parameterName);
        }

        return value.Trim();
    }

    private static string ExtractSourceExtension(string sourceFilePath)
    {
        var extension = Path.GetExtension(sourceFilePath);
        if (extension.StartsWith('.'))
        {
            extension = extension[1..];
        }

        return extension;
    }
}
