# 45_FILE_STORAGE_USER_DECISION_RECORD

## 1. Goal

이 문서는 `docs/44_FILE_STORAGE_AND_DOCUMENT_METADATA_DECISION.md`의 Q1~Q5에 대한 사용자 결정값을 기록한다.

이번 기록은 파일 저장 정책의 구현 전 결정 기록이다. C# 파일, XAML 파일, DB 파일, OCR 파일, 실제 문서 파일, metadata 파일은 생성하지 않는다.

## 2. Checked Files / Paths

| 구분 | 경로 | 확인 내용 |
|---|---|---|
| Data Model | `docs/06_DATA_MODEL.md` | `Document`, `PolicyDocument`, `ClaimDocument`, 파일 경로 기준 |
| Screen Mapping | `docs/23_SCREEN_TO_DATA_MODEL_MAPPING.md` | 화면별 문서 연결과 조회/저장 후보 |
| Gap Review | `docs/24_DATA_MODEL_GAP_REVIEW.md` | 문서 연결, 원본 파일명, 파일명 마스킹 위험 |
| UI State | `docs/33_UI_STATE_AND_CONFIRMATION_MESSAGE_GUIDE.md` | 파일명 규칙 위반과 민감정보 경고 기준 |
| Prior User Decisions | `docs/37_USER_DECISION_Q1_Q6_ACCEPTANCE_RECORD.md` | `displayTitle`, `originalFileName` 위험 메모 |
| MVVM Design | `docs/43_WPF_MINIMAL_MVVM_STRUCTURE_DESIGN.md` | Service 후보와 구현 금지 경계 |
| File Decision | `docs/44_FILE_STORAGE_AND_DOCUMENT_METADATA_DECISION.md` | Q1~Q5 질문과 권장 기본 답변 |
| Solution | `FamilyClaimRef.sln` | 수정하지 않음 |
| WPF Project | `app/FamilyClaimRef.App/FamilyClaimRef.App.csproj` | 수정하지 않음 |
| File Root Candidate | `attachments/` | 내부 파일 생성하지 않음 |
| Local Data Candidate | `data/local/` | 내부 파일 생성하지 않음 |

## 3. Scope

이 문서의 범위는 다음으로 제한한다.

- Q1~Q5 사용자 결정값 기록
- Accepted 항목과 위험 메모 분리
- Not Accepted / Deferred 항목 분리
- 데이터 모델 영향 후보 기록
- WPF MVVM service 후보 영향 기록
- 구현 전 남은 결정 항목 기록

이 문서의 범위 밖 항목은 다음과 같다.

- 파일 저장 구현
- 파일 복사 구현
- 실제 문서 파일 생성
- DB 테이블 또는 metadata 파일 생성
- OCR 실행 또는 OCR 임시 결과 저장
- C# class 생성
- XAML 화면 생성
- `.sln`, `.csproj`, Target Framework, NuGet package 변경

## 4. User Decisions Summary

사용자 결정값은 다음과 같다.

| 질문 | 사용자 결정 | 판정 |
|---|---|---|
| Q1 `physicalFileName` 포맷 | `claimId/policyId_날짜_문서유형` | Accepted |
| Q2 `displayTitle` 사용 | `애칭_보험사_진단명_날짜`를 화면 표시명으로 허용하되 물리 파일명에는 사용하지 않음 | Accepted with Risk Note |
| Q3 `originalFileName` 보존 | 마스킹된 표시명만 저장하고 raw 원본 파일명은 MVP에서 저장하지 않음 | Accepted |
| Q4 `attachments/` 역할 | 실제 파일 저장 루트 후보로 두되 구현은 보류 | Accepted as Candidate |
| Q5 `data/local/` 역할 | 로컬 메타데이터 저장 후보로 두고 OCR 임시 결과 저장은 보류 | Accepted as Candidate |

## 5. Accepted Decisions

다음 항목은 Accepted로 기록한다.

- `physicalFileName`은 `claimId/policyId_날짜_문서유형` 구조를 우선한다.
- `displayTitle`은 `애칭_보험사_진단명_날짜` 구조를 화면 표시명으로 허용한다.
- `displayTitle`은 물리 파일명으로 사용하지 않는다.
- raw `originalFileName`은 MVP에서 저장하지 않는다.
- 원본 파일명은 마스킹된 표시명만 저장한다.
- `attachments/`는 실제 파일 저장 루트 후보로 둔다.
- `data/local/`은 로컬 메타데이터 저장 후보로 둔다.
- OCR 임시 결과 저장은 보류한다.

주의:

- 위 결정은 구현 승인이 아니다.
- 실제 파일 저장, metadata 저장, DB 구조, OCR 임시 결과 저장은 별도 승인 전까지 진행하지 않는다.

## 6. Accepted with Risk Notes

다음 항목은 사용성 측면에서 허용하되 위험 메모를 붙인다.

| 항목 | 허용 이유 | 위험 |
|---|---|---|
| `displayTitle` | 사용자가 문서를 쉽게 찾을 수 있음 | 화면 표시명이어도 민감정보 단서가 될 수 있음 |
| `애칭_보험사_진단명_날짜` 구조 | 사용성이 좋고 사용자의 선호와 일치 | 화면 공유, 검색, 백업, 캡처에서 노출 위험이 있음 |
| `attachments/` 실제 저장 루트 후보 | 로컬 앱에서 문서 파일을 안정적으로 관리하기 쉬움 | 파일 삭제, 이동, 백업, 복구 정책이 필요함 |
| `data/local/` 로컬 메타데이터 후보 | 로컬 앱에서 문서 인덱스와 상태 관리가 쉬움 | 암호화, 백업, 삭제 정책이 후속으로 필요할 수 있음 |

민감정보 기준:

- 표시명에도 실제 가족 실명, 실제 보험사명, 실제 병원명, 실제 진단명을 그대로 저장하지 않는 방향을 유지한다.
- 표시명과 파일명은 외부 전송, 학습, 운영 API 전송에 사용하지 않는다.
- 화면 공유나 캡처 가능성을 고려해 표시명 노출 범위는 별도 결정한다.

## 7. Not Accepted / Deferred

다음 항목은 현재 단계에서 확정하지 않는다.

| 항목 | 판정 | 이유 |
|---|---|---|
| `displayTitle`을 물리 파일명으로 사용 | Not Accepted | 민감정보 단서가 파일 시스템에 노출될 수 있음 |
| raw `originalFileName` 저장 | Deferred | 원본 파일명에는 민감정보가 포함될 수 있음 |
| OCR 임시 결과 저장 | Deferred | OCR 원문 또는 후보값에 민감정보가 포함될 수 있음 |
| OCR 원문 전체 저장 | Not Accepted for MVP | 기본 미저장 방향 유지 |
| DB 테이블 생성 | Deferred | 물리 DB 구조 미확정 |
| metadata 파일 생성 | Deferred | `data/local/` 실제 형식 미확정 |
| 파일 복사/저장 구현 | Deferred | 파일 저장 정책 구현 승인 전 |
| `PolicyDocument` / `ClaimDocument` 물리 테이블 분리 | Needs Decision | 단일 `Document` 후보 유지 |
| `FileNamePolicyService` 구현 | Deferred | service 후보만 기록 |
| `LocalDocumentService` 구현 | Deferred | service 후보만 기록 |
| `DocumentMetadataService` 구현 | Deferred | service 후보만 기록 |

## 8. Data Model Impact

### `Document`

`Document`는 물리 파일 저장 메타데이터 후보로 유지한다. 아래는 DB 필드 확정이 아니라 후보 필드 범주이다.

- `documentId`
- `documentPurpose`
- `documentType`
- `relativeFilePath`
- `physicalFileName`
- `displayTitle`
- `originalFileNamePolicy`
- `linkedPolicyId`
- `linkedClaimCaseId`
- `ocrStatus`
- `reviewStatus`

주의:

- DB 필드 확정이 아니다.
- 실제 테이블 구조 확정이 아니다.
- 구현 전 후보 범주만 기록한다.
- `originalFileNamePolicy`는 raw 원본 파일명 저장이 아니라 보존 정책을 기록하는 후보다.

### `PolicyDocument` / `ClaimDocument`

`PolicyDocument`와 `ClaimDocument`의 경계는 다음과 같이 유지한다.

- `PolicyDocument`는 `Policy`에 연결되는 도메인 문서다.
- `ClaimDocument`는 `ClaimCase`에 연결되는 도메인 문서다.
- 물리 저장은 단일 `Document` 후보를 유지한다.
- `PolicyDocument` / `ClaimDocument` 물리 테이블 분리는 `Needs Decision`이다.

### 상태값 영향

문서 관련 상태값 후보는 기존 기준을 따른다.

- `registered`
- `ocr_needed`
- `ocr_completed`
- `user_confirmed`
- `ignored`

OCR 임시 결과 저장은 보류하므로 `ocr_needed`, `ocr_completed`, `user_confirmed`의 실제 저장 방식은 아직 확정하지 않는다.

## 9. WPF MVVM Impact

WPF MVVM에는 다음 service 후보가 영향을 받는다. 모두 후보이며 구현하지 않는다.

### `FileNamePolicyService`

책임 후보:

- `physicalFileName` 생성 정책 후보
- `displayTitle` 표시 정책 후보
- `originalFileName` 보존/마스킹 정책 후보

현재 판정:

- 구현 금지
- interface/class 생성 금지
- 파일명 생성 로직 작성 금지

### `LocalDocumentService`

책임 후보:

- `attachments/` 저장 루트 후보 관리
- 파일 선택, 복사, 저장, 조회 경계 후보

현재 판정:

- 구현 금지
- 실제 파일 복사 금지
- 실제 문서 파일 생성 금지

### `DocumentMetadataService`

책임 후보:

- `Document`, `PolicyDocument`, `ClaimDocument` 메타데이터 연결 후보
- `data/local/` 또는 DB metadata 후보 경계

현재 판정:

- 구현 금지
- DB metadata 생성 금지
- file metadata 생성 금지

## 10. Still Needs Decision

다음 항목은 구현 전 미결정으로 남긴다.

### 실제 `physicalFileName` 세부 규칙

- 구분자
- 날짜 포맷
- 문서유형 코드
- `claimId` / `policyId` 생성 방식
- 중복 파일명 처리 방식

### `displayTitle` 노출 범위

- 문서함 표시 여부
- 상세 화면 표시 여부
- 검색 결과 표시 여부
- 화면 캡처 또는 내보내기 제외 여부
- 민감정보 마스킹 표시 여부

### `attachments/` 실제 저장 정책

- 파일 복사 여부
- 외부 참조 여부
- 파일 이동 시 처리
- 파일 삭제 시 처리
- 백업/복구 정책

### `data/local/` 실제 저장 형식

- JSON
- SQLite
- 기타 형식
- 암호화 여부
- 백업 제외 여부

### 문서 삭제/사용 중지/복구 정책

- 문서 연결 해제
- 문서 파일 삭제 요청
- 파일 보존 기간
- 복구 가능 기간
- 청구 이력 연결 문서 삭제 제한

## 11. Risks

남은 위험은 다음과 같다.

- `displayTitle`이 화면 표시명이어도 민감정보 단서가 될 수 있다.
- `attachments/`를 실제 저장 루트로 쓰면 파일 삭제/이동/백업/복구 정책 없이는 운영 위험이 생긴다.
- `data/local/`에 metadata를 저장하면 암호화, 백업 제외, 삭제 정책을 별도로 정해야 한다.
- raw `originalFileName`을 MVP에서 저장하지 않더라도, 마스킹된 표시명 생성 규칙이 부실하면 민감정보가 남을 수 있다.
- OCR 임시 결과 저장을 보류했으나 OCR 화면의 후보값 표시와 사용자 확정값 저장 경계는 별도 설계가 필요하다.
- `PolicyDocument` / `ClaimDocument` 물리 테이블 분리를 미루면 이후 DB 설계에서 migration 비용이 생길 수 있다.

## 12. Recommendation

다음 구현 전 결정 순서를 권장한다.

1. `physicalFileName` 세부 규칙을 확정한다.
2. `displayTitle` 노출 화면과 마스킹 기준을 확정한다.
3. `attachments/`의 실제 파일 복사 여부를 결정한다.
4. `data/local/`의 첫 저장 형식과 암호화 여부를 결정한다.
5. 문서 삭제, 사용 중지, 복구 정책을 결정한다.
6. 그 후 `FileNamePolicyService`, `LocalDocumentService`, `DocumentMetadataService` 구현 범위를 별도 승인한다.

보수적 기본 방향:

- raw 원본 파일명 저장 없음
- 물리 파일명에는 민감정보 단서 없음
- 표시명은 화면용으로만 사용
- OCR 임시 결과 저장 없음
- 단일 `Document` 후보 유지
- 파일 저장 구현은 별도 승인 전 보류

## 13. Next Step

다음 문서 후보:

```text
docs/46_FILE_STORAGE_IMPLEMENTATION_SCOPE_DECISION.md
```

이 문서는 실제 구현 범위를 승인할 때만 작성한다.

구현 전 확인할 질문:

- 첫 구현이 `FileNamePolicyService`의 순수 정책 함수까지만인지
- `attachments/`에 실제 파일을 복사할지
- `data/local/`을 JSON 또는 SQLite 중 무엇으로 둘지
- metadata 저장 전에 암호화와 삭제 정책을 먼저 정할지

## Result

`FILE_STORAGE_USER_DECISIONS_RECORDED`

