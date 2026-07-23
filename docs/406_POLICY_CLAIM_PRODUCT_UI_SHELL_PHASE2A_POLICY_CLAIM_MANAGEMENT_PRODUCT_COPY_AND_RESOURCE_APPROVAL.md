# Product UI Shell Phase 2A Policy Claim Management Product Copy And Resource Approval

## A. Marker

`POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE2A_POLICY_CLAIM_MANAGEMENT_PRODUCT_COPY_AND_RESOURCE_APPROVAL_READY`

## B. Terminology

Approved product terms:

- Policy: `보험 계약`
- Claim: `청구 건`
- Target: `연결 대상`
- Status/result: `처리 결과`

Raw identifiers, paths, diagnostics, policy numbers, claim numbers, and real personal data are not product copy.

## C. Runtime Message Strategy

Selected strategy: `A`.

- Keep the existing ten runtime-message keys.
- Change their values from validation-harness English to approved Korean.
- Do not add product wrapper projection.
- Add only the new safe-error and duplicate-validation keys that have no existing semantic equivalent.
- Mirror runtime-consumed values in the `AppServices` fallback dictionary.
- MainWindow will receive the same Korean values; behavior and bindings remain unchanged.

Rejected:

- Strategy B, duplicate product-specific versions of all ten runtime keys.
- Strategy C, wrapper-projected product messages.

Reason:

- The existing keys already describe domain outcomes, not layout-specific copy.
- One approved value per outcome prevents resource drift.
- B2 does not introduce wrappers.

## D. Approved Product Static Keys

All 18 candidates from `docs/401` are approved.

| Exact key | Exact Korean value | Screen owner | Classification | Approved |
|---|---|---|---|---|
| `Ui.Product.Navigation.PolicyContracts` | `보험 계약` | ProductShell navigation | new | yes |
| `Ui.Product.Navigation.ClaimCases` | `청구 건` | ProductShell navigation | new | yes |
| `Ui.Product.PolicyContracts.Title` | `보험 계약` | policy screen | new | yes |
| `Ui.Product.ClaimCases.Title` | `청구 건` | claim screen | new | yes |
| `Ui.Product.PolicyContracts.EmptyMessage` | `등록된 보험 계약이 없습니다.` | policy screen | new | yes |
| `Ui.Product.ClaimCases.EmptyMessage` | `등록된 청구 건이 없습니다.` | claim screen | new | yes |
| `Ui.Product.PolicyContracts.CreationSection` | `보험 계약 등록` | policy screen | new | yes |
| `Ui.Product.ClaimCases.CreationSection` | `청구 건 등록` | claim screen | new | yes |
| `Ui.Product.PolicyContracts.ActiveListLabel` | `보험 계약 목록` | policy screen | new | yes |
| `Ui.Product.ClaimCases.ActiveListLabel` | `청구 건 목록` | claim screen | new | yes |
| `Ui.Product.PolicyContracts.DisplayTitleLabel` | `보험 계약 이름` | policy screen | new | yes |
| `Ui.Product.ClaimCases.DisplayTitleLabel` | `청구 건 이름` | claim screen | new | yes |
| `Ui.Product.ClaimCases.PolicyLabel` | `보험 계약` | claim screen | new | yes |
| `Ui.Product.PolicyContracts.CreateAction` | `보험 계약 등록` | policy screen | new | yes |
| `Ui.Product.PolicyContracts.DisableAction` | `보험 계약 사용 중지` | policy screen | new | yes |
| `Ui.Product.ClaimCases.CreateAction` | `청구 건 등록` | claim screen | new | yes |
| `Ui.Product.ClaimCases.DisableAction` | `청구 건 사용 중지` | claim screen | new | yes |
| `Ui.Product.Management.StatusLabel` | `처리 결과` | both management screens | new | yes |

## E. Approved Product Error And Duplicate Keys

| Exact key | Exact Korean value | Owner | Classification | Approved |
|---|---|---|---|---|
| `Ui.Product.Management.LoadFailedMessage` | `목록을 불러오지 못했습니다. 다시 시도해 주세요.` | core management load boundary | new | yes |
| `Ui.Product.PolicyContracts.OperationFailedMessage` | `보험 계약을 처리하지 못했습니다. 다시 시도해 주세요.` | core policy mutation boundary | new | yes |
| `Ui.Product.ClaimCases.OperationFailedMessage` | `청구 건을 처리하지 못했습니다. 다시 시도해 주세요.` | core claim mutation boundary | new | yes |
| `Ui.Product.PolicyContracts.DuplicateTitleMessage` | `같은 이름의 활성 보험 계약이 이미 있습니다.` | policy duplicate validation | new | yes |
| `Ui.Product.ClaimCases.DuplicateTitleMessage` | `같은 이름의 활성 청구 건이 이미 있습니다.` | claim duplicate validation | new | yes |

These messages intentionally omit diagnostic details and internal state.

## F. Approved Existing Runtime Value Changes

| Existing exact key | Current value | Approved Korean value | Consumers | Approved |
|---|---|---|---|---|
| `Ui.ClaimManagement.Message.Created` | `Claim target was created.` | `청구 건을 등록했습니다.` | core ViewModel, MainWindow, product claim screen | yes |
| `Ui.ClaimManagement.Message.Disabled` | `Claim target was disabled.` | `청구 건을 사용 중지했습니다.` | core ViewModel, MainWindow, product claim screen | yes |
| `Ui.ClaimManagement.Validation.TitleRequired` | `Claim target title is required.` | `청구 건 이름을 입력해 주세요.` | core ViewModel, MainWindow, product claim screen | yes |
| `Ui.PolicyManagement.Message.Created` | `Policy target was created.` | `보험 계약을 등록했습니다.` | core ViewModel, MainWindow, product policy screen | yes |
| `Ui.PolicyManagement.Message.Disabled` | `Policy target was disabled.` | `보험 계약을 사용 중지했습니다.` | core ViewModel, MainWindow, product policy screen | yes |
| `Ui.PolicyManagement.Validation.DisableBlockedByActiveClaims` | `Policy target has active claim targets. Disable claim targets first.` | `활성 청구 건이 있어 보험 계약을 사용 중지할 수 없습니다. 청구 건을 먼저 사용 중지해 주세요.` | core ViewModel, MainWindow, product policy screen | yes |
| `Ui.ClaimManagement.Validation.SelectPolicyBeforeCreate` | `Select an active policy target before creating a claim target.` | `청구 건을 등록할 보험 계약을 선택해 주세요.` | core ViewModel, MainWindow, product claim screen | yes |
| `Ui.PolicyManagement.Validation.TitleRequired` | `Policy target title is required.` | `보험 계약 이름을 입력해 주세요.` | core ViewModel, MainWindow, product policy screen | yes |
| `Ui.ClaimManagement.Validation.SelectClaimTarget` | `Select a claim target.` | `사용 중지할 청구 건을 선택해 주세요.` | core ViewModel, MainWindow, product claim screen | yes |
| `Ui.PolicyManagement.Validation.SelectPolicyTarget` | `Select a policy target.` | `사용 중지할 보험 계약을 선택해 주세요.` | core ViewModel, MainWindow, product policy screen | yes |

## G. Count Contract

Current:

- Resource keys: `68`
- Constants: `68`
- `Ui.Product.*` resource keys: `12`
- `Ui.Product.*` constants: `12`

Approved future delta:

- New product keys: `23`
  - static candidates: `18`
  - safe errors: `3`
  - duplicate validations: `2`
- Existing value changes: `10`
- New shared non-product keys: `0`

Expected after implementation:

- Resource keys/constants: `91/91`
- `Ui.Product.*` resource keys/constants: `35/35`
- Duplicate resource keys: `0`
- Duplicate constants: `0`
- Missing resource/constant pairs: `0`

## H. Resource Ownership

- `UiStrings.xaml` is the primary exact value source.
- `UiTextKeys.cs` exposes one constant for each new exact key.
- `ResourceUiTextProvider` remains unchanged unless compilation proves otherwise; no change is authorized by this contract.
- `AppServices.CreateUiTextProvider` fallback values must:
  - update the existing ten runtime values;
  - include the five new runtime-consumed error/duplicate values;
  - include the two new navigation values;
  - avoid adding fallback entries for XAML-only static labels unless required by source use.

## I. MainWindow Impact

- MainWindow gets Korean text for the existing ten shared outcomes.
- No MainWindow XAML or ViewModel change is required.
- No validation rule or persistence behavior is changed by the copy update.
- Existing resource injection tests must update expected values.
- Static validation-harness English labels outside the ten runtime messages remain unchanged and out of scope.

## J. Privacy And Error Copy Rule

Product messages must never concatenate:

- `Exception.Message`;
- local/runtime path;
- internal record ID;
- JSON text;
- stack trace;
- exception type;
- actual personal, insurance, hospital, diagnosis, policy-number, or claim-number sample.

## K. Decision

- Static product candidates approved: `18/18`.
- New safe-error keys approved: `3/3`.
- New duplicate-validation keys approved: `2/2`.
- Existing runtime value changes approved: `10/10`.
- Copy/resource blocker: `0`.
- Resource implementation started in this batch: `no`.
