namespace FamilyClaimRef.App.Services.Storage;

public sealed record FileAttachmentCopyResult(
    string RelativePath,
    string PhysicalFileName,
    string Extension,
    long SizeBytes,
    string? ValidatedFileType = null,
    string? Sha256 = null,
    string? OriginalDisplayFileName = null);
