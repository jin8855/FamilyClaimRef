# MainWindow Document Registration Manual Runtime Check Result

## A. Goal

이 문서는 MainWindow document registration manual runtime check result 문서다.

기록 대상:

- app launch 결과
- MainWindow UI controls 표시 결과
- DatePicker / ReferenceDate binding runtime 결과
- OpenFileDialog cancel 결과
- dummy file select 결과
- validation message 결과
- dummy registration 결과
- production root side effect
- cleanup 결과

## B. Baseline / Checked Documents

| 대상 | 상태 |
|---|---|
| `docs/111_MAINWINDOW_DOCUMENT_REGISTRATION_MANUAL_RUNTIME_CHECK_PLAN.md` | 확인 |
| `docs/110_MAINWINDOW_DOCUMENT_REGISTRATION_UI_BINDING_IMPLEMENTATION_REVIEW.md` | 확인 |
| `docs/109_MAINWINDOW_DOCUMENT_REGISTRATION_UI_BINDING_USER_DECISION_RECORD.md` | 확인 |
| `docs/107_SERVICE_COMPOSITION_ROOT_PATH_MANUAL_RUNTIME_CHECK_RESULT.md` | 확인 |
| `docs/105_SERVICE_COMPOSITION_ROOT_PATH_IMPLEMENTATION_REVIEW.md` | 확인 |
| `app/FamilyClaimRef.App/MainWindow.xaml` | 확인 |
| `app/FamilyClaimRef.App/MainWindow.xaml.cs` | 확인 |
| `app/FamilyClaimRef.App/Composition/AppServices.cs` | 확인 |
| `app/FamilyClaimRef.App/ViewModels/DocumentRegistrationViewModel.cs` | 확인 |

## C. Pre-check Result

### Build / Test

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

Execution notes:

- 권한 상승 실행 여부: 있음
- 초기 실패/재실행 여부: 없음
- build/test 모두 권한 상승 실행에서 PASS

### Production Root Before

| Path | Before |
|---|---|
| `%LOCALAPPDATA%\FamilyClaimRef` | ABSENT |
| `%LOCALAPPDATA%\FamilyClaimRef\data\local` | ABSENT |
| `%LOCALAPPDATA%\FamilyClaimRef\attachments` | ABSENT |
| `%LOCALAPPDATA%\FamilyClaimRef\attachments\documents` | ABSENT |
| `%LOCALAPPDATA%\FamilyClaimRef\data\local\documents.json` | ABSENT |
| `%LOCALAPPDATA%\FamilyClaimRef\data\local\policy-documents.json` | ABSENT |
| `%LOCALAPPDATA%\FamilyClaimRef\data\local\claim-documents.json` | ABSENT |

### Project Root Before

| Path | Before |
|---|---|
| `C:\EtcProject\FamilyClaimRef\attachments` | EXISTS, files=0 |
| `C:\EtcProject\FamilyClaimRef\data\local` | EXISTS, files=0 |

### DB / SQLite Before

```text
No *.db / *.sqlite / *.sqlite3 files found outside bin/obj.
```

### Dummy File Creation

Created dummy files:

```text
C:\Users\jin8855\AppData\Local\Temp\FamilyClaimRefManualCheck\dummy-receipt.txt
C:\Users\jin8855\AppData\Local\Temp\FamilyClaimRefManualCheck\dummy-receipt.png
```

Dummy content summary:

```text
dummy manual check file - no personal data
dummy png extension manual check file - no personal data
```

Personal data:

```text
None
```

Git status query:

```text
git status 조회만 수행했다.
git add/commit/reset/checkout/clean 수행 없음.
```

## D. Runtime Command

App launch method:

```text
C:\EtcProject\FamilyClaimRef\app\FamilyClaimRef.App\bin\Debug\net10.0-windows\FamilyClaimRef.App.exe
```

Execution method:

```text
Built WPF exe launched through Windows app automation.
```

Permission elevation:

```text
build/test and cleanup attempts used elevated shell where required.
WPF UI interaction used Windows app automation.
```

App exit method:

```text
Alt+F4
```

App launch available:

```text
Yes
```

## E. Phase Result Summary

| Phase | Action | Result | Evidence | File System Side Effect | Notes |
|---|---|---|---|---|---|
| 1. App launch / UI display | WPF exe launch | PASS | MainWindow displayed | startup-only files none | No automatic dialog/workflow |
| 2. DatePicker / ReferenceDate binding | Date field entry | PASS_WITH_NOTES | `2026-07-02` entered without crash | none before registration | DateOnly/DatePicker did not crash in this flow |
| 3. OpenFileDialog cancel | Select file then cancel | PASS | Dialog opened only after click, cancel returned to app | none | No metadata/attachment created |
| 4. Dummy file select | Select dummy file | PASS | UI displayed `dummy-receipt.txt`, later `dummy-receipt.png` | none before registration | UI showed display filename only |
| 5. Validation without required inputs | Register with missing TargetId | PASS | `저장할 대상을 입력해 주세요.` | none | Workflow side effect not observed |
| 6. Dummy registration | Policy registration with dummy png | PASS_WITH_NOTES | Success message and summary displayed | metadata/attachment created under `%LOCALAPPDATA%` | `.txt` attempt failed as unsupported extension, `.png` succeeded |
| 7. Cleanup | Delete runtime files | FAIL | normal shell saw files but deletion access denied; elevated shell did not see same path | production root files remain | Manual cleanup needed |

## F. UI Runtime Result

| UI 항목 | 결과 |
|---|---|
| MainWindow 표시 | PASS |
| scope notice 표시 | PASS |
| file select button 표시 | PASS |
| register button 표시 | PASS |
| target kind selector 표시 | PASS |
| target id TextBox 표시 | PASS |
| documentType ComboBox 표시 | PASS |
| displayTitle TextBox 표시 | PASS |
| referenceDate DatePicker 표시 | PASS |
| validation/status/result area 표시 | PASS |
| automatic OpenFileDialog | 없음 |
| automatic registration | 없음 |

## G. DatePicker / ReferenceDate Binding Result

| 항목 | 결과 |
|---|---|
| initial display | `날짜 선택` placeholder 표시 |
| date selection/input result | `2026-07-02` 입력 가능 |
| crash 여부 | 없음 |
| binding exception 여부 | 관찰되지 않음 |
| DateOnly/DatePicker issue 여부 | registration flow에서 crash 없음 |
| 판정 | PASS_WITH_NOTES |

Notes:

- ViewModel `ReferenceDate`는 `DateOnly`이다.
- WPF `DatePicker.SelectedDate`는 일반적으로 `DateTime?` surface다.
- 이번 manual check에서는 입력과 registration까지 crash는 없었다.
- DatePicker calendar popup 세부 동작은 별도 hardening 대상이다.

## H. OpenFileDialog Result

| 항목 | 결과 |
|---|---|
| button click 전 자동 실행 | 없음 |
| file select button click 후 dialog 표시 | PASS |
| cancel 가능 | PASS |
| cancel 후 crash | 없음 |
| cancel 후 metadata side effect | 없음 |
| cancel 후 attachment side effect | 없음 |
| project root pollution | 없음 |

## I. Dummy File Select Result

| 항목 | 결과 |
|---|---|
| dummy txt path | `C:\Users\jin8855\AppData\Local\Temp\FamilyClaimRefManualCheck\dummy-receipt.txt` |
| dummy png path | `C:\Users\jin8855\AppData\Local\Temp\FamilyClaimRefManualCheck\dummy-receipt.png` |
| selected display name | `dummy-receipt.txt`, `dummy-receipt.png` |
| full path UI 노출 | 관찰되지 않음 |
| metadata/attachment 생성 | file select 단계에서는 없음 |
| project root pollution | 없음 |

## J. Validation Result

| Case | Displayed Message | Metadata/Attachment Side Effect | Result |
|---|---|---|---|
| selected file 있음, TargetId 없음 | `저장할 대상을 입력해 주세요.` | 없음 | PASS |

Notes:

- workflow 호출 여부는 instrumentation 없이 UI message와 file system side effect로 관찰했다.
- validation 단계에서 production root와 project root file 생성은 없었다.

## K. Dummy Registration Result

Executed case:

```text
policy
```

Input values:

```text
TargetKind: policy
TargetId: POLICY-DEMO-001
DocumentType: policy
DisplayTitle: Dummy Policy Document
ReferenceDate: 2026-07-02
SourceFile: dummy-receipt.png
```

UI result:

```text
문서 등록이 완료되었습니다.
policy:POLICY-DEMO-001; document:doc_ea8a2b89b3184dc3909c2cdd9fef99f2
```

Created metadata files:

```text
C:\Users\jin8855\AppData\Local\FamilyClaimRef\data\local\documents.json
C:\Users\jin8855\AppData\Local\FamilyClaimRef\data\local\policy-documents.json
```

Created attachment files:

```text
C:\Users\jin8855\AppData\Local\FamilyClaimRef\attachments\documents\policy-document_20260702_policy_001.png
```

Metadata checks:

| 항목 | 결과 |
|---|---|
| `documents.json` 생성 | PASS |
| `policy-documents.json` 생성 | PASS |
| `claim-documents.json` 생성 | not applicable |
| absolute source path 저장 | 없음 |
| `DocumentRecord.RelativePath` shape | `documents/policy-document_20260702_policy_001.png` |
| attachment root 기준 relative path | PASS |
| actual personal data | 없음 |

Additional note:

- `dummy-receipt.txt` registration attempt displayed `문서 등록에 실패했습니다.`
- The likely cause is unsupported `.txt` extension under current file policy.
- No file system side effect was observed after the `.txt` failure.
- Retry with dummy `.png` succeeded.

## L. File System Result

### Before

| Path | State |
|---|---|
| `%LOCALAPPDATA%\FamilyClaimRef` | ABSENT |
| `%LOCALAPPDATA%\FamilyClaimRef\data\local` | ABSENT |
| `%LOCALAPPDATA%\FamilyClaimRef\attachments` | ABSENT |
| `%LOCALAPPDATA%\FamilyClaimRef\attachments\documents` | ABSENT |
| project root `attachments/` | files=0 |
| project root `data/local` | files=0 |

### After Registration

| Path | State |
|---|---|
| `%LOCALAPPDATA%\FamilyClaimRef` | EXISTS, files=3 |
| `%LOCALAPPDATA%\FamilyClaimRef\data\local` | EXISTS, files=2 |
| `%LOCALAPPDATA%\FamilyClaimRef\attachments` | EXISTS, files=1 |
| `%LOCALAPPDATA%\FamilyClaimRef\attachments\documents` | EXISTS, files=1 |
| `documents.json` | CREATED |
| `policy-documents.json` | CREATED |
| `claim-documents.json` | MISSING |
| project root `attachments/` | files=0 |
| project root `data/local` | files=0 |
| DB/SQLite files | none found |

### After Cleanup Attempt

| Path | State |
|---|---|
| `%LOCALAPPDATA%\FamilyClaimRef` | STILL EXISTS, files=3 |
| `%TEMP%\FamilyClaimRefManualCheck` | ABSENT |
| project root `attachments/` | files=0 |
| project root `data/local` | files=0 |

## M. Cleanup Result

Cleanup attempted:

```powershell
Remove-Item -Recurse -Force "$env:LOCALAPPDATA\FamilyClaimRef"
Remove-Item -Recurse -Force "$env:TEMP\FamilyClaimRefManualCheck"
```

Cleanup result:

| Target | Result |
|---|---|
| `%TEMP%\FamilyClaimRefManualCheck` | PASS, removed |
| `%LOCALAPPDATA%\FamilyClaimRef` | FAIL, files remain |

Observed behavior:

- Elevated shell cleanup reported the target as absent or removed.
- Normal shell still sees `%LOCALAPPDATA%\FamilyClaimRef` with 3 files.
- Normal shell deletion failed with access denied.

Remaining files:

```text
C:\Users\jin8855\AppData\Local\FamilyClaimRef\attachments\documents\policy-document_20260702_policy_001.png
C:\Users\jin8855\AppData\Local\FamilyClaimRef\data\local\documents.json
C:\Users\jin8855\AppData\Local\FamilyClaimRef\data\local\policy-documents.json
```

Cleanup note:

```text
Manual cleanup is still required for C:\Users\jin8855\AppData\Local\FamilyClaimRef.
```

## N. Stop Condition Review

| Stop Condition | 발생 여부 |
|---|---|
| app startup crash | No |
| DatePicker binding crash | No |
| unhandled exception | No |
| automatic OpenFileDialog on startup | No |
| automatic registration workflow on startup | No |
| OpenFileDialog opens without button click | No |
| registration happens without button click | No |
| metadata/attachment files created before registration | No |
| project root pollution | No |
| actual personal/private document selected | No |
| actual personal sample data entered | No |
| cleanup target path ambiguous | No |
| cleanup failure | Yes |

## O. Issues Found

### ISSUE-001. Cleanup failed for `%LOCALAPPDATA%\FamilyClaimRef`

- Observed behavior: runtime files remained after cleanup attempt.
- Expected behavior: runtime files removed after cleanup.
- Affected scope: local manual runtime artifact cleanup.
- Severity: Medium.
- Next recommendation: perform manual deletion of `C:\Users\jin8855\AppData\Local\FamilyClaimRef` from an environment with the same filesystem view and sufficient permission.

### ISSUE-002. Plan allowed text dummy file, but current file policy rejects `.txt`

- Observed behavior: `dummy-receipt.txt` registration displayed `문서 등록에 실패했습니다.`
- Expected behavior: dummy file should use current allowlist extension.
- Affected scope: manual check plan dummy file guidance.
- Severity: Low.
- Next recommendation: update future manual check guidance to prefer `pdf`, `jpg`, `jpeg`, or `png` dummy files.

## P. Risks / Remaining Work

- Policy/Claim storage 없음.
- target id manual dummy input 상태.
- command pattern 없음.
- final UI styling 없음.
- duplicate registration detailed matrix 미검증.
- production installer/environment 미검증.
- cleanup 실패로 `%LOCALAPPDATA%\FamilyClaimRef`에 dummy runtime files가 남아 있다.
- DatePicker calendar popup 세부 동작은 별도 확인이 필요하다.

## Q. Recommendation

Result가 cleanup 실패를 포함하므로 다음 작업을 권장한다.

1. `C:\Users\jin8855\AppData\Local\FamilyClaimRef` 수동 정리 또는 cleanup failure review 작성.
2. `docs/113_MAINWINDOW_DOCUMENT_REGISTRATION_MANUAL_RUNTIME_CHECK_REVIEW.md` 생성.
3. 다음 manual check plan에서는 dummy file extension을 `pdf`, `jpg`, `jpeg`, `png`로 제한.
4. cleanup이 완료된 후 commit 후보 정리 여부 결정.

## R. Result

`MAINWINDOW_DOCUMENT_REGISTRATION_MANUAL_RUNTIME_CHECK_FAILED`
