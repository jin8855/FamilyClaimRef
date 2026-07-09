# Policy Claim Remaining Deferred Work Gate Review

Status: DEFERRED_WORK_GATE_REVIEW

Marker:
POLICY_CLAIM_REMAINING_DEFERRED_WORK_GATE_REVIEW_READY

## 1. Purpose

현재 남아 있는 deferred work를 implementation 착수 전 gate 기준으로 정리한다.

이번 문서는 결정 기록이 아니라 gate review다. 아래 항목은 이 문서만으로 구현 승인되지 않는다.

## 2. Gate Table

| Work item | Current status | Required before implementation | Current recommendation |
|---|---|---|---|
| cleanup execution | unapproved/deferred | dry-run result, exact path list, explicit user approval | do not implement automatically |
| deferred diagnostic summary format extraction | unapproved/deferred | final display model decision or explicit diagnostic ownership decision | do not implement automatically |
| DB/SQLite/OCR/repository planning | unapproved/deferred | separate scope decision and implementation plan | do not implement automatically |
| UI redesign | unapproved/deferred | product UI shell decision and wireframe-to-XAML scope approval | do not implement automatically |
| product UI shell | unapproved/deferred | screen structure, navigation, localization ownership, validation scope | do not implement automatically |
| `Ui.BusinessDuplicate.*` | unapproved/deferred | final copy ownership and business duplicate display model decision | do not implement automatically |
| `Ui.Product.*` | unapproved/deferred | product-facing wording table and UI placement decision | do not implement automatically |
| `Ui.ActionResult.*` | unapproved/deferred | action result model, ownership, and final Korean copy approval | do not implement automatically |
| culture/dynamic language switching | unapproved/deferred | culture strategy, runtime switching requirements, test scope | do not implement automatically |

## 3. Gate Notes

- `data/claimdoc/` remains protected and is not a cleanup candidate.
- resource key additions are not approved by this gate review.
- diagnostic summary formats remain `Keep deferred`.
- cleanup dry-run documentation does not authorize deletion.
- DB/SQLite/OCR/repository work remains outside the current implementation surface.

## 4. Next Approval Needed

The next work item should be selected explicitly by the user. No deferred item should move into implementation through a current-state or commit-candidate document alone.
