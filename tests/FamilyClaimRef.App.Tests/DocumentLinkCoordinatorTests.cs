using FamilyClaimRef.App.Models.Storage;
using FamilyClaimRef.App.Services.Storage;
using Xunit;

namespace FamilyClaimRef.App.Tests;

public sealed class DocumentLinkCoordinatorTests
{
    [Fact]
    public void Constructor_rejects_null_document_storage_service()
    {
        var exception = Record.Exception(() => new DocumentLinkCoordinator(null!));

        Assert.NotNull(exception);
        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public async Task LinkPolicyDocumentAsync_rejects_null_request()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var coordinator = new DocumentLinkCoordinator(new JsonDocumentStorageService(rootPath));

            var exception = await Record.ExceptionAsync(() => coordinator.LinkPolicyDocumentAsync(null!));

            Assert.NotNull(exception);
            Assert.IsType<ArgumentNullException>(exception);
        });
    }

    [Fact]
    public async Task LinkClaimDocumentAsync_rejects_null_request()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var coordinator = new DocumentLinkCoordinator(new JsonDocumentStorageService(rootPath));

            var exception = await Record.ExceptionAsync(() => coordinator.LinkClaimDocumentAsync(null!));

            Assert.NotNull(exception);
            Assert.IsType<ArgumentNullException>(exception);
        });
    }

    [Theory]
    [InlineData(null, "doc_001", "terms")]
    [InlineData("", "doc_001", "terms")]
    [InlineData("   ", "doc_001", "terms")]
    [InlineData("policy_001", null, "terms")]
    [InlineData("policy_001", "", "terms")]
    [InlineData("policy_001", "   ", "terms")]
    [InlineData("policy_001", "doc_001", null)]
    [InlineData("policy_001", "doc_001", "")]
    [InlineData("policy_001", "doc_001", "   ")]
    public async Task LinkPolicyDocumentAsync_rejects_required_values(
        string? policyId,
        string? documentId,
        string? documentType)
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var coordinator = new DocumentLinkCoordinator(new JsonDocumentStorageService(rootPath));

            var exception = await Record.ExceptionAsync(() => coordinator.LinkPolicyDocumentAsync(
                new PolicyDocumentLinkRequest(policyId!, documentId!, documentType!)));

            Assert.NotNull(exception);
            Assert.IsType<ArgumentException>(exception);
        });
    }

    [Theory]
    [InlineData(null, "doc_001", "receipt")]
    [InlineData("", "doc_001", "receipt")]
    [InlineData("   ", "doc_001", "receipt")]
    [InlineData("claim_001", null, "receipt")]
    [InlineData("claim_001", "", "receipt")]
    [InlineData("claim_001", "   ", "receipt")]
    [InlineData("claim_001", "doc_001", null)]
    [InlineData("claim_001", "doc_001", "")]
    [InlineData("claim_001", "doc_001", "   ")]
    public async Task LinkClaimDocumentAsync_rejects_required_values(
        string? claimId,
        string? documentId,
        string? documentType)
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var coordinator = new DocumentLinkCoordinator(new JsonDocumentStorageService(rootPath));

            var exception = await Record.ExceptionAsync(() => coordinator.LinkClaimDocumentAsync(
                new ClaimDocumentLinkRequest(claimId!, documentId!, documentType!)));

            Assert.NotNull(exception);
            Assert.IsType<ArgumentException>(exception);
        });
    }

    [Fact]
    public async Task LinkPolicyDocumentAsync_links_existing_active_document_to_policy()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var storage = new JsonDocumentStorageService(rootPath);
            var document = await storage.AddDocumentAsync(CreateDocumentDraft());
            var coordinator = new DocumentLinkCoordinator(storage);

            var result = await coordinator.LinkPolicyDocumentAsync(new PolicyDocumentLinkRequest(
                "policy_001",
                document.Id,
                "terms"));

            Assert.NotNull(result.PolicyDocument);
            Assert.Equal("policy_001", result.PolicyDocument.PolicyId);
            Assert.Equal(document.Id, result.PolicyDocument.DocumentId);
            Assert.Equal("terms", result.PolicyDocument.DocumentType);
            Assert.Null(result.PolicyDocument.DisabledAt);
        });
    }

    [Fact]
    public async Task LinkClaimDocumentAsync_links_existing_active_document_to_claim()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var storage = new JsonDocumentStorageService(rootPath);
            var document = await storage.AddDocumentAsync(CreateDocumentDraft());
            var coordinator = new DocumentLinkCoordinator(storage);

            var result = await coordinator.LinkClaimDocumentAsync(new ClaimDocumentLinkRequest(
                "claim_001",
                document.Id,
                "receipt"));

            Assert.NotNull(result.ClaimDocument);
            Assert.Equal("claim_001", result.ClaimDocument.ClaimId);
            Assert.Equal(document.Id, result.ClaimDocument.DocumentId);
            Assert.Equal("receipt", result.ClaimDocument.DocumentType);
            Assert.Null(result.ClaimDocument.DisabledAt);
        });
    }

    [Fact]
    public async Task LinkPolicyDocumentAsync_passes_missing_document_validation_to_storage()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var coordinator = new DocumentLinkCoordinator(new JsonDocumentStorageService(rootPath));

            var exception = await Record.ExceptionAsync(() => coordinator.LinkPolicyDocumentAsync(
                new PolicyDocumentLinkRequest("policy_001", "doc_missing", "terms")));

            Assert.NotNull(exception);
            Assert.IsType<InvalidOperationException>(exception);
        });
    }

    [Fact]
    public async Task LinkClaimDocumentAsync_passes_missing_document_validation_to_storage()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var coordinator = new DocumentLinkCoordinator(new JsonDocumentStorageService(rootPath));

            var exception = await Record.ExceptionAsync(() => coordinator.LinkClaimDocumentAsync(
                new ClaimDocumentLinkRequest("claim_001", "doc_missing", "receipt")));

            Assert.NotNull(exception);
            Assert.IsType<InvalidOperationException>(exception);
        });
    }

    [Fact]
    public async Task LinkPolicyDocumentAsync_passes_disabled_document_validation_to_storage()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var storage = new JsonDocumentStorageService(rootPath);
            var document = await storage.AddDocumentAsync(CreateDocumentDraft());
            await storage.DisableDocumentAsync(document.Id, DateTimeOffset.UtcNow);
            var coordinator = new DocumentLinkCoordinator(storage);

            var exception = await Record.ExceptionAsync(() => coordinator.LinkPolicyDocumentAsync(
                new PolicyDocumentLinkRequest("policy_001", document.Id, "terms")));

            Assert.NotNull(exception);
            Assert.IsType<InvalidOperationException>(exception);
        });
    }

    [Fact]
    public async Task LinkClaimDocumentAsync_passes_disabled_document_validation_to_storage()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var storage = new JsonDocumentStorageService(rootPath);
            var document = await storage.AddDocumentAsync(CreateDocumentDraft());
            await storage.DisableDocumentAsync(document.Id, DateTimeOffset.UtcNow);
            var coordinator = new DocumentLinkCoordinator(storage);

            var exception = await Record.ExceptionAsync(() => coordinator.LinkClaimDocumentAsync(
                new ClaimDocumentLinkRequest("claim_001", document.Id, "receipt")));

            Assert.NotNull(exception);
            Assert.IsType<InvalidOperationException>(exception);
        });
    }

    [Fact]
    public async Task LinkPolicyDocumentAsync_accepts_policy_capture_document_type()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var storage = new JsonDocumentStorageService(rootPath);
            var document = await storage.AddDocumentAsync(CreateDocumentDraft());
            var coordinator = new DocumentLinkCoordinator(storage);

            var result = await coordinator.LinkPolicyDocumentAsync(new PolicyDocumentLinkRequest(
                "policy_001",
                document.Id,
                "capture"));

            Assert.Equal("capture", result.PolicyDocument.DocumentType);
        });
    }

    [Fact]
    public async Task LinkClaimDocumentAsync_rejects_claim_capture_document_type()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var storage = new JsonDocumentStorageService(rootPath);
            var document = await storage.AddDocumentAsync(CreateDocumentDraft());
            var coordinator = new DocumentLinkCoordinator(storage);

            var exception = await Record.ExceptionAsync(() => coordinator.LinkClaimDocumentAsync(
                new ClaimDocumentLinkRequest("claim_001", document.Id, "capture")));

            Assert.NotNull(exception);
            Assert.IsType<ArgumentException>(exception);
        });
    }

    [Fact]
    public async Task LinkPolicyDocumentAsync_rejects_invalid_document_type()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var storage = new JsonDocumentStorageService(rootPath);
            var document = await storage.AddDocumentAsync(CreateDocumentDraft());
            var coordinator = new DocumentLinkCoordinator(storage);

            var exception = await Record.ExceptionAsync(() => coordinator.LinkPolicyDocumentAsync(
                new PolicyDocumentLinkRequest("policy_001", document.Id, "receipt")));

            Assert.NotNull(exception);
            Assert.IsType<ArgumentException>(exception);
        });
    }

    [Fact]
    public async Task LinkClaimDocumentAsync_rejects_invalid_document_type()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var storage = new JsonDocumentStorageService(rootPath);
            var document = await storage.AddDocumentAsync(CreateDocumentDraft());
            var coordinator = new DocumentLinkCoordinator(storage);

            var exception = await Record.ExceptionAsync(() => coordinator.LinkClaimDocumentAsync(
                new ClaimDocumentLinkRequest("claim_001", document.Id, "terms")));

            Assert.NotNull(exception);
            Assert.IsType<ArgumentException>(exception);
        });
    }

    [Fact]
    public async Task LinkPolicyDocumentAsync_rejects_duplicate_active_link()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var storage = new JsonDocumentStorageService(rootPath);
            var document = await storage.AddDocumentAsync(CreateDocumentDraft());
            var coordinator = new DocumentLinkCoordinator(storage);
            await coordinator.LinkPolicyDocumentAsync(new PolicyDocumentLinkRequest("policy_001", document.Id, "terms"));

            var exception = await Record.ExceptionAsync(() => coordinator.LinkPolicyDocumentAsync(
                new PolicyDocumentLinkRequest("policy_001", document.Id, "capture")));

            Assert.NotNull(exception);
            Assert.IsType<InvalidOperationException>(exception);
        });
    }

    [Fact]
    public async Task LinkClaimDocumentAsync_rejects_duplicate_active_link()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var storage = new JsonDocumentStorageService(rootPath);
            var document = await storage.AddDocumentAsync(CreateDocumentDraft());
            var coordinator = new DocumentLinkCoordinator(storage);
            await coordinator.LinkClaimDocumentAsync(new ClaimDocumentLinkRequest("claim_001", document.Id, "receipt"));

            var exception = await Record.ExceptionAsync(() => coordinator.LinkClaimDocumentAsync(
                new ClaimDocumentLinkRequest("claim_001", document.Id, "medicine")));

            Assert.NotNull(exception);
            Assert.IsType<InvalidOperationException>(exception);
        });
    }

    [Fact]
    public async Task LinkPolicyDocumentAsync_excludes_disabled_link_from_duplicate_check()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var storage = new JsonDocumentStorageService(rootPath);
            var document = await storage.AddDocumentAsync(CreateDocumentDraft());
            var firstLink = await storage.AddPolicyDocumentAsync(new PolicyDocumentDraft(
                "policy_001",
                document.Id,
                "terms"));
            await storage.DisablePolicyDocumentAsync(firstLink.Id, DateTimeOffset.UtcNow);
            var coordinator = new DocumentLinkCoordinator(storage);

            var result = await coordinator.LinkPolicyDocumentAsync(new PolicyDocumentLinkRequest(
                "policy_001",
                document.Id,
                "terms"));

            Assert.NotNull(result.PolicyDocument);
            Assert.NotEqual(firstLink.Id, result.PolicyDocument.Id);
            Assert.Null(result.PolicyDocument.DisabledAt);
        });
    }

    [Fact]
    public async Task LinkClaimDocumentAsync_excludes_disabled_link_from_duplicate_check()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var storage = new JsonDocumentStorageService(rootPath);
            var document = await storage.AddDocumentAsync(CreateDocumentDraft());
            var firstLink = await storage.AddClaimDocumentAsync(new ClaimDocumentDraft(
                "claim_001",
                document.Id,
                "receipt"));
            await storage.DisableClaimDocumentAsync(firstLink.Id, DateTimeOffset.UtcNow);
            var coordinator = new DocumentLinkCoordinator(storage);

            var result = await coordinator.LinkClaimDocumentAsync(new ClaimDocumentLinkRequest(
                "claim_001",
                document.Id,
                "receipt"));

            Assert.NotNull(result.ClaimDocument);
            Assert.NotEqual(firstLink.Id, result.ClaimDocument.Id);
            Assert.Null(result.ClaimDocument.DisabledAt);
        });
    }

    [Fact]
    public async Task LinkPolicyDocumentAsync_allows_same_document_for_different_policy()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var storage = new JsonDocumentStorageService(rootPath);
            var document = await storage.AddDocumentAsync(CreateDocumentDraft());
            var coordinator = new DocumentLinkCoordinator(storage);
            await coordinator.LinkPolicyDocumentAsync(new PolicyDocumentLinkRequest("policy_001", document.Id, "terms"));

            var result = await coordinator.LinkPolicyDocumentAsync(new PolicyDocumentLinkRequest(
                "policy_002",
                document.Id,
                "terms"));

            Assert.Equal("policy_002", result.PolicyDocument.PolicyId);
            Assert.Equal(document.Id, result.PolicyDocument.DocumentId);
        });
    }

    [Fact]
    public async Task LinkClaimDocumentAsync_allows_same_document_for_different_claim()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var storage = new JsonDocumentStorageService(rootPath);
            var document = await storage.AddDocumentAsync(CreateDocumentDraft());
            var coordinator = new DocumentLinkCoordinator(storage);
            await coordinator.LinkClaimDocumentAsync(new ClaimDocumentLinkRequest("claim_001", document.Id, "receipt"));

            var result = await coordinator.LinkClaimDocumentAsync(new ClaimDocumentLinkRequest(
                "claim_002",
                document.Id,
                "receipt"));

            Assert.Equal("claim_002", result.ClaimDocument.ClaimId);
            Assert.Equal(document.Id, result.ClaimDocument.DocumentId);
        });
    }

    [Fact]
    public async Task Coordinator_tests_do_not_create_project_root_attachment_or_data_files()
    {
        var projectRoot = FindProjectRoot();
        var attachmentsBefore = SnapshotFiles(Path.Combine(projectRoot, "attachments"));
        var dataLocalBefore = SnapshotFiles(Path.Combine(projectRoot, "data", "local"));

        await UsingTempRootAsync(async rootPath =>
        {
            var storage = new JsonDocumentStorageService(rootPath);
            var document = await storage.AddDocumentAsync(CreateDocumentDraft());
            var coordinator = new DocumentLinkCoordinator(storage);

            await coordinator.LinkPolicyDocumentAsync(new PolicyDocumentLinkRequest("policy_001", document.Id, "terms"));
            await coordinator.LinkClaimDocumentAsync(new ClaimDocumentLinkRequest("claim_001", document.Id, "receipt"));
        });

        var attachmentsAfter = SnapshotFiles(Path.Combine(projectRoot, "attachments"));
        var dataLocalAfter = SnapshotFiles(Path.Combine(projectRoot, "data", "local"));
        Assert.Equal(attachmentsBefore, attachmentsAfter);
        Assert.Equal(dataLocalBefore, dataLocalAfter);
    }

    private static DocumentDraft CreateDocumentDraft()
    {
        return new DocumentDraft(
            "claim-document-20260701-receipt_001.pdf",
            "Document A",
            "pdf",
            "documents/claim-document-20260701-receipt_001.pdf");
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
