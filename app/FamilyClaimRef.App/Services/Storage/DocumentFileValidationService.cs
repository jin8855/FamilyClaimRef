using System.IO;
using System.Security.Cryptography;

namespace FamilyClaimRef.App.Services.Storage;

public sealed class DocumentFileValidationService
{
    public const long MaximumFileSizeBytes = 26_214_400;

    private static readonly IReadOnlyDictionary<string, string> ValidatedTypes =
        new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["pdf"] = "PDF",
            ["jpg"] = "JPEG",
            ["jpeg"] = "JPEG",
            ["png"] = "PNG"
        };

    public Task<DocumentFileValidationResult> ValidateSourceAsync(
        string sourceFilePath,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(sourceFilePath))
        {
            throw Create(DocumentRegistrationErrorCode.SourceUnavailable, "Source file is required.");
        }

        string fullPath;
        try
        {
            fullPath = Path.GetFullPath(sourceFilePath);
        }
        catch (Exception exception) when (exception is ArgumentException or NotSupportedException or PathTooLongException)
        {
            throw Create(
                DocumentRegistrationErrorCode.SourceUnavailable,
                "Source file path is invalid.",
                exception);
        }

        var displayName = SanitizeDisplayName(Path.GetFileName(fullPath));
        var extension = NormalizeExtension(Path.GetExtension(displayName));

        return ValidateFileAsync(fullPath, displayName, extension, cancellationToken);
    }

    public Task<DocumentFileValidationResult> ValidateStagedAsync(
        string stagedFilePath,
        string safeDisplayName,
        string normalizedExtension,
        CancellationToken cancellationToken = default)
    {
        var displayName = SanitizeDisplayName(safeDisplayName);
        var extension = NormalizeExtension(normalizedExtension);

        return ValidateFileAsync(stagedFilePath, displayName, extension, cancellationToken);
    }

    private static async Task<DocumentFileValidationResult> ValidateFileAsync(
        string filePath,
        string safeDisplayName,
        string normalizedExtension,
        CancellationToken cancellationToken)
    {
        if (!ValidatedTypes.TryGetValue(normalizedExtension, out var validatedFileType))
        {
            throw Create(
                DocumentRegistrationErrorCode.UnsupportedFileType,
                "File extension is not supported.");
        }

        FileInfo file;
        try
        {
            file = new FileInfo(filePath);
            file.Refresh();
            if (!file.Exists
                || file.Attributes.HasFlag(FileAttributes.Directory)
                || file.Attributes.HasFlag(FileAttributes.ReparsePoint))
            {
                throw Create(
                    DocumentRegistrationErrorCode.SourceUnavailable,
                    "Source must be an existing regular file.");
            }
        }
        catch (DocumentRegistrationException)
        {
            throw;
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
            throw Create(
                DocumentRegistrationErrorCode.SourceUnavailable,
                "Source file information is unavailable.",
                exception);
        }

        if (file.Length == 0)
        {
            throw Create(DocumentRegistrationErrorCode.EmptyFile, "Source file is empty.");
        }

        if (file.Length > MaximumFileSizeBytes)
        {
            throw Create(DocumentRegistrationErrorCode.FileTooLarge, "Source file exceeds the size limit.");
        }

        var initialLength = file.Length;
        var initialLastWriteTimeUtc = file.LastWriteTimeUtc;

        try
        {
            await using var stream = new FileStream(
                file.FullName,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read,
                bufferSize: 81920,
                FileOptions.Asynchronous | FileOptions.SequentialScan);

            var signature = new byte[8];
            var signatureLength = await stream.ReadAsync(signature, cancellationToken);
            ValidateSignature(normalizedExtension, signature.AsSpan(0, signatureLength));

            stream.Position = 0;
            var hash = await SHA256.HashDataAsync(stream, cancellationToken);

            file.Refresh();
            if (!file.Exists
                || file.Length != initialLength
                || file.LastWriteTimeUtc != initialLastWriteTimeUtc)
            {
                throw Create(
                    DocumentRegistrationErrorCode.SourceChanged,
                    "Source file changed while it was being validated.");
            }

            return new DocumentFileValidationResult(
                safeDisplayName,
                normalizedExtension,
                validatedFileType,
                initialLength,
                Convert.ToHexString(hash).ToLowerInvariant(),
                new DateTimeOffset(initialLastWriteTimeUtc, TimeSpan.Zero));
        }
        catch (DocumentRegistrationException)
        {
            throw;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
            throw Create(
                DocumentRegistrationErrorCode.SourceUnavailable,
                "Source file cannot be read.",
                exception);
        }
    }

    private static void ValidateSignature(string extension, ReadOnlySpan<byte> signature)
    {
        var valid = extension switch
        {
            "pdf" => signature.Length >= 5
                && signature[0] == (byte)'%'
                && signature[1] == (byte)'P'
                && signature[2] == (byte)'D'
                && signature[3] == (byte)'F'
                && signature[4] == (byte)'-',
            "jpg" or "jpeg" => signature.Length >= 3
                && signature[0] == 0xFF
                && signature[1] == 0xD8
                && signature[2] == 0xFF,
            "png" => signature.Length >= 8
                && signature.SequenceEqual(
                    new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }),
            _ => false
        };

        if (!valid)
        {
            throw Create(
                DocumentRegistrationErrorCode.UnsupportedFileType,
                "File signature does not match its extension.");
        }
    }

    private static string NormalizeExtension(string extension)
    {
        var normalized = extension.Trim().TrimStart('.').ToLowerInvariant();
        if (!ValidatedTypes.ContainsKey(normalized))
        {
            throw Create(
                DocumentRegistrationErrorCode.UnsupportedFileType,
                "File extension is not supported.");
        }

        return normalized;
    }

    private static string SanitizeDisplayName(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            throw Create(
                DocumentRegistrationErrorCode.SourceUnavailable,
                "Source display name is unavailable.");
        }

        var invalidCharacters = Path.GetInvalidFileNameChars().ToHashSet();
        var sanitized = new string(fileName
            .Where(character => !char.IsControl(character) && !invalidCharacters.Contains(character))
            .ToArray())
            .Trim()
            .TrimEnd('.');

        if (string.IsNullOrWhiteSpace(sanitized))
        {
            throw Create(
                DocumentRegistrationErrorCode.SourceUnavailable,
                "Source display name is invalid.");
        }

        if (sanitized.Length <= 255)
        {
            return sanitized;
        }

        var extension = Path.GetExtension(sanitized);
        var maximumBaseLength = Math.Max(1, 255 - extension.Length);
        var baseName = Path.GetFileNameWithoutExtension(sanitized);

        return $"{baseName[..Math.Min(baseName.Length, maximumBaseLength)]}{extension}";
    }

    private static DocumentRegistrationException Create(
        DocumentRegistrationErrorCode errorCode,
        string message,
        Exception? innerException = null)
    {
        return new DocumentRegistrationException(errorCode, message, innerException);
    }
}
