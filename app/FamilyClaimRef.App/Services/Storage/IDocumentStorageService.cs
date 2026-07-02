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
}
