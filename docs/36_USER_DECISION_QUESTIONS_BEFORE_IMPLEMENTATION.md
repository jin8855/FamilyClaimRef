# 36_USER_DECISION_QUESTIONS_BEFORE_IMPLEMENTATION

## 1. Goal

V5.5 기준 문서와 화면 검토 결과를 바탕으로, app scaffold, DB 설계, OCR 설계 전에 사용자가 먼저 결정해야 할 질문을 정리한다.

이 문서는 구현 지시가 아니다. 앱 생성, DB 생성, OCR 구현, HTML/CSS/JS 수정 없이 사용자 승인 필요 항목만 분리한다.

## 2. Checked Files / Paths

- `docs/06_DATA_MODEL.md`
- `docs/23_SCREEN_TO_DATA_MODEL_MAPPING.md`
- `docs/24_DATA_MODEL_GAP_REVIEW.md`
- `docs/33_UI_STATE_AND_CONFIRMATION_MESSAGE_GUIDE.md`
- `docs/34_PRE_IMPLEMENTATION_READINESS_CHECKLIST.md`
- `docs/35_PRE_IMPLEMENTATION_DECISION_MATRIX.md`

## 3. Scope

이 문서는 다음 범위만 다룬다.

- 구현 착수 전 사용자 결정 질문
- 권장 기본 답변 후보
- MVP에서 보류 가능한 항목
- 지금 확정하면 안 되는 항목

이 문서는 다음을 하지 않는다.

- app scaffold 생성
- DB 테이블 구조 확정
- OCR 엔진 또는 라이브러리 선정
- HTML/CSS/JS 수정
- 실제 개인정보 예시 작성
- 기존 문서의 확정 상태 변경

## 4. Question Priority

| 우선순위 | 구분 | 설명 |
|---|---|---|
| P0 | App scaffold 전 필수 | 구현 방식, 삭제/사용 중지, 파일명 마스킹, 청구 완료 기준처럼 화면과 저장 정책에 직접 영향을 주는 질문 |
| P1 | DB 설계 전 필수 | 후보값/확정값, snapshot, projection, 태그, 문서 저장 경계처럼 데이터 구조에 영향을 주는 질문 |
| P2 | OCR 설계 전 필수 | OCR 원문 저장 예외와 OCR 구현 착수 여부처럼 보안 경계와 구현 범위를 결정하는 질문 |
| P3 | MVP 중 결정 가능 | dirty-check 세부 기준, memo 분리처럼 화면 구현 과정에서 더 구체화해도 되는 질문 |

## 5. Questions Before App Scaffold

### Q1. 앱 구현 방식은 지금 확정할 것인가?

선택지:

- A. 아직 결정하지 않고 app scaffold 생성을 보류한다.
- B. Windows 데스크톱 앱 후보로 유지한다.
- C. Web/local runtime 후보로 유지한다.
- D. 사용자가 별도 지정한다.

권장 기본 답변:

- A를 기본값으로 둔다.
- B 또는 C는 후보로만 유지한다.
- 실제 scaffold 생성은 사용자의 별도 승인 전까지 금지한다.

이유:

- 현재 산출물은 개발 전 시각화와 데이터 모델 검토 단계다.
- WPF/WinForms, Web/local runtime, 기타 app 구조는 아직 확정 근거가 부족하다.

### Q2. 연결 데이터가 있는 가족/보험/분류/항목은 물리 삭제 대신 사용 중지를 우선할 것인가?

선택지:

- A. 연결 데이터가 있으면 물리 삭제를 제한하고 사용 중지를 우선한다.
- B. 연결 데이터가 있어도 물리 삭제를 허용한다.
- C. 삭제 요청 상태를 별도로 두고 실제 삭제는 보류한다.
- D. 보류한다.

권장 기본 답변:

- A를 기본값으로 둔다.
- C의 `delete_requested`는 Candidate로 유지한다.
- MVP에서는 연결 데이터가 있는 항목의 물리 삭제를 금지하거나 강하게 제한한다.

이유:

- 가족, 보험, 문서, 청구, 이력은 서로 연결된다.
- 물리 삭제는 이력 무결성과 감사 가능성을 깨뜨릴 수 있다.

### Q3. 사용 중지된 항목은 검색과 선택 목록에 어떻게 노출할 것인가?

선택지:

- A. 신규 선택 목록에서는 숨기고 기존 청구/이력에서는 표시한다.
- B. 모든 화면에서 숨긴다.
- C. 모든 화면에 그대로 표시한다.
- D. 보류한다.

권장 기본 답변:

- A를 기본값으로 둔다.
- 기존 청구/이력에서는 `사용 중지됨` 표시를 Candidate로 둔다.

이유:

- 신규 업무에서는 비활성 항목 선택을 막아야 한다.
- 과거 이력에서는 당시 연결 정보를 보존해야 한다.

### Q4. 파일명 마스킹 세부 포맷은 무엇으로 할 것인가?

선택지:

- A. 내부 ID + 날짜 + 문서유형
- B. 가족표시ID + 날짜 + 문서유형
- C. 청구ID/보험ID + 날짜 + 문서유형
- D. 보류한다.

권장 기본 답변:

- A 또는 C를 후보로 둔다.
- 실제 가족명, 실제 병원명, 실제 보험사명, 전체 번호, 실제 진단코드 기반 개인 사례는 파일명에 포함하지 않는다.

이유:

- 파일명은 OS, 백업, 로그, 검색 결과에 노출될 수 있다.
- 개인정보와 민감 진료 정보가 파일명에 들어가면 통제하기 어렵다.

### Q5. 원본 파일명을 저장할 것인가?

선택지:

- A. 원본 파일명 저장을 금지한다.
- B. 마스킹한 표시명만 저장한다.
- C. 원본 파일명을 별도 보관하되 기본 화면에는 표시하지 않는다.
- D. 보류한다.

권장 기본 답변:

- A 또는 B를 기본 후보로 둔다.
- C는 감사나 복원 요구가 명확해질 때 Needs Decision으로 검토한다.

이유:

- 원본 파일명에는 실제 가족명, 병원명, 보험사명, 진료 내용이 포함될 수 있다.
- 원본 파일명 저장은 민감정보 저장 범위를 넓힌다.

### Q6. 청구 완료 상태를 어떻게 분리할 것인가?

선택지:

- A. `ClaimCase.case_completed`는 청구 준비 완료로 본다.
- B. `ClaimSubmission.submission_completed`는 보험사별 제출/심사 진행 완료로 본다.
- C. `ClaimPayment.paid`는 지급 완료로 본다.
- D. 세 상태를 하나의 완료 상태로 합친다.

권장 기본 답변:

- A, B, C를 함께 사용한다.
- D는 MVP에서도 피한다.

이유:

- 청구 사건 완료, 보험사별 제출 완료, 지급 완료는 업무 시점이 다르다.
- 하나로 합치면 홈, 진행 현황, 이력 보기에서 상태 해석이 흔들릴 수 있다.

## 6. Questions Before DB Design

### Q7. 사용자 확정값은 어디에 저장할 것인가?

선택지:

- A. 업무 객체에만 반영한다.
- B. 업무 객체와 별도 확정값 기록을 함께 둔다.
- C. 후보값/확정값/audit 구조를 별도로 설계한다.
- D. 보류한다.

권장 기본 답변:

- MVP 기본값은 A로 둔다.
- B 또는 C는 DB 설계 단계에서 Candidate로 검토한다.

이유:

- OCR 후보값은 업무 객체에 자동 반영하지 않는 것이 현재 기준이다.
- 확정값 audit이 필요하면 저장 경계가 커지므로 별도 결정이 필요하다.

### Q8. `ClaimReferenceResult` snapshot은 저장할 것인가?

선택지:

- A. 검색 결과 전체 자동 저장을 금지한다.
- B. 사용자가 선택한 참고 결과만 저장한다.
- C. 제출 판단에 사용한 결과만 저장한다.
- D. 검색 결과 전체 snapshot을 저장한다.

권장 기본 답변:

- A를 기본 원칙으로 둔다.
- B 또는 C는 Candidate로 둔다.
- D는 금지 후보로 둔다.

이유:

- 과거 유사 청구 결과에는 민감한 청구 맥락이 포함될 수 있다.
- 전체 snapshot 저장은 보존 범위와 삭제 정책을 복잡하게 만든다.

### Q9. `HistoryItem`은 저장 객체인가, 조회 projection인가?

선택지:

- A. MVP에서는 조회 projection으로 둔다.
- B. 별도 저장 객체로 둔다.
- C. projection으로 시작하고 성능/시점 보존 요구가 생기면 저장 객체로 전환한다.
- D. 보류한다.

권장 기본 답변:

- A 또는 C를 기본값으로 둔다.

이유:

- 이력은 `ClaimCase`, `ClaimSubmission`, `ClaimPayment`, 문서 연결 정보에서 조합 가능하다.
- 별도 저장 객체는 동기화와 정합성 부담을 만든다.

### Q10. `Tag`는 `CategoryItem`과 분리할 것인가?

선택지:

- A. MVP에서는 `CategoryItem` 중심으로 둔다.
- B. `Tag`를 별도 객체로 분리한다.
- C. 검색 랭킹, 동의어, prefix, 태그 통계가 필요해질 때 분리한다.
- D. 보류한다.

권장 기본 답변:

- A 또는 C를 기본값으로 둔다.

이유:

- 현재 분류/태그 관리는 일반 태그 예시 중심이다.
- `Tag`를 별도 객체로 분리하면 검색 기능 확장에는 유리하지만 MVP 복잡도가 증가한다.

### Q11. 물리 DB 테이블 구조를 지금 확정할 것인가?

선택지:

- A. 지금 확정하지 않는다.
- B. 단일 `Document` 물리 저장 후보만 유지한다.
- C. `PolicyDocument` / `ClaimDocument` 물리 분리 후보를 유지한다.
- D. 지금 테이블 구조를 확정한다.

권장 기본 답변:

- A를 기본값으로 둔다.
- B와 C는 Candidate / Needs Decision으로 유지한다.
- D는 현재 단계에서 금지한다.

이유:

- 현재 문서는 데이터 모델과 화면 매핑 기준 문서다.
- 물리 DB 테이블 구조는 구현 방식과 저장소 결정 이후 확정해야 한다.

## 7. Questions Before OCR Design

### Q12. OCR 원문 전체 저장 예외를 허용할 것인가?

선택지:

- A. 기본 저장하지 않고 예외도 두지 않는다.
- B. 기본 저장하지 않되 오류 분석용 임시 저장 후보만 둔다.
- C. 기본 저장하지 않되 사용자 승인 및 마스킹 후 저장 후보만 둔다.
- D. OCR 원문 전체 저장을 허용한다.

권장 기본 답변:

- A 또는 C를 기본 후보로 둔다.
- D는 금지 후보로 둔다.

이유:

- OCR 원문에는 민감정보와 진료 정보가 포함될 수 있다.
- 원문 전체 저장은 보안, 마스킹, 삭제, 접근 통제 기준이 확정된 뒤에만 검토할 수 있다.

### Q13. OCR 구현 방식은 지금 결정할 것인가?

선택지:

- A. 지금 결정하지 않는다.
- B. 로컬 OCR 후보만 유지한다.
- C. OCR 엔진 후보 조사 문서를 별도로 작성한다.
- D. 바로 구현한다.

권장 기본 답변:

- A 또는 C를 기본값으로 둔다.
- D는 현재 단계에서 금지한다.

이유:

- 현재 작업은 개발 전 문서 검토 단계다.
- OCR 구현은 보안, 파일 저장, 원문 보존, 후보값 검토 UI 기준이 먼저 확정되어야 한다.

## 8. Questions Deferrable Until MVP Implementation

### Q14. dirty-check 세부 기준은 지금 확정할 것인가?

선택지:

- A. 지금 확정하지 않고 화면 구현 시 입력 컴포넌트별로 정한다.
- B. 모든 입력 화면에 동일 기준을 적용한다.
- C. 저장/임시저장 화면만 우선 적용한다.
- D. 보류한다.

권장 기본 답변:

- A를 기본값으로 둔다.
- 닫기 확인 메시지 기준은 유지하되, 실제 dirty-check 조건은 MVP 화면 구현 중 정한다.

이유:

- dirty-check는 실제 입력 컴포넌트와 저장 타이밍에 따라 달라진다.
- 정적 와이어프레임 단계에서 과도하게 확정하면 구현 시 불일치가 생길 수 있다.

### Q15. `ClaimMemo` / `HistoryMemo`를 별도 객체로 둘 것인가?

선택지:

- A. MVP에서는 단순 `memo` 필드로 둔다.
- B. 작성자, 작성시각, 변경 이력이 필요하면 별도 객체로 분리한다.
- C. 처음부터 `ClaimMemo` / `HistoryMemo` 별도 객체로 둔다.
- D. 보류한다.

권장 기본 답변:

- A를 기본값으로 둔다.
- B는 추후 확장 후보로 둔다.

이유:

- MVP에서는 메모의 독립 이력보다 청구/이력 조회 흐름이 우선이다.
- 별도 객체화는 감사 요구가 명확할 때 결정하는 편이 안전하다.

## 9. Items That Must Not Be Finalized Now

아래 항목은 지금 확정하지 않는다.

- app scaffold 실제 파일 생성
- DB 테이블 구조
- OCR 엔진 또는 OCR 라이브러리
- OCR 원문 저장 예외의 상세 정책
- `ClaimReferenceResult` 전체 자동 snapshot 저장
- `HistoryItem` 테이블 저장 전환
- `Tag` 별도 객체 분리
- 원본 파일명 raw 보존
- 사용자 확정값 audit 구조
- `PolicyDocument` / `ClaimDocument` 물리 테이블 분리

## 10. Recommended Default Answers

| 질문 | 권장 기본 답변 | 상태 |
|---|---|---|
| Q1 | app scaffold 생성 보류 | Recommended |
| Q2 | 연결 데이터가 있으면 물리 삭제 제한, 사용 중지 우선 | Recommended |
| Q3 | 신규 선택 목록 숨김, 기존 이력 표시 | Recommended |
| Q4 | 내부 ID 또는 청구ID/보험ID 기반 마스킹 파일명 | Candidate |
| Q5 | 원본 파일명 저장 금지 또는 마스킹 표시명만 저장 | Recommended |
| Q6 | `ClaimCase`, `ClaimSubmission`, `ClaimPayment` 완료 분리 | Recommended |
| Q7 | MVP는 업무 객체 반영, audit은 후보 | Candidate |
| Q8 | 전체 자동 snapshot 금지, 선택/사용 결과만 후보 | Recommended |
| Q9 | MVP는 조회 projection | Recommended |
| Q10 | MVP는 `CategoryItem` 중심 | Recommended |
| Q11 | 물리 DB 테이블 구조 미확정 | Recommended |
| Q12 | OCR 원문 전체 저장 금지, 예외는 별도 승인 후보 | Recommended |
| Q13 | OCR 구현 보류 또는 조사 문서만 작성 | Recommended |
| Q14 | dirty-check 세부 기준은 MVP 구현 중 결정 | Deferrable |
| Q15 | MVP는 단순 `memo` 필드 | Deferrable |

## 11. User Answer Template

아래 형식으로 사용자가 답변하면 다음 문서 패치 또는 구현 준비 문서로 이어갈 수 있다.

```text
Q1:
Q2:
Q3:
Q4:
Q5:
Q6:
Q7:
Q8:
Q9:
Q10:
Q11:
Q12:
Q13:
Q14:
Q15:

추가 조건:
-
```

## 12. Risks

- 질문에 답하지 않은 상태로 app scaffold를 만들면 구현 방식이 문서 기준과 충돌할 수 있다.
- 삭제/사용 중지 정책이 불명확하면 과거 이력과 연결 데이터 무결성이 깨질 수 있다.
- 파일명 마스킹 기준이 늦게 정해지면 문서 저장 경로와 표시명이 다시 바뀔 수 있다.
- OCR 원문 저장 예외가 불명확하면 민감정보 저장 범위가 통제되지 않을 수 있다.
- `HistoryItem`, `Tag`, `ClaimReferenceResult`를 조기 확정하면 MVP 범위가 불필요하게 커질 수 있다.

## 13. Recommendation

구현 착수 전에는 P0 질문인 Q1부터 Q6까지 먼저 사용자 답변을 확정한다.

DB 설계 문서를 작성하기 전에는 Q7부터 Q11까지를 Candidate / Needs Decision 상태로 분리해서 검토한다.

OCR 설계는 Q12와 Q13 답변 없이는 시작하지 않는다.

Q14와 Q15는 MVP 구현 중 화면 입력 구조와 감사 요구가 구체화된 뒤 확정해도 된다.

## 14. Next Step

1. 사용자가 Q1부터 Q6까지 먼저 답변한다.
2. 답변 결과를 바탕으로 구현 착수 가능 여부를 다시 판정한다.
3. DB 설계 전 Q7부터 Q11까지 별도 결정 문서로 정리한다.
4. OCR 설계 전 Q12와 Q13을 보안 기준과 함께 확정한다.

## Result

USER_DECISION_QUESTIONS_READY
