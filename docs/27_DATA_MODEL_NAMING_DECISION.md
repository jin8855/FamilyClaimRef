# Data Model Naming Decision

## 1. 목적

이 문서는 `docs/25_DATA_MODEL_NAMING_DECISION_DRAFT.md`의 후보를 기준으로, 구현 지시와 후속 문서에서 사용할 데이터 모델 명칭 기준을 정리한다.

이 문서는 최종 구현 스키마가 아니다. 문서와 구현 지시에서 사용할 명칭 기준이며, 물리 DB 테이블 구조 결정과는 분리한다.

## 2. 기준 문서

| 기준 문서 | 사용 목적 |
|---|---|
| `README.md` | 프로젝트 목적, 보안 원칙, 개발 전 시각화 게이트 확인 |
| `docs/06_DATA_MODEL.md` | 기존 데이터 모델 초안 명칭 확인 |
| `docs/23_SCREEN_TO_DATA_MODEL_MAPPING.md` | V5.5 화면별 입력/저장/조회 객체 확인 |
| `docs/24_DATA_MODEL_GAP_REVIEW.md` | 결정 필요 항목과 위험 확인 |
| `docs/25_DATA_MODEL_NAMING_DECISION_DRAFT.md` | rename / alias 후보 확인 |
| `docs/26_PRE_DEV_ARTIFACT_GAP_REVIEW.md` | 구현 전 보완 필요 항목 확인 |
| `docs/13_SCREEN_REVIEW_CHECKLIST.md` | 화면 검토 기준 확인 |
| `docs/17_WIREFRAME_V5_REVIEW.md` | V5 계열 와이어프레임 변경 기준 확인 |
| `C:\DevKnowledgeVault\00_Common\COMMON_OPERATION_GUIDE.md` | 기존 문서 overwrite 금지와 확인 사실/후보 분리 기준 확인 |
| `C:\DevKnowledgeVault\00_Common\MARKDOWN_DOCUMENT_RULES.md` | 한국어 기본 작성과 raw identifier 보존 기준 확인 |
| `C:\DevKnowledgeVault\00_Common\PRE_DEV_ARTIFACTS_TEMPLATE.md` | 개발 전 산출물 검토 기준 확인 |

## 3. 결정 요약

| 기존 명칭 | V5.5 기준 명칭 | 결정 | Alias 처리 | 비고 |
|---|---|---|---|---|
| `Person` | `FamilyMember` | Confirmed | `Person`은 legacy alias로 기록 | 문서/화면/구현 지시에서는 `FamilyMember` 사용 |
| `Policy` | `Policy` | Confirmed | alias 불필요 | 기존 명칭 유지 |
| `Coverage` | `PolicyCoverage` | Confirmed | `Coverage`는 legacy alias로 기록 | 특정 `Policy`에 종속된 담보/특약 의미를 명확히 함 |
| `Document` | `PolicyDocument`, `ClaimDocument` | Confirmed for document naming | `Document`는 physical storage alias 후보 | 문서/화면 명칭은 분리 확정, 물리 저장 구조는 Needs Decision |
| `OcrExtraction` | `OcrCandidate` | Candidate | OCR 실행/원문 추출 기록 alias로 유지 | OCR 실행 기록 자체는 별도 객체 여부 Needs Decision |
| `ReviewCandidate` | `OcrCandidate` 또는 사용자 확정값 검토 상태 | Candidate | OCR 후보 검토 상태 alias로 유지 | 사용자 확정값 저장 경계는 Needs Decision |
| 없음 | `ClaimReferenceResult` | Candidate | 신규 화면 결과 객체 후보 | 저장 객체 여부는 Needs Decision |
| 없음 | `HistoryItem` | Candidate | 신규 projection 후보 | 우선 projection 후보, 저장 객체 여부는 Needs Decision |
| 없음 | `Category` | Confirmed | 신규 관리 데이터 명칭 | 상위 분류 |
| 없음 | `CategoryItem` | Confirmed | 신규 관리 데이터 명칭 | 분류 하위 항목 |
| 없음 | `Tag` | Candidate | 검색용 태그 후보 | `CategoryItem`과 분리 여부 Needs Decision |
| 없음 | `ClaimMemo`, `HistoryMemo` | Candidate | 메모 분리 후보 | 즉시 확정하지 않음 |
| `ClaimCase` | `ClaimCase` | Confirmed | alias 불필요 | 청구 사건 단위 |
| `ClaimSubmission` | `ClaimSubmission` | Confirmed | alias 불필요 | 보험사별 청구 진행 |
| `ClaimPayment` | `ClaimPayment` | Confirmed | alias 불필요 | 지급 결과 |

## 4. 확정 명칭

다음 명칭은 문서와 구현 지시에서 기본 명칭으로 사용한다.

| 명칭 | 상태 | 설명 |
|---|---|---|
| `FamilyMember` | Confirmed | 가족 표시명, 관계 후보, 사용 상태를 관리하는 객체 |
| `Policy` | Confirmed | 가족 구성원에 연결되는 보험 기본정보 객체 |
| `PolicyCoverage` | Confirmed | 특정 보험에 종속된 담보/특약 객체 |
| `PolicyDocument` | Confirmed for document naming | 특정 보험에 연결되는 보험 문서 객체명 |
| `ClaimCase` | Confirmed | 하나의 진료/청구 준비 단위 |
| `ClaimDocument` | Confirmed for document naming | 특정 청구 사건에 연결되는 청구 서류 객체명 |
| `ClaimSubmission` | Confirmed | 보험사별 청구 진행 기록 |
| `ClaimPayment` | Confirmed | 지급/부지급/감액 결과 |
| `Category` | Confirmed | 진료상황, 지급상태, 문서유형, 키워드/태그 같은 상위 분류 |
| `CategoryItem` | Confirmed | `Category`에 속한 선택 항목 |

`PolicyDocument`와 `ClaimDocument`는 문서/화면 명칭으로 확정한다. 다만 물리 저장 구조가 단일 `Document`인지, 분리 테이블인지 여부는 아직 결정하지 않는다.

## 5. Alias / Legacy 명칭

| Legacy 명칭 | 현재 명칭 | 처리 원칙 |
|---|---|---|
| `Person` | `FamilyMember` | 기존 문서에서 발견되면 `FamilyMember`의 legacy alias로 해석 |
| `Coverage` | `PolicyCoverage` | 기존 문서에서 발견되면 `PolicyCoverage`의 legacy alias로 해석 |
| `Document` | `PolicyDocument` / `ClaimDocument` | 연결 대상이 `Policy`이면 `PolicyDocument`, `ClaimCase`이면 `ClaimDocument`로 해석 |
| `OcrExtraction` | `OcrCandidate` Candidate | OCR 실행 기록 또는 원문 추출 기록으로 해석하되, V5.5 화면의 후보값 명칭은 `OcrCandidate` 사용 |
| `ReviewCandidate` | `OcrCandidate` Candidate | OCR 후보 검토 상태 또는 사용자 확정값 검토 상태로 해석 |

## 6. Needs Decision으로 유지할 항목

| 항목 | 상태 | 이유 |
|---|---|---|
| `PolicyDocument` / `ClaimDocument`의 물리 저장 구조 | Needs Decision | 문서 명칭은 분리하되, 단일 `Document` + 목적 필드로 저장할지 물리 분리할지 미정 |
| `OcrExtraction` 유지 여부 | Needs Decision | OCR 실행 로그와 후보값을 분리할 필요가 있는지 결정 필요 |
| `OcrCandidate`와 사용자 확정값 저장 경계 | Needs Decision | 후보값 보존, 수정 전후값, 확정 시점 기록 범위 결정 필요 |
| `ClaimReferenceResult` 저장 여부 | Needs Decision | 일회성 조회 결과인지 청구 판단 근거 스냅샷인지 결정 필요 |
| `HistoryItem` 저장 여부 | Needs Decision | projection인지 저장 객체인지 결정 필요 |
| `Tag`와 `CategoryItem` 분리 여부 | Needs Decision | 단순 선택값인지 검색 랭킹/동의어/prefix 규칙을 갖는 태그인지 결정 필요 |
| `ClaimMemo` / `HistoryMemo` 분리 여부 | Needs Decision | 단순 필드인지 작성 이력과 확인 상태를 갖는 별도 객체인지 결정 필요 |
| 삭제와 사용 중지 정책 | Needs Decision | 연결 데이터가 있는 경우 물리 삭제 제한과 비활성 처리 기준 필요 |

## 7. docs/06_DATA_MODEL.md 반영 원칙

`docs/06_DATA_MODEL.md`를 나중에 수정할 경우 다음 원칙을 적용한다.

1. 기존 `Person`은 `FamilyMember`로 바꾸되, 문서 하단에 `Person` legacy alias를 기록한다.
2. 기존 `Coverage`는 `PolicyCoverage`로 바꾸되, `Coverage` legacy alias를 기록한다.
3. 기존 `Document`는 문서 목적에 따라 `PolicyDocument`와 `ClaimDocument`로 설명한다.
4. 물리 저장 구조는 확정하지 말고 `단일 Document 가능성`과 `분리 저장 가능성`을 Needs Decision으로 남긴다.
5. 기존 `OcrExtraction`과 `ReviewCandidate`는 `OcrCandidate` 중심으로 재정리하되, OCR 실행 기록 분리 여부는 Needs Decision으로 둔다.
6. `ClaimReferenceResult`, `HistoryItem`, `Category`, `CategoryItem`, `Tag`를 V5.5 화면 기준 객체 후보로 추가한다.
7. `ClaimMemo`, `HistoryMemo`는 별도 객체 후보로만 둔다.
8. 명칭 결정과 DB 테이블 생성 결정을 혼동하지 않는다.
9. 실제 개인정보 샘플이나 실제 보험사명, 병원명, 가족 실명을 추가하지 않는다.

## 8. 다음 작업

1. `docs/28_DATA_MODEL_V5_5_PROPOSED.md`를 검토해 `docs/06_DATA_MODEL.md` 반영 범위를 결정한다.
2. `PolicyDocument` / `ClaimDocument` 물리 저장 구조를 결정한다.
3. `OcrCandidate`와 사용자 확정값 저장 경계를 결정한다.
4. `HistoryItem`과 `ClaimReferenceResult`의 저장 여부를 결정한다.
5. `Tag`와 `CategoryItem`의 분리 여부를 결정한다.
6. 삭제와 사용 중지 정책을 확정한다.
7. 위 결정 후에만 `docs/06_DATA_MODEL.md` 수정 지시문을 별도로 작성한다.
