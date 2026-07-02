# Storage Service Interface User Decision Record

## A. Goal

이 문서는 `docs/66_STORAGE_SERVICE_INTERFACE_DESIGN.md`의 Needs Decision Q1~Q12에 대한 사용자 결정 기록이다.

목적은 FamilyClaimRef MVP의 storage service interface 방향을 확정하고, 이후 JSON schema 초안, C# model/interface 구현 승인, JSON file storage 구현 승인 여부를 판단하기 위한 기준을 제공하는 것이다.

이 문서는 구현 문서가 아니다. 실제 C# interface/model/storage 구현, JSON 저장 구현, repository 구현, DB/OCR/metadata/file storage 구현은 수행하지 않는다.

## B. Checked Files / Paths

| 대상 | 확인 목적 | 비고 |
|---|---|---|
| `docs/66_STORAGE_SERVICE_INTERFACE_DESIGN.md` | Q1~Q12 Needs Decision과 interface 후보 확인 | 읽기 전용 |
| `docs/65_CATEGORY_ITEM_DOCUMENT_TYPE_USER_DECISION_RECORD.md` | documentType source of truth와 CategoryItem seed 결정 확인 | 읽기 전용 |
| `docs/63_DOCUMENT_STORAGE_STRUCTURE_USER_DECISION_RECORD.md` | `Document`, `PolicyDocument`, `ClaimDocument` 저장 구조 결정 확인 | 읽기 전용 |
| `docs/61_LOCAL_STORAGE_USER_DECISION_RECORD.md` | JSON file storage와 storage interface 선설계 결정 확인 | 읽기 전용 |
| `docs/59_FILENAME_POLICY_TEST_PROJECT_IMPLEMENTATION_REVIEW.md` | `FileNamePolicyService` 테스트 PASS 기록 확인 | 읽기 전용 |
| `docs/43_WPF_MINIMAL_MVVM_STRUCTURE_DESIGN.md` | ViewModel/Model/Service 책임 후보 확인 | 읽기 전용 |
| `app/FamilyClaimRef.App/Services/FileNamePolicyService.cs` | 기존 production service 확인 | 읽기 전용 |
| `tests/FamilyClaimRef.App.Tests/FileNamePolicyServiceTests.cs` | 기존 테스트 파일 확인 | 읽기 전용 |
| `app/FamilyClaimRef.App/Models/` | model 구현 여부 확인 | 수정 없음 |
| `app/FamilyClaimRef.App/Services/` | service 구현 범위 확인 | 수정 없음 |
| `app/FamilyClaimRef.App/ViewModels/` | ViewModel 구현 여부 확인 | 수정 없음 |

## C. User Decision Summary

| ID | Question | Decision | Recorded Direction |
|---|---|---|---|
| Q1 | MVP 1차 storage interface는 facade + 내부 도메인 책임 분리 방향으로 갈 것인가? | Accepted | ViewModel 접근 지점은 단순하게 유지하고, 내부에서는 document/policy/claim 등 도메인별 책임을 분리한다. JSON 파일 단위는 implementation detail로 숨긴다. |
| Q2 | ViewModel은 JSON 파일 경로나 JSON DTO를 직접 알지 않게 할 것인가? | Accepted | ViewModel은 service contract와 화면 상태 model에만 의존한다. JSON에서 SQLite로 전환해도 ViewModel 변경을 최소화한다. |
| Q3 | `IDocumentStorageService`를 먼저 설계 대상으로 둘 것인가? | Accepted | `Document`, `PolicyDocument`, `ClaimDocument` metadata 저장과 연결 record 관리를 첫 설계 범위로 둔다. |
| Q4 | actual file copy/open은 `IFileAttachmentService` 후보로 분리하고 구현은 보류할 것인가? | Accepted - Deferred Implementation | actual file copy/open은 후보 service로 분리하고, 실제 파일 복사/열기/저장은 별도 승인 전까지 보류한다. |
| Q5 | CategoryItem seed 조회/검증은 `ICategorySeedService` 후보로 분리하고 구현은 보류할 것인가? | Accepted - Deferred Implementation | CategoryItem seed 조회/검증은 후보 service로 분리하고, 실제 구현은 별도 승인 전까지 보류한다. |
| Q6 | interface는 async method 후보로 둘 것인가? | Accepted | JSON file storage와 SQLite 전환 가능성을 고려해 async contract 후보로 둔다. |
| Q7 | 실제 삭제 method는 MVP 1차에서 제외하고 `Disable...` 계열만 둘 것인가? | Accepted | 실제 삭제 method는 제외하고 `disabledAt` 기반 `Disable...` 계열 method만 후보로 둔다. |
| Q8 | raw `originalFileName`을 service input으로 받지 않을 것인가? | Accepted | raw `originalFileName`은 service input으로 받지 않는다. `displayTitle`과 `physicalFileName` 분리 기준을 유지한다. |
| Q9 | `Document.documentType`은 service contract에서 제외할 것인가? | Accepted | `Document.documentType`은 MVP 1차 persisted field가 아니며 service contract에서 제외한다. |
| Q10 | `PolicyDocument.documentType` / `ClaimDocument.documentType`을 source of truth로 service contract에 반영할 것인가? | Accepted | 보험 문서는 `PolicyDocument.documentType`, 청구 문서는 `ClaimDocument.documentType`을 기준으로 한다. |
| Q11 | JSON schema version과 reference validation은 JSON implementation 설계 문서에서 별도 결정할 것인가? | Accepted - Deferred | JSON schema version과 reference validation 세부 정책은 JSON implementation 설계 문서에서 별도 결정한다. |
| Q12 | 이번 단계에서는 구현 없이 interface 설계 문서까지만 진행할 것인가? | Accepted | 이번 단계는 구현 없이 interface 설계 방향과 사용자 결정 기록까지만 진행한다. |

## D. Accepted Interface Direction

- MVP 1차 storage interface는 facade + 내부 도메인 책임 분리 방향으로 간다.
- ViewModel은 JSON 파일 경로, JSON 파일명, JSON DTO를 알지 않는다.
- ViewModel은 service contract와 화면 상태 model에만 의존한다.
- `IDocumentStorageService`를 먼저 설계 대상으로 둔다.
- actual file copy/open은 `IFileAttachmentService` 후보로 분리하고 구현은 보류한다.
- CategoryItem seed 조회/검증은 `ICategorySeedService` 후보로 분리하고 구현은 보류한다.
- interface는 async method 후보로 둔다.
- 실제 삭제 method는 MVP 1차에서 제외한다.
- `Disable...` 계열 method만 후보로 둔다.
- raw `originalFileName`은 service input으로 받지 않는다.
- `Document.documentType`은 service contract에서 제외한다.
- `PolicyDocument.documentType` / `ClaimDocument.documentType`은 source of truth로 service contract에 반영한다.
- JSON schema version과 reference validation은 JSON implementation 설계 문서에서 별도 결정한다.
- 이번 단계에서는 구현 없이 interface 설계 방향까지만 진행한다.

## E. Candidate Services

### `IDocumentStorageService`

후보 역할:

- Document metadata record 관리
- PolicyDocument 연결 record 관리
- ClaimDocument 연결 record 관리
- `disabledAt` 기반 사용 중지 처리
- documentId reference 검증 후보

주의:

- 실제 파일 copy/open은 하지 않는다.
- raw `originalFileName`은 받지 않는다.
- `Document.documentType`은 persisted/service contract에서 제외한다.
- documentType은 `PolicyDocument` / `ClaimDocument`에서 받는다.

### `IFileAttachmentService`

후보 역할:

- actual file boundary
- 파일 copy/open/path prepare 후보

상태:

- 구현 보류
- `attachments/` 내부 파일 생성 보류

### `ICategorySeedService`

후보 역할:

- fixed seed document type 조회
- scope/code 검증
- label/sortOrder 제공

상태:

- 구현 보류
- CategoryItem JSON 저장 구현 보류

## F. Still Not Implemented

아래 항목은 아직 구현하지 않았다.

- C# interface 구현 없음
- C# model 구현 없음
- JSON 저장 구현 없음
- SQLite DB 생성 없음
- SQLite package 추가 없음
- repository/data access 구현 없음
- migration 구현 없음
- CategoryItem 구현 없음
- CategoryItem JSON 저장 구현 없음
- actual file copy/storage 구현 없음
- `attachments/` 내부 파일 생성 없음
- `data/local/` 내부 파일 생성 없음
- DB 구현 없음
- OCR 구현 없음
- metadata 구현 없음
- file storage 구현 없음
- WPF UI/XAML 구현 없음
- navigation 구현 없음
- test code 수정 없음
- NuGet package 추가 없음

## G. Next Decision Needed

다음 항목은 이후 구현 착수 전 별도 결정이 필요하다.

1. JSON schema 초안 결정
2. C# model/interface 구현 범위 결정
3. `DocumentRecord`, `PolicyDocumentRecord`, `ClaimDocumentRecord` contract 확정
4. `DocumentDraft`, `PolicyDocumentDraft`, `ClaimDocumentDraft` contract 확정
5. document type seed constant 구현 여부 결정
6. allowlist와 seed 기준 일치 테스트 범위 결정
7. JSON file storage implementation 범위 결정
8. actual file attachment service 구현 범위 결정

## H. Recommendation

다음 순서로 진행하는 것이 적절하다.

1. JSON schema 초안 문서를 생성한다.
2. 그 다음 C# model/interface 구현 여부를 별도 승인받는다.
3. 그 다음 document type seed constant 구현 여부를 별도 승인받는다.
4. 그 다음 JSON file storage 구현 여부를 별도 승인받는다.
5. actual file attachment service는 metadata storage 이후 별도 승인받는다.

## I. Result

`STORAGE_SERVICE_INTERFACE_USER_DECISION_RECORDED`
