namespace FamilyClaimRef.App.Services.Storage;

public sealed record PolicyDocumentRegistrationResult(
    DocumentAttachmentResult Attachment,
    PolicyDocumentLinkResult Link);
