# 51_FILENAME_POLICY_SERVICE_IMPLEMENTATION_REVIEW

## 1. Goal

이 문서는 `FileNamePolicyService` 첫 구현 결과를 기록한다.

이번 작업은 첫 C# 코드 구현 작업이며, 범위는 순수 `physicalFileName` 문자열 생성 정책 함수 1개 class로 제한했다. 파일 저장, 파일 복사, DB, OCR, metadata 저장, navigation, ViewModel, XAML 화면 구현은 수행하지 않았다.

## 2. Approved Scope

승인된 범위는 다음과 같다.

- `app/FamilyClaimRef.App/Services/FileNamePolicyService.cs` 생성
- `FileNamePolicyService.CreatePhysicalFileName(...)` 구현
- 문자열 수준 validation 구현
- `dotnet build FamilyClaimRef.sln` 검증
- 구현 결과 문서 생성

범위 밖 항목은 다음과 같다.

- 기존 C# 파일 수정
- 기존 XAML 파일 수정
- `.csproj`, `.sln`, NuGet package 수정
- 파일 저장 또는 파일 복사
- `attachments/` 내부 파일 생성
- `data/local/` 내부 파일 생성
- DB/OCR/metadata 구현
- `LocalDocumentService`, `DocumentMetadataService` 구현
- `ViewModelBase`, `RelayCommand`, `NavigationService` 구현

## 3. Created Files

생성 파일:

- `app/FamilyClaimRef.App/Services/FileNamePolicyService.cs`
- `docs/51_FILENAME_POLICY_SERVICE_IMPLEMENTATION_REVIEW.md`

허용된 build 산출물:

- `bin/`
- `obj/`

## 4. Modified Files

수정 파일:

- 없음

기존 파일은 수정하지 않았다.

## 5. Implementation Summary

`FileNamePolicyService`는 입력값을 받아 안전한 `physicalFileName` 문자열을 생성한다.

구현된 책임:

- `documentScope`를 `claim` 또는 `policy`로 검증하고 소문자로 정규화
- `claim`은 `claim-`, `policy`는 `policy-` prefix 사용
- `id`는 ASCII 영문, 숫자, hyphen, underscore만 허용
- `date`는 `yyyyMMdd` 형식으로 출력
- `documentType`은 scope별 허용 후보 안에서 검증
- `extension`은 앞의 dot 유무를 허용하고 출력에는 dot 포함
- `duplicateIndex`가 있으면 `_001`, `_002` 형식 suffix 출력
- `duplicateIndex`가 0 이하이면 `ArgumentOutOfRangeException` 발생

구현하지 않은 책임:

- 실제 파일 존재 여부 확인
- 디스크 접근
- 파일 생성 또는 복사
- DB 조회
- metadata 조회 또는 저장
- OCR 상태 조회 또는 OCR 실행
- 중복 파일 자동 탐색
- 파일 hash 계산
- `displayTitle` 생성
- raw `originalFileName` 저장
- 민감정보 자동 탐지 엔진

## 6. API Summary

생성된 public API:

```csharp
public static class FileNamePolicyService
{
    public static string CreatePhysicalFileName(
        string documentScope,
        string id,
        DateOnly date,
        string documentType,
        string extension,
        int? duplicateIndex = null)
}
```

출력 형식:

```text
claim-000001_20260626_receipt.pdf
claim-000001_20260626_receipt_001.pdf
policy-000003_20260626_terms.pdf
policy-000003_20260626_terms_001.pdf
```

위 예시는 구조 예시이며 실제 개인정보, 실제 가족 실명, 실제 보험사명, 실제 병원명, 실제 진단명, 실제 진단코드 기반 개인 사례를 포함하지 않는다.

## 7. Validation Boundary

허용 validation:

- `documentScope` 필수값 확인
- `documentScope` 허용값 확인
- `id` 필수값 확인
- `id` 안전 문자 확인
- `documentType` 필수값 확인
- `documentType` scope별 허용 후보 확인
- `extension` 필수값 확인
- `extension` 안전 문자 확인
- `duplicateIndex` 1 이상 확인

금지 validation:

- 실제 파일 존재 여부 확인
- 디스크 접근
- DB 조회
- OCR 상태 조회
- 문서 metadata 조회
- 중복 파일 자동 탐색
- 파일 hash 계산
- 민감정보 자동 판별

## 8. Forbidden Scope Check

금지 범위 확인 결과는 다음과 같다.

| 항목 | 결과 |
|---|---|
| 기존 C# 파일 수정 | 수행하지 않음 |
| 기존 XAML 파일 수정 | 수행하지 않음 |
| `.csproj` 수정 | 수행하지 않음 |
| `.sln` 수정 | 수행하지 않음 |
| NuGet package 추가 | 수행하지 않음 |
| 신규 ViewModel 생성 | 수행하지 않음 |
| 신규 Model 생성 | 수행하지 않음 |
| 신규 XAML 생성 | 수행하지 않음 |
| DB 파일 생성 | 수행하지 않음 |
| OCR 파일 생성 | 수행하지 않음 |
| metadata 파일 생성 | 수행하지 않음 |
| 실제 문서 파일 생성 | 수행하지 않음 |
| `attachments/` 내부 파일 생성 | 수행하지 않음 |
| `data/local/` 내부 파일 생성 | 수행하지 않음 |
| navigation 구현 | 수행하지 않음 |
| `LocalDocumentService` 구현 | 수행하지 않음 |
| `DocumentMetadataService` 구현 | 수행하지 않음 |

## 9. Build Result

실행 명령:

```bat
dotnet build FamilyClaimRef.sln
```

첫 sandbox 실행 결과:

- 실패
- 원인: sandbox가 `C:\Users\jin8855\AppData\Local\Microsoft SDKs` 접근을 막아 `Access to the path` 오류 발생
- 조치: 동일한 build 명령을 권한 상승으로 재실행

첫 권한 상승 build 결과:

- 실패
- 원인: `FileNamePolicyService.cs`의 `StartsWith` 호출에서 잘못된 overload 사용
- 조치: 동일 파일 안에서 extension dot 확인 로직을 char index 비교로 수정

최종 권한 상승 build 결과:

```text
FamilyClaimRef.App -> C:\EtcProject\FamilyClaimRef\app\FamilyClaimRef.App\bin\Debug\net10.0-windows\FamilyClaimRef.App.dll

빌드했습니다.
    경고 0개
    오류 0개
```

결과:

```text
Build: PASS
```

## 10. Risks

남은 위험은 다음과 같다.

- unit test project 생성이 금지되어 자동화된 입력/출력 케이스 검증은 아직 없다.
- `documentType` 후보는 상수로 구현했으나 최종 목록은 이후 `CategoryItem` 또는 DB 설계에서 바뀔 수 있다.
- 날짜 기준은 호출자가 넘기는 `DateOnly`에 의존한다. 진료일, 등록일, 문서 발행일 중 무엇인지 함수 내부에서 판단하지 않는다.
- `duplicateIndex`는 호출자가 넘긴 값을 반영할 뿐 실제 파일 중복을 탐색하지 않는다.
- 민감정보 자동 탐지는 구현하지 않았으므로 호출 전 입력 정책과 UI 경고가 별도로 필요하다.

## 11. Recommendation

다음 순서를 권장한다.

1. `FileNamePolicyService` 입력/출력 예시를 수동 검토한다.
2. unit test project 생성 승인 여부를 별도 결정한다.
3. `documentType` 최종 목록과 `CategoryItem` 연결 여부를 결정한다.
4. 날짜 기준을 청구 문서와 보험 문서별로 결정한다.
5. 실제 파일 복사 또는 `attachments/` 저장 구현은 별도 승인 전까지 진행하지 않는다.

## 12. Next Step

다음 작업 후보:

```text
docs/52_FILENAME_POLICY_SERVICE_TEST_SCOPE_DECISION.md
```

또는 사용자가 바로 수동 검토를 원하면 다음을 확인한다.

- `claim`, `policy` prefix 결과
- `yyyyMMdd` 날짜 포맷
- scope별 `documentType` 허용 목록
- `_001`, `_002` suffix 출력
- dot이 있거나 없는 extension 입력 처리
- 금지 문자를 가진 `id`, `extension` 입력 거부

## Result

`FILENAME_POLICY_SERVICE_IMPLEMENTED_BUILD_PASS`
