# Document Type Seed Constant Implementation Decision

## A. Goal

이 문서는 document type seed constant 구현 여부와 범위를 결정하기 위한 문서이다.

목적은 `FileNamePolicyService` allowlist와 CategoryItem fixed seed 기준을 일치시키기 위한 준비 기준을 정리하는 것이다.

이 문서는 구현 문서가 아니다. C# constant/model/test 구현, CategoryItem 구현, `FileNamePolicyService` 수정, allowlist/seed consistency test 구현은 수행하지 않는다.

## B. Current State

- `FileNamePolicyService` 구현 및 테스트는 완료되어 있다.
- C# storage model/interface 1차 구현은 완료되어 있다.
- 최근 검토 기준 build/test는 PASS 상태이다.
- 현재 테스트 수는 33개이다.
- document type 관리는 Hybrid fixed seed CategoryItem 방향으로 결정되어 있다.
- 사용자 정의 document type 추가/수정/삭제는 MVP 1차에서 제외한다.
- 저장 document type 값은 label이 아니라 code이다.
- label은 화면 표시용이다.
- claim/policy scope allowlist는 분리되어 있다.
- CategoryItem seed 후보 필드는 다음으로 정리되어 있다.
  - `code`
  - `label`
  - `scope`
  - `sortOrder`
  - `disabledAt`
- CategoryItem JSON 저장 구현은 보류되어 있다.
- `FileNamePolicyService` allowlist와 CategoryItem seed 기준은 반드시 일치해야 한다.
- 현재 코드 allowlist에 없는 type은 `New Candidate / Needs Implementation`으로 분리되어 있다.
- documentType source of truth는 도메인 연결 record이다.
  - `PolicyDocumentRecord.DocumentType`
  - `ClaimDocumentRecord.DocumentType`
- `DocumentRecord.DocumentType`은 제외되어 있다.
- JSON storage implementation은 아직 없다.
- CategoryItem 구현은 아직 없다.
- document type seed constant 구현은 아직 없다.
- allowlist/seed consistency test는 아직 없다.

## C. Problem Statement

현재 document type allowlist는 `FileNamePolicyService` 내부 기준이다.

CategoryItem fixed seed 기준은 문서로만 존재한다. 두 기준이 코드상으로 분리되어 유지되면 drift가 발생할 수 있다.

UI label, sortOrder, scope를 제공하려면 seed constant가 필요할 수 있다. 다만 seed constant를 너무 빨리 넓게 구현하면 CategoryItem storage, service, validation까지 범위가 커질 위험이 있다.

따라서 다음 단계에서는 seed constant 구현 여부와 최소 구현 범위를 먼저 결정해야 한다.

## D. Candidate Options

### Candidate 1. 아직 seed constant 구현 안 함

내용:

- 계속 `FileNamePolicyService` 내부 allowlist만 유지한다.
- CategoryItem seed는 문서 기준으로만 유지한다.

장점:

- 구현 범위가 늘어나지 않는다.
- 현재 테스트 33개를 그대로 유지하기 쉽다.
- MVP 속도가 빠르다.

단점:

- allowlist와 seed 문서 간 drift 가능성이 있다.
- UI label/sortOrder 제공 기준이 코드에 없다.
- 후속 ViewModel/UI 단계에서 다시 정리해야 한다.

### Candidate 2. document type seed constant만 구현

내용:

- C# static class 또는 record 기반 fixed seed를 정의한다.
- claim/policy document type code, label, scope, sortOrder, disabledAt 후보를 포함한다.
- CategoryItem JSON 저장 구현은 하지 않는다.
- `FileNamePolicyService` 수정은 아직 하지 않거나 별도 단계로 보류한다.

장점:

- UI label/sortOrder 기준을 코드로 제공할 수 있다.
- allowlist/seed consistency test 준비가 가능하다.
- CategoryItem JSON 저장 없이도 fixed seed를 사용할 수 있다.

단점:

- `FileNamePolicyService` allowlist와 중복 기준이 생길 수 있다.
- consistency test 없이는 여전히 drift 위험이 있다.
- seed constant 구현 후 allowlist 통합 여부를 별도로 결정해야 한다.

### Candidate 3. seed constant 구현 + `FileNamePolicyService` allowlist 통합

내용:

- document type seed constant를 만든다.
- `FileNamePolicyService`가 seed 기준을 사용하도록 변경한다.
- allowlist/seed consistency test를 추가한다.

장점:

- drift 위험을 최소화할 수 있다.
- allowlist 기준이 단일화된다.
- 테스트로 보호할 수 있다.

단점:

- production code 수정 범위가 커진다.
- 기존 테스트 수정/확장이 필요하다.
- 이번 단계로는 범위가 크다.
- CategoryItem service 구현 전 dependency 구조를 다시 설계해야 할 수 있다.

## E. Recommended Direction

Candidate Recommendation:

- 바로 Candidate 3으로 가지 않는다.
- 다음 단계는 Candidate 2인 **document type seed constant만 구현**이 적절하다.
- 이때 `FileNamePolicyService` 수정은 하지 않는다.
- seed constant 구현 후 별도 단계에서 allowlist/seed consistency test를 만든다.
- consistency test가 PASS한 뒤 `FileNamePolicyService` allowlist 통합 여부를 검토한다.
- CategoryItem JSON 저장 구현은 계속 보류한다.
- 사용자 정의 document type 기능은 MVP 1차에서 제외한다.

## F. Seed Constant Type Candidate

예시 후보:

- `DocumentTypeSeed`
- `DocumentTypeSeeds`
- `DocumentTypeScope`

### `DocumentTypeSeed`

Field 후보:

- `string Code`
- `string Label`
- `string Scope`
- `int SortOrder`
- `DateTimeOffset? DisabledAt`

### `DocumentTypeSeeds`

역할 후보:

- claim document type seed 목록 제공
- policy document type seed 목록 제공
- 전체 seed 목록 제공
- scope/code validation 후보 제공

### `DocumentTypeScope`

후보:

- `Claim`
- `Policy`

주의:

- enum을 쓸지 string constant를 쓸지 결정이 필요하다.
- 기존 `FileNamePolicyService`는 string scope를 사용하므로 과도한 enum 도입은 주의한다.
- seed constant 구현 시 JSON storage는 하지 않는다.

## G. Current Seed Candidate

### Claim seed 후보

| Code | Label | Scope | SortOrder | Status |
|---|---|---|---:|---|
| `receipt` | 영수증 | `claim` | 10 | Current Allowlist |
| `diagnosis` | 진단서 | `claim` | 20 | Current Allowlist |
| `medicine` | 약제비 서류 | `claim` | 30 | Current Allowlist |
| `visit` | 통원 확인 서류 | `claim` | 40 | Current Allowlist |
| `admission` | 입퇴원 확인 서류 | `claim` | 50 | Current Allowlist |
| `surgery` | 수술 확인 서류 | `claim` | 60 | Current Allowlist |
| `etc` | 기타 | `claim` | 999 | Current Allowlist |

### Policy seed 후보

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

- New Candidate는 seed constant 구현 범위에 포함하지 않는 것을 우선 추천한다.
- New Candidate를 포함하려면 `FileNamePolicyService` patch와 테스트 갱신이 먼저 필요하다.

## H. Test Scope Candidate

후속 테스트 후보:

- seed 목록에 중복 code/scope 조합이 없는지 검증
- claim seed code set과 `FileNamePolicyService` claim allowlist가 일치하는지 검증
- policy seed code set과 `FileNamePolicyService` policy allowlist가 일치하는지 검증
- label이 비어 있지 않은지 검증
- sortOrder 중복 또는 정렬 기준 검증 후보
- disabledAt null 기준 검증 후보

주의:

- 이 문서에서는 테스트를 구현하지 않는다.
- consistency test는 seed constant 구현 후 별도 작업으로 진행한다.

## I. Needs Decision

1. 다음 구현은 document type seed constant만 구현할 것인가?
2. `FileNamePolicyService`는 다음 구현에서 수정하지 않을 것인가?
3. CategoryItem JSON 저장 구현은 계속 보류할 것인가?
4. 사용자 정의 document type 추가/수정/삭제는 계속 MVP 1차에서 제외할 것인가?
5. seed constant에는 current allowlist만 포함할 것인가?
6. `statement`, `prescription`, claim `capture`는 seed constant에서 제외하고 `New Candidate / Needs Implementation`으로 유지할 것인가?
7. seed field는 `Code`, `Label`, `Scope`, `SortOrder`, `DisabledAt`으로 둘 것인가?
8. scope는 기존 string 기준인 `claim`, `policy`를 유지할 것인가?
9. `DocumentTypeSeed`, `DocumentTypeSeeds` 같은 static seed 구조로 갈 것인가?
10. seed constant 구현 후 `dotnet build`와 `dotnet test`를 실행할 것인가?
11. allowlist/seed consistency test는 seed constant 구현 후 별도 작업으로 진행할 것인가?
12. JSON storage implementation은 계속 별도 승인 전까지 제외할 것인가?

## J. Out of Scope

- C# constant 구현 없음
- C# model 수정 없음
- C# interface 수정 없음
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

## K. Risks

- seed constant만 만들고 consistency test가 없으면 drift 위험이 남는다.
- `FileNamePolicyService` allowlist와 seed constant가 중복 기준이 될 수 있다.
- scope를 enum으로 바꾸면 기존 string 기반 API와 마찰이 생길 수 있다.
- New Candidate를 seed에 포함하면 현재 allowlist와 충돌한다.
- label 표현이 화면 UX와 맞지 않을 수 있다.
- sortOrder 기준은 이후 UI에서 변경될 수 있다.

## L. Recommendation

1. 이 문서를 기준으로 document type seed constant 구현 여부를 결정받는다.
2. 사용자 결정 후 `docs/74_DOCUMENT_TYPE_SEED_CONSTANT_USER_DECISION_RECORD.md`를 생성한다.
3. 그 다음 별도 승인으로 document type seed constant를 구현한다.
4. 구현 후 `dotnet build FamilyClaimRef.sln`, `dotnet test FamilyClaimRef.sln`을 실행한다.
5. 그 다음 allowlist/seed consistency test 설계를 진행한다.

## M. Result

`DOCUMENT_TYPE_SEED_CONSTANT_IMPLEMENTATION_DECISION_DRAFTED`
