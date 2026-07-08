# Policy/Claim UI Phase Entry Decision

## A. Status

Status: DECISION_ONLY

Marker:

POLICY_CLAIM_UI_PHASE_ENTRY_DECISION_RECORDED

This document records the UI phase entry decision only.

No UI implementation is authorized by this document.

No XAML change is authorized by this document.

No ViewModel change is authorized by this document.

No localization implementation is authorized by this document.

No resource file creation is authorized by this document.

No wireframe port is authorized by this document.

## B. Baseline

Record:

- latest commit:
  893311f docs(familyclaimref): close core validation status review

- source docs reviewed:
  - docs/213_POLICY_CLAIM_CORE_VALIDATION_STATUS_CLOSURE_REVIEW.md
  - docs/214_POLICY_CLAIM_REMAINING_PRODUCT_UI_BOUNDARY_DECISION.md
  - docs/215_POLICY_CLAIM_NEXT_WORK_SELECTION_REVIEW.md
  - docs/216_POLICY_CLAIM_CORE_VALIDATION_CLOSURE_COMMIT_CANDIDATE_REVIEW.md

## C. Decision

Record:

- Core validation is now closed for the validated local JSON/runtime/document registration scope.
- UI work may now enter planning phase.
- UI implementation remains blocked.
- XAML redesign remains blocked.
- Korean localization implementation remains blocked.
- Resource extraction implementation remains blocked.
- Wireframe port remains blocked.
- The next approved class of work is planning and inventory only.

## D. UI Phase Entry Rules

Record:

1. Do not start with wireframe port.
2. Do not directly replace English strings with Korean strings.
3. Do not polish the current validation harness as product UI.
4. Do not mix product UI work with DB/SQLite/OCR/repository.
5. Do not introduce business duplicate UX copy before product duplicate policy is decided.
6. Start with UI string inventory and resource extraction architecture.
7. Then decide resource infrastructure.
8. Then implement resource infrastructure after separate approval.
9. Then map wireframes to WPF screens.
10. Then implement product shell screen-by-screen.

## E. Current MainWindow Classification

Record:

- Current MainWindow role:
  validation harness

- Not classified as:
  final product UI

- The validation harness may remain available during product UI development.
- Whether the validation harness remains as a dev-only screen is a later design decision.

## F. Deferred Product Decisions

Record:

- business duplicate warning/rejection UX:
  DEFER
- Korean validation/status copy:
  DEFER
- resource key ownership:
  DEFER until resource plan
- product shell navigation:
  DEFER until wireframe mapping
- DB/SQLite/OCR/repository:
  NOT AUTHORIZED
- real document ingestion:
  NOT AUTHORIZED
- data/claimdoc use:
  NOT AUTHORIZED

## G. Decision Judgment

POLICY_CLAIM_UI_PHASE_ENTRY_READY_FOR_RESOURCE_PLANNING
