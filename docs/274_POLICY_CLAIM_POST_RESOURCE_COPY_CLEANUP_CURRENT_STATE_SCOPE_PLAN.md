# Policy Claim Post Resource Copy Cleanup Current State Scope Plan

Status: CURRENT_STATE_SCOPE_PLAN_ONLY

Marker:
POLICY_CLAIM_POST_RESOURCE_COPY_CLEANUP_CURRENT_STATE_SCOPE_READY

## 1. Baseline

기준 commit:

`46852e6 docs(familyclaimref): plan deferred diagnostic summary format decision`

## 2. Purpose

post resource/copy/cleanup/diagnostic summary decision state를 하나의 current-state 기준으로 정리한다.

이번 문서는 documentation-only current-state 문서다. code/test/XAML/ViewModel/resource 수정, cleanup 실행, diagnostic summary extraction 구현, DB/SQLite/OCR/repository 구현 또는 계획 확정은 수행하지 않는다.

## 3. Included Scope

- resource/copy baseline
- cleanup policy/dry-run baseline
- diagnostic summary deferred decision
- remaining unapproved work

## 4. Excluded Scope

- code/test/resource modification
- cleanup execution
- DB/SQLite/OCR/repository
- UI redesign/product UI shell
- diagnostic summary extraction

## 5. Resource / Copy Baseline

- `UiStrings.xaml` `Ui.*` keys: 56
- `UiTextKeys.cs` `Ui.*` constants: 56
- approved Korean resource copy applied rows: 21
- new/deleted/renamed `Ui.*` keys: 0
- latest known full test: PASS 331

## 6. Cleanup Baseline

- cleanup execution: no
- project root cleanup candidates: none
- root attachments files: 0
- root data/local files: 0
- root runtime_test_document.* files: 0
- DB/SQLite unexpected root files: 0
- `data/claimdoc`: Never cleanup

## 7. Diagnostic Summary Baseline

- `policy:{policyId}; document:{documentId}`: Keep deferred
- `claim:{claimId}; document:{documentId}`: Keep deferred
- diagnostic summary format extraction is not approved.
- final display model is still deferred.

## 8. Remaining Unapproved Work

- cleanup execution
- diagnostic summary extraction implementation
- DB/SQLite/OCR/repository planning
- UI redesign
- product UI shell

## 9. Scope Judgment

The current state is stable for documentation and validation handoff. It does not authorize implementation, resource key changes, cleanup execution, runtime execution, or product UI changes.
