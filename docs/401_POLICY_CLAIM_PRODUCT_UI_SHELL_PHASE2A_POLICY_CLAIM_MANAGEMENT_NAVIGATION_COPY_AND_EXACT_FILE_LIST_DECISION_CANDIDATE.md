# Product UI Shell Phase 2A Policy Claim Management Navigation Copy And Exact File List Decision Candidate

## A. Marker

`POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE2A_POLICY_CLAIM_MANAGEMENT_NAVIGATION_COPY_AND_EXACT_FILE_LIST_DECISION_CANDIDATE_READY`

## B. Candidate A Through G Comparison

| Candidate | Create/modify shape | Ownership and lifecycle | Copy/resource impact | Fresh-root capability | Testability/complexity | Recommendation |
|---|---|---|---|---|---|---|
| A: combined direct reuse | One product view; shell/composition/resources modified | One core instance and one Loaded path | Combined title/navigation copy | yes after implementation | Low, but screen boundary is weak | not selected |
| B: two views, one shared core instance | Four view files; shell/composition/resources modified | One ProductShell-only core instance shared by two views | Two navigation/title/empty families; shared runtime messages | yes after implementation and refresh proof | Moderate; reuses covered behavior | selected, conditional |
| C: two wrappers around one core instance | Candidate B plus two wrapper classes and tests | Screen-local projections wrap shared state | Easier product copy separation | yes | Higher notification/lifecycle complexity | not required initially |
| D: split core ViewModel | Replace one core behavior surface with policy/claim ViewModels | Clear split but broad behavior migration | Separate message ownership | yes | High regression scope | rejected |
| E: product ViewModel uses storage directly | New product business ViewModel and tests | Separate product state bypasses current core logic | Product-specific | yes | Logic duplication and drift | rejected |
| F: view/code-behind uses storage | Views construct/use storage | UI owns persistence | Uncontrolled | possible | Poor boundary/testability | excluded |
| G: repository/query/DB | New data layer and migrations | New architecture | Unrelated | possible | Highest and unapproved | excluded |

## C. Selected Screen Architecture

Selected candidate: `Candidate B`.

- Screen count: `2`.
- `ProductPolicyContractsView` owns the policy-facing visual surface.
- `ProductClaimCasesView` owns the claim-facing visual surface.
- Both views receive the same ProductShell-only `PolicyClaimManagementViewModel` instance.
- MainWindow and ProductShell never share the same mutable management ViewModel.
- Wrapper ViewModels are not required for the first candidate.
- The existing management ViewModel and storage remain unchanged.

Approved now: `no`.

## D. Navigation ID And Order Decision

Selected future IDs and order:

1. `Home`
2. `PolicyContracts`
3. `ClaimCases`
4. `DocumentRegistration`
5. `DocumentList`

Decisions:

- `PolicyContracts` is selected instead of `InsuranceContracts` to align with the model/resource candidate family.
- Navigation item additions: `2`.
- `ProductNavigationItemViewModel` needs no modification because it already accepts arbitrary non-empty IDs/display text.
- Home remains the first item and the initial selected item.
- Existing registration/list IDs remain unchanged.

## E. DataContext And Lifetime Decision

- `AppServices` creates a new ProductShell-only `PolicyClaimManagementViewModel`.
- The MainWindow management instance remains separate.
- `ProductShellViewModel` exposes the injected child as a read-only property.
- Both product management views bind to that same child through Window-relative DataContext, matching existing content templates.
- Each management view may call the shared child's `LoadAsync` from `Loaded`, matching current product view event forwarding.
- Repeated loads must be tested for stable active-only collections and no duplicate rows.
- Policy creation immediately refreshes the shared claim policy options.
- Registration target refresh relies on the existing `ProductDocumentRegistrationView.Loaded -> LoadTargetOptionsAsync` path when registration is entered.
- Shared form inputs and `ManagementMessage` persist across navigation unless a later behavior decision changes that contract.
- Window/view self-composition, static mutable singleton state, and direct storage construction remain forbidden.

## F. Display And Privacy Projection

| Source field/state | Policy screen | Claim screen | Decision |
|---|---|---|---|
| `DisplayTitle` | show | show | primary user-facing value |
| `ReferenceDate` | optional initial display | optional initial display | source-confirmed but not required for creation input |
| `PolicyId` on claim | no raw display | map through policy title if needed | raw relationship ID hidden |
| `Id` | bind as selection value only | bind as selection value only | never display raw ID |
| `CreatedAt` / `UpdatedAt` | not shown initially | not shown initially | defer |
| `DisabledAt` | not shown; disabled rows are absent | not shown; disabled rows are absent | active-only projection |
| Local/runtime paths | never | never | forbidden display |
| Diagnostics/exception details | never | never | product-safe message boundary required |

No real names, insurer names, hospital names, diagnosis data, policy numbers, claim numbers, local paths, or diagnostic details are permitted.

## G. Copy And Resource Inventory

| Concept | Classification | Decision |
|---|---|---|
| Policy/claim navigation | product-specific key candidate | add two candidates |
| Page titles | product-specific key candidate | add two candidates |
| Empty states | product-specific key candidate | add two candidates |
| Creation/list/form labels | product-specific key candidate | do not reuse validation-harness English copy |
| Create/disable actions | product-specific key candidate | exact copy approval required |
| Status label | product-specific key candidate | exact copy approval required |
| Required-field/selection/success messages | runtime-message reuse candidate | existing ten keys need exact Korean value approval |
| Load failure | not supported by current management ViewModel | needs behavior and copy approval |
| Duplicate validation | no current business rule | not shown until behavior approval |

The 14 static management keys under `Ui.Policy.*`, `Ui.Claim.*`, `Ui.Management.*`, and `Ui.Action.*` remain validation-harness-only evidence. They are not automatically promoted.

## H. Exact Candidate Copy Table

The following 18 additions are candidates, not approvals:

| Key candidate | Value candidate | Approved now |
|---|---|---|
| `Ui.Product.Navigation.PolicyContracts` | `보험 계약` | no |
| `Ui.Product.Navigation.ClaimCases` | `청구 건` | no |
| `Ui.Product.PolicyContracts.Title` | `보험 계약` | no |
| `Ui.Product.ClaimCases.Title` | `청구 건` | no |
| `Ui.Product.PolicyContracts.EmptyMessage` | `등록된 보험 계약이 없습니다.` | no |
| `Ui.Product.ClaimCases.EmptyMessage` | `등록된 청구 건이 없습니다.` | no |
| `Ui.Product.PolicyContracts.CreationSection` | `보험 계약 등록` | no |
| `Ui.Product.ClaimCases.CreationSection` | `청구 건 등록` | no |
| `Ui.Product.PolicyContracts.ActiveListLabel` | `보험 계약 목록` | no |
| `Ui.Product.ClaimCases.ActiveListLabel` | `청구 건 목록` | no |
| `Ui.Product.PolicyContracts.DisplayTitleLabel` | `보험 계약 이름` | no |
| `Ui.Product.ClaimCases.DisplayTitleLabel` | `청구 건 이름` | no |
| `Ui.Product.ClaimCases.PolicyLabel` | `보험 계약` | no |
| `Ui.Product.PolicyContracts.CreateAction` | `보험 계약 등록` | no |
| `Ui.Product.PolicyContracts.DisableAction` | `보험 계약 사용 중지` | no |
| `Ui.Product.ClaimCases.CreateAction` | `청구 건 등록` | no |
| `Ui.Product.ClaimCases.DisableAction` | `청구 건 사용 중지` | no |
| `Ui.Product.Management.StatusLabel` | `처리 결과` | no |

Ten existing `Ui.PolicyManagement.*` and `Ui.ClaimManagement.*` runtime message values are reuse candidates. Their exact product Korean values need a separate approval table because changing them also changes the MainWindow validation harness.

Additional product key candidate count: `18`.

## I. Future Exact Implementation Candidate

This is one source-based candidate, not an approved implementation list.

| File | Classification | Reason | Approved now |
|---|---|---|---|
| `app/FamilyClaimRef.App/Views/ProductPolicyContractsView.xaml` | include candidate | policy product surface | no |
| `app/FamilyClaimRef.App/Views/ProductPolicyContractsView.xaml.cs` | include candidate | Loaded/create/disable forwarding | no |
| `app/FamilyClaimRef.App/Views/ProductClaimCasesView.xaml` | include candidate | claim product surface | no |
| `app/FamilyClaimRef.App/Views/ProductClaimCasesView.xaml.cs` | include candidate | Loaded/create/disable forwarding | no |
| `app/FamilyClaimRef.App/ViewModels/ProductShellViewModel.cs` | include candidate | child exposure and two navigation items | no |
| `app/FamilyClaimRef.App/ProductShell/ProductShellWindow.xaml` | include candidate | two DataTemplates/triggers | no |
| `app/FamilyClaimRef.App/Composition/AppServices.cs` | include candidate | separate ProductShell management child and fallback mirrors | no |
| `app/FamilyClaimRef.App/Resources/UiStrings.xaml` | needs copy approval | 18 product keys and possible runtime value decisions | no |
| `app/FamilyClaimRef.App/Services/Localization/UiTextKeys.cs` | needs copy approval | constants for approved product keys | no |
| `tests/FamilyClaimRef.App.Tests/ProductPolicyClaimManagementIntegrationTests.cs` | include candidate | fresh-root management/refresh behavior | no |
| `tests/FamilyClaimRef.App.Tests/ProductShellViewModelTests.cs` | include candidate | count/order/constructor/child regression | no |
| `tests/FamilyClaimRef.App.Tests/Composition/AppServicesTests.cs` | include candidate | graph separation and fallback copy | no |
| `tests/FamilyClaimRef.App.Tests/Services/Localization/ResourceUiTextProviderTests.cs` | include candidate | resource/constant contract | no |
| Future implementation result review, number unassigned | include candidate | result evidence | no |

Not required:

- `ProductPolicyContractsViewModel.cs`
- `ProductClaimCasesViewModel.cs`
- `ProductPolicyContractItemViewModel.cs`
- `ProductClaimCaseItemViewModel.cs`
- `PolicyClaimManagementViewModel.cs`
- `ProductNavigationItemViewModel.cs`
- `ProductShellWindow.xaml.cs`
- `PolicyClaimManagementViewModelTests.cs`
- policy/claim storage interface, JSON implementation, models, and storage tests
- project/solution/package files

## J. Candidate Counts

| Category | Count |
|---|---:|
| Production create | 4 |
| Production modify, excluding resources | 3 |
| Test create | 1 |
| Test modify | 3 |
| Resource modify | 2 |
| Storage modify | 0 |
| Result document | 1 |
| Total future candidate files | 14 |
| Implementation target now | 0 |

## K. Blocker Counts

| Blocker class | Count | Detail |
|---|---:|---|
| Source | 0 | Minimum core behavior is source-confirmed |
| Lifecycle | 1 | Shared input/message persistence and repeated Loaded policy |
| Behavior | 1 | Display-title duplicate semantics |
| Copy/resource | 2 | 18 new product keys; 10 shared runtime-message values |
| Composition | 1 | Registration target refresh proof across child ViewModels |
| Default-startup readiness | 7 | implementation, build/regression, guarded management smoke, isolated-root create flow, registration refresh smoke, navigation/UI evidence, separate startup approval |

## L. Fresh-Root And Default-Startup Judgment

- Current ProductShell fresh-root policy creation: unavailable.
- Current ProductShell fresh-root claim creation: unavailable.
- Candidate B policy creation after implementation: capable through existing storage/ViewModel.
- Candidate B claim creation after implementation: capable only after an active policy exists.
- Candidate B registration target refresh: candidate through existing registration view load, not yet proven.
- ProductShell default startup readiness after implementation: not automatic.
- Default startup remains `MainWindow`.
- Guarded preview remains `--product-shell-preview`.
- Default startup change approved now: no.
- Exact implementation file list approved now: no.
