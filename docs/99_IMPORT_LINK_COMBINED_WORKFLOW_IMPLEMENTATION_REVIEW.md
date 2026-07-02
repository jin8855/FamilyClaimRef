# Import + Link Combined Workflow Implementation Review

## A. Goal

이 문서는 import + link combined workflow 구현 결과 리뷰 문서다.

기록 대상은 다음과 같다.

- `DocumentRegistrationWorkflow` 구현 결과
- policy/claim registration request model 구현 결과
- policy/claim registration result model 구현 결과
- `DocumentRegistrationWorkflowTests` 구현 결과
- 구현 범위 준수 여부
- build/test 검증 결과

이 문서는 다음 구현 리뷰가 아니다.

- WPF UI/ViewModel/file picker 구현 리뷰가 아니다.
- Policy/Claim storage 구현 리뷰가 아니다.
- OCR, SQLite, repository, data access 구현 리뷰가 아니다.

## B. Checked Files / Paths

| Path | Purpose | Result |
|---|---|---|
| `docs/98_IMPORT_LINK_COMBINED_WORKFLOW_USER_DECISION_RECORD.md` | 사용자 결정 기준 확인 | Checked |
| `docs/97_IMPORT_LINK_COMBINED_WORKFLOW_SCOPE_DESIGN.md` | workflow scope 기준 확인 | Checked |
| `docs/96_POLICY_CLAIM_DOCUMENT_LINK_WORKFLOW_IMPLEMENTATION_REVIEW.md` | link coordinator 구현 경계 확인 | Checked |
| `docs/93_DOCUMENT_ATTACHMENT_COORDINATOR_IMPLEMENTATION_REVIEW.md` | attachment coordinator 구현 경계 확인 | Checked |
| `docs/90_FILE_ATTACHMENT_SERVICE_IMPLEMENTATION_REVIEW.md` | file attachment service 구현 경계 확인 | Checked |
| `docs/87_JSON_STORAGE_IMPLEMENTATION_REVIEW.md` | JSON metadata storage 구현 경계 확인 | Checked |
| `app/FamilyClaimRef.App/Services/Storage/PolicyDocumentRegistrationRequest.cs` | policy registration request 확인 | Checked |
| `app/FamilyClaimRef.App/Services/Storage/PolicyDocumentRegistrationResult.cs` | policy registration result 확인 | Checked |
| `app/FamilyClaimRef.App/Services/Storage/ClaimDocumentRegistrationRequest.cs` | claim registration request 확인 | Checked |
| `app/FamilyClaimRef.App/Services/Storage/ClaimDocumentRegistrationResult.cs` | claim registration result 확인 | Checked |
| `app/FamilyClaimRef.App/Services/Storage/DocumentRegistrationWorkflow.cs` | combined workflow 구현 확인 | Checked |
| `tests/FamilyClaimRef.App.Tests/DocumentRegistrationWorkflowTests.cs` | workflow test coverage 확인 | Checked |
| `app/FamilyClaimRef.App/Services/Storage/DocumentAttachmentCoordinator.cs` | import coordinator 의존 경계 확인 | Checked |
| `app/FamilyClaimRef.App/Services/Storage/DocumentLinkCoordinator.cs` | link coordinator 의존 경계 확인 | Checked |
| `app/FamilyClaimRef.App/Services/Storage/IDocumentStorageService.cs` | metadata rollback 경계 확인 | Checked |
| `app/FamilyClaimRef.App/Services/Storage/JsonDocumentStorageService.cs` | JSON storage validation owner 확인 | Checked |
| `app/FamilyClaimRef.App/Services/Storage/IFileAttachmentService.cs` | copied file rollback 경계 확인 | Checked |
| `app/FamilyClaimRef.App/Services/Storage/LocalFileAttachmentService.cs` | local file primitive 경계 확인 | Checked |
| `app/FamilyClaimRef.App/Services/FileNamePolicyService.cs` | workflow 직접 의존 여부 확인 | Checked |
| `FamilyClaimRef.sln` | build/test 대상 확인 | Checked |
| `app/FamilyClaimRef.App/FamilyClaimRef.App.csproj` | app project 대상 확인 | Checked |
| `tests/FamilyClaimRef.App.Tests/FamilyClaimRef.App.Tests.csproj` | test project 대상 확인 | Checked |

## C. Implementation Summary

- `PolicyDocumentRegistrationRequest.cs` 생성 확인.
- `PolicyDocumentRegistrationResult.cs` 생성 확인.
- `ClaimDocumentRegistrationRequest.cs` 생성 확인.
- `ClaimDocumentRegistrationResult.cs` 생성 확인.
- `DocumentRegistrationWorkflow.cs` 생성 확인.
- `DocumentRegistrationWorkflowTests.cs` 생성 확인.
- policy registration method와 claim registration method가 분리되어 있다.
- workflow는 `DocumentAttachmentCoordinator`와 `DocumentLinkCoordinator`를 조합한다.
- 기존 `DocumentAttachmentCoordinator`와 `DocumentLinkCoordinator`의 책임은 변경하지 않았다.
- link 실패 시 copied file delete를 시도한다.
- link 실패 시 created `DocumentRecord` disable을 시도한다.
- rollback 실패 시 `AggregateException`으로 원본 link failure와 rollback failure를 함께 노출한다.
- custom exception은 만들지 않았다.
- Policy/Claim entity 생성은 구현하지 않았다.
- Policy/Claim storage existence validation은 구현하지 않았다.
- tests는 temp directory 기반으로 작성되어 있다.
- WPF UI/ViewModel/file picker는 구현하지 않았다.
- project root `attachments/`, `data/local` 내부 파일은 생성하지 않았다.

## D. Request Model Review

`PolicyDocumentRegistrationRequest`는 다음 필드만 포함한다.

- `SourceFilePath`
- `PolicyId`
- `DocumentType`
- `DisplayTitle`
- `ReferenceDate`

`ClaimDocumentRegistrationRequest`는 다음 필드만 포함한다.

- `SourceFilePath`
- `ClaimId`
- `DocumentType`
- `DisplayTitle`
- `ReferenceDate`

확인 결과:

- absolute path storage field는 없다.
- original file name storage field는 없다.
- Policy/Claim entity creation field는 없다.
- 실제 개인정보 샘플 필드는 없다.

## E. Result Model Review

`PolicyDocumentRegistrationResult`는 다음 결과를 포함한다.

- `DocumentAttachmentResult Attachment`
- `PolicyDocumentLinkResult Link`

`ClaimDocumentRegistrationResult`는 다음 결과를 포함한다.

- `DocumentAttachmentResult Attachment`
- `ClaimDocumentLinkResult Link`

확인 결과:

- nullable result 구조를 만들지 않았다.
- absolute path를 결과에 추가하지 않았다.
- original file name을 결과에 추가하지 않았다.
- Policy/Claim entity creation result를 포함하지 않았다.

## F. DocumentRegistrationWorkflow Review

확인 결과:

- constructor에서 `DocumentAttachmentCoordinator` null guard가 있다.
- constructor에서 `DocumentLinkCoordinator` null guard가 있다.
- constructor에서 `IDocumentStorageService` null guard가 있다.
- constructor에서 `IFileAttachmentService` null guard가 있다.
- `RegisterPolicyDocumentAsync(...)`가 구현되어 있다.
- `RegisterClaimDocumentAsync(...)`가 구현되어 있다.
- request null validation이 있다.
- `PolicyId` required validation이 있다.
- `ClaimId` required validation이 있다.
- policy registration은 `DocumentScope = "policy"`로 `DocumentAttachmentRequest`를 만든다.
- claim registration은 `DocumentScope = "claim"`으로 `DocumentAttachmentRequest`를 만든다.
- file import는 `DocumentAttachmentCoordinator.AttachDocumentAsync(...)`에 위임한다.
- policy link는 `DocumentLinkCoordinator.LinkPolicyDocumentAsync(...)`에 위임한다.
- claim link는 `DocumentLinkCoordinator.LinkClaimDocumentAsync(...)`에 위임한다.
- success flow는 registration result를 반환한다.
- link failure flow는 rollback을 시도한다.
- rollback success 시 원본 link failure를 다시 throw한다.
- rollback failure 시 원본 link failure와 rollback failure를 함께 노출한다.
- hard delete는 수행하지 않는다.
- custom exception은 만들지 않는다.
- Policy/Claim entity 생성은 하지 않는다.
- Policy/Claim existence validation은 하지 않는다.
- WPF file picker 또는 UI state 변경은 하지 않는다.
- `FileNamePolicyService` 직접 의존은 없다.
- Policy/Claim storage, SQLite, repository 직접 의존은 없다.

## G. Rollback Review

rollback 대상:

- copied file
- created `DocumentRecord`

rollback 방식:

- copied file rollback은 `IFileAttachmentService.DeleteDocumentFileIfExistsAsync(...)`를 사용한다.
- metadata rollback은 `IDocumentStorageService.DisableDocumentAsync(...)`를 사용한다.
- `DocumentRecord` hard delete는 하지 않는다.
- copied file delete와 document disable은 모두 시도한다.
- rollback success 시 원본 link exception을 유지한다.
- rollback failure 시 `AggregateException`을 사용한다.

확인 결과:

- link failure가 rollback success로 정리되면 원본 link exception이 노출된다.
- file delete failure가 발생하면 rollback failure로 노출된다.
- document disable failure가 발생하면 rollback failure로 노출된다.
- file delete와 document disable이 모두 실패하면 원본 link failure 포함 3개 exception이 추적된다.
- rollback failure는 숨기지 않는다.

## H. Test Coverage Review

`DocumentRegistrationWorkflowTests.cs` 기준 test method 수는 18개다.

xUnit `[Theory]` row를 포함한 runtime case 기준 실행 수는 22개다.

### Success flow

확인:

- register policy document succeeds.
- register claim document succeeds.
- result contains attachment result and link result.
- copied file exists under temp attachment root.
- document metadata exists under temp metadata root.
- policy link exists under temp metadata root.
- claim link exists under temp metadata root.
- result/document relative path is not absolute.

### Request validation

확인:

- constructor null dependencies rejected.
- null policy request rejected.
- null claim request rejected.
- missing/empty policyId rejected before file copy.
- missing/empty claimId rejected before file copy.
- missing source fails before link.
- invalid claim documentType fails before final link success.

보완 메모:

- workflow-level invalid policy documentType 전용 test method는 현재 파일에서 확인되지 않는다.
- 다만 lower-level `DocumentLinkCoordinatorTests`와 `DocumentAttachmentCoordinatorTests`에서 documentType validation 경계가 이미 검증되어 있다.

### Rollback

확인:

- policy link failure deletes copied file and disables document.
- claim link failure deletes copied file and disables document.
- rollback file delete failure is reported.
- rollback document disable failure is reported.
- rollback attempts file delete and document disable when both fail.
- rollback success rethrows original link failure.
- rollback failure throws `AggregateException` containing link and rollback failures.

### Scope safety

확인:

- tests use temp directory only.
- project root `attachments/` files are unchanged.
- project root `data/local` files are unchanged.
- no actual 개인정보 sample is used.
- dummy file name and dummy content only are used.

### Excluded tests

다음 테스트는 이번 구현 범위에서 제외되어 있다.

- WPF file picker test.
- UI/ViewModel test.
- OCR test.
- SQLite test.
- Policy/Claim storage existence validation test.
- concurrent import + link test.
- production `attachments/` permission test.
- production `data/local` permission test.

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
- total tests: 201
- failed: 0
- passed: 201
- skipped: 0
- `DocumentRegistrationWorkflowTests.cs` test method count: 18
- `DocumentRegistrationWorkflowTests.cs` runtime case count: 22
- 권한 상승 실행 여부: 있음
- 사유: 이전 검증에서 Windows SDK 경로 접근 제한으로 일반 실행 build가 실패한 이력이 있어 fresh build/test를 권한 상승으로 실행했다.
- project root `attachments/` files: 0
- project root `data/local` files: 0
- SQLite/DB file 생성: 없음
- Git 상태 확인: 현재 경로가 Git 저장소가 아니어서 `git status --short`는 실패했다.

## J. Scope Compliance Review

범위 준수 확인:

- 기존 `DocumentAttachmentCoordinator.cs` 수정 없음.
- 기존 `DocumentLinkCoordinator.cs` 수정 없음.
- 기존 JSON metadata storage 수정 없음.
- 기존 file attachment service 수정 없음.
- 기존 `FileNamePolicyService.cs` 수정 없음.
- 기존 storage model 수정 없음.
- 기존 test file 수정 없음.
- Policy/Claim storage 구현 없음.
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

- Policy/Claim storage.
- target policyId/claimId existence validation.
- WPF UI/ViewModel integration.
- file picker.
- custom exception.
- UI error classification.
- concurrent import + link test.
- production path permission test.
- SQLite/repository.
- OCR.
- Policy/Claim entity creation.

## L. Risks

남은 위험:

- Policy/Claim storage가 아직 없어 target id existence validation이 없다.
- rollback은 hard delete가 아니라 disabled `DocumentRecord` history를 남긴다.
- custom exception이 없어 UI error classification이 아직 거칠 수 있다.
- WPF integration이 없어 실제 사용자 파일 선택 흐름은 아직 검증되지 않았다.
- production `attachments/`, `data/local` 권한 문제는 temp directory 테스트로만 간접 검증되어 있다.
- rollback 실패 시 사용자가 수동 복구해야 할 수 있다.
- UI가 어떤 단위에서 workflow를 호출할지 아직 확정되지 않았다.
- workflow-level invalid policy documentType 전용 테스트는 추가 보완 후보로 남아 있다.

## M. Recommendation

추천:

1. 현재 `DocumentRegistrationWorkflow` 구현은 build/test PASS 상태로 유지한다.
2. WPF ViewModel/file picker boundary 설계 문서를 다음 단계로 작성한다.
3. UI는 lower-level coordinator/storage가 아니라 `DocumentRegistrationWorkflow`를 호출하도록 설계한다.
4. target id selection 또는 dummy local policy/claim 생성 정책은 별도 문서에서 결정한다.
5. custom exception과 UI error classification은 MVP 이후 hardening 후보로 둔다.
6. workflow-level invalid policy documentType test 추가 여부를 다음 테스트 보강 후보로 검토한다.

## N. Result

`IMPORT_LINK_COMBINED_WORKFLOW_IMPLEMENTATION_REVIEWED`
