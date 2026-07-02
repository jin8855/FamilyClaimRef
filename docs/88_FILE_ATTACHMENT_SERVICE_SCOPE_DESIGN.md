# File Attachment Service Scope Design

## A. Goal

이 문서는 actual file copy/storage 구현 전 `IFileAttachmentService` 범위를 결정하기 위한 설계 문서다.

목적은 다음과 같다.

- JSON metadata storage와 actual file storage의 책임 분리를 정리한다.
- metadata 저장과 file copy 간 transaction boundary를 검토한다.
- physical file name 생성 책임을 검토한다.
- 실패/rollback 정책 후보를 정리한다.
- 후속 interface/test 구현 범위를 결정하기 위한 Needs Decision을 정리한다.

이 문서는 실제 구현 문서가 아니다.

- C# 구현을 수행하지 않는다.
- interface 구현을 수행하지 않는다.
- actual file copy/storage를 구현하지 않는다.
- 실제 파일을 생성하지 않는다.
- test code를 구현하지 않는다.

## B. Current State

- JSON metadata storage 1차 구현은 완료되었다.
- `JsonFileStore<T>` 구현은 완료되었다.
- `JsonDocumentStorageService` 구현은 완료되었다.
- `JsonDocumentStorageServiceTests` 구현은 완료되었다.
- build/test는 PASS 상태로 기록되어 있다.
- 총 테스트 수는 89개다.
- `documents.json`, `policy-documents.json`, `claim-documents.json` metadata 분리 저장 구조가 구현되어 있다.
- documentId reference validation이 구현되어 있다.
- disabled document 연결 거부가 구현되어 있다.
- documentType allowlist 검증이 구현되어 있다.
- `JsonDocumentStorageService`는 metadata만 담당한다.
- actual file copy/storage는 아직 없다.
- `IFileAttachmentService`는 아직 없다.
- metadata 저장과 actual file copy 간 transaction boundary는 아직 없다.
- `attachments/` 내부 실제 파일 생성은 없다.
- `data/local` 운영 저장 파일 생성은 없다.
- WPF UI/ViewModel 연동은 없다.
- SQLite/DB/repository 구현은 없다.

## C. Problem Statement

사용자가 보험 문서나 청구 서류를 첨부하면 실제 파일을 `attachments/` 또는 유사 storage root로 복사해야 한다.

동시에 해당 파일 metadata는 `documents.json`에 저장되어야 한다. 이때 file copy와 metadata save는 서로 다른 책임이므로 실패 순서에 따라 데이터 불일치가 생길 수 있다.

핵심 문제:

- file copy 성공 후 metadata 저장이 실패하면 orphan file이 생길 수 있다.
- metadata 저장 성공 후 file copy가 실패하면 broken metadata가 생길 수 있다.
- actual file path와 metadata `RelativePath`, `PhysicalFileName`, `Extension` 기준을 맞춰야 한다.
- physical file name을 `FileNamePolicyService`로 생성할지, attachment service가 직접 생성할지 결정해야 한다.
- raw `OriginalFileName`은 MVP 1차 metadata 저장 대상에서 제외되어 있으므로 서비스 입력과 저장 기준을 분리해야 한다.
- UI/ViewModel은 실제 file path rule을 직접 알면 안 된다.
- path traversal, duplicate target, file lock, 권한 오류 같은 OS file boundary 위험을 분리해서 다뤄야 한다.

## D. Responsibility Boundary

### `JsonDocumentStorageService`

담당:

- metadata JSON read/write
- `DocumentRecord` 저장
- `PolicyDocumentRecord` 저장
- `ClaimDocumentRecord` 저장
- documentId reference validation
- documentType validation
- disabled document 연결 거부
- `DisabledAt`, `UpdatedAt` 갱신

담당하지 않음:

- 실제 파일 복사
- 실제 파일 삭제
- 실제 파일 열기
- physical file name 생성
- 원본 파일 경로 접근
- `attachments/` 내부 파일 관리
- file picker 또는 UI 상태 관리

### `IFileAttachmentService`

담당 후보:

- source file 존재 확인
- source file extension 확인
- target physical file name 생성 또는 전달받은 physical file name 사용
- target relative path 결정
- file copy
- overwrite 방지
- copied file delete/cleanup
- file open 가능 여부 확인 후보
- temp file copy 후 move 후보
- path traversal 방어 후보

담당하지 않아야 할 후보:

- `documents.json` 직접 수정
- `policy-documents.json` 직접 수정
- `claim-documents.json` 직접 수정
- policyId/claimId reference validation
- documentType business validation
- UI 상태 관리
- file picker 직접 실행

### Orchestration Layer 후보

file copy와 metadata 저장을 하나의 사용자 작업으로 묶으려면 orchestration layer가 필요할 수 있다.

후보 이름:

- `DocumentAttachmentCoordinator`
- `DocumentRegistrationService`
- `DocumentImportService`

담당 후보:

- source file copy
- metadata `DocumentRecord` 저장
- 실패 시 rollback
- physical file name 생성 흐름 조합
- 향후 `PolicyDocument` / `ClaimDocument` 연결까지 조합

주의:

- 이번 문서에서는 orchestration layer를 구현하지 않는다.
- 이름과 책임은 Candidate이며 후속 사용자 결정이 필요하다.

## E. Candidate Options

### Candidate 1. File copy 먼저, metadata 저장 나중

흐름:

1. `IFileAttachmentService`가 파일을 `attachments/`에 copy한다.
2. `JsonDocumentStorageService.AddDocumentAsync(...)`가 metadata를 저장한다.
3. metadata 저장 실패 시 copied file 삭제를 시도한다.

장점:

- metadata가 실제 없는 파일을 가리킬 가능성이 낮다.
- file copy 실패 시 metadata 저장을 시작하지 않는다.
- actual file boundary 오류를 metadata 저장 전에 걸러낼 수 있다.

단점:

- metadata 저장 실패 시 orphan file cleanup이 필요하다.
- cleanup 실패 시 orphan file이 남을 수 있다.
- file copy와 metadata save를 호출자가 순서대로 조합해야 한다.

### Candidate 2. Metadata 저장 먼저, file copy 나중

흐름:

1. `JsonDocumentStorageService.AddDocumentAsync(...)`가 metadata를 저장한다.
2. `IFileAttachmentService`가 파일을 copy한다.
3. file copy 실패 시 metadata disable 또는 rollback이 필요하다.

장점:

- metadata id를 먼저 확보할 수 있다.
- target file name을 metadata id 기반으로 만들기 쉽다.

단점:

- file copy 실패 시 broken metadata가 생길 위험이 크다.
- metadata rollback/disable 정책이 복잡하다.
- metadata에는 있는데 파일은 없는 상태가 사용자 화면에 노출될 수 있다.

### Candidate 3. Coordinator가 file copy와 metadata 저장을 묶어 처리

흐름:

1. coordinator가 `FileNamePolicyService`로 physical file name을 생성한다.
2. `IFileAttachmentService`가 file copy를 수행한다.
3. `JsonDocumentStorageService`가 metadata를 저장한다.
4. 실패 시 coordinator가 cleanup/rollback을 수행한다.

장점:

- 책임 분리가 명확하다.
- `IFileAttachmentService`는 파일만 담당한다.
- `JsonDocumentStorageService`는 metadata만 담당한다.
- 실패 보상 처리를 한 곳에서 관리할 수 있다.
- UI/ViewModel은 file path rule과 JSON file rule을 직접 알 필요가 없다.

단점:

- 새로운 orchestration abstraction이 필요하다.
- MVP 초기 구현량이 늘어난다.
- coordinator test 범위가 별도로 필요하다.

### Candidate 4. `JsonDocumentStorageService`가 file copy까지 담당

흐름:

1. `JsonDocumentStorageService`가 file copy를 수행한다.
2. 같은 service가 metadata 저장까지 직접 수행한다.

장점:

- 호출자는 단순하다.
- orchestration layer가 필요 없다.

단점:

- metadata storage와 actual file storage 책임이 섞인다.
- 기존 책임 분리 결정과 충돌한다.
- 테스트와 유지보수가 어려워진다.
- SQLite 전환 또는 file service 교체 시 변경 범위가 커진다.

## F. Recommended Direction

Candidate Recommendation:

- Candidate 3을 장기 구조로 추천한다.
- 다만 MVP 1차에서는 coordinator 구현 전, 먼저 `IFileAttachmentService` 범위를 확정한다.
- `IFileAttachmentService`는 actual file copy/storage만 담당한다.
- `IFileAttachmentService`는 JSON metadata를 직접 수정하지 않는다.
- `JsonDocumentStorageService`는 actual file copy를 하지 않는다.
- physical file name은 `FileNamePolicyService` 기준을 사용한다.
- transaction boundary는 coordinator 또는 후속 application service에서 처리한다.
- 다음 단계는 `IFileAttachmentService` interface 설계와 user decision record가 적절하다.
- 이번 문서에서는 구현하지 않는다.

이 추천은 확정이 아니라 `Candidate Recommendation`이다.

## G. Attachment Root / Path Candidate

파일 저장 경로 후보:

- attachment root: `attachments/`
- document files root: `attachments/documents/`
- year/month subfolder 후보: `attachments/documents/yyyy/MM/`
- flat folder 후보: `attachments/documents/`
- relative path 저장 기준: attachment root 기준 상대 경로

검토 기준:

- Windows 경로 separator 대응이 필요하다.
- UI/ViewModel에 absolute path를 노출하지 않는다.
- metadata에는 absolute path를 저장하지 않는다.
- `DocumentRecord.RelativePath`는 attachment root 기준 relative path 후보로 둔다.
- test에서는 temp directory만 사용한다.
- actual project `attachments/` 내부 파일은 test에서 생성하지 않는다.

추천 후보:

- attachment root path는 생성자 등으로 주입한다.
- test는 temp directory를 사용한다.
- metadata에는 absolute path를 저장하지 않는다.
- MVP 1차는 flat folder 또는 `documents/` 하위 folder로 시작한다.
- year/month folder는 후속 후보로 둔다.

## H. File Name Policy Candidate

파일명 생성 기준 후보:

- 기존 `FileNamePolicyService.CreatePhysicalFileName(...)`을 사용한다.
- documentScope, id, date, documentType, source extension, duplicateIndex를 기준으로 physical file name을 생성한다.
- raw original file name은 metadata에 저장하지 않는다.
- source file extension은 입력 또는 validation 용도로만 사용한다.
- extension allowlist는 `FileNamePolicyService` 기준과 일치해야 한다.

결정 질문:

- `IFileAttachmentService`가 physical file name을 직접 만들 것인가?
- coordinator가 `FileNamePolicyService`로 physical file name을 만든 뒤 `IFileAttachmentService`에 전달할 것인가?
- duplicateIndex 산정 책임은 caller/coordinator가 가질 것인가, file service가 target 존재 여부를 보고 결정할 것인가?

추천 후보:

- `IFileAttachmentService`는 전달받은 physical file name 또는 target relative path로 copy만 한다.
- physical file name 생성은 coordinator 또는 caller가 `FileNamePolicyService`를 통해 수행한다.
- 이렇게 하면 file service가 documentType policy를 알 필요가 없다.

## I. Failure / Rollback Candidate

실패 정책 후보:

- source file missing -> failure
- source file extension invalid -> failure
- target file already exists -> failure
- path traversal attempt -> failure
- copy failure -> failure
- metadata save failure after copy -> copied file cleanup 시도
- cleanup failure -> warning/risk 기록 후보
- metadata save success 후 file copy failure -> 이 흐름은 가급적 피함
- disabled metadata와 actual file 삭제 정책은 별도 결정

추천 후보:

- MVP에서는 file copy 먼저, metadata 저장 나중 후보를 사용한다.
- metadata 저장 실패 시 copied file cleanup을 시도한다.
- 단, 이 orchestration은 `IFileAttachmentService`가 아니라 coordinator 책임으로 둔다.
- `IFileAttachmentService`는 copy/delete/exists 같은 primitive operation을 제공한다.
- cleanup 실패는 삼키지 말고 호출자가 기록하거나 사용자에게 복구 필요 상태로 노출할 수 있게 한다.

## J. Candidate Interface Shape

### Candidate A. Generic relative path copy service

```csharp
public interface IFileAttachmentService
{
    Task<FileAttachmentCopyResult> CopyAsync(
        string sourceFilePath,
        string targetRelativePath,
        CancellationToken cancellationToken = default);

    Task DeleteIfExistsAsync(
        string relativePath,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(
        string relativePath,
        CancellationToken cancellationToken = default);
}
```

특징:

- document-specific naming을 모른다.
- caller/coordinator가 `targetRelativePath`를 결정한다.
- 범용 attachment storage로 확장하기 쉽다.
- path validation 책임은 service와 caller 사이에서 명확히 정해야 한다.

### Candidate B. Document-specific copy service

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

특징:

- document file 저장 흐름에 맞춰 호출부가 단순해진다.
- service가 document folder 기준 relative path를 만들 수 있다.
- 향후 다른 attachment category가 생기면 method가 늘어날 수 있다.
- physical file name 생성 책임은 여전히 caller/coordinator에 둘 수 있다.

비교 기준:

| 기준 | Candidate A | Candidate B |
|---|---|---|
| service 성격 | generic file service | document-specific service |
| relative path 생성 책임 | caller/coordinator 후보 | service 후보 |
| physical file name 생성 책임 | caller/coordinator 후보 | caller/coordinator 후보 |
| test temp directory 사용 | 가능 | 가능 |
| future attachment category 확장 | 유리 | 별도 method 증가 가능 |
| 호출부 단순성 | 보통 | 높음 |

초기 추천 후보:

- MVP 1차는 Candidate B가 호출부를 단순하게 만들 수 있다.
- 장기적으로는 Candidate A가 범용성이 높다.
- 최종 선택은 coordinator 도입 여부와 attachment category 확장 가능성을 보고 결정한다.

## K. Test Scope Candidate

후속 구현 시 포함 후보:

- missing source file rejected
- copy creates target file under temp attachment root
- copied file content matches source
- target existing rejected
- relative path returned
- absolute path not stored in result
- delete existing copied file
- delete missing file does not fail 또는 명시 정책
- path traversal attempt rejected
- invalid physical file name rejected
- test uses temp directory only
- actual project `attachments/` not created

후속 구현 시 제외 후보:

- JSON metadata save test
- coordinator rollback test
- WPF file picker test
- actual OS open file test
- OCR test
- SQLite test

테스트 주의:

- 테스트 source file도 temp directory 안에서 dummy file로만 만든다.
- 실제 개인정보 샘플 파일명이나 파일 내용은 사용하지 않는다.
- 실제 `attachments/` root에 파일을 만들지 않는다.

## L. Needs Decision

후속 사용자 결정이 필요한 항목:

1. actual file copy/storage 구현 전에 `IFileAttachmentService` interface를 먼저 설계할 것인가?
2. `IFileAttachmentService`는 JSON metadata를 직접 수정하지 않는 것으로 확정할 것인가?
3. `JsonDocumentStorageService`는 actual file copy를 하지 않는 것으로 유지할 것인가?
4. file copy와 metadata 저장을 묶는 coordinator/application service는 별도 후속 후보로 둘 것인가?
5. MVP 1차 흐름은 file copy 먼저, metadata 저장 나중, 실패 시 copied file cleanup 시도 방향으로 둘 것인가?
6. attachment root는 생성자 등으로 주입받게 할 것인가?
7. test에서는 temp directory만 사용할 것인가?
8. production 후보 root는 `attachments/`로 둘 것인가?
9. metadata에는 absolute path를 저장하지 않을 것인가?
10. `DocumentRecord.RelativePath`는 attachment root 기준 relative path로 둘 것인가?
11. physical file name 생성은 `IFileAttachmentService`가 아니라 caller/coordinator가 `FileNamePolicyService`를 통해 수행하게 할 것인가?
12. `IFileAttachmentService`는 전달받은 physical file name 또는 relative path로 copy만 수행하게 할 것인가?
13. source file missing은 failure로 처리할 것인가?
14. target file already exists는 failure로 처리할 것인가?
15. path traversal attempt는 failure로 처리할 것인가?
16. delete-if-exists primitive를 포함할 것인가?
17. exists primitive를 포함할 것인가?
18. 후속 구현 시 `IFileAttachmentServiceTests.cs`를 추가할 것인가?
19. actual project `attachments/` 파일 생성은 production 구현에서만 가능하고 test에서는 temp directory만 사용할 것인가?

## M. Out of Scope

이번 문서에서 제외하는 범위는 다음과 같다.

- C# 구현 없음
- interface 구현 없음
- actual file copy/storage 구현 없음
- 실제 file 생성 없음
- test code 구현 없음
- test file 생성 없음
- JSON metadata storage 수정 없음
- `JsonDocumentStorageService` 수정 없음
- `FileNamePolicyService` 수정 없음
- WPF UI/XAML/navigation/ViewModel 구현 없음
- file picker 구현 없음
- OCR 구현 없음
- SQLite DB/package 추가 없음
- repository/data access/migration 구현 없음
- `attachments/` 내부 파일 생성 없음
- `data/local` 내부 파일 생성 없음
- 실제 개인정보 샘플 없음

## N. Risks

- file copy와 metadata save 간 rollback 실패 가능성이 있다.
- copied file cleanup 실패 시 orphan file이 남을 수 있다.
- metadata save 성공 후 file missing 상태가 발생할 수 있다.
- path traversal 방어가 누락되면 attachment root 밖 파일 접근 위험이 있다.
- absolute path를 저장하면 PC 이전/백업/복원에서 경로가 깨질 수 있다.
- source file extension과 generated physical file name extension mismatch 위험이 있다.
- production attachment root 권한 문제가 발생할 수 있다.
- UI file picker와 service boundary가 섞일 위험이 있다.
- file lock / antivirus / OneDrive 동기화 환경에서 copy가 실패할 수 있다.
- 동시 import 시 duplicate physical file name 충돌 가능성이 있다.
- duplicateIndex 산정 책임이 불명확하면 동일 파일명 충돌 처리 기준이 흔들릴 수 있다.

## O. Recommendation

1. 이 문서를 기준으로 `IFileAttachmentService` 범위 결정을 받는다.
2. 사용자 결정 후 `docs/89_FILE_ATTACHMENT_SERVICE_USER_DECISION_RECORD.md`를 생성한다.
3. 그 다음 별도 승인으로 `IFileAttachmentService` interface/implementation/test를 구현한다.
4. 구현 시 temp directory 기준 테스트만 사용한다.
5. JSON metadata storage와 actual file storage는 계속 분리한다.
6. coordinator/application service는 file service 구현 후 별도 설계로 둔다.
7. WPF UI/ViewModel 연동은 그 이후로 보류한다.

## P. Result

`FILE_ATTACHMENT_SERVICE_SCOPE_DESIGN_DRAFTED`
