# 59_FILENAME_POLICY_TEST_PROJECT_IMPLEMENTATION_REVIEW

## 1. Goal

이 문서는 `FileNamePolicyService` 자동화 테스트 프로젝트 생성 결과를 기록한다.

이 문서는 구현 결과 리뷰 문서다. production code 수정 문서가 아니며, DB/OCR/metadata/file storage/navigation/WPF UI 구현 문서가 아니다.

## 2. Checked Files / Paths

| 구분 | 경로 | 확인 내용 |
|---|---|---|
| Solution | `FamilyClaimRef.sln` | `FamilyClaimRef.App.Tests` test project가 solution에 포함됨 |
| App Project | `app/FamilyClaimRef.App/FamilyClaimRef.App.csproj` | `net10.0-windows`, production app project에 test package 없음 |
| Production Source | `app/FamilyClaimRef.App/Services/FileNamePolicyService.cs` | production code 수정 없이 테스트 대상 유지 |
| Test Project | `tests/FamilyClaimRef.App.Tests/FamilyClaimRef.App.Tests.csproj` | xUnit test project, `net10.0-windows`, app project reference |
| Test Source | `tests/FamilyClaimRef.App.Tests/FileNamePolicyServiceTests.cs` | N/E/B 케이스 자동화 |
| Test Cases | `docs/53_FILENAME_POLICY_TEST_CASES.md` | patch 이후 테스트 케이스 기준 |
| Manual Review | `docs/54_FILENAME_POLICY_MANUAL_REVIEW_RECORD.md` | patch 이후 수동 검토 기록 |
| Patch Review | `docs/57_FILENAME_POLICY_PATCH_REVIEW.md` | 정책 patch 결과와 이전 build PASS |
| Test Project Scope | `docs/58_FILENAME_POLICY_TEST_PROJECT_SCOPE_DECISION.md` | test project 생성 범위 결정 초안 |

## 3. Implementation Summary

구현 결과는 다음과 같다.

- xUnit test project가 생성되었다.
- test project 경로는 `tests/FamilyClaimRef.App.Tests/`이다.
- test project 이름은 `FamilyClaimRef.App.Tests`이다.
- test project Target Framework는 `net10.0-windows`이다.
- xUnit/NuGet package는 test project에만 추가되었다.
- test project에 app project reference가 추가되었다.
- `FamilyClaimRef.sln`에 test project가 추가되었다.
- production app project에는 test package가 추가되지 않았다.
- `FileNamePolicyService.cs` production code는 수정하지 않았다.

test project package:

- `Microsoft.NET.Test.Sdk`
- `xunit`
- `xunit.runner.visualstudio`

project reference:

- `..\..\app\FamilyClaimRef.App\FamilyClaimRef.App.csproj`

## 4. Test Coverage Summary

자동화된 테스트 범위는 `FileNamePolicyService.CreatePhysicalFileName(...)` 순수 함수에 한정한다.

포함된 범위:

- N1-N7 정상 케이스
- E1-E22 오류 케이스
- B1-B2 경계 케이스
- B4-B5 경계 케이스
- B3은 E19로 검증

총 테스트 개수:

```text
33
```

명확한 기준:

- B3은 `_1000` 정상 출력 테스트가 아니다.
- `duplicateIndex=1000`은 `ArgumentOutOfRangeException`으로 검증한다.
- allowlist 밖 extension `exe`, `zip`, `docx`는 `ArgumentException`으로 검증한다.
- 실제 파일 접근은 테스트하지 않는다.
- 실제 개인정보 또는 실제 기관명 기반 샘플은 사용하지 않는다.

## 5. Verification Result

실행 명령:

```powershell
dotnet build FamilyClaimRef.sln
dotnet test FamilyClaimRef.sln
```

build 결과:

```text
PASS
경고 0개
오류 0개
```

test 결과:

```text
PASS
실패: 0
통과: 33
건너뜀: 0
전체: 33
```

실패 테스트:

```text
없음
```

실패 원인:

```text
없음
```

## 6. Out of Scope

이번 테스트 프로젝트에서 제외된 범위는 다음과 같다.

- DB 테스트 없음
- OCR 테스트 없음
- metadata 테스트 없음
- file storage 테스트 없음
- navigation 테스트 없음
- WPF UI/XAML 테스트 없음
- 실제 파일 접근 테스트 없음
- 실제 개인정보 샘플 없음
- 실제 가족 실명 없음
- 실제 보험사명 없음
- 실제 병원명 없음
- 실제 진단명/진단코드 사례 없음

## 7. Forbidden Scope Check

| 항목 | 결과 |
|---|---|
| production C# 수정 | 수행하지 않음 |
| `FileNamePolicyService.cs` 수정 | 수행하지 않음 |
| test code 수정 | 수행하지 않음 |
| `FileNamePolicyServiceTests.cs` 수정 | 수행하지 않음 |
| XAML 수정 | 수행하지 않음 |
| `.sln` 수정 | 수행하지 않음 |
| `.csproj` 수정 | 수행하지 않음 |
| NuGet package 추가 | 수행하지 않음 |
| test project 추가 생성 | 수행하지 않음 |
| DB 구현 | 수행하지 않음 |
| OCR 구현 | 수행하지 않음 |
| metadata 저장 구현 | 수행하지 않음 |
| 파일 저장/복사 구현 | 수행하지 않음 |
| `attachments/` 내부 파일 생성 | 수행하지 않음 |
| `data/local/` 내부 파일 생성 | 수행하지 않음 |
| 실제 개인정보 샘플 사용 | 수행하지 않음 |

주의:

- 위 표는 이 리뷰 문서 생성 작업 기준이다.
- 이전 단계에서 이미 생성된 test project와 test code는 읽기 대상으로만 확인했다.
- 이번 작업에서 수정 또는 추가한 파일은 이 문서뿐이다.

## 8. Risks

남은 위험은 다음과 같다.

- document type final list가 바뀌면 테스트 케이스 갱신이 필요하다.
- MVP 이후 extension allowlist가 확장되면 테스트 케이스 갱신이 필요하다.
- MVP 이후 `duplicateIndex` 1000 이상 허용 정책이 생기면 E19/B3 기준 갱신이 필요하다.
- 현재 테스트는 `FileNamePolicyService` 순수 함수 범위에만 한정된다.
- DB/OCR/metadata/file storage/navigation/WPF UI 테스트는 아직 없다.

## 9. Recommendation

다음 순서를 권장한다.

1. `FileNamePolicyService` 테스트 기준은 현재 PASS 상태로 고정한다.
2. 다음 구현 후보를 정하기 전에 도메인 모델/저장 방식 중 어느 쪽으로 갈지 결정한다.
3. 바로 기능 개발로 들어가기보다는 다음 중 하나를 선택한다.
   - 문서/메타데이터 저장 모델 결정
   - JSON vs SQLite 로컬 저장 결정
   - `Document` / `PolicyDocument` / `ClaimDocument` 저장 구조 결정
   - `CategoryItem`과 document type 연결 결정
4. UI 구현은 저장 모델 결정 이후로 미룬다.

## 10. Next Step

다음 작업 후보:

- 로컬 저장 방식 결정 문서 작성
- JSON vs SQLite 결정 문서 작성
- `Document` / `PolicyDocument` / `ClaimDocument` 저장 구조 결정
- `CategoryItem`과 document type 연결 정책 결정

후속 작업 전까지 다음은 진행하지 않는다.

- DB/OCR/metadata/file storage 구현
- navigation 구현
- WPF UI/XAML 구현
- 실제 파일 접근 테스트
- 실제 개인정보 샘플 사용

## Result

`FILENAME_POLICY_TEST_PROJECT_IMPLEMENTATION_REVIEWED`
