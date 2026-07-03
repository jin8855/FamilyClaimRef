# MainWindow Document Registration UI Binding Design

## A. Goal

이 문서는 MainWindow document registration UI binding 설계 문서다.

목적은 `DocumentRegistrationViewModel`을 실제 WPF 화면에 최소 연결하기 위한 UI binding 후보를 정리하는 것이다. file select, target input, document type, display title, reference date, status/result 표시 영역을 설계한다.

이 문서는 actual 구현 문서가 아니다. 이번 작업에서는 XAML/C# 수정이 없으며, app launch, OpenFileDialog, registration workflow 실행도 수행하지 않는다.

## B. Current Baseline

현재 baseline은 다음과 같다.

- startup-only manual runtime check 통과.
- `AppServices`와 `MainWindow.DataContext` 연결 완료.
- MainWindow는 표시되지만 controls 없음.
- UI interaction availability는 현재 `Not available`.
- OpenFileDialog runtime check는 UI binding 이후 가능.
- registration runtime check는 UI binding 이후 가능.
- Policy/Claim storage가 없어 target id는 manual dummy input 상태.
- `dotnet build FamilyClaimRef.sln` PASS.
- `dotnet test FamilyClaimRef.sln` PASS.
- 총 테스트 수 216.
- MainWindow 표시 확인.
- startup exception 없음.
- OpenFileDialog 자동 실행 없음.
- registration workflow 자동 실행 없음.
- production `%LOCALAPPDATA%\FamilyClaimRef` root 생성 없음.
- project root `attachments/`, `data/local` files=0.

## C. Problem Statement

현재 문제는 다음과 같다.

- ViewModel과 App composition은 연결되었지만 사용자가 조작할 UI가 없다.
- `SelectFileAsync`를 실행할 button이 없다.
- `RegisterAsync`를 실행할 button이 없다.
- `SelectedSourceFileDisplayName`, `TargetKind`, `TargetId`, `DocumentType`, `DisplayTitle`, `ReferenceDate`를 입력/표시할 binding이 없다.
- `ValidationMessage`, `StatusMessage`, `LastRegistrationSummary`를 표시할 영역이 없다.
- XAML UI를 과하게 만들면 scope가 커질 수 있으므로 MVP 최소 binding으로 제한해야 한다.
- Policy/Claim storage가 없으므로 target id는 실제 선택 UI가 아니라 manual dummy input으로 시작해야 한다.

## D. ViewModel Binding Surface Review

`DocumentRegistrationViewModel` 기준 binding 대상 후보는 다음과 같다.

### Property Candidate

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

### Method Candidate

- `SelectFileAsync(...)`
- `RegisterAsync(...)`

주의:

- 현재 command wrapper가 없다면 button click은 code-behind에서 ViewModel method를 호출하는 후보가 될 수 있다.
- command pattern 도입은 후속 후보로 둘 수 있다.
- 이번 설계에서는 최소 binding을 우선한다.

## E. UI Layout Candidate

MVP 최소 layout 후보는 다음과 같다.

```text
MainWindow
 └─ Document Registration Panel
     ├─ Source File Section
     │   ├─ Select file button
     │   └─ Selected file display name
     ├─ Target Section
     │   ├─ Target kind: policy / claim
     │   └─ Target id input
     ├─ Document Metadata Section
     │   ├─ Document type input or ComboBox
     │   ├─ Display title input
     │   └─ Reference date input
     ├─ Action Section
     │   └─ Register button
     ├─ Status Section
     │   ├─ Validation message
     │   ├─ Status message
     │   └─ Last registration summary
     └─ Scope Notice Section
         └─ MVP/manual dummy target 안내
```

주의:

- visual design은 최소로 둔다.
- 복잡한 styling은 하지 않는다.
- 화면 목적은 binding/runtime 확인이다.
- 실제 사용자용 최종 UI가 아니다.

## F. Control Candidate

| ViewModel Field | Control Candidate | Notes |
|---|---|---|
| `SelectedSourceFileDisplayName` | `TextBlock` | 선택된 파일명 표시 |
| `SelectFileAsync` | `Button` | click handler 후보 |
| `TargetKind` | `ComboBox` 또는 `RadioButton` | `policy` / `claim` |
| `TargetId` | `TextBox` | manual dummy input |
| `DocumentType` | `ComboBox` 또는 `TextBox` | MVP에서는 TextBox 가능, seed options binding은 후속 |
| `DisplayTitle` | `TextBox` | required |
| `ReferenceDate` | `DatePicker` | date binding |
| `RegisterAsync` | `Button` | click handler 후보 |
| `IsBusy` | button enabled/disabled 후보 | 후속 binding 가능 |
| `ValidationMessage` | `TextBlock` | validation feedback |
| `StatusMessage` | `TextBlock` | success/failure |
| `LastRegistrationSummary` | `TextBlock` | result summary |

## G. Event Handler vs Command Candidate

button action 연결 후보는 다음과 같다.

### Candidate 1. code-behind click handler

내용:

- `MainWindow.xaml.cs`에서 `DataContext`를 `DocumentRegistrationViewModel`로 cast.
- button click에서 `SelectFileAsync`, `RegisterAsync` 호출.

장점:

- 최소 구현이 쉽다.
- ViewModel 수정이 필요 없다.
- command class 추가가 필요 없다.

단점:

- code-behind가 생긴다.
- command binding 방식보다 MVVM 순도가 낮다.

### Candidate 2. ViewModel에 command property 추가

내용:

- `SelectFileCommand`, `RegisterCommand`를 ViewModel에 추가.
- XAML에서 `Command="{Binding SelectFileCommand}"` 사용.

장점:

- WPF MVVM 방식에 가깝다.
- button enabled 상태와 연결하기 쉽다.

단점:

- ViewModel 수정 필요.
- command implementation class 또는 async command pattern 필요.
- 테스트 추가 필요.
- 이번 최소 binding 범위가 커질 수 있다.

### Candidate Recommendation

- MVP 최소 UI binding은 Candidate 1, code-behind click handler를 후보 권장안으로 둔다.
- 이유:
  - 현재 ViewModel method가 이미 구현되어 있다.
  - command abstraction을 추가하면 scope가 커진다.
  - 이번 단계 목표는 actual OpenFileDialog와 registration runtime check를 가능하게 하는 최소 UI다.
- 단, code-behind는 ViewModel method 호출만 허용한다.
- code-behind에서 service 생성, workflow 생성, storage/file service 호출은 금지한다.

## H. Document Type Input Candidate

documentType 입력 방식 후보는 다음과 같다.

### Candidate 1. TextBox manual input

장점:

- 구현이 가장 작다.
- ViewModel 수정이 없다.
- 기존 validation/workflow allowlist가 최종 검증한다.

단점:

- 사용자가 allowed value를 모르면 실패하기 쉽다.

### Candidate 2. ComboBox static items

후보 값:

Policy:

- `policy`
- `terms`
- `contract`
- `capture`
- `etc`

Claim:

- `receipt`
- `diagnosis`
- `medicine`
- `visit`
- `admission`
- `surgery`
- `etc`

장점:

- 사용자가 허용 값을 선택하기 쉽다.
- runtime check 성공 가능성이 높다.

단점:

- target kind에 따라 목록 변경 로직 필요.
- XAML 또는 code-behind logic이 늘어난다.

### Candidate 3. ViewModel exposes documentType options

장점:

- UI와 data source가 분리된다.

단점:

- ViewModel 수정 필요.
- seed service 연결 여부가 다시 논점이 된다.

### Candidate Recommendation

- MVP 최소 binding에서는 Candidate 1 또는 Candidate 2 중 선택 가능.
- 더 안전한 runtime check를 위해 Candidate 2 static ComboBox를 후보 권장안으로 둔다.
- 단, documentType seed source-of-truth는 기존 service/storage validation에 남긴다.
- static ComboBox는 UI helper일 뿐 최종 검증 owner가 아니다.

## I. Target Kind / Target Id Candidate

현재 제약:

- Policy/Claim storage 없음.
- target id existence validation 없음.
- 실제 policy/claim selection UI 없음.

MVP 후보:

- `TargetKind`: ComboBox 또는 RadioButton
  - `policy`
  - `claim`
- `TargetId`: TextBox manual dummy input

금지:

- 실제 보험계약 번호 샘플 금지.
- 실제 청구 번호 샘플 금지.
- 실제 가족 실명 금지.
- 실제 보험사명 금지.

dummy 예시 후보:

- `POLICY-DEMO-001`
- `CLAIM-DEMO-001`

주의:

- dummy id는 실제 개인정보가 아니다.
- existence validation은 하지 않는다.
- Policy/Claim storage 설계 전까지 manual input 상태로 둔다.

## J. Runtime Check Enablement

이 UI binding이 가능하게 하는 후속 manual check는 다음과 같다.

- file select button click
- actual OpenFileDialog open
- OpenFileDialog cancel no crash
- selected dummy file display name appears
- register button click
- validation message 표시
- dummy registration execution
- `%LOCALAPPDATA%\FamilyClaimRef` operation-time creation
- metadata files 생성 위치 확인
- attachment files 생성 위치 확인
- project root pollution 없음 확인

이번 설계 문서에서는 실행하지 않는다.

## K. Scope Boundary

후속 구현에서 허용 후보:

- `MainWindow.xaml` 최소 controls 추가
- `MainWindow.xaml.cs` click handler 추가
- ViewModel method 호출
- no service creation in MainWindow
- no storage/file service direct call in MainWindow

후속 구현에서 금지 후보:

- ViewModel 수정
- AppServices 수정
- workflow/coordinator/storage/file service 수정
- Policy/Claim storage 구현
- OCR/SQLite/repository 구현
- complex styling
- navigation 구현
- real personal data
- automatic registration on startup
- automatic OpenFileDialog on startup

## L. Test / Verification Candidate

후속 구현 후 자동 검증 후보:

```powershell
dotnet build FamilyClaimRef.sln
dotnet test FamilyClaimRef.sln
```

기대:

- build PASS
- test PASS
- 총 테스트 수 216 유지 또는 별도 테스트 추가 시 증가
- 기존 tests 깨지지 않음

manual runtime check 후보:

- app launch
- MainWindow 표시
- file select button 표시
- OpenFileDialog opens only after button click
- cancel no crash
- selected file display name appears
- register validation message
- dummy registration with dummy file
- production root file creation under `%LOCALAPPDATA%`
- project root pollution 없음

## M. Needs Decision

1. MainWindow document registration UI binding 설계를 진행할 것인가?
2. 후속 구현에서 MainWindow.xaml에 최소 controls를 추가할 것인가?
3. visual styling은 최소로 제한할 것인가?
4. file select button을 추가할 것인가?
5. register button을 추가할 것인가?
6. button action은 code-behind click handler로 ViewModel method만 호출하게 할 것인가?
7. command pattern 추가는 보류할 것인가?
8. TargetKind는 policy/claim 선택 control로 둘 것인가?
9. TargetId는 manual dummy input으로 둘 것인가?
10. DocumentType은 static ComboBox 후보로 둘 것인가?
11. DisplayTitle은 TextBox로 둘 것인가?
12. ReferenceDate는 DatePicker로 둘 것인가?
13. ValidationMessage / StatusMessage / LastRegistrationSummary 표시 영역을 둘 것인가?
14. MainWindow code-behind에서 service/workflow/storage 생성은 금지할 것인가?
15. 후속 구현 후 OpenFileDialog와 registration manual runtime check를 별도 결과 문서로 기록할 것인가?

## N. Out of Scope

이번 문서에서 제외하는 범위는 다음과 같다.

- XAML 수정 없음
- MainWindow.xaml.cs 수정 없음
- AppServices 수정 없음
- App/App.xaml 수정 없음
- ViewModel 수정 없음
- file picker 수정 없음
- workflow/coordinator/storage/file service 수정 없음
- code-behind 구현 없음
- command 구현 없음
- actual app launch 없음
- actual OpenFileDialog 실행 없음
- actual file select 없음
- actual registration workflow 실행 없음
- production root file 생성 없음
- cleanup 실행 없음
- test code 수정 없음
- test file 생성 없음
- Policy/Claim storage 구현 없음
- OCR/SQLite/repository 구현 없음
- 실제 개인정보 샘플 사용 없음
- Git add/commit/reset/checkout/clean 없음

## O. Risks

남은 위험은 다음과 같다.

- code-behind click handler는 MVVM 순도가 낮다.
- command pattern을 보류하면 button enabled 상태 관리가 제한될 수 있다.
- static documentType ComboBox는 seed source-of-truth와 중복될 수 있다.
- Policy/Claim storage가 없어 target id는 manual dummy 상태다.
- dummy registration을 수행하면 `%LOCALAPPDATA%`에 test files가 생성될 수 있다.
- cleanup 정책을 별도로 기록해야 한다.
- 실제 개인정보 파일을 선택하지 않도록 주의가 필요하다.

## P. Recommendation

추천 순서:

1. 이 문서를 기준으로 사용자 결정을 받는다.
2. 사용자 결정 후 `docs/109_MAINWINDOW_DOCUMENT_REGISTRATION_UI_BINDING_USER_DECISION_RECORD.md`를 생성한다.
3. 그 다음 별도 승인으로 MainWindow 최소 UI binding을 구현한다.
4. 구현 후 build/test를 실행한다.
5. 구현 후 OpenFileDialog/runtime registration manual check 계획 또는 결과 문서를 생성한다.

## Q. Result

`MAINWINDOW_DOCUMENT_REGISTRATION_UI_BINDING_DESIGN_DRAFTED`
