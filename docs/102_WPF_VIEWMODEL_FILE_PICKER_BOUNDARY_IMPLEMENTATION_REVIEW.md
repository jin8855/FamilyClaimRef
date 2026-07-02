# WPF ViewModel / File Picker Boundary Implementation Review

## A. Goal

이 문서는 WPF ViewModel/file picker boundary 구현 결과 리뷰 문서다.

기록 대상:

- `IFilePickerService` 구현 결과.
- `FilePickerResult` 구현 결과.
- `WpfFilePickerService` 구현 결과.
- `DocumentRegistrationViewModel` 구현 결과.
- `DocumentRegistrationViewModelTests` 구현 결과.
- 구현 범위 준수 여부.
- build/test 검증 결과.
- 남은 위험과 다음 추천 작업.

이 문서는 다음 구현 리뷰가 아니다.

- XAML UI 구현 리뷰가 아니다.
- App composition 구현 리뷰가 아니다.
- Policy/Claim storage 구현 리뷰가 아니다.
- OCR, SQLite, repository 구현 리뷰가 아니다.

## B. Checked Files / Paths

| Path | Purpose | Result |
|---|---|---|
| `docs/101_WPF_VIEWMODEL_FILE_PICKER_BOUNDARY_USER_DECISION_RECORD.md` | 사용자 결정 기준 확인 | Checked |
| `docs/100_WPF_VIEWMODEL_FILE_PICKER_BOUNDARY_DESIGN.md` | 설계 기준 확인 | Checked |
| `docs/99_IMPORT_LINK_COMBINED_WORKFLOW_IMPLEMENTATION_REVIEW.md` | workflow 구현 결과 확인 | Checked |
| `app/FamilyClaimRef.App/Services/UI/IFilePickerService.cs` | file picker abstraction 확인 | Checked |
| `app/FamilyClaimRef.App/Services/UI/FilePickerResult.cs` | file picker result 확인 | Checked |
| `app/FamilyClaimRef.App/Services/UI/WpfFilePickerService.cs` | WPF file picker 구현체 확인 | Checked |
| `app/FamilyClaimRef.App/ViewModels/DocumentRegistrationViewModel.cs` | ViewModel 구현 확인 | Checked |
| `tests/FamilyClaimRef.App.Tests/DocumentRegistrationViewModelTests.cs` | ViewModel tests 확인 | Checked |
| `app/FamilyClaimRef.App/Services/Storage/DocumentRegistrationWorkflow.cs` | ViewModel 호출 대상 확인 | Checked |
| `app/FamilyClaimRef.App/App.xaml` | WPF entry 수정 여부 확인 | Checked |
| `app/FamilyClaimRef.App/App.xaml.cs` | WPF app class 수정 여부 확인 | Checked |
| `app/FamilyClaimRef.App/MainWindow.xaml` | WPF window 수정 여부 확인 | Checked |
| `app/FamilyClaimRef.App/MainWindow.xaml.cs` | WPF window code-behind 수정 여부 확인 | Checked |
| `FamilyClaimRef.sln` | build/test 대상 확인 | Checked |
| `app/FamilyClaimRef.App/FamilyClaimRef.App.csproj` | SDK-style include 확인 | Checked |
| `tests/FamilyClaimRef.App.Tests/FamilyClaimRef.App.Tests.csproj` | test project 확인 | Checked |

## C. Implementation Summary

- `IFilePickerService.cs` 생성 확인.
- `FilePickerResult.cs` 생성 확인.
- `WpfFilePickerService.cs` 생성 확인.
- `DocumentRegistrationViewModel.cs` 생성 확인.
- `DocumentRegistrationViewModelTests.cs` 생성 확인.
- `DocumentRegistrationViewModel`은 `DocumentRegistrationWorkflow`와 `IFilePickerService`만 직접 의존한다.
- ViewModel은 `DocumentAttachmentCoordinator`, `DocumentLinkCoordinator`, storage/file service, `FileNamePolicyService`를 직접 조합하지 않는다.
- file picker cancel no-op이 구현되어 있다.
- registration state가 구현되어 있다.
- validation/status/busy/result summary가 구현되어 있다.
- MVP user message 변환이 구현되어 있다.
- fake file picker 기반 ViewModel tests가 구현되어 있다.
- ViewModel tests는 temp directory만 사용한다.
- actual `OpenFileDialog` unit test 실행은 없다.
- XAML 수정은 없다.
- `App.xaml`, `App.xaml.cs`, `MainWindow.xaml`, `MainWindow.xaml.cs` 수정은 없다.
- service composition 구현은 없다.
- `AppServices.cs` 생성은 없다.

## D. File Picker Boundary Review

확인 결과:

- `IFilePickerService.PickDocumentFileAsync(...)`가 존재한다.
- `FilePickerResult`는 `SourceFilePath`, `SafeDisplayName`을 포함한다.
- `WpfFilePickerService`는 `OpenFileDialog`를 감싼 production implementation이다.
- 사용자가 cancel하면 `null`을 반환한다.
- 파일 선택 성공 시 `FilePickerResult`를 반환한다.
- `SafeDisplayName`은 `Path.GetFileName(...)` 기준이다.
- file picker는 actual file copy를 수행하지 않는다.
- file picker는 JSON metadata를 저장하지 않는다.
- file picker는 `DocumentRegistrationWorkflow`를 호출하지 않는다.
- unit test에서는 actual `OpenFileDialog`를 호출하지 않는다.

보완 메모:

- filter는 UX 보조이며 최종 validation은 workflow/FileNamePolicyService 계층에 남아 있다.

## E. ViewModel Review

확인 결과:

- constructor에서 `DocumentRegistrationWorkflow` null guard가 있다.
- constructor에서 `IFilePickerService` null guard가 있다.
- ViewModel은 `DocumentRegistrationWorkflow`와 `IFilePickerService`만 직접 의존한다.
- ViewModel은 `OpenFileDialog`를 직접 호출하지 않는다.
- ViewModel은 `DocumentAttachmentCoordinator`를 직접 호출하지 않는다.
- ViewModel은 `DocumentLinkCoordinator`를 직접 호출하지 않는다.
- ViewModel은 `JsonDocumentStorageService`를 직접 호출하지 않는다.
- ViewModel은 `LocalFileAttachmentService`를 직접 호출하지 않는다.
- ViewModel은 `FileNamePolicyService`를 직접 호출하지 않는다.
- `INotifyPropertyChanged`가 구현되어 있다.
- `SelectedSourceFilePath` 상태가 구현되어 있다.
- `SelectedSourceFileDisplayName` 상태가 구현되어 있다.
- `TargetKind` 상태가 구현되어 있다.
- `TargetId` 상태가 구현되어 있다.
- `DocumentType` 상태가 구현되어 있다.
- `DisplayTitle` 상태가 구현되어 있다.
- `ReferenceDate` 상태가 구현되어 있다.
- `IsBusy` 상태가 구현되어 있다.
- `ValidationMessage` 상태가 구현되어 있다.
- `StatusMessage` 상태가 구현되어 있다.
- `LastRegistrationSummary` 상태가 구현되어 있다.
- file select method가 구현되어 있다.
- register method가 구현되어 있다.
- policy registration request 생성이 구현되어 있다.
- claim registration request 생성이 구현되어 있다.
- success status 처리가 구현되어 있다.
- failure status 처리가 구현되어 있다.
- busy state finally 처리가 구현되어 있다.

## F. Validation / Status Review

확인 결과:

- missing source file path validation이 있다.
- invalid target kind validation이 있다.
- missing target id validation이 있다.
- missing documentType validation이 있다.
- missing displayTitle validation이 있다.
- default reference date validation이 있다.
- validation failure 시 workflow를 호출하지 않는다.
- success 시 user message를 설정한다.
- workflow failure 시 간단한 user message를 설정한다.
- `AggregateException`은 cleanup failure user message로 분리한다.
- 상세 exception message를 사용자 메시지로 그대로 노출하지 않는다.
- custom exception은 만들지 않았다.

## G. Test Coverage Review

`DocumentRegistrationViewModelTests.cs` 기준 추가 test method 수는 15개다.

### Constructor

확인:

- null workflow rejected.
- null file picker rejected.

### File select

확인:

- selecting file updates selected file path.
- selecting file updates display name.
- cancel file picker no-op.
- cancel file picker does not set error.

### Validation

확인:

- missing source file path rejected before workflow success.
- missing target id rejected.
- missing documentType rejected.
- missing displayTitle rejected.
- invalid target kind rejected.
- default reference date rejected.

### Register success

확인:

- register policy document succeeds with temp services.
- register claim document succeeds with temp services.
- workflow success updates status message.
- workflow success updates last registration summary.
- `IsBusy` returns false after success.

### Register failure

확인:

- workflow failure updates error status message.
- `AggregateException` maps to cleanup failure user message.
- `IsBusy` returns false after failure.

### Boundary

확인:

- fake file picker is used in tests.
- actual `OpenFileDialog` is not called in tests.
- no WPF visual automation test.
- no XAML binding test.
- no actual project `attachments/`, `data/local` file creation.
- temp directory only.

## H. Verification Result

검증 명령:

```powershell
dotnet build FamilyClaimRef.sln
dotnet test FamilyClaimRef.sln
```

검증 결과:

- `dotnet build FamilyClaimRef.sln`: PASS
- warning: 0
- error: 0
- `dotnet test FamilyClaimRef.sln`: PASS
- 총 테스트 수: 216
- 추가 테스트 수: 15
- 실패 테스트: 없음
- 실패 원인: 없음
- 권한 상승 실행 여부: 있음
- 초기 실패/재시도 여부: 이번 리뷰 검증에서는 없음
- 참고: 구현 당시에는 `WpfFilePickerService.cs`의 `System.IO.Path` using 누락으로 1회 build 실패 후 수정 및 재시도 성공 기록이 있다.
- actual `OpenFileDialog` unit test 실행 여부: 없음
- project root `attachments/` 상태: files=0
- project root `data/local` 상태: files=0
- temp directory만 사용 여부: 확인
- SQLite/DB 파일 생성: 없음
- Git 상태: 현재 경로가 Git 저장소가 아니어서 `git status --short` 실패.

## I. Scope Compliance Review

범위 준수 확인:

- XAML 수정 없음.
- `App.xaml` 수정 없음.
- `App.xaml.cs` 수정 없음.
- `MainWindow.xaml` 수정 없음.
- `MainWindow.xaml.cs` 수정 없음.
- service composition 구현 없음.
- `AppServices.cs` 생성 없음.
- workflow/coordinator/storage/file service 수정 없음.
- `DocumentRegistrationWorkflow.cs` 수정 없음.
- `DocumentAttachmentCoordinator.cs` 수정 없음.
- `DocumentLinkCoordinator.cs` 수정 없음.
- JSON metadata storage 수정 없음.
- file attachment service 수정 없음.
- `FileNamePolicyService.cs` 수정 없음.
- existing test file 수정 없음.
- Policy/Claim storage 구현 없음.
- actual `OpenFileDialog` test 실행 없음.
- WPF UI automation test 없음.
- OCR/SQLite/repository 구현 없음.
- project root `attachments/`, `data/local` 내부 파일 생성 없음.
- 실제 개인정보 샘플 사용 없음.
- `.sln` 수정 없음.
- `.csproj` 수정 없음.
- NuGet package 추가 없음.
- Git commit/reset/checkout/add 없음.

## J. Out of Scope / Not Implemented

아직 구현하지 않은 항목:

- XAML UI binding 없음.
- `App.xaml` / `MainWindow.xaml` 연결 없음.
- service composition 없음.
- `AppServices.cs` 없음.
- production root path 실제 연결 없음.
- Policy/Claim storage 없음.
- target id는 manual input 상태.
- actual WPF file picker runtime 동작은 unit test에서 검증하지 않음.
- custom exception 기반 error classification 없음.
- UI navigation 없음.
- visual styling 없음.

## K. Risks

남은 위험:

- XAML UI binding이 아직 없다.
- service composition이 없어 실제 app에서 ViewModel이 연결되지 않았다.
- production root path 실제 연결이 아직 없다.
- Policy/Claim storage가 없어 target id는 manual input 상태다.
- custom exception 기반 error classification은 아직 단순하다.
- actual WPF file picker runtime 동작은 unit test에서 검증하지 않았다.
- UI에서 실제 file picker를 열면 production path 권한 문제가 새로 드러날 수 있다.
- ViewModel과 XAML binding 연결 시 command/property naming mismatch 가능성이 있다.

## L. Recommendation

추천 순서:

1. 현재 ViewModel/file picker boundary implementation은 build/test PASS 상태로 유지한다.
2. 다음 작업은 service composition/root path 설계 문서가 적절하다.
3. 이후 ViewModel을 MainWindow에 연결하는 XAML/UI 설계를 진행한다.
4. actual `OpenFileDialog` runtime 동작은 manual/WPF runtime 검증 단계로 둔다.
5. Policy/Claim storage와 target selection은 이후 별도 설계로 둔다.

## M. Result

`WPF_VIEWMODEL_FILE_PICKER_BOUNDARY_IMPLEMENTATION_REVIEWED`
