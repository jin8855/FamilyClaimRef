# JSON Storage Implementation Review

## A. Goal

이 문서는 JSON storage implementation 구현 결과 리뷰 문서다.

기록 대상은 다음과 같다.

- `JsonFileStore<T>` 구현 결과
- `JsonDocumentStorageService` 구현 결과
- `JsonDocumentStorageServiceTests` 구현 결과
- 구현 범위 준수 여부
- build/test 검증 결과
- 남은 위험과 후속 추천 작업

이 문서는 다음 작업의 리뷰가 아니다.

- actual file attachment copy/storage 구현 리뷰가 아니다.
- WPF UI/ViewModel 구현 리뷰가 아니다.
- SQLite/repository/data access/migration 구현 리뷰가 아니다.
- OCR parsing/storage 구현 리뷰가 아니다.

## B. Checked Files / Paths

| 대상 | 확인 목적 | 판정 |
|---|---|---|
| `docs/86_JSON_STORAGE_IMPLEMENTATION_USER_DECISION_RECORD.md` | JSON storage 구현 사용자 결정 기준 확인 | PASS |
| `docs/85_JSON_STORAGE_IMPLEMENTATION_DESIGN.md` | JSON storage 설계 후보와 테스트 범위 확인 | PASS |
| `docs/84_FULL_EQUALITY_CONSISTENCY_TEST_IMPLEMENTATION_REVIEW.md` | 선행 테스트 기준 확인 | PASS |
| `docs/72_CSHARP_MODEL_INTERFACE_IMPLEMENTATION_REVIEW.md` | storage model/interface 구현 기준 확인 | PASS |
| `docs/69_JSON_SCHEMA_USER_DECISION_RECORD.md` | JSON schema 사용자 결정 확인 | PASS |
| `docs/67_STORAGE_SERVICE_INTERFACE_USER_DECISION_RECORD.md` | storage service interface 방향 확인 | PASS |
| `app/FamilyClaimRef.App/Services/Storage/JsonFileStore.cs` | JSON envelope read/write helper 구현 확인 | PASS |
| `app/FamilyClaimRef.App/Services/Storage/JsonDocumentStorageService.cs` | `IDocumentStorageService` JSON 구현체 확인 | PASS |
| `tests/FamilyClaimRef.App.Tests/JsonDocumentStorageServiceTests.cs` | JSON storage test 구현 확인 | PASS_WITH_NOTES |
| `app/FamilyClaimRef.App/Services/Storage/IDocumentStorageService.cs` | service contract 확인 | PASS |
| `app/FamilyClaimRef.App/Models/Storage/JsonFileEnvelope.cs` | envelope model 확인 | PASS |
| `app/FamilyClaimRef.App/Models/Storage/DocumentRecord.cs` | document metadata record 확인 | PASS |
| `app/FamilyClaimRef.App/Models/Storage/PolicyDocumentRecord.cs` | policy-document link record 확인 | PASS |
| `app/FamilyClaimRef.App/Models/Storage/ClaimDocumentRecord.cs` | claim-document link record 확인 | PASS |
| `app/FamilyClaimRef.App/Models/Storage/DocumentDraft.cs` | document draft 확인 | PASS |
| `app/FamilyClaimRef.App/Models/Storage/PolicyDocumentDraft.cs` | policy-document draft 확인 | PASS |
| `app/FamilyClaimRef.App/Models/Storage/ClaimDocumentDraft.cs` | claim-document draft 확인 | PASS |
| `app/FamilyClaimRef.App/Services/FileNamePolicyService.cs` | documentType allowlist 기준 확인 | PASS |
| `FamilyClaimRef.sln` | solution 구성 확인 | PASS |
| `app/FamilyClaimRef.App/FamilyClaimRef.App.csproj` | app project 구성 확인 | PASS |
| `tests/FamilyClaimRef.App.Tests/FamilyClaimRef.App.Tests.csproj` | test project 구성 확인 | PASS |

## C. Implementation Summary

- `JsonFileStore.cs`가 생성되었다.
- `JsonDocumentStorageService.cs`가 생성되었다.
- `JsonDocumentStorageServiceTests.cs`가 생성되었다.
- `documents.json`, `policy-documents.json`, `claim-documents.json` 분리 저장 구조가 구현되었다.
- `schemaVersion`, `savedAt`, `items` envelope 저장/검증이 구현되었다.
- `Document`, `PolicyDocument`, `ClaimDocument` add/get/disable가 구현되었다.
- disable 시 `DisabledAt`, `UpdatedAt`을 동시에 갱신한다.
- `PolicyDocument` / `ClaimDocument`는 활성 `Document`만 연결할 수 있다.
- `documentType` allowlist 검증은 `FileNamePolicyService.GetAllowedDocumentTypes(...)` 기준으로 적용되었다.
- 손상 JSON, schema mismatch, null items는 `InvalidOperationException`으로 failure 처리된다.
- test는 시스템 temp 하위 임시 디렉터리만 사용한다.
- 실제 `data/local` 파일 생성은 없다.
- 실제 `attachments` 파일 생성은 없다.
- DB/SQLite/JSON 운영 저장 파일 생성은 없다.

## D. JsonFileStore Review

`JsonFileStore<T>`는 JSON file envelope read/write helper로 제한되어 있다.

확인 결과:

- `JsonFileEnvelope<T>` 기준으로 read/write를 수행한다.
- missing file은 empty envelope로 처리한다.
- missing file empty envelope는 `SchemaVersion`을 설정하고 `Items` 기본값을 사용한다.
- save 시 `schemaVersion = 1`을 저장한다.
- save 시 `SavedAt = DateTimeOffset.UtcNow`를 저장한다.
- `items`가 null이면 invalid schema로 보고 `InvalidOperationException`을 발생시킨다.
- invalid JSON은 `JsonException`을 `InvalidOperationException`으로 감싸 명시적으로 failure 처리한다.
- schemaVersion mismatch는 `InvalidOperationException`으로 처리한다.
- temp file write 후 `File.Move(..., overwrite: true)` 방식으로 최종 파일을 교체한다.
- write 실패 후 temp file이 남아 있으면 정리한다.
- document-specific validation을 담당하지 않는다.
- actual file attachment를 다루지 않는다.
- `attachments/`에 접근하지 않는다.
- 실제 개인정보 샘플을 쓰지 않는다.

판정: PASS

보완 후보:

- 현재 temp write는 단일 프로세스 기본 쓰기 안정성을 높이지만, 명시적 multi-process lock/conflict handling은 없다.
- production `data/local` 경로 권한 오류나 디스크 full 상황에 대한 별도 error wrapping은 아직 없다.

## E. JsonDocumentStorageService Review

`JsonDocumentStorageService`는 `IDocumentStorageService` 구현체다.

확인 결과:

- `documents.json`, `policy-documents.json`, `claim-documents.json` metadata read/write를 담당한다.
- root path를 생성자로 주입받아 test temp directory 사용이 가능하다.
- service가 `Id`, `CreatedAt`, `UpdatedAt` 생성 책임을 가진다.
- add 시 `CreatedAt`, `UpdatedAt`을 설정한다.
- disable 시 `DisabledAt`, `UpdatedAt`을 갱신한다.
- 실제 삭제는 없다.
- `DocumentRecord.DocumentType`은 사용하지 않는다.
- raw `OriginalFileName`은 사용하지 않는다.
- `PolicyDocumentRecord.DocumentType`, `ClaimDocumentRecord.DocumentType`이 documentType source of truth다.
- documentType 검증은 `FileNamePolicyService.GetAllowedDocumentTypes(...)` 기준이다.
- policy `capture`는 허용된다.
- claim `capture`, `statement`, `prescription`은 allowlist 밖이라 거부된다.
- `PolicyDocumentDraft.DocumentId`, `ClaimDocumentDraft.DocumentId`는 existing active `DocumentRecord.Id`여야 한다.
- disabled document 연결은 거부된다.
- policyId/claimId 존재 검증은 아직 없다.
- policyId/claimId는 null/empty/whitespace만 거부한다.
- actual file copy/storage는 없다.
- `IFileAttachmentService` 구현은 없다.

판정: PASS_WITH_NOTES

보완 후보:

- `GetDocumentsAsync`, `GetPolicyDocumentsAsync`, `GetClaimDocumentsAsync`는 disabled record도 반환한다. disabled 제외 조회 정책은 아직 별도 API로 분리되지 않았다.
- policyId/claimId가 실제 `Policy` / `ClaimCase`에 존재하는지 확인하는 reference validation은 해당 storage 구현 전까지 보류 상태다.

## F. Test Coverage Review

`JsonDocumentStorageServiceTests.cs`에는 20개의 test가 추가되었다.

### Missing file / empty state

확인 결과:

- missing JSON files에서 empty list 반환 검증이 있다.

판정: Covered

### Add document

확인 결과:

- document add 성공 검증이 있다.
- id/time 생성 검증이 있다.
- `DisabledAt` null 검증이 있다.
- JSON file persist 검증이 있다.
- service 재생성 후 load 가능 검증이 있다.

판정: Covered

### Get by id

확인 결과:

- existing document id 조회 성공은 add document persist/reload test에서 함께 검증된다.
- missing id null 반환 검증이 있다.

판정: Covered

### Disable document

확인 결과:

- `DisabledAt` 설정 검증이 있다.
- `UpdatedAt` 갱신 검증이 있다.
- 저장 후 재로드 시 disabled 상태 유지 검증이 있다.

판정: Covered

### Policy document link

확인 결과:

- existing active documentId로 add 성공 검증이 있다.
- missing documentId rejected 검증이 있다.
- disabled documentId rejected 검증이 있다.
- invalid policy documentType rejected 검증이 있다.
- policy `capture` accepted 검증이 있다.
- missing/empty policyId rejected 명시 테스트는 없다.

판정: PASS_WITH_NOTES

보완 후보:

- `PolicyDocumentDraft.PolicyId` null/empty/whitespace rejection test를 별도로 추가할 수 있다.

### Claim document link

확인 결과:

- existing active documentId로 add 성공 검증이 있다.
- missing documentId rejected 검증이 있다.
- disabled documentId rejected 검증이 있다.
- invalid claim documentType rejected 검증이 있다.
- claim `capture` rejected 검증이 있다.
- current claim type accepted 검증이 있다.
- missing/empty claimId rejected 명시 테스트는 없다.

판정: PASS_WITH_NOTES

보완 후보:

- `ClaimDocumentDraft.ClaimId` null/empty/whitespace rejection test를 별도로 추가할 수 있다.
- claim `statement`, `prescription` rejected test는 현재 직접 포함되어 있지 않다. allowlist 기준상 거부되지만 명시 테스트는 보완 후보다.

### Envelope/schema

확인 결과:

- `schemaVersion = 1` 저장 검증이 있다.
- `savedAt` 저장 검증이 있다.
- invalid JSON load failure 검증이 있다.
- schemaVersion mismatch failure 검증이 있다.
- null items failure 검증이 있다.

판정: Covered

### Excluded tests

아래 테스트는 제외 상태다.

- actual file copy/open test
- UI/ViewModel test
- OCR parsing/storage test
- SQLite migration test
- CategoryItem JSON storage test
- 동시 쓰기/lock conflict test
- production `data/local` 권한 test

## G. Verification Result

검증 명령:

```powershell
dotnet build C:\EtcProject\FamilyClaimRef\FamilyClaimRef.sln
dotnet test C:\EtcProject\FamilyClaimRef\FamilyClaimRef.sln
```

이번 리뷰 문서 작성 시점의 최신 검증 결과:

- `dotnet build C:\EtcProject\FamilyClaimRef\FamilyClaimRef.sln`: PASS
- warning: 0
- error: 0
- `dotnet test C:\EtcProject\FamilyClaimRef\FamilyClaimRef.sln`: PASS
- 총 테스트 개수: 89
- 통과: 89
- 실패: 0
- 건너뜀: 0
- 추가 테스트: 20개
- 권한 상승 실행: 있음
- 이번 리뷰 검증 명령의 초기 실패/재실행: 없음
- 테스트 저장 위치: 시스템 temp 하위 임시 디렉터리만 사용하고 삭제
- 실제 `data/local` 파일 생성 없음
- 실제 `attachments` 파일 생성 없음
- DB/SQLite/JSON 운영 저장 파일 미생성

구현 당시 검증 기록:

- 최초 빌드는 sandbox가 `C:\Users\jin8855\AppData\Local\Microsoft SDKs` 접근을 제한해 실패했다.
- 권한 상승으로 빌드를 재실행했다.
- 초기 컴파일 오류는 `System.IO` 명시 import 누락이었다.
- `System.IO` import 보정 후 build/test가 통과했다.

Git 상태:

- 현재 경로는 Git 저장소가 아니어서 `git status` 조회는 실패하는 상태로 기록되어 있다.

## H. Scope Compliance Review

아래 범위는 지켜졌다.

- `FileNamePolicyService.cs` 수정 없음.
- `DocumentTypeSeed.cs` 수정 없음.
- `DocumentTypeSeeds.cs` 수정 없음.
- `IDocumentStorageService.cs` 수정 없음.
- `.sln` 수정 없음.
- `.csproj` 수정 없음.
- NuGet package 추가 없음.
- WPF UI/ViewModel 구현 없음.
- SQLite/DB/OCR/file copy 구현 없음.
- actual file copy/storage 구현 없음.
- `IFileAttachmentService` 구현 없음.
- repository/data access/migration 구현 없음.
- CategoryItem JSON storage 구현 없음.
- `attachments/`, `data/local` 내부 파일 생성 없음.
- 실제 개인정보 샘플 사용 없음.
- Git commit/reset/checkout/add 없음.

## I. Out of Scope / Not Implemented

아래 항목은 아직 구현되지 않았다.

- 동시 쓰기 lock/conflict handling 없음.
- actual file copy/storage 없음.
- metadata 저장과 actual file copy 간 transaction boundary 없음.
- `IFileAttachmentService` 없음.
- Policy/Claim storage가 없어 policyId/claimId 존재 검증 없음.
- schema migration 없음.
- CategoryItem JSON storage 없음.
- SQLite implementation 없음.
- WPF UI/ViewModel integration 없음.
- OCR parsing/storage 없음.

## J. Risks

- 동시 쓰기 잠금/충돌 처리는 MVP 1차 범위 밖이다.
- 실제 파일 복사와 metadata 저장의 transaction boundary는 아직 구현되지 않았다.
- policyId/claimId 존재 검증은 아직 없다.
- schema migration은 아직 없다.
- JSON storage API는 UI 구현 전 변경될 수 있다.
- actual file attachment service와 metadata storage가 나중에 연결될 때 실패 rollback 정책이 필요하다.
- temp directory test는 실제 production `data/local` 경로 권한 문제를 완전히 검증하지는 않는다.
- disabled record 포함 조회와 제외 조회 정책은 UI 요구사항에 따라 추가 API가 필요할 수 있다.

## K. Recommendation

1. 현재 JSON storage implementation은 build/test PASS 상태로 고정한다.
2. 다음 작업은 `IFileAttachmentService` 범위 결정 문서가 적절하다.
3. actual file copy/storage 구현 전 transaction boundary를 먼저 정리한다.
4. metadata JSON storage와 actual file storage를 분리한 상태를 유지한다.
5. UI/ViewModel 연동은 file attachment service 이후로 보류한다.
6. 정책 입력값 rejection 보강이 필요하면 policyId/claimId empty case와 claim `statement` / `prescription` rejection test를 추가 후보로 둔다.

## L. Result

`JSON_STORAGE_IMPLEMENTATION_REVIEWED`
