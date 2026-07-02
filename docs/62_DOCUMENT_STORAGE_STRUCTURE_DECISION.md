# Document Storage Structure Decision

## A. Goal

이 문서는 `Document`, `PolicyDocument`, `ClaimDocument`의 저장 책임을 분리하기 위한 결정 문서다.

FamilyClaimRef MVP의 1차 저장 방식은 JSON file storage로 확정되었으므로, JSON file storage 구현 전에 문서 metadata와 도메인 연결 record의 경계를 정리한다.

이 문서는 구현 문서가 아니다. C# 모델 구현, JSON 저장 구현, SQLite DB 생성, storage service 구현, repository 구현, OCR 저장, metadata 저장, file copy/storage 구현, WPF UI/XAML 구현, navigation 구현은 수행하지 않는다.

## B. Current State

| 항목 | 상태 |
|---|---|
| WPF scaffold | 생성 완료 |
| Target Framework | `net10.0-windows` |
| `FileNamePolicyService` | 구현됨 |
| `FileNamePolicyService` automated test | PASS 기록 존재 |
| MVP 1차 저장 방식 | JSON file storage로 결정됨 |
| SQLite | MVP 이후 확장 후보로 보류 |
| metadata root 후보 | `data/local/` |
| actual file root 후보 | `attachments/` |
| `Document` / `PolicyDocument` / `ClaimDocument` 모델 구현 | 없음 또는 후보 상태 |
| JSON metadata 파일 생성 | 없음 |
| `attachments/`, `data/local` 내부 파일 생성 | 없음 |
| storage service interface 구현 | 없음 |

검토 시점에 `docs/60_LOCAL_STORAGE_STRATEGY_DECISION.md`는 현재 파일 목록에서 확인되지 않았고, 사용자 결정 기록인 `docs/61_LOCAL_STORAGE_USER_DECISION_RECORD.md`의 Accepted 기준을 현재 확정 기준으로 참조했다.

후속 작업으로 `docs/60_LOCAL_STORAGE_STRATEGY_DECISION.md`가 복구되었으며, `docs/61_LOCAL_STORAGE_USER_DECISION_RECORD.md`의 Accepted 결정과 정합성을 유지한다.

## C. Terminology Baseline

### Document

`Document`는 실제 첨부 파일의 공통 metadata를 나타내는 단일 물리 문서 record 후보다.

역할 후보:

- `physicalFileName`
- `displayTitle`
- `documentType`
- `extension`
- `createdAt`
- `disabledAt` 또는 `isDisabled`
- 실제 파일 root 기준 상대 경로 후보
- raw `originalFileName`은 MVP에서 저장하지 않음

주의:

- `Document`는 보험 문서인지 청구 문서인지 자체적으로 업무 의미를 모두 갖지 않는다.
- `Document`는 실제 파일 저장 공통 record에 가깝다.
- `Document`에는 실제 파일 metadata를 모으고, 도메인 연결 의미는 별도 record가 담당한다.

### PolicyDocument

`PolicyDocument`는 보험 계약, 증권, 약관 등 보험 기준 문서와 `Document`를 연결하는 도메인 관계 record 후보다.

역할 후보:

- `policyId`
- `documentId`
- 보험 문서용 `documentType`
- 사용 중지 여부
- 등록일 후보
- 메모 후보

주의:

- 실제 파일 metadata를 중복 저장하지 않는다.
- `Document`를 참조한다.
- 보험 기준 문서의 업무 의미는 `PolicyDocument`가 가진다.

### ClaimDocument

`ClaimDocument`는 청구 건에 필요한 영수증, 진료비 세부내역서, 진단서, 처방전, 기타 문서와 `Document`를 연결하는 도메인 관계 record 후보다.

역할 후보:

- `claimId`
- `documentId`
- 청구 문서용 `documentType`
- OCR 후보값과 사용자 확정값 연결 후보
- 청구 제출 여부 또는 사용 여부 후보

주의:

- 실제 파일 metadata를 중복 저장하지 않는다.
- `Document`를 참조한다.
- OCR 임시 결과 자체 저장은 MVP에서 보류한다.
- 사용자 확정 OCR 값 snapshot 저장 여부는 별도 결정으로 둔다.

## D. Candidate Storage Shape

JSON file storage 기준으로 아래 구조를 후보로 둔다.

분리 파일 후보:

```text
data/local/documents.json
data/local/policy-documents.json
data/local/claim-documents.json
```

단일 파일 후보:

```text
data/local/documents.json
```

### Candidate 1: 분리 파일 구조

파일 후보:

- `documents.json`
- `policy-documents.json`
- `claim-documents.json`

장점:

- 공통 file metadata와 업무 연결 record가 분리된다.
- SQLite 전환 시 테이블 구조로 옮기기 쉽다.
- Policy/Claim 문서 책임이 명확하다.
- `Document`와 도메인 연결 record의 책임을 문서 구조에서 바로 확인할 수 있다.

단점:

- JSON 파일이 여러 개로 늘어난다.
- 저장/로드 시 reference 정합성 검증이 필요하다.
- 부분 저장 실패 시 파일 간 불일치가 생길 수 있다.

### Candidate 2: 단일 `documents.json` 내 nested 구조

구조 후보:

- `documents`
- `policyDocuments`
- `claimDocuments`

장점:

- MVP 구현이 단순하다.
- 한 파일에서 문서 관련 metadata를 한 번에 볼 수 있다.
- 파일 간 reference 불일치 위험이 상대적으로 작다.

단점:

- 파일이 커질 수 있다.
- 부분 업데이트와 충돌 처리가 불리하다.
- SQLite 전환 시 분리 작업이 필요하다.
- `Document`, `PolicyDocument`, `ClaimDocument`의 물리 경계가 흐려질 수 있다.

## E. Recommended Direction

### Candidate Recommendation

MVP 1차에서는 문서 관련 metadata를 논리적으로 `Document`, `PolicyDocument`, `ClaimDocument`로 분리하는 방향을 후보로 둔다.

JSON 물리 파일은 초기에는 단일 `documents.json` 또는 분리 파일 중 선택이 필요하다. 장기 SQLite 전환 가능성을 고려하면 분리 파일 구조가 더 낫다.

단, 구현 부담을 낮추기 위해 storage service interface 뒤에서 파일 단위를 숨기는 방향이 적절하다. 외부에서는 `DocumentStorageService` 또는 유사 interface를 통해 접근하게 하고, UI/ViewModel이 JSON 파일 구조를 직접 알지 않게 한다.

추천 후보:

- 실제 파일 metadata는 `Document`에만 둔다.
- `PolicyDocument` / `ClaimDocument`에는 `documentId` 참조를 둔다.
- `PolicyDocument`는 `policyId + documentId` 연결 record로 둔다.
- `ClaimDocument`는 `claimId + documentId` 연결 record로 둔다.
- raw `originalFileName`은 저장하지 않는다.
- `displayTitle`은 앱 내부 표시용으로 허용한다.
- `physicalFileName`은 민감정보 없는 파일명 정책을 따른다.
- 삭제는 실제 삭제보다 사용 중지/비활성화 우선으로 둔다.

이 추천은 구현 확정이 아니라 `Candidate Recommendation`이다. 사용자 결정 기록 전까지는 구현하지 않는다.

## F. Field Candidate

### Document field 후보

| Field | Status | Note |
|---|---|---|
| `id` | Candidate | document id |
| `physicalFileName` | Candidate | `FileNamePolicyService` 결과 |
| `displayTitle` | Candidate | 화면 표시명 |
| `documentType` | Candidate | receipt/terms/capture/etc |
| `extension` | Candidate | pdf/jpg/jpeg/png |
| `relativePath` | Candidate | `attachments/` root 기준 |
| `createdAt` | Candidate | 생성 시각 |
| `updatedAt` | Candidate | 수정 시각 |
| `disabledAt` | Candidate | 실제 삭제 대신 사용 중지 |
| `isDisabled` | Candidate | `disabledAt`과 둘 중 선택 필요 |

### PolicyDocument field 후보

| Field | Status | Note |
|---|---|---|
| `id` | Candidate | policy-document id |
| `policyId` | Candidate | 보험 record 참조 |
| `documentId` | Candidate | `Document` 참조 |
| `documentType` | Candidate | policy document type |
| `createdAt` | Candidate | 연결 생성 시각 |
| `disabledAt` | Candidate | 사용 중지 |
| `memo` | Candidate | MVP 포함 여부 결정 필요 |

### ClaimDocument field 후보

| Field | Status | Note |
|---|---|---|
| `id` | Candidate | claim-document id |
| `claimId` | Candidate | claim record 참조 |
| `documentId` | Candidate | `Document` 참조 |
| `documentType` | Candidate | claim document type |
| `createdAt` | Candidate | 연결 생성 시각 |
| `disabledAt` | Candidate | 사용 중지 |
| `ocrConfirmedFieldsSnapshot` | Candidate / Later | 사용자 확정값 snapshot 여부 결정 필요 |
| `memo` | Candidate | MVP 포함 여부 결정 필요 |

## G. Needs Decision

| ID | Question | Status |
|---|---|---|
| Q1 | `Document`를 실제 파일 metadata 공통 record로 둘 것인가? | Needs Decision |
| Q2 | `PolicyDocument`는 `policyId + documentId` 연결 record로 둘 것인가? | Needs Decision |
| Q3 | `ClaimDocument`는 `claimId + documentId` 연결 record로 둘 것인가? | Needs Decision |
| Q4 | 실제 파일 metadata는 `Document`에만 저장하고, `PolicyDocument` / `ClaimDocument`에는 중복 저장하지 않을 것인가? | Needs Decision |
| Q5 | JSON 파일 단위는 `documents.json`, `policy-documents.json`, `claim-documents.json` 분리 파일 구조로 갈 것인가? | Needs Decision |
| Q6 | 삭제는 실제 삭제보다 `disabledAt` 또는 `isDisabled` 사용 중지 방식으로 갈 것인가? | Needs Decision |
| Q7 | `displayTitle`은 `Document`에 저장할 것인가? | Needs Decision |
| Q8 | `relativePath`는 `Document`에 저장할 것인가? | Needs Decision |
| Q9 | raw `originalFileName` 저장 보류를 계속 유지할 것인가? | Needs Decision |
| Q10 | OCR 임시 결과 저장 보류를 계속 유지할 것인가? | Needs Decision |
| Q11 | `ClaimDocument`에 사용자 확정 OCR 값 snapshot을 저장할지 여부는 별도 결정으로 보류할 것인가? | Needs Decision |
| Q12 | `memo` 필드는 MVP에서 포함할지 보류할 것인가? | Needs Decision |

## H. Out of Scope

이번 문서에서 제외한다.

- C# 모델 구현 없음
- JSON 저장 구현 없음
- SQLite DB 생성 없음
- SQLite package 추가 없음
- storage service interface 구현 없음
- repository 구현 없음
- data access 구현 없음
- migration 구현 없음
- DB 구현 없음
- OCR 구현 없음
- metadata 구현 없음
- file copy/storage 구현 없음
- WPF UI/XAML 구현 없음
- navigation 구현 없음
- 실제 개인정보 샘플 없음
- 실제 가족 실명 없음
- 실제 보험사명 없음
- 실제 병원명 없음
- 실제 진단명/진단코드 사례 없음

## I. Risks

| 위험 | 설명 | 완화 후보 |
|---|---|---|
| 파일 metadata 중복 | `Document`와 도메인 문서를 분리하지 않으면 파일 metadata 중복이 생길 수 있다. | 실제 파일 metadata는 `Document`에만 둔다. |
| reference 정합성 | JSON 분리 파일 구조는 `documentId`, `policyId`, `claimId` 참조 검증이 필요하다. | storage service interface에서 load/save 검증을 담당한다. |
| SQLite 전환 비용 | 단일 JSON 구조는 MVP에 편하지만 SQLite 전환 시 분리 비용이 커질 수 있다. | 논리 모델은 처음부터 분리한다. |
| `displayTitle` 민감정보 | `displayTitle`에 민감정보가 포함될 수 있다. | 외부 출력/공유 전 마스킹 정책을 별도 결정한다. |
| 삭제/복구 정책 흔들림 | `disabledAt`과 `isDisabled` 중 하나를 선택하지 않으면 삭제/복구 정책이 흔들릴 수 있다. | 사용 중지 표현을 하나로 확정한다. |
| OCR 이력 재현성 | OCR 확정값 snapshot 저장 여부를 결정하지 않으면 청구 제출 이력 재현성이 약할 수 있다. | `ocrConfirmedFieldsSnapshot`은 별도 결정으로 둔다. |

## J. Recommendation

다음 순서를 추천한다.

1. 이 문서를 기준으로 `Document` / `PolicyDocument` / `ClaimDocument` 저장 구조 결정을 받는다.
2. 사용자 결정 후 `docs/63_DOCUMENT_STORAGE_STRUCTURE_USER_DECISION_RECORD.md`를 생성한다.
3. 그 다음 `CategoryItem`과 document type 연결 정책 문서를 생성한다.
4. 그 다음 storage service interface 설계 문서를 생성한다.
5. 그 다음 JSON file storage 구현 여부를 별도 승인받는다.

## K. Result

`DOCUMENT_STORAGE_STRUCTURE_DECISION_DRAFTED`
