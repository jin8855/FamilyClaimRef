# Policy/Claim Isolated Runtime Automated Validation Plan

## A. Status

Status: TEST_PLAN_ONLY

Marker:

```text
POLICY_CLAIM_ISOLATED_RUNTIME_AUTOMATED_VALIDATION_PLANNED
```

This document plans automated validation only.

No code is modified by this document.

No test is implemented by this document.

No app launch is authorized by this document.

No cleanup is authorized by this document.

## B. Baseline

- latest commit:
  `e25de59 feat(familyclaimref): add isolated runtime root provider`
- git status before this document:
  clean
- source docs reviewed:
  - `docs/175_POLICY_CLAIM_ISOLATED_RUNTIME_ROOT_DESIGN_REVIEW.md`
  - `docs/177_POLICY_CLAIM_UI_REDESIGN_DEFER_UNTIL_CORE_VALIDATION_DECISION.md`
  - `docs/179_POLICY_CLAIM_RUNTIME_ROOT_PROVIDER_IMPLEMENTATION_SCOPE_PLAN.md`
  - `docs/180_POLICY_CLAIM_RUNTIME_ROOT_PROVIDER_TEST_AND_VALIDATION_PLAN.md`
  - `docs/182_POLICY_CLAIM_RUNTIME_ROOT_PROVIDER_IMPLEMENTATION_RESULT_REVIEW.md`

## C. Purpose

- RuntimeRootProvider unit/composition tests already passed.
- Next validation should prove that actual document registration workflow writes metadata/link/attachments under an isolated runtime root.
- This validation should be automated and should not require WPF app launch.
- Existing `%LOCALAPPDATA%\FamilyClaimRef` evidence must remain untouched.
- UI redesign/localization/wireframe work remains deferred.

## D. Read-Only Source/Test Inspection Summary

Confirmed:

- `AppServices.CreateDefault()` uses `EnvironmentRuntimeRootProvider`.
- `AppServices.Create(IRuntimeRootProvider)` exposes selected `RuntimeRootPath`, `MetadataRootPath`, and `AttachmentRootPath`.
- `AppServices` creates `JsonDocumentStorageService`, `JsonPolicyClaimStorageService`, `LocalFileAttachmentService`, `DocumentAttachmentCoordinator`, `DocumentLinkCoordinator`, and `DocumentRegistrationWorkflow` from the selected roots.
- `MainWindowViewModel` exposes `CreatePolicyAsync`, `CreateClaimAsync`, `LoadAsync`, and `RegisterAsync` without requiring app launch.
- `DocumentRegistrationViewModel.RegisterAsync` invokes `DocumentRegistrationWorkflow`.
- Existing workflow tests already verify policy/claim document registration with temp metadata and attachment roots.
- Existing tests already include project root `attachments/` and `data/local` safety snapshots.

Candidate:

- Future automated validation can use `AppServices.Create(...)` with a test runtime root provider.
- Future automated validation can drive `MainWindowViewModel` or directly compose storage/workflow services under the selected isolated root.
- Direct service/workflow composition gives stronger file-level assertions.
- `MainWindowViewModel` path gives stronger production composition coverage, but needs a fake file picker if file selection is involved.

Unknown:

- There is no existing `Integration` test folder.
- No test currently performs both policy and claim registration through the full selected runtime root in one scenario.

## E. Proposed Automated Test Scope

Recommended future test file:

- `tests/FamilyClaimRef.App.Tests/Integration/IsolatedRuntimeDocumentWorkflowTests.cs`

or another existing test folder that matches repository convention.

Test scenario:

1. Create a unique synthetic isolated runtime root under test temp.
2. Enable:
   - `FAMILYCLAIMREF_ENABLE_DEV_RUNTIME_ROOT_OVERRIDE=1`
   - `FAMILYCLAIMREF_RUNTIME_ROOT=<unique absolute test temp path>`
3. Compose `AppServices` through the same production composition path if feasible.
4. Create synthetic-only policy target.
5. Create synthetic-only claim target.
6. Create synthetic-only source document file under a test-owned temp input directory.
7. Register a policy target document through service/workflow layer.
8. Register a claim target document through service/workflow layer.
9. Verify isolated root contains:
   - `data/local/policies.json`
   - `data/local/claims.json`
   - `data/local/documents.json`
   - `data/local/policy-documents.json`
   - `data/local/claim-documents.json`
   - `attachments/documents/...`
10. Verify project root `attachments/` remains files=0.
11. Verify project root `data/local` remains files=0.
12. Verify `data/claimdoc` is untouched.

## F. Test Data Rules

Allowed:

- synthetic policy title
- synthetic claim title
- synthetic document title
- synthetic `.txt` or `.png` file created inside test-owned temp directory
- synthetic document type values already allowed by current services

Forbidden:

- real policy/contract/insurance/hospital/claim documents
- real personal names
- real family names
- real insurer names
- real hospital names
- real policy numbers
- real claim numbers
- real diagnosis names
- real diagnosis codes
- `data/claimdoc` access

## G. Test Cleanup Boundary

- Test may create and clean only its own unique temp directory.
- Test cleanup must not use broad wildcard deletion.
- Test cleanup must not delete `%TEMP%\FamilyClaimRef`.
- Test cleanup must not delete `%LOCALAPPDATA%\FamilyClaimRef`.
- Test cleanup must not delete project root files.
- Test cleanup must not touch `data/claimdoc`.

## H. Acceptance Criteria

Automated validation is acceptable only if:

- default root behavior remains covered by existing tests.
- override root behavior remains covered by existing tests.
- document registration workflow succeeds under isolated root.
- policy metadata, claim metadata, document metadata, policy links, claim links, and attachments are all under the same isolated root.
- no project root `attachments/` or `data/local` files are created.
- existing `%LOCALAPPDATA%\FamilyClaimRef` evidence is not modified or deleted.
- `data/claimdoc` remains ignored and untouched.
- DB/SQLite/OCR/repository features remain absent.
- `dotnet build` passes.
- `dotnet test` passes.

## I. Out of Scope

- WPF app launch
- OpenFileDialog
- manual registration
- UI/XAML/ViewModel/resource changes
- Korean localization
- wireframe port
- cleanup of existing runtime evidence
- DB/SQLite/OCR/repository implementation

## J. Planned Result Review

Future implementation batch must create:

- `docs/185_POLICY_CLAIM_ISOLATED_RUNTIME_AUTOMATED_VALIDATION_RESULT_REVIEW.md`

## K. Planning Judgment

```text
POLICY_CLAIM_ISOLATED_RUNTIME_AUTOMATED_VALIDATION_PLAN_READY
```
