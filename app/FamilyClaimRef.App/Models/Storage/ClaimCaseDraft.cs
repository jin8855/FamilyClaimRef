namespace FamilyClaimRef.App.Models.Storage;

public sealed record class ClaimCaseDraft(
    string DisplayTitle,
    string FamilyMemberId,
    DateOnly TreatmentDate,
    string HospitalName,
    string? DiagnosisCode,
    string? DiagnosisName,
    string VisitType,
    bool HasSurgery,
    bool HasPrescription,
    long? CoveredAmount,
    long? NonCoveredAmount,
    long? PrescriptionAmount,
    string? Memo);
