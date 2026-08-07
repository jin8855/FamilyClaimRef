using System.Runtime.ExceptionServices;

namespace FamilyClaimRef.App.Services.Storage;

public sealed class DocumentRegistrationWorkflow
{
    private const string ClaimScope = "claim";
    private const string PolicyScope = "policy";
    private static readonly SemaphoreSlim RegistrationCriticalSection = new(1, 1);

    private readonly DocumentAttachmentCoordinator attachmentCoordinator;
    private readonly DocumentLinkCoordinator linkCoordinator;
    private readonly IDocumentStorageService documentStorageService;
    private readonly IFileAttachmentService fileAttachmentService;
    private readonly IPolicyClaimStorageService? policyClaimStorageService;

    public DocumentRegistrationWorkflow(
        DocumentAttachmentCoordinator attachmentCoordinator,
        DocumentLinkCoordinator linkCoordinator,
        IDocumentStorageService documentStorageService,
        IFileAttachmentService fileAttachmentService)
        : this(
            attachmentCoordinator,
            linkCoordinator,
            documentStorageService,
            fileAttachmentService,
            null)
    {
    }

    public DocumentRegistrationWorkflow(
        DocumentAttachmentCoordinator attachmentCoordinator,
        DocumentLinkCoordinator linkCoordinator,
        IDocumentStorageService documentStorageService,
        IFileAttachmentService fileAttachmentService,
        IPolicyClaimStorageService? policyClaimStorageService)
    {
        this.attachmentCoordinator = attachmentCoordinator
            ?? throw new ArgumentNullException(nameof(attachmentCoordinator));
        this.linkCoordinator = linkCoordinator
            ?? throw new ArgumentNullException(nameof(linkCoordinator));
        this.documentStorageService = documentStorageService
            ?? throw new ArgumentNullException(nameof(documentStorageService));
        this.fileAttachmentService = fileAttachmentService
            ?? throw new ArgumentNullException(nameof(fileAttachmentService));
        this.policyClaimStorageService = policyClaimStorageService;
    }

    public async Task<PolicyDocumentRegistrationResult> RegisterPolicyDocumentAsync(
        PolicyDocumentRegistrationRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        cancellationToken.ThrowIfCancellationRequested();

        var policyId = NormalizeRequiredValue(request.PolicyId, nameof(request.PolicyId));
        if (request.SelectionSnapshot is not null)
        {
            return await RegisterGate8Async(
                PolicyScope,
                policyId,
                new DocumentAttachmentRequest(
                    request.SourceFilePath,
                    PolicyScope,
                    request.DocumentType,
                    request.DisplayTitle,
                    request.ReferenceDate,
                    request.SelectionSnapshot),
                async (attachment, finalizationToken) =>
                {
                    var link = await linkCoordinator.ReplacePolicyDocumentAsync(
                        new PolicyDocumentLinkRequest(policyId, attachment.Document.Id, request.DocumentType),
                        finalizationToken);
                    return new PolicyDocumentRegistrationResult(attachment, link);
                },
                cancellationToken);
        }

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
            var link = await linkCoordinator.ReplacePolicyDocumentAsync(
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
        if (request.SelectionSnapshot is not null)
        {
            return await RegisterGate8Async(
                ClaimScope,
                claimId,
                new DocumentAttachmentRequest(
                    request.SourceFilePath,
                    ClaimScope,
                    request.DocumentType,
                    request.DisplayTitle,
                    request.ReferenceDate,
                    request.SelectionSnapshot),
                async (attachment, finalizationToken) =>
                {
                    var link = await linkCoordinator.LinkClaimDocumentAsync(
                        new ClaimDocumentLinkRequest(claimId, attachment.Document.Id, request.DocumentType),
                        finalizationToken);
                    return new ClaimDocumentRegistrationResult(attachment, link);
                },
                cancellationToken);
        }

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

    private async Task<TResult> RegisterGate8Async<TResult>(
        string targetKind,
        string targetId,
        DocumentAttachmentRequest attachmentRequest,
        Func<DocumentAttachmentResult, CancellationToken, Task<TResult>> createLinkAndResult,
        CancellationToken cancellationToken)
    {
        if (policyClaimStorageService is null)
        {
            throw new InvalidOperationException(
                "Gate8 registration requires policy and claim storage composition.");
        }

        var stagedFile = await attachmentCoordinator.StageDocumentAsync(
            attachmentRequest,
            cancellationToken);
        var finalPayloadCreated = false;
        var criticalSectionEntered = false;

        try
        {
            await RegistrationCriticalSection.WaitAsync(cancellationToken);
            criticalSectionEntered = true;
            cancellationToken.ThrowIfCancellationRequested();

            if (!await TargetExistsAsync(targetKind, targetId, CancellationToken.None))
            {
                throw new DocumentRegistrationException(
                    DocumentRegistrationErrorCode.TargetUnavailable,
                    "Registration target is not active.");
            }

            var stagedSha256 = stagedFile.Validation?.Sha256
                ?? throw new InvalidOperationException("Staged SHA-256 is required.");
            if (await documentStorageService.ActiveTargetDocumentWithSha256ExistsAsync(
                    targetKind,
                    targetId,
                    stagedSha256,
                    CancellationToken.None))
            {
                throw new DocumentRegistrationException(
                    DocumentRegistrationErrorCode.DuplicateDocument,
                    "An active registration already contains the same payload.");
            }

            var attachment = await attachmentCoordinator.FinalizeStagedDocumentAsync(
                attachmentRequest,
                stagedFile,
                CancellationToken.None);
            finalPayloadCreated = true;

            try
            {
                return await createLinkAndResult(attachment, CancellationToken.None);
            }
            catch (Exception linkException)
            {
                await RollbackAttachmentAsync(
                    attachment,
                    linkException,
                    CancellationToken.None);
                throw;
            }
        }
        catch (Exception registrationException) when (!finalPayloadCreated)
        {
            await CleanupStagedFileAsync(stagedFile, registrationException);
            throw;
        }
        finally
        {
            if (criticalSectionEntered)
            {
                RegistrationCriticalSection.Release();
            }
        }
    }

    private Task<bool> TargetExistsAsync(
        string targetKind,
        string targetId,
        CancellationToken cancellationToken)
    {
        return string.Equals(targetKind, PolicyScope, StringComparison.Ordinal)
            ? policyClaimStorageService!.PolicyExistsAsync(targetId, cancellationToken)
            : policyClaimStorageService!.ClaimExistsAsync(targetId, cancellationToken);
    }

    private async Task CleanupStagedFileAsync(
        StagedFileAttachment stagedFile,
        Exception registrationException)
    {
        try
        {
            await attachmentCoordinator.DeleteStagedFileIfExistsAsync(
                stagedFile,
                CancellationToken.None);
        }
        catch (Exception cleanupException)
        {
            throw new AggregateException(
                "Document registration failed and staging cleanup also failed.",
                registrationException,
                cleanupException);
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
