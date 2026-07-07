# Policy / Claim Scenario 8B Claim Target Scope Decision

## A. Status Marker

POLICY_CLAIM_SCENARIO8B_CLAIM_TARGET_SCOPE_DECISION_RECORDED

## B. Decision Context

- Scenario 8A policy target registration initially failed with `.txt` due to extension policy.
- Scenario 8A allowed-extension PNG retry succeeded.
- Policy target path confirmed copied attachment, `documents.json`, and `policy-documents.json`.
- Scenario 8B claim target registration remains untested.
- `data/claimdoc` is ignored by exact `.gitignore` rule and remains forbidden for validation.
- Scenario 8A runtime artifacts remain under `%LOCALAPPDATA%`.
- Temp `.txt` and `.png` files remain under `%TEMP%`.
- Cleanup is deferred.

## C. Current Baseline

Current repository baseline:

- latest commit: `5615736 chore(familyclaimref): ignore local claim documents`
- git status before this document: clean
- project root `attachments/`: files=0
- project root `data/local`: files=0
- project root `runtime_test_document.*`: missing
- `data/claimdoc` ignored by `/data/claimdoc/`

Current runtime baseline checked at path level:

- `%LOCALAPPDATA%\FamilyClaimRef`: exists
- `%LOCALAPPDATA%\FamilyClaimRef\data\local\claims.json`: missing
- `%LOCALAPPDATA%\FamilyClaimRef\data\local\claim-documents.json`: missing
- `%TEMP%\FamilyClaimRef\runtime_test_document.txt`: exists
- `%TEMP%\FamilyClaimRef\runtime_test_document.png`: exists

Interpretation:

- Scenario 8A evidence remains.
- Scenario 8B should record a fresh pre-run snapshot before execution.
- `claims.json` and `claim-documents.json` are currently missing from the Scenario 8A policy-only result unless runtime state changes later.
- Runtime state is not clean-room.

## D. Confirmed Decisions

### Decision 1: Scenario 8B Execution Status

Confirmed:

- Scenario 8B has not been executed.
- This document does not launch the app.
- This document does not run OpenFileDialog.
- This document does not create a claim.
- This document does not run document registration workflow.

### Decision 2: Scenario 8B Candidate

Confirmed:

- Scenario 8B is the candidate for claim target document registration success path validation.
- Execution is allowed only after a separate execution instruction and explicit user approval.

### Decision 3: Target Type

Confirmed:

- Scenario 8B is limited to claim target registration only.
- Policy target registration was already confirmed in Scenario 8A and should not be repeated as the primary validation goal.

### Decision 4: Synthetic Data

Allowed synthetic values:

- `policy_title_scenario8b_demo`
- `claim_title_scenario8b_demo`
- `scenario8b_claim_document_png_demo`
- `%TEMP%\FamilyClaimRef\runtime_test_document_claim.png`

Forbidden:

- real family names
- real policy numbers
- real claim numbers
- real insurance company names
- real hospital names
- real diagnosis names or diagnosis codes
- real contract, terms, insurance, hospital, or claim documents
- files under `data/claimdoc`

### Decision 5: Synthetic File

Recommended synthetic file:

```text
%TEMP%\FamilyClaimRef\runtime_test_document_claim.png
```

File rules:

- allowed extension PNG
- temporary path outside project root
- minimal valid PNG binary
- no real document image
- no screenshot
- no actual insurance, medical, contract, hospital, diagnosis, family, or claim data

### Decision 6: Runtime Preparation

Scenario 8B requires:

- active policy creation
- active claim creation under that active policy
- claim target selection
- document registration using claim target

Policy:

- Do not reuse Scenario 8A policy unless a future execution instruction explicitly decides it is safe.
- Prefer a fresh synthetic policy/claim pair for Scenario 8B to separate evidence:
  - `policy_title_scenario8b_demo`
  - `claim_title_scenario8b_demo`
- Active claim creation must occur through the existing policy/claim management UI flow or another separately approved runtime path.

### Decision 7: Expected Artifacts

If Scenario 8B succeeds:

- `policies.json` created or updated
- `claims.json` created or updated
- `documents.json` updated
- `claim-documents.json` created or updated
- copied attachment created under runtime attachment root
- `policy-documents.json` should not be updated by Scenario 8B
- project root `attachments/` remains files=0
- project root `data/local` remains files=0
- project root `runtime_test_document.*` remains missing

### Decision 8: Cleanup Policy

Confirmed:

- No cleanup during Scenario 8B.
- Cleanup decision comes after Scenario 8B result review only.
- Scenario 8A artifacts remain evidence unless separately cleaned.
- Temp `.txt` and `.png` artifacts from earlier Scenario 8A remain until separately approved cleanup.

## E. Options

### Option A: Proceed With Scenario 8B Claim Target Validation

Pros:

- Closes claim target document registration success path.
- Verifies `claim-documents.json` path.
- Completes policy and claim target coverage.

Cons:

- Creates more runtime artifacts.
- Cleanup burden increases.
- Runtime root is not clean-room.

Assessment:

- Recommended as the next scope planning path.
- Execution still requires a separate instruction and explicit approval.

### Option B: Defer Scenario 8B And Cleanup First

Pros:

- Cleaner runtime state before new validation.
- Less artifact mixing.

Cons:

- Delays claim target validation.
- Cleanup scope is nontrivial because Scenario 8A artifacts include documents, link metadata, and attachment evidence.

Assessment:

- Not selected for the immediate next planning step.

### Option C: Skip Scenario 8B

Pros:

- No additional runtime artifacts.
- Existing policy target validation may be enough for the current MVP if claim target registration is not needed immediately.

Cons:

- Claim target workflow remains unverified.
- `claim-documents.json` path remains untested.

Assessment:

- Not selected.

Recommended:

- Select Option A for the next execution instruction candidate.
- Do not execute Scenario 8B in this decision task.

## F. Stop Criteria For Future Execution

Future Scenario 8B execution must stop if any of the following occurs:

- actual document selection risk
- `data/claimdoc` selection risk
- project root `attachments/` pollution
- project root `data/local` pollution
- project root `runtime_test_document.*` created
- `FileNamePolicyService` or allowlist change required
- active policy creation failure
- active claim creation failure
- claim target unavailable
- OpenFileDialog selects a file other than approved temp PNG
- document registration failure
- copied attachment appears under project root
- `policy-documents.json` unexpectedly updated for claim target
- `claim-documents.json` missing after claimed success
- DB/SQLite unexpected file
- actual personal sample detected
- cleanup becomes necessary during execution

## G. Explicit Non-Scope

This task does not perform:

- app launch
- OpenFileDialog
- Scenario 8B execution
- synthetic PNG creation
- runtime policy creation
- runtime claim creation
- document registration workflow
- cleanup
- temp file deletion
- runtime artifact deletion
- code modification
- XAML modification
- ViewModel modification
- test modification
- `AppServices` modification
- `DocumentLinkCoordinator` modification
- `DocumentRegistrationWorkflow` modification
- `FileNamePolicyService` modification
- allowlist change
- `data/claimdoc` file use, open, listing, selection, stage, commit, delete, or move
- DB/SQLite/OCR/repository implementation
- git add/commit/reset/checkout/clean

## H. Next Recommendation

Next recommended document:

```text
docs/158_POLICY_CLAIM_SCENARIO8B_CLAIM_TARGET_EXECUTION_INSTRUCTION.md
```

Recommended next task:

```text
Scenario 8B claim target execution instruction document creation
```

Required verification for this decision document:

- `git diff --check`
- `git status --short`
- `git check-ignore -v -- data/claimdoc/`
- project root `attachments/`: files=0
- project root `data/local`: files=0
- project root `runtime_test_document.*`: missing
- DB/SQLite unexpected file check
- build/test not run, documentation-only change
