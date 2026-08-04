namespace FamilyClaimRef.App.Services.Storage;

public sealed record PolicyDocumentRegistrationRequest(
    string SourceFilePath,
    string PolicyId,
    string DocumentType,
    string DisplayTitle,
    DateOnly ReferenceDate,
    DocumentFileValidationResult? SelectionSnapshot = null);
