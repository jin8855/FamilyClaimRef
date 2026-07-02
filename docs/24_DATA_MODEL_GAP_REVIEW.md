# Data Model Gap Review

## 1. 목적

화면 기준으로 데이터 모델에서 아직 확정되지 않은 부분과 위험을 정리한다.

이 문서는 V5.5 정적 와이어프레임과 기존 `docs/06_DATA_MODEL.md`를 비교해 작성한 검토 문서다. 개발, DB 생성, OCR 구현 지시가 아니다.

최신 기준에서는 명칭 확정, planning object 확정, 물리 저장 구조 확정을 분리한다. 오래된 gap 표현은 `Resolved`, `Resolved for naming / planning`, `Still Valid Needs Decision`으로 재분류한다.

## 2. 확인된 객체

| 객체 | 확인 근거 | 판단 |
|---|---|---|
| `FamilyMember` | 가족 관리, 가족 등록/편집 화면 | Confirmed. 기존 `Person`은 legacy alias |
| `Policy` | 보험 관리, 보험 등록/편집, 보험 검색, 보험 상세 화면 | Confirmed |
| `ClaimCase` | 청구 시작, 청구 서류 등록, 진행 현황, 청구 완료 화면 | Confirmed |
| `ClaimSubmission` | 진행 현황, 이력 보기, 이력 상세 화면 | Confirmed |
| `ClaimPayment` | 진행 현황, 이력 보기, 이력 상세 화면 | Confirmed |
| `Category` | 분류/태그 관리, 분류 등록/편집 화면 | Confirmed for planning |
| `CategoryItem` | 분류/태그 관리, 항목 등록/편집 화면 | Confirmed for planning |

## 3. 추가/보강 객체와 미결정 후보

| 객체 후보 | 필요 이유 | 판단 |
|---|---|---|
| `PolicyCoverage` | 보험 찾기와 보험 상세에서 담보/특약 후보를 조회하고, 약관 문서 근거와 연결해야 한다. | Resolved for naming / planning. 담보 자동 추출 범위, 상세 속성, 물리 구현은 Needs Decision |
| `PolicyDocument` | 보험 조회 캡처, 보험증권, 계약서, 약관이 특정 보험에 종속된다. | Confirmed for naming. 물리 저장 분리 여부는 Needs Decision |
| `ClaimDocument` | 진단서, 영수증, 약제비 서류 등이 특정 청구 사건에 종속된다. | Confirmed for naming. 물리 저장 분리 여부는 Needs Decision |
| `OcrCandidate` | 문서 후보값과 사용자 확정값을 분리해야 한다. | Confirmed for planning. 저장 경계, review status, 사용자 확정값 보존 방식은 Needs Decision |
| `ClaimReferenceResult` | 보험 찾기 결과와 과거 유사 청구 Top 3가 화면상 독립 구조를 가진다. | Candidate. snapshot 저장 범위, 보존 기간, 재계산 정책은 Needs Decision |
| `HistoryItem` | 이력 보기와 이력 상세에서 통합 목록을 제공한다. | Projection 우선 Candidate. 저장 객체 전환 여부는 Needs Decision |
| `Tag` | 진단명, 키워드, 일반 태그, 진단코드 prefix 기반 검색에 쓰인다. | Candidate. `CategoryItem`과 별도 객체로 분리할지는 Needs Decision |
| `ClaimMemo` | 청구 시작, 진행 현황, 완료 화면의 메모 후보를 분리할 수 있다. | Candidate. MVP에서는 단순 `memo` 필드로 시작 가능하며 별도 객체 채택 여부는 Needs Decision |
| `HistoryMemo` | 이력 상세의 사용자 메모와 확인 상태를 별도로 보존할 수 있다. | Candidate. MVP에서는 단순 `memo` 필드로 시작 가능하며 별도 객체 채택 여부는 Needs Decision |

## 4. 결정 필요 사항

1. `PolicyCoverage`를 별도 객체로 둘 것인가

   기존 데이터 모델의 `Coverage`는 `PolicyCoverage`의 legacy alias로 정리한다. `PolicyCoverage` 명칭과 planning object는 정리된 것으로 본다. 다만 담보 자동 추출 범위, 상세 속성, 물리 구현 방식은 `Needs Decision`으로 유지한다.

2. `Tag`를 `CategoryItem`과 분리할 것인가

   `CategoryItem`은 진료상황, 문서유형, 지급상태 같은 관리 선택값까지 포함한다. 반면 `Tag`는 유사 청구 검색과 키워드 기반 조회에 특화된다. 검색 랭킹, 동의어, prefix 규칙을 둘 예정이면 분리 후보가 강하다.

3. `HistoryItem`을 저장 객체로 둘 것인가, 조회 projection으로 둘 것인가

   이력 보기는 `ClaimCase`, `ClaimSubmission`, `ClaimPayment`, 유사 청구 결과를 하나로 보여준다. 최신 기준에서는 우선 projection 후보로 둔다. 저장 객체로 전환하면 빠른 조회가 가능하지만 원본 상태와 동기화 위험이 생기므로 저장 객체 전환 여부는 `Needs Decision`으로 유지한다.

4. `ClaimMemo` / `HistoryMemo`를 별도 객체로 둘 것인가

   `ClaimMemo`와 `HistoryMemo`는 별도 객체 후보인 `Candidate`다. 화면에는 청구 메모, 진행 메모, 완료 메모, 이력 상세 메모가 분리되어 나타난다. MVP에서는 단순 `memo` 필드로 시작할 수 있지만, 작성 시점, 작성 위치, 확인 상태, 이력 보존이 필요하면 별도 객체 채택 여부를 결정해야 한다.

5. `OcrCandidate`와 사용자 확정값의 저장 경계를 어디로 둘 것인가

   `OcrCandidate`는 OCR 후보값 객체로 `Confirmed for planning`이다. OCR 후보값은 업무 객체에 자동 반영하지 않고, 사용자 확정값만 `PolicyDocument`, `ClaimDocument`, `PolicyCoverage`, `ClaimCase`에 반영한다. 다만 후보값 원문, 수정 전후값, 확정 시점, 확정값 저장 위치, 원본 후보 보존 범위는 `Needs Decision`으로 유지한다.

6. `ClaimCase` 완료와 `ClaimSubmission` 완료를 어떻게 분리할 것인가

   `ClaimCase` 완료와 `ClaimSubmission` 완료는 분리하는 방향으로 보정한다. `ClaimCase` 완료는 청구 흐름 화면의 완료이고, `ClaimSubmission` 완료는 보험사별 제출/심사/지급 흐름의 완료다. 화면상 `14_claim_complete.html`은 청구 사건 완료 확인에 가깝고, 실제 지급 완료는 `08_claim_submission.html`과 이력 보기에서 확인한다. 상태값 세부와 UI 반영 범위는 `Needs Decision`으로 유지한다.

7. `ClaimPayment`가 `ClaimSubmission`에만 종속되는지, `ClaimCase`에도 직접 연결되는지

   지급 결과는 `ClaimSubmission` 중심으로 보정한다. 기존 모델은 `ClaimSubmission -> ClaimPayment` 구조다. 화면에서는 지급 결과를 청구 사건 전체 이력에서도 보여주므로 `ClaimCase` 직접 연결처럼 보일 수 있다. 정규 모델은 `ClaimSubmission` 종속, `ClaimCase` 기준 표시는 조회 projection 또는 보조 연결 결정 사항으로 낮추는 후보가 안전하다.

8. 삭제와 사용 중지의 정책 차이

   가족, 보험, 분류, 항목 화면 모두 삭제와 사용 중지를 표시한다. 연결된 보험, 문서, 청구 이력이 있으면 물리 삭제를 제한하고 사용 중지로 전환하는 정책이 필요하다.

9. 문서 파일 경로와 메타데이터 저장 위치

   원본 문서와 이미지는 `attachments/`에 두고 Git에서 제외한다. 저장 객체에는 파일 경로, 원본 파일명 후보, 문서 유형, 연결 대상, OCR 상태, 확인 상태만 둘지 결정해야 한다. 파일명에는 실제 가족 실명, 병원명, 보험사명, 주민번호, 증권번호 전체값, 진단코드 기반 개인 사례가 들어가지 않도록 해야 한다. 파일명은 내부 식별자, 날짜 범위, 문서 유형 수준으로 제한하는 방향이 안전하다. 원본 파일명 보존 여부와 마스킹 세부 포맷은 `Needs Decision`으로 유지한다. `data/local/`에 로컬 인덱스나 OCR 임시 결과를 둘 경우 Git 제외와 삭제 정책이 필요하다.

10. 민감정보 마스킹 기준

    증권번호, 계좌번호, 카드번호, 고유식별번호 전체값은 저장하지 않는다. 병원명, 보험사명, 진단명, 진단코드 prefix, 금액, 지급 결과는 화면상 필요하지만 민감정보 단서이므로 마스킹, 최소 저장, 검색 인덱스 범위를 결정해야 한다. 파일명과 경로도 민감정보 저장면에 포함되므로 원본 파일명 보존 여부와 표시용 파일명 생성 규칙을 별도 gap으로 둔다.

## 5. 상태값 후보 정리

| 대상 | 상태값 후보 | 결정 위험 |
|---|---|---|
| 문서 | `registered`, `ocr_needed`, `ocr_completed`, `user_confirmed`, `ignored` | 보험 문서와 청구 문서가 같은 상태 집합을 쓸지 결정 필요 |
| OCR 후보 | `needs_user_review`, `edited`, `confirmed`, `ignored` | 확정 후 후보값을 삭제할지 보존할지 결정 필요 |
| 청구 사건 | `draft`, `saved`, `needs_ocr`, `reference_checked`, `case_completed`, `cancelled` | 청구 완료와 보험사별 제출 완료 혼동 가능 |
| 보험사별 청구 | `preparing`, `submitted`, `additional_documents_requested`, `reviewing`, `paid`, `denied`, `cancelled`, `submission_completed` | 지급 결과 상태와 중복될 수 있음 |
| 지급 결과 | `pending`, `paid`, `partially_paid`, `denied`, `cancelled` | 감액과 부지급 사유 구조 필요 |
| 관리 데이터 | `active`, `disabled`, `delete_requested` | 사용 중지와 삭제 정책 필요 |

## 6. 문서 연결 위험

- 단일 `Document` 객체만 쓰면 보험 문서와 청구 서류의 연결 대상이 모호해질 수 있다.
- `PolicyDocument`는 `Policy`에 종속되고, `ClaimDocument`는 `ClaimCase`에 종속된다는 화면 규칙을 유지해야 한다.
- `PolicyDocument` / `ClaimDocument` 도메인 명칭은 `Confirmed for naming`이지만, 물리 저장은 단일 `Document` 후보 우선이며 물리 분리 여부는 `Needs Decision`이다.
- `18_claim_document_register.html`은 청구 시작 단계의 보조 화면이며 단계바의 독립 단계가 아니다.
- 문서함은 등록 화면이 아니라 조회/관리 화면이다. 저장 이벤트는 등록 화면에서 발생해야 한다.
- 원본 파일 경로와 OCR 추출 결과가 Git 추적 대상에 들어가면 안 된다.
- 파일명에 실제 가족 실명, 병원명, 보험사명, 주민번호, 증권번호 전체값, 진단코드 기반 개인 사례가 들어가지 않도록 해야 한다.

## 7. OCR 후보값 / 사용자 확정값 위험

- OCR 후보값을 업무 객체에 바로 반영하면 사용자가 확인하지 않은 정보가 보험 찾기나 이력 조회에 쓰일 수 있다.
- 사용자 확정값만 업무 객체에 반영한다는 원칙은 확정된 기준이다.
- 사용자 확정값과 원본 후보값을 같은 필드에 덮어쓰면 추적성이 떨어진다.
- 보험 문서 OCR과 청구 서류 OCR은 확정 대상 객체가 다르다.
- OCR 원문 전체 저장은 기본 미저장 방향으로 둔다. 예외 저장 조건, 보존 기간, 마스킹 기준은 `Needs Decision`이다.
- 후보값 기반 검색 결과는 `candidate`임을 명확히 표시해야 한다.

## 8. 민감정보 저장 위험

- 가족 표시명이 실제 실명으로 바뀌면 민감정보가 된다.
- 보험사명, 병원명, 진단명, 금액, 지급 결과는 조합될 때 개인 의료/보험 이력을 추정할 수 있다.
- 일반 태그도 청구 사건과 결합되면 민감정보 단서가 된다.
- 파일 경로에 실제 이름이나 병원명이 포함되면 Git 제외만으로 충분하지 않을 수 있다.
- 파일명에도 실제 가족 실명, 병원명, 보험사명, 주민번호, 증권번호 전체값, 진단코드 기반 개인 사례가 포함되지 않아야 한다.
- 파일명은 내부 식별자, 날짜 범위, 문서 유형 수준으로 제한하는 방향이 안전하며, 원본 파일명 보존 여부와 마스킹 세부 포맷은 `Needs Decision`이다.
- 이력 보기 projection을 저장할 경우 원본보다 더 넓은 민감정보 조합이 생길 수 있다.

## 9. 다음 추천 작업

- `Person` -> `FamilyMember`, `Coverage` -> `PolicyCoverage`는 legacy alias로 유지하고 새 문서에서는 기본 명칭을 사용한다.
- `PolicyDocument`와 `ClaimDocument`는 도메인 명칭으로 유지하되, 단일 `Document` 물리 저장 후보와 분리 저장 가능성을 비교한다.
- `HistoryItem`은 projection 우선 후보로 두고 저장 객체 전환 필요성을 별도 검토한다.
- `OcrCandidate`와 사용자 확정값의 저장 위치, 보존 범위, review status를 정한다.
- `ClaimReferenceResult`는 전체 자동 저장으로 확정하지 말고, 사용자가 선택하거나 제출 판단에 사용한 결과의 snapshot 범위만 검토한다.
- `ClaimMemo` / `HistoryMemo`는 MVP 단순 `memo` 필드로 충분한지 먼저 검토한다.
- 파일명 마스킹 세부 규칙과 원본 파일명 보존 여부를 확정한다.
- 삭제/사용 중지 정책과 민감정보 마스킹 기준을 개발 착수 전 확정한다.
