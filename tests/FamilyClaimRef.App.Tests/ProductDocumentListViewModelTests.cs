using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using FamilyClaimRef.App.Models.Storage;
using FamilyClaimRef.App.Services.Localization;
using FamilyClaimRef.App.Services.Storage;
using FamilyClaimRef.App.ViewModels;
using Xunit;

namespace FamilyClaimRef.App.Tests;

public sealed class ProductDocumentListViewModelTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Item_constructor_rejects_null_empty_or_whitespace_title(string? displayTitle)
    {
        var exception = Record.Exception(() => new ProductDocumentListItemViewModel(displayTitle!));

        Assert.NotNull(exception);
        if (displayTitle is null)
        {
            Assert.IsType<ArgumentNullException>(exception);
        }
        else
        {
            Assert.IsType<ArgumentException>(exception);
        }
    }

    [Fact]
    public void Item_is_immutable_and_exposes_the_approved_document_box_columns()
    {
        const string displayTitle = "  Synthetic title  ";
        var item = new ProductDocumentListItemViewModel(displayTitle);
        var publicProperties = typeof(ProductDocumentListItemViewModel)
            .GetProperties(BindingFlags.Instance | BindingFlags.Public);

        Assert.Equal(
            [
                nameof(ProductDocumentListItemViewModel.DisplayTitle),
                nameof(ProductDocumentListItemViewModel.DocumentType),
                nameof(ProductDocumentListItemViewModel.OcrStatus),
                nameof(ProductDocumentListItemViewModel.Purpose),
                nameof(ProductDocumentListItemViewModel.ReferenceDate),
                nameof(ProductDocumentListItemViewModel.Target)
            ],
            publicProperties.Select(property => property.Name).Order(StringComparer.Ordinal));
        Assert.All(publicProperties, property => Assert.False(property.CanWrite));
        Assert.Equal(displayTitle, item.DisplayTitle);
        Assert.DoesNotContain(typeof(INotifyPropertyChanged), typeof(ProductDocumentListItemViewModel).GetInterfaces());
    }

    [Fact]
    public void Constructor_rejects_null_storage()
    {
        var exception = Record.Exception(
            () => new ProductDocumentListViewModel(null!, CreateUiTextProvider()));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void Constructor_rejects_null_ui_text_provider()
    {
        var exception = Record.Exception(
            () => new ProductDocumentListViewModel(new FakeDocumentStorageService(), null!));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void Constructor_resolves_copy_and_starts_in_initial_state()
    {
        var viewModel = CreateViewModel(new FakeDocumentStorageService());

        Assert.Equal("Document list", viewModel.Title);
        Assert.Equal("No documents", viewModel.EmptyMessage);
        Assert.Equal("Unable to load documents", viewModel.LoadFailedMessage);
        Assert.Empty(viewModel.Items);
        AssertExclusiveState(viewModel, isLoading: false, isEmpty: false, hasLoadError: false);
    }

    [Fact]
    public async Task Empty_source_produces_successful_empty_state()
    {
        var storage = new FakeDocumentStorageService();
        storage.EnqueueDocuments();
        var viewModel = CreateViewModel(storage);
        var propertyNames = new List<string?>();
        viewModel.PropertyChanged += (_, args) => propertyNames.Add(args.PropertyName);

        await viewModel.LoadAsync();

        Assert.Empty(viewModel.Items);
        AssertExclusiveState(viewModel, isLoading: false, isEmpty: true, hasLoadError: false);
        Assert.Contains(nameof(ProductDocumentListViewModel.Items), propertyNames);
        Assert.Contains(nameof(ProductDocumentListViewModel.IsLoading), propertyNames);
        Assert.Contains(nameof(ProductDocumentListViewModel.IsEmpty), propertyNames);
        Assert.Equal(1, storage.GetDocumentsCallCount);
        Assert.Equal(0, storage.UnexpectedCallCount);
    }

    [Fact]
    public async Task Load_filters_disabled_documents_and_projects_titles_in_source_order()
    {
        var storage = new FakeDocumentStorageService();
        storage.EnqueueDocuments(
            CreateDocument("First title"),
            CreateDocument("Disabled title", DateTimeOffset.UtcNow),
            CreateDocument("Second title"));
        var viewModel = CreateViewModel(storage);

        await viewModel.LoadAsync();

        Assert.Equal(["First title", "Second title"], viewModel.Items.Select(item => item.DisplayTitle));
        Assert.All(viewModel.Items, item => Assert.Equal("Purpose unavailable", item.Purpose));
        AssertExclusiveState(viewModel, isLoading: false, isEmpty: false, hasLoadError: false);
        Assert.Equal(0, storage.UnexpectedCallCount);
    }

    [Fact]
    public async Task Load_projects_available_metadata_and_explicit_empty_values()
    {
        var storage = new FakeDocumentStorageService();
        storage.EnqueueDocuments(
            CreateDocument(
                "Policy terms",
                documentType: "terms",
                referenceDate: new DateOnly(2026, 7, 31)),
            CreateDocument("Unknown", documentType: null));
        var viewModel = CreateViewModel(storage);

        await viewModel.LoadAsync();

        var policy = viewModel.Items[0];
        Assert.Equal("Managed purpose", policy.Purpose);
        Assert.Equal("약관", policy.DocumentType);
        Assert.Equal("Target unavailable", policy.Target);
        Assert.Equal("OCR unavailable", policy.OcrStatus);
        Assert.Equal("2026-07-31", policy.ReferenceDate);

        var unknown = viewModel.Items[1];
        Assert.Equal("Purpose unavailable", unknown.Purpose);
        Assert.Equal("Empty value", unknown.DocumentType);
        Assert.Equal("Empty value", unknown.ReferenceDate);
    }

    [Fact]
    public async Task Loading_clears_prior_items_and_keeps_state_exclusive()
    {
        var storage = new FakeDocumentStorageService();
        storage.EnqueueDocuments(CreateDocument("Prior title"));
        var pendingLoad = new TaskCompletionSource<IReadOnlyList<DocumentRecord>>(
            TaskCreationOptions.RunContinuationsAsynchronously);
        storage.Enqueue(_ => pendingLoad.Task);
        var viewModel = CreateViewModel(storage);
        await viewModel.LoadAsync();
        var propertyNames = new List<string?>();
        viewModel.PropertyChanged += (_, args) => propertyNames.Add(args.PropertyName);

        var loadTask = viewModel.LoadAsync();

        Assert.Empty(viewModel.Items);
        AssertExclusiveState(viewModel, isLoading: true, isEmpty: false, hasLoadError: false);
        Assert.Contains(nameof(ProductDocumentListViewModel.Items), propertyNames);
        Assert.Contains(nameof(ProductDocumentListViewModel.IsLoading), propertyNames);

        pendingLoad.SetResult([CreateDocument("Current title")]);
        await loadTask;

        Assert.Equal("Current title", Assert.Single(viewModel.Items).DisplayTitle);
        AssertExclusiveState(viewModel, isLoading: false, isEmpty: false, hasLoadError: false);
    }

    [Fact]
    public async Task Sequential_reload_replaces_snapshot_without_duplicates()
    {
        var storage = new FakeDocumentStorageService();
        storage.EnqueueDocuments(CreateDocument("First title"), CreateDocument("Second title"));
        storage.EnqueueDocuments(CreateDocument("Replacement title"));
        var viewModel = CreateViewModel(storage);

        await viewModel.LoadAsync();
        var firstSnapshot = viewModel.Items;
        await viewModel.LoadAsync();

        Assert.NotSame(firstSnapshot, viewModel.Items);
        Assert.Equal("Replacement title", Assert.Single(viewModel.Items).DisplayTitle);
        Assert.DoesNotContain(viewModel.Items, item => item.DisplayTitle == "First title");
        Assert.Equal(2, storage.GetDocumentsCallCount);
        Assert.Equal(0, storage.UnexpectedCallCount);
    }

    [Fact]
    public async Task InvalidOperationException_from_storage_produces_load_error()
    {
        await AssertStorageFailureAsync(new InvalidOperationException("raw invalid operation detail"));
    }

    [Fact]
    public async Task IOException_from_storage_produces_load_error()
    {
        await AssertStorageFailureAsync(new IOException("raw I/O detail"));
    }

    [Fact]
    public async Task UnauthorizedAccessException_from_storage_produces_load_error()
    {
        await AssertStorageFailureAsync(new UnauthorizedAccessException("raw access detail"));
    }

    [Fact]
    public async Task Invalid_projected_title_produces_load_error_without_partial_items()
    {
        var storage = new FakeDocumentStorageService();
        storage.EnqueueDocuments(CreateDocument("Valid title"), CreateDocument("   "));
        var viewModel = CreateViewModel(storage);

        await viewModel.LoadAsync();

        Assert.Empty(viewModel.Items);
        AssertExclusiveState(viewModel, isLoading: false, isEmpty: false, hasLoadError: true);
        Assert.Equal("Unable to load documents", viewModel.LoadFailedMessage);
        Assert.Equal(0, storage.UnexpectedCallCount);
    }

    [Fact]
    public async Task Success_after_failure_clears_error_state()
    {
        var storage = new FakeDocumentStorageService();
        storage.Enqueue(_ => Task.FromException<IReadOnlyList<DocumentRecord>>(
            new IOException("raw failure detail")));
        storage.EnqueueDocuments(CreateDocument("Recovered title"));
        var viewModel = CreateViewModel(storage);
        var propertyNames = new List<string?>();
        viewModel.PropertyChanged += (_, args) => propertyNames.Add(args.PropertyName);

        await viewModel.LoadAsync();
        Assert.True(viewModel.HasLoadError);
        await viewModel.LoadAsync();

        Assert.Equal("Recovered title", Assert.Single(viewModel.Items).DisplayTitle);
        AssertExclusiveState(viewModel, isLoading: false, isEmpty: false, hasLoadError: false);
        Assert.True(propertyNames.Count(name => name == nameof(ProductDocumentListViewModel.HasLoadError)) >= 2);
    }

    [Fact]
    public async Task Cancellation_propagates_and_resets_initial_state()
    {
        var storage = new FakeDocumentStorageService();
        storage.Enqueue(cancellationToken =>
            Task.FromCanceled<IReadOnlyList<DocumentRecord>>(cancellationToken));
        var viewModel = CreateViewModel(storage);
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => viewModel.LoadAsync(cancellationTokenSource.Token));

        Assert.Empty(viewModel.Items);
        AssertExclusiveState(viewModel, isLoading: false, isEmpty: false, hasLoadError: false);
        Assert.Equal(0, storage.UnexpectedCallCount);
    }

    [Fact]
    public async Task Items_exposes_read_only_replacement_snapshot()
    {
        var storage = new FakeDocumentStorageService();
        storage.EnqueueDocuments(CreateDocument("Snapshot title"));
        var viewModel = CreateViewModel(storage);
        ReadOnlyCollection<ProductDocumentListItemViewModel> initialSnapshot = viewModel.Items;

        await viewModel.LoadAsync();

        ReadOnlyCollection<ProductDocumentListItemViewModel> loadedSnapshot = viewModel.Items;
        Assert.NotSame(initialSnapshot, loadedSnapshot);
        Assert.True(((ICollection<ProductDocumentListItemViewModel>)loadedSnapshot).IsReadOnly);
        Assert.Throws<NotSupportedException>(
            () => ((IList<ProductDocumentListItemViewModel>)loadedSnapshot)
                .Add(new ProductDocumentListItemViewModel("Another title")));
    }

    private static async Task AssertStorageFailureAsync(Exception exception)
    {
        var storage = new FakeDocumentStorageService();
        storage.Enqueue(_ => Task.FromException<IReadOnlyList<DocumentRecord>>(exception));
        var viewModel = CreateViewModel(storage);
        var propertyNames = new List<string?>();
        viewModel.PropertyChanged += (_, args) => propertyNames.Add(args.PropertyName);

        await viewModel.LoadAsync();

        Assert.Empty(viewModel.Items);
        AssertExclusiveState(viewModel, isLoading: false, isEmpty: false, hasLoadError: true);
        Assert.Equal("Unable to load documents", viewModel.LoadFailedMessage);
        Assert.DoesNotContain("raw", viewModel.LoadFailedMessage, StringComparison.OrdinalIgnoreCase);
        Assert.Contains(nameof(ProductDocumentListViewModel.HasLoadError), propertyNames);
        Assert.Equal(0, storage.UnexpectedCallCount);
    }

    private static void AssertExclusiveState(
        ProductDocumentListViewModel viewModel,
        bool isLoading,
        bool isEmpty,
        bool hasLoadError)
    {
        Assert.Equal(isLoading, viewModel.IsLoading);
        Assert.Equal(isEmpty, viewModel.IsEmpty);
        Assert.Equal(hasLoadError, viewModel.HasLoadError);
        Assert.InRange(
            new[] { viewModel.IsLoading, viewModel.IsEmpty, viewModel.HasLoadError }.Count(value => value),
            0,
            1);
    }

    private static ProductDocumentListViewModel CreateViewModel(FakeDocumentStorageService storage)
    {
        return new ProductDocumentListViewModel(storage, CreateUiTextProvider());
    }

    private static IUiTextProvider CreateUiTextProvider()
    {
        return new FakeUiTextProvider(new Dictionary<string, string>(StringComparer.Ordinal)
        {
            [UiTextKeys.ProductDocumentListTitle] = "Document list",
            [UiTextKeys.ProductDocumentListEmptyMessage] = "No documents",
            [UiTextKeys.ProductDocumentListLoadFailedMessage] = "Unable to load documents",
            [ProductScreenTextKeys.EmptyValue] = "Empty value",
            [ProductScreenTextKeys.DocumentManagedPurpose] = "Managed purpose",
            [ProductScreenTextKeys.DocumentClaimPurpose] = "Claim purpose",
            [ProductScreenTextKeys.DocumentPurposeUnavailable] = "Purpose unavailable",
            [ProductScreenTextKeys.DocumentTargetUnavailable] = "Target unavailable",
            [ProductScreenTextKeys.DocumentOcrUnavailable] = "OCR unavailable"
        });
    }

    private static DocumentRecord CreateDocument(
        string displayTitle,
        DateTimeOffset? disabledAt = null,
        string? documentType = null,
        DateOnly? referenceDate = null)
    {
        var timestamp = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var identifier = Guid.NewGuid().ToString("N");
        return new DocumentRecord(
            identifier,
            $"{identifier}.png",
            displayTitle,
            "png",
            $"documents/{identifier}.png",
            timestamp,
            timestamp,
            disabledAt,
            DocumentType: documentType,
            ReferenceDate: referenceDate);
    }

    private sealed class FakeUiTextProvider(IReadOnlyDictionary<string, string> values) : IUiTextProvider
    {
        public string Get(string key)
        {
            return values.TryGetValue(key, out var value) ? value : key;
        }

        public string Format(string key, params object?[] args)
        {
            return string.Format(Get(key), args);
        }
    }

    private sealed class FakeDocumentStorageService : IDocumentStorageService
    {
        private readonly Queue<Func<CancellationToken, Task<IReadOnlyList<DocumentRecord>>>> loads = new();

        public int GetDocumentsCallCount { get; private set; }

        public int UnexpectedCallCount { get; private set; }

        public void EnqueueDocuments(params DocumentRecord[] documents)
        {
            Enqueue(_ => Task.FromResult<IReadOnlyList<DocumentRecord>>(documents));
        }

        public void Enqueue(Func<CancellationToken, Task<IReadOnlyList<DocumentRecord>>> load)
        {
            loads.Enqueue(load);
        }

        public Task<IReadOnlyList<DocumentRecord>> GetDocumentsAsync(
            CancellationToken cancellationToken = default)
        {
            GetDocumentsCallCount++;
            return loads.Count == 0
                ? Task.FromResult<IReadOnlyList<DocumentRecord>>([])
                : loads.Dequeue()(cancellationToken);
        }

        public Task<DocumentRecord?> GetDocumentByIdAsync(
            string documentId,
            CancellationToken cancellationToken = default)
        {
            return UnexpectedCall<DocumentRecord?>();
        }

        public Task<DocumentRecord> AddDocumentAsync(
            DocumentDraft draft,
            CancellationToken cancellationToken = default)
        {
            return UnexpectedCall<DocumentRecord>();
        }

        public Task DisableDocumentAsync(
            string documentId,
            DateTimeOffset disabledAt,
            CancellationToken cancellationToken = default)
        {
            return UnexpectedCall();
        }

        public Task<IReadOnlyList<PolicyDocumentRecord>> GetPolicyDocumentsAsync(
            string policyId,
            CancellationToken cancellationToken = default)
        {
            return UnexpectedCall<IReadOnlyList<PolicyDocumentRecord>>();
        }

        public Task<PolicyDocumentRecord> AddPolicyDocumentAsync(
            PolicyDocumentDraft draft,
            CancellationToken cancellationToken = default)
        {
            return UnexpectedCall<PolicyDocumentRecord>();
        }

        public Task DisablePolicyDocumentAsync(
            string policyDocumentId,
            DateTimeOffset disabledAt,
            CancellationToken cancellationToken = default)
        {
            return UnexpectedCall();
        }

        public Task<IReadOnlyList<ClaimDocumentRecord>> GetClaimDocumentsAsync(
            string claimId,
            CancellationToken cancellationToken = default)
        {
            return UnexpectedCall<IReadOnlyList<ClaimDocumentRecord>>();
        }

        public Task<ClaimDocumentRecord> AddClaimDocumentAsync(
            ClaimDocumentDraft draft,
            CancellationToken cancellationToken = default)
        {
            return UnexpectedCall<ClaimDocumentRecord>();
        }

        public Task DisableClaimDocumentAsync(
            string claimDocumentId,
            DateTimeOffset disabledAt,
            CancellationToken cancellationToken = default)
        {
            return UnexpectedCall();
        }

        private Task UnexpectedCall()
        {
            UnexpectedCallCount++;
            return Task.FromException(new InvalidOperationException("Unexpected storage method call."));
        }

        private Task<T> UnexpectedCall<T>()
        {
            UnexpectedCallCount++;
            return Task.FromException<T>(new InvalidOperationException("Unexpected storage method call."));
        }
    }
}
