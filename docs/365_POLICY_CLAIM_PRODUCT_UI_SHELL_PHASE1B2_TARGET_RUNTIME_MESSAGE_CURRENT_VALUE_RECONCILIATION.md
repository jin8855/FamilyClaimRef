# Product UI Shell Phase 1B2 Target Runtime Message Current Value Reconciliation

## A. Status

- Marker: `POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1B2_TARGET_RUNTIME_MESSAGE_CURRENT_VALUE_RECONCILIATION_READY`
- Baseline: `e269e7469f0e462e2b00d88ae468a88fa40833a1`
- Source audit result: PASS
- Six required keys present: 6/6
- Unresolved source evidence: none

## B. Current Six-Key Inventory

| Resource key | Current source value | Source owner |
|---|---|---|
| `Ui.DocumentRegistration.Message.NoActiveClaim` | `선택할 수 있는 활성 청구 대상이 없습니다.` | `UiStrings.xaml` |
| `Ui.DocumentRegistration.Message.NoActivePolicy` | `선택할 수 있는 활성 보험 대상이 없습니다.` | `UiStrings.xaml` |
| `Ui.DocumentRegistration.Validation.SelectClaimBeforeRegister` | `문서를 등록하기 전에 청구 대상을 선택해 주세요.` | `UiStrings.xaml` |
| `Ui.DocumentRegistration.Validation.SelectPolicyBeforeRegister` | `문서를 등록하기 전에 보험 대상을 선택해 주세요.` | `UiStrings.xaml` |
| `Ui.DocumentRegistration.Validation.SelectTarget` | `저장할 대상을 선택해 주세요.` | `UiStrings.xaml` |
| `Ui.DocumentRegistration.Validation.SelectTargetKind` | `저장할 대상 유형을 선택해 주세요.` | `UiStrings.xaml` |

The actual source values match the expected current values. No value was inferred from an older document in place of source evidence.

## C. Constant and ViewModel Lookup Mapping

| Resource key | `UiTextKeys` constant | `DocumentRegistrationViewModel` use |
|---|---|---|
| `Ui.DocumentRegistration.Message.NoActiveClaim` | `DocumentRegistrationMessageNoActiveClaim` | claim empty state and claim target selection message |
| `Ui.DocumentRegistration.Message.NoActivePolicy` | `DocumentRegistrationMessageNoActivePolicy` | policy empty state and policy target selection message |
| `Ui.DocumentRegistration.Validation.SelectClaimBeforeRegister` | `DocumentRegistrationValidationSelectClaimBeforeRegister` | missing selected claim before registration |
| `Ui.DocumentRegistration.Validation.SelectPolicyBeforeRegister` | `DocumentRegistrationValidationSelectPolicyBeforeRegister` | missing selected policy before registration |
| `Ui.DocumentRegistration.Validation.SelectTarget` | `DocumentRegistrationValidationSelectTarget` | missing target ID in generic registration validation |
| `Ui.DocumentRegistration.Validation.SelectTargetKind` | `DocumentRegistrationValidationSelectTargetKind` | unsupported or missing target kind validation |

All six messages are resolved through `IUiTextProvider`. No direct Korean literal is required in the ViewModel or product XAML.

## D. Display Paths and Shared Impact

ProductShell display path:

1. `ProductShellViewModel.DocumentRegistration` exposes the existing `DocumentRegistrationViewModel` instance.
2. `ProductDocumentRegistrationView` receives that instance as its DataContext.
3. The view displays `TargetSelectionMessage`, `ValidationMessage`, and `StatusMessage`.
4. The six keys above can therefore reach the compile-only ProductShell registration content without a product-specific ViewModel.

MainWindow validation-harness impact:

- The validation harness uses the same resource provider and existing `DocumentRegistrationViewModel` message ownership.
- A shared resource value change will affect both the ProductShell registration view and the harness.
- This shared value effect does not replace, redesign, or productize the MainWindow validation harness.

## E. Terminology Conflict

The committed ProductShell static copy uses:

- `보험 계약`
- `청구 건`
- `연결 대상`

The current target-specific runtime messages still use:

- `활성 보험 대상`
- `활성 청구 대상`
- `보험 대상`
- `청구 대상`
- `저장할 대상`

This mismatch is acceptable only for the existing compile-only compatibility exception. It remains a blocker before ProductShell runtime entry.

## F. Strategy Comparison

| Candidate | Scope | Advantages | Costs and risks | Judgment |
|---|---|---|---|---|
| Candidate A: change six existing shared values | `UiStrings.xaml` values and existing test expectations | no new keys, no production code change, one converged vocabulary for ProductShell and harness | shared copy changes in the harness too | recommended candidate |
| Candidate B: add six `Ui.Product.DocumentRegistration.*` runtime keys | resources, constants, message ownership, ViewModel/provider selection, tests | ProductShell and harness copy can diverge | adds context-dependent ownership and unnecessary Phase 1B2 complexity | not recommended |
| Candidate C: keep compatibility exception | no source change | compile-only state remains stable | runtime-entry blocker remains unresolved | defer only, not convergence |

## G. Recommendation

- Selected recommendation candidate: Candidate A.
- Change only the values of the six existing shared runtime resource keys in a separately approved implementation batch.
- New runtime resource keys: not required.
- `UiTextKeys.cs` modification: not required.
- `DocumentRegistrationViewModel.cs` modification: not required.
- Product registration XAML or ProductShell source modification: not required.
- Current batch implementation approval: no.

## H. Remaining Boundary

- Recommended values are recorded in docs/366 but are not approved for implementation now.
- ProductShell runtime entry remains blocked until the six-value implementation is separately approved, completed, and validated.
- No source, resource, test, or project file is modified in this batch.
