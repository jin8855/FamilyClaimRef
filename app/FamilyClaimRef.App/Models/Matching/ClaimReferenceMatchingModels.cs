using FamilyClaimRef.App.Models.Storage;

namespace FamilyClaimRef.App.Models.Matching;

public sealed record ClaimReferenceMatchingRequest(
    string SelectedClaimCaseId,
    string? AnchorPolicyCoverageId,
    IReadOnlyList<FamilyMemberRecord>? FamilyMembers,
    IReadOnlyList<PolicyRecord>? Policies,
    IReadOnlyList<PolicyCoverageRecord>? PolicyCoverages,
    IReadOnlyList<ClaimRecord>? ClaimCases,
    IReadOnlyList<ClaimSubmissionRecord>? ClaimSubmissions,
    IReadOnlyList<ClaimPaymentRecord>? ClaimPayments,
    IReadOnlyList<PolicyDocumentRecord>? PolicyDocuments);

public sealed record ClaimReferenceProjection(
    IReadOnlyList<ClaimReferenceCoverageResult> CoverageResults,
    IReadOnlyList<ClaimReferenceSimilarClaim> SimilarClaims,
    bool HasExcludedUnconfirmedCoverages);

public sealed record ClaimReferenceCoverageResult(
    string PolicyId,
    string PolicyCoverageId,
    string PolicyDisplayName,
    string CoverageDisplayName,
    string ResultGroup,
    int PassedRuleCount,
    IReadOnlyList<ClaimReferenceRuleEvidence> RuleEvidence,
    bool HasSourcePolicyDocument);

public sealed record ClaimReferenceRuleEvidence(
    string RuleName,
    string Outcome);

public sealed record ClaimReferenceSimilarClaim(
    string ClaimCaseId,
    string ClaimSubmissionId,
    string PolicyId,
    string PolicyDisplayName,
    string PolicyContractStatus,
    bool IsPolicyCurrentlyActive,
    string SimilarityTier,
    DateOnly TreatmentDate,
    string VisitType,
    DateTimeOffset SubmissionUpdatedAt,
    IReadOnlyList<ClaimReferencePaymentFact> TerminalPaymentFacts);

public sealed record ClaimReferencePaymentFact(
    string Status,
    DateOnly? PaidDate,
    long? PaidAmount,
    string? PaidCoverageDisplayName);
