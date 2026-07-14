# Policy Claim Product UI Shell Phase 1B2 Target Runtime Terminology Dependency Scope Correction Plan

## A. Status

- Marker: `POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1B2_TARGET_RUNTIME_TERMINOLOGY_DEPENDENCY_SCOPE_CORRECTION_PLAN_READY`
- Task ID: `POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1B2_TARGET_RUNTIME_TERMINOLOGY_DEPENDENCY_SCOPE_CORRECTION_DOCS_BATCH`
- Work type: documentation-only dependency-scope correction
- Implementation target now: 0
- Build/test: not run
- Stage/commit: not run

## B. Baseline

- Full hash: `5740757ec74ee11ba677ef368e4df18ce7fc474c`
- Subject: `docs(familyclaimref): plan phase1b2 target message terminology convergence`
- Initial working tree: clean
- Initial staged files: none
- Reserved implementation result document `docs/369_POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1B2_TARGET_RUNTIME_TERMINOLOGY_IMPLEMENTATION_RESULT_REVIEW.md`: absent

## C. Original Implementation Block

The approved four-file implementation batch stopped during the dependency scan before any file was changed.

- Blocked phase: `dependency scan`
- Original four-file candidate: incomplete
- Additional exact old-value occurrences:
  - `app/FamilyClaimRef.App/Composition/AppServices.cs`
  - `tests/FamilyClaimRef.App.Tests/PolicyClaimManagementViewModelTests.cs`
- Files changed by the blocked batch: none
- `docs/369` created by the blocked batch: no

## D. Correction Scope

This batch performs only the following read-only audit and documentation work:

1. Re-scan all six old values across tracked `app/tests` files.
2. Classify every occurrence by symbol and semantic role.
3. Confirm the `AppServices.cs` occurrence is an executable document-registration fallback entry.
4. Confirm the `PolicyClaimManagementViewModelTests.cs` occurrence is a document-registration provider fixture.
5. Replace the inaccurate future four-file candidate with a revised six-file candidate.
6. Add `PolicyClaimManagementViewModelTests` to future targeted validation commands.
7. Preserve all six Candidate A values without implementing them.

## E. Supersession Boundary

Existing docs/364~368 remain unchanged historical decisions. Docs/370~374 supersede only these statements:

- the future exact implementation candidate contains four files;
- production source-code modification is unnecessary;
- `AppServices.cs` is excluded;
- `PolicyClaimManagementViewModelTests.cs` is excluded;
- the future targeted test command list does not include `PolicyClaimManagementViewModelTests`.

The following decisions remain unchanged:

- Candidate A uses six existing shared resource keys.
- New runtime keys/constants: 0.
- `Ui.*` resources/constants remain 67/67.
- `Ui.Product.*` resources/constants remain 11/11.
- Generic runtime-message changes remain 0.
- ProductShell runtime entry remains unapproved.
- `ProductDocumentListView` remains unapproved and absent.

## F. Exact Documentation Files

- `docs/370_POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1B2_TARGET_RUNTIME_TERMINOLOGY_DEPENDENCY_SCOPE_CORRECTION_PLAN.md`
- `docs/371_POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1B2_TARGET_RUNTIME_TERMINOLOGY_EXTERNAL_DEPENDENCY_RECONCILIATION.md`
- `docs/372_POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1B2_TARGET_RUNTIME_TERMINOLOGY_REVISED_EXACT_FILE_LIST_AND_IMPLEMENTATION_PLAN.md`
- `docs/373_POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1B2_TARGET_RUNTIME_TERMINOLOGY_REVISED_VALIDATION_TEST_PLAN.md`
- `docs/374_POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1B2_TARGET_RUNTIME_TERMINOLOGY_SCOPE_CORRECTION_COMMIT_CANDIDATE_REVIEW.md`

`docs/369` is reserved for a separately approved implementation batch and is not created here.

## G. Approval Matrix

| Item | Approved now |
|---|---|
| revised dependency scope implementation | no |
| `AppServices.cs` value update | no |
| `PolicyClaimManagementViewModelTests.cs` fixture update | no |
| six resource value implementation | no |
| revised six-file exact list implementation | no |
| runtime entry | no |
| `ProductDocumentListView` | no |

## H. Non-Scope

- Source, test, XAML, ViewModel, resource, project, and solution changes: none.
- Build/test, app launch, workflow, screenshot, and cleanup: not run.
- Data, DB, SQLite, repository, OCR, and migration work: none.
- Git add, stage, commit, push, reset, restore, checkout, and clean: not run.

## I. Next Boundary

Stop after docs/370~374. Do not implement the revised candidate until a separate exact implementation instruction is approved.
