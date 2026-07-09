# DB SQLite Architecture Validation Test Plan

## A. Status

DB_SQLITE_ARCHITECTURE_VALIDATION_TEST_PLAN_ONLY

## B. Marker

POLICY_CLAIM_DB_SQLITE_ARCHITECTURE_VALIDATION_TEST_PLAN_READY

## C. Scope

This document defines future validation targets only.

No code/test/XAML/ViewModel/resource is modified by this document.
No DB/SQLite/repository/OCR/migration implementation is approved by this document.

## D. Future Validation Targets

If DB/SQLite/repository implementation is later approved, validation should cover:

1. existing JSON behavior remains equivalent or migration path is explicit
2. repository contract tests cover policy/claim/document persistence
3. migration tests use synthetic-only fixtures
4. DB file location is under approved runtime root only
5. no `data/claimdoc` usage
6. backup/rollback behavior is tested
7. schema versioning is tested
8. OCR raw text/candidate storage remains out of scope unless separately approved
9. cleanup policy remains compatible
10. full test suite passes

## E. Future Build/Test Commands

These commands are future candidates only. They are not run in this batch.

- `dotnet build FamilyClaimRef.sln`
- `dotnet test FamilyClaimRef.sln`

## F. Future Forbidden Validation

Future validation must not use or perform:

- `data/claimdoc`
- real personal/sample data
- app launch unless explicitly approved
- manual workflow unless explicitly approved
- DB file creation outside approved runtime root
- cleanup execution without exact approval
- OCR raw text storage without separate approval
- OCR candidate snapshot storage without separate approval
- package reference addition without separate approval

## G. Result Review Candidate

Future result review candidate:

- `docs/293_POLICY_CLAIM_DB_SQLITE_ARCHITECTURE_DECISION_RESULT_REVIEW.md`

## H. Current Batch Result

- build/test not run
- architecture planning only
- implementation remains blocked

POLICY_CLAIM_DB_SQLITE_ARCHITECTURE_VALIDATION_TEST_PLAN_READY
