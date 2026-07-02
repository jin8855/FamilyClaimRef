# Data Model Core Decisions

## 1. 목적

이 문서는 V5.5 데이터 모델에서 구현 전에 반드시 정해야 하는 핵심 결정 사항을 1차로 정리한다.

이 문서는 `DECISION` 성격의 문서다. 다만 물리 DB 스키마 생성 지시가 아니며, DB 파일 생성, OCR 구현, 앱 구현 지시도 아니다.

## 2. 기준 문서

| 기준 문서 | 사용 목적 |
|---|---|
| `README.md` | 프로젝트 목적, 보안 원칙, 개발 전 시각화 게이트 확인 |
| `docs/06_DATA_MODEL.md` | 기존 데이터 모델 초안과 legacy 명칭 확인 |
| `docs/23_SCREEN_TO_DATA_MODEL_MAPPING.md` | 화면별 입력값, 저장 후보, 조회 후보 확인 |
| `docs/24_DATA_MODEL_GAP_REVIEW.md` | 미결정 항목과 위험 확인 |
| `docs/25_DATA_MODEL_NAMING_DECISION_DRAFT.md` | 명칭 변경 초안 확인 |
| `docs/26_PRE_DEV_ARTIFACT_GAP_REVIEW.md` | 구현 전 gap 확인 |
| `docs/27_DATA_MODEL_NAMING_DECISION.md` | V5.5 명칭 결정 기준 확인 |
| `docs/28_DATA_MODEL_V5_5_PROPOSED.md` | V5.5 제안 모델 확인 |
| `C:\DevKnowledgeVault\00_Common\COMMON_OPERATION_GUIDE.md` | 확인 사실, 후보, 미확정 항목 분리 기준 확인 |
| `C:\DevKnowledgeVault\00_Common\MARKDOWN_DOCUMENT_RULES.md` | 한국어 문서 작성과 raw identifier 보존 기준 확인 |

## 3. 결정 요약

| 결정 항목 | 결정 | 상태 | 근거 | 남은 위험 |
|---|---|---|---|---|
| 문서 화면/도메인 명칭 | `PolicyDocument`, `ClaimDocument`로 분리한다. | Confirmed for naming | V5.5 화면은 보험 문서와 청구 서류의 연결 대상이 다르다. | 물리 저장 구조는 아직 최종 확정 아님 |
| 문서 물리 저장 1차 후보 | 단일 `Document` + `documentPurpose` + `linkedPolicyId` + `linkedClaimCaseId`를 우선 후보로 둔다. | Candidate | 중복 필드를 줄이고, 기존 `docs/06_DATA_MODEL.md`의 `Document` 구조와 연결 가능하다. | 실제 DB 설계 전 물리 분리 가능성 재검토 필요 |
| OCR 후보값 | `OcrCandidate`를 후보값 객체로 사용한다. | Confirmed for planning | V5.5 화면은 OCR 후보값과 사용자 확정값을 분리한다. | OCR 실행 기록 분리 여부는 보류 |
| OCR 자동 반영 | OCR 후보값은 업무 객체에 자동 반영하지 않는다. | Confirmed | 사용자 확정값만 업무 객체에 반영해야 한다는 문서 기준이 일관된다. | 사용자 확정값 보존 범위 결정 필요 |
| OCR 실행 기록 | 별도 `OcrExtraction` 객체 후보로 보류한다. | Candidate | 실행 로그와 후보값의 관심사가 다르다. | OCR 원문 전체 저장은 민감정보 위험이 큼 |
| `ClaimReferenceResult` | 기본은 조회 결과 객체로 둔다. 선택 또는 제출 판단에 사용한 결과만 snapshot 저장 후보로 둔다. | Candidate with rule | 보험 찾기 화면은 담보 후보와 과거 유사 청구 Top 3를 묶어 보여준다. | 전체 자동 저장 시 민감정보 과다 저장 위험 |
| `HistoryItem` | 우선 저장 객체가 아닌 projection으로 둔다. | Candidate with direction | 이력 목록은 `ClaimCase`, `ClaimSubmission`, `ClaimPayment`를 통합 조회한다. | 성능 또는 시점 보존 요구가 생기면 저장 객체 전환 필요 |
| `CategoryItem` / `Tag` | MVP는 `CategoryItem` 중심으로 시작하고, `Tag`는 별도 객체 후보로 유지한다. | Candidate with direction | 관리 화면은 분류/항목 구조를 제공하고, 태그는 검색 규칙 확장 가능성이 있다. | 동의어, 검색 가중치, prefix 규칙이 커지면 분리 필요 |
| 메모 구조 | MVP는 각 업무 객체의 단순 `memo` 필드로 시작한다. | Candidate with direction | 현재 화면은 메모 이력보다 메모 입력 자체를 보여준다. | 작성 이력, 확인 상태, 복수 메모가 필요하면 별도 객체 필요 |
| 삭제 / 사용 중지 | 연결 데이터가 있는 객체는 물리 삭제하지 않고 `delete_requested` 또는 `disabled` 상태로 처리하는 후보를 둔다. | Candidate with rule | 가족, 보험, 분류/항목 화면에 삭제와 사용 중지가 분리되어 있다. | 복구 가능 여부와 표시 정책 결정 필요 |
| 파일 경로 / 민감정보 | 원본 파일은 `attachments/` 하위 후보로 관리하고, 저장소에는 경로와 메타데이터만 저장한다. | Confirmed for principle | `README.md`와 `docs/06_DATA_MODEL.md` 모두 원본 문서 Git 제외와 경로 저장을 전제한다. | 파일명 자체의 민감정보 마스킹 규칙 필요 |

## 4. 문서 객체 결정

화면/도메인 명칭:

- `PolicyDocument`
- `ClaimDocument`

물리 저장 1차 후보:

- 단일 `Document`
- `documentPurpose`
- `linkedPolicyId`
- `linkedClaimCaseId`

결정:

- 화면/도메인 명칭은 분리한다.
- 물리 저장은 우선 단일 `Document` 후보로 둔다.
- 실제 DB 설계 전까지 물리 분리 가능성은 `Candidate`로 유지한다.
- `PolicyDocument`는 `Policy`에 연결되는 문서 의미다.
- `ClaimDocument`는 `ClaimCase`에 연결되는 문서 의미다.

보류:

- 단일 `Document`에 둘 공통 필드와 목적별 확장 필드의 경계
- `PolicyDocument` / `ClaimDocument` 물리 분리 여부
- 문서 파일명 보존 여부와 마스킹 규칙

## 5. OCR 후보값 / 사용자 확정값 결정

- `OcrCandidate`를 후보값 객체로 사용한다.
- OCR 후보값은 업무 객체에 자동 반영하지 않는다.
- 사용자 확정값만 `PolicyDocument`, `ClaimDocument`, `PolicyCoverage`, `ClaimCase` 등에 반영한다.
- OCR 실행 기록은 별도 `OcrExtraction` 후보로 보류한다.
- OCR 원문 전체 저장은 민감정보 위험 때문에 기본 저장하지 않는 방향으로 둔다.

사용자 확정값 반영 기준:

| 문서 목적 | 후보값 | 확정값 반영 대상 |
|---|---|---|
| 보험 문서 | 문서 유형, 담보/특약 후보, 약관 근거 후보 | `PolicyDocument`, `PolicyCoverage` |
| 청구 서류 | 진료일 후보, 진료상황 후보, 금액 후보, 키워드/태그 후보 | `ClaimDocument`, `ClaimCase` |

보류:

- 후보값 수정 전후 이력 보존 여부
- 후보값 보존 기간
- OCR 실행 로그와 후보값 객체의 분리 방식

## 6. ClaimReferenceResult 결정

- `ClaimReferenceResult`는 기본적으로 보험 찾기 화면의 조회 결과 객체로 둔다.
- 전체 결과를 자동 저장하지 않는다.
- 사용자가 담보 후보를 선택하거나 청구 제출 판단에 사용한 결과만 snapshot 저장 후보로 둔다.
- 저장 시에도 Top 3 유사 청구, 선택 담보, 확인 필요 사유처럼 판단에 필요한 요약만 남기는 방향을 둔다.

보류:

- snapshot 저장 시점
- snapshot 저장 필드 범위
- 유사 청구 검색 결과의 재현 필요 수준

## 7. HistoryItem 결정

- `HistoryItem`은 우선 저장 객체가 아닌 projection으로 둔다.
- 원본은 `ClaimCase`, `ClaimSubmission`, `ClaimPayment`다.
- 이력 보기 화면은 원본 객체를 통합해 표시하는 조회 모델로 해석한다.
- 검색 성능이나 시점 보존 요구가 생기면 저장 객체 전환 후보로 둔다.

보류:

- projection 캐시 필요 여부
- 이력 상세 메모의 원본 객체 연결 방식
- 이력 검색 인덱스 저장 여부

## 8. Tag / CategoryItem 결정

- MVP에서는 `CategoryItem`을 넓은 의미의 관리 항목으로 사용한다.
- `Tag`는 별도 객체 `Candidate`로 유지한다.
- 단순 키워드/태그 선택은 `CategoryItem`의 하위 항목으로 시작할 수 있다.
- 동의어, 검색 가중치, 유사 청구 랭킹, 진단코드 prefix 규칙이 커지면 `Tag`를 분리한다.

보류:

- `CategoryItem`과 `Tag`의 물리 분리 여부
- 태그 동의어 관리 방식
- 태그 검색 가중치와 prefix 규칙 저장 방식

## 9. Memo 구조 결정

- MVP에서는 `ClaimCase.memo`, `ClaimSubmission.memo`, `ClaimPayment.memo` 같은 단순 필드로 시작한다.
- `ClaimMemo`, `HistoryMemo`는 별도 객체 `Candidate`로 유지한다.
- 작성 이력, 확인 상태, 복수 메모, 작성 위치 추적이 필요해지면 별도 객체로 분리한다.

보류:

- 메모 작성자 개념 필요 여부
- 메모 수정 이력 보존 여부
- 이력 상세 메모와 청구 진행 메모의 분리 여부

## 10. 삭제 / 사용 중지 결정

- 연결 데이터가 있는 객체는 물리 삭제하지 않는다.
- 삭제 요청은 `delete_requested` 상태 후보로 둔다.
- 사용 중지는 `disabled` 상태 후보로 둔다.
- 삭제와 사용 중지는 같은 정책으로 취급하지 않는다.
- 사용 중지된 항목은 신규 선택 목록에서 숨기거나 비활성 표시하는 후보를 둔다.

적용 후보:

- `FamilyMember`
- `Policy`
- `Category`
- `CategoryItem`
- `Tag`

보류:

- 복구 가능 여부
- 삭제 요청 후 표시 방식
- 연결 데이터가 없는 경우의 물리 삭제 허용 여부

## 11. 파일 경로 / 민감정보 결정

- 원본 문서 파일은 `attachments/` 하위 후보로 관리한다.
- `attachments/`는 Git 추적 대상이 아니다.
- DB 또는 로컬 메타 저장소에는 상대 경로, 문서 유형, 연결 대상, OCR 상태, 사용자 확인 상태만 저장하는 방향으로 둔다.
- 파일명에 실제 이름, 병원명, 주민번호, 증권번호 전체값이 들어가지 않도록 기준이 필요하다.
- 보험사명, 병원명, 진단명, 진단코드 prefix, 금액, 지급 결과는 민감정보 단서로 취급한다.
- OCR 원문 전체 저장은 기본 저장하지 않는 방향으로 둔다.

보류:

- 원본 파일명 보존 여부
- 파일명 자동 익명화 규칙
- 로컬 인덱스와 메타데이터 저장 위치
- 마스킹 표시 기준

## 12. 아직 보류할 항목

| 항목 | 상태 | 이유 |
|---|---|---|
| `PolicyDocument` / `ClaimDocument` 물리 분리 | Needs Decision | 문서 명칭은 분리했지만 저장 구조는 단일 `Document` 후보가 남아 있음 |
| `OcrExtraction` 별도 객체 | Needs Decision | OCR 실행 로그와 후보값 분리 필요 수준 미정 |
| OCR 원문 전체 저장 | Needs Decision | 민감정보 위험이 커서 기본 저장하지 않는 방향이나 예외 정책 미정 |
| `ClaimReferenceResult` snapshot 저장 범위 | Needs Decision | 판단 근거 보존과 민감정보 최소 저장 사이의 균형 필요 |
| `HistoryItem` 저장 객체 전환 | Needs Decision | projection으로 시작하되 성능 요구는 아직 미확인 |
| `Tag` 별도 객체 | Needs Decision | 단순 관리 항목인지 검색 전용 객체인지 미정 |
| `ClaimMemo` / `HistoryMemo` 별도 객체 | Needs Decision | 단순 메모 필드로 충분한지 미정 |
| 삭제 요청 후 복구 정책 | Needs Decision | `delete_requested` 상태의 후속 처리 기준 필요 |
| 파일명 마스킹 규칙 | Needs Decision | 실제 파일명에 민감정보가 포함될 수 있음 |

## 13. 다음 작업

1. `docs/30_DATA_MODEL_06_UPDATE_PLAN.md`를 기준으로 `docs/06_DATA_MODEL.md` 수정 범위를 검토한다.
2. 물리 저장 구조 결정 전에는 DB 테이블명이나 파일 생성 작업으로 넘어가지 않는다.
3. `Document` 단일 저장 후보와 `PolicyDocument` / `ClaimDocument` 물리 분리 후보를 비교한다.
4. OCR 실행 기록과 `OcrCandidate`의 분리 필요성을 결정한다.
5. `HistoryItem`, `ClaimReferenceResult`, `Tag`, `ClaimMemo`, `HistoryMemo`의 저장 여부를 최종 결정한다.
6. 민감정보 마스킹 기준과 파일명 익명화 기준을 별도 문서 또는 `docs/06_DATA_MODEL.md` 반영 계획에 연결한다.
