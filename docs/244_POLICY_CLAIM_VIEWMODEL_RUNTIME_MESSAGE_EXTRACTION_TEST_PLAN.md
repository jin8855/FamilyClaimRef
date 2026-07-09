# Policy Claim ViewModel Runtime Message Extraction Test Plan

## A. Status

```text
TEST_PLAN_ONLY
POLICY_CLAIM_VIEWMODEL_RUNTIME_MESSAGE_EXTRACTION_TEST_PLAN_PLANNED
```

이 문서는 ViewModel runtime message extraction의 향후 검증 범위를 계획한다. 이 문서 자체는 code, test, XAML, ViewModel, resource 파일을 수정하지 않는다.

## B. Future implementation validation targets

1. `UiStrings.xaml` contains approved runtime message keys only
2. `UiTextKeys.cs` contains approved runtime message constants only
3. ViewModel constructor or provider access change is explicit and minimal
4. existing ViewModel behavior remains equivalent
5. exact message string tests are updated only if approved
6. `ResourceUiTextProviderTests` still pass
7. `DocumentRegistrationViewModel` tests still pass
8. `PolicyClaimManagementViewModel` tests still pass
9. full test suite still passes
10. no direct Korean replacement
11. no storage/workflow/data regression

## C. Future build/test commands

```powershell
dotnet build FamilyClaimRef.sln
dotnet test FamilyClaimRef.sln --filter FullyQualifiedName~ResourceUiTextProviderTests
dotnet test FamilyClaimRef.sln --filter FullyQualifiedName~DocumentRegistrationViewModel
dotnet test FamilyClaimRef.sln --filter FullyQualifiedName~PolicyClaimManagementViewModel
dotnet test FamilyClaimRef.sln
```

## D. Existing test impact observed by read-only inspection

| Test file | Current dependency |
|---|---|
| `tests/FamilyClaimRef.App.Tests/DocumentRegistrationViewModelTests.cs` | Multiple tests assert exact `ValidationMessage`, `StatusMessage`, and `TargetSelectionMessage` strings. |
| `tests/FamilyClaimRef.App.Tests/PolicyClaimManagementViewModelTests.cs` | Multiple tests assert exact `ManagementMessage` strings. |
| `tests/FamilyClaimRef.App.Tests/Services/Localization/ResourceUiTextProviderTests.cs` | Provider behavior tests exist and should remain passing after any runtime message key addition. |

## E. Future test update policy

- If ViewModel messages move behind `IUiTextProvider`, tests may need an explicit test provider or resource dictionary fixture.
- Exact string assertions may remain if first implementation values are intentionally unchanged.
- If key lookup is tested instead of literal result, that must be separately approved.
- Constructor null-argument tests must be updated only when a provider dependency is explicitly added.
- `MainWindow.xaml` `policy` / `claim` ComboBox value and `StringFormat=Is busy: {0}` tests are out of scope for this ViewModel runtime message plan.

## F. Forbidden validation

- no app launch
- no screenshot comparison
- no visual automation
- no OpenFileDialog
- no manual workflow
- no exact Korean copy assertion
- no wireframe visual assertion
- no `data/claimdoc`
- no DB/SQLite/OCR/repository

## G. Elevated rerun rule

- Windows SDK user-profile access boundary 실패와 실제 build/test 실패를 구분한다.
- 동일한 환경성 실패가 발생하면 permitted elevated rerun 결과를 별도 기록한다.

## H. Future result review

향후 구현 결과 문서는 다음 파일명을 사용한다.

```text
docs/246_POLICY_CLAIM_VIEWMODEL_RUNTIME_MESSAGE_EXTRACTION_RESULT_REVIEW.md
```

## I. Test plan judgment

```text
POLICY_CLAIM_VIEWMODEL_RUNTIME_MESSAGE_EXTRACTION_TEST_PLAN_READY
```
