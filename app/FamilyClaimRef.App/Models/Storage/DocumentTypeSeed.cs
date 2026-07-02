namespace FamilyClaimRef.App.Models.Storage;

public sealed record class DocumentTypeSeed(
    string Code,
    string Label,
    string Scope,
    int SortOrder,
    DateTimeOffset? DisabledAt);
