namespace FamilyClaimRef.App.Models.Storage;

public sealed record class PolicyDocumentDraft(
    string PolicyId,
    string DocumentId,
    string DocumentType);
