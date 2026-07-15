# Product UI Shell Phase 1C Document List Implementation Contract Commit Candidate Review

## A. Status Marker

`POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1C_DOCUMENT_LIST_IMPLEMENTATION_CONTRACT_COMMIT_CANDIDATE_REVIEW_READY`

## B. Baseline

- Full hash: `378714bd574b516eec29335b0ea23cbac1b2923b`.
- Subject: `docs(familyclaimref): plan product shell phase1c document list`.
- Initial working tree: clean.
- Initial staged files: none.
- Latest known full tests: PASS `358/358`.
- Resources/constants: `67/67`.
- `Ui.Product.*`: `11/11`.

## C. Exact Documentation Candidate

- `docs/381_POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1C_DOCUMENT_LIST_IMPLEMENTATION_CONTRACT_APPROVAL_SCOPE_PLAN.md`
- `docs/382_POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1C_DOCUMENT_LIST_COMPOSITION_LIFECYCLE_AND_STATE_APPROVED_DECISION.md`
- `docs/383_POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1C_DOCUMENT_LIST_LOAD_FAILURE_COPY_APPROVED_TABLE_AND_FINAL_FILE_LIST.md`
- `docs/384_POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1C_DOCUMENT_LIST_IMPLEMENTATION_VALIDATION_TEST_PLAN.md`
- `docs/385_POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1C_DOCUMENT_LIST_IMPLEMENTATION_CONTRACT_COMMIT_CANDIDATE_REVIEW.md`

Reserved `docs/380` remains absent.

## D. Approved Future Contracts

- Candidate A dedicated list ViewModel over `IDocumentStorageService.GetDocumentsAsync`.
- Immutable `DisplayTitle`-only item projection with runtime input guards.
- Active-only filtering and source-order preservation.
- Every-activation load with replacement snapshots and no explicit refresh command.
- Exclusive initial/loading/non-empty/empty/error state model.
- Source-load handling for `InvalidOperationException`, `IOException`, and `UnauthorizedAccessException`.
- Projection-only handling for invalid display-title `ArgumentException`.
- Cancellation propagation rather than failure conversion.
- ProductShell list ViewModel injection/property and DocumentList DataTemplate mapping.
- Load-failure key `Ui.Product.DocumentList.LoadFailedMessage` and value `문서 목록을 불러오지 못했습니다.`.
- Final future candidate count: `12` files.
- Implementation target now: `0`.

## E. Blocker Judgment

- Prior source/lifecycle/copy/composition notation: `2/0/1/1`.
- Basic-list compile-only blockers after contract: `0/0/0/0`.
- Deferred richer-field constraints: `2`.
- Retained runtime-composition blocker: `1`.
- Storage modification approved: no.
- `AppServices` modification approved: no.
- Runtime entry approved: no.

## F. Source And Test Audit Judgment

- Storage method signature: confirmed through the interface.
- `DocumentRecord` fields and nullable `DisabledAt`: confirmed; explicit runtime field validation is absent.
- Missing file: confirmed successful empty source.
- Invalid JSON/schema/envelope: confirmed `InvalidOperationException`; existing invalid-JSON/schema/null-items tests provide partial direct coverage.
- File access failure: `File.OpenRead` pass-through supports `IOException` and `UnauthorizedAccessException`; dedicated storage tests are absent and future ViewModel tests use a fake interface dependency.
- Cancellation: token forwarding and absence of a cancellation catch confirm propagation.
- ProductShell constructor/property and XAML mapping conventions: confirmed.
- Resource naming/order and inventory test conventions: confirmed.
- Current targeted test counts: ProductShell `11`, JSON storage `20`, resources `39`.
- Expected future targeted counts: list `18`, ProductShell `13`, JSON storage `20`, resources `40`.
- Expected future full count: `379`, derived from the exact planned additions and not claimed as an executed result.
- Candidate consistency: PASS.

## G. Actual Batch Validation

- Exact scope: PASS, docs/381 through docs/385 only.
- Tracked modified files: `0`.
- Staged files: `0`.
- Untracked files: exact five documentation files.
- `docs/380`: absent.
- Source/test/XAML/ViewModel/resource/storage/project changes: none.
- `git diff --check`: PASS.
- Trailing whitespace: PASS.
- EOF gate: PASS.
- Personal/sample/local-user scan: PASS.
- Protected ignore checks: PASS.
- Build/test: not run, documentation-only implementation-contract batch.
- Final Git status: docs/381 through docs/385 untracked only.

## H. Commit Readiness

Documentation commit readiness: `ready` if final status remains the exact five untracked documents with no tracked or staged changes.

Recommended commit message:

`docs(familyclaimref): approve phase1c document list implementation contract`

## I. Next Boundary

- Stop after the contract documents.
- Do not implement the 12-file candidate.
- Do not add the load-failure resource.
- Do not modify ProductShell or storage.
- Do not add runtime entry.
- Do not create `docs/380`.
- Wait for document review and a separate exact documentation commit instruction.
