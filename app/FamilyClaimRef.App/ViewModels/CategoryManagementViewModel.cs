using System.ComponentModel;
using System.Runtime.CompilerServices;
using FamilyClaimRef.App.Models.Storage;
using FamilyClaimRef.App.Services.Localization;
using FamilyClaimRef.App.Services.Storage;

namespace FamilyClaimRef.App.ViewModels;

public sealed class CategoryManagementViewModel : INotifyPropertyChanged
{
    private readonly ICategoryAggregateStorageService storageService;
    private readonly IUiTextProvider uiTextProvider;
    private readonly SemaphoreSlim operationGate = new(1, 1);

    private IReadOnlyList<CategoryRecord> categories = [];
    private CategoryRecord? selectedCategory;
    private Guid? editingCategoryRowId;
    private Guid? editingItemRowId;
    private Guid? editingItemParentRowId;
    private long aggregateVersion;
    private string? categoryName;
    private string? categoryCode;
    private string categorySortOrderText = "0";
    private string? categoryDescription;
    private bool categoryIsSystemDefault;
    private CategoryRecord? selectedItemParentCategory;
    private string? itemName;
    private string? itemCode;
    private string itemSortOrderText = "0";
    private string? itemDescription;
    private bool itemUseForPolicySearch = true;
    private bool itemUseForHistorySearch = true;
    private string? managementMessage;
    private bool isBusy;

    public CategoryManagementViewModel(
        ICategoryAggregateStorageService storageService,
        IUiTextProvider uiTextProvider)
    {
        this.storageService = storageService
            ?? throw new ArgumentNullException(nameof(storageService));
        this.uiTextProvider = uiTextProvider
            ?? throw new ArgumentNullException(nameof(uiTextProvider));
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public IReadOnlyList<CategoryRecord> Categories
    {
        get => categories;
        private set
        {
            if (SetProperty(ref categories, value))
            {
                OnPropertyChanged(nameof(HasCategories));
                OnPropertyChanged(nameof(ActiveParentCategories));
            }
        }
    }

    public IReadOnlyList<CategoryRecord> ActiveParentCategories =>
        Categories.Where(category => category.IsActive).ToList();

    public bool HasCategories => Categories.Count > 0;

    public CategoryRecord? SelectedCategory
    {
        get => selectedCategory;
        set
        {
            if (SetProperty(ref selectedCategory, value))
            {
                OnPropertyChanged(nameof(SelectedCategoryItems));
                OnPropertyChanged(nameof(HasSelectedCategory));
                OnPropertyChanged(nameof(CanCreateItem));
            }
        }
    }

    public IReadOnlyList<CategoryItemRecord> SelectedCategoryItems =>
        SelectedCategory?.Items ?? [];

    public bool HasSelectedCategory => SelectedCategory is not null;

    public long AggregateVersion
    {
        get => aggregateVersion;
        private set => SetProperty(ref aggregateVersion, value);
    }

    public string? CategoryName
    {
        get => categoryName;
        set
        {
            if (SetProperty(ref categoryName, value))
            {
                OnCommandStateChanged();
            }
        }
    }

    public string? CategoryCode
    {
        get => categoryCode;
        set
        {
            if (SetProperty(ref categoryCode, value))
            {
                OnCommandStateChanged();
            }
        }
    }

    public string CategorySortOrderText
    {
        get => categorySortOrderText;
        set
        {
            if (SetProperty(ref categorySortOrderText, value))
            {
                OnCommandStateChanged();
            }
        }
    }

    public string? CategoryDescription
    {
        get => categoryDescription;
        set => SetProperty(ref categoryDescription, value);
    }

    public bool CategoryIsSystemDefault
    {
        get => categoryIsSystemDefault;
        set => SetProperty(ref categoryIsSystemDefault, value);
    }

    public CategoryRecord? SelectedItemParentCategory
    {
        get => selectedItemParentCategory;
        set
        {
            if (editingItemRowId is not null
                && value?.RowId != editingItemParentRowId)
            {
                return;
            }

            if (SetProperty(ref selectedItemParentCategory, value))
            {
                OnCommandStateChanged();
            }
        }
    }

    public string? ItemName
    {
        get => itemName;
        set
        {
            if (SetProperty(ref itemName, value))
            {
                OnCommandStateChanged();
            }
        }
    }

    public string? ItemCode
    {
        get => itemCode;
        set
        {
            if (SetProperty(ref itemCode, value))
            {
                OnCommandStateChanged();
            }
        }
    }

    public string ItemSortOrderText
    {
        get => itemSortOrderText;
        set
        {
            if (SetProperty(ref itemSortOrderText, value))
            {
                OnCommandStateChanged();
            }
        }
    }

    public string? ItemDescription
    {
        get => itemDescription;
        set => SetProperty(ref itemDescription, value);
    }

    public bool ItemUseForPolicySearch
    {
        get => itemUseForPolicySearch;
        set => SetProperty(ref itemUseForPolicySearch, value);
    }

    public bool ItemUseForHistorySearch
    {
        get => itemUseForHistorySearch;
        set => SetProperty(ref itemUseForHistorySearch, value);
    }

    public string? ManagementMessage
    {
        get => managementMessage;
        private set => SetProperty(ref managementMessage, value);
    }

    public bool IsBusy
    {
        get => isBusy;
        private set
        {
            if (SetProperty(ref isBusy, value))
            {
                OnCommandStateChanged();
            }
        }
    }

    public bool IsCategoryEditMode => editingCategoryRowId is not null;

    public bool IsItemEditMode => editingItemRowId is not null;

    public bool CanSaveCategory =>
        !IsBusy
        && !string.IsNullOrWhiteSpace(CategoryName)
        && !string.IsNullOrWhiteSpace(CategoryCode)
        && TryParseSortOrder(CategorySortOrderText, out _);

    public bool CanSaveItem =>
        !IsBusy
        && SelectedItemParentCategory?.IsActive == true
        && !string.IsNullOrWhiteSpace(ItemName)
        && !string.IsNullOrWhiteSpace(ItemCode)
        && TryParseSortOrder(ItemSortOrderText, out _);

    public bool CanCreateItem => !IsBusy && SelectedCategory?.IsActive == true;

    public bool CanSelectItemParent => !IsBusy && !IsItemEditMode;

    public bool CanUseRowActions => !IsBusy;

    public bool CanDelete => false;

    internal Guid? EditingCategoryRowId => editingCategoryRowId;

    internal Guid? EditingItemRowId => editingItemRowId;

    internal Guid? EditingItemParentRowId => editingItemParentRowId;

    public void ClearManagementMessage()
    {
        ManagementMessage = null;
    }

    public async Task<bool> LoadAsync(CancellationToken cancellationToken = default)
    {
        if (!await TryEnterOperationAsync(cancellationToken))
        {
            return false;
        }

        try
        {
            ApplySnapshot(await storageService.LoadAsync(cancellationToken));
            return true;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch
        {
            ManagementMessage = Text(UiTextKeys.ProductCategoryLoadFailedMessage);
            return false;
        }
        finally
        {
            ExitOperation();
        }
    }

    public void BeginCategoryCreate()
    {
        if (IsBusy)
        {
            return;
        }

        editingCategoryRowId = null;
        CategoryName = null;
        CategoryCode = null;
        CategorySortOrderText = "0";
        CategoryDescription = null;
        CategoryIsSystemDefault = false;
        ManagementMessage = null;
        OnEditorModeChanged();
    }

    public bool PrepareCategoryEdit(Guid rowId, long expectedAggregateVersion)
    {
        if (expectedAggregateVersion != AggregateVersion)
        {
            ManagementMessage = Text(UiTextKeys.ProductCategoryConflictMessage);
            return false;
        }

        var record = Categories.FirstOrDefault(category => category.RowId == rowId);
        if (record is null)
        {
            ManagementMessage = Text(UiTextKeys.ProductCategoryTargetUnavailableMessage);
            return false;
        }

        editingCategoryRowId = record.RowId;
        CategoryName = record.Name;
        CategoryCode = record.Code;
        CategorySortOrderText = record.SortOrder.ToString();
        CategoryDescription = record.Description;
        CategoryIsSystemDefault = record.IsSystemDefault;
        ManagementMessage = null;
        OnEditorModeChanged();
        return true;
    }

    public async Task<bool> SaveCategoryAsync(CancellationToken cancellationToken = default)
    {
        if (!await TryEnterOperationAsync(cancellationToken))
        {
            return false;
        }

        try
        {
            if (!TryCreateCategoryDraft(out var draft))
            {
                ManagementMessage = Text(UiTextKeys.ProductCategoryValidationMessage);
                return false;
            }

            CategoryMutationResult<CategoryRecord> result;
            if (editingCategoryRowId is Guid rowId)
            {
                result = await storageService.UpdateCategoryAsync(
                    rowId,
                    AggregateVersion,
                    draft,
                    cancellationToken);
            }
            else
            {
                result = await storageService.CreateCategoryAsync(
                    AggregateVersion,
                    draft,
                    cancellationToken);
            }

            ApplySnapshot(result.Snapshot, result.Record.RowId);
            PrepareCategoryEdit(result.Record.RowId, result.Snapshot.AggregateVersion);
            ManagementMessage = Text(UiTextKeys.ProductCategorySavedMessage);
            return true;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (CategoryAggregateStorageException exception)
        {
            ManagementMessage = MapStorageError(exception.ErrorCode);
            return false;
        }
        catch (ArgumentException)
        {
            ManagementMessage = Text(UiTextKeys.ProductCategoryValidationMessage);
            return false;
        }
        catch
        {
            ManagementMessage = Text(UiTextKeys.ProductCategoryOperationFailedMessage);
            return false;
        }
        finally
        {
            ExitOperation();
        }
    }

    public Task<bool> DeactivateCategoryAsync(
        Guid rowId,
        long expectedAggregateVersion,
        CancellationToken cancellationToken = default)
    {
        return MutateCategoryLifecycleAsync(
            rowId,
            expectedAggregateVersion,
            reactivate: false,
            cancellationToken);
    }

    public Task<bool> ReactivateCategoryAsync(
        Guid rowId,
        long expectedAggregateVersion,
        CancellationToken cancellationToken = default)
    {
        return MutateCategoryLifecycleAsync(
            rowId,
            expectedAggregateVersion,
            reactivate: true,
            cancellationToken);
    }

    public void BeginItemCreate(Guid? preferredParentRowId = null)
    {
        if (IsBusy)
        {
            return;
        }

        editingItemRowId = null;
        editingItemParentRowId = null;
        SelectedItemParentCategory = ActiveParentCategories.FirstOrDefault(category =>
                category.RowId == preferredParentRowId)
            ?? ActiveParentCategories.FirstOrDefault();
        ItemName = null;
        ItemCode = null;
        ItemSortOrderText = "0";
        ItemDescription = null;
        ItemUseForPolicySearch = true;
        ItemUseForHistorySearch = true;
        ManagementMessage = null;
        OnEditorModeChanged();
    }

    public bool PrepareItemEdit(
        Guid parentRowId,
        Guid itemRowId,
        long expectedAggregateVersion)
    {
        if (expectedAggregateVersion != AggregateVersion)
        {
            ManagementMessage = Text(UiTextKeys.ProductCategoryConflictMessage);
            return false;
        }

        var parent = Categories.FirstOrDefault(category => category.RowId == parentRowId);
        var item = parent?.Items.FirstOrDefault(candidate => candidate.RowId == itemRowId);
        if (parent is null || item is null || item.ParentCategoryId != parent.RowId)
        {
            ManagementMessage = Text(UiTextKeys.ProductCategoryTargetUnavailableMessage);
            return false;
        }

        editingItemParentRowId = parent.RowId;
        editingItemRowId = item.RowId;
        selectedItemParentCategory = parent;
        OnPropertyChanged(nameof(SelectedItemParentCategory));
        ItemName = item.Name;
        ItemCode = item.Code;
        ItemSortOrderText = item.SortOrder.ToString();
        ItemDescription = item.Description;
        ItemUseForPolicySearch = item.UseForPolicySearch;
        ItemUseForHistorySearch = item.UseForHistorySearch;
        ManagementMessage = null;
        OnEditorModeChanged();
        return true;
    }

    public async Task<bool> SaveItemAsync(CancellationToken cancellationToken = default)
    {
        if (!await TryEnterOperationAsync(cancellationToken))
        {
            return false;
        }

        try
        {
            if (!TryCreateItemDraft(out var draft)
                || SelectedItemParentCategory is not { IsActive: true } parent)
            {
                ManagementMessage = Text(UiTextKeys.ProductCategoryValidationMessage);
                return false;
            }

            CategoryMutationResult<CategoryItemRecord> result;
            if (editingItemRowId is Guid itemRowId)
            {
                if (editingItemParentRowId != parent.RowId)
                {
                    ManagementMessage = Text(UiTextKeys.ProductCategoryTargetUnavailableMessage);
                    return false;
                }

                result = await storageService.UpdateItemAsync(
                    parent.RowId,
                    itemRowId,
                    AggregateVersion,
                    draft,
                    cancellationToken);
            }
            else
            {
                result = await storageService.CreateItemAsync(
                    parent.RowId,
                    AggregateVersion,
                    draft,
                    cancellationToken);
            }

            ApplySnapshot(result.Snapshot, parent.RowId);
            PrepareItemEdit(parent.RowId, result.Record.RowId, result.Snapshot.AggregateVersion);
            ManagementMessage = Text(UiTextKeys.ProductCategoryItemSavedMessage);
            return true;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (CategoryAggregateStorageException exception)
        {
            ManagementMessage = MapStorageError(exception.ErrorCode);
            return false;
        }
        catch (ArgumentException)
        {
            ManagementMessage = Text(UiTextKeys.ProductCategoryValidationMessage);
            return false;
        }
        catch
        {
            ManagementMessage = Text(UiTextKeys.ProductCategoryOperationFailedMessage);
            return false;
        }
        finally
        {
            ExitOperation();
        }
    }

    public Task<bool> DeactivateItemAsync(
        Guid parentRowId,
        Guid itemRowId,
        long expectedAggregateVersion,
        CancellationToken cancellationToken = default)
    {
        return MutateItemLifecycleAsync(
            parentRowId,
            itemRowId,
            expectedAggregateVersion,
            reactivate: false,
            cancellationToken);
    }

    public Task<bool> ReactivateItemAsync(
        Guid parentRowId,
        Guid itemRowId,
        long expectedAggregateVersion,
        CancellationToken cancellationToken = default)
    {
        return MutateItemLifecycleAsync(
            parentRowId,
            itemRowId,
            expectedAggregateVersion,
            reactivate: true,
            cancellationToken);
    }

    private async Task<bool> MutateCategoryLifecycleAsync(
        Guid rowId,
        long expectedAggregateVersion,
        bool reactivate,
        CancellationToken cancellationToken)
    {
        if (!await TryEnterOperationAsync(cancellationToken))
        {
            return false;
        }

        try
        {
            CategoryMutationResult<CategoryRecord> result = reactivate
                ? await storageService.ReactivateCategoryAsync(
                    rowId,
                    expectedAggregateVersion,
                    cancellationToken)
                : await storageService.DeactivateCategoryAsync(
                    rowId,
                    expectedAggregateVersion,
                    cancellationToken);
            ApplySnapshot(result.Snapshot, rowId);
            ManagementMessage = Text(reactivate
                ? UiTextKeys.ProductCategoryReactivatedMessage
                : UiTextKeys.ProductCategoryDeactivatedMessage);
            return true;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (CategoryAggregateStorageException exception)
        {
            ManagementMessage = MapStorageError(exception.ErrorCode);
            return false;
        }
        catch
        {
            ManagementMessage = Text(UiTextKeys.ProductCategoryOperationFailedMessage);
            return false;
        }
        finally
        {
            ExitOperation();
        }
    }

    private async Task<bool> MutateItemLifecycleAsync(
        Guid parentRowId,
        Guid itemRowId,
        long expectedAggregateVersion,
        bool reactivate,
        CancellationToken cancellationToken)
    {
        if (!await TryEnterOperationAsync(cancellationToken))
        {
            return false;
        }

        try
        {
            CategoryMutationResult<CategoryItemRecord> result = reactivate
                ? await storageService.ReactivateItemAsync(
                    parentRowId,
                    itemRowId,
                    expectedAggregateVersion,
                    cancellationToken)
                : await storageService.DeactivateItemAsync(
                    parentRowId,
                    itemRowId,
                    expectedAggregateVersion,
                    cancellationToken);
            ApplySnapshot(result.Snapshot, parentRowId);
            ManagementMessage = Text(reactivate
                ? UiTextKeys.ProductCategoryItemReactivatedMessage
                : UiTextKeys.ProductCategoryItemDeactivatedMessage);
            return true;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (CategoryAggregateStorageException exception)
        {
            ManagementMessage = MapStorageError(exception.ErrorCode);
            return false;
        }
        catch
        {
            ManagementMessage = Text(UiTextKeys.ProductCategoryOperationFailedMessage);
            return false;
        }
        finally
        {
            ExitOperation();
        }
    }

    private void ApplySnapshot(CategoryAggregateSnapshot snapshot, Guid? selectedRowId = null)
    {
        var preservedSelection = selectedRowId ?? SelectedCategory?.RowId;
        AggregateVersion = snapshot.AggregateVersion;
        Categories = snapshot.Categories;
        SelectedCategory = Categories.FirstOrDefault(category => category.RowId == preservedSelection)
            ?? Categories.FirstOrDefault();

        if (selectedItemParentCategory is not null)
        {
            selectedItemParentCategory = Categories.FirstOrDefault(category =>
                category.RowId == selectedItemParentCategory.RowId);
            OnPropertyChanged(nameof(SelectedItemParentCategory));
        }
    }

    private bool TryCreateCategoryDraft(out CategoryDraft draft)
    {
        if (string.IsNullOrWhiteSpace(CategoryName)
            || string.IsNullOrWhiteSpace(CategoryCode)
            || !TryParseSortOrder(CategorySortOrderText, out var sortOrder))
        {
            draft = null!;
            return false;
        }

        draft = new CategoryDraft(
            CategoryName,
            CategoryCode,
            sortOrder,
            CategoryDescription,
            CategoryIsSystemDefault);
        return true;
    }

    private bool TryCreateItemDraft(out CategoryItemDraft draft)
    {
        if (string.IsNullOrWhiteSpace(ItemName)
            || string.IsNullOrWhiteSpace(ItemCode)
            || !TryParseSortOrder(ItemSortOrderText, out var sortOrder))
        {
            draft = null!;
            return false;
        }

        draft = new CategoryItemDraft(
            ItemName,
            ItemCode,
            sortOrder,
            ItemDescription,
            ItemUseForPolicySearch,
            ItemUseForHistorySearch);
        return true;
    }

    private string MapStorageError(CategoryAggregateStorageErrorCode errorCode)
    {
        return Text(errorCode switch
        {
            CategoryAggregateStorageErrorCode.VersionConflict =>
                UiTextKeys.ProductCategoryConflictMessage,
            CategoryAggregateStorageErrorCode.DuplicateCode =>
                UiTextKeys.ProductCategoryDuplicateCodeMessage,
            CategoryAggregateStorageErrorCode.ParentInactive =>
                UiTextKeys.ProductCategoryParentInactiveMessage,
            CategoryAggregateStorageErrorCode.ActiveItemsBlockDeactivation =>
                UiTextKeys.ProductCategoryActiveItemsBlockMessage,
            _ => UiTextKeys.ProductCategoryTargetUnavailableMessage
        });
    }

    private string Text(string key) => uiTextProvider.Get(key);

    private static bool TryParseSortOrder(string? value, out int sortOrder)
    {
        return int.TryParse(value, out sortOrder) && sortOrder >= 0;
    }

    private async Task<bool> TryEnterOperationAsync(CancellationToken cancellationToken)
    {
        var entered = await operationGate.WaitAsync(0, cancellationToken);
        if (!entered)
        {
            return false;
        }

        IsBusy = true;
        return true;
    }

    private void ExitOperation()
    {
        IsBusy = false;
        operationGate.Release();
    }

    private void OnEditorModeChanged()
    {
        OnPropertyChanged(nameof(IsCategoryEditMode));
        OnPropertyChanged(nameof(IsItemEditMode));
        OnCommandStateChanged();
    }

    private void OnCommandStateChanged()
    {
        OnPropertyChanged(nameof(CanSaveCategory));
        OnPropertyChanged(nameof(CanSaveItem));
        OnPropertyChanged(nameof(CanCreateItem));
        OnPropertyChanged(nameof(CanSelectItemParent));
        OnPropertyChanged(nameof(CanUseRowActions));
        OnPropertyChanged(nameof(CanDelete));
    }

    private bool SetProperty<T>(
        ref T field,
        T value,
        [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return false;
        }

        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
