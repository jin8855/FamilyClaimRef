# Product UI Shell Phase 2A Policy Claim Management Source Workflow ViewModel Reconciliation

## A. Marker

`POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE2A_POLICY_CLAIM_MANAGEMENT_SOURCE_WORKFLOW_VIEWMODEL_RECONCILIATION_READY`

## B. Evidence Rule

Only tracked source, tests, resources, and decision documents are treated as evidence. Unconfirmed behavior is not promoted to a product contract.

## C. Source Evidence Matrix

| Area | Actual source evidence | Phase 2A implication | Status |
|---|---|---|---|
| Management constructor | `PolicyClaimManagementViewModel(IPolicyClaimStorageService, IUiTextProvider)` | Existing storage and resource boundaries can be reused | confirmed |
| Policy input state | `NewPolicyDisplayTitle` | Initial product form can accept one display title | confirmed |
| Policy collection | `AvailablePolicies`, `HasAvailablePolicies` | Active-only policy list can be projected | confirmed |
| Policy selection | `SelectedPolicyId`, `CanDisablePolicy` | Selection can bind by ID while displaying title only | confirmed |
| Claim input state | `NewClaimDisplayTitle` | Initial product form can accept one display title | confirmed |
| Claim collection | `AvailableClaims`, `HasAvailableClaims` | Active-only claim list can be projected | confirmed |
| Claim selection | `SelectedClaimId`, `CanDisableClaim` | Selection can bind by ID while hiding the ID | confirmed |
| Claim policy dependency | `SelectedPolicyForClaimId`, `CanCreateClaim` | Active policy selection is required before claim creation | confirmed |
| Load lifecycle | One `LoadAsync` reads policies then claims and repairs three selections | One shared instance owns both screen collections | confirmed |
| Policy create | Normalizes title, creates `PolicyDraft` with today's date, reloads, selects created policy | Direct reuse supports minimum policy creation | confirmed |
| Claim create | Validates title and active selected policy, creates `ClaimDraft`, reloads, selects claim | Direct reuse supports policy-dependent claim creation | confirmed |
| Policy disable | Blocks when active claims exist, then disables and reloads | Existing relationship rule is reusable | confirmed |
| Claim disable | Disables selected claim and reloads | Existing minimum claim disable is reusable | confirmed |
| Validation state | Boolean `Can*` properties plus `ManagementMessage` | Product views can bind current guards; exact product copy remains unresolved | conditional |
| Status/error state | Success and validation text use `UiTextKeys`; storage exceptions are not caught | Product error presentation and load failure behavior need a later behavior/copy decision | blocker |
| Storage interface | `Get*`, `Add*`, `Disable*`, existence checks | No interface expansion is needed for current minimum scope | confirmed |
| JSON implementation | Active-only reads; GUID-based IDs; UTC timestamps; `DisabledAt` projection | JSON remains source of truth; IDs/timestamps stay storage-owned | confirmed |
| Duplicate prevention | Generated-ID collision check only; no display-title duplicate rule | Business duplicate semantics are not implemented | blocker |
| Required fields | Storage requires title/reference date and claim policy ID; ViewModel supplies today | Initial product form need not expose raw ID or timestamps | confirmed |
| MainWindow code-behind | `Loaded` and button handlers only forward to `MainWindowViewModel` | Existing event-forwarding convention exists; no `ICommand` convention exists | confirmed |
| MainWindow delegation | Successful management actions reload registration targets | Cross-child refresh behavior exists only in MainWindow wrapper | confirmed |
| AppServices graph | One MainWindow management child exists; ProductShell has registration/list children only | ProductShell needs a separate management child instance | confirmed |
| Existing tests | Management creation, disable, active filtering, relationship block, and MainWindow refresh are covered | Core behavior can remain regression-tested | confirmed |
| Runtime messages | Ten policy/claim message keys are injected through `IUiTextProvider` | Keys are reusable candidates, but current values are validation-harness English | copy blocker |
| Product terminology | Existing `Ui.Product.*` uses `보험 계약` and `청구 건` | Product screens should keep these terms | confirmed |
| Guarded preview graph | App selects ProductShell only for exact preview token | Runtime entry does not need modification for Phase 2A candidate | confirmed |
| Fresh-root dependency | ProductShell currently has no target-creation screen | Current preview cannot create registration targets | confirmed blocker |

## D. Policy Contract Behavior Inventory

| Concern | Source finding |
|---|---|
| Public input | `NewPolicyDisplayTitle` |
| Public collection | `AvailablePolicies` active-only |
| Selected item | `SelectedPolicyId` |
| Create method | `CreatePolicyAsync` |
| Disable method | `DisableSelectedPolicyAsync` |
| Validation | Required trimmed title; selected target; active-claim disable block |
| Status | Shared `ManagementMessage` |
| Refresh | `LoadAsync` after successful create/disable |
| Storage calls | `GetPoliciesAsync`, `AddPolicyAsync`, `GetClaimsByPolicyIdAsync`, `DisablePolicyAsync` |
| Duplicate behavior | Display-title duplicates are not rejected |
| ID ownership | `JsonPolicyClaimStorageService` generates `policy_<guid>` |
| Timestamp/status ownership | Storage sets `CreatedAt`, `UpdatedAt`, and `DisabledAt` |

## E. Claim Case Behavior Inventory

| Concern | Source finding |
|---|---|
| Public input | `NewClaimDisplayTitle` |
| Public collection | `AvailableClaims` active-only |
| Selected item | `SelectedClaimId` |
| Selected policy dependency | `SelectedPolicyForClaimId` must refer to an active policy |
| Create method | `CreateClaimAsync` |
| Disable method | `DisableSelectedClaimAsync` |
| Validation | Required trimmed title, active policy selection, selected claim |
| Status | Shared `ManagementMessage` |
| Refresh | `LoadAsync` after successful create/disable |
| Storage calls | `GetClaimsAsync`, `AddClaimAsync`, `DisableClaimAsync` |
| Duplicate behavior | Display-title duplicates are not rejected |
| ID ownership | `JsonPolicyClaimStorageService` generates `claim_<guid>` |
| Timestamp/status ownership | Storage sets `CreatedAt`, `UpdatedAt`, and `DisabledAt` |

## F. Shared Behavior And Lifecycle

- One `PolicyClaimManagementViewModel` instance owns both policy and claim state.
- One `LoadAsync` refreshes both collections.
- Policy creation refreshes policy options and selects the created policy for claim creation.
- Policy disable is blocked while active claims exist.
- Claim disable removes the claim from subsequent active projections.
- Existing UI uses code-behind event forwarding; there is no reusable `ICommand` infrastructure.
- Validation and success messages are resource-owned.
- Storage and load exceptions propagate because the management ViewModel has no catch boundary.
- `NewPolicyDisplayTitle`, `NewClaimDisplayTitle`, and `ManagementMessage` are shared across two future product views if one core instance is shared.

## G. Architecture Candidate Summary

| Candidate | Production shape | Main concern | Judgment |
|---|---|---|---|
| A | One combined product view using existing ViewModel | Product screen boundary conflicts with separate policy/claim wireframe areas | not selected |
| B | Two product views sharing one ProductShell-only existing ViewModel | Shared message/input and repeated Loaded lifecycle | selected, conditional |
| C | Two wrapper ViewModels delegating to existing ViewModel | Notification duplication and wrapper drift | not required initially |
| D | Split existing management ViewModel | Large production/test behavior change | rejected for Phase 2A |
| E | New product ViewModel using storage directly | Duplicates business/validation logic | rejected |
| F | View/code-behind uses storage directly | Violates composition boundary | excluded |
| G | New repository/query/DB layer | Unnecessary and outside approved storage direction | excluded |

## H. Required Judgments

- Combined existing ViewModel direct reuse: `yes, conditional`.
- Separate product views: `required` by selected product screen boundary.
- Wrapper ViewModels: `not required` for the initial candidate.
- Existing production management ViewModel modification: `not required`.
- Storage modification: `not required`.
- ProductShell composition modification: `required` in a future approved implementation.
- AppServices modification: `required` to create a ProductShell-only management child.

## I. Blockers

- Source blockers: `0` for minimum create/list/disable behavior.
- Lifecycle blocker: shared input/message persistence and repeated `Loaded` refresh contract need explicit acceptance.
- Behavior blocker: display-title duplicate semantics are not defined as a product rule.
- Composition blocker: registration target refresh after management changes must be proven through existing registration-view load behavior.
- Copy blocker: validation-harness management keys cannot be promoted automatically to product copy.
