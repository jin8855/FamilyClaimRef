# MainWindow Document Registration Manual Runtime Check Review

## A. Goal

이 문서는 MainWindow document registration manual runtime check review 문서다.

목적:

- `docs/112_MAINWINDOW_DOCUMENT_REGISTRATION_MANUAL_RUNTIME_CHECK_RESULT.md` 결과를 검토한다.
- app launch, UI display, DatePicker binding, OpenFileDialog, dummy file select, validation, dummy registration, file system side effect를 정리한다.
- cleanup failure를 별도 이슈로 검토한다.
- cleanup 재시도 결과를 기록한다.

이 문서는 코드 수정 문서가 아니다.

## B. Checked Files / Documents

| 대상 | 상태 |
|---|---|
| `docs/112_MAINWINDOW_DOCUMENT_REGISTRATION_MANUAL_RUNTIME_CHECK_RESULT.md` | 확인 |
| `docs/111_MAINWINDOW_DOCUMENT_REGISTRATION_MANUAL_RUNTIME_CHECK_PLAN.md` | 확인 |
| `docs/110_MAINWINDOW_DOCUMENT_REGISTRATION_UI_BINDING_IMPLEMENTATION_REVIEW.md` | 확인 |
| `app/FamilyClaimRef.App/MainWindow.xaml` | 확인 |
| `app/FamilyClaimRef.App/MainWindow.xaml.cs` | 확인 |
| `app/FamilyClaimRef.App/Composition/AppServices.cs` | 확인 |
| `app/FamilyClaimRef.App/ViewModels/DocumentRegistrationViewModel.cs` | 확인 |

## C. Manual Runtime Check Summary

`docs/112_MAINWINDOW_DOCUMENT_REGISTRATION_MANUAL_RUNTIME_CHECK_RESULT.md` 기준 요약:

- build PASS.
- test PASS.
- 총 테스트 216개 PASS.
- app launch PASS.
- MainWindow 표시 PASS.
- UI controls 표시 PASS.
- DatePicker binding PASS_WITH_NOTES.
- OpenFileDialog cancel PASS.
- dummy file select PASS.
- validation PASS.
- dummy registration PASS_WITH_NOTES.
- metadata files는 `%LOCALAPPDATA%\FamilyClaimRef\data\local`에 생성됐다.
- attachment files는 `%LOCALAPPDATA%\FamilyClaimRef\attachments\documents`에 생성됐다.
- project root pollution 없음.
- actual personal sample 사용 없음.
- cleanup FAIL로 최종 Result는 `MAINWINDOW_DOCUMENT_REGISTRATION_MANUAL_RUNTIME_CHECK_FAILED`였다.

## D. Functional Result Review

| 항목 | 판정 | 비고 |
|---|---|---|
| App launch / UI display | PASS | WPF exe launch 후 MainWindow 표시 |
| DatePicker / ReferenceDate binding | PASS_WITH_NOTES | `DateOnly` / DatePicker 조합에서 registration flow crash는 없었으나 calendar popup 세부 검증은 남음 |
| OpenFileDialog cancel | PASS | button click 후 dialog 표시, cancel 후 side effect 없음 |
| Dummy file select | PASS | display name만 표시, full path UI 노출 없음 |
| Validation without required inputs | PASS | `저장할 대상을 입력해 주세요.` 표시 |
| Dummy registration | PASS_WITH_NOTES | `.png` dummy file 등록 성공, `.txt` dummy file은 allowlist 밖이라 실패 |
| Metadata relative path | PASS | `documents/policy-document_20260702_policy_001.png` |
| Absolute source path not stored | PASS | temp source absolute path 저장 없음 |
| Project root pollution | PASS | `attachments/`, `data/local` files=0 |

Functional conclusion:

```text
Manual runtime functional flow is PASS_WITH_NOTES.
Final review result is blocked by cleanup failure only.
```

## E. Cleanup Failure Review

Cleanup failure target:

```text
%LOCALAPPDATA%\FamilyClaimRef
```

Remaining files from `docs/112` and current retry:

```text
C:\Users\jin8855\AppData\Local\FamilyClaimRef\attachments\documents\policy-document_20260702_policy_001.png
C:\Users\jin8855\AppData\Local\FamilyClaimRef\data\local\documents.json
C:\Users\jin8855\AppData\Local\FamilyClaimRef\data\local\policy-documents.json
```

Cleanup failure evidence:

- normal shell context: `jin\codexsandboxoffline`
- normal shell sees `%LOCALAPPDATA%\FamilyClaimRef`.
- normal shell deletion fails with access denied.
- elevated shell context: `epience\jin8855`
- elevated shell does not see the same absolute path.

Likely classification:

```text
Manual cleanup/environment issue.
Current evidence does not indicate a source code defect.
```

Severity:

```text
Medium
```

Cleanup retry result:

```text
FAIL
```

## F. Cleanup Retry Evidence

| Target | Before | Action | After | Result | Notes |
|---|---|---|---|---|---|
| `%LOCALAPPDATA%\FamilyClaimRef` | EXISTS, files=3 | `Remove-Item -LiteralPath ... -Recurse -Force` in normal shell | EXISTS, files=3 | FAIL | access denied |
| `%LOCALAPPDATA%\FamilyClaimRef` | elevated shell reports ABSENT | elevated visibility check | ABSENT in elevated context | SKIPPED | elevated context does not see same path |
| `%TEMP%\FamilyClaimRefManualCheck` | ABSENT | no delete needed | ABSENT | PASS | temp dummy directory already removed |
| project root `attachments/` | EXISTS, files=0 | count only | EXISTS, files=0 | PASS | no deletion performed |
| project root `data/local` | EXISTS, files=0 | count only | EXISTS, files=0 | PASS | no deletion performed |

Cleanup command:

```powershell
$target = 'C:\Users\jin8855\AppData\Local\FamilyClaimRef'
Remove-Item -LiteralPath $target -Recurse -Force
```

Error message:

```text
경로에 대한 액세스가 거부되었습니다.
```

Remaining files:

```text
C:\Users\jin8855\AppData\Local\FamilyClaimRef\attachments\documents\policy-document_20260702_policy_001.png
C:\Users\jin8855\AppData\Local\FamilyClaimRef\data\local\documents.json
C:\Users\jin8855\AppData\Local\FamilyClaimRef\data\local\policy-documents.json
```

## G. Issue Review

### ISSUE-001. Cleanup failed for `%LOCALAPPDATA%\FamilyClaimRef`

Observed behavior:

- Runtime dummy files remain under `%LOCALAPPDATA%\FamilyClaimRef`.
- Normal shell cleanup fails with access denied.
- Elevated shell does not see the same path.

Expected behavior:

- Runtime dummy files should be removed after manual runtime check cleanup.

Severity:

```text
Medium
```

Current status:

```text
Still Open
```

Next action:

- User-side manual deletion or separate cleanup failure follow-up is needed.
- Do not proceed to commit candidate review until cleanup status is explicitly accepted or resolved.

### ISSUE-002. Plan allowed text dummy file, but current file policy rejects `.txt`

Observed behavior:

- `dummy-receipt.txt` registration displayed `문서 등록에 실패했습니다.`

Expected behavior:

- Dummy registration should use a currently allowed extension.

Severity:

```text
Low
```

Current status:

```text
Open
```

Next action:

- Future manual check guidance should prefer dummy extensions from the current allowlist:
  - `pdf`
  - `jpg`
  - `jpeg`
  - `png`

## H. File System Final State

| Path | Final State |
|---|---|
| `%LOCALAPPDATA%\FamilyClaimRef` | EXISTS, files=3 |
| `%TEMP%\FamilyClaimRefManualCheck` | ABSENT, files=0 |
| `C:\EtcProject\FamilyClaimRef\attachments` | EXISTS, files=0 |
| `C:\EtcProject\FamilyClaimRef\data\local` | EXISTS, files=0 |
| DB/SQLite files outside bin/obj | none found |

Unexpected files:

```text
C:\Users\jin8855\AppData\Local\FamilyClaimRef\attachments\documents\policy-document_20260702_policy_001.png
C:\Users\jin8855\AppData\Local\FamilyClaimRef\data\local\documents.json
C:\Users\jin8855\AppData\Local\FamilyClaimRef\data\local\policy-documents.json
```

Project root pollution:

```text
None
```

Actual personal sample:

```text
None
```

## I. Scope Compliance Review

| 항목 | 판정 |
|---|---|
| XAML 수정 없음 | PASS |
| C# 수정 없음 | PASS |
| production C# 수정 없음 | PASS |
| `MainWindow.xaml` 수정 없음 | PASS |
| `MainWindow.xaml.cs` 수정 없음 | PASS |
| `AppServices.cs` 수정 없음 | PASS |
| `App.xaml` 수정 없음 | PASS |
| `App.xaml.cs` 수정 없음 | PASS |
| ViewModel 수정 없음 | PASS |
| file picker 수정 없음 | PASS |
| workflow/coordinator/storage/file service 수정 없음 | PASS |
| test code 수정 없음 | PASS |
| test file 생성 없음 | PASS |
| Policy/Claim storage 구현 없음 | PASS |
| OCR/SQLite/repository 구현 없음 | PASS |
| 실제 개인정보 샘플 사용 없음 | PASS |
| `.sln`, `.csproj` 수정 없음 | PASS |
| NuGet package 추가 없음 | PASS |
| Git add/commit/reset/checkout/clean 없음 | PASS |
| project root 삭제 없음 | PASS |
| project root `attachments/`, `data/local` 삭제 없음 | PASS |
| source tree cleanup 없음 | PASS |

## J. Risks / Remaining Work

- `%LOCALAPPDATA%\FamilyClaimRef` cleanup failure remains open.
- Policy/Claim storage 없음.
- target id manual dummy input 상태.
- command pattern 없음.
- final UI styling 없음.
- duplicate registration detailed matrix 미검증.
- production installer/environment 미검증.
- DatePicker calendar popup 세부 동작은 별도 hardening 후보.
- `.txt` dummy extension은 allowlist 밖이므로 manual check guidance 보완 필요.
- cleanup 실패 상태로 commit 후보 정리를 진행하면 runtime artifact 잔여 이슈가 남는다.

## K. Recommendation

Cleanup retry failed.

Recommended next step:

```text
docs/114_RUNTIME_CLEANUP_FAILURE_FOLLOWUP.md
```

Alternative:

```text
User manually deletes C:\Users\jin8855\AppData\Local\FamilyClaimRef, then rerun final file system check.
```

Do not treat this state as commit-ready until one of the following is true:

- cleanup succeeds
- user explicitly accepts remaining local runtime artifact
- cleanup failure is documented as environment-only and excluded from commit readiness

## L. Result

`MAINWINDOW_DOCUMENT_REGISTRATION_MANUAL_RUNTIME_CHECK_REVIEWED_WITH_CLEANUP_FAILURE`
