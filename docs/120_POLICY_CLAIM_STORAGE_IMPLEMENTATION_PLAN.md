# Policy / Claim Storage Implementation Plan

## A. Goal

이 문서는 Policy/Claim storage implementation plan 문서다.

목적은 다음과 같다.

- `docs/119_POLICY_CLAIM_STORAGE_USER_DECISION_RECORD.md` 사용자 결정 기준으로 구현 순서를 정한다.
- model/interface/service/tests 구현 범위를 정리한다.
- `DocumentLinkCoordinator` target existence validation 적용 순서를 정리한다.
- Workflow rollback 영향과 AppServices composition 영향도 정리한다.
- 이 문서는 구현 문서가 아니다.
- C# 구현, test 구현, AppServices 수정은 하지 않는다.

## B. Decision Baseline

`docs/119_POLICY_CLAIM_STORAGE_USER_DECISION_RECORD.md`에서 확정된 결정:

- Policy/Claim storage 함께 진행.
- `policies.json`, `claims.json`.
- `JsonFileEnvelope<T>` 재사용.
- `DisabledAt` 기반 disable.
- `PolicyRecord` 최소 필드.
- `ClaimRecord` 최소 필드.
- 실제 보험/병원/진단/개인정보 필드 제외.
- combined `IPolicyClaimStorageService`.
- Claim 생성 시 active Policy existence validation.
- `DocumentLinkCoordinator`가 target existence validation owner.
- ViewModel은 UI pre-validation만 담당.
- AppServices에 `JsonPolicyClaimStorageService` composition 추가 후보.
- storage/link/workflow tests 추가.
- Policy/Claim selection UI는 storage 구현 이후 별도 설계.

현재 기준 상태:

```text
APP_COMPOSITION_MAINWINDOW_UI_BINDING_COMMITTED
commit: 5584d63
```

현재 구현 완료:

- Document JSON storage.
- PolicyDocument / ClaimDocument link storage.
- file attachment service.
- `DocumentAttachmentCoordinator`.
- `DocumentLinkCoordinator`.
- `DocumentRegistrationWorkflow`.
- ViewModel / file picker boundary.
- AppServices.
- MainWindow 최소 UI binding.
- manual runtime check.

현재 미구현:

- `PolicyRecord` / `PolicyDraft`.
- `ClaimRecord` / `ClaimDraft`.
- `IPolicyClaimStorageService`.
- `JsonPolicyClaimStorageService`.
- `policies.json` / `claims.json`.
- Claim 생성 시 active Policy validation.
- `DocumentLinkCoordinator` target existence validation.
- AppServices에 `JsonPolicyClaimStorageService` composition.
- Policy/Claim selection UI.

## C. Implementation Strategy Candidate

### Candidate 1. Storage-only first

내용:

- Policy/Claim model/interface/service/tests만 구현한다.
- `DocumentLinkCoordinator` target validation은 후속으로 둔다.

장점:

- 변경 범위가 작다.
- storage 단독 검증이 쉽다.

단점:

- manual dummy `TargetId` 문제는 아직 해결되지 않는다.
- document registration workflow는 여전히 missing target을 허용한다.

### Candidate 2. Storage + Link validation together

내용:

- Policy/Claim storage를 구현한다.
- `DocumentLinkCoordinator`에 target existence validation을 함께 연결한다.
- 관련 link/workflow tests도 함께 보강한다.

장점:

- storage 구현의 실제 목적이 바로 반영된다.
- invalid policyId/claimId link를 막을 수 있다.
- manual dummy `TargetId`의 위험을 application boundary에서 줄인다.

단점:

- 변경 범위가 커진다.
- 기존 `DocumentLinkCoordinator` tests 수정이 필요하다.
- existing workflow tests도 보강해야 한다.

### Candidate 3. Storage + AppServices only

내용:

- Policy/Claim storage와 AppServices composition만 추가한다.
- `DocumentLinkCoordinator` validation은 후속으로 둔다.

장점:

- runtime graph에 storage를 먼저 넣을 수 있다.

단점:

- storage가 연결되어도 validation owner가 아직 사용하지 않는다.
- 의미 있는 기능 변화가 작다.

### Candidate Recommendation

권장안:

```text
Candidate 2. Storage + Link validation together
```

이유:

- `119`에서 `DocumentLinkCoordinator`를 canonical validation owner로 확정했다.
- storage만 구현하면 manual dummy `TargetId` risk가 남는다.
- link boundary에 target existence validation을 넣어야 direct link / registration workflow 모두 보호된다.

단, 구현은 단계별 commit 전 검증이 가능하도록 세부 순서를 나눈다.

## D. Implementation Phase Plan

### Phase 1. Model / Draft 생성

생성 후보:

```text
app/FamilyClaimRef.App/Models/Storage/PolicyRecord.cs
app/FamilyClaimRef.App/Models/Storage/PolicyDraft.cs
app/FamilyClaimRef.App/Models/Storage/ClaimRecord.cs
app/FamilyClaimRef.App/Models/Storage/ClaimDraft.cs
```

기준:

- 실제 보험계약 번호, 보험사명, 병원명, 진단명/진단코드 필드 없음.
- 개인정보 필드 없음.
- `DisabledAt` nullable field 사용.
- 기존 storage model style과 맞춤.

### Phase 2. Interface 생성

생성 후보:

```text
app/FamilyClaimRef.App/Services/Storage/IPolicyClaimStorageService.cs
```

method 후보는 `119`의 accepted shape를 따른다.

주의:

- active-only 반환을 기본으로 한다.
- disabled 조회는 후속 hardening 후보로 둔다.
- hard delete method 없음.

### Phase 3. JsonPolicyClaimStorageService 구현

생성 후보:

```text
app/FamilyClaimRef.App/Services/Storage/JsonPolicyClaimStorageService.cs
```

storage files:

```text
policies.json
claims.json
```

기준:

- `JsonFileEnvelope<T>` 재사용.
- `JsonFileStore<T>` 재사용.
- invalid JSON / schema mismatch / null items 정책은 기존 `JsonDocumentStorageService`와 맞춤.
- AddPolicy:
  - required DisplayTitle.
  - required ReferenceDate.
  - generated Id.
  - CreatedAt/UpdatedAt 설정.
- DisablePolicy:
  - active policy만 disable.
  - 이미 disabled 또는 missing이면 기존 storage 정책과 맞춘 예외 처리.
- AddClaim:
  - required PolicyId.
  - required DisplayTitle.
  - required ReferenceDate.
  - active Policy existence validation.
  - missing/disabled Policy reject.
- DisableClaim:
  - active claim만 disable.
- `PolicyExistsAsync`:
  - active policy 기준.
- `ClaimExistsAsync`:
  - active claim 기준.

### Phase 4. Storage Tests

생성 후보:

```text
tests/FamilyClaimRef.App.Tests/JsonPolicyClaimStorageServiceTests.cs
```

테스트:

- add policy succeeds.
- get policies returns active policies.
- get policy returns active policy.
- disable policy hides from active list.
- policy exists returns true only for active policy.
- add claim succeeds with active policy.
- add claim rejects missing policy.
- add claim rejects disabled policy.
- get claims returns active claims.
- get claims by policy id filters active claims.
- disable claim hides from active list.
- claim exists returns true only for active claim.
- invalid JSON rejected.
- schema mismatch rejected.
- null items rejected.
- tests use temp directory only.
- no project root files.

### Phase 5. DocumentLinkCoordinator validation

수정 후보:

```text
app/FamilyClaimRef.App/Services/Storage/DocumentLinkCoordinator.cs
```

변경 후보:

- constructor에 `IPolicyClaimStorageService` 추가.
- policy link 시 `PolicyExistsAsync(policyId)` 검증.
- claim link 시 `ClaimExistsAsync(claimId)` 검증.
- missing/disabled target이면 `InvalidOperationException` 또는 기존 예외 스타일과 맞춘 예외.
- duplicate link validation은 기존 유지.
- disabled Document validation 기존 유지.
- documentType validation 기존 유지.

주의:

- 기존 constructor 변경 시 tests와 AppServices composition 영향 확인 필요.
- optional dependency로 둘지 mandatory dependency로 둘지 결정 필요.
- `119` 결정상 canonical validation owner이므로 mandatory dependency 후보가 적합하다.

### Phase 6. Link / Workflow Tests 보강

수정 후보:

```text
tests/FamilyClaimRef.App.Tests/DocumentLinkCoordinatorTests.cs
tests/FamilyClaimRef.App.Tests/DocumentRegistrationWorkflowTests.cs
```

추가 테스트:

- link policy document rejects missing policy.
- link claim document rejects missing claim.
- link policy document accepts active policy.
- link claim document accepts active claim.
- disabled policy rejected.
- disabled claim rejected.
- duplicate link policy rule still enforced.
- duplicate link claim rule still enforced.
- register policy document rejects missing policy and rolls back imported document/file.
- register claim document rejects missing claim and rolls back imported document/file.
- rollback behavior still works when target validation fails.
- no project root files.
- temp directory only.

### Phase 7. AppServices composition update

수정 후보:

```text
app/FamilyClaimRef.App/Composition/AppServices.cs
```

변경:

- `JsonPolicyClaimStorageService` 생성.
- `IPolicyClaimStorageService`를 `DocumentLinkCoordinator`에 전달.
- metadata root는 기존 `%LOCALAPPDATA%\FamilyClaimRef\data\local`.
- policies/claims file 생성은 operation 시점.
- App startup만으로 `policies.json` / `claims.json` 생성되지 않아야 함.

주의:

- AppServices가 커지지 않도록 method extraction 후보 검토 가능.
- DI container는 사용하지 않는다.

## E. Constructor / Compatibility Impact

`DocumentLinkCoordinator` constructor 변경 영향:

확인 필요:

- 기존 tests에서 `DocumentLinkCoordinator` 생성 위치.
- `DocumentRegistrationWorkflowTests` setup.
- AppServices setup.
- 직접 생성되는 다른 production code 여부.

후보 정책:

```text
DocumentLinkCoordinator(IDocumentStorageService documentStorageService, IPolicyClaimStorageService policyClaimStorageService)
```

장점:

- target validation owner가 명확하다.
- invalid target link 경로를 막을 수 있다.

단점:

- 기존 tests setup 수정 필요.

주의:

- backward-compatible overload를 둘지 여부는 구현 전 결정 필요.
- MVP에서는 명확성을 위해 mandatory dependency만 둘 후보가 우선이다.

## F. ID / Naming Policy Candidate

Policy/Claim Id 생성 후보:

```text
policy_<guid compact>
claim_<guid compact>
```

예:

```text
policy_6f7a...
claim_8b2c...
```

기준:

- 실제 보험계약 번호/청구번호가 아님.
- local internal id.
- 사용자 표시용은 `DisplayTitle`.
- tests는 dummy id를 직접 만들지 않고 AddPolicy/AddClaim 결과 id를 사용하는 방향 권장.

## G. Error Policy Candidate

validation failure 후보:

- missing displayTitle -> `ArgumentException`.
- missing policyId -> `ArgumentException`.
- missing/disabled policy when adding claim -> `InvalidOperationException`.
- missing/disabled target when linking document -> `InvalidOperationException`.
- invalid JSON/schema/null items -> 기존 `InvalidOperationException` 스타일 유지.

주의:

- custom exception은 아직 도입하지 않는다.
- UI error classification은 후속 hardening 후보로 둔다.

## H. Runtime / File System Policy

파일 시스템 정책:

- storage root는 existing metadata root 사용.
- `policies.json`, `claims.json`는 operation 시점 생성.
- App startup만으로 파일 생성 금지.
- tests는 temp directory only.
- project root `attachments/`, `data/local`에 파일 생성 금지.
- DB/SQLite 구현 없음.
- OCR/repository 구현 없음.
- 실제 개인정보 샘플 없음.

## I. Implementation Candidate File List

생성 후보:

```text
app/FamilyClaimRef.App/Models/Storage/PolicyRecord.cs
app/FamilyClaimRef.App/Models/Storage/PolicyDraft.cs
app/FamilyClaimRef.App/Models/Storage/ClaimRecord.cs
app/FamilyClaimRef.App/Models/Storage/ClaimDraft.cs
app/FamilyClaimRef.App/Services/Storage/IPolicyClaimStorageService.cs
app/FamilyClaimRef.App/Services/Storage/JsonPolicyClaimStorageService.cs
tests/FamilyClaimRef.App.Tests/JsonPolicyClaimStorageServiceTests.cs
```

수정 후보:

```text
app/FamilyClaimRef.App/Services/Storage/DocumentLinkCoordinator.cs
app/FamilyClaimRef.App/Composition/AppServices.cs
tests/FamilyClaimRef.App.Tests/DocumentLinkCoordinatorTests.cs
tests/FamilyClaimRef.App.Tests/DocumentRegistrationWorkflowTests.cs
```

수정 가능 여부는 phase별로 나눈다.

주의:

- `DocumentRegistrationWorkflow.cs`는 수정 필요 여부를 먼저 확인한다.
- 가능하면 workflow 직접 수정 없이 `DocumentLinkCoordinator` validation failure를 기존 rollback flow로 처리한다.
- ViewModel/MainWindow 수정은 이번 implementation plan 기준 제외한다.

## J. Verification Plan

후속 구현 후 실행할 자동 검증:

```powershell
dotnet build FamilyClaimRef.sln
dotnet test FamilyClaimRef.sln
```

기대:

- build PASS.
- test PASS.
- 기존 216개 + 신규 tests 증가.
- no project root files.
- no DB/SQLite files.
- no actual personal sample.

추가 확인:

- `git diff --check`.
- project root `attachments/` files=0.
- project root `data/local` files=0.
- `%LOCALAPPDATA%\FamilyClaimRef`는 AppServices 생성만으로 새 파일 생성되지 않음.
- tests use temp directory only.

## K. Commit Strategy Candidate

### Option 1. One implementation commit

포함:

- models.
- service.
- link validation.
- AppServices.
- tests.
- implementation review docs.

장점:

- 기능 단위가 완결된다.

단점:

- 변경량이 크다.

### Option 2. Two commits

Commit 1:

- Policy/Claim storage model/interface/service/tests.

Commit 2:

- `DocumentLinkCoordinator` target validation + workflow tests + AppServices composition.

장점:

- 검증 단위가 명확하다.
- rollback/리뷰가 쉽다.

단점:

- 중간 상태에서 storage가 사용되지 않을 수 있다.

### Candidate Recommendation

권장:

```text
Option 2. Two commits
```

이유:

- storage 자체와 link validation을 분리해서 검토할 수 있다.
- test failure 원인 분리가 쉽다.
- AppServices composition 변경은 validation 연결과 함께 별도 commit으로 보는 편이 안전하다.

## L. Out of Scope

이번 문서에서 제외할 범위:

- C# 구현 없음.
- model 생성 없음.
- interface 생성 없음.
- storage service 생성 없음.
- `DocumentLinkCoordinator` 수정 없음.
- `DocumentRegistrationWorkflow` 수정 없음.
- AppServices 수정 없음.
- ViewModel 수정 없음.
- MainWindow 수정 없음.
- test code 생성 없음.
- app launch 없음.
- OpenFileDialog 실행 없음.
- registration workflow 실행 없음.
- Policy/Claim selection UI 구현 없음.
- DB/SQLite 구현 없음.
- OCR 구현 없음.
- repository/data access/migration 구현 없음.
- 실제 개인정보 샘플 사용 없음.
- Git add/commit/reset/checkout/clean 없음.

## M. Risks

남은 위험:

- `DocumentLinkCoordinator` constructor 변경으로 tests setup이 많이 수정될 수 있음.
- registration workflow rollback이 target validation failure와 잘 맞는지 확인 필요.
- AppServices가 커질 수 있음.
- Claim 생성 시 Policy validation을 storage에서 처리할 때 circular dependency 없이 구현해야 함.
- disabled policy에 연결된 active claims 처리 정책은 후속 hardening 필요.
- Policy disable 시 active claim 처리 정책은 아직 미정.
- UI는 여전히 manual target input 상태이므로 storage 구현 후 UI 전환 필요.
- local runtime artifact context mismatch는 commit scope 밖으로 accept되었지만 문서화 상태로 남음.

## N. Needs Decision Before Implementation

구현 전 추가 결정 필요 항목:

1. 구현을 두 commit으로 나눌 것인가?
2. 1차 구현은 storage model/interface/service/tests만 할 것인가?
3. 2차 구현에서 `DocumentLinkCoordinator` validation/AppServices/workflow tests를 할 것인가?
4. `DocumentLinkCoordinator`의 `IPolicyClaimStorageService` dependency는 mandatory로 둘 것인가?
5. disabled policy에 속한 active claim 처리 정책은 이번 구현에서 제외할 것인가?
6. policy disable 시 related claim auto-disable은 제외할 것인가?
7. `PolicyExistsAsync`, `ClaimExistsAsync`는 active-only 기준으로 둘 것인가?
8. `GetPoliciesAsync`, `GetClaimsAsync`는 active-only 반환으로 둘 것인가?
9. storage ID prefix는 `policy_`, `claim_`으로 둘 것인가?
10. custom exception 없이 기존 exception style을 유지할 것인가?

## O. Recommendation

다음 순서를 추천한다.

1. 이 계획 문서를 기준으로 구현 분할 결정을 받는다.
2. 사용자 결정 후 `docs/121_POLICY_CLAIM_STORAGE_IMPLEMENTATION_PLAN_DECISION.md`를 생성한다.
3. 그 다음 1차 구현:
   - Policy/Claim model/interface/service/tests.
4. 1차 구현 리뷰 문서 생성.
5. 2차 구현:
   - `DocumentLinkCoordinator` target validation.
   - AppServices composition.
   - workflow/link tests.
6. 2차 구현 리뷰 문서 생성.
7. 이후 Policy/Claim selection UI 설계.

## P. Result

```text
POLICY_CLAIM_STORAGE_IMPLEMENTATION_PLAN_DRAFTED
```
