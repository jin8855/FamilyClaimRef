# Policy/Claim Document Link Workflow User Decision Record

## A. Goal

이 문서는 `docs/94_POLICY_CLAIM_DOCUMENT_LINK_WORKFLOW_SCOPE_DESIGN.md`의 사용자 결정 기록이다.

목적은 Policy/Claim document link workflow의 책임 범위와 후속 구현 방향을 확정하는 것이다.

이 문서는 구현 문서가 아니다.

- C# 구현을 수행하지 않는다.
- link coordinator 구현을 수행하지 않는다.
- request/result model을 생성하지 않는다.
- test code를 구현하지 않는다.
- UI/ViewModel을 구현하지 않는다.

## B. Checked Files / Paths

| 대상 | 확인 목적 | 비고 |
|---|---|---|
| `docs/94_POLICY_CLAIM_DOCUMENT_LINK_WORKFLOW_SCOPE_DESIGN.md` | Needs Decision Q1~Q19 확인 | 읽기 전용 |
| `docs/93_DOCUMENT_ATTACHMENT_COORDINATOR_IMPLEMENTATION_REVIEW.md` | attachment coordinator 구현 결과 확인 | 읽기 전용 |
| `docs/92_DOCUMENT_ATTACHMENT_COORDINATOR_USER_DECISION_RECORD.md` | attachment coordinator 사용자 결정 확인 | 읽기 전용 |
| `docs/87_JSON_STORAGE_IMPLEMENTATION_REVIEW.md` | JSON metadata storage 구현 결과 확인 | 읽기 전용 |
| `app/FamilyClaimRef.App/Services/Storage/DocumentAttachmentCoordinator.cs` | import coordinator 책임 경계 확인 | 읽기 전용 |
| `app/FamilyClaimRef.App/Services/Storage/IDocumentStorageService.cs` | link storage method 확인 | 읽기 전용 |
| `app/FamilyClaimRef.App/Services/Storage/JsonDocumentStorageService.cs` | link 저장 validation 기준 확인 | 읽기 전용 |
| `app/FamilyClaimRef.App/Models/Storage/DocumentRecord.cs` | existing Document 기준 확인 | 읽기 전용 |
| `app/FamilyClaimRef.App/Models/Storage/PolicyDocumentRecord.cs` | policy link record 확인 | 읽기 전용 |
| `app/FamilyClaimRef.App/Models/Storage/PolicyDocumentDraft.cs` | policy link input 확인 | 읽기 전용 |
| `app/FamilyClaimRef.App/Models/Storage/ClaimDocumentRecord.cs` | claim link record 확인 | 읽기 전용 |
| `app/FamilyClaimRef.App/Models/Storage/ClaimDocumentDraft.cs` | claim link input 확인 | 읽기 전용 |
| `FamilyClaimRef.sln` | solution 기준 확인 | 읽기 전용 |

## C. User Decision Summary

| ID | Question | Decision | Recorded Direction |
|---|---|---|---|
| Q1 | Policy/Claim document link workflow를 설계할 것인가? | Accepted | existing `DocumentRecord`를 `PolicyDocumentRecord` 또는 `ClaimDocumentRecord`로 연결하는 application workflow를 후속 구현 대상으로 둔다. |
| Q2 | `DocumentAttachmentCoordinator`를 확장하지 않고 별도 `DocumentLinkCoordinator`를 둘 것인가? | Accepted | `DocumentAttachmentCoordinator`는 file import + `DocumentRecord` metadata 저장까지만 유지하고, domain link 책임은 별도 `DocumentLinkCoordinator`로 분리한다. |
| Q3 | MVP 1차 link workflow는 existing Document link만 처리할 것인가? | Accepted | 이미 저장된 active `DocumentRecord.Id`를 받아 link만 생성한다. file import와 actual file copy/storage는 처리하지 않는다. |
| Q4 | import + link 통합 workflow는 후속으로 보류할 것인가? | Accepted - Deferred | rollback 경계가 커지고 Policy/Claim storage가 아직 없으므로 import + link 통합 workflow는 후속으로 보류한다. |
| Q5 | policy/claim request/result model은 분리할 것인가? | Accepted | policy/claim request/result model을 분리한다. Candidate B를 채택한다. |
| Q6 | `DocumentLinkCoordinator`는 `IDocumentStorageService`만 주입받을 것인가? | Accepted | `DocumentLinkCoordinator`는 `IDocumentStorageService`만 생성자 주입받고 concrete `JsonDocumentStorageService`에 직접 의존하지 않는다. |
| Q7 | `DocumentLinkCoordinator`는 file attachment service에 의존하지 않을 것인가? | Accepted | existing Document link만 처리하므로 `IFileAttachmentService`에 의존하지 않는다. |
| Q8 | `DocumentLinkCoordinator`는 `FileNamePolicyService`를 직접 호출하지 않을 것인가? | Accepted | documentType validation은 `IDocumentStorageService` 구현체가 수행하므로 직접 호출하지 않는다. |
| Q9 | documentType validation은 storage service에 맡길 것인가? | Accepted | `JsonDocumentStorageService`의 `FileNamePolicyService.GetAllowedDocumentTypes(...)` 기준 validation을 사용한다. |
| Q10 | policyId/claimId 존재 검증은 Policy/Claim storage 전까지 보류할 것인가? | Accepted - Deferred | Policy/Claim storage 구현 전까지 target existence validation은 보류하고, 현재는 required validation만 수행한다. |
| Q11 | policyId/claimId null/empty/whitespace는 거부할 것인가? | Accepted | policyId/claimId null/empty/whitespace는 거부한다. test에서는 dummy id만 사용한다. |
| Q12 | same target + documentId active duplicate는 documentType과 무관하게 거부할 것인가? | Accepted | 같은 policyId + documentId 또는 claimId + documentId active link가 있으면 documentType과 무관하게 새 link를 거부한다. |
| Q13 | disabled link는 active duplicate 판단에서 제외할 것인가? | Accepted | disabled link는 active duplicate 판단에서 제외한다. reactivation은 구현하지 않고 새 active link 생성은 허용 후보로 둔다. |
| Q14 | same document를 다른 policyId/claimId에 연결하는 것은 허용할 것인가? | Accepted | 같은 document를 다른 policyId 또는 다른 claimId에 연결하는 것은 허용한다. |
| Q15 | disabled Document는 신규 link 불가로 유지할 것인가? | Accepted | disabled `DocumentRecord`는 신규 policy/claim link 불가로 유지한다. storage service의 active documentId validation 기준을 따른다. |
| Q16 | 후속 구현 시 `DocumentLinkCoordinatorTests.cs`를 추가할 것인가? | Accepted | 후속 구현 시 `tests/FamilyClaimRef.App.Tests/DocumentLinkCoordinatorTests.cs`를 추가한다. |
| Q17 | test는 temp directory만 사용할 것인가? | Accepted | metadata JSON도 temp directory 안에서만 생성한다. actual file copy는 테스트 범위가 아니다. |
| Q18 | actual project `attachments/`, `data/local` 파일 생성은 금지할 것인가? | Accepted | 후속 test 구현에서 actual project `attachments/`, `data/local` 파일 생성은 금지한다. |
| Q19 | WPF UI/ViewModel 연동은 link workflow 구현 후로 보류할 것인가? | Accepted | WPF UI/ViewModel, file picker, 화면 바인딩, navigation은 link workflow 구현 후로 보류한다. |

## D. Accepted Link Workflow Direction

후속 구현 방향은 아래와 같이 확정한다.

- Policy/Claim document link workflow를 진행한다.
- `DocumentAttachmentCoordinator`는 확장하지 않는다.
- 별도 `DocumentLinkCoordinator`를 도입한다.
- MVP 1차는 existing Document link만 처리한다.
- import + link 통합 workflow는 보류한다.
- policy/claim request/result model은 분리한다.
- `DocumentLinkCoordinator`는 `IDocumentStorageService`만 주입받는다.
- file attachment service 의존은 없다.
- `FileNamePolicyService` 직접 호출은 없다.
- documentType validation은 storage service에 맡긴다.
- policyId/claimId 존재 검증은 Policy/Claim storage 전까지 보류한다.
- policyId/claimId null/empty/whitespace는 거부한다.
- same target + documentId active duplicate는 documentType과 무관하게 거부한다.
- disabled link는 active duplicate 판단에서 제외한다.
- same document를 다른 policyId/claimId에 연결하는 것은 허용한다.
- disabled Document 신규 link 불가를 유지한다.
- 후속 구현 시 `DocumentLinkCoordinatorTests.cs`를 추가한다.
- test는 temp directory만 사용한다.
- actual project `attachments/`, `data/local` 파일 생성은 금지한다.
- WPF UI/ViewModel 연동은 보류한다.

## E. Implementation Candidate Files

후속 구현 후보 파일:

- `app/FamilyClaimRef.App/Services/Storage/PolicyDocumentLinkRequest.cs`
- `app/FamilyClaimRef.App/Services/Storage/ClaimDocumentLinkRequest.cs`
- `app/FamilyClaimRef.App/Services/Storage/PolicyDocumentLinkResult.cs`
- `app/FamilyClaimRef.App/Services/Storage/ClaimDocumentLinkResult.cs`
- `app/FamilyClaimRef.App/Services/Storage/DocumentLinkCoordinator.cs`
- `tests/FamilyClaimRef.App.Tests/DocumentLinkCoordinatorTests.cs`

주의:

- 이번 문서에서는 위 파일을 생성하지 않는다.
- 실제 파일 생성은 별도 구현 승인 후 진행한다.
- 후속 구현에서도 test는 temp directory 기준으로 작성한다.
- 후속 구현에서도 actual project `attachments/`, `data/local` 파일 생성 여부를 분리해 검증한다.

## F. Request / Result Shape Candidate

`PolicyDocumentLinkRequest` 후보:

```csharp
public sealed record PolicyDocumentLinkRequest(
    string PolicyId,
    string DocumentId,
    string DocumentType);
```

`ClaimDocumentLinkRequest` 후보:

```csharp
public sealed record ClaimDocumentLinkRequest(
    string ClaimId,
    string DocumentId,
    string DocumentType);
```

`PolicyDocumentLinkResult` 후보:

```csharp
public sealed record PolicyDocumentLinkResult(
    PolicyDocumentRecord PolicyDocument);
```

`ClaimDocumentLinkResult` 후보:

```csharp
public sealed record ClaimDocumentLinkResult(
    ClaimDocumentRecord ClaimDocument);
```

주의:

- nullable result를 피한다.
- actual file copy result는 포함하지 않는다.
- absolute path는 포함하지 않는다.
- original file name은 포함하지 않는다.

## G. Coordinator Flow Candidate

### Policy link flow

1. request validation
2. policyId/documentId/documentType required validation
3. existing active duplicate policy link check
4. `PolicyDocumentDraft` 생성
5. `IDocumentStorageService.AddPolicyDocumentAsync(...)` 호출
6. `PolicyDocumentLinkResult` 반환

### Claim link flow

1. request validation
2. claimId/documentId/documentType required validation
3. existing active duplicate claim link check
4. `ClaimDocumentDraft` 생성
5. `IDocumentStorageService.AddClaimDocumentAsync(...)` 호출
6. `ClaimDocumentLinkResult` 반환

주의:

- documentId existence validation은 storage service에서 수행한다.
- disabled document validation은 storage service에서 수행한다.
- documentType validation은 storage service에서 수행한다.
- coordinator는 duplicate active link policy를 담당한다.
- Policy/Claim storage existence validation은 하지 않는다.

## H. Duplicate Link Policy

후속 구현 정책:

- active policy duplicate:
  - same policyId + documentId active link가 있으면 거부한다.
  - documentType이 달라도 거부한다.
- active claim duplicate:
  - same claimId + documentId active link가 있으면 거부한다.
  - documentType이 달라도 거부한다.
- disabled link:
  - active duplicate 판단에서 제외한다.
  - reactivation은 하지 않는다.
  - 새 active link 생성은 허용 후보로 둔다.
- cross target:
  - 같은 document를 다른 policyId에 연결 가능하다.
  - 같은 document를 다른 claimId에 연결 가능하다.

## I. Still Not Implemented

아래 항목은 아직 구현하지 않았다.

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

## J. Next Step

다음 작업 후보:

1. 별도 승인 후 `DocumentLinkCoordinator` / request / result / test 구현
2. 구현 파일 후보:
   - `PolicyDocumentLinkRequest.cs`
   - `ClaimDocumentLinkRequest.cs`
   - `PolicyDocumentLinkResult.cs`
   - `ClaimDocumentLinkResult.cs`
   - `DocumentLinkCoordinator.cs`
   - `DocumentLinkCoordinatorTests.cs`
3. 구현 후 `dotnet build FamilyClaimRef.sln`, `dotnet test FamilyClaimRef.sln` 실행
4. 구현 후 `docs/96_POLICY_CLAIM_DOCUMENT_LINK_WORKFLOW_IMPLEMENTATION_REVIEW.md` 생성
5. import + link 통합 workflow는 이후 별도 설계
6. WPF UI/ViewModel 연동은 이후 보류

## K. Result

`POLICY_CLAIM_DOCUMENT_LINK_WORKFLOW_USER_DECISION_RECORDED`
