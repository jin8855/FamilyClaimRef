# Policy Claim Validation Harness Management Static XAML Extraction Scope Plan

## A. 상태

- Status: EXTRACTION_PLAN_ONLY
- Marker:

```text
POLICY_CLAIM_VALIDATION_HARNESS_MANAGEMENT_STATIC_XAML_EXTRACTION_SCOPE_PLANNED
```

## B. 기준

- 기준 commit: `a570d9a refactor(familyclaimref): extract document registration static xaml strings`
- 기준 문서:
  - `docs/218_POLICY_CLAIM_UI_STRING_INVENTORY_REVIEW.md`
  - `docs/222_POLICY_CLAIM_RESOURCE_KEY_NAMING_AND_STRING_OWNERSHIP_DECISION.md`
  - `docs/235_POLICY_CLAIM_VALIDATION_HARNESS_NEXT_STATIC_XAML_EXTRACTION_RESULT_REVIEW.md`

## C. 목표

다음 implementation batch에서는 `MainWindow.xaml`의 Policy/Claim Management 영역에 남아 있는 static XAML labels/buttons/headers만 resource로 분리한다.

이번 문서는 계획 문서이며 code, test, XAML, ViewModel, resource 파일을 수정하지 않는다.

## D. 포함 후보 Literal

| No | Literal | 구분 |
|---|---|---|
| 1 | `Policy/Claim Management` | section header |
| 2 | `Create and disable local policy/claim targets with synthetic-safe titles only.` | harness warning |
| 3 | `Policy Management` | group header |
| 4 | `Active policy targets` | label |
| 5 | `New policy title` | label |
| 6 | `Create policy` | button |
| 7 | `Disable policy` | button |
| 8 | `Claim Management` | group header |
| 9 | `Policy for new claim` | label |
| 10 | `Active claim targets` | label |
| 11 | `New claim title` | label |
| 12 | `Create claim` | button |
| 13 | `Disable claim` | button |
| 14 | `Management message` | label |

## E. 제외 범위

- `PolicyClaimManagementViewModel` runtime messages
- ViewModel message provider injection
- command behavior
- policy/claim lifecycle logic
- business duplicate warning/copy
- product UI shell
- wireframe port
- UI redesign
- final Korean copy
- direct Korean replacement
- DB/SQLite/OCR/repository
- `data/claimdoc`

## F. 구현 후보 파일

다음 batch에서 구현이 승인될 경우의 후보 파일은 아래로 제한한다.

- `app/FamilyClaimRef.App/Resources/UiStrings.xaml`
- `app/FamilyClaimRef.App/Services/Localization/UiTextKeys.cs`
- `app/FamilyClaimRef.App/MainWindow.xaml`
- `docs/240_POLICY_CLAIM_VALIDATION_HARNESS_MANAGEMENT_STATIC_XAML_EXTRACTION_RESULT_REVIEW.md`

## G. 범위 판단

Policy/Claim Management 영역은 현재 validation harness support 영역이다.
따라서 이번 추출 계획은 product UI shell이나 최종 화면 문구 확정이 아니라, validation harness의 static XAML 문자열을 resource key 경계로 옮기는 작업으로 제한한다.

## H. Scope Judgment

```text
POLICY_CLAIM_VALIDATION_HARNESS_MANAGEMENT_STATIC_XAML_EXTRACTION_SCOPE_READY
```
