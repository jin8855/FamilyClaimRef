namespace FamilyClaimRef.App.Models.Storage;

public sealed record class InsurancePolicyDraft(
    string DisplayTitle,
    string FamilyMemberId,
    string InsurerName,
    string ContractStatus,
    DateOnly EnrollmentDate,
    string CoveragePeriod,
    string PremiumPaymentPeriod,
    decimal? TotalPlannedPremiumAmount,
    string RenewalType,
    string RefundType,
    string InsuranceBusinessType,
    string ProductCategory);
