# File Attachment Service Implementation Review

## A. Goal

이 문서는 `IFileAttachmentService` 구현 결과 리뷰 문서다.

기록 대상은 다음과 같다.

- `IFileAttachmentService` 구현 결과
- `FileAttachmentCopyResult` 구현 결과
- `LocalFileAttachmentService` 구현 결과
- `IFileAttachmentServiceTests` 구현 결과
- 구현 범위 준수 여부
- build/test 검증 결과
- 남은 위험과 후속 추천 작업

이 문서는 다음 작업의 리뷰가 아니다.

- JSON metadata storage 구현 리뷰가 아니다.
- coordinator/application service 구현 리뷰가 아니다.
- WPF UI/ViewModel 구현 리뷰가 아니다.
- OCR, SQLite, repository/data access 구현 리뷰가 아니다.

## B. Checked Files / Paths

| 대상 | 확인 목적 | 판정 |
|---|---|---|
| `docs/89_FILE_ATTACHMENT_SERVICE_USER_DECISION_RECORD.md` | 사용자 결정 기준 확인 | PASS |
| `docs/88_FILE_ATTACHMENT_SERVICE_SCOPE_DESIGN.md` | file attachment scope 설계 기준 확인 | PASS |
| `docs/87_JSON_STORAGE_IMPLEMENTATION_REVIEW.md` | JSON metadata storage와 actual file storage 분리 기준 확인 | PASS |
| `app/FamilyClaimRef.App/Services/Storage/IFileAttachmentService.cs` | file attachment interface 구현 확인 | PASS |
| `app/FamilyClaimRef.App/Services/Storage/FileAttachmentCopyResult.cs` | copy result model 구현 확인 | PASS |
| `app/FamilyClaimRef.App/Services/Storage/LocalFileAttachmentService.cs` | local file system implementation 확인 | PASS |
| `tests/FamilyClaimRef.App.Tests/IFileAttachmentServiceTests.cs` | file attachment tests 구현 확인 | PASS |
| `app/FamilyClaimRef.App/Services/Storage/JsonDocumentStorageService.cs` | actual file copy 미포함 유지 확인 | PASS |
| `app/FamilyClaimRef.App/Services/Storage/JsonFileStore.cs` | JSON metadata helper 수정 여부 확인 | PASS |
| `app/FamilyClaimRef.App/Services/Storage/IDocumentStorageService.cs` | metadata storage interface 수정 여부 확인 | PASS |
| `app/FamilyClaimRef.App/Services/FileNamePolicyService.cs` | physical file name 생성 책임 분리 확인 | PASS |
| `FamilyClaimRef.sln` | solution 구성 확인 | PASS |
| `app/FamilyClaimRef.App/FamilyClaimRef.App.csproj` | app project 구성 확인 | PASS |
| `tests/FamilyClaimRef.App.Tests/FamilyClaimRef.App.Tests.csproj` | test project 구성 확인 | PASS |

## C. Implementation Summary

- `IFileAttachmentService.cs`가 생성되었다.
- `FileAttachmentCopyResult.cs`가 생성되었다.
- `LocalFileAttachmentService.cs`가 생성되었다.
- `IFileAttachmentServiceTests.cs`가 생성되었다.
- Candidate B document-specific copy service가 구현되었다.
- attachment root 생성자 주입이 구현되었다.
- `documents/<physicalFileName>` relative path 반환이 구현되었다.
- absolute path metadata/result 미노출 기준이 구현되었다.
- source missing failure가 구현되었다.
- target exists failure가 구현되었다.
- path traversal failure가 구현되었다.
- delete-if-exists primitive가 구현되었다.
- exists primitive가 구현되었다.
- temp directory 기반 tests가 구현되었다.
- JSON metadata storage 직접 수정은 없다.
- `JsonDocumentStorageService` actual file copy 없음이 유지되었다.
- coordinator/application service는 구현하지 않았다.
- project root `attachments/`, `data/local` 내부 파일 생성은 없다.

## D. Interface Review

`IFileAttachmentService`는 document-specific actual file primitive interface다.

확인 결과:

- `CopyDocumentFileAsync(...)`가 포함되어 있다.
- `DeleteDocumentFileIfExistsAsync(...)`가 포함되어 있다.
- `DocumentFileExistsAsync(...)`가 포함되어 있다.
- JSON metadata를 직접 수정하지 않는다.
- `JsonDocumentStorageService`를 참조하지 않는다.
- `FileNamePolicyService.CreatePhysicalFileName(...)`을 직접 호출하지 않는다.
- physical file name 생성은 caller/coordinator 책임으로 남아 있다.
- actual file copy/delete/exists primitive만 제공한다.

판정: PASS

## E. Result Model Review

`FileAttachmentCopyResult`는 file copy 결과를 나타내는 record다.

확인 결과:

- `RelativePath`가 포함되어 있다.
- `PhysicalFileName`이 포함되어 있다.
- `Extension`이 포함되어 있다.
- `SizeBytes`가 포함되어 있다.
- `RelativePath`는 attachment root 기준 relative path로 사용된다.
- absolute path를 반환하지 않는 구조다.
- 원본 파일명을 저장하지 않는다.
- 실제 개인정보 정보를 포함하지 않는다.

판정: PASS

## F. LocalFileAttachmentService Review

`LocalFileAttachmentService`는 local file system 기반 document attachment implementation이다.

확인 결과:

- attachment root path를 생성자에서 주입받는다.
- constructor에서는 root path를 full path로 normalize한다.
- test temp directory 사용이 가능하다.
- service 내부에서 project root `attachments/`를 고정 생성하지 않는다.
- document files는 root 하위 `documents/` 기준으로 저장한다.
- 반환 relative path는 `/` separator 기준이다.
- Windows 내부 file path는 `Path.Combine`, `Path.GetFullPath`, separator 변환을 사용한다.
- source file missing은 `FileNotFoundException` failure다.
- `physicalFileName` null/empty/whitespace는 `ArgumentException` failure다.
- `physicalFileName`에 directory separator 포함 시 `ArgumentException` failure다.
- absolute physical file name은 `ArgumentException` failure다.
- `..` path traversal은 `ArgumentException` failure다.
- target full path가 attachment root 밖이면 `ArgumentException` failure다.
- target file already exists는 `IOException` failure다.
- source와 target이 같은 full path이면 `InvalidOperationException` failure다.
- target directory 생성은 copy 시점에 수행한다.
- overwrite는 금지되어 있다.
- delete existing file이 가능하다.
- delete missing file은 no-op이다.
- exists primitive가 동작한다.
- JSON metadata를 읽거나 쓰지 않는다.
- `attachments/` project root를 직접 사용하지 않는다.

판정: PASS

보완 후보:

- 현재 file operation은 동기 `File.Copy`, `File.Delete`, `File.Exists`를 `Task` wrapper 형태로 제공한다. MVP 1차 범위에서는 허용 가능하지만, 대용량 파일이나 UI thread 호출 경계에서는 비동기 offload 정책 검토가 필요하다.
- `FileNamePolicyService`가 만든 physical file name을 전제로 하므로 duplicateIndex 산정과 extension allowlist 검증은 caller/coordinator에 남아 있다.

## G. Test Coverage Review

`IFileAttachmentServiceTests.cs`에는 24개 테스트가 추가되었다.

### Constructor / root

확인 결과:

- invalid root path rejected 검증이 있다.
- temp root로 service 생성 가능 검증이 있다.

판정: Covered

### Copy

확인 결과:

- missing source file rejected 검증이 있다.
- copy creates target file under temp attachment root 검증이 있다.
- copied file content matches source 검증이 있다.
- result relative path is not absolute 검증이 있다.
- result relative path uses `documents/` 검증이 있다.
- result physical file name equals input 검증이 있다.
- result extension is target extension 검증이 있다.
- result size bytes matches copied file 검증이 있다.
- target existing rejected 검증이 있다.
- physical file name with path traversal rejected 검증이 있다.
- physical file name with directory separator rejected 검증이 있다.
- absolute physical file name rejected 검증이 있다.

판정: Covered

### Exists

확인 결과:

- existing copied file returns true 검증이 있다.
- missing relative path returns false 검증이 있다.
- absolute relative path rejected 검증이 있다.
- path traversal relative path rejected 검증이 있다.

판정: Covered

### Delete

확인 결과:

- delete existing copied file removes file 검증이 있다.
- delete missing file does not fail 검증이 있다.
- path traversal relative path rejected 검증이 있다.

판정: Covered

### Scope safety

확인 결과:

- test uses temp directory only.
- project root `attachments/` 내부 파일 생성 없음이 검증되었다.
- project root `data/local` 내부 파일 생성 없음이 검증되었다.
- 실제 개인정보 샘플은 사용하지 않는다.
- dummy file name과 dummy file content만 사용한다.

판정: Covered

## H. Verification Result

검증 명령:

```powershell
dotnet build C:\EtcProject\FamilyClaimRef\FamilyClaimRef.sln
dotnet test C:\EtcProject\FamilyClaimRef\FamilyClaimRef.sln
```

이번 리뷰 문서 작성 시점의 최신 검증 결과:

- `dotnet build FamilyClaimRef.sln`: PASS
- warning: 0
- error: 0
- `dotnet test FamilyClaimRef.sln`: PASS
- 총 테스트 개수: 113
- 추가된 테스트 개수: 24
- 실패 테스트: 없음
- 실패 원인: 없음
- 권한 상승 실행 여부: 있음
- 초기 실패/재실행 여부: 없음
- project root `attachments/` 내부 파일 생성 여부: 없음
- project root `data/local` 내부 파일 생성 여부: 없음
- temp directory만 사용 여부: 확인
- Git 상태: 현재 경로가 Git 저장소가 아니어서 `git status` 실패

## I. Scope Compliance Review

아래 범위는 지켜졌다.

- JSON metadata storage 수정 없음.
- `JsonDocumentStorageService.cs` 수정 없음.
- `JsonFileStore.cs` 수정 없음.
- `IDocumentStorageService.cs` 수정 없음.
- `FileNamePolicyService.cs` 수정 없음.
- storage model 수정 없음.
- 기존 test file 수정 없음.
- `.sln` 수정 없음.
- `.csproj` 수정 없음.
- NuGet package 추가 없음.
- coordinator/application service 구현 없음.
- WPF UI/XAML/navigation/ViewModel 구현 없음.
- file picker 구현 없음.
- OCR 구현 없음.
- SQLite DB/package 추가 없음.
- repository/data access/migration 구현 없음.
- project root `attachments/` 내부 파일 생성 없음.
- project root `data/local` 내부 파일 생성 없음.
- 실제 개인정보 샘플 사용 없음.
- Git commit/reset/checkout/add 없음.

## J. Out of Scope / Not Implemented

아래 항목은 아직 구현되지 않았다.

- file copy와 metadata save 간 rollback 없음.
- coordinator/application service 없음.
- duplicateIndex 산정 책임은 caller/coordinator에 남아 있음.
- actual production `attachments/` 권한 문제 검증 없음.
- UI file picker 연동 없음.
- JSON metadata와 actual file storage transaction boundary 없음.
- metadata 저장 실패 시 copied file cleanup orchestration 없음.
- Policy/Claim link까지 포함한 document registration workflow 없음.
- WPF UI/ViewModel integration 없음.

## K. Risks

- file copy와 metadata save 간 rollback은 아직 없다.
- coordinator/application service가 아직 없어 사용자 작업 단위의 atomicity가 없다.
- duplicateIndex 산정 책임이 caller/coordinator에 남아 있다.
- actual production `attachments/` 권한 문제는 아직 검증하지 않았다.
- UI file picker와 service boundary가 아직 연결되지 않았다.
- JSON metadata와 actual file storage transaction boundary는 아직 없다.
- target exists 검증은 있지만 동시 copy race condition은 완전히 막지 못할 수 있다.
- antivirus / OneDrive / file lock 환경에서 copy 실패 가능성이 있다.
- delete-if-exists cleanup 실패를 상위에서 어떻게 처리할지 아직 정해지지 않았다.
- synchronous file operation을 UI path에서 직접 호출하면 responsiveness 문제가 생길 수 있다.

## L. Recommendation

1. 현재 `IFileAttachmentService` implementation은 build/test PASS 상태로 고정한다.
2. 다음 작업은 coordinator/application service 범위 결정 문서가 적절하다.
3. coordinator는 file copy 먼저, metadata 저장 나중, metadata failure 시 cleanup을 다뤄야 한다.
4. physical file name 생성은 coordinator가 `FileNamePolicyService`로 수행하는 방향을 검토한다.
5. UI/ViewModel 연동은 coordinator 이후로 보류한다.

## M. Result

`FILE_ATTACHMENT_SERVICE_IMPLEMENTATION_REVIEWED`
