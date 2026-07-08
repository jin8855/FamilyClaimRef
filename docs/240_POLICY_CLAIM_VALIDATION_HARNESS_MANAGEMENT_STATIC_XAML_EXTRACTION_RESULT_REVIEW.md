# Policy Claim Validation Harness Management Static XAML Extraction Result Review

## A. 상태

```text
POLICY_CLAIM_VALIDATION_HARNESS_MANAGEMENT_STATIC_XAML_EXTRACTION_COMPLETED
```

## B. 기준 commit

- 기준 commit: `8c68369 docs(familyclaimref): plan management static xaml string extraction`

## C. 검토한 기준 문서

- `docs/236_POLICY_CLAIM_VALIDATION_HARNESS_MANAGEMENT_STATIC_XAML_EXTRACTION_SCOPE_PLAN.md`
- `docs/237_POLICY_CLAIM_VALIDATION_HARNESS_MANAGEMENT_STATIC_XAML_RESOURCE_KEY_PLAN.md`
- `docs/238_POLICY_CLAIM_VALIDATION_HARNESS_MANAGEMENT_STATIC_XAML_EXTRACTION_TEST_PLAN.md`
- `docs/239_POLICY_CLAIM_VALIDATION_HARNESS_MANAGEMENT_STATIC_XAML_EXTRACTION_COMMIT_CANDIDATE_REVIEW.md`

## D. 변경 파일

- `app/FamilyClaimRef.App/Resources/UiStrings.xaml`
- `app/FamilyClaimRef.App/Services/Localization/UiTextKeys.cs`
- `app/FamilyClaimRef.App/MainWindow.xaml`

## E. 생성 문서

- `docs/240_POLICY_CLAIM_VALIDATION_HARNESS_MANAGEMENT_STATIC_XAML_EXTRACTION_RESULT_REVIEW.md`

## F. 추가 Resource Key

승인된 14개 management static XAML key만 추가했다.
resource value는 현재 English neutral/current value를 유지했다.

| Key | Value |
|---|---|
| `Ui.Management.PolicyClaimSection` | `Policy/Claim Management` |
| `Ui.DevHarness.ManagementWarning` | `Create and disable local policy/claim targets with synthetic-safe titles only.` |
| `Ui.Management.PolicySection` | `Policy Management` |
| `Ui.Policy.ActiveTargetsLabel` | `Active policy targets` |
| `Ui.Policy.NewTitleLabel` | `New policy title` |
| `Ui.Action.CreatePolicy` | `Create policy` |
| `Ui.Action.DisablePolicy` | `Disable policy` |
| `Ui.Management.ClaimSection` | `Claim Management` |
| `Ui.Claim.PolicyForNewClaimLabel` | `Policy for new claim` |
| `Ui.Claim.ActiveTargetsLabel` | `Active claim targets` |
| `Ui.Claim.NewTitleLabel` | `New claim title` |
| `Ui.Action.CreateClaim` | `Create claim` |
| `Ui.Action.DisableClaim` | `Disable claim` |
| `Ui.Management.MessageLabel` | `Management message` |

## G. XAML Mapping

`MainWindow.xaml`의 Policy/Claim Management 영역에서 승인된 14개 static literal만 `{StaticResource ...}` 참조로 교체했다.

| Previous literal | Applied resource |
|---|---|
| `Policy/Claim Management` | `{StaticResource Ui.Management.PolicyClaimSection}` |
| `Create and disable local policy/claim targets with synthetic-safe titles only.` | `{StaticResource Ui.DevHarness.ManagementWarning}` |
| `Policy Management` | `{StaticResource Ui.Management.PolicySection}` |
| `Active policy targets` | `{StaticResource Ui.Policy.ActiveTargetsLabel}` |
| `New policy title` | `{StaticResource Ui.Policy.NewTitleLabel}` |
| `Create policy` | `{StaticResource Ui.Action.CreatePolicy}` |
| `Disable policy` | `{StaticResource Ui.Action.DisablePolicy}` |
| `Claim Management` | `{StaticResource Ui.Management.ClaimSection}` |
| `Policy for new claim` | `{StaticResource Ui.Claim.PolicyForNewClaimLabel}` |
| `Active claim targets` | `{StaticResource Ui.Claim.ActiveTargetsLabel}` |
| `New claim title` | `{StaticResource Ui.Claim.NewTitleLabel}` |
| `Create claim` | `{StaticResource Ui.Action.CreateClaim}` |
| `Disable claim` | `{StaticResource Ui.Action.DisableClaim}` |
| `Management message` | `{StaticResource Ui.Management.MessageLabel}` |

## H. 명시적 제외 범위

- `PolicyClaimManagementViewModel` runtime messages는 추출하지 않았다.
- ViewModel message provider injection은 구현하지 않았다.
- `Ui.Management.Message.*` runtime key는 추가하지 않았다.
- `Ui.Policy.Created`, `Ui.Policy.Disabled` 계열 runtime message key는 추가하지 않았다.
- `Ui.Claim.Created`, `Ui.Claim.Disabled` 계열 runtime message key는 추가하지 않았다.
- `Ui.BusinessDuplicate.*`와 `Ui.Product.*` key는 추가하지 않았다.
- layout, control hierarchy, binding, command binding은 변경하지 않았다.
- ViewModel, test, `App.xaml`, `IUiTextProvider`, `ResourceUiTextProvider`는 변경하지 않았다.
- localization culture switching과 dynamic language switching은 구현하지 않았다.
- direct Korean replacement와 final Korean copy 확정은 수행하지 않았다.
- wireframe port, UI redesign, product UI shell은 구현하지 않았다.
- app launch, OpenFileDialog, screenshot/visual automation, manual workflow는 실행하지 않았다.
- cleanup, runtime metadata deletion, runtime attachment deletion은 실행하지 않았다.
- `data/claimdoc` 접근은 수행하지 않았다.
- DB/SQLite/OCR/repository는 구현하지 않았다.

## I. 검증 결과

- 최초 sandbox `dotnet build FamilyClaimRef.sln`: Windows SDK user-profile path access boundary로 실패했다.
- elevated `dotnet build FamilyClaimRef.sln`: PASS, warning 0, error 0.
- 최초 sandbox targeted resource test: 동일한 Windows SDK access boundary로 실패했다.
- elevated `dotnet test FamilyClaimRef.sln --filter FullyQualifiedName~ResourceUiTextProviderTests`: PASS, total 9, failed 0.
- elevated `dotnet test FamilyClaimRef.sln`: PASS, total 306, failed 0.
- `git diff --check`: PASS.
- exact changed files trailing whitespace scan: PASS.
- exact changed files actual personal/sample scan: PASS.
- `git check-ignore -v -- data/claimdoc/`: PASS.
- `git check-ignore -v -- docs/nightwork_20260706/`: PASS.
- project root `attachments/`: files 0.
- project root `data/local/`: files 0.
- project root `runtime_test_document.*`: missing.
- DB/SQLite unexpected file: none.

## J. Commit Candidate

Exact file list:

- `app/FamilyClaimRef.App/Resources/UiStrings.xaml`
- `app/FamilyClaimRef.App/Services/Localization/UiTextKeys.cs`
- `app/FamilyClaimRef.App/MainWindow.xaml`
- `docs/240_POLICY_CLAIM_VALIDATION_HARNESS_MANAGEMENT_STATIC_XAML_EXTRACTION_RESULT_REVIEW.md`

Recommended commit message:

```text
refactor(familyclaimref): extract management static xaml strings
```

## K. 잔여 위험

- Policy/Claim Management runtime messages는 아직 hard-coded 상태다.
- product UI shell과 최종 Korean copy는 아직 결정하지 않았다.
- UI redesign은 계속 deferred 상태다.
