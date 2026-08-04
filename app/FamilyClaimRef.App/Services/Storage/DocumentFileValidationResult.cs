namespace FamilyClaimRef.App.Services.Storage;

public sealed record DocumentFileValidationResult(
    string SafeDisplayName,
    string NormalizedExtension,
    string ValidatedFileType,
    long ByteLength,
    string Sha256,
    DateTimeOffset LastWriteTimeUtc);
