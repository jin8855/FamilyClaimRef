# Policy / Claim Target Selection UI Phase 3B Implementation Review

## A. Status Marker

POLICY_CLAIM_TARGET_SELECTION_UI_PHASE3B_IMPLEMENTED

## B. Implementation Scope

이번 Phase 3B에서는 Document Registration 화면의 target 선택 흐름을 active `Policy` / `Claim` dropdown 기반으로 최소 구현했다.

구현 범위:

- `DocumentRegistrationViewModel` active policy/claim option loading 추가
- `MainWindow.xaml` target selection dropdown UI 추가
- 선택된 policy/claim을 기존 registration workflow contract인 target kind/id로 연결
- target 미선택 시 validation message로 registration 차단
- `AppServices`에서 기존 `JsonPolicyClaimStorageService` instance를 ViewModel에 주입
- focused ViewModel tests 보강

명시적 비범위:

- policy/claim CRUD 없음
- quick create 없음
- seed data 없음
- DB/SQLite/OCR/repository 없음
- app launch 없음
- OpenFileDialog 실행 없음
- runtime manual workflow 실행 없음

## C. ViewModel Changes

수정 파일:

- `app/FamilyClaimRef.App/ViewModels/DocumentRegistrationViewModel.cs`

구현 내용:

- `IPolicyClaimStorageService` dependency 추가
- `LoadTargetOptionsAsync` 추가
- `AvailablePolicies` 추가
- `AvailableClaims` 추가
- `SelectedPolicyId` 추가
- `SelectedClaimId` 추가
- `TargetSelectionMessage` 추가
- `HasAvailablePolicies` 추가
- `HasAvailableClaims` 추가
- target kind 변경 시 selected target id를 기존 `TargetId` contract에 반영
- target option load 이후 no active target / no selected target validation 추가

검증 경계:

- UI validation은 `DocumentLinkCoordinator` validation을 대체하지 않는다.
- workflow로 전달되는 최종 contract는 기존 target kind/id 구조를 유지한다.

## D. MainWindow UI Changes

수정 파일:

- `app/FamilyClaimRef.App/MainWindow.xaml`

구현 내용:

- 기존 `Target` 영역을 `Target selection` 영역으로 재구성
- target type selector 유지
- policy target dropdown 추가
- claim target dropdown 추가
- `TargetKind`에 따라 policy 또는 claim dropdown 표시
- `TargetSelectionMessage` 표시 영역 추가
- direct target id input은 primary UI에서 제거
- create button / quick create link는 추가하지 않음

direct id input 처리:

- user-facing primary UI에서는 제거했다.
- 내부 `TargetId` property는 workflow contract와 dev fallback 호환을 위해 유지했다.
- dropdown selection이 `TargetId`에 반영되는 구조로 연결했다.

## E. MainWindow.xaml.cs Update

수정 파일:

- `app/FamilyClaimRef.App/MainWindow.xaml.cs`

수정 이유:

- async constructor를 피하면서 UI lifecycle에서 `LoadTargetOptionsAsync`를 호출하기 위해 `Window_Loaded` handler를 최소 추가했다.

구현 내용:

- `Loaded="Window_Loaded"` 연결
- `DataContext`가 `DocumentRegistrationViewModel`일 때 `LoadTargetOptionsAsync` 호출

제한 준수:

- 기존 `SelectFileButton_Click` / `RegisterButton_Click` 흐름은 유지했다.
- app launch는 수행하지 않았다.
- OpenFileDialog는 실행하지 않았다.

## F. AppServices Update

수정 파일:

- `app/FamilyClaimRef.App/Composition/AppServices.cs`

구현 내용:

- 이미 생성된 `JsonPolicyClaimStorageService` instance를 `DocumentRegistrationViewModel`에 주입했다.

판단:

- 중복 service instance를 만들지 않았다.
- metadata root/path 일관성을 유지했다.
- broad composition redesign, service locator, static access는 사용하지 않았다.

## G. Service API Decision

수정 여부:

- `IPolicyClaimStorageService.cs` 수정 없음
- `JsonPolicyClaimStorageService.cs` 수정 없음

판단:

- 기존 `GetPoliciesAsync`는 active-only list를 반환한다.
- 기존 `GetClaimsAsync`는 active-only list를 반환한다.
- Phase 3B active dropdown 구현에는 기존 API로 충분했다.
- service API extension은 필요하지 않았다.

## H. Test Coverage

수정 파일:

- `tests/FamilyClaimRef.App.Tests/DocumentRegistrationViewModelTests.cs`

보강한 테스트 범위:

- ViewModel이 active policy/claim options를 로드한다.
- disabled policy/claim records are not exposed in available options.
- selecting policy sets target kind/id used for registration.
- selecting claim sets target kind/id used for registration.
- no active policy shows empty state message.
- no active claim shows empty state message.
- registration without selected policy target is blocked.
- registration without selected claim target is blocked.
- constructor rejects null policy claim storage.

테스트 원칙:

- unit test 범위에서만 검증했다.
- app launch 없음
- OpenFileDialog 실행 없음
- actual runtime registration workflow 실행 없음
- synthetic id만 사용

## I. Verification Results

검증 명령:

```powershell
git diff --check
dotnet build FamilyClaimRef.sln
dotnet test FamilyClaimRef.sln
git status --short
```

현재 기록된 결과:

| Check | Result | Notes |
|---|---|---|
| `git diff --check` | PASS | LF/CRLF warning only |
| `dotnet build FamilyClaimRef.sln` | PASS | warning 0, error 0 |
| `dotnet test FamilyClaimRef.sln` | PASS | total 258, failed 0, skipped 0 |
| project root `attachments/` | PASS | files=0 |
| project root `data/local` | PASS | files=0 |
| DB/SQLite unexpected file | PASS | none |
| actual personal sample | PASS | none |
| `git status --short` | PASS | expected Phase 3B files and docs/126~129 |

초기 build 실패:

```text
Access to the path 'C:\Users\jin8855\AppData\Local\Microsoft SDKs' is denied.
```

기록:

- Windows SDK 경로 접근 권한 문제로 일반 build가 실패했다.
- 권한 상승 build/test로 재실행하여 통과했다.

## J. Safety Review

확인된 비범위:

- `DocumentLinkCoordinator.cs` 수정 없음
- `DocumentRegistrationWorkflow.cs` 수정 없음
- policy/claim CRUD 구현 없음
- quick create 구현 없음
- seed data 구현 없음
- DB/SQLite/OCR/repository 구현 없음
- app launch 없음
- OpenFileDialog 실행 없음
- registration workflow 실제 실행 없음
- Git add/commit/reset/checkout/clean 없음

개인정보 샘플:

- 실제 가족 실명 없음
- 실제 보험사명 없음
- 실제 병원명 없음
- 실제 진단명 없음
- 실제 진단코드 없음
- 실제 계약 번호 없음
- 실제 청구 번호 없음

## K. Remaining Risks / Follow-up

남은 위험:

- active policy/claim이 없으면 사용자는 아직 문서 등록을 완료할 수 없다.
- policy/claim creation/management UX가 없어 실제 MVP 사용성은 제한된다.
- dropdown display label이 `DisplayTitle` 중심이라 구분성이 부족할 수 있다.
- runtime manual validation은 아직 수행하지 않았다.
- actual app launch는 아직 수행하지 않았다.

후속 작업:

- policy/claim creation/management UX는 Phase 3C에서 진행
- runtime manual validation은 Phase 3D에서 별도 승인 후 진행
- display label hardening은 후속 UX hardening에서 진행
- docs/126~129 및 Phase 3B 구현 변경분 commit candidate review 필요

## L. Result

POLICY_CLAIM_TARGET_SELECTION_UI_PHASE3B_IMPLEMENTED
