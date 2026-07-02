# File Attachment Service User Decision Record

## A. Goal

이 문서는 `docs/88_FILE_ATTACHMENT_SERVICE_SCOPE_DESIGN.md`의 사용자 결정 기록이다.

목적은 `IFileAttachmentService`의 책임 범위와 후속 구현 방향을 확정하는 것이다.

이 문서는 구현 문서가 아니다.

- C# 구현을 수행하지 않는다.
- interface 구현을 수행하지 않는다.
- actual file copy/storage를 구현하지 않는다.
- 실제 파일을 생성하지 않는다.
- test code를 구현하지 않는다.

## B. Checked Files / Paths

| 대상 | 확인 목적 | 비고 |
|---|---|---|
| `docs/88_FILE_ATTACHMENT_SERVICE_SCOPE_DESIGN.md` | Needs Decision Q1~Q19와 interface 후보 확인 | 읽기 전용 |
| `docs/87_JSON_STORAGE_IMPLEMENTATION_REVIEW.md` | JSON metadata storage 구현 결과와 미구현 범위 확인 | 읽기 전용 |
| `docs/86_JSON_STORAGE_IMPLEMENTATION_USER_DECISION_RECORD.md` | JSON storage와 actual file copy/storage 분리 결정 확인 | 읽기 전용 |
| `docs/85_JSON_STORAGE_IMPLEMENTATION_DESIGN.md` | JSON metadata storage 설계 기준 확인 | 읽기 전용 |
| `app/FamilyClaimRef.App/Services/Storage/JsonDocumentStorageService.cs` | metadata storage 책임과 actual file copy 미포함 확인 | 읽기 전용 |
| `app/FamilyClaimRef.App/Services/Storage/IDocumentStorageService.cs` | document metadata service contract 확인 | 읽기 전용 |
| `app/FamilyClaimRef.App/Services/FileNamePolicyService.cs` | physical file name 생성 기준 확인 | 읽기 전용 |
| `app/FamilyClaimRef.App/Models/Storage/DocumentRecord.cs` | `RelativePath`, `PhysicalFileName`, `Extension` 저장 기준 확인 | 읽기 전용 |
| `app/FamilyClaimRef.App/Models/Storage/DocumentDraft.cs` | document 저장 입력 기준 확인 | 읽기 전용 |
| `tests/FamilyClaimRef.App.Tests/JsonDocumentStorageServiceTests.cs` | temp directory 기반 metadata test 기준 확인 | 읽기 전용 |
| `FamilyClaimRef.sln` | solution 기준 확인 | 읽기 전용 |

## C. User Decision Summary

| ID | Question | Decision | Recorded Direction |
|---|---|---|---|
| Q1 | actual file copy/storage 구현 전에 `IFileAttachmentService` interface를 먼저 설계할 것인가? | Accepted | actual file copy/storage 구현 전에 `IFileAttachmentService` interface 방향을 먼저 확정한다. |
| Q2 | `IFileAttachmentService`는 JSON metadata를 직접 수정하지 않는 것으로 확정할 것인가? | Accepted | `IFileAttachmentService`는 `documents.json`, `policy-documents.json`, `claim-documents.json`에 접근하지 않는다. metadata 저장은 `JsonDocumentStorageService` 책임으로 유지한다. |
| Q3 | `JsonDocumentStorageService`는 actual file copy를 하지 않는 것으로 유지할 것인가? | Accepted | `JsonDocumentStorageService`는 actual file copy를 하지 않는다. metadata storage와 actual file storage 책임을 분리한다. |
| Q4 | file copy와 metadata 저장을 묶는 coordinator/application service는 별도 후속 후보로 둘 것인가? | Accepted - Deferred | `DocumentAttachmentCoordinator`, `DocumentRegistrationService`, `DocumentImportService` 같은 coordinator/application service는 별도 후속 후보로 둔다. 이번 단계에서는 구현하지 않는다. |
| Q5 | MVP 1차 흐름은 file copy 먼저, metadata 저장 나중, 실패 시 copied file cleanup 시도 방향으로 둘 것인가? | Accepted | MVP 1차 orchestration 후보는 file copy 먼저, metadata 저장 나중으로 둔다. metadata 저장 실패 시 copied file cleanup을 시도한다. 이 흐름은 후속 coordinator/application service 책임으로 둔다. |
| Q6 | attachment root는 생성자 등으로 주입받게 할 것인가? | Accepted | attachment root path는 생성자 등으로 주입받는다. test에서는 temp directory를 주입할 수 있어야 한다. service 내부에서 고정 absolute path를 만들지 않는다. |
| Q7 | test에서는 temp directory만 사용할 것인가? | Accepted | test에서는 temp directory만 사용한다. 실제 project `attachments/`를 만들지 않는다. 테스트 source file도 temp directory 안의 dummy file만 사용한다. |
| Q8 | production 후보 root는 `attachments/`로 둘 것인가? | Accepted | production 후보 root는 `attachments/`로 둔다. 실제 production root 적용은 후속 구현 또는 composition 단계에서 결정한다. 이번 문서에서는 실제 `attachments/`를 생성하지 않는다. |
| Q9 | metadata에는 absolute path를 저장하지 않을 것인가? | Accepted | metadata에는 absolute path를 저장하지 않는다. PC 이전, 백업, 복원, root 변경에 대비하기 위해 relative path만 저장한다. |
| Q10 | `DocumentRecord.RelativePath`는 attachment root 기준 relative path로 둘 것인가? | Accepted | `DocumentRecord.RelativePath`는 attachment root 기준 relative path로 둔다. 저장값은 `/` 기반 relative path 후보로 둔다. |
| Q11 | physical file name 생성은 `IFileAttachmentService`가 아니라 caller/coordinator가 `FileNamePolicyService`를 통해 수행하게 할 것인가? | Accepted | physical file name 생성은 `IFileAttachmentService` 책임이 아니다. caller/coordinator가 `FileNamePolicyService.CreatePhysicalFileName(...)`으로 생성한다. |
| Q12 | `IFileAttachmentService`는 전달받은 physical file name 또는 relative path로 copy만 수행하게 할 것인가? | Accepted | `IFileAttachmentService`는 전달받은 physical file name 또는 relative path를 기준으로 copy만 수행한다. MVP 1차 interface shape는 Candidate B document-specific copy service를 우선한다. |
| Q13 | source file missing은 failure로 처리할 것인가? | Accepted | source file이 없으면 failure로 처리한다. 후속 구현에서는 일관된 exception/result 정책을 적용한다. |
| Q14 | target file already exists는 failure로 처리할 것인가? | Accepted | target file이 이미 있으면 overwrite하지 않고 failure로 처리한다. duplicate physical file name 충돌은 caller/coordinator가 duplicateIndex 산정으로 피해야 한다. |
| Q15 | path traversal attempt는 failure로 처리할 것인가? | Accepted | `../`, absolute path, root escape 시도는 failure로 처리한다. attachment root 밖으로 파일이 생성되면 안 된다. |
| Q16 | delete-if-exists primitive를 포함할 것인가? | Accepted | cleanup/rollback을 위해 delete-if-exists primitive를 포함한다. metadata 저장 실패 후 copied file cleanup은 후속 coordinator가 이 primitive를 사용해 처리할 수 있다. |
| Q17 | exists primitive를 포함할 것인가? | Accepted | target 존재 확인과 UI/검증 후보를 위해 exists primitive를 포함한다. 다만 exists 후 copy 사이 race condition은 완전 방지하지 못하므로 overwrite 방지 검증은 copy 시점에도 수행해야 한다. |
| Q18 | 후속 구현 시 `IFileAttachmentServiceTests.cs`를 추가할 것인가? | Accepted | 후속 구현 시 `tests/FamilyClaimRef.App.Tests/IFileAttachmentServiceTests.cs` 또는 구현체 기준 test file을 추가한다. test는 temp directory만 사용한다. |
| Q19 | actual project `attachments/` 파일 생성은 production 구현에서만 가능하고 test에서는 temp directory만 사용할 것인가? | Accepted | actual project `attachments/` 파일 생성은 production 구현에서만 가능하다. test에서는 temp directory만 사용한다. 테스트가 project root, `attachments/`, `data/local`을 오염시키면 안 된다. |

## D. Accepted File Attachment Direction

아래 방향을 후속 구현 기준으로 기록한다.

- actual file copy/storage 구현 전 interface 방향을 확정한다.
- `IFileAttachmentService`는 JSON metadata를 직접 수정하지 않는다.
- `JsonDocumentStorageService`는 actual file copy를 하지 않는다.
- coordinator/application service는 후속 후보로 둔다.
- MVP 1차 흐름은 file copy 먼저, metadata 저장 나중, 실패 시 cleanup 시도 방향으로 둔다.
- attachment root는 생성자 등으로 주입한다.
- test는 temp directory만 사용한다.
- production 후보 root는 `attachments/`로 둔다.
- metadata에는 absolute path를 저장하지 않는다.
- `DocumentRecord.RelativePath`는 attachment root 기준 relative path로 둔다.
- physical file name 생성은 caller/coordinator가 `FileNamePolicyService` 기준으로 수행한다.
- `IFileAttachmentService`는 전달받은 physical file name 기준으로 copy한다.
- source file missing은 failure로 처리한다.
- target already exists는 failure로 처리한다.
- path traversal attempt는 failure로 처리한다.
- delete-if-exists primitive를 포함한다.
- exists primitive를 포함한다.
- 후속 구현 시 file attachment tests를 추가한다.
- actual project `attachments/` 파일 생성은 production 구현에서만 가능하다.
- test에서는 temp directory만 사용한다.

## E. Interface Shape Decision

MVP 1차 interface shape는 아래 방향으로 기록한다.

### Accepted: Candidate B. Document-specific copy service

선택 이유:

- 현재 범위는 범용 file storage가 아니라 document attachment 저장이다.
- `DocumentRecord.RelativePath`를 생성해야 한다.
- 호출부/coordinator가 단순해진다.
- physical file name 생성은 여전히 caller/coordinator 책임으로 유지할 수 있다.
- service는 document folder 기준 relative path를 반환할 수 있다.

후속 interface 후보:

```csharp
public interface IFileAttachmentService
{
    Task<FileAttachmentCopyResult> CopyDocumentFileAsync(
        string sourceFilePath,
        string physicalFileName,
        CancellationToken cancellationToken = default);

    Task DeleteDocumentFileIfExistsAsync(
        string relativePath,
        CancellationToken cancellationToken = default);

    Task<bool> DocumentFileExistsAsync(
        string relativePath,
        CancellationToken cancellationToken = default);
}
```

추가 result model 후보:

```csharp
public sealed record FileAttachmentCopyResult(
    string RelativePath,
    string PhysicalFileName,
    string Extension,
    long SizeBytes);
```

주의:

- 이번 문서에서는 interface와 result model을 생성하지 않는다.
- 실제 생성은 후속 구현 지시에서만 수행한다.
- physical file name 생성은 service가 하지 않는다.
- service는 document folder 기준 relative path 반환 후보를 가진다.

### Deferred: Candidate A. Generic relative path copy service

generic relative path copy service는 장기 후보로 둔다.

향후 document 외 attachment category가 늘어나면 재검토한다.

## F. Implementation Candidate Files

후속 구현 후보 파일:

- `app/FamilyClaimRef.App/Services/Storage/IFileAttachmentService.cs`
- `app/FamilyClaimRef.App/Services/Storage/FileAttachmentCopyResult.cs`
- `app/FamilyClaimRef.App/Services/Storage/LocalFileAttachmentService.cs`
- `tests/FamilyClaimRef.App.Tests/IFileAttachmentServiceTests.cs`

주의:

- 이번 문서에서는 위 파일을 생성하지 않는다.
- 실제 파일 생성은 별도 구현 승인 후 진행한다.
- 후속 구현에서도 test는 temp directory 기준으로 작성한다.
- 후속 구현에서도 actual project `attachments/` 내부 파일 생성 여부를 분리해 보고해야 한다.

## G. Still Not Implemented

아래 항목은 아직 구현되지 않았다.

- C# 구현 없음
- interface 구현 없음
- actual file copy/storage 구현 없음
- 실제 file 생성 없음
- test code 구현 없음
- test file 생성 없음
- JSON metadata storage 수정 없음
- `JsonDocumentStorageService` 수정 없음
- `FileNamePolicyService` 수정 없음
- coordinator/application service 구현 없음
- WPF UI/XAML/navigation/ViewModel 구현 없음
- file picker 구현 없음
- OCR 구현 없음
- SQLite DB/package 추가 없음
- repository/data access/migration 구현 없음
- `attachments/` 내부 파일 생성 없음
- `data/local` 내부 파일 생성 없음
- 실제 개인정보 샘플 없음

## H. Next Step

다음 작업 후보:

1. 별도 승인 후 `IFileAttachmentService` interface/implementation/test 구현
2. 구현 파일 후보:
   - `IFileAttachmentService.cs`
   - `FileAttachmentCopyResult.cs`
   - `LocalFileAttachmentService.cs`
   - `IFileAttachmentServiceTests.cs`
3. 구현 후 `dotnet build FamilyClaimRef.sln`, `dotnet test FamilyClaimRef.sln` 실행
4. 구현 후 `docs/90_FILE_ATTACHMENT_SERVICE_IMPLEMENTATION_REVIEW.md` 생성
5. coordinator/application service는 file attachment service 구현 후 별도 설계
6. WPF UI/ViewModel 연동은 이후 보류

## I. Result

`FILE_ATTACHMENT_SERVICE_USER_DECISION_RECORDED`
