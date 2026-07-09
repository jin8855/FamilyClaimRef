# Policy Claim Final Korean Copy Candidate Table

## A. Status

```text
FINAL_KOREAN_COPY_CANDIDATE_TABLE_ONLY
POLICY_CLAIM_FINAL_KOREAN_COPY_CANDIDATE_TABLE_PLANNED
```

## B. Baseline

```text
01aeffe docs(familyclaimref): plan final korean copy strategy
```

## C. Table Status

- candidate only
- pending user approval
- not approved for implementation
- not reflected in `UiStrings.xaml`
- not reflected in source/test files

## D. Table Rule

- current resource value는 현재 source/resource 기준 그대로 기록한다.
- candidate Korean copy는 docs 안의 proposal일 뿐이다.
- candidate Korean copy는 approved final copy로 취급하지 않는다.
- candidate Korean copy를 resource value로 반영하지 않는다.
- existing Korean source literal은 새 Korean translation으로 취급하지 않는다.
- English current value의 candidate Korean copy는 사용자 승인 전까지 final이 아니다.
- harness-only value는 product copy 후보표에서 제외한다.
- key name은 변경하지 않는다.

## E. Count Summary

| Count item | Expected | Actual | Discrepancy |
|---|---:|---:|---|
| Candidate/review rows | 31 | 31 | none |
| Excluded resource rows | 25 | 25 | none |
| Deferred/non-resource rows | 8 | 8 | none |
| Total `Ui.*` resource rows | 56 | 56 | none |

## F. Candidate Korean Copy Table

| Resource key | Current value | Current language | Ownership | Copy source state | Candidate Korean copy | Candidate status | Test impact | Notes |
|---|---|---|---|---|---|---|---|---|
| `Ui.App.Title` | `FamilyClaimRef` | English | infrastructure/app-shell | app-shell value retained | `FamilyClaimRef` | Keep current, pending user approval | No test update expected | app title retention candidate |
| `Ui.Document.SourceFileSection` | `Source file` | English | product-facing candidate | current English value | `원본 파일` | Candidate, pending user approval | ResourceUiTextProviderTests update likely | section label |
| `Ui.Document.SelectedFileLabel` | `Selected file` | English | product-facing candidate | current English value | `선택한 파일` | Candidate, pending user approval | ResourceUiTextProviderTests update likely | label |
| `Ui.Document.MetadataSection` | `Document metadata` | English | product-facing candidate | current English value | `문서 정보` | Candidate, pending user approval | ResourceUiTextProviderTests update likely | section label |
| `Ui.Document.TypeLabel` | `Document type` | English | product-facing candidate | current English value | `문서 유형` | Candidate, pending user approval | ResourceUiTextProviderTests update likely | label |
| `Ui.Document.DisplayTitleLabel` | `Display title` | English | product-facing candidate | current English value | `표시 제목` | Candidate, pending user approval | ResourceUiTextProviderTests update likely | label |
| `Ui.Document.ReferenceDateLabel` | `Reference date` | English | product-facing candidate | current English value | `기준일` | Candidate, pending user approval | ResourceUiTextProviderTests update likely | label |
| `Ui.Target.SelectionSection` | `Target selection` | English | product-facing candidate | current English value | `저장 대상 선택` | Candidate, pending user approval | ResourceUiTextProviderTests update likely | section label |
| `Ui.Target.KindLabel` | `Target kind` | English | product-facing candidate | current English value | `대상 유형` | Candidate, pending user approval | ResourceUiTextProviderTests update likely | label |
| `Ui.Policy.TargetLabel` | `Policy target` | English | product-facing candidate | current English value | `보험 대상` | Candidate, pending user approval | ResourceUiTextProviderTests update likely | generic policy target, not insurer name |
| `Ui.Claim.TargetLabel` | `Claim target` | English | product-facing candidate | current English value | `청구 대상` | Candidate, pending user approval | ResourceUiTextProviderTests update likely | generic claim target |
| `Ui.Action.SelectFile` | `Select file` | English | product-facing candidate | current English value | `파일 선택` | Candidate, pending user approval | ResourceUiTextProviderTests update likely | action label has current exact test |
| `Ui.Action.RegisterDocument` | `Register` | English | product-facing candidate | current English value | `등록` | Candidate, pending user approval | ResourceUiTextProviderTests update likely | action label |
| `Ui.Validation.SectionLabel` | `Validation` | English | product-facing candidate | current English value | `입력 확인` | Candidate, pending user approval | ResourceUiTextProviderTests update likely | section label |
| `Ui.Status.RegistrationSection` | `Registration status` | English | product-facing candidate | current English value | `등록 상태` | Candidate, pending user approval | ResourceUiTextProviderTests update likely | section label |
| `Ui.Status.Label` | `Status` | English | product-facing candidate | current English value | `상태` | Candidate, pending user approval | ResourceUiTextProviderTests update likely | label |
| `Ui.Status.LastRegistrationSummaryLabel` | `Last registration summary` | English | product-facing candidate | current English value | `마지막 등록 요약` | Candidate, pending user approval | ResourceUiTextProviderTests update likely | label |
| `Ui.DocumentRegistration.Status.CleanupFailed` | `등록 중 일부 정리가 실패했습니다. 다시 시도하거나 관리자에게 문의하세요.` | Korean | product-facing candidate | existing Korean source literal retained | `등록 중 일부 정리가 실패했습니다. 다시 시도하거나 관리자에게 문의하세요.` | Existing source Korean, review required | No test update expected | source-retained Korean value |
| `Ui.DocumentRegistration.Message.NoActiveClaim` | `No active claim is available for selection.` | English | product-facing candidate | current English value | `선택할 수 있는 활성 청구 대상이 없습니다.` | Candidate, pending user approval | ViewModel exact string tests update likely | validation message |
| `Ui.DocumentRegistration.Message.NoActivePolicy` | `No active policy is available for selection.` | English | product-facing candidate | current English value | `선택할 수 있는 활성 보험 대상이 없습니다.` | Candidate, pending user approval | ViewModel exact string tests update likely | validation message |
| `Ui.DocumentRegistration.Status.Failed` | `문서 등록에 실패했습니다.` | Korean | product-facing candidate | existing Korean source literal retained | `문서 등록에 실패했습니다.` | Existing source Korean, review required | No test update expected | source-retained Korean value |
| `Ui.DocumentRegistration.Status.Completed` | `문서 등록이 완료되었습니다.` | Korean | product-facing candidate | existing Korean source literal retained | `문서 등록이 완료되었습니다.` | Existing source Korean, review required | No test update expected | source-retained Korean value |
| `Ui.DocumentRegistration.Validation.SelectClaimBeforeRegister` | `Select a claim before registering this document.` | English | product-facing candidate | current English value | `문서를 등록하기 전에 청구 대상을 선택해 주세요.` | Candidate, pending user approval | ViewModel exact string tests update likely | validation message |
| `Ui.DocumentRegistration.Validation.SelectPolicyBeforeRegister` | `Select a policy before registering this document.` | English | product-facing candidate | current English value | `문서를 등록하기 전에 보험 대상을 선택해 주세요.` | Candidate, pending user approval | ViewModel exact string tests update likely | validation message |
| `Ui.DocumentRegistration.Status.FileSelected` | `파일을 선택했습니다.` | Korean | product-facing candidate | existing Korean source literal retained | `파일을 선택했습니다.` | Existing source Korean, review required | No test update expected | source-retained Korean value |
| `Ui.DocumentRegistration.Validation.SelectFile` | `파일을 선택해 주세요.` | Korean | product-facing candidate | existing Korean source literal retained | `파일을 선택해 주세요.` | Existing source Korean, review required | No test update expected | source-retained Korean value |
| `Ui.DocumentRegistration.Validation.SelectTargetKind` | `저장할 대상 유형을 선택해 주세요.` | Korean | product-facing candidate | existing Korean source literal retained | `저장할 대상 유형을 선택해 주세요.` | Existing source Korean, review required | No test update expected | source-retained Korean value |
| `Ui.DocumentRegistration.Validation.SelectTarget` | `저장할 대상을 입력해 주세요.` | Korean | product-facing candidate | existing Korean source literal retained | `저장할 대상을 선택해 주세요.` | Candidate, pending user approval | ViewModel exact string tests update likely | wording candidate changes current source literal |
| `Ui.DocumentRegistration.Validation.SelectDocumentType` | `문서 유형을 선택해 주세요.` | Korean | product-facing candidate | existing Korean source literal retained | `문서 유형을 선택해 주세요.` | Existing source Korean, review required | No test update expected | source-retained Korean value |
| `Ui.DocumentRegistration.Validation.EnterDisplayTitle` | `표시 제목을 입력해 주세요.` | Korean | product-facing candidate | existing Korean source literal retained | `표시 제목을 입력해 주세요.` | Existing source Korean, review required | No test update expected | source-retained Korean value |
| `Ui.DocumentRegistration.Validation.SelectReferenceDate` | `기준일을 선택해 주세요.` | Korean | product-facing candidate | existing Korean source literal retained | `기준일을 선택해 주세요.` | Existing source Korean, review required | No test update expected | source-retained Korean value |

## G. Excluded Resource Table

| Resource key | Current value | Ownership | Exclusion reason | Future handling |
|---|---|---|---|---|
| `Ui.Policy.ActiveTargetsLabel` | `Active policy targets` | validation-harness-only | validation harness management panel | defer until product UI shell |
| `Ui.Policy.NewTitleLabel` | `New policy title` | validation-harness-only | validation harness management panel | defer until product UI shell |
| `Ui.Claim.PolicyForNewClaimLabel` | `Policy for new claim` | validation-harness-only | validation harness management panel | defer until product UI shell |
| `Ui.Claim.ActiveTargetsLabel` | `Active claim targets` | validation-harness-only | validation harness management panel | defer until product UI shell |
| `Ui.Claim.NewTitleLabel` | `New claim title` | validation-harness-only | validation harness management panel | defer until product UI shell |
| `Ui.Management.PolicyClaimSection` | `Policy/Claim Management` | validation-harness-only | validation harness management panel | defer until product UI shell |
| `Ui.Management.PolicySection` | `Policy Management` | validation-harness-only | validation harness management panel | defer until product UI shell |
| `Ui.Management.ClaimSection` | `Claim Management` | validation-harness-only | validation harness management panel | defer until product UI shell |
| `Ui.Management.MessageLabel` | `Management message` | validation-harness-only | validation harness management panel | defer until product UI shell |
| `Ui.Action.CreatePolicy` | `Create policy` | validation-harness-only | validation harness management action | defer until product UI shell |
| `Ui.Action.DisablePolicy` | `Disable policy` | validation-harness-only | validation harness management action | defer until product UI shell |
| `Ui.Action.CreateClaim` | `Create claim` | validation-harness-only | validation harness management action | defer until product UI shell |
| `Ui.Action.DisableClaim` | `Disable claim` | validation-harness-only | validation harness management action | defer until product UI shell |
| `Ui.ClaimManagement.Message.Created` | `Claim target was created.` | validation-harness-only | local target management message | defer until product UI shell |
| `Ui.ClaimManagement.Message.Disabled` | `Claim target was disabled.` | validation-harness-only | local target management message | defer until product UI shell |
| `Ui.ClaimManagement.Validation.TitleRequired` | `Claim target title is required.` | validation-harness-only | local target management validation | defer until product UI shell |
| `Ui.PolicyManagement.Message.Created` | `Policy target was created.` | validation-harness-only | local target management message | defer until product UI shell |
| `Ui.PolicyManagement.Message.Disabled` | `Policy target was disabled.` | validation-harness-only | local target management message | defer until product UI shell |
| `Ui.PolicyManagement.Validation.DisableBlockedByActiveClaims` | `Policy target has active claim targets. Disable claim targets first.` | validation-harness-only | local target management validation | defer until product UI shell |
| `Ui.ClaimManagement.Validation.SelectPolicyBeforeCreate` | `Select an active policy target before creating a claim target.` | validation-harness-only | local target management validation | defer until product UI shell |
| `Ui.PolicyManagement.Validation.TitleRequired` | `Policy target title is required.` | validation-harness-only | local target management validation | defer until product UI shell |
| `Ui.ClaimManagement.Validation.SelectClaimTarget` | `Select a claim target.` | validation-harness-only | local target management validation | defer until product UI shell |
| `Ui.PolicyManagement.Validation.SelectPolicyTarget` | `Select a policy target.` | validation-harness-only | local target management validation | defer until product UI shell |
| `Ui.DevHarness.Warning.LocalMvpValidation` | `Local MVP validation screen. Do not use real personal, insurer, hospital, diagnosis, policy number, or claim number samples.` | dev-harness-only | warning copy for development validation harness | exclude from final product copy |
| `Ui.DevHarness.ManagementWarning` | `Create and disable local policy/claim targets with synthetic-safe titles only.` | dev-harness-only | warning copy for development validation harness | exclude from final product copy |

## H. Deferred / Non-Resource Table

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

## I. Implementation Boundary

이 문서는 implementation을 승인하지 않는다. 후보 문구를 source/resource/test에 반영하려면 별도의 user approval table과 exact-file-list implementation batch가 필요하다.

## J. Final Marker

```text
POLICY_CLAIM_FINAL_KOREAN_COPY_CANDIDATE_TABLE_READY
```
