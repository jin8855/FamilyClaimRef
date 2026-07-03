using FamilyClaimRef.App.Models.Storage;
using FamilyClaimRef.App.Services.Storage;
using FamilyClaimRef.App.Services.UI;
using FamilyClaimRef.App.ViewModels;
using Xunit;

namespace FamilyClaimRef.App.Tests;

public sealed class PolicyClaimManagementViewModelTests
{
    [Fact]
    public void Constructor_rejects_null_storage()
    {
        var exception = Record.Exception(() => new PolicyClaimManagementViewModel(null!));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public async Task LoadAsync_loads_active_policies_and_claims()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var service = new JsonPolicyClaimStorageService(rootPath);
            var policy = await service.AddPolicyAsync(CreatePolicyDraft("policy_title_demo"));
            var claim = await service.AddClaimAsync(CreateClaimDraft(policy.Id, "claim_title_demo"));
            var viewModel = new PolicyClaimManagementViewModel(service);

            await viewModel.LoadAsync();

            Assert.Equal(policy.Id, Assert.Single(viewModel.AvailablePolicies).Id);
            Assert.Equal(claim.Id, Assert.Single(viewModel.AvailableClaims).Id);
            Assert.Equal(policy.Id, viewModel.SelectedPolicyForClaimId);
            Assert.True(viewModel.HasAvailablePolicies);
            Assert.True(viewModel.HasAvailableClaims);
        });
    }

    [Fact]
    public async Task CreatePolicyAsync_with_title_adds_active_policy()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var service = new JsonPolicyClaimStorageService(rootPath);
            var viewModel = new PolicyClaimManagementViewModel(service)
            {
                NewPolicyDisplayTitle = " policy_title_demo "
            };

            var created = await viewModel.CreatePolicyAsync();

            Assert.True(created);
            var policy = Assert.Single(viewModel.AvailablePolicies);
            Assert.Equal("policy_title_demo", policy.DisplayTitle);
            Assert.Equal(policy.Id, viewModel.SelectedPolicyId);
            Assert.Equal(policy.Id, viewModel.SelectedPolicyForClaimId);
            Assert.Null(viewModel.NewPolicyDisplayTitle);
            Assert.Equal("Policy target was created.", viewModel.ManagementMessage);
        });
    }

    [Fact]
    public async Task CreatePolicyAsync_with_empty_title_is_blocked()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var service = new JsonPolicyClaimStorageService(rootPath);
            var viewModel = new PolicyClaimManagementViewModel(service)
            {
                NewPolicyDisplayTitle = " "
            };

            var created = await viewModel.CreatePolicyAsync();

            Assert.False(created);
            Assert.Empty(viewModel.AvailablePolicies);
            Assert.Equal("Policy target title is required.", viewModel.ManagementMessage);
        });
    }

    [Fact]
    public async Task DisableSelectedPolicyAsync_disables_policy_when_no_active_claims()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var service = new JsonPolicyClaimStorageService(rootPath);
            var policy = await service.AddPolicyAsync(CreatePolicyDraft("policy_title_demo"));
            var viewModel = new PolicyClaimManagementViewModel(service)
            {
                SelectedPolicyId = policy.Id
            };
            await viewModel.LoadAsync();
            viewModel.SelectedPolicyId = policy.Id;

            var disabled = await viewModel.DisableSelectedPolicyAsync();

            Assert.True(disabled);
            Assert.Empty(viewModel.AvailablePolicies);
            Assert.Equal("Policy target was disabled.", viewModel.ManagementMessage);
        });
    }

    [Fact]
    public async Task DisableSelectedPolicyAsync_blocks_when_active_claims_exist()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var service = new JsonPolicyClaimStorageService(rootPath);
            var policy = await service.AddPolicyAsync(CreatePolicyDraft("policy_title_demo"));
            await service.AddClaimAsync(CreateClaimDraft(policy.Id, "claim_title_demo"));
            var viewModel = new PolicyClaimManagementViewModel(service)
            {
                SelectedPolicyId = policy.Id
            };
            await viewModel.LoadAsync();
            viewModel.SelectedPolicyId = policy.Id;

            var disabled = await viewModel.DisableSelectedPolicyAsync();

            Assert.False(disabled);
            Assert.Single(viewModel.AvailablePolicies);
            Assert.Equal(
                "Policy target has active claim targets. Disable claim targets first.",
                viewModel.ManagementMessage);
        });
    }

    [Fact]
    public async Task CreateClaimAsync_requires_selected_active_policy()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var service = new JsonPolicyClaimStorageService(rootPath);
            var viewModel = new PolicyClaimManagementViewModel(service)
            {
                NewClaimDisplayTitle = "claim_title_demo"
            };

            var created = await viewModel.CreateClaimAsync();

            Assert.False(created);
            Assert.Empty(viewModel.AvailableClaims);
            Assert.Equal("Select an active policy target before creating a claim target.", viewModel.ManagementMessage);
        });
    }

    [Fact]
    public async Task CreateClaimAsync_with_title_adds_active_claim()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var service = new JsonPolicyClaimStorageService(rootPath);
            var policy = await service.AddPolicyAsync(CreatePolicyDraft("policy_title_demo"));
            var viewModel = new PolicyClaimManagementViewModel(service);
            await viewModel.LoadAsync();
            viewModel.SelectedPolicyForClaimId = policy.Id;
            viewModel.NewClaimDisplayTitle = " claim_title_demo ";

            var created = await viewModel.CreateClaimAsync();

            Assert.True(created);
            var claim = Assert.Single(viewModel.AvailableClaims);
            Assert.Equal(policy.Id, claim.PolicyId);
            Assert.Equal("claim_title_demo", claim.DisplayTitle);
            Assert.Equal(claim.Id, viewModel.SelectedClaimId);
            Assert.Null(viewModel.NewClaimDisplayTitle);
            Assert.Equal("Claim target was created.", viewModel.ManagementMessage);
        });
    }

    [Fact]
    public async Task CreateClaimAsync_with_empty_title_is_blocked()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var service = new JsonPolicyClaimStorageService(rootPath);
            var policy = await service.AddPolicyAsync(CreatePolicyDraft("policy_title_demo"));
            var viewModel = new PolicyClaimManagementViewModel(service);
            await viewModel.LoadAsync();
            viewModel.SelectedPolicyForClaimId = policy.Id;
            viewModel.NewClaimDisplayTitle = " ";

            var created = await viewModel.CreateClaimAsync();

            Assert.False(created);
            Assert.Empty(viewModel.AvailableClaims);
            Assert.Equal("Claim target title is required.", viewModel.ManagementMessage);
        });
    }

    [Fact]
    public async Task DisableSelectedClaimAsync_disables_claim()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var service = new JsonPolicyClaimStorageService(rootPath);
            var policy = await service.AddPolicyAsync(CreatePolicyDraft("policy_title_demo"));
            var claim = await service.AddClaimAsync(CreateClaimDraft(policy.Id, "claim_title_demo"));
            var viewModel = new PolicyClaimManagementViewModel(service);
            await viewModel.LoadAsync();
            viewModel.SelectedClaimId = claim.Id;

            var disabled = await viewModel.DisableSelectedClaimAsync();

            Assert.True(disabled);
            Assert.Empty(viewModel.AvailableClaims);
            Assert.Equal("Claim target was disabled.", viewModel.ManagementMessage);
        });
    }

    [Fact]
    public async Task Disabled_policy_and_claim_disappear_after_reload()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var service = new JsonPolicyClaimStorageService(rootPath);
            var policy = await service.AddPolicyAsync(CreatePolicyDraft("policy_title_demo"));
            var claim = await service.AddClaimAsync(CreateClaimDraft(policy.Id, "claim_title_demo"));
            await service.DisableClaimAsync(claim.Id);
            await service.DisablePolicyAsync(policy.Id);
            var viewModel = new PolicyClaimManagementViewModel(service)
            {
                SelectedPolicyId = policy.Id,
                SelectedClaimId = claim.Id,
                SelectedPolicyForClaimId = policy.Id
            };

            await viewModel.LoadAsync();

            Assert.Empty(viewModel.AvailablePolicies);
            Assert.Empty(viewModel.AvailableClaims);
            Assert.Null(viewModel.SelectedPolicyId);
            Assert.Null(viewModel.SelectedClaimId);
            Assert.Null(viewModel.SelectedPolicyForClaimId);
        });
    }

    [Fact]
    public async Task MainWindowViewModel_management_action_refreshes_registration_targets()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var service = new JsonPolicyClaimStorageService(rootPath);
            var documentRegistration = CreateDocumentRegistrationViewModel(rootPath, service);
            var management = new PolicyClaimManagementViewModel(service)
            {
                NewPolicyDisplayTitle = "policy_title_demo"
            };
            var mainWindow = new MainWindowViewModel(documentRegistration, management);

            await mainWindow.LoadAsync();
            Assert.Empty(documentRegistration.AvailablePolicies);

            await mainWindow.CreatePolicyAsync();

            Assert.Single(documentRegistration.AvailablePolicies);
            Assert.Equal("policy_title_demo", documentRegistration.AvailablePolicies[0].DisplayTitle);
        });
    }

    [Fact]
    public async Task Management_actions_do_not_create_project_root_attachment_or_data_local_files()
    {
        var projectRoot = FindProjectRoot();
        var attachmentsPath = Path.Combine(projectRoot, "attachments");
        var dataLocalPath = Path.Combine(projectRoot, "data", "local");
        var attachmentsBefore = SnapshotFiles(attachmentsPath);
        var dataLocalBefore = SnapshotFiles(dataLocalPath);

        await UsingTempRootAsync(async rootPath =>
        {
            var service = new JsonPolicyClaimStorageService(rootPath);
            var viewModel = new PolicyClaimManagementViewModel(service)
            {
                NewPolicyDisplayTitle = "policy_title_demo"
            };

            await viewModel.CreatePolicyAsync();
            viewModel.NewClaimDisplayTitle = "claim_title_demo";
            await viewModel.CreateClaimAsync();
            await viewModel.DisableSelectedClaimAsync();
            await viewModel.DisableSelectedPolicyAsync();
        });

        Assert.Equal(attachmentsBefore, SnapshotFiles(attachmentsPath));
        Assert.Equal(dataLocalBefore, SnapshotFiles(dataLocalPath));
    }

    private static PolicyDraft CreatePolicyDraft(string displayTitle = "policy_title_demo")
    {
        return new PolicyDraft(displayTitle, new DateOnly(2026, 7, 1));
    }

    private static ClaimDraft CreateClaimDraft(
        string policyId,
        string displayTitle = "claim_title_demo")
    {
        return new ClaimDraft(policyId, displayTitle, new DateOnly(2026, 7, 1));
    }

    private static DocumentRegistrationViewModel CreateDocumentRegistrationViewModel(
        string rootPath,
        IPolicyClaimStorageService policyClaimStorage)
    {
        var metadataRoot = Path.Combine(rootPath, "metadata");
        var attachmentRoot = Path.Combine(rootPath, "attachments");
        IDocumentStorageService documentStorage = new JsonDocumentStorageService(metadataRoot);
        IFileAttachmentService fileAttachment = new LocalFileAttachmentService(attachmentRoot);
        var workflow = new DocumentRegistrationWorkflow(
            new DocumentAttachmentCoordinator(documentStorage, fileAttachment),
            new DocumentLinkCoordinator(documentStorage, policyClaimStorage),
            documentStorage,
            fileAttachment);

        return new DocumentRegistrationViewModel(
            workflow,
            new FakeFilePickerService(),
            policyClaimStorage);
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

    private sealed class FakeFilePickerService : IFilePickerService
    {
        public Task<FilePickerResult?> PickDocumentFileAsync(
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult<FilePickerResult?>(null);
        }
    }
}
