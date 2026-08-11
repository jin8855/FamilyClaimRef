namespace FamilyClaimRef.App.Models.Storage;

public static class PolicyCoverageValues
{
    public const string ReviewStatusCandidate = "candidate";
    public const string ReviewStatusNeedsReview = "needs_review";
    public const string ReviewStatusUserConfirmed = "user_confirmed";
    public const string ReviewStatusIgnored = "ignored";

    public const string VisitTypeAny = "any";
    public const string VisitTypeOutpatient = "outpatient";
    public const string VisitTypeInpatient = "inpatient";

    public const string ConditionAny = "any";
    public const string ConditionRequired = "required";
    public const string ConditionExcluded = "excluded";

    public const string DiagnosisRuleAny = "any";
    public const string DiagnosisRulePrefixList = "prefix_list";

    public const string SourceManual = "manual";
    public const string SourcePolicyDocument = "policy_document";

    public static IReadOnlyList<string> ReviewStatuses { get; } =
    [
        ReviewStatusCandidate,
        ReviewStatusNeedsReview,
        ReviewStatusUserConfirmed,
        ReviewStatusIgnored
    ];

    public static IReadOnlyList<string> InitialReviewStatuses { get; } =
    [
        ReviewStatusCandidate,
        ReviewStatusNeedsReview
    ];

    public static IReadOnlyList<string> VisitTypeRules { get; } =
    [
        VisitTypeAny,
        VisitTypeOutpatient,
        VisitTypeInpatient
    ];

    public static IReadOnlyList<string> ConditionRules { get; } =
    [
        ConditionAny,
        ConditionRequired,
        ConditionExcluded
    ];

    public static IReadOnlyList<string> DiagnosisRuleModes { get; } =
    [
        DiagnosisRuleAny,
        DiagnosisRulePrefixList
    ];

    public static IReadOnlyList<string> SourceKinds { get; } =
    [
        SourceManual,
        SourcePolicyDocument
    ];

    public static IReadOnlyList<string> GetAllowedReviewStatusTargets(string currentStatus)
    {
        return currentStatus switch
        {
            ReviewStatusCandidate =>
            [
                ReviewStatusNeedsReview,
                ReviewStatusUserConfirmed,
                ReviewStatusIgnored
            ],
            ReviewStatusNeedsReview =>
            [
                ReviewStatusUserConfirmed,
                ReviewStatusIgnored
            ],
            ReviewStatusUserConfirmed =>
            [
                ReviewStatusNeedsReview,
                ReviewStatusIgnored
            ],
            ReviewStatusIgnored => [ReviewStatusNeedsReview],
            _ => []
        };
    }
}
