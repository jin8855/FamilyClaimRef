# Product UI Shell Phase 1B2 Target Runtime Message Validation Test Plan

## A. Status

- Marker: `POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1B2_TARGET_RUNTIME_MESSAGE_VALIDATION_TEST_PLAN_READY`
- Plan type: future implementation validation only
- Current implementation target: 0
- Current command execution: none

## B. Future Validation Commands

Run only after a separate exact implementation approval.

```powershell
dotnet build FamilyClaimRef.sln
dotnet test FamilyClaimRef.sln --filter FullyQualifiedName~DocumentRegistrationViewModelTests
dotnet test FamilyClaimRef.sln --filter FullyQualifiedName~ResourceUiTextProviderTests
dotnet test FamilyClaimRef.sln
```

## C. Future Build and Scope Gates

- Build: PASS, warnings 0, errors 0.
- Exact future file scope: three modified files and one created result document.
- `UiStrings.xaml`: exactly six target-specific value changes.
- `UiTextKeys.cs`: unchanged.
- `DocumentRegistrationViewModel.cs`: unchanged.
- `ProductDocumentRegistrationView.xaml`: unchanged.
- ProductShell source: unchanged.
- MainWindow, App, and `AppServices`: unchanged.
- ProductShell runtime entry: absent.
- `ProductDocumentListView`: absent.

## D. Resource Gates

- `Ui.*` resources/constants remain 67/67.
- `Ui.Product.*` resources/constants remain 11/11.
- Resource/constant mismatch remains 0.
- New keys: 0.
- Deleted keys: 0.
- Renamed keys: 0.
- Target-specific changed values: exactly 6.
- Generic runtime-message changes: 0.
- Each changed value matches the exact six-row table in docs/366.

## E. Test Gates

- `DocumentRegistrationViewModelTests`: failed 0.
- `ResourceUiTextProviderTests`: failed 0.
- Full solution tests: failed 0.
- Existing tests deleted: 0.
- Existing behavioral assertions weakened: 0.
- Full test total is compared with baseline 357.
- Expected total remains 357 unless an evidence-backed test-count change is reported.
- No new test class is required.

## F. Copy and Privacy Gates

- Visible candidate terminology uses `보험 계약`, `청구 건`, and `연결 대상`.
- Raw `policy` and `claim` technical values are not introduced into visible copy.
- Direct Korean C#/XAML literals added: 0.
- Actual personal, insurance, hospital, diagnosis, or claim samples: 0.
- Local user-profile paths: 0.

## G. Runtime and Operation Gates

- App launch: not run.
- ProductShellWindow launch: not run.
- OpenFileDialog: not run.
- Manual workflow: not run.
- Screenshot or visual automation: not run.
- Cleanup: not run.
- DB, SQLite, repository, OCR, and migration work: none.

## H. Current Batch Execution Record

| Command or action | Current result |
|---|---|
| build | not run |
| DocumentRegistration targeted tests | not run |
| resource targeted tests | not run |
| full tests | not run |
| app launch | not run |
| OpenFileDialog | not run |
| manual workflow | not run |
| reason | documentation-only terminology convergence decision batch |
