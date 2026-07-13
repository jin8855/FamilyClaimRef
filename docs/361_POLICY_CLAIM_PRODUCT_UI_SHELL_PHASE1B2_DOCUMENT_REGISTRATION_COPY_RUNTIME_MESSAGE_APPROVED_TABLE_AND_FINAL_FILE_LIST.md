# Product UI Shell Phase 1B2 Document Registration Copy Runtime Message Approved Table and Final File List

## A. Status

- Marker: `POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1B2_DOCUMENT_REGISTRATION_COPY_RUNTIME_MESSAGE_APPROVED_TABLE_AND_FINAL_FILE_LIST_READY`
- Contract type: future implementation copy, option-display, and exact-file candidate
- Implemented now: no

## B. Product-Specific Static Resource Contract

| Future resource key | Exact value | Approved for future candidate | Implemented now |
|---|---|---|---|
| `Ui.Product.DocumentRegistration.TargetSelectionSection` | `연결 대상 선택` | yes | no |
| `Ui.Product.DocumentRegistration.PolicyTargetLabel` | `보험 계약` | yes | no |
| `Ui.Product.DocumentRegistration.ClaimTargetLabel` | `청구 건` | yes | no |

Count contract:

- Current `Ui.*` resources/constants: 64/64
- Future candidate `Ui.*` resources/constants: 67/67
- Current `Ui.Product.*` resources/constants: 8/8
- Future candidate `Ui.Product.*` resources/constants: 11/11
- Existing key deletion: 0
- Existing key rename: 0
- Existing resource value modification: 0

The following existing conflicting values remain unchanged and are not used as product target-section/target-option copy:

| Existing key | Preserved value | Product registration use |
|---|---|---|
| `Ui.Target.SelectionSection` | `저장 대상 선택` | no |
| `Ui.Policy.TargetLabel` | `보험 대상` | no |
| `Ui.Claim.TargetLabel` | `청구 대상` | no |

## C. Approved Shared Static Copy Reuse

| Existing key | Current value | Future product use |
|---|---|---|
| `Ui.Product.DocumentRegistration.Title` | `문서 등록` | page title |
| `Ui.Document.SourceFileSection` | `원본 파일` | source-file section |
| `Ui.Action.SelectFile` | `파일 선택` | file-picker action |
| `Ui.Document.SelectedFileLabel` | `선택한 파일` | selected-file label |
| `Ui.Target.KindLabel` | `대상 유형` | target-kind field label |
| `Ui.Document.MetadataSection` | `문서 정보` | metadata section |
| `Ui.Document.TypeLabel` | `문서 유형` | document-type field label |
| `Ui.Document.DisplayTitleLabel` | `표시 제목` | display-title field label |
| `Ui.Document.ReferenceDateLabel` | `기준일` | reference-date field label |
| `Ui.Action.RegisterDocument` | `등록` | register button action |
| `Ui.Validation.SectionLabel` | `입력 확인` | validation section |
| `Ui.Status.RegistrationSection` | `등록 상태` | registration-status section |
| `Ui.Status.Label` | `상태` | status label |

This approval is limited to section, field, action, validation, and status copy. It does not transfer validation-harness ownership to ProductShell, does not change any existing value, and does not allow direct Korean XAML literals.

## D. Generic Runtime-Message Reuse

| Existing key | Future product display |
|---|---|
| `Ui.DocumentRegistration.Status.CleanupFailed` | approved |
| `Ui.DocumentRegistration.Status.Failed` | approved |
| `Ui.DocumentRegistration.Status.Completed` | approved |
| `Ui.DocumentRegistration.Status.FileSelected` | approved |
| `Ui.DocumentRegistration.Validation.SelectFile` | approved |
| `Ui.DocumentRegistration.Validation.SelectDocumentType` | approved |
| `Ui.DocumentRegistration.Validation.EnterDisplayTitle` | approved |
| `Ui.DocumentRegistration.Validation.SelectReferenceDate` | approved |

Existing keys and values remain unchanged.

## E. Target-Specific Compile-Only Compatibility Exception

| Existing key | Compile-only treatment | Final product terminology status |
|---|---|---|
| `Ui.DocumentRegistration.Message.NoActiveClaim` | preserve existing behavior | unresolved before runtime entry |
| `Ui.DocumentRegistration.Message.NoActivePolicy` | preserve existing behavior | unresolved before runtime entry |
| `Ui.DocumentRegistration.Validation.SelectClaimBeforeRegister` | preserve existing behavior | unresolved before runtime entry |
| `Ui.DocumentRegistration.Validation.SelectPolicyBeforeRegister` | preserve existing behavior | unresolved before runtime entry |
| `Ui.DocumentRegistration.Validation.SelectTarget` | preserve existing behavior | unresolved before runtime entry |
| `Ui.DocumentRegistration.Validation.SelectTargetKind` | preserve existing behavior | unresolved before runtime entry |

Exception boundary:

- Existing key/value changes: none.
- `DocumentRegistrationViewModel` changes: none.
- This is a compile-only compatibility exception, not final terminology approval.
- This does not approve ProductShell runtime entry.
- A terminology-convergence decision is required before runtime entry.
- The future docs/358 result review must record this exception.

## F. Excluded Display State

The Phase 1B2 product view must not display:

- `Ui.Status.LastRegistrationSummaryLabel`
- `LastRegistrationSummary`
- `policy:{policyId}; document:{documentId}`
- `claim:{claimId}; document:{documentId}`

Diagnostic summary formats remain deferred.

## G. Option-Display Audit

Audit result: `PASS_WITH_REQUIRED_BINDING_CONTRACT`.

| Option area | Current source evidence | Required future product binding | Judgment |
|---|---|---|---|
| target kind | no `ItemsSource`; inline items use raw `Content`, `SelectedValuePath="Content"`, and display `policy`/`claim` | set item `Content` from approved product policy/claim resources, keep the technical code in `Tag`, and use `SelectedValuePath="Tag"` | PASS with binding contract |
| document type | no `ItemsSource`; 11 inline raw-code items use `SelectedValuePath="Content"` | bind scope-appropriate `DocumentTypeSeeds.Policy` or `DocumentTypeSeeds.Claim`; `DisplayMemberPath="Label"`, `SelectedValuePath="Code"` | PASS with existing seed contract |
| policy target | `ItemsSource=AvailablePolicies`, `DisplayMemberPath=DisplayTitle`, `SelectedValuePath=Id` | preserve | PASS |
| claim target | `ItemsSource=AvailableClaims`, `DisplayMemberPath=DisplayTitle`, `SelectedValuePath=Id` | preserve | PASS |
| reference date | `DatePicker.SelectedDate` binds to `ReferenceDate`; no explicit `StringFormat` | preserve culture-aware DatePicker display without adding a raw format literal | PASS |

The current validation-harness raw ComboBox markup is not approved for verbatim reuse. The future product view must use the binding contract above. Existing `DocumentTypeSeeds` already provide Korean labels and storage codes, so no additional resource key, ViewModel, or production file is required.

Additional option-display blocker: none after applying this binding contract.

## H. Final Future Implementation Candidate

Final exact candidate approval state: **approved for future implementation candidate, conditional on a separate exact implementation instruction and compliance with section G**.

Created:

1. `app/FamilyClaimRef.App/Views/ProductDocumentRegistrationView.xaml`
2. `app/FamilyClaimRef.App/Views/ProductDocumentRegistrationView.xaml.cs`
3. `docs/358_POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1B2_DOCUMENT_REGISTRATION_IMPLEMENTATION_RESULT_REVIEW.md`

Modified production/resource:

4. `app/FamilyClaimRef.App/ViewModels/ProductShellViewModel.cs`
5. `app/FamilyClaimRef.App/ProductShell/ProductShellWindow.xaml`
6. `app/FamilyClaimRef.App/Resources/UiStrings.xaml`
7. `app/FamilyClaimRef.App/Services/Localization/UiTextKeys.cs`

Modified tests:

8. `tests/FamilyClaimRef.App.Tests/ProductShellViewModelTests.cs`
9. `tests/FamilyClaimRef.App.Tests/DocumentRegistrationViewModelTests.cs`
10. `tests/FamilyClaimRef.App.Tests/Services/Localization/ResourceUiTextProviderTests.cs`

Candidate counts:

| Count item | Value |
|---|---:|
| production create | 2 |
| production modify | 4 |
| test create | 0 |
| test modify | 3 |
| result document | 1 |
| total candidate files | 10 |

Not included:

- `DocumentRegistrationViewModel.cs`
- `ProductDocumentRegistrationViewModel.cs`
- `ProductShellWindow.xaml.cs`
- `AppServices.cs`
- `App.xaml` or `App.xaml.cs`
- `MainWindow*` or `MainWindowViewModel.cs`
- project files
- `ProductDocumentListView*`
- new command, router, service, or lifecycle files

Implementation target now: 0.
