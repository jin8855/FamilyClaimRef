using FamilyClaimRef.App.Models.Storage;

namespace FamilyClaimRef.App.Services.Storage;

public interface IClaimPaymentStorageService
{
    Task<IReadOnlyList<ClaimPaymentRecord>> GetBySubmissionAsync(
        string claimSubmissionId,
        CancellationToken cancellationToken = default);

    Task<ClaimPaymentRecord?> GetAsync(
        string id,
        CancellationToken cancellationToken = default);

    Task<ClaimPaymentRecord> CreateAsync(
        ClaimPaymentDraft draft,
        CancellationToken cancellationToken = default);

    Task<ClaimPaymentRecord> UpdateAsync(
        string id,
        int expectedRevision,
        ClaimPaymentDraft draft,
        CancellationToken cancellationToken = default);
}
