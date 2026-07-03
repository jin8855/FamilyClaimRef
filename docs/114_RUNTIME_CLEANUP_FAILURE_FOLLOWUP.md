# Runtime Cleanup Failure Follow-up

## A. Goal

이 문서는 `docs/113_MAINWINDOW_DOCUMENT_REGISTRATION_MANUAL_RUNTIME_CHECK_REVIEW.md`에서 `Still Open`으로 남은 runtime cleanup failure를 별도 follow-up으로 정리한다.

이 문서의 목적은 다음과 같다.

- `%LOCALAPPDATA%\FamilyClaimRef`에 남은 dummy runtime files를 추적한다.
- MainWindow document registration manual runtime 기능 흐름과 cleanup failure를 분리한다.
- commit-readiness 판단 전에 필요한 정리 조건을 명시한다.
- 사용자 측 manual cleanup 필요 사항을 기록한다.
- 이 문서는 코드 수정 문서가 아니다.

## B. Source Documents

확인 기준 문서는 다음과 같다.

| 문서 | 확인 내용 |
|---|---|
| `docs/113_MAINWINDOW_DOCUMENT_REGISTRATION_MANUAL_RUNTIME_CHECK_REVIEW.md` | cleanup failure review, classification, open issue |
| `docs/112_MAINWINDOW_DOCUMENT_REGISTRATION_MANUAL_RUNTIME_CHECK_RESULT.md` | actual runtime check result, generated runtime files, cleanup failure |
| `docs/111_MAINWINDOW_DOCUMENT_REGISTRATION_MANUAL_RUNTIME_CHECK_PLAN.md` | manual runtime check plan, expected cleanup target |
| `docs/110_MAINWINDOW_DOCUMENT_REGISTRATION_UI_BINDING_IMPLEMENTATION_REVIEW.md` | UI binding implementation review, build/test result |

## C. Functional Runtime Result Summary

기능 runtime 흐름 결과는 cleanup failure와 분리해서 판단한다.

| 항목 | 결과 | 비고 |
|---|---|---|
| build | PASS | `dotnet build FamilyClaimRef.sln` 기준 |
| test | PASS | `dotnet test FamilyClaimRef.sln` 기준 |
| total tests | 216 PASS | 실패 테스트 없음 |
| app launch | PASS | WPF exe launch 후 MainWindow 표시 |
| MainWindow 표시 | PASS | startup crash 없음 |
| UI controls 표시 | PASS | document registration 최소 controls 표시 |
| DatePicker binding | PASS_WITH_NOTES | 입력 flow crash는 없었으나 calendar popup 세부 검증은 남음 |
| OpenFileDialog cancel | PASS | button click 후 dialog 표시, cancel 가능 |
| dummy file select | PASS | 선택 파일명 표시, 전체 source path 노출 없음 |
| validation | PASS | TargetId 누락 시 validation message 표시 |
| dummy registration | PASS_WITH_NOTES | `.png` dummy file 등록 성공, `.txt`는 allowlist 밖이라 실패 |
| metadata absolute source path 저장 | 없음 | source temp absolute path 저장 evidence 없음 |
| `DocumentRecord.RelativePath` | root-relative shape | attachment root 기준 relative path |
| project root pollution | 없음 | project root `attachments/`, `data/local` files=0 |
| actual personal sample 사용 | 없음 | dummy-only sample 사용 |

결론:

```text
Functional runtime flow: PASS_WITH_NOTES
Blocking item: cleanup failure only
```

## D. Cleanup Failure Summary

Cleanup failure target:

```text
%LOCALAPPDATA%\FamilyClaimRef
```

현재 확인된 절대 경로:

```text
C:\Users\jin8855\AppData\Local\FamilyClaimRef
```

Remaining files:

```text
C:\Users\jin8855\AppData\Local\FamilyClaimRef\attachments\documents\policy-document_20260702_policy_001.png
C:\Users\jin8855\AppData\Local\FamilyClaimRef\data\local\documents.json
C:\Users\jin8855\AppData\Local\FamilyClaimRef\data\local\policy-documents.json
```

Cleanup failure evidence:

| Context | Observed Result |
|---|---|
| normal shell | path visible |
| normal shell delete | access denied |
| elevated shell | same absolute path absent |
| current classification | manual cleanup/environment issue |
| severity | Medium |
| source code defect evidence | 현재 기준 없음 |

이번 follow-up 문서 작성 중에는 cleanup 재시도를 수행하지 않았다.

## E. Current Status

현재 상태:

```text
Still Open
```

판정 이유:

- 사용자가 직접 삭제했다는 확인 증거가 아직 없다.
- `%LOCALAPPDATA%\FamilyClaimRef`가 현재 확인 기준으로 존재한다.
- remaining files count는 3개다.
- source tree 내부 pollution은 확인되지 않았다.
- cleanup failure는 runtime artifact 정리 문제로 남아 있다.

상태 전환 기준:

| 상태 | 조건 |
|---|---|
| `Still Open` | 사용자 직접 삭제 또는 accept 증거 없음 |
| `Resolved` | `%LOCALAPPDATA%\FamilyClaimRef` 삭제 완료 확인 |
| `Accepted Local Artifact` | 사용자가 local runtime artifact 잔여를 명시적으로 accept |

## F. User Action Required

사용자 측 정리 대상:

```text
C:\Users\jin8855\AppData\Local\FamilyClaimRef
```

권장 조치:

1. Windows Explorer에서 아래 폴더로 이동한다.

```text
C:\Users\jin8855\AppData\Local
```

2. `FamilyClaimRef` 폴더를 삭제한다.

3. 삭제 후 아래 3개 파일이 남아 있지 않은지 확인한다.

```text
C:\Users\jin8855\AppData\Local\FamilyClaimRef\attachments\documents\policy-document_20260702_policy_001.png
C:\Users\jin8855\AppData\Local\FamilyClaimRef\data\local\documents.json
C:\Users\jin8855\AppData\Local\FamilyClaimRef\data\local\policy-documents.json
```

PowerShell 확인 후보:

```powershell
Test-Path "$env:LOCALAPPDATA\FamilyClaimRef"
```

기대 결과:

```text
False
```

주의:

- `C:\EtcProject\FamilyClaimRef` project root는 삭제하지 않는다.
- project root `attachments/`, `data/local`은 삭제하지 않는다.
- `git clean`은 사용하지 않는다.
- 실제 개인정보 파일은 아니지만 runtime artifact이므로 commit 전 정리하는 편이 안전하다.

## G. Commit Readiness Impact

Commit readiness 영향:

- source tree에는 project root pollution이 없다.
- Git add/commit/reset/checkout/clean은 수행되지 않았다.
- runtime artifact는 Git working tree 밖에 있다.
- cleanup failure가 open이면 운영상 commit-ready 판단을 보류하는 편이 안전하다.

Commit-ready 조건 후보:

1. `%LOCALAPPDATA%\FamilyClaimRef` 삭제 확인.
2. 또는 사용자가 local runtime artifact 유지를 명시적으로 accept.
3. cleanup failure가 environment-only로 문서화되고 commit scope에서 제외됨을 명시.

권장:

```text
Commit candidate review는 cleanup 상태가 Resolved 또는 User Accepted가 된 뒤 진행한다.
```

## H. Follow-up Verification Plan

사용자 삭제 후 final verification 대상:

```text
%LOCALAPPDATA%\FamilyClaimRef
C:\EtcProject\FamilyClaimRef\attachments
C:\EtcProject\FamilyClaimRef\data\local
```

확인 항목:

- `%LOCALAPPDATA%\FamilyClaimRef`: absent
- project root `attachments/`: files=0
- project root `data/local`: files=0
- DB/SQLite unexpected file 없음
- Git status는 source changes만 표시
- Git add/commit/reset/checkout/clean 없음

후속 문서 후보:

```text
docs/115_RUNTIME_CLEANUP_FINAL_VERIFICATION.md
```

사용자가 잔여 artifact를 명시적으로 accept하고 commit 후보 정리로 바로 가는 경우:

```text
docs/115_CURRENT_WORKING_TREE_COMMIT_CANDIDATE_REVIEW.md
```

## I. Risk Review

남은 위험:

- local runtime artifact가 남아 있으면 다음 manual runtime check에 영향을 줄 수 있다.
- 기존 `policy-document_20260702_policy_001.png`가 남아 있으면 duplicate filename 또는 duplicateIndex 확인 시 간섭할 수 있다.
- production root가 이미 존재하면 fresh-run evidence가 흐려질 수 있다.
- cleanup failure는 권한, 가상화, 계정 context 문제일 가능성이 있다.
- 현재 source code defect evidence는 없지만 runtime environment issue는 별도 관리가 필요하다.

## J. Recommendation

추천 순서:

1. 사용자가 `C:\Users\jin8855\AppData\Local\FamilyClaimRef`를 직접 삭제한다.
2. 삭제 후 final file system verification 문서를 생성한다.
3. cleanup resolved가 확인되면 current working tree commit candidate review 문서를 생성한다.
4. commit은 별도 승인 후 수행한다.

다음 추천 문서:

삭제 확인 후:

```text
docs/115_RUNTIME_CLEANUP_FINAL_VERIFICATION.md
```

또는 사용자가 잔여 local artifact를 accept하면:

```text
docs/115_CURRENT_WORKING_TREE_COMMIT_CANDIDATE_REVIEW.md
```

## K. Result

```text
RUNTIME_CLEANUP_FAILURE_FOLLOWUP_RECORDED
```
