# Policy/Claim Business Duplicate Validation Boundary Review

## A. Status

Status: BOUNDARY_REVIEW_ONLY

## B. Confirmed Existing Coverage

Record:

- negative validation covers supported rejection paths.
- attachment duplicate/collision validation covers physical filename collision.
- duplicate-index exhaustion is covered.
- duplicate active policy link rejection is covered.
- duplicate active claim link rejection is covered.
- repeated registration currently creates distinct document ids when workflow permits it.

## C. Boundary

Record:

Covered as core storage/workflow safety:

- no overwrite on physical filename collision
- duplicate-index deterministic retry
- duplicate active link rejection
- rollback on link-stage failures
- disabled target rejection

Not covered as core safety:

- whether same source file should be blocked
- whether same title/type/target should be blocked
- whether UI should warn about possible duplicates
- whether Korean copy should explain duplicates

## D. Risk

Record:

- Adding duplicate rejection now would mix product semantics into storage validation.
- UI warning requires localization/resource extraction, which is deferred.
- Current validation harness should remain unchanged.
- Business duplicate semantics should be revisited before product UI work.

## E. Recommendation

Record:

- Keep current workflow semantics for now.
- Continue feature validation without adding business duplicate rejection.
- Revisit business duplicate UX during product UI/resource phase.
- If later approved, create a separate product-rule design document.
