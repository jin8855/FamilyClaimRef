# DB SQLite OCR Repository Decision Gate Matrix

## A. Status Marker

POLICY_CLAIM_DB_SQLITE_OCR_REPOSITORY_DECISION_GATE_MATRIX_READY

## B. Purpose

This matrix records decision gates for DB, SQLite, OCR, and repository work in `FamilyClaimRef`.

No implementation is authorized by this document.

## C. Gate Matrix

| Area | Current state | Required approval | Required planning docs | Implementation allowed now | Risk |
|---|---|---|---|---|---|
| JSON storage continuation | Current validated baseline uses JSON storage | explicit user approval to change | storage direction decision update | no | medium |
| SQLite adoption | Not implemented and not approved | explicit user approval | SQLite adoption architecture decision | no | high |
| DB schema design | Not implemented and not approved | explicit user approval | schema ownership and migration planning | no | high |
| repository abstraction | Not implemented and not approved | explicit user approval | repository boundary and interface plan | no | high |
| data migration | Not implemented and not approved | explicit user approval | JSON-to-DB migration strategy | no | high |
| OCR planning | Deferred; no OCR implementation approved | explicit user approval | OCR boundary, candidate value, and privacy plan | no | high |
| OCR storage | Deferred; OCR raw text and candidate snapshots not approved | explicit user approval | OCR storage and retention decision | no | high |
| diagnostic summary extraction | Keep deferred until final display model and ownership are approved | explicit user approval | diagnostic display model and ownership decision | no | medium |
| runtime metadata cleanup | Deferred | explicit user approval | exact-target cleanup scope and result plan | no | medium |
| runtime attachment cleanup | Deferred | explicit user approval | exact-target cleanup scope and result plan | no | medium |
| `data/claimdoc` handling | Protected local real-document artifact | no operational use allowed | keep protected unless user gives a separate policy change | no | high |

## D. Required Decisions Before Implementation

| Decision | Required before implementation |
|---|---|
| Keep JSON or introduce SQLite | yes |
| Define repository interface ownership | yes |
| Define migration strategy from existing JSON files | yes |
| Define backup and rollback strategy | yes |
| Define OCR boundary and whether OCR is in MVP scope | yes |
| Define OCR candidate storage and raw text retention | yes |
| Define privacy masking and artifact rules | yes |
| Define test strategy for storage and repository behavior | yes |

## E. Recommended Next Steps

| Track | Recommendation |
|---|---|
| DB/SQLite | Architecture planning only. Do not implement until storage direction and migration policy are approved. |
| Repository | Boundary planning only. Do not add repository code until the storage direction is decided. |
| OCR | Planning only. Do not implement OCR or store OCR raw text until privacy and retention policy are approved. |
| Cleanup | Keep deferred. No project root cleanup candidates are known. |
| `data/claimdoc` | Keep protected. Do not read, list, use, select, stage, commit, delete, or move. |

## F. Gate Result

Implementation allowed now: no for all DB, SQLite, OCR, repository, migration, cleanup, and diagnostic extraction work.

The only allowed next action from this matrix is a separate planning or decision document explicitly approved by the user.
