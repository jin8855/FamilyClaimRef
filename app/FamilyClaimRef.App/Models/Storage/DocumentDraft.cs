namespace FamilyClaimRef.App.Models.Storage;

public sealed record class DocumentDraft(
    string PhysicalFileName,
    string DisplayTitle,
    string Extension,
    string RelativePath);
