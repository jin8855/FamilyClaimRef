# Allowlist / Seed Consistency Test Design

## A. Goal

이 문서는 `FileNamePolicyService` document type allowlist와 `DocumentTypeSeeds` fixed seed 사이의 drift 방지 테스트 설계 문서다.

이 문서는 테스트 구현 문서가 아니다.

이 문서는 production code 수정 문서가 아니다.

이 문서는 JSON storage 구현 문서가 아니다.

## B. Current State

- `FileNamePolicyService` 구현은 완료되어 있다.
- `FileNamePolicyService` 기존 테스트 33개는 PASS 기준으로 기록되어 있다.
- `DocumentTypeSeed` 구현은 완료되어 있다.
- `DocumentTypeSeeds` 구현은 완료되어 있다.
- 최근 구현 리뷰 기준 build/test는 PASS로 기록되어 있다.
- claim seed는 7개다.
- policy seed는 5개다.
- `FileNamePolicyService` 수정은 없었다.
- allowlist/seed consistency test는 아직 없다.
- seed constant와 `FileNamePolicyService` 통합은 아직 없다.
- CategoryItem JSON storage는 아직 없다.
- JSON storage implementation은 아직 없다.
- New Candidate는 seed에서 제외되어 있다.

현재 current allowlist / seed baseline은 다음과 같다.

### Claim current seed

- `receipt`
- `diagnosis`
- `medicine`
- `visit`
- `admission`
- `surgery`
- `etc`

### Policy current seed

- `policy`
- `terms`
- `contract`
- `capture`
- `etc`

### New Candidate / Needs Implementation

- `statement`
- `prescription`
- claim scope `capture`

## C. Test Problem Statement

`FileNamePolicyService`는 `documentType` allowlist를 기준으로 물리 파일명을 생성한다.

`DocumentTypeSeeds`는 UI label, sortOrder, scope seed 기준을 제공한다.

이 두 기준이 어긋나면 다음 문제가 생길 수 있다.

- 화면에서는 선택 가능한 document type인데 파일명 생성에서는 거부될 수 있다.
- 파일명 생성은 가능하지만 seed 목록에는 없는 document type이 생길 수 있다.
- scope별로 허용되는 document type 기준이 서로 다르게 유지될 수 있다.
- New Candidate가 정책 결정 없이 seed나 service allowlist 중 한쪽에만 반영될 수 있다.

현재 두 기준은 코드상 통합되어 있지 않다. 따라서 drift 방지를 위한 테스트가 필요하다.

## D. Test Strategy Candidate

### Candidate 1. 테스트 없이 문서 기준만 유지

내용:

- `FileNamePolicyService` allowlist와 `DocumentTypeSeeds` baseline을 문서로만 관리한다.
- 테스트 파일은 추가하지 않는다.

장점:

- 구현 부담이 없다.
- 현재 코드 변경이 없다.

단점:

- drift를 자동으로 잡지 못한다.
- seed나 allowlist가 바뀌어도 실패 신호가 없다.

### Candidate 2. black-box acceptance test

내용:

- `DocumentTypeSeeds`의 각 seed를 `FileNamePolicyService.CreatePhysicalFileName(...)`에 넣어 accepted 여부를 검증한다.
- seed에 있는 code가 해당 scope에서 파일명 생성에 성공해야 한다.
- New Candidate는 현재 거부되어야 한다.
- production code는 수정하지 않는다.

장점:

- `FileNamePolicyService` 내부 allowlist를 노출하지 않아도 된다.
- production code 수정이 없다.
- 현재 구조에서 가장 안전하다.
- seed가 service에서 받아들여지는지 직접 검증할 수 있다.

단점:

- `FileNamePolicyService` 내부에는 있지만 seed에는 없는 추가 allowlist를 완전하게 감지하지 못한다.
- `seed -> allowlist` 검증에는 강하지만 `allowlist -> seed` 검증은 제한적이다.

### Candidate 3. allowlist accessor 추가 후 full equality test

내용:

- `FileNamePolicyService` 또는 별도 provider에서 allowlist를 공개한다.
- 공개 allowlist와 seed code set의 full equality를 검증한다.

장점:

- 양방향 일치 검증이 가능하다.
- drift 방지 강도가 가장 높다.

단점:

- production code 수정이 필요하다.
- 현재 단계 범위를 넘어선다.
- allowlist 구조를 공개 API로 노출하는 설계 부담이 생긴다.

## E. Recommended Direction

Candidate Recommendation:

- 다음 테스트 구현은 Candidate 2, 즉 black-box acceptance test로 간다.
- production code는 수정하지 않는다.
- `FileNamePolicyService` allowlist accessor는 아직 만들지 않는다.
- `DocumentTypeSeeds`의 current seed가 `FileNamePolicyService`에서 accepted 되는지 검증한다.
- New Candidate `statement`, `prescription`, claim scope `capture`가 현재 rejected 되는지 검증한다.
- seed 자체의 최소 구조도 함께 검증한다.

Seed structure test 후보:

- code 비어 있음 금지
- label 비어 있음 금지
- scope는 `claim` 또는 `policy`
- `(Scope, Code)` 조합 중복 없음
- scope별 sortOrder 검증
- `DisabledAt`는 현재 모든 seed에서 null

위 방향은 확정이 아니라 `Candidate Recommendation`이다.

## F. Proposed Test Cases

### Seed structure tests

후보:

- `DocumentTypeSeeds.Claim` count는 7
- `DocumentTypeSeeds.Policy` count는 5
- `DocumentTypeSeeds.All` count는 12
- 모든 seed의 `Code`는 비어 있지 않다.
- 모든 seed의 `Label`은 비어 있지 않다.
- 모든 seed의 `Scope`는 `claim` 또는 `policy`다.
- `(Scope, Code)` 조합은 중복이 없다.
- 각 scope별 `SortOrder`는 중복이 없거나 정렬 가능해야 한다.
- `DisabledAt`는 현재 모든 seed에서 null이다.

### FileNamePolicyService acceptance tests

후보:

- claim seed 7개는 `FileNamePolicyService.CreatePhysicalFileName(...)`에서 claim scope로 성공해야 한다.
- policy seed 5개는 `FileNamePolicyService.CreatePhysicalFileName(...)`에서 policy scope로 성공해야 한다.
- claim seed를 policy scope에 넣었을 때 scope 불일치 항목은 거부되어야 한다.
- policy seed를 claim scope에 넣었을 때 scope 불일치 항목은 거부되어야 한다.
- 단, 양쪽 scope에 모두 있는 `etc`는 양쪽 scope에서 허용될 수 있으므로 예외 처리해야 한다.

### New Candidate rejection tests

후보:

- claim `statement`는 현재 `ArgumentException`이어야 한다.
- claim `prescription`은 현재 `ArgumentException`이어야 한다.
- claim `capture`는 현재 `ArgumentException`이어야 한다.

주의:

- policy `capture`는 current allowlist이므로 policy scope에서는 accepted 되어야 한다.

## G. Test File Candidate

후속 구현 후보 파일:

- `tests/FamilyClaimRef.App.Tests/DocumentTypeSeedsTests.cs`
- `tests/FamilyClaimRef.App.Tests/DocumentTypeSeedConsistencyTests.cs`

추천 후보:

- `tests/FamilyClaimRef.App.Tests/DocumentTypeSeedConsistencyTests.cs`

주의:

- 이번 문서 작성 단계에서는 테스트 파일을 생성하지 않는다.

## H. Test Implementation Boundary

후속 테스트 구현 범위 후보:

- test project에만 테스트 파일을 추가한다.
- app production code 수정은 하지 않는다.
- `FileNamePolicyService.cs` 수정은 하지 않는다.
- `DocumentTypeSeed.cs` 수정은 하지 않는다.
- `DocumentTypeSeeds.cs` 수정은 하지 않는다.
- `.sln`, `.csproj` 수정은 하지 않는다.
- NuGet package 추가는 하지 않는다.
- JSON storage 구현은 하지 않는다.

검증 명령 후보:

```powershell
dotnet build FamilyClaimRef.sln
dotnet test FamilyClaimRef.sln
```

예상 테스트 수:

- 현재 33개에서 증가한다.
- 정확한 추가 개수는 구현 단계에서 보고한다.

## I. Needs Decision

1. 다음 테스트 구현은 black-box acceptance test 방식으로 갈 것인가?
2. production code 수정 없이 진행할 것인가?
3. `FileNamePolicyService` allowlist accessor는 만들지 않을 것인가?
4. `DocumentTypeSeeds`의 seed가 `FileNamePolicyService`에서 accepted 되는지 검증할 것인가?
5. New Candidate `statement`, `prescription`, claim `capture`가 rejected 되는지 검증할 것인가?
6. seed structure test를 포함할 것인가?
7. `(Scope, Code)` 중복 검증을 포함할 것인가?
8. `DisabledAt`이 현재 모두 null인지 검증할 것인가?
9. scope별 sortOrder 검증을 포함할 것인가?
10. 테스트 파일 후보는 `DocumentTypeSeedConsistencyTests.cs`로 할 것인가?
11. 테스트 구현 후 `dotnet build`, `dotnet test`를 실행할 것인가?
12. JSON storage implementation은 계속 제외할 것인가?

## J. Out of Scope

이번 문서에서 제외하는 범위는 다음과 같다.

- 테스트 코드 구현 없음
- test file 생성 없음
- production C# 수정 없음
- `FileNamePolicyService` 수정 없음
- seed constant 수정 없음
- `.sln` 수정 없음
- `.csproj` 수정 없음
- NuGet package 추가 없음
- allowlist accessor 추가 없음
- allowlist/seed consistency test 구현 없음
- CategoryItem 구현 없음
- CategoryItem JSON storage 구현 없음
- JSON storage implementation 없음
- 실제 JSON file 생성 없음
- SQLite DB/package 추가 없음
- repository/data access/migration 구현 없음
- actual file copy/storage 구현 없음
- WPF UI/XAML/navigation/ViewModel 구현 없음
- `attachments/`, `data/local` 내부 파일 생성 없음
- 실제 개인정보 샘플 사용 없음

## K. Risks

- black-box test는 seed가 service에서 accepted 되는지는 검증하지만, service 내부에 seed에는 없는 추가 allowlist가 있는지는 완전하게 검증하지 못한다.
- 양방향 full equality 검증은 production code accessor 없이는 제한적이다.
- scope 교차 테스트에서는 `etc`처럼 양쪽 scope에 있는 code를 예외 처리해야 한다.
- New Candidate 정책이 바뀌면 rejection test를 갱신해야 한다.
- label/sortOrder는 UI 단계에서 변경될 수 있다.
- `DocumentTypeSeeds` label의 실제 표시 문구는 UI UX 단계에서 별도 검토가 필요하다.

## L. Recommendation

1. 이 문서를 기준으로 allowlist/seed consistency test 방향 결정을 받는다.
2. 사용자 결정 후 `docs/77_ALLOWLIST_SEED_CONSISTENCY_TEST_USER_DECISION_RECORD.md`를 생성한다.
3. 그 다음 별도 작업으로 consistency test를 구현한다.
4. 구현 후 build/test를 실행한다.
5. 그 다음 `docs/78_ALLOWLIST_SEED_CONSISTENCY_TEST_IMPLEMENTATION_REVIEW.md`를 생성한다.

## M. Result

`ALLOWLIST_SEED_CONSISTENCY_TEST_DESIGN_DRAFTED`
