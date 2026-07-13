# Product UI Shell Phase 1 Compile-Only Skeleton Validation Test Gate Plan

## A. Status

PRODUCT_UI_SHELL_PHASE1_COMPILE_ONLY_SKELETON_VALIDATION_TEST_GATE_PLAN_ONLY

## B. Marker

POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1_COMPILE_ONLY_SKELETON_VALIDATION_TEST_GATE_PLAN_READY

## C. Baseline

- baseline hash: `f4d9f7697d1124f0caf2727af6a21a143e134b45`
- current full-test baseline: PASS 334
- current resource/constants baseline: 64/64
- current `Ui.Product.*` baseline: 8/8
- work type: documentation-only validation planning

## D. Future Command Candidates

If a separate implementation batch is approved, run:

```powershell
dotnet build FamilyClaimRef.sln
dotnet test FamilyClaimRef.sln --filter FullyQualifiedName~ProductShellViewModelTests
dotnet test FamilyClaimRef.sln --filter FullyQualifiedName~ProductNavigationItemViewModelTests
dotnet test FamilyClaimRef.sln
```

If `ProductNavigationItemViewModel` is excluded by a later approved exact list, its targeted command must be recorded as excluded rather than executed.

## E. Future Validation Gates

- build succeeds and ProductShell XAML compiles
- selected targeted tests succeed with failed 0
- full tests succeed with failed 0
- full test count is compared with baseline 334
- existing tests are not deleted or weakened
- ProductShellWindow exists but has no runtime entry
- ProductShellWindow is not referenced from App startup
- MainWindow, App.xaml, App.xaml.cs, and AppServices remain unchanged
- project files remain unchanged
- `UiStrings.xaml` remains 64 keys
- `UiTextKeys.cs` remains 64 constants
- `Ui.Product.*` remains 8/8
- no resource key or direct Korean literal is added
- ProductHomeView, ProductDocumentRegistrationView, and ProductDocumentListView are absent
- no DB/SQLite/repository/OCR/migration dependency
- no app launch/manual workflow/visual automation

## E-1. Protected Path Gate

- `data/claimdoc` internal read/list/search/use does not occur.
- `docs/nightwork_*` internal read/search does not occur.
- protection verification is limited to approved ignore-rule checks.
- no protected local document is used as ProductShell sample or test data.

## F. Candidate Test Responsibilities

`ProductShellViewModelTests` candidate:

- constructor dependency guards
- navigation item order and exact approved text resolution
- initial selected navigation state
- selection change and property notification
- no dependency on MainWindow or storage/workflow services

`ProductNavigationItemViewModelTests` candidate:

- constructor guards
- stable navigation identifier and text
- selected-state notification if the model owns selection state

## G. Environment Boundary Rule

If normal build/test encounters the known Windows SDK user-profile access boundary, record the normal failure separately and rerun the identical command with the previously approved elevated path. Do not change SDK, project, system, or global Git settings.

## H. Current Batch Execution State

| Execution | Result |
|---|---|
| build | not run |
| targeted tests | not run |
| full tests | not run |
| app launch | not run |
| manual workflow | not run |
| reason | documentation-only compile-only skeleton exact-scope decision batch |

## I. Gate Judgment

These commands and checks are future candidates only. No implementation or validation execution is approved by this document.
