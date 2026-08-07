using System.Text.Json.Serialization;

namespace FamilyClaimRef.App.Models.Storage;

public sealed record CategoryDraft(
    string Name,
    string Code,
    int SortOrder,
    string? Description,
    bool IsSystemDefault);

public sealed record CategoryItemDraft(
    string Name,
    string Code,
    int SortOrder,
    string? Description,
    bool UseForPolicySearch,
    bool UseForHistorySearch);

public sealed record CategoryItemRecord(
    Guid RowId,
    Guid ParentCategoryId,
    string Name,
    string Code,
    int SortOrder,
    string? Description,
    bool UseForPolicySearch,
    bool UseForHistorySearch,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    DateTimeOffset? DisabledAt)
{
    [JsonIgnore]
    public bool IsActive => DisabledAt is null;

    public override string ToString() => Name;
}

public sealed record CategoryRecord(
    Guid RowId,
    string Name,
    string Code,
    int SortOrder,
    string? Description,
    bool IsSystemDefault,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    DateTimeOffset? DisabledAt,
    IReadOnlyList<CategoryItemRecord> Items)
{
    [JsonIgnore]
    public bool IsActive => DisabledAt is null;

    [JsonIgnore]
    public int ActiveItemCount => Items.Count(item => item.IsActive);

    public override string ToString() => Name;
}

public sealed record CategoryAggregateSnapshot(
    int SchemaVersion,
    long AggregateVersion,
    IReadOnlyList<CategoryRecord> Categories);

public sealed record CategoryMutationResult<T>(
    CategoryAggregateSnapshot Snapshot,
    T Record);

internal sealed class CategoryAggregateEnvelope
{
    public int SchemaVersion { get; init; }

    public long AggregateVersion { get; init; }

    public DateTimeOffset SavedAt { get; init; }

    public List<CategoryRecord> Categories { get; init; } = [];
}
