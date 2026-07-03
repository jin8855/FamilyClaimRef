using FamilyClaimRef.App.Models.Storage;

namespace FamilyClaimRef.App.Services.Storage;

public interface IPolicyClaimStorageService
{
    Task<IReadOnlyList<PolicyRecord>> GetPoliciesAsync(
        CancellationToken cancellationToken = default);

    Task<PolicyRecord?> GetPolicyAsync(
        string id,
        CancellationToken cancellationToken = default);

    Task<PolicyRecord> AddPolicyAsync(
        PolicyDraft draft,
        CancellationToken cancellationToken = default);

    Task<PolicyRecord> DisablePolicyAsync(
        string id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ClaimRecord>> GetClaimsAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ClaimRecord>> GetClaimsByPolicyIdAsync(
        string policyId,
        CancellationToken cancellationToken = default);

    Task<ClaimRecord?> GetClaimAsync(
        string id,
        CancellationToken cancellationToken = default);

    Task<ClaimRecord> AddClaimAsync(
        ClaimDraft draft,
        CancellationToken cancellationToken = default);

    Task<ClaimRecord> DisableClaimAsync(
        string id,
        CancellationToken cancellationToken = default);

    Task<bool> PolicyExistsAsync(
        string id,
        CancellationToken cancellationToken = default);

    Task<bool> ClaimExistsAsync(
        string id,
        CancellationToken cancellationToken = default);
}
