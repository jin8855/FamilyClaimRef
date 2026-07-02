# Data Model 06 Update Plan

## 1. 목적

이 문서는 `docs/06_DATA_MODEL.md`를 직접 수정하기 전에, 어떤 섹션을 어떻게 바꿀지 정리하는 수정 계획이다.

이번 작업에서는 `docs/06_DATA_MODEL.md`를 수정하지 않는다. 이 문서는 후속 수정 지시를 위한 계획 문서이며, DB 생성, OCR 구현, 앱 구현 지시가 아니다.

## 2. 현재 docs/06_DATA_MODEL.md 기준 요약

`docs/06_DATA_MODEL.md`는 다음 구조를 가진다.

| 현재 섹션 | 현재 핵심 내용 | 판단 |
|---|---|---|
| `1. 설계 원칙` | OCR 원문과 사용자 확정 데이터 분리, 청구 사건/보험사별 청구/지급 결과 분리, 원본 문서 경로 저장 | 유지 필요 |
| `2. 핵심 엔티티` | `Person`, `Policy`, `Coverage`, `Document`, `OcrExtraction`, `ReviewCandidate`, `ClaimCase`, `ClaimSubmission`, `ClaimPayment` | V5.5 명칭 반영 필요 |
| `3. ERD` | 기존 엔티티 중심 Mermaid ERD | V5.5 객체 관계로 재정리 필요 |
| `4. Person` | 가족 구성원 예시 | `FamilyMember`로 rename 필요 |
| `5. Policy` | 가입 보험 예시 | 유지하되 `FamilyMember` 연결로 변경 필요 |
| `6. Coverage` | 담보/특약 예시 | `PolicyCoverage`로 rename 필요 |
| `7. Document` | 단일 원본 문서 예시 | 단일 `Document` 후보 유지하되 `PolicyDocument` / `ClaimDocument` 도메인 명칭 설명 필요 |
| `8. OcrExtraction` | OCR 실행 결과 예시 | `OcrCandidate` 중심으로 재정리하고 실행 기록은 Candidate로 보류 |
| `9. ReviewCandidate` | 추출 후보값 예시 | `OcrCandidate.reviewStatus` 후보로 흡수 계획 필요 |
| `10. ClaimCase` | 하나의 진료/치료 건 예시 | 유지하되 V5.5 청구 시작 화면 기준 필드 후보 반영 필요 |
| `11. ClaimSubmission` | 보험사별 청구 기록 예시 | 유지하되 5단계 흐름과 지급 결과 분리 명확화 필요 |
| `12. ClaimPayment` | 지급/부지급 결과 예시 | 유지하되 감액/부지급 사유 구조 보강 필요 |
| `13. 조회 규칙 초안` | 담보 후보 매칭, 과거 유사 청구 조회 | V5.5 보험 찾기, Top 3, 태그 기준 반영 필요 |
| `14. 데이터 상태 원칙` | `candidate`, `user_confirmed`, `ignored`, `needs_review` | 상태값 후보 확장 필요 |

## 3. V5.5 기준 반영 방향

- `Person`은 `FamilyMember`로 반영하고, `Person`은 legacy alias로 기록한다.
- `Coverage`는 `PolicyCoverage`로 반영하고, `Coverage`는 legacy alias로 기록한다.
- 화면/도메인 문서 명칭은 `PolicyDocument`, `ClaimDocument`로 분리한다.
- 물리 저장 구조는 우선 단일 `Document` 후보를 설명하되, 물리 분리 가능성을 `Needs Decision`으로 남긴다.
- `OcrCandidate`를 후보값 객체로 반영한다.
- `OcrExtraction`은 OCR 실행 기록 후보로 보류한다.
- `ReviewCandidate`는 `OcrCandidate.reviewStatus` 또는 후보 검토 상태로 흡수하는 방향을 둔다.
- `ClaimReferenceResult`는 조회 결과 객체 후보로 추가하되, 전체 자동 저장을 확정하지 않는다.
- `HistoryItem`은 우선 projection 후보로 추가한다.
- `Category`, `CategoryItem`은 관리 데이터 객체로 추가한다.
- `Tag`는 별도 객체 `Candidate`로 추가한다.
- `ClaimMemo`, `HistoryMemo`는 별도 객체 후보로만 둔다.
- 삭제와 사용 중지 정책, 파일 경로 마스킹, 민감정보 최소 저장 기준을 보강한다.

## 4. 섹션별 수정 계획

| 대상 섹션 | 현재 내용 | 수정 방향 | 근거 문서 | 위험 |
|---|---|---|---|---|
| `1. 설계 원칙` | 보안과 분리 원칙 중심 | 유지하되 `Candidate`와 사용자 확정값 경계, Git 제외 경로, 파일명 마스킹 기준 보강 | `README.md`, `docs/28_DATA_MODEL_V5_5_PROPOSED.md`, `docs/29_DATA_MODEL_CORE_DECISIONS.md` | 원칙이 구현 지시처럼 읽히지 않도록 주의 |
| `2. 핵심 엔티티` | 기존 9개 엔티티 | V5.5 명칭과 상태를 반영한 핵심 객체 표로 교체 계획 | `docs/27_DATA_MODEL_NAMING_DECISION.md`, `docs/29_DATA_MODEL_CORE_DECISIONS.md` | `Candidate`를 `Confirmed`로 과도 승격하면 안 됨 |
| `3. ERD` | 기존 Mermaid ERD | 단일 `Document` 후보와 도메인 명칭 분리를 함께 설명하는 관계도로 수정 계획 | `docs/28_DATA_MODEL_V5_5_PROPOSED.md`, `docs/29_DATA_MODEL_CORE_DECISIONS.md` | 물리 저장 구조가 확정된 것처럼 보일 수 있음 |
| `4. Person` | `Person` JSON 예시 | `FamilyMember` 섹션으로 rename, `Person` legacy alias 기록 | `docs/27_DATA_MODEL_NAMING_DECISION.md` | 기존 문서 참조와 alias 누락 위험 |
| `5. Policy` | `personId` 연결 | `familyMemberId` 연결로 변경 계획, 증권번호 전체값 저장 금지 유지 | `docs/23_SCREEN_TO_DATA_MODEL_MAPPING.md` | 실제 보험사명 샘플 사용 금지 |
| `6. Coverage` | `Coverage` JSON 예시 | `PolicyCoverage`로 rename, 약관 근거와 조건 규칙 후보 보강 | `docs/27_DATA_MODEL_NAMING_DECISION.md`, `docs/28_DATA_MODEL_V5_5_PROPOSED.md` | 담보 객체를 저장 확정 이상으로 과장하지 않기 |
| `7. Document` | 단일 `Document` | 단일 물리 저장 후보를 유지하되 `PolicyDocument` / `ClaimDocument` 도메인 명칭과 연결 규칙 추가 | `docs/29_DATA_MODEL_CORE_DECISIONS.md` | 문서 물리 분리 여부는 보류로 남겨야 함 |
| `8. OcrExtraction` | OCR 실행 결과 | `OcrCandidate` 중심 설명으로 전환하고 `OcrExtraction`은 실행 기록 후보로 보류 | `docs/29_DATA_MODEL_CORE_DECISIONS.md` | OCR 구현 지시처럼 작성하면 안 됨 |
| `9. ReviewCandidate` | 추출 후보값 | `OcrCandidate` 상태 또는 검토 상태로 흡수 계획, 사용자 확정값 경계 명시 | `docs/23_SCREEN_TO_DATA_MODEL_MAPPING.md`, `docs/29_DATA_MODEL_CORE_DECISIONS.md` | 후보값과 확정값 덮어쓰기 위험 |
| `10. ClaimCase` | 진료/치료 건 | 5단계 청구 흐름의 시작 객체로 설명 보강 | `docs/28_DATA_MODEL_V5_5_PROPOSED.md` | 청구 완료와 지급 완료 혼동 위험 |
| `11. ClaimSubmission` | 보험사별 청구 기록 | 보험사별 진행 상태와 제출 서류 참조 기준 보강 | `docs/23_SCREEN_TO_DATA_MODEL_MAPPING.md` | `ClaimPayment` 상태와 중복 위험 |
| `12. ClaimPayment` | 지급 결과 | 지급/부지급/감액 결과와 민감정보 기준 보강 | `docs/24_DATA_MODEL_GAP_REVIEW.md` | `ClaimCase` 직접 연결 여부를 확정하지 않도록 주의 |
| `13. 조회 규칙 초안` | 담보 후보 매칭, 과거 유사 청구 | `ClaimReferenceResult`, Top 3, 진단명/키워드/태그/prefix 기준 반영 | `docs/28_DATA_MODEL_V5_5_PROPOSED.md` | 조회 결과 저장 여부를 확정하지 않기 |
| `14. 데이터 상태 원칙` | 기본 상태 4개 | 문서, OCR 후보, 청구 사건, 제출, 지급, 관리 데이터 상태 후보 확장 | `docs/23_SCREEN_TO_DATA_MODEL_MAPPING.md`, `docs/28_DATA_MODEL_V5_5_PROPOSED.md` | 상태값이 최종 구현 enum처럼 보일 수 있음 |
| 신규 섹션 | 없음 | `Category` / `CategoryItem` / `Tag`, 메모 후보, 삭제/사용 중지, 민감정보 기준, `Needs Decision` 섹션 추가 계획 | `docs/29_DATA_MODEL_CORE_DECISIONS.md` | 문서가 과도하게 길어질 수 있음 |

## 5. 객체명 변경 계획

| 기존 명칭 | 반영 명칭 | 처리 방식 |
|---|---|---|
| `Person` | `FamilyMember` | rename, legacy alias 기록 |
| `Coverage` | `PolicyCoverage` | rename, legacy alias 기록 |
| `Document` | `PolicyDocument` / `ClaimDocument` | 도메인 명칭 분리, 물리 저장은 단일 `Document` 우선 후보 |
| `OcrExtraction` | `OcrCandidate` / `OcrExtraction` Candidate | 후보값과 실행 기록 구분 |
| `ReviewCandidate` | `OcrCandidate.reviewStatus` Candidate | 후보 검토 상태로 흡수 후보 |

## 6. 추가할 객체

| 추가 객체 | 반영 방식 | 상태 |
|---|---|---|
| `ClaimReferenceResult` | 보험 찾기 조회 결과 객체 후보로 추가 | Candidate |
| `HistoryItem` | 이력 보기 projection 후보로 추가 | Candidate |
| `Category` | 관리 데이터 상위 분류로 추가 | Confirmed for planning |
| `CategoryItem` | 관리 데이터 항목으로 추가 | Confirmed for planning |
| `Tag` | 검색용 태그 별도 객체 후보로 추가 | Candidate |
| `ClaimMemo` | 청구 메모 별도 객체 후보로 기록 | Candidate |
| `HistoryMemo` | 이력 메모 별도 객체 후보로 기록 | Candidate |

## 7. 유지할 Candidate / Needs Decision

| 항목 | 상태 | 유지 이유 |
|---|---|---|
| `PolicyDocument` / `ClaimDocument` 물리 분리 | Needs Decision | 도메인 명칭은 분리했지만 단일 `Document` 저장 후보가 있음 |
| `OcrExtraction` 별도 객체 | Needs Decision | OCR 실행 로그 저장 필요성이 미정 |
| OCR 원문 전체 저장 | Needs Decision | 민감정보 위험 때문에 기본 저장하지 않는 방향이나 예외 미정 |
| `ClaimReferenceResult` 전체 저장 | Needs Decision | 조회 결과와 판단 근거 snapshot의 경계 미정 |
| `HistoryItem` 저장 객체 | Needs Decision | projection 우선이나 성능 요구 미확인 |
| `Tag` 별도 객체 | Needs Decision | 단순 `CategoryItem`으로 충분한지 미정 |
| `ClaimMemo` / `HistoryMemo` 별도 객체 | Needs Decision | 단순 `memo` 필드로 시작할 수 있음 |
| 삭제 요청 후 복구 정책 | Needs Decision | `disabled`, `delete_requested` 후속 처리 미정 |
| 파일명 마스킹 규칙 | Needs Decision | 원본 파일명에 민감정보가 포함될 수 있음 |

## 8. 수정하지 말아야 하는 항목

- `docs/06_DATA_MODEL.md`를 이번 작업에서 직접 수정하지 않는다.
- 기존 문서를 overwrite하지 않는다.
- HTML, CSS, JavaScript 파일을 수정하지 않는다.
- DB 파일, OCR 구현 파일, runtime scaffold를 생성하지 않는다.
- `app/`, `src/`, `package.json`, `tsconfig.json`을 생성하지 않는다.
- `attachments/`, `data/local/` 내부에 파일을 생성하지 않는다.
- 실제 개인정보 샘플, 실제 가족 실명, 실제 보험사명, 실제 병원명, 실제 진단코드 기반 개인 사례를 추가하지 않는다.
- `Candidate` 또는 `Needs Decision` 항목을 근거 없이 `Confirmed`로 승격하지 않는다.

## 9. 실제 수정 때 확인할 항목

`docs/06_DATA_MODEL.md`를 실제로 수정하는 별도 작업에서는 다음을 확인한다.

- 기존 `Person` 참조가 `FamilyMember`로 정리되었는가
- `Person` legacy alias가 기록되었는가
- 기존 `Coverage` 참조가 `PolicyCoverage`로 정리되었는가
- `Coverage` legacy alias가 기록되었는가
- `PolicyDocument` / `ClaimDocument` 도메인 명칭과 단일 `Document` 물리 저장 후보가 구분되는가
- `OcrCandidate`와 사용자 확정값 저장 경계가 분리되는가
- `OcrExtraction`은 실행 기록 후보로 보류되는가
- `ClaimReferenceResult`는 전체 자동 저장이 아니라 조회 결과 또는 선택 snapshot 후보로 설명되는가
- `HistoryItem`은 projection 우선으로 설명되는가
- `Category`, `CategoryItem`, `Tag` 경계가 과도하게 확정되지 않았는가
- `ClaimMemo`, `HistoryMemo`는 별도 객체 후보로만 남아 있는가
- 삭제와 사용 중지 정책이 분리되어 있는가
- 실제 개인정보 샘플이 추가되지 않았는가
- 물리 DB 테이블 생성 지시처럼 읽히지 않는가

## 10. 다음 작업

1. 사용자가 `docs/29_DATA_MODEL_CORE_DECISIONS.md`와 이 문서를 검토한다.
2. `docs/06_DATA_MODEL.md`를 수정해도 되는지 별도 승인한다.
3. 승인 후 `docs/06_DATA_MODEL.md`만 대상으로 한 좁은 수정 지시문을 작성한다.
4. 수정 시에는 기존 문서의 의미를 보존하면서 V5.5 명칭과 보류 항목을 반영한다.
5. 수정 후 `docs/23_SCREEN_TO_DATA_MODEL_MAPPING.md`, `docs/24_DATA_MODEL_GAP_REVIEW.md`, `docs/28_DATA_MODEL_V5_5_PROPOSED.md`와 용어 정합성을 검증한다.
