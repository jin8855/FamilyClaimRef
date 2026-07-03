namespace FamilyClaimRef.App.Models.Storage;

public sealed record class PolicyRecord(
    string Id,
    string DisplayTitle,
    DateOnly ReferenceDate,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    DateTimeOffset? DisabledAt);
