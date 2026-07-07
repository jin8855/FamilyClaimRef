# Policy/Claim RuntimeRootProvider Test and Validation Plan

## A. Status

Status: TEST_PLAN_ONLY

No implementation is performed by this document.

## B. Test Targets

Recommended tests:

1. Default root test:
   - no override env vars
   - selected root equals `LocalApplicationData` + `FamilyClaimRef`

2. Guard-disabled override test:
   - `FAMILYCLAIMREF_RUNTIME_ROOT` is set
   - guard is absent or not `1`
   - selected root remains default

3. Guard-enabled override test:
   - guard is `1`
   - runtime root is absolute synthetic temp path
   - selected root equals override

4. Invalid override test:
   - guard is `1`
   - runtime root is relative or empty
   - provider rejects with clear exception or falls back according to documented behavior

5. Path composition test:
   - metadata root = selected root + `data/local`
   - attachment root = selected root + `attachments`

6. `AppServices` composition test if feasible:
   - `AppServices` uses selected metadata root and attachment root consistently
   - no project root `attachments/` or `data/local` files created

## C. Validation Commands

Allow later implementation batch to run:

- `dotnet build`
- `dotnet test`

Codex must discover the correct solution/test project paths before running.

## D. Forbidden Validation

- app launch
- OpenFileDialog
- document registration workflow
- real document ingestion
- `data/claimdoc` access
- cleanup/deletion
- DB/SQLite/OCR/repository

## E. Result Review Requirement

Future implementation batch must create:

```text
docs/182_POLICY_CLAIM_RUNTIME_ROOT_PROVIDER_IMPLEMENTATION_RESULT_REVIEW.md
```
