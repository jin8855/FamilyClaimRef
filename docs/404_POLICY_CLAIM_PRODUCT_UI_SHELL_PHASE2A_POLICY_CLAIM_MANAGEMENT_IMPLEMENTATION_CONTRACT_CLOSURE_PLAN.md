# Product UI Shell Phase 2A Policy Claim Management Implementation Contract Closure Plan

## A. Marker

`POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE2A_POLICY_CLAIM_MANAGEMENT_IMPLEMENTATION_CONTRACT_CLOSURE_PLAN_READY`

## B. Task

- Repository: `C:\EtcProject\FamilyClaimRef`
- Batch: `POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE2A_POLICY_CLAIM_MANAGEMENT_IMPLEMENTATION_CONTRACT_CLOSURE_DOCS_BATCH`
- Purpose: close the remaining source, lifecycle, behavior, copy/resource, and composition blockers before a separate implementation batch.
- This batch is documentation-only after the exact commit of `docs/399~403`.

## C. Initial Baseline And Documentation Commit

Initial baseline:

- Commit: `2eddca1d006f1e4657157bb685fa22f387005a22`
- Subject: `docs(familyclaimref): record guarded entry manual smoke`
- Working tree: exactly `docs/399~403` untracked
- Tracked modified files: `0`
- Staged files: `0`

Exact documentation commit completed:

- Commit: `eda6fdba20fb713035b51b7a960442f119192a1b`
- Subject: `docs(familyclaimref): plan product shell phase2a policy claim management`
- Committed files: `5`

Committed exact file list:

1. `docs/399_POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE2A_POLICY_CLAIM_MANAGEMENT_ENTRY_DECISION_SCOPE_PLAN.md`
2. `docs/400_POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE2A_POLICY_CLAIM_MANAGEMENT_SOURCE_WORKFLOW_VIEWMODEL_RECONCILIATION.md`
3. `docs/401_POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE2A_POLICY_CLAIM_MANAGEMENT_NAVIGATION_COPY_AND_EXACT_FILE_LIST_DECISION_CANDIDATE.md`
4. `docs/402_POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE2A_POLICY_CLAIM_MANAGEMENT_VALIDATION_TEST_GATE_PLAN.md`
5. `docs/403_POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE2A_POLICY_CLAIM_MANAGEMENT_COMMIT_CANDIDATE_REVIEW.md`

The commit did not include source, test, XAML, resource, project, runtime, or local data changes.

## D. Closure Scope

This closure resolves:

1. Product management architecture selection among B1, B2, and C.
2. State, input, selection, message, and repeated-`Loaded` lifecycle.
3. Storage/runtime exception ownership and product-safe error presentation.
4. Active display-title duplicate behavior.
5. Product copy and resource values.
6. Registration target refresh after policy/claim changes.
7. Exact future implementation file list.
8. Automated validation contract.
9. Separation between Phase 2A implementation approval and default-startup approval.

## E. Source Facts Used

- `PolicyClaimManagementViewModel` owns both policy and claim collections, inputs, selections, and one `ManagementMessage`.
- Its current load/create/disable paths allow storage exceptions to propagate.
- MainWindow code-behind uses `async void` event handlers and does not provide a general storage-error boundary.
- ProductShell currently owns only document registration and document list children.
- MainWindow and ProductShell registration ViewModels are already separate instances.
- `ProductDocumentRegistrationView.Loaded` invokes `LoadTargetOptionsAsync`.
- `LoadTargetOptionsAsync` replaces active policy/claim collections and repairs only invalid selections.
- Policy and claim selectors display `DisplayTitle` and keep `Id` as the hidden selection value.
- JSON storage owns identity, timestamps, active-only projection, and policy/claim relationship validation.
- JSON storage does not define display-title uniqueness.
- Current resource/constant baseline is `68/68`; current `Ui.Product.*` baseline is `12/12`.
- Latest known full solution test result is carry-forward PASS `393/393`.

## F. Selected Direction

Selected architecture: `B2`.

- Keep one ProductShell-only `PolicyClaimManagementViewModel`.
- Share that one instance between the policy and claim product views.
- Keep the MainWindow management ViewModel as a different instance.
- Minimally strengthen the existing management ViewModel with:
  - serialized asynchronous operations;
  - safe load/create/disable error boundaries;
  - explicit message reset support;
  - active display-title duplicate validation;
  - refresh semantics that do not append duplicate rows.
- Do not add wrapper ViewModels.
- Do not modify storage interfaces, JSON storage, records, or models.

## G. Documents Created By This Closure

1. `docs/404_POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE2A_POLICY_CLAIM_MANAGEMENT_IMPLEMENTATION_CONTRACT_CLOSURE_PLAN.md`
2. `docs/405_POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE2A_POLICY_CLAIM_MANAGEMENT_STATE_LIFECYCLE_ERROR_BOUNDARY_DECISION.md`
3. `docs/406_POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE2A_POLICY_CLAIM_MANAGEMENT_PRODUCT_COPY_AND_RESOURCE_APPROVAL.md`
4. `docs/407_POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE2A_POLICY_CLAIM_MANAGEMENT_EXACT_IMPLEMENTATION_FILE_LIST_AND_VALIDATION_CONTRACT.md`
5. `docs/408_POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE2A_POLICY_CLAIM_MANAGEMENT_IMPLEMENTATION_GATE_REVIEW.md`

These five files remain untracked in this batch.

## H. Explicit Non-Scope

- No production code, test code, XAML, ViewModel, resource, project, or solution modification.
- No build or test execution.
- No app launch, workflow execution, UI Automation, or screenshot.
- No runtime data creation or cleanup.
- No storage interface/implementation or model change.
- No DB, SQLite, OCR, migration, or repository work.
- No MainWindow behavior or layout change.
- No startup selector or default-startup change.
- No stage, amend, push, reset, restore, checkout, clean, or rebase for `docs/404~408`.

## I. Default-Startup Separation

This closure may open the guarded ProductShell Phase 2A implementation gate. It does not:

- make ProductShell the default startup window;
- remove the MainWindow default;
- remove the guarded preview token;
- claim release or deployment readiness.

The seven default-startup readiness gates remain separate follow-up gates:

1. Phase 2A implementation completion.
2. Build and regression completion.
3. Guarded management smoke completion.
4. Isolated-root create-flow completion.
5. Registration target refresh smoke completion.
6. Navigation and visual evidence completion.
7. Explicit default-startup approval.

## J. Completion Rule

The implementation gate can be opened only when `docs/405~408` prove:

- one selected architecture;
- explicit owners for state, load, mutation, errors, and messages;
- no cross-screen stale management message;
- duplicate-title behavior;
- repeated-load behavior;
- registration refresh behavior;
- approved Korean copy and exact resource impact;
- one exact implementation file list;
- zero Phase 2A source, lifecycle, behavior, copy/resource, and composition blockers.

Implementation does not begin in this batch.
