# Migration Backup Rollback Options And Policy

## A. Status

MIGRATION_BACKUP_ROLLBACK_OPTIONS_AND_POLICY_ONLY

## B. Marker

POLICY_CLAIM_MIGRATION_BACKUP_ROLLBACK_OPTIONS_READY

## C. 기준 commit

`81af6c4 docs(familyclaimref): plan ocr boundary privacy storage decision`

## D. Option Matrix

| Option | Description | Implementation impact | Data risk | Rollback complexity | Test impact | Pros | Cons | Recommendation |
|---|---|---|---|---|---|---|---|---|
| No migration, keep current JSON baseline | 현재 JSON source of truth를 유지하고 storage transition을 수행하지 않는다. | none now | low | none | current tests remain primary | 검증된 baseline을 흔들지 않는다. | 향후 전환 요구가 생기면 다시 결정해야 한다. | Current recommendation |
| Backup-only policy for existing JSON files | storage transition 없이 JSON file backup 정책만 future option으로 정리한다. | future backup service or command scope | low to medium | low | backup integrity tests required | 전환 전 안전장치를 정의할 수 있다. | backup 저장 위치/보관/삭제 정책이 필요하다. | Policy planning only |
| Dry-run migration report only | 실제 mutation 없이 migration impact report만 생성한다. | future reporting workflow | low | low | synthetic dry-run tests required | 위험 없이 전환 범위를 볼 수 있다. | report format과 source-of-truth 기준이 필요하다. | Future candidate |
| One-way JSON to SQLite migration | JSON을 SQLite primary storage로 일회성 전환한다. | high future migration work | high | high | migration and rollback tests required | storage query capability를 얻을 수 있다. | 되돌리기 어렵고 privacy/backup surface가 커진다. | Not recommended now |
| Dual-write migration window | 일정 기간 JSON과 future store에 동시에 기록한다. | high future architecture work | high | high | dual-write consistency tests required | 전환 중 비교 검증이 가능하다. | drift와 실패 복구 정책이 복잡하다. | Not now |
| JSON source-of-truth plus SQLite projection rebuild | JSON은 source of truth로 유지하고 SQLite projection은 재생성 가능하게 둔다. | future projection/rebuild work | medium | low to medium | rebuild consistency tests required | irreversible migration을 피한다. | projection drift 검증이 필요하다. | Preferred if SQLite projection is approved |
| SQLite primary with JSON export rollback | SQLite primary 전환 후 rollback은 JSON export로 수행한다. | high future DB/export work | high | high | export/import integrity tests required | rollback artifact를 명시할 수 있다. | export 시점과 부분 실패 처리가 어렵다. | Not now |
| Defer migration until storage direction is approved | storage direction, repository boundary, backup/rollback, synthetic test strategy가 승인될 때까지 migration을 미룬다. | none now | low | none | current tests remain primary | 과잉 구현을 피한다. | future planning debt가 남는다. | Recommended with current baseline |

## E. Recommendation

Current recommendation:

- No migration now; keep current JSON baseline.

Secondary recommendation:

- Define backup/rollback policy before any storage transition implementation.

SQLite migration recommendation:

- Do not implement JSON-to-SQLite migration until SQLite source-of-truth decision, schema versioning, backup/rollback, and synthetic-only tests are approved.

Projection recommendation:

- If SQLite is later used as projection/index, prefer rebuildable projection over irreversible migration.

`data/claimdoc` relation:

- `data/claimdoc` must not be used as migration input or validation fixture.

## F. Backup Policy Questions

- What files are backed up?
- Where are backups stored under approved runtime root?
- Are backups encrypted or plain?
- How long are backups retained?
- Are backups deleted by cleanup policy?
- How is backup integrity verified?
- What happens if backup fails?

## G. Rollback Policy Questions

- What is the rollback trigger?
- Is rollback automatic or manual?
- What exact files are restored?
- How is partial migration detected?
- How is rollback tested with synthetic-only fixtures?
- What is the expected user-visible state after rollback?
- How are failed rollback artifacts preserved for review?

## H. Source-Of-Truth Policy Table

| Future storage direction | Source of truth | Backup required | Rollback path | Approved now |
|---|---|---|---|---|
| current JSON only | JSON files under approved runtime root | existing baseline only | no transition rollback needed | yes, existing baseline |
| JSON + SQLite projection | JSON files | projection rebuild policy required | rebuild projection from JSON | no |
| SQLite primary | SQLite DB | pre-migration JSON backup required | restore JSON or export from SQLite, policy undecided | no |
| dual-write transition | undecided until transition policy is approved | both stores likely require backup | drift-aware rollback policy required | no |
| OCR candidate store | undecided and OCR storage not approved | retention/privacy backup policy required if ever approved | candidate discard/restore policy required | no |

## H-1. Exact Source-Of-Truth Approval Summary

| Future storage direction | Source of truth | Backup required | Rollback path | Approved now |
|---|---|---|---|---|
| current JSON only | JSON | policy candidate only | not needed now | yes, existing baseline |
| JSON + SQLite projection | JSON | yes before projection rebuild if mutation exists | drop/rebuild projection from JSON | no |
| SQLite primary | SQLite | yes before transition | restore pre-migration JSON or validated export | no |
| dual-write transition | not decided | yes | consistency-window rollback | no |
| OCR candidate store | not decided | yes if persisted | remove candidate store or restore backup | no |

## I. Policy Judgment

Migration, backup, and rollback remain decision gates only. Current implementation direction is no migration now and current JSON baseline retention.
