# C# Model / Interface User Decision Record

## A. Goal

이 문서는 `docs/70_CSHARP_MODEL_INTERFACE_IMPLEMENTATION_SCOPE_DECISION.md`의 Needs Decision Q1~Q15에 대한 사용자 결정 기록이다.

목적은 FamilyClaimRef MVP의 C# model/interface 구현 범위를 확정하고, 다음 단계에서 실제 C# model/interface 구현 승인 여부를 판단하기 위한 기준을 제공하는 것이다.

이 문서는 구현 문서가 아니다. 실제 C# model/interface 구현, JSON file 생성, JSON storage 구현, storage service 구현, repository 구현, DB/OCR/metadata/file storage 구현은 수행하지 않는다.

## B. Checked Files / Paths

| 대상 | 확인 목적 | 비고 |
|---|---|---|
| `docs/70_CSHARP_MODEL_INTERFACE_IMPLEMENTATION_SCOPE_DECISION.md` | Q1~Q15 Needs Decision과 구현 범위 후보 확인 | 읽기 전용 |
| `docs/69_JSON_SCHEMA_USER_DECISION_RECORD.md` | JSON schema 사용자 결정 확인 | 읽기 전용 |
| `docs/67_STORAGE_SERVICE_INTERFACE_USER_DECISION_RECORD.md` | storage service interface 방향 확인 | 읽기 전용 |
| `docs/65_CATEGORY_ITEM_DOCUMENT_TYPE_USER_DECISION_RECORD.md` | document type source of truth와 allowlist 기준 확인 | 읽기 전용 |
| `docs/63_DOCUMENT_STORAGE_STRUCTURE_USER_DECISION_RECORD.md` | Document storage structure 사용자 결정 확인 | 읽기 전용 |
| `docs/59_FILENAME_POLICY_TEST_PROJECT_IMPLEMENTATION_REVIEW.md` | test project와 `FileNamePolicyService` 검토 상태 확인 | 읽기 전용 |
| `docs/43_WPF_MINIMAL_MVVM_STRUCTURE_DESIGN.md` | WPF MVVM 책임 분리 기준 확인 | 읽기 전용 |
| `app/FamilyClaimRef.App/Services/FileNamePolicyService.cs` | 기존 production service 확인 | 읽기 전용 |
| `tests/FamilyClaimRef.App.Tests/FileNamePolicyServiceTests.cs` | 기존 test 기준 확인 | 읽기 전용 |

## C. User Decision Summary

| ID | Question | Decision | Recorded Direction |
|---|---|---|---|
| Q1 | 다음 구현은 Record/Draft model + `IDocumentStorageService` interface 동시 구현으로 갈 것인가? | Accepted | 다음 실제 구현 단계는 Record/Draft model + `IDocumentStorageService` interface 동시 구현으로 간다. JSON storage implementation과 actual file copy/storage 구현은 포함하지 않는다. |
| Q2 | `JsonFileEnvelope<T>`를 C# model 후보에 포함할 것인가? | Accepted | `JsonFileEnvelope<T>`를 C# model 후보에 포함한다. `SchemaVersion`, `SavedAt`, `Items`를 가진다. `Items` 구현 형식은 `System.Text.Json` 직렬화 단순성을 고려해 `List<T>` 후보로 둔다. service 반환 타입은 필요 시 `IReadOnlyList<T>`를 사용한다. |
| Q3 | `DocumentRecord`, `PolicyDocumentRecord`, `ClaimDocumentRecord`를 구현 후보로 확정할 것인가? | Accepted | `DocumentRecord`, `PolicyDocumentRecord`, `ClaimDocumentRecord`를 구현 후보로 확정한다. |
| Q4 | `DocumentDraft`, `PolicyDocumentDraft`, `ClaimDocumentDraft`를 구현 후보로 확정할 것인가? | Accepted | `DocumentDraft`, `PolicyDocumentDraft`, `ClaimDocumentDraft`를 구현 후보로 확정한다. Draft는 저장 요청 입력이고 Record는 저장된 상태이다. |
| Q5 | `DocumentRecord`에서 `DocumentType`, `OriginalFileName`, `IsDisabled`, `Memo`를 제외할 것인가? | Accepted | `DocumentRecord`에서 `DocumentType`, `OriginalFileName`, `IsDisabled`, `Memo`를 제외한다. documentType source of truth는 domain link record에 둔다. `IsDisabled`는 `DisabledAt != null`에서 파생한다. |
| Q6 | `PolicyDocumentRecord.DocumentType`, `ClaimDocumentRecord.DocumentType`을 포함할 것인가? | Accepted | `PolicyDocumentRecord.DocumentType`, `ClaimDocumentRecord.DocumentType`을 포함한다. 저장값은 label이 아니라 code이다. |
| Q7 | `OcrConfirmedFieldsSnapshot`과 `Memo`를 MVP 1차 model에서 제외할 것인가? | Accepted | `OcrConfirmedFieldsSnapshot`과 `Memo`는 MVP 1차 model에서 제외한다. OCR 확정값 snapshot과 memo/tag/history memo 정책은 후속 문서에서 별도 결정한다. |
| Q8 | `CreatedAt`, `UpdatedAt`, `SavedAt`, `DisabledAt`은 `DateTimeOffset` 기반으로 갈 것인가? | Accepted | 시간 관련 type은 `DateTimeOffset` 기반으로 둔다. `DisabledAt`은 nullable `DateTimeOffset?` 후보로 둔다. JSON 표현은 UTC ISO-8601 string 기준을 유지한다. |
| Q9 | `SchemaVersion`은 `int`로 둘 것인가? | Accepted | `SchemaVersion`은 `int`로 둔다. 초기 schemaVersion 값은 `1` 후보로 둔다. |
| Q10 | `IDocumentStorageService`는 async method + `CancellationToken` 후보로 갈 것인가? | Accepted | `IDocumentStorageService`는 async method 후보로 간다. 각 method에는 `CancellationToken cancellationToken = default` 후보를 둔다. JSON file I/O와 SQLite 전환 가능성을 고려한다. |
| Q11 | 실제 삭제 method는 제외하고 `Disable...` method만 둘 것인가? | Accepted | 실제 삭제 method는 MVP 1차에서 제외한다. `DisableDocumentAsync`, `DisablePolicyDocumentAsync`, `DisableClaimDocumentAsync` 같은 `DisabledAt` 기반 method만 후보로 둔다. |
| Q12 | JSON file path/name은 interface에 노출하지 않을 것인가? | Accepted | JSON file path와 JSON file name은 interface에 노출하지 않는다. JSON file 단위는 implementation detail이다. ViewModel은 JSON path, file name, DTO를 직접 알지 않는다. |
| Q13 | `IFileAttachmentService`, `ICategorySeedService` 구현은 이번 첫 구현 범위에서 제외할 것인가? | Accepted | `IFileAttachmentService`, `ICategorySeedService` 구현은 이번 첫 구현 범위에서 제외한다. actual file copy/open, CategoryItem seed 조회/검증 구현은 별도 승인 후 진행한다. |
| Q14 | C# model/interface 구현 후 `dotnet build`와 기존 `dotnet test`를 실행할 것인가? | Accepted | 실제 C# model/interface 구현 후 `dotnet build FamilyClaimRef.sln`과 `dotnet test FamilyClaimRef.sln`을 실행한다. 기존 `FileNamePolicyService` 테스트가 깨지지 않는지 확인한다. |
| Q15 | JSON storage implementation은 별도 승인 전까지 하지 않을 것인가? | Accepted | JSON storage implementation은 별도 승인 전까지 하지 않는다. 실제 JSON file 생성도 별도 승인 전까지 하지 않는다. `data/local/*.json` file은 이번 구현 범위에도 포함하지 않는다. |

## D. Accepted Implementation Scope

- 다음 구현은 Record/Draft model + `IDocumentStorageService` interface 동시 구현으로 간다.
- `JsonFileEnvelope<T>`를 포함한다.
- `JsonFileEnvelope<T>.Items`는 `List<T>` 후보로 기록한다.
- `DocumentRecord`, `PolicyDocumentRecord`, `ClaimDocumentRecord`를 구현 후보로 확정한다.
- `DocumentDraft`, `PolicyDocumentDraft`, `ClaimDocumentDraft`를 구현 후보로 확정한다.
- `DocumentRecord`에서 `DocumentType`, `OriginalFileName`, `IsDisabled`, `Memo`를 제외한다.
- `PolicyDocumentRecord.DocumentType`, `ClaimDocumentRecord.DocumentType`을 포함한다.
- `OcrConfirmedFieldsSnapshot`, `Memo`는 MVP 1차 model에서 제외한다.
- 시간 type은 `DateTimeOffset` 기반으로 둔다.
- `DisabledAt`은 `DateTimeOffset?`로 둔다.
- `SchemaVersion`은 `int`로 둔다.
- `IDocumentStorageService`는 async method + `CancellationToken` 기준으로 둔다.
- 실제 삭제 method는 제외한다.
- `Disable...` method만 후보로 둔다.
- JSON file path/name은 interface에 노출하지 않는다.
- `IFileAttachmentService`, `ICategorySeedService` 구현은 첫 구현 범위에서 제외한다.
- 구현 후 `dotnet build`, `dotnet test`를 실행한다.
- JSON storage implementation은 별도 승인 전까지 제외한다.

## E. Implementation Candidate Files

후속 구현 후보 파일:

- `app/FamilyClaimRef.App/Models/Storage/JsonFileEnvelope.cs`
- `app/FamilyClaimRef.App/Models/Storage/DocumentRecord.cs`
- `app/FamilyClaimRef.App/Models/Storage/DocumentDraft.cs`
- `app/FamilyClaimRef.App/Models/Storage/PolicyDocumentRecord.cs`
- `app/FamilyClaimRef.App/Models/Storage/PolicyDocumentDraft.cs`
- `app/FamilyClaimRef.App/Models/Storage/ClaimDocumentRecord.cs`
- `app/FamilyClaimRef.App/Models/Storage/ClaimDocumentDraft.cs`
- `app/FamilyClaimRef.App/Services/Storage/IDocumentStorageService.cs`

주의:

- 이 문서에서는 위 파일을 생성하지 않는다.
- 실제 파일 생성은 별도 구현 승인 후 진행한다.

## F. Still Not Implemented

아래 항목은 아직 구현하지 않았다.

- C# model 구현 없음
- C# interface 구현 없음
- JSON file 생성 없음
- JSON storage 구현 없음
- SQLite DB 생성 없음
- SQLite package 추가 없음
- repository/data access/migration 구현 없음
- CategoryItem 구현 없음
- CategoryItem JSON 저장 구현 없음
- document type seed constant 구현 없음
- allowlist/seed consistency test 구현 없음
- actual file copy/storage 구현 없음
- `attachments/` 내부 파일 생성 없음
- `data/local/` 내부 파일 생성 없음
- DB/OCR/metadata/file storage 구현 없음
- WPF UI/XAML/navigation 구현 없음
- test code 수정 없음
- NuGet package 추가 없음
- 실제 개인정보 샘플 없음

## G. Next Step

다음 작업 후보:

1. 별도 승인 후 C# model/interface 구현
2. 구현 후 `docs/72_CSHARP_MODEL_INTERFACE_IMPLEMENTATION_REVIEW.md` 생성
3. 그 다음 document type seed constant 구현 여부 결정
4. 그 다음 allowlist/seed consistency test 설계
5. 그 다음 JSON file storage implementation 설계

## H. Result

`CSHARP_MODEL_INTERFACE_USER_DECISION_RECORDED`
