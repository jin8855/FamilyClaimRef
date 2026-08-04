using FamilyClaimRef.App.Services.Storage;
using Xunit;

namespace FamilyClaimRef.App.Tests;

public sealed class DocumentFileValidationServiceTests
{
    [Fact]
    public async Task U09_allowed_extensions_are_accepted_case_insensitively()
    {
        await UsingGate8RootAsync(async root =>
        {
            var service = new DocumentFileValidationService();
            var cases = new[]
            {
                ("sample.PDF", PdfBytes()),
                ("sample.JpG", JpegBytes(0x01)),
                ("sample.JPEG", JpegBytes(0x02)),
                ("sample.PnG", PngBytes(0x03))
            };

            foreach (var (fileName, bytes) in cases)
            {
                var path = await WriteBytesAsync(root, fileName, bytes);
                var result = await service.ValidateSourceAsync(path);
                Assert.Contains(result.NormalizedExtension, new[] { "pdf", "jpg", "jpeg", "png" });
                Assert.Equal(64, result.Sha256.Length);
            }
        });
    }

    [Fact]
    public async Task U10_unsupported_extension_is_rejected()
    {
        await UsingGate8RootAsync(async root =>
        {
            var path = await WriteBytesAsync(root, "sample.txt", PdfBytes());

            var exception = await Record.ExceptionAsync(() =>
                new DocumentFileValidationService().ValidateSourceAsync(path));

            AssertRegistrationError(exception, DocumentRegistrationErrorCode.UnsupportedFileType);
        });
    }

    [Fact]
    public async Task U11_zero_byte_file_is_rejected()
    {
        await UsingGate8RootAsync(async root =>
        {
            var path = await WriteBytesAsync(root, "empty.pdf", []);

            var exception = await Record.ExceptionAsync(() =>
                new DocumentFileValidationService().ValidateSourceAsync(path));

            AssertRegistrationError(exception, DocumentRegistrationErrorCode.EmptyFile);
        });
    }

    [Fact]
    public async Task U12_exact_25_mib_boundary_is_accepted()
    {
        await UsingGate8RootAsync(async root =>
        {
            var path = Path.Combine(root, "boundary.pdf");
            await using (var stream = new FileStream(path, FileMode.CreateNew, FileAccess.Write, FileShare.None))
            {
                stream.SetLength(DocumentFileValidationService.MaximumFileSizeBytes);
                await stream.WriteAsync(PdfBytes());
            }

            var result = await new DocumentFileValidationService().ValidateSourceAsync(path);

            Assert.Equal(DocumentFileValidationService.MaximumFileSizeBytes, result.ByteLength);
        });
    }

    [Fact]
    public async Task U13_above_25_mib_boundary_is_rejected()
    {
        await UsingGate8RootAsync(async root =>
        {
            var path = Path.Combine(root, "above-boundary.pdf");
            await using (var stream = new FileStream(path, FileMode.CreateNew, FileAccess.Write, FileShare.None))
            {
                stream.SetLength(DocumentFileValidationService.MaximumFileSizeBytes + 1);
                await stream.WriteAsync(PdfBytes());
            }

            var exception = await Record.ExceptionAsync(() =>
                new DocumentFileValidationService().ValidateSourceAsync(path));

            AssertRegistrationError(exception, DocumentRegistrationErrorCode.FileTooLarge);
        });
    }

    [Fact]
    public async Task U14_pdf_jpeg_and_png_signatures_are_validated()
    {
        await UsingGate8RootAsync(async root =>
        {
            var service = new DocumentFileValidationService();
            var pdf = await service.ValidateSourceAsync(
                await WriteBytesAsync(root, "valid.pdf", PdfBytes()));
            var jpeg = await service.ValidateSourceAsync(
                await WriteBytesAsync(root, "valid.jpeg", JpegBytes(0x04)));
            var png = await service.ValidateSourceAsync(
                await WriteBytesAsync(root, "valid.png", PngBytes(0x05)));

            Assert.Equal("PDF", pdf.ValidatedFileType);
            Assert.Equal("JPEG", jpeg.ValidatedFileType);
            Assert.Equal("PNG", png.ValidatedFileType);
        });
    }

    [Fact]
    public async Task U15_extension_and_signature_mismatch_is_rejected()
    {
        await UsingGate8RootAsync(async root =>
        {
            var path = await WriteBytesAsync(root, "renamed.pdf", PngBytes(0x06));

            var exception = await Record.ExceptionAsync(() =>
                new DocumentFileValidationService().ValidateSourceAsync(path));

            AssertRegistrationError(exception, DocumentRegistrationErrorCode.UnsupportedFileType);
        });
    }

    [Fact]
    public async Task U16_missing_locked_and_reparse_boundaries_are_rejected_or_guarded()
    {
        await UsingGate8RootAsync(async root =>
        {
            var service = new DocumentFileValidationService();
            var missing = await Record.ExceptionAsync(() =>
                service.ValidateSourceAsync(Path.Combine(root, "missing.pdf")));
            AssertRegistrationError(missing, DocumentRegistrationErrorCode.SourceUnavailable);

            var lockedPath = await WriteBytesAsync(root, "locked.pdf", PdfBytes());
            await using var locked = new FileStream(
                lockedPath,
                FileMode.Open,
                FileAccess.ReadWrite,
                FileShare.None);
            var lockedException = await Record.ExceptionAsync(() =>
                service.ValidateSourceAsync(lockedPath));
            AssertRegistrationError(lockedException, DocumentRegistrationErrorCode.SourceUnavailable);

            var targetPath = await WriteBytesAsync(root, "reparse-target.pdf", PdfBytes());
            var targetAttributes = File.GetAttributes(targetPath);
            Assert.False(targetAttributes.HasFlag(FileAttributes.ReparsePoint));

            var linkPath = Path.Combine(root, "reparse-link.pdf");
            var link = File.CreateSymbolicLink(linkPath, targetPath);

            Assert.True(File.Exists(linkPath));
            Assert.True(File.GetAttributes(linkPath).HasFlag(FileAttributes.ReparsePoint));
            var resolvedTarget = File.ResolveLinkTarget(linkPath, returnFinalTarget: false);
            Assert.NotNull(resolvedTarget);
            Assert.Equal(
                Path.GetFullPath(targetPath),
                Path.GetFullPath(resolvedTarget.FullName),
                ignoreCase: true);
            Assert.Equal(
                Path.GetFullPath(linkPath),
                Path.GetFullPath(link.FullName),
                ignoreCase: true);

            var reparseException = await Record.ExceptionAsync(() =>
                service.ValidateSourceAsync(linkPath));
            var registrationException =
                Assert.IsType<DocumentRegistrationException>(reparseException);
            Assert.Equal(
                DocumentRegistrationErrorCode.SourceUnavailable,
                registrationException.ErrorCode);
            Assert.DoesNotContain(root, registrationException.Message, StringComparison.OrdinalIgnoreCase);
            Assert.False(Directory.Exists(Path.Combine(root, "runtime")));
        });
    }

    [Fact]
    public async Task U17_selection_sha_and_staged_sha_mismatch_requires_reselection()
    {
        await UsingGate8RootAsync(async root =>
        {
            var sourcePath = await WriteBytesAsync(root, "changed.png", PngBytes(0x07));
            var validation = new DocumentFileValidationService();
            var selection = await validation.ValidateSourceAsync(sourcePath);
            await File.WriteAllBytesAsync(sourcePath, PngBytes(0x08));
            var attachmentRoot = Path.Combine(root, "runtime", "attachments");
            var fileService = new LocalFileAttachmentService(attachmentRoot);
            var coordinator = new DocumentAttachmentCoordinator(
                new JsonDocumentStorageService(Path.Combine(root, "runtime", "data", "local")),
                fileService,
                validation);

            var exception = await Record.ExceptionAsync(() =>
                coordinator.StageDocumentAsync(new DocumentAttachmentRequest(
                    sourcePath,
                    "policy",
                    "terms",
                    "Synthetic changed payload",
                    new DateOnly(2026, 7, 24),
                    selection)));

            AssertRegistrationError(exception, DocumentRegistrationErrorCode.SourceChanged);
            Assert.Empty(SnapshotFiles(Path.Combine(attachmentRoot, "staging")));
        });
    }

    private static void AssertRegistrationError(
        Exception? exception,
        DocumentRegistrationErrorCode expectedCode)
    {
        var registrationException = Assert.IsType<DocumentRegistrationException>(exception);
        Assert.Equal(expectedCode, registrationException.ErrorCode);
    }

    private static byte[] PdfBytes() => System.Text.Encoding.ASCII.GetBytes("%PDF-1.4\nsynthetic");

    private static byte[] JpegBytes(byte marker) => [0xFF, 0xD8, 0xFF, marker, 0xD9];

    private static byte[] PngBytes(byte marker) =>
    [
        0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, marker
    ];

    private static async Task<string> WriteBytesAsync(string root, string fileName, byte[] bytes)
    {
        var path = Path.Combine(root, fileName);
        await File.WriteAllBytesAsync(path, bytes);
        return path;
    }

    private static async Task UsingGate8RootAsync(Func<string, Task> action)
    {
        var root = Path.Combine(
            Path.GetTempPath(),
            "FamilyClaimRef",
            "Gate8",
            $"gate8-validation-{Guid.NewGuid():N}");
        Directory.CreateDirectory(root);
        try
        {
            await action(root);
        }
        finally
        {
            if (Directory.Exists(root))
            {
                Directory.Delete(root, recursive: true);
            }
        }
    }

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
}
