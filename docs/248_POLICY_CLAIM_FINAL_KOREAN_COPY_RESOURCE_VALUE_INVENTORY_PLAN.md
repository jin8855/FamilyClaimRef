# Policy Claim Final Korean Copy Resource Value Inventory Plan

## A. Status

```text
RESOURCE_VALUE_INVENTORY_PLAN_ONLY
POLICY_CLAIM_FINAL_KOREAN_COPY_RESOURCE_VALUE_INVENTORY_PLANNED
```

## B. Baseline

```text
a8a2407 refactor(familyclaimref): extract viewmodel runtime messages
```

## C. Inventory rule

- current resource value를 그대로 기록한다.
- 새 Korean copy를 쓰지 않는다.
- final Korean copy를 확정하지 않는다.
- current Korean value는 existing source literal인지 표시한다.
- current English value는 later copy decision 필요 여부를 표시한다.
- harness-only value는 product copy로 승격하지 않는다.
- product-facing candidate와 validation-harness-only ownership을 분리한다.

## D. Count summary

- expected static XAML keys: 32
- expected runtime message keys: 24
- expected total `Ui.*` keys: 56
- verified `UiStrings.xaml` `Ui.*` keys: 56
- verified `UiTextKeys.cs` `Ui.*` constants: 56
- discrepancy:
  - none

## E. Inventory table

| Category | Resource key | Current value | Current language | Ownership | Copy state | Future decision |
|---|---|---|---|---|---|---|
| Pilot static | `Ui.App.Title` | `FamilyClaimRef` | English | infrastructure/app-shell | current value retained | review ownership first |
| Pilot static | `Ui.Document.SourceFileSection` | `Source file` | English | product-facing candidate | needs Korean copy decision later | review for final Korean product copy |
| Document registration static | `Ui.Document.SelectedFileLabel` | `Selected file` | English | product-facing candidate | needs Korean copy decision later | review for final Korean product copy |
| Document registration static | `Ui.Document.MetadataSection` | `Document metadata` | English | product-facing candidate | needs Korean copy decision later | review for final Korean product copy |
| Document registration static | `Ui.Document.TypeLabel` | `Document type` | English | product-facing candidate | needs Korean copy decision later | review for final Korean product copy |
| Document registration static | `Ui.Document.DisplayTitleLabel` | `Display title` | English | product-facing candidate | needs Korean copy decision later | review for final Korean product copy |
| Document registration static | `Ui.Document.ReferenceDateLabel` | `Reference date` | English | product-facing candidate | needs Korean copy decision later | review for final Korean product copy |
| Document registration static | `Ui.Target.SelectionSection` | `Target selection` | English | product-facing candidate | needs Korean copy decision later | review for final Korean product copy |
| Document registration static | `Ui.Target.KindLabel` | `Target kind` | English | product-facing candidate | needs Korean copy decision later | review for final Korean product copy |
| Document registration static | `Ui.Policy.TargetLabel` | `Policy target` | English | product-facing candidate | needs Korean copy decision later | review for final Korean product copy |
| Management static | `Ui.Policy.ActiveTargetsLabel` | `Active policy targets` | English | validation-harness-only | harness-only, do not productize yet | defer until product UI shell |
| Management static | `Ui.Policy.NewTitleLabel` | `New policy title` | English | validation-harness-only | harness-only, do not productize yet | defer until product UI shell |
| Document registration static | `Ui.Claim.TargetLabel` | `Claim target` | English | product-facing candidate | needs Korean copy decision later | review for final Korean product copy |
| Management static | `Ui.Claim.PolicyForNewClaimLabel` | `Policy for new claim` | English | validation-harness-only | harness-only, do not productize yet | defer until product UI shell |
| Management static | `Ui.Claim.ActiveTargetsLabel` | `Active claim targets` | English | validation-harness-only | harness-only, do not productize yet | defer until product UI shell |
| Management static | `Ui.Claim.NewTitleLabel` | `New claim title` | English | validation-harness-only | harness-only, do not productize yet | defer until product UI shell |
| Management static | `Ui.Management.PolicyClaimSection` | `Policy/Claim Management` | English | validation-harness-only | harness-only, do not productize yet | defer until product UI shell |
| Management static | `Ui.Management.PolicySection` | `Policy Management` | English | validation-harness-only | harness-only, do not productize yet | defer until product UI shell |
| Management static | `Ui.Management.ClaimSection` | `Claim Management` | English | validation-harness-only | harness-only, do not productize yet | defer until product UI shell |
| Management static | `Ui.Management.MessageLabel` | `Management message` | English | validation-harness-only | harness-only, do not productize yet | defer until product UI shell |
| Pilot static | `Ui.Action.SelectFile` | `Select file` | English | product-facing candidate | needs Korean copy decision later | review for final Korean product copy |
| Document registration static | `Ui.Action.RegisterDocument` | `Register` | English | product-facing candidate | needs Korean copy decision later | review for final Korean product copy |
| Management static | `Ui.Action.CreatePolicy` | `Create policy` | English | validation-harness-only | harness-only, do not productize yet | defer until product UI shell |
| Management static | `Ui.Action.DisablePolicy` | `Disable policy` | English | validation-harness-only | harness-only, do not productize yet | defer until product UI shell |
| Management static | `Ui.Action.CreateClaim` | `Create claim` | English | validation-harness-only | harness-only, do not productize yet | defer until product UI shell |
| Management static | `Ui.Action.DisableClaim` | `Disable claim` | English | validation-harness-only | harness-only, do not productize yet | defer until product UI shell |
| Document registration static | `Ui.Validation.SectionLabel` | `Validation` | English | product-facing candidate | needs Korean copy decision later | review for final Korean product copy |
| Pilot static | `Ui.Status.RegistrationSection` | `Registration status` | English | product-facing candidate | needs Korean copy decision later | review for final Korean product copy |
| Document registration static | `Ui.Status.Label` | `Status` | English | product-facing candidate | needs Korean copy decision later | review for final Korean product copy |
| Document registration static | `Ui.Status.LastRegistrationSummaryLabel` | `Last registration summary` | English | product-facing candidate | needs Korean copy decision later | review for final Korean product copy |
| Pilot static | `Ui.DevHarness.Warning.LocalMvpValidation` | `Local MVP validation screen. Do not use real personal, insurer, hospital, diagnosis, policy number, or claim number samples.` | English | dev-harness-only | harness-only, do not productize yet | exclude from final copy batch |
| Management static | `Ui.DevHarness.ManagementWarning` | `Create and disable local policy/claim targets with synthetic-safe titles only.` | English | dev-harness-only | harness-only, do not productize yet | exclude from final copy batch |
| DocumentRegistration runtime | `Ui.DocumentRegistration.Status.CleanupFailed` | `등록 중 일부 정리가 실패했습니다. 다시 시도하거나 관리자에게 문의하세요.` | Korean | product-facing candidate | existing Korean source literal retained | review for final Korean product copy |
| DocumentRegistration runtime | `Ui.DocumentRegistration.Message.NoActiveClaim` | `No active claim is available for selection.` | English | product-facing candidate | needs Korean copy decision later | review for final Korean product copy |
| DocumentRegistration runtime | `Ui.DocumentRegistration.Message.NoActivePolicy` | `No active policy is available for selection.` | English | product-facing candidate | needs Korean copy decision later | review for final Korean product copy |
| DocumentRegistration runtime | `Ui.DocumentRegistration.Status.Failed` | `문서 등록에 실패했습니다.` | Korean | product-facing candidate | existing Korean source literal retained | review for final Korean product copy |
| DocumentRegistration runtime | `Ui.DocumentRegistration.Status.Completed` | `문서 등록이 완료되었습니다.` | Korean | product-facing candidate | existing Korean source literal retained | review for final Korean product copy |
| DocumentRegistration runtime | `Ui.DocumentRegistration.Validation.SelectClaimBeforeRegister` | `Select a claim before registering this document.` | English | product-facing candidate | needs Korean copy decision later | review for final Korean product copy |
| DocumentRegistration runtime | `Ui.DocumentRegistration.Validation.SelectPolicyBeforeRegister` | `Select a policy before registering this document.` | English | product-facing candidate | needs Korean copy decision later | review for final Korean product copy |
| DocumentRegistration runtime | `Ui.DocumentRegistration.Status.FileSelected` | `파일을 선택했습니다.` | Korean | product-facing candidate | existing Korean source literal retained | review for final Korean product copy |
| DocumentRegistration runtime | `Ui.DocumentRegistration.Validation.SelectFile` | `파일을 선택해 주세요.` | Korean | product-facing candidate | existing Korean source literal retained | review for final Korean product copy |
| DocumentRegistration runtime | `Ui.DocumentRegistration.Validation.SelectTargetKind` | `저장할 대상 유형을 선택해 주세요.` | Korean | product-facing candidate | existing Korean source literal retained | review for final Korean product copy |
| DocumentRegistration runtime | `Ui.DocumentRegistration.Validation.SelectTarget` | `저장할 대상을 입력해 주세요.` | Korean | product-facing candidate | existing Korean source literal retained | review for final Korean product copy |
| DocumentRegistration runtime | `Ui.DocumentRegistration.Validation.SelectDocumentType` | `문서 유형을 선택해 주세요.` | Korean | product-facing candidate | existing Korean source literal retained | review for final Korean product copy |
| DocumentRegistration runtime | `Ui.DocumentRegistration.Validation.EnterDisplayTitle` | `표시 제목을 입력해 주세요.` | Korean | product-facing candidate | existing Korean source literal retained | review for final Korean product copy |
| DocumentRegistration runtime | `Ui.DocumentRegistration.Validation.SelectReferenceDate` | `기준일을 선택해 주세요.` | Korean | product-facing candidate | existing Korean source literal retained | review for final Korean product copy |
| PolicyClaimManagement runtime | `Ui.ClaimManagement.Message.Created` | `Claim target was created.` | English | validation-harness-only | harness-only, do not productize yet | defer until product UI shell |
| PolicyClaimManagement runtime | `Ui.ClaimManagement.Message.Disabled` | `Claim target was disabled.` | English | validation-harness-only | harness-only, do not productize yet | defer until product UI shell |
| PolicyClaimManagement runtime | `Ui.ClaimManagement.Validation.TitleRequired` | `Claim target title is required.` | English | validation-harness-only | harness-only, do not productize yet | defer until product UI shell |
| PolicyClaimManagement runtime | `Ui.PolicyManagement.Message.Created` | `Policy target was created.` | English | validation-harness-only | harness-only, do not productize yet | defer until product UI shell |
| PolicyClaimManagement runtime | `Ui.PolicyManagement.Message.Disabled` | `Policy target was disabled.` | English | validation-harness-only | harness-only, do not productize yet | defer until product UI shell |
| PolicyClaimManagement runtime | `Ui.PolicyManagement.Validation.DisableBlockedByActiveClaims` | `Policy target has active claim targets. Disable claim targets first.` | English | validation-harness-only | harness-only, do not productize yet | defer until product UI shell |
| PolicyClaimManagement runtime | `Ui.ClaimManagement.Validation.SelectPolicyBeforeCreate` | `Select an active policy target before creating a claim target.` | English | validation-harness-only | harness-only, do not productize yet | defer until product UI shell |
| PolicyClaimManagement runtime | `Ui.PolicyManagement.Validation.TitleRequired` | `Policy target title is required.` | English | validation-harness-only | harness-only, do not productize yet | defer until product UI shell |
| PolicyClaimManagement runtime | `Ui.ClaimManagement.Validation.SelectClaimTarget` | `Select a claim target.` | English | validation-harness-only | harness-only, do not productize yet | defer until product UI shell |
| PolicyClaimManagement runtime | `Ui.PolicyManagement.Validation.SelectPolicyTarget` | `Select a policy target.` | English | validation-harness-only | harness-only, do not productize yet | defer until product UI shell |

## F. Deferred / non-resource items

| Deferred item | Current location | Reason | Future handling |
|---|---|---|---|
| MainWindow.xaml `policy` ComboBox value | `app/FamilyClaimRef.App/MainWindow.xaml` | target kind value, not final copy | decide with product UI shell |
| MainWindow.xaml `claim` ComboBox value | `app/FamilyClaimRef.App/MainWindow.xaml` | target kind value, not final copy | decide with product UI shell |
| MainWindow.xaml `StringFormat=Is busy: {0}` | `app/FamilyClaimRef.App/MainWindow.xaml` | diagnostic harness display | defer until product UI shell |
| `policy:{policyId}; document:{documentId}` | `DocumentRegistrationViewModel.CreatePolicySummary` | diagnostic summary format | defer until final display model |
| `claim:{claimId}; document:{documentId}` | `DocumentRegistrationViewModel.CreateClaimSummary` | diagnostic summary format | defer until final display model |
| `Ui.BusinessDuplicate.*` | not implemented | business duplicate rule/copy not approved | separate decision required |
| `Ui.Product.*` | not implemented | product UI shell not approved | separate decision required |
| `Ui.ActionResult.*` | not implemented | shared action result ownership unclear | separate decision required |

## G. 판단 marker

```text
POLICY_CLAIM_FINAL_KOREAN_COPY_RESOURCE_VALUE_INVENTORY_READY
```
