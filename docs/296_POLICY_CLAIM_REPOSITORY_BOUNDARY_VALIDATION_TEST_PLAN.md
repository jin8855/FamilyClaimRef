# Repository Boundary Validation Test Plan

## A. Status

REPOSITORY_BOUNDARY_VALIDATION_TEST_PLAN_ONLY

## B. Marker

POLICY_CLAIM_REPOSITORY_BOUNDARY_VALIDATION_TEST_PLAN_READY

## C. Scope Statement

- no code/test/XAML/ViewModel/resource modified by this document
- no repository/DB/SQLite/OCR/migration implementation by this document
- no package reference addition by this document
- no `data/claimdoc` operational use by this document

## D. Future Validation Targets

If repository boundary implementation is later approved, validation should cover:

1. existing JSON storage behavior remains equivalent
2. storage service tests remain passing
3. repository contract tests use synthetic-only fixtures
4. repository implementation does not use `data/claimdoc`
5. workflow behavior remains in workflow/ViewModel layer, not repository
6. repository does not create DB files unless DB track is separately approved
7. repository interfaces do not rename existing storage keys or resource keys
8. full test suite passes

## E. Future Build/Test Commands

```powershell
dotnet build FamilyClaimRef.sln
dotnet test FamilyClaimRef.sln
```

These commands are future validation commands only. They are not run by this documentation-only planning batch.

## F. Future Forbidden Validation

- no `data/claimdoc`
- no real personal/sample data
- no DB file creation unless DB track approved
- no OCR raw text/candidate storage
- no cleanup execution
- no app launch unless explicitly approved

## G. Contract Test Candidate Areas

| Candidate area | Future check | Approved now |
|---|---|---|
| storage equivalence | JSON storage results match repository results | no |
| policy lifecycle | create, reload, disable behavior remains equivalent | no |
| claim lifecycle | create, reload, disable behavior remains equivalent | no |
| document links | policy/claim document link behavior remains equivalent | no |
| rollback boundary | workflow rollback remains outside repository | no |
| query projection | read-only search/index behavior, if approved | no |
| migration compatibility | JSON-to-future-store contract, if approved | no |

## H. Result Review Candidate

`docs/298_POLICY_CLAIM_REPOSITORY_BOUNDARY_DECISION_RESULT_REVIEW.md`

## I. Validation Judgment

Repository boundary validation remains a future candidate. Current validated baseline remains JSON storage service behavior under the existing service interfaces.

