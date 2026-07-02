# Screen to Data Model Mapping

## 1. 목적

V5.5 와이어프레임 기준으로 화면별 입력값, 저장 객체, 조회 객체, 상태값, 문서 연결, 민감정보 여부를 정리한다.

이 문서는 개발 구현 문서가 아니다. 화면 검토 단계에서 데이터 경계를 확인하기 위한 기준 문서이며, `Candidate`와 `Needs Decision`은 아직 확정되지 않은 모델 판단을 뜻한다.

## 2. 핵심 객체 요약

| 객체 | 역할 | 현재 판단 |
|---|---|---|
| `FamilyMember` | 가족 표시명, 관계 후보, 사용 상태를 관리한다. | Confirmed. `Person`은 legacy alias |
| `Policy` | 가족 구성원에 연결되는 보험 기본정보를 관리한다. | Confirmed |
| `PolicyCoverage` | 보험의 담보/특약 및 약관 근거를 관리한다. | Confirmed for planning. `Coverage`는 legacy alias |
| `PolicyDocument` | 특정 `Policy`에 연결되는 보험 조회 캡처, 보험증권, 계약서, 약관 문서다. | Confirmed for naming. 물리 저장 분리는 Needs Decision |
| `ClaimCase` | 하나의 진료/청구 준비 단위다. | Confirmed |
| `ClaimDocument` | 특정 `ClaimCase`에 연결되는 청구용 병원 서류다. | Confirmed for naming. 물리 저장 분리는 Needs Decision |
| `Document` | 원본 문서의 물리 저장 후보다. | Candidate. 단일 `Document` + purpose/link 구조 우선 후보 |
| `OcrCandidate` | 문서에서 추출된 OCR 후보값이다. | Confirmed for planning. 업무 객체 자동 반영 금지 |
| `OcrExtraction` | OCR 실행 기록 후보다. | Candidate. 별도 객체 유지 여부는 Needs Decision |
| `ClaimReferenceResult` | 보험 찾기 화면의 담보 후보와 과거 유사 청구 Top 3 결과다. | Candidate. 저장 객체인지 조회 결과인지 결정 필요 |
| `ClaimSubmission` | 보험사별 청구 진행 기록이다. | Confirmed |
| `ClaimPayment` | 지급/부지급/감액 결과다. | Confirmed |
| `HistoryItem` | 이력 보기 목록에 표시되는 통합 이력 항목이다. | Candidate. 우선 projection 후보, 저장 객체 전환 여부는 Needs Decision |
| `Category` | 진료상황, 지급상태, 문서유형, 키워드/태그 같은 상위 분류다. | Confirmed for planning |
| `CategoryItem` | `Category`에 속한 선택 항목이다. | Confirmed for planning |
| `Tag` | 유사 청구 조회와 보험 검색에 쓰는 일반 태그다. | Candidate. MVP는 `CategoryItem` 중심, 별도 객체 여부는 Needs Decision |
| `ClaimMemo` | 청구 화면 메모를 별도 객체로 분리할 수 있는 후보 | Candidate. MVP는 단순 `memo` 필드로 시작 가능 |
| `HistoryMemo` | 이력 상세 메모를 별도 객체로 분리할 수 있는 후보 | Candidate. 별도 객체 여부는 Needs Decision |

Legacy alias:

- `Person`은 `FamilyMember`의 legacy alias다.
- `Coverage`는 `PolicyCoverage`의 legacy alias다.
- `Document`는 물리 저장 후보이며, 화면/도메인 명칭은 `PolicyDocument`와 `ClaimDocument`로 나눈다.
- `OcrExtraction`은 OCR 실행 기록 후보로 보류한다.
- `ReviewCandidate`는 `OcrCandidate.reviewStatus` 후보로 흡수할 수 있다.

## 3. 화면별 매핑 표

| 화면 | 화면 목적 | 입력값 | 저장 객체 | 조회 객체 | 상태값 | 문서 연결 | 민감정보 여부 | 비고 |
|---|---|---|---|---|---|---|---|---|
| `index.html` | 와이어프레임 전체 진입 | 메뉴 선택 | 없음 | 없음 | 없음 | 없음 | 낮음 | 정적 인덱스 |
| `01_home_dashboard.html` | 주요 메뉴와 진행 요약 제공 | 없음 | 없음 | `ClaimCase`, `ClaimSubmission`, `ClaimPayment`, `HistoryItem` | 진행 요약 후보 | 없음 | 있음 | `HistoryItem`은 projection 후보 |
| `02_family_members.html` | 가족 표시명과 사용 상태 관리 | 편집, 삭제, 사용 중지 선택 | `FamilyMember` | `FamilyMember`, 관련 `Policy`, `ClaimCase` 카운트 후보 | `active`, `disabled`, `delete_requested` Candidate | 없음 | 있음 | 삭제 제한은 Needs Decision |
| `13_family_register.html` | 가족 등록/편집 | 표시명, 관계 후보, 메모, 사용 여부 | `FamilyMember` | 기존 `FamilyMember` 후보 | `active`, `disabled` Candidate | 없음 | 있음 | 실명과 고유식별정보 저장 금지 |
| `15_manage_home.html` | 관리 기능 진입 | 없음 | 없음 | `FamilyMember`, `Policy`, `PolicyDocument`, `ClaimDocument`, `Category`, `CategoryItem` 요약 | 없음 | 없음 | 있음 | 관리 메뉴 허브 |
| `11_policy_manage.html` | 보험 목록 관리와 문서 추가 진입 | 보험 등록/편집, 편집, 문서 추가, 삭제, 사용 중지 | `Policy` | `Policy`, `FamilyMember`, `PolicyDocument`, `PolicyCoverage` 상태 | `active`, `disabled`, `delete_requested`, `needs_review` Candidate | 행 단위 `PolicyDocument` 추가 | 있음 | 보험 문서 등록은 행의 문서 추가에서 진입 |
| `12_policy_register.html` | 보험 기본정보 등록/편집 및 문서 연결 | 가족, 보험사 후보, 보험명 후보, 계약 상태, 기간, 문서 추가 | `Policy`, `PolicyDocument` 연결 후보 | `FamilyMember`, `PolicyDocument`, `PolicyCoverage` 후보 | `draft`, `saved`, `on_hold`, `disabled` Candidate | `PolicyDocument` 연결 | 있음 | 증권번호는 마스킹 메모만 허용 |
| `17_policy_document_register.html` | 선택된 보험에 보험 문서 연결 | 선택된 보험, 문서 유형, 문서 확인값, 담보 추출 대상 여부 | `PolicyDocument`, `OcrCandidate` 후보 | `Policy` 요약 | `registered`, `ocr_needed`, `user_confirmed` Candidate | `PolicyDocument -> Policy` | 있음 | 원본 파일은 `attachments/`에 두고 Git 제외 |
| `05_document_box.html` | 등록 문서 조회와 OCR 상태 확인 | 문서 유형 필터, OCR 확인 선택 | 없음 | `PolicyDocument`, `ClaimDocument`, `OcrCandidate` | `ocr_needed`, `ocr_confirmed`, `needs_review` Candidate | 보험 문서와 청구 서류 모두 조회 | 있음 | 등록 화면이 아니라 조회/관리 화면 |
| `07_claim_case.html` | 청구 시작과 서류/이미지 추가 | 가족, 진료일, 진료유형, 진단명 후보, 진단코드 prefix, 키워드/태그, 병원 후보, 금액, 약제비 여부, 메모 | `ClaimCase`, `ClaimDocument` 후보, `Tag` 연결 후보 | `FamilyMember`, `ClaimDocument`, `OcrCandidate` | `draft`, `saved`, `needs_ocr` Candidate | `ClaimDocument -> ClaimCase` | 있음 | 병원명은 후보값이며 실제 샘플 금지 |
| `18_claim_document_register.html` | 현재 청구 사건 후보에 청구 서류 연결 | 청구 서류 유형, 현재 청구 사건 후보, 가족, 진료일 후보 | `ClaimDocument`, `OcrCandidate` 후보 | `ClaimCase`, `FamilyMember` | `registered`, `ocr_needed`, `user_confirmed` Candidate | `ClaimDocument -> ClaimCase` | 있음 | 청구 시작 단계의 보조 화면 |
| `06_ocr_review.html` | 문서 후보, OCR 후보값, 사용자 확정값 확인 | 후보값 승인, 수정, 제외, 사용자 확인 상태 | `OcrCandidate`, 사용자 확정값 반영 대상 객체 | `PolicyDocument`, `ClaimDocument`, `OcrCandidate` | `candidate`, `edited`, `confirmed`, `ignored`, `needs_user_review` | 문서 목적에 따라 `PolicyDocument` 또는 `ClaimDocument` | 있음 | 사용자 확정값만 업무 객체에 반영 |
| `09_claim_reference_result.html` | 보험 찾기 결과와 과거 유사 청구 Top 3 검토 | 보험 선택 후보, 담보 후보, 제출 서류, 청구 메모 | `ClaimReferenceResult` Candidate, `ClaimCase` 보강 후보 | `Policy`, `PolicyCoverage`, `HistoryItem`, `ClaimSubmission`, `ClaimPayment`, `Tag` | `matched`, `needs_review`, `selected` Candidate | 선택 담보의 근거 문서 참조 | 있음 | 조건 불일치 담보는 제외, 확인 필요 담보만 표시 |
| `08_claim_submission.html` | 현재 청구 진행과 이력 요약 확인 | 진행 메모, 지급 결과 후보 | `ClaimSubmission`, `ClaimPayment`, 메모 후보 | `ClaimCase`, `ClaimSubmission`, `ClaimPayment`, `HistoryItem`, `ClaimReferenceResult` | `preparing`, `submitted`, `reviewing`, `paid`, `denied`, `cancelled` Candidate | 제출 서류 목록은 `ClaimDocument` 참조 | 있음 | 우측 패널은 조회 중심 |
| `14_claim_complete.html` | 청구 흐름 완료와 후속 이동 확인 | 완료 메모 후보 | `ClaimCase`, `ClaimSubmission` 완료 후보 | 저장 요약, `HistoryItem` 후보 | `case_completed`, `submission_completed` Needs Decision | 없음 | 있음 | `ClaimCase` 완료와 `ClaimSubmission` 완료 분리 필요 |
| `03_policy_list.html` | 조건 기반 보험 후보 검색 | 진단명, 진료상황, 기간, 키워드/태그 | 없음 | `Policy`, `PolicyCoverage`, `PolicyDocument`, `CategoryItem`, `Tag` | `matched`, `needs_review` Candidate | 약관 근거 문서 참조 | 있음 | 검색 조건은 진료 단서 |
| `04_policy_detail.html` | 보험 상세 기준 정보 검토 | 보험 선택 | 없음 | `Policy`, `PolicyCoverage`, `PolicyDocument` | `active`, `needs_review` Candidate | 연결 문서 조회 | 있음 | 문서 등록으로 이동 가능 |
| `10_history_view.html` | 통합 이력 목록 검색 | 가족, 보험사, 진료상황, 기간, 키워드/태그 | 없음 | `HistoryItem`, `ClaimCase`, `ClaimSubmission`, `ClaimPayment`, `Tag`, `CategoryItem` | 이력 상태 후보 | 관련 문서 요약 참조 | 있음 | `HistoryItem` 저장/projection 결정 필요 |
| `21_history_detail.html` | 선택 이력 상세 확인 | 사용자 메모, 확인 상태 | `HistoryMemo` Candidate 또는 `HistoryItem` 확인 상태 | `HistoryItem`, `ClaimCase`, `ClaimSubmission`, `ClaimPayment`, `ClaimDocument` | `checked`, `needs_follow_up` Candidate | 관련 청구 문서 참조 | 있음 | 메모 분리 여부 Needs Decision |
| `16_category_manage.html` | 분류와 항목 관계 관리 | 분류 등록, 항목 등록, 편집, 삭제 | `Category`, `CategoryItem`, `Tag` Candidate | `Category`, `CategoryItem` | `active`, `disabled`, `delete_requested` Candidate | 없음 | 있음 | 태그 조합은 민감정보 단서가 될 수 있음 |
| `19_category_register.html` | 상위 분류 등록/편집 | 분류명, 코드 후보, 사용 여부, 정렬 순서, 설명, 시스템 기본값 여부 | `Category` | 기존 `Category` 후보 | `active`, `disabled` Candidate | 없음 | 없음 | 연결 항목이 있으면 삭제 제한 필요 |
| `20_category_item_register.html` | 분류 안의 항목 등록/편집 | 상위 분류, 항목명, 코드 후보, 사용 여부, 정렬 순서, 설명, 검색 사용 여부 | `CategoryItem`, `Tag` Candidate | `Category`, 기존 `CategoryItem` 후보 | `active`, `disabled` Candidate | 없음 | 있음 | `Tag` 분리 여부 Needs Decision |

## 4. 객체별 사용 화면

| 객체 | 생성/수정 화면 | 조회 화면 | 참조 화면 | 비고 |
|---|---|---|---|---|
| `FamilyMember` | `13_family_register.html` | `02_family_members.html` | `07_claim_case.html`, `18_claim_document_register.html`, `10_history_view.html`, `12_policy_register.html` | `Person` legacy alias의 현재 기준 명칭 |
| `Policy` | `12_policy_register.html`, `11_policy_manage.html` | `03_policy_list.html`, `04_policy_detail.html`, `11_policy_manage.html` | `09_claim_reference_result.html`, `17_policy_document_register.html` | 삭제/사용 중지 정책 필요 |
| `PolicyCoverage` | `06_ocr_review.html` 사용자 확정 후 반영 Candidate | `03_policy_list.html`, `04_policy_detail.html`, `09_claim_reference_result.html` | `ClaimSubmission`, `ClaimReferenceResult` | 명칭은 Confirmed for planning. 세부 구현 범위와 자동 추출 범위는 Needs Decision |
| `PolicyDocument` | `17_policy_document_register.html`, `12_policy_register.html` 문서 추가 | `05_document_box.html`, `04_policy_detail.html`, `11_policy_manage.html` | `06_ocr_review.html`, `PolicyCoverage` 근거 | 도메인 명칭은 Confirmed for naming. 물리 저장은 단일 `Document` 후보 |
| `ClaimCase` | `07_claim_case.html`, `14_claim_complete.html` | `08_claim_submission.html`, `10_history_view.html`, `21_history_detail.html` | `18_claim_document_register.html`, `09_claim_reference_result.html` | 청구 준비 단위 |
| `ClaimDocument` | `18_claim_document_register.html`, `07_claim_case.html` | `05_document_box.html`, `21_history_detail.html` | `06_ocr_review.html`, `ClaimSubmission` 제출 서류 | 도메인 명칭은 Confirmed for naming. 물리 저장은 단일 `Document` 후보 |
| `OcrCandidate` | `06_ocr_review.html`, `17_policy_document_register.html`, `18_claim_document_register.html` | `05_document_box.html`, `06_ocr_review.html` | `PolicyCoverage`, `ClaimCase`, `ClaimDocument` 확정값 반영 | OCR 후보값 객체는 Confirmed for planning. 후보값/확정값 저장 경계는 Needs Decision |
| `OcrExtraction` | 없음 | 없음 | `OcrCandidate` 생성 전후 실행 기록 후보 | OCR 실행 기록 후보. 별도 객체 유지 여부는 Needs Decision |
| `ClaimReferenceResult` | `09_claim_reference_result.html` Candidate | `09_claim_reference_result.html`, `08_claim_submission.html` | `ClaimCase`, `PolicyCoverage`, `HistoryItem` | 전체 자동 저장은 미확정. 선택/제출 판단 사용 결과만 snapshot 저장 후보 |
| `ClaimSubmission` | `08_claim_submission.html`, `14_claim_complete.html` Candidate | `08_claim_submission.html`, `10_history_view.html`, `21_history_detail.html` | `ClaimPayment`, `HistoryItem` | 보험사별 진행 기록 |
| `ClaimPayment` | `08_claim_submission.html` Candidate | `08_claim_submission.html`, `10_history_view.html`, `21_history_detail.html` | `HistoryItem` | `ClaimCase` 직접 연결 여부 Needs Decision |
| `HistoryItem` | 없음 또는 projection 생성 | `10_history_view.html`, `21_history_detail.html`, `01_home_dashboard.html` | `09_claim_reference_result.html`, `08_claim_submission.html` | 우선 projection 후보. 저장 객체 전환 여부는 Needs Decision |
| `Category` | `19_category_register.html`, `16_category_manage.html` | `16_category_manage.html` | 검색 조건 화면, 등록 화면 | Confirmed for planning |
| `CategoryItem` | `20_category_item_register.html`, `16_category_manage.html` | `16_category_manage.html` | `03_policy_list.html`, `07_claim_case.html`, `10_history_view.html` | Confirmed for planning. MVP 태그성 항목의 중심 |
| `Tag` | `20_category_item_register.html` Candidate | `16_category_manage.html` | `03_policy_list.html`, `07_claim_case.html`, `09_claim_reference_result.html`, `10_history_view.html` | Candidate. 검색 규칙 확장 시 분리 검토 |
| `ClaimMemo` | 없음 또는 각 청구 화면의 `memo` 필드 | `08_claim_submission.html`, `14_claim_complete.html` | `ClaimCase`, `ClaimSubmission`, `ClaimPayment` | Candidate. 별도 객체 여부는 Needs Decision |
| `HistoryMemo` | `21_history_detail.html` Candidate | `21_history_detail.html` | `HistoryItem`, 원본 청구 객체 | Candidate. MVP는 단순 메모 필드 가능 |

## 5. 문서 연결 구조

- `PolicyDocument`는 `Policy`에 연결한다.
- `ClaimDocument`는 `ClaimCase`에 연결한다.
- `PolicyDocument`는 보험 문서 도메인 명칭이다.
- `ClaimDocument`는 청구 문서 도메인 명칭이다.
- 물리 저장 후보는 단일 `Document` 객체를 우선 검토한다.
- 단일 `Document`는 `documentPurpose`, `linkedPolicyId`, `linkedClaimCaseId` 등으로 보험 문서와 청구 문서의 연결 대상을 구분할 수 있다.
- 실제 DB 테이블 구조와 `PolicyDocument` / `ClaimDocument` 물리 분리 여부는 `Needs Decision`으로 유지한다.
- `OcrCandidate`는 문서에서 추출된 후보값이다.
- 사용자 확정값만 실제 업무 객체에 반영한다.
- 보험 조회 캡처, 보험증권, 계약서, 약관은 `PolicyDocument`로 분류한다.
- 진단서, 진료비 영수증, 약제비 영수증, 통원 확인 서류, 입퇴원 확인서, 수술 확인서는 `ClaimDocument`로 분류한다.
- 원본 파일은 `attachments/`에 두는 후보이며 Git 추적 대상이 아니다.
- DB 또는 로컬 메타 저장소에는 파일 경로, 문서 유형, 연결 대상, OCR 상태, 사용자 확인 상태만 저장하는 방향이 기준이다.

## 6. OCR 후보값과 사용자 확정값 분리

`OcrCandidate`는 OCR 후보값 객체이며 Confirmed for planning으로 둔다. 화면에서는 문서 후보, OCR 후보값, 사용자 확정값을 분리한다.

`OcrExtraction`은 OCR 실행 기록 후보로 보류한다. OCR 실행 기록과 사용자 검토 후보값은 분리해서 판단하며, `OcrExtraction` 별도 객체 유지 여부와 OCR 원문 전체 저장 여부는 `Needs Decision`이다.

`ReviewCandidate`는 별도 주 객체로 확정하지 않는다. 사용자 검토 상태는 `OcrCandidate.reviewStatus` 후보로 흡수할 수 있으며, 별도 객체화 여부는 `Needs Decision`으로 유지한다.

사용자 확정값은 다음 업무 객체 중 하나에 반영된다.

- 보험 문서에서 확정된 약관/담보 정보: `PolicyCoverage` Confirmed for planning
- 보험 문서의 유형, 연결 대상, 확인 상태: `PolicyDocument`
- 청구 서류에서 확정된 진료일, 진료유형, 금액, 태그 후보: `ClaimCase`
- 청구 서류의 유형, 연결 대상, 확인 상태: `ClaimDocument`

후보값 상태의 데이터는 보험 찾기 또는 지급 판단의 확정 근거로 쓰지 않는다.

## 7. 상태값 후보

| 대상 | 상태값 후보 | 비고 |
|---|---|---|
| `FamilyMember` | `active`, `disabled`, `delete_requested` | 삭제 제한 필요 |
| `Policy` | `draft`, `active`, `on_hold`, `disabled`, `delete_requested`, `needs_review` | 보험 등록/편집 화면 기준 |
| `PolicyCoverage` | `candidate`, `needs_review`, `user_confirmed`, `ignored` | OCR/약관 확인 연계 |
| `PolicyDocument`, `ClaimDocument` | `registered`, `ocr_needed`, `ocr_completed`, `user_confirmed`, `ignored` | 문서 목적 분리 필요 |
| `OcrCandidate` | `needs_user_review`, `edited`, `confirmed`, `ignored` | 기존 데이터 모델의 `ReviewCandidate` 상태와 유사 |
| `ClaimCase` | `draft`, `saved`, `needs_ocr`, `reference_checked`, `case_completed`, `cancelled` | 완료 기준 Needs Decision |
| `ClaimReferenceResult` | `generated`, `selected`, `ignored`, `expired` | 조회 결과 후보 상태. 전체 자동 저장은 미확정, snapshot 저장 범위는 Needs Decision |
| `ClaimSubmission` | `preparing`, `submitted`, `additional_documents_requested`, `reviewing`, `paid`, `denied`, `cancelled`, `submission_completed` | 기존 데이터 모델 상태 후보 유지 |
| `ClaimPayment` | `pending`, `paid`, `partially_paid`, `denied`, `cancelled` | 감액 사유 필요 |
| `Category`, `CategoryItem`, `Tag` | `active`, `disabled`, `delete_requested` | 사용 중지와 삭제 정책 필요 |

## 8. 민감정보 저장 기준

- 실제 가족 실명, 고유식별번호, 상세 주소, 계좌번호, 카드번호, 증권번호 전체값은 저장하지 않는다.
- 보험사명, 병원명, 진단명, 진단코드 prefix, 금액, 청구 상태, 지급 결과는 민감정보 단서로 취급한다.
- 화면 샘플은 `가족 A`, `보험사 A`, `병원 후보`처럼 익명화된 표현만 사용한다.
- 원본 문서와 이미지 파일은 Git 추적 제외 대상인 `attachments/`에 둔다.
- 파일명에는 실제 가족 실명, 병원명, 보험사명, 주민번호, 증권번호 전체값, 진단코드 기반 개인 사례가 들어가지 않도록 한다.
- 파일명은 내부 식별자, 날짜 범위, 문서 유형 수준으로 제한한다.
- 파일명 마스킹 규칙의 세부 포맷은 Needs Decision으로 유지한다.
- OCR 원문 전체 저장은 최소화하고, 후보값과 사용자 확정값의 보존 범위를 별도로 결정해야 한다.
- 검색용 `Tag`와 `CategoryItem`도 조합되면 진료 단서가 될 수 있으므로 민감정보 취급 기준이 필요하다.

## 9. Unknown / Needs Decision

| 항목 | 상태 | 설명 |
|---|---|---|
| `PolicyCoverage` 세부 구현 범위 | Needs Decision | 명칭과 계획상 객체는 Confirmed for planning이나, 담보 항목 자동 추출 범위와 세부 구현 범위는 결정 필요 |
| `PolicyDocument` / `ClaimDocument` 물리 분리 | Needs Decision | 도메인 명칭은 Confirmed for naming이나 저장 모델은 단일 `Document` + 목적 필드 후보가 남아 있음 |
| `OcrExtraction` 별도 객체 유지 여부 | Needs Decision | OCR 실행 기록과 후보값을 분리해 저장할지 결정 필요 |
| `OcrCandidate` 저장 경계 | Needs Decision | 후보값, 수정 전후값, 사용자 확정값 보존 범위 결정 필요 |
| 사용자 확정값 저장 경계 | Needs Decision | 확정값을 원본 후보와 함께 보존할지, 업무 객체에만 반영할지 결정 필요 |
| `ClaimReferenceResult` snapshot 저장 범위 | Needs Decision | 전체 자동 저장은 확정하지 않고, 선택/제출 판단 사용 결과의 snapshot 범위 결정 필요 |
| `HistoryItem` 저장 객체 전환 여부 | Needs Decision | 우선 projection 후보이며, 실제 테이블 전환 여부는 결정 필요 |
| `Tag` 분리 여부 | Needs Decision | `CategoryItem`의 일부인지 별도 검색 태그 객체인지 결정 필요 |
| 메모 객체 | Needs Decision | `ClaimMemo`, `HistoryMemo`를 별도 객체로 둘지 각 업무 객체 필드로 둘지 결정 필요 |
| 삭제와 사용 중지 | Needs Decision | 연결 데이터가 있는 경우 삭제 제한과 비활성 처리 정책 필요 |
| 파일 경로 저장 위치와 파일명 마스킹 | Needs Decision | 문서 메타데이터 저장소, 원본 파일 경로, 파일명 마스킹 세부 포맷 필요 |
