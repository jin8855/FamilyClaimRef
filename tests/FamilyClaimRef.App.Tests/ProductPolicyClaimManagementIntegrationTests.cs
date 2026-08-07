using FamilyClaimRef.App.Composition;
using FamilyClaimRef.App.Services.Runtime;
using Xunit;

namespace FamilyClaimRef.App.Tests;

public sealed class ProductPolicyClaimManagementIntegrationTests
{
    [Fact]
    public void Product_management_views_bind_to_the_same_shell_management_child()
    {
        var xamlPath = Path.Combine(
            FindProjectRoot(),
            "app",
            "FamilyClaimRef.App",
            "ProductShell",
            "ProductShellWindow.xaml");
        var xaml = File.ReadAllText(xamlPath);

        Assert.Contains("<views:ProductPolicyContractsView", xaml, StringComparison.Ordinal);
        Assert.Contains("<views:ProductClaimCasesView", xaml, StringComparison.Ordinal);
        Assert.Contains("<views:ProductInsurancePolicyEditorView", xaml, StringComparison.Ordinal);
        Assert.Equal(
            3,
            CountOccurrences(xaml, "DataContext.PolicyClaimManagement"));
    }

    [Fact]
    public async Task Product_management_changes_refresh_registration_targets_on_entry_load()
    {
        var runtimeRoot = Path.Combine(
            Path.GetTempPath(),
            "FamilyClaimRef.App.Tests",
            nameof(ProductPolicyClaimManagementIntegrationTests),
            Guid.NewGuid().ToString("N"));

        try
        {
            var services = AppServices.Create(
                new StubRuntimeRootProvider(RuntimeRootPaths.FromRuntimeRoot(runtimeRoot)));
            var management = services.ProductShellViewModel.PolicyClaimManagement;
            var registration = services.ProductShellViewModel.DocumentRegistration;

            Assert.NotSame(
                services.MainWindowViewModel.PolicyClaimManagement,
                management);

            management.NewPolicyDisplayTitle = "synthetic_policy_title";
            Assert.True(await management.CreatePolicyAsync());
            var policy = Assert.Single(management.AvailablePolicies);

            management.SelectedPolicyForClaimId = policy.Id;
            management.NewClaimDisplayTitle = "synthetic_claim_title";
            Assert.True(await management.CreateClaimAsync());
            var claim = Assert.Single(management.AvailableClaims);

            await registration.LoadTargetOptionsAsync();

            Assert.Equal(policy.Id, Assert.Single(registration.AvailablePolicies).Id);
            Assert.Equal(claim.Id, Assert.Single(registration.AvailableClaims).Id);

            registration.SelectedPolicyId = policy.Id;
            registration.SelectedClaimId = claim.Id;
            management.SelectedClaimId = claim.Id;
            Assert.True(await management.DisableSelectedClaimAsync());
            management.SelectedPolicyId = policy.Id;
            Assert.True(await management.DisableSelectedPolicyAsync());

            await registration.LoadTargetOptionsAsync();

            Assert.Empty(registration.AvailablePolicies);
            Assert.Empty(registration.AvailableClaims);
            Assert.Null(registration.SelectedPolicyId);
            Assert.Null(registration.SelectedClaimId);
        }
        finally
        {
            if (Directory.Exists(runtimeRoot))
            {
                Directory.Delete(runtimeRoot, recursive: true);
            }
        }
    }

    private sealed class StubRuntimeRootProvider(RuntimeRootPaths runtimeRootPaths)
        : IRuntimeRootProvider
    {
        public RuntimeRootPaths GetRuntimeRootPaths()
        {
            return runtimeRootPaths;
        }
    }

    private static int CountOccurrences(string value, string fragment)
    {
        var count = 0;
        var startIndex = 0;
        while ((startIndex = value.IndexOf(fragment, startIndex, StringComparison.Ordinal)) >= 0)
        {
            count++;
            startIndex += fragment.Length;
        }

        return count;
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
}
