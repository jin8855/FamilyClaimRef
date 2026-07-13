# Product UI Shell Phase 1B Home Content Host Exact File List Decision Candidate

## A. Status

- Marker: `POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1B_HOME_CONTENT_HOST_EXACT_FILE_LIST_DECISION_CANDIDATE_READY`
- Baseline: `c53cc53f82413973d0d897e6fa18b2bf95f24730`
- Exact implementation file list approved now: no
- Implementation target now: 0

## B. Architecture Candidate Comparison

| Candidate | Required files | Existing files modified | New production files | Testability | Runtime/AppServices dependency | Resource impact | Future registration/list impact | Complexity | Recommendation |
|---|---|---|---|---|---|---|---|---|---|
| A. ViewModel-driven current content | Home view pair, ProductHomeViewModel, shell ViewModel, shell XAML, tests | ProductShellViewModel, ProductShellWindow | at least 3 | high | none for compile-only | none for title-only | scalable | medium | supported later, not selected for Phase 1B1 |
| B. SelectedNavigationItem-based XAML template switching | Home view pair, shell XAML | ProductShellWindow | 2 | existing selection tests plus XAML build gate | none | none for title-only | templates can extend; string-Id risk grows with complexity | low | selected minimal candidate |
| C. Window code-behind switching | Home view pair, shell XAML/code-behind | ProductShellWindow XAML and code-behind | 2 | low | none | none for title-only | event/view creation logic becomes difficult to extend | medium | not recommended |
| D. Navigation/content router service | Home view pair, router/service, shell integration, tests | shell and composition candidates | at least 3 | high | likely AppServices dependency | none for title-only | scalable but premature | high | deferred |

## C. Selected Candidate

Candidate B is selected as the future exact-scope recommendation because the current shell already owns valid selected-navigation state and the Home slice has no dynamic state. The candidate uses a ContentControl and XAML template/data-trigger mapping for `SelectedNavigationItem.Id == "Home"`.

This selection is not implementation approval. The string-Id XAML branch is acceptable only for the smallest compile-only slice. A ViewModel-driven content contract must be reconsidered before richer registration/list content or navigation lifecycle behavior is added.

## D. Recommended Implementation Candidate Exact File List

| File | Action | Role | Approved now |
|---|---|---|---|
| `app/FamilyClaimRef.App/Views/ProductHomeView.xaml` | create candidate | View-only title landing surface using `Ui.Product.Home.Title` | no |
| `app/FamilyClaimRef.App/Views/ProductHomeView.xaml.cs` | create candidate | Minimal view-only partial class | no |
| `app/FamilyClaimRef.App/ProductShell/ProductShellWindow.xaml` | modify candidate | Replace placeholder with ContentControl/template selection for Home | no |
| `docs/352_POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1B_HOME_CONTENT_HOST_IMPLEMENTATION_RESULT_REVIEW.md` | create candidate | Record future implementation and validation evidence | no |

## E. Considered but Not Required

| File | Classification | Reason |
|---|---|---|
| `app/FamilyClaimRef.App/ViewModels/ProductHomeViewModel.cs` | not required | No dynamic Home state or Home-specific commands are approved. |
| `app/FamilyClaimRef.App/ViewModels/ProductShellViewModel.cs` | not required | Existing selected item and `Home` Id support the minimal switch. |
| `tests/FamilyClaimRef.App.Tests/ProductHomeViewModelTests.cs` | not required | ProductHomeViewModel is not part of the candidate. |
| `tests/FamilyClaimRef.App.Tests/ProductShellViewModelTests.cs` | not required to modify | Existing selection guards remain unchanged and serve as regression tests. |
| `app/FamilyClaimRef.App/ProductShell/ProductShellWindow.xaml.cs` | not required | Code-behind switching is excluded. |
| `app/FamilyClaimRef.App/Composition/AppServices.cs` | not required | No runtime composition or service dependency is introduced. |
| `app/FamilyClaimRef.App/Resources/UiStrings.xaml` | not required | Existing `Ui.Product.Home.Title` is sufficient. |
| `app/FamilyClaimRef.App/Services/Localization/UiTextKeys.cs` | not required | Existing `ProductHomeTitle` constant is sufficient. |
| ProductDocumentRegistrationView files | deferred | Registration product flow is outside Phase 1B1. |
| ProductDocumentListView files | deferred | Document list data and product flow are outside Phase 1B1. |

## F. Candidate Counts

- Production create candidates: 2
- Production modify candidates: 1
- Test create candidates: 0
- Test modify candidates: 0
- Resource modify candidates: 0
- Result document candidate: 1
- Total implementation candidate files: 4
- Implementation target now: 0

## G. Boundary

- This exact list is a candidate, not an approval.
- ProductHomeView, shell modification, and content-host implementation are all approved now: no.
- Runtime entry, MainWindow, App startup, AppServices, and project-file changes are excluded.
- Registration/list views remain deferred.
- Dashboard metrics, counts, amounts, recent activity, alerts, cards, subtitles, calls to action, and empty-state copy must not be invented.
- No additional resource is required for the selected title-only candidate. Any richer copy creates a separate approval gate.
