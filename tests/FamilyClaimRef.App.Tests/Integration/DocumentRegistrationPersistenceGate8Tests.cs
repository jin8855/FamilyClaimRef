using FamilyClaimRef.App.Models.Storage;
using FamilyClaimRef.App.Services.Storage;
using Xunit;

namespace FamilyClaimRef.App.Tests.Integration;

public sealed class DocumentRegistrationPersistenceGate8Tests
{
    private static readonly DateOnly ReferenceDate = new(2026, 7, 24);

    [Fact]
    public async Task I01_managed_copy_survives_source_deletion()
    {
        await UsingContextAsync(async context =>
        {
            var policy = await context.CreatePolicyAsync("Synthetic policy");
            var source = await context.CreatePngAsync("source.png", 0x01);
            var snapshot = await context.Validation.ValidateSourceAsync(source);

            var result = await context.RegisterPolicyAsync(policy.Id, source, snapshot);
            File.Delete(source);
            var finalPath = context.GetAttachmentPath(result.Attachment.File.RelativePath);

            Assert.True(File.Exists(finalPath));
            Assert.Equal(PngBytes(0x01), await File.ReadAllBytesAsync(finalPath));
        });
    }

    [Fact]
    public async Task I02_relative_key_length_type_and_sha_persist()
    {
        await UsingContextAsync(async context =>
        {
            var policy = await context.CreatePolicyAsync("Synthetic policy");
            var source = await context.CreatePngAsync("metadata.png", 0x02);
            var snapshot = await context.Validation.ValidateSourceAsync(source);

            var result = await context.RegisterPolicyAsync(policy.Id, source, snapshot);
            var loaded = await context.DocumentStorage.GetDocumentByIdAsync(result.Attachment.Document.Id);

            Assert.NotNull(loaded);
            Assert.StartsWith("documents/", loaded.RelativePath, StringComparison.Ordinal);
            Assert.False(Path.IsPathRooted(loaded.RelativePath));
            Assert.Equal(snapshot.ByteLength, loaded.ByteLength);
            Assert.Equal("PNG", loaded.ValidatedFileType);
            Assert.Equal(snapshot.Sha256, loaded.Sha256);
            Assert.Equal("metadata.png", loaded.OriginalDisplayFileName);
            Assert.Equal(ReferenceDate, loaded.ReferenceDate);
            Assert.Equal("terms", loaded.DocumentType);
        });
    }

    [Fact]
    public async Task I03_same_target_and_same_sha_is_rejected()
    {
        await UsingContextAsync(async context =>
        {
            var policy = await context.CreatePolicyAsync("Synthetic policy");
            var source = await context.CreatePngAsync("duplicate.png", 0x03);
            var snapshot = await context.Validation.ValidateSourceAsync(source);
            await context.RegisterPolicyAsync(policy.Id, source, snapshot);

            var exception = await Record.ExceptionAsync(() =>
                context.RegisterPolicyAsync(policy.Id, source, snapshot));

            AssertRegistrationError(exception, DocumentRegistrationErrorCode.DuplicateDocument);
            Assert.Single(await context.DocumentStorage.GetDocumentsAsync());
            Assert.Single(await context.DocumentStorage.GetPolicyDocumentsAsync(policy.Id));
        });
    }

    [Fact]
    public async Task I04_same_name_and_different_bytes_is_allowed()
    {
        await UsingContextAsync(async context =>
        {
            var policy = await context.CreatePolicyAsync("Synthetic policy");
            var source = await context.CreatePngAsync("same-name.png", 0x04);
            var first = await context.Validation.ValidateSourceAsync(source);
            await context.RegisterPolicyAsync(policy.Id, source, first);
            await File.WriteAllBytesAsync(source, PngBytes(0x05));
            var second = await context.Validation.ValidateSourceAsync(source);

            await context.RegisterPolicyAsync(policy.Id, source, second);

            Assert.Equal(2, (await context.DocumentStorage.GetDocumentsAsync()).Count);
            var policyLinks = await context.DocumentStorage.GetPolicyDocumentsAsync(policy.Id);
            Assert.Equal(2, policyLinks.Count);
            Assert.Single(policyLinks, link => link.DisabledAt is null);
            Assert.Single(policyLinks, link => link.DisabledAt is not null);
        });
    }

    [Fact]
    public async Task I05_different_targets_and_same_bytes_are_allowed()
    {
        await UsingContextAsync(async context =>
        {
            var firstPolicy = await context.CreatePolicyAsync("Synthetic policy one");
            var secondPolicy = await context.CreatePolicyAsync("Synthetic policy two");
            var source = await context.CreatePngAsync("shared.png", 0x06);
            var snapshot = await context.Validation.ValidateSourceAsync(source);

            await context.RegisterPolicyAsync(firstPolicy.Id, source, snapshot);
            await context.RegisterPolicyAsync(secondPolicy.Id, source, snapshot);

            Assert.Equal(2, (await context.DocumentStorage.GetDocumentsAsync()).Count);
            Assert.Single(await context.DocumentStorage.GetPolicyDocumentsAsync(firstPolicy.Id));
            Assert.Single(await context.DocumentStorage.GetPolicyDocumentsAsync(secondPolicy.Id));
        });
    }

    [Fact]
    public async Task I06_document_metadata_failure_deletes_final_payload()
    {
        await UsingContextAsync(async context =>
        {
            var policy = await context.CreatePolicyAsync("Synthetic policy");
            var source = await context.CreatePngAsync("document-failure.png", 0x07);
            var snapshot = await context.Validation.ValidateSourceAsync(source);
            var storage = new DelegatingDocumentStorageService(context.DocumentStorage)
            {
                FailAddDocument = true
            };

            var exception = await Record.ExceptionAsync(() =>
                context.RegisterPolicyAsync(policy.Id, source, snapshot, storage));

            Assert.IsType<InvalidOperationException>(exception);
            Assert.Empty(SnapshotFiles(Path.Combine(context.AttachmentRoot, "documents")));
            Assert.Empty(await context.DocumentStorage.GetDocumentsAsync());
        });
    }

    [Fact]
    public async Task I07_link_failure_deletes_payload_and_disables_document()
    {
        await UsingContextAsync(async context =>
        {
            var policy = await context.CreatePolicyAsync("Synthetic policy");
            var source = await context.CreatePngAsync("link-failure.png", 0x08);
            var snapshot = await context.Validation.ValidateSourceAsync(source);
            var storage = new DelegatingDocumentStorageService(context.DocumentStorage)
            {
                FailAddPolicyDocument = true
            };

            var exception = await Record.ExceptionAsync(() =>
                context.RegisterPolicyAsync(policy.Id, source, snapshot, storage));

            Assert.IsType<InvalidOperationException>(exception);
            Assert.Empty(SnapshotFiles(Path.Combine(context.AttachmentRoot, "documents")));
            var document = Assert.Single(await context.DocumentStorage.GetDocumentsAsync());
            Assert.NotNull(document.DisabledAt);
            Assert.Empty(await context.DocumentStorage.GetPolicyDocumentsAsync(policy.Id));
        });
    }

    [Fact]
    public async Task I08_compensation_failure_never_returns_success()
    {
        await UsingContextAsync(async context =>
        {
            var policy = await context.CreatePolicyAsync("Synthetic policy");
            var source = await context.CreatePngAsync("compensation-failure.png", 0x09);
            var snapshot = await context.Validation.ValidateSourceAsync(source);
            var storage = new DelegatingDocumentStorageService(context.DocumentStorage)
            {
                FailAddPolicyDocument = true,
                FailDisableDocument = true
            };
            var fileService = new DelegatingFileAttachmentService(context.FileAttachmentService)
            {
                FailFinalDelete = true
            };

            var exception = await Record.ExceptionAsync(() =>
                context.RegisterPolicyAsync(policy.Id, source, snapshot, storage, fileService));

            Assert.IsType<AggregateException>(exception);
            Assert.Empty(await context.DocumentStorage.GetPolicyDocumentsAsync(policy.Id));
        });
    }

    [Fact]
    public async Task I09_success_leaves_zero_staging_residue()
    {
        await UsingContextAsync(async context =>
        {
            var policy = await context.CreatePolicyAsync("Synthetic policy");
            var source = await context.CreatePngAsync("no-staging.png", 0x0A);
            var snapshot = await context.Validation.ValidateSourceAsync(source);

            await context.RegisterPolicyAsync(policy.Id, source, snapshot);

            Assert.Empty(SnapshotFiles(Path.Combine(context.AttachmentRoot, "staging")));
            Assert.Single(SnapshotFiles(Path.Combine(context.AttachmentRoot, "documents")));
        });
    }

    [Fact]
    public async Task I10_failure_has_no_active_link_to_missing_payload()
    {
        await UsingContextAsync(async context =>
        {
            var policy = await context.CreatePolicyAsync("Synthetic policy");
            var source = await context.CreatePngAsync("no-missing-link.png", 0x0B);
            var snapshot = await context.Validation.ValidateSourceAsync(source);
            var storage = new DelegatingDocumentStorageService(context.DocumentStorage)
            {
                FailAddPolicyDocument = true
            };

            _ = await Record.ExceptionAsync(() =>
                context.RegisterPolicyAsync(policy.Id, source, snapshot, storage));

            Assert.Empty(await context.DocumentStorage.GetPolicyDocumentsAsync(policy.Id));
            Assert.Empty(SnapshotFiles(Path.Combine(context.AttachmentRoot, "documents")));
        });
    }

    [Fact]
    public async Task I11_legacy_document_loads_without_fabricated_metadata_or_rewrite()
    {
        await UsingContextAsync(async context =>
        {
            var legacy = await context.DocumentStorage.AddDocumentAsync(new DocumentDraft(
                "legacy.pdf",
                "Legacy synthetic",
                "pdf",
                "documents/legacy.pdf"));
            var path = Path.Combine(context.MetadataRoot, "documents.json");
            var before = Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(
                await File.ReadAllBytesAsync(path)));

            var loaded = await context.DocumentStorage.GetDocumentByIdAsync(legacy.Id);
            var after = Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(
                await File.ReadAllBytesAsync(path)));

            Assert.NotNull(loaded);
            Assert.Null(loaded.OriginalDisplayFileName);
            Assert.Null(loaded.ValidatedFileType);
            Assert.Null(loaded.ByteLength);
            Assert.Null(loaded.Sha256);
            Assert.Null(loaded.ReferenceDate);
            Assert.Null(loaded.DocumentType);
            Assert.Equal(before, after);
        });
    }

    [Fact]
    public async Task I12_registration_uses_only_injected_temp_runtime_root()
    {
        var projectRoot = FindProjectRoot();
        var projectAttachmentsBefore = SnapshotFiles(Path.Combine(projectRoot, "attachments"));
        var projectDataBefore = SnapshotFiles(Path.Combine(projectRoot, "data", "local"));

        await UsingContextAsync(async context =>
        {
            var policy = await context.CreatePolicyAsync("Synthetic policy");
            var source = await context.CreatePngAsync("temp-only.png", 0x0C);
            var snapshot = await context.Validation.ValidateSourceAsync(source);

            await context.RegisterPolicyAsync(policy.Id, source, snapshot);

            var expectedRoot = $"{Path.GetFullPath(context.Root)}{Path.DirectorySeparatorChar}";
            Assert.All(
                SnapshotFiles(context.Root),
                path => Assert.StartsWith(
                    expectedRoot,
                    Path.GetFullPath(path),
                    StringComparison.OrdinalIgnoreCase));
        });

        Assert.Equal(projectAttachmentsBefore, SnapshotFiles(Path.Combine(projectRoot, "attachments")));
        Assert.Equal(projectDataBefore, SnapshotFiles(Path.Combine(projectRoot, "data", "local")));
    }

    [Fact]
    public async Task I13_concurrent_identical_registration_has_one_success_and_one_duplicate()
    {
        await UsingContextAsync(async context =>
        {
            var policy = await context.CreatePolicyAsync("Synthetic policy");
            var source = await context.CreatePngAsync("concurrent.png", 0x0D);
            var snapshot = await context.Validation.ValidateSourceAsync(source);
            var workflow = context.CreateWorkflow(context.DocumentStorage, context.FileAttachmentService);
            var request = new PolicyDocumentRegistrationRequest(
                source,
                policy.Id,
                "terms",
                "Synthetic concurrent",
                ReferenceDate,
                snapshot);

            var tasks = new[]
            {
                CaptureAsync(() => workflow.RegisterPolicyDocumentAsync(request)),
                CaptureAsync(() => workflow.RegisterPolicyDocumentAsync(request))
            };
            var outcomes = await Task.WhenAll(tasks);

            Assert.Single(outcomes, outcome => outcome.Result is not null);
            var failure = Assert.Single(
                outcomes,
                outcome => outcome.Exception is not null).Exception;
            AssertRegistrationError(failure, DocumentRegistrationErrorCode.DuplicateDocument);
            Assert.Single(await context.DocumentStorage.GetDocumentsAsync());
            Assert.Single(await context.DocumentStorage.GetPolicyDocumentsAsync(policy.Id));
            Assert.Empty(SnapshotFiles(Path.Combine(context.AttachmentRoot, "staging")));
        });
    }

    private static async Task<RegistrationOutcome> CaptureAsync(
        Func<Task<PolicyDocumentRegistrationResult>> action)
    {
        try
        {
            return new RegistrationOutcome(await action(), null);
        }
        catch (Exception exception)
        {
            return new RegistrationOutcome(null, exception);
        }
    }

    private static void AssertRegistrationError(
        Exception? exception,
        DocumentRegistrationErrorCode expectedCode)
    {
        var registrationException = Assert.IsType<DocumentRegistrationException>(exception);
        Assert.Equal(expectedCode, registrationException.ErrorCode);
    }

    private static byte[] PngBytes(byte marker) =>
    [
        0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, marker
    ];

    private static string[] SnapshotFiles(string path)
    {
        return Directory.Exists(path)
            ? Directory.GetFiles(path, "*", SearchOption.AllDirectories)
            : [];
    }

    private static string FindProjectRoot()
    {
        var current = new DirectoryInfo(AppContext.BaseDirectory);
        while (current is not null)
        {
            if (File.Exists(Path.Combine(current.FullName, "FamilyClaimRef.sln")))
            {
                return current.FullName;
            }

            current = current.Parent;
        }

        throw new InvalidOperationException("Project root was not found.");
    }

    private static async Task UsingContextAsync(Func<Gate8Context, Task> action)
    {
        var context = new Gate8Context();
        try
        {
            await action(context);
        }
        finally
        {
            context.Dispose();
        }
    }

    private sealed class Gate8Context : IDisposable
    {
        public Gate8Context()
        {
            Root = Path.Combine(
                Path.GetTempPath(),
                "FamilyClaimRef",
                "Gate8",
                $"gate8-validation-{Guid.NewGuid():N}");
            InputRoot = Path.Combine(Root, "input");
            var runtimeRoot = Path.Combine(Root, "runtime");
            MetadataRoot = Path.Combine(runtimeRoot, "data", "local");
            AttachmentRoot = Path.Combine(runtimeRoot, "attachments");
            Directory.CreateDirectory(InputRoot);
            DocumentStorage = new JsonDocumentStorageService(MetadataRoot);
            PolicyClaimStorage = new JsonPolicyClaimStorageService(MetadataRoot);
            FileAttachmentService = new LocalFileAttachmentService(AttachmentRoot);
            Validation = new DocumentFileValidationService();
        }

        public string Root { get; }

        public string InputRoot { get; }

        public string MetadataRoot { get; }

        public string AttachmentRoot { get; }

        public JsonDocumentStorageService DocumentStorage { get; }

        public JsonPolicyClaimStorageService PolicyClaimStorage { get; }

        public LocalFileAttachmentService FileAttachmentService { get; }

        public DocumentFileValidationService Validation { get; }

        public Task<PolicyRecord> CreatePolicyAsync(string title)
        {
            return PolicyClaimStorage.AddPolicyAsync(new PolicyDraft(title, ReferenceDate));
        }

        public async Task<string> CreatePngAsync(string fileName, byte marker)
        {
            var path = Path.Combine(InputRoot, fileName);
            await File.WriteAllBytesAsync(path, PngBytes(marker));
            return path;
        }

        public string GetAttachmentPath(string relativePath)
        {
            return Path.Combine(AttachmentRoot, relativePath.Replace('/', Path.DirectorySeparatorChar));
        }

        public Task<PolicyDocumentRegistrationResult> RegisterPolicyAsync(
            string policyId,
            string sourcePath,
            DocumentFileValidationResult snapshot,
            IDocumentStorageService? storage = null,
            IFileAttachmentService? fileService = null)
        {
            var workflow = CreateWorkflow(
                storage ?? DocumentStorage,
                fileService ?? FileAttachmentService);
            return workflow.RegisterPolicyDocumentAsync(new PolicyDocumentRegistrationRequest(
                sourcePath,
                policyId,
                "terms",
                "Synthetic document",
                ReferenceDate,
                snapshot));
        }

        public DocumentRegistrationWorkflow CreateWorkflow(
            IDocumentStorageService storage,
            IFileAttachmentService fileService)
        {
            var coordinator = new DocumentAttachmentCoordinator(storage, fileService, Validation);
            var linkCoordinator = new DocumentLinkCoordinator(storage, PolicyClaimStorage);
            return new DocumentRegistrationWorkflow(
                coordinator,
                linkCoordinator,
                storage,
                fileService,
                PolicyClaimStorage);
        }

        public void Dispose()
        {
            if (Directory.Exists(Root))
            {
                Directory.Delete(Root, recursive: true);
            }
        }
    }

    private sealed class DelegatingDocumentStorageService(IDocumentStorageService inner)
        : IDocumentStorageService
    {
        public bool FailAddDocument { get; init; }

        public bool FailAddPolicyDocument { get; init; }

        public bool FailDisableDocument { get; init; }

        public Task<IReadOnlyList<DocumentRecord>> GetDocumentsAsync(
            CancellationToken cancellationToken = default) =>
            inner.GetDocumentsAsync(cancellationToken);

        public Task<DocumentRecord?> GetDocumentByIdAsync(
            string documentId,
            CancellationToken cancellationToken = default) =>
            inner.GetDocumentByIdAsync(documentId, cancellationToken);

        public Task<DocumentRecord> AddDocumentAsync(
            DocumentDraft draft,
            CancellationToken cancellationToken = default) =>
            FailAddDocument
                ? Task.FromException<DocumentRecord>(new InvalidOperationException("Synthetic Document failure."))
                : inner.AddDocumentAsync(draft, cancellationToken);

        public Task DisableDocumentAsync(
            string documentId,
            DateTimeOffset disabledAt,
            CancellationToken cancellationToken = default) =>
            FailDisableDocument
                ? Task.FromException(new InvalidOperationException("Synthetic disable failure."))
                : inner.DisableDocumentAsync(documentId, disabledAt, cancellationToken);

        public Task<IReadOnlyList<PolicyDocumentRecord>> GetPolicyDocumentsAsync(
            string policyId,
            CancellationToken cancellationToken = default) =>
            inner.GetPolicyDocumentsAsync(policyId, cancellationToken);

        public Task<PolicyDocumentRecord> AddPolicyDocumentAsync(
            PolicyDocumentDraft draft,
            CancellationToken cancellationToken = default) =>
            FailAddPolicyDocument
                ? Task.FromException<PolicyDocumentRecord>(
                    new InvalidOperationException("Synthetic policy link failure."))
                : inner.AddPolicyDocumentAsync(draft, cancellationToken);

        public Task DisablePolicyDocumentAsync(
            string policyDocumentId,
            DateTimeOffset disabledAt,
            CancellationToken cancellationToken = default) =>
            inner.DisablePolicyDocumentAsync(policyDocumentId, disabledAt, cancellationToken);

        public Task<IReadOnlyList<ClaimDocumentRecord>> GetClaimDocumentsAsync(
            string claimId,
            CancellationToken cancellationToken = default) =>
            inner.GetClaimDocumentsAsync(claimId, cancellationToken);

        public Task<ClaimDocumentRecord> AddClaimDocumentAsync(
            ClaimDocumentDraft draft,
            CancellationToken cancellationToken = default) =>
            inner.AddClaimDocumentAsync(draft, cancellationToken);

        public Task DisableClaimDocumentAsync(
            string claimDocumentId,
            DateTimeOffset disabledAt,
            CancellationToken cancellationToken = default) =>
            inner.DisableClaimDocumentAsync(claimDocumentId, disabledAt, cancellationToken);

        public Task<bool> ActiveTargetDocumentWithSha256ExistsAsync(
            string targetKind,
            string targetId,
            string sha256,
            CancellationToken cancellationToken = default) =>
            inner.ActiveTargetDocumentWithSha256ExistsAsync(
                targetKind,
                targetId,
                sha256,
                cancellationToken);
    }

    private sealed class DelegatingFileAttachmentService(IFileAttachmentService inner)
        : IFileAttachmentService
    {
        public bool FailFinalDelete { get; init; }

        public Task<StagedFileAttachment> StageDocumentFileAsync(
            string sourceFilePath,
            CancellationToken cancellationToken = default) =>
            inner.StageDocumentFileAsync(sourceFilePath, cancellationToken);

        public Task<FileAttachmentCopyResult> FinalizeStagedDocumentFileAsync(
            StagedFileAttachment stagedFile,
            string physicalFileName,
            CancellationToken cancellationToken = default) =>
            inner.FinalizeStagedDocumentFileAsync(stagedFile, physicalFileName, cancellationToken);

        public Task DeleteStagedFileIfExistsAsync(
            StagedFileAttachment stagedFile,
            CancellationToken cancellationToken = default) =>
            inner.DeleteStagedFileIfExistsAsync(stagedFile, cancellationToken);

        public Task<FileAttachmentCopyResult> CopyDocumentFileAsync(
            string sourceFilePath,
            string physicalFileName,
            CancellationToken cancellationToken = default) =>
            inner.CopyDocumentFileAsync(sourceFilePath, physicalFileName, cancellationToken);

        public Task DeleteDocumentFileIfExistsAsync(
            string relativePath,
            CancellationToken cancellationToken = default) =>
            FailFinalDelete
                ? Task.FromException(new IOException("Synthetic payload cleanup failure."))
                : inner.DeleteDocumentFileIfExistsAsync(relativePath, cancellationToken);

        public Task<bool> DocumentFileExistsAsync(
            string relativePath,
            CancellationToken cancellationToken = default) =>
            inner.DocumentFileExistsAsync(relativePath, cancellationToken);
    }

    private sealed record RegistrationOutcome(
        PolicyDocumentRegistrationResult? Result,
        Exception? Exception);
}
