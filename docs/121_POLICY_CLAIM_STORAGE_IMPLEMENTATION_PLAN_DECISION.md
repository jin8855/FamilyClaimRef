# Policy / Claim Storage Implementation Plan Decision

## A. Goal

이 문서는 `docs/120_POLICY_CLAIM_STORAGE_IMPLEMENTATION_PLAN.md`의 구현 전 결정 기록이다.

목적은 다음과 같다.

- Policy/Claim storage 구현 분할 전략을 확정한다.
- `DocumentLinkCoordinator` dependency, validation policy, query policy, exception policy를 확정한다.
- Phase 1과 Phase 2의 구현 범위를 분리한다.
- 이 문서는 구현 문서가 아니다.
- C# 구현, test 구현, AppServices 수정은 하지 않는다.

## B. Checked Files / Paths

| Path | Status | Purpose |
|---|---|---|
| `docs/120_POLICY_CLAIM_STORAGE_IMPLEMENTATION_PLAN.md` | 확인 | implementation plan baseline |
| `docs/119_POLICY_CLAIM_STORAGE_USER_DECISION_RECORD.md` | 확인 | user decision baseline |
| `docs/118_POLICY_CLAIM_STORAGE_SCOPE_DESIGN.md` | 확인 | storage scope design baseline |
| `docs/117_CURRENT_WORKING_TREE_COMMIT_CANDIDATE_REVIEW.md` | 확인 | commit candidate baseline |
| `app/FamilyClaimRef.App/Services/Storage/JsonDocumentStorageService.cs` | 확인 | existing JSON storage style |
| `app/FamilyClaimRef.App/Services/Storage/JsonFileStore.cs` | 확인 | existing JSON file store style |
| `app/FamilyClaimRef.App/Services/Storage/DocumentLinkCoordinator.cs` | 확인 | link boundary baseline |
| `app/FamilyClaimRef.App/Services/Storage/DocumentRegistrationWorkflow.cs` | 확인 | workflow rollback boundary |
| `app/FamilyClaimRef.App/Composition/AppServices.cs` | 확인 | manual composition root |

## C. User Decision Summary

| ID | Question | Decision | Recorded Direction |
|---|---|---|---|
| Q1 | 구현을 두 commit으로 나눌 것인가? | Accepted | two-commit 전략으로 나눈다. |
| Q2 | 1차 구현은 storage model/interface/service/tests만 할 것인가? | Accepted | Phase 1은 storage-only 구현으로 제한한다. |
| Q3 | 2차 구현에서 `DocumentLinkCoordinator` validation/AppServices/workflow tests를 할 것인가? | Accepted | Phase 2에서 link validation, AppServices composition, link/workflow tests를 진행한다. |
| Q4 | `DocumentLinkCoordinator`의 `IPolicyClaimStorageService` dependency는 mandatory로 둘 것인가? | Accepted | mandatory dependency로 두고 backward-compatible overload는 추가하지 않는다. |
| Q5 | disabled policy에 속한 active claim 처리 정책은 이번 구현에서 제외할 것인가? | Accepted - Deferred | parent policy가 disabled된 기존 claim 처리 정책은 후속 hardening으로 둔다. |
| Q6 | policy disable 시 related claim auto-disable은 제외할 것인가? | Accepted - Deferred | cascade disable은 이번 구현에서 제외한다. |
| Q7 | `PolicyExistsAsync`, `ClaimExistsAsync`는 active-only 기준으로 둘 것인가? | Accepted | disabled record는 exists false로 본다. |
| Q8 | `GetPoliciesAsync`, `GetClaimsAsync`는 active-only 반환으로 둘 것인가? | Accepted | disabled record 조회는 후속 hardening 후보로 둔다. |
| Q9 | storage ID prefix는 `policy_`, `claim_`으로 둘 것인가? | Accepted | local internal id prefix로 사용한다. |
| Q10 | custom exception 없이 기존 exception style을 유지할 것인가? | Accepted | `ArgumentException` / `InvalidOperationException`을 사용한다. |

## D. Accepted Implementation Split

구현은 two-commit 전략으로 나눈다.

1차:

- Policy/Claim model.
- Policy/Claim draft.
- `IPolicyClaimStorageService`.
- `JsonPolicyClaimStorageService`.
- Policy/Claim storage tests.

2차:

- `DocumentLinkCoordinator` target existence validation.
- AppServices composition.
- link validation tests.
- workflow rollback tests.

추가 기준:

- 1차 구현 후 1차 리뷰 문서를 생성한다.
- 2차 구현 후 2차 리뷰 문서를 생성한다.
- 각 phase는 build/test로 독립 검증한다.

## E. Phase 1 Accepted Scope

1차 구현 생성 후보:

```text
app/FamilyClaimRef.App/Models/Storage/PolicyRecord.cs
app/FamilyClaimRef.App/Models/Storage/PolicyDraft.cs
app/FamilyClaimRef.App/Models/Storage/ClaimRecord.cs
app/FamilyClaimRef.App/Models/Storage/ClaimDraft.cs
app/FamilyClaimRef.App/Services/Storage/IPolicyClaimStorageService.cs
app/FamilyClaimRef.App/Services/Storage/JsonPolicyClaimStorageService.cs
tests/FamilyClaimRef.App.Tests/JsonPolicyClaimStorageServiceTests.cs
```

1차 구현에서 금지:

```text
DocumentLinkCoordinator 수정
DocumentRegistrationWorkflow 수정
AppServices 수정
ViewModel 수정
MainWindow 수정
XAML 수정
app launch
OpenFileDialog 실행
registration workflow 실행
Git add/commit
```

1차 구현 기준:

- `policies.json`, `claims.json` 후보를 구현한다.
- `JsonFileEnvelope<T>` / `JsonFileStore<T>` 패턴을 재사용한다.
- `DisabledAt` 기반 disable을 사용한다.
- `PolicyExistsAsync`, `ClaimExistsAsync`는 active-only 기준이다.
- `GetPoliciesAsync`, `GetClaimsAsync`, `GetClaimsByPolicyIdAsync`는 active-only 반환이다.
- tests는 temp directory only로 작성한다.
- project root `attachments/`, `data/local`에 파일을 만들지 않는다.

## F. Phase 2 Accepted Scope

2차 구현 수정 후보:

```text
app/FamilyClaimRef.App/Services/Storage/DocumentLinkCoordinator.cs
app/FamilyClaimRef.App/Composition/AppServices.cs
tests/FamilyClaimRef.App.Tests/DocumentLinkCoordinatorTests.cs
tests/FamilyClaimRef.App.Tests/DocumentRegistrationWorkflowTests.cs
```

2차 기준:

- `IPolicyClaimStorageService`는 `DocumentLinkCoordinator` mandatory dependency다.
- target existence validation은 link boundary에서 수행한다.
- workflow 직접 수정은 가급적 피한다.
- workflow rollback은 target validation failure로 검증한다.
- `DocumentLinkCoordinator` 직접 사용 경로에서도 target validation이 적용되어야 한다.

후보 constructor:

```text
DocumentLinkCoordinator(
    IDocumentStorageService documentStorageService,
    IPolicyClaimStorageService policyClaimStorageService)
```

## G. Accepted Validation / Query Policy

확정 정책:

- `ClaimRecord.PolicyId`는 active Policy existence validation이 필요하다.
- `PolicyExistsAsync`는 active policy 기준이다.
- `ClaimExistsAsync`는 active claim 기준이다.
- `GetPoliciesAsync`는 active-only 반환이다.
- `GetClaimsAsync`는 active-only 반환이다.
- `GetClaimsByPolicyIdAsync`는 active-only 반환이다.
- disabled record 조회는 후속 hardening 후보로 둔다.
- disabled policy에 속한 active claim 처리 정책은 후속 hardening 후보로 둔다.
- policy disable 시 related claim auto-disable은 후속 hardening 후보로 둔다.

Deferred policy:

- 이미 생성된 claim의 parent policy가 나중에 disabled될 때 어떻게 처리할지는 이번 구현에서 제외한다.
- Policy disable 시 related claim cascade disable은 이번 구현에서 제외한다.

## H. Accepted ID / Error Policy

ID policy:

```text
Policy id prefix: policy_
Claim id prefix: claim_
```

예:

```text
policy_6f7a...
claim_8b2c...
```

기준:

- 실제 보험계약 번호가 아니다.
- 실제 청구번호가 아니다.
- local internal id다.

Error policy:

- custom exception은 도입하지 않는다.
- missing required input: `ArgumentException` 후보.
- invalid/missing/disabled relation: `InvalidOperationException` 후보.
- invalid JSON/schema/null items: existing `InvalidOperationException` style 유지.
- UI error classification은 후속 hardening 후보로 둔다.

## I. Verification Expectations

1차 구현 후 기대:

- `dotnet build FamilyClaimRef.sln` PASS.
- `dotnet test FamilyClaimRef.sln` PASS.
- 기존 216개 + storage tests 증가.
- no project root files.
- no DB/SQLite files.
- no actual personal sample.
- no AppServices/ViewModel/MainWindow changes.

2차 구현 후 기대:

- build/test PASS.
- link validation tests 증가.
- workflow rollback tests 증가.
- App startup만으로 `policies.json`, `claims.json` 생성 없음.
- project root pollution 없음.

## J. Still Out of Scope

아직 제외되는 범위:

- C# 구현 없음.
- model 생성 없음.
- interface 생성 없음.
- storage service 생성 없음.
- `DocumentLinkCoordinator` 수정 없음.
- `DocumentRegistrationWorkflow` 수정 없음.
- AppServices 수정 없음.
- ViewModel 수정 없음.
- MainWindow 수정 없음.
- XAML 수정 없음.
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

## K. Next Step

다음 작업:

```text
Phase 1 implementation:
Policy/Claim storage model/interface/service/tests
```

후속 문서 후보:

```text
docs/122_POLICY_CLAIM_STORAGE_PHASE1_IMPLEMENTATION_REVIEW.md
```

또는 구현 전 상세 지시서 필요 시:

```text
Policy/Claim storage phase1 implementation instruction
```

## L. Result

```text
POLICY_CLAIM_STORAGE_IMPLEMENTATION_PLAN_DECISION_RECORDED
```
