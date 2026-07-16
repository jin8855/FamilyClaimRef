# Product UI Shell Phase 1D2 Guarded Entry Validation Test Gate Plan

## A. Status

- Marker: `POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1D2_GUARDED_ENTRY_VALIDATION_TEST_GATE_PLAN_READY`
- Baseline full solution: PASS `382/382`
- Current batch execution: none
- Manual smoke: separate future approval

## B. Planned Selector Test Inventory

The planned test class is `StartupWindowModeSelectorTests`.

| Planned structure | Discovered cases | Contract |
|---|---:|---|
| `[Fact] Select_null_arguments_returns_main_window` | 1 | null collection returns MainWindow |
| `[Fact] Select_empty_arguments_returns_main_window` | 1 | empty args return MainWindow |
| `[Fact] Select_unknown_arguments_returns_main_window` | 1 | unknown args do not affect startup mode |
| `[Theory] Select_exact_preview_argument_returns_product_shell_preview` with lower-case and case-variant `[InlineData]` | 2 | exact token uses OrdinalIgnoreCase |
| `[Fact] Select_preview_argument_among_unrelated_arguments_returns_product_shell_preview` | 1 | unrelated args do not hide an exact flag |
| `[Fact] Select_duplicate_preview_arguments_returns_product_shell_preview` | 1 | duplicate flags select one preview mode |
| `[Fact] Select_preview_token_prefix_returns_main_window` | 1 | incomplete prefix does not match |
| `[Theory] Select_non_exact_preview_tokens_return_main_window` with suffix and value-assignment `[InlineData]` | 2 | suffix and `=true` forms do not match |
| `[Fact] Select_is_deterministic_and_stateless` | 1 | repeated calls with equal input return equal mode |

Planned discovered selector case count:

`1 + 1 + 1 + 2 + 1 + 1 + 1 + 2 + 1 = 11`

This is a planned count, not an execution result.

## C. Future Commands

```powershell
dotnet build FamilyClaimRef.sln
dotnet test FamilyClaimRef.sln --filter FullyQualifiedName~StartupWindowModeSelectorTests
dotnet test FamilyClaimRef.sln --filter FullyQualifiedName~AppServicesTests
dotnet test FamilyClaimRef.sln --filter FullyQualifiedName~ProductShellViewModelTests
dotnet test FamilyClaimRef.sln --filter FullyQualifiedName~ProductDocumentListViewModelTests
dotnet test FamilyClaimRef.sln --filter FullyQualifiedName~DocumentRegistrationViewModelTests
dotnet test FamilyClaimRef.sln
```

These commands are future candidates only and were not executed in this batch.

## D. Future Automated And Static Gates

| Gate | Evidence type | Required result |
|---|---|---|
| selector exact behavior | selector unit tests | all 11 planned discovered cases PASS or actual justified inventory recorded |
| default mode | selector unit tests | missing/empty/unknown/non-exact args select MainWindow |
| preview mode | selector unit tests | exact case-insensitive token selects ProductShellPreview |
| AppServices call count | focused static diff review | exactly one call in `OnStartup` |
| selected Window construction | focused static diff review | exactly one branch-local Window construction |
| `Application.MainWindow` | focused static diff review | assigned selected Window once |
| `Show` | focused static diff review | selected Window shown exactly once |
| `ShowDialog` | focused static search | zero startup calls |
| dual-window construction | focused static review | zero paths construct both Windows |
| default behavior unchanged | source diff and existing tests | MainWindow branch preserves current graph/DataContext |
| AppServices | exact-scope review | unchanged |
| MainWindow/ProductShellWindow source | exact-scope review | unchanged |
| resources | inventory checks | resources/constants `68/68`, `Ui.Product.*` `12/12` |
| environment/AppContext startup switch | static search | absent |
| startup persistence | static search/review | absent |
| hidden fallback | static review | absent |
| existing tests | test inventory | deleted `0` |
| full solution | full test | compare actual result with PASS `382/382` baseline |

The selector tests do not prove real WPF Window count, `Show`, focus, or process exit. Those remain static source gates until a separately approved manual smoke is run.

## E. Future Manual Smoke Boundary

A manual smoke requires separate approval after exact implementation and commit review.

Candidate evidence:

1. Use the existing guarded absolute runtime-root override with an isolated temporary path.
2. Launch without the preview flag and verify only MainWindow appears.
3. Close it and verify the process exits.
4. Launch with `--product-shell-preview` and verify only ProductShellWindow appears with Home selected.
5. Close it and verify the process exits.
6. Verify no dual launch and no `ShowDialog` behavior.
7. Do not navigate to registration/list, invoke workflow/file picker, or use protected/actual documents.

Current batch results:

- App launch: not run
- Default runtime smoke: not run
- Preview runtime smoke: not run
- Workflow/file picker: not run
- Cleanup: not run
