# Product UI Shell Phase 1B2 Target Runtime Terminology Convergence Scope Plan

## A. Status

- Marker: `POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1B2_TARGET_RUNTIME_TERMINOLOGY_CONVERGENCE_SCOPE_READY`
- Task ID: `POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1B2_TARGET_RUNTIME_MESSAGE_TERMINOLOGY_CONVERGENCE_DECISION_DOCS_BATCH`
- Work type: documentation-only terminology convergence decision
- Implementation target now: 0

## B. Baseline

- Full hash: `e269e7469f0e462e2b00d88ae468a88fa40833a1`
- Subject: `feat(familyclaimref): add compile-only product registration view`
- Initial working tree: clean
- Initial staged files: none
- `Ui.*` resources/constants: 67/67
- `Ui.Product.*` resources/constants: 11/11
- Resource/constant mismatch: 0
- Latest known full solution tests: PASS 357/357
- Compile-only `ProductDocumentRegistrationView`: committed
- ProductShell runtime entry: absent
- `ProductDocumentListView`: absent
- Target-specific runtime terminology blocker: 1

## C. Purpose

This batch records the current values and a future terminology-convergence candidate for six target-specific document-registration runtime messages. It compares shared-value replacement, product-specific key creation, and continued compatibility-exception strategies without implementing any of them.

The decision output must:

1. Preserve the six current source values as evidence.
2. Record six exact recommended candidate values.
3. Keep all implementation targets set to `no`.
4. Define a future exact four-file candidate without approving it.
5. Keep ProductShell runtime entry blocked until a separate value implementation and validation batch completes.

## D. Exact Documentation Scope

- `docs/364_POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1B2_TARGET_RUNTIME_TERMINOLOGY_CONVERGENCE_SCOPE_PLAN.md`
- `docs/365_POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1B2_TARGET_RUNTIME_MESSAGE_CURRENT_VALUE_RECONCILIATION.md`
- `docs/366_POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1B2_TARGET_RUNTIME_MESSAGE_CANDIDATE_VALUE_TABLE_AND_IMPLEMENTATION_PLAN.md`
- `docs/367_POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1B2_TARGET_RUNTIME_MESSAGE_VALIDATION_TEST_PLAN.md`
- `docs/368_POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1B2_TARGET_RUNTIME_TERMINOLOGY_COMMIT_CANDIDATE_REVIEW.md`

Reserved future implementation result document:

- `docs/369_POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1B2_TARGET_RUNTIME_TERMINOLOGY_IMPLEMENTATION_RESULT_REVIEW.md`
- Created now: no

## E. Approval Matrix

| Item | Approved now |
|---|---|
| existing shared value changes | no |
| six recommended values | no |
| new runtime resource keys | no |
| `DocumentRegistrationViewModel` modification | no |
| test modification | no |
| runtime entry | no |
| `ProductDocumentListView` creation | no |
| exact implementation file list | no |

## F. Decision Boundary

- Selected recommendation for future review: Candidate A, update six existing shared values.
- New product runtime keys required: no.
- Production C#, XAML, and ViewModel source modification required: no.
- MainWindow validation harness continues to share the same ViewModel and resources; shared copy convergence does not productize or replace the harness.
- The six recommended values and future four-file list remain candidates, not approvals.
- Runtime entry remains absent and unapproved.

## G. Current Batch Execution Record

- Documentation-only: yes
- Implementation target now: 0
- Source/test/XAML/ViewModel/resource/project changes: none
- Resource value changes: none
- New resource keys/constants: none
- `docs/369` creation: no
- Build: not run
- Targeted tests: not run
- Full tests: not run
- App launch and workflow execution: not run
- Git add/stage/commit/push: not run
