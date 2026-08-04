namespace FamilyClaimRef.App.Services.Storage;

public sealed record StagedFileAttachment(
    string RelativePath,
    string FullPath,
    DocumentFileValidationResult? Validation = null);
