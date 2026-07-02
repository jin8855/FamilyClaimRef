# 52_FILENAME_POLICY_SERVICE_TEST_SCOPE_DECISION

## 1. Goal

이 문서는 `FileNamePolicyService` 첫 구현 이후 후속 테스트 범위를 결정하기 위한 기준을 정리한다.

이번 작업은 테스트 구현 작업이 아니다. test project 생성, C# 테스트 파일 생성, `.sln` 수정, `.csproj` 수정, NuGet package 추가, production code 수정, DB/OCR/metadata 구현, 파일 저장 구현은 수행하지 않는다.

목표는 다음과 같다.

- 현재 `FileNamePolicyService` 구현 책임을 요약한다.
- 필요한 정상/오류 테스트 케이스 후보를 정리한다.
- 수동 검토, 별도 test project, 임시 runner, build-only 방식의 장단점을 비교한다.
- test project 생성과 package 추가가 필요한 경우의 주의사항을 정리한다.
- 다음 Codex 지시에서 사용할 사용자 결정 질문을 정리한다.

## 2. Checked Files / Paths

| 구분 | 경로 | 확인 내용 |
|---|---|---|
| Filename Policy Scope | `docs/48_FILENAME_POLICY_IMPLEMENTATION_SCOPE_DECISION.md` | 순수 파일명 정책 함수의 입력/출력/validation 경계 |
| Implementation Review | `docs/51_FILENAME_POLICY_SERVICE_IMPLEMENTATION_REVIEW.md` | 구현 결과, build 결과, 남은 위험 |
| Service Source | `app/FamilyClaimRef.App/Services/FileNamePolicyService.cs` | 현재 public API와 validation 구현 |
| WPF Project | `app/FamilyClaimRef.App/FamilyClaimRef.App.csproj` | `net10.0-windows`, package 추가 없음 |
| Solution | `FamilyClaimRef.sln` | app project만 포함, test project 없음 |

## 3. Scope

이 문서의 범위는 다음으로 제한한다.

- 테스트 케이스 후보 정리
- 테스트 방식 후보 비교
- 수동 검토와 자동화 테스트 범위 분리
- test project 생성 시 주의사항 정리
- 사용자 결정 질문과 권장 기본 답변 정리

이 문서의 범위 밖 항목은 다음과 같다.

- test project 생성
- C# 테스트 코드 작성
- production C# 코드 수정
- `.sln`, `.csproj`, Target Framework 수정
- NuGet package 추가
- DB/OCR/metadata 구현
- 파일 저장 또는 파일 복사 구현
- `attachments/`, `data/local` 내부 파일 생성

## 4. Current Implementation Summary

현재 public API는 다음과 같다.

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

현재 구현 책임:

- `claim` / `policy` scope 검증
- scope 입력값 trim 및 소문자 정규화
- `claim-` / `policy-` prefix 적용
- `yyyyMMdd` 날짜 포맷 적용
- scope별 document type 검증
- dot이 있거나 없는 extension 입력 정규화
- extension 소문자 정규화
- duplicate index가 있으면 `_001`, `_002` suffix 적용
- `id`, `extension` 안전 문자 validation
- 잘못된 입력에 대해 `ArgumentException` 또는 `ArgumentOutOfRangeException` 발생

명시적 범위 밖:

- 파일 접근
- DB 접근
- OCR 접근
- metadata 저장
- 실제 중복 탐색
- file hash 계산
- `displayTitle` 생성
- raw `originalFileName` 저장
- 민감정보 자동 탐지

## 5. Test Case Candidates

테스트 케이스 후보는 구조 예시만 사용한다. 실제 가족 실명, 실제 보험사명, 실제 병원명, 실제 진단명, 실제 진단코드 기반 개인 사례는 사용하지 않는다.

### 정상 케이스

| 번호 | 입력 | 기대 결과 | 목적 |
|---|---|---|---|
| N1 | `claim`, `000001`, `2026-06-26`, `receipt`, `pdf` | `claim-000001_20260626_receipt.pdf` | 기본 청구 문서 파일명 |
| N2 | `claim`, `000001`, `2026-06-26`, `receipt`, `.pdf`, `1` | `claim-000001_20260626_receipt_001.pdf` | duplicate suffix `_001` |
| N3 | `policy`, `000003`, `2026-06-26`, `terms`, `pdf` | `policy-000003_20260626_terms.pdf` | 기본 보험 문서 파일명 |
| N4 | `policy`, `000003`, `2026-06-26`, `terms`, `.pdf`, `2` | `policy-000003_20260626_terms_002.pdf` | duplicate suffix `_002` |
| N5 | ` Claim `, `ID_001`, `2026-06-26`, `receipt`, `PDF` | `claim-ID_001_20260626_receipt.pdf` | scope trim, scope 소문자화, extension 소문자화 |
| N6 | `policy`, `POLICY-001`, `2026-06-26`, `capture`, `.PNG` | `policy-POLICY-001_20260626_capture.png` | hyphen id와 대문자 extension |
| N7 | `claim`, `CLAIM_001`, `2026-06-26`, `etc`, `jpg` | `claim-CLAIM_001_20260626_etc.jpg` | underscore id와 `etc` |

### 오류 케이스

| 번호 | 입력 조건 | 기대 예외 | 목적 |
|---|---|---|---|
| E1 | `documentScope`가 null | `ArgumentException` | 필수값 검증 |
| E2 | `documentScope`가 empty 또는 whitespace | `ArgumentException` | 공백 입력 거부 |
| E3 | `documentScope`가 `other` | `ArgumentException` | 허용 scope 제한 |
| E4 | `id`가 null | `ArgumentException` | 필수값 검증 |
| E5 | `id`가 empty 또는 whitespace | `ArgumentException` | 공백 입력 거부 |
| E6 | `id`에 공백 포함 | `ArgumentException` | 파일명 안전 문자 제한 |
| E7 | `id`에 slash 또는 backslash 포함 | `ArgumentException` | 경로 구분자 차단 |
| E8 | `id`에 colon 또는 wildcard 포함 | `ArgumentException` | 파일명 위험 문자 차단 |
| E9 | `documentType`이 null | `ArgumentException` | 필수값 검증 |
| E10 | `documentType`이 empty 또는 whitespace | `ArgumentException` | 공백 입력 거부 |
| E11 | `claim` scope에 `terms` 입력 | `ArgumentException` | scope별 document type 검증 |
| E12 | `policy` scope에 `receipt` 입력 | `ArgumentException` | scope별 document type 검증 |
| E13 | `extension`이 null | `ArgumentException` | 필수값 검증 |
| E14 | `extension`이 empty 또는 whitespace | `ArgumentException` | 공백 입력 거부 |
| E15 | `extension`에 slash 또는 backslash 포함 | `ArgumentException` | 경로 구분자 차단 |
| E16 | `extension`이 `.`만 있음 | `ArgumentException` | 빈 확장자 거부 |
| E17 | `duplicateIndex`가 0 | `ArgumentOutOfRangeException` | suffix index 하한 검증 |
| E18 | `duplicateIndex`가 음수 | `ArgumentOutOfRangeException` | suffix index 하한 검증 |

### 경계 케이스

| 번호 | 입력 조건 | 기대 결과 | 목적 |
|---|---|---|---|
| B1 | `duplicateIndex` null | suffix 없음 | 기본 suffix 생략 |
| B2 | `duplicateIndex` 999 | `_999` | 세 자리 suffix |
| B3 | `duplicateIndex` 1000 | `_1000` | 세 자리 초과 입력 처리 확인 |
| B4 | extension 앞 dot 있음 | dot 1개만 출력 | `.pdf` 입력 정규화 |
| B5 | extension 앞 dot 없음 | dot 포함 출력 | `pdf` 입력 정규화 |

주의:

- B3은 현재 구현상 `D3` 포맷이므로 1000 이상은 `_1000`으로 출력된다.
- 999 초과를 허용할지 제한할지는 후속 정책 결정 후보이다.

## 6. Test Strategy Options

### Option A. 문서 기반 수동 케이스 검토

내용:

- 테스트 케이스 표만 문서화한다.
- 코드, test project, package를 추가하지 않는다.
- 수동 리뷰 또는 코드 리뷰 기준으로 사용한다.

장점:

- 현재 금지 범위를 가장 잘 지킨다.
- `.sln`, `.csproj`, production code 영향이 없다.
- test framework 선택을 뒤로 미룰 수 있다.

단점:

- 자동 회귀 검증이 불가능하다.
- 함수 변경 시 사람이 다시 확인해야 한다.

판정 후보:

- 즉시 가능
- 현재 단계의 보수적 권장안

### Option B. 별도 test project 생성

내용:

- 예: `FamilyClaimRef.App.Tests`
- xUnit, MSTest, NUnit 중 하나를 선택한다.
- `.sln`에 test project를 추가한다.
- test project에만 test package를 추가한다.
- production project에는 test package를 추가하지 않는다.

장점:

- 자동화된 회귀 검증이 가능하다.
- 입력/출력과 예외 케이스를 반복 검증할 수 있다.
- 향후 정책 변경 시 안전망이 된다.

단점:

- test project 생성 승인이 필요하다.
- `.sln` 수정 승인이 필요하다.
- NuGet package 추가 승인이 필요할 가능성이 높다.
- 현재 금지 범위 밖이다.

판정 후보:

- 사용자 승인 필요

### Option C. 임시 console/manual runner 생성

내용:

- 임시 console app 또는 실행 코드로 결과를 확인한다.

장점:

- 빠르게 입출력 결과를 확인할 수 있다.

단점:

- 임시 코드가 남을 위험이 있다.
- production 구조를 오염시킬 수 있다.
- `.sln`, `.csproj`, C# 파일 생성 범위를 건드릴 가능성이 높다.
- 자동화 테스트 체계로 이어지기 어렵다.

판정 후보:

- 비추천

### Option D. 현재처럼 build만 유지

내용:

- `dotnet build FamilyClaimRef.sln`만 실행한다.
- 테스트 케이스 검증은 보류한다.

장점:

- 범위가 가장 작다.
- package와 test project가 필요 없다.

단점:

- 컴파일 가능 여부만 확인한다.
- 정책 함수의 입력/출력과 예외 동작은 검증하지 못한다.

판정 후보:

- 가능하지만 품질 위험 있음

## 7. Recommended Direction

권장 방향은 다음과 같다.

1. 먼저 Option A로 테스트 케이스 문서를 확정한다.
2. 사용자 승인 후 Option B로 별도 test project를 생성한다.
3. test framework는 app project와 분리한다.
4. production project에는 test package를 추가하지 않는다.
5. `FileNamePolicyService`는 계속 파일/DB/OCR/metadata 접근이 없는 순수 함수로 유지한다.
6. Option C는 사용하지 않는다.
7. Option D는 build 확인용으로만 유지하고 정책 검증의 대체 수단으로 보지 않는다.

보수적 추천:

- 다음 단계는 `docs/53_FILENAME_POLICY_TEST_CASES.md` 생성
- 그 다음 단계에서 test project 생성 여부 결정

## 8. Test Project Cautions

test project를 만들 경우 아래 조건을 먼저 승인해야 한다.

- 별도 test project로 분리
- production `.csproj`에는 test package 추가 금지
- solution에 test project 추가 승인 필요
- test project `.csproj` 생성 승인 필요
- NuGet package 추가 승인 필요
- test framework 선택 필요
- 테스트 데이터는 구조 예시만 사용
- 실제 가족 실명, 실제 보험사명, 실제 병원명, 실제 진단명 금지
- 실제 진단코드 기반 개인 사례 금지
- DB/OCR/file 접근 테스트 금지
- `attachments/`, `data/local` 내부 파일 생성 금지
- 테스트 대상은 `FileNamePolicyService.CreatePhysicalFileName(...)`로 제한

test framework 후보:

| 후보 | 장점 | 위험 | 판정 |
|---|---|---|---|
| xUnit | 많이 쓰이며 간결함 | package 추가 필요 | Candidate |
| MSTest | Microsoft 기본 생태계와 익숙함 | package 추가 필요 | Candidate |
| NUnit | 성숙한 assertion 생태계 | package 추가 필요 | Candidate |

현재 문서에서는 test framework를 확정하지 않는다.

## 9. User Decision Questions

후속 작업 전 사용자 결정 질문은 다음과 같다.

```text
Q1 테스트 방식:
- A. 문서 기반 수동 테스트 케이스만 작성
- B. 별도 test project 생성 승인
- C. 현재는 build만 유지
- D. 보류

Q2 test project 생성 시 framework:
- A. xUnit
- B. MSTest
- C. NUnit
- D. 아직 결정하지 않음

Q3 NuGet package 추가:
- A. test project에만 허용
- B. production project에는 금지
- C. 모든 package 추가 보류

Q4 테스트 데이터 기준:
- A. 구조 예시만 사용
- B. 실제 개인정보/기관명/진단명 금지
- C. DB/OCR/file 접근 금지
```

## 10. Recommended Default Answers

권장 기본 답변은 다음과 같다.

```text
Q1: A 먼저
Q2: D
Q3: C
Q4: A + B + C
```

해석:

- 지금은 테스트 케이스 문서화까지만 진행한다.
- test project와 package 추가는 다음 승인 단계로 미룬다.
- production project에는 test package를 추가하지 않는다.
- 테스트 데이터는 구조 예시만 사용한다.
- 실제 개인정보와 실제 기관명, 실제 진단명은 사용하지 않는다.
- DB/OCR/file 접근 테스트는 하지 않는다.

## 11. Risks

남은 위험은 다음과 같다.

- 문서 기반 테스트 케이스만으로는 자동 회귀 검증이 되지 않는다.
- build-only 검증은 `FileNamePolicyService`의 정책 정확성을 보장하지 못한다.
- test project를 만들려면 `.sln`, test `.csproj`, NuGet package 변경 승인이 필요하다.
- test framework 선택이 늦어지면 자동 테스트 도입이 지연된다.
- `documentType` 최종 목록이 바뀌면 테스트 케이스도 함께 갱신해야 한다.
- 999 초과 duplicate index 허용 여부는 아직 정책으로 닫히지 않았다.
- 날짜 기준은 함수 외부에서 결정되므로 테스트는 포맷만 검증할 수 있다.

## 12. Recommendation

다음 순서를 권장한다.

1. 이 문서를 기준으로 테스트 방식 Q1~Q4에 대한 사용자 결정을 받는다.
2. 우선 `docs/53_FILENAME_POLICY_TEST_CASES.md`를 만들어 테스트 케이스를 더 상세히 확정한다.
3. 자동화가 필요하다고 결정되면 별도 test project 생성 지시를 새로 받는다.
4. test project 생성 시에는 production project에 test package를 추가하지 않는다.
5. `FileNamePolicyService` production code는 테스트 범위 결정 단계에서 수정하지 않는다.

## 13. Next Step

다음 작업 후보:

```text
docs/53_FILENAME_POLICY_TEST_CASES.md
```

또는 사용자가 자동화 테스트를 바로 승인할 경우:

```text
FamilyClaimRef.App.Tests 생성 범위 결정 문서
```

후속 작업 전까지 다음은 진행하지 않는다.

- test project 생성
- C# 테스트 파일 생성
- `.sln` 수정
- `.csproj` 수정
- NuGet package 추가
- production code 수정
- DB/OCR/metadata 구현
- `attachments/`, `data/local` 내부 파일 생성

## Result

`TEST_SCOPE_DECISION_READY`
