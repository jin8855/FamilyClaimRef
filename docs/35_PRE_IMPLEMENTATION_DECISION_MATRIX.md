# 35_PRE_IMPLEMENTATION_DECISION_MATRIX

## 1. Goal

`docs/34_PRE_IMPLEMENTATION_READINESS_CHECKLIST.md`의 `Needs Decision Before Implementation` 항목을 기준으로 구현 전 결정 항목을 app scaffold, DB 설계, OCR 설계, MVP 구현 중 보류 항목으로 분류한다.

이 문서는 구현 지시서가 아니다. 앱 생성, DB 테이블 생성, OCR 구현, runtime scaffold 생성, HTML/CSS/JavaScript 수정, 실제 개인정보 샘플 작성을 포함하지 않는다.

## 2. Checked Files / Paths

- `README.md`
- `docs/01_PRD.md`
- `docs/02_FEATURE_SPEC.md`
- `docs/03_USER_FLOW.md`
- `docs/04_SCREEN_LIST.md`
- `docs/05_WIREFRAME_SPEC.md`
- `docs/06_DATA_MODEL.md`
- `docs/13_SCREEN_REVIEW_CHECKLIST.md`
- `docs/22_WIREFRAME_V5_5_ACTION_ALIGNMENT_AND_DETAIL_REVIEW.md`
- `docs/23_SCREEN_TO_DATA_MODEL_MAPPING.md`
- `docs/24_DATA_MODEL_GAP_REVIEW.md`
- `docs/33_UI_STATE_AND_CONFIRMATION_MESSAGE_GUIDE.md`
- `docs/34_PRE_IMPLEMENTATION_READINESS_CHECKLIST.md`

## 3. Scope

포함 범위:

- app scaffold 전 결정 항목 분류
- DB 설계 전 결정 항목 분류
- OCR 설계 전 결정 항목 분류
- MVP 구현 중 보류 가능 항목 분류
- 항목별 권장 결정안 제시
- 즉시 채택 가능한 MVP 후보와 사용자 승인 필요 항목 분리

제외 범위:

- app scaffold 방식 확정
- DB 테이블 구조 확정
- OCR 엔진/라이브러리 확정
- OCR 원문 예외 저장 세부 정책 확정
- HTML/CSS/JavaScript 수정
- 실제 개인정보 샘플 작성

## 4. Decision Status Legend

| 상태 | 의미 |
|---|---|
| Accepted | 현재 문서 기준으로 바로 채택 가능한 결정 |
| Accepted for MVP | MVP 한정으로 채택 가능한 결정 |
| Deferred | 구현 중 뒤로 미뤄도 되는 항목 |
| Needs User Approval | 사용자의 명시적 승인이 필요한 항목 |
| Needs Technical Design | 별도 설계 문서가 필요한 항목 |
| Keep Candidate | 후보로 유지하고 확정하지 않는 항목 |

## 5. App Scaffold Decision Matrix

| 항목 | 권장 결정 | 상태 | 이유 | 후속 영향 |
|---|---|---|---|---|
| 삭제 가능 조건 | MVP에서는 물리 삭제를 금지하거나 제한한다. 연결 데이터가 있으면 사용 중지를 우선하고, 삭제 요청은 `delete_requested` 후보 상태로만 처리한다. | Accepted for MVP | 가족, 보험, 분류, 항목은 기존 청구/문서/이력과 연결될 수 있다. | 화면의 삭제 버튼은 삭제 요청 또는 제한 안내 기준으로 설계 |
| 사용 중지 후 검색 노출 | 신규 선택 목록에서는 숨기고, 과거 이력/기존 청구 조회에서는 표시한다. 화면에는 사용 중지됨 표시 후보를 둔다. | Accepted for MVP | 과거 이력 정합성을 유지하면서 신규 입력 오염을 줄인다. | 목록 필터와 이력 표시 정책에 반영 |
| 파일명 마스킹 세부 포맷 | 내부 식별자 + 날짜 범위 + 문서 유형 중심으로 제한한다. 실제 이름, 실제 병원명, 실제 보험사명, 전체 번호, 진단코드 기반 개인 사례는 금지한다. 세부 포맷은 사용자 승인 대상으로 둔다. | Needs User Approval | 파일명은 민감정보 저장면에 포함된다. | 파일 연결 UI, 문서 메타데이터 표시, 백업 정책에 영향 |
| 원본 파일명 보존 여부 | MVP에서는 원본 파일명 저장 금지 또는 마스킹 후 저장을 우선한다. 원본 파일명 보존은 사용자 승인 전까지 확정하지 않는다. | Needs User Approval | 원본 파일명에 민감정보가 포함될 수 있다. | 파일 import, 문서함 표시, 추적성 정책에 영향 |
| 청구 완료 상태 세부 | `ClaimCase.case_completed`는 청구 준비 흐름 완료만 의미한다. `ClaimSubmission.submission_completed`와 `ClaimPayment.paid`는 별도 흐름으로 둔다. 완료 화면에서 지급 완료를 암시하지 않는다. | Accepted for MVP | 청구 완료와 지급 완료 혼동을 방지한다. | `14_claim_complete.html`, `08_claim_submission.html`, 이력 보기 문구 기준에 반영 |
| 앱 구현 방식 | 아직 확정하지 않는다. WPF, WinForms, Web, local runtime 등은 후보로만 남기고 app scaffold 전 별도 승인으로 결정한다. | Needs User Approval | 구현 방식은 프로젝트 구조와 런타임 설정을 결정한다. | 승인 전 app, src, package.json, tsconfig.json 생성 금지 |

## 6. DB Design Decision Matrix

| 항목 | 권장 결정 | 상태 | 이유 | 후속 영향 |
|---|---|---|---|---|
| 사용자 확정값 저장 위치 | MVP에서는 업무 객체에 사용자 확정값만 반영한다. OCR 후보값 원문, 수정 전후 보존 범위, 확정값 별도 audit 구조는 별도 설계로 둔다. | Needs Technical Design | 후보값과 확정값 경계가 DB 구조에 직접 영향 | `OcrCandidate`, `PolicyDocument`, `ClaimDocument`, `PolicyCoverage`, `ClaimCase` 설계 필요 |
| `ClaimReferenceResult` snapshot 저장 범위 | 전체 자동 저장은 금지한다. 사용자가 선택하거나 제출 판단에 사용한 결과만 snapshot 후보로 둔다. 보존 기간과 민감정보 범위는 별도 설계가 필요하다. | Needs Technical Design | 전체 저장 시 민감정보 과다 저장 위험이 있다. | 보험 찾기 결과, 진행 현황, 이력 조회 구조에 영향 |
| `HistoryItem` 저장 객체 전환 여부 | MVP에서는 projection 우선으로 둔다. 저장 객체 전환은 성능 또는 시점 보존 요구가 확인된 뒤 결정한다. | Accepted for MVP | 원본 객체와 이력 저장 객체의 동기화 위험을 줄인다. | 초기 DB 설계에서는 projection 기준 쿼리 또는 조회 모델 검토 |
| `Tag` 별도 객체 여부 | MVP에서는 `CategoryItem` 중심으로 둔다. 별도 `Tag`는 검색 랭킹, 동의어, prefix 규칙이 커질 때 분리한다. | Accepted for MVP | 단순 관리 데이터와 검색 태그를 초기에 분리하면 설계 복잡도가 커진다. | `Category`, `CategoryItem` 중심으로 시작 가능 |
| 물리 DB 테이블 구조 | 현재 문서 단계에서 확정하지 않는다. DB 설계 문서에서 단일 `Document` 후보, `PolicyDocument` / `ClaimDocument` 물리 분리 여부, 후보값 저장 경계를 별도 결정한다. | Needs Technical Design | 현재 문서는 개념 모델과 화면 매핑 단계다. | DB 설계 문서 없이는 테이블 생성 금지 |

## 7. OCR Design Decision Matrix

| 항목 | 권장 결정 | 상태 | 이유 | 후속 영향 |
|---|---|---|---|---|
| OCR 원문 예외 저장 조건 | 기본 미저장으로 둔다. 예외 저장 시 마스킹, 보존 기간, 사용 목적, 사용자 승인 기준이 필요하다. | Needs User Approval | OCR 원문은 민감정보 노출 위험이 크다. | OCR 설계 전 보안 정책 확정 필요 |
| OCR 구현 방식 | 아직 확정하지 않는다. 로컬 OCR 원칙만 유지하고, OCR 엔진/라이브러리/저장 방식은 별도 설계 전까지 확정하지 않는다. | Needs User Approval | OCR 방식은 성능, 배포, 저장 정책에 영향이 크다. | OCR 설계 문서와 사용자 승인 필요 |

## 8. Deferrable Until MVP Implementation

| 항목 | 권장 결정 | 상태 | 이유 | 후속 영향 |
|---|---|---|---|---|
| dirty-check 기준 | MVP 구현 중 화면별로 정의 가능하다. 단, 닫기 전 확인 메시지 기준은 유지한다. | Deferred | 실제 입력 컴포넌트가 정해진 뒤 세부 기준을 잡는 편이 안전하다. | 등록/편집 화면 구현 시 세부 정의 |
| `ClaimMemo` / `HistoryMemo` 별도 객체 여부 | MVP에서는 단순 `memo` 필드로 시작할 수 있다. 작성자, 작성 시점, 이력 보존 요구가 커지면 별도 객체화한다. | Accepted for MVP | 초기에는 메모 이력 관리보다 청구 흐름 정리가 우선이다. | DB 설계에서 단순 필드와 별도 객체 후보를 구분 |

## 9. User Approval Required

| 항목 | 승인 필요 이유 | 승인 전 상태 |
|---|---|---|
| 앱 구현 방식 | 프로젝트 구조와 런타임 파일 생성 여부를 결정한다. | Needs User Approval |
| 파일명 마스킹 세부 포맷 | 민감정보 노출과 파일 추적성의 균형을 결정한다. | Needs User Approval |
| 원본 파일명 보존 여부 | 원본 파일명은 민감정보를 포함할 수 있다. | Needs User Approval |
| OCR 원문 예외 저장 조건 | OCR 원문은 민감정보 위험이 크다. | Needs User Approval |
| OCR 구현 방식 | 로컬 OCR 원칙은 있으나 엔진과 저장 방식은 미정이다. | Needs User Approval |
| 물리 DB 테이블 구조 | 저장 구조는 이후 마이그레이션 비용과 보안 범위에 직접 영향이 있다. | Needs Technical Design + Needs User Approval |

## 10. Accepted MVP Decision Candidates

- 연결 데이터가 있는 항목은 물리 삭제보다 사용 중지를 우선한다.
- 삭제 요청은 `delete_requested` 후보 상태로만 처리한다.
- 비활성 데이터는 신규 선택 목록에서는 숨기고 과거 이력에서는 표시한다.
- `ClaimCase.case_completed`는 청구 준비 완료만 의미한다.
- `ClaimSubmission` / `ClaimPayment`는 보험사별 제출/지급 흐름으로 분리한다.
- `HistoryItem`은 projection 우선으로 둔다.
- MVP 태그는 `CategoryItem` 중심으로 둔다.
- `ClaimMemo` / `HistoryMemo`는 MVP에서는 단순 `memo` 필드 우선으로 둔다.
- OCR 원문 전체 저장은 기본 미저장으로 둔다.
- `ClaimReferenceResult` 전체 자동 저장은 금지한다.
- 사용자 확정값만 업무 객체에 반영한다.

## 11. Items That Must Not Be Finalized Yet

- app scaffold 방식
- DB 테이블 구조
- OCR 엔진/라이브러리
- OCR 원문 예외 저장 세부 정책
- 전체 `ClaimReferenceResult` 자동 snapshot 저장
- `HistoryItem` 저장 테이블화
- `Tag` 별도 객체화
- 원본 파일명 원문 보존
- 사용자 확정값 별도 audit 구조
- `PolicyDocument` / `ClaimDocument` 물리 테이블 분리

## 12. Risks

- 앱 구현 방식을 먼저 확정하지 않으면 scaffold 파일을 생성할 수 없다.
- DB 테이블 구조를 조기에 확정하면 `Document`, `OcrCandidate`, `ClaimReferenceResult`, `HistoryItem`의 후보 경계가 굳어질 수 있다.
- OCR 원문 예외 저장을 넓게 잡으면 민감정보 저장면이 급격히 커진다.
- 원본 파일명을 그대로 보존하면 파일명만으로 민감정보가 드러날 수 있다.
- `ClaimReferenceResult` 전체 snapshot 저장은 유사 청구 조회 결과를 과도하게 보존할 위험이 있다.
- `HistoryItem`을 저장 객체로 확정하면 원본 청구/지급 상태와 동기화 위험이 생긴다.

## 13. Recommendation

- app scaffold 전에는 앱 구현 방식, 파일명 마스킹 포맷, 원본 파일명 보존 여부, 삭제/사용 중지 정책, 청구 완료 상태 의미를 사용자 승인 대상으로 정리한다.
- DB 설계 전에는 사용자 확정값 저장 위치, `ClaimReferenceResult` snapshot 저장 범위, `HistoryItem` projection 여부, `Tag` 분리 여부, 물리 DB 테이블 구조를 별도 설계 문서로 다룬다.
- OCR 설계 전에는 OCR 원문 예외 저장 조건과 OCR 구현 방식을 별도 승인 대상으로 둔다.
- MVP에서는 삭제보다 사용 중지 우선, `HistoryItem` projection 우선, `CategoryItem` 중심 태그, 단순 `memo` 필드, OCR 원문 기본 미저장을 우선 후보로 둔다.

## 14. Next Step

권장 다음 작업:

1. 사용자 승인 필요 항목을 질문 목록으로 분리한다.
2. app scaffold 전 결정 항목만 먼저 확정한다.
3. DB 설계 전 결정 항목을 별도 DB decision 문서로 분리한다.
4. OCR 설계 전 결정 항목을 별도 OCR decision 문서로 분리한다.
5. 승인 전까지 app, src, package.json, tsconfig.json, DB 파일, OCR 구현 파일은 생성하지 않는다.

## Result

`DECISION_MATRIX_READY`

구현 전 decision 항목은 app scaffold, DB 설계, OCR 설계, MVP defer 항목으로 분리되었다. 사용자 승인 필요 항목은 확정값으로 승격하지 않고 별도 승인 대상으로 남겼다.
