namespace FamilyClaimRef.App.Services.Storage;

public sealed record FileAttachmentCopyResult(
    string RelativePath,
    string PhysicalFileName,
    string Extension,
    long SizeBytes);
