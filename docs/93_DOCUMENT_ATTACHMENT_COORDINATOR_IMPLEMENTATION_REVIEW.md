# DocumentAttachmentCoordinator Implementation Review

## A. Goal

이 문서는 `DocumentAttachmentCoordinator` 구현 결과 리뷰 문서다.

기록 대상은 다음과 같다.

- `DocumentAttachmentRequest` 구현 결과
- `DocumentAttachmentResult` 구현 결과
- `DocumentAttachmentCoordinator` 구현 결과
- `DocumentAttachmentCoordinatorTests` 구현 결과
- 구현 범위 준수 여부
- build/test 검증 결과
- 남은 위험과 후속 추천 작업

이 문서는 다음 작업의 리뷰가 아니다.

- JSON metadata storage 구현 리뷰가 아니다.
- file attachment primitive 구현 리뷰가 아니다.
- PolicyDocument/ClaimDocument link workflow 구현 리뷰가 아니다.
- WPF UI/ViewModel 구현 리뷰가 아니다.
- OCR, SQLite, repository/data access 구현 리뷰가 아니다.

## B. Checked Files / Paths

| 대상 | 확인 목적 | 판정 |
|---|---|---|
| `docs/92_DOCUMENT_ATTACHMENT_COORDINATOR_USER_DECISION_RECORD.md` | 사용자 결정 기준 확인 | PASS |
| `docs/91_DOCUMENT_ATTACHMENT_COORDINATOR_SCOPE_DESIGN.md` | coordinator scope 설계 기준 확인 | PASS |
| `docs/90_FILE_ATTACHMENT_SERVICE_IMPLEMENTATION_REVIEW.md` | file attachment service 구현 결과 확인 | PASS |
| `app/FamilyClaimRef.App/Services/Storage/DocumentAttachmentRequest.cs` | request model 구현 확인 | PASS |
| `app/FamilyClaimRef.App/Services/Storage/DocumentAttachmentResult.cs` | result model 구현 확인 | PASS |
| `app/FamilyClaimRef.App/Services/Storage/DocumentAttachmentCoordinator.cs` | coordinator 구현 확인 | PASS |
| `tests/FamilyClaimRef.App.Tests/DocumentAttachmentCoordinatorTests.cs` | coordinator tests 구현 확인 | PASS |
| `app/FamilyClaimRef.App/Services/Storage/IFileAttachmentService.cs` | file attachment interface 경계 확인 | PASS |
| `app/FamilyClaimRef.App/Services/Storage/LocalFileAttachmentService.cs` | local file primitive 경계 확인 | PASS |
| `app/FamilyClaimRef.App/Services/Storage/FileAttachmentCopyResult.cs` | copy result model 확인 | PASS |
| `app/FamilyClaimRef.App/Services/Storage/IDocumentStorageService.cs` | metadata storage interface 확인 | PASS |
| `app/FamilyClaimRef.App/Services/Storage/JsonDocumentStorageService.cs` | metadata storage 구현 경계 확인 | PASS |
| `app/FamilyClaimRef.App/Services/FileNamePolicyService.cs` | physical file name 정책 확인 | PASS |
| `app/FamilyClaimRef.App/Models/Storage/DocumentRecord.cs` | 저장된 document metadata record 확인 | PASS |
| `app/FamilyClaimRef.App/Models/Storage/DocumentDraft.cs` | document metadata 저장 입력 확인 | PASS |
| `FamilyClaimRef.sln` | solution 구성 확인 | PASS |
| `app/FamilyClaimRef.App/FamilyClaimRef.App.csproj` | app project 구성 확인 | PASS |
| `tests/FamilyClaimRef.App.Tests/FamilyClaimRef.App.Tests.csproj` | test project 구성 확인 | PASS |

## C. Implementation Summary

- `DocumentAttachmentRequest.cs`가 생성되었다.
- `DocumentAttachmentResult.cs`가 생성되었다.
- `DocumentAttachmentCoordinator.cs`가 생성되었다.
- `DocumentAttachmentCoordinatorTests.cs`가 생성되었다.
- file copy 먼저, metadata 저장 나중 흐름이 구현되었다.
- physical file name 생성은 `FileNamePolicyService` 기준으로 구현되었다.
- source extension은 coordinator가 source file path에서 추출한다.
- duplicateIndex 자동 산정이 구현되었다.
- target exists 시 다음 duplicateIndex 시도가 구현되었다.
- copy result 기준 `DocumentDraft` 생성이 구현되었다.
- `IDocumentStorageService.AddDocumentAsync(...)`를 통해 `DocumentRecord` metadata를 저장한다.
- metadata 저장 실패 시 copied file cleanup이 구현되었다.
- cleanup 실패 시 `AggregateException`으로 metadata failure와 cleanup failure를 모두 노출한다.
- MVP 1차는 Document metadata 저장까지만 처리한다.
- PolicyDocument/ClaimDocument link workflow는 구현하지 않았다.
- WPF UI/ViewModel/file picker는 구현하지 않았다.
- JSON metadata storage 수정은 없다.
- file attachment service 수정은 없다.
- `FileNamePolicyService.cs` 수정은 없다.
- project root `attachments/`, `data/local` 내부 파일 생성은 없다.

## D. Request Model Review

`DocumentAttachmentRequest`는 coordinator 입력 모델이다.

확인 결과:

- `SourceFilePath`가 포함되어 있다.
- `DocumentScope`가 포함되어 있다.
- `DocumentType`이 포함되어 있다.
- `DisplayTitle`이 포함되어 있다.
- `ReferenceDate`가 포함되어 있다.
- `DuplicateIndex`는 포함하지 않는다.
- source extension은 request에 포함하지 않는다.
- 실제 개인정보 저장 필드는 없다.
- raw original file name 저장 필드는 없다.

판정: PASS

## E. Result Model Review

`DocumentAttachmentResult`는 coordinator 성공 결과 모델이다.

확인 결과:

- `DocumentRecord`가 포함되어 있다.
- `FileAttachmentCopyResult`가 포함되어 있다.
- absolute path를 포함하지 않는다.
- 원본 파일명을 포함하지 않는다.
- PolicyDocument/ClaimDocument link 결과를 포함하지 않는다.

판정: PASS

## F. Coordinator Review

`DocumentAttachmentCoordinator`는 source file 선택 이후의 application workflow를 담당한다.

확인 결과:

- constructor에서 `IDocumentStorageService` null guard가 있다.
- constructor에서 `IFileAttachmentService` null guard가 있다.
- public method `AttachDocumentAsync(...)`가 구현되어 있다.
- request null validation이 있다.
- source file path required validation이 있다.
- source file existence validation이 있다.
- source extension extraction이 있다.
- documentScope required validation이 있다.
- documentType required validation이 있다.
- displayTitle required validation이 있다.
- referenceDate validation이 있다.
- duplicateIndex 자동 산정이 있다.
- `FileNamePolicyService.CreatePhysicalFileName(...)`를 호출한다.
- `IFileAttachmentService.CopyDocumentFileAsync(...)`를 호출한다.
- copy result 기준 `DocumentDraft`를 생성한다.
- `IDocumentStorageService.AddDocumentAsync(...)`를 호출한다.
- metadata 저장 성공 시 `DocumentAttachmentResult`를 반환한다.
- metadata 저장 실패 시 `DeleteDocumentFileIfExistsAsync(...)`를 호출한다.
- cleanup 성공 시 원래 metadata 저장 failure를 노출한다.
- cleanup 실패 시 metadata failure와 cleanup failure를 모두 추적 가능하게 노출한다.
- custom exception은 생성하지 않았다.
- PolicyDocument/ClaimDocument link 저장은 없다.
- WPF file picker 실행은 없다.
- UI state 변경은 없다.
- OCR parsing은 없다.
- SQLite/repository 구현은 없다.

판정: PASS

보완 메모:

- 현재 physical file name 생성에 사용하는 id token은 `document` 고정값이다. MVP 1차에서 Policy/Claim link를 보류했기 때문에 가능한 단순화지만, link workflow 도입 시 id source 정책을 다시 결정해야 한다.

## G. DuplicateIndex Review

duplicateIndex는 coordinator가 자동 산정한다.

확인 결과:

- caller/request가 duplicateIndex를 전달하지 않는다.
- coordinator가 1부터 순차 시도한다.
- `FileNamePolicyService.CreatePhysicalFileName(...)` 정책을 사용한다.
- `IFileAttachmentService.DocumentFileExistsAsync(...)`로 target 존재 여부를 확인한다.
- `IDocumentStorageService.GetDocumentsAsync(...)` 결과의 `PhysicalFileName`도 고려한다.
- target exists 또는 metadata에 같은 physical file name이 있으면 다음 duplicateIndex를 시도한다.
- 최종 copy 시 `IOException`이 발생하면 다음 duplicateIndex를 다시 시도한다.
- duplicateIndex 상한은 `FileNamePolicyService` 정책과 동일하게 999로 제한한다.
- 임의 filename policy는 추가하지 않았다.

판정: PASS

보완 메모:

- exists 후 copy 사이 race condition 완전 방지는 MVP 1차 범위 밖이다.
- 모든 duplicateIndex가 소진된 경우 `InvalidOperationException`으로 failure 처리한다.

## H. Rollback / Cleanup Review

metadata 저장 실패 후 cleanup 흐름이 구현되어 있다.

확인 결과:

- file copy 성공 후 metadata save 실패 시 copied file cleanup을 시도한다.
- cleanup은 `IFileAttachmentService.DeleteDocumentFileIfExistsAsync(...)`를 사용한다.
- cleanup 성공 시 원래 metadata save failure를 노출한다.
- cleanup 실패 시 metadata failure와 cleanup failure를 모두 추적 가능하다.
- cleanup 실패를 삼키지 않는다.
- custom exception은 생성하지 않았다.
- cleanup 실패 표현에는 `AggregateException`을 사용한다.

판정: PASS

## I. Test Coverage Review

`DocumentAttachmentCoordinatorTests.cs`에는 28개 테스트가 추가되었다.

### Success Flow

확인 결과:

- successful import copies file and saves document metadata 검증이 있다.
- result contains `DocumentRecord` and `FileAttachmentCopyResult` 검증이 있다.
- copied file exists under temp attachment root 검증이 있다.
- metadata JSON exists only under temp metadata root 검증이 있다.
- result/document contains no absolute path 검증이 있다.
- `DocumentRecord.RelativePath` equals copy result relative path 검증이 있다.
- `DocumentRecord.PhysicalFileName` equals copy result physical file name 검증이 있다.
- `DocumentRecord.Extension` equals copy result extension 검증이 있다.

판정: Covered

### Physical File Name

확인 결과:

- physical file name generated by `FileNamePolicyService` 검증이 있다.
- invalid documentType fails before file copy 검증이 있다.
- invalid extension fails before file copy 검증이 있다.
- source extension is extracted from source file path 검증이 있다.

판정: Covered

### DuplicateIndex

확인 결과:

- duplicateIndex auto starts at 1 검증이 있다.
- duplicateIndex auto increments when target file exists 검증이 있다.
- duplicateIndex auto increments when metadata has same physical file name 검증이 있다.
- target exists at final copy 발생 시 다음 duplicateIndex로 retry되는지 검증이 있다.

판정: Covered

### Validation

확인 결과:

- null request rejected 검증이 있다.
- source path required 검증이 있다.
- missing source file rejected before metadata save 검증이 있다.
- documentScope required 검증이 있다.
- documentType required 검증이 있다.
- displayTitle required 검증이 있다.
- default referenceDate rejected 검증이 있다.

판정: Covered

### Rollback

확인 결과:

- metadata save failure cleans up copied file 검증이 있다.
- cleanup failure is reported 검증이 있다.
- metadata save failure does not leave copied file when cleanup succeeds 검증이 있다.

판정: Covered

### Scope Safety

확인 결과:

- test uses temp directory only.
- project root `attachments/` 내부 파일 생성 없음이 검증되었다.
- project root `data/local` 내부 파일 생성 없음이 검증되었다.
- 실제 개인정보 sample은 사용하지 않는다.
- dummy source file name과 dummy content만 사용한다.

판정: Covered

### Excluded Tests

아래 테스트는 제외 상태다.

- WPF file picker test 없음.
- UI/ViewModel test 없음.
- OCR test 없음.
- SQLite test 없음.
- PolicyDocument/ClaimDocument link test 없음.
- concurrent duplicate import test 없음.
- production `attachments/` permission test 없음.

판정: Scope 유지

## J. Verification Result

검증 명령:

```powershell
dotnet build FamilyClaimRef.sln
dotnet test FamilyClaimRef.sln
```

이번 리뷰 문서 작성 시점의 최신 검증 결과:

- `dotnet build FamilyClaimRef.sln`: PASS
- warning: 0
- error: 0
- `dotnet test FamilyClaimRef.sln`: PASS
- 총 테스트 개수: 141
- 추가된 테스트 개수: 28
- 실패 테스트: 없음
- 실패 원인: 없음
- 권한 상승 실행 여부: 있음
- 초기 실패/재실행 여부: 있음
  - 최초 build는 sandbox의 Windows SDK 경로 접근 제한으로 실패 후 권한 상승 재실행
  - 이후 `System.IO` using 누락 수정
  - 최초 test는 기본 날짜 테스트 입력 문제 1건 수정 후 재실행
- 이번 리뷰 문서 작성 중 재검증:
  - `dotnet build FamilyClaimRef.sln`: PASS, warning 0, error 0
  - `dotnet test FamilyClaimRef.sln`: PASS, 실패 0, 통과 141, 건너뜀 0, 전체 141
- project root `attachments/` 상태:
  - 기존 폴더 존재
  - 내부 파일 없음
- project root `data/local` 상태:
  - 기존 폴더 존재
  - 내부 파일 없음
- temp directory만 사용 여부: 확인

## K. Scope Compliance Review

아래 범위는 지켜졌다.

- JSON metadata storage 수정 없음.
- file attachment service 수정 없음.
- `FileNamePolicyService.cs` 수정 없음.
- storage model 수정 없음.
- 기존 test file 수정 없음.
- `.sln` 수정 없음.
- `.csproj` 수정 없음.
- NuGet package 추가 없음.
- WPF UI/ViewModel/file picker 구현 없음.
- PolicyDocument/ClaimDocument link workflow 구현 없음.
- OCR/SQLite/repository 구현 없음.
- project root `attachments/`, `data/local` 내부 파일 생성 없음.
- 실제 개인정보 샘플 사용 없음.
- Git commit/reset/checkout/add 없음.

## L. Out of Scope / Not Implemented

아래 항목은 아직 구현되지 않았다.

- Policy/Claim link workflow 없음.
- WPF UI/ViewModel 연동 없음.
- file picker 없음.
- duplicateIndex exists 후 copy race condition 완전 방지 없음.
- custom exception 없음.
- UI error classification 없음.
- production `attachments/`, `data/local` 권한 검증 없음.
- concurrent duplicate import test 없음.
- OCR 없음.
- SQLite/repository 없음.

## M. Risks

- Policy/Claim link workflow는 아직 없다.
- WPF UI/ViewModel 연동은 아직 없다.
- duplicateIndex exists 후 copy race condition은 완전 방지하지 못한다.
- custom exception이 없어 UI error 분류는 아직 거칠 수 있다.
- production `attachments/`, `data/local` 권한 문제는 temp test로만 간접 검증되었다.
- file picker와 coordinator 연결 시 source file path trust boundary를 다시 검토해야 한다.
- Policy/Claim link를 후속으로 처리할 때 document metadata 저장 후 link 저장 실패 rollback 정책이 필요하다.
- 현재 physical file name id token 고정값은 link workflow 도입 전까지의 MVP 단순화이며, 후속 workflow에서 id source 정책이 필요하다.

## N. Recommendation

1. 현재 `DocumentAttachmentCoordinator` implementation은 build/test PASS 상태로 고정한다.
2. 다음 작업은 Policy/Claim document link workflow 범위 결정 문서가 적절하다.
3. UI 연결을 먼저 해야 한다면 coordinator를 호출하는 ViewModel boundary 설계를 별도 문서로 분리한다.
4. MVP 데이터 정합성을 우선하면 Policy/Claim link workflow 설계를 먼저 진행한다.
5. WPF UI/ViewModel/file picker 연동은 link workflow 정책 확인 후 진행한다.

## O. Result

`DOCUMENT_ATTACHMENT_COORDINATOR_IMPLEMENTATION_REVIEWED`
