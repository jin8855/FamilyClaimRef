# Policy/Claim Validation Harness Next Static XAML Extraction Test Plan

## A. Status

Status: TEST_PLAN_ONLY

Marker:

POLICY_CLAIM_VALIDATION_HARNESS_NEXT_STATIC_XAML_EXTRACTION_TEST_PLANNED

No test is implemented by this document.

No code is modified by this document.

No XAML is modified by this document.

## B. Baseline

Record:

- latest commit:
  478e6cd refactor(familyclaimref): extract validation harness pilot strings

## C. Future Test / Validation Targets

For the future implementation, validate:

1. UiStrings.xaml contains all planned keys.
2. UiTextKeys.cs contains all planned constants.
3. MainWindow.xaml resolves all added StaticResource references at build time.
4. ResourceUiTextProviderTests still pass.
5. Full test suite still passes.
6. MainWindow layout is not intentionally changed.
7. No direct Korean replacement occurs.
8. No ViewModel tests regress.
9. No storage/workflow tests regress.

## D. Future Build/Test Commands

Future implementation batch may run:

- dotnet build FamilyClaimRef.sln
- dotnet test FamilyClaimRef.sln --filter FullyQualifiedName~ResourceUiTextProviderTests
- dotnet test FamilyClaimRef.sln

If XAML compile errors occur, fix only within approved static XAML resource binding scope.

## E. Forbidden Validation

Record:

- no app launch
- no screenshot comparison
- no visual automation
- no OpenFileDialog
- no manual workflow
- no exact Korean copy assertion
- no wireframe visual assertion
- no data/claimdoc
- no DB/SQLite/OCR/repository

## F. Future Result Review Requirement

Future implementation batch must create:

- docs/235_POLICY_CLAIM_VALIDATION_HARNESS_NEXT_STATIC_XAML_EXTRACTION_RESULT_REVIEW.md

## G. Test Plan Judgment

POLICY_CLAIM_VALIDATION_HARNESS_NEXT_STATIC_XAML_EXTRACTION_TEST_PLAN_READY
