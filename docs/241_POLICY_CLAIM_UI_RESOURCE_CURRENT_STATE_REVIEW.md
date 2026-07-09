# Policy Claim UI Resource Current State Review

## A. 상태

```text
POLICY_CLAIM_UI_RESOURCE_CURRENT_STATE_REVIEW_READY
```

이 문서는 현재까지 완료된 FamilyClaimRef UI resource extraction 상태를 하나의 current-state 기준으로 정리한다. 구현 계획, product UI 설계, Korean copy 확정, ViewModel message provider injection, cleanup 계획은 이 문서의 범위가 아니다.

## B. 기준 commit

기준 commit:

```text
26b031f refactor(familyclaimref): extract management static xaml strings
```

최근 흐름:

- `26b031f refactor(familyclaimref): extract management static xaml strings`
- `8c68369 docs(familyclaimref): plan management static xaml string extraction`
- `a570d9a refactor(familyclaimref): extract document registration static xaml strings`
- `aeec44d docs(familyclaimref): plan next static xaml string extraction`
- `478e6cd refactor(familyclaimref): extract validation harness pilot strings`
- `a8f8df8 docs(familyclaimref): plan validation harness pilot string extraction`
- `14f0541 feat(familyclaimref): add ui resource infrastructure`
- `85e274a docs(familyclaimref): plan resource infrastructure implementation`

## C. 검토한 문서

| 문서 | 검토 결과 |
|---|---|
| `docs/235_POLICY_CLAIM_VALIDATION_HARNESS_NEXT_STATIC_XAML_EXTRACTION_RESULT_REVIEW.md` | Document registration static XAML extraction 결과 검토. 13개 key, `StaticResource` mapping, build/test PASS 기록 확인. |
| `docs/240_POLICY_CLAIM_VALIDATION_HARNESS_MANAGEMENT_STATIC_XAML_EXTRACTION_RESULT_REVIEW.md` | Policy/Claim Management static XAML extraction 결과 검토. 14개 key, `StaticResource` mapping, build/test PASS 기록 확인. |
| `docs/218_POLICY_CLAIM_UI_STRING_INVENTORY_REVIEW.md` | 최초 UI string inventory와 ViewModel runtime message 후보 확인. |
| `docs/222_POLICY_CLAIM_RESOURCE_KEY_NAMING_AND_STRING_OWNERSHIP_DECISION.md` | key naming, string ownership, direct Korean replacement 금지 기준 확인. |

## D. 검토한 파일

| 파일 | Read-only inspection 결과 |
|---|---|
| `app/FamilyClaimRef.App/App.xaml` | `Resources/UiStrings.xaml`을 `ResourceDictionary.MergedDictionaries`로 merge한다. |
| `app/FamilyClaimRef.App/Resources/UiStrings.xaml` | 현재 `Ui.*` resource key 32개를 보유한다. |
| `app/FamilyClaimRef.App/Services/Localization/IUiTextProvider.cs` | `Get`과 `Format` 경계를 가진 최소 UI text provider interface가 있다. |
| `app/FamilyClaimRef.App/Services/Localization/ResourceUiTextProvider.cs` | `ResourceDictionary` 또는 dictionary 기반 lookup, missing-key fallback, non-string reject를 제공한다. |
| `app/FamilyClaimRef.App/Services/Localization/UiTextKeys.cs` | 현재 `Ui.*` constant 32개를 보유한다. |
| `app/FamilyClaimRef.App/MainWindow.xaml` | 현재 `StaticResource Ui.*` 참조 33곳을 사용한다. `Ui.App.Title`은 window title과 top label 두 곳에서 사용된다. |
| `tests/FamilyClaimRef.App.Tests/Services/Localization/ResourceUiTextProviderTests.cs` | provider lookup, fallback, validation, formatting, pilot key existence, ResourceDictionary behavior를 검증한다. |

## E. Resource infrastructure current state

- `UiStrings.xaml`은 WPF `ResourceDictionary`로 존재한다.
- `App.xaml`은 `Resources/UiStrings.xaml`을 merge한다.
- `IUiTextProvider`는 `Get`과 `Format`만 제공하는 최소 interface다.
- `ResourceUiTextProvider`는 WPF `ResourceDictionary`와 `IReadOnlyDictionary<string, string>` 기반 lookup을 지원한다.
- missing key는 `[[key]]` 형식 fallback으로 반환한다.
- non-string resource value는 `InvalidOperationException`으로 거부한다.
- culture switching, dynamic language switching, final Korean copy switching은 구현되어 있지 않다.

## F. Extracted key inventory

현재 `UiStrings.xaml`과 `UiTextKeys.cs`에서 확인한 extracted key count:

```text
expected extracted key count: 32
verified extracted key count in UiStrings.xaml: 32
verified extracted key count in UiTextKeys.cs: 32
```

### F-1. Pilot extraction keys

| Key | Current value |
|---|---|
| `Ui.App.Title` | `FamilyClaimRef` |
| `Ui.Document.SourceFileSection` | `Source file` |
| `Ui.Action.SelectFile` | `Select file` |
| `Ui.Status.RegistrationSection` | `Registration status` |
| `Ui.DevHarness.Warning.LocalMvpValidation` | `Local MVP validation screen. Do not use real personal, insurer, hospital, diagnosis, policy number, or claim number samples.` |

### F-2. Document registration static keys

| Key | Current value |
|---|---|
| `Ui.Document.SelectedFileLabel` | `Selected file` |
| `Ui.Target.SelectionSection` | `Target selection` |
| `Ui.Target.KindLabel` | `Target kind` |
| `Ui.Policy.TargetLabel` | `Policy target` |
| `Ui.Claim.TargetLabel` | `Claim target` |
| `Ui.Document.MetadataSection` | `Document metadata` |
| `Ui.Document.TypeLabel` | `Document type` |
| `Ui.Document.DisplayTitleLabel` | `Display title` |
| `Ui.Document.ReferenceDateLabel` | `Reference date` |
| `Ui.Action.RegisterDocument` | `Register` |
| `Ui.Validation.SectionLabel` | `Validation` |
| `Ui.Status.Label` | `Status` |
| `Ui.Status.LastRegistrationSummaryLabel` | `Last registration summary` |

### F-3. Policy/Claim Management static keys

| Key | Current value |
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

## G. MainWindow.xaml StaticResource 적용 범위

`MainWindow.xaml`은 현재 validation harness 화면의 주요 label/button/header static literal을 `StaticResource`로 참조한다.

- window title과 top label은 `Ui.App.Title`을 사용한다.
- local MVP warning은 `Ui.DevHarness.Warning.LocalMvpValidation`을 사용한다.
- source file, target selection, document metadata, registration status 영역의 static label/button/header는 resource key를 사용한다.
- Policy/Claim Management 영역의 static label/button/header 14개는 resource key를 사용한다.
- `Ui.App.Title`이 두 곳에서 사용되므로 `StaticResource Ui.*` 참조 수는 33곳이고, unique key 수는 32개다.

## H. 남은 hard-coded / deferred 영역

다음 항목은 아직 resource extraction 범위 밖으로 남아 있다.

- `MainWindow.xaml`의 target kind ComboBox value `policy`, `claim`
- `MainWindow.xaml`의 `StringFormat=Is busy: {0}`
- `DocumentRegistrationViewModel` validation/status/runtime messages
- `PolicyClaimManagementViewModel` runtime management messages
- `Ui.Management.Message.*` runtime key family
- `Ui.Policy.Created`, `Ui.Policy.Disabled` 계열 runtime message key
- `Ui.Claim.Created`, `Ui.Claim.Disabled` 계열 runtime message key
- `Ui.BusinessDuplicate.*`
- `Ui.Product.*`
- final Korean copy
- culture switching / dynamic language switching
- product UI shell / UI redesign

## I. Last known validation state

마지막 known PASS는 `docs/240` 기준이다.

- elevated `dotnet build FamilyClaimRef.sln`: PASS, warning 0, error 0.
- elevated `dotnet test FamilyClaimRef.sln --filter FullyQualifiedName~ResourceUiTextProviderTests`: PASS, total 9, failed 0.
- elevated `dotnet test FamilyClaimRef.sln`: PASS, total 306, failed 0.
- `git diff --check`: PASS.
- exact changed files trailing whitespace scan: PASS.
- exact changed files actual personal/sample scan: PASS.
- `git check-ignore -v -- data/claimdoc/`: PASS.
- `git check-ignore -v -- docs/nightwork_20260706/`: PASS.
- cleanup, runtime metadata deletion, runtime attachment deletion은 실행하지 않았다.
- `data/claimdoc` 접근은 수행하지 않았다.
- DB/SQLite/OCR/repository 구현은 수행하지 않았다.

이번 문서 batch에서는 build/test를 재실행하지 않는다. 현재 문서는 documentation-only current-state review다.

## J. Explicit non-scope

- code 수정 없음
- test 수정 없음
- XAML 수정 없음
- ViewModel 수정 없음
- resource 수정 없음
- `App.xaml` 수정 없음
- `UiStrings.xaml` 수정 없음
- `UiTextKeys.cs` 수정 없음
- `IUiTextProvider` 수정 없음
- `ResourceUiTextProvider` 수정 없음
- `ResourceUiTextProviderTests` 수정 없음
- localization 구현 없음
- culture switching 없음
- dynamic language switching 없음
- direct Korean replacement 없음
- final Korean copy 없음
- ViewModel message provider injection 없음
- runtime message extraction 없음
- business duplicate rule/copy 없음
- wireframe port 없음
- UI redesign 없음
- product UI shell 없음
- app launch 없음
- OpenFileDialog 없음
- manual workflow 없음
- cleanup 없음
- runtime metadata deletion 없음
- runtime attachment deletion 없음
- `data/claimdoc` 접근 없음
- DB/SQLite/OCR/repository 구현 없음
- git add/stage 없음
- commit 없음

## K. Current-state judgment

- static XAML resource extraction baseline is consolidated.
- resource infrastructure remains minimal.
- no culture switching implementation exists.
- no final Korean copy decision exists.
- no ViewModel message provider injection exists.
- no cleanup executed.
- no data/claimdoc access executed.
- no DB/SQLite/OCR/repository implementation exists.

## L. Next recommended work

다음 후보 순서를 권장한다.

1. `docs/241` exact commit
2. ViewModel runtime message extraction planning
3. final Korean copy strategy planning
4. Scenario 9 cleanup policy review

이 문서는 위 후속 구현을 자동 승인하지 않는다. 각 후속 작업은 별도 decision 또는 implementation batch가 필요하다.

## M. Commit candidate

이번 batch에서는 commit하지 않는다.

Exact file list:

- `docs/241_POLICY_CLAIM_UI_RESOURCE_CURRENT_STATE_REVIEW.md`

Recommended commit message:

```text
docs(familyclaimref): consolidate ui resource current state
```

Commit readiness:

```text
ready, if final git status contains only docs/241 untracked
blocked, if any other file changed
```
