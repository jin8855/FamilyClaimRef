# Policy Claim Product UI Shell Phase 1C Document List Architecture, Copy, And Exact File List Decision Candidate

## A. Status Marker

`POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1C_DOCUMENT_LIST_ARCHITECTURE_COPY_AND_EXACT_FILE_LIST_DECISION_CANDIDATE_READY`

## B. Architecture Candidate Comparison

| Candidate | Ownership and dependency | DataContext/load lifecycle | Raw-data risk | Testability | Complexity | Decision |
|---|---|---|---|---|---|---|
| A. Dedicated list ViewModel over existing interface | `ProductDocumentListViewModel` receives `IDocumentStorageService` and `IUiTextProvider`; item projection owns UI-safe fields | Shell exposes list VM; view Loaded forwards to `LoadAsync()` | Low | High | Medium | Selected candidate |
| B. ProductShellViewModel owns document collection | Shell also owns storage reads, projection, load/error state, and navigation | Selection and data lifecycle become coupled | Medium | Medium | Medium | Rejected |
| C. Bind `DocumentRecord` directly | View receives domain records and XAML selects fields | Formatting and privacy decisions leak into XAML | High | Medium | Low initially | Rejected |
| D. Reuse validation/management ViewModel | Product list depends on MainWindow validation-harness concerns | ProductShell and validation harness become coupled | Medium | Low | Medium | Rejected |
| E. View/code-behind self-composition | View creates or queries storage directly | Data access and UI lifecycle are inseparable | High | Low | Low initially | Rejected |
| F. New repository/query service | Introduces a new abstraction despite sufficient existing interface | Additional composition and tests required | Low | High | High | Deferred; unnecessary now |

Candidate A preserves the existing storage boundary and gives the product list its own state without expanding ProductShell into a data-access owner.

## C. Display Field Decision

| Field | Source exists | Product display candidate | Mapping required | Approved now |
|---|---|---|---|---|
| Page title | yes, `Ui.Product.DocumentList.Title` | yes | resource lookup | no |
| Empty-state message | yes, `Ui.Product.DocumentList.EmptyMessage` | yes | resource lookup | no |
| `DocumentRecord.DisplayTitle` | yes | yes, only row text in the basic list | item projection | no |
| Document type label | not on `DocumentRecord` | no | policy/claim link join plus seed mapping | no |
| Reference date | not persisted in document or link records | no | storage-model change would be required | no |
| Created timestamp | yes | deferred | formatting and product-copy decision | no |
| Disabled state | yes | filter input only, not visible text | `DisabledAt is null` | no |
| Document ID | yes | forbidden as visible text | none | no |
| Physical/relative path | yes | forbidden | none | no |
| Extension | yes | deferred and not shown | none | no |
| Policy/claim target | separate link source | excluded | join required | no |

## D. Copy And Resource Decision

| Copy | Existing source | Classification | Candidate action | Approved now |
|---|---|---|---|---|
| Page title | `Ui.Product.DocumentList.Title` | reuse approved value | reuse unchanged | no |
| Empty state | `Ui.Product.DocumentList.EmptyMessage` | reuse approved value | reuse unchanged | no |
| Display-title column label | `Ui.Document.DisplayTitleLabel` exists | not required | no column header in minimal list | no |
| Type label | `Ui.Document.TypeLabel` exists | blocked by data source | do not use | no |
| Reference-date label | `Ui.Document.ReferenceDateLabel` exists | blocked by data source | do not use | no |
| Load-failure message | absent | needs new product key and value approval | candidate `Ui.Product.DocumentList.LoadFailedMessage`; value not approved here | no |

No direct Korean production literal is permitted. Resource changes remain blocked until the load-failure key and value receive explicit approval.

## E. Recommended Future Exact Implementation File List Candidate

| File | Classification | Reason | Approved now |
|---|---|---|---|
| `app/FamilyClaimRef.App/Views/ProductDocumentListView.xaml` | create candidate | read-only title/list/empty/error/loading presentation | no |
| `app/FamilyClaimRef.App/Views/ProductDocumentListView.xaml.cs` | create candidate | thin Loaded forwarding | no |
| `app/FamilyClaimRef.App/ViewModels/ProductDocumentListViewModel.cs` | create candidate | storage load, active filter, projection, replacement state | no |
| `app/FamilyClaimRef.App/ViewModels/ProductDocumentListItemViewModel.cs` | create candidate | `DisplayTitle`-only UI surface | no |
| `app/FamilyClaimRef.App/ViewModels/ProductShellViewModel.cs` | modify candidate | inject/expose list ViewModel | no |
| `app/FamilyClaimRef.App/ProductShell/ProductShellWindow.xaml` | modify candidate | DocumentList DataTemplate and mapping | no |
| `app/FamilyClaimRef.App/Resources/UiStrings.xaml` | modify candidate, blocked | add approved load-failure value | no |
| `app/FamilyClaimRef.App/Services/Localization/UiTextKeys.cs` | modify candidate, blocked | add approved load-failure key constant | no |
| `tests/FamilyClaimRef.App.Tests/ProductDocumentListViewModelTests.cs` | create candidate | load/projection/refresh/error-state tests | no |
| `tests/FamilyClaimRef.App.Tests/ProductShellViewModelTests.cs` | modify candidate | constructor/property/navigation compatibility | no |
| `tests/FamilyClaimRef.App.Tests/Services/Localization/ResourceUiTextProviderTests.cs` | modify candidate, blocked | resource inventory and exact copy coverage | no |
| `docs/380_POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1C_DOCUMENT_LIST_IMPLEMENTATION_RESULT_REVIEW.md` | create candidate | implementation evidence | no |

Not required:

- `IDocumentStorageService.cs`
- `JsonDocumentStorageService.cs`
- `JsonDocumentStorageServiceTests.cs`
- `DocumentTypeSeeds.cs`
- `ProductShellWindow.xaml.cs`
- `AppServices.cs`
- `App.xaml` / `App.xaml.cs`
- project or solution files

## F. Candidate Counts

| Category | Unique file count |
|---|---:|
| Production create | 4 |
| Production modify, including resource/constant files | 4 |
| Test create | 1 |
| Test modify | 2 |
| Storage modify | 0 |
| Result document create | 1 |
| Total future candidate files | 12 |

Cross-cutting resource-modification count: `2`, already included in production modify.

## G. Blocker Counts

- Source blockers: `2` for richer type/reference-date display.
- Lifecycle blockers: `0` for the selected every-activation load candidate.
- Copy blockers: `1` for the load-failure message.
- Composition blockers: `1` for explicit list ViewModel injection and template-mapping approval.
- Total recorded blockers: `4`.

The basic `DisplayTitle` list is technically feasible without storage changes, but the future exact list is not approved now. Implementation target now: `0`.
