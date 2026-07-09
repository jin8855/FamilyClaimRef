# Policy Claim Final Korean Copy Approval Decision Scope

## A. Status

```text
FINAL_KOREAN_COPY_APPROVAL_DECISION_DOC_ONLY
POLICY_CLAIM_FINAL_KOREAN_COPY_APPROVAL_DECISION_SCOPE_PLANNED
```

## B. Baseline

```text
1036fba docs(familyclaimref): draft final korean copy candidate table
```

## C. Purpose

`docs/253_POLICY_CLAIM_FINAL_KOREAN_COPY_CANDIDATE_TABLE.md`의 31개 candidate/review row에 대한 approval decision을 문서화한다.

이 문서는 documentation-only approval table batch의 범위 문서다. 구현 문서가 아니며, `UiStrings.xaml` 또는 source/test 파일을 수정하지 않는다.

## D. Batch Character

- documentation-only approval table batch
- approved value table 문서화
- future implementation scope 정리
- no resource value changes
- no implementation performed

## E. Allowed In This Batch

- approved value table 문서화
- future implementation scope 정리
- 승인/유지/보류 count 정리
- expected test impact 정리

## F. Forbidden In This Batch

- `UiStrings.xaml` 수정
- `UiTextKeys.cs` 수정
- test 수정
- ViewModel 수정
- XAML 수정
- resource value 변경
- approved value source 반영
- direct Korean replacement
- implementation 실행
- culture switching 구현
- dynamic language switching 구현
- DB/SQLite/OCR/repository 구현
- app launch
- workflow 실행
- cleanup 실행
- `data/claimdoc` 접근
- git add/stage/commit

## G. Approval Decision Summary

| Decision item | Count |
|---|---:|
| Candidate/review rows | 31 |
| Approved rows | 21 |
| Keep current rows | 10 |
| Revise rows | 0 |
| Defer rows | 0 |
| Reject rows | 0 |
| Excluded resource rows | 25 |
| Deferred/non-resource rows | 8 |
| Implementation target yes | 21 |
| Implementation target no | 10 |

## H. Future Implementation Principle

- Approved rows만 value change candidate다.
- Keep current rows는 implementation 대상에서 제외한다.
- Excluded resource rows는 implementation 대상에서 제외한다.
- Deferred/non-resource rows는 implementation 대상에서 제외한다.
- Resource key names remain unchanged.
- Implementation은 별도 exact-file-list batch에서만 가능하다.
- Implementation 전후로 `ResourceUiTextProviderTests`, `DocumentRegistrationViewModelTests`, `PolicyClaimManagementViewModelTests`, full test suite를 검증한다.

## I. Current Source/Test Inspection Notes

- `UiStrings.xaml` current values are not changed by this batch.
- `UiTextKeys.cs` key names are unchanged by this batch.
- `ResourceUiTextProviderTests` has exact current value assertions for app title and select-file behavior.
- `DocumentRegistrationViewModelTests` has exact string assertions for document registration runtime messages.
- `PolicyClaimManagementViewModelTests` includes management harness exact string assertions and document registration provider values, but approved table implementation excludes management harness values.

## J. Final Marker

```text
POLICY_CLAIM_FINAL_KOREAN_COPY_APPROVAL_DECISION_SCOPE_READY
```
