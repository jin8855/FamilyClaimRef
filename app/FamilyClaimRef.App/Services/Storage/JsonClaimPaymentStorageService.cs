using System.Collections.Concurrent;
using System.IO;
using FamilyClaimRef.App.Models.Storage;

namespace FamilyClaimRef.App.Services.Storage;

public sealed class JsonClaimPaymentStorageService :
    IClaimPaymentStorageService,
    IClaimPaymentHistoryStorageReader
{
    private const string FileName = "claim-payments.json";

    private static readonly ConcurrentDictionary<string, SemaphoreSlim> MutationGates =
        new(StringComparer.OrdinalIgnoreCase);

    private readonly JsonFileStore<ClaimPaymentRecord> store;
    private readonly IClaimSubmissionStorageService submissionStorageService;
    private readonly IClaimCaseStorageService claimCaseStorageService;
    private readonly IPolicyClaimStorageService policyStorageService;
    private readonly SemaphoreSlim mutationGate;

    public JsonClaimPaymentStorageService(
        string metadataRootPath,
        IClaimSubmissionStorageService submissionStorageService,
        IClaimCaseStorageService claimCaseStorageService,
        IPolicyClaimStorageService policyStorageService)
    {
        if (string.IsNullOrWhiteSpace(metadataRootPath))
        {
            throw new ArgumentException("Metadata root path is required.", nameof(metadataRootPath));
        }

        this.submissionStorageService = submissionStorageService
            ?? throw new ArgumentNullException(nameof(submissionStorageService));
        this.claimCaseStorageService = claimCaseStorageService
            ?? throw new ArgumentNullException(nameof(claimCaseStorageService));
        this.policyStorageService = policyStorageService
            ?? throw new ArgumentNullException(nameof(policyStorageService));

        var canonicalPath = Path.GetFullPath(Path.Combine(metadataRootPath, FileName));
        store = new JsonFileStore<ClaimPaymentRecord>(
            metadataRootPath,
            FileName,
            preserveBackupOnReplace: true);
        mutationGate = MutationGates.GetOrAdd(canonicalPath, _ => new SemaphoreSlim(1, 1));
    }

    public async Task<IReadOnlyList<ClaimPaymentRecord>> GetBySubmissionAsync(
        string claimSubmissionId,
        CancellationToken cancellationToken = default)
    {
        var normalizedSubmissionId = NormalizeRequired(
            claimSubmissionId,
            nameof(claimSubmissionId));
        var envelope = await store.LoadAsync(cancellationToken);

        return envelope.Items
            .Where(item => string.Equals(
                item.ClaimSubmissionId,
                normalizedSubmissionId,
                StringComparison.Ordinal))
            .OrderByDescending(item => item.UpdatedAt)
            .ThenBy(item => item.Id, StringComparer.Ordinal)
            .ToList();
    }

    public async Task<IReadOnlyList<ClaimPaymentRecord>> GetAllPaymentsForHistoryAsync(
        CancellationToken cancellationToken = default)
    {
        return (await store.LoadAsync(cancellationToken)).Items.ToList();
    }

    public async Task<ClaimPaymentRecord?> GetAsync(
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

    public async Task<ClaimPaymentRecord> CreateAsync(
        ClaimPaymentDraft draft,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(draft);
        var normalizedDraft = NormalizeDraft(draft);
        if (!string.Equals(
                normalizedDraft.Status,
                ClaimPaymentValues.StatusPending,
                StringComparison.Ordinal))
        {
            throw new ClaimPaymentTransitionException();
        }

        await mutationGate.WaitAsync(cancellationToken);
        try
        {
            await ValidateParentAsync(
                normalizedDraft.ClaimSubmissionId,
                normalizedDraft.Status,
                cancellationToken);
            var items = (await store.LoadAsync(cancellationToken)).Items.ToList();
            var timestamp = DateTimeOffset.UtcNow;
            var record = new ClaimPaymentRecord(
                Id: $"payment_{Guid.NewGuid():N}",
                ClaimSubmissionId: normalizedDraft.ClaimSubmissionId,
                Status: normalizedDraft.Status,
                PaidDate: normalizedDraft.PaidDate,
                PaidAmount: normalizedDraft.PaidAmount,
                PaidCoverageDisplayName: normalizedDraft.PaidCoverageDisplayName,
                DenyReason: normalizedDraft.DenyReason,
                ReductionReason: normalizedDraft.ReductionReason,
                AdditionalDocumentsMemo: normalizedDraft.AdditionalDocumentsMemo,
                Memo: normalizedDraft.Memo,
                Revision: 1,
                CreatedAt: timestamp,
                UpdatedAt: timestamp);

            if (items.Any(item => string.Equals(item.Id, record.Id, StringComparison.Ordinal)))
            {
                throw new InvalidOperationException("Generated claim payment ID is not unique.");
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

    public async Task<ClaimPaymentRecord> UpdateAsync(
        string id,
        int expectedRevision,
        ClaimPaymentDraft draft,
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
                throw new ClaimPaymentReferenceException();
            }

            var current = items[index];
            if (current.Revision != expectedRevision)
            {
                throw new ClaimPaymentConcurrencyException();
            }

            if (ClaimPaymentValues.IsTerminal(current.Status))
            {
                throw new ClaimPaymentTransitionException();
            }

            if (!string.Equals(
                    current.ClaimSubmissionId,
                    normalizedDraft.ClaimSubmissionId,
                    StringComparison.Ordinal))
            {
                throw new ClaimPaymentReferenceException();
            }

            if (!string.Equals(current.Status, normalizedDraft.Status, StringComparison.Ordinal)
                && !ClaimPaymentValues.GetAllowedTargets(current.Status)
                    .Contains(normalizedDraft.Status, StringComparer.Ordinal))
            {
                throw new ClaimPaymentTransitionException();
            }

            await ValidateParentAsync(
                normalizedDraft.ClaimSubmissionId,
                normalizedDraft.Status,
                cancellationToken);

            var updated = current with
            {
                Status = normalizedDraft.Status,
                PaidDate = normalizedDraft.PaidDate,
                PaidAmount = normalizedDraft.PaidAmount,
                PaidCoverageDisplayName = normalizedDraft.PaidCoverageDisplayName,
                DenyReason = normalizedDraft.DenyReason,
                ReductionReason = normalizedDraft.ReductionReason,
                AdditionalDocumentsMemo = normalizedDraft.AdditionalDocumentsMemo,
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

    private async Task ValidateParentAsync(
        string submissionId,
        string targetStatus,
        CancellationToken cancellationToken)
    {
        var submission = await submissionStorageService.GetAsync(submissionId, cancellationToken)
            ?? throw new ClaimPaymentReferenceException();
        if (!ClaimSubmissionValues.Statuses.Contains(submission.Status, StringComparer.Ordinal))
        {
            throw new ClaimPaymentReferenceException();
        }

        if (string.Equals(submission.Status, ClaimSubmissionValues.StatusPreparing, StringComparison.Ordinal)
            || string.Equals(submission.Status, ClaimSubmissionValues.StatusCancelled, StringComparison.Ordinal))
        {
            throw new ClaimPaymentReferenceException();
        }

        if (ClaimPaymentValues.IsTerminal(targetStatus)
            && !string.Equals(targetStatus, ClaimPaymentValues.StatusCancelled, StringComparison.Ordinal)
            && !string.Equals(
                submission.Status,
                ClaimSubmissionValues.StatusCompleted,
                StringComparison.Ordinal))
        {
            throw new ClaimPaymentReferenceException();
        }

        var claim = await claimCaseStorageService.GetClaimCaseAsync(
                submission.ClaimCaseId,
                cancellationToken)
            ?? throw new ClaimPaymentReferenceException();
        if (string.IsNullOrWhiteSpace(claim.FamilyMemberId))
        {
            throw new ClaimPaymentLegacyReviewRequiredException();
        }

        if (claim.DisabledAt is not null
            || !string.Equals(claim.CaseStatus, ClaimCaseValues.StatusSaved, StringComparison.Ordinal))
        {
            throw new ClaimPaymentReferenceException();
        }

        var policy = await policyStorageService.GetPolicyAsync(
                submission.PolicyId,
                cancellationToken)
            ?? throw new ClaimPaymentReferenceException();
        if (string.IsNullOrWhiteSpace(policy.FamilyMemberId))
        {
            throw new ClaimPaymentLegacyReviewRequiredException();
        }

        if (!string.Equals(policy.FamilyMemberId, claim.FamilyMemberId, StringComparison.Ordinal)
            || !IsActivePolicy(policy))
        {
            throw new ClaimPaymentReferenceException();
        }
    }

    private static bool IsActivePolicy(PolicyRecord policy)
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

    private static ClaimPaymentDraft NormalizeDraft(ClaimPaymentDraft draft)
    {
        var normalizedStatus = NormalizeRequired(draft.Status, nameof(draft.Status));
        if (!ClaimPaymentValues.Statuses.Contains(normalizedStatus, StringComparer.Ordinal))
        {
            throw new ArgumentException("Claim payment status is not supported.", nameof(draft.Status));
        }

        if (draft.PaidAmount is <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(draft.PaidAmount),
                "Paid amount must be a positive whole number when entered.");
        }

        var normalized = draft with
        {
            ClaimSubmissionId = NormalizeRequired(
                draft.ClaimSubmissionId,
                nameof(draft.ClaimSubmissionId)),
            Status = normalizedStatus,
            PaidCoverageDisplayName = NormalizeOptional(draft.PaidCoverageDisplayName),
            DenyReason = NormalizeOptional(draft.DenyReason),
            ReductionReason = NormalizeOptional(draft.ReductionReason),
            AdditionalDocumentsMemo = NormalizeOptional(draft.AdditionalDocumentsMemo),
            Memo = NormalizeOptional(draft.Memo)
        };
        ValidateStatusFields(normalized);
        return normalized;
    }

    private static void ValidateStatusFields(ClaimPaymentDraft draft)
    {
        switch (draft.Status)
        {
            case ClaimPaymentValues.StatusPaid:
                RequirePaidFields(draft);
                if (draft.DenyReason is not null || draft.ReductionReason is not null)
                {
                    throw new ArgumentException("Paid result cannot include deny or reduction reasons.");
                }

                break;
            case ClaimPaymentValues.StatusPartiallyPaid:
                RequirePaidFields(draft);
                if (draft.ReductionReason is null || draft.DenyReason is not null)
                {
                    throw new ArgumentException("Partial payment requires only a reduction reason.");
                }

                break;
            case ClaimPaymentValues.StatusDenied:
                if (draft.DenyReason is null
                    || draft.PaidDate is not null
                    || draft.PaidAmount is not null
                    || draft.PaidCoverageDisplayName is not null
                    || draft.ReductionReason is not null)
                {
                    throw new ArgumentException("Denied result fields are invalid.");
                }

                break;
            case ClaimPaymentValues.StatusCancelled:
                if (draft.PaidDate is not null
                    || draft.PaidAmount is not null
                    || draft.PaidCoverageDisplayName is not null
                    || draft.DenyReason is not null
                    || draft.ReductionReason is not null
                    || draft.AdditionalDocumentsMemo is not null)
                {
                    throw new ArgumentException("Cancelled payment result fields must be empty.");
                }

                break;
        }
    }

    private static void RequirePaidFields(ClaimPaymentDraft draft)
    {
        if (draft.PaidDate is null
            || draft.PaidAmount is null
            || draft.PaidCoverageDisplayName is null)
        {
            throw new ArgumentException("Payment date, amount, and coverage are required.");
        }
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
