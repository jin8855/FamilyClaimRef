namespace FamilyClaimRef.App.Models.Storage;

public sealed record class DocumentRecord(
    string Id,
    string PhysicalFileName,
    string DisplayTitle,
    string Extension,
    string RelativePath,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    DateTimeOffset? DisabledAt);
