using FamilyClaimRef.App.Models.Matching;
using FamilyClaimRef.App.Models.Storage;

namespace FamilyClaimRef.App.Services.Matching;

public sealed class ClaimReferenceMatchingEngine : IClaimReferenceMatchingEngine
{
    public ClaimReferenceProjection BuildProjection(ClaimReferenceMatchingRequest request)
    {
        if (request is null)
        {
            throw InvalidGraph();
        }

        var graph = ValidateGraph(request);
        var selectedClaim = RequireSelectedClaim(request.SelectedClaimCaseId, graph);
        var selectedFamily = graph.FamilyMembers[selectedClaim.FamilyMemberId!];
        EnsureSelectedClaimIsSearchable(selectedClaim, selectedFamily);
        var anchorCoverageId = NormalizeOptionalIdentifier(request.AnchorPolicyCoverageId);
        ValidateAnchorCoverage(anchorCoverageId, selectedClaim.FamilyMemberId!, graph);

        var eligiblePolicies = graph.Policies.Values
            .Where(policy => string.Equals(
                policy.FamilyMemberId,
                selectedClaim.FamilyMemberId,
                StringComparison.Ordinal))
            .Where(IsPolicyEligibleForCurrentMatching)
            .ToDictionary(policy => policy.Id, StringComparer.Ordinal);

        var hasExcludedUnconfirmedCoverages = graph.PolicyCoverages.Values.Any(coverage =>
            coverage.DisabledAt is null
            && eligiblePolicies.ContainsKey(coverage.PolicyId)
            && (string.Equals(
                    coverage.ReviewStatus,
                    PolicyCoverageValues.ReviewStatusCandidate,
                    StringComparison.Ordinal)
                || string.Equals(
                    coverage.ReviewStatus,
                    PolicyCoverageValues.ReviewStatusNeedsReview,
                    StringComparison.Ordinal)));

        var coverageResults = graph.PolicyCoverages.Values
            .Where(coverage => coverage.DisabledAt is null)
            .Where(coverage => string.Equals(
                coverage.ReviewStatus,
                PolicyCoverageValues.ReviewStatusUserConfirmed,
                StringComparison.Ordinal))
            .Where(coverage => eligiblePolicies.ContainsKey(coverage.PolicyId))
            .Select(coverage => BuildCoverageResult(
                selectedClaim,
                eligiblePolicies[coverage.PolicyId],
                coverage))
            .OrderBy(result => GetResultGroupRank(result.ResultGroup))
            .ThenByDescending(result => result.PassedRuleCount)
            .ThenBy(result => result.PolicyDisplayName, StringComparer.Ordinal)
            .ThenBy(result => result.CoverageDisplayName, StringComparer.Ordinal)
            .ThenBy(result => result.PolicyCoverageId, StringComparer.Ordinal)
            .ToArray();

        var similarClaims = BuildSimilarClaims(
            selectedClaim,
            anchorCoverageId,
            graph);

        return new ClaimReferenceProjection(
            coverageResults,
            similarClaims,
            hasExcludedUnconfirmedCoverages);
    }

    private static ValidatedGraph ValidateGraph(ClaimReferenceMatchingRequest request)
    {
        var familyMembers = BuildUniqueLookup(
            request.FamilyMembers,
            record => record.Id);
        var policies = BuildUniqueLookup(request.Policies, record => record.Id);
        var coverages = BuildUniqueLookup(
            request.PolicyCoverages,
            record => record.PolicyCoverageId);
        var claims = BuildUniqueLookup(request.ClaimCases, record => record.Id);
        var submissions = BuildUniqueLookup(
            request.ClaimSubmissions,
            record => record.Id);
        var payments = BuildUniqueLookup(request.ClaimPayments, record => record.Id);
        var policyDocuments = BuildUniqueLookup(
            request.PolicyDocuments,
            record => record.Id);

        foreach (var family in familyMembers.Values)
        {
            ValidateFamilyMember(family);
        }

        foreach (var policy in policies.Values)
        {
            ValidatePolicy(policy, familyMembers);
        }

        foreach (var policyDocument in policyDocuments.Values)
        {
            ValidatePolicyDocument(policyDocument, policies);
        }

        foreach (var coverage in coverages.Values)
        {
            ValidatePolicyCoverage(coverage, policies, policyDocuments);
        }

        foreach (var claim in claims.Values)
        {
            ValidateClaimCase(claim, familyMembers, policies);
        }

        foreach (var submission in submissions.Values)
        {
            ValidateClaimSubmission(submission, claims, policies, coverages);
        }

        foreach (var payment in payments.Values)
        {
            ValidateClaimPayment(payment, submissions);
        }

        return new ValidatedGraph(
            familyMembers,
            policies,
            coverages,
            claims,
            submissions,
            payments,
            policyDocuments);
    }

    private static Dictionary<string, T> BuildUniqueLookup<T>(
        IReadOnlyList<T>? records,
        Func<T, string> idSelector)
        where T : class
    {
        if (records is null)
        {
            throw InvalidGraph();
        }

        var lookup = new Dictionary<string, T>(StringComparer.Ordinal);
        foreach (var record in records)
        {
            if (record is null)
            {
                throw InvalidGraph();
            }

            var id = idSelector(record);
            if (!IsNormalizedRequired(id) || !lookup.TryAdd(id, record))
            {
                throw InvalidGraph();
            }
        }

        return lookup;
    }

    private static void ValidateFamilyMember(FamilyMemberRecord record)
    {
        if (!IsNormalizedRequired(record.DisplayName)
            || !IsNormalizedRequired(record.Relation)
            || !IsNormalizedOptional(record.Memo)
            || record.Version < 1
            || !HasValidLifecycle(record.CreatedAt, record.UpdatedAt, record.DisabledAt))
        {
            throw InvalidGraph();
        }
    }

    private static void ValidatePolicy(
        PolicyRecord record,
        IReadOnlyDictionary<string, FamilyMemberRecord> familyMembers)
    {
        if (!IsNormalizedRequired(record.DisplayTitle)
            || !IsNormalizedRequired(record.FamilyMemberId)
            || !familyMembers.ContainsKey(record.FamilyMemberId!)
            || !IsKnownPolicyStatus(record.ContractStatus)
            || !IsValidDate(record.ReferenceDate)
            || !IsValidDate(record.EnrollmentDate)
            || !HasValidLifecycle(record.CreatedAt, record.UpdatedAt, record.DisabledAt))
        {
            throw InvalidGraph();
        }
    }

    private static void ValidatePolicyDocument(
        PolicyDocumentRecord record,
        IReadOnlyDictionary<string, PolicyRecord> policies)
    {
        if (!IsNormalizedRequired(record.PolicyId)
            || !policies.ContainsKey(record.PolicyId)
            || !IsNormalizedRequired(record.DocumentId)
            || !IsNormalizedRequired(record.DocumentType)
            || !HasValidLifecycle(record.CreatedAt, record.UpdatedAt, record.DisabledAt))
        {
            throw InvalidGraph();
        }
    }

    private static void ValidatePolicyCoverage(
        PolicyCoverageRecord record,
        IReadOnlyDictionary<string, PolicyRecord> policies,
        IReadOnlyDictionary<string, PolicyDocumentRecord> policyDocuments)
    {
        if (!IsNormalizedRequired(record.PolicyId)
            || !policies.ContainsKey(record.PolicyId)
            || !IsNormalizedRequired(record.DisplayName)
            || !PolicyCoverageValues.ReviewStatuses.Contains(
                record.ReviewStatus,
                StringComparer.Ordinal)
            || !PolicyCoverageValues.VisitTypeRules.Contains(
                record.VisitTypeRule,
                StringComparer.Ordinal)
            || !PolicyCoverageValues.ConditionRules.Contains(
                record.SurgeryRule,
                StringComparer.Ordinal)
            || !PolicyCoverageValues.ConditionRules.Contains(
                record.PrescriptionRule,
                StringComparer.Ordinal)
            || !PolicyCoverageValues.DiagnosisRuleModes.Contains(
                record.DiagnosisRuleMode,
                StringComparer.Ordinal)
            || !PolicyCoverageValues.SourceKinds.Contains(record.SourceKind, StringComparer.Ordinal)
            || !IsNormalizedOptional(record.SourcePolicyDocumentId)
            || record.DiagnosisCodePrefixes is null
            || !IsValidDate(record.EffectiveFrom)
            || !IsValidDate(record.EffectiveTo)
            || (record.EffectiveFrom is not null
                && record.EffectiveTo is not null
                && record.EffectiveFrom > record.EffectiveTo)
            || record.Revision < 1
            || !HasValidLifecycle(record.CreatedAt, record.UpdatedAt, record.DisabledAt))
        {
            throw InvalidGraph();
        }

        var prefixes = new HashSet<string>(StringComparer.Ordinal);
        foreach (var prefix in record.DiagnosisCodePrefixes)
        {
            if (!IsNormalizedRequired(prefix)
                || !string.Equals(prefix, prefix.ToUpperInvariant(), StringComparison.Ordinal)
                || !prefixes.Add(prefix))
            {
                throw InvalidGraph();
            }
        }

        if (string.Equals(
                record.DiagnosisRuleMode,
                PolicyCoverageValues.DiagnosisRulePrefixList,
                StringComparison.Ordinal)
            && record.DiagnosisCodePrefixes.Length == 0)
        {
            throw InvalidGraph();
        }

        if (string.Equals(
                record.SourceKind,
                PolicyCoverageValues.SourcePolicyDocument,
                StringComparison.Ordinal)
            && record.SourcePolicyDocumentId is null)
        {
            throw InvalidGraph();
        }

        if (string.Equals(
                record.SourceKind,
                PolicyCoverageValues.SourceManual,
                StringComparison.Ordinal)
            && record.SourcePolicyDocumentId is not null)
        {
            throw InvalidGraph();
        }

        if (record.SourcePolicyDocumentId is not null
            && (!policyDocuments.TryGetValue(record.SourcePolicyDocumentId, out var document)
                || !string.Equals(document.PolicyId, record.PolicyId, StringComparison.Ordinal)))
        {
            throw InvalidGraph();
        }
    }

    private static void ValidateClaimCase(
        ClaimRecord record,
        IReadOnlyDictionary<string, FamilyMemberRecord> familyMembers,
        IReadOnlyDictionary<string, PolicyRecord> policies)
    {
        if (!IsNormalizedRequired(record.DisplayTitle)
            || !IsNormalizedRequired(record.FamilyMemberId)
            || !familyMembers.ContainsKey(record.FamilyMemberId!)
            || (record.ReferenceDate == default)
            || !IsKnownClaimCaseStatus(record.CaseStatus)
            || (record.VisitType is not null
                && !ClaimCaseValues.VisitTypes.Contains(record.VisitType, StringComparer.Ordinal))
            || record.Revision < 0
            || !HasValidLifecycle(record.CreatedAt, record.UpdatedAt, record.DisabledAt))
        {
            throw InvalidGraph();
        }

        if (record.PolicyId is not null
            && (!IsNormalizedRequired(record.PolicyId)
                || !policies.TryGetValue(record.PolicyId, out var policy)
                || !string.Equals(
                    policy.FamilyMemberId,
                    record.FamilyMemberId,
                    StringComparison.Ordinal)))
        {
            throw InvalidGraph();
        }
    }

    private static void ValidateClaimSubmission(
        ClaimSubmissionRecord record,
        IReadOnlyDictionary<string, ClaimRecord> claims,
        IReadOnlyDictionary<string, PolicyRecord> policies,
        IReadOnlyDictionary<string, PolicyCoverageRecord> coverages)
    {
        if (!IsNormalizedRequired(record.ClaimCaseId)
            || !claims.TryGetValue(record.ClaimCaseId, out var claim)
            || !IsNormalizedRequired(record.PolicyId)
            || !policies.TryGetValue(record.PolicyId, out var policy)
            || !string.Equals(policy.FamilyMemberId, claim.FamilyMemberId, StringComparison.Ordinal)
            || !ClaimSubmissionValues.Statuses.Contains(record.Status, StringComparer.Ordinal)
            || !IsNormalizedOptional(record.PolicyCoverageId)
            || !IsNormalizedOptional(record.CoverageDisplayName)
            || !IsNormalizedOptional(record.Memo)
            || !IsValidDate(record.SubmittedDate)
            || record.SubmittedAmount < 0
            || record.SubmittedClaimDocumentIds is null
            || record.Revision < 1
            || !HasValidLifecycle(record.CreatedAt, record.UpdatedAt, null))
        {
            throw InvalidGraph();
        }

        if (ClaimSubmissionValues.RequiresSubmittedDetails(record.Status)
            && (record.SubmittedDate is null
                || !IsNormalizedRequired(record.CoverageDisplayName)))
        {
            throw InvalidGraph();
        }

        var documentIds = new HashSet<string>(StringComparer.Ordinal);
        foreach (var documentId in record.SubmittedClaimDocumentIds)
        {
            if (!IsNormalizedRequired(documentId) || !documentIds.Add(documentId))
            {
                throw InvalidGraph();
            }
        }

        if (record.PolicyCoverageId is not null
            && (!coverages.TryGetValue(record.PolicyCoverageId, out var coverage)
                || !string.Equals(coverage.PolicyId, record.PolicyId, StringComparison.Ordinal)))
        {
            throw InvalidGraph();
        }
    }

    private static void ValidateClaimPayment(
        ClaimPaymentRecord record,
        IReadOnlyDictionary<string, ClaimSubmissionRecord> submissions)
    {
        if (!IsNormalizedRequired(record.ClaimSubmissionId)
            || !submissions.ContainsKey(record.ClaimSubmissionId)
            || !ClaimPaymentValues.Statuses.Contains(record.Status, StringComparer.Ordinal)
            || !IsValidDate(record.PaidDate)
            || record.PaidAmount is <= 0
            || !IsNormalizedOptional(record.PaidCoverageDisplayName)
            || !IsNormalizedOptional(record.DenyReason)
            || !IsNormalizedOptional(record.ReductionReason)
            || !IsNormalizedOptional(record.AdditionalDocumentsMemo)
            || !IsNormalizedOptional(record.Memo)
            || !HasValidClaimPaymentStatusFields(record)
            || record.Revision < 1
            || !HasValidLifecycle(record.CreatedAt, record.UpdatedAt, null))
        {
            throw InvalidGraph();
        }
    }

    private static bool HasValidClaimPaymentStatusFields(ClaimPaymentRecord record)
    {
        return record.Status switch
        {
            ClaimPaymentValues.StatusPending => true,
            ClaimPaymentValues.StatusPaid => HasRequiredPaidFields(record)
                && record.DenyReason is null
                && record.ReductionReason is null,
            ClaimPaymentValues.StatusPartiallyPaid => HasRequiredPaidFields(record)
                && IsNormalizedRequired(record.ReductionReason)
                && record.DenyReason is null,
            ClaimPaymentValues.StatusDenied => IsNormalizedRequired(record.DenyReason)
                && record.PaidDate is null
                && record.PaidAmount is null
                && record.PaidCoverageDisplayName is null
                && record.ReductionReason is null,
            ClaimPaymentValues.StatusCancelled => record.PaidDate is null
                && record.PaidAmount is null
                && record.PaidCoverageDisplayName is null
                && record.DenyReason is null
                && record.ReductionReason is null
                && record.AdditionalDocumentsMemo is null,
            _ => false
        };
    }

    private static bool HasRequiredPaidFields(ClaimPaymentRecord record)
    {
        return record.PaidDate is not null
            && record.PaidAmount is not null
            && IsNormalizedRequired(record.PaidCoverageDisplayName);
    }

    private static ClaimRecord RequireSelectedClaim(
        string selectedClaimCaseId,
        ValidatedGraph graph)
    {
        if (!IsNormalizedRequired(selectedClaimCaseId)
            || !graph.ClaimCases.TryGetValue(selectedClaimCaseId, out var selectedClaim))
        {
            throw SelectedClaimUnavailable();
        }

        return selectedClaim;
    }

    private static void EnsureSelectedClaimIsSearchable(
        ClaimRecord claim,
        FamilyMemberRecord familyMember)
    {
        if (!string.Equals(claim.CaseStatus, ClaimCaseValues.StatusSaved, StringComparison.Ordinal)
            || claim.DisabledAt is not null
            || familyMember.DisabledAt is not null
            || claim.ReferenceDate == default
            || !ClaimCaseValues.VisitTypes.Contains(claim.VisitType!, StringComparer.Ordinal))
        {
            throw SelectedClaimUnavailable();
        }
    }

    private static void ValidateAnchorCoverage(
        string? anchorCoverageId,
        string selectedFamilyMemberId,
        ValidatedGraph graph)
    {
        if (anchorCoverageId is null)
        {
            return;
        }

        if (!graph.PolicyCoverages.TryGetValue(anchorCoverageId, out var coverage)
            || !graph.Policies.TryGetValue(coverage.PolicyId, out var policy)
            || !string.Equals(
                policy.FamilyMemberId,
                selectedFamilyMemberId,
                StringComparison.Ordinal))
        {
            throw InvalidGraph();
        }
    }

    private static ClaimReferenceCoverageResult BuildCoverageResult(
        ClaimRecord selectedClaim,
        PolicyRecord policy,
        PolicyCoverageRecord coverage)
    {
        var evidence = new[]
        {
            new ClaimReferenceRuleEvidence(
                ClaimReferenceMatchingValues.RulePolicyStatus,
                ClaimReferenceMatchingValues.OutcomePassed),
            new ClaimReferenceRuleEvidence(
                ClaimReferenceMatchingValues.RuleTreatmentDate,
                EvaluateTreatmentDate(selectedClaim.ReferenceDate, policy, coverage)),
            new ClaimReferenceRuleEvidence(
                ClaimReferenceMatchingValues.RuleVisitType,
                EvaluateVisitType(selectedClaim.VisitType!, coverage.VisitTypeRule)),
            new ClaimReferenceRuleEvidence(
                ClaimReferenceMatchingValues.RuleSurgery,
                EvaluateBooleanCondition(selectedClaim.HasSurgery, coverage.SurgeryRule)),
            new ClaimReferenceRuleEvidence(
                ClaimReferenceMatchingValues.RulePrescription,
                EvaluateBooleanCondition(selectedClaim.HasPrescription, coverage.PrescriptionRule)),
            new ClaimReferenceRuleEvidence(
                ClaimReferenceMatchingValues.RuleDiagnosisCode,
                EvaluateDiagnosis(selectedClaim.DiagnosisCode, coverage)),
            new ClaimReferenceRuleEvidence(
                ClaimReferenceMatchingValues.RuleSourceDocument,
                coverage.SourcePolicyDocumentId is null
                    ? ClaimReferenceMatchingValues.OutcomeNotApplicable
                    : ClaimReferenceMatchingValues.OutcomePassed)
        };

        var resultGroup = evidence.Any(item => string.Equals(
                item.Outcome,
                ClaimReferenceMatchingValues.OutcomeMismatch,
                StringComparison.Ordinal))
            ? ClaimReferenceMatchingValues.ResultGroupCurrentInputMismatch
            : evidence.Any(item => string.Equals(
                item.Outcome,
                ClaimReferenceMatchingValues.OutcomeNeedsConfirmation,
                StringComparison.Ordinal))
                ? ClaimReferenceMatchingValues.ResultGroupNeedsConfirmation
                : ClaimReferenceMatchingValues.ResultGroupConditionMatch;
        var passedRuleCount = evidence.Count(item =>
            ClaimReferenceMatchingValues.ScoredRuleNames.Contains(
                item.RuleName,
                StringComparer.Ordinal)
            && string.Equals(
                item.Outcome,
                ClaimReferenceMatchingValues.OutcomePassed,
                StringComparison.Ordinal));

        return new ClaimReferenceCoverageResult(
            policy.Id,
            coverage.PolicyCoverageId,
            policy.DisplayTitle,
            coverage.DisplayName,
            resultGroup,
            passedRuleCount,
            evidence,
            coverage.SourcePolicyDocumentId is not null);
    }

    private static string EvaluateTreatmentDate(
        DateOnly treatmentDate,
        PolicyRecord policy,
        PolicyCoverageRecord coverage)
    {
        if ((policy.EnrollmentDate is not null && treatmentDate < policy.EnrollmentDate)
            || (coverage.EffectiveFrom is not null && treatmentDate < coverage.EffectiveFrom)
            || (coverage.EffectiveTo is not null && treatmentDate > coverage.EffectiveTo))
        {
            return ClaimReferenceMatchingValues.OutcomeMismatch;
        }

        return policy.EnrollmentDate is null && coverage.EffectiveFrom is null
            ? ClaimReferenceMatchingValues.OutcomeNeedsConfirmation
            : ClaimReferenceMatchingValues.OutcomePassed;
    }

    private static string EvaluateVisitType(string visitType, string rule)
    {
        return string.Equals(rule, PolicyCoverageValues.VisitTypeAny, StringComparison.Ordinal)
            || string.Equals(rule, visitType, StringComparison.Ordinal)
                ? ClaimReferenceMatchingValues.OutcomePassed
                : ClaimReferenceMatchingValues.OutcomeMismatch;
    }

    private static string EvaluateBooleanCondition(bool actual, string rule)
    {
        if (string.Equals(rule, PolicyCoverageValues.ConditionAny, StringComparison.Ordinal))
        {
            return ClaimReferenceMatchingValues.OutcomePassed;
        }

        var passed = string.Equals(rule, PolicyCoverageValues.ConditionRequired, StringComparison.Ordinal)
            ? actual
            : !actual;
        return passed
            ? ClaimReferenceMatchingValues.OutcomePassed
            : ClaimReferenceMatchingValues.OutcomeMismatch;
    }

    private static string EvaluateDiagnosis(
        string? diagnosisCode,
        PolicyCoverageRecord coverage)
    {
        if (string.Equals(
                coverage.DiagnosisRuleMode,
                PolicyCoverageValues.DiagnosisRuleAny,
                StringComparison.Ordinal))
        {
            return ClaimReferenceMatchingValues.OutcomePassed;
        }

        var normalizedCode = NormalizeDiagnosisCode(diagnosisCode);
        if (normalizedCode is null)
        {
            return ClaimReferenceMatchingValues.OutcomeNeedsConfirmation;
        }

        return coverage.DiagnosisCodePrefixes.Any(prefix => normalizedCode.StartsWith(
            prefix,
            StringComparison.Ordinal))
            ? ClaimReferenceMatchingValues.OutcomePassed
            : ClaimReferenceMatchingValues.OutcomeMismatch;
    }

    private static IReadOnlyList<ClaimReferenceSimilarClaim> BuildSimilarClaims(
        ClaimRecord selectedClaim,
        string? anchorCoverageId,
        ValidatedGraph graph)
    {
        var selectedDiagnosis = NormalizeDiagnosisCode(selectedClaim.DiagnosisCode);
        return graph.ClaimSubmissions.Values
            .Where(submission => string.Equals(
                submission.Status,
                ClaimSubmissionValues.StatusCompleted,
                StringComparison.Ordinal))
            .Select(submission => BuildSimilarClaimCandidate(
                submission,
                selectedClaim,
                selectedDiagnosis,
                anchorCoverageId,
                graph))
            .Where(candidate => candidate is not null)
            .Select(candidate => candidate!)
            .OrderBy(candidate => GetSimilarityTierRank(candidate.Result.SimilarityTier))
            .ThenByDescending(candidate => candidate.Result.TreatmentDate)
            .ThenByDescending(candidate => candidate.Result.SubmissionUpdatedAt)
            .ThenBy(candidate => candidate.Result.ClaimSubmissionId, StringComparer.Ordinal)
            .Take(3)
            .Select(candidate => candidate.Result)
            .ToArray();
    }

    private static SimilarClaimCandidate? BuildSimilarClaimCandidate(
        ClaimSubmissionRecord submission,
        ClaimRecord selectedClaim,
        string? selectedDiagnosis,
        string? anchorCoverageId,
        ValidatedGraph graph)
    {
        var claim = graph.ClaimCases[submission.ClaimCaseId];
        if (string.Equals(claim.Id, selectedClaim.Id, StringComparison.Ordinal)
            || !string.Equals(claim.FamilyMemberId, selectedClaim.FamilyMemberId, StringComparison.Ordinal)
            || !string.Equals(claim.CaseStatus, ClaimCaseValues.StatusSaved, StringComparison.Ordinal)
            || claim.DisabledAt is not null
            || !ClaimCaseValues.VisitTypes.Contains(claim.VisitType!, StringComparer.Ordinal))
        {
            return null;
        }

        var terminalPayments = graph.ClaimPayments.Values
            .Where(payment => string.Equals(
                payment.ClaimSubmissionId,
                submission.Id,
                StringComparison.Ordinal))
            .Where(payment => IsEligibleTerminalPaymentStatus(payment.Status))
            .OrderByDescending(payment => payment.UpdatedAt)
            .ThenBy(payment => payment.Id, StringComparer.Ordinal)
            .ToArray();
        if (terminalPayments.Length == 0)
        {
            return null;
        }

        var similarityTier = GetSimilarityTier(
            anchorCoverageId,
            submission.PolicyCoverageId,
            selectedDiagnosis,
            NormalizeDiagnosisCode(claim.DiagnosisCode),
            selectedClaim.VisitType!,
            claim.VisitType!);
        if (similarityTier is null)
        {
            return null;
        }

        var policy = graph.Policies[submission.PolicyId];
        var paymentFacts = terminalPayments
            .Select(payment => new ClaimReferencePaymentFact(
                payment.Status,
                payment.PaidDate,
                payment.PaidAmount,
                payment.PaidCoverageDisplayName))
            .ToArray();
        var result = new ClaimReferenceSimilarClaim(
            claim.Id,
            submission.Id,
            policy.Id,
            policy.DisplayTitle,
            policy.ContractStatus!,
            IsPolicyEligibleForCurrentMatching(policy),
            similarityTier,
            claim.ReferenceDate,
            claim.VisitType!,
            submission.UpdatedAt,
            paymentFacts);
        return new SimilarClaimCandidate(result);
    }

    private static string? GetSimilarityTier(
        string? anchorCoverageId,
        string? submissionCoverageId,
        string? selectedDiagnosis,
        string? otherDiagnosis,
        string selectedVisitType,
        string otherVisitType)
    {
        if (anchorCoverageId is not null
            && string.Equals(anchorCoverageId, submissionCoverageId, StringComparison.Ordinal))
        {
            return ClaimReferenceMatchingValues.SimilarityTierA;
        }

        if (!string.Equals(selectedVisitType, otherVisitType, StringComparison.Ordinal)
            || selectedDiagnosis is null
            || otherDiagnosis is null)
        {
            return null;
        }

        if (string.Equals(selectedDiagnosis, otherDiagnosis, StringComparison.Ordinal))
        {
            return ClaimReferenceMatchingValues.SimilarityTierB;
        }

        return selectedDiagnosis.StartsWith(otherDiagnosis, StringComparison.Ordinal)
            || otherDiagnosis.StartsWith(selectedDiagnosis, StringComparison.Ordinal)
                ? ClaimReferenceMatchingValues.SimilarityTierC
                : null;
    }

    private static string? NormalizeDiagnosisCode(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim().ToUpperInvariant();
    }

    private static string? NormalizeOptionalIdentifier(string? value)
    {
        if (value is null)
        {
            return null;
        }

        if (!IsNormalizedRequired(value))
        {
            throw InvalidGraph();
        }

        return value;
    }

    private static bool IsPolicyEligibleForCurrentMatching(PolicyRecord policy)
    {
        return policy.DisabledAt is null
            && (string.Equals(
                    policy.ContractStatus,
                    InsurancePolicyValues.ContractStatusActive,
                    StringComparison.Ordinal)
                || string.Equals(
                    policy.ContractStatus,
                    InsurancePolicyValues.ContractStatusPremiumWaived,
                    StringComparison.Ordinal)
                || string.Equals(
                    policy.ContractStatus,
                    InsurancePolicyValues.LegacyContractStatusActive,
                    StringComparison.Ordinal));
    }

    private static bool IsKnownPolicyStatus(string? status)
    {
        return status is not null
            && (InsurancePolicyValues.ContractStatuses.Contains(status, StringComparer.Ordinal)
                || string.Equals(
                    status,
                    InsurancePolicyValues.LegacyContractStatusActive,
                    StringComparison.Ordinal));
    }

    private static bool IsKnownClaimCaseStatus(string? status)
    {
        return string.Equals(status, ClaimCaseValues.StatusDraft, StringComparison.Ordinal)
            || string.Equals(status, ClaimCaseValues.StatusSaved, StringComparison.Ordinal);
    }

    private static bool IsEligibleTerminalPaymentStatus(string status)
    {
        return string.Equals(status, ClaimPaymentValues.StatusPaid, StringComparison.Ordinal)
            || string.Equals(status, ClaimPaymentValues.StatusPartiallyPaid, StringComparison.Ordinal)
            || string.Equals(status, ClaimPaymentValues.StatusDenied, StringComparison.Ordinal);
    }

    private static bool HasValidLifecycle(
        DateTimeOffset createdAt,
        DateTimeOffset updatedAt,
        DateTimeOffset? disabledAt)
    {
        return createdAt != default
            && updatedAt != default
            && updatedAt >= createdAt
            && (disabledAt is null
                || (disabledAt >= createdAt && disabledAt <= updatedAt));
    }

    private static bool IsValidDate(DateOnly? value)
    {
        return value is null || value != default;
    }

    private static bool IsNormalizedRequired(string? value)
    {
        return !string.IsNullOrWhiteSpace(value)
            && string.Equals(value, value.Trim(), StringComparison.Ordinal);
    }

    private static bool IsNormalizedOptional(string? value)
    {
        return value is null || IsNormalizedRequired(value);
    }

    private static int GetResultGroupRank(string resultGroup)
    {
        return resultGroup switch
        {
            ClaimReferenceMatchingValues.ResultGroupConditionMatch => 0,
            ClaimReferenceMatchingValues.ResultGroupNeedsConfirmation => 1,
            ClaimReferenceMatchingValues.ResultGroupCurrentInputMismatch => 2,
            _ => int.MaxValue
        };
    }

    private static int GetSimilarityTierRank(string tier)
    {
        return tier switch
        {
            ClaimReferenceMatchingValues.SimilarityTierA => 0,
            ClaimReferenceMatchingValues.SimilarityTierB => 1,
            ClaimReferenceMatchingValues.SimilarityTierC => 2,
            _ => int.MaxValue
        };
    }

    private static ClaimReferenceMatchingException InvalidGraph()
    {
        return new ClaimReferenceMatchingException(
            ClaimReferenceMatchingErrorCode.InvalidGraph,
            "Claim reference matching input is unavailable or inconsistent.");
    }

    private static ClaimReferenceMatchingException SelectedClaimUnavailable()
    {
        return new ClaimReferenceMatchingException(
            ClaimReferenceMatchingErrorCode.SelectedClaimUnavailable,
            "The selected claim cannot be used for reference matching.");
    }

    private sealed record ValidatedGraph(
        IReadOnlyDictionary<string, FamilyMemberRecord> FamilyMembers,
        IReadOnlyDictionary<string, PolicyRecord> Policies,
        IReadOnlyDictionary<string, PolicyCoverageRecord> PolicyCoverages,
        IReadOnlyDictionary<string, ClaimRecord> ClaimCases,
        IReadOnlyDictionary<string, ClaimSubmissionRecord> ClaimSubmissions,
        IReadOnlyDictionary<string, ClaimPaymentRecord> ClaimPayments,
        IReadOnlyDictionary<string, PolicyDocumentRecord> PolicyDocuments);

    private sealed record SimilarClaimCandidate(ClaimReferenceSimilarClaim Result);
}
