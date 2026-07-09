# Policy Claim Current Validation Baseline Scope Plan

Status: CURRENT_VALIDATION_BASELINE_SCOPE_PLAN_ONLY

Marker:
POLICY_CLAIM_CURRENT_VALIDATION_BASELINE_SCOPE_READY

## 1. Baseline

기준 commit:

`a360002 docs(familyclaimref): consolidate post resource copy cleanup state`

## 2. Purpose

latest baseline에서 build/test를 재검증하고 현재 validation baseline을 문서화한다.

## 3. Prohibited Scope

- code/test/resource 수정
- XAML/ViewModel/resource 수정
- app launch
- OpenFileDialog
- manual workflow
- screenshot
- cleanup
- `data/claimdoc` access
- DB/SQLite/OCR/repository
- git add/stage/commit

## 4. Validation Commands

```text
dotnet build FamilyClaimRef.sln
dotnet test FamilyClaimRef.sln --filter FullyQualifiedName~ResourceUiTextProviderTests
dotnet test FamilyClaimRef.sln --filter FullyQualifiedName~DocumentRegistrationViewModel
dotnet test FamilyClaimRef.sln --filter FullyQualifiedName~PolicyClaimManagementViewModel
dotnet test FamilyClaimRef.sln
```

If a Windows SDK user-profile access boundary appears, the same command may be rerun with permitted elevated execution and the result must be recorded separately.

## 5. Expected Result

- build: PASS
- targeted tests: PASS
- full tests: PASS
- code/test/resource modifications: none
- cleanup/data/DB access: none

## 6. Result Documents

- `docs/279_POLICY_CLAIM_CURRENT_VALIDATION_BASELINE_RESULT_REVIEW.md`
- `docs/280_POLICY_CLAIM_CURRENT_VALIDATION_BASELINE_COMMIT_CANDIDATE_REVIEW.md`
