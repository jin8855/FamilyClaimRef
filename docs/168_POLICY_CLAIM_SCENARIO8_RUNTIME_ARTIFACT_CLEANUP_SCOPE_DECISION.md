# Policy / Claim Scenario 8 Runtime Artifact Cleanup Scope Decision

## A. Status Marker

POLICY_CLAIM_SCENARIO8_RUNTIME_ARTIFACT_CLEANUP_SCOPE_DECISION_RECORDED

## B. Decision Context

- Scenario 8A policy target registration success path was verified.
- Scenario 8B claim target registration success path was verified.
- Scenario 8A/8B result docs are committed.
- Scenario 8A/8B runtime artifacts remain under `%LOCALAPPDATA%\FamilyClaimRef`.
- Temp synthetic files remain under `%TEMP%\FamilyClaimRef`.
- Cleanup has not been approved.
- Runtime root is not clean-room.
- This document decides cleanup options but does not execute cleanup.

## C. Current Artifact State

Current source baseline:

- latest commit: `8c0f2f1 docs(familyclaimref): add scenario8b result review`
- git status before this document: clean

Temp artifacts:

- `%TEMP%\FamilyClaimRef\runtime_test_document.txt`: exists
- `%TEMP%\FamilyClaimRef\runtime_test_document.png`: exists
- `%TEMP%\FamilyClaimRef\runtime_test_document_claim.png`: exists

Runtime metadata:

- `%LOCALAPPDATA%\FamilyClaimRef\data\local\policies.json`: exists
- `%LOCALAPPDATA%\FamilyClaimRef\data\local\claims.json`: exists
- `%LOCALAPPDATA%\FamilyClaimRef\data\local\documents.json`: exists
- `%LOCALAPPDATA%\FamilyClaimRef\data\local\policy-documents.json`: exists
- `%LOCALAPPDATA%\FamilyClaimRef\data\local\claim-documents.json`: exists

Runtime attachments:

- `%LOCALAPPDATA%\FamilyClaimRef\attachments\documents\claim-document_20260707_etc_001.png`: exists, 68 bytes
- `%LOCALAPPDATA%\FamilyClaimRef\attachments\documents\policy-document_20260702_policy_001.png`: exists, 61 bytes
- `%LOCALAPPDATA%\FamilyClaimRef\attachments\documents\policy-document_20260706_capture_001.png`: exists, 68 bytes

Project root:

- `attachments/`: files=0
- `data/local`: files=0
- `runtime_test_document.*`: missing

`data/claimdoc`:

- ignored by `.gitignore`
- not a cleanup target
- not inspected, listed, or used in this task

## D. Problem Definition

- Runtime/temp evidence is useful but may affect future validation.
- Cleanup before documentation would destroy evidence.
- Evidence is now committed, so cleanup may be considered.
- Full runtime cleanup could remove pre-existing evidence and is risky.
- JSON record-level cleanup is risky without tooling.
- Temp file cleanup is low risk but still needs explicit approval.
- Scenario 8A/8B runtime metadata/attachments cleanup needs exact path and preservation policy.

## E. Cleanup Options

### Option A: No Cleanup

Pros:

- Evidence remains fully available.
- No deletion risk.

Cons:

- Future runtime validations see existing artifacts.
- Temp files remain.

### Option B: Temp Synthetic Files Cleanup Only

Targets:

- `%TEMP%\FamilyClaimRef\runtime_test_document.txt`
- `%TEMP%\FamilyClaimRef\runtime_test_document.png`
- `%TEMP%\FamilyClaimRef\runtime_test_document_claim.png`

Pros:

- Low risk.
- Does not alter runtime metadata evidence.
- Reduces temp clutter.

Cons:

- `%LOCALAPPDATA%` runtime artifacts remain.

### Option C: Scenario 8 Runtime Metadata/Attachment Cleanup

Potential targets:

- policies/claims/documents/link records related to Scenario 8A/8B
- copied attachments from Scenario 8A/8B

Risk:

- JSON record-level cleanup requires safe tooling or exact verified mutation.
- File-level deletion of `documents.json`, `policy-documents.json`, or `claim-documents.json` would remove entire metadata files and is too broad.
- Not recommended without tooling.

### Option D: Full Runtime Root Cleanup

Reject by default:

- `%LOCALAPPDATA%\FamilyClaimRef` full deletion is too broad.
- Could delete pre-existing evidence.
- Requires separate explicit approval and pre/post snapshot.

### Option E: Keep Runtime Evidence, Design Isolated Runtime Root For Future

Pros:

- Avoids deleting evidence.
- Future validation can be clean-room if root override is designed.

Cons:

- Requires code/config design.
- Out of scope for cleanup decision.

## F. Recommended Decision

Recommended:

- Select Option B as first cleanup candidate: temp synthetic files cleanup only.
- Do not cleanup runtime metadata/attachments now.
- Do not delete `%LOCALAPPDATA%\FamilyClaimRef`.
- Defer JSON record-level cleanup until safe tooling/design exists.
- Runtime evidence remains preserved for audit.
- Cleanup execution requires separate instruction and explicit approval.

## G. Confirmed Decision

Confirmed:

- No cleanup in this decision task.
- Temp synthetic files cleanup is the recommended next cleanup execution candidate.
- Runtime metadata/link/attachment cleanup deferred.
- Full runtime root cleanup rejected.
- `data/claimdoc` is never a cleanup target.

## H. Future Cleanup Execution Guardrails

If temp cleanup is approved later:

- exact path delete only
- no wildcard
- no recursive delete
- no directory delete
- no `%TEMP%\FamilyClaimRef` folder deletion
- no runtime metadata deletion
- no runtime attachment deletion
- pre-cleanup snapshot
- post-cleanup snapshot
- cleanup result review document

## I. Explicit Non-Scope

- cleanup execution 없음
- temp deletion 없음
- runtime artifact deletion 없음
- app launch 없음
- OpenFileDialog 없음
- Scenario 8 execution 없음
- `data/claimdoc` use/listing 없음
- code/XAML/ViewModel/test 수정 없음
- git add/commit/reset/checkout/clean 없음

## J. Verification

- `git diff --check`: to be run after document creation
- `git status --short`: expected `?? docs/168_POLICY_CLAIM_SCENARIO8_RUNTIME_ARTIFACT_CLEANUP_SCOPE_DECISION.md`
- `git check-ignore -v -- data/claimdoc/`: `.gitignore:6:/data/claimdoc/	data/claimdoc/`
- project root `attachments/`: files=0
- project root `data/local`: files=0
- project root `runtime_test_document.*`: missing
- DB/SQLite unexpected file 없음
- build/test not run, documentation-only decision

## K. Next Recommendation

Scenario 8 temp synthetic file cleanup execution instruction 문서 생성.

Expected document:

- `docs/169_POLICY_CLAIM_SCENARIO8_TEMP_FILE_CLEANUP_EXECUTION_INSTRUCTION.md`
