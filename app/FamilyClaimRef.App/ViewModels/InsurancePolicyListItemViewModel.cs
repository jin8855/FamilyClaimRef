using FamilyClaimRef.App.Models.Storage;
using System.Globalization;

namespace FamilyClaimRef.App.ViewModels;

public sealed record InsurancePolicyListItemViewModel(
    PolicyRecord Policy,
    string FamilyDisplayName,
    string UnregisteredValue,
    string LegacyValueReviewRequired)
{
    public string Id => Policy.Id;

    public string DisplayTitle => Policy.DisplayTitle;

    public string? InsurerName => Policy.InsurerName;

    public string ContractStatus => Policy.ContractStatus switch
    {
        InsurancePolicyValues.LegacyContractStatusActive =>
            InsurancePolicyValues.ContractStatusActive,
        { } value when InsurancePolicyValues.ContractStatuses.Contains(value, StringComparer.Ordinal) =>
            value,
        null or "" => UnregisteredValue,
        _ => LegacyValueReviewRequired
    };

    public string EnrollmentDate => Policy.EnrollmentDate?.ToString(
        "yyyy-MM-dd",
        CultureInfo.InvariantCulture) ?? UnregisteredValue;

    public string ProductCategory => Policy.ProductCategory is { } value
        && InsurancePolicyValues.ProductCategories.Contains(value, StringComparer.Ordinal)
            ? value
            : UnregisteredValue;

    public string TotalPlannedPremiumAmount => Policy.TotalPlannedPremiumAmount is { } amount
        ? $"{amount.ToString("N0", CultureInfo.InvariantCulture)}원"
        : UnregisteredValue;
}
