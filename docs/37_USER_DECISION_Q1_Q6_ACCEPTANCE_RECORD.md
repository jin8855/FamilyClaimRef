# 37_USER_DECISION_Q1_Q6_ACCEPTANCE_RECORD

## 1. Goal

사용자가 답변한 Q1~Q6 결정을 app scaffold 전 사용자 결정 기록으로 정리한다.

이 문서는 구현 지시가 아니다. 데스크톱 앱 방향, 삭제/사용 중지 원칙, 해지 보험 노출 기준, 파일명 표시 선호, 원본 파일명 보존 후보, 보험사별 청구 상태 분리 원칙을 기록한다.

## 2. Checked Files / Paths

- `docs/06_DATA_MODEL.md`
- `docs/23_SCREEN_TO_DATA_MODEL_MAPPING.md`
- `docs/24_DATA_MODEL_GAP_REVIEW.md`
- `docs/33_UI_STATE_AND_CONFIRMATION_MESSAGE_GUIDE.md`
- `docs/34_PRE_IMPLEMENTATION_READINESS_CHECKLIST.md`
- `docs/35_PRE_IMPLEMENTATION_DECISION_MATRIX.md`
- `docs/36_USER_DECISION_QUESTIONS_BEFORE_IMPLEMENTATION.md`

## 3. Scope

포함 범위:

- Q1~Q6 사용자 답변 기록
- Accepted / Accepted for MVP / Accepted with Risk Note / Needs Decision / Needs Technical Design 분리
- app scaffold 전 영향 정리
- 데이터 모델, 화면, 보안 영향 후보 정리

제외 범위:

- app scaffold 생성
- WPF, WinForms, MAUI, Avalonia, Electron 등 기술 스택 확정
- DB 테이블 구조 확정
- OCR 구현 방식 확정
- HTML/CSS/JavaScript 수정
- 실제 개인정보 또는 실제 사례 예시 작성

## 4. User Answers Summary

| 질문 | 사용자 답변 요약 | 기록 판정 |
|---|---|---|
| Q1 앱 구현 방식 | 앱은 데스크톱 앱으로 한다. | Accepted |
| Q2 삭제/사용 중지 | 삭제보다 사용 중지로 이력을 유지한다. 보험은 유지/해지로 구분하고 해지 보험은 하단에 표시한다. | Accepted for MVP |
| Q3 해지 보험 노출 | 해지 보험은 청구 시 필요 없고 이력에서만 필요하다. | Accepted for MVP, 예외는 Needs Decision |
| Q4 파일명 포맷 | 보기 편하게 `애칭_보험사_진단명_날짜` 구조를 원한다. | 표시명은 Accepted for MVP, 물리 파일명 사용은 Not Accepted / Risk |
| Q5 원본 파일명 | 로컬 위주이고 학습만 되지 않는다면 찾기 편한 쪽이 좋다. | Accepted with Risk Note |
| Q6 청구 상태 | 청구는 보험사별로 따로 관리해야 하며 결과와 외부심사 상태도 보험사별로 다를 수 있다. | Accepted |

## 5. Accepted Decisions

### Q1. 앱 구현 방식

- 데스크톱 앱 방향으로 진행한다.
- 판정: `Accepted`

단, 아래 항목은 아직 확정하지 않는다.

- WPF
- WinForms
- MAUI
- Avalonia
- Electron
- Web/local runtime
- 실제 app scaffold 생성

기술 스택 선택은 `Needs Technical Design`으로 남긴다. app scaffold 생성은 사용자 별도 승인 전까지 금지한다.

### Q2. 삭제보다 사용 중지 우선

- 연결 데이터가 있는 항목은 물리 삭제보다 사용 중지를 우선한다.
- 기존 보험, 문서, 청구, 이력은 유지한다.
- 판정: `Accepted for MVP`

### Q2. 보험 유지/해지 표시

- 보험은 화면에서 `유지`와 `해지`가 구분되어야 한다.
- 해지 보험은 목록 하단에 표시한다.
- 판정: `Accepted for MVP`

### Q3. 해지 보험의 청구 화면 기본 제외

- 해지 보험은 청구 시 기본 목록에서 제외한다.
- 해지 보험은 이력에서만 표시한다.
- 판정: `Accepted for MVP`

### Q6. 보험사별 청구 진행 분리

- 하나의 진료/청구 준비 단위와 보험사별 청구 진행 단위를 분리한다.
- 같은 진료 건을 여러 보험사에 동시에 청구해도 결과는 보험사별로 따로 관리한다.
- 판정: `Accepted`

권장 모델 방향:

- `ClaimCase`: 하나의 진료/청구 준비 단위
- `ClaimSubmission`: 보험사별 청구 진행 단위
- `ClaimPayment`: 보험사별 지급/부지급/감액 결과

## 6. Accepted with Risk Notes

### Q4. 사용자 친화 표시명

사용자 선호 표시명:

```text
애칭_보험사_진단명_날짜
```

판정:

- 사용자 친화 표시명: `Accepted for MVP`
- 물리 저장 파일명에 그대로 사용: `Not Accepted / Risk`
- `physicalFileName`과 `displayTitle` 분리: `Recommended`

위 구조는 찾기 쉽다는 장점이 있지만, 애칭, 보험사, 진단명, 날짜가 결합되면 민감정보 단서가 될 수 있다.

권장 분리:

```text
physicalFileName:
- 내부ID_날짜_문서유형

displayTitle:
- 애칭_보험사_진단명_날짜
```

### Q5. 원본 파일명 local-only metadata 후보

사용자 의도:

- 로컬 위주라면 찾기 쉬운 쪽이 좋다.
- 학습에 사용되지 않는다면 원본 정보가 있어도 괜찮을 수 있다고 본다.

판정:

- local-only `originalFileName` metadata 후보: `Accepted with Risk Note`
- 외부 전송 또는 학습 사용: `Forbidden`
- 기본 표시명으로 원본 파일명 사용: `Not Recommended`

주의:

- 학습 여부와 로컬 파일명 노출 위험은 별개다.
- 로컬 파일명도 탐색기, 백업, 검색 인덱스, 화면 공유, 압축 파일에서 노출될 수 있다.
- 원본 파일명 raw 보존은 `Needs User Approval`로 유지한다.

## 7. Not Accepted as Physical Model

아래 항목은 사용자 선호가 있더라도 물리 모델 또는 저장 구조로 그대로 확정하지 않는다.

- `애칭_보험사_진단명_날짜`를 물리 저장 파일명으로 그대로 사용
- 원본 파일명을 기본 화면 표시명으로 사용
- 원본 파일명을 외부 전송, OCR 학습, 운영 API 전송에 사용
- `사용중지`와 `해지`를 같은 상태 필드로 합치기
- `ClaimCase`, `ClaimSubmission`, `ClaimPayment` 완료 상태를 하나로 합치기
- 데스크톱 앱 방향을 특정 기술 스택 확정으로 해석하기

## 8. Still Needs Decision

| 항목 | 상태 | 이유 |
|---|---|---|
| 데스크톱 앱 기술 스택 | `Needs Technical Design` | WPF, WinForms, MAUI, Avalonia, Electron 중 선택 근거가 아직 없음 |
| app scaffold 생성 승인 | `Needs User Approval` | 현재 문서는 결정 기록이며 scaffold 생성 지시가 아님 |
| 정확한 상태값 명칭과 DB 필드명 | `Needs Technical Design` | `Policy.appStatus`, `Policy.contractStatus` 후보는 있으나 물리 필드명은 미확정 |
| 해지 보험의 보장기간 기반 예외 청구 | `Needs Decision` | 해지 보험이라도 진료일이 보장기간 안이면 청구 가능성이 있을 수 있음 |
| 원본 파일명 raw 보존 | `Needs User Approval` | 찾기 편의와 민감정보 노출 위험이 충돌함 |
| 외부심사 상태 세부 | `Needs Technical Design` | 보험사별 진행 상태 후보는 있으나 화면/DB 반영 범위는 미확정 |

## 9. App Scaffold Impact

- app scaffold를 생성하기 전 기본 방향은 `데스크톱 앱`이다.
- 기술 스택은 아직 확정하지 않는다.
- 실제 app scaffold 생성은 사용자 별도 승인 전까지 금지한다.
- scaffold 전 최소 결정 필요 항목은 다음과 같다.
  - 데스크톱 기술 스택
  - 로컬 파일 저장 방식
  - 민감정보 파일명 표시 기준
  - 보험 유지/해지와 사용중지 상태의 화면 기준

## 10. Data Model Impact

### Policy 상태 분리 후보

`사용중지`는 앱 관리 상태이고, `해지`는 보험 계약 상태다. 두 상태는 같은 필드로 합치지 않는다.

권장 모델 후보:

```text
Policy.appStatus:
- active
- disabled
- delete_requested 후보

Policy.contractStatus:
- maintained
- terminated
- on_hold 후보
```

판정:

- 사용 중지 우선: `Accepted for MVP`
- 보험 유지/해지 표시: `Accepted for MVP`
- 정확한 상태값 명칭과 DB 필드명: `Needs Technical Design`

### Claim 상태 분리 후보

권장 모델:

```text
ClaimCase:
- 하나의 진료/청구 준비 단위

ClaimSubmission:
- 보험사별 청구 진행 단위

ClaimPayment:
- 보험사별 지급/부지급/감액 결과
```

추가 상태 후보:

- `external_review_requested`
- `additional_documents_requested`
- `reviewing`
- `paid`
- `denied`
- `partially_paid`
- `cancelled`

판정:

- 보험사별 청구 진행 분리: `Accepted`
- 외부심사 상태 후보 추가 검토: `Needs Technical Design`

## 11. UI / Screen Impact

보험 관리 화면:

- 유지 보험은 상단에 표시한다.
- 해지 보험은 하단에 표시한다.
- 사용중지 상태와 해지 상태를 별도 의미로 보여준다.

청구 화면:

- 기본 보험 후보 목록은 유지 보험만 표시한다.
- 해지 보험은 기본적으로 숨긴다.
- 보장기간 기반 예외 노출은 `Needs Decision`으로 남긴다.

이력 화면:

- 유지 보험과 해지 보험을 모두 표시한다.
- 과거 청구 결과는 보험사별 `ClaimSubmission` / `ClaimPayment` 기준으로 보여준다.

문서 표시:

- 화면 기본 표시명은 `displayTitle`을 사용한다.
- 물리 파일명은 내부 식별자 중심으로 둔다.
- 원본 파일명은 local-only metadata 후보로만 둔다.

## 12. Security / Sensitive Data Impact

- 실제 개인정보, 실제 가족 실명, 실제 보험사명, 실제 병원명, 실제 진단코드 기반 개인 사례는 예시로 사용하지 않는다.
- 사용자 친화 표시명은 민감정보 단서가 될 수 있으므로 물리 저장 파일명과 분리한다.
- 원본 파일명은 로컬이어도 노출될 수 있으므로 기본 표시명으로 사용하지 않는다.
- 원본 파일명을 보존하더라도 외부 전송, OCR 학습, 운영 API 전송은 금지한다.
- `attachments/`와 `data/local/` 내부 파일 생성은 이 문서 작업 범위가 아니다.

## 13. Recommendation

1. 다음 단계에서는 데스크톱 기술 스택 후보를 비교하는 기술 설계 문서를 먼저 작성한다.
2. DB 설계 전에는 `Policy.appStatus`와 `Policy.contractStatus`의 정확한 상태명과 전이 규칙을 정리한다.
3. 파일 저장 설계 전에는 `physicalFileName`, `displayTitle`, `originalFileName`의 저장 위치와 노출 범위를 분리한다.
4. 청구 진행 설계 전에는 `ClaimSubmission` 상태 후보와 `ClaimPayment` 결과 상태 후보를 화면 메시지와 함께 정리한다.

## 14. Next Step

- `Q1_Q6_DECISIONS_RECORDED`를 기준으로 app scaffold 전 기술 설계 질문을 별도 문서로 분리한다.
- 기술 스택 선택 전까지 app scaffold는 생성하지 않는다.
- DB 설계 문서를 작성할 때 이 문서의 Accepted / Risk / Needs Decision 분리를 그대로 참조한다.

## Result

Q1_Q6_DECISIONS_RECORDED
