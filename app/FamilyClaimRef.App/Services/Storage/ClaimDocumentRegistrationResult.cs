namespace FamilyClaimRef.App.Services.Storage;

public sealed record ClaimDocumentRegistrationResult(
    DocumentAttachmentResult Attachment,
    ClaimDocumentLinkResult Link);
