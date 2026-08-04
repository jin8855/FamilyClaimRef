using System.IO;
using FamilyClaimRef.App.Services.Storage;
using Microsoft.Win32;

namespace FamilyClaimRef.App.Services.UI;

public sealed class WpfFilePickerService : IFilePickerService
{
    private const string DocumentFilter =
        "Document files|*.pdf;*.png;*.jpg;*.jpeg";

    private readonly DocumentFileValidationService fileValidationService;

    public WpfFilePickerService()
        : this(new DocumentFileValidationService())
    {
    }

    public WpfFilePickerService(DocumentFileValidationService fileValidationService)
    {
        this.fileValidationService = fileValidationService
            ?? throw new ArgumentNullException(nameof(fileValidationService));
    }

    public async Task<FilePickerResult?> PickDocumentFileAsync(
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
            return null;
        }

        var sourceFilePath = dialog.FileName;
        var validation = await fileValidationService.ValidateSourceAsync(
            sourceFilePath,
            cancellationToken);
        var result = new FilePickerResult(
            sourceFilePath,
            validation.SafeDisplayName,
            validation);

        return result;
    }
}
