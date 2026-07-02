# 47_FILE_STORAGE_DETAIL_POLICY_USER_DECISION_RECORD

## 1. Goal

이 문서는 `docs/46_FILE_STORAGE_DETAIL_POLICY_DECISION.md`의 Q1~Q6에 대한 사용자 결정값을 기록한다.

이번 기록은 파일 저장 세부 정책의 사용자 결정 기록이며 구현 작업이 아니다. C# 파일, XAML 파일, DB 파일, metadata 파일, OCR 파일, 실제 문서 파일, `attachments/` 내부 파일, `data/local/` 내부 파일은 생성하지 않는다.

## 2. Checked Files / Paths

| 구분 | 경로 | 확인 내용 |
|---|---|---|
| File Decision | `docs/44_FILE_STORAGE_AND_DOCUMENT_METADATA_DECISION.md` | 파일명 3계층, 문서 객체, 저장 경로 후보 |
| File User Decision | `docs/45_FILE_STORAGE_USER_DECISION_RECORD.md` | Q1~Q5 파일 저장 사용자 결정 기록 |
| Detail Policy Decision | `docs/46_FILE_STORAGE_DETAIL_POLICY_DECISION.md` | Q1~Q6 세부 정책 질문과 권장 답변 |
| Solution | `FamilyClaimRef.sln` | 수정하지 않음 |
| WPF Project | `app/FamilyClaimRef.App/FamilyClaimRef.App.csproj` | 수정하지 않음 |
| File Root Candidate | `attachments/` | 내부 파일 생성하지 않음 |
| Local Data Candidate | `data/local/` | 내부 파일 생성하지 않음 |

## 3. Scope

이 문서의 범위는 다음으로 제한한다.

- Q1~Q6 사용자 결정값 기록
- Accepted Decisions 분리
- Accepted with Risk Notes 분리
- Deferred / Not Implemented 분리
- `FileNamePolicyService` 후속 구현 가능 후보 정리
- Still Needs Decision 항목 유지

범위 밖 항목은 다음과 같다.

- 신규 C# 파일 생성 또는 수정
- 신규 XAML 파일 생성 또는 수정
- `.sln`, `.csproj`, Target Framework, NuGet package 변경
- 실제 파일 복사 또는 저장
- `attachments/` 내부 파일 생성
- `data/local/` 내부 파일 생성
- DB 파일 또는 metadata 파일 생성
- OCR 실행 또는 OCR 임시 결과 저장
- `ViewModelBase`, `RelayCommand`, `NavigationService` 구현
- model/service class 구현

## 4. User Decisions Summary

사용자 결정값은 다음과 같다.

| 질문 | 사용자 결정 | 판정 |
|---|---|---|
| Q1 `physicalFileName` 세부 규칙 | 청구 문서는 `claim-{id}_yyyyMMdd_{documentType}`, 보험 문서는 `policy-{id}_yyyyMMdd_{documentType}` 규칙을 함께 사용 | Accepted |
| Q2 중복 파일명 처리 | `_001`, `_002` suffix 사용 | Accepted |
| Q3 `displayTitle` 노출 | 앱 내부 화면에는 전체 표시. 단, 외부 출력/공유/내보내기 기능 전까지로 제한 | Accepted with Risk Note |
| Q4 `attachments` 저장 정책 | 앱 내부 `attachments/`로 파일을 복사하는 방향을 MVP 후보로 둠. 단, 실제 파일 복사 구현은 아직 하지 않음 | Accepted as Candidate |
| Q5 `data/local` 저장 형식 | DB 설계 전까지 보류. JSON vs SQLite는 DB 설계 문서에서 결정 | Deferred |
| Q6 문서 삭제 정책 | MVP에서는 물리 삭제 금지, 연결 해제/사용 중지 우선 | Accepted |

## 5. Accepted Decisions

다음 항목은 Accepted로 기록한다.

### 청구 문서 `physicalFileName` 규칙

```text
claim-{id}_yyyyMMdd_{documentType}
```

의미:

- 청구 문서에는 `claim-` prefix를 사용한다.
- `{id}`는 청구 문서 연결 대상의 내부 식별자 후보이다.
- `{documentType}`은 문서유형 코드 후보이다.

### 보험 문서 `physicalFileName` 규칙

```text
policy-{id}_yyyyMMdd_{documentType}
```

의미:

- 보험 문서에는 `policy-` prefix를 사용한다.
- `{id}`는 보험 문서 연결 대상의 내부 식별자 후보이다.
- `{documentType}`은 문서유형 코드 후보이다.

주의:

- 실제 가족 실명, 실제 보험사명, 실제 병원명, 실제 진단명은 물리 파일명에 넣지 않는다.
- 실제 진단코드 기반 개인 사례도 물리 파일명에 넣지 않는다.

### 중복 파일명 처리

중복 파일명은 suffix를 사용한다.

```text
_001
_002
```

예시 형식:

```text
claim-000001_20260626_receipt_001
policy-000003_20260626_terms_001
```

위 예시는 구조 예시이며 실제 개인정보나 실제 문서 사례가 아니다.

### `displayTitle` 노출

- 앱 내부 화면에는 전체 표시한다.
- 단, 외부 출력, 공유, 내보내기 기능 전까지만 허용한다.
- 물리 파일명으로 사용하지 않는다.

### `attachments/` 저장 정책

- 앱 내부 `attachments/`로 파일을 복사하는 방향을 MVP 후보로 둔다.
- 단, 실제 파일 복사 구현은 아직 하지 않는다.
- `attachments/` 내부 파일도 이번 작업에서 생성하지 않는다.

### `data/local` 저장 형식

- DB 설계 전까지 보류한다.
- JSON vs SQLite는 DB 설계 문서에서 결정한다.
- 이번 작업에서 `data/local` 내부 파일을 생성하지 않는다.

### 문서 삭제 정책

- MVP에서는 물리 삭제를 금지한다.
- 연결 해제와 사용 중지를 우선한다.
- 삭제/복구 처리는 구현하지 않는다.

## 6. Accepted with Risk Notes

위험 메모는 다음과 같다.

| 항목 | 위험 |
|---|---|
| `displayTitle` 전체 표시 | 앱 내부라도 화면 공유, 캡처, 검색 상황에서 민감정보 단서가 될 수 있음 |
| `attachments/` 내부 복사 | 참조 안정성은 높지만 민감 파일이 앱 폴더에 모임 |
| suffix 방식 | 단순하지만 충돌 관리 기준과 순번 산정 기준이 필요함 |
| `data/local` 저장 형식 보류 | metadata service 구현을 아직 시작할 수 없음 |
| 문서 물리 삭제 금지 | 안전하지만 저장 용량과 정리 정책이 필요함 |

보완 원칙:

- `displayTitle`은 앱 내부 표시명으로만 사용한다.
- 외부 출력, 공유, 내보내기 기능을 만들기 전에는 별도 마스킹 기준을 먼저 정한다.
- `attachments/` 실제 복사 구현은 별도 승인 전까지 진행하지 않는다.
- `data/local` 저장 형식이 정해지기 전에는 metadata 저장 구현을 시작하지 않는다.

## 7. Deferred / Not Implemented

다음 항목은 Deferred 또는 Not Implemented로 기록한다.

| 항목 | 판정 | 비고 |
|---|---|---|
| 실제 파일 복사 구현 | Not Implemented | `attachments/` 정책 후보만 기록 |
| `attachments/` 내부 파일 생성 | Not Implemented | 이번 작업 범위 아님 |
| metadata 파일 생성 | Not Implemented | `data/local` 형식 미정 |
| DB 생성 | Deferred | DB 설계 전까지 보류 |
| JSON/SQLite 선택 | Deferred | DB 설계 문서에서 결정 |
| OCR 임시 결과 저장 | Deferred | 민감정보 위험과 OCR 경계 미정 |
| `displayTitle` 자동 생성 | Deferred | 표시명 생성 규칙과 마스킹 기준 추가 필요 |
| raw `originalFileName` 저장 | Not Implemented | MVP에서는 저장하지 않음 |
| 문서 삭제/복구 처리 | Deferred | 복구 가능 기간과 보존 정책 미정 |
| `LocalDocumentService` 구현 | Not Implemented | service 후보만 유지 |
| `DocumentMetadataService` 구현 | Not Implemented | service 후보만 유지 |
| `FileNamePolicyService` 구현 | Not Implemented | 후속 승인 시 순수 정책 함수 후보 |

## 8. FileNamePolicyService Implementation Candidate

후속 승인 시 구현 가능한 최소 범위는 순수 파일명 정책 함수로 제한한다.

구현 가능 후보:

- 순수 파일명 정책 함수

입력 후보:

- document scope: `claim` / `policy`
- id
- date
- documentType
- extension
- duplicateIndex 후보

출력 후보:

- safe `physicalFileName`

제약:

- 파일 접근 없음
- DB 접근 없음
- OCR 없음
- metadata 저장 없음
- `attachments/` 내부 파일 생성 없음
- `data/local/` 내부 파일 생성 없음
- 실제 문서 파일 생성 없음
- `LocalDocumentService` 구현 없음
- `DocumentMetadataService` 구현 없음

후속 구현 승인 전 확인:

- `{id}`의 실제 생성 주체
- 날짜 기준
- 문서유형 코드 최종 목록
- 허용 확장자
- suffix 산정 기준

## 9. Still Needs Decision

다음 항목은 아직 결정하지 않는다.

### document type code 최종 목록

- 보험 문서 코드 최종 목록
- 청구 문서 코드 최종 목록
- `CategoryItem`과의 연결 여부

### 날짜 기준

- 진료일
- 등록일
- 문서 발행일
- 날짜 값이 없을 때 fallback 기준

### 허용 파일 확장자

- PDF
- 이미지
- 문서 파일
- 기타 확장자 허용 여부

### `attachments/` 백업/복구 정책

- 백업 포함 여부
- 복구 방식
- 저장 용량 정리 기준
- 파일 이동/삭제 감지 방식

### `data/local` JSON vs SQLite

- JSON 후보
- SQLite 후보
- migration 필요 여부
- DB 설계 문서에서 결정

### metadata 암호화 여부

- metadata 암호화 필요 여부
- 암호화 키 관리
- 백업 시 암호화 유지 기준

### 문서 복구 가능 기간

- 삭제 요청 후 복구 가능 기간
- 물리 파일 보존 기간
- 이력 연결 문서 삭제 제한

### 외부 출력/공유/내보내기 시 `displayTitle` 마스킹 기준

- `displayTitle` 제외 여부
- 부분 마스킹 기준
- 화면 공유 모드 후보

## 10. Risks

남은 위험은 다음과 같다.

- 앱 내부 전체 `displayTitle` 표시는 사용성이 좋지만 화면 공유와 캡처에서 민감정보 단서가 될 수 있다.
- `attachments/` 복사 정책은 참조 안정성을 높이지만 민감 파일을 앱 폴더에 모으므로 보안과 정리 정책이 필요하다.
- suffix 방식은 단순하지만 동시에 같은 기준의 파일을 추가할 때 순번 산정 기준이 필요하다.
- `data/local` 저장 형식이 보류되어 metadata service 구현은 아직 시작할 수 없다.
- 문서 물리 삭제 금지는 안전하지만 저장 용량 증가와 장기 정리 정책이 필요하다.
- DB 설계 전에 JSON/SQLite를 임의로 선택하면 이후 migration 비용이 생길 수 있다.

## 11. Recommendation

다음 순서를 권장한다.

1. document type code 최종 목록을 먼저 확정한다.
2. 날짜 기준을 문서 목적별로 확정한다.
3. 허용 파일 확장자를 확정한다.
4. `FileNamePolicyService`의 순수 정책 함수 구현 범위를 별도 승인한다.
5. `attachments/` 실제 복사 구현은 백업/복구 정책 결정 뒤로 둔다.
6. `data/local` 저장 형식은 DB 설계 문서에서 JSON vs SQLite를 비교한 뒤 결정한다.
7. 외부 출력/공유/내보내기 기능 전에는 `displayTitle` 마스킹 기준을 먼저 확정한다.

보수적 첫 구현 후보:

- `FileNamePolicyService` 순수 정책 함수만
- 파일 접근 없음
- DB 접근 없음
- metadata 저장 없음
- OCR 없음
- `attachments/`, `data/local` 내부 파일 생성 없음

## 12. Next Step

다음 문서 후보:

```text
docs/48_FILENAME_POLICY_IMPLEMENTATION_SCOPE_DECISION.md
```

이 문서에서는 실제 구현이 아니라 `FileNamePolicyService`의 순수 정책 함수만 구현할지, 입력/출력 계약을 어디까지 허용할지 결정한다.

그 전에는 다음을 하지 않는다.

- C# service class 생성
- 실제 파일 복사
- metadata 파일 생성
- DB 생성
- OCR 임시 결과 저장
- `attachments/` 내부 파일 생성
- `data/local/` 내부 파일 생성
- 문서 삭제/복구 구현

## Result

`FILE_STORAGE_DETAIL_USER_DECISIONS_RECORDED`

