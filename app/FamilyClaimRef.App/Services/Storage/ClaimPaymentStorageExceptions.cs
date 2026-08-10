namespace FamilyClaimRef.App.Services.Storage;

public sealed class ClaimPaymentConcurrencyException : InvalidOperationException
{
    public ClaimPaymentConcurrencyException()
        : base("Claim payment revision does not match the stored revision.")
    {
    }
}

public sealed class ClaimPaymentLegacyReviewRequiredException : InvalidOperationException
{
    public ClaimPaymentLegacyReviewRequiredException()
        : base("Claim payment ownership requires legacy data review.")
    {
    }
}

public sealed class ClaimPaymentReferenceException : InvalidOperationException
{
    public ClaimPaymentReferenceException()
        : base("Claim payment reference is unavailable or inconsistent.")
    {
    }
}

public sealed class ClaimPaymentTransitionException : InvalidOperationException
{
    public ClaimPaymentTransitionException()
        : base("Claim payment status transition is not allowed.")
    {
    }
}
