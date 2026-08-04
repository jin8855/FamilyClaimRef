namespace FamilyClaimRef.App.Models.Storage;

public sealed record FamilyMemberRecord(
    string Id,
    string DisplayName,
    string Relation,
    string? Memo,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    DateTimeOffset? DisabledAt,
    int Version);
