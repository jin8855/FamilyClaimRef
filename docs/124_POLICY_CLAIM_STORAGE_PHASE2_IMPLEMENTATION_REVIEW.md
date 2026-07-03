# Policy / Claim Storage Phase 2 Implementation Review

## A. Result

```text
POLICY_CLAIM_STORAGE_PHASE2_IMPLEMENTED
```

## B. Goal

이 문서는 Policy/Claim Storage Phase 2 구현 결과 리뷰 문서다.

Phase 2의 목표는 Phase 1에서 추가한 policy/claim storage를 document link validation 흐름과 runtime composition에 연결하는 것이다.

## C. Implementation Scope

이번 구현 범위:

- `DocumentLinkCoordinator`에 `IPolicyClaimStorageService` mandatory dependency 추가.
- policy document link 생성 전 active policy existence validation 추가.
- claim document link 생성 전 active claim existence validation 추가.
- missing policy / disabled policy link 생성 차단.
- missing claim / disabled claim link 생성 차단.
- AppServices manual composition root에 `JsonPolicyClaimStorageService` 연결.
- `DocumentLinkCoordinatorTests` 보강.
- `DocumentRegistrationWorkflowTests` rollback/link validation 보강.
- constructor 변경 영향으로 `DocumentRegistrationViewModelTests` test helper 보정.

## D. Modified Files

| Path | Change |
|---|---|
| `app/FamilyClaimRef.App/Services/Storage/DocumentLinkCoordinator.cs` | `IPolicyClaimStorageService` mandatory dependency 추가, policy/claim active target validation 추가 |
| `app/FamilyClaimRef.App/Composition/AppServices.cs` | `JsonPolicyClaimStorageService` 생성 및 `DocumentLinkCoordinator` 주입 추가 |
| `tests/FamilyClaimRef.App.Tests/DocumentLinkCoordinatorTests.cs` | active/missing/disabled policy/claim validation 테스트 보강 |
| `tests/FamilyClaimRef.App.Tests/DocumentRegistrationWorkflowTests.cs` | target validation failure rollback 테스트 보강 |
| `tests/FamilyClaimRef.App.Tests/DocumentRegistrationViewModelTests.cs` | `DocumentLinkCoordinator` constructor 변경에 맞춘 test helper 보정 |

## E. Created Documents

- `docs/124_POLICY_CLAIM_STORAGE_PHASE2_IMPLEMENTATION_REVIEW.md`

## F. DocumentLinkCoordinator Review

확인:

- constructor가 `IDocumentStorageService`와 `IPolicyClaimStorageService`를 mandatory dependency로 받는다.
- optional dependency, null fallback, service locator, static access를 사용하지 않는다.
- null dependency는 `ArgumentNullException`으로 거부한다.
- policy document link 생성 전에 `PolicyExistsAsync`로 active policy existence를 검증한다.
- claim document link 생성 전에 `ClaimExistsAsync`로 active claim existence를 검증한다.
- missing target 또는 disabled target은 `InvalidOperationException`으로 link 생성을 중단한다.
- target validation 이후 기존 duplicate link validation을 수행한다.
- 기존 document storage validation과 document type validation 흐름은 유지된다.
- validation message에는 실제 보험사명, 병원명, 진단명, 계약번호, 청구번호를 포함하지 않는다.

판정:

```text
PASS
```

## G. AppServices Composition Review

확인:

- `JsonPolicyClaimStorageService`가 기존 `metadataRootPath`로 생성된다.
- `policies.json`, `claims.json` 저장 방식은 Phase 1 구현과 일치한다.
- `DocumentLinkCoordinator` 생성 시 `IPolicyClaimStorageService` instance를 전달한다.
- 기존 `App.xaml.cs -> AppServices -> MainWindow.DataContext` 흐름은 유지된다.
- MainWindow, ViewModel, XAML은 수정하지 않았다.

판정:

```text
PASS
```

## H. Test Coverage Review

### DocumentLinkCoordinatorTests

보강된 검증:

- active policy가 존재하면 policy document link 성공.
- 없는 policy이면 policy document link 실패.
- disabled policy이면 policy document link 실패.
- active claim이 존재하면 claim document link 성공.
- 없는 claim이면 claim document link 실패.
- disabled claim이면 claim document link 실패.
- target validation 실패 시 `policy-documents.json` 또는 `claim-documents.json` link persistence가 남지 않음.
- 기존 duplicate validation, disabled link duplicate 제외, document type validation, document validation 테스트 유지.

### DocumentRegistrationWorkflowTests

보강된 검증:

- missing policy target validation 실패 시 attachment rollback 수행.
- missing claim target validation 실패 시 attachment rollback 수행.
- target validation 실패 시 final link가 생성되지 않음.
- rollback 시 copied file 삭제와 document disable이 시도됨.
- 기존 rollback semantics가 유지됨.

### DocumentRegistrationViewModelTests

보정 내용:

- production ViewModel은 수정하지 않았다.
- test helper가 새 `DocumentLinkCoordinator` constructor에 맞게 fake `IPolicyClaimStorageService`를 주입한다.

## I. Explicit Non-Implementation

이번 작업에서 하지 않은 항목:

- ViewModel 수정 없음.
- MainWindow 수정 없음.
- XAML 수정 없음.
- target selection UI 구현 없음.
- policy/claim 생성 UI 구현 없음.
- DB/SQLite/OCR/repository 구현 없음.
- app launch 없음.
- OpenFileDialog 실행 없음.
- 실제 registration workflow 실행 없음.
- 실제 개인정보 샘플 사용 없음.
- 실제 가족 실명 샘플 없음.
- 실제 보험계약 번호 샘플 없음.
- 실제 청구 번호 샘플 없음.
- 실제 보험사명 샘플 없음.
- 실제 병원명 샘플 없음.
- 실제 진단명/진단코드 샘플 없음.
- Git add/commit/reset/checkout/clean 없음.

## J. Verification Result

검증 명령:

```powershell
git diff --check
dotnet build FamilyClaimRef.sln
dotnet test FamilyClaimRef.sln
```

결과:

| Check | Result | Notes |
|---|---|---|
| `git diff --check` | PASS | LF to CRLF warning only |
| `dotnet build FamilyClaimRef.sln` | PASS | elevated run, warning 0, error 0 |
| `dotnet test FamilyClaimRef.sln` | PASS | elevated run, total 249, failed 0, skipped 0 |
| project root `attachments/` | files=0 | PASS |
| project root `data/local` | files=0 | PASS |
| DB/SQLite unexpected file | 없음 | PASS |
| actual personal sample | 없음 | PASS |

일반 빌드 초기 실패:

```text
Access to the path 'C:\Users\jin8855\AppData\Local\Microsoft SDKs' is denied.
```

조치:

- Windows SDK 경로 접근 권한 문제로 권한 상승 build/test를 재실행했다.
- 권한 상승 build/test는 통과했다.

## K. Git Status Summary

문서 생성 전 확인된 변경 파일:

```text
M app/FamilyClaimRef.App/Composition/AppServices.cs
M app/FamilyClaimRef.App/Services/Storage/DocumentLinkCoordinator.cs
M tests/FamilyClaimRef.App.Tests/DocumentLinkCoordinatorTests.cs
M tests/FamilyClaimRef.App.Tests/DocumentRegistrationViewModelTests.cs
M tests/FamilyClaimRef.App.Tests/DocumentRegistrationWorkflowTests.cs
```

문서 생성 후 expected additional file:

```text
?? docs/124_POLICY_CLAIM_STORAGE_PHASE2_IMPLEMENTATION_REVIEW.md
```

## L. Remaining Risks

남은 위험:

- MainWindow target selection UI는 별도 Phase가 필요하다.
- user-facing policy/claim 선택 흐름은 별도 설계가 필요하다.
- policy/claim 생성 UX는 별도 설계가 필요하다.
- runtime manual validation은 별도 승인 후 가능하다.
- disabled policy related active claim/cascade disable 정책은 후속 hardening 항목이다.
- custom exception은 아직 없다.

## M. Next Recommendation

다음 추천 작업:

```text
Policy/Claim Storage Phase 2 Commit Candidate Review 문서 생성
```

또는:

```text
MainWindow target selection UI scope design 문서 생성
```
