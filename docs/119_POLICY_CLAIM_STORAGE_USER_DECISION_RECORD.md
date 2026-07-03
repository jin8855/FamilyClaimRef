# Policy / Claim Storage User Decision Record

## A. Goal

이 문서는 `docs/118_POLICY_CLAIM_STORAGE_SCOPE_DESIGN.md`의 사용자 결정 기록이다.

목적은 다음과 같다.

- Policy/Claim storage의 책임 범위와 후속 구현 방향을 확정한다.
- manual dummy `TargetId`를 실제 local target entity 기반으로 전환하기 위한 결정을 기록한다.
- document link target existence validation의 canonical boundary를 확정한다.
- 이 문서는 구현 문서가 아니다.
- C# 구현, model/interface/storage service 생성, test code 생성은 하지 않는다.

## B. Checked Files / Paths

| Path | Status | Purpose |
|---|---|---|
| `docs/118_POLICY_CLAIM_STORAGE_SCOPE_DESIGN.md` | 확인 | Policy/Claim storage scope design |
| `docs/117_CURRENT_WORKING_TREE_COMMIT_CANDIDATE_REVIEW.md` | 확인 | current commit baseline |
| `docs/110_MAINWINDOW_DOCUMENT_REGISTRATION_UI_BINDING_IMPLEMENTATION_REVIEW.md` | 확인 | MainWindow UI binding baseline |
| `docs/102_WPF_VIEWMODEL_FILE_PICKER_BOUNDARY_IMPLEMENTATION_REVIEW.md` | 확인 | ViewModel / file picker boundary baseline |
| `docs/99_IMPORT_LINK_COMBINED_WORKFLOW_IMPLEMENTATION_REVIEW.md` | 확인 | import + link workflow baseline |
| `docs/96_POLICY_CLAIM_DOCUMENT_LINK_WORKFLOW_IMPLEMENTATION_REVIEW.md` | 확인 | DocumentLinkCoordinator baseline |
| `docs/87_JSON_STORAGE_IMPLEMENTATION_REVIEW.md` | 확인 | JSON storage baseline |
| `app/FamilyClaimRef.App/Services/Storage/DocumentLinkCoordinator.cs` | 확인 | link validation boundary |
| `app/FamilyClaimRef.App/Services/Storage/DocumentRegistrationWorkflow.cs` | 확인 | registration workflow boundary |
| `app/FamilyClaimRef.App/Services/Storage/JsonDocumentStorageService.cs` | 확인 | existing JSON storage pattern |
| `app/FamilyClaimRef.App/Services/Storage/JsonFileStore.cs` | 확인 | JSON file store baseline |
| `app/FamilyClaimRef.App/Composition/AppServices.cs` | 확인 | manual composition root |

## C. User Decision Summary

| ID | Question | Decision | Recorded Direction |
|---|---|---|---|
| Q1 | Policy/Claim storage 설계를 진행할 것인가? | Accepted | 후속 구현 대상으로 둔다. |
| Q2 | Policy와 Claim storage를 함께 설계할 것인가? | Accepted | `policy / claim` target validation을 함께 설계한다. |
| Q3 | JSON files는 `policies.json`, `claims.json`으로 둘 것인가? | Accepted | 두 파일을 `%LOCALAPPDATA%\FamilyClaimRef\data\local` 아래 후보로 둔다. |
| Q4 | `JsonFileEnvelope<T>` 패턴을 재사용할 것인가? | Accepted | `SchemaVersion`, `SavedAt`, `Items` 구조를 유지한다. |
| Q5 | hard delete 없이 `DisabledAt` 기반 disable을 사용할 것인가? | Accepted | `DisabledAt != null`이면 inactive로 본다. |
| Q6 | `PolicyRecord` 최소 필드를 확정할 것인가? | Accepted | `Id`, `DisplayTitle`, `ReferenceDate`, `CreatedAt`, `UpdatedAt`, `DisabledAt`. |
| Q7 | `ClaimRecord` 최소 필드를 확정할 것인가? | Accepted | `Id`, `PolicyId`, `DisplayTitle`, `ReferenceDate`, `CreatedAt`, `UpdatedAt`, `DisabledAt`. |
| Q8 | 실제 보험/병원/진단/개인정보 필드를 MVP storage에서 제외할 것인가? | Accepted | 실제 보험계약 번호, 보험사명, 병원명, 진단명/진단코드, 개인정보 필드는 제외한다. |
| Q9 | `IPolicyClaimStorageService` combined interface를 사용할 것인가? | Accepted | MVP에서는 combined interface를 사용한다. |
| Q10 | `ClaimRecord.PolicyId`는 active policy existence validation을 요구할 것인가? | Accepted | missing policy와 disabled policy는 claim 생성에서 reject한다. |
| Q11 | document link target existence validation owner를 `DocumentLinkCoordinator`로 둘 것인가? | Accepted | link boundary에서 canonical validation을 수행한다. |
| Q12 | ViewModel은 UI pre-validation만 담당할 것인가? | Accepted | canonical target validation은 service/coordinator layer가 담당한다. |
| Q13 | AppServices에 `JsonPolicyClaimStorageService` composition을 추가할 것인가? | Accepted | manual composition root를 유지한다. |
| Q14 | 후속 구현 시 storage/link/workflow tests를 추가할 것인가? | Accepted | storage tests, link validation tests, workflow rollback tests를 추가한다. |
| Q15 | Policy/Claim selection UI는 storage 구현 이후 별도 설계로 둘 것인가? | Accepted | storage와 validation을 먼저 구현한 뒤 UI를 selection 기반으로 전환한다. |

## D. Accepted Storage Direction

확정 방향:

- Policy/Claim storage를 진행한다.
- Policy와 Claim storage를 함께 설계한다.
- `policies.json`, `claims.json`을 사용한다.
- `JsonFileEnvelope<T>`를 재사용한다.
- `DisabledAt` 기반 disable을 사용한다.
- `PolicyRecord` 최소 필드를 확정한다.
- `ClaimRecord` 최소 필드를 확정한다.
- 실제 보험계약 번호, 보험사명, 병원명, 진단명/진단코드는 제외한다.
- combined `IPolicyClaimStorageService`를 사용한다.
- Claim 생성 시 active Policy existence validation을 수행한다.
- `DocumentLinkCoordinator`가 target existence validation owner다.
- ViewModel은 UI pre-validation만 담당한다.
- AppServices에 `JsonPolicyClaimStorageService` composition 추가를 후보로 둔다.
- storage/link/workflow tests를 추가한다.
- Policy/Claim selection UI는 storage 구현 이후 별도 설계한다.

## E. Accepted Model Shape

Policy model:

```text
PolicyRecord
- Id
- DisplayTitle
- ReferenceDate
- CreatedAt
- UpdatedAt
- DisabledAt
```

Policy draft:

```text
PolicyDraft
- DisplayTitle
- ReferenceDate
```

Claim model:

```text
ClaimRecord
- Id
- PolicyId
- DisplayTitle
- ReferenceDate
- CreatedAt
- UpdatedAt
- DisabledAt
```

Claim draft:

```text
ClaimDraft
- PolicyId
- DisplayTitle
- ReferenceDate
```

제외 필드:

```text
ActualPolicyNumber
ActualClaimNumber
RealInsurerName
RealHospitalName
DiagnosisName
DiagnosisCode
ResidentNumber
PhoneNumber
Address
SensitiveHealthInfo
MedicalDetails
```

## F. Accepted Storage Interface Shape

후속 구현 후보 `IPolicyClaimStorageService` method:

```text
Task<IReadOnlyList<PolicyRecord>> GetPoliciesAsync(CancellationToken cancellationToken = default)
Task<PolicyRecord?> GetPolicyAsync(string id, CancellationToken cancellationToken = default)
Task<PolicyRecord> AddPolicyAsync(PolicyDraft draft, CancellationToken cancellationToken = default)
Task<PolicyRecord> DisablePolicyAsync(string id, CancellationToken cancellationToken = default)

Task<IReadOnlyList<ClaimRecord>> GetClaimsAsync(CancellationToken cancellationToken = default)
Task<IReadOnlyList<ClaimRecord>> GetClaimsByPolicyIdAsync(string policyId, CancellationToken cancellationToken = default)
Task<ClaimRecord?> GetClaimAsync(string id, CancellationToken cancellationToken = default)
Task<ClaimRecord> AddClaimAsync(ClaimDraft draft, CancellationToken cancellationToken = default)
Task<ClaimRecord> DisableClaimAsync(string id, CancellationToken cancellationToken = default)

Task<bool> PolicyExistsAsync(string id, CancellationToken cancellationToken = default)
Task<bool> ClaimExistsAsync(string id, CancellationToken cancellationToken = default)
```

주의:

- active-only 반환을 기본 후보로 둔다.
- disabled record 조회 필요성은 후속 hardening 후보로 둔다.
- hard delete는 제외한다.

## G. Accepted Validation Boundary

확정 boundary:

- Claim 생성 시 active Policy existence validation은 `JsonPolicyClaimStorageService`에서 수행한다.
- Policy/Claim document link target existence validation은 `DocumentLinkCoordinator`에서 수행한다.
- ViewModel은 UI pre-validation만 담당한다.
- `DocumentRegistrationWorkflow`는 기존 combined workflow 역할을 유지한다.
- link validation 실패 시 기존 rollback 정책을 유지한다.
- direct `DocumentLinkCoordinator` 사용 경로에서도 target validation이 적용되어야 한다.

## H. Implementation Candidate Files

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
app/FamilyClaimRef.App/Services/Storage/DocumentRegistrationWorkflow.cs
app/FamilyClaimRef.App/Composition/AppServices.cs
tests/FamilyClaimRef.App.Tests/DocumentLinkCoordinatorTests.cs
tests/FamilyClaimRef.App.Tests/DocumentRegistrationWorkflowTests.cs
```

주의:

- `DocumentRegistrationWorkflow` 수정이 꼭 필요한지 후속 구현 전 확인한다.
- `DocumentLinkCoordinator` 생성자 변경 시 existing tests 수정 필요.
- AppServices composition 변경은 storage implementation 이후에 진행한다.

## I. Test Scope Accepted

Storage tests:

- add policy succeeds.
- get policies returns active policies.
- disable policy hides from active list.
- add claim succeeds with active policy.
- add claim rejects missing policy.
- add claim rejects disabled policy.
- disable claim hides from active list.
- invalid JSON rejected.
- schema mismatch rejected.
- null items rejected.
- temp directory only.

Link validation tests:

- link policy document rejects missing policy.
- link claim document rejects missing claim.
- link policy document accepts active policy.
- link claim document accepts active claim.
- disabled policy rejected.
- disabled claim rejected.
- duplicate link policy rule still enforced.
- duplicate link claim rule still enforced.

Workflow tests:

- register policy document rejects missing policy before final success.
- register claim document rejects missing claim before final success.
- rollback behavior still works when link validation fails.
- no project root files.
- temp directory only.

## J. Still Out of Scope

아직 제외되는 범위:

- C# 구현 없음.
- model 생성 없음.
- interface 생성 없음.
- storage service 생성 없음.
- Json storage 수정 없음.
- DocumentLinkCoordinator 수정 없음.
- DocumentRegistrationWorkflow 수정 없음.
- ViewModel 수정 없음.
- MainWindow 수정 없음.
- AppServices 수정 없음.
- test code 생성 없음.
- app launch 없음.
- OpenFileDialog 실행 없음.
- registration workflow 실행 없음.
- DB/SQLite 구현 없음.
- OCR 구현 없음.
- repository/data access/migration 구현 없음.
- 실제 개인정보 샘플 사용 없음.
- Git add/commit/reset/checkout/clean 없음.

## K. Next Step

다음 작업 후보:

1. 별도 승인 후 Policy/Claim storage model/interface/service/tests 구현.
2. 이후 `DocumentLinkCoordinator` target existence validation 구현.
3. 이후 workflow rollback tests 보강.
4. 이후 AppServices composition에 `JsonPolicyClaimStorageService` 추가.
5. 이후 Policy/Claim selection UI 설계.

후속 문서 후보:

```text
docs/120_POLICY_CLAIM_STORAGE_IMPLEMENTATION_PLAN.md
```

또는 바로 구현 승인 시:

```text
Policy/Claim storage model/interface/service/tests 구현 지시서
```

## L. Result

```text
POLICY_CLAIM_STORAGE_USER_DECISION_RECORDED
```
