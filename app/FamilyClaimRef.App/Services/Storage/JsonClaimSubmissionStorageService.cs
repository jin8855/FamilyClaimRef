using System.Collections.Concurrent;
using System.IO;
using FamilyClaimRef.App.Models.Storage;

namespace FamilyClaimRef.App.Services.Storage;

public sealed class JsonClaimSubmissionStorageService : IClaimSubmissionStorageService
{
    private const string FileName = "claim-submissions.json";

    private static readonly ConcurrentDictionary<string, SemaphoreSlim> MutationGates =
        new(StringComparer.OrdinalIgnoreCase);

    private readonly JsonFileStore<ClaimSubmissionRecord> store;
    private readonly IClaimCaseStorageService claimCaseStorageService;
    private readonly IPolicyClaimStorageService policyStorageService;
    private readonly IDocumentStorageService documentStorageService;
    private readonly SemaphoreSlim mutationGate;

    public JsonClaimSubmissionStorageService(
        string metadataRootPath,
        IClaimCaseStorageService claimCaseStorageService,
        IPolicyClaimStorageService policyStorageService,
        IDocumentStorageService documentStorageService)
    {
        if (string.IsNullOrWhiteSpace(metadataRootPath))
        {
            throw new ArgumentException("Metadata root path is required.", nameof(metadataRootPath));
        }

        this.claimCaseStorageService = claimCaseStorageService
            ?? throw new ArgumentNullException(nameof(claimCaseStorageService));
        this.policyStorageService = policyStorageService
            ?? throw new ArgumentNullException(nameof(policyStorageService));
        this.documentStorageService = documentStorageService
            ?? throw new ArgumentNullException(nameof(documentStorageService));

        var canonicalPath = Path.GetFullPath(Path.Combine(metadataRootPath, FileName));
        store = new JsonFileStore<ClaimSubmissionRecord>(
            metadataRootPath,
            FileName,
            preserveBackupOnReplace: true);
        mutationGate = MutationGates.GetOrAdd(canonicalPath, _ => new SemaphoreSlim(1, 1));
    }

    public async Task<IReadOnlyList<ClaimSubmissionRecord>> GetByClaimCaseAsync(
        string claimCaseId,
        CancellationToken cancellationToken = default)
    {
        var normalizedClaimCaseId = NormalizeRequired(claimCaseId, nameof(claimCaseId));
        var envelope = await store.LoadAsync(cancellationToken);

        return envelope.Items
            .Where(item => string.Equals(
                item.ClaimCaseId,
                normalizedClaimCaseId,
                StringComparison.Ordinal))
            .OrderByDescending(item => item.UpdatedAt)
            .ThenBy(item => item.Id, StringComparer.Ordinal)
            .ToList();
    }

    public async Task<ClaimSubmissionRecord?> GetAsync(
        string id,
        CancellationToken cancellationToken = default)
    {
        var normalizedId = NormalizeRequired(id, nameof(id));
        var envelope = await store.LoadAsync(cancellationToken);
        return envelope.Items.FirstOrDefault(item => string.Equals(
            item.Id,
            normalizedId,
            StringComparison.Ordinal));
    }

    public async Task<IReadOnlyList<PolicyRecord>> GetClaimablePoliciesAsync(
        string claimCaseId,
        CancellationToken cancellationToken = default)
    {
        var claim = await RequireSavedClaimCaseAsync(claimCaseId, cancellationToken);
        return (await policyStorageService.GetPoliciesAsync(cancellationToken))
            .Where(policy => IsClaimablePolicy(policy)
                && string.Equals(
                    policy.FamilyMemberId,
                    claim.FamilyMemberId,
                    StringComparison.Ordinal))
            .OrderBy(policy => policy.DisplayTitle, StringComparer.Ordinal)
            .ThenBy(policy => policy.Id, StringComparer.Ordinal)
            .ToList();
    }

    public async Task<ClaimSubmissionRecord> CreateAsync(
        ClaimSubmissionDraft draft,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(draft);
        var normalizedDraft = NormalizeDraft(draft);
        if (!string.Equals(
                normalizedDraft.Status,
                ClaimSubmissionValues.StatusPreparing,
                StringComparison.Ordinal))
        {
            throw new ClaimSubmissionTransitionException();
        }

        await mutationGate.WaitAsync(cancellationToken);
        try
        {
            await ValidateReferencesAsync(normalizedDraft, cancellationToken);
            var items = (await store.LoadAsync(cancellationToken)).Items.ToList();
            var timestamp = DateTimeOffset.UtcNow;
            var record = new ClaimSubmissionRecord(
                Id: $"submission_{Guid.NewGuid():N}",
                ClaimCaseId: normalizedDraft.ClaimCaseId,
                PolicyId: normalizedDraft.PolicyId,
                PolicyCoverageId: normalizedDraft.PolicyCoverageId,
                CoverageDisplayName: normalizedDraft.CoverageDisplayName,
                SubmittedDate: normalizedDraft.SubmittedDate,
                SubmittedAmount: normalizedDraft.SubmittedAmount,
                SubmittedClaimDocumentIds: normalizedDraft.SubmittedClaimDocumentIds!.ToArray(),
                Status: normalizedDraft.Status,
                Memo: normalizedDraft.Memo,
                Revision: 1,
                CreatedAt: timestamp,
                UpdatedAt: timestamp);

            if (items.Any(item => string.Equals(item.Id, record.Id, StringComparison.Ordinal)))
            {
                throw new InvalidOperationException("Generated claim submission ID is not unique.");
            }

            items.Add(record);
            await store.SaveAsync(items, cancellationToken);
            return record;
        }
        finally
        {
            mutationGate.Release();
        }
    }

    public async Task<ClaimSubmissionRecord> UpdateAsync(
        string id,
        int expectedRevision,
        ClaimSubmissionDraft draft,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(draft);
        var normalizedId = NormalizeRequired(id, nameof(id));
        var normalizedDraft = NormalizeDraft(draft);

        await mutationGate.WaitAsync(cancellationToken);
        try
        {
            var items = (await store.LoadAsync(cancellationToken)).Items.ToList();
            var index = items.FindIndex(item => string.Equals(
                item.Id,
                normalizedId,
                StringComparison.Ordinal));
            if (index < 0)
            {
                throw new ClaimSubmissionReferenceException();
            }

            var current = items[index];
            if (current.Revision != expectedRevision)
            {
                throw new ClaimSubmissionConcurrencyException();
            }

            if (ClaimSubmissionValues.IsTerminal(current.Status))
            {
                throw new ClaimSubmissionTransitionException();
            }

            if (!string.Equals(current.ClaimCaseId, normalizedDraft.ClaimCaseId, StringComparison.Ordinal)
                || !string.Equals(current.PolicyId, normalizedDraft.PolicyId, StringComparison.Ordinal))
            {
                throw new ClaimSubmissionReferenceException();
            }

            if (!string.Equals(current.Status, normalizedDraft.Status, StringComparison.Ordinal)
                && !ClaimSubmissionValues.GetAllowedTargets(current.Status)
                    .Contains(normalizedDraft.Status, StringComparer.Ordinal))
            {
                throw new ClaimSubmissionTransitionException();
            }

            if (!string.Equals(current.Status, ClaimSubmissionValues.StatusPreparing, StringComparison.Ordinal)
                && current.SubmittedClaimDocumentIds.Except(
                        normalizedDraft.SubmittedClaimDocumentIds!,
                        StringComparer.Ordinal)
                    .Any())
            {
                throw new ClaimSubmissionReferenceException();
            }

            await ValidateReferencesAsync(
                normalizedDraft,
                cancellationToken,
                current.Status);

            var updated = current with
            {
                PolicyCoverageId = normalizedDraft.PolicyCoverageId,
                CoverageDisplayName = normalizedDraft.CoverageDisplayName,
                SubmittedDate = normalizedDraft.SubmittedDate,
                SubmittedAmount = normalizedDraft.SubmittedAmount,
                SubmittedClaimDocumentIds = normalizedDraft.SubmittedClaimDocumentIds!.ToArray(),
                Status = normalizedDraft.Status,
                Memo = normalizedDraft.Memo,
                Revision = checked(current.Revision + 1),
                UpdatedAt = DateTimeOffset.UtcNow
            };
            items[index] = updated;
            await store.SaveAsync(items, cancellationToken);
            return updated;
        }
        finally
        {
            mutationGate.Release();
        }
    }

    private async Task ValidateReferencesAsync(
        ClaimSubmissionDraft draft,
        CancellationToken cancellationToken,
        string? currentStatus = null)
    {
        var claim = await RequireSavedClaimCaseAsync(draft.ClaimCaseId, cancellationToken);
        var policy = await policyStorageService.GetPolicyAsync(draft.PolicyId, cancellationToken)
            ?? throw new ClaimSubmissionReferenceException();

        if (string.IsNullOrWhiteSpace(policy.FamilyMemberId))
        {
            throw new ClaimSubmissionLegacyReviewRequiredException();
        }

        if (!string.Equals(policy.FamilyMemberId, claim.FamilyMemberId, StringComparison.Ordinal)
            || !IsClaimablePolicy(policy))
        {
            throw new ClaimSubmissionReferenceException();
        }

        var activeDocumentIds = (await documentStorageService.GetClaimDocumentsAsync(
                draft.ClaimCaseId,
                cancellationToken))
            .Where(link => link.DisabledAt is null)
            .Select(link => link.Id)
            .ToHashSet(StringComparer.Ordinal);
        if (draft.SubmittedClaimDocumentIds!.Any(id => !activeDocumentIds.Contains(id)))
        {
            throw new ClaimSubmissionReferenceException();
        }

        var requiresDetails = ClaimSubmissionValues.RequiresSubmittedDetails(draft.Status)
            || (string.Equals(draft.Status, ClaimSubmissionValues.StatusCancelled, StringComparison.Ordinal)
                && !string.Equals(currentStatus, ClaimSubmissionValues.StatusPreparing, StringComparison.Ordinal)
                && currentStatus is not null);
        if (requiresDetails
            && (draft.SubmittedDate is null
                || draft.SubmittedAmount is null
                || string.IsNullOrWhiteSpace(draft.CoverageDisplayName)))
        {
            throw new ArgumentException(
                "Submitted date, amount, and coverage are required after preparing.");
        }
    }

    private async Task<ClaimRecord> RequireSavedClaimCaseAsync(
        string claimCaseId,
        CancellationToken cancellationToken)
    {
        var normalizedClaimCaseId = NormalizeRequired(claimCaseId, nameof(claimCaseId));
        var claim = await claimCaseStorageService.GetClaimCaseAsync(
                normalizedClaimCaseId,
                cancellationToken)
            ?? throw new ClaimSubmissionReferenceException();

        if (string.IsNullOrWhiteSpace(claim.FamilyMemberId))
        {
            throw new ClaimSubmissionLegacyReviewRequiredException();
        }

        if (!string.Equals(claim.CaseStatus, ClaimCaseValues.StatusSaved, StringComparison.Ordinal))
        {
            throw new ClaimSubmissionReferenceException();
        }

        return claim;
    }

    private static bool IsClaimablePolicy(PolicyRecord policy)
    {
        return policy.DisabledAt is null
            && (string.Equals(
                    policy.ContractStatus,
                    InsurancePolicyValues.ContractStatusActive,
                    StringComparison.Ordinal)
                || string.Equals(
                    policy.ContractStatus,
                    InsurancePolicyValues.ContractStatusPremiumWaived,
                    StringComparison.Ordinal)
                || string.Equals(
                    policy.ContractStatus,
                    InsurancePolicyValues.LegacyContractStatusActive,
                    StringComparison.Ordinal));
    }

    private static ClaimSubmissionDraft NormalizeDraft(ClaimSubmissionDraft draft)
    {
        var normalizedStatus = NormalizeRequired(draft.Status, nameof(draft.Status));
        if (!ClaimSubmissionValues.Statuses.Contains(normalizedStatus, StringComparer.Ordinal))
        {
            throw new ArgumentException("Claim submission status is not supported.", nameof(draft.Status));
        }

        if (draft.SubmittedAmount < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(draft.SubmittedAmount),
                "Submitted amount must be a non-negative whole number.");
        }

        var documentIds = (draft.SubmittedClaimDocumentIds ?? [])
            .Select(id => NormalizeRequired(id, nameof(draft.SubmittedClaimDocumentIds)))
            .Distinct(StringComparer.Ordinal)
            .ToArray();

        return draft with
        {
            ClaimCaseId = NormalizeRequired(draft.ClaimCaseId, nameof(draft.ClaimCaseId)),
            PolicyId = NormalizeRequired(draft.PolicyId, nameof(draft.PolicyId)),
            PolicyCoverageId = NormalizeOptional(draft.PolicyCoverageId),
            CoverageDisplayName = NormalizeOptional(draft.CoverageDisplayName),
            SubmittedClaimDocumentIds = documentIds,
            Status = normalizedStatus,
            Memo = NormalizeOptional(draft.Memo)
        };
    }

    private static string NormalizeRequired(string value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Value is required.", parameterName);
        }

        return value.Trim();
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
