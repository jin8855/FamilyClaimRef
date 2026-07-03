# Policy / Claim Creation Management Phase 3C User Decision Record

## A. Status Marker

POLICY_CLAIM_CREATION_MANAGEMENT_PHASE3C_USER_DECISION_RECORDED

## B. Decision Context

Phase 1에서 `Policy` / `Claim` JSON storage가 추가되었다.

Phase 2에서 `DocumentLinkCoordinator` active target validation이 추가되었다.

Phase 3B에서 document registration target dropdown이 추가되었다.

`docs/131_POLICY_CLAIM_CREATION_MANAGEMENT_UX_SCOPE_DESIGN.md`에서 policy/claim creation/management UX scope가 정리되었다.

현재 문제는 active policy/claim을 사용자가 만들고 관리하는 UX가 없다는 점이다. Document registration 화면은 active target을 선택할 수 있지만, 비어 있는 active list를 채울 수는 없다.

이 문서는 Phase 3C 구현 전에 사용자 결정을 고정하기 위한 기록이다.

## C. Confirmed Decisions

### Decision 1: UX Architecture

Confirmed:

- Phase 3C 초기 MVP는 Option A를 따른다.
- `MainWindow` 안에 별도 `Policy/Claim Management` section을 추가하는 방식으로 진행한다.

Rejected:

- Option C, document registration 안의 quick create는 계속 제외한다.

Deferred:

- Option B, separate `Policy/Claim Management Window`는 후속 구조 후보로 둔다.

Reason:

- 현재 앱은 single-window MVP 구조다.
- 별도 window/dialog lifecycle은 구현과 테스트 범위를 넓힌다.
- 다만 document registration 영역과 management 영역은 시각적으로 분리해야 한다.

### Decision 2: ViewModel Structure

Confirmed:

- 장기적으로는 별도 `PolicyClaimManagementViewModel` 방향이 맞다.
- Phase 3C에서는 가능하면 `DocumentRegistrationViewModel`에 management 책임을 추가하지 않고, 별도 `PolicyClaimManagementViewModel` 후보를 우선 검토한다.

Implementation rule:

- `MainWindow` 안에 표시되더라도 registration ViewModel과 management ViewModel 책임은 분리하는 방향을 우선한다.
- 이 composition 또는 binding 영향이 과도하면 Codex가 이유를 보고하고 최소 대안을 제시한다.

### Decision 3: Policy Minimum Scope

Confirmed Phase 3C MVP policy scope:

- active policy list 표시
- policy 생성
- policy display title 입력
- policy internal id를 synthetic-safe local id로 자동 생성
- selected policy disable
- disabled policy는 document registration dropdown에서 사라져야 함
- policy edit는 Phase 3C MVP에서 제외하고 후속 후보로 둠

Rejected:

- 실제 보험계약 번호 입력
- 실제 보험사명 입력
- 실제 가족 실명 입력

Reason:

- MVP 목적은 target 생성과 선택 가능한 상태 확보다.
- 실제 보험 도메인 필드는 개인정보/민감정보 위험과 UX 범위를 늘린다.

### Decision 4: Claim Minimum Scope

Confirmed Phase 3C MVP claim scope:

- active claim list 표시
- claim 생성
- claim 생성 시 active policy 선택 필수
- claim display title 입력
- claim internal id를 synthetic-safe local id로 자동 생성
- selected claim disable
- disabled claim은 document registration dropdown에서 사라져야 함
- claim edit는 Phase 3C MVP에서 제외하고 후속 후보로 둠

Rejected:

- 실제 청구 번호 입력
- 실제 병원명 입력
- 실제 진단명 입력
- 실제 진단코드 입력

Reason:

- claim은 policy에 종속되어야 한다.
- MVP 목적은 실제 보험 청구 모델 완성이 아니라 document target 생성 흐름 완성이다.

### Decision 5: Policy Disable Relationship Policy

Confirmed:

- active claim이 있는 policy disable은 Phase 3C MVP에서 block한다.

Rejected:

- active claim이 있어도 policy만 disable
- confirmation 없이 active claims cascade disable

Reason:

- active claim이 남아 있는 상태에서 parent policy만 disable하면 관계 정합성이 흔들린다.
- cascade disable은 위험하고 confirmation UX가 필요하다.
- MVP에서는 block이 가장 단순하고 안전하다.

### Decision 6: Claim Disable Policy

Confirmed:

- claim disable은 claim만 disable한다.
- linked document files는 삭제하지 않는다.
- existing document link metadata는 삭제하지 않는다.
- disabled claim은 신규 document registration target으로 선택할 수 없다.
- 기존 link/history 표시 정책은 후속 Phase로 둔다.

### Decision 7: Document Link Impact Policy

Confirmed:

- policy/claim disable은 기존 document file을 삭제하지 않는다.
- policy/claim disable은 기존 document link metadata를 삭제하지 않는다.
- disabled policy/claim은 신규 document registration target으로 선택할 수 없다.
- 기존 link 조회/표시 정책은 별도 Phase로 둔다.
- Phase 3C는 document link history viewer가 아니다.

### Decision 8: Id Generation

Confirmed:

- Phase 3C MVP에서는 policy/claim internal id를 자동 생성한다.
- 사용자가 실제 보험계약 번호 또는 실제 청구 번호를 id로 입력하지 않는다.
- id는 synthetic-safe local internal id로 둔다.

Candidate pattern:

- `policy_<timestamp-or-guid-short>`
- `claim_<timestamp-or-guid-short>`

Implementation rule:

- 기존 `JsonPolicyClaimStorageService` 또는 Phase 1 id generation rule이 있다면 그 규칙을 우선 재사용한다.
- 새 id generation rule이 필요하면 implementation plan에서 최소 범위로 정리한다.

### Decision 9: Display Title

Confirmed:

- policy/claim display title은 사용자가 입력할 수 있다.
- 샘플 또는 테스트 placeholder에는 실제 보험사명, 병원명, 진단명, 계약번호, 청구번호를 사용하지 않는다.
- placeholder는 synthetic-safe 문구만 사용한다.

Allowed examples:

- `policy_title_demo`
- `claim_title_demo`

### Decision 10: MainWindow Layout Boundary

Confirmed:

- `MainWindow`에 management section을 추가하되 document registration section과 분리한다.
- document registration target selection 영역 안에 quick create button/link를 넣지 않는다.
- management section에서 생성/disable 후 document registration dropdown은 reload 또는 refresh로 반영되는 흐름을 설계한다.

## D. Explicit Non-Scope

Phase 3C MVP에서 제외할 항목:

- policy edit 없음
- claim edit 없음
- actual insurer field 없음
- actual hospital field 없음
- diagnosis field 없음
- diagnosis code field 없음
- real family member field 없음
- quick create 없음
- OCR 기반 자동 생성 없음
- 보험사 API 연동 없음
- 병원/진료 데이터 연동 없음
- document link history viewer 없음
- DB/SQLite/repository 구현 없음
- seed data 구현 없음
- runtime manual validation 없음
- app launch 없음
- OpenFileDialog 실행 없음
- commit 없음

## E. Implementation Guardrails for Next Phase

Phase 3C implementation plan 작성 시 적용할 guardrail:

- `MainWindow`에 별도 management section 추가
- document registration과 management 책임 분리
- 가능하면 별도 `PolicyClaimManagementViewModel` 사용
- 기존 `IPolicyClaimStorageService` 사용
- service API 확장은 concrete gap이 있을 때만 허용
- `DocumentLinkCoordinator` 수정 금지 후보
- `DocumentRegistrationWorkflow` 수정 금지 후보
- quick create 금지
- 실제 개인정보/보험/병원/진단 샘플 금지
- disabled target은 신규 registration target으로 노출하지 않음
- active claim이 있는 policy disable은 block
- disable은 file/link metadata를 삭제하지 않음
- unit test 중심 검증
- app launch/OpenFileDialog/runtime workflow 금지

## F. Risks Accepted

Accepted risks:

- `MainWindow`가 다소 복잡해질 수 있다.
- 실제 보험계약/청구번호를 배제하므로 사용자가 record를 구분하기 어려울 수 있다.
- display title만으로는 실제 도메인 식별성이 부족할 수 있다.
- policy/claim edit을 빼서 초기 사용성이 제한된다.
- active claim이 있는 policy disable block으로 사용자가 먼저 claim을 disable해야 한다.
- runtime validation은 별도 Phase가 필요하다.

## G. Next Recommendation

다음 추천 작업:

`Policy/Claim Creation Management Phase 3C implementation plan` 문서 생성.

구현 지시서로 바로 가지 않는다. 먼저 implementation plan에서 다음 항목을 확정한다.

- expected modified files
- ViewModel composition 방식
- `MainWindow` section layout
- refresh/reload flow
- policy creation validation
- claim creation validation
- policy disable block implementation
- test scope
- build/test/safety verification plan
