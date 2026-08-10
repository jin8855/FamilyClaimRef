using FamilyClaimRef.App.Models.Storage;

namespace FamilyClaimRef.App.Services.Storage;

public interface IClaimHistoryStorageReader
{
    Task<IReadOnlyList<PolicyRecord>> GetAllPoliciesForHistoryAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ClaimRecord>> GetAllClaimCasesForHistoryAsync(
        CancellationToken cancellationToken = default);
}
