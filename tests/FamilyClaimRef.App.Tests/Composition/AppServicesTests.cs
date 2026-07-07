using FamilyClaimRef.App.Composition;
using FamilyClaimRef.App.Services.Runtime;
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
