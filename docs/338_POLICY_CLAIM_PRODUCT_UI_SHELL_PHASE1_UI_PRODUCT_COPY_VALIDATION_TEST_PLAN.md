# Product UI Shell Phase 1 Ui.Product Copy Validation Test Plan

## A. Status

PRODUCT_UI_SHELL_PHASE1_UI_PRODUCT_COPY_VALIDATION_TEST_PLAN_ONLY

## B. Marker

POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1_UI_PRODUCT_COPY_VALIDATION_TEST_PLAN_READY

## C. Baseline

- 기준 commit: `21c51ab docs(familyclaimref): plan product shell phase1 entry copy filelist decisions`
- no code/test/XAML/ViewModel/resource modified by this document
- no `Ui.Product.*` implementation by this document

## D. Future Validation Commands

If `Ui.Product.*` implementation is later approved, run:

- `dotnet build FamilyClaimRef.sln`
- `dotnet test FamilyClaimRef.sln --filter FullyQualifiedName~ResourceUiTextProviderTests`
- `dotnet test FamilyClaimRef.sln`

No validation command is run now because this batch is documentation-only.

## E. Future Validation Targets

1. build succeeds.
2. focused `ResourceUiTextProviderTests` succeeds.
3. full solution test succeeds.
4. known full-test baseline PASS 331 and new result comparison are recorded.
5. `UiStrings.xaml` `Ui.*` key count changes from 56 to 64.
6. `UiTextKeys.cs` `Ui.*` constant count changes from 56 to 64.
7. new `Ui.Product.*` key count is exactly 8.
8. approved key/value exact match is verified.
9. no duplicate resource key exists.
10. no missing constant exists.
11. no orphan constant exists.
12. existing 56 resource values remain unchanged.
13. `Ui.Policy.TargetLabel = 보험 대상` remains unchanged.
14. `Ui.Claim.TargetLabel = 청구 대상` remains unchanged.
15. ProductShell implementation is absent.
16. ProductShellWindow creation is absent.
17. MainWindow replacement is absent.
18. App startup change is absent.
19. data/claimdoc access is absent.

## F. Future Forbidden Validation

- no app launch
- no OpenFileDialog
- no manual workflow
- no screenshot/visual automation
- no DB/SQLite/repository/OCR/migration
- no cleanup execution

## G. Elevated Rerun Rule

Windows SDK user-profile access boundary and actual build/test failure must be separated.

A permitted elevated rerun result may be recorded separately if the same environment boundary appears.

## H. Validation Judgment

- no validation command is run now
- future implementation must include build/test
- this document only defines the validation plan for a separately approved implementation batch

## I. Current Documentation-Only Batch Status

| Execution item | Current batch result |
|---|---|
| build | not run |
| focused `ResourceUiTextProviderTests` | not run |
| full solution test | not run |
| app launch | not run |
| manual workflow | not run |
| screenshot/visual automation | not run |
| reason | documentation-only approved copy table batch |
