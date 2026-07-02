using FamilyClaimRef.App.Models.Storage;

namespace FamilyClaimRef.App.Services.Storage;

public sealed record DocumentAttachmentResult(
    DocumentRecord Document,
    FileAttachmentCopyResult File);
