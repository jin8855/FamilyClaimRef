using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using FamilyClaimRef.App.Models.Storage;
using FamilyClaimRef.App.Services.Localization;
using FamilyClaimRef.App.Services.Storage;

namespace FamilyClaimRef.App.ViewModels;

public sealed class ProductDocumentListViewModel : INotifyPropertyChanged
{
    private readonly IDocumentStorageService documentStorageService;
    private ReadOnlyCollection<ProductDocumentListItemViewModel> items = CreateEmptyItems();
    private bool isLoading;
    private bool isEmpty;
    private bool hasLoadError;

    public ProductDocumentListViewModel(
        IDocumentStorageService documentStorageService,
        IUiTextProvider uiTextProvider)
    {
        ArgumentNullException.ThrowIfNull(documentStorageService);
        ArgumentNullException.ThrowIfNull(uiTextProvider);

        this.documentStorageService = documentStorageService;
        Title = uiTextProvider.Get(UiTextKeys.ProductDocumentListTitle);
        EmptyMessage = uiTextProvider.Get(UiTextKeys.ProductDocumentListEmptyMessage);
        LoadFailedMessage = uiTextProvider.Get(UiTextKeys.ProductDocumentListLoadFailedMessage);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public string Title { get; }

    public string EmptyMessage { get; }

    public string LoadFailedMessage { get; }

    public ReadOnlyCollection<ProductDocumentListItemViewModel> Items
    {
        get => items;
        private set => SetProperty(ref items, value);
    }

    public bool IsLoading
    {
        get => isLoading;
        private set => SetProperty(ref isLoading, value);
    }

    public bool IsEmpty
    {
        get => isEmpty;
        private set => SetProperty(ref isEmpty, value);
    }

    public bool HasLoadError
    {
        get => hasLoadError;
        private set => SetProperty(ref hasLoadError, value);
    }

    public async Task LoadAsync(CancellationToken cancellationToken = default)
    {
        EnterLoadingState();

        IReadOnlyList<DocumentRecord> documents;
        try
        {
            documents = await documentStorageService.GetDocumentsAsync(cancellationToken);
        }
        catch (OperationCanceledException)
        {
            SetInitialState();
            throw;
        }
        catch (InvalidOperationException)
        {
            SetLoadErrorState();
            return;
        }
        catch (IOException)
        {
            SetLoadErrorState();
            return;
        }
        catch (UnauthorizedAccessException)
        {
            SetLoadErrorState();
            return;
        }

        ProductDocumentListItemViewModel[] projectedItems;
        try
        {
            projectedItems = documents
                .Where(document => document.DisabledAt is null)
                .Select(document => new ProductDocumentListItemViewModel(document.DisplayTitle))
                .ToArray();
        }
        catch (ArgumentNullException)
        {
            SetLoadErrorState();
            return;
        }
        catch (ArgumentException)
        {
            SetLoadErrorState();
            return;
        }

        Items = new ReadOnlyCollection<ProductDocumentListItemViewModel>(projectedItems.ToList());
        IsLoading = false;
        IsEmpty = Items.Count == 0;
        HasLoadError = false;
    }

    private static ReadOnlyCollection<ProductDocumentListItemViewModel> CreateEmptyItems()
    {
        return new ReadOnlyCollection<ProductDocumentListItemViewModel>(
            new List<ProductDocumentListItemViewModel>());
    }

    private void EnterLoadingState()
    {
        Items = CreateEmptyItems();
        IsLoading = true;
        IsEmpty = false;
        HasLoadError = false;
    }

    private void SetInitialState()
    {
        Items = CreateEmptyItems();
        IsLoading = false;
        IsEmpty = false;
        HasLoadError = false;
    }

    private void SetLoadErrorState()
    {
        Items = CreateEmptyItems();
        IsLoading = false;
        IsEmpty = false;
        HasLoadError = true;
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
