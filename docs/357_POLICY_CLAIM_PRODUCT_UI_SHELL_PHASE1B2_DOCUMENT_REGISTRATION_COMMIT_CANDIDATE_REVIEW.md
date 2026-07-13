# Product UI Shell Phase 1B2 Document Registration Commit Candidate Review

## A. Status

- Marker: `POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1B2_DOCUMENT_REGISTRATION_COMMIT_CANDIDATE_REVIEW_READY`
- Work type: documentation-only decision candidate review
- Documentation commit readiness: ready for a separate exact documentation commit review
- Readiness basis: actual final Git status contains only docs/353~357 as untracked files and all recorded documentation checks passed
- Implementation readiness: blocked

## B. Baseline

- Full hash: `e79b4f8489f7c066abd0025fa856ce16bba8a6f5`
- Subject: `feat(familyclaimref): add title-only home content host`
- Initial working tree: clean
- Initial staged files: none
- Known full tests: PASS 351/351
- Resources/constants: 64/64
- `Ui.Product.*` resources/constants: 8/8
- ProductHomeView: committed, title-only
- ProductDocumentRegistrationView: absent
- ProductDocumentListView: absent
- ProductShell runtime entry: absent
- MainWindow/App startup: unchanged validation harness

## C. Exact Documentation Candidate

- `docs/353_POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1B2_DOCUMENT_REGISTRATION_STATEFUL_CONTENT_DECISION_SCOPE_PLAN.md`
- `docs/354_POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1B2_DOCUMENT_REGISTRATION_VIEWMODEL_COMPOSITION_RECONCILIATION.md`
- `docs/355_POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1B2_DOCUMENT_REGISTRATION_COPY_RESOURCE_AND_EXACT_FILE_LIST_DECISION_CANDIDATE.md`
- `docs/356_POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1B2_DOCUMENT_REGISTRATION_VALIDATION_TEST_GATE_PLAN.md`
- `docs/357_POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1B2_DOCUMENT_REGISTRATION_COMMIT_CANDIDATE_REVIEW.md`

`docs/358_POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1B2_DOCUMENT_REGISTRATION_IMPLEMENTATION_RESULT_REVIEW.md` is reserved for a future implementation result and is not created in this batch.

## D. Decision Summary

| Decision | Actual judgment |
|---|---|
| Selected architecture | Candidate A, conditional direct reuse through `ProductShellViewModel` |
| Existing registration ViewModel reuse | yes, conditionally |
| Product wrapper required | no |
| ProductShellViewModel modification candidate | yes |
| ProductShellWindow.xaml modification candidate | yes |
| ProductShellWindow.xaml.cs modification candidate | no |
| Product view code-behind candidate | yes, forwarding only |
| DocumentRegistrationViewModel modification candidate | no under Candidate A |
| MainWindowViewModel reuse | no |
| AppServices modification candidate | no for compile-only scope |
| Runtime entry | remains absent |
| Additional product resources | yes, three conditional key candidates |
| LastRegistrationSummary product display | no; diagnostic formats Keep deferred |

## E. Interaction and Lifecycle Judgment

- Select file and register actions should forward to the existing ViewModel methods.
- File picker and workflow boundaries must be reused.
- Target options require an explicit activation lifecycle.
- Repeated page activation semantics are not defined by current tests or navigation infrastructure.
- Lifecycle approval and a matching regression test are required before implementation.

## F. Conditional Implementation Candidate Summary

- Production create: 2
- Production modify: 4
- Test create: 0
- Test modify: 2
- Result document: 1
- Total conditional candidate files: 9
- Source blockers: 0
- Lifecycle blockers: 1
- Copy/resource blockers: 2
- Composition blockers: 1
- Implementation target now: 0
- Exact implementation file list approved now: no
- New resource keys approved now: no
- Runtime entry approved now: no

The conditional exact list is recorded in docs/355. It must be revalidated after the copy/runtime-message, lifecycle, and composition decisions.

## G. Required Approvals Before Implementation

1. Approve Candidate A ownership: `ProductShellViewModel` receives and exposes the existing registration ViewModel.
2. Approve target option load timing and re-entry behavior.
3. Approve three product-specific static resource keys.
4. Approve reuse of shared static registration copy.
5. Decide target-specific runtime message treatment where existing wording conflicts with ProductShell terminology.
6. Reapprove the final exact implementation file list after items 1~5.

## H. Validation Record

| Validation | Actual result |
|---|---|
| Baseline hash/subject | PASS |
| Initial working tree/staged state | PASS; clean/none |
| Exact documentation file set | PASS; docs/353~357 only |
| ProductShell/Home baseline | PASS |
| Registration ViewModel/workflow/picker/storage evidence | PASS |
| ProductDocumentRegistrationView absence | PASS |
| ProductDocumentListView absence | PASS |
| ProductShell runtime-entry absence | PASS |
| Resources/constants | PASS; 64/64 and 8/8 |
| Selected architecture | Candidate A, conditional direct reuse |
| Source blockers | 0 |
| Lifecycle blockers | 1 |
| Copy/resource blockers | 2 |
| Composition blockers | 1 |
| Conditional implementation candidate files | 9 |
| Candidate architecture consistency | PASS; docs/354~357 agree on Candidate A conditional |
| Copy/resource consistency | PASS; conflicts are blockers, not approvals |
| Implementation target now | PASS; 0 |
| Source/test/XAML/ViewModel/resource/project changes | none |
| Marker/content/count scan | PASS |
| `git diff --check` | PASS |
| Trailing whitespace findings | 0 |
| EOF issues | 0 |
| Personal/sample/local-user path findings | 0 |
| `data/claimdoc/` ignore check | PASS |
| `docs/nightwork_20260706/` ignore check | PASS |
| Project root `attachments/` files | 0 |
| Project root `data/local/` files | 0 |
| Project root `runtime_test_document.*` files | 0 |
| Root DB/SQLite unexpected files | 0 |
| Tracked modified files | 0 |
| Staged files | none |
| docs/358 created | no |
| Build/test | not run, documentation-only Phase 1B2 document registration exact-scope decision batch |
| App launch/OpenFileDialog/manual workflow | not run |
| Git add/stage/commit/push | not run |

## I. Final Git Status

```text
?? docs/353_POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1B2_DOCUMENT_REGISTRATION_STATEFUL_CONTENT_DECISION_SCOPE_PLAN.md
?? docs/354_POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1B2_DOCUMENT_REGISTRATION_VIEWMODEL_COMPOSITION_RECONCILIATION.md
?? docs/355_POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1B2_DOCUMENT_REGISTRATION_COPY_RESOURCE_AND_EXACT_FILE_LIST_DECISION_CANDIDATE.md
?? docs/356_POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1B2_DOCUMENT_REGISTRATION_VALIDATION_TEST_GATE_PLAN.md
?? docs/357_POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1B2_DOCUMENT_REGISTRATION_COMMIT_CANDIDATE_REVIEW.md
```

## J. Documentation Commit Readiness and Non-Approval Boundary

- Actual final Git status contains only docs/353~357 as untracked files.
- All recorded documentation validation checks passed.
- docs/353~357 are ready for a separate exact documentation commit review.
- Implementation remains blocked.
- This readiness does not approve Candidate A implementation.
- This readiness does not approve `ProductDocumentRegistrationView` creation.
- This readiness does not approve `ProductShellViewModel` modification.
- This readiness does not approve `ProductShellWindow` modification.
- This readiness does not approve the three resource keys.
- This readiness does not approve the conditional nine-file implementation candidate.
- This readiness does not approve runtime entry.
- A separate exact documentation commit instruction is required.
- Implementation must not start after this batch.

## K. Commit Candidate

Recommended documentation commit message:

`docs(familyclaimref): plan product shell phase1b2 document registration`

This batch does not stage or commit the files.

## L. Final Boundary

- Stop after docs/353~357 validation.
- Do not implement `ProductDocumentRegistrationView`.
- Do not modify ProductShellWindow or ProductShellViewModel.
- Do not modify DocumentRegistrationViewModel.
- Do not add resource keys.
- Do not add runtime entry.
- Do not create docs/358.
- Wait for document review and a separate exact documentation commit instruction.
