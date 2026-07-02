# JSON Storage Implementation Design

## A. Goal

이 문서는 FamilyClaimRef MVP의 JSON storage implementation 전 구현 범위와 정책을 결정하기 위한 설계 문서다.

목적은 `IDocumentStorageService`의 JSON 구현체 후보를 설계하고, 파일 단위, service 책임, validation 책임, load/save/error 정책, 테스트 범위를 정리하는 것이다.

이 문서는 실제 구현 문서가 아니다.

- C# 구현을 수행하지 않는다.
- JSON storage implementation을 수행하지 않는다.
- 실제 JSON file을 생성하지 않는다.
- test code를 구현하지 않는다.
- repository/data access/migration을 구현하지 않는다.
- actual file copy/storage를 구현하지 않는다.
- WPF UI/ViewModel을 구현하지 않는다.

## B. Current State

- WPF scaffold 생성은 완료되어 있다.
- TargetFramework는 `net10.0-windows`이다.
- `FileNamePolicyService` 구현은 완료되어 있다.
- C# storage model/interface 구현은 완료되어 있다.
- `JsonFileEnvelope<T>` 구현은 완료되어 있다.
- `DocumentRecord`, `PolicyDocumentRecord`, `ClaimDocumentRecord` 구현은 완료되어 있다.
- `DocumentDraft`, `PolicyDocumentDraft`, `ClaimDocumentDraft` 구현은 완료되어 있다.
- `IDocumentStorageService` 구현은 완료되어 있다.
- document type seed constant 구현은 완료되어 있다.
- allowlist/seed black-box consistency test 구현은 완료되어 있다.
- allowlist accessor 구현은 완료되어 있다.
- full equality consistency test 구현은 완료되어 있다.
- build/test는 PASS 상태로 기록되어 있다.
- 총 테스트 수는 69개다.
- JSON storage implementation은 아직 없다.
- 실제 JSON file 생성은 아직 없다.
- repository/data access/migration 구현은 아직 없다.
- WPF UI/ViewModel 구현은 아직 없다.
- `attachments/`, `data/local` 내부 파일 생성은 없다.

선행 결정:

- MVP 1차 저장 방식은 JSON file storage다.
- JSON metadata file은 3개 분리 구조다.
  - `data/local/documents.json`
  - `data/local/policy-documents.json`
  - `data/local/claim-documents.json`
- 각 JSON file은 `envelope + items` 구조다.
- 각 envelope에는 `schemaVersion`, `savedAt`을 포함한다.
- 초기 `schemaVersion = 1`이다.
- `savedAt`은 UTC ISO-8601 기준이다.
- `DocumentRecord.DocumentType`은 제외한다.
- raw `OriginalFileName`은 제외한다.
- `IsDisabled`는 저장하지 않고 `DisabledAt != null`에서 파생한다.
- `PolicyDocumentRecord.DocumentType`은 보험 문서 documentType source of truth다.
- `ClaimDocumentRecord.DocumentType`은 청구 문서 documentType source of truth다.
- `OcrConfirmedFieldsSnapshot`은 제외한다.
- `Memo`는 MVP 1차에서 제외한다.
- `IDocumentStorageService`는 async method + `CancellationToken` 기준이다.
- 실제 삭제 method는 없다.
- `Disable...` method만 후보로 둔다.
- JSON file path/name은 interface에 노출하지 않는다.
- JSON storage implementation 전 reference validation/load failure policy 설계가 필요하다.

## C. Implementation Problem Statement

JSON metadata file을 실제로 읽고 쓰는 implementation이 필요하다.

다만 file I/O를 시작하면 아래 정책이 구현 결과에 직접 영향을 준다.

- `data/local/*.json` 실제 파일 생성 시점
- app data root 기준
- test temp directory 사용 방식
- `schemaVersion` 검증 방식
- invalid JSON load failure policy
- missing file 처리 방식
- duplicate id 처리 방식
- documentId reference validation
- disabled document 연결 허용 여부
- documentType validation 기준
- save 시 `savedAt`, `createdAt`, `updatedAt`, `disabledAt` 갱신 책임
- atomic write 여부

또한 metadata storage와 actual file copy/storage는 분리해야 한다.

ViewModel은 JSON file path/name, JSON DTO, schema detail을 직접 알면 안 된다.

## D. Candidate Implementation Options

### Candidate 1. 단순 JSON storage implementation

내용:

- `JsonDocumentStorageService`를 구현한다.
- `documents.json`, `policy-documents.json`, `claim-documents.json`을 읽고 쓴다.
- 파일이 없으면 빈 envelope로 시작한다.
- add/disable method 중심으로 구현한다.
- reference validation은 최소화하거나 후속으로 보류한다.

장점:

- MVP 구현 속도가 빠르다.
- 현재 interface와 schema를 빠르게 검증할 수 있다.

단점:

- 잘못된 reference 저장 위험이 있다.
- schemaVersion/load failure 정책이 약할 수 있다.
- 파일 쓰기 실패 또는 부분 실패 대응이 약할 수 있다.

### Candidate 2. validation 포함 JSON storage implementation

내용:

- `JsonDocumentStorageService`를 구현한다.
- JSON file read/write를 수행한다.
- `documentId` reference validation을 포함한다.
- `schemaVersion` validation을 포함한다.
- disabled item 기본 제외/포함 정책을 결정한다.
- 가능한 atomic write를 포함한다.

장점:

- 저장 정합성이 높다.
- 이후 UI/ViewModel에서 오류가 줄어든다.
- SQLite 전환 시 안정적인 기준이 된다.

단점:

- 구현 범위가 커진다.
- 테스트 범위가 커진다.
- 아직 없는 Policy/Claim storage 때문에 일부 validation은 보류가 필요하다.

### Candidate 3. JSON storage helper + service 분리

내용:

- `JsonFileStore<T>` 같은 generic helper를 둔다.
- `JsonDocumentStorageService`가 helper를 사용한다.
- envelope read/write, schemaVersion, savedAt, missing file empty initialization은 helper가 담당한다.
- document-specific validation은 service가 담당한다.

장점:

- 반복 코드가 줄어든다.
- 향후 `FamilyMember`, `Policy`, `Claim` storage 확장에 유리하다.
- 테스트 가능성이 높다.

단점:

- abstraction이 하나 더 생긴다.
- MVP 초기에는 설계 부담이 늘 수 있다.

## E. Recommended Direction

Candidate Recommendation:

- MVP 1차는 Candidate 3 방향이 가장 낫다.
- helper는 작고 제한적으로 둔다.
- `JsonFileStore<T>` 또는 유사 helper는 envelope read/write만 담당한다.
- `JsonDocumentStorageService`는 `IDocumentStorageService` 구현체로 둔다.
- document-specific validation은 service가 담당한다.
- actual file copy/storage는 구현하지 않는다.
- `IFileAttachmentService`는 아직 구현하지 않는다.
- 초기 구현에서 `documents.json`, `policy-documents.json`, `claim-documents.json` 실제 파일이 생성될 수 있으므로 별도 사용자 승인 후 진행한다.
- 이 문서에서는 구현하지 않는다.

이 추천은 확정이 아니라 `Candidate Recommendation`이다.

## F. File Path / Root Candidate

후보 파일 단위:

- metadata root: `data/local/`
- documents file: `data/local/documents.json`
- policy documents file: `data/local/policy-documents.json`
- claim documents file: `data/local/claim-documents.json`

결정 필요:

- 상대 경로 기준을 app working directory로 할지, app base directory로 할지, user data directory로 할지 결정해야 한다.
- MVP에서 프로젝트 root 기준으로 둘지, 실행 파일 기준으로 둘지 결정해야 한다.
- test에서는 temp directory를 사용할지 결정해야 한다.

추천 후보:

- production implementation은 app data root abstraction을 주입받는 방식으로 설계한다.
- test에서는 temp directory를 사용한다.
- UI/ViewModel에는 경로를 노출하지 않는다.
- `IDocumentStorageService` interface에도 JSON file path/name을 노출하지 않는다.

## G. Load Policy Candidate

후보:

| 상황 | 처리 후보 |
|---|---|
| file missing | empty envelope로 처리 |
| invalid JSON | 명시적 failure |
| schemaVersion mismatch | MVP 1차에서는 unsupported error |
| schemaVersion missing | unsupported error |
| items null | empty list 처리 또는 invalid schema |

추천 후보:

- file missing은 empty envelope로 처리한다.
- invalid JSON은 명시적 exception 또는 storage error로 처리한다.
- schemaVersion mismatch는 MVP 1차에서 unsupported error로 처리한다.
- schemaVersion missing은 unsupported error로 처리한다.
- migration은 MVP 1차에서 구현하지 않는다.
- items null은 invalid schema로 보는 쪽이 안전하다.

## H. Save Policy Candidate

후보:

- add document 시 service가 `Id`, `CreatedAt`, `UpdatedAt`을 생성할지 결정해야 한다.
- update/disable 시 `DisabledAt` 설정과 `UpdatedAt` 갱신 여부를 결정해야 한다.
- 저장 시 envelope `SavedAt`을 갱신해야 한다.
- duplicate id는 저장 거부해야 한다.
- 가능한 경우 temp file write 후 replace 방식의 atomic write를 사용한다.

추천 후보:

- service가 id/time 생성 책임을 가진다.
- add 시 `CreatedAt`, `UpdatedAt`을 같은 UTC timestamp로 둔다.
- disable 시 `DisabledAt`과 `UpdatedAt`을 갱신한다.
- envelope `SavedAt`은 save마다 갱신한다.
- duplicate id는 명시적으로 거부한다.
- atomic write는 가능하면 포함한다.

## I. Reference Validation Candidate

후보:

- `PolicyDocumentDraft.DocumentId`는 existing active `DocumentRecord.Id`여야 한다.
- `ClaimDocumentDraft.DocumentId`는 existing active `DocumentRecord.Id`여야 한다.
- disabled `Document` 연결 허용 여부를 결정해야 한다.
- `PolicyId`, `ClaimId`는 아직 policies/claims storage가 없으므로 검증 보류 후보이다.
- `DocumentType`은 `FileNamePolicyService.GetAllowedDocumentTypes(scope)` 기준으로 검증할 수 있다.

추천 후보:

- documentId 존재 여부 검증은 포함한다.
- disabled document 연결은 MVP 1차에서 거부한다.
- policyId/claimId 존재 검증은 policies/claims storage 구현 전까지 보류한다.
- documentType 검증은 `FileNamePolicyService.GetAllowedDocumentTypes(...)` 기준으로 포함한다.
- claim `capture`, `statement`, `prescription`은 현재 claim documentType으로 허용하지 않는다.
- policy `capture`는 현재 policy documentType으로 허용한다.

## J. Test Scope Candidate

후속 구현 시 테스트 후보:

- missing file returns empty list
- add document creates record
- add document persists to JSON file
- get document by id works
- disable document sets `DisabledAt`
- disable document updates `UpdatedAt`
- save writes `schemaVersion`
- save writes `savedAt`
- add policy document rejects missing documentId
- add claim document rejects missing documentId
- add policy document rejects disabled documentId
- add claim document rejects disabled documentId
- add policy document rejects invalid documentType
- add claim document rejects invalid documentType
- policy `capture` documentType is accepted
- claim `capture` documentType is rejected
- invalid JSON load fails predictably
- schemaVersion mismatch fails predictably
- existing 69 tests remain PASS

제외 테스트:

- 실제 첨부 파일 copy/open test
- UI/ViewModel test
- OCR parsing/storage test
- SQLite migration test
- CategoryItem JSON storage test

## K. Needs Decision

후속 사용자 결정이 필요한 항목:

1. JSON storage implementation은 helper + service 분리 방향으로 갈 것인가?
2. `JsonFileStore<T>` 같은 envelope read/write helper를 둘 것인가?
3. `JsonDocumentStorageService`가 `IDocumentStorageService`를 구현하게 할 것인가?
4. actual file attachment copy/storage는 계속 제외할 것인가?
5. metadata root는 `data/local/` 후보를 유지할 것인가?
6. implementation은 root path를 생성자 등으로 주입받아 test temp directory를 사용할 수 있게 할 것인가?
7. file missing은 empty envelope로 처리할 것인가?
8. schemaVersion mismatch/invalid JSON은 명시적 failure로 처리할 것인가?
9. service가 id/time 생성 책임을 가질 것인가?
10. disable 시 `DisabledAt`과 `UpdatedAt`을 갱신할 것인가?
11. envelope `SavedAt`은 save마다 갱신할 것인가?
12. 가능하면 atomic write를 사용할 것인가?
13. documentId reference validation을 포함할 것인가?
14. disabled document 연결은 거부할 것인가?
15. policyId/claimId 존재 검증은 policies/claims storage 전까지 보류할 것인가?
16. documentType 검증은 `FileNamePolicyService.GetAllowedDocumentTypes(...)` 기준으로 할 것인가?
17. JSON storage 구현 후 test project에 JSON storage tests를 추가할 것인가?
18. 실제 `data/local/*.json` 파일 생성은 production 구현에서만 가능하고 test에서는 temp directory만 사용할 것인가?

## L. Out of Scope

이번 문서에서 제외하는 범위는 다음과 같다.

- C# 구현 없음
- C# 수정 없음
- production C# 수정 없음
- test code 구현 없음
- test file 생성 없음
- JSON storage implementation 없음
- 실제 JSON file 생성 없음
- SQLite DB/package 추가 없음
- repository/data access/migration 구현 없음
- actual file copy/storage 구현 없음
- `IFileAttachmentService` 구현 없음
- WPF UI/XAML/navigation/ViewModel 구현 없음
- `attachments/` 내부 파일 생성 없음
- `data/local` 내부 파일 생성 없음
- 실제 개인정보 샘플 없음

## M. Risks

- JSON storage 구현을 시작하면 실제 파일 생성 정책이 중요해진다.
- app working directory 기준을 잘못 잡으면 실행 환경에 따라 데이터 위치가 달라질 수 있다.
- reference validation이 너무 약하면 잘못된 연결 record가 저장될 수 있다.
- validation이 너무 많으면 아직 없는 Policy/Claim storage 때문에 구현이 막힐 수 있다.
- atomic write를 생략하면 파일 손상 위험이 커진다.
- exception policy가 불명확하면 UI error state와 연결하기 어렵다.
- JSON storage를 먼저 구현하면 아직 UI가 없는 상태에서 API가 변경될 수 있다.
- metadata storage와 actual file copy/storage를 섞으면 이후 책임 분리가 어려워진다.

## N. Recommendation

1. 이 문서를 기준으로 JSON storage implementation 방향 결정을 받는다.
2. 사용자 결정 후 `docs/86_JSON_STORAGE_IMPLEMENTATION_USER_DECISION_RECORD.md`를 생성한다.
3. 그 다음 별도 승인으로 JSON storage implementation을 구현한다.
4. 구현 시 test는 temp directory 기준으로 작성한다.
5. 구현 후 `docs/87_JSON_STORAGE_IMPLEMENTATION_REVIEW.md`를 생성한다.
6. 실제 UI/ViewModel과 actual file attachment service는 이후 단계로 둔다.

## O. Result

`JSON_STORAGE_IMPLEMENTATION_DESIGN_DRAFTED`
