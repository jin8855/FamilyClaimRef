# Data Model V5.5 Proposed

## 1. 목적

이 문서는 기존 `docs/06_DATA_MODEL.md`를 직접 수정하기 전에, V5.5 화면 기준 데이터 모델 제안본을 별도 문서로 정리한다.

이 문서는 `docs/06_DATA_MODEL.md`의 대체 초안이며, 사용자 검토 후 실제 `06_DATA_MODEL.md` 반영 여부를 결정한다. DB 생성, OCR 구현, 앱 구현 지시가 아니다.

## 2. 적용 범위

- V5.5 정적 Low-Fi 와이어프레임의 화면 입력값, 저장 후보, 조회 후보를 반영한다.
- `docs/27_DATA_MODEL_NAMING_DECISION.md`의 명칭 기준을 따른다.
- 문서/화면 명칭과 물리 저장 구조 결정을 분리한다.
- `Confirmed`, `Candidate`, `Needs Decision`을 구분한다.
- 실제 개인정보 샘플, 실제 가족 실명, 실제 보험사명, 실제 병원명은 포함하지 않는다.

## 3. 보안 / 로컬 저장 원칙

- 민감정보는 기본적으로 사용자 PC 밖으로 내보내지 않는다.
- 원본 문서와 이미지는 Git 추적 제외 대상인 `attachments/`에 두는 후보로 관리한다.
- 로컬 추출 데이터와 임시 인덱스는 Git 추적 제외 대상인 `data/local/`에 두는 후보로 관리한다.
- 계좌번호, 카드번호, 고유식별번호, 증권번호 전체값은 저장하지 않는다.
- 보험사명, 병원명, 진단명, 진단코드 prefix, 금액, 지급 결과는 민감정보 단서로 취급한다.
- OCR 원문 전체 저장은 최소화한다.
- OCR 후보값은 사용자 확정 전에는 업무 객체의 확정 근거로 사용하지 않는다.

## 4. 핵심 객체

| 객체 | 역할 | 주요 필드 후보 | 상태 | 비고 |
|---|---|---|---|---|
| `FamilyMember` | 가족 구성원 표시명과 사용 상태 관리 | `familyMemberId`, `displayName`, `relationCandidate`, `status`, `memo` | Confirmed | 기존 `Person`의 V5.5 명칭 |
| `Policy` | 가족 구성원에 연결되는 보험 기본정보 | `policyId`, `familyMemberId`, `insurerNameCandidate`, `productNameCandidate`, `policyNumberMaskedMemo`, `contractStatus`, `startDateCandidate`, `endDateCandidate`, `status`, `memo` | Confirmed | 실제 증권번호 전체값 저장 금지 |
| `PolicyCoverage` | 보험의 담보/특약과 약관 근거 | `policyCoverageId`, `policyId`, `coverageNameCandidate`, `visitTypeRules`, `expenseTypeRules`, `diagnosisCodePrefixRules`, `sourcePolicyDocumentId`, `reviewStatus`, `memo` | Confirmed naming | 기존 `Coverage` legacy alias |
| `PolicyDocument` | 특정 보험에 연결되는 보험 문서 | `policyDocumentId`, `policyId`, `documentType`, `filePathCandidate`, `ocrStatus`, `userReviewStatus`, `createdAt` | Confirmed naming | 물리 저장 구조는 Needs Decision |
| `ClaimCase` | 하나의 진료/청구 준비 단위 | `claimCaseId`, `familyMemberId`, `treatmentDateCandidate`, `visitType`, `diagnosisNameCandidate`, `diagnosisCodePrefixCandidate`, `hospitalNameCandidate`, `tagIds`, `amountCandidates`, `caseStatus`, `memo` | Confirmed | 청구 시작 화면 중심 객체 |
| `ClaimDocument` | 특정 청구 사건에 연결되는 청구 서류 | `claimDocumentId`, `claimCaseId`, `documentType`, `filePathCandidate`, `ocrStatus`, `userReviewStatus`, `createdAt` | Confirmed naming | 물리 저장 구조는 Needs Decision |
| `OcrCandidate` | 문서에서 추출된 후보값 | `ocrCandidateId`, `sourceDocumentType`, `sourceDocumentId`, `candidateType`, `candidateFields`, `reviewStatus`, `confirmedTargetType`, `createdAt` | Candidate | OCR 실행 기록과 분리 여부 Needs Decision |
| `ClaimReferenceResult` | 보험 찾기 화면의 담보 후보와 과거 유사 청구 결과 | `claimReferenceResultId`, `claimCaseId`, `matchedCoverageCandidates`, `needsReviewCoverageCandidates`, `similarHistoryTop3`, `generatedAt` | Candidate | 저장 여부 Needs Decision |
| `ClaimSubmission` | 보험사별 청구 진행 기록 | `claimSubmissionId`, `claimCaseId`, `policyId`, `policyCoverageId`, `submittedDateCandidate`, `submittedAmountCandidate`, `submissionStatus`, `submittedDocumentIds`, `memo` | Confirmed | 지급 결과와 분리 |
| `ClaimPayment` | 지급/부지급/감액 결과 | `claimPaymentId`, `claimSubmissionId`, `paymentStatus`, `paidDateCandidate`, `paidAmountCandidate`, `paidCoverageNameCandidate`, `denyReasonCandidate`, `reductionReasonCandidate`, `memo` | Confirmed | `ClaimCase` 직접 연결 여부 Needs Decision |
| `HistoryItem` | 이력 보기 통합 목록 항목 | `historyItemIdCandidate`, `sourceType`, `sourceId`, `summaryFields`, `status`, `displayDate` | Candidate | 우선 projection 후보 |
| `Category` | 관리 데이터 상위 분류 | `categoryId`, `categoryName`, `categoryCodeCandidate`, `status`, `sortOrder`, `description` | Confirmed | 진료상황, 문서유형 등 |
| `CategoryItem` | 분류에 속한 선택 항목 | `categoryItemId`, `categoryId`, `itemName`, `itemCodeCandidate`, `status`, `sortOrder`, `searchEnabled`, `description` | Confirmed | 선택값 관리 |
| `Tag` | 유사 청구와 보험 검색용 태그 후보 | `tagId`, `tagName`, `tagType`, `status`, `synonymCandidates`, `searchWeightCandidate` | Candidate | `CategoryItem`과 분리 여부 Needs Decision |

## 5. 객체 관계

```text
FamilyMember 1 - N Policy
FamilyMember 1 - N ClaimCase
Policy 1 - N PolicyCoverage
Policy 1 - N PolicyDocument
ClaimCase 1 - N ClaimDocument
ClaimCase 1 - N ClaimSubmission
ClaimSubmission 1 - N ClaimPayment
PolicyDocument 1 - N OcrCandidate
ClaimDocument 1 - N OcrCandidate
Category 1 - N CategoryItem
```

추가 후보 관계:

```text
ClaimCase N - N Tag
PolicyCoverage N - N CategoryItem
ClaimReferenceResult N - N PolicyCoverage
HistoryItem -> ClaimCase / ClaimSubmission / ClaimPayment projection
```

## 6. 문서 객체 구조

문서/화면 기준 명칭:

- `PolicyDocument`
- `ClaimDocument`

물리 저장 후보:

- 단일 `Document` + `documentPurpose` + `linkedPolicyId` + `linkedClaimCaseId`
- 또는 `PolicyDocument` / `ClaimDocument` 물리 분리

판단:

- 문서/화면 명칭은 분리 확정이다.
- 물리 저장 구조는 Needs Decision이다.
- `PolicyDocument`는 `Policy`에 연결한다.
- `ClaimDocument`는 `ClaimCase`에 연결한다.
- `OcrCandidate`는 문서에서 추출된 후보값이며, 사용자 확정값만 업무 객체에 반영한다.

## 7. OCR 후보값과 사용자 확정값

- `OcrCandidate`는 후보값이다.
- OCR 후보값은 업무 객체에 자동 반영하지 않는다.
- 사용자 확정값만 `PolicyDocument`, `ClaimDocument`, `PolicyCoverage`, `ClaimCase` 같은 업무 객체에 반영한다.
- OCR 원문 전체 저장은 민감정보 위험이 있으므로 최소화한다.
- OCR 실행 기록을 별도 `OcrExtraction`으로 둘지 여부는 Needs Decision이다.
- 기존 `ReviewCandidate`는 `OcrCandidate.reviewStatus` alias 후보로 둔다.

후보 상태:

```text
needs_user_review
edited
confirmed
ignored
```

사용자 확정값 반영 후보:

| 문서 목적 | 확정값 반영 대상 |
|---|---|
| 보험 관리용 문서 | `PolicyDocument`, `PolicyCoverage` |
| 청구용 문서 | `ClaimDocument`, `ClaimCase` |

## 8. 청구 흐름 객체 구조

청구 흐름은 5단계다.

```text
1. 청구 시작(서류/이미지 추가)
2. OCR 확인
3. 보험 찾기
4. 진행 현황
5. 청구 완료
```

객체 흐름:

```text
ClaimCase
-> ClaimDocument
-> OcrCandidate
-> ClaimReferenceResult Candidate
-> ClaimSubmission
-> ClaimPayment
-> HistoryItem projection
```

구분 원칙:

- `ClaimCase`는 하나의 진료/청구 준비 단위다.
- `ClaimSubmission`은 보험사별 청구 진행 기록이다.
- `ClaimPayment`는 `ClaimSubmission`의 지급 결과다.
- `ClaimCase` 완료와 `ClaimSubmission` 완료는 같은 상태로 합치지 않는다.
- `14_claim_complete.html`의 청구 완료는 청구 흐름 완료 확인에 가깝다.
- 실제 지급 완료는 `ClaimSubmission` / `ClaimPayment`에서 확인한다.

## 9. 이력 조회 구조

- `HistoryItem`은 우선 projection 후보로 둔다.
- 원본 객체는 `ClaimCase`, `ClaimSubmission`, `ClaimPayment`다.
- 이력 조회 성능 또는 스냅샷 보존이 필요하면 저장 객체로 전환 가능하다고 기록한다.
- `HistoryItem`을 저장 객체로 확정하지 않는다.
- 이력 상세의 사용자 메모는 `HistoryMemo` 후보로 둔다.

조회 기준 후보:

- 가족
- 보험사 후보
- 진료상황
- 기간
- 키워드/태그
- 진단명 후보
- 진단코드 prefix 후보

## 10. 분류 / 태그 구조

`Category`와 `CategoryItem`은 관리 데이터로 둔다.

예시 분류:

- 진료상황
- 지급상태
- 문서유형
- 키워드/태그
- 담보태그
- 비용구분

`Tag`는 검색용 태그 후보로 둔다.

- 단순 선택값이면 `CategoryItem`으로 충분할 수 있다.
- 유사 청구 검색, 동의어, 검색 가중치, 진단코드 prefix 규칙과 결합하면 별도 `Tag`가 필요할 수 있다.
- `Tag` 분리 여부는 Needs Decision이다.

## 11. 상태값 후보

| 대상 | 상태값 후보 | 비고 |
|---|---|---|
| `FamilyMember` | `active`, `disabled`, `delete_requested` | 삭제 제한 필요 |
| `Policy` | `draft`, `active`, `on_hold`, `disabled`, `delete_requested`, `needs_review` | 보험 등록/편집 기준 |
| `PolicyCoverage` | `candidate`, `needs_review`, `user_confirmed`, `ignored` | OCR/약관 확인 연계 |
| `PolicyDocument`, `ClaimDocument` | `registered`, `ocr_needed`, `ocr_completed`, `user_confirmed`, `ignored` | 문서 목적 분리 필요 |
| `OcrCandidate` | `needs_user_review`, `edited`, `confirmed`, `ignored` | 사용자 확정 전 후보값 |
| `ClaimCase` | `draft`, `saved`, `needs_ocr`, `reference_checked`, `case_completed`, `cancelled` | 청구 사건 상태 |
| `ClaimReferenceResult` | `generated`, `selected`, `ignored`, `expired` | 저장 여부 Needs Decision |
| `ClaimSubmission` | `preparing`, `submitted`, `additional_documents_requested`, `reviewing`, `paid`, `denied`, `cancelled`, `submission_completed` | 보험사별 진행 상태 |
| `ClaimPayment` | `pending`, `paid`, `partially_paid`, `denied`, `cancelled` | 지급 결과 상태 |
| `Category`, `CategoryItem`, `Tag` | `active`, `disabled`, `delete_requested` | 사용 중지와 삭제 정책 필요 |

## 12. 삭제 / 사용 중지 정책 후보

- 사용 중지는 이후 선택 목록과 검색 조건에서 숨기거나 비활성으로 표시하는 후보 상태다.
- 삭제는 연결 데이터가 없는 경우에만 허용하는 후보 정책이다.
- 연결된 `Policy`, `PolicyDocument`, `ClaimCase`, `ClaimSubmission`, `ClaimPayment`, `HistoryItem`이 있으면 물리 삭제를 제한한다.
- 삭제와 사용 중지는 같은 정책으로 취급하지 않는다.
- 실제 구현 전 확인 메시지와 복구 가능 여부를 별도로 결정해야 한다.

## 13. 민감정보 마스킹 기준

저장 금지 후보:

- 실제 가족 실명
- 고유식별번호
- 계좌번호
- 카드번호
- 증권번호 전체값
- 상세 주소

마스킹 또는 최소 저장 후보:

- 보험사명 후보
- 병원명 후보
- 진단명 후보
- 진단코드 prefix 후보
- 청구 금액
- 지급 결과
- 파일 경로

표시 기준:

- 샘플은 `가족 A`, `보험사 A`, `병원 후보`처럼 익명화한다.
- OCR 원문 전체 저장은 피한다.
- 태그 조합도 민감정보 단서로 취급한다.

## 14. 기존 docs/06_DATA_MODEL.md와의 차이

| 기존 `docs/06_DATA_MODEL.md` | V5.5 Proposed | 차이 |
|---|---|---|
| `Person` | `FamilyMember` | 가족 관리 화면 기준 명칭으로 변경 제안 |
| `Coverage` | `PolicyCoverage` | 보험 종속 담보/특약 의미 명확화 |
| `Document` | `PolicyDocument`, `ClaimDocument` | 문서 목적과 연결 대상 기준으로 문서 명칭 분리 |
| `OcrExtraction` | `OcrCandidate` Candidate | 화면은 후보값 검토 중심. OCR 실행 기록 분리 여부는 미정 |
| `ReviewCandidate` | `OcrCandidate.reviewStatus` Candidate | 후보 검토 상태로 흡수 가능 |
| 없음 | `ClaimReferenceResult` Candidate | 보험 찾기 결과 묶음 후보 추가 |
| 없음 | `HistoryItem` Candidate | 이력 보기 projection 후보 추가 |
| 없음 | `Category`, `CategoryItem` | 관리 데이터 구조 추가 |
| 없음 | `Tag` Candidate | 검색용 태그 후보 추가 |
| 없음 | `ClaimMemo`, `HistoryMemo` Candidate | 메모 분리 후보 추가 |

## 15. Needs Decision

| 항목 | 결정 필요 내용 |
|---|---|
| 문서 물리 저장 구조 | 단일 `Document`인지 `PolicyDocument` / `ClaimDocument` 물리 분리인지 결정 |
| OCR 실행 기록 | `OcrExtraction`을 별도 유지할지, `OcrCandidate`에 포함할지 결정 |
| 사용자 확정값 저장 경계 | 후보값과 확정값을 어디까지 함께 보존할지 결정 |
| `ClaimReferenceResult` 저장 여부 | 조회 결과 캐시인지 청구 판단 근거 스냅샷인지 결정 |
| `HistoryItem` 저장 여부 | projection인지 저장 객체인지 결정 |
| `Tag` 분리 여부 | `CategoryItem`으로 충분한지 별도 검색 태그가 필요한지 결정 |
| 메모 구조 | 단순 `memo` 필드인지 `ClaimMemo` / `HistoryMemo` 별도 객체인지 결정 |
| 삭제와 사용 중지 | 물리 삭제 제한, 비활성 표시, 복구 가능 여부 결정 |
| 파일 경로 저장 | 파일 경로 마스킹, 원본 파일명 보존 여부, 메타데이터 저장 위치 결정 |
| 민감정보 마스킹 | 보험사명, 병원명, 진단명, 금액, 지급 결과의 표시/저장 기준 결정 |
