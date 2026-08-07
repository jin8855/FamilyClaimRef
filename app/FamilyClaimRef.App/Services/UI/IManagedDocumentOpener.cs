namespace FamilyClaimRef.App.Services.UI;

public interface IManagedDocumentOpener
{
    Task OpenAsync(string relativePath, CancellationToken cancellationToken = default);
}
