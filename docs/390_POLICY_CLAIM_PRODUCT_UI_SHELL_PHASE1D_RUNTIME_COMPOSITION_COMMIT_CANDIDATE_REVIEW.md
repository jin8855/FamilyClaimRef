# Product UI Shell Phase 1D Runtime Composition Decision Docs Commit Candidate Review

## A. Status

- Marker: `POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1D_RUNTIME_COMPOSITION_COMMIT_CANDIDATE_REVIEW_READY`
- Commit readiness: ready after exact-scope validation
- Implementation must not start.

## B. Baseline

- Hash: `d57a345f8bd7c46b53de185ea91cf8f164137e43`
- Subject: `feat(familyclaimref): add compile-only product document list`
- Initial working tree: clean.
- Initial staged files: none.
- Current full test evidence: PASS `379/379`.
- Resources/constants: `68/68`.
- `Ui.Product.*` resources/constants: `12/12`.

## C. Exact Documentation Candidate

- `docs/386_POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1D_RUNTIME_COMPOSITION_ENTRY_DECISION_SCOPE_PLAN.md`
- `docs/387_POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1D_RUNTIME_COMPOSITION_SOURCE_LIFETIME_RECONCILIATION.md`
- `docs/388_POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1D_RUNTIME_ENTRY_STRATEGY_AND_EXACT_FILE_LIST_DECISION_CANDIDATE.md`
- `docs/389_POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1D_RUNTIME_COMPOSITION_VALIDATION_TEST_GATE_PLAN.md`
- `docs/390_POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1D_RUNTIME_COMPOSITION_COMMIT_CANDIDATE_REVIEW.md`

Expected candidate count: 5 documentation files.

## D. Decision Summary

- Selected composition strategy: Candidate A, AppServices composition-only.
- Selected factory boundary: ViewModel-only AppServices instance graph; App remains the Window owner.
- Lifetime: separate MainWindow/ProductShell child ViewModels, shared infrastructure services inside one AppServices creation call.
- Fallback audit: 7 ProductShell/List ViewModel keys missing from the 24-entry no-Application dictionary; registration runtime keys covered `14/14`.
- Default ProductShell startup ready: no.
- ProductShell runtime entry: deferred and unapproved.
- MainWindow validation harness and policy/claim management path: retained.

## E. Future Exact Composition Candidate

- Production modify: `app/FamilyClaimRef.App/Composition/AppServices.cs`.
- Test modify: `tests/FamilyClaimRef.App.Tests/Composition/AppServicesTests.cs`.
- Result document create: `docs/391_POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1D_RUNTIME_COMPOSITION_IMPLEMENTATION_RESULT_REVIEW.md`.
- Production create: 0.
- Test create: 0.
- Total future candidate files: 3.
- Implementation target now: 0.

App.xaml, App.xaml.cs, MainWindow files, ProductShellWindow files, resources, ViewModels, project, and solution files are not composition-only candidates.

## F. Blocker Counts

- Composition source blockers: 0.
- Fallback-copy gaps: 7.
- Lifetime blockers after decision: 0.
- Testability limitations: 1, non-blocking private service-identity visibility.
- Default-startup functional blockers: 3.
- Runtime-entry decision blockers: 2.

## G. Current Approval State

- AppServices composition approved now: no.
- Fallback resource mirror additions approved now: no.
- App startup change approved now: no.
- MainWindow replacement approved now: no.
- Guarded startup approved now: no.
- Runtime entry approved now: no.
- ProductShell launch approved now: no.
- Future exact implementation list approved now: no, candidate only.
- `docs/391` created: no.

## H. Batch Validation Record

- Source baseline: PASS.
- Composition and lifetime audit: PASS.
- Fallback resource audit: PASS, exact gap 7.
- Strategy consistency: PASS.
- Source/test/XAML/ViewModel/resource/project changes: none.
- Tracked modified files: 0.
- Untracked files: exact docs/386 through docs/390 only.
- Staged files: none.
- Build/test/app launch: not run, documentation-only Phase 1D decision batch.

## I. Commit Candidate

Recommended commit message:

`docs(familyclaimref): plan product shell phase1d runtime composition`

This batch does not stage or commit the documentation candidate.

## J. Next Boundary

- Stop after decision documents.
- Do not modify AppServices.
- Do not modify App startup.
- Do not add runtime entry.
- Wait for document review and an exact documentation commit instruction.
