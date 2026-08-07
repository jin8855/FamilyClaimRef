namespace FamilyClaimRef.App.Models.Storage;

public static class ClaimCaseValues
{
    public const string VisitTypeOutpatient = "outpatient";
    public const string VisitTypeInpatient = "inpatient";
    public const string StatusDraft = "draft";
    public const string StatusSaved = "saved";

    public static IReadOnlyList<string> VisitTypes { get; } =
    [
        VisitTypeOutpatient,
        VisitTypeInpatient
    ];
}
