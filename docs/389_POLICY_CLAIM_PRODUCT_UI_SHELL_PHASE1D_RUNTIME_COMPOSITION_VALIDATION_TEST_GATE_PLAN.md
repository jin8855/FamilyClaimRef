# Product UI Shell Phase 1D Runtime Composition Validation And Test Gate Plan

## A. Status

- Marker: `POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1D_RUNTIME_COMPOSITION_VALIDATION_TEST_GATE_PLAN_READY`
- Selected future scope: AppServices composition-only
- Current batch build/test/app launch: not run

## B. Future Validation Commands

Primary compilation and composition suite:

```powershell
dotnet build FamilyClaimRef.sln
dotnet test FamilyClaimRef.sln --filter FullyQualifiedName~AppServicesTests
```

Regression-only targeted suites:

```powershell
dotnet test FamilyClaimRef.sln --filter FullyQualifiedName~ProductShellViewModelTests
dotnet test FamilyClaimRef.sln --filter FullyQualifiedName~ProductDocumentListViewModelTests
dotnet test FamilyClaimRef.sln --filter FullyQualifiedName~DocumentRegistrationViewModelTests
```

Full regression:

```powershell
dotnet test FamilyClaimRef.sln
```

The current baseline is PASS `379/379`. Future total count must be recorded from actual discovery rather than assumed.

## C. Future AppServices Composition Gates

Required:

- `AppServices.Create` exposes non-null MainWindowViewModel and ProductShellViewModel.
- ProductShell child graph uses the existing interface services and workflow.
- MainWindow and ProductShell DocumentRegistrationViewModel instances are different.
- ProductShellViewModel exposes the injected ProductDocumentListViewModel instance.
- Separate AppServices creation calls produce separate ViewModel graphs.
- Existing runtime-root path assertions remain valid.
- Creating the graph does not create project-root or runtime metadata/attachment files.
- Null runtime-root-provider behavior remains unchanged.

## D. Fallback Resource Gates

Application-null AppServices tests must resolve these exact seven values without `[[key]]` fallback text:

| Key | Expected value |
|---|---|
| `Ui.Product.Shell.Title` | `FamilyClaimRef` |
| `Ui.Product.Navigation.Home` | `홈` |
| `Ui.Product.Navigation.DocumentRegistration` | `문서 등록` |
| `Ui.Product.Navigation.DocumentList` | `문서 목록` |
| `Ui.Product.DocumentList.Title` | `문서 목록` |
| `Ui.Product.DocumentList.EmptyMessage` | `등록된 문서가 없습니다.` |
| `Ui.Product.DocumentList.LoadFailedMessage` | `문서 목록을 불러오지 못했습니다.` |

Additional gates:

- The 14 existing DocumentRegistrationViewModel fallback keys remain present.
- `UiStrings.xaml` and `UiTextKeys.cs` remain unchanged in composition-only implementation.
- Fallback additions are mirrors, not a new resource source of truth.

## E. Lifetime And Source Wiring Gates

- Shared infrastructure is established in one `AppServices.Create` call before child ViewModels are constructed.
- No mutable ViewModel instance is shared between MainWindow and ProductShell graphs.
- Private service identity is verified by source wiring review; no reflection-only production API or new service exposure is added solely for tests.
- No DI container, service locator, bootstrapper, factory interface, or runtime-mode service is introduced.

## F. Runtime And Startup Negative Gates

For composition-only implementation:

- `App.xaml` unchanged.
- `App.xaml.cs` unchanged.
- MainWindow XAML/code-behind/ViewModel unchanged.
- ProductShellWindow XAML/code-behind unchanged.
- No `ProductShellWindow` construction in runtime source.
- No added `Show` or `ShowDialog`.
- No startup environment variable or command-line switch.
- No launcher button and no dual Window launch.
- No app launch, manual workflow, OpenFileDialog, screenshot, or visual automation.

## G. Test And Scope Gates

- Future exact changed scope equals the separately approved candidate.
- Existing tests deleted: 0.
- Existing assertions weakened: 0.
- ProductShell XAML compile remains PASS.
- Full tests remain at least the `379` baseline plus actual new AppServices cases.
- `git diff --check`, trailing whitespace, EOF, privacy, protected ignore, and root-artifact gates pass.
- App launch/manual runtime evidence remains not run for composition-only.

## H. Current Batch Result

- Build: not run.
- Targeted tests: not run.
- Full tests: not run.
- App launch: not run.
- Source/test/runtime changes: none.

These commands and gates are future planning only and do not authorize implementation.
