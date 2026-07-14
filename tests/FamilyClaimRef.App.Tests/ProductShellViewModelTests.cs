using FamilyClaimRef.App.Services.Localization;
using FamilyClaimRef.App.Services.Storage;
using FamilyClaimRef.App.Services.UI;
using FamilyClaimRef.App.ViewModels;
using Xunit;

namespace FamilyClaimRef.App.Tests;

public sealed class ProductShellViewModelTests
{
    [Fact]
    public void Constructor_rejects_null_ui_text_provider()
    {
        var uiTextProvider = CreateUiTextProvider();
        var documentRegistration = CreateDocumentRegistrationViewModel(uiTextProvider);

        var exception = Record.Exception(() => new ProductShellViewModel(null!, documentRegistration));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void Constructor_rejects_null_document_registration_view_model()
    {
        var exception = Record.Exception(() => new ProductShellViewModel(CreateUiTextProvider(), null!));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void Constructor_resolves_shell_title()
    {
        var viewModel = CreateViewModel();

        Assert.Equal("FamilyClaimRef", viewModel.ShellTitle);
    }

    [Fact]
    public void Navigation_items_have_expected_count_order_ids_and_display_text()
    {
        var viewModel = CreateViewModel();

        Assert.Collection(
            viewModel.NavigationItems,
            item =>
            {
                Assert.Equal("Home", item.Id);
                Assert.Equal("Home display", item.DisplayText);
            },
            item =>
            {
                Assert.Equal("DocumentRegistration", item.Id);
                Assert.Equal("Registration display", item.DisplayText);
            },
            item =>
            {
                Assert.Equal("DocumentList", item.Id);
                Assert.Equal("List display", item.DisplayText);
            });
    }

    [Fact]
    public void Initial_selection_is_home()
    {
        var viewModel = CreateViewModel();

        Assert.Same(viewModel.NavigationItems[0], viewModel.SelectedNavigationItem);
        Assert.Equal("Home", viewModel.SelectedNavigationItem!.Id);
    }

    [Fact]
    public void Selection_change_raises_PropertyChanged()
    {
        var viewModel = CreateViewModel();
        var propertyNames = new List<string?>();
        viewModel.PropertyChanged += (_, args) => propertyNames.Add(args.PropertyName);

        viewModel.SelectedNavigationItem = viewModel.NavigationItems[1];

        Assert.Same(viewModel.NavigationItems[1], viewModel.SelectedNavigationItem);
        Assert.Equal([nameof(ProductShellViewModel.SelectedNavigationItem)], propertyNames);
    }

    [Fact]
    public void Assigning_same_selection_does_not_raise_PropertyChanged()
    {
        var viewModel = CreateViewModel();
        var notificationCount = 0;
        viewModel.PropertyChanged += (_, _) => notificationCount++;

        viewModel.SelectedNavigationItem = viewModel.SelectedNavigationItem;

        Assert.Equal(0, notificationCount);
    }

    [Fact]
    public void Null_selection_is_ignored()
    {
        var viewModel = CreateViewModel();
        var initialSelection = viewModel.SelectedNavigationItem;

        viewModel.SelectedNavigationItem = null;

        Assert.Same(initialSelection, viewModel.SelectedNavigationItem);
    }

    [Fact]
    public void Foreign_navigation_item_is_rejected()
    {
        var viewModel = CreateViewModel();
        var foreignItem = new ProductNavigationItemViewModel("Foreign", "Foreign display");

        var exception = Record.Exception(() => viewModel.SelectedNavigationItem = foreignItem);

        Assert.IsType<ArgumentException>(exception);
        Assert.Same(viewModel.NavigationItems[0], viewModel.SelectedNavigationItem);
    }

    [Fact]
    public void Constructor_dependencies_match_approved_composition_contract()
    {
        var constructor = Assert.Single(typeof(ProductShellViewModel).GetConstructors());
        var parameters = constructor.GetParameters();

        Assert.Collection(
            parameters,
            parameter => Assert.Equal(typeof(IUiTextProvider), parameter.ParameterType),
            parameter => Assert.Equal(typeof(DocumentRegistrationViewModel), parameter.ParameterType));
    }

    [Fact]
    public void Document_registration_property_exposes_injected_instance()
    {
        var uiTextProvider = CreateUiTextProvider();
        var documentRegistration = CreateDocumentRegistrationViewModel(uiTextProvider);

        var viewModel = new ProductShellViewModel(uiTextProvider, documentRegistration);

        Assert.Same(documentRegistration, viewModel.DocumentRegistration);
    }

    private static ProductShellViewModel CreateViewModel()
    {
        var uiTextProvider = CreateUiTextProvider();
        return new ProductShellViewModel(
            uiTextProvider,
            CreateDocumentRegistrationViewModel(uiTextProvider));
    }

    private static DocumentRegistrationViewModel CreateDocumentRegistrationViewModel(
        IUiTextProvider uiTextProvider)
    {
        var testRoot = Path.Combine(
            Path.GetTempPath(),
            "FamilyClaimRef.App.Tests",
            "ProductShellViewModelTests",
            Guid.NewGuid().ToString("N"));
        var metadataRoot = Path.Combine(testRoot, "data", "local");
        var attachmentRoot = Path.Combine(testRoot, "attachments");
        var documentStorage = new JsonDocumentStorageService(metadataRoot);
        var policyClaimStorage = new JsonPolicyClaimStorageService(metadataRoot);
        var fileAttachment = new LocalFileAttachmentService(attachmentRoot);
        var workflow = new DocumentRegistrationWorkflow(
            new DocumentAttachmentCoordinator(documentStorage, fileAttachment),
            new DocumentLinkCoordinator(documentStorage, policyClaimStorage),
            documentStorage,
            fileAttachment);

        return new DocumentRegistrationViewModel(
            workflow,
            new WpfFilePickerService(),
            policyClaimStorage,
            uiTextProvider);
    }

    private static IUiTextProvider CreateUiTextProvider()
    {
        return new FakeUiTextProvider(new Dictionary<string, string>(StringComparer.Ordinal)
        {
            [UiTextKeys.ProductShellTitle] = "FamilyClaimRef",
            [UiTextKeys.ProductNavigationHome] = "Home display",
            [UiTextKeys.ProductNavigationDocumentRegistration] = "Registration display",
            [UiTextKeys.ProductNavigationDocumentList] = "List display"
        });
    }

    private sealed class FakeUiTextProvider(IReadOnlyDictionary<string, string> values) : IUiTextProvider
    {
        public string Get(string key)
        {
            return values[key];
        }

        public string Format(string key, params object?[] args)
        {
            return string.Format(Get(key), args);
        }
    }
}
