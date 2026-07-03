# Policy / Claim Creation Management Phase 3C Implementation Review

## A. Status Marker

POLICY_CLAIM_CREATION_MANAGEMENT_PHASE3C_IMPLEMENTED

## B. Implementation Scope

Phase 3C minimal implementation 범위로 다음 항목을 반영했다.

- `MainWindowViewModel` wrapper 추가
- `PolicyClaimManagementViewModel` 추가
- `MainWindow`에 document registration과 분리된 `Policy/Claim Management` section 추가
- policy create 구현
- policy disable 구현
- active claim이 있는 policy disable block 구현
- active policy 기반 claim create 구현
- claim disable 구현
- management action 성공 후 document registration target dropdown reload 구현
- unit test 보강

## C. Composition Decision

Option A, `MainWindowViewModel` wrapper를 사용했다.

구성:

- `MainWindowViewModel.DocumentRegistration`
- `MainWindowViewModel.PolicyClaimManagement`

`AppServices`는 기존 `JsonPolicyClaimStorageService` instance를 재사용해 두 ViewModel에 연결한다.

Fallback은 사용하지 않았다.

## D. Service API Decision

기존 `IPolicyClaimStorageService` API로 충분했다.

사용한 기존 API:

- `GetPoliciesAsync`
- `GetClaimsAsync`
- `GetClaimsByPolicyIdAsync`
- `AddPolicyAsync`
- `AddClaimAsync`
- `DisablePolicyAsync`
- `DisableClaimAsync`

Service API 변경 없음.

`JsonPolicyClaimStorageService` 변경 없음.

Repository, DB, SQLite abstraction 추가 없음.

## E. MainWindow.xaml.cs Change

`MainWindow.xaml.cs`는 lifecycle hook과 button bridge만 유지한다.

변경 내용:

- `Window_Loaded`에서 `MainWindowViewModel.LoadAsync` 호출
- source file selection은 `MainWindowViewModel.SelectFileAsync`로 위임
- document registration은 `MainWindowViewModel.RegisterAsync`로 위임
- policy/claim create/disable button bridge 추가

Code-behind에 policy/claim creation logic 없음.

Code-behind에 disable block logic 없음.

Code-behind에 storage 직접 접근 없음.

## F. MainWindow.xaml Change

`MainWindow.DataContext`는 `MainWindowViewModel`을 기준으로 변경했다.

기존 registration 영역은 `DocumentRegistration` child context로 분리했다.

추가된 management 영역은 `PolicyClaimManagement` child context로 분리했다.

`Target selection` 영역 안에는 quick create button/link를 추가하지 않았다.

Hardcoded seed data 없음.

Real insurer/hospital/diagnosis/policy number/claim number placeholder 없음.

## G. PolicyClaimManagementViewModel Behavior

구현된 behavior:

- active policy list load
- active claim list load
- policy display title trim validation
- empty policy title block
- policy create
- selected policy disable
- active claim이 있는 selected policy disable block
- claim display title trim validation
- empty claim title block
- active policy selection 없이 claim create block
- active policy 기반 claim create
- selected claim disable
- create/disable 성공 후 management list reload

Message는 synthetic-safe generic text만 사용한다.

## H. Refresh / Reload Flow

`MainWindowViewModel`이 refresh coordination을 담당한다.

흐름:

- `LoadAsync`
  - `DocumentRegistrationViewModel.LoadTargetOptionsAsync`
  - `PolicyClaimManagementViewModel.LoadAsync`
- `CreatePolicyAsync` 성공
  - management list reload
  - registration target dropdown reload
- `CreateClaimAsync` 성공
  - management list reload
  - registration target dropdown reload
- `DisableSelectedPolicyAsync` 성공
  - management list reload
  - registration target dropdown reload
- `DisableSelectedClaimAsync` 성공
  - management list reload
  - registration target dropdown reload

실패 또는 validation block 시 registration dropdown reload는 수행하지 않는다.

## I. Test Coverage

추가 테스트 파일:

- `tests/FamilyClaimRef.App.Tests/PolicyClaimManagementViewModelTests.cs`

보강된 테스트 범위:

- constructor null storage guard
- `LoadAsync` active policy/claim load
- policy create success
- empty policy title block
- policy disable success
- active claim 존재 시 policy disable block
- selected active policy 없이 claim create block
- claim create success
- empty claim title block
- claim disable success
- disabled policy/claim reload 후 목록 제외
- management action 후 registration target reload
- project root `attachments/`, `data/local` pollution 없음

기존 테스트:

- `DocumentRegistrationViewModelTests` 계속 통과
- `JsonPolicyClaimStorageServiceTests` 계속 통과

## J. Explicit Non-Scope

- policy edit 없음
- claim edit 없음
- quick create 없음
- actual insurer field 없음
- actual hospital field 없음
- diagnosis field 없음
- diagnosis code field 없음
- real family member field 없음
- seed data 없음
- DB 구현 없음
- SQLite 구현 없음
- OCR 구현 없음
- repository 구현 없음
- app launch 없음
- `OpenFileDialog` 실행 없음
- runtime registration workflow manual execution 없음

## K. Safety Review

실제 개인정보 샘플 없음.

실제 가족 실명 없음.

실제 보험사명 없음.

실제 병원명 없음.

실제 진단명 없음.

실제 진단코드 없음.

실제 보험계약 번호 없음.

실제 청구 번호 없음.

Document file 삭제 구현 없음.

Document link metadata 삭제 구현 없음.

Project root `attachments/` 내부 파일 생성 없음.

Project root `data/local` 내부 파일 생성 없음.

## L. Verification Results

`git diff --check`:

```text
PASS
```

일반 `dotnet build FamilyClaimRef.sln`은 Windows SDK 경로 접근 권한 문제로 실패했다.

권한 상승 재실행 결과:

```text
dotnet build FamilyClaimRef.sln: PASS
warning: 0
error: 0
```

권한 상승 테스트 결과:

```text
dotnet test FamilyClaimRef.sln: PASS
total tests: 271
failed tests: 0
skipped tests: 0
```

권한 상승 사유:

- `C:\Users\jin8855\AppData\Local\Microsoft SDKs` 접근 권한 문제

Actual app launch:

- not run

Actual `OpenFileDialog`:

- not run

Runtime registration workflow manual execution:

- not run

Project root `attachments/`:

```text
files=0
```

Project root `data/local`:

```text
files=0
```

DB/SQLite unexpected file:

```text
none
```

Actual personal sample:

```text
none
```

Synthetic-safe sample values used in tests:

- `policy_title_demo`
- `claim_title_demo`
- `policy_demo_001`
- `claim_demo_001`

## M. Modified Files

수정 파일:

- `app/FamilyClaimRef.App/App.xaml.cs`
- `app/FamilyClaimRef.App/Composition/AppServices.cs`
- `app/FamilyClaimRef.App/MainWindow.xaml`
- `app/FamilyClaimRef.App/MainWindow.xaml.cs`

생성 파일:

- `app/FamilyClaimRef.App/ViewModels/MainWindowViewModel.cs`
- `app/FamilyClaimRef.App/ViewModels/PolicyClaimManagementViewModel.cs`
- `tests/FamilyClaimRef.App.Tests/PolicyClaimManagementViewModelTests.cs`
- `docs/134_POLICY_CLAIM_CREATION_MANAGEMENT_PHASE3C_IMPLEMENTATION_REVIEW.md`

## N. Unchanged Guarded Files

- `app/FamilyClaimRef.App/Services/Storage/DocumentLinkCoordinator.cs`
- `app/FamilyClaimRef.App/Services/Storage/DocumentRegistrationWorkflow.cs`
- `app/FamilyClaimRef.App/Services/Storage/IPolicyClaimStorageService.cs`
- `app/FamilyClaimRef.App/Services/Storage/JsonPolicyClaimStorageService.cs`
- Phase 1 storage model files
- `docs/131_POLICY_CLAIM_CREATION_MANAGEMENT_UX_SCOPE_DESIGN.md`
- `docs/132_POLICY_CLAIM_CREATION_MANAGEMENT_PHASE3C_USER_DECISION_RECORD.md`
- `docs/133_POLICY_CLAIM_CREATION_MANAGEMENT_PHASE3C_IMPLEMENTATION_PLAN.md`

## O. Remaining Risks / Follow-up

- `MainWindow`가 더 커졌으므로 후속 UI 정리가 필요할 수 있다.
- policy/claim edit은 후속으로 남아 있다.
- display label hardening은 후속으로 남아 있다.
- runtime manual validation은 Phase 3D에서 별도로 수행해야 한다.
- Phase 3C commit candidate review가 필요하다.
