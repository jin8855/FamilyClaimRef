# Policy/Claim Validation Harness Pilot Extraction Test Plan

## A. Status

Status: TEST_PLAN_ONLY

Marker:

POLICY_CLAIM_VALIDATION_HARNESS_PILOT_EXTRACTION_TEST_PLANNED

No test is implemented by this document.

No code is modified by this document.

No XAML is modified by this document.

## B. Baseline

Record:

- latest commit:
  14f0541 feat(familyclaimref): add ui resource infrastructure

## C. Future Test / Validation Targets

For the future pilot extraction implementation, validate:

1. App.xaml still compiles after resource usage.
2. MainWindow.xaml can resolve pilot resource keys at build time.
3. ResourceUiTextProviderTests still pass.
4. Full test suite still passes.
5. No ViewModel tests regress.
6. No storage/workflow tests regress.
7. MainWindow layout is not intentionally changed.
8. No Korean direct replacement occurs.
9. No new resource keys are required unless separately approved.

## D. Future Build/Test Commands

Future implementation batch may run:

- dotnet build FamilyClaimRef.sln
- dotnet test FamilyClaimRef.sln
- dotnet test FamilyClaimRef.sln --filter FullyQualifiedName~ResourceUiTextProviderTests

If XAML compile errors occur, fix only within the approved pilot resource binding scope.

## E. Forbidden Validation

Record:

- no app launch
- no screenshot comparison
- no visual automation
- no OpenFileDialog
- no manual workflow
- no exact product copy assertion
- no Korean final copy assertion
- no wireframe visual assertion
- no data/claimdoc
- no DB/SQLite/OCR/repository

## F. Future Result Review Requirement

Future implementation batch must create:

- docs/230_POLICY_CLAIM_VALIDATION_HARNESS_PILOT_STRING_EXTRACTION_RESULT_REVIEW.md

## G. Test Plan Judgment

POLICY_CLAIM_VALIDATION_HARNESS_PILOT_EXTRACTION_TEST_PLAN_READY
