# Product UI Shell Phase 1C Document List Composition, Lifecycle, And State Approved Decision

## A. Status Marker

`POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1C_DOCUMENT_LIST_COMPOSITION_LIFECYCLE_AND_STATE_APPROVED_DECISION_READY`

## B. Source Audit

| Evidence | Actual contract | Decision |
|---|---|---|
| `IDocumentStorageService.GetDocumentsAsync` | `Task<IReadOnlyList<DocumentRecord>> GetDocumentsAsync(CancellationToken cancellationToken = default)` | Use the interface directly. |
| `JsonDocumentStorageService.GetDocumentsAsync` | Returns `JsonFileEnvelope<DocumentRecord>.Items` without active filtering | Filter `DisabledAt is null` in the list ViewModel. |
| `DocumentRecord` | Positional sealed record with `Id`, `PhysicalFileName`, `DisplayTitle`, `Extension`, `RelativePath`, `CreatedAt`, `UpdatedAt`, and nullable `DisabledAt` | Project only `DisplayTitle`; do not bind the storage record to XAML. |
| `DocumentRecord` validation | Non-nullable annotations exist, but the record has no explicit runtime value guards | Validate `DisplayTitle` when constructing the UI item. |
| `JsonFileStore.LoadAsync` | Missing file returns an empty envelope | Treat as successful empty state. |
| Invalid JSON | `JsonException` is wrapped as `InvalidOperationException`; an existing test confirms the public type | Map to the approved load-error state. |
| Invalid envelope/schema | Null envelope, unsupported schema, missing `savedAt`, or null `items` throws `InvalidOperationException`; schema/null-items tests exist | Map to the approved load-error state. |
| File access | `File.OpenRead` is outside a file-access catch; `IOException` and `UnauthorizedAccessException` can pass through; no dedicated storage test exists | Catch only these source-path access failures in the list load boundary. |
| Cancellation | The token is forwarded to async JSON deserialization and only `JsonException` is caught | Do not convert `OperationCanceledException` into a load error. |

The approved source-load catch set is `InvalidOperationException`, `IOException`, and `UnauthorizedAccessException`. `JsonException` is not caught by the ViewModel because the storage layer already wraps it. `OperationCanceledException` propagates. An `ArgumentException` raised by the approved item projection is handled only around projection and is classified as invalid row data, not as a storage exception.

## C. ProductDocumentListItemViewModel Contract

Future candidate:

`app/FamilyClaimRef.App/ViewModels/ProductDocumentListItemViewModel.cs`

Exact public data surface:

```csharp
public string DisplayTitle { get; }
```

Constructor meaning:

```csharp
ProductDocumentListItemViewModel(string displayTitle)
```

Contract:

- Reject `null` with `ArgumentNullException`.
- Reject empty or whitespace-only input with `ArgumentException`.
- Preserve the accepted title as immutable UI data.
- Do not implement `INotifyPropertyChanged`.
- Expose no document ID, physical file name, relative path, extension, timestamp, document type, reference date, or policy/claim link.
- Hold no command, service, storage, or domain-record dependency.
- Verify this contract in `ProductDocumentListViewModelTests.cs`; do not create a separate item test file.

## D. ProductDocumentListViewModel Contract

Future candidate:

`app/FamilyClaimRef.App/ViewModels/ProductDocumentListViewModel.cs`

Constructor, following the current dependency naming and ordering convention:

```csharp
ProductDocumentListViewModel(
    IDocumentStorageService documentStorageService,
    IUiTextProvider uiTextProvider)
```

- Both dependencies use explicit null guards.
- No concrete JSON service, mutation service, link storage, repository, `AppServices`, MainWindow, command, or router dependency is allowed.

Approved public state:

```csharp
public string Title { get; }
public string EmptyMessage { get; }
public string LoadFailedMessage { get; }
public ReadOnlyCollection<ProductDocumentListItemViewModel> Items { get; }
public bool IsLoading { get; }
public bool IsEmpty { get; }
public bool HasLoadError { get; }
public Task LoadAsync(CancellationToken cancellationToken = default)
```

- `Title` resolves `UiTextKeys.ProductDocumentListTitle`.
- `EmptyMessage` resolves `UiTextKeys.ProductDocumentListEmptyMessage`.
- `LoadFailedMessage` resolves the future `UiTextKeys.ProductDocumentListLoadFailedMessage`.
- The ViewModel implements `INotifyPropertyChanged` for replacement collection and state changes.
- `Items` is externally read-only and replaced with a new snapshot; it is never appended to in place.
- Each item contains only `DisplayTitle`.

## E. State Transition Contract

| State | `IsLoading` | `IsEmpty` | `HasLoadError` | `Items` |
|---|---:|---:|---:|---|
| Initial | false | false | false | empty |
| Loading | true | false | false | empty; prior snapshot cleared |
| Successful non-empty | false | false | false | active projected snapshot |
| Successful empty | false | true | false | empty |
| Failed | false | false | true | empty |

Rules:

- Loading, empty, and error flags are mutually exclusive.
- Failure never displays the successful empty-state copy.
- Prior items are not displayed beside loading or error state.
- Raw exception details are never exposed.
- Cancellation propagates and returns the visible state to initial semantics: not loading, not empty, not error, items empty.
- A later successful load clears a previous failure state.

## F. LoadAsync Lifecycle

- `ProductDocumentListView.Loaded` forwards to `LoadAsync` on every activation.
- There is no one-time initialized flag, cache, or explicit refresh command.
- Every call obtains the current storage snapshot.
- Before awaiting storage, the ViewModel clears visible items and enters Loading.
- On success, it filters `DisabledAt is null`, preserves source-return order, validates and projects each `DisplayTitle`, and replaces the snapshot.
- Sequential calls replace the prior snapshot and cannot accumulate duplicate rows.
- Navigating to DocumentList after registration therefore performs a fresh read.
- Overlapping/concurrent call semantics, cancellation infrastructure, locks, and concurrency services are not introduced in Phase 1C.

## G. Failure And Recovery Contract

| Failure source | Public exception evidence | ViewModel behavior |
|---|---|---|
| Invalid JSON | `InvalidOperationException`, test-confirmed | Catch at source-load boundary and enter Failed. |
| Invalid schema/envelope | `InvalidOperationException`, source-confirmed and partly test-confirmed | Catch at source-load boundary and enter Failed. |
| File read I/O | `IOException`, direct `File.OpenRead` pass-through; no dedicated test | Catch at source-load boundary and enter Failed. |
| File access denied | `UnauthorizedAccessException`, direct `File.OpenRead` pass-through; no dedicated test | Catch at source-load boundary and enter Failed. |
| Invalid projected title | `ArgumentNullException` or `ArgumentException` from the item constructor | Catch only around projection and enter Failed. |
| Cancellation | `OperationCanceledException` can propagate from the async storage path | Do not catch as a load failure. |

Broad silent catch is prohibited. On a handled failure, clear `Items`, set only `HasLoadError`, and expose only `LoadFailedMessage`. On a subsequent success, replace items and clear the failure state.

## H. ProductShell Composition Contract

Future constructor:

```csharp
ProductShellViewModel(
    IUiTextProvider uiTextProvider,
    DocumentRegistrationViewModel documentRegistration,
    ProductDocumentListViewModel documentList)
```

- Preserve the current first two parameters and append `documentList` last.
- Null-guard the new dependency.
- Add a read-only `DocumentList` property exposing the same injected instance.
- Preserve `DocumentRegistration` identity, navigation count/order/IDs, selection behavior, and Home initial selection.
- Do not inject storage directly into ProductShellViewModel.

Future XAML mapping:

- Home remains mapped to `ProductHomeView`.
- DocumentRegistration remains mapped to `ProductDocumentRegistrationView` and `ProductShellViewModel.DocumentRegistration`.
- DocumentList maps to `ProductDocumentListView` and `ProductShellViewModel.DocumentList`.
- `ProductShellWindow.xaml.cs`, code-behind navigation, `AppServices`, startup, and runtime entry remain unchanged.

## I. Blocker Result

- Basic-list compile-only source/lifecycle/copy/composition blockers after contract: `0/0/0/0`.
- Deferred richer-field constraints retained: `2`.
- Runtime-readiness blocker retained: `1`.
- Compile-only implementation remains a future candidate, not an implementation authorized by this batch.
