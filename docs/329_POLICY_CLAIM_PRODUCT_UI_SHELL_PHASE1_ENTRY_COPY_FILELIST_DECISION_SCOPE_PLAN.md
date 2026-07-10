# Product UI Shell Phase 1 Entry Copy Filelist Decision Scope Plan

## A. Status

PRODUCT_UI_SHELL_PHASE1_ENTRY_COPY_FILELIST_DECISION_SCOPE_PLAN_ONLY

## B. Marker

POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1_ENTRY_COPY_FILELIST_DECISION_SCOPE_READY

## C. Baseline

- baseline commit: `574af1a docs(familyclaimref): plan product shell phase1 implementation preflight`
- project: `FamilyClaimRef`
- current work type: documentation-only decision candidate planning

## D. Purpose

ProductShell Phase 1 implementation 전에 필요한 entry strategy, exact file list, resource/copy boundary, validation/test gate decision 후보를 구현 없이 정리한다.

이번 문서는 implementation approval 문서가 아니다.

## E. Current Baseline

| Item | Current state |
|---|---|
| ProductShell implementation | not approved |
| `ProductShellWindow` addition | not approved |
| `MainWindow` replacement | not approved |
| App startup change | not approved |
| `Ui.Product.*` addition | not approved |
| product terminology finalization | not approved |
| exact implementation file list | not approved |
| latest known full test | PASS 331 |
| storage source of truth | JSON retained |
| `data/claimdoc` | protected / no operational use |

## F. Included

- entry/startup strategy candidate decision
- exact file list candidate decision
- resource/copy and terminology candidate table
- validation/test gate decision candidate

## G. Explicitly Excluded

- implementation
- `ProductShellWindow` creation
- XAML port
- `MainWindow` replacement
- App startup change
- code/test/resource/project changes
- `Ui.Product.*` addition
- `data/claimdoc` access
- DB/SQLite/repository/OCR/migration work
- cleanup execution
- app launch
- OpenFileDialog execution
- manual workflow execution
- screenshot or visual automation
- diagnostic summary extraction
- git staging or commit

## H. Scope Judgment

This batch is decision candidate planning only.

Implementation remains blocked. Future implementation still requires explicit user approval and a separate exact implementation batch.
