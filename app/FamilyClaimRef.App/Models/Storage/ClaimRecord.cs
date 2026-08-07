namespace FamilyClaimRef.App.Models.Storage;

public sealed record class ClaimRecord(
    string Id,
    string? PolicyId,
    string DisplayTitle,
    DateOnly ReferenceDate,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    DateTimeOffset? DisabledAt,
    string? FamilyMemberId = null,
    string? HospitalName = null,
    string? DiagnosisCode = null,
    string? DiagnosisName = null,
    string? VisitType = null,
    bool HasSurgery = false,
    bool HasPrescription = false,
    long? CoveredAmount = null,
    long? NonCoveredAmount = null,
    long? PrescriptionAmount = null,
    string? Memo = null,
    string? CaseStatus = null,
    int Revision = 0);
