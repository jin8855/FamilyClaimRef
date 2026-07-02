# 44_FILE_STORAGE_AND_DOCUMENT_METADATA_DECISION

## 1. Goal

이 문서는 WPF MVP 구현 전에 파일 저장명, 화면 표시명, 원본 파일명, 문서 메타데이터, 저장 경로 후보의 경계를 정리한다.

현재 단계는 Decision 문서 작성 단계이며, 파일 저장 구현, 파일 복사, DB 테이블 생성, OCR 구현, C# service class 생성은 하지 않는다.

## 2. Checked Files / Paths

| 구분 | 경로 | 확인 내용 |
|---|---|---|
| Data Model | `docs/06_DATA_MODEL.md` | `Document`, `PolicyDocument`, `ClaimDocument`, 파일 경로 기준 |
| Screen Mapping | `docs/23_SCREEN_TO_DATA_MODEL_MAPPING.md` | 문서 등록, 문서함, OCR 확인 화면의 저장/조회 경계 |
| Gap Review | `docs/24_DATA_MODEL_GAP_REVIEW.md` | 문서 연결 위험, 파일명 마스킹, 원본 파일명 보존 위험 |
| UI State | `docs/33_UI_STATE_AND_CONFIRMATION_MESSAGE_GUIDE.md` | 파일명 규칙 위반, 민감정보 경고 메시지 기준 |
| Readiness | `docs/34_PRE_IMPLEMENTATION_READINESS_CHECKLIST.md` | 구현 전 결정 항목 |
| Decision Matrix | `docs/35_PRE_IMPLEMENTATION_DECISION_MATRIX.md` | 구현 착수 전 보류/승인 기준 |
| User Questions | `docs/36_USER_DECISION_QUESTIONS_BEFORE_IMPLEMENTATION.md` | 파일명 마스킹, 원본 파일명, 물리 DB 구조 질문 |
| User Decisions | `docs/37_USER_DECISION_Q1_Q6_ACCEPTANCE_RECORD.md` | 표시명 허용, 원본 파일명 local-only 후보, 위험 메모 |
| Scaffold Review | `docs/40_WPF_MINIMAL_SCAFFOLD_REVIEW.md` | `attachments/`, `data/local/` 파일 생성 금지 확인 |
| Scaffold Structure | `docs/41_WPF_SCAFFOLD_STRUCTURE_AND_TFM_REVIEW.md` | WPF scaffold 구조와 TFM 검토 |
| MVVM Structure | `docs/43_WPF_MINIMAL_MVVM_STRUCTURE_DESIGN.md` | `FileNamePolicyService`, `LocalDocumentService`, `DocumentMetadataService` 후보 |
| File Root Candidate | `attachments/` | 사용자 문서 파일 저장 또는 참조 후보. 현재 파일 생성 금지 |
| Local Data Candidate | `data/local/` | 로컬 메타데이터 또는 OCR 임시 결과 후보. 현재 파일 생성 금지 |

## 3. Scope

이 문서가 하는 일은 다음과 같다.

- `physicalFileName`, `displayTitle`, `originalFileName`의 의미와 사용 범위를 분리한다.
- `PolicyDocument`, `ClaimDocument`, `Document`의 도메인 명칭과 물리 저장 후보를 분리한다.
- `attachments/`, `data/local/`의 역할 후보를 정리한다.
- 원본 파일명 보존 정책 선택지를 비교한다.
- WPF MVVM service 후보에 미치는 영향을 정리한다.
- 구현 전 사용자 결정 질문과 권장 기본 답변을 기록한다.

이 문서가 하지 않는 일은 다음과 같다.

- 실제 파일 저장 또는 복사
- 실제 문서 파일 생성
- DB 파일 또는 DB 테이블 생성
- OCR 실행 또는 OCR 결과 저장
- C# 파일 생성 또는 수정
- XAML 파일 생성 또는 수정
- `.csproj`, `.sln`, Target Framework, NuGet package 수정
- sample/mock data 생성

## 4. Filename Layer Decision

파일명은 3계층으로 분리한다.

| 계층 | 의미 | MVP 판정 | 저장/노출 기준 |
|---|---|---|---|
| `physicalFileName` | 실제 디스크에 저장되는 파일명 후보 | `Accepted for MVP` | 민감정보를 넣지 않는다. 내부 식별자, 날짜, 문서 유형 중심 |
| `displayTitle` | 사용자가 화면에서 찾기 쉽게 보는 표시명 | `Accepted with Risk Note` | 화면 표시명으로만 사용한다. 물리 파일명으로 쓰지 않는다 |
| `originalFileName` | 사용자가 가져온 파일의 기존 이름 | `Needs User Approval` | 기본 표시명으로 쓰지 않는다. raw 보존은 별도 승인 필요 |

### `physicalFileName`

`physicalFileName`은 실제 디스크, 백업, 검색 인덱스, 압축 파일, 로그에서 노출될 수 있는 이름이다.

권장 후보:

```text
내부ID_날짜_문서유형
```

또는:

```text
claimId-or-policyId_날짜_문서유형
```

금지 기준:

- 실제 가족 실명
- 실제 보험사명
- 실제 병원명
- 실제 진단명
- 실제 진단코드 기반 개인 사례
- 주민번호, 증권번호, 계좌번호, 카드번호 전체값

MVP 판정:

- `Accepted for MVP`
- 단, 정확한 포맷은 사용자 결정 질문 Q1에서 확정한다.

### `displayTitle`

`displayTitle`은 사용자가 화면에서 문서를 찾기 쉽게 보는 표시명이다.

사용자 선호 표시명 구조:

```text
애칭_보험사_진단명_날짜
```

판정:

- 화면 표시명으로 사용: `Accepted for MVP`
- 물리 저장 파일명으로 그대로 사용: `Not Accepted / Risk`
- `physicalFileName`과 분리: `Recommended`

주의:

- 표시명은 로컬 화면에서만 사용한다.
- 외부 전송, 학습, 운영 API 전송에 사용하지 않는다.
- 화면 공유, 검색, 백업에서 민감정보 단서가 될 수 있으므로 경고 기준을 둔다.

### `originalFileName`

`originalFileName`은 사용자가 가져온 기존 파일명이다.

예시 범주:

- 스캔 파일명
- 휴대폰 저장명
- 다운로드 PDF명
- 사용자가 직접 만든 문서명

판정:

- local-only metadata 후보: `Accepted with Risk Note`
- raw 원본 파일명 보존: `Needs User Approval`
- 기본 화면 표시명으로 사용: `Not Recommended`
- 외부 전송 또는 학습 사용: `Forbidden`

MVP 기본값:

- `not stored` 또는 `masked only` 중 하나를 우선 후보로 둔다.
- raw 원본 파일명 저장은 사용자 명시 승인 전까지 보류한다.

## 5. Document Object Boundary

문서 객체는 도메인 명칭과 물리 저장 후보를 섞지 않는다.

| 객체 | 의미 | 연결 대상 | 현재 판정 |
|---|---|---|---|
| `PolicyDocument` | 보험 문서 도메인 명칭 | `Policy` | `Confirmed for naming` |
| `ClaimDocument` | 청구 서류 도메인 명칭 | `ClaimCase` | `Confirmed for naming` |
| `Document` | 원본 문서의 물리 저장 메타데이터 후보 | `Policy` 또는 `ClaimCase` | `Candidate` |

### `PolicyDocument`

`PolicyDocument`는 보험에 연결되는 문서의 도메인 명칭이다.

대상 후보:

- 보험 조회 캡처
- 보험증권
- 계약서
- 약관 문서

연결 기준:

- `Policy`에 연결한다.
- 실제 물리 저장 테이블 분리는 아직 확정하지 않는다.

### `ClaimDocument`

`ClaimDocument`는 청구 사건에 연결되는 문서의 도메인 명칭이다.

대상 후보:

- 진단서
- 진료비 영수증
- 약제비 영수증
- 통원, 입퇴원, 수술 확인 서류

연결 기준:

- `ClaimCase`에 연결한다.
- OCR 후보값이 있더라도 사용자 확정 전에는 업무 객체에 자동 반영하지 않는다.

### `Document`

`Document`는 단일 물리 저장 메타데이터 후보이다.

후보 필드 범주:

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

중요:

- `PolicyDocument` / `ClaimDocument` 물리 테이블 분리는 `Needs Decision`으로 유지한다.
- MVP에서는 단일 `Document` 후보와 도메인 명칭 분리를 우선한다.
- 물리 DB 구조는 이 문서에서 확정하지 않는다.

## 6. Storage Path Candidate

저장 경로는 후보로만 둔다.

| 경로 | 후보 역할 | Git 기준 | 현재 작업 기준 |
|---|---|---|---|
| `attachments/` | 사용자가 연결한 원본 문서 파일 저장 또는 참조 후보 | Git 추적 제외 대상 | 내부 파일 생성 금지 |
| `data/local/` | 로컬 인덱스, 메타데이터, OCR 임시 결과 후보 | Git 추적 제외 대상 | 내부 파일 생성 금지 |

### `attachments/`

후보 역할:

- 실제 문서 파일 저장 루트 후보
- 또는 외부 파일 참조 경로의 기준 후보
- `Document.relativeFilePath`가 가리키는 기준 경로 후보

결정 필요:

- 실제 파일을 복사해 보관할지
- 외부 파일 경로만 참조할지
- 삭제/이동 시 재연결 규칙을 둘지
- 백업과 복구 범위를 어떻게 둘지

### `data/local/`

후보 역할:

- 로컬 메타데이터 저장 후보
- 로컬 인덱스 저장 후보
- OCR 임시 결과 후보

결정 필요:

- OCR 임시 결과를 둘지
- OCR 원문 전체 저장을 계속 금지할지
- 로컬 메타데이터 형식을 무엇으로 둘지
- 삭제, 보존 기간, 백업 제외 정책을 어떻게 둘지

현재 판정:

- `attachments/`는 실제 파일 저장 루트 후보로 유지한다.
- `data/local/`은 로컬 메타데이터 저장 후보로 유지한다.
- OCR 임시 결과 저장은 보류한다.

## 7. Original Filename Policy Options

원본 파일명 보존 정책은 다음 선택지로 비교한다.

| 선택지 | 내용 | 장점 | 위험 | 판정 |
|---|---|---|---|---|
| A | 원본 파일명 저장 금지 | 민감정보 노출면 최소화, 정책 단순 | 원래 파일과 대조하기 어려움 | MVP 기본 후보 |
| B | 마스킹된 원본 표시명만 저장 | 일부 추적성 확보, 노출 위험 완화 | 마스킹 규칙 필요 | MVP 기본 후보 |
| C | raw 원본 파일명을 local-only metadata로 저장 | 찾기 쉬움, 원본 파일과 대조 가능 | 탐색기, 백업, 검색, 화면 공유 위험 | 사용자 명시 승인 필요 |

권장:

- MVP 기본값은 A 또는 B로 둔다.
- C는 감사, 복원, 원본 대조 요구가 명확하고 사용자가 명시 승인할 때만 가능하다.
- 어떤 경우에도 원본 파일명은 외부 전송, 학습, 운영 API 전송에 사용하지 않는다.

## 8. WPF MVVM Service Impact

파일 저장 정책은 MVVM service 후보에 영향을 준다. 단, 이번 문서에서는 service를 구현하지 않는다.

| Service 후보 | 책임 후보 | 필요한 결정 | 현재 금지 |
|---|---|---|---|
| `FileNamePolicyService` | `physicalFileName`, `displayTitle`, `originalFileName` 정책 적용 후보 | 파일명 포맷, 마스킹 규칙, 원본 파일명 보존 여부 | interface/class 생성 금지 |
| `LocalDocumentService` | 파일 선택, 복사, 저장, 조회 경계 후보 | `attachments/`가 저장 루트인지 참조 기준인지 | 실제 파일 복사/저장 금지 |
| `DocumentMetadataService` | `Document`, `PolicyDocument`, `ClaimDocument` 메타데이터 연결 후보 | 단일 `Document` 후보 유지 여부, DB/file metadata 위치 | DB/file metadata 생성 금지 |

ViewModel 영향:

- `PolicyDocumentRegisterViewModel`은 보험 문서 연결 상태 후보를 가진다.
- `ClaimDocumentRegisterViewModel`은 청구 서류 연결 상태 후보를 가진다.
- `DocumentBoxViewModel`은 문서 목록, 문서 유형, OCR 상태, 확인 상태 후보를 조회한다.
- `OcrReviewViewModel`은 `OcrCandidate`와 사용자 확정값을 분리해 보여준다.

구현 전 경계:

- ViewModel, Model, Service class를 생성하지 않는다.
- `ViewModelBase`, `RelayCommand`, `NavigationService`를 구현하지 않는다.
- 파일 선택 dialog, 복사, 저장, OCR 호출을 구현하지 않는다.

## 9. User Decision Questions

구현 전 사용자 결정 질문은 다음과 같다.

```text
Q1 physicalFileName 포맷:
- A. 내부ID_날짜_문서유형
- B. claimId/policyId_날짜_문서유형
- C. 기타
- D. 보류

Q2 displayTitle 사용:
- A. 애칭_보험사_진단명_날짜를 화면 표시명으로 허용
- B. 더 보수적인 표시명 사용
- C. 보류

Q3 originalFileName 보존:
- A. 저장 금지
- B. 마스킹된 표시명만 저장
- C. raw 원본 파일명을 local-only metadata로 저장
- D. 보류

Q4 attachments/ 역할:
- A. 실제 파일 저장 루트 후보
- B. 외부 파일 참조 경로만 저장
- C. 보류

Q5 data/local 역할:
- A. 로컬 메타데이터 저장 후보
- B. OCR 임시 결과 후보
- C. 보류
```

## 10. Recommended Default Answers

권장 기본 답변은 다음과 같다.

| 질문 | 권장 답변 | 이유 | 상태 |
|---|---|---|---|
| Q1 | B | 청구 또는 보험 연결 맥락을 파일명에 최소 수준으로 남기되 실명과 실제 기관명을 제외할 수 있음 | Candidate |
| Q2 | A, 단 `displayTitle`로만 사용 | 사용자가 찾기 쉬우며 물리 파일명과 분리하면 위험을 줄일 수 있음 | Accepted with Risk Note |
| Q3 | B, raw 원본 파일명은 보류 | 원본 대조 가능성을 일부 남기되 raw 노출 위험을 줄임 | Recommended |
| Q4 | A 후보, 구현은 보류 | 로컬 앱에서 문서 파일을 안정적으로 관리하기 쉬움 | Candidate |
| Q5 | A 후보, OCR 임시는 보류 | 구현 전 메타데이터 경계만 잡고 OCR 임시 저장은 민감정보 위험 때문에 보류 | Candidate |

이 권장안은 실제 구현 승인이 아니다. 다음 구현 단계 전에 사용자가 Q1~Q5를 승인해야 한다.

## 11. Risks

남은 위험은 다음과 같다.

- `displayTitle`에 사용자 친화 정보가 포함되면 로컬 화면에서도 민감정보 단서가 될 수 있다.
- raw `originalFileName`은 로컬이어도 탐색기, 백업, 검색 인덱스, 화면 공유, 압축 파일에서 노출될 수 있다.
- `PolicyDocument`와 `ClaimDocument`를 물리 테이블로 분리할지 정하지 않으면 `DocumentMetadataService` 경계가 바뀔 수 있다.
- `attachments/`를 실제 저장 루트로 쓰면 파일 삭제, 이동, 백업, 복구 정책이 필요하다.
- `data/local/`에 OCR 임시 결과를 두면 OCR 원문 전체 저장 금지 원칙과 충돌할 수 있다.
- 파일명 마스킹 규칙이 느슨하면 Git 제외만으로 민감정보 노출 위험을 막을 수 없다.
- 표시명, 원본 파일명, 물리 파일명의 관계가 UI에 명확히 보이지 않으면 사용자가 물리 파일명에 민감정보를 넣을 수 있다.

## 12. Recommendation

MVP 구현 전 권장 순서는 다음과 같다.

1. Q1에서 `physicalFileName` 포맷을 확정한다.
2. Q2에서 `displayTitle`을 화면 표시명으로만 허용하는지 확정한다.
3. Q3에서 raw `originalFileName`을 저장하지 않을지, 마스킹 표시명만 둘지 확정한다.
4. Q4에서 `attachments/`가 실제 저장 루트인지 외부 참조 기준인지 확정한다.
5. Q5에서 `data/local/`의 첫 역할을 로컬 메타데이터로 제한할지 확정한다.
6. 그 후에만 `FileNamePolicyService`, `LocalDocumentService`, `DocumentMetadataService` 구현 여부를 별도 승인한다.

보수적 기본안:

- `physicalFileName`: `claimId-or-policyId_날짜_문서유형`
- `displayTitle`: 화면 표시명으로만 허용
- `originalFileName`: raw 저장 보류, 마스킹된 표시명만 후보
- `attachments/`: 실제 파일 저장 루트 후보
- `data/local/`: 로컬 메타데이터 후보
- OCR 임시 결과: 보류

## 13. Next Step

다음 작업은 사용자 승인 질문에 대한 답변 기록 문서를 만드는 것이다.

권장 문서 후보:

```text
docs/45_FILE_STORAGE_USER_DECISION_RECORD.md
```

그 전에는 다음을 하지 않는다.

- C# service 구현
- 파일 선택/복사/저장 구현
- DB 또는 metadata 파일 생성
- OCR 임시 결과 저장
- raw 원본 파일명 저장
- `attachments/`, `data/local/` 내부 파일 생성

## Result

`FILE_STORAGE_DECISION_READY`

