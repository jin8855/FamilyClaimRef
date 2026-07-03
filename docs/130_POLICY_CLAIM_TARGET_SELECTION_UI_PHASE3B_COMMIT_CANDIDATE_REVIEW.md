# Policy / Claim Target Selection UI Phase 3B Commit Candidate Review

## A. Status Marker

POLICY_CLAIM_TARGET_SELECTION_UI_PHASE3B_COMMIT_CANDIDATE_READY

## B. Review Target

검토 대상 파일:

- `app/FamilyClaimRef.App/Composition/AppServices.cs`
- `app/FamilyClaimRef.App/MainWindow.xaml`
- `app/FamilyClaimRef.App/MainWindow.xaml.cs`
- `app/FamilyClaimRef.App/ViewModels/DocumentRegistrationViewModel.cs`
- `tests/FamilyClaimRef.App.Tests/DocumentRegistrationViewModelTests.cs`
- `docs/126_MAINWINDOW_TARGET_SELECTION_UI_SCOPE_DESIGN.md`
- `docs/127_POLICY_CLAIM_TARGET_SELECTION_UI_PHASE3B_IMPLEMENTATION_PLAN.md`
- `docs/128_POLICY_CLAIM_TARGET_SELECTION_UI_PHASE3B_USER_DECISION_RECORD.md`
- `docs/129_POLICY_CLAIM_TARGET_SELECTION_UI_PHASE3B_IMPLEMENTATION_REVIEW.md`

직전 커밋:

```text
1aba6d5 feat(familyclaimref): add policy claim link validation
```

## C. Scope Review

판정:

```text
PASS
```

확인 내용:

- Phase 3B 범위인 active `Policy` / `Claim` dropdown 기반 target selection 구현이다.
- `docs/126`의 Option B 권장 방향과 일치한다.
- `docs/127`의 Phase 3B implementation plan과 일치한다.
- `docs/128`의 user decision record와 일치한다.
- `docs/129`의 구현 결과 리뷰와 현재 변경분이 일치한다.
- Option C quick create는 구현되지 않았다.
- policy/claim CRUD는 구현되지 않았다.
- seed data는 구현되지 않았다.
- DB/SQLite/OCR/repository는 구현되지 않았다.
- `DocumentLinkCoordinator.cs`와 `DocumentRegistrationWorkflow.cs`는 수정되지 않았다.

## D. Implementation Review

### ViewModel

판정:

```text
PASS
```

확인 내용:

- `DocumentRegistrationViewModel`에 active policy/claim option loading이 추가되었다.
- async constructor를 사용하지 않는다.
- `LoadTargetOptionsAsync`가 명시적 load method로 추가되었다.
- 기존 `GetPoliciesAsync` / `GetClaimsAsync` active-only API를 사용한다.
- `AvailablePolicies`, `AvailableClaims`, `SelectedPolicyId`, `SelectedClaimId`, `TargetSelectionMessage`가 추가되었다.
- selected policy/claim은 기존 registration workflow contract인 target kind/id로 연결된다.
- target option load 이후 target 미선택 시 validation message로 registration을 차단한다.
- UI validation은 `DocumentLinkCoordinator` validation을 대체하지 않는다.
- 기존 `TargetId` property는 workflow contract와 fallback 호환을 위해 유지되었다.

### MainWindow.xaml

판정:

```text
PASS
```

확인 내용:

- `Target selection` 영역에 active policy dropdown과 active claim dropdown이 추가되었다.
- `TargetKind`에 따라 policy 또는 claim dropdown이 표시된다.
- direct `TargetId` input은 primary UI에서 제거되었다.
- `TargetSelectionMessage` 표시 영역이 추가되었다.
- quick create button/link는 추가되지 않았다.
- policy/claim 생성, 수정, disable UI는 추가되지 않았다.
- 실제 개인정보/보험/병원/진단 샘플 item은 XAML에 하드코딩되지 않았다.

### MainWindow.xaml.cs

판정:

```text
PASS_WITH_NOTES
```

확인 내용:

- `Loaded` 시점에 `LoadTargetOptionsAsync`를 호출하는 최소 lifecycle hook이 추가되었다.
- async constructor를 피하기 위한 제한된 변경으로 판단된다.
- app launch나 OpenFileDialog 실행 로직은 추가되지 않았다.
- 기존 `SelectFileButton_Click` / `RegisterButton_Click` 흐름은 유지되었다.
- ViewModel 책임이 code-behind로 과도하게 이동하지 않았다.

비고:

- 원래 가급적 수정 금지 후보였으나, UI lifecycle에서 target options를 로드해야 하므로 commit 범위에 포함 가능하다고 판단한다.

### AppServices

판정:

```text
PASS
```

확인 내용:

- 기존 `JsonPolicyClaimStorageService` instance를 `DocumentRegistrationViewModel`에 주입한다.
- 중복 storage instance를 만들지 않는다.
- metadata root/path 일관성을 유지한다.
- broad composition redesign은 없다.
- service locator, static access, optional null fallback은 없다.

### Service API

판정:

```text
PASS
```

확인 내용:

- `IPolicyClaimStorageService.cs` 수정 없음.
- `JsonPolicyClaimStorageService.cs` 수정 없음.
- 기존 `GetPoliciesAsync` / `GetClaimsAsync` active-only list API로 Phase 3B 구현이 충분했다.

### Direct id input handling

판정:

```text
PASS
```

확인 내용:

- direct target id input은 primary UI에서 제거되었다.
- 내부 `TargetId` contract는 기존 workflow 호출과 기존 tests 호환을 위해 유지되었다.
- dropdown selection이 `TargetId`로 반영된다.
- target option load 이후에는 selected policy/claim 없이 registration이 진행되지 않는다.

## E. Test Review

판정:

```text
PASS
```

보강된 테스트 범위:

- ViewModel이 active policy/claim options를 로드한다.
- disabled policy/claim records are not exposed in available options.
- selecting policy sets target kind/id used for registration.
- selecting claim sets target kind/id used for registration.
- no active policy shows empty state message.
- no active claim shows empty state message.
- registration without selected policy target is blocked.
- registration without selected claim target is blocked.
- constructor rejects null policy claim storage.

전체 테스트 결과:

```text
total tests: 258
passed tests: 258
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
- 실제 진단명/진단코드 사례 없음.
- synthetic id만 테스트에 사용됨.
- DB/SQLite unexpected file 없음.
- project root `attachments/` files=0.
- project root `data/local` files=0.
- app launch 없음.
- OpenFileDialog 실행 없음.
- registration workflow 실제 실행 없음.

참고:

- 개인정보 관련 검색에서 발견된 문자열은 금지/주의 안내 문구이며 실제 샘플 데이터가 아니다.

## G. Verification Results

검증 명령:

```powershell
git diff --check
dotnet build FamilyClaimRef.sln
dotnet test FamilyClaimRef.sln
git status --short
```

검증 결과:

| Check | Result | Notes |
|---|---|---|
| `git diff --check` | PASS | LF/CRLF warning only |
| `dotnet build FamilyClaimRef.sln` | PASS | elevated run, warning 0, error 0 |
| `dotnet test FamilyClaimRef.sln` | PASS | elevated run, total 258, failed 0, skipped 0 |
| project root `attachments/` | PASS | files=0 |
| project root `data/local` | PASS | files=0 |
| DB/SQLite unexpected file | PASS | none |
| actual personal sample | PASS | none |

일반 build 초기 실패:

```text
Access to the path 'C:\Users\jin8855\AppData\Local\Microsoft SDKs' is denied.
```

기록:

- Windows SDK 경로 접근 권한 문제로 일반 build가 실패했다.
- 권한 상승 build/test로 재실행하여 통과했다.
- 권한 상승 필요는 환경 접근 권한 문제이며 구현 실패 사유가 아니다.

## H. Git Status Summary

문서 생성 전 status:

```text
 M app/FamilyClaimRef.App/Composition/AppServices.cs
 M app/FamilyClaimRef.App/MainWindow.xaml
 M app/FamilyClaimRef.App/MainWindow.xaml.cs
 M app/FamilyClaimRef.App/ViewModels/DocumentRegistrationViewModel.cs
 M tests/FamilyClaimRef.App.Tests/DocumentRegistrationViewModelTests.cs
?? docs/126_MAINWINDOW_TARGET_SELECTION_UI_SCOPE_DESIGN.md
?? docs/127_POLICY_CLAIM_TARGET_SELECTION_UI_PHASE3B_IMPLEMENTATION_PLAN.md
?? docs/128_POLICY_CLAIM_TARGET_SELECTION_UI_PHASE3B_USER_DECISION_RECORD.md
?? docs/129_POLICY_CLAIM_TARGET_SELECTION_UI_PHASE3B_IMPLEMENTATION_REVIEW.md
```

이 문서 생성 후 expected additional file:

```text
?? docs/130_POLICY_CLAIM_TARGET_SELECTION_UI_PHASE3B_COMMIT_CANDIDATE_REVIEW.md
```

unexpected file:

```text
none
```

## I. Commit Readiness

commit readiness:

```text
ready
```

reason:

- 변경 범위가 Phase 3B target selection UI에 한정된다.
- 구현 문서와 사용자 결정 기록의 범위를 벗어나지 않는다.
- `DocumentLinkCoordinator`와 `DocumentRegistrationWorkflow`는 수정되지 않았다.
- quick create, policy/claim CRUD, seed data, DB/SQLite/OCR/repository 구현이 없다.
- build/test가 통과했다.
- root `attachments/`, `data/local`에 파일이 없다.
- 실제 개인정보 샘플이 없다.

## J. Commit Candidate Exact File List

commit candidate exact file list:

- `app/FamilyClaimRef.App/Composition/AppServices.cs`
- `app/FamilyClaimRef.App/MainWindow.xaml`
- `app/FamilyClaimRef.App/MainWindow.xaml.cs`
- `app/FamilyClaimRef.App/ViewModels/DocumentRegistrationViewModel.cs`
- `tests/FamilyClaimRef.App.Tests/DocumentRegistrationViewModelTests.cs`
- `docs/126_MAINWINDOW_TARGET_SELECTION_UI_SCOPE_DESIGN.md`
- `docs/127_POLICY_CLAIM_TARGET_SELECTION_UI_PHASE3B_IMPLEMENTATION_PLAN.md`
- `docs/128_POLICY_CLAIM_TARGET_SELECTION_UI_PHASE3B_USER_DECISION_RECORD.md`
- `docs/129_POLICY_CLAIM_TARGET_SELECTION_UI_PHASE3B_IMPLEMENTATION_REVIEW.md`
- `docs/130_POLICY_CLAIM_TARGET_SELECTION_UI_PHASE3B_COMMIT_CANDIDATE_REVIEW.md`

## K. Recommended Commit Message

```text
feat(familyclaimref): add policy claim target selection UI
```

## L. Remaining Risks / Follow-up

남은 위험:

- active policy/claim이 없으면 사용자는 문서 등록을 완료할 수 없다.
- policy/claim creation/management UX는 아직 없다.
- runtime manual validation은 아직 수행하지 않았다.
- actual app launch는 아직 수행하지 않았다.
- dropdown display label hardening이 후속으로 필요할 수 있다.

후속 작업:

- policy/claim creation/management UX는 Phase 3C에서 결정한다.
- runtime manual validation은 Phase 3D에서 별도 승인 후 진행한다.
- display label hardening은 후속 UX hardening으로 둔다.
- docs/126~130 및 Phase 3B 구현 변경분 commit 진행 여부를 별도 지시로 확정한다.

## M. Result

POLICY_CLAIM_TARGET_SELECTION_UI_PHASE3B_COMMIT_CANDIDATE_READY
