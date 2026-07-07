# Policy / Claim Scenario 8B Execution Instruction Ready Review

## A. Status Marker

READY_FOR_USER_APPROVED_SCENARIO8B_EXECUTION

## B. Purpose

This document reviews whether `docs/157` and `docs/158` are sufficient as the instruction basis for Scenario 8B claim target document registration.

This review is documentation-only.

It does not execute Scenario 8B and does not create `docs/159`.

## C. Reviewed Documents

- `docs/157_POLICY_CLAIM_SCENARIO8B_CLAIM_TARGET_SCOPE_DECISION.md`
- `docs/158_POLICY_CLAIM_SCENARIO8B_CLAIM_TARGET_EXECUTION_INSTRUCTION.md`
- `docs/nightwork_20260706/00_INDEX.md`
- `docs/nightwork_20260706/01_SCENARIO8B_APPROVAL_BOUNDARY.md`
- `docs/nightwork_20260706/02_SCENARIO8B_EXECUTION_INSTRUCTION_READY_CHECK.md`
- `docs/nightwork_20260706/03_SCENARIO8B_BASELINE_REVIEW.md`

## D. Ready Check Summary

| Check | Result | Evidence |
|---|---|---|
| Approval marker defined | PASS | `PHASE3D_SCENARIO8B_SYNTHETIC_CLAIM_DOCUMENT_REGISTRATION_APPROVED` exists in `docs/158`. |
| Target type is claim only | PASS | `docs/158` states `claim target only`. |
| Policy target registration is not primary goal | PASS | `docs/158` forbids policy target registration as the primary goal. |
| Synthetic PNG path is explicit | PASS | `%TEMP%\FamilyClaimRef\runtime_test_document_claim.png`. |
| Synthetic policy title is explicit | PASS | `policy_title_scenario8b_demo`. |
| Synthetic claim title is explicit | PASS | `claim_title_scenario8b_demo`. |
| Document display title is explicit | PASS | `scenario8b_claim_document_png_demo`. |
| Fresh policy/claim pair is required | PASS | `docs/158` specifies fresh synthetic policy and claim creation. |
| `data/claimdoc` exclusion is explicit | PASS | `docs/158` forbids use, inspection, listing, selection, stage, commit, delete, or move. |
| Stop criteria are sufficient | PASS | `docs/158` includes failure and safety stop criteria. |
| Expected artifacts are sufficient | PASS | `docs/158` defines policy, claim, document, claim-document, and project-root safety expectations. |
| Result review document is reserved | PASS | `docs/159_POLICY_CLAIM_SCENARIO8B_CLAIM_TARGET_RESULT_REVIEW.md`. |
| Cleanup is forbidden during execution | PASS | `docs/158` states no cleanup. |

## E. Baseline Review

### E1. Source Baseline

Latest commit:

```text
5615736 chore(familyclaimref): ignore local claim documents
```

Current `git status --short` before this document was created:

```text
?? docs/157_POLICY_CLAIM_SCENARIO8B_CLAIM_TARGET_SCOPE_DECISION.md
?? docs/158_POLICY_CLAIM_SCENARIO8B_CLAIM_TARGET_EXECUTION_INSTRUCTION.md
?? docs/nightwork_20260706/
```

Tracked source diff before this document:

```text
none
```

### E2. Document Path Baseline

| Path | Status |
|---|---|
| `docs/157_POLICY_CLAIM_SCENARIO8B_CLAIM_TARGET_SCOPE_DECISION.md` | exists |
| `docs/158_POLICY_CLAIM_SCENARIO8B_CLAIM_TARGET_EXECUTION_INSTRUCTION.md` | exists |
| `docs/159_POLICY_CLAIM_SCENARIO8B_CLAIM_TARGET_RESULT_REVIEW.md` | missing, reserved for actual Scenario 8B execution result |
| `docs/160_POLICY_CLAIM_SCENARIO8B_EXECUTION_INSTRUCTION_READY_REVIEW.md` | missing before this review |
| `docs/161_POLICY_CLAIM_SCENARIO8B_DOCS_COMMIT_CANDIDATE_REVIEW.md` | missing before this review |
| `docs/162_POLICY_CLAIM_SCENARIO8_ARTIFACT_CLEANUP_SCOPE_DESIGN.md` | missing before this review |

### E3. Project Root Safety Baseline

| Check | Result |
|---|---|
| project root `attachments/` files | 0 |
| project root `data/local` files | 0 |
| project root `runtime_test_document.*` files | 0, missing |
| unexpected DB/SQLite files in Git-visible untracked set | none |

### E4. `data/claimdoc` Baseline

Path-level ignore check:

```text
.gitignore:6:/data/claimdoc/	data/claimdoc/
```

Handling:

- `data/claimdoc` contents were not inspected.
- `data/claimdoc` file list was not collected.
- `data/claimdoc` was not opened, used, staged, committed, moved, deleted, or selected.

### E5. Runtime Path Baseline

Path existence only:

| Runtime path | Exists |
|---|---|
| `%LOCALAPPDATA%\FamilyClaimRef` | yes |
| `%LOCALAPPDATA%\FamilyClaimRef\data\local` | yes |
| `%LOCALAPPDATA%\FamilyClaimRef\attachments` | yes |
| `%LOCALAPPDATA%\FamilyClaimRef\data\local\policies.json` | yes |
| `%LOCALAPPDATA%\FamilyClaimRef\data\local\claims.json` | no |
| `%LOCALAPPDATA%\FamilyClaimRef\data\local\documents.json` | yes |
| `%LOCALAPPDATA%\FamilyClaimRef\data\local\policy-documents.json` | yes |
| `%LOCALAPPDATA%\FamilyClaimRef\data\local\claim-documents.json` | no |

Interpretation:

- Scenario 8A policy-target evidence remains.
- Scenario 8B claim-target evidence has not been created in runtime metadata.
- Runtime state is not clean-room.

### E6. Temp File Baseline

Path existence only:

| Temp path | Exists |
|---|---|
| `%TEMP%\FamilyClaimRef\runtime_test_document.txt` | yes |
| `%TEMP%\FamilyClaimRef\runtime_test_document.png` | yes |
| `%TEMP%\FamilyClaimRef\runtime_test_document_claim.png` | yes |

Interpretation:

- Existing temp files are artifacts from prior approved attempts or aborted setup.
- This ready review did not create, delete, or modify temp files.

## F. Readiness Decision

Decision:

```text
READY_FOR_USER_APPROVED_SCENARIO8B_EXECUTION
```

Reason:

- `docs/158` contains the required Scenario 8B execution boundary.
- The target is claim-only.
- The synthetic values and approved temp PNG path are explicit.
- `docs/159` remains reserved and was not created by this ready review.
- Cleanup remains forbidden during execution.
- No runtime execution was performed.

Important boundary:

- This readiness result does not authorize runtime execution inside night work.
- Actual Scenario 8B execution still requires a separate execution turn that is allowed to launch the app and run OpenFileDialog.

## G. Non-Scope Confirmed

Not performed:

- app launch
- OpenFileDialog
- Scenario 8B runtime execution
- synthetic PNG creation
- runtime policy creation
- runtime claim creation
- document registration workflow
- cleanup
- temp file deletion
- runtime artifact deletion
- code/XAML/ViewModel/test modification
- `FileNamePolicyService` modification
- allowlist change
- DB/SQLite/OCR/repository implementation
- git add/commit/reset/checkout/clean

## H. Next Recommendation

Next recommended document:

```text
docs/161_POLICY_CLAIM_SCENARIO8B_DOCS_COMMIT_CANDIDATE_REVIEW.md
```

Recommended commit message candidate for the planning docs:

```text
docs(familyclaimref): add scenario8b claim target plan
```
