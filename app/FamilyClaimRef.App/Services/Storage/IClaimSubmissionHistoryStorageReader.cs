using FamilyClaimRef.App.Models.Storage;

namespace FamilyClaimRef.App.Services.Storage;

public interface IClaimSubmissionHistoryStorageReader
{
    Task<IReadOnlyList<ClaimSubmissionRecord>> GetAllSubmissionsForHistoryAsync(
        CancellationToken cancellationToken = default);
}
