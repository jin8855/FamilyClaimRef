# Policy/Claim UI String Inventory Review

## A. Status

Status: INVENTORY_REVIEW_ONLY

Marker:

POLICY_CLAIM_UI_STRING_INVENTORY_RECORDED

This document records a read-only UI string inventory.

No string is changed by this document.

No resource file is created by this document.

No localization implementation is authorized by this document.

## B. Baseline

Record:

- latest commit:
  893311f docs(familyclaimref): close core validation status review

## C. Read-Only Inspection Scope

Record inspected files/areas:

- XAML files inspected:
  app/FamilyClaimRef.App/App.xaml, app/FamilyClaimRef.App/MainWindow.xaml
- ViewModel files inspected:
  app/FamilyClaimRef.App/ViewModels/DocumentRegistrationViewModel.cs, app/FamilyClaimRef.App/ViewModels/PolicyClaimManagementViewModel.cs, app/FamilyClaimRef.App/ViewModels/MainWindowViewModel.cs
- code-behind files inspected:
  app/FamilyClaimRef.App/App.xaml.cs, app/FamilyClaimRef.App/MainWindow.xaml.cs
- source files in app excluding bin/obj:
  51 files
- existing resource/localization files found:
  0 source files named Resource, Resources, Strings, Localization, UIStrings, or .resx
- wireframe files found in repo:
  22 HTML files under design/wireframes
- wireframes.zip:
  not present in repo

Do not invent files.

## D. String Categories

Classify observed strings into categories:

1. Window/app title strings
2. Section headers
3. Field labels
4. Button labels
5. Validation messages
6. Status messages
7. Target selection messages
8. Last registration summary strings
9. Management panel strings
10. Diagnostic/dev validation strings
11. Synthetic/test-only strings
12. Internal exception strings

## E. Current Literal String Findings

Representative XAML findings:

| Source file | Current literal string | Category | Proposed resource key candidate | Product-facing or validation-harness-only | Notes |
|---|---|---|---|---|---|
| app/FamilyClaimRef.App/MainWindow.xaml | FamilyClaimRef | Window/app title strings | Ui.App.Title | product-facing candidate | Appears as Window Title and top label. |
| app/FamilyClaimRef.App/MainWindow.xaml | Local MVP validation screen. Do not use real personal, insurer, hospital, diagnosis, policy number, or claim number samples. | Diagnostic/dev validation strings | Ui.DevHarness.Warning.LocalMvpValidation | validation-harness-only | Keep as dev harness copy until product shell exists. |
| app/FamilyClaimRef.App/MainWindow.xaml | Source file | Section headers | Ui.Document.SourceFileSection | product-facing candidate | XAML GroupBox header. |
| app/FamilyClaimRef.App/MainWindow.xaml | Select file | Button labels | Ui.Action.SelectFile | product-facing candidate | File picker action label. |
| app/FamilyClaimRef.App/MainWindow.xaml | Selected file | Field labels | Ui.Document.SelectedFileLabel | product-facing candidate | Displays selected source file. |
| app/FamilyClaimRef.App/MainWindow.xaml | Target selection | Section headers | Ui.Target.SelectionSection | product-facing candidate | Registration target group. |
| app/FamilyClaimRef.App/MainWindow.xaml | Target kind | Field labels | Ui.Target.KindLabel | product-facing candidate | Target kind selector label. |
| app/FamilyClaimRef.App/MainWindow.xaml | policy | Target selection messages | Ui.Target.Kind.Policy | product-facing candidate | ComboBox value; may remain machine-like if bound to enum later. |
| app/FamilyClaimRef.App/MainWindow.xaml | claim | Target selection messages | Ui.Target.Kind.Claim | product-facing candidate | ComboBox value; may remain machine-like if bound to enum later. |
| app/FamilyClaimRef.App/MainWindow.xaml | Policy target | Field labels | Ui.Policy.TargetLabel | product-facing candidate | Policy target selector label. |
| app/FamilyClaimRef.App/MainWindow.xaml | Claim target | Field labels | Ui.Claim.TargetLabel | product-facing candidate | Claim target selector label. |
| app/FamilyClaimRef.App/MainWindow.xaml | Document metadata | Section headers | Ui.Document.MetadataSection | product-facing candidate | Metadata group header. |
| app/FamilyClaimRef.App/MainWindow.xaml | Document type | Field labels | Ui.Document.TypeLabel | product-facing candidate | Document type selector label. |
| app/FamilyClaimRef.App/MainWindow.xaml | Display title | Field labels | Ui.Document.DisplayTitleLabel | product-facing candidate | Display title field. |
| app/FamilyClaimRef.App/MainWindow.xaml | Reference date | Field labels | Ui.Document.ReferenceDateLabel | product-facing candidate | Reference date field. |
| app/FamilyClaimRef.App/MainWindow.xaml | Register | Button labels | Ui.Action.RegisterDocument | product-facing candidate | Main document registration action. |
| app/FamilyClaimRef.App/MainWindow.xaml | Is busy: {0} | Status messages | Ui.Status.IsBusyFormat | validation-harness-only | Diagnostic state display. |
| app/FamilyClaimRef.App/MainWindow.xaml | Registration status | Section headers | Ui.Status.RegistrationSection | product-facing candidate | Status group header. |
| app/FamilyClaimRef.App/MainWindow.xaml | Validation | Field labels | Ui.Validation.SectionLabel | validation-harness-only | Could change in product UI. |
| app/FamilyClaimRef.App/MainWindow.xaml | Status | Field labels | Ui.Status.Label | product-facing candidate | General status label. |
| app/FamilyClaimRef.App/MainWindow.xaml | Last registration summary | Last registration summary strings | Ui.Status.LastRegistrationSummaryLabel | validation-harness-only | Summary presentation may change in product UI. |
| app/FamilyClaimRef.App/MainWindow.xaml | Policy/Claim Management | Management panel strings | Ui.Management.PolicyClaimSection | validation-harness-only | Current management panel is harness support. |
| app/FamilyClaimRef.App/MainWindow.xaml | Create and disable local policy/claim targets with synthetic-safe titles only. | Diagnostic/dev validation strings | Ui.DevHarness.ManagementWarning | validation-harness-only | Keep as dev-only copy. |
| app/FamilyClaimRef.App/MainWindow.xaml | Policy Management | Management panel strings | Ui.Management.PolicySection | validation-harness-only | Product screen may use different IA. |
| app/FamilyClaimRef.App/MainWindow.xaml | Active policy targets | Management panel strings | Ui.Policy.ActiveTargetsLabel | validation-harness-only | Dev target list label. |
| app/FamilyClaimRef.App/MainWindow.xaml | New policy title | Management panel strings | Ui.Policy.NewTitleLabel | validation-harness-only | Synthetic policy creation label. |
| app/FamilyClaimRef.App/MainWindow.xaml | Create policy | Button labels | Ui.Action.CreatePolicy | validation-harness-only | Dev target creation action. |
| app/FamilyClaimRef.App/MainWindow.xaml | Disable policy | Button labels | Ui.Action.DisablePolicy | validation-harness-only | Dev target disable action. |
| app/FamilyClaimRef.App/MainWindow.xaml | Claim Management | Management panel strings | Ui.Management.ClaimSection | validation-harness-only | Product screen may use different IA. |
| app/FamilyClaimRef.App/MainWindow.xaml | Policy for new claim | Management panel strings | Ui.Claim.PolicyForNewClaimLabel | validation-harness-only | Dev claim creation label. |
| app/FamilyClaimRef.App/MainWindow.xaml | Active claim targets | Management panel strings | Ui.Claim.ActiveTargetsLabel | validation-harness-only | Dev target list label. |
| app/FamilyClaimRef.App/MainWindow.xaml | New claim title | Management panel strings | Ui.Claim.NewTitleLabel | validation-harness-only | Synthetic claim creation label. |
| app/FamilyClaimRef.App/MainWindow.xaml | Create claim | Button labels | Ui.Action.CreateClaim | validation-harness-only | Dev target creation action. |
| app/FamilyClaimRef.App/MainWindow.xaml | Disable claim | Button labels | Ui.Action.DisableClaim | validation-harness-only | Dev target disable action. |
| app/FamilyClaimRef.App/MainWindow.xaml | Management message | Management panel strings | Ui.Management.MessageLabel | validation-harness-only | Displays PolicyClaimManagementViewModel message. |

Representative ViewModel findings:

| Source file | Current literal string | Category | Proposed resource key candidate | Product-facing or validation-harness-only | Notes |
|---|---|---|---|---|---|
| app/FamilyClaimRef.App/ViewModels/DocumentRegistrationViewModel.cs | No active claim is available for selection. | Target selection messages | Ui.Target.NoActiveClaim | product-facing candidate | Runtime message from target selection. |
| app/FamilyClaimRef.App/ViewModels/DocumentRegistrationViewModel.cs | No active policy is available for selection. | Target selection messages | Ui.Target.NoActivePolicy | product-facing candidate | Runtime message from target selection. |
| app/FamilyClaimRef.App/ViewModels/DocumentRegistrationViewModel.cs | Select a claim before registering this document. | Validation messages | Ui.Validation.SelectClaimBeforeRegister | product-facing candidate | Guard message. |
| app/FamilyClaimRef.App/ViewModels/DocumentRegistrationViewModel.cs | Select a policy before registering this document. | Validation messages | Ui.Validation.SelectPolicyBeforeRegister | product-facing candidate | Guard message. |
| app/FamilyClaimRef.App/ViewModels/DocumentRegistrationViewModel.cs | 문서 등록에 실패했습니다. | Status messages | Ui.Status.DocumentRegistrationFailed | product-facing candidate | Existing Korean literal; should be moved behind resource/message provider later. |
| app/FamilyClaimRef.App/ViewModels/DocumentRegistrationViewModel.cs | 문서 등록이 완료되었습니다. | Status messages | Ui.Status.DocumentRegistrationCompleted | product-facing candidate | Existing Korean literal; should be moved behind resource/message provider later. |
| app/FamilyClaimRef.App/ViewModels/DocumentRegistrationViewModel.cs | 파일을 선택했습니다. | Status messages | Ui.Status.FileSelected | product-facing candidate | Existing Korean literal. |
| app/FamilyClaimRef.App/ViewModels/DocumentRegistrationViewModel.cs | 파일을 선택해 주세요. | Validation messages | Ui.Validation.SelectFile | product-facing candidate | Existing Korean literal. |
| app/FamilyClaimRef.App/ViewModels/DocumentRegistrationViewModel.cs | 저장할 대상 유형을 선택해 주세요. | Validation messages | Ui.Validation.SelectTargetKind | product-facing candidate | Existing Korean literal. |
| app/FamilyClaimRef.App/ViewModels/DocumentRegistrationViewModel.cs | 저장할 대상을 입력해 주세요. | Validation messages | Ui.Validation.SelectTarget | product-facing candidate | Existing Korean literal. |
| app/FamilyClaimRef.App/ViewModels/DocumentRegistrationViewModel.cs | 문서 유형을 선택해 주세요. | Validation messages | Ui.Validation.SelectDocumentType | product-facing candidate | Existing Korean literal. |
| app/FamilyClaimRef.App/ViewModels/DocumentRegistrationViewModel.cs | 표시 제목을 입력해 주세요. | Validation messages | Ui.Validation.EnterDisplayTitle | product-facing candidate | Existing Korean literal. |
| app/FamilyClaimRef.App/ViewModels/DocumentRegistrationViewModel.cs | 기준일을 선택해 주세요. | Validation messages | Ui.Validation.SelectReferenceDate | product-facing candidate | Existing Korean literal. |
| app/FamilyClaimRef.App/ViewModels/PolicyClaimManagementViewModel.cs | Claim target was created. | Management panel strings | Ui.Management.ClaimCreated | validation-harness-only | Synthetic target management feedback. |
| app/FamilyClaimRef.App/ViewModels/PolicyClaimManagementViewModel.cs | Claim target was disabled. | Management panel strings | Ui.Management.ClaimDisabled | validation-harness-only | Synthetic target management feedback. |
| app/FamilyClaimRef.App/ViewModels/PolicyClaimManagementViewModel.cs | Claim target title is required. | Validation messages | Ui.Validation.ClaimTitleRequired | validation-harness-only | Dev target management guard. |
| app/FamilyClaimRef.App/ViewModels/PolicyClaimManagementViewModel.cs | Policy target was created. | Management panel strings | Ui.Management.PolicyCreated | validation-harness-only | Synthetic target management feedback. |
| app/FamilyClaimRef.App/ViewModels/PolicyClaimManagementViewModel.cs | Policy target was disabled. | Management panel strings | Ui.Management.PolicyDisabled | validation-harness-only | Synthetic target management feedback. |
| app/FamilyClaimRef.App/ViewModels/PolicyClaimManagementViewModel.cs | Policy target has active claim targets. Disable claim targets first. | Validation messages | Ui.Validation.PolicyDisableBlockedByClaims | validation-harness-only | Product copy may differ. |
| app/FamilyClaimRef.App/ViewModels/PolicyClaimManagementViewModel.cs | Select an active policy target before creating a claim target. | Validation messages | Ui.Validation.SelectPolicyBeforeCreateClaim | validation-harness-only | Dev target management guard. |
| app/FamilyClaimRef.App/ViewModels/PolicyClaimManagementViewModel.cs | Policy target title is required. | Validation messages | Ui.Validation.PolicyTitleRequired | validation-harness-only | Dev target management guard. |
| app/FamilyClaimRef.App/ViewModels/PolicyClaimManagementViewModel.cs | Select a claim target. | Validation messages | Ui.Validation.SelectClaimTarget | validation-harness-only | Dev target management guard. |
| app/FamilyClaimRef.App/ViewModels/PolicyClaimManagementViewModel.cs | Select a policy target. | Validation messages | Ui.Validation.SelectPolicyTarget | validation-harness-only | Dev target management guard. |

Grouped counts from read-only inspection:

- XAML literal hits using Title/Text/Content/Header:
  58
- ViewModel representative message/property hits:
  52
- Source resource/localization files found:
  0

## F. Proposed Resource Key Prefixes

Recommend key prefixes:

- Ui.App.*
- Ui.Nav.*
- Ui.Document.*
- Ui.Target.*
- Ui.Policy.*
- Ui.Claim.*
- Ui.Management.*
- Ui.Action.*
- Ui.Status.*
- Ui.Validation.*
- Ui.Diagnostics.*
- Ui.DevHarness.*

## G. Strings Not To Localize Yet

Record:

- synthetic test titles
- generated identifiers
- machine-readable status markers
- internal exception class names
- commit messages
- path placeholders
- environment variable names
- business duplicate final UX copy, until policy decision
- raw diagnostic summaries needed for validation

## H. Inventory Judgment

POLICY_CLAIM_UI_STRING_INVENTORY_READY_FOR_RESOURCE_ARCHITECTURE
