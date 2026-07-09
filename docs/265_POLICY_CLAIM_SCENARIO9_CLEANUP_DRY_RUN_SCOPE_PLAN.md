# Policy Claim Scenario 9 Cleanup Dry-Run Scope Plan

## 1. Status

SCENARIO9_CLEANUP_DRY_RUN_REPORT_PLAN_ONLY

## 2. Marker

POLICY_CLAIM_SCENARIO9_CLEANUP_DRY_RUN_SCOPE_PLANNED

## 3. Baseline Commit

- `a167867 docs(familyclaimref): review scenario9 cleanup policy`

## 4. Purpose

Scenario 9 cleanup execution 전 dry-run report 범위와 금지 범위를 문서화한다.

이번 문서는 cleanup 실행 문서가 아니다. 파일 삭제, 파일 이동, runtime metadata deletion, runtime attachment deletion, source-controlled file 정리, runtime workflow 실행을 수행하지 않는다.

## 5. Current Policy Baseline

- Scenario 9 cleanup remains deferred.
- `data/claimdoc`: Never cleanup.
- isolated runtime synthetic artifacts: future exact cleanup batch only.
- future cleanup requires dry-run + exact path approval.
- policy review 문서는 cleanup을 실행하지 않았다.
- latest known full test: PASS 331, recorded in `docs/251_POLICY_CLAIM_FINAL_KOREAN_COPY_STRATEGY_RESULT_REVIEW.md`.
- latest known project root artifact count: 0 for `attachments/`, `data/local/`, `runtime_test_document.*`, and root DB/SQLite files.
- DB/SQLite/OCR/repository implementation remains unapproved for this cleanup track.

## 6. Allowed This Batch

- dry-run candidate artifact count.
- exact candidate path documentation, if found in allowed project root classes.
- user approval status 기록.
- cleanup risk classification.
- existing policy baseline review.
- tracked `git grep` read-only inspection over `docs app tests`.
- `git check-ignore` verification for protected local-only paths.

## 7. Forbidden This Batch

- cleanup execution.
- file deletion or file move.
- runtime metadata deletion.
- runtime attachment deletion.
- `data/claimdoc` access.
- `docs/nightwork_*` internal access.
- source-controlled files cleanup.
- `git clean`.
- `git reset`.
- `git checkout`.
- app launch.
- OpenFileDialog.
- manual workflow execution.
- DB/SQLite/OCR/repository implementation.
- code/test/XAML/ViewModel/resource modification.
- git add, stage, or commit.

## 8. Dry-Run Evidence Boundary

The dry-run report may record counts and candidate classes only. It must not turn a candidate into an approved cleanup target.

Exact path approval is required before any future cleanup execution. A committed dry-run report is not deletion approval.

## 9. Protected Paths

| Path or pattern | Policy | Evidence boundary |
|---|---|---|
| `data/claimdoc/` | Never cleanup | ignore check only; no internal access |
| `docs/nightwork_*/` | Keep | ignore check only; no internal access |
| source-controlled `docs/` | Never cleanup | evidence chain |
| source-controlled `app/` and `tests/` | Never cleanup | implementation and validation source |

## 10. Final Marker

POLICY_CLAIM_SCENARIO9_CLEANUP_DRY_RUN_SCOPE_READY
