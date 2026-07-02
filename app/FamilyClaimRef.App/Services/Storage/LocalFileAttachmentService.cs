using System.IO;

namespace FamilyClaimRef.App.Services.Storage;

public sealed class LocalFileAttachmentService : IFileAttachmentService
{
    private const string DocumentsFolderName = "documents";

    private readonly string attachmentRootPath;

    public LocalFileAttachmentService(string attachmentRootPath)
    {
        if (string.IsNullOrWhiteSpace(attachmentRootPath))
        {
            throw new ArgumentException("Attachment root path is required.", nameof(attachmentRootPath));
        }

        this.attachmentRootPath = Path.GetFullPath(attachmentRootPath);
    }

    public Task<FileAttachmentCopyResult> CopyDocumentFileAsync(
        string sourceFilePath,
        string physicalFileName,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var sourceFullPath = NormalizeSourceFilePath(sourceFilePath);
        var normalizedPhysicalFileName = NormalizePhysicalFileName(physicalFileName);
        var relativePath = CreateDocumentRelativePath(normalizedPhysicalFileName);
        var targetFullPath = GetFullPathUnderAttachmentRoot(relativePath);

        if (File.Exists(targetFullPath))
        {
            throw new IOException("Target file already exists.");
        }

        if (string.Equals(sourceFullPath, targetFullPath, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Source and target file paths must be different.");
        }

        var targetDirectory = Path.GetDirectoryName(targetFullPath);
        if (!string.IsNullOrWhiteSpace(targetDirectory))
        {
            Directory.CreateDirectory(targetDirectory);
        }

        File.Copy(sourceFullPath, targetFullPath, overwrite: false);

        var copiedFile = new FileInfo(targetFullPath);
        var result = new FileAttachmentCopyResult(
            relativePath,
            normalizedPhysicalFileName,
            GetExtension(normalizedPhysicalFileName),
            copiedFile.Length);

        return Task.FromResult(result);
    }

    public Task DeleteDocumentFileIfExistsAsync(
        string relativePath,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var targetFullPath = GetFullPathUnderAttachmentRoot(relativePath);
        if (File.Exists(targetFullPath))
        {
            File.Delete(targetFullPath);
        }

        return Task.CompletedTask;
    }

    public Task<bool> DocumentFileExistsAsync(
        string relativePath,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var targetFullPath = GetFullPathUnderAttachmentRoot(relativePath);

        return Task.FromResult(File.Exists(targetFullPath));
    }

    private static string NormalizeSourceFilePath(string sourceFilePath)
    {
        if (string.IsNullOrWhiteSpace(sourceFilePath))
        {
            throw new ArgumentException("Source file path is required.", nameof(sourceFilePath));
        }

        var sourceFullPath = Path.GetFullPath(sourceFilePath);
        if (!File.Exists(sourceFullPath))
        {
            throw new FileNotFoundException("Source file was not found.", sourceFullPath);
        }

        return sourceFullPath;
    }

    private static string NormalizePhysicalFileName(string physicalFileName)
    {
        if (string.IsNullOrWhiteSpace(physicalFileName))
        {
            throw new ArgumentException("Physical file name is required.", nameof(physicalFileName));
        }

        var normalizedPhysicalFileName = physicalFileName.Trim();
        if (Path.IsPathRooted(normalizedPhysicalFileName))
        {
            throw new ArgumentException("Physical file name must not be an absolute path.", nameof(physicalFileName));
        }

        if (normalizedPhysicalFileName.Contains("..", StringComparison.Ordinal))
        {
            throw new ArgumentException("Physical file name must not contain path traversal.", nameof(physicalFileName));
        }

        if (normalizedPhysicalFileName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
        {
            throw new ArgumentException("Physical file name contains invalid characters.", nameof(physicalFileName));
        }

        if (normalizedPhysicalFileName.Contains(Path.DirectorySeparatorChar)
            || normalizedPhysicalFileName.Contains(Path.AltDirectorySeparatorChar))
        {
            throw new ArgumentException("Physical file name must not contain directory separators.", nameof(physicalFileName));
        }

        return normalizedPhysicalFileName;
    }

    private static string CreateDocumentRelativePath(string physicalFileName)
    {
        return $"{DocumentsFolderName}/{physicalFileName}";
    }

    private static string GetExtension(string physicalFileName)
    {
        var extension = Path.GetExtension(physicalFileName);
        if (extension.StartsWith('.'))
        {
            extension = extension[1..];
        }

        return extension.ToLowerInvariant();
    }

    private string GetFullPathUnderAttachmentRoot(string relativePath)
    {
        if (string.IsNullOrWhiteSpace(relativePath))
        {
            throw new ArgumentException("Relative path is required.", nameof(relativePath));
        }

        var normalizedRelativePath = relativePath.Trim();
        if (Path.IsPathRooted(normalizedRelativePath))
        {
            throw new ArgumentException("Relative path must not be an absolute path.", nameof(relativePath));
        }

        if (normalizedRelativePath.Split(['/', '\\'], StringSplitOptions.RemoveEmptyEntries)
            .Any(segment => segment == ".."))
        {
            throw new ArgumentException("Relative path must not contain path traversal.", nameof(relativePath));
        }

        var fullPath = Path.GetFullPath(Path.Combine(
            attachmentRootPath,
            normalizedRelativePath.Replace('/', Path.DirectorySeparatorChar)));

        var pathFromRoot = Path.GetRelativePath(attachmentRootPath, fullPath);
        if (Path.IsPathRooted(pathFromRoot)
            || pathFromRoot == ".."
            || pathFromRoot.StartsWith($"..{Path.DirectorySeparatorChar}", StringComparison.Ordinal)
            || pathFromRoot.StartsWith($"..{Path.AltDirectorySeparatorChar}", StringComparison.Ordinal))
        {
            throw new ArgumentException("Relative path escapes the attachment root.", nameof(relativePath));
        }

        return fullPath;
    }
}
