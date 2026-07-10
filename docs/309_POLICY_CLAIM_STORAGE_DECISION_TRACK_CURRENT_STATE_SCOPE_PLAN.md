# Policy Claim Storage Decision Track Current State Scope Plan

## A. Status

STORAGE_DECISION_TRACK_CURRENT_STATE_SCOPE_PLAN_ONLY

## B. Marker

POLICY_CLAIM_STORAGE_DECISION_TRACK_CURRENT_STATE_SCOPE_READY

## C. 기준 Commit

`6a2f67c docs(familyclaimref): plan migration backup rollback decision`

## D. 목적

DB/SQLite architecture, repository boundary, OCR boundary/privacy/storage, migration/backup/rollback planning track의 current-state closure 범위를 정리한다.

이 문서는 구현 문서가 아니다. 현재까지의 storage decision track을 한 번 닫고, 어떤 구현도 승인되지 않았다는 상태를 명확히 기록한다.

## E. Included Tracks

| Track | Included reason |
|---|---|
| DB/SQLite architecture decision | JSON source of truth 유지와 SQLite 미승인 상태 확인 |
| repository boundary decision | 기존 storage service boundary 유지 상태 확인 |
| OCR boundary/privacy/storage decision | OCR implementation/storage 미승인 상태 확인 |
| migration/backup/rollback decision | No migration now; keep current JSON baseline 상태 확인 |

## F. Excluded

- implementation
- package reference addition
- DB file creation
- JSON storage replacement
- `data/claimdoc` access
- cleanup execution
- diagnostic summary extraction
- UI redesign/product UI shell

## G. Baseline Summary

| 항목 | 현재 상태 |
|---|---|
| current storage baseline | existing JSON baseline |
| current source of truth | JSON source of truth |
| latest known full test | PASS 331 |
| `data/claimdoc` | protected / no operational use |
| cleanup dry-run | no project root candidates |
| diagnostic summary formats | Keep deferred |
| DB/SQLite implementation | not approved |
| repository implementation | not approved |
| migration implementation | not approved |
| backup/rollback implementation | not approved |
| OCR implementation/storage | not approved |
| UI redesign/product UI shell | not approved |

## H. Scope Judgment

- current-state closure only
- implementation remains blocked
- no storage direction change is approved by this document
- existing JSON baseline remains the safest current state
- explicit user approval is required before any implementation track starts

## I. Next State

The safest next state is stop/handoff unless the user explicitly selects one planning-only track.
