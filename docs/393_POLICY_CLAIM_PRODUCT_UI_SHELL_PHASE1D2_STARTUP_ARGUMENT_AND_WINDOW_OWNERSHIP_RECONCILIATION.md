# Product UI Shell Phase 1D2 Startup Argument And Window Ownership Reconciliation

## A. Status

- Marker: `POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1D2_STARTUP_ARGUMENT_AND_WINDOW_OWNERSHIP_RECONCILIATION_READY`
- Baseline: `ced4a00f16a55bbe1e76e0b016922983bf1aefd5`
- Review method: tracked source, tests, and committed decision evidence read-only inspection

## B. Source Evidence Matrix

| Area | Actual source evidence | Phase 1D2 implication | Status |
|---|---|---|---|
| `App.xaml` `StartupUri` | `App.xaml` has resources only; `StartupUri` is absent | `App.OnStartup` remains the single startup owner | confirmed |
| `App.OnStartup` signature/base call | `protected override void OnStartup(StartupEventArgs e)` calls `base.OnStartup(e)` first | Existing signature and base call can be preserved | confirmed |
| `StartupEventArgs.Args` | `StartupEventArgs e` is already received; `e.Args` is not currently consumed | Command-line guard can use the supplied argument collection without rereading process state | source-supported |
| `AppServices.CreateDefault` count | Current `OnStartup` calls it exactly once | Future default and preview branches must share one created graph | confirmed |
| MainWindow ownership | `App` constructs `new MainWindow`, assigns `services.MainWindowViewModel`, sets `MainWindow`, and calls `Show()` once | App remains Window owner; default branch can stay equivalent | confirmed |
| `ShutdownMode` | No explicit `ShutdownMode` assignment exists | One selected Window and `Application.MainWindow` assignment preserve the current last-window lifecycle assumption | confirmed, runtime evidence pending |
| `ProductShellWindow` constructor | Public constructor requires `ProductShellViewModel`, rejects null, calls `InitializeComponent`, and assigns `DataContext` | Preview branch only needs `new ProductShellWindow(services.ProductShellViewModel)` | confirmed |
| `AppServices.ProductShellViewModel` | Public read-only property is composed by `AppServices.Create` | No AppServices change is needed for guarded entry | confirmed |
| ProductShell initial navigation | `ProductShellViewModel` selects `NavigationItems[0]`, whose ID is `Home` | Initial ProductShell content is Home | confirmed |
| Initial ProductShell content | Home template creates `ProductHomeView`; its constructor only calls `InitializeComponent` | Initial Home construction has no application storage/workflow call in source | confirmed |
| Registration/list content loading | Registration and list views have `Loaded` handlers, but are selected through non-Home DataTemplates | They are not part of the initial Home branch; navigation runtime evidence remains separate | source-supported, runtime evidence pending |
| Current command-line convention | No `Environment.GetCommandLineArgs` or startup argument parser exists in app/tests | A new pure selector does not conflict with an existing parser | absent |
| Current environment startup convention | Environment variables are used only by `EnvironmentRuntimeRootProvider`, not for Window selection | Runtime-root override must remain distinct from startup-mode selection | no startup convention |
| Current AppContext convention | No `AppContext.TryGetSwitch` or `AppContext.SetSwitch` exists | AppContext guard would introduce new process-global state | absent |
| `InternalsVisibleTo` | No declaration exists in `AssemblyInfo.cs`, app project, or tests | An internal selector is not test-accessible without widening scope | absent |
| SDK source inclusion | App project uses `Microsoft.NET.Sdk` with no explicit `Compile` item list | A new `.cs` file is included by SDK defaults without project-file modification | confirmed |
| Test project access | Test project has a `ProjectReference` to the app; current focused tests instantiate public app types | A public pure selector can be tested without project or AssemblyInfo changes | confirmed |
| Runtime-root provider | `FAMILYCLAIMREF_ENABLE_DEV_RUNTIME_ROOT_OVERRIDE=1` enables absolute `FAMILYCLAIMREF_RUNTIME_ROOT` | A separate isolated absolute runtime root is source-supported for a future smoke batch | confirmed |
| Graph-construction side effects | `AppServices.Create` constructs service/ViewModel objects; focused test verifies the selected runtime root directory is not created | `CreateDefault` graph construction alone is not expected to write runtime files | confirmed by source and test |
| MainWindow load behavior | Constructor only initializes XAML; `Loaded` invokes read-oriented target/management loads | Construction has no mutation; default smoke can still read its selected isolated root | confirmed |
| Current ProductShell runtime caller | No production `new ProductShellWindow`, `Show`, or `ShowDialog` call exists | Guarded runtime entry is genuinely absent | confirmed |

## C. Startup Audit Answers

1. `StartupUri`: absent.
2. Exact signature: `protected override void OnStartup(StartupEventArgs e)`.
3. `base.OnStartup(e)`: yes, before graph construction.
4. `StartupEventArgs.Args`: directly available, currently unused.
5. `AppServices.CreateDefault`: one current call.
6. MainWindow construction: after the AppServices graph is created.
7. MainWindow DataContext: object initializer assigns `services.MainWindowViewModel`.
8. `Application.MainWindow`: `MainWindow = window` in `App.OnStartup`.
9. Show: `window.Show()` exactly once.
10. Explicit `ShutdownMode`: absent.
11. ProductShell constructor parameter: `ProductShellViewModel`.
12. ProductShell DataContext: assigned in its constructor.
13. Initial ProductShell navigation: Home.
14. Home source-side storage/workflow call: none.
15. Existing command-line startup convention: none.
16. Existing environment Window-selection convention: none.
17. Existing AppContext convention: none.
18. Internal app type access from tests: not enabled.
19. Pure selector addition without project change: yes, through SDK default inclusion.
20. Existing no-Window App startup wiring test convention: none; selector-only unit testing is available, Window wiring requires static review and separately approved smoke evidence.

## D. Runtime-Root And Launch-Side-Effect Audit

- Default runtime root: `%LOCALAPPDATA%\FamilyClaimRef` through `EnvironmentRuntimeRootProvider`.
- Isolated root: source-supported only when guard value is exactly `1` and the override is an absolute path.
- `AppServices.CreateDefault`: resolves and composes paths and objects; no directory/file creation is performed by constructors.
- MainWindow construction: no storage mutation; its `Loaded` path performs reads.
- ProductShell initial Home construction: no storage or workflow call.
- Registration/list views: not initial Home content; their future navigation/load behavior is outside startup smoke.
- Explicit `ShutdownMode`: absent; one selected MainWindow assignment is required before `Show`.
- Project-root artifact risk: avoidable by using the existing isolated runtime-root override in a separately approved smoke batch.
- Protected data: not required for preview startup.

## E. Reconciliation Conclusions

- Command-line guard source-supported: yes.
- Exact token source conflict: no.
- Pure selector source-supported: yes.
- Selected selector visibility candidate: public stateless selector and public mode enum in one source file.
- Project-file modification required: no.
- AppServices modification required: no.
- ProductShellWindow modification required: no.
- MainWindow modification required: no.
- Manual isolated smoke source-supported: yes, conditionally through the existing guarded absolute runtime-root override.

## F. Remaining Blockers

- Source blockers: none for the selected command-line selector candidate.
- Testability blockers: no focused App lifecycle/Window factory seam exists; Window-count and `Show` behavior need static review plus separate manual smoke.
- Launch-safety blockers: actual default/preview launch and process-exit evidence is not collected in this batch.
- Product-readiness blockers: policy contract management, claim case management, and fresh-root target creation remain absent from ProductShell.
