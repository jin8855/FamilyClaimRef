# Policy / Claim Storage Phase 1 Implementation Review

## A. Goal

이 문서는 Policy/Claim storage Phase 1 구현 결과 리뷰 문서다.

목적은 다음과 같다.

- Policy/Claim storage Phase 1에서 생성된 model, draft, interface, service, tests 구현 결과를 기록한다.
- `docs/121_POLICY_CLAIM_STORAGE_IMPLEMENTATION_PLAN_DECISION.md`의 storage-only 범위 준수 여부를 확인한다.
- build/test 검증 결과를 기록한다.
- Phase 2로 넘길 항목을 정리한다.
- 이 문서는 코드 수정 문서가 아니다.

## B. Checked Files / Paths

| Path | Purpose | Review Result |
|---|---|---|
| `docs/121_POLICY_CLAIM_STORAGE_IMPLEMENTATION_PLAN_DECISION.md` | Phase 1 구현 결정 기준 | PASS |
| `docs/120_POLICY_CLAIM_STORAGE_IMPLEMENTATION_PLAN.md` | implementation plan 기준 | PASS |
| `docs/119_POLICY_CLAIM_STORAGE_USER_DECISION_RECORD.md` | 사용자 결정 기준 | PASS |
| `docs/118_POLICY_CLAIM_STORAGE_SCOPE_DESIGN.md` | storage scope 기준 | PASS |
| `docs/87_JSON_STORAGE_IMPLEMENTATION_REVIEW.md` | 기존 JSON storage 구현 패턴 기준 | PASS |
| `app/FamilyClaimRef.App/Models/Storage/PolicyRecord.cs` | Policy record 구현 확인 | PASS |
| `app/FamilyClaimRef.App/Models/Storage/PolicyDraft.cs` | Policy draft 구현 확인 | PASS |
| `app/FamilyClaimRef.App/Models/Storage/ClaimRecord.cs` | Claim record 구현 확인 | PASS |
| `app/FamilyClaimRef.App/Models/Storage/ClaimDraft.cs` | Claim draft 구현 확인 | PASS |
| `app/FamilyClaimRef.App/Services/Storage/IPolicyClaimStorageService.cs` | storage interface 구현 확인 | PASS |
| `app/FamilyClaimRef.App/Services/Storage/JsonPolicyClaimStorageService.cs` | JSON storage service 구현 확인 | PASS_WITH_NOTES |
| `tests/FamilyClaimRef.App.Tests/JsonPolicyClaimStorageServiceTests.cs` | storage test coverage 확인 | PASS |
| `app/FamilyClaimRef.App/Models/Storage/JsonFileEnvelope.cs` | 기존 JSON envelope 기준 확인 | PASS |
| `app/FamilyClaimRef.App/Services/Storage/JsonFileStore.cs` | 기존 JSON file store 재사용 확인 | PASS |
| `app/FamilyClaimRef.App/Services/Storage/JsonDocumentStorageService.cs` | 기존 document JSON storage 패턴 확인 | PASS |
| `app/FamilyClaimRef.App/Services/Storage/DocumentLinkCoordinator.cs` | Phase 1 미수정 범위 확인 | PASS |
| `app/FamilyClaimRef.App/Services/Storage/DocumentRegistrationWorkflow.cs` | Phase 1 미수정 범위 확인 | PASS |
| `app/FamilyClaimRef.App/Composition/AppServices.cs` | Phase 1 미수정 범위 확인 | PASS |
| `FamilyClaimRef.sln` | solution 미수정 확인 | PASS |
| `app/FamilyClaimRef.App/FamilyClaimRef.App.csproj` | app project 미수정 확인 | PASS |
| `tests/FamilyClaimRef.App.Tests/FamilyClaimRef.App.Tests.csproj` | test project 미수정 확인 | PASS |

## C. Implementation Summary

- `PolicyRecord.cs` 생성 확인.
- `PolicyDraft.cs` 생성 확인.
- `ClaimRecord.cs` 생성 확인.
- `ClaimDraft.cs` 생성 확인.
- `IPolicyClaimStorageService.cs` 생성 확인.
- `JsonPolicyClaimStorageService.cs` 생성 확인.
- `JsonPolicyClaimStorageServiceTests.cs` 생성 확인.
- `policies.json`, `claims.json` 분리 저장 구현 확인.
- `JsonFileEnvelope<T>` / `JsonFileStore<T>` 패턴 재사용 확인.
- active-only 조회 구현 확인.
- `DisabledAt` 기반 disable 구현 확인.
- `PolicyExistsAsync`, `ClaimExistsAsync` active-only 기준 구현 확인.
- Claim 생성 시 active Policy existence validation 구현 확인.
- invalid JSON, schema mismatch, null items 테스트 포함 확인.
- tests는 temp directory만 사용한다.
- 실제 개인정보 샘플은 사용하지 않았다.

## D. Model / Draft Review

### PolicyRecord

확인된 필드:

- `Id`
- `DisplayTitle`
- `ReferenceDate`
- `CreatedAt`
- `UpdatedAt`
- `DisabledAt`

제외 확인:

- 실제 보험계약 번호 없음.
- 실제 보험사명 없음.
- 실제 고객명, 주민번호, 전화번호, 주소 없음.
- 보장내역 또는 민감정보 없음.

판정: PASS

### PolicyDraft

확인된 필드:

- `DisplayTitle`
- `ReferenceDate`

제외 확인:

- `Id` 없음.
- `CreatedAt` 없음.
- `UpdatedAt` 없음.
- `DisabledAt` 없음.

판정: PASS

### ClaimRecord

확인된 필드:

- `Id`
- `PolicyId`
- `DisplayTitle`
- `ReferenceDate`
- `CreatedAt`
- `UpdatedAt`
- `DisabledAt`

제외 확인:

- 실제 청구번호 없음.
- 실제 병원명 없음.
- 진단명 없음.
- 진단코드 없음.
- 결제금액 또는 민감 건강정보 없음.

판정: PASS

### ClaimDraft

확인된 필드:

- `PolicyId`
- `DisplayTitle`
- `ReferenceDate`

제외 확인:

- `Id` 없음.
- `CreatedAt` 없음.
- `UpdatedAt` 없음.
- `DisabledAt` 없음.

판정: PASS

## E. Interface Review

`IPolicyClaimStorageService`는 Phase 1 결정 문서의 method shape를 따른다.

확인 method:

- `GetPoliciesAsync`
- `GetPolicyAsync`
- `AddPolicyAsync`
- `DisablePolicyAsync`
- `GetClaimsAsync`
- `GetClaimsByPolicyIdAsync`
- `GetClaimAsync`
- `AddClaimAsync`
- `DisableClaimAsync`
- `PolicyExistsAsync`
- `ClaimExistsAsync`

정책 확인:

- active-only 반환 기준이다.
- disabled record 전용 조회 method는 없다.
- hard delete method는 없다.
- custom exception은 없다.

판정: PASS

## F. JsonPolicyClaimStorageService Review

확인 결과:

- constructor에서 `metadataRootPath`를 받는다.
- `metadataRootPath`가 비어 있으면 `ArgumentException`을 발생시킨다.
- `policies.json`을 사용한다.
- `claims.json`을 사용한다.
- App startup만으로 파일을 생성하지 않는다.
- operation 시점에 `JsonFileStore<T>.SaveAsync`가 저장 파일을 생성한다.
- `JsonFileEnvelope<T>` / `JsonFileStore<T>`를 재사용한다.
- invalid JSON은 `InvalidOperationException`으로 처리된다.
- schema mismatch는 `InvalidOperationException`으로 처리된다.
- null items는 `InvalidOperationException`으로 처리된다.
- `AddPolicyAsync` id prefix는 `policy_`다.
- `AddClaimAsync` id prefix는 `claim_`다.
- 생성 시 `CreatedAt`, `UpdatedAt`이 설정된다.
- 생성 시 `DisabledAt`은 null이다.
- disable 시 `DisabledAt`, `UpdatedAt`이 갱신된다.
- `PolicyExistsAsync`는 active-only 기준이다.
- `ClaimExistsAsync`는 active-only 기준이다.
- `GetPoliciesAsync`, `GetClaimsAsync`, `GetClaimsByPolicyIdAsync`는 active-only 기준이다.
- `AddClaimAsync`는 active Policy existence validation을 수행한다.
- missing policy를 reject한다.
- disabled policy를 reject한다.
- related claim cascade disable은 구현하지 않았다.
- parent policy disabled 이후 기존 active claim 처리 정책은 deferred 상태다.

판정: PASS_WITH_NOTES

보완 후보:

- disabled policy에 연결된 기존 active claim을 어떻게 조회하거나 제한할지 아직 결정되지 않았다.
- policy disable 시 related claim cascade disable은 Phase 1 범위 밖이다.
- custom exception 없이 기존 exception style을 유지한다.

## G. Test Coverage Review

`JsonPolicyClaimStorageServiceTests.cs`는 26개 `[Fact]` 테스트를 포함한다.

### Policy tests

확인:

- add policy succeeds.
- created policy id starts with `policy_`.
- get policies returns active policies.
- get policy returns active policy.
- policy exists returns true for active policy.
- disable policy hides from active list.
- disabled policy exists returns false.
- disabled policy get returns null.
- missing policy disable rejects.
- already disabled policy disable rejects.
- missing display title rejects.
- default reference date rejects.

판정: PASS

### Claim tests

확인:

- add claim succeeds with active policy.
- created claim id starts with `claim_`.
- add claim rejects missing policy.
- add claim rejects disabled policy.
- add claim rejects missing policy id.
- add claim rejects missing display title.
- add claim rejects default reference date.
- get claims returns active claims.
- get claims by policy id filters active claims.
- get claim returns active claim.
- claim exists returns true for active claim.
- disable claim hides from active list.
- disabled claim exists returns false.
- disabled claim get returns null.
- missing claim disable rejects.
- already disabled claim disable rejects.

판정: PASS

### JSON validation tests

확인:

- invalid policies JSON rejected.
- invalid claims JSON rejected.
- schema mismatch policies rejected.
- schema mismatch claims rejected.
- null policy items rejected.
- null claim items rejected.

판정: PASS

### File system safety

확인:

- tests use temp directory only.
- project root `attachments/` files remain 0.
- project root `data/local` files remain 0.
- no DB/SQLite files created.
- no actual personal sample.

판정: PASS

## H. Phase 1 Scope Compliance Review

Phase 1 storage-only 범위 준수 여부:

- `DocumentLinkCoordinator` 수정 없음.
- `DocumentRegistrationWorkflow` 수정 없음.
- `AppServices` 수정 없음.
- ViewModel 수정 없음.
- MainWindow 수정 없음.
- XAML 수정 없음.
- `App.xaml` / `App.xaml.cs` 수정 없음.
- test project 수정 없음.
- `.sln`, `.csproj` 수정 없음.
- NuGet package 추가 없음.
- app launch 없음.
- OpenFileDialog 실행 없음.
- registration workflow 실행 없음.
- Policy/Claim selection UI 구현 없음.
- DB/SQLite/OCR/repository 구현 없음.
- 실제 개인정보 샘플 사용 없음.
- Git add/commit/reset/checkout/clean 없음.

판정: PASS

## I. Verification Result

검증 명령:

```powershell
dotnet build FamilyClaimRef.sln
dotnet test FamilyClaimRef.sln
git diff --check
```

최신 검증 결과:

- `dotnet build FamilyClaimRef.sln`: PASS
- warning: 0
- error: 0
- `dotnet test FamilyClaimRef.sln`: PASS
- 총 테스트 개수: 242
- 추가 테스트 개수: 26
- 실패 테스트: 없음
- 실패 원인: 없음
- 권한 상승 실행 여부: 있음
- 초기 실패/재실행 여부: 있음
- 초기 실패 원인: Windows SDK 경로 접근 권한 문제
- 재실행 방식: 권한 상승으로 build/test 재실행
- `git diff --check`: PASS
- project root `attachments`: files=0
- project root `data/local`: files=0
- DB/SQLite unexpected file: 없음
- 실제 개인정보 샘플 사용 여부: 없음

일반 빌드 초기 실패 메시지 요약:

```text
Access to the path 'C:\Users\jin8855\AppData\Local\Microsoft SDKs' is denied.
```

## J. Current Pending Changes

현재 pending 상태에 포함된 기존 untracked docs:

- `docs/118_POLICY_CLAIM_STORAGE_SCOPE_DESIGN.md`
- `docs/119_POLICY_CLAIM_STORAGE_USER_DECISION_RECORD.md`
- `docs/120_POLICY_CLAIM_STORAGE_IMPLEMENTATION_PLAN.md`
- `docs/121_POLICY_CLAIM_STORAGE_IMPLEMENTATION_PLAN_DECISION.md`

Phase 1 생성 files:

- `app/FamilyClaimRef.App/Models/Storage/PolicyRecord.cs`
- `app/FamilyClaimRef.App/Models/Storage/PolicyDraft.cs`
- `app/FamilyClaimRef.App/Models/Storage/ClaimRecord.cs`
- `app/FamilyClaimRef.App/Models/Storage/ClaimDraft.cs`
- `app/FamilyClaimRef.App/Services/Storage/IPolicyClaimStorageService.cs`
- `app/FamilyClaimRef.App/Services/Storage/JsonPolicyClaimStorageService.cs`
- `tests/FamilyClaimRef.App.Tests/JsonPolicyClaimStorageServiceTests.cs`

이번 리뷰 문서:

- `docs/122_POLICY_CLAIM_STORAGE_PHASE1_IMPLEMENTATION_REVIEW.md`

주의:

- commit은 아직 수행하지 않았다.
- add/commit은 별도 승인 후 진행한다.

## K. Risks / Remaining Work

남은 위험:

- `DocumentLinkCoordinator` target existence validation은 Phase 2 범위다.
- AppServices composition은 Phase 2 범위다.
- DocumentRegistrationWorkflow rollback tests는 Phase 2 범위다.
- MainWindow target selection UI는 storage 이후 별도 설계가 필요하다.
- disabled policy related active claim 처리 정책은 후속 hardening 항목이다.
- policy disable 시 related claim cascade disable은 후속 hardening 항목이다.
- custom exception은 없다.
- local runtime artifact context mismatch는 commit scope 밖에서 accepted 상태로 남아 있다.
- Phase 1 storage는 아직 runtime graph에 연결되지 않았다.

## L. Recommendation

다음 작업을 권장한다.

1. Phase 1 구현은 build/test PASS 상태로 유지한다.
2. 다음 문서로 Phase 1 commit candidate review를 생성한다.

```text
docs/123_POLICY_CLAIM_STORAGE_PHASE1_COMMIT_CANDIDATE_REVIEW.md
```

또는 Phase 2를 먼저 이어갈 경우:

```text
Policy/Claim storage Phase 2 implementation instruction
```

권장:

- Phase 1은 먼저 review 후 commit하는 편이 안전하다.
- Phase 2는 별도 commit으로 진행하는 것이 `docs/121_POLICY_CLAIM_STORAGE_IMPLEMENTATION_PLAN_DECISION.md`의 결정과 맞다.

## M. Result

```text
POLICY_CLAIM_STORAGE_PHASE1_IMPLEMENTATION_REVIEWED
```
