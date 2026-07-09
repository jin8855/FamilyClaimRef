# Policy Claim Deferred Diagnostic Summary Format Scope Plan

## 1. Status

DIAGNOSTIC_SUMMARY_FORMAT_SCOPE_PLAN_ONLY

## 2. Marker

POLICY_CLAIM_DEFERRED_DIAGNOSTIC_SUMMARY_FORMAT_SCOPE_PLANNED

## 3. Baseline Commit

- `b131255 docs(familyclaimref): add scenario9 cleanup dry-run report`

## 4. 목적

deferred diagnostic summary format 2개의 extraction/display ownership 검토 범위를 문서화한다.

이번 문서는 planning 문서다. source format 변경, resource key 추가, ViewModel 수정, test 수정, final display model 확정, cleanup 실행을 수행하지 않는다.

## 5. Current Deferred Formats

- `policy:{policyId}; document:{documentId}`
- `claim:{claimId}; document:{documentId}`

## 6. Baseline Findings

- ViewModel runtime message extraction은 완료되었으나 diagnostic summary format 2개는 Defer 상태로 유지되었다.
- `policy:{policyId}; document:{documentId}`는 아직 resource key로 추출되지 않았다.
- `claim:{claimId}; document:{documentId}`는 아직 resource key로 추출되지 않았다.
- final display model은 아직 확정되지 않았다.
- final Korean copy는 approved resource copy 21개만 반영했다.
- cleanup dry-run은 완료되었지만 cleanup execution은 미승인 상태다.
- `data/claimdoc/`는 Never cleanup이며 접근 금지 상태다.
- DB/SQLite/OCR/repository implementation은 이번 범위가 아니다.

## 7. 포함 후보

- current format ownership.
- current placeholder contract.
- diagnostic vs product-facing display 판단.
- final display model 필요 여부.
- future resource key extraction 가능 조건.
- test impact.

## 8. 제외 범위

- source format 변경.
- resource key 추가.
- `UiStrings.xaml` 수정.
- `UiTextKeys.cs` 수정.
- ViewModel 수정.
- test 수정.
- final Korean copy.
- direct Korean replacement.
- DB/SQLite/OCR/repository.
- cleanup execution.
- `data/claimdoc` 접근.
- UI redesign/product UI shell.
- app launch.
- OpenFileDialog.
- manual workflow.
- git add/stage/commit.

## 9. Scope Judgment

- current formats remain deferred.
- extraction is not approved by this document.
- future extraction requires final display model or explicit diagnostic ownership decision.
- current strings are better treated as validation-harness diagnostic summaries than product-facing final copy.
- this planning batch records ownership options only.

## 10. Final Marker

POLICY_CLAIM_DEFERRED_DIAGNOSTIC_SUMMARY_FORMAT_SCOPE_READY
