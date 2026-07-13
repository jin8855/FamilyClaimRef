# Product UI Shell Phase 1 Compile-Only Skeleton Exact File List Decision Candidate

## A. Status

PRODUCT_UI_SHELL_PHASE1_COMPILE_ONLY_SKELETON_EXACT_FILE_LIST_DECISION_CANDIDATE_ONLY

## B. Marker

POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1_COMPILE_ONLY_SKELETON_EXACT_FILE_LIST_DECISION_CANDIDATE_READY

## C. Baseline

- hash: `f4d9f7697d1124f0caf2727af6a21a143e134b45`
- subject: `feat(familyclaimref): add product shell phase1 ui copy resources`
- implementation target now: 0

## D. Prior Candidate Reconciliation

| Prior candidate | Current classification | Reason |
|---|---|---|
| `ProductShell/ProductShellWindow.xaml` | Phase 1A include candidate | compile-only shell layout; default WPF inclusion supported |
| `ProductShell/ProductShellWindow.xaml.cs` | Phase 1A include candidate | minimal constructor/DataContext boundary only |
| `ViewModels/ProductShellViewModel.cs` | Phase 1A include candidate | shell and selected-navigation state owner |
| `ViewModels/ProductNavigationItemViewModel.cs` | Phase 1A include candidate | stable navigation item state/test boundary |
| `Views/ProductHomeView.xaml/.xaml.cs` | Phase 1B deferred | actual product view is outside compile-only skeleton |
| `Views/ProductDocumentRegistrationView.xaml/.xaml.cs` | Phase 1B deferred | ViewModel reuse and workflow wiring require a later decision |
| `Views/ProductDocumentListView.xaml/.xaml.cs` | Phase 1C deferred | list data-source boundary requires later planning |
| `Composition/AppServices.cs` | Explicitly excluded | no runtime entry or composition in Phase 1A |
| `Resources/UiStrings.xaml` | Already completed / no change | 64 keys and approved product copy already implemented |
| `Services/Localization/UiTextKeys.cs` | Already completed / no change | 64 constants and 8 product constants already implemented |
| `ResourceUiTextProviderTests.cs` | Already completed / no change | resource contract already validated 35/35 |
| `ProductShellViewModelTests.cs` | Phase 1A include candidate | shell/navigation selection state tests |
| `ProductNavigationItemViewModelTests.cs` | Phase 1A include candidate | navigation item constructor/state tests |
| App/App startup/MainWindow | Explicitly excluded | validation harness remains the only runtime entry |
| project files | Explicitly excluded | SDK-style WPF default inclusion removes the need |

## E. Recommended Phase 1A Exact File List Candidate

| File | Candidate action | Role | Approved now |
|---|---|---|---|
| `app/FamilyClaimRef.App/ProductShell/ProductShellWindow.xaml` | create candidate | compile-only shell layout using approved resources | no |
| `app/FamilyClaimRef.App/ProductShell/ProductShellWindow.xaml.cs` | create candidate | minimal constructor and DataContext assignment | no |
| `app/FamilyClaimRef.App/ViewModels/ProductShellViewModel.cs` | create candidate | shell and navigation state owner | no |
| `app/FamilyClaimRef.App/ViewModels/ProductNavigationItemViewModel.cs` | create candidate | navigation item state | no |
| `tests/FamilyClaimRef.App.Tests/ProductShellViewModelTests.cs` | create candidate | shell/navigation state tests | no |
| `tests/FamilyClaimRef.App.Tests/ProductNavigationItemViewModelTests.cs` | create candidate | navigation item tests | no |
| `docs/346_POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1_COMPILE_ONLY_SKELETON_IMPLEMENTATION_RESULT_REVIEW.md` | create candidate | implementation result review | no |

## F. Candidate Counts

| Count item | Value |
|---|---:|
| production create candidates | 4 |
| test create candidates | 2 |
| result document candidate | 1 |
| existing modified file candidates | 0 |
| total candidate files | 7 |
| implementation target now | 0 |

## G. Deferred And Excluded

Deferred:

- ProductHomeView
- ProductDocumentRegistrationView
- ProductDocumentListView
- document registration workflow wiring
- document list data source
- runtime composition and entry

Excluded from Phase 1A:

- AppServices, App, MainWindow, resource, project-file modifications
- DB/SQLite/repository/OCR/migration
- direct Korean literals in new XAML/source

## H. Decision Boundary

This exact list is a candidate, not an approval. A separate implementation directive must explicitly approve all seven paths or provide a revised exact list.
