namespace FamilyClaimRef.App.Models.Storage;

public sealed record class PolicyCoverageRecord(
    string PolicyCoverageId,
    string PolicyId,
    string DisplayName,
    string ReviewStatus,
    DateOnly? EffectiveFrom,
    DateOnly? EffectiveTo,
    string VisitTypeRule,
    string SurgeryRule,
    string PrescriptionRule,
    string DiagnosisRuleMode,
    string[] DiagnosisCodePrefixes,
    string SourceKind,
    string? SourcePolicyDocumentId,
    string? SourceLocator,
    string? Memo,
    int Revision,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    DateTimeOffset? DisabledAt);
