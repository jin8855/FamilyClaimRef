# Policy Claim Validation Harness Management Static XAML Resource Key Plan

## A. 상태

- Status: RESOURCE_KEY_PLAN_ONLY
- Marker:

```text
POLICY_CLAIM_VALIDATION_HARNESS_MANAGEMENT_STATIC_XAML_RESOURCE_KEYS_PLANNED
```

## B. 기준

- 기준 commit: `a570d9a refactor(familyclaimref): extract document registration static xaml strings`
- 기준 원칙: `docs/222_POLICY_CLAIM_RESOURCE_KEY_NAMING_AND_STRING_OWNERSHIP_DECISION.md`

## C. Key Rule

- keys describe product or harness meaning, not current English text.
- values remain current neutral English for first implementation.
- direct Korean replacement is not allowed.
- ViewModel message keys are not included.
- business duplicate keys are not included.
- product UI shell keys are not included.

## D. Planned Key Table

| Current literal | Planned key | Ownership | Resource value for first implementation |
|---|---|---|---|
| `Policy/Claim Management` | `Ui.Management.PolicyClaimSection` | validation-harness-only | `Policy/Claim Management` |
| `Create and disable local policy/claim targets with synthetic-safe titles only.` | `Ui.DevHarness.ManagementWarning` | validation-harness-only | `Create and disable local policy/claim targets with synthetic-safe titles only.` |
| `Policy Management` | `Ui.Management.PolicySection` | validation-harness-only | `Policy Management` |
| `Active policy targets` | `Ui.Policy.ActiveTargetsLabel` | validation-harness-only | `Active policy targets` |
| `New policy title` | `Ui.Policy.NewTitleLabel` | validation-harness-only | `New policy title` |
| `Create policy` | `Ui.Action.CreatePolicy` | validation-harness-only | `Create policy` |
| `Disable policy` | `Ui.Action.DisablePolicy` | validation-harness-only | `Disable policy` |
| `Claim Management` | `Ui.Management.ClaimSection` | validation-harness-only | `Claim Management` |
| `Policy for new claim` | `Ui.Claim.PolicyForNewClaimLabel` | validation-harness-only | `Policy for new claim` |
| `Active claim targets` | `Ui.Claim.ActiveTargetsLabel` | validation-harness-only | `Active claim targets` |
| `New claim title` | `Ui.Claim.NewTitleLabel` | validation-harness-only | `New claim title` |
| `Create claim` | `Ui.Action.CreateClaim` | validation-harness-only | `Create claim` |
| `Disable claim` | `Ui.Action.DisableClaim` | validation-harness-only | `Disable claim` |
| `Management message` | `Ui.Management.MessageLabel` | validation-harness-only | `Management message` |

## E. Keys Not Included

- `Ui.Management.Message.*`
- `Ui.Policy.Created` / `Ui.Policy.Disabled` / similar runtime messages
- `Ui.Claim.Created` / `Ui.Claim.Disabled` / similar runtime messages
- `Ui.Validation.*`
- `Ui.BusinessDuplicate.*`
- `Ui.Product.*`

## F. Resource Key Judgment

```text
POLICY_CLAIM_VALIDATION_HARNESS_MANAGEMENT_STATIC_XAML_RESOURCE_KEYS_READY
```
