using System.Text.Json.Serialization;

namespace FamilyClaimRef.App.Models.Storage;

public sealed record class DocumentRecord(
    string Id,
    string PhysicalFileName,
    string DisplayTitle,
    string Extension,
    string RelativePath,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    DateTimeOffset? DisabledAt,
    string? OriginalDisplayFileName = null,
    string? ValidatedFileType = null,
    long? ByteLength = null,
    string? Sha256 = null,
    DateOnly? ReferenceDate = null,
    string? DocumentType = null)
{
    [JsonIgnore]
    public bool IsDisabled => DisabledAt is not null;

    [JsonIgnore]
    public string? DeclaredContentType => ValidatedFileType switch
    {
        "PDF" => "application/pdf",
        "JPEG" => "image/jpeg",
        "PNG" => "image/png",
        _ => null
    };
}
