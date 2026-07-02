using FamilyClaimRef.App.Models.Storage;

namespace FamilyClaimRef.App.Services.Storage;

public sealed class DocumentLinkCoordinator
{
    private readonly IDocumentStorageService documentStorageService;

    public DocumentLinkCoordinator(IDocumentStorageService documentStorageService)
    {
        this.documentStorageService = documentStorageService
            ?? throw new ArgumentNullException(nameof(documentStorageService));
    }

    public async Task<PolicyDocumentLinkResult> LinkPolicyDocumentAsync(
        PolicyDocumentLinkRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        cancellationToken.ThrowIfCancellationRequested();

        var policyId = NormalizeRequiredValue(request.PolicyId, nameof(request.PolicyId));
        var documentId = NormalizeRequiredValue(request.DocumentId, nameof(request.DocumentId));
        var documentType = NormalizeRequiredValue(request.DocumentType, nameof(request.DocumentType));

        await EnsureNoActivePolicyDuplicateAsync(policyId, documentId, cancellationToken);

        var policyDocument = await documentStorageService.AddPolicyDocumentAsync(
            new PolicyDocumentDraft(policyId, documentId, documentType),
            cancellationToken);

        return new PolicyDocumentLinkResult(policyDocument);
    }

    public async Task<ClaimDocumentLinkResult> LinkClaimDocumentAsync(
        ClaimDocumentLinkRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        cancellationToken.ThrowIfCancellationRequested();

        var claimId = NormalizeRequiredValue(request.ClaimId, nameof(request.ClaimId));
        var documentId = NormalizeRequiredValue(request.DocumentId, nameof(request.DocumentId));
        var documentType = NormalizeRequiredValue(request.DocumentType, nameof(request.DocumentType));

        await EnsureNoActiveClaimDuplicateAsync(claimId, documentId, cancellationToken);

        var claimDocument = await documentStorageService.AddClaimDocumentAsync(
            new ClaimDocumentDraft(claimId, documentId, documentType),
            cancellationToken);

        return new ClaimDocumentLinkResult(claimDocument);
    }

    private async Task EnsureNoActivePolicyDuplicateAsync(
        string policyId,
        string documentId,
        CancellationToken cancellationToken)
    {
        var policyDocuments = await documentStorageService.GetPolicyDocumentsAsync(policyId, cancellationToken);
        if (policyDocuments.Any(policyDocument =>
                policyDocument.DisabledAt is null
                && string.Equals(policyDocument.DocumentId, documentId, StringComparison.Ordinal)))
        {
            throw new InvalidOperationException("Active policy document link already exists.");
        }
    }

    private async Task EnsureNoActiveClaimDuplicateAsync(
        string claimId,
        string documentId,
        CancellationToken cancellationToken)
    {
        var claimDocuments = await documentStorageService.GetClaimDocumentsAsync(claimId, cancellationToken);
        if (claimDocuments.Any(claimDocument =>
                claimDocument.DisabledAt is null
                && string.Equals(claimDocument.DocumentId, documentId, StringComparison.Ordinal)))
        {
            throw new InvalidOperationException("Active claim document link already exists.");
        }
    }

    private static string NormalizeRequiredValue(string value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Value is required.", parameterName);
        }

        return value.Trim();
    }
}
