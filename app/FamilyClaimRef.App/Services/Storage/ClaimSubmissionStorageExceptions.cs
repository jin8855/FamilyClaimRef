namespace FamilyClaimRef.App.Services.Storage;

public sealed class ClaimSubmissionConcurrencyException : InvalidOperationException
{
    public ClaimSubmissionConcurrencyException()
        : base("Claim submission revision does not match the stored revision.")
    {
    }
}

public sealed class ClaimSubmissionLegacyReviewRequiredException : InvalidOperationException
{
    public ClaimSubmissionLegacyReviewRequiredException()
        : base("Claim submission ownership requires legacy data review.")
    {
    }
}

public sealed class ClaimSubmissionReferenceException : InvalidOperationException
{
    public ClaimSubmissionReferenceException()
        : base("Claim submission reference is unavailable or inconsistent.")
    {
    }
}

public sealed class ClaimSubmissionTransitionException : InvalidOperationException
{
    public ClaimSubmissionTransitionException()
        : base("Claim submission status transition is not allowed.")
    {
    }
}
