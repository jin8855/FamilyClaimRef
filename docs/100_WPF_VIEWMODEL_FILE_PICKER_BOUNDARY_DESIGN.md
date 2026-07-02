# WPF ViewModel / File Picker Boundary Design

## A. Goal

이 문서는 WPF ViewModel/file picker boundary 설계 문서다.

목적은 `DocumentRegistrationWorkflow` 구현 이후 WPF UI가 문서 등록 workflow를 어떤 경계로 호출해야 하는지 검토하는 것이다.

포함 내용:

- UI가 `DocumentRegistrationWorkflow`를 어떻게 호출할지 검토한다.
- ViewModel이 lower-level storage/file service를 직접 알지 않게 하는 방향을 검토한다.
- file picker를 ViewModel에 직접 넣을지, abstraction으로 분리할지 검토한다.
- ViewModel 상태, 입력값, 오류 표시, 테스트 범위 후보를 정리한다.

이 문서는 실제 구현 문서가 아니다.

- C# 구현은 하지 않는다.
- XAML 수정은 하지 않는다.
- file picker 구현은 하지 않는다.
- service composition 구현은 하지 않는다.
- 실제 파일 선택, 복사, 저장은 수행하지 않는다.

## B. Current State

현재 상태:

- JSON metadata storage 구현 완료.
- file attachment primitive 구현 완료.
- `DocumentAttachmentCoordinator` 구현 완료.
- `DocumentLinkCoordinator` 구현 완료.
- `DocumentRegistrationWorkflow` 구현 완료.
- `PolicyDocumentRegistrationRequest` 구현 완료.
- `ClaimDocumentRegistrationRequest` 구현 완료.
- `PolicyDocumentRegistrationResult` 구현 완료.
- `ClaimDocumentRegistrationResult` 구현 완료.
- combined workflow test PASS 기록이 있다.
- 전체 테스트 201개 PASS 기록이 있다.
- `DocumentRegistrationWorkflow`는 file import, `DocumentRecord` metadata 저장, Policy/Claim link 저장을 사용자 작업 단위로 묶는다.
- link 실패 시 copied file delete와 created `DocumentRecord` disable을 시도한다.
- WPF UI/ViewModel/file picker 연동은 아직 없다.
- ViewModel 파일은 아직 없다.
- file picker 관련 파일은 아직 없다.
- UI binding은 아직 없다.
- Policy/Claim target selection UI는 아직 없다.
- Policy/Claim storage가 없어 target id existence validation은 아직 없다.
- app composition/root path 결정은 아직 없다.
- production `attachments/`, `data/local` 권한 문제는 temp directory 기반 테스트로만 간접 검증되어 있다.
- project root `attachments/`, `data/local` 내부 파일 생성은 없다.

현재 WPF entry 상태:

- `App.xaml`은 `StartupUri="MainWindow.xaml"` 기본 구조다.
- `App.xaml.cs`는 기본 `Application` partial class만 가진다.
- `MainWindow.xaml`은 빈 `Grid`를 가진 기본 window다.
- `MainWindow.xaml.cs`는 `InitializeComponent()`만 호출한다.

## C. Problem Statement

backend/application workflow는 준비되었지만 WPF UI가 어떤 경계로 호출해야 할지 아직 정해지지 않았다.

핵심 문제:

- UI/ViewModel이 `JsonDocumentStorageService`, `LocalFileAttachmentService`, `DocumentAttachmentCoordinator`, `DocumentLinkCoordinator`를 직접 조합하면 workflow/rollback 책임이 UI로 새어 나간다.
- `DocumentRegistrationWorkflow`가 사용자 작업 단위의 application boundary이므로 UI는 이 workflow를 중심으로 호출해야 한다.
- file picker는 WPF UI 기술에 속하므로 ViewModel이 `OpenFileDialog`를 직접 알면 testability가 낮아진다.
- production metadata root와 attachment root를 어디에서 구성할지 결정해야 한다.
- Policy/Claim storage가 아직 없으므로 policyId/claimId 선택 UI는 임시 manual input 또는 local candidate list가 필요할 수 있다.
- 오류 표시 정책이 아직 없다.
- 실제 파일 선택, 복사, 저장은 project root를 오염시킬 수 있으므로 UI boundary가 명확해야 한다.

## D. Existing Application Boundary

### `DocumentRegistrationWorkflow`

해당:

- policy document registration.
- claim document registration.
- file import + metadata save + link save 조합.
- link 실패 시 copied file cleanup.
- link 실패 시 created `DocumentRecord` disable.
- rollback failure 노출.

UI가 직접 하지 않아야 하는 것:

- `DocumentAttachmentCoordinator` 직접 호출.
- `DocumentLinkCoordinator` 직접 호출.
- `JsonDocumentStorageService` 직접 호출.
- `LocalFileAttachmentService` 직접 호출.
- `FileNamePolicyService` 직접 호출.
- physical file name 직접 생성.
- duplicateIndex 직접 계산.
- rollback 직접 처리.
- JSON metadata 직접 저장.
- actual file copy 직접 수행.

## E. Candidate UI Boundary Options

### Candidate 1. ViewModel이 모든 service를 직접 조합

내용:

- ViewModel이 file picker, workflow, attachment coordinator, link coordinator, storage service 등을 직접 호출한다.

장점:

- 초기 구현이 빠를 수 있다.

단점:

- ViewModel이 비대해진다.
- workflow/rollback rule이 UI로 새어 나온다.
- 테스트가 어려워진다.
- 기존 application service 분리 취지와 충돌한다.

판정:

- 추천하지 않는다.

### Candidate 2. ViewModel은 `DocumentRegistrationWorkflow`만 호출하고 file picker는 code-behind가 처리

내용:

- code-behind가 file picker를 열고 source path를 ViewModel에 전달한다.
- ViewModel은 `DocumentRegistrationWorkflow`만 호출한다.

장점:

- ViewModel이 WPF dialog를 직접 알지 않는다.
- 구현이 비교적 단순하다.
- MVP 임시 구현으로는 가능하다.

단점:

- code-behind와 ViewModel 간 전달 규칙이 필요하다.
- file picker 흐름을 unit test로 대체하기 어렵다.
- UI 기술 경계가 문서화되지 않으면 code-behind 책임이 커질 수 있다.

판정:

- MVP 임시 후보로는 가능하지만 우선 추천은 아니다.

### Candidate 3. `IFilePickerService` abstraction 도입

내용:

- ViewModel은 `IFilePickerService`를 통해 파일 선택을 요청한다.
- WPF 구현체가 `OpenFileDialog`를 감싼다.
- ViewModel은 `IFilePickerService`와 `DocumentRegistrationWorkflow`만 의존한다.

장점:

- MVVM 경계가 명확하다.
- test에서 file picker를 fake로 대체할 수 있다.
- ViewModel이 WPF dialog API에 직접 의존하지 않는다.
- workflow/rollback 책임은 application workflow에 남는다.

단점:

- interface와 WPF 구현체가 추가된다.
- MVP 기준 구현 파일 수가 늘어난다.

판정:

- Candidate Recommendation.

### Candidate 4. ViewModel이 `OpenFileDialog`를 직접 호출

내용:

- ViewModel에서 `Microsoft.Win32.OpenFileDialog`를 직접 사용한다.

장점:

- 가장 단순하게 구현할 수 있다.

단점:

- ViewModel이 WPF UI 기술에 직접 의존한다.
- 테스트가 어렵다.
- MVVM boundary가 흐려진다.

판정:

- 추천하지 않는다.

## F. Recommended Direction

Candidate Recommendation:

- Candidate 3, `IFilePickerService` abstraction 도입을 추천한다.
- ViewModel은 `DocumentRegistrationWorkflow`와 `IFilePickerService`만 직접 의존한다.
- ViewModel은 lower-level coordinator/storage/file service를 직접 조합하지 않는다.
- WPF file picker 구현체는 이후 구현 단계에서 `OpenFileDialog`를 감싼다.
- 이번 문서에서는 구현하지 않는다.

주의:

- 이 추천은 확정 결정이 아니다.
- 사용자 결정 문서에서 Accepted 여부를 별도로 기록해야 한다.

## G. ViewModel Responsibility Candidate

ViewModel 해당 후보:

- selected source file path 보관.
- selected source file display name 표시.
- target kind 선택.
  - policy.
  - claim.
- target id 입력 또는 선택.
  - policyId.
  - claimId.
- documentType 선택.
- displayTitle 입력.
- referenceDate 입력.
- file select command 실행.
- register command 실행.
- busy state 관리.
- validation message 표시.
- success/failure message 표시.
- registration result summary 표시.

ViewModel이 하지 말아야 하는 것:

- physical file name 직접 생성.
- duplicateIndex 직접 계산.
- actual file copy 직접 수행.
- JSON metadata 직접 저장.
- rollback 직접 수행.
- `DocumentAttachmentCoordinator` 직접 조합.
- `DocumentLinkCoordinator` 직접 조합.
- `FileNamePolicyService` 직접 호출.
- actual file path rule 직접 계산.
- `OpenFileDialog` 직접 호출.

## H. File Picker Boundary Candidate

후보 interface:

```csharp
public interface IFilePickerService
{
    Task<FilePickerResult?> PickDocumentFileAsync(
        CancellationToken cancellationToken = default);
}
```

후보 result:

```csharp
public sealed record FilePickerResult(
    string SourceFilePath,
    string SafeDisplayName);
```

기준:

- `SourceFilePath`는 workflow 입력값이다.
- `SafeDisplayName`은 UI 표시값이다.
- raw original file name은 metadata에 저장하지 않는다.
- ViewModel은 file picker result를 registration request 생성에 사용한다.
- actual file copy는 file picker가 아니라 `DocumentRegistrationWorkflow` 하위 service가 수행한다.

대안:

- code-behind에서 file picker를 열고 ViewModel property에 path를 전달할 수 있다.
- MVP 단순성을 우선하면 code-behind 방식도 후보로 남길 수 있다.
- 다만 testability와 MVVM boundary 관점에서는 `IFilePickerService` 방식이 더 적합하다.

## I. Registration Form Candidate

공통 입력 후보:

- source file.
- displayTitle.
- documentType.
- referenceDate.

policy registration 입력 후보:

- policyId.

claim registration 입력 후보:

- claimId.

target kind 방식 후보:

1. policy/claim tab 분리.
2. target kind radio 선택.
3. 별도 화면 분리.

추천 후보:

- MVP 1차는 policy/claim tab 또는 target kind radio 중 하나를 후보로 둔다.
- Policy/Claim storage가 없으므로 policyId/claimId는 manual dummy input 또는 local candidate list 후보로 둔다.
- 실제 target selection UI는 Policy/Claim storage 설계 이후 강화한다.

## J. Root Path / Composition Candidate

필요 root 후보:

- metadata root.
  - 후보: `data/local/`.
- attachment root.
  - 후보: `attachments/`.

검토 위치 후보:

- `App.xaml.cs`.
- `MainWindow.xaml.cs`.
- 별도 composition root class.

composition 방식 후보:

1. App startup에서 service graph 구성.
2. MainWindow 생성 시 service graph 구성.
3. 간단한 `AppServices` factory class 도입.

주의:

- 이번 문서에서는 구현하지 않는다.
- root path 정책은 production 실제 파일 생성과 연결되므로 별도 결정이 필요하다.
- test는 계속 temp directory를 사용한다.
- `attachments/`, `data/local`은 Git 추적 제외 대상이어야 한다.

## K. Error / Status Boundary Candidate

exception source 후보:

- validation failure.
- file picker cancel.
- file copy failure.
- metadata storage failure.
- link failure.
- rollback failure.
- `AggregateException`.

MVP error 표시 후보:

- MVP에서는 custom exception 없이 기존 exception message를 내부 log/debug용으로 보관한다.
- 사용자에게는 간단한 user message로 변환한다.
- 상세 exception은 UI에 그대로 노출하지 않는다.

사용자 메시지 후보:

- 파일을 선택해 주세요.
- 저장할 대상을 입력해 주세요.
- 문서 등록에 실패했습니다.
- 등록 중 일부 정리가 실패했습니다. 다시 시도하거나 관리자에게 문의하세요.

보류:

- error classification은 이후 hardening 후보로 둔다.
- custom exception 도입은 이후 별도 결정으로 둔다.

## L. Test Scope Candidate

ViewModel/file picker boundary test 후보:

- selecting file updates selected file state.
- cancel file picker does not change selected file.
- register policy document calls `DocumentRegistrationWorkflow`.
- register claim document calls `DocumentRegistrationWorkflow`.
- missing source file path disables command or returns validation error.
- missing policyId rejected before workflow call.
- missing claimId rejected before workflow call.
- workflow success updates success state.
- workflow failure updates error state.
- busy state set during command.
- lower-level services are not directly called by ViewModel.
- fake file picker used in test.
- no actual file picker UI shown in tests.
- no actual project `attachments/`, `data/local` file creation.

제외 후보:

- actual `OpenFileDialog` test.
- WPF visual automation test.
- real file copy integration test.
- JSON storage integration test.
- OCR test.
- SQLite test.
- Policy/Claim storage existence validation test.

## M. Needs Decision

사용자 결정 질문 후보:

1. WPF UI/ViewModel boundary를 설계할 것인가?
2. ViewModel은 `DocumentRegistrationWorkflow`만 workflow 진입점으로 호출하게 할 것인가?
3. ViewModel은 `DocumentAttachmentCoordinator`, `DocumentLinkCoordinator`, storage/file service를 직접 조합하지 않게 할 것인가?
4. file picker는 ViewModel 직접 `OpenFileDialog` 호출이 아니라 별도 boundary로 분리할 것인가?
5. `IFilePickerService` abstraction을 도입할 것인가?
6. file picker cancel은 no-op으로 처리할 것인가?
7. ViewModel은 source file path, target kind, target id, documentType, displayTitle, referenceDate를 상태로 가질 것인가?
8. Policy/Claim storage가 없으므로 policyId/claimId는 MVP에서 manual dummy input 후보로 둘 것인가?
9. production metadata root 후보를 `data/local/`로 유지할 것인가?
10. production attachment root 후보를 `attachments/`로 유지할 것인가?
11. service composition은 App startup 또는 별도 composition root 후보로 둘 것인가?
12. MVP error 표시는 간단한 user message로 변환하고 custom exception 분류는 보류할 것인가?
13. 이후 구현 시 ViewModel/file picker tests를 추가할 것인가?
14. actual `OpenFileDialog`는 unit test에서 호출하지 않을 것인가?
15. WPF UI/XAML 구현은 boundary 결정 후 별도 단계로 진행할 것인가?

## N. Out of Scope

이번 문서에서 제외하는 범위:

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

## O. Risks

남은 위험:

- Policy/Claim storage가 없어 target selection이 임시/manual input에 머무를 수 있다.
- production root path 권한 문제는 아직 실제 UI에서 검증되지 않았다.
- file picker에서 들어오는 source path trust boundary를 검토해야 한다.
- custom exception이 없어 UI error classification이 거칠 수 있다.
- ViewModel이 workflow 대신 lower-level service를 직접 호출하면 기존 책임 분리가 깨질 수 있다.
- UI 구현 전까지 실제 사용자 흐름은 검증되지 않는다.
- `IFilePickerService`를 도입하면 MVP 구현 파일 수가 늘어난다.
- code-behind 방식으로 시작하면 이후 MVVM boundary 정리가 추가로 필요할 수 있다.

## P. Recommendation

추천 순서:

1. 이 문서를 기준으로 WPF ViewModel/file picker boundary 결정을 받는다.
2. 사용자 결정 후 `docs/101_WPF_VIEWMODEL_FILE_PICKER_BOUNDARY_USER_DECISION_RECORD.md`를 생성한다.
3. 이후 별도 승인으로 ViewModel/file picker interface/test 구현 범위를 정한다.
4. XAML UI 구현은 ViewModel boundary 구현 이후로 보류한다.
5. Policy/Claim storage와 target selection은 별도 후속 설계로 둔다.

## Q. Result

`WPF_VIEWMODEL_FILE_PICKER_BOUNDARY_DESIGN_DRAFTED`
