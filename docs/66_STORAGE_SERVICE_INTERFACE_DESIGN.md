# Storage Service Interface Design

## A. Goal

이 문서는 FamilyClaimRef MVP의 JSON file storage 구현 전에 storage service interface 경계를 정리하는 설계 문서다.

목적은 UI/ViewModel이 JSON 파일 구조, JSON 파일명, 저장 경로를 직접 알지 않도록 하는 것이다. ViewModel은 service method 후보만 호출하고, JSON 파일 단위와 schema는 implementation detail로 숨기는 방향을 정리한다.

이 문서는 구현 문서가 아니다. C# interface/model/repository 구현, JSON storage 구현, DB/OCR/metadata/file storage 구현, WPF UI/XAML 구현은 수행하지 않는다.

## B. Current State

| 항목 | 현재 상태 |
|---|---|
| WPF scaffold | 생성 완료 |
| Target Framework | `net10.0-windows` |
| `FileNamePolicyService` | 구현 완료 |
| `FileNamePolicyService` 테스트 | PASS 기록 존재 |
| test project | 존재 |
| JSON file storage 방향 | 결정 완료 |
| Document storage structure | 결정 완료 |
| CategoryItem/document type 정책 | 결정 완료 |
| storage service interface 구현 | 없음 |
| C# model 구현 | 없음 |
| JSON 저장 구현 | 없음 |
| `data/local/`, `attachments/` 내부 파일 생성 | 없음 |
| `app/FamilyClaimRef.App/Models/` | 현재 비어 있음 |
| `app/FamilyClaimRef.App/ViewModels/` | 현재 비어 있음 |
| `app/FamilyClaimRef.App/Services/` | `FileNamePolicyService.cs`만 존재 |

현재 확정된 선행 기준:

- MVP 1차 저장 방식은 JSON file storage다.
- SQLite는 MVP 이후 확장 후보로 보류한다.
- metadata root는 `data/local/`이다.
- actual file root는 `attachments/`다.
- storage service interface를 먼저 설계한다.
- JSON implementation은 interface 뒤에 붙이는 MVP 1차 구현체 후보다.
- `Document`는 실제 파일 metadata 공통 record다.
- `PolicyDocument`는 `policyId + documentId` 연결 record다.
- `ClaimDocument`는 `claimId + documentId` 연결 record다.
- 파일 metadata는 `Document`에만 저장한다.
- JSON metadata는 `documents.json`, `policy-documents.json`, `claim-documents.json` 분리 파일 구조다.
- 사용 중지 source of truth는 `disabledAt`이다.
- `isDisabled`는 저장하지 않고 파생 상태로 둔다.
- `displayTitle`, `relativePath`는 `Document`에 저장한다.
- raw `originalFileName`은 MVP에서 저장하지 않는다.
- OCR 임시 결과는 MVP에서 저장하지 않는다.
- OCR 확정값 snapshot은 별도 결정으로 보류한다.
- `memo`는 MVP 1차에서 보류한다.
- document type 관리는 Hybrid fixed seed CategoryItem 방향이다.
- 사용자 정의 document type 추가/수정/삭제는 MVP 1차에서 제외한다.
- 저장 document type 값은 label이 아니라 code다.
- documentType source of truth는 도메인 연결 record다.
  - `PolicyDocument.documentType`
  - `ClaimDocument.documentType`
- `Document.documentType`은 MVP 1차 persisted field에서 제외한다.
- documentType 중복 저장은 금지한다.

## C. Design Problem

JSON file storage는 MVP에 적합하지만, JSON 파일 구조가 UI/ViewModel에 직접 노출되면 이후 SQLite 전환이 어려워진다.

핵심 문제:

- JSON 파일 구조가 UI/ViewModel에 직접 노출되면 이후 SQLite 전환 비용이 커진다.
- `Document`, `PolicyDocument`, `ClaimDocument`가 분리됐으므로 저장 계층에서 reference 정합성을 관리해야 한다.
- actual file storage와 metadata storage의 책임을 분리해야 한다.
- raw `originalFileName`과 OCR 임시 결과 저장 보류 기준이 service boundary에 반영되어야 한다.
- 사용 중지 정책은 실제 삭제가 아니라 `disabledAt` 기준이어야 한다.
- documentType은 도메인 연결 record의 source of truth로 다뤄야 한다.
- `FileNamePolicyService`는 physical file name 생성 책임만 유지해야 한다.

## D. Service Boundary Candidate

### Candidate 1: 단일 `ILocalStorageService`

역할:

- FamilyMember, Policy, Claim, Document 등 모든 metadata를 한 interface에서 처리한다.

장점:

- MVP 구현이 단순하다.
- 파일 수가 적을 때 빠르게 시작할 수 있다.
- ViewModel에서 접근 지점이 하나라 사용법이 단순하다.

단점:

- interface가 비대해진다.
- 도메인별 책임이 흐려진다.
- 테스트 범위가 커진다.
- 일부 기능 변경이 전체 storage service에 영향을 줄 수 있다.

### Candidate 2: 도메인별 storage service

후보:

- `IFamilyMemberStorageService`
- `IPolicyStorageService`
- `IClaimStorageService`
- `IDocumentStorageService`

장점:

- 책임 분리가 명확하다.
- 테스트 작성이 쉽다.
- 도메인별 확장이 쉽다.
- JSON에서 SQLite로 전환할 때 도메인별 migration 계획을 세우기 좋다.

단점:

- 파일과 interface 수가 증가한다.
- MVP 초기에 설계 부담이 증가한다.
- ViewModel이 여러 service를 직접 알게 될 수 있다.

### Candidate 3: MVP용 aggregate storage facade + 내부 도메인 분리

후보:

- ViewModel은 `IAppStorageService` 또는 `IStorageFacade`에 접근한다.
- 내부 구현은 Document, Policy, Claim 저장 책임을 분리한다.
- JSON 파일 단위는 implementation 내부에 숨긴다.

장점:

- ViewModel이 단순해진다.
- JSON/SQLite 전환 가능성을 유지한다.
- 도메인 분리와 MVP 단순성의 균형을 잡을 수 있다.
- 저장 구현 변경이 ViewModel에 직접 전파되는 위험을 줄인다.

단점:

- facade와 내부 service 경계 설계가 필요하다.
- 과도하게 추상화하면 MVP 속도가 느려질 수 있다.
- facade가 커지면 Candidate 1과 같은 비대화 위험이 생긴다.

## E. Recommended Direction

### Candidate Recommendation

MVP 1차는 storage facade + 내부 도메인별 책임 분리 방향이 적절하다.

추천 기준:

- ViewModel은 JSON 파일 경로나 파일명을 직접 알면 안 된다.
- UI/ViewModel은 service method만 호출한다.
- JSON 파일 단위는 implementation detail로 둔다.
- interface는 SQLite 전환을 고려해 async 기반 후보로 둔다.
- actual file copy/storage는 metadata storage interface와 분리한다.
- `FileNamePolicyService`는 physical file name 생성 책임만 유지한다.
- metadata 저장 service가 `FileNamePolicyService`의 책임을 가져오면 안 된다.
- documentType source of truth는 `PolicyDocument.documentType`과 `ClaimDocument.documentType`으로 유지한다.
- 실제 삭제 method는 MVP 1차에서 제외하고 `disabledAt` 기반 disable method만 후보로 둔다.

이 추천은 구현 확정이 아니라 `Candidate Recommendation`이다. 사용자 결정 기록 전까지는 구현하지 않는다.

## F. Interface Candidate

이 절은 구현이 아니라 메서드 후보 기록이다.

### `IDocumentStorageService` 후보

메서드 후보:

- `GetDocumentsAsync()`
- `GetDocumentByIdAsync(documentId)`
- `AddDocumentAsync(documentDraft)`
- `DisableDocumentAsync(documentId, disabledAt)`
- `GetPolicyDocumentsAsync(policyId)`
- `AddPolicyDocumentAsync(policyDocumentDraft)`
- `DisablePolicyDocumentAsync(policyDocumentId, disabledAt)`
- `GetClaimDocumentsAsync(claimId)`
- `AddClaimDocumentAsync(claimDocumentDraft)`
- `DisableClaimDocumentAsync(claimDocumentId, disabledAt)`

주의:

- 실제 파일 복사는 하지 않는다.
- raw `originalFileName`은 받지 않는다.
- `Document.documentType`은 persisted field로 두지 않는다.
- `PolicyDocument.documentType` / `ClaimDocument.documentType`이 source of truth다.
- 실제 삭제 method는 MVP 1차에서 두지 않는다.
- `disabledAt`은 저장되는 사용 중지 기준이다.

### `IFileAttachmentService` 후보

역할 후보:

- 실제 파일 copy/move/read/open 등 actual file boundary를 담당한다.
- MVP 1차에서는 interface 설계만 후보로 둔다.
- 실제 구현은 별도 승인 전까지 보류한다.

메서드 후보:

- `PrepareAttachmentPathAsync(...)`
- `CopyAttachmentAsync(...)`
- `OpenAttachmentAsync(...)`

주의:

- 이번 문서에서는 actual file service 구현은 범위 밖이다.
- `attachments/` 내부 파일 생성도 범위 밖이다.
- `FileNamePolicyService`가 생성한 `physicalFileName`을 사용할 수 있지만, 파일 복사 자체는 별도 승인 전까지 하지 않는다.

### `ICategorySeedService` 후보

역할 후보:

- fixed seed document type category를 조회한다.
- code, label, scope, sortOrder, disabledAt 기준을 제공한다.
- `FileNamePolicyService` allowlist와 seed 기준의 일치 여부를 검증하는 후속 테스트 후보와 연결된다.

메서드 후보:

- `GetDocumentTypeSeeds()`
- `GetDocumentTypesByScope(scope)`
- `ValidateDocumentType(scope, code)`

주의:

- CategoryItem JSON 저장 구현은 MVP 1차 storage 구현 이후로 보류한다.
- 현재는 seed/constant 후보만 다룬다.
- 사용자 정의 document type 추가/수정/삭제는 MVP 1차에서 제외한다.

## G. Data Contract Candidate

아래는 draft/input/output 후보이며 C# record 또는 class 구현 지시가 아니다.

| 후보 | 용도 | 비고 |
|---|---|---|
| `DocumentDraft` | 새 파일 metadata 생성 요청 | raw `originalFileName` 제외 |
| `PolicyDocumentDraft` | 보험 문서 연결 생성 요청 | `policyId`, `documentId`, `documentType` 후보 |
| `ClaimDocumentDraft` | 청구 문서 연결 생성 요청 | `claimId`, `documentId`, `documentType` 후보 |
| `DocumentRecord` | 저장된 파일 metadata 상태 | `physicalFileName`, `displayTitle`, `relativePath`, `disabledAt` 후보 |
| `PolicyDocumentRecord` | 저장된 보험 문서 연결 상태 | `PolicyDocument.documentType` source of truth |
| `ClaimDocumentRecord` | 저장된 청구 문서 연결 상태 | `ClaimDocument.documentType` source of truth |
| `CategoryItemSeed` | fixed seed document type 기준 | `code`, `label`, `scope`, `sortOrder`, `disabledAt` 후보 |

정리 기준:

- Draft는 사용자 입력/생성 요청용이다.
- Record는 저장된 상태다.
- ViewModel은 JSON DTO에 직접 의존하지 않는다.
- 저장 record와 화면 ViewModel은 분리한다.
- JSON DTO가 필요해지더라도 service implementation 내부 detail로 둔다.

## H. Validation Responsibility

### `FileNamePolicyService`

책임 후보:

- scope 검증
- id 안전 문자 검증
- documentType allowlist 검증
- extension allowlist 검증
- duplicateIndex 검증
- physical file name 생성

주의:

- `FileNamePolicyService`는 metadata 저장, JSON 저장, 파일 복사 책임을 갖지 않는다.
- `FileNamePolicyService`는 `data/local/` 또는 `attachments/`에 접근하지 않는다.

### Storage service

책임 후보:

- id 중복 검증
- documentId reference 검증
- policyId/claimId reference 검증 후보
- disabledAt 기반 사용 중지 처리
- JSON schema version 후보 처리
- 저장 전 필수값 검증 후보
- JSON 파일 단위 숨김

주의:

- raw `originalFileName` 저장을 허용하지 않는다.
- OCR 임시 결과 저장을 허용하지 않는다.
- 실제 파일 copy/open을 직접 담당하지 않는다.

### Category seed service

책임 후보:

- scope/code 기준 검증 후보
- label/sortOrder 제공 후보
- disabledAt 기준 비활성 seed 처리 후보
- `FileNamePolicyService` allowlist와 seed 기준 일치 검증 후보

### File attachment service

책임 후보:

- 실제 파일 존재 여부 확인 후보
- 확장자와 파일 접근 검증 후보
- copy/open 경계 후보
- `attachments/` root 기준 경로 처리 후보

주의:

- 이번 문서에서는 실제 파일 복사, 열기, 저장 구현을 하지 않는다.

## I. Out of Scope

이번 문서에서 제외한다.

- C# interface 구현 없음
- C# model 구현 없음
- JSON 저장 구현 없음
- SQLite DB 생성 없음
- SQLite package 추가 없음
- repository 구현 없음
- data access 구현 없음
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
- 실제 개인정보 샘플 없음

## J. Risks

| 위험 | 설명 | 완화 후보 |
|---|---|---|
| interface 비대화 | interface를 너무 크게 만들면 구현과 테스트가 무거워진다. | facade는 얇게 두고 내부 도메인 책임을 분리한다. |
| interface 과분리 | interface를 너무 잘게 나누면 MVP 개발 속도가 느려진다. | MVP에서는 facade + 핵심 domain service 후보까지만 둔다. |
| ViewModel 결합 | ViewModel이 JSON DTO에 직접 의존하면 SQLite 전환 비용이 커진다. | ViewModel은 service contract와 화면 model에만 의존한다. |
| 책임 혼합 | actual file storage와 metadata storage를 섞으면 책임이 흐려진다. | `IDocumentStorageService`와 `IFileAttachmentService`를 분리한다. |
| documentType drift | documentType source of truth가 service method에 반영되지 않으면 결정 문서와 구현이 어긋난다. | `PolicyDocument` / `ClaimDocument` draft에 documentType을 둔다. |
| 삭제 정책 충돌 | `disabledAt` 대신 실제 삭제 method를 먼저 만들면 기존 삭제 정책과 충돌한다. | MVP 1차에는 `Disable...` method 후보만 둔다. |

## K. Needs Decision

| ID | Question | Status |
|---|---|---|
| Q1 | MVP 1차 storage interface는 facade + 내부 도메인 책임 분리 방향으로 갈 것인가? | Needs Decision |
| Q2 | ViewModel은 JSON 파일 경로나 JSON DTO를 직접 알지 않게 할 것인가? | Needs Decision |
| Q3 | `IDocumentStorageService`를 먼저 설계 대상으로 둘 것인가? | Needs Decision |
| Q4 | actual file copy/open은 `IFileAttachmentService` 후보로 분리하고 구현은 보류할 것인가? | Needs Decision |
| Q5 | CategoryItem seed 조회/검증은 `ICategorySeedService` 후보로 분리하고 구현은 보류할 것인가? | Needs Decision |
| Q6 | interface는 async method 후보로 둘 것인가? | Needs Decision |
| Q7 | 실제 삭제 method는 MVP 1차에서 제외하고 `Disable...` 계열만 둘 것인가? | Needs Decision |
| Q8 | raw `originalFileName`을 service input으로 받지 않을 것인가? | Needs Decision |
| Q9 | `Document.documentType`은 service contract에서 제외할 것인가? | Needs Decision |
| Q10 | `PolicyDocument.documentType` / `ClaimDocument.documentType`을 source of truth로 service contract에 반영할 것인가? | Needs Decision |
| Q11 | JSON schema version과 reference validation은 JSON implementation 설계 문서에서 별도 결정할 것인가? | Needs Decision |
| Q12 | 이번 단계에서는 구현 없이 interface 설계 문서까지만 진행할 것인가? | Needs Decision |

## L. Recommendation

다음 순서로 진행하는 것이 적절하다.

1. 이 문서를 기준으로 storage service interface 방향 결정을 받는다.
2. 사용자 결정 후 `docs/67_STORAGE_SERVICE_INTERFACE_USER_DECISION_RECORD.md`를 생성한다.
3. 그 다음 JSON schema 초안 문서를 생성한다.
4. 그 다음 C# model/interface 구현 여부를 별도 승인받는다.
5. 그 다음 JSON file storage 구현 여부를 별도 승인받는다.

## M. Result

`STORAGE_SERVICE_INTERFACE_DESIGN_DRAFTED`
