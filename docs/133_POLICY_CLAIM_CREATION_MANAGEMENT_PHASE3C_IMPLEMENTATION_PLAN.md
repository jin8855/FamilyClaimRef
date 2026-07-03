# Policy / Claim Creation Management Phase 3C Implementation Plan

## A. Status Marker

POLICY_CLAIM_CREATION_MANAGEMENT_PHASE3C_IMPLEMENTATION_PLAN_CREATED

## B. Background

Phase 1에서 `Policy` / `Claim` JSON storage가 추가되었다.

Phase 2에서 `DocumentLinkCoordinator` active target validation이 추가되었다.

Phase 3B에서 document registration target dropdown이 추가되었다.

`docs/131_POLICY_CLAIM_CREATION_MANAGEMENT_UX_SCOPE_DESIGN.md`에서 creation/management UX scope가 정리되었다.

`docs/132_POLICY_CLAIM_CREATION_MANAGEMENT_PHASE3C_USER_DECISION_RECORD.md`에서 Phase 3C 사용자 결정이 고정되었다.

현재 gap은 active policy/claim 생성 및 관리 UX가 없다는 점이다.

## C. Implementation Goal

Phase 3C minimal implementation 목표:

- `MainWindow` 안에 document registration과 분리된 `Policy/Claim Management` section 추가
- active policy list 표시
- policy 생성
- selected policy disable
- active claim list 표시
- active policy 기반 claim 생성
- selected claim disable
- active claim이 있는 policy disable block
- 생성/disable 후 document registration target dropdown reload 또는 refresh
- 기존 document file/link metadata 삭제 없음
- unit test 보강

## D. Explicit Non-Scope

Phase 3C minimal implementation에 포함하지 않는다.

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
- `OpenFileDialog` 실행 없음
- commit 없음

## E. Expected Modified Files

Phase 3C minimal implementation 예상 생성 파일:

- `app/FamilyClaimRef.App/ViewModels/MainWindowViewModel.cs`
- `app/FamilyClaimRef.App/ViewModels/PolicyClaimManagementViewModel.cs`
- `tests/FamilyClaimRef.App.Tests/PolicyClaimManagementViewModelTests.cs`
- `docs/134_POLICY_CLAIM_CREATION_MANAGEMENT_PHASE3C_IMPLEMENTATION_REVIEW.md`

Phase 3C minimal implementation 예상 수정 파일:

- `app/FamilyClaimRef.App/Composition/AppServices.cs`
- `app/FamilyClaimRef.App/MainWindow.xaml`
- `app/FamilyClaimRef.App/MainWindow.xaml.cs`, only if lifecycle refresh/load hook is required

조건부 수정 파일:

- `app/FamilyClaimRef.App/ViewModels/DocumentRegistrationViewModel.cs`
  - registration dropdown refresh coordination이 필요한 경우만 수정한다.
- `app/FamilyClaimRef.App/Services/Storage/IPolicyClaimStorageService.cs`
  - service API concrete gap이 있을 때만 수정한다.
- `app/FamilyClaimRef.App/Services/Storage/JsonPolicyClaimStorageService.cs`
  - service API concrete gap이 있을 때만 수정한다.
- `tests/FamilyClaimRef.App.Tests/JsonPolicyClaimStorageServiceTests.cs`
  - service API 변경 시에만 수정한다.

예상 unchanged files:

- `app/FamilyClaimRef.App/Services/Storage/DocumentLinkCoordinator.cs`
- `app/FamilyClaimRef.App/Services/Storage/DocumentRegistrationWorkflow.cs`
- Phase 1 storage models, concrete gap 없으면 수정 금지
- `docs/131_POLICY_CLAIM_CREATION_MANAGEMENT_UX_SCOPE_DESIGN.md`
- `docs/132_POLICY_CLAIM_CREATION_MANAGEMENT_PHASE3C_USER_DECISION_RECORD.md`

## F. ViewModel Composition Plan

### Option A: MainWindowViewModel Wrapper 추가

설명:

- `MainWindow.DataContext`를 새 `MainWindowViewModel`로 변경한다.
- `MainWindowViewModel`이 `DocumentRegistrationViewModel`과 `PolicyClaimManagementViewModel`을 포함한다.

장점:

- registration과 management 책임이 분리된다.
- XAML binding boundary가 명확하다.
- 후속 separate window 분리에도 유리하다.

단점:

- `MainWindow.xaml` binding path 변경 범위가 커질 수 있다.
- 기존 tests 일부 조정 가능성이 있다.

### Option B: DocumentRegistrationViewModel에 PolicyClaimManagementViewModel Child Property 추가

장점:

- `DataContext` 변경을 최소화할 수 있다.
- `MainWindow.xaml` binding 변경이 작을 수 있다.

단점:

- `DocumentRegistrationViewModel`에 management 책임이 얹힌다.
- `docs/132`의 책임 분리 결정과 약하게 충돌한다.

### Option C: MainWindow Code-behind에서 두 ViewModel 직접 연결

장점:

- 빠른 구현이 가능하다.

단점:

- code-behind 책임이 증가한다.
- 테스트가 어려워진다.
- MVVM 경계가 약해진다.

Recommended:

- Option A, `MainWindowViewModel` wrapper를 우선 검토한다.
- 구현 영향이 과도하면 Option B를 fallback으로 검토하고, 이유를 implementation review에 기록한다.
- Option C는 피한다.

## G. PolicyClaimManagementViewModel Plan

후보 ViewModel:

- `PolicyClaimManagementViewModel`

Dependency:

- `IPolicyClaimStorageService`

Candidate properties:

- `AvailablePolicies`
- `AvailableClaims`
- `SelectedPolicyId`
- `SelectedClaimId`
- `SelectedPolicyForClaimId`
- `NewPolicyDisplayTitle`
- `NewClaimDisplayTitle`
- `ManagementMessage`
- `HasAvailablePolicies`
- `HasAvailableClaims`
- `CanCreatePolicy`
- `CanCreateClaim`
- `CanDisablePolicy`
- `CanDisableClaim`

Candidate methods/commands:

- `LoadAsync`
- `CreatePolicyAsync`
- `CreateClaimAsync`
- `DisableSelectedPolicyAsync`
- `DisableSelectedClaimAsync`

Implementation rules:

- async constructor 금지
- service locator 금지
- static access 금지
- optional null fallback 금지
- 실제 개인정보/보험/병원/진단 샘플 금지
- policy/claim id는 service 또는 existing storage rule에 위임
- display title trim validation 필요
- empty title이면 create 차단
- claim 생성 시 selected active policy 필수
- active claim이 있는 policy disable은 block
- disable은 document file/link metadata를 삭제하지 않음

## H. Refresh / Coordination Plan

Phase 3B의 `DocumentRegistrationViewModel` target dropdown과 Phase 3C management section을 동기화해야 한다.

후보:

1. management create/disable 후 `DocumentRegistrationViewModel.LoadTargetOptionsAsync` 재호출
2. `MainWindowViewModel`에서 `RefreshAllAsync` 조정
3. `PolicyClaimManagementViewModel`이 callback/event를 받아 registration VM refresh
4. code-behind에서 직접 refresh 연결

Recommended:

- `MainWindowViewModel` wrapper를 사용한다면 `RefreshAllAsync` 또는 equivalent coordination method를 둔다.
- management action 성공 후 registration target options reload가 필요하다.
- code-behind 직접 연결은 피한다.
- `MainWindow.xaml.cs` 수정이 필요하면 `Loaded` 시점 initial load 정도로 제한한다.

Expected flow:

- Window Loaded
  - `MainWindowViewModel.LoadAsync`
  - `DocumentRegistrationViewModel.LoadTargetOptionsAsync`
  - `PolicyClaimManagementViewModel.LoadAsync`
- Create policy success
  - management list reload
  - registration target options reload
- Disable policy success
  - management list reload
  - registration target options reload
- Create claim success
  - management list reload
  - registration target options reload
- Disable claim success
  - management list reload
  - registration target options reload

## I. MainWindow Layout Plan

MainWindow sections:

1. Document Registration
2. Target Selection
3. Policy/Claim Management

Policy Management section:

- active policy list
- new policy display title input
- create policy button
- disable selected policy button
- message area

Claim Management section:

- active policy selector for claim creation
- active claim list
- new claim display title input
- create claim button
- disable selected claim button
- message area

Layout rules:

- document registration target selection 안에 quick create button/link 금지
- management section은 별도 heading으로 분리
- 실제 보험/병원/진단 관련 placeholder 금지
- placeholder는 synthetic-safe only
- `MainWindow`가 과도하게 커지는 위험은 implementation review에 기록

## J. Policy Creation Validation Plan

- display title required
- trim 후 empty이면 create 차단
- duplicate title 허용 여부는 Phase 3C에서 엄격히 막지 않는다.
- id는 internal synthetic-safe auto id
- 실제 보험계약 번호 입력 없음
- 실제 보험사명 입력 없음
- 생성 성공 후 active policy list와 registration dropdown refresh

## K. Claim Creation Validation Plan

- display title required
- active policy selection required
- trim 후 empty이면 create 차단
- parent policy는 active policy여야 함
- id는 internal synthetic-safe auto id
- 실제 청구 번호 입력 없음
- 실제 병원명 입력 없음
- 실제 진단명/진단코드 입력 없음
- 생성 성공 후 active claim list와 registration dropdown refresh

## L. Policy Disable Block Plan

Confirmed policy:

- active claim이 있는 policy disable은 block한다.

Implementation candidate:

- selected policy id 기준으로 `GetClaimsByPolicyIdAsync` 호출
- active claim count가 1개 이상이면 disable하지 않고 `ManagementMessage` 표시
- active claim이 없으면 `DisablePolicyAsync` 호출
- linked document metadata/file은 건드리지 않음

주의:

- `DocumentLinkCoordinator` 수정 금지
- document link store 수정 금지
- file 삭제 금지

## M. Claim Disable Plan

- selected claim이 없으면 validation message 표시
- selected claim이 있으면 `DisableClaimAsync` 호출
- linked document metadata/file은 건드리지 않음
- 성공 후 active claim list와 registration dropdown refresh

## N. Service API Review

현재 service API 후보:

- `GetPoliciesAsync`
- `GetClaimsAsync`
- `GetClaimsByPolicyIdAsync`
- `AddPolicyAsync`
- `AddClaimAsync`
- `DisablePolicyAsync`
- `DisableClaimAsync`

판정 후보:

- API sufficient라면 service 수정 없이 진행한다.
- API gap이 있으면 최소 extension만 plan 또는 review에 기록한다.

주의:

- repository abstraction 추가 금지
- DB/SQLite 금지
- broad storage redesign 금지

## O. Test Plan

Phase 3C implementation 시 필요한 test scope:

`PolicyClaimManagementViewModelTests` 후보:

1. `LoadAsync` loads active policies and claims
2. `CreatePolicyAsync` with title adds active policy
3. `CreatePolicyAsync` with empty title is blocked
4. `DisableSelectedPolicyAsync` disables policy when no active claims
5. `DisableSelectedPolicyAsync` blocks when active claims exist
6. `CreateClaimAsync` requires selected active policy
7. `CreateClaimAsync` with title adds active claim
8. `CreateClaimAsync` with empty title is blocked
9. `DisableSelectedClaimAsync` disables claim
10. Disabled policy/claim disappear after reload
11. Management action requests or triggers registration target reload
12. No document file is created
13. No root `attachments/` or `data/local/` pollution

Existing tests:

- `DocumentRegistrationViewModelTests` must continue passing
- `JsonPolicyClaimStorageServiceTests` must continue passing

Test data:

- synthetic-safe only
- `policy_title_demo`
- `claim_title_demo`
- `policy_demo_001`
- `claim_demo_001`

Forbidden test data:

- actual insurer name
- actual hospital name
- actual diagnosis name
- actual diagnosis code
- actual policy number
- actual claim number
- actual family name

## P. Verification Plan for Phase 3C Implementation

구현 후 필수 검증:

- `git diff --check`
- `dotnet build FamilyClaimRef.sln`
- `dotnet test FamilyClaimRef.sln`
- project root `attachments/` files count
- project root `data/local/` files count
- DB/SQLite unexpected file check
- actual personal sample check
- `git status --short`

금지:

- app launch
- `OpenFileDialog`
- runtime manual registration workflow

주의:

- Windows SDK 경로 권한 문제가 있으면 권한 상승 build/test 가능
- 권한 상승이 필요하면 `docs/134` implementation review에 기록한다.

## Q. Risks

- `MainWindowViewModel` wrapper 도입 시 binding 변경 범위가 커질 수 있다.
- `MainWindow`가 비대해질 수 있다.
- management section 추가로 UX가 복잡해질 수 있다.
- display title만으로는 실제 record 구분성이 부족할 수 있다.
- policy/claim edit이 없어 수정 실수 대응이 약하다.
- active claim block 정책으로 사용자는 먼저 claim을 disable해야 한다.
- refresh coordination 누락 시 registration dropdown이 stale 상태가 될 수 있다.
- runtime validation은 별도 Phase가 필요하다.

## R. Recommended Phase 3C Direction

추천:

- `MainWindow` 안 별도 management section
- 별도 `PolicyClaimManagementViewModel` 생성
- 가능하면 `MainWindowViewModel` wrapper로 registration VM과 management VM 조정
- 기존 `IPolicyClaimStorageService` 사용
- service API 확장은 concrete gap이 있을 때만 허용
- policy/claim edit 제외
- quick create 제외
- active claim이 있는 policy disable block
- implementation 후 `docs/134_POLICY_CLAIM_CREATION_MANAGEMENT_PHASE3C_IMPLEMENTATION_REVIEW.md` 생성

## S. Next Recommendation

다음 추천 작업:

`Policy/Claim Creation Management Phase 3C minimal implementation instruction` 작성.

이 문서에서 승인한 범위만 구현 지시서에 포함한다.
