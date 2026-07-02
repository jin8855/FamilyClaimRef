using System.IO;
using Microsoft.Win32;

namespace FamilyClaimRef.App.Services.UI;

public sealed class WpfFilePickerService : IFilePickerService
{
    private const string DocumentFilter =
        "Document files|*.pdf;*.png;*.jpg;*.jpeg;*.webp;*.bmp|All files|*.*";

    public Task<FilePickerResult?> PickDocumentFileAsync(
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var dialog = new OpenFileDialog
        {
            CheckFileExists = true,
            CheckPathExists = true,
            Filter = DocumentFilter,
            Multiselect = false
        };

        var selected = dialog.ShowDialog();
        if (selected != true)
        {
            return Task.FromResult<FilePickerResult?>(null);
        }

        var sourceFilePath = dialog.FileName;
        var result = new FilePickerResult(
            sourceFilePath,
            Path.GetFileName(sourceFilePath));

        return Task.FromResult<FilePickerResult?>(result);
    }
}
