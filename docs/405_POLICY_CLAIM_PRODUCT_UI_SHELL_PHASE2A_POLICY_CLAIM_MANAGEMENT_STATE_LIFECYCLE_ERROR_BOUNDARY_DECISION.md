# Product UI Shell Phase 2A Policy Claim Management State Lifecycle Error Boundary Decision

## A. Marker

`POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE2A_POLICY_CLAIM_MANAGEMENT_STATE_LIFECYCLE_ERROR_BOUNDARY_DECISION_READY`

## B. Source Conflict Summary

The conditional Candidate B from `docs/401` cannot be used unchanged.

Confirmed conflicts:

- One core ViewModel currently owns policy and claim inputs, selections, collections, and one shared `ManagementMessage`.
- A policy result can remain visible when the user moves to the claim screen.
- Repeated view `Loaded` events can overlap asynchronous reads or mutations unless an operation boundary is added.
- Storage exceptions currently propagate through load/create/disable calls.
- Product view event handlers are expected to be `async void` forwarding handlers, so uncaught storage exceptions can reach the WPF dispatcher.
- Policy and claim selectors show only `DisplayTitle`; raw IDs remain hidden.
- Active duplicate titles therefore create indistinguishable user choices.

## C. Architecture Comparison

| Candidate | State owner | Load owner | Create/disable owner | Error catch owner | Screen message owner | MainWindow impact | Test/file impact | Decision |
|---|---|---|---|---|---|---|---|---|
| B1: unchanged shared core | Existing core ViewModel | Each view forwards `Loaded` directly | Existing core | none for storage/runtime failures | One shared message with no entry reset | none | smallest | rejected |
| B2: minimally strengthened shared core | One ProductShell-only strengthened core | Each view requests load; core serializes and performs it | Strengthened core | strengthened core catches non-cancellation storage/runtime failures | core stores message; entering product view clears stale message | existing MainWindow instance uses the strengthened compatible API; no MainWindow file change | one core file plus existing/new tests | selected |
| C: two wrappers around one core | Wrappers project screen state around a shared core | wrapper forwarding | wrapper forwarding to core | wrapper or core split ownership | wrapper-local projection | none | at least two wrappers and wrapper tests | rejected |

### B1 Rejection

B1 leaves the shared-message leak and unhandled storage exception path unresolved. It cannot satisfy the product error boundary.

### C Rejection

C would need to translate a single mutable core message into two wrapper-local projections. That either duplicates validation/message mapping or adds a notification bridge with more lifecycle states. The strengthened core plus entry reset resolves the confirmed conflict with fewer files and one behavior owner.

### B2 Selection

B2 preserves the tested storage/business flow while fixing only the product-safety gaps confirmed by source inspection.

## D. Ownership Contract

| Concern | Owner | Contract |
|---|---|---|
| Active policy/claim collections | `PolicyClaimManagementViewModel` | one ProductShell-only instance shared by both product management views |
| Policy input | `PolicyClaimManagementViewModel.NewPolicyDisplayTitle` | retained across navigation until successful policy creation or explicit user edit |
| Claim input | `PolicyClaimManagementViewModel.NewClaimDisplayTitle` | retained independently from policy input |
| Policy selection | core ViewModel | retained if still active; repaired only when invalid |
| Claim policy selection | core ViewModel | retained if still active; otherwise first active policy or null |
| Claim selection | core ViewModel | retained if still active; repaired only when invalid |
| Load | core ViewModel | public load requests are serialized and replace collections |
| Create/disable | core ViewModel | validation, storage call, refresh, selection repair, and result message |
| Storage/runtime catch | core ViewModel | catch non-cancellation exceptions and expose only approved safe resource text |
| Message storage | core ViewModel | one `ManagementMessage` remains for MainWindow compatibility |
| Message reset | product view entry | each policy/claim view clears the previous management message before requesting load |
| UI event forwarding | product view code-behind | no storage access and no diagnostic formatting |
| MainWindow management state | separately composed core instance | never shared with ProductShell |

## E. Shared State And Navigation Lifecycle

Selected policy:

- Policy and claim input values remain separate properties.
- In-progress input is retained when navigating away and back.
- A successful create clears only the corresponding input.
- A failed create does not clear input, allowing correction or retry.
- Selection remains stable when the selected active record still exists.
- A disabled or missing selection is repaired only during refresh.
- Policy and claim views never display raw IDs.

Selected message policy:

- Use safe message reset on screen entry.
- `ProductPolicyContractsView.Loaded` clears the previous management message, then requests load.
- `ProductClaimCasesView.Loaded` clears the previous management message, then requests load.
- A policy success/error cannot remain as the visible claim-screen result after claim-screen entry.
- A claim success/error cannot remain as the visible policy-screen result after policy-screen entry.
- Repeated `Loaded` does not clear form input or valid selections.

Wrapper-local message state is not added.

## F. Repeated Loaded And Concurrency Contract

- Repeated `Loaded` is allowed.
- Load replaces active-only collections; it never appends.
- One private asynchronous operation gate serializes load/create/disable work on the shared core instance.
- Validation that depends on current storage state runs inside the operation gate.
- Rapid repeated clicks cannot execute overlapping creates/disables on that instance.
- A view can request another load while an operation is running; it waits for the preceding operation.
- The gate must be released in `finally`.
- No UI element owns a lock or storage transaction.
- Input is not overwritten by load.
- Only invalid selection repair is allowed.

This contract prevents duplicate rows and same-instance double submissions without changing storage interfaces.

## G. Load And Mutation Result Contract

The strengthened core keeps the four mutation methods returning `Task<bool>`.

The strengthened core changes `LoadAsync` to return `Task<bool>` so callers can distinguish a refreshed view from a safe load failure. Existing callers may await and ignore the Boolean without changing behavior.

Rules:

1. A successful load returns `true`, replaces both collections, and repairs invalid selections.
2. A failed load returns `false` and sets the approved list-load failure message.
3. A validation failure returns `false`, keeps user input, and sets the corresponding validation message.
4. A storage mutation failure returns `false`, keeps user input where applicable, and sets the approved policy/claim operation failure message.
5. A successful mutation clears only its matching input.
6. A successful mutation followed by a refresh failure remains a successful mutation:
   - return `true`;
   - keep the list-load failure message;
   - do not replace it with a success message;
   - allow the caller to refresh dependent registration targets.
7. A successful mutation and refresh sets the normal success message and updated selection.
8. `OperationCanceledException` is not converted to a product failure message. Product event handlers pass the default non-cancelled token in this phase.

The distinction in rule 6 prevents an already-completed create from being presented as safe to repeat.

## H. Error Boundary

The core ViewModel is the single product error boundary for management storage operations.

It must not expose:

- exception message text;
- stack trace;
- local or runtime paths;
- JSON payloads;
- internal IDs;
- diagnostic type names.

It may expose only these approved resource-backed outcomes:

- list load failed;
- policy operation failed;
- claim operation failed;
- existing validation or relationship-block messages.

The two product code-behind files:

- forward `Loaded`, create, and disable events only;
- do not access `IPolicyClaimStorageService`;
- do not catch and format storage exceptions;
- do not use `MessageBox`;
- do not write diagnostics to the screen.

## I. Active Display-Title Duplicate Decision

Decision: reject duplicate active display titles through `PolicyClaimManagementViewModel`.

Identity remains the storage-generated `Id`. The duplicate rule is a product selection-safety guard, not a replacement identity.

Actual conflict path:

- `ProductDocumentRegistrationView` policy and claim selectors bind `DisplayMemberPath="DisplayTitle"` and `SelectedValuePath="Id"`.
- MainWindow management and registration selectors use the same display/value pattern.
- Product management lists must also hide raw IDs.
- Two active records with the same displayed title are therefore indistinguishable during disable and document-target selection.

Exact rule:

- Normalize the proposed title with existing trim behavior.
- Compare active titles using `StringComparison.OrdinalIgnoreCase`.
- Policy titles must be unique across active policies.
- Claim titles must be unique across active claims, not only within one policy, because the claim target selector is global and title-only.
- Disabled records do not block title reuse.
- The core reads current active records inside the operation gate before duplicate validation.
- Duplicate validation returns `false`, keeps the typed title, and shows the approved duplicate message.
- No raw ID or generated suffix is shown.
- No storage interface/implementation or schema change is authorized.

This is a ViewModel/product-flow invariant for the current single-window application. It is not claimed as a cross-process atomic storage invariant.

## J. Registration Target Refresh

Selected contract:

- Do not add an event bus.
- Do not share mutable registration state between management and registration ViewModels.
- Policy/claim create and disable refresh the shared management ViewModel immediately.
- After a successful management operation, entering `ProductDocumentRegistrationView` invokes its existing `Loaded -> LoadTargetOptionsAsync` path.
- `LoadTargetOptionsAsync` reads active policies and claims from storage, replaces collections, and repairs invalid target selections.
- Therefore the registration screen receives the latest completed management state on entry.
- A management operation must complete before this refresh guarantee applies.

Required integration proof:

1. Start from an isolated root.
2. Create a policy through the ProductShell management child.
3. Create a claim under that policy.
4. Invoke the ProductShell registration child load.
5. Confirm the new active policy and claim appear exactly once.
6. Disable the claim and policy through management.
7. Invoke registration load again.
8. Confirm disabled targets are absent and no stale selection remains.

## K. MainWindow Compatibility

- MainWindow keeps a separately composed `PolicyClaimManagementViewModel`.
- MainWindow XAML, code-behind, and `MainWindowViewModel` are not modified.
- Existing property and mutation method names remain available.
- Existing `ManagementMessage` remains available.
- Existing successful mutation delegation can continue to reload registration targets.
- Korean value changes to the ten shared management keys will change displayed harness wording but not its validation or storage behavior.
- Existing MainWindow and management tests must remain passing.

## L. Decision

- Selected architecture: `B2`.
- B1: rejected.
- C: rejected.
- Wrapper files required: `0`.
- Source blocker: `0`.
- Lifecycle blocker: `0`.
- Behavior blocker: `0`.
- Composition blocker: `0`.
