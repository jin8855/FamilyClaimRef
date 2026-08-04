namespace FamilyClaimRef.App.Models.Storage;

public sealed record class DocumentDraft(
    string PhysicalFileName,
    string DisplayTitle,
    string Extension,
    string RelativePath,
    string? OriginalDisplayFileName = null,
    string? ValidatedFileType = null,
    long? ByteLength = null,
    string? Sha256 = null,
    DateOnly? ReferenceDate = null,
    string? DocumentType = null);
