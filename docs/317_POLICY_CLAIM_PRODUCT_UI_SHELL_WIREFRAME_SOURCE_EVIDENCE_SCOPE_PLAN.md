# Product UI Shell Wireframe Source Evidence Scope Plan

## A. Status

PRODUCT_UI_SHELL_WIREFRAME_SOURCE_EVIDENCE_SCOPE_PLAN_ONLY

## B. Marker

POLICY_CLAIM_PRODUCT_UI_SHELL_WIREFRAME_SOURCE_EVIDENCE_SCOPE_READY

## C. Baseline Commit

`9e40fe5 docs(familyclaimref): plan product ui shell wireframe scope`

## D. Purpose

`docs/314_POLICY_CLAIM_PRODUCT_UI_SHELL_WIREFRAME_SCREEN_FUNCTION_INVENTORY.md`에서 `Unknown / needs source`로 남은 wireframe screen/function 항목의 source evidence를 재검토하고, product target scope와 implementation gate를 분리한다.

이번 문서는 product UI shell 구현 문서가 아니다. WPF/XAML port, `MainWindow` replacement, navigation 구현, ViewModel 추가, resource key 추가, DB/SQLite/repository/OCR/migration 구현을 시작하지 않는다.

## E. Current Baseline

| 항목 | 현재 기준 |
|---|---|
| initial wireframe full scope | final product target scope로 수용 |
| MVP | phased delivery이며 scope deletion이 아님 |
| current `MainWindow` | validation harness 유지 |
| product shell implementation | not approved |
| XAML port implementation | not approved |
| UI redesign implementation | not approved |
| `Ui.Product.*` addition | not approved |
| latest known full test | PASS 331 |
| storage source of truth | JSON retained |
| `data/claimdoc` | protected / no operational use |

## F. Included Scope

- `Unknown / needs source` item evidence search
- source-confirmed / user-scope-confirmed / still-unknown classification
- implementation gate review
- phase impact review
- commit candidate review for docs/317~320

## G. Excluded Scope

- product shell implementation
- WPF/XAML port
- `MainWindow` replacement
- source/resource/test changes
- `Ui.Product.*` addition
- `Ui.BusinessDuplicate.*` addition
- `Ui.ActionResult.*` addition
- DB/SQLite/repository/OCR/migration implementation
- package reference addition
- JSON storage replacement
- `data/claimdoc` access
- cleanup execution
- app launch
- `OpenFileDialog`
- manual workflow
- diagnostic summary extraction
- git add/stage/commit

## H. Evidence Targets

| Target | Previous status | Review purpose |
|---|---|---|
| standalone Document detail | Unknown / needs source | standalone detail screen source가 있는지 확인 |
| Settings | Unknown / needs source | product settings screen source가 있는지 확인 |
| source-confirmed product screens | mixed | final target scope와 implementation gate를 분리 |
| validation harness screens | validation harness only | product screen으로 승격하지 않음 |
| OCR/search dependent items | future-only or later phase | dependency approval 전 구현 금지 유지 |

## I. Scope Judgment

이번 batch는 evidence reconciliation only다.

Source evidence가 부족한 항목은 사용자 발화 기반 final product target principle과 별개로 exact screen implementation target으로 확정하지 않는다. `Unknown / needs source` 또는 `User-scope-confirmed final target, needs source detail` 상태를 유지한다.

Implementation remains blocked. Product shell code batch는 별도 exact-scope 승인과 구현 계획이 필요하다.
