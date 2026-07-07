# Policy/Claim Isolated Runtime Manual Validation Execution Instruction

## A. Status

Status: INSTRUCTION_ONLY

No execution is performed by this document.

No app launch is performed by this document.

No OpenFileDialog is performed by this document.

No workflow execution is performed by this document.

No synthetic file is created by this document.

No cleanup is performed by this document.

## B. Required Approval Marker

Future execution requires the user to provide this exact marker:

```text
POLICY_CLAIM_ISOLATED_RUNTIME_MANUAL_VALIDATION_APPROVED
```

Without this marker, do not launch the app and do not run Scenario 9.

## C. Preflight For Future Execution

Before any future execution, verify and record:

- `git status --short` is clean.
- latest commit is the expected commit for the approved execution batch.
- no running `FamilyClaimRef` app process exists.
- `data/claimdoc` is ignored and untouched.
- project root `attachments/` file count is 0.
- project root `data/local` file count is 0.
- project root `runtime_test_document.*` file count is 0.
- existing `%LOCALAPPDATA%/FamilyClaimRef` evidence is checked by existence/count only.
- existing `%LOCALAPPDATA%/FamilyClaimRef` metadata file content is not opened.
- existing `%LOCALAPPDATA%/FamilyClaimRef` attachment file content is not opened.
- isolated root is absent or empty before launch.

Future execution environment variables:

```text
FAMILYCLAIMREF_ENABLE_DEV_RUNTIME_ROOT_OVERRIDE=1
FAMILYCLAIMREF_RUNTIME_ROOT=<isolated absolute path>
```

The isolated absolute path must be under:

```text
%TEMP%/FamilyClaimRef-Isolated/
```

Do not use `%LOCALAPPDATA%/FamilyClaimRef`, project root, or `data/claimdoc` as an isolated runtime root.

## D. Execution Steps

Only after explicit future approval:

1. Create synthetic input files under:
   `%TEMP%/FamilyClaimRef-IsolatedInputs/scenario9`
2. Launch the app with the isolated runtime environment variables in the same process environment.
3. Confirm the app opens against the isolated runtime root.
4. Create synthetic policy target:
   `policy_title_scenario9_isolated_demo`
5. Create synthetic claim target:
   `claim_title_scenario9_isolated_demo`
6. Register policy synthetic document.
7. Register claim synthetic document.
8. Record UI status messages.
9. Close app after validation.

Synthetic input content must contain no real personal, insurance, hospital, diagnosis, claim, contract, or terms data.

## E. Post Validation Checks

After future execution, verify and record:

- isolated root `data/local/policies.json` exists.
- isolated root `data/local/claims.json` exists.
- isolated root `data/local/documents.json` exists.
- isolated root `data/local/policy-documents.json` exists.
- isolated root `data/local/claim-documents.json` exists.
- isolated root `attachments/documents` file count is expected for the run.
- `%LOCALAPPDATA%/FamilyClaimRef` metadata is preserved.
- `%LOCALAPPDATA%/FamilyClaimRef` attachment count is preserved.
- project root `attachments/` files remain 0.
- project root `data/local` files remain 0.
- project root `runtime_test_document.*` files remain 0.
- no DB/SQLite unexpected files exist in safe locations.
- `data/claimdoc` remains untouched.

## F. Expected Result Review

Expected future result document:

- `docs/189_POLICY_CLAIM_ISOLATED_RUNTIME_MANUAL_VALIDATION_RESULT_REVIEW.md`

The result review must record:

- approval marker used
- launch command or method
- isolated root used, sanitized if needed
- synthetic file names
- policy and claim synthetic titles
- metadata/link/attachment checks
- default runtime evidence preservation
- project root safety checks
- cleanup decision, if any
