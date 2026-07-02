# C# Model / Interface Implementation Scope Decision

## A. Goal

이 문서는 C# model/interface 구현 전에 구현 범위를 결정하기 위한 문서이다.

목적은 `docs/69_JSON_SCHEMA_USER_DECISION_RECORD.md`와 storage service interface 결정 내용을 C# type 후보로 매핑하고, 다음 구현 단계에서 어떤 record/class/interface를 만들지, 어디까지 구현할지, 무엇을 보류할지 결정하기 위한 기준을 제공하는 것이다.

이 문서는 구현 문서가 아니다. 실제 C# model/interface 구현, JSON 저장 구현, storage service 구현, repository 구현, DB/OCR/metadata/file storage 구현은 수행하지 않는다.

## B. Current State

- WPF scaffold 생성은 완료되어 있다.
- TargetFramework는 `net10.0-windows`이다.
- `FileNamePolicyService`는 구현되어 있으며 테스트 PASS 기준이 기록되어 있다.
- test project가 존재한다.
- JSON schema 사용자 결정은 완료되어 있다.
- storage service interface 사용자 결정은 완료되어 있다.
- 아직 C# model 구현은 없다.
- 아직 C# interface 구현은 없다.
- 아직 실제 JSON file 생성은 없다.
- 아직 JSON storage 구현은 없다.
- `app/FamilyClaimRef.App/Models/`는 구현 대기 후보 영역이다.
- `app/FamilyClaimRef.App/Services/`에는 현재 `FileNamePolicyService.cs`가 있으며, 추가 storage service/interface 구현은 아직 없다.

## C. Implementation Scope Candidate

### Candidate 1. Record / Draft model만 먼저 구현

대상 후보:

- `DocumentRecord`
- `PolicyDocumentRecord`
- `ClaimDocumentRecord`
- `DocumentDraft`
- `PolicyDocumentDraft`
- `ClaimDocumentDraft`
- `JsonFileEnvelope<T>`

장점:

- JSON schema와 직접 대응되는 C# type을 먼저 안정화할 수 있다.
- service interface 구현 전 type 안정성을 확보할 수 있다.
- 후속 테스트 작성 준비가 가능하다.

단점:

- interface 없이 model만 있으면 실제 사용 흐름이 아직 없다.
- DTO와 domain model 경계가 흐려질 수 있다.

### Candidate 2. Interface만 먼저 구현

대상 후보:

- `IDocumentStorageService`
- `IFileAttachmentService`
- `ICategorySeedService`

장점:

- ViewModel/service 경계를 먼저 고정할 수 있다.
- JSON implementation 없이도 설계 의도를 표현할 수 있다.

단점:

- 입력/출력 type 없이 interface signature가 불완전하다.
- method signature가 후속 model 결정에 따라 흔들릴 수 있다.

### Candidate 3. Record / Draft model + 핵심 interface 동시 구현

대상 후보:

- Record/Draft model 후보
- `JsonFileEnvelope<T>`
- `IDocumentStorageService`
- `IFileAttachmentService`, `ICategorySeedService`는 구현 보류 또는 문서 후보 유지

장점:

- JSON schema, storage interface, 테스트 범위가 연결된다.
- 다음 단계에서 JSON storage implementation으로 넘어가기 쉽다.
- 과도한 구현 없이 metadata 저장 경계를 잡을 수 있다.

단점:

- 한 번에 생성할 파일 수가 늘어난다.
- JSON storage 구현 전이라 일부 type은 미사용 상태가 될 수 있다.

## D. Recommended Direction

Candidate Recommendation:

- MVP 다음 구현 후보는 **Record / Draft model + `IDocumentStorageService` interface 동시 구현**이 적절하다.
- 실제 JSON storage implementation은 아직 하지 않는다.
- `IFileAttachmentService`와 `ICategorySeedService`는 이번 첫 구현에서 보류하거나 별도 승인 대상으로 둔다.
- `JsonFileEnvelope<T>`는 JSON schema와 직접 연결되므로 model 또는 storage contract 후보로 둔다.
- ViewModel이 JSON DTO를 직접 알지 않도록 model namespace와 storage contract namespace를 분리한다.
- `DocumentRecord`에는 `DocumentType`, `OriginalFileName`, `IsDisabled`, `Memo`를 넣지 않는다.
- `PolicyDocumentRecord`와 `ClaimDocumentRecord`에는 `DocumentType`을 둔다.
- `DisabledAt`은 nullable `DateTimeOffset?` 후보로 둔다.
- `CreatedAt`, `UpdatedAt`, `SavedAt`은 `DateTimeOffset` 후보로 둔다.
- `SchemaVersion`은 `int` 후보로 둔다.
- 위 방향은 확정 구현 지시가 아니라 사용자 결정 전 Candidate Recommendation이다.

## E. C# Type Candidate

### Envelope 후보

| Type | Status | Note |
|---|---|---|
| `JsonFileEnvelope<T>` | Candidate | `schemaVersion`, `savedAt`, `items` |

Field 후보:

- `int SchemaVersion`
- `DateTimeOffset SavedAt`
- `IReadOnlyList<T>` 또는 `List<T> Items`

Needs Decision:

- `Items`를 immutable하게 둘지 mutable list로 둘지 결정이 필요하다.

### Document model 후보

| Type | Status | Note |
|---|---|---|
| `DocumentRecord` | Candidate | persisted document metadata |
| `DocumentDraft` | Candidate | add document input |

`DocumentRecord` field 후보:

- `string Id`
- `string PhysicalFileName`
- `string DisplayTitle`
- `string Extension`
- `string RelativePath`
- `DateTimeOffset CreatedAt`
- `DateTimeOffset UpdatedAt`
- `DateTimeOffset? DisabledAt`

Excluded:

- `DocumentType`
- `OriginalFileName`
- `IsDisabled`
- `Memo`

`DocumentDraft` field 후보:

- `string PhysicalFileName`
- `string DisplayTitle`
- `string Extension`
- `string RelativePath`

Needs Decision:

- `DocumentDraft`가 `Id`를 받을지, service가 생성할지 결정이 필요하다.
- `CreatedAt`, `UpdatedAt`을 service가 생성할지, caller가 전달할지 결정이 필요하다.

### PolicyDocument model 후보

| Type | Status | Note |
|---|---|---|
| `PolicyDocumentRecord` | Candidate | persisted policy-document link |
| `PolicyDocumentDraft` | Candidate | add policy document input |

`PolicyDocumentRecord` field 후보:

- `string Id`
- `string PolicyId`
- `string DocumentId`
- `string DocumentType`
- `DateTimeOffset CreatedAt`
- `DateTimeOffset UpdatedAt`
- `DateTimeOffset? DisabledAt`

`PolicyDocumentDraft` field 후보:

- `string PolicyId`
- `string DocumentId`
- `string DocumentType`

Excluded:

- `Memo`

### ClaimDocument model 후보

| Type | Status | Note |
|---|---|---|
| `ClaimDocumentRecord` | Candidate | persisted claim-document link |
| `ClaimDocumentDraft` | Candidate | add claim document input |

`ClaimDocumentRecord` field 후보:

- `string Id`
- `string ClaimId`
- `string DocumentId`
- `string DocumentType`
- `DateTimeOffset CreatedAt`
- `DateTimeOffset UpdatedAt`
- `DateTimeOffset? DisabledAt`

`ClaimDocumentDraft` field 후보:

- `string ClaimId`
- `string DocumentId`
- `string DocumentType`

Excluded / Deferred:

- `OcrConfirmedFieldsSnapshot`
- `Memo`

## F. Interface Candidate

`IDocumentStorageService` method 후보:

- `Task<IReadOnlyList<DocumentRecord>> GetDocumentsAsync(CancellationToken cancellationToken = default)`
- `Task<DocumentRecord?> GetDocumentByIdAsync(string documentId, CancellationToken cancellationToken = default)`
- `Task<DocumentRecord> AddDocumentAsync(DocumentDraft draft, CancellationToken cancellationToken = default)`
- `Task DisableDocumentAsync(string documentId, DateTimeOffset disabledAt, CancellationToken cancellationToken = default)`
- `Task<IReadOnlyList<PolicyDocumentRecord>> GetPolicyDocumentsAsync(string policyId, CancellationToken cancellationToken = default)`
- `Task<PolicyDocumentRecord> AddPolicyDocumentAsync(PolicyDocumentDraft draft, CancellationToken cancellationToken = default)`
- `Task DisablePolicyDocumentAsync(string policyDocumentId, DateTimeOffset disabledAt, CancellationToken cancellationToken = default)`
- `Task<IReadOnlyList<ClaimDocumentRecord>> GetClaimDocumentsAsync(string claimId, CancellationToken cancellationToken = default)`
- `Task<ClaimDocumentRecord> AddClaimDocumentAsync(ClaimDocumentDraft draft, CancellationToken cancellationToken = default)`
- `Task DisableClaimDocumentAsync(string claimDocumentId, DateTimeOffset disabledAt, CancellationToken cancellationToken = default)`

주의:

- 실제 삭제 method는 포함하지 않는다.
- raw `originalFileName` input은 포함하지 않는다.
- `Document.documentType`은 포함하지 않는다.
- actual file copy/open method는 포함하지 않는다.
- JSON path/file name은 interface에 노출하지 않는다.
- reference validation 세부 규칙은 JSON implementation design에서 결정한다.

## G. Namespace / Folder Candidate

예시 후보:

- `app/FamilyClaimRef.App/Models/Storage/DocumentRecord.cs`
- `app/FamilyClaimRef.App/Models/Storage/DocumentDraft.cs`
- `app/FamilyClaimRef.App/Models/Storage/PolicyDocumentRecord.cs`
- `app/FamilyClaimRef.App/Models/Storage/PolicyDocumentDraft.cs`
- `app/FamilyClaimRef.App/Models/Storage/ClaimDocumentRecord.cs`
- `app/FamilyClaimRef.App/Models/Storage/ClaimDocumentDraft.cs`
- `app/FamilyClaimRef.App/Models/Storage/JsonFileEnvelope.cs`
- `app/FamilyClaimRef.App/Services/Storage/IDocumentStorageService.cs`

주의:

- 이 문서에서는 위 파일을 생성하지 않는다.
- folder/file 생성은 별도 구현 승인 후 진행한다.

## H. Test Scope Candidate

후속 구현 시 테스트 범위 후보:

- C# record/class 생성 후 build test
- `DocumentRecord`에 excluded field가 없는지 compile-level 확인
- `IDocumentStorageService` interface signature compile 확인
- JSON storage 구현 전에는 behavior test 없음
- 이후 JSON implementation 단계에서 serialization/deserialization test 추가
- allowlist/seed consistency test는 document type seed constant 구현 후 별도 진행

주의:

- 이 문서에서는 test code를 생성하지 않는다.
- `dotnet test` 실행은 이 문서 생성 작업 범위가 아니다.

## I. Needs Decision

1. 다음 구현은 Record/Draft model + `IDocumentStorageService` interface 동시 구현으로 갈 것인가?
2. `JsonFileEnvelope<T>`를 C# model 후보에 포함할 것인가?
3. `DocumentRecord`, `PolicyDocumentRecord`, `ClaimDocumentRecord`를 구현 후보로 확정할 것인가?
4. `DocumentDraft`, `PolicyDocumentDraft`, `ClaimDocumentDraft`를 구현 후보로 확정할 것인가?
5. `DocumentRecord`에서 `DocumentType`, `OriginalFileName`, `IsDisabled`, `Memo`를 제외할 것인가?
6. `PolicyDocumentRecord.DocumentType`, `ClaimDocumentRecord.DocumentType`을 포함할 것인가?
7. `OcrConfirmedFieldsSnapshot`과 `Memo`를 MVP 1차 model에서 제외할 것인가?
8. `CreatedAt`, `UpdatedAt`, `SavedAt`, `DisabledAt`은 `DateTimeOffset` 기반으로 갈 것인가?
9. `SchemaVersion`은 `int`로 둘 것인가?
10. `IDocumentStorageService`는 async method + `CancellationToken` 후보로 갈 것인가?
11. 실제 삭제 method는 제외하고 `Disable...` method만 둘 것인가?
12. JSON file path/name은 interface에 노출하지 않을 것인가?
13. `IFileAttachmentService`, `ICategorySeedService` 구현은 이번 첫 구현 범위에서 제외할 것인가?
14. C# model/interface 구현 후 `dotnet build`와 기존 `dotnet test`를 실행할 것인가?
15. JSON storage implementation은 별도 승인 전까지 하지 않을 것인가?

## J. Out of Scope

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

## K. Risks

- model과 JSON schema가 어긋나면 storage 구현 시 재작업이 발생한다.
- interface를 먼저 확정하면 후속 JSON implementation에서 signature 변경이 필요할 수 있다.
- Draft에서 id/timestamp 생성 책임을 결정하지 않으면 service 구현 시 논란이 발생한다.
- `IReadOnlyList<T>`와 `List<T>` 선택에 따라 serialization convenience가 달라진다.
- `DateTimeOffset` 사용 기준은 UI 표시 timezone 정책과 연결될 수 있다.
- model을 너무 빨리 구현하면 아직 미결정인 reference validation 정책을 반영하기 어려울 수 있다.

## L. Recommendation

1. 이 문서를 기준으로 C# model/interface 구현 범위를 결정받는다.
2. 사용자 결정 후 `docs/71_CSHARP_MODEL_INTERFACE_USER_DECISION_RECORD.md`를 생성한다.
3. 그 다음 별도 승인으로 C# model/interface 구현을 진행한다.
4. 구현 후 `dotnet build FamilyClaimRef.sln`과 `dotnet test FamilyClaimRef.sln`을 실행한다.
5. 그 다음 document type seed constant 구현 여부를 결정한다.
6. 그 다음 JSON file storage implementation 설계로 진행한다.

## M. Result

`CSHARP_MODEL_INTERFACE_IMPLEMENTATION_SCOPE_DECISION_DRAFTED`
