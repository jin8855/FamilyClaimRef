# Import + Link Combined Workflow Scope Design

## A. Goal

이 문서는 import + link combined workflow 범위 결정 설계 문서다.

목적은 파일 import, `DocumentRecord` metadata 저장, Policy/Claim link 저장을 하나의 사용자 작업 단위로 묶을지 검토하는 것이다.

이 문서에서는 다음 항목을 검토한다.

- 현재 분리 구현 상태.
- existing coordinator 책임 경계.
- import + link combined workflow 후보.
- rollback/cleanup/disable 정책 후보.
- 후속 구현 시 request/result model 후보.
- 후속 테스트 범위 후보.
- 사용자 결정이 필요한 항목.

이 문서는 실제 구현 문서가 아니다.

- C# 구현을 하지 않는다.
- workflow 구현을 하지 않는다.
- request/result model을 생성하지 않는다.
- test code를 작성하지 않는다.
- WPF UI/ViewModel/file picker를 구현하지 않는다.

## B. Current State

현재 상태:

- JSON metadata storage 구현 완료.
- actual file attachment primitive 구현 완료.
- `DocumentAttachmentCoordinator` 구현 완료.
- `DocumentLinkCoordinator` 구현 완료.
- `dotnet build FamilyClaimRef.sln` PASS 기록 있음.
- `dotnet test FamilyClaimRef.sln` PASS 기록 있음.
- 총 테스트 수는 179개 PASS 기록 있음.
- `DocumentAttachmentCoordinator`는 file copy + `DocumentRecord` metadata 저장까지만 처리한다.
- `DocumentLinkCoordinator`는 existing `DocumentRecord`를 policy/claim target에 연결한다.
- import + link combined workflow는 아직 없다.
- Policy/Claim storage가 없어 policyId/claimId 존재 검증은 아직 없다.
- WPF UI/ViewModel/file picker 연동은 아직 없다.
- project root `attachments/`, `data/local` 내부 파일 생성은 없다.

## C. Problem Statement

현재 실제 사용자 흐름은 두 단계로 나뉜다.

1. file import + `DocumentRecord` 저장.
2. existing `DocumentRecord`를 policy/claim target에 link.

이 구조는 책임 분리가 명확하지만 다음 문제가 남는다.

- imported but unlinked document가 생길 수 있다.
- UI/ViewModel이 두 coordinator 호출 순서와 실패 처리를 직접 조합해야 한다.
- import 성공 후 link 실패 시 copied file과 document metadata를 어떻게 처리할지 정책이 필요하다.
- link 성공 이후 후속 오류가 생기면 rollback 범위가 커진다.
- Policy/Claim storage가 없으므로 target id 존재 검증은 아직 완전하지 않다.
- UI/ViewModel에서 workflow와 rollback 규칙을 직접 들고 있으면 UI 책임이 커진다.

## D. Existing Boundary

### `DocumentAttachmentCoordinator`

담당:

- source file validation.
- physical file name 생성.
- duplicateIndex 자동 산정.
- file copy.
- `DocumentRecord` metadata 저장.
- metadata 저장 실패 시 copied file cleanup.

담당하지 않음:

- policy/claim link 저장.
- policyId/claimId existence validation.
- UI state 관리.
- import + link combined workflow.

### `DocumentLinkCoordinator`

담당:

- existing active document link.
- policy/claim link request/result 분리.
- active duplicate link 거부.
- storage service를 통한 documentType validation.
- disabled Document link 거부 유지.

담당하지 않음:

- file copy.
- file cleanup.
- import workflow.
- Policy/Claim storage existence validation.
- UI state 관리.

## E. Candidate Options

### Candidate 1. 현재처럼 import와 link를 분리 유지

내용:

- UI/ViewModel 또는 상위 workflow가 `DocumentAttachmentCoordinator`와 `DocumentLinkCoordinator`를 순차 호출한다.
- 별도 combined workflow를 만들지 않는다.

장점:

- 기존 구현을 그대로 유지한다.
- rollback 경계가 작다.
- 각 coordinator 책임이 명확하다.

단점:

- imported but unlinked document가 생길 수 있다.
- UI/ViewModel이 두 단계를 조합해야 한다.
- 사용자 작업 단위 테스트가 부족하다.

### Candidate 2. 별도 `DocumentRegistrationWorkflow` 도입

내용:

- `DocumentRegistrationWorkflow`가 import coordinator와 link coordinator를 조합한다.
- policy/claim별 registration method를 제공한다.
- import 성공 후 link 실패 시 rollback/disable/cleanup 정책을 수행한다.

장점:

- 사용자 작업 단위가 한곳에 모인다.
- UI/ViewModel 호출이 단순해진다.
- rollback/cleanup 테스트가 가능하다.
- 기존 coordinator 책임을 유지할 수 있다.

단점:

- abstraction이 하나 더 생긴다.
- rollback 정책이 복잡하다.
- Policy/Claim storage 부재로 target existence validation은 여전히 보류된다.

### Candidate 3. `DocumentAttachmentCoordinator`에 link까지 추가

내용:

- 기존 attachment coordinator가 import + link를 모두 처리한다.

장점:

- 호출부는 단순하다.
- 파일 import 중심 흐름으로 보기 쉽다.

단점:

- attachment coordinator가 비대해진다.
- file import와 domain link 책임이 섞인다.
- 기존 책임 분리 방향과 충돌한다.

### Candidate 4. `DocumentLinkCoordinator`에 import까지 추가

내용:

- link coordinator가 file import를 호출한다.

장점:

- policy/claim link 중심 흐름으로 만들 수 있다.

단점:

- link coordinator가 file storage를 알게 된다.
- existing document link 책임과 import 책임이 섞인다.
- file cleanup 책임이 모호해진다.

## F. Recommended Direction

Candidate Recommendation:

- Candidate 2, 별도 `DocumentRegistrationWorkflow` 도입을 추천한다.
- 기존 `DocumentAttachmentCoordinator`와 `DocumentLinkCoordinator`는 수정하지 않는 방향을 우선한다.
- `DocumentRegistrationWorkflow`가 두 coordinator를 조합한다.
- MVP 1차는 policy/claim 각각의 registration method를 분리한다.
- import + link rollback은 workflow 책임으로 둔다.
- Policy/Claim storage 존재 검증은 계속 보류한다.
- 이 문서에서는 구현하지 않는다.

주의:

- 이 추천은 확정 결정이 아니다.
- 후속 사용자 결정 문서에서 Accepted 여부를 별도로 기록해야 한다.

## G. Request / Result Model Candidate

### Policy registration

후보 파일:

- `PolicyDocumentRegistrationRequest.cs`
- `PolicyDocumentRegistrationResult.cs`

요청 후보:

```csharp
public sealed record PolicyDocumentRegistrationRequest(
    string SourceFilePath,
    string PolicyId,
    string DocumentType,
    string DisplayTitle,
    DateOnly ReferenceDate);
```

결과 후보:

```csharp
public sealed record PolicyDocumentRegistrationResult(
    DocumentAttachmentResult Attachment,
    PolicyDocumentLinkResult Link);
```

### Claim registration

후보 파일:

- `ClaimDocumentRegistrationRequest.cs`
- `ClaimDocumentRegistrationResult.cs`

요청 후보:

```csharp
public sealed record ClaimDocumentRegistrationRequest(
    string SourceFilePath,
    string ClaimId,
    string DocumentType,
    string DisplayTitle,
    DateOnly ReferenceDate);
```

결과 후보:

```csharp
public sealed record ClaimDocumentRegistrationResult(
    DocumentAttachmentResult Attachment,
    ClaimDocumentLinkResult Link);
```

주의:

- absolute path는 result에 포함하지 않는다.
- original file name은 저장하지 않는다.
- Policy/Claim storage existence validation은 아직 하지 않는다.

## H. Workflow Shape Candidate

후속 service 후보:

- `DocumentRegistrationWorkflow.cs`

method 후보:

```csharp
public Task<PolicyDocumentRegistrationResult> RegisterPolicyDocumentAsync(
    PolicyDocumentRegistrationRequest request,
    CancellationToken cancellationToken = default);

public Task<ClaimDocumentRegistrationResult> RegisterClaimDocumentAsync(
    ClaimDocumentRegistrationRequest request,
    CancellationToken cancellationToken = default);
```

constructor 후보:

```csharp
public DocumentRegistrationWorkflow(
    DocumentAttachmentCoordinator attachmentCoordinator,
    DocumentLinkCoordinator linkCoordinator,
    IDocumentStorageService documentStorageService,
    IFileAttachmentService fileAttachmentService);
```

검토 필요:

- rollback에서 document disable과 copied file cleanup이 필요할 수 있으므로 `IDocumentStorageService`, `IFileAttachmentService`를 직접 받을지 결정해야 한다.
- 또는 rollback primitive를 기존 coordinator가 제공하도록 할지 검토할 수 있다.
- 현재 기준에서는 기존 coordinator를 수정하지 않는 방향이 우선이다.

## I. Rollback Policy Candidate

### 성공 흐름

1. request validation.
2. `DocumentAttachmentCoordinator.AttachDocumentAsync(...)` 호출.
3. 성공 시 `DocumentRecord`와 copied file 생성.
4. `DocumentLinkCoordinator.LinkPolicyDocumentAsync(...)` 또는 `LinkClaimDocumentAsync(...)` 호출.
5. link 성공 시 registration result 반환.

### link 실패 시 rollback 후보

link 실패 전에 이미 생성된 것:

- copied file.
- `DocumentRecord` metadata.

rollback 후보:

1. copied file delete 시도.
2. `DocumentRecord` disable 시도.
3. link 실패 원인 노출.
4. file cleanup 실패 또는 document disable 실패도 함께 노출.

정책 후보:

- MVP에서는 link 실패 시 copied file delete + document disable을 시도한다.
- document metadata는 hard delete하지 않는다.
- cleanup/disable 실패를 숨기지 않는다.
- custom exception은 만들지 않는다.
- 필요 시 `AggregateException`으로 link failure + rollback failure를 함께 노출한다.

## J. Pre-validation Candidate

link 실패를 줄이기 위한 사전 검증 후보:

- policyId/claimId required validation.
- documentType validation은 attachment coordinator와 storage service에서 이미 수행.
- target existence validation은 Policy/Claim storage 전까지 보류.
- duplicate active link validation은 새 documentId 생성 전에는 제한적이다.
- source file existence/extension validation은 attachment coordinator에서 수행.

주의:

- import 전에는 documentId가 생성되지 않으므로 same target + documentId duplicate은 사실상 발생하지 않는다.
- link 실패는 주로 policyId/claimId required, documentType validation, storage failure, IO failure에서 발생할 수 있다.

## K. Test Scope Candidate

후속 구현 시 포함 후보:

- register policy document succeeds.
- register claim document succeeds.
- result contains attachment result and link result.
- copied file exists under temp attachment root.
- document metadata exists under temp metadata root.
- policy link exists under temp metadata root.
- claim link exists under temp metadata root.
- link failure cleans up copied file.
- link failure disables created `DocumentRecord`.
- cleanup failure is reported.
- disable failure is reported.
- source missing fails before link.
- invalid policy documentType fails before file copy or before link.
- invalid claim documentType fails before file copy or before link.
- missing policyId rejected.
- missing claimId rejected.
- temp directory only.
- project root `attachments/`, `data/local` not created.

제외 후보:

- WPF file picker test.
- UI/ViewModel test.
- OCR test.
- SQLite test.
- Policy/Claim storage existence validation test.
- concurrent import + link test.
- production path permission test.

## L. Needs Decision

사용자 결정 질문 후보:

1. import + link combined workflow를 설계할 것인가?
2. 기존 `DocumentAttachmentCoordinator`와 `DocumentLinkCoordinator`를 수정하지 않을 것인가?
3. 별도 `DocumentRegistrationWorkflow`를 도입할 것인가?
4. policy/claim registration request/result model을 분리할 것인가?
5. workflow가 attachment coordinator와 link coordinator를 조합할 것인가?
6. link 실패 시 copied file delete를 시도할 것인가?
7. link 실패 시 created `DocumentRecord`를 disable할 것인가?
8. rollback 실패를 별도 failure로 노출할 것인가?
9. MVP에서는 custom exception 없이 기존 exception/`AggregateException`을 사용할 것인가?
10. Policy/Claim storage existence validation은 계속 보류할 것인가?
11. import + link workflow는 PolicyDocument/ClaimDocument link까지만 처리하고 Policy/Claim entity 생성은 하지 않을 것인가?
12. 후속 구현 시 `DocumentRegistrationWorkflowTests.cs`를 추가할 것인가?
13. test는 temp directory만 사용할 것인가?
14. actual project `attachments/`, `data/local` 파일 생성은 금지할 것인가?
15. WPF UI/ViewModel/file picker 연동은 workflow 구현 후로 보류할 것인가?

## M. Out of Scope

이 문서에서 제외하는 범위:

- C# 구현 없음.
- workflow 구현 없음.
- request/result model 생성 없음.
- test code 구현 없음.
- test file 생성 없음.
- existing coordinator 수정 없음.
- JSON metadata storage 수정 없음.
- file attachment service 수정 없음.
- `FileNamePolicyService` 수정 없음.
- Policy/Claim storage 구현 없음.
- WPF UI/XAML/navigation/ViewModel 구현 없음.
- file picker 구현 없음.
- OCR 구현 없음.
- SQLite DB/package 추가 없음.
- repository/data access/migration 구현 없음.
- project root `attachments/` 내부 파일 생성 없음.
- project root `data/local` 내부 파일 생성 없음.
- 실제 개인정보 샘플 없음.

## N. Risks

남은 위험:

- rollback 중 copied file cleanup 실패 가능성.
- rollback 중 `DocumentRecord` disable 실패 가능성.
- link 실패 후 document를 disable하면 history가 남는다.
- hard delete가 없으므로 완전한 rollback은 아니다.
- Policy/Claim storage가 없어 target id 존재 검증은 아직 없다.
- import + link workflow가 추가되면 application service 계층이 생긴다.
- custom exception이 없어 UI error 분류가 거칠 수 있다.
- UI 연동 전까지 사용자 흐름 전체는 아직 검증되지 않는다.

## O. Recommendation

추천 순서:

1. 이 문서를 기준으로 import + link combined workflow 사용자 결정을 받는다.
2. 사용자 결정 후 `docs/98_IMPORT_LINK_COMBINED_WORKFLOW_USER_DECISION_RECORD.md`를 생성한다.
3. 그 다음 별도 승인으로 `DocumentRegistrationWorkflow` / request / result / test를 구현한다.
4. 구현 후 `dotnet build FamilyClaimRef.sln`, `dotnet test FamilyClaimRef.sln`을 실행한다.
5. 구현 후 `docs/99_IMPORT_LINK_COMBINED_WORKFLOW_IMPLEMENTATION_REVIEW.md`를 생성한다.
6. WPF UI/ViewModel/file picker 연동은 이후로 보류한다.

## P. Result

`IMPORT_LINK_COMBINED_WORKFLOW_SCOPE_DESIGN_DRAFTED`
