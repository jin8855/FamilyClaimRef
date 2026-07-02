# Policy/Claim Document Link Workflow Implementation Review

## A. Goal

이 문서는 Policy/Claim document link workflow 구현 결과 리뷰 문서다.

검토 대상은 `DocumentLinkCoordinator`, policy/claim request/result model, `DocumentLinkCoordinatorTests` 구현 결과다.

이 문서는 다음 구현 리뷰가 아니다.

- import + link combined workflow 구현 리뷰가 아니다.
- WPF UI/ViewModel 구현 리뷰가 아니다.
- Policy/Claim storage 구현 리뷰가 아니다.
- OCR, SQLite, repository, migration 구현 리뷰가 아니다.

## B. Checked Files / Paths

| Path | Purpose | Result |
|---|---|---|
| `docs/95_POLICY_CLAIM_DOCUMENT_LINK_WORKFLOW_USER_DECISION_RECORD.md` | 사용자 결정 기준 확인 | Checked |
| `docs/94_POLICY_CLAIM_DOCUMENT_LINK_WORKFLOW_SCOPE_DESIGN.md` | 구현 범위 기준 확인 | Checked |
| `docs/93_DOCUMENT_ATTACHMENT_COORDINATOR_IMPLEMENTATION_REVIEW.md` | attachment coordinator 책임 경계 확인 | Checked |
| `app/FamilyClaimRef.App/Services/Storage/PolicyDocumentLinkRequest.cs` | policy request model 확인 | Checked |
| `app/FamilyClaimRef.App/Services/Storage/ClaimDocumentLinkRequest.cs` | claim request model 확인 | Checked |
| `app/FamilyClaimRef.App/Services/Storage/PolicyDocumentLinkResult.cs` | policy result model 확인 | Checked |
| `app/FamilyClaimRef.App/Services/Storage/ClaimDocumentLinkResult.cs` | claim result model 확인 | Checked |
| `app/FamilyClaimRef.App/Services/Storage/DocumentLinkCoordinator.cs` | coordinator 구현 확인 | Checked |
| `tests/FamilyClaimRef.App.Tests/DocumentLinkCoordinatorTests.cs` | test coverage 확인 | Checked |
| `app/FamilyClaimRef.App/Services/Storage/DocumentAttachmentCoordinator.cs` | 수정 여부 및 책임 경계 확인 | Checked |
| `app/FamilyClaimRef.App/Services/Storage/IDocumentStorageService.cs` | storage boundary 확인 | Checked |
| `app/FamilyClaimRef.App/Services/Storage/JsonDocumentStorageService.cs` | validation owner 확인 | Checked |
| `app/FamilyClaimRef.App/Services/FileNamePolicyService.cs` | direct dependency 여부 확인 | Checked |
| `app/FamilyClaimRef.App/Models/Storage/PolicyDocumentRecord.cs` | policy link record 확인 | Checked |
| `app/FamilyClaimRef.App/Models/Storage/PolicyDocumentDraft.cs` | policy link draft 확인 | Checked |
| `app/FamilyClaimRef.App/Models/Storage/ClaimDocumentRecord.cs` | claim link record 확인 | Checked |
| `app/FamilyClaimRef.App/Models/Storage/ClaimDocumentDraft.cs` | claim link draft 확인 | Checked |
| `FamilyClaimRef.sln` | build/test 대상 확인 | Checked |
| `app/FamilyClaimRef.App/FamilyClaimRef.App.csproj` | SDK-style compile include 확인 | Checked |
| `tests/FamilyClaimRef.App.Tests/FamilyClaimRef.App.Tests.csproj` | test project include 확인 | Checked |

## C. Implementation Summary

- `PolicyDocumentLinkRequest.cs` 생성 확인.
- `ClaimDocumentLinkRequest.cs` 생성 확인.
- `PolicyDocumentLinkResult.cs` 생성 확인.
- `ClaimDocumentLinkResult.cs` 생성 확인.
- `DocumentLinkCoordinator.cs` 생성 확인.
- `DocumentLinkCoordinatorTests.cs` 생성 확인.
- policy/claim link method가 분리되어 있다.
- `DocumentLinkCoordinator`는 `IDocumentStorageService`만 주입받는다.
- file attachment service 의존이 없다.
- `FileNamePolicyService` 직접 호출이 없다.
- documentType validation은 storage service에 위임되어 있다.
- policyId/claimId required validation이 구현되어 있다.
- policy/claim active duplicate link 거부가 구현되어 있다.
- disabled link는 active duplicate 판단에서 제외된다.
- same document cross target link가 허용된다.
- disabled Document 신규 link 불가 정책은 storage validation으로 유지된다.
- `DocumentAttachmentCoordinator.cs` 수정 없음.
- JSON metadata storage 수정 없음.
- file attachment service 수정 없음.
- `FileNamePolicyService` 수정 없음.
- Policy/Claim storage 구현 없음.
- import + link combined workflow 구현 없음.
- WPF UI/ViewModel/file picker 구현 없음.
- project root `attachments/`, `data/local` 내부 파일 생성 없음.

## D. Request Model Review

`PolicyDocumentLinkRequest`는 다음 필드만 포함한다.

- `PolicyId`
- `DocumentId`
- `DocumentType`

`ClaimDocumentLinkRequest`는 다음 필드만 포함한다.

- `ClaimId`
- `DocumentId`
- `DocumentType`

확인 결과:

- nullable target result 구조를 두지 않았다.
- actual file copy 관련 필드가 없다.
- absolute path 필드가 없다.
- original file name 필드가 없다.
- 실제 개인정보 샘플이 없다.

## E. Result Model Review

`PolicyDocumentLinkResult`는 `PolicyDocumentRecord`를 포함한다.

`ClaimDocumentLinkResult`는 `ClaimDocumentRecord`를 포함한다.

확인 결과:

- nullable result를 만들지 않았다.
- actual file copy result를 포함하지 않는다.
- absolute path를 포함하지 않는다.
- original file name을 포함하지 않는다.

## F. DocumentLinkCoordinator Review

확인 결과:

- constructor에서 `IDocumentStorageService` null guard가 있다.
- concrete `JsonDocumentStorageService`에 직접 의존하지 않는다.
- `IFileAttachmentService`에 의존하지 않는다.
- `FileNamePolicyService`에 의존하지 않는다.
- `LinkPolicyDocumentAsync(...)`가 구현되어 있다.
- `LinkClaimDocumentAsync(...)`가 구현되어 있다.
- request null validation이 있다.
- policyId/claimId required validation이 있다.
- documentId required validation이 있다.
- documentType required validation이 있다.
- active duplicate policy link check가 있다.
- active duplicate claim link check가 있다.
- `PolicyDocumentDraft`를 생성한다.
- `ClaimDocumentDraft`를 생성한다.
- `IDocumentStorageService.AddPolicyDocumentAsync(...)`를 호출한다.
- `IDocumentStorageService.AddClaimDocumentAsync(...)`를 호출한다.
- `PolicyDocumentLinkResult`를 반환한다.
- `ClaimDocumentLinkResult`를 반환한다.
- documentId existence validation은 storage service에 위임되어 있다.
- disabled document validation은 storage service에 위임되어 있다.
- documentType validation은 storage service에 위임되어 있다.
- policyId/claimId existence validation은 구현하지 않았다.
- custom exception을 생성하지 않았다.
- import + link combined workflow는 구현하지 않았다.
- WPF UI/ViewModel/file picker는 구현하지 않았다.

## G. Duplicate Link Policy Review

### Policy duplicate

확인 결과:

- 같은 `PolicyId + DocumentId` active link가 있으면 거부한다.
- `DocumentType`이 달라도 거부한다.
- disabled policy link는 active duplicate 판단에서 제외한다.
- 같은 document를 다른 policyId에 연결할 수 있다.

### Claim duplicate

확인 결과:

- 같은 `ClaimId + DocumentId` active link가 있으면 거부한다.
- `DocumentType`이 달라도 거부한다.
- disabled claim link는 active duplicate 판단에서 제외한다.
- 같은 document를 다른 claimId에 연결할 수 있다.

### Validation owner

확인 결과:

- duplicate active link check는 coordinator 책임이다.
- documentType validation은 storage service 책임이다.
- disabled document validation은 storage service 책임이다.
- target policyId/claimId existence validation은 아직 없다.

## H. Test Coverage Review

`DocumentLinkCoordinatorTests.cs` 기준 테스트 메서드 수는 22개다.

xUnit theory row를 포함한 실행 test case 증가분은 38개다.

### Constructor / request validation

확인:

- constructor null storage rejected.
- null policy request rejected.
- null claim request rejected.
- missing/empty policyId rejected.
- missing/empty claimId rejected.
- missing/empty documentId rejected.
- missing/empty documentType rejected.

### Policy link success

확인:

- link existing active document to policy succeeds.
- result contains `PolicyDocumentRecord`.
- result policyId equals request policyId.
- result documentId equals request documentId.
- result documentType equals request documentType.

### Claim link success

확인:

- link existing active document to claim succeeds.
- result contains `ClaimDocumentRecord`.
- result claimId equals request claimId.
- result documentId equals request documentId.
- result documentType equals request documentType.

### Storage validation passthrough

확인:

- missing documentId rejected.
- disabled documentId rejected.
- invalid policy documentType rejected.
- invalid claim documentType rejected.
- policy `capture` accepted.
- claim `capture` rejected.

### Duplicate link policy

확인:

- duplicate active policy link rejected.
- duplicate active claim link rejected.
- duplicate active policy link rejected even if documentType differs.
- duplicate active claim link rejected even if documentType differs.
- disabled policy link is excluded from duplicate check.
- disabled claim link is excluded from duplicate check.
- same document can link to different policyId.
- same document can link to different claimId.

### Scope safety

확인:

- test uses temp directory only.
- project root `data/local` is not created.
- project root `attachments` is not created.
- actual 개인정보 sample 없음.

### Excluded tests

다음 테스트는 이번 범위에서 제외되어 있다.

- file copy test 없음.
- `DocumentAttachmentCoordinator` test 없음.
- WPF UI/ViewModel test 없음.
- file picker test 없음.
- OCR test 없음.
- SQLite test 없음.
- Policy/Claim storage existence validation test 없음.
- import + link combined workflow test 없음.
- production `data/local` permission test 없음.
- production `attachments` permission test 없음.

## I. Verification Result

검증 명령:

```powershell
dotnet build FamilyClaimRef.sln
dotnet test FamilyClaimRef.sln
```

검증 결과:

- `dotnet build FamilyClaimRef.sln`: PASS
- warning: 0
- error: 0
- `dotnet test FamilyClaimRef.sln`: PASS
- 총 테스트 개수: 179
- 실패: 0
- 통과: 179
- 건너뜀: 0
- 추가 테스트 개수 보고:
  - test method 기준: 22
  - xUnit theory row 포함 실행 case 기준: 38
- 실패 테스트: 없음
- 실패 원인: 없음
- 권한 상승 실행 여부: 있음
- 초기 실패/재시도 여부:
  - 직전 구현 검증에서 최초 build는 Windows SDK 경로 접근 권한 문제로 실패 후 권한 상승으로 재시도했다.
  - 이번 리뷰 검증은 같은 원인을 피하기 위해 권한 상승으로 fresh build/test를 실행했다.
- project root `attachments/` 상태: files=0
- project root `data/local` 상태: files=0
- temp directory만 사용 여부: 확인
- SQLite/DB 파일 생성 없음
- Git 상태:
  - 현재 경로가 Git 저장소가 아니어서 `git status --short`는 실패했다.

참고:

- 직전 리뷰 기준 총 테스트 수가 141개였고, 이번 실행 기준 총 테스트 수는 179개다.
- 총 실행 test case 증가는 38개다.
- 문서상 추가 테스트 파일의 test method 수는 22개다.
- 차이는 xUnit `[Theory]`의 `[InlineData]` row가 개별 test case로 실행되기 때문이다.

## J. Scope Compliance Review

범위 준수 확인:

- `DocumentAttachmentCoordinator.cs` 수정 없음.
- JSON metadata storage 수정 없음.
- `JsonDocumentStorageService.cs` 수정 없음.
- file attachment service 수정 없음.
- `IFileAttachmentService.cs` 수정 없음.
- `LocalFileAttachmentService.cs` 수정 없음.
- `FileNamePolicyService.cs` 수정 없음.
- storage model 수정 없음.
- existing test file 수정 없음.
- actual file copy/storage 수정 없음.
- Policy/Claim storage 구현 없음.
- import + link combined workflow 구현 없음.
- WPF UI/XAML/navigation/ViewModel 구현 없음.
- file picker 구현 없음.
- OCR 구현 없음.
- SQLite DB/package 추가 없음.
- repository/data access/migration 구현 없음.
- project root `attachments/` 내부 파일 생성 없음.
- project root `data/local` 내부 파일 생성 없음.
- 실제 개인정보 샘플 사용 없음.
- `.sln` 수정 없음.
- `.csproj` 수정 없음.
- NuGet package 추가 없음.
- Git commit/reset/checkout/add 없음.

## K. Out of Scope / Not Implemented

아직 구현하지 않은 항목:

- Policy/Claim storage 없음.
- target policyId/claimId 존재 검증 없음.
- import + link combined workflow 없음.
- `DocumentAttachmentCoordinator` 확장 없음.
- file copy test 없음.
- WPF UI/ViewModel 연동 없음.
- file picker 없음.
- disabled link reactivation 없음.
- custom exception 없음.
- UI error classification 없음.
- SQLite/repository 없음.
- OCR 없음.

## L. Risks

남은 위험:

- Policy/Claim storage가 아직 없어 target id 존재 검증이 없다.
- import + link combined workflow가 아직 없다.
- WPF UI/ViewModel 연동이 아직 없다.
- disabled link reactivation이 없다.
- duplicate link 정책은 추후 실제 사용 흐름에서 조정될 수 있다.
- same target + documentId duplicate 거부가 실제 업무 흐름에서는 너무 엄격할 가능성이 있다.
- import와 link가 분리되어 imported but unlinked document가 생길 수 있다.
- UI 연동 전까지 사용자 흐름 전체는 검증되지 않았다.

## M. Recommendation

추천 순서:

1. 현재 `DocumentLinkCoordinator` implementation은 build/test PASS 상태로 고정한다.
2. 다음 단계에서는 import + link combined workflow 설계 또는 WPF ViewModel/file picker boundary 설계 중 하나를 선택한다.
3. 데이터 정합성 기준으로는 import + link combined workflow 설계가 다음 후보가 될 수 있다.
4. UI 진행 기준으로는 WPF ViewModel/file picker boundary 설계가 다음 후보가 될 수 있다.
5. Policy/Claim storage가 아직 없으므로 target id existence validation은 후속 storage 설계 이후로 둔다.

## N. Result

`POLICY_CLAIM_DOCUMENT_LINK_WORKFLOW_IMPLEMENTATION_REVIEWED`
