using FamilyClaimRef.App.Models.Storage;
using FamilyClaimRef.App.Services.Localization;
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
        var exception = Record.Exception(() => new PolicyClaimManagementViewModel(null!, CreateUiTextProvider()));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void Constructor_rejects_null_ui_text_provider()
    {
        var exception = Record.Exception(() => new PolicyClaimManagementViewModel(
            new TestPolicyClaimStorageService(),
            null!));

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
            var viewModel = new PolicyClaimManagementViewModel(service, CreateUiTextProvider());

            var loaded = await viewModel.LoadAsync();

            Assert.True(loaded);
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
            var viewModel = new PolicyClaimManagementViewModel(service, CreateUiTextProvider())
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
            Assert.Equal("보험 계약을 등록했습니다.", viewModel.ManagementMessage);
        });
    }

    [Fact]
    public async Task CreatePolicyAsync_with_empty_title_is_blocked()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var service = new JsonPolicyClaimStorageService(rootPath);
            var viewModel = new PolicyClaimManagementViewModel(service, CreateUiTextProvider())
            {
                NewPolicyDisplayTitle = " "
            };

            var created = await viewModel.CreatePolicyAsync();

            Assert.False(created);
            Assert.Empty(viewModel.AvailablePolicies);
            Assert.Equal("보험 계약 이름을 입력해 주세요.", viewModel.ManagementMessage);
        });
    }

    [Fact]
    public async Task DisableSelectedPolicyAsync_disables_policy_when_no_active_claims()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var service = new JsonPolicyClaimStorageService(rootPath);
            var policy = await service.AddPolicyAsync(CreatePolicyDraft("policy_title_demo"));
            var viewModel = new PolicyClaimManagementViewModel(service, CreateUiTextProvider())
            {
                SelectedPolicyId = policy.Id
            };
            await viewModel.LoadAsync();
            viewModel.SelectedPolicyId = policy.Id;

            var disabled = await viewModel.DisableSelectedPolicyAsync();

            Assert.True(disabled);
            Assert.Empty(viewModel.AvailablePolicies);
            Assert.Equal("보험 계약을 사용 중지했습니다.", viewModel.ManagementMessage);
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
            var viewModel = new PolicyClaimManagementViewModel(service, CreateUiTextProvider())
            {
                SelectedPolicyId = policy.Id
            };
            await viewModel.LoadAsync();
            viewModel.SelectedPolicyId = policy.Id;

            var disabled = await viewModel.DisableSelectedPolicyAsync();

            Assert.False(disabled);
            Assert.Single(viewModel.AvailablePolicies);
            Assert.Equal(
                "활성 청구 건이 있어 보험 계약을 사용 중지할 수 없습니다. 청구 건을 먼저 사용 중지해 주세요.",
                viewModel.ManagementMessage);
        });
    }

    [Fact]
    public async Task CreateClaimAsync_requires_selected_active_policy()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var service = new JsonPolicyClaimStorageService(rootPath);
            var viewModel = new PolicyClaimManagementViewModel(service, CreateUiTextProvider())
            {
                NewClaimDisplayTitle = "claim_title_demo"
            };

            var created = await viewModel.CreateClaimAsync();

            Assert.False(created);
            Assert.Empty(viewModel.AvailableClaims);
            Assert.Equal("청구 건을 등록할 보험 계약을 선택해 주세요.", viewModel.ManagementMessage);
        });
    }

    [Fact]
    public async Task CreateClaimAsync_with_title_adds_active_claim()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var service = new JsonPolicyClaimStorageService(rootPath);
            var policy = await service.AddPolicyAsync(CreatePolicyDraft("policy_title_demo"));
            var viewModel = new PolicyClaimManagementViewModel(service, CreateUiTextProvider());
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
            Assert.Equal("청구 건을 등록했습니다.", viewModel.ManagementMessage);
        });
    }

    [Fact]
    public async Task CreateClaimAsync_with_empty_title_is_blocked()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var service = new JsonPolicyClaimStorageService(rootPath);
            var policy = await service.AddPolicyAsync(CreatePolicyDraft("policy_title_demo"));
            var viewModel = new PolicyClaimManagementViewModel(service, CreateUiTextProvider());
            await viewModel.LoadAsync();
            viewModel.SelectedPolicyForClaimId = policy.Id;
            viewModel.NewClaimDisplayTitle = " ";

            var created = await viewModel.CreateClaimAsync();

            Assert.False(created);
            Assert.Empty(viewModel.AvailableClaims);
            Assert.Equal("청구 건 이름을 입력해 주세요.", viewModel.ManagementMessage);
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
            var viewModel = new PolicyClaimManagementViewModel(service, CreateUiTextProvider());
            await viewModel.LoadAsync();
            viewModel.SelectedClaimId = claim.Id;

            var disabled = await viewModel.DisableSelectedClaimAsync();

            Assert.True(disabled);
            Assert.Empty(viewModel.AvailableClaims);
            Assert.Equal("청구 건을 사용 중지했습니다.", viewModel.ManagementMessage);
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
            var viewModel = new PolicyClaimManagementViewModel(service, CreateUiTextProvider())
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
    public async Task Repeated_load_replaces_collections_without_duplicate_rows()
    {
        var service = new TestPolicyClaimStorageService();
        var policy = service.SeedPolicy("policy_title_demo");
        var claim = service.SeedClaim(policy.Id, "claim_title_demo");
        var viewModel = new PolicyClaimManagementViewModel(service, CreateUiTextProvider());

        var firstLoaded = await viewModel.LoadAsync();
        var secondLoaded = await viewModel.LoadAsync();

        Assert.True(firstLoaded);
        Assert.True(secondLoaded);
        Assert.Equal(policy.Id, Assert.Single(viewModel.AvailablePolicies).Id);
        Assert.Equal(claim.Id, Assert.Single(viewModel.AvailableClaims).Id);
    }

    [Fact]
    public async Task ClearManagementMessage_preserves_inputs_selections_and_collections()
    {
        var service = new TestPolicyClaimStorageService();
        var policy = service.SeedPolicy("policy_title_demo");
        var claim = service.SeedClaim(policy.Id, "claim_title_demo");
        var viewModel = new PolicyClaimManagementViewModel(service, CreateUiTextProvider());
        await viewModel.LoadAsync();
        viewModel.SelectedPolicyId = policy.Id;
        viewModel.SelectedClaimId = claim.Id;
        viewModel.NewPolicyDisplayTitle = "pending_policy_title";
        viewModel.NewClaimDisplayTitle = "pending_claim_title";
        viewModel.SelectedClaimId = null;
        await viewModel.DisableSelectedClaimAsync();
        viewModel.SelectedClaimId = claim.Id;

        viewModel.ClearManagementMessage();

        Assert.Null(viewModel.ManagementMessage);
        Assert.Equal("pending_policy_title", viewModel.NewPolicyDisplayTitle);
        Assert.Equal("pending_claim_title", viewModel.NewClaimDisplayTitle);
        Assert.Equal(policy.Id, viewModel.SelectedPolicyId);
        Assert.Equal(policy.Id, viewModel.SelectedPolicyForClaimId);
        Assert.Equal(claim.Id, viewModel.SelectedClaimId);
        Assert.Single(viewModel.AvailablePolicies);
        Assert.Single(viewModel.AvailableClaims);
    }

    [Fact]
    public async Task LoadAsync_storage_failure_returns_false_and_hides_diagnostics()
    {
        var service = new TestPolicyClaimStorageService
        {
            ThrowOnPolicyRead = true
        };
        var viewModel = new PolicyClaimManagementViewModel(service, CreateUiTextProvider());

        var loaded = await viewModel.LoadAsync();

        Assert.False(loaded);
        Assert.Equal(
            "목록을 불러오지 못했습니다. 다시 시도해 주세요.",
            viewModel.ManagementMessage);
        Assert.DoesNotContain("internal-storage-detail", viewModel.ManagementMessage, StringComparison.Ordinal);
    }

    [Fact]
    public async Task CreatePolicyAsync_rejects_trimmed_case_insensitive_active_duplicate()
    {
        var service = new TestPolicyClaimStorageService();
        service.SeedPolicy("policy_title_demo");
        var viewModel = new PolicyClaimManagementViewModel(service, CreateUiTextProvider())
        {
            NewPolicyDisplayTitle = "  POLICY_TITLE_DEMO  "
        };

        var created = await viewModel.CreatePolicyAsync();

        Assert.False(created);
        Assert.Equal("  POLICY_TITLE_DEMO  ", viewModel.NewPolicyDisplayTitle);
        Assert.Equal(0, service.AddPolicyCallCount);
        Assert.Equal(
            "같은 이름의 활성 보험 계약이 이미 있습니다.",
            viewModel.ManagementMessage);
    }

    [Fact]
    public async Task CreateClaimAsync_rejects_global_active_duplicate_across_policies()
    {
        var service = new TestPolicyClaimStorageService();
        var firstPolicy = service.SeedPolicy("first_policy");
        var secondPolicy = service.SeedPolicy("second_policy");
        service.SeedClaim(firstPolicy.Id, "claim_title_demo");
        var viewModel = new PolicyClaimManagementViewModel(service, CreateUiTextProvider())
        {
            SelectedPolicyForClaimId = secondPolicy.Id,
            NewClaimDisplayTitle = "  CLAIM_TITLE_DEMO "
        };

        var created = await viewModel.CreateClaimAsync();

        Assert.False(created);
        Assert.Equal("  CLAIM_TITLE_DEMO ", viewModel.NewClaimDisplayTitle);
        Assert.Equal(0, service.AddClaimCallCount);
        Assert.Equal(
            "같은 이름의 활성 청구 건이 이미 있습니다.",
            viewModel.ManagementMessage);
    }

    [Fact]
    public async Task Disabled_policy_and_claim_titles_can_be_reused()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var service = new JsonPolicyClaimStorageService(rootPath);
            var firstPolicy = await service.AddPolicyAsync(CreatePolicyDraft("reusable_policy"));
            var firstClaim = await service.AddClaimAsync(CreateClaimDraft(firstPolicy.Id, "reusable_claim"));
            await service.DisableClaimAsync(firstClaim.Id);
            await service.DisablePolicyAsync(firstPolicy.Id);
            var viewModel = new PolicyClaimManagementViewModel(service, CreateUiTextProvider())
            {
                NewPolicyDisplayTitle = "REUSABLE_POLICY"
            };

            Assert.True(await viewModel.CreatePolicyAsync());
            viewModel.NewClaimDisplayTitle = "REUSABLE_CLAIM";
            Assert.True(await viewModel.CreateClaimAsync());

            Assert.Equal("REUSABLE_POLICY", Assert.Single(viewModel.AvailablePolicies).DisplayTitle);
            Assert.Equal("REUSABLE_CLAIM", Assert.Single(viewModel.AvailableClaims).DisplayTitle);
        });
    }

    [Fact]
    public async Task Policy_mutation_failure_keeps_input_and_uses_safe_message()
    {
        var service = new TestPolicyClaimStorageService
        {
            ThrowOnPolicyAdd = true
        };
        var viewModel = new PolicyClaimManagementViewModel(service, CreateUiTextProvider())
        {
            NewPolicyDisplayTitle = "pending_policy_title"
        };

        var created = await viewModel.CreatePolicyAsync();

        Assert.False(created);
        Assert.Equal("pending_policy_title", viewModel.NewPolicyDisplayTitle);
        Assert.Equal(
            "보험 계약을 처리하지 못했습니다. 다시 시도해 주세요.",
            viewModel.ManagementMessage);
        Assert.DoesNotContain("internal-storage-detail", viewModel.ManagementMessage, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Claim_mutation_failure_keeps_input_and_uses_safe_message()
    {
        var service = new TestPolicyClaimStorageService
        {
            ThrowOnClaimAdd = true
        };
        var policy = service.SeedPolicy("policy_title_demo");
        var viewModel = new PolicyClaimManagementViewModel(service, CreateUiTextProvider())
        {
            SelectedPolicyForClaimId = policy.Id,
            NewClaimDisplayTitle = "pending_claim_title"
        };

        var created = await viewModel.CreateClaimAsync();

        Assert.False(created);
        Assert.Equal("pending_claim_title", viewModel.NewClaimDisplayTitle);
        Assert.Equal(
            "청구 건을 처리하지 못했습니다. 다시 시도해 주세요.",
            viewModel.ManagementMessage);
        Assert.DoesNotContain("internal-storage-detail", viewModel.ManagementMessage, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Successful_mutation_with_refresh_failure_returns_true_and_keeps_load_error()
    {
        var service = new TestPolicyClaimStorageService
        {
            ThrowOnPolicyReadAfterPolicyAdd = true
        };
        var viewModel = new PolicyClaimManagementViewModel(service, CreateUiTextProvider())
        {
            NewPolicyDisplayTitle = "policy_title_demo"
        };

        var created = await viewModel.CreatePolicyAsync();

        Assert.True(created);
        Assert.Null(viewModel.NewPolicyDisplayTitle);
        Assert.Equal(
            "목록을 불러오지 못했습니다. 다시 시도해 주세요.",
            viewModel.ManagementMessage);
        Assert.Equal(1, service.AddPolicyCallCount);
    }

    [Fact]
    public async Task Parallel_same_instance_policy_creates_are_serialized()
    {
        var service = new TestPolicyClaimStorageService
        {
            PausePolicyAdd = true
        };
        var viewModel = new PolicyClaimManagementViewModel(service, CreateUiTextProvider())
        {
            NewPolicyDisplayTitle = "policy_title_demo"
        };

        var firstCreate = viewModel.CreatePolicyAsync();
        await service.WaitForPolicyAddAsync();
        var secondCreate = viewModel.CreatePolicyAsync();
        service.ReleasePolicyAdd();

        var results = await Task.WhenAll(firstCreate, secondCreate);

        Assert.Equal([true, false], results);
        Assert.Equal(1, service.AddPolicyCallCount);
        Assert.Single(viewModel.AvailablePolicies);
    }

    [Fact]
    public async Task MainWindowViewModel_management_action_refreshes_registration_targets()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var service = new JsonPolicyClaimStorageService(rootPath);
            var documentRegistration = CreateDocumentRegistrationViewModel(rootPath, service);
            var management = new PolicyClaimManagementViewModel(service, CreateUiTextProvider())
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
            var viewModel = new PolicyClaimManagementViewModel(service, CreateUiTextProvider())
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
            policyClaimStorage,
            CreateDocumentRegistrationUiTextProvider());
    }

    private static IUiTextProvider CreateUiTextProvider()
    {
        return new ResourceUiTextProvider(new Dictionary<string, string>(StringComparer.Ordinal)
        {
            [UiTextKeys.ClaimManagementMessageCreated] = "청구 건을 등록했습니다.",
            [UiTextKeys.ClaimManagementMessageDisabled] = "청구 건을 사용 중지했습니다.",
            [UiTextKeys.ClaimManagementValidationTitleRequired] = "청구 건 이름을 입력해 주세요.",
            [UiTextKeys.PolicyManagementMessageCreated] = "보험 계약을 등록했습니다.",
            [UiTextKeys.PolicyManagementMessageDisabled] = "보험 계약을 사용 중지했습니다.",
            [UiTextKeys.PolicyManagementValidationDisableBlockedByActiveClaims] =
                "활성 청구 건이 있어 보험 계약을 사용 중지할 수 없습니다. 청구 건을 먼저 사용 중지해 주세요.",
            [UiTextKeys.ClaimManagementValidationSelectPolicyBeforeCreate] =
                "청구 건을 등록할 보험 계약을 선택해 주세요.",
            [UiTextKeys.PolicyManagementValidationTitleRequired] = "보험 계약 이름을 입력해 주세요.",
            [UiTextKeys.ClaimManagementValidationSelectClaimTarget] = "사용 중지할 청구 건을 선택해 주세요.",
            [UiTextKeys.PolicyManagementValidationSelectPolicyTarget] =
                "사용 중지할 보험 계약을 선택해 주세요.",
            [UiTextKeys.ProductManagementLoadFailedMessage] =
                "목록을 불러오지 못했습니다. 다시 시도해 주세요.",
            [UiTextKeys.ProductPolicyContractsOperationFailedMessage] =
                "보험 계약을 처리하지 못했습니다. 다시 시도해 주세요.",
            [UiTextKeys.ProductClaimCasesOperationFailedMessage] =
                "청구 건을 처리하지 못했습니다. 다시 시도해 주세요.",
            [UiTextKeys.ProductPolicyContractsDuplicateTitleMessage] =
                "같은 이름의 활성 보험 계약이 이미 있습니다.",
            [UiTextKeys.ProductClaimCasesDuplicateTitleMessage] =
                "같은 이름의 활성 청구 건이 이미 있습니다."
        });
    }

    private static IUiTextProvider CreateDocumentRegistrationUiTextProvider()
    {
        return new ResourceUiTextProvider(new Dictionary<string, string>(StringComparer.Ordinal)
        {
            [UiTextKeys.DocumentRegistrationStatusCleanupFailed] =
                "등록 중 일부 정리가 실패했습니다. 다시 시도하거나 관리자에게 문의하세요.",
            [UiTextKeys.DocumentRegistrationMessageNoActiveClaim] = "No active claim is available for selection.",
            [UiTextKeys.DocumentRegistrationMessageNoActivePolicy] = "No active policy is available for selection.",
            [UiTextKeys.DocumentRegistrationStatusFailed] = "문서 등록에 실패했습니다.",
            [UiTextKeys.DocumentRegistrationStatusCompleted] = "문서 등록이 완료되었습니다.",
            [UiTextKeys.DocumentRegistrationValidationSelectClaimBeforeRegister] =
                "Select a claim before registering this document.",
            [UiTextKeys.DocumentRegistrationValidationSelectPolicyBeforeRegister] =
                "Select a policy before registering this document.",
            [UiTextKeys.DocumentRegistrationStatusFileSelected] = "파일을 선택했습니다.",
            [UiTextKeys.DocumentRegistrationValidationSelectFile] = "파일을 선택해 주세요.",
            [UiTextKeys.DocumentRegistrationValidationSelectTargetKind] = "연결 대상 유형을 선택해 주세요.",
            [UiTextKeys.DocumentRegistrationValidationSelectTarget] = "저장할 대상을 입력해 주세요.",
            [UiTextKeys.DocumentRegistrationValidationSelectDocumentType] = "문서 유형을 선택해 주세요.",
            [UiTextKeys.DocumentRegistrationValidationEnterDisplayTitle] = "표시 제목을 입력해 주세요.",
            [UiTextKeys.DocumentRegistrationValidationSelectReferenceDate] = "기준일을 선택해 주세요."
        });
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

    private sealed class TestPolicyClaimStorageService : IPolicyClaimStorageService
    {
        private readonly object syncRoot = new();
        private readonly List<PolicyRecord> policies = [];
        private readonly List<ClaimRecord> claims = [];
        private readonly TaskCompletionSource<bool> policyAddStarted =
            new(TaskCreationOptions.RunContinuationsAsynchronously);
        private readonly TaskCompletionSource<bool> policyAddRelease =
            new(TaskCreationOptions.RunContinuationsAsynchronously);
        private int policySequence;
        private int claimSequence;

        public bool ThrowOnPolicyRead { get; init; }

        public bool ThrowOnClaimRead { get; init; }

        public bool ThrowOnPolicyReadAfterPolicyAdd { get; init; }

        public bool ThrowOnPolicyAdd { get; init; }

        public bool ThrowOnClaimAdd { get; init; }

        public bool PausePolicyAdd { get; init; }

        public int AddPolicyCallCount { get; private set; }

        public int AddClaimCallCount { get; private set; }

        public PolicyRecord SeedPolicy(string displayTitle)
        {
            var now = DateTimeOffset.UtcNow;
            var policy = new PolicyRecord(
                $"policy-{++policySequence}",
                displayTitle,
                new DateOnly(2026, 7, 1),
                now,
                now,
                null);

            lock (syncRoot)
            {
                policies.Add(policy);
            }

            return policy;
        }

        public ClaimRecord SeedClaim(string policyId, string displayTitle)
        {
            var now = DateTimeOffset.UtcNow;
            var claim = new ClaimRecord(
                $"claim-{++claimSequence}",
                policyId,
                displayTitle,
                new DateOnly(2026, 7, 1),
                now,
                now,
                null);

            lock (syncRoot)
            {
                claims.Add(claim);
            }

            return claim;
        }

        public Task WaitForPolicyAddAsync()
        {
            return policyAddStarted.Task;
        }

        public void ReleasePolicyAdd()
        {
            policyAddRelease.TrySetResult(true);
        }

        public Task<IReadOnlyList<PolicyRecord>> GetPoliciesAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (ThrowOnPolicyRead
                || (ThrowOnPolicyReadAfterPolicyAdd && AddPolicyCallCount > 0))
            {
                throw CreateStorageException();
            }

            lock (syncRoot)
            {
                return Task.FromResult<IReadOnlyList<PolicyRecord>>(
                    policies.Where(policy => policy.DisabledAt is null).ToArray());
            }
        }

        public Task<PolicyRecord?> GetPolicyAsync(string id, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            lock (syncRoot)
            {
                return Task.FromResult(
                    policies.FirstOrDefault(policy =>
                        policy.DisabledAt is null
                        && string.Equals(policy.Id, id, StringComparison.Ordinal)));
            }
        }

        public async Task<PolicyRecord> AddPolicyAsync(
            PolicyDraft draft,
            CancellationToken cancellationToken = default)
        {
            lock (syncRoot)
            {
                AddPolicyCallCount++;
            }

            policyAddStarted.TrySetResult(true);
            if (PausePolicyAdd)
            {
                await policyAddRelease.Task.WaitAsync(cancellationToken);
            }

            if (ThrowOnPolicyAdd)
            {
                throw CreateStorageException();
            }

            return SeedPolicy(draft.DisplayTitle);
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

        public Task<PolicyRecord> DisablePolicyAsync(string id, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            lock (syncRoot)
            {
                var index = policies.FindIndex(policy =>
                    policy.DisabledAt is null
                    && string.Equals(policy.Id, id, StringComparison.Ordinal));
                if (index < 0)
                {
                    throw new KeyNotFoundException(id);
                }

                var disabled = policies[index] with
                {
                    DisabledAt = DateTimeOffset.UtcNow,
                    UpdatedAt = DateTimeOffset.UtcNow
                };
                policies[index] = disabled;
                return Task.FromResult(disabled);
            }
        }

        public Task<IReadOnlyList<ClaimRecord>> GetClaimsAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (ThrowOnClaimRead)
            {
                throw CreateStorageException();
            }

            lock (syncRoot)
            {
                return Task.FromResult<IReadOnlyList<ClaimRecord>>(
                    claims.Where(claim => claim.DisabledAt is null).ToArray());
            }
        }

        public Task<IReadOnlyList<ClaimRecord>> GetClaimsByPolicyIdAsync(
            string policyId,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            lock (syncRoot)
            {
                return Task.FromResult<IReadOnlyList<ClaimRecord>>(
                    claims
                        .Where(claim =>
                            claim.DisabledAt is null
                            && string.Equals(claim.PolicyId, policyId, StringComparison.Ordinal))
                        .ToArray());
            }
        }

        public Task<ClaimRecord?> GetClaimAsync(string id, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            lock (syncRoot)
            {
                return Task.FromResult(
                    claims.FirstOrDefault(claim =>
                        claim.DisabledAt is null
                        && string.Equals(claim.Id, id, StringComparison.Ordinal)));
            }
        }

        public Task<ClaimRecord> AddClaimAsync(ClaimDraft draft, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            lock (syncRoot)
            {
                AddClaimCallCount++;
            }

            if (ThrowOnClaimAdd)
            {
                throw CreateStorageException();
            }

            return Task.FromResult(SeedClaim(draft.PolicyId, draft.DisplayTitle));
        }

        public Task<ClaimRecord> DisableClaimAsync(string id, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            lock (syncRoot)
            {
                var index = claims.FindIndex(claim =>
                    claim.DisabledAt is null
                    && string.Equals(claim.Id, id, StringComparison.Ordinal));
                if (index < 0)
                {
                    throw new KeyNotFoundException(id);
                }

                var disabled = claims[index] with
                {
                    DisabledAt = DateTimeOffset.UtcNow,
                    UpdatedAt = DateTimeOffset.UtcNow
                };
                claims[index] = disabled;
                return Task.FromResult(disabled);
            }
        }

        public Task<bool> PolicyExistsAsync(string id, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            lock (syncRoot)
            {
                return Task.FromResult(policies.Any(policy =>
                    policy.DisabledAt is null
                    && string.Equals(policy.Id, id, StringComparison.Ordinal)));
            }
        }

        public Task<bool> ClaimExistsAsync(string id, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            lock (syncRoot)
            {
                return Task.FromResult(claims.Any(claim =>
                    claim.DisabledAt is null
                    && string.Equals(claim.Id, id, StringComparison.Ordinal)));
            }
        }

        private static InvalidOperationException CreateStorageException()
        {
            return new InvalidOperationException(
                "Synthetic internal-storage-detail that must not reach product copy.");
        }
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
