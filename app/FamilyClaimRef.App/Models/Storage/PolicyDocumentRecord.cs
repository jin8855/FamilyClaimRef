namespace FamilyClaimRef.App.Models.Storage;

public sealed record class PolicyDocumentRecord(
    string Id,
    string PolicyId,
    string DocumentId,
    string DocumentType,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    DateTimeOffset? DisabledAt);
