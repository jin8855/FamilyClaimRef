using FamilyClaimRef.App.Services.Storage;
using Xunit;

namespace FamilyClaimRef.App.Tests;

public sealed class IFileAttachmentServiceTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_rejects_invalid_root_path(string? attachmentRootPath)
    {
        var exception = Record.Exception(() => new LocalFileAttachmentService(attachmentRootPath!));

        Assert.NotNull(exception);
        Assert.IsType<ArgumentException>(exception);
    }

    [Fact]
    public async Task Constructor_accepts_temp_root_path()
    {
        await UsingTempRootAsync(rootPath =>
        {
            var service = new LocalFileAttachmentService(rootPath);

            Assert.NotNull(service);

            return Task.CompletedTask;
        });
    }

    [Fact]
    public async Task CopyDocumentFileAsync_rejects_missing_source_file()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var service = new LocalFileAttachmentService(rootPath);
            var missingSourcePath = Path.Combine(rootPath, "missing-source.pdf");

            var exception = await Record.ExceptionAsync(() => service.CopyDocumentFileAsync(
                missingSourcePath,
                "claim-claim_001_20260701_receipt.pdf"));

            Assert.NotNull(exception);
            Assert.IsType<FileNotFoundException>(exception);
        });
    }

    [Fact]
    public async Task CopyDocumentFileAsync_creates_target_file_under_temp_attachment_root()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var service = new LocalFileAttachmentService(rootPath);
            var sourcePath = await CreateDummySourceFileAsync(rootPath, "source.pdf", "dummy file content");

            var result = await service.CopyDocumentFileAsync(sourcePath, "claim-claim_001_20260701_receipt.pdf");

            var targetPath = Path.Combine(rootPath, "documents", "claim-claim_001_20260701_receipt.pdf");
            Assert.True(File.Exists(targetPath));
            Assert.Equal("documents/claim-claim_001_20260701_receipt.pdf", result.RelativePath);
        });
    }

    [Fact]
    public async Task CopyDocumentFileAsync_preserves_file_content()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            const string sourceContent = "dummy file content";
            var service = new LocalFileAttachmentService(rootPath);
            var sourcePath = await CreateDummySourceFileAsync(rootPath, "source.pdf", sourceContent);

            await service.CopyDocumentFileAsync(sourcePath, "claim-claim_001_20260701_receipt.pdf");

            var targetPath = Path.Combine(rootPath, "documents", "claim-claim_001_20260701_receipt.pdf");
            var copiedContent = await File.ReadAllTextAsync(targetPath);
            Assert.Equal(sourceContent, copiedContent);
        });
    }

    [Fact]
    public async Task CopyDocumentFileAsync_returns_relative_path_not_absolute()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var service = new LocalFileAttachmentService(rootPath);
            var sourcePath = await CreateDummySourceFileAsync(rootPath, "source.pdf", "dummy file content");

            var result = await service.CopyDocumentFileAsync(sourcePath, "claim-claim_001_20260701_receipt.pdf");

            Assert.False(Path.IsPathRooted(result.RelativePath));
        });
    }

    [Fact]
    public async Task CopyDocumentFileAsync_returns_documents_relative_path()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var service = new LocalFileAttachmentService(rootPath);
            var sourcePath = await CreateDummySourceFileAsync(rootPath, "source.pdf", "dummy file content");

            var result = await service.CopyDocumentFileAsync(sourcePath, "claim-claim_001_20260701_receipt.pdf");

            Assert.StartsWith("documents/", result.RelativePath, StringComparison.Ordinal);
        });
    }

    [Fact]
    public async Task CopyDocumentFileAsync_returns_input_physical_file_name()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            const string physicalFileName = "claim-claim_001_20260701_receipt.pdf";
            var service = new LocalFileAttachmentService(rootPath);
            var sourcePath = await CreateDummySourceFileAsync(rootPath, "source.pdf", "dummy file content");

            var result = await service.CopyDocumentFileAsync(sourcePath, physicalFileName);

            Assert.Equal(physicalFileName, result.PhysicalFileName);
        });
    }

    [Fact]
    public async Task CopyDocumentFileAsync_returns_target_extension()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var service = new LocalFileAttachmentService(rootPath);
            var sourcePath = await CreateDummySourceFileAsync(rootPath, "source.tmp", "dummy file content");

            var result = await service.CopyDocumentFileAsync(sourcePath, "claim-claim_001_20260701_receipt.PDF");

            Assert.Equal("pdf", result.Extension);
        });
    }

    [Fact]
    public async Task CopyDocumentFileAsync_returns_copied_file_size()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            const string sourceContent = "dummy file content";
            var service = new LocalFileAttachmentService(rootPath);
            var sourcePath = await CreateDummySourceFileAsync(rootPath, "source.pdf", sourceContent);

            var result = await service.CopyDocumentFileAsync(sourcePath, "claim-claim_001_20260701_receipt.pdf");

            Assert.Equal(sourceContent.Length, result.SizeBytes);
        });
    }

    [Fact]
    public async Task CopyDocumentFileAsync_rejects_existing_target_file()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var service = new LocalFileAttachmentService(rootPath);
            var firstSourcePath = await CreateDummySourceFileAsync(rootPath, "source-a.pdf", "dummy file content A");
            var secondSourcePath = await CreateDummySourceFileAsync(rootPath, "source-b.pdf", "dummy file content B");
            const string physicalFileName = "claim-claim_001_20260701_receipt.pdf";
            await service.CopyDocumentFileAsync(firstSourcePath, physicalFileName);

            var exception = await Record.ExceptionAsync(() => service.CopyDocumentFileAsync(
                secondSourcePath,
                physicalFileName));

            Assert.NotNull(exception);
            Assert.IsType<IOException>(exception);
        });
    }

    [Fact]
    public async Task CopyDocumentFileAsync_rejects_physical_file_name_with_path_traversal()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var service = new LocalFileAttachmentService(rootPath);
            var sourcePath = await CreateDummySourceFileAsync(rootPath, "source.pdf", "dummy file content");

            var exception = await Record.ExceptionAsync(() => service.CopyDocumentFileAsync(
                sourcePath,
                "..claim-claim_001_20260701_receipt.pdf"));

            Assert.NotNull(exception);
            Assert.IsType<ArgumentException>(exception);
        });
    }

    [Fact]
    public async Task CopyDocumentFileAsync_rejects_physical_file_name_with_directory_separator()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var service = new LocalFileAttachmentService(rootPath);
            var sourcePath = await CreateDummySourceFileAsync(rootPath, "source.pdf", "dummy file content");

            var exception = await Record.ExceptionAsync(() => service.CopyDocumentFileAsync(
                sourcePath,
                "nested/claim-claim_001_20260701_receipt.pdf"));

            Assert.NotNull(exception);
            Assert.IsType<ArgumentException>(exception);
        });
    }

    [Fact]
    public async Task CopyDocumentFileAsync_rejects_absolute_physical_file_name()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var service = new LocalFileAttachmentService(rootPath);
            var sourcePath = await CreateDummySourceFileAsync(rootPath, "source.pdf", "dummy file content");
            var absolutePhysicalFileName = Path.Combine(rootPath, "claim-claim_001_20260701_receipt.pdf");

            var exception = await Record.ExceptionAsync(() => service.CopyDocumentFileAsync(
                sourcePath,
                absolutePhysicalFileName));

            Assert.NotNull(exception);
            Assert.IsType<ArgumentException>(exception);
        });
    }

    [Fact]
    public async Task DocumentFileExistsAsync_returns_true_for_existing_copied_file()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var service = new LocalFileAttachmentService(rootPath);
            var sourcePath = await CreateDummySourceFileAsync(rootPath, "source.pdf", "dummy file content");
            var result = await service.CopyDocumentFileAsync(sourcePath, "claim-claim_001_20260701_receipt.pdf");

            var exists = await service.DocumentFileExistsAsync(result.RelativePath);

            Assert.True(exists);
        });
    }

    [Fact]
    public async Task DocumentFileExistsAsync_returns_false_for_missing_relative_path()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var service = new LocalFileAttachmentService(rootPath);

            var exists = await service.DocumentFileExistsAsync("documents/missing.pdf");

            Assert.False(exists);
        });
    }

    [Fact]
    public async Task DocumentFileExistsAsync_rejects_absolute_relative_path()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var service = new LocalFileAttachmentService(rootPath);
            var absolutePath = Path.Combine(rootPath, "documents", "missing.pdf");

            var exception = await Record.ExceptionAsync(() => service.DocumentFileExistsAsync(absolutePath));

            Assert.NotNull(exception);
            Assert.IsType<ArgumentException>(exception);
        });
    }

    [Fact]
    public async Task DocumentFileExistsAsync_rejects_path_traversal_relative_path()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var service = new LocalFileAttachmentService(rootPath);

            var exception = await Record.ExceptionAsync(() => service.DocumentFileExistsAsync("../outside.pdf"));

            Assert.NotNull(exception);
            Assert.IsType<ArgumentException>(exception);
        });
    }

    [Fact]
    public async Task DeleteDocumentFileIfExistsAsync_removes_existing_copied_file()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var service = new LocalFileAttachmentService(rootPath);
            var sourcePath = await CreateDummySourceFileAsync(rootPath, "source.pdf", "dummy file content");
            var result = await service.CopyDocumentFileAsync(sourcePath, "claim-claim_001_20260701_receipt.pdf");

            await service.DeleteDocumentFileIfExistsAsync(result.RelativePath);

            var exists = await service.DocumentFileExistsAsync(result.RelativePath);
            Assert.False(exists);
        });
    }

    [Fact]
    public async Task DeleteDocumentFileIfExistsAsync_does_not_fail_for_missing_file()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var service = new LocalFileAttachmentService(rootPath);

            var exception = await Record.ExceptionAsync(() => service.DeleteDocumentFileIfExistsAsync("documents/missing.pdf"));

            Assert.Null(exception);
        });
    }

    [Fact]
    public async Task DeleteDocumentFileIfExistsAsync_rejects_path_traversal_relative_path()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var service = new LocalFileAttachmentService(rootPath);

            var exception = await Record.ExceptionAsync(() => service.DeleteDocumentFileIfExistsAsync("../outside.pdf"));

            Assert.NotNull(exception);
            Assert.IsType<ArgumentException>(exception);
        });
    }

    [Fact]
    public async Task Stage_and_finalize_moves_payload_from_staging_to_documents_without_residue()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var service = new LocalFileAttachmentService(rootPath);
            var sourcePath = await CreateDummySourceFileAsync(rootPath, "source.pdf", "%PDF-1.4");
            var staged = await service.StageDocumentFileAsync(sourcePath);
            var validated = staged with
            {
                Validation = new DocumentFileValidationResult(
                    "source.pdf",
                    "pdf",
                    "PDF",
                    8,
                    new string('a', 64),
                    DateTimeOffset.UtcNow)
            };

            var result = await service.FinalizeStagedDocumentFileAsync(
                validated,
                "policy-document_20260724_terms_001.pdf");

            Assert.False(File.Exists(staged.FullPath));
            Assert.True(await service.DocumentFileExistsAsync(result.RelativePath));
            Assert.Equal("documents/policy-document_20260724_terms_001.pdf", result.RelativePath);
            Assert.Empty(SnapshotFiles(Path.Combine(rootPath, "staging")));
        });
    }

    [Fact]
    public async Task Service_operations_do_not_create_project_root_attachment_or_data_files()
    {
        var projectRoot = FindProjectRoot();
        var attachmentsBefore = SnapshotFiles(Path.Combine(projectRoot, "attachments"));
        var dataLocalBefore = SnapshotFiles(Path.Combine(projectRoot, "data", "local"));

        await UsingTempRootAsync(async rootPath =>
        {
            var service = new LocalFileAttachmentService(rootPath);
            var sourcePath = await CreateDummySourceFileAsync(rootPath, "source.pdf", "dummy file content");
            var result = await service.CopyDocumentFileAsync(sourcePath, "claim-claim_001_20260701_receipt.pdf");

            Assert.True(await service.DocumentFileExistsAsync(result.RelativePath));
            await service.DeleteDocumentFileIfExistsAsync(result.RelativePath);
        });

        var attachmentsAfter = SnapshotFiles(Path.Combine(projectRoot, "attachments"));
        var dataLocalAfter = SnapshotFiles(Path.Combine(projectRoot, "data", "local"));
        Assert.Equal(attachmentsBefore, attachmentsAfter);
        Assert.Equal(dataLocalBefore, dataLocalAfter);
    }

    private static async Task<string> CreateDummySourceFileAsync(string rootPath, string fileName, string content)
    {
        var sourceDirectory = Path.Combine(rootPath, "source");
        Directory.CreateDirectory(sourceDirectory);
        var sourcePath = Path.Combine(sourceDirectory, fileName);
        await File.WriteAllTextAsync(sourcePath, content);

        return sourcePath;
    }

    private static async Task UsingTempRootAsync(Func<string, Task> action)
    {
        var rootPath = Path.Combine(Path.GetTempPath(), "FamilyClaimRef.App.Tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(rootPath);

        try
        {
            await action(rootPath);
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
}
