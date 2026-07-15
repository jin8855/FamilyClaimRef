# Product UI Shell Phase 1C Document List Implementation Contract Approval Scope Plan

## A. Status Marker

`POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1C_DOCUMENT_LIST_IMPLEMENTATION_CONTRACT_APPROVAL_SCOPE_READY`

## B. Task And Baseline

- Task ID: `POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1C_DOCUMENT_LIST_IMPLEMENTATION_CONTRACT_APPROVAL_DOCS_BATCH`
- Baseline hash: `378714bd574b516eec29335b0ea23cbac1b2923b`
- Baseline subject: `docs(familyclaimref): plan product shell phase1c document list`
- Initial working tree: clean.
- Initial staged files: none.
- Latest known full solution tests: PASS `358/358`.
- Current resources/constants: `67/67`.
- Current `Ui.Product.*` resources/constants: `11/11`.
- Prior source/lifecycle/copy/composition blocker notation: `2/0/1/1`.

## C. Approval Purpose

This batch approves a future compile-only implementation contract for the basic product document list. It fixes the composition, public state, privacy projection, load lifecycle, state exclusivity, failure handling, copy, future file list, and validation accounting before any implementation begins.

Approval in this document set does not authorize source implementation. Implementation target now remains `0`.

## D. Retained Decisions

- Architecture: Candidate A, a dedicated list ViewModel over the existing storage interface.
- Source: `IDocumentStorageService.GetDocumentsAsync(CancellationToken cancellationToken = default)`.
- Concrete `JsonDocumentStorageService` dependency: no.
- Storage interface or implementation modification: no.
- JSON source of truth: retained.
- Active filter: `DisabledAt is null`.
- Visible row field: `DisplayTitle` only.
- Source-return order: preserved.
- Lifecycle: reload on every view activation.
- Repeated load: replace, never append.
- Explicit refresh command: none.
- Document type and reference date: excluded.
- Policy/claim link join: excluded.
- Raw ID, physical file name, path, extension, and timestamp display: prohibited.
- `AppServices` modification: no for compile-only scope.
- Runtime entry: no.

## E. Blocker Reclassification

| Classification | Before contract | After this contract | Meaning |
|---|---:|---:|---|
| Basic-list source blocker | 2 | 0 | The existing `DocumentRecord` is sufficient for `DisplayTitle` only. |
| Basic-list lifecycle blocker | 0 | 0 | Every-activation replacement loading is fixed. |
| Basic-list copy blocker | 1 | 0 | The load-failure key and value are approved for a future candidate. |
| Basic-list composition blocker | 1 | 0 | Injection, property, and template mapping are approved as a future contract. |
| Deferred richer-field constraints | 2 | 2 | Document type is absent from `DocumentRecord`; reference date is not persisted. |
| Runtime-readiness blocker | 1 | 1 | ProductShell composition in `AppServices` and startup remains undecided. |

The two richer-field constraints do not block the basic `DisplayTitle`-only compile scope. They remain excluded rather than promoted into implementation. The runtime blocker does not block compile-only work, but it continues to block runtime entry.

## F. Exact Documentation Scope

This batch creates only:

- `docs/381_POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1C_DOCUMENT_LIST_IMPLEMENTATION_CONTRACT_APPROVAL_SCOPE_PLAN.md`
- `docs/382_POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1C_DOCUMENT_LIST_COMPOSITION_LIFECYCLE_AND_STATE_APPROVED_DECISION.md`
- `docs/383_POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1C_DOCUMENT_LIST_LOAD_FAILURE_COPY_APPROVED_TABLE_AND_FINAL_FILE_LIST.md`
- `docs/384_POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1C_DOCUMENT_LIST_IMPLEMENTATION_VALIDATION_TEST_PLAN.md`
- `docs/385_POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1C_DOCUMENT_LIST_IMPLEMENTATION_CONTRACT_COMMIT_CANDIDATE_REVIEW.md`

Reserved and not created:

- `docs/380_POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1C_DOCUMENT_LIST_IMPLEMENTATION_RESULT_REVIEW.md`

## G. Approval Matrix

| Contract item | Approved for future candidate | Implemented now |
|---|---|---|
| Candidate A list ViewModel architecture | yes | no |
| `DisplayTitle`-only item projection | yes | no |
| Active-only filter | yes | no |
| Source-order preservation | yes | no |
| Every-activation replacement load | yes | no |
| Exclusive loading/empty/error states | yes | no |
| Approved load-failure key/value | yes | no |
| ProductShell list injection/property | yes | no |
| Final 12-file candidate, source-audit conditional | yes, conditional | no |
| Storage modification | no | no |
| `AppServices` modification | no | no |
| Runtime entry | no | no |

## H. Explicit Non-Scope

- No source, test, XAML, ViewModel, resource, storage, project, or solution modification.
- No ProductDocumentList type creation.
- No ProductShell modification.
- No resource key or value addition.
- No `docs/380` creation.
- No DB, SQLite, repository, OCR, migration, runtime composition, or runtime entry work.
- No app launch, workflow, screenshot, cleanup, or protected local document access.
- No build or test execution.
- No Git add, stage, commit, reset, restore, checkout, clean, or push.

## I. Batch Execution Record

- `docs/380` created: no.
- Implementation target now: `0`.
- Build/test: not run, documentation-only implementation-contract batch.
- Stage/commit: not run.
