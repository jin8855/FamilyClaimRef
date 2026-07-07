using FamilyClaimRef.App.Services.Runtime;
using Xunit;

namespace FamilyClaimRef.App.Tests;

public sealed class EnvironmentRuntimeRootProviderTests
{
    [Fact]
    public void GetRuntimeRootPaths_without_env_vars_uses_default_local_application_data_root()
    {
        var localApplicationDataPath = CreateSyntheticRootPath("local-app-data");
        var provider = CreateProvider(localApplicationDataPath, null, null);

        var paths = provider.GetRuntimeRootPaths();

        var expectedRuntimeRoot = Path.GetFullPath(Path.Combine(localApplicationDataPath, "FamilyClaimRef"));
        Assert.Equal(expectedRuntimeRoot, paths.RuntimeRootPath);
    }

    [Fact]
    public void GetRuntimeRootPaths_ignores_runtime_root_env_var_when_guard_is_absent()
    {
        var localApplicationDataPath = CreateSyntheticRootPath("local-app-data");
        var overrideRoot = CreateSyntheticRootPath("override-root");
        var provider = CreateProvider(localApplicationDataPath, null, overrideRoot);

        var paths = provider.GetRuntimeRootPaths();

        var expectedRuntimeRoot = Path.GetFullPath(Path.Combine(localApplicationDataPath, "FamilyClaimRef"));
        Assert.Equal(expectedRuntimeRoot, paths.RuntimeRootPath);
    }

    [Fact]
    public void GetRuntimeRootPaths_ignores_runtime_root_env_var_when_guard_is_not_one()
    {
        var localApplicationDataPath = CreateSyntheticRootPath("local-app-data");
        var overrideRoot = CreateSyntheticRootPath("override-root");
        var provider = CreateProvider(localApplicationDataPath, "true", overrideRoot);

        var paths = provider.GetRuntimeRootPaths();

        var expectedRuntimeRoot = Path.GetFullPath(Path.Combine(localApplicationDataPath, "FamilyClaimRef"));
        Assert.Equal(expectedRuntimeRoot, paths.RuntimeRootPath);
    }

    [Fact]
    public void GetRuntimeRootPaths_uses_absolute_override_root_when_guard_is_one()
    {
        var localApplicationDataPath = CreateSyntheticRootPath("local-app-data");
        var overrideRoot = CreateSyntheticRootPath("override-root");
        var provider = CreateProvider(localApplicationDataPath, "1", overrideRoot);

        var paths = provider.GetRuntimeRootPaths();

        Assert.Equal(Path.GetFullPath(overrideRoot), paths.RuntimeRootPath);
    }

    [Fact]
    public void GetRuntimeRootPaths_rejects_empty_override_root_when_guard_is_one()
    {
        var provider = CreateProvider(CreateSyntheticRootPath("local-app-data"), "1", " ");

        var exception = Record.Exception(() => provider.GetRuntimeRootPaths());

        Assert.NotNull(exception);
        Assert.IsType<InvalidOperationException>(exception);
    }

    [Fact]
    public void GetRuntimeRootPaths_rejects_relative_override_root_when_guard_is_one()
    {
        var provider = CreateProvider(CreateSyntheticRootPath("local-app-data"), "1", "relative-root");

        var exception = Record.Exception(() => provider.GetRuntimeRootPaths());

        Assert.NotNull(exception);
        Assert.IsType<InvalidOperationException>(exception);
    }

    [Fact]
    public void GetRuntimeRootPaths_composes_metadata_and_attachment_roots_under_selected_root()
    {
        var overrideRoot = CreateSyntheticRootPath("override-root");
        var provider = CreateProvider(CreateSyntheticRootPath("local-app-data"), "1", overrideRoot);

        var paths = provider.GetRuntimeRootPaths();

        Assert.Equal(Path.Combine(paths.RuntimeRootPath, "data", "local"), paths.MetadataRootPath);
        Assert.Equal(Path.Combine(paths.RuntimeRootPath, "attachments"), paths.AttachmentRootPath);
    }

    [Fact]
    public void GetRuntimeRootPaths_rejects_missing_local_application_data_path()
    {
        var provider = CreateProvider(" ", null, null);

        var exception = Record.Exception(() => provider.GetRuntimeRootPaths());

        Assert.NotNull(exception);
        Assert.IsType<InvalidOperationException>(exception);
    }

    private static EnvironmentRuntimeRootProvider CreateProvider(
        string localApplicationDataPath,
        string? guardValue,
        string? runtimeRootValue)
    {
        return new EnvironmentRuntimeRootProvider(
            () => localApplicationDataPath,
            () => guardValue,
            () => runtimeRootValue);
    }

    private static string CreateSyntheticRootPath(string name)
    {
        return Path.Combine(Path.GetTempPath(), "FamilyClaimRef.App.Tests", name, Guid.NewGuid().ToString("N"));
    }
}
