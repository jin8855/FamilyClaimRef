# Current Working Tree Commit Candidate Review

## A. Goal

이 문서는 current working tree commit candidate review 문서다.

목적은 다음과 같다.

- 현재 pending 변경을 commit 후보로 분류한다.
- source/doc changes와 runtime artifact를 분리한다.
- commit 전 build/test와 scope compliance를 확인한다.
- 이 문서는 commit 수행 문서가 아니다.
- Git add/commit/reset/checkout/clean은 수행하지 않는다.

## B. Source Documents

확인 기준 문서는 다음과 같다.

| 문서 | 상태 |
|---|---|
| `docs/116_RUNTIME_CLEANUP_CONTEXT_MISMATCH_REVIEW.md` | 확인 |
| `docs/115_RUNTIME_CLEANUP_FINAL_VERIFICATION.md` | 확인 |
| `docs/114_RUNTIME_CLEANUP_FAILURE_FOLLOWUP.md` | 확인 |
| `docs/113_MAINWINDOW_DOCUMENT_REGISTRATION_MANUAL_RUNTIME_CHECK_REVIEW.md` | 확인 |
| `docs/112_MAINWINDOW_DOCUMENT_REGISTRATION_MANUAL_RUNTIME_CHECK_RESULT.md` | 확인 |
| `docs/111_MAINWINDOW_DOCUMENT_REGISTRATION_MANUAL_RUNTIME_CHECK_PLAN.md` | 확인 |
| `docs/110_MAINWINDOW_DOCUMENT_REGISTRATION_UI_BINDING_IMPLEMENTATION_REVIEW.md` | 확인 |
| `docs/109_MAINWINDOW_DOCUMENT_REGISTRATION_UI_BINDING_USER_DECISION_RECORD.md` | 확인 |
| `docs/108_MAINWINDOW_DOCUMENT_REGISTRATION_UI_BINDING_DESIGN.md` | 확인 |
| `docs/107_SERVICE_COMPOSITION_ROOT_PATH_MANUAL_RUNTIME_CHECK_RESULT.md` | 확인 |
| `docs/106_SERVICE_COMPOSITION_ROOT_PATH_MANUAL_RUNTIME_CHECK_PLAN.md` | 확인 |
| `docs/105_SERVICE_COMPOSITION_ROOT_PATH_IMPLEMENTATION_REVIEW.md` | 확인 |
| `docs/104_SERVICE_COMPOSITION_ROOT_PATH_USER_DECISION_RECORD.md` | 확인 |
| `docs/103_SERVICE_COMPOSITION_ROOT_PATH_DESIGN.md` | 확인 |
| `docs/102_WPF_VIEWMODEL_FILE_PICKER_BOUNDARY_IMPLEMENTATION_REVIEW.md` | 확인 |

## C. User Decision / Cleanup Context

이번 commit candidate review는 아래 사용자 결정을 전제로 한다.

```text
User accepted local runtime artifact as outside commit scope.
```

기록:

- Codex context에서는 `%LOCALAPPDATA%\FamilyClaimRef`가 exists, files=3으로 보일 수 있다.
- 해당 artifact는 Git working tree 밖의 local runtime artifact다.
- commit 대상이 아니다.
- project root pollution은 없다.
- source code defect evidence는 없다.
- cleanup context mismatch는 environment/manual cleanup issue로 기록되었다.
- commit review에서는 local runtime artifact를 scope 밖으로 둔다.

## D. Git Status Summary

`git status --short` 조회 결과:

```text
 M app/FamilyClaimRef.App/App.xaml
 M app/FamilyClaimRef.App/App.xaml.cs
 M app/FamilyClaimRef.App/MainWindow.xaml
 M app/FamilyClaimRef.App/MainWindow.xaml.cs
?? app/FamilyClaimRef.App/Composition/
?? docs/105_SERVICE_COMPOSITION_ROOT_PATH_IMPLEMENTATION_REVIEW.md
?? docs/106_SERVICE_COMPOSITION_ROOT_PATH_MANUAL_RUNTIME_CHECK_PLAN.md
?? docs/107_SERVICE_COMPOSITION_ROOT_PATH_MANUAL_RUNTIME_CHECK_RESULT.md
?? docs/108_MAINWINDOW_DOCUMENT_REGISTRATION_UI_BINDING_DESIGN.md
?? docs/109_MAINWINDOW_DOCUMENT_REGISTRATION_UI_BINDING_USER_DECISION_RECORD.md
?? docs/110_MAINWINDOW_DOCUMENT_REGISTRATION_UI_BINDING_IMPLEMENTATION_REVIEW.md
?? docs/111_MAINWINDOW_DOCUMENT_REGISTRATION_MANUAL_RUNTIME_CHECK_PLAN.md
?? docs/112_MAINWINDOW_DOCUMENT_REGISTRATION_MANUAL_RUNTIME_CHECK_RESULT.md
?? docs/113_MAINWINDOW_DOCUMENT_REGISTRATION_MANUAL_RUNTIME_CHECK_REVIEW.md
?? docs/114_RUNTIME_CLEANUP_FAILURE_FOLLOWUP.md
?? docs/115_RUNTIME_CLEANUP_FINAL_VERIFICATION.md
?? docs/116_RUNTIME_CLEANUP_CONTEXT_MISMATCH_REVIEW.md
```

요약:

| 유형 | 수 | 내용 |
|---|---:|---|
| modified app files | 4 | App startup, MainWindow UI binding |
| untracked app folder | 1 | `app/FamilyClaimRef.App/Composition/` |
| untracked docs | 12 | docs `105` through `116` |
| deleted files | 0 | 없음 |
| unexpected files | 0 | 없음 |

`docs/117_CURRENT_WORKING_TREE_COMMIT_CANDIDATE_REVIEW.md` 자체는 이 문서 생성 후 다음 commit 후보에 포함할 수 있다.

## E. Diff / Whitespace Check

`git diff --check` 결과:

```text
PASS
```

비고:

- whitespace error는 보고되지 않았다.
- Git은 일부 working copy 파일에 대해 LF가 다음 Git touch 시 CRLF로 바뀔 수 있다는 warning을 표시했다.
- 해당 warning은 `diff --check` 실패가 아니다.

`git diff --stat` 결과:

```text
app/FamilyClaimRef.App/App.xaml           |   3 +-
app/FamilyClaimRef.App/App.xaml.cs        |  14 +++
app/FamilyClaimRef.App/MainWindow.xaml    | 152 +++++++++++++++++++++++++++++-
app/FamilyClaimRef.App/MainWindow.xaml.cs |  28 ++++--
4 files changed, 180 insertions(+), 17 deletions(-)
```

주의:

- `git diff --stat`는 tracked modified files만 표시한다.
- untracked `AppServices.cs`와 docs는 `git status --short` 기준으로 별도 분류했다.

## F. Commit Candidate File List

| Path | Status | Category | Reason | Candidate Decision |
|---|---|---|---|---|
| `app/FamilyClaimRef.App/Composition/AppServices.cs` | untracked | App composition | manual composition root 추가 | Include |
| `app/FamilyClaimRef.App/App.xaml` | modified | WPF startup | `StartupUri` 제거, startup code-behind 연결 준비 | Include |
| `app/FamilyClaimRef.App/App.xaml.cs` | modified | WPF startup | `AppServices` 생성 및 `MainWindow.DataContext` 연결 | Include |
| `app/FamilyClaimRef.App/MainWindow.xaml` | modified | MainWindow UI binding | document registration 최소 UI controls 추가 | Include |
| `app/FamilyClaimRef.App/MainWindow.xaml.cs` | modified | MainWindow UI binding | ViewModel click handler 연결 | Include |
| `docs/105_SERVICE_COMPOSITION_ROOT_PATH_IMPLEMENTATION_REVIEW.md` | untracked | Architecture / review docs | AppServices/root path 구현 결과 리뷰 | Include |
| `docs/106_SERVICE_COMPOSITION_ROOT_PATH_MANUAL_RUNTIME_CHECK_PLAN.md` | untracked | Runtime check docs | startup-only manual runtime check plan | Include |
| `docs/107_SERVICE_COMPOSITION_ROOT_PATH_MANUAL_RUNTIME_CHECK_RESULT.md` | untracked | Runtime check docs | startup-only manual runtime check result | Include |
| `docs/108_MAINWINDOW_DOCUMENT_REGISTRATION_UI_BINDING_DESIGN.md` | untracked | Architecture / review docs | MainWindow UI binding design | Include |
| `docs/109_MAINWINDOW_DOCUMENT_REGISTRATION_UI_BINDING_USER_DECISION_RECORD.md` | untracked | Architecture / review docs | UI binding user decision record | Include |
| `docs/110_MAINWINDOW_DOCUMENT_REGISTRATION_UI_BINDING_IMPLEMENTATION_REVIEW.md` | untracked | Architecture / review docs | UI binding implementation review | Include |
| `docs/111_MAINWINDOW_DOCUMENT_REGISTRATION_MANUAL_RUNTIME_CHECK_PLAN.md` | untracked | Runtime check docs | document registration manual runtime check plan | Include |
| `docs/112_MAINWINDOW_DOCUMENT_REGISTRATION_MANUAL_RUNTIME_CHECK_RESULT.md` | untracked | Runtime check docs | manual runtime check result | Include |
| `docs/113_MAINWINDOW_DOCUMENT_REGISTRATION_MANUAL_RUNTIME_CHECK_REVIEW.md` | untracked | Cleanup follow-up docs | cleanup failure review | Include |
| `docs/114_RUNTIME_CLEANUP_FAILURE_FOLLOWUP.md` | untracked | Cleanup follow-up docs | cleanup failure follow-up | Include |
| `docs/115_RUNTIME_CLEANUP_FINAL_VERIFICATION.md` | untracked | Cleanup follow-up docs | cleanup final verification with context mismatch | Include |
| `docs/116_RUNTIME_CLEANUP_CONTEXT_MISMATCH_REVIEW.md` | untracked | Cleanup follow-up docs | context mismatch review | Include |
| `docs/117_CURRENT_WORKING_TREE_COMMIT_CANDIDATE_REVIEW.md` | new in this step | Commit review docs | current working tree commit candidate review | Include |

## G. Explicit Exclusions

Commit 대상에서 명시적으로 제외한다.

```text
%LOCALAPPDATA%\FamilyClaimRef
C:\Users\jin8855\AppData\Local\FamilyClaimRef
C:\EtcProject\FamilyClaimRef\attachments contents
C:\EtcProject\FamilyClaimRef\data\local contents
bin/
obj/
*.db
*.sqlite
*.sqlite3
```

확인:

| 항목 | 상태 |
|---|---|
| `%LOCALAPPDATA%\FamilyClaimRef` | Codex context에서 exists, files=3, outside commit scope |
| project root `attachments/` | files=0 |
| project root `data/local` | files=0 |
| DB/SQLite unexpected files | 0 |

## H. Build / Test Verification

일반 sandbox 실행:

| Command | Result | Notes |
|---|---|---|
| `dotnet build FamilyClaimRef.sln` | FAIL | `C:\Users\jin8855\AppData\Local\Microsoft SDKs` access denied |
| `dotnet test FamilyClaimRef.sln` | FAIL | 동일 Windows SDK path access denied |

권한 상승 재실행:

| Command | Result | Warnings | Errors | Notes |
|---|---|---:|---:|---|
| `dotnet build FamilyClaimRef.sln` | PASS | 0 | 0 | elevated run |
| `dotnet test FamilyClaimRef.sln` | PASS | 0 | 0 | elevated run |

Test summary:

```text
total tests: 216
passed tests: 216
failed tests: 0
skipped tests: 0
```

초기 실패/재실행 여부:

```text
Initial sandbox build/test failed because Windows SDK path access was denied.
Elevated build/test passed.
```

## I. Runtime Evidence Summary

Runtime evidence 요약:

- startup-only check PASSED.
- MainWindow UI binding implementation reviewed.
- manual runtime functional flow PASS_WITH_NOTES.
- cleanup context mismatch recorded.
- local artifact accepted outside commit scope.
- project root pollution 없음.
- actual personal sample 사용 없음.
- metadata absolute source path 저장 없음.
- manual check 중 attachment는 의도된 user app data root 아래 생성되었다.

## J. Scope Compliance Review

범위 준수:

- AppServices manual composition root implemented.
- App startup/DataContext connected.
- MainWindow minimum UI binding implemented.
- ViewModel/file picker/workflow/storage existing boundary preserved.
- Policy/Claim storage not implemented.
- OCR/SQLite/repository not implemented.
- Command pattern not implemented.
- Test project not modified in this step.
- No actual personal sample.
- No project root pollution.
- Git add/commit/reset/checkout/clean not performed.

## K. Commit Readiness

판정:

```text
COMMIT_CANDIDATE_READY_WITH_ACCEPTED_LOCAL_ARTIFACT_NOTE
```

근거:

- git status contains expected source/doc changes.
- diff --check PASS.
- build/test PASS after elevated rerun.
- project root pollution 없음.
- local runtime artifact accepted outside commit scope.
- DB/SQLite unexpected files 없음.
- unexpected working tree files 없음.

주의:

- local runtime artifact는 Codex context에서 여전히 files=3으로 보일 수 있다.
- 이 artifact는 Git working tree 밖이며 commit 대상이 아니다.
- commit 문서 또는 commit 전 확인에는 accepted local artifact note를 남기는 것이 안전하다.

## L. Suggested Commit Scope

Commit message 후보:

```text
feat(familyclaimref): add document registration app composition and UI binding
```

포함 요약:

- AppServices composition root.
- App startup MainWindow DataContext 연결.
- MainWindow document registration minimum UI.
- manual runtime check docs.
- cleanup/context mismatch docs.

주의:

- 이 문서에서는 commit하지 않는다.
- commit message는 후보 문구다.
- 실제 commit은 별도 승인 후 수행한다.

## M. Remaining Risks

남은 위험:

- Policy/Claim storage 없음.
- target id manual dummy input 상태.
- command pattern 없음.
- final UI styling 없음.
- duplicate registration detailed matrix 미검증.
- production installer/environment 미검증.
- DatePicker calendar popup 내부 동작은 별도 hardening 후보.
- `.txt` dummy extension guidance issue open.
- local runtime artifact accepted outside commit scope, but Codex context mismatch remains documented.

## N. Recommendation

Ready 상태이므로 다음 작업 후보:

```text
Next: explicit git add/commit instruction with exact file list.
```

주의:

- 아직 commit하지 않는다.
- 다음 사용자가 명시적으로 commit 지시를 내려야 add/commit을 수행한다.
- commit 수행 시 exact file list로 stage해야 한다.

## O. Result

```text
CURRENT_WORKING_TREE_COMMIT_CANDIDATE_READY_WITH_ACCEPTED_LOCAL_ARTIFACT_NOTE
```
