using FamilyClaimRef.App.Models.Storage;
using FamilyClaimRef.App.Services;
using FamilyClaimRef.App.Services.Storage;
using Xunit;

namespace FamilyClaimRef.App.Tests;

public sealed class DocumentAttachmentCoordinatorTests
{
    [Fact]
    public void Constructor_rejects_null_document_storage_service()
    {
        var exception = Record.Exception(() => new DocumentAttachmentCoordinator(
            null!,
            new SpyFileAttachmentService()));

        Assert.NotNull(exception);
        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void Constructor_rejects_null_file_attachment_service()
    {
        var exception = Record.Exception(() => new DocumentAttachmentCoordinator(
            new InMemoryDocumentStorageService(),
            null!));

        Assert.NotNull(exception);
        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void Constructor_rejects_null_file_validation_service()
    {
        var exception = Record.Exception(() => new DocumentAttachmentCoordinator(
            new InMemoryDocumentStorageService(),
            new SpyFileAttachmentService(),
            null!));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public async Task AttachDocumentAsync_copies_file_and_saves_document_metadata()
    {
        await UsingTempRootsAsync(async (metadataRoot, attachmentRoot) =>
        {
            var documentStorage = new JsonDocumentStorageService(metadataRoot);
            var fileAttachment = new LocalFileAttachmentService(attachmentRoot);
            var coordinator = new DocumentAttachmentCoordinator(documentStorage, fileAttachment);
            var sourcePath = await CreateDummySourceFileAsync(attachmentRoot, "source.pdf", "dummy file content");

            var result = await coordinator.AttachDocumentAsync(CreateRequest(sourcePath));

            Assert.NotNull(result.Document);
            Assert.NotNull(result.File);
            Assert.Equal(result.File.RelativePath, result.Document.RelativePath);
            Assert.Equal(result.File.PhysicalFileName, result.Document.PhysicalFileName);
            Assert.Equal(result.File.Extension, result.Document.Extension);
            Assert.Equal("청구 서류 A", result.Document.DisplayTitle);
            Assert.False(Path.IsPathRooted(result.Document.RelativePath));
            Assert.False(Path.IsPathRooted(result.File.RelativePath));
            Assert.True(File.Exists(Path.Combine(attachmentRoot, "documents", result.File.PhysicalFileName)));
            Assert.True(File.Exists(Path.Combine(metadataRoot, "documents.json")));
        });
    }

    [Fact]
    public async Task AttachDocumentAsync_generates_physical_file_name_with_file_name_policy_service()
    {
        await UsingTempRootsAsync(async (metadataRoot, attachmentRoot) =>
        {
            var coordinator = new DocumentAttachmentCoordinator(
                new JsonDocumentStorageService(metadataRoot),
                new LocalFileAttachmentService(attachmentRoot));
            var sourcePath = await CreateDummySourceFileAsync(attachmentRoot, "source.JPEG", "dummy file content");
            var expectedPhysicalFileName = FileNamePolicyService.CreatePhysicalFileName(
                "claim",
                "document",
                new DateOnly(2026, 7, 1),
                "receipt",
                "JPEG",
                1);

            var result = await coordinator.AttachDocumentAsync(CreateRequest(sourcePath));

            Assert.Equal(expectedPhysicalFileName, result.File.PhysicalFileName);
            Assert.Equal("jpeg", result.File.Extension);
        });
    }

    [Fact]
    public async Task AttachDocumentAsync_rejects_invalid_document_type_before_file_copy()
    {
        await UsingTempRootsAsync(async (_, attachmentRoot) =>
        {
            var fileAttachment = new SpyFileAttachmentService();
            var coordinator = new DocumentAttachmentCoordinator(new InMemoryDocumentStorageService(), fileAttachment);
            var sourcePath = await CreateDummySourceFileAsync(attachmentRoot, "source.pdf", "dummy file content");

            var exception = await Record.ExceptionAsync(() => coordinator.AttachDocumentAsync(
                CreateRequest(sourcePath, documentType: "terms")));

            Assert.NotNull(exception);
            Assert.IsType<ArgumentException>(exception);
            Assert.False(fileAttachment.CopyCalled);
        });
    }

    [Fact]
    public async Task AttachDocumentAsync_rejects_invalid_extension_before_file_copy()
    {
        await UsingTempRootsAsync(async (_, attachmentRoot) =>
        {
            var fileAttachment = new SpyFileAttachmentService();
            var coordinator = new DocumentAttachmentCoordinator(new InMemoryDocumentStorageService(), fileAttachment);
            var sourcePath = await CreateDummySourceFileAsync(attachmentRoot, "source.exe", "dummy file content");

            var exception = await Record.ExceptionAsync(() => coordinator.AttachDocumentAsync(CreateRequest(sourcePath)));

            Assert.NotNull(exception);
            Assert.IsType<ArgumentException>(exception);
            Assert.False(fileAttachment.CopyCalled);
        });
    }

    [Fact]
    public async Task AttachDocumentAsync_auto_starts_duplicate_index_at_1()
    {
        await UsingTempRootsAsync(async (_, attachmentRoot) =>
        {
            var fileAttachment = new SpyFileAttachmentService();
            var coordinator = new DocumentAttachmentCoordinator(new InMemoryDocumentStorageService(), fileAttachment);
            var sourcePath = await CreateDummySourceFileAsync(attachmentRoot, "source.pdf", "dummy file content");

            var result = await coordinator.AttachDocumentAsync(CreateRequest(sourcePath));

            Assert.EndsWith("_001.pdf", result.File.PhysicalFileName, StringComparison.Ordinal);
        });
    }

    [Fact]
    public async Task AttachDocumentAsync_auto_increments_duplicate_index_when_target_file_exists()
    {
        await UsingTempRootsAsync(async (_, attachmentRoot) =>
        {
            var existingFileName = FileNamePolicyService.CreatePhysicalFileName(
                "claim",
                "document",
                new DateOnly(2026, 7, 1),
                "receipt",
                "pdf",
                1);
            var fileAttachment = new SpyFileAttachmentService(existingRelativePaths: [$"documents/{existingFileName}"]);
            var coordinator = new DocumentAttachmentCoordinator(new InMemoryDocumentStorageService(), fileAttachment);
            var sourcePath = await CreateDummySourceFileAsync(attachmentRoot, "source.pdf", "dummy file content");

            var result = await coordinator.AttachDocumentAsync(CreateRequest(sourcePath));

            Assert.EndsWith("_002.pdf", result.File.PhysicalFileName, StringComparison.Ordinal);
        });
    }

    [Fact]
    public async Task AttachDocumentAsync_auto_increments_duplicate_index_when_metadata_has_same_physical_file_name()
    {
        await UsingTempRootsAsync(async (_, attachmentRoot) =>
        {
            var existingFileName = FileNamePolicyService.CreatePhysicalFileName(
                "claim",
                "document",
                new DateOnly(2026, 7, 1),
                "receipt",
                "pdf",
                1);
            var documentStorage = new InMemoryDocumentStorageService([
                CreateDocumentRecord(existingFileName, $"documents/{existingFileName}")
            ]);
            var coordinator = new DocumentAttachmentCoordinator(documentStorage, new SpyFileAttachmentService());
            var sourcePath = await CreateDummySourceFileAsync(attachmentRoot, "source.pdf", "dummy file content");

            var result = await coordinator.AttachDocumentAsync(CreateRequest(sourcePath));

            Assert.EndsWith("_002.pdf", result.File.PhysicalFileName, StringComparison.Ordinal);
        });
    }

    [Fact]
    public async Task AttachDocumentAsync_retries_next_duplicate_index_when_final_copy_target_exists()
    {
        await UsingTempRootsAsync(async (_, attachmentRoot) =>
        {
            var fileAttachment = new SpyFileAttachmentService(throwTargetExistsOnFirstCopy: true);
            var coordinator = new DocumentAttachmentCoordinator(new InMemoryDocumentStorageService(), fileAttachment);
            var sourcePath = await CreateDummySourceFileAsync(attachmentRoot, "source.pdf", "dummy file content");

            var result = await coordinator.AttachDocumentAsync(CreateRequest(sourcePath));

            Assert.Equal(2, fileAttachment.CopyCallCount);
            Assert.EndsWith("_002.pdf", result.File.PhysicalFileName, StringComparison.Ordinal);
        });
    }

    [Fact]
    public async Task AttachDocumentAsync_rejects_null_request()
    {
        var coordinator = new DocumentAttachmentCoordinator(
            new InMemoryDocumentStorageService(),
            new SpyFileAttachmentService());

        var exception = await Record.ExceptionAsync(() => coordinator.AttachDocumentAsync(null!));

        Assert.NotNull(exception);
        Assert.IsType<ArgumentNullException>(exception);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task AttachDocumentAsync_rejects_required_source_path(string? sourceFilePath)
    {
        var coordinator = new DocumentAttachmentCoordinator(
            new InMemoryDocumentStorageService(),
            new SpyFileAttachmentService());

        var exception = await Record.ExceptionAsync(() => coordinator.AttachDocumentAsync(
            CreateRequest(sourceFilePath!)));

        Assert.NotNull(exception);
        Assert.IsType<ArgumentException>(exception);
    }

    [Fact]
    public async Task AttachDocumentAsync_rejects_missing_source_file_before_metadata_save()
    {
        var documentStorage = new InMemoryDocumentStorageService();
        var coordinator = new DocumentAttachmentCoordinator(documentStorage, new SpyFileAttachmentService());
        var missingSourcePath = Path.Combine(Path.GetTempPath(), "FamilyClaimRef.App.Tests", "missing.pdf");

        var exception = await Record.ExceptionAsync(() => coordinator.AttachDocumentAsync(CreateRequest(missingSourcePath)));

        Assert.NotNull(exception);
        Assert.IsType<FileNotFoundException>(exception);
        Assert.False(documentStorage.AddDocumentCalled);
    }

    [Theory]
    [InlineData(null, "receipt", "청구 서류 A")]
    [InlineData("", "receipt", "청구 서류 A")]
    [InlineData("   ", "receipt", "청구 서류 A")]
    [InlineData("claim", null, "청구 서류 A")]
    [InlineData("claim", "", "청구 서류 A")]
    [InlineData("claim", "   ", "청구 서류 A")]
    [InlineData("claim", "receipt", null)]
    [InlineData("claim", "receipt", "")]
    [InlineData("claim", "receipt", "   ")]
    public async Task AttachDocumentAsync_rejects_required_request_values(
        string? documentScope,
        string? documentType,
        string? displayTitle)
    {
        await UsingTempRootsAsync(async (_, attachmentRoot) =>
        {
            var sourcePath = await CreateDummySourceFileAsync(attachmentRoot, "source.pdf", "dummy file content");
            var coordinator = new DocumentAttachmentCoordinator(
                new InMemoryDocumentStorageService(),
                new SpyFileAttachmentService());

            var exception = await Record.ExceptionAsync(() => coordinator.AttachDocumentAsync(
                CreateRequest(sourcePath, documentScope, documentType, displayTitle)));

            Assert.NotNull(exception);
            Assert.IsType<ArgumentException>(exception);
        });
    }

    [Fact]
    public async Task AttachDocumentAsync_allows_null_reference_date_without_today_default()
    {
        await UsingTempRootsAsync(async (_, attachmentRoot) =>
        {
            var sourcePath = await CreateDummySourceFileAsync(attachmentRoot, "source.pdf", "dummy file content");
            var coordinator = new DocumentAttachmentCoordinator(
                new InMemoryDocumentStorageService(),
                new SpyFileAttachmentService());

            var result = await coordinator.AttachDocumentAsync(new DocumentAttachmentRequest(
                sourcePath,
                "claim",
                "receipt",
                "청구 서류 A",
                null));

            Assert.Null(result.Document.ReferenceDate);
            Assert.Contains("_00010101_", result.File.PhysicalFileName, StringComparison.Ordinal);
        });
    }

    [Fact]
    public async Task AttachDocumentAsync_cleans_up_copied_file_when_metadata_save_fails()
    {
        await UsingTempRootsAsync(async (_, attachmentRoot) =>
        {
            var fileAttachment = new SpyFileAttachmentService();
            var coordinator = new DocumentAttachmentCoordinator(
                new FailingAddDocumentStorageService(),
                fileAttachment);
            var sourcePath = await CreateDummySourceFileAsync(attachmentRoot, "source.pdf", "dummy file content");

            var exception = await Record.ExceptionAsync(() => coordinator.AttachDocumentAsync(CreateRequest(sourcePath)));

            Assert.NotNull(exception);
            Assert.IsType<InvalidOperationException>(exception);
            Assert.True(fileAttachment.DeleteCalled);
            Assert.Empty(fileAttachment.CopiedRelativePaths);
        });
    }

    [Fact]
    public async Task AttachDocumentAsync_reports_cleanup_failure_with_metadata_failure()
    {
        await UsingTempRootsAsync(async (_, attachmentRoot) =>
        {
            var fileAttachment = new SpyFileAttachmentService(failDelete: true);
            var coordinator = new DocumentAttachmentCoordinator(
                new FailingAddDocumentStorageService(),
                fileAttachment);
            var sourcePath = await CreateDummySourceFileAsync(attachmentRoot, "source.pdf", "dummy file content");

            var exception = await Record.ExceptionAsync(() => coordinator.AttachDocumentAsync(CreateRequest(sourcePath)));

            var aggregate = Assert.IsType<AggregateException>(exception);
            Assert.Contains(aggregate.InnerExceptions, inner => inner.Message == "Metadata save failed.");
            Assert.Contains(aggregate.InnerExceptions, inner => inner.Message == "Cleanup failed.");
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
            var coordinator = new DocumentAttachmentCoordinator(
                new JsonDocumentStorageService(metadataRoot),
                new LocalFileAttachmentService(attachmentRoot));
            var sourcePath = await CreateDummySourceFileAsync(attachmentRoot, "source.png", "dummy file content");

            await coordinator.AttachDocumentAsync(CreateRequest(sourcePath, documentType: "medicine"));
        });

        var attachmentsAfter = SnapshotFiles(Path.Combine(projectRoot, "attachments"));
        var dataLocalAfter = SnapshotFiles(Path.Combine(projectRoot, "data", "local"));
        Assert.Equal(attachmentsBefore, attachmentsAfter);
        Assert.Equal(dataLocalBefore, dataLocalAfter);
    }

    private static DocumentAttachmentRequest CreateRequest(
        string sourceFilePath,
        string? documentScope = "claim",
        string? documentType = "receipt",
        string? displayTitle = "청구 서류 A",
        DateOnly? referenceDate = null)
    {
        return new DocumentAttachmentRequest(
            sourceFilePath,
            documentScope!,
            documentType!,
            displayTitle!,
            referenceDate ?? new DateOnly(2026, 7, 1));
    }

    private static DocumentRecord CreateDocumentRecord(string physicalFileName, string relativePath)
    {
        var timestamp = DateTimeOffset.UtcNow;

        return new DocumentRecord(
            $"doc_{Guid.NewGuid():N}",
            physicalFileName,
            "기존 문서",
            Path.GetExtension(physicalFileName).TrimStart('.').ToLowerInvariant(),
            relativePath,
            timestamp,
            timestamp,
            null);
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
        private readonly bool throwTargetExistsOnFirstCopy;
        private readonly bool failDelete;

        public SpyFileAttachmentService(
            IEnumerable<string>? existingRelativePaths = null,
            bool throwTargetExistsOnFirstCopy = false,
            bool failDelete = false)
        {
            this.existingRelativePaths = (existingRelativePaths ?? []).ToHashSet(StringComparer.Ordinal);
            this.throwTargetExistsOnFirstCopy = throwTargetExistsOnFirstCopy;
            this.failDelete = failDelete;
        }

        public bool CopyCalled { get; private set; }

        public int CopyCallCount { get; private set; }

        public bool DeleteCalled { get; private set; }

        public HashSet<string> CopiedRelativePaths { get; } = new(StringComparer.Ordinal);

        public Task<FileAttachmentCopyResult> CopyDocumentFileAsync(
            string sourceFilePath,
            string physicalFileName,
            CancellationToken cancellationToken = default)
        {
            CopyCalled = true;
            CopyCallCount++;
            if (throwTargetExistsOnFirstCopy && CopyCallCount == 1)
            {
                throw new IOException("Target file already exists.");
            }

            var relativePath = $"documents/{physicalFileName}";
            CopiedRelativePaths.Add(relativePath);

            return Task.FromResult(new FileAttachmentCopyResult(
                relativePath,
                physicalFileName,
                Path.GetExtension(physicalFileName).TrimStart('.').ToLowerInvariant(),
                18));
        }

        public Task DeleteDocumentFileIfExistsAsync(
            string relativePath,
            CancellationToken cancellationToken = default)
        {
            DeleteCalled = true;
            if (failDelete)
            {
                throw new IOException("Cleanup failed.");
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

    private class InMemoryDocumentStorageService : IDocumentStorageService
    {
        private readonly List<DocumentRecord> documents;

        public InMemoryDocumentStorageService(IEnumerable<DocumentRecord>? documents = null)
        {
            this.documents = documents?.ToList() ?? [];
        }

        public bool AddDocumentCalled { get; private set; }

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

        public virtual Task<DocumentRecord> AddDocumentAsync(
            DocumentDraft draft,
            CancellationToken cancellationToken = default)
        {
            AddDocumentCalled = true;
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
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<PolicyDocumentRecord>> GetPolicyDocumentsAsync(
            string policyId,
            CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<PolicyDocumentRecord> AddPolicyDocumentAsync(
            PolicyDocumentDraft draft,
            CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public Task<ClaimDocumentRecord> AddClaimDocumentAsync(
            ClaimDocumentDraft draft,
            CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task DisableClaimDocumentAsync(
            string claimDocumentId,
            DateTimeOffset disabledAt,
            CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }

    private sealed class FailingAddDocumentStorageService : InMemoryDocumentStorageService
    {
        public override Task<DocumentRecord> AddDocumentAsync(
            DocumentDraft draft,
            CancellationToken cancellationToken = default)
        {
            throw new InvalidOperationException("Metadata save failed.");
        }
    }
}
