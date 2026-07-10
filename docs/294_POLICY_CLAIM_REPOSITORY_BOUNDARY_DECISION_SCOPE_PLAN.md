# Repository Boundary Decision Scope Plan

## A. Status

REPOSITORY_BOUNDARY_DECISION_SCOPE_PLAN_ONLY

## B. Marker

POLICY_CLAIM_REPOSITORY_BOUNDARY_DECISION_SCOPE_READY

## C. 기준 commit

`9c5fca4 docs(familyclaimref): plan db sqlite architecture decision`

## D. 목적

현 JSON storage service boundary를 유지할지, repository abstraction을 나중에 도입할지, 도입한다면 어떤 ownership으로 나눌지를 구현 없이 정리한다.

이 문서는 repository boundary decision planning 문서다. Repository interface/class 구현 문서가 아니며, DB/SQLite adoption 승인 문서도 아니다.

## E. Current Baseline

| 항목 | 현재 기준 |
|---|---|
| latest known full test | PASS 331 |
| current storage baseline | JSON source of truth |
| current storage interfaces | `IDocumentStorageService`, `IPolicyClaimStorageService` |
| repository implementation | not present / not approved |
| SQLite implementation | not present / not approved |
| migration implementation | not present / not approved |
| OCR storage | not approved |
| `data/claimdoc` | protected / no operational use |
| cleanup execution | not approved |
| diagnostic summary extraction | not approved |
| UI redesign/product UI shell | not approved |

## F. Current Service Boundary Review

| Boundary | Current ownership | Repository decision relevance |
|---|---|---|
| `IDocumentStorageService` | `DocumentRecord`, `PolicyDocumentRecord`, `ClaimDocumentRecord` 저장/조회/사용 중지 | 현재 document storage boundary로 유지한다. |
| `IPolicyClaimStorageService` | `PolicyRecord`, `ClaimRecord` 저장/조회/사용 중지 및 존재 확인 | 현재 policy/claim storage boundary로 유지한다. |
| `JsonDocumentStorageService` | document metadata JSON storage 구현 | repository 후보가 아니라 current concrete storage다. |
| `JsonPolicyClaimStorageService` | policy/claim JSON storage 구현 | repository 후보가 아니라 current concrete storage다. |
| `JsonFileStore` | schema envelope 기반 JSON file load/save | low-level JSON persistence helper다. |
| `DocumentRegistrationWorkflow` | attachment import, document metadata 생성, policy/claim link를 작업 단위로 조합 | workflow behavior는 repository로 이동하지 않는다. |

Read-only inspection 기준으로 `app/tests`에는 `Repository` implementation match가 없고, `SQLite` 및 `Migration` implementation match도 없다. `docs`에는 future planning과 gate 문서 언급만 존재한다.

## G. 포함 후보

- current service boundary review
- repository abstraction 필요성 판단
- repository ownership candidate
- interface granularity candidate
- future contract test strategy
- future migration compatibility question

## H. 제외 범위

- repository interface/class implementation
- storage service rename/refactor
- SQLite implementation
- migration implementation
- OCR implementation/storage
- package reference addition
- `data/claimdoc` access
- cleanup execution
- UI redesign/product UI shell

## I. Scope Judgment

- repository boundary planning only
- implementation remains blocked
- current recommendation is to keep existing storage service boundary until storage direction is approved
- future repository work requires explicit user approval, separate implementation scope, and synthetic-only validation plan

