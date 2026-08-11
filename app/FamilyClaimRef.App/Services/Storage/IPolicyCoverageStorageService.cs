using FamilyClaimRef.App.Models.Storage;

namespace FamilyClaimRef.App.Services.Storage;

public interface IPolicyCoverageStorageService
{
    Task<IReadOnlyList<PolicyCoverageRecord>> GetPolicyCoveragesAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PolicyCoverageRecord>> GetActivePolicyCoveragesAsync(
        CancellationToken cancellationToken = default);

    Task<PolicyCoverageRecord?> GetPolicyCoverageAsync(
        string id,
        CancellationToken cancellationToken = default);

    Task<PolicyCoverageRecord> CreatePolicyCoverageAsync(
        PolicyCoverageCreateDraft draft,
        CancellationToken cancellationToken = default);

    Task<PolicyCoverageRecord> UpdatePolicyCoverageAsync(
        string id,
        int expectedRevision,
        PolicyCoverageUpdateDraft draft,
        CancellationToken cancellationToken = default);

    Task<PolicyCoverageRecord> ChangePolicyCoverageReviewStatusAsync(
        string id,
        int expectedRevision,
        string targetReviewStatus,
        CancellationToken cancellationToken = default);

    Task<PolicyCoverageRecord> DisablePolicyCoverageAsync(
        string id,
        int expectedRevision,
        CancellationToken cancellationToken = default);

    Task<PolicyCoverageRecord> RestorePolicyCoverageAsync(
        string id,
        int expectedRevision,
        CancellationToken cancellationToken = default);
}
