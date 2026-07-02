namespace FamilyClaimRef.App.Models.Storage;

public sealed record class ClaimDocumentRecord(
    string Id,
    string ClaimId,
    string DocumentId,
    string DocumentType,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    DateTimeOffset? DisabledAt);
