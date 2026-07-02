# Policy/Claim Document Link Workflow Scope Design

## A. Goal

이 문서는 Policy/Claim document link workflow 범위 결정 설계 문서다.

목적은 다음과 같다.

- 저장된 `DocumentRecord`를 `PolicyDocumentRecord` 또는 `ClaimDocumentRecord`로 연결하는 책임을 검토한다.
- `DocumentAttachmentCoordinator`와 link workflow를 분리할지 확장할지 검토한다.
- Policy/Claim storage가 없는 상태에서 validation 범위를 정리한다.
- 중복 link 정책, disabled link 정책, 테스트 범위를 정리한다.
- 후속 구현 전 사용자 결정이 필요한 항목을 정리한다.

이 문서는 실제 구현 문서가 아니다.

- C# 구현을 수행하지 않는다.
- link coordinator 구현을 수행하지 않는다.
- request/result model을 생성하지 않는다.
- test code를 구현하지 않는다.
- UI/ViewModel을 구현하지 않는다.

## B. Current State

- JSON metadata storage 구현은 완료되었다.
- file attachment primitive 구현은 완료되었다.
- `DocumentAttachmentCoordinator` 구현은 완료되었다.
- `DocumentAttachmentCoordinator`는 file copy + `DocumentRecord` metadata 저장까지만 처리한다.
- `JsonDocumentStorageService` 구현은 완료되었다.
- `PolicyDocumentRecord`, `ClaimDocumentRecord` model 구현은 완료되었다.
- `PolicyDocumentDraft`, `ClaimDocumentDraft` model 구현은 완료되었다.
- `IDocumentStorageService`에 policy/claim document link 관련 method가 존재한다.
- `JsonDocumentStorageService`에는 policy/claim document link 저장 기능이 이미 구현되어 있다.
- `PolicyDocumentDraft.DocumentId`, `ClaimDocumentDraft.DocumentId`는 existing active `DocumentRecord.Id`여야 한다.
- disabled document 연결은 거부된다.
- documentType 검증은 `FileNamePolicyService.GetAllowedDocumentTypes(...)` 기준으로 수행된다.
- policyId/claimId 존재 검증은 Policy/Claim storage가 없어 아직 보류 상태다.
- JSON storage tests는 PASS 상태로 기록되어 있다.
- build/test는 PASS 상태로 기록되어 있다.
- 총 테스트 수는 141개다.
- link workflow는 아직 없다.
- Policy/Claim storage 구현은 아직 없다.
- WPF UI/ViewModel/file picker 연동은 아직 없다.
- actual project `attachments/`, `data/local` 내부 파일 생성은 없다.

## C. Problem Statement

현재 파일을 import하면 `DocumentRecord`만 생성된다. 실제 업무에서는 import된 문서를 특정 보험 또는 청구 건에 연결해야 한다.

핵심 문제:

- link workflow를 만들지 않으면 imported document가 미연결 상태로 남을 수 있다.
- import와 link를 한 번에 묶으면 rollback 경계가 커진다.
- Policy/Claim storage가 아직 없기 때문에 policyId/claimId 존재 검증을 완전히 할 수 없다.
- documentType은 policy/claim scope별 allowlist 기준이 다르다.
- 동일 document가 같은 policy/claim에 중복 연결되는 것을 허용할지 결정해야 한다.
- disabled document, disabled link, re-link 정책을 정해야 한다.
- UI/ViewModel이 link validation과 storage detail을 직접 알면 안 된다.

## D. Existing Service Boundary

### `DocumentAttachmentCoordinator`

담당:

- source file validation
- physical file name 생성
- duplicateIndex 자동 산정
- file copy
- `DocumentRecord` metadata 저장
- metadata 저장 실패 시 file cleanup

담당하지 않음:

- `PolicyDocumentRecord` 저장
- `ClaimDocumentRecord` 저장
- policyId/claimId 존재 검증
- Policy/Claim case 생성
- UI 상태 관리

### `JsonDocumentStorageService`

담당:

- `DocumentRecord` 저장
- `PolicyDocumentRecord` 저장
- `ClaimDocumentRecord` 저장
- active documentId validation
- documentType allowlist validation
- disabled document 연결 거부
- policy/claim document link disable

담당하지 않음:

- file copy
- user workflow orchestration
- Policy/Claim storage 존재 검증
- duplicate link policy orchestration
- UI 상태 관리

### 후속 link workflow 후보

담당 후보:

- existing documentId 입력 수신
- target kind 입력 수신
  - policy
  - claim
- target id 입력 수신
  - policyId
  - claimId
- documentType 입력 수신
- duplicate active link 검증 후보
- `AddPolicyDocumentAsync(...)` 또는 `AddClaimDocumentAsync(...)` 호출
- 성공 시 link result 반환
- 실패 시 exception 또는 failure result 반환

## E. Candidate Options

### Candidate 1. UI/ViewModel이 link method를 직접 호출

흐름:

1. UI/ViewModel이 `IDocumentStorageService.AddPolicyDocumentAsync(...)` 또는 `AddClaimDocumentAsync(...)`를 직접 호출한다.
2. validation failure를 UI에서 처리한다.

장점:

- 새 service가 필요 없다.
- 구현량이 작다.

단점:

- ViewModel이 policy/claim link rule을 알게 된다.
- link validation과 UI state가 섞인다.
- 테스트가 UI로 밀릴 수 있다.
- 장기 유지보수에 불리하다.

### Candidate 2. `DocumentAttachmentCoordinator`에 link method 추가

흐름:

1. 기존 coordinator에 `AttachDocumentToPolicyAsync(...)`, `AttachDocumentToClaimAsync(...)` 같은 method를 추가한다.
2. file import와 link 책임을 같은 coordinator에 둔다.

장점:

- attachment 관련 workflow가 한 class에 모인다.
- 호출부가 단순할 수 있다.

단점:

- coordinator가 커진다.
- file import 책임과 domain link 책임이 섞일 수 있다.
- Policy/Claim storage 도입 시 변경 범위가 커질 수 있다.

### Candidate 3. 별도 `DocumentLinkCoordinator` 도입

흐름:

1. `DocumentAttachmentCoordinator`는 file import + Document metadata 저장만 담당한다.
2. `DocumentLinkCoordinator`가 existing Document를 Policy/Claim에 연결한다.
3. UI/ViewModel은 import 후 link를 순차적으로 호출하거나, 후속 상위 workflow가 둘을 조합한다.

장점:

- 책임 분리가 명확하다.
- link workflow만 독립 테스트 가능하다.
- Policy/Claim storage 도입 시 확장하기 쉽다.
- `DocumentAttachmentCoordinator`가 비대해지지 않는다.

단점:

- abstraction이 하나 더 생긴다.
- import + link를 한 번에 처리하는 사용자 흐름은 후속 orchestration이 필요하다.

### Candidate 4. `DocumentRegistrationWorkflow`로 import + link까지 통합

흐름:

1. source file import
2. Document metadata 저장
3. Policy/Claim link 저장
4. 실패 시 file cleanup, document disable, link rollback 등을 한 번에 처리

장점:

- 실제 사용자 작업 단위에 가장 가깝다.
- 한 번의 호출로 import + link 완료 가능하다.

단점:

- rollback 경계가 가장 크다.
- 아직 Policy/Claim storage가 없어 validation이 불완전하다.
- 구현량과 테스트 범위가 크다.
- MVP 1차에서는 과도할 수 있다.

## F. Recommended Direction

Candidate Recommendation:

- MVP 1차는 Candidate 3, 별도 `DocumentLinkCoordinator` 도입을 추천한다.
- `DocumentAttachmentCoordinator`는 기존처럼 Document metadata 저장까지만 담당한다.
- `DocumentLinkCoordinator`는 existing active Document를 Policy/Claim target에 연결하는 application service로 둔다.
- Policy/Claim link까지 포함한 통합 import workflow는 후속으로 보류한다.
- Policy/Claim storage가 아직 없으므로 policyId/claimId 존재 검증은 보류한다.
- 이번 문서에서는 구현하지 않는다.

이 추천은 확정이 아니라 `Candidate Recommendation`이다.

## G. Request / Result Model Candidate

### Candidate A. 단일 request/result

후보 파일:

- `DocumentLinkRequest.cs`
- `DocumentLinkResult.cs`

요청 후보:

```csharp
public sealed record DocumentLinkRequest(
    string DocumentId,
    string TargetScope,
    string TargetId,
    string DocumentType);
```

기준:

- `TargetScope` 값 후보:
  - `policy`
  - `claim`
- `TargetId`는 policyId 또는 claimId다.
- documentType은 target scope 기준 allowlist로 검증한다.

결과 후보:

```csharp
public sealed record DocumentLinkResult(
    string TargetScope,
    PolicyDocumentRecord? PolicyDocument,
    ClaimDocumentRecord? ClaimDocument);
```

장점:

- 하나의 API로 policy/claim을 처리할 수 있다.

단점:

- nullable result가 생긴다.
- targetScope branching이 늘어난다.

### Candidate B. policy/claim 분리 request/result

후보 파일:

- `PolicyDocumentLinkRequest.cs`
- `ClaimDocumentLinkRequest.cs`
- `PolicyDocumentLinkResult.cs`
- `ClaimDocumentLinkResult.cs`

장점:

- type이 명확하다.
- nullable result가 줄어든다.
- 기존 `PolicyDocumentDraft`, `ClaimDocumentDraft` 구조와 맞다.

단점:

- 파일과 method가 늘어난다.
- 중복 코드가 생길 수 있다.

Candidate Recommendation:

- MVP 1차는 Candidate B를 추천한다.
- 이유는 policy/claim link record가 이미 분리되어 있고, validation scope도 다르기 때문이다.
- 단, 구현량을 줄이고 싶으면 Candidate A도 가능 후보로 기록한다.

## H. Link Coordinator Shape Candidate

후속 service 후보:

- `DocumentLinkCoordinator.cs`

method 후보:

```csharp
public Task<PolicyDocumentRecord> LinkPolicyDocumentAsync(
    PolicyDocumentLinkRequest request,
    CancellationToken cancellationToken = default);

public Task<ClaimDocumentRecord> LinkClaimDocumentAsync(
    ClaimDocumentLinkRequest request,
    CancellationToken cancellationToken = default);
```

기준:

- `IDocumentStorageService`를 생성자 주입받는다.
- `JsonDocumentStorageService` concrete type에 직접 의존하지 않는다.
- file attachment service에는 의존하지 않는다.
- `FileNamePolicyService` 직접 호출은 원칙적으로 필요 없다.
  - documentType validation은 `IDocumentStorageService` 구현체가 이미 수행한다.
  - 단, pre-validation을 넣을지 여부는 후속 결정 후보로 둔다.
- UI/ViewModel은 link coordinator를 호출하는 방향으로 둔다.

## I. Duplicate Link Policy Candidate

검토 대상:

- 같은 policyId + documentId 중복
- 같은 claimId + documentId 중복
- 같은 target + documentId + documentType 중복
- disabled link가 있을 때 재연결
- 같은 document를 다른 policy/claim에 연결

### Candidate 1. exact duplicate active link만 거부

내용:

- 같은 targetId + documentId + documentType의 active link가 있으면 거부한다.
- 같은 document를 같은 target에 다른 documentType으로 연결할 수 있다.

장점:

- 유연하다.

단점:

- 같은 문서가 같은 target에 여러 type으로 중복 연결될 수 있다.

### Candidate 2. same target + documentId active link는 documentType과 무관하게 거부

내용:

- 같은 policyId + documentId active link가 있으면 거부한다.
- 같은 claimId + documentId active link가 있으면 거부한다.
- documentType이 달라도 같은 target에는 하나만 연결 가능하다.

장점:

- 중복 표시를 줄인다.
- MVP UI가 단순하다.

단점:

- 한 문서를 여러 type으로 분류해야 하는 특수 케이스를 막는다.

### Candidate 3. duplicate validation은 storage service에 맡기고 workflow는 보류

내용:

- 현재 storage behavior를 유지한다.
- duplicate policy는 후속으로 결정한다.

장점:

- 구현이 빠르다.

단점:

- 중복 link가 생길 수 있다.

Candidate Recommendation:

- MVP 1차는 Candidate 2를 추천한다.
- 같은 target + documentId active link는 하나만 허용한다.
- disabled link는 active 중복 판단에서 제외할지 별도 결정 후보로 둔다.
- 같은 document를 다른 policy/claim에 연결하는 것은 허용 후보로 둔다.

## J. Disabled Link Policy Candidate

후보:

- disabled Document는 신규 link 불가로 유지한다.
- disabled PolicyDocument/ClaimDocument가 있을 때 같은 target/document 재연결 허용 여부는 결정이 필요하다.
- link disable은 `IDocumentStorageService.DisablePolicyDocumentAsync(...)`, `DisableClaimDocumentAsync(...)`가 이미 존재한다.
- MVP 1차에서는 active duplicate만 거부하고 disabled link는 history로 둔다.
- 재연결은 새 active link 생성 후보로 둔다.
- 기존 disabled link reactivate는 보류 후보로 둔다.

## K. PolicyId / ClaimId Validation Candidate

현재 제약:

- Policy storage 없음.
- Claim storage 없음.
- 따라서 실제 policyId/claimId 존재 검증 불가.

후보:

- null/empty/whitespace는 거부한다.
- existence validation은 보류한다.
- 후속 Policy/Claim storage 구현 후 강화한다.
- test에서는 dummy id를 사용한다.
- 실제 개인정보, 실제 보험계약 번호, 실제 청구 번호는 사용하지 않는다.

## L. Workflow Boundary Candidate

### Candidate 1. Link existing Document only

내용:

- `DocumentLinkCoordinator`는 이미 저장된 `DocumentRecord.Id`를 받아 link만 생성한다.
- file import는 처리하지 않는다.

장점:

- 책임이 명확하다.
- rollback 경계가 작다.
- 기존 `DocumentAttachmentCoordinator`와 잘 분리된다.

단점:

- 사용자 흐름상 import 후 link를 별도로 호출해야 한다.

### Candidate 2. Import + link 통합 method도 포함

내용:

- link coordinator 또는 registration workflow가 import + link를 한 번에 처리한다.

장점:

- 사용자 흐름에 가깝다.

단점:

- file cleanup, document disable, link rollback 경계가 커진다.
- Policy/Claim storage 부재 상태에서는 과도하다.

Candidate Recommendation:

- MVP 1차는 Candidate 1을 추천한다.
- import + link 통합 workflow는 후속으로 보류한다.

## M. Test Scope Candidate

후속 구현 시 포함 후보:

- link existing active document to policy succeeds
- link existing active document to claim succeeds
- missing documentId rejected
- disabled documentId rejected
- missing/empty policyId rejected
- missing/empty claimId rejected
- invalid policy documentType rejected
- invalid claim documentType rejected
- policy `capture` accepted
- claim `capture` rejected
- duplicate active policy link rejected
- duplicate active claim link rejected
- same document can link to different policyId
- same document can link to different claimId
- disabled link duplicate policy 후보에 따른 test
- temp directory only
- project root `data/local` not created
- project root `attachments` not created

후속 구현 시 제외 후보:

- file copy test
- DocumentAttachmentCoordinator test
- WPF UI/ViewModel test
- file picker test
- OCR test
- SQLite test
- Policy/Claim storage existence validation test
- import + link combined workflow test

## N. Needs Decision

사용자 결정 질문 후보:

1. Policy/Claim document link workflow를 설계할 것인가?
2. `DocumentAttachmentCoordinator`를 확장하지 않고 별도 `DocumentLinkCoordinator`를 둘 것인가?
3. MVP 1차 link workflow는 existing Document link만 처리할 것인가?
4. import + link 통합 workflow는 후속으로 보류할 것인가?
5. policy/claim request/result model은 분리할 것인가?
6. `DocumentLinkCoordinator`는 `IDocumentStorageService`만 주입받을 것인가?
7. `DocumentLinkCoordinator`는 file attachment service에 의존하지 않을 것인가?
8. `DocumentLinkCoordinator`는 `FileNamePolicyService`를 직접 호출하지 않을 것인가?
9. documentType validation은 storage service에 맡길 것인가?
10. policyId/claimId 존재 검증은 Policy/Claim storage 전까지 보류할 것인가?
11. policyId/claimId null/empty/whitespace는 거부할 것인가?
12. same target + documentId active duplicate는 documentType과 무관하게 거부할 것인가?
13. disabled link는 active duplicate 판단에서 제외할 것인가?
14. same document를 다른 policyId/claimId에 연결하는 것은 허용할 것인가?
15. disabled Document는 신규 link 불가로 유지할 것인가?
16. 후속 구현 시 `DocumentLinkCoordinatorTests.cs`를 추가할 것인가?
17. test는 temp directory만 사용할 것인가?
18. actual project `attachments/`, `data/local` 파일 생성은 금지할 것인가?
19. WPF UI/ViewModel 연동은 link workflow 구현 후로 보류할 것인가?

## O. Out of Scope

이번 문서에서 제외하는 범위:

- C# 구현 없음
- link coordinator 구현 없음
- request/result model 생성 없음
- test code 구현 없음
- test file 생성 없음
- DocumentAttachmentCoordinator 수정 없음
- JSON metadata storage 수정 없음
- file attachment service 수정 없음
- FileNamePolicyService 수정 없음
- Policy/Claim storage 구현 없음
- import + link combined workflow 구현 없음
- WPF UI/XAML/navigation/ViewModel 구현 없음
- file picker 구현 없음
- OCR 구현 없음
- SQLite DB/package 추가 없음
- repository/data access/migration 구현 없음
- project root `attachments/` 내부 파일 생성 없음
- project root `data/local` 내부 파일 생성 없음
- 실제 개인정보 샘플 없음

## P. Risks

- Policy/Claim storage가 없어 target id 존재 검증은 아직 불완전하다.
- import + link가 분리되면 imported but unlinked document가 생길 수 있다.
- duplicate link 정책이 지나치게 엄격하면 실제 재사용 시나리오를 막을 수 있다.
- duplicate link 정책이 느슨하면 UI에 중복 문서가 나타날 수 있다.
- link rollback은 import + link 통합 workflow 전까지 완전하지 않다.
- disabled link 재연결 정책이 모호하면 history와 active 상태가 섞일 수 있다.
- UI 연동 전까지 사용자 흐름 전체는 검증되지 않는다.

## Q. Recommendation

1. 이 문서를 기준으로 Policy/Claim document link workflow 결정을 받는다.
2. 사용자 결정 후 `docs/95_POLICY_CLAIM_DOCUMENT_LINK_WORKFLOW_USER_DECISION_RECORD.md`를 생성한다.
3. 그 다음 별도 승인으로 `DocumentLinkCoordinator` / request / result / test를 구현한다.
4. 구현 후 `dotnet build FamilyClaimRef.sln`, `dotnet test FamilyClaimRef.sln`을 실행한다.
5. 구현 후 `docs/96_POLICY_CLAIM_DOCUMENT_LINK_WORKFLOW_IMPLEMENTATION_REVIEW.md`를 생성한다.
6. import + link 통합 workflow와 WPF UI/ViewModel 연동은 이후로 보류한다.

## R. Result

`POLICY_CLAIM_DOCUMENT_LINK_WORKFLOW_SCOPE_DESIGN_DRAFTED`
