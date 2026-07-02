namespace FamilyClaimRef.App.Models.Storage;

public sealed record class ClaimDocumentDraft(
    string ClaimId,
    string DocumentId,
    string DocumentType);
