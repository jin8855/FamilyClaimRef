# Product UI Shell Phase 1D2 Guarded Entry Strategy And Exact File List Decision Candidate

## A. Status

- Marker: `POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1D2_GUARDED_ENTRY_STRATEGY_AND_EXACT_FILE_LIST_DECISION_CANDIDATE_READY`
- Selected recommendation: Candidate B, pure startup-mode selector plus `App.xaml.cs` wiring
- Implementation target now: `0`

## B. Candidate A To G Comparison

| Candidate | Source/test files | Guard visibility and global state | Window count/ownership | Default/failure/shutdown | Management path | Testability/manual smoke | Complexity | Recommendation | Approved now |
|---|---|---|---|---|---|---|---|---|---|
| Candidate A. Inline command-line check | `App.xaml.cs`; no focused selector test | private inline logic; no global state | one App-owned selected Window | MainWindow default; failure contract can be obscured in startup code | retained by default | parser unit test weak; smoke required | low file count, medium coupling | not selected | no |
| Candidate B. Pure selector + wiring | new selector, `App.xaml.cs`, selector tests, result doc | public stateless selector; no global state | one App-owned selected Window | MainWindow default; exception visible; current shutdown model retained | retained by default | focused parser tests plus separate smoke | low | selected | no |
| Candidate C. Environment guard | `App.xaml.cs` and environment-aware tests | hidden process-global state | one selected Window possible | default can be preserved; state leakage risk | retained by default | test isolation weaker; smoke required | medium | excluded | no |
| Candidate D. AppContext switch | `App.xaml.cs` and switch tests | process-global AppContext state | one selected Window possible | invocation convention unclear | retained by default | source convention absent | medium | excluded | no |
| Candidate E. Default ProductShell replacement | `App.xaml.cs` and broad startup validation | no guard | ProductShell only | changes default and product lifecycle | current policy/claim management path lost | broad manual evidence required | high functional risk | excluded | no |
| Candidate F. MainWindow launcher | MainWindow UI/code/ViewModel/resources and launch tests | user action state | commonly two Windows with split ownership | shutdown and re-entry contract required | harness coupled to product shell | broad UI/manual testing | high | excluded | no |
| Candidate G. Dual launch/bootstrapper/runtime service | multiple startup/runtime files and tests | new runtime mode/state | two Windows or new owner | lifetime and shutdown complexity | ambiguous | broad integration/manual testing | excessive | excluded | no |

## C. Selected Guard Contract

### Exact token

- Token: `--product-shell-preview`
- Comparison: `StringComparison.OrdinalIgnoreCase`
- Match unit: one complete command-line argument token
- Prefix match: no
- Substring match: no
- Value assignment such as `--product-shell-preview=true`: no
- Short alias: none
- Environment-variable alias: none
- AppContext alias: none

### Default behavior

- Missing flag: `MainWindow` mode.
- Empty or unknown arguments: `MainWindow` mode.
- Existing MainWindow constructor, `MainWindowViewModel`, assignment, and one `Show` remain semantically unchanged.

### Preview behavior

- One or more exact flags: `ProductShellPreview` mode.
- Duplicate flags do not create multiple Windows.
- `AppServices.CreateDefault()` executes exactly once.
- MainWindow is not constructed.
- `new ProductShellWindow(services.ProductShellViewModel)` constructs the only Window.
- `Application.MainWindow` receives the selected Window.
- The selected Window receives `Show()` exactly once.
- `ShowDialog()` is not used.

### Failure and persistence

- ProductShell construction or `Show` failure is not silently replaced by MainWindow.
- Exceptions are not hidden and no startup failure copy is added.
- Mode is not persisted to config, registry, environment, or the next process.

## D. Selector Visibility Decision

No existing `InternalsVisibleTo` declaration is available. Adding one would require an extra production/project visibility change outside the preferred candidate. The selected candidate is therefore a small public, immutable parsing surface in one SDK-included source file.

Planned API shape:

- Public enum: `StartupWindowMode` with `MainWindow` and `ProductShellPreview`.
- Public static class: `StartupWindowModeSelector`.
- Public constant: exact preview argument.
- Public pure method: `Select(IEnumerable<string>? arguments)`.
- No mutable state, environment reads, AppContext reads, WPF Window construction, or service dependency.

Public API growth is the accepted candidate cost, not an implementation approval.

## E. Future `App.xaml.cs` Wiring Contract

1. Preserve the existing `OnStartup(StartupEventArgs e)` signature and `base.OnStartup(e)` call.
2. Call `AppServices.CreateDefault()` exactly once.
3. Pass `e.Args` to the selector.
4. Construct one local `Window` according to the selected mode.
5. MainWindow branch uses the existing MainWindow constructor and `services.MainWindowViewModel` DataContext assignment.
6. Preview branch uses `new ProductShellWindow(services.ProductShellViewModel)` and its existing constructor DataContext contract.
7. Assign the selected Window to `Application.MainWindow`.
8. Call `Show()` exactly once on the selected Window.

Forbidden wiring includes two AppServices calls, constructing both Windows, constructing MainWindow before selection, `ShowDialog`, hidden fallback, environment reads, `Environment.GetCommandLineArgs`, or Window source modification.

## F. Default Startup Versus Guarded Preview

- Default ProductShell startup ready: no.
- Guarded preview is an explicit developer/preview entry only.
- Default MainWindow retains policy/claim management access.
- Preview feasibility does not claim primary product readiness.
- Default-startup functional blockers remain exactly three: policy contract management, claim case management, and fresh-root target creation.

## G. Future Exact Implementation Candidate

| Path | Change | Purpose | Approved now |
|---|---|---|---|
| `app/FamilyClaimRef.App/Startup/StartupWindowModeSelector.cs` | create | Pure mode selection and public enum | no |
| `app/FamilyClaimRef.App/App.xaml.cs` | modify | Select and show exactly one Window | no |
| `tests/FamilyClaimRef.App.Tests/StartupWindowModeSelectorTests.cs` | create | Validate exact token semantics | no |
| `docs/397_POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1D2_GUARDED_RUNTIME_ENTRY_IMPLEMENTATION_RESULT_REVIEW.md` | create | Record future implementation evidence | no |

Candidate counts:

- Production create: `1`
- Production modify: `1`
- Test create: `1`
- Test modify: `0`
- Result document create: `1`
- Total future candidate files: `4`
- Implementation target now: `0`

Not included: `App.xaml`, AppServices, AppServicesTests, MainWindow files, ProductShellWindow files, ViewModels, resources, project/solution/package files, AssemblyInfo, launcher/runtime-mode service, or docs/398 onward.

## H. Blocker Counts

- Source blockers for Candidate B: `0`
- Selector-unit-test blockers: `0`
- App startup wiring testability blockers: `1` (no approved no-Window lifecycle seam)
- Launch-safety/manual-evidence blockers: `1` (default and preview have not been launched under this contract)
- Default-startup functional blockers: `3`

The future four-file candidate remains unapproved until an exact implementation instruction is issued.
