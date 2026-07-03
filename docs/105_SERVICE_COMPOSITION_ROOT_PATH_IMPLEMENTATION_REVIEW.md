# Service Composition / Root Path Implementation Review

## A. Goal

이 문서는 service composition/root path 구현 결과 리뷰 문서다.

기록 대상은 다음과 같다.

- `app/FamilyClaimRef.App/Composition/AppServices.cs` 구현 결과
- `app/FamilyClaimRef.App/App.xaml` startup 변경 결과
- `app/FamilyClaimRef.App/App.xaml.cs` `OnStartup` 기반 MainWindow/DataContext 연결 결과
- production root path 계산 결과
- scope compliance
- build/test 검증 결과

이 문서는 XAML visual layout 구현 리뷰가 아니다. manual runtime check 리뷰가 아니며, Policy/Claim storage 구현 리뷰도 아니다.

## B. Checked Files / Paths

| Path | Purpose | Result |
|---|---|---|
| `docs/104_SERVICE_COMPOSITION_ROOT_PATH_USER_DECISION_RECORD.md` | 사용자 결정 기준 확인 | Checked |
| `docs/103_SERVICE_COMPOSITION_ROOT_PATH_DESIGN.md` | service composition/root path 설계 기준 확인 | Checked |
| `docs/102_WPF_VIEWMODEL_FILE_PICKER_BOUNDARY_IMPLEMENTATION_REVIEW.md` | ViewModel/file picker 구현 기준 확인 | Checked |
| `app/FamilyClaimRef.App/Composition/AppServices.cs` | manual composition root 구현 결과 확인 | Checked |
| `app/FamilyClaimRef.App/App.xaml` | `StartupUri` 제거 확인 | Checked |
| `app/FamilyClaimRef.App/App.xaml.cs` | `OnStartup` 기반 DataContext 연결 확인 | Checked |
| `app/FamilyClaimRef.App/MainWindow.xaml` | visual layout 미수정 확인 | Checked |
| `app/FamilyClaimRef.App/MainWindow.xaml.cs` | MainWindow boundary 확인 | Checked |
| `app/FamilyClaimRef.App/ViewModels/DocumentRegistrationViewModel.cs` | ViewModel dependency boundary 확인 | Checked |
| `app/FamilyClaimRef.App/Services/UI/WpfFilePickerService.cs` | WPF file picker 연결 대상 확인 | Checked |
| `app/FamilyClaimRef.App/Services/Storage/DocumentRegistrationWorkflow.cs` | workflow 생성자 기준 확인 | Checked |
| `app/FamilyClaimRef.App/Services/Storage/JsonDocumentStorageService.cs` | metadata storage root 기준 확인 | Checked |
| `app/FamilyClaimRef.App/Services/Storage/JsonFileStore.cs` | JSON file 생성 시점 확인 | Checked |
| `app/FamilyClaimRef.App/Services/Storage/LocalFileAttachmentService.cs` | attachment root 기준 확인 | Checked |
| `FamilyClaimRef.sln` | build/test 대상 확인 | Checked |
| `app/FamilyClaimRef.App/FamilyClaimRef.App.csproj` | app project 확인 | Checked |
| `tests/FamilyClaimRef.App.Tests/FamilyClaimRef.App.Tests.csproj` | test project 확인 | Checked |

## C. Implementation Summary

- `app/FamilyClaimRef.App/Composition/AppServices.cs` 생성 확인.
- `App.xaml` 수정 확인.
- `App.xaml.cs` 수정 확인.
- `MainWindow.xaml.cs` 수정 없음.
- `MainWindow.xaml` visual layout 수정 없음.
- `AppServices`가 manual composition root 역할을 수행함.
- DI container 사용 없음.
- NuGet package 추가 없음.
- production metadata root는 `%LOCALAPPDATA%\FamilyClaimRef\data\local`.
- production attachment root는 `%LOCALAPPDATA%\FamilyClaimRef\attachments`.
- project root `data/local`, `attachments`를 runtime production root로 사용하지 않음.
- `JsonDocumentStorageService`, `LocalFileAttachmentService`, coordinators, workflow, file picker, ViewModel 연결 확인.
- `App.xaml`에서 `StartupUri` 제거 확인.
- `App.xaml.cs`에서 `OnStartup`으로 `MainWindow` 생성 및 `DataContext` 연결 확인.
- actual app launch 없음.
- actual OpenFileDialog 실행 없음.
- registration workflow 실행 없음.
- production root directory/file 생성 없음.
- XAML UI controls 추가 없음.

## D. AppServices Review

확인 결과:

- `AppServices` class가 존재한다.
- `DocumentRegistrationViewModel` 접근 property가 존재한다.
- `CreateDefault()` factory method가 존재한다.
- production metadata root는 `Environment.SpecialFolder.LocalApplicationData` 기반으로 계산된다.
- production attachment root는 `Environment.SpecialFolder.LocalApplicationData` 기반으로 계산된다.
- project root를 runtime production root로 사용하지 않는다.
- `IDocumentStorageService`는 `JsonDocumentStorageService`로 구성된다.
- `IFileAttachmentService`는 `LocalFileAttachmentService`로 구성된다.
- `DocumentAttachmentCoordinator` 생성이 포함되어 있다.
- `DocumentLinkCoordinator` 생성이 포함되어 있다.
- `DocumentRegistrationWorkflow` 생성이 포함되어 있다.
- `IFilePickerService`는 `WpfFilePickerService`로 구성된다.
- `DocumentRegistrationViewModel` 생성이 포함되어 있다.
- ViewModel은 root path를 알지 않는다.
- `AppServices` 생성만으로 JSON metadata file을 생성하지 않는다.
- `AppServices` 생성만으로 attachment file을 생성하지 않는다.
- DI container 또는 external package 사용 없음.

구성 그래프:

```text
DocumentRegistrationViewModel
 ├─ DocumentRegistrationWorkflow
 │  ├─ DocumentAttachmentCoordinator
 │  │  ├─ IDocumentStorageService -> JsonDocumentStorageService
 │  │  └─ IFileAttachmentService -> LocalFileAttachmentService
 │  ├─ DocumentLinkCoordinator
 │  │  └─ IDocumentStorageService -> JsonDocumentStorageService
 │  ├─ IDocumentStorageService -> JsonDocumentStorageService
 │  └─ IFileAttachmentService -> LocalFileAttachmentService
 └─ IFilePickerService -> WpfFilePickerService
```

## E. App Startup Review

확인 결과:

- `App.xaml`에서 `StartupUri="MainWindow.xaml"`가 제거되었다.
- `App.xaml.cs`에서 `OnStartup` override가 추가되었다.
- `base.OnStartup(e)` 호출이 있다.
- `AppServices.CreateDefault()` 호출이 있다.
- `MainWindow` 생성이 있다.
- `MainWindow.DataContext`에 `services.DocumentRegistrationViewModel`이 연결된다.
- `Application.MainWindow` 설정이 있다.
- `window.Show()` 호출이 있다.
- App startup에서 actual OpenFileDialog 실행 없음.
- App startup에서 registration workflow 실행 없음.
- App startup에서 production JSON file 생성 없음.
- App startup에서 attachment file 생성 없음.

## F. MainWindow Boundary Review

확인 결과:

- `MainWindow.xaml` visual layout 수정 없음.
- XAML UI controls 추가 없음.
- binding layout 구현 없음.
- `MainWindow.xaml.cs` 수정 없음.
- `MainWindow.xaml.cs`는 기본 생성자와 `InitializeComponent()`만 유지한다.
- MainWindow에서 lower-level service 생성 없음.
- MainWindow에서 `DocumentRegistrationWorkflow` 생성 없음.
- MainWindow에서 `OpenFileDialog` 직접 호출 없음.
- MainWindow는 DataContext 소비자 경계로 유지된다.

## G. Root Path Review

root path 기준:

```text
metadata root:
%LOCALAPPDATA%\FamilyClaimRef\data\local

attachment root:
%LOCALAPPDATA%\FamilyClaimRef\attachments
```

metadata files 후보:

```text
documents.json
policy-documents.json
claim-documents.json
```

attachment files 후보:

```text
attachments\documents\<physicalFileName>
```

확인 결과:

- metadata에는 absolute path 저장 없음.
- `DocumentRecord.RelativePath`는 attachment root 기준 relative path를 유지한다.
- root path는 composition root에서만 결정된다.
- ViewModel은 root path를 알지 않는다.
- project root `data/local`, `attachments`는 runtime production root가 아니다.
- App startup만으로 production root directory가 생성되지 않았다.
- App startup만으로 production metadata file이 생성되지 않았다.
- App startup만으로 production attachment file이 생성되지 않았다.

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
- 총 테스트 개수: 216
- 실패 테스트: 없음
- 권한 상승 실행 여부: 있음
- 초기 실패/재실행 여부: 있음
- 초기 실패 원인: Windows SDK 경로 접근 권한 문제
- 재실행 결과: 성공
- actual app launch 여부: 없음
- actual OpenFileDialog 실행 여부: 없음
- registration workflow 실행 여부: 없음
- production `%LOCALAPPDATA%\FamilyClaimRef` 파일 생성 여부: 없음
- production `%LOCALAPPDATA%\FamilyClaimRef` root directory 생성 여부: 없음
- project root `attachments/` 상태: files=0
- project root `data/local/` 상태: files=0
- DB/SQLite 파일 생성 없음.
- Git 상태: 현재 경로는 Git 저장소이며, 구현 변경 파일과 이 리뷰 문서가 커밋 전 working tree에 남아 있음.

## I. Scope Compliance Review

구현 작업 기준 범위 준수 결과:

- `AppServices.cs` 생성 외 production 신규 파일 없음.
- 허용 범위 내에서 `App.xaml` 수정.
- 허용 범위 내에서 `App.xaml.cs` 수정.
- `MainWindow.xaml.cs` 수정 없음.
- `MainWindow.xaml` visual layout 수정 없음.
- XAML UI controls 추가 없음.
- binding layout 구현 없음.
- `DocumentRegistrationViewModel.cs` 수정 없음.
- `IFilePickerService.cs` 수정 없음.
- `WpfFilePickerService.cs` 수정 없음.
- workflow/coordinator/storage/file service 수정 없음.
- `JsonDocumentStorageService.cs` 수정 없음.
- `LocalFileAttachmentService.cs` 수정 없음.
- `FileNamePolicyService.cs` 수정 없음.
- test code 수정 없음.
- test file 생성 없음.
- actual app launch 없음.
- actual OpenFileDialog 실행 없음.
- registration workflow 실행 없음.
- production root JSON file 생성 없음.
- production attachment file 생성 없음.
- Policy/Claim storage 구현 없음.
- OCR/SQLite/repository 구현 없음.
- 실제 개인정보 샘플 사용 없음.
- `.sln` 수정 없음.
- `.csproj` 수정 없음.
- NuGet package 추가 없음.
- Git commit/reset/checkout/add 없음.

리뷰 문서 생성 작업 기준 범위 준수 결과:

- 신규 생성 파일은 `docs/105_SERVICE_COMPOSITION_ROOT_PATH_IMPLEMENTATION_REVIEW.md` 하나다.
- C# production code 수정 없음.
- test code 수정 없음.
- XAML 수정 없음.
- App/MainWindow 연결 코드 수정 없음.

## J. Out of Scope / Not Implemented

아직 구현하지 않은 항목:

- XAML UI binding 없음.
- visual layout 없음.
- file select button 없음.
- register button 없음.
- actual app launch/manual runtime check 없음.
- actual OpenFileDialog runtime 검증 없음.
- production root 권한 검증 없음.
- production root 실제 파일 생성 검증 없음.
- Policy/Claim storage 없음.
- target id는 manual input 상태.
- custom exception 기반 error classification 없음.
- UI navigation 없음.

## K. Risks

남은 위험:

- XAML UI binding은 아직 없다.
- actual app launch/manual runtime check는 아직 없다.
- actual OpenFileDialog runtime 검증은 아직 없다.
- production root 권한 문제는 아직 manual 검증 전이다.
- production root는 계산만 되었고 실제 operation 시점 생성 검증은 아직 없다.
- Policy/Claim storage가 없어 target id는 manual input 상태다.
- App startup 변경은 실제 WPF runtime에서 별도 확인이 필요하다.
- `AppServices`가 커지지 않도록 후속 관리가 필요하다.

## L. Recommendation

추천 순서:

1. 현재 service composition/root path implementation은 build/test PASS 상태로 유지한다.
2. 다음 작업은 manual runtime check 설계 문서 또는 최소 XAML UI binding 설계 문서 중 선택한다.
3. 실제 app launch와 OpenFileDialog는 manual runtime check 단계에서 검증한다.
4. XAML visual layout은 별도 UI 설계 후 진행한다.
5. Policy/Claim storage와 target selection은 이후 별도 설계로 둔다.

권장 다음 작업:

- `docs/106_SERVICE_COMPOSITION_ROOT_PATH_MANUAL_RUNTIME_CHECK_PLAN.md` 생성

또는 UI를 먼저 설계하려면:

- `docs/106_MAINWINDOW_DOCUMENT_REGISTRATION_UI_BINDING_DESIGN.md` 생성

## M. Result

`SERVICE_COMPOSITION_ROOT_PATH_IMPLEMENTATION_REVIEWED`
