using FamilyClaimRef.App.Models.Storage;

namespace FamilyClaimRef.App.Services.Storage;

public sealed class JsonPolicyClaimStorageService : IPolicyClaimStorageService
{
    private const string PoliciesFileName = "policies.json";
    private const string ClaimsFileName = "claims.json";

    private readonly JsonFileStore<PolicyRecord> policyStore;
    private readonly JsonFileStore<ClaimRecord> claimStore;

    public JsonPolicyClaimStorageService(string metadataRootPath)
    {
        if (string.IsNullOrWhiteSpace(metadataRootPath))
        {
            throw new ArgumentException("Metadata root path is required.", nameof(metadataRootPath));
        }

        policyStore = new JsonFileStore<PolicyRecord>(metadataRootPath, PoliciesFileName);
        claimStore = new JsonFileStore<ClaimRecord>(metadataRootPath, ClaimsFileName);
    }

    public async Task<IReadOnlyList<PolicyRecord>> GetPoliciesAsync(
        CancellationToken cancellationToken = default)
    {
        var envelope = await policyStore.LoadAsync(cancellationToken);

        return envelope.Items
            .Where(policy => policy.DisabledAt is null)
            .ToList();
    }

    public async Task<PolicyRecord?> GetPolicyAsync(
        string id,
        CancellationToken cancellationToken = default)
    {
        var normalizedId = NormalizeRequiredValue(id, nameof(id));
        var policies = await GetPoliciesAsync(cancellationToken);

        return policies.FirstOrDefault(policy => policy.Id == normalizedId);
    }

    public async Task<PolicyRecord> AddPolicyAsync(
        PolicyDraft draft,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(draft);

        var policies = (await policyStore.LoadAsync(cancellationToken)).Items.ToList();
        var timestamp = DateTimeOffset.UtcNow;
        var record = new PolicyRecord(
            CreateId("policy"),
            NormalizeRequiredValue(draft.DisplayTitle, nameof(draft.DisplayTitle)),
            NormalizeReferenceDate(draft.ReferenceDate, nameof(draft.ReferenceDate)),
            timestamp,
            timestamp,
            null);

        EnsureUniqueId(policies.Select(policy => policy.Id), record.Id);

        policies.Add(record);
        await policyStore.SaveAsync(policies, cancellationToken);

        return record;
    }

    public async Task<PolicyRecord> DisablePolicyAsync(
        string id,
        CancellationToken cancellationToken = default)
    {
        var normalizedId = NormalizeRequiredValue(id, nameof(id));
        var policies = (await policyStore.LoadAsync(cancellationToken)).Items.ToList();
        var policyIndex = policies.FindIndex(policy => policy.Id == normalizedId);
        if (policyIndex < 0)
        {
            throw new InvalidOperationException("Policy was not found.");
        }

        if (policies[policyIndex].DisabledAt is not null)
        {
            throw new InvalidOperationException("Policy is already disabled.");
        }

        var timestamp = DateTimeOffset.UtcNow;
        var disabledPolicy = policies[policyIndex] with
        {
            UpdatedAt = timestamp,
            DisabledAt = timestamp
        };
        policies[policyIndex] = disabledPolicy;

        await policyStore.SaveAsync(policies, cancellationToken);

        return disabledPolicy;
    }

    public async Task<IReadOnlyList<ClaimRecord>> GetClaimsAsync(
        CancellationToken cancellationToken = default)
    {
        var envelope = await claimStore.LoadAsync(cancellationToken);

        return envelope.Items
            .Where(claim => claim.DisabledAt is null)
            .ToList();
    }

    public async Task<IReadOnlyList<ClaimRecord>> GetClaimsByPolicyIdAsync(
        string policyId,
        CancellationToken cancellationToken = default)
    {
        var normalizedPolicyId = NormalizeRequiredValue(policyId, nameof(policyId));
        var claims = await GetClaimsAsync(cancellationToken);

        return claims
            .Where(claim => claim.PolicyId == normalizedPolicyId)
            .ToList();
    }

    public async Task<ClaimRecord?> GetClaimAsync(
        string id,
        CancellationToken cancellationToken = default)
    {
        var normalizedId = NormalizeRequiredValue(id, nameof(id));
        var claims = await GetClaimsAsync(cancellationToken);

        return claims.FirstOrDefault(claim => claim.Id == normalizedId);
    }

    public async Task<ClaimRecord> AddClaimAsync(
        ClaimDraft draft,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(draft);

        var normalizedPolicyId = NormalizeRequiredValue(draft.PolicyId, nameof(draft.PolicyId));
        await EnsureActivePolicyExistsAsync(normalizedPolicyId, cancellationToken);

        var claims = (await claimStore.LoadAsync(cancellationToken)).Items.ToList();
        var timestamp = DateTimeOffset.UtcNow;
        var record = new ClaimRecord(
            CreateId("claim"),
            normalizedPolicyId,
            NormalizeRequiredValue(draft.DisplayTitle, nameof(draft.DisplayTitle)),
            NormalizeReferenceDate(draft.ReferenceDate, nameof(draft.ReferenceDate)),
            timestamp,
            timestamp,
            null);

        EnsureUniqueId(claims.Select(claim => claim.Id), record.Id);

        claims.Add(record);
        await claimStore.SaveAsync(claims, cancellationToken);

        return record;
    }

    public async Task<ClaimRecord> DisableClaimAsync(
        string id,
        CancellationToken cancellationToken = default)
    {
        var normalizedId = NormalizeRequiredValue(id, nameof(id));
        var claims = (await claimStore.LoadAsync(cancellationToken)).Items.ToList();
        var claimIndex = claims.FindIndex(claim => claim.Id == normalizedId);
        if (claimIndex < 0)
        {
            throw new InvalidOperationException("Claim was not found.");
        }

        if (claims[claimIndex].DisabledAt is not null)
        {
            throw new InvalidOperationException("Claim is already disabled.");
        }

        var timestamp = DateTimeOffset.UtcNow;
        var disabledClaim = claims[claimIndex] with
        {
            UpdatedAt = timestamp,
            DisabledAt = timestamp
        };
        claims[claimIndex] = disabledClaim;

        await claimStore.SaveAsync(claims, cancellationToken);

        return disabledClaim;
    }

    public async Task<bool> PolicyExistsAsync(
        string id,
        CancellationToken cancellationToken = default)
    {
        return await GetPolicyAsync(id, cancellationToken) is not null;
    }

    public async Task<bool> ClaimExistsAsync(
        string id,
        CancellationToken cancellationToken = default)
    {
        return await GetClaimAsync(id, cancellationToken) is not null;
    }

    private static string CreateId(string prefix)
    {
        return $"{prefix}_{Guid.NewGuid():N}";
    }

    private static string NormalizeRequiredValue(string value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Value is required.", parameterName);
        }

        return value.Trim();
    }

    private static DateOnly NormalizeReferenceDate(DateOnly value, string parameterName)
    {
        if (value == default)
        {
            throw new ArgumentException("Reference date is required.", parameterName);
        }

        return value;
    }

    private static void EnsureUniqueId(IEnumerable<string> ids, string id)
    {
        if (ids.Contains(id, StringComparer.Ordinal))
        {
            throw new InvalidOperationException("Generated id already exists.");
        }
    }

    private async Task EnsureActivePolicyExistsAsync(
        string policyId,
        CancellationToken cancellationToken)
    {
        if (!await PolicyExistsAsync(policyId, cancellationToken))
        {
            throw new InvalidOperationException("Referenced policy was not found or is disabled.");
        }
    }
}
