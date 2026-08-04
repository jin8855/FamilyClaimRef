using FamilyClaimRef.App.Models.Storage;

namespace FamilyClaimRef.App.Services.Storage;

public interface IFamilyMemberStorageService
{
    Task<IReadOnlyList<FamilyMemberRecord>> GetFamilyMembersAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<FamilyMemberRecord>> GetActiveFamilyMembersAsync(
        CancellationToken cancellationToken = default);

    Task<FamilyMemberRecord?> GetFamilyMemberAsync(
        string id,
        CancellationToken cancellationToken = default);

    Task<FamilyMemberRecord> CreateFamilyMemberAsync(
        FamilyMemberDraft draft,
        CancellationToken cancellationToken = default);

    Task<FamilyMemberRecord> UpdateFamilyMemberAsync(
        string id,
        int expectedVersion,
        FamilyMemberDraft draft,
        CancellationToken cancellationToken = default);

    Task<FamilyMemberRecord> DeactivateFamilyMemberAsync(
        string id,
        int expectedVersion,
        CancellationToken cancellationToken = default);

    Task<FamilyMemberRecord> ReactivateFamilyMemberAsync(
        string id,
        int expectedVersion,
        CancellationToken cancellationToken = default);
}
