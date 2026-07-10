# Product UI Shell Phase 1 Validation and Risk Plan

## A. Status

PRODUCT_UI_SHELL_PHASE1_VALIDATION_AND_RISK_PLAN_ONLY

## B. Marker

POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1_VALIDATION_AND_RISK_PLAN_READY

## C. Baseline

- baseline commit: `6cee3a9 docs(familyclaimref): plan product shell phase1 scope`
- current work type: documentation-only implementation preflight planning

## D. Validation Prerequisites

Future implementation must not start until these prerequisites are explicitly approved:

- exact implementation file list approved
- resource/copy table approved if `Ui.Product.*` keys are added
- product terminology decision approved if shown in UI
- `MainWindow` non-replacement confirmed
- app startup impact confirmed
- no `data/claimdoc` usage confirmed
- no DB/SQLite/repository/OCR/migration dependency confirmed

## E. Future Validation Commands

If a future implementation batch is approved, expected validation may include:

```powershell
dotnet build FamilyClaimRef.sln
dotnet test FamilyClaimRef.sln --filter FullyQualifiedName~ResourceUiTextProviderTests
dotnet test FamilyClaimRef.sln --filter FullyQualifiedName~DocumentRegistrationViewModel
dotnet test FamilyClaimRef.sln
```

No build or test command is approved or run by this documentation-only batch.

## F. Future Test Candidate Table

| Future test area | Candidate test file | Required if | Approved now |
|---|---|---|---|
| ProductShellViewModel navigation state | `tests/FamilyClaimRef.App.Tests/ProductShellViewModelTests.cs` | `ProductShellViewModel` is created | no |
| ProductNavigationItemViewModel behavior | `tests/FamilyClaimRef.App.Tests/ProductNavigationItemViewModelTests.cs` | navigation item model is created | no |
| Product shell resource key resolution | `tests/FamilyClaimRef.App.Tests/Services/Localization/ResourceUiTextProviderTests.cs` | `Ui.Product.*` keys are added | no |
| DocumentRegistrationViewModel reuse regression | `tests/FamilyClaimRef.App.Tests/DocumentRegistrationViewModelTests.cs` | product view reuses or wraps existing ViewModel | no |
| Document list data-source behavior | future document list test candidate | document list view model or data adapter is created | no |
| full regression | existing test suite | any ProductShell code is added | no |

## G. Risk Table

| Risk | Impact | Mitigation | Implementation blocker |
|---|---|---|---|
| dead `ProductShellWindow` with no runtime entry | product shell compiles but cannot be reached manually | approve compile-only as intentional or approve a runtime entry separately | yes |
| `MainWindow` validation harness regression | existing validation workflow could break | keep `MainWindow` unchanged in Phase 1 preflight | yes |
| hard-coded product copy without resource ownership | copy drift and localization debt | approve resource/copy table first | yes |
| `Ui.Product.*` addition without copy table | resource keys become ungoverned | require approved key/value table before changes | yes |
| terminology finalization too early | product copy may conflict with later domain decisions | keep terminology as candidate until approved | yes |
| ProductShell ViewModel overcoupled to `MainWindowViewModel` | validation harness and product shell become tangled | use separate `ProductShellViewModel` candidate | yes |
| `DocumentRegistrationViewModel` behavior regression | existing document registration tests may fail | require reuse regression tests if reused | yes |
| document list requiring repository/DB too early | opens storage architecture prematurely | keep JSON/storage-service source of truth | yes |
| app launch/manual workflow not approved | runtime validation cannot be claimed | keep implementation test-only unless runtime is approved | yes |
| `data/claimdoc` accidental use | real local documents could enter validation scope | keep `data/claimdoc` protected and excluded | yes |

## H. Readiness Judgment

Product shell code implementation should not start until exact file list, entry strategy, resource/copy boundary, and test candidates are approved.

The current batch does not approve implementation.

