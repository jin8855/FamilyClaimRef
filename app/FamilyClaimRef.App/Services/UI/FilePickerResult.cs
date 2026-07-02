namespace FamilyClaimRef.App.Services.UI;

public sealed record FilePickerResult(
    string SourceFilePath,
    string SafeDisplayName);
