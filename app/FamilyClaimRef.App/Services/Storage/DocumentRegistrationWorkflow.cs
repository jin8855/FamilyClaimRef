using System.Runtime.ExceptionServices;

namespace FamilyClaimRef.App.Services.Storage;

public sealed class DocumentRegistrationWorkflow
{
    private const string ClaimScope = "claim";
    private const string PolicyScope = "policy";

    private readonly DocumentAttachmentCoordinator attachmentCoordinator;
    private readonly DocumentLinkCoordinator linkCoordinator;
    private readonly IDocumentStorageService documentStorageService;
    private readonly IFileAttachmentService fileAttachmentService;

    public DocumentRegistrationWorkflow(
        DocumentAttachmentCoordinator attachmentCoordinator,
        DocumentLinkCoordinator linkCoordinator,
        IDocumentStorageService documentStorageService,
        IFileAttachmentService fileAttachmentService)
    {
        this.attachmentCoordinator = attachmentCoordinator
            ?? throw new ArgumentNullException(nameof(attachmentCoordinator));
        this.linkCoordinator = linkCoordinator
            ?? throw new ArgumentNullException(nameof(linkCoordinator));
        this.documentStorageService = documentStorageService
            ?? throw new ArgumentNullException(nameof(documentStorageService));
        this.fileAttachmentService = fileAttachmentService
            ?? throw new ArgumentNullException(nameof(fileAttachmentService));
    }

    public async Task<PolicyDocumentRegistrationResult> RegisterPolicyDocumentAsync(
        PolicyDocumentRegistrationRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        cancellationToken.ThrowIfCancellationRequested();

        var policyId = NormalizeRequiredValue(request.PolicyId, nameof(request.PolicyId));
        var attachment = await attachmentCoordinator.AttachDocumentAsync(
            new DocumentAttachmentRequest(
                request.SourceFilePath,
                PolicyScope,
                request.DocumentType,
                request.DisplayTitle,
                request.ReferenceDate),
            cancellationToken);

        try
        {
            var link = await linkCoordinator.LinkPolicyDocumentAsync(
                new PolicyDocumentLinkRequest(policyId, attachment.Document.Id, request.DocumentType),
                cancellationToken);

            return new PolicyDocumentRegistrationResult(attachment, link);
        }
        catch (Exception linkException)
        {
            await RollbackAttachmentAsync(attachment, linkException, cancellationToken);
            throw;
        }
    }

    public async Task<ClaimDocumentRegistrationResult> RegisterClaimDocumentAsync(
        ClaimDocumentRegistrationRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        cancellationToken.ThrowIfCancellationRequested();

        var claimId = NormalizeRequiredValue(request.ClaimId, nameof(request.ClaimId));
        var attachment = await attachmentCoordinator.AttachDocumentAsync(
            new DocumentAttachmentRequest(
                request.SourceFilePath,
                ClaimScope,
                request.DocumentType,
                request.DisplayTitle,
                request.ReferenceDate),
            cancellationToken);

        try
        {
            var link = await linkCoordinator.LinkClaimDocumentAsync(
                new ClaimDocumentLinkRequest(claimId, attachment.Document.Id, request.DocumentType),
                cancellationToken);

            return new ClaimDocumentRegistrationResult(attachment, link);
        }
        catch (Exception linkException)
        {
            await RollbackAttachmentAsync(attachment, linkException, cancellationToken);
            throw;
        }
    }

    private async Task RollbackAttachmentAsync(
        DocumentAttachmentResult attachment,
        Exception linkException,
        CancellationToken cancellationToken)
    {
        var rollbackFailures = new List<Exception>();

        try
        {
            await fileAttachmentService.DeleteDocumentFileIfExistsAsync(
                attachment.File.RelativePath,
                cancellationToken);
        }
        catch (Exception fileCleanupException)
        {
            rollbackFailures.Add(fileCleanupException);
        }

        try
        {
            await documentStorageService.DisableDocumentAsync(
                attachment.Document.Id,
                DateTimeOffset.UtcNow,
                cancellationToken);
        }
        catch (Exception documentDisableException)
        {
            rollbackFailures.Add(documentDisableException);
        }

        if (rollbackFailures.Count == 0)
        {
            ExceptionDispatchInfo.Capture(linkException).Throw();
        }

        var exceptions = new List<Exception> { linkException };
        exceptions.AddRange(rollbackFailures);

        throw new AggregateException(
            "Document registration failed and rollback also failed.",
            exceptions);
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
