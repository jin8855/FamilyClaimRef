using FamilyClaimRef.App.Composition;
using FamilyClaimRef.App.Services.Runtime;
using FamilyClaimRef.App.Services.Storage;
using FamilyClaimRef.App.ViewModels;
using Xunit;

namespace FamilyClaimRef.App.Tests.Integration;

public sealed class IsolatedRuntimeDocumentWorkflowTests
{
    private const string RuntimeOverrideEnabledVariable = "FAMILYCLAIMREF_ENABLE_DEV_RUNTIME_ROOT_OVERRIDE";
    private const string RuntimeRootVariable = "FAMILYCLAIMREF_RUNTIME_ROOT";

    [Fact]
    public async Task AppServices_WithRuntimeRootOverride_RegistersPolicyAndClaimDocumentsInIsolatedRoot()
    {
        var projectRoot = FindProjectRoot();
        var projectAttachmentsBefore = SnapshotFiles(Path.Combine(projectRoot, "attachments"));
        var projectDataLocalBefore = SnapshotFiles(Path.Combine(projectRoot, "data", "local"));
        var projectRuntimeTestFilesBefore = SnapshotFiles(projectRoot, "runtime_test_document.*");
        var testRunRoot = Path.Combine(
            Path.GetTempPath(),
            "FamilyClaimRef-TestRuns",
            $"isolated-workflow-{Guid.NewGuid():N}");
        var syntheticInputRoot = Path.Combine(testRunRoot, "input");
        var isolatedRuntimeRoot = Path.Combine(testRunRoot, "runtime");

        Directory.CreateDirectory(syntheticInputRoot);

        try
        {
            var policySourcePath = await CreateSyntheticInputFileAsync(
                syntheticInputRoot,
                "synthetic-policy-source.png",
                "FamilyClaimRef automated validation synthetic policy document.");
            var claimSourcePath = await CreateSyntheticInputFileAsync(
                syntheticInputRoot,
                "synthetic-claim-source.png",
                "FamilyClaimRef automated validation synthetic claim document.");

            using var _ = new EnvironmentVariableScope(
                (RuntimeOverrideEnabledVariable, "1"),
                (RuntimeRootVariable, isolatedRuntimeRoot));

            var services = AppServices.CreateDefault();

            Assert.Equal(Path.GetFullPath(isolatedRuntimeRoot), services.RuntimeRootPath);
            Assert.True(IsUnderDirectory(services.RuntimeRootPath, services.MetadataRootPath));
            Assert.True(IsUnderDirectory(services.RuntimeRootPath, services.AttachmentRootPath));

            var mainWindow = services.MainWindowViewModel;
            await mainWindow.LoadAsync();

            mainWindow.PolicyClaimManagement.NewPolicyDisplayTitle = "policy_title_automated_demo";
            await mainWindow.CreatePolicyAsync();

            var policyId = mainWindow.PolicyClaimManagement.SelectedPolicyId;
            Assert.NotNull(policyId);
            mainWindow.PolicyClaimManagement.SelectedPolicyForClaimId = policyId;
            mainWindow.PolicyClaimManagement.NewClaimDisplayTitle = "claim_title_automated_demo";
            await mainWindow.CreateClaimAsync();

            var claimId = mainWindow.PolicyClaimManagement.SelectedClaimId;
            Assert.NotNull(claimId);

            await RegisterPolicyDocumentAsync(mainWindow.DocumentRegistration, policyId, policySourcePath);
            await RegisterClaimDocumentAsync(mainWindow.DocumentRegistration, claimId, claimSourcePath);

            AssertMetadataFilesExist(services.MetadataRootPath);

            var policyClaimStorage = new JsonPolicyClaimStorageService(services.MetadataRootPath);
            var documentStorage = new JsonDocumentStorageService(services.MetadataRootPath);

            var policies = await policyClaimStorage.GetPoliciesAsync();
            var claims = await policyClaimStorage.GetClaimsAsync();
            var documents = await documentStorage.GetDocumentsAsync();
            var policyDocuments = await documentStorage.GetPolicyDocumentsAsync(policyId);
            var claimDocuments = await documentStorage.GetClaimDocumentsAsync(claimId);

            var policy = Assert.Single(policies);
            var claim = Assert.Single(claims);
            Assert.Equal(policyId, policy.Id);
            Assert.Equal(claimId, claim.Id);
            Assert.Equal(policyId, claim.PolicyId);
            Assert.Equal(2, documents.Count);
            Assert.Equal("terms", Assert.Single(policyDocuments).DocumentType);
            Assert.Equal("receipt", Assert.Single(claimDocuments).DocumentType);

            var attachmentFiles = Directory.GetFiles(
                Path.Combine(services.AttachmentRootPath, "documents"),
                "*",
                SearchOption.AllDirectories);
            Assert.Equal(2, attachmentFiles.Length);

            Assert.All(documents, document =>
            {
                Assert.False(Path.IsPathRooted(document.RelativePath));
                var attachmentPath = Path.GetFullPath(Path.Combine(
                    services.AttachmentRootPath,
                    document.RelativePath.Replace('/', Path.DirectorySeparatorChar)));

                Assert.True(IsUnderDirectory(services.AttachmentRootPath, attachmentPath));
                Assert.True(File.Exists(attachmentPath));
            });
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

    private static async Task RegisterPolicyDocumentAsync(
        DocumentRegistrationViewModel viewModel,
        string policyId,
        string sourcePath)
    {
        viewModel.TargetKind = DocumentRegistrationViewModel.PolicyTargetKind;
        viewModel.SelectedPolicyId = policyId;
        viewModel.SelectedSourceFilePath = sourcePath;
        viewModel.SelectedSourceFileDisplayName = Path.GetFileName(sourcePath);
        viewModel.DocumentType = "terms";
        viewModel.DisplayTitle = "synthetic_policy_document_png_demo";
        viewModel.ReferenceDate = new DateOnly(2026, 7, 7);

        await viewModel.RegisterAsync();

        Assert.Null(viewModel.ValidationMessage);
        Assert.StartsWith($"policy:{policyId}; document:", viewModel.LastRegistrationSummary);
    }

    private static async Task RegisterClaimDocumentAsync(
        DocumentRegistrationViewModel viewModel,
        string claimId,
        string sourcePath)
    {
        viewModel.TargetKind = DocumentRegistrationViewModel.ClaimTargetKind;
        viewModel.SelectedClaimId = claimId;
        viewModel.SelectedSourceFilePath = sourcePath;
        viewModel.SelectedSourceFileDisplayName = Path.GetFileName(sourcePath);
        viewModel.DocumentType = "receipt";
        viewModel.DisplayTitle = "synthetic_claim_document_png_demo";
        viewModel.ReferenceDate = new DateOnly(2026, 7, 7);

        await viewModel.RegisterAsync();

        Assert.Null(viewModel.ValidationMessage);
        Assert.StartsWith($"claim:{claimId}; document:", viewModel.LastRegistrationSummary);
    }

    private static void AssertMetadataFilesExist(string metadataRootPath)
    {
        Assert.True(File.Exists(Path.Combine(metadataRootPath, "policies.json")));
        Assert.True(File.Exists(Path.Combine(metadataRootPath, "claims.json")));
        Assert.True(File.Exists(Path.Combine(metadataRootPath, "documents.json")));
        Assert.True(File.Exists(Path.Combine(metadataRootPath, "policy-documents.json")));
        Assert.True(File.Exists(Path.Combine(metadataRootPath, "claim-documents.json")));
    }

    private static async Task<string> CreateSyntheticInputFileAsync(
        string inputRoot,
        string fileName,
        string content)
    {
        Directory.CreateDirectory(inputRoot);
        var sourcePath = Path.Combine(inputRoot, fileName);
        await File.WriteAllTextAsync(sourcePath, content);

        return sourcePath;
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
