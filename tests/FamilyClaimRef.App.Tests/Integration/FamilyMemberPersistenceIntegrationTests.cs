using FamilyClaimRef.App.Composition;
using FamilyClaimRef.App.Models.Storage;
using FamilyClaimRef.App.Services.Storage;
using Xunit;

namespace FamilyClaimRef.App.Tests.Integration;

[Collection(RuntimeEnvironmentCollectionName.Value)]
public sealed class FamilyMemberPersistenceIntegrationTests
{
    private const string RuntimeOverrideEnabledVariable =
        "FAMILYCLAIMREF_ENABLE_DEV_RUNTIME_ROOT_OVERRIDE";
    private const string RuntimeRootVariable = "FAMILYCLAIMREF_RUNTIME_ROOT";

    [Fact]
    public async Task Product_composition_persists_create_update_deactivate_and_duplicate_display_names()
    {
        var projectRoot = FindProjectRoot();
        var projectAttachmentsBefore = SnapshotFiles(Path.Combine(projectRoot, "attachments"));
        var projectDataLocalBefore = SnapshotFiles(Path.Combine(projectRoot, "data", "local"));
        var projectRuntimeTestFilesBefore = SnapshotFiles(projectRoot, "runtime_test_document.*");
        var testRunRoot = Path.Combine(
            Path.GetTempPath(),
            "FamilyClaimRef-TestRuns",
            $"family-persistence-{Guid.NewGuid():N}");
        var isolatedRuntimeRoot = Path.Combine(testRunRoot, "runtime");

        try
        {
            using var _ = new EnvironmentVariableScope(
                (RuntimeOverrideEnabledVariable, "1"),
                (RuntimeRootVariable, isolatedRuntimeRoot));

            var createdServices = AppServices.CreateDefault();
            var createdViewModel = createdServices.ProductShellViewModel.FamilyMemberManagement;
            createdViewModel.BeginCreate();
            SetInput(createdViewModel, "synthetic family");
            Assert.True(await createdViewModel.SaveAsync());
            var firstId = Assert.IsType<string>(createdViewModel.EditingTargetId);
            Assert.Equal(1, createdViewModel.ExpectedVersion);

            createdViewModel.BeginCreate();
            SetInput(createdViewModel, "synthetic family");
            Assert.True(await createdViewModel.SaveAsync());
            var secondId = Assert.IsType<string>(createdViewModel.EditingTargetId);
            Assert.NotEqual(firstId, secondId);

            var reloadedServices = AppServices.CreateDefault();
            var reloadedViewModel = reloadedServices.ProductShellViewModel.FamilyMemberManagement;
            Assert.True(await reloadedViewModel.LoadAsync());
            Assert.Equal(2, reloadedViewModel.AvailableMembers.Count);
            Assert.All(
                reloadedViewModel.AvailableMembers,
                member => Assert.Equal("synthetic family", member.DisplayName));

            var first = reloadedViewModel.AvailableMembers.Single(member => member.Id == firstId);
            Assert.True(await reloadedViewModel.PrepareEditAsync(first.Id, first.Version));
            reloadedViewModel.DisplayName = "updated synthetic family";
            reloadedViewModel.Memo = "updated synthetic memo";
            Assert.True(await reloadedViewModel.SaveAsync());
            Assert.Equal(2, reloadedViewModel.ExpectedVersion);

            var editedReloadServices = AppServices.CreateDefault();
            var editedReloadViewModel = editedReloadServices.ProductShellViewModel.FamilyMemberManagement;
            Assert.True(await editedReloadViewModel.LoadAsync());
            var edited = editedReloadViewModel.AvailableMembers.Single(member => member.Id == firstId);
            Assert.Equal("updated synthetic family", edited.DisplayName);
            Assert.Equal("updated synthetic memo", edited.Memo);
            Assert.Equal(2, edited.Version);

            Assert.True(await editedReloadViewModel.DeactivateAsync(edited.Id, edited.Version));

            var deactivatedReloadServices = AppServices.CreateDefault();
            var deactivatedReloadViewModel =
                deactivatedReloadServices.ProductShellViewModel.FamilyMemberManagement;
            Assert.True(await deactivatedReloadViewModel.LoadAsync());
            Assert.Equal(2, deactivatedReloadViewModel.AvailableMembers.Count);
            var inactive = deactivatedReloadViewModel.AvailableMembers.Single(
                member => member.Id == firstId);
            Assert.NotNull(inactive.DisabledAt);
            var active = Assert.Single(await new JsonFamilyMemberStorageService(
                deactivatedReloadServices.MetadataRootPath).GetActiveFamilyMembersAsync());
            Assert.Equal(secondId, active.Id);

            var store = new JsonFileStore<FamilyMemberRecord>(
                deactivatedReloadServices.MetadataRootPath,
                JsonFamilyMemberStorageService.StoreFileName,
                JsonFamilyMemberStorageService.StoreSchemaVersion);
            var persisted = (await store.LoadAsync()).Items;
            Assert.Equal(2, persisted.Count);
            var disabled = persisted.Single(member => member.Id == firstId);
            Assert.NotNull(disabled.DisabledAt);
            Assert.Equal(3, disabled.Version);

            Assert.True(await deactivatedReloadViewModel.ReactivateAsync(
                disabled.Id,
                disabled.Version));

            var reactivatedReloadServices = AppServices.CreateDefault();
            var reactivatedReloadViewModel =
                reactivatedReloadServices.ProductShellViewModel.FamilyMemberManagement;
            Assert.True(await reactivatedReloadViewModel.LoadAsync());
            Assert.Equal(2, reactivatedReloadViewModel.AvailableMembers.Count);
            var reactivated = reactivatedReloadViewModel.AvailableMembers.Single(
                member => member.Id == firstId);
            Assert.Null(reactivated.DisabledAt);
            Assert.Equal(4, reactivated.Version);
            Assert.Equal(
                2,
                (await new JsonFamilyMemberStorageService(
                    reactivatedReloadServices.MetadataRootPath)
                    .GetActiveFamilyMembersAsync()).Count);
        }
        finally
        {
            DeleteTestRunRoot(testRunRoot);
        }

        Assert.Equal(
            projectAttachmentsBefore,
            SnapshotFiles(Path.Combine(projectRoot, "attachments")));
        Assert.Equal(
            projectDataLocalBefore,
            SnapshotFiles(Path.Combine(projectRoot, "data", "local")));
        Assert.Equal(
            projectRuntimeTestFilesBefore,
            SnapshotFiles(projectRoot, "runtime_test_document.*"));
    }

    [Fact]
    public async Task Product_composition_and_policy_claim_storage_remain_independent_in_same_root()
    {
        var testRunRoot = Path.Combine(
            Path.GetTempPath(),
            "FamilyClaimRef-TestRuns",
            $"family-policy-regression-{Guid.NewGuid():N}");
        var isolatedRuntimeRoot = Path.Combine(testRunRoot, "runtime");

        try
        {
            using var _ = new EnvironmentVariableScope(
                (RuntimeOverrideEnabledVariable, "1"),
                (RuntimeRootVariable, isolatedRuntimeRoot));

            var services = AppServices.CreateDefault();
            var family = services.ProductShellViewModel.FamilyMemberManagement;
            family.BeginCreate();
            SetInput(family, "synthetic family");
            Assert.True(await family.SaveAsync());

            var policyClaim = services.ProductShellViewModel.PolicyClaimManagement;
            policyClaim.NewPolicyDisplayTitle = "synthetic policy";
            Assert.True(await policyClaim.CreatePolicyAsync());
            policyClaim.SelectedPolicyForClaimId = policyClaim.SelectedPolicyId;
            policyClaim.NewClaimDisplayTitle = "synthetic claim";
            Assert.True(await policyClaim.CreateClaimAsync());

            var files = Directory
                .GetFiles(services.MetadataRootPath, "*.json", SearchOption.TopDirectoryOnly)
                .Select(path => Path.GetFileName(path)!)
                .Order(StringComparer.Ordinal)
                .ToArray();
            Assert.Equal(
                ["claims.json", "family-members.json", "policies.json"],
                files);

            var reloaded = AppServices.CreateDefault();
            Assert.True(await reloaded.ProductShellViewModel.FamilyMemberManagement.LoadAsync());
            Assert.True(await reloaded.ProductShellViewModel.PolicyClaimManagement.LoadAsync());
            Assert.Single(reloaded.ProductShellViewModel.FamilyMemberManagement.AvailableMembers);
            Assert.Single(reloaded.ProductShellViewModel.PolicyClaimManagement.AvailablePolicies);
            Assert.Single(reloaded.ProductShellViewModel.PolicyClaimManagement.AvailableClaims);
        }
        finally
        {
            DeleteTestRunRoot(testRunRoot);
        }
    }

    [Fact]
    public async Task Separate_product_view_models_share_store_gate_and_one_stale_update_conflicts()
    {
        var testRunRoot = Path.Combine(
            Path.GetTempPath(),
            "FamilyClaimRef-TestRuns",
            $"family-viewmodel-concurrency-{Guid.NewGuid():N}");
        var isolatedRuntimeRoot = Path.Combine(testRunRoot, "runtime");

        try
        {
            using var _ = new EnvironmentVariableScope(
                (RuntimeOverrideEnabledVariable, "1"),
                (RuntimeRootVariable, isolatedRuntimeRoot));

            var seed = AppServices.CreateDefault().ProductShellViewModel.FamilyMemberManagement;
            seed.BeginCreate();
            SetInput(seed, "synthetic seed");
            Assert.True(await seed.SaveAsync());

            var first = AppServices.CreateDefault().ProductShellViewModel.FamilyMemberManagement;
            var second = AppServices.CreateDefault().ProductShellViewModel.FamilyMemberManagement;
            Assert.True(await first.LoadAsync());
            Assert.True(await second.LoadAsync());
            var firstTarget = Assert.Single(first.AvailableMembers);
            var secondTarget = Assert.Single(second.AvailableMembers);
            Assert.Equal(firstTarget.Id, secondTarget.Id);
            Assert.Equal(firstTarget.Version, secondTarget.Version);
            Assert.True(await first.PrepareEditAsync(firstTarget.Id, firstTarget.Version));
            Assert.True(await second.PrepareEditAsync(secondTarget.Id, secondTarget.Version));
            first.DisplayName = "synthetic first contender";
            second.DisplayName = "synthetic second contender";

            var results = await Task.WhenAll(first.SaveAsync(), second.SaveAsync());

            Assert.Single(results, result => result);
            Assert.Single(results, result => !result);
            var reloaded = AppServices.CreateDefault().ProductShellViewModel.FamilyMemberManagement;
            Assert.True(await reloaded.LoadAsync());
            var persisted = Assert.Single(reloaded.AvailableMembers);
            Assert.Equal(2, persisted.Version);
            Assert.Contains(
                persisted.DisplayName,
                new[] { "synthetic first contender", "synthetic second contender" });
        }
        finally
        {
            DeleteTestRunRoot(testRunRoot);
        }
    }

    private static void SetInput(
        FamilyClaimRef.App.ViewModels.FamilyMemberManagementViewModel viewModel,
        string displayName)
    {
        viewModel.DisplayName = displayName;
        viewModel.SelectedRelation = FamilyMemberRelationValues.Mother;
        viewModel.Memo = "synthetic memo";
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

    private static void DeleteTestRunRoot(string testRunRoot)
    {
        var allowedParent = Path.Combine(Path.GetTempPath(), "FamilyClaimRef-TestRuns");
        var parent = Path.GetFullPath(allowedParent + Path.DirectorySeparatorChar);
        var target = Path.GetFullPath(testRunRoot);
        if (!target.StartsWith(parent, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Test cleanup path is outside the allowed test temp root.");
        }

        if (Directory.Exists(target))
        {
            Directory.Delete(target, recursive: true);
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
