# Policy / Claim Scenario 8 Synthetic Document Registration Execution Instruction

## A. Status Marker

POLICY_CLAIM_SCENARIO8_SYNTHETIC_DOCUMENT_REGISTRATION_EXECUTION_INSTRUCTION_CREATED

## B. Purpose

이 문서는 Scenario 8A policy target synthetic document registration을 실제로 수행하기 위한 실행 지시서다.

이 문서 생성 작업 자체에서는 app launch, OpenFileDialog, synthetic file creation, document registration workflow를 수행하지 않는다.

실제 실행은 이 문서가 생성되고 사용자가 별도 승인한 뒤에만 수행한다.

Scenario 8A는 policy target registration only로 제한한다.

Scenario 8B claim target registration은 이번 instruction 범위가 아니며 별도 approval 후보로 남긴다.

## C. Execution Approval Gate

실제 실행 전 필요한 승인 marker 후보:

```text
PHASE3D_SCENARIO8A_SYNTHETIC_POLICY_DOCUMENT_REGISTRATION_APPROVED
```

승인 범위:

- app launch
- temp synthetic document creation
- runtime synthetic policy creation
- OpenFileDialog execution
- approved synthetic file selection only
- policy target selection
- document registration workflow execution
- runtime copied attachment verification
- documents.json sanity check
- policy-documents.json sanity check
- project root safety check
- result review document creation

미승인 / 계속 금지:

- claim target registration
- Scenario 8B
- actual personal/insurance/hospital/diagnosis document use
- actual user document use
- cleanup
- `%LOCALAPPDATA%\FamilyClaimRef` deletion
- project root cleanup
- DB/SQLite/OCR/repository implementation
- code/XAML/ViewModel/test modification

## D. Execution Scope

Approved future execution candidate:

- Scenario 8A: policy target synthetic document registration only

Not included:

- Scenario 8B claim target synthetic document registration
- cleanup
- full runtime root cleanup
- display label hardening
- UI cleanup
- code changes

Scenario 8A expected flow:

1. pre-run source and runtime snapshot
2. build/test baseline
3. create synthetic temp document outside project root
4. app launch
5. create active synthetic policy
6. select synthetic file through OpenFileDialog
7. select policy target
8. select existing synthetic-safe document type
9. run document registration
10. verify copied attachment under runtime attachment root
11. verify documents.json update
12. verify policy-documents.json update
13. verify project root remains clean
14. create result review document
15. no cleanup

## E. Current Runtime Baseline

docs/145 기준 current runtime state:

- `%LOCALAPPDATA%\FamilyClaimRef` exists
- `%LOCALAPPDATA%\FamilyClaimRef\data\local` exists
- `%LOCALAPPDATA%\FamilyClaimRef\attachments` exists
- `policies.json` missing
- `claims.json` missing
- `documents.json` exists
- `policy-documents.json` exists
- `claim-documents.json` missing
- known runtime attachment exists
- project root `attachments/`: files=0
- project root `data/local`: files=0
- DB/SQLite unexpected file: 없음

주의:

- runtime root는 clean-room 상태가 아니다.
- existing documents/link/attachment evidence가 남아 있다.
- Scenario 8A 결과는 pre/post snapshot으로 구분해야 한다.
- active policy는 execution 중 새로 생성해야 한다.

## F. Synthetic Test Data

Allowed synthetic runtime policy title:

- `policy_title_scenario8_demo`

Allowed synthetic document file name:

- `runtime_test_document.txt`

Approved synthetic document path:

- `%TEMP%\FamilyClaimRef\runtime_test_document.txt`

Allowed file content:

```text
FamilyClaimRef runtime manual validation synthetic file.
No real personal, insurance, hospital, diagnosis, or claim data.
```

Forbidden:

- 실제 개인정보
- 실제 가족 실명
- 실제 보험계약 번호
- 실제 청구 번호
- 실제 보험사명
- 실제 병원명
- 실제 진단명/진단코드
- 실제 OCR 결과
- 실제 사용자 문서 내용
- 실제 보험/의료/가족 관련 파일명

## G. Absolute Forbidden During Scenario 8A Execution

실제 실행 시에도 금지할 항목:

- actual personal document selection
- actual insurance document selection
- actual hospital/medical document selection
- actual family document selection
- approved synthetic file 외 파일 선택
- claim target registration
- Scenario 8B 실행
- runtime cleanup
- `%LOCALAPPDATA%\FamilyClaimRef` 삭제
- project root cleanup
- project root `runtime_test_document.txt` 생성
- DB/SQLite/OCR/repository 구현
- code 수정
- XAML 수정
- ViewModel 수정
- tests 수정
- `git add .` / `git add -A` / `git add --all`
- `git reset` / `git checkout` / `git clean`
- commit

## H. Pre-Run Checklist

실제 Scenario 8A execution 전 수행할 checklist:

1. 프로젝트 루트 이동

```powershell
cd C:\EtcProject\FamilyClaimRef
```

2. source tree 상태 확인

```powershell
git status --short
git log -1 --oneline
```

기대:

- latest commit:

```text
58f891a docs(familyclaimref): add runtime validation cleanup review
```

- `docs/145`, `docs/146`만 untracked이거나, 해당 문서들이 이미 commit된 경우 clean

unexpected source tree change가 있으면 실행 중단한다.
`reset`/`checkout`/`clean`으로 정리하지 않는다.

3. build/test baseline 확인

```powershell
dotnet build FamilyClaimRef.sln
dotnet test FamilyClaimRef.sln
```

주의:

- Windows SDK path permission 문제가 발생하면 권한 상승 build/test를 수행할 수 있다.
- 권한 상승이 필요하면 result review에 기록한다.

4. project root safety pre-check

확인:

- `C:\EtcProject\FamilyClaimRef\attachments` files count
- `C:\EtcProject\FamilyClaimRef\data\local` files count
- project root `runtime_test_document.txt` absence

기대:

- project root `attachments/`: files=0
- project root `data/local`: files=0
- `C:\EtcProject\FamilyClaimRef\runtime_test_document.txt`: missing

5. temp synthetic file pre-check

확인:

- `%TEMP%\FamilyClaimRef` exists / not exists
- `%TEMP%\FamilyClaimRef\runtime_test_document.txt` exists / not exists

주의:

- 파일이 이미 있으면 내용이 approved synthetic content와 정확히 일치하는지 확인한다.
- 내용이 다르면 중단하거나 result review에 blocked로 기록한다.
- 기존 파일이 있고 내용이 정확히 일치하면 reuse 가능 여부를 기록한다.

6. runtime root pre-run snapshot

기록:

- `%LOCALAPPDATA%\FamilyClaimRef` exists
- metadata root exists
- attachments root exists
- `policies.json` exists / missing
- `claims.json` exists / missing
- `documents.json` exists / missing
- `policy-documents.json` exists / missing
- `claim-documents.json` exists / missing
- known runtime attachment exists / missing
- runtime file list
- DB/SQLite unexpected file check
- actual personal sample targeted scan

7. expected pre-run baseline

기대:

- `policies.json` missing
- `claims.json` missing
- `documents.json` exists
- `policy-documents.json` exists
- `claim-documents.json` missing
- runtime attachments exists
- project root attachments/data/local clean

기대와 다르면 result review에 기록하고, 실행 여부를 신중히 판단한다.

## I. Synthetic Test Document Creation Step for Future Approved Run

중요:
이 docs/146 생성 작업에서는 실행하지 않는다.
후속 사용자가 Scenario 8A execution을 별도 승인한 경우에만 수행한다.

실행 후보 PowerShell:

```powershell
$scenario8TempRoot = Join-Path $env:TEMP 'FamilyClaimRef'
$scenario8DocumentPath = Join-Path $scenario8TempRoot 'runtime_test_document.txt'
New-Item -ItemType Directory -Path $scenario8TempRoot -Force | Out-Null
@'
FamilyClaimRef runtime manual validation synthetic file.
No real personal, insurance, hospital, diagnosis, or claim data.
'@ | Set-Content -LiteralPath $scenario8DocumentPath -Encoding UTF8
```

검증:

- `Test-Path $scenario8DocumentPath`
- `Get-Content $scenario8DocumentPath -Raw`
- path가 project root 밖인지 확인
- git status에 `runtime_test_document.txt`가 나타나지 않는지 확인

주의:

- project root에 `runtime_test_document.txt`를 만들지 않는다.
- temp file cleanup은 Scenario 8A 실행 중 수행하지 않는다.
- cleanup 필요 여부는 result review에 기록한다.

## J. Scenario 8A Runtime Execution Steps

중요:
이 docs/146 생성 작업에서는 실행하지 않는다.
후속 사용자가 Scenario 8A execution을 별도 승인한 경우에만 수행한다.

실행 절차:

1. 앱 실행
2. MainWindow 표시 확인
3. Policy / Claim Management section 확인
4. policy title input에 `policy_title_scenario8_demo` 입력
5. Create policy 실행
6. active policy list에 `policy_title_scenario8_demo` 표시 확인
7. Document Registration target selection에서 policy target 선택
8. policy dropdown에서 `policy_title_scenario8_demo` 선택
9. Select File 실행
10. OpenFileDialog에서 오직 `%TEMP%\FamilyClaimRef\runtime_test_document.txt` 선택
11. document type은 existing synthetic-safe type 중 하나를 선택
12. Register 실행
13. registration success message 또는 expected success indicator 확인
14. 앱 종료

주의:

- claim 생성은 하지 않는다.
- claim target registration은 하지 않는다.
- 실제 파일 선택 위험이 있으면 즉시 중단한다.
- Register 실행 전 selected source file path가 approved synthetic file인지 확인한다.

## K. Expected Runtime Artifact Changes

Scenario 8A execution 후 expected changes:

- `policies.json` created or updated
- `documents.json` updated
- `policy-documents.json` updated
- copied attachment created under:

```text
%LOCALAPPDATA%\FamilyClaimRef\attachments\documents\<physicalFileName>
```

- `claims.json` remains missing unless app creates it indirectly; if created unexpectedly, record as note or possible issue
- `claim-documents.json` remains missing unless unexpected
- project root `attachments/`: files=0
- project root `data/local`: files=0
- temp synthetic file exists under `%TEMP%\FamilyClaimRef`

Expected no changes:

- source code files
- XAML files
- ViewModel files
- tests
- project root runtime artifacts
- DB/SQLite files

## L. Post-Run Checklist

실행 후 확인 항목:

1. app close 확인
2. `git status --short`
3. project root `attachments/` files count
4. project root `data/local` files count
5. project root `runtime_test_document.txt` absence
6. temp synthetic file existence and content
7. runtime root post-run snapshot
8. `policies.json` sanity
9. `documents.json` sanity
10. `policy-documents.json` sanity
11. copied attachment path sanity
12. `claims.json` status
13. `claim-documents.json` status
14. DB/SQLite unexpected file check
15. actual personal sample targeted scan
16. cleanup needed 여부 기록
17. cleanup performed: no

## M. Stop Criteria

아래 상황 발생 시 즉시 중단하고 result review에 BLOCKED로 기록한다.

- unexpected source tree changes before execution
- build/test failure not attributable to known Windows SDK permission issue
- project root `attachments/` files > 0 before or after execution
- project root `data/local` files > 0 before or after execution
- project root `runtime_test_document.txt` created
- temp synthetic file cannot be created
- temp synthetic file content mismatch
- app startup crash
- MainWindow binding failure
- policy creation failure
- approved synthetic file cannot be selected
- OpenFileDialog points to or selects real personal/insurance/medical/family document
- selected source file path is not `%TEMP%\FamilyClaimRef\runtime_test_document.txt`
- document registration failure
- copied attachment is created under project root `attachments/`
- metadata is created under project root `data/local`
- DB/SQLite unexpected file created
- actual personal sample appears in runtime artifact
- cleanup becomes necessary before evidence is recorded

## N. Result Review Document Requirement

후속 Scenario 8A 실행 후 반드시 생성할 문서:

```text
docs/147_POLICY_CLAIM_SCENARIO8_SYNTHETIC_DOCUMENT_REGISTRATION_RESULT_REVIEW.md
```

문서에 포함할 내용:

- Status Marker
  - `POLICY_CLAIM_SCENARIO8A_SYNTHETIC_POLICY_DOCUMENT_REGISTRATION_EXECUTED`
  - 또는 `POLICY_CLAIM_SCENARIO8A_SYNTHETIC_POLICY_DOCUMENT_REGISTRATION_BLOCKED`
- approval marker
- executed scenario: Scenario 8A policy target only
- skipped scenario: Scenario 8B claim target
- pre-run source status
- build/test result
- pre-run runtime snapshot
- synthetic temp file creation result
- app launch result
- policy creation result
- OpenFileDialog result
- selected file path
- document registration result
- post-run runtime snapshot
- policies.json sanity
- documents.json sanity
- policy-documents.json sanity
- copied attachment sanity
- claims.json status
- claim-documents.json status
- project root safety
- DB/SQLite check
- actual personal sample check
- cleanup performed: no
- cleanup needed 여부
- temp synthetic file cleanup needed 여부
- remaining risks
- next recommendation

## O. Explicit Non-Scope for This Documentation Task

이 docs/146 생성 작업에서 하지 않는 항목:

- app launch 없음
- OpenFileDialog 실행 없음
- synthetic test document 생성 없음
- runtime policy 생성 없음
- document registration workflow 실행 없음
- runtime artifact 생성 없음
- runtime artifact 삭제 없음
- temp file cleanup 없음
- code/XAML/ViewModel/test 수정 없음
- DB/SQLite/OCR/repository 구현 없음
- git add/commit/reset/checkout/clean 없음

## P. Verification for This Documentation Task

docs/146 생성 후 수행:

- `git diff --check`
- `git status --short`
- project root `attachments/` files count
- project root `data/local` files count
- project root `runtime_test_document.txt` absence
- `%TEMP%\FamilyClaimRef\runtime_test_document.txt` absence or not-created confirmation

build/test:

- documentation-only change이므로 실행하지 않는다.

## Q. Completion Report Format

완료 보고 형식:

```text
POLICY_CLAIM_SCENARIO8_SYNTHETIC_DOCUMENT_REGISTRATION_EXECUTION_INSTRUCTION_CREATED

생성 문서:
- docs/146_POLICY_CLAIM_SCENARIO8_SYNTHETIC_DOCUMENT_REGISTRATION_EXECUTION_INSTRUCTION.md

구현/실행 여부:
- code 수정 없음
- XAML 수정 없음
- ViewModel 수정 없음
- tests 수정 없음
- app launch 없음
- OpenFileDialog 없음
- synthetic test document 생성 없음
- document registration workflow 실행 없음
- runtime artifact 생성 없음
- cleanup 없음

execution instruction 요약:
- execution scope:
- approval marker:
- target type:
- synthetic file path:
- synthetic file content:
- pre-run checklist:
- runtime steps:
- expected artifacts:
- stop criteria:
- result review document:

검증 결과:
- git diff --check: PASS/FAIL
- git status --short: expected docs/145 and docs/146 only / unexpected
- project root attachments/: files=<count>
- project root data/local: files=<count>
- project root runtime_test_document.txt: missing/exists
- temp runtime_test_document.txt: missing/exists
- build/test: not run, documentation-only change

수정하지 않은 항목:
- AppServices 수정 없음
- DocumentLinkCoordinator 수정 없음
- DocumentRegistrationWorkflow 수정 없음
- MainWindow 수정 없음
- XAML 수정 없음
- ViewModel 수정 없음
- tests 수정 없음
- runtime artifact 생성 없음
- runtime artifact 삭제 없음
- project root cleanup 없음
- DB/SQLite/OCR/repository 구현 없음
- git add/commit/reset/checkout/clean 사용 없음

다음 추천 작업:
PHASE3D_SCENARIO8A_SYNTHETIC_POLICY_DOCUMENT_REGISTRATION_APPROVED 여부 사용자 승인
```
