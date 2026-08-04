# Gate 8 Transient File Picker UIA Semantic Harness Repair and Full Runtime Recheck

## 1. Marker and Judgment

`POLICY_CLAIM_PRODUCT_UI_SHELL_GATE8_TRANSIENT_FILE_PICKER_UIA_SEMANTIC_RECHECK_HOLD`

- Judgment: `HOLD`
- Primary reason: `HOLD_UIA_SEMANTIC_INTERACTION_FAILURE`
- Secondary reason: `HOLD_VISUAL_EVIDENCE_INCOMPLETE`
- User visual acceptance: `REQUIRED_NOT_YET_EXECUTED`
- Final Gate 8 implementation: `HOLD_RUNTIME_REVIEW_REQUIRED`
- Deployment/production readiness: `NOT_AUTHORIZED`
- Stage/commit: `NOT_AUTHORIZED`

## 2. Baseline

- Branch: `main`
- HEAD: `aecf7edfd43b4124ec5ff17d35687020cf4c0d90`
- Subject: `docs(familyclaimref): record gate8 registration persistence decision package`
- Parent: `79d8f1d5b76b22b0ef1a65fbf781c833bbcd7fff`
- Start tracked/staged/untracked: `27/0/10`
- Start status entries: `37`
- Existing exact 37-path set equality: `37/37`
- Missing/extra: `0/0`
- `docs/422` preexistence: `0`

## 3. Protected Hashes

| File | SHA-256 | Result |
|---|---|---|
| `tests/FamilyClaimRef.App.Tests/DocumentFileValidationServiceTests.cs` | `ec55a7e3d1ebc9e8f5625ed628ea90914057d3fe8bab08a2772047ac8ff37431` | PASS |
| `docs/419_POLICY_CLAIM_PRODUCT_UI_SHELL_GATE8_REAL_DOCUMENT_REGISTRATION_IMPLEMENTATION_RESULT_REVIEW.md` | `b81b76fe43bef81142db1beb30c930b939773993a26a3a83d24a500d97a73506` | PASS |
| `docs/420_POLICY_CLAIM_PRODUCT_UI_SHELL_GATE8_U16_ACTUAL_REPARSE_POINT_TEST_REPAIR_INDEPENDENT_RECHECK.md` | `fc7101ce4347d19178edbfbb0920e42eedfdbb7ba30848516a0b688ff8d24001` | PASS |
| `docs/421_POLICY_CLAIM_PRODUCT_UI_SHELL_GATE8_GUARDED_RUNTIME_UIA_MANUAL_VISUAL_REVIEW_RESULT_REVIEW.md` | `1056ad68f56cd1e89b618a9ba3207f36181eb781e81db79cff1ad306346533db` | PASS |
| `docs/413` | `8f8a5717085ea3f08745e3ae16b8226897af0b127bfff55fbba6fc595650dabd` | PASS |
| `docs/414` | `522d1e9518cf2d4314f9cf3214d57d22be06c4f3b8b0f77fddf1cd4044c0141f` | PASS |
| `docs/415` | `04db1ba9dbb606a8ed2c429c447834294f2a407ee0d2714bb8369d0274e7727a` | PASS |
| `docs/416` | `e62e2cc9cb49b8fe090db49f608ef0c3ed76014bc336ea986a1a321b58b58b28` | PASS |
| `docs/417` | `2b6ff910b6699f8fcdb38344494472f34ed9c942a9916b8f0972a3dcbf6488c1` | PASS |
| `docs/418` | `e458f808079d07f8418072f31304ba10b74d28b84dcd4d30a4ffc326783c6363` | PASS |

## 4. Binary Identity

| Artifact | Relative path | Bytes | SHA-256 | Result |
|---|---|---:|---|---|
| EXE | `app/FamilyClaimRef.App/bin/Debug/net10.0-windows/FamilyClaimRef.App.exe` | 162816 | `bb3de775939bbeb06aa9abe42e9e93cee51881084b3e6f20e7293a2d23300c39` | PASS |
| DLL | `app/FamilyClaimRef.App/bin/Debug/net10.0-windows/FamilyClaimRef.App.dll` | 294912 | `4534420c13f4f01b80263a73d3a3c71bbe3ce1c0c01836e3b069a6ce218e1f3f` | PASS |

- Launch: verified EXE direct, no arguments
- Build/test: not run, prohibited by this runtime recheck batch

## 5. Isolated Runtime and Harness Boundary

- Final normalized run identity: `gate8-uia-live-20260727-141821-808c0789`
- Logical run root: `%TEMP%\FamilyClaimRef\Gate8RuntimeReview\<run-id>`
- Child environment:
  - `FAMILYCLAIMREF_ENABLE_DEV_RUNTIME_ROOT_OVERRIDE=1`
  - `FAMILYCLAIMREF_RUNTIME_ROOT=<run-root>\runtime`
- Parent Process/User/Machine persistent environment mutation: `0/0/0`
- TEMP harness location: `<run-root>\harness`
- Repository harness copy: `0`
- Product assembly harness call: `0`
- Production runtime root access/delete: `0/0`
- `data/claimdoc` access: `0`

Harness repair observations:

1. optional UIA `Name` and `AutomationId` filters were corrected to distinguish omitted values from empty-string values;
2. the native `#32770` picker was found as a live `Window` descendant of ProductShell with the same product PID and a distinct native handle;
3. file-name resolution was narrowed to `AutomationId=1148`, `ClassName=Edit`, and `ValuePattern`;
4. shell list items sharing `AutomationId=1/2` were separated from the actual Open/Cancel button HWNDs;
5. no stale Window2 element index or cached action element was used.

## 6. Synthetic Inputs

| File | Bytes | SHA-256 |
|---|---:|---|
| `gate8-runtime-a.pdf` | 69 | `cfa3181c1ee36e8bce5e39f84959f4558ea7ba32c0e4539a8ab3c8ce8c716ec6` |
| `gate8-runtime-a-copy.pdf` | 69 | `cfa3181c1ee36e8bce5e39f84959f4558ea7ba32c0e4539a8ab3c8ce8c716ec6` |
| `gate8-runtime-b.pdf` | 89 | `05282e8c3f2187f651713dd4003bd7825b4e991fcb20d3e01d24d7304c817b65` |
| `gate8-runtime-invalid.pdf` | 39 | `8fa3d1dcd0db6a67213df95d58af0696ae3c00d18fe0cba1d05bd8e8c82f581e` |
| `gate8-runtime-large.pdf` | 26214400 | `89d0f4f15ff8df658f606c525a224e91adabb4aa1e8d84c371811988c5fcb09d` |

- Actual user document or personal/insurance/medical/claim sample use: `0`

## 7. Live Dialog Selector

Actual dialog root:

| Property | Value |
|---|---|
| Candidate count | `1` |
| Name | `열기` |
| ControlType | `Window` |
| ClassName | `#32770` |
| ProcessId | product PID, captured but not persisted |
| Native handle | distinct from ProductShell, captured but not persisted |
| Lookup | live query before each action |

The picker is a native top-level transient window even though the active UIA provider exposes it beneath the ProductShell fragment. The resolver therefore uses product PID, `ControlType=Window`, `ClassName=#32770`, and a native handle distinct from the main window.

## 8. File-name, Open, and Cancel Semantic Targets

| Target | AutomationId | ControlType | ClassName | Required pattern | Actual result |
|---|---|---|---|---|---|
| file-name | `1148` | `Edit` | `Edit` | `ValuePattern` | PASS |
| Open actual button | `1` | `Pane` | `Button` | `InvokePattern` | FAIL, pattern unavailable |
| Cancel actual button | `2` | `Button` | `Button` | `InvokePattern` | PASS |

The actual Open HWND was independently rechecked through:

- Raw view;
- Control view;
- Content view;
- `AutomationElement.FromHandle`;
- provider-side `AutomationId=1 AND ClassName=Button` query.

All five surfaces returned `InvokePattern` unavailable for the actual Open button.

An unrelated shell list item also exposed `AutomationId=1` and `InvokePattern`. It was rejected because its `ControlType=ListItem` and `ClassName=UIItem` do not identify the Open button. Invoking that list item during harness diagnosis did not close the picker and is not counted as an Open semantic action.

## 9. P00 Semantic Capability Result

| Check | Result |
|---|---|
| Dialog root unique resolution | PASS |
| file-name target unique resolution | PASS |
| file-name `ValuePattern.SetValue` | PASS |
| Cancel target unique resolution | PASS |
| Cancel `InvokePattern.Invoke` | PASS |
| Open actual button unique resolution | PASS |
| Open actual button `InvokePattern` availability | FAIL |
| Open actual button semantic invocation | NOT_EXECUTABLE |
| Picker bypass | `0` |
| Coordinate/blind/index actions | `0/0/0` |
| Storage side effects | `0` |

The initial harness P00 record that treated the shell list item as the Open target is invalidated by the independent class/control-type recheck. Final P00 status is `FAILED`.

## 10. Runtime Scenario Results

| Scenario | Result | Evidence |
|---|---|---|
| R01 | PASS | ProductShell 1, navigation 5, initial `홈`, unexpected dialog 0 |
| R02 | PASS | Policy A and Claim A created through actual UI; Claim A linked to Policy A |
| P00 | FAIL | actual Open button has no `InvokePattern`; storage side effect 0 |
| R03 | INVALID_INCOMPLETE | diagnostic attempt invoked a shell list item, picker remained open, registration side effect 0 |
| R04 | NOT_EXECUTED | stopped by P00 semantic gate |
| R05 | NOT_EXECUTED | stopped by P00 semantic gate |
| R06 | NOT_EXECUTED | stopped by P00 semantic gate |
| R07 | NOT_EXECUTED | stopped by P00 semantic gate |
| R08 | NOT_EXECUTED | stopped by P00 semantic gate |
| R09 | NOT_VALIDATED | normal-close outcome was not established; final process residue 0 |

No R03 result is promoted to a product implementation finding.

## 11. Picker and Interaction Counts

- Picker dialog open: `2`
- file-name `ValuePattern.SetValue`: `2`
- valid Cancel `InvokePattern.Invoke`: `1`
- valid Open-button `InvokePattern.Invoke`: `0`
- rejected shell-list-item invocation: `1`
- UIA audit records: `17`
- Blind click: `0`
- Coordinate click: `0`
- Hardcoded element-index action: `0`
- Direct ViewModel/storage mutation: `0/0`
- Picker bypass: `0`

## 12. Persistence Evidence

State immediately before approved exact cleanup:

| Item | Count |
|---|---:|
| Active Policy | 1 |
| Active Claim | 1 |
| Document | 0 |
| Policy link | 0 |
| Claim link | 0 |
| Managed payload | 0 |
| Staging file | 0 |

- `policies.json` SHA-256: `eca7b0c046f07366fff19d3c3da234bc7c0ca3312ce989b034339e14773047a5`
- `claims.json` SHA-256: `a36b96d3dcee515f46ee93f917b40783698e46cd2baa6013dfbcea96a6a2b737`
- Absolute source/staging/managed path in durable JSON: `0/0/0`
- Successful document registration: `0`
- Selection/staged/final SHA equality: NOT_EXECUTED
- Duplicate, invalid, busy, and stale-target persistence checks: NOT_EXECUTED

## 13. Screenshot Manifest

Expected screenshots: `10`

Actual screenshots: `2`

| File | Dimensions | Bytes | SHA-256 | Result |
|---|---:|---:|---|---|
| `00_default_product_shell_home.png` | 820x520 | 15053 | `0811697cc6aff8e35ed6fb68a2111dfba3f6cc5fa9121008094e6fbb19e306c0` | PASS |
| `01_registration_initial.png` | 820x520 | 24003 | `9d821ce3f3c8c9263aaa14e562b1098527e3a00eb2016f5a1210d7df07734c11` | PASS |

Missing visual evidence: `8`

- `02_valid_file_selected_draft.png`
- `03_success_reset_target_retained.png`
- `04_picker_cancel_draft_retained.png`
- `05_invalid_replacement_safe_rejection.png`
- `06_duplicate_rejected_inputs_retained.png`
- `07_busy_navigation_return.png`
- `08_busy_registration_completed.png`
- `09_stale_target_cleared_draft_retained.png`

## 14. Evidence 02 Transport

- Required ZIP: `POLICY_CLAIM_PRODUCT_UI_SHELL_GATE8_RUNTIME_VISUAL_EVIDENCE_02.zip`
- ZIP created: `no`
- Required entries: `14`
- Actual entries: `0`
- Reason: P00 failed before a valid Open-button `InvokePattern` action; incomplete evidence was not promoted to an Evidence 02 submission.
- Expanded TEMP path in preserved JSON: `0`
- User/machine/account identifier in preserved JSON: `0`
- Source PDF included in evidence/transport: `0`

Preserved final-run evidence:

- screenshots: `2`
- JSON evidence files: `5`
- log JSON files: `1`
- transport files: `0`

## 15. Process, Dialog, Cleanup, and Residue

- Final-run FamilyClaimRef process residue: `0`
- Unexpected external dialog: `0`
- Forced termination during failed final-run cleanup: `1`
- Crash/hang promoted as product finding: `0/0`
- Final-run isolated source/runtime/harness files after cleanup: `0/0/0`
- Final-run isolated staging/final payload residue after cleanup: `0/0`
- Final-run evidence/logs/transport files: `7/1/0`
- Diagnostic sibling roots: preserved as required; not used as final Evidence 02
- Project-root attachments files: `0`
- Project-root data/local files: `0`
- Project-root `runtime_test_document.*`: `0`
- Production runtime access/delete: `0/0`
- `data/claimdoc` access: `0`

## 16. Existing 37-Path Content Delta

- Existing exact 37-path content delta caused by this batch: `0`
- Production source delta caused by this batch: `0`
- Test delta caused by this batch: `0`
- XAML/resource/project delta caused by this batch: `0`
- Protected docs `413~421` delta: `0`
- Repository file created by this batch:
  - `docs/422_POLICY_CLAIM_PRODUCT_UI_SHELL_GATE8_TRANSIENT_FILE_PICKER_UIA_SEMANTIC_HARNESS_REPAIR_AND_FULL_RUNTIME_RECHECK.md`

## 17. Findings

Product implementation findings:

- Blocking: `0`
- Major: `0`
- Minor: `0`

Review infrastructure findings:

- Blocker: `1`
  - the actual native Open button does not expose `InvokePattern` through the available Windows UIA provider surfaces;
- Visual evidence missing: `8`;
- Invalid target classification repaired: `1`.

The UIA provider limitation is not promoted to a product defect.

## 18. Final State

- Independent source/test recheck: `PASS` carry-forward
- U16 actual reparse-point validation: `PASS` carry-forward
- P00 semantic capability: `HOLD`
- Guarded runtime functional/UIA review: `HOLD`
- Objective visual evidence: `HOLD`
- User visual acceptance: `REQUIRED_NOT_YET_EXECUTED`
- Final Gate 8 implementation: `HOLD_RUNTIME_REVIEW_REQUIRED`
- Startup crash recovery: `DEFERRED_NOT_IMPLEMENTED`
- Cross-process uniqueness: `DEFERRED_NOT_IMPLEMENTED`
- Deployment/production readiness: `NOT_AUTHORIZED`
- Stage/commit: `NOT_AUTHORIZED`

## 19. Next Recommendation

Actual native Open button에 `InvokePattern`을 제공하는 UIA adapter 또는 검증 환경을 준비한 뒤 P00부터 새 격리 run으로 재실행한다.
