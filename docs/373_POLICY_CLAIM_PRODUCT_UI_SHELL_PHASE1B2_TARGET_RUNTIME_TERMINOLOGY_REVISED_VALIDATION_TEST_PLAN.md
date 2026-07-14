# Policy Claim Product UI Shell Phase 1B2 Target Runtime Terminology Revised Validation Test Plan

## A. Status

- Marker: `POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1B2_TARGET_RUNTIME_TERMINOLOGY_REVISED_VALIDATION_TEST_PLAN_READY`
- Current implementation target: 0
- Current batch build/test: not run
- Run this plan only after separate exact implementation approval.

## B. Revised Future Commands

```powershell
dotnet build FamilyClaimRef.sln
dotnet test FamilyClaimRef.sln --filter FullyQualifiedName~DocumentRegistrationViewModelTests
dotnet test FamilyClaimRef.sln --filter FullyQualifiedName~PolicyClaimManagementViewModelTests
dotnet test FamilyClaimRef.sln --filter FullyQualifiedName~ResourceUiTextProviderTests
dotnet test FamilyClaimRef.sln
```

The added targeted command is `PolicyClaimManagementViewModelTests`; it validates preservation of management behavior and consistency of the document-registration fixture.

## C. Future Scope Gates

- Exact modified files: two production/resource files and three test files.
- Exact created file: `docs/369_POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1B2_TARGET_RUNTIME_TERMINOLOGY_IMPLEMENTATION_RESULT_REVIEW.md`.
- Files outside the revised six-file candidate: unchanged.
- Staged files before implementation: none.
- ProductShell runtime entry: absent.
- `ProductDocumentListView`: absent.

## D. Resource and Mirror Gates

- `Ui.*` resources/constants: 67/67.
- `Ui.Product.*` resources/constants: 11/11.
- Resource/constant mismatch: 0.
- New/deleted/renamed keys: 0/0/0.
- Canonical resource value changes: exactly 6.
- Unchanged canonical resource values: 61.
- `AppServices.CreateUiTextProvider()` target-kind fallback matches the Candidate A value.
- `CreateDocumentRegistrationUiTextProvider()` target-kind fixture matches the Candidate A value.
- Other `AppServices` fallback values: unchanged.
- Policy/Claim management message values: unchanged.
- Generic runtime-message changes: 0.

## E. Dependency Scan Gate

Re-run the six exact old-value scans across tracked `app/tests` after implementation.

Expected:

- old-value findings in `app/tests`: 0;
- new values appear only in approved canonical resource, expected assertions, provider dictionary, exact fallback, and exact fixture locations;
- additional or unresolved dependency findings: 0.

## F. Behavior Preservation Gates

- `DocumentRegistrationViewModelTests`: all pass; registration guards and workflow behavior unchanged.
- `PolicyClaimManagementViewModelTests`: all pass; management assertions and behavior unchanged.
- `ResourceUiTextProviderTests`: all pass; provider and inventory behavior unchanged.
- Existing test methods deleted: 0.
- Existing assertions weakened: 0.
- Production composition logic changes: 0.

## G. Test Count Reconciliation

Current known baseline:

- `DocumentRegistrationViewModelTests`: 26/26.
- `ResourceUiTextProviderTests`: 38/38.
- Full solution: 357/357.

Static audit found only five direct exact-value `InlineData` rows for the six Candidate A keys. `DocumentRegistrationValidationSelectTargetKind` is currently covered by `RuntimeMessageKeys`, but not by a direct exact-value row.

Preferred future gate:

- Add one `InlineData` row for `DocumentRegistrationValidationSelectTargetKind` in the existing theory.
- No new test class or test method is required.
- Expected `ResourceUiTextProviderTests`: 39/39.
- Expected full solution total: 358/358.
- The 357 to 358 change is evidence-backed by the added direct exact-value theory case.

If a future implementation directive requires the total to remain 357, it must explicitly approve a count-preserving six-value assertion design before implementation. This documentation batch does not choose or implement an alternate test design.

## H. Static and Safety Gates

- `git diff --check`: PASS.
- Trailing whitespace and EOF: PASS.
- Actual personal/sample/local-user path findings: 0.
- `data/claimdoc/` and `docs/nightwork_20260706/`: remain ignored without internal access.
- Root attachments, `data/local`, runtime test documents, and unexpected DB/SQLite artifacts: 0.

## I. Current Batch Boundary

Build, targeted tests, full tests, app launch, workflow, cleanup, staging, and commit are not run in this documentation-only correction batch.
