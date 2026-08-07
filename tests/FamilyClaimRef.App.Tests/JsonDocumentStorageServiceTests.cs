using System.Text.Json;
using FamilyClaimRef.App.Models.Storage;
using FamilyClaimRef.App.Services.Storage;
using Xunit;

namespace FamilyClaimRef.App.Tests;

public sealed class JsonDocumentStorageServiceTests
{
    [Fact]
    public async Task Missing_json_files_return_empty_lists()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var service = new JsonDocumentStorageService(rootPath);

            var documents = await service.GetDocumentsAsync();
            var policyDocuments = await service.GetPolicyDocumentsAsync("policy_001");
            var claimDocuments = await service.GetClaimDocumentsAsync("claim_001");

            Assert.Empty(documents);
            Assert.Empty(policyDocuments);
            Assert.Empty(claimDocuments);
        });
    }

    [Fact]
    public async Task AddDocumentAsync_creates_record_and_persists_to_json_file()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var service = new JsonDocumentStorageService(rootPath);
            var draft = CreateDocumentDraft();

            var record = await service.AddDocumentAsync(draft);

            Assert.StartsWith("doc_", record.Id, StringComparison.Ordinal);
            Assert.Equal(draft.PhysicalFileName, record.PhysicalFileName);
            Assert.Equal(draft.DisplayTitle, record.DisplayTitle);
            Assert.Equal(draft.Extension, record.Extension);
            Assert.Equal(draft.RelativePath, record.RelativePath);
            Assert.NotEqual(default, record.CreatedAt);
            Assert.NotEqual(default, record.UpdatedAt);
            Assert.Null(record.DisabledAt);
            Assert.True(File.Exists(Path.Combine(rootPath, "documents.json")));

            var reloadedService = new JsonDocumentStorageService(rootPath);
            var reloaded = await reloadedService.GetDocumentByIdAsync(record.Id);

            Assert.Equal(record, reloaded);
        });
    }

    [Fact]
    public async Task AddDocumentAsync_persists_complete_gate8_metadata()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var service = new JsonDocumentStorageService(rootPath);
            var draft = new DocumentDraft(
                "policy-document_20260724_terms_001.pdf",
                "Synthetic document",
                "PDF",
                "documents/policy-document_20260724_terms_001.pdf",
                "synthetic.pdf",
                "PDF",
                12,
                new string('a', 64),
                new DateOnly(2026, 7, 24),
                "terms");

            var record = await service.AddDocumentAsync(draft);
            var loaded = await service.GetDocumentByIdAsync(record.Id);

            Assert.NotNull(loaded);
            Assert.Equal("pdf", loaded.Extension);
            Assert.Equal("synthetic.pdf", loaded.OriginalDisplayFileName);
            Assert.Equal("PDF", loaded.ValidatedFileType);
            Assert.Equal(12, loaded.ByteLength);
            Assert.Equal(new string('a', 64), loaded.Sha256);
            Assert.Equal(new DateOnly(2026, 7, 24), loaded.ReferenceDate);
            Assert.Equal("terms", loaded.DocumentType);
            Assert.Equal("application/pdf", loaded.DeclaredContentType);
            Assert.False(loaded.IsDisabled);
        });
    }

    [Fact]
    public async Task GetDocumentByIdAsync_returns_null_for_missing_document()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var service = new JsonDocumentStorageService(rootPath);

            var record = await service.GetDocumentByIdAsync("doc_missing");

            Assert.Null(record);
        });
    }

    [Fact]
    public async Task DisableDocumentAsync_sets_disabledAt_and_updatedAt()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var service = new JsonDocumentStorageService(rootPath);
            var document = await service.AddDocumentAsync(CreateDocumentDraft());
            var disabledAt = DateTimeOffset.UtcNow.AddMinutes(1);

            await service.DisableDocumentAsync(document.Id, disabledAt);

            var reloadedService = new JsonDocumentStorageService(rootPath);
            var disabledDocument = await reloadedService.GetDocumentByIdAsync(document.Id);
            Assert.NotNull(disabledDocument);
            Assert.Equal(disabledAt, disabledDocument.DisabledAt);
            Assert.Equal(disabledAt, disabledDocument.UpdatedAt);
        });
    }

    [Fact]
    public async Task AddPolicyDocumentAsync_accepts_existing_active_document()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var service = new JsonDocumentStorageService(rootPath);
            var document = await service.AddDocumentAsync(CreateDocumentDraft());

            var policyDocument = await service.AddPolicyDocumentAsync(new PolicyDocumentDraft(
                "policy_001",
                document.Id,
                "terms"));

            Assert.StartsWith("pdoc_", policyDocument.Id, StringComparison.Ordinal);
            Assert.Equal("policy_001", policyDocument.PolicyId);
            Assert.Equal(document.Id, policyDocument.DocumentId);
            Assert.Equal("terms", policyDocument.DocumentType);
            Assert.Null(policyDocument.DisabledAt);
            Assert.True(File.Exists(Path.Combine(rootPath, "policy-documents.json")));
        });
    }

    [Fact]
    public async Task AddPolicyDocumentAsync_rejects_missing_document()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var service = new JsonDocumentStorageService(rootPath);

            var exception = await Record.ExceptionAsync(() => service.AddPolicyDocumentAsync(new PolicyDocumentDraft(
                "policy_001",
                "doc_missing",
                "terms")));

            Assert.NotNull(exception);
            Assert.IsType<InvalidOperationException>(exception);
        });
    }

    [Fact]
    public async Task AddPolicyDocumentAsync_rejects_disabled_document()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var service = new JsonDocumentStorageService(rootPath);
            var document = await service.AddDocumentAsync(CreateDocumentDraft());
            await service.DisableDocumentAsync(document.Id, DateTimeOffset.UtcNow);

            var exception = await Record.ExceptionAsync(() => service.AddPolicyDocumentAsync(new PolicyDocumentDraft(
                "policy_001",
                document.Id,
                "terms")));

            Assert.NotNull(exception);
            Assert.IsType<InvalidOperationException>(exception);
        });
    }

    [Fact]
    public async Task AddPolicyDocumentAsync_rejects_invalid_document_type()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var service = new JsonDocumentStorageService(rootPath);
            var document = await service.AddDocumentAsync(CreateDocumentDraft());

            var exception = await Record.ExceptionAsync(() => service.AddPolicyDocumentAsync(new PolicyDocumentDraft(
                "policy_001",
                document.Id,
                "receipt")));

            Assert.NotNull(exception);
            Assert.IsType<ArgumentException>(exception);
        });
    }

    [Fact]
    public async Task AddPolicyDocumentAsync_accepts_policy_capture_document_type()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var service = new JsonDocumentStorageService(rootPath);
            var document = await service.AddDocumentAsync(CreateDocumentDraft());

            var policyDocument = await service.AddPolicyDocumentAsync(new PolicyDocumentDraft(
                "policy_001",
                document.Id,
                "capture"));

            Assert.Equal("capture", policyDocument.DocumentType);
        });
    }

    [Fact]
    public async Task DisablePolicyDocumentAsync_sets_disabledAt_and_updatedAt()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var service = new JsonDocumentStorageService(rootPath);
            var document = await service.AddDocumentAsync(CreateDocumentDraft());
            var policyDocument = await service.AddPolicyDocumentAsync(new PolicyDocumentDraft(
                "policy_001",
                document.Id,
                "terms"));
            var disabledAt = DateTimeOffset.UtcNow.AddMinutes(2);

            await service.DisablePolicyDocumentAsync(policyDocument.Id, disabledAt);

            var reloadedPolicyDocuments = await service.GetPolicyDocumentsAsync("policy_001");
            var disabledPolicyDocument = Assert.Single(reloadedPolicyDocuments);
            Assert.Equal(disabledAt, disabledPolicyDocument.DisabledAt);
            Assert.Equal(disabledAt, disabledPolicyDocument.UpdatedAt);
        });
    }

    [Fact]
    public async Task ReplaceActivePolicyDocumentAsync_keeps_history_and_leaves_one_active_type_link()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var service = new JsonDocumentStorageService(rootPath);
            var firstDocument = await service.AddDocumentAsync(CreateDocumentDraft());
            var secondDocument = await service.AddDocumentAsync(CreateDocumentDraft() with
            {
                PhysicalFileName = "claim-claim_001_20260626_receipt_002.pdf",
                DisplayTitle = "Document B",
                RelativePath = "claims/claim_001/claim-claim_001_20260626_receipt_002.pdf"
            });
            var firstLink = await service.AddPolicyDocumentAsync(new PolicyDocumentDraft(
                "policy_001",
                firstDocument.Id,
                "terms"));

            var replacement = await service.ReplaceActivePolicyDocumentAsync(
                new PolicyDocumentDraft("policy_001", secondDocument.Id, "terms"));

            var links = await service.GetPolicyDocumentsAsync("policy_001");
            Assert.Equal(2, links.Count);
            Assert.Equal(firstLink.Id, Assert.Single(links, link => link.DisabledAt is not null).Id);
            Assert.Equal(replacement.Id, Assert.Single(links, link => link.DisabledAt is null).Id);
            Assert.Equal(secondDocument.Id, replacement.DocumentId);
            Assert.Equal(2, (await service.GetDocumentsAsync()).Count);
        });
    }

    [Fact]
    public async Task DisableActivePolicyDocumentsByTypeAsync_preserves_link_and_document_history()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var service = new JsonDocumentStorageService(rootPath);
            var document = await service.AddDocumentAsync(CreateDocumentDraft());
            await service.AddPolicyDocumentAsync(new PolicyDocumentDraft(
                "policy_001",
                document.Id,
                "terms"));
            var disabledAt = DateTimeOffset.UtcNow.AddMinutes(3);

            var disabledCount = await service.DisableActivePolicyDocumentsByTypeAsync(
                "policy_001",
                "terms",
                disabledAt);

            Assert.Equal(1, disabledCount);
            Assert.NotNull(Assert.Single(await service.GetPolicyDocumentsAsync("policy_001")).DisabledAt);
            Assert.Null((await service.GetDocumentByIdAsync(document.Id))?.DisabledAt);
        });
    }

    [Fact]
    public async Task AddClaimDocumentAsync_accepts_existing_active_document()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var service = new JsonDocumentStorageService(rootPath);
            var document = await service.AddDocumentAsync(CreateDocumentDraft());

            var claimDocument = await service.AddClaimDocumentAsync(new ClaimDocumentDraft(
                "claim_001",
                document.Id,
                "receipt"));

            Assert.StartsWith("cdoc_", claimDocument.Id, StringComparison.Ordinal);
            Assert.Equal("claim_001", claimDocument.ClaimId);
            Assert.Equal(document.Id, claimDocument.DocumentId);
            Assert.Equal("receipt", claimDocument.DocumentType);
            Assert.Null(claimDocument.DisabledAt);
            Assert.True(File.Exists(Path.Combine(rootPath, "claim-documents.json")));
        });
    }

    [Fact]
    public async Task AddClaimDocumentAsync_rejects_missing_document()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var service = new JsonDocumentStorageService(rootPath);

            var exception = await Record.ExceptionAsync(() => service.AddClaimDocumentAsync(new ClaimDocumentDraft(
                "claim_001",
                "doc_missing",
                "receipt")));

            Assert.NotNull(exception);
            Assert.IsType<InvalidOperationException>(exception);
        });
    }

    [Fact]
    public async Task AddClaimDocumentAsync_rejects_disabled_document()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var service = new JsonDocumentStorageService(rootPath);
            var document = await service.AddDocumentAsync(CreateDocumentDraft());
            await service.DisableDocumentAsync(document.Id, DateTimeOffset.UtcNow);

            var exception = await Record.ExceptionAsync(() => service.AddClaimDocumentAsync(new ClaimDocumentDraft(
                "claim_001",
                document.Id,
                "receipt")));

            Assert.NotNull(exception);
            Assert.IsType<InvalidOperationException>(exception);
        });
    }

    [Fact]
    public async Task AddClaimDocumentAsync_rejects_invalid_document_type()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var service = new JsonDocumentStorageService(rootPath);
            var document = await service.AddDocumentAsync(CreateDocumentDraft());

            var exception = await Record.ExceptionAsync(() => service.AddClaimDocumentAsync(new ClaimDocumentDraft(
                "claim_001",
                document.Id,
                "terms")));

            Assert.NotNull(exception);
            Assert.IsType<ArgumentException>(exception);
        });
    }

    [Fact]
    public async Task AddClaimDocumentAsync_rejects_claim_capture_document_type()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var service = new JsonDocumentStorageService(rootPath);
            var document = await service.AddDocumentAsync(CreateDocumentDraft());

            var exception = await Record.ExceptionAsync(() => service.AddClaimDocumentAsync(new ClaimDocumentDraft(
                "claim_001",
                document.Id,
                "capture")));

            Assert.NotNull(exception);
            Assert.IsType<ArgumentException>(exception);
        });
    }

    [Fact]
    public async Task DisableClaimDocumentAsync_sets_disabledAt_and_updatedAt()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var service = new JsonDocumentStorageService(rootPath);
            var document = await service.AddDocumentAsync(CreateDocumentDraft());
            var claimDocument = await service.AddClaimDocumentAsync(new ClaimDocumentDraft(
                "claim_001",
                document.Id,
                "receipt"));
            var disabledAt = DateTimeOffset.UtcNow.AddMinutes(3);

            await service.DisableClaimDocumentAsync(claimDocument.Id, disabledAt);

            var reloadedClaimDocuments = await service.GetClaimDocumentsAsync("claim_001");
            var disabledClaimDocument = Assert.Single(reloadedClaimDocuments);
            Assert.Equal(disabledAt, disabledClaimDocument.DisabledAt);
            Assert.Equal(disabledAt, disabledClaimDocument.UpdatedAt);
        });
    }

    [Fact]
    public async Task Saved_json_contains_schemaVersion_and_savedAt()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var service = new JsonDocumentStorageService(rootPath);

            await service.AddDocumentAsync(CreateDocumentDraft());

            var json = await File.ReadAllTextAsync(Path.Combine(rootPath, "documents.json"));
            using var document = JsonDocument.Parse(json);
            var root = document.RootElement;
            Assert.Equal(1, root.GetProperty("schemaVersion").GetInt32());
            Assert.NotEqual(default, root.GetProperty("savedAt").GetDateTimeOffset());
            Assert.Equal(JsonValueKind.Array, root.GetProperty("items").ValueKind);
        });
    }

    [Fact]
    public async Task Invalid_json_load_fails_with_invalidOperationException()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            await File.WriteAllTextAsync(Path.Combine(rootPath, "documents.json"), "{ invalid json");
            var service = new JsonDocumentStorageService(rootPath);

            var exception = await Record.ExceptionAsync(() => service.GetDocumentsAsync());

            Assert.NotNull(exception);
            Assert.IsType<InvalidOperationException>(exception);
        });
    }

    [Fact]
    public async Task Schema_version_mismatch_fails_with_invalidOperationException()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            await File.WriteAllTextAsync(
                Path.Combine(rootPath, "documents.json"),
                """
                {
                  "schemaVersion": 2,
                  "savedAt": "2026-01-01T00:00:00Z",
                  "items": []
                }
                """);
            var service = new JsonDocumentStorageService(rootPath);

            var exception = await Record.ExceptionAsync(() => service.GetDocumentsAsync());

            Assert.NotNull(exception);
            Assert.IsType<InvalidOperationException>(exception);
        });
    }

    [Fact]
    public async Task Null_items_fails_with_invalidOperationException()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            await File.WriteAllTextAsync(
                Path.Combine(rootPath, "documents.json"),
                """
                {
                  "schemaVersion": 1,
                  "savedAt": "2026-01-01T00:00:00Z",
                  "items": null
                }
                """);
            var service = new JsonDocumentStorageService(rootPath);

            var exception = await Record.ExceptionAsync(() => service.GetDocumentsAsync());

            Assert.NotNull(exception);
            Assert.IsType<InvalidOperationException>(exception);
        });
    }

    private static DocumentDraft CreateDocumentDraft()
    {
        return new DocumentDraft(
            "claim-claim_001_20260626_receipt.pdf",
            "Document A",
            "pdf",
            "claims/claim_001/claim-claim_001_20260626_receipt.pdf");
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
}
