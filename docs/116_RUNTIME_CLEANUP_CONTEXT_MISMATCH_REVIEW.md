# Runtime Cleanup Context Mismatch Review

## A. Goal

이 문서는 runtime cleanup context mismatch review 문서다.

목적은 다음과 같다.

- 사용자 shell과 Codex shell의 `%LOCALAPPDATA%\FamilyClaimRef` 조회 결과 불일치를 정리한다.
- cleanup issue를 source defect와 분리한다.
- remaining artifact가 source tree 밖의 local runtime artifact인지 확인한다.
- commit candidate review로 넘어갈 수 있는 조건을 다시 정리한다.
- 이 문서는 코드 수정 문서가 아니다.

## B. Source Documents

확인 기준 문서는 다음과 같다.

| 문서 | 확인 내용 |
|---|---|
| `docs/115_RUNTIME_CLEANUP_FINAL_VERIFICATION.md` | final verification result, unresolved cleanup status |
| `docs/114_RUNTIME_CLEANUP_FAILURE_FOLLOWUP.md` | cleanup failure follow-up, user action required |
| `docs/113_MAINWINDOW_DOCUMENT_REGISTRATION_MANUAL_RUNTIME_CHECK_REVIEW.md` | cleanup failure review, issue status |
| `docs/112_MAINWINDOW_DOCUMENT_REGISTRATION_MANUAL_RUNTIME_CHECK_RESULT.md` | runtime check result, generated artifact list |
| `docs/111_MAINWINDOW_DOCUMENT_REGISTRATION_MANUAL_RUNTIME_CHECK_PLAN.md` | cleanup target and runtime verification plan |

## C. Mismatch Summary

필수 기록:

- 사용자 shell에서는 `Test-Path "$env:LOCALAPPDATA\FamilyClaimRef"` 결과가 `False`로 보고되었다.
- Codex verification context에서는 `%LOCALAPPDATA%\FamilyClaimRef`가 `exists, files=3`으로 확인되었다.
- `docs/115_RUNTIME_CLEANUP_FINAL_VERIFICATION.md`는 `RUNTIME_CLEANUP_FINAL_VERIFICATION_NOT_RESOLVED`로 기록되었다.
- cleanup resolved로 판단하지 않았다.
- issue는 `Still Open in Codex verification context`다.

해석:

```text
Cleanup status is not globally resolved.
The issue is now a runtime cleanup context mismatch.
```

## D. Context Comparison

| Context | User/Identity | LOCALAPPDATA | Target Exists | Files Count | Notes |
|---|---|---|---:|---:|---|
| User shell | user-reported | `$env:LOCALAPPDATA` 기준 | False | 0 | 사용자 보고 |
| Codex shell | `jin\codexsandboxoffline` | `C:\Users\jin8855\AppData\Local` | True | 3 | Codex 확인 |
| Elevated shell if known | 이전 문서 기준 | same absolute path absent | absent | 0 | `113`, `114`, `115` 흐름의 이전 확인값 |

확인 가능한 값만 confirmed로 기록한다.

Candidate cause:

- Windows 계정 context 차이.
- 권한 차이.
- sandbox 또는 가상화된 file system view 차이.
- elevated shell과 Codex shell이 서로 다른 user profile view를 보고 있을 가능성.

위 원인은 현재 문서 기준 추정이므로 source defect로 확정하지 않는다.

## E. Remaining Artifact Review

Codex context에서 보이는 remaining artifacts:

```text
C:\Users\jin8855\AppData\Local\FamilyClaimRef\attachments\documents\policy-document_20260702_policy_001.png
C:\Users\jin8855\AppData\Local\FamilyClaimRef\data\local\documents.json
C:\Users\jin8855\AppData\Local\FamilyClaimRef\data\local\policy-documents.json
```

Files count:

```text
3
```

개인정보 여부:

```text
none / dummy-only
```

Source tree 포함 여부:

```text
no, outside Git working tree
```

Project root pollution:

```text
none
```

Project root check:

| Target | Result |
|---|---|
| `C:\EtcProject\FamilyClaimRef\attachments` | exists, files=0 |
| `C:\EtcProject\FamilyClaimRef\data\local` | exists, files=0 |
| DB/SQLite unexpected files | none |

정리:

```text
Remaining artifacts are local runtime files outside the source tree.
They do not appear to be source changes or commit contents.
```

## F. Source Code Impact Review

Source code defect 여부:

- 기능 runtime flow는 `PASS_WITH_NOTES`다.
- metadata/attachment 생성 위치는 `%LOCALAPPDATA%\FamilyClaimRef`로 의도된 production root다.
- project root pollution은 없다.
- metadata에 temp source absolute path를 저장한 evidence는 없다.
- cleanup failure/context mismatch는 현재 evidence 기준 source defect가 아니다.
- 이 이슈는 manual runtime cleanup과 verification context의 운영 이슈로 남는다.

## G. Commit Readiness Options

### Option 1. Cleanup fully resolved

조건:

- Codex context에서도 `%LOCALAPPDATA%\FamilyClaimRef`가 absent.
- remaining files count 0.

다음:

```text
docs/117_CURRENT_WORKING_TREE_COMMIT_CANDIDATE_REVIEW.md
```

생성 가능.

### Option 2. User accepts local artifact

조건:

- 사용자가 잔여 local runtime artifact가 Git working tree 밖이고 commit 대상이 아님을 명시적으로 accept.
- cleanup context mismatch가 environment issue로 문서화됨.

다음:

```text
docs/117_CURRENT_WORKING_TREE_COMMIT_CANDIDATE_REVIEW.md
```

생성 가능.

단, commit review에는 cleanup context mismatch open/accepted를 명시해야 한다.

### Option 3. Cleanup remains blocking

조건:

- 사용자가 accept하지 않음.
- Codex context에서 files=3이 계속 보임.

다음:

- commit candidate review 보류.
- 별도 manual cleanup environment guidance 작성.

권장:

```text
If user explicitly accepts the local artifact as outside commit scope, proceed to commit candidate review with note.
Otherwise keep commit candidate review blocked.
```

## H. User Decision Needed

사용자 결정 질문:

1. Codex context에서 보이는 `%LOCALAPPDATA%\FamilyClaimRef` 잔여 3개 파일을 local runtime artifact로 accept할 것인가?
2. Git working tree 밖 artifact이므로 commit candidate review를 진행해도 되는가?
3. 아니면 Codex context에서도 absent가 될 때까지 commit candidate review를 보류할 것인가?

결정 후보:

```text
Accept Local Artifact
```

또는

```text
Keep Blocking
```

또는

```text
Retry Cleanup Outside Codex
```

## I. Scope Compliance Review

이번 작업의 범위 준수 여부:

- XAML 수정 없음.
- C# 수정 없음.
- production C# 수정 없음.
- MainWindow.xaml 수정 없음.
- MainWindow.xaml.cs 수정 없음.
- AppServices 수정 없음.
- App/App.xaml 수정 없음.
- ViewModel 수정 없음.
- file picker 수정 없음.
- workflow/coordinator/storage/file service 수정 없음.
- test code 수정 없음.
- test file 생성 없음.
- app launch 없음.
- OpenFileDialog 실행 없음.
- file select 없음.
- registration workflow 실행 없음.
- cleanup 재시도 없음.
- project root 삭제 없음.
- Policy/Claim storage 구현 없음.
- OCR/SQLite/repository 구현 없음.
- 실제 개인정보 샘플 사용 없음.
- `.sln`, `.csproj` 수정 없음.
- NuGet package 추가 없음.
- Git add/commit/reset/checkout/clean 없음.

## J. Recommendation

기본 추천:

```text
Ask user to choose whether to accept the local runtime artifact as outside commit scope.
```

사용자가 accept하면 다음 문서:

```text
docs/117_CURRENT_WORKING_TREE_COMMIT_CANDIDATE_REVIEW.md
```

사용자가 보류하면 다음 문서:

```text
docs/117_RUNTIME_CLEANUP_ENVIRONMENT_GUIDANCE.md
```

## K. Git Status Observation

Git status는 조회만 수행했다.

요약:

- modified app files exist.
- untracked `app/FamilyClaimRef.App/Composition/` exists.
- untracked docs `105` through `116` exist.
- Git add/commit/reset/checkout/clean은 수행하지 않았다.

## L. Result

```text
RUNTIME_CLEANUP_CONTEXT_MISMATCH_REVIEW_RECORDED
```
