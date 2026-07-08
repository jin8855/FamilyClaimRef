# Policy/Claim Scenario 9 Isolated Runtime Artifact Cleanup Policy Decision

## A. Status

Status: DECISION_ONLY

Marker:

```text
POLICY_CLAIM_SCENARIO9_ISOLATED_RUNTIME_ARTIFACT_CLEANUP_POLICY_RECORDED
```

This document records cleanup policy only.

No cleanup is executed by this document.

No deletion is authorized by this document.

## B. Baseline

Record:

- latest commit:
  `3fd316e docs(familyclaimref): review isolated runtime manual validation`
- source docs reviewed:
  - `docs/182_POLICY_CLAIM_RUNTIME_ROOT_PROVIDER_IMPLEMENTATION_RESULT_REVIEW.md`
  - `docs/185_POLICY_CLAIM_ISOLATED_RUNTIME_AUTOMATED_VALIDATION_RESULT_REVIEW.md`
  - `docs/186_POLICY_CLAIM_ISOLATED_RUNTIME_MANUAL_VALIDATION_PLAN.md`
  - `docs/187_POLICY_CLAIM_ISOLATED_RUNTIME_MANUAL_VALIDATION_EXECUTION_INSTRUCTION.md`
  - `docs/189_POLICY_CLAIM_ISOLATED_RUNTIME_MANUAL_VALIDATION_RESULT_REVIEW.md`

## C. Artifact Classes

Record:

1. Default runtime evidence:
   - `%LOCALAPPDATA%\FamilyClaimRef\data\local\...`
   - `%LOCALAPPDATA%\FamilyClaimRef\attachments\documents`
   - cleanup decision: DEFER / preserve

2. Scenario 9 isolated runtime evidence:
   - `%TEMP%\FamilyClaimRef-Isolated\scenario9_manual_validation...`
   - cleanup decision: eligible for later exact cleanup after separate approval

3. Scenario 9 synthetic input files:
   - `%TEMP%\FamilyClaimRef-IsolatedInputs\scenario9\...`
   - cleanup decision: eligible for later exact cleanup after separate approval

4. Project root:
   - `attachments/`
   - `data/local/`
   - `runtime_test_document.*`
   - expected files=0
   - cleanup decision: no cleanup needed

5. `data/claimdoc`:
   - ignored
   - not inspected
   - not listed
   - not used
   - not a cleanup target

## D. Cleanup Options

### Option A: Keep all Scenario 9 artifacts

- safest for audit
- leaves `%TEMP%` clutter

### Option B: Clean only Scenario 9 synthetic input files

- low risk
- preserves isolated runtime root evidence

### Option C: Clean Scenario 9 isolated runtime root and synthetic inputs

- acceptable later because Scenario 9 result review is committed
- must be exact path only
- must not delete parent `%TEMP%\FamilyClaimRef-Isolated`
- must not delete parent `%TEMP%\FamilyClaimRef-IsolatedInputs`
- requires separate explicit approval

### Option D: Clean default runtime evidence

- reject
- default evidence remains preserved
- runtime metadata/attachment cleanup remains DEFER

## E. Recommended Policy

Recommend:

- Keep default runtime evidence preserved.
- Keep runtime metadata cleanup DEFER.
- Keep runtime attachment cleanup DEFER.
- Reject default runtime root cleanup.
- Allow later exact cleanup of Scenario 9 isolated runtime artifacts only after separate approval.
- Do not execute cleanup in this batch.

## F. Future Cleanup Approval Marker

Define future marker:

```text
POLICY_CLAIM_SCENARIO9_ISOLATED_RUNTIME_TEMP_ARTIFACT_CLEANUP_APPROVED
```

Important:

- This marker appearing in this document is only a definition.
- It is not approval.
- Cleanup requires a later explicit user approval message.

## G. Future Cleanup Guardrails

If approved later:

- exact path delete only
- no wildcard deletion
- no recursive parent deletion
- no `%TEMP%\FamilyClaimRef-Isolated` parent deletion
- no `%TEMP%\FamilyClaimRef-IsolatedInputs` parent deletion
- no `%LOCALAPPDATA%\FamilyClaimRef` deletion
- no default runtime metadata deletion
- no default runtime attachment deletion
- no project root cleanup
- no `data/claimdoc` access
- pre/post snapshot required
- result review document required

## H. Decision Judgment

```text
POLICY_CLAIM_SCENARIO9_ISOLATED_RUNTIME_ARTIFACT_CLEANUP_POLICY_READY
```
