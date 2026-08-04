namespace FamilyClaimRef.App.Services.Storage;

public enum DocumentRegistrationErrorCode
{
    RegistrationFailed,
    UnsupportedFileType,
    EmptyFile,
    FileTooLarge,
    SourceUnavailable,
    SourceChanged,
    DuplicateDocument,
    TargetUnavailable,
    CleanupFailed
}

public sealed class DocumentRegistrationException : Exception
{
    public DocumentRegistrationException(
        DocumentRegistrationErrorCode errorCode,
        string message,
        Exception? innerException = null)
        : base(message, innerException)
    {
        ErrorCode = errorCode;
    }

    public DocumentRegistrationErrorCode ErrorCode { get; }
}
