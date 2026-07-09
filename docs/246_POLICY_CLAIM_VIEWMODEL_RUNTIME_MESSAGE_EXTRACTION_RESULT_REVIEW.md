# Policy Claim ViewModel Runtime Message Extraction Result Review

## A. Status

```text
POLICY_CLAIM_VIEWMODEL_RUNTIME_MESSAGE_EXTRACTION_COMPLETED
```

## B. 기준 commit

```text
ee0c7b5 docs(familyclaimref): plan viewmodel runtime message extraction
```

## C. 검토한 기준 문서

- `docs/242_POLICY_CLAIM_VIEWMODEL_RUNTIME_MESSAGE_EXTRACTION_SCOPE_PLAN.md`
- `docs/243_POLICY_CLAIM_VIEWMODEL_RUNTIME_MESSAGE_RESOURCE_KEY_PLAN.md`
- `docs/244_POLICY_CLAIM_VIEWMODEL_RUNTIME_MESSAGE_EXTRACTION_TEST_PLAN.md`
- `docs/245_POLICY_CLAIM_VIEWMODEL_RUNTIME_MESSAGE_EXTRACTION_COMMIT_CANDIDATE_REVIEW.md`

## D. Modified files

- `app/FamilyClaimRef.App/Resources/UiStrings.xaml`
- `app/FamilyClaimRef.App/Services/Localization/UiTextKeys.cs`
- `app/FamilyClaimRef.App/ViewModels/DocumentRegistrationViewModel.cs`
- `app/FamilyClaimRef.App/ViewModels/PolicyClaimManagementViewModel.cs`
- `app/FamilyClaimRef.App/Composition/AppServices.cs`
- `tests/FamilyClaimRef.App.Tests/DocumentRegistrationViewModelTests.cs`
- `tests/FamilyClaimRef.App.Tests/PolicyClaimManagementViewModelTests.cs`
- `tests/FamilyClaimRef.App.Tests/Services/Localization/ResourceUiTextProviderTests.cs`

## E. Created docs

- `docs/246_POLICY_CLAIM_VIEWMODEL_RUNTIME_MESSAGE_EXTRACTION_RESULT_REVIEW.md`

## F. Added resource keys

| Resource key | First implementation value |
|---|---|
| `Ui.DocumentRegistration.Status.CleanupFailed` | `등록 중 일부 정리가 실패했습니다. 다시 시도하거나 관리자에게 문의하세요.` |
| `Ui.DocumentRegistration.Message.NoActiveClaim` | `No active claim is available for selection.` |
| `Ui.DocumentRegistration.Message.NoActivePolicy` | `No active policy is available for selection.` |
| `Ui.DocumentRegistration.Status.Failed` | `문서 등록에 실패했습니다.` |
| `Ui.DocumentRegistration.Status.Completed` | `문서 등록이 완료되었습니다.` |
| `Ui.DocumentRegistration.Validation.SelectClaimBeforeRegister` | `Select a claim before registering this document.` |
| `Ui.DocumentRegistration.Validation.SelectPolicyBeforeRegister` | `Select a policy before registering this document.` |
| `Ui.DocumentRegistration.Status.FileSelected` | `파일을 선택했습니다.` |
| `Ui.DocumentRegistration.Validation.SelectFile` | `파일을 선택해 주세요.` |
| `Ui.DocumentRegistration.Validation.SelectTargetKind` | `저장할 대상 유형을 선택해 주세요.` |
| `Ui.DocumentRegistration.Validation.SelectTarget` | `저장할 대상을 입력해 주세요.` |
| `Ui.DocumentRegistration.Validation.SelectDocumentType` | `문서 유형을 선택해 주세요.` |
| `Ui.DocumentRegistration.Validation.EnterDisplayTitle` | `표시 제목을 입력해 주세요.` |
| `Ui.DocumentRegistration.Validation.SelectReferenceDate` | `기준일을 선택해 주세요.` |
| `Ui.ClaimManagement.Message.Created` | `Claim target was created.` |
| `Ui.ClaimManagement.Message.Disabled` | `Claim target was disabled.` |
| `Ui.ClaimManagement.Validation.TitleRequired` | `Claim target title is required.` |
| `Ui.PolicyManagement.Message.Created` | `Policy target was created.` |
| `Ui.PolicyManagement.Message.Disabled` | `Policy target was disabled.` |
| `Ui.PolicyManagement.Validation.DisableBlockedByActiveClaims` | `Policy target has active claim targets. Disable claim targets first.` |
| `Ui.ClaimManagement.Validation.SelectPolicyBeforeCreate` | `Select an active policy target before creating a claim target.` |
| `Ui.PolicyManagement.Validation.TitleRequired` | `Policy target title is required.` |
| `Ui.ClaimManagement.Validation.SelectClaimTarget` | `Select a claim target.` |
| `Ui.PolicyManagement.Validation.SelectPolicyTarget` | `Select a policy target.` |

## G. ViewModel mapping

| ViewModel | Runtime message group | Provider lookup |
|---|---|---|
| `DocumentRegistrationViewModel` | cleanup failure status | `UiTextKeys.DocumentRegistrationStatusCleanupFailed` |
| `DocumentRegistrationViewModel` | no active claim empty state | `UiTextKeys.DocumentRegistrationMessageNoActiveClaim` |
| `DocumentRegistrationViewModel` | no active policy empty state | `UiTextKeys.DocumentRegistrationMessageNoActivePolicy` |
| `DocumentRegistrationViewModel` | registration failure status | `UiTextKeys.DocumentRegistrationStatusFailed` |
| `DocumentRegistrationViewModel` | registration success status | `UiTextKeys.DocumentRegistrationStatusCompleted` |
| `DocumentRegistrationViewModel` | claim target validation | `UiTextKeys.DocumentRegistrationValidationSelectClaimBeforeRegister` |
| `DocumentRegistrationViewModel` | policy target validation | `UiTextKeys.DocumentRegistrationValidationSelectPolicyBeforeRegister` |
| `DocumentRegistrationViewModel` | file selected status | `UiTextKeys.DocumentRegistrationStatusFileSelected` |
| `DocumentRegistrationViewModel` | missing file validation | `UiTextKeys.DocumentRegistrationValidationSelectFile` |
| `DocumentRegistrationViewModel` | unsupported target kind validation | `UiTextKeys.DocumentRegistrationValidationSelectTargetKind` |
| `DocumentRegistrationViewModel` | missing target validation | `UiTextKeys.DocumentRegistrationValidationSelectTarget` |
| `DocumentRegistrationViewModel` | missing document type validation | `UiTextKeys.DocumentRegistrationValidationSelectDocumentType` |
| `DocumentRegistrationViewModel` | missing display title validation | `UiTextKeys.DocumentRegistrationValidationEnterDisplayTitle` |
| `DocumentRegistrationViewModel` | missing reference date validation | `UiTextKeys.DocumentRegistrationValidationSelectReferenceDate` |
| `PolicyClaimManagementViewModel` | claim created message | `UiTextKeys.ClaimManagementMessageCreated` |
| `PolicyClaimManagementViewModel` | claim disabled message | `UiTextKeys.ClaimManagementMessageDisabled` |
| `PolicyClaimManagementViewModel` | claim title required validation | `UiTextKeys.ClaimManagementValidationTitleRequired` |
| `PolicyClaimManagementViewModel` | policy created message | `UiTextKeys.PolicyManagementMessageCreated` |
| `PolicyClaimManagementViewModel` | policy disabled message | `UiTextKeys.PolicyManagementMessageDisabled` |
| `PolicyClaimManagementViewModel` | policy disable blocked validation | `UiTextKeys.PolicyManagementValidationDisableBlockedByActiveClaims` |
| `PolicyClaimManagementViewModel` | claim create policy selection validation | `UiTextKeys.ClaimManagementValidationSelectPolicyBeforeCreate` |
| `PolicyClaimManagementViewModel` | policy title required validation | `UiTextKeys.PolicyManagementValidationTitleRequired` |
| `PolicyClaimManagementViewModel` | claim selection validation | `UiTextKeys.ClaimManagementValidationSelectClaimTarget` |
| `PolicyClaimManagementViewModel` | policy selection validation | `UiTextKeys.PolicyManagementValidationSelectPolicyTarget` |

## H. Deferred summary formats

- `policy:{policyId}; document:{documentId}`: deferred 유지
- `claim:{claimId}; document:{documentId}`: deferred 유지

## I. Provider injection strategy

- `DocumentRegistrationViewModel` constructor에 `IUiTextProvider`를 추가했다.
- `PolicyClaimManagementViewModel` constructor에 `IUiTextProvider`를 추가했다.
- user-visible runtime message assignment는 `uiTextProvider.Get(UiTextKeys...)`를 사용한다.
- 기존 validation condition, storage/workflow behavior, command behavior는 변경하지 않았다.

## J. AppServices 변경 요약

- `AppServices`에서 shared `ResourceUiTextProvider`를 생성해 두 ViewModel에 전달한다.
- real app runtime에서는 `Application.Current.Resources`를 사용한다.
- test/non-application composition 경로에서는 승인된 runtime message key/value만 담은 fallback dictionary를 사용한다.
- `App.xaml` resource merge 방식은 변경하지 않았다.

## K. Test update 요약

- `DocumentRegistrationViewModelTests`는 provider dependency를 주입하도록 fixture를 보강했다.
- `PolicyClaimManagementViewModelTests`는 provider dependency를 주입하도록 fixture를 보강했다.
- 기존 exact string assertion은 유지했다.
- `ResourceUiTextProviderTests`에 runtime key count와 `UiStrings.xaml` 기반 provider resolve 검증을 추가했다.

## L. Forbidden action 확인

- `MainWindow.xaml` 수정 없음
- `App.xaml` 수정 없음
- `IUiTextProvider.cs` 수정 없음
- `ResourceUiTextProvider.cs` 수정 없음
- resource infrastructure 변경 없음
- culture switching / dynamic language switching 없음
- direct Korean replacement 없음
- 새 Korean translation 없음
- final Korean copy 확정 없음
- deferred diagnostic summary format 추출 없음
- `Ui.BusinessDuplicate.*`, `Ui.Product.*`, `Ui.ActionResult.*` 추가 없음
- DB/SQLite/OCR/repository 구현 없음
- app launch / OpenFileDialog / manual workflow 실행 없음
- cleanup / runtime metadata deletion / runtime attachment deletion 없음
- `data/claimdoc` 접근 없음
- git add/stage/commit 없음

## M. Build/Test results

- `dotnet build FamilyClaimRef.sln`: PASS, warning 0, error 0
- sandbox build: Windows SDK user-profile path access denied
- elevated build rerun: PASS
- `dotnet test FamilyClaimRef.sln --filter FullyQualifiedName~ResourceUiTextProviderTests`: PASS, 11 passed
- `dotnet test FamilyClaimRef.sln --filter FullyQualifiedName~DocumentRegistrationViewModel`: PASS, 25 passed
- `dotnet test FamilyClaimRef.sln --filter FullyQualifiedName~PolicyClaimManagementViewModel`: PASS, 14 passed
- `dotnet test FamilyClaimRef.sln`: PASS, 310 passed

## N. Project root safety results

- project root `attachments/`: files 0
- project root `data/local/`: files 0
- project root `runtime_test_document.*`: files 0
- project root DB/SQLite unexpected files: 0
- `data/claimdoc/`: ignored 확인 대상이며 내부 read/list/use/select/stage/commit/delete/move 없음

## O. Commit candidate exact file list

- `app/FamilyClaimRef.App/Resources/UiStrings.xaml`
- `app/FamilyClaimRef.App/Services/Localization/UiTextKeys.cs`
- `app/FamilyClaimRef.App/ViewModels/DocumentRegistrationViewModel.cs`
- `app/FamilyClaimRef.App/ViewModels/PolicyClaimManagementViewModel.cs`
- `app/FamilyClaimRef.App/Composition/AppServices.cs`
- `tests/FamilyClaimRef.App.Tests/DocumentRegistrationViewModelTests.cs`
- `tests/FamilyClaimRef.App.Tests/PolicyClaimManagementViewModelTests.cs`
- `tests/FamilyClaimRef.App.Tests/Services/Localization/ResourceUiTextProviderTests.cs`
- `docs/246_POLICY_CLAIM_VIEWMODEL_RUNTIME_MESSAGE_EXTRACTION_RESULT_REVIEW.md`

## P. Recommended commit message

```text
refactor(familyclaimref): extract viewmodel runtime messages
```
