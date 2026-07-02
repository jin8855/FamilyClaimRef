# Document Type Seed / FileNamePolicyService Integration Decision

## A. Goal

이 문서는 `DocumentTypeSeeds`와 `FileNamePolicyService` allowlist의 통합 여부를 결정하기 위한 문서다.

현재 black-box consistency test 이후에도 남아 있는 drift 위험을 정리하고, `DocumentTypeSeeds` fixed seed constant와 `FileNamePolicyService` document type allowlist를 어떤 방식으로 관리할지 비교한다.

이 문서는 구현 문서가 아니다. production code 수정, test code 수정, JSON storage 구현은 수행하지 않는다.

## B. Current State

- `FileNamePolicyService` 구현은 완료되어 있다.
- `DocumentTypeSeed` / `DocumentTypeSeeds` 구현은 완료되어 있다.
- `DocumentTypeSeedConsistencyTests.cs` 구현은 완료되어 있다.
- build/test는 PASS 상태로 기록되어 있다.
- 총 테스트 62개가 PASS 상태로 기록되어 있다.
- consistency test는 black-box 방식으로 구현되어 있다.
- claim seed 7개 accepted 검증이 완료되어 있다.
- policy seed 5개 accepted 검증이 완료되어 있다.
- New Candidate claim `statement`, `prescription`, `capture` rejected 검증이 완료되어 있다.
- policy `capture` accepted 검증이 완료되어 있다.
- `etc`는 claim/policy 양쪽 허용 예외로 반영되어 있다.
- production code 수정은 없었다.
- `FileNamePolicyService` 수정은 없었다.
- seed constant 수정은 없었다.
- allowlist accessor는 없다.
- seed constant와 `FileNamePolicyService` 통합은 아직 없다.
- JSON storage implementation은 아직 없다.

현재 black-box test는 `seed -> service accepted` 검증에는 강하다. 다만 service 내부에 seed에는 없는 추가 allowlist가 생기는 경우를 완전하게 검증하지는 못한다.

## C. Problem Statement

현재 `DocumentTypeSeeds`는 UI label, sortOrder, scope 기준이다.

`FileNamePolicyService`는 파일명 생성 시 documentType allowlist를 검증한다.

두 기준이 코드상 분리되어 있으면 장기적으로 drift 위험이 남는다.

현재 black-box consistency test는 seed가 service에서 accepted 되는지를 검증한다. 하지만 service 내부 allowlist에 seed에는 없는 항목이 추가되는 경우를 완전하게 검증하지 못한다.

통합하면 drift 위험은 줄지만 production code 구조가 바뀐다.

통합하지 않으면 현재 안정성은 유지되지만 drift 방지는 테스트에 의존한다.

## D. Candidate Options

### Candidate 1. 현 상태 유지

내용:

- `DocumentTypeSeeds`와 `FileNamePolicyService`를 통합하지 않는다.
- black-box consistency test만 유지한다.
- allowlist accessor는 추가하지 않는다.

장점:

- production code 변경이 없다.
- 현재 build/test PASS 기준을 유지한다.
- 구현 안정성이 높다.
- 범위가 가장 작다.

단점:

- service 내부 allowlist에 seed에는 없는 type이 추가되는 문제를 완전하게 잡지 못한다.
- 기준이 이중화된다.
- 장기 drift 위험이 남는다.

### Candidate 2. allowlist accessor만 추가

내용:

- `FileNamePolicyService`에 scope별 allowlist 조회 accessor를 추가한다.
- `DocumentTypeSeeds`와 accessor 결과를 비교하는 full equality test를 추가한다.
- 파일명 생성 로직 자체는 변경하지 않는다.

장점:

- full equality test가 가능하다.
- drift 감지력이 강해진다.
- production behavior 변경은 작다.

단점:

- production code에 테스트 검증 목적의 accessor가 생긴다.
- allowlist를 public API로 노출할지 설계 판단이 필요하다.
- accessor가 또 하나의 유지보수 대상이 될 수 있다.

### Candidate 3. `FileNamePolicyService`가 `DocumentTypeSeeds`를 직접 사용

내용:

- `FileNamePolicyService` 내부 documentType allowlist를 제거하거나 축소한다.
- `DocumentTypeSeeds`를 기준으로 scope/code 검증을 수행한다.

장점:

- 기준이 단일화된다.
- drift 위험이 줄어든다.
- seed label/scope/sortOrder 기준과 allowlist 기준이 직접 연결된다.

단점:

- production code 변경 범위가 커진다.
- `FileNamePolicyService`가 storage model namespace에 의존할 수 있다.
- storage seed metadata와 file name policy 책임이 섞일 위험이 있다.
- 순수 파일명 정책 서비스가 category seed 구조를 알게 될 수 있다.
- 구조 의존성이 부적절하면 장기 유지보수에 불리하다.

### Candidate 4. 별도 `DocumentTypePolicy` 또는 `DocumentTypeCatalog` 도입

내용:

- `DocumentTypeSeeds`와 `FileNamePolicyService`가 모두 별도 policy/catalog 기준을 참조한다.
- seed label/sortOrder와 validation code set을 분리하거나 공통화한다.

장점:

- 책임 분리가 가능하다.
- 단일 기준을 만들면서도 `FileNamePolicyService`가 UI seed model에 직접 의존하지 않게 할 수 있다.
- 장기 구조가 가장 깨끗할 수 있다.

단점:

- 새로운 abstraction이 추가된다.
- 지금 MVP 단계에서는 과설계 가능성이 있다.
- 추가 문서와 테스트가 필요하다.

## E. Recommended Direction

Candidate Recommendation:

- 지금 바로 Candidate 3으로 가지 않는다.
- `FileNamePolicyService`가 `DocumentTypeSeeds`를 직접 참조하는 구조는 책임 혼합 위험이 있다.
- 다음 단계에서는 Candidate 2 또는 Candidate 4 중 선택을 검토한다.
- MVP 1차에서는 Candidate 2, 즉 allowlist accessor + full equality test 방향이 가장 현실적이다.
- 다만 accessor를 production public API로 노출하는 것이 부담이면 Candidate 4의 `DocumentTypePolicy` / `DocumentTypeCatalog` 도입을 별도 설계로 검토한다.
- 현재 단계에서는 통합 구현을 하지 않고, 사용자 결정 문서를 먼저 만든다.

이 추천은 확정이 아니라 `Candidate Recommendation`이다.

## F. Design Considerations

### Responsibility 기준

- `FileNamePolicyService`의 주 책임은 physical file name 생성이다.
- `DocumentTypeSeeds`의 주 책임은 fixed seed metadata 제공이다.
- UI label/sortOrder는 `FileNamePolicyService`의 관심사가 아니다.
- documentType validation 기준은 양쪽에서 일치해야 한다.

### Dependency 기준

검토할 dependency 방향:

- `FileNamePolicyService -> DocumentTypeSeeds`
- `DocumentTypeSeeds -> FileNamePolicyService`
- 둘 다 별도 `DocumentTypePolicy` 또는 `DocumentTypeCatalog` 참조
- 계속 분리하고 test로만 보호

현재 구조에서 가장 보수적인 방향은 계속 분리하고 test로 보호하는 것이다. drift 방지 강도를 높이려면 accessor 또는 별도 catalog가 필요하다.

### Test 기준

- 현재 black-box test는 유지한다.
- full equality test를 하려면 accessor 또는 공통 catalog가 필요하다.
- New Candidate 정책은 계속 별도 결정으로 둔다.

## G. Needs Decision

1. 현 상태 유지가 아니라 full equality 검증을 강화할 것인가?
2. 다음 단계에서 allowlist accessor 추가 후보로 갈 것인가?
3. `FileNamePolicyService`가 `DocumentTypeSeeds`를 직접 참조하는 방식은 보류할 것인가?
4. 별도 `DocumentTypePolicy` / `DocumentTypeCatalog` 도입은 MVP 이후 또는 별도 설계로 보류할 것인가?
5. MVP 1차에서는 allowlist accessor + full equality test가 적절한가?
6. accessor를 추가한다면 production behavior는 변경하지 않을 것인가?
7. accessor는 scope별 current allowlist code set만 노출할 것인가?
8. New Candidate `statement`, `prescription`, claim `capture`는 계속 제외할 것인가?
9. full equality test는 accessor 추가 후 별도 작업으로 구현할 것인가?
10. JSON storage implementation은 계속 제외할 것인가?
11. 이번 단계에서는 구현 없이 통합 여부 결정 문서까지만 진행할 것인가?

## H. Out of Scope

이번 문서에서 제외하는 범위는 다음과 같다.

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

## I. Risks

- 현 상태 유지 시 drift 위험은 완전하게 제거되지 않는다.
- accessor 추가 시 production API가 늘어난다.
- `FileNamePolicyService`가 seed model을 직접 참조하면 책임이 섞일 수 있다.
- 별도 catalog 도입은 구조적으로 깨끗하지만 MVP에서는 과설계일 수 있다.
- New Candidate 정책이 바뀌면 seed/allowlist/test를 모두 갱신해야 한다.
- full equality test를 도입하면 기존 black-box test와 역할 중복이 생길 수 있다.

## J. Recommendation

1. 이 문서를 기준으로 seed constant와 `FileNamePolicyService` 통합 여부 결정을 받는다.
2. 사용자 결정 후 `docs/80_DOCUMENT_TYPE_SEED_POLICY_SERVICE_INTEGRATION_USER_DECISION_RECORD.md`를 생성한다.
3. 그 다음 별도 작업으로 allowlist accessor 또는 catalog 설계를 진행한다.
4. 그 다음 full equality consistency test를 설계/구현한다.
5. JSON storage implementation은 아직 진행하지 않는다.

## K. Result

`DOCUMENT_TYPE_SEED_POLICY_SERVICE_INTEGRATION_DECISION_DRAFTED`
