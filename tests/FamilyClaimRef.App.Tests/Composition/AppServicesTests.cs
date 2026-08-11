using FamilyClaimRef.App.Composition;
using FamilyClaimRef.App.Services.Runtime;
using FamilyClaimRef.App.Services.Storage;
using FamilyClaimRef.App.ViewModels;
using Xunit;

namespace FamilyClaimRef.App.Tests;

public sealed class AppServicesTests
{
    [Fact]
    public void Create_uses_runtime_root_provider_paths_consistently()
    {
        var runtimeRoot = Path.Combine(
            Path.GetTempPath(),
            "FamilyClaimRef.App.Tests",
            "composition",
            Guid.NewGuid().ToString("N"));
        var runtimeRootPaths = RuntimeRootPaths.FromRuntimeRoot(runtimeRoot);

        var services = AppServices.Create(new StubRuntimeRootProvider(runtimeRootPaths));

        Assert.Equal(runtimeRootPaths.RuntimeRootPath, services.RuntimeRootPath);
        Assert.Equal(runtimeRootPaths.MetadataRootPath, services.MetadataRootPath);
        Assert.Equal(runtimeRootPaths.AttachmentRootPath, services.AttachmentRootPath);
        Assert.StartsWith(services.RuntimeRootPath, services.MetadataRootPath, StringComparison.OrdinalIgnoreCase);
        Assert.StartsWith(services.RuntimeRootPath, services.AttachmentRootPath, StringComparison.OrdinalIgnoreCase);
        Assert.IsType<JsonPolicyCoverageStorageService>(services.PolicyCoverageStorageService);
        Assert.False(File.Exists(Path.Combine(
            services.MetadataRootPath,
            JsonPolicyCoverageStorageService.StoreFileName)));
    }

    [Fact]
    public void Create_composes_separate_main_window_and_product_shell_view_model_graphs()
    {
        var services = CreateServices();

        Assert.NotNull(services.MainWindowViewModel);
        Assert.NotNull(services.ProductShellViewModel);
        Assert.IsType<DocumentRegistrationViewModel>(services.MainWindowViewModel.DocumentRegistration);
        Assert.IsType<DocumentRegistrationViewModel>(services.ProductShellViewModel.DocumentRegistration);
        Assert.NotSame(
            services.MainWindowViewModel.DocumentRegistration,
            services.ProductShellViewModel.DocumentRegistration);
        Assert.NotNull(services.ProductShellViewModel.DocumentList);
        Assert.NotNull(services.ProductShellViewModel.PolicyClaimManagement);
        Assert.NotNull(services.ProductShellViewModel.FamilyMemberManagement);
        Assert.NotNull(services.ProductShellViewModel.CategoryManagement);
        Assert.NotNull(services.ProductShellViewModel.ClaimSubmissionManagement);
        Assert.NotNull(services.ProductShellViewModel.ClaimSubmissionManagement.PaymentManagement);
        Assert.NotNull(services.ProductShellViewModel.ClaimCompleteSummary);
        Assert.NotNull(services.ProductShellViewModel.ClaimHistory);
        Assert.NotNull(services.ProductShellViewModel.HomeDashboard);
        Assert.NotSame(
            services.MainWindowViewModel.PolicyClaimManagement,
            services.ProductShellViewModel.PolicyClaimManagement);

        var workflowField = typeof(DocumentRegistrationViewModel).GetField(
            "registrationWorkflow",
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
            ?? throw new InvalidOperationException("Registration workflow field was not found.");
        Assert.Same(
            workflowField.GetValue(services.MainWindowViewModel.DocumentRegistration),
            workflowField.GetValue(services.ProductShellViewModel.DocumentRegistration));
    }

    [Fact]
    public void Create_reuses_mutation_storages_as_unfiltered_history_readers()
    {
        var services = CreateServices();
        var history = services.ProductShellViewModel.ClaimHistory;
        var submissionReader = GetPrivateField(
            history,
            "submissionHistoryStorageReader");
        var paymentReader = GetPrivateField(
            history,
            "paymentHistoryStorageReader");
        var submissionStorage = GetPrivateField(
            services.ProductShellViewModel.ClaimSubmissionManagement,
            "submissionStorageService");
        var paymentStorage = GetPrivateField(
            services.ProductShellViewModel.ClaimSubmissionManagement.PaymentManagement,
            "storageService");

        Assert.IsAssignableFrom<IClaimSubmissionHistoryStorageReader>(submissionReader);
        Assert.IsAssignableFrom<IClaimPaymentHistoryStorageReader>(paymentReader);
        Assert.Same(submissionStorage, submissionReader);
        Assert.Same(paymentStorage, paymentReader);
    }

    [Fact]
    public void Create_reuses_the_same_unfiltered_readers_for_home_dashboard()
    {
        var services = CreateServices();
        var history = services.ProductShellViewModel.ClaimHistory;
        var dashboard = services.ProductShellViewModel.HomeDashboard;

        Assert.Same(
            GetPrivateField(history, "historyStorageReader"),
            GetPrivateField(dashboard, "historyStorageReader"));
        Assert.Same(
            GetPrivateField(history, "submissionHistoryStorageReader"),
            GetPrivateField(dashboard, "submissionHistoryStorageReader"));
        Assert.Same(
            GetPrivateField(history, "paymentHistoryStorageReader"),
            GetPrivateField(dashboard, "paymentHistoryStorageReader"));
        Assert.Same(
            GetPrivateField(history, "familyMemberStorageService"),
            GetPrivateField(dashboard, "familyMemberStorageService"));
    }

    [Fact]
    public void Create_uses_separate_view_model_graphs_for_separate_calls()
    {
        var first = CreateServices();
        var second = CreateServices();

        Assert.NotSame(first.MainWindowViewModel, second.MainWindowViewModel);
        Assert.NotSame(first.ProductShellViewModel, second.ProductShellViewModel);
        Assert.NotSame(
            first.ProductShellViewModel.DocumentRegistration,
            second.ProductShellViewModel.DocumentRegistration);
        Assert.NotSame(first.ProductShellViewModel.DocumentList, second.ProductShellViewModel.DocumentList);
        Assert.NotSame(
            first.ProductShellViewModel.PolicyClaimManagement,
            second.ProductShellViewModel.PolicyClaimManagement);
        Assert.NotSame(
            first.ProductShellViewModel.FamilyMemberManagement,
            second.ProductShellViewModel.FamilyMemberManagement);
        Assert.NotSame(
            first.ProductShellViewModel.CategoryManagement,
            second.ProductShellViewModel.CategoryManagement);
        Assert.NotSame(
            first.ProductShellViewModel.ClaimSubmissionManagement,
            second.ProductShellViewModel.ClaimSubmissionManagement);
        Assert.NotSame(
            first.ProductShellViewModel.ClaimSubmissionManagement.PaymentManagement,
            second.ProductShellViewModel.ClaimSubmissionManagement.PaymentManagement);
        Assert.NotSame(
            first.ProductShellViewModel.ClaimCompleteSummary,
            second.ProductShellViewModel.ClaimCompleteSummary);
        Assert.NotSame(
            first.ProductShellViewModel.ClaimHistory,
            second.ProductShellViewModel.ClaimHistory);
        Assert.NotSame(
            first.ProductShellViewModel.HomeDashboard,
            second.ProductShellViewModel.HomeDashboard);
        Assert.NotSame(
            first.PolicyCoverageStorageService,
            second.PolicyCoverageStorageService);
    }

    [Fact]
    public void Create_resolves_product_shell_fallback_copy_without_application_resources()
    {
        var productShell = CreateServices().ProductShellViewModel;

        Assert.Equal("FamilyClaimRef", productShell.ShellTitle);
        Assert.Collection(
            productShell.NavigationItems,
            item => Assert.Equal("홈", item.DisplayText),
            item => Assert.Equal("보험 계약", item.DisplayText),
            item => Assert.Equal("청구 건", item.DisplayText),
            item => Assert.Equal("문서 등록", item.DisplayText),
            item => Assert.Equal("문서 목록", item.DisplayText));
        Assert.Equal("문서 목록", productShell.DocumentList.Title);
        Assert.Equal("등록된 문서가 없습니다.", productShell.DocumentList.EmptyMessage);
        Assert.Equal("문서 목록을 불러오지 못했습니다.", productShell.DocumentList.LoadFailedMessage);
    }

    [Fact]
    public void Create_does_not_create_project_root_attachment_or_data_files()
    {
        var projectRoot = FindProjectRoot();
        var attachmentsBefore = SnapshotFiles(Path.Combine(projectRoot, "attachments"));
        var dataLocalBefore = SnapshotFiles(Path.Combine(projectRoot, "data", "local"));
        var runtimeRoot = Path.Combine(
            Path.GetTempPath(),
            "FamilyClaimRef.App.Tests",
            "composition",
            Guid.NewGuid().ToString("N"));

        _ = AppServices.Create(new StubRuntimeRootProvider(RuntimeRootPaths.FromRuntimeRoot(runtimeRoot)));

        var attachmentsAfter = SnapshotFiles(Path.Combine(projectRoot, "attachments"));
        var dataLocalAfter = SnapshotFiles(Path.Combine(projectRoot, "data", "local"));
        Assert.Equal(attachmentsBefore, attachmentsAfter);
        Assert.Equal(dataLocalBefore, dataLocalAfter);
        Assert.False(Directory.Exists(runtimeRoot));
    }

    [Fact]
    public void Create_rejects_null_runtime_root_provider()
    {
        var exception = Record.Exception(() => AppServices.Create(null!));

        Assert.NotNull(exception);
        Assert.IsType<ArgumentNullException>(exception);
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

    private static object GetPrivateField(object target, string fieldName)
    {
        return target.GetType().GetField(
                fieldName,
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
            ?.GetValue(target)
            ?? throw new InvalidOperationException($"Field {fieldName} was not found.");
    }

    private static AppServices CreateServices()
    {
        var runtimeRoot = Path.Combine(
            Path.GetTempPath(),
            "FamilyClaimRef.App.Tests",
            "composition",
            Guid.NewGuid().ToString("N"));

        return AppServices.Create(
            new StubRuntimeRootProvider(RuntimeRootPaths.FromRuntimeRoot(runtimeRoot)));
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

    private sealed class StubRuntimeRootProvider(RuntimeRootPaths runtimeRootPaths) : IRuntimeRootProvider
    {
        public RuntimeRootPaths GetRuntimeRootPaths()
        {
            return runtimeRootPaths;
        }
    }
}
