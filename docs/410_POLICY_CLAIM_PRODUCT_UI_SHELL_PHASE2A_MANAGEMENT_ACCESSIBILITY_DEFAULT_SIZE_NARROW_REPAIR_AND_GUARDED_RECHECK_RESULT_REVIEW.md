# Product UI Shell Phase 2A Management Accessibility Default-Size Narrow Repair And Guarded Recheck Result Review

## A. Status Marker

`POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE2A_MANAGEMENT_REPAIR_OBJECTIVE_PASS_VISUAL_EVIDENCE_INTEGRITY_HOLD`

Status: **PASS for the approved objective implementation and runtime workflow checks; HOLD for visual evidence integrity and user visual acceptance.**

이 문서는 `docs/409`의 objective HOLD blocker 세 가지에 대한 exact narrow repair와 동일한 guarded runtime workflow 재검토 결과를 기록한다. ProductShell을 기본 시작 화면으로 전환하지 않았고, 사용자 visual acceptance를 대신 선언하지 않는다. 이후 사용자 visual review에서 기존 screenshot package의 무결성 finding 두 건이 확정되어 objective 결과와 visual evidence 결과를 분리해 기록한다.

## B. Starting Baseline And Docs/409

- Initial HEAD: `73808a52e7af7c9706d83ef3c905dd81fb3bf4c2`
- Initial subject: `feat(familyclaimref): add product policy claim management`
- Initial parent: `9706eccd39248d66bf2d40a8dd20a5bd1ff2207f`
- Initial tracked/staged changes: `0/0`
- Initial untracked file: `docs/409_POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE2A_POLICY_CLAIM_MANAGEMENT_GUARDED_RUNTIME_VISUAL_SMOKE_RESULT_REVIEW.md`
- `docs/409` SHA-256: `232946B8E534DE44E93597D2A90550948B569D49CA7BBCDA58EB79F3C27E8927`
- `docs/409` judgment: `HOLD`
- FamilyClaimRef process before work: `0`
- `docs/410` collision before work: none

`docs/409` was committed without content modification:

- Commit: `81718175b3b680cb1d5872872eb904be4efab9ee`
- Subject: `docs(familyclaimref): record phase2a management guarded smoke`
- Parent: `73808a52e7af7c9706d83ef3c905dd81fb3bf4c2`
- Exact committed file count: `1`
- Committed/worktree SHA-256 mismatch: `0`

## C. Narrow Repair Scope And Commit

Created:

- `tests/FamilyClaimRef.App.Tests/ProductPolicyClaimAccessibilityLayoutContractTests.cs`

Modified:

- `app/FamilyClaimRef.App/ProductShell/ProductShellWindow.xaml`
- `app/FamilyClaimRef.App/Views/ProductPolicyContractsView.xaml`
- `app/FamilyClaimRef.App/Views/ProductClaimCasesView.xaml`
- `app/FamilyClaimRef.App/Views/ProductDocumentRegistrationView.xaml`

All other implementation changes: `0`.

Repair details:

- Product navigation now uses one WPF `ListBox` with `SelectionMode="Single"` and the existing two-way `SelectedNavigationItem` binding.
- Navigation containers bind `AutomationProperties.Name` to user-facing `DisplayText`.
- Policy rows, claim policy options, claim rows, registration policy/claim targets, and document-type options bind container names to display-only properties.
- Policy and claim management views use fixed outer Grid rows; the active list owns growth/scrolling and the result region remains in its own bottom row.
- ProductShell default `Width="820"` and `Height="520"` remain unchanged.

Implementation commit:

- Commit: `614833892ad82177a5541eea46265f24d1612046`
- Subject: `fix(familyclaimref): repair product management accessibility`
- Parent: `81718175b3b680cb1d5872872eb904be4efab9ee`
- Exact committed file count: `5`
- Modify/create: `4/1`
- Committed/worktree blob mismatch: `0`

## D. Static, Build, And Automated Test Results

Static verification:

- Exact implementation scope: `5`
- Other source changes: `0`
- `git diff --check`: PASS
- Trailing whitespace findings: `0`
- EOF newline failures: `0`
- XAML/build errors: `0`
- `UiStrings.xaml`/`UiTextKeys.cs` changes: `0`
- Resource/constants: `91/91`
- `Ui.Product.*`: `35/35`
- Raw-object Automation Name bindings: `0`
- Default ProductShell size changes: `0`
- Existing test deletion/weakening/skip additions: `0`

New tests: `8`.

| Test command scope | Passed | Failed | Skipped |
|---|---:|---:|---:|
| `ProductPolicyClaimAccessibilityLayoutContractTests` | 8 | 0 | 0 |
| `ProductPolicyClaimManagementIntegrationTests` | 2 | 0 | 0 |
| `PolicyClaimManagementViewModelTests` | 24 | 0 | 0 |
| `ProductShellViewModelTests` | 15 | 0 | 0 |
| `ResourceUiTextProviderTests` | 50 | 0 | 0 |
| `DocumentRegistrationViewModelTests` | 26 | 0 | 0 |
| Full solution | 425 | 0 | 0 |

- Existing 417-test baseline losses: `0`
- Build warnings/errors: `0/0`
- Post-commit build warnings/errors: `0/0`

## E. Committed Runtime Provenance

- Configuration: `Debug`
- Target framework: `net10.0-windows`
- EXE: `app/FamilyClaimRef.App/bin/Debug/net10.0-windows/FamilyClaimRef.App.exe`
- EXE SHA-256: `2745F3986673180DEF9AE0C336022D8E0F81A2816C098BBFD77C0F3C7F52E0D4`
- EXE last-write UTC: `2026-07-23T09:40:17.0833490Z`
- DLL: `app/FamilyClaimRef.App/bin/Debug/net10.0-windows/FamilyClaimRef.App.dll`
- DLL SHA-256: `009BF213C0A8958A9895A6451C131EF08E2B42C94E52A789A06AD33387943E3E`
- DLL last-write UTC: `2026-07-23T09:40:17.0397529Z`
- Provenance capture UTC: `2026-07-23T09:40:36.9628031Z`
- `dotnet run` use: `0`

## F. Isolated Runtime Environment

- Run ID: `20260723-184036834-c18505e0`
- Normalized root: `<TEMP>\FamilyClaimRef-Phase2A-ManagementRepairRecheck\<run-id>`
- Child-only override:
  - `FAMILYCLAIMREF_ENABLE_DEV_RUNTIME_ROOT_OVERRIDE=1`
  - `FAMILYCLAIMREF_RUNTIME_ROOT=<child-specific-root>`
- Persistent Process/User/Machine environment mutations: `0`
- Production runtime root access: `0`
- Real personal, insurer, hospital, diagnosis, policy-number, or claim-number samples: `0`

## G. Default Startup And Guarded Preview

Default launch:

- Argument: none
- Window: existing `MainWindow`
- Top-level FamilyClaimRef windows: `1`
- ProductShell exposure: `0`
- Screenshot: `00_default_startup.png`
- Normal close accepted: yes
- Exit within 10 seconds: yes
- Forced termination: `0`

Preview launch:

- Argument: `--product-shell-preview`
- Window: `ProductShellWindow`
- Top-level FamilyClaimRef windows: `1`
- Navigation items: `5`
- Initial content: Home
- Captured ProductShell size: `820x520`
- UIA-observed content size: `806x513`
- DPI/scaling: `95.99 DPI / 100%`

## H. Navigation UIA Selection And Names

All five navigation containers exposed `SelectionItemPattern`. Every observed state had exactly one selected item.

| Navigation target | UIA Name | Target selected | Other four selected | Selected count |
|---|---|---|---|---:|
| Home | `홈` | true | false | 1 |
| PolicyContracts | `보험 계약` | true | false | 1 |
| ClaimCases | `청구 건` | true | false | 1 |
| DocumentRegistration | `문서 등록` | true | false | 1 |
| DocumentList | `문서 목록` | true | false | 1 |

Stable navigation item screen rectangles were:

- Home: `(158,226,200,20)`
- PolicyContracts: `(158,246,200,20)`
- ClaimCases: `(158,266,200,20)`
- DocumentRegistration: `(158,286,200,20)`
- DocumentList: `(158,306,200,20)`

ViewModel CLR type or namespace exposed as navigation Name: `0`.

## I. Display-Only Automation Name Matrix

| Context | Observed UIA item Name | Result |
|---|---|---|
| Navigation five items | `홈`, `보험 계약`, `청구 건`, `문서 등록`, `문서 목록` | PASS |
| Policy active row | `Smoke Policy A` | PASS |
| Claim policy selector option | `Smoke Policy A` | PASS |
| Claim active row | `Smoke Claim A` | PASS |
| Registration policy target option | `Smoke Policy A` | PASS |
| Registration claim target option | `Smoke Claim A` | PASS |
| Registration document-type options | `영수증`, `진단서`, `약제비 서류`, `통원 확인 서류`, `입퇴원 확인 서류`, `수술 확인 서류`, `기타` | PASS |

Document-type options were inspected through UIA `ExpandCollapsePattern`; no file selection or registration workflow was invoked.

Whole-snapshot UIA forbidden scan:

- GUID patterns: `0`
- ISO/record timestamps: `0`
- `PolicyRecord`/`ClaimRecord`: `0`
- ViewModel CLR type/namespace: `0`
- JSON file names: `0`
- local paths: `0`
- stack traces: `0`
- document-type code/scope/sort/order metadata: `0`
- item Name/display-text mismatches found: `0`

## J. Result-Region Layout Evidence

UIA screen-coordinate viewport:

- ProductShell window: `(130,130,820,520)`
- Window bottom: `650`

Policy/claim result region:

- Result GroupBox: `(376,561,548,63)`
- Result text: `(394,590,512,16)`
- Result text bottom: `606`
- Result text inside viewport: yes
- Outer management-screen vertical scrollbar: none
- List-owned scrolling remains available when list content grows.

Objective visual review:

| Screenshot | Message state | Fully visible | Outer scroll required |
|---|---|---|---|
| `03_policy_created.png` | `보험 계약을 등록했습니다.` | yes | no |
| `04_policy_duplicate_rejected.png` | `같은 이름의 활성 보험 계약이 이미 있습니다.` | yes | no |
| `05_claim_created.png` | `청구 건을 등록했습니다.` | yes | no |
| `06_claim_input_retained.png` | result reset/blank | result region fully visible | no |

No clipping, overlap, ellipsis, forced line break, reduced font size, auto-maximize, or default-size increase was used.

## K. Runtime Workflow Results

| Step | Result | Evidence |
|---|---|---|
| Home initial | PASS | Home selected, selected count 1 |
| Policy empty | PASS | active policy count 0 |
| Create `Smoke Policy A` | PASS | active policy count 1, success message |
| Reject `  smoke policy a  ` | PASS | count remained 1, duplicate message, input retained |
| Create `Smoke Claim A` | PASS | active claim count 1, success message |
| Retain `Unsaved Claim Draft` | PASS | survived PolicyContracts/ClaimCases round trip |
| Entry message reset | PASS | previous result cleared on re-entry |
| Registration targets present | PASS | one policy and one claim target; claim selected |
| Disable claim | PASS | claim removed; draft retained |
| Disable policy | PASS | policy removed; duplicate input retained |
| Registration targets removed | PASS | policy/claim titles absent after re-entry |
| Stale target correction | PASS | selected target cleared; empty claim-target message visible |

Unexpected FamilyClaimRef runtime counts:

- Product dialogs: `0`
- OpenFileDialog/file picker opens: `0`
- Document registration workflow executions: `0`
- Product crashes: `0`

## L. Interaction Audit

- Actual mouse clicks: `35` (`1` default close + `34` preview interactions)
- UIA `element_index`-targeted clicks: `35`
- UIA-targeted click ratio: `100%`
- Screenshot-only identified clicks: `0`
- Blind coordinate clicks: `0`
- Bounded coordinate carry-forward clicks: `0`
- Direct ViewModel calls: `0`
- Storage JSON edits: `0`

Click categories:

| UIA target category | Count | Name/control evidence |
|---|---:|---|
| Navigation items | 11 | user-facing Name, `ListItem`, stable rectangles in section H |
| Policy input/actions | 4 | policy title `Edit`, create `Button` |
| Claim policy selector/option | 2 | `ComboBox`, `Smoke Policy A` `ListItem` |
| Claim input/actions | 4 | title label/edit and create `Button`; one harmless label-target no-op was retried by the correct edit element |
| Registration target kind/target controls | 9 | UIA `ComboBox`/`ListItem`; policy/claim display names only |
| Document type selector | 1 | UIA `ComboBox`; option names cross-checked through `ExpandCollapsePattern` |
| Disable actions | 2 | claim/policy disable `Button` |
| Window close buttons | 2 | default/preview `닫기` `Button` |

Every click used the bounding rectangle cached for its current UIA element index. No coordinate value was supplied to the click API. The public UIA snapshot serializer did not emit every cached rectangle; selected-state, key item, result-region, and viewport rectangles were independently cross-checked with a read-only TEMP UIA inspector.

## M. Screenshot Manifest

| File | Pixel size | DPI | SHA-256 | State |
|---|---:|---:|---|---|
| `00_default_startup.png` | 900x760 | 95.99 | `FBCA9524071A17D70F079E68DE229FE3A4209D078E297ECF7098593E7D3CC3AC` | existing MainWindow |
| `01_product_shell_home_initial.png` | 820x520 | 95.99 | `6FFF0C01A9AD3C2CA69178213C4DB50DF8834BDC5CD3021A9F230B00631E1489` | Home initial |
| `02_policy_empty.png` | 820x520 | 95.99 | `83EDCA374C93981D19D631DEDDA4F2A54972D3CDE8685729D1D30A62ED30D51F` | policy empty |
| `03_policy_created.png` | 820x520 | 95.99 | `766914B67D1F0A6C1F36B21355932CBFC0FFE5CF4D9520C03B55C8FF351D7B3A` | policy created |
| `04_policy_duplicate_rejected.png` | 820x520 | 95.99 | `5E1417E0B7BDDFA9917A779BDFA019FB607BC57CC4334215B5AA78879544D2EA` | duplicate rejected |
| `05_claim_created.png` | 820x520 | 95.99 | `3C7C0C2D292B66386A88D937E22903227DAE8A441896821E7B78B85040F515AD` | claim created |
| `06_claim_input_retained.png` | 820x520 | 95.99 | `77208D4B5551ABC39E426E07C1858C5B89D47B14BF52B782800F62A320A59AE8` | draft retained/message reset |
| `07_registration_targets_present.png` | 820x520 | 95.99 | `11935B835264D0FA3B66DCC98E6716344098070F527D9B977A5A6D5E27C9786F` | claim target selected |
| `08_management_targets_disabled.png` | 820x520 | 95.99 | `4A16238B94BE42593FDDDD7C77B83929FB4A8DB9A0AC909AD17A2D93BC8E50AE` | claim/policy disabled |
| `09_registration_targets_removed.png` | 820x520 | 95.99 | `D9B749079C788C7DEA08C94492D6F49035B8FE7C40FB36AEF8916F8A59AEAF01` | targets removed |

Screenshot count: `10/10`.

Evidence directory:

`<TEMP>\FamilyClaimRef-Phase2A-ManagementRepairRecheck\<run-id>\evidence`

## N. User Visual Review And Evidence Integrity Reconciliation

사용자 visual review에서 다음 두 finding이 확정되었다.

| Screenshot | SHA-256 | Finding | Visual integrity |
|---|---|---|---|
| `01_product_shell_home_initial.png` | `6FFF0C01A9AD3C2CA69178213C4DB50DF8834BDC5CD3021A9F230B00631E1489` | 오른쪽 아래에 `UiaInspector.exe` 응용 프로그램 오류 dialog가 노출됨 | contaminated |
| `07_registration_targets_present.png` | `11935B835264D0FA3B66DCC98E6716344098070F527D9B977A5A6D5E27C9786F` | `FamilyClaimRef` branding 앞부분이 잘려 `hilyClaimRef`로 보이고 문서 등록 화면 제목이 보이지 않음 | clipped/invalid |

두 SHA-256은 section M의 기존 manifest와 일치한다. 따라서 전달 또는 업로드 과정의 변형이 아니라 저장된 screenshot 자체의 finding으로 판정한다.

- Screenshot captured count: `10/10`
- Clean accepted visual evidence count: `8/10`
- Saved screenshot에 노출된 external/test-infrastructure dialog: `1`
- FamilyClaimRef product dialog: `0`
- Contaminated screenshot: `01_product_shell_home_initial.png`
- Clipped/invalid screenshot: `07_registration_targets_present.png`
- User visual acceptance: `HOLD`
- Objective implementation repair: `PASS`
- Runtime functional workflow: `PASS`
- Source repair required: `0`
- Clean recapture required: yes

기존 build `0/0`, full tests `425/425`, UIA accessibility PASS, management workflow PASS 결과는 변경하지 않는다. 동일한 committed implementation으로 새 격리 run을 수행하고 10개 screenshot을 모두 다시 캡처해야 한다.

## O. Normal Close, Cleanup, And Repository Safety

- Default close accepted/exit within 10 seconds: yes
- Preview close accepted/exit within 10 seconds: yes
- Forced termination: `0`
- FamilyClaimRef process residue: `0`
- Unexpected ProductShell dialog: `0`
- Default isolated root files before cleanup: `0`
- Preview isolated root files before cleanup: `2`
- Exact default/preview roots after cleanup: missing
- Evidence/logs/harness: preserved
- Production root access/deletion: `0/0`
- Persistent environment mutation: `0`
- Project root `attachments/` files: `0`
- Project root `data/local/` files: `0`
- Project root `runtime_test_document.*`: `0`
- Unexpected DB/SQLite files in checked project/app/test scope: `0`
- `data/claimdoc/` content access: `0`

## P. Separate Seven Gates

The seven gates remain separate and do not authorize a default-startup change.

| Gate | Status | Evidence |
|---:|---|---|
| 1. Implementation result review | PASS | exact five-file repair committed |
| 2. Build and full regression | PASS | build `0/0`; full tests `425/425` |
| 3. Guarded management smoke | PASS | complete synthetic lifecycle workflow |
| 4. Isolated-root create-flow validation | PASS | policy/claim create and disable in child-only root |
| 5. Registration refresh runtime smoke | PASS | targets present, removed, stale selection cleared |
| 6. Navigation and visual evidence | PASS objective / visual evidence integrity HOLD | UIA single selection PASS; captured 10/10; clean accepted 8/10; screenshot 01 contaminated; screenshot 07 clipped/invalid |
| 7. Explicit user approval for default-startup change | NOT GRANTED | existing MainWindow remains default |

## Q. Final Judgment

Objective repair judgment: **PASS**

Runtime functional workflow judgment: **PASS**

Visual evidence integrity judgment: **HOLD**

User visual acceptance: **HOLD**

Exact marker:

`POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE2A_MANAGEMENT_REPAIR_OBJECTIVE_PASS_VISUAL_EVIDENCE_INTEGRITY_HOLD`

Resolved blockers:

1. Navigation exposes user-facing Names and stable UIA single-selection state.
2. Policy/claim/registration/document-type items expose display-only Names without internal IDs, records, timestamps, paths, or metadata.
3. Policy/claim result messages remain fully visible at 820x520 without outer screen scrolling.

User visual acceptance is on HOLD because clean accepted evidence is `8/10`. No product source repair requirement was found. A clean recapture of all 10 states is required. Gate 7 remains closed, and this batch does not change the default startup screen.

## R. Next Recommendation

Use the same committed implementation in a new isolated run, recapture all 10 screenshots without mixing prior images, and submit the clean package for user visual acceptance. Do not infer default-startup authorization from the objective PASS.
