namespace FamilyClaimRef.App.Services.Storage;

public enum FamilyMemberStorageErrorCode
{
    TargetUnavailable,
    VersionConflict
}

public sealed class FamilyMemberStorageException(
    FamilyMemberStorageErrorCode errorCode,
    string message) : InvalidOperationException(message)
{
    public FamilyMemberStorageErrorCode ErrorCode { get; } = errorCode;
}
