# Policy / Claim Scenario 8 Artifact Cleanup Scope Design

## A. Status Marker

POLICY_CLAIM_SCENARIO8_ARTIFACT_CLEANUP_SCOPE_DESIGN_DRAFTED

## B. Purpose

This document designs cleanup options for Scenario 8 artifacts.

This document does not perform cleanup and does not approve deletion.

## C. Current Boundary

Confirmed:

- no cleanup now
- no runtime artifact deletion
- no temp file deletion
- no directory deletion
- no wildcard deletion
- no recursive deletion
- no app launch
- no OpenFileDialog
- no Scenario 8B runtime execution
- `data/claimdoc` is never a cleanup target

## D. Artifact Groups

### D1. Scenario 8A Runtime Metadata

Known metadata paths from path-level checks and prior reviews:

```text
%LOCALAPPDATA%\FamilyClaimRef\data\local\policies.json
%LOCALAPPDATA%\FamilyClaimRef\data\local\documents.json
%LOCALAPPDATA%\FamilyClaimRef\data\local\policy-documents.json
```

Potential cleanup handling:

- keep as evidence until Scenario 8B is complete
- later targeted cleanup by exact file path only
- no wildcard metadata deletion
- no full runtime root deletion by default

### D2. Scenario 8B Future Runtime Metadata

Future possible paths:

```text
%LOCALAPPDATA%\FamilyClaimRef\data\local\claims.json
%LOCALAPPDATA%\FamilyClaimRef\data\local\claim-documents.json
```

Potential cleanup handling:

- do not clean until Scenario 8B result review is recorded
- if cleanup is later approved, clean by exact path only
- preserve evidence if failure analysis is still needed

### D3. Runtime Attachments

Runtime attachment root:

```text
%LOCALAPPDATA%\FamilyClaimRef\attachments
```

Policy:

- do not delete the full attachment root by default
- do not use wildcard deletion
- exact copied attachment paths must come from a future result review before deletion
- no attachment cleanup without explicit user approval

### D4. Temp Synthetic Files

Known temp candidates:

```text
%TEMP%\FamilyClaimRef\runtime_test_document.txt
%TEMP%\FamilyClaimRef\runtime_test_document.png
%TEMP%\FamilyClaimRef\runtime_test_document_claim.png
```

Potential cleanup handling:

- delete only after explicit approval
- delete only exact paths
- do not delete `%TEMP%\FamilyClaimRef` as a directory by default

## E. Rejected Cleanup Targets

Never clean:

```text
C:\EtcProject\FamilyClaimRef\data\claimdoc
C:\EtcProject\FamilyClaimRef\data
C:\EtcProject\FamilyClaimRef\attachments
C:\EtcProject\FamilyClaimRef\.git
```

Do not clean:

- source files
- docs
- tests
- project configuration
- any real user document

## F. Cleanup Options

### Option A: No Cleanup Until Scenario 8B Completes

Assessment:

- Recommended.
- Preserves Scenario 8A evidence.
- Avoids mixing cleanup with future Scenario 8B execution.

### Option B: Targeted Metadata Cleanup After Scenario 8B Result Review

Assessment:

- Candidate after `docs/159` exists.
- Must use exact file paths.
- Requires user approval.

### Option C: Targeted Temp File Cleanup

Assessment:

- Candidate after user confirms temp synthetic evidence is no longer needed.
- Must use exact file paths.
- Requires user approval.

### Option D: Full Runtime Root Cleanup

Assessment:

- Rejected by default.
- Too broad for evidence-preserving validation.
- Could delete unrelated local runtime state.

## G. Required Future Approval Format

Future cleanup should require:

- exact path list
- deletion purpose
- evidence preservation decision
- explicit statement that `data/claimdoc` is excluded
- explicit statement that wildcard and recursive deletion are forbidden

## H. Current Decision

Current recommendation:

```text
NO_CLEANUP_NOW
```

Next recommended cleanup-related document, only if needed later:

```text
docs/163_POLICY_CLAIM_SCENARIO8_ARTIFACT_CLEANUP_USER_DECISION_RECORD.md
```

## I. Non-Scope Confirmed

Not performed:

- cleanup
- deletion
- app launch
- OpenFileDialog
- Scenario 8B execution
- runtime workflow
- file selection
- code/XAML/ViewModel/test modification
- DB/SQLite/OCR/repository implementation
- git add/commit/reset/checkout/clean

## J. data/claimdoc Handling

`data/claimdoc` was not opened, listed, inspected, selected, staged, committed, moved, deleted, or used.
