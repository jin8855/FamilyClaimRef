namespace FamilyClaimRef.App.Services.Storage;

public sealed record DocumentAttachmentRequest(
    string SourceFilePath,
    string DocumentScope,
    string DocumentType,
    string DisplayTitle,
    DateOnly? ReferenceDate,
    DocumentFileValidationResult? SelectionSnapshot = null);
