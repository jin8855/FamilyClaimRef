namespace FamilyClaimRef.App.Models.Storage;

public sealed record class PolicyDraft(
    string DisplayTitle,
    DateOnly ReferenceDate);
