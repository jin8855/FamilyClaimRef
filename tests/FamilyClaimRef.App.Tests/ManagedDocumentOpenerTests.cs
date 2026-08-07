using System.Diagnostics;
using FamilyClaimRef.App.Services.UI;
using Xunit;

namespace FamilyClaimRef.App.Tests;

public sealed class ManagedDocumentOpenerTests
{
    [Fact]
    public async Task OpenAsync_opens_only_the_resolved_managed_regular_file()
    {
        var rootPath = CreateTempRoot();
        try
        {
            var documentDirectory = Path.Combine(rootPath, "documents");
            Directory.CreateDirectory(documentDirectory);
            var documentPath = Path.Combine(documentDirectory, "synthetic.pdf");
            await File.WriteAllTextAsync(documentPath, "synthetic");
            ProcessStartInfo? observed = null;
            var opener = new ManagedDocumentOpener(rootPath, startInfo => observed = startInfo);

            await opener.OpenAsync("documents/synthetic.pdf");

            Assert.NotNull(observed);
            Assert.Equal(documentPath, observed.FileName);
            Assert.True(observed.UseShellExecute);
        }
        finally
        {
            Directory.Delete(rootPath, recursive: true);
        }
    }

    [Fact]
    public async Task OpenAsync_rejects_rooted_traversal_and_missing_paths_without_launching()
    {
        var rootPath = CreateTempRoot();
        try
        {
            var launchCount = 0;
            var opener = new ManagedDocumentOpener(rootPath, _ => launchCount++);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                opener.OpenAsync(Path.Combine(rootPath, "outside.pdf")));
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                opener.OpenAsync("../outside.pdf"));
            await Assert.ThrowsAsync<FileNotFoundException>(() =>
                opener.OpenAsync("documents/missing.pdf"));
            Assert.Equal(0, launchCount);
        }
        finally
        {
            Directory.Delete(rootPath, recursive: true);
        }
    }

    private static string CreateTempRoot()
    {
        var rootPath = Path.Combine(
            Path.GetTempPath(),
            "FamilyClaimRef.App.Tests",
            nameof(ManagedDocumentOpenerTests),
            Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(rootPath);
        return rootPath;
    }
}
