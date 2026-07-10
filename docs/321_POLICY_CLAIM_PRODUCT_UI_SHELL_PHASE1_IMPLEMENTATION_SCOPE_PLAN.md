# Product UI Shell Phase 1 Implementation Scope Plan

## A. Status

PRODUCT_UI_SHELL_PHASE1_IMPLEMENTATION_SCOPE_PLAN_ONLY

## B. Marker

POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1_IMPLEMENTATION_SCOPE_READY

## C. Baseline Commit

`1e487c1 docs(familyclaimref): reconcile product ui shell wireframe evidence`

## D. Purpose

Phase 1 ProductShell implementation의 future exact-scope 후보를 구현 없이 정리한다.

이번 문서는 ProductShell implementation 문서가 아니다. `ProductShellWindow`, product navigation, product view, XAML port, resource key, ViewModel, test, project file을 생성하거나 수정하지 않는다.

## E. Current Baseline

| 항목 | 현재 기준 |
|---|---|
| initial wireframe full scope | final product target scope로 수용 |
| MVP | phased delivery이며 scope deletion이 아님 |
| source-confirmed final target count | 8 |
| standalone Document detail | User-scope-confirmed final target, needs source detail |
| Settings | User-scope-confirmed final target, needs source detail |
| current `MainWindow` | validation harness 유지 |
| ProductShell implementation | not approved |
| `Ui.Product.*` addition | not approved |
| latest known full test | PASS 331 |
| storage source of truth | JSON retained |
| `data/claimdoc` | protected / no operational use |

## F. Phase 1 Source-Confirmed/Core Candidate Scope

| Screen candidate | Phase 1 candidate status | Reason |
|---|---|---|
| Product navigation shell | Phase 1 candidate | source-confirmed final target이며 이후 screen 확장을 담을 shell 후보 |
| Home / dashboard | Phase 1 candidate | source-confirmed final target이며 product first screen 후보 |
| Document registration product view | Phase 1 candidate | existing document registration workflow와 current harness 기능을 product flow로 재배치할 후보 |
| Document list view | Phase 1 candidate | source-confirmed final target이며 document management entry 후보 |

## G. Phase 1 Candidate Functions

- basic product navigation
- register document
- link document to policy/claim
- edit document metadata
- basic document list display

## H. Explicitly Excluded From Phase 1

- standalone Document detail
- Settings
- Policy contract list/detail
- Claim case list/detail
- Claim preparation checklist
- OCR candidate review
- advanced search/filter
- DB/SQLite/repository/OCR/migration work
- UI redesign/polish/culture switching
- `MainWindow` replacement
- `Ui.Product.*` addition
- product terminology finalization

## I. Source/Test Read-Only Inspection Summary

| Area | Read-only observation | Planning implication |
|---|---|---|
| `MainWindow` | current app startup composes `MainWindow` with `MainWindowViewModel` | keep as validation harness |
| `DocumentRegistrationViewModel` | existing ViewModel is composed through `AppServices` and used by `MainWindowViewModel` | possible reuse or wrapper decision remains future planning |
| `DocumentRegistrationWorkflow` | existing workflow is the document registration boundary | future product view should not bypass workflow |
| `PolicyClaimManagementViewModel` | existing management ViewModel is present for validation harness management | do not productize as Phase 1 shell without separate copy/screen plan |
| `IFilePickerService` | file picker abstraction exists | future product view may reuse boundary if approved |
| `ProductShell` | tracked hits are planning docs only | no ProductShell implementation exists in current app |
| `Ui.Product.*` | tracked hits are planning/resource decision docs only | no `Ui.Product.*` implementation now |
| navigation | prior docs record navigation candidate; app implementation is not present as product shell navigation | navigation remains future implementation candidate |

## J. Scope Judgment

This is planning only.

Implementation remains blocked. A future Phase 1 implementation requires a separate exact-file-list implementation batch and explicit approval.
