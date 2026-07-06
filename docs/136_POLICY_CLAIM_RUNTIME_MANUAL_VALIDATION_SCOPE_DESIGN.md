# Policy / Claim Runtime Manual Validation Scope Design

## A. Status Marker

POLICY_CLAIM_RUNTIME_MANUAL_VALIDATION_SCOPE_DESIGNED

## B. Background

Phase 1에서는 Policy / Claim JSON storage가 추가되었다.

Phase 2에서는 `DocumentLinkCoordinator`가 active target validation을 수행하도록 확장되었다.

Phase 3B에서는 문서 등록 화면에 Policy / Claim target selection dropdown이 추가되었다.

Phase 3C에서는 `MainWindow`에 Policy / Claim Management UI가 추가되었다.

최근 커밋 기준 자동화 검증은 통과했지만, actual app launch, `OpenFileDialog`, runtime document registration workflow는 아직 수행하지 않았다.

Phase 3D는 실제 런타임 수동 검증 범위를 설계하는 단계다. 이 문서는 실행 계획을 정의할 뿐이며, 앱 실행이나 런타임 데이터 생성은 수행하지 않는다.

## C. Purpose

이 문서의 목적은 Phase 3D에서 별도 승인 후 수행할 runtime manual validation 범위를 정리하는 것이다.

검증 대상은 다음과 같다.

- actual app startup
- `AppServices` composition root
- `MainWindow.DataContext`
- `MainWindowViewModel`
- `DocumentRegistrationViewModel`
- `PolicyClaimManagementViewModel`
- synthetic-safe policy / claim create and disable flow
- document registration target dropdown refresh
- runtime artifact location and cleanup policy

이 문서 작성 작업에서는 다음을 수행하지 않는다.

- app launch
- `OpenFileDialog` 실행
- actual file selection
- actual registration workflow
- runtime policy creation
- runtime claim creation
- runtime disable operation
- runtime cleanup

## D. Current Automated Verification Baseline

최근 커밋:

```text
b58155d feat(familyclaimref): add policy claim management UI
```

최근 리뷰 문서 기준 자동화 검증 결과:

```text
dotnet build FamilyClaimRef.sln: PASS
warning: 0
error: 0
```

```text
dotnet test FamilyClaimRef.sln: PASS
total tests: 271
failed tests: 0
skipped tests: 0
```

최근 리뷰 문서 기준 안전 검증:

- project root `attachments/`: files=0
- project root `data/local`: files=0
- DB/SQLite unexpected file: none
- actual personal sample: none
- actual app launch: not run
- actual `OpenFileDialog`: not run
- runtime registration workflow manual execution: not run

이번 문서 생성 작업에서는 build/test를 재실행하지 않는다. 이 작업은 documentation-only change다.

## E. Runtime Manual Validation Is Not Yet Approved

Phase 3D runtime manual validation은 아직 승인되지 않았다.

이번 문서 작성 작업에서 금지되는 작업은 다음과 같다.

- 앱 실행 금지
- `OpenFileDialog` 실행 금지
- 실제 파일 선택 금지
- 문서 등록 workflow 실제 실행 금지
- runtime policy 생성 금지
- runtime claim 생성 금지
- runtime disable 실행 금지
- runtime artifact 삭제 금지
- `%LOCALAPPDATA%\FamilyClaimRef` 삭제 금지
- source tree cleanup 금지
- Git add / commit / reset / checkout / clean 금지

Phase 3D 실행은 별도 user decision record와 실행 지시가 있어야 한다.

## F. Runtime Artifact Policy

현재 구현 기준 runtime root 후보:

```text
%LOCALAPPDATA%\FamilyClaimRef
```

현재 `AppServices` 기준 metadata root:

```text
%LOCALAPPDATA%\FamilyClaimRef\data\local
```

현재 `AppServices` 기준 attachment root:

```text
%LOCALAPPDATA%\FamilyClaimRef\attachments
```

현재 구현상 가능한 runtime metadata file:

- `%LOCALAPPDATA%\FamilyClaimRef\data\local\policies.json`
- `%LOCALAPPDATA%\FamilyClaimRef\data\local\claims.json`
- `%LOCALAPPDATA%\FamilyClaimRef\data\local\documents.json`
- `%LOCALAPPDATA%\FamilyClaimRef\data\local\policy-documents.json`
- `%LOCALAPPDATA%\FamilyClaimRef\data\local\claim-documents.json`

현재 구현상 가능한 copied attachment path:

```text
%LOCALAPPDATA%\FamilyClaimRef\attachments\documents\<physicalFileName>
```

project root의 아래 경로는 runtime artifact location이 아니다.

- `C:\EtcProject\FamilyClaimRef\attachments`
- `C:\EtcProject\FamilyClaimRef\data\local`

Phase 3D 실행 전에는 `%LOCALAPPDATA%\FamilyClaimRef` pre-run snapshot이 필요하다.

Phase 3D 실행 후에는 `%LOCALAPPDATA%\FamilyClaimRef` post-run snapshot이 필요하다.

cleanup은 이 문서 범위에 포함하지 않는다. `%LOCALAPPDATA%\FamilyClaimRef` 삭제는 별도 cleanup scope design과 명시적 승인 없이는 수행하지 않는다.

## G. Synthetic Runtime Test Data Policy

Phase 3D에서 허용 가능한 synthetic-safe sample value 후보:

- `policy_runtime_demo_001`
- `claim_runtime_demo_001`
- `document_runtime_demo_001`
- `policy_title_runtime_demo`
- `claim_title_runtime_demo`
- `runtime_test_document.txt`

금지되는 값:

- 실제 가족 실명
- 실제 보험사명
- 실제 병원명
- 실제 진단명
- 실제 진단코드
- 실제 보험증권번호
- 실제 청구번호
- 실제 OCR 결과
- 실제 사용자 문서
- 실제 보험 / 의료 / 가족 정보가 포함된 파일명

synthetic document registration을 별도 승인 후 수행한다면, test document는 harmless synthetic text file이어야 한다.

권장 synthetic file content:

```text
FamilyClaimRef runtime manual validation synthetic file.
No real personal, insurance, hospital, diagnosis, or claim data.
```

위 파일은 이 문서 작성 작업에서 생성하지 않는다.

## H. Proposed Phase 3D Manual Validation Scenarios

### 1. Startup / MainWindow Binding

목적:

- 앱이 정상 시작되는지 확인한다.
- `MainWindow`가 표시되는지 확인한다.
- `MainWindow.DataContext`가 `MainWindowViewModel`로 연결되는지 확인한다.
- `DocumentRegistrationViewModel`과 `PolicyClaimManagementViewModel`이 함께 연결되는지 확인한다.

확인 항목:

- Document Registration section 표시
- Policy / Claim Management section 표시
- startup exception 없음
- binding error 없음
- source tree 변경 없음

### 2. Empty State

목적:

- active policy / claim이 없을 때의 UI 상태를 확인한다.

확인 항목:

- active policy list empty state 표시
- active claim list empty state 표시
- document registration target selection이 empty 또는 blocked 상태로 표시
- registration area에 quick create action 없음
- 실제 domain field 입력 요구 없음

### 3. Runtime Policy Creation

목적:

- synthetic policy 생성 후 목록과 문서 등록 target dropdown이 갱신되는지 확인한다.

입력 후보:

```text
policy_title_runtime_demo
```

확인 항목:

- active policy list에 synthetic policy 표시
- document registration policy dropdown에 synthetic policy 표시
- project root `attachments/` 파일 생성 없음
- project root `data/local` 파일 생성 없음
- runtime artifact는 `%LOCALAPPDATA%\FamilyClaimRef` 아래에만 생성

### 4. Runtime Claim Creation

목적:

- active policy 선택 후 synthetic claim 생성과 dropdown 반영을 확인한다.

입력 후보:

```text
claim_title_runtime_demo
```

확인 항목:

- active claim list에 synthetic claim 표시
- document registration claim dropdown에 synthetic claim 표시
- claim이 selected policy와 연결됨
- 실제 보험사명 / 병원명 / 진단명 / 진단코드 field 없음

### 5. Policy Disable Block With Active Claim

목적:

- active claim이 연결된 policy의 disable이 차단되는지 확인한다.

확인 항목:

- policy disable action blocked
- active policy 유지
- active claim 유지
- file metadata 삭제 없음
- link metadata 삭제 없음
- 사용자 메시지가 generic하고 실제 개인정보를 포함하지 않음

### 6. Claim Disable

목적:

- selected claim disable 후 active claim list와 claim dropdown이 갱신되는지 확인한다.

확인 항목:

- disabled claim이 active claim list에서 제거
- disabled claim이 document registration claim dropdown에서 제거
- 연결된 policy는 active 상태 유지
- file metadata 삭제 없음
- link metadata 삭제 없음

### 7. Policy Disable After Claim Disabled

목적:

- active claim이 없는 policy disable이 가능한지 확인한다.

확인 항목:

- disabled policy가 active policy list에서 제거
- disabled policy가 document registration policy dropdown에서 제거
- file metadata 삭제 없음
- link metadata 삭제 없음
- project root pollution 없음

### 8. Synthetic Document Registration

상태:

```text
Requires Separate Approval
```

이 시나리오는 `OpenFileDialog`와 actual registration workflow를 포함하므로 Phase 3D 실행 승인과 별도 synthetic file 준비가 필요하다.

입력 후보:

```text
runtime_test_document.txt
```

확인 항목:

- synthetic file만 선택
- copied file은 `%LOCALAPPDATA%\FamilyClaimRef\attachments\documents` 아래에만 생성
- `documents.json` metadata 생성 또는 갱신
- `policy-documents.json` 또는 `claim-documents.json` link metadata 생성 또는 갱신
- project root `attachments/` 파일 생성 없음
- project root `data/local` 파일 생성 없음
- 실제 개인정보 샘플 없음

## I. Pre-Run Checklist for Future Phase 3D Execution

Phase 3D 실행 전 확인할 항목:

- user explicit approval 확보
- `git status --short` clean 확인
- `git log -1 --oneline` 확인
- `dotnet build FamilyClaimRef.sln` PASS 확인
- `dotnet test FamilyClaimRef.sln` PASS 확인
- project root `attachments/` files=0 확인
- project root `data/local` files=0 확인
- DB/SQLite unexpected file 없음 확인
- actual personal sample 없음 확인
- `%LOCALAPPDATA%\FamilyClaimRef` pre-run snapshot 기록
- synthetic test document path 결정
- cleanup policy 별도 승인 여부 확인
- real file picker selection risk 확인

## J. Post-Run Checklist for Future Phase 3D Execution

Phase 3D 실행 후 확인할 항목:

- app close 확인
- `git status --short` 확인
- project root `attachments/` files=0 확인
- project root `data/local` files=0 확인
- `%LOCALAPPDATA%\FamilyClaimRef` post-run snapshot 기록
- `policies.json` sanity 확인
- `claims.json` sanity 확인
- `documents.json` sanity 확인
- `policy-documents.json` / `claim-documents.json` sanity 확인
- copied attachment location 확인
- DB/SQLite unexpected file 없음 확인
- actual personal sample 없음 확인
- cleanup needed 여부 기록
- 별도 승인 없는 cleanup 미수행 확인

## K. Failure / Stop Criteria

아래 항목이 발생하면 Phase 3D 실행을 중단하고 review 문서를 작성한다.

- startup crash
- `MainWindow` 표시 실패
- binding failure
- `DocumentRegistrationViewModel` 연결 실패
- `PolicyClaimManagementViewModel` 연결 실패
- Policy / Claim Management section missing
- policy create failure
- claim create failure
- active claim이 있는 policy disable 허용
- disable action이 file metadata 또는 link metadata 삭제
- project root `attachments/` 파일 생성
- project root `data/local` 파일 생성
- DB/SQLite file 생성
- 실제 개인정보 샘플 포함
- 실제 보험 / 의료 / 가족 파일 선택 위험 발생
- source tree unexpected modification
- cleanup이 검증 증거를 삭제할 위험 발생

## L. Explicit Non-Scope for Phase 3D Scope Design

이 문서 작성 작업의 non-scope:

- app launch
- `OpenFileDialog` execution
- actual file selection
- actual registration workflow
- runtime artifact creation
- runtime artifact deletion
- `%LOCALAPPDATA%\FamilyClaimRef` deletion
- C# code modification
- XAML modification
- ViewModel modification
- test code modification
- `AppServices` modification
- `DocumentLinkCoordinator` modification
- `DocumentRegistrationWorkflow` modification
- DB / SQLite / OCR / repository implementation
- Git add / commit / reset / checkout / clean

## M. Recommended Phase 3D Split

권장 분리:

1. Phase 3D-1 scope design
   - 이 문서.
2. Phase 3D-2 user decision record
   - runtime manual validation 실행 여부와 synthetic data policy 승인 기록.
3. Phase 3D-3 execution instruction
   - 실제 app launch, policy / claim create, optional synthetic document registration 수행 지시.
4. Phase 3D-4 result review
   - 실행 결과, runtime artifact snapshot, cleanup 필요 여부 기록.

## N. Risks

남은 위험:

- runtime validation은 source tree 또는 `%LOCALAPPDATA%\FamilyClaimRef`에 artifact를 만들 수 있다.
- `OpenFileDialog`에서 실제 보험 / 의료 / 가족 문서를 선택할 위험이 있다.
- 실제 개인정보가 파일명이나 문서 내용에 섞일 위험이 있다.
- cleanup이 필요한 증거를 삭제할 수 있다.
- `MainWindow`에 기능이 늘어나 UI 검증 항목이 복잡해질 수 있다.
- 수동 검증은 pre-run / post-run snapshot이 없으면 재현성이 낮다.
- project root artifact와 runtime artifact를 혼동할 수 있다.

## O. Next Recommendation

다음 작업은 `Policy / Claim Runtime Manual Validation Phase 3D user decision record` 문서를 생성하는 것이다.

그 문서에서 다음 항목을 명시적으로 승인받아야 한다.

- app launch 허용 여부
- runtime policy / claim creation 허용 여부
- synthetic document registration 수행 여부
- `OpenFileDialog` 실행 허용 여부
- `%LOCALAPPDATA%\FamilyClaimRef` snapshot 방식
- cleanup 수행 여부와 별도 승인 필요 여부
