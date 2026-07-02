# Data Model Naming Decision Draft

## 1. 목적

이 문서는 기존 `docs/06_DATA_MODEL.md`의 객체명과 V5.5 화면 기준 객체명을 비교하고, rename / alias / 유지 여부를 결정하기 위한 초안이다.

이 문서는 최종 결정 문서가 아니다. `DECISION DRAFT`이며, 기존 데이터 모델 문서를 직접 수정하지 않는다.

## 2. 기준 문서

| 기준 문서 | 사용 목적 |
|---|---|
| `README.md` | 프로젝트 목적, 보안 원칙, 개발 전 시각화 게이트 확인 |
| `docs/06_DATA_MODEL.md` | 기존 데이터 모델 초안의 객체명과 관계 확인 |
| `docs/23_SCREEN_TO_DATA_MODEL_MAPPING.md` | V5.5 화면 기준 입력/저장/조회 객체 확인 |
| `docs/24_DATA_MODEL_GAP_REVIEW.md` | 미결정 객체와 위험 항목 확인 |
| `docs/13_SCREEN_REVIEW_CHECKLIST.md` | 화면 검토 기준과 V5.5 액션 정렬 기준 확인 |
| `docs/17_WIREFRAME_V5_REVIEW.md` | V5 계열 화면 구조 변화 확인 |
| `C:\DevKnowledgeVault\00_Common\MARKDOWN_DOCUMENT_RULES.md` | 한국어 기본 작성, 확인 사실/후보/미확정 분리 원칙 확인 |
| `C:\DevKnowledgeVault\00_Common\COMMON_OPERATION_GUIDE.md` | 공통 문서와 프로젝트 문서 경계, 기존 문서 overwrite 금지 원칙 확인 |
| `C:\DevKnowledgeVault\00_Common\PRE_DEV_ARTIFACTS_TEMPLATE.md` | 개발 전 산출물 점검 기준 확인 |
| `C:\DevKnowledgeVault\00_Common\UI_COMMON_GUIDE.md` | 화면 검토 산출물 기준 확인 |

## 3. 확인된 객체명 차이

| 기존 명칭 | V5.5 화면 기준 명칭 | 판단 | 근거 | 결정 필요 여부 |
|---|---|---|---|---|
| `Person` | `FamilyMember` | Candidate | 화면은 가족 표시명, 관계 후보, 사용 상태를 `가족 관리`로 다룬다. `Person`보다 `FamilyMember`가 화면 목적과 맞다. | Needs Decision |
| `Policy` | `Policy` | Confirmed | 기존 모델과 화면 모두 보험 기본정보를 `Policy`로 볼 수 있다. | 불필요 |
| `Coverage` | `PolicyCoverage` | Candidate | 화면은 보험에 종속된 담보/특약, 약관 근거, 조건 일치 후보를 다룬다. | Needs Decision |
| `Document` | `PolicyDocument` / `ClaimDocument` | Candidate | V5.5 화면은 보험 문서와 청구 서류의 연결 대상이 다르다. | Needs Decision |
| `OcrExtraction` | `OcrCandidate` | Candidate | 화면은 OCR 원문 처리보다 후보값 확인과 사용자 확정값 분리를 중심으로 한다. | Needs Decision |
| `ReviewCandidate` | `OcrCandidate` 또는 사용자 확정값 검토 상태 | Candidate | 기존 `ReviewCandidate`는 OCR 후보 검토와 겹치지만, 사용자 확정값은 업무 객체에 반영되는 별도 경계가 필요하다. | Needs Decision |
| 없음 | `ClaimReferenceResult` | Candidate | 보험 찾기 화면의 담보 후보, 확인 필요 담보, 과거 유사 청구 Top 3 결과가 화면상 독립된다. | Needs Decision |
| 없음 | `HistoryItem` | Needs Decision | 이력 보기는 통합 이력 목록을 제공하지만 저장 객체인지 projection인지 미정이다. | Needs Decision |
| 없음 | `Category` | Candidate | 분류/태그 관리 화면에서 상위 분류를 관리한다. | Needs Decision |
| 없음 | `CategoryItem` | Candidate | 분류 안의 선택 항목을 등록/편집한다. | Needs Decision |
| 없음 | `Tag` | Needs Decision | 검색용 일반 태그가 `CategoryItem`과 분리되어야 하는지 미정이다. | Needs Decision |
| 없음 | `ClaimMemo` / `HistoryMemo` 후보 | Needs Decision | 청구 메모, 진행 메모, 완료 메모, 이력 상세 메모가 화면에 나타나지만 별도 객체 여부는 미정이다. | Needs Decision |
| `ClaimCase` | `ClaimCase` | Confirmed | 기존 모델과 V5.5 화면 모두 하나의 진료/청구 준비 단위로 사용한다. | 불필요 |
| `ClaimSubmission` | `ClaimSubmission` | Confirmed | 기존 모델과 V5.5 화면 모두 보험사별 청구 진행 기록으로 사용한다. | 불필요 |
| `ClaimPayment` | `ClaimPayment` | Confirmed | 기존 모델과 V5.5 화면 모두 지급/부지급/감액 결과로 사용한다. | 불필요 |

## 4. Rename / Alias 후보

| 결정 항목 | 권장 방향 | 대안 | 선택하지 않을 경우 위험 |
|---|---|---|---|
| `Person` 명칭 | `FamilyMember`로 rename | `Person` 유지 후 화면 alias로 `FamilyMember` 사용 | 가족 관리 화면과 데이터 모델 명칭이 달라져 사용자 검토와 구현 지시가 어긋날 수 있다. |
| `Coverage` 명칭 | `PolicyCoverage`로 rename | `Coverage` 유지 후 `policyId` 종속을 명시 | 청구 보장, 담보 후보, 약관 근거의 범위가 모호해질 수 있다. |
| `Document` 구조 | 저장 모델은 단일 `Document` alias 가능, 화면/도메인 문서는 `PolicyDocument`와 `ClaimDocument`로 구분 | 물리 객체도 `PolicyDocument`, `ClaimDocument`로 분리 | 단일 `Document`만 쓰면 연결 대상 혼동이 생기고, 물리 분리는 중복 필드가 늘 수 있다. |
| `OcrExtraction` / `ReviewCandidate` | `OcrCandidate` 중심으로 alias 정리 | `OcrExtraction`과 `ReviewCandidate`를 계속 분리 | OCR 원문, 후보값, 사용자 확정값의 저장 경계가 흐려질 수 있다. |
| `ClaimReferenceResult` | 조회 결과 객체 후보로 추가 | 저장하지 않고 서비스 결과 projection으로만 사용 | 청구 판단 시점의 참고 근거를 재현하기 어렵거나, 반대로 불필요한 민감정보 저장이 늘 수 있다. |
| `HistoryItem` | projection 후보로 우선 정의 | 별도 저장 객체로 정의 | projection이면 조회 구현이 복잡하고, 저장 객체면 원본 상태와 동기화 위험이 생긴다. |
| `Category` / `CategoryItem` | 관리 데이터 후보로 추가 | 화면 전용 정적 목록으로 유지 | 이력 검색 조건과 등록 화면 선택값이 관리 데이터에서 선택되어야 한다는 V5.5 기준을 만족하기 어렵다. |
| `Tag` | `CategoryItem`과 분리 여부를 보류 | `CategoryItem` 중 `keyword_tag` 유형으로 통합 | 유사 청구 검색용 태그 규칙이 커지면 `CategoryItem`만으로 부족할 수 있다. |
| `ClaimMemo` / `HistoryMemo` | 별도 객체 후보로 보류 | 각 객체의 `memo` 필드로 유지 | 메모 작성 위치, 이력, 확인 상태가 필요해질 때 추적성이 부족할 수 있다. |

## 5. 객체별 결정 초안

### 5.1 `Person` -> `FamilyMember`

- 판단: Candidate
- 권장: V5.5 화면 기준 명칭은 `FamilyMember`로 둔다.
- 이유: 이 프로젝트는 실제 인물 식별보다 가족 보험 관리 단위가 중요하다.
- 보류: 기존 `docs/06_DATA_MODEL.md`를 수정하기 전에는 `Person`을 alias 후보로 둔다.

### 5.2 `Coverage` -> `PolicyCoverage`

- 판단: Candidate
- 권장: `PolicyCoverage`를 우선 명칭 후보로 둔다.
- 이유: 담보/특약은 특정 보험에 종속되며, 보험 검색과 청구 참고 조회에서 보험 기준으로 쓰인다.
- 보류: 담보가 청구 제출에 직접 연결되는 방식은 추가 결정이 필요하다.

### 5.3 `Document` -> `PolicyDocument` / `ClaimDocument`

- 판단: Candidate
- 권장: 화면과 문서에서는 `PolicyDocument`, `ClaimDocument`를 구분한다.
- 대안: 저장 모델은 단일 `Document`에 `documentPurpose`, `linkedPolicyId`, `linkedClaimCaseId`를 둘 수 있다.
- 보류: 물리 저장 구조는 `docs/06_DATA_MODEL.md` 수정 전에 결정한다.

### 5.4 `OcrExtraction` / `ReviewCandidate` -> `OcrCandidate`

- 판단: Candidate
- 권장: 화면 기준 명칭은 `OcrCandidate`로 통일한다.
- 이유: V5.5 화면은 OCR 엔진 결과 자체보다 후보값과 사용자 확정값 검토에 초점을 둔다.
- 보류: OCR 원문, 추출 실행 기록, 후보 필드, 사용자 확정값의 물리 분리 여부는 Needs Decision이다.

### 5.5 `ClaimReferenceResult`

- 판단: Candidate
- 권장: 보험 찾기 결과의 화면 단위 객체 후보로 둔다.
- 이유: 담보 후보, 확인 필요 담보, 과거 유사 청구 Top 3는 화면상 묶음 결과다.
- 보류: 저장 객체인지 일회성 조회 결과인지 결정이 필요하다.

### 5.6 `HistoryItem`

- 판단: Needs Decision
- 권장: 우선 projection 후보로 둔다.
- 이유: 이력 보기는 `ClaimCase`, `ClaimSubmission`, `ClaimPayment`를 통합해서 보여준다.
- 보류: 검색 성능이나 스냅샷 보존 요구가 있으면 저장 객체가 될 수 있다.

### 5.7 `Category` / `CategoryItem` / `Tag`

- 판단: Candidate / Needs Decision
- 권장: `Category`와 `CategoryItem`은 관리 데이터 후보로 둔다.
- 권장: `Tag`는 검색용 의미가 커질 때 별도 객체로 분리한다.
- 보류: 일반 태그가 단순 선택값인지 검색 랭킹/동의어/prefix 규칙을 갖는지 결정이 필요하다.

### 5.8 `ClaimMemo` / `HistoryMemo`

- 판단: Needs Decision
- 권장: 현재는 후보로만 둔다.
- 이유: 화면에는 여러 메모가 있으나, 작성 이력과 확인 상태가 필수인지 아직 확정되지 않았다.
- 보류: 단순 메모 필드로 충분한지 별도 객체가 필요한지 결정해야 한다.

## 6. 아직 확정하지 말아야 할 항목

- `docs/06_DATA_MODEL.md`의 객체명을 바로 변경하는 일
- `Document` 물리 객체를 `PolicyDocument`와 `ClaimDocument`로 즉시 분리하는 일
- `HistoryItem`을 확정 저장 객체로 단정하는 일
- `ClaimReferenceResult`를 확정 저장 객체로 단정하는 일
- `Tag`를 `CategoryItem`과 별도 객체로 확정하는 일
- `OcrCandidate`를 OCR 실행 기록, 후보값, 사용자 확정값 전체를 포함하는 단일 객체로 확정하는 일
- `ClaimCase` 완료와 `ClaimSubmission` 완료를 같은 상태로 합치는 일
- 삭제와 사용 중지를 같은 정책으로 취급하는 일

## 7. 다음 결정 필요 사항

1. `docs/06_DATA_MODEL.md`를 V5.5 명칭으로 수정할지, alias 표를 별도 유지할지 결정한다.
2. `Document`를 단일 저장 객체로 둘지, `PolicyDocument`와 `ClaimDocument`로 물리 분리할지 결정한다.
3. OCR 실행 기록과 OCR 후보값을 분리할지 결정한다.
4. 사용자 확정값의 저장 위치와 후보값 보존 기간을 결정한다.
5. `HistoryItem`을 projection으로 둘지 저장 객체로 둘지 결정한다.
6. `ClaimReferenceResult`를 청구 판단 근거로 저장할지 결정한다.
7. `CategoryItem`과 `Tag`의 경계를 결정한다.
8. 메모를 단순 필드로 둘지 `ClaimMemo` / `HistoryMemo`로 분리할지 결정한다.
9. 삭제와 사용 중지의 정책 차이를 확정한다.
10. 민감정보 마스킹 기준과 파일 경로 저장 기준을 확정한다.
