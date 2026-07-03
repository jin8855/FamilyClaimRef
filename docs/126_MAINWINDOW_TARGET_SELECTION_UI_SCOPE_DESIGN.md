# MainWindow Target Selection UI Scope Design

## A. Status Marker

MAINWINDOW_TARGET_SELECTION_UI_SCOPE_DESIGNED

## B. Background

`FamilyClaimRef`는 문서 파일을 로컬에 보관하고, 문서 metadata를 JSON storage에 기록한 뒤, 해당 문서를 `Policy` 또는 `Claim`에 연결하는 흐름을 단계적으로 구현하고 있다.

Phase 1에서는 `Policy` / `Claim` storage가 추가되었고, Phase 2에서는 `DocumentLinkCoordinator`가 target 존재 여부와 사용 중지 여부를 검증하도록 보강되었다.

이 문서는 `MainWindow`의 document registration 화면에서 사용자가 어떤 `Policy` 또는 `Claim`에 문서를 연결할지 선택하는 UI 범위를 정리한다. 이번 작업은 설계 문서 작성만 수행하며, UI 구현은 포함하지 않는다.

## C. Current State

- Phase 1에서 `Policy` / `Claim` storage가 추가되었다.
- Phase 2에서 `DocumentLinkCoordinator` target existence validation이 추가되었다.
- `AppServices` runtime composition은 `JsonDocumentStorageService`, `JsonPolicyClaimStorageService`, `DocumentLinkCoordinator`, `DocumentRegistrationWorkflow`, `DocumentRegistrationViewModel`을 연결한다.
- `MainWindow`에는 현재 `TargetKind`와 `TargetId`를 직접 입력하는 최소 target 영역이 있다.
- 하지만 실제 사용자 기준의 target selection UI는 아직 없다.
- 사용자는 현재 어떤 active `Policy` 또는 active `Claim`을 선택해야 하는지 화면에서 알기 어렵다.
- 현재 document registration target selection flow는 사용자 친화적인 방식으로 정의되어 있지 않다.

## D. Problem Definition

- 문서 등록 시 사용자는 target type과 target id를 선택해야 한다.
- target이 없거나 disabled 상태이면 Phase 2 validation에서 link가 실패한다.
- UI가 active target selection을 제공하지 않으면 runtime 사용 흐름이 완성되지 않는다.
- 사용자가 id를 직접 입력하는 방식은 검증 흐름 확인에는 유효하지만, 실제 사용 UX로는 부족하다.
- policy/claim creation UX와 document registration UX를 한 번에 구현하면 범위가 과도하게 커진다.

## E. Design Constraints

- 이번 문서는 scope design만 작성한다.
- code, XAML, ViewModel, AppServices, coordinator, workflow, tests를 수정하지 않는다.
- target selection UI는 Phase 2의 coordinator validation을 대체하지 않는다.
- UI는 active-only target을 보여주는 방향을 우선 검토한다.
- disabled `Policy` / `Claim`은 선택 목록에 표시하지 않는 방향을 기본값으로 둔다.
- 실제 개인정보, 실제 보험 계약 번호, 실제 청구 번호, 실제 보험사명, 실제 병원명, 실제 진단명, 실제 진단코드 사례를 사용하지 않는다.
- 샘플 id가 필요하면 `policy_demo_001`, `claim_demo_001` 같은 synthetic id만 사용한다.

## F. Explicit Non-Scope

- UI 구현 없음
- XAML 수정 없음
- ViewModel 수정 없음
- AppServices 수정 없음
- DocumentLinkCoordinator 수정 없음
- DocumentRegistrationWorkflow 수정 없음
- tests 수정 없음
- app launch 없음
- OpenFileDialog 실행 없음
- registration workflow 실제 실행 없음
- policy/claim CRUD 구현 없음
- seed data 구현 없음
- DB/SQLite/OCR/repository 구현 없음
- Git add/commit/reset/checkout/clean 없음

## G. UI Options

### Option A: Direct Target Id Input

`TargetKind`와 `TargetId`를 사용자가 직접 입력하거나 선택하는 최소 방식이다.

장점:

- 구현 범위가 가장 작다.
- storage list UI가 없어도 validation flow를 확인할 수 있다.
- Phase 2의 target existence validation을 빠르게 확인하기 좋다.
- 현재 `MainWindow`와 `DocumentRegistrationViewModel` 구조에서 가장 가까운 방식이다.

단점:

- 실제 사용자 UX가 약하다.
- 사용자가 target id를 알고 있어야 한다.
- 오입력 가능성이 높다.
- missing/disabled target 오류가 사용자의 입력 실수인지 실제 데이터 문제인지 구분하기 어렵다.

판정:

- 임시 또는 dev fallback 후보로만 둔다.
- 실제 MVP 사용자 흐름의 기본 방향으로는 부적합하다.

### Option B: Active Policy/Claim Dropdown

active `Policy` / active `Claim` 목록을 로드하고, 사용자가 dropdown에서 연결 대상을 선택하는 방식이다.

장점:

- 실제 UX에 가장 가깝다.
- missing/disabled target 선택 가능성을 줄인다.
- Phase 2 validation과 자연스럽게 연결된다.
- 사용자에게 현재 연결 가능한 target을 명확히 보여줄 수 있다.
- disabled target을 UI 목록에서 제외할 수 있다.

단점:

- ViewModel에서 active policies/claims list load가 필요하다.
- empty state 처리가 필요하다.
- policy/claim creation UX가 없으면 목록이 비어 있을 수 있다.
- list API가 부족하면 Phase 3B에서 service method 추가 여부를 별도로 결정해야 한다.

판정:

- Phase 3B 구현 우선 후보로 둔다.
- 단, policy/claim creation UX가 아직 없기 때문에 empty state와 dev data 준비 정책을 함께 결정해야 한다.

### Option C: Quick Create Inside Registration

document registration 화면 안에서 `Policy` 또는 `Claim`을 바로 생성하고, 생성된 target에 문서를 연결하는 통합 방식이다.

장점:

- 사용자의 흐름은 가장 통합적이다.
- target이 없을 때 화면을 이동하지 않고 바로 생성할 수 있다.

단점:

- 범위가 과도하게 커진다.
- policy/claim storage CRUD, validation, UI state, rollback, link workflow가 한 화면에 얽힌다.
- document registration 실패와 target creation 실패의 책임 경계가 복잡해진다.
- 현재 Phase에서는 구현 리스크가 크다.

판정:

- 이번 단계에서는 reject한다.
- policy/claim creation/management UX는 Phase 3C에서 별도 설계와 구현 대상으로 둔다.

## H. Recommended Direction

Phase 3B의 기본 방향은 Option B: Active Policy/Claim Dropdown으로 둔다.

근거:

- Phase 2에서 이미 active target existence validation이 coordinator에 들어갔다.
- UI도 active target만 선택하게 하면 사용자의 실패 가능성을 줄일 수 있다.
- 최종 방어선은 `DocumentLinkCoordinator`에 유지하면서, UI는 더 나은 선택 경험을 제공하는 역할로 분리할 수 있다.
- MVP에서 사용자에게 id 직접 입력을 요구하는 방식은 유지보수와 검토 모두에 불리하다.

보조 판단:

- Option A는 임시 fallback 또는 개발 검증용으로만 유지할 수 있다.
- Option C는 이번 단계에서 제외한다.
- policy/claim creation UX가 없어서 active list가 비어 있을 수 있으므로, Phase 3B 전에 empty state와 테스트 데이터 준비 기준을 명확히 해야 한다.

## I. ViewModel Impact Candidates

아래 항목은 구현 확정이 아니라 Candidate이다. 최종 property/command 이름은 기존 `DocumentRegistrationViewModel` naming convention을 기준으로 Phase 3B implementation plan에서 확정한다.

Candidate properties:

- `SelectedTargetType`
- `SelectedPolicyId`
- `SelectedClaimId`
- `AvailablePolicies`
- `AvailableClaims`
- `TargetSelectionErrorMessage`
- `IsTargetSelectionRequired`

Candidate commands / flows:

- LoadTargets command 또는 initialization flow
- target type 변경 시 target list display 전환
- register command validation dependency
- selected target이 없을 때 registration 차단 또는 validation message 표시

Existing relationship:

- 현재 `TargetKind`와 `TargetId`는 registration request로 전달된다.
- Phase 3B에서는 dropdown selection 결과가 최종적으로 `TargetKind` / `TargetId` equivalent 값으로 workflow에 전달되어야 한다.
- 기존 입력형 target 구조를 완전히 제거할지, dev fallback으로 남길지는 Phase 3B에서 결정한다.

## J. XAML Impact Candidates

아래 항목은 XAML 구현 후보이며 이번 문서에서는 수정하지 않는다.

Candidate UI 영역:

- Document registration section 안에 Target section 추가 또는 기존 Target section 재구성
- target type 선택 control
- policy dropdown
- claim dropdown
- empty state message
- target selection validation message

Display policy:

- disabled target은 목록에서 제외한다.
- target create button은 이번 phase에서 제외한다.
- `Policy` 또는 `Claim` 목록이 비어 있으면 사용자가 등록을 진행할 수 없는 이유를 짧게 표시한다.

Candidate synthetic display:

- `policy_demo_001`
- `claim_demo_001`

위 값은 실제 개인정보나 실제 계약/청구 번호가 아니라 문서용 synthetic id 예시이다.

## K. Storage / Service Interaction

- UI는 active-only target 목록만 보여주는 방향이 원칙이다.
- disabled `Policy` / `Claim`은 target selection UI에 표시하지 않는다.
- 최종 방어선은 계속 `DocumentLinkCoordinator` validation이다.
- UI validation은 coordinator validation을 대체하지 않는다.
- coordinator validation은 제거하지 않는다.
- `IPolicyClaimStorageService`의 기존 list API가 UI에 충분한지 Phase 3B에서 확인한다.
- list API가 부족하면 service method 추가 여부를 Phase 3B implementation plan에서 별도로 결정한다.
- DB/SQLite/repository 구현은 여전히 금지 범위다.

## L. Empty State Policy

No active policy:

- policy target 선택이 불가능하다는 message를 표시한다.
- 예: `선택 가능한 active policy가 없습니다.`
- policy create button은 이번 phase에 포함하지 않는다.

No active claim:

- claim target 선택이 불가능하다는 message를 표시한다.
- 예: `선택 가능한 active claim이 없습니다.`
- claim create button은 이번 phase에 포함하지 않는다.

Registration button policy:

- target list가 비어 있을 때 register button을 disable할지, validation message로만 막을지는 Phase 3B에서 결정한다.
- 어떤 방식을 선택하더라도 `DocumentLinkCoordinator` validation은 유지한다.

## M. Privacy / Sample Data Policy

문서와 향후 UI 검토에서는 다음 값을 사용하지 않는다.

- 실제 가족 실명
- 실제 보험 계약 번호
- 실제 청구 번호
- 실제 보험사명
- 실제 병원명
- 실제 진단명
- 실제 진단코드 기반 개인 사례

허용되는 예시는 synthetic id만 사용한다.

- `policy_demo_001`
- `claim_demo_001`

실제 사용자 파일, 실제 문서 metadata, 실제 OCR 결과, 실제 진단 관련 값은 이 단계의 샘플로 사용하지 않는다.

## N. Phase Split Recommendation

Phase 3A: MainWindow target selection UI scope only

- target type 선택 방식 결정
- target id 선택/입력 방식 결정
- ViewModel property/command 영향 분석
- user-facing validation message 기준 설계
- 구현 없음

Phase 3B: ViewModel + XAML minimal target selection implementation

- active `Policy` / `Claim` 목록 표시 또는 승인된 fallback 방식 반영
- document registration command와 target type/id 연결
- empty state message 반영
- 필요한 경우 tests 보강
- app launch와 OpenFileDialog runtime 검증은 별도 승인 후 수행

Phase 3C: policy/claim creation/management UX

- policy 생성/수정/disable
- claim 생성/수정/disable
- claim이 policy에 종속되는 UX 정리
- document registration과 분리된 관리 흐름 설계

Phase 3D: runtime manual validation

- 실제 앱 실행
- OpenFileDialog
- registration workflow 수동 검증
- `%LOCALAPPDATA%` runtime file 생성 여부와 cleanup 정책 확인

## O. Risks

- active list가 비어 있으면 Option B UI만으로는 사용자가 다음 행동을 알기 어렵다.
- policy/claim creation UX가 없기 때문에 Phase 3B에서 empty state가 주요 UX 문제가 될 수 있다.
- UI가 active-only list를 제공하더라도 data race 또는 외부 변경으로 target이 disabled될 수 있으므로 coordinator validation은 반드시 유지해야 한다.
- 기존 direct `TargetId` 입력 구조와 새 dropdown 구조가 함께 존재하면 UX가 혼란스러울 수 있다.
- target display label 정책이 없으면 사용자가 어떤 `Policy` 또는 `Claim`인지 구분하기 어렵다.
- 실제 runtime validation은 이 문서 범위 밖이므로, Phase 3D에서 별도 확인이 필요하다.

## P. Next Recommendation

다음 작업은 바로 구현이 아니라 `Policy/Claim Target Selection UI Phase 3B implementation plan` 문서 생성이다.

해당 문서에서 먼저 결정할 항목:

- Option B를 구현 기준으로 확정할지 여부
- direct id input fallback 유지 여부
- active policies/claims load timing
- empty state에서 register button disable 여부
- target display label 후보
- 필요한 ViewModel property/command 최종 이름
- 필요한 test scope

Phase 3B implementation plan 승인 후에만 ViewModel + XAML 최소 target selection 구현으로 넘어간다.
