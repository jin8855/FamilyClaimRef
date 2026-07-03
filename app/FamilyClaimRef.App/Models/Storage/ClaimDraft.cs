namespace FamilyClaimRef.App.Models.Storage;

public sealed record class ClaimDraft(
    string PolicyId,
    string DisplayTitle,
    DateOnly ReferenceDate);
