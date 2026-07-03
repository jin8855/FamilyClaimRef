# Policy / Claim Creation Management Phase 3C Commit Candidate Review

## A. Status Marker

POLICY_CLAIM_CREATION_MANAGEMENT_PHASE3C_COMMIT_CANDIDATE_READY

## B. Review Target

검토 대상 파일:

- `app/FamilyClaimRef.App/App.xaml.cs`
- `app/FamilyClaimRef.App/Composition/AppServices.cs`
- `app/FamilyClaimRef.App/MainWindow.xaml`
- `app/FamilyClaimRef.App/MainWindow.xaml.cs`
- `app/FamilyClaimRef.App/ViewModels/MainWindowViewModel.cs`
- `app/FamilyClaimRef.App/ViewModels/PolicyClaimManagementViewModel.cs`
- `tests/FamilyClaimRef.App.Tests/PolicyClaimManagementViewModelTests.cs`
- `docs/131_POLICY_CLAIM_CREATION_MANAGEMENT_UX_SCOPE_DESIGN.md`
- `docs/132_POLICY_CLAIM_CREATION_MANAGEMENT_PHASE3C_USER_DECISION_RECORD.md`
- `docs/133_POLICY_CLAIM_CREATION_MANAGEMENT_PHASE3C_IMPLEMENTATION_PLAN.md`
- `docs/134_POLICY_CLAIM_CREATION_MANAGEMENT_PHASE3C_IMPLEMENTATION_REVIEW.md`

Review 생성 문서:

- `docs/135_POLICY_CLAIM_CREATION_MANAGEMENT_PHASE3C_COMMIT_CANDIDATE_REVIEW.md`

## C. Scope Review

판정:

```text
PASS
```

확인 내용:

- Phase 3C 범위인 Policy/Claim Management UI와 ViewModel만 추가되었다.
- `MainWindow` 안에 document registration과 분리된 `Policy/Claim Management` section이 추가되었다.
- document registration target selection 영역 안에 quick create button/link는 없다.
- policy edit / claim edit은 구현되지 않았다.
- actual insurer/hospital/diagnosis domain fields는 추가되지 않았다.
- seed data 구현 없음.
- DB/SQLite/OCR/repository 구현 없음.
- `DocumentLinkCoordinator` 수정 없음.
- `DocumentRegistrationWorkflow` 수정 없음.
- `docs/131`~`docs/134`는 기존 상태를 유지한다.

## D. Implementation Review

### App.xaml.cs

판정:

```text
PASS_WITH_NOTES
```

확인 내용:

- `AppServices.CreateDefault()` 호출 흐름은 유지된다.
- `MainWindow.DataContext`가 `services.MainWindowViewModel`로 연결된다.
- 변경은 `MainWindowViewModel` wrapper 도입에 필요한 최소 DataContext 연결이다.
- app launch, `OpenFileDialog`, registration workflow 실행 로직은 추가되지 않았다.
- policy/claim business logic은 들어가지 않았다.
- storage 직접 접근은 들어가지 않았다.

비고:

- 원래 Phase 3C 구현 허용 파일 목록에 명시된 파일은 아니었으나, wrapper DataContext 연결을 위해 필요한 최소 startup wiring으로 판단된다.

### MainWindowViewModel

판정:

```text
PASS
```

확인 내용:

- `DocumentRegistrationViewModel`과 `PolicyClaimManagementViewModel`을 child property로 보유한다.
- registration 책임과 management 책임은 각 child ViewModel에 남아 있다.
- `LoadAsync`가 registration target options와 management list load를 조정한다.
- management action 성공 시 registration dropdown reload가 수행된다.
- code-behind가 두 child ViewModel을 직접 조정하지 않는다.

### PolicyClaimManagementViewModel

판정:

```text
PASS
```

확인 내용:

- `IPolicyClaimStorageService` dependency는 mandatory다.
- async constructor 없음.
- service locator 없음.
- static access 없음.
- optional null fallback 없음.
- policy create는 display title required validation을 수행한다.
- claim create는 active policy selection과 display title required validation을 수행한다.
- selected policy disable은 active claim 존재 시 block한다.
- selected claim disable은 claim만 disable한다.
- disable 시 document file/link metadata 삭제 로직은 없다.
- message/test data는 synthetic-safe generic text만 사용한다.

### AppServices

판정:

```text
PASS
```

확인 내용:

- `MainWindowViewModel`, `DocumentRegistrationViewModel`, `PolicyClaimManagementViewModel` composition이 명확하다.
- 기존 `JsonPolicyClaimStorageService` instance를 registration과 management ViewModel이 공유한다.
- storage service instance를 중복 생성하지 않는다.
- metadata root/path 일관성을 유지한다.
- broad composition redesign은 없다.

### MainWindow.xaml

판정:

```text
PASS_WITH_NOTES
```

확인 내용:

- document registration section과 `Policy/Claim Management` section이 child DataContext로 분리되어 있다.
- document registration target selection 영역 안에 quick create button/link는 없다.
- policy management UI는 active list, title input, create, disable을 포함한다.
- claim management UI는 active policy selector, active claim list, title input, create, disable을 포함한다.
- seed data hardcoding 없음.
- 실제 보험사명, 병원명, 진단명, 계약번호, 청구번호 placeholder 없음.

비고:

- 기존 document type selector에는 `diagnosis` document type code가 남아 있다. 이는 기존 문서 유형 코드이며 실제 진단명/진단코드 샘플은 아니다.
- `MainWindow`가 커졌으므로 후속 UI 분리가 필요할 수 있다.

### MainWindow.xaml.cs

판정:

```text
PASS
```

확인 내용:

- `Loaded` 시점에 `MainWindowViewModel.LoadAsync`를 호출한다.
- 기존 `SelectFileButton_Click` / `RegisterButton_Click` bridge 흐름은 유지된다.
- policy/claim create/disable button bridge가 추가되었다.
- code-behind에 policy/claim creation logic 없음.
- code-behind에 disable block logic 없음.
- storage 직접 접근 없음.
- `OpenFileDialog` 실행 추가 없음.

### Service API

판정:

```text
PASS
```

확인 내용:

- `IPolicyClaimStorageService` 수정 없음.
- `JsonPolicyClaimStorageService` 수정 없음.
- 기존 API로 Phase 3C 구현이 충분했다.
- repository abstraction 추가 없음.
- DB/SQLite 추가 없음.

### Refresh / Coordination

판정:

```text
PASS
```

확인 내용:

- `MainWindowViewModel`이 management action 성공 후 `DocumentRegistrationViewModel.LoadTargetOptionsAsync`를 다시 호출한다.
- validation block이나 실패 시 불필요한 registration reload는 수행하지 않는다.
- stale dropdown 위험을 줄이는 최소 coordination이 구현되어 있다.

### Disable Relationship Policy

판정:

```text
PASS
```

확인 내용:

- active claim이 있는 policy disable은 `GetClaimsByPolicyIdAsync`로 확인 후 block한다.
- claim disable은 claim만 disable한다.
- document file 삭제 없음.
- document link metadata 삭제 없음.

## E. Test Review

판정:

```text
PASS
```

보강된 테스트:

- `PolicyClaimManagementViewModelTests`

커버 범위:

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

전체 테스트 결과:

```text
dotnet test FamilyClaimRef.sln: PASS
total tests: 271
failed tests: 0
skipped tests: 0
```

## F. Safety Review

판정:

```text
PASS
```

확인 내용:

- 실제 개인정보 샘플 없음.
- 실제 가족 실명 없음.
- 실제 보험계약 번호 없음.
- 실제 청구 번호 없음.
- 실제 보험사명 없음.
- 실제 병원명 없음.
- 실제 진단명/진단코드 샘플 없음.
- DB/SQLite unexpected file 없음.
- project root `attachments/` files=0.
- project root `data/local` files=0.
- app launch 없음.
- `OpenFileDialog` 실행 없음.
- registration workflow 실제 수동 실행 없음.
- document file/link metadata deletion 없음.

## G. Verification Results

`git diff --check`:

```text
PASS
```

일반 `dotnet build FamilyClaimRef.sln`:

```text
FAIL
reason: Access to the path 'C:\Users\jin8855\AppData\Local\Microsoft SDKs' is denied.
```

권한 상승 `dotnet build FamilyClaimRef.sln`:

```text
PASS
warning: 0
error: 0
```

권한 상승 `dotnet test FamilyClaimRef.sln`:

```text
PASS
total tests: 271
failed tests: 0
skipped tests: 0
```

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

## H. Git Status Summary

`docs/135` 생성 전 status는 Phase 3C expected files와 `docs/131`~`docs/134`만 포함했다.

`docs/135` 생성 후 expected additional file:

- `docs/135_POLICY_CLAIM_CREATION_MANAGEMENT_PHASE3C_COMMIT_CANDIDATE_REVIEW.md`

## I. Commit Readiness

commit readiness:

```text
ready
```

reason:

- Scope matches Phase 3C decision and implementation plan.
- Build and tests pass.
- Safety checks pass.
- No guarded storage/workflow files were modified.
- No DB/SQLite/OCR/repository implementation was introduced.
- App.xaml.cs change is a minimal wrapper DataContext wiring and contains no business logic.

## J. Commit Candidate Exact File List

Commit candidate files:

- `app/FamilyClaimRef.App/App.xaml.cs`
- `app/FamilyClaimRef.App/Composition/AppServices.cs`
- `app/FamilyClaimRef.App/MainWindow.xaml`
- `app/FamilyClaimRef.App/MainWindow.xaml.cs`
- `app/FamilyClaimRef.App/ViewModels/MainWindowViewModel.cs`
- `app/FamilyClaimRef.App/ViewModels/PolicyClaimManagementViewModel.cs`
- `tests/FamilyClaimRef.App.Tests/PolicyClaimManagementViewModelTests.cs`
- `docs/131_POLICY_CLAIM_CREATION_MANAGEMENT_UX_SCOPE_DESIGN.md`
- `docs/132_POLICY_CLAIM_CREATION_MANAGEMENT_PHASE3C_USER_DECISION_RECORD.md`
- `docs/133_POLICY_CLAIM_CREATION_MANAGEMENT_PHASE3C_IMPLEMENTATION_PLAN.md`
- `docs/134_POLICY_CLAIM_CREATION_MANAGEMENT_PHASE3C_IMPLEMENTATION_REVIEW.md`
- `docs/135_POLICY_CLAIM_CREATION_MANAGEMENT_PHASE3C_COMMIT_CANDIDATE_REVIEW.md`

## K. Recommended Commit Message

```text
feat(familyclaimref): add policy claim management UI
```

## L. Remaining Risks / Follow-up

- `MainWindow`가 비대해질 수 있다.
- policy/claim edit은 후속이다.
- display label hardening은 후속이다.
- runtime manual validation은 Phase 3D에서 별도로 필요하다.
- separate management window는 후속 구조 후보로 남긴다.
- actual domain fields는 후속 별도 결정이 필요하다.
