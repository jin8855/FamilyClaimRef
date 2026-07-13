# Product UI Shell Phase 1 Compile-Only Skeleton Decision Scope Plan

## A. Status

PRODUCT_UI_SHELL_PHASE1_COMPILE_ONLY_SKELETON_DECISION_SCOPE_PLAN_ONLY

## B. Marker

POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1_COMPILE_ONLY_SKELETON_DECISION_SCOPE_READY

## C. Task And Baseline

- task ID: `POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1_COMPILE_ONLY_SKELETON_EXACT_SCOPE_DECISION_DOCS_BATCH`
- project: `C:\EtcProject\FamilyClaimRef`
- baseline hash: `f4d9f7697d1124f0caf2727af6a21a143e134b45`
- baseline subject: `feat(familyclaimref): add product shell phase1 ui copy resources`
- initial working tree: clean
- initial staged files: none
- work type: documentation-only exact-scope decision candidate batch

## D. Current Baseline

| Item | Current state |
|---|---|
| `UiStrings.xaml` `Ui.*` resources | 64 |
| `UiTextKeys.cs` `Ui.*` constants | 64 |
| `Ui.Product.*` resources/constants | 8/8 |
| resource/constant missing-orphan mismatch | 0 |
| focused `ResourceUiTextProviderTests` | PASS 35/35 |
| full solution tests | PASS 334/334 |
| ProductShell implementation | absent |
| ProductShellWindow implementation | absent |
| ProductShell targeted tests | absent |
| current `MainWindow` | validation harness |
| current app startup | `MainWindow` with `MainWindowViewModel` |
| storage source of truth | JSON retained |

## E. Whole Phase 1 Target

The final Phase 1 target remains:

- product navigation shell
- Home/dashboard
- Document registration product view
- Document list view

Splitting Phase 1A is phased delivery, not scope deletion. Home, Document registration, and Document list remain in later Phase 1B/1C decisions.

## F. Phase 1A Candidate Scope

- compile-only `ProductShellWindow`
- product navigation state owned by a separate `ProductShellViewModel`
- separate navigation item state candidate
- no runtime entry
- no `MainWindow` replacement
- no App startup change
- no validation harness command/button
- no actual product view implementation
- no business workflow wiring
- no AppServices modification
- no resource or project-file modification

## G. In Scope For This Decision Batch

- latest source dependency reconciliation
- prior candidate reclassification
- Phase 1A exact file list candidate
- future build and test gates
- explicit approval boundary

## H. Out Of Scope

- source/test/XAML/ViewModel/resource/project implementation
- ProductShell runtime composition
- Home/dashboard implementation
- Document registration product view implementation
- Document list implementation or data source
- `DocumentRegistrationViewModel` behavior change
- AppServices/App/MainWindow modification
- DB/SQLite/repository/OCR/migration
- app launch/manual workflow/visual automation
- cleanup

## I. Explicit Approval Matrix

| Approval item | Approved now |
|---|---|
| ProductShell implementation | no |
| ProductShellWindow creation | no |
| ProductShellViewModel creation | no |
| ProductNavigationItemViewModel creation | no |
| Product view creation | no |
| AppServices modification | no |
| MainWindow replacement | no |
| App startup change | no |
| exact implementation file list | no |

Implementation target now count: 0.

## J. Protection And Execution State

- `data/claimdoc`: protected; internal access not performed
- `docs/nightwork_*`: protected; internal access not performed
- docs/346 created: no
- build/test: not run
- git add/stage: not run
- commit/push: not run

## K. Scope Judgment

Phase 1A compile-only skeleton is a supported future delivery candidate. It is not approved for implementation by this document.
