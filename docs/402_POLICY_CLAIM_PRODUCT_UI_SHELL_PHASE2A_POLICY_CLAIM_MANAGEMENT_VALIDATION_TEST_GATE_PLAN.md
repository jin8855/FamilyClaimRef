# Product UI Shell Phase 2A Policy Claim Management Validation Test Gate Plan

## A. Marker

`POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE2A_POLICY_CLAIM_MANAGEMENT_VALIDATION_TEST_GATE_PLAN_READY`

## B. Execution Status

- This is a future validation plan.
- Build/test commands run in this batch: none.
- App launch/manual workflow/UI Automation run in this batch: none.
- Baseline full tests remain carry-forward PASS `393/393`.

## C. Future Commands

Run only after an exact implementation contract is separately approved.

```powershell
dotnet build FamilyClaimRef.sln
dotnet test FamilyClaimRef.sln --filter FullyQualifiedName~ProductPolicyClaimManagementIntegrationTests
dotnet test FamilyClaimRef.sln --filter FullyQualifiedName~PolicyClaimManagementViewModelTests
dotnet test FamilyClaimRef.sln --filter FullyQualifiedName~ProductShellViewModelTests
dotnet test FamilyClaimRef.sln --filter FullyQualifiedName~AppServicesTests
dotnet test FamilyClaimRef.sln --filter FullyQualifiedName~ResourceUiTextProviderTests
dotnet test FamilyClaimRef.sln
```

## D. Compile And Static Gates

| Gate | Future expectation |
|---|---|
| XAML compile | Both product management views compile |
| Navigation mapping | Five exact IDs map to five templates |
| Initial selection | Home remains initial |
| Existing mapping | Registration/list mappings remain unchanged |
| Resource parity | Resource and constant sets remain exactly equal |
| Production Korean literals | Resource-backed only |
| Project/package scope | No project, solution, or package change unless a new evidence gap appears |

## E. Policy Gates

- Fresh empty storage loads an empty policy list.
- Required policy display title is enforced.
- Whitespace is normalized.
- Policy creation adds one active policy.
- Generated ID is not displayed.
- Repeated reload produces no duplicate rows.
- Disable with no active claim removes the policy from the active projection.
- Disable with an active claim remains blocked.
- Display-title duplicate behavior matches a separately approved rule; current source does not reject duplicates.

## F. Claim Gates

- Fresh empty storage loads an empty claim list.
- Claim creation is blocked without an active policy.
- Required claim display title is enforced.
- Claim creation under the selected active policy succeeds.
- Raw policy/claim IDs are not displayed.
- Repeated reload produces no duplicate rows.
- Disable removes the claim from the active projection.
- Disabled claims are not available as new registration targets.
- Display-title duplicate behavior matches a separately approved rule.

## G. Composition And Lifetime Gates

- MainWindow and ProductShell management ViewModels are different instances.
- The two ProductShell management views share one ProductShell-only management instance.
- Separate `AppServices.Create` calls produce separate mutable graphs.
- Repeated policy/claim view Loaded cycles preserve stable collections.
- Shared form input/message retention follows the approved lifecycle rule.
- Policy creation refreshes claim policy options.
- Entering document registration reloads policy/claim target options.
- Storage and workflow instances remain composed centrally.
- No view or Window creates concrete storage.

## H. Privacy And Copy Gates

- Display only approved product fields.
- No raw IDs, policy relationship IDs, local paths, runtime roots, or diagnostic details.
- No real personal, insurer, hospital, diagnosis, contract-number, or claim-number samples.
- Product terminology remains `보험 계약`, `청구 건`, and `연결 대상`.
- Validation-harness static copy is not silently reused as product copy.
- Ten shared runtime message values require exact approval before product release.
- Load failure copy and behavior require a separate decision if implemented.

## I. Runtime And Startup Gates

- Guarded preview token remains exact: `--product-shell-preview`.
- Default startup remains `MainWindow`.
- No dual-window launch.
- Guarded management workflow smoke requires separate approval.
- Fresh isolated-root policy create, claim create, and registration target refresh require separate runtime approval.
- ProductShell default-startup readiness remains `no` until those gates and a separate startup decision are complete.

## J. Regression Gate

- Existing tests deleted: `0`.
- Build warnings/errors: `0/0`.
- Selected product management tests: all pass.
- Existing management, ProductShell, AppServices, and resource tests: all pass.
- Full solution tests: pass with discovered count at least the carry-forward baseline `393`.
- Storage behavior remains unchanged.
- Project-root runtime artifacts remain absent.
