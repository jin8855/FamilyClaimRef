using FamilyClaimRef.App.Models.Storage;
using FamilyClaimRef.App.Services.Storage;
using Xunit;

namespace FamilyClaimRef.App.Tests.Integration;

[Collection(RuntimeEnvironmentCollectionName.Value)]
public sealed class DocumentRegistrationNegativeValidationTests
{
    private static readonly DateOnly ReferenceDate = new(2026, 7, 8);

    [Fact]
    public async Task RegisterPolicyDocumentAsync_MissingSourceFilePath_DoesNotCreateDocumentOrAttachment()
    {
        await UsingTestContextAsync(async context =>
        {
            var policy = await context.PolicyClaimStorage.AddPolicyAsync(CreatePolicyDraft());

            var exception = await Record.ExceptionAsync(() =>
                context.Workflow.RegisterPolicyDocumentAsync(new PolicyDocumentRegistrationRequest(
                    " ",
                    policy.Id,
                    "terms",
                    "synthetic_policy_document_negative_demo",
                    ReferenceDate)));

            Assert.IsType<ArgumentException>(exception);
            await AssertNoDocumentMetadataOrAttachmentsAsync(context);
            await AssertNoPolicyLinksAsync(context, policy.Id);
        });
    }

    [Fact]
    public async Task RegisterPolicyDocumentAsync_NonexistentSourceFile_DoesNotCreateDocumentLinkOrAttachment()
    {
        await UsingTestContextAsync(async context =>
        {
            var policy = await context.PolicyClaimStorage.AddPolicyAsync(CreatePolicyDraft());
            var missingSourcePath = Path.Combine(context.InputRootPath, "missing_source.png");

            var exception = await Record.ExceptionAsync(() =>
                context.Workflow.RegisterPolicyDocumentAsync(new PolicyDocumentRegistrationRequest(
                    missingSourcePath,
                    policy.Id,
                    "terms",
                    "synthetic_policy_document_negative_demo",
                    ReferenceDate)));

            Assert.IsType<FileNotFoundException>(exception);
            await AssertNoDocumentMetadataOrAttachmentsAsync(context);
            await AssertNoPolicyLinksAsync(context, policy.Id);
        });
    }

    [Fact]
    public async Task RegisterPolicyDocumentAsync_UnsupportedExtension_DoesNotCreateDocumentLinkOrAttachment()
    {
        await UsingTestContextAsync(async context =>
        {
            var policy = await context.PolicyClaimStorage.AddPolicyAsync(CreatePolicyDraft());
            var sourcePath = await CreateSyntheticInputFileAsync(
                context.InputRootPath,
                "unsupported_extension.txt");

            var exception = await Record.ExceptionAsync(() =>
                context.Workflow.RegisterPolicyDocumentAsync(new PolicyDocumentRegistrationRequest(
                    sourcePath,
                    policy.Id,
                    "terms",
                    "synthetic_policy_document_negative_demo",
                    ReferenceDate)));

            Assert.IsType<ArgumentException>(exception);
            await AssertNoDocumentMetadataOrAttachmentsAsync(context);
            await AssertNoPolicyLinksAsync(context, policy.Id);
        });
    }

    [Fact]
    public async Task RegisterPolicyDocumentAsync_UnsupportedDocumentType_DoesNotCreateDocumentLinkOrAttachment()
    {
        await UsingTestContextAsync(async context =>
        {
            var policy = await context.PolicyClaimStorage.AddPolicyAsync(CreatePolicyDraft());
            var sourcePath = await CreateSyntheticInputFileAsync(
                context.InputRootPath,
                "unsupported_document_type.png");

            var exception = await Record.ExceptionAsync(() =>
                context.Workflow.RegisterPolicyDocumentAsync(new PolicyDocumentRegistrationRequest(
                    sourcePath,
                    policy.Id,
                    "receipt",
                    "synthetic_policy_document_negative_demo",
                    ReferenceDate)));

            Assert.IsType<ArgumentException>(exception);
            await AssertNoDocumentMetadataOrAttachmentsAsync(context);
            await AssertNoPolicyLinksAsync(context, policy.Id);
        });
    }

    [Theory]
    [InlineData("policy")]
    [InlineData("claim")]
    public async Task RegisterDocumentAsync_MissingTargetId_DoesNotCreateDocumentOrAttachment(string targetScope)
    {
        await UsingTestContextAsync(async context =>
        {
            var sourcePath = await CreateSyntheticInputFileAsync(
                context.InputRootPath,
                $"missing_{targetScope}_target.png");

            var exception = targetScope == "policy"
                ? await Record.ExceptionAsync(() =>
                    context.Workflow.RegisterPolicyDocumentAsync(new PolicyDocumentRegistrationRequest(
                        sourcePath,
                        " ",
                        "terms",
                        "synthetic_policy_document_negative_demo",
                        ReferenceDate)))
                : await Record.ExceptionAsync(() =>
                    context.Workflow.RegisterClaimDocumentAsync(new ClaimDocumentRegistrationRequest(
                        sourcePath,
                        " ",
                        "receipt",
                        "synthetic_claim_document_negative_demo",
                        ReferenceDate)));

            Assert.IsType<ArgumentException>(exception);
            await AssertNoDocumentMetadataOrAttachmentsAsync(context);
        });
    }

    [Fact]
    public async Task RegisterPolicyDocumentAsync_DisabledPolicyTarget_RollsBackAttachmentAndCreatesNoActiveLink()
    {
        await UsingTestContextAsync(async context =>
        {
            var policy = await context.PolicyClaimStorage.AddPolicyAsync(CreatePolicyDraft());
            await context.PolicyClaimStorage.DisablePolicyAsync(policy.Id);
            var sourcePath = await CreateSyntheticInputFileAsync(
                context.InputRootPath,
                "disabled_policy_target.png");

            var exception = await Record.ExceptionAsync(() =>
                context.Workflow.RegisterPolicyDocumentAsync(new PolicyDocumentRegistrationRequest(
                    sourcePath,
                    policy.Id,
                    "terms",
                    "synthetic_policy_document_negative_demo",
                    ReferenceDate)));

            Assert.IsType<InvalidOperationException>(exception);
            await AssertSingleDisabledTransientDocumentAsync(context);
            await AssertNoPolicyLinksAsync(context, policy.Id);
            Assert.Empty(SnapshotFiles(context.AttachmentRootPath));
        });
    }

    [Fact]
    public async Task RegisterClaimDocumentAsync_DisabledClaimTarget_RollsBackAttachmentAndCreatesNoActiveLink()
    {
        await UsingTestContextAsync(async context =>
        {
            var policy = await context.PolicyClaimStorage.AddPolicyAsync(CreatePolicyDraft());
            var claim = await context.PolicyClaimStorage.AddClaimAsync(CreateClaimDraft(policy.Id));
            await context.PolicyClaimStorage.DisableClaimAsync(claim.Id, claim.Revision);
            var sourcePath = await CreateSyntheticInputFileAsync(
                context.InputRootPath,
                "disabled_claim_target.png");

            var exception = await Record.ExceptionAsync(() =>
                context.Workflow.RegisterClaimDocumentAsync(new ClaimDocumentRegistrationRequest(
                    sourcePath,
                    claim.Id,
                    "receipt",
                    "synthetic_claim_document_negative_demo",
                    ReferenceDate)));

            Assert.IsType<InvalidOperationException>(exception);
            await AssertSingleDisabledTransientDocumentAsync(context);
            await AssertNoClaimLinksAsync(context, claim.Id);
            Assert.Empty(SnapshotFiles(context.AttachmentRootPath));
        });
    }

    [Fact]
    public async Task Gate8_source_removed_after_selection_creates_no_metadata_link_or_attachment()
    {
        await UsingTestContextAsync(async context =>
        {
            var policy = await context.PolicyClaimStorage.AddPolicyAsync(CreatePolicyDraft());
            var sourcePath = Path.Combine(context.InputRootPath, "removed-after-selection.png");
            await File.WriteAllBytesAsync(
                sourcePath,
                [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, 0x01]);
            var snapshot = await new DocumentFileValidationService().ValidateSourceAsync(sourcePath);
            File.Delete(sourcePath);

            var exception = await Record.ExceptionAsync(() =>
                context.Workflow.RegisterPolicyDocumentAsync(new PolicyDocumentRegistrationRequest(
                    sourcePath,
                    policy.Id,
                    "terms",
                    "Synthetic removed source",
                    ReferenceDate,
                    snapshot)));

            var registrationException =
                Assert.IsType<DocumentRegistrationException>(exception);
            Assert.Equal(
                DocumentRegistrationErrorCode.SourceUnavailable,
                registrationException.ErrorCode);
            await AssertNoDocumentMetadataOrAttachmentsAsync(context);
            await AssertNoPolicyLinksAsync(context, policy.Id);
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
            $"document-registration-negative-{Guid.NewGuid():N}");
        var inputRootPath = Path.Combine(testRunRoot, "input");
        var runtimeRootPath = Path.Combine(testRunRoot, "runtime");
        var metadataRootPath = Path.Combine(runtimeRootPath, "data", "local");
        var attachmentRootPath = Path.Combine(runtimeRootPath, "attachments");

        Directory.CreateDirectory(inputRootPath);

        var documentStorage = new JsonDocumentStorageService(metadataRootPath);
        var policyClaimStorage = new JsonPolicyClaimStorageService(metadataRootPath);
        var fileAttachmentService = new LocalFileAttachmentService(attachmentRootPath);
        var validationService = new DocumentFileValidationService();
        var attachmentCoordinator = new DocumentAttachmentCoordinator(
            documentStorage,
            fileAttachmentService,
            validationService);
        var linkCoordinator = new DocumentLinkCoordinator(
            documentStorage,
            policyClaimStorage);
        var workflow = new DocumentRegistrationWorkflow(
            attachmentCoordinator,
            linkCoordinator,
            documentStorage,
            fileAttachmentService,
            policyClaimStorage);
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
            "policy_title_document_negative_demo",
            ReferenceDate);
    }

    private static ClaimDraft CreateClaimDraft(string policyId)
    {
        return new ClaimDraft(
            policyId,
            "claim_title_document_negative_demo",
            ReferenceDate);
    }

    private static async Task<string> CreateSyntheticInputFileAsync(
        string inputRootPath,
        string fileName)
    {
        Directory.CreateDirectory(inputRootPath);
        var sourcePath = Path.Combine(inputRootPath, fileName);
        await File.WriteAllTextAsync(
            sourcePath,
            "FamilyClaimRef automated negative validation synthetic document.");

        return sourcePath;
    }

    private static async Task AssertNoDocumentMetadataOrAttachmentsAsync(TestContext context)
    {
        Assert.Empty(await context.DocumentStorage.GetDocumentsAsync());
        Assert.Empty(SnapshotFiles(context.AttachmentRootPath));
    }

    private static async Task AssertSingleDisabledTransientDocumentAsync(TestContext context)
    {
        var document = Assert.Single(await context.DocumentStorage.GetDocumentsAsync());

        Assert.NotNull(document.DisabledAt);
        Assert.Empty(SnapshotFiles(context.AttachmentRootPath));
    }

    private static async Task AssertNoPolicyLinksAsync(TestContext context, string policyId)
    {
        Assert.Empty(await context.DocumentStorage.GetPolicyDocumentsAsync(policyId));
    }

    private static async Task AssertNoClaimLinksAsync(TestContext context, string claimId)
    {
        Assert.Empty(await context.DocumentStorage.GetClaimDocumentsAsync(claimId));
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
}
