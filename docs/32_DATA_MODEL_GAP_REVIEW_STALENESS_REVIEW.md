# 32_DATA_MODEL_GAP_REVIEW_STALENESS_REVIEW

## 1. Goal

`docs/24_DATA_MODEL_GAP_REVIEW.md`가 최신 데이터 모델 결정 문서와 비교해 여전히 유효한 gap 문서인지 검토한다.

이 문서는 기존 문서를 수정하지 않고, stale 표현과 유지해야 할 `Needs Decision` 항목을 분리해 다음 패치 후보만 기록한다.

## 2. Checked Files / Paths

- `docs/06_DATA_MODEL.md`
- `docs/23_SCREEN_TO_DATA_MODEL_MAPPING.md`
- `docs/24_DATA_MODEL_GAP_REVIEW.md`
- `docs/27_DATA_MODEL_NAMING_DECISION.md`
- `docs/29_DATA_MODEL_CORE_DECISIONS.md`
- `docs/30_DATA_MODEL_06_UPDATE_PLAN.md`
- `docs/31_DATA_MODEL_TERMINOLOGY_CONSISTENCY_REVIEW.md`

## 3. Review Basis

- `06_DATA_MODEL.md`는 V5.5 기준으로 `Confirmed for planning`, `Candidate`, `Needs Decision`을 분리한다.
- `23_SCREEN_TO_DATA_MODEL_MAPPING.md`는 화면별 저장/조회 객체를 기준으로 데이터 모델 사용 위치를 정리한다.
- `24_DATA_MODEL_GAP_REVIEW.md`는 이전 기준에서 미확정 항목과 위험을 기록한 gap 문서다.
- `27_DATA_MODEL_NAMING_DECISION.md`와 `29_DATA_MODEL_CORE_DECISIONS.md`는 명칭과 핵심 객체 결정의 최신 기준이다.
- `30_DATA_MODEL_06_UPDATE_PLAN.md`는 `06_DATA_MODEL.md` 반영 계획과 보수적 수정 범위를 설명한다.
- `31_DATA_MODEL_TERMINOLOGY_CONSISTENCY_REVIEW.md`는 `06`과 `23` 사이의 용어 정합성 검토 결과를 제공한다.

## 4. Summary Result

`PATCH_RECOMMENDED`

`24_DATA_MODEL_GAP_REVIEW.md`는 큰 위험 항목을 여전히 보존하고 있으므로 다음 단계 진행을 막는 수준은 아니다. 다만 일부 객체가 최신 문서에서 `Confirmed for planning`으로 정리되었는데도 `Candidate` 또는 별도 객체 여부 미결정처럼 남아 있어, 구현 전에는 표현을 갱신하는 패치가 권장된다.

## 5. Resolved Gaps

| Gap Item | Old Expression in 24 | Latest Basis | Recommendation |
|---|---|---|---|
| `Person` 명칭 | `FamilyMember`가 확인된 객체로 정리되어 있으나 legacy alias 정리는 약함 | `27`, `31`에서 `Person` -> `FamilyMember` alias 확인 | `24`에 legacy alias 해소 항목으로 표시 |
| `Coverage` 명칭 | `PolicyCoverage` 별도 객체 여부가 결정 필요로 남음 | `06`, `27`, `31`에서 `Coverage` -> `PolicyCoverage`, planning object confirmed | 별도 객체 명칭은 해소, 구현 세부만 `Needs Decision`으로 분리 |
| `PolicyDocument` / `ClaimDocument` 도메인 명칭 | 문서 객체 분리가 추가 필요 객체 후보로 표현됨 | `06`, `27`, `31`에서 도메인 명칭은 confirmed, 물리 저장 분리는 `Needs Decision` | 도메인 명칭 confirmed와 물리 분리 미결정을 분리 |
| `OcrCandidate` 명칭 | 추가 필요 객체 후보로 표현됨 | `06`, `23`, `31`에서 후보값 객체로 confirmed for planning | 객체명은 해소, 저장 경계와 review status는 `Needs Decision`으로 유지 |
| `Category` / `CategoryItem` | `Candidate` 또는 추가 필요 객체 후보 성격으로 남음 | `06`, `23`, `27`에서 관리 분류와 항목 planning object confirmed | `Category` / `CategoryItem`은 resolved로 정리 |
| `ClaimMemo` / `HistoryMemo` | 별도 객체 여부 결정 필요로만 표현됨 | `06`, `29`에서 Candidate object, 별도 객체 여부는 `Needs Decision` | Candidate status와 별도 객체 결정 필요를 같이 기록 |
| 사용자 확정값 반영 원칙 | 위험 항목으로 남아 있으나 기준 표현이 약함 | `06`, `23`, `31`에서 사용자 확정값만 업무 객체 반영 | 원칙은 confirmed, 저장 위치만 `Needs Decision`으로 유지 |
| `ClaimCase` 완료와 `ClaimSubmission` 완료 분리 | 결정 필요 사항으로 남음 | `06`, `29`에서 사건 완료와 제출 완료를 분리하는 방향 확인 | 방향은 resolved, 상태값 세부와 UI 반영 범위는 유지 검토 |
| `ClaimPayment` 연결 방향 | `ClaimSubmission` 종속 여부 결정 필요로 남음 | `06`, `23`, `29`에서 지급 결과는 제출 단위 중심, 이력은 projection 가능 | 제출 단위 중심으로 보정하고 direct case link 여부만 보조 결정으로 유지 |

## 6. Still Valid Needs Decision Items

| Item | Why Still Valid | Related Latest Document | Recommendation |
|---|---|---|---|
| `PolicyDocument` / `ClaimDocument` 물리 분리 여부 | 도메인 명칭은 정리되었지만 저장 테이블 분리는 결정되지 않음 | `06`, `31` | `Needs Decision` 유지 |
| 단일 `Document` 물리 저장 구조 | 파일 메타데이터 공통 저장 후보이나 실제 물리 모델은 미정 | `06`, `31` | 구현 전 결정 |
| `OcrExtraction` 별도 객체 여부 | OCR 실행 기록 후보는 남아 있으나 기본 모델로 확정되지 않음 | `06`, `31` | Candidate 유지 |
| OCR 원문 전체 저장 여부 | 보안 원칙상 기본 저장하지 않는 방향이나 예외 정책은 미정 | `06`, `31` | 예외 조건 별도 결정 |
| `OcrCandidate` 저장 경계 | 후보값 저장 범위, review status, 사용자 확정값 분리 방식이 미정 | `06`, `23`, `31` | `Needs Decision` 유지 |
| 사용자 확정값 저장 위치 | 업무 객체 직접 반영과 별도 확정값 기록의 경계가 미정 | `23`, `31` | 구현 전 확정 |
| `ClaimReferenceResult` snapshot 저장 범위 | 유사 청구 결과를 매번 계산할지 snapshot으로 남길지 미정 | `06`, `23`, `24` | `Needs Decision` 유지 |
| `HistoryItem` 저장 객체 여부 | 이력 화면 projection인지 저장 객체인지 아직 확정되지 않음 | `06`, `23`, `31` | projection 기본, 저장 전환은 보류 |
| `Tag` 별도 객체 여부 | `CategoryItem`과 분리할지 아직 결정되지 않음 | `06`, `23`, `24` | `Needs Decision` 유지 |
| `ClaimMemo` / `HistoryMemo` 별도 객체 여부 | 후보 객체는 있으나 별도 객체 채택은 미정 | `06`, `29` | Candidate + `Needs Decision` 유지 |
| 삭제 요청 후 복구 정책 | 삭제와 사용 중지의 정책 차이는 화면에는 있으나 데이터 정책 미정 | `24`, `29` | 구현 전 정책화 |
| 파일명 마스킹 세부 규칙 | 마스킹 원칙은 있으나 파일명 포맷과 저장 위치는 세부 미정 | `06`, `31` | `24`에 보강 권장 |
| 물리 DB 테이블 구조 | 현재 문서는 개념 모델 단계이며 DB 구현이 아님 | `06`, `30`, `31` | 확정 금지 |

## 7. Stale / Needs Patch Expressions

| Area | Current Expression in 24 | Conflict / Risk | Recommended Patch |
|---|---|---|---|
| `PolicyCoverage` | 별도 객체 여부 결정 필요 | 최신 기준에서는 planning object 명칭은 confirmed | 객체명은 confirmed, 담보 자동 추출/상세 속성/물리 구현만 `Needs Decision`으로 변경 |
| `PolicyDocument` / `ClaimDocument` | 추가 필요 객체 후보 또는 분리 여부 중심 | 도메인 명칭 confirmed와 물리 저장 split 후보가 섞임 | 도메인 객체명 confirmed, 물리 분리 여부 `Needs Decision`으로 분리 |
| `OcrCandidate` | 추가 필요 객체 후보 | 최신 기준에서는 OCR 후보값 객체로 planning confirmed | `OcrCandidate` 자체는 confirmed for planning, `OcrExtraction`과 저장 경계만 Candidate로 수정 |
| `Category` / `CategoryItem` | Candidate 성격으로 표현 | 최신 기준에서는 관리 데이터 화면 기준 planning object | confirmed for planning으로 보정 |
| `ClaimMemo` / `HistoryMemo` | 결정 필요 사항으로만 표현 | 최신 기준에서는 Candidate object로 둔 뒤 별도 객체 여부 결정 | Candidate status를 추가하고 별도 객체 채택만 `Needs Decision`으로 정리 |
| `HistoryItem` | 저장 객체 여부 결정 필요 | 최신 기준과 충돌은 없으나 projection 기본 표현이 더 명확해짐 | projection 우선, 저장 객체 전환은 `Needs Decision`으로 보강 |
| OCR 원문 저장 | 민감정보 저장 위험으로 표현 | 최신 기준은 원문 전체 기본 저장 금지 방향을 더 명확히 함 | 기본 미저장, 예외 저장 조건만 `Needs Decision`으로 수정 |
| 파일명 마스킹 | 파일 경로와 메타데이터 저장 위치 중심 | 최신 기준에서는 파일명 마스킹 규칙도 중요 위험 | 파일명 마스킹 포맷과 원본명 보존 여부를 별도 gap으로 추가 |
| `ClaimPayment` 연결 | `ClaimSubmission`에만 종속되는지, `ClaimCase`에도 직접 연결되는지 | 최신 기준은 제출 단위 중심으로 더 기울어져 있음 | `ClaimSubmission` 중심으로 보정하고 `ClaimCase`는 조회 projection 연결 여부로 낮춤 |

## 8. Items That Should Not Be Changed Yet

| Item | Reason |
|---|---|
| `HistoryItem`을 저장 객체로 확정 | 최신 문서에서도 projection 후보 성격이 남아 있음 |
| `ClaimReferenceResult` 전체 snapshot 저장 확정 | snapshot 범위와 보존 기간이 아직 미정 |
| `PolicyDocument` / `ClaimDocument` 물리 테이블 분리 확정 | 도메인 명칭과 물리 저장 구조는 별도 결정 사항 |
| 단일 `Document` 후보 제거 | 공통 파일 메타데이터 저장 후보로 여전히 필요 |
| `Tag`를 별도 객체로 확정 | `CategoryItem`과의 경계가 아직 결정되지 않음 |
| OCR 원문 전체 저장 | 보안 원칙상 기본 저장하지 않는 방향이며 예외만 검토 대상 |
| `ClaimMemo` / `HistoryMemo`를 최종 객체로 승격 | Candidate 상태를 유지해야 함 |
| 물리 DB 테이블 구조 확정 | 현재 단계는 문서/시각화 검토이며 DB 구현 단계가 아님 |

## 9. Risks

- `24_DATA_MODEL_GAP_REVIEW.md`를 그대로 두면 resolved된 명칭 결정과 unresolved된 구현 결정을 구분하기 어렵다.
- `Candidate`와 `Confirmed for planning` 표현이 섞이면 다음 문서에서 같은 객체가 서로 다른 성숙도로 해석될 수 있다.
- OCR 후보값과 사용자 확정값의 경계는 여전히 구현 전 핵심 위험이다.
- 문서 파일 경로, 파일명 마스킹, 원본명 보존 여부는 민감정보 위험과 직접 연결된다.
- 이력과 유사 청구 결과를 저장 객체로 볼지 projection으로 볼지 결정하지 않으면 화면 조회 모델과 저장 모델이 섞일 수 있다.

## 10. Recommendation

`24_DATA_MODEL_GAP_REVIEW.md`는 폐기하지 말고 V5.5 최신 기준에 맞춰 보수적으로 패치하는 것이 적절하다.

패치 방향은 다음과 같다.

- resolved된 명칭 결정은 `Resolved` 또는 `Confirmed for planning`으로 이동한다.
- 구현 전 결정이 필요한 항목은 `Needs Decision`으로 남긴다.
- `Candidate`는 실제 객체 채택 전 후보에만 사용한다.
- `PolicyDocument` / `ClaimDocument`는 도메인 명칭과 물리 저장 구조를 분리해 설명한다.
- `OcrCandidate`는 후보값 객체로 인정하되, `OcrExtraction`, 원문 저장, 사용자 확정값 반영 경계는 보류한다.
- 파일명 마스킹과 원본명 보존 정책을 민감정보 gap에 추가한다.

## 11. Next Step

다음 작업은 `docs/24_DATA_MODEL_GAP_REVIEW.md`를 직접 수정하는 별도 패치 지시로 진행하는 것이 좋다.

권장 범위:

- `24`의 `Candidate` / `Needs Decision` 표현만 최신 기준에 맞춰 정리
- resolved gap과 still valid gap을 분리
- `06`, `23`, `31`의 기준과 충돌하지 않도록 용어만 보정
- DB 테이블, OCR 구현, 앱 구현으로 확장하지 않음
