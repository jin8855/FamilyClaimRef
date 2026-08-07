namespace FamilyClaimRef.App.Models.Storage;

public sealed record class ClaimSubmissionRecord(
    string Id,
    string ClaimCaseId,
    string PolicyId,
    string? PolicyCoverageId,
    string? CoverageDisplayName,
    DateOnly? SubmittedDate,
    long? SubmittedAmount,
    string[] SubmittedClaimDocumentIds,
    string Status,
    string? Memo,
    int Revision,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);
