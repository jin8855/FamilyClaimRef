# Policy/Claim Remaining Product UI Boundary Decision

## A. Status

Status: DECISION_ONLY

Marker:

POLICY_CLAIM_REMAINING_PRODUCT_UI_BOUNDARY_DECISION_RECORDED

No UI implementation is authorized by this document.

No localization implementation is authorized by this document.

No resource extraction implementation is authorized by this document.

No wireframe port is authorized by this document.

## B. Baseline

Record:

- latest commit:
  224e1da docs(familyclaimref): review document registration viewmodel validation coverage

## C. Boundary Decision

Record:

- Core storage/workflow/runtime validation is now closeable.
- Product UI work must still be separated into a later phase.
- Korean localization must not be done by direct string replacement.
- Resource extraction must precede Korean UI conversion.
- Wireframe port must not be mixed with remaining product-rule decisions.
- Business duplicate final UX remains deferred.

## D. Product Decisions Still Deferred

Record:

1. Whether same source file repeated registration should show warning.
2. Whether same target + document type + display title should show warning.
3. Whether duplicate warning is blocking or non-blocking.
4. Whether business duplicate should become a service-level rule.
5. Korean copy for validation/status messages.
6. Resource key naming and ownership.
7. WPF screen structure aligned to wireframes.
8. Whether current MainWindow remains dev validation harness after product UI shell is introduced.

## E. Recommended UI Entry Sequence

Recommended later sequence:

1. UI phase entry decision document.
2. UI string inventory.
3. Resource extraction plan.
4. Resource infrastructure implementation.
5. Current validation harness string extraction if needed.
6. Wireframe-to-WPF screen mapping.
7. Product shell implementation.
8. Screen-by-screen port.

Do not start at step 7.

## F. Decision Judgment

POLICY_CLAIM_PRODUCT_UI_BOUNDARY_READY
