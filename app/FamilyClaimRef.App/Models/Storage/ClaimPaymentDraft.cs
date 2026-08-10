namespace FamilyClaimRef.App.Models.Storage;

public sealed record class ClaimPaymentDraft(
    string ClaimSubmissionId,
    string Status,
    DateOnly? PaidDate,
    long? PaidAmount,
    string? PaidCoverageDisplayName,
    string? DenyReason,
    string? ReductionReason,
    string? AdditionalDocumentsMemo,
    string? Memo);
