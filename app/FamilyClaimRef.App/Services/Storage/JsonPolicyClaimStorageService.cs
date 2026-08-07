using System.IO;
using FamilyClaimRef.App.Models.Storage;

namespace FamilyClaimRef.App.Services.Storage;

public sealed class JsonPolicyClaimStorageService :
    IPolicyClaimStorageService,
    IClaimCaseStorageService
{
    private const string PoliciesFileName = "policies.json";
    private const string ClaimsFileName = "claims.json";

    private static readonly System.Collections.Concurrent.ConcurrentDictionary<string, SemaphoreSlim>
        ClaimMutationGates = new(StringComparer.OrdinalIgnoreCase);

    private readonly JsonFileStore<PolicyRecord> policyStore;
    private readonly JsonFileStore<ClaimRecord> claimStore;
    private readonly IFamilyMemberStorageService familyMemberStorageService;
    private readonly SemaphoreSlim claimMutationGate;

    public JsonPolicyClaimStorageService(string metadataRootPath)
        : this(metadataRootPath, new JsonFamilyMemberStorageService(metadataRootPath))
    {
    }

    public JsonPolicyClaimStorageService(
        string metadataRootPath,
        IFamilyMemberStorageService familyMemberStorageService)
    {
        if (string.IsNullOrWhiteSpace(metadataRootPath))
        {
            throw new ArgumentException("Metadata root path is required.", nameof(metadataRootPath));
        }

        this.familyMemberStorageService = familyMemberStorageService
            ?? throw new ArgumentNullException(nameof(familyMemberStorageService));
        var claimFilePath = Path.GetFullPath(Path.Combine(metadataRootPath, ClaimsFileName));
        policyStore = new JsonFileStore<PolicyRecord>(metadataRootPath, PoliciesFileName);
        claimStore = new JsonFileStore<ClaimRecord>(
            metadataRootPath,
            ClaimsFileName,
            preserveBackupOnReplace: true);
        claimMutationGate = ClaimMutationGates.GetOrAdd(claimFilePath, _ => new SemaphoreSlim(1, 1));
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

    public async Task<PolicyRecord> CreateInsurancePolicyAsync(
        InsurancePolicyDraft draft,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(draft);

        var normalizedDraft = NormalizeInsurancePolicyDraft(draft);
        await EnsureFamilyReferenceAsync(
            normalizedDraft.FamilyMemberId,
            requireActive: true,
            cancellationToken);

        var policies = (await policyStore.LoadAsync(cancellationToken)).Items.ToList();
        var timestamp = DateTimeOffset.UtcNow;
        var record = new PolicyRecord(
            Id: CreateId("policy"),
            DisplayTitle: normalizedDraft.DisplayTitle,
            ReferenceDate: null,
            CreatedAt: timestamp,
            UpdatedAt: timestamp,
            DisabledAt: null,
            FamilyMemberId: normalizedDraft.FamilyMemberId,
            InsurerName: normalizedDraft.InsurerName,
            ContractStatus: normalizedDraft.ContractStatus,
            EnrollmentDate: normalizedDraft.EnrollmentDate,
            CoveragePeriod: normalizedDraft.CoveragePeriod,
            RegistrationSource: InsurancePolicyValues.RegistrationSourceDirectInput,
            PremiumPaymentPeriod: normalizedDraft.PremiumPaymentPeriod,
            TotalPlannedPremiumAmount: normalizedDraft.TotalPlannedPremiumAmount,
            RenewalType: normalizedDraft.RenewalType,
            RefundType: normalizedDraft.RefundType,
            InsuranceBusinessType: normalizedDraft.InsuranceBusinessType,
            ProductCategory: normalizedDraft.ProductCategory);

        EnsureUniqueId(policies.Select(policy => policy.Id), record.Id);
        policies.Add(record);
        await policyStore.SaveAsync(policies, cancellationToken);

        return record;
    }

    public async Task<PolicyRecord> UpdateInsurancePolicyAsync(
        string id,
        InsurancePolicyDraft draft,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(draft);

        var normalizedId = NormalizeRequiredValue(id, nameof(id));
        var normalizedDraft = NormalizeInsurancePolicyDraft(draft);
        var policies = (await policyStore.LoadAsync(cancellationToken)).Items.ToList();
        var policyIndex = policies.FindIndex(policy => policy.Id == normalizedId);
        if (policyIndex < 0 || policies[policyIndex].DisabledAt is not null)
        {
            throw new InvalidOperationException("Policy was not found or is disabled.");
        }

        var current = policies[policyIndex];
        var changesFamilyReference = !string.Equals(
            current.FamilyMemberId,
            normalizedDraft.FamilyMemberId,
            StringComparison.Ordinal);
        await EnsureFamilyReferenceAsync(
            normalizedDraft.FamilyMemberId,
            requireActive: changesFamilyReference,
            cancellationToken);

        var updated = current with
        {
            DisplayTitle = normalizedDraft.DisplayTitle,
            UpdatedAt = DateTimeOffset.UtcNow,
            FamilyMemberId = normalizedDraft.FamilyMemberId,
            InsurerName = normalizedDraft.InsurerName,
            ContractStatus = normalizedDraft.ContractStatus,
            EnrollmentDate = normalizedDraft.EnrollmentDate,
            CoveragePeriod = normalizedDraft.CoveragePeriod,
            PremiumPaymentPeriod = normalizedDraft.PremiumPaymentPeriod,
            TotalPlannedPremiumAmount = normalizedDraft.TotalPlannedPremiumAmount,
            RenewalType = normalizedDraft.RenewalType,
            RefundType = normalizedDraft.RefundType,
            InsuranceBusinessType = normalizedDraft.InsuranceBusinessType,
            ProductCategory = normalizedDraft.ProductCategory
        };
        policies[policyIndex] = updated;
        await policyStore.SaveAsync(policies, cancellationToken);

        return updated;
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

    public Task<IReadOnlyList<ClaimRecord>> GetClaimsAsync(
        CancellationToken cancellationToken = default)
    {
        return GetClaimCasesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ClaimRecord>> GetClaimCasesAsync(
        CancellationToken cancellationToken = default)
    {
        var claims = (await claimStore.LoadAsync(cancellationToken)).Items;
        var policies = (await policyStore.LoadAsync(cancellationToken)).Items;

        return claims
            .Where(claim => claim.DisabledAt is null)
            .Select(claim => ProjectClaimCase(claim, policies))
            .ToList();
    }

    public async Task<IReadOnlyList<ClaimRecord>> GetClaimsByPolicyIdAsync(
        string policyId,
        CancellationToken cancellationToken = default)
    {
        var normalizedPolicyId = NormalizeRequiredValue(policyId, nameof(policyId));
        var claims = await GetClaimCasesAsync(cancellationToken);

        return claims
            .Where(claim => string.Equals(
                claim.PolicyId,
                normalizedPolicyId,
                StringComparison.Ordinal))
            .ToList();
    }

    public Task<ClaimRecord?> GetClaimAsync(
        string id,
        CancellationToken cancellationToken = default)
    {
        return GetClaimCaseAsync(id, cancellationToken);
    }

    public async Task<ClaimRecord?> GetClaimCaseAsync(
        string id,
        CancellationToken cancellationToken = default)
    {
        var normalizedId = NormalizeRequiredValue(id, nameof(id));
        var claims = await GetClaimCasesAsync(cancellationToken);

        return claims.FirstOrDefault(claim => claim.Id == normalizedId);
    }

    public async Task<ClaimRecord> AddClaimAsync(
        ClaimDraft draft,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(draft);

        var normalizedPolicyId = NormalizeRequiredValue(draft.PolicyId, nameof(draft.PolicyId));
        var policy = await GetPolicyAsync(normalizedPolicyId, cancellationToken)
            ?? throw new InvalidOperationException("Referenced policy was not found or is disabled.");

        await claimMutationGate.WaitAsync(cancellationToken);
        try
        {
            var claims = (await claimStore.LoadAsync(cancellationToken)).Items.ToList();
            var timestamp = DateTimeOffset.UtcNow;
            var record = new ClaimRecord(
                Id: CreateId("claim"),
                PolicyId: normalizedPolicyId,
                DisplayTitle: NormalizeRequiredValue(
                    draft.DisplayTitle,
                    nameof(draft.DisplayTitle)),
                ReferenceDate: NormalizeReferenceDate(
                    draft.ReferenceDate,
                    nameof(draft.ReferenceDate)),
                CreatedAt: timestamp,
                UpdatedAt: timestamp,
                DisabledAt: null,
                FamilyMemberId: policy.FamilyMemberId,
                CaseStatus: ClaimCaseValues.StatusDraft,
                Revision: 1);

            EnsureUniqueId(claims.Select(claim => claim.Id), record.Id);
            claims.Add(record);
            await claimStore.SaveAsync(claims, cancellationToken);

            return record;
        }
        finally
        {
            claimMutationGate.Release();
        }
    }

    public async Task<ClaimRecord> CreateClaimCaseAsync(
        ClaimCaseDraft draft,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(draft);

        var normalizedDraft = NormalizeClaimCaseDraft(draft);
        await EnsureFamilyReferenceAsync(
            normalizedDraft.FamilyMemberId,
            requireActive: true,
            cancellationToken);

        await claimMutationGate.WaitAsync(cancellationToken);
        try
        {
            var claims = (await claimStore.LoadAsync(cancellationToken)).Items.ToList();
            var timestamp = DateTimeOffset.UtcNow;
            var record = new ClaimRecord(
                Id: CreateId("claim"),
                PolicyId: null,
                DisplayTitle: normalizedDraft.DisplayTitle,
                ReferenceDate: normalizedDraft.TreatmentDate,
                CreatedAt: timestamp,
                UpdatedAt: timestamp,
                DisabledAt: null,
                FamilyMemberId: normalizedDraft.FamilyMemberId,
                HospitalName: normalizedDraft.HospitalName,
                DiagnosisCode: normalizedDraft.DiagnosisCode,
                DiagnosisName: normalizedDraft.DiagnosisName,
                VisitType: normalizedDraft.VisitType,
                HasSurgery: normalizedDraft.HasSurgery,
                HasPrescription: normalizedDraft.HasPrescription,
                CoveredAmount: normalizedDraft.CoveredAmount,
                NonCoveredAmount: normalizedDraft.NonCoveredAmount,
                PrescriptionAmount: normalizedDraft.PrescriptionAmount,
                Memo: normalizedDraft.Memo,
                CaseStatus: ClaimCaseValues.StatusDraft,
                Revision: 1);

            EnsureUniqueId(claims.Select(claim => claim.Id), record.Id);
            claims.Add(record);
            await claimStore.SaveAsync(claims, cancellationToken);

            return record;
        }
        finally
        {
            claimMutationGate.Release();
        }
    }

    public async Task<ClaimRecord> UpdateClaimCaseAsync(
        string id,
        int expectedRevision,
        ClaimCaseDraft draft,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(draft);

        var normalizedId = NormalizeRequiredValue(id, nameof(id));
        var normalizedDraft = NormalizeClaimCaseDraft(draft);

        await claimMutationGate.WaitAsync(cancellationToken);
        try
        {
            var claims = (await claimStore.LoadAsync(cancellationToken)).Items.ToList();
            var claimIndex = claims.FindIndex(claim => claim.Id == normalizedId);
            if (claimIndex < 0 || claims[claimIndex].DisabledAt is not null)
            {
                throw new InvalidOperationException("Claim case was not found or is disabled.");
            }

            var current = claims[claimIndex];
            if (current.Revision != expectedRevision)
            {
                throw new ClaimCaseConcurrencyException();
            }

            var policies = (await policyStore.LoadAsync(cancellationToken)).Items;
            EnsureLegacyOwnershipResolved(current, policies);
            await EnsureFamilyReferenceAsync(
                normalizedDraft.FamilyMemberId,
                requireActive: true,
                cancellationToken);

            var updated = current with
            {
                DisplayTitle = normalizedDraft.DisplayTitle,
                ReferenceDate = normalizedDraft.TreatmentDate,
                UpdatedAt = DateTimeOffset.UtcNow,
                FamilyMemberId = normalizedDraft.FamilyMemberId,
                HospitalName = normalizedDraft.HospitalName,
                DiagnosisCode = normalizedDraft.DiagnosisCode,
                DiagnosisName = normalizedDraft.DiagnosisName,
                VisitType = normalizedDraft.VisitType,
                HasSurgery = normalizedDraft.HasSurgery,
                HasPrescription = normalizedDraft.HasPrescription,
                CoveredAmount = normalizedDraft.CoveredAmount,
                NonCoveredAmount = normalizedDraft.NonCoveredAmount,
                PrescriptionAmount = normalizedDraft.PrescriptionAmount,
                Memo = normalizedDraft.Memo,
                CaseStatus = ClaimCaseValues.StatusSaved,
                Revision = checked(current.Revision + 1)
            };
            claims[claimIndex] = updated;
            await claimStore.SaveAsync(claims, cancellationToken);

            return updated;
        }
        finally
        {
            claimMutationGate.Release();
        }
    }

    public Task<ClaimRecord> DisableClaimAsync(
        string id,
        int expectedRevision,
        CancellationToken cancellationToken = default)
    {
        return DisableClaimCoreAsync(id, expectedRevision, cancellationToken);
    }

    public Task<ClaimRecord> DisableClaimCaseAsync(
        string id,
        int expectedRevision,
        CancellationToken cancellationToken = default)
    {
        return DisableClaimCoreAsync(id, expectedRevision, cancellationToken);
    }

    private async Task<ClaimRecord> DisableClaimCoreAsync(
        string id,
        int expectedRevision,
        CancellationToken cancellationToken)
    {
        var normalizedId = NormalizeRequiredValue(id, nameof(id));

        await claimMutationGate.WaitAsync(cancellationToken);
        try
        {
            var claims = (await claimStore.LoadAsync(cancellationToken)).Items.ToList();
            var claimIndex = claims.FindIndex(claim => claim.Id == normalizedId);
            if (claimIndex < 0)
            {
                throw new InvalidOperationException("Claim was not found.");
            }

            var current = claims[claimIndex];
            if (current.DisabledAt is not null)
            {
                throw new InvalidOperationException("Claim is already disabled.");
            }

            if (current.Revision != expectedRevision)
            {
                throw new ClaimCaseConcurrencyException();
            }

            var policies = (await policyStore.LoadAsync(cancellationToken)).Items;
            if (current.Revision == 0 || string.IsNullOrWhiteSpace(current.CaseStatus))
            {
                // Schema v1 predates CaseStatus and Revision; the compatibility API writes both.
                EnsureLegacyOwnershipResolved(current, policies);
            }

            var timestamp = DateTimeOffset.UtcNow;
            var disabledClaim = current with
            {
                UpdatedAt = timestamp,
                DisabledAt = timestamp,
                Revision = checked(current.Revision + 1)
            };
            claims[claimIndex] = disabledClaim;
            await claimStore.SaveAsync(claims, cancellationToken);

            return disabledClaim;
        }
        finally
        {
            claimMutationGate.Release();
        }
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

    private static ClaimCaseDraft NormalizeClaimCaseDraft(ClaimCaseDraft draft)
    {
        return draft with
        {
            DisplayTitle = NormalizeRequiredValue(
                draft.DisplayTitle,
                nameof(draft.DisplayTitle)),
            FamilyMemberId = NormalizeRequiredValue(
                draft.FamilyMemberId,
                nameof(draft.FamilyMemberId)),
            TreatmentDate = NormalizeReferenceDate(
                draft.TreatmentDate,
                nameof(draft.TreatmentDate)),
            HospitalName = NormalizeRequiredValue(
                draft.HospitalName,
                nameof(draft.HospitalName)),
            DiagnosisCode = NormalizeOptionalValue(draft.DiagnosisCode)?.ToUpperInvariant(),
            DiagnosisName = NormalizeOptionalValue(draft.DiagnosisName),
            VisitType = NormalizeAllowedValue(
                draft.VisitType,
                ClaimCaseValues.VisitTypes,
                nameof(draft.VisitType)),
            CoveredAmount = NormalizeClaimAmount(
                draft.CoveredAmount,
                nameof(draft.CoveredAmount)),
            NonCoveredAmount = NormalizeClaimAmount(
                draft.NonCoveredAmount,
                nameof(draft.NonCoveredAmount)),
            PrescriptionAmount = NormalizeClaimAmount(
                draft.PrescriptionAmount,
                nameof(draft.PrescriptionAmount)),
            Memo = NormalizeOptionalValue(draft.Memo)
        };
    }

    private static string? NormalizeOptionalValue(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    private static long? NormalizeClaimAmount(long? value, string parameterName)
    {
        if (value < 0)
        {
            throw new ArgumentOutOfRangeException(
                parameterName,
                "Claim amount must be a non-negative whole number.");
        }

        return value;
    }

    private static ClaimRecord ProjectClaimCase(
        ClaimRecord claim,
        IReadOnlyList<PolicyRecord> policies)
    {
        var familyMemberId = claim.FamilyMemberId;
        if (string.IsNullOrWhiteSpace(familyMemberId)
            && !string.IsNullOrWhiteSpace(claim.PolicyId))
        {
            familyMemberId = policies.FirstOrDefault(policy => string.Equals(
                policy.Id,
                claim.PolicyId,
                StringComparison.Ordinal))?.FamilyMemberId;
        }

        return claim with
        {
            FamilyMemberId = familyMemberId,
            CaseStatus = string.IsNullOrWhiteSpace(claim.CaseStatus)
                ? ClaimCaseValues.StatusSaved
                : claim.CaseStatus
        };
    }

    private static void EnsureLegacyOwnershipResolved(
        ClaimRecord claim,
        IReadOnlyList<PolicyRecord> policies)
    {
        if (!string.IsNullOrWhiteSpace(claim.FamilyMemberId))
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(claim.PolicyId)
            || string.IsNullOrWhiteSpace(policies.FirstOrDefault(policy => string.Equals(
                policy.Id,
                claim.PolicyId,
                StringComparison.Ordinal))?.FamilyMemberId))
        {
            throw new ClaimCaseLegacyReviewRequiredException();
        }
    }

    private static InsurancePolicyDraft NormalizeInsurancePolicyDraft(
        InsurancePolicyDraft draft)
    {
        return new InsurancePolicyDraft(
            NormalizeRequiredValue(draft.DisplayTitle, nameof(draft.DisplayTitle)),
            NormalizeRequiredValue(draft.FamilyMemberId, nameof(draft.FamilyMemberId)),
            NormalizeRequiredValue(draft.InsurerName, nameof(draft.InsurerName)),
            NormalizeAllowedValue(
                draft.ContractStatus,
                InsurancePolicyValues.ContractStatuses,
                nameof(draft.ContractStatus)),
            NormalizeReferenceDate(draft.EnrollmentDate, nameof(draft.EnrollmentDate)),
            NormalizeRequiredValue(draft.CoveragePeriod, nameof(draft.CoveragePeriod)),
            NormalizeRequiredValue(draft.PremiumPaymentPeriod, nameof(draft.PremiumPaymentPeriod)),
            NormalizePlannedPremiumAmount(draft.TotalPlannedPremiumAmount),
            NormalizeAllowedValue(
                draft.RenewalType,
                InsurancePolicyValues.RenewalTypes,
                nameof(draft.RenewalType)),
            NormalizeAllowedValue(
                draft.RefundType,
                InsurancePolicyValues.RefundTypes,
                nameof(draft.RefundType)),
            NormalizeAllowedValue(
                draft.InsuranceBusinessType,
                InsurancePolicyValues.BusinessTypes,
                nameof(draft.InsuranceBusinessType)),
            NormalizeAllowedValue(
                draft.ProductCategory,
                InsurancePolicyValues.ProductCategories,
                nameof(draft.ProductCategory)));
    }

    private static string NormalizeAllowedValue(
        string value,
        IReadOnlyList<string> allowedValues,
        string parameterName)
    {
        var normalized = NormalizeRequiredValue(value, parameterName);
        if (!allowedValues.Contains(normalized, StringComparer.Ordinal))
        {
            throw new ArgumentException("Value is not allowed.", parameterName);
        }

        return normalized;
    }

    private static decimal? NormalizePlannedPremiumAmount(decimal? value)
    {
        if (value is null)
        {
            return null;
        }

        if (value < 0 || value != decimal.Truncate(value.Value))
        {
            throw new ArgumentOutOfRangeException(
                nameof(InsurancePolicyDraft.TotalPlannedPremiumAmount),
                "Planned premium amount must be a non-negative whole number.");
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
    private async Task EnsureFamilyReferenceAsync(
        string familyMemberId,
        bool requireActive,
        CancellationToken cancellationToken)
    {
        var familyMember = await familyMemberStorageService.GetFamilyMemberAsync(
            familyMemberId,
            cancellationToken);
        if (familyMember is null || (requireActive && familyMember.DisabledAt is not null))
        {
            throw new InvalidOperationException(
                "Referenced family member was not found or is unavailable.");
        }
    }
}
