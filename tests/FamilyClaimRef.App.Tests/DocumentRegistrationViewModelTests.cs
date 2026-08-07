using FamilyClaimRef.App.Models.Storage;
using FamilyClaimRef.App.Services.Localization;
using FamilyClaimRef.App.Services.Storage;
using FamilyClaimRef.App.Services.UI;
using FamilyClaimRef.App.ViewModels;
using Xunit;

namespace FamilyClaimRef.App.Tests;

public sealed class DocumentRegistrationViewModelTests
{
    [Fact]
    public void Constructor_rejects_null_workflow()
    {
        var exception = Record.Exception(() => new DocumentRegistrationViewModel(
            null!,
            new FakeFilePickerService(null),
            new FakePolicyClaimStorageService(),
            CreateUiTextProvider()));

        Assert.NotNull(exception);
        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void Constructor_rejects_null_file_picker()
    {
        var workflow = CreateWorkflow(new SpyDocumentStorageService(), new SpyFileAttachmentService());

        var exception = Record.Exception(() => new DocumentRegistrationViewModel(
            workflow,
            null!,
            new FakePolicyClaimStorageService(),
            CreateUiTextProvider()));

        Assert.NotNull(exception);
        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void Constructor_rejects_null_policy_claim_storage()
    {
        var workflow = CreateWorkflow(new SpyDocumentStorageService(), new SpyFileAttachmentService());

        var exception = Record.Exception(() => new DocumentRegistrationViewModel(
            workflow,
            new FakeFilePickerService(null),
            null!,
            CreateUiTextProvider()));

        Assert.NotNull(exception);
        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void Constructor_rejects_null_ui_text_provider()
    {
        var workflow = CreateWorkflow(new SpyDocumentStorageService(), new SpyFileAttachmentService());

        var exception = Record.Exception(() => new DocumentRegistrationViewModel(
            workflow,
            new FakeFilePickerService(null),
            new FakePolicyClaimStorageService(),
            null!));

        Assert.NotNull(exception);
        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public async Task SelectFileAsync_updates_selected_file_state()
    {
        var workflow = CreateWorkflow(new SpyDocumentStorageService(), new SpyFileAttachmentService());
        var viewModel = new DocumentRegistrationViewModel(
            workflow,
            new FakeFilePickerService(new FilePickerResult("C:\\Temp\\dummy.pdf", "dummy.pdf")),
            new FakePolicyClaimStorageService(),
            CreateUiTextProvider());

        await viewModel.SelectFileAsync();

        Assert.Equal("C:\\Temp\\dummy.pdf", viewModel.SelectedSourceFilePath);
        Assert.Equal("dummy.pdf", viewModel.SelectedSourceFileDisplayName);
        Assert.Null(viewModel.ValidationMessage);
        Assert.Equal("파일을 선택했습니다.", viewModel.StatusMessage);
    }

    [Fact]
    public async Task SelectFileAsync_cancel_keeps_previous_state_and_does_not_set_error()
    {
        var workflow = CreateWorkflow(new SpyDocumentStorageService(), new SpyFileAttachmentService());
        var viewModel = new DocumentRegistrationViewModel(
            workflow,
            new FakeFilePickerService(null),
            new FakePolicyClaimStorageService(),
            CreateUiTextProvider())
        {
            SelectedSourceFilePath = "C:\\Temp\\previous.pdf",
            SelectedSourceFileDisplayName = "previous.pdf"
        };

        await viewModel.SelectFileAsync();

        Assert.Equal("C:\\Temp\\previous.pdf", viewModel.SelectedSourceFilePath);
        Assert.Equal("previous.pdf", viewModel.SelectedSourceFileDisplayName);
        Assert.Null(viewModel.ValidationMessage);
        Assert.Equal("파일 선택을 취소했습니다.", viewModel.StatusMessage);
    }

    [Fact]
    public async Task LoadTargetOptionsAsync_loads_active_policy_and_claim_options()
    {
        var viewModel = CreateTargetSelectionViewModel(new FakePolicyClaimStorageService(
            ["policy_demo_001"],
            ["claim_demo_001"]));

        await viewModel.LoadTargetOptionsAsync();

        var policy = Assert.Single(viewModel.AvailablePolicies);
        var claim = Assert.Single(viewModel.AvailableClaims);
        Assert.Equal("policy_demo_001", policy.Id);
        Assert.Equal("claim_demo_001", claim.Id);
        Assert.True(viewModel.HasAvailablePolicies);
        Assert.True(viewModel.HasAvailableClaims);
        Assert.Null(viewModel.TargetSelectionMessage);
    }

    [Fact]
    public async Task LoadTargetOptionsAsync_repeated_load_replaces_snapshot_and_clears_invalid_selections()
    {
        var storage = new FakePolicyClaimStorageService(
            ["policy_initial_001"],
            ["claim_initial_001"]);
        var filePicker = new FakeFilePickerService(null);
        var fileAttachment = new SpyFileAttachmentService();
        var viewModel = CreateTargetSelectionViewModel(storage, filePicker, fileAttachment);

        await viewModel.LoadTargetOptionsAsync();
        viewModel.TargetKind = DocumentRegistrationViewModel.PolicyTargetKind;
        viewModel.SelectedPolicyId = "policy_initial_001";
        viewModel.TargetKind = DocumentRegistrationViewModel.ClaimTargetKind;
        viewModel.SelectedClaimId = "claim_initial_001";

        storage.ReplaceActiveTargets(
            ["policy_current_001", "policy_current_002"],
            ["claim_current_001", "claim_current_002"]);

        await viewModel.LoadTargetOptionsAsync();

        Assert.Equal(
            ["policy_current_001", "policy_current_002"],
            viewModel.AvailablePolicies.Select(policy => policy.Id).Order(StringComparer.Ordinal));
        Assert.Equal(
            ["claim_current_001", "claim_current_002"],
            viewModel.AvailableClaims.Select(claim => claim.Id).Order(StringComparer.Ordinal));
        Assert.DoesNotContain(viewModel.AvailablePolicies, policy => policy.Id == "policy_initial_001");
        Assert.DoesNotContain(viewModel.AvailableClaims, claim => claim.Id == "claim_initial_001");
        Assert.Null(viewModel.SelectedPolicyId);
        Assert.Null(viewModel.SelectedClaimId);
        Assert.Null(viewModel.TargetId);
        Assert.Equal(
            viewModel.AvailablePolicies.Count,
            viewModel.AvailablePolicies.Select(policy => policy.Id).Distinct(StringComparer.Ordinal).Count());
        Assert.Equal(
            viewModel.AvailableClaims.Count,
            viewModel.AvailableClaims.Select(claim => claim.Id).Distinct(StringComparer.Ordinal).Count());
        Assert.Equal(2, storage.GetPoliciesCallCount);
        Assert.Equal(2, storage.GetClaimsCallCount);
        Assert.Equal(0, filePicker.PickCallCount);
        Assert.False(fileAttachment.CopyCalled);
    }

    [Fact]
    public async Task LoadTargetOptionsAsync_does_not_expose_disabled_policy_or_claim_records()
    {
        await UsingTempRootsAsync(async (metadataRoot, _) =>
        {
            var service = new JsonPolicyClaimStorageService(metadataRoot);
            var activePolicy = await service.AddPolicyAsync(new PolicyDraft(
                "Policy Active",
                new DateOnly(2026, 7, 1)));
            var disabledPolicy = await service.AddPolicyAsync(new PolicyDraft(
                "Policy Disabled",
                new DateOnly(2026, 7, 1)));
            var activeClaim = await service.AddClaimAsync(new ClaimDraft(
                activePolicy.Id,
                "Claim Active",
                new DateOnly(2026, 7, 1)));
            var disabledClaim = await service.AddClaimAsync(new ClaimDraft(
                activePolicy.Id,
                "Claim Disabled",
                new DateOnly(2026, 7, 1)));
            await service.DisablePolicyAsync(disabledPolicy.Id);
            await service.DisableClaimAsync(disabledClaim.Id, disabledClaim.Revision);
            var viewModel = CreateTargetSelectionViewModel(service);

            await viewModel.LoadTargetOptionsAsync();

            var policy = Assert.Single(viewModel.AvailablePolicies);
            var claim = Assert.Single(viewModel.AvailableClaims);
            Assert.Equal(activePolicy.Id, policy.Id);
            Assert.Equal(activeClaim.Id, claim.Id);
            Assert.DoesNotContain(viewModel.AvailablePolicies, item => item.Id == disabledPolicy.Id);
            Assert.DoesNotContain(viewModel.AvailableClaims, item => item.Id == disabledClaim.Id);
        });
    }

    [Fact]
    public async Task Selecting_policy_sets_target_kind_and_id_for_registration_contract()
    {
        var viewModel = CreateTargetSelectionViewModel(new FakePolicyClaimStorageService(
            ["policy_demo_001"],
            ["claim_demo_001"]));

        await viewModel.LoadTargetOptionsAsync();
        viewModel.TargetKind = DocumentRegistrationViewModel.PolicyTargetKind;
        viewModel.SelectedPolicyId = "policy_demo_001";

        Assert.Equal(DocumentRegistrationViewModel.PolicyTargetKind, viewModel.TargetKind);
        Assert.Equal("policy_demo_001", viewModel.TargetId);
    }

    [Fact]
    public async Task Selecting_claim_sets_target_kind_and_id_for_registration_contract()
    {
        var viewModel = CreateTargetSelectionViewModel(new FakePolicyClaimStorageService(
            ["policy_demo_001"],
            ["claim_demo_001"]));

        await viewModel.LoadTargetOptionsAsync();
        viewModel.TargetKind = DocumentRegistrationViewModel.ClaimTargetKind;
        viewModel.SelectedClaimId = "claim_demo_001";

        Assert.Equal(DocumentRegistrationViewModel.ClaimTargetKind, viewModel.TargetKind);
        Assert.Equal("claim_demo_001", viewModel.TargetId);
    }

    [Fact]
    public async Task LoadTargetOptionsAsync_no_active_policy_shows_empty_state_message()
    {
        var viewModel = CreateTargetSelectionViewModel(new FakePolicyClaimStorageService(
            [],
            ["claim_demo_001"]));

        await viewModel.LoadTargetOptionsAsync();

        Assert.False(viewModel.HasAvailablePolicies);
        Assert.Equal("선택할 수 있는 보험 계약이 없습니다.", viewModel.TargetSelectionMessage);
    }

    [Fact]
    public async Task LoadTargetOptionsAsync_no_active_claim_shows_empty_state_message()
    {
        var viewModel = CreateTargetSelectionViewModel(new FakePolicyClaimStorageService(
            ["policy_demo_001"],
            []));
        viewModel.TargetKind = DocumentRegistrationViewModel.ClaimTargetKind;

        await viewModel.LoadTargetOptionsAsync();

        Assert.False(viewModel.HasAvailableClaims);
        Assert.Equal("선택할 수 있는 청구 건이 없습니다.", viewModel.TargetSelectionMessage);
    }

    [Fact]
    public async Task RegisterAsync_without_selected_policy_target_is_blocked()
    {
        await UsingTempRootsAsync(async (_, attachmentRoot) =>
        {
            var sourcePath = await CreateDummySourceFileAsync(attachmentRoot, "source.pdf");
            var fileAttachment = new SpyFileAttachmentService();
            var viewModel = CreateReadyPolicyViewModel(fileAttachment);
            viewModel.SelectedSourceFilePath = sourcePath;
            viewModel.TargetId = null;
            await viewModel.LoadTargetOptionsAsync();

            Assert.False(await viewModel.RegisterAsync());

            Assert.Equal("문서를 등록하기 전에 연결할 보험 계약을 선택해 주세요.", viewModel.ValidationMessage);
            Assert.False(fileAttachment.CopyCalled);
        });
    }

    [Fact]
    public async Task RegisterAsync_without_selected_claim_target_is_blocked()
    {
        await UsingTempRootsAsync(async (_, attachmentRoot) =>
        {
            var sourcePath = await CreateDummySourceFileAsync(attachmentRoot, "source.pdf");
            var fileAttachment = new SpyFileAttachmentService();
            var viewModel = CreateReadyClaimViewModel(CreateWorkflow(
                new SpyDocumentStorageService(),
                fileAttachment));
            viewModel.SelectedSourceFilePath = sourcePath;
            viewModel.TargetId = null;
            await viewModel.LoadTargetOptionsAsync();

            Assert.False(await viewModel.RegisterAsync());

            Assert.Equal("문서를 등록하기 전에 연결할 청구 건을 선택해 주세요.", viewModel.ValidationMessage);
            Assert.False(fileAttachment.CopyCalled);
        });
    }

    [Fact]
    public async Task RegisterAsync_missing_source_path_rejects_before_workflow_success()
    {
        var storage = new SpyDocumentStorageService();
        var fileAttachment = new SpyFileAttachmentService();
        var viewModel = new DocumentRegistrationViewModel(
            CreateWorkflow(storage, fileAttachment),
            new FakeFilePickerService(null),
            new FakePolicyClaimStorageService(),
            CreateUiTextProvider())
        {
            TargetId = "policy_001",
            DocumentType = "terms",
            DisplayTitle = "Document A"
        };

        await viewModel.RegisterAsync();

        Assert.Equal("파일을 선택해 주세요.", viewModel.ValidationMessage);
        Assert.False(fileAttachment.CopyCalled);
        Assert.Null(viewModel.LastRegistrationSummary);
    }

    [Fact]
    public async Task RegisterAsync_missing_target_id_rejects()
    {
        await UsingTempRootsAsync(async (_, attachmentRoot) =>
        {
            var sourcePath = await CreateDummySourceFileAsync(attachmentRoot, "source.pdf");
            var fileAttachment = new SpyFileAttachmentService();
            var viewModel = CreateReadyPolicyViewModel(fileAttachment);
            viewModel.SelectedSourceFilePath = sourcePath;
            viewModel.TargetId = "";

            await viewModel.RegisterAsync();

            Assert.Equal("연결할 대상을 선택해 주세요.", viewModel.ValidationMessage);
            Assert.False(fileAttachment.CopyCalled);
        });
    }

    [Fact]
    public async Task RegisterAsync_missing_document_type_rejects()
    {
        await UsingTempRootsAsync(async (_, attachmentRoot) =>
        {
            var sourcePath = await CreateDummySourceFileAsync(attachmentRoot, "source.pdf");
            var fileAttachment = new SpyFileAttachmentService();
            var viewModel = CreateReadyPolicyViewModel(fileAttachment);
            viewModel.SelectedSourceFilePath = sourcePath;
            viewModel.DocumentType = "";

            await viewModel.RegisterAsync();

            Assert.Equal("문서 유형을 선택해 주세요.", viewModel.ValidationMessage);
            Assert.False(fileAttachment.CopyCalled);
        });
    }

    [Fact]
    public async Task RegisterAsync_missing_display_title_rejects()
    {
        await UsingTempRootsAsync(async (_, attachmentRoot) =>
        {
            var sourcePath = await CreateDummySourceFileAsync(attachmentRoot, "source.pdf");
            var fileAttachment = new SpyFileAttachmentService();
            var viewModel = CreateReadyPolicyViewModel(fileAttachment);
            viewModel.SelectedSourceFilePath = sourcePath;
            viewModel.DisplayTitle = "";

            await viewModel.RegisterAsync();

            Assert.Equal("표시 제목을 입력해 주세요.", viewModel.ValidationMessage);
            Assert.False(fileAttachment.CopyCalled);
        });
    }

    [Fact]
    public async Task RegisterAsync_invalid_target_kind_rejects()
    {
        await UsingTempRootsAsync(async (_, attachmentRoot) =>
        {
            var sourcePath = await CreateDummySourceFileAsync(attachmentRoot, "source.pdf");
            var fileAttachment = new SpyFileAttachmentService();
            var viewModel = CreateReadyPolicyViewModel(fileAttachment);
            viewModel.SelectedSourceFilePath = sourcePath;
            viewModel.TargetKind = "unknown";

            await viewModel.RegisterAsync();

            Assert.Equal("연결 대상 유형을 선택해 주세요.", viewModel.ValidationMessage);
            Assert.False(fileAttachment.CopyCalled);
        });
    }

    [Fact]
    public async Task RegisterAsync_null_reference_date_is_allowed_and_preserved()
    {
        await UsingTempRootsAsync(async (metadataRoot, attachmentRoot) =>
        {
            var sourcePath = await CreateDummySourceFileAsync(attachmentRoot, "source.pdf");
            var storage = new JsonDocumentStorageService(metadataRoot);
            var viewModel = CreateReadyPolicyViewModel(CreateWorkflow(
                storage,
                new LocalFileAttachmentService(attachmentRoot)));
            viewModel.SelectedSourceFilePath = sourcePath;
            viewModel.ReferenceDate = default;

            await viewModel.RegisterAsync();

            Assert.Null(viewModel.ValidationMessage);
            Assert.Null(viewModel.ReferenceDate);
            var document = Assert.Single(await storage.GetDocumentsAsync());
            Assert.Null(document.ReferenceDate);
            var managedFile = Assert.Single(
                Directory.GetFiles(attachmentRoot, "*", SearchOption.AllDirectories),
                path => !string.Equals(path, sourcePath, StringComparison.Ordinal));
            Assert.Contains("_00010101_", Path.GetFileName(managedFile), StringComparison.Ordinal);
        });
    }

    [Fact]
    public async Task RegisterAsync_registers_policy_document_with_temp_services()
    {
        await UsingTempRootsAsync(async (metadataRoot, attachmentRoot) =>
        {
            var sourcePath = await CreateDummySourceFileAsync(attachmentRoot, "source.pdf");
            var workflow = CreateWorkflow(
                new JsonDocumentStorageService(metadataRoot),
                new LocalFileAttachmentService(attachmentRoot));
            var viewModel = CreateReadyPolicyViewModel(workflow);
            viewModel.SelectedSourceFilePath = sourcePath;

            Assert.True(await viewModel.RegisterAsync());

            Assert.Null(viewModel.ValidationMessage);
            Assert.Equal("문서 등록이 완료되었습니다.", viewModel.StatusMessage);
            Assert.False(viewModel.IsBusy);
            Assert.NotNull(viewModel.LastRegistrationSummary);
            Assert.Contains("policy:policy_001", viewModel.LastRegistrationSummary);
            Assert.True(File.Exists(Path.Combine(metadataRoot, "documents.json")));
            Assert.True(File.Exists(Path.Combine(metadataRoot, "policy-documents.json")));
        });
    }

    [Fact]
    public async Task RegisterAsync_registers_claim_document_with_temp_services()
    {
        await UsingTempRootsAsync(async (metadataRoot, attachmentRoot) =>
        {
            var sourcePath = await CreateDummySourceFileAsync(attachmentRoot, "source.png");
            var workflow = CreateWorkflow(
                new JsonDocumentStorageService(metadataRoot),
                new LocalFileAttachmentService(attachmentRoot));
            var viewModel = CreateReadyClaimViewModel(workflow);
            viewModel.SelectedSourceFilePath = sourcePath;

            await viewModel.RegisterAsync();

            Assert.Null(viewModel.ValidationMessage);
            Assert.Equal("문서 등록이 완료되었습니다.", viewModel.StatusMessage);
            Assert.False(viewModel.IsBusy);
            Assert.NotNull(viewModel.LastRegistrationSummary);
            Assert.Contains("claim:claim_001", viewModel.LastRegistrationSummary);
            Assert.True(File.Exists(Path.Combine(metadataRoot, "documents.json")));
            Assert.True(File.Exists(Path.Combine(metadataRoot, "claim-documents.json")));
        });
    }

    [Fact]
    public async Task RegisterAsync_source_unavailable_retains_inputs_for_retry()
    {
        var missingSourcePath = Path.Combine(Path.GetTempPath(), "FamilyClaimRef.App.Tests", "missing.pdf");
        var viewModel = CreateReadyPolicyViewModel();
        viewModel.SelectedSourceFilePath = missingSourcePath;

        await viewModel.RegisterAsync();

        Assert.Equal(
            "선택한 파일을 읽을 수 없습니다. 다시 선택해 주세요.",
            viewModel.ValidationMessage);
        Assert.Equal(
            "입력 내용을 유지했습니다. 확인 후 다시 시도해 주세요.",
            viewModel.StatusMessage);
        Assert.False(viewModel.IsBusy);
        Assert.Null(viewModel.LastRegistrationSummary);
    }

    [Fact]
    public async Task RegisterAsync_aggregate_exception_updates_cleanup_failure_message()
    {
        await UsingTempRootsAsync(async (_, attachmentRoot) =>
        {
            var sourcePath = await CreateDummySourceFileAsync(attachmentRoot, "source.pdf");
            var storage = new SpyDocumentStorageService(
                failAddPolicyDocument: true,
                failDisableDocument: true);
            var fileAttachment = new SpyFileAttachmentService(failDelete: true);
            var viewModel = CreateReadyPolicyViewModel(CreateWorkflow(storage, fileAttachment));
            viewModel.SelectedSourceFilePath = sourcePath;

            await viewModel.RegisterAsync();

            Assert.Equal("등록 중 일부 정리가 실패했습니다. 다시 시도하거나 관리자에게 문의하세요.", viewModel.StatusMessage);
            Assert.False(viewModel.IsBusy);
            Assert.Null(viewModel.LastRegistrationSummary);
            Assert.True(fileAttachment.DeleteCalled);
            Assert.True(storage.DisableDocumentCalled);
        });
    }

    [Fact]
    public async Task RegisterAsync_does_not_create_project_root_attachment_or_data_files()
    {
        var projectRoot = FindProjectRoot();
        var attachmentsBefore = SnapshotFiles(Path.Combine(projectRoot, "attachments"));
        var dataLocalBefore = SnapshotFiles(Path.Combine(projectRoot, "data", "local"));

        await UsingTempRootsAsync(async (metadataRoot, attachmentRoot) =>
        {
            var sourcePath = await CreateDummySourceFileAsync(attachmentRoot, "source.pdf");
            var workflow = CreateWorkflow(
                new JsonDocumentStorageService(metadataRoot),
                new LocalFileAttachmentService(attachmentRoot));
            var viewModel = CreateReadyPolicyViewModel(workflow);
            viewModel.SelectedSourceFilePath = sourcePath;

            await viewModel.RegisterAsync();
        });

        var attachmentsAfter = SnapshotFiles(Path.Combine(projectRoot, "attachments"));
        var dataLocalAfter = SnapshotFiles(Path.Combine(projectRoot, "data", "local"));
        Assert.Equal(attachmentsBefore, attachmentsAfter);
        Assert.Equal(dataLocalBefore, dataLocalAfter);
    }

    private static DocumentRegistrationViewModel CreateReadyPolicyViewModel(
        IFileAttachmentService? fileAttachment = null)
    {
        return CreateReadyPolicyViewModel(CreateWorkflow(
            new SpyDocumentStorageService(),
            fileAttachment ?? new SpyFileAttachmentService()));
    }

    private static DocumentRegistrationViewModel CreateTargetSelectionViewModel(
        IPolicyClaimStorageService policyClaimStorageService,
        FakeFilePickerService? filePickerService = null,
        IFileAttachmentService? fileAttachmentService = null)
    {
        return new DocumentRegistrationViewModel(
            CreateWorkflow(
                new SpyDocumentStorageService(),
                fileAttachmentService ?? new SpyFileAttachmentService()),
            filePickerService ?? new FakeFilePickerService(null),
            policyClaimStorageService,
            CreateUiTextProvider());
    }

    private static DocumentRegistrationViewModel CreateReadyPolicyViewModel(
        DocumentRegistrationWorkflow workflow)
    {
        return new DocumentRegistrationViewModel(
            workflow,
            new FakeFilePickerService(null),
            new FakePolicyClaimStorageService(
                ["policy_001"],
                ["claim_001"]),
            CreateUiTextProvider())
        {
            TargetKind = DocumentRegistrationViewModel.PolicyTargetKind,
            TargetId = "policy_001",
            DocumentType = "terms",
            DisplayTitle = "Document A",
            ReferenceDate = new DateOnly(2026, 7, 1)
        };
    }

    private static DocumentRegistrationViewModel CreateReadyClaimViewModel(
        DocumentRegistrationWorkflow workflow)
    {
        return new DocumentRegistrationViewModel(
            workflow,
            new FakeFilePickerService(null),
            new FakePolicyClaimStorageService(
                ["policy_001"],
                ["claim_001"]),
            CreateUiTextProvider())
        {
            TargetKind = DocumentRegistrationViewModel.ClaimTargetKind,
            TargetId = "claim_001",
            DocumentType = "receipt",
            DisplayTitle = "Document A",
            ReferenceDate = new DateOnly(2026, 7, 1)
        };
    }

    private static DocumentRegistrationWorkflow CreateWorkflow(
        IDocumentStorageService storage,
        IFileAttachmentService fileAttachment)
    {
        var policyClaimStorage = new FakePolicyClaimStorageService(
            ["policy_001"],
            ["claim_001"]);
        return new DocumentRegistrationWorkflow(
            new DocumentAttachmentCoordinator(storage, fileAttachment),
            new DocumentLinkCoordinator(
                storage,
                policyClaimStorage),
            storage,
            fileAttachment,
            policyClaimStorage);
    }

    private static IUiTextProvider CreateUiTextProvider()
    {
        return new ResourceUiTextProvider(new Dictionary<string, string>(StringComparer.Ordinal)
        {
            [UiTextKeys.DocumentRegistrationStatusCleanupFailed] =
                "등록 중 일부 정리가 실패했습니다. 다시 시도하거나 관리자에게 문의하세요.",
            [UiTextKeys.DocumentRegistrationMessageNoActiveClaim] = "선택할 수 있는 청구 건이 없습니다.",
            [UiTextKeys.DocumentRegistrationMessageNoActivePolicy] = "선택할 수 있는 보험 계약이 없습니다.",
            [UiTextKeys.DocumentRegistrationStatusFailed] = "문서 등록에 실패했습니다.",
            [UiTextKeys.DocumentRegistrationStatusCompleted] = "문서 등록이 완료되었습니다.",
            [UiTextKeys.DocumentRegistrationValidationSelectClaimBeforeRegister] =
                "문서를 등록하기 전에 연결할 청구 건을 선택해 주세요.",
            [UiTextKeys.DocumentRegistrationValidationSelectPolicyBeforeRegister] =
                "문서를 등록하기 전에 연결할 보험 계약을 선택해 주세요.",
            [UiTextKeys.DocumentRegistrationStatusFileSelected] = "파일을 선택했습니다.",
            [UiTextKeys.DocumentRegistrationValidationSelectFile] = "파일을 선택해 주세요.",
            [UiTextKeys.DocumentRegistrationValidationSelectTargetKind] = "연결 대상 유형을 선택해 주세요.",
            [UiTextKeys.DocumentRegistrationValidationSelectTarget] = "연결할 대상을 선택해 주세요.",
            [UiTextKeys.DocumentRegistrationValidationSelectDocumentType] = "문서 유형을 선택해 주세요.",
            [UiTextKeys.DocumentRegistrationValidationEnterDisplayTitle] = "표시 제목을 입력해 주세요.",
            [UiTextKeys.DocumentRegistrationValidationSelectReferenceDate] = "기준일을 선택해 주세요."
            ,
            [UiTextKeys.ProductDocumentRegistrationValidationUnsupportedFileType] =
                "지원하지 않는 파일 형식입니다.",
            [UiTextKeys.ProductDocumentRegistrationValidationEmptyFile] =
                "빈 파일은 등록할 수 없습니다.",
            [UiTextKeys.ProductDocumentRegistrationValidationFileTooLarge] =
                "파일 크기는 25MB 이하여야 합니다.",
            [UiTextKeys.ProductDocumentRegistrationValidationSourceUnavailable] =
                "선택한 파일을 읽을 수 없습니다. 다시 선택해 주세요.",
            [UiTextKeys.ProductDocumentRegistrationValidationSourceChanged] =
                "선택 후 파일이 변경되었습니다. 다시 선택해 주세요.",
            [UiTextKeys.ProductDocumentRegistrationValidationDuplicateDocument] =
                "같은 대상에 동일한 문서가 이미 등록되어 있습니다.",
            [UiTextKeys.ProductDocumentRegistrationStatusCanceled] =
                "파일 선택을 취소했습니다.",
            [UiTextKeys.ProductDocumentRegistrationStatusRetryAvailable] =
                "입력 내용을 유지했습니다. 확인 후 다시 시도해 주세요."
        });
    }

    private static async Task<string> CreateDummySourceFileAsync(string rootPath, string fileName)
    {
        var sourceDirectory = Path.Combine(rootPath, "source");
        Directory.CreateDirectory(sourceDirectory);
        var sourcePath = Path.Combine(sourceDirectory, fileName);
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        var content = extension switch
        {
            ".pdf" => System.Text.Encoding.ASCII.GetBytes("%PDF-1.4\nsynthetic"),
            ".png" => new byte[]
            {
                0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, 0x01
            },
            ".jpg" or ".jpeg" => new byte[] { 0xFF, 0xD8, 0xFF, 0x01 },
            _ => System.Text.Encoding.ASCII.GetBytes("unsupported")
        };
        await File.WriteAllBytesAsync(sourcePath, content);

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

    private sealed class FakeFilePickerService : IFilePickerService
    {
        private readonly FilePickerResult? result;

        public FakeFilePickerService(FilePickerResult? result)
        {
            this.result = result;
        }

        public int PickCallCount { get; private set; }

        public Task<FilePickerResult?> PickDocumentFileAsync(
            CancellationToken cancellationToken = default)
        {
            PickCallCount++;
            return Task.FromResult(result);
        }
    }

    private sealed class SpyFileAttachmentService : IFileAttachmentService
    {
        private readonly bool failDelete;

        public SpyFileAttachmentService(bool failDelete = false)
        {
            this.failDelete = failDelete;
        }

        public bool CopyCalled { get; private set; }

        public bool DeleteCalled { get; private set; }

        public HashSet<string> CopiedRelativePaths { get; } = new(StringComparer.Ordinal);

        public Task<StagedFileAttachment> StageDocumentFileAsync(
            string sourceFilePath,
            CancellationToken cancellationToken = default)
        {
            CopyCalled = true;
            return Task.FromResult(new StagedFileAttachment(
                $"staging/{Guid.NewGuid():N}.tmp",
                sourceFilePath));
        }

        public Task<FileAttachmentCopyResult> FinalizeStagedDocumentFileAsync(
            StagedFileAttachment stagedFile,
            string physicalFileName,
            CancellationToken cancellationToken = default)
        {
            var relativePath = $"documents/{physicalFileName}";
            CopiedRelativePaths.Add(relativePath);
            return Task.FromResult(new FileAttachmentCopyResult(
                relativePath,
                physicalFileName,
                stagedFile.Validation?.NormalizedExtension
                    ?? Path.GetExtension(physicalFileName).TrimStart('.').ToLowerInvariant(),
                stagedFile.Validation?.ByteLength ?? 13,
                stagedFile.Validation?.ValidatedFileType,
                stagedFile.Validation?.Sha256,
                stagedFile.Validation?.SafeDisplayName));
        }

        public Task DeleteStagedFileIfExistsAsync(
            StagedFileAttachment stagedFile,
            CancellationToken cancellationToken = default)
        {
            DeleteCalled = true;
            if (failDelete)
            {
                throw new IOException("Delete failed.");
            }

            return Task.CompletedTask;
        }

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
            return Task.FromResult(CopiedRelativePaths.Contains(relativePath));
        }
    }

    private sealed class SpyDocumentStorageService : IDocumentStorageService
    {
        private readonly bool failAddPolicyDocument;
        private readonly bool failDisableDocument;
        private readonly List<DocumentRecord> documents = [];
        private readonly List<PolicyDocumentRecord> policyDocuments = [];
        private readonly List<ClaimDocumentRecord> claimDocuments = [];

        public SpyDocumentStorageService(
            bool failAddPolicyDocument = false,
            bool failDisableDocument = false)
        {
            this.failAddPolicyDocument = failAddPolicyDocument;
            this.failDisableDocument = failDisableDocument;
        }

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
                null,
                draft.OriginalDisplayFileName,
                draft.ValidatedFileType,
                draft.ByteLength,
                draft.Sha256,
                draft.ReferenceDate,
                draft.DocumentType);

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

            return Task.CompletedTask;
        }

        public Task<IReadOnlyList<PolicyDocumentRecord>> GetPolicyDocumentsAsync(
            string policyId,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IReadOnlyList<PolicyDocumentRecord>>(
                policyDocuments.Where(policyDocument => policyDocument.PolicyId == policyId).ToList());
        }

        public Task<PolicyDocumentRecord> AddPolicyDocumentAsync(
            PolicyDocumentDraft draft,
            CancellationToken cancellationToken = default)
        {
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

        public int GetPoliciesCallCount { get; private set; }

        public int GetClaimsCallCount { get; private set; }

        public void ReplaceActiveTargets(
            IEnumerable<string> policyIds,
            IEnumerable<string> claimIds)
        {
            activePolicyIds.Clear();
            activePolicyIds.UnionWith(policyIds);
            activeClaimIds.Clear();
            activeClaimIds.UnionWith(claimIds);
        }

        public Task<IReadOnlyList<PolicyRecord>> GetPoliciesAsync(
            CancellationToken cancellationToken = default)
        {
            GetPoliciesCallCount++;
            return Task.FromResult<IReadOnlyList<PolicyRecord>>(
                activePolicyIds.Select(CreatePolicyRecord).ToList());
        }

        public Task<PolicyRecord?> GetPolicyAsync(
            string id,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(
                activePolicyIds.Contains(id)
                    ? CreatePolicyRecord(id)
                    : null);
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
            GetClaimsCallCount++;
            return Task.FromResult<IReadOnlyList<ClaimRecord>>(
                activeClaimIds.Select(CreateClaimRecord).ToList());
        }

        public Task<IReadOnlyList<ClaimRecord>> GetClaimsByPolicyIdAsync(
            string policyId,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IReadOnlyList<ClaimRecord>>(
                activeClaimIds
                    .Select(CreateClaimRecord)
                    .Where(claim => claim.PolicyId == policyId)
                    .ToList());
        }

        public Task<ClaimRecord?> GetClaimAsync(
            string id,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(
                activeClaimIds.Contains(id)
                    ? CreateClaimRecord(id)
                    : null);
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

        private static PolicyRecord CreatePolicyRecord(string id)
        {
            var timestamp = DateTimeOffset.UtcNow;
            return new PolicyRecord(
                id,
                id,
                new DateOnly(2026, 7, 1),
                timestamp,
                timestamp,
                null);
        }

        private static ClaimRecord CreateClaimRecord(string id)
        {
            var timestamp = DateTimeOffset.UtcNow;
            return new ClaimRecord(
                id,
                "policy_demo_001",
                id,
                new DateOnly(2026, 7, 1),
                timestamp,
                timestamp,
                null);
        }
    }
}
