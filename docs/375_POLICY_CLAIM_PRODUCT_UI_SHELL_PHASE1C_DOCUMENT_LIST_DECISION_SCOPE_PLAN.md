# Policy Claim Product UI Shell Phase 1C Document List Decision Scope Plan

## A. Status Marker

`POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1C_DOCUMENT_LIST_DECISION_SCOPE_READY`

## B. Task And Baseline

- Task ID: `POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1C_DOCUMENT_LIST_EXACT_SCOPE_DECISION_DOCS_BATCH`
- Project: `C:\EtcProject\FamilyClaimRef`
- Baseline hash: `488172c516e174cb9d58a2f206492cb3c03b0100`
- Baseline subject: `refactor(familyclaimref): converge registration target terminology`
- Initial working tree: clean
- Initial staged files: none
- Latest known full tests: PASS `358/358`
- Resources/constants: `67/67`
- `Ui.Product.*` resources/constants: `11/11`

## C. Current ProductShell Baseline

- `ProductShellWindow` is compile-only.
- `Home` maps to `ProductHomeView`.
- `DocumentRegistration` maps to `ProductDocumentRegistrationView`.
- `DocumentList` still maps to fallback `DisplayText`.
- `ProductDocumentListView` is absent.
- `ProductDocumentListViewModel` is absent.
- ProductShell runtime entry is absent.
- MainWindow/App startup remains the validation harness.
- The existing product list title and empty-state resources are present.

## D. Decision Goal

The Phase 1C candidate is a basic read-only product document list. This batch determines the source contract, UI projection boundary, load lifecycle, copy requirements, and future exact implementation file list. Phased delivery narrows the first implementation; it does not delete richer document-management scope from later phases.

## E. In Scope

- Existing document storage read contract reconciliation.
- Basic active-document list source selection.
- Dedicated list and item ViewModel necessity decision.
- UI-safe display-field decision.
- Load and repeat-refresh lifecycle decision.
- ProductShell future content mapping decision.
- Copy/resource gap identification.
- Future exact implementation file list candidate.
- Future validation and test gate plan.

## F. Out Of Scope

- Document detail, open, edit, delete, disable, or unlink actions.
- Attachment open or physical-path display.
- Search, filtering UI, sorting UI, pagination, or bulk selection.
- Policy/claim link display or join.
- OCR, DB, SQLite, repository, or migration work.
- Runtime entry, MainWindow replacement, app launch, or manual workflow.
- Any source, test, XAML, ViewModel, resource, project, or solution modification.
- `docs/380_POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1C_DOCUMENT_LIST_IMPLEMENTATION_RESULT_REVIEW.md` creation.

## G. Protected Boundaries

- Protected local document content is not read, listed, searched, selected, or used.
- Nightwork instruction-pack content is not read or searched.
- No cleanup, delete, move, rename, or sample-document creation is permitted.
- No staging, commit, reset, restore, checkout, clean, or push is permitted.

## H. Approval Matrix

| Candidate action | Approved now |
|---|---|
| Create `ProductDocumentListView` | no |
| Create `ProductDocumentListViewModel` | no |
| Create `ProductDocumentListItemViewModel` | no |
| Modify `ProductShellViewModel` | no |
| Modify `ProductShellWindow` | no |
| Modify storage interface or implementation | no |
| Add or change resource values | no |
| Modify `AppServices` | no |
| Add runtime entry | no |
| Approve future exact implementation file list | no |
| Create `docs/380` | no |

Implementation target now: `0`.

## I. Exact Documentation Scope

This batch creates only:

- `docs/375_POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1C_DOCUMENT_LIST_DECISION_SCOPE_PLAN.md`
- `docs/376_POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1C_DOCUMENT_LIST_SOURCE_STORAGE_AND_DATA_MODEL_RECONCILIATION.md`
- `docs/377_POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1C_DOCUMENT_LIST_ARCHITECTURE_COPY_AND_EXACT_FILE_LIST_DECISION_CANDIDATE.md`
- `docs/378_POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1C_DOCUMENT_LIST_VALIDATION_TEST_GATE_PLAN.md`
- `docs/379_POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1C_DOCUMENT_LIST_COMMIT_CANDIDATE_REVIEW.md`

Build/test, staging, and commit are not run in this documentation-only batch.
