# Document Storage Structure User Decision Record

## A. Goal

이 문서는 `docs/62_DOCUMENT_STORAGE_STRUCTURE_DECISION.md`의 Needs Decision Q1~Q12에 대한 사용자 결정 기록이다.

목적은 `Document`, `PolicyDocument`, `ClaimDocument`의 MVP 저장 구조 방향을 확정하고, 이후 `CategoryItem`과 document type 연결 정책, storage service interface 설계, JSON schema 초안 작성의 기준을 제공하는 것이다.

이 문서는 구현 문서가 아니다. 실제 C# 모델 구현, JSON 저장 구현, storage service 구현, repository 구현, DB/OCR/metadata/file storage 구현은 수행하지 않는다.

## B. Checked Files / Paths

| 대상 | 확인 목적 | 비고 |
|---|---|---|
| `docs/62_DOCUMENT_STORAGE_STRUCTURE_DECISION.md` | Q1~Q12 Needs Decision과 storage structure 후보 확인 | 읽기 전용 |
| `docs/61_LOCAL_STORAGE_USER_DECISION_RECORD.md` | JSON file storage, `data/local/`, `attachments/` 사용자 결정 확인 | 읽기 전용 |
| `docs/60_LOCAL_STORAGE_STRATEGY_DECISION.md` | 로컬 저장 방식 비교와 후속 Accepted 상태 확인 | 읽기 전용 |
| `docs/44_FILE_STORAGE_AND_DOCUMENT_METADATA_DECISION.md` | 파일 metadata, `originalFileName`, OCR 임시 결과 기준 확인 | 읽기 전용 |
| `docs/45_FILE_STORAGE_USER_DECISION_RECORD.md` | 파일 저장 사용자 결정 기준 확인 | 읽기 전용 |
| `docs/46_FILE_STORAGE_DETAIL_POLICY_DECISION.md` | 파일 저장 세부 정책 후보 확인 | 읽기 전용 |
| `docs/47_FILE_STORAGE_DETAIL_POLICY_USER_DECISION_RECORD.md` | 파일 저장 세부 정책 사용자 결정 확인 | 읽기 전용 |
| `docs/59_FILENAME_POLICY_TEST_PROJECT_IMPLEMENTATION_REVIEW.md` | 구현/테스트 현황과 미구현 범위 확인 | 읽기 전용 |
| `FamilyClaimRef.sln` | solution 기준 확인 | 수정 없음 |
| `app/FamilyClaimRef.App/FamilyClaimRef.App.csproj` | WPF app project와 Target Framework 기준 확인 | 수정 없음 |
| `app/FamilyClaimRef.App/Models/` | model 구현 여부 확인 | 수정 없음 |
| `app/FamilyClaimRef.App/Services/` | service 구현 범위 확인 | 수정 없음 |

## C. User Decision Summary

| ID | Question | Decision | Recorded Direction |
|---|---|---|---|
| Q1 | `Document`를 실제 파일 metadata 공통 record로 둘 것인가? | Accepted | `Document`는 실제 첨부 파일의 공통 metadata record로 둔다. 파일 자체의 저장 정보와 표시 정보를 담당한다. |
| Q2 | `PolicyDocument`는 `policyId + documentId` 연결 record로 둘 것인가? | Accepted | `PolicyDocument`는 보험 record와 `Document`를 연결하는 도메인 관계 record로 둔다. |
| Q3 | `ClaimDocument`는 `claimId + documentId` 연결 record로 둘 것인가? | Accepted | `ClaimDocument`는 청구 record와 `Document`를 연결하는 도메인 관계 record로 둔다. |
| Q4 | 실제 파일 metadata는 `Document`에만 저장하고, `PolicyDocument` / `ClaimDocument`에는 중복 저장하지 않을 것인가? | Accepted | 파일 metadata는 `Document`에만 저장하고, 도메인 연결 record에는 `documentId` 참조만 둔다. |
| Q5 | JSON 파일 단위는 분리 파일 구조로 갈 것인가? | Accepted | `documents.json`, `policy-documents.json`, `claim-documents.json` 분리 파일 구조를 후보로 확정한다. |
| Q6 | 삭제는 실제 삭제보다 `disabledAt` 또는 `isDisabled` 사용 중지 방식으로 갈 것인가? | Accepted - disabledAt preferred | persisted source of truth는 `disabledAt` 하나로 둔다. `isDisabled`는 저장하지 않고 파생 상태로 둔다. |
| Q7 | `displayTitle`은 `Document`에 저장할 것인가? | Accepted | `displayTitle`은 앱 내부 화면 표시용 이름으로 `Document`에 저장한다. |
| Q8 | `relativePath`는 `Document`에 저장할 것인가? | Accepted | `relativePath`는 `attachments/` root 기준 상대 경로 후보로 `Document`에 저장한다. |
| Q9 | raw `originalFileName` 저장 보류를 계속 유지할 것인가? | Accepted | raw `originalFileName`은 MVP에서 저장하지 않는다. |
| Q10 | OCR 임시 결과 저장 보류를 계속 유지할 것인가? | Accepted | OCR 임시 결과 저장은 MVP에서 보류한다. 사용자 확정값만 업무 객체에 반영한다. |
| Q11 | `ClaimDocument`에 사용자 확정 OCR 값 snapshot을 저장할지 여부는 별도 결정으로 보류할 것인가? | Accepted - Deferred | `ocrConfirmedFieldsSnapshot`은 `Candidate / Later`로 유지하고 별도 정책 문서에서 결정한다. |
| Q12 | `memo` 필드는 MVP에서 포함할지 보류할 것인가? | Deferred for MVP 1st | `PolicyDocument.memo`, `ClaimDocument.memo`는 후보로만 유지하고 MVP 1차에서는 보류한다. |

## D. Accepted Storage Structure

- `Document`는 실제 파일 metadata 공통 record다.
- `PolicyDocument`는 `policyId + documentId` 연결 record다.
- `ClaimDocument`는 `claimId + documentId` 연결 record다.
- 파일 metadata는 `Document`에만 저장한다.
- `PolicyDocument` / `ClaimDocument`에는 파일 metadata를 중복 저장하지 않는다.
- `PolicyDocument` / `ClaimDocument`에는 `documentId` 참조를 둔다.
- JSON metadata는 분리 파일 구조 후보로 확정한다.
  - `documents.json`
  - `policy-documents.json`
  - `claim-documents.json`
- 사용 중지 source of truth는 `disabledAt`이다.
- `isDisabled`는 저장하지 않고 `disabledAt != null`에서 계산하는 파생 상태로 둔다.
- `displayTitle`은 `Document`에 저장한다.
- `relativePath`는 `Document`에 저장한다.
- raw `originalFileName` 저장 보류를 유지한다.
- OCR 임시 결과 저장 보류를 유지한다.
- OCR 확정값 snapshot은 별도 결정으로 보류한다.
- `memo`는 MVP 1차에서 보류한다.

## E. Still Not Implemented

아래 항목은 아직 구현하지 않았다.

- C# 모델 구현 없음
- JSON 저장 구현 없음
- SQLite DB 생성 없음
- SQLite package 추가 없음
- storage service interface 구현 없음
- JSON implementation 구현 없음
- repository 구현 없음
- data access 구현 없음
- migration 구현 없음
- DB 구현 없음
- OCR 구현 없음
- metadata 구현 없음
- file storage 구현 없음
- file copy/storage 구현 없음
- WPF UI/XAML 구현 없음
- navigation 구현 없음
- `attachments/` 내부 파일 생성 없음
- `data/local/` 내부 파일 생성 없음

## F. Next Decision Needed

다음 항목은 이후 구현 착수 전 별도 결정이 필요하다.

1. `CategoryItem`과 document type 연결 정책 결정
2. storage service interface 설계 범위 결정
3. JSON 파일 schema 초안 결정
4. `Document`, `PolicyDocument`, `ClaimDocument` C# model 구현 범위 결정
5. 저장 구현 전 테스트 범위 결정
6. `displayTitle` 외부 출력/공유 마스킹 정책 결정
7. OCR 확정값 snapshot 저장 정책 결정
8. memo/tag/history memo 정책 결정

## G. Recommendation

다음 순서로 진행하는 것이 적절하다.

1. `CategoryItem`과 document type 연결 정책 문서를 생성한다.
2. 그 다음 storage service interface 설계 문서를 생성한다.
3. 그 다음 JSON schema 초안 문서를 생성한다.
4. 그 다음 C# model 구현 여부를 별도 승인받는다.
5. 그 다음 JSON file storage 구현 여부를 별도 승인받는다.

## H. Result

`DOCUMENT_STORAGE_STRUCTURE_USER_DECISION_RECORDED`
