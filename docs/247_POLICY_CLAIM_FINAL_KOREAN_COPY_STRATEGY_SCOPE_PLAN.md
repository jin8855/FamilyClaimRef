# Policy Claim Final Korean Copy Strategy Scope Plan

## A. Status

```text
FINAL_KOREAN_COPY_STRATEGY_PLAN_ONLY
POLICY_CLAIM_FINAL_KOREAN_COPY_STRATEGY_SCOPE_PLANNED
```

## B. 기준 commit

```text
a8a2407 refactor(familyclaimref): extract viewmodel runtime messages
```

## C. 목적

FamilyClaimRef `Ui.*` resource baseline 56개에 대한 final Korean copy strategy 범위와 제외 범위를 계획한다.

이 문서는 copywriting 결과 확정 문서가 아니며, resource value 변경 문서도 아니다. 현재 resource value는 `current source literal/current behavior preservation` 기준으로만 다룬다.

## D. Current resource baseline

- static XAML resource keys: 32
- ViewModel runtime message keys: 24
- expected total `Ui.*` resource keys: 56
- verified `UiStrings.xaml` `Ui.*` keys: 56
- verified `UiTextKeys.cs` `Ui.*` constants: 56

## E. 포함 후보

- product-facing candidate resource values
- existing Korean source literal review
- English current value review
- validation-harness-only string classification
- dev-harness-only warning classification
- copy ownership classification
- future implementation sequencing

## F. 제외 범위

- resource value 변경
- 새 Korean translation 작성
- final Korean copy 확정
- direct Korean replacement
- culture switching / dynamic language switching implementation
- resx/satellite resource architecture
- `MainWindow.xaml` layout/control hierarchy
- ViewModel behavior
- tests update
- deferred diagnostic summary format extraction
- `policy:{policyId}; document:{documentId}`
- `claim:{claimId}; document:{documentId}`
- `Ui.BusinessDuplicate.*`
- `Ui.Product.*`
- `Ui.ActionResult.*`
- business duplicate rule/copy
- product UI shell
- wireframe port
- UI redesign
- Scenario 9 cleanup
- DB/SQLite/OCR/repository
- `data/claimdoc`

## G. Copy decision categories

| Category | Meaning | First handling |
|---|---|---|
| Keep current value for harness | 현재 validation harness에서만 쓰이는 값 | product copy로 승격하지 않고 유지 |
| Needs final Korean copy later | English current value가 product-facing 후보인 경우 | 별도 승인된 copy table 이후 결정 |
| Existing Korean source literal, review later | 기존 source에 있던 Korean literal을 resource로 이동한 경우 | 새 translation으로 보지 않고 추후 product copy 검토 |
| Product-facing candidate, decision required | 최종 UI에서도 사용될 가능성이 있는 key | ownership과 copy tone 결정 필요 |
| Validation-harness-only, do not productize yet | MVP 검증 화면용 key | product UI shell 이전에는 final copy 대상에서 분리 |
| Dev-harness-only, keep separate from product copy | 개발/검증 경고 문구 | product-facing copy로 승격하지 않음 |
| Deferred, outside this batch | 아직 resource화하지 않거나 별도 결정이 필요한 항목 | 이번 batch에서는 문구 결정 제외 |

## H. Ownership strategy

- `Ui.App.*`: infrastructure/app-shell로 분리한다.
- `Ui.Document.*`, `Ui.Target.*`, 일부 `Ui.Policy.*`, 일부 `Ui.Claim.*`, `Ui.Status.*`: product-facing candidate로 검토한다.
- `Ui.Management.*`, management action labels, management runtime messages: 현재 validation-harness-only로 분리한다.
- `Ui.DevHarness.*`: dev-harness-only로 분리한다.
- `Ui.DocumentRegistration.*`: product-facing candidate이나, 현재 Korean literal은 source-retained 상태로 기록한다.
- `Ui.PolicyManagement.*`, `Ui.ClaimManagement.*`: 현재 validation-harness-only runtime message로 기록한다.

## I. Future implementation sequencing

1. final Korean copy 대상 key를 product-facing candidate와 harness-only로 분리한다.
2. product-facing candidate에 대해서만 승인된 copy table을 만든다.
3. existing Korean source literal은 새 Korean translation으로 취급하지 않는다.
4. English current value를 Korean으로 바꾸는 작업은 별도 exact-file-list implementation batch에서만 수행한다.
5. Resource key 이름은 별도 승인 없이는 유지한다.
6. `ResourceUiTextProviderTests`, ViewModel exact string tests, full test suite를 함께 갱신/검증한다.

## J. 판단 marker

```text
POLICY_CLAIM_FINAL_KOREAN_COPY_STRATEGY_SCOPE_READY
```
