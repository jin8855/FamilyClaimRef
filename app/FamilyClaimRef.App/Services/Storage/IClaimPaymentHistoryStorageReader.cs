using FamilyClaimRef.App.Models.Storage;

namespace FamilyClaimRef.App.Services.Storage;

public interface IClaimPaymentHistoryStorageReader
{
    Task<IReadOnlyList<ClaimPaymentRecord>> GetAllPaymentsForHistoryAsync(
        CancellationToken cancellationToken = default);
}
