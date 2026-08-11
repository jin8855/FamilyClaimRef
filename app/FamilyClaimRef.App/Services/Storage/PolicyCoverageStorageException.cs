namespace FamilyClaimRef.App.Services.Storage;

public enum PolicyCoverageStorageErrorCode
{
    TargetUnavailable,
    VersionConflict,
    InvalidTransition,
    ReferenceInvalid,
    IntegrityViolation
}

public sealed class PolicyCoverageStorageException : InvalidOperationException
{
    public PolicyCoverageStorageException(
        PolicyCoverageStorageErrorCode errorCode,
        string message)
        : base(message)
    {
        ErrorCode = errorCode;
    }

    public PolicyCoverageStorageErrorCode ErrorCode { get; }
}
