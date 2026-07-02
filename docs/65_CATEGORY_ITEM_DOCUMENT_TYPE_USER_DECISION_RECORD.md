# CategoryItem Document Type User Decision Record

## A. Goal

이 문서는 `docs/64_CATEGORY_ITEM_DOCUMENT_TYPE_POLICY_DECISION.md`의 Needs Decision Q1~Q11에 대한 사용자 결정 기록이다.

목적은 `CategoryItem`과 document type의 MVP 정책 방향을 확정하고, 이후 storage service interface 설계, JSON schema 초안, C# model 구현 승인 여부를 판단하기 위한 기준을 제공하는 것이다.

이 문서는 구현 문서가 아니다. C# enum/constant/model 구현, CategoryItem storage 구현, JSON 저장 구현, storage service 구현, repository 구현, DB/OCR/metadata/file storage 구현은 수행하지 않는다.

## B. Checked Files / Paths

| 대상 | 확인 목적 | 비고 |
|---|---|---|
| `docs/64_CATEGORY_ITEM_DOCUMENT_TYPE_POLICY_DECISION.md` | Q1~Q11 Needs Decision과 document type 정책 후보 확인 | 읽기 전용 |
| `docs/63_DOCUMENT_STORAGE_STRUCTURE_USER_DECISION_RECORD.md` | `Document`, `PolicyDocument`, `ClaimDocument` 저장 구조 결정 확인 | 읽기 전용 |
| `docs/62_DOCUMENT_STORAGE_STRUCTURE_DECISION.md` | documentType 저장 위치 후보 확인 | 읽기 전용 |
| `docs/61_LOCAL_STORAGE_USER_DECISION_RECORD.md` | JSON file storage와 storage service interface 선설계 결정 확인 | 읽기 전용 |
| `docs/60_LOCAL_STORAGE_STRATEGY_DECISION.md` | 로컬 저장 방식 기준 확인 | 읽기 전용 |
| `docs/59_FILENAME_POLICY_TEST_PROJECT_IMPLEMENTATION_REVIEW.md` | 테스트 프로젝트와 PASS 기록 확인 | 읽기 전용 |
| `app/FamilyClaimRef.App/Services/FileNamePolicyService.cs` | 현재 scope별 documentType allowlist 확인 | 읽기 전용 |
| `tests/FamilyClaimRef.App.Tests/FileNamePolicyServiceTests.cs` | 현재 테스트 케이스 기준 확인 | 읽기 전용 |

## C. User Decision Summary

| ID | Question | Decision | Recorded Direction |
|---|---|---|---|
| Q1 | MVP document type 관리는 Hybrid fixed seed CategoryItem 방향으로 갈 것인가? | Accepted | document type code는 고정 string code로 두고, label/scope/sortOrder/disabledAt은 CategoryItem seed 후보로 문서화한다. |
| Q2 | 사용자 정의 document type 추가/수정/삭제는 MVP 1차에서 제외할 것인가? | Accepted | MVP에서는 고정 seed type만 사용하고 사용자 정의 category 기능은 MVP 이후 확장 후보로 둔다. |
| Q3 | 저장되는 document type 값은 label이 아니라 code로 할 것인가? | Accepted | 저장값은 code이며, label은 화면 표시용이다. |
| Q4 | claim scope와 policy scope의 document type allowlist를 분리할 것인가? | Accepted | claim 문서 type과 policy 문서 type은 scope별로 분리한다. 같은 code라도 scope가 다르면 별도 seed 항목으로 본다. |
| Q5 | CategoryItem seed에는 `code`, `label`, `scope`, `sortOrder`, `disabledAt` 후보를 둘 것인가? | Accepted | seed 후보 필드는 `code`, `label`, `scope`, `sortOrder`, `disabledAt`으로 둔다. |
| Q6 | CategoryItem 자체 JSON 저장 구현은 MVP 1차 storage 구현 이후로 보류할 것인가? | Accepted - Deferred | 우선 fixed seed 기준 문서와 constant 후보만 유지하고 CategoryItem storage 구현은 별도 승인 전까지 진행하지 않는다. |
| Q7 | `FileNamePolicyService` allowlist와 CategoryItem seed 기준은 반드시 일치해야 하는가? | Accepted | allowlist와 seed 기준은 반드시 일치해야 하며, 후속 테스트에서 일치 검증을 자동화 후보로 둔다. |
| Q8 | 현재 코드 allowlist에 없는 document type 후보는 `New Candidate / Needs Implementation`으로 분리할 것인가? | Accepted | 현재 코드 allowlist에 없는 type은 별도 정책 결정과 code patch 전까지 자동 허용하지 않는다. |
| Q9 | `Document.documentType`, `PolicyDocument.documentType`, `ClaimDocument.documentType` 중 source of truth를 어디에 둘 것인가? | Accepted - Domain Link Record Source of Truth | 보험 문서는 `PolicyDocument.documentType`, 청구 문서는 `ClaimDocument.documentType`을 source of truth로 둔다. |
| Q10 | MVP에서는 documentType 중복 저장을 허용할 것인가, 아니면 도메인 연결 record에만 둘 것인가? | Accepted - No Duplicate Storage | documentType은 도메인 연결 record에만 두고 `Document.documentType`은 MVP 1차 persisted field로 두지 않는다. |
| Q11 | document type 변경 시 기존 저장 문서의 처리 정책은 별도 결정으로 보류할 것인가? | Accepted - Deferred | document type 변경/비활성화 시 기존 저장 문서 처리 정책은 별도 결정으로 보류한다. |

## D. Accepted Policy

- MVP document type 관리는 Hybrid fixed seed CategoryItem 방향으로 간다.
- 사용자 정의 document type 추가/수정/삭제는 MVP 1차에서 제외한다.
- 저장값은 label이 아니라 code다.
- label은 화면 표시용이다.
- claim/policy scope allowlist는 분리한다.
- CategoryItem seed 후보 필드는 다음으로 둔다.
  - `code`
  - `label`
  - `scope`
  - `sortOrder`
  - `disabledAt`
- CategoryItem JSON 저장 구현은 MVP 1차 storage 구현 이후로 보류한다.
- `FileNamePolicyService` allowlist와 CategoryItem seed 기준은 반드시 일치해야 한다.
- 현재 코드 allowlist에 없는 type은 `New Candidate / Needs Implementation`으로 분리한다.
- documentType source of truth는 도메인 연결 record에 둔다.
  - `PolicyDocument.documentType`
  - `ClaimDocument.documentType`
- `Document.documentType`은 MVP 1차 persisted field로 두지 않는다.
- documentType 중복 저장은 금지한다.
- document type 변경/비활성화 시 기존 저장 문서 처리 정책은 별도 결정으로 보류한다.

## E. Current Allowlist Baseline

현재 코드 기준 allowlist는 `FileNamePolicyService.cs` 기준이다.

### Claim current allowlist

- `receipt`
- `diagnosis`
- `medicine`
- `visit`
- `admission`
- `surgery`
- `etc`

### Policy current allowlist

- `policy`
- `terms`
- `contract`
- `capture`
- `etc`

### New Candidate / Needs Implementation

- `statement`
- `prescription`
- claim scope `capture`

주의:

- New Candidate는 현재 구현 기준으로 자동 허용하지 않는다.
- New Candidate를 허용하려면 별도 정책 결정, `FileNamePolicyService` patch, 테스트 케이스 갱신이 필요하다.

## F. Still Not Implemented

아래 항목은 아직 구현하지 않았다.

- C# enum/constant 구현 없음
- CategoryItem C# 모델 구현 없음
- CategoryItem JSON 저장 구현 없음
- document type allowlist 코드 수정 없음
- `FileNamePolicyService` 수정 없음
- test code 수정 없음
- JSON 저장 구현 없음
- SQLite DB 생성 없음
- storage service interface 구현 없음
- repository/data access 구현 없음
- DB/OCR/metadata/file storage 구현 없음
- WPF UI/XAML 구현 없음
- navigation 구현 없음
- `attachments/`, `data/local` 내부 파일 생성 없음

## G. Next Decision Needed

다음 항목은 이후 구현 착수 전 별도 결정이 필요하다.

1. storage service interface 설계 범위 결정
2. JSON schema 초안 결정
3. `Document`, `PolicyDocument`, `ClaimDocument` C# model 구현 범위 결정
4. document type seed constant 구현 여부 결정
5. allowlist와 seed 기준 일치 테스트 범위 결정
6. `statement`, `prescription`, claim `capture` 허용 여부 결정
7. document type 변경/비활성화 시 기존 문서 처리 정책 결정

## H. Recommendation

다음 순서로 진행하는 것이 적절하다.

1. storage service interface 설계 문서를 생성한다.
2. 그 다음 JSON schema 초안 문서를 생성한다.
3. 그 다음 C# model 구현 여부를 별도 승인받는다.
4. 그 다음 document type seed constant 구현 여부를 별도 승인받는다.
5. 그 다음 JSON file storage 구현 여부를 별도 승인받는다.

## I. Result

`CATEGORY_ITEM_DOCUMENT_TYPE_USER_DECISION_RECORDED`
