using FamilyClaimRef.App.Composition;
using FamilyClaimRef.App.Models.Storage;
using FamilyClaimRef.App.Services.Storage;
using Xunit;

namespace FamilyClaimRef.App.Tests.Integration;

[Collection(RuntimeEnvironmentCollectionName.Value)]
public sealed class PolicyClaimLifecyclePersistenceTests
{
    private const string RuntimeOverrideEnabledVariable = "FAMILYCLAIMREF_ENABLE_DEV_RUNTIME_ROOT_OVERRIDE";
    private const string RuntimeRootVariable = "FAMILYCLAIMREF_RUNTIME_ROOT";
    private const string PolicyTitle = "policy_title_lifecycle_persistence_demo";
    private const string ClaimTitle = "claim_title_lifecycle_persistence_demo";

    [Fact]
    public async Task AppServices_WithRuntimeRootOverride_PersistsPolicyClaimLifecycleAcrossReloads()
    {
        var projectRoot = FindProjectRoot();
        var projectAttachmentsBefore = SnapshotFiles(Path.Combine(projectRoot, "attachments"));
        var projectDataLocalBefore = SnapshotFiles(Path.Combine(projectRoot, "data", "local"));
        var projectRuntimeTestFilesBefore = SnapshotFiles(projectRoot, "runtime_test_document.*");
        var testRunRoot = Path.Combine(
            Path.GetTempPath(),
            "FamilyClaimRef-TestRuns",
            $"lifecycle-persistence-{Guid.NewGuid():N}");
        var isolatedRuntimeRoot = Path.Combine(testRunRoot, "runtime");

        try
        {
            using var _ = new EnvironmentVariableScope(
                (RuntimeOverrideEnabledVariable, "1"),
                (RuntimeRootVariable, isolatedRuntimeRoot));

            var createdServices = AppServices.CreateDefault();
            Assert.Equal(Path.GetFullPath(isolatedRuntimeRoot), createdServices.RuntimeRootPath);

            var createdMainWindow = createdServices.MainWindowViewModel;
            await createdMainWindow.LoadAsync();

            createdMainWindow.PolicyClaimManagement.NewPolicyDisplayTitle = PolicyTitle;
            await createdMainWindow.CreatePolicyAsync();

            var policyId = createdMainWindow.PolicyClaimManagement.SelectedPolicyId;
            Assert.NotNull(policyId);

            createdMainWindow.PolicyClaimManagement.SelectedPolicyForClaimId = policyId;
            createdMainWindow.PolicyClaimManagement.NewClaimDisplayTitle = ClaimTitle;
            await createdMainWindow.CreateClaimAsync();

            var claimId = createdMainWindow.PolicyClaimManagement.SelectedClaimId;
            Assert.NotNull(claimId);

            AssertActiveTargets(createdMainWindow, policyId, claimId);

            var reloadedServices = AppServices.CreateDefault();
            Assert.Equal(createdServices.MetadataRootPath, reloadedServices.MetadataRootPath);
            var reloadedMainWindow = reloadedServices.MainWindowViewModel;
            await reloadedMainWindow.LoadAsync();

            AssertActiveTargets(reloadedMainWindow, policyId, claimId);

            reloadedMainWindow.PolicyClaimManagement.SelectedClaimId = claimId;
            await reloadedMainWindow.DisableSelectedClaimAsync();

            Assert.Empty(reloadedMainWindow.PolicyClaimManagement.AvailableClaims);

            reloadedMainWindow.PolicyClaimManagement.SelectedPolicyId = policyId;
            await reloadedMainWindow.DisableSelectedPolicyAsync();

            Assert.Empty(reloadedMainWindow.PolicyClaimManagement.AvailablePolicies);

            var disabledReloadServices = AppServices.CreateDefault();
            var disabledReloadMainWindow = disabledReloadServices.MainWindowViewModel;
            await disabledReloadMainWindow.LoadAsync();

            Assert.Empty(disabledReloadMainWindow.PolicyClaimManagement.AvailablePolicies);
            Assert.Empty(disabledReloadMainWindow.PolicyClaimManagement.AvailableClaims);

            var policyStore = new JsonFileStore<PolicyRecord>(
                disabledReloadServices.MetadataRootPath,
                "policies.json");
            var claimStore = new JsonFileStore<ClaimRecord>(
                disabledReloadServices.MetadataRootPath,
                "claims.json");

            var persistedPolicy = Assert.Single((await policyStore.LoadAsync()).Items);
            var persistedClaim = Assert.Single((await claimStore.LoadAsync()).Items);

            Assert.Equal(policyId, persistedPolicy.Id);
            Assert.Equal(PolicyTitle, persistedPolicy.DisplayTitle);
            Assert.NotNull(persistedPolicy.DisabledAt);
            Assert.Equal(claimId, persistedClaim.Id);
            Assert.Equal(policyId, persistedClaim.PolicyId);
            Assert.Equal(ClaimTitle, persistedClaim.DisplayTitle);
            Assert.NotNull(persistedClaim.DisabledAt);

            var activeStorage = new JsonPolicyClaimStorageService(disabledReloadServices.MetadataRootPath);
            Assert.Empty(await activeStorage.GetPoliciesAsync());
            Assert.Empty(await activeStorage.GetClaimsAsync());
            Assert.Empty(await activeStorage.GetClaimsByPolicyIdAsync(policyId));
            Assert.Null(await activeStorage.GetPolicyAsync(policyId));
            Assert.Null(await activeStorage.GetClaimAsync(claimId));
            Assert.False(await activeStorage.PolicyExistsAsync(policyId));
            Assert.False(await activeStorage.ClaimExistsAsync(claimId));
        }
        finally
        {
            DeleteTestRunRoot(testRunRoot);
        }

        var projectAttachmentsAfter = SnapshotFiles(Path.Combine(projectRoot, "attachments"));
        var projectDataLocalAfter = SnapshotFiles(Path.Combine(projectRoot, "data", "local"));
        var projectRuntimeTestFilesAfter = SnapshotFiles(projectRoot, "runtime_test_document.*");

        Assert.Equal(projectAttachmentsBefore, projectAttachmentsAfter);
        Assert.Equal(projectDataLocalBefore, projectDataLocalAfter);
        Assert.Equal(projectRuntimeTestFilesBefore, projectRuntimeTestFilesAfter);
    }

    private static void AssertActiveTargets(
        FamilyClaimRef.App.ViewModels.MainWindowViewModel mainWindow,
        string policyId,
        string claimId)
    {
        var activePolicy = Assert.Single(mainWindow.PolicyClaimManagement.AvailablePolicies);
        var activeClaim = Assert.Single(mainWindow.PolicyClaimManagement.AvailableClaims);

        Assert.Equal(policyId, activePolicy.Id);
        Assert.Equal(PolicyTitle, activePolicy.DisplayTitle);
        Assert.Null(activePolicy.DisabledAt);
        Assert.Equal(claimId, activeClaim.Id);
        Assert.Equal(policyId, activeClaim.PolicyId);
        Assert.Equal(ClaimTitle, activeClaim.DisplayTitle);
        Assert.Null(activeClaim.DisabledAt);
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

    private static string[] SnapshotFiles(string directoryPath, string searchPattern = "*")
    {
        if (!Directory.Exists(directoryPath))
        {
            return [];
        }

        return Directory
            .GetFiles(directoryPath, searchPattern, SearchOption.AllDirectories)
            .Select(path => Path.GetRelativePath(directoryPath, path))
            .Order(StringComparer.Ordinal)
            .ToArray();
    }

    private static bool IsUnderDirectory(string parentPath, string childPath)
    {
        var parentFullPath = EnsureTrailingSeparator(Path.GetFullPath(parentPath));
        var childFullPath = Path.GetFullPath(childPath);

        return childFullPath.StartsWith(parentFullPath, StringComparison.OrdinalIgnoreCase);
    }

    private static string EnsureTrailingSeparator(string path)
    {
        if (path.EndsWith(Path.DirectorySeparatorChar)
            || path.EndsWith(Path.AltDirectorySeparatorChar))
        {
            return path;
        }

        return path + Path.DirectorySeparatorChar;
    }

    private static void DeleteTestRunRoot(string testRunRoot)
    {
        var allowedParent = Path.Combine(Path.GetTempPath(), "FamilyClaimRef-TestRuns");
        if (!IsUnderDirectory(allowedParent, testRunRoot))
        {
            throw new InvalidOperationException("Test cleanup path is outside the allowed test temp root.");
        }

        if (Directory.Exists(testRunRoot))
        {
            Directory.Delete(testRunRoot, recursive: true);
        }
    }

    private sealed class EnvironmentVariableScope : IDisposable
    {
        private readonly (string Name, string? PreviousValue)[] previousValues;

        public EnvironmentVariableScope(params (string Name, string Value)[] values)
        {
            previousValues = values
                .Select(value => (value.Name, Environment.GetEnvironmentVariable(value.Name)))
                .ToArray();

            foreach (var (name, value) in values)
            {
                Environment.SetEnvironmentVariable(name, value);
            }
        }

        public void Dispose()
        {
            foreach (var (name, previousValue) in previousValues)
            {
                Environment.SetEnvironmentVariable(name, previousValue);
            }
        }
    }
}
