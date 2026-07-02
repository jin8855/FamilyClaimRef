# Document Type Seed Constant Implementation Review

## A. Goal

이 문서는 `DocumentTypeSeed` / `DocumentTypeSeeds` 1차 구현 결과 리뷰 문서다.

목적은 document type seed constant 구현 결과를 기록하고, 생성 파일, seed baseline, 검증 결과, 범위 준수 여부, 남은 위험을 정리하는 것이다.

이 문서는 다음 작업의 리뷰가 아니다.

- `FileNamePolicyService` 통합 리뷰가 아니다.
- allowlist/seed consistency test 구현 리뷰가 아니다.
- CategoryItem JSON storage 구현 리뷰가 아니다.
- JSON storage implementation 리뷰가 아니다.

## B. Checked Files / Paths

| 대상 | 확인 목적 | 판정 |
|---|---|---|
| `docs/74_DOCUMENT_TYPE_SEED_CONSTANT_USER_DECISION_RECORD.md` | 사용자 결정 기준 확인 | PASS |
| `docs/73_DOCUMENT_TYPE_SEED_CONSTANT_IMPLEMENTATION_DECISION.md` | 구현 범위 후보와 seed baseline 확인 | PASS |
| `docs/72_CSHARP_MODEL_INTERFACE_IMPLEMENTATION_REVIEW.md` | 기존 C# model/interface 구현 결과 확인 | PASS |
| `docs/71_CSHARP_MODEL_INTERFACE_USER_DECISION_RECORD.md` | model/interface 결정 기준 확인 | PASS |
| `docs/65_CATEGORY_ITEM_DOCUMENT_TYPE_USER_DECISION_RECORD.md` | CategoryItem/document type 사용자 결정 확인 | PASS |
| `docs/64_CATEGORY_ITEM_DOCUMENT_TYPE_POLICY_DECISION.md` | document type 정책 기준 확인 | PASS |
| `docs/59_FILENAME_POLICY_TEST_PROJECT_IMPLEMENTATION_REVIEW.md` | 기존 test project와 검증 기준 확인 | PASS |
| `app/FamilyClaimRef.App/Models/Storage/DocumentTypeSeed.cs` | seed item record 구현 확인 | PASS |
| `app/FamilyClaimRef.App/Models/Storage/DocumentTypeSeeds.cs` | fixed seed constant 구현 확인 | PASS |
| `app/FamilyClaimRef.App/Services/FileNamePolicyService.cs` | 기존 allowlist 기준 확인 | PASS |
| `tests/FamilyClaimRef.App.Tests/FileNamePolicyServiceTests.cs` | 기존 테스트 유지 여부 확인 | PASS |
| `FamilyClaimRef.sln` | build/test 대상 solution 확인 | PASS |
| `app/FamilyClaimRef.App/FamilyClaimRef.App.csproj` | app project 확인 | PASS |

## C. Implementation Summary

- `DocumentTypeSeed` record가 구현되었다.
- `DocumentTypeSeeds` static seed constant가 구현되었다.
- scope string `claim`, `policy`가 유지되었다.
- current allowlist만 seed에 포함되었다.
- New Candidate는 seed에서 제외되었다.
- claim seed 7개가 포함되었다.
- policy seed 5개가 포함되었다.
- `FileNamePolicyService.cs` 수정은 없었다.
- 기존 test code 수정은 없었다.
- `.sln` 수정은 없었다.
- `.csproj` 수정은 없었다.
- NuGet package 추가는 없었다.
- allowlist/seed consistency test 구현은 없었다.
- CategoryItem JSON 저장 구현은 없었다.
- JSON storage implementation은 없었다.
- 실제 JSON file 생성은 없었다.

## D. Seed Model Review

`DocumentTypeSeed`는 fixed seed item model 역할만 수행한다.

포함 field:

- `Code`
- `Label`
- `Scope`
- `SortOrder`
- `DisabledAt`

확인 결과:

- `DisabledAt`는 nullable `DateTimeOffset?` 기준이다.
- 실제 개인정보, 실제 기관명, 실제 진단명 샘플은 없다.
- CategoryItem JSON storage model이 아니다.
- fixed seed item model 역할만 수행한다.

판정: PASS

## E. Seed List Review

`DocumentTypeSeeds`는 fixed seed 목록을 제공하는 static class다.

확인 결과:

- `ClaimScope = "claim"` 기준이 존재한다.
- `PolicyScope = "policy"` 기준이 존재한다.
- claim seed 목록이 제공된다.
- policy seed 목록이 제공된다.
- `All` 목록으로 claim/policy seed를 합쳐 조회할 수 있다.
- 공개 API는 읽기 전용 성격의 `IReadOnlyList<DocumentTypeSeed>`로 제공된다.
- 외부에서 seed 목록을 쉽게 mutate하지 않도록 `Array.AsReadOnly(...)`로 노출한다.

판정: PASS

## F. Current Seed Baseline Review

현재 구현된 seed는 아래 baseline과 일치한다.

### Claim seed baseline

| Code | Label | Scope | SortOrder | DisabledAt |
|---|---|---|---:|---|
| `receipt` | 영수증 | `claim` | 10 | null |
| `diagnosis` | 진단서 | `claim` | 20 | null |
| `medicine` | 약제비 서류 | `claim` | 30 | null |
| `visit` | 통원 확인 서류 | `claim` | 40 | null |
| `admission` | 입퇴원 확인 서류 | `claim` | 50 | null |
| `surgery` | 수술 확인 서류 | `claim` | 60 | null |
| `etc` | 기타 | `claim` | 999 | null |

### Policy seed baseline

| Code | Label | Scope | SortOrder | DisabledAt |
|---|---|---|---:|---|
| `policy` | 보험증권 | `policy` | 10 | null |
| `terms` | 약관 | `policy` | 20 | null |
| `contract` | 계약서 | `policy` | 30 | null |
| `capture` | 캡처 | `policy` | 40 | null |
| `etc` | 기타 | `policy` | 999 | null |

### Excluded New Candidate

아래 항목은 seed에 포함되지 않았다.

- `statement`
- `prescription`
- claim scope `capture`

기록 기준:

- 위 항목은 계속 `New Candidate / Needs Implementation`이다.
- 허용하려면 별도 정책 결정, `FileNamePolicyService` patch, 테스트 케이스 갱신이 필요하다.

판정: PASS

## G. Scope Compliance Review

아래 금지 범위는 지켜졌다.

- `FileNamePolicyService.cs` 수정 없음
- 기존 test code 수정 없음
- test project 수정 없음
- `.sln` 수정 없음
- `.csproj` 수정 없음
- NuGet package 추가 없음
- allowlist/seed consistency test 구현 없음
- CategoryItem 구현 없음
- CategoryItem JSON 저장 구현 없음
- JSON storage implementation 없음
- 실제 JSON file 생성 없음
- `data/local/*.json` 생성 없음
- SQLite DB/package 추가 없음
- repository/data access/migration 구현 없음
- actual file copy/storage 구현 없음
- WPF UI/XAML/navigation/ViewModel 구현 없음
- `attachments/`, `data/local` 내부 파일 생성 없음
- 실제 개인정보 샘플 사용 없음

판정: PASS

## H. Verification Result

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
- 총 테스트 개수: 33
- 실패 테스트: 0
- 실패 원인: 없음

권한 관련 기록:

- build/test는 Windows/WPF SDK 경로 접근 때문에 권한 상승 환경에서 실행했고 PASS를 확인했다.
- 이번 seed constant 구현 검증에서는 별도 실패 후 재시도는 없었다.
- 코드 실패와 실행 환경 권한 문제를 구분해 기록한다.

## I. Out of Scope / Not Implemented

아래 항목은 아직 구현되지 않았다.

- allowlist/seed consistency test 없음
- seed constant와 `FileNamePolicyService` 통합 없음
- CategoryItem JSON storage 없음
- CategoryItem 구현 없음
- JSON storage implementation 없음
- 실제 JSON file 생성 없음
- reference validation 구현 없음
- schema migration/load failure policy 없음
- actual file attachment service 구현 없음
- WPF UI/ViewModel 구현 없음

## J. Risks

- `FileNamePolicyService` allowlist와 seed constant drift 위험은 아직 남아 있다.
- allowlist/seed consistency test가 아직 없다.
- seed constant와 `FileNamePolicyService` 통합은 아직 없다.
- New Candidate 허용 정책은 아직 미결정이다.
- label 표현이 실제 UI UX와 맞지 않을 수 있다.
- sortOrder 기준은 이후 UI에서 바뀔 수 있다.
- CategoryItem JSON storage 구현은 아직 없다.
- JSON storage implementation은 아직 없다.

## K. Recommendation

1. 현재 document type seed constant 구현 기준은 build/test PASS 상태로 고정한다.
2. 다음 작업은 allowlist/seed consistency test 설계 문서 생성이 적절하다.
3. 그 다음 consistency test를 구현한다.
4. 그 다음 seed constant와 `FileNamePolicyService` 통합 여부를 검토한다.
5. JSON storage implementation은 아직 진행하지 않는다.

## L. Result

`DOCUMENT_TYPE_SEED_CONSTANT_IMPLEMENTATION_REVIEWED`
