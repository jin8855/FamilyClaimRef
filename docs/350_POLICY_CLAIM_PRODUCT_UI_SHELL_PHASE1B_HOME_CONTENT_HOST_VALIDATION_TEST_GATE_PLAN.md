# Product UI Shell Phase 1B Home Content Host Validation Test Gate Plan

## A. Status

- Marker: `POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1B_HOME_CONTENT_HOST_VALIDATION_TEST_GATE_PLAN_READY`
- Baseline full tests: PASS 351/351
- Current work type: documentation-only
- Commands in this document are future candidates and were not run in this batch.

## B. Future Validation Commands

### Build

```powershell
dotnet build FamilyClaimRef.sln
```

### Targeted Regression Test

```powershell
dotnet test FamilyClaimRef.sln --filter FullyQualifiedName~ProductShellViewModelTests
```

`ProductHomeViewModelTests` must not be included because ProductHomeViewModel is not part of the selected candidate and that test class does not exist.

### Full Test Suite

```powershell
dotnet test FamilyClaimRef.sln
```

## C. Required Gates

| Gate | Required future result |
|---|---|
| Solution build | succeeds with failed build count 0 |
| ProductHomeView XAML | compiles if the candidate is implemented |
| Targeted ProductShellViewModel tests | pass |
| Full solution tests | failed 0 and result compared with baseline 351 |
| Existing tests | deleted 0 |
| Runtime entry | absent |
| ProductShellWindow startup reference | absent |
| MainWindow | unchanged |
| App.xaml/App.xaml.cs | unchanged |
| AppServices | unchanged |
| Project files | unchanged |
| Resources/constants | remain 64/64 unless separately approved |
| Ui.Product resources/constants | remain 8/8 unless separately approved |
| Product copy | no direct Korean literal; use approved resource |
| Dashboard content | no invented metrics, values, summaries, alerts, or calls to action |
| ProductDocumentRegistrationView | absent |
| ProductDocumentListView | absent |
| DB/SQLite/repository/OCR/migration | none |
| Protected-path access | none |
| App launch/manual workflow/visual automation | not run |

## D. Candidate Consistency Gate

- The future changed-file list must match the four-file candidate in docs/349 exactly.
- ProductHomeViewModel and ProductShellViewModel changes require a new decision; they are not implicit fallback files.
- Any new resource key/value requires separate copy/resource approval before implementation.
- If title-only Home cannot compile without an additional file, stop and revise the exact-scope decision instead of broadening implementation.

## E. Current Execution State

| Execution | Result |
|---|---|
| build | not run |
| targeted tests | not run |
| full tests | not run |
| app launch | not run |
| manual workflow | not run |
| reason | documentation-only Phase 1B1 Home content-host exact-scope decision batch |
