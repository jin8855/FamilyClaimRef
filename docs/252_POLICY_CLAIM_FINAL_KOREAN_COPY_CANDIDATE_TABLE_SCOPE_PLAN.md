# Policy Claim Final Korean Copy Candidate Table Scope Plan

## A. Status

```text
FINAL_KOREAN_COPY_CANDIDATE_TABLE_PLAN_ONLY
POLICY_CLAIM_FINAL_KOREAN_COPY_CANDIDATE_TABLE_SCOPE_PLANNED
```

## B. Baseline

```text
01aeffe docs(familyclaimref): plan final korean copy strategy
```

## C. Purpose

FamilyClaimRef `Ui.*` resource baseline 56개 중 product-facing candidate와 infrastructure/app-shell review 대상에 대한 Korean copy candidate table을 만든다.

이 문서는 copy 후보 제안 문서다. 승인된 final copy table이 아니며, 구현 문서도 아니고, resource value 변경 문서도 아니다.

## D. Current Baseline

- static XAML resource keys: 32
- ViewModel runtime message keys: 24
- total `Ui.*` resource keys: 56
- verified `UiStrings.xaml` `Ui.*` keys: 56
- verified `UiTextKeys.cs` `Ui.*` constants: 56

## E. Candidate Table Scope

포함 대상:

- product-facing candidate rows
- infrastructure/app-shell review rows
- existing Korean source literal review rows
- English current value requiring later Korean copy decision

제외 대상:

- validation-harness-only rows
- dev-harness-only rows
- deferred/non-resource rows
- product UI shell future-only rows
- DB/SQLite/OCR/repository message rows

## F. Allowed In This Batch

- docs 안에서 candidate Korean copy proposal 작성
- candidate status를 `pending user approval`로 기록
- current resource value와 ownership 기준을 검토 가능한 형태로 정리
- final implementation 전에 필요한 approval review guide 작성

## G. Forbidden In This Batch

- resource value 변경
- final copy approved 처리
- `UiStrings.xaml` 수정
- `UiTextKeys.cs` 수정
- ViewModel/test 수정
- direct Korean replacement
- culture switching 구현
- dynamic language switching 구현
- DB/SQLite/OCR/repository 구현
- app launch
- OpenFileDialog 실행
- workflow 실행
- cleanup 실행
- `data/claimdoc` 접근
- git add/stage/commit

## H. Count Expectation

| Count item | Expected | Actual inspection result | Notes |
|---|---:|---:|---|
| Candidate/review rows | 31 | 31 | product-facing candidate 30 + infrastructure/app-shell 1 |
| Excluded resource rows | 25 | 25 | validation-harness-only 23 + dev-harness-only 2 |
| Deferred/non-resource rows | 8 | 8 | deferred display/diagnostic/future-only items |
| Total `Ui.*` resource rows | 56 | 56 | `UiStrings.xaml` and `UiTextKeys.cs` counts match |

## I. Count Discrepancy Handling

docs/248 또는 source read-only inspection 결과가 위 count와 다르면 임의로 보정하지 않는다. 해당 차이는 candidate table 문서의 discrepancy 항목에 기록하고 별도 결정으로 넘긴다.

현재 inspection 결과:

- discrepancy: none

## J. Source Inspection Notes

- `App.xaml`은 `Resources/UiStrings.xaml`을 merge한다.
- `MainWindow.xaml`은 static label/button/header에 `StaticResource Ui.*`를 사용한다.
- `DocumentRegistrationViewModel`은 runtime message를 `IUiTextProvider.Get(...)`으로 조회한다.
- `PolicyClaimManagementViewModel`은 management runtime message를 `IUiTextProvider.Get(...)`으로 조회한다.
- `ResourceUiTextProviderTests`에는 일부 exact current value assertion이 있다.
- ViewModel tests에는 runtime message exact string assertion이 있다.

## K. Output Documents

- `docs/252_POLICY_CLAIM_FINAL_KOREAN_COPY_CANDIDATE_TABLE_SCOPE_PLAN.md`
- `docs/253_POLICY_CLAIM_FINAL_KOREAN_COPY_CANDIDATE_TABLE.md`
- `docs/254_POLICY_CLAIM_FINAL_KOREAN_COPY_CANDIDATE_APPROVAL_REVIEW_GUIDE.md`
- `docs/255_POLICY_CLAIM_FINAL_KOREAN_COPY_CANDIDATE_TABLE_COMMIT_CANDIDATE_REVIEW.md`

## L. Final Marker

```text
POLICY_CLAIM_FINAL_KOREAN_COPY_CANDIDATE_TABLE_SCOPE_READY
```
