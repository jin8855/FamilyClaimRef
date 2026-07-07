using System.IO;

namespace FamilyClaimRef.App.Services.Runtime;

public sealed record RuntimeRootPaths(
    string RuntimeRootPath,
    string MetadataRootPath,
    string AttachmentRootPath)
{
    public static RuntimeRootPaths FromRuntimeRoot(string runtimeRootPath)
    {
        if (string.IsNullOrWhiteSpace(runtimeRootPath))
        {
            throw new ArgumentException("Runtime root path is required.", nameof(runtimeRootPath));
        }

        var normalizedRuntimeRootPath = Path.GetFullPath(runtimeRootPath.Trim());

        return new RuntimeRootPaths(
            normalizedRuntimeRootPath,
            Path.Combine(normalizedRuntimeRootPath, "data", "local"),
            Path.Combine(normalizedRuntimeRootPath, "attachments"));
    }
}
