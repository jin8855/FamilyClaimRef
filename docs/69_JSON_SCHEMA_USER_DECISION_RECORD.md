# JSON Schema User Decision Record

## A. Goal

이 문서는 `docs/68_JSON_SCHEMA_DRAFT.md`의 Needs Decision Q1~Q15에 대한 사용자 결정 기록이다.

목적은 FamilyClaimRef MVP의 JSON schema 방향을 확정하고, 이후 C# model/interface 구현 범위 결정과 JSON file storage 구현 승인 여부를 판단하기 위한 기준을 제공하는 것이다.

이 문서는 구현 문서가 아니다. 실제 JSON file 생성, C# model/interface 구현, JSON storage 구현, storage service 구현, repository 구현, DB/OCR/metadata/file storage 구현은 수행하지 않는다.

## B. Checked Files / Paths

| 대상 | 확인 목적 | 비고 |
|---|---|---|
| `docs/68_JSON_SCHEMA_DRAFT.md` | Q1~Q15 Needs Decision과 JSON schema 후보 확인 | 읽기 전용 |
| `docs/67_STORAGE_SERVICE_INTERFACE_USER_DECISION_RECORD.md` | storage service interface 방향 확인 | 읽기 전용 |
| `docs/65_CATEGORY_ITEM_DOCUMENT_TYPE_USER_DECISION_RECORD.md` | document type source of truth와 allowlist 기준 확인 | 읽기 전용 |
| `docs/63_DOCUMENT_STORAGE_STRUCTURE_USER_DECISION_RECORD.md` | `Document`, `PolicyDocument`, `ClaimDocument` 저장 구조 결정 확인 | 읽기 전용 |
| `docs/61_LOCAL_STORAGE_USER_DECISION_RECORD.md` | local JSON storage 방향 확인 | 읽기 전용 |
| `docs/59_FILENAME_POLICY_TEST_PROJECT_IMPLEMENTATION_REVIEW.md` | test project와 `FileNamePolicyService` 검토 상태 확인 | 읽기 전용 |
| `app/FamilyClaimRef.App/Services/FileNamePolicyService.cs` | 현재 document type allowlist 기준 확인 | 읽기 전용 |
| `tests/FamilyClaimRef.App.Tests/FileNamePolicyServiceTests.cs` | 현재 테스트 기준 확인 | 읽기 전용 |

## C. User Decision Summary

| ID | Question | Decision | Recorded Direction |
|---|---|---|---|
| Q1 | JSON files를 `documents.json`, `policy-documents.json`, `claim-documents.json`으로 분리할 것인가? | Accepted | JSON metadata file은 `data/local/documents.json`, `data/local/policy-documents.json`, `data/local/claim-documents.json` 3개 분리 구조로 간다. 실제 file 생성은 별도 구현 승인 전까지 하지 않는다. |
| Q2 | 각 JSON file은 envelope + items 구조를 사용할 것인가? | Accepted | 각 JSON file은 `envelope + items` 구조를 사용한다. 단순 배열만 저장하는 구조는 사용하지 않는다. |
| Q3 | 각 JSON file에 `schemaVersion`을 둘 것인가? | Accepted | 각 JSON file envelope에는 `schemaVersion`을 둔다. 초기값은 `1` 후보로 둔다. |
| Q4 | 각 JSON envelope에 `savedAt`을 둘 것인가? | Accepted | 각 JSON file envelope에는 `savedAt`을 둔다. `savedAt`은 UTC ISO-8601 string 후보로 둔다. |
| Q5 | `DocumentRecord`는 `id`, `physicalFileName`, `displayTitle`, `extension`, `relativePath`, `createdAt`, `updatedAt`, `disabledAt`만 둘 것인가? | Accepted | `DocumentRecord` MVP 1차 field는 해당 8개 field로 제한한다. |
| Q6 | `DocumentRecord.documentType`을 제외할 것인가? | Accepted | `DocumentRecord.documentType`은 제외한다. documentType source of truth는 domain link record에 둔다. |
| Q7 | `DocumentRecord.originalFileName`을 제외할 것인가? | Accepted | raw `originalFileName`은 저장하지 않는다. 원본 파일명은 민감정보 유입 위험이 있으므로 MVP에서 제외한다. |
| Q8 | `isDisabled`는 저장하지 않고 파생 상태로 둘 것인가? | Accepted | `isDisabled`는 저장하지 않는다. persisted source of truth는 `disabledAt`이며, `disabledAt != null`에서 파생한다. |
| Q9 | `PolicyDocumentRecord.documentType`을 source of truth로 둘 것인가? | Accepted | 보험 문서 documentType source of truth는 `PolicyDocumentRecord.documentType`이다. 저장값은 label이 아니라 code이다. |
| Q10 | `ClaimDocumentRecord.documentType`을 source of truth로 둘 것인가? | Accepted | 청구 문서 documentType source of truth는 `ClaimDocumentRecord.documentType`이다. 저장값은 label이 아니라 code이다. |
| Q11 | `ocrConfirmedFieldsSnapshot`은 별도 결정 전까지 제외할 것인가? | Accepted - Deferred | `ocrConfirmedFieldsSnapshot`은 별도 결정 전까지 제외한다. OCR temporary result와 사용자 확정 OCR 값 snapshot 저장 여부는 후속 정책에서 결정한다. |
| Q12 | `memo`는 MVP 1차에서 제외할 것인가? | Accepted - Excluded for MVP 1st | `memo`는 MVP 1차에서 제외한다. memo/tag/history memo 정책은 후속 문서에서 별도 결정한다. |
| Q13 | reference validation 세부 규칙은 JSON implementation design doc으로 미룰 것인가? | Accepted - Deferred | 이번 결정에서는 id-based reference 방향만 유지한다. `documentId`, `policyId`, `claimId` validation 구현 세부 규칙은 JSON implementation design 문서에서 결정한다. |
| Q14 | schema migration은 MVP 1차에서 구현하지 않고 deferred로 둘 것인가? | Accepted - Deferred | `schemaVersion`은 두되, schema migration 구현은 MVP 1차에서 하지 않는다. schemaVersion 불일치와 load failure policy는 후속 설계에서 결정한다. |
| Q15 | 이번 단계에서는 실제 JSON file을 생성하지 않는가? | Accepted | 이번 단계에서는 실제 JSON file을 생성하지 않는다. `data/local/*.json` file은 별도 storage implementation 승인 후에만 생성한다. |

## D. Accepted JSON Schema Direction

- JSON metadata file은 3개 분리 구조로 둔다.
  - `documents.json`
  - `policy-documents.json`
  - `claim-documents.json`
- 각 JSON file은 `envelope + items` 구조를 사용한다.
- 각 envelope에는 `schemaVersion`, `savedAt`을 포함한다.
- 초기 `schemaVersion`은 `1` 후보로 둔다.
- `savedAt`은 UTC ISO-8601 string 후보로 둔다.
- `DocumentRecord` field는 다음으로 제한한다.
  - `id`
  - `physicalFileName`
  - `displayTitle`
  - `extension`
  - `relativePath`
  - `createdAt`
  - `updatedAt`
  - `disabledAt`
- `DocumentRecord.documentType`은 제외한다.
- `DocumentRecord.originalFileName`은 제외한다.
- `isDisabled`는 제외하고 `disabledAt != null`에서 파생한다.
- `PolicyDocumentRecord.documentType`은 보험 문서 documentType source of truth이다.
- `ClaimDocumentRecord.documentType`은 청구 문서 documentType source of truth이다.
- `ocrConfirmedFieldsSnapshot`은 별도 결정으로 보류한다.
- `memo`는 MVP 1차에서 제외한다.
- reference validation 세부 규칙은 JSON implementation design으로 보류한다.
- schema migration은 MVP 1차에서 구현하지 않는다.
- 실제 JSON file은 이번 단계에서 생성하지 않는다.

## E. Current Allowlist Baseline

현재 코드 기준 allowlist는 `FileNamePolicyService` 기준과 일치해야 한다.

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

- New Candidate는 현재 schema에서 자동 허용하지 않는다.
- New Candidate를 허용하려면 별도 정책 결정, `FileNamePolicyService` patch, 테스트 갱신이 필요하다.

## F. Still Not Implemented

아래 항목은 아직 구현하지 않았다.

- 실제 JSON file 생성 없음
- C# model 구현 없음
- C# interface 구현 없음
- JSON storage 구현 없음
- SQLite DB 생성 없음
- SQLite package 추가 없음
- repository/data access/migration 구현 없음
- CategoryItem 구현 없음
- CategoryItem JSON 저장 구현 없음
- storage service 구현 없음
- actual file copy/storage 구현 없음
- `attachments/` 내부 파일 생성 없음
- `data/local/` 내부 파일 생성 없음
- DB/OCR/metadata/file storage 구현 없음
- WPF UI/XAML/navigation 구현 없음
- test code/test project/package 작업 없음
- 실제 개인정보 샘플 없음

## G. Next Decision Needed

다음 항목은 후속 결정 후보이다.

1. C# model/interface 구현 범위 결정
2. `DocumentRecord`, `PolicyDocumentRecord`, `ClaimDocumentRecord` C# record/class 구현 여부 결정
3. `DocumentDraft`, `PolicyDocumentDraft`, `ClaimDocumentDraft` 구현 여부 결정
4. document type seed constant 구현 여부 결정
5. allowlist와 seed 기준 일치 테스트 범위 결정
6. JSON file storage implementation 범위 결정
7. reference validation 세부 구현 범위 결정
8. schema migration/load failure policy 결정

## H. Recommendation

다음 순서로 진행하는 것을 추천한다.

1. C# model/interface 구현 범위 결정 문서를 생성한다.
2. 그 다음 document type seed constant 구현 여부를 별도 승인받는다.
3. 그 다음 allowlist/seed consistency test를 설계한다.
4. 그 다음 JSON file storage implementation을 설계한다.
5. 그 다음 실제 JSON storage 구현 여부를 별도 승인받는다.

## I. Result

`JSON_SCHEMA_USER_DECISION_RECORDED`
