namespace FamilyClaimRef.App.Services.UI;

public interface IFilePickerService
{
    Task<FilePickerResult?> PickDocumentFileAsync(
        CancellationToken cancellationToken = default);
}
