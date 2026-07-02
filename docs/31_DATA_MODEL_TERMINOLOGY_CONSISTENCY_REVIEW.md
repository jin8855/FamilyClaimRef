# Data Model Terminology Consistency Review

## 1. 목적

`docs/06_DATA_MODEL.md`와 `docs/23_SCREEN_TO_DATA_MODEL_MAPPING.md` 사이의 객체명, 상태값, 문서 연결, OCR 경계, 민감정보 기준이 일치하는지 검토한다.

이번 문서는 REVIEW 문서다. 기존 문서를 직접 수정하지 않고, 불일치와 수정 후보만 기록한다.

## 2. 검토 대상

| 문서 | 검토 목적 |
|---|---|
| `docs/06_DATA_MODEL.md` | V5.5 기준으로 보수 수정된 데이터 모델 기준 확인 |
| `docs/23_SCREEN_TO_DATA_MODEL_MAPPING.md` | 화면별 입력/저장/조회 객체와 상태값 확인 |
| `docs/24_DATA_MODEL_GAP_REVIEW.md` | 기존 gap과 미결정 항목 확인 |
| `docs/27_DATA_MODEL_NAMING_DECISION.md` | 명칭 결정과 legacy alias 기준 확인 |
| `docs/28_DATA_MODEL_V5_5_PROPOSED.md` | V5.5 제안 모델과 상태값 후보 확인 |
| `docs/29_DATA_MODEL_CORE_DECISIONS.md` | 핵심 1차 결정과 보류 항목 확인 |
| `docs/30_DATA_MODEL_06_UPDATE_PLAN.md` | `06_DATA_MODEL.md` 반영 계획 확인 |
| `C:\DevKnowledgeVault\00_Common\COMMON_OPERATION_GUIDE.md` | 확인 사실, 후보, 미확정 항목 분리 원칙 확인 |
| `C:\DevKnowledgeVault\00_Common\MARKDOWN_DOCUMENT_RULES.md` | Markdown 작성과 Candidate / Needs Decision 구분 기준 확인 |

## 3. 요약 판정

판정: `PATCH_REQUIRED`

요약:

- 객체명 자체는 대부분 같은 의미로 연결된다.
- `PolicyDocument`, `ClaimDocument`, `OcrCandidate`, `HistoryItem`, `ClaimReferenceResult`의 핵심 의미는 두 문서에서 크게 충돌하지 않는다.
- 다만 `docs/06_DATA_MODEL.md`가 최신 결정 상태를 반영하면서, `docs/23_SCREEN_TO_DATA_MODEL_MAPPING.md`에는 이전 `Candidate` / `Needs Decision` 표현이 일부 남아 있다.
- 특히 `PolicyCoverage`, `Category`, `CategoryItem`, `PolicyDocument`, `ClaimDocument`, `OcrCandidate`의 상태 표현은 후속 정리가 필요하다.
- 구현을 즉시 막는 `BLOCKER`는 아니지만, 구현 지시 전에는 `docs/23_SCREEN_TO_DATA_MODEL_MAPPING.md`의 용어 상태를 최신 기준으로 패치하는 것이 안전하다.

## 4. 객체명 정합성

| 객체 | 06_DATA_MODEL.md | 23_SCREEN_TO_DATA_MODEL_MAPPING.md | 판정 | 비고 |
|---|---|---|---|---|
| `FamilyMember` | 기본 명칭, `Confirmed` | 기본 명칭, `Confirmed` | PASS | `Person` legacy alias와 연결됨 |
| `Policy` | 기본 명칭, `Confirmed` | 기본 명칭, `Confirmed` | PASS | 의미 일치 |
| `PolicyCoverage` | `Confirmed` | 핵심 요약에서는 `Candidate`, 일부 표에서는 별도 객체 여부 `Needs Decision` | PATCH_REQUIRED | 27/06 기준은 명칭 확정에 가까우므로 23의 상태 표현 갱신 필요 |
| `PolicyDocument` | `Confirmed for naming`, 물리 분리 `Needs Decision` | `Candidate`, 기존 `Document` 분리 후보 | PATCH_REQUIRED | 의미는 맞지만 상태 표현이 구버전 기준 |
| `ClaimDocument` | `Confirmed for naming`, 물리 분리 `Needs Decision` | `Candidate`, 기존 `Document` 분리 후보 | PATCH_REQUIRED | 의미는 맞지만 상태 표현이 구버전 기준 |
| `Document` | 단일 물리 저장 후보, `Candidate` | 원본 파일/메타데이터 저장 방향은 있음, 단일 물리 저장 후보 표현은 약함 | PASS_WITH_NOTES | 23에 단일 `Document` 물리 후보 설명 보강 가능 |
| `OcrCandidate` | 후보값 객체, `Confirmed for planning` | 후보값 객체, `Candidate` | PATCH_REQUIRED | 의미는 일치하나 상태 표현 차이 |
| `OcrExtraction` | OCR 실행 기록 후보, `Candidate` | 핵심 객체 목록에는 없음, `OcrCandidate` 범위 논의에만 간접 반영 | PATCH_REQUIRED | legacy alias / 실행 기록 후보로 명시 보강 필요 |
| `ClaimCase` | 기본 명칭, `Confirmed` | 기본 명칭, `Confirmed` | PASS | 완료와 제출 완료 분리 원칙도 일치 |
| `ClaimReferenceResult` | 조회 결과 객체, `Candidate`, 전체 자동 저장 미확정 | 보험 찾기 결과 객체, `Candidate`, 저장 여부 `Needs Decision` | PASS_WITH_NOTES | 의미 일치. snapshot 저장 범위 표현은 06이 더 구체적 |
| `ClaimSubmission` | 기본 명칭, `Confirmed` | 기본 명칭, `Confirmed` | PASS | 의미 일치 |
| `ClaimPayment` | 기본 명칭, `Confirmed` | 기본 명칭, `Confirmed` | PASS | 의미 일치 |
| `HistoryItem` | projection 우선, `Candidate`, 저장 객체 확정 금지 | 저장 객체 또는 projection 선택 필요, `Needs Decision` | PASS_WITH_NOTES | 방향은 같지만 06이 projection 우선으로 더 구체적 |
| `Category` | `Confirmed for planning` | `Candidate` | PATCH_REQUIRED | 27/06 기준으로 23 상태 업데이트 필요 |
| `CategoryItem` | `Confirmed for planning` | `Candidate` | PATCH_REQUIRED | 27/06 기준으로 23 상태 업데이트 필요 |
| `Tag` | `Candidate`, MVP는 `CategoryItem` 중심 | `Needs Decision`, `CategoryItem`과 분리 여부 필요 | PASS_WITH_NOTES | 의미는 일치. 23에 MVP 기준을 보강하면 좋음 |
| `ClaimMemo` | `Candidate` | 일부 화면/Needs Decision에서 간접 언급 | PASS_WITH_NOTES | 23 핵심 객체 요약에는 없음 |
| `HistoryMemo` | `Candidate` | 이력 상세 화면과 메모 객체 Needs Decision에 있음 | PASS_WITH_NOTES | 의미 일치 |

## 5. 상태값 정합성

| 대상 | 06 기준 | 23 기준 | 판정 | 보완 필요 여부 |
|---|---|---|---|---|
| `FamilyMember` | `active`, `disabled`, `delete_requested` | 상태값 표는 동일. 일부 화면 행은 `active`, `disabled`만 표시 | PASS_WITH_NOTES | 화면별 입력 행에는 삭제 요청 상태 누락 가능 |
| `Policy` | `draft`, `active`, `on_hold`, `disabled`, `delete_requested`, `needs_review` | 상태값 표는 동일. 일부 화면 행은 `saved`, `on_hold` 등 혼재 | PASS_WITH_NOTES | 23 내부 화면별 상태 표현 정리 후보 |
| `PolicyCoverage` | `candidate`, `needs_review`, `user_confirmed`, `ignored` | 상태값 표는 동일 | PASS | 객체 상태 판정만 보정 필요 |
| `PolicyDocument`, `ClaimDocument` | `registered`, `ocr_needed`, `ocr_completed`, `user_confirmed`, `ignored` | 상태값 표는 동일 | PASS | 의미 일치 |
| `OcrCandidate` | `needs_user_review`, `edited`, `confirmed`, `ignored` | 상태값 표는 동일 | PASS | 의미 일치 |
| `ClaimCase` | `draft`, `saved`, `needs_ocr`, `reference_checked`, `case_completed`, `cancelled` | 상태값 표는 동일 | PASS | 의미 일치 |
| `ClaimReferenceResult` | 06 상태값 표에는 없음. 후보 객체와 snapshot 저장 범위만 설명 | `generated`, `selected`, `ignored`, `expired` | PATCH_REQUIRED | 06 또는 23 중 하나로 정합화 필요. 28 기준은 23과 동일 |
| `ClaimSubmission` | `preparing`, `submitted`, `additional_documents_requested`, `reviewing`, `paid`, `denied`, `cancelled`, `submission_completed` | 상태값 표는 동일 | PASS | 의미 일치 |
| `ClaimPayment` | `pending`, `paid`, `partially_paid`, `denied`, `cancelled` | 상태값 표는 동일 | PASS | 의미 일치 |
| `Category`, `CategoryItem`, `Tag` | `active`, `disabled`, `delete_requested` | 상태값 표는 동일 | PASS | 객체 상태 판정만 보정 필요 |

## 6. 문서 연결 정합성

판정: `PASS_WITH_NOTES`

일치하는 항목:

- `PolicyDocument`는 `Policy`에 연결된다.
- `ClaimDocument`는 `ClaimCase`에 연결된다.
- 원본 문서는 `attachments/` 하위 후보이며 Git 추적 대상이 아니다.
- 메타 저장소에는 파일 경로, 문서 유형, 연결 대상, OCR 상태, 사용자 확인 상태만 저장하는 방향이 유지된다.
- `PolicyDocument` / `ClaimDocument` 물리 분리 여부는 확정하지 않는다.

보완 후보:

- `docs/23_SCREEN_TO_DATA_MODEL_MAPPING.md`는 단일 `Document` + `documentPurpose` + `linkedPolicyId` + `linkedClaimCaseId` 물리 저장 후보를 `docs/06_DATA_MODEL.md`만큼 명시하지 않는다.
- `docs/23_SCREEN_TO_DATA_MODEL_MAPPING.md`의 `PolicyDocument` / `ClaimDocument`는 여전히 `Candidate`로 표현되어 있어, “도메인 명칭은 확정, 물리 저장은 Needs Decision” 구조로 조정할 필요가 있다.

## 7. OCR 후보값 / 사용자 확정값 정합성

판정: `PASS_WITH_NOTES`

일치하는 항목:

- `OcrCandidate`는 후보값이다.
- OCR 후보값은 업무 객체에 자동 반영하지 않는다.
- 사용자 확정값만 `PolicyDocument`, `ClaimDocument`, `PolicyCoverage`, `ClaimCase` 등에 반영한다.
- 후보값 상태의 데이터는 보험 찾기나 지급 판단의 확정 근거로 쓰지 않는다.
- OCR 원문 전체 저장은 최소화하거나 기본 저장하지 않는 방향이다.

보완 후보:

- `docs/23_SCREEN_TO_DATA_MODEL_MAPPING.md`는 `OcrExtraction`을 별도 실행 기록 후보로 명시하지 않는다.
- `ReviewCandidate`가 `OcrCandidate.reviewStatus` 후보로 흡수될 수 있다는 legacy alias 설명이 `docs/23_SCREEN_TO_DATA_MODEL_MAPPING.md`에는 약하다.
- `docs/23_SCREEN_TO_DATA_MODEL_MAPPING.md`의 `OcrCandidate` 상태가 `Candidate`로 남아 있어, 06의 `Confirmed for planning`과 표현 차이가 있다.

## 8. 민감정보 기준 정합성

판정: `PASS_WITH_NOTES`

일치하는 항목:

- 실제 가족 실명, 고유식별번호, 상세 주소, 계좌번호, 카드번호, 증권번호 전체값은 저장 금지다.
- 보험사명, 병원명, 진단명, 진단코드 prefix, 금액, 지급 결과는 민감정보 단서로 취급한다.
- 원본 문서와 이미지는 `attachments/`에 두는 후보이며 Git 추적 대상이 아니다.
- OCR 원문 전체 저장은 최소화한다.
- 태그와 분류 항목도 조합되면 진료 단서가 될 수 있다.

보완 후보:

- `docs/06_DATA_MODEL.md`는 파일명에 실제 이름, 병원명, 주민번호, 증권번호 전체값이 들어가지 않도록 한다는 기준을 추가했다.
- `docs/23_SCREEN_TO_DATA_MODEL_MAPPING.md`에는 파일명 마스킹 기준이 상대적으로 약하다.

## 9. 발견된 불일치

| 항목 | 문서 | 내용 | 위험 | 수정 후보 |
|---|---|---|---|---|
| `PolicyCoverage` 상태 | `docs/23_SCREEN_TO_DATA_MODEL_MAPPING.md` | 핵심 요약에서 `Candidate`, Unknown에서 별도 객체 여부 `Needs Decision`으로 남아 있음 | 06/27의 명칭 확정 기준과 어긋남 | `Confirmed` 또는 `Confirmed for planning`으로 조정하고, 미결정은 물리/세부 구현 범위로 제한 |
| `PolicyDocument` / `ClaimDocument` 상태 | `docs/23_SCREEN_TO_DATA_MODEL_MAPPING.md` | `Candidate`로 남아 있음 | 도메인 명칭 확정과 물리 저장 미결정이 섞임 | `Confirmed for naming`, 물리 분리는 `Needs Decision`으로 분리 |
| `OcrCandidate` 상태 | `docs/23_SCREEN_TO_DATA_MODEL_MAPPING.md` | `Candidate`로 남아 있음 | 06의 `Confirmed for planning`과 다름 | 후보값 객체 명칭은 `Confirmed for planning`, 저장 경계는 `Needs Decision`으로 분리 |
| `OcrExtraction` alias | `docs/23_SCREEN_TO_DATA_MODEL_MAPPING.md` | OCR 실행 기록 후보 설명이 약함 | 실행 로그와 후보값 경계가 흐려질 수 있음 | legacy alias / 실행 기록 후보로 보강 |
| `Category` / `CategoryItem` 상태 | `docs/23_SCREEN_TO_DATA_MODEL_MAPPING.md` | `Candidate`로 남아 있음 | 06/27의 관리 데이터 명칭 기준과 차이 | `Confirmed for planning`으로 조정 |
| `ClaimReferenceResult` 상태값 | `docs/06_DATA_MODEL.md`, `docs/23_SCREEN_TO_DATA_MODEL_MAPPING.md` | 23에는 `generated`, `selected`, `ignored`, `expired`가 있으나 06 상태값 표에는 없음 | 상태값 표 기준이 서로 다르게 보일 수 있음 | 06에 추가하거나 23에서 별도 후보 상태임을 명시 |
| 파일명 마스킹 기준 | `docs/23_SCREEN_TO_DATA_MODEL_MAPPING.md` | 06보다 설명이 약함 | 파일 경로/파일명 민감정보 기준 누락 위험 | 23 민감정보 기준에 파일명 마스킹 후보 추가 |
| `ClaimMemo` 핵심 요약 | `docs/23_SCREEN_TO_DATA_MODEL_MAPPING.md` | 화면별 표에는 간접적으로 보이나 핵심 객체 요약에는 없음 | 메모 후보 누락으로 보일 수 있음 | 핵심 객체 요약에 `ClaimMemo` Candidate 추가 후보 |

## 10. 수정 필요 후보

후속 패치 대상 후보는 `docs/23_SCREEN_TO_DATA_MODEL_MAPPING.md`다.

권장 수정 후보:

1. `PolicyCoverage`를 `Confirmed` 또는 `Confirmed for planning`으로 정리한다.
2. `PolicyDocument`, `ClaimDocument`를 `Confirmed for naming`으로 정리하고, 물리 분리는 `Needs Decision`으로 분리한다.
3. `Document`는 단일 물리 저장 후보로 명시한다.
4. `OcrCandidate`를 `Confirmed for planning`으로 정리하고, 사용자 확정값 저장 경계는 `Needs Decision`으로 유지한다.
5. `OcrExtraction`은 OCR 실행 기록 후보로 명시한다.
6. `ReviewCandidate`는 `OcrCandidate.reviewStatus` 후보로 명시한다.
7. `Category`, `CategoryItem`은 `Confirmed for planning`으로 정리한다.
8. `Tag`는 `Candidate`, 별도 객체 여부는 `Needs Decision`으로 분리한다.
9. `ClaimMemo`, `HistoryMemo`를 핵심 객체 요약에 `Candidate`로 추가한다.
10. `ClaimReferenceResult` 상태값 후보를 06/23 중 어느 문서에 둘지 정한다.
11. 파일명 마스킹 기준을 23의 민감정보 기준에 보강한다.

## 11. 수정하지 않아도 되는 차이

- `docs/06_DATA_MODEL.md`는 데이터 모델 기준 문서이고, `docs/23_SCREEN_TO_DATA_MODEL_MAPPING.md`는 화면별 매핑 문서이므로 화면별 상태값이 더 좁게 표현되는 것은 허용 가능하다.
- `HistoryItem`은 06에서 projection 우선으로 더 구체화되었고, 23에서 `Needs Decision`으로 표현되어 있으나 의미 충돌은 아니다.
- `ClaimReferenceResult`는 두 문서 모두 전체 자동 저장으로 확정하지 않으므로 핵심 원칙은 일치한다.
- `Tag`는 06에서 `Candidate`, 23에서 `Needs Decision`으로 표현되지만, 둘 다 별도 객체 확정은 하지 않는다.
- `ClaimPayment`가 `ClaimSubmission`에 종속된다는 방향은 두 문서에서 유지된다.

## 12. 다음 작업

1. `docs/23_SCREEN_TO_DATA_MODEL_MAPPING.md`만 대상으로 하는 좁은 패치 지시를 작성한다.
2. 패치 범위는 객체 상태 표현, legacy alias, 문서 물리 저장 후보, OCR 실행 기록 후보, 파일명 마스킹 기준에 제한한다.
3. `docs/06_DATA_MODEL.md`는 이번 검토 기준으로 추가 수정하지 않는다.
4. `Candidate` / `Needs Decision`을 근거 없이 `Confirmed`로 승격하지 않는다.
5. 패치 후 다시 `docs/06_DATA_MODEL.md`와 `docs/23_SCREEN_TO_DATA_MODEL_MAPPING.md`만 해시 또는 diff 기준으로 검증한다.
