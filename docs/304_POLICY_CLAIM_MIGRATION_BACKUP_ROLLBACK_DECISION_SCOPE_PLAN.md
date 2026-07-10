# Migration Backup Rollback Decision Scope Plan

## A. Status

MIGRATION_BACKUP_ROLLBACK_DECISION_SCOPE_PLAN_ONLY

## B. Marker

POLICY_CLAIM_MIGRATION_BACKUP_ROLLBACK_DECISION_SCOPE_READY

## C. 기준 commit

`81af6c4 docs(familyclaimref): plan ocr boundary privacy storage decision`

## D. 목적

향후 JSON 유지, SQLite projection, SQLite primary, repository boundary 변경이 승인될 경우 필요한 migration / backup / rollback decision gate를 구현 없이 정리한다.

이 문서는 migration / backup / rollback decision planning 문서다. Migration implementation, backup/rollback implementation, SQLite adoption, repository implementation, OCR storage를 승인하지 않는다.

## E. Current Baseline

| 항목 | 현재 기준 |
|---|---|
| latest known full test | PASS 331 |
| current storage baseline | JSON source of truth |
| SQLite implementation | 승인 없음 |
| repository implementation | 승인 없음 |
| migration implementation | 승인 없음 |
| backup/rollback implementation | 승인 없음 |
| OCR implementation/storage | 승인 없음 |
| `data/claimdoc` | protected / no operational use |
| cleanup execution | 승인 없음 |
| diagnostic summary extraction | 승인 없음 |
| UI redesign/product UI shell | 승인 없음 |

## E-1. Exact Approval Baseline

| item | approval |
|---|---|
| migration implementation approved | no |
| backup/rollback implementation approved | no |
| SQLite implementation approved | no |
| repository implementation approved | no |
| OCR implementation/storage approved | no |
| data/claimdoc protected / no operational use | yes |

## F. Read-Only Inspection Summary

| 대상 | 확인 결과 |
|---|---|
| `JsonFileEnvelope` | `SchemaVersion`, `SavedAt`, `Items` 구조가 존재한다. |
| `JsonFileStore` | JSON envelope load/save helper로 `DefaultSchemaVersion = 1`을 사용한다. |
| `JsonDocumentStorageService` | document metadata와 document link JSON storage 구현이다. |
| `JsonPolicyClaimStorageService` | policy/claim JSON storage 구현이다. |
| `RuntimeRootPaths` | runtime root 아래 `data/local`과 `attachments` 경로를 구성한다. |
| `Migration` search | app/tests 구현 match 없음, docs planning mention만 확인된다. |
| `Backup` search | app/tests/docs 기준 구현 match 없음 |
| `Rollback` search | document registration workflow rollback은 있으나 migration/backup rollback 구현은 아니다. |
| `SQLite` search | app/tests 구현 match 없음, docs planning mention만 확인된다. |
| `Schema` / `Version` search | JSON envelope schema-like version concept가 확인된다. |

## G. 포함 후보

- migration trigger conditions
- source-of-truth transition policy
- backup strategy options
- rollback strategy options
- schema/envelope versioning question
- synthetic-only migration test fixture policy
- runtime root artifact policy
- no `data/claimdoc` migration input policy

## H. 제외 범위

- migration implementation
- backup/rollback implementation
- DB/SQLite/repository/OCR implementation
- package reference addition
- JSON storage replacement
- DB file creation
- production data access
- `data/claimdoc` access
- cleanup execution
- app launch/manual workflow

## I. Scope Judgment

- planning only
- implementation remains blocked
- migration/backup/rollback requires explicit user decision and exact implementation scope
- current JSON baseline remains the source of truth until a separate storage direction decision changes it
