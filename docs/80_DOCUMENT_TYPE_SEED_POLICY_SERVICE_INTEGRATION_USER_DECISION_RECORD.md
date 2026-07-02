# Document Type Seed / FileNamePolicyService Integration User Decision Record

## A. Goal

이 문서는 `docs/79_DOCUMENT_TYPE_SEED_POLICY_SERVICE_INTEGRATION_DECISION.md`의 Needs Decision Q1~Q11에 대한 사용자 결정 기록이다.

목적은 `DocumentTypeSeeds`와 `FileNamePolicyService` allowlist 통합 방향을 확정하고, 다음 단계에서 무엇을 구현하고 무엇을 보류할지 명확히 하는 것이다.

이 문서는 구현 문서가 아니다. production code 수정, allowlist accessor 추가, full equality test 구현은 수행하지 않는다.

## B. Checked Files / Paths

| 대상 | 확인 목적 | 비고 |
|---|---|---|
| `docs/79_DOCUMENT_TYPE_SEED_POLICY_SERVICE_INTEGRATION_DECISION.md` | Q1~Q11 Needs Decision 확인 | 읽기 전용 |
| `docs/78_ALLOWLIST_SEED_CONSISTENCY_TEST_IMPLEMENTATION_REVIEW.md` | consistency test 구현 결과 확인 | 읽기 전용 |
| `docs/77_ALLOWLIST_SEED_CONSISTENCY_TEST_USER_DECISION_RECORD.md` | black-box consistency test 사용자 결정 확인 | 읽기 전용 |
| `docs/75_DOCUMENT_TYPE_SEED_CONSTANT_IMPLEMENTATION_REVIEW.md` | seed constant 구현 결과 확인 | 읽기 전용 |
| `app/FamilyClaimRef.App/Models/Storage/DocumentTypeSeed.cs` | seed item model 확인 | 읽기 전용 |
| `app/FamilyClaimRef.App/Models/Storage/DocumentTypeSeeds.cs` | fixed seed 목록 확인 | 읽기 전용 |
| `app/FamilyClaimRef.App/Services/FileNamePolicyService.cs` | current allowlist 확인 | 읽기 전용 |
| `tests/FamilyClaimRef.App.Tests/DocumentTypeSeedConsistencyTests.cs` | black-box consistency test 확인 | 읽기 전용 |
| `tests/FamilyClaimRef.App.Tests/FileNamePolicyServiceTests.cs` | 기존 filename policy test 확인 | 읽기 전용 |

## C. User Decision Summary

| ID | Question | Decision | Recorded Direction |
|---|---|---|---|
| Q1 | 현 상태 유지가 아니라 full equality 검증을 강화할 것인가? | Accepted | 현 상태 유지에 머무르지 않고 full equality 검증을 강화하는 방향으로 간다. 현재 black-box consistency test는 유지한다. |
| Q2 | 다음 단계에서 allowlist accessor 추가 후보로 갈 것인가? | Accepted | 다음 설계/구현 후보는 `FileNamePolicyService`의 allowlist accessor 추가 방향으로 간다. 이번 문서에서는 구현하지 않는다. |
| Q3 | `FileNamePolicyService`가 `DocumentTypeSeeds`를 직접 참조하는 방식은 보류할 것인가? | Accepted - Deferred | 직접 참조 방식은 보류한다. 파일명 정책 서비스가 seed metadata 구조에 직접 의존하면 책임이 섞일 수 있기 때문이다. |
| Q4 | 별도 `DocumentTypePolicy` / `DocumentTypeCatalog` 도입은 MVP 이후 또는 별도 설계로 보류할 것인가? | Accepted - Deferred | 별도 policy/catalog 도입은 MVP 이후 또는 별도 설계로 보류한다. 현재 MVP 단계에서는 과설계 위험이 있다. |
| Q5 | MVP 1차에서는 allowlist accessor + full equality test가 적절한가? | Accepted | MVP 1차에서는 allowlist accessor + full equality test 방향이 적절하다. production behavior 변경을 최소화하면서 drift 감지력을 높인다. |
| Q6 | accessor를 추가한다면 production behavior는 변경하지 않을 것인가? | Accepted | accessor를 추가하더라도 production behavior는 변경하지 않는다. `CreatePhysicalFileName(...)` 동작은 바꾸지 않는다. |
| Q7 | accessor는 scope별 current allowlist code set만 노출할 것인가? | Accepted | accessor는 scope별 current allowlist code set만 노출한다. label, sortOrder, disabledAt은 `DocumentTypeSeeds` 책임으로 유지한다. |
| Q8 | New Candidate `statement`, `prescription`, claim `capture`는 계속 제외할 것인가? | Accepted | New Candidate는 계속 제외한다. 허용하려면 별도 정책 결정, service patch, seed 갱신, test 갱신이 필요하다. |
| Q9 | full equality test는 accessor 추가 후 별도 작업으로 구현할 것인가? | Accepted | full equality test는 allowlist accessor 추가 후 별도 작업으로 구현한다. 이번 문서에서는 테스트를 구현하지 않는다. |
| Q10 | JSON storage implementation은 계속 제외할 것인가? | Accepted | JSON storage implementation은 계속 제외한다. 실제 JSON file 생성도 제외한다. |
| Q11 | 이번 단계에서는 구현 없이 통합 여부 결정 문서까지만 진행할 것인가? | Accepted | 이번 단계에서는 구현 없이 사용자 결정 기록 문서까지만 진행한다. production code 수정, accessor 추가, full equality test 구현은 별도 작업으로 진행한다. |

## D. Accepted Direction

아래 방향을 사용자 결정으로 기록한다.

- full equality 검증을 강화한다.
- 다음 후보는 allowlist accessor 추가다.
- `FileNamePolicyService`가 `DocumentTypeSeeds`를 직접 참조하는 방식은 보류한다.
- 별도 `DocumentTypePolicy` / `DocumentTypeCatalog` 도입은 MVP 이후 또는 별도 설계로 보류한다.
- MVP 1차는 allowlist accessor + full equality test 방향으로 간다.
- accessor를 추가하더라도 production behavior는 변경하지 않는다.
- accessor는 scope별 current allowlist code set만 노출한다.
- New Candidate는 계속 제외한다.
- full equality test는 accessor 추가 후 별도 작업으로 구현한다.
- JSON storage implementation은 계속 제외한다.
- 이번 단계는 구현 없이 사용자 결정 기록만 한다.

## E. Deferred Items

아래 항목은 보류로 기록한다.

- `FileNamePolicyService`가 `DocumentTypeSeeds`를 직접 참조하는 구조
- 별도 `DocumentTypePolicy` / `DocumentTypeCatalog` 도입
- full equality test 구현
- seed constant와 `FileNamePolicyService` 실제 통합
- New Candidate 허용
- JSON storage implementation
- CategoryItem JSON storage

## F. Still Not Implemented

아래 항목은 아직 구현되지 않았다.

- production C# 수정 없음
- `FileNamePolicyService` 수정 없음
- `DocumentTypeSeed` / `DocumentTypeSeeds` 수정 없음
- allowlist accessor 추가 없음
- test code 수정 없음
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

## G. Next Step

다음 작업 후보:

1. allowlist accessor 구현 범위 결정 또는 구현 지시문 작성
2. accessor 구현 후 build/test 실행
3. accessor 구현 리뷰 문서 생성
4. full equality consistency test 설계/구현
5. seed constant와 `FileNamePolicyService` 실제 통합 여부 재검토
6. JSON storage implementation은 아직 보류

## H. Result

`DOCUMENT_TYPE_SEED_POLICY_SERVICE_INTEGRATION_USER_DECISION_RECORDED`
