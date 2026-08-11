using System.Text.Json.Serialization;

namespace FamilyClaimRef.App.Models.Storage;

public sealed record class JsonFileEnvelope<T>
{
    public int SchemaVersion { get; init; }

    public DateTimeOffset SavedAt { get; init; }

    [JsonRequired]
    public List<T> Items { get; init; } = [];
}
