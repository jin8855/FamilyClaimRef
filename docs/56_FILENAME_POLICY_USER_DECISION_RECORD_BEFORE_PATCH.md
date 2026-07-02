# 56_FILENAME_POLICY_USER_DECISION_RECORD_BEFORE_PATCH

## 1. Goal

이 문서는 `FileNamePolicyService` patch 전에 사용자가 승인한 파일명 정책 결정을 기록한다.

이번 작업은 구현 작업이 아니다. C# 파일 수정, XAML 수정, HTML/CSS/JavaScript 수정, test project 생성, `.sln` 수정, `.csproj` 수정, NuGet package 추가, DB/OCR/metadata 구현, 실제 파일 저장은 수행하지 않는다.

목표는 다음과 같다.

- `duplicateIndex` 최댓값 정책을 사용자 결정으로 기록한다.
- MVP 허용 확장자 정책을 사용자 결정으로 기록한다.
- document type code MVP 기준을 기록한다.
- 날짜 의미 결정과 id 생성 책임을 `FileNamePolicyService` 밖으로 분리한다.
- `FileNamePolicyService` patch 필요 범위와 금지 범위를 분리한다.
- 자동화 테스트 후보에 미치는 영향을 정리한다.

## 2. Checked Files / Paths

| 구분 | 경로 | 확인 내용 |
|---|---|---|
| Implementation Review | `docs/51_FILENAME_POLICY_SERVICE_IMPLEMENTATION_REVIEW.md` | 현재 구현 책임과 미구현 책임 |
| Test Cases | `docs/53_FILENAME_POLICY_TEST_CASES.md` | N/E/B 테스트 후보와 B3 정책 미결정 항목 |
| Manual Review | `docs/54_FILENAME_POLICY_MANUAL_REVIEW_RECORD.md` | `MANUAL_REVIEW_NEEDS_POLICY_DECISION` 판정 |
| Policy Decision Candidate | `docs/55_FILENAME_POLICY_POLICY_DECISION_BEFORE_AUTOMATION.md` | patch 전 정책 후보와 후속 영향 |
| Service Source | `app/FamilyClaimRef.App/Services/FileNamePolicyService.cs` | 현재 구현이 아직 사용자 결정 기준으로 patch되지 않은 상태 |

## 3. Scope

이 문서의 범위는 다음으로 제한한다.

- 사용자 결정 기록
- production code patch 필요 항목 분리
- 자동화 테스트 영향 정리
- still out of scope 항목 재확인
- still needs decision 항목 분리

이 문서의 범위 밖 항목은 다음과 같다.

- production code 수정
- test project 생성
- 테스트 코드 생성
- `.sln`, `.csproj`, Target Framework 수정
- NuGet package 추가
- 파일 접근
- DB 접근
- OCR 접근
- metadata 저장
- 실제 문서 파일 생성
- sample/mock data 생성

## 4. User Decisions Summary

| 항목 | 사용자 결정 | 후속 영향 |
|---|---|---|
| `duplicateIndex` 최댓값 | `1`부터 `999`까지만 허용 | `duplicateIndex >= 1000` 거부 patch 필요 |
| 허용 확장자 | `pdf`, `jpg`, `jpeg`, `png`만 MVP 허용 | extension allowlist patch 필요 |
| document type code | 현재 후보 목록을 MVP 상수 기준으로 유지 | 현재 구현 기준 유지 |
| 날짜 의미 | 호출자 책임 | service는 `yyyyMMdd` 포맷만 담당 |
| id 생성 | 호출자 책임 | service는 safe character validation만 담당 |
| Q6 항목 | 범위 밖 유지 | 별도 승인 전 구현 금지 |

## 5. Accepted Policy Decisions

### duplicateIndex

Accepted policy:

- `duplicateIndex`는 `1`부터 `999`까지만 허용한다.
- `duplicateIndex >= 1000`은 거부한다.
- `duplicateIndex=1000`은 더 이상 정상 suffix 후보가 아니라 오류 케이스이다.

예상 오류:

```text
ArgumentOutOfRangeException
```

### Allowed Extensions

MVP 허용 확장자는 다음으로 제한한다.

- `pdf`
- `jpg`
- `jpeg`
- `png`

다음 확장자는 MVP에서 거부하는 후보이다.

- `exe`
- `zip`
- `docx`

예상 오류:

```text
ArgumentException
```

### documentType

`documentType`은 현재 후보 목록을 MVP 상수 기준으로 유지한다.

보험 문서 후보:

- `policy`
- `terms`
- `contract`
- `capture`
- `etc`

청구 문서 후보:

- `receipt`
- `diagnosis`
- `medicine`
- `visit`
- `admission`
- `surgery`
- `etc`

주의:

- 현재 단계에서는 `CategoryItem`과 연결하지 않는다.
- 최종 분류 체계와 연결하는 시점에는 별도 결정이 필요하다.

### Date Responsibility

날짜 의미 결정은 호출자 책임이다.

- `FileNamePolicyService`는 날짜 의미를 결정하지 않는다.
- `FileNamePolicyService`는 전달받은 날짜를 `yyyyMMdd`로 포맷만 한다.

호출자 기준 후보:

- 청구 문서: 진료일 우선, 없으면 등록일 후보
- 보험 문서: 문서 기준일 또는 발행일 우선, 없으면 등록일 후보

### Id Responsibility

id 생성은 호출자 책임이다.

- `FileNamePolicyService`는 id를 생성하지 않는다.
- `FileNamePolicyService`는 전달받은 id가 파일명 안전 문자 규칙을 지키는지만 검증한다.
- 화면 임시 id, DB id, document id 중 어떤 값을 사용할지는 후속 설계에서 결정한다.

## 6. Code Patch Required

Patch 필요:

```text
있음
```

production code patch가 필요하다.

Patch 항목:

1. `duplicateIndex > 999`이면 `ArgumentOutOfRangeException`을 발생시킨다.
2. extension allowlist를 적용한다.

허용 extension:

- `pdf`
- `jpg`
- `jpeg`
- `png`

거부 후보:

- `exe`
- `zip`
- `docx`

Patch 금지 범위:

- file access
- DB access
- OCR access
- metadata storage
- test project creation
- NuGet package
- `.sln`
- `.csproj`
- `attachments/`
- `data/local/`

이번 문서에서는 patch를 수행하지 않는다.

## 7. Automation Test Impact

자동화 테스트 후보 영향은 다음과 같다.

| Case Range | 영향 | 비고 |
|---|---|---|
| N1-N7 | Auto Candidate 유지 | 정상 케이스 유지 |
| E1-E18 | Auto Candidate 유지 | 기존 오류 케이스 유지 |
| B1-B2 | Auto Candidate 유지 | 경계값 유지 |
| B3 | 오류 케이스로 전환 | `duplicateIndex=1000` |
| B4-B5 | Auto Candidate 유지 | 기존 경계값 유지 |

B3 전환:

| ID | Input | Expected Exception | Purpose |
|---|---|---|---|
| B3 | `duplicateIndex=1000` | `ArgumentOutOfRangeException` | suffix 최댓값 999 초과 거부 |

추가 오류 케이스 후보:

| ID | Input | Expected Exception | Purpose |
|---|---|---|---|
| E19 | `duplicateIndex=1000` | `ArgumentOutOfRangeException` | suffix 최댓값 999 초과 거부 |
| E20 | extension `exe` | `ArgumentException` | 실행 파일 확장자 거부 |
| E21 | extension `zip` | `ArgumentException` | 압축 파일 확장자 거부 |
| E22 | extension `docx` | `ArgumentException` | MVP 미허용 문서 확장자 거부 |

주의:

- 이 문서는 자동화 테스트 코드를 생성하지 않는다.
- test project 생성 여부는 별도 승인 후 결정한다.
- `dotnet test`는 이번 작업 범위가 아니다.

## 8. Still Out of Scope

다음 항목은 이번 patch 전 사용자 결정 기록에서도 범위 밖으로 유지한다.

- `displayTitle` generation
- raw `originalFileName`
- sensitive data auto detection
- real file existence check
- file save/copy
- DB/metadata query
- OCR status query
- duplicate file auto search
- file hash calculation

위 항목은 별도 승인 전까지 production code, test code, 문서 기반 테스트 케이스로 확정하지 않는다.

## 9. Still Needs Decision

아직 결정이 필요한 항목은 다음과 같다.

1. document type final list와 `CategoryItem` 연결 여부
2. 화면별 호출자가 날짜 기준을 결정하는 방식
3. id generation source: screen temp id, DB id, document id candidate
4. allowed extension list를 MVP 이후 확장할지 여부
5. 외부 출력, 공유, export 상황에서 `displayTitle` masking 기준

## 10. Risks

남은 위험은 다음과 같다.

- 현재 production code는 아직 이 사용자 결정 기준으로 patch되지 않았다.
- `duplicateIndex=1000`은 현재 구현에서 허용될 수 있으므로 후속 code patch 전까지 문서와 코드가 불일치한다.
- 현재 extension 검증이 allowlist가 아니라 안전 문자 검증이면 `exe`, `zip`, `docx`가 거부되지 않을 수 있다.
- document type 후보는 MVP 상수 기준이지만, 최종 분류/태그 모델과 연결되면 변경될 수 있다.
- 날짜 의미와 id 생성 책임은 호출자에게 있으므로 호출부 구현 시 추가 검증이 필요하다.
- 테스트 자동화는 아직 수행하지 않았으므로 이 문서는 implementation-ready가 아니라 patch-ready 결정 기록이다.

## 11. Recommendation

다음 순서를 권장한다.

1. `FileNamePolicyService`만 대상으로 작은 production code patch를 수행한다.
2. patch 범위는 `duplicateIndex > 999` 거부와 extension allowlist 검증으로 제한한다.
3. `.sln`, `.csproj`, test project, NuGet package는 수정하지 않는다.
4. patch 후 문서 기반 수동 검토를 먼저 갱신한다.
5. 이후 test project 생성과 자동화 테스트 전환 여부를 별도 결정한다.

## 12. Next Step

다음 작업 후보:

```text
FileNamePolicyService duplicateIndex / extension allowlist patch
```

허용 patch 범위:

- `app/FamilyClaimRef.App/Services/FileNamePolicyService.cs`
- `duplicateIndex > 999` validation
- extension allowlist validation

계속 금지할 범위:

- `.sln` 수정
- `.csproj` 수정
- test project 생성
- NuGet package 추가
- DB/OCR/metadata 구현
- 실제 파일 저장
- `attachments/`, `data/local` 내부 파일 생성

## Result

`FILENAME_POLICY_USER_DECISIONS_RECORDED_FOR_PATCH`
