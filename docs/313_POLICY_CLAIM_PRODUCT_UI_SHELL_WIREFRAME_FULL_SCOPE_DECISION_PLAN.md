# Policy Claim Product UI Shell Wireframe Full Scope Decision Plan

## A. Status

PRODUCT_UI_SHELL_WIREFRAME_FULL_SCOPE_DECISION_PLAN_ONLY

## B. Marker

POLICY_CLAIM_PRODUCT_UI_SHELL_WIREFRAME_FULL_SCOPE_DECISION_READY

## C. 기준 Commit

`7d24fb1 docs(familyclaimref): consolidate storage decision track state`

## D. 사용자 결정

처음 wireframe으로 구상한 화면과 기능은 모두 final product target scope에 포함한다.

이 결정은 product scope decision이다. 이번 batch는 구현 승인 문서가 아니며, XAML, ProductShellWindow, MainWindow replacement, navigation, resource key 추가를 수행하지 않는다.

## E. 목적

기존 wireframe 전체 범위를 FamilyClaimRef product UI shell의 final target scope로 고정하고, phased implementation에서 의도치 않게 삭제되지 않도록 관리한다.

## F. 핵심 원칙

- MVP 또는 phased delivery는 final scope 축소가 아니다.
- Phase 1에서 구현하지 않는 화면과 기능은 final backlog로 남긴다.
- WPF/XAML 구조는 final wireframe scope 확장을 막지 않아야 한다.
- validation harness는 product UI shell과 분리해서 보존하는 방향을 우선 검토한다.
- wireframe-to-WPF mapping은 구현 전에 문서화한다.
- final wireframe target scope is recorded as planning only.
- product shell, XAML port, MainWindow replacement, and UI redesign are not approved for implementation.

## G. Current Baseline

| 항목 | 현재 상태 |
|---|---|
| latest known full test | PASS 331 |
| Ui.* key count | 56 |
| approved Korean resource copy | 21 applied |
| storage baseline | JSON source of truth retained |
| product UI shell implementation | not approved |
| UI redesign implementation | not approved |
| DB/SQLite/repository/OCR/migration implementation | not approved |
| `data/claimdoc` | protected / no operational use |
| current MainWindow | validation harness, not product shell |

## H. 포함 후보

- wireframe full screen inventory
- wireframe full function inventory
- MVP phase selection
- later phase backlog
- validation harness vs product shell separation
- WPF screen mapping candidates
- navigation/resource/copy ownership candidates

## I. 제외 범위

- XAML implementation
- product shell implementation
- UI redesign implementation
- MainWindow replacement
- code/test/resource changes
- DB/SQLite/OCR/repository/migration implementation
- cleanup execution
- `data/claimdoc` access

Excluded scope wording:

- implementation excluded
- package reference addition excluded
- data/claimdoc access excluded
- cleanup execution excluded

## J. Source Evidence Summary

| Evidence | 확인 내용 |
|---|---|
| `docs/10_IA_MERMAID.md` | 홈, 보험 청구하기, 보험 검색, 이력 보기, 관리하기 IA와 주요 wireframe file mapping을 기록 |
| `docs/13_SCREEN_REVIEW_CHECKLIST.md` | V5.5 화면 목록과 검토 기준을 기록 |
| `docs/14_WIREFRAME_V2_REVIEW.md` | 초기 wireframe 개선 대상 화면을 기록 |
| `docs/17_WIREFRAME_V5_REVIEW.md` | V5 이후 청구 5단계, 보험 검색, 관리하기 구조를 기록 |
| `docs/22_WIREFRAME_V5_5_ACTION_ALIGNMENT_AND_DETAIL_REVIEW.md` | V5.5 action alignment와 관리/문서함 기준을 기록 |
| `docs/177_POLICY_CLAIM_UI_REDESIGN_DEFER_UNTIL_CORE_VALIDATION_DECISION.md` | 현재 MainWindow를 validation harness로 분류하고 wireframe port를 defer |
| `docs/217_POLICY_CLAIM_UI_PHASE_ENTRY_DECISION.md` | product shell은 별도 단계에서 screen-by-screen으로 진행해야 함을 기록 |

## K. Scope Judgment

- full wireframe scope is accepted as final product target
- final wireframe target scope is accepted as planning only
- implementation remains blocked
- implementation is not approved for implementation
- product shell future candidate only
- explicit user approval required before product shell implementation
- next step after this document is wireframe-to-WPF mapping review or exact commit, not code implementation
