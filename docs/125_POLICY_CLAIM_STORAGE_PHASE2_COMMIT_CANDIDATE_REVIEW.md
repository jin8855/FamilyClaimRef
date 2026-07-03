# Policy / Claim Storage Phase 2 Commit Candidate Review

## A. Status Marker

```text
POLICY_CLAIM_STORAGE_PHASE2_COMMIT_CANDIDATE_READY
```

## B. Review Target

검토 대상 파일:

- `app/FamilyClaimRef.App/Services/Storage/DocumentLinkCoordinator.cs`
- `app/FamilyClaimRef.App/Composition/AppServices.cs`
- `tests/FamilyClaimRef.App.Tests/DocumentLinkCoordinatorTests.cs`
- `tests/FamilyClaimRef.App.Tests/DocumentRegistrationWorkflowTests.cs`
- `tests/FamilyClaimRef.App.Tests/DocumentRegistrationViewModelTests.cs`
- `docs/124_POLICY_CLAIM_STORAGE_PHASE2_IMPLEMENTATION_REVIEW.md`
- `docs/125_POLICY_CLAIM_STORAGE_PHASE2_COMMIT_CANDIDATE_REVIEW.md`

직전 커밋:

```text
e2f629f feat(familyclaimref): add policy claim storage phase1
```

## C. Scope Review

Phase 2 범위 내 변경으로 판단한다.

확인 내용:

- `DocumentLinkCoordinator` target existence validation 추가는 Phase 2 목표와 직접 관련된다.
- `AppServices`에서 `JsonPolicyClaimStorageService`를 runtime graph에 연결한 변경은 Phase 2 목표와 직접 관련된다.
- `DocumentLinkCoordinatorTests` 보강은 policy/claim active/missing/disabled target validation 검증 범위에 해당한다.
- `DocumentRegistrationWorkflowTests` 보강은 target validation failure 후 rollback/link side effect 검증 범위에 해당한다.
- `DocumentRegistrationViewModelTests.cs` 변경은 production ViewModel 수정이 아니라 `DocumentLinkCoordinator` constructor 변경 영향에 따른 test helper 보정이다.

범위 밖 변경 확인:

- production ViewModel 수정 없음.
- MainWindow 수정 없음.
- XAML 수정 없음.
- target selection UI 구현 없음.
- DB/SQLite/OCR/repository 구현 없음.
- 불필요한 production abstraction 추가 없음.

판정:

```text
PASS
```

## D. Implementation Review

### DocumentLinkCoordinator

확인 내용:

- `IPolicyClaimStorageService`가 mandatory dependency로 추가되었다.
- optional dependency, null fallback, service locator, static access는 사용하지 않는다.
- null dependency는 `ArgumentNullException`으로 거부한다.
- policy target link 전에 `PolicyExistsAsync`로 active policy existence validation을 수행한다.
- claim target link 전에 `ClaimExistsAsync`로 active claim existence validation을 수행한다.
- missing policy / disabled policy는 link 생성을 차단한다.
- missing claim / disabled claim은 link 생성을 차단한다.
- target validation은 duplicate link validation과 link persistence 전에 수행된다.
- validation failure message에는 실제 보험사명, 병원명, 진단명, 계약번호, 청구번호가 포함되지 않는다.

판정:

```text
PASS
```

### AppServices

확인 내용:

- `JsonPolicyClaimStorageService`가 runtime graph에 구성되었다.
- 기존 `%LOCALAPPDATA%\FamilyClaimRef\data\local` metadata root 규칙을 재사용한다.
- `JsonPolicyClaimStorageService`는 Phase 1의 `policies.json`, `claims.json` 저장 방식과 일치한다.
- `DocumentLinkCoordinator` 생성 시 `IPolicyClaimStorageService`가 전달된다.
- `App.xaml.cs -> AppServices -> MainWindow.DataContext` 흐름은 깨지지 않았다.
- MainWindow, ViewModel, XAML은 수정하지 않았다.

판정:

```text
PASS
```

### Side Effect Blocking

확인 내용:

- target validation 실패 시 link persistence가 남지 않도록 테스트가 보강되었다.
- workflow target validation 실패 시 copied file 삭제와 document disable rollback이 수행되는지 테스트가 보강되었다.
- app launch, OpenFileDialog, actual registration workflow 실행 없이 테스트 범위에서만 검증했다.

판정:

```text
PASS
```

## E. Test Review

보강된 테스트 요약:

- `DocumentLinkCoordinatorTests`에 active policy link 성공 검증 유지.
- `DocumentLinkCoordinatorTests`에 missing policy link 실패 검증 추가.
- `DocumentLinkCoordinatorTests`에 disabled policy link 실패 검증 추가.
- `DocumentLinkCoordinatorTests`에 active claim link 성공 검증 유지.
- `DocumentLinkCoordinatorTests`에 missing claim link 실패 검증 추가.
- `DocumentLinkCoordinatorTests`에 disabled claim link 실패 검증 추가.
- `DocumentLinkCoordinatorTests`에 validation 실패 후 link persistence가 남지 않는 검증 추가.
- `DocumentRegistrationWorkflowTests`에 missing policy target failure rollback 검증 추가.
- `DocumentRegistrationWorkflowTests`에 missing claim target failure rollback 검증 추가.
- `DocumentRegistrationViewModelTests`는 constructor 변경에 맞춰 test helper만 보정했다.

전체 테스트 결과:

```text
total tests: 249
passed tests: 249
failed tests: 0
skipped tests: 0
```

판정:

```text
PASS
```

## F. Safety Review

확인 내용:

- 실제 개인정보 샘플 없음.
- 실제 보험사명 샘플 없음.
- 실제 병원명 샘플 없음.
- 실제 진단명/진단코드 샘플 없음.
- 실제 계약번호 샘플 없음.
- 실제 청구번호 샘플 없음.
- DB/SQLite unexpected file 없음.
- project root `attachments/` files=0.
- project root `data/local` files=0.
- app launch 없음.
- OpenFileDialog 실행 없음.
- registration workflow 실제 실행 없음.
- Git add/commit/reset/checkout/clean 없음.

판정:

```text
PASS
```

## G. Verification Results

검증 명령:

```powershell
git diff --check
dotnet build FamilyClaimRef.sln
dotnet test FamilyClaimRef.sln
git status --short
```

검증 결과:

| Check | Result | Notes |
|---|---|---|
| `git diff --check` | PASS | LF to CRLF warning only |
| `dotnet build FamilyClaimRef.sln` | PASS | elevated run, warning 0, error 0 |
| `dotnet test FamilyClaimRef.sln` | PASS | elevated run, total 249, failed 0, skipped 0 |
| `git status --short` | PASS | expected Phase 2 files only |
| project root `attachments/` | PASS | files=0 |
| project root `data/local` | PASS | files=0 |
| DB/SQLite unexpected file | PASS | 없음 |
| actual personal sample | PASS | 없음 |

일반 build 초기 실패:

```text
Access to the path 'C:\Users\jin8855\AppData\Local\Microsoft SDKs' is denied.
```

기록:

- Windows SDK 경로 접근 권한 문제로 일반 build는 실패했다.
- 권한 상승 build/test는 통과했다.
- 권한 상승 필요는 환경 접근 권한 문제이며 구현 실패 사유가 아니다.

## H. Git Status Summary

문서 생성 전 확인된 status:

```text
 M app/FamilyClaimRef.App/Composition/AppServices.cs
 M app/FamilyClaimRef.App/Services/Storage/DocumentLinkCoordinator.cs
 M tests/FamilyClaimRef.App.Tests/DocumentLinkCoordinatorTests.cs
 M tests/FamilyClaimRef.App.Tests/DocumentRegistrationViewModelTests.cs
 M tests/FamilyClaimRef.App.Tests/DocumentRegistrationWorkflowTests.cs
?? docs/124_POLICY_CLAIM_STORAGE_PHASE2_IMPLEMENTATION_REVIEW.md
```

이 문서 생성 후 expected additional file:

```text
?? docs/125_POLICY_CLAIM_STORAGE_PHASE2_COMMIT_CANDIDATE_REVIEW.md
```

unexpected file:

```text
없음
```

## I. Commit Readiness

commit readiness:

```text
ready
```

reason:

- Phase 2 목적과 직접 관련된 파일만 변경되었다.
- production ViewModel, MainWindow, XAML 변경이 없다.
- DB/SQLite/OCR/repository 구현이 없다.
- `git diff --check`가 통과했다.
- `dotnet build FamilyClaimRef.sln`이 권한 상승 실행에서 통과했다.
- `dotnet test FamilyClaimRef.sln`이 권한 상승 실행에서 통과했다.
- project root `attachments/`, `data/local` 오염이 없다.
- 실제 개인정보 샘플이 없다.

## J. Commit Candidate Exact File List

commit candidate exact file list:

- `app/FamilyClaimRef.App/Services/Storage/DocumentLinkCoordinator.cs`
- `app/FamilyClaimRef.App/Composition/AppServices.cs`
- `tests/FamilyClaimRef.App.Tests/DocumentLinkCoordinatorTests.cs`
- `tests/FamilyClaimRef.App.Tests/DocumentRegistrationWorkflowTests.cs`
- `tests/FamilyClaimRef.App.Tests/DocumentRegistrationViewModelTests.cs`
- `docs/124_POLICY_CLAIM_STORAGE_PHASE2_IMPLEMENTATION_REVIEW.md`
- `docs/125_POLICY_CLAIM_STORAGE_PHASE2_COMMIT_CANDIDATE_REVIEW.md`

## K. Recommended Commit Message

```text
feat(familyclaimref): add policy claim link validation
```

## L. Remaining Risks / Follow-up

남은 위험 및 후속:

- MainWindow target selection UI는 별도 Phase.
- policy/claim 생성/선택 UX는 별도 설계 필요.
- runtime manual validation은 별도 승인 후 진행.
- disabled policy related active claim/cascade disable은 후속 hardening 항목.

## M. Result

```text
POLICY_CLAIM_STORAGE_PHASE2_COMMIT_CANDIDATE_READY
```
