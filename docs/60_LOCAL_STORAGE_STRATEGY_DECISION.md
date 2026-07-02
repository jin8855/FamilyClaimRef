# Local Storage Strategy Decision

## 1. Goal

이 문서는 FamilyClaimRef MVP의 로컬 저장 방식 후보를 비교하고, 구현 착수 전 결정이 필요한 항목을 정리하는 전략 결정 문서다.

비교 대상은 JSON file storage, SQLite local DB, Hybrid 방식이다. 이 문서는 구현 문서가 아니며, DB 구현, JSON 저장 구현, SQLite package 추가, repository/service 구현, OCR 저장, metadata 저장, file storage, navigation, WPF UI 구현은 수행하지 않는다.

후속 문서인 `docs/61_LOCAL_STORAGE_USER_DECISION_RECORD.md`에서 Q1~Q8은 `Accepted`로 기록되어 있다. 이 문서는 그 결정의 기준 문서로 다시 생성한 복원본이다.

## 2. Current State

| 항목 | 상태 |
|---|---|
| WPF scaffold | 생성 완료 |
| Target Framework | `net10.0-windows` |
| `FileNamePolicyService` | 구현됨 |
| `FileNamePolicyService` automated test project | 존재 |
| `dotnet build FamilyClaimRef.sln` | PASS 기록 존재 |
| `dotnet test FamilyClaimRef.sln` | PASS 기록 존재 |
| Total tests | 33 |
| DB 구현 | 없음 |
| JSON storage 구현 | 없음 |
| SQLite package | 없음 |
| metadata 저장 구현 | 없음 |
| file storage 구현 | 없음 |
| `attachments/`, `data/local` 내부 실제 파일 생성 | 없음 |

현재 확정된 후속 결정:

- MVP 1차 저장 방식은 JSON file storage다.
- SQLite는 MVP 이후 확장 후보로 보류한다.
- metadata root 후보는 `data/local/`이다.
- actual file root 후보는 `attachments/`이다.
- storage service interface를 먼저 설계한다.
- JSON implementation은 interface 뒤에 붙이는 MVP 1차 구현체 후보다.
- raw `originalFileName` 저장은 MVP에서 보류한다.
- OCR 임시 결과 저장은 MVP에서 보류한다.
- 실제 개인정보 샘플은 사용하지 않는다.

## 3. Storage Candidate Comparison

### 3.1 Candidate 1: JSON file storage

후보 경로:

- `data/local/app-state.json`
- `data/local/family-members.json`
- `data/local/policies.json`
- `data/local/documents.json`
- `data/local/claims.json`

장점:

- MVP에서 구현 부담이 낮다.
- 구조가 단순하고 빠르게 검증할 수 있다.
- 파일 내용이 사람이 읽을 수 있는 형태다.
- 초기 migration 부담이 작다.
- 소규모 개인용 desktop app에는 적합할 수 있다.

단점:

- 데이터가 늘어나면 검색, 필터, 정렬이 약해진다.
- 동시성 제어가 약하다.
- 일부 항목만 갱신하는 partial update가 어색하다.
- 데이터가 커질수록 성능과 안정성 부담이 커진다.
- schema evolution을 수동으로 관리해야 한다.

### 3.2 Candidate 2: SQLite local DB

후보 경로:

- `data/local/familyclaimref.db`

장점:

- 관계형 데이터 관리에 적합하다.
- 검색, 필터, 정렬에 유리하다.
- 청구, 보험, 문서 간 관계를 명확하게 다룰 수 있다.
- 데이터 증가에 JSON보다 안정적으로 대응할 수 있다.
- history와 state 관리에 유리하다.

단점:

- 초기 구현 부담이 커진다.
- SQLite 관련 package 추가가 필요하다.
- migration policy가 필요하다.
- repository 또는 data access layer 설계가 필요하다.
- 초기 MVP에는 과한 구조가 될 수 있다.

### 3.3 Candidate 3: Hybrid

후보 구조:

- metadata: SQLite
- actual files: `attachments/`
- settings/UI preferences: JSON

장점:

- 장기 구조로 가장 자연스럽다.
- 관계형 metadata와 실제 파일 경계를 분리할 수 있다.
- 보험, 청구, 문서, OCR 후보값, 이력 관계를 확장하기 좋다.

단점:

- 초기 MVP 범위로는 크다.
- JSON과 SQLite를 함께 관리해야 한다.
- 테스트 범위가 커진다.
- migration, backup, export 정책을 함께 정해야 한다.

## 4. FamilyClaimRef Domain Evaluation Criteria

| 기준 | 저장 방식에 미치는 영향 |
|---|---|
| 가족 구성원 관리 | `FamilyMember` 생성, 편집, 삭제, 사용 중지 상태가 필요하다. |
| 보험 active/terminated 분리 | `Policy` 상태와 이력 조회가 필요하다. |
| 보험사별 청구 진행 | `ClaimSubmission` 상태와 `ClaimPayment` 연결이 필요하다. |
| 청구 서류와 보험 문서 분리 | `ClaimDocument`와 `PolicyDocument`의 도메인 구분이 필요하다. |
| 단일 물리 `Document` 후보 | 실제 저장 파일과 업무 문서 역할을 분리할 수 있어야 한다. |
| `PolicyDocument` / `ClaimDocument` domain separation | 도메인 명칭과 물리 저장 구조가 분리될 수 있다. |
| OCR 후보값과 사용자 확정값 분리 | `OcrCandidate`는 후보값이며 업무 객체 자동 반영을 피해야 한다. |
| 청구 이력과 상태 관리 | `ClaimCase`, `ClaimSubmission`, `ClaimPayment`, `HistoryItem` 경계가 필요하다. |
| `displayTitle` vs `physicalFileName` | 사용자 표시명과 물리 파일명은 분리되어야 한다. |
| 민감정보 저장 위험 | 가족, 보험, 병원, 청구 정보는 masking, export, 삭제 정책과 연결된다. |
| 향후 검색/필터/이력 확장 | SQLite 또는 Hybrid로 전환 가능한 설계가 필요하다. |

## 5. Recommended Direction

### 5.1 Candidate Recommendation

MVP 첫 저장 방식은 JSON file storage candidate로 시작하는 방향을 추천한다.

단, JSON 구현에 직접 결합하지 않고 storage service interface를 먼저 정의한 뒤 JSON implementation을 붙이는 방식이 적절하다. 이렇게 하면 초기 구현은 작게 유지하면서도 SQLite migration 가능성을 남길 수 있다.

후속 사용자 결정 기록에서는 이 방향이 `Accepted`로 기록되었다.

### 5.2 Recommended Boundary

- 실제 파일은 `attachments/` 하위에 두는 후보를 유지한다.
- metadata는 `data/local/` 하위에 두는 후보를 유지한다.
- MVP에서는 JSON metadata 저장을 후보로 둔다.
- SQLite는 claim history, search, multi-user, data growth 필요가 명확해질 때까지 defer한다.
- OCR temporary result는 MVP에서 저장하지 않는 기존 결정을 유지하는 방향이 안전하다.
- `originalFileName` raw 저장은 MVP에서 defer하는 기존 결정을 유지하는 방향이 안전하다.

### 5.3 Migration Readiness

JSON으로 시작하더라도 다음 원칙을 지키면 SQLite 전환 비용을 줄일 수 있다.

- 화면에서 직접 파일을 읽고 쓰지 않는다.
- `StorageService` 또는 이에 준하는 interface를 먼저 둔다.
- 저장 객체의 id, status, document link는 DB 전환을 고려해 명확히 둔다.
- JSON schema version 후보를 둔다.
- `displayTitle`과 `physicalFileName`을 분리한다.

## 6. Needs Decision

| 번호 | 결정 필요 사항 | 후속 결정 기록 |
|---|---|---|
| Q1 | MVP를 JSON file storage로 시작할 것인가 | Accepted |
| Q2 | SQLite는 MVP 이후로 보류할 것인가 | Accepted |
| Q3 | JSON metadata를 `data/local/` 아래에 저장할 것인가 | Accepted |
| Q4 | actual attachments를 `attachments/` 아래에 저장할 것인가 | Accepted |
| Q5 | storage service interface를 먼저 설계한 뒤 JSON implementation을 붙일 것인가 | Accepted |
| Q6 | OCR temporary result를 MVP에서 저장하지 않는 결정을 유지할 것인가 | Accepted |
| Q7 | raw `originalFileName` 저장을 MVP 이후로 보류할 것인가 | Accepted |
| Q8 | 테스트와 샘플은 dummy data만 사용하고 실제 개인정보 샘플을 금지할 것인가 | Accepted |

후속 결정 기록 문서:

- `docs/61_LOCAL_STORAGE_USER_DECISION_RECORD.md`

## 7. Out of Scope

이번 문서 작성 범위에서 제외한다.

- JSON storage implementation
- SQLite DB creation
- SQLite NuGet package 추가
- repository 구현
- data access layer 구현
- migration 구현
- schema 구현
- OCR storage 구현
- metadata storage 구현
- file copy/storage 구현
- WPF UI 구현
- XAML 수정
- navigation 구현
- 실제 개인정보 샘플 작성

## 8. Risks

| 위험 | 설명 | 완화 후보 |
|---|---|---|
| JSON relational query 한계 | 보험, 문서, 청구 관계가 복잡해지면 JSON 조회가 불편해질 수 있다. | storage interface와 id/link 규칙을 먼저 둔다. |
| JSON integrity 한계 | 참조 무결성을 코드로 직접 관리해야 한다. | delete/disable policy와 validation rule을 문서화한다. |
| SQLite 초기 부담 | DB, package, migration, repository가 MVP 속도를 늦출 수 있다. | MVP에서는 defer하고 전환 기준을 둔다. |
| UI 선행 위험 | storage model 없이 UI부터 구현하면 화면과 데이터 경계가 어긋날 수 있다. | storage decision record 후 interface 설계를 진행한다. |
| Document 경계 위험 | `Document`, `PolicyDocument`, `ClaimDocument`의 물리/도메인 분리가 확정되지 않았다. | 구현 전 문서 저장 구조를 별도 결정한다. |
| 민감정보 위험 | 가족, 보험, 청구 데이터는 masking/export/delete 정책과 연결된다. | dummy data 원칙과 민감정보 저장 기준을 유지한다. |

## 9. Recommendation Sequence

1. 이 문서에서 JSON, SQLite, Hybrid 후보를 비교한다.
2. 사용자 결정은 `docs/61_LOCAL_STORAGE_USER_DECISION_RECORD.md`에 기록한다.
3. `Document` / `PolicyDocument` / `ClaimDocument` storage structure를 결정한다.
4. storage service interface를 설계한다.
5. JSON storage implementation은 별도 승인 후 진행한다.

## 10. Result

`LOCAL_STORAGE_STRATEGY_DECISION_DRAFTED`
