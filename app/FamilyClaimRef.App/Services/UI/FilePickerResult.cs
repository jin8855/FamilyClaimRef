using FamilyClaimRef.App.Services.Storage;

namespace FamilyClaimRef.App.Services.UI;

public sealed record FilePickerResult(
    string SourceFilePath,
    string SafeDisplayName,
    DocumentFileValidationResult? Validation = null);
