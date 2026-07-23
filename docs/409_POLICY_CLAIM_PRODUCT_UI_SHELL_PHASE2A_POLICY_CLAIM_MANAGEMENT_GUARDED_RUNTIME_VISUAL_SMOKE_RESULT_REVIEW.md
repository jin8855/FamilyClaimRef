# Product UI Shell Phase 2A Policy Claim Management Guarded Runtime Visual Smoke Result Review

## A. Status Marker

`POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE2A_POLICY_CLAIM_MANAGEMENT_GUARDED_RUNTIME_VISUAL_SMOKE_HOLD`

판정: **HOLD**

사용자 최종 visual acceptance: **pending**

이 문서는 격리 runtime root에서 수행한 guarded runtime/UI Automation/visual smoke 결과만 기록한다. 소스, 테스트, XAML, 리소스, startup 및 기존 문서는 수정하지 않았다.

## B. Baseline And Initial State

| Item | Result |
|---|---|
| Repository | `C:\EtcProject\FamilyClaimRef` |
| HEAD | `73808a52e7af7c9706d83ef3c905dd81fb3bf4c2` |
| Subject | `feat(familyclaimref): add product policy claim management` |
| Parent | `9706eccd39248d66bf2d40a8dd20a5bd1ff2207f` |
| Documentation parent | `eda6fdba20fb713035b51b7a960442f119192a1b` |
| Initial tracked status | clean |
| Initial staged files | 0 |
| Initial untracked files | 0 |
| Initial FamilyClaimRef process count | 0 |
| Existing `docs/409` collision | none |

Commit reconciliation:

- `9706eccd39248d66bf2d40a8dd20a5bd1ff2207f`: `docs/404` through `docs/408`, exactly five added files.
- `73808a52e7af7c9706d83ef3c905dd81fb3bf4c2`: exactly 15 files, CREATE 5 and MODIFY 10.
- Working file versus committed blob mismatch before execution: 0.
- Resource/constants count: 91/91.
- `Ui.Product.*` count: 35/35.
- Navigation source order: Home, PolicyContracts, ClaimCases, DocumentRegistration, DocumentList.
- Initial navigation source index: Home.
- `docs/408` marker confirmed:
  `POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE2A_POLICY_CLAIM_MANAGEMENT_EXACT_IMPLEMENTATION_GATE_OPEN`.

## C. Build And Runtime Provenance

Build command:

```text
dotnet build FamilyClaimRef.sln
```

Build result:

- PASS
- warnings: 0
- errors: 0
- elapsed: 16.61 seconds
- configuration: Debug
- target framework: `net10.0-windows`
- build evidence capture: `2026-07-23T08:44:07.9661307+00:00`

Launched output:

| Artifact | Path | SHA-256 | Last write UTC |
|---|---|---|---|
| Executable | `C:\EtcProject\FamilyClaimRef\app\FamilyClaimRef.App\bin\Debug\net10.0-windows\FamilyClaimRef.App.exe` | `E6ADBEE30194306F90E0EDD8B5FB0A11C0E388FD5F6217B9D2875D08221FAF8A` | `2026-07-23T08:43:25.6953068Z` |
| Application assembly | `C:\EtcProject\FamilyClaimRef\app\FamilyClaimRef.App\bin\Debug\net10.0-windows\FamilyClaimRef.App.dll` | `66D55F37DE413FF82BCF4FDB1007FF916D0E682330DFDB964C0B43FEB6908D77` | `2026-07-23T08:43:25.6123164Z` |

The executable above was launched directly. `dotnet run` was not used.

Automated tests were not rerun in this smoke batch. The carried-forward implementation evidence is full test PASS 417/417 and was not used as a substitute for runtime smoke evidence.

## D. Isolated Runtime Environment

Run ID:

`20260723-174407852-51817d1a`

Normalized run root:

`<TEMP>\FamilyClaimRef-Phase2A-ManagementSmoke\<run-id>\`

Child-only environment:

```text
FAMILYCLAIMREF_ENABLE_DEV_RUNTIME_ROOT_OVERRIDE=1
FAMILYCLAIMREF_RUNTIME_ROOT=<default-or-preview-isolated-root>
```

Isolation results:

- The default and preview processes used different roots under the same run ID.
- `ProcessStartInfo.Environment` was used only for each child process.
- Parent process environment mutation: 0.
- User environment mutation: 0.
- Machine environment mutation: 0.
- Both variables were absent in Process/User/Machine scope after execution.
- Production runtime root read/write access: 0.
- Real personal, insurance, hospital, diagnosis, policy-number, or claim-number data used: 0.

Normalized evidence directory:

`<TEMP>\FamilyClaimRef-Phase2A-ManagementSmoke\<run-id>\evidence`

## E. Default Startup Smoke

Launch:

- executable: the verified Debug executable from section C
- argument: none
- isolated root: `<run-id>\default`

Observed:

- visible top-level windows: 1
- title: `FamilyClaimRef`
- surface: existing MainWindow local MVP validation surface
- ProductShell navigation displayed as the default surface: no
- unexpected dialogs: 0
- file picker: 0
- document registration workflow execution: 0
- crash/hang: 0

Close:

- normal window close accepted
- process exited within the 10-second limit
- child process residue: 0
- forced termination: 0

## F. Guarded ProductShell Entry And UI Automation Inventory

Launch:

- argument: `--product-shell-preview`
- isolated root: `<run-id>\preview`

Observed:

- visible top-level windows: 1
- title: `FamilyClaimRef`
- surface: ProductShellWindow
- unexpected dialogs: 0
- production runtime root used: no

Navigation:

1. Home
2. PolicyContracts
3. ClaimCases
4. DocumentRegistration
5. DocumentList

Home content was visible initially. The selected Home row was visually distinguishable in the screenshot, but UI Automation exposed neither `SelectionItemPattern.IsSelected` nor an equivalent selected/toggled property. Per the approved acceptance contract, visible Home content alone cannot replace selected-state UIA evidence. This is blocker 1.

UI Automation identified:

- five navigation list items
- policy title edit, create action, active list, disable action, result region, and empty state
- claim policy selector, title edit, create action, active list, disable action, result region, and empty state
- registration file action, target-kind selector, target selector, document-type selector, display-title edit, date picker, register action, and status region

Missing or insufficient UIA evidence:

- navigation selected-state property
- stable semantic AutomationId/name for navigation items; item names were the ViewModel type
- safe display-only Automation Name for policy/claim rows and target options

## G. Runtime Workflow Results

| Step | Result | Evidence |
|---|---|---|
| PolicyContracts initial entry | PASS | active count 0, correct empty state, no stale message |
| Create `Smoke Policy A` | PASS | active count 1, title visible, input cleared, success message |
| Duplicate `  smoke policy a  ` | PASS | active count remained 1, duplicate rows 0, input retained, duplicate message |
| ClaimCases entry | PASS | policy selector contained one `Smoke Policy A`; prior policy message cleared |
| Create `Smoke Claim A` | PASS | active count 1, title visible, claim input cleared, success message |
| Input retention | PASS | `Unsaved Claim Draft` survived PolicyContracts and ClaimCases round trip |
| Entry message reset | PASS | prior claim result message cleared on re-entry |
| Policy input independence | PASS | duplicate policy input remained after claim operations |
| Registration target refresh | PASS | one policy option and one claim option; no duplicate target |
| Claim target selection | PASS | actual claim target selected; file picker and register action not invoked |
| Disable claim | PASS | active claim removed, success message, draft input retained |
| Disable policy | PASS | active policy removed, success message, duplicate input retained |
| Registration target removal | PASS | synthetic policy/claim absent after re-entry |
| Stale target correction | PASS | selected target cleared; empty claim-target message displayed |

Interaction counts:

- actual mouse clicks issued: 34
- UIA-element-targeted clicks: 31
- bounded coordinate clicks after unique UIA/screenshot identification: 3
- keyboard text-entry actions: 4
- blind coordinate clicks: 0
- direct ViewModel method calls: 0
- direct storage JSON edits: 0

Unexpected runtime counts:

- dialogs: 0
- OpenFileDialog/file picker opens: 0
- document registration workflow executions: 0
- crashes: 0

## H. Internal Information Exposure Finding

The visible rows showed only `Smoke Policy A` and `Smoke Claim A`. However, UI Automation `ListBoxItem.Name` exposed complete record string representations.

Observed exposure classes:

- generated policy ID
- generated claim ID
- claim-to-policy internal ID
- created/updated timestamps
- disabled timestamp field
- record type names
- document-type code/scope/sort metadata in document-type options

Raw policy/claim record exposure was observed in at least five distinct control contexts: policy list, claim policy selector, claim list, registration policy target, and registration claim target. Exact generated IDs are intentionally not copied into this document.

Visible local path, exception, stack trace, or JSON exposure: 0.

The UIA internal record exposure violates the required raw internal ID/diagnostic non-exposure condition. This is blocker 2.

## I. Screenshot Manifest

All captures are application-window-only PNG files.

| File | Pixel size | DPI/scaling | UIA window size | SHA-256 | State |
|---|---:|---|---:|---|---|
| `00_default_startup.png` | 900x760 | 95.99 DPI / 100% | 886x753 | `32F65204F3E7D72D184812BD640BAD231286660EC864385C4527607D56DE2208` | default MainWindow |
| `01_product_shell_home_initial.png` | 820x520 | 95.99 DPI / 100% | 806x513 | `41F64644BF7FC0AC42AB6E7E4C4157C1212DFB70916B0C6C99DC90E78D5AFA69` | ProductShell Home |
| `02_policy_empty.png` | 820x520 | 95.99 DPI / 100% | 806x513 | `C8D1138201E1D1A2CC2565DC21E9E7C38FB791AC7B641FBD46D7AF6FE27D8A0C` | policy empty state |
| `03_policy_created.png` | 820x520 | 95.99 DPI / 100% | 806x513 | `FC7DFF77A6F8C36EC82A87D2DED66B595AB06091DD285A94BFDAE963628315FE` | policy created |
| `04_policy_duplicate_rejected.png` | 820x520 | 95.99 DPI / 100% | 806x513 | `EDCBE64D33BA0980A8D07000EF208E9E256662AA71750804E0939193114F7C6A` | duplicate rejected |
| `05_claim_created.png` | 820x520 | 95.99 DPI / 100% | 806x513 | `C1EBDCD9E103A9B2C9D944F82E3FB0892D413A4809CD9485647AA08200D11BC3` | claim created |
| `06_claim_input_retained.png` | 820x520 | 95.99 DPI / 100% | 806x513 | `EE4BC4FE0EEB6AB79E8999A3F8994A98AB71CA3E78F46D18E8FCF7C1272816EE` | draft retained |
| `07_registration_targets_present.png` | 820x520 | 95.99 DPI / 100% | 806x513 | `4FC4D31787A4B20A670854A7FF7BD5F1E108F1E229D5B4D9ED587C033B4101A8` | claim target selected |
| `08_management_targets_disabled.png` | 820x520 | 95.99 DPI / 100% | 806x513 | `ABD5F15E00B07C6333DDCB8FCC1DC73E30B8CE19EFE7921B397D05B3C1EBFC60` | management targets disabled |
| `09_registration_targets_removed.png` | 820x520 | 95.99 DPI / 100% | 806x513 | `D921ED9ADC2B0E6E216B56F49AF9DF96B6BD9FE42AE8A6A1371EB66C716E5A9C` | disabled targets removed |

Screenshot count: 10/10.

## J. Objective Visual Review

PASS observations:

- five navigation rows are separate and do not overlap
- navigation order is correct
- management titles, creation regions, lists, and result regions are structurally separated
- labels, inputs, and buttons are aligned
- button and list-row text is not clipped
- empty and populated states are distinguishable
- Korean text is readable and not corrupted
- visible screen contains no raw ID, path, exception, stack trace, JSON, or diagnostic type text
- enabled and disabled action states are visually distinguishable
- focus indication did not cover essential copy

Objective blockers:

1. Initial Home selected state is not exposed by UI Automation.
2. UI Automation names expose raw PolicyRecord/ClaimRecord identifiers and timestamps.
3. At the default ProductShell window size, populated policy/claim screens produce a vertical scrollbar and the result message is partially below the viewport in screenshots `03` through `06`. The success/duplicate message is not fully readable without scrolling. This is an objective message-clipping/accessibility blocker.

User-review-only observations:

- density, spacing preference, typography weight, and navigation emphasis remain subject to direct user visual review
- no final aesthetic acceptance is claimed

## K. Normal Close And Cleanup

Preview close:

- normal close accepted
- process exited within the 10-second limit
- child process residue: 0
- unexpected dialogs: 0
- forced termination: 0

Cleanup was performed only after process exit.

Exact isolated-root results:

- `<run-id>\default`: already absent
- `<run-id>\preview`: removed
- default root after cleanup: absent
- preview root after cleanup: absent
- evidence/logs retained outside the repository
- production root access: 0
- repository runtime artifacts created: 0

## L. Repository And Static Verification

Before creating this result document:

- `git diff --check`: PASS
- production source changes: 0
- test changes: 0
- XAML/resource changes: 0
- startup changes: 0
- project/solution/package changes: 0
- existing documentation changes: 0
- `attachments/` file count: 0
- `data/local/` file count: 0
- project-root runtime artifacts: 0
- staged files: 0
- unexpected untracked files: 0
- running FamilyClaimRef processes: 0
- persistent environment mutation: 0

Expected final repository scope:

- exact untracked file:
  `docs/409_POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE2A_POLICY_CLAIM_MANAGEMENT_GUARDED_RUNTIME_VISUAL_SMOKE_RESULT_REVIEW.md`
- staged files: 0
- all other tracked/untracked changes: 0

## M. Separate Default-Startup Gates

The seven gates from `docs/408` remain separate:

1. Implementation result review.
2. Build and full regression.
3. Guarded management smoke.
4. Isolated-root create-flow validation.
5. Registration refresh runtime smoke.
6. Navigation and visual evidence.
7. Explicit user approval for default-startup change.

This batch provides HOLD evidence relevant to gates 3, 5, and 6. It does not merge, waive, or close the seven gates and does not authorize a default-startup change.

## N. Final Judgment And Minimal Repair Scope

Final judgment: **HOLD**

Exact marker:

`POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE2A_POLICY_CLAIM_MANAGEMENT_GUARDED_RUNTIME_VISUAL_SMOKE_HOLD`

Minimal blockers:

1. Product navigation must expose a stable UIA selected state for Home and subsequent navigation items.
2. Policy/claim list and combo item Automation Names must expose display titles only, not record `ToString()` values, IDs, or timestamps.
3. The default ProductShell window layout must keep the operation result message fully readable without requiring scrolling on the validated 820x520 capture size.

No repair was attempted in this batch.

Recommended next step:

1. Approve a narrow accessibility and default-size layout repair scope for the three blockers.
2. After repair, rerun the same guarded smoke with the same ten evidence states.
3. Keep user visual acceptance separate from the objective repair/recheck.
4. Do not proceed to default-startup change while the guarded smoke remains HOLD.

Recommended follow-up documentation commit message after user review:

`docs(familyclaimref): record phase2a management guarded smoke`
