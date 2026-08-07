using FamilyClaimRef.App.Models.Storage;
using FamilyClaimRef.App.Services.Storage;
using Xunit;

namespace FamilyClaimRef.App.Tests;

public sealed class DocumentLinkCoordinatorTests
{
    [Fact]
    public void Constructor_rejects_null_document_storage_service()
    {
        var exception = Record.Exception(() => new DocumentLinkCoordinator(
            null!,
            new FakePolicyClaimStorageService()));

        Assert.NotNull(exception);
        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void Constructor_rejects_null_policy_claim_storage_service()
    {
        var exception = Record.Exception(() => new DocumentLinkCoordinator(
            new JsonDocumentStorageService(Path.GetTempPath()),
            null!));

        Assert.NotNull(exception);
        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public async Task LinkPolicyDocumentAsync_rejects_null_request()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var coordinator = CreateCoordinator(new JsonDocumentStorageService(rootPath));

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
            var coordinator = CreateCoordinator(new JsonDocumentStorageService(rootPath));

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
            var coordinator = CreateCoordinator(new JsonDocumentStorageService(rootPath));

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
            var coordinator = CreateCoordinator(new JsonDocumentStorageService(rootPath));

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
            var coordinator = CreateCoordinator(storage);

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
            var coordinator = CreateCoordinator(storage);

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
    public async Task LinkPolicyDocumentAsync_rejects_missing_policy_before_link_is_persisted()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var storage = new JsonDocumentStorageService(rootPath);
            var document = await storage.AddDocumentAsync(CreateDocumentDraft());
            var coordinator = CreateCoordinator(
                storage,
                activePolicyIds: [],
                activeClaimIds: ["claim_001"]);

            var exception = await Record.ExceptionAsync(() => coordinator.LinkPolicyDocumentAsync(
                new PolicyDocumentLinkRequest("policy_missing", document.Id, "terms")));

            Assert.NotNull(exception);
            Assert.IsType<InvalidOperationException>(exception);
            Assert.Empty(await storage.GetPolicyDocumentsAsync("policy_missing"));
            Assert.False(File.Exists(Path.Combine(rootPath, "policy-documents.json")));
        });
    }

    [Fact]
    public async Task LinkPolicyDocumentAsync_rejects_disabled_policy_before_link_is_persisted()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var documentStorage = new JsonDocumentStorageService(rootPath);
            var policyClaimStorage = new JsonPolicyClaimStorageService(rootPath);
            var policy = await policyClaimStorage.AddPolicyAsync(CreatePolicyDraft());
            await policyClaimStorage.DisablePolicyAsync(policy.Id);
            var document = await documentStorage.AddDocumentAsync(CreateDocumentDraft());
            var coordinator = new DocumentLinkCoordinator(documentStorage, policyClaimStorage);

            var exception = await Record.ExceptionAsync(() => coordinator.LinkPolicyDocumentAsync(
                new PolicyDocumentLinkRequest(policy.Id, document.Id, "terms")));

            Assert.NotNull(exception);
            Assert.IsType<InvalidOperationException>(exception);
            Assert.Empty(await documentStorage.GetPolicyDocumentsAsync(policy.Id));
            Assert.False(File.Exists(Path.Combine(rootPath, "policy-documents.json")));
        });
    }

    [Fact]
    public async Task LinkClaimDocumentAsync_rejects_missing_claim_before_link_is_persisted()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var storage = new JsonDocumentStorageService(rootPath);
            var document = await storage.AddDocumentAsync(CreateDocumentDraft());
            var coordinator = CreateCoordinator(
                storage,
                activePolicyIds: ["policy_001"],
                activeClaimIds: []);

            var exception = await Record.ExceptionAsync(() => coordinator.LinkClaimDocumentAsync(
                new ClaimDocumentLinkRequest("claim_missing", document.Id, "receipt")));

            Assert.NotNull(exception);
            Assert.IsType<InvalidOperationException>(exception);
            Assert.Empty(await storage.GetClaimDocumentsAsync("claim_missing"));
            Assert.False(File.Exists(Path.Combine(rootPath, "claim-documents.json")));
        });
    }

    [Fact]
    public async Task LinkClaimDocumentAsync_rejects_disabled_claim_before_link_is_persisted()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var documentStorage = new JsonDocumentStorageService(rootPath);
            var policyClaimStorage = new JsonPolicyClaimStorageService(rootPath);
            var policy = await policyClaimStorage.AddPolicyAsync(CreatePolicyDraft());
            var claim = await policyClaimStorage.AddClaimAsync(CreateClaimDraft(policy.Id));
            await policyClaimStorage.DisableClaimAsync(claim.Id, claim.Revision);
            var document = await documentStorage.AddDocumentAsync(CreateDocumentDraft());
            var coordinator = new DocumentLinkCoordinator(documentStorage, policyClaimStorage);

            var exception = await Record.ExceptionAsync(() => coordinator.LinkClaimDocumentAsync(
                new ClaimDocumentLinkRequest(claim.Id, document.Id, "receipt")));

            Assert.NotNull(exception);
            Assert.IsType<InvalidOperationException>(exception);
            Assert.Empty(await documentStorage.GetClaimDocumentsAsync(claim.Id));
            Assert.False(File.Exists(Path.Combine(rootPath, "claim-documents.json")));
        });
    }

    [Fact]
    public async Task LinkPolicyDocumentAsync_passes_missing_document_validation_to_storage()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var coordinator = CreateCoordinator(new JsonDocumentStorageService(rootPath));

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
            var coordinator = CreateCoordinator(new JsonDocumentStorageService(rootPath));

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
            var coordinator = CreateCoordinator(storage);

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
            var coordinator = CreateCoordinator(storage);

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
            var coordinator = CreateCoordinator(storage);

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
            var coordinator = CreateCoordinator(storage);

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
            var coordinator = CreateCoordinator(storage);

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
            var coordinator = CreateCoordinator(storage);

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
            var coordinator = CreateCoordinator(storage);
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
            var coordinator = CreateCoordinator(storage);
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
            var coordinator = CreateCoordinator(storage);

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
            var coordinator = CreateCoordinator(storage);

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
            var coordinator = CreateCoordinator(storage);
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
            var coordinator = CreateCoordinator(storage);
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
            var coordinator = CreateCoordinator(storage);

            await coordinator.LinkPolicyDocumentAsync(new PolicyDocumentLinkRequest("policy_001", document.Id, "terms"));
            await coordinator.LinkClaimDocumentAsync(new ClaimDocumentLinkRequest("claim_001", document.Id, "receipt"));
        });

        var attachmentsAfter = SnapshotFiles(Path.Combine(projectRoot, "attachments"));
        var dataLocalAfter = SnapshotFiles(Path.Combine(projectRoot, "data", "local"));
        Assert.Equal(attachmentsBefore, attachmentsAfter);
        Assert.Equal(dataLocalBefore, dataLocalAfter);
    }

    private static DocumentLinkCoordinator CreateCoordinator(
        IDocumentStorageService documentStorageService,
        IEnumerable<string>? activePolicyIds = null,
        IEnumerable<string>? activeClaimIds = null)
    {
        return new DocumentLinkCoordinator(
            documentStorageService,
            new FakePolicyClaimStorageService(
                activePolicyIds ?? ["policy_001", "policy_002"],
                activeClaimIds ?? ["claim_001", "claim_002"]));
    }

    private static PolicyDraft CreatePolicyDraft()
    {
        return new PolicyDraft("Policy A", new DateOnly(2026, 7, 1));
    }

    private static ClaimDraft CreateClaimDraft(string policyId)
    {
        return new ClaimDraft(policyId, "Claim A", new DateOnly(2026, 7, 1));
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

        public Task<PolicyRecord> CreateInsurancePolicyAsync(
            InsurancePolicyDraft draft,
            CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<PolicyRecord> UpdateInsurancePolicyAsync(
            string id,
            InsurancePolicyDraft draft,
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
            int expectedRevision,
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
