# Product UI Shell Phase 1D Runtime Entry Strategy And Exact File List Decision Candidate

## A. Status

- Marker: `POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1D_RUNTIME_ENTRY_STRATEGY_DECISION_CANDIDATE_READY`
- Implementation approved now: no
- Runtime entry approved now: no

## B. Candidate A To F Comparison

| Candidate | Exact source/test candidates | Window ownership and ViewModel lifetime | Harness and management availability | Startup/shutdown and manual evidence | Testability / complexity | Recommendation |
|---|---|---|---|---|---|---|
| A. AppServices composition-only | `AppServices.cs`, `AppServicesTests.cs`, result doc | AppServices composes separate ProductShell child VMs; no Window is created | MainWindow harness and policy/claim management remain available | Default startup and shutdown behavior unchanged; no manual runtime evidence | High testability, low complexity | Selected |
| B. ProductShell default startup | `App.xaml.cs` plus startup tests and composition files | App owns ProductShellWindow; MainWindow is no longer shown | Current policy/claim management path becomes unavailable | Default Window changes; shutdown ownership moves to ProductShell; manual evidence required | Medium testability, high functional risk | Not recommended |
| C. Guarded ProductShell startup mode | `App.xaml.cs`, a source-confirmed guard, and startup-mode tests | App selects exactly one Window; child graphs remain separate | MainWindow remains default and accessible | Explicit guard only; no dual launch; manual evidence possible after approval | Medium complexity | Deferred after composition |
| D. MainWindow launcher action | MainWindow XAML/code-behind or ViewModel, copy/resources, startup tests | MainWindow owns or launches a second Window | Harness becomes coupled to product navigation | Two-window shutdown/ownership must be specified | High coupling | Excluded |
| E. Dual Window launch | `App.xaml.cs`, lifetime and shutdown tests | Two Windows coexist and risk shared-state coupling | Both paths visible but purpose and ownership are unclear | Shutdown behavior becomes ambiguous | High complexity and risk | Rejected |
| F. New launcher/bootstrapper/runtime-mode service | New production abstractions and tests | New owner layer | Could preserve both, but duplicates current App boundary | Requires new lifecycle contract | Excessive abstraction | Deferred |

## C. Selected Strategy

Selected: Candidate A, AppServices composition-only.

- App startup remains MainWindow.
- App.xaml and App.xaml.cs remain unchanged.
- ProductShellWindow is not created or shown.
- MainWindow remains the validation harness and current policy/claim management path.
- Runtime entry is deferred to a separate decision after composition validation.

## D. Selected Factory Boundary

Selected semantic boundary: Option 1, ViewModel-only composition.

The current convention does not use a Window factory or a standalone MainWindowViewModel factory. `AppServices.Create(...)` constructs an instance graph and exposes `MainWindowViewModel`; App creates the Window. The compatible future candidate is therefore:

- `AppServices.Create(...)` also constructs and exposes a read-only `ProductShellViewModel` property.
- A private local/helper construction block may keep child graph creation readable.
- AppServices does not create `ProductShellWindow`.
- A later App/runtime-entry batch, if approved, owns Window creation.

A public `CreateProductShellWindow()` is not selected. A separate public `CreateProductShellViewModel()` is also not required unless a later source review proves the property-based graph cannot satisfy tests.

## E. Lifetime Contract Candidate

Shared inside one AppServices graph:

- `IDocumentStorageService`
- `IPolicyClaimStorageService`
- `IFileAttachmentService`
- `DocumentRegistrationWorkflow`
- `IFilePickerService`
- `IUiTextProvider`

Not shared:

- MainWindow `DocumentRegistrationViewModel`
- ProductShell `DocumentRegistrationViewModel`
- `PolicyClaimManagementViewModel`
- `MainWindowViewModel`
- `ProductDocumentListViewModel`
- `ProductShellViewModel`

Two Window launch remains unapproved.

## F. Future Exact Composition-Only File Candidate

| Path | Candidate status | Reason | Approved now |
|---|---|---|---|
| `app/FamilyClaimRef.App/Composition/AppServices.cs` | include candidate | Compose separate ProductShell child graph, expose ProductShellViewModel, and mirror seven fallback keys | no |
| `tests/FamilyClaimRef.App.Tests/Composition/AppServicesTests.cs` | include candidate | Verify graph exposure, child VM separation, fallback copy, roots, and no file creation | no |
| `docs/391_POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1D_RUNTIME_COMPOSITION_IMPLEMENTATION_RESULT_REVIEW.md` | include candidate | Record future implementation evidence | no |
| `app/FamilyClaimRef.App/App.xaml.cs` | deferred to runtime-entry batch | Not required for composition-only | no |
| `app/FamilyClaimRef.App/App.xaml` | not required | Resources are already merged | no |
| `app/FamilyClaimRef.App/MainWindow.xaml` | not required | Harness UI is preserved | no |
| `app/FamilyClaimRef.App/MainWindow.xaml.cs` | not required | No launcher or replacement | no |
| `app/FamilyClaimRef.App/ProductShell/ProductShellWindow.xaml.cs` | not required | Existing constructor already accepts ProductShellViewModel | no |
| Startup test file | deferred to runtime-entry batch | No tracked startup test exists and startup is unchanged | no |

Future composition-only candidate counts:

- Production modify: 1.
- Production create: 0.
- Test modify: 1.
- Test create: 0.
- Result document create: 1.
- Total future candidate files: 3.
- Implementation target now: 0.

## G. Default Startup Readiness

Default ProductShell startup ready: no.

Functional blockers:

1. ProductShell has no policy contract creation or management screen.
2. ProductShell has no claim case creation or management screen.
3. A fresh runtime root has no ProductShell path for creating registration targets.

When no active policies or claims exist, the registration ViewModel loads empty target lists and exposes no-active-target messages; registration cannot establish a target from ProductShell. Replacing MainWindow would therefore remove the only current management path.

Compile-only composition feasibility is not default-startup readiness. ProductShell must not be described as primary-startup ready.

## H. Runtime Entry Status

- Default replacement: approved no.
- Guarded mode: approved no.
- Harness launcher: approved no.
- Dual Window launch: rejected.
- Runtime entry: approved no.
- ProductShell launch/manual evidence: not run.

Blocker counts:

- Composition source blockers: 0.
- Fallback-copy gaps: 7.
- Lifetime blockers after decision: 0.
- Testability limitations: 1, non-blocking.
- Default-startup functional blockers: 3.
- Runtime-entry decision blockers: 2, default readiness and guarded-mode contract/approval.
