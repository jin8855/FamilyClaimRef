# Product UI Shell Phase 1B2 Document Registration ViewModel Composition Reconciliation

## A. Status

- Marker: `POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1B2_DOCUMENT_REGISTRATION_VIEWMODEL_COMPOSITION_RECONCILIATION_READY`
- Baseline: `e79b4f8489f7c066abd0025fa856ce16bba8a6f5`
- Work type: read-only source and architecture reconciliation

## B. Source Evidence Matrix

| Area | Actual evidence | Phase 1B2 implication | Status |
|---|---|---|---|
| Product content host | `ContentControl.Content` is `SelectedNavigationItem`; a XAML trigger maps only `Home` to `ProductHomeView`. | A registration branch can extend the existing host, but state must be supplied separately from the navigation item. | Source-confirmed |
| ProductShell DataContext | `ProductShellWindow` constructor receives `ProductShellViewModel` and assigns it as DataContext. | Registration state should be exposed through the shell DataContext or a separately approved composition boundary. | Source-confirmed |
| ProductShell public contract | Shell title, navigation items, and selected navigation item only. | No registration state or current-content object exists. | Source-confirmed |
| Home mapping | Title-only, view-only `ProductHomeView`; no Home ViewModel. | The Home-specific string-Id branch is not a canonical stateful page architecture. | Source-confirmed |
| Registration ViewModel constructor | Requires `DocumentRegistrationWorkflow`, `IFilePickerService`, `IPolicyClaimStorageService`, and `IUiTextProvider`. | The existing ViewModel already owns all registration behavior dependencies. | Source-confirmed |
| Registration state | Exposes file, target, metadata, busy, validation, status, target message, and summary state through `INotifyPropertyChanged`. | Product view bindings can use the existing state directly. | Source-confirmed |
| Target loading | `LoadTargetOptionsAsync` reads active policies and claims, clears invalid selections, and refreshes the empty-state message. | Product view activation needs an explicit load lifecycle. | Source-confirmed |
| File selection | `SelectFileAsync` delegates to `IFilePickerService`; cancel preserves prior state. | Product view should call the ViewModel method and must not create `OpenFileDialog` directly. | Source-confirmed |
| Registration | `RegisterAsync` validates, calls `DocumentRegistrationWorkflow`, handles success/failure/cleanup status, and resets `IsBusy`. | Product view must not duplicate validation or call storage/coordinators directly. | Source-confirmed |
| Workflow boundary | `DocumentRegistrationWorkflow` owns attach, link, and rollback coordination. | Workflow bypass is prohibited. | Source-confirmed |
| Current interaction convention | `MainWindow.xaml.cs` forwards `Loaded`, file-select click, and register click to async ViewModel methods. | A narrow Product view code-behind forwarding candidate matches the current convention. | Source-confirmed |
| Command convention | No `ICommand`, `RelayCommand`, or `AsyncCommand` exists in app/tests. | Introducing command infrastructure solely for Phase 1B2 would be a new architecture decision. | Source-confirmed |
| MainWindowViewModel | Combines registration and policy/claim management and delegates all window actions. | Reusing it would couple ProductShell to the validation harness. | Source-confirmed |
| AppServices | Creates the existing registration ViewModel and MainWindowViewModel only. | Compile-only ProductShell work can leave AppServices unchanged; a runtime entry would require later composition. | Source-confirmed |
| Project inclusion | SDK-style WPF project uses default XAML/C# inclusion. | A future view pair does not require `.csproj` modification. | Source-confirmed |
| Runtime entry | No ProductShell reference exists in App/AppServices/MainWindow startup paths. | ProductShell remains compile-only after any separately approved Phase 1B2 implementation. | Source-confirmed |
| Existing tests | Registration behavior tests and shell navigation tests exist; visual/XAML automation convention does not. | Build validates XAML; targeted ViewModel tests remain the behavior gate. | Source-confirmed |

## C. Architecture Candidate Comparison

| Candidate | Core shape | DataContext ownership | Interaction/lifecycle | Exact impact direction | Main risks | Recommendation |
|---|---|---|---|---|---|---|
| A. Direct reuse through `ProductShellViewModel` | Shell receives and exposes existing `DocumentRegistrationViewModel`; registration template binds to it. | Shell owns a read-only reference; Product view receives that reference. | View code-behind forwards Loaded/select/register. | New view pair; shell XAML and shell VM changes; shell VM tests change. | Constructor/test fixture expansion; load re-entry and copy ownership still need approval. | Candidate supported, preferred conditionally |
| B. Product wrapper ViewModel | New wrapper delegates to existing registration ViewModel. | Wrapper owns forwarding state. | Wrapper may add commands and lifecycle. | Adds wrapper and wrapper tests in addition to Candidate A files. | Duplicate notification/state surface and behavior drift; no source need for wrapper. | Deferred, not selected |
| C. General current-content contract | Shell exposes typed current page content and uses type-based templates. | Shell/router owns page content objects. | Page activation contract would be new. | Modifies Home and shell architecture; adds page abstractions/tests. | Over-scoped for one stateful page; string-Id replacement becomes a broader migration. | Deferred |
| D. View self-composition | View creates workflow, picker, storage, or ViewModel. | View owns services. | View directly creates dependencies. | View code-behind and composition code expand. | Breaks composition ownership and testability; risks workflow/service duplication. | Excluded |
| E. Reuse `MainWindowViewModel` | Product shell receives harness aggregate ViewModel. | Validation-harness aggregate owns page state. | Reuses window-level methods. | Shell couples to management state and harness lifecycle. | Product/harness boundary violation. | Excluded |
| F. Router/content service | New router/factory resolves stateful page content. | New service owns navigation and content. | New navigation lifecycle contract. | New production service/interface/tests and composition. | No runtime entry or second stateful page justifies it yet. | Deferred |

## D. Selected Architecture Candidate

Selected candidate: **Candidate A, direct reuse through `ProductShellViewModel`, conditional on separate approvals**.

Reasons:

- It reuses the existing behavior-complete `DocumentRegistrationViewModel` and `DocumentRegistrationWorkflow`.
- It does not duplicate file picker, storage, validation, rollback, or message state.
- It follows the current event-forwarding convention without inventing command infrastructure.
- It preserves `MainWindow` as a validation harness and keeps ProductShell compile-only.
- It does not require `ProductDocumentRegistrationViewModel`, router, service locator, or AppServices changes for compile-only scope.

This is an architecture recommendation, not implementation approval.

## E. Direct Answers

| Question | Judgment | Status |
|---|---|---|
| Selected architecture candidate | Candidate A | Candidate supported |
| Existing `DocumentRegistrationViewModel` direct reuse | yes, conditionally | Candidate supported |
| Wrapper required | no | Not required |
| `ProductShellViewModel` modification | yes; constructor dependency plus read-only registration property | Needs explicit composition approval |
| `ProductShellWindow.xaml` modification | yes; registration content template/trigger | Candidate supported |
| `ProductShellWindow.xaml.cs` modification | no | Not required |
| Product view code-behind interaction | yes; event forwarding only | Needs explicit lifecycle approval |
| `DocumentRegistrationViewModel` production modification | no for Candidate A behavior reuse | Not required |
| `MainWindowViewModel` reuse | no | Not required |
| `AppServices` modification | no for compile-only scope | Not required |
| Runtime-entry dependency | none for compile-only scope | Deferred |
| New command infrastructure | no | Not required |
| Product wrapper ViewModel | no | Not required |

## F. Interaction Contract Candidate

| View event | Candidate call | Boundary |
|---|---|---|
| `Loaded` | `DocumentRegistrationViewModel.LoadTargetOptionsAsync()` | Read active target options only; no workflow execution. |
| Select file click | `DocumentRegistrationViewModel.SelectFileAsync()` | Reuse `IFilePickerService`; view does not instantiate a picker. |
| Register click | `DocumentRegistrationViewModel.RegisterAsync()` | Reuse ViewModel validation and workflow; view does not call storage/coordinators. |

The code-behind candidate may only forward events to the bound ViewModel. It must not create services, normalize business data, or translate workflow results.

## G. Lifecycle Reconciliation

- Current `MainWindow` loads registration target options once from the window `Loaded` event through `MainWindowViewModel.LoadAsync`.
- A stateful Product view can be unloaded and loaded again when navigation changes.
- `LoadTargetOptionsAsync` performs repeatable reads and selection reconciliation, but no test currently defines repeated view activation, overlapping calls, or an error surface for load failure.
- No async page lifecycle interface or navigation command convention exists.
- A one-load-per-view-instance guard is a narrow candidate, but refresh-on-reentry semantics remain a product behavior decision.

Lifecycle blocker count: **1**.

Blocker: approve whether target options load once per view instance or on every registration-page activation, and require a regression test for the chosen rule.

## H. Async Error Boundary

- `RegisterAsync` converts workflow exceptions into status messages.
- `LoadTargetOptionsAsync` and `SelectFileAsync` do not add a general catch boundary.
- Current `async void` window event forwarding relies on the called ViewModel behavior.
- Phase 1B2 must not add silent catches or new user copy without a separate decision.

## I. Composition Reconciliation

- Candidate A adds a constructor dependency to `ProductShellViewModel` and invalidates the current test assertion that its only dependency is `IUiTextProvider`.
- `ProductShellViewModelTests` therefore becomes a required modification candidate.
- No production caller currently constructs ProductShell, so AppServices can remain unchanged for compile-only validation.
- A later runtime entry must separately decide whether AppServices exposes a shared registration ViewModel instance and constructs ProductShell.

Composition blocker count: **1**.

Blocker: approve ProductShellViewModel as the owner of the existing registration ViewModel reference for compile-only Phase 1B2.

## J. Blocker Summary

| Blocker family | Count | Judgment |
|---|---:|---|
| Source blockers | 0 | Existing source is sufficient to select Candidate A conditionally. |
| Lifecycle blockers | 1 | Target option load/re-entry rule is not approved. |
| Composition blockers | 1 | Shell ownership/injection change is not approved. |
| Copy/resource blockers | 2 | Recorded in docs/355. |

Implementation readiness: `BLOCKED_PENDING_COPY_RESOURCE_LIFECYCLE_AND_COMPOSITION_APPROVAL`.
