# FamilyMember Persistence T3-PER-B Implementation and Evidence

## A. Status

- Product/runtime verdict: `PASS`

- Risk tier: `T3_HIGH`
- Source implementation: completed
- Automated validation: pass
- Independent source/test review: pass
- Product runtime process launch: completed with isolated TEMP root and one approved clean relaunch
- User-assisted Product runtime create/update/deactivate/reactivate phases: pass
- Defect repairs: implemented; automated and user-assisted runtime rechecks passed
- Stage/commit/push: `0/0/0`

The repaired Product UI flow completed the required user-assisted runtime
rechecks. The single terminal marker is recorded in the final judgment.

## B. Baseline

- Repository: `C:\EtcProject\FamilyClaimRef`
- Branch: `main`
- HEAD: `aecf7edfd43b4124ec5ff17d35687020cf4c0d90`
- Subject: `docs(familyclaimref): record gate8 registration persistence decision package`
- Initial tracked/staged/untracked: `46/0/43`
- Initial status entries: `89`
- `docs/439` SHA-256:
  `34B6BD6A26D3552DF0B3297C7D0FC8560EA4FE02969478D47E09F71C95F14306`
- `docs/440` SHA-256:
  `3C78449820EDDA3E9C67C07AE789D2BAE3024DC7373922E6323F532F05B4BDE2`

The existing dirty and untracked paths were preserved as user work. No reset,
checkout, restore, clean, stash, rebase, stage, commit, or push was performed.

## C. User Decisions

All ten decisions remain `USER_SELECTED`.

| Decision | Selected option | Implemented in this batch |
|---|---|---|
| `DEC-PER-001` | `USER_SELECTED_OPTION_B` | No |
| `DEC-PER-002` | `USER_SELECTED_OPTION_A` | No |
| `DEC-PER-003` | `USER_SELECTED_OPTION_A` | Yes |
| `DEC-PER-004` | `USER_SELECTED_OPTION_A` | Yes |
| `DEC-PER-005` | `USER_SELECTED_OPTION_A` | No |
| `DEC-PER-006` | `USER_SELECTED_OPTION_A` | No |
| `DEC-PER-007` | `USER_SELECTED_OPTION_A` | No |
| `DEC-PER-008` | `USER_SELECTED_OPTION_A` | Yes |
| `DEC-PER-009` | `USER_SELECTED_OPTION_A_WITH_INTEGER_VERSION` | Yes |
| `DEC-PER-010` | `USER_SELECTED_OPTION_A` | Yes |

`DEC-PER-001/002` Policy persistence and `DEC-PER-005/006/007` Category
persistence remain selected but not implemented.

## D. Implemented Contract

### D1. FamilyMember data

`FamilyMemberRecord` contains exactly:

- `Id`
- `DisplayName`
- `Relation`
- `Memo`
- `CreatedAt`
- `UpdatedAt`
- `DisabledAt`
- `Version`

Rules implemented:

- opaque system-generated `Id`
- trimmed required `DisplayName`
- duplicate display names allowed
- approved relation values only, in exact order: `본인`, `남편`, `아들`, `딸`,
  `아버지`, `어머니`, `동생`, `할머니`, `할아버지`, `기타`
- optional trimmed `Memo`
- active state derived from `DisabledAt == null`
- create Version `1`
- successful update/deactivate/reactivate increments integer Version
- `CreatedAt` remains immutable
- `UpdatedAt` is audit time only, not a concurrency token
- no personal identity inference and no additional personal fields

### D2. JSON storage

- File: `family-members.json`
- Envelope schemaVersion: `1`
- Existing `JsonFileStore<T>` envelope and temp-write/final-move path reused
- One aggregate save per successful create/update/deactivate/reactivate
- static process-scoped gate keyed by normalized full store path
- explicit `Id` and `expectedVersion` required for update/deactivate/reactivate
- stale Version returns structured conflict without a write
- inactive or missing target returns structured unavailable result without a write
- no automatic retry, merge, cross-process lock, or hard delete

### D3. Product UI and commands

- Screen 02 uses the FamilyMember management list, including active and
  disabled rows, with explicit row `Id`/`Version` context for edit,
  deactivate, and reactivate.
- Screen 13 supports create and edit modes.
- `CMD-PER-008`: enabled for valid create/update input.
- `CMD-PER-009`: remains disabled; it does not redirect to deactivate.
- `CMD-PER-010`: enabled only for an explicit active edit target.
- Busy/reentry rejects a second mutation without queueing it.
- Product messages do not include raw ID, path, payload, memo, or exception text.
- Screen 02/13 guidance now describes persisted Family data instead of the old
  non-persistent placeholder behavior.
- Resource parity: total `116/116`, `Ui.Product.*` `60/60`.

### D4. Refresh failure

Mutation success is recorded before the post-write list refresh. If refresh
fails, the operation returns successful mutation state with a distinct safe
message. Create/update retain the new explicit target and Version so a retry
cannot repeat create or use a stale Version. Deactivate clears the edit target,
so the same current target cannot be deactivated again.

## E. Exact Changed Files

### CREATE - implementation and tests

- `app/FamilyClaimRef.App/Models/Storage/FamilyMemberDraft.cs`
- `app/FamilyClaimRef.App/Models/Storage/FamilyMemberRecord.cs`
- `app/FamilyClaimRef.App/Models/Storage/FamilyMemberRelationValues.cs`
- `app/FamilyClaimRef.App/Services/Storage/FamilyMemberStorageException.cs`
- `app/FamilyClaimRef.App/Services/Storage/IFamilyMemberStorageService.cs`
- `app/FamilyClaimRef.App/Services/Storage/JsonFamilyMemberStorageService.cs`
- `app/FamilyClaimRef.App/ViewModels/FamilyMemberManagementViewModel.cs`
- `app/FamilyClaimRef.App/Views/ProductFamilyMembersView.xaml`
- `app/FamilyClaimRef.App/Views/ProductFamilyMembersView.xaml.cs`
- `app/FamilyClaimRef.App/Views/ProductFamilyMemberEditorView.xaml`
- `app/FamilyClaimRef.App/Views/ProductFamilyMemberEditorView.xaml.cs`
- `tests/FamilyClaimRef.App.Tests/JsonFamilyMemberStorageServiceTests.cs`
- `tests/FamilyClaimRef.App.Tests/FamilyMemberManagementViewModelTests.cs`
- `tests/FamilyClaimRef.App.Tests/Integration/FamilyMemberPersistenceIntegrationTests.cs`

### MODIFY - minimal composition, route, copy, and regression coverage

- `app/FamilyClaimRef.App/Composition/AppServices.cs`
- `app/FamilyClaimRef.App/ProductShell/ProductShellWindow.xaml`
- `app/FamilyClaimRef.App/Resources/ProductScreenContent.xaml`
- `app/FamilyClaimRef.App/Resources/UiStrings.xaml`
- `app/FamilyClaimRef.App/Services/Localization/UiTextKeys.cs`
- `app/FamilyClaimRef.App/ViewModels/ProductScreenCatalog.cs`
- `app/FamilyClaimRef.App/ViewModels/ProductShellViewModel.cs`
- `tests/FamilyClaimRef.App.Tests/Composition/AppServicesTests.cs`
- `tests/FamilyClaimRef.App.Tests/DocumentRegistrationLifecycleGate8Tests.cs`
- `tests/FamilyClaimRef.App.Tests/ProductShellViewModelTests.cs`
- `tests/FamilyClaimRef.App.Tests/ProductWireframeRouteCoverageTests.cs`
- `tests/FamilyClaimRef.App.Tests/Services/Localization/ResourceUiTextProviderTests.cs`

### CREATE - combined evidence document

- `docs/441_FAMILY_MEMBER_PERSISTENCE_T3_PER_B_IMPLEMENTATION_AND_EVIDENCE.md`

### DELETE

- None

## F. Automated Evidence

### F1. Restore and build

```text
dotnet restore FamilyClaimRef.sln
PASS - all projects up to date

dotnet build FamilyClaimRef.sln --no-restore
PASS - warnings 0, errors 0
```

The first sandboxed restore was denied access to the local Microsoft SDK path.
The same approved command succeeded outside the sandbox; no global Git or SDK
configuration was changed.

### F2. Family focused tests

Command filter covered FamilyMember storage, ViewModel, integration,
ProductShell, route/XAML, composition, and resource tests.

```text
PASS - 142/142
failed 0, skipped 0
```

Direct evidence includes:

- create/update/deactivate/reload
- duplicate display names with distinct IDs
- active stale update and stale deactivate conflicts
- inactive target non-write failures
- deterministic shared process gate before the second provider load
- separate Product ViewModels competing on the same store: success 1,
  conflict 1
- actual target-lock final move failure: previous JSON unchanged and temp
  residue 0
- create/update/deactivate refresh-failure retry semantics
- delete disabled and explicit edit navigation

### F3. Policy/Claim and screens 17/18 regression tests

The focused filter covered Policy/Claim storage and lifecycle plus document
registration ViewModel, workflow, Gate 8 lifecycle, persistence, and negative
validation suites.

```text
PASS - 141/141
failed 0, skipped 0
```

### F4. Full suite

This result is the historical full-suite baseline from the initial
implementation stage. The final automated result after the bounded repairs is
`592/592` as recorded in I.2 and L.

```text
dotnet test FamilyClaimRef.sln --no-build
PASS - 586/586
failed 0, skipped 0
```

### F5. Static checks

- trailing whitespace: `0`
- merge markers: `0`
- local profile/protected data path findings in T3 files: `0`
- `git diff --check`: PASS; existing line-ending warnings only
- project root `attachments/` files: `0`
- project root `data/local/` files: `0`
- project root `runtime_test_document.*` files: `0`
- staged files: `0`
- running `FamilyClaimRef.App` processes after the attempt: `0`
- `docs/439` and `docs/440`: exact SHA-256 unchanged

## G. Product Runtime Attempt

This section records the historical state at the first automation attempt. Its
unexecuted items and HOLD were true at that time and are superseded by the
later user-assisted observations in I; they are not the final Product verdict.

The Product executable was launched once with:

- argument: `--product-shell-preview`
- guard: `FAMILYCLAIMREF_ENABLE_DEV_RUNTIME_ROOT_OVERRIDE=1`
- a new TEMP-only `FAMILYCLAIMREF_RUNTIME_ROOT`
- synthetic/non-personal scope only

Observed before input:

- Product process responsive: yes
- Product window handle and title present: yes
- production root access: no
- `data/claimdoc` access: no
- runtime root content before UI input: empty

The approved Computer Use path repeatedly returned the Product window under an
incorrect app identity and then rejected the same handle with a window-owner
mismatch. The automation session was reset and retried once, with the same
result. No alternate UIA, P03, R03, R07, or R08 harness was introduced.

Consequently, the following required Product runtime actions were unexecuted at
that historical point:

1. screen 13 create-mode input and save
2. screen 02/edit-mode persisted value confirmation
3. update and reload
4. deactivate and reload
5. UI stale-Version result
6. UI delete-disabled observation
7. UI raw ID/path/exception non-exposure observation

The Product process closed gracefully. The empty TEMP runtime root created by
this attempt was path-checked and removed. Runtime source and project files were
not modified.

## H. UI Runtime Closeout

This section records the historical automated closeout HOLD. The environment
blocker and unexecuted actions were true at that time and are superseded by the
later user-assisted runtime PASS in I; they are retained as execution history.

The screen 02/13 runtime-only closeout was retried on 2026-08-04 without
repeating build, tests, static audit, or independent review.

Execution environment:

- command: `C:\EtcProject\FamilyClaimRef\app\FamilyClaimRef.App\bin\Debug\net10.0-windows\FamilyClaimRef.App.exe --product-shell-preview`
- working directory: `C:\EtcProject\FamilyClaimRef\app\FamilyClaimRef.App\bin\Debug\net10.0-windows`
- runtime root: `C:\Users\jin8855\AppData\Local\Temp\FamilyClaimRef\T3_PER_B_Closeout_20260804_090537_283`
- evidence root: `C:\Users\jin8855\AppData\Local\Temp\FamilyClaimRef\T3_PER_B_Evidence_20260804_090537_283`
- timezone: `Korea Standard Time`
- first start: `2026-08-04T09:05:37.367+09:00`, PID `31168`, window handle `7344100`
- approved clean relaunch: `2026-08-04T09:06:26.656+09:00`, PID `37264`, window handle `17698156`
- top-level window title: `FamilyClaimRef` for both processes

The Win32 top-level handles returned by each Product process exactly matched
the window IDs returned by Computer Use. In both launches, however, Computer
Use classified the Product window as owned by `OneDrive.App.exe`. A read-only
window-state request then failed with the same contradictory ownership error:
the reported previous and current owner were both `OneDrive.App.exe`.

No click or keyboard input was issued because Product process ownership could
not be established through the approved automation path. The synthetic family
member was not entered, and no runtime JSON or screenshot was created.

| Required capture | exists | proves_claim | Result |
|---|---:|---:|---|
| Create mode Delete/Deactivate disabled | no | no | Blocked before input |
| Active list after create | no | no | Create not executed |
| Updated value after restart | no | no | Update not executed |
| Active-list exclusion after deactivate | no | no | Deactivate not executed |
| Exclusion after final restart | no | no | Final restart not executed |

Both Product processes accepted `CloseMainWindow` and exited. The runtime and
evidence roots contained zero files, were path-checked under the current TEMP
`FamilyClaimRef` directory, and were removed. Product process residue is zero.
Actual user data, the production runtime root, and `data/claimdoc` were not
accessed.

This was the second HOLD for the same window-owner misidentification. No new
automation harness or closure document was authorized. Its minimum action was
a user-performed observation of the five required screen 02/13 states in an
isolated synthetic runtime root; that historical action was completed by the
later observations in I.

- Runtime closeout Blocking: `1` (execution environment)
- Runtime closeout Major: `0`
- Runtime closeout Minor: `0`
- Product defect asserted by this HOLD: no

## I. User-Assisted Manual UI Defect and Repair

The user-assisted run started on 2026-08-04 with the already-built Product and
the approved isolated runtime override:

- PID: `6812`
- top-level window handle: `11603728`
- title: `FamilyClaimRef`
- runtime root: `C:\Users\jin8855\AppData\Local\Temp\FamilyClaimRef\T3_PER_B_Manual_20260804_092714_073`
- evidence root: `C:\Users\jin8855\AppData\Local\Temp\FamilyClaimRef\T3_PER_B_Manual_Evidence_20260804_092714_073`
- start: `2026-08-04T09:27:14.208+09:00`
- timezone: `Korea Standard Time`

The user stopped the runtime after the create phase and reported:

- Major: save succeeded but screen 13 did not return to screen 02
- Minor: the multiline memo input began vertically centered instead of at the
  upper-left corner
- Major: the relation contract exposed `본인 후보` and `가족 후보` instead of
  the approved exact relation list
- Blocking/Major/Minor: `0/2/1`

The Product process accepted `CloseMainWindow` and exited before repair work.
The synthetic runtime and screenshot evidence roots were not inspected,
modified, moved, or deleted during the repair.

Repair contract and implementation:

1. `ProductShellViewModel.SaveFamilyMemberAndReturnAsync` performs exactly one
   save attempt. It returns to screen 02 only when `SaveAsync` returns success.
   A failed save returns false and leaves screen 13 active.
2. `ProductFamilyMemberEditorView` delegates save to that coordinator. Screen
   02 uses the refreshed `AvailableMembers` state and its existing load path.
3. `FamilyMemberRelationValues.All` now contains exactly, in order: `본인`,
   `남편`, `아들`, `딸`, `아버지`, `어머니`, `동생`, `할머니`, `할아버지`,
   `기타`.
4. New writes reject `본인 후보`, `가족 후보`, and other unsupported values.
   No production-root migration or name-based inference was added.
5. Product family UI copy uses `관계`; the two legacy candidate values and the
   `관계 후보` label have zero production findings.
6. The memo input preserves multiline and wrapping behavior while explicitly
   using left/top content alignment.
7. Delete remains disabled and the existing Deactivate contract is unchanged.
8. No source-controlled `엄마` or `아빠` family fixture was found. The only
   matching `딸` and `아들` source terms are approved relation constants, so no
   fixture conversion was required.

Repair exact source/test file list:

- `app/FamilyClaimRef.App/Composition/AppServices.cs`
- `app/FamilyClaimRef.App/Models/Storage/FamilyMemberRelationValues.cs`
- `app/FamilyClaimRef.App/Resources/ProductScreenContent.xaml`
- `app/FamilyClaimRef.App/Resources/UiStrings.xaml`
- `app/FamilyClaimRef.App/ViewModels/ProductShellViewModel.cs`
- `app/FamilyClaimRef.App/Views/ProductFamilyMemberEditorView.xaml`
- `app/FamilyClaimRef.App/Views/ProductFamilyMemberEditorView.xaml.cs`
- `tests/FamilyClaimRef.App.Tests/FamilyMemberManagementViewModelTests.cs`
- `tests/FamilyClaimRef.App.Tests/Integration/FamilyMemberPersistenceIntegrationTests.cs`
- `tests/FamilyClaimRef.App.Tests/JsonFamilyMemberStorageServiceTests.cs`
- `tests/FamilyClaimRef.App.Tests/ProductShellViewModelTests.cs`
- `tests/FamilyClaimRef.App.Tests/ProductWireframeRouteCoverageTests.cs`
- `tests/FamilyClaimRef.App.Tests/Services/Localization/ResourceUiTextProviderTests.cs`

Repair verification:

- build: warning/error `0/0`
- Family focused tests: `29/29`, failed/skipped `0/0`
- related ProductShell/route/localization/composition regression: `115/115`,
  failed/skipped `0/0`
- full test suite: `588/588`, failed/skipped `0/0`
- exact repair files trailing whitespace: `0`
- production old candidate/label findings: `0`
- relation allowlist count: `10`
- `git diff --check`: pass
- staged/commit/push: `0/0/0`

Manual runtime recheck remains required from create mode against a new isolated
synthetic runtime root. Update and deactivate phases remain deferred until the
user passes the preceding phase markers.

### I.1 User-Assisted Recheck and Inactive-Row Contract Repair

The user completed the ordered create, restart/update, and
restart/deactivate observations against the same isolated synthetic runtime
root:

- runtime root: `C:\Users\jin8855\AppData\Local\Temp\FamilyClaimRef\T3_PER_B_Manual_Recheck_20260804_095727_589`
- evidence root: `C:\Users\jin8855\AppData\Local\Temp\FamilyClaimRef\T3_PER_B_Manual_Recheck_Evidence_20260804_095727_589`
- create observation: pass; exact relation options, disabled create-mode
  Delete/Deactivate, top-left memo input, screen 02 return, and one active row
  were observed
- update observation: pass; restart persistence, edit load, update, screen 02
  return, and no duplicate record were observed
- deactivate observation: pass; restart persistence, enabled Deactivate,
  disabled Delete, deactivate write, and removal from the active-only list
  were observed
- existing captures: `01-create-mode-disabled.png`,
  `02-create-active-list.png`, `03-restart-updated-edit-mode.png`, and
  `04-deactivated-active-list.png`

On the final restart, the user observed that the disabled family member was no
longer visible on screen 02. The user selected a revised management contract:
screen 02 must retain the family member and change only its displayed state.
This decision supersedes the earlier screen-02 active-only presentation. The
first repair did not reactivate the member and did not change the stored
schema.

Repair contract:

1. `IFamilyMemberStorageService.GetFamilyMembersAsync` returns active and
   disabled records for management presentation.
2. `GetActiveFamilyMembersAsync` remains unchanged for active-only target
   selection and future consumers that must reject disabled members.
3. `FamilyMemberManagementViewModel` uses the all-record query for initial
   load and post-mutation refresh.
4. Screen 02 renders the stored `DisabledAt` state instead of a static active
   value. A disabled row displays `사용 중지`.
5. Edit and Deactivate are disabled for a disabled row in XAML and guarded
   again in code-behind. Delete remains disabled.
6. Active/disabled/all filters remain deferred follow-up work. Reactivation is
   implemented by the subsequent repair recorded below. Permanent delete
   remains deferred until `FamilyMemberId` reference integrity is implemented.
7. The synthetic runtime and evidence roots were preserved and were not read,
   modified, moved, or deleted during source repair.

Inactive-row repair exact source/test file list:

- `app/FamilyClaimRef.App/Services/Storage/IFamilyMemberStorageService.cs`
- `app/FamilyClaimRef.App/Services/Storage/JsonFamilyMemberStorageService.cs`
- `app/FamilyClaimRef.App/ViewModels/FamilyMemberManagementViewModel.cs`
- `app/FamilyClaimRef.App/Views/ProductFamilyMembersView.xaml`
- `app/FamilyClaimRef.App/Views/ProductFamilyMembersView.xaml.cs`
- `tests/FamilyClaimRef.App.Tests/FamilyMemberManagementViewModelTests.cs`
- `tests/FamilyClaimRef.App.Tests/Integration/FamilyMemberPersistenceIntegrationTests.cs`
- `tests/FamilyClaimRef.App.Tests/JsonFamilyMemberStorageServiceTests.cs`
- `tests/FamilyClaimRef.App.Tests/ProductWireframeRouteCoverageTests.cs`

Inactive-row repair verification:

- initial regression test: expected compile failure because the management
  all-record query did not yet exist
- build: warning/error `0/0`
- Family focused tests: `29/29`, failed/skipped `0/0`
- related ProductShell/route/localization/composition regression: `144/144`,
  failed/skipped `0/0`
- full test suite: `588/588`, failed/skipped `0/0`
- Product runtime during repair: not running
- stage/commit/push: `0/0/0`

The user subsequently confirmed that screen 02 retained the disabled row, but
observed that the Product UI provided no command to return it to active use.
That observation opened the bounded reactivation repair below.

### I.2 Disabled-to-Active Reactivation Repair

User-observed defect:

- the disabled record remained visible on screen 02
- its stored state was represented correctly as `사용 중지`
- no Product command existed to change that same record back to active use

Approved repair contract:

1. A disabled row exposes one `다시 사용` command. An active row continues to
   expose `사용 중지` instead.
2. Reactivation operates on the existing `FamilyMemberId`; it does not create a
   replacement or duplicate record.
3. `JsonFamilyMemberStorageService` executes reactivation under the existing
   store gate, requires an inactive target and exact expected version, clears
   `DisabledAt`, updates `UpdatedAt`, and increments `Version` once.
4. An already-active, missing, or stale target produces the existing safe
   non-write error contract. No automatic retry or duplicate write is added.
5. The management list refreshes after success and displays the same row as
   active. Active-only target queries include the record again.
6. Edit remains disabled while a row is inactive. Delete remains disabled.
7. No schema, migration, DB, policy reference, or production-root change is
   introduced.
8. Active/disabled/all filters remain deferred. Permanent delete remains
   deferred until `FamilyMemberId` reference integrity is implemented.

Reactivation repair exact source/test file list:

- `app/FamilyClaimRef.App/Services/Storage/IFamilyMemberStorageService.cs`
- `app/FamilyClaimRef.App/Services/Storage/JsonFamilyMemberStorageService.cs`
- `app/FamilyClaimRef.App/ViewModels/FamilyMemberManagementViewModel.cs`
- `app/FamilyClaimRef.App/Services/Localization/UiTextKeys.cs`
- `app/FamilyClaimRef.App/Resources/UiStrings.xaml`
- `app/FamilyClaimRef.App/Views/ProductFamilyMembersView.xaml`
- `app/FamilyClaimRef.App/Views/ProductFamilyMembersView.xaml.cs`
- `tests/FamilyClaimRef.App.Tests/FamilyMemberManagementViewModelTests.cs`
- `tests/FamilyClaimRef.App.Tests/Integration/FamilyMemberPersistenceIntegrationTests.cs`
- `tests/FamilyClaimRef.App.Tests/JsonFamilyMemberStorageServiceTests.cs`
- `tests/FamilyClaimRef.App.Tests/ProductWireframeRouteCoverageTests.cs`
- `tests/FamilyClaimRef.App.Tests/Services/Localization/ResourceUiTextProviderTests.cs`
- `tests/FamilyClaimRef.App.Tests/DocumentRegistrationLifecycleGate8Tests.cs`

Reactivation repair verification:

- initial regression test: expected compile failure because the reactivation
  resource constants did not yet exist
- build: warning/error `0/0`
- Family focused tests: `31/31`, failed/skipped `0/0`
- related ProductShell/route/localization/composition regression: `148/148`,
  failed/skipped `0/0`
- full test suite: `592/592`, failed/skipped `0/0`
- resource/constants parity: `116/116`
- `Ui.Product.*` parity: `60/60`
- the existing synthetic runtime and evidence roots were not read, modified,
  moved, or deleted during source repair
- Product PID `33468`, which had no top-level window and locked the build
  output, was closed gracefully before final validation
- Product runtime after final validation: not running
- stage/commit/push: `0/0/0`

The user completed the remaining manual check and returned the exact marker:

```text
REACTIVATE_OBSERVED_RESTART_4: PASS
```

This marker closes the agreed bounded observations:

- the disabled row exposed `다시 사용`
- one reactivation changed the existing row to active
- no duplicate row was created
- the active state persisted after restart against the same synthetic root

No screenshot, PID, or window handle is asserted for this observation because
the user did not provide those values. Build and tests were not rerun after the
user observation; the immediately preceding automated results remain the
authoritative source/test evidence.

The following is a historical automated handoff record, superseded by the
later user-provided reactivation PASS marker. Automated final-launch handoff
could not be completed in the execution-tool environment. Each launch initially
opened a responsive `FamilyClaimRef` Product window, but the top-level window
was withdrawn after the launching tool invocation ended while the process
remained alive. The attempts used Product PIDs `24980`, `41208`, `31984`, and
`37212`; the last attempt was held by a separate helper process and produced
the same result. Every process and helper created by these attempts was closed,
and the unrelated pre-existing PID `33468` was preserved at that historical
point.

- command mode for each attempt: `--product-shell-preview`
- runtime root: same approved synthetic root listed above
- UI click/input: `0/0`
- create/update/deactivate write repetition: `0/0/0`
- evidence-root modification: `0`
- final usable Product window: none
- historical handoff action: superseded and completed by the later
  user-assisted disabled-row and reactivation observations

## J. Independent Review

The fresh-context reviewer initially reported:

- Blocking: `0`
- Major: `1`
- Minor: `3`

The findings covered obsolete persistence guidance, deterministic gate test
strength, actual atomic move failure evidence, and refresh-failure retry
evidence. All findings were repaired in scope and the reviewer rechecked the
changed locations read-only.

Final independent review:

- Blocking: `0`
- Major: `0`
- Minor: `0`
- Recommendation: `PASS`

The independent reviewer did not modify files or run build, test, app, or Git
write operations.

## K. Protected and Deferred Scope

- `REFERENCE_GUARD_STATE = DEFERRED_UNTIL_FIRST_REAL_FAMILY_REFERENCE_CONSUMER`
- `POLICY_PERSISTENCE_EXTENSION_STATE = USER_SELECTED_NOT_IMPLEMENTED`
- `CATEGORY_PERSISTENCE_STATE = USER_SELECTED_NOT_IMPLEMENTED`
- `DB_MIGRATION_STATE = NOT_AUTHORIZED`
- Family reactivation: implemented; active/disabled/all filters remain deferred
- Family hard delete: not implemented
- Policy-Family reference field/checker: not implemented
- screen 12, 16, 19, and 20 write enablement: unchanged
- screen 17/18 storage contract: unchanged
- actual user data access: `0`
- DB/API execution: `0`
- P03/R03/R07/R08 execution: `0`
- production readiness: `NOT_EVALUATED`
- deployment: `NOT_AUTHORIZED`

## L. Final Judgment

Source repair, automated validation, static checks, the prior independent
review, and the ordered user-assisted runtime rechecks are complete. The
T3-PER-B FamilyMember JSON persistence evidence package passes.

- Final branch: `main`
- Final HEAD: `aecf7edfd43b4124ec5ff17d35687020cf4c0d90`
- Final tracked/staged/untracked: `46/0/58`
- Final status entries: `104`
- T3 additions over the approved baseline: fourteen source/test paths and this
  one evidence document
- Historical user-observed defect Blocking/Major/Minor: `0/2/1`
- Remaining open defect Blocking/Major/Minor: `0/0/0`
- Repair automated validation: pass
- Final full test suite: `592/592`, failed/skipped `0/0`
- Manual runtime recheck: create/update/deactivate and disabled-row visibility
  pass; reactivation and restart persistence pass
- Final observed execution state: PID `35692` remains without a top-level
  window; it was not terminated in the final-state reconciliation task
- Synthetic/evidence root cleanup: not executed
- The windowless PID residue does not revoke the Product behavior PASS

```text
FAMILYCLAIMREF_T3_PER_B_FAMILY_PERSISTENCE_IMPLEMENTED_EVIDENCE_PASS
```

Minimum next action: preserve the synthetic runtime and evidence roots without
cleanup and obtain a separate user decision for the next persistence slice or
for an exact documentation commit. Do not automatically begin T3-PER-A, add
active/disabled/all filters, enable hard delete, or change production readiness.
