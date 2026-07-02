# Import + Link Combined Workflow User Decision Record

## A. Goal

이 문서는 `docs/97_IMPORT_LINK_COMBINED_WORKFLOW_SCOPE_DESIGN.md`의 사용자 결정 기록이다.

목적은 import + link combined workflow의 책임 범위와 후속 구현 방향을 확정하는 것이다.

이 문서는 구현 문서가 아니다.

- C# 구현을 하지 않는다.
- workflow 구현을 하지 않는다.
- request/result model을 생성하지 않는다.
- test code를 구현하지 않는다.
- WPF UI/ViewModel/file picker를 구현하지 않는다.

## B. Checked Files / Paths

| Path | Purpose | Result |
|---|---|---|
| `docs/97_IMPORT_LINK_COMBINED_WORKFLOW_SCOPE_DESIGN.md` | Needs Decision Q1~Q15 기준 확인 | Checked |
| `docs/96_POLICY_CLAIM_DOCUMENT_LINK_WORKFLOW_IMPLEMENTATION_REVIEW.md` | link workflow 구현 결과 확인 | Checked |
| `docs/93_DOCUMENT_ATTACHMENT_COORDINATOR_IMPLEMENTATION_REVIEW.md` | attachment coordinator 구현 결과 확인 | Checked |
| `app/FamilyClaimRef.App/Services/Storage/DocumentAttachmentCoordinator.cs` | import coordinator 책임 경계 확인 | Checked |
| `app/FamilyClaimRef.App/Services/Storage/DocumentAttachmentRequest.cs` | import request 구조 확인 | Checked |
| `app/FamilyClaimRef.App/Services/Storage/DocumentAttachmentResult.cs` | import result 구조 확인 | Checked |
| `app/FamilyClaimRef.App/Services/Storage/DocumentLinkCoordinator.cs` | link coordinator 책임 경계 확인 | Checked |
| `app/FamilyClaimRef.App/Services/Storage/PolicyDocumentLinkRequest.cs` | policy link request 구조 확인 | Checked |
| `app/FamilyClaimRef.App/Services/Storage/ClaimDocumentLinkRequest.cs` | claim link request 구조 확인 | Checked |
| `app/FamilyClaimRef.App/Services/Storage/PolicyDocumentLinkResult.cs` | policy link result 구조 확인 | Checked |
| `app/FamilyClaimRef.App/Services/Storage/ClaimDocumentLinkResult.cs` | claim link result 구조 확인 | Checked |
| `app/FamilyClaimRef.App/Services/Storage/IDocumentStorageService.cs` | metadata rollback 가능성 확인 | Checked |
| `app/FamilyClaimRef.App/Services/Storage/JsonDocumentStorageService.cs` | document/link validation owner 확인 | Checked |
| `app/FamilyClaimRef.App/Services/Storage/IFileAttachmentService.cs` | copied file cleanup 가능성 확인 | Checked |
| `app/FamilyClaimRef.App/Services/Storage/LocalFileAttachmentService.cs` | local file attachment implementation 확인 | Checked |
| `FamilyClaimRef.sln` | 후속 build/test 대상 확인 | Checked |

## C. User Decision Summary

| ID | Question | Decision | Recorded Direction |
|---|---|---|---|
| Q1 | import + link combined workflow를 설계할 것인가? | Accepted | file import, `DocumentRecord` metadata 저장, Policy/Claim link 저장을 하나의 사용자 작업 단위로 묶는 workflow를 후속 구현 대상으로 둔다. |
| Q2 | 기존 `DocumentAttachmentCoordinator`와 `DocumentLinkCoordinator`를 수정하지 않을 것인가? | Accepted | 기존 두 coordinator의 책임 경계를 유지하고, combined workflow는 별도 application workflow에서 조합한다. |
| Q3 | 별도 `DocumentRegistrationWorkflow`를 도입할 것인가? | Accepted | `DocumentRegistrationWorkflow`를 도입하여 attachment coordinator와 link coordinator를 조합한다. |
| Q4 | policy/claim registration request/result model을 분리할 것인가? | Accepted | policy와 claim의 target id 및 link result type이 다르므로 registration request/result model을 분리한다. |
| Q5 | workflow가 attachment coordinator와 link coordinator를 조합할 것인가? | Accepted | file import와 link 책임은 기존 coordinator에 위임하고, workflow는 사용자 작업 단위와 rollback/cleanup 책임을 맡는다. |
| Q6 | link 실패 시 copied file delete를 시도할 것인가? | Accepted | link 실패 시 copied file orphan을 줄이기 위해 `IFileAttachmentService.DeleteDocumentFileIfExistsAsync(...)`를 호출한다. |
| Q7 | link 실패 시 created `DocumentRecord`를 disable할 것인가? | Accepted | hard delete 없이 `DisabledAt` 기반 soft-disable 정책을 유지한다. |
| Q8 | rollback 실패를 별도 failure로 노출할 것인가? | Accepted | link failure, copied file cleanup failure, document disable failure가 모두 추적 가능해야 한다. |
| Q9 | MVP에서는 custom exception 없이 기존 exception/`AggregateException`을 사용할 것인가? | Accepted | MVP 1차에서는 custom exception을 만들지 않고 .NET 기본 exception과 `AggregateException`을 사용한다. |
| Q10 | Policy/Claim storage existence validation은 계속 보류할 것인가? | Accepted - Deferred | Policy/Claim storage가 아직 없으므로 target existence validation은 후속 storage 구현 이후 강화한다. |
| Q11 | import + link workflow는 PolicyDocument/ClaimDocument link까지만 처리하고 Policy/Claim entity 생성은 하지 않을 것인가? | Accepted | workflow는 `PolicyDocumentRecord` 또는 `ClaimDocumentRecord` link 저장까지만 처리한다. |
| Q12 | 후속 구현 시 `DocumentRegistrationWorkflowTests.cs`를 추가할 것인가? | Accepted | 후속 구현 시 `tests/FamilyClaimRef.App.Tests/DocumentRegistrationWorkflowTests.cs`를 추가한다. |
| Q13 | test는 temp directory만 사용할 것인가? | Accepted | source file, copied file, metadata JSON file은 모두 temp directory 안에서만 생성한다. |
| Q14 | actual project `attachments/`, `data/local` 파일 생성은 금지할 것인가? | Accepted | 후속 test 구현에서도 project root `attachments/`, `data/local` 파일 생성은 금지한다. |
| Q15 | WPF UI/ViewModel/file picker 연동은 workflow 구현 후로 보류할 것인가? | Accepted | workflow 구현과 검증 이후 WPF UI/ViewModel/file picker 연동을 검토한다. |

## D. Accepted Combined Workflow Direction

확정된 방향:

- import + link combined workflow를 진행한다.
- 기존 `DocumentAttachmentCoordinator`와 `DocumentLinkCoordinator`는 수정하지 않는다.
- 별도 `DocumentRegistrationWorkflow`를 도입한다.
- policy/claim registration request/result model을 분리한다.
- workflow가 attachment coordinator와 link coordinator를 조합한다.
- link 실패 시 copied file delete를 시도한다.
- link 실패 시 created `DocumentRecord`를 disable한다.
- rollback 실패는 별도 failure로 노출한다.
- custom exception 없이 기존 exception/`AggregateException`을 사용한다.
- Policy/Claim storage existence validation은 계속 보류한다.
- workflow는 PolicyDocument/ClaimDocument link까지만 처리한다.
- Policy/Claim entity 생성은 하지 않는다.
- 후속 구현 시 `DocumentRegistrationWorkflowTests.cs`를 추가한다.
- test는 temp directory만 사용한다.
- actual project `attachments/`, `data/local` 파일 생성은 금지한다.
- WPF UI/ViewModel/file picker 연동은 보류한다.

## E. Implementation Candidate Files

후속 구현 후보 파일:

- `app/FamilyClaimRef.App/Services/Storage/PolicyDocumentRegistrationRequest.cs`
- `app/FamilyClaimRef.App/Services/Storage/PolicyDocumentRegistrationResult.cs`
- `app/FamilyClaimRef.App/Services/Storage/ClaimDocumentRegistrationRequest.cs`
- `app/FamilyClaimRef.App/Services/Storage/ClaimDocumentRegistrationResult.cs`
- `app/FamilyClaimRef.App/Services/Storage/DocumentRegistrationWorkflow.cs`
- `tests/FamilyClaimRef.App.Tests/DocumentRegistrationWorkflowTests.cs`

주의:

- 이 문서에서는 위 파일을 생성하지 않는다.
- 실제 파일 생성은 별도 구현 승인 이후 진행한다.

## F. Request / Result Shape Candidate

`PolicyDocumentRegistrationRequest` 후보:

```csharp
public sealed record PolicyDocumentRegistrationRequest(
    string SourceFilePath,
    string PolicyId,
    string DocumentType,
    string DisplayTitle,
    DateOnly ReferenceDate);
```

`PolicyDocumentRegistrationResult` 후보:

```csharp
public sealed record PolicyDocumentRegistrationResult(
    DocumentAttachmentResult Attachment,
    PolicyDocumentLinkResult Link);
```

`ClaimDocumentRegistrationRequest` 후보:

```csharp
public sealed record ClaimDocumentRegistrationRequest(
    string SourceFilePath,
    string ClaimId,
    string DocumentType,
    string DisplayTitle,
    DateOnly ReferenceDate);
```

`ClaimDocumentRegistrationResult` 후보:

```csharp
public sealed record ClaimDocumentRegistrationResult(
    DocumentAttachmentResult Attachment,
    ClaimDocumentLinkResult Link);
```

주의:

- absolute path는 result에 포함하지 않는다.
- original file name은 저장하지 않는다.
- Policy/Claim entity 생성 결과는 포함하지 않는다.

## G. Workflow Flow Candidate

### Policy registration flow

1. request validation.
2. `DocumentAttachmentCoordinator.AttachDocumentAsync(...)` 호출.
3. file copy + `DocumentRecord` metadata 저장 성공.
4. `DocumentLinkCoordinator.LinkPolicyDocumentAsync(...)` 호출.
5. policy link 성공 시 `PolicyDocumentRegistrationResult` 반환.
6. policy link 실패 시 copied file delete 시도.
7. policy link 실패 시 created `DocumentRecord` disable 시도.
8. rollback 성공 시 원래 link failure를 노출.
9. rollback 실패 시 link failure와 rollback failure를 모두 추적 가능하게 노출.

### Claim registration flow

1. request validation.
2. `DocumentAttachmentCoordinator.AttachDocumentAsync(...)` 호출.
3. file copy + `DocumentRecord` metadata 저장 성공.
4. `DocumentLinkCoordinator.LinkClaimDocumentAsync(...)` 호출.
5. claim link 성공 시 `ClaimDocumentRegistrationResult` 반환.
6. claim link 실패 시 copied file delete 시도.
7. claim link 실패 시 created `DocumentRecord` disable 시도.
8. rollback 성공 시 원래 link failure를 노출.
9. rollback 실패 시 link failure와 rollback failure를 모두 추적 가능하게 노출.

## H. Rollback Policy

후속 구현 정책:

- link 실패 시 rollback 대상:
  - copied file.
  - created `DocumentRecord`.
- copied file rollback:
  - `IFileAttachmentService.DeleteDocumentFileIfExistsAsync(...)`.
- document metadata rollback:
  - `IDocumentStorageService.DisableDocumentAsync(...)`.
- hard delete:
  - 없음.
- rollback failure:
  - 숨기지 않는다.
  - 기존 exception/`AggregateException`으로 노출한다.
- custom exception:
  - 없음.

## I. Still Not Implemented

아직 구현하지 않은 항목:

- C# 구현 없음.
- workflow 구현 없음.
- request/result model 생성 없음.
- test code 구현 없음.
- test file 생성 없음.
- existing coordinator 수정 없음.
- `DocumentAttachmentCoordinator` 수정 없음.
- `DocumentLinkCoordinator` 수정 없음.
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

## J. Next Step

다음 작업 후보:

1. 별도 승인 후 `DocumentRegistrationWorkflow` / request / result / test 구현.
2. 구현 파일 후보:
   - `PolicyDocumentRegistrationRequest.cs`
   - `PolicyDocumentRegistrationResult.cs`
   - `ClaimDocumentRegistrationRequest.cs`
   - `ClaimDocumentRegistrationResult.cs`
   - `DocumentRegistrationWorkflow.cs`
   - `DocumentRegistrationWorkflowTests.cs`
3. 구현 후 `dotnet build FamilyClaimRef.sln`, `dotnet test FamilyClaimRef.sln` 실행.
4. 구현 후 `docs/99_IMPORT_LINK_COMBINED_WORKFLOW_IMPLEMENTATION_REVIEW.md` 생성.
5. WPF UI/ViewModel/file picker 연동은 이후 보류.

## K. Result

`IMPORT_LINK_COMBINED_WORKFLOW_USER_DECISION_RECORDED`
