# FileNamePolicyService Allowlist Accessor Implementation Review

## A. Goal

이 문서는 `FileNamePolicyService.GetAllowedDocumentTypes(string documentScope)` 구현 결과 리뷰 문서다.

목적은 `FileNamePolicyService`에 allowlist accessor가 추가된 결과를 기록하고, 구현 범위, 검증 결과, 범위 준수 여부, 남은 위험을 정리하는 것이다.

이 문서는 다음 작업의 리뷰가 아니다.

- `CreatePhysicalFileName(...)` behavior 변경 리뷰가 아니다.
- full equality test 구현 리뷰가 아니다.
- JSON storage 구현 리뷰가 아니다.
- CategoryItem 구현 리뷰가 아니다.
- WPF UI/ViewModel 구현 리뷰가 아니다.

## B. Checked Files / Paths

| 대상 | 확인 목적 | 판정 |
|---|---|---|
| `docs/80_DOCUMENT_TYPE_SEED_POLICY_SERVICE_INTEGRATION_USER_DECISION_RECORD.md` | allowlist accessor 사용자 결정 기준 확인 | PASS |
| `docs/79_DOCUMENT_TYPE_SEED_POLICY_SERVICE_INTEGRATION_DECISION.md` | integration decision 방향 확인 | PASS |
| `docs/78_ALLOWLIST_SEED_CONSISTENCY_TEST_IMPLEMENTATION_REVIEW.md` | 기존 consistency test 구현 결과 확인 | PASS |
| `docs/77_ALLOWLIST_SEED_CONSISTENCY_TEST_USER_DECISION_RECORD.md` | black-box consistency test 사용자 결정 확인 | PASS |
| `docs/76_ALLOWLIST_SEED_CONSISTENCY_TEST_DESIGN.md` | consistency test 설계 기준 확인 | PASS |
| `docs/75_DOCUMENT_TYPE_SEED_CONSTANT_IMPLEMENTATION_REVIEW.md` | seed constant 구현 결과 확인 | PASS |
| `app/FamilyClaimRef.App/Services/FileNamePolicyService.cs` | allowlist accessor 구현 확인 | PASS |
| `app/FamilyClaimRef.App/Models/Storage/DocumentTypeSeed.cs` | seed item model 유지 확인 | PASS |
| `app/FamilyClaimRef.App/Models/Storage/DocumentTypeSeeds.cs` | fixed seed constant 유지 확인 | PASS |
| `tests/FamilyClaimRef.App.Tests/DocumentTypeSeedConsistencyTests.cs` | 기존 consistency test 유지 확인 | PASS |
| `tests/FamilyClaimRef.App.Tests/FileNamePolicyServiceTests.cs` | 기존 filename policy test 유지 확인 | PASS |
| `tests/FamilyClaimRef.App.Tests/FamilyClaimRef.App.Tests.csproj` | test project 수정 없음 확인 | PASS |
| `app/FamilyClaimRef.App/FamilyClaimRef.App.csproj` | app project 수정 없음 확인 | PASS |
| `FamilyClaimRef.sln` | solution 수정 없음 확인 | PASS |

## C. Implementation Summary

- `FileNamePolicyService.GetAllowedDocumentTypes(string documentScope)`가 추가되었다.
- claim/policy scope별 current allowlist code set 조회가 가능해졌다.
- claim allowlist 7개가 유지되었다.
- policy allowlist 5개가 유지되었다.
- New Candidate는 accessor 결과에 포함되지 않는다.
- `CreatePhysicalFileName(...)` 동작 변경은 없다.
- `FileNamePolicyService`가 `DocumentTypeSeeds`를 직접 참조하지 않는다.
- full equality test는 아직 구현되지 않았다.
- 기존 test file 수정은 없다.
- 새 test file 생성은 없다.
- `.sln` 수정은 없다.
- `.csproj` 수정은 없다.
- NuGet package 추가는 없다.
- JSON storage implementation은 없다.
- 실제 JSON file 생성은 없다.

## D. Accessor Review

`GetAllowedDocumentTypes(string documentScope)` 검토 결과:

| 기준 | 확인 내용 | 판정 |
|---|---|---|
| claim scope | `documentScope = "claim"`이면 claim allowlist를 반환한다. | PASS |
| policy scope | `documentScope = "policy"`이면 policy allowlist를 반환한다. | PASS |
| 반환 범위 | scope별 current allowlist code set만 포함한다. | PASS |
| metadata 노출 | label, sortOrder, disabledAt은 노출하지 않는다. | PASS |
| New Candidate 제외 | claim `statement`, claim `prescription`, claim `capture`는 포함하지 않는다. | PASS |
| invalid scope | 기존 `NormalizeDocumentScope(...)` 기준과 같은 `ArgumentException` 흐름을 사용한다. | PASS |
| 외부 mutation | 내부 `HashSet<string>`을 직접 반환하지 않고 배열 copy를 `IReadOnlyCollection<string>`로 반환한다. | PASS_WITH_NOTES |
| JSON 정보 노출 | JSON path/file name을 노출하지 않는다. | PASS |
| seed 직접 참조 | `DocumentTypeSeeds`를 직접 참조하지 않는다. | PASS |

비고:

- 반환 타입은 `IReadOnlyCollection<string>`이므로 호출자가 accessor 반환값을 통해 내부 allowlist를 직접 mutate할 수 없다.
- 구현은 `ToArray()` copy를 반환하므로 내부 `HashSet<string>` 참조가 외부로 노출되지 않는다.
- full equality test가 아직 없으므로 accessor와 seed의 완전 일치 검증은 후속 작업으로 남아 있다.

## E. Current Allowlist Review

### Claim allowlist

현재 claim allowlist는 아래 7개다.

- `receipt`
- `diagnosis`
- `medicine`
- `visit`
- `admission`
- `surgery`
- `etc`

### Policy allowlist

현재 policy allowlist는 아래 5개다.

- `policy`
- `terms`
- `contract`
- `capture`
- `etc`

### Excluded New Candidate

아래 항목은 accessor 결과에 포함되지 않아야 하며, 현재 기준에서 제외 상태다.

- claim `statement`
- claim `prescription`
- claim `capture`

주의:

- policy `capture`는 policy allowlist에 포함되어야 한다.
- claim `capture`는 claim allowlist에 포함되면 안 된다.
- `etc`는 claim/policy 양쪽 allowlist에 포함될 수 있다.

## F. Verification Result

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
- 실패 테스트: 없음
- 실패 원인: 없음
- 권한 상승 실행 여부: 있음, build/test 실행용
- 초기 실패/재시도 여부: build/test 기준 없음

권한 관련 기록:

- build/test는 Windows/WPF SDK 및 로컬 .NET SDK 경로 접근 때문에 권한 상승 환경에서 실행했고 PASS를 확인했다.
- 이번 리뷰 문서 작성 단계에서 build/test 실패 후 코드 수정은 없었다.

## G. Scope Compliance Review

아래 금지 범위는 지켜졌다.

- `DocumentTypeSeed.cs` 수정 없음
- `DocumentTypeSeeds.cs` 수정 없음
- 기존 test code 수정 없음
- 새 test file 생성 없음
- full equality test 구현 없음
- `.sln` 수정 없음
- `.csproj` 수정 없음
- NuGet package 추가 없음
- CategoryItem 구현 없음
- CategoryItem JSON storage 구현 없음
- JSON storage implementation 없음
- 실제 JSON file 생성 없음
- SQLite DB/package 추가 없음
- repository/data access/migration 구현 없음
- WPF UI/XAML/navigation/ViewModel 구현 없음
- `attachments/`, `data/local` 내부 파일 생성 없음
- 실제 개인정보 샘플 사용 없음

추가 확인:

- `FileNamePolicyService.cs` 외 production source 수정은 없다.
- 이번 리뷰 문서 작성 작업에서는 production C# 수정이 없다.
- 이번 리뷰 문서 작성 작업에서는 test code 수정이 없다.

## H. Out of Scope / Not Implemented

아래 항목은 아직 구현되지 않았다.

- full equality test 없음
- accessor와 seed 기준 일치 검증 자동화 없음
- seed constant와 `FileNamePolicyService` 실제 통합 없음
- `FileNamePolicyService`가 `DocumentTypeSeeds`를 직접 참조하지 않음
- 별도 `DocumentTypePolicy` / `DocumentTypeCatalog` 없음
- CategoryItem JSON storage 없음
- JSON storage implementation 없음
- reference validation 구현 없음
- schema migration/load failure policy 없음
- actual file attachment service 구현 없음
- WPF UI/ViewModel 구현 없음

## I. Risks

- full equality test는 아직 없다.
- accessor와 seed 기준 일치 검증은 아직 자동화되지 않았다.
- accessor는 추가되었지만 seed constant와 `FileNamePolicyService`가 단일 기준으로 통합된 것은 아니다.
- allowlist accessor가 production API로 남는 구조적 부담이 있다.
- New Candidate 허용 정책은 아직 미결정이다.
- JSON storage implementation은 아직 없다.

## J. Recommendation

1. 현재 allowlist accessor 구현 기준은 build/test PASS 상태로 고정한다.
2. 다음 작업은 full equality consistency test 설계 또는 구현 범위 결정 문서가 적절하다.
3. 그 다음 accessor와 `DocumentTypeSeeds`의 full equality test를 구현한다.
4. `FileNamePolicyService`가 `DocumentTypeSeeds`를 직접 참조하는 방식은 계속 보류한다.
5. JSON storage implementation은 아직 진행하지 않는다.

## K. Result

`FILENAME_POLICY_ALLOWLIST_ACCESSOR_IMPLEMENTATION_REVIEWED`
