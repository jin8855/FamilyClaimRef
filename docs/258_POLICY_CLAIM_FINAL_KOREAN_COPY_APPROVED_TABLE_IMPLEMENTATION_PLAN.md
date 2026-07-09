# Policy Claim Final Korean Copy Approved Table Implementation Plan

## A. Status

```text
FINAL_KOREAN_COPY_APPROVED_TABLE_IMPLEMENTATION_PLAN_ONLY
POLICY_CLAIM_FINAL_KOREAN_COPY_APPROVED_TABLE_IMPLEMENTATION_PLAN_PLANNED
```

## B. Baseline

```text
1036fba docs(familyclaimref): draft final korean copy candidate table
```

## C. Purpose

`docs/257_POLICY_CLAIM_FINAL_KOREAN_COPY_APPROVED_VALUE_TABLE.md`의 approved value table을 향후 implementation batch에서 어떻게 반영할지 계획한다.

이번 batch에서는 implementation을 수행하지 않는다.

## D. Future Implementation Target

향후 implementation 대상 후보:

- `UiStrings.xaml` value 변경: approved implementation target yes rows 21개만
- `ResourceUiTextProviderTests` expected value update
- `DocumentRegistrationViewModelTests` exact string expected value update
- `docs/251_POLICY_CLAIM_FINAL_KOREAN_COPY_STRATEGY_RESULT_REVIEW.md` 생성

## E. Future Implementation Non-Targets

향후 implementation에서도 별도 승인 없이는 수정하지 않는다.

- `UiTextKeys.cs` key rename
- `MainWindow.xaml`
- ViewModel behavior
- `App.xaml`
- `IUiTextProvider.cs`
- `ResourceUiTextProvider.cs`
- `AppServices.cs`
- `PolicyClaimManagementViewModelTests`, unless build/test proves exact current value dependency exists
- culture switching
- dynamic language switching
- DB/SQLite/OCR/repository
- `data/claimdoc`
- cleanup

## F. Expected Future Implementation Exact File Candidates

- `app/FamilyClaimRef.App/Resources/UiStrings.xaml`
- `tests/FamilyClaimRef.App.Tests/Services/Localization/ResourceUiTextProviderTests.cs`
- `tests/FamilyClaimRef.App.Tests/DocumentRegistrationViewModelTests.cs`
- `docs/251_POLICY_CLAIM_FINAL_KOREAN_COPY_STRATEGY_RESULT_REVIEW.md`

## G. Expected Future Validation Commands

```powershell
dotnet build FamilyClaimRef.sln
dotnet test FamilyClaimRef.sln --filter FullyQualifiedName~ResourceUiTextProviderTests
dotnet test FamilyClaimRef.sln --filter FullyQualifiedName~DocumentRegistrationViewModel
dotnet test FamilyClaimRef.sln --filter FullyQualifiedName~PolicyClaimManagementViewModel
dotnet test FamilyClaimRef.sln
```

## H. Expected Future Count Checks

| Check | Expected |
|---|---:|
| `UiStrings.xaml` value changes | 21 |
| `UiTextKeys.cs` key name changes | 0 |
| New `Ui.*` keys | 0 |
| Deleted `Ui.*` keys | 0 |
| Keep current rows changed | 0 |
| Excluded resource rows changed | 0 |
| Deferred/non-resource rows changed | 0 |

## I. Future Commit Message Candidate

```text
refactor(familyclaimref): apply approved korean resource copy
```

## J. Current Batch Non-Scope

- no implementation
- no resource value changes
- no source/test changes
- no build/test execution
- no app launch
- no workflow execution
- no `data/claimdoc` access
- no DB/SQLite/OCR/repository implementation
- no git add/stage/commit

## K. Final Marker

```text
POLICY_CLAIM_FINAL_KOREAN_COPY_APPROVED_TABLE_IMPLEMENTATION_PLAN_READY
```
