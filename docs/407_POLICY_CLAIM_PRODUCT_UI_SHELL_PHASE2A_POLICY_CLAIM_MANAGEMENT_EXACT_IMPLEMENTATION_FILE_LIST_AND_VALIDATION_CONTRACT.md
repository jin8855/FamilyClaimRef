# Product UI Shell Phase 2A Policy Claim Management Exact Implementation File List And Validation Contract

## A. Marker

`POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE2A_POLICY_CLAIM_MANAGEMENT_EXACT_IMPLEMENTATION_FILE_LIST_AND_VALIDATION_CONTRACT_READY`

## B. Exact Scope Summary

- Future implementation files: `15`
- CREATE: `5`
- MODIFY: `10`
- Wrapper ViewModels: `0`
- Storage/model/project/solution changes: `0`
- MainWindow/startup changes: `0`

No implementation is performed by this document batch.

## C. CREATE

| Exact path | Change purpose | Allowed changes | Forbidden changes | Related validation | Approved |
|---|---|---|---|---|---|
| `app/FamilyClaimRef.App/Views/ProductPolicyContractsView.xaml` | product policy list/create/disable surface | bindings and approved resource references only | storage access, raw IDs, diagnostics, literals outside approved copy | build, resource scan, integration tests | yes |
| `app/FamilyClaimRef.App/Views/ProductPolicyContractsView.xaml.cs` | forward Loaded/create/disable events | call shared core methods and clear entry message | storage construction/access, diagnostic formatting, `MessageBox` | build, guarded smoke later | yes |
| `app/FamilyClaimRef.App/Views/ProductClaimCasesView.xaml` | product claim list/policy select/create/disable surface | bindings and approved resource references only | storage access, raw IDs, diagnostics, literals outside approved copy | build, resource scan, integration tests | yes |
| `app/FamilyClaimRef.App/Views/ProductClaimCasesView.xaml.cs` | forward Loaded/create/disable events | call shared core methods and clear entry message | storage construction/access, diagnostic formatting, `MessageBox` | build, guarded smoke later | yes |
| `tests/FamilyClaimRef.App.Tests/ProductPolicyClaimManagementIntegrationTests.cs` | isolated ProductShell management and registration refresh proof | fresh-root synthetic-safe tests | app launch, real data, local production root | targeted test | yes |

## D. MODIFY

| Exact path | Change purpose | Allowed changes | Forbidden changes | Related validation | Approved |
|---|---|---|---|---|---|
| `app/FamilyClaimRef.App/ViewModels/ProductShellViewModel.cs` | expose ProductShell-only management child and add two navigation items | constructor/property/nav order `Home`, `PolicyContracts`, `ClaimCases`, `DocumentRegistration`, `DocumentList` | default startup, static singleton, direct storage | `ProductShellViewModelTests` | yes |
| `app/FamilyClaimRef.App/ProductShell/ProductShellWindow.xaml` | add two content templates and navigation triggers | bind both views to the shared management child | code-behind composition, MainWindow changes | build and guarded smoke later | yes |
| `app/FamilyClaimRef.App/Composition/AppServices.cs` | compose a distinct ProductShell management ViewModel and fallback copy | inject existing storage/provider, keep MainWindow instance separate | shared mutable instance across windows, storage redesign | `AppServicesTests` | yes |
| `app/FamilyClaimRef.App/ViewModels/PolicyClaimManagementViewModel.cs` | add operation serialization, safe errors, entry message clear, active duplicate guard | minimal B2 contract from `docs/405` | storage schema/interface changes, diagnostics in UI, wrapper behavior | core and integration tests | yes |
| `app/FamilyClaimRef.App/Resources/UiStrings.xaml` | add 23 approved keys and update ten values | exact values from `docs/406` | unapproved copy, key renames, unrelated resource changes | resource tests and scans | yes |
| `app/FamilyClaimRef.App/Services/Localization/UiTextKeys.cs` | add constants for 23 approved keys | exact key strings from `docs/406` | unrelated constants or duplicate aliases | resource tests | yes |
| `tests/FamilyClaimRef.App.Tests/ProductShellViewModelTests.cs` | navigation order, child ownership, initial selection | constructor and property assertions | UI Automation | targeted test | yes |
| `tests/FamilyClaimRef.App.Tests/Composition/AppServicesTests.cs` | ProductShell/MainWindow management separation and fallback values | graph identity and exact copy assertions | production root writes | targeted test | yes |
| `tests/FamilyClaimRef.App.Tests/Services/Localization/ResourceUiTextProviderTests.cs` | 91/91 and 35/35 resource contract | exact values, counts, uniqueness | broad unrelated resource rewrite | targeted test | yes |
| `tests/FamilyClaimRef.App.Tests/PolicyClaimManagementViewModelTests.cs` | B2 state/error/duplicate/concurrency regression | deterministic fake-storage and existing JSON-backed cases | product UI launch | targeted test | yes |

## E. VERIFY ONLY

These files may be read and validated but not modified in the implementation batch unless a new scope decision is approved.

| Exact path | Verification purpose |
|---|---|
| `app/FamilyClaimRef.App/ViewModels/ProductNavigationItemViewModel.cs` | arbitrary non-empty navigation IDs already supported |
| `tests/FamilyClaimRef.App.Tests/ProductNavigationItemViewModelTests.cs` | existing navigation item behavior remains passing |
| `app/FamilyClaimRef.App/Views/ProductDocumentRegistrationView.xaml` | title-only policy/claim selector and existing resource pattern |
| `app/FamilyClaimRef.App/Views/ProductDocumentRegistrationView.xaml.cs` | existing `Loaded -> LoadTargetOptionsAsync` forwarding |
| `app/FamilyClaimRef.App/ViewModels/DocumentRegistrationViewModel.cs` | active-target replacement and selection repair |
| `tests/FamilyClaimRef.App.Tests/DocumentRegistrationViewModelTests.cs` | existing registration target behavior remains passing |
| `app/FamilyClaimRef.App/Services/Storage/IPolicyClaimStorageService.cs` | existing boundary remains sufficient |
| `app/FamilyClaimRef.App/Services/Storage/JsonPolicyClaimStorageService.cs` | active-only reads and identity remain unchanged |
| `tests/FamilyClaimRef.App.Tests/JsonPolicyClaimStorageServiceTests.cs` | storage regression remains passing |
| `app/FamilyClaimRef.App/Models/Storage/PolicyRecord.cs` | identity and display fields unchanged |
| `app/FamilyClaimRef.App/Models/Storage/PolicyDraft.cs` | create contract unchanged |
| `app/FamilyClaimRef.App/Models/Storage/ClaimRecord.cs` | relationship and display fields unchanged |
| `app/FamilyClaimRef.App/Models/Storage/ClaimDraft.cs` | create contract unchanged |
| `app/FamilyClaimRef.App/ViewModels/MainWindowViewModel.cs` | existing successful-operation registration refresh remains compatible |
| `app/FamilyClaimRef.App/App.xaml.cs` | guarded startup selection remains unchanged |
| `app/FamilyClaimRef.App/Startup/StartupWindowModeSelector.cs` | preview/default selection remains unchanged |
| `tests/FamilyClaimRef.App.Tests/Startup/StartupWindowModeSelectorTests.cs` | startup gates remain passing |

## F. EXCLUDED

Explicitly excluded files and areas:

- any policy/claim wrapper ViewModel file;
- `app/FamilyClaimRef.App/ProductShell/ProductShellWindow.xaml.cs`;
- `app/FamilyClaimRef.App/MainWindow.xaml`;
- `app/FamilyClaimRef.App/MainWindow.xaml.cs`;
- `app/FamilyClaimRef.App/App.xaml`;
- `app/FamilyClaimRef.App/App.xaml.cs`;
- storage interface or JSON storage modifications;
- policy/claim record or draft modifications;
- `.sln`, `.csproj`, package, or NuGet changes;
- default-startup changes;
- DB, SQLite, migration, OCR, repository, backup, or rollback implementation;
- runtime artifact cleanup;
- real document or personal sample access.

## G. Production Behavior Contract

The implementation must provide:

1. Five navigation items in the approved order.
2. Home remains initial.
3. One ProductShell-only management ViewModel shared by two product views.
4. A different MainWindow management ViewModel instance.
5. Policy create/list/disable.
6. Claim policy selection/create/list/disable.
7. Active-only display.
8. Stable repeated load with no appended rows.
9. Input retention across navigation.
10. Screen-entry message reset.
11. Safe resource-backed failure messages.
12. No exception/path/ID/diagnostic display.
13. Active title duplicate rejection with case-insensitive trimmed comparison.
14. Serialized same-instance operations.
15. Registration targets refreshed by existing registration entry load.
16. No event bus and no cross-child mutable collection sharing.

## H. Automated Validation Contract

Required commands in a later implementation batch:

```powershell
dotnet build FamilyClaimRef.sln
dotnet test FamilyClaimRef.sln --filter FullyQualifiedName~ProductPolicyClaimManagementIntegrationTests
dotnet test FamilyClaimRef.sln --filter FullyQualifiedName~PolicyClaimManagementViewModelTests
dotnet test FamilyClaimRef.sln --filter FullyQualifiedName~ProductShellViewModelTests
dotnet test FamilyClaimRef.sln --filter FullyQualifiedName~AppServicesTests
dotnet test FamilyClaimRef.sln --filter FullyQualifiedName~ResourceUiTextProviderTests
dotnet test FamilyClaimRef.sln --filter FullyQualifiedName~DocumentRegistrationViewModelTests
dotnet test FamilyClaimRef.sln
```

Required assertions:

- Build warnings/errors: `0/0`.
- Existing tests deleted: `0`.
- Full discovered test count: at least carry-forward `393`.
- Navigation IDs/order/count exact.
- MainWindow/ProductShell management instances are different.
- Both product management views receive the same ProductShell instance.
- Empty load, populated load, and repeated load are stable.
- Policy input and claim input remain independent.
- Entry message clear does not clear input.
- Validation failure keeps input.
- Create/disable success refreshes active collections.
- Failed mutation does not report success.
- Successful mutation plus refresh failure reports the safe load error and returns mutation success.
- Storage exceptions never become displayed exception text.
- Policy and claim active duplicate titles are rejected.
- Case-only and surrounding-space duplicates are rejected.
- Disabled title reuse is allowed.
- Parallel same-instance create calls do not create duplicates.
- Registration load sees newly created targets exactly once.
- Registration load removes disabled targets and repairs stale selection.
- Resource/constants are `91/91`.
- `Ui.Product.*` resource/constants are `35/35`.
- Exact 33 value decisions from `docs/406` are present.
- Production Korean literal scan outside approved resources: `0` findings.
- Local profile/path/diagnostic/privacy scan: `0` findings.

## I. Manual Validation Separation

Automated implementation validation does not authorize:

- app launch;
- OpenFileDialog;
- runtime workflow;
- screenshot;
- default-startup change.

A guarded ProductShell management smoke requires separate explicit approval after implementation and automated validation.

## J. Exact Count Decision

| Classification | Count |
|---|---:|
| CREATE | 5 |
| MODIFY | 10 |
| Exact implementation total | 15 |
| VERIFY ONLY | 17 |
| Wrapper ViewModels | 0 |
| Storage/model modifications | 0 |
| MainWindow/startup modifications | 0 |

The former candidate count is replaced by this source-reconciled exact count.
