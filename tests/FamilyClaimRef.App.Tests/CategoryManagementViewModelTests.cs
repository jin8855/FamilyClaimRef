using System.Xml.Linq;
using FamilyClaimRef.App.Composition;
using FamilyClaimRef.App.Models.Storage;
using FamilyClaimRef.App.Services.Localization;
using FamilyClaimRef.App.Services.Runtime;
using FamilyClaimRef.App.Services.Storage;
using FamilyClaimRef.App.ViewModels;
using Xunit;

namespace FamilyClaimRef.App.Tests;

public sealed class CategoryManagementViewModelTests
{
    [Fact]
    public void Category_records_use_safe_display_names_for_automation()
    {
        var categoryId = Guid.NewGuid();
        var itemId = Guid.NewGuid();
        var now = DateTimeOffset.UtcNow;
        var item = new CategoryItemRecord(
            itemId,
            categoryId,
            "Synthetic item",
            "ITEM",
            0,
            null,
            true,
            true,
            now,
            now,
            null);
        var category = new CategoryRecord(
            categoryId,
            "Synthetic category",
            "CATEGORY",
            0,
            null,
            false,
            now,
            now,
            null,
            [item]);

        Assert.Equal("Synthetic category", category.ToString());
        Assert.Equal("Synthetic item", item.ToString());
        Assert.DoesNotContain(categoryId.ToString(), category.ToString(), StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain(itemId.ToString(), item.ToString(), StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Load_exposes_empty_version_zero_state_without_creating_file()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var viewModel = CreateViewModel(rootPath);

            Assert.True(await viewModel.LoadAsync());

            Assert.Equal(0, viewModel.AggregateVersion);
            Assert.Empty(viewModel.Categories);
            Assert.False(viewModel.HasCategories);
            Assert.False(File.Exists(StorePath(rootPath)));
        });
    }

    [Fact]
    public async Task Category_and_item_save_persist_across_new_view_model_instances()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var first = CreateViewModel(rootPath);
            await first.LoadAsync();
            FillCategory(first, "진료", "CARE");

            Assert.True(await first.SaveCategoryAsync());
            var category = Assert.Single(first.Categories);
            first.BeginItemCreate(category.RowId);
            FillItem(first, "통원", "OUTPATIENT");
            Assert.True(await first.SaveItemAsync());

            var reloaded = CreateViewModel(rootPath);
            Assert.True(await reloaded.LoadAsync());
            var reloadedCategory = Assert.Single(reloaded.Categories);
            var reloadedItem = Assert.Single(reloadedCategory.Items);
            Assert.Equal(category.RowId, reloadedCategory.RowId);
            Assert.Equal("CARE", reloadedCategory.Code);
            Assert.Equal("OUTPATIENT", reloadedItem.Code);
            Assert.Equal(reloadedCategory.RowId, reloadedItem.ParentCategoryId);
            Assert.Equal(2, reloaded.AggregateVersion);
        });
    }

    [Fact]
    public async Task Validation_failure_keeps_editor_state_and_does_not_write()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var viewModel = CreateViewModel(rootPath);
            await viewModel.LoadAsync();
            viewModel.CategoryName = "synthetic";
            viewModel.CategoryCode = " ";
            viewModel.CategorySortOrderText = "not-a-number";

            Assert.False(await viewModel.SaveCategoryAsync());

            Assert.Equal(UiTextKeys.ProductCategoryValidationMessage, viewModel.ManagementMessage);
            Assert.Equal("synthetic", viewModel.CategoryName);
            Assert.Equal(0, viewModel.AggregateVersion);
            Assert.False(File.Exists(StorePath(rootPath)));
        });
    }

    [Fact]
    public async Task Duplicate_and_stale_conflict_use_safe_product_messages()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var first = CreateViewModel(rootPath);
            var stale = CreateViewModel(rootPath);
            await first.LoadAsync();
            await stale.LoadAsync();
            FillCategory(first, "first", "SAME");
            FillCategory(stale, "stale", "STALE");
            Assert.True(await first.SaveCategoryAsync());

            Assert.False(await stale.SaveCategoryAsync());
            Assert.Equal(UiTextKeys.ProductCategoryConflictMessage, stale.ManagementMessage);

            var current = CreateViewModel(rootPath);
            await current.LoadAsync();
            current.BeginCategoryCreate();
            FillCategory(current, "duplicate", " same ");
            Assert.False(await current.SaveCategoryAsync());
            Assert.Equal(UiTextKeys.ProductCategoryDuplicateCodeMessage, current.ManagementMessage);
            Assert.DoesNotContain(rootPath, current.ManagementMessage, StringComparison.OrdinalIgnoreCase);
        });
    }

    [Fact]
    public async Task Active_item_deactivation_block_and_parent_inactive_error_are_visible()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var viewModel = CreateViewModel(rootPath);
            await viewModel.LoadAsync();
            FillCategory(viewModel, "category", "C");
            await viewModel.SaveCategoryAsync();
            var parent = Assert.Single(viewModel.Categories);
            viewModel.BeginItemCreate(parent.RowId);
            FillItem(viewModel, "item", "I");
            await viewModel.SaveItemAsync();
            var item = Assert.Single(viewModel.Categories.Single().Items);

            Assert.False(await viewModel.DeactivateCategoryAsync(
                parent.RowId,
                viewModel.AggregateVersion));
            Assert.Equal(
                UiTextKeys.ProductCategoryActiveItemsBlockMessage,
                viewModel.ManagementMessage);

            Assert.True(await viewModel.DeactivateItemAsync(
                parent.RowId,
                item.RowId,
                viewModel.AggregateVersion));
            Assert.True(await viewModel.DeactivateCategoryAsync(
                parent.RowId,
                viewModel.AggregateVersion));
            viewModel.BeginItemCreate(parent.RowId);
            FillItem(viewModel, "blocked", "BLOCKED");
            Assert.False(await viewModel.SaveItemAsync());
            Assert.Equal(UiTextKeys.ProductCategoryValidationMessage, viewModel.ManagementMessage);
        });
    }

    [Fact]
    public async Task Editing_uses_explicit_row_and_disallows_parent_selector_change()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var service = new JsonCategoryAggregateStorageService(rootPath);
            var first = await service.CreateCategoryAsync(0, CategoryDraft("first", "C1"));
            var second = await service.CreateCategoryAsync(1, CategoryDraft("second", "C2"));
            var item = await service.CreateItemAsync(
                first.Record.RowId,
                2,
                ItemDraft("item", "I"));
            var viewModel = CreateViewModel(rootPath);
            await viewModel.LoadAsync();

            Assert.True(viewModel.PrepareItemEdit(
                first.Record.RowId,
                item.Record.RowId,
                viewModel.AggregateVersion));
            Assert.False(viewModel.CanSelectItemParent);
            viewModel.SelectedItemParentCategory = viewModel.Categories.Single(c =>
                c.RowId == second.Record.RowId);

            Assert.Equal(first.Record.RowId, viewModel.SelectedItemParentCategory?.RowId);
            viewModel.ItemName = "updated item";
            Assert.True(await viewModel.SaveItemAsync());
            var snapshot = await service.LoadAsync();
            Assert.Equal("updated item", snapshot.Categories
                .Single(c => c.RowId == first.Record.RowId).Items.Single().Name);
            Assert.Empty(snapshot.Categories.Single(c => c.RowId == second.Record.RowId).Items);
        });
    }

    [Fact]
    public async Task Screen_19_and_20_shell_flow_saves_returns_to_16_and_reloads()
    {
        await UsingTempRootAsync(async runtimeRoot =>
        {
            var services = AppServices.Create(new StubRuntimeRootProvider(
                RuntimeRootPaths.FromRuntimeRoot(runtimeRoot)));
            var shell = services.ProductShellViewModel;
            await shell.CategoryManagement.LoadAsync();

            shell.NavigateToCategoryCreate();
            Assert.Equal(ProductScreenRoutes.CategoryRegister, shell.CurrentRouteId);
            FillCategory(shell.CategoryManagement, "보험 분류", "POLICY");
            Assert.True(await shell.SaveCategoryAndReturnAsync());
            Assert.Equal(ProductScreenRoutes.CategoryManage, shell.CurrentRouteId);

            var category = Assert.Single(shell.CategoryManagement.Categories);
            shell.NavigateToCategoryItemCreate(category.RowId);
            Assert.Equal(ProductScreenRoutes.CategoryItemRegister, shell.CurrentRouteId);
            FillItem(shell.CategoryManagement, "보장 항목", "COVERAGE");
            Assert.True(await shell.SaveCategoryItemAndReturnAsync());
            Assert.Equal(ProductScreenRoutes.CategoryManage, shell.CurrentRouteId);

            var secondGraph = AppServices.Create(new StubRuntimeRootProvider(
                RuntimeRootPaths.FromRuntimeRoot(runtimeRoot)));
            Assert.True(await secondGraph.ProductShellViewModel.CategoryManagement.LoadAsync());
            var reloadedCategory = Assert.Single(
                secondGraph.ProductShellViewModel.CategoryManagement.Categories);
            Assert.Single(reloadedCategory.Items);
            Assert.Equal(2, secondGraph.ProductShellViewModel.CategoryManagement.AggregateVersion);
        });
    }

    [Fact]
    public void Product_shell_maps_routes_16_19_20_to_dedicated_views_and_locators()
    {
        var projectRoot = FindProjectRoot();
        var shellXaml = XDocument.Load(Path.Combine(
            projectRoot,
            "app",
            "FamilyClaimRef.App",
            "ProductShell",
            "ProductShellWindow.xaml"));
        var shellText = shellXaml.ToString();

        Assert.Contains("ProductCategoryManagementView", shellText, StringComparison.Ordinal);
        Assert.Contains("ProductCategoryEditorView", shellText, StringComparison.Ordinal);
        Assert.Contains("ProductCategoryItemEditorView", shellText, StringComparison.Ordinal);
        Assert.Contains(ProductScreenRoutes.CategoryManage, shellText, StringComparison.Ordinal);
        Assert.Contains(ProductScreenRoutes.CategoryRegister, shellText, StringComparison.Ordinal);
        Assert.Contains(ProductScreenRoutes.CategoryItemRegister, shellText, StringComparison.Ordinal);

        AssertViewLocator(projectRoot, "ProductCategoryManagementView.xaml", "ProductScreen_16");
        AssertViewLocator(projectRoot, "ProductCategoryEditorView.xaml", "ProductScreen_19");
        AssertViewLocator(projectRoot, "ProductCategoryItemEditorView.xaml", "ProductScreen_20");
    }

    private static void AssertViewLocator(string projectRoot, string fileName, string locator)
    {
        var text = File.ReadAllText(Path.Combine(
            projectRoot,
            "app",
            "FamilyClaimRef.App",
            "Views",
            fileName));
        Assert.Contains(locator, text, StringComparison.Ordinal);
    }

    private static CategoryManagementViewModel CreateViewModel(string rootPath)
    {
        return new CategoryManagementViewModel(
            new JsonCategoryAggregateStorageService(rootPath),
            new KeyUiTextProvider());
    }

    private static void FillCategory(
        CategoryManagementViewModel viewModel,
        string name,
        string code)
    {
        viewModel.CategoryName = name;
        viewModel.CategoryCode = code;
        viewModel.CategorySortOrderText = "10";
        viewModel.CategoryDescription = "synthetic category";
    }

    private static void FillItem(
        CategoryManagementViewModel viewModel,
        string name,
        string code)
    {
        viewModel.ItemName = name;
        viewModel.ItemCode = code;
        viewModel.ItemSortOrderText = "20";
        viewModel.ItemDescription = "synthetic item";
        viewModel.ItemUseForPolicySearch = true;
        viewModel.ItemUseForHistorySearch = true;
    }

    private static CategoryDraft CategoryDraft(string name, string code)
    {
        return new CategoryDraft(name, code, 0, null, false);
    }

    private static CategoryItemDraft ItemDraft(string name, string code)
    {
        return new CategoryItemDraft(name, code, 0, null, true, true);
    }

    private static string StorePath(string rootPath)
    {
        return Path.Combine(rootPath, JsonCategoryAggregateStorageService.StoreFileName);
    }

    private static async Task UsingTempRootAsync(Func<string, Task> action)
    {
        var rootPath = Path.Combine(
            Path.GetTempPath(),
            "FamilyClaimRef.App.Tests",
            "CategoryManagement",
            Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(rootPath);

        try
        {
            await action(rootPath);
        }
        finally
        {
            if (Directory.Exists(rootPath))
            {
                Directory.Delete(rootPath, recursive: true);
            }
        }
    }

    private static string FindProjectRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "FamilyClaimRef.sln")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new InvalidOperationException("Project root was not found.");
    }

    private sealed class KeyUiTextProvider : IUiTextProvider
    {
        public string Get(string key) => key;

        public string Format(string key, params object?[] args) => string.Format(Get(key), args);
    }

    private sealed class StubRuntimeRootProvider(RuntimeRootPaths paths) : IRuntimeRootProvider
    {
        public RuntimeRootPaths GetRuntimeRootPaths() => paths;
    }
}
