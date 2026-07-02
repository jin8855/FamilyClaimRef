# Document Attachment Coordinator User Decision Record

## A. Goal

이 문서는 `docs/91_DOCUMENT_ATTACHMENT_COORDINATOR_SCOPE_DESIGN.md`의 사용자 결정 기록이다.

목적은 `DocumentAttachmentCoordinator`의 책임 범위와 후속 구현 방향을 확정하는 것이다.

이 문서는 구현 문서가 아니다.

- C# 구현을 수행하지 않는다.
- coordinator 구현을 수행하지 않는다.
- request/result model을 생성하지 않는다.
- test code를 구현하지 않는다.
- UI/ViewModel을 구현하지 않는다.

## B. Checked Files / Paths

| 대상 | 확인 목적 | 비고 |
|---|---|---|
| `docs/91_DOCUMENT_ATTACHMENT_COORDINATOR_SCOPE_DESIGN.md` | Needs Decision Q1~Q19 확인 | 읽기 전용 |
| `docs/90_FILE_ATTACHMENT_SERVICE_IMPLEMENTATION_REVIEW.md` | file attachment service 구현 결과 확인 | 읽기 전용 |
| `docs/89_FILE_ATTACHMENT_SERVICE_USER_DECISION_RECORD.md` | file attachment service 사용자 결정 확인 | 읽기 전용 |
| `docs/87_JSON_STORAGE_IMPLEMENTATION_REVIEW.md` | JSON metadata storage 구현 결과 확인 | 읽기 전용 |
| `app/FamilyClaimRef.App/Services/Storage/IFileAttachmentService.cs` | file attachment interface 확인 | 읽기 전용 |
| `app/FamilyClaimRef.App/Services/Storage/LocalFileAttachmentService.cs` | local file attachment implementation 확인 | 읽기 전용 |
| `app/FamilyClaimRef.App/Services/Storage/FileAttachmentCopyResult.cs` | file copy result model 확인 | 읽기 전용 |
| `app/FamilyClaimRef.App/Services/Storage/IDocumentStorageService.cs` | metadata storage interface 확인 | 읽기 전용 |
| `app/FamilyClaimRef.App/Services/Storage/JsonDocumentStorageService.cs` | metadata storage implementation 책임 확인 | 읽기 전용 |
| `app/FamilyClaimRef.App/Services/FileNamePolicyService.cs` | physical file name 생성 정책 확인 | 읽기 전용 |
| `app/FamilyClaimRef.App/Models/Storage/DocumentRecord.cs` | 저장된 document metadata record 확인 | 읽기 전용 |
| `app/FamilyClaimRef.App/Models/Storage/DocumentDraft.cs` | document metadata 저장 입력 확인 | 읽기 전용 |
| `FamilyClaimRef.sln` | solution 기준 확인 | 읽기 전용 |

## C. User Decision Summary

| ID | Question | Decision | Recorded Direction |
|---|---|---|---|
| Q1 | coordinator/application service를 도입할 것인가? | Accepted | file copy와 metadata 저장을 하나의 사용자 작업 단위로 묶고 rollback/cleanup 책임을 ViewModel에서 분리하기 위해 coordinator/application service를 도입한다. |
| Q2 | 이름은 `DocumentAttachmentCoordinator`를 우선 후보로 둘 것인가? | Accepted | 이름은 `DocumentAttachmentCoordinator`를 우선 후보로 둔다. `DocumentRegistrationService`, `DocumentImportService`는 장기 후보 또는 rename 후보로만 둔다. |
| Q3 | UI/ViewModel이 `JsonDocumentStorageService`와 `IFileAttachmentService`를 직접 조합하지 않게 할 것인가? | Accepted | UI/ViewModel은 metadata storage와 file attachment service를 직접 조합하지 않고 후속 단계에서 coordinator를 호출한다. |
| Q4 | coordinator는 file copy 먼저, metadata 저장 나중 흐름을 담당할 것인가? | Accepted | coordinator는 file copy 먼저, metadata 저장 나중 흐름을 담당한다. |
| Q5 | coordinator가 physical file name 생성을 `FileNamePolicyService`로 수행할 것인가? | Accepted | coordinator가 `FileNamePolicyService.CreatePhysicalFileName(...)`을 호출해 physical file name을 생성한다. |
| Q6 | duplicateIndex는 coordinator가 자동 산정할 것인가? | Accepted | duplicateIndex는 coordinator가 자동 산정한다. caller/UI가 duplicateIndex를 직접 관리하지 않는다. |
| Q7 | target exists 시 다음 duplicateIndex를 시도할 것인가? | Accepted | target exists 시 다음 duplicateIndex를 시도한다. duplicateIndex 상한은 기존 `FileNamePolicyService` 정책을 따른다. |
| Q8 | metadata 저장 실패 시 copied file cleanup을 시도할 것인가? | Accepted | metadata 저장 실패 시 coordinator가 `IFileAttachmentService.DeleteDocumentFileIfExistsAsync(...)`로 copied file cleanup을 시도한다. |
| Q9 | cleanup 실패를 별도 failure로 노출할 것인가? | Accepted | cleanup 실패는 삼키지 않고 별도 failure로 노출한다. metadata 저장 실패와 cleanup 실패가 모두 추적 가능해야 한다. |
| Q10 | MVP에서는 custom exception 없이 기존 exception으로 처리할 것인가? | Accepted | MVP 1차에서는 custom exception을 만들지 않고 기존 .NET exception 흐름을 일관되게 사용한다. |
| Q11 | coordinator input model `DocumentAttachmentRequest`를 둘 것인가? | Accepted | `DocumentAttachmentRequest`를 둔다. duplicateIndex는 자동 산정하므로 request에는 포함하지 않는 방향을 우선한다. |
| Q12 | coordinator result model `DocumentAttachmentResult`를 둘 것인가? | Accepted | `DocumentAttachmentResult`를 둔다. 성공 결과에는 `DocumentRecord`와 `FileAttachmentCopyResult`를 포함하고 absolute path는 포함하지 않는다. |
| Q13 | source extension은 coordinator가 source file path에서 추출할 것인가? | Accepted | source extension은 coordinator가 source file path에서 추출하고 file copy 전에 validation failure를 처리한다. |
| Q14 | MVP 1차 coordinator는 Document metadata 저장까지만 처리할 것인가? | Accepted | MVP 1차 coordinator는 file copy와 `DocumentRecord` metadata 저장까지만 처리한다. |
| Q15 | Policy/Claim link까지 처리하는 workflow는 후속으로 보류할 것인가? | Accepted - Deferred | `PolicyDocumentRecord` / `ClaimDocumentRecord` link workflow는 후속으로 보류한다. |
| Q16 | 후속 구현 시 `DocumentAttachmentCoordinatorTests.cs`를 추가할 것인가? | Accepted | 후속 구현 시 `tests/FamilyClaimRef.App.Tests/DocumentAttachmentCoordinatorTests.cs`를 추가한다. |
| Q17 | test는 temp directory만 사용할 것인가? | Accepted | test source file, copied file, metadata JSON file 모두 temp directory 안에서만 생성한다. |
| Q18 | actual project `attachments/`, `data/local` 파일 생성은 금지할 것인가? | Accepted | 후속 test 구현에서 actual project `attachments/`, `data/local` 파일 생성은 금지한다. |
| Q19 | WPF UI/ViewModel 연동은 coordinator 구현 후로 보류할 것인가? | Accepted | WPF UI/ViewModel 연동과 file picker 구현은 coordinator 구현과 검증 이후로 보류한다. |

## D. Accepted Coordinator Direction

후속 구현 방향은 아래와 같이 확정한다.

- coordinator/application service를 도입한다.
- 이름은 `DocumentAttachmentCoordinator`를 우선한다.
- UI/ViewModel은 `JsonDocumentStorageService`와 `IFileAttachmentService`를 직접 조합하지 않는다.
- coordinator는 file copy 먼저, metadata 저장 나중 흐름을 담당한다.
- physical file name은 coordinator가 `FileNamePolicyService`로 생성한다.
- duplicateIndex는 coordinator가 자동 산정한다.
- target exists 시 다음 duplicateIndex를 시도한다.
- metadata 저장 실패 시 copied file cleanup을 시도한다.
- cleanup 실패는 별도 failure로 노출한다.
- MVP에서는 custom exception 없이 기존 exception을 사용한다.
- `DocumentAttachmentRequest` input model을 둔다.
- `DocumentAttachmentResult` result model을 둔다.
- source extension은 coordinator가 source file path에서 추출한다.
- MVP 1차는 Document metadata 저장까지만 처리한다.
- Policy/Claim link workflow는 후속으로 보류한다.
- 후속 구현 시 `DocumentAttachmentCoordinatorTests.cs`를 추가한다.
- test는 temp directory만 사용한다.
- actual project `attachments/`, `data/local` 파일 생성은 금지한다.
- WPF UI/ViewModel 연동은 보류한다.

## E. Implementation Candidate Files

후속 구현 후보 파일:

- `app/FamilyClaimRef.App/Services/Storage/DocumentAttachmentRequest.cs`
- `app/FamilyClaimRef.App/Services/Storage/DocumentAttachmentResult.cs`
- `app/FamilyClaimRef.App/Services/Storage/DocumentAttachmentCoordinator.cs`
- `tests/FamilyClaimRef.App.Tests/DocumentAttachmentCoordinatorTests.cs`

주의:

- 이번 문서에서는 위 파일을 생성하지 않는다.
- 실제 파일 생성은 별도 구현 승인 후 진행한다.
- 후속 구현에서도 test는 temp directory 기준으로 작성한다.
- 후속 구현에서도 actual project `attachments/`, `data/local` 파일 생성 여부를 분리해 검증한다.

## F. Request / Result Shape Candidate

`DocumentAttachmentRequest` 후보:

```csharp
public sealed record DocumentAttachmentRequest(
    string SourceFilePath,
    string DocumentScope,
    string DocumentType,
    string DisplayTitle,
    DateOnly ReferenceDate);
```

주의:

- `DuplicateIndex`는 request에서 제외한다.
- duplicateIndex는 coordinator가 자동 산정한다.
- source extension은 coordinator가 source file path에서 추출한다.

`DocumentAttachmentResult` 후보:

```csharp
public sealed record DocumentAttachmentResult(
    DocumentRecord Document,
    FileAttachmentCopyResult File);
```

주의:

- absolute path는 result에 포함하지 않는다.

## G. Coordinator Flow Candidate

후속 구현 흐름 후보:

1. request validation
2. source file path validation
3. source extension extraction
4. documentScope/documentType/displayTitle/referenceDate validation
5. duplicateIndex 자동 산정
6. `FileNamePolicyService.CreatePhysicalFileName(...)` 호출
7. `IFileAttachmentService.CopyDocumentFileAsync(...)` 호출
8. copy result 기준 `DocumentDraft` 생성
9. `IDocumentStorageService.AddDocumentAsync(...)` 호출
10. metadata 저장 성공 시 `DocumentAttachmentResult` 반환
11. metadata 저장 실패 시 `DeleteDocumentFileIfExistsAsync(copyResult.RelativePath)` 호출
12. cleanup 성공 시 원래 metadata 저장 failure를 노출
13. cleanup 실패 시 metadata failure와 cleanup failure를 모두 추적 가능한 방식으로 노출

## H. Still Not Implemented

아래 항목은 아직 구현하지 않았다.

- C# 구현 없음
- coordinator 구현 없음
- request/result model 생성 없음
- test code 구현 없음
- test file 생성 없음
- JSON metadata storage 수정 없음
- file attachment service 수정 없음
- `FileNamePolicyService` 수정 없음
- PolicyDocument/ClaimDocument link workflow 구현 없음
- WPF UI/XAML/navigation/ViewModel 구현 없음
- file picker 구현 없음
- OCR 구현 없음
- SQLite DB/package 추가 없음
- repository/data access/migration 구현 없음
- project root `attachments/` 내부 파일 생성 없음
- project root `data/local` 내부 파일 생성 없음
- 실제 개인정보 샘플 없음

## I. Next Step

다음 작업 후보:

1. 별도 승인 후 coordinator/request/result/test 구현
2. 구현 파일 후보:
   - `DocumentAttachmentRequest.cs`
   - `DocumentAttachmentResult.cs`
   - `DocumentAttachmentCoordinator.cs`
   - `DocumentAttachmentCoordinatorTests.cs`
3. 구현 후 `dotnet build FamilyClaimRef.sln`, `dotnet test FamilyClaimRef.sln` 실행
4. 구현 후 `docs/93_DOCUMENT_ATTACHMENT_COORDINATOR_IMPLEMENTATION_REVIEW.md` 생성
5. Policy/Claim link workflow는 이후 별도 설계
6. WPF UI/ViewModel 연동은 이후 보류

## J. Result

`DOCUMENT_ATTACHMENT_COORDINATOR_USER_DECISION_RECORDED`
