using System.Collections.Concurrent;
using System.IO;
using System.Security;
using System.Text.Json;
using FamilyClaimRef.App.Models.Storage;

namespace FamilyClaimRef.App.Services.Storage;

public sealed class JsonPolicyCoverageStorageService : IPolicyCoverageStorageService
{
    public const string StoreFileName = "policy-coverages.json";
    public const int StoreSchemaVersion = 1;

    private static readonly ConcurrentDictionary<string, SemaphoreSlim> MutationGates =
        new(StringComparer.OrdinalIgnoreCase);

    private readonly JsonFileStore<PolicyCoverageRecord> store;
    private readonly IClaimHistoryStorageReader policyHistoryStorageReader;
    private readonly IDocumentStorageService documentStorageService;
    private readonly SemaphoreSlim mutationGate;

    public JsonPolicyCoverageStorageService(
        string metadataRootPath,
        IClaimHistoryStorageReader policyHistoryStorageReader,
        IDocumentStorageService documentStorageService)
    {
        if (string.IsNullOrWhiteSpace(metadataRootPath))
        {
            throw new ArgumentException("Metadata root path is required.", nameof(metadataRootPath));
        }

        this.policyHistoryStorageReader = policyHistoryStorageReader
            ?? throw new ArgumentNullException(nameof(policyHistoryStorageReader));
        this.documentStorageService = documentStorageService
            ?? throw new ArgumentNullException(nameof(documentStorageService));

        var canonicalPath = Path.GetFullPath(Path.Combine(metadataRootPath, StoreFileName));
        store = new JsonFileStore<PolicyCoverageRecord>(
            metadataRootPath,
            StoreFileName,
            StoreSchemaVersion,
            preserveBackupOnReplace: true);
        mutationGate = MutationGates.GetOrAdd(
            canonicalPath,
            static _ => new SemaphoreSlim(1, 1));
    }

    public Task<IReadOnlyList<PolicyCoverageRecord>> GetPolicyCoveragesAsync(
        CancellationToken cancellationToken = default)
    {
        return ExecuteAsync<IReadOnlyList<PolicyCoverageRecord>>(async () =>
        {
            var context = await LoadValidatedAsync(cancellationToken);
            return Sort(context.Records);
        });
    }

    public Task<IReadOnlyList<PolicyCoverageRecord>> GetActivePolicyCoveragesAsync(
        CancellationToken cancellationToken = default)
    {
        return ExecuteAsync<IReadOnlyList<PolicyCoverageRecord>>(async () =>
        {
            var context = await LoadValidatedAsync(cancellationToken);
            return Sort(context.Records.Where(record => record.DisabledAt is null));
        });
    }

    public Task<PolicyCoverageRecord?> GetPolicyCoverageAsync(
        string id,
        CancellationToken cancellationToken = default)
    {
        return ExecuteAsync(async () =>
        {
            var normalizedId = NormalizeIdentifier(id);
            var context = await LoadValidatedAsync(cancellationToken);
            return context.Records.FirstOrDefault(record => string.Equals(
                record.PolicyCoverageId,
                normalizedId,
                StringComparison.Ordinal));
        });
    }

    public Task<PolicyCoverageRecord> CreatePolicyCoverageAsync(
        PolicyCoverageCreateDraft draft,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(draft);

        return ExecuteAsync(async () =>
        {
            var normalizedDraft = NormalizeCreateDraft(draft);
            await mutationGate.WaitAsync(cancellationToken);
            try
            {
                var context = await LoadValidatedAsync(cancellationToken);
                EnsureActivePolicy(context, normalizedDraft.PolicyId);
                await EnsureDraftSourceReferenceAsync(
                    normalizedDraft.PolicyId,
                    normalizedDraft.SourceKind,
                    normalizedDraft.SourcePolicyDocumentId,
                    cancellationToken);

                var records = context.Records.ToList();
                var id = $"coverage_{Guid.NewGuid():N}";
                if (records.Any(record => string.Equals(
                        record.PolicyCoverageId,
                        id,
                        StringComparison.Ordinal)))
                {
                    throw IntegrityViolation();
                }

                var timestamp = DateTimeOffset.UtcNow;
                var created = new PolicyCoverageRecord(
                    PolicyCoverageId: id,
                    PolicyId: normalizedDraft.PolicyId,
                    DisplayName: normalizedDraft.DisplayName,
                    ReviewStatus: normalizedDraft.ReviewStatus,
                    EffectiveFrom: normalizedDraft.EffectiveFrom,
                    EffectiveTo: normalizedDraft.EffectiveTo,
                    VisitTypeRule: normalizedDraft.VisitTypeRule,
                    SurgeryRule: normalizedDraft.SurgeryRule,
                    PrescriptionRule: normalizedDraft.PrescriptionRule,
                    DiagnosisRuleMode: normalizedDraft.DiagnosisRuleMode,
                    DiagnosisCodePrefixes: normalizedDraft.DiagnosisCodePrefixes,
                    SourceKind: normalizedDraft.SourceKind,
                    SourcePolicyDocumentId: normalizedDraft.SourcePolicyDocumentId,
                    SourceLocator: normalizedDraft.SourceLocator,
                    Memo: normalizedDraft.Memo,
                    Revision: 1,
                    CreatedAt: timestamp,
                    UpdatedAt: timestamp,
                    DisabledAt: null);

                records.Add(created);
                await store.SaveAsync(records, cancellationToken);
                return created;
            }
            finally
            {
                mutationGate.Release();
            }
        });
    }

    public Task<PolicyCoverageRecord> UpdatePolicyCoverageAsync(
        string id,
        int expectedRevision,
        PolicyCoverageUpdateDraft draft,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(draft);

        return ExecuteAsync(async () =>
        {
            var normalizedId = NormalizeIdentifier(id);
            var normalizedDraft = NormalizeUpdateDraft(draft);
            EnsureExpectedRevision(expectedRevision);

            await mutationGate.WaitAsync(cancellationToken);
            try
            {
                var context = await LoadValidatedAsync(cancellationToken);
                var records = context.Records.ToList();
                var index = FindTargetIndex(records, normalizedId);
                var current = RequireTarget(records, index);
                EnsureRevision(current, expectedRevision);
                EnsureActiveTarget(current);
                EnsureActivePolicy(context, current.PolicyId);
                await EnsureDraftSourceReferenceAsync(
                    current.PolicyId,
                    normalizedDraft.SourceKind,
                    normalizedDraft.SourcePolicyDocumentId,
                    cancellationToken);

                var reviewStatus = string.Equals(
                        current.ReviewStatus,
                        PolicyCoverageValues.ReviewStatusUserConfirmed,
                        StringComparison.Ordinal)
                    && HasSubstantiveRuleOrSourceChange(current, normalizedDraft)
                        ? PolicyCoverageValues.ReviewStatusNeedsReview
                        : current.ReviewStatus;

                var updated = current with
                {
                    DisplayName = normalizedDraft.DisplayName,
                    ReviewStatus = reviewStatus,
                    EffectiveFrom = normalizedDraft.EffectiveFrom,
                    EffectiveTo = normalizedDraft.EffectiveTo,
                    VisitTypeRule = normalizedDraft.VisitTypeRule,
                    SurgeryRule = normalizedDraft.SurgeryRule,
                    PrescriptionRule = normalizedDraft.PrescriptionRule,
                    DiagnosisRuleMode = normalizedDraft.DiagnosisRuleMode,
                    DiagnosisCodePrefixes = normalizedDraft.DiagnosisCodePrefixes,
                    SourceKind = normalizedDraft.SourceKind,
                    SourcePolicyDocumentId = normalizedDraft.SourcePolicyDocumentId,
                    SourceLocator = normalizedDraft.SourceLocator,
                    Memo = normalizedDraft.Memo,
                    Revision = checked(current.Revision + 1),
                    UpdatedAt = DateTimeOffset.UtcNow
                };

                records[index] = updated;
                await store.SaveAsync(records, cancellationToken);
                return updated;
            }
            finally
            {
                mutationGate.Release();
            }
        });
    }

    public Task<PolicyCoverageRecord> ChangePolicyCoverageReviewStatusAsync(
        string id,
        int expectedRevision,
        string targetReviewStatus,
        CancellationToken cancellationToken = default)
    {
        return ExecuteAsync(async () =>
        {
            var normalizedId = NormalizeIdentifier(id);
            var normalizedTargetStatus = NormalizeReviewStatus(targetReviewStatus);
            EnsureExpectedRevision(expectedRevision);

            await mutationGate.WaitAsync(cancellationToken);
            try
            {
                var context = await LoadValidatedAsync(cancellationToken);
                var records = context.Records.ToList();
                var index = FindTargetIndex(records, normalizedId);
                var current = RequireTarget(records, index);
                EnsureRevision(current, expectedRevision);
                EnsureActiveTarget(current);
                EnsureActivePolicy(context, current.PolicyId);

                if (string.Equals(
                        current.ReviewStatus,
                        normalizedTargetStatus,
                        StringComparison.Ordinal)
                    || !PolicyCoverageValues.GetAllowedReviewStatusTargets(current.ReviewStatus)
                        .Contains(normalizedTargetStatus, StringComparer.Ordinal))
                {
                    throw InvalidTransition();
                }

                var updated = current with
                {
                    ReviewStatus = normalizedTargetStatus,
                    Revision = checked(current.Revision + 1),
                    UpdatedAt = DateTimeOffset.UtcNow
                };
                records[index] = updated;
                await store.SaveAsync(records, cancellationToken);
                return updated;
            }
            finally
            {
                mutationGate.Release();
            }
        });
    }

    public Task<PolicyCoverageRecord> DisablePolicyCoverageAsync(
        string id,
        int expectedRevision,
        CancellationToken cancellationToken = default)
    {
        return ExecuteAsync(async () =>
        {
            var normalizedId = NormalizeIdentifier(id);
            EnsureExpectedRevision(expectedRevision);

            await mutationGate.WaitAsync(cancellationToken);
            try
            {
                var context = await LoadValidatedAsync(cancellationToken);
                var records = context.Records.ToList();
                var index = FindTargetIndex(records, normalizedId);
                var current = RequireTarget(records, index);
                EnsureRevision(current, expectedRevision);
                EnsureActiveTarget(current);

                var timestamp = DateTimeOffset.UtcNow;
                var updated = current with
                {
                    Revision = checked(current.Revision + 1),
                    UpdatedAt = timestamp,
                    DisabledAt = timestamp
                };
                records[index] = updated;
                await store.SaveAsync(records, cancellationToken);
                return updated;
            }
            finally
            {
                mutationGate.Release();
            }
        });
    }

    public Task<PolicyCoverageRecord> RestorePolicyCoverageAsync(
        string id,
        int expectedRevision,
        CancellationToken cancellationToken = default)
    {
        return ExecuteAsync(async () =>
        {
            var normalizedId = NormalizeIdentifier(id);
            EnsureExpectedRevision(expectedRevision);

            await mutationGate.WaitAsync(cancellationToken);
            try
            {
                var context = await LoadValidatedAsync(cancellationToken);
                var records = context.Records.ToList();
                var index = FindTargetIndex(records, normalizedId);
                var current = RequireTarget(records, index);
                EnsureRevision(current, expectedRevision);
                if (current.DisabledAt is null)
                {
                    throw TargetUnavailable();
                }

                EnsureActivePolicy(context, current.PolicyId);
                await EnsureDraftSourceReferenceAsync(
                    current.PolicyId,
                    current.SourceKind,
                    current.SourcePolicyDocumentId,
                    cancellationToken);

                var updated = current with
                {
                    Revision = checked(current.Revision + 1),
                    UpdatedAt = DateTimeOffset.UtcNow,
                    DisabledAt = null
                };
                records[index] = updated;
                await store.SaveAsync(records, cancellationToken);
                return updated;
            }
            finally
            {
                mutationGate.Release();
            }
        });
    }

    private async Task<ValidationContext> LoadValidatedAsync(
        CancellationToken cancellationToken)
    {
        var envelope = await store.LoadAsync(cancellationToken);
        var policies = await policyHistoryStorageReader.GetAllPoliciesForHistoryAsync(
            cancellationToken);
        var policyLookup = new Dictionary<string, PolicyRecord>(StringComparer.Ordinal);
        foreach (var policy in policies)
        {
            if (string.IsNullOrWhiteSpace(policy.Id)
                || !policyLookup.TryAdd(policy.Id, policy))
            {
                throw IntegrityViolation();
            }
        }

        var coverageIds = new HashSet<string>(StringComparer.Ordinal);
        foreach (var record in envelope.Items)
        {
            ValidateStoredRecord(record, coverageIds);
            if (!policyLookup.ContainsKey(record.PolicyId))
            {
                throw IntegrityViolation();
            }

            await EnsureStoredSourceReferenceAsync(record, cancellationToken);
        }

        return new ValidationContext(envelope.Items.ToList(), policyLookup);
    }

    private async Task EnsureStoredSourceReferenceAsync(
        PolicyCoverageRecord record,
        CancellationToken cancellationToken)
    {
        if (record.SourcePolicyDocumentId is null)
        {
            return;
        }

        var links = await documentStorageService.GetPolicyDocumentsAsync(
            record.PolicyId,
            cancellationToken);
        if (!links.Any(link => string.Equals(
                link.Id,
                record.SourcePolicyDocumentId,
                StringComparison.Ordinal)
            && string.Equals(link.PolicyId, record.PolicyId, StringComparison.Ordinal)))
        {
            throw IntegrityViolation();
        }
    }

    private async Task EnsureDraftSourceReferenceAsync(
        string policyId,
        string sourceKind,
        string? sourcePolicyDocumentId,
        CancellationToken cancellationToken)
    {
        if (string.Equals(
                sourceKind,
                PolicyCoverageValues.SourcePolicyDocument,
                StringComparison.Ordinal)
            && sourcePolicyDocumentId is null)
        {
            throw ReferenceInvalid();
        }

        if (sourcePolicyDocumentId is null)
        {
            return;
        }

        var links = await documentStorageService.GetPolicyDocumentsAsync(
            policyId,
            cancellationToken);
        if (!links.Any(link => string.Equals(
                link.Id,
                sourcePolicyDocumentId,
                StringComparison.Ordinal)
            && string.Equals(link.PolicyId, policyId, StringComparison.Ordinal)))
        {
            throw ReferenceInvalid();
        }
    }

    private static PolicyCoverageCreateDraft NormalizeCreateDraft(
        PolicyCoverageCreateDraft draft)
    {
        var reviewStatus = NormalizeReviewStatus(draft.ReviewStatus);
        if (!PolicyCoverageValues.InitialReviewStatuses.Contains(
                reviewStatus,
                StringComparer.Ordinal))
        {
            throw InvalidTransition();
        }

        return new PolicyCoverageCreateDraft(
            PolicyId: NormalizeRequired(draft.PolicyId),
            DisplayName: NormalizeRequired(draft.DisplayName),
            ReviewStatus: reviewStatus,
            EffectiveFrom: draft.EffectiveFrom,
            EffectiveTo: draft.EffectiveTo,
            VisitTypeRule: NormalizeSupported(
                draft.VisitTypeRule,
                PolicyCoverageValues.VisitTypeRules),
            SurgeryRule: NormalizeSupported(
                draft.SurgeryRule,
                PolicyCoverageValues.ConditionRules),
            PrescriptionRule: NormalizeSupported(
                draft.PrescriptionRule,
                PolicyCoverageValues.ConditionRules),
            DiagnosisRuleMode: NormalizeSupported(
                draft.DiagnosisRuleMode,
                PolicyCoverageValues.DiagnosisRuleModes),
            DiagnosisCodePrefixes: NormalizePrefixes(draft.DiagnosisCodePrefixes),
            SourceKind: NormalizeSupported(
                draft.SourceKind,
                PolicyCoverageValues.SourceKinds),
            SourcePolicyDocumentId: NormalizeOptional(draft.SourcePolicyDocumentId),
            SourceLocator: NormalizeOptional(draft.SourceLocator),
            Memo: NormalizeOptional(draft.Memo))
            .ValidateDatesAndPrefixes();
    }

    private static PolicyCoverageUpdateDraft NormalizeUpdateDraft(
        PolicyCoverageUpdateDraft draft)
    {
        return new PolicyCoverageUpdateDraft(
            DisplayName: NormalizeRequired(draft.DisplayName),
            EffectiveFrom: draft.EffectiveFrom,
            EffectiveTo: draft.EffectiveTo,
            VisitTypeRule: NormalizeSupported(
                draft.VisitTypeRule,
                PolicyCoverageValues.VisitTypeRules),
            SurgeryRule: NormalizeSupported(
                draft.SurgeryRule,
                PolicyCoverageValues.ConditionRules),
            PrescriptionRule: NormalizeSupported(
                draft.PrescriptionRule,
                PolicyCoverageValues.ConditionRules),
            DiagnosisRuleMode: NormalizeSupported(
                draft.DiagnosisRuleMode,
                PolicyCoverageValues.DiagnosisRuleModes),
            DiagnosisCodePrefixes: NormalizePrefixes(draft.DiagnosisCodePrefixes),
            SourceKind: NormalizeSupported(
                draft.SourceKind,
                PolicyCoverageValues.SourceKinds),
            SourcePolicyDocumentId: NormalizeOptional(draft.SourcePolicyDocumentId),
            SourceLocator: NormalizeOptional(draft.SourceLocator),
            Memo: NormalizeOptional(draft.Memo))
            .ValidateDatesAndPrefixes();
    }

    private static void ValidateStoredRecord(
        PolicyCoverageRecord record,
        ISet<string> coverageIds)
    {
        if (record is null
            || !IsNormalizedRequired(record.PolicyCoverageId)
            || !coverageIds.Add(record.PolicyCoverageId)
            || !IsNormalizedRequired(record.PolicyId)
            || !IsNormalizedRequired(record.DisplayName)
            || !PolicyCoverageValues.ReviewStatuses.Contains(
                record.ReviewStatus,
                StringComparer.Ordinal)
            || !PolicyCoverageValues.VisitTypeRules.Contains(
                record.VisitTypeRule,
                StringComparer.Ordinal)
            || !PolicyCoverageValues.ConditionRules.Contains(
                record.SurgeryRule,
                StringComparer.Ordinal)
            || !PolicyCoverageValues.ConditionRules.Contains(
                record.PrescriptionRule,
                StringComparer.Ordinal)
            || !PolicyCoverageValues.DiagnosisRuleModes.Contains(
                record.DiagnosisRuleMode,
                StringComparer.Ordinal)
            || !PolicyCoverageValues.SourceKinds.Contains(
                record.SourceKind,
                StringComparer.Ordinal)
            || !IsNormalizedOptional(record.SourcePolicyDocumentId)
            || !IsNormalizedOptional(record.SourceLocator)
            || !IsNormalizedOptional(record.Memo)
            || record.DiagnosisCodePrefixes is null
            || record.Revision < 1
            || record.CreatedAt == default
            || record.UpdatedAt == default
            || record.UpdatedAt < record.CreatedAt
            || (record.DisabledAt is not null
                && (record.DisabledAt < record.CreatedAt
                    || record.DisabledAt > record.UpdatedAt))
            || (record.EffectiveFrom is not null
                && record.EffectiveTo is not null
                && record.EffectiveFrom > record.EffectiveTo)
            || (string.Equals(
                    record.SourceKind,
                    PolicyCoverageValues.SourcePolicyDocument,
                    StringComparison.Ordinal)
                && record.SourcePolicyDocumentId is null))
        {
            throw IntegrityViolation();
        }

        var prefixes = new HashSet<string>(StringComparer.Ordinal);
        foreach (var prefix in record.DiagnosisCodePrefixes)
        {
            if (!IsNormalizedRequired(prefix)
                || !string.Equals(prefix, prefix.ToUpperInvariant(), StringComparison.Ordinal)
                || !prefixes.Add(prefix))
            {
                throw IntegrityViolation();
            }
        }

        if (string.Equals(
                record.DiagnosisRuleMode,
                PolicyCoverageValues.DiagnosisRulePrefixList,
                StringComparison.Ordinal)
            && record.DiagnosisCodePrefixes.Length == 0)
        {
            throw IntegrityViolation();
        }
    }

    private static bool HasSubstantiveRuleOrSourceChange(
        PolicyCoverageRecord current,
        PolicyCoverageUpdateDraft draft)
    {
        return current.EffectiveFrom != draft.EffectiveFrom
            || current.EffectiveTo != draft.EffectiveTo
            || !string.Equals(current.VisitTypeRule, draft.VisitTypeRule, StringComparison.Ordinal)
            || !string.Equals(current.SurgeryRule, draft.SurgeryRule, StringComparison.Ordinal)
            || !string.Equals(
                current.PrescriptionRule,
                draft.PrescriptionRule,
                StringComparison.Ordinal)
            || !string.Equals(
                current.DiagnosisRuleMode,
                draft.DiagnosisRuleMode,
                StringComparison.Ordinal)
            || !current.DiagnosisCodePrefixes.SequenceEqual(
                draft.DiagnosisCodePrefixes,
                StringComparer.Ordinal)
            || !string.Equals(current.SourceKind, draft.SourceKind, StringComparison.Ordinal)
            || !string.Equals(
                current.SourcePolicyDocumentId,
                draft.SourcePolicyDocumentId,
                StringComparison.Ordinal)
            || !string.Equals(current.SourceLocator, draft.SourceLocator, StringComparison.Ordinal);
    }

    private static void EnsureActivePolicy(ValidationContext context, string policyId)
    {
        if (!context.Policies.TryGetValue(policyId, out var policy)
            || !IsActivePolicy(policy))
        {
            throw ReferenceInvalid();
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

    private static int FindTargetIndex(
        IReadOnlyList<PolicyCoverageRecord> records,
        string id)
    {
        return records.ToList().FindIndex(record => string.Equals(
            record.PolicyCoverageId,
            id,
            StringComparison.Ordinal));
    }

    private static PolicyCoverageRecord RequireTarget(
        IReadOnlyList<PolicyCoverageRecord> records,
        int index)
    {
        if (index < 0)
        {
            throw TargetUnavailable();
        }

        return records[index];
    }

    private static void EnsureRevision(PolicyCoverageRecord record, int expectedRevision)
    {
        if (record.Revision != expectedRevision)
        {
            throw VersionConflict();
        }
    }

    private static void EnsureActiveTarget(PolicyCoverageRecord record)
    {
        if (record.DisabledAt is not null)
        {
            throw TargetUnavailable();
        }
    }

    private static void EnsureExpectedRevision(int expectedRevision)
    {
        if (expectedRevision < 1)
        {
            throw VersionConflict();
        }
    }

    private static string NormalizeIdentifier(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw TargetUnavailable();
        }

        return value.Trim();
    }

    private static string NormalizeRequired(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw IntegrityViolation();
        }

        return value.Trim();
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    private static string NormalizeReviewStatus(string value)
    {
        var normalized = NormalizeRequired(value);
        if (!PolicyCoverageValues.ReviewStatuses.Contains(
                normalized,
                StringComparer.Ordinal))
        {
            throw InvalidTransition();
        }

        return normalized;
    }

    private static string NormalizeSupported(
        string value,
        IReadOnlyList<string> allowedValues)
    {
        var normalized = NormalizeRequired(value);
        if (!allowedValues.Contains(normalized, StringComparer.Ordinal))
        {
            throw IntegrityViolation();
        }

        return normalized;
    }

    private static string[] NormalizePrefixes(string[] prefixes)
    {
        if (prefixes is null)
        {
            throw IntegrityViolation();
        }

        var normalized = new List<string>();
        var seen = new HashSet<string>(StringComparer.Ordinal);
        foreach (var prefix in prefixes)
        {
            var value = NormalizeRequired(prefix).ToUpperInvariant();
            if (seen.Add(value))
            {
                normalized.Add(value);
            }
        }

        return normalized.ToArray();
    }

    private static bool IsNormalizedRequired(string? value)
    {
        return !string.IsNullOrWhiteSpace(value)
            && string.Equals(value, value.Trim(), StringComparison.Ordinal);
    }

    private static bool IsNormalizedOptional(string? value)
    {
        return value is null || IsNormalizedRequired(value);
    }

    private static IReadOnlyList<PolicyCoverageRecord> Sort(
        IEnumerable<PolicyCoverageRecord> records)
    {
        return records
            .OrderBy(record => record.CreatedAt)
            .ThenBy(record => record.PolicyCoverageId, StringComparer.Ordinal)
            .ToList();
    }

    private static Task<T> ExecuteAsync<T>(Func<Task<T>> operation)
    {
        return ExecuteCoreAsync(operation);
    }

    private static async Task<T> ExecuteCoreAsync<T>(Func<Task<T>> operation)
    {
        try
        {
            return await operation();
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (PolicyCoverageStorageException)
        {
            throw;
        }
        catch (Exception exception) when (exception is IOException
            or UnauthorizedAccessException
            or SecurityException
            or JsonException
            or InvalidOperationException
            or ArgumentException
            or NotSupportedException
            or OverflowException)
        {
            throw new PolicyCoverageStorageException(
                PolicyCoverageStorageErrorCode.IntegrityViolation,
                "Policy coverage storage integrity validation failed.");
        }
    }

    private static PolicyCoverageStorageException TargetUnavailable()
    {
        return new PolicyCoverageStorageException(
            PolicyCoverageStorageErrorCode.TargetUnavailable,
            "Policy coverage target is unavailable.");
    }

    private static PolicyCoverageStorageException VersionConflict()
    {
        return new PolicyCoverageStorageException(
            PolicyCoverageStorageErrorCode.VersionConflict,
            "Policy coverage version conflict.");
    }

    private static PolicyCoverageStorageException InvalidTransition()
    {
        return new PolicyCoverageStorageException(
            PolicyCoverageStorageErrorCode.InvalidTransition,
            "Policy coverage review status transition is not allowed.");
    }

    private static PolicyCoverageStorageException ReferenceInvalid()
    {
        return new PolicyCoverageStorageException(
            PolicyCoverageStorageErrorCode.ReferenceInvalid,
            "Policy coverage reference is unavailable or inconsistent.");
    }

    private static PolicyCoverageStorageException IntegrityViolation()
    {
        return new PolicyCoverageStorageException(
            PolicyCoverageStorageErrorCode.IntegrityViolation,
            "Policy coverage storage integrity validation failed.");
    }

    private sealed record ValidationContext(
        IReadOnlyList<PolicyCoverageRecord> Records,
        IReadOnlyDictionary<string, PolicyRecord> Policies);
}

internal static class PolicyCoverageDraftValidationExtensions
{
    public static PolicyCoverageCreateDraft ValidateDatesAndPrefixes(
        this PolicyCoverageCreateDraft draft)
    {
        Validate(draft.EffectiveFrom, draft.EffectiveTo, draft.DiagnosisRuleMode, draft.DiagnosisCodePrefixes);
        return draft;
    }

    public static PolicyCoverageUpdateDraft ValidateDatesAndPrefixes(
        this PolicyCoverageUpdateDraft draft)
    {
        Validate(draft.EffectiveFrom, draft.EffectiveTo, draft.DiagnosisRuleMode, draft.DiagnosisCodePrefixes);
        return draft;
    }

    private static void Validate(
        DateOnly? effectiveFrom,
        DateOnly? effectiveTo,
        string diagnosisRuleMode,
        IReadOnlyCollection<string> diagnosisCodePrefixes)
    {
        if ((effectiveFrom is not null && effectiveTo is not null && effectiveFrom > effectiveTo)
            || (string.Equals(
                    diagnosisRuleMode,
                    PolicyCoverageValues.DiagnosisRulePrefixList,
                    StringComparison.Ordinal)
                && diagnosisCodePrefixes.Count == 0))
        {
            throw new PolicyCoverageStorageException(
                PolicyCoverageStorageErrorCode.IntegrityViolation,
                "Policy coverage storage integrity validation failed.");
        }
    }
}
