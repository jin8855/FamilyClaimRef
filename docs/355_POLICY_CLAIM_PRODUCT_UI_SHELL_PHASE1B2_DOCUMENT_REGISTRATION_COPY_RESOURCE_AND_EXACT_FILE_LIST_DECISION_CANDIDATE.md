# Product UI Shell Phase 1B2 Document Registration Copy Resource and Exact File List Decision Candidate

## A. Status

- Marker: `POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1B2_DOCUMENT_REGISTRATION_COPY_RESOURCE_AND_EXACT_FILE_LIST_DECISION_CANDIDATE_READY`
- Selected architecture: Candidate A, conditional direct reuse
- Implementation target now: 0
- Exact implementation file list approved now: no

## B. Current Resource Baseline

- `UiStrings.xaml` `Ui.*` resources: 64
- `UiTextKeys.cs` `Ui.*` constants: 64
- `Ui.Product.*` resources/constants: 8/8
- Resource/constant mismatch: 0
- Approved ProductShell terminology rows: 7
- Current approved ProductShell resource rows: 8
- Existing validation-harness resource values remain preserved.

## C. Copy and Resource Inventory

| UI concept | Current resource key/value | Current ownership | Product use judgment | Additional approval |
|---|---|---|---|---|
| Product page title | `Ui.Product.DocumentRegistration.Title` = `문서 등록` | ProductShell approved resource | reuse approved | implementation approval only |
| Source file section | `Ui.Document.SourceFileSection` = `원본 파일` | shared key first used by validation harness | candidate reuse | product-use copy approval required |
| Select action | `Ui.Action.SelectFile` = `파일 선택` | shared action key first used by validation harness | candidate reuse | product-use copy approval required |
| Selected file label | `Ui.Document.SelectedFileLabel` = `선택한 파일` | shared key first used by validation harness | candidate reuse | product-use copy approval required |
| Target selection section | `Ui.Target.SelectionSection` = `저장 대상 선택` | validation-harness resource | reuse prohibited for approved ProductShell terminology | new product key approval required |
| Target kind label | `Ui.Target.KindLabel` = `대상 유형` | validation-harness resource | candidate reuse | product-use copy approval required |
| Policy target | `Ui.Policy.TargetLabel` = `보험 대상` | validation-harness resource | conflicts with approved `보험 계약` | new product key approval required |
| Claim target | `Ui.Claim.TargetLabel` = `청구 대상` | validation-harness resource | conflicts with approved `청구 건` | new product key approval required |
| Metadata section | `Ui.Document.MetadataSection` = `문서 정보` | shared key first used by validation harness | value matches approved product terminology | implementation approval only |
| Document type | `Ui.Document.TypeLabel` = `문서 유형` | shared key first used by validation harness | candidate reuse | product-use copy approval required |
| Display title | `Ui.Document.DisplayTitleLabel` = `표시 제목` | shared key first used by validation harness | candidate reuse | product-use copy approval required |
| Reference date | `Ui.Document.ReferenceDateLabel` = `기준일` | shared key first used by validation harness | candidate reuse | product-use copy approval required |
| Register action | `Ui.Action.RegisterDocument` = `등록` | shared action key first used by validation harness | candidate reuse; approved product concept is `문서 등록` | button-copy approval required |
| Validation heading | `Ui.Validation.SectionLabel` = `입력 확인` | validation-harness resource | candidate reuse | product-use copy approval required |
| Status heading | `Ui.Status.RegistrationSection` = `등록 상태`; `Ui.Status.Label` = `상태` | validation-harness resource | candidate reuse | product-use copy approval required |
| Last summary heading | `Ui.Status.LastRegistrationSummaryLabel` = `마지막 등록 요약` | validation-harness resource | not shown in Phase 1B2 candidate | diagnostic display remains deferred |
| Runtime success/failure/file messages | `Ui.DocumentRegistration.Status.*` | registration ViewModel runtime copy | candidate runtime-message reuse | product-use copy approval required |
| Runtime target/validation messages | `Ui.DocumentRegistration.Message.*`, `Ui.DocumentRegistration.Validation.*` | registration ViewModel runtime copy | some values contain `보험 대상`, `청구 대상`, or `저장할 대상`; not productized automatically | terminology exception or separate message-ownership decision required |
| Diagnostic summary formats | `policy:{policyId}; document:{documentId}`, `claim:{claimId}; document:{documentId}` | validation-harness diagnostic output | not shown in Phase 1B2 candidate | Keep deferred |

## D. Product Terminology Conflicts

| Concept | Current value | Approved ProductShell value | Decision candidate |
|---|---|---|---|
| Target selection | `저장 대상 선택` | `연결 대상 선택` | Add product-specific key candidate; do not change existing key. |
| Policy target | `보험 대상` | `보험 계약` | Add product-specific key candidate; do not change existing key. |
| Claim target | `청구 대상` | `청구 건` | Add product-specific key candidate; do not change existing key. |
| Register button | `등록` | Product concept `문서 등록` | Exact button value needs approval; do not infer a replacement. |
| Target runtime messages | existing `보험 대상`, `청구 대상`, `저장할 대상` wording | product terminology differs | Reuse is blocked until an explicit exception or separate runtime-message ownership design is approved. |

Existing `Ui.Policy.TargetLabel`, `Ui.Claim.TargetLabel`, and validation-harness values must not be changed by inference.

## E. New Product Resource Candidates

| Candidate key | Candidate value | Purpose | Approved now |
|---|---|---|---|
| `Ui.Product.DocumentRegistration.TargetSelectionSection` | `연결 대상 선택` | Product registration target section | no |
| `Ui.Product.DocumentRegistration.PolicyTargetLabel` | `보험 계약` | Product policy target option/label | no |
| `Ui.Product.DocumentRegistration.ClaimTargetLabel` | `청구 건` | Product claim target option/label | no |

- Candidate new key count: 3
- Candidate post-change resource/constant count: 67/67
- Candidate post-change `Ui.Product.*` count: 11/11
- These counts are planning values only; the current baseline remains 64/64 and 8/8.
- No new key or value is approved by this document.

## F. Resource Classification

| Classification | Keys or areas |
|---|---|
| reuse approved | `Ui.Product.DocumentRegistration.Title`; `Ui.Document.MetadataSection` value alignment |
| candidate reuse, needs copy approval | source file, selected file, file action, target kind, field labels, register action, validation/status headings |
| reuse prohibited | existing target section, policy target label, and claim target label for ProductShell terminology |
| product-specific key candidate | three keys in section E |
| runtime message reuse, conditional | generic registration status and field-validation messages |
| needs separate runtime-message decision | target-specific and target-selection messages with conflicting terminology |
| validation-harness-only | dev warnings and policy/claim management copy |
| not shown in Phase 1B2 | `LastRegistrationSummary` and its diagnostic formats |

## G. Conditional Recommended Implementation Exact File List

The following list is the smallest Candidate A file set if the three product keys, shared-copy reuse, runtime-message exception, lifecycle rule, and shell composition are separately approved.

| Exact path | Change type | Purpose | Classification | Approved now |
|---|---|---|---|---|
| `app/FamilyClaimRef.App/Views/ProductDocumentRegistrationView.xaml` | create | Stateful product registration bindings and product resource use | include candidate | no |
| `app/FamilyClaimRef.App/Views/ProductDocumentRegistrationView.xaml.cs` | create | Loaded/select/register event forwarding only | include candidate; lifecycle approval needed | no |
| `app/FamilyClaimRef.App/ViewModels/ProductShellViewModel.cs` | modify | Receive and expose existing registration ViewModel | include candidate; composition approval needed | no |
| `app/FamilyClaimRef.App/ProductShell/ProductShellWindow.xaml` | modify | Add registration content template and Id trigger | include candidate | no |
| `app/FamilyClaimRef.App/Resources/UiStrings.xaml` | modify | Add three approved-after-review product resource candidates | needs copy approval | no |
| `app/FamilyClaimRef.App/Services/Localization/UiTextKeys.cs` | modify | Add matching constants for three product keys | needs copy approval | no |
| `tests/FamilyClaimRef.App.Tests/ProductShellViewModelTests.cs` | modify | Validate new constructor/property contract and preserve navigation behavior | include candidate | no |
| `tests/FamilyClaimRef.App.Tests/Services/Localization/ResourceUiTextProviderTests.cs` | modify | Validate 67/67, 11/11, and exact approved values if approved | needs copy approval | no |
| `docs/358_POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1B2_DOCUMENT_REGISTRATION_IMPLEMENTATION_RESULT_REVIEW.md` | create | Future implementation evidence | result document candidate | no |

This list is conditional, not implementation-ready. If target runtime messages must use product-specific terminology without an exception, a separate message-ownership design is required and this list must be revised before implementation.

## H. Considered Paths Not Included

| Exact path | Judgment | Reason |
|---|---|---|
| `app/FamilyClaimRef.App/ViewModels/ProductDocumentRegistrationViewModel.cs` | not required | Wrapper is not justified by current state/workflow evidence. |
| `app/FamilyClaimRef.App/ViewModels/DocumentRegistrationViewModel.cs` | not required for Candidate A | Existing behavior is reused unchanged; becomes a candidate only after a separate runtime-message ownership decision. |
| `app/FamilyClaimRef.App/ViewModels/MainWindowViewModel.cs` | not required | Validation-harness aggregate must not be productized. |
| `app/FamilyClaimRef.App/ProductShell/ProductShellWindow.xaml.cs` | not required | XAML can bind the registration view to the shell property; no window event routing is needed. |
| `app/FamilyClaimRef.App/Composition/AppServices.cs` | deferred | No ProductShell runtime entry exists; compile-only slice does not need production composition. |
| `app/FamilyClaimRef.App/App.xaml` | not required | Existing resource dictionary merge remains sufficient. |
| `app/FamilyClaimRef.App/App.xaml.cs` | deferred | Runtime entry remains absent. |
| `app/FamilyClaimRef.App/MainWindow.xaml` | not required | Validation harness remains unchanged. |
| `app/FamilyClaimRef.App/MainWindow.xaml.cs` | not required | Product view has its own narrow forwarding candidate. |
| `tests/FamilyClaimRef.App.Tests/DocumentRegistrationViewModelTests.cs` | no modification candidate | Existing behavior remains unchanged; test suite is still a required validation gate. |
| `app/FamilyClaimRef.App/Views/ProductDocumentListView.xaml` | deferred | Document list is outside Phase 1B2. |
| `app/FamilyClaimRef.App/FamilyClaimRef.App.csproj` | not required | SDK-style WPF default inclusion applies. |

## I. Candidate Counts

| Count item | Value |
|---|---:|
| production create | 2 |
| production modify | 4 |
| test create | 0 |
| test modify | 2 |
| resource files modified, included in production modify | 2 |
| result document | 1 |
| total conditional implementation candidate files | 9 |
| implementation target now | 0 |
| source blockers | 0 |
| lifecycle blockers | 1 |
| copy/resource blockers | 2 |
| composition blockers | 1 |

Copy/resource blocker 1: approve three product-specific static keys and the reuse boundary for shared static copy.

Copy/resource blocker 2: decide whether existing target-specific runtime messages may appear in ProductShell or require a separate product message ownership design.

## J. Readiness Judgment

`NEEDS COPY/RESOURCE APPROVAL`

Also required before implementation:

- explicit ProductShellViewModel composition approval
- explicit target-load lifecycle approval
- exact conditional file list revalidation after copy/runtime-message decisions

Implementation must not start from this document.
