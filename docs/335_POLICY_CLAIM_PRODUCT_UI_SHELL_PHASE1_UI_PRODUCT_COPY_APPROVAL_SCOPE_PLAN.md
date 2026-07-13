# Product UI Shell Phase 1 Ui.Product Copy Approval Scope Plan

## A. Status

PRODUCT_UI_SHELL_PHASE1_UI_PRODUCT_COPY_APPROVAL_SCOPE_PLAN_ONLY

## B. Marker

POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1_UI_PRODUCT_COPY_APPROVAL_SCOPE_READY

## C. Baseline

- 기준 commit: `21c51ab docs(familyclaimref): plan product shell phase1 entry copy filelist decisions`
- 사용자 결정: ChatGPT 추천안을 그대로 승인
- 작업 유형: documentation-only approved table batch

## C-1. Task ID

POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1_UI_PRODUCT_COPY_APPROVED_TABLE_DOCS_BATCH

## C-2. Exact Created File List

- `docs/335_POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1_UI_PRODUCT_COPY_APPROVAL_SCOPE_PLAN.md`
- `docs/336_POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1_UI_PRODUCT_APPROVED_VALUE_TABLE.md`
- `docs/337_POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1_UI_PRODUCT_COPY_IMPLEMENTATION_PLAN.md`
- `docs/338_POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1_UI_PRODUCT_COPY_VALIDATION_TEST_PLAN.md`
- `docs/339_POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1_UI_PRODUCT_COPY_COMMIT_CANDIDATE_REVIEW.md`

## C-3. Approval Count Summary

| Count item | Value |
|---|---:|
| approved terminology rows | 7 |
| approved `Ui.Product.*` value rows | 8 |
| implementation target now | 0 |
| source implementation target now | 0 |

## D. Purpose

ProductShell Phase 1에 필요한 `Ui.Product.*` key/value와 product terminology 승인 범위를 구현 없이 문서화한다.

이번 문서는 승인된 copy/value table을 기록하는 문서이며, source/resource/test 변경 승인 문서가 아니다.

## E. Current Baseline

| Item | Current state |
|---|---|
| existing `Ui.*` key count | 56 |
| approved Korean resource copy applied | 21 |
| `Ui.Product.*` addition | not implemented |
| ProductShell implementation | not approved |
| ProductShellWindow addition | not approved |
| MainWindow replacement | not approved |
| App startup change | not approved |
| exact implementation file list | not approved |
| latest known full test | PASS 331 |
| data/claimdoc | protected / no operational use |

## F. Included

- approved product terminology table
- approved `Ui.Product.*` value table
- future implementation candidate boundary
- future validation/test impact

## G. Excluded

- implementation
- `UiStrings.xaml` modification
- `UiTextKeys.cs` modification
- `ResourceUiTextProviderTests` modification
- ProductShellWindow creation
- XAML port
- MainWindow replacement
- App startup change
- DB/SQLite/repository/OCR/migration work
- data/claimdoc access

## H. Scope Judgment

- approved table docs only
- source/resource/test implementation remains blocked
- future implementation requires separate exact-file-list batch

## I. ProductShell Phase 1 Relation

- 이번 배치는 ProductShell Phase 1 화면 구현이 아니라 copy contract 승인 문서 배치다.
- current `MainWindow` remains the validation harness.
- validation harness is not promoted to a product screen.
- approved copy does not authorize ProductShell, ProductShellWindow, navigation, product view, XAML port, or runtime entry implementation.

## J. Explicit Approval Matrix

| Approval item | Approved |
|---|---|
| ProductShell implementation approved | no |
| ProductShellWindow creation approved | no |
| MainWindow replacement approved | no |
| App startup change approved | no |
| `Ui.Product.*` source addition approved for this batch | no |
| exact ProductShell implementation file list approved | no |

## K. Protection And Execution State

- data/claimdoc: protected / no operational use / not accessed
- docs/nightwork_*: protected / not accessed
- build/test: not run, documentation-only approved copy table batch
- git add/stage: not run
- commit: not run
