# Allowlist / Seed Consistency Test User Decision Record

## A. Goal

이 문서는 `docs/76_ALLOWLIST_SEED_CONSISTENCY_TEST_DESIGN.md`의 Needs Decision Q1~Q12에 대한 사용자 결정 기록이다.

목적은 allowlist/seed consistency test 구현 방향을 확정하고, 다음 구현 단계에서 포함할 테스트 범위와 제외할 범위를 명확히 하는 것이다.

이 문서는 구현 문서가 아니다. 테스트 파일 생성, 테스트 코드 구현, production code 수정은 수행하지 않는다.

## B. Checked Files / Paths

| 대상 | 확인 목적 | 비고 |
|---|---|---|
| `docs/76_ALLOWLIST_SEED_CONSISTENCY_TEST_DESIGN.md` | Q1~Q12 Needs Decision 확인 | 읽기 전용 |
| `docs/75_DOCUMENT_TYPE_SEED_CONSTANT_IMPLEMENTATION_REVIEW.md` | seed constant 구현 결과 확인 | 읽기 전용 |
| `docs/74_DOCUMENT_TYPE_SEED_CONSTANT_USER_DECISION_RECORD.md` | document type seed 사용자 결정 확인 | 읽기 전용 |
| `docs/59_FILENAME_POLICY_TEST_PROJECT_IMPLEMENTATION_REVIEW.md` | 기존 test project 기준 확인 | 읽기 전용 |
| `app/FamilyClaimRef.App/Models/Storage/DocumentTypeSeed.cs` | seed item model 확인 | 읽기 전용 |
| `app/FamilyClaimRef.App/Models/Storage/DocumentTypeSeeds.cs` | fixed seed 목록 확인 | 읽기 전용 |
| `app/FamilyClaimRef.App/Services/FileNamePolicyService.cs` | document type allowlist 확인 | 읽기 전용 |
| `tests/FamilyClaimRef.App.Tests/FileNamePolicyServiceTests.cs` | 기존 테스트 기준 확인 | 읽기 전용 |

## C. User Decision Summary

| ID | Question | Decision | Recorded Direction |
|---|---|---|---|
| Q1 | 다음 테스트 구현은 black-box acceptance test 방식으로 갈 것인가? | Accepted | 다음 consistency test 구현은 black-box acceptance test 방식으로 진행한다. `DocumentTypeSeeds` seed를 `FileNamePolicyService.CreatePhysicalFileName(...)`에 넣어 accepted/rejected 여부를 검증한다. |
| Q2 | production code 수정 없이 진행할 것인가? | Accepted | production code 수정 없이 진행한다. `FileNamePolicyService.cs`, `DocumentTypeSeed.cs`, `DocumentTypeSeeds.cs`는 수정하지 않는다. |
| Q3 | `FileNamePolicyService` allowlist accessor는 만들지 않을 것인가? | Accepted | allowlist accessor는 만들지 않는다. full equality test는 현재 단계에서 보류한다. |
| Q4 | `DocumentTypeSeeds`의 seed가 `FileNamePolicyService`에서 accepted 되는지 검증할 것인가? | Accepted | claim seed 7개는 claim scope에서 accepted 되어야 하고, policy seed 5개는 policy scope에서 accepted 되어야 한다. |
| Q5 | New Candidate `statement`, `prescription`, claim `capture`가 rejected 되는지 검증할 것인가? | Accepted | claim `statement`, claim `prescription`, claim `capture`는 현재 `ArgumentException`이어야 한다. policy `capture`는 policy scope에서 accepted 되어야 한다. |
| Q6 | seed structure test를 포함할 것인가? | Accepted | seed count, code/label/scope, 중복, sortOrder, disabledAt 기준을 검증한다. |
| Q7 | `(Scope, Code)` 중복 검증을 포함할 것인가? | Accepted | `(Scope, Code)` 조합 중복 검증을 포함한다. 같은 code라도 scope가 다르면 별도 seed로 본다. |
| Q8 | `DisabledAt`이 현재 모두 null인지 검증할 것인가? | Accepted | 현재 seed의 `DisabledAt`은 모두 null이어야 한다. |
| Q9 | scope별 sortOrder 검증을 포함할 것인가? | Accepted | 각 scope 안에서 `SortOrder` 중복이 없어야 하며 정렬 가능한 기준을 유지한다. |
| Q10 | 테스트 파일 후보는 `DocumentTypeSeedConsistencyTests.cs`로 할 것인가? | Accepted | 후속 구현 파일명은 `tests/FamilyClaimRef.App.Tests/DocumentTypeSeedConsistencyTests.cs`로 확정한다. |
| Q11 | 테스트 구현 후 `dotnet build`, `dotnet test`를 실행할 것인가? | Accepted | 테스트 구현 후 `dotnet build FamilyClaimRef.sln`, `dotnet test FamilyClaimRef.sln`을 실행한다. 기존 33개 테스트에 consistency test가 추가되어야 한다. |
| Q12 | JSON storage implementation은 계속 제외할 것인가? | Accepted | JSON storage implementation은 계속 제외한다. 실제 JSON file 생성도 제외한다. |

## D. Accepted Test Direction

다음 구현 단계의 테스트 방향은 아래와 같이 확정한다.

- black-box acceptance test 방식으로 진행한다.
- production code 수정은 없다.
- `FileNamePolicyService` allowlist accessor 추가는 없다.
- `DocumentTypeSeeds`의 current seed가 `FileNamePolicyService`에서 accepted 되는지 검증한다.
- New Candidate rejected 검증을 포함한다.
- seed structure test를 포함한다.
- `(Scope, Code)` 중복 검증을 포함한다.
- `DisabledAt` 전체 null 검증을 포함한다.
- scope별 sortOrder 검증을 포함한다.
- 테스트 파일은 `tests/FamilyClaimRef.App.Tests/DocumentTypeSeedConsistencyTests.cs`로 한다.
- 테스트 구현 후 build/test를 실행한다.
- JSON storage implementation은 제외한다.

## E. Test Case Scope for Next Implementation

### Seed structure tests

다음 구현에 포함할 seed structure test 범위:

- Claim seed count = 7
- Policy seed count = 5
- All seed count = 12
- 모든 seed `Code` non-empty
- 모든 seed `Label` non-empty
- 모든 seed `Scope`는 `claim` 또는 `policy`
- `(Scope, Code)` 조합 중복 없음
- scope별 `SortOrder` 중복 없음
- 모든 seed `DisabledAt == null`

### Acceptance tests

다음 구현에 포함할 acceptance test 범위:

- claim seed 7개는 claim scope에서 accepted
- policy seed 5개는 policy scope에서 accepted
- scope mismatch는 rejected
- 단, `etc`는 claim/policy 양쪽 허용 예외
- policy `capture`는 policy scope에서 accepted
- claim `capture`는 rejected

### New Candidate rejection tests

다음 구현에 포함할 New Candidate rejection test 범위:

- claim `statement` rejected
- claim `prescription` rejected
- claim `capture` rejected

## F. Still Not Implemented

아래 항목은 아직 구현되지 않았다.

- 테스트 코드 구현 없음
- test file 생성 없음
- production C# 수정 없음
- `FileNamePolicyService` 수정 없음
- seed constant 수정 없음
- allowlist accessor 추가 없음
- allowlist/seed consistency test 구현 없음
- CategoryItem 구현 없음
- CategoryItem JSON storage 구현 없음
- JSON storage implementation 없음
- 실제 JSON file 생성 없음
- SQLite DB/package 추가 없음
- repository/data access/migration 구현 없음
- WPF UI/XAML/navigation/ViewModel 구현 없음
- `attachments/`, `data/local` 내부 파일 생성 없음
- 실제 개인정보 샘플 없음

## G. Next Step

다음 작업 후보:

1. 별도 작업으로 `tests/FamilyClaimRef.App.Tests/DocumentTypeSeedConsistencyTests.cs` 구현
2. 구현 후 `dotnet build FamilyClaimRef.sln`, `dotnet test FamilyClaimRef.sln` 실행
3. 구현 후 `docs/78_ALLOWLIST_SEED_CONSISTENCY_TEST_IMPLEMENTATION_REVIEW.md` 생성
4. 그 다음 seed constant와 `FileNamePolicyService` 통합 여부 검토
5. JSON storage implementation은 아직 보류

## H. Result

`ALLOWLIST_SEED_CONSISTENCY_TEST_USER_DECISION_RECORDED`
