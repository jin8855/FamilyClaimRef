# Product UI Shell Phase 1D Runtime Composition Implementation Result Review

## A. Status Marker

POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1D_RUNTIME_COMPOSITION_IMPLEMENTATION_COMPLETED

## B. Baseline

- Hash: `cb53c14922af1d796031b03b98db48a9d248dfca`
- Subject: `docs(familyclaimref): plan product shell phase1d runtime composition`
- Initial working tree: clean
- Initial staged files: none
- ProductShellViewModel production composition: absent
- ProductShellWindow runtime construction: absent
- ProductShell runtime entry: absent
- Fallback dictionary entries: 24
- ProductShell/List fallback gaps: 7
- Registration runtime fallback coverage: `14/14`
- Resources/constants: `68/68`
- `Ui.Product.*` resources/constants: `12/12`
- Full solution test baseline: PASS `379/379`

## C. Exact Changed File List

Modified production, 1:

- `app/FamilyClaimRef.App/Composition/AppServices.cs`

Modified tests, 1:

- `tests/FamilyClaimRef.App.Tests/Composition/AppServicesTests.cs`

Created result document, 1:

- `docs/391_POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1D_RUNTIME_COMPOSITION_IMPLEMENTATION_RESULT_REVIEW.md`

Total changed files: 3.

## D. Composition Result

- Read-only `AppServices.ProductShellViewModel` property: implemented.
- `ProductShellViewModel` construction inside `AppServices.Create`: implemented.
- ProductShell `DocumentRegistrationViewModel`: created separately from the MainWindow instance.
- `ProductDocumentListViewModel`: composed with the existing `IDocumentStorageService` and shared `IUiTextProvider`.
- ProductShell graph constructor order: `IUiTextProvider`, ProductShell registration ViewModel, document-list ViewModel.
- MainWindow and ProductShell mutable child ViewModels: not shared.
- Separate `AppServices.Create` calls: separate ViewModel graphs.
- Existing document storage, policy/claim storage, attachment, workflow, picker, and text-provider instances: reused inside one AppServices graph.
- Exact private infrastructure-service identity: not exposed or asserted through the public surface.
- Infrastructure sharing relationship: confirmed by source wiring review.
- Reflection-only production API added for testing: none.
- Test-only public service exposure added: none.
- New DI container, service locator, bootstrapper, factory interface, or runtime-mode service: none.

## E. Fallback Resource Result

- Fallback dictionary entries: `24 -> 31`.
- Added ProductShell/List mirrors: 7.
- `Ui.Product.Shell.Title`: `FamilyClaimRef`.
- `Ui.Product.Navigation.Home`: `홈`.
- `Ui.Product.Navigation.DocumentRegistration`: `문서 등록`.
- `Ui.Product.Navigation.DocumentList`: `문서 목록`.
- `Ui.Product.DocumentList.Title`: `문서 목록`.
- `Ui.Product.DocumentList.EmptyMessage`: `등록된 문서가 없습니다.`.
- `Ui.Product.DocumentList.LoadFailedMessage`: `문서 목록을 불러오지 못했습니다.`.
- Existing registration fallback coverage: retained `14/14`.
- `UiStrings.xaml` and `UiTextKeys.cs`: unchanged.
- Resource source of truth: unchanged; additions are no-Application mirrors only.

## F. Focused Test Result

- AppServicesTests pre-implementation discovered count: 3.
- Added discovered cases: 3.
- AppServicesTests final: PASS `6/6`.

- MainWindow and ProductShell graph exposure and registration-child separation.
- Separate ViewModel graphs across separate AppServices creation calls.
- Exact ProductShell/List fallback copy resolution without Application resources.
- Existing project-root safety test now also verifies that graph construction does not create the supplied runtime root.
- Existing tests deleted: 0.
- Existing assertions weakened: 0.

## G. Build And Test Result

- Initial normal build: failed at the Windows SDK user-profile access boundary.
- Elevated build: PASS, warnings 0, errors 0.
- `AppServicesTests`: PASS `6/6`.
- `ProductShellViewModelTests`: PASS `13/13`.
- `ProductDocumentListViewModelTests`: PASS `18/18`.
- `DocumentRegistrationViewModelTests`: PASS `26/26`.
- Full solution tests: PASS `382/382`.
- Baseline comparison: `379 -> 382`.
- Added discovered test cases: 3.

## H. Runtime And Startup Boundary

- `App.xaml` and `App.xaml.cs`: unchanged.
- `App.OnStartup`: unchanged; MainWindow remains the startup Window.
- MainWindow files and MainWindowViewModel: unchanged.
- ProductShellWindow files: unchanged.
- `new ProductShellWindow`: absent.
- ProductShellWindow `Show` or `ShowDialog`: absent.
- ProductShell runtime entry: absent.
- Environment-variable or command-line startup switch: absent.
- Launcher and dual-Window launch: absent.
- App launch, OpenFileDialog, manual workflow, screenshot, and visual automation: not run.

## I. Static And Safety Result

- Exact changed scope: 3 files.
- Non-target diff: 0.
- AppServices minimal diff: PASS.
- AppServicesTests scope diff: PASS.
- Fallback entry count: PASS, 31.
- Exact fallback additions: PASS, 7.
- Existing fallback changes: 0.
- ProductShell/List fallback coverage: PASS `7/7`.
- Registration fallback coverage: PASS `14/14`.
- `[[key]]` placeholder findings in tested ProductShell/List copy: 0.
- Resources/constants: `68/68` unchanged.
- `Ui.Product.*` resources/constants: `12/12` unchanged.
- `UiStrings.xaml` and `UiTextKeys.cs` diff: 0.
- Runtime/startup negative scan: PASS.
- `git diff --check`: PASS.
- Exact-file trailing whitespace scan: PASS, findings 0.
- Exact-file EOF gate: PASS, all three files have one terminal newline and no extra terminal blank line.
- Korean fallback literal boundary: PASS, six approved Korean mirror values and one `FamilyClaimRef` mirror.
- Other production Korean additions or changes: 0.
- Fallback entry count and exact values: PASS.
- ProductShellWindow construction and runtime-entry scan: PASS, findings 0.
- Protected ignore checks: PASS for `data/claimdoc/` and `docs/nightwork_20260706/` without internal content access.
- Project root `attachments/` files: 0.
- Project root `data/local/` files: 0.
- Project root `runtime_test_document.*` files: 0.
- Unexpected root DB/SQLite files: 0.
- Actual personal/sample and local-profile path scan: PASS, findings 0.
- Staged files: none.

## I-1. Final Git Status

```text
 M app/FamilyClaimRef.App/Composition/AppServices.cs
 M tests/FamilyClaimRef.App.Tests/Composition/AppServicesTests.cs
?? docs/391_POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1D_RUNTIME_COMPOSITION_IMPLEMENTATION_RESULT_REVIEW.md
```

- Tracked modified files: 2.
- Untracked files: 1, exact `docs/391` only.
- Staged files: 0.
- Deleted files: 0.
- Renamed files: 0.
- Additional changed or untracked files: 0.
- HEAD remains: `cb53c14922af1d796031b03b98db48a9d248dfca`.

## J. Explicit Non-Scope

- App, MainWindow, ProductShellWindow, existing ViewModel, resource, project, solution, and package modifications: none.
- ProductShellWindow factory, construction, launch, or runtime entry: none.
- Storage source-of-truth change: none.
- DB, SQLite, repository, OCR, or migration implementation: none.
- App launch, manual workflow, protected-path access, or cleanup: none.
- Git staging or commit: none.

## K. Commit Candidate

Exact three-file candidate:

- `app/FamilyClaimRef.App/Composition/AppServices.cs`
- `tests/FamilyClaimRef.App.Tests/Composition/AppServicesTests.cs`
- `docs/391_POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1D_RUNTIME_COMPOSITION_IMPLEMENTATION_RESULT_REVIEW.md`

Recommended commit message:

`feat(familyclaimref): compose product shell view model graph`

This batch did not stage or commit files.

## L. Next Boundary

- Exact-file commit requires a separate instruction.
- ProductShell runtime entry remains unapproved.
- ProductShell default startup remains not ready because policy/claim management is not available in ProductShell.
- Do not modify App startup, MainWindow, ProductShellWindow, or existing ViewModels.
- Stop after this implementation result review and wait for user review.
