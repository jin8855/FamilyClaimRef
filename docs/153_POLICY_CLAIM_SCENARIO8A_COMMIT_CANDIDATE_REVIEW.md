# Policy / Claim Scenario 8A Commit Candidate Review

## A. Status Marker

POLICY_CLAIM_SCENARIO8A_COMMIT_CANDIDATE_READY

## B. Review Target

Reviewed uncommitted Scenario 8A evidence chain:

- `docs/145_POLICY_CLAIM_SCENARIO8_SYNTHETIC_DOCUMENT_REGISTRATION_DECISION.md`
- `docs/146_POLICY_CLAIM_SCENARIO8_SYNTHETIC_DOCUMENT_REGISTRATION_EXECUTION_INSTRUCTION.md`
- `docs/147_POLICY_CLAIM_SCENARIO8_SYNTHETIC_DOCUMENT_REGISTRATION_RESULT_REVIEW.md`
- `docs/148_POLICY_CLAIM_SCENARIO8A_RETRY_POLICY_DECISION.md`
- `docs/149_POLICY_CLAIM_LOCAL_DATA_CLAIMDOC_HANDLING_DECISION.md`
- `docs/150_POLICY_CLAIM_SCENARIO8A_ALLOWED_EXTENSION_RETRY_EXECUTION_INSTRUCTION.md`
- `docs/151_POLICY_CLAIM_SCENARIO8A_ALLOWED_EXTENSION_RETRY_RESULT_REVIEW.md`
- `docs/152_POLICY_CLAIM_SCENARIO8A_RETRY_ARTIFACT_CLEANUP_SCOPE_DECISION.md`

This review creates:

- `docs/153_POLICY_CLAIM_SCENARIO8A_COMMIT_CANDIDATE_REVIEW.md`

Known local excluded artifact:

- `data/`

`data/` is treated only as expected-but-excluded. Its child files were not inspected, listed, used, staged, moved, or deleted.

## C. Scope Review

- documentation-only change: PASS
- Scenario 8A evidence chain: PASS
- source code changes: none
- XAML changes: none
- ViewModel changes: none
- tests changes: none
- `FileNamePolicyService` changes: none
- allowlist changes: none
- `data/` excluded: PASS
- runtime artifacts included in source tree: none
- cleanup performed: no
- Scenario 8B executed: no
- app launch in this review: no
- OpenFileDialog in this review: no
- git add/commit/reset/checkout/clean in this review: no

The current candidate is limited to docs/145~153 plus the explicitly excluded untracked `data/` item.

## D. Scenario 8A Initial Attempt Review

Based on `docs/145`~`docs/147`:

- Scenario 8A was limited to policy target synthetic document registration.
- Scenario 8B claim target registration was not executed.
- The initial attempt used a temp `.txt` synthetic file outside the project root.
- App launch: PASS.
- Runtime synthetic policy creation: PASS.
- OpenFileDialog: PASS.
- Approved `.txt` file selected: PASS.
- Registration result: BLOCKED.
- Likely cause: `.txt` extension outside the current allowlist.
- `documents.json`: unchanged.
- `policy-documents.json`: unchanged.
- copied attachment: none.
- project root `attachments/`: files=0.
- project root `data/local`: files=0.
- project root `runtime_test_document.txt`: missing.

The initial blocked result is valid evidence for file extension policy rejection, but not sufficient evidence for the successful document metadata/link/copy path.

## E. Retry Decision / Execution Review

Based on `docs/148`~`docs/151`:

- Retry policy selected allowed-extension synthetic PNG.
- `.txt` final-only result was rejected as incomplete for Scenario 8A success-path validation.
- PDF retry was not selected as the first retry option.
- `.txt` allowlist addition was rejected.
- UI error-message hardening was deferred as a follow-up.
- `FileNamePolicyService` was not changed.
- allowlist was not changed.
- `data/claimdoc` was not used, inspected, listed, selected, staged, or committed.
- Retry used `%TEMP%\FamilyClaimRef\runtime_test_document.png`.
- OpenFileDialog selected the approved PNG only.
- Policy target document registration: PASS.
- UI status indicated document registration completed.
- `documents.json` sanity: PASS.
- `policy-documents.json` sanity: PASS.
- copied attachment created under runtime attachment root: PASS.
- `claims.json`: missing, expected for Scenario 8A policy-only scope.
- `claim-documents.json`: missing, expected for Scenario 8A policy-only scope.
- project root `attachments/`: files=0.
- project root `data/local`: files=0.
- project root `runtime_test_document.*`: missing.
- DB/SQLite unexpected file: none.
- actual personal sample: none.

Scenario 8A policy target registration success path is documented. Scenario 8B remains gated and untested.

## F. data/claimdoc Handling Review

Based on `docs/149`:

- `data/claimdoc` is a known local real-document artifact.
- `data/` appears in git status as an expected-but-excluded item.
- `data/` is not a commit candidate.
- `data/claimdoc` files are not used for Scenario 8A validation.
- `data/claimdoc` files are not inspected.
- `data/claimdoc` files are not listed.
- `data/claimdoc` files are not staged.
- `data/claimdoc` files are not moved.
- `data/claimdoc` files are not deleted.
- `.gitignore` change for `data/claimdoc` remains deferred.

Commit candidate exact-file-list must not include `data/`.

## G. Cleanup Decision Review

Based on `docs/152`:

- selected cleanup policy: Option A, No Cleanup + Commit Evidence First
- temp/runtime artifact cleanup: deferred
- full runtime root cleanup: rejected
- Scenario 8A-created runtime JSON/link/attachment cleanup: deferred
- temp `.txt` and `.png` cleanup: possible later follow-up
- `data/claimdoc`: still expected-but-excluded and untouched

Current cleanup policy preserves Scenario 8A evidence before any cleanup step.

## H. Safety Review

Safety checks:

- actual personal information sample: none detected in candidate docs
- actual family name sample: none detected in candidate docs
- actual insurance contract number sample: none detected in candidate docs
- actual claim number sample: none detected in candidate docs
- actual insurance product name sample: none detected in candidate docs
- actual hospital name sample: none detected in candidate docs
- actual diagnosis name/code sample: none detected in candidate docs
- actual contract/insurance document used: none
- `data/claimdoc` use: none
- project root pollution: none
- DB/SQLite unexpected file: none
- tracked source tree unexpected modification: none
- git add/commit/reset/checkout/clean: none

Only approved synthetic markers were found in docs/145~152, such as:

- `policy_title_scenario8_demo`
- `policy_title_scenario8_retry_demo`
- `scenario8_policy_document_demo`
- `scenario8_policy_document_png_retry_demo`
- `runtime_test_document.txt`
- `runtime_test_document.png`

## I. Verification Results

`git diff --check`:

```text
PASS
```

Direct `git status --short`:

```text
fatal: detected dubious ownership in repository at 'C:/EtcProject/FamilyClaimRef'
```

Non-mutating status query used for review:

```powershell
git -c safe.directory=C:/EtcProject/FamilyClaimRef status --short
```

Result before creating `docs/153`:

```text
?? data/
?? docs/145_POLICY_CLAIM_SCENARIO8_SYNTHETIC_DOCUMENT_REGISTRATION_DECISION.md
?? docs/146_POLICY_CLAIM_SCENARIO8_SYNTHETIC_DOCUMENT_REGISTRATION_EXECUTION_INSTRUCTION.md
?? docs/147_POLICY_CLAIM_SCENARIO8_SYNTHETIC_DOCUMENT_REGISTRATION_RESULT_REVIEW.md
?? docs/148_POLICY_CLAIM_SCENARIO8A_RETRY_POLICY_DECISION.md
?? docs/149_POLICY_CLAIM_LOCAL_DATA_CLAIMDOC_HANDLING_DECISION.md
?? docs/150_POLICY_CLAIM_SCENARIO8A_ALLOWED_EXTENSION_RETRY_EXECUTION_INSTRUCTION.md
?? docs/151_POLICY_CLAIM_SCENARIO8A_ALLOWED_EXTENSION_RETRY_RESULT_REVIEW.md
?? docs/152_POLICY_CLAIM_SCENARIO8A_RETRY_ARTIFACT_CLEANUP_SCOPE_DECISION.md
```

Tracked source diff:

```text
none
```

Project root `attachments/`:

```text
files=0
```

Project root `data/local`:

```text
files=0
```

Project root `runtime_test_document.*`:

```text
missing
```

DB/SQLite unexpected file:

```text
none
```

Actual personal sample targeted scan:

```text
PASS
No matches for targeted resident-id, phone, email, or synthetic policy-number-like PII patterns in docs/145~152.
```

Current expected runtime evidence state:

```text
%TEMP%\FamilyClaimRef\runtime_test_document.txt: exists
%TEMP%\FamilyClaimRef\runtime_test_document.png: exists
%LOCALAPPDATA%\FamilyClaimRef\data\local\policies.json: exists
%LOCALAPPDATA%\FamilyClaimRef\data\local\documents.json: exists
%LOCALAPPDATA%\FamilyClaimRef\data\local\policy-documents.json: exists
%LOCALAPPDATA%\FamilyClaimRef\data\local\claims.json: missing
%LOCALAPPDATA%\FamilyClaimRef\data\local\claim-documents.json: missing
%LOCALAPPDATA%\FamilyClaimRef\attachments\documents\policy-document_20260706_capture_001.png: exists
```

Build/test for this review:

```text
not run, documentation-only review
```

Prior relevant build/test baseline:

- `docs/147`: escalated `dotnet build` PASS, escalated `dotnet test` PASS, total tests 271.
- `docs/151`: `dotnet build FamilyClaimRef.sln` PASS; `dotnet test` not run for the retry execution scope.

## J. Git Status Summary

Status before creating this document:

```text
?? data/
?? docs/145_POLICY_CLAIM_SCENARIO8_SYNTHETIC_DOCUMENT_REGISTRATION_DECISION.md
?? docs/146_POLICY_CLAIM_SCENARIO8_SYNTHETIC_DOCUMENT_REGISTRATION_EXECUTION_INSTRUCTION.md
?? docs/147_POLICY_CLAIM_SCENARIO8_SYNTHETIC_DOCUMENT_REGISTRATION_RESULT_REVIEW.md
?? docs/148_POLICY_CLAIM_SCENARIO8A_RETRY_POLICY_DECISION.md
?? docs/149_POLICY_CLAIM_LOCAL_DATA_CLAIMDOC_HANDLING_DECISION.md
?? docs/150_POLICY_CLAIM_SCENARIO8A_ALLOWED_EXTENSION_RETRY_EXECUTION_INSTRUCTION.md
?? docs/151_POLICY_CLAIM_SCENARIO8A_ALLOWED_EXTENSION_RETRY_RESULT_REVIEW.md
?? docs/152_POLICY_CLAIM_SCENARIO8A_RETRY_ARTIFACT_CLEANUP_SCOPE_DECISION.md
```

Expected additional file after this document:

```text
?? docs/153_POLICY_CLAIM_SCENARIO8A_COMMIT_CANDIDATE_REVIEW.md
```

Unexpected file:

```text
none, except expected-but-excluded data/
```

Staged files:

```text
none
```

## K. Commit Readiness

commit readiness:

```text
ready
```

reason:

- docs/145~153 are documentation-only commit candidates.
- `data/` is expected-but-excluded and must not be committed.
- `git diff --check` passed.
- tracked source diff is empty.
- project root `attachments/` files count is 0.
- project root `data/local` files count is 0.
- project root `runtime_test_document.*` is missing.
- DB/SQLite unexpected file check found none.
- actual personal sample targeted scan found none.
- Scenario 8A success path is documented.
- Scenario 8B remains gated.
- cleanup remains deferred.

Operational caveat:

- Plain `git status --short` is blocked by dubious ownership in this environment.
- This review used `git -c safe.directory=C:/EtcProject/FamilyClaimRef ...` for read-only status/diff queries and did not change global git config.

## L. Commit Candidate Exact File List

Commit candidate exact file list:

- `docs/145_POLICY_CLAIM_SCENARIO8_SYNTHETIC_DOCUMENT_REGISTRATION_DECISION.md`
- `docs/146_POLICY_CLAIM_SCENARIO8_SYNTHETIC_DOCUMENT_REGISTRATION_EXECUTION_INSTRUCTION.md`
- `docs/147_POLICY_CLAIM_SCENARIO8_SYNTHETIC_DOCUMENT_REGISTRATION_RESULT_REVIEW.md`
- `docs/148_POLICY_CLAIM_SCENARIO8A_RETRY_POLICY_DECISION.md`
- `docs/149_POLICY_CLAIM_LOCAL_DATA_CLAIMDOC_HANDLING_DECISION.md`
- `docs/150_POLICY_CLAIM_SCENARIO8A_ALLOWED_EXTENSION_RETRY_EXECUTION_INSTRUCTION.md`
- `docs/151_POLICY_CLAIM_SCENARIO8A_ALLOWED_EXTENSION_RETRY_RESULT_REVIEW.md`
- `docs/152_POLICY_CLAIM_SCENARIO8A_RETRY_ARTIFACT_CLEANUP_SCOPE_DECISION.md`
- `docs/153_POLICY_CLAIM_SCENARIO8A_COMMIT_CANDIDATE_REVIEW.md`

Do not include:

- `data/`
- `data/claimdoc`
- runtime files
- temp files
- code files
- XAML files
- ViewModel files
- test files
- project root `attachments/`
- project root `data/local`

## M. Recommended Commit Message

```text
docs(familyclaimref): add scenario8 document registration review
```

## N. Remaining Risks / Follow-up

Remaining risks:

- Scenario 8B claim target remains untested.
- temp `.txt` / `.png` cleanup remains deferred.
- Scenario 8A runtime artifacts remain under `%LOCALAPPDATA%`.
- `data/claimdoc` remains an untracked excluded local artifact.
- optional `.gitignore` decision for `data/claimdoc` remains deferred.
- UI error message hardening for extension rejection remains follow-up.
- cleanup decision for Scenario 8A artifacts remains deferred.
- plain git status requires safe.directory handling or repository ownership resolution before regular git operations.

Recommended next actions:

1. Commit exact docs/145~153 file list only.
2. Keep `data/` unstaged and excluded.
3. Decide separately whether to add an exact `.gitignore` rule for `data/claimdoc`.
4. Decide separately whether and how to clean up Scenario 8A temp/runtime artifacts.
5. Decide separately whether to proceed with Scenario 8B claim target registration scope.
