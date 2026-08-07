using System.Diagnostics;
using System.IO;

namespace FamilyClaimRef.App.Services.UI;

public sealed class ManagedDocumentOpener : IManagedDocumentOpener
{
    private readonly string attachmentRootPath;
    private readonly Action<ProcessStartInfo> startProcess;

    public ManagedDocumentOpener(string attachmentRootPath)
        : this(
            attachmentRootPath,
            startInfo =>
            {
                _ = Process.Start(startInfo)
                    ?? throw new InvalidOperationException("The managed document could not be opened.");
            })
    {
    }

    internal ManagedDocumentOpener(
        string attachmentRootPath,
        Action<ProcessStartInfo> startProcess)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(attachmentRootPath);
        this.attachmentRootPath = Path.GetFullPath(attachmentRootPath);
        this.startProcess = startProcess ?? throw new ArgumentNullException(nameof(startProcess));
    }

    public Task OpenAsync(
        string relativePath,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentException.ThrowIfNullOrWhiteSpace(relativePath);

        if (Path.IsPathRooted(relativePath))
        {
            throw new InvalidOperationException("Managed document paths must be relative.");
        }

        var normalizedRoot = Path.TrimEndingDirectorySeparator(attachmentRootPath)
            + Path.DirectorySeparatorChar;
        var fullPath = Path.GetFullPath(Path.Combine(
            attachmentRootPath,
            relativePath.Replace('/', Path.DirectorySeparatorChar)));
        if (!fullPath.StartsWith(normalizedRoot, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Managed document path escapes the attachment root.");
        }

        EnsureRegularPath(normalizedRoot, fullPath);
        startProcess(new ProcessStartInfo
        {
            FileName = fullPath,
            UseShellExecute = true
        });
        return Task.CompletedTask;
    }

    private static void EnsureRegularPath(string normalizedRoot, string fullPath)
    {
        if (!File.Exists(fullPath))
        {
            throw new FileNotFoundException("The managed document does not exist.", fullPath);
        }

        var rootPath = Path.TrimEndingDirectorySeparator(normalizedRoot);
        var currentPath = Path.GetDirectoryName(fullPath);
        while (currentPath is not null
            && currentPath.StartsWith(normalizedRoot, StringComparison.OrdinalIgnoreCase))
        {
            EnsureNotReparsePoint(currentPath);
            if (string.Equals(currentPath, rootPath, StringComparison.OrdinalIgnoreCase))
            {
                break;
            }

            currentPath = Path.GetDirectoryName(currentPath);
        }

        EnsureNotReparsePoint(rootPath);
        EnsureNotReparsePoint(fullPath);
    }

    private static void EnsureNotReparsePoint(string path)
    {
        if ((File.GetAttributes(path) & FileAttributes.ReparsePoint) != 0)
        {
            throw new InvalidOperationException("Managed document paths cannot contain reparse points.");
        }
    }
}
