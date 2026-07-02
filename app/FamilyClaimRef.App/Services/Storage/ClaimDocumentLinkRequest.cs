namespace FamilyClaimRef.App.Services.Storage;

public sealed record ClaimDocumentLinkRequest(
    string ClaimId,
    string DocumentId,
    string DocumentType);
