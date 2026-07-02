# 54_FILENAME_POLICY_MANUAL_REVIEW_RECORD

## 1. Goal

이 문서는 `docs/53_FILENAME_POLICY_TEST_CASES.md`의 문서 기반 수동 테스트 케이스가 현재 `FileNamePolicyService` 구현 설명과 일치하는지 검토한 결과를 기록한다.

이번 작업은 테스트 구현 작업이 아니다. test project 생성, C# 테스트 파일 생성, `.sln` 수정, `.csproj` 수정, NuGet package 추가, production code 수정, DB/OCR/metadata 구현, 파일 저장 구현은 수행하지 않는다.

## 2. Checked Files / Paths

| 구분 | 경로 | 확인 내용 |
|---|---|---|
| Implementation Review | `docs/51_FILENAME_POLICY_SERVICE_IMPLEMENTATION_REVIEW.md` | 구현 책임, API, validation 경계, build 결과 |
| Test Scope Decision | `docs/52_FILENAME_POLICY_SERVICE_TEST_SCOPE_DECISION.md` | 문서 기반 수동 테스트 케이스 작성 결정 |
| Test Cases | `docs/53_FILENAME_POLICY_TEST_CASES.md` | N1-N7, E1-E22, B1-B5, 정책 미결정 항목 |
| Patch Review | `docs/57_FILENAME_POLICY_PATCH_REVIEW.md` | `duplicateIndex` 999 초과 거부와 extension allowlist patch 결과 |
| Service Source | `app/FamilyClaimRef.App/Services/FileNamePolicyService.cs` | 현재 구현과 문서 케이스의 정합성 확인 |

## 3. Scope

이번 수동 검토의 범위는 다음과 같다.

- 정상 케이스 N1-N7의 expected output과 현재 구현 설명 일치 여부 확인
- 오류 케이스 E1-E22의 expected exception과 현재 구현 설명 일치 여부 확인
- 경계 케이스 B1-B5의 expected output과 정책 메모 확인
- 정책 미결정 항목 분리
- 자동화 전환 후보 재확인
- patch 이후 production code와 테스트 케이스 문서의 정합성 확인

범위 밖 항목은 다음과 같다.

- 실제 테스트 코드 작성
- `dotnet test` 실행
- test project 생성
- package 추가
- production code 수정
- build 설정 변경
- 파일/DB/OCR/metadata 접근
- 실제 개인정보 샘플 사용

## 4. Summary Result

수동 검토 결과:

```text
MANUAL_REVIEW_RECORD_UPDATED_AFTER_POLICY_PATCH
```

판정 이유:

- N1-N7 정상 케이스는 현재 구현 설명과 일치한다.
- E1-E22 오류 케이스는 현재 구현 설명과 일치한다.
- B1-B2, B4-B5 경계 케이스는 현재 구현 설명과 일치한다.
- B3은 더 이상 `_1000` 정상 케이스가 아니며 Error Case E19로 이동되었다.
- `duplicateIndex=1000`의 기대 결과는 `ArgumentOutOfRangeException`이다.
- 허용 extension은 `pdf`, `jpg`, `jpeg`, `png`로 정리되었다.
- allowlist 밖 extension인 `exe`, `zip`, `docx`는 `ArgumentException` 기준으로 기록되었다.
- document type code 최종 목록, 호출자의 날짜 기준 선택 방식, id source 선택은 아직 정책 미결정이다.

## 5. Normal Case Review

| ID | Expected | Review Result | Notes |
|---|---|---|---|
| N1 | `claim-000001_20260626_receipt.pdf` | PASS | `claim` scope, `receipt`, `pdf` 조합은 현재 구현과 일치 |
| N2 | `claim-000001_20260626_receipt_001.pdf` | PASS | dot 포함 extension과 `duplicateIndex` 1 suffix가 현재 구현과 일치 |
| N3 | `policy-000003_20260626_terms.pdf` | PASS | `policy` scope, `terms`, `pdf` 조합은 현재 구현과 일치 |
| N4 | `policy-000003_20260626_terms_002.pdf` | PASS | dot 포함 extension과 `duplicateIndex` 2 suffix가 현재 구현과 일치 |
| N5 | `claim-ID_001_20260626_receipt.pdf` | PASS | scope 소문자 정규화와 extension 소문자화가 현재 구현과 일치 |
| N6 | `policy-POLICY-001_20260626_capture.png` | PASS | hyphen id, `capture`, `.PNG` 정규화가 현재 구현과 일치 |
| N7 | `claim-CLAIM_001_20260626_etc.jpg` | PASS | underscore id, `etc`, `jpg` 조합은 현재 구현과 일치 |

## 6. Error Case Review

| ID | Expected Exception | Review Result | Notes |
|---|---|---|---|
| E1 | `ArgumentException` | PASS | `documentScope` null은 `string.IsNullOrWhiteSpace` 경로에서 거부 |
| E2 | `ArgumentException` | PASS | empty 또는 whitespace scope는 거부 |
| E3 | `ArgumentException` | PASS | `claim`, `policy` 외 scope는 거부 |
| E4 | `ArgumentException` | PASS | `id` null은 필수값 검증에서 거부 |
| E5 | `ArgumentException` | PASS | empty 또는 whitespace id는 거부 |
| E6 | `ArgumentException` | PASS | id 공백 문자는 안전 문자 검증에서 거부 |
| E7 | `ArgumentException` | PASS | slash 또는 backslash는 안전 문자 검증에서 거부 |
| E8 | `ArgumentException` | PASS | colon 또는 wildcard는 안전 문자 검증에서 거부 |
| E9 | `ArgumentException` | PASS | `documentType` null은 필수값 검증에서 거부 |
| E10 | `ArgumentException` | PASS | empty 또는 whitespace document type은 거부 |
| E11 | `ArgumentException` | PASS | `claim` scope의 `terms`는 scope별 type 검증에서 거부 |
| E12 | `ArgumentException` | PASS | `policy` scope의 `receipt`는 scope별 type 검증에서 거부 |
| E13 | `ArgumentException` | PASS | `extension` null은 필수값 검증에서 거부 |
| E14 | `ArgumentException` | PASS | empty 또는 whitespace extension은 거부 |
| E15 | `ArgumentException` | PASS | slash 또는 backslash 포함 extension은 안전 문자 검증에서 거부 |
| E16 | `ArgumentException` | PASS | dot only extension은 dot 제거 후 빈 값으로 거부 |
| E17 | `ArgumentOutOfRangeException` | PASS | `duplicateIndex` 0은 0 이하 검증에서 거부 |
| E18 | `ArgumentOutOfRangeException` | PASS | 음수 `duplicateIndex`는 0 이하 검증에서 거부 |
| E19 | `ArgumentOutOfRangeException` | PASS | `duplicateIndex` 1000은 999 초과 검증에서 거부 |
| E20 | `ArgumentException` | PASS | extension `exe`는 allowlist 밖 확장자로 거부 |
| E21 | `ArgumentException` | PASS | extension `zip`은 allowlist 밖 확장자로 거부 |
| E22 | `ArgumentException` | PASS | extension `docx`는 allowlist 밖 확장자로 거부 |

## 7. Boundary Case Review

| ID | Expected | Review Result | Policy Note |
|---|---|---|---|
| B1 | suffix 없음 | PASS | null `duplicateIndex`는 suffix 없이 출력하는 현재 구현과 일치 |
| B2 | `_999` | PASS | MVP 기준 최대값 999는 `_999` 출력 |
| B3 | `ArgumentOutOfRangeException` | MOVED_TO_ERROR_CASE_E19 | `duplicateIndex` 1000은 Error Case E19로 이동 |
| B4 | output has one dot | PASS | dot 포함 extension 입력은 dot 하나만 포함해 출력 |
| B5 | output has one dot | PASS | dot 없는 extension 입력도 출력에는 dot 포함 |

특히 B3은 다음과 같이 기록한다.

- `duplicateIndex=1000`은 정상 출력 기대값 `_1000`이 아니다.
- `duplicateIndex=1000`은 오류 케이스다.
- 기대 결과는 `ArgumentOutOfRangeException`이다.
- B3은 `Moved to Error Case / E19`로 표시한다.

## 8. Policy Decision Items

아래 항목은 정책 미결정으로 기록한다.

| 항목 | 상태 | 비고 |
|---|---|---|
| 허용 파일 확장자 최종 목록 | MVP 기준 정리됨 / 이후 확장 Needs Decision | MVP는 `pdf`, `jpg`, `jpeg`, `png`만 허용 |
| document type code 최종 목록 | MVP 기준 정리됨 / `CategoryItem` 연결 Needs Decision | 현재 후보 목록을 MVP 상수 기준으로 유지 |
| 날짜 기준 | Boundary Decision / 호출부 설계 Needs Decision | 함수는 전달받은 `DateOnly`를 `yyyyMMdd`로 포맷만 함 |
| `duplicateIndex` 최댓값 | MVP 기준 정리됨 / 이후 확장 Needs Decision | MVP는 999까지만 허용 |
| id 생성 주체 | Boundary Decision / source 선택 Needs Decision | id 생성은 호출자 책임, 함수는 id 안전 문자만 검증 |
| `displayTitle` 자동 생성 | Out of Scope | 현재 API 책임 아님 |
| raw `originalFileName` 처리 | Out of Scope | MVP에서 raw 저장 보류 |
| 민감정보 자동 탐지 | Out of Scope | 현재 함수가 자동 탐지 책임을 갖지 않음 |

Extension allowlist 수동 검토 기준:

- 허용: `pdf`, `jpg`, `jpeg`, `png`
- 거부: allowlist 밖 extension
- 거부 예시: `exe`, `zip`, `docx`
- 기대 예외: `ArgumentException`

## 9. Automation Candidate Review

자동화 후보는 다음과 같이 재확인한다.

| Case Range | Automation Status | Review |
|---|---|---|
| N1-N7 | Auto Candidate | 현재 구현 설명과 일치. test project 승인 후 자동화 가능 |
| E1-E22 | Auto Candidate | 현재 구현 설명과 일치. 예외 타입 검증 자동화 가능 |
| B1-B2 | Auto Candidate | 현재 구현 설명과 일치 |
| B3 | Moved to Error Case / E19 | `duplicateIndex` 1000은 `ArgumentOutOfRangeException`으로 검증 |
| B4-B5 | Auto Candidate | 현재 구현 설명과 일치 |
| Undecided Policy Cases | Needs Policy Decision | 자동화 전 정책 결정 필요 |

자동화 전환 조건:

- test project 생성 승인
- test framework 선택
- NuGet package 추가 범위 승인
- document type 최종 목록 또는 현재 후보 상수 기준 테스트 승인
- MVP 이후 확장자 allowlist 확장 여부 결정

## 10. Risks

남은 위험은 다음과 같다.

- 실제 테스트 코드를 실행하지 않았으므로 이 문서는 문서 기반 수동 판단 기록이다.
- 자동 회귀 검증은 아직 없다.
- document type code 최종 목록이 바뀌면 N/E 케이스 일부가 바뀔 수 있다.
- MVP 이후 허용 확장자 목록이 확장되면 extension 관련 케이스가 추가될 수 있다.
- MVP 이후 `duplicateIndex` 1000 이상 허용 정책이 생기면 B3/E19 기대값이 변경될 수 있다.
- 날짜 기준은 함수 외부 책임이므로 포맷 검증만 가능하다.

## 11. Recommendation

다음 권장안을 기록한다.

- 자동화 test project 생성 전 E1-E22를 기준으로 예외 타입 검증 범위를 확정한다.
- document type 최종 목록은 아직 후보이므로 자동화는 현재 후보 상수 기준으로만 테스트한다.
- extension 테스트는 MVP allowlist `pdf`, `jpg`, `jpeg`, `png`와 거부 예시 `exe`, `zip`, `docx` 기준으로 작성한다.
- 날짜 기준은 함수 외부 책임으로 두고, 함수는 `yyyyMMdd` 포맷만 검증한다.
- production code 수정은 현재 단계에서 하지 않는다.

## 12. Next Step

다음 작업 후보:

```text
FileNamePolicyService test project scope decision
```

또는 자동화 테스트 전환을 승인할 경우:

```text
docs/55_FILENAME_POLICY_TEST_PROJECT_SCOPE_DECISION.md
```

후속 작업 전까지 다음은 진행하지 않는다.

- `dotnet test` 실행
- test project 생성
- C# 테스트 파일 생성
- `.sln` 수정
- `.csproj` 수정
- NuGet package 추가
- production code 수정
- 파일/DB/OCR/metadata 구현
- `attachments/`, `data/local` 내부 파일 생성

## Result

`MANUAL_REVIEW_RECORD_UPDATED_AFTER_POLICY_PATCH`
