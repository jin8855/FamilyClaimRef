# Policy Claim Validation Harness Management Static XAML Extraction Test Plan

## A. 상태

- Status: TEST_PLAN_ONLY
- Marker:

```text
POLICY_CLAIM_VALIDATION_HARNESS_MANAGEMENT_STATIC_XAML_EXTRACTION_TEST_PLANNED
```

## B. 범위

이번 문서는 test plan만 기록한다.
이 문서 생성 과정에서 code, test, XAML, ViewModel, resource 파일은 수정하지 않는다.

## C. Future Implementation Validation Targets

향후 implementation batch가 승인되면 다음 항목을 검증한다.

1. `UiStrings.xaml` contains approved 14 management static keys.
2. `UiTextKeys.cs` contains approved 14 constants.
3. `MainWindow.xaml` resolves added `StaticResource` references at build time.
4. `ResourceUiTextProviderTests` still pass.
5. Full test suite still passes.
6. `MainWindow.xaml` layout/control hierarchy is not changed.
7. `PolicyClaimManagementViewModel` is not changed.
8. No direct Korean replacement is introduced.
9. No storage/workflow regression is introduced.

## D. Future Build/Test Commands

```powershell
dotnet build FamilyClaimRef.sln
dotnet test FamilyClaimRef.sln --filter FullyQualifiedName~ResourceUiTextProviderTests
dotnet test FamilyClaimRef.sln
```

## E. Forbidden Validation

- no app launch
- no screenshot comparison
- no visual automation
- no OpenFileDialog
- no manual workflow
- no exact Korean copy assertion
- no wireframe visual assertion
- no `data/claimdoc`
- no DB/SQLite/OCR/repository

## F. Future Result Review

향후 구현 결과 문서는 아래 파일에 기록한다.

- `docs/240_POLICY_CLAIM_VALIDATION_HARNESS_MANAGEMENT_STATIC_XAML_EXTRACTION_RESULT_REVIEW.md`

## G. Test Plan Judgment

```text
POLICY_CLAIM_VALIDATION_HARNESS_MANAGEMENT_STATIC_XAML_EXTRACTION_TEST_PLAN_READY
```
