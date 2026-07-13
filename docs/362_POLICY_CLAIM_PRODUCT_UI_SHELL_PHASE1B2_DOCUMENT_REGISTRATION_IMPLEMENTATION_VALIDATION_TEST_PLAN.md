# Product UI Shell Phase 1B2 Document Registration Implementation Validation Test Plan

## A. Status

- Marker: `POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1B2_DOCUMENT_REGISTRATION_IMPLEMENTATION_VALIDATION_TEST_PLAN_READY`
- Baseline known full tests: PASS 351/351
- Current execution: documentation-only; commands not run

## B. Future Validation Commands

Run only in a separately approved implementation batch.

```powershell
dotnet build FamilyClaimRef.sln
dotnet test FamilyClaimRef.sln --filter FullyQualifiedName~DocumentRegistrationViewModelTests
dotnet test FamilyClaimRef.sln --filter FullyQualifiedName~ProductShellViewModelTests
dotnet test FamilyClaimRef.sln --filter FullyQualifiedName~ResourceUiTextProviderTests
dotnet test FamilyClaimRef.sln
```

## C. Build and XAML Gates

- Solution build: PASS, warnings 0, errors 0.
- `ProductDocumentRegistrationView.xaml`: compiles.
- `.sln` and `.csproj`: unchanged.
- ProductShell runtime entry: absent.
- `AppServices`, `MainWindow`, `App.xaml`, and `App.xaml.cs`: unchanged.

## D. Composition and Lifecycle Gates

- `ProductShellViewModel` constructor accepts `IUiTextProvider` and the existing `DocumentRegistrationViewModel`.
- `ProductShellViewModel.DocumentRegistration` exposes the same instance as a read-only property.
- Constructor null guards and property identity are tested.
- Existing navigation count, order, IDs, copy, selection, and rejection behavior remains preserved.
- `ProductDocumentRegistrationView.Loaded` forwards to `LoadTargetOptionsAsync()` on every activation.
- Sequential repeated-load regression confirms the second storage snapshot replaces the first.
- Removed or disabled targets are absent after reload.
- Invalid prior selections are cleared.
- Duplicate options are not introduced.
- Repeated loading does not call the workflow or file picker.
- No one-time cache, initialized flag, router, lifecycle service, or command framework is added.

## E. Registration Behavior Gates

- Existing `DocumentRegistrationViewModel` production behavior remains unchanged.
- Existing registration validation and status behavior remains preserved.
- `DocumentRegistrationWorkflow` is reused and not bypassed.
- `IFilePickerService` is reused; the view does not instantiate `OpenFileDialog`.
- Product view code-behind forwards only Loaded, select-file, and register events.
- No silent catch is added.
- `LastRegistrationSummary` is not displayed.

## F. Option and Copy Gates

- Target-kind visible options use approved product labels; raw `policy`/`claim` remain non-visible technical values only.
- Document-type options use scope-appropriate `DocumentTypeSeeds` items.
- Document-type display uses `Label`; selected storage value uses `Code`.
- Policy and claim target items display `DisplayTitle`, not IDs.
- Reference date remains a `DatePicker` binding.
- Production direct Korean XAML literals: 0.
- Existing conflicting static keys are not used by the product target section.
- Shared static copy use matches docs/361.
- Target-specific runtime-message exception is recorded and unchanged.

## G. Resource Gates

- Resources/constants: 67/67.
- `Ui.Product.*` resources/constants: 11/11.
- Resource/constant mismatch: 0.
- Exact assertions exist for:
  - `Ui.Product.DocumentRegistration.TargetSelectionSection = 연결 대상 선택`
  - `Ui.Product.DocumentRegistration.PolicyTargetLabel = 보험 계약`
  - `Ui.Product.DocumentRegistration.ClaimTargetLabel = 청구 건`
- Existing 64 resource keys remain present.
- Existing resource values remain unchanged.
- Deleted or renamed keys: 0.

## H. ProductShell Regression Gates

- Home remains the initial selection and maps to `ProductHomeView`.
- DocumentRegistration maps to `ProductDocumentRegistrationView` only after approved implementation.
- DocumentList preserves the existing fallback.
- `ProductDocumentListView` remains absent.
- `ProductShellWindow.xaml.cs` remains unchanged.
- Runtime entry remains absent.

## I. Test Accounting

- Prior full-test baseline: 351.
- Added test count must be reported.
- Existing tests deleted: 0.
- Targeted suites fail 0.
- Full solution tests fail 0.
- Any total-count change must be explained by the approved test modifications.

## J. Current Batch Execution Record

| Execution | Result |
|---|---|
| build | not run |
| DocumentRegistration targeted tests | not run |
| ProductShell targeted tests | not run |
| resource targeted tests | not run |
| full tests | not run |
| app launch | not run |
| OpenFileDialog | not run |
| manual workflow | not run |
| reason | documentation-only implementation-contract approval batch |
