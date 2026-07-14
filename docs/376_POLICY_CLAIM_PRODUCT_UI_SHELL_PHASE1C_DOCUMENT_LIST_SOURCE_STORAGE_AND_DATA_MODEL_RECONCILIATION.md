# Policy Claim Product UI Shell Phase 1C Document List Source, Storage, And Data Model Reconciliation

## A. Status Marker

`POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1C_DOCUMENT_LIST_SOURCE_STORAGE_AND_DATA_MODEL_RECONCILIATION_READY`

## B. Source Evidence Matrix

| Area | Actual source evidence | Phase 1C implication | Status |
|---|---|---|---|
| Document list contract | `IDocumentStorageService.GetDocumentsAsync()` returns `IReadOnlyList<DocumentRecord>` | Existing interface is sufficient for a basic list | Confirmed |
| JSON implementation | `JsonDocumentStorageService.GetDocumentsAsync()` returns `documentStore.LoadAsync(...).Items` | ViewModel can depend on the interface; concrete JSON dependency is unnecessary | Confirmed |
| JSON source of truth | Current decision track retains JSON and existing storage services | No repository or DB boundary is introduced | Confirmed |
| Document fields | `DocumentRecord` has `Id`, `PhysicalFileName`, `DisplayTitle`, `Extension`, `RelativePath`, `CreatedAt`, `UpdatedAt`, and `DisabledAt` | Only UI-safe fields may be projected | Confirmed |
| Disabled behavior | `GetDocumentsAsync()` returns disabled records; `DisabledAt` is the persisted disable source of truth | Candidate list filters `DisabledAt is null` in the list ViewModel without changing storage | Candidate |
| Document type | `DocumentType` exists on `PolicyDocumentRecord` and `ClaimDocumentRecord`, not `DocumentRecord` | Type display requires a link join and is excluded from the basic list | Blocked for Phase 1C display |
| Reference date | `ReferenceDate` exists in registration requests and file-name generation input but is not persisted on `DocumentRecord` or link records | Reference-date display is excluded | Blocked for Phase 1C display |
| Type labels | `DocumentTypeSeeds` maps link type codes to labels by policy/claim scope | Mapping cannot be applied without first selecting and joining a link record | Deferred |
| Link boundary | Policy and claim links are separate records and separate storage reads | Basic list does not join policy/claim link data | Confirmed exclusion |
| ProductShell navigation | `Home`, `DocumentRegistration`, and `DocumentList` navigation items exist | No new navigation item is needed | Confirmed |
| Current content mapping | Home and registration have templates; DocumentList uses fallback text | Future implementation needs one list template mapping | Confirmed |
| Shell ownership | `ProductShellViewModel` owns selection and currently exposes only `DocumentRegistration` content state | Future list ViewModel should be injected and exposed as a separate property | Candidate |
| Existing view lifecycle | `ProductDocumentRegistrationView.Loaded` forwards to its ViewModel load method | List view may use the same thin Loaded-forwarding pattern | Confirmed convention |
| Storage tests | `JsonDocumentStorageServiceTests` covers missing-file empty list, persistence, disable state, and invalid JSON/schema failures | Existing storage behavior should remain a regression gate | Confirmed |
| ProductShell tests | `ProductShellViewModelTests` fixes constructor dependencies, navigation order, and selection behavior | Future constructor/property changes require focused updates | Confirmed |
| App composition | `AppServices` creates MainWindow validation-harness dependencies only | Compile-only list candidate does not require AppServices modification | Confirmed |
| Runtime entry | ProductShellWindow appears only in its own XAML/code-behind definition | Runtime entry remains absent | Confirmed |
| WPF inclusion | SDK-style project with `UseWPF=true` and no explicit item list | New `.cs` and XAML files do not require project-file edits | Confirmed |
| Existing copy | `Ui.Product.DocumentList.Title` and `Ui.Product.DocumentList.EmptyMessage` exist | Title and empty state can be reused | Confirmed |

## C. Selected Data Source Candidate

- Selected source: `IDocumentStorageService.GetDocumentsAsync()`.
- Concrete `JsonDocumentStorageService` dependency: not required.
- Storage interface modification: not required.
- Storage implementation modification: not required.
- Repository/query service: not required.
- Policy/claim link reads: excluded from the basic list.

## D. Selected Projection Boundary

- Dedicated `ProductDocumentListViewModel`: required as a future candidate.
- Dedicated `ProductDocumentListItemViewModel`: required as a future candidate.
- Item display surface: `DisplayTitle` only.
- Internal raw model binding: rejected.
- Raw document ID, physical file name, extension, relative path, and timestamps: not displayed.
- Document type and reference date: not projected because the selected source does not contain them.

The item projection prevents future XAML changes from accidentally exposing local paths or technical identifiers. The domain record remains owned by the storage boundary.

## E. Load And Refresh Candidate

- Load trigger: each `ProductDocumentListView.Loaded` activation.
- View responsibility: forward only to `ProductDocumentListViewModel.LoadAsync()`.
- ViewModel responsibility: call `GetDocumentsAsync()`, filter active records, project safe items, and replace the visible collection atomically.
- Ordering: preserve storage-return order; no invented date sorting.
- Repeated load: replace rather than append, preventing duplicates.
- Registration-to-list refresh: navigating to DocumentList after registration triggers the list view Loaded path and a fresh read.
- Explicit refresh command: not required for the basic candidate.

## F. Error And Empty-State Boundary

- Missing JSON file already produces an empty list.
- Empty successful load uses the approved product empty-state resource.
- Invalid JSON/schema can throw `InvalidOperationException` from storage.
- A product-specific load-failure copy is not currently approved.
- Silent failure or reuse of registration-specific error copy is rejected.
- Load-failure resource approval is therefore a copy blocker before implementation.

## G. Reconciliation Result

| Decision | Result |
|---|---|
| Selected architecture | Candidate A, dedicated list ViewModel over existing storage interface |
| Selected data source | `IDocumentStorageService.GetDocumentsAsync()` |
| Dedicated list ViewModel required | yes, future candidate only |
| Item projection required | yes, future candidate only |
| Storage interface modification required | no |
| Concrete storage dependency required | no |
| ProductShellViewModel modification required | yes, future candidate |
| ProductShellWindow modification required | yes, future candidate |
| View code-behind lifecycle forwarding required | yes, future candidate |
| AppServices modification required | no |
| Source blockers | 2: document type and reference date are unavailable in the selected record |
| Lifecycle blockers | 0 after selecting every-activation replacement load |
| Copy blockers | 1: product list load-failure message |
| Composition blockers | 1: list ViewModel injection/template mapping still needs explicit approval |

Implementation target now remains `0`.
