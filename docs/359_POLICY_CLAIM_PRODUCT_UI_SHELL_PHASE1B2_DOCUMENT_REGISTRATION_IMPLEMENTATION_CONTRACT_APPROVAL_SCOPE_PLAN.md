# Product UI Shell Phase 1B2 Document Registration Implementation Contract Approval Scope Plan

## A. Status

- Marker: `POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1B2_DOCUMENT_REGISTRATION_IMPLEMENTATION_CONTRACT_APPROVAL_SCOPE_READY`
- Task ID: `POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1B2_DOCUMENT_REGISTRATION_IMPLEMENTATION_CONTRACT_APPROVAL_DOCS_BATCH`
- Work type: documentation-only implementation-contract approval batch
- Implementation target now: 0

## B. Baseline

- Full hash: `817e4ecb80385776c24e26b10e413992a7dcef09`
- Subject: `docs(familyclaimref): plan product shell phase1b2 document registration`
- Initial working tree: clean
- Initial staged files: none
- Known full solution tests: PASS 351/351
- `UiStrings.xaml` `Ui.*` resources: 64
- `UiTextKeys.cs` `Ui.*` constants: 64
- `Ui.Product.*` resources/constants: 8/8
- Prior blockers, source/lifecycle/copy/composition: 0/1/2/1

## C. Purpose

This batch records future implementation contracts without implementing them. It resolves the compile-only composition, lifecycle, static-copy, and runtime-message decisions needed to re-evaluate the Phase 1B2 candidate.

The recommended resolution covers:

- Candidate A direct reuse through `ProductShellViewModel`
- target loading on every registration-view activation
- a sequential repeated-load regression test
- three product-specific static keys
- approved shared static copy reuse
- generic runtime-message reuse
- a target-message compile-only compatibility exception
- an option-display binding contract
- a final future implementation candidate and validation plan

Source implementation still requires a separate exact implementation instruction.

## D. Exact Created Documentation List

- `docs/359_POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1B2_DOCUMENT_REGISTRATION_IMPLEMENTATION_CONTRACT_APPROVAL_SCOPE_PLAN.md`
- `docs/360_POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1B2_DOCUMENT_REGISTRATION_COMPOSITION_LIFECYCLE_APPROVED_DECISION.md`
- `docs/361_POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1B2_DOCUMENT_REGISTRATION_COPY_RUNTIME_MESSAGE_APPROVED_TABLE_AND_FINAL_FILE_LIST.md`
- `docs/362_POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1B2_DOCUMENT_REGISTRATION_IMPLEMENTATION_VALIDATION_TEST_PLAN.md`
- `docs/363_POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1B2_DOCUMENT_REGISTRATION_IMPLEMENTATION_CONTRACT_COMMIT_CANDIDATE_REVIEW.md`

Reserved implementation result document:

- `docs/358_POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1B2_DOCUMENT_REGISTRATION_IMPLEMENTATION_RESULT_REVIEW.md`
- Created now: no

## E. Approval Matrix

| Contract item | Approved for future candidate | Implemented now |
|---|---|---|
| Candidate A composition | yes | no |
| every-activation target loading | yes | no |
| lifecycle regression test | yes | no |
| three product static keys | yes | no |
| shared static copy reuse | yes | no |
| generic runtime-message reuse | yes | no |
| target-message compile-only exception | yes | no |
| final 10-file candidate, audit conditional | yes, conditional | no |
| runtime entry | no | no |
| `AppServices` modification | no | no |

## F. Option-Display Audit Condition

The final candidate is valid only if the future product view does not expose raw target-kind or document-type identifiers.

Read-only source audit result: `PASS_WITH_REQUIRED_BINDING_CONTRACT`.

- The current validation harness displays raw `policy`/`claim` and document-type codes and must not be copied verbatim.
- The future product target-kind selector must separate visible product labels from technical values.
- The future document-type selector must use existing `DocumentTypeSeeds.Label` for display and `DocumentTypeSeeds.Code` for the selected value.
- Policy and claim target selectors may retain `DisplayMemberPath="DisplayTitle"` and `SelectedValuePath="Id"`.
- No additional resource key or production file is required beyond the approved candidate.

## G. Explicit Non-Scope

- source, test, XAML, ViewModel, resource, or project-file modification
- Candidate A implementation
- `ProductDocumentRegistrationView` creation
- resource-key addition
- runtime-message value modification
- `DocumentRegistrationViewModel` modification
- `AppServices` or app startup modification
- ProductShell runtime entry
- build, test, app launch, picker, workflow, or visual execution
- staging, commit, or push

## H. Batch Execution Boundary

- Implementation target now: 0
- Source/test/XAML/ViewModel/resource/project changes: none
- docs/358 created: no
- Build/test: not run
- Stage/commit: not run
- Implementation is not authorized by this documentation batch.
