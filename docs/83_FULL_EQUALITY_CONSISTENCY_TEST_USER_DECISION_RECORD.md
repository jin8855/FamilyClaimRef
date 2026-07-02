# Full Equality Consistency Test User Decision Record

## A. Goal

이 문서는 `docs/82_FULL_EQUALITY_CONSISTENCY_TEST_DESIGN.md`의 Needs Decision Q1~Q12에 대한 사용자 결정 기록이다.

목적은 `DocumentTypeSeeds` seed code set과 `FileNamePolicyService.GetAllowedDocumentTypes(...)` accessor result set 사이의 full equality consistency test 구현 방향을 확정하는 것이다.

이 문서는 구현 문서가 아니다. 테스트 코드 구현, test file 생성, production code 수정, `FileNamePolicyService` 수정, seed constant 수정은 수행하지 않는다.

## B. Checked Files / Paths

| 대상 | 확인 목적 | 비고 |
|---|---|---|
| `docs/82_FULL_EQUALITY_CONSISTENCY_TEST_DESIGN.md` | Needs Decision Q1~Q12 확인 | 읽기 전용 |
| `docs/81_FILENAME_POLICY_ALLOWLIST_ACCESSOR_IMPLEMENTATION_REVIEW.md` | accessor 구현 결과 확인 | 읽기 전용 |
| `docs/80_DOCUMENT_TYPE_SEED_POLICY_SERVICE_INTEGRATION_USER_DECISION_RECORD.md` | allowlist accessor 및 full equality 방향 사용자 결정 확인 | 읽기 전용 |
| `docs/78_ALLOWLIST_SEED_CONSISTENCY_TEST_IMPLEMENTATION_REVIEW.md` | 기존 black-box consistency test 구현 결과 확인 | 읽기 전용 |
| `docs/77_ALLOWLIST_SEED_CONSISTENCY_TEST_USER_DECISION_RECORD.md` | 기존 consistency test 사용자 결정 확인 | 읽기 전용 |
| `docs/76_ALLOWLIST_SEED_CONSISTENCY_TEST_DESIGN.md` | 기존 consistency test 설계 기준 확인 | 읽기 전용 |
| `docs/75_DOCUMENT_TYPE_SEED_CONSTANT_IMPLEMENTATION_REVIEW.md` | seed constant 구현 결과 확인 | 읽기 전용 |
| `app/FamilyClaimRef.App/Services/FileNamePolicyService.cs` | accessor 및 현재 allowlist 확인 | 읽기 전용 |
| `app/FamilyClaimRef.App/Models/Storage/DocumentTypeSeed.cs` | seed item model 확인 | 읽기 전용 |
| `app/FamilyClaimRef.App/Models/Storage/DocumentTypeSeeds.cs` | fixed seed 목록 확인 | 읽기 전용 |
| `tests/FamilyClaimRef.App.Tests/DocumentTypeSeedConsistencyTests.cs` | 기존 consistency test 파일 확인 | 읽기 전용 |
| `tests/FamilyClaimRef.App.Tests/FileNamePolicyServiceTests.cs` | 기존 filename policy test 확인 | 읽기 전용 |
| `FamilyClaimRef.sln` | solution 기준 확인 | 읽기 전용 |
| `app/FamilyClaimRef.App/FamilyClaimRef.App.csproj` | app project 기준 확인 | 읽기 전용 |
| `tests/FamilyClaimRef.App.Tests/FamilyClaimRef.App.Tests.csproj` | test project 기준 확인 | 읽기 전용 |

## C. User Decision Summary

| ID | Question | Decision | Recorded Direction |
|---|---|---|---|
| Q1 | full equality consistency test를 구현할 것인가? | Accepted | full equality consistency test를 구현한다. 목적은 `DocumentTypeSeeds` seed code set과 `FileNamePolicyService.GetAllowedDocumentTypes(...)` accessor result set의 완전 일치 여부를 검증하는 것이다. 기존 black-box acceptance test는 유지한다. |
| Q2 | claim seed code set과 claim accessor allowlist set의 equality를 검증할 것인가? | Accepted | claim scope에서 `DocumentTypeSeeds.Claim` code set과 `FileNamePolicyService.GetAllowedDocumentTypes("claim")` result set이 정확히 동일해야 한다. |
| Q3 | policy seed code set과 policy accessor allowlist set의 equality를 검증할 것인가? | Accepted | policy scope에서 `DocumentTypeSeeds.Policy` code set과 `FileNamePolicyService.GetAllowedDocumentTypes("policy")` result set이 정확히 동일해야 한다. |
| Q4 | 비교는 순서 무관 set equality로 할 것인가? | Accepted | 비교는 순서 무관 set equality로 한다. seed `SortOrder`는 full equality test의 비교 대상이 아니며, code set 일치만 검증한다. |
| Q5 | New Candidate가 accessor 결과에 없는지 검증할 것인가? | Accepted | claim accessor result에는 `statement`, `prescription`, `capture`가 없어야 한다. policy accessor result에는 `capture`가 있어야 한다. |
| Q6 | invalid scope accessor 예외 테스트를 포함할 것인가? | Accepted | invalid scope를 `FileNamePolicyService.GetAllowedDocumentTypes(...)`에 전달하면 `ArgumentException`이 발생해야 한다. 기존 scope validation 흐름과 일관되어야 한다. |
| Q7 | 반환 컬렉션 mutation이 내부 allowlist에 영향을 주지 않는지 테스트할 것인가, 아니면 후보로만 둘 것인가? | Deferred | 반환 컬렉션 mutation 테스트는 이번 구현 범위에서 제외한다. accessor가 내부 `HashSet<string>`을 직접 반환하지 않고 array copy를 `IReadOnlyCollection<string>`로 반환하는 것은 이미 구현 리뷰에서 확인했다. mutation 테스트는 후속 hardening test 후보로만 유지한다. |
| Q8 | 기존 black-box consistency test는 유지할 것인가? | Accepted | 기존 black-box consistency test는 유지한다. full equality test는 기존 test를 대체하지 않고 보강한다. |
| Q9 | 기존 `DocumentTypeSeedConsistencyTests.cs`에 full equality test를 추가할 것인가? | Accepted | 새 테스트 파일을 만들지 않는다. 기존 `tests/FamilyClaimRef.App.Tests/DocumentTypeSeedConsistencyTests.cs`에 full equality test를 추가한다. |
| Q10 | production code 수정 없이 test code만 변경할 것인가? | Accepted | production code는 수정하지 않는다. `FileNamePolicyService.cs`, `DocumentTypeSeed.cs`, `DocumentTypeSeeds.cs`는 수정하지 않는다. 기존 테스트 파일 1개만 수정하는 방향으로 간다. |
| Q11 | 구현 후 `dotnet build FamilyClaimRef.sln`, `dotnet test FamilyClaimRef.sln`을 실행할 것인가? | Accepted | 구현 후 `dotnet build FamilyClaimRef.sln`과 `dotnet test FamilyClaimRef.sln`을 실행한다. 기존 62개 테스트에 full equality test가 추가되어야 한다. |
| Q12 | JSON storage implementation은 계속 제외할 것인가? | Accepted | JSON storage implementation은 계속 제외한다. 실제 JSON file 생성도 제외한다. 이번 후속 구현은 test code 수정만 허용한다. |

## D. Accepted Test Direction

후속 구현 방향은 아래와 같이 확정한다.

- full equality consistency test를 구현한다.
- claim seed/accessor set equality를 검증한다.
- policy seed/accessor set equality를 검증한다.
- 순서 무관 set equality를 사용한다.
- New Candidate accessor exclusion을 검증한다.
- invalid scope accessor exception을 검증한다.
- 반환 컬렉션 mutation test는 보류한다.
- 기존 black-box consistency test를 유지한다.
- 기존 `DocumentTypeSeedConsistencyTests.cs`에 테스트를 추가한다.
- production code 수정은 하지 않는다.
- test code만 변경한다.
- 구현 후 build/test를 실행한다.
- JSON storage implementation은 제외한다.

## E. Test Scope for Next Implementation

### Included

다음 구현에 포함할 테스트 범위:

- claim seed code set == claim accessor allowlist set
- policy seed code set == policy accessor allowlist set
- claim accessor does not contain `statement`
- claim accessor does not contain `prescription`
- claim accessor does not contain `capture`
- policy accessor contains `capture`
- invalid scope throws `ArgumentException`

### Excluded

다음 구현에서 제외할 범위:

- 반환 컬렉션 mutation test
- production code 수정
- 새 test file 생성
- `FileNamePolicyService.cs` 수정
- `DocumentTypeSeed.cs` 수정
- `DocumentTypeSeeds.cs` 수정
- `.sln` 수정
- `.csproj` 수정
- NuGet package 추가
- JSON storage implementation
- 실제 JSON file 생성
- SQLite DB/package 추가
- repository/data access/migration 구현
- WPF UI/XAML/navigation/ViewModel 구현

## F. Still Not Implemented

아래 항목은 아직 구현되지 않았다.

- full equality test 구현 없음
- test code 수정 없음
- production C# 수정 없음
- `FileNamePolicyService` 수정 없음
- seed constant 수정 없음
- 새 test file 생성 없음
- JSON storage implementation 없음
- 실제 JSON file 생성 없음
- SQLite DB/package 추가 없음
- repository/data access/migration 구현 없음
- WPF UI/XAML/navigation/ViewModel 구현 없음
- `attachments/`, `data/local` 내부 파일 생성 없음
- 실제 개인정보 샘플 없음

## G. Next Step

다음 작업 후보:

1. 별도 작업으로 기존 `tests/FamilyClaimRef.App.Tests/DocumentTypeSeedConsistencyTests.cs`에 full equality tests 추가
2. 구현 후 `dotnet build FamilyClaimRef.sln`, `dotnet test FamilyClaimRef.sln` 실행
3. 구현 후 `docs/84_FULL_EQUALITY_CONSISTENCY_TEST_IMPLEMENTATION_REVIEW.md` 생성
4. 그 다음 seed constant와 `FileNamePolicyService` 실제 통합 여부 재검토
5. JSON storage implementation은 아직 보류

## H. Result

`FULL_EQUALITY_CONSISTENCY_TEST_USER_DECISION_RECORDED`
