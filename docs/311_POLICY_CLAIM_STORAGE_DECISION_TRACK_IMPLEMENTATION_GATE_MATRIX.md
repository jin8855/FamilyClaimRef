# Policy Claim Storage Decision Track Implementation Gate Matrix

## A. Status

STORAGE_DECISION_TRACK_IMPLEMENTATION_GATE_MATRIX

## B. Marker

POLICY_CLAIM_STORAGE_DECISION_TRACK_IMPLEMENTATION_GATE_MATRIX_READY

## C. Gate Matrix

| Work item | Current status | Implementation allowed now | Required before implementation | Risk |
|---|---|---|---|---|
| JSON baseline continuation | Existing baseline only | allowed as existing baseline only | no storage direction change | low |
| SQLite implementation | Not approved | no | explicit SQLite implementation scope, package policy, schema policy, and validation plan | high |
| repository implementation | Not approved | no | explicit repository interface/class boundary and migration strategy | high |
| migration implementation | Not approved | no | explicit migration scope, source/target version policy, synthetic-only tests | high |
| backup/rollback implementation | Not approved | no | explicit backup file policy, rollback target, integrity checks, and failure handling | high |
| OCR implementation | Not approved | no | explicit OCR provider/API/package approval and privacy boundary | high |
| OCR raw text storage | Not approved | no | explicit retention, masking, deletion, and privacy approval | high |
| OCR candidate snapshot storage | Not approved | no | explicit candidate ownership, retention, masking, and user confirmation policy | high |
| DB file creation | Not approved | no | explicit database file location, lifecycle, backup, and test approval | high |
| package reference addition | Not approved | no | exact package name/version and project file modification approval | medium |
| data/claimdoc usage | Protected local real-document artifact | no | separate policy change from user; current state is no operational use | high |
| cleanup execution | Dry-run found no project root candidates | no | exact cleanup target approval and pre/post evidence | high |
| diagnostic summary extraction | Keep deferred | no | final display model and diagnostic ownership approval | medium |
| UI redesign | Deferred | no | explicit UI redesign scope and product validation plan | high |
| product UI shell | Not approved | no | explicit product UI shell scope and navigation/resource plan | high |

## D. Closure Judgment

- No implementation track is opened by this current-state closure.
- Any future implementation must start from a separate user-approved exact-scope batch.
- The safest next state is stop/handoff unless the user explicitly selects one planning-only track.

## E. Allowed Language

- current-state closure
- existing JSON baseline
- implementation remains blocked
- explicit user approval required
- planning-only track
- stop/handoff recommended

## F. Blocked Direction

The following remain blocked:

- SQLite implementation
- repository implementation
- migration implementation
- backup/rollback implementation
- OCR implementation/storage
- package reference addition
- JSON storage replacement
- DB file creation
- `data/claimdoc` operational use
- cleanup execution
- diagnostic summary extraction
- UI redesign/product UI shell
