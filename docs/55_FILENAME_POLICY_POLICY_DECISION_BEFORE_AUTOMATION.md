# 55_FILENAME_POLICY_POLICY_DECISION_BEFORE_AUTOMATION

## 1. Goal

이 문서는 `FileNamePolicyService` 자동화 테스트 전 정책적으로 닫아야 할 항목을 정리한다.

이번 작업은 구현 작업이 아니다. C# 파일 수정, 테스트 코드 생성, test project 생성, `.sln` 수정, `.csproj` 수정, NuGet package 추가, DB/OCR/metadata 구현, 파일 저장 구현은 수행하지 않는다.

목표는 다음과 같다.

- `duplicateIndex` 최댓값 정책을 결정 후보로 정리한다.
- MVP 허용 확장자 후보를 정리한다.
- document type code MVP 후보를 정리한다.
- 날짜 기준과 id 생성 책임을 `FileNamePolicyService` 밖으로 분리한다.
- `displayTitle`, raw `originalFileName`, 민감정보 자동 탐지 등을 범위 밖으로 재확인한다.
- 정책 결정에 따른 후속 code patch 필요 여부를 판단한다.
- 자동화 테스트 전환 영향을 정리한다.

## 2. Checked Files / Paths

| 구분 | 경로 | 확인 내용 |
|---|---|---|
| Implementation Review | `docs/51_FILENAME_POLICY_SERVICE_IMPLEMENTATION_REVIEW.md` | 현재 구현 책임과 미구현 책임 |
| Test Scope Decision | `docs/52_FILENAME_POLICY_SERVICE_TEST_SCOPE_DECISION.md` | 자동화 테스트 전 사용자 결정 필요 항목 |
| Test Cases | `docs/53_FILENAME_POLICY_TEST_CASES.md` | B3, extension, document type, 날짜 기준 미결정 항목 |
| Manual Review | `docs/54_FILENAME_POLICY_MANUAL_REVIEW_RECORD.md` | `MANUAL_REVIEW_NEEDS_POLICY_DECISION` 판정 |
| Service Source | `app/FamilyClaimRef.App/Services/FileNamePolicyService.cs` | 현재 구현이 999 초과 suffix와 안전 문자 extension만 허용하는 상태 확인 |

## 3. Scope

이 문서의 범위는 다음으로 제한한다.

- 자동화 테스트 전 정책 결정 후보 정리
- 후속 patch 필요 여부 기록
- 자동화 테스트 케이스 영향 기록
- 범위 밖 항목 재확인

이 문서의 범위 밖 항목은 다음과 같다.

- production code 수정
- test project 생성
- 테스트 코드 생성
- package 추가
- `.sln`, `.csproj`, Target Framework 수정
- 파일/DB/OCR/metadata 접근
- sample/mock 파일 생성
- 실제 개인정보 샘플 사용

## 4. duplicateIndex Max Policy

현재 상태:

- 현재 구현은 `duplicateIndex=1000`을 `_1000`으로 허용한다.
- `docs/54_FILENAME_POLICY_MANUAL_REVIEW_RECORD.md`에서 B3은 `Needs Policy Decision`으로 기록되었다.

정책 선택지:

| 선택지 | 내용 | 영향 |
|---|---|---|
| A | 999까지만 허용 | 현재 구현 수정 필요 |
| B | 1000 이상도 허용 | 현재 구현 유지 가능 |
| C | 보류 | 자동화 테스트 전환 지연 |

권장 결정:

```text
A. 999까지만 허용
```

이유:

- `_001`부터 `_999`까지가 사람이 읽기 쉽고 정책 경계가 명확하다.
- MVP 범위에서 1000개 이상의 동일 기준 중복 파일은 비정상 상황으로 보는 것이 보수적이다.
- 1000개 이상이 실제로 필요하면 중복 파일 산정 정책을 다시 검토해야 한다.

판정:

```text
Accepted Policy Candidate
```

후속 영향:

- 현재 구현과 다르므로 후속 code patch가 필요하다.
- `duplicateIndex > 999` 입력은 `ArgumentOutOfRangeException`으로 전환하는 것이 후보이다.

## 5. Extension Allowlist Policy

현재 상태:

- 현재 구현은 extension의 안전 문자만 검증한다.
- 허용 파일 확장자 최종 목록은 아직 `Needs Decision`이다.

정책 선택지:

| 선택지 | 내용 |
|---|---|
| A | `pdf`, `jpg`, `jpeg`, `png`만 MVP 허용 |
| B | 이미지 + PDF + 문서 파일 확장자 일부 허용 |
| C | 확장자 제한 없이 안전 문자만 검증 |
| D | 보류 |

권장 결정:

```text
A. pdf, jpg, jpeg, png만 MVP 허용
```

이유:

- 보험/병원 문서 MVP 입력은 PDF와 이미지 중심이 현실적이다.
- 문서 파일, 압축 파일, 실행 파일 등까지 열면 보안, 미리보기, 처리 정책이 넓어진다.
- 파일 저장 구현 전까지 허용 범위를 좁게 잡는 것이 안전하다.

판정:

```text
Accepted Policy Candidate
```

후속 영향:

- 현재 구현은 안전 문자만 검증하므로 후속 code patch가 필요할 수 있다.
- `exe`, `zip`, `docx` 등은 오류 케이스로 전환하는 것이 후보이다.

## 6. Document Type Code Policy

현재 후보:

보험 문서:

- `policy`
- `terms`
- `contract`
- `capture`
- `etc`

청구 문서:

- `receipt`
- `diagnosis`
- `medicine`
- `visit`
- `admission`
- `surgery`
- `etc`

정책 선택지:

| 선택지 | 내용 |
|---|---|
| A | 현재 후보 목록을 MVP 상수 기준으로 유지 |
| B | `etc`만 남기고 단순화 |
| C | `CategoryItem` 설계 전까지 보류 |
| D | 목록 확장 |

권장 결정:

```text
A. 현재 후보 목록을 MVP 상수 기준으로 유지
```

주의:

- DB의 `CategoryItem`과 연결하지 않는다.
- 자동화 테스트는 현재 후보 목록 기준으로만 작성한다.
- 추후 `CategoryItem` 설계 이후 갱신 가능하다.

판정:

```text
Accepted for MVP
```

후속 영향:

- 현재 구현과 일치하므로 이 항목만으로는 code patch가 필요하지 않다.

## 7. Date Responsibility Policy

현재 상태:

- `FileNamePolicyService`는 전달받은 `DateOnly`를 `yyyyMMdd`로 포맷만 한다.
- 날짜가 진료일인지, 등록일인지, 문서 발행일인지는 함수 밖 책임이다.

정책 결정:

청구 문서:

- 진료일 우선
- 진료일이 없으면 등록일 후보

보험 문서:

- 문서 기준일 또는 발행일 우선
- 없으면 등록일 후보

책임 분리:

- `FileNamePolicyService`는 날짜 의미를 판단하지 않는다.
- 호출자가 이미 결정한 날짜를 넘긴다.
- 자동화 테스트는 `yyyyMMdd` 포맷만 검증한다.

판정:

```text
Accepted Boundary Decision
```

후속 영향:

- 현재 구현과 일치하므로 code patch가 필요하지 않다.

## 8. Id Responsibility Policy

현재 상태:

- `FileNamePolicyService`는 id 안전 문자만 검증한다.
- id 생성 주체는 미정이었다.

정책 결정:

- id 생성은 호출자 책임으로 둔다.
- `FileNamePolicyService`는 id를 생성하지 않는다.
- DB id, 화면 임시 id, 문서 id 중 무엇을 사용할지는 DB/metadata 설계에서 결정한다.
- 이 함수는 전달받은 id가 파일명 안전 문자 규칙을 지키는지만 검증한다.

판정:

```text
Accepted Boundary Decision
```

후속 영향:

- 현재 구현과 일치하므로 code patch가 필요하지 않다.

## 9. Out-of-Scope Items

다음 항목은 `FileNamePolicyService` 자동화 테스트 범위 밖으로 유지한다.

- `displayTitle` 자동 생성
- raw `originalFileName` 처리
- 민감정보 자동 탐지
- 실제 파일 존재 확인
- 파일 저장 또는 복사
- DB/metadata 조회
- OCR 상태 조회
- 중복 파일 자동 탐색
- file hash 계산

판정:

```text
Out of Scope
```

주의:

- 위 항목은 테스트 케이스로 확정하지 않는다.
- 별도 기능 승인 전까지 production code에도 포함하지 않는다.

## 10. Code Patch Impact

정책 결정이 권장안대로 확정되면 후속 patch가 필요하다.

Patch 필요:

```text
있음
```

후속 patch 후보:

1. `duplicateIndex`가 999를 초과하면 거부한다.
2. `extension`은 `pdf`, `jpg`, `jpeg`, `png`만 허용한다.

후속 patch 금지 범위:

- 파일 접근 금지
- DB 접근 금지
- OCR 접근 금지
- metadata 저장 금지
- test project 생성 금지
- package 추가 금지
- `.sln`, `.csproj` 수정 금지
- `attachments/`, `data/local` 내부 파일 생성 금지

현재 문서에서는 patch를 수행하지 않는다.

## 11. Automation Test Impact

정책 확정 후 자동화 후보는 다음과 같이 갱신한다.

| Case Range | 영향 |
|---|---|
| N1-N7 | Auto Candidate 유지 |
| E1-E18 | Auto Candidate 유지 |
| B1-B2 | Auto Candidate 유지 |
| B3 | 정책 A 채택 시 오류 케이스로 전환 |
| B4-B5 | Auto Candidate 유지 |
| Extension allowlist | 추가 오류 케이스 필요 |

B3 전환 후보:

| ID | Invalid Input | Expected Exception | Purpose |
|---|---|---|---|
| E19 | `duplicateIndex` 1000 | `ArgumentOutOfRangeException` | suffix 최댓값 999 초과 거부 |

확장자 제한 정책 채택 시 추가 오류 케이스 후보:

| ID | Invalid Input | Expected Exception | Purpose |
|---|---|---|---|
| E20 | extension `exe` | `ArgumentException` | 실행 파일 확장자 거부 |
| E21 | extension `zip` | `ArgumentException` | 압축 파일 확장자 거부 |
| E22 | extension `docx` | `ArgumentException` | MVP 미허용 문서 확장자 거부 |

정책 확정 후 정상 케이스 영향:

- `pdf`, `jpg`, `.PNG`는 허용 유지
- `jpeg` 허용 정상 케이스 추가 가능

## 12. Risks

남은 위험은 다음과 같다.

- 이 문서는 정책 결정 후보 문서이며 실제 code patch를 수행하지 않았다.
- `Accepted Policy Candidate` 항목은 사용자 최종 승인 전까지 구현 확정이 아니다.
- 확장자 허용 목록을 좁히면 기존 사용자가 보유한 문서 파일 확장자 일부가 MVP에서 제외될 수 있다.
- document type 목록은 현재 상수 후보를 유지하지만, `CategoryItem` 설계 이후 갱신될 수 있다.
- 날짜 기준은 호출자 책임으로 분리되어 있어 호출부 구현 시 별도 검증이 필요하다.
- id 생성 주체는 DB/metadata 설계까지 완전히 닫히지 않았다.

## 13. Recommendation

다음 순서를 권장한다.

1. 사용자에게 `duplicateIndex` 999 초과 거부와 확장자 allowlist를 승인받는다.
2. 승인되면 `FileNamePolicyService`만 대상으로 작은 code patch를 수행한다.
3. patch 범위는 `duplicateIndex > 999` 거부와 extension allowlist 검증으로 제한한다.
4. patch 후 문서 기반 테스트 케이스를 갱신한다.
5. 그 다음 test project 생성 여부를 별도 결정한다.

## 14. Next Step

다음 작업 후보:

```text
docs/56_FILENAME_POLICY_USER_DECISION_RECORD_BEFORE_PATCH.md
```

또는 사용자가 바로 patch를 승인할 경우:

```text
FileNamePolicyService duplicateIndex / extension policy patch
```

후속 작업 전까지 다음은 진행하지 않는다.

- production code 수정
- test project 생성
- 테스트 코드 생성
- `.sln` 수정
- `.csproj` 수정
- NuGet package 추가
- `dotnet test` 실행
- 파일/DB/OCR/metadata 구현
- `attachments/`, `data/local` 내부 파일 생성

## Result

`FILENAME_POLICY_DECISIONS_READY_FOR_PATCH`
