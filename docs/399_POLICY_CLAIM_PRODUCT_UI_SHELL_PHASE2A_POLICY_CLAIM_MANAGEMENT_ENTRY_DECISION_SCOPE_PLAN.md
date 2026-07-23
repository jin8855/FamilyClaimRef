# Product UI Shell Phase 2A Policy Claim Management Entry Decision Scope Plan

## A. Task And Marker

- Task ID: `POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE2A_POLICY_CLAIM_MANAGEMENT_ENTRY_EXACT_SCOPE_DECISION_DOCS_BATCH`
- Marker: `POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE2A_POLICY_CLAIM_MANAGEMENT_ENTRY_DECISION_SCOPE_READY`
- Status: documentation-only exact-scope decision

## B. Baseline

- Project: `C:\EtcProject\FamilyClaimRef`
- Full hash: `2eddca1d006f1e4657157bb685fa22f387005a22`
- Subject: `docs(familyclaimref): record guarded entry manual smoke`
- Initial working tree: clean
- Initial staged files: none
- Default startup: `MainWindow`
- Guarded ProductShell preview token: `--product-shell-preview`
- Guarded runtime smoke: accepted `PARTIAL`
- Deferred evidence: Home navigation selection state
- Current ProductShell screens: `Home`, `DocumentRegistration`, `DocumentList`
- Current ProductShell navigation count: `3`
- Product policy-management view: absent
- Product claim-management view: absent
- Current policy/claim management surface: `MainWindow` validation harness
- Resources/constants: `68/68`
- `Ui.Product.*` resources/constants: `12/12`
- Latest known full solution tests: PASS `393/393`
- Default ProductShell readiness: no

The test result is carry-forward evidence. Build and tests are not rerun in this documentation-only batch.

## C. Phase 2A Purpose

This batch decides, without implementation:

- whether policy contracts and claim cases need separate product screens;
- whether the existing `PolicyClaimManagementViewModel` can be reused;
- how ProductShell and MainWindow mutable management state remain separated;
- the navigation IDs and order;
- the fresh-root policy-before-claim dependency;
- product display and privacy projection boundaries;
- copy/resource candidates and blockers;
- one future exact implementation candidate;
- future validation gates and default-startup blockers.

Implementation target now: `0`.

## D. In Scope

- Read-only source, test, resource, project, and tracked decision-document inspection.
- Existing policy/claim management behavior inventory.
- Candidate A through G architecture comparison.
- Navigation, DataContext, lifetime, copy, and privacy decisions.
- Future exact file candidate classification.
- Documentation-only validation of `docs/399~403`.

## E. Out Of Scope

- Any source, test, XAML, ViewModel, resource, project, solution, or package change.
- Product management view or wrapper ViewModel creation.
- `PolicyClaimManagementViewModel`, storage, workflow, or model changes.
- ProductShell navigation implementation.
- AppServices or startup changes.
- Build, test, app launch, manual smoke, UI Automation, or workflow execution.
- DB, SQLite, repository, migration, OCR, or cleanup work.
- Protected-path content access.
- Git staging, commit, push, reset, restore, checkout, or clean.

## F. Exact Documentation File List

1. `docs/399_POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE2A_POLICY_CLAIM_MANAGEMENT_ENTRY_DECISION_SCOPE_PLAN.md`
2. `docs/400_POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE2A_POLICY_CLAIM_MANAGEMENT_SOURCE_WORKFLOW_VIEWMODEL_RECONCILIATION.md`
3. `docs/401_POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE2A_POLICY_CLAIM_MANAGEMENT_NAVIGATION_COPY_AND_EXACT_FILE_LIST_DECISION_CANDIDATE.md`
4. `docs/402_POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE2A_POLICY_CLAIM_MANAGEMENT_VALIDATION_TEST_GATE_PLAN.md`
5. `docs/403_POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE2A_POLICY_CLAIM_MANAGEMENT_COMMIT_CANDIDATE_REVIEW.md`

No future implementation-result document number is assigned.

## G. Approval Matrix

| Item | Approved now |
|---|---|
| Policy-contract product view | no |
| Claim-case product view | no |
| Combined management view | no |
| Management wrapper ViewModel | no |
| Existing management ViewModel modification | no |
| ProductShell navigation expansion | no |
| ProductShellViewModel modification | no |
| ProductShellWindow modification | no |
| AppServices modification | no |
| Resource addition/change | no |
| Storage modification | no |
| Default startup change | no |
| Exact implementation file list | no |

## H. Execution Boundary

- Build/test: not run.
- App launch/manual smoke/UI Automation: not run.
- Source/test/runtime/project modification: none.
- Stage/commit/push: not run.
- Default startup remains `MainWindow`.
- Guarded preview remains opt-in only.
