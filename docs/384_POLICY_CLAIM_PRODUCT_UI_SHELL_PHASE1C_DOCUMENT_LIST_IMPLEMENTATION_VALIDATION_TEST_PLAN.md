# Product UI Shell Phase 1C Document List Implementation Validation Test Plan

## A. Status Marker

`POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1C_DOCUMENT_LIST_IMPLEMENTATION_VALIDATION_TEST_PLAN_READY`

## B. Current Evidence Baseline

- Latest known full solution tests: PASS `358/358`.
- Current `ProductShellViewModelTests`: `11` discovered Fact cases.
- Current `JsonDocumentStorageServiceTests`: `20` discovered Fact cases.
- Current `ResourceUiTextProviderTests`: `39` discovered cases, consisting of `11` Facts and `28` InlineData cases.
- Current resources/constants: `67/67`.
- Current `Ui.Product.*` resources/constants: `11/11`.
- No command in this plan is run during the documentation batch.

## C. Future Command Order

Run only after a separate exact implementation approval:

```powershell
dotnet build FamilyClaimRef.sln
dotnet test FamilyClaimRef.sln --filter FullyQualifiedName~ProductDocumentListViewModelTests
dotnet test FamilyClaimRef.sln --filter FullyQualifiedName~ProductShellViewModelTests
dotnet test FamilyClaimRef.sln --filter FullyQualifiedName~JsonDocumentStorageServiceTests
dotnet test FamilyClaimRef.sln --filter FullyQualifiedName~ResourceUiTextProviderTests
dotnet test FamilyClaimRef.sln
```

## D. ProductDocumentListViewModelTests Inventory

The new test file includes item and list contracts. No separate item test file is created.

| # | Planned test | Form | Discovered cases |
|---:|---|---|---:|
| 1 | Item constructor rejects null, empty, and whitespace display titles | Theory with 3 InlineData rows | 3 |
| 2 | Item exposes an immutable `DisplayTitle`-only public data surface | Fact | 1 |
| 3 | List constructor rejects null document storage | Fact | 1 |
| 4 | List constructor rejects null UI text provider | Fact | 1 |
| 5 | Constructor resolves three copies and exposes the initial exclusive state | Fact | 1 |
| 6 | Empty source produces successful empty state | Fact | 1 |
| 7 | Load filters disabled rows, projects titles only, and preserves source order | Fact | 1 |
| 8 | Loading clears prior items and keeps state flags exclusive | Fact | 1 |
| 9 | Sequential reload replaces the snapshot without duplicates | Fact | 1 |
| 10 | `InvalidOperationException` becomes load error without raw detail | Fact | 1 |
| 11 | `IOException` becomes load error | Fact | 1 |
| 12 | `UnauthorizedAccessException` becomes load error | Fact | 1 |
| 13 | Invalid projected title becomes load error at the projection boundary | Fact | 1 |
| 14 | Successful reload after failure clears error state | Fact | 1 |
| 15 | Cancellation propagates and is not converted to load error | Fact | 1 |
| 16 | Items exposes a read-only replacement snapshot | Fact | 1 |
|  | Total | 15 Facts plus 1 Theory with 3 rows | 18 |

## E. Existing Target Test Changes

`ProductShellViewModelTests`:

- Add null-document-list constructor guard: `+1` case.
- Update the existing constructor-signature assertion: `+0` cases.
- Add injected `DocumentList` identity assertion: `+1` case.
- Expected targeted count: `11 + 2 = 13`.

`ResourceUiTextProviderTests`:

- Update count assertions from `67/67` and `11/11` to `68/68` and `12/12`.
- Add the approved load-failure key/value to the existing product resource map.
- Add one InlineData row to approved Korean copy coverage: `+1` case.
- Expected targeted count: `39 + 1 = 40`.

`JsonDocumentStorageServiceTests`:

- Source and tests remain unchanged.
- Run as a regression gate.
- Expected targeted count remains `20`.

## F. Expected Future Test Accounting

| Target | Current | Added | Expected future |
|---|---:|---:|---:|
| `ProductDocumentListViewModelTests` | 0 | 18 | 18 |
| `ProductShellViewModelTests` | 11 | 2 | 13 |
| `JsonDocumentStorageServiceTests` | 20 | 0 | 20 |
| `ResourceUiTextProviderTests` | 39 | 1 | 40 |
| Full solution | 358 | 21 | 379 |

The expected full count is arithmetic from the exact planned cases: `358 + 18 + 2 + 1 = 379`. It is not execution evidence. The implementation batch must record the actual discovered and passing counts and stop if discovery differs.

## G. XAML, State, And Privacy Gates

- SDK-style WPF compilation succeeds without project-file changes.
- Home and DocumentRegistration mappings remain unchanged.
- DocumentList maps to `ProductDocumentListView` and `ProductShellViewModel.DocumentList` only after approval.
- Loaded code-behind only forwards to `LoadAsync`.
- Initial, loading, non-empty, empty, and error presentations are mutually exclusive.
- Every activation reloads; successful retry clears failure; repeated load does not duplicate rows.
- No raw ID, physical file name, path, extension, timestamp, type code, reference date, or link data is exposed.
- No detail, open, edit, delete, disable, unlink, search, filter, sort, refresh, or bulk-selection control is added.

## H. Resource, Storage, And Runtime Gates

- Resources/constants: `68/68` with no duplicate, missing, orphan, or mismatch.
- `Ui.Product.*`: `12/12`.
- Exactly one new resource and constant: `Ui.Product.DocumentList.LoadFailedMessage` / `ProductDocumentListLoadFailedMessage`.
- Existing resource values changed: `0`.
- Direct Korean production literals outside `UiStrings.xaml`: `0`.
- `IDocumentStorageService`, `JsonDocumentStorageService`, storage models, and storage tests remain unchanged.
- JSON remains the source of truth.
- `AppServices`, App startup, MainWindow, ProductShellWindow code-behind, and runtime entry remain unchanged.
- App launch, manual workflow, screenshot automation, cleanup, and protected local data access are not validation steps.

## I. Current Batch Execution

- Build: not run.
- Targeted tests: not run.
- Full tests: not run.
- Reason: documentation-only implementation-contract batch.
