# 상태 흐름 Mermaid

## 1. 목적

이 문서는 주요 데이터 객체의 상태 전이를 개발 전 검토용 Mermaid 다이어그램으로 정리한다.

## 2. Document 상태

```mermaid
stateDiagram-v2
    [*] --> Draft
    Draft --> Registered: 문서함 등록
    Registered --> ExtractionPending: 추출 대기
    ExtractionPending --> ExtractionReview: 후보 검토 필요
    ExtractionReview --> Confirmed: 사용자 확인
    ExtractionReview --> NeedsCorrection: 수정 필요
    NeedsCorrection --> ExtractionReview: 재검토
    Confirmed --> Linked: 보험 또는 청구 사건 연결
    Linked --> Archived: 보관
```

## 3. OCR Candidate 상태

```mermaid
stateDiagram-v2
    [*] --> Candidate
    Candidate --> ReviewRequired: 검토 필요
    ReviewRequired --> Edited: 사용자 수정
    ReviewRequired --> Rejected: 후보 제외
    ReviewRequired --> Confirmed: 후보 확정
    Edited --> Confirmed: 수정값 확정
    Confirmed --> Applied: 원본 화면에 반영
    Rejected --> Archived: 제외 기록 보관
```

## 4. ClaimCase 상태

```mermaid
stateDiagram-v2
    [*] --> Draft
    Draft --> ReadyForReview: 문서와 기본 정보 연결
    ReadyForReview --> NeedsMoreInfo: 정보 부족
    NeedsMoreInfo --> ReadyForReview: 추가 입력
    ReadyForReview --> SubmissionPlanned: 청구 예정
    SubmissionPlanned --> Submitted: 보험사별 청구 등록
    Submitted --> ResultPending: 지급 결과 대기
    ResultPending --> Closed: 결과 입력 완료
    Closed --> Reopened: 추가 확인 필요
    Reopened --> ResultPending
```

## 5. ClaimSubmission 상태

```mermaid
stateDiagram-v2
    [*] --> Planned
    Planned --> Submitted: 접수 완료
    Submitted --> InReview: 보험사 심사 중
    InReview --> NeedSupplement: 보완 요청
    NeedSupplement --> InReview: 보완 제출
    InReview --> PaymentDecided: 지급 결정
    InReview --> Rejected: 미지급 결정
    PaymentDecided --> Closed: 지급 결과 기록
    Rejected --> Closed: 미지급 사유 기록
```

## 6. ClaimPayment 상태

```mermaid
stateDiagram-v2
    [*] --> Unknown
    Unknown --> Expected: 지급 예상 메모
    Unknown --> Paid: 지급 입력
    Unknown --> PartiallyPaid: 일부 지급 입력
    Unknown --> NotPaid: 미지급 입력
    Expected --> Paid
    Expected --> PartiallyPaid
    Expected --> NotPaid
    Paid --> Confirmed
    PartiallyPaid --> Confirmed
    NotPaid --> Confirmed
```

## 7. 상태 설계 원칙

- 자동 추출 결과는 `Confirmed`가 아니라 검토 대상 상태에서 시작한다.
- 사용자가 확인하기 전에는 보험 목록, 청구 사건, 지급 결과에 확정값으로 반영하지 않는다.
- `NeedsCorrection`, `NeedsMoreInfo`, `NeedSupplement` 상태를 통해 보류와 재검토를 명시한다.
- 지급 관련 상태는 청구 가능성 확정이 아니라 입력된 결과 기록으로만 해석한다.
