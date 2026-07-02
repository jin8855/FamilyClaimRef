# JSON Storage Implementation User Decision Record

## A. Goal

이 문서는 `docs/85_JSON_STORAGE_IMPLEMENTATION_DESIGN.md`의 Needs Decision Q1~Q18에 대한 사용자 결정 기록이다.

목적은 FamilyClaimRef MVP의 JSON storage implementation 방향을 확정하고, 후속 구현 범위와 제외 범위를 명확히 하는 것이다.

이 문서는 구현 문서가 아니다. C# 구현, JSON storage 구현, 실제 JSON file 생성, test code 구현은 수행하지 않는다.

## B. Checked Files / Paths

| 대상 | 확인 목적 | 비고 |
|---|---|---|
| `docs/85_JSON_STORAGE_IMPLEMENTATION_DESIGN.md` | Needs Decision Q1~Q18 확인 | 읽기 전용 |
| `docs/84_FULL_EQUALITY_CONSISTENCY_TEST_IMPLEMENTATION_REVIEW.md` | full equality consistency test 구현 결과 확인 | 읽기 전용 |
| `docs/81_FILENAME_POLICY_ALLOWLIST_ACCESSOR_IMPLEMENTATION_REVIEW.md` | allowlist accessor 구현 결과 확인 | 읽기 전용 |
| `docs/72_CSHARP_MODEL_INTERFACE_IMPLEMENTATION_REVIEW.md` | storage model/interface 구현 결과 확인 | 읽기 전용 |
| `docs/69_JSON_SCHEMA_USER_DECISION_RECORD.md` | JSON schema 사용자 결정 확인 | 읽기 전용 |
| `docs/67_STORAGE_SERVICE_INTERFACE_USER_DECISION_RECORD.md` | storage service interface 사용자 결정 확인 | 읽기 전용 |
| `app/FamilyClaimRef.App/Models/Storage/` | storage model 파일 확인 | 읽기 전용 |
| `app/FamilyClaimRef.App/Services/Storage/` | storage service interface 확인 | 읽기 전용 |
| `app/FamilyClaimRef.App/Services/FileNamePolicyService.cs` | documentType allowlist accessor 기준 확인 | 읽기 전용 |
| `tests/FamilyClaimRef.App.Tests/` | 기존 test project 기준 확인 | 읽기 전용 |
| `FamilyClaimRef.sln` | solution 기준 확인 | 읽기 전용 |
| `app/FamilyClaimRef.App/FamilyClaimRef.App.csproj` | app project 기준 확인 | 읽기 전용 |
| `tests/FamilyClaimRef.App.Tests/FamilyClaimRef.App.Tests.csproj` | test project 기준 확인 | 읽기 전용 |

## C. User Decision Summary

| ID | Question | Decision | Recorded Direction |
|---|---|---|---|
| Q1 | JSON storage implementation은 helper + service 분리 방향으로 갈 것인가? | Accepted | JSON storage implementation은 helper + service 분리 방향으로 간다. 반복되는 envelope read/write 책임과 document-specific storage service 책임을 분리한다. |
| Q2 | `JsonFileStore<T>` 같은 envelope read/write helper를 둘 것인가? | Accepted | `JsonFileStore<T>` 또는 유사 helper를 둔다. helper는 JSON envelope read/write, missing file empty initialization, schemaVersion, savedAt 처리 후보를 담당한다. document-specific validation은 service가 담당한다. |
| Q3 | `JsonDocumentStorageService`가 `IDocumentStorageService`를 구현하게 할 것인가? | Accepted | `JsonDocumentStorageService`가 `IDocumentStorageService`를 구현한다. ViewModel/UI는 JSON file path/name을 알지 않는다. JSON file 단위는 implementation detail로 유지한다. |
| Q4 | actual file attachment copy/storage는 계속 제외할 것인가? | Accepted | actual file attachment copy/storage는 계속 제외한다. `IFileAttachmentService` 구현은 이번 범위에서 제외한다. 이번 구현 후보는 metadata JSON storage까지만 다룬다. |
| Q5 | metadata root는 `data/local/` 후보를 유지할 것인가? | Accepted | metadata root 후보는 `data/local/`로 유지한다. production 구현에서 실제 JSON file 생성이 가능해지는 범위는 별도 구현 승인으로 제한한다. |
| Q6 | implementation은 root path를 생성자 등으로 주입받아 test temp directory를 사용할 수 있게 할 것인가? | Accepted | JSON storage implementation은 root path를 생성자 등으로 주입받는 구조로 간다. test에서는 temp directory를 사용한다. production path와 test path를 분리한다. |
| Q7 | file missing은 empty envelope로 처리할 것인가? | Accepted | file missing은 empty envelope로 처리한다. 최초 실행 시 metadata file이 없어도 empty list로 시작할 수 있어야 한다. |
| Q8 | schemaVersion mismatch/invalid JSON은 명시적 failure로 처리할 것인가? | Accepted | schemaVersion mismatch, schemaVersion missing, invalid JSON은 명시적 failure로 처리한다. migration은 MVP 1차에서 구현하지 않는다. |
| Q9 | service가 id/time 생성 책임을 가질 것인가? | Accepted | service가 `Id`, `CreatedAt`, `UpdatedAt` 생성 책임을 가진다. Draft는 저장 요청 입력이고 Record는 저장된 상태다. Draft에는 id/time을 요구하지 않는다. |
| Q10 | disable 시 `DisabledAt`과 `UpdatedAt`을 갱신할 것인가? | Accepted | disable 시 `DisabledAt`과 `UpdatedAt`을 갱신한다. 실제 삭제는 하지 않는다. `IsDisabled`는 저장하지 않고 `DisabledAt != null`에서 파생한다. |
| Q11 | envelope `SavedAt`은 save마다 갱신할 것인가? | Accepted | envelope `SavedAt`은 save마다 갱신한다. `SavedAt`은 UTC `DateTimeOffset` 기준으로 둔다. |
| Q12 | 가능하면 atomic write를 사용할 것인가? | Accepted | 가능한 경우 temp file write 후 replace 방식의 atomic write를 사용한다. 플랫폼 제약이 있으면 구현 단계에서 제한 사항을 보고한다. |
| Q13 | documentId reference validation을 포함할 것인가? | Accepted | `PolicyDocumentDraft.DocumentId`와 `ClaimDocumentDraft.DocumentId`는 existing active `DocumentRecord.Id`여야 한다. missing documentId는 거부한다. |
| Q14 | disabled document 연결은 거부할 것인가? | Accepted | disabled `DocumentRecord`는 신규 `PolicyDocumentRecord` 또는 `ClaimDocumentRecord`에 연결할 수 없다. 기존 연결 처리 정책은 후속 정책에서 다룰 수 있다. |
| Q15 | policyId/claimId 존재 검증은 policies/claims storage 전까지 보류할 것인가? | Accepted - Deferred | policyId/claimId 존재 검증은 policies/claims storage 구현 전까지 보류한다. 현재는 문자열 필수값 검증 후보까지만 둔다. 이후 Policy/Claim storage가 구현되면 reference validation을 강화한다. |
| Q16 | documentType 검증은 `FileNamePolicyService.GetAllowedDocumentTypes(...)` 기준으로 할 것인가? | Accepted | documentType 검증은 `FileNamePolicyService.GetAllowedDocumentTypes(...)` 기준으로 한다. policy documentType은 policy allowlist 기준, claim documentType은 claim allowlist 기준이다. |
| Q17 | JSON storage 구현 후 test project에 JSON storage tests를 추가할 것인가? | Accepted | JSON storage 구현 후 test project에 JSON storage tests를 추가한다. test는 temp directory 기준으로 작성한다. 실제 `data/local/*.json` 파일은 test에서 만들지 않는다. |
| Q18 | 실제 `data/local/*.json` 파일 생성은 production 구현에서만 가능하고 test에서는 temp directory만 사용할 것인가? | Accepted | 실제 `data/local/*.json` 파일 생성은 production 구현에서만 가능하게 한다. test에서는 temp directory만 사용한다. 테스트가 프로젝트 `data/local`이나 실제 사용자 데이터 위치를 오염시키면 안 된다. |

## D. Accepted JSON Storage Direction

후속 구현 방향은 아래와 같이 확정한다.

- helper + service 분리 방향으로 간다.
- `JsonFileStore<T>` helper 후보를 포함한다.
- `JsonDocumentStorageService`가 `IDocumentStorageService`를 구현한다.
- actual file attachment copy/storage는 제외한다.
- metadata root 후보는 `data/local/`로 유지한다.
- root path는 생성자 등으로 주입한다.
- test는 temp directory를 사용한다.
- file missing은 empty envelope로 처리한다.
- schemaVersion mismatch/invalid JSON은 명시적 failure로 처리한다.
- service가 id/time 생성 책임을 가진다.
- disable 시 `DisabledAt`, `UpdatedAt`을 갱신한다.
- envelope `SavedAt`은 save마다 갱신한다.
- 가능한 경우 atomic write를 사용한다.
- documentId reference validation을 포함한다.
- disabled document 연결은 거부한다.
- policyId/claimId 존재 검증은 보류한다.
- documentType 검증은 `FileNamePolicyService.GetAllowedDocumentTypes(...)` 기준으로 한다.
- JSON storage tests를 추가한다.
- test는 temp directory만 사용한다.
- actual `data/local/*.json` 생성은 production 구현에서만 가능하게 한다.

## E. Implementation Candidate Files

후속 구현 후보 파일:

- `app/FamilyClaimRef.App/Services/Storage/JsonFileStore.cs`
- `app/FamilyClaimRef.App/Services/Storage/JsonDocumentStorageService.cs`
- `tests/FamilyClaimRef.App.Tests/JsonDocumentStorageServiceTests.cs`

주의:

- 이 문서에서는 위 파일을 생성하지 않는다.
- 실제 파일 생성은 별도 구현 승인 후 진행한다.
- 구현 단계에서도 production C# 외 범위, test file 범위, 실제 `data/local` 파일 생성 여부를 분리해 보고해야 한다.

## F. Still Not Implemented

아래 항목은 아직 구현되지 않았다.

- C# 구현 없음
- JSON storage implementation 없음
- 실제 JSON file 생성 없음
- test code 구현 없음
- test file 생성 없음
- SQLite DB/package 추가 없음
- repository/data access/migration 구현 없음
- actual file copy/storage 구현 없음
- `IFileAttachmentService` 구현 없음
- WPF UI/XAML/navigation/ViewModel 구현 없음
- `attachments/` 내부 파일 생성 없음
- `data/local` 내부 파일 생성 없음
- 실제 개인정보 샘플 없음

## G. Next Step

다음 작업 후보:

1. 별도 승인 후 JSON storage implementation 구현
2. 구현 파일 후보:
   - `JsonFileStore.cs`
   - `JsonDocumentStorageService.cs`
   - `JsonDocumentStorageServiceTests.cs`
3. 구현 후 `dotnet build FamilyClaimRef.sln`, `dotnet test FamilyClaimRef.sln` 실행
4. 구현 후 `docs/87_JSON_STORAGE_IMPLEMENTATION_REVIEW.md` 생성
5. actual file attachment service와 UI/ViewModel은 이후 단계로 보류

## H. Result

`JSON_STORAGE_IMPLEMENTATION_USER_DECISION_RECORDED`
