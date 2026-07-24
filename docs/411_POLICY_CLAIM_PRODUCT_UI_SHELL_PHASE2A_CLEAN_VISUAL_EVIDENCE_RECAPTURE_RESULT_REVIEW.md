# Product UI Shell Phase 2A Clean Visual Evidence Recapture Result Review

## A. Status Marker

`POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE2A_USER_VISUAL_ACCEPTANCE_PASS_GATE7_DEFAULT_STARTUP_TRANSITION_APPROVED`

Status: **PASS for clean visual evidence recapture and user visual acceptance; Gate 7 default-startup transition is explicitly approved.**

이 문서는 동일한 committed implementation으로 새 격리 run을 수행하여 기존 screenshot package의 visual evidence integrity finding을 정정한 결과를 기록한다. 제품 source, XAML, ViewModel, code-behind, tests, resources/constants, startup, project/solution/package, runtime storage contract, default window size는 변경하지 않았다.

## B. Docs/410 Evidence Hold Reconciliation

기존 visual evidence finding:

1. `01_product_shell_home_initial.png`
   - 기존 SHA-256: `6FFF0C01A9AD3C2CA69178213C4DB50DF8834BDC5CD3021A9F230B00631E1489`
   - 오른쪽 아래에 `UiaInspector.exe` 응용 프로그램 오류 dialog가 노출됨.
2. `07_registration_targets_present.png`
   - 기존 SHA-256: `11935B835264D0FA3B66DCC98E6716344098070F527D9B977A5A6D5E27C9786F`
   - `FamilyClaimRef` branding 앞부분과 문서 등록 화면 제목이 잘려 정상 top-position evidence가 아니었음.

`docs/410`은 위 finding을 반영해 다음 marker로 정정되었다.

`POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE2A_MANAGEMENT_REPAIR_OBJECTIVE_PASS_VISUAL_EVIDENCE_INTEGRITY_HOLD`

Evidence HOLD commit:

- Commit: `06149fe59fae71d8d1a0421fb4559f7af7454c07`
- Subject: `docs(familyclaimref): record phase2a repair evidence hold`
- Parent: `614833892ad82177a5541eea46265f24d1612046`
- Exact committed file: `docs/410_POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE2A_MANAGEMENT_ACCESSIBILITY_DEFAULT_SIZE_NARROW_REPAIR_AND_GUARDED_RECHECK_RESULT_REVIEW.md`
- Exact committed file count: `1`
- Source/test/XAML changes in that commit: `0`

The objective implementation repair and runtime functional workflow remained PASS. Only the prior visual evidence package was placed on HOLD.

## C. Committed Source And Build

- Implementation commit: `614833892ad82177a5541eea46265f24d1612046`
- Subject: `fix(familyclaimref): repair product management accessibility`
- Runtime HEAD after docs/410 commit: `06149fe59fae71d8d1a0421fb4559f7af7454c07`
- Baseline `app` tree: `438b5d26affbefeed509e0532a3c0bf5cefd88cb`
- Runtime HEAD `app` tree: `438b5d26affbefeed509e0532a3c0bf5cefd88cb`
- Baseline `tests` tree: `c0daeb50c3d88c96fb011ccb6802fc5eb90411dc`
- Runtime HEAD `tests` tree: `c0daeb50c3d88c96fb011ccb6802fc5eb90411dc`
- Product source hash change: `0`
- Test source hash change: `0`
- Build configuration: `Debug`
- Build warnings/errors: `0/0`
- Initial sandbox build: Windows SDK user-path access denied
- Elevated retry: PASS
- Full test result: carry-forward `425/425`, failed/skipped `0/0`
- Full test rerun: not performed because source/test trees were unchanged and the batch authorized carry-forward

Current build provenance:

- EXE: `app/FamilyClaimRef.App/bin/Debug/net10.0-windows/FamilyClaimRef.App.exe`
- EXE SHA-256: `B764F737B15DEFB5FC040D716FBDE7BDAF0D75BB2B583F4F2E3C306B55DC0062`
- EXE last-write UTC: `2026-07-24T00:47:08.8629845Z`
- DLL: `app/FamilyClaimRef.App/bin/Debug/net10.0-windows/FamilyClaimRef.App.dll`
- DLL SHA-256: `8A1E855B3D70B4B660A4251EF199671C8CC51A6419CE3610209388BEA1CF2017`
- DLL last-write UTC: `2026-07-24T00:47:08.8290633Z`
- `dotnet run` use: `0`

## D. New Isolated Run

- Run ID: `20260724-094855441-67ec06a7`
- Normalized root: `<TEMP>\FamilyClaimRef-Phase2A-ManagementCleanRecapture\<run-id>`
- Evidence directory: `<run-root>\evidence`
- Logs directory: `<run-root>\logs`
- Harness directory: `<run-root>\harness`
- Default child root: `<run-root>\default`
- Preview child root: `<run-root>\preview`
- Child-only environment:
  - `FAMILYCLAIMREF_ENABLE_DEV_RUNTIME_ROOT_OVERRIDE=1`
  - `FAMILYCLAIMREF_RUNTIME_ROOT=<child-specific-root>`
- Persistent Process/User/Machine environment values after run: unset
- Production runtime root access/deletion: `0/0`
- Real personal, insurer, hospital, diagnosis, policy-number, or claim-number samples: `0`

The TEMP harness used a no-window launcher for child-only environment injection and a no-window capture helper for exact PNG output. `UiaInspector` source/binary was not copied into the new run and was not executed.

## E. Default And Guarded Preview

Default launch:

- Argument: none
- Window: existing `MainWindow`
- ProductShell exposure: `0`
- Screenshot: `00_default_startup.png`
- Captured dimensions: `900x760`
- Normal close through the UIA `닫기` button: yes
- Forced termination: `0`

Guarded preview launch:

- Argument: `--product-shell-preview`
- Window: `ProductShellWindow`
- Initial content: Home
- Navigation items: `5`
- Window dimensions: `820x520`
- DPI/scaling: `95.99 DPI / 100%`
- Normal close through the UIA `닫기` button: yes
- Forced termination: `0`

## F. Replayed Synthetic Workflow

| Step | Result | Evidence |
|---|---|---|
| Home initial | PASS | branding, Home title, navigation fully visible |
| Policy empty | PASS | active policy count 0 |
| Create `Smoke Policy A` | PASS | one active policy and success message |
| Reject `  smoke policy a  ` | PASS | active count stayed 1; duplicate message; input retained |
| Create `Smoke Claim A` | PASS | one active claim and success message |
| Retain `Unsaved Claim Draft` | PASS | retained through PolicyContracts/ClaimCases round trip |
| Entry message reset | PASS | previous claim-create result cleared |
| Registration targets present | PASS | claim target kind and `Smoke Claim A` selected |
| Disable claim | PASS | active claim removed; draft retained |
| Disable policy | PASS | active policy removed; duplicate input retained |
| Registration targets removed | PASS | policy/claim display titles absent |
| Stale target correction | PASS | selected target cleared; empty claim-target message visible |

- OpenFileDialog/file picker opens: `0`
- Document registration workflow executions: `0`
- Direct ViewModel calls: `0`
- Storage JSON edits: `0`
- Product crashes: `0`

One non-mutating `set_value` request was rejected by the WPF UIA cache before text entry. It created no dialog, changed no app state, and left no process. The synthetic policy title was then entered through an actual focused edit control and normal typing.

## G. Old/New Finding Comparison

| Screenshot | Old evidence | New evidence | Correction |
|---|---|---|---|
| `01_product_shell_home_initial.png` | SHA `6FFF0C01A9AD3C2CA69178213C4DB50DF8834BDC5CD3021A9F230B00631E1489`; `UiaInspector.exe` error dialog visible | SHA `C40BC20B618C696B44BA050D4233437564C329CFDDC0380FBFC11DA312D80EA0`; external dialog count 0 | clean |
| `07_registration_targets_present.png` | SHA `11935B835264D0FA3B66DCC98E6716344098070F527D9B977A5A6D5E27C9786F`; branding/title clipped | SHA `BC75B30EAEC938063CEEDCCD634C6D57C4565A8E1EE57F58908F4C9B085713F3`; full branding/title/navigation/claim target visible | clean |

The new package contains only images generated in run `20260724-094855441-67ec06a7`. No prior image was copied or mixed into the new evidence directory.

## H. New Screenshot Manifest

| File | Dimensions | DPI | SHA-256 | Runtime state | External dialog | Branding/title | Clipping/overlap | Scroll offset | Clean |
|---|---:|---:|---|---|---:|---|---:|---:|---|
| `00_default_startup.png` | 900x760 | 95.99 | `A93533E62729BC563B39E7F20EC12B015A5E121341D9C1EDC564E7292C325982` | existing MainWindow | 0 | full | 0 | top/0 | PASS |
| `01_product_shell_home_initial.png` | 820x520 | 95.99 | `C40BC20B618C696B44BA050D4233437564C329CFDDC0380FBFC11DA312D80EA0` | Home initial | 0 | full | 0 | 0 | PASS |
| `02_policy_empty.png` | 820x520 | 95.99 | `4B82211EA9059DB900355EF53E7D50F1509B9D9AF782CB8C0BDBBC1EB0491E2E` | policy empty | 0 | full | 0 | 0 | PASS |
| `03_policy_created.png` | 820x520 | 95.99 | `B10D0DA8716A5B98C7960E8544649373B64EF2A251720B9E1ACA1D90E122A4F1` | policy created | 0 | full | 0 | 0 | PASS |
| `04_policy_duplicate_rejected.png` | 820x520 | 95.99 | `A999C3C99B61BAA3A9ED6BBEB64F0C60FDA86FA6FD0B8C68B870F37BF09A32FD` | duplicate rejected | 0 | full | 0 | 0 | PASS |
| `05_claim_created.png` | 820x520 | 95.99 | `D4053ADC503CBD0104D3560AA2AE2A5DAA0F42BE2F33BFBF1D614F0B981595EA` | claim created | 0 | full | 0 | 0 | PASS |
| `06_claim_input_retained.png` | 820x520 | 95.99 | `0C53DE6A1EDCE3BB0D7966A7381E26F95DB50E3CDFCD82BC8ACE50145AA00FD1` | draft retained/message reset | 0 | full | 0 | 0 | PASS |
| `07_registration_targets_present.png` | 820x520 | 95.99 | `BC75B30EAEC938063CEEDCCD634C6D57C4565A8E1EE57F58908F4C9B085713F3` | claim target selected | 0 | full | 0 | top/0 | PASS |
| `08_management_targets_disabled.png` | 820x520 | 95.99 | `955BFE5BDE5EA2A724C56ADB496A1285B59947A3FCEE149A244B05682BAF4EA3` | claim/policy disabled | 0 | full | 0 | 0 | PASS |
| `09_registration_targets_removed.png` | 820x520 | 95.99 | `5B4011681AFD89DDFAA136781CCF9DDA2219BE82604263FB3362FB2D2C9AEFD3` | targets removed/stale cleared | 0 | full | 0 | top/0 | PASS |

- Screenshot count: `10/10`
- Clean accepted evidence count: `10/10`
- Missing expected screenshots: `0`
- Unexpected evidence files: `0`
- ProductShell bounds across screenshots 01-09: `(78,78,820,520)`
- Nonzero abnormal horizontal offset: `0`

## I. Dialog, Inspector, And Process Integrity

- External/test-infrastructure dialogs visible in saved screenshots: `0`
- FamilyClaimRef product dialogs: `0`
- `UiaInspector` executions: `0`
- `UiaInspector` error dialogs: `0`
- UIA inspector crash/hang: `0`
- UIA inspector process residue: `0`
- Capture helper error files: `0`
- Capture helper process residue: `0`
- FamilyClaimRef process residue after normal close: `0`
- Screenshot contamination: `0`

## J. Cleanup And Repository Safety

- Default isolated root files before cleanup: `0`
- Preview isolated root files before cleanup: `2`
  - `data/local/policies.json`
  - `data/local/claims.json`
- Exact default/preview roots after cleanup: missing
- Evidence/logs/harness: preserved
- Production runtime root access/deletion: `0/0`
- Persistent environment mutation: `0`
- Project root `attachments/` files: `0`
- Project root `data/local/` files: `0`
- Project root `runtime_test_document.*`: `0`
- Unexpected DB/SQLite files in checked root/app/test scope: `0`
- `data/claimdoc/` content access: `0`
- `data/claimdoc/` ignore rule: preserved
- `docs/nightwork_20260706/` ignore rule: preserved

## K. Separate Seven Gates

## K. User Visual Acceptance And Gate 7 Approval

On `2026-07-24`, the user explicitly reviewed and accepted the clean recapture package and authorized Gate 7.

- Clean recapture screenshots confirmed by the user: `10/10`
- External dialog contamination: `0`
- Branding/title clipping: `0`
- Phase 2A ProductShell visual acceptance: **PASS**
- Current functional ProductShell UI approved as the target for the Gate 7 default-startup transition: **yes**
- Final commercial UI completion or deployment readiness approval: **no**
- Home content expansion: **deferred**
- Existing `MainWindow` approved as the default product start: **no**
- Gate 7 default-startup transition explicitly approved by the user on `2026-07-24`: **yes**

This approval authorizes only the bounded default-startup transition. It does not authorize product UI expansion, deployment, installer/package work, storage changes, or production readiness claims.

## L. Separate Seven Gates

| Gate | Status | Evidence |
|---:|---|---|
| 1. Implementation result review | PASS | exact five-file repair remains committed at `6148338` |
| 2. Build and full regression | PASS | fresh build `0/0`; unchanged-source test result carried forward `425/425` |
| 3. Guarded management smoke | PASS | complete synthetic lifecycle workflow replayed |
| 4. Isolated-root create-flow validation | PASS | policy/claim create and disable in child-only preview root |
| 5. Registration refresh runtime smoke | PASS | targets present, removed, and stale selection cleared |
| 6. Navigation and clean visual evidence | PASS / user accepted | new-run screenshots `10/10`; clean accepted `10/10`; dialogs/clipping 0 |
| 7. Explicit user approval for default-startup change | APPROVED | ProductShell is approved as the bounded default-startup transition target; existing MainWindow is not approved as the default product start |

## M. Final Judgment

- Objective implementation repair: **PASS**
- Runtime functional workflow: **PASS**
- Clean visual evidence recapture: **PASS**
- Clean accepted evidence: **10/10**
- Source repair required: `0`
- User visual acceptance: **PASS**
- Default-startup transition: **APPROVED**
- Final commercial UI completion: **NOT APPROVED**
- Deployment/production readiness: **NOT APPROVED**
- Home content expansion: **DEFERRED**

Exact marker:

`POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE2A_USER_VISUAL_ACCEPTANCE_PASS_GATE7_DEFAULT_STARTUP_TRANSITION_APPROVED`

## N. Next Recommendation

Proceed with the separately bounded Gate 7 default-startup transition. Preserve `MainWindow` source, keep Home expansion deferred, and make no deployment or production-readiness claim.
