# Product UI Shell Phase 1B2 Target Runtime Message Candidate Value Table and Implementation Plan

## A. Status

- Marker: `POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1B2_TARGET_RUNTIME_MESSAGE_CANDIDATE_VALUE_TABLE_AND_IMPLEMENTATION_PLAN_READY`
- Selected strategy: Candidate A, update existing shared values
- Candidate row count: 6
- Implementation target now: 0
- Candidate approval state: not approved

## B. Exact Candidate Value Table

| Resource key | Current value | Recommended candidate value | Implementation target now |
|---|---|---|---|
| `Ui.DocumentRegistration.Message.NoActiveClaim` | `선택할 수 있는 활성 청구 대상이 없습니다.` | `선택할 수 있는 청구 건이 없습니다.` | no |
| `Ui.DocumentRegistration.Message.NoActivePolicy` | `선택할 수 있는 활성 보험 대상이 없습니다.` | `선택할 수 있는 보험 계약이 없습니다.` | no |
| `Ui.DocumentRegistration.Validation.SelectClaimBeforeRegister` | `문서를 등록하기 전에 청구 대상을 선택해 주세요.` | `문서를 등록하기 전에 연결할 청구 건을 선택해 주세요.` | no |
| `Ui.DocumentRegistration.Validation.SelectPolicyBeforeRegister` | `문서를 등록하기 전에 보험 대상을 선택해 주세요.` | `문서를 등록하기 전에 연결할 보험 계약을 선택해 주세요.` | no |
| `Ui.DocumentRegistration.Validation.SelectTarget` | `저장할 대상을 선택해 주세요.` | `연결할 대상을 선택해 주세요.` | no |
| `Ui.DocumentRegistration.Validation.SelectTargetKind` | `저장할 대상 유형을 선택해 주세요.` | `연결 대상 유형을 선택해 주세요.` | no |

Candidate validation summary:

- Exact rows: 6
- Duplicate keys: 0
- Missing required keys: 0
- `Implementation target now = yes`: 0
- Raw `policy`/`claim` English visible candidate values: 0
- New key candidates: 0

## C. Candidate Value Principles

- Align with the approved ProductShell terms `보험 계약`, `청구 건`, and `연결 대상`.
- Remove `활성` from empty-state copy because the user does not need internal active-state terminology in these messages.
- Keep technical `policy` and `claim` values out of visible copy.
- Identify the actual object that must be selected before registration.
- Preserve the existing punctuation and `해 주세요.` convention.
- Keep existing resource key names and `UiTextKeys` constants unchanged.
- Do not create product-specific runtime message keys.

## D. Future Exact Implementation Candidate

Modified:

- `app/FamilyClaimRef.App/Resources/UiStrings.xaml`
- `tests/FamilyClaimRef.App.Tests/DocumentRegistrationViewModelTests.cs`
- `tests/FamilyClaimRef.App.Tests/Services/Localization/ResourceUiTextProviderTests.cs`

Created:

- `docs/369_POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1B2_TARGET_RUNTIME_TERMINOLOGY_IMPLEMENTATION_RESULT_REVIEW.md`

Candidate counts:

| Item | Count |
|---|---:|
| future modified files | 3 |
| future result documents | 1 |
| future total files | 4 |

This exact list is a candidate only. It is not approved by this batch.

## E. Excluded Future Files

- `app/FamilyClaimRef.App/Services/Localization/UiTextKeys.cs`
- `app/FamilyClaimRef.App/ViewModels/DocumentRegistrationViewModel.cs`
- `app/FamilyClaimRef.App/Views/ProductDocumentRegistrationView.xaml`
- `app/FamilyClaimRef.App/ViewModels/ProductShellViewModel.cs`
- `app/FamilyClaimRef.App/ProductShell/ProductShellWindow.xaml`
- `app/FamilyClaimRef.App/Composition/AppServices.cs`
- MainWindow and App startup files
- project and solution files

## F. Count Contract

| Count item | Current | Future candidate |
|---|---:|---:|
| `Ui.*` resources/constants | 67/67 | 67/67 |
| `Ui.Product.*` resources/constants | 11/11 | 11/11 |
| new keys | 0 | 0 |
| deleted keys | 0 | 0 |
| renamed keys | 0 | 0 |
| target-specific changed values | 0 | exactly 6 |
| generic runtime-message changes | 0 | 0 |

## G. Test Impact Candidate

- Update existing exact resource-value assertions for the six candidate values.
- Update existing `DocumentRegistrationViewModel` expected-message assertions and its test resource dictionary.
- Add no new test class.
- Delete or weaken no existing test.
- Expected full test count: 357, unless implementation evidence explains a change.

## H. Replacement Boundary

The six old values are not intended to remain the final ProductShell runtime terminology if Candidate A is later approved. They would be replaced by the six candidate values. This batch does not perform that replacement and leaves all current source values unchanged.
