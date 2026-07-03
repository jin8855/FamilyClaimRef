# MainWindow Document Registration UI Binding Implementation Review

## A. Goal

이 문서는 MainWindow document registration UI binding 구현 결과 리뷰 문서다.

기록 대상:

- `MainWindow.xaml` 최소 controls 추가 결과
- `MainWindow.xaml.cs` click handler 구현 결과
- ViewModel / AppServices / workflow boundary 준수 여부
- build/test 결과

이 문서는 actual app launch, OpenFileDialog, file select, registration workflow runtime check 리뷰가 아니다.
이번 작업에서는 XAML, C#, test, project, solution 파일을 수정하지 않는다.

## B. Checked Files / Paths

| 경로 | 확인 목적 | 상태 |
|---|---|---|
| `docs/109_MAINWINDOW_DOCUMENT_REGISTRATION_UI_BINDING_USER_DECISION_RECORD.md` | 사용자 결정 기준 | 확인 |
| `docs/108_MAINWINDOW_DOCUMENT_REGISTRATION_UI_BINDING_DESIGN.md` | UI binding 설계 기준 | 확인 |
| `docs/107_SERVICE_COMPOSITION_ROOT_PATH_MANUAL_RUNTIME_CHECK_RESULT.md` | startup-only runtime check 기준 | 확인 |
| `docs/105_SERVICE_COMPOSITION_ROOT_PATH_IMPLEMENTATION_REVIEW.md` | composition root 구현 결과 기준 | 확인 |
| `app/FamilyClaimRef.App/MainWindow.xaml` | UI 구현 결과 | 확인 |
| `app/FamilyClaimRef.App/MainWindow.xaml.cs` | click handler 구현 결과 | 확인 |
| `app/FamilyClaimRef.App/App.xaml` | App startup boundary | 확인 |
| `app/FamilyClaimRef.App/App.xaml.cs` | DataContext 연결 boundary | 확인 |
| `app/FamilyClaimRef.App/Composition/AppServices.cs` | composition root boundary | 확인 |
| `app/FamilyClaimRef.App/ViewModels/DocumentRegistrationViewModel.cs` | binding surface / method boundary | 확인 |
| `app/FamilyClaimRef.App/Services/UI/IFilePickerService.cs` | file picker abstraction boundary | 확인 |
| `app/FamilyClaimRef.App/Services/UI/WpfFilePickerService.cs` | OpenFileDialog boundary | 확인 |
| `app/FamilyClaimRef.App/Services/Storage/DocumentRegistrationWorkflow.cs` | registration workflow boundary | 확인 |
| `tests/FamilyClaimRef.App.Tests/DocumentRegistrationViewModelTests.cs` | existing ViewModel test scope | 확인 |
| `FamilyClaimRef.sln` | build/test 대상 | 확인 |
| `app/FamilyClaimRef.App/FamilyClaimRef.App.csproj` | app project | 확인 |
| `tests/FamilyClaimRef.App.Tests/FamilyClaimRef.App.Tests.csproj` | test project | 확인 |

## C. Implementation Summary

- `MainWindow.xaml` 수정 확인.
- `MainWindow.xaml.cs` 수정 확인.
- 신규 파일 생성 없음.
- `App.xaml`, `App.xaml.cs`, `AppServices.cs` 수정 없음.
- `DocumentRegistrationViewModel.cs` 수정 없음.
- file picker, workflow, coordinator, storage, file service 수정 없음.
- command pattern 추가 없음.
- command class 추가 없음.
- converter class 추가 없음.
- test code/test file 생성 없음.
- `MainWindow.xaml`에 최소 document registration controls 추가.
- scope notice 추가.
- file select button 추가.
- selected file display binding 추가.
- target kind selector 추가.
- target id TextBox 추가.
- documentType static ComboBox 추가.
- displayTitle TextBox 추가.
- referenceDate DatePicker 추가.
- register button 추가.
- validation/status/result summary 표시 영역 추가.
- `MainWindow.xaml.cs` click handler 추가.
- click handler는 `DocumentRegistrationViewModel.SelectFileAsync(...)`, `RegisterAsync(...)`만 호출.
- code-behind에서 service, workflow, storage, file picker 직접 생성 없음.

## D. MainWindow XAML Review

| 확인 항목 | 판정 | 근거 |
|---|---|---|
| scope notice 존재 | PASS | 실제 개인정보, 보험사명, 병원명, 진단명, 진단코드 사용 금지 문구가 있다. |
| file select button 존재 | PASS | `Select file` button과 `SelectFileButton_Click` 연결이 있다. |
| selected file display name binding 존재 | PASS | `SelectedSourceFileDisplayName` binding이 있다. |
| target kind selector 존재 | PASS | `ComboBox`가 있다. |
| target kind values | PASS | `policy`, `claim`이 있다. |
| target id TextBox 존재 | PASS | `TargetId` TextBox binding이 있다. |
| documentType static ComboBox 존재 | PASS | `DocumentType` ComboBox binding이 있다. |
| documentType 후보 값 | PASS | `policy`, `terms`, `contract`, `capture`, `receipt`, `diagnosis`, `medicine`, `visit`, `admission`, `surgery`, `etc`가 있다. |
| displayTitle TextBox 존재 | PASS | `DisplayTitle` TextBox binding이 있다. |
| referenceDate DatePicker 존재 | PASS_WITH_NOTES | `ReferenceDate` DatePicker binding이 있다. ViewModel type이 `DateOnly`라 actual runtime 확인이 필요하다. |
| register button 존재 | PASS | `Register` button과 `RegisterButton_Click` 연결이 있다. |
| ValidationMessage 표시 binding | PASS | `ValidationMessage` TextBlock binding이 있다. |
| StatusMessage 표시 binding | PASS | `StatusMessage` TextBlock binding이 있다. |
| LastRegistrationSummary 표시 binding | PASS | `LastRegistrationSummary` TextBlock binding이 있다. |
| complex styling 없음 | PASS | 기본 WPF controls와 기본 spacing만 사용한다. |
| navigation 없음 | PASS | route/navigation 요소가 없다. |
| converter class 필요 없는 구성 | PASS_WITH_NOTES | 새 converter는 추가하지 않았다. 단, `DateOnly`/`DatePicker` runtime binding은 별도 확인이 필요하다. |
| command binding 없음 | PASS | click handler만 사용한다. |
| automatic OpenFileDialog 유발 요소 없음 | PASS | XAML에는 button click 연결만 있다. |
| automatic registration workflow 유발 요소 없음 | PASS | XAML에는 register button click 연결만 있다. |

## E. Binding Review

| Binding 대상 | 구현 위치 | Binding 상태 | 비고 |
|---|---|---|---|
| `SelectedSourceFileDisplayName` | `TextBlock.Text` | PASS | OneWay display binding |
| `TargetKind` | `ComboBox.SelectedValue` | PASS | `Mode=TwoWay`, `UpdateSourceTrigger=PropertyChanged` |
| `TargetId` | `TextBox.Text` | PASS | `Mode=TwoWay`, `UpdateSourceTrigger=PropertyChanged` |
| `DocumentType` | `ComboBox.SelectedValue` | PASS | `Mode=TwoWay`, `UpdateSourceTrigger=PropertyChanged` |
| `DisplayTitle` | `TextBox.Text` | PASS | `Mode=TwoWay`, `UpdateSourceTrigger=PropertyChanged` |
| `ReferenceDate` | `DatePicker.SelectedDate` | PASS_WITH_NOTES | ViewModel property가 `DateOnly`라 runtime 표시/입력 확인 필요 |
| `ValidationMessage` | `TextBlock.Text` | PASS | OneWay display binding |
| `StatusMessage` | `TextBlock.Text` | PASS | OneWay display binding |
| `LastRegistrationSummary` | `TextBlock.Text` | PASS | OneWay display binding |
| `IsBusy` | `TextBlock.Text` | PASS | 상태 표시용 display binding |

주의:

- `ReferenceDate`는 ViewModel에서 `DateOnly`이다.
- WPF `DatePicker.SelectedDate`는 일반적으로 nullable `DateTime` surface를 사용한다.
- 이번 리뷰에서는 app launch/runtime 확인을 하지 않았으므로 실제 표시와 입력은 후속 manual runtime check에서 확인해야 한다.

## F. Code-behind Review

| 확인 항목 | 판정 | 근거 |
|---|---|---|
| file select button click handler 존재 | PASS | `SelectFileButton_Click` 존재 |
| register button click handler 존재 | PASS | `RegisterButton_Click` 존재 |
| `DataContext`를 `DocumentRegistrationViewModel`로 cast | PASS | 두 handler 모두 `DataContext is DocumentRegistrationViewModel viewModel` guard 사용 |
| `SelectFileAsync(...)` 호출 | PASS | file select handler에서만 호출 |
| `RegisterAsync(...)` 호출 | PASS | register handler에서만 호출 |
| `AppServices` 생성 없음 | PASS | forbidden pattern search에서 match 없음 |
| `DocumentRegistrationWorkflow` 생성 없음 | PASS | forbidden pattern search에서 match 없음 |
| `DocumentAttachmentCoordinator` 생성 없음 | PASS | forbidden pattern search에서 match 없음 |
| `DocumentLinkCoordinator` 생성 없음 | PASS | forbidden pattern search에서 match 없음 |
| `JsonDocumentStorageService` 생성 없음 | PASS | forbidden pattern search에서 match 없음 |
| `LocalFileAttachmentService` 생성 없음 | PASS | forbidden pattern search에서 match 없음 |
| `FileNamePolicyService` 호출 없음 | PASS | forbidden pattern search에서 match 없음 |
| OpenFileDialog 직접 호출 없음 | PASS | forbidden pattern search에서 match 없음 |
| file copy 직접 수행 없음 | PASS | forbidden pattern search에서 match 없음 |
| JSON metadata 직접 저장 없음 | PASS | forbidden pattern search에서 match 없음 |
| registration workflow 직접 실행 없음 | PASS | forbidden pattern search에서 match 없음 |

Code-behind 실제 역할:

```text
DataContext 확인
-> DocumentRegistrationViewModel method 호출
```

## G. Scope Compliance Review

| 항목 | 판정 |
|---|---|
| 수정 파일이 `MainWindow.xaml`, `MainWindow.xaml.cs`로 제한됨 | PASS |
| 신규 구현 파일 생성 없음 | PASS |
| 이번 리뷰 문서 생성 전 구현 파일 추가 수정 없음 | PASS |
| `App.xaml` 수정 없음 | PASS |
| `App.xaml.cs` 수정 없음 | PASS |
| `AppServices.cs` 수정 없음 | PASS |
| `DocumentRegistrationViewModel.cs` 수정 없음 | PASS |
| `IFilePickerService.cs` 수정 없음 | PASS |
| `WpfFilePickerService.cs` 수정 없음 | PASS |
| workflow/coordinator/storage/file service 수정 없음 | PASS |
| command pattern 추가 없음 | PASS |
| command class 추가 없음 | PASS |
| converter class 추가 없음 | PASS |
| new ViewModel 추가 없음 | PASS |
| new service 추가 없음 | PASS |
| test code 수정 없음 | PASS |
| test file 생성 없음 | PASS |
| actual app launch 없음 | PASS |
| actual OpenFileDialog 실행 없음 | PASS |
| actual file select 없음 | PASS |
| actual registration workflow 실행 없음 | PASS |
| dummy registration 실행 없음 | PASS |
| production root file 생성 없음 | PASS |
| cleanup 실행 없음 | PASS |
| Policy/Claim storage 구현 없음 | PASS |
| OCR/SQLite/repository 구현 없음 | PASS |
| 실제 개인정보 샘플 사용 없음 | PASS |
| `.sln` 수정 없음 | PASS |
| `.csproj` 수정 없음 | PASS |
| NuGet package 추가 없음 | PASS |
| Git add/commit/reset/checkout/clean 없음 | PASS |

## H. Verification Result

`dotnet build FamilyClaimRef.sln`:

```text
PASS
warning: 0
error: 0
```

`dotnet test FamilyClaimRef.sln`:

```text
PASS
total tests: 216
failed tests: 0
skipped tests: 0
```

실행 기록:

| 항목 | 결과 |
|---|---|
| 권한 상승 실행 여부 | 있음 |
| 초기 실패/재실행 여부 | 있음 |
| 초기 실패 원인 | Windows SDK 경로 접근 권한 문제 |
| 권한 상승 재실행 결과 | build/test PASS |
| actual app launch 여부 | 없음 |
| actual OpenFileDialog 실행 여부 | 없음 |
| actual file select 여부 | 없음 |
| actual registration workflow 실행 여부 | 없음 |
| production `%LOCALAPPDATA%\FamilyClaimRef` file 생성 여부 | 없음 |
| production `%LOCALAPPDATA%\FamilyClaimRef` root directory | 없음 |
| project root `attachments/` 상태 | files=0 |
| project root `data/local` 상태 | files=0 |
| DB/SQLite file 생성 여부 | 없음 |

참고:

- 일반 sandbox 실행에서는 `C:\Users\jin8855\AppData\Local\Microsoft SDKs` 접근 권한 문제로 build/test가 실패했다.
- 같은 명령을 권한 상승으로 재실행했을 때 build/test가 통과했다.

## I. Out of Scope / Not Implemented

- actual app launch 없음.
- actual OpenFileDialog runtime 검증 없음.
- actual file select 검증 없음.
- registration runtime 검증 없음.
- production root operation-time 생성 검증 없음.
- dummy registration 없음.
- cleanup 없음.
- Policy/Claim storage 없음.
- target id manual dummy input 상태.
- command pattern 없음.
- `ReferenceDate` DateOnly/DatePicker runtime binding 확인 필요.
- static documentType ComboBox는 UI helper이며 source of truth는 service/storage validation.

## J. Risks

- actual app launch가 아직 없다.
- actual OpenFileDialog runtime 검증이 아직 없다.
- actual file select 검증이 아직 없다.
- registration runtime 검증이 아직 없다.
- production root operation-time 생성 검증이 아직 없다.
- `ReferenceDate`는 ViewModel에서 `DateOnly`이고, DatePicker는 최소 binding 상태이므로 실제 화면 표시/입력 확인이 필요하다.
- Policy/Claim storage가 없어 target id는 manual dummy input 상태다.
- code-behind click handler 방식이라 MVVM 순도는 낮지만 이번 승인 범위에는 부합한다.
- documentType static ComboBox는 UI helper이며 source of truth는 service/storage validation에 있다.
- dummy registration 수행 시 `%LOCALAPPDATA%`에 test files가 생성될 수 있다.

## K. Recommendation

1. 현재 MainWindow UI binding implementation은 build/test PASS 상태로 유지한다.
2. 다음 작업은 OpenFileDialog / file select / validation / dummy registration manual runtime check 계획 문서 생성이다.
3. manual runtime check는 단계적으로 진행한다.
   - app launch
   - OpenFileDialog cancel
   - dummy file select
   - validation message
   - dummy registration
   - production root 생성 확인
   - cleanup 여부 기록
4. runtime check 전 실제 개인정보 파일 사용 금지 조건을 다시 명시한다.

권장 다음 문서:

- `docs/111_MAINWINDOW_DOCUMENT_REGISTRATION_MANUAL_RUNTIME_CHECK_PLAN.md`

## L. Result

`MAINWINDOW_DOCUMENT_REGISTRATION_UI_BINDING_IMPLEMENTATION_REVIEWED`
