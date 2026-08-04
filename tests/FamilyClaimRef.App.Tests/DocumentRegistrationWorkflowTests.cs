using FamilyClaimRef.App.Models.Storage;
using FamilyClaimRef.App.Services.Storage;
using Xunit;

namespace FamilyClaimRef.App.Tests;

public sealed class DocumentRegistrationWorkflowTests
{
    [Fact]
    public void Constructor_rejects_null_attachment_coordinator()
    {
        var exception = Record.Exception(() => new DocumentRegistrationWorkflow(
            null!,
            CreateLinkCoordinator(new SpyDocumentStorageService()),
            new SpyDocumentStorageService(),
            new SpyFileAttachmentService()));

        Assert.NotNull(exception);
        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void Constructor_rejects_null_link_coordinator()
    {
        var storage = new SpyDocumentStorageService();

        var exception = Record.Exception(() => new DocumentRegistrationWorkflow(
            CreateAttachmentCoordinator(storage, new SpyFileAttachmentService()),
            null!,
            storage,
            new SpyFileAttachmentService()));

        Assert.NotNull(exception);
        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void Constructor_rejects_null_document_storage_service()
    {
        var storage = new SpyDocumentStorageService();

        var exception = Record.Exception(() => new DocumentRegistrationWorkflow(
            CreateAttachmentCoordinator(storage, new SpyFileAttachmentService()),
            CreateLinkCoordinator(storage),
            null!,
            new SpyFileAttachmentService()));

        Assert.NotNull(exception);
        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void Constructor_rejects_null_file_attachment_service()
    {
        var storage = new SpyDocumentStorageService();

        var exception = Record.Exception(() => new DocumentRegistrationWorkflow(
            CreateAttachmentCoordinator(storage, new SpyFileAttachmentService()),
            CreateLinkCoordinator(storage),
            storage,
            null!));

        Assert.NotNull(exception);
        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public async Task Gate8_registration_requires_target_storage_composition_before_staging()
    {
        var storage = new SpyDocumentStorageService();
        var fileAttachment = new SpyFileAttachmentService();
        var workflow = CreateWorkflow(storage, fileAttachment);
        var snapshot = new DocumentFileValidationResult(
            "synthetic.pdf",
            "pdf",
            "PDF",
            9,
            new string('a', 64),
            DateTimeOffset.UtcNow);

        var exception = await Record.ExceptionAsync(() =>
            workflow.RegisterPolicyDocumentAsync(new PolicyDocumentRegistrationRequest(
                "synthetic.pdf",
                "policy_001",
                "terms",
                "Synthetic",
                new DateOnly(2026, 7, 24),
                snapshot)));

        Assert.IsType<InvalidOperationException>(exception);
        Assert.False(fileAttachment.CopyCalled);
    }

    [Fact]
    public async Task RegisterPolicyDocumentAsync_registers_attachment_and_policy_link()
    {
        await UsingTempRootsAsync(async (metadataRoot, attachmentRoot) =>
        {
            var storage = new JsonDocumentStorageService(metadataRoot);
            var workflow = CreateWorkflow(storage, new LocalFileAttachmentService(attachmentRoot));
            var sourcePath = await CreateDummySourceFileAsync(attachmentRoot, "source.pdf", "dummy content");

            var result = await workflow.RegisterPolicyDocumentAsync(new PolicyDocumentRegistrationRequest(
                sourcePath,
                "policy_001",
                "terms",
                "Document A",
                new DateOnly(2026, 7, 1)));

            Assert.NotNull(result.Attachment);
            Assert.NotNull(result.Link);
            Assert.Equal(result.Attachment.Document.Id, result.Link.PolicyDocument.DocumentId);
            Assert.Equal("policy_001", result.Link.PolicyDocument.PolicyId);
            Assert.Equal("terms", result.Link.PolicyDocument.DocumentType);
            Assert.False(Path.IsPathRooted(result.Attachment.File.RelativePath));
            Assert.False(Path.IsPathRooted(result.Attachment.Document.RelativePath));
            Assert.True(File.Exists(Path.Combine(attachmentRoot, result.Attachment.File.RelativePath)));
            Assert.True(File.Exists(Path.Combine(metadataRoot, "documents.json")));
            Assert.True(File.Exists(Path.Combine(metadataRoot, "policy-documents.json")));
        });
    }

    [Fact]
    public async Task RegisterClaimDocumentAsync_registers_attachment_and_claim_link()
    {
        await UsingTempRootsAsync(async (metadataRoot, attachmentRoot) =>
        {
            var storage = new JsonDocumentStorageService(metadataRoot);
            var workflow = CreateWorkflow(storage, new LocalFileAttachmentService(attachmentRoot));
            var sourcePath = await CreateDummySourceFileAsync(attachmentRoot, "source.png", "dummy content");

            var result = await workflow.RegisterClaimDocumentAsync(new ClaimDocumentRegistrationRequest(
                sourcePath,
                "claim_001",
                "receipt",
                "Document A",
                new DateOnly(2026, 7, 1)));

            Assert.NotNull(result.Attachment);
            Assert.NotNull(result.Link);
            Assert.Equal(result.Attachment.Document.Id, result.Link.ClaimDocument.DocumentId);
            Assert.Equal("claim_001", result.Link.ClaimDocument.ClaimId);
            Assert.Equal("receipt", result.Link.ClaimDocument.DocumentType);
            Assert.False(Path.IsPathRooted(result.Attachment.File.RelativePath));
            Assert.False(Path.IsPathRooted(result.Attachment.Document.RelativePath));
            Assert.True(File.Exists(Path.Combine(attachmentRoot, result.Attachment.File.RelativePath)));
            Assert.True(File.Exists(Path.Combine(metadataRoot, "documents.json")));
            Assert.True(File.Exists(Path.Combine(metadataRoot, "claim-documents.json")));
        });
    }

    [Fact]
    public async Task RegisterPolicyDocumentAsync_rejects_null_request()
    {
        var storage = new SpyDocumentStorageService();
        var workflow = CreateWorkflow(storage, new SpyFileAttachmentService());

        var exception = await Record.ExceptionAsync(() => workflow.RegisterPolicyDocumentAsync(null!));

        Assert.NotNull(exception);
        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public async Task RegisterClaimDocumentAsync_rejects_null_request()
    {
        var storage = new SpyDocumentStorageService();
        var workflow = CreateWorkflow(storage, new SpyFileAttachmentService());

        var exception = await Record.ExceptionAsync(() => workflow.RegisterClaimDocumentAsync(null!));

        Assert.NotNull(exception);
        Assert.IsType<ArgumentNullException>(exception);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task RegisterPolicyDocumentAsync_rejects_missing_policy_id_before_file_copy(string? policyId)
    {
        await UsingTempRootsAsync(async (_, attachmentRoot) =>
        {
            var storage = new SpyDocumentStorageService();
            var fileAttachment = new SpyFileAttachmentService();
            var workflow = CreateWorkflow(storage, fileAttachment);
            var sourcePath = await CreateDummySourceFileAsync(attachmentRoot, "source.pdf", "dummy content");

            var exception = await Record.ExceptionAsync(() => workflow.RegisterPolicyDocumentAsync(
                new PolicyDocumentRegistrationRequest(
                    sourcePath,
                    policyId!,
                    "terms",
                    "Document A",
                    new DateOnly(2026, 7, 1))));

            Assert.NotNull(exception);
            Assert.IsType<ArgumentException>(exception);
            Assert.False(fileAttachment.CopyCalled);
        });
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task RegisterClaimDocumentAsync_rejects_missing_claim_id_before_file_copy(string? claimId)
    {
        await UsingTempRootsAsync(async (_, attachmentRoot) =>
        {
            var storage = new SpyDocumentStorageService();
            var fileAttachment = new SpyFileAttachmentService();
            var workflow = CreateWorkflow(storage, fileAttachment);
            var sourcePath = await CreateDummySourceFileAsync(attachmentRoot, "source.pdf", "dummy content");

            var exception = await Record.ExceptionAsync(() => workflow.RegisterClaimDocumentAsync(
                new ClaimDocumentRegistrationRequest(
                    sourcePath,
                    claimId!,
                    "receipt",
                    "Document A",
                    new DateOnly(2026, 7, 1))));

            Assert.NotNull(exception);
            Assert.IsType<ArgumentException>(exception);
            Assert.False(fileAttachment.CopyCalled);
        });
    }

    [Fact]
    public async Task RegisterPolicyDocumentAsync_missing_source_fails_before_link()
    {
        var storage = new SpyDocumentStorageService();
        var workflow = CreateWorkflow(storage, new SpyFileAttachmentService());
        var missingSourcePath = Path.Combine(Path.GetTempPath(), "FamilyClaimRef.App.Tests", "missing.pdf");

        var exception = await Record.ExceptionAsync(() => workflow.RegisterPolicyDocumentAsync(
            new PolicyDocumentRegistrationRequest(
                missingSourcePath,
                "policy_001",
                "terms",
                "Document A",
                new DateOnly(2026, 7, 1))));

        Assert.NotNull(exception);
        Assert.IsType<FileNotFoundException>(exception);
        Assert.False(storage.GetPolicyDocumentsCalled);
    }

    [Fact]
    public async Task RegisterClaimDocumentAsync_invalid_document_type_fails_before_final_link_success()
    {
        await UsingTempRootsAsync(async (_, attachmentRoot) =>
        {
            var storage = new SpyDocumentStorageService();
            var workflow = CreateWorkflow(storage, new SpyFileAttachmentService());
            var sourcePath = await CreateDummySourceFileAsync(attachmentRoot, "source.pdf", "dummy content");

            var exception = await Record.ExceptionAsync(() => workflow.RegisterClaimDocumentAsync(
                new ClaimDocumentRegistrationRequest(
                    sourcePath,
                    "claim_001",
                    "terms",
                    "Document A",
                    new DateOnly(2026, 7, 1))));

            Assert.NotNull(exception);
            Assert.IsType<ArgumentException>(exception);
            Assert.False(storage.AddClaimDocumentCalled);
        });
    }

    [Fact]
    public async Task RegisterPolicyDocumentAsync_missing_policy_target_rolls_back_attachment_without_link()
    {
        await UsingTempRootsAsync(async (_, attachmentRoot) =>
        {
            var storage = new SpyDocumentStorageService();
            var fileAttachment = new SpyFileAttachmentService();
            var workflow = CreateWorkflow(
                storage,
                fileAttachment,
                new FakePolicyClaimStorageService(activePolicyIds: [], activeClaimIds: ["claim_001"]));
            var sourcePath = await CreateDummySourceFileAsync(attachmentRoot, "source.pdf", "dummy content");

            var exception = await Record.ExceptionAsync(() => workflow.RegisterPolicyDocumentAsync(
                new PolicyDocumentRegistrationRequest(
                    sourcePath,
                    "policy_missing",
                    "terms",
                    "Document A",
                    new DateOnly(2026, 7, 1))));

            Assert.NotNull(exception);
            Assert.IsType<InvalidOperationException>(exception);
            Assert.False(storage.AddPolicyDocumentCalled);
            Assert.True(fileAttachment.DeleteCalled);
            Assert.True(storage.DisableDocumentCalled);
            Assert.Empty(fileAttachment.CopiedRelativePaths);
            Assert.All(storage.Documents, document => Assert.NotNull(document.DisabledAt));
        });
    }

    [Fact]
    public async Task RegisterClaimDocumentAsync_missing_claim_target_rolls_back_attachment_without_link()
    {
        await UsingTempRootsAsync(async (_, attachmentRoot) =>
        {
            var storage = new SpyDocumentStorageService();
            var fileAttachment = new SpyFileAttachmentService();
            var workflow = CreateWorkflow(
                storage,
                fileAttachment,
                new FakePolicyClaimStorageService(activePolicyIds: ["policy_001"], activeClaimIds: []));
            var sourcePath = await CreateDummySourceFileAsync(attachmentRoot, "source.pdf", "dummy content");

            var exception = await Record.ExceptionAsync(() => workflow.RegisterClaimDocumentAsync(
                new ClaimDocumentRegistrationRequest(
                    sourcePath,
                    "claim_missing",
                    "receipt",
                    "Document A",
                    new DateOnly(2026, 7, 1))));

            Assert.NotNull(exception);
            Assert.IsType<InvalidOperationException>(exception);
            Assert.False(storage.AddClaimDocumentCalled);
            Assert.True(fileAttachment.DeleteCalled);
            Assert.True(storage.DisableDocumentCalled);
            Assert.Empty(fileAttachment.CopiedRelativePaths);
            Assert.All(storage.Documents, document => Assert.NotNull(document.DisabledAt));
        });
    }

    [Fact]
    public async Task RegisterPolicyDocumentAsync_link_failure_deletes_copied_file_and_disables_document()
    {
        await UsingTempRootsAsync(async (_, attachmentRoot) =>
        {
            var storage = new SpyDocumentStorageService(failAddPolicyDocument: true);
            var fileAttachment = new SpyFileAttachmentService();
            var workflow = CreateWorkflow(storage, fileAttachment);
            var sourcePath = await CreateDummySourceFileAsync(attachmentRoot, "source.pdf", "dummy content");

            var exception = await Record.ExceptionAsync(() => workflow.RegisterPolicyDocumentAsync(
                new PolicyDocumentRegistrationRequest(
                    sourcePath,
                    "policy_001",
                    "terms",
                    "Document A",
                    new DateOnly(2026, 7, 1))));

            Assert.NotNull(exception);
            Assert.IsType<InvalidOperationException>(exception);
            Assert.True(fileAttachment.DeleteCalled);
            Assert.True(storage.DisableDocumentCalled);
            Assert.Empty(fileAttachment.CopiedRelativePaths);
            Assert.All(storage.Documents, document => Assert.NotNull(document.DisabledAt));
        });
    }

    [Fact]
    public async Task RegisterClaimDocumentAsync_link_failure_deletes_copied_file_and_disables_document()
    {
        await UsingTempRootsAsync(async (_, attachmentRoot) =>
        {
            var storage = new SpyDocumentStorageService(failAddClaimDocument: true);
            var fileAttachment = new SpyFileAttachmentService();
            var workflow = CreateWorkflow(storage, fileAttachment);
            var sourcePath = await CreateDummySourceFileAsync(attachmentRoot, "source.pdf", "dummy content");

            var exception = await Record.ExceptionAsync(() => workflow.RegisterClaimDocumentAsync(
                new ClaimDocumentRegistrationRequest(
                    sourcePath,
                    "claim_001",
                    "receipt",
                    "Document A",
                    new DateOnly(2026, 7, 1))));

            Assert.NotNull(exception);
            Assert.IsType<InvalidOperationException>(exception);
            Assert.True(fileAttachment.DeleteCalled);
            Assert.True(storage.DisableDocumentCalled);
            Assert.Empty(fileAttachment.CopiedRelativePaths);
            Assert.All(storage.Documents, document => Assert.NotNull(document.DisabledAt));
        });
    }

    [Fact]
    public async Task RegisterPolicyDocumentAsync_rollback_file_delete_failure_is_reported()
    {
        await UsingTempRootsAsync(async (_, attachmentRoot) =>
        {
            var storage = new SpyDocumentStorageService(failAddPolicyDocument: true);
            var fileAttachment = new SpyFileAttachmentService(failDelete: true);
            var workflow = CreateWorkflow(storage, fileAttachment);
            var sourcePath = await CreateDummySourceFileAsync(attachmentRoot, "source.pdf", "dummy content");

            var exception = await Record.ExceptionAsync(() => workflow.RegisterPolicyDocumentAsync(
                new PolicyDocumentRegistrationRequest(
                    sourcePath,
                    "policy_001",
                    "terms",
                    "Document A",
                    new DateOnly(2026, 7, 1))));

            var aggregate = Assert.IsType<AggregateException>(exception);
            Assert.Contains(aggregate.InnerExceptions, inner => inner.Message == "Policy link failed.");
            Assert.Contains(aggregate.InnerExceptions, inner => inner.Message == "Delete failed.");
            Assert.True(storage.DisableDocumentCalled);
        });
    }

    [Fact]
    public async Task RegisterClaimDocumentAsync_rollback_document_disable_failure_is_reported()
    {
        await UsingTempRootsAsync(async (_, attachmentRoot) =>
        {
            var storage = new SpyDocumentStorageService(
                failAddClaimDocument: true,
                failDisableDocument: true);
            var fileAttachment = new SpyFileAttachmentService();
            var workflow = CreateWorkflow(storage, fileAttachment);
            var sourcePath = await CreateDummySourceFileAsync(attachmentRoot, "source.pdf", "dummy content");

            var exception = await Record.ExceptionAsync(() => workflow.RegisterClaimDocumentAsync(
                new ClaimDocumentRegistrationRequest(
                    sourcePath,
                    "claim_001",
                    "receipt",
                    "Document A",
                    new DateOnly(2026, 7, 1))));

            var aggregate = Assert.IsType<AggregateException>(exception);
            Assert.Contains(aggregate.InnerExceptions, inner => inner.Message == "Claim link failed.");
            Assert.Contains(aggregate.InnerExceptions, inner => inner.Message == "Disable failed.");
            Assert.True(fileAttachment.DeleteCalled);
        });
    }

    [Fact]
    public async Task RegisterPolicyDocumentAsync_rollback_attempts_file_delete_and_document_disable_when_both_fail()
    {
        await UsingTempRootsAsync(async (_, attachmentRoot) =>
        {
            var storage = new SpyDocumentStorageService(
                failAddPolicyDocument: true,
                failDisableDocument: true);
            var fileAttachment = new SpyFileAttachmentService(failDelete: true);
            var workflow = CreateWorkflow(storage, fileAttachment);
            var sourcePath = await CreateDummySourceFileAsync(attachmentRoot, "source.pdf", "dummy content");

            var exception = await Record.ExceptionAsync(() => workflow.RegisterPolicyDocumentAsync(
                new PolicyDocumentRegistrationRequest(
                    sourcePath,
                    "policy_001",
                    "terms",
                    "Document A",
                    new DateOnly(2026, 7, 1))));

            var aggregate = Assert.IsType<AggregateException>(exception);
            Assert.Equal(3, aggregate.InnerExceptions.Count);
            Assert.True(fileAttachment.DeleteCalled);
            Assert.True(storage.DisableDocumentCalled);
        });
    }

    [Fact]
    public async Task Coordinator_tests_do_not_create_project_root_attachment_or_data_files()
    {
        var projectRoot = FindProjectRoot();
        var attachmentsBefore = SnapshotFiles(Path.Combine(projectRoot, "attachments"));
        var dataLocalBefore = SnapshotFiles(Path.Combine(projectRoot, "data", "local"));

        await UsingTempRootsAsync(async (metadataRoot, attachmentRoot) =>
        {
            var storage = new JsonDocumentStorageService(metadataRoot);
            var workflow = CreateWorkflow(storage, new LocalFileAttachmentService(attachmentRoot));
            var sourcePath = await CreateDummySourceFileAsync(attachmentRoot, "source.pdf", "dummy content");

            await workflow.RegisterPolicyDocumentAsync(new PolicyDocumentRegistrationRequest(
                sourcePath,
                "policy_001",
                "terms",
                "Document A",
                new DateOnly(2026, 7, 1)));
        });

        var attachmentsAfter = SnapshotFiles(Path.Combine(projectRoot, "attachments"));
        var dataLocalAfter = SnapshotFiles(Path.Combine(projectRoot, "data", "local"));
        Assert.Equal(attachmentsBefore, attachmentsAfter);
        Assert.Equal(dataLocalBefore, dataLocalAfter);
    }

    private static DocumentRegistrationWorkflow CreateWorkflow(
        IDocumentStorageService storage,
        IFileAttachmentService fileAttachment)
    {
        return CreateWorkflow(
            storage,
            fileAttachment,
            new FakePolicyClaimStorageService(
                ["policy_001"],
                ["claim_001"]));
    }

    private static DocumentRegistrationWorkflow CreateWorkflow(
        IDocumentStorageService storage,
        IFileAttachmentService fileAttachment,
        IPolicyClaimStorageService policyClaimStorageService)
    {
        return new DocumentRegistrationWorkflow(
            CreateAttachmentCoordinator(storage, fileAttachment),
            CreateLinkCoordinator(storage, policyClaimStorageService),
            storage,
            fileAttachment);
    }

    private static DocumentAttachmentCoordinator CreateAttachmentCoordinator(
        IDocumentStorageService storage,
        IFileAttachmentService fileAttachment)
    {
        return new DocumentAttachmentCoordinator(storage, fileAttachment);
    }

    private static DocumentLinkCoordinator CreateLinkCoordinator(IDocumentStorageService storage)
    {
        return new DocumentLinkCoordinator(
            storage,
            new FakePolicyClaimStorageService(
                ["policy_001"],
                ["claim_001"]));
    }

    private static DocumentLinkCoordinator CreateLinkCoordinator(
        IDocumentStorageService storage,
        IPolicyClaimStorageService policyClaimStorageService)
    {
        return new DocumentLinkCoordinator(storage, policyClaimStorageService);
    }

    private static async Task<string> CreateDummySourceFileAsync(string rootPath, string fileName, string content)
    {
        var sourceDirectory = Path.Combine(rootPath, "source");
        Directory.CreateDirectory(sourceDirectory);
        var sourcePath = Path.Combine(sourceDirectory, fileName);
        await File.WriteAllTextAsync(sourcePath, content);

        return sourcePath;
    }

    private static async Task UsingTempRootsAsync(Func<string, string, Task> action)
    {
        var rootPath = Path.Combine(Path.GetTempPath(), "FamilyClaimRef.App.Tests", Guid.NewGuid().ToString("N"));
        var metadataRoot = Path.Combine(rootPath, "metadata");
        var attachmentRoot = Path.Combine(rootPath, "attachments");
        Directory.CreateDirectory(metadataRoot);
        Directory.CreateDirectory(attachmentRoot);

        try
        {
            await action(metadataRoot, attachmentRoot);
        }
        finally
        {
            if (Directory.Exists(rootPath))
            {
                Directory.Delete(rootPath, recursive: true);
            }
        }
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

    private static string[] SnapshotFiles(string directoryPath)
    {
        if (!Directory.Exists(directoryPath))
        {
            return [];
        }

        return Directory
            .GetFiles(directoryPath, "*", SearchOption.AllDirectories)
            .Select(path => Path.GetRelativePath(directoryPath, path))
            .Order(StringComparer.Ordinal)
            .ToArray();
    }

    private sealed class SpyFileAttachmentService : IFileAttachmentService
    {
        private readonly HashSet<string> existingRelativePaths;
        private readonly bool failDelete;

        public SpyFileAttachmentService(
            IEnumerable<string>? existingRelativePaths = null,
            bool failDelete = false)
        {
            this.existingRelativePaths = (existingRelativePaths ?? []).ToHashSet(StringComparer.Ordinal);
            this.failDelete = failDelete;
        }

        public bool CopyCalled { get; private set; }

        public bool DeleteCalled { get; private set; }

        public HashSet<string> CopiedRelativePaths { get; } = new(StringComparer.Ordinal);

        public Task<FileAttachmentCopyResult> CopyDocumentFileAsync(
            string sourceFilePath,
            string physicalFileName,
            CancellationToken cancellationToken = default)
        {
            CopyCalled = true;
            var relativePath = $"documents/{physicalFileName}";
            CopiedRelativePaths.Add(relativePath);

            return Task.FromResult(new FileAttachmentCopyResult(
                relativePath,
                physicalFileName,
                Path.GetExtension(physicalFileName).TrimStart('.').ToLowerInvariant(),
                13));
        }

        public Task DeleteDocumentFileIfExistsAsync(
            string relativePath,
            CancellationToken cancellationToken = default)
        {
            DeleteCalled = true;
            if (failDelete)
            {
                throw new IOException("Delete failed.");
            }

            CopiedRelativePaths.Remove(relativePath);

            return Task.CompletedTask;
        }

        public Task<bool> DocumentFileExistsAsync(
            string relativePath,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(existingRelativePaths.Contains(relativePath));
        }
    }

    private sealed class SpyDocumentStorageService : IDocumentStorageService
    {
        private readonly bool failAddPolicyDocument;
        private readonly bool failAddClaimDocument;
        private readonly bool failDisableDocument;
        private readonly List<DocumentRecord> documents = [];
        private readonly List<PolicyDocumentRecord> policyDocuments = [];
        private readonly List<ClaimDocumentRecord> claimDocuments = [];

        public SpyDocumentStorageService(
            bool failAddPolicyDocument = false,
            bool failAddClaimDocument = false,
            bool failDisableDocument = false)
        {
            this.failAddPolicyDocument = failAddPolicyDocument;
            this.failAddClaimDocument = failAddClaimDocument;
            this.failDisableDocument = failDisableDocument;
        }

        public IReadOnlyList<DocumentRecord> Documents => documents;

        public bool AddPolicyDocumentCalled { get; private set; }

        public bool AddClaimDocumentCalled { get; private set; }

        public bool GetPolicyDocumentsCalled { get; private set; }

        public bool DisableDocumentCalled { get; private set; }

        public Task<IReadOnlyList<DocumentRecord>> GetDocumentsAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IReadOnlyList<DocumentRecord>>(documents.ToList());
        }

        public Task<DocumentRecord?> GetDocumentByIdAsync(
            string documentId,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(documents.FirstOrDefault(document => document.Id == documentId));
        }

        public Task<DocumentRecord> AddDocumentAsync(
            DocumentDraft draft,
            CancellationToken cancellationToken = default)
        {
            var timestamp = DateTimeOffset.UtcNow;
            var record = new DocumentRecord(
                $"doc_{Guid.NewGuid():N}",
                draft.PhysicalFileName,
                draft.DisplayTitle,
                draft.Extension,
                draft.RelativePath,
                timestamp,
                timestamp,
                null);

            documents.Add(record);

            return Task.FromResult(record);
        }

        public Task DisableDocumentAsync(
            string documentId,
            DateTimeOffset disabledAt,
            CancellationToken cancellationToken = default)
        {
            DisableDocumentCalled = true;
            if (failDisableDocument)
            {
                throw new InvalidOperationException("Disable failed.");
            }

            var documentIndex = documents.FindIndex(document => document.Id == documentId);
            if (documentIndex < 0)
            {
                throw new InvalidOperationException("Document was not found.");
            }

            documents[documentIndex] = documents[documentIndex] with
            {
                UpdatedAt = disabledAt,
                DisabledAt = disabledAt
            };

            return Task.CompletedTask;
        }

        public Task<IReadOnlyList<PolicyDocumentRecord>> GetPolicyDocumentsAsync(
            string policyId,
            CancellationToken cancellationToken = default)
        {
            GetPolicyDocumentsCalled = true;

            return Task.FromResult<IReadOnlyList<PolicyDocumentRecord>>(
                policyDocuments.Where(policyDocument => policyDocument.PolicyId == policyId).ToList());
        }

        public Task<PolicyDocumentRecord> AddPolicyDocumentAsync(
            PolicyDocumentDraft draft,
            CancellationToken cancellationToken = default)
        {
            AddPolicyDocumentCalled = true;
            if (failAddPolicyDocument)
            {
                throw new InvalidOperationException("Policy link failed.");
            }

            var timestamp = DateTimeOffset.UtcNow;
            var record = new PolicyDocumentRecord(
                $"pdoc_{Guid.NewGuid():N}",
                draft.PolicyId,
                draft.DocumentId,
                draft.DocumentType,
                timestamp,
                timestamp,
                null);

            policyDocuments.Add(record);

            return Task.FromResult(record);
        }

        public Task DisablePolicyDocumentAsync(
            string policyDocumentId,
            DateTimeOffset disabledAt,
            CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<ClaimDocumentRecord>> GetClaimDocumentsAsync(
            string claimId,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IReadOnlyList<ClaimDocumentRecord>>(
                claimDocuments.Where(claimDocument => claimDocument.ClaimId == claimId).ToList());
        }

        public Task<ClaimDocumentRecord> AddClaimDocumentAsync(
            ClaimDocumentDraft draft,
            CancellationToken cancellationToken = default)
        {
            AddClaimDocumentCalled = true;
            if (failAddClaimDocument)
            {
                throw new InvalidOperationException("Claim link failed.");
            }

            var timestamp = DateTimeOffset.UtcNow;
            var record = new ClaimDocumentRecord(
                $"cdoc_{Guid.NewGuid():N}",
                draft.ClaimId,
                draft.DocumentId,
                draft.DocumentType,
                timestamp,
                timestamp,
                null);

            claimDocuments.Add(record);

            return Task.FromResult(record);
        }

        public Task DisableClaimDocumentAsync(
            string claimDocumentId,
            DateTimeOffset disabledAt,
            CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }

    private sealed class FakePolicyClaimStorageService : IPolicyClaimStorageService
    {
        private readonly HashSet<string> activePolicyIds;
        private readonly HashSet<string> activeClaimIds;

        public FakePolicyClaimStorageService(
            IEnumerable<string>? activePolicyIds = null,
            IEnumerable<string>? activeClaimIds = null)
        {
            this.activePolicyIds = (activePolicyIds ?? []).ToHashSet(StringComparer.Ordinal);
            this.activeClaimIds = (activeClaimIds ?? []).ToHashSet(StringComparer.Ordinal);
        }

        public Task<IReadOnlyList<PolicyRecord>> GetPoliciesAsync(
            CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<PolicyRecord?> GetPolicyAsync(
            string id,
            CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<PolicyRecord> AddPolicyAsync(
            PolicyDraft draft,
            CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<PolicyRecord> DisablePolicyAsync(
            string id,
            CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<ClaimRecord>> GetClaimsAsync(
            CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<ClaimRecord>> GetClaimsByPolicyIdAsync(
            string policyId,
            CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<ClaimRecord?> GetClaimAsync(
            string id,
            CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<ClaimRecord> AddClaimAsync(
            ClaimDraft draft,
            CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<ClaimRecord> DisableClaimAsync(
            string id,
            CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<bool> PolicyExistsAsync(
            string id,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(activePolicyIds.Contains(id));
        }

        public Task<bool> ClaimExistsAsync(
            string id,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(activeClaimIds.Contains(id));
        }
    }
}
