# Policy / Claim Storage Phase 1 Commit Candidate Review

## A. Goal

이 문서는 Policy/Claim storage Phase 1 commit candidate review 문서다.

목적은 다음과 같다.

- Phase 1 storage-only 변경을 commit 후보로 분류한다.
- source/doc changes와 runtime artifact를 분리한다.
- commit 전 build/test와 scope compliance를 확인한다.
- 이 문서는 commit 수행 문서가 아니다.
- Git add/commit/reset/checkout/clean은 수행하지 않는다.

## B. Source Documents

기준 문서는 다음과 같다.

| Document | Status |
|---|---|
| `docs/122_POLICY_CLAIM_STORAGE_PHASE1_IMPLEMENTATION_REVIEW.md` | Checked |
| `docs/121_POLICY_CLAIM_STORAGE_IMPLEMENTATION_PLAN_DECISION.md` | Checked |
| `docs/120_POLICY_CLAIM_STORAGE_IMPLEMENTATION_PLAN.md` | Checked |
| `docs/119_POLICY_CLAIM_STORAGE_USER_DECISION_RECORD.md` | Checked |
| `docs/118_POLICY_CLAIM_STORAGE_SCOPE_DESIGN.md` | Checked |
| `docs/117_CURRENT_WORKING_TREE_COMMIT_CANDIDATE_REVIEW.md` | Checked |

## C. Phase 1 Scope Baseline

Phase 1 기준:

- storage-only implementation.
- Policy/Claim model/draft.
- `IPolicyClaimStorageService`.
- `JsonPolicyClaimStorageService`.
- storage tests.
- no `DocumentLinkCoordinator` changes.
- no Workflow changes.
- no AppServices changes.
- no ViewModel/MainWindow/XAML changes.
- no app launch/OpenFileDialog/registration workflow.
- no Git add/commit/reset/checkout/clean.

## D. Git Status Summary

`git status --short` 확인 결과:

```text
?? app/FamilyClaimRef.App/Models/Storage/ClaimDraft.cs
?? app/FamilyClaimRef.App/Models/Storage/ClaimRecord.cs
?? app/FamilyClaimRef.App/Models/Storage/PolicyDraft.cs
?? app/FamilyClaimRef.App/Models/Storage/PolicyRecord.cs
?? app/FamilyClaimRef.App/Services/Storage/IPolicyClaimStorageService.cs
?? app/FamilyClaimRef.App/Services/Storage/JsonPolicyClaimStorageService.cs
?? docs/118_POLICY_CLAIM_STORAGE_SCOPE_DESIGN.md
?? docs/119_POLICY_CLAIM_STORAGE_USER_DECISION_RECORD.md
?? docs/120_POLICY_CLAIM_STORAGE_IMPLEMENTATION_PLAN.md
?? docs/121_POLICY_CLAIM_STORAGE_IMPLEMENTATION_PLAN_DECISION.md
?? docs/122_POLICY_CLAIM_STORAGE_PHASE1_IMPLEMENTATION_REVIEW.md
?? tests/FamilyClaimRef.App.Tests/JsonPolicyClaimStorageServiceTests.cs
```

이 문서 생성 후 추가되는 expected file:

```text
?? docs/123_POLICY_CLAIM_STORAGE_PHASE1_COMMIT_CANDIDATE_REVIEW.md
```

요약:

| Type | Count | Notes |
|---|---:|---|
| modified files | 0 | 없음 |
| untracked Phase 1 source/test files | 7 | expected |
| untracked Phase 1 docs | 6 | docs 118~123 expected |
| deleted files | 0 | 없음 |
| unexpected files | 0 | 없음 |

판정:

```text
PASS
```

## E. Diff / Whitespace Check

명령:

```powershell
git diff --check
git diff --stat
```

결과:

| Command | Result | Notes |
|---|---|---|
| `git diff --check` | PASS | whitespace issue 없음 |
| `git diff --stat` | PASS | tracked modified files가 없어 출력 없음 |

주의:

- 현재 변경은 untracked file 중심이다.
- `git diff --stat`은 untracked file을 표시하지 않는다.
- untracked file 범위는 `git status --short` 기준으로 분류했다.

## F. Commit Candidate File List

| Path | Status | Category | Reason | Candidate Decision |
|---|---|---|---|---|
| `app/FamilyClaimRef.App/Models/Storage/PolicyRecord.cs` | untracked | Policy model | Policy record 최소 필드 구현 | Include |
| `app/FamilyClaimRef.App/Models/Storage/PolicyDraft.cs` | untracked | Policy model | Policy draft 최소 입력 모델 구현 | Include |
| `app/FamilyClaimRef.App/Models/Storage/ClaimRecord.cs` | untracked | Claim model | Claim record 최소 필드 구현 | Include |
| `app/FamilyClaimRef.App/Models/Storage/ClaimDraft.cs` | untracked | Claim model | Claim draft 최소 입력 모델 구현 | Include |
| `app/FamilyClaimRef.App/Services/Storage/IPolicyClaimStorageService.cs` | untracked | Storage interface | Policy/Claim combined storage interface 구현 | Include |
| `app/FamilyClaimRef.App/Services/Storage/JsonPolicyClaimStorageService.cs` | untracked | Storage service | `policies.json`, `claims.json` JSON storage 구현 | Include |
| `tests/FamilyClaimRef.App.Tests/JsonPolicyClaimStorageServiceTests.cs` | untracked | Storage tests | Policy/Claim storage behavior와 JSON validation 테스트 | Include |
| `docs/118_POLICY_CLAIM_STORAGE_SCOPE_DESIGN.md` | untracked | Architecture docs | storage scope design | Include |
| `docs/119_POLICY_CLAIM_STORAGE_USER_DECISION_RECORD.md` | untracked | Architecture docs | user decision record | Include |
| `docs/120_POLICY_CLAIM_STORAGE_IMPLEMENTATION_PLAN.md` | untracked | Architecture docs | implementation plan | Include |
| `docs/121_POLICY_CLAIM_STORAGE_IMPLEMENTATION_PLAN_DECISION.md` | untracked | Architecture docs | implementation plan decision | Include |
| `docs/122_POLICY_CLAIM_STORAGE_PHASE1_IMPLEMENTATION_REVIEW.md` | untracked | Implementation review docs | Phase 1 implementation review | Include |
| `docs/123_POLICY_CLAIM_STORAGE_PHASE1_COMMIT_CANDIDATE_REVIEW.md` | new in this step | Commit review docs | Phase 1 exact commit scope review | Include |

## G. Explicit Exclusions

아래 항목은 commit 대상에서 명시적으로 제외한다.

```text
DocumentLinkCoordinator.cs
DocumentRegistrationWorkflow.cs
AppServices.cs
ViewModel files
MainWindow files
XAML files
app launch/runtime artifacts
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

| Exclusion | Status |
|---|---|
| `DocumentLinkCoordinator.cs` | no status change |
| `DocumentRegistrationWorkflow.cs` | no status change |
| `AppServices.cs` | no status change |
| ViewModel files | no status change |
| MainWindow files | no status change |
| XAML files | no status change |
| `.sln`, `.csproj` | no status change |
| project root `attachments/` | files=0 |
| project root `data/local` | files=0 |
| DB/SQLite unexpected files | 없음 |

## H. Build / Test Verification

명령:

```powershell
dotnet build FamilyClaimRef.sln
dotnet test FamilyClaimRef.sln
```

일반 sandbox 실행:

| Command | Result | Warnings | Errors | Notes |
|---|---|---:|---:|---|
| `dotnet build FamilyClaimRef.sln` | FAIL | 0 | 2 | Windows SDK path access denied |

초기 실패 메시지 요약:

```text
Access to the path 'C:\Users\jin8855\AppData\Local\Microsoft SDKs' is denied.
```

권한 상승 재실행:

| Command | Result | Warnings | Errors | Notes |
|---|---|---:|---:|---|
| `dotnet build FamilyClaimRef.sln` | PASS | 0 | 0 | elevated run |
| `dotnet test FamilyClaimRef.sln` | PASS | 0 | 0 | elevated run |

Test summary:

```text
total tests: 242
passed tests: 242
failed tests: 0
skipped tests: 0
```

검증 판정:

```text
PASS
```

## I. File System Safety Check

| Check | Result |
|---|---|
| project root `attachments/` | files=0 |
| project root `data/local` | files=0 |
| DB/SQLite unexpected file | 없음 |
| actual personal sample | 없음 |
| `%LOCALAPPDATA%\FamilyClaimRef` | outside commit scope |

판정:

```text
PASS
```

## J. Runtime Evidence / Artifact Note

기록:

- 이번 Phase 1에서는 app launch 없음.
- OpenFileDialog 실행 없음.
- registration workflow 실행 없음.
- local runtime artifact context mismatch는 이전 commit에서 accepted outside commit scope로 문서화됨.
- Phase 1 tests는 temp directory만 사용한다.
- storage service는 아직 runtime graph에 연결되지 않았다.
- `%LOCALAPPDATA%\FamilyClaimRef`는 Git working tree 밖의 local runtime artifact이며 commit 대상이 아니다.

## K. Commit Readiness

판정:

```text
POLICY_CLAIM_STORAGE_PHASE1_COMMIT_CANDIDATE_READY
```

근거:

- git status contains only expected Phase 1 files/docs.
- `git diff --check` PASS.
- build/test PASS after elevated rerun.
- project root pollution 없음.
- no DB/SQLite unexpected files.
- no actual personal sample.
- no unexpected files.
- no forbidden source/project file changes.

## L. Suggested Commit Scope

commit message 후보:

```text
feat(familyclaimref): add policy claim storage phase1
```

대안:

```text
feat(familyclaimref): add policy and claim JSON storage
```

포함 요약:

- Policy/Claim record/draft models.
- `IPolicyClaimStorageService`.
- `JsonPolicyClaimStorageService`.
- storage tests.
- design/decision/plan/review docs.

주의:

- 이 문서에서는 commit하지 않는다.
- 실제 commit은 별도 승인 후 수행한다.
- stage는 exact file list로만 해야 한다.

## M. Remaining Risks

남은 위험:

- `DocumentLinkCoordinator` target existence validation은 Phase 2 범위다.
- AppServices composition은 Phase 2 범위다.
- DocumentRegistrationWorkflow rollback tests는 Phase 2 범위다.
- storage는 아직 runtime graph에 연결되지 않았다.
- MainWindow target selection UI는 별도 설계가 필요하다.
- disabled policy related active claim/cascade disable은 후속 hardening 항목이다.
- custom exception은 없다.
- local runtime artifact context mismatch는 commit scope 밖으로 accepted/documented 상태다.

## N. Recommendation

Ready 상태이므로 다음 작업 후보:

```text
Next: exact-file-list git add/commit instruction for Phase 1.
```

주의:

- 다음 단계에서도 `git add .`보다 exact file list staging을 권장한다.
- Phase 2는 별도 commit으로 진행하는 것이 `docs/121_POLICY_CLAIM_STORAGE_IMPLEMENTATION_PLAN_DECISION.md`와 맞다.

## O. Result

```text
POLICY_CLAIM_STORAGE_PHASE1_COMMIT_CANDIDATE_READY
```
