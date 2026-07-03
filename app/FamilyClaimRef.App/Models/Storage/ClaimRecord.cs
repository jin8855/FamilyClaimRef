namespace FamilyClaimRef.App.Models.Storage;

public sealed record class ClaimRecord(
    string Id,
    string PolicyId,
    string DisplayTitle,
    DateOnly ReferenceDate,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    DateTimeOffset? DisabledAt);
