# 57_FILENAME_POLICY_PATCH_REVIEW

## 1. Goal

이 문서는 `docs/56_FILENAME_POLICY_USER_DECISION_RECORD_BEFORE_PATCH.md` 기준으로 `FileNamePolicyService`에 적용한 최소 정책 patch 결과를 기록한다.

이번 patch의 목표는 다음 두 가지로 제한했다.

- `duplicateIndex > 999` 거부
- extension allowlist 적용

## 2. Approved Scope

수정 허용 범위:

- `app/FamilyClaimRef.App/Services/FileNamePolicyService.cs`

생성 허용 범위:

- `docs/57_FILENAME_POLICY_PATCH_REVIEW.md`
- build 검증으로 생성되는 `bin/`, `obj/` 산출물

유지한 경계:

- 기존 API signature 유지
- 기존 정상 출력 형식 유지
- 기존 scope/documentType/id/date 처리 유지
- 실제 file access 없음
- DB/OCR/metadata access 없음

## 3. Modified Files

수정 파일:

- `app/FamilyClaimRef.App/Services/FileNamePolicyService.cs`

수정 내용:

- `duplicateIndex`가 `1`부터 `999` 사이가 아니면 `ArgumentOutOfRangeException`을 발생하도록 조정했다.
- extension allowlist를 추가했다.
- allowlist는 `pdf`, `jpg`, `jpeg`, `png`로 제한했다.

## 4. Created Files

생성 파일:

- `docs/57_FILENAME_POLICY_PATCH_REVIEW.md`

생성하지 않은 파일:

- test project
- C# test file
- 신규 C# file
- 신규 XAML file
- DB file
- metadata file
- OCR file
- actual document file
- `package.json`
- `tsconfig.json`
- sample/mock data file

## 5. Patch Summary

적용한 patch:

1. `duplicateIndex` validation
   - null은 suffix 없음으로 유지
   - `1`부터 `999`는 허용
   - `0` 이하와 `1000` 이상은 `ArgumentOutOfRangeException`
2. extension allowlist
   - 입력 앞의 dot 유무 허용
   - 내부 정규화 후 소문자 비교
   - 출력에는 dot 1개 포함 유지
   - `pdf`, `jpg`, `jpeg`, `png`만 허용
   - 그 외 extension은 `ArgumentException`

수정하지 않은 동작:

- `claim` scope는 `claim-` prefix
- `policy` scope는 `policy-` prefix
- scope trim 및 lowercase
- id 안전 문자 검증
- 날짜 `yyyyMMdd` 출력
- scope별 document type 검증
- duplicate suffix format `_001`, `_002`, `_999`
- 파일, DB, OCR, metadata 접근 없음

## 6. Behavior Changes

변경 전:

- `duplicateIndex=1000`이 `_1000` suffix로 허용될 수 있었다.
- extension은 안전 문자만 통과하면 허용될 수 있었다.

변경 후:

- `duplicateIndex=1000`은 `ArgumentOutOfRangeException`으로 거부된다.
- `exe`, `zip`, `docx` 등 allowlist 밖 extension은 `ArgumentException`으로 거부된다.
- `pdf`, `jpg`, `jpeg`, `png`는 허용된다.

## 7. Validation Boundary

이번 patch가 검증하는 것:

- suffix index 범위
- extension 안전 문자
- extension allowlist

이번 patch가 검증하지 않는 것:

- 실제 파일 존재 여부
- 파일 저장 또는 복사
- DB 조회
- metadata 조회 또는 저장
- OCR 상태 조회 또는 OCR 실행
- 중복 파일 자동 탐색
- file hash 계산
- `displayTitle` 생성
- raw `originalFileName` 처리
- 민감정보 자동 탐지

## 8. Forbidden Scope Check

| 항목 | 결과 |
|---|---|
| `.sln` 수정 | 수행하지 않음 |
| `.csproj` 수정 | 수행하지 않음 |
| 다른 C# 파일 수정 | 수행하지 않음 |
| XAML 파일 수정 | 수행하지 않음 |
| test project 생성 | 수행하지 않음 |
| C# test file 생성 | 수행하지 않음 |
| NuGet package 추가 | 수행하지 않음 |
| DB file 생성 | 수행하지 않음 |
| OCR file 생성 | 수행하지 않음 |
| metadata file 생성 | 수행하지 않음 |
| actual document file 생성 | 수행하지 않음 |
| `attachments/` 내부 파일 생성 | 수행하지 않음 |
| `data/local/` 내부 파일 생성 | 수행하지 않음 |
| sample/mock data 생성 | 수행하지 않음 |
| navigation 구현 | 수행하지 않음 |
| `LocalDocumentService` 구현 | 수행하지 않음 |
| `DocumentMetadataService` 구현 | 수행하지 않음 |

## 9. Build Result

실행 명령:

```bat
dotnet build FamilyClaimRef.sln
```

결과:

```text
FamilyClaimRef.App -> C:\EtcProject\FamilyClaimRef\app\FamilyClaimRef.App\bin\Debug\net10.0-windows\FamilyClaimRef.App.dll

빌드했습니다.
    경고 0개
    오류 0개
```

판정:

```text
PASS
```

## 10. Test Case Impact

기존 자동화 후보 영향:

| Case Range | 영향 |
|---|---|
| N1-N7 | Auto Candidate 유지 |
| E1-E18 | Auto Candidate 유지 |
| B1-B2 | Auto Candidate 유지 |
| B3 | 오류 케이스로 전환 |
| B4-B5 | Auto Candidate 유지 |

추가 오류 케이스 후보:

| ID | Input | Expected Exception | Purpose |
|---|---|---|---|
| E19 | `duplicateIndex=1000` | `ArgumentOutOfRangeException` | suffix 최댓값 999 초과 거부 |
| E20 | extension `exe` | `ArgumentException` | 실행 파일 확장자 거부 |
| E21 | extension `zip` | `ArgumentException` | 압축 파일 확장자 거부 |
| E22 | extension `docx` | `ArgumentException` | MVP 미허용 문서 확장자 거부 |

주의:

- test project는 생성하지 않았다.
- C# 테스트 파일은 생성하지 않았다.
- `dotnet test`는 실행하지 않았다.

## 11. Risks

남은 위험은 다음과 같다.

- 자동화 테스트 프로젝트가 없어 입력/출력 케이스는 아직 자동 회귀 검증되지 않았다.
- document type final list는 현재 후보 상수 기준이며 `CategoryItem` 연결 이후 바뀔 수 있다.
- 날짜 의미와 id 생성은 호출자 책임으로 남아 있다.
- allowlist 확장 여부는 MVP 이후 별도 결정이 필요하다.
- 실제 파일 중복 탐색은 수행하지 않으므로 `duplicateIndex` 산정은 호출자 책임이다.

## 12. Recommendation

다음 순서를 권장한다.

1. `docs/53_FILENAME_POLICY_TEST_CASES.md`를 patch 이후 기준으로 갱신한다.
2. 문서 기반 수동 검토 기록을 갱신한다.
3. 이후 test project 생성 여부를 별도 승인받는다.
4. 자동화 테스트 전환 시 production project에는 test package를 추가하지 않는다.

## 13. Next Step

다음 작업 후보:

```text
docs/58_FILENAME_POLICY_TEST_CASES_AFTER_PATCH.md
```

또는 자동화 테스트를 시작하려면 먼저 다음 결정을 별도 문서로 정리한다.

```text
FileNamePolicyService test project scope decision
```

## Result

`FILENAME_POLICY_PATCH_BUILD_PASS`
