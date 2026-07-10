# Product UI Shell Phase 1 Screen Boundary and Navigation Plan

## A. Status

PRODUCT_UI_SHELL_PHASE1_SCREEN_BOUNDARY_AND_NAVIGATION_PLAN_ONLY

## B. Marker

POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1_SCREEN_BOUNDARY_AND_NAVIGATION_READY

## C. Baseline Commit

`1e487c1 docs(familyclaimref): reconcile product ui shell wireframe evidence`

## D. Shell Strategy

- keep `MainWindow` as validation harness
- add separate `ProductShellWindow` only in a future implementation batch, if approved
- do not replace `MainWindow` in Phase 1
- keep validation harness navigation separate from product shell navigation
- avoid hard-coding product shell behavior into validation `MainWindow`

## E. Phase 1 Screen Boundary Table

| Screen candidate | Phase 1 status | Source evidence status | Implementation allowed now | Notes |
|---|---|---|---|---|
| Product navigation shell | Phase 1 candidate | Source-confirmed final target | no | product shell navigation candidate only |
| Home / dashboard | Phase 1 candidate | Source-confirmed final target | no | product entry screen candidate only |
| Document registration product view | Phase 1 candidate | Source-confirmed final target | no | may reuse or wrap existing `DocumentRegistrationViewModel` later |
| Document list view | Phase 1 candidate | Source-confirmed final target | no | basic list display candidate only |
| standalone Document detail | final target, not Phase 1 exact target | User-scope-confirmed final target, needs source detail | no | source detail confirmation required |
| Settings | final target, not Phase 1 exact target | User-scope-confirmed final target, needs source detail | no | product settings source confirmation required |
| Validation harness | not product shell | Validation harness only | no | keep separate |
| Management harness | not product shell | Validation harness only | no | do not productize harness management copy |
| OCR candidate review | future-only | Future-only | no | requires OCR/privacy/storage approval |

## F. Navigation Candidate Plan

- no navigation implementation now
- future navigation should support Phase 1 screens without blocking Phase 2 extension
- avoid hard-coding product shell in validation `MainWindow`
- do not introduce DB/search/OCR dependency for Phase 1 navigation
- keep Phase 1 navigation compatible with JSON source of truth
- keep document registration and document list as separate product areas unless a later implementation plan decides otherwise

## G. Open Questions

| Question | Status |
|---|---|
| exact `ProductShellWindow` file location | Needs future implementation decision |
| navigation ViewModel ownership | Needs future implementation decision |
| whether document registration product view reuses `DocumentRegistrationViewModel` or wraps it | Needs future implementation decision |
| document list data source and filtering boundary | Needs future implementation decision |
| product terminology for Policy/Claim | Planning only, not finalized |

## H. Boundary Judgment

Phase 1 may later start from source-confirmed/core screens only. That later work still requires explicit approval and an exact file list.

No screen or navigation implementation is authorized by this document.
