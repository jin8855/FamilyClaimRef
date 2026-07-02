# 46_FILE_STORAGE_DETAIL_POLICY_DECISION

## 1. Goal

이 문서는 파일 저장 구현 전에 닫아야 할 세부 정책 후보를 정리한다.

기준은 `docs/45_FILE_STORAGE_USER_DECISION_RECORD.md`의 Still Needs Decision 항목이다. 이 문서는 구현 문서가 아니며, 실제 파일 저장, 파일 복사, metadata 저장, DB 생성, OCR 구현, C# service class 생성을 하지 않는다.

## 2. Checked Files / Paths

| 구분 | 경로 | 확인 내용 |
|---|---|---|
| Data Model | `docs/06_DATA_MODEL.md` | 문서 객체, 파일 경로, 삭제/사용 중지 후보 |
| Screen Mapping | `docs/23_SCREEN_TO_DATA_MODEL_MAPPING.md` | 문서 등록/문서함/OCR 화면의 데이터 연결 |
| Gap Review | `docs/24_DATA_MODEL_GAP_REVIEW.md` | 문서 연결, 파일명 마스킹, `data/local/` 위험 |
| UI State | `docs/33_UI_STATE_AND_CONFIRMATION_MESSAGE_GUIDE.md` | 파일명 경고, 삭제/사용 중지 메시지 기준 |
| Prior User Decisions | `docs/37_USER_DECISION_Q1_Q6_ACCEPTANCE_RECORD.md` | 사용자 친화 표시명과 원본 파일명 위험 메모 |
| MVVM Design | `docs/43_WPF_MINIMAL_MVVM_STRUCTURE_DESIGN.md` | 파일 저장 관련 service 후보와 구현 금지 경계 |
| File Decision | `docs/44_FILE_STORAGE_AND_DOCUMENT_METADATA_DECISION.md` | 파일명 3계층, 문서 객체, 저장 경로 후보 |
| User Decision Record | `docs/45_FILE_STORAGE_USER_DECISION_RECORD.md` | Q1~Q5 사용자 결정과 Still Needs Decision |
| File Root Candidate | `attachments/` | 내부 파일 생성하지 않음 |
| Local Data Candidate | `data/local/` | 내부 파일 생성하지 않음 |

## 3. Scope

이 문서의 범위는 다음과 같다.

- `physicalFileName` 세부 규칙 후보 정리
- `displayTitle` 노출 범위 후보 정리
- `attachments/` 실제 저장 정책 후보 정리
- `data/local/` 실제 저장 형식 후보 정리
- 문서 삭제, 사용 중지, 복구 정책 후보 정리
- 후속 구현 범위로 넘길 수 있는 항목과 계속 보류할 항목 분리
- 사용자 결정 질문과 권장 기본 답변 정리

범위 밖 항목은 다음과 같다.

- 신규 C# 파일 생성 또는 수정
- 신규 XAML 파일 생성 또는 수정
- `.sln`, `.csproj`, Target Framework, NuGet package 변경
- 실제 파일 복사 또는 저장
- `attachments/` 내부 파일 생성
- `data/local/` 내부 파일 생성
- DB 파일, metadata 파일, OCR 파일 생성
- navigation, ViewModelBase, RelayCommand, NavigationService 구현

## 4. physicalFileName Detail Policy

기준 결정:

```text
claimId/policyId_날짜_문서유형
```

세부 정책은 파일 시스템에 노출되는 이름에서 민감정보 단서를 제거하는 방향으로 잡는다.

### ID Prefix Candidate

문서 목적별 prefix 후보는 다음과 같다.

| 문서 목적 | prefix 후보 | 설명 |
|---|---|---|
| 청구 문서 | `claim-{claimId}` | `ClaimCase` 또는 청구 준비 단위에 연결되는 문서 |
| 보험 문서 | `policy-{policyId}` | `Policy`에 연결되는 보험 문서 |

예시 형식:

```text
claim-000001_20260626_receipt
policy-000003_20260626_terms
```

주의:

- 실제 가족명, 실제 보험사명, 실제 병원명, 실제 진단명은 사용하지 않는다.
- 실제 진단코드 기반 개인 사례를 파일명에 넣지 않는다.
- 파일 확장자는 원본 확장자를 보존하는 후보로 두되, 허용 확장자 정책은 별도 결정한다.

### Date Format Candidate

날짜 포맷 후보:

```text
yyyyMMdd
```

문서 유형별 날짜 기준 후보:

| 문서 유형 | 날짜 기준 후보 | 판정 |
|---|---|---|
| 청구 문서 | 진료일 후보 또는 등록일 후보 | Needs Decision |
| 보험 문서 | 문서 발행일 후보 또는 등록일 후보 | Needs Decision |
| 날짜 기준 없음 | 등록일 후보 | Candidate |

MVP 기본 후보:

- 포맷은 `yyyyMMdd`
- 날짜 기준 세부는 문서 유형별로 `Needs Decision`

### Document Type Code Candidate

보험 문서 유형 코드 후보:

| 코드 | 의미 |
|---|---|
| `policy` | 보험 문서 일반 |
| `terms` | 약관 |
| `contract` | 계약서 |
| `capture` | 화면 캡처 |

청구 문서 유형 코드 후보:

| 코드 | 의미 |
|---|---|
| `receipt` | 영수증 |
| `diagnosis` | 진단 관련 문서 |
| `medicine` | 약제비 관련 문서 |
| `visit` | 통원 관련 문서 |
| `admission` | 입퇴원 관련 문서 |
| `surgery` | 수술 관련 문서 |
| `etc` | 기타 |

주의:

- 문서유형 코드에 실제 진단명이나 병원명을 넣지 않는다.
- 문서유형 코드는 향후 `CategoryItem`과 연결될 수 있으나 DB 구조를 확정하지 않는다.

### Duplicate Handling Candidate

중복 파일명 처리 후보:

| 선택지 | 방식 | 장점 | 위험 | 판정 |
|---|---|---|---|---|
| A | `_001`, `_002` suffix 추가 | 단순하고 사람이 확인하기 쉬움 | 순번 관리 필요 | MVP 권장 |
| B | `documentId`를 더 길게 사용 | 충돌 가능성 낮음 | 파일명이 길어짐 | Candidate |
| C | 파일 hash 후보 사용 | 충돌 방지에 강함 | 구현 복잡도와 파일 접근 필요 | Deferred |
| D | 보류 | 결정 지연 | 구현 착수 불가 | Not Recommended |

권장:

- MVP는 A를 우선한다.
- hash 방식은 구현 복잡도와 파일 접근 필요 때문에 보류한다.

## 5. displayTitle Exposure Policy

기준 결정:

```text
애칭_보험사_진단명_날짜
```

단, `displayTitle`은 화면 표시명으로만 사용하고 물리 파일명에는 사용하지 않는다.

### Exposure Screen Candidate

노출 후보 화면은 다음과 같다.

| 화면 | 노출 후보 | 비고 |
|---|---|---|
| 문서함 | 표시 허용 후보 | 목록에서는 줄임 표시 후보 |
| 보험 문서 등록 화면 | 표시 허용 후보 | 연결 대상 확인용 |
| 청구 서류 등록 화면 | 표시 허용 후보 | 연결 대상 확인용 |
| OCR 확인 화면 | 표시 허용 후보 | 후보값과 사용자 확정값 구분 필요 |
| 이력 상세 화면 | 표시 허용 후보 | 과거 문서 연결 확인용 |

### Masking Candidate

마스킹 후보:

| 상황 | 표시 기준 후보 |
|---|---|
| 앱 내부 기본 화면 | 전체 표시 후보 |
| 화면 공유 모드 후보 | 애칭 또는 진단명 일부 마스킹 |
| 외부 출력/내보내기 | `displayTitle` 제외 또는 마스킹 |

판정 후보:

- MVP에서는 앱 내부 화면 표시를 허용한다.
- 내보내기, 공유, 출력 기능 전까지 외부 노출 범위는 보류한다.
- 긴 제목은 목록 화면에서 줄임 표시 후보로 둔다.

## 6. attachments Storage Policy

`attachments/`는 실제 파일 저장 루트 후보로 유지한다. 단, 이번 문서에서는 내부 파일을 만들지 않는다.

### Option A. 앱 내부 `attachments/`로 파일 복사

장점:

- 원본 파일 이동/삭제에 덜 취약하다.
- 앱이 참조를 안정적으로 유지할 수 있다.
- 백업 범위를 정하기 쉽다.

단점:

- 저장 용량이 증가한다.
- 삭제/복구 정책이 필요하다.
- 민감 파일이 앱 폴더에 모인다.

판정 후보:

- MVP 권장
- 구현은 별도 승인 전까지 금지

### Option B. 외부 원본 파일 경로만 참조

장점:

- 저장 용량을 줄일 수 있다.
- 원본 파일을 그대로 유지한다.

단점:

- 원본 파일 이동/삭제 시 링크가 깨질 수 있다.
- 백업/복구 기준이 약하다.
- 사용자 파일 정리 방식에 취약하다.

판정 후보:

- 후순위 Candidate

### Option C. 둘 다 지원

장점:

- 사용 방식이 유연하다.

단점:

- MVP 복잡도가 증가한다.
- UI와 정책이 복잡해진다.
- 파일 복사본과 외부 참조의 동기화 기준이 필요하다.

판정 후보:

- MVP에서는 보류

## 7. data/local Metadata Policy

`data/local/`은 로컬 metadata 저장 후보로 유지한다. 단, 이번 문서에서는 metadata 파일을 만들지 않는다.

### Option A. JSON Metadata

장점:

- 단순하다.
- 초기 개발이 빠르다.
- 사람이 확인하기 쉽다.

단점:

- 구조가 커지면 관리가 어렵다.
- 동시성, 검색, 무결성에 약하다.
- 민감정보 보호와 암호화 설계가 필요하다.

판정 후보:

- 아주 초기 후보
- 장기 MVP 기준으로는 보류

### Option B. SQLite

장점:

- 검색, 관계, 이력 관리에 유리하다.
- 보험, 청구, 문서, 이력 구조와 잘 맞는다.
- 장기 확장성이 좋다.

단점:

- DB 설계가 필요하다.
- migration 정책이 필요하다.
- 암호화 여부를 결정해야 한다.

판정 후보:

- MVP 장기 기준에서 강한 후보
- DB 설계 전까지 확정하지 않는다.

### Option C. 기타 형식

판정 후보:

- 현재 보류

현재 결정:

- `data/local/`은 로컬 metadata 저장 후보로 유지한다.
- 첫 구현에서는 저장 파일을 생성하지 않는다.
- JSON vs SQLite는 DB 설계 문서에서 결정한다.
- OCR 임시 결과 저장은 보류한다.

## 8. Document Delete / Disable / Restore Policy

문서 관련 삭제, 사용 중지, 복구는 물리 파일과 metadata를 분리해서 판단한다.

### 문서 연결 해제

후보 정책:

- metadata 연결만 끊는다.
- 실제 파일은 보존 후보로 둔다.
- 이력과 연결된 문서는 연결 해제를 제한할 수 있다.

판정 후보:

- MVP에서 물리 삭제보다 우선 검토

### 문서 파일 삭제 요청

후보 정책:

- 즉시 물리 삭제를 금지한다.
- `delete_requested` 또는 삭제 대기 상태 후보를 둔다.
- 복구 가능 기간 후보를 둔다.

판정 후보:

- MVP에서는 물리 삭제 금지 또는 강한 제한

### 문서 사용 중지

후보 정책:

- 신규 청구/검색에서는 숨긴다.
- 기존 이력에서는 표시한다.
- 보험 해지/사용 중지 정책과 유사하게 관리한다.

판정 후보:

- 연결 데이터가 있는 경우 사용 중지 우선

### 복구 정책

후보 정책:

- 삭제 요청 후 복구 가능 후보를 둔다.
- 복구 기간과 파일 보존 기간은 `Needs Decision`으로 유지한다.

권장:

- MVP에서는 물리 삭제 금지 또는 강한 제한
- 연결 해제/사용 중지 우선
- 실제 파일 삭제는 별도 승인 전 금지

## 9. Implementation Scope Candidates

이 문서는 구현하지 않는다. 다만 후속 승인 시 넘길 수 있는 최소 구현 후보와 금지 항목을 분리한다.

### 지금 구현 가능 후보

후속 승인 시 가능한 최소 구현 후보:

- 순수 `FileNamePolicyService` 함수

입력 후보:

- document scope
- id
- date
- document type
- extension

출력 후보:

- safe `physicalFileName`

제약:

- 파일 접근 없음
- DB 접근 없음
- OCR 없음
- metadata 저장 없음
- `attachments/` 내부 파일 생성 없음
- `data/local/` 내부 파일 생성 없음

### 아직 구현 금지

- 실제 파일 복사
- `attachments/` 내부 파일 생성
- metadata 저장
- DB 생성
- OCR 임시 결과 저장
- `displayTitle` 자동 생성
- raw `originalFileName` 저장
- 문서 삭제/복구 처리
- `LocalDocumentService` 구현
- `DocumentMetadataService` 구현

## 10. User Decision Questions

구현 전 사용자 결정 질문은 다음과 같다.

```text
Q1 physicalFileName 세부 규칙:
- A. claim-{id}_yyyyMMdd_{documentType}
- B. policy-{id}_yyyyMMdd_{documentType}
- C. 위 두 규칙을 문서 목적별로 함께 사용
- D. 보류

Q2 중복 파일명 처리:
- A. _001, _002 suffix
- B. documentId 확장
- C. hash 사용
- D. 보류

Q3 displayTitle 노출:
- A. 앱 내부 화면에는 전체 표시
- B. 문서함/상세만 표시
- C. 기본 마스킹 표시
- D. 보류

Q4 attachments 저장 정책:
- A. 앱 내부 attachments로 파일 복사
- B. 외부 경로 참조만 저장
- C. 둘 다 지원
- D. 보류

Q5 data/local 저장 형식:
- A. JSON 후보
- B. SQLite 후보
- C. DB 설계 전 보류
- D. 보류

Q6 문서 삭제 정책:
- A. MVP 물리 삭제 금지, 연결 해제/사용 중지 우선
- B. 삭제 요청 후 복구 가능 기간 둠
- C. 즉시 삭제 허용
- D. 보류
```

## 11. Recommended Default Answers

권장 기본 답변은 다음과 같다.

| 질문 | 권장 답변 | 이유 |
|---|---|---|
| Q1 | C | 청구 문서와 보험 문서가 서로 다른 연결 대상을 가지므로 목적별 prefix를 함께 사용 |
| Q2 | A | `_001`, `_002` suffix가 단순하고 파일 접근 없이 정책화하기 쉬움 |
| Q3 | A, 단 외부 출력/공유 기능 전까지 | 앱 내부 사용성과 검색성을 우선하되 외부 노출은 보류 |
| Q4 | A | 앱 내부 저장 루트가 참조 안정성과 백업 범위 관리에 유리 |
| Q5 | C | JSON vs SQLite는 DB 설계 전에는 확정하지 않는 것이 안전 |
| Q6 | A | 연결 이력 보존과 민감 문서 보호를 우선 |

## 12. Risks

남은 위험은 다음과 같다.

- `physicalFileName` 세부 규칙이 늦게 확정되면 이후 파일 저장 구현과 migration 기준이 흔들릴 수 있다.
- `displayTitle` 전체 표시는 앱 내부라도 화면 공유, 검색, 캡처에서 민감정보 단서가 될 수 있다.
- `attachments/`로 파일을 복사하면 민감 파일이 앱 폴더에 모이므로 백업/삭제/복구 정책이 필요하다.
- 외부 경로 참조만 쓰면 파일 이동/삭제로 문서 연결이 쉽게 깨질 수 있다.
- `data/local/` 저장 형식이 늦게 결정되면 metadata service 경계가 흔들릴 수 있다.
- SQLite를 채택하면 migration과 암호화 정책이 필요하다.
- 문서 물리 삭제를 허용하면 이력과 청구 근거가 사라질 수 있다.
- OCR 임시 결과 저장을 보류했더라도 OCR 후보값 표시와 사용자 확정값 저장 경계는 별도 설계가 필요하다.

## 13. Recommendation

다음 순서로 결정하는 것을 권장한다.

1. Q1, Q2를 먼저 확정해 `FileNamePolicyService`의 순수 정책 함수 구현 가능 여부를 판단한다.
2. Q3을 확정해 화면 표시명과 마스킹 범위를 정한다.
3. Q4를 확정해 파일 복사 방식과 외부 참조 방식의 MVP 범위를 고른다.
4. Q5는 DB 설계 문서에서 JSON vs SQLite를 비교한 뒤 확정한다.
5. Q6을 확정해 문서 삭제/사용 중지/복구 command의 위험을 줄인다.

후속 구현을 시작하더라도 첫 범위는 `FileNamePolicyService`의 순수 파일명 생성 정책으로 제한하는 것이 가장 안전하다.

## 14. Next Step

다음 문서 후보:

```text
docs/47_FILE_STORAGE_DETAIL_POLICY_USER_DECISION_RECORD.md
```

이 문서에는 Q1~Q6 사용자 답변을 기록한다.

사용자 답변이 기록되기 전에는 다음을 하지 않는다.

- C# service class 생성
- 파일 복사 구현
- metadata 파일 생성
- DB 생성
- OCR 임시 결과 저장
- 문서 삭제/복구 구현
- `attachments/` 내부 파일 생성
- `data/local/` 내부 파일 생성

## Result

`FILE_STORAGE_DETAIL_POLICY_READY`

