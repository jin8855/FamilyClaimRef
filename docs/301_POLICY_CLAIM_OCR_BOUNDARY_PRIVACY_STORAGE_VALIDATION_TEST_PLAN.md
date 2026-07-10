# OCR Boundary Privacy Storage Validation Test Plan

## A. Status

OCR_BOUNDARY_PRIVACY_STORAGE_VALIDATION_TEST_PLAN_ONLY

## B. Marker

POLICY_CLAIM_OCR_BOUNDARY_PRIVACY_STORAGE_VALIDATION_TEST_PLAN_READY

## C. Scope Statement

- no code/test/XAML/ViewModel/resource modified by this document
- no OCR/DB/SQLite/repository/migration implementation by this document
- no OCR package/API/provider addition by this document
- no raw OCR text storage by this document
- no OCR candidate snapshot storage by this document
- no `data/claimdoc` operational use by this document

## D. Future Validation Targets

If OCR work is later approved, validation should cover:

1. OCR uses synthetic-only fixtures
2. no `data/claimdoc` input
3. raw OCR text persistence is off unless separately approved
4. OCR candidate storage is exact-scope approved before implementation
5. masking/redaction is tested before storage
6. retention/deletion policy is tested
7. runtime root location is approved and tested
8. no DB/SQLite dependency unless DB track is separately approved
9. no product UI shell dependency unless product UI shell is approved
10. full test suite passes

## E. Future Build/Test Commands

```powershell
dotnet build FamilyClaimRef.sln
dotnet test FamilyClaimRef.sln
```

These commands are future validation commands only. They are not run by this documentation-only OCR planning batch.

## F. Future Forbidden Validation

- no `data/claimdoc`
- no real personal/sample data
- no real insurer/hospital/diagnosis/policy/claim number samples
- no cloud OCR/API unless explicitly approved
- no DB file creation unless DB track approved
- no raw OCR text storage unless privacy policy approved
- no cleanup execution without exact approval
- no app launch unless explicitly approved

## G. Future Test Areas

| Future area | Validation expectation | Approved now |
|---|---|---|
| input boundary | synthetic-only OCR fixture source | no |
| provider boundary | selected provider/API is explicit and local/cloud policy is approved | no |
| raw text handling | discarded by default or stored only after explicit policy approval | no |
| candidate values | separated from raw text and user-confirmed before business use | no |
| confidence/provenance | retained only after provenance policy is approved | no |
| masking/redaction | tested before any persisted OCR artifact | no |
| retention/deletion | exact runtime root and cleanup policy tested | no |
| DB/SQLite relation | no dependency unless DB track is approved | no |

## H. Result Review Candidate

`docs/303_POLICY_CLAIM_OCR_BOUNDARY_PRIVACY_STORAGE_DECISION_RESULT_REVIEW.md`

## I. Validation Judgment

OCR validation remains future-only. Current validation baseline remains the non-OCR JSON storage and document registration behavior.

