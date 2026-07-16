# Product UI Shell Phase 1D2 Guarded Entry Commit Candidate Review

## A. Status

- Marker: `POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1D2_GUARDED_ENTRY_COMMIT_CANDIDATE_REVIEW_READY`
- Commit readiness: ready after exact documentation validation
- Implementation readiness: not approved

## B. Baseline

- Hash: `ced4a00f16a55bbe1e76e0b016922983bf1aefd5`
- Subject: `feat(familyclaimref): compose product shell view model graph`
- Initial working tree: clean
- Initial staged files: none
- Full solution baseline: PASS `382/382`

## C. Exact Documentation Candidate

1. `docs/392_POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1D2_GUARDED_RUNTIME_ENTRY_DECISION_SCOPE_PLAN.md`
2. `docs/393_POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1D2_STARTUP_ARGUMENT_AND_WINDOW_OWNERSHIP_RECONCILIATION.md`
3. `docs/394_POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1D2_GUARDED_ENTRY_STRATEGY_AND_EXACT_FILE_LIST_DECISION_CANDIDATE.md`
4. `docs/395_POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1D2_GUARDED_ENTRY_VALIDATION_TEST_GATE_PLAN.md`
5. `docs/396_POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1D2_GUARDED_ENTRY_COMMIT_CANDIDATE_REVIEW.md`

Expected documentation candidate count: `5`.

Reserved `docs/397_POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1D2_GUARDED_RUNTIME_ENTRY_IMPLEMENTATION_RESULT_REVIEW.md`: absent.

## D. Decision Summary

- Source audit result: command-line selection through existing `StartupEventArgs.Args` is source-supported.
- Selected strategy: Candidate B, pure startup-mode selector plus `App.xaml.cs` wiring.
- Exact preview token: `--product-shell-preview`.
- Comparison: `StringComparison.OrdinalIgnoreCase` over a complete argument token.
- Selector visibility: public stateless selector and public mode enum; no new `InternalsVisibleTo`.
- Default Window: MainWindow.
- Preview Window: ProductShellWindow.
- Future AppServices call count: exactly one.
- Simultaneous Window count: one.
- Silent MainWindow fallback: none.
- Manual launch: separate future approval.
- Default ProductShell startup ready: no.
- Guarded preview implementation target now: `0`.

## E. Future Exact Implementation Candidate

| Path | Change | Approved now |
|---|---|---|
| `app/FamilyClaimRef.App/Startup/StartupWindowModeSelector.cs` | create | no |
| `app/FamilyClaimRef.App/App.xaml.cs` | modify | no |
| `tests/FamilyClaimRef.App.Tests/StartupWindowModeSelectorTests.cs` | create | no |
| `docs/397_POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1D2_GUARDED_RUNTIME_ENTRY_IMPLEMENTATION_RESULT_REVIEW.md` | create | no |

- Production create: `1`
- Production modify: `1`
- Test create: `1`
- Test modify: `0`
- Result document create: `1`
- Total future candidate files: `4`

## F. Blockers

- Source blockers: `0` for Candidate B.
- Selector testability blockers: `0` with the public selector candidate.
- App lifecycle testability blockers: `1`; no approved no-Window wiring test seam exists.
- Launch-safety blockers: `1`; actual one-Window/default/preview/process-exit evidence is absent.
- Default-startup functional blockers: `3`.

## G. Approval State

- AppServices modification approved now: no.
- App startup modification approved now: no.
- Guarded runtime entry approved now: no.
- ProductShellWindow construction approved now: no.
- Startup selector creation approved now: no.
- Manual launch approved now: no.
- Default startup replacement approved now: no.
- docs/397 creation approved now: no.

Implementation must not start from this review.

## H. Validation Record

- Baseline HEAD/subject: PASS.
- Initial clean/staged state: PASS.
- docs/392~397 pre-existence gate: PASS, all missing before creation.
- Startup/source audit: PASS.
- Runtime-root side-effect audit: PASS for conditional future isolated-smoke feasibility.
- Strategy/source consistency: PASS.
- Candidate/visibility consistency: PASS.
- Resources/constants: `68/68`.
- `Ui.Product.*`: `12/12`.
- Source/test/XAML/ViewModel/resource/project changes: none.
- Build/test/app launch: not run, documentation-only decision batch.
- Git add/stage/commit/push: not run.
- Actual final Git status at validation: exactly docs/392~396 untracked, tracked modifications `0`, staged files `0`.

## I. Commit Candidate

Recommended commit message:

`docs(familyclaimref): plan product shell phase1d2 guarded entry`

This batch does not stage or commit the documentation candidate.

## J. Next Boundary

- Stop after decision documents.
- Do not modify `App.xaml.cs`.
- Do not create the startup selector or tests.
- Do not construct or launch ProductShellWindow.
- Wait for document review and an exact documentation commit instruction.
