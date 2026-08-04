namespace FamilyClaimRef.App.Models.Storage;

public sealed record FamilyMemberDraft(
    string DisplayName,
    string Relation,
    string? Memo);
