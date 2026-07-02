# WPF ViewModel / File Picker Boundary User Decision Record

## A. Goal

이 문서는 `docs/100_WPF_VIEWMODEL_FILE_PICKER_BOUNDARY_DESIGN.md`의 사용자 결정 기록이다.

목적은 WPF ViewModel/file picker boundary의 책임 범위와 후속 구현 방향을 확정하는 것이다.

이 문서는 구현 문서가 아니다.

- C# 구현은 하지 않는다.
- ViewModel 구현은 하지 않는다.
- file picker 구현은 하지 않는다.
- XAML 수정은 하지 않는다.
- service composition 구현은 하지 않는다.
- test code 구현은 하지 않는다.

## B. Checked Files / Paths

| Path | Purpose | Result |
|---|---|---|
| `docs/100_WPF_VIEWMODEL_FILE_PICKER_BOUNDARY_DESIGN.md` | 결정 기준 문서 확인 | Checked |
| `docs/99_IMPORT_LINK_COMBINED_WORKFLOW_IMPLEMENTATION_REVIEW.md` | workflow 구현 결과 확인 | Checked |
| `docs/98_IMPORT_LINK_COMBINED_WORKFLOW_USER_DECISION_RECORD.md` | import + link 사용자 결정 확인 | Checked |
| `docs/96_POLICY_CLAIM_DOCUMENT_LINK_WORKFLOW_IMPLEMENTATION_REVIEW.md` | link workflow 구현 경계 확인 | Checked |
| `docs/93_DOCUMENT_ATTACHMENT_COORDINATOR_IMPLEMENTATION_REVIEW.md` | attachment coordinator 경계 확인 | Checked |
| `docs/90_FILE_ATTACHMENT_SERVICE_IMPLEMENTATION_REVIEW.md` | file attachment service 경계 확인 | Checked |
| `docs/87_JSON_STORAGE_IMPLEMENTATION_REVIEW.md` | JSON metadata storage 경계 확인 | Checked |
| `app/FamilyClaimRef.App/Services/Storage/DocumentRegistrationWorkflow.cs` | UI 호출 대상 workflow 확인 | Checked |
| `app/FamilyClaimRef.App/Services/Storage/PolicyDocumentRegistrationRequest.cs` | policy request shape 확인 | Checked |
| `app/FamilyClaimRef.App/Services/Storage/ClaimDocumentRegistrationRequest.cs` | claim request shape 확인 | Checked |
| `app/FamilyClaimRef.App/Services/Storage/PolicyDocumentRegistrationResult.cs` | policy result shape 확인 | Checked |
| `app/FamilyClaimRef.App/Services/Storage/ClaimDocumentRegistrationResult.cs` | claim result shape 확인 | Checked |
| `app/FamilyClaimRef.App/App.xaml` | WPF entry 확인 | Checked |
| `app/FamilyClaimRef.App/App.xaml.cs` | WPF app class 확인 | Checked |
| `app/FamilyClaimRef.App/MainWindow.xaml` | WPF window 확인 | Checked |
| `app/FamilyClaimRef.App/MainWindow.xaml.cs` | WPF window code-behind 확인 | Checked |
| `FamilyClaimRef.sln` | solution 확인 | Checked |

## C. User Decision Summary

| ID | Question | Decision | Recorded Direction |
|---|---|---|---|
| Q1 | WPF UI/ViewModel boundary를 설계할 것인가? | Accepted | WPF UI/ViewModel boundary를 설계하고 후속 구현 대상으로 둔다. |
| Q2 | ViewModel은 `DocumentRegistrationWorkflow`만 workflow 진입점으로 호출하게 할 것인가? | Accepted | ViewModel은 document registration workflow 진입점으로 `DocumentRegistrationWorkflow`만 호출한다. |
| Q3 | ViewModel은 lower-level coordinator/storage/file service를 직접 조합하지 않게 할 것인가? | Accepted | ViewModel은 `DocumentAttachmentCoordinator`, `DocumentLinkCoordinator`, storage/file service, `FileNamePolicyService`를 직접 조합하지 않는다. |
| Q4 | file picker는 ViewModel 직접 `OpenFileDialog` 호출이 아니라 별도 boundary로 분리할 것인가? | Accepted | ViewModel은 `OpenFileDialog`를 직접 호출하지 않고 file picker boundary를 둔다. |
| Q5 | `IFilePickerService` abstraction을 도입할 것인가? | Accepted | `IFilePickerService` abstraction을 도입한다. |
| Q6 | file picker cancel은 no-op으로 처리할 것인가? | Accepted | file picker cancel은 오류가 아닌 사용자 취소 상태로 보고 no-op 처리한다. |
| Q7 | ViewModel은 source file path, target kind, target id, documentType, displayTitle, referenceDate를 상태로 가질 것인가? | Accepted | ViewModel은 registration form 상태와 busy/status/result 상태를 가진다. |
| Q8 | Policy/Claim storage가 없으므로 policyId/claimId는 MVP에서 manual dummy input 후보로 둘 것인가? | Accepted | MVP에서는 policyId/claimId를 manual dummy input 후보로 둔다. |
| Q9 | production metadata root 후보를 `data/local/`로 유지할 것인가? | Accepted | production metadata root 후보를 `data/local/`로 둔다. |
| Q10 | production attachment root 후보를 `attachments/`로 유지할 것인가? | Accepted | production attachment root 후보를 `attachments/`로 둔다. |
| Q11 | service composition은 App startup 또는 별도 composition root 후보로 둘 것인가? | Accepted | App startup 또는 별도 composition root 후보로 둔다. |
| Q12 | MVP error 표시는 간단한 user message로 변환하고 custom exception 분류는 보류할 것인가? | Accepted | MVP에서는 간단한 user message로 변환하고 custom exception 분류는 보류한다. |
| Q13 | 이후 구현 시 ViewModel/file picker tests를 추가할 것인가? | Accepted | 후속 구현 시 ViewModel/file picker boundary tests를 추가한다. |
| Q14 | actual `OpenFileDialog`는 unit test에서 호출하지 않을 것인가? | Accepted | unit test에서는 actual `OpenFileDialog`를 호출하지 않고 fake implementation으로 검증한다. |
| Q15 | WPF UI/XAML 구현은 boundary 결정 후 별도 단계로 진행할 것인가? | Accepted | WPF UI/XAML 구현은 boundary 결정 이후 별도 단계로 진행한다. |

## D. Accepted WPF Boundary Direction

확정 방향:

- WPF ViewModel/file picker boundary를 진행한다.
- ViewModel은 `DocumentRegistrationWorkflow`만 workflow 진입점으로 호출한다.
- ViewModel은 lower-level coordinator/storage/file service를 직접 조합하지 않는다.
- file picker는 ViewModel 직접 `OpenFileDialog` 호출이 아니라 별도 boundary로 분리한다.
- `IFilePickerService` abstraction을 도입한다.
- file picker cancel은 no-op으로 처리한다.
- ViewModel은 source file path, target kind, target id, documentType, displayTitle, referenceDate 상태를 가진다.
- policyId/claimId는 MVP에서 manual dummy input 후보로 둔다.
- production metadata root 후보는 `data/local/`로 둔다.
- production attachment root 후보는 `attachments/`로 둔다.
- service composition은 App startup 또는 별도 composition root 후보로 둔다.
- MVP error 표시는 간단한 user message로 변환한다.
- custom exception 분류는 보류한다.
- 후속 구현 시 ViewModel/file picker tests를 추가한다.
- actual `OpenFileDialog`는 unit test에서 호출하지 않는다.
- WPF UI/XAML 구현은 별도 단계로 둔다.

## E. Implementation Candidate Files

후속 구현 후보 파일:

- `app/FamilyClaimRef.App/Services/UI/IFilePickerService.cs`
- `app/FamilyClaimRef.App/Services/UI/FilePickerResult.cs`
- `app/FamilyClaimRef.App/Services/UI/WpfFilePickerService.cs`
- `app/FamilyClaimRef.App/ViewModels/DocumentRegistrationViewModel.cs`
- `tests/FamilyClaimRef.App.Tests/DocumentRegistrationViewModelTests.cs`

선택 후보:

- `app/FamilyClaimRef.App/Composition/AppServices.cs`

주의:

- 이 문서에서는 위 파일을 생성하지 않는다.
- 실제 생성은 별도 구현 승인 이후 진행한다.

## F. ViewModel State Candidate

후속 구현 후보 상태:

- `SelectedSourceFilePath`
- `SelectedSourceFileDisplayName`
- `TargetKind`
- `TargetId`
- `DocumentType`
- `DisplayTitle`
- `ReferenceDate`
- `IsBusy`
- `ValidationMessage`
- `StatusMessage`
- `LastRegistrationSummary`

주의:

- ViewModel은 physical file name을 직접 생성하지 않는다.
- ViewModel은 duplicateIndex를 직접 계산하지 않는다.
- ViewModel은 rollback state를 직접 관리하지 않는다.
- ViewModel은 lower-level service를 직접 호출하지 않는다.
- ViewModel은 `OpenFileDialog`를 직접 호출하지 않는다.

## G. File Picker Shape Candidate

`IFilePickerService` 후보:

```csharp
public interface IFilePickerService
{
    Task<FilePickerResult?> PickDocumentFileAsync(
        CancellationToken cancellationToken = default);
}
```

`FilePickerResult` 후보:

```csharp
public sealed record FilePickerResult(
    string SourceFilePath,
    string SafeDisplayName);
```

주의:

- `SourceFilePath`는 workflow 입력값이다.
- `SafeDisplayName`은 UI 표시값이다.
- raw original file name은 metadata에 저장하지 않는다.
- actual file copy는 file picker가 수행하지 않는다.
- actual file copy는 `DocumentRegistrationWorkflow` 하위 service가 수행한다.

## H. Still Not Implemented

아직 구현하지 않은 항목:

- C# 구현 없음.
- C# 수정 없음.
- production C# 수정 없음.
- ViewModel 구현 없음.
- file picker 구현 없음.
- actual `OpenFileDialog` 실행 없음.
- XAML 수정 없음.
- route/navigation 수정 없음.
- service composition 구현 없음.
- workflow/coordinator 수정 없음.
- JSON metadata storage 수정 없음.
- file attachment service 수정 없음.
- `FileNamePolicyService` 수정 없음.
- test code 수정 없음.
- test file 생성 없음.
- Policy/Claim storage 구현 없음.
- OCR 구현 없음.
- SQLite DB/package 추가 없음.
- repository/data access/migration 구현 없음.
- project root `attachments/` 내부 파일 생성 없음.
- project root `data/local` 내부 파일 생성 없음.
- 실제 개인정보 샘플 없음.
- `.sln` 수정 없음.
- `.csproj` 수정 없음.
- NuGet package 추가 없음.
- Git commit/reset/checkout/add 없음.

## I. Next Step

다음 작업 후보:

1. 별도 승인 후 ViewModel/file picker interface/test 구현 범위 결정 또는 바로 구현.
2. 구현 후보 파일:
   - `IFilePickerService.cs`
   - `FilePickerResult.cs`
   - `WpfFilePickerService.cs`
   - `DocumentRegistrationViewModel.cs`
   - `DocumentRegistrationViewModelTests.cs`
3. service composition 후보:
   - `AppServices.cs`
4. XAML UI 구현은 ViewModel boundary 구현 후 별도 진행.
5. Policy/Claim storage와 target selection은 이후 별도 설계.

## J. Result

`WPF_VIEWMODEL_FILE_PICKER_BOUNDARY_USER_DECISION_RECORDED`
