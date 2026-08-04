# Gate 8 R07 Preflight Lifetime Repair and Conditional Final Runtime Recheck

## 1. Status

`POLICY_CLAIM_PRODUCT_UI_SHELL_GATE8_R07_PREFLIGHT_LIFETIME_REPAIR_PASS_PRODUCT_RUNTIME_RECHECK_HOLD`

- Judgment: `HOLD`
- Corrected TEMP preflight: `PASS`
- Corrected preflight execution count: `1`
- Conditional Product launch count: `1`
- Product R01-R09: `NOT_EXECUTED`
- Primary blocker: `HOLD_PRODUCT_TOP_LEVEL_WINDOW_UNAVAILABLE_PROCESS_RESIDUE`
- Guarded runtime functional review: `NOT_COMPLETED`
- User visual acceptance: `NOT_AVAILABLE_AS_NEXT_GATE`
- Final Gate 8 implementation: `HOLD_RUNTIME_REVIEW_REQUIRED`
- Deployment/production readiness: `NOT_AUTHORIZED`
- Stage/commit: `NOT_AUTHORIZED`

## 2. Baseline

| Item | Observed value | Result |
|---|---|---|
| Branch | `main` | PASS |
| HEAD | `aecf7edfd43b4124ec5ff17d35687020cf4c0d90` | PASS |
| Subject | `docs(familyclaimref): record gate8 registration persistence decision package` | PASS |
| Initial tracked/staged/untracked | `27/0/15` | PASS |
| Initial status entries | `42` | PASS |
| Existing exact path set | `42/42` | PASS |
| Missing/extra | `0/0` | PASS |
| `docs/427` preexistence | `0` | PASS |
| `docs/426` SHA-256 | `7ee4b05f86159f1bc1a0d75bc9044cfa6e3037fa66e224c87491ae76b0b10f13` | PASS |

Binary identity immediately before the conditional Product run:

| Artifact | Bytes | SHA-256 | Result |
|---|---:|---|---|
| EXE | 162816 | `bb3de775939bbeb06aa9abe42e9e93cee51881084b3e6f20e7293a2d23300c39` | PASS |
| DLL | 294912 | `4534420c13f4f01b80263a73d3a3c71bbe3ce1c0c01836e3b069a6ce218e1f3f` | PASS |

Build and tests were not run because this batch prohibited them.

## 3. Corrected TEMP Preflight

Logical run root:

`%TEMP%\FamilyClaimRef\Gate8RuntimeReview\gate8-lifetime-final-20260727-172646-29f116c9`

The TEMP WPF host was structurally repaired without changing Product source:

- the host object owned its `DispatcherTimer` in a private field
- the host object owned `invokeCount`
- the constructor created the timer and attached one Tick handler
- the Button handler disabled the Button and started the owned timer
- the Tick handler stopped the timer, set `Invoked:1`, and restored the Button
- Window close stopped the timer and detached handlers
- callback-local timer capture was not used

Static preflight checks:

- PowerShell parser: PASS
- C# host compile: PASS
- C# observer compile: PASS
- forbidden action scan: PASS
- Product process count before preflight: `0`

Single corrected preflight result:

| Gate | Observed value | Result |
|---|---|---|
| unique live Button candidate | `1` | PASS |
| initial `IsEnabled` | `true` | PASS |
| object-based `TryGetCurrentPattern` | `true` | PASS |
| returned object | `InvokePattern` | PASS |
| semantic Invoke count | `1` | PASS |
| final UI state | `Invoked:1` | PASS |
| first disabled observation | monotonic ordering after action | PASS |
| first restored observation | monotonic ordering after disabled | PASS |
| selected proof route | `POLLING` | PASS |
| disabled interval | `258.5273 ms` | PASS |
| event/polling exception count | `0/0` | PASS |
| action calls from observers | `0` | PASS |
| blind/coordinate/direct-message action count | `0/0/0` | PASS |
| timer callback count | `1` | PASS |
| timer callback exception count | `0` | PASS |
| timer stopped / handlers detached | `true/true` | PASS |
| pending callback count | `0` | PASS |
| preflight process residue | `0` | PASS |

Preflight marker:

`GATE8_R07_PREFLIGHT_LIFETIME_REPAIR_PASS`

The corrected preflight was not repeated.

## 4. Conditional Product Run

The complete preflight PASS authorized one conditional Product run.

Before launch:

- TEMP Product harness PowerShell parser: PASS
- TEMP UIA helper C# compile under Windows PowerShell: PASS
- forbidden command and protected-path literal scan: `0`
- Product process count: `0`
- source/runtime/evidence file count: `0/0/0`
- child-only runtime-root override configured

The Product executable was started exactly once. The process remained alive and reported `Responding=true`, but no top-level WPF window became observable:

| Observation | Result |
|---|---|
| Product launch count | `1` |
| initial ProductShell lookup window | `15 seconds` |
| unique top-level ProductShell candidate | `0` |
| `MainWindowHandle` | `0` |
| `MainWindowTitle` | empty |
| UIA top-level child owned by Product PID | `0` |
| Product process residue | `1` |

The HOLD cleanup path performed a second bounded ProductShell lookup for normal UIA close. It also timed out after 15 seconds. A final `MainWindowHandle` check remained zero, so no close action was issued. Forced termination was not performed.

This observation does not establish a Product source defect. It establishes that the required visible ProductShell and normal UIA close boundary were unavailable in this single authorized runtime attempt.

## 5. Scenario Results

| Scenario | Result | Reason |
|---|---|---|
| R01 | NOT_EXECUTED | ProductShell top-level window unavailable |
| R02 | NOT_EXECUTED | stopped before R01 |
| P02 | NOT_REEXECUTED | native picker was never opened |
| R03 | NOT_EXECUTED | stopped before R01 |
| R04 | NOT_EXECUTED | stopped before R01 |
| R05 | NOT_EXECUTED | stopped before R01 |
| R06 | NOT_EXECUTED | stopped before R01 |
| R07 | NOT_EXECUTED | registration Button was never queried or invoked |
| R08 | NOT_EXECUTED | stopped before R01 |
| R09 | HOLD | no top-level UIA Window or main-window handle existed for normal close |

No automatic Product rerun was performed.

## 6. Screenshot and Evidence 06

- Required screenshots: `10`
- Created screenshots: `0`
- Screenshot result: `0/10`
- Required Evidence 06 entries: `14`
- Created Evidence 06 entries: `0`
- Evidence 06 ZIP: `NOT_CREATED`

No prior screenshot, state, UIA element, action object, or runtime data was reused.

## 7. Runtime and Persistence

Five approved synthetic source files were created before the Product launch. No picker or registration workflow accessed them.

Observed isolated runtime state after the HOLD:

| Runtime item | Count |
|---|---:|
| metadata files in `runtime\data\local` | 0 |
| managed payload files | 0 |
| staging files | 0 |
| successful registrations | 0 |

Therefore, the required final persistence state `1/2/2/1/1/2/0` was not attempted and cannot be claimed.

## 8. TEMP Cleanup

Exact safe cleanup completed:

- this run's `source`: removed
- this run's `harness`: removed
- this run's `preflight`: removed

Preserved:

- `evidence`: preserved, file count `0`
- `logs`: preserved, file count `3`
- `transport`: preserved, file count `0`
- `runtime`: preserved, file count `0`

The isolated `runtime` directory was not removed because the Product process remained alive. Removing a runtime root still owned by a live process would not be a valid cleanup action.

## 9. Protected Boundary Audit

- production runtime root access/deletion: `0/0`
- `data/claimdoc` access: `0`
- recursive project-root scan: `0`
- broad extension scan: `0`
- build/test: `0/0`
- source/test/XAML/resource/project modification by this batch: `0/0/0/0/0`
- App launch count: `1`
- native file picker launch count: `0`
- registration workflow execution count: `0`
- stage/commit/push: `0/0/0`
- reset/checkout/clean/stash: `0/0/0/0`
- forced termination: `0`

## 10. Repository State

Before creating this document:

- tracked/staged/untracked: `27/0/15`
- status entries: `42`
- existing exact 42-path set: unchanged
- source/test/XAML/resource/project edits from this batch: `0`
- `git diff --check`: PASS

This document is the only repository file created by this batch.

Expected final state:

- tracked modified: `27`
- staged: `0`
- untracked: `16`
- status entries: `43`
- existing exact 42-path set: unchanged
- new repository path: this `docs/427` only

## 11. Findings

| Severity | Count | Finding |
|---|---:|---|
| Blocking | 1 | Single authorized Product process exposed no top-level window and remained alive, so R01-R09 and normal UIA close could not complete |
| Major | 0 | none established |
| Minor | 0 | none |
| TEMP preflight harness | 0 | DispatcherTimer lifetime repair passed |

## 12. Final Judgment

PASS conditions were not met:

- corrected preflight: PASS
- Product launch: `1`
- ProductShell top-level window: unavailable
- P02: NOT_REEXECUTED
- R01-R09: NOT_COMPLETED
- R07 disabled transition: NOT_EXECUTED
- screenshots: `0/10`
- Evidence 06: `0/14`
- process residue: `1`
- protected-path-safe audit: PASS

Final marker:

`POLICY_CLAIM_PRODUCT_UI_SHELL_GATE8_R07_PREFLIGHT_LIFETIME_REPAIR_PASS_PRODUCT_RUNTIME_RECHECK_HOLD`

State after this batch:

- Corrected preflight: `PASS`
- Guarded runtime functional review: `HOLD_PRODUCT_TOP_LEVEL_WINDOW_UNAVAILABLE_PROCESS_RESIDUE`
- Transient busy objective evidence: `NOT_EXECUTED`
- Objective visual evidence: `NOT_CREATED`
- User visual acceptance: `NOT_AVAILABLE_AS_NEXT_GATE`
- Final Gate 8 implementation: `HOLD_RUNTIME_REVIEW_REQUIRED`
- Deployment/production readiness: `NOT_AUTHORIZED`
- Stage/commit: `NOT_AUTHORIZED`

Any retry or process-residue handling requires a new explicit user decision. This batch does not authorize a Product rerun, force termination, source repair, stage, or commit.
