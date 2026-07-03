# MainWindow Document Registration UI Binding User Decision Record

## A. Goal

이 문서는 `docs/108_MAINWINDOW_DOCUMENT_REGISTRATION_UI_BINDING_DESIGN.md`에 대한 사용자 결정 기록이다.

목적은 MainWindow 문서 등록 UI binding의 책임 범위, 후속 구현 방향, 구현 전 제한 사항을 확정하는 것이다.

이 문서는 구현 문서가 아니다. 이번 기록에서는 XAML 수정, C# 수정, app launch, OpenFileDialog 실행, 파일 선택, 등록 workflow 실행을 수행하지 않는다.

## B. Checked Files / Paths

| 경로 | 확인 목적 | 상태 |
|---|---|---|
| `docs/108_MAINWINDOW_DOCUMENT_REGISTRATION_UI_BINDING_DESIGN.md` | UI binding 설계 기준 | 확인 |
| `docs/107_SERVICE_COMPOSITION_ROOT_PATH_MANUAL_RUNTIME_CHECK_RESULT.md` | startup-only runtime check 결과 기준 | 확인 |
| `docs/105_SERVICE_COMPOSITION_ROOT_PATH_IMPLEMENTATION_REVIEW.md` | service composition 구현 결과 기준 | 확인 |
| `app/FamilyClaimRef.App/MainWindow.xaml` | 후속 XAML 수정 후보 | 확인 |
| `app/FamilyClaimRef.App/MainWindow.xaml.cs` | 후속 code-behind 수정 후보 | 확인 |
| `app/FamilyClaimRef.App/App.xaml` | composition root 연결 상태 확인 대상 | 확인 |
| `app/FamilyClaimRef.App/App.xaml.cs` | MainWindow DataContext 연결 상태 확인 대상 | 확인 |
| `app/FamilyClaimRef.App/Composition/AppServices.cs` | service composition root 확인 대상 | 확인 |
| `app/FamilyClaimRef.App/ViewModels/DocumentRegistrationViewModel.cs` | UI binding 대상 ViewModel 확인 대상 | 확인 |
| `app/FamilyClaimRef.App/Services/UI/WpfFilePickerService.cs` | OpenFileDialog boundary 확인 대상 | 확인 |
| `app/FamilyClaimRef.App/Services/Storage/DocumentRegistrationWorkflow.cs` | registration workflow boundary 확인 대상 | 확인 |
| `FamilyClaimRef.sln` | solution 기준 | 확인 |

## C. User Decision Summary

| ID | Question | Decision | Recorded Direction |
|---|---|---|---|
| Q1 | MainWindow document registration UI binding 설계를 진행할 것인가 | Accepted | 후속 구현 대상으로 둔다. 목적은 actual OpenFileDialog와 registration runtime check 가능한 최소 UI 확보이다. |
| Q2 | 후속 구현에서 MainWindow.xaml에 최소 controls를 추가할 것인가 | Accepted | `MainWindow.xaml`에 최소 document registration controls를 추가하는 방향으로 확정한다. Complex layout, styling, navigation은 제외한다. |
| Q3 | visual styling은 최소로 제한할 것인가 | Accepted | 최종 사용자용 완성 화면이 아니라 runtime/binding 확인용 MVP로 제한한다. |
| Q4 | file select button을 추가할 것인가 | Accepted | 버튼 click은 ViewModel의 `SelectFileAsync(...)` 호출로 제한한다. 자동 OpenFileDialog 실행은 금지한다. |
| Q5 | register button을 추가할 것인가 | Accepted | 버튼 click은 ViewModel의 `RegisterAsync(...)` 호출로 제한한다. 자동 registration workflow 실행은 금지한다. |
| Q6 | button action은 code-behind click handler로 ViewModel method만 호출하게 할 것인가 | Accepted | code-behind는 허용하되 ViewModel method 호출만 허용한다. service, workflow, storage, file service 생성 또는 직접 호출은 금지한다. |
| Q7 | command pattern 추가는 보류할 것인가 | Accepted - Deferred | `SelectFileCommand`, `RegisterCommand`, async command class, command test scope는 보류한다. UI hardening 단계에서 재검토한다. |
| Q8 | TargetKind는 policy/claim 선택 control로 둘 것인가 | Accepted | `policy` / `claim` selector를 둔다. ComboBox 또는 RadioButton 중 구현 시 단순한 방식을 선택한다. |
| Q9 | TargetId는 manual dummy input으로 둘 것인가 | Accepted | Policy/Claim storage가 없으므로 manual dummy input만 사용한다. 실제 개인정보나 실제 보험/청구 식별자는 사용하지 않는다. |
| Q10 | DocumentType은 static ComboBox 후보로 둘 것인가 | Accepted | static ComboBox를 UI helper 후보로 둔다. Source of truth는 service/storage validation에 둔다. |
| Q11 | DisplayTitle은 TextBox로 둘 것인가 | Accepted | `DisplayTitle`은 TextBox로 둔다. 필수 validation은 기존 ViewModel 기준을 따른다. |
| Q12 | ReferenceDate는 DatePicker로 둘 것인가 | Accepted | `ReferenceDate`는 DatePicker 후보로 둔다. Binding/converter가 과해지면 구현 시 최소 조정 후 기록한다. |
| Q13 | ValidationMessage / StatusMessage / LastRegistrationSummary 표시 영역을 둘 것인가 | Accepted | validation, failure, success runtime check가 가능하도록 표시 영역을 둔다. |
| Q14 | MainWindow code-behind에서 service/workflow/storage 생성은 금지할 것인가 | Accepted | MainWindow code-behind는 ViewModel method만 호출한다. `AppServices`가 composition root로 유지된다. |
| Q15 | 후속 구현 후 OpenFileDialog와 registration manual runtime check를 별도 결과 문서로 기록할 것인가 | Accepted | dummy-only data로 별도 결과 문서를 작성한다. 실제 개인정보, 실제 보험사명, 실제 병원명, 실제 진단 정보는 사용하지 않는다. |

## D. Accepted UI Binding Direction

- MainWindow document registration UI binding을 진행한다.
- `MainWindow.xaml`에 최소 controls를 추가한다.
- Visual styling은 최소화한다.
- File select button을 추가한다.
- Register button을 추가한다.
- Button action은 code-behind click handler를 허용한다.
- Code-behind는 ViewModel method만 호출한다.
- Command pattern은 deferred로 둔다.
- TargetKind는 `policy` / `claim` selector로 둔다.
- TargetId는 manual dummy input으로 둔다.
- DocumentType은 static ComboBox 후보로 둔다.
- DisplayTitle은 TextBox로 둔다.
- ReferenceDate는 DatePicker 후보로 둔다.
- `ValidationMessage`, `StatusMessage`, `LastRegistrationSummary` 표시 영역을 둔다.
- MainWindow code-behind에서 service, workflow, storage 생성은 금지한다.
- 후속 구현 후 OpenFileDialog와 registration manual runtime check는 별도 결과 문서에 기록한다.

## E. Implementation Candidate Files

수정 후보:

- `app/FamilyClaimRef.App/MainWindow.xaml`
- `app/FamilyClaimRef.App/MainWindow.xaml.cs`

읽기 및 보존 후보:

- `app/FamilyClaimRef.App/App.xaml`
- `app/FamilyClaimRef.App/App.xaml.cs`
- `app/FamilyClaimRef.App/Composition/AppServices.cs`
- `app/FamilyClaimRef.App/ViewModels/DocumentRegistrationViewModel.cs`

이번 문서는 위 파일을 수정하지 않는다. 실제 수정은 별도 구현 승인 후 진행한다.

## F. Minimum UI Field Candidate

| UI Field | 목적 | 비고 |
|---|---|---|
| Select file button | 사용자가 명시적으로 파일 선택을 시작 | 자동 실행 금지 |
| Selected file display name | 선택된 파일의 표시명 확인 | 실제 파일 경로 전체 노출은 피한다 |
| Target kind selector | `policy` / `claim` 선택 | 최소 selector |
| Target id TextBox | 연결 대상 id 입력 | dummy input |
| Document type ComboBox | 문서 유형 선택 | static candidate |
| Display title TextBox | 표시 제목 입력 | ViewModel validation 기준 |
| Reference date DatePicker | 기준일 입력 | 구현 난이도에 따라 최소 조정 가능 |
| Register button | 사용자가 명시적으로 등록 실행 | 자동 실행 금지 |
| Validation message TextBlock | 입력 검증 메시지 표시 | runtime check 대상 |
| Status message TextBlock | 처리 상태 표시 | runtime check 대상 |
| Last registration summary TextBlock | 마지막 등록 결과 요약 표시 | runtime check 대상 |
| Scope notice TextBlock | dummy-only / local-only 안내 | 실제 개인정보 사용 금지 안내 |

## G. Code-behind Boundary

허용:

- `DataContext`를 `DocumentRegistrationViewModel`로 cast
- file select button click에서 `SelectFileAsync(...)` 호출
- register button click에서 `RegisterAsync(...)` 호출
- 기본 exception guard 또는 status-safe handling 후보

금지:

- `AppServices` 생성
- `DocumentRegistrationWorkflow` 생성
- `DocumentAttachmentCoordinator` 생성
- `DocumentLinkCoordinator` 생성
- `JsonDocumentStorageService` 생성
- `LocalFileAttachmentService` 생성
- `FileNamePolicyService` 직접 호출
- 직접 file copy
- 직접 JSON metadata save
- 직접 OpenFileDialog 호출
- 자동 registration workflow 실행

Notes:

- OpenFileDialog는 반드시 `WpfFilePickerService` 경계를 통해서만 실행한다.
- MainWindow code-behind는 ViewModel만 호출한다.

## H. Static DocumentType ComboBox Candidate

Policy document type values:

- `policy`
- `terms`
- `contract`
- `capture`
- `etc`

Claim document type values:

- `receipt`
- `diagnosis`
- `medicine`
- `visit`
- `admission`
- `surgery`
- `etc`

Static ComboBox는 UI helper이다. Validation owner는 기존 service/storage 기준으로 유지한다.

TargetKind에 따라 list를 전환할지, unified list로 둘지는 후속 구현에서 최소 변경 원칙으로 선택하고 구현 결과 문서에 기록한다.

## I. Manual Runtime Check After Implementation

후속 구현 후 다음 항목을 별도 결과 문서에서 확인한다.

- App launch
- MainWindow 표시
- File select button 표시
- Button click 전 OpenFileDialog 자동 실행 없음
- File select button click 시 OpenFileDialog 표시
- Cancel 시 crash 없음
- Cancel 시 metadata/attachment 파일 생성 없음
- Dummy file select 후 selected display name 표시
- Register button click 시 validation message 표시
- Dummy registration 시 metadata/attachment는 `%LOCALAPPDATA%\FamilyClaimRef` 아래에만 생성
- Project root `attachments/`, `data/local` 오염 없음
- 실제 개인정보 샘플 사용 없음

## J. Still Not Implemented

- XAML modification 없음
- `MainWindow.xaml.cs` modification 없음
- `AppServices`, `App.xaml`, `App.xaml.cs`, ViewModel, file picker, workflow, storage modification 없음
- Code-behind 없음
- Command 없음
- Actual app launch 없음
- OpenFileDialog 실행 없음
- File select 실행 없음
- Registration workflow 실행 없음
- Production root file 생성 없음
- Cleanup 없음
- Tests 없음
- Policy/Claim storage 없음
- OCR, SQLite, repository 없음
- Real personal data 없음

## K. Next Step

1. 별도 승인 후 MainWindow 최소 UI binding을 구현한다.
2. 수정 후보는 `app/FamilyClaimRef.App/MainWindow.xaml`, `app/FamilyClaimRef.App/MainWindow.xaml.cs`로 제한한다.
3. 구현 후 `dotnet build`와 기존 test scope를 실행한다.
4. `docs/110_MAINWINDOW_DOCUMENT_REGISTRATION_UI_BINDING_IMPLEMENTATION_REVIEW.md`를 작성한다.
5. 이후 OpenFileDialog / registration manual runtime check 결과 문서를 별도로 작성한다.

## L. Result

`MAINWINDOW_DOCUMENT_REGISTRATION_UI_BINDING_USER_DECISION_RECORDED`
