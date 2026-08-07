namespace FamilyClaimRef.App.Models.Storage;

public sealed record class ClaimSubmissionDraft(
    string ClaimCaseId,
    string PolicyId,
    string? PolicyCoverageId,
    string? CoverageDisplayName,
    DateOnly? SubmittedDate,
    long? SubmittedAmount,
    IReadOnlyList<string>? SubmittedClaimDocumentIds,
    string Status,
    string? Memo);
