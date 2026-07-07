# Policy/Claim Scenario 8 Temp File Cleanup Execution Instruction

## A. Status

Status: INSTRUCTION_ONLY

This document defines a future cleanup instruction only.

No cleanup is executed by creating this document.

No temp file deletion is authorized by this document.

No runtime artifact deletion is authorized by this document.

## B. Source Decision

Source decision document:

- `docs/168_POLICY_CLAIM_SCENARIO8_RUNTIME_ARTIFACT_CLEANUP_SCOPE_DECISION.md`

Recorded decision:

- Option B is the first cleanup candidate.
- Option B means temp synthetic files cleanup only.
- Runtime metadata/link/attachment cleanup remains deferred.
- Full runtime root cleanup is rejected for this phase.
- Cleanup execution requires a later explicit approval marker.

## C. Required Future Approval Marker

The required future approval marker is:

```text
PHASE3D_SCENARIO8_TEMP_SYNTHETIC_FILE_CLEANUP_APPROVED
```

Important interpretation rule:

- The marker text appearing inside this document is only a definition.
- The marker text inside this document is not approval.
- Cleanup requires a later explicit user approval message.
- Without the later explicit user approval message, deletion remains forbidden.

## D. Exact Future Deletion Candidates

After later explicit approval only, the deletion candidates are exactly:

- `%TEMP%\FamilyClaimRef\runtime_test_document.txt`
- `%TEMP%\FamilyClaimRef\runtime_test_document.png`
- `%TEMP%\FamilyClaimRef\runtime_test_document_claim.png`

No other temp files are included.

No directories are included.

No runtime files are included.

## E. Explicitly Forbidden During Future Temp Cleanup

Even after approval, the following remain forbidden:

- Do not delete `%TEMP%\FamilyClaimRef` directory.
- Do not use wildcard deletion.
- Do not use recursive deletion.
- Do not delete runtime metadata.
- Do not delete runtime links.
- Do not delete runtime attachments.
- Do not delete `%LOCALAPPDATA%\FamilyClaimRef`.
- Do not clean project root `attachments/`.
- Do not clean project root `data/local`.
- Do not touch `data/claimdoc`.
- Do not run app launch.
- Do not open OpenFileDialog.
- Do not run document registration workflow.
- Do not create synthetic files.
- Do not modify code, XAML, ViewModel, tests, `FileNamePolicyService`, or allowlist.
- Do not commit as part of cleanup execution unless separately approved after result review.

## F. Future Approved Execution Shape

If the user later provides the required approval marker, execution must use exact literal paths only.

Required execution shape:

- Use `Test-Path` before deletion.
- Use `Remove-Item` only for existing exact files.
- Do not pass `-Recurse`.
- Do not use globs.
- Do not delete parent directories.
- Missing candidate files must be reported as missing.
- Missing candidate files must not be treated as authorization to broaden scope.

## G. Pre-Cleanup Snapshot Procedure

For a later approved cleanup, capture this before deletion:

1. Confirm current directory:

```text
C:\EtcProject\FamilyClaimRef
```

2. Confirm git status and latest commit.

3. Confirm the user provided the later explicit approval marker:

```text
PHASE3D_SCENARIO8_TEMP_SYNTHETIC_FILE_CLEANUP_APPROVED
```

4. Check exact temp candidate existence:

- `%TEMP%\FamilyClaimRef\runtime_test_document.txt`
- `%TEMP%\FamilyClaimRef\runtime_test_document.png`
- `%TEMP%\FamilyClaimRef\runtime_test_document_claim.png`

5. For each existing candidate, capture:

- path
- file size
- last write time

6. Capture runtime metadata existence only:

- `%LOCALAPPDATA%\FamilyClaimRef\data\local\policies.json`
- `%LOCALAPPDATA%\FamilyClaimRef\data\local\claims.json`
- `%LOCALAPPDATA%\FamilyClaimRef\data\local\documents.json`
- `%LOCALAPPDATA%\FamilyClaimRef\data\local\policy-documents.json`
- `%LOCALAPPDATA%\FamilyClaimRef\data\local\claim-documents.json`

7. Capture runtime attachment directory existence/count only:

- `%LOCALAPPDATA%\FamilyClaimRef\attachments\documents\...`

8. Confirm project root `attachments/` file count.

9. Confirm project root `data/local` file count.

10. Confirm project root `runtime_test_document.*` files are absent.

11. Confirm `data/claimdoc` remains ignored and untouched.

## H. Post-Cleanup Snapshot Procedure

For a later approved cleanup, capture this after deletion:

1. Re-check the three exact temp candidate paths.

2. Confirm no wildcard deletion was used.

3. Confirm no recursive deletion was used.

4. Confirm `%TEMP%\FamilyClaimRef` directory was not deleted as a directory cleanup target.

5. Confirm `%LOCALAPPDATA%\FamilyClaimRef` still exists if it existed before.

6. Confirm runtime metadata/link files were not deleted.

7. Confirm runtime attachments were not deleted.

8. Confirm project root `attachments/` file count remains 0.

9. Confirm project root `data/local` file count remains 0.

10. Confirm project root `runtime_test_document.*` files remain absent.

11. Confirm no unexpected DB/SQLite file was created.

12. Confirm no code/XAML/ViewModel/test changes.

13. Confirm `FileNamePolicyService` and allowlist were not changed.

14. Prepare result review document.

## I. Expected Future Result Review Document

Expected future result review document:

- `docs/172_POLICY_CLAIM_SCENARIO8_TEMP_FILE_CLEANUP_RESULT_REVIEW.md`

The result review must record:

- approval marker source
- pre-cleanup snapshot
- exact deletion attempts
- missing/existing candidate status
- post-cleanup snapshot
- confirmation that runtime metadata/attachments were preserved
- confirmation that project root files were unaffected
- confirmation that no app launch/workflow execution occurred
- confirmation that no code changes occurred

## J. Non-Authorization Statement

This document is not an execution approval.

This document must not be used as a cleanup trigger.

Cleanup remains blocked until the user separately provides:

```text
PHASE3D_SCENARIO8_TEMP_SYNTHETIC_FILE_CLEANUP_APPROVED
```
