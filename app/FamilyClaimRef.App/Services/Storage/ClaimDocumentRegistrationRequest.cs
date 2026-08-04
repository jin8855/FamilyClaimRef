namespace FamilyClaimRef.App.Services.Storage;

public sealed record ClaimDocumentRegistrationRequest(
    string SourceFilePath,
    string ClaimId,
    string DocumentType,
    string DisplayTitle,
    DateOnly ReferenceDate,
    DocumentFileValidationResult? SelectionSnapshot = null);
