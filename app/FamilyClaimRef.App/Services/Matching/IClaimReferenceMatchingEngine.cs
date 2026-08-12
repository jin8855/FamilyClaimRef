using FamilyClaimRef.App.Models.Matching;

namespace FamilyClaimRef.App.Services.Matching;

public interface IClaimReferenceMatchingEngine
{
    ClaimReferenceProjection BuildProjection(ClaimReferenceMatchingRequest request);
}
