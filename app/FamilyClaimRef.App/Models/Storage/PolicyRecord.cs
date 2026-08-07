namespace FamilyClaimRef.App.Models.Storage;

public sealed record class PolicyRecord(
    string Id,
    string DisplayTitle,
    DateOnly? ReferenceDate,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    DateTimeOffset? DisabledAt,
    string? FamilyMemberId = null,
    string? InsurerName = null,
    string? ContractStatus = null,
    DateOnly? EnrollmentDate = null,
    string? CoveragePeriod = null,
    string? RegistrationSource = null,
    string? PremiumPaymentPeriod = null,
    decimal? TotalPlannedPremiumAmount = null,
    string? RenewalType = null,
    string? RefundType = null,
    string? InsuranceBusinessType = null,
    string? ProductCategory = null);
