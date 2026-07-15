# Product UI Shell Phase 1D Runtime Composition Source And Lifetime Reconciliation

## A. Status

- Marker: `POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1D_RUNTIME_COMPOSITION_SOURCE_LIFETIME_RECONCILIATION_READY`
- Baseline: `d57a345f8bd7c46b53de185ea91cf8f164137e43`
- Source inspection mode: read-only

## B. Actual Source Evidence Matrix

| Area | Actual source evidence | Phase 1D implication | Status |
|---|---|---|---|
| App.xaml StartupUri | `App.xaml` has no `StartupUri` | Startup is owned by `App.OnStartup` | Confirmed |
| App.OnStartup | Calls `AppServices.CreateDefault()`, creates `MainWindow`, assigns `services.MainWindowViewModel`, sets `Application.MainWindow`, and calls `Show()` | Current startup remains unchanged in composition-only work | Confirmed |
| MainWindow creation | `App` directly calls `new MainWindow` | App layer owns Window creation | Confirmed |
| MainWindowViewModel creation | `AppServices.Create(...)` creates and exposes `MainWindowViewModel` | AppServices owns ViewModel composition, not Window creation | Confirmed |
| AppServices style | Sealed instance composition root with private constructor and static `CreateDefault`/`Create` factories | Extend the existing returned graph instead of adding a DI container or service locator | Confirmed |
| Service lifetime | Every `Create` call creates new storage, workflow, picker, provider, and ViewModels; instances are shared only inside that call | A ProductShell child graph can share infrastructure inside one AppServices instance | Confirmed |
| DocumentRegistrationViewModel creation | Constructed directly inside `AppServices.Create` from workflow, picker, policy/claim storage, and text provider | The same dependency wiring can create a separate ProductShell registration ViewModel | Confirmed |
| ProductDocumentList dependency | `IDocumentStorageService` and `IUiTextProvider` already exist in the current graph | No new storage or provider abstraction is required | Confirmed |
| ProductShell constructor graph | Requires `IUiTextProvider`, `DocumentRegistrationViewModel`, and `ProductDocumentListViewModel` | Current services satisfy the graph after creating two ProductShell child ViewModels | Confirmed |
| ProductShellWindow constructor | Requires only `ProductShellViewModel`, sets DataContext, and does not launch itself | A Window factory is not needed for composition-only | Confirmed |
| IUiTextProvider Application path | With `Application.Current`, AppServices uses `Application.Current.Resources`; App.xaml merges `UiStrings.xaml` | Runtime Application path can resolve all 68 resources | Confirmed |
| IUiTextProvider fallback path | With no Application, AppServices uses a 24-entry dictionary | Existing registration runtime keys are covered, ProductShell/List keys are not | Gap |
| Product resource fallback coverage | ProductShell requires 4 keys and ProductDocumentList requires 3 keys; all 7 are missing | Future AppServices composition candidate must mirror those 7 values | Gap 7 |
| Storage/workflow/picker sharing | Current graph reuses storage in coordinators/workflow and reuses policy/claim storage across registration and management | Share infrastructure services, not mutable child ViewModels | Confirmed |
| AppServicesTests convention | Tests call `AppServices.Create` with a stub runtime-root provider without creating Application or Window | Fallback coverage and child-graph identity can be tested without app launch | Confirmed |
| ProductShell runtime callers | Tracked callers are tests and ProductShell type declarations only | Runtime caller count remains 0 | Confirmed |
| Shutdown mode/window ownership | No explicit ShutdownMode is set; App sets and shows only MainWindow | Composition-only must create no Window and change no shutdown behavior | Confirmed |

## C. Current Composition Graph

```text
App.OnStartup
  -> AppServices.CreateDefault()
     -> EnvironmentRuntimeRootProvider
     -> JsonDocumentStorageService
     -> JsonPolicyClaimStorageService
     -> LocalFileAttachmentService
     -> DocumentAttachmentCoordinator
     -> DocumentLinkCoordinator
     -> DocumentRegistrationWorkflow
     -> WpfFilePickerService
     -> IUiTextProvider
     -> DocumentRegistrationViewModel
     -> PolicyClaimManagementViewModel
     -> MainWindowViewModel
  -> new MainWindow
  -> DataContext = services.MainWindowViewModel
  -> MainWindow = window
  -> window.Show()
```

`AppServices.Create` does not currently create or expose ProductShell types.

## D. Fallback Resource Audit

ProductShell ViewModel required keys, missing `7/7`:

| Key | UiStrings value | Fallback status |
|---|---|---|
| `Ui.Product.Shell.Title` | `FamilyClaimRef` | missing |
| `Ui.Product.Navigation.Home` | `홈` | missing |
| `Ui.Product.Navigation.DocumentRegistration` | `문서 등록` | missing |
| `Ui.Product.Navigation.DocumentList` | `문서 목록` | missing |
| `Ui.Product.DocumentList.Title` | `문서 목록` | missing |
| `Ui.Product.DocumentList.EmptyMessage` | `등록된 문서가 없습니다.` | missing |
| `Ui.Product.DocumentList.LoadFailedMessage` | `문서 목록을 불러오지 못했습니다.` | missing |

DocumentRegistrationViewModel runtime key coverage:

- Referenced keys: 14.
- Present in AppServices fallback: 14.
- Missing: 0.

The seven future additions are a no-Application fallback mirror. They do not replace `UiStrings.xaml` as the runtime resource source of truth and do not require new `UiTextKeys` constants.

## E. Lifetime Decision

Selected lifetime contract:

- MainWindow and ProductShell use separate `DocumentRegistrationViewModel` instances.
- MainWindow keeps its current `MainWindowViewModel` and `PolicyClaimManagementViewModel`.
- ProductShell receives a separate `DocumentRegistrationViewModel`, `ProductDocumentListViewModel`, and `ProductShellViewModel`.
- One AppServices creation call may share `IDocumentStorageService`, `IPolicyClaimStorageService`, `IFileAttachmentService`, `DocumentRegistrationWorkflow`, `IFilePickerService`, and `IUiTextProvider` across those child graphs.
- Separate AppServices creation calls share no service or ViewModel instances.
- MainWindow and ProductShell are not approved to launch together.

Rejected sharing:

- Sharing the same DocumentRegistrationViewModel would couple file selection, selected target, busy state, validation/status messages, and Loaded lifecycle between Windows.
- Sharing MainWindowViewModel or ProductShellViewModel has no source-supported purpose.

## F. Reconciliation Result

- Composition-only possible: yes.
- ProductShell ViewModel composition boundary candidate: yes.
- ProductShell Window factory candidate: no for composition-only.
- Fallback key changes required: 7.
- MainWindow/ProductShell child ViewModel sharing recommended: no.
- AppServices modification candidate: yes.
- App.xaml.cs modification candidate for composition-only: no.
- Source blockers: 0.
- Fallback-copy gaps: 7.
- Lifetime blockers after separation decision: 0.
- Testability limitation: 1, exact private service identity is not publicly observable without reflection or an expanded API; source wiring review is preferred.

The testability limitation does not block composition-only implementation. It limits direct unit assertions about private infrastructure identity and must not justify a new public service exposure API.
