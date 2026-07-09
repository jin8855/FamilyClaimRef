# DB SQLite Architecture Decision Scope Plan

## A. Status

DB_SQLITE_ARCHITECTURE_DECISION_SCOPE_PLAN_ONLY

## B. Marker

POLICY_CLAIM_DB_SQLITE_ARCHITECTURE_DECISION_SCOPE_READY

## C. 기준 Commit

`3a621b2 docs(familyclaimref): document db sqlite ocr repository gates`

## D. 목적

현재 JSON storage baseline을 유지할지, SQLite를 도입할지, repository boundary를 둘지, migration 방향을 어떻게 잡을지 구현 없이 architecture option으로 정리한다.

이 문서는 implementation 문서가 아니다.
SQLite adoption 승인 문서가 아니다.
repository implementation 승인 문서가 아니다.
migration implementation 승인 문서가 아니다.
OCR planning/storage 승인 문서가 아니다.

## E. Current Baseline

| 항목 | 현재 기준 |
|---|---|
| latest known full test | PASS 331 |
| `Ui.*` key count | 56 |
| approved Korean resource copy | 21 applied |
| current storage baseline | JSON storage baseline remains validated |
| `data/claimdoc` | protected / no operational use |
| cleanup dry-run | no root candidates |
| DB/SQLite/OCR/repository implementation allowed now | no |
| repository abstraction implementation allowed now | no |
| migration implementation allowed now | no |
| OCR raw text/candidate storage | not approved, separate privacy/storage planning required |
| diagnostic summary extraction | not approved |
| UI redesign/product UI shell | not approved |

## F. Read-Only Inspection Summary

| Area | Inspection result |
|---|---|
| current storage service boundary | `IDocumentStorageService`, `IPolicyClaimStorageService`, `JsonDocumentStorageService`, `JsonPolicyClaimStorageService` 확인 |
| current JSON storage classes | `JsonFileStore`, `JsonFileEnvelope`, `JsonDocumentStorageService`, `JsonPolicyClaimStorageService` 확인 |
| runtime root relation | `RuntimeRootPaths`가 `data/local` metadata root와 `attachments` root를 runtime root 아래에 구성 |
| repository abstraction | app/tests에서 `Repository` implementation match 없음 |
| SQLite implementation/package | app/tests에서 `SQLite` match 없음 |
| migration implementation | app/tests에서 `Migration` match 없음 |
| storage behavior tests | `JsonPolicyClaimStorageServiceTests`, document workflow integration tests, lifecycle persistence tests 확인 |
| `data/claimdoc` | protected local real-document artifact로 문서화되어 있으며 operational use 없음 |

## G. 포함 후보

- JSON baseline 유지 option
- SQLite adoption option
- repository boundary option
- migration strategy option
- backup/rollback strategy question
- privacy/artifact policy question
- test strategy question

## H. 제외 범위

- DB implementation
- SQLite implementation
- repository implementation
- OCR implementation
- migration implementation
- package reference addition
- schema implementation
- production data access
- `data/claimdoc` access
- cleanup execution
- diagnostic summary extraction
- UI redesign/product UI shell
- build/test execution
- Git staging
- Git commit

## I. Scope Judgment

- architecture planning only
- implementation remains blocked
- current JSON storage baseline remains the default
- storage direction requires explicit user decision
- any future DB/SQLite/repository/migration/OCR work requires a separate exact-scope approval

POLICY_CLAIM_DB_SQLITE_ARCHITECTURE_DECISION_SCOPE_READY
