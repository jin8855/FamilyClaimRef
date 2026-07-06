# Policy / Claim Scenario 8A Allowed Extension Retry Execution Instruction

## A. Status Marker

POLICY_CLAIM_SCENARIO8A_ALLOWED_EXTENSION_RETRY_EXECUTION_INSTRUCTION_CREATED

## B. Purpose

- 이 문서는 Scenario 8A policy target synthetic document registration을 allowed extension synthetic PNG로 다시 실행하기 위한 실행 지시서다.
- 이전 Scenario 8A는 `.txt` 파일을 선택했고 document registration 단계에서 BLOCKED 되었다.
- likely cause는 `FileNamePolicyService` allowlist가 `pdf`, `jpg`, `jpeg`, `png`만 허용하고 `.txt`를 허용하지 않는다는 점이다.
- 이 instruction은 `FileNamePolicyService`를 수정하지 않는다.
- 이 instruction은 allowlist 안의 synthetic PNG를 사용해 success path를 검증하는 retry 실행 절차만 정의한다.
- 이 문서 생성 작업 자체에서는 app launch, OpenFileDialog, PNG 생성, retry execution을 수행하지 않는다.
- 실제 retry는 이 문서 생성 후 사용자가 별도로 승인한 경우에만 수행한다.

## C. Numbering Note

- `docs/149_POLICY_CLAIM_LOCAL_DATA_CLAIMDOC_HANDLING_DECISION.md`는 `data/claimdoc` handling decision으로 사용되었다.
- 이 retry execution instruction은 `docs/150_POLICY_CLAIM_SCENARIO8A_ALLOWED_EXTENSION_RETRY_EXECUTION_INSTRUCTION.md`로 생성한다.
- 후속 retry result review는 `docs/151_POLICY_CLAIM_SCENARIO8A_ALLOWED_EXTENSION_RETRY_RESULT_REVIEW.md`로 생성한다.
- 기존 `docs/145`~`docs/149`는 수정하지 않는다.

## D. Approval Gate

후속 실제 retry 실행에 필요한 승인 marker:

```text
PHASE3D_SCENARIO8A_ALLOWED_EXTENSION_SYNTHETIC_PNG_RETRY_APPROVED
```

승인 범위:

- app launch
- allowed-extension synthetic PNG creation
- existing active policy sanity check
- runtime policy reuse 또는 필요 시 synthetic retry policy 생성
- OpenFileDialog execution
- approved synthetic PNG selection only
- policy target selection
- document registration workflow execution
- runtime copied attachment verification
- `documents.json` sanity check
- `policy-documents.json` sanity check
- project root safety check
- result review document creation

계속 금지:

- Scenario 8B claim target registration
- claim target registration
- `FileNamePolicyService` 수정
- allowlist 변경
- `.txt` retry
- PDF retry
- 실제 약관/계약서 사용
- `data/claimdoc` 파일 사용
- actual personal/insurance/hospital/diagnosis document use
- cleanup
- `%LOCALAPPDATA%\FamilyClaimRef` deletion
- project root cleanup
- code/XAML/ViewModel/test modification
- DB/SQLite/OCR/repository implementation

## E. Execution Scope

Retry execution scope:

- Scenario 8A policy target only
- allowed extension synthetic PNG only
- no claim creation unless policy reuse is impossible and claim remains unnecessary
- no claim target registration
- no Scenario 8B
- no production code change
- no file allowlist change
- no cleanup
- `data/claimdoc` not used

Expected retry flow:

1. pre-run source and runtime snapshot
2. build/test baseline
3. confirm or create temp synthetic PNG outside project root
4. app launch
5. reuse existing active policy if safe, or create `policy_title_scenario8_retry_demo`
6. select synthetic PNG through OpenFileDialog
7. select policy target
8. select existing synthetic-safe document type
9. run document registration
10. verify copied attachment under runtime attachment root
11. verify `documents.json` update
12. verify `policy-documents.json` update
13. verify project root remains clean
14. create result review document
15. no cleanup

## F. Current Known State After Blocked Scenario 8A

`docs/147_POLICY_CLAIM_SCENARIO8_SYNTHETIC_DOCUMENT_REGISTRATION_RESULT_REVIEW.md` 기준 current state:

- app launch: PASS
- MainWindow display: PASS
- temp `.txt` synthetic document creation: PASS
- runtime synthetic policy creation: PASS
- OpenFileDialog execution: PASS
- approved `.txt` file selection: PASS
- policy target selection: PASS
- document registration workflow reached registration step
- registration result: BLOCKED
- likely cause: `.txt` extension not allowed
- `documents.json` unchanged
- `policy-documents.json` unchanged
- no new copied attachment
- `policies.json` exists with active synthetic policy
- `claims.json` missing
- `claim-documents.json` missing
- temp `runtime_test_document.txt` exists
- project root `attachments/`: files=0
- project root `data/local`: files=0
- project root `runtime_test_document.txt`: missing
- known local excluded artifact: `?? data/`

## G. Actual Contract / Terms File Exclusion

User reported:

```text
C:\EtcProject\FamilyClaimRef\data\claimdoc contains 약관/계약서.
```

Decision:

- 해당 경로의 파일은 Scenario 8A retry에서 사용하지 않는다.
- 해당 파일은 실제 약관/계약서일 가능성이 있으므로 forbidden source로 본다.
- OpenFileDialog에서 해당 경로 또는 파일이 보이면 선택하지 않는다.
- 해당 파일을 등록하지 않는다.
- 해당 파일 내용을 열람, OCR, 복사, 분석하지 않는다.
- 해당 파일 목록이나 파일명도 수집하지 않는다.
- 실제 약관/계약서 파일은 synthetic validation에 사용하지 않는다.
- `data/` is expected-but-excluded in git status.
- `data/` is never staged or committed.

Stop criteria:

- OpenFileDialog가 `C:\EtcProject\FamilyClaimRef\data\claimdoc`로 이동하거나 해당 파일 선택이 필요한 상황
- 실수로 해당 경로의 파일을 선택하려는 상황
- 해당 파일이 workflow input으로 들어가는 상황
- `data/`가 staged 상태로 표시되는 상황

## H. Retry Synthetic File

Approved retry file:

```text
%TEMP%\FamilyClaimRef\runtime_test_document.png
```

File type:

- minimal valid PNG binary
- allowed extension under current `FileNamePolicyService` policy
- no personal/insurance/hospital/diagnosis content
- no screenshot
- no real document image
- no actual insurance/medical/contract image

Recommended PNG generation:

- Use script-created minimal 1x1 PNG.
- Store only under `%TEMP%\FamilyClaimRef`.
- Do not create project root PNG.
- Do not use real image.
- Do not use image from `data/claimdoc`.

Execution-time PowerShell candidate:

```powershell
$scenario8TempRoot = Join-Path $env:TEMP 'FamilyClaimRef'
$scenario8PngPath = Join-Path $scenario8TempRoot 'runtime_test_document.png'
New-Item -ItemType Directory -Path $scenario8TempRoot -Force | Out-Null

$pngBase64 = 'iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mP8/x8AAwMCAO+/p9sAAAAASUVORK5CYII='
[IO.File]::WriteAllBytes($scenario8PngPath, [Convert]::FromBase64String($pngBase64))
```

Verification:

- `Test-Path $scenario8PngPath`
- file extension is `.png`
- file is under `%TEMP%\FamilyClaimRef`
- file is not under project root
- git status does not show `runtime_test_document.png`
- file size > 0

주의:

- 이 `docs/150` 생성 작업에서는 PNG를 만들지 않는다.
- 후속 승인된 retry execution에서만 생성한다.

## I. Existing Runtime Artifacts Policy

Current remaining artifacts:

- `%LOCALAPPDATA%\FamilyClaimRef\data\local\policies.json`
- `%TEMP%\FamilyClaimRef\runtime_test_document.txt`
- pre-existing `documents.json`
- pre-existing `policy-documents.json`
- pre-existing runtime attachment

Policy:

- retry execution does not cleanup these artifacts.
- `.txt` file remains evidence.
- `policies.json` active policy may be reused if sanity check passes.
- If `policies.json` does not contain exactly one active policy with displayTitle `policy_title_scenario8_demo`, create a new `policy_title_scenario8_retry_demo`.
- Do not create claim.
- Do not cleanup before retry.
- Cleanup needed 여부는 retry result review에 기록한다.

## J. Source Tree Status Policy

후속 retry execution에서 expected source status includes:

- `?? data/`
- `?? docs/145_POLICY_CLAIM_SCENARIO8_SYNTHETIC_DOCUMENT_REGISTRATION_DECISION.md`
- `?? docs/146_POLICY_CLAIM_SCENARIO8_SYNTHETIC_DOCUMENT_REGISTRATION_EXECUTION_INSTRUCTION.md`
- `?? docs/147_POLICY_CLAIM_SCENARIO8_SYNTHETIC_DOCUMENT_REGISTRATION_RESULT_REVIEW.md`
- `?? docs/148_POLICY_CLAIM_SCENARIO8A_RETRY_POLICY_DECISION.md`
- `?? docs/149_POLICY_CLAIM_LOCAL_DATA_CLAIMDOC_HANDLING_DECISION.md`
- `?? docs/150_POLICY_CLAIM_SCENARIO8A_ALLOWED_EXTENSION_RETRY_EXECUTION_INSTRUCTION.md`

Policy:

- `data/` is expected-but-excluded.
- `data/` must not be staged.
- `data/` must not be committed.
- docs only are eligible for later exact-file commit.
- if `data/` appears in staged diff, stop immediately.
- do not use reset/checkout/clean without separate instruction.

## K. Pre-Run Checklist For Future Approved Retry

1. 프로젝트 루트 이동

```powershell
cd C:\EtcProject\FamilyClaimRef
```

2. source tree 상태 확인

```powershell
git status --short
git log -1 --oneline
```

기대 latest commit:

```text
58f891a docs(familyclaimref): add runtime validation cleanup review
```

기대 status:

```text
?? data/
?? docs/145...
?? docs/146...
?? docs/147...
?? docs/148...
?? docs/149...
?? docs/150...
```

- unexpected source tree change가 있으면 실행 중단.
- `data/`는 expected-but-excluded로만 인정한다.
- `data/`가 staged 상태이면 실행 중단.
- reset/checkout/clean으로 정리하지 않는다.

3. build/test baseline 확인

```powershell
dotnet build FamilyClaimRef.sln
dotnet test FamilyClaimRef.sln
```

주의:

- Windows SDK permission issue가 발생하면 권한 상승 build/test 가능.
- 권한 상승 필요 여부는 result review에 기록한다.

4. project root safety pre-check

확인:

- project root `attachments/`: files=0
- project root `data/local`: files=0
- project root `runtime_test_document.txt` missing
- project root `runtime_test_document.png` missing
- `data/claimdoc` is not selected, not inspected, not used

5. temp file pre-check

확인:

- `%TEMP%\FamilyClaimRef\runtime_test_document.txt` status
- `%TEMP%\FamilyClaimRef\runtime_test_document.png` status

주의:

- existing `.txt` is evidence; do not delete.
- existing `.png`가 있으면 minimal PNG 여부와 path를 확인한다.
- `.png` content가 approved generated PNG가 아니면 retry를 중단하거나 overwrite 여부를 별도 지시로 명확히 기록한다.
- cleanup은 수행하지 않는다.

6. runtime root pre-run snapshot

기록:

- runtime root exists
- metadata root exists
- attachments root exists
- `policies.json` exists / missing
- `claims.json` exists / missing
- `documents.json` exists / missing
- `policy-documents.json` exists / missing
- `claim-documents.json` exists / missing
- runtime attachment list
- DB/SQLite unexpected file check
- actual personal sample targeted scan

7. active policy sanity

If `policies.json` exists:

- item count
- active policy count
- displayTitle values
- disabledAt values

Reuse rule:

- If exactly one active policy exists with displayTitle `policy_title_scenario8_demo`, reuse it.
- Otherwise create `policy_title_scenario8_retry_demo` during retry execution.
- Do not create claim.

## L. Runtime Retry Steps For Future Approved Execution

중요:

- 이 `docs/150` 생성 작업에서는 실행하지 않는다.
- 후속 사용자가 `PHASE3D_SCENARIO8A_ALLOWED_EXTENSION_SYNTHETIC_PNG_RETRY_APPROVED`를 승인한 경우에만 수행한다.

1. synthetic PNG 생성 또는 확인

- `%TEMP%\FamilyClaimRef\runtime_test_document.png`
- approved minimal PNG only
- project root에 만들지 않음

2. app launch

- MainWindow 표시 확인
- Policy/Claim Management section 표시 확인
- Document Registration section 표시 확인

3. active policy 준비

- reuse rule에 따라 `policy_title_scenario8_demo` 재사용
- reuse 불가 시 `policy_title_scenario8_retry_demo` 생성
- active policy list는 document registration policy dropdown에 반영 확인

4. document registration 준비

- target kind: policy
- policy dropdown: approved synthetic policy 선택
- document type: `capture` 또는 existing synthetic-safe type 선택
- display title: `scenario8_policy_document_png_retry_demo`
- reference date: current app default 또는 synthetic-safe value
- Select File 실행
- OpenFileDialog에서 오직 `%TEMP%\FamilyClaimRef\runtime_test_document.png` 선택

5. document registration 실행

- Register 실행
- success message 또는 expected success indicator 확인

6. app close

- process 종료 확인

## M. Expected Retry Runtime Artifacts

If retry succeeds:

- `policies.json` exists
- `documents.json` updated with new document record
- `policy-documents.json` updated with new policy-document link
- copied attachment created under `%LOCALAPPDATA%\FamilyClaimRef\attachments\documents`
- copied attachment extension likely `.png`
- `claims.json` remains missing unless unexpected
- `claim-documents.json` remains missing unless unexpected
- temp `.txt` remains
- temp `.png` remains
- project root `attachments/`: files=0
- project root `data/local`: files=0
- project root `runtime_test_document.*` missing

If retry fails:

- result review records failure point
- no cleanup
- project root remains clean

## N. Stop Criteria

중단 조건:

- unexpected source tree change
- `data/` appears staged
- user or OpenFileDialog attempts to select `C:\EtcProject\FamilyClaimRef\data\claimdoc` file
- project root `attachments/` files > 0
- project root `data/local` files > 0
- project root `runtime_test_document.*` created
- temp PNG cannot be created
- temp PNG not under `%TEMP%\FamilyClaimRef`
- temp PNG invalid or size=0
- app startup crash
- policy target unavailable and policy creation fails
- OpenFileDialog selects anything except approved temp PNG
- selected file path is not `%TEMP%\FamilyClaimRef\runtime_test_document.png`
- document registration fails again
- copied attachment created under project root
- metadata created under project root
- DB/SQLite unexpected file created
- actual personal/insurance/hospital/diagnosis sample detected

## O. Result Review Requirement

후속 retry 실행 후 반드시 생성할 문서:

```text
docs/151_POLICY_CLAIM_SCENARIO8A_ALLOWED_EXTENSION_RETRY_RESULT_REVIEW.md
```

포함할 내용:

- Status Marker
  - `POLICY_CLAIM_SCENARIO8A_ALLOWED_EXTENSION_RETRY_EXECUTED`
  - 또는 `POLICY_CLAIM_SCENARIO8A_ALLOWED_EXTENSION_RETRY_BLOCKED`
- approval marker
- retry option: synthetic PNG
- `FileNamePolicyService` changed: no
- allowlist changed: no
- source tree pre-status
- build/test baseline
- project root pre/post safety
- temp `.txt` status
- temp `.png` creation result
- `data/claimdoc` exclusion confirmation
- runtime pre snapshot
- active policy reuse or retry policy creation
- OpenFileDialog result
- selected file path
- document registration result
- copied attachment sanity
- `documents.json` sanity
- `policy-documents.json` sanity
- `claims.json` status
- `claim-documents.json` status
- DB/SQLite check
- actual personal sample check
- cleanup performed: no
- cleanup needed 여부
- remaining risks
- next recommendation

## P. Explicit Non-Scope For This Documentation Task

이 `docs/150` 생성 작업에서 하지 않는 항목:

- app launch 없음
- OpenFileDialog 없음
- retry 실행 없음
- synthetic PNG 생성 없음
- runtime policy 생성 없음
- document registration workflow 실행 없음
- cleanup 없음
- code/XAML/ViewModel/test 수정 없음
- `FileNamePolicyService` 수정 없음
- allowlist 변경 없음
- `data/claimdoc` 파일 사용 없음
- DB/SQLite/OCR/repository 구현 없음
- git add/commit/reset/checkout/clean 없음

## Q. Verification For This Documentation Task

`docs/150` 생성 후 수행:

- `git diff --check`
- `git status --short`
- expected:
  - `?? data/`
  - `?? docs/145...`
  - `?? docs/146...`
  - `?? docs/147...`
  - `?? docs/148...`
  - `?? docs/149...`
  - `?? docs/150...`
- project root `attachments/` files count
- project root `data/local` files count
- project root `runtime_test_document.*` absence
- `%TEMP%\FamilyClaimRef\runtime_test_document.png` absence or not-created confirmation
- DB/SQLite unexpected file check

build/test:

- documentation-only change이므로 실행하지 않는다.

## R. Completion Report Format

완료 보고 형식:

```md
POLICY_CLAIM_SCENARIO8A_ALLOWED_EXTENSION_RETRY_EXECUTION_INSTRUCTION_CREATED

생성 문서:
- docs/150_POLICY_CLAIM_SCENARIO8A_ALLOWED_EXTENSION_RETRY_EXECUTION_INSTRUCTION.md

구현/실행 여부:
- code 수정 없음
- XAML 수정 없음
- ViewModel 수정 없음
- tests 수정 없음
- app launch 없음
- OpenFileDialog 없음
- retry 실행 없음
- synthetic PNG 생성 없음
- cleanup 없음

execution instruction 요약:
- retry option:
- approval marker:
- synthetic PNG path:
- actual contract file exclusion:
- active policy strategy:
- OpenFileDialog policy:
- expected artifacts:
- stop criteria:
- result review document:

검증 결과:
- git diff --check: PASS/FAIL
- git status --short: expected docs/145~150 plus excluded data/ / unexpected
- project root attachments/: files=<count>
- project root data/local: files=<count>
- project root runtime_test_document.*: missing/exists
- temp runtime_test_document.png: missing/exists
- build/test: not run, documentation-only change

수정하지 않은 항목:
- AppServices 수정 없음
- DocumentLinkCoordinator 수정 없음
- DocumentRegistrationWorkflow 수정 없음
- MainWindow 수정 없음
- XAML 수정 없음
- ViewModel 수정 없음
- tests 수정 없음
- FileNamePolicyService 수정 없음
- allowlist 변경 없음
- runtime artifact 생성 없음
- runtime artifact 삭제 없음
- project root cleanup 없음
- data/claimdoc 파일 사용 없음
- DB/SQLite/OCR/repository 구현 없음
- git add/commit/reset/checkout/clean 사용 없음

다음 추천 작업:
- PHASE3D_SCENARIO8A_ALLOWED_EXTENSION_SYNTHETIC_PNG_RETRY_APPROVED 여부 사용자 승인
```
