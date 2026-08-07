using FamilyClaimRef.App.Models.Storage;

namespace FamilyClaimRef.App.Services.Storage;

public interface ICategoryAggregateStorageService
{
    Task<CategoryAggregateSnapshot> LoadAsync(
        CancellationToken cancellationToken = default);

    Task<CategoryMutationResult<CategoryRecord>> CreateCategoryAsync(
        long expectedAggregateVersion,
        CategoryDraft draft,
        CancellationToken cancellationToken = default);

    Task<CategoryMutationResult<CategoryRecord>> UpdateCategoryAsync(
        Guid categoryRowId,
        long expectedAggregateVersion,
        CategoryDraft draft,
        CancellationToken cancellationToken = default);

    Task<CategoryMutationResult<CategoryRecord>> DeactivateCategoryAsync(
        Guid categoryRowId,
        long expectedAggregateVersion,
        CancellationToken cancellationToken = default);

    Task<CategoryMutationResult<CategoryRecord>> ReactivateCategoryAsync(
        Guid categoryRowId,
        long expectedAggregateVersion,
        CancellationToken cancellationToken = default);

    Task<CategoryMutationResult<CategoryItemRecord>> CreateItemAsync(
        Guid parentCategoryId,
        long expectedAggregateVersion,
        CategoryItemDraft draft,
        CancellationToken cancellationToken = default);

    Task<CategoryMutationResult<CategoryItemRecord>> UpdateItemAsync(
        Guid parentCategoryId,
        Guid itemRowId,
        long expectedAggregateVersion,
        CategoryItemDraft draft,
        CancellationToken cancellationToken = default);

    Task<CategoryMutationResult<CategoryItemRecord>> DeactivateItemAsync(
        Guid parentCategoryId,
        Guid itemRowId,
        long expectedAggregateVersion,
        CancellationToken cancellationToken = default);

    Task<CategoryMutationResult<CategoryItemRecord>> ReactivateItemAsync(
        Guid parentCategoryId,
        Guid itemRowId,
        long expectedAggregateVersion,
        CancellationToken cancellationToken = default);
}

public enum CategoryAggregateStorageErrorCode
{
    VersionConflict,
    DuplicateCode,
    TargetUnavailable,
    ParentMismatch,
    ParentInactive,
    ActiveItemsBlockDeactivation
}

public sealed class CategoryAggregateStorageException : InvalidOperationException
{
    public CategoryAggregateStorageException(
        CategoryAggregateStorageErrorCode errorCode,
        string message)
        : base(message)
    {
        ErrorCode = errorCode;
    }

    public CategoryAggregateStorageErrorCode ErrorCode { get; }
}
