# Product UI Shell Phase 1B2 Document Registration Validation Test Gate Plan

## A. Status

- Marker: `POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1B2_DOCUMENT_REGISTRATION_VALIDATION_TEST_GATE_PLAN_READY`
- Baseline known full tests: PASS 351/351
- Current batch execution: documentation-only; all build/test/runtime commands are not run

## B. Future Validation Commands

Run only after a separately approved implementation batch.

```powershell
dotnet build FamilyClaimRef.sln
dotnet test FamilyClaimRef.sln --filter FullyQualifiedName~DocumentRegistrationViewModelTests
dotnet test FamilyClaimRef.sln --filter FullyQualifiedName~ProductShellViewModelTests
dotnet test FamilyClaimRef.sln --filter FullyQualifiedName~ResourceUiTextProviderTests
dotnet test FamilyClaimRef.sln
```

- `ProductShellViewModelTests` is included because Candidate A modifies the shell ViewModel contract.
- `ResourceUiTextProviderTests` is included because the conditional exact list contains resource changes.
- No nonexistent `ProductDocumentRegistrationViewTests` command is proposed.
- No OpenFileDialog or visual automation test is proposed.

## C. Build and XAML Gates

| Gate | Required future result |
|---|---|
| Solution build | PASS, warnings 0, errors 0 |
| `ProductDocumentRegistrationView.xaml` compile | PASS if included |
| Existing `ProductHomeView` compile and mapping | preserved |
| Project-file scope | `.sln` and `.csproj` unchanged |
| Runtime entry | absent |
| MainWindow/App startup | unchanged |

## D. Behavior Gates

- `DocumentRegistrationViewModelTests` failed 0.
- Existing registration validation, target mapping, picker cancel, success, failure, cleanup-failure, and project-root safety behavior remains unchanged.
- `DocumentRegistrationWorkflow` is reused and not bypassed.
- `IFilePickerService` is reused; the view does not instantiate `OpenFileDialog`.
- `DocumentRegistrationViewModel` production behavior remains unchanged for Candidate A.
- Product view event handlers only forward to `LoadTargetOptionsAsync`, `SelectFileAsync`, and `RegisterAsync`.
- Target option load/re-entry behavior has a test matching the separately approved lifecycle rule.
- No service locator, router, command infrastructure, or view self-composition is introduced.
- `MainWindowViewModel` is not reused by ProductShell.

## E. ProductShell Regression Gates

- `ProductShellViewModelTests` failed 0.
- Navigation item count, order, IDs, and display text remain unchanged.
- Home remains the initial selection.
- Null selection remains ignored.
- Foreign navigation item remains rejected.
- Home maps to `ProductHomeView`.
- DocumentRegistration maps to `ProductDocumentRegistrationView` only after approval.
- DocumentList remains on the existing fallback and `ProductDocumentListView` remains absent.
- `ProductShellWindow.xaml.cs` remains unchanged under Candidate A.

## F. Resource and Copy Gates

- Current baseline before implementation is 64/64 and `Ui.Product.*` 8/8.
- If the three resource candidates are approved, expected result is 67/67 and `Ui.Product.*` 11/11.
- Resource/constant mismatch is 0.
- Existing 56 non-product resource values remain unchanged unless separately approved.
- Exact values for approved new keys are asserted.
- No direct Korean production literal is introduced.
- Existing validation-harness copy is not silently promoted to product copy.
- Target-specific runtime message treatment matches the separate approval.
- `LastRegistrationSummary` diagnostic formats remain Keep deferred and are not shown in the Phase 1B2 product view.

## G. Safety and Scope Gates

- Existing tests deleted: 0.
- App launch: not run.
- OpenFileDialog: not run.
- Manual workflow: not run.
- Screenshot/visual automation: not run.
- ProductShell runtime entry: absent.
- AppServices unchanged unless a later separate batch approves it.
- MainWindow and App startup unchanged.
- ProductDocumentListView absent.
- DB/SQLite/repository/OCR/migration/backup/rollback changes: 0.
- Protected-path internal access: 0.
- Project root runtime artifacts: 0.
- Git stage/commit is outside implementation validation unless separately instructed.

## H. Baseline Comparison

| Metric | Baseline | Required future judgment |
|---|---:|---|
| Full tests | 351 | failed 0; count change explained if tests are added |
| ProductShellViewModel tests | 9 | failed 0; constructor/property tests updated |
| ProductNavigationItemViewModel tests | 8 | unchanged and failed 0 |
| Ui resources/constants | 64/64 | unchanged or approved 67/67 only |
| Ui.Product resources/constants | 8/8 | unchanged or approved 11/11 only |

## I. Current Batch Execution Record

| Execution | Result |
|---|---|
| build | not run |
| DocumentRegistration targeted tests | not run |
| ProductShell targeted tests | not run |
| resource targeted tests | not run |
| full tests | not run |
| app launch | not run |
| OpenFileDialog | not run |
| manual workflow | not run |
| reason | documentation-only Phase 1B2 exact-scope decision batch |
