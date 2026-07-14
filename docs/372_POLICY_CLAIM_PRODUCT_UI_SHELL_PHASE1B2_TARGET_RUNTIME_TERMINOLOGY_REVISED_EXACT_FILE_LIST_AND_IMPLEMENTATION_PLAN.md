# Policy Claim Product UI Shell Phase 1B2 Target Runtime Terminology Revised Exact File List and Implementation Plan

## A. Status

- Marker: `POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1B2_TARGET_RUNTIME_TERMINOLOGY_REVISED_EXACT_FILE_LIST_AND_IMPLEMENTATION_PLAN_READY`
- Selected strategy: Candidate A, update existing shared values
- Candidate status: revised and ready for separate review
- Implementation target now: 0
- Exact implementation list approved now: no

## B. Candidate Values

The six candidate values are unchanged from docs/366.

| Resource key | Current value | Future candidate value | Approved now |
|---|---|---|---|
| `Ui.DocumentRegistration.Message.NoActiveClaim` | `선택할 수 있는 활성 청구 대상이 없습니다.` | `선택할 수 있는 청구 건이 없습니다.` | no |
| `Ui.DocumentRegistration.Message.NoActivePolicy` | `선택할 수 있는 활성 보험 대상이 없습니다.` | `선택할 수 있는 보험 계약이 없습니다.` | no |
| `Ui.DocumentRegistration.Validation.SelectClaimBeforeRegister` | `문서를 등록하기 전에 청구 대상을 선택해 주세요.` | `문서를 등록하기 전에 연결할 청구 건을 선택해 주세요.` | no |
| `Ui.DocumentRegistration.Validation.SelectPolicyBeforeRegister` | `문서를 등록하기 전에 보험 대상을 선택해 주세요.` | `문서를 등록하기 전에 연결할 보험 계약을 선택해 주세요.` | no |
| `Ui.DocumentRegistration.Validation.SelectTarget` | `저장할 대상을 선택해 주세요.` | `연결할 대상을 선택해 주세요.` | no |
| `Ui.DocumentRegistration.Validation.SelectTargetKind` | `저장할 대상 유형을 선택해 주세요.` | `연결 대상 유형을 선택해 주세요.` | no |

## C. Original and Revised Scope

| Scope item | Original docs/366~368 | Revised candidate |
|---|---:|---:|
| production/resource modified files | 1 | 2 |
| test modified files | 2 | 3 |
| result document | 1 | 1 |
| total files | 4 | 6 |
| production source-code modification required | no | yes, one value-only fallback update |

Superseded exclusions:

- `app/FamilyClaimRef.App/Composition/AppServices.cs` is no longer excluded.
- `tests/FamilyClaimRef.App.Tests/PolicyClaimManagementViewModelTests.cs` is added because its document-registration fixture mirrors the old target-kind value.

## D. Revised Future Exact Candidate

Modified production/resource:

- `app/FamilyClaimRef.App/Resources/UiStrings.xaml`
- `app/FamilyClaimRef.App/Composition/AppServices.cs`

Modified tests:

- `tests/FamilyClaimRef.App.Tests/DocumentRegistrationViewModelTests.cs`
- `tests/FamilyClaimRef.App.Tests/PolicyClaimManagementViewModelTests.cs`
- `tests/FamilyClaimRef.App.Tests/Services/Localization/ResourceUiTextProviderTests.cs`

Created result document:

- `docs/369_POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1B2_TARGET_RUNTIME_TERMINOLOGY_IMPLEMENTATION_RESULT_REVIEW.md`

Candidate counts:

- Production/resource modified files: 2.
- Test modified files: 3.
- Result document: 1.
- Total candidate files: 6.

This exact list is a candidate, not an implementation approval.

## E. Future File Contracts

### `UiStrings.xaml`

- Change exactly six canonical values.
- Preserve all 67 keys and key order.
- Preserve the other 61 values.

### `AppServices.cs`

- Change only the observed `DocumentRegistrationValidationSelectTargetKind` fallback dictionary value.
- Do not change service creation, constructors, dependency injection, runtime root, startup, MainWindow, or ProductShell composition.
- Do not change any other fallback value.

### `DocumentRegistrationViewModelTests.cs`

- Update six exact user-visible assertions and the six matching provider dictionary values.
- Preserve methods, setup, workflow behavior, and test count.

### `PolicyClaimManagementViewModelTests.cs`

- Update only the `DocumentRegistrationValidationSelectTargetKind` value in `CreateDocumentRegistrationUiTextProvider()`.
- Preserve management assertions, management behavior, methods, and test count.

### `ResourceUiTextProviderTests.cs`

- Update the five existing direct exact-value rows.
- Add one direct exact-value row for `DocumentRegistrationValidationSelectTargetKind` if the future approval requires six direct assertions.
- Preserve provider behavior and all resource/constant inventory checks.
- Record the resulting evidence-backed test-count change if the new theory row is added.

### `docs/369`

- Record the revised six-file scope and both mirrored dependencies.

## F. Files That Remain Excluded

- `app/FamilyClaimRef.App/Services/Localization/UiTextKeys.cs`
- `app/FamilyClaimRef.App/ViewModels/DocumentRegistrationViewModel.cs`
- `app/FamilyClaimRef.App/ViewModels/PolicyClaimManagementViewModel.cs`
- `app/FamilyClaimRef.App/Views/ProductDocumentRegistrationView.xaml`
- `app/FamilyClaimRef.App/Views/ProductDocumentRegistrationView.xaml.cs`
- `app/FamilyClaimRef.App/ViewModels/ProductShellViewModel.cs`
- `app/FamilyClaimRef.App/ProductShell/ProductShellWindow.xaml`
- `app/FamilyClaimRef.App/ProductShell/ProductShellWindow.xaml.cs`
- MainWindow and App startup files
- project, solution, and package files

## G. Count Contract

| Item | Current | Future candidate |
|---|---:|---:|
| `Ui.*` resources/constants | 67/67 | 67/67 |
| `Ui.Product.*` resources/constants | 11/11 | 11/11 |
| new/deleted/renamed keys | 0/0/0 | 0/0/0 |
| canonical changed resource values | 0 | 6 |
| fallback mirrored value updates | 0 | 1 |
| test-fixture mirrored value updates | 0 | 1 |
| generic runtime-message changes | 0 | 0 |

Fallback and fixture mirror updates do not create additional resource-key changes.

## H. Approval Boundary

Every future file row and value row remains `Approved now = no`. Do not implement this candidate until a separate exact implementation directive is issued.
