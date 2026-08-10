namespace FamilyClaimRef.App.Models.Storage;

public sealed record class ClaimPaymentRecord(
    string Id,
    string ClaimSubmissionId,
    string Status,
    DateOnly? PaidDate,
    long? PaidAmount,
    string? PaidCoverageDisplayName,
    string? DenyReason,
    string? ReductionReason,
    string? AdditionalDocumentsMemo,
    string? Memo,
    int Revision,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);
