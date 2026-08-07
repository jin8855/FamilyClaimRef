using FamilyClaimRef.App.Models.Storage;

namespace FamilyClaimRef.App.Services.Storage;

public interface IClaimCaseStorageService
{
    Task<IReadOnlyList<ClaimRecord>> GetClaimCasesAsync(
        CancellationToken cancellationToken = default);

    Task<ClaimRecord?> GetClaimCaseAsync(
        string id,
        CancellationToken cancellationToken = default);

    Task<ClaimRecord> CreateClaimCaseAsync(
        ClaimCaseDraft draft,
        CancellationToken cancellationToken = default);

    Task<ClaimRecord> UpdateClaimCaseAsync(
        string id,
        int expectedRevision,
        ClaimCaseDraft draft,
        CancellationToken cancellationToken = default);

    Task<ClaimRecord> DisableClaimCaseAsync(
        string id,
        int expectedRevision,
        CancellationToken cancellationToken = default);
}
