# Policy Claim ViewModel Runtime Message Resource Key Plan

## A. Status

```text
RESOURCE_KEY_PLAN_ONLY
POLICY_CLAIM_VIEWMODEL_RUNTIME_MESSAGE_RESOURCE_KEYS_PLANNED
```

이 문서는 ViewModel runtime message extraction을 위한 resource key 후보를 계획한다. 실제 key, resource value, constant는 추가하지 않는다.

## B. Key rule

- keys describe message purpose, not current English text
- values preserve the current source literal/current behavior for first implementation
- existing Korean source literals may be retained as first implementation values when they already exist in source
- existing English source literals remain English for first implementation
- direct Korean replacement is not allowed
- no new Korean translation is introduced by this plan
- final Korean copy is not decided
- formatting placeholders must remain explicit and tested
- static XAML keys are not duplicated

## C. Planned key table

| Source ViewModel | Current literal | Planned key | Placeholder contract | Ownership | First implementation value | Notes |
|---|---|---|---|---|---|---|
| `DocumentRegistrationViewModel` | `등록 중 일부 정리가 실패했습니다. 다시 시도하거나 관리자에게 문의하세요.` | `Ui.DocumentRegistration.Status.CleanupFailed` | none | product-facing candidate | `등록 중 일부 정리가 실패했습니다. 다시 시도하거나 관리자에게 문의하세요.` | Existing Korean source literal retained for first implementation. |
| `DocumentRegistrationViewModel` | `No active claim is available for selection.` | `Ui.DocumentRegistration.Message.NoActiveClaim` | none | product-facing candidate | `No active claim is available for selection.` | Target selection empty state. |
| `DocumentRegistrationViewModel` | `No active policy is available for selection.` | `Ui.DocumentRegistration.Message.NoActivePolicy` | none | product-facing candidate | `No active policy is available for selection.` | Target selection empty state. |
| `DocumentRegistrationViewModel` | `문서 등록에 실패했습니다.` | `Ui.DocumentRegistration.Status.Failed` | none | product-facing candidate | `문서 등록에 실패했습니다.` | Existing Korean source literal retained for first implementation. |
| `DocumentRegistrationViewModel` | `문서 등록이 완료되었습니다.` | `Ui.DocumentRegistration.Status.Completed` | none | product-facing candidate | `문서 등록이 완료되었습니다.` | Existing Korean source literal retained for first implementation. |
| `DocumentRegistrationViewModel` | `Select a claim before registering this document.` | `Ui.DocumentRegistration.Validation.SelectClaimBeforeRegister` | none | product-facing candidate | `Select a claim before registering this document.` | Claim target validation. |
| `DocumentRegistrationViewModel` | `Select a policy before registering this document.` | `Ui.DocumentRegistration.Validation.SelectPolicyBeforeRegister` | none | product-facing candidate | `Select a policy before registering this document.` | Policy target validation. |
| `DocumentRegistrationViewModel` | `파일을 선택했습니다.` | `Ui.DocumentRegistration.Status.FileSelected` | none | product-facing candidate | `파일을 선택했습니다.` | Existing Korean source literal retained for first implementation. |
| `DocumentRegistrationViewModel` | `파일을 선택해 주세요.` | `Ui.DocumentRegistration.Validation.SelectFile` | none | product-facing candidate | `파일을 선택해 주세요.` | Existing Korean source literal retained for first implementation. |
| `DocumentRegistrationViewModel` | `저장할 대상 유형을 선택해 주세요.` | `Ui.DocumentRegistration.Validation.SelectTargetKind` | none | product-facing candidate | `저장할 대상 유형을 선택해 주세요.` | Existing Korean source literal retained for first implementation. |
| `DocumentRegistrationViewModel` | `저장할 대상을 입력해 주세요.` | `Ui.DocumentRegistration.Validation.SelectTarget` | none | product-facing candidate | `저장할 대상을 입력해 주세요.` | Existing Korean source literal retained for first implementation. |
| `DocumentRegistrationViewModel` | `문서 유형을 선택해 주세요.` | `Ui.DocumentRegistration.Validation.SelectDocumentType` | none | product-facing candidate | `문서 유형을 선택해 주세요.` | Existing Korean source literal retained for first implementation. |
| `DocumentRegistrationViewModel` | `표시 제목을 입력해 주세요.` | `Ui.DocumentRegistration.Validation.EnterDisplayTitle` | none | product-facing candidate | `표시 제목을 입력해 주세요.` | Existing Korean source literal retained for first implementation. |
| `DocumentRegistrationViewModel` | `기준일을 선택해 주세요.` | `Ui.DocumentRegistration.Validation.SelectReferenceDate` | none | product-facing candidate | `기준일을 선택해 주세요.` | Existing Korean source literal retained for first implementation. |
| `PolicyClaimManagementViewModel` | `Claim target was created.` | `Ui.ClaimManagement.Message.Created` | none | validation-harness-only | `Claim target was created.` | Synthetic target management feedback. |
| `PolicyClaimManagementViewModel` | `Claim target was disabled.` | `Ui.ClaimManagement.Message.Disabled` | none | validation-harness-only | `Claim target was disabled.` | Synthetic target management feedback. |
| `PolicyClaimManagementViewModel` | `Claim target title is required.` | `Ui.ClaimManagement.Validation.TitleRequired` | none | validation-harness-only | `Claim target title is required.` | Dev target management guard. |
| `PolicyClaimManagementViewModel` | `Policy target was created.` | `Ui.PolicyManagement.Message.Created` | none | validation-harness-only | `Policy target was created.` | Synthetic target management feedback. |
| `PolicyClaimManagementViewModel` | `Policy target was disabled.` | `Ui.PolicyManagement.Message.Disabled` | none | validation-harness-only | `Policy target was disabled.` | Synthetic target management feedback. |
| `PolicyClaimManagementViewModel` | `Policy target has active claim targets. Disable claim targets first.` | `Ui.PolicyManagement.Validation.DisableBlockedByActiveClaims` | none | validation-harness-only | `Policy target has active claim targets. Disable claim targets first.` | Product copy may differ later. |
| `PolicyClaimManagementViewModel` | `Select an active policy target before creating a claim target.` | `Ui.ClaimManagement.Validation.SelectPolicyBeforeCreate` | none | validation-harness-only | `Select an active policy target before creating a claim target.` | Dev target management guard. |
| `PolicyClaimManagementViewModel` | `Policy target title is required.` | `Ui.PolicyManagement.Validation.TitleRequired` | none | validation-harness-only | `Policy target title is required.` | Dev target management guard. |
| `PolicyClaimManagementViewModel` | `Select a claim target.` | `Ui.ClaimManagement.Validation.SelectClaimTarget` | none | validation-harness-only | `Select a claim target.` | Dev target management guard. |
| `PolicyClaimManagementViewModel` | `Select a policy target.` | `Ui.PolicyManagement.Validation.SelectPolicyTarget` | none | validation-harness-only | `Select a policy target.` | Dev target management guard. |

## D. Deferred key candidates

| Source ViewModel | Current literal shape | Decision | Reason |
|---|---|---|---|
| `DocumentRegistrationViewModel` | `policy:{policyId}; document:{documentId}` | Defer | Diagnostic summary format; final display model may change. |
| `DocumentRegistrationViewModel` | `claim:{claimId}; document:{documentId}` | Defer | Diagnostic summary format; final display model may change. |

## E. Keys not included

- `Ui.BusinessDuplicate.*`
- `Ui.Product.*`
- final Korean copy keys
- culture switching keys
- static XAML duplicate keys
- DB/OCR/repository message keys
- `MainWindow.xaml` `policy` / `claim` ComboBox value keys
- `MainWindow.xaml` `StringFormat=Is busy: {0}` key
- `Ui.ActionResult.*`, because source ownership is not yet clear enough to use a shared family

## F. Runtime key family state

- `UiTextKeys.cs` currently has no `Ui.DocumentRegistration.*` runtime message key family.
- `UiTextKeys.cs` currently has no `Ui.PolicyManagement.*` runtime message key family.
- `UiTextKeys.cs` currently has no `Ui.ClaimManagement.*` runtime message key family.
- `UiStrings.xaml` currently has no runtime message key family for these ViewModel messages.
- This document does not add any key.

## G. 판단 marker

```text
POLICY_CLAIM_VIEWMODEL_RUNTIME_MESSAGE_RESOURCE_KEYS_READY
```
