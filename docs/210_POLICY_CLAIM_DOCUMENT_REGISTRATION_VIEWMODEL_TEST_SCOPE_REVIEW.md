# Policy/Claim Document Registration ViewModel Test Scope Review

## A. Status

Status: TEST_SCOPE_REVIEW_ONLY

## B. Read-Only Source Findings

### Confirmed

- `DocumentRegistrationViewModel` exists at `app/FamilyClaimRef.App/ViewModels/DocumentRegistrationViewModel.cs`.
- `MainWindowViewModel` delegates `LoadAsync`, `SelectFileAsync`, and `RegisterAsync` to `DocumentRegistrationViewModel`.
- `MainWindow.xaml` binds source file, target kind, selected policy, selected claim, document type, display title, reference date, validation message, status message, and last registration summary to the ViewModel.
- `DocumentRegistrationViewModel.RegisterAsync` calls an internal validation path before calling `DocumentRegistrationWorkflow`.
- The ViewModel has explicit state for:
  - `SelectedSourceFilePath`
  - `SelectedSourceFileDisplayName`
  - `TargetKind`
  - `TargetId`
  - `SelectedPolicyId`
  - `SelectedClaimId`
  - `DocumentType`
  - `DisplayTitle`
  - `ReferenceDate`
  - `ValidationMessage`
  - `StatusMessage`
  - `TargetSelectionMessage`
  - `LastRegistrationSummary`
- Existing `DocumentRegistrationViewModelTests` already cover constructor null guards, file picker selection and cancel behavior, active target option loading, disabled target exclusion, policy/claim target selection mapping, empty target messages, missing source path, missing target id, missing document type, missing display title, invalid target kind, default reference date, success paths, workflow failure message, cleanup failure message, and project root safety.
- Existing tests use fake picker, fake policy/claim storage, spy attachment service, and spy document storage without launching the app.
- Existing tests do not require OpenFileDialog.

### Candidate

- Future ViewModel validation tests can consolidate current scattered validation checks into a focused validation suite.
- Future tests can assert workflow-not-called behavior through existing spy/fake services.
- Future tests can prefer `ValidationMessage` non-empty or current stable markers instead of final product copy.
- Future tests can include target option loaded/not-loaded boundaries because `ValidateTargetSelection` changes behavior after target options are loaded.
- Future tests can confirm XAML-only target visibility remains out of scope for ViewModel unit tests.

### Unknown

- The ViewModel does not expose a separate command object or `CanExecute` API today.
- Whether future product UI should disable the Register button before validation is not decided.
- Whether document type should be constrained by target kind at ViewModel level is not decided.
- Whether final validation copy should be localized through resources is deferred.

## C. Risk Review

Record:

- WPF command behavior may require dispatcher or property changed handling if future command objects are introduced.
- OpenFileDialog behavior should not be tested here.
- Some validation may currently live only in XAML binding or code-behind.
- If production ViewModel change is needed, future implementation must STOP_AND_REPORT.
- Localization/resource extraction remains deferred.

## D. Recommended Strategy

Record:

- Prefer ViewModel unit tests with fake workflow/service dependencies.
- Avoid XAML automation.
- Avoid app launch.
- Avoid exact UI copy assertions unless the batch explicitly treats current strings as stable markers.
- Test validation guard and workflow-not-called behavior first.
- Reuse existing fake services in `DocumentRegistrationViewModelTests` where possible.
- Classify XAML-only behavior as deferred.

## E. Scope Judgment

POLICY_CLAIM_DOCUMENT_REGISTRATION_VIEWMODEL_TEST_SCOPE_READY
