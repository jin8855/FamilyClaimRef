# Policy / Claim Runtime Manual Validation Phase 3D Execution Instruction

## A. Status Marker

POLICY_CLAIM_RUNTIME_MANUAL_VALIDATION_PHASE3D_EXECUTION_INSTRUCTION_CREATED

## B. Purpose

이 문서는 Phase 3D runtime manual validation을 실제로 수행하기 위한 실행 지시서다.

이 문서 생성 작업 자체에서는 app launch, `OpenFileDialog`, runtime workflow를 수행하지 않는다.

실제 실행은 이 문서가 생성된 뒤 사용자가 별도로 승인했을 때만 진행한다.

Scenario 1~7은 base execution으로 분리한다.

Scenario 8 synthetic document registration은 optional gated execution으로 분리한다.

## C. Execution Scope

### Base execution scope

Scenario 1~7:

1. Startup / MainWindow Binding
2. Empty State
3. Runtime Policy Creation
4. Runtime Claim Creation
5. Policy Disable Block With Active Claim
6. Claim Disable
7. Policy Disable After Claim Disabled

### Optional gated execution scope

Scenario 8:

8. Synthetic Document Registration

명시:

- Scenario 8은 `OpenFileDialog`와 actual registration workflow를 포함한다.
- Scenario 8은 별도 explicit approval 없이 실행하지 않는다.
- synthetic test document는 Scenario 8 승인 시에만 생성한다.

## D. Absolute Forbidden During Base Execution

Base execution, 즉 Scenario 1~7에서 금지되는 항목:

- `OpenFileDialog` 실행 금지
- 실제 파일 선택 금지
- document registration workflow 실제 실행 금지
- synthetic test document 생성 금지
- 실제 사용자 문서 사용 금지
- 실제 개인정보 사용 금지
- 실제 가족 실명 사용 금지
- 실제 보험계약 번호 사용 금지
- 실제 청구 번호 사용 금지
- 실제 보험사명 사용 금지
- 실제 병원명 사용 금지
- 실제 진단명 / 진단코드 사용 금지
- DB/SQLite/OCR/repository 구현 금지
- runtime artifact cleanup 금지
- `%LOCALAPPDATA%\FamilyClaimRef` 삭제 금지
- project root cleanup 금지
- git add / commit / reset / checkout / clean 금지

## E. Allowed During Base Execution

Base execution에서 허용되는 항목:

- app launch
- `MainWindow` 표시 확인
- Policy / Claim Management section 확인
- empty state 확인
- synthetic-safe policy title 입력
- runtime policy 생성
- synthetic-safe claim title 입력
- runtime claim 생성
- active claim이 있는 policy disable block 확인
- runtime claim disable
- active claim이 없는 policy disable
- pre-run snapshot
- post-run snapshot
- project root `attachments/` files count 확인
- project root `data/local` files count 확인
- DB/SQLite unexpected file 확인
- actual personal sample 확인
- result review 문서 생성

## F. Synthetic Runtime Data

Base execution에서 사용할 값:

- `policy_title_runtime_demo`
- `claim_title_runtime_demo`

Optional Scenario 8에서만 사용할 값:

- `runtime_test_document.txt`
- `document_runtime_demo_001`

금지:

- 실제 가족 실명
- 실제 보험계약 번호
- 실제 청구 번호
- 실제 보험사명
- 실제 병원명
- 실제 진단명
- 실제 진단코드
- 실제 OCR 결과
- 실제 사용자 문서 파일명

## G. Runtime Artifact Root

Runtime root:

```text
%LOCALAPPDATA%\FamilyClaimRef
```

Metadata root:

```text
%LOCALAPPDATA%\FamilyClaimRef\data\local
```

Attachment root:

```text
%LOCALAPPDATA%\FamilyClaimRef\attachments
```

Expected metadata files:

- `%LOCALAPPDATA%\FamilyClaimRef\data\local\policies.json`
- `%LOCALAPPDATA%\FamilyClaimRef\data\local\claims.json`
- `%LOCALAPPDATA%\FamilyClaimRef\data\local\documents.json`
- `%LOCALAPPDATA%\FamilyClaimRef\data\local\policy-documents.json`
- `%LOCALAPPDATA%\FamilyClaimRef\data\local\claim-documents.json`

Expected copied attachment path candidate:

- `%LOCALAPPDATA%\FamilyClaimRef\attachments\documents\<physicalFileName>`

Project root paths that must stay clean:

- `C:\EtcProject\FamilyClaimRef\attachments`
- `C:\EtcProject\FamilyClaimRef\data\local`

## H. Pre-Run Checklist

실제 execution instruction 수행 전 checklist:

1. 프로젝트 루트로 이동한다.

```text
cd C:\EtcProject\FamilyClaimRef
```

2. git 상태를 확인한다.

```text
git status --short
git log -1 --oneline
```

기대 상태:

```text
?? docs/136_POLICY_CLAIM_RUNTIME_MANUAL_VALIDATION_SCOPE_DESIGN.md
?? docs/137_POLICY_CLAIM_RUNTIME_MANUAL_VALIDATION_PHASE3D_USER_DECISION_RECORD.md
?? docs/138_POLICY_CLAIM_RUNTIME_MANUAL_VALIDATION_PHASE3D_EXECUTION_INSTRUCTION.md
```

또는 docs/136~138이 이미 commit된 상태라면 clean일 수 있다.

latest commit 기대값:

```text
b58155d feat(familyclaimref): add policy claim management UI
```

3. build/test를 확인한다.

```text
dotnet build FamilyClaimRef.sln
dotnet test FamilyClaimRef.sln
```

주의:

- Windows SDK 경로 권한 문제가 발생하면 권한 상승 build/test를 수행할 수 있다.
- 권한 상승이 필요하면 result review에 기록한다.

4. project root safety를 확인한다.

```text
project root attachments/ files count
project root data/local files count
```

기대값:

```text
project root attachments/: files=0
project root data/local: files=0
```

5. DB/SQLite unexpected file을 확인한다.

기대값:

```text
DB/SQLite unexpected file: none
```

6. actual personal sample을 확인한다.

기대값:

```text
actual personal sample: none
```

7. `%LOCALAPPDATA%\FamilyClaimRef` pre-run snapshot을 기록한다.

기록할 것:

- directory exists / not exists
- file list
- `policies.json` existence
- `claims.json` existence
- `documents.json` existence
- `policy-documents.json` existence
- `claim-documents.json` existence
- `attachments` directory existence
- DB/SQLite files existence

주의:

- snapshot은 확인과 기록만 수행한다.
- cleanup은 수행하지 않는다.

## I. Base Execution Steps: Scenario 1~7

### Scenario 1: Startup / MainWindow Binding

허용:

- app launch

수행:

1. 앱을 실행한다.
2. `MainWindow`가 표시되는지 확인한다.
3. Document Registration section이 보이는지 확인한다.
4. Policy / Claim Management section이 보이는지 확인한다.
5. `MainWindow.DataContext` binding이 깨지지 않았는지 확인한다.
6. startup crash가 없는지 확인한다.

중단 기준:

- startup crash
- `MainWindow` 표시 실패
- binding failure
- Policy / Claim Management section missing

### Scenario 2: Empty State

수행:

1. active policy / claim이 없는 경우 empty state message를 확인한다.
2. document registration target dropdown이 empty 또는 blocked 상태인지 확인한다.
3. registration 영역 안에 quick create button/link가 없는지 확인한다.

주의:

- 이미 기존 runtime artifacts가 있어 active policy / claim이 존재할 수 있다.
- 이 경우 pre-run snapshot에 기존 artifacts 존재를 기록하고, empty state는 skipped 또는 not applicable로 기록한다.
- cleanup은 수행하지 않는다.

### Scenario 3: Runtime Policy Creation

입력:

```text
policy_title_runtime_demo
```

수행:

1. Policy / Claim Management section에서 policy title에 `policy_title_runtime_demo`를 입력한다.
2. create policy를 실행한다.
3. active policy list에 표시되는지 확인한다.
4. document registration policy dropdown에 반영되는지 확인한다.

검증:

- policy 생성 성공
- active policy list refresh
- registration target dropdown refresh
- project root pollution 없음

### Scenario 4: Runtime Claim Creation

입력:

```text
claim_title_runtime_demo
```

수행:

1. claim 생성용 active policy selector에서 synthetic policy를 선택한다.
2. claim title에 `claim_title_runtime_demo`를 입력한다.
3. create claim을 실행한다.
4. active claim list에 표시되는지 확인한다.
5. document registration claim dropdown에 반영되는지 확인한다.

검증:

- claim 생성 성공
- active claim list refresh
- registration target dropdown refresh
- 실제 병원 / 진단 / 청구번호 field 없음

### Scenario 5: Policy Disable Block With Active Claim

수행:

1. active claim이 연결된 policy를 선택한다.
2. disable policy를 실행한다.
3. block message를 확인한다.

검증:

- policy disable 차단
- active policy 유지
- active claim 유지
- file/link metadata 삭제 없음
- generic message 표시

중단 기준:

- active claim이 있는데 policy disable이 허용됨

### Scenario 6: Claim Disable

수행:

1. selected claim disable을 실행한다.
2. active claim list에서 사라지는지 확인한다.
3. document registration claim dropdown에서 사라지는지 확인한다.
4. policy는 active 상태로 유지되는지 확인한다.

검증:

- claim만 disable
- file/link metadata 삭제 없음

### Scenario 7: Policy Disable After Claim Disabled

수행:

1. active claim이 없는 policy를 선택한다.
2. disable policy를 실행한다.
3. active policy list에서 사라지는지 확인한다.
4. document registration policy dropdown에서 사라지는지 확인한다.

검증:

- policy disable 성공
- file/link metadata 삭제 없음
- project root pollution 없음

## J. Optional Gated Execution: Scenario 8

상태:

```text
Requires separate explicit approval.
```

Scenario 8은 이 execution instruction에 포함하되, 실행은 별도 승인 없이 금지한다.

Scenario 8 실행 시 추가 허용 필요:

- synthetic test document 생성
- `OpenFileDialog` 실행
- actual file selection
- document registration workflow 실제 실행

Synthetic test document:

File name:

```text
runtime_test_document.txt
```

Allowed content:

```text
FamilyClaimRef runtime manual validation synthetic file.
No real personal, insurance, hospital, diagnosis, or claim data.
```

Suggested location:

```text
C:\EtcProject\FamilyClaimRef\runtime_test_document.txt
```

주의:

- 이 파일은 Scenario 8 승인 후에만 생성한다.
- 실제 개인 / 보험 / 의료 / 가족 문서를 사용하지 않는다.
- Scenario 8 후 source tree에 synthetic file이 생기므로 처리 정책을 result review에 기록한다.
- synthetic file cleanup은 별도 승인 없이 수행하지 않는다.

Scenario 8 steps:

1. synthetic test document 생성
2. app launch 상태에서 Select File 실행
3. `OpenFileDialog`에서 `runtime_test_document.txt`만 선택
4. active policy 또는 active claim target 선택
5. document type은 synthetic-safe existing type만 선택
6. register 실행
7. runtime attachments path에 copied file이 생성되었는지 확인
8. `documents.json` sanity 확인
9. `policy-documents.json` 또는 `claim-documents.json` sanity 확인
10. project root `attachments/`와 `data/local` 오염 없음 확인

중단 기준:

- 실제 파일을 선택하려는 상황 발생
- actual personal data 포함 가능성 발생
- copied file이 project root `attachments/`에 생성됨
- DB/SQLite file 생성
- source tree unexpected modification

## K. Post-Run Checklist

Base execution 또는 Scenario 8 이후 확인할 항목:

1. app close 확인
2. `git status --short` 확인
3. project root `attachments/` files count
4. project root `data/local` files count
5. `%LOCALAPPDATA%\FamilyClaimRef` post-run snapshot
6. `policies.json` sanity
7. `claims.json` sanity
8. `documents.json` sanity, Scenario 8 실행 시
9. `policy-documents.json` / `claim-documents.json` sanity, Scenario 8 실행 시
10. copied attachment file location, Scenario 8 실행 시
11. DB/SQLite unexpected file 확인
12. actual personal sample 확인
13. cleanup needed 여부 기록
14. cleanup은 수행하지 않음

## L. Failure / Stop Criteria

다음 상황이 발생하면 즉시 중단하고 result review를 작성한다.

- app startup crash
- `MainWindow` 표시 실패
- `MainWindow.DataContext` binding failure
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

## M. Result Review Document Requirement

Phase 3D execution 이후 반드시 result review 문서를 생성한다.

예상 문서:

```text
docs/139_POLICY_CLAIM_RUNTIME_MANUAL_VALIDATION_PHASE3D_RESULT_REVIEW.md
```

포함할 내용:

- Status Marker
  - `POLICY_CLAIM_RUNTIME_MANUAL_VALIDATION_PHASE3D_BASE_EXECUTED`
  - `POLICY_CLAIM_RUNTIME_MANUAL_VALIDATION_PHASE3D_SCENARIO8_EXECUTED`
  - 또는 `BLOCKED` marker
- executed scenarios
- skipped scenarios
- pre-run snapshot
- post-run snapshot
- app launch result
- policy creation result
- claim creation result
- policy disable block result
- claim disable result
- policy disable after claim disabled result
- Scenario 8 result, if executed
- project root safety result
- DB/SQLite result
- actual personal sample result
- cleanup needed 여부
- cleanup performed: no, unless separately approved
- remaining risks
- next recommendation

## N. Explicit Non-Scope for This Documentation Task

이 docs/138 생성 작업에서는 수행하지 않는다.

- app launch 없음
- `OpenFileDialog` 실행 없음
- runtime workflow 실행 없음
- runtime policy 생성 없음
- runtime claim 생성 없음
- runtime disable 없음
- synthetic test document 생성 없음
- runtime artifact 생성 없음
- runtime artifact 삭제 없음
- `%LOCALAPPDATA%\FamilyClaimRef` 삭제 없음
- code / XAML / ViewModel / test 수정 없음
- DB/SQLite/OCR/repository 구현 없음
- git add / commit / reset / checkout / clean 없음

## O. Verification for This Documentation Task

docs/138 생성 후 수행:

- `git diff --check`
- `git status --short`
- project root `attachments/` files count
- project root `data/local` files count

build/test:

- documentation-only change이므로 실행하지 않는다.

## P. Completion Report Format

완료 보고 형식:

```text
POLICY_CLAIM_RUNTIME_MANUAL_VALIDATION_PHASE3D_EXECUTION_INSTRUCTION_CREATED

생성 문서:
- docs/138_POLICY_CLAIM_RUNTIME_MANUAL_VALIDATION_PHASE3D_EXECUTION_INSTRUCTION.md

분석 대상:
- ...

구현 여부:
- 코드 수정 없음
- XAML 수정 없음
- ViewModel 수정 없음
- 테스트 수정 없음
- app launch 없음
- OpenFileDialog 실행 없음
- runtime workflow 실행 없음
- synthetic test document 생성 없음
- runtime artifact 삭제 없음

execution instruction 요약:
- base scenarios:
- optional gated scenario:
- app launch:
- OpenFileDialog:
- synthetic test document:
- pre/post snapshot:
- cleanup:
- result review document:

검증 결과:
- git diff --check: PASS/FAIL
- git status --short: expected docs/136, docs/137, docs/138 only / unexpected
- project root attachments/: files=<count>
- project root data/local: files=<count>
- build/test: not run, documentation-only change

수정하지 않은 항목:
- AppServices 수정 없음
- DocumentLinkCoordinator 수정 없음
- DocumentRegistrationWorkflow 수정 없음
- MainWindow 수정 없음
- XAML 수정 없음
- ViewModel 수정 없음
- tests 수정 없음
- app launch 없음
- OpenFileDialog 실행 없음
- registration workflow 실제 실행 없음
- runtime artifact 삭제 없음
- %LOCALAPPDATA%\FamilyClaimRef 삭제 없음
- DB/SQLite/OCR/repository 구현 없음
- git add/commit/reset/checkout/clean 사용 없음

다음 추천 작업:
Phase 3D base runtime manual validation execution 진행 여부 사용자 승인
```
