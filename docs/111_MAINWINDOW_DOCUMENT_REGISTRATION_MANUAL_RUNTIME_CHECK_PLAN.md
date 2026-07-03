# MainWindow Document Registration Manual Runtime Check Plan

## A. Goal

이 문서는 MainWindow document registration manual runtime check 계획 문서다.

목적은 실제 WPF runtime에서 UI binding, OpenFileDialog, validation, dummy registration, production root side effect를 수동으로 검증하기 위한 절차와 증거 기록 기준을 정리하는 것이다.

이 문서는 실행 결과 문서가 아니다.
이번 문서 작성 중에는 app launch, OpenFileDialog 실행, file select, registration workflow 실행, dummy registration, cleanup을 수행하지 않는다.

## B. Current Implementation Baseline

- MainWindow UI binding 구현 완료.
- `MainWindow.xaml`에 document registration controls가 있다.
- file select button이 있다.
- register button이 있다.
- target kind selector가 있다.
- target id TextBox가 있다.
- documentType static ComboBox가 있다.
- displayTitle TextBox가 있다.
- referenceDate DatePicker가 있다.
- validation/status/result summary 표시 영역이 있다.
- `MainWindow.xaml.cs` click handler는 `SelectFileAsync(...)`, `RegisterAsync(...)`만 호출한다.
- command pattern은 없다.
- converter는 없다.
- AppServices/App/App.xaml/ViewModel/file picker/workflow/storage 수정은 없다.
- build/test는 `docs/110_MAINWINDOW_DOCUMENT_REGISTRATION_UI_BINDING_IMPLEMENTATION_REVIEW.md` 기준 PASS다.
- 총 테스트 수는 216개다.
- actual app launch는 UI binding 이후 아직 없다.
- actual OpenFileDialog runtime 검증은 아직 없다.
- actual file select 검증은 아직 없다.
- registration runtime 검증은 아직 없다.
- production root operation-time 생성 검증은 아직 없다.
- `ReferenceDate`는 ViewModel에서 `DateOnly`이고 DatePicker는 최소 binding 상태라 runtime 표시/입력 확인이 필요하다.
- Policy/Claim storage는 없다.
- target id는 manual dummy input 상태다.
- production root 후보:
  - `%LOCALAPPDATA%\FamilyClaimRef\data\local`
  - `%LOCALAPPDATA%\FamilyClaimRef\attachments`

## C. Manual Runtime Check Purpose

manual runtime check가 필요한 이유:

- XAML binding은 build/test만으로 충분히 검증되지 않는다.
- actual OpenFileDialog는 unit test에서 검증하지 않는다.
- file select cancel side effect를 확인해야 한다.
- dummy registration이 실제로 user app data root 아래에만 파일을 생성하는지 확인해야 한다.
- project root pollution이 없는지 확인해야 한다.
- DateOnly/DatePicker binding이 실제 화면에서 깨지는지 확인해야 한다.

## D. Check Preconditions

수동 runtime check 전 필수 조건:

- `dotnet build FamilyClaimRef.sln` PASS.
- `dotnet test FamilyClaimRef.sln` PASS.
- 총 테스트 216개 기준 PASS.
- 실제 개인정보 파일 사용 금지.
- 실제 가족 실명 사용 금지.
- 실제 보험계약 번호 사용 금지.
- 실제 청구 번호 사용 금지.
- 실제 보험사명 사용 금지.
- 실제 병원명 사용 금지.
- 실제 진단명/진단코드 사용 금지.
- dummy file만 사용.
- dummy target id만 사용.
- production `%LOCALAPPDATA%\FamilyClaimRef` 현재 상태 사전 확인.
- project root `attachments/`, `data/local` 상태 사전 확인.
- cleanup 정책 확인.

## E. Dummy Test Data Policy

허용 dummy target id:

```text
POLICY-DEMO-001
CLAIM-DEMO-001
```

허용 dummy file 후보:

- 임시 텍스트 파일
- dummy pdf
- dummy png/jpg
- 내용은 개인정보가 아닌 임의 문자열만 포함

금지:

- 실제 가족 문서
- 실제 보험 증권
- 실제 병원 영수증
- 실제 진단서
- 실제 처방전
- 실제 주민번호/생년월일/전화번호/주소 포함 파일
- 실제 보험사명/병원명/진단명/진단코드 포함 파일

dummy file 생성 후보:

```powershell
$dummyDir = Join-Path $env:TEMP "FamilyClaimRefManualCheck"
New-Item -ItemType Directory -Force -Path $dummyDir
$dummyFile = Join-Path $dummyDir "dummy-receipt.txt"
"dummy manual check file - no personal data" | Set-Content -Encoding UTF8 $dummyFile
```

주의:

- dummy file 생성은 후속 runtime check 결과 작업에서만 수행한다.
- 이번 계획 문서에서는 dummy file을 생성하지 않는다.

## F. Check Target Paths

production root 확인 대상:

```text
%LOCALAPPDATA%\FamilyClaimRef
%LOCALAPPDATA%\FamilyClaimRef\data\local
%LOCALAPPDATA%\FamilyClaimRef\attachments
%LOCALAPPDATA%\FamilyClaimRef\attachments\documents
```

metadata file 후보:

```text
%LOCALAPPDATA%\FamilyClaimRef\data\local\documents.json
%LOCALAPPDATA%\FamilyClaimRef\data\local\policy-documents.json
%LOCALAPPDATA%\FamilyClaimRef\data\local\claim-documents.json
```

project root 확인 대상:

```text
C:\EtcProject\FamilyClaimRef\attachments
C:\EtcProject\FamilyClaimRef\data\local
```

DB/SQLite 확인 후보:

```text
*.db
*.sqlite
*.sqlite3
```

## G. Manual Check Phase 1 - App Launch / UI Display

실행 후보:

```powershell
dotnet run --project app/FamilyClaimRef.App/FamilyClaimRef.App.csproj --no-build
```

확인 항목:

- app starts without exception.
- MainWindow shows.
- immediate crash 없음.
- document registration panel 표시.
- scope notice 표시.
- file select button 표시.
- register button 표시.
- target kind selector 표시.
- target id TextBox 표시.
- documentType ComboBox 표시.
- displayTitle TextBox 표시.
- referenceDate DatePicker 표시.
- validation/status/result area 표시.
- OpenFileDialog 자동 실행 없음.
- registration workflow 자동 실행 없음.
- startup-only metadata/attachment file 생성 없음.

## H. Manual Check Phase 2 - DatePicker / ReferenceDate Binding

목표:

- `DateOnly` ViewModel property와 WPF DatePicker binding runtime 상태 확인.

확인 항목:

- app launch 시 binding exception 여부.
- DatePicker 초기 표시 여부.
- date 선택 가능 여부.
- date 선택 후 crash 없음.
- date 선택 후 validation/register 흐름에서 문제 없는지 후보 기록.

판정:

- PASS
- PASS_WITH_NOTES
- FAIL
- UNKNOWN

실패 시 기록:

- exception message
- binding error
- affected control
- next recommendation

## I. Manual Check Phase 3 - OpenFileDialog Cancel

목표:

- file select button click 시 OpenFileDialog가 열리는지 확인.
- cancel 시 side effect가 없는지 확인.

확인 항목:

- button click 전 dialog 자동 실행 없음.
- file select button click 시 OpenFileDialog 표시.
- cancel 선택.
- app crash 없음.
- SelectedSourceFileDisplayName 변화 여부 기록.
- ValidationMessage/StatusMessage 변화 여부 기록.
- metadata JSON file 생성 없음.
- attachment file 생성 없음.
- project root pollution 없음.

금지:

- 이 phase에서는 file select를 완료하지 않는다.
- registration을 실행하지 않는다.

## J. Manual Check Phase 4 - Dummy File Select

목표:

- dummy file 선택 후 UI 표시 상태 확인.

절차:

1. dummy file 생성.
2. file select button click.
3. dummy file 선택.
4. selected file display name 표시 확인.
5. metadata/attachment file이 아직 생성되지 않았는지 확인.

확인 항목:

- selected file display name 표시.
- full path가 부주의하게 UI에 노출되는지 확인.
- app crash 없음.
- OpenFileDialog boundary 정상.
- file copy가 아직 발생하지 않음.
- metadata JSON file 생성 없음.
- attachment file 생성 없음.
- project root pollution 없음.

## K. Manual Check Phase 5 - Validation Without Required Inputs

목표:

- register click 시 ViewModel validation message가 표시되는지 확인.

후보 케이스:

1. source file 없이 register click.
2. TargetId 비우고 register click.
3. DocumentType 비우고 register click.
4. DisplayTitle 비우고 register click.
5. ReferenceDate invalid/default 상태 후보.

확인 항목:

- validation message 표시.
- workflow 호출 없이 실패하는지 확인.
- metadata/attachment file 생성 없음.
- project root pollution 없음.

주의:

- workflow 호출 여부는 파일 생성 side effect와 UI message 중심으로 관찰한다.
- instrumentation 추가는 금지한다.

## L. Manual Check Phase 6 - Dummy Registration

목표:

- dummy-only data로 실제 registration을 수행하고 operation-time file creation을 확인한다.

입력 후보:

Policy case:

```text
TargetKind: policy
TargetId: POLICY-DEMO-001
DocumentType: policy
DisplayTitle: Dummy Policy Document
ReferenceDate: 오늘 날짜
SourceFile: dummy-receipt.txt
```

Claim case:

```text
TargetKind: claim
TargetId: CLAIM-DEMO-001
DocumentType: receipt
DisplayTitle: Dummy Claim Receipt
ReferenceDate: 오늘 날짜
SourceFile: dummy-receipt.txt
```

확인 항목:

- register click 후 crash 없음.
- success/failure status message 표시.
- metadata root 생성 위치.
- `documents.json` 생성 여부.
- `policy-documents.json` 또는 `claim-documents.json` 생성 여부.
- attachment file 생성 위치.
- attachment file이 `%LOCALAPPDATA%\FamilyClaimRef\attachments\documents` 아래에 생성되는지 확인.
- project root `attachments/`, `data/local` 오염 없음.
- metadata에 absolute source path 저장 여부 확인.
- `DocumentRecord.RelativePath`가 root-relative인지 확인.
- duplicate registration 시 filename duplicateIndex 처리 후보는 후속으로 둘 수 있음.

주의:

- Policy/Claim storage가 없으므로 target existence validation은 하지 않는다.
- 실제 개인정보 파일 금지.
- 실제 보험/병원/진단 샘플 금지.

## M. Cleanup Plan

후속 runtime check 후 cleanup 후보:

```powershell
Remove-Item -Recurse -Force "$env:LOCALAPPDATA\FamilyClaimRef"
Remove-Item -Recurse -Force "$env:TEMP\FamilyClaimRefManualCheck"
```

주의:

- cleanup은 후속 결과 문서에서 생성 파일 목록을 기록한 뒤 수행한다.
- 이번 계획 문서에서는 cleanup을 실행하지 않는다.
- 실제 개인정보가 포함된 파일이 생성되면 즉시 중단하고 별도 정리한다.

## N. Expected Results Matrix

| Phase | Action | Expected UI Result | Expected File System Result | Pass/Fail Criteria | Notes |
|---|---|---|---|---|---|
| 1. App launch/UI display | app 실행 | MainWindow와 등록 UI 표시 | startup-only 파일 생성 없음 | crash 없음, 자동 dialog/workflow 없음 | app launch는 후속 작업에서만 수행 |
| 2. DatePicker/ReferenceDate binding | DatePicker 확인/날짜 선택 | 날짜 표시/선택 가능 | 파일 생성 없음 | binding crash 없음 | `DateOnly` runtime 확인 필요 |
| 3. OpenFileDialog cancel | file select click 후 cancel | dialog 표시 후 cancel 가능 | metadata/attachment 생성 없음 | cancel 후 crash 없음 | file 선택 완료 금지 |
| 4. Dummy file select | dummy file 선택 | selected display name 표시 | metadata/attachment 생성 없음 | file copy 없음 | full path 노출 여부 확인 |
| 5. Validation without required inputs | 필수값 누락 후 register | validation message 표시 | metadata/attachment 생성 없음 | workflow side effect 없음 | instrumentation 추가 금지 |
| 6. Dummy registration | dummy-only data로 register | success/failure status 표시 | `%LOCALAPPDATA%\FamilyClaimRef` 아래 생성 | project root 오염 없음 | 실제 개인정보 금지 |
| 7. Cleanup | 생성 파일 목록 기록 후 cleanup | app 종료 후 정리 | runtime check 파일 제거 | cleanup target 명확 | 이번 문서에서는 실행 없음 |

## O. Stop Conditions

즉시 중단 조건:

- app startup crash
- DatePicker binding crash
- unhandled exception
- automatic OpenFileDialog on startup
- automatic registration workflow on startup
- OpenFileDialog opens without button click
- registration happens without button click
- metadata/attachment files created before registration
- project root pollution
- actual personal/private document selected
- actual personal sample data entered
- cleanup target path ambiguous

중단 시 기록:

- executed command
- observed exception
- current phase
- created file paths
- whether cleanup is needed
- next recommendation

## P. Evidence To Record

결과 문서에 기록할 evidence 항목:

- 실행 명령
- 실행 시각
- build/test 상태
- app launch result
- MainWindow 표시 여부
- UI controls 표시 여부
- DatePicker binding result
- OpenFileDialog cancel result
- dummy file select result
- validation message result
- dummy registration result
- production root before/after
- metadata files before/after
- attachment files before/after
- project root `attachments/`, `data/local` before/after
- DB/SQLite file 생성 여부
- cleanup 여부
- 실제 개인정보 샘플 사용 여부
- issue list
- result status

## Q. Deferred / Not Covered

- real policy/claim selection
- Policy/Claim storage existence validation
- OCR
- SQLite/repository
- final UI design/styling
- command pattern
- duplicate registration detailed matrix
- production installer/environment check
- accessibility check
- localization

## R. Out of Scope

- actual app launch 없음
- actual OpenFileDialog 실행 없음
- actual file select 없음
- actual registration workflow 실행 없음
- dummy file 생성 없음
- dummy registration 없음
- cleanup 실행 없음
- XAML 수정 없음
- C# 수정 없음
- AppServices 수정 없음
- ViewModel 수정 없음
- workflow/coordinator/storage/file service 수정 없음
- test code 수정 없음
- test file 생성 없음
- Policy/Claim storage 구현 없음
- OCR/SQLite/repository 구현 없음
- 실제 개인정보 샘플 사용 없음
- Git add/commit/reset/checkout/clean 없음

## S. Recommendation

1. 이 계획 문서를 기준으로 manual runtime check 실행 여부를 사용자에게 확인받는다.
2. 후속 작업으로 `docs/112_MAINWINDOW_DOCUMENT_REGISTRATION_MANUAL_RUNTIME_CHECK_RESULT.md`를 생성한다.
3. manual runtime check는 phase별로 수행한다.
4. dummy registration 후 생성 파일 목록을 기록한다.
5. cleanup 실행 여부를 결과 문서에 기록한다.
6. runtime check 결과에 따라 다음을 선택한다.
   - PASS: commit 후보 또는 hardening 문서
   - FAIL: failure review / minimal fix scope 문서

## T. Result

`MAINWINDOW_DOCUMENT_REGISTRATION_MANUAL_RUNTIME_CHECK_PLAN_DRAFTED`
