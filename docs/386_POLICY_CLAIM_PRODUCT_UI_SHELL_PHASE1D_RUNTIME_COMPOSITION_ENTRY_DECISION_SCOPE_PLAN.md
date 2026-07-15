# Product UI Shell Phase 1D Runtime Composition And Entry Decision Scope Plan

## A. Status

- Task ID: `POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1D_RUNTIME_COMPOSITION_AND_ENTRY_STRATEGY_EXACT_SCOPE_DECISION_DOCS_BATCH`
- Marker: `POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1D_RUNTIME_COMPOSITION_ENTRY_DECISION_SCOPE_READY`
- Work type: documentation-only exact-scope decision
- Implementation target now: 0

## B. Baseline

- Hash: `d57a345f8bd7c46b53de185ea91cf8f164137e43`
- Subject: `feat(familyclaimref): add compile-only product document list`
- Initial working tree: clean
- Initial staged files: none
- Current full test evidence: PASS `379/379`
- Resources/constants: `68/68`
- `Ui.Product.*` resources/constants: `12/12`

## C. Current Boundary

- `ProductShellWindow` is implemented but compile-only.
- Home, DocumentRegistration, and DocumentList content mappings are implemented.
- ProductShell runtime caller is absent.
- AppServices ProductShell composition is absent.
- `MainWindow` remains the validation harness and current startup Window.
- ProductShell composition and runtime entry are separate decisions.
- This batch does not authorize either implementation.

## D. Decision Scope

This batch records:

1. Current `App.OnStartup`, `AppServices`, and MainWindow ownership.
2. ProductShell constructor graph and reusable services.
3. Application-resource and no-Application fallback coverage.
4. MainWindow/ProductShell ViewModel lifetime isolation.
5. Composition-only and runtime-entry strategy alternatives.
6. Default-startup readiness and functional blockers.
7. A future exact composition-only implementation candidate.
8. Future validation gates without executing them.

## E. Exact Documentation Scope

- `docs/386_POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1D_RUNTIME_COMPOSITION_ENTRY_DECISION_SCOPE_PLAN.md`
- `docs/387_POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1D_RUNTIME_COMPOSITION_SOURCE_LIFETIME_RECONCILIATION.md`
- `docs/388_POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1D_RUNTIME_ENTRY_STRATEGY_AND_EXACT_FILE_LIST_DECISION_CANDIDATE.md`
- `docs/389_POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1D_RUNTIME_COMPOSITION_VALIDATION_TEST_GATE_PLAN.md`
- `docs/390_POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1D_RUNTIME_COMPOSITION_COMMIT_CANDIDATE_REVIEW.md`

`docs/391_POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1D_RUNTIME_COMPOSITION_IMPLEMENTATION_RESULT_REVIEW.md` created: no.

## F. Approval Matrix

| Item | Approved now |
|---|---|
| AppServices ProductShell composition | no |
| ProductShell ViewModel factory/property | no |
| ProductShell Window factory | no |
| Fallback resource additions | no |
| App startup change | no |
| MainWindow replacement | no |
| Guarded startup mode | no |
| Runtime entry | no |
| ProductShell launch | no |
| Future exact implementation file list | no, candidate only |
| `docs/391` creation | no |

## G. Explicit Non-Scope

- Source, test, XAML, ViewModel, resource, project, and solution changes: none.
- AppServices, App startup, MainWindow, and ProductShell changes: none.
- Window creation, `Show`, `ShowDialog`, and launch: none.
- DB, SQLite, repository, OCR, migration, and cleanup: none.
- Build and tests: not run.
- App launch and manual workflow: not run.
- Git add, stage, commit, and push: not run.

## H. Scope Result

The source supports a composition-only planning boundary. Runtime entry remains deferred and must not be inferred from composition feasibility.
