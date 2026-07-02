# C# Model / Interface Implementation Review

## A. Goal

이 문서는 C# Record/Draft model과 `IDocumentStorageService` interface 1차 구현 결과 리뷰 문서이다.

검토 범위는 storage model/interface 구현 결과, 생성 파일, 결정 범위 준수 여부, build/test 검증 결과, 남은 위험을 기록하는 것이다.

이 문서는 JSON storage 구현 리뷰가 아니다. actual file storage 구현 리뷰도 아니며, WPF UI/ViewModel 구현 리뷰도 아니다.

## B. Checked Files / Paths

| 대상 | 확인 목적 | 판정 |
|---|---|---|
| `app/FamilyClaimRef.App/Models/Storage/JsonFileEnvelope.cs` | envelope model 확인 | PASS |
| `app/FamilyClaimRef.App/Models/Storage/DocumentRecord.cs` | document metadata record 확인 | PASS |
| `app/FamilyClaimRef.App/Models/Storage/DocumentDraft.cs` | document draft 확인 | PASS |
| `app/FamilyClaimRef.App/Models/Storage/PolicyDocumentRecord.cs` | policy-document link record 확인 | PASS |
| `app/FamilyClaimRef.App/Models/Storage/PolicyDocumentDraft.cs` | policy-document draft 확인 | PASS |
| `app/FamilyClaimRef.App/Models/Storage/ClaimDocumentRecord.cs` | claim-document link record 확인 | PASS |
| `app/FamilyClaimRef.App/Models/Storage/ClaimDocumentDraft.cs` | claim-document draft 확인 | PASS |
| `app/FamilyClaimRef.App/Services/Storage/IDocumentStorageService.cs` | storage service interface 확인 | PASS |
| `app/FamilyClaimRef.App/Services/FileNamePolicyService.cs` | 기존 service 수정 여부 확인 | PASS |
| `tests/FamilyClaimRef.App.Tests/FileNamePolicyServiceTests.cs` | 기존 test 수정 여부 확인 | PASS |
| `FamilyClaimRef.sln` | solution 수정 여부 확인 | PASS |
| `app/FamilyClaimRef.App/FamilyClaimRef.App.csproj` | app project 수정 여부 확인 | PASS |

## C. Implementation Summary

- `app/FamilyClaimRef.App/Models/Storage/` 폴더가 생성되었다.
- `app/FamilyClaimRef.App/Services/Storage/` 폴더가 생성되었다.
- `JsonFileEnvelope<T>`가 구현되었다.
- `DocumentRecord`, `DocumentDraft`가 구현되었다.
- `PolicyDocumentRecord`, `PolicyDocumentDraft`가 구현되었다.
- `ClaimDocumentRecord`, `ClaimDocumentDraft`가 구현되었다.
- `IDocumentStorageService`가 구현되었다.
- `.sln` 수정 없음.
- `.csproj` 수정 없음.
- NuGet package 추가 없음.
- `FileNamePolicyService.cs` 수정 없음.
- 기존 test code 수정 없음.

## D. Model Review

### `JsonFileEnvelope<T>`

확인 결과:

- `SchemaVersion` 포함.
- `SavedAt` 포함.
- `Items` 포함.
- `Items`는 `List<T>` 기준.
- `Items`는 빈 list 기본값으로 null 안전하게 초기화된다.

판정: PASS

### `DocumentRecord`

포함 확인:

- `Id`
- `PhysicalFileName`
- `DisplayTitle`
- `Extension`
- `RelativePath`
- `CreatedAt`
- `UpdatedAt`
- `DisabledAt`

제외 확인:

- `DocumentType`
- `OriginalFileName`
- `IsDisabled`
- `Memo`

판정: PASS

### `DocumentDraft`

포함 확인:

- `PhysicalFileName`
- `DisplayTitle`
- `Extension`
- `RelativePath`

제외 확인:

- `Id`
- `CreatedAt`
- `UpdatedAt`
- `DisabledAt`
- `DocumentType`
- `OriginalFileName`
- `IsDisabled`
- `Memo`

판정: PASS

### `PolicyDocumentRecord`

포함 확인:

- `Id`
- `PolicyId`
- `DocumentId`
- `DocumentType`
- `CreatedAt`
- `UpdatedAt`
- `DisabledAt`

제외 확인:

- `Memo`

판정: PASS

### `PolicyDocumentDraft`

포함 확인:

- `PolicyId`
- `DocumentId`
- `DocumentType`

제외 확인:

- `Id`
- `CreatedAt`
- `UpdatedAt`
- `DisabledAt`
- `Memo`

판정: PASS

### `ClaimDocumentRecord`

포함 확인:

- `Id`
- `ClaimId`
- `DocumentId`
- `DocumentType`
- `CreatedAt`
- `UpdatedAt`
- `DisabledAt`

제외 확인:

- `OcrConfirmedFieldsSnapshot`
- `Memo`

판정: PASS

### `ClaimDocumentDraft`

포함 확인:

- `ClaimId`
- `DocumentId`
- `DocumentType`

제외 확인:

- `Id`
- `CreatedAt`
- `UpdatedAt`
- `DisabledAt`
- `OcrConfirmedFieldsSnapshot`
- `Memo`

판정: PASS

## E. Interface Review

`IDocumentStorageService` 포함 method:

- `GetDocumentsAsync(...)`
- `GetDocumentByIdAsync(...)`
- `AddDocumentAsync(...)`
- `DisableDocumentAsync(...)`
- `GetPolicyDocumentsAsync(...)`
- `AddPolicyDocumentAsync(...)`
- `DisablePolicyDocumentAsync(...)`
- `GetClaimDocumentsAsync(...)`
- `AddClaimDocumentAsync(...)`
- `DisableClaimDocumentAsync(...)`

확인 결과:

- async method 기준이다.
- 모든 method에 `CancellationToken cancellationToken = default`가 포함되어 있다.
- 실제 삭제 method는 없다.
- `Delete...` method는 없다.
- JSON file path/name 노출은 없다.
- raw `originalFileName` input은 없다.
- actual file copy/open method는 없다.
- `Document.documentType` input은 없다.

판정: PASS

## F. Verification Result

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
- 총 테스트 개수: 33
- 실패 테스트: 없음
- 실패 원인: 없음

추가 기록:

- C# model/interface 1차 구현 직후 첫 `dotnet build`는 sandbox가 `C:\Users\jin8855\AppData\Local\Microsoft SDKs` 접근을 막아 실패했다.
- 같은 명령을 권한 상승으로 재실행해 PASS를 확인했다.
- 이번 리뷰 문서 작성 시점에도 `dotnet build FamilyClaimRef.sln`, `dotnet test FamilyClaimRef.sln`을 권한 상승으로 다시 실행했고 모두 PASS를 확인했다.
- 해당 접근 실패는 코드 실패가 아니라 실행 환경 권한 문제로 기록한다.

## G. Out of Scope / Not Implemented

- JSON storage implementation 없음
- 실제 JSON file 생성 없음
- `data/local/*.json` 생성 없음
- SQLite DB/package 추가 없음
- repository/data access/migration 구현 없음
- CategoryItem 구현 없음
- CategoryItem JSON 저장 구현 없음
- document type seed constant 구현 없음
- allowlist/seed consistency test 구현 없음
- actual file copy/storage 구현 없음
- `IFileAttachmentService` 구현 없음
- `ICategorySeedService` 구현 없음
- WPF UI/XAML/navigation/ViewModel 구현 없음
- `attachments/`, `data/local` 내부 파일 생성 없음
- 실제 개인정보 샘플 사용 없음

## H. Risks

- JSON storage implementation은 아직 없다.
- reference validation 구현은 아직 없다.
- schema migration/load failure policy는 아직 없다.
- document type seed constant 구현은 아직 없다.
- allowlist/seed consistency test는 아직 없다.
- actual file attachment service 구현은 아직 없다.
- C# model과 JSON schema 정합성은 현재 build 수준에서만 확인되었다.
- serialization/deserialization test는 아직 없다.

## I. Recommendation

1. 현재 C# model/interface 구현 기준은 build/test PASS 상태로 고정한다.
2. 다음 작업은 document type seed constant 구현 여부 결정 문서가 적절하다.
3. 그 다음 allowlist/seed consistency test 설계를 진행한다.
4. JSON storage implementation은 아직 진행하지 않는다.
5. actual file attachment service 구현은 metadata storage 이후로 보류한다.

## J. Result

`CSHARP_MODEL_INTERFACE_IMPLEMENTATION_REVIEWED`
