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

    Task DisablePolicyDocumentAsync(string policyDocumentId, DateTimeOffset disabledAt, CancellationToken cancellationToken = default);

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
