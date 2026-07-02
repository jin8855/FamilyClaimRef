namespace FamilyClaimRef.App.Services.Storage;

public sealed record PolicyDocumentLinkRequest(
    string PolicyId,
    string DocumentId,
    string DocumentType);
