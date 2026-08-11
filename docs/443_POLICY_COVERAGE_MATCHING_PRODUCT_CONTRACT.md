# PolicyCoverage Matching Product Contract

## 1. Document Status

- Status: `AUTHORITATIVE_PRODUCT_CONTRACT`
- Marker: `POLICY_COVERAGE_MATCHING_PRODUCT_CONTRACT_READY`
- Risk tier: `T2_MODERATE`
- Baseline: `f632cdd0c84795439ce1494729d24ba1346802ac`
- Scope: 화면 03 보험 검색, 화면 09 보험 찾기, `PolicyCoverage` 논리 모델, read-only 매칭 및 결과 계약
- Implementation authorization: 문서 계약만 승인
- `PolicyCoverage` persistence: `NOT_IMPLEMENTED`, `NOT_AUTHORIZED_IN_THIS_BATCH`
- Production readiness: `NOT_EVALUATED`
- Deployment: `NOT_AUTHORIZED`

이 문서는 이 문서의 주제에 한해 기존 draft 또는 candidate 문서보다 우선한다. 앱, 테스트, JSON, schema, migration, runtime, 실제 데이터는 변경하지 않는다.

## 2. Objectives and Scope

### OBJ-PCS-001

화면 03 `ProductScreenRoutes.PolicyList`와 화면 09 `ProductScreenRoutes.ClaimReferenceResult`의 역할을 분리한다.

### OBJ-PCS-002

특정 `Policy`에 종속되는 `PolicyCoverage` 논리 모델과 사용자 확인 생명주기를 확정한다.

### OBJ-PCS-003

저장된 `ClaimCase`와 확인된 `PolicyCoverage`를 사용하는 read-only 매칭, 과거 유사 청구, 안전한 결과 표현 및 저장 경계를 확정한다.

### Non-scope

- `PolicyCoverage` production class, interface, storage 또는 JSON 구현
- 기존 JSON, schema, migration 또는 데이터 변경
- 화면 03 또는 화면 09 UI 구현
- OCR, 문서 분석, 외부 보험 API, 외부 AI 또는 원격 전송
- 보험금 지급 가능성, 보장 확정 또는 예상 보험금 추론
- production readiness 평가 또는 deployment

## 3. Authority and Current Code Evidence

### 3.1 Current route evidence

| Evidence | Current fact | Contract use |
|---|---|---|
| `app/FamilyClaimRef.App/ViewModels/ProductScreenRoutes.cs` | 화면 03은 `ProductScreenRoutes.PolicyList` (`03_policy_list`) | 등록된 Policy 검색 owner |
| `app/FamilyClaimRef.App/ViewModels/ProductScreenRoutes.cs` | 화면 09는 `ProductScreenRoutes.ClaimReferenceResult` (`09_claim_reference_result`) | ClaimCase 기반 보험 찾기 owner |
| `app/FamilyClaimRef.App/ViewModels/ProductScreenCatalog.cs` | 화면 03과 화면 09는 서로 다른 route, command, claim-step 위치를 가짐 | 역할 혼용 금지 |
| `app/FamilyClaimRef.App/Models/Storage/ClaimSubmissionRecord.cs` | `PolicyCoverageId`는 nullable | 향후 선택 reference 호환 필드 |
| `app/FamilyClaimRef.App/Models/Storage/ClaimSubmissionRecord.cs` | `CoverageDisplayName`은 nullable | 저장 시점 표시 snapshot 호환 필드 |

현재 `PolicyCoverage` production model 또는 storage는 존재하지 않는다. 이 문서는 해당 구현을 생성하지 않는다.

### 3.2 Source documents reconciled

| Source document | Reconciled subject | Result |
|---|---|---|
| `docs/00_PROJECT_BASE_GUIDE.md` | 개인정보, 로컬 처리, 확정적 지급 표현 금지 | Preserved |
| `docs/01_PRD.md` | 보험 찾기와 참고 후보 제품 목표 | Refined by this contract |
| `docs/03_USER_FLOW.md` | 화면 흐름과 청구 단계 | Current route 기준으로 reconciled |
| `docs/04_SCREEN_LIST.md` | 화면 03과 화면 09 목적 | Current route 기준으로 reconciled |
| `docs/06_DATA_MODEL.md` | `PolicyCoverage`, `ClaimReferenceResult`, 기존 후보 표현 | Logical contract refined |
| `docs/23_SCREEN_TO_DATA_MODEL_MAPPING.md` | 화면-객체 mapping과 candidate 상태 | Role and persistence boundary refined |
| `docs/28_DATA_MODEL_V5_5_PROPOSED.md` | proposed fields와 관계 | Source only, not promoted wholesale |
| `docs/31_DATA_MODEL_TERMINOLOGY_CONSISTENCY_REVIEW.md` | `PolicyCoverage` naming과 legacy alias | Preserved |
| `docs/32_DATA_MODEL_GAP_REVIEW_STALENESS_REVIEW.md` | unresolved persistence details | Deferred to WBS-PCS-02 |
| `docs/33_UI_STATE_AND_CONFIRMATION_MESSAGE_GUIDE.md` | 참고 후보와 안전 문구 | Refined by DEC-PCS-010 |
| `docs/35_PRE_IMPLEMENTATION_DECISION_MATRIX.md` | snapshot과 OCR 경계 | Replaced for this subject by DEC-PCS-013 |

Source reconciliation result: `11/11`.

## 4. Requirements

### REQ-PCS-001

화면 03은 등록된 `Policy`의 목록 검색과 필터만 제공해야 한다.

### REQ-PCS-002

화면 09는 저장된 `ClaimCase`를 기준으로 확인된 담보 참고 후보를 계산해야 한다.

### REQ-PCS-003

매칭에는 `ReviewStatus = user_confirmed`이고 사용 중인 `PolicyCoverage`만 사용해야 한다.

### REQ-PCS-004

매칭은 `TreatmentDate`, `VisitType`, `HasSurgery`, `HasPrescription`, `DiagnosisCode`의 확정 필드만 사용해야 한다.

### REQ-PCS-005

결과는 보험금 지급 판단이 아닌 참고 후보임을 명확히 표시해야 한다.

### REQ-PCS-006

과거 유사 청구는 현재 담보 분류와 분리된 read-only 보조 정보여야 한다.

### REQ-PCS-007

검색과 후보 선택 자체는 어떤 durable record도 생성하거나 수정하지 않아야 한다.

### REQ-PCS-008

사용자가 화면 08 저장 동작을 실행할 때만 기존 `ClaimSubmission` 계약으로 선택 reference를 저장해야 한다.

### REQ-PCS-009

전체 graph를 먼저 검증하고 검증 성공 후에만 ClaimCase scope와 projection을 적용해야 한다.

### REQ-PCS-010

load 또는 reload 실패 시 이전 결과, 선택, 과거 유사 청구를 모두 제거해야 한다.

### REQ-PCS-011

raw ID, 경로, JSON, 예외 및 민감정보를 UI와 Automation 속성에 노출하지 않아야 한다.

### REQ-PCS-012

후속 구현은 WBS-PCS-01부터 WBS-PCS-05까지의 순서와 위험 경계를 따라야 한다.

## 5. Product Decisions

### DEC-PCS-001 - 화면 03과 화면 09 역할 분리

화면 03 보험 검색은 등록된 `Policy`를 찾는 화면이다.

- 허용 필터: 가족, 보험사, 계약 상태, 상품 구분, 키워드
- 사용하지 않는 조건: 진단명, 진단코드, 수술, 약제비, 진료 조건
- 담보 일치도 계산: 없음
- 청구 또는 지급 가능성 표시: 없음

화면 09 보험 찾기는 저장된 `ClaimCase`에서 확인된 담보 참고 후보를 계산한다.

- `user_confirmed` `PolicyCoverage`만 매칭에 사용
- 과거 유사 청구는 보조 근거로 별도 표시
- 결과는 참고 후보이며 보험금 지급 판단이 아님

두 화면의 ViewModel, 문구 및 Automation 속성에서 보험 검색과 보험 찾기를 혼용하지 않는다.

### DEC-PCS-002 - PolicyCoverage 논리 소유권

관계는 `Policy 1 : N PolicyCoverage`이다. 가족 소유권을 복제하지 않고 부모 `Policy.FamilyMemberId`를 따른다.

#### DATA-PCS-001 - PolicyCoverage logical fields

| Field | Contract |
|---|---|
| `PolicyCoverageId` | 내부 immutable logical ID |
| `PolicyId` | 존재하는 부모 `Policy` reference |
| `DisplayName` | 공백이 아닌 표시명 |
| `ReviewStatus` | DATA-PCS-002의 상태 |
| `EffectiveFrom` | optional effective lower bound |
| `EffectiveTo` | optional effective upper bound |
| `VisitTypeRule` | DATA-PCS-003의 값 |
| `SurgeryRule` | DATA-PCS-004의 값 |
| `PrescriptionRule` | DATA-PCS-004의 값 |
| `DiagnosisRuleMode` | DATA-PCS-005의 값 |
| `DiagnosisCodePrefixes` | 정규화된 prefix collection |
| `SourceKind` | DATA-PCS-006의 값 |
| `SourcePolicyDocumentId` | optional same-Policy document reference |
| `SourceLocator` | optional non-public locator metadata |
| `Memo` | optional note, 자동 매칭 입력이 아님 |
| `Revision` | optimistic concurrency logical field |
| `CreatedAt` | creation timestamp |
| `UpdatedAt` | last update timestamp |
| `DisabledAt` | optional disabled timestamp |

JSON 파일명, envelope, atomic write, backup, migration 및 물리 저장 형식은 WBS-PCS-02에서 별도 승인한다.

### DEC-PCS-003 - 상태와 전이

#### DATA-PCS-002 - ReviewStatus

| State | Search use |
|---|---|
| `candidate` | Excluded |
| `needs_review` | Excluded |
| `user_confirmed` | Included only when `DisabledAt = null` |
| `ignored` | Excluded |

허용 전이:

- `candidate -> needs_review | user_confirmed | ignored`
- `needs_review -> user_confirmed | ignored`
- `user_confirmed -> needs_review | ignored`
- `ignored -> needs_review`

사용자가 규칙 또는 출처를 실질적으로 변경하면 `user_confirmed`를 유지하지 않고 `needs_review`로 되돌린다. 미확인 담보는 개별 검색 결과로 표시하지 않고 다음 안내만 허용한다.

> 확인이 끝나지 않은 담보는 검색 결과에서 제외했습니다.

unknown 상태는 조용히 제외하지 않고 전체 검색을 fail-closed 처리한다.

### DEC-PCS-004 - 규칙 값과 기본 검증

#### DATA-PCS-003 - VisitTypeRule

`any`, `outpatient`, `inpatient`

#### DATA-PCS-004 - Boolean condition rules

`any`, `required`, `excluded`

#### DATA-PCS-005 - DiagnosisRuleMode

`any`, `prefix_list`

#### DATA-PCS-006 - SourceKind

`manual`, `policy_document`

#### RULE-PCS-001 - 기본 무결성

- `DisplayName`은 trim 후 공백이 아닌 값이어야 한다.
- `PolicyId`는 존재하는 `Policy`를 참조해야 한다.
- `EffectiveFrom`과 `EffectiveTo`가 모두 있으면 `EffectiveFrom <= EffectiveTo`여야 한다.
- `DiagnosisRuleMode = prefix_list`이면 prefix가 1개 이상이어야 한다.
- prefix는 trim 후 대문자로 정규화한다.
- 점, 하이픈 및 기타 진단코드 문자는 임의 제거하지 않는다.
- `SourceKind = policy_document`이면 같은 `Policy`의 문서 reference가 필수다.
- `SourceKind = manual`이면 `SourcePolicyDocumentId = null`을 허용한다.
- unknown enum, 중복 ID 또는 orphan reference는 fail-closed 처리한다.

비용 한도, 공제금액, 횟수 제한, 면책기간, 진단명 자유문자, 보험료, 예상 지급액 및 자유문자 `CoveragePeriod`는 MVP 자동 매칭 조건이 아니다.

### DEC-PCS-005 - 검색 입력

#### DATA-PCS-007 - ClaimCase search input

검색 가능한 `ClaimCase`:

- `CaseStatus = saved`
- `DisabledAt = null`
- 존재하고 사용 중인 `FamilyMember` 소유
- `TreatmentDate` 존재
- `VisitType`이 허용 값

사용하는 확정 필드:

- `FamilyMemberId`
- `TreatmentDate`
- `DiagnosisCode`
- `VisitType`
- `HasSurgery`
- `HasPrescription`

자동 매칭에 사용하지 않는 값:

- `DiagnosisName` 자유문자
- `HospitalName`
- `Memo`
- 원본 문서명
- OCR 미확정 후보
- 금액만으로 계산한 지급 가능성

화면 09는 `ClaimCase`를 수정하거나 저장하지 않는다. 수정이 필요하면 화면 07로 이동한다.

### DEC-PCS-006 - 부모 Policy 포함 조건

#### RULE-PCS-002 - 신규 후보 Policy

포함 조건:

- `Policy.DisabledAt = null`
- `Policy.FamilyMemberId = ClaimCase.FamilyMemberId`
- `ContractStatus`가 `유지`, `보험료 납입면제`, `legacy 사용 중` 중 하나

제외 조건:

- `만기`
- disabled
- legacy owner
- 다른 `FamilyMember` 소유
- unknown `ContractStatus`

만기 또는 disabled `Policy`는 과거 유사 청구 사실에는 사용 중지 또는 만기 표시와 함께 나타날 수 있지만 신규 담보 후보에는 사용할 수 없다.

### DEC-PCS-007 - 날짜 판정

#### RULE-PCS-003 - 날짜 순서

1. `TreatmentDate < Policy.EnrollmentDate`이면 불일치다.
2. `EffectiveFrom`이 있으면 `TreatmentDate >= EffectiveFrom`이어야 한다.
3. `EffectiveTo`가 있으면 `TreatmentDate <= EffectiveTo`여야 한다.
4. `EffectiveFrom`이 없으면 `Policy.EnrollmentDate`를 하한으로 사용한다.
5. `EffectiveTo`가 없고 `Policy`가 현재 사용 가능한 상태이면 상한이 열린 것으로 처리한다.
6. 자유문자 `CoveragePeriod`는 자동 파싱하지 않는다.

날짜가 모순되거나 허용 형식이 아니면 특정 담보만 추정 분류하지 않고 전체 검색을 fail-closed 처리한다.

### DEC-PCS-008 - 진단코드 판정

#### RULE-PCS-004 - Diagnosis matching

- `DiagnosisRuleMode = any`: 진단코드와 관계없이 통과
- `DiagnosisRuleMode = prefix_list`이고 `ClaimCase.DiagnosisCode`가 없음: 추가 확인 필요
- 정규화된 진단코드가 prefix 중 하나로 시작함: 통과
- 어느 prefix에도 일치하지 않음: 불일치

비교는 trim, 대문자 변환, `Ordinal` 비교를 사용하며 점과 하이픈을 보존한다. 문화권 비교, 진단명 자유문자 검색, 유사어, 오타 보정 및 생성형 AI 의미 추론은 금지한다.

### DEC-PCS-009 - 진료, 수술 및 약제비 판정

#### RULE-PCS-005 - VisitType

`any`는 항상 통과한다. `outpatient`와 `inpatient`는 명시적으로 같은 값일 때만 통과한다.

#### RULE-PCS-006 - Surgery

- `any`: 항상 통과
- `required`: `HasSurgery = true`일 때 통과
- `excluded`: `HasSurgery = false`일 때 통과

#### RULE-PCS-007 - Prescription

- `any`: 항상 통과
- `required`: `HasPrescription = true`일 때 통과
- `excluded`: `HasPrescription = false`일 때 통과

금액 존재만으로 `HasSurgery` 또는 `HasPrescription`을 자동 변경하지 않는다.

### DEC-PCS-010 - 결과 분류와 안전 문구

#### DATA-PCS-008 - Result groups

1. `조건 일치 후보`
2. `추가 확인 필요`
3. `현재 입력과 불일치`

#### RULE-PCS-008 - Classification priority

- 명시적 불일치가 1개 이상이면 `현재 입력과 불일치`
- 명시적 불일치가 없고 확인 불가 조건이 1개 이상이면 `추가 확인 필요`
- 모든 적용 조건을 통과하면 `조건 일치 후보`

필수 안내:

> 등록한 조건과 일치하는 참고 후보입니다. 실제 보장 및 지급 여부는 약관과 보험사 심사를 확인해 주세요.

#### Prohibited result wording

다음 표현은 제품 결과, 명령, 상태, Automation 속성 또는 사용자 안내에 사용하지 않는다.

- `청구 가능`
- `보험금 수령 가능`
- `지급 가능`
- `보장 확정`
- `보장 불가`
- `청구 추천`
- `예상 보험금`
- `받을 수 있음`

### DEC-PCS-011 - 결과 설명과 정렬

#### FUNC-PCS-001 - Result evidence

각 결과는 내부 점수 대신 다음 근거를 표시한다.

- 보험 계약 상태
- 진료일 범위
- 진료 구분
- 수술 조건
- 약제비 조건
- 진단코드 규칙
- 추가 확인 사유
- 출처 문서 유무

#### RULE-PCS-009 - Result ordering

정렬 순서:

1. `조건 일치 후보`
2. `추가 확인 필요`
3. `현재 입력과 불일치`
4. 구체적으로 통과한 규칙 수 내림차순
5. `Policy` 표시명 오름차순
6. `PolicyCoverage` 표시명 오름차순
7. 내부 ID `Ordinal` 오름차순

내부 ID는 deterministic tie-break에만 사용하고 화면 또는 Automation 속성에 노출하지 않는다.

### DEC-PCS-012 - 과거 유사 청구

#### FUNC-PCS-002 - Similar claim projection

과거 유사 청구는 담보 매칭 결과와 별도 영역에 최대 3건 표시한다.

포함 조건:

- 동일 `FamilyMember`
- saved `ClaimCase`
- `submission_completed` `ClaimSubmission`
- terminal `ClaimPayment` 존재
- terminal 상태가 `paid`, `partially_paid`, `denied` 중 하나

`pending`, `cancelled`, orphan 또는 ownership mismatch는 제외한다.

#### RULE-PCS-010 - Similarity tiers

- Tier A: 동일한 non-null `PolicyCoverageId`
- Tier B: 정규화된 `DiagnosisCode` exact 일치 및 `VisitType` 일치
- Tier C: 한 `DiagnosisCode`가 다른 코드의 prefix이고 `VisitType` 일치

#### RULE-PCS-011 - Similarity ordering

Tier A, B, C 순서 후 `TreatmentDate` 내림차순, `ClaimSubmission.UpdatedAt` 내림차순, 내부 Submission ID `Ordinal` 오름차순으로 정렬한다.

과거 지급 또는 부지급 결과는 사실로만 표시하고 현재 담보 후보 분류를 올리거나 내리지 않는다.

필수 안내:

> 과거 결과는 현재 청구의 보장 또는 지급을 보장하지 않습니다.

### DEC-PCS-013 - 선택과 저장 경계

#### FUNC-PCS-003 - Read-only result

화면 09 결과는 read-only projection이다.

- `ClaimReferenceResult` 파일 생성: 금지
- 검색 조건 자동 저장: 금지
- 검색 결과 전체 snapshot 저장: 금지
- 검색 로그 저장: 금지

#### FUNC-PCS-004 - Draft navigation

사용자가 후보를 청구 준비에 사용하면 화면 08 `ClaimSubmission` draft로 이동하며 다음 값만 preselect 또는 복사한다.

- `PolicyId`
- `PolicyCoverageId`
- `CoverageDisplayName` snapshot

이 이동과 선택만으로는 저장하지 않는다. 사용자가 화면 08 저장 동작을 실행할 때만 기존 `ClaimSubmission` 계약을 따라 저장한다.

| Result group | 화면 08 이동 |
|---|---|
| `조건 일치 후보` | 사용 가능 |
| `추가 확인 필요` | 약관 확인 안내 후 사용 가능 |
| `현재 입력과 불일치` | 비활성 |

### DEC-PCS-014 - 전체 graph 검증과 fail-closed

#### FUNC-PCS-005 - Read order

1. `FamilyMember` 전체 조회
2. `Policy` 전체 조회
3. `PolicyCoverage` 전체 조회
4. `ClaimCase` 전체 조회
5. `ClaimSubmission` 전체 조회
6. `ClaimPayment` 전체 조회
7. `PolicyDocument` reference 조회
8. 전체 reference, ownership, 상태 검증
9. 검증 성공 후 ClaimCase scope 적용
10. 담보 매칭과 유사 청구 projection 생성

#### ERR-PCS-001 - Reference failures

orphan `PolicyCoverage`, orphan `PolicyDocument`, source document의 `Policy` 불일치 및 Submission 또는 Payment orphan은 전체 결과를 제거한다.

#### ERR-PCS-002 - Ownership failures

legacy owner, `FamilyMember` ownership mismatch 및 신뢰할 수 없는 전체 graph는 전체 결과를 제거한다.

#### ERR-PCS-003 - Value failures

중복 ID, unknown 상태, unknown rule, 날짜 역전 또는 허용 형식 위반은 전체 결과를 제거한다.

#### ERR-PCS-004 - Load failures

load exception 또는 reload 실패 시 다음 상태로 초기화한다.

- 후보 목록 empty
- 과거 유사 청구 empty
- 선택 상태 null
- `HasLoadedProjection = false`
- 안전한 오류 메시지

이전 정상 결과를 남기지 않는다.

### DEC-PCS-015 - 개인정보와 로컬 경계

#### ERR-PCS-005 - UI privacy boundary

다음을 화면 오류, 로그성 UI 및 Automation 속성에 노출하지 않는다.

- raw ID
- 실제 파일 경로
- JSON 원문
- 예외 메시지 또는 예외 타입
- stack trace
- 진단코드와 결합한 Automation 값
- 가족, 보험 또는 병원명과 결합한 Automation 값

#### ERR-PCS-006 - Test data boundary

실제 개인정보, 보험 또는 의료 데이터를 fixture와 문서 예시에 사용하지 않는다.

#### ERR-PCS-007 - External boundary

외부 API, 클라우드 OCR, 생성형 AI 분석 및 원격 전송은 금지한다.

## 6. Functional Ownership

### FUNC-PCS-006 - 화면 03 owner

화면 03의 후속 ViewModel은 등록된 `Policy` read-only 검색과 필터만 소유한다.

### FUNC-PCS-007 - 화면 09 owner

화면 09의 후속 ViewModel은 선택된 saved `ClaimCase`, 매칭 결과, 유사 청구 및 화면 08 draft 이동 상태를 소유한다.

### FUNC-PCS-008 - Pure matching owner

WBS-PCS-03의 non-UI engine은 정규화, 조건 판정, 결과 분류 및 deterministic ordering을 소유하며 저장을 소유하지 않는다.

### FUNC-PCS-009 - Persistence owner

WBS-PCS-02의 storage는 사용자 확인된 `PolicyCoverage` 생명주기와 reference integrity만 소유한다. 이 문서는 물리 구현을 승인하지 않는다.

### FUNC-PCS-010 - ClaimSubmission save owner

선택한 coverage reference의 durable save owner는 화면 08의 기존 `ClaimSubmission` 저장 동작이다.

## 7. Acceptance Scenarios

| ID | Given | When | Then |
|---|---|---|---|
| TEST-PCS-001 | 등록된 Policy 목록 | 화면 03에서 검색 | 등록 Policy 필터만 수행하고 진료 조건을 사용하지 않는다 |
| TEST-PCS-002 | draft 또는 disabled ClaimCase | 화면 09 검색 요청 | 검색하지 않고 안전한 상태를 표시한다 |
| TEST-PCS-003 | saved, active ClaimCase | 화면 09 검색 요청 | 전체 graph 검증 후 검색한다 |
| TEST-PCS-004 | 다른 가족 소유 PolicyCoverage | 후보 계산 | 제외한다 |
| TEST-PCS-005 | `candidate` coverage | 후보 계산 | 제외하고 일반 미확인 안내만 허용한다 |
| TEST-PCS-006 | `needs_review` coverage | 후보 계산 | 제외한다 |
| TEST-PCS-007 | `ignored` coverage | 후보 계산 | 제외한다 |
| TEST-PCS-008 | active `user_confirmed` coverage | 후보 계산 | 포함한다 |
| TEST-PCS-009 | disabled coverage | 후보 계산 | 제외한다 |
| TEST-PCS-010 | 만기 Policy | 신규 후보 계산 | 제외한다 |
| TEST-PCS-011 | TreatmentDate가 Policy 가입일 이전 | 날짜 판정 | `현재 입력과 불일치`로 분류한다 |
| TEST-PCS-012 | TreatmentDate가 coverage 유효기간 밖 | 날짜 판정 | `현재 입력과 불일치`로 분류한다 |
| TEST-PCS-013 | 자유문자 CoveragePeriod | 날짜 판정 | 자동 파싱하지 않는다 |
| TEST-PCS-014 | VisitType 일치 또는 불일치 | rule 판정 | 명시적 동일성으로 통과 또는 불일치를 결정한다 |
| TEST-PCS-015 | SurgeryRule required 또는 excluded | rule 판정 | HasSurgery 확정값으로 판정한다 |
| TEST-PCS-016 | PrescriptionRule required 또는 excluded | rule 판정 | HasPrescription 확정값으로 판정한다 |
| TEST-PCS-017 | DiagnosisRuleMode any | 진단 판정 | 진단코드와 관계없이 통과한다 |
| TEST-PCS-018 | prefix_list와 일치하는 진단코드 | 진단 판정 | 점과 하이픈을 보존한 Ordinal prefix 비교로 통과한다 |
| TEST-PCS-019 | prefix_list이나 진단코드 없음 | 진단 판정 | `추가 확인 필요`로 분류한다 |
| TEST-PCS-020 | 명시적 불일치와 확인 불가가 함께 존재 | 결과 분류 | `현재 입력과 불일치`가 우선한다 |
| TEST-PCS-021 | 모든 적용 조건 통과 | 결과 분류 | `조건 일치 후보`로 분류한다 |
| TEST-PCS-022 | 과거 지급 또는 부지급 이력 | 현재 후보 분류 | 현재 결과 분류를 변경하지 않는다 |
| TEST-PCS-023 | Tier A, B, C 유사 청구 | 유사 청구 정렬 | tier와 날짜, update, 내부 ID 순으로 최대 3건을 반환한다 |
| TEST-PCS-024 | 화면 09 검색 완료 | durable files 확인 | `ClaimReferenceResult` 또는 검색 로그를 생성하지 않는다 |
| TEST-PCS-025 | 사용자가 후보만 선택 | 저장 상태 확인 | `ClaimSubmission`을 저장하지 않는다 |
| TEST-PCS-026 | 사용자가 화면 08 저장 실행 | 저장 상태 확인 | 기존 계약으로 coverage reference와 표시 snapshot을 저장한다 |
| TEST-PCS-027 | orphan, unknown 또는 ownership mismatch | 전체 graph 검증 | 전체 검색을 fail-closed 처리한다 |
| TEST-PCS-028 | 정상 load 후 reload 실패 | projection 확인 | 이전 결과와 선택을 제거한다 |
| TEST-PCS-029 | 결과, 오류 및 Automation tree | privacy 검사 | raw ID, 경로, 예외 및 민감정보가 없다 |
| TEST-PCS-030 | 결과 및 안내 문구 | prohibited wording 검사 | 확정적 청구, 지급 또는 보장 표현이 없다 |
| TEST-PCS-031 | `policy_document` source | reference 검증 | 같은 Policy 문서만 허용한다 |
| TEST-PCS-032 | 모순된 effective dates | 전체 graph 검증 | 전체 검색을 fail-closed 처리한다 |

Acceptance scenario count: `32`.

## 8. Traceability

| Decision | User work / requirement | Function | Rules or data | Owner | Tests | Follow-up WBS | Status |
|---|---|---|---|---|---|---|---|
| DEC-PCS-001 | REQ-PCS-001, REQ-PCS-002 | FUNC-PCS-006, FUNC-PCS-007 | 화면 역할 분리 | 화면 03, 화면 09 | TEST-PCS-001, TEST-PCS-002, TEST-PCS-003 | WBS-PCS-01, WBS-PCS-04 | Approved |
| DEC-PCS-002 | REQ-PCS-003 | FUNC-PCS-009 | DATA-PCS-001 | PolicyCoverage persistence owner | TEST-PCS-004, TEST-PCS-031 | WBS-PCS-02 | Logical contract approved |
| DEC-PCS-003 | REQ-PCS-003 | FUNC-PCS-008, FUNC-PCS-009 | DATA-PCS-002 | matching and persistence owners | TEST-PCS-005 through TEST-PCS-009 | WBS-PCS-02, WBS-PCS-03 | Approved |
| DEC-PCS-004 | REQ-PCS-004 | FUNC-PCS-008 | DATA-PCS-003 through DATA-PCS-006, RULE-PCS-001 | matching engine | TEST-PCS-013 through TEST-PCS-019, TEST-PCS-031 | WBS-PCS-02, WBS-PCS-03 | Approved |
| DEC-PCS-005 | REQ-PCS-002, REQ-PCS-004 | FUNC-PCS-007 | DATA-PCS-007 | 화면 09 | TEST-PCS-002, TEST-PCS-003 | WBS-PCS-03, WBS-PCS-04 | Approved |
| DEC-PCS-006 | REQ-PCS-003 | FUNC-PCS-008 | RULE-PCS-002 | matching engine | TEST-PCS-004, TEST-PCS-010 | WBS-PCS-03 | Approved |
| DEC-PCS-007 | REQ-PCS-004 | FUNC-PCS-008 | RULE-PCS-003 | matching engine | TEST-PCS-011 through TEST-PCS-013, TEST-PCS-032 | WBS-PCS-03 | Approved |
| DEC-PCS-008 | REQ-PCS-004 | FUNC-PCS-008 | RULE-PCS-004 | matching engine | TEST-PCS-017 through TEST-PCS-019 | WBS-PCS-03 | Approved |
| DEC-PCS-009 | REQ-PCS-004 | FUNC-PCS-008 | RULE-PCS-005 through RULE-PCS-007 | matching engine | TEST-PCS-014 through TEST-PCS-016 | WBS-PCS-03 | Approved |
| DEC-PCS-010 | REQ-PCS-005 | FUNC-PCS-007, FUNC-PCS-008 | DATA-PCS-008, RULE-PCS-008 | 화면 09 and matching engine | TEST-PCS-020, TEST-PCS-021, TEST-PCS-030 | WBS-PCS-03, WBS-PCS-04 | Approved |
| DEC-PCS-011 | REQ-PCS-005 | FUNC-PCS-001 | RULE-PCS-009 | 화면 09 projection | TEST-PCS-021, TEST-PCS-029 | WBS-PCS-04 | Approved |
| DEC-PCS-012 | REQ-PCS-006 | FUNC-PCS-002 | RULE-PCS-010, RULE-PCS-011 | matching engine and 화면 09 | TEST-PCS-022, TEST-PCS-023 | WBS-PCS-03, WBS-PCS-04 | Approved |
| DEC-PCS-013 | REQ-PCS-007, REQ-PCS-008 | FUNC-PCS-003, FUNC-PCS-004, FUNC-PCS-010 | 저장 경계 | 화면 08 save owner | TEST-PCS-024 through TEST-PCS-026 | WBS-PCS-04 | Approved |
| DEC-PCS-014 | REQ-PCS-009, REQ-PCS-010 | FUNC-PCS-005 | ERR-PCS-001 through ERR-PCS-004 | read-only projection pipeline | TEST-PCS-027, TEST-PCS-028, TEST-PCS-032 | WBS-PCS-02, WBS-PCS-03, WBS-PCS-04 | Approved |
| DEC-PCS-015 | REQ-PCS-011 | privacy boundary | ERR-PCS-005 through ERR-PCS-007 | all follow-up owners | TEST-PCS-029, TEST-PCS-030 | WBS-PCS-01 through WBS-PCS-04 | Approved |

Traceability result: every `DEC-PCS-*` is connected to at least one requirement, acceptance scenario and follow-up WBS.

## 9. Conflicting Legacy Statements

| Existing document | Existing state or wording | New contract | Change reason | Affected WBS |
|---|---|---|---|---|
| `docs/23_SCREEN_TO_DATA_MODEL_MAPPING.md` | 화면 03이 진단, 진료상황 및 담보 후보 조건 검색을 포함 | 화면 03은 등록 Policy 필터만 소유 | 화면 03과 화면 09 혼용 제거 | WBS-PCS-01, WBS-PCS-04 |
| `docs/33_UI_STATE_AND_CONFIRMATION_MESSAGE_GUIDE.md` | 화면 03을 조건 기반 보험 후보 검색으로 설명 | 화면 03은 가족, 보험사, 상태, 상품, 키워드 필터 | 진료 조건의 owner를 화면 09로 이동 | WBS-PCS-01 |
| `docs/06_DATA_MODEL.md` | `해당 가능 담보` 후보 표현 | `조건 일치 후보` | 지급 또는 보장 확정 오해 방지 | WBS-PCS-03, WBS-PCS-04 |
| `docs/23_SCREEN_TO_DATA_MODEL_MAPPING.md` | 조건 불일치 담보 제외, 확인 필요 담보 표시 | 세 결과 그룹을 모두 안전한 참고 분류로 표시 | 분류 근거와 fail-closed 감사 가능성 확보 | WBS-PCS-03, WBS-PCS-04 |
| `docs/06_DATA_MODEL.md`, `docs/23_SCREEN_TO_DATA_MODEL_MAPPING.md`, `docs/35_PRE_IMPLEMENTATION_DECISION_MATRIX.md` | 선택 또는 제출 판단 결과의 snapshot 저장 후보 | 화면 09 결과와 검색 로그는 저장하지 않고 화면 08 explicit save만 durable owner | 민감정보 최소화와 저장 owner 단일화 | WBS-PCS-04 |
| `docs/28_DATA_MODEL_V5_5_PROPOSED.md` | durable `ClaimReferenceResultId`를 포함한 proposed object | identity 없는 read-only projection | 조회 결과의 과도한 영속화 방지 | WBS-PCS-03, WBS-PCS-04 |
| `docs/06_DATA_MODEL.md`, `docs/28_DATA_MODEL_V5_5_PROPOSED.md` | PolicyCoverage candidate field set | DATA-PCS-001의 logical contract | 상태, 날짜, source 및 rule 경계를 명시 | WBS-PCS-02, WBS-PCS-03 |

기존 문서는 삭제하거나 수정하지 않는다. 위 충돌은 이 문서의 주제 범위에서만 새 계약이 우선한다.

## 10. Risk Register

| ID | Risk | Control |
|---|---|---|
| RISK-PCS-001 | 조건 일치를 지급 가능성으로 오인 | DEC-PCS-010의 안전한 결과명과 필수 안내 |
| RISK-PCS-002 | 화면 03과 화면 09 역할 혼용 | DEC-PCS-001과 별도 owner |
| RISK-PCS-003 | OCR 후보를 확정 근거로 사용 | user_confirmed만 허용, 외부 OCR 금지 |
| RISK-PCS-004 | 민감정보 또는 진단정보 과다 저장 | DEC-PCS-013과 DEC-PCS-015 |
| RISK-PCS-005 | 자유문자 기간의 잘못된 날짜 해석 | RULE-PCS-003에서 자동 파싱 금지 |
| RISK-PCS-006 | 과거 지급이 미래 결과를 보장하는 오해 | DEC-PCS-012의 별도 projection과 필수 안내 |
| RISK-PCS-007 | 계약 문서가 T3 persistence 구현 승인으로 오인 | WBS-PCS-02 별도 승인과 비승인 상태 명시 |

## 11. Deferred Decisions and Boundaries

이 계약에 필요한 mandatory product decisions는 모두 확정되었다. 다음 물리 구현 결정은 의도적으로 후속 승인에 남긴다.

- JSON 파일명과 envelope
- atomic write, backup 및 migration 방식
- `PolicyCoverage` storage interface와 implementation
- OCR 후보 생성 및 사용자 확인 workflow
- source locator의 물리 형식과 공개 범위

이 항목은 승인 규칙의 미결정이 아니라 WBS-PCS-02의 별도 T3 scope다.

Deferred minor preserved:

- 홈 최근 5건 영역의 `통합 청구 이력` 제목은 다음 UI resource 변경 배치에서 `최근 청구 활동`으로 수정한다.

## 12. Follow-up WBS

### WBS-PCS-01 - T2_POLICY_SEARCH_B_REGISTERED_POLICY_READ_ONLY_SEARCH

화면 03에 기존 `Policy` 데이터 기반 검색과 필터를 구현한다. `PolicyCoverage` 또는 진료 조건 매칭은 포함하지 않는다.

### WBS-PCS-02 - T3_POLICY_COVERAGE_PERSISTENCE_MVP

`PolicyCoverage` 저장, 수정, 사용 중지, 복원 및 reference integrity를 구현한다. 새 schema와 storage이므로 별도 T3 지시서가 필요하다.

### WBS-PCS-03 - T2_CLAIM_REFERENCE_MATCHING_ENGINE

확정된 `PolicyCoverage`와 `ClaimCase`를 사용하는 pure read-only matching engine을 구현한다.

### WBS-PCS-04 - T2_CLAIM_REFERENCE_RESULT_UI

화면 09 결과 그룹, 근거, 유사 청구 및 화면 08 draft 이동을 구현한다.

### WBS-PCS-05 - T2_HOME_RECENT_ACTIVITY_TITLE_MINOR

`통합 청구 이력`을 `최근 청구 활동`으로 정정한다. 다음 UI resource 변경 배치에 함께 처리한다.

WBS order: `WBS-PCS-01 -> WBS-PCS-02 -> WBS-PCS-03 -> WBS-PCS-04 -> WBS-PCS-05`.

WBS-PCS-02는 이 문서로 구현 권한을 받지 않는다.

## 13. ID Inventory and Gate Result

| ID family | Count |
|---|---:|
| `OBJ-PCS` | 3 |
| `REQ-PCS` | 12 |
| `FUNC-PCS` | 10 |
| `RULE-PCS` | 11 |
| `DATA-PCS` | 8 |
| `ERR-PCS` | 7 |
| `TEST-PCS` | 32 |
| `DEC-PCS` | 15 |
| `RISK-PCS` | 7 |
| `WBS-PCS` | 5 |

- Unresolved mandatory decisions: `0`
- Code changes: `0`
- Test changes: `0`
- JSON/schema/migration changes: `0`
- Runtime changes or execution: `0`
- Actual data access: `0`
- External API/AI/cloud use: `0`
- Production/deployment authorization: `0`

Final document gate: `PASS_USER_REVIEW_AND_DOCUMENTATION_COMMIT_AUTHORIZED`

Implementation readiness: `READY_FOR_T2_POLICY_SEARCH_B_REGISTERED_POLICY_READ_ONLY_SEARCH`

PolicyCoverage persistence: `NOT_IMPLEMENTED`, `NOT_AUTHORIZED_IN_THIS_BATCH`

Production readiness: `NOT_EVALUATED`

Deployment: `NOT_AUTHORIZED`
