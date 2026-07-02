# 53_FILENAME_POLICY_TEST_CASES

## 1. Goal

이 문서는 `FileNamePolicyService.CreatePhysicalFileName(...)`의 문서 기반 수동 테스트 케이스를 정리한다.

이번 작업은 테스트 구현 작업이 아니다. test project 생성, C# 테스트 파일 생성, `.sln` 수정, `.csproj` 수정, NuGet package 추가, production code 수정, DB/OCR/metadata 구현, 파일 저장 구현은 수행하지 않는다.

이 문서는 다음 목적을 가진다.

- 정상 입력/출력 케이스를 수동 검토 가능한 표로 정리한다.
- 오류 입력과 기대 예외를 정리한다.
- 경계 케이스와 정책 메모를 분리한다.
- 자동화 테스트로 전환 가능한 후보와 정책 결정이 필요한 후보를 분리한다.
- 수동 검토 결과를 기록할 칸을 제공한다.

## 2. Checked Files / Paths

| 구분 | 경로 | 확인 내용 |
|---|---|---|
| Filename Policy Scope | `docs/48_FILENAME_POLICY_IMPLEMENTATION_SCOPE_DECISION.md` | 순수 파일명 정책 함수의 범위와 금지 경계 |
| Implementation Review | `docs/51_FILENAME_POLICY_SERVICE_IMPLEMENTATION_REVIEW.md` | 구현된 API, validation, build 결과 |
| Test Scope Decision | `docs/52_FILENAME_POLICY_SERVICE_TEST_SCOPE_DECISION.md` | 문서 기반 수동 테스트 케이스 작성 결정 |
| Service Source | `app/FamilyClaimRef.App/Services/FileNamePolicyService.cs` | 현재 구현된 public API와 validation |

확인한 사용자 결정:

```text
Q1 테스트 방식:
A. 문서 기반 수동 테스트 케이스만 작성

Q2 test framework:
D. 아직 결정하지 않음

Q3 NuGet package 추가:
C. 모든 package 추가 보류

Q4 테스트 데이터 기준:
A + B + C
- 구조 예시만 사용
- 실제 개인정보/기관명/진단명 금지
- DB/OCR/file 접근 금지
```

## 3. Scope

이 문서의 범위는 다음으로 제한한다.

- 정상 케이스 정리
- 오류 케이스 정리
- 경계 케이스 정리
- 정책 미결정 케이스 분리
- 자동화 후보 분류
- 수동 검토 기록 칸 제공

이 문서에서 하지 않는 일은 다음과 같다.

- 테스트 코드 작성
- test project 생성
- package 추가
- production code 수정
- `.sln`, `.csproj` 수정
- 파일/DB/OCR 접근
- sample/mock 파일 생성
- 실제 개인정보 샘플 사용

## 4. API Under Test

현재 API는 다음과 같다.

```csharp
public static string CreatePhysicalFileName(
    string documentScope,
    string id,
    DateOnly date,
    string documentType,
    string extension,
    int? duplicateIndex = null)
```

현재 함수의 책임:

- `documentScope`는 `claim` 또는 `policy`만 허용한다.
- `documentScope`는 trim 후 소문자로 정규화한다.
- `claim`은 `claim-`, `policy`는 `policy-` prefix를 사용한다.
- `date`는 `yyyyMMdd`로 출력한다.
- `documentType`은 scope별 허용 목록으로 검증한다.
- `extension`은 앞의 dot 유무를 허용하고 출력에는 dot 1개를 포함한다.
- `extension`은 소문자로 출력한다.
- `extension`은 `pdf`, `jpg`, `jpeg`, `png`만 허용한다.
- allowlist 밖 extension은 `ArgumentException`으로 거부한다.
- `duplicateIndex`가 있으면 `_001`, `_002`, `_999` 형식 suffix를 붙인다.
- `duplicateIndex`는 `1`부터 `999`까지만 허용한다.
- `id`는 ASCII 영문, 숫자, hyphen, underscore만 허용한다.
- 잘못된 입력은 `ArgumentException` 또는 `ArgumentOutOfRangeException`으로 거부한다.

범위 밖 책임:

- 실제 파일 존재 확인
- 파일 생성 또는 복사
- DB 조회
- OCR 실행 또는 OCR 상태 조회
- metadata 조회 또는 저장
- 중복 파일 자동 탐색
- file hash 계산
- `displayTitle` 생성
- raw `originalFileName` 처리
- 민감정보 자동 탐지

## 5. Normal Cases

정상 케이스는 구조 예시만 사용한다. 실제 가족 실명, 실제 보험사명, 실제 병원명, 실제 진단명, 실제 진단코드 기반 개인 사례는 사용하지 않는다.

| ID | documentScope | id | date | documentType | extension | duplicateIndex | Expected | Purpose | Manual Review |
|---|---|---|---|---|---|---|---|---|---|
| N1 | `claim` | `000001` | `2026-06-26` | `receipt` | `pdf` | null | `claim-000001_20260626_receipt.pdf` | 기본 청구 문서 파일명 | Not Reviewed |
| N2 | `claim` | `000001` | `2026-06-26` | `receipt` | `.pdf` | `1` | `claim-000001_20260626_receipt_001.pdf` | dot 포함 extension과 `_001` suffix | Not Reviewed |
| N3 | `policy` | `000003` | `2026-06-26` | `terms` | `pdf` | null | `policy-000003_20260626_terms.pdf` | 기본 보험 문서 파일명 | Not Reviewed |
| N4 | `policy` | `000003` | `2026-06-26` | `terms` | `.pdf` | `2` | `policy-000003_20260626_terms_002.pdf` | dot 포함 extension과 `_002` suffix | Not Reviewed |
| N5 | `Claim` | `ID_001` | `2026-06-26` | `receipt` | `PDF` | null | `claim-ID_001_20260626_receipt.pdf` | scope 대소문자 정규화와 extension 소문자화 | Not Reviewed |
| N6 | `policy` | `POLICY-001` | `2026-06-26` | `capture` | `.PNG` | null | `policy-POLICY-001_20260626_capture.png` | hyphen id와 대문자 image extension | Not Reviewed |
| N7 | `claim` | `CLAIM_001` | `2026-06-26` | `etc` | `jpg` | null | `claim-CLAIM_001_20260626_etc.jpg` | underscore id와 `etc` document type | Not Reviewed |

### Manual Review Notes

- Reviewer:
- Date:
- Result:
- Notes:

## 6. Error Cases

오류 케이스는 입력 validation과 예외 경계를 확인하기 위한 문서 기반 케이스다.

| ID | Invalid Input | Expected Exception | Purpose | Manual Review |
|---|---|---|---|---|
| E1 | `documentScope` null | `ArgumentException` | scope 필수값 검증 | Not Reviewed |
| E2 | `documentScope` empty or whitespace | `ArgumentException` | scope 공백 입력 거부 | Not Reviewed |
| E3 | `documentScope` other | `ArgumentException` | `claim`, `policy` 외 scope 거부 | Not Reviewed |
| E4 | `id` null | `ArgumentException` | id 필수값 검증 | Not Reviewed |
| E5 | `id` empty or whitespace | `ArgumentException` | id 공백 입력 거부 | Not Reviewed |
| E6 | `id` contains space | `ArgumentException` | id 공백 문자 거부 | Not Reviewed |
| E7 | `id` contains slash or backslash | `ArgumentException` | 경로 구분자 차단 | Not Reviewed |
| E8 | `id` contains colon or wildcard | `ArgumentException` | 파일명 위험 문자 차단 | Not Reviewed |
| E9 | `documentType` null | `ArgumentException` | document type 필수값 검증 | Not Reviewed |
| E10 | `documentType` empty or whitespace | `ArgumentException` | document type 공백 입력 거부 | Not Reviewed |
| E11 | `claim` scope with `terms` | `ArgumentException` | 청구 문서 scope에 보험 문서 type 입력 거부 | Not Reviewed |
| E12 | `policy` scope with `receipt` | `ArgumentException` | 보험 문서 scope에 청구 문서 type 입력 거부 | Not Reviewed |
| E13 | `extension` null | `ArgumentException` | extension 필수값 검증 | Not Reviewed |
| E14 | `extension` empty or whitespace | `ArgumentException` | extension 공백 입력 거부 | Not Reviewed |
| E15 | `extension` contains slash or backslash | `ArgumentException` | extension 경로 구분자 차단 | Not Reviewed |
| E16 | `extension` is dot only | `ArgumentException` | 빈 extension 거부 | Not Reviewed |
| E17 | `duplicateIndex` 0 | `ArgumentOutOfRangeException` | suffix index 하한 검증 | Not Reviewed |
| E18 | `duplicateIndex` negative | `ArgumentOutOfRangeException` | 음수 suffix index 거부 | Not Reviewed |
| E19 | `duplicateIndex` 1000 | `ArgumentOutOfRangeException` | suffix 최댓값 999 초과 거부 | Not Reviewed |
| E20 | extension `exe` | `ArgumentException` | 실행 파일 확장자 거부 | Not Reviewed |
| E21 | extension `zip` | `ArgumentException` | 압축 파일 확장자 거부 | Not Reviewed |
| E22 | extension `docx` | `ArgumentException` | MVP 미허용 문서 확장자 거부 | Not Reviewed |

### Manual Review Notes

- Reviewer:
- Date:
- Result:
- Notes:

## 7. Boundary Cases

경계 케이스는 현재 구현 결과와 아직 닫히지 않은 정책 메모를 함께 기록한다.

| ID | Input Condition | Expected | Purpose | Policy Note | Manual Review |
|---|---|---|---|---|---|
| B1 | `duplicateIndex` null | suffix 없음 | 기본 파일명에서 suffix 생략 확인 | 현재 구현 기준 확정 가능 | Not Reviewed |
| B2 | `duplicateIndex` 999 | `_999` | 세 자리 suffix 최댓값 확인 | MVP 기준 최대값으로 정리됨 | Not Reviewed |
| B3 | `duplicateIndex` 1000 | `ArgumentOutOfRangeException` | 세 자리 초과 입력 거부 확인 | 정책 확정 후 Error Case E19로 이동됨 | Not Reviewed |
| B4 | extension starts with dot | output has one dot | `.pdf` 입력을 `.pdf` 출력으로 정규화 | 확장자 앞 dot은 입력 편의로 허용 | Not Reviewed |
| B5 | extension without dot | output has one dot | `pdf` 입력을 `.pdf` 출력으로 정규화 | 출력은 항상 dot 포함 | Not Reviewed |

### Manual Review Notes

- Reviewer:
- Date:
- Result:
- Notes:

## 8. Undecided Policy Cases

아래 항목은 아직 테스트 케이스로 확정하지 않는다. 정책 결정 후 테스트 케이스로 전환한다.

| 항목 | 현재 상태 | 테스트 확정 보류 이유 | 후속 결정 |
|---|---|---|---|
| 허용 파일 확장자 최종 목록 | MVP 기준 정리됨 / 이후 확장 Needs Decision | MVP는 `pdf`, `jpg`, `jpeg`, `png`만 허용 | MVP 이후 확장 여부 결정 필요 |
| document type code 최종 목록 | MVP 기준 정리됨 / `CategoryItem` 연결 Needs Decision | 현재 후보 목록을 MVP 상수 기준으로 유지 | `CategoryItem` 연결 여부와 최종 코드 결정 필요 |
| 날짜 기준 | Boundary Decision / 호출부 설계 Needs Decision | 함수는 전달받은 `DateOnly`를 포맷만 함 | 호출자가 진료일, 등록일, 문서 발행일 중 기준을 선택하는 방식 설계 필요 |
| `duplicateIndex` 최댓값 | MVP 기준 정리됨 / 이후 확장 Needs Decision | MVP는 999까지만 허용 | MVP 이후 1000 이상 확장 여부 결정 필요 |
| id 생성 주체 | Boundary Decision / source 선택 Needs Decision | id 생성은 호출자 책임, 함수는 안전 문자만 검증 | DB id, 화면 임시 id, document id 중 선택 필요 |
| `displayTitle` 자동 생성 | Out of Scope | 현재 API 책임 아님 | 별도 표시명 정책 결정 필요 |
| raw `originalFileName` 처리 | Out of Scope | MVP에서 raw 저장 보류 | 원본 파일명 보존 정책 재승인 필요 |
| 민감정보 자동 탐지 | Out of Scope | 현재 함수가 완전 탐지한다고 가정하지 않음 | UI 입력 정책과 경고 기준 결정 필요 |

## 9. Automation Candidate Classification

각 케이스의 자동화 전환 후보는 다음과 같이 분류한다.

| Case Range | Classification | Reason | Notes |
|---|---|---|---|
| N1-N7 | `Auto Candidate` | 순수 함수 입력/출력 비교만 필요 | test project 승인 후 자동화 가능 |
| E1-E22 | `Auto Candidate` | 순수 함수 예외 타입 검증만 필요 | test framework 선택 후 자동화 가능 |
| B1-B2 | `Auto Candidate` | 현재 구현과 정책이 비교적 명확함 | suffix 형식 검증 가능 |
| B3 | `Moved to Error Case / E19` | 999 초과 거부 정책이 확정됨 | `duplicateIndex` 1000은 `ArgumentOutOfRangeException` |
| B4-B5 | `Auto Candidate` | extension dot 정규화 입출력 검증 가능 | allowlist 내 extension 기준으로 검증 |
| Undecided Policy Cases | `Needs Policy Decision` | 테스트 확정 전 정책 결정 필요 | 자동화 전환 보류 |

분류 기준:

- 순수 함수 입력/출력/예외 검증은 `Auto Candidate`
- 정책이 아직 닫히지 않은 항목은 `Needs Policy Decision`
- 화면, 파일, DB, OCR 접근과 관련된 항목은 현재 문서 범위 밖이며 자동화 대상으로 확정하지 않는다.

## 10. Manual Review Notes

전체 수동 검토 기록:

- Reviewer:
- Date:
- Result:
- Notes:

검토 결과 선택 후보:

```text
PASS
PASS_WITH_NOTES
NEEDS_POLICY_DECISION
REJECTED
```

검토 시 확인할 항목:

- 출력 파일명에 실제 개인정보나 기관명이 포함되지 않는가
- prefix가 scope와 일치하는가
- 날짜 포맷이 `yyyyMMdd`인가
- extension dot 처리와 소문자화가 기대와 일치하는가
- extension allowlist가 `pdf`, `jpg`, `jpeg`, `png` 기준으로 적용되는가
- scope별 document type 거부 기준이 기대와 일치하는가
- `duplicateIndex` 1000이 정상 출력이 아니라 오류 케이스로 분리되어 있는가

## 11. Risks

남은 위험은 다음과 같다.

- 이 문서는 수동 테스트 케이스 문서이므로 자동 회귀 검증을 제공하지 않는다.
- test project와 test framework가 아직 결정되지 않았다.
- NuGet package 추가가 보류되어 자동화 테스트 착수는 별도 승인 전까지 불가하다.
- `documentType` 최종 목록이 바뀌면 N1-N7, E11-E12 케이스도 갱신해야 한다.
- MVP 이후 허용 확장자 목록이 확장되면 extension 관련 케이스가 추가될 수 있다.
- MVP 이후 `duplicateIndex` 최댓값이 확장되면 B3/E19 기대값이 변경될 수 있다.
- 날짜 기준은 함수 외부 책임이므로 이 문서는 포맷 검증만 다룬다.

## 12. Recommendation

다음 순서를 권장한다.

1. 이 문서의 N/E/B 케이스를 수동으로 검토한다.
2. E19-E22를 patch 이후 수동 검토 기록에 반영한다.
3. 자동화가 필요하면 test project 생성 범위를 별도 승인한다.
4. test project를 만들 경우 production project에는 test package를 추가하지 않는다.
5. `FileNamePolicyService` production code는 테스트 케이스 문서 작성 단계에서 수정하지 않는다.

## 13. Next Step

다음 작업 후보:

```text
docs/54_FILENAME_POLICY_MANUAL_REVIEW_RECORD.md patch 이후 기준 갱신
```

또는 자동화 테스트로 전환할 경우:

```text
docs/54_FILENAME_POLICY_TEST_PROJECT_SCOPE_DECISION.md
```

후속 작업 전까지 다음은 진행하지 않는다.

- test project 생성
- C# 테스트 파일 생성
- `.sln` 수정
- `.csproj` 수정
- NuGet package 추가
- production code 수정
- 파일/DB/OCR/metadata 구현
- `attachments/`, `data/local` 내부 파일 생성

## Result

`TEST_CASES_UPDATED_AFTER_POLICY_PATCH`
