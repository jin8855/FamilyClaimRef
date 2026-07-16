# Product UI Shell Phase 1D2 Guarded Runtime Entry Implementation Result Review

## Status Marker

`POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1D2_GUARDED_RUNTIME_ENTRY_IMPLEMENTATION_COMPLETED`

## A. Baseline

- Baseline hash: `50036e5fa8df33995a43e1248fb44d5b2d585a2f`
- Baseline subject: `docs(familyclaimref): plan product shell phase1d2 guarded entry`
- Initial working tree: clean
- Initial staged files: none
- Full test baseline: PASS `382/382`
- Default startup before implementation: `MainWindow`
- Product shell guarded entry before implementation: absent

## B. Exact Changed File List

Created production:

- `app/FamilyClaimRef.App/Startup/StartupWindowModeSelector.cs`

Modified production:

- `app/FamilyClaimRef.App/App.xaml.cs`

Created test:

- `tests/FamilyClaimRef.App.Tests/StartupWindowModeSelectorTests.cs`

Created result document:

- `docs/397_POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1D2_GUARDED_RUNTIME_ENTRY_IMPLEMENTATION_RESULT_REVIEW.md`

Total changed files: `4`

## C. Selector Result

- Public enum: `StartupWindowMode`
- Enum members: `MainWindow`, `ProductShellPreview`
- Public pure selector: `StartupWindowModeSelector`
- Public exact token constant: `ProductShellPreviewArgument`
- Exact token value: `--product-shell-preview`
- Comparison: `StringComparison.OrdinalIgnoreCase`
- `null`, empty, and unknown arguments: `MainWindow`
- Exact preview token: `ProductShellPreview`
- Duplicate exact tokens: `ProductShellPreview`
- Prefix, suffix, and value-assignment forms: rejected as non-exact
- Repeated calls: deterministic and stateless
- Mutable state: absent
- Environment, `AppContext`, process-global, WPF, and service dependencies: absent

## D. Startup Wiring Result

- Existing `OnStartup(StartupEventArgs e)` signature: preserved
- Existing `base.OnStartup(e)` call: preserved
- Startup arguments: `e.Args` passed to the selector
- `AppServices.CreateDefault()` calls per startup: `1`
- Default branch: `MainWindow` with the existing `MainWindowViewModel`
- Preview branch: `ProductShellWindow` with `ProductShellViewModel`
- Selected `Window` constructions per startup path: `1`
- `Application.MainWindow` assignments: `1`
- `Show()` calls: `1`
- `ShowDialog()` calls: `0`
- Dual-window construction path: absent
- Startup catch or silent fallback: absent
- Default `MainWindow` behavior: preserved
- `AppServices`, `MainWindow`, and `ProductShellWindow` source files: unchanged

## E. Test And Build Result

Normal execution first encountered the Windows SDK user-profile access boundary:

- Error type: `MSB4184`
- Boundary: access denied for the local Microsoft SDKs profile path
- Classification: environment permission boundary, not a code or test failure

The same commands were then rerun with approved elevated execution:

- `dotnet build FamilyClaimRef.sln`: PASS, warnings `0`, errors `0`
- `StartupWindowModeSelectorTests`: PASS `11/11`
- `AppServicesTests`: PASS `6/6`
- `ProductShellViewModelTests`: PASS `13/13`
- `ProductDocumentListViewModelTests`: PASS `18/18`
- `DocumentRegistrationViewModelTests`: PASS `26/26`
- Full solution tests: PASS `393/393`
- Full test baseline comparison: `382 -> 393`
- Added discovered cases: `11`
- Existing tests deleted: `0`
- Existing assertions weakened: `0`

## F. Runtime Evidence Boundary

- App launch: not run
- Actual default-window runtime evidence: not collected
- Actual preview-window runtime evidence: not collected
- One-window and process-exit evidence: not collected
- Automated selector evidence: complete
- App startup wiring evidence: static inspection and build only
- Manual smoke: separate explicit approval required

This result does not claim that either startup path was manually observed at runtime.

## G. Default Readiness Boundary

- Default startup remains `MainWindow`.
- Product shell preview requires the explicit `--product-shell-preview` token.
- Product shell default startup ready: no
- Remaining functional blockers: policy contract management, claim case management, and fresh-root target creation
- This batch does not authorize changing the default startup window.

## H. Static And Safety Result

- Exact four-file scope: PASS
- Non-target diff: `0`
- Public enum members: `2`
- Selector definitions: `1`
- Exact token constant definitions: `1`
- Selector calls in `App.xaml.cs`: `1`
- `AppServices.CreateDefault()` calls in startup: `1`
- `new MainWindow` branches: `1`
- `new ProductShellWindow` branches: `1`
- `Application.MainWindow` assignments: `1`
- `Show()` calls: `1`
- `ShowDialog()` calls: `0`
- Startup catch blocks: `0`
- `StartupUri`: absent
- Environment, `AppContext`, process-global, and persistence selector reads: `0`
- Resources/constants: `68/68`
- `Ui.Product.*` resources/constants: `12/12`
- Resource/source diff: `0`
- Production Korean literal scan: PASS, findings `0`
- Personal, sample, and local-profile scan: PASS, findings `0`
- `git diff --check`: PASS; line-ending conversion warning only
- Trailing whitespace: PASS
- EOF terminal newline: exactly one per target
- EOF extra blank line: `0`
- `data/claimdoc/` ignore rule: confirmed without internal access
- `docs/nightwork_20260706/` ignore rule: confirmed without internal access
- Project root `attachments/` files: `0`
- Project root `data/local/` files: `0`
- Project root `runtime_test_document.*`: `0`
- Unexpected project-root DB/SQLite files: `0`
- Staged files: none
- Actual final Git status is recorded in section H-1.

## H-1. Final Git Status

```text
 M app/FamilyClaimRef.App/App.xaml.cs
?? app/FamilyClaimRef.App/Startup/StartupWindowModeSelector.cs
?? docs/397_POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1D2_GUARDED_RUNTIME_ENTRY_IMPLEMENTATION_RESULT_REVIEW.md
?? tests/FamilyClaimRef.App.Tests/StartupWindowModeSelectorTests.cs
```

- Tracked modified files: 1.
- Untracked files: 3.
- Staged files: 0.
- Deleted files: 0.
- Renamed files: 0.
- Additional changed or untracked files: 0.
- HEAD remains: `50036e5fa8df33995a43e1248fb44d5b2d585a2f`.

## I. Explicit Non-Scope

- `App.xaml` modification: none
- `AppServices` modification: none
- `MainWindow` modification: none
- `ProductShellWindow` modification: none
- ViewModel, resource, project, solution, and package modification: none
- Default product shell replacement: none
- Launcher or dual-window mode: none
- App launch or manual smoke: none
- Workflow or file picker execution: none
- DB, SQLite, repository, OCR, and migration work: none
- Cleanup or protected-path internal access: none

## J. Commit Candidate

Commit candidate exact file list:

- `app/FamilyClaimRef.App/Startup/StartupWindowModeSelector.cs`
- `app/FamilyClaimRef.App/App.xaml.cs`
- `tests/FamilyClaimRef.App.Tests/StartupWindowModeSelectorTests.cs`
- `docs/397_POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1D2_GUARDED_RUNTIME_ENTRY_IMPLEMENTATION_RESULT_REVIEW.md`

Recommended commit message:

`feat(familyclaimref): add guarded product shell preview entry`

This batch did not stage or commit any file.

## K. Next Boundary

- Exact implementation commit requires a separate instruction.
- Manual smoke requires separate approval after the exact commit.
- Do not change the default startup window.
- Do not modify `MainWindow`, `ProductShellWindow`, or `AppServices` in this batch.
- Do not claim primary product shell readiness from static and automated evidence alone.
- Stop after this implementation result review and final static verification.
