# 34_PRE_IMPLEMENTATION_READINESS_CHECKLIST

## 1. Goal

V5.5 와이어프레임, 데이터 모델, 화면-데이터 매핑, gap review, 상태/메시지 기준, 화면 검토 체크리스트를 기준으로 구현 착수 전 준비 상태를 점검한다.

이 문서는 구현 지시서가 아니다. 개발 환경 생성, DB 설계 확정, OCR 구현 방식, runtime scaffold 생성 지시는 포함하지 않는다.

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
- `docs/31_DATA_MODEL_TERMINOLOGY_CONSISTENCY_REVIEW.md`
- `docs/32_DATA_MODEL_GAP_REVIEW_STALENESS_REVIEW.md`
- `docs/33_UI_STATE_AND_CONFIRMATION_MESSAGE_GUIDE.md`
- `design/wireframes/*.html`

## 3. Scope

포함 범위:

- 구현 착수 전 문서 준비 상태 점검
- V5.5 정적 와이어프레임 기준 점검
- 데이터 모델 명칭과 Candidate / Needs Decision 경계 점검
- 화면-데이터 매핑 준비 상태 점검
- UI 상태/메시지 기준 준비 상태 점검
- 민감정보와 파일명 기준 점검
- 구현 전 결정 필요 항목 분류

제외 범위:

- app scaffold 생성
- DB 테이블 설계 확정
- OCR 구현 방식 확정
- HTML/CSS/JavaScript 수정
- 실제 개인정보 샘플 작성
- runtime 또는 build 설정 생성

## 4. Document Readiness

| 항목 | 확인 문서 | 점검 결과 | 비고 |
|---|---|---|---|
| PRD | `docs/01_PRD.md` | Ready | 목표, 비목표, 보안 요구사항이 분리되어 있음 |
| 기능 명세서 | `docs/02_FEATURE_SPEC.md` | Ready | 가족, 보험, 문서, OCR, 청구, 이력 기능 범위가 정리됨 |
| 사용자 프로세스 | `docs/03_USER_FLOW.md` | Ready with notes | 초기 흐름 문서이며 V5.5 세부 흐름은 후속 문서와 함께 확인 필요 |
| 화면 목록표 | `docs/04_SCREEN_LIST.md` | Ready with notes | 초기 화면 목록 성격. V5.5 실제 화면은 `design/wireframes/*.html`과 체크리스트 기준 |
| 와이어프레임 기준 | `docs/05_WIREFRAME_SPEC.md`, `docs/22_WIREFRAME_V5_5_ACTION_ALIGNMENT_AND_DETAIL_REVIEW.md` | Ready | 액션 정렬, 닫기, 우측 패널 기준이 정리됨 |
| 데이터 모델 | `docs/06_DATA_MODEL.md` | Ready with decisions required | 명칭은 정리됐으나 물리 저장과 일부 후보 객체는 `Needs Decision` |
| 화면-데이터 매핑 | `docs/23_SCREEN_TO_DATA_MODEL_MAPPING.md` | Ready | 화면별 저장/조회 객체와 상태값 후보가 정리됨 |
| 데이터 모델 gap review | `docs/24_DATA_MODEL_GAP_REVIEW.md`, `docs/32_DATA_MODEL_GAP_REVIEW_STALENESS_REVIEW.md` | Ready | 오래된 gap과 유효한 `Needs Decision`이 분리됨 |
| 상태/메시지 기준 | `docs/33_UI_STATE_AND_CONFIRMATION_MESSAGE_GUIDE.md` | Ready with decisions required | 공통 상태 기준은 있으나 dirty-check, 삭제 가능 조건 등은 보류 |
| 화면 검토 체크리스트 | `docs/13_SCREEN_REVIEW_CHECKLIST.md` | Ready | V5.5 화면 구조와 상태/메시지 검토 항목이 포함됨 |

판정:

- 문서별 역할은 대체로 분리되어 있다.
- TASK / REVIEW / 기준 문서 성격은 구분되어 있다.
- 오래된 gap은 최신 기준과 충돌하지 않도록 후속 review와 `24` 패치로 정리되었다.
- 구현 전 `Needs Decision` 항목이 남아 있음이 명시되어 있다.

## 5. UI / Wireframe Readiness

| 점검 항목 | 판정 | 근거 / 비고 |
|---|---|---|
| 현재 UI 기준은 V5.5로 관리되는가 | Ready with notes | 체크리스트와 V5.5 review 문서 기준. 단, `index.html` 제목의 버전 표기는 별도 확인 가능 |
| HTML 화면은 구현 전 기준 화면으로만 사용되는가 | Ready | 정적 Low-Fi HTML이며 기능 구현 지시가 아님 |
| `index.html`은 전체 화면 검토용 인덱스인가 | Ready | 와이어프레임 목록 진입 역할 |
| 사용자 첫 진입 홈 화면은 `01_home_dashboard.html`로 분리되어 있는가 | Ready | 홈 대시보드 별도 파일 존재 |
| 청구 흐름은 5단계로 고정되어 있는가 | Ready | 체크리스트와 데이터 모델 기준에서 5단계 유지 |
| `18_claim_document_register.html`은 보조 화면인가 | Ready | 청구 시작 단계 안의 청구 서류 등록 보조 화면으로 정리됨 |
| 등록/편집/상세 화면 닫기 기준이 있는가 | Ready | V5.5 액션 정렬 체크와 상태/메시지 기준에 포함 |
| 우측 패널은 Top 3 이후 최대 20건 + 페이지 영역 구조를 유지하는가 | Ready | V5.5 우측 패널 체크 항목으로 관리 |
| HTML/CSS/JS 수정 없이 문서 기준만 정리되었는가 | Ready | 현재 문서 단계 기준 |

## 6. Data Model Readiness

| 점검 항목 | 판정 | 비고 |
|---|---|---|
| `FamilyMember`가 기본 명칭이고 `Person`은 legacy alias인가 | Ready | `06`, `23`, `24` 기준 |
| `PolicyCoverage`가 기본 명칭이고 `Coverage`는 legacy alias인가 | Ready | 명칭과 planning object 정리 |
| `PolicyDocument` / `ClaimDocument`는 도메인 명칭으로 분리되었는가 | Ready | 물리 저장 구조와 분리해 설명 |
| 단일 `Document` 후보와 물리 분리 `Needs Decision`이 분리되어 있는가 | Ready | DB 테이블 확정 아님 |
| `OcrCandidate`는 OCR 후보값 객체로 정리되었는가 | Ready | Confirmed for planning |
| 사용자 확정값만 업무 객체에 반영하는 원칙이 유지되는가 | Ready | OCR 후보값 자동 반영 금지 |
| `ClaimReferenceResult` 전체 자동 저장이 확정되지 않았는가 | Ready | 선택/제출 판단 사용 결과 snapshot 후보만 검토 |
| `HistoryItem`은 projection 우선 후보로 남아 있는가 | Ready | 저장 객체 전환 여부는 `Needs Decision` |
| `Tag`, `ClaimMemo`, `HistoryMemo`는 Candidate로 유지되는가 | Ready | 확정 객체로 승격하지 않음 |
| 물리 DB 테이블 구조는 아직 확정하지 않았는가 | Ready | 구현 전 결정 대상으로 남김 |

## 7. Screen-to-Data Mapping Readiness

| 점검 항목 | 판정 | 비고 |
|---|---|---|
| 각 화면의 저장 객체와 조회 객체가 구분되어 있는가 | Ready | `docs/23_SCREEN_TO_DATA_MODEL_MAPPING.md`에 표로 정리 |
| 등록 화면과 조회/관리 화면이 혼동되지 않는가 | Ready | 문서함은 조회/관리 화면으로 분리 |
| 문서 연결 구조가 구분되는가 | Ready | `PolicyDocument -> Policy`, `ClaimDocument -> ClaimCase` |
| OCR 후보값과 사용자 확정값 경계가 유지되는가 | Ready | `06_ocr_review.html`, `23`, `33` 기준 |
| 상태값 후보가 확정 DB enum처럼 표현되지 않는가 | Ready with notes | 후보 상태로 정리되어 있으나 DB 설계 전 재검토 필요 |
| 민감정보 저장 기준이 반영되어 있는가 | Ready | 파일명, 경로, 후보값, 확정값 기준 포함 |

## 8. UI State / Message Readiness

| 점검 항목 | 판정 | 비고 |
|---|---|---|
| Empty / Loading / Error / Success / Warning / Confirm 상태 기준이 있는가 | Ready | `docs/33_UI_STATE_AND_CONFIRMATION_MESSAGE_GUIDE.md` |
| Empty 상태는 다음 행동을 안내하는가 | Ready | 체크리스트에 반영됨 |
| Loading 상태는 중복 클릭 방지 기준을 포함하는가 | Ready | 저장, 삭제, 완료, OCR 확인, 보험 찾기 기준 |
| Error 상태는 복구 행동을 안내하는가 | Ready | 오류 유형별 사용자 행동 기준 |
| Success 메시지는 민감정보 값을 반복하지 않는가 | Ready | 성공 메시지 기준에 포함 |
| 삭제와 사용 중지 메시지가 분리되어 있는가 | Ready with decisions required | 실제 삭제 가능 조건은 `Needs Decision` |
| 닫기 전 변경사항 확인 기준이 있는가 | Ready with decisions required | dirty-check 세부 기준은 미정 |
| OCR 후보값을 자동 확정값처럼 표현하지 않는가 | Ready | 후보값과 사용자 확정값 분리 |
| 청구 완료 메시지가 지급 완료를 암시하지 않는가 | Ready | `ClaimCase` 완료와 `ClaimSubmission` 완료 분리 |

## 9. Sensitive Data / Filename Readiness

| 점검 항목 | 판정 | 비고 |
|---|---|---|
| 실제 가족 실명 샘플을 사용하지 않는가 | Ready | 익명 샘플 기준 |
| 실제 보험사명, 병원명 샘플을 사용하지 않는가 | Ready | `보험사 A`, `병원 후보` 등 익명 표현 |
| 실제 진단코드 기반 개인 사례를 사용하지 않는가 | Ready | prefix 후보와 일반 태그 중심 |
| 고유식별번호, 계좌번호, 카드번호, 증권번호 전체값 저장 금지 기준이 있는가 | Ready | PRD, 데이터 모델 기준 |
| 파일명 민감정보 포함 금지 기준이 있는가 | Ready | `06`, `23`, `24`, `33` 기준 |
| 파일명은 내부 식별자, 날짜 범위, 문서 유형 수준으로 제한하는가 | Ready | 방향은 정리됨 |
| 원본 파일명 보존 여부와 세부 마스킹 포맷은 `Needs Decision`인가 | Ready with decisions required | 구현 전 결정 필요 |
| `attachments/`, `data/local/` 내부 파일은 비어 있어야 하는 기준이 유지되는가 | Ready | Git 추적 제외 및 빈 상태 기준 확인 필요 |

## 10. Needs Decision Before Implementation

| 항목 | 분류 | 이유 |
|---|---|---|
| 삭제 가능 조건 | Must Decide Before App Scaffold | 화면별 삭제/사용 중지 UX와 데이터 보존 정책에 영향 |
| 사용 중지 후 검색 노출 | Must Decide Before App Scaffold | 비활성 데이터의 조회/검색 노출 기준 필요 |
| dirty-check 기준 | Can Defer Until MVP Implementation | app scaffold 전 확정은 아니나 등록/편집 UX 구현 전 필요 |
| OCR 원문 예외 저장 조건 | Must Decide Before OCR Design | OCR 보안과 저장 범위에 직접 영향 |
| 사용자 확정값 저장 위치 | Must Decide Before DB Design | 업무 객체 반영과 별도 확정 기록 구조 결정 필요 |
| `ClaimReferenceResult` snapshot 저장 범위 | Must Decide Before DB Design | 판단 근거 보존과 민감정보 최소 저장 균형 필요 |
| `HistoryItem` 저장 객체 전환 여부 | Must Decide Before DB Design | projection 또는 저장 테이블 여부 결정 필요 |
| 파일명 마스킹 세부 포맷 | Must Decide Before App Scaffold | 파일 업로드/연결 UI와 저장 규칙에 영향 |
| 원본 파일명 보존 여부 | Must Decide Before App Scaffold | 민감정보 저장면과 파일 추적 정책에 영향 |
| `Tag` 별도 객체 여부 | Must Decide Before DB Design | `CategoryItem` 중심 MVP와 검색 확장 구조 결정 |
| `ClaimMemo` / `HistoryMemo` 별도 객체 여부 | Can Defer Until MVP Implementation | MVP는 단순 `memo` 필드 가능 |
| 청구 완료 상태 세부 | Must Decide Before App Scaffold | `ClaimCase` 완료와 `ClaimSubmission` 완료 표시 기준 필요 |
| 물리 DB 테이블 구조 | Must Decide Before DB Design | 현재 문서 단계에서는 확정 금지 |
| 앱 구현 방식 | Must Decide Before App Scaffold | WPF/WinForms/runtime scaffold 여부는 아직 정하지 않음 |
| OCR 구현 방식 | Must Decide Before OCR Design | 로컬 OCR 원칙은 있으나 구현 방식은 미정 |

## 11. Blocking Issues

현재 문서 기준에서 구현 계획 작성 자체를 막는 차단 이슈는 없다.

다만 app scaffold, DB 설계, OCR 설계 전에 반드시 결정해야 할 항목이 남아 있으므로 즉시 구현 착수 판정은 하지 않는다.

## 12. Non-Blocking Notes

- `index.html` 제목의 버전 표기는 V5.5 review 문서와 별도 확인이 가능하다.
- 초기 문서인 `03_USER_FLOW.md`, `04_SCREEN_LIST.md`, `05_WIREFRAME_SPEC.md`는 후속 V5.5 문서와 함께 읽어야 한다.
- `31_DATA_MODEL_TERMINOLOGY_CONSISTENCY_REVIEW.md`에는 과거 기준의 patch required 항목이 남아 있으나, 후속 `23`, `24`, `32` 기준에서 주요 gap이 정리되었다.
- 실제 브라우저 검토에서 화면별 Empty / Error 상태의 구체 문구는 추가 copy deck으로 분리할 수 있다.

## 13. Readiness Result

`READY_WITH_DECISIONS_REQUIRED`

문서, 와이어프레임, 데이터 모델, 화면-데이터 매핑, 상태/메시지 기준은 구현 계획을 작성할 수 있는 수준으로 정리되어 있다. 그러나 app scaffold, DB 설계, OCR 설계 전에 삭제/사용 중지 정책, 파일명 마스킹 포맷, 사용자 확정값 저장 위치, snapshot 저장 범위, 물리 DB 구조 등은 결정해야 한다.

## 14. Recommendation

- 구현 지시를 만들기 전에 `Needs Decision Before Implementation` 표의 `Must Decide Before App Scaffold` 항목을 먼저 정리한다.
- DB 설계 문서를 만들기 전에 `Must Decide Before DB Design` 항목을 별도 decision 문서로 분리한다.
- OCR 관련 설계 전에는 OCR 원문 예외 저장 조건과 OCR 구현 방식을 별도 승인 대상으로 둔다.
- 구현 지시서에는 앱 생성, DB 생성, OCR 구현을 한 번에 섞지 말고 app scaffold, 데이터 모델 설계, OCR 설계를 단계별로 분리한다.

## 15. Next Step

권장 다음 작업:

1. 구현 전 decision 문서 생성
2. app scaffold 전 결정 항목 확정
3. DB 설계 전 결정 항목 확정
4. OCR 설계 전 결정 항목 확정
5. 사용자가 개발 시작을 승인한 뒤에만 app scaffold 작업 지시 작성
