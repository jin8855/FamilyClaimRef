# Policy Claim Validation Harness Next Static XAML Extraction Result Review

## A. 상태 마커

POLICY_CLAIM_VALIDATION_HARNESS_NEXT_STATIC_XAML_EXTRACTION_COMPLETED

## B. 기준 commit 및 기준 문서

- 기준 commit: `aeec44d docs(familyclaimref): plan next static xaml string extraction`
- 기준 문서:
  - `docs/231_POLICY_CLAIM_VALIDATION_HARNESS_NEXT_STATIC_XAML_EXTRACTION_SCOPE_PLAN.md`
  - `docs/232_POLICY_CLAIM_VALIDATION_HARNESS_NEXT_STATIC_XAML_RESOURCE_KEY_PLAN.md`
  - `docs/233_POLICY_CLAIM_VALIDATION_HARNESS_NEXT_STATIC_XAML_EXTRACTION_TEST_PLAN.md`
  - `docs/234_POLICY_CLAIM_VALIDATION_HARNESS_NEXT_STATIC_XAML_EXTRACTION_COMMIT_CANDIDATE_REVIEW.md`

## C. 변경 파일

- `app/FamilyClaimRef.App/Resources/UiStrings.xaml`
- `app/FamilyClaimRef.App/Services/Localization/UiTextKeys.cs`
- `app/FamilyClaimRef.App/MainWindow.xaml`

## D. 생성 문서

- `docs/235_POLICY_CLAIM_VALIDATION_HARNESS_NEXT_STATIC_XAML_EXTRACTION_RESULT_REVIEW.md`

## E. 추가 Resource Key

이번 작업에서는 승인된 13개 정적 XAML 문구만 resource key로 분리했다.
resource value는 기준 문서의 영문 값을 그대로 유지했다.

| Key | Value |
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

## F. XAML Mapping

`MainWindow.xaml`에서는 승인된 literal 13개만 `{StaticResource ...}` 참조로 바꿨다.
화면의 최종 한국어 문구 확정, culture switching, dynamic language switching은 구현하지 않았다.

| Previous literal | Applied resource |
|---|---|
| `Selected file` | `{StaticResource Ui.Document.SelectedFileLabel}` |
| `Target selection` | `{StaticResource Ui.Target.SelectionSection}` |
| `Target kind` | `{StaticResource Ui.Target.KindLabel}` |
| `Policy target` | `{StaticResource Ui.Policy.TargetLabel}` |
| `Claim target` | `{StaticResource Ui.Claim.TargetLabel}` |
| `Document metadata` | `{StaticResource Ui.Document.MetadataSection}` |
| `Document type` | `{StaticResource Ui.Document.TypeLabel}` |
| `Display title` | `{StaticResource Ui.Document.DisplayTitleLabel}` |
| `Reference date` | `{StaticResource Ui.Document.ReferenceDateLabel}` |
| `Register` | `{StaticResource Ui.Action.RegisterDocument}` |
| `Validation` | `{StaticResource Ui.Validation.SectionLabel}` |
| `Status` | `{StaticResource Ui.Status.Label}` |
| `Last registration summary` | `{StaticResource Ui.Status.LastRegistrationSummaryLabel}` |

## G. 명시적 제외 범위

- `policy` / `claim` `ComboBoxItem` 값은 resource로 분리하지 않았다.
- `Is busy: {0}` 문구는 resource로 분리하지 않았다.
- 기존 `Registration status` resource key는 변경하지 않았다.
- Policy/Claim Management section 문구는 이번 범위에서 제외했다.
- ViewModel validation/status message는 변경하지 않았다.
- `UiStrings.xaml` value를 한국어로 바꾸지 않았다.
- `MainWindow.xaml`에 한국어 literal을 직접 삽입하지 않았다.
- localization culture switching을 구현하지 않았다.
- dynamic language switching을 구현하지 않았다.
- `App.xaml`, `IUiTextProvider`, `ResourceUiTextProvider`는 변경하지 않았다.
- XAML layout, control hierarchy, binding, command binding은 변경하지 않았다.
- business duplicate rule/copy는 구현하지 않았다.
- wireframe port, UI redesign, product UI shell은 구현하지 않았다.
- app launch, OpenFileDialog, manual workflow는 실행하지 않았다.
- cleanup, runtime metadata deletion, runtime attachment deletion은 실행하지 않았다.
- DB/SQLite/OCR/repository는 구현하지 않았다.
- `data/claimdoc` 접근은 수행하지 않았다.

## H. 검증 결과

- 최초 sandbox `dotnet build FamilyClaimRef.sln`: Windows SDK user-profile path access boundary로 실패했다.
- elevated `dotnet build FamilyClaimRef.sln`: PASS, warning 0, error 0.
- 최초 sandbox targeted resource test: 동일한 Windows SDK access boundary로 실패했다.
- elevated `dotnet test FamilyClaimRef.sln --filter FullyQualifiedName~ResourceUiTextProviderTests`: PASS, total 9, failed 0.
- elevated `dotnet test FamilyClaimRef.sln`: PASS, total 306, failed 0.
- `git diff --check`: PASS.
- exact changed files trailing whitespace scan: PASS.
- exact changed files local profile path scan: PASS.
- exact changed files actual sample-data scan: PASS.
- `git check-ignore -v -- data/claimdoc/`: PASS.
- `git check-ignore -v -- docs/nightwork_20260706/`: PASS.
- project root `attachments/`: files 0.
- project root `data/local/`: files 0.
- project root `runtime_test_document.*`: missing.
- DB/SQLite unexpected file: none.

## I. Commit Candidate

Exact file list:

- `app/FamilyClaimRef.App/Resources/UiStrings.xaml`
- `app/FamilyClaimRef.App/Services/Localization/UiTextKeys.cs`
- `app/FamilyClaimRef.App/MainWindow.xaml`
- `docs/235_POLICY_CLAIM_VALIDATION_HARNESS_NEXT_STATIC_XAML_EXTRACTION_RESULT_REVIEW.md`

Recommended commit message:

```text
refactor(familyclaimref): extract document registration static xaml strings
```

## J. 잔여 위험

- Policy/Claim Management section 문구는 아직 hard-coded 상태이며, 별도 extraction batch가 필요하다.
- ViewModel 동적 message는 아직 resource provider 경계에 연결하지 않았다.
- UI redesign은 계속 deferred 상태다.
