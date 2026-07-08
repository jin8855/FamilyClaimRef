# Policy/Claim Document Registration Negative Test Scope Review

## A. Status

Status: TEST_SCOPE_REVIEW_ONLY

## B. Read-Only Source Findings

### Confirmed

- `DocumentRegistrationWorkflow` rejects null requests.
- `DocumentRegistrationWorkflow` normalizes required policy and claim target ids before attachment copy.
- `DocumentRegistrationWorkflow` attaches a document first, then links it to the policy or claim target.
- `DocumentRegistrationWorkflow` rolls back copied attachment files and disables the transient document metadata record when link creation fails.
- `DocumentAttachmentCoordinator` validates source file path, document scope, document type, display title, and reference date before saving metadata.
- `DocumentAttachmentCoordinator` rejects nonexistent source files before document metadata is saved.
- `DocumentAttachmentCoordinator` delegates document type, document scope, extension, and duplicate index validation to `FileNamePolicyService`.
- `DocumentAttachmentCoordinator` retries filename collisions through duplicate indexes and stops at the configured maximum.
- `DocumentAttachmentCoordinator` deletes a copied file if metadata save fails.
- `DocumentLinkCoordinator` validates required policy id, claim id, document id, and document type values.
- `DocumentLinkCoordinator` rejects missing or disabled policy targets before persisting a policy document link.
- `DocumentLinkCoordinator` rejects missing or disabled claim targets before persisting a claim document link.
- `DocumentLinkCoordinator` rejects duplicate active policy and claim document links.
- `FileNamePolicyService` keeps separate policy and claim document type allowlists.
- `FileNamePolicyService` allows only `pdf`, `jpg`, `jpeg`, and `png` extensions.
- `DocumentRegistrationViewModel` has UI-layer validation for selected source file, target kind, target id, document type, display title, and reference date.
- Existing tests already cover several negative paths in workflow, attachment coordinator, link coordinator, and ViewModel layers.

### Candidate

- Add automated workflow-level tests that assert missing source path and nonexistent source file rejection does not create document metadata, link metadata, or attachment files.
- Add automated workflow-level tests that assert unsupported document type rejection does not create final links.
- Add automated workflow-level tests for disabled policy target and disabled claim target rejection with rollback verification.
- Add automated workflow-level tests for unsupported extension rejection using isolated runtime synthetic files only.
- Add automated tests for filename collision behavior at the attachment coordinator layer, or reuse existing coverage if it already satisfies the implementation batch scope.
- Add ViewModel tests only when the negative case is clearly UI-only, such as target kind mismatch.

### Unknown

- `target kind mismatch` is not a workflow-level concept because the workflow exposes separate policy and claim registration methods.
- Exact product meaning of `duplicate registration` needs decision: same source file physical-name collision, same document linked twice, or same target/type business duplicate.
- Whether unsupported ViewModel-only cases should be included in the first negative validation implementation batch remains a scope decision.
- Whether future negative validation should assert exact exception types or only durable side effects remains a test design decision.
- Whether existing duplicate collision tests are sufficient for this plan or need an end-to-end workflow variant remains a test scope decision.

## C. Risk Review

- Some validations may be ViewModel-only.
- Some validations may currently be missing.
- If missing validation requires production code changes, future implementation must STOP_AND_REPORT or create separate implementation plan.
- Negative tests must not force broad production changes.
- Service-level tests should avoid UI wording and localization expectations.
- Workflow-level negative tests must isolate runtime roots and synthetic inputs from default runtime evidence.

## D. Recommended Strategy

- implement tests for currently supported service-level validations first.
- classify unsupported cases as gaps.
- do not alter UI.
- do not add localization copy.
- keep `DocumentRegistrationWorkflow` behavior distinct from `DocumentRegistrationViewModel` behavior.
- keep rollback and no-side-effect assertions focused on metadata, link records, and copied attachment files.
