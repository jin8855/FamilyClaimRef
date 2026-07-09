# Policy Claim Final Korean Copy Candidate Approval Review Guide

## A. Status

```text
APPROVAL_REVIEW_GUIDE_ONLY
POLICY_CLAIM_FINAL_KOREAN_COPY_CANDIDATE_APPROVAL_REVIEW_GUIDE_PLANNED
```

## B. Purpose

사용자가 `docs/253_POLICY_CLAIM_FINAL_KOREAN_COPY_CANDIDATE_TABLE.md`의 candidate Korean copy를 승인, 수정, 보류할 때 사용할 review 기준을 정의한다.

이 문서는 approval guide이며, 승인 결과 문서가 아니다.

## C. User Decision Values

허용되는 user decision 값:

- Approved
- Revise
- Defer
- Reject
- Keep current

## D. User Review Table Format

| Resource key | Candidate Korean copy | User decision | User replacement copy | Notes |
|---|---|---|---|---|
| `Ui.Example.Key` | `후보 문구` | Approved / Revise / Defer / Reject / Keep current | `수정 문구` | reviewer note |

## E. Approval Rule

- Approved rows만 future implementation 대상이 된다.
- Revise rows는 user replacement copy가 있어야 implementation 대상이 된다.
- Defer rows는 이번 implementation 대상에서 제외한다.
- Reject rows는 이번 implementation 대상에서 제외한다.
- Keep current rows는 resource value 변경 대상에서 제외한다.
- 승인되지 않은 candidate copy는 source/resource에 반영하지 않는다.
- candidate copy는 승인 전까지 final Korean copy가 아니다.

## F. Future Implementation Candidate

Approved copy table이 별도 문서로 생성된 뒤에만 implementation을 검토한다.

구현 범위 후보:

- `UiStrings.xaml` value 변경
- 필요한 exact string test update
- `ResourceUiTextProviderTests` expected value update
- ViewModel runtime message exact assertion update

구현 금지 후보:

- `UiTextKeys.cs` key rename
- MainWindow.xaml literal replacement
- ViewModel literal reintroduction
- ViewModel behavior change
- culture switching
- dynamic language switching
- DB/SQLite/OCR/repository
- `data/claimdoc` 접근

## G. Future Result Review

향후 implementation이 승인되면 다음 중 하나의 result review를 별도 생성한다.

- `docs/251_POLICY_CLAIM_FINAL_KOREAN_COPY_STRATEGY_RESULT_REVIEW.md`
- 또는 사용자가 승인한 별도 번호의 final Korean copy result review

## H. Forbidden

- no direct Korean replacement outside `UiStrings.xaml`
- no XAML literal replacement
- no ViewModel literal reintroduction
- no culture switching
- no dynamic language switching
- no `data/claimdoc`
- no DB/SQLite/OCR/repository
- no actual personal, insurer, hospital, diagnosis, policy number, or claim number samples

## I. Final Marker

```text
POLICY_CLAIM_FINAL_KOREAN_COPY_CANDIDATE_APPROVAL_REVIEW_GUIDE_READY
```
