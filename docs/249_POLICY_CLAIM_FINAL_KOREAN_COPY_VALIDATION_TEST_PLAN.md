# Policy Claim Final Korean Copy Validation Test Plan

## A. Status

```text
TEST_PLAN_ONLY
POLICY_CLAIM_FINAL_KOREAN_COPY_VALIDATION_TEST_PLAN_PLANNED
```

## B. Scope

- no code/test/XAML/ViewModel/resource modified by this document
- no resource value change is authorized by this document
- final Korean copy must be approved through a separate copy table before implementation

## C. Future implementation validation targets

1. `UiStrings.xaml` updates only approved final Korean copy values
2. `UiTextKeys.cs` key names remain unchanged unless separately approved
3. no key deletion
4. no static XAML binding change
5. no ViewModel behavior change
6. no direct Korean replacement in XAML/ViewModel
7. `ResourceUiTextProviderTests` pass
8. ViewModel exact string tests updated only if copy decision is approved
9. full test suite passes
10. no storage/workflow/data regression
11. `data/claimdoc` untouched
12. DB/SQLite/OCR/repository untouched

## D. Future build/test commands

```text
dotnet build FamilyClaimRef.sln
dotnet test FamilyClaimRef.sln --filter FullyQualifiedName~ResourceUiTextProviderTests
dotnet test FamilyClaimRef.sln --filter FullyQualifiedName~DocumentRegistrationViewModel
dotnet test FamilyClaimRef.sln --filter FullyQualifiedName~PolicyClaimManagementViewModel
dotnet test FamilyClaimRef.sln
```

## E. Future copy validation

- approved copy table must exist before implementation
- product-facing and harness-only ownership must be separated
- current Korean source literal must not be treated as newly written copy
- final Korean copy must not be inferred from source without approval
- English-to-Korean value replacement must be exact-file-list implementation only

## F. Forbidden validation

- no app launch
- no screenshot comparison
- no visual automation
- no OpenFileDialog
- no manual workflow
- no `data/claimdoc`
- no DB/SQLite/OCR/repository
- no cleanup

## G. Elevated rerun rule

- Windows SDK user-profile access boundary 실패와 실제 build/test 실패를 구분한다.
- 동일한 환경성 실패가 발생하면 permitted elevated rerun 결과를 별도 기록한다.

## H. Future result review

```text
docs/251_POLICY_CLAIM_FINAL_KOREAN_COPY_STRATEGY_RESULT_REVIEW.md
```

## I. 판단 marker

```text
POLICY_CLAIM_FINAL_KOREAN_COPY_VALIDATION_TEST_PLAN_READY
```
