# Allowlist / Seed Consistency Test Implementation Review

## A. Goal

이 문서는 `DocumentTypeSeedConsistencyTests.cs` 구현 결과 리뷰 문서다.

목적은 allowlist/seed drift 방지 테스트 구현 결과를 기록하고, 생성 파일, 테스트 범위, 검증 결과, 범위 준수 여부, 남은 위험을 정리하는 것이다.

이 문서는 다음 작업의 리뷰가 아니다.

- production code 수정 리뷰가 아니다.
- JSON storage 구현 리뷰가 아니다.
- CategoryItem JSON storage 구현 리뷰가 아니다.

## B. Checked Files / Paths

| 대상 | 확인 목적 | 판정 |
|---|---|---|
| `docs/77_ALLOWLIST_SEED_CONSISTENCY_TEST_USER_DECISION_RECORD.md` | 사용자 결정 기준 확인 | PASS |
| `docs/76_ALLOWLIST_SEED_CONSISTENCY_TEST_DESIGN.md` | 테스트 설계 기준 확인 | PASS |
| `docs/75_DOCUMENT_TYPE_SEED_CONSTANT_IMPLEMENTATION_REVIEW.md` | seed constant 구현 결과 확인 | PASS |
| `docs/74_DOCUMENT_TYPE_SEED_CONSTANT_USER_DECISION_RECORD.md` | document type seed 사용자 결정 확인 | PASS |
| `docs/72_CSHARP_MODEL_INTERFACE_IMPLEMENTATION_REVIEW.md` | 기존 C# model/interface 구현 결과 확인 | PASS |
| `docs/59_FILENAME_POLICY_TEST_PROJECT_IMPLEMENTATION_REVIEW.md` | 기존 test project 기준 확인 | PASS |
| `tests/FamilyClaimRef.App.Tests/DocumentTypeSeedConsistencyTests.cs` | 신규 consistency test 구현 확인 | PASS |
| `tests/FamilyClaimRef.App.Tests/FileNamePolicyServiceTests.cs` | 기존 test file 유지 확인 | PASS |
| `app/FamilyClaimRef.App/Models/Storage/DocumentTypeSeed.cs` | seed item model 유지 확인 | PASS |
| `app/FamilyClaimRef.App/Models/Storage/DocumentTypeSeeds.cs` | fixed seed constant 유지 확인 | PASS |
| `app/FamilyClaimRef.App/Services/FileNamePolicyService.cs` | allowlist service 유지 확인 | PASS |
| `tests/FamilyClaimRef.App.Tests/FamilyClaimRef.App.Tests.csproj` | test project 수정 없음 확인 | PASS |
| `app/FamilyClaimRef.App/FamilyClaimRef.App.csproj` | app project 수정 없음 확인 | PASS |
| `FamilyClaimRef.sln` | solution 수정 없음 확인 | PASS |

## C. Implementation Summary

- `tests/FamilyClaimRef.App.Tests/DocumentTypeSeedConsistencyTests.cs`가 생성되었다.
- seed structure test가 구현되었다.
- `FileNamePolicyService` black-box acceptance test가 구현되었다.
- New Candidate rejection test가 구현되었다.
- production C# 수정은 없었다.
- `FileNamePolicyService.cs` 수정은 없었다.
- `DocumentTypeSeed.cs` 수정은 없었다.
- `DocumentTypeSeeds.cs` 수정은 없었다.
- 기존 test file 수정은 없었다.
- `.sln` 수정은 없었다.
- `.csproj` 수정은 없었다.
- NuGet package 추가는 없었다.
- JSON storage implementation은 없었다.
- 실제 JSON file 생성은 없었다.

## D. Test Coverage Review

### Seed structure tests

아래 항목이 검증되었다.

- claim seed count = 7
- policy seed count = 5
- all seed count = 12
- 모든 seed `Code` non-empty
- 모든 seed `Label` non-empty
- 모든 seed `Scope`는 `claim` 또는 `policy`
- `(Scope, Code)` 조합 중복 없음
- scope별 `SortOrder` 중복 없음
- 모든 seed `DisabledAt == null`

### Acceptance tests

아래 항목이 검증되었다.

- claim seed 7개는 claim scope에서 accepted
- policy seed 5개는 policy scope에서 accepted
- seed가 `FileNamePolicyService.CreatePhysicalFileName(...)`에서 accepted 되는지 black-box 검증
- policy `capture`는 policy scope에서 accepted
- `etc`는 claim/policy 양쪽 scope 허용 예외 반영

### Rejection tests

아래 항목이 검증되었다.

- New Candidate claim `statement` rejected
- New Candidate claim `prescription` rejected
- New Candidate claim `capture` rejected
- scope mismatch rejected
- `etc` scope 양쪽 허용 예외 처리

## E. Verification Result

검증 명령:

```powershell
dotnet build FamilyClaimRef.sln
dotnet test FamilyClaimRef.sln
```

검증 결과:

- `dotnet build FamilyClaimRef.sln`: PASS
- warning: 0
- error: 0
- `dotnet test FamilyClaimRef.sln`: PASS
- 총 테스트 개수: 62
- 추가된 테스트 개수: 29
- 실패 테스트: 0
- 실패 원인: 없음
- 권한 상승 실행 여부: 있음
- 초기 실패/재시도 여부: 없음

권한 관련 기록:

- build/test는 Windows/WPF SDK 경로 접근 때문에 권한 상승 환경에서 실행했고 PASS를 확인했다.
- 이번 리뷰 문서 작성 단계에서는 별도 실패 후 재시도는 없었다.
- 코드 실패와 실행 환경 권한 문제를 구분해 기록한다.

## F. Scope Compliance Review

아래 금지 범위는 지켜졌다.

- production C# 수정 없음
- `FileNamePolicyService.cs` 수정 없음
- `DocumentTypeSeed.cs` 수정 없음
- `DocumentTypeSeeds.cs` 수정 없음
- 기존 test file 수정 없음
- `.sln` 수정 없음
- `.csproj` 수정 없음
- NuGet package 추가 없음
- allowlist accessor 추가 없음
- CategoryItem 구현 없음
- CategoryItem JSON storage 구현 없음
- JSON storage implementation 없음
- 실제 JSON file 생성 없음
- SQLite DB/package 추가 없음
- repository/data access/migration 구현 없음
- WPF UI/XAML/navigation/ViewModel 구현 없음
- `attachments/`, `data/local` 내부 파일 생성 없음
- 실제 개인정보 샘플 사용 없음

## G. Out of Scope / Not Implemented

아래 항목은 아직 구현되지 않았다.

- full equality allowlist accessor 없음
- seed constant와 `FileNamePolicyService` 통합 없음
- CategoryItem JSON storage 없음
- JSON storage implementation 없음
- reference validation 구현 없음
- schema migration/load failure policy 없음
- actual file attachment service 구현 없음
- WPF UI/ViewModel 구현 없음

## H. Risks

- black-box test는 seed가 service에서 accepted 되는지는 검증하지만, service 내부에 seed에는 없는 추가 allowlist가 있는지는 완전하게 검증하지 못한다.
- full equality test는 allowlist accessor 없이 제한적이다.
- seed constant와 `FileNamePolicyService` 통합은 아직 없다.
- New Candidate 허용 정책은 아직 미결정이다.
- label/sortOrder는 실제 UI 단계에서 변경될 수 있다.
- JSON storage implementation은 아직 없다.

## I. Recommendation

1. 현재 allowlist/seed consistency test 기준은 build/test PASS 상태로 고정한다.
2. 다음 작업은 seed constant와 `FileNamePolicyService` 통합 여부 결정 문서가 적절하다.
3. JSON storage implementation은 아직 진행하지 않는다.
4. full equality allowlist accessor를 도입할지는 통합 여부 결정 문서에서 검토한다.
5. actual file attachment service와 UI 구현은 계속 보류한다.

## J. Result

`ALLOWLIST_SEED_CONSISTENCY_TEST_IMPLEMENTATION_REVIEWED`
