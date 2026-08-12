namespace FamilyClaimRef.App.Models.Matching;

public static class ClaimReferenceMatchingValues
{
    public const string ResultGroupConditionMatch = "condition_match";
    public const string ResultGroupNeedsConfirmation = "needs_confirmation";
    public const string ResultGroupCurrentInputMismatch = "current_input_mismatch";

    public const string RulePolicyStatus = "policy_status";
    public const string RuleTreatmentDate = "treatment_date";
    public const string RuleVisitType = "visit_type";
    public const string RuleSurgery = "surgery";
    public const string RulePrescription = "prescription";
    public const string RuleDiagnosisCode = "diagnosis_code";
    public const string RuleSourceDocument = "source_document";

    public const string OutcomePassed = "passed";
    public const string OutcomeNeedsConfirmation = "needs_confirmation";
    public const string OutcomeMismatch = "mismatch";
    public const string OutcomeNotApplicable = "not_applicable";

    public const string SimilarityTierA = "A";
    public const string SimilarityTierB = "B";
    public const string SimilarityTierC = "C";

    public static IReadOnlyList<string> ScoredRuleNames { get; } =
    [
        RulePolicyStatus,
        RuleTreatmentDate,
        RuleVisitType,
        RuleSurgery,
        RulePrescription,
        RuleDiagnosisCode
    ];
}
