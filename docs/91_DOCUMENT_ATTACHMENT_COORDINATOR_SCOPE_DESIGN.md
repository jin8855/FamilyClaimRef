# Document Attachment Coordinator Scope Design

## A. Goal

이 문서는 `DocumentAttachmentCoordinator` 또는 유사 application service의 책임 범위를 결정하기 위한 설계 문서다.

목적은 다음과 같다.

- file copy와 metadata 저장을 하나의 사용자 작업으로 묶는 책임을 검토한다.
- file copy 성공 후 metadata 저장 실패 시 rollback/cleanup 정책을 검토한다.
- physical file name 생성 책임을 검토한다.
- duplicateIndex 산정 책임을 검토한다.
- 후속 coordinator/request/result/test 구현 범위를 결정하기 위한 Needs Decision을 정리한다.

이 문서는 실제 구현 문서가 아니다.

- C# 구현을 수행하지 않는다.
- coordinator 구현을 수행하지 않는다.
- request/result model을 생성하지 않는다.
- test code를 구현하지 않는다.
- UI/ViewModel을 구현하지 않는다.

## B. Current State

- JSON metadata storage 구현은 완료되었다.
- `JsonFileStore<T>` 구현은 완료되었다.
- `JsonDocumentStorageService` 구현은 완료되었다.
- file attachment primitive 구현은 완료되었다.
- `IFileAttachmentService` 구현은 완료되었다.
- `FileAttachmentCopyResult` 구현은 완료되었다.
- `LocalFileAttachmentService` 구현은 완료되었다.
- build/test는 PASS 상태로 기록되어 있다.
- 총 테스트 수는 113개다.
- metadata storage와 actual file storage는 분리되어 있다.
- `JsonDocumentStorageService`는 metadata만 담당한다.
- `LocalFileAttachmentService`는 actual file copy/delete/exists primitive만 담당한다.
- `FileNamePolicyService`는 physical file name 생성 정책을 담당한다.
- `IFileAttachmentService`는 physical file name을 직접 생성하지 않는다.
- coordinator/application service는 아직 없다.
- file copy와 metadata save 간 transaction boundary는 아직 없다.
- file copy와 metadata save 간 rollback은 아직 없다.
- duplicateIndex 산정 책임은 아직 caller/coordinator 후보로 남아 있다.
- UI file picker 연동은 아직 없다.
- WPF UI/ViewModel 구현은 아직 없다.
- project root `attachments/`, `data/local` 내부 파일 생성은 없다.

## C. Problem Statement

실제 사용자 작업은 다음 흐름으로 구성된다.

1. source file 선택
2. physical file name 생성
3. actual file copy
4. `DocumentRecord` metadata 저장

현재 file copy와 metadata 저장은 서로 다른 service에 분리되어 있다. 이 분리는 유지해야 하지만, 사용자 작업 단위에서는 두 작업을 하나로 묶는 orchestration이 필요할 수 있다.

핵심 문제:

- file copy 성공 후 metadata 저장이 실패하면 orphan file이 생길 수 있다.
- metadata 저장 성공 후 file copy가 실패하는 흐름을 허용하면 broken metadata가 생길 수 있다.
- physical file name은 `FileNamePolicyService` 기준으로 생성되어야 한다.
- duplicateIndex 산정 책임이 아직 명확하지 않다.
- `DocumentRecord.RelativePath`, `PhysicalFileName`, `Extension`은 copy result와 metadata draft 사이에서 일관되어야 한다.
- UI/ViewModel이 file path rule, JSON file rule, rollback rule을 직접 알면 안 된다.

## D. Existing Service Boundary

### `JsonDocumentStorageService`

담당:

- `DocumentRecord` metadata 저장
- `PolicyDocumentRecord` 저장
- `ClaimDocumentRecord` 저장
- documentId reference validation
- documentType validation
- disabled document 연결 거부
- `DisabledAt`, `UpdatedAt` 갱신

담당하지 않음:

- actual file copy
- actual file delete
- physical file name 생성
- source file path 접근
- rollback orchestration
- UI 상태 관리

### `LocalFileAttachmentService`

담당:

- source file copy
- copied file delete-if-exists
- copied file exists check
- attachment root 기준 relative path 생성
- `documents/` 하위 document file 저장
- path traversal 방어
- target exists failure

담당하지 않음:

- JSON metadata 저장
- physical file name 생성
- duplicateIndex 산정
- documentType business validation
- metadata rollback
- UI 상태 관리

### `FileNamePolicyService`

담당:

- physical file name 생성
- documentScope validation
- documentType validation
- extension allowlist validation
- duplicateIndex를 입력으로 받아 file name 생성

담당하지 않음:

- actual file copy
- target file existence scan
- duplicateIndex 자동 산정
- metadata 저장
- UI 상태 관리

## E. Coordinator Responsibility Candidate

후보 이름:

- `DocumentAttachmentCoordinator`
- `DocumentRegistrationService`
- `DocumentImportService`

담당 후보:

- source file path 입력 수신
- documentScope 입력 수신
- documentType 입력 수신
- displayTitle 입력 수신
- target date 또는 reference date 입력 수신
- source extension 추출
- duplicateIndex 산정
- `FileNamePolicyService.CreatePhysicalFileName(...)` 호출
- `IFileAttachmentService.CopyDocumentFileAsync(...)` 호출
- `IDocumentStorageService.AddDocumentAsync(...)` 호출
- copy result 기준 `DocumentDraft` 생성
- metadata 저장 실패 시 copied file cleanup
- cleanup 실패 시 예외 또는 failure result로 보고
- 성공 시 `DocumentRecord` 또는 application result 반환

담당하지 않아야 할 후보:

- WPF file picker 직접 실행
- UI state 직접 변경
- XAML navigation
- OCR parsing
- Policy/Claim case 생성
- SQLite/repository 구현

## F. Candidate Options

### Candidate 1. UI/ViewModel이 service들을 직접 조합

흐름:

1. ViewModel이 `FileNamePolicyService`를 호출한다.
2. ViewModel이 `IFileAttachmentService`를 호출한다.
3. ViewModel이 `IDocumentStorageService`를 호출한다.
4. 실패 시 ViewModel이 cleanup을 수행한다.

장점:

- 새 service가 필요 없다.
- 구현량이 작다.

단점:

- ViewModel이 file path rule, metadata rule, rollback rule을 너무 많이 알게 된다.
- 테스트가 UI/ViewModel에 묶인다.
- 실패 보상 처리가 분산된다.
- 장기 유지보수에 불리하다.

### Candidate 2. Document attachment coordinator 도입

흐름:

1. coordinator가 입력을 받는다.
2. coordinator가 physical file name을 생성한다.
3. coordinator가 file copy를 수행한다.
4. coordinator가 metadata 저장을 수행한다.
5. metadata 저장 실패 시 copied file cleanup을 시도한다.
6. 성공 결과를 반환한다.

장점:

- 사용자 작업 단위가 한 곳에 모인다.
- ViewModel이 단순해진다.
- rollback/cleanup 테스트가 가능하다.
- metadata storage와 file storage 책임 분리를 유지한다.

단점:

- 새 abstraction이 추가된다.
- test 범위가 늘어난다.
- duplicateIndex 산정 정책을 정해야 한다.

### Candidate 3. `JsonDocumentStorageService`에 file attachment orchestration 추가

흐름:

1. metadata service가 file copy까지 호출한다.
2. metadata service가 metadata 저장과 file copy를 내부에서 처리한다.

장점:

- 호출부가 단순하다.

단점:

- 기존 책임 분리와 충돌한다.
- metadata storage가 actual file storage를 알게 된다.
- SQLite 전환 시 변경 범위가 커진다.
- 이미 구현된 service boundary를 흐린다.

### Candidate 4. `LocalFileAttachmentService`가 metadata 저장까지 담당

흐름:

1. file attachment service가 file copy 후 metadata 저장을 호출한다.

장점:

- file 중심 흐름으로 보면 단순하다.

단점:

- actual file service가 JSON metadata를 알게 된다.
- 기존 결정과 충돌한다.
- 테스트와 유지보수가 어려워진다.

## G. Recommended Direction

Candidate Recommendation:

- Candidate 2, 즉 document attachment coordinator 도입을 추천한다.
- 이름은 `DocumentAttachmentCoordinator`를 우선 후보로 둔다.
- coordinator는 file copy 먼저, metadata 저장 나중 흐름을 담당한다.
- metadata 저장 실패 시 copied file cleanup을 시도한다.
- cleanup 실패는 삼키지 말고 결과 또는 exception으로 노출한다.
- `JsonDocumentStorageService`와 `LocalFileAttachmentService`는 기존 책임을 유지한다.
- physical file name 생성은 coordinator가 `FileNamePolicyService`를 호출해 수행한다.
- UI/ViewModel은 coordinator만 호출하는 방향을 후속 후보로 둔다.
- 이번 문서에서는 구현하지 않는다.

이 추천은 확정이 아니라 `Candidate Recommendation`이다.

## H. Input Model Candidate

후보 파일:

- `DocumentAttachmentRequest.cs`
- `DocumentAttachmentResult.cs`

입력 후보:

```csharp
public sealed record DocumentAttachmentRequest(
    string SourceFilePath,
    string DocumentScope,
    string DocumentType,
    string DisplayTitle,
    DateOnly ReferenceDate,
    int DuplicateIndex);
```

검토 필요:

- `DocumentScope` 값: `claim`, `policy`
- `DocumentType` 값: allowlist 기준
- `DisplayTitle` null/empty 처리
- `ReferenceDate` 기준
- `DuplicateIndex`를 caller가 줄지 coordinator가 산정할지
- source extension은 coordinator가 source file에서 추출할지 caller가 줄지

결과 후보:

```csharp
public sealed record DocumentAttachmentResult(
    DocumentRecord Document,
    FileAttachmentCopyResult File);
```

주의:

- 이번 문서에서는 위 파일을 생성하지 않는다.
- 위 코드는 후속 구현 범위 결정을 위한 후보 예시다.

## I. DuplicateIndex Candidate

### Candidate 1. caller가 duplicateIndex를 전달

장점:

- coordinator가 단순하다.
- 기존 `FileNamePolicyService` 입력 구조와 맞다.

단점:

- UI/ViewModel이 target file existence나 existing metadata를 알아야 할 수 있다.
- 잘못 전달하면 target exists failure가 발생한다.

### Candidate 2. coordinator가 duplicateIndex를 자동 산정

방법 후보:

- `IFileAttachmentService.DocumentFileExistsAsync(...)`로 target 후보를 검사한다.
- `IDocumentStorageService.GetDocumentsAsync(...)`의 기존 physical file names를 확인한다.
- 1부터 증가시키며 사용 가능한 file name을 탐색한다.

장점:

- 호출부가 단순하다.
- duplicate collision 처리를 한 곳에 모을 수 있다.

단점:

- exists 후 copy 사이 race condition 가능성은 남는다.
- coordinator 구현량이 늘어난다.
- metadata와 file system 양쪽을 모두 봐야 한다.

Candidate Recommendation:

- MVP 1차에서는 coordinator가 duplicateIndex를 자동 산정하는 방향을 추천한다.
- target exists 시 다음 duplicateIndex를 시도하는 방식이 사용자 경험에 유리하다.
- 단, 최종 copy 시 target exists failure는 여전히 처리해야 한다.
- 동시 import race condition은 MVP 1차 범위 밖으로 둔다.

## J. Transaction / Rollback Candidate

추천 후보 흐름:

1. coordinator validates request.
2. coordinator extracts source extension.
3. coordinator calculates duplicateIndex.
4. coordinator creates physical file name using `FileNamePolicyService`.
5. coordinator copies file using `IFileAttachmentService`.
6. coordinator creates `DocumentDraft` from copy result.
7. coordinator saves metadata using `IDocumentStorageService`.
8. metadata save succeeds -> return result.
9. metadata save fails -> call `DeleteDocumentFileIfExistsAsync(copyResult.RelativePath)`.
10. cleanup succeeds -> rethrow or return failure.
11. cleanup fails -> throw aggregate-style failure or return failure containing both errors.

결정이 필요한 질문 후보:

- metadata 저장 실패 시 cleanup을 시도할 것인가?
- cleanup 실패를 별도 failure로 노출할 것인가?
- custom exception을 만들 것인가?
- MVP에서는 custom exception 없이 기존 exception으로 처리할 것인가?
- coordinator result model을 둘 것인가?

## K. Policy / Claim Link Boundary Candidate

### Candidate 1. Document metadata만 저장

내용:

- file copy와 `DocumentRecord` 저장까지만 처리한다.
- `PolicyDocumentRecord` / `ClaimDocumentRecord` 연결은 별도 단계에서 처리한다.

장점:

- 범위가 작다.
- attachment import와 domain link를 분리한다.

단점:

- 실제 사용자 흐름에서는 파일만 저장되고 policy/claim에 연결되지 않을 수 있다.

### Candidate 2. Document metadata 저장 + Policy/Claim link까지 처리

내용:

- request에 policyId 또는 claimId를 포함한다.
- coordinator가 `AddPolicyDocumentAsync(...)` 또는 `AddClaimDocumentAsync(...)`까지 호출한다.

장점:

- 사용자 작업 단위에 가깝다.
- 한 번에 연결까지 완료된다.

단점:

- Policy/Claim storage가 아직 없다.
- 실패/rollback이 더 복잡하다.
- document 저장 성공 후 link 저장 실패 시 rollback/disable 정책이 필요하다.

Candidate Recommendation:

- MVP 1차 coordinator는 Document metadata 저장까지만 처리한다.
- Policy/Claim link까지 묶는 workflow는 후속 단계로 둔다.
- 이유는 아직 Policy/Claim storage가 없고 rollback 경계가 커지기 때문이다.

## L. Test Scope Candidate

후속 구현 시 포함 후보:

- successful import copies file and saves document metadata
- result contains `DocumentRecord` and `FileAttachmentCopyResult`
- physical file name generated by `FileNamePolicyService`
- duplicateIndex auto increments when target exists
- metadata save failure cleans up copied file
- cleanup failure is reported
- source missing fails before metadata save
- invalid documentType fails before file copy
- invalid extension fails before file copy
- displayTitle required
- request source path required
- temp directory only
- project root `attachments/`, `data/local` not created

후속 구현 시 제외 후보:

- WPF file picker test
- UI/ViewModel test
- OCR test
- SQLite test
- PolicyDocument/ClaimDocument link test
- concurrent duplicate import test

## M. Needs Decision

사용자 결정 질문 후보:

1. coordinator/application service를 도입할 것인가?
2. 이름은 `DocumentAttachmentCoordinator`를 우선 후보로 둘 것인가?
3. UI/ViewModel이 `JsonDocumentStorageService`와 `IFileAttachmentService`를 직접 조합하지 않게 할 것인가?
4. coordinator는 file copy 먼저, metadata 저장 나중 흐름을 담당할 것인가?
5. coordinator가 physical file name 생성을 `FileNamePolicyService`로 수행할 것인가?
6. duplicateIndex는 coordinator가 자동 산정할 것인가?
7. target exists 시 다음 duplicateIndex를 시도할 것인가?
8. metadata 저장 실패 시 copied file cleanup을 시도할 것인가?
9. cleanup 실패를 별도 failure로 노출할 것인가?
10. MVP에서는 custom exception 없이 기존 exception으로 처리할 것인가?
11. coordinator input model `DocumentAttachmentRequest`를 둘 것인가?
12. coordinator result model `DocumentAttachmentResult`를 둘 것인가?
13. source extension은 coordinator가 source file path에서 추출할 것인가?
14. MVP 1차 coordinator는 Document metadata 저장까지만 처리할 것인가?
15. Policy/Claim link까지 처리하는 workflow는 후속으로 보류할 것인가?
16. 후속 구현 시 `DocumentAttachmentCoordinatorTests.cs`를 추가할 것인가?
17. test는 temp directory만 사용할 것인가?
18. actual project `attachments/`, `data/local` 파일 생성은 금지할 것인가?
19. WPF UI/ViewModel 연동은 coordinator 구현 후로 보류할 것인가?

## N. Out of Scope

이번 문서에서 제외하는 범위:

- C# 구현 없음
- coordinator 구현 없음
- request/result model 생성 없음
- test code 구현 없음
- test file 생성 없음
- JSON metadata storage 수정 없음
- file attachment service 수정 없음
- `FileNamePolicyService` 수정 없음
- WPF UI/XAML/navigation/ViewModel 구현 없음
- file picker 구현 없음
- PolicyDocument/ClaimDocument link workflow 구현 없음
- OCR 구현 없음
- SQLite DB/package 추가 없음
- repository/data access/migration 구현 없음
- project root `attachments/` 내부 파일 생성 없음
- project root `data/local` 내부 파일 생성 없음
- 실제 개인정보 샘플 없음

## O. Risks

- duplicateIndex 자동 산정은 exists 후 copy 사이 race condition을 완전히 막지 못한다.
- metadata save failure cleanup 실패 시 orphan file이 남을 수 있다.
- custom exception 없이 처리하면 UI error state 분류가 거칠 수 있다.
- Policy/Claim link를 후속으로 미루면 파일 import 후 미연결 상태가 생길 수 있다.
- coordinator가 과도하게 커지면 application service 책임이 비대해질 수 있다.
- actual production `attachments/`/`data/local` 권한 문제는 temp test로 완전히 검증되지 않는다.
- duplicateIndex가 999를 초과하는 대량 충돌 케이스는 MVP 1차 정책 밖일 수 있다.

## P. Recommendation

1. 이 문서를 기준으로 coordinator/application service 범위 결정을 받는다.
2. 사용자 결정 후 `docs/92_DOCUMENT_ATTACHMENT_COORDINATOR_USER_DECISION_RECORD.md`를 생성한다.
3. 그 다음 별도 승인으로 coordinator/request/result/test를 구현한다.
4. 구현 후 `dotnet build FamilyClaimRef.sln`, `dotnet test FamilyClaimRef.sln`을 실행한다.
5. 구현 후 `docs/93_DOCUMENT_ATTACHMENT_COORDINATOR_IMPLEMENTATION_REVIEW.md`를 생성한다.
6. UI/ViewModel 연동은 coordinator 이후로 보류한다.

## Q. Result

`DOCUMENT_ATTACHMENT_COORDINATOR_SCOPE_DESIGN_DRAFTED`
