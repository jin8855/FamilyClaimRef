# Policy / Claim Scenario 8B Claim Target Execution Instruction

## A. Status Marker

POLICY_CLAIM_SCENARIO8B_CLAIM_TARGET_EXECUTION_INSTRUCTION_CREATED

## B. Purpose

- This document defines the future execution instruction for Scenario 8B claim target synthetic PNG document registration.
- Scenario 8A already confirmed the policy target registration success path.
- Scenario 8B is limited to claim target registration only.
- This documentation task does not launch the app.
- This documentation task does not run OpenFileDialog.
- This documentation task does not create a synthetic PNG.
- This documentation task does not create runtime policy or claim data.
- This documentation task does not run document registration workflow.
- Actual execution is allowed only after this document is created and the user explicitly approves the Scenario 8B approval marker.

## C. Approval Gate

Future execution requires this exact approval marker:

```text
PHASE3D_SCENARIO8B_SYNTHETIC_CLAIM_DOCUMENT_REGISTRATION_APPROVED
```

Approved execution scope:

- app launch
- allowed-extension synthetic PNG creation
- fresh synthetic policy creation
- fresh synthetic claim creation under the synthetic policy
- OpenFileDialog execution
- approved synthetic PNG selection only
- claim target selection
- document registration workflow execution
- runtime copied attachment verification
- `documents.json` sanity check
- `claim-documents.json` sanity check
- project root safety check
- result review document creation

Still forbidden during future execution:

- policy target registration as the primary goal
- Scenario 8A repeat
- actual personal, insurance, hospital, diagnosis, contract, terms, or claim document use
- `data/claimdoc` file use
- `FileNamePolicyService` modification
- allowlist change
- cleanup
- `%LOCALAPPDATA%\FamilyClaimRef` deletion
- project root cleanup
- code/XAML/ViewModel/test modification
- DB/SQLite/OCR/repository implementation
- git add/commit/reset/checkout/clean

## D. Execution Scope

Scenario 8B execution scope:

- claim target only
- fresh synthetic policy/claim pair preferred
- allowed extension synthetic PNG only
- no policy target registration as the primary goal
- no Scenario 8A repeat
- no production code change
- no file allowlist change
- no cleanup
- `data/claimdoc` not used

Expected flow:

1. pre-run source and runtime snapshot
2. build/test baseline
3. confirm or create temp synthetic PNG outside project root
4. app launch
5. create fresh synthetic policy: `policy_title_scenario8b_demo`
6. create fresh synthetic claim under that policy: `claim_title_scenario8b_demo`
7. select synthetic PNG through OpenFileDialog
8. select claim target
9. select existing synthetic-safe document type
10. run document registration
11. verify copied attachment under runtime attachment root
12. verify `documents.json` update
13. verify `claim-documents.json` creation/update
14. verify `policy-documents.json` was not updated by Scenario 8B
15. verify project root remains clean
16. create result review document
17. no cleanup

## E. Current Known State

Based on `docs/157_POLICY_CLAIM_SCENARIO8B_CLAIM_TARGET_SCOPE_DECISION.md`:

- latest commit: `5615736 chore(familyclaimref): ignore local claim documents`
- git status before docs/157: clean
- current uncommitted expected document: `docs/157_POLICY_CLAIM_SCENARIO8B_CLAIM_TARGET_SCOPE_DECISION.md`
- project root `attachments/`: files=0
- project root `data/local`: files=0
- project root `runtime_test_document.*`: missing
- `data/claimdoc` ignored by `/data/claimdoc/`
- `claims.json` missing
- `claim-documents.json` missing
- Scenario 8A artifacts remain under runtime root
- temp `.txt` and `.png` remain under `%TEMP%`
- runtime state is not clean-room

Based on `docs/151_POLICY_CLAIM_SCENARIO8A_ALLOWED_EXTENSION_RETRY_RESULT_REVIEW.md`, Scenario 8A runtime artifacts include:

- `policies.json` exists
- `documents.json` exists and includes Scenario 8A document
- `policy-documents.json` exists and includes Scenario 8A policy-document link
- copied attachment exists under runtime attachment root
- `claims.json` missing
- `claim-documents.json` missing

## F. Synthetic Test Data

Allowed synthetic values:

- `policy_title_scenario8b_demo`
- `claim_title_scenario8b_demo`
- `scenario8b_claim_document_png_demo`

Approved synthetic PNG:

```text
%TEMP%\FamilyClaimRef\runtime_test_document_claim.png
```

File rules:

- minimal valid PNG binary
- allowed extension under current `FileNamePolicyService` policy
- no personal, insurance, hospital, diagnosis, contract, family, or claim content
- no real document image
- no screenshot
- no actual insurance, medical, contract, family, or claim image

Forbidden values or inputs:

- real family names
- real policy numbers
- real claim numbers
- real insurance company names
- real hospital names
- real diagnosis names or diagnosis codes
- real contract, terms, insurance, hospital, or claim documents
- `data/claimdoc` files
- actual user documents

## G. data/claimdoc Exclusion

Required handling:

- `data/claimdoc` is ignored by exact `.gitignore` rule.
- `data/claimdoc` remains a local real-document artifact.
- Scenario 8B does not use, inspect, list, select, stage, commit, delete, or move `data/claimdoc`.
- OpenFileDialog must not navigate to or select files under `data/claimdoc`.
- If `data/claimdoc` appears in workflow input, execution is BLOCKED.
- If `git status --short` shows `data/`, execution is BLOCKED.

Allowed verification:

```powershell
git -c safe.directory=C:/EtcProject/FamilyClaimRef check-ignore -v -- data/claimdoc/
```

Forbidden verification:

- child file path checks under `data/claimdoc`
- `data/claimdoc` file listing
- `data/claimdoc` filename collection
- `data/claimdoc` content inspection

## H. Synthetic PNG Creation Step For Future Approved Execution

This documentation task does not run this step.

Run this only in a future approved Scenario 8B execution.

PowerShell candidate:

```powershell
$scenario8TempRoot = Join-Path $env:TEMP 'FamilyClaimRef'
$scenario8ClaimPngPath = Join-Path $scenario8TempRoot 'runtime_test_document_claim.png'
New-Item -ItemType Directory -Path $scenario8TempRoot -Force | Out-Null

$pngBase64 = 'iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mP8/x8AAwMCAO+/p9sAAAAASUVORK5CYII='
[IO.File]::WriteAllBytes($scenario8ClaimPngPath, [Convert]::FromBase64String($pngBase64))
```

Future execution verification:

- `Test-Path $scenario8ClaimPngPath`
- extension `.png`
- file under `%TEMP%\FamilyClaimRef`
- file not under project root
- git status does not show `runtime_test_document_claim.png`
- file size > 0

## I. Pre-Run Checklist For Future Approved Execution

1. Move to project root:

```powershell
cd C:\EtcProject\FamilyClaimRef
```

2. Confirm source tree state:

```powershell
git -c safe.directory=C:/EtcProject/FamilyClaimRef status --short
git -c safe.directory=C:/EtcProject/FamilyClaimRef log -1 --oneline
```

Expected latest commit before docs/157~158 are committed:

```text
5615736 chore(familyclaimref): ignore local claim documents
```

Expected status before docs/157~158 are committed:

```text
?? docs/157_POLICY_CLAIM_SCENARIO8B_CLAIM_TARGET_SCOPE_DECISION.md
?? docs/158_POLICY_CLAIM_SCENARIO8B_CLAIM_TARGET_EXECUTION_INSTRUCTION.md
```

If docs/157~158 are already committed, expected status is clean.

Stop if:

- `data/` appears
- code/XAML/ViewModel/test changed
- unexpected source tree change appears
- reset/checkout/clean would be needed

3. Confirm ignore rule:

```powershell
git -c safe.directory=C:/EtcProject/FamilyClaimRef check-ignore -v -- data/claimdoc/
```

Expected:

```text
.gitignore:<line>:/data/claimdoc/	data/claimdoc/
```

4. Confirm build/test baseline:

```powershell
dotnet build FamilyClaimRef.sln
dotnet test FamilyClaimRef.sln
```

Notes:

- If Windows SDK permission issues occur, rerun only with explicit approval or record the permission issue in the result review.
- Record whether elevated execution was used.

5. Project root safety pre-check:

- project root `attachments/`: files=0
- project root `data/local`: files=0
- project root `runtime_test_document.*`: missing
- `data/claimdoc` not used

6. Temp file pre-check:

- `%TEMP%\FamilyClaimRef\runtime_test_document.txt`
- `%TEMP%\FamilyClaimRef\runtime_test_document.png`
- `%TEMP%\FamilyClaimRef\runtime_test_document_claim.png`

Notes:

- Previous `.txt` and `.png` are Scenario 8A evidence; do not delete.
- If existing claim PNG is present, confirm it is the approved generated PNG.
- If the existing claim PNG is not the approved generated PNG, stop or record an explicit overwrite decision in the result review.
- No cleanup is performed.

7. Runtime root pre-run snapshot:

Record:

- runtime root exists
- metadata root exists
- attachments root exists
- `policies.json` exists/missing
- `claims.json` exists/missing
- `documents.json` exists/missing
- `policy-documents.json` exists/missing
- `claim-documents.json` exists/missing
- runtime attachment list
- DB/SQLite unexpected file
- actual personal sample targeted scan

## J. Runtime Execution Steps For Future Approved Execution

This documentation task does not execute these steps.

Run these only after the user provides:

```text
PHASE3D_SCENARIO8B_SYNTHETIC_CLAIM_DOCUMENT_REGISTRATION_APPROVED
```

1. Create or confirm synthetic claim PNG:

- `%TEMP%\FamilyClaimRef\runtime_test_document_claim.png`
- approved minimal PNG only
- not created under project root

2. Launch app:

- confirm MainWindow is shown
- confirm Policy/Claim Management section is shown
- confirm Document Registration section is shown

3. Create fresh synthetic policy:

- policy title: `policy_title_scenario8b_demo`
- confirm active policy list reflects it

4. Create fresh synthetic claim:

- parent policy: `policy_title_scenario8b_demo`
- claim title: `claim_title_scenario8b_demo`
- confirm active claim list reflects it
- confirm document registration claim dropdown reflects it

5. Prepare document registration:

- target kind: `claim`
- claim dropdown: select `claim_title_scenario8b_demo`
- document type: `capture` or existing synthetic-safe type
- display title: `scenario8b_claim_document_png_demo`
- reference date: current app default or synthetic-safe value
- run Select File
- in OpenFileDialog, select only `%TEMP%\FamilyClaimRef\runtime_test_document_claim.png`

6. Run document registration:

- run Register
- confirm success message or expected success indicator
- record `LastRegistrationSummary`

7. Close app:

- confirm process exit
- no cleanup

## K. Expected Runtime Artifacts

If Scenario 8B succeeds:

- `policies.json` exists/updated
- `claims.json` exists/updated
- `documents.json` updated with new claim document record
- `claim-documents.json` created/updated with claim-document link
- copied attachment created under `%LOCALAPPDATA%\FamilyClaimRef\attachments\documents`
- copied attachment extension likely `.png`
- `policy-documents.json` should not be updated by Scenario 8B
- temp `.txt` remains
- temp `.png` remains
- temp claim `.png` remains
- project root `attachments/`: files=0
- project root `data/local`: files=0
- project root `runtime_test_document.*`: missing

## L. Stop Criteria

Stop execution if any of the following occurs:

- unexpected source tree change
- `data/` appears in git status
- `data/claimdoc` selection risk
- project root `attachments/` files > 0
- project root `data/local` files > 0
- project root `runtime_test_document.*` created
- temp claim PNG cannot be created
- temp claim PNG invalid or outside `%TEMP%\FamilyClaimRef`
- app startup crash
- fresh policy creation failure
- fresh claim creation failure
- claim target unavailable
- OpenFileDialog selects anything except approved temp claim PNG
- selected file path is not `%TEMP%\FamilyClaimRef\runtime_test_document_claim.png`
- document registration fails
- copied attachment created under project root
- metadata created under project root
- `policy-documents.json` unexpectedly updated for claim target
- `claim-documents.json` missing after claimed success
- DB/SQLite unexpected file created
- actual personal, insurance, hospital, diagnosis, contract, terms, family, or claim sample detected

## M. Result Review Requirement

Required future result review document:

```text
docs/159_POLICY_CLAIM_SCENARIO8B_CLAIM_TARGET_RESULT_REVIEW.md
```

Required result review contents:

- Status Marker:
  - `POLICY_CLAIM_SCENARIO8B_CLAIM_TARGET_EXECUTED`
  - or `POLICY_CLAIM_SCENARIO8B_CLAIM_TARGET_BLOCKED`
- approval marker
- scenario: 8B claim target only
- source status
- build/test baseline
- `git check-ignore` result
- `data/claimdoc` exclusion confirmation
- temp file status
- runtime pre snapshot
- app launch result
- policy creation result
- claim creation result
- claim target selection result
- OpenFileDialog result
- selected file path
- registration result
- runtime post snapshot
- `claims.json` sanity
- `documents.json` sanity
- `claim-documents.json` sanity
- `policy-documents.json` unchanged or delta note
- copied attachment sanity
- project root safety
- DB/SQLite check
- actual personal sample check
- cleanup performed: no
- remaining risks
- next recommendation

## N. Explicit Non-Scope For This Documentation Task

This `docs/158` creation task does not perform:

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
- code/XAML/ViewModel/test modification
- `FileNamePolicyService` modification
- allowlist change
- `data/claimdoc` use, open, listing, selection, stage, commit, delete, or move
- DB/SQLite/OCR/repository implementation
- git add/commit/reset/checkout/clean

## O. Verification For This Documentation Task

After creating `docs/158`, run:

- `git diff --check`
- `git status --short`
- `git check-ignore -v -- data/claimdoc/`
- project root `attachments/` files count
- project root `data/local` files count
- project root `runtime_test_document.*` absence
- DB/SQLite unexpected file check

Build/test:

- not run, documentation-only change

## P. Completion Report Format

```md
POLICY_CLAIM_SCENARIO8B_CLAIM_TARGET_EXECUTION_INSTRUCTION_CREATED

Created document:
- docs/158_POLICY_CLAIM_SCENARIO8B_CLAIM_TARGET_EXECUTION_INSTRUCTION.md

Implementation/execution:
- code modification: none
- XAML modification: none
- ViewModel modification: none
- tests modification: none
- app launch: none
- OpenFileDialog: none
- Scenario 8B execution: none
- synthetic PNG creation: none
- cleanup: none

Execution instruction summary:
- approval marker:
- target type:
- synthetic PNG:
- runtime preparation:
- expected artifacts:
- stop criteria:
- result review document:

Verification:
- git diff --check:
- git status --short:
- git check-ignore:
- project root attachments/: files=<count>
- project root data/local: files=<count>
- project root runtime_test_document.*: missing/exists
- DB/SQLite unexpected file:
- build/test: not run, documentation-only change

Not modified or not performed:
- AppServices modification: none
- DocumentLinkCoordinator modification: none
- DocumentRegistrationWorkflow modification: none
- FileNamePolicyService modification: none
- allowlist change: none
- data/claimdoc file use: none
- runtime artifact deletion: none
- project root cleanup: none
- git add/commit/reset/checkout/clean: none

Next recommended task:
- confirm whether to approve PHASE3D_SCENARIO8B_SYNTHETIC_CLAIM_DOCUMENT_REGISTRATION_APPROVED
```
