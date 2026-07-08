# Policy/Claim Document Registration ViewModel Validation Plan

## A. Status

Status: TEST_PLAN_ONLY

Marker:

POLICY_CLAIM_DOCUMENT_REGISTRATION_VIEWMODEL_VALIDATION_PLANNED

No code is modified by this document.

No test is implemented by this document.

No XAML/UI redesign is authorized by this document.

## B. Purpose

Record:

- Workflow-level negative validation is covered.
- Attachment duplicate/collision validation is covered.
- Target kind mismatch is a ViewModel concern, not a workflow concern.
- Current MainWindow remains validation harness.
- Product UI redesign and localization remain deferred.
- ViewModel validation can be tested without changing UI layout.

## C. Candidate ViewModel Validation Cases

Plan tests for:

1. no selected source file
2. missing target kind
3. invalid target kind, if representable
4. missing target id for policy target
5. missing target id for claim target
6. missing document type
7. blank display title
8. missing or invalid reference date, if representable
9. policy document type selected for claim target, if ViewModel validates it
10. claim document type selected for policy target, if ViewModel validates it
11. command disabled / workflow not called when validation fails
12. status message set to validation failure, without asserting final product copy if localization is deferred

## D. Assertion Rules

Record:

- Avoid asserting final Korean/English UX copy.
- It is acceptable to assert non-empty validation status or known current validation marker only if stable.
- Prefer asserting:
  - workflow fake was not called
  - selected target state is invalid
  - command cannot execute
  - status category/state indicates validation failure
- Do not create resource files.
- Do not localize messages.

## E. Safety Rules

- no app launch
- no OpenFileDialog
- no XAML change
- no UI redesign
- no real files unless test-owned synthetic temp file is required
- no data/claimdoc
- no real document data
- no DB/SQLite/OCR/repository

## F. Planned Result Review

Future implementation batch must create:

- `docs/212_POLICY_CLAIM_DOCUMENT_REGISTRATION_VIEWMODEL_VALIDATION_RESULT_REVIEW.md`

## G. Planning Judgment

POLICY_CLAIM_DOCUMENT_REGISTRATION_VIEWMODEL_VALIDATION_PLAN_READY
