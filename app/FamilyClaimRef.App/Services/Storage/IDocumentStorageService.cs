using FamilyClaimRef.App.Models.Storage;

namespace FamilyClaimRef.App.Services.Storage;

public interface IDocumentStorageService
{
    Task<IReadOnlyList<DocumentRecord>> GetDocumentsAsync(CancellationToken cancellationToken = default);

    Task<DocumentRecord?> GetDocumentByIdAsync(string documentId, CancellationToken cancellationToken = default);

    Task<DocumentRecord> AddDocumentAsync(DocumentDraft draft, CancellationToken cancellationToken = default);

    Task DisableDocumentAsync(string documentId, DateTimeOffset disabledAt, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PolicyDocumentRecord>> GetPolicyDocumentsAsync(string policyId, CancellationToken cancellationToken = default);

    Task<PolicyDocumentRecord> AddPolicyDocumentAsync(PolicyDocumentDraft draft, CancellationToken cancellationToken = default);

    async Task<PolicyDocumentRecord> ReplaceActivePolicyDocumentAsync(
        PolicyDocumentDraft draft,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(draft);

        var activeLinks = (await GetPolicyDocumentsAsync(draft.PolicyId, cancellationToken))
            .Where(link => link.DisabledAt is null
                && string.Equals(link.DocumentType, draft.DocumentType, StringComparison.OrdinalIgnoreCase))
            .ToList();
        var replacement = await AddPolicyDocumentAsync(draft, cancellationToken);

        try
        {
            var disabledAt = DateTimeOffset.UtcNow;
            foreach (var link in activeLinks)
            {
                await DisablePolicyDocumentAsync(link.Id, disabledAt, cancellationToken);
            }
        }
        catch
        {
            await DisablePolicyDocumentAsync(
                replacement.Id,
                DateTimeOffset.UtcNow,
                CancellationToken.None);
            throw;
        }

        return replacement;
    }

    Task DisablePolicyDocumentAsync(string policyDocumentId, DateTimeOffset disabledAt, CancellationToken cancellationToken = default);

    async Task<int> DisableActivePolicyDocumentsByTypeAsync(
        string policyId,
        string documentType,
        DateTimeOffset disabledAt,
        CancellationToken cancellationToken = default)
    {
        var activeLinks = (await GetPolicyDocumentsAsync(policyId, cancellationToken))
            .Where(link => link.DisabledAt is null
                && string.Equals(link.DocumentType, documentType, StringComparison.OrdinalIgnoreCase))
            .ToList();

        foreach (var link in activeLinks)
        {
            await DisablePolicyDocumentAsync(link.Id, disabledAt, cancellationToken);
        }

        return activeLinks.Count;
    }

    Task<IReadOnlyList<ClaimDocumentRecord>> GetClaimDocumentsAsync(string claimId, CancellationToken cancellationToken = default);

    Task<ClaimDocumentRecord> AddClaimDocumentAsync(ClaimDocumentDraft draft, CancellationToken cancellationToken = default);

    Task DisableClaimDocumentAsync(string claimDocumentId, DateTimeOffset disabledAt, CancellationToken cancellationToken = default);

    async Task<bool> ActiveTargetDocumentWithSha256ExistsAsync(
        string targetKind,
        string targetId,
        string sha256,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(targetKind);
        ArgumentException.ThrowIfNullOrWhiteSpace(targetId);
        ArgumentException.ThrowIfNullOrWhiteSpace(sha256);

        IReadOnlyList<string> documentIds = targetKind switch
        {
            "policy" => (await GetPolicyDocumentsAsync(targetId, cancellationToken))
                .Where(link => link.DisabledAt is null)
                .Select(link => link.DocumentId)
                .ToList(),
            "claim" => (await GetClaimDocumentsAsync(targetId, cancellationToken))
                .Where(link => link.DisabledAt is null)
                .Select(link => link.DocumentId)
                .ToList(),
            _ => throw new ArgumentException("Target kind must be policy or claim.", nameof(targetKind))
        };

        foreach (var documentId in documentIds)
        {
            var document = await GetDocumentByIdAsync(documentId, cancellationToken);
            if (document is not null
                && document.DisabledAt is null
                && string.Equals(document.Sha256, sha256, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }
}
