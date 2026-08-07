namespace FamilyClaimRef.App.Services.Storage;

public sealed class ClaimCaseConcurrencyException : InvalidOperationException
{
    public ClaimCaseConcurrencyException()
        : base("Claim case revision does not match the stored revision.")
    {
    }
}

public sealed class ClaimCaseLegacyReviewRequiredException : InvalidOperationException
{
    public ClaimCaseLegacyReviewRequiredException()
        : base("Legacy claim case ownership could not be resolved.")
    {
    }
}
