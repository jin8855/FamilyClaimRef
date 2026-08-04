# Gate 8 Dialog-root MSAA Default Action and Full Runtime Recheck

## 1. Marker and Judgment

`POLICY_CLAIM_PRODUCT_UI_SHELL_GATE8_DIALOG_ROOT_DEFAULT_ACTION_RECHECK_HOLD`

- Judgment: `HOLD`
- Primary reason: `HOLD_VISUAL_EVIDENCE_INCOMPLETE`
- P02 dialog-root default-action gate: `PASS`
- Product implementation finding: `0`
- Review infrastructure finding: `1`
- User visual acceptance: `NOT_EXECUTED`
- Final Gate 8 implementation: `HOLD_RUNTIME_REVIEW_REQUIRED`
- Deployment/production readiness: `NOT_AUTHORIZED`
- Stage/commit: `NOT_AUTHORIZED`

## 2. Baseline

- Branch: `main`
- HEAD: `aecf7edfd43b4124ec5ff17d35687020cf4c0d90`
- Subject: `docs(familyclaimref): record gate8 registration persistence decision package`
- Start tracked/staged/untracked: `27/0/12`
- Start status entries: `39`
- Existing exact 39-path set: unchanged
- `docs/424` preexistence: `0`
- `docs/423` SHA-256: `f14220edc2a23742055ef5fa00fe560a1278f87f1d5fe6a36298bc14f6c3a51c`

## 3. Binary Identity

| Artifact | Relative path | Bytes | SHA-256 | Result |
|---|---|---:|---|---|
| EXE | `app/FamilyClaimRef.App/bin/Debug/net10.0-windows/FamilyClaimRef.App.exe` | 162816 | `bb3de775939bbeb06aa9abe42e9e93cee51881084b3e6f20e7293a2d23300c39` | PASS |
| DLL | `app/FamilyClaimRef.App/bin/Debug/net10.0-windows/FamilyClaimRef.App.dll` | 294912 | `4534420c13f4f01b80263a73d3a3c71bbe3ce1c0c01836e3b069a6ce218e1f3f` | PASS |

- Launch: verified EXE direct, no arguments
- Build/test: not run, prohibited by this runtime batch

## 4. Isolated Run History

Three isolated TEMP runs were used. Product source, tests, XAML, resources, and project files were not changed.

| Run | Classification | Result |
|---|---|---|
| `gate8-dialog-root-20260727-152543-3644f975` | harness diagnostic | stopped before R01 because the new read-only occlusion probe constructed its point array incorrectly |
| `gate8-dialog-root-20260727-153401-386d10c4` | harness diagnostic | P02 and R03 Product persistence succeeded; a stale harness success-message assertion caused the stop |
| `gate8-dialog-root-20260727-153737-a468bc80` | authoritative recheck | P02 and R01-R06 PASS; R07 busy-disabled visual state not observed |

The first diagnostic run opened no picker and created no runtime data. The second run proved that R03 completed despite the stale assertion:

- Document: `1`
- Claim link: `1`
- Payload: `1`
- Staging residue: `0`
- Forced termination/process residue: `0/0`

Both harness-only defects were corrected only in TEMP. Each superseded run had its source/runtime/harness removed while its evidence and logs were preserved.

## 5. Isolated Runtime Boundary

- Authoritative run identity: `gate8-dialog-root-20260727-153737-a468bc80`
- Logical run root: `%TEMP%\FamilyClaimRef\Gate8RuntimeReview\<run-id>`
- Child-only runtime root override: PASS
- Parent Process/User/Machine environment mutation: `0/0/0`
- Repository harness or runtime artifact creation: `0`
- Production runtime root access/delete: `0/0`
- `data/claimdoc` access: `0`

## 6. P02 Identity and Default-target Proof

| Check | Result |
|---|---|
| Product child dialog, `ClassName=#32770`, unique HWND | PASS |
| File-name `AutomationId=1148`, `ClassName=Edit`, `ValuePattern` | PASS |
| Open `AutomationId=1`, `ClassName=Button`, enabled, unique dialog descendant | PASS |
| Unrelated shell `ListItem` exclusion | PASS |
| Exact source identity entered and read back | PASS |
| `DM_GETDEFID` high word | `0x534B`, `DC_HASDEFID` |
| `DM_GETDEFID` low word | `1` |
| Default ID and UIA Open candidate equality | PASS |

The final run opened six picker dialogs. Every dialog/default ID was resolved live:

- Picker dialog count: `6`
- Normalized dialog identities: `6`
- Read-only `DM_GETDEFID` query count: `11`
- Default ID mismatch: `0`
- Raw dialog HWND persisted in evidence: `0`

## 7. Dialog-root MSAA Action

P02 dialog-root accessible object:

| Property | Value | Result |
|---|---|---|
| Role | `18`, `ROLE_SYSTEM_DIALOG` | PASS |
| Name | `열기` | PASS |
| DefaultAction | `열기(O)` | PASS, localized Open action |
| Child identity | `CHILDID_SELF` | PASS |
| State | enabled/available | PASS |

Selected P02 route:

`DialogRoot_MSAA_accDoDefaultAction`

| Interaction | Count |
|---|---:|
| Dialog-root `accDoDefaultAction(CHILDID_SELF)` | 5 |
| Validated default-keyboard action | 0 |
| UIA Open `InvokePattern` | 0 |
| `LegacyIAccessiblePattern` Open | 0 |
| Native Open-button MSAA action | 0 |
| Blind keyboard | 0 |
| Blind/coordinate/hardcoded-index click | `0/0/0` |
| Direct action message | 0 |
| Read-only `DM_GETDEFID` message | 11 |
| Picker bypass | 0 |
| Direct ViewModel/service/storage mutation | 0 |

No `BM_CLICK`, `WM_COMMAND`, action-purpose `SendMessage`, `PostMessage`, or `SetWindowText` call was used.

## 8. P02 Result

| Check | Result |
|---|---|
| Actual dialog/Open/file-name identity | PASS |
| Default target ID | `1`, PASS |
| Dialog-root MSAA action | PASS |
| Picker closed | PASS |
| Product leaf filename only | PASS |
| Selected A source snapshot | PASS |
| Storage side effect before registration | `0` |
| Absolute path exposed in Product UI | `0` |
| Forbidden action | `0` |

P02 resolved the previous native-picker action blocker.

## 9. Runtime Scenario Results

| Scenario | Result | Evidence |
|---|---|---|
| R01 | PASS | ProductShell 1, navigation 5, selected Home 1, unexpected dialog 0 |
| R02 | PASS | Policy A and Claim A created through Product UI and linked |
| P02 | PASS | Default ID 1 and dialog-root MSAA Open action |
| R03 | PASS | A registered once, inputs reset, Claim A target retained |
| R04 | PASS | B draft survived a second picker Cancel; durable state unchanged |
| R05 | PASS | Invalid replacement rejected; B and draft retained |
| R06 | PASS | A-copy duplicate rejected; durable counts unchanged; staging residue 0 |
| R07 | PARTIAL_HOLD | Large registration persisted once, but busy-disabled state was not observed |
| R08 | NOT_EXECUTED | stopped by R07 visual gate |
| R09 | PASS_FOR_HOLD_CLEANUP | top-level UIA close completed; process residue 0 |

No delay was inserted after the R07 busy state was missed. This follows the required stop rule and establishes:

`HOLD_VISUAL_EVIDENCE_INCOMPLETE`

## 10. Persistence Evidence

State immediately before approved exact cleanup:

| Item | Count |
|---|---:|
| Policy | 1 |
| Claim | 1 |
| Document | 2 |
| Policy-document link | 1 |
| Claim-document link | 1 |
| Managed payload | 2 |
| Staging file | 0 |

| Registration | Metadata SHA | Managed payload SHA | Result |
|---|---|---|---|
| R03 Claim A document | `cfa3181c1ee36e8bce5e39f84959f4558ea7ba32c0e4539a8ab3c8ce8c716ec6` | same | PASS |
| R07 Policy A large document | `89d0f4f15ff8df658f606c525a224e91adabb4aa1e8d84c371811988c5fcb09d` | same | PASS |

- Document/payload hash match: `2/2`
- Hash mismatch: `0`
- R03 registration count: `1`
- R07 registration count: `1`
- Expanded run-root path in durable JSON: `0`
- User-profile path in durable JSON: `0`
- Absolute synthetic source path in durable JSON: `0`

R07 functional persistence is observed, but the scenario is not promoted to PASS because its mandatory busy visual evidence is absent.

## 11. Screenshot Evidence

Expected screenshots: `10`

Actual and visually inspected screenshots: `7`

All seven available captures passed ProductShell foreground, five-point occlusion, expected-view identity, clipping, overlap, and forbidden-exposure inspection.

| File | Dimensions | Bytes | SHA-256 |
|---|---:|---:|---|
| `00_default_product_shell_home.png` | 820x520 | 13261 | `daca13808092423a9cab07b1821ceb70edc35ccc8648663de0e4efeb0940ceb1` |
| `01_registration_initial.png` | 820x520 | 22364 | `04caef3cbe08ddaba91c6cd9a6c8951b51b26b97b1a2629458e21b128ca22aa5` |
| `02_valid_file_selected_draft.png` | 820x520 | 25028 | `42d89cf4d70254736d0a909f8107b319e87a340057662a3f58d04ad81d519f24` |
| `03_success_reset_target_retained.png` | 820x520 | 23340 | `c2d8172bdd064c91f916a9a227b2856d32fbf502e34bf5aaa30ca025bd49332c` |
| `04_picker_cancel_draft_retained.png` | 820x520 | 23827 | `d57b5030791880da06068e0302f83b268644087d034a0921241503f5231e5445` |
| `05_invalid_replacement_safe_rejection.png` | 820x520 | 25334 | `e20fd5b6230b60b5e5a7b77aae67ab277eab7bfece91dd1902c7656ae0edb9ae` |
| `06_duplicate_rejected_inputs_retained.png` | 820x520 | 26279 | `baaa03f73ff3337e6054567be5fa949bb1dff4749acb9fb623916dff2f170b93` |

Missing:

- `07_busy_navigation_return.png`
- `08_busy_registration_completed.png`
- `09_stale_target_cleared_draft_retained.png`

Screenshot completeness: `7/10`

## 12. Evidence 04

- Required ZIP: `POLICY_CLAIM_PRODUCT_UI_SHELL_GATE8_RUNTIME_VISUAL_EVIDENCE_04.zip`
- Required entries: `14`
- ZIP created: `no`
- Actual transport entries: `0`
- Reason: R07 busy visual evidence and R08/R09 screenshots are incomplete.

Preserved authoritative-run evidence:

- PNG: `7`
- JSON: `4`
  - `RUNTIME_EVIDENCE_MANIFEST.json`
  - `UIA_CLICK_AUDIT.json`
  - `PERSISTENCE_EVIDENCE.json`
  - `PROCESS_AND_DIALOG_AUDIT.json`
- Logs: `1`
- Transport files: `0`

Incomplete evidence was not promoted to Evidence 04.

## 13. Process, Cleanup, and Residue

- Authoritative Product process launch: `1`
- Forced termination: `0`
- Crash/hang: `0/0`
- Final process residue: `0`
- Authoritative source/runtime/harness after exact cleanup: `0/0/0`
- Authoritative evidence/logs/transport files: `11/1/0`
- Isolated staging/final payload residue after cleanup: `0/0`
- Project-root attachments files: `0`
- Project-root `data/local` files: `0`
- Project-root `runtime_test_document.*` files: `0`
- Production runtime access/delete: `0/0`
- Persistent environment mutation: `0`
- `data/claimdoc` access: `0`

## 14. Repository Scope

- Existing exact 39-path content delta caused by this batch: `0`
- Production source delta caused by this batch: `0`
- Test delta caused by this batch: `0`
- XAML/resource/project delta caused by this batch: `0`
- Existing docs `413~423` delta caused by this batch: `0`
- Repository file created by this batch:
  - `docs/424_POLICY_CLAIM_PRODUCT_UI_SHELL_GATE8_DIALOG_ROOT_MSAA_DEFAULT_ACTION_AND_FULL_RUNTIME_RECHECK.md`
- `git diff --check`: PASS
- Build/test: not run
- Stage/commit/push/tag/rebase/amend/reset/checkout/clean: `0/0/0/0/0/0/0/0/0`

## 15. Findings

| Severity | Count | Finding |
|---|---:|---|
| Blocking product finding | 0 | none |
| Major product finding | 0 | none |
| Review infrastructure finding | 1 | R07 busy-disabled visual state completed too quickly to be observed |
| Minor product finding | 0 | none |

P02, R03, R04, R05, and R06 produced no Product defect finding. R07 persistence also completed consistently, but the required visual state remains unproven.

## 16. Final Gate

PASS conditions not met:

- P02: PASS
- R01-R06: PASS
- R07: PARTIAL_HOLD
- R08: NOT_EXECUTED
- screenshot: `7/10`
- Evidence 04: `0/14`
- forced termination/process residue: `0/0`

Final state:

- Marker: `POLICY_CLAIM_PRODUCT_UI_SHELL_GATE8_DIALOG_ROOT_DEFAULT_ACTION_RECHECK_HOLD`
- Guarded runtime functional review: `PARTIAL_R01_TO_R06_PASS`
- Objective visual evidence: `HOLD_VISUAL_EVIDENCE_INCOMPLETE`
- User visual acceptance: `NOT_EXECUTED`
- Final Gate 8 implementation: `HOLD_RUNTIME_REVIEW_REQUIRED`
- Deployment/production readiness: `NOT_AUTHORIZED`
- Stage/commit: `NOT_AUTHORIZED`
- Gate 8 source implementation automatic continuation: `NOT_AUTHORIZED`

## 17. Next Recommendation

Require an explicit user decision on whether R07 must remain visually observable or whether a different read-only busy-state evidence mechanism may be used. Do not add Product delay solely to make the transient busy state visible.
