# Document Type Seed Constant User Decision Record

## A. Goal

이 문서는 `docs/73_DOCUMENT_TYPE_SEED_CONSTANT_IMPLEMENTATION_DECISION.md`의 Needs Decision Q1~Q12에 대한 사용자 결정 기록이다.

목적은 document type seed constant 구현 범위를 확정하고, 다음 구현 단계에서 무엇을 만들고 무엇을 제외할지 기준을 제공하는 것이다.

이 문서는 구현 문서가 아니다. C# constant 구현, `FileNamePolicyService` 수정, CategoryItem 구현, test 구현은 수행하지 않는다.

## B. Checked Files / Paths

| 대상 | 확인 목적 | 비고 |
|---|---|---|
| `docs/73_DOCUMENT_TYPE_SEED_CONSTANT_IMPLEMENTATION_DECISION.md` | Q1~Q12 Needs Decision과 seed constant 후보 확인 | 읽기 전용 |
| `docs/72_CSHARP_MODEL_INTERFACE_IMPLEMENTATION_REVIEW.md` | C# model/interface 구현 결과와 build/test PASS 상태 확인 | 읽기 전용 |
| `docs/71_CSHARP_MODEL_INTERFACE_USER_DECISION_RECORD.md` | C# model/interface 사용자 결정 확인 | 읽기 전용 |
| `docs/65_CATEGORY_ITEM_DOCUMENT_TYPE_USER_DECISION_RECORD.md` | CategoryItem/document type 사용자 결정 확인 | 읽기 전용 |
| `docs/64_CATEGORY_ITEM_DOCUMENT_TYPE_POLICY_DECISION.md` | document type policy 후보 확인 | 읽기 전용 |
| `docs/59_FILENAME_POLICY_TEST_PROJECT_IMPLEMENTATION_REVIEW.md` | 기존 test project와 PASS 기준 확인 | 읽기 전용 |
| `app/FamilyClaimRef.App/Services/FileNamePolicyService.cs` | 현재 allowlist 기준 확인 | 읽기 전용 |
| `tests/FamilyClaimRef.App.Tests/FileNamePolicyServiceTests.cs` | 현재 테스트 기준 확인 | 읽기 전용 |

## C. User Decision Summary

| ID | Question | Decision | Recorded Direction |
|---|---|---|---|
| Q1 | 다음 구현은 document type seed constant만 구현할 것인가? | Accepted | 다음 실제 구현 단계는 document type seed constant만 구현한다. CategoryItem JSON 저장 구현, `FileNamePolicyService` 통합, allowlist/seed consistency test는 포함하지 않는다. |
| Q2 | `FileNamePolicyService`는 다음 구현에서 수정하지 않을 것인가? | Accepted | 다음 seed constant 구현 단계에서는 `FileNamePolicyService`를 수정하지 않는다. 현재 allowlist 기준은 그대로 유지한다. seed constant와 allowlist 일치 검증은 후속 테스트 단계에서 다룬다. |
| Q3 | CategoryItem JSON 저장 구현은 계속 보류할 것인가? | Accepted | CategoryItem JSON 저장 구현은 계속 보류한다. MVP 1차에서는 fixed seed constant 기준만 먼저 둔다. |
| Q4 | 사용자 정의 document type 추가/수정/삭제는 계속 MVP 1차에서 제외할 것인가? | Accepted | 사용자 정의 document type 추가/수정/삭제는 MVP 1차에서 제외한다. 현재는 고정 seed 기준만 유지한다. |
| Q5 | seed constant에는 current allowlist만 포함할 것인가? | Accepted | seed constant에는 현재 `FileNamePolicyService` allowlist에 있는 current allowlist만 포함한다. 현재 allowlist에 없는 후보는 포함하지 않는다. |
| Q6 | `statement`, `prescription`, claim `capture`는 seed constant에서 제외하고 `New Candidate / Needs Implementation`으로 유지할 것인가? | Accepted | `statement`, `prescription`, claim scope `capture`는 seed constant에서 제외한다. 해당 항목은 별도 정책 결정, `FileNamePolicyService` patch, 테스트 갱신 전까지 `New Candidate / Needs Implementation`으로 유지한다. |
| Q7 | seed field는 `Code`, `Label`, `Scope`, `SortOrder`, `DisabledAt`으로 둘 것인가? | Accepted | seed field는 `Code`, `Label`, `Scope`, `SortOrder`, `DisabledAt`으로 둔다. `DisabledAt`은 fixed seed 비활성화 정책을 대비한 nullable field 후보로 둔다. |
| Q8 | scope는 기존 string 기준인 `claim`, `policy`를 유지할 것인가? | Accepted | scope는 기존 string 기준인 `claim`, `policy`를 유지한다. 과도한 enum 도입은 이번 단계에서 하지 않는다. |
| Q9 | `DocumentTypeSeed`, `DocumentTypeSeeds` 같은 static seed 구조로 갈 것인가? | Accepted | static seed 구조 후보로 간다. `DocumentTypeSeed`는 seed item record 역할, `DocumentTypeSeeds`는 claim/policy/current/all seed 목록을 제공하는 static class 후보로 둔다. |
| Q10 | seed constant 구현 후 `dotnet build`와 `dotnet test`를 실행할 것인가? | Accepted | 실제 seed constant 구현 후 `dotnet build FamilyClaimRef.sln`과 `dotnet test FamilyClaimRef.sln`을 실행한다. 기존 33개 테스트가 깨지지 않는지 확인한다. |
| Q11 | allowlist/seed consistency test는 seed constant 구현 후 별도 작업으로 진행할 것인가? | Accepted | allowlist/seed consistency test는 seed constant 구현 후 별도 작업으로 진행한다. 다음 seed constant 구현 단계에서는 테스트 코드를 추가하지 않는다. |
| Q12 | JSON storage implementation은 계속 별도 승인 전까지 제외할 것인가? | Accepted | JSON storage implementation은 계속 제외한다. 실제 JSON file 생성도 제외한다. `data/local/*.json` file은 별도 storage implementation 승인 전까지 만들지 않는다. |

## D. Accepted Implementation Direction

- 다음 구현은 document type seed constant만 구현한다.
- `FileNamePolicyService`는 다음 구현에서 수정하지 않는다.
- CategoryItem JSON 저장 구현은 보류한다.
- 사용자 정의 document type 추가/수정/삭제는 MVP 1차에서 제외한다.
- seed constant에는 current allowlist만 포함한다.
- `statement`, `prescription`, claim `capture`는 `New Candidate / Needs Implementation`으로 유지한다.
- seed field는 다음으로 둔다.
  - `Code`
  - `Label`
  - `Scope`
  - `SortOrder`
  - `DisabledAt`
- scope는 string 기준 `claim`, `policy`를 유지한다.
- static seed 구조 후보는 다음으로 둔다.
  - `DocumentTypeSeed`
  - `DocumentTypeSeeds`
- seed constant 구현 후 build/test를 실행한다.
- allowlist/seed consistency test는 seed constant 구현 후 별도 작업으로 분리한다.
- JSON storage implementation은 별도 승인 전까지 제외한다.

## E. Current Seed Baseline

### Claim seed

| Code | Label | Scope | SortOrder | Status |
|---|---|---|---:|---|
| `receipt` | 영수증 | `claim` | 10 | Current Allowlist |
| `diagnosis` | 진단서 | `claim` | 20 | Current Allowlist |
| `medicine` | 약제비 서류 | `claim` | 30 | Current Allowlist |
| `visit` | 통원 확인 서류 | `claim` | 40 | Current Allowlist |
| `admission` | 입퇴원 확인 서류 | `claim` | 50 | Current Allowlist |
| `surgery` | 수술 확인 서류 | `claim` | 60 | Current Allowlist |
| `etc` | 기타 | `claim` | 999 | Current Allowlist |

### Policy seed

| Code | Label | Scope | SortOrder | Status |
|---|---|---|---:|---|
| `policy` | 보험증권 | `policy` | 10 | Current Allowlist |
| `terms` | 약관 | `policy` | 20 | Current Allowlist |
| `contract` | 계약서 | `policy` | 30 | Current Allowlist |
| `capture` | 캡처 | `policy` | 40 | Current Allowlist |
| `etc` | 기타 | `policy` | 999 | Current Allowlist |

### New Candidate / Needs Implementation

| Code | Label | Scope | Reason |
|---|---|---|---|
| `statement` | 진료비 세부내역서 | `claim` | 현재 allowlist 없음 |
| `prescription` | 처방전 | `claim` | 현재 allowlist 없음 |
| `capture` | 캡처 | `claim` | 현재 allowlist 없음 |

주의:

- New Candidate는 다음 seed constant 구현 범위에 포함하지 않는다.

## F. Candidate Files for Next Implementation

후속 구현 후보 파일:

- `app/FamilyClaimRef.App/Models/Storage/DocumentTypeSeed.cs`
- `app/FamilyClaimRef.App/Models/Storage/DocumentTypeSeeds.cs`

주의:

- 이 문서에서는 위 파일을 생성하지 않는다.
- 실제 파일 생성은 별도 구현 승인 후 진행한다.

## G. Still Not Implemented

아래 항목은 아직 구현하지 않았다.

- C# constant 구현 없음
- C# model/interface 수정 없음
- `FileNamePolicyService` 수정 없음
- test code 수정 없음
- allowlist/seed consistency test 구현 없음
- CategoryItem 구현 없음
- CategoryItem JSON 저장 구현 없음
- JSON storage implementation 없음
- 실제 JSON file 생성 없음
- SQLite DB/package 추가 없음
- repository/data access/migration 구현 없음
- actual file copy/storage 구현 없음
- `attachments/`, `data/local` 내부 파일 생성 없음
- WPF UI/XAML/navigation/ViewModel 구현 없음
- 실제 개인정보 샘플 없음

## H. Next Step

다음 작업 후보:

1. 별도 승인 후 document type seed constant 구현
2. 구현 후 `docs/75_DOCUMENT_TYPE_SEED_CONSTANT_IMPLEMENTATION_REVIEW.md` 생성
3. 그 다음 allowlist/seed consistency test 설계 문서 생성
4. 그 다음 consistency test 구현
5. 그 다음 JSON file storage implementation 설계

## I. Result

`DOCUMENT_TYPE_SEED_CONSTANT_USER_DECISION_RECORDED`
