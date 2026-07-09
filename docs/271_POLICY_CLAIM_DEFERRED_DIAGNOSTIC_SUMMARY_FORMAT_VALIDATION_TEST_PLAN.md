# Policy Claim Deferred Diagnostic Summary Format Validation Test Plan

## 1. Status

DIAGNOSTIC_SUMMARY_FORMAT_VALIDATION_TEST_PLAN_ONLY

## 2. Marker

POLICY_CLAIM_DEFERRED_DIAGNOSTIC_SUMMARY_FORMAT_VALIDATION_TEST_PLAN_PLANNED

## 3. Scope Boundary

- no code/test/XAML/ViewModel/resource modified by this document.
- no diagnostic summary format extraction by this document.
- no resource key added by this document.
- no final display model decided by this document.
- no cleanup executed by this document.
- no `data/claimdoc` access by this document.
- no DB/SQLite/OCR/repository implementation by this document.

## 4. Future Implementation Validation Targets

1. placeholder contract preserved.
2. policy summary still includes `policyId` and `documentId` if diagnostic format retained.
3. claim summary still includes `claimId` and `documentId` if diagnostic format retained.
4. no user-facing copy regression.
5. no storage/workflow behavior change.
6. `DocumentRegistrationViewModelTests` updated only if approved.
7. `ResourceUiTextProviderTests` updated only if keys are approved.
8. full test suite passes.
9. `data/claimdoc` untouched.
10. DB/SQLite/OCR/repository untouched.

## 5. Future Build/Test Commands

```text
dotnet build FamilyClaimRef.sln
dotnet test FamilyClaimRef.sln --filter FullyQualifiedName~DocumentRegistrationViewModel
dotnet test FamilyClaimRef.sln --filter FullyQualifiedName~ResourceUiTextProviderTests
dotnet test FamilyClaimRef.sln
```

## 6. Forbidden Validation

- no app launch.
- no OpenFileDialog.
- no manual workflow.
- no screenshot/visual automation.
- no `data/claimdoc`.
- no DB/SQLite/OCR/repository.
- no cleanup.
- no final Korean copy without explicit approval.
- no product UI shell without explicit approval.

## 7. Future Result Review Candidate

- `docs/273_POLICY_CLAIM_DEFERRED_DIAGNOSTIC_SUMMARY_FORMAT_RESULT_REVIEW.md`

이 문서는 future exact implementation batch 이후에만 생성 후보가 된다. 이번 planning batch에서는 생성하지 않는다.

## 8. Test Plan Judgment

POLICY_CLAIM_DEFERRED_DIAGNOSTIC_SUMMARY_FORMAT_VALIDATION_TEST_PLAN_READY
