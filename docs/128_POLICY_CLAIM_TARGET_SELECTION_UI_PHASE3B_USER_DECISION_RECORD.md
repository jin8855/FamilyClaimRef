# Policy / Claim Target Selection UI Phase 3B User Decision Record

## A. Status Marker

POLICY_CLAIM_TARGET_SELECTION_UI_PHASE3B_USER_DECISION_RECORDED

## B. Decision Context

- Phase 1에서 `Policy` / `Claim` storage가 추가되었다.
- Phase 2에서 `DocumentLinkCoordinator` active target validation이 추가되었다.
- `docs/126_MAINWINDOW_TARGET_SELECTION_UI_SCOPE_DESIGN.md`에서 Option B, active `Policy` / `Claim` dropdown이 추천되었다.
- `docs/127_POLICY_CLAIM_TARGET_SELECTION_UI_PHASE3B_IMPLEMENTATION_PLAN.md`에서 Phase 3B implementation plan이 작성되었다.
- 이 문서는 Phase 3B minimal implementation 전에 사용자 결정 사항을 고정하기 위한 기록이다.

## C. Confirmed Decisions

### Decision 1: UI Method

Confirmed:

- Phase 3B는 Option B, active `Policy` / `Claim` dropdown 방식으로 진행한다.

Rejected:

- Option C, quick create inside registration은 Phase 3B에서 제외한다.

Reason:

- 실제 사용자 UX에는 dropdown 방식이 더 적합하다.
- quick create는 policy/claim CRUD, validation, rollback, UI state가 함께 얽혀 범위가 과도하게 커진다.

### Decision 2: Direct Target Id Input

Confirmed:

- Direct target id input is not the primary user flow.

Implementation rule:

- Prefer dropdown-only user flow.
- If removing direct input causes broad refactoring, keep it only as a clearly secondary/dev fallback and report the reason.

Reason:

- 기존 direct `TargetKind` / `TargetId` 입력은 사용자가 id를 알고 있어야 하므로 실제 사용자 흐름에 부적합하다.
- 다만 구현 시점에 기존 테스트 또는 binding 영향이 크면 dev fallback으로 임시 유지할 수 있다.

### Decision 3: Active List Load Timing

Confirmed:

- async constructor는 사용하지 않는다.
- `LoadTargetOptionsAsync` 후보 흐름을 우선 사용한다.
- UI lifecycle에서 controlled 방식으로 호출하는 구현 후보를 둔다.
- registration command 직전 lazy load만으로 처리하지 않는다.

Reason:

- 사용자는 등록 버튼을 누르기 전에 선택 가능한 target을 볼 수 있어야 한다.
- constructor에서 async 작업을 수행하면 초기화 경계와 오류 처리가 불명확해질 수 있다.

### Decision 4: Register Button Policy

Confirmed:

- Phase 3B 최소 기준은 validation message로 registration을 차단하는 방식이다.
- register button disable은 기존 binding/command 구조에서 무리 없이 가능할 때만 추가한다.
- `DocumentLinkCoordinator` validation은 그대로 유지한다.

Reason:

- command refactoring을 넓히지 않고 최소 UX를 확보한다.
- UI validation은 coordinator validation을 대체하지 않는다.

### Decision 5: Service API

Confirmed:

- 기존 `GetPoliciesAsync` / `GetClaimsAsync`가 active-only이므로 Phase 3B 초기 구현에는 충분한 것으로 본다.
- service API 확장은 implementation 중 concrete gap이 발견될 때만 최소 범위로 허용한다.
- repository abstraction, DB/SQLite 구현은 금지한다.

Reason:

- `JsonPolicyClaimStorageService.GetPoliciesAsync`와 `GetClaimsAsync`는 `DisabledAt is null` 기준으로 active record만 반환한다.
- Phase 3B의 dropdown 요구에는 active-only list가 핵심이다.

### Decision 6: Display Label

Confirmed:

- 실제 보험사명, 병원명, 진단명, 진단코드, 실제 계약 번호, 실제 청구 번호는 사용하지 않는다.
- 초기 display label은 available model field 기반으로 synthetic/internal-safe 값만 사용한다.
- display label이 부족하면 후속 UX hardening으로 둔다.

Allowed examples:

- `policy_demo_001`
- `claim_demo_001`
- `document_demo_001`

Reason:

- MVP 초기 단계에서는 개인정보와 실제 보험/의료 정보를 UI 검토 샘플에 포함하지 않는 것이 우선이다.
- 구분성이 부족한 label은 추후 policy/claim management UX와 함께 개선한다.

### Decision 7: Empty State

Confirmed:

- active policy가 없으면 policy dropdown 영역에 empty state message를 표시한다.
- active claim이 없으면 claim dropdown 영역에 empty state message를 표시한다.
- Phase 3B에서는 create button 또는 quick create link를 추가하지 않는다.
- empty state에서는 다음 행동 안내를 최소 문장으로 제한한다.

Candidate messages:

- `No active policy is available for selection.`
- `No active claim is available for selection.`

Reason:

- policy/claim 생성 UX는 Phase 3B 범위가 아니다.
- empty state에서 create action을 제공하면 Phase 3C 범위로 확장된다.

### Decision 8: Expected Phase 3B Implementation Files

Confirmed expected files:

- `app/FamilyClaimRef.App/ViewModels/DocumentRegistrationViewModel.cs`
- `app/FamilyClaimRef.App/MainWindow.xaml`
- `tests/FamilyClaimRef.App.Tests/DocumentRegistrationViewModelTests.cs`

Conditional files:

- `app/FamilyClaimRef.App/Composition/AppServices.cs`
  - ViewModel constructor injection이 필요한 경우에만 수정 허용
- `app/FamilyClaimRef.App/Services/Storage/IPolicyClaimStorageService.cs`
  - concrete API gap이 있을 경우에만 수정 허용
- `app/FamilyClaimRef.App/Services/Storage/JsonPolicyClaimStorageService.cs`
  - concrete API gap이 있을 경우에만 수정 허용
- `tests/FamilyClaimRef.App.Tests/JsonPolicyClaimStorageServiceTests.cs`
  - service API 변경 시에만 수정 허용

Expected unchanged files:

- `DocumentLinkCoordinator.cs`
- `DocumentRegistrationWorkflow.cs`
- `MainWindow.xaml.cs`, 가능하면 수정 금지

Reason:

- Phase 3B의 핵심은 ViewModel binding과 XAML target selection이다.
- link validation과 registration workflow는 Phase 2에서 이미 책임 경계가 정리되었다.

## D. Explicit Non-Scope

- policy 생성 UI 없음
- claim 생성 UI 없음
- policy 수정 UI 없음
- claim 수정 UI 없음
- policy disable UI 없음
- claim disable UI 없음
- quick create 없음
- seed data 구현 없음
- DB/SQLite/OCR/repository 구현 없음
- runtime manual validation 없음
- app launch 없음
- OpenFileDialog 실행 없음
- actual registration workflow 실행 없음
- commit 없음

## E. Implementation Guardrails for Next Phase

Phase 3B implementation instruction 작성 시 다음 guardrail을 적용한다.

- active dropdown 구현을 기본 방향으로 둔다.
- disabled policy/claim은 UI에 노출하지 않는다.
- final workflow contract는 target kind/id를 유지한다.
- `DocumentLinkCoordinator` validation은 유지한다.
- UI validation은 coordinator validation을 대체하지 않는다.
- synthetic id만 사용한다.
- 실제 개인정보, 보험, 병원, 진단 샘플을 사용하지 않는다.
- app launch, OpenFileDialog, runtime workflow는 실행하지 않는다.
- build/test는 구현 작업 후 필수로 수행한다.

## F. Risks Accepted

Accepted risks:

- active policy/claim이 없으면 사용자는 문서 등록을 완료할 수 없다.
- policy/claim creation UX가 없으므로 실제 MVP 사용성은 아직 제한된다.
- display label이 internal id 중심이면 사용자 구분성이 낮을 수 있다.
- direct id fallback을 숨기면 개발 검증 편의성이 줄어든다.
- direct id fallback을 남기면 UX 혼란 가능성이 있다.
- runtime validation은 별도 Phase에서 확인해야 한다.

Risk handling:

- active target이 없는 문제는 Phase 3C policy/claim creation/management UX에서 다룬다.
- display label 문제는 후속 UX hardening에서 다룬다.
- runtime validation은 Phase 3D에서 별도 승인 후 수행한다.

## G. Next Recommendation

다음 추천 작업:

- Phase 3B minimal implementation instruction 작성

해당 구현 지시서에는 다음 제한을 포함한다.

- exact implementation scope
- allowed files
- conditional files
- forbidden files
- tests required
- build/test required
- app launch/OpenFileDialog/runtime workflow 금지
- commit 금지

Documentation-only note:

- 이 문서는 구현 전 사용자 결정 기록이다.
- build/test는 documentation-only change이므로 실행하지 않는다.
