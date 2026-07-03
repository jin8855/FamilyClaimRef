# Service Composition / Root Path Manual Runtime Check Plan

## A. Goal

이 문서는 service composition/root path manual runtime check 계획 문서다.

목적은 `AppServices`, `App.xaml`, `App.xaml.cs`를 통한 최소 service composition 및 `MainWindow.DataContext` 연결 이후 실제 앱 실행 전에 무엇을 확인할지 정의하는 것이다.

이 문서는 actual app launch, OpenFileDialog, production root 생성 여부, project root 오염 여부를 검증하기 위한 계획을 정리한다. 실행 결과 문서가 아니며, 이번 작업에서는 앱을 실행하지 않는다. OpenFileDialog를 실행하지 않고, 파일 선택이나 문서 등록도 하지 않는다.

## B. Current Implementation Baseline

현재 구현 baseline은 다음과 같다.

- `app/FamilyClaimRef.App/Composition/AppServices.cs`가 생성됨.
- `AppServices.CreateDefault()`가 service graph를 구성함.
- metadata root는 `%LOCALAPPDATA%\FamilyClaimRef\data\local`.
- attachment root는 `%LOCALAPPDATA%\FamilyClaimRef\attachments`.
- `App.xaml`에서 `StartupUri` 제거됨.
- `App.xaml.cs`에서 `OnStartup` override로 MainWindow 생성 및 DataContext 연결됨.
- `MainWindow.xaml` visual layout은 아직 없음.
- `MainWindow.xaml.cs`는 수정 없음.
- production root directory/file은 아직 생성되지 않음.
- project root `attachments/`, `data/local`은 files=0 상태로 기록됨.
- build/test는 PASS 상태로 기록됨.
- 총 테스트 수는 216개로 기록됨.

## C. Manual Runtime Check Purpose

manual runtime check가 필요한 이유는 다음과 같다.

- unit test는 actual WPF app launch를 검증하지 않는다.
- unit test는 actual OpenFileDialog runtime을 검증하지 않는다.
- App startup 변경은 실제 WPF runtime에서 별도 확인이 필요하다.
- `%LOCALAPPDATA%` root 권한과 생성 시점은 실제 실행에서 확인해야 한다.
- XAML UI binding이 아직 없으므로 현재 manual check는 app startup/DataContext 중심으로 제한된다.

## D. Check Preconditions

manual check 실행 전 필수 조건은 다음과 같다.

- `dotnet build FamilyClaimRef.sln` PASS.
- `dotnet test FamilyClaimRef.sln` PASS.
- 총 테스트 수 216개 기준 PASS.
- Git commit/add/reset/checkout 없음.
- 실제 개인정보 샘플 준비 금지.
- 실제 보험사명/병원명/진단명/진단코드 사용 금지.
- 테스트용 dummy 파일만 사용할 수 있음.
- production `%LOCALAPPDATA%\FamilyClaimRef` 현재 상태를 먼저 확인.
- project root `attachments/`, `data/local` 상태를 먼저 확인.

## E. Check Target Paths

production root 확인 대상:

```text
%LOCALAPPDATA%\FamilyClaimRef
%LOCALAPPDATA%\FamilyClaimRef\data\local
%LOCALAPPDATA%\FamilyClaimRef\attachments
%LOCALAPPDATA%\FamilyClaimRef\attachments\documents
```

project root 확인 대상:

```text
C:\EtcProject\FamilyClaimRef\attachments
C:\EtcProject\FamilyClaimRef\data\local
```

metadata file 후보:

```text
%LOCALAPPDATA%\FamilyClaimRef\data\local\documents.json
%LOCALAPPDATA%\FamilyClaimRef\data\local\policy-documents.json
%LOCALAPPDATA%\FamilyClaimRef\data\local\claim-documents.json
```

주의:

- App startup만으로 위 metadata files가 생성되면 안 된다.
- App startup만으로 attachment files가 생성되면 안 된다.
- file select cancel만으로 metadata/file 생성이 발생하면 안 된다.

## F. Manual Check Phase 1 - App Startup Only

목표:

- 앱이 예외 없이 시작되는지 확인한다.
- `App.xaml.cs` `OnStartup` flow가 실제 WPF runtime에서 동작하는지 확인한다.
- MainWindow가 표시되는지 확인한다.
- App startup만으로 production root directory/file이 생성되지 않는지 확인한다.

실행 후보:

```powershell
dotnet run --project app/FamilyClaimRef.App/FamilyClaimRef.App.csproj
```

확인 항목:

- app starts without exception.
- MainWindow shows.
- immediate crash 없음.
- MainWindow DataContext 연결로 인한 startup exception 없음.
- visual layout이 비어 있거나 최소 상태여도 정상.
- actual OpenFileDialog가 자동으로 열리지 않음.
- registration workflow가 자동 실행되지 않음.
- `%LOCALAPPDATA%\FamilyClaimRef` root directory 생성 여부 기록.
- metadata files 생성 여부 기록.
- attachment files 생성 여부 기록.
- project root `attachments/`, `data/local` 오염 없음.

주의:

- 이번 문서 작성 단계에서는 실행하지 않는다.
- 이 phase는 후속 실행 문서에서만 수행한다.

## G. Manual Check Phase 2 - Window / DataContext Observation

목표:

- MainWindow가 실제로 생성되는지 확인한다.
- DataContext 설정으로 인한 runtime binding exception이 없는지 확인한다.

현재 한계:

- `MainWindow.xaml`에는 visual controls/bindings가 없으므로 binding 동작 검증은 제한적이다.
- DataContext type 확인은 debugger 또는 임시 log 없이는 직접 확인이 어렵다.
- 임시 log 추가는 별도 승인 없이는 금지한다.

확인 항목:

- window created.
- window shown.
- no unhandled exception.
- no startup binding error.
- no file system side effect beyond expected.

주의:

- UI controls가 없으므로 file select/register interaction은 아직 불가할 수 있다.
- 이 경우 “UI interaction not available”로 기록한다.

## H. Manual Check Phase 3 - OpenFileDialog Candidate

목표:

- actual `OpenFileDialog` runtime 동작 검증 계획을 정리한다.

현재 한계:

- XAML UI binding/file select button이 없으면 사용자가 OpenFileDialog를 열 수 없다.
- 따라서 Phase 3은 현재 즉시 실행 대상이 아니라 XAML 최소 binding 이후 실행 후보로 둔다.

후속 확인 항목:

- file select button click opens OpenFileDialog.
- cancel does not crash.
- cancel does not create metadata files.
- cancel does not create attachment files.
- selected dummy file display name appears.
- actual file copy는 register 전까지 발생하지 않음.

주의:

- 이번 계획 문서에서는 실행하지 않는다.
- OpenFileDialog 자동 실행 금지.
- 실제 개인정보 파일 선택 금지.

## I. Manual Check Phase 4 - Registration Candidate

목표:

- actual registration operation 검증 계획을 정리한다.

현재 한계:

- XAML UI binding이 없으므로 현재 즉시 실행 대상이 아니다.
- Policy/Claim storage가 없으므로 target id는 manual dummy input 상태다.

후속 확인 항목:

- dummy source file 선택.
- dummy policyId 또는 claimId 입력.
- allowed documentType 입력.
- displayTitle 입력.
- referenceDate 입력.
- register 실행.
- metadata files 생성 위치 확인.
- attachment file 생성 위치 확인.
- metadata에 absolute path 저장되지 않는지 확인.
- `DocumentRecord.RelativePath`가 attachment root 기준 relative path인지 확인.
- project root `attachments/`, `data/local` 오염 없음.

금지:

- 실제 가족 실명 사용 금지.
- 실제 보험계약 번호 사용 금지.
- 실제 청구 번호 사용 금지.
- 실제 보험사명 사용 금지.
- 실제 병원명 사용 금지.
- 실제 진단명/진단코드 사용 금지.

## J. Expected Results Matrix

| Phase | Action | Expected Result | File System Expected Result | Pass/Fail Criteria | Notes |
|---|---|---|---|---|---|
| App startup | run app | MainWindow appears | no metadata/attachment files created | no startup crash | startup-only phase |
| Startup root side effect | inspect `%LOCALAPPDATA%\FamilyClaimRef` | root may remain absent | no JSON files | no startup-created metadata | AppServices should compute paths only |
| Project root pollution | inspect project root folders | files=0 | no files under project `attachments/`, `data/local` | project root remains clean | required safety check |
| OpenFileDialog | click file select | dialog opens | no metadata/file created on cancel | deferred until UI binding | not currently executable |
| Registration | register dummy document | files created under user app data only | metadata/attachment under `%LOCALAPPDATA%` only | deferred until UI binding | requires dummy-only data |

## K. Stop Conditions

manual runtime check 즉시 중단 조건은 다음과 같다.

- app startup crash.
- unhandled exception.
- automatic OpenFileDialog launch on startup.
- registration workflow auto-executes on startup.
- production metadata files created during startup only.
- project root `attachments/` or `data/local` unexpectedly receives files.
- actual personal/private document accidentally selected.
- actual personal sample data entered.

중단 후 기록할 내용:

- executed command.
- observed exception.
- created file paths.
- project root pollution 여부.
- production root 생성 여부.
- screenshot 필요 여부.
- rollback/manual cleanup 필요 여부.

## L. Cleanup Policy Candidate

manual check 후 cleanup 정책 후보는 다음과 같다.

- startup-only check에서 파일이 생성되지 않았으면 cleanup 없음.
- dummy registration check 후에는 생성된 `%LOCALAPPDATA%\FamilyClaimRef` 테스트 데이터 삭제 후보.
- 삭제 전 생성 파일 목록을 문서에 기록.
- 실제 개인정보 파일이 포함되면 즉시 중단하고 별도 정리.

주의:

- 이번 문서에서는 삭제 작업을 하지 않는다.
- cleanup 실행은 manual runtime check 결과 문서에서 별도 지시 후 수행한다.

## M. Evidence To Record

manual check 결과 문서에 기록해야 할 증거는 다음과 같다.

- 실행 명령
- 실행 시각
- build/test 상태
- app launch 성공/실패
- MainWindow 표시 여부
- startup exception 여부
- OpenFileDialog 실행 여부
- registration workflow 실행 여부
- `%LOCALAPPDATA%\FamilyClaimRef` 존재 여부
- metadata files 존재 여부
- attachment files 존재 여부
- project root `attachments/` files count
- project root `data/local` files count
- DB/SQLite file 생성 여부
- 실제 개인정보 샘플 사용 여부
- cleanup 여부

## N. Out of Scope

이번 문서에서 제외하는 범위는 다음과 같다.

- actual app launch 없음
- actual OpenFileDialog 실행 없음
- actual file select 없음
- actual registration workflow 실행 없음
- actual production root file 생성 없음
- cleanup 실행 없음
- C# 수정 없음
- AppServices 수정 없음
- App/MainWindow 수정 없음
- XAML 수정 없음
- ViewModel 수정 없음
- workflow/coordinator/storage/file service 수정 없음
- test code 수정 없음
- test file 생성 없음
- Policy/Claim storage 구현 없음
- OCR/SQLite/repository 구현 없음
- 실제 개인정보 샘플 사용 없음
- Git add/commit/reset/checkout 없음

## O. Risks

남은 위험은 다음과 같다.

- App startup 변경은 아직 실제 WPF runtime에서 검증되지 않았다.
- XAML controls가 없어 OpenFileDialog/manual registration 검증은 아직 제한된다.
- production root 권한 문제는 operation 시점에야 드러날 수 있다.
- registration check를 수행하면 `%LOCALAPPDATA%`에 실제 test files가 생성될 수 있다.
- cleanup 정책을 사전에 정하지 않으면 test residue가 남을 수 있다.
- 실제 개인정보 파일을 선택하지 않도록 강한 주의가 필요하다.

## P. Recommendation

추천 순서:

1. 이 계획 문서를 기준으로 manual runtime check 실행 여부를 사용자에게 확인받는다.
2. 후속 작업으로 `docs/107_SERVICE_COMPOSITION_ROOT_PATH_MANUAL_RUNTIME_CHECK_RESULT.md`를 생성한다.
3. manual runtime check는 startup-only phase부터 수행한다.
4. XAML UI binding이 없으면 OpenFileDialog/registration phase는 deferred로 기록한다.
5. startup-only check가 통과하면 다음 작업으로 최소 XAML UI binding 설계를 진행한다.

## Q. Result

`SERVICE_COMPOSITION_ROOT_PATH_MANUAL_RUNTIME_CHECK_PLAN_DRAFTED`
