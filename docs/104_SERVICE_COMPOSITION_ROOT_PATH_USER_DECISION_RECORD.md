# Service Composition / Root Path User Decision Record

## A. Goal

이 문서는 `docs/103_SERVICE_COMPOSITION_ROOT_PATH_DESIGN.md`의 Needs Decision Q1~Q14에 대한 사용자 결정 기록이다.

목적은 service composition/root path의 책임 범위와 후속 구현 방향을 확정하는 것이다.

이 문서는 구현 문서가 아니다. 이번 작업에서는 C# 구현, `AppServices.cs` 생성, App/MainWindow 수정, XAML 수정, DataContext 연결, actual app launch, actual OpenFileDialog 실행, actual production root file 생성을 수행하지 않는다.

## B. Checked Files / Paths

| Path | Purpose | Status |
|---|---|---|
| `docs/103_SERVICE_COMPOSITION_ROOT_PATH_DESIGN.md` | service composition/root path 설계 기준 | Checked |
| `docs/102_WPF_VIEWMODEL_FILE_PICKER_BOUNDARY_IMPLEMENTATION_REVIEW.md` | ViewModel/file picker 구현 결과 기준 | Checked |
| `docs/101_WPF_VIEWMODEL_FILE_PICKER_BOUNDARY_USER_DECISION_RECORD.md` | ViewModel/file picker 사용자 결정 기준 | Exists |
| `docs/100_WPF_VIEWMODEL_FILE_PICKER_BOUNDARY_DESIGN.md` | ViewModel/file picker 설계 기준 | Exists |
| `docs/99_IMPORT_LINK_COMBINED_WORKFLOW_IMPLEMENTATION_REVIEW.md` | import + link workflow 구현 결과 기준 | Exists |
| `app/FamilyClaimRef.App/ViewModels/DocumentRegistrationViewModel.cs` | ViewModel dependency boundary 확인 | Checked |
| `app/FamilyClaimRef.App/Services/UI/IFilePickerService.cs` | file picker abstraction 확인 | Exists |
| `app/FamilyClaimRef.App/Services/UI/FilePickerResult.cs` | file picker result 확인 | Exists |
| `app/FamilyClaimRef.App/Services/UI/WpfFilePickerService.cs` | WPF file picker implementation 확인 | Exists |
| `app/FamilyClaimRef.App/Services/Storage/DocumentRegistrationWorkflow.cs` | registration workflow dependency 확인 | Checked |
| `app/FamilyClaimRef.App/Services/Storage/DocumentAttachmentCoordinator.cs` | attachment coordinator 확인 | Exists |
| `app/FamilyClaimRef.App/Services/Storage/DocumentLinkCoordinator.cs` | link coordinator 확인 | Exists |
| `app/FamilyClaimRef.App/Services/Storage/JsonDocumentStorageService.cs` | JSON metadata storage 확인 | Exists |
| `app/FamilyClaimRef.App/Services/Storage/JsonFileStore.cs` | JSON file boundary 확인 | Exists |
| `app/FamilyClaimRef.App/Services/Storage/LocalFileAttachmentService.cs` | local attachment file service 확인 | Exists |
| `app/FamilyClaimRef.App/Services/Storage/IDocumentStorageService.cs` | document storage interface 확인 | Exists |
| `app/FamilyClaimRef.App/Services/Storage/IFileAttachmentService.cs` | file attachment interface 확인 | Exists |
| `app/FamilyClaimRef.App/App.xaml` | WPF entry candidate 확인 | Exists |
| `app/FamilyClaimRef.App/App.xaml.cs` | WPF startup candidate 확인 | Exists |
| `app/FamilyClaimRef.App/MainWindow.xaml` | MainWindow view 확인 | Exists |
| `app/FamilyClaimRef.App/MainWindow.xaml.cs` | MainWindow code-behind 확인 | Exists |
| `tests/FamilyClaimRef.App.Tests/DocumentRegistrationViewModelTests.cs` | ViewModel test boundary 확인 | Exists |
| `FamilyClaimRef.sln` | solution 확인 | Exists |
| `app/FamilyClaimRef.App/FamilyClaimRef.App.csproj` | app project 확인 | Exists |
| `tests/FamilyClaimRef.App.Tests/FamilyClaimRef.App.Tests.csproj` | test project 확인 | Exists |

## C. User Decision Summary

| ID | Question | Decision | Recorded Direction |
|---|---|---|---|
| Q1 | service composition/root path 설계를 진행할 것인가? | Accepted | WPF runtime에서 `DocumentRegistrationViewModel`과 application services 생성/연결 방식을 후속 구현 대상으로 둔다. |
| Q2 | 별도 `AppServices` manual composition root를 둘 것인가? | Accepted | `AppServices`를 ViewModel과 service graph를 만드는 작은 factory로 둔다. |
| Q3 | DI container를 사용하지 않고 manual factory로 둘 것인가? | Accepted | MVP 1차에서는 DI container와 NuGet package 추가 없이 manual factory를 사용한다. |
| Q4 | production metadata root를 user app data 기준으로 둘 것인가? | Accepted | `%LOCALAPPDATA%\FamilyClaimRef\data\local`을 production metadata root 후보로 확정한다. |
| Q5 | production attachment root를 user app data 기준으로 둘 것인가? | Accepted | `%LOCALAPPDATA%\FamilyClaimRef\attachments`를 production attachment root 후보로 확정한다. |
| Q6 | project root `data/local`, `attachments`를 runtime production root로 쓰지 않을 것인가? | Accepted | project root의 `data/local`, `attachments`는 runtime production root로 사용하지 않는다. |
| Q7 | root path를 ViewModel이 알지 않게 할 것인가? | Accepted | root path는 composition root에서만 결정하고 ViewModel은 알지 않는다. |
| Q8 | actual directory/file 생성은 service operation 시점에 맡길 것인가? | Accepted | App startup은 root path 계산만 하고 실제 directory/file 생성은 service operation 시점에 맡긴다. |
| Q9 | MainWindow DataContext 연결은 App startup 또는 composition root를 통해 둘 것인가? | Accepted | App startup 또는 composition root를 통해 DataContext 연결을 수행하는 방향으로 둔다. |
| Q10 | `App.xaml`의 `StartupUri` 변경 가능성을 후속 구현 후보로 둘 것인가? | Accepted | startup override가 필요할 수 있으므로 후속 구현 후보로 둔다. |
| Q11 | 후속 구현 때 `AppServices.cs`를 생성할 것인가? | Accepted | `app/FamilyClaimRef.App/Composition/AppServices.cs` 생성 후보를 확정한다. |
| Q12 | 후속 구현 때 XAML UI는 최소 연결만 할 것인가, 아니면 별도 단계로 보류할 것인가? | Accepted - Deferred | XAML layout/UI 구현은 별도 단계로 보류하고, composition 구현은 DataContext 연결과 service graph 구성까지로 제한한다. |
| Q13 | unit test에서는 actual OpenFileDialog와 production app data root를 사용하지 않을 것인가? | Accepted | unit test에서는 fake service, temp directory, custom root만 사용한다. |
| Q14 | manual runtime check를 별도 기록 문서로 남길 것인가? | Accepted | actual app launch, actual OpenFileDialog, user app data root 생성 여부는 manual runtime check로 분리한다. |

## D. Accepted Composition Direction

확정된 방향은 다음과 같다.

- service composition/root path 설계를 후속 구현 기준으로 진행한다.
- 별도 `AppServices` manual composition root를 도입한다.
- DI container는 사용하지 않는다.
- production metadata root는 user app data 기준으로 둔다.
- production attachment root는 user app data 기준으로 둔다.
- project root `data/local`, `attachments`는 runtime production root로 사용하지 않는다.
- root path는 ViewModel이 알지 않는다.
- actual directory/file 생성은 service operation 시점에 맡긴다.
- MainWindow DataContext 연결은 App startup 또는 composition root를 통해 수행한다.
- `App.xaml` `StartupUri` 변경 가능성은 후속 구현 후보로 둔다.
- 후속 구현 때 `AppServices.cs`를 생성한다.
- XAML layout/UI 구현은 별도 단계로 보류한다.
- unit test에서는 actual OpenFileDialog와 production app data root를 사용하지 않는다.
- manual runtime check는 별도 기록 문서로 분리한다.

## E. Implementation Candidate Files

후속 구현에서 생성 후보로 둘 파일은 다음과 같다.

- `app/FamilyClaimRef.App/Composition/AppServices.cs`

후속 구현에서 수정 후보로 둘 파일은 다음과 같다.

- `app/FamilyClaimRef.App/App.xaml`
- `app/FamilyClaimRef.App/App.xaml.cs`
- `app/FamilyClaimRef.App/MainWindow.xaml.cs`

후속 구현에서 읽기/확인 후보로 둘 파일은 다음과 같다.

- `app/FamilyClaimRef.App/MainWindow.xaml`

주의:

- 이번 문서 작업에서는 위 파일을 생성하거나 수정하지 않는다.
- 실제 생성/수정은 별도 구현 승인 후 진행한다.

## F. Root Path Shape

후속 구현 후보 production metadata root는 다음과 같다.

```text
%LOCALAPPDATA%/FamilyClaimRef/data/local/
```

후속 구현 후보 production attachment root는 다음과 같다.

```text
%LOCALAPPDATA%/FamilyClaimRef/attachments/
```

metadata files 후보는 다음과 같다.

```text
data/local/documents.json
data/local/policy-documents.json
data/local/claim-documents.json
```

attachment files 후보는 다음과 같다.

```text
attachments/documents/<physicalFileName>
```

확정된 저장 경계는 다음과 같다.

- metadata에는 absolute path를 저장하지 않는다.
- `DocumentRecord.RelativePath`는 attachment root 기준 relative path를 유지한다.
- ViewModel은 root path를 알지 않는다.
- unit test는 production root를 사용하지 않는다.

## G. Service Graph Candidate

후속 구현 후보 service graph는 다음과 같다.

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

후속 구현 주의사항은 다음과 같다.

- 같은 runtime에서는 동일 root path 기준 service를 구성한다.
- `JsonDocumentStorageService` instance 공유 여부는 후속 구현에서 선택하되, 동일 metadata root를 사용해야 한다.
- `LocalFileAttachmentService`는 동일 attachment root를 사용해야 한다.
- ViewModel은 lower-level service를 직접 생성하지 않는다.

## H. Still Not Implemented

아직 구현하지 않은 항목은 다음과 같다.

- C# 구현 없음
- `AppServices.cs` 생성 없음
- App/MainWindow 수정 없음
- XAML 수정 없음
- DataContext 연결 없음
- actual app launch 없음
- actual OpenFileDialog 실행 없음
- actual production root file 생성 없음
- ViewModel 수정 없음
- workflow/coordinator/storage/file service 수정 없음
- test code 수정 없음
- test file 생성 없음
- Policy/Claim storage 구현 없음
- OCR 구현 없음
- SQLite DB/package 추가 없음
- repository/data access/migration 구현 없음
- 실제 개인정보 샘플 없음

## I. Next Step

다음 작업 후보는 다음과 같다.

1. 별도 승인 후 `AppServices` / App startup / MainWindow DataContext 최소 연결을 구현한다.
2. 구현 후보는 다음 파일을 중심으로 검토한다.
   - `app/FamilyClaimRef.App/Composition/AppServices.cs`
   - `app/FamilyClaimRef.App/App.xaml`
   - `app/FamilyClaimRef.App/App.xaml.cs`
   - `app/FamilyClaimRef.App/MainWindow.xaml.cs`
3. XAML visual layout은 별도 단계로 보류한다.
4. 구현 후 `dotnet build FamilyClaimRef.sln`, `dotnet test FamilyClaimRef.sln`을 실행한다.
5. 구현 후 `docs/105_SERVICE_COMPOSITION_ROOT_PATH_IMPLEMENTATION_REVIEW.md`를 생성한다.
6. 그 다음 manual runtime check 문서를 생성한다.

## J. Result

`SERVICE_COMPOSITION_ROOT_PATH_USER_DECISION_RECORDED`
