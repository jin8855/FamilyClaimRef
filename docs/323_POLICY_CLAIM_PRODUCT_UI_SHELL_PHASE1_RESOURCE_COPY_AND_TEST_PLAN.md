# Product UI Shell Phase 1 Resource Copy and Test Plan

## A. Status

PRODUCT_UI_SHELL_PHASE1_RESOURCE_COPY_AND_TEST_PLAN_ONLY

## B. Marker

POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1_RESOURCE_COPY_AND_TEST_READY

## C. Baseline Commit

`1e487c1 docs(familyclaimref): reconcile product ui shell wireframe evidence`

## D. Resource/Copy Boundary

- existing `Ui.*` 56 baseline remains unchanged by this planning batch
- `Ui.Product.*` remains future candidate only
- no `Ui.Product.*` key addition now
- product terminology candidates remain planning only
- validation-harness-only management copy must not be productized without product shell copy table
- final product copy is not finalized by this document

## E. Product Terminology Candidates

| Source term | Candidate Korean copy | Status |
|---|---|---|
| Policy target | 보험 계약 | planning candidate only |
| Claim target | 청구 건 | planning candidate only |

Terminology is not finalized by this batch.

## F. Future Phase 1 Implementation File Candidates

If later approved, a separate implementation plan may consider:

- `ProductShellWindow` XAML file candidate
- ProductShell view model candidate
- Product navigation model candidate
- Document registration product view candidate
- Document list product view candidate
- product shell resource/copy table docs
- tests for product shell navigation/view model

These are future implementation candidates only. No file is created or modified by this planning batch.

## G. Future Test Plan

If a future implementation batch is approved, expected verification may include:

- `dotnet build FamilyClaimRef.sln`
- product shell targeted tests, if new test classes are approved
- `ResourceUiTextProviderTests` if `Ui.Product.*` keys are approved
- `DocumentRegistrationViewModel` tests if reused
- full `dotnet test FamilyClaimRef.sln`

Build/test is not run in this documentation-only planning batch.

## H. Forbidden Future Implementation Unless Separately Approved

- `MainWindow` replacement
- ViewModel behavior changes
- DB/SQLite/repository/OCR/migration implementation
- `data/claimdoc` usage
- app launch/manual workflow/screenshot
- culture/dynamic language switching
- `Ui.Product.*` resource key addition
- product terminology finalization

## I. Resource/Test Judgment

Phase 1 resource and test work is not approved now.

This document only records future candidates and verification expectations for a later exact-scope implementation batch.
