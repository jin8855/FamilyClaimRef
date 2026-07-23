# Product UI Shell Phase 1D2 Guarded Runtime Entry Manual Smoke Result Review

## Status Marker

`POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1D2_GUARDED_RUNTIME_ENTRY_MANUAL_SMOKE_PARTIAL`

## A. Baseline

- Commit hash: `8491216e295d9ae9c804cce86f427832e26c4b41`
- Subject: `feat(familyclaimref): add guarded product shell preview entry`
- Initial working tree: clean
- Initial staged files: none
- Existing `FamilyClaimRef.App` processes: `0`
- Result classification: PARTIAL
- Blocking evidence gap: preview Home content was visible, but UI Automation did not report the Home navigation item as selected.

## B. Executable Evidence

- Repository-relative executable: `app/FamilyClaimRef.App/bin/Debug/net10.0-windows/FamilyClaimRef.App.exe`
- Target framework: `net10.0-windows`
- Output type: `WinExe`
- Exact executable candidates: `1`
- Companion application DLL: present
- Executable SHA-256: `3D806F92A63F7A784CFD30A35B9139435587B45D7917F6C7844E3264D26B9FB3`
- Executable last-write UTC: `2026-07-16T06:39:56.7670939Z`
- Build/test rerun: no
- Prior full validation carried forward: PASS `393/393`

## C. Isolation Contract

- Environment injection method: child `ProcessStartInfo.Environment` only
- Guard variable: `FAMILYCLAIMREF_ENABLE_DEV_RUNTIME_ROOT_OVERRIDE=1`
- Runtime-root variable: `FAMILYCLAIMREF_RUNTIME_ROOT`
- Default root: `<TEMP>\FamilyClaimRef-GuardedSmoke\<run-id>-default`
- Preview root: `<TEMP>\FamilyClaimRef-GuardedSmoke\<run-id>-preview`
- Default root before launch: absent
- Preview root before launch: absent
- User environment variables after run: unset
- Machine environment variables after run: unset
- Persistent environment mutation API: not used
- Project root used as runtime root: no

## D. Default Smoke Result

- Arguments: none
- PID: `3352`
- Process start: PASS
- Visible top-level Windows: `1`
- Observed title: `FamilyClaimRef`
- Source-confirmed validation warning marker: present
- Product navigation markers: absent
- Identified Window: `MainWindow`
- Unexpected dialogs: `0`
- UI interaction or navigation: none
- Workflow or file picker: not invoked
- `CloseMainWindow()` accepted: yes
- Process exit within 10 seconds: yes
- Forced termination: no
- Isolated root after run: absent
- Result: PASS

## E. Preview Smoke Result

- Argument: `--product-shell-preview`
- PID: `30876`
- Process start: PASS
- Visible top-level Windows: `1`
- Observed title: `FamilyClaimRef`
- Source-confirmed `문서 등록` marker: present
- Source-confirmed `문서 목록` marker: present
- Source-confirmed Home content marker: visible
- MainWindow validation warning marker: absent
- Identified Window surface: `ProductShellWindow`
- Home navigation selection reported by UI Automation: no
- Unexpected dialogs: `0`
- UI interaction or navigation: none
- Workflow or file picker: not invoked
- `CloseMainWindow()` accepted: yes
- Process exit within 10 seconds: yes
- Forced termination: no
- Isolated root after run: absent
- Result: PARTIAL

The preview Window and Home content were observed. The required read-only selection-state evidence was not collected, so this review does not infer that Home was selected.

## F. One-Window Judgment

- Default visible Window count: `1`
- Preview visible Window count: `1`
- App processes between scenarios: `0`
- App processes after both scenarios: `0`
- Simultaneous app processes during each scenario: `1`
- Dual-window evidence: none
- ShowDialog-like modal Window evidence: none
- Normal close evidence: collected for both scenarios
- Forced termination: none

## G. Runtime Artifact Result

- Default isolated root after launch: absent
- Preview isolated root after launch: absent
- Project root `attachments/` files: `0`
- Project root `data/local/` files: `0`
- Project root `runtime_test_document.*`: `0`
- Unexpected project-root DB/SQLite files: `0`
- Running `FamilyClaimRef.App` processes after run: `0`
- User/Machine persistent override variables: unset
- Protected-path internal access: none
- Runtime-root cleanup: not run

## H. Evidence Boundary

- Actual default Window startup: observed
- Actual preview Window startup: observed
- Actual normal close and process exit: observed for both scenarios
- One visible top-level Window per process: observed
- Preview Home content: observed
- Preview Home selection state: not confirmed
- Navigation beyond Home: not tested
- Registration/list workflow: not tested
- Policy/claim management behavior: not tested
- Primary ProductShell readiness: not claimed
- Default startup remains: `MainWindow`
- Preview remains explicit opt-in: yes

## I. Static And Safety Validation

- Source/test/resource/project changes from this batch: none
- App launches attempted: `2`
- Additional app launch after partial result: none
- Build/test rerun: no
- Screenshots: none
- Mouse/keyboard input: none
- Workflow/file-picker execution: none
- Cleanup: none
- App processes after run: `0`
- Initial Git status: clean
- Expected final changed scope: `docs/398` only
- Staged files: none

## I-1. User Review Decision

- User review decision: accept this PARTIAL smoke evidence without another app launch.
- Accepted runtime evidence: default and preview startup identity, one visible top-level Window per process, no unexpected dialog, normal close, normal process exit, isolated runtime-root safety, and no project-root artifact.
- Accepted preview evidence: ProductShellWindow surface and Home content were visible.
- Remaining evidence gap: read-only UI Automation did not confirm the Home navigation item's selected state.
- The remaining selection-state gap is not reclassified as PASS and is not inferred from visible content.
- The remaining gap does not invalidate the guarded-entry Window-selection, one-Window, close, process-exit, or isolation evidence.
- Any future selection-state verification requires a separate accessibility/UI Automation evidence decision.
- Additional app launch under this review: not approved and not run.
- Overall smoke classification remains: PARTIAL.

## I-2. Final Git Status

```text
?? docs/398_POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1D2_GUARDED_RUNTIME_ENTRY_MANUAL_SMOKE_RESULT_REVIEW.md
```

- Tracked modified files: 0.
- Untracked files: 1.
- Staged files: 0.
- Deleted files: 0.
- Renamed files: 0.
- Additional changed or untracked files: 0.
- HEAD remains: `8491216e295d9ae9c804cce86f427832e26c4b41`.

## J. Commit Candidate

Exact candidate if the partial evidence is accepted:

- `docs/398_POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1D2_GUARDED_RUNTIME_ENTRY_MANUAL_SMOKE_RESULT_REVIEW.md`

Recommended commit message:

`docs(familyclaimref): record guarded entry manual smoke`

Commit readiness: ready for an exact documentation commit of this accepted PARTIAL evidence.

This batch did not stage or commit the document.

## K. Next Boundary

- Exact docs/398 commit requires a separate instruction.
- Do not launch the app again under this evidence track.
- Do not infer the Home navigation selection state.
- The Home selection-state gap is deferred to a separate accessibility/UI Automation decision only if that evidence becomes required.
- Do not change the default startup window.
- Do not execute navigation, registration, file picker, or workflow actions.
- Primary ProductShell readiness is not claimed.
