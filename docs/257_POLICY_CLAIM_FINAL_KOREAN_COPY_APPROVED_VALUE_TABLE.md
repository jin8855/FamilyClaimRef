# Policy Claim Final Korean Copy Approved Value Table

## A. Status

```text
FINAL_KOREAN_COPY_APPROVED_VALUE_TABLE_ONLY
POLICY_CLAIM_FINAL_KOREAN_COPY_APPROVED_VALUE_TABLE_PLANNED
```

## B. Baseline

```text
1036fba docs(familyclaimref): draft final korean copy candidate table
```

## C. Table Status

- approved value table for future implementation planning
- not implemented
- not reflected in `UiStrings.xaml`
- not reflected in source/test files
- no resource value changes in this batch

## D. Table Rule

- Approved rows are future value-change candidates.
- Keep current rows are not value-change candidates.
- Resource key names remain unchanged.
- `UiStrings.xaml` must not be modified in this batch.
- Actual implementation requires a separate exact-file-list implementation batch.
- Harness-only rows are excluded.
- Deferred/non-resource rows are excluded.

## E. Approved / Keep Current Value Table

| Resource key | Current value | Approved value | User decision | Implementation target | Expected test impact | Notes |
|---|---|---|---|---|---|---|
| `Ui.App.Title` | `FamilyClaimRef` | `FamilyClaimRef` | Keep current | no | No test update expected | app title retained |
| `Ui.Document.SourceFileSection` | `Source file` | `원본 파일` | Approved | yes | ResourceUiTextProviderTests update likely | section label |
| `Ui.Document.SelectedFileLabel` | `Selected file` | `선택한 파일` | Approved | yes | ResourceUiTextProviderTests update likely | label |
| `Ui.Document.MetadataSection` | `Document metadata` | `문서 정보` | Approved | yes | ResourceUiTextProviderTests update likely | section label |
| `Ui.Document.TypeLabel` | `Document type` | `문서 유형` | Approved | yes | ResourceUiTextProviderTests update likely | label |
| `Ui.Document.DisplayTitleLabel` | `Display title` | `표시 제목` | Approved | yes | ResourceUiTextProviderTests update likely | label |
| `Ui.Document.ReferenceDateLabel` | `Reference date` | `기준일` | Approved | yes | ResourceUiTextProviderTests update likely | label |
| `Ui.Target.SelectionSection` | `Target selection` | `저장 대상 선택` | Approved | yes | ResourceUiTextProviderTests update likely | section label |
| `Ui.Target.KindLabel` | `Target kind` | `대상 유형` | Approved | yes | ResourceUiTextProviderTests update likely | label |
| `Ui.Policy.TargetLabel` | `Policy target` | `보험 대상` | Approved | yes | ResourceUiTextProviderTests update likely | generic policy target, not insurer name |
| `Ui.Claim.TargetLabel` | `Claim target` | `청구 대상` | Approved | yes | ResourceUiTextProviderTests update likely | generic claim target |
| `Ui.Action.SelectFile` | `Select file` | `파일 선택` | Approved | yes | ResourceUiTextProviderTests update likely | action label has current exact test |
| `Ui.Action.RegisterDocument` | `Register` | `등록` | Approved | yes | ResourceUiTextProviderTests update likely | action label |
| `Ui.Validation.SectionLabel` | `Validation` | `입력 확인` | Approved | yes | ResourceUiTextProviderTests update likely | section label |
| `Ui.Status.RegistrationSection` | `Registration status` | `등록 상태` | Approved | yes | ResourceUiTextProviderTests update likely | section label |
| `Ui.Status.Label` | `Status` | `상태` | Approved | yes | ResourceUiTextProviderTests update likely | label |
| `Ui.Status.LastRegistrationSummaryLabel` | `Last registration summary` | `마지막 등록 요약` | Approved | yes | ResourceUiTextProviderTests update likely | label |
| `Ui.DocumentRegistration.Status.CleanupFailed` | `등록 중 일부 정리가 실패했습니다. 다시 시도하거나 관리자에게 문의하세요.` | `등록 중 일부 정리가 실패했습니다. 다시 시도하거나 관리자에게 문의하세요.` | Keep current | no | No test update expected | existing source Korean retained |
| `Ui.DocumentRegistration.Message.NoActiveClaim` | `No active claim is available for selection.` | `선택할 수 있는 활성 청구 대상이 없습니다.` | Approved | yes | ViewModel exact string tests update likely | validation/empty state message |
| `Ui.DocumentRegistration.Message.NoActivePolicy` | `No active policy is available for selection.` | `선택할 수 있는 활성 보험 대상이 없습니다.` | Approved | yes | ViewModel exact string tests update likely | validation/empty state message |
| `Ui.DocumentRegistration.Status.Failed` | `문서 등록에 실패했습니다.` | `문서 등록에 실패했습니다.` | Keep current | no | No test update expected | existing source Korean retained |
| `Ui.DocumentRegistration.Status.Completed` | `문서 등록이 완료되었습니다.` | `문서 등록이 완료되었습니다.` | Keep current | no | No test update expected | existing source Korean retained |
| `Ui.DocumentRegistration.Validation.SelectClaimBeforeRegister` | `Select a claim before registering this document.` | `문서를 등록하기 전에 청구 대상을 선택해 주세요.` | Approved | yes | ViewModel exact string tests update likely | validation message |
| `Ui.DocumentRegistration.Validation.SelectPolicyBeforeRegister` | `Select a policy before registering this document.` | `문서를 등록하기 전에 보험 대상을 선택해 주세요.` | Approved | yes | ViewModel exact string tests update likely | validation message |
| `Ui.DocumentRegistration.Status.FileSelected` | `파일을 선택했습니다.` | `파일을 선택했습니다.` | Keep current | no | No test update expected | existing source Korean retained |
| `Ui.DocumentRegistration.Validation.SelectFile` | `파일을 선택해 주세요.` | `파일을 선택해 주세요.` | Keep current | no | No test update expected | existing source Korean retained |
| `Ui.DocumentRegistration.Validation.SelectTargetKind` | `저장할 대상 유형을 선택해 주세요.` | `저장할 대상 유형을 선택해 주세요.` | Keep current | no | No test update expected | existing source Korean retained |
| `Ui.DocumentRegistration.Validation.SelectTarget` | `저장할 대상을 입력해 주세요.` | `저장할 대상을 선택해 주세요.` | Approved | yes | ViewModel exact string tests update likely | wording changes current source literal; target is selected, not typed |
| `Ui.DocumentRegistration.Validation.SelectDocumentType` | `문서 유형을 선택해 주세요.` | `문서 유형을 선택해 주세요.` | Keep current | no | No test update expected | existing source Korean retained |
| `Ui.DocumentRegistration.Validation.EnterDisplayTitle` | `표시 제목을 입력해 주세요.` | `표시 제목을 입력해 주세요.` | Keep current | no | No test update expected | existing source Korean retained |
| `Ui.DocumentRegistration.Validation.SelectReferenceDate` | `기준일을 선택해 주세요.` | `기준일을 선택해 주세요.` | Keep current | no | No test update expected | existing source Korean retained |

## F. Count Summary

| Count item | Count |
|---|---:|
| Total rows | 31 |
| Approved | 21 |
| Keep current | 10 |
| Revise | 0 |
| Defer | 0 |
| Reject | 0 |
| Implementation target yes | 21 |
| Implementation target no | 10 |

## G. Excluded Scope Summary

| Excluded item | Count | Reason |
|---|---:|---|
| Excluded resource rows | 25 | validation-harness-only and dev-harness-only rows |
| Deferred/non-resource rows | 8 | target values, diagnostic formats, and future-only keys |

## H. Implementation Boundary

This table does not modify resource values. It documents approved values for a future exact-file-list implementation batch.

## I. Final Marker

```text
POLICY_CLAIM_FINAL_KOREAN_COPY_APPROVED_VALUE_TABLE_READY
```
