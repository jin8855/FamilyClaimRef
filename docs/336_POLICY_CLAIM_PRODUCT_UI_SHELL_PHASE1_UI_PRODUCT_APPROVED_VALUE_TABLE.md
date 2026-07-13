# Product UI Shell Phase 1 Ui.Product Approved Value Table

## A. Status

PRODUCT_UI_SHELL_PHASE1_UI_PRODUCT_APPROVED_VALUE_TABLE_ONLY

## B. Marker

POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1_UI_PRODUCT_APPROVED_VALUE_TABLE_READY

## C. Baseline

- 기준 commit: `21c51ab docs(familyclaimref): plan product shell phase1 entry copy filelist decisions`
- approval source: user approved ChatGPT recommendation
- 작업 유형: documentation-only approved table batch

## D. Approved Product Terminology Table

| Concept | Approved product terminology | Implementation target now | Notes |
|---|---|---|---|
| Policy target | 보험 계약 | no | product shell terminology approval only |
| Claim target | 청구 건 | no | product shell terminology approval only |
| Document registration | 문서 등록 | no | product shell terminology approval only |
| Document list | 문서 목록 | no | product shell terminology approval only |
| Home / dashboard | 홈 | no | product shell terminology approval only |
| Target selection | 연결 대상 선택 | no | product shell terminology approval only |
| Document metadata | 문서 정보 | no | product shell terminology approval only |

Implementation target now is `no` for every terminology row.

This terminology approval is for the ProductShell Phase 1 copy table. It does not approve direct changes to existing validation harness resource values such as `Ui.Policy.TargetLabel` or `Ui.Claim.TargetLabel`.

## E. Approved Ui.Product.* Value Table

| Resource key | Approved value | Purpose | Implementation target now | Test impact |
|---|---|---|---|---|
| `Ui.Product.Shell.Title` | FamilyClaimRef | product shell title | no | future `ResourceUiTextProviderTests` update |
| `Ui.Product.Navigation.Home` | 홈 | product navigation home label | no | future `ResourceUiTextProviderTests` update |
| `Ui.Product.Navigation.DocumentRegistration` | 문서 등록 | product navigation document registration label | no | future `ResourceUiTextProviderTests` update |
| `Ui.Product.Navigation.DocumentList` | 문서 목록 | product navigation document list label | no | future `ResourceUiTextProviderTests` update |
| `Ui.Product.Home.Title` | 홈 | product home title | no | future `ResourceUiTextProviderTests` update |
| `Ui.Product.DocumentRegistration.Title` | 문서 등록 | product document registration title | no | future `ResourceUiTextProviderTests` update |
| `Ui.Product.DocumentList.Title` | 문서 목록 | product document list title | no | future `ResourceUiTextProviderTests` update |
| `Ui.Product.DocumentList.EmptyMessage` | 등록된 문서가 없습니다. | product document list empty state | no | future `ResourceUiTextProviderTests` update |

Implementation target now is `no` for every `Ui.Product.*` row.

This document approves key names and values for a future exact-file-list implementation only.

## F. Boundary

- This table is an approved copy contract, not a source implementation result.
- Existing validation harness resource values are not changed by this approval.
- `Ui.Policy.TargetLabel = 보험 대상` remains unchanged.
- `Ui.Claim.TargetLabel = 청구 대상` remains unchanged.
- Changing either existing value is not approved by this document.
- direct Korean replacement is not approved.
- no key is added by this document
- no `Ui.Product.*` key is added in this batch
- no copy is written to `UiStrings.xaml` by this document
- existing `Ui.*` 56 baseline remains unchanged by this document
- existing validation harness copy is not productized by this document
- `Ui.Product.*` implementation remains blocked until a separate implementation batch is explicitly approved
- implementation target now count: 0
- approved terminology row count: 7
- approved `Ui.Product.*` value row count: 8

## G. Approval Count Summary

| Count item | Value |
|---|---:|
| approved terminology rows | 7 |
| approved `Ui.Product.*` value rows | 8 |
| implementation target now | 0 |
| source/resource/test changes now | 0 |
