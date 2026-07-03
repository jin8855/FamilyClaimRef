# Policy / Claim Storage Scope Design

## A. Goal

이 문서는 Policy / Claim storage scope design 문서다.

목적은 다음과 같다.

- manual dummy `TargetId`를 실제 local target entity 기반으로 전환하기 위한 설계를 정리한다.
- Policy / Claim entity 저장, 조회, disable, active validation 범위를 검토한다.
- document registration workflow와 link validation에 어떤 영향을 주는지 검토한다.
- Policy / Claim target selection UI로 가기 전 storage 책임 경계를 먼저 정리한다.
- 이 문서는 구현 문서가 아니다.
- C# 구현, XAML 수정, test 구현은 하지 않는다.

## B. Current State

현재 확정 상태:

```text
APP_COMPOSITION_MAINWINDOW_UI_BINDING_COMMITTED
commit: 5584d63
working tree: clean
```

현재 구현된 것:

- `DocumentRecord` / `PolicyDocumentRecord` / `ClaimDocumentRecord`.
- JSON metadata storage.
- file attachment service.
- `DocumentAttachmentCoordinator`.
- `DocumentLinkCoordinator`.
- `DocumentRegistrationWorkflow`.
- `IFilePickerService`.
- `DocumentRegistrationViewModel`.
- `AppServices`.
- MainWindow 최소 UI binding.
- document registration manual runtime check.

현재 미구현:

- Policy storage.
- Claim storage.
- Policy/Claim target selection.
- policyId/claimId existence validation.
- real target list UI.
- Policy/Claim entity creation UI.
- Policy/Claim storage 기반 document link validation.

확인된 현 상태:

- Document storage는 구현되어 있다.
- PolicyDocument / ClaimDocument link storage는 구현되어 있다.
- file import + metadata save + link workflow는 구현되어 있다.
- ViewModel과 MainWindow는 target id를 manual input으로 받는다.
- Policy/Claim storage가 없기 때문에 target existence validation이 없다.
- Policy/Claim storage가 없기 때문에 target selection UI가 없다.
- manual runtime check에서는 dummy `POLICY-DEMO-001`로 policy document registration이 가능했다.
- project root pollution은 없다.
- production root는 `%LOCALAPPDATA%\FamilyClaimRef` 기준이다.

## C. Problem Statement

현재 문제:

- 현재 `TargetId`는 manual dummy input이다.
- 실제 policyId / claimId가 존재하는지 검증하지 않는다.
- 사용자는 실제 등록 대상 목록을 선택할 수 없다.
- `PolicyDocumentRecord`와 `ClaimDocumentRecord`는 link는 저장하지만, link 대상인 Policy / Claim entity는 없다.
- Claim이 어떤 Policy에 속하는지도 아직 저장되지 않는다.
- document link validation의 책임 위치가 정해지지 않았다.
- UI가 target id를 직접 입력하게 두면 MVP 이후 사용성이 낮고 데이터 무결성도 약하다.

현재 `DocumentLinkCoordinator`는 active duplicate link는 검증하지만, policyId / claimId target existence는 검증하지 않는다.

## D. Scope Candidate

### Candidate 1. Policy storage only

내용:

- `PolicyRecord`만 저장한다.
- claim은 아직 만들지 않는다.
- document registration은 policy document만 실제 target validation 가능하다.
- claim document는 manual dummy 상태를 유지한다.

장점:

- 범위가 작다.
- policy document 흐름부터 안정화 가능하다.

단점:

- claim document 흐름의 dummy 상태가 계속 남는다.
- ClaimDocument link validation은 해결되지 않는다.

### Candidate 2. Policy + Claim storage together

내용:

- `PolicyRecord`와 `ClaimRecord`를 함께 설계한다.
- Claim은 Policy에 속할 수 있다.
- policy document와 claim document 모두 target validation 후보가 생긴다.

장점:

- 현재 UI의 `TargetKind: policy / claim` 구조와 맞다.
- manual dummy input 제거 방향이 명확하다.
- document link validation을 양쪽 모두 설계할 수 있다.

단점:

- 설계 범위가 Candidate 1보다 크다.
- Claim의 필드 범위를 신중히 줄여야 한다.

### Candidate 3. Target registry abstraction

내용:

- Policy/Claim을 별도 entity로 만들지 않고 target id registry만 만든다.
- 예: `DocumentTargetRecord { Id, TargetKind, DisplayTitle }`

장점:

- 단순하다.
- UI target selection만 빠르게 가능하다.

단점:

- Policy/Claim domain이 모호해진다.
- Claim이 Policy에 속한다는 관계를 표현하기 어렵다.
- 나중에 다시 분리할 가능성이 높다.

### Candidate Recommendation

후보 권장안:

```text
Candidate 2. Policy + Claim storage together
```

이유:

- 현재 UI가 이미 `policy / claim` target kind를 가진다.
- `PolicyDocumentRecord`, `ClaimDocumentRecord`가 이미 분리되어 있다.
- target existence validation을 양쪽 모두 설계할 수 있다.
- Claim이 Policy에 속하는 구조를 최소 형태로 잡을 수 있다.

단, 필드 범위는 MVP에 맞게 최소화한다.

## E. Storage File Candidate

JSON storage 파일 후보:

```text
policies.json
claims.json
```

저장 위치:

```text
%LOCALAPPDATA%\FamilyClaimRef\data\local\policies.json
%LOCALAPPDATA%\FamilyClaimRef\data\local\claims.json
```

기존 storage 패턴과 맞출 기준:

- `JsonFileEnvelope<T>`.
- `SchemaVersion`.
- `SavedAt`.
- `Items`.
- invalid JSON / schema mismatch / null items 처리 정책은 기존 JSON storage 정책과 맞춘다.
- hard delete 없음.
- disable 방식 사용.

## F. PolicyRecord Candidate

MVP 최소 후보:

```text
Id
DisplayTitle
ReferenceDate
CreatedAt
UpdatedAt
DisabledAt
```

선택 후보:

```text
Memo
SortOrder
```

제외 후보:

```text
ActualPolicyNumber
RealInsurerName
RealCustomerName
RealResidentNumber
RealPhoneNumber
RealAddress
PremiumAmount
CoverageDetails
SensitiveHealthInfo
```

설계 기준:

- 실제 보험계약 번호를 저장하지 않는다.
- 실제 보험사명 샘플을 사용하지 않는다.
- 실제 개인정보를 저장하지 않는다.
- MVP에서는 사용자가 구분할 수 있는 local display title 정도만 둔다.
- `DisabledAt != null`이면 inactive로 본다.

## G. ClaimRecord Candidate

MVP 최소 후보:

```text
Id
PolicyId
DisplayTitle
ReferenceDate
CreatedAt
UpdatedAt
DisabledAt
```

선택 후보:

```text
Memo
SortOrder
```

제외 후보:

```text
ActualClaimNumber
RealHospitalName
DiagnosisName
DiagnosisCode
ResidentNumber
PhoneNumber
Address
PaymentAmount
SensitiveHealthInfo
MedicalDetails
```

설계 기준:

- Claim은 최소한 `PolicyId`를 가질 수 있다.
- `PolicyId` existence validation은 Policy storage 구현 후 가능하다.
- 실제 병원명/진단명/진단코드/청구번호는 MVP에서 제외한다.
- `DisabledAt != null`이면 inactive로 본다.

## H. Draft Model Candidate

Policy draft 후보:

```text
PolicyDraft
- DisplayTitle
- ReferenceDate
```

Claim draft 후보:

```text
ClaimDraft
- PolicyId
- DisplayTitle
- ReferenceDate
```

기준:

- draft에는 `Id`, `CreatedAt`, `UpdatedAt`, `DisabledAt`를 넣지 않는다.
- storage service가 Id/time fields를 생성한다.
- 실제 개인정보 필드는 넣지 않는다.

## I. Storage Interface Candidate

### Candidate 1. Separate interfaces

```text
IPolicyStorageService
IClaimStorageService
```

장점:

- 책임이 분명하다.
- 각각 독립 테스트가 쉽다.

단점:

- service 수가 늘어난다.
- composition이 조금 길어진다.

### Candidate 2. Combined interface

```text
IPolicyClaimStorageService
```

장점:

- MVP에서 단순하다.
- Policy/Claim relationship validation을 한 곳에서 다루기 쉽다.

단점:

- interface가 커질 수 있다.
- 나중에 분리 필요성이 생길 수 있다.

### Candidate Recommendation

후보 권장안:

```text
IPolicyClaimStorageService
```

MVP에서는 combined interface를 사용한다.

이유:

- Policy와 Claim은 함께 target selection / target validation 문제를 해결한다.
- Claim은 PolicyId를 가질 수 있어 두 entity 관계 확인이 필요하다.
- 현재 프로젝트 규모에서는 별도 DI container 없이 manual composition이므로 combined service가 단순하다.

## J. Interface Method Candidate

`IPolicyClaimStorageService` method 후보:

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

- `GetPoliciesAsync` / `GetClaimsAsync`는 active records만 반환할지, all records를 반환할지 결정 필요.
- 기존 document storage와 맞추려면 active-only 기본 반환 후보를 검토한다.
- disabled record도 조회할 필요가 있으면 별도 method 후보로 둔다.
- hard delete는 제외한다.

## K. Target Existence Validation Owner Candidate

### Candidate 1. ViewModel validation

내용:

- ViewModel이 selected target list를 보고 검증한다.

장점:

- UI feedback이 빠르다.

단점:

- ViewModel 우회 시 invalid link 가능.
- application layer validation이 약하다.

### Candidate 2. DocumentRegistrationWorkflow validation

내용:

- registration workflow가 Policy/Claim storage를 통해 target existence를 검증한다.

장점:

- import + link combined flow에서 유효하다.

단점:

- `DocumentLinkCoordinator`를 직접 쓰는 경로에서는 validation이 빠질 수 있다.

### Candidate 3. DocumentLinkCoordinator validation

내용:

- `DocumentLinkCoordinator`가 Policy/Claim storage를 통해 target existence를 검증한다.

장점:

- link 생성의 canonical boundary에서 검증된다.
- workflow가 이 coordinator를 쓰므로 combined flow에도 적용된다.
- direct link workflow에도 적용된다.

단점:

- `DocumentLinkCoordinator` dependency가 늘어난다.
- 기존 tests 수정/확장이 필요하다.

### Candidate Recommendation

후보 권장안:

```text
Candidate 3. DocumentLinkCoordinator validation
```

기준:

- PolicyDocument / ClaimDocument link 생성 시 target existence는 link boundary에서 검증한다.
- ViewModel은 UI pre-validation만 수행한다.
- storage/service layer가 canonical validation owner가 된다.
- existing tests는 후속 구현 단계에서 보강한다.

## L. AppServices / Composition Impact

후속 구현에서 추가될 수 있는 것:

```text
IPolicyClaimStorageService -> JsonPolicyClaimStorageService(metadataRoot)
DocumentLinkCoordinator(..., policyClaimStorageService)
DocumentRegistrationWorkflow(... existing dependencies ...)
DocumentRegistrationViewModel(... existing dependencies ...)
```

주의:

- ViewModel이 root path를 알지 않음.
- MainWindow가 storage service를 생성하지 않음.
- AppServices가 너무 커지지 않도록 후속 관리 필요.
- DI container는 계속 사용하지 않는다.

## M. ViewModel / UI Impact

현재:

- `TargetKind`.
- `TargetId` manual input.

후속 후보:

- target list 조회.
- policy selection ComboBox.
- claim selection ComboBox.
- Policy/Claim create minimal UI.
- TargetId manual input 제거 또는 debug-only로 축소.

이번 scope에서는 UI 수정 없음.

## N. Test Scope Candidate

### Storage tests

- add policy succeeds.
- get policies returns active policies.
- disable policy hides from active list.
- add claim succeeds with active policy.
- add claim rejects missing policy.
- disable claim hides from active list.
- invalid JSON rejected.
- schema mismatch rejected.
- null items rejected.
- temp directory only.

### Link validation tests

- link policy document rejects missing policy.
- link claim document rejects missing claim.
- link policy document accepts active policy.
- link claim document accepts active claim.
- disabled policy rejected.
- disabled claim rejected.
- same duplicate link policy remains enforced.
- same duplicate link claim remains enforced.

### Workflow tests

- register policy document rejects missing policy before final success.
- register claim document rejects missing claim before final success.
- rollback behavior still works when link validation fails.
- no project root files.
- temp directory only.

## O. Out of Scope

이번 문서에서 제외할 범위:

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

## P. Needs Decision

사용자 결정 질문:

1. Policy/Claim storage 설계를 진행할 것인가?
2. Policy와 Claim storage를 함께 설계할 것인가?
3. JSON files는 `policies.json`, `claims.json`으로 둘 것인가?
4. `JsonFileEnvelope<T>` 패턴을 재사용할 것인가?
5. hard delete 없이 `DisabledAt` 기반 disable을 사용할 것인가?
6. `PolicyRecord`는 `Id`, `DisplayTitle`, `ReferenceDate`, `CreatedAt`, `UpdatedAt`, `DisabledAt` 최소 필드로 둘 것인가?
7. `ClaimRecord`는 `Id`, `PolicyId`, `DisplayTitle`, `ReferenceDate`, `CreatedAt`, `UpdatedAt`, `DisabledAt` 최소 필드로 둘 것인가?
8. 실제 보험계약 번호, 보험사명, 병원명, 진단명/진단코드는 MVP storage에서 제외할 것인가?
9. `IPolicyClaimStorageService` combined interface를 사용할 것인가?
10. `ClaimRecord.PolicyId`는 active policy existence validation을 요구할 것인가?
11. Policy/Claim document link target existence validation owner는 `DocumentLinkCoordinator`로 둘 것인가?
12. ViewModel은 UI pre-validation만 담당하고 canonical target validation은 service/coordinator에 둘 것인가?
13. AppServices에 `JsonPolicyClaimStorageService` composition을 추가하는 방향으로 둘 것인가?
14. 후속 구현 시 storage tests, link validation tests, workflow rollback tests를 추가할 것인가?
15. Policy/Claim selection UI는 storage 구현 이후 별도 설계로 둘 것인가?

## Q. Recommendation

다음 순서를 추천한다.

1. 이 문서를 기준으로 사용자 결정을 받는다.
2. 사용자 결정 후 `docs/119_POLICY_CLAIM_STORAGE_USER_DECISION_RECORD.md`를 생성한다.
3. 그 다음 별도 승인으로 Policy/Claim storage model/interface/service/tests 구현 범위를 정한다.
4. 이후 `DocumentLinkCoordinator` target existence validation을 구현한다.
5. 마지막으로 MainWindow target selection UI를 manual input에서 selection 기반으로 바꾼다.

## R. Result

```text
POLICY_CLAIM_STORAGE_SCOPE_DESIGN_DRAFTED
```
