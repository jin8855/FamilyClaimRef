using System.Reflection;
using FamilyClaimRef.App.Composition;
using FamilyClaimRef.App.Models.Storage;
using FamilyClaimRef.App.Services;
using FamilyClaimRef.App.Services.Localization;
using FamilyClaimRef.App.Services.Runtime;
using FamilyClaimRef.App.Services.Storage;
using FamilyClaimRef.App.Services.UI;
using FamilyClaimRef.App.ViewModels;
using Xunit;

namespace FamilyClaimRef.App.Tests;

public sealed class DocumentRegistrationLifecycleGate8Tests
{
    private static readonly DateOnly ReferenceDate = new(2026, 7, 24);

    [Fact]
    public async Task U01_cancel_without_prior_file_preserves_empty_state()
    {
        await UsingContextAsync(async context =>
        {
            var viewModel = context.CreateViewModel(
                new QueueFilePickerService((FilePickerResult?)null));

            await viewModel.SelectFileAsync();

            Assert.Null(viewModel.SelectedSourceFilePath);
            Assert.Null(viewModel.SelectedSourceFileDisplayName);
            Assert.Equal("파일 선택을 취소했습니다.", viewModel.StatusMessage);
            Assert.Empty(await context.DocumentStorage.GetDocumentsAsync());
        });
    }

    [Fact]
    public async Task U02_cancel_with_prior_file_preserves_previous_snapshot()
    {
        await UsingContextAsync(async context =>
        {
            var previousPath = await context.CreatePngAsync("previous.png", 0x01);
            var previous = await context.Validation.ValidateSourceAsync(previousPath);
            var viewModel = context.CreateViewModel(new QueueFilePickerService(
                new FilePickerResult(previousPath, previous.SafeDisplayName, previous),
                null));
            await viewModel.SelectFileAsync();

            await viewModel.SelectFileAsync();

            Assert.Equal(previousPath, viewModel.SelectedSourceFilePath);
            Assert.Equal("previous.png", viewModel.SelectedSourceFileDisplayName);
            Assert.Equal("파일 선택을 취소했습니다.", viewModel.StatusMessage);
        });
    }

    [Fact]
    public async Task U03_valid_replacement_changes_only_file_snapshot()
    {
        await UsingContextAsync(async context =>
        {
            var firstPath = await context.CreatePngAsync("first.png", 0x02);
            var secondPath = await context.CreatePngAsync("second.png", 0x03);
            var first = await context.Validation.ValidateSourceAsync(firstPath);
            var second = await context.Validation.ValidateSourceAsync(secondPath);
            var viewModel = context.CreateViewModel(new QueueFilePickerService(
                new FilePickerResult(firstPath, first.SafeDisplayName, first),
                new FilePickerResult(secondPath, second.SafeDisplayName, second)));
            viewModel.DocumentType = "terms";
            viewModel.DisplayTitle = "Synthetic title";
            viewModel.ReferenceDate = ReferenceDate;
            viewModel.TargetKind = DocumentRegistrationViewModel.PolicyTargetKind;
            viewModel.TargetId = "policy-retained";
            await viewModel.SelectFileAsync();

            await viewModel.SelectFileAsync();

            Assert.Equal(secondPath, viewModel.SelectedSourceFilePath);
            Assert.Equal("second.png", viewModel.SelectedSourceFileDisplayName);
            Assert.Equal("terms", viewModel.DocumentType);
            Assert.Equal("Synthetic title", viewModel.DisplayTitle);
            Assert.Equal("policy-retained", viewModel.TargetId);
            Assert.Null(viewModel.ValidationMessage);
        });
    }

    [Fact]
    public async Task U04_reentry_preserves_draft_and_refreshes_targets()
    {
        await UsingContextAsync(async context =>
        {
            var policy = await context.PolicyClaimStorage.AddPolicyAsync(
                new PolicyDraft("Synthetic policy", ReferenceDate));
            var sourcePath = await context.CreatePngAsync("draft.png", 0x04);
            var snapshot = await context.Validation.ValidateSourceAsync(sourcePath);
            var viewModel = context.CreateViewModel(new QueueFilePickerService(
                new FilePickerResult(sourcePath, snapshot.SafeDisplayName, snapshot)));
            viewModel.DocumentType = "terms";
            viewModel.DisplayTitle = "Synthetic draft";
            viewModel.ReferenceDate = ReferenceDate;
            await viewModel.SelectFileAsync();
            viewModel.SelectedPolicyId = policy.Id;

            await viewModel.LoadTargetOptionsAsync();

            Assert.Equal(sourcePath, viewModel.SelectedSourceFilePath);
            Assert.Equal("terms", viewModel.DocumentType);
            Assert.Equal("Synthetic draft", viewModel.DisplayTitle);
            Assert.Equal(ReferenceDate, viewModel.ReferenceDate);
            Assert.Contains(viewModel.AvailablePolicies, item => item.Id == policy.Id);
        });
    }

    [Fact]
    public async Task U05_reentry_clears_only_inactive_target()
    {
        await UsingContextAsync(async context =>
        {
            var policy = await context.PolicyClaimStorage.AddPolicyAsync(
                new PolicyDraft("Synthetic policy", ReferenceDate));
            var sourcePath = await context.CreatePngAsync("retained.png", 0x05);
            var snapshot = await context.Validation.ValidateSourceAsync(sourcePath);
            var viewModel = context.CreateViewModel(new QueueFilePickerService(
                new FilePickerResult(sourcePath, snapshot.SafeDisplayName, snapshot)));
            viewModel.DocumentType = "terms";
            viewModel.DisplayTitle = "Synthetic retained draft";
            viewModel.ReferenceDate = ReferenceDate;
            await viewModel.SelectFileAsync();
            await viewModel.LoadTargetOptionsAsync();
            viewModel.SelectedPolicyId = policy.Id;
            await context.PolicyClaimStorage.DisablePolicyAsync(policy.Id);

            await viewModel.LoadTargetOptionsAsync();

            Assert.Null(viewModel.SelectedPolicyId);
            Assert.Null(viewModel.TargetId);
            Assert.Equal(sourcePath, viewModel.SelectedSourceFilePath);
            Assert.Equal("Synthetic retained draft", viewModel.DisplayTitle);
        });
    }

    [Fact]
    public async Task U06_success_resets_document_draft_and_retains_active_target()
    {
        await UsingContextAsync(async context =>
        {
            var policy = await context.PolicyClaimStorage.AddPolicyAsync(
                new PolicyDraft("Synthetic policy", ReferenceDate));
            var sourcePath = await context.CreatePngAsync("success.png", 0x06);
            var snapshot = await context.Validation.ValidateSourceAsync(sourcePath);
            var viewModel = context.CreateViewModel(new QueueFilePickerService(
                new FilePickerResult(sourcePath, snapshot.SafeDisplayName, snapshot)));
            await viewModel.LoadTargetOptionsAsync();
            viewModel.SelectedPolicyId = policy.Id;
            viewModel.DocumentType = "terms";
            viewModel.DisplayTitle = "Synthetic success";
            viewModel.ReferenceDate = ReferenceDate;
            await viewModel.SelectFileAsync();

            await viewModel.RegisterAsync();

            Assert.Null(viewModel.SelectedSourceFilePath);
            Assert.Null(viewModel.SelectedSourceFileDisplayName);
            Assert.Null(viewModel.DocumentType);
            Assert.Null(viewModel.DisplayTitle);
            Assert.Null(viewModel.ReferenceDate);
            Assert.Equal(policy.Id, viewModel.SelectedPolicyId);
            Assert.Equal(policy.Id, viewModel.TargetId);
            Assert.Equal("문서 등록이 완료되었습니다.", viewModel.StatusMessage);
        });
    }

    [Fact]
    public async Task U07_recoverable_duplicate_failure_retains_retry_inputs()
    {
        await UsingContextAsync(async context =>
        {
            var policy = await context.PolicyClaimStorage.AddPolicyAsync(
                new PolicyDraft("Synthetic policy", ReferenceDate));
            var sourcePath = await context.CreatePngAsync("duplicate.png", 0x07);
            var snapshot = await context.Validation.ValidateSourceAsync(sourcePath);
            await context.RegisterPolicyAsync(policy.Id, sourcePath, snapshot, "First");
            var viewModel = context.CreateViewModel(new QueueFilePickerService(
                new FilePickerResult(sourcePath, snapshot.SafeDisplayName, snapshot)));
            await viewModel.LoadTargetOptionsAsync();
            viewModel.SelectedPolicyId = policy.Id;
            viewModel.DocumentType = "terms";
            viewModel.DisplayTitle = "Retry title";
            viewModel.ReferenceDate = ReferenceDate;
            await viewModel.SelectFileAsync();

            await viewModel.RegisterAsync();

            Assert.Equal(sourcePath, viewModel.SelectedSourceFilePath);
            Assert.Equal("terms", viewModel.DocumentType);
            Assert.Equal("Retry title", viewModel.DisplayTitle);
            Assert.Equal(policy.Id, viewModel.SelectedPolicyId);
            Assert.Equal("같은 대상에 동일한 문서가 이미 등록되어 있습니다.", viewModel.ValidationMessage);
            Assert.Equal("입력 내용을 유지했습니다. 확인 후 다시 시도해 주세요.", viewModel.StatusMessage);
        });
    }

    [Fact]
    public async Task U08_busy_state_prevents_duplicate_command_execution()
    {
        await UsingContextAsync(async context =>
        {
            var policy = await context.PolicyClaimStorage.AddPolicyAsync(
                new PolicyDraft("Synthetic policy", ReferenceDate));
            var sourcePath = await context.CreatePngAsync("busy.png", 0x08);
            var snapshot = await context.Validation.ValidateSourceAsync(sourcePath);
            var delayedFileService = new DelayedFileAttachmentService(context.FileAttachmentService);
            var viewModel = context.CreateViewModel(
                new QueueFilePickerService(new FilePickerResult(
                    sourcePath,
                    snapshot.SafeDisplayName,
                    snapshot)),
                delayedFileService);
            await viewModel.LoadTargetOptionsAsync();
            viewModel.SelectedPolicyId = policy.Id;
            viewModel.DocumentType = "terms";
            viewModel.DisplayTitle = "Synthetic busy";
            viewModel.ReferenceDate = ReferenceDate;
            await viewModel.SelectFileAsync();

            var first = viewModel.RegisterAsync();
            await delayedFileService.StageEntered.Task.WaitAsync(TimeSpan.FromSeconds(5));
            var second = viewModel.RegisterAsync();
            await second;
            delayedFileService.ReleaseStage.TrySetResult();
            await first;

            Assert.Single(await context.DocumentStorage.GetDocumentsAsync());
            Assert.False(viewModel.IsBusy);
        });
    }

    [Fact]
    public async Task U18_product_registration_copy_exposes_no_forbidden_internal_values()
    {
        var projectRoot = FindProjectRoot();
        var xaml = await File.ReadAllTextAsync(Path.Combine(
            projectRoot,
            "app",
            "FamilyClaimRef.App",
            "Views",
            "ProductDocumentRegistrationView.xaml"));
        var codeBehind = await File.ReadAllTextAsync(Path.Combine(
            projectRoot,
            "app",
            "FamilyClaimRef.App",
            "Views",
            "ProductDocumentRegistrationView.xaml.cs"));

        Assert.DoesNotContain("LastRegistrationSummary", xaml, StringComparison.Ordinal);
        Assert.DoesNotContain("SelectedSourceFilePath", xaml, StringComparison.Ordinal);
        Assert.DoesNotContain("System.IO", codeBehind, StringComparison.Ordinal);
        Assert.DoesNotContain("Json", codeBehind, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void C01_app_services_reuses_one_lower_registration_workflow()
    {
        var root = Path.Combine(
            Path.GetTempPath(),
            "FamilyClaimRef",
            "Gate8",
            $"gate8-validation-{Guid.NewGuid():N}");
        var services = AppServices.Create(
            new StubRuntimeRootProvider(RuntimeRootPaths.FromRuntimeRoot(root)));
        var field = typeof(DocumentRegistrationViewModel).GetField(
            "registrationWorkflow",
            BindingFlags.Instance | BindingFlags.NonPublic);

        var workflowField = field
            ?? throw new InvalidOperationException("Registration workflow field was not found.");
        Assert.Same(
            workflowField.GetValue(services.MainWindowViewModel.DocumentRegistration),
            workflowField.GetValue(services.ProductShellViewModel.DocumentRegistration));
        Assert.False(Directory.Exists(root));
    }

    [Fact]
    public async Task C02_product_view_and_code_behind_have_no_direct_file_or_json_calls()
    {
        var viewPath = Path.Combine(
            FindProjectRoot(),
            "app",
            "FamilyClaimRef.App",
            "Views",
            "ProductDocumentRegistrationView.xaml");
        var codePath = Path.ChangeExtension(viewPath, ".xaml.cs");
        var content = $"{await File.ReadAllTextAsync(viewPath)}\n{await File.ReadAllTextAsync(codePath)}";

        Assert.DoesNotContain("File.", content, StringComparison.Ordinal);
        Assert.DoesNotContain("Directory.", content, StringComparison.Ordinal);
        Assert.DoesNotContain("Json", content, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task C03_product_shell_remains_default_startup_and_main_window_default_count_is_zero()
    {
        var appCode = await File.ReadAllTextAsync(Path.Combine(
            FindProjectRoot(),
            "app",
            "FamilyClaimRef.App",
            "App.xaml.cs"));

        Assert.Contains("ProductShellWindow", appCode, StringComparison.Ordinal);
        Assert.DoesNotContain("new MainWindow", appCode, StringComparison.Ordinal);
    }

    [Fact]
    public void C04_navigation_has_five_destinations_and_one_selection()
    {
        var root = Path.Combine(
            Path.GetTempPath(),
            "FamilyClaimRef",
            "Gate8",
            $"gate8-validation-{Guid.NewGuid():N}");
        var shell = AppServices.Create(
            new StubRuntimeRootProvider(RuntimeRootPaths.FromRuntimeRoot(root))).ProductShellViewModel;

        Assert.Equal(5, shell.NavigationItems.Count);
        var selectedNavigationItem =
            Assert.IsType<ProductNavigationItemViewModel>(shell.SelectedNavigationItem);
        Assert.Contains(selectedNavigationItem, shell.NavigationItems);
        Assert.False(Directory.Exists(root));
    }

    [Fact]
    public void C05_resource_constants_have_416_416_and_product_359_359_parity()
    {
        var resources = LoadResources();
        var constants = typeof(UiTextKeys)
            .GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(field => field.IsLiteral && field.FieldType == typeof(string))
            .Select(field => (string)field.GetRawConstantValue()!)
            .ToArray();

        Assert.Equal(416, resources.Count);
        Assert.Equal(416, constants.Length);
        Assert.Equal(
            resources.Keys.Order(StringComparer.Ordinal),
            constants.Order(StringComparer.Ordinal));
        Assert.Equal(359, resources.Keys.Count(key => key.StartsWith("Ui.Product.", StringComparison.Ordinal)));
        Assert.Equal(359, constants.Count(key => key.StartsWith("Ui.Product.", StringComparison.Ordinal)));
    }

    [Fact]
    public void C06_picker_extensions_equal_file_name_policy_allowlist()
    {
        var filterField = typeof(WpfFilePickerService).GetField(
            "DocumentFilter",
            BindingFlags.NonPublic | BindingFlags.Static);
        var filter = Assert.IsType<string>(filterField?.GetRawConstantValue());
        var pickerExtensions = filter
            .Split('|')[1]
            .Split(';')
            .Select(pattern => pattern.TrimStart('*', '.').ToLowerInvariant())
            .ToHashSet(StringComparer.Ordinal);
        var candidates = new[] { "pdf", "jpg", "jpeg", "png", "webp", "bmp" };
        var policyExtensions = candidates
            .Where(extension => Record.Exception(() =>
                FileNamePolicyService.CreatePhysicalFileName(
                    "policy",
                    "document",
                    ReferenceDate,
                    "terms",
                    extension,
                    1)) is null)
            .ToHashSet(StringComparer.Ordinal);

        Assert.Equal(policyExtensions.Order(StringComparer.Ordinal), pickerExtensions.Order(StringComparer.Ordinal));
    }

    private static IReadOnlyDictionary<string, string> LoadResources()
    {
        var path = Path.Combine(
            FindProjectRoot(),
            "app",
            "FamilyClaimRef.App",
            "Resources",
            "UiStrings.xaml");
        var document = System.Xml.Linq.XDocument.Load(path);
        var keyName = System.Xml.Linq.XName.Get(
            "Key",
            "http://schemas.microsoft.com/winfx/2006/xaml");
        return document
            .Descendants()
            .Where(element => element.Attribute(keyName) is not null)
            .ToDictionary(
                element => element.Attribute(keyName)!.Value,
                element => element.Value,
                StringComparer.Ordinal);
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
            Directory.CreateDirectory(InputRoot);
            DocumentStorage = new JsonDocumentStorageService(Path.Combine(runtimeRoot, "data", "local"));
            PolicyClaimStorage = new JsonPolicyClaimStorageService(Path.Combine(runtimeRoot, "data", "local"));
            FileAttachmentService = new LocalFileAttachmentService(Path.Combine(runtimeRoot, "attachments"));
            Validation = new DocumentFileValidationService();
        }

        public string Root { get; }

        public string InputRoot { get; }

        public JsonDocumentStorageService DocumentStorage { get; }

        public JsonPolicyClaimStorageService PolicyClaimStorage { get; }

        public LocalFileAttachmentService FileAttachmentService { get; }

        public DocumentFileValidationService Validation { get; }

        public DocumentRegistrationViewModel CreateViewModel(
            IFilePickerService picker,
            IFileAttachmentService? fileAttachmentService = null)
        {
            var workflow = CreateWorkflow(fileAttachmentService ?? FileAttachmentService);
            return new DocumentRegistrationViewModel(
                workflow,
                picker,
                PolicyClaimStorage,
                new ResourceUiTextProvider(LoadResources()),
                Validation);
        }

        public Task<PolicyDocumentRegistrationResult> RegisterPolicyAsync(
            string policyId,
            string sourcePath,
            DocumentFileValidationResult snapshot,
            string displayTitle)
        {
            return CreateWorkflow(FileAttachmentService).RegisterPolicyDocumentAsync(
                new PolicyDocumentRegistrationRequest(
                    sourcePath,
                    policyId,
                    "terms",
                    displayTitle,
                    ReferenceDate,
                    snapshot));
        }

        public async Task<string> CreatePngAsync(string fileName, byte marker)
        {
            var path = Path.Combine(InputRoot, fileName);
            await File.WriteAllBytesAsync(
                path,
                [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, marker]);
            return path;
        }

        public void Dispose()
        {
            if (Directory.Exists(Root))
            {
                Directory.Delete(Root, recursive: true);
            }
        }

        private DocumentRegistrationWorkflow CreateWorkflow(IFileAttachmentService fileAttachmentService)
        {
            var coordinator = new DocumentAttachmentCoordinator(
                DocumentStorage,
                fileAttachmentService,
                Validation);
            var linkCoordinator = new DocumentLinkCoordinator(DocumentStorage, PolicyClaimStorage);
            return new DocumentRegistrationWorkflow(
                coordinator,
                linkCoordinator,
                DocumentStorage,
                fileAttachmentService,
                PolicyClaimStorage);
        }
    }

    private sealed class QueueFilePickerService(params FilePickerResult?[] results) : IFilePickerService
    {
        private readonly Queue<FilePickerResult?> queue = new(results);

        public Task<FilePickerResult?> PickDocumentFileAsync(
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(queue.Count == 0 ? null : queue.Dequeue());
        }
    }

    private sealed class DelayedFileAttachmentService(IFileAttachmentService inner)
        : IFileAttachmentService
    {
        public TaskCompletionSource StageEntered { get; } =
            new(TaskCreationOptions.RunContinuationsAsynchronously);

        public TaskCompletionSource ReleaseStage { get; } =
            new(TaskCreationOptions.RunContinuationsAsynchronously);

        public async Task<StagedFileAttachment> StageDocumentFileAsync(
            string sourceFilePath,
            CancellationToken cancellationToken = default)
        {
            StageEntered.TrySetResult();
            await ReleaseStage.Task.WaitAsync(cancellationToken);
            return await inner.StageDocumentFileAsync(sourceFilePath, cancellationToken);
        }

        public Task<FileAttachmentCopyResult> FinalizeStagedDocumentFileAsync(
            StagedFileAttachment stagedFile,
            string physicalFileName,
            CancellationToken cancellationToken = default)
        {
            return inner.FinalizeStagedDocumentFileAsync(stagedFile, physicalFileName, cancellationToken);
        }

        public Task DeleteStagedFileIfExistsAsync(
            StagedFileAttachment stagedFile,
            CancellationToken cancellationToken = default)
        {
            return inner.DeleteStagedFileIfExistsAsync(stagedFile, cancellationToken);
        }

        public Task<FileAttachmentCopyResult> CopyDocumentFileAsync(
            string sourceFilePath,
            string physicalFileName,
            CancellationToken cancellationToken = default)
        {
            return inner.CopyDocumentFileAsync(sourceFilePath, physicalFileName, cancellationToken);
        }

        public Task DeleteDocumentFileIfExistsAsync(
            string relativePath,
            CancellationToken cancellationToken = default)
        {
            return inner.DeleteDocumentFileIfExistsAsync(relativePath, cancellationToken);
        }

        public Task<bool> DocumentFileExistsAsync(
            string relativePath,
            CancellationToken cancellationToken = default)
        {
            return inner.DocumentFileExistsAsync(relativePath, cancellationToken);
        }
    }

    private sealed class StubRuntimeRootProvider(RuntimeRootPaths paths) : IRuntimeRootProvider
    {
        public RuntimeRootPaths GetRuntimeRootPaths() => paths;
    }
}
