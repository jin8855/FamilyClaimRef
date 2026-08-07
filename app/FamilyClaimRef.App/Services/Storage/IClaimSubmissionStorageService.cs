using FamilyClaimRef.App.Models.Storage;

namespace FamilyClaimRef.App.Services.Storage;

public interface IClaimSubmissionStorageService
{
    Task<IReadOnlyList<ClaimSubmissionRecord>> GetByClaimCaseAsync(
        string claimCaseId,
        CancellationToken cancellationToken = default);

    Task<ClaimSubmissionRecord?> GetAsync(
        string id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PolicyRecord>> GetClaimablePoliciesAsync(
        string claimCaseId,
        CancellationToken cancellationToken = default);

    Task<ClaimSubmissionRecord> CreateAsync(
        ClaimSubmissionDraft draft,
        CancellationToken cancellationToken = default);

    Task<ClaimSubmissionRecord> UpdateAsync(
        string id,
        int expectedRevision,
        ClaimSubmissionDraft draft,
        CancellationToken cancellationToken = default);
}
