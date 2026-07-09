# Policy Claim ViewModel Runtime Message Extraction Scope Plan

## A. Status

```text
VIEWMODEL_RUNTIME_MESSAGE_EXTRACTION_PLAN_ONLY
POLICY_CLAIM_VIEWMODEL_RUNTIME_MESSAGE_EXTRACTION_SCOPE_PLANNED
```

이 문서는 ViewModel runtime message extraction의 범위와 제외 범위를 계획한다. 구현 문서가 아니며 code, test, XAML, ViewModel, resource 파일을 수정하지 않는다.

## B. 기준 commit

```text
687bc26 docs(familyclaimref): consolidate ui resource current state
```

## C. Current static baseline

- extracted static UI resource keys: 32
- document registration static XAML extraction 완료
- management static XAML extraction 완료
- `MainWindow.xaml`의 static label/button/header resource baseline은 유지한다.
- direct Korean replacement, final Korean copy, culture switching, dynamic language switching은 아직 미구현 상태다.

## D. Source-inspected ViewModel list

| Source file | Inspection result |
|---|---|
| `app/FamilyClaimRef.App/ViewModels/DocumentRegistrationViewModel.cs` | validation/status/target selection/summary runtime literal 확인. Constructor는 `DocumentRegistrationWorkflow`, `IFilePickerService`, `IPolicyClaimStorageService`만 받는다. |
| `app/FamilyClaimRef.App/ViewModels/PolicyClaimManagementViewModel.cs` | management action result/guard runtime literal 확인. Constructor는 `IPolicyClaimStorageService`만 받는다. |
| `app/FamilyClaimRef.App/ViewModels/MainWindowViewModel.cs` | 두 ViewModel을 조합하고 action refresh를 위임한다. 자체 runtime message literal은 확인되지 않았다. |
| `app/FamilyClaimRef.App/Composition/AppServices.cs` | `DocumentRegistrationViewModel`, `PolicyClaimManagementViewModel`, `MainWindowViewModel` 생성 경로 확인. 현재 `IUiTextProvider`는 ViewModel constructor에 주입되지 않는다. |

## E. 포함 후보

- `DocumentRegistrationViewModel` validation/status/runtime messages
- `PolicyClaimManagementViewModel` runtime management messages
- future `IUiTextProvider` access strategy
- future test impact

## F. 제외 범위

- static XAML label/button/header extraction
- `MainWindow.xaml` layout/control hierarchy
- `MainWindow.xaml` `policy` / `claim` ComboBox value
- `MainWindow.xaml` `StringFormat=Is busy: {0}`
- final Korean copy
- direct Korean replacement
- culture switching / dynamic language switching
- business duplicate rule/copy
- product UI shell
- wireframe port
- UI redesign
- Scenario 9 cleanup
- DB/SQLite/OCR/repository
- `data/claimdoc`

## G. Candidate runtime message inventory

| Source file | ViewModel | Current literal | Message purpose | Candidate extraction decision | Notes |
|---|---|---|---|---|---|
| `app/FamilyClaimRef.App/ViewModels/DocumentRegistrationViewModel.cs` | `DocumentRegistrationViewModel` | `등록 중 일부 정리가 실패했습니다. 다시 시도하거나 관리자에게 문의하세요.` | cleanup failure status | Include candidate | Source constant: `CleanupFailureMessage`. |
| `app/FamilyClaimRef.App/ViewModels/DocumentRegistrationViewModel.cs` | `DocumentRegistrationViewModel` | `No active claim is available for selection.` | target selection empty state | Include candidate | Source constant: `NoActiveClaimMessage`. |
| `app/FamilyClaimRef.App/ViewModels/DocumentRegistrationViewModel.cs` | `DocumentRegistrationViewModel` | `No active policy is available for selection.` | target selection empty state | Include candidate | Source constant: `NoActivePolicyMessage`. |
| `app/FamilyClaimRef.App/ViewModels/DocumentRegistrationViewModel.cs` | `DocumentRegistrationViewModel` | `문서 등록에 실패했습니다.` | registration failure status | Include candidate | Source constant: `RegistrationFailureMessage`. |
| `app/FamilyClaimRef.App/ViewModels/DocumentRegistrationViewModel.cs` | `DocumentRegistrationViewModel` | `문서 등록이 완료되었습니다.` | registration success status | Include candidate | Source constant: `RegistrationSuccessMessage`. |
| `app/FamilyClaimRef.App/ViewModels/DocumentRegistrationViewModel.cs` | `DocumentRegistrationViewModel` | `Select a claim before registering this document.` | claim target validation | Include candidate | Source constant: `SelectClaimMessage`. |
| `app/FamilyClaimRef.App/ViewModels/DocumentRegistrationViewModel.cs` | `DocumentRegistrationViewModel` | `Select a policy before registering this document.` | policy target validation | Include candidate | Source constant: `SelectPolicyMessage`. |
| `app/FamilyClaimRef.App/ViewModels/DocumentRegistrationViewModel.cs` | `DocumentRegistrationViewModel` | `파일을 선택했습니다.` | file selected status | Include candidate | Direct assignment in `SelectFileAsync`. |
| `app/FamilyClaimRef.App/ViewModels/DocumentRegistrationViewModel.cs` | `DocumentRegistrationViewModel` | `파일을 선택해 주세요.` | missing file validation | Include candidate | Direct assignment in `Validate`. |
| `app/FamilyClaimRef.App/ViewModels/DocumentRegistrationViewModel.cs` | `DocumentRegistrationViewModel` | `저장할 대상 유형을 선택해 주세요.` | unsupported target kind validation | Include candidate | Direct assignment in `Validate`. |
| `app/FamilyClaimRef.App/ViewModels/DocumentRegistrationViewModel.cs` | `DocumentRegistrationViewModel` | `저장할 대상을 입력해 주세요.` | missing target validation | Include candidate | Direct assignment in `Validate`. |
| `app/FamilyClaimRef.App/ViewModels/DocumentRegistrationViewModel.cs` | `DocumentRegistrationViewModel` | `문서 유형을 선택해 주세요.` | missing document type validation | Include candidate | Direct assignment in `Validate`. |
| `app/FamilyClaimRef.App/ViewModels/DocumentRegistrationViewModel.cs` | `DocumentRegistrationViewModel` | `표시 제목을 입력해 주세요.` | missing display title validation | Include candidate | Direct assignment in `Validate`. |
| `app/FamilyClaimRef.App/ViewModels/DocumentRegistrationViewModel.cs` | `DocumentRegistrationViewModel` | `기준일을 선택해 주세요.` | missing reference date validation | Include candidate | Direct assignment in `Validate`. |
| `app/FamilyClaimRef.App/ViewModels/DocumentRegistrationViewModel.cs` | `DocumentRegistrationViewModel` | `policy:{policyId}; document:{documentId}` | diagnostic policy registration summary format | Defer | Derived from interpolation shape in `CreatePolicySummary`; not final user copy. |
| `app/FamilyClaimRef.App/ViewModels/DocumentRegistrationViewModel.cs` | `DocumentRegistrationViewModel` | `claim:{claimId}; document:{documentId}` | diagnostic claim registration summary format | Defer | Derived from interpolation shape in `CreateClaimSummary`; not final user copy. |
| `app/FamilyClaimRef.App/ViewModels/PolicyClaimManagementViewModel.cs` | `PolicyClaimManagementViewModel` | `Claim target was created.` | claim creation result | Include candidate | Source constant: `ClaimCreatedMessage`. |
| `app/FamilyClaimRef.App/ViewModels/PolicyClaimManagementViewModel.cs` | `PolicyClaimManagementViewModel` | `Claim target was disabled.` | claim disable result | Include candidate | Source constant: `ClaimDisabledMessage`. |
| `app/FamilyClaimRef.App/ViewModels/PolicyClaimManagementViewModel.cs` | `PolicyClaimManagementViewModel` | `Claim target title is required.` | claim title validation | Include candidate | Source constant: `ClaimTitleRequiredMessage`. |
| `app/FamilyClaimRef.App/ViewModels/PolicyClaimManagementViewModel.cs` | `PolicyClaimManagementViewModel` | `Policy target was created.` | policy creation result | Include candidate | Source constant: `PolicyCreatedMessage`. |
| `app/FamilyClaimRef.App/ViewModels/PolicyClaimManagementViewModel.cs` | `PolicyClaimManagementViewModel` | `Policy target was disabled.` | policy disable result | Include candidate | Source constant: `PolicyDisabledMessage`. |
| `app/FamilyClaimRef.App/ViewModels/PolicyClaimManagementViewModel.cs` | `PolicyClaimManagementViewModel` | `Policy target has active claim targets. Disable claim targets first.` | policy disable blocked validation | Include candidate | Source constant: `PolicyDisableBlockedMessage`. |
| `app/FamilyClaimRef.App/ViewModels/PolicyClaimManagementViewModel.cs` | `PolicyClaimManagementViewModel` | `Select an active policy target before creating a claim target.` | claim creation policy selection validation | Include candidate | Source constant: `PolicyRequiredForClaimMessage`. |
| `app/FamilyClaimRef.App/ViewModels/PolicyClaimManagementViewModel.cs` | `PolicyClaimManagementViewModel` | `Policy target title is required.` | policy title validation | Include candidate | Source constant: `PolicyTitleRequiredMessage`. |
| `app/FamilyClaimRef.App/ViewModels/PolicyClaimManagementViewModel.cs` | `PolicyClaimManagementViewModel` | `Select a claim target.` | claim selection validation | Include candidate | Source constant: `SelectClaimMessage`. |
| `app/FamilyClaimRef.App/ViewModels/PolicyClaimManagementViewModel.cs` | `PolicyClaimManagementViewModel` | `Select a policy target.` | policy selection validation | Include candidate | Source constant: `SelectPolicyMessage`. |

## H. Candidate extraction summary

- candidate runtime messages found: 26
- Include candidate: 24
- Defer: 2
- Exclude: 0
- Unknown: 0
- `DocumentRegistrationViewModel` source-inspected messages: 16
- `PolicyClaimManagementViewModel` source-inspected messages: 10

## I. 판단 marker

```text
POLICY_CLAIM_VIEWMODEL_RUNTIME_MESSAGE_EXTRACTION_SCOPE_READY
```
