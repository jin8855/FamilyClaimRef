namespace FamilyClaimRef.App.Models.Storage;

public sealed record class PolicyCoverageCreateDraft(
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
    string? Memo);

public sealed record class PolicyCoverageUpdateDraft(
    string DisplayName,
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
    string? Memo);
