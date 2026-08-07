using FamilyClaimRef.App.Models.Storage;
using FamilyClaimRef.App.Services.Storage;
using Xunit;

namespace FamilyClaimRef.App.Tests.Integration;

[Collection(RuntimeEnvironmentCollectionName.Value)]
public sealed class AttachmentDuplicateCollisionValidationTests
{
    private static readonly DateOnly ReferenceDate = new(2026, 7, 8);

    [Fact]
    public async Task RegisterPolicyDocumentAsync_RepeatedFilenameCollision_CreatesUniqueAttachmentsWithoutOverwrite()
    {
        await UsingTestContextAsync(async context =>
        {
            var policy = await context.PolicyClaimStorage.AddPolicyAsync(CreatePolicyDraft());
            var sourcePath = await CreateSyntheticInputFileAsync(
                context.InputRootPath,
                "synthetic_policy_collision_document.png",
                "first synthetic policy collision content");

            var first = await context.Workflow.RegisterPolicyDocumentAsync(new PolicyDocumentRegistrationRequest(
                sourcePath,
                policy.Id,
                "terms",
                "synthetic_policy_collision_document_demo",
                ReferenceDate));
            var firstAttachmentPath = GetAttachmentPath(context, first.Attachment.File.RelativePath);
            var firstAttachmentContent = await File.ReadAllTextAsync(firstAttachmentPath);

            await File.WriteAllTextAsync(sourcePath, "second synthetic policy collision content");

            var second = await context.Workflow.RegisterPolicyDocumentAsync(new PolicyDocumentRegistrationRequest(
                sourcePath,
                policy.Id,
                "terms",
                "synthetic_policy_collision_document_demo",
                ReferenceDate));
            var secondAttachmentPath = GetAttachmentPath(context, second.Attachment.File.RelativePath);

            Assert.NotEqual(first.Attachment.Document.Id, second.Attachment.Document.Id);
            Assert.NotEqual(first.Attachment.File.PhysicalFileName, second.Attachment.File.PhysicalFileName);
            Assert.EndsWith("_001.png", first.Attachment.File.PhysicalFileName, StringComparison.Ordinal);
            Assert.EndsWith("_002.png", second.Attachment.File.PhysicalFileName, StringComparison.Ordinal);
            Assert.Equal("first synthetic policy collision content", firstAttachmentContent);
            Assert.Equal(
                "first synthetic policy collision content",
                await File.ReadAllTextAsync(firstAttachmentPath));
            Assert.Equal(
                "second synthetic policy collision content",
                await File.ReadAllTextAsync(secondAttachmentPath));
            Assert.Equal(2, SnapshotFiles(Path.Combine(context.AttachmentRootPath, "documents")).Length);
            Assert.Equal(2, (await context.DocumentStorage.GetDocumentsAsync()).Count);
            var policyLinks = await context.DocumentStorage.GetPolicyDocumentsAsync(policy.Id);
            Assert.Equal(2, policyLinks.Count);
            Assert.Single(policyLinks, link => link.DisabledAt is null);
            Assert.Single(policyLinks, link => link.DisabledAt is not null);
        });
    }

    [Fact]
    public async Task RegisterClaimDocumentAsync_RepeatedFilenameCollision_CreatesUniqueAttachmentsWithoutOverwrite()
    {
        await UsingTestContextAsync(async context =>
        {
            var policy = await context.PolicyClaimStorage.AddPolicyAsync(CreatePolicyDraft());
            var claim = await context.PolicyClaimStorage.AddClaimAsync(CreateClaimDraft(policy.Id));
            var sourcePath = await CreateSyntheticInputFileAsync(
                context.InputRootPath,
                "synthetic_claim_collision_document.png",
                "first synthetic claim collision content");

            var first = await context.Workflow.RegisterClaimDocumentAsync(new ClaimDocumentRegistrationRequest(
                sourcePath,
                claim.Id,
                "receipt",
                "synthetic_claim_collision_document_demo",
                ReferenceDate));
            var firstAttachmentPath = GetAttachmentPath(context, first.Attachment.File.RelativePath);
            var firstAttachmentContent = await File.ReadAllTextAsync(firstAttachmentPath);

            await File.WriteAllTextAsync(sourcePath, "second synthetic claim collision content");

            var second = await context.Workflow.RegisterClaimDocumentAsync(new ClaimDocumentRegistrationRequest(
                sourcePath,
                claim.Id,
                "receipt",
                "synthetic_claim_collision_document_demo",
                ReferenceDate));
            var secondAttachmentPath = GetAttachmentPath(context, second.Attachment.File.RelativePath);

            Assert.NotEqual(first.Attachment.Document.Id, second.Attachment.Document.Id);
            Assert.NotEqual(first.Attachment.File.PhysicalFileName, second.Attachment.File.PhysicalFileName);
            Assert.EndsWith("_001.png", first.Attachment.File.PhysicalFileName, StringComparison.Ordinal);
            Assert.EndsWith("_002.png", second.Attachment.File.PhysicalFileName, StringComparison.Ordinal);
            Assert.Equal("first synthetic claim collision content", firstAttachmentContent);
            Assert.Equal(
                "first synthetic claim collision content",
                await File.ReadAllTextAsync(firstAttachmentPath));
            Assert.Equal(
                "second synthetic claim collision content",
                await File.ReadAllTextAsync(secondAttachmentPath));
            Assert.Equal(2, SnapshotFiles(Path.Combine(context.AttachmentRootPath, "documents")).Length);
            Assert.Equal(2, (await context.DocumentStorage.GetDocumentsAsync()).Count);
            Assert.Equal(2, (await context.DocumentStorage.GetClaimDocumentsAsync(claim.Id)).Count);
        });
    }

    [Fact]
    public async Task LinkPolicyDocumentAsync_DuplicateActiveLink_IsRejectedWithoutExtraActiveLink()
    {
        await UsingTestContextAsync(async context =>
        {
            var policy = await context.PolicyClaimStorage.AddPolicyAsync(CreatePolicyDraft());
            var document = await context.DocumentStorage.AddDocumentAsync(CreateDocumentDraft("policy_duplicate_link.png"));
            var linkCoordinator = new DocumentLinkCoordinator(
                context.DocumentStorage,
                context.PolicyClaimStorage);

            await linkCoordinator.LinkPolicyDocumentAsync(new PolicyDocumentLinkRequest(
                policy.Id,
                document.Id,
                "terms"));

            var exception = await Record.ExceptionAsync(() =>
                linkCoordinator.LinkPolicyDocumentAsync(new PolicyDocumentLinkRequest(
                    policy.Id,
                    document.Id,
                    "capture")));

            Assert.IsType<InvalidOperationException>(exception);
            var policyLinks = await context.DocumentStorage.GetPolicyDocumentsAsync(policy.Id);
            Assert.Single(policyLinks);
            Assert.All(policyLinks, link => Assert.Null(link.DisabledAt));
        });
    }

    [Fact]
    public async Task LinkClaimDocumentAsync_DuplicateActiveLink_IsRejectedWithoutExtraActiveLink()
    {
        await UsingTestContextAsync(async context =>
        {
            var policy = await context.PolicyClaimStorage.AddPolicyAsync(CreatePolicyDraft());
            var claim = await context.PolicyClaimStorage.AddClaimAsync(CreateClaimDraft(policy.Id));
            var document = await context.DocumentStorage.AddDocumentAsync(CreateDocumentDraft("claim_duplicate_link.png"));
            var linkCoordinator = new DocumentLinkCoordinator(
                context.DocumentStorage,
                context.PolicyClaimStorage);

            await linkCoordinator.LinkClaimDocumentAsync(new ClaimDocumentLinkRequest(
                claim.Id,
                document.Id,
                "receipt"));

            var exception = await Record.ExceptionAsync(() =>
                linkCoordinator.LinkClaimDocumentAsync(new ClaimDocumentLinkRequest(
                    claim.Id,
                    document.Id,
                    "medicine")));

            Assert.IsType<InvalidOperationException>(exception);
            var claimLinks = await context.DocumentStorage.GetClaimDocumentsAsync(claim.Id);
            Assert.Single(claimLinks);
            Assert.All(claimLinks, link => Assert.Null(link.DisabledAt));
        });
    }

    [Fact]
    public async Task AttachDocumentAsync_WhenAllDuplicateIndexesCollide_RejectsWithoutCopy()
    {
        await UsingTestContextAsync(async context =>
        {
            var sourcePath = await CreateSyntheticInputFileAsync(
                context.InputRootPath,
                "synthetic_duplicate_index_exhaustion.png",
                "synthetic duplicate index exhaustion content");
            var fileAttachment = new AllTargetsExistingAttachmentService();
            var coordinator = new DocumentAttachmentCoordinator(
                context.DocumentStorage,
                fileAttachment);

            var exception = await Record.ExceptionAsync(() =>
                coordinator.AttachDocumentAsync(new DocumentAttachmentRequest(
                    sourcePath,
                    "policy",
                    "terms",
                    "synthetic_duplicate_index_exhaustion_demo",
                    ReferenceDate)));

            Assert.IsType<InvalidOperationException>(exception);
            Assert.Equal(999, fileAttachment.ExistsCallCount);
            Assert.Equal(0, fileAttachment.CopyCallCount);
            Assert.Empty(await context.DocumentStorage.GetDocumentsAsync());
            Assert.Empty(SnapshotFiles(context.AttachmentRootPath));
        });
    }

    private static async Task UsingTestContextAsync(Func<TestContext, Task> action)
    {
        var projectRoot = FindProjectRoot();
        var projectAttachmentsBefore = SnapshotFiles(Path.Combine(projectRoot, "attachments"));
        var projectDataLocalBefore = SnapshotFiles(Path.Combine(projectRoot, "data", "local"));
        var projectRuntimeTestFilesBefore = SnapshotFiles(projectRoot, "runtime_test_document.*");
        var testRunRoot = Path.Combine(
            Path.GetTempPath(),
            "FamilyClaimRef-TestRuns",
            $"attachment-duplicate-collision-{Guid.NewGuid():N}");
        var inputRootPath = Path.Combine(testRunRoot, "input");
        var runtimeRootPath = Path.Combine(testRunRoot, "runtime");
        var metadataRootPath = Path.Combine(runtimeRootPath, "data", "local");
        var attachmentRootPath = Path.Combine(runtimeRootPath, "attachments");

        Directory.CreateDirectory(inputRootPath);

        var documentStorage = new JsonDocumentStorageService(metadataRootPath);
        var policyClaimStorage = new JsonPolicyClaimStorageService(metadataRootPath);
        var fileAttachmentService = new LocalFileAttachmentService(attachmentRootPath);
        var attachmentCoordinator = new DocumentAttachmentCoordinator(
            documentStorage,
            fileAttachmentService);
        var linkCoordinator = new DocumentLinkCoordinator(
            documentStorage,
            policyClaimStorage);
        var workflow = new DocumentRegistrationWorkflow(
            attachmentCoordinator,
            linkCoordinator,
            documentStorage,
            fileAttachmentService);
        var context = new TestContext(
            inputRootPath,
            metadataRootPath,
            attachmentRootPath,
            documentStorage,
            policyClaimStorage,
            workflow);

        try
        {
            await action(context);
        }
        finally
        {
            DeleteTestRunRoot(testRunRoot);
        }

        var projectAttachmentsAfter = SnapshotFiles(Path.Combine(projectRoot, "attachments"));
        var projectDataLocalAfter = SnapshotFiles(Path.Combine(projectRoot, "data", "local"));
        var projectRuntimeTestFilesAfter = SnapshotFiles(projectRoot, "runtime_test_document.*");

        Assert.Equal(projectAttachmentsBefore, projectAttachmentsAfter);
        Assert.Equal(projectDataLocalBefore, projectDataLocalAfter);
        Assert.Equal(projectRuntimeTestFilesBefore, projectRuntimeTestFilesAfter);
    }

    private static PolicyDraft CreatePolicyDraft()
    {
        return new PolicyDraft(
            "policy_title_duplicate_collision_demo",
            ReferenceDate);
    }

    private static ClaimDraft CreateClaimDraft(string policyId)
    {
        return new ClaimDraft(
            policyId,
            "claim_title_duplicate_collision_demo",
            ReferenceDate);
    }

    private static DocumentDraft CreateDocumentDraft(string fileName)
    {
        return new DocumentDraft(
            fileName,
            "synthetic_duplicate_link_document_demo",
            "png",
            $"documents/{fileName}");
    }

    private static async Task<string> CreateSyntheticInputFileAsync(
        string inputRootPath,
        string fileName,
        string content)
    {
        Directory.CreateDirectory(inputRootPath);
        var sourcePath = Path.Combine(inputRootPath, fileName);
        await File.WriteAllTextAsync(sourcePath, content);

        return sourcePath;
    }

    private static string GetAttachmentPath(TestContext context, string relativePath)
    {
        var attachmentPath = Path.GetFullPath(Path.Combine(
            context.AttachmentRootPath,
            relativePath.Replace('/', Path.DirectorySeparatorChar)));

        Assert.True(IsUnderDirectory(context.AttachmentRootPath, attachmentPath));

        return attachmentPath;
    }

    private static string FindProjectRoot()
    {
        var currentDirectory = new DirectoryInfo(AppContext.BaseDirectory);
        while (currentDirectory is not null)
        {
            if (File.Exists(Path.Combine(currentDirectory.FullName, "FamilyClaimRef.sln")))
            {
                return currentDirectory.FullName;
            }

            currentDirectory = currentDirectory.Parent;
        }

        throw new InvalidOperationException("Project root was not found.");
    }

    private static string[] SnapshotFiles(string directoryPath, string searchPattern = "*")
    {
        if (!Directory.Exists(directoryPath))
        {
            return [];
        }

        return Directory
            .GetFiles(directoryPath, searchPattern, SearchOption.AllDirectories)
            .Select(path => Path.GetRelativePath(directoryPath, path))
            .Order(StringComparer.Ordinal)
            .ToArray();
    }

    private static bool IsUnderDirectory(string parentPath, string childPath)
    {
        var parentFullPath = EnsureTrailingSeparator(Path.GetFullPath(parentPath));
        var childFullPath = Path.GetFullPath(childPath);

        return childFullPath.StartsWith(parentFullPath, StringComparison.OrdinalIgnoreCase);
    }

    private static string EnsureTrailingSeparator(string path)
    {
        if (path.EndsWith(Path.DirectorySeparatorChar)
            || path.EndsWith(Path.AltDirectorySeparatorChar))
        {
            return path;
        }

        return path + Path.DirectorySeparatorChar;
    }

    private static void DeleteTestRunRoot(string testRunRoot)
    {
        var allowedParent = Path.Combine(Path.GetTempPath(), "FamilyClaimRef-TestRuns");
        if (!IsUnderDirectory(allowedParent, testRunRoot))
        {
            throw new InvalidOperationException("Test cleanup path is outside the allowed test temp root.");
        }

        if (Directory.Exists(testRunRoot))
        {
            Directory.Delete(testRunRoot, recursive: true);
        }
    }

    private sealed record TestContext(
        string InputRootPath,
        string MetadataRootPath,
        string AttachmentRootPath,
        JsonDocumentStorageService DocumentStorage,
        JsonPolicyClaimStorageService PolicyClaimStorage,
        DocumentRegistrationWorkflow Workflow);

    private sealed class AllTargetsExistingAttachmentService : IFileAttachmentService
    {
        public int ExistsCallCount { get; private set; }

        public int CopyCallCount { get; private set; }

        public Task<FileAttachmentCopyResult> CopyDocumentFileAsync(
            string sourceFilePath,
            string physicalFileName,
            CancellationToken cancellationToken = default)
        {
            CopyCallCount++;

            throw new InvalidOperationException("Copy should not be called when every target exists.");
        }

        public Task DeleteDocumentFileIfExistsAsync(
            string relativePath,
            CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public Task<bool> DocumentFileExistsAsync(
            string relativePath,
            CancellationToken cancellationToken = default)
        {
            ExistsCallCount++;

            return Task.FromResult(true);
        }
    }
}
