namespace FamilyClaimRef.App.Services.Matching;

public enum ClaimReferenceMatchingErrorCode
{
    InvalidGraph,
    SelectedClaimUnavailable
}

public sealed class ClaimReferenceMatchingException : InvalidOperationException
{
    public ClaimReferenceMatchingException(
        ClaimReferenceMatchingErrorCode errorCode,
        string message)
        : base(message)
    {
        ErrorCode = errorCode;
    }

    public ClaimReferenceMatchingErrorCode ErrorCode { get; }
}
