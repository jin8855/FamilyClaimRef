# Full Equality Consistency Test Design

## A. Goal

이 문서는 `FileNamePolicyService.GetAllowedDocumentTypes(string documentScope)` accessor와 `DocumentTypeSeeds` fixed seed 사이의 full equality consistency test 설계 문서다.

목적은 기존 black-box acceptance test 이후에도 남아 있는 allowlist/seed drift 위험을 줄이기 위한 테스트 방향을 정리하는 것이다.

이 문서는 다음 작업이 아니다.

- 테스트 구현 문서가 아니다.
- production code 수정 문서가 아니다.
- seed constant 수정 문서가 아니다.
- JSON storage 구현 문서가 아니다.
- WPF UI/ViewModel 구현 문서가 아니다.

## B. Current State

- `FileNamePolicyService.GetAllowedDocumentTypes(string documentScope)` 구현은 완료되어 있다.
- accessor는 scope별 current allowlist code set만 반환한다.
- accessor는 label, sortOrder, disabledAt을 노출하지 않는다.
- accessor는 New Candidate를 포함하지 않는다.
- accessor는 `DocumentTypeSeeds`를 직접 참조하지 않는다.
- `CreatePhysicalFileName(...)` 동작 변경은 없다.
- 기존 black-box consistency test 구현은 완료되어 있다.
- build/test는 PASS 상태로 기록되어 있다.
- 총 테스트 62개가 PASS 상태로 기록되어 있다.
- full equality test는 아직 없다.
- accessor와 seed 기준 일치 검증은 아직 자동화되지 않았다.
- JSON storage implementation은 아직 없다.

### Claim allowlist / seed

현재 claim 기준은 아래 7개다.

- `receipt`
- `diagnosis`
- `medicine`
- `visit`
- `admission`
- `surgery`
- `etc`

### Policy allowlist / seed

현재 policy 기준은 아래 5개다.

- `policy`
- `terms`
- `contract`
- `capture`
- `etc`

### New Candidate / 제외 유지

아래 claim 후보는 현재 기준에서 제외 상태로 유지한다.

- claim `statement`
- claim `prescription`
- claim `capture`

주의:

- policy `capture`는 policy 기준에 포함한다.
- claim `capture`는 claim 기준에 포함하지 않는다.
- `etc`는 claim/policy 양쪽 기준에 포함될 수 있다.

## C. Test Problem Statement

기존 black-box consistency test는 `DocumentTypeSeeds`의 seed가 `FileNamePolicyService.CreatePhysicalFileName(...)`에서 accepted 되는지를 검증한다.

하지만 이 방식만으로는 `FileNamePolicyService` 내부 allowlist에 seed에는 없는 type이 추가되는 경우를 완전하게 잡지 못한다.

allowlist accessor가 추가되었으므로 이제 scope별 accessor allowlist set과 seed code set을 직접 비교할 수 있다.

full equality test는 아래 drift 위험을 줄인다.

- seed에는 없는 code가 service allowlist에 추가되는 위험
- service allowlist에는 없는 code가 seed에 추가되는 위험
- claim/policy scope가 서로 섞이는 위험
- New Candidate가 승인 없이 accessor 결과에 포함되는 위험

이 테스트는 production behavior를 바꾸지 않고 test만 추가하는 방향으로 설계한다.

## D. Test Strategy

full equality test 전략은 아래와 같다.

- `DocumentTypeSeeds.Claim` code set과 `FileNamePolicyService.GetAllowedDocumentTypes("claim")` 결과는 정확히 동일해야 한다.
- `DocumentTypeSeeds.Policy` code set과 `FileNamePolicyService.GetAllowedDocumentTypes("policy")` 결과는 정확히 동일해야 한다.
- 비교는 순서에 의존하지 않는 set equality로 한다.
- accessor 결과는 label, sortOrder, disabledAt이 아니라 code set만 비교한다.
- New Candidate는 해당 scope의 accessor 결과에 없어야 한다.
- 기존 black-box acceptance test는 유지한다.
- full equality test는 black-box test를 대체하지 않고 보강한다.

## E. Proposed Test Cases

### Equality tests

후속 구현 후보:

- claim seed code set == claim accessor allowlist set
- policy seed code set == policy accessor allowlist set

검증 후보 예시:

```csharp
var seedCodes = DocumentTypeSeeds.Claim.Select(seed => seed.Code).ToHashSet(StringComparer.Ordinal);
var accessorCodes = FileNamePolicyService.GetAllowedDocumentTypes("claim").ToHashSet(StringComparer.Ordinal);

Assert.Equal(seedCodes, accessorCodes);
```

위 코드는 설계 예시이며, 이 문서에서는 테스트를 구현하지 않는다.

### Exclusion tests

후속 구현 후보:

- claim accessor result does not contain `statement`
- claim accessor result does not contain `prescription`
- claim accessor result does not contain `capture`
- policy accessor result contains `capture`

### Accessor behavior tests

후속 구현 후보:

- invalid scope passed to accessor throws `ArgumentException`
- returned collection mutation cannot affect internal allowlist

비고:

- 현재 accessor는 내부 `HashSet<string>`을 직접 반환하지 않고 array copy를 `IReadOnlyCollection<string>`로 반환한다.
- 반환 컬렉션 mutation 테스트는 구현 방식에 따라 과도한 테스트가 될 수 있다.
- mutation 검증은 구현 후보로 두되, 후속 사용자 결정에서 포함 여부를 정한다.

### Regression relationship

- 기존 `DocumentTypeSeedConsistencyTests.cs` black-box tests는 유지한다.
- full equality test는 black-box acceptance test의 대체가 아니라 drift 방지 보강이다.
- 후속 구현에서 기존 테스트 파일에 추가할지, 별도 테스트 파일을 만들지는 사용자 결정이 필요하다.

## F. Test File Candidate

### Candidate 1. 기존 `DocumentTypeSeedConsistencyTests.cs`에 추가

장점:

- allowlist/seed consistency 목적의 테스트가 한 파일에 모인다.
- 새 test file 생성이 없다.
- 현재 black-box consistency test와 직접 연결된다.

단점:

- 파일 크기가 커질 수 있다.
- black-box acceptance test와 full equality test의 목적이 섞일 수 있다.

### Candidate 2. 새 `DocumentTypeSeedFullEqualityTests.cs` 생성

장점:

- test 목적 분리가 명확하다.
- 구현 리뷰에서 추가 범위를 추적하기 쉽다.
- full equality test만 독립적으로 읽을 수 있다.

단점:

- test file 수가 증가한다.
- 작은 범위에 파일이 추가되어 구조가 다소 분산될 수 있다.

### Candidate Recommendation

Candidate 1을 우선 추천한다.

이유:

- 현재 목적은 같은 allowlist/seed drift 방지다.
- 기존 `DocumentTypeSeedConsistencyTests.cs`가 이미 seed structure, black-box acceptance, New Candidate rejection을 다룬다.
- full equality test는 같은 consistency 검증의 보강으로 볼 수 있다.

다만 기존 파일 수정 부담이 크거나 테스트 목적 분리를 더 중시하면 Candidate 2를 선택할 수 있다.

## G. Implementation Boundary Candidate

후속 구현 범위 후보:

- test code만 수정 또는 추가한다.
- production C# 수정은 하지 않는다.
- `FileNamePolicyService.cs` 수정은 하지 않는다.
- `DocumentTypeSeed.cs` 수정은 하지 않는다.
- `DocumentTypeSeeds.cs` 수정은 하지 않는다.
- `.sln` 수정은 하지 않는다.
- `.csproj` 수정은 하지 않는다.
- NuGet package 추가는 하지 않는다.
- JSON storage 구현은 하지 않는다.
- SQLite DB/package 추가는 하지 않는다.
- repository/data access/migration 구현은 하지 않는다.
- WPF UI/XAML/navigation/ViewModel 구현은 하지 않는다.

검증 명령 후보:

```powershell
dotnet build FamilyClaimRef.sln
dotnet test FamilyClaimRef.sln
```

기대:

- 총 테스트 수는 62개에서 증가한다.
- 정확한 증가 개수는 후속 구현 단계에서 보고한다.
- build/test는 PASS여야 한다.

## H. Needs Decision

후속 사용자 결정이 필요한 항목:

1. full equality consistency test를 구현할 것인가?
2. claim seed code set과 claim accessor allowlist set의 equality를 검증할 것인가?
3. policy seed code set과 policy accessor allowlist set의 equality를 검증할 것인가?
4. 비교는 순서 무관 set equality로 할 것인가?
5. New Candidate가 accessor 결과에 없는지 검증할 것인가?
6. invalid scope accessor 예외 테스트를 포함할 것인가?
7. 반환 컬렉션 mutation이 내부 allowlist에 영향을 주지 않는지 테스트할 것인가, 아니면 후보로만 둘 것인가?
8. 기존 black-box consistency test는 유지할 것인가?
9. 기존 `DocumentTypeSeedConsistencyTests.cs`에 full equality test를 추가할 것인가?
10. production code 수정 없이 test code만 변경할 것인가?
11. 구현 후 `dotnet build FamilyClaimRef.sln`, `dotnet test FamilyClaimRef.sln`을 실행할 것인가?
12. JSON storage implementation은 계속 제외할 것인가?

## I. Out of Scope

이번 문서에서 제외하는 범위는 다음과 같다.

- 테스트 코드 구현 없음
- test file 생성 없음
- production C# 수정 없음
- `FileNamePolicyService` 수정 없음
- seed constant 수정 없음
- allowlist accessor 수정 없음
- full equality test 구현 없음
- CategoryItem 구현 없음
- CategoryItem JSON storage 구현 없음
- JSON storage implementation 없음
- 실제 JSON file 생성 없음
- SQLite DB/package 추가 없음
- repository/data access/migration 구현 없음
- WPF UI/XAML/navigation/ViewModel 구현 없음
- `attachments/`, `data/local` 내부 파일 생성 없음
- 실제 개인정보 샘플 없음

## J. Risks

- full equality test는 accessor 기준과 seed 기준의 불일치를 확인하지만, 두 기준을 단일 source로 통합하는 것은 아니다.
- accessor가 production API로 존재하는 구조적 부담은 계속 남는다.
- 기존 black-box test와 일부 역할이 중복될 수 있다.
- New Candidate 정책이 바뀌면 equality/exclusion test도 갱신해야 한다.
- 반환 컬렉션 mutation test는 구현 방식에 따라 과도한 테스트가 될 수 있다.
- `DocumentTypeSeeds` label/sortOrder/disabledAt은 full equality test 대상이 아니므로 metadata drift는 별도 테스트 범위로 남는다.

## K. Recommendation

1. 이 문서를 기준으로 full equality consistency test 방향 결정을 받는다.
2. 사용자 결정 후 `docs/83_FULL_EQUALITY_CONSISTENCY_TEST_USER_DECISION_RECORD.md`를 생성한다.
3. 그 다음 별도 작업으로 full equality consistency test를 구현한다.
4. 구현 후 `dotnet build FamilyClaimRef.sln`, `dotnet test FamilyClaimRef.sln`을 실행한다.
5. 구현 후 `docs/84_FULL_EQUALITY_CONSISTENCY_TEST_IMPLEMENTATION_REVIEW.md`를 생성한다.
6. JSON storage implementation은 아직 진행하지 않는다.

## L. Result

`FULL_EQUALITY_CONSISTENCY_TEST_DESIGN_DRAFTED`
