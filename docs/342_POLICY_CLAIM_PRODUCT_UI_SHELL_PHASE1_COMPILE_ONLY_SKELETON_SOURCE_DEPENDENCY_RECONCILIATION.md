# Product UI Shell Phase 1 Compile-Only Skeleton Source Dependency Reconciliation

## A. Status

PRODUCT_UI_SHELL_PHASE1_COMPILE_ONLY_SKELETON_SOURCE_DEPENDENCY_RECONCILIATION_ONLY

## B. Marker

POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1_COMPILE_ONLY_SKELETON_SOURCE_DEPENDENCY_RECONCILIATION_READY

## C. Baseline

- hash: `f4d9f7697d1124f0caf2727af6a21a143e134b45`
- subject: `feat(familyclaimref): add product shell phase1 ui copy resources`
- purpose: reconcile docs/326 and docs/331 candidates against current tracked source

## D. Source Evidence Matrix

| Area | Actual source evidence | Phase 1A implication | Status |
|---|---|---|---|
| project SDK/XAML inclusion | `Microsoft.NET.Sdk`, `UseWPF=true`, no explicit `Compile`/`Page` includes | new `.cs` and WPF XAML candidates do not require a project-file edit | Source-confirmed |
| ProductShell folder | no tracked path and no current directory | folder and Window files would be new candidates | Source-confirmed |
| Views folder | directory exists but contains no tracked or local items | no established product-view convention is available; views stay deferred | Deferred |
| ViewModels namespace | existing ViewModels use `FamilyClaimRef.App.ViewModels` | both candidate ViewModels can follow the existing namespace | Source-confirmed |
| property-change pattern | two feature ViewModels implement `INotifyPropertyChanged` and local `SetProperty`; no shared base | candidate shell state can follow the direct pattern without a new base class | Candidate supported |
| command pattern | no `ICommand`, `RelayCommand`, or `ObservableObject` convention exists | Phase 1A should use property/state tests and must not invent command infrastructure | Source-confirmed |
| Window/DataContext pattern | `MainWindow` has minimal code-behind; App assigns its DataContext | a future shell may accept an already-created ViewModel and set DataContext without composing services | Candidate supported |
| `IUiTextProvider` | feature ViewModels receive `IUiTextProvider`; approved `Ui.Product.*` keys exist | candidate shell ViewModel may receive the provider; no new copy is required | Candidate supported |
| AppServices composition | JSON services and current ViewModels only; ProductShell references 0 | no AppServices edit is required while there is no runtime entry | Not required |
| MainWindow startup | `App.OnStartup` creates `MainWindow` and assigns `MainWindowViewModel` | startup and validation harness remain untouched | Source-confirmed |
| current resources | resources/constants 64/64; `Ui.Product.*` 8/8; mismatch 0 | resource prerequisite is already completed | Source-confirmed |
| test convention | xUnit, namespace `FamilyClaimRef.App.Tests`, constructor guards and state assertions | two pure ViewModel test candidates fit existing style | Source-confirmed |
| ProductShell implementation | tracked ProductShell paths/classes/tests 0 | all Phase 1A implementation paths would be new | Source-confirmed |
| storage boundary | `JsonDocumentStorageService` and `JsonPolicyClaimStorageService` remain composed | no storage dependency is needed by compile-only navigation state | Source-confirmed |

## E. Required Judgments

1. New Window files can compile without `.csproj` modification: **yes, Source-confirmed**.
2. `ProductShellViewModel` can follow existing ViewModel conventions: **yes, Candidate supported**.
3. A separate `ProductNavigationItemViewModel` is required by source: **no**. It remains recommended as a candidate because it gives each navigation item a stable state/test boundary.
4. `ProductShellWindow` can remain compile-only without AppServices modification: **yes**.
5. Constructor/DataContext candidate: caller-created `ProductShellViewModel` may be accepted by the Window and assigned as DataContext; service construction in code-behind is not recommended.
6. Unresolved runtime dependency under no-runtime-entry: **none**. Runtime composition is intentionally deferred.
7. Two candidate tests align with existing convention: **yes**.
8. MainWindow/App startup can remain untouched: **yes**.

## F. Candidate Dependency Boundary

- `ProductShellViewModel` may depend only on navigation state and `IUiTextProvider`.
- `ProductNavigationItemViewModel` must remain UI state only.
- Product views, registration workflow, document list source, storage services, and AppServices composition are excluded.
- New product XAML must reference approved resources and must not add direct Korean literals.

## G. Reconciliation Result

| Result item | Value |
|---|---:|
| source reconciliation blockers | 0 |
| required existing-file modifications | 0 |
| unresolved composition blockers for compile-only scope | 0 |
| explicit approval still required | yes |

The candidate is source-supported but not implementation-approved.
