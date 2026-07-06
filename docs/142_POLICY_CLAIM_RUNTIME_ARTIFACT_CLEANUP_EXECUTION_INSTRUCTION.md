# Policy / Claim Runtime Artifact Cleanup Execution Instruction

## A. Status Marker

POLICY_CLAIM_RUNTIME_ARTIFACT_CLEANUP_EXECUTION_INSTRUCTION_CREATED

## B. Purpose

이 문서는 Phase 3D base runtime validation 이후 남은 policy/claim runtime artifacts를 targeted cleanup하기 위한 실행 지시서다.

이 문서 생성 작업에서는 cleanup을 실행하지 않는다.

실제 cleanup은 이 문서가 생성되고 사용자가 별도 승인한 뒤에만 수행한다.

cleanup 범위는 Option B targeted cleanup으로 제한한다.

cleanup 대상은 policies.json과 claims.json 두 파일뿐이다.

full runtime root cleanup은 금지한다.

Scenario 8은 이 cleanup execution instruction 범위가 아니다.

## C. Numbering Note

docs/141에서는 cleanup result review 예상 문서를 docs/142로 기록했다.

그러나 cleanup 실행 전 execution instruction 문서가 필요하므로 docs/142는 이 execution instruction으로 사용한다.

cleanup result review는 후속 cleanup 실행 후 docs/143_POLICY_CLAIM_RUNTIME_ARTIFACT_CLEANUP_RESULT_REVIEW.md로 생성한다.

기존 docs/141은 수정하지 않는다.

## D. Approved Cleanup Scope

Approved cleanup option:

```text
Option B: Targeted Cleanup of Phase 3D Base Policy / Claim Artifacts Only
```

Cleanup candidate exact paths:

- `%LOCALAPPDATA%\FamilyClaimRef\data\local\policies.json`
- `%LOCALAPPDATA%\FamilyClaimRef\data\local\claims.json`

Allowed cleanup action in future execution:

- 위 exact file paths가 존재할 경우 해당 파일만 삭제한다.

Not allowed:

- wildcard deletion
- recursive deletion
- directory deletion
- runtime root deletion
- project root cleanup
- any source tree cleanup

## E. Absolute Forbidden During Cleanup Execution

cleanup execution에서 금지할 항목:

- `%LOCALAPPDATA%\FamilyClaimRef` 전체 삭제 금지
- `%LOCALAPPDATA%\FamilyClaimRef\data\local` 전체 삭제 금지
- `%LOCALAPPDATA%\FamilyClaimRef\attachments` 전체 삭제 금지
- documents.json 삭제 금지
- policy-documents.json 삭제 금지
- claim-documents.json 삭제 금지
- attachments 폴더 삭제 금지
- runtime attachment 파일 삭제 금지
- project root attachments/ 삭제 금지
- project root data/local 삭제 금지
- wildcard deletion 금지
- recursive deletion 금지
- directory deletion 금지
- git clean 금지
- git reset 금지
- git checkout 금지
- app launch 금지
- OpenFileDialog 실행 금지
- Scenario 8 실행 금지
- runtime_test_document.txt 생성 금지
- registration workflow 실행 금지
- 코드/XAML/ViewModel/test 수정 금지
- DB/SQLite/OCR/repository 구현 금지

## F. Do-Not-Delete Paths

Do not delete:

- `%LOCALAPPDATA%\FamilyClaimRef\data\local\documents.json`
- `%LOCALAPPDATA%\FamilyClaimRef\data\local\policy-documents.json`
- `%LOCALAPPDATA%\FamilyClaimRef\data\local\claim-documents.json`
- `%LOCALAPPDATA%\FamilyClaimRef\attachments`
- `%LOCALAPPDATA%\FamilyClaimRef\attachments\documents\policy-document_20260702_policy_001.png`
- `C:\EtcProject\FamilyClaimRef\attachments`
- `C:\EtcProject\FamilyClaimRef\data\local`

Notes:

- claim-documents.json은 missing일 수 있으나, 존재하더라도 cleanup 대상이 아니다.
- documents.json, policy-documents.json, runtime attachment는 pre-existing evidence로 본다.

## G. Pre-Cleanup Checklist

cleanup 실행 전 반드시 수행할 절차:

1. 프로젝트 루트로 이동한다.

```text
cd C:\EtcProject\FamilyClaimRef
```

2. git 상태를 확인한다.

```text
git status --short
git log -1 --oneline
```

기대 latest commit:

```text
b58155d feat(familyclaimref): add policy claim management UI
```

기대 git status:

```text
docs/136~142 only
```

또는 docs/136~142가 이미 commit된 경우 clean이어도 된다.

주의:

- unexpected source tree change가 있으면 cleanup 실행 금지.
- reset/checkout/clean으로 정리하지 않는다.

3. project root safety를 확인한다.

확인:

- `C:\EtcProject\FamilyClaimRef\attachments` files count
- `C:\EtcProject\FamilyClaimRef\data\local` files count

기대:

```text
project root attachments/: files=0
project root data/local: files=0
```

4. runtime root snapshot을 기록한다.

PowerShell 후보:

```powershell
$runtimeRoot = Join-Path $env:LOCALAPPDATA 'FamilyClaimRef'
$metadataRoot = Join-Path $runtimeRoot 'data\local'
$attachmentsRoot = Join-Path $runtimeRoot 'attachments'

$policiesPath = Join-Path $metadataRoot 'policies.json'
$claimsPath = Join-Path $metadataRoot 'claims.json'
$documentsPath = Join-Path $metadataRoot 'documents.json'
$policyDocumentsPath = Join-Path $metadataRoot 'policy-documents.json'
$claimDocumentsPath = Join-Path $metadataRoot 'claim-documents.json'

Test-Path $runtimeRoot
Test-Path $metadataRoot
Test-Path $attachmentsRoot
Get-ChildItem -LiteralPath $runtimeRoot -Recurse -File -ErrorAction SilentlyContinue |
    Select-Object FullName, Length, LastWriteTime
```

5. exact target existence를 기록한다.

확인 대상:

- `$policiesPath`
- `$claimsPath`

기록할 값:

- exists / missing
- file length
- last write time
- 가능한 경우 item count
- 가능한 경우 id / title / disabledAt sanity

6. do-not-delete path preservation 상태를 기록한다.

확인 대상:

- `$documentsPath`
- `$policyDocumentsPath`
- `$claimDocumentsPath`
- `$attachmentsRoot`
- `%LOCALAPPDATA%\FamilyClaimRef\attachments\documents\policy-document_20260702_policy_001.png`

주의:

- missing인 do-not-delete path는 missing으로 기록만 한다.
- missing인 do-not-delete path를 생성하지 않는다.

7. DB/SQLite unexpected file을 확인한다.

확인 범위:

- `C:\EtcProject\FamilyClaimRef`
- `%LOCALAPPDATA%\FamilyClaimRef`

기대:

```text
DB/SQLite unexpected file: none
```

## H. Cleanup Execution Steps for Future Approval

이 섹션은 후속 작업에서 사용자가 cleanup 실행을 별도 승인한 경우에만 수행한다.

1. Pre-Cleanup Checklist를 모두 완료한다.

2. 승인 범위가 Option B targeted cleanup인지 다시 확인한다.

3. `$policiesPath`가 존재하면 해당 exact file path만 삭제한다.

4. `$claimsPath`가 존재하면 해당 exact file path만 삭제한다.

5. 삭제 중 오류가 발생하면 즉시 중단하고, 추가 삭제를 수행하지 않는다.

6. wildcard, recursive, directory deletion을 사용하지 않는다.

7. `%LOCALAPPDATA%\FamilyClaimRef`, `%LOCALAPPDATA%\FamilyClaimRef\data\local`, `%LOCALAPPDATA%\FamilyClaimRef\attachments` 디렉터리는 삭제하지 않는다.

8. project root `attachments/`, project root `data/local`은 삭제하지 않는다.

권장 PowerShell 형태:

```powershell
if (Test-Path -LiteralPath $policiesPath) {
    Remove-Item -LiteralPath $policiesPath -Force
}

if (Test-Path -LiteralPath $claimsPath) {
    Remove-Item -LiteralPath $claimsPath -Force
}
```

안전 조건:

- 위 후보는 문서화용이며, 실제 실행 전 사용자의 별도 승인이 필요하다.
- `Remove-Item` 대상은 `$policiesPath`, `$claimsPath` 두 exact paths로만 제한한다.
- `-Recurse`는 사용하지 않는다.
- wildcard path는 사용하지 않는다.

## I. Post-Cleanup Checklist

cleanup 실행 후 반드시 확인할 항목:

1. exact target deletion result

- `%LOCALAPPDATA%\FamilyClaimRef\data\local\policies.json`: missing expected
- `%LOCALAPPDATA%\FamilyClaimRef\data\local\claims.json`: missing expected

2. do-not-delete path preservation

- `%LOCALAPPDATA%\FamilyClaimRef\data\local\documents.json`
- `%LOCALAPPDATA%\FamilyClaimRef\data\local\policy-documents.json`
- `%LOCALAPPDATA%\FamilyClaimRef\data\local\claim-documents.json`
- `%LOCALAPPDATA%\FamilyClaimRef\attachments`
- `%LOCALAPPDATA%\FamilyClaimRef\attachments\documents\policy-document_20260702_policy_001.png`
- `C:\EtcProject\FamilyClaimRef\attachments`
- `C:\EtcProject\FamilyClaimRef\data\local`

3. project root safety

기대:

```text
project root attachments/: files=0
project root data/local: files=0
```

4. runtime root post-cleanup snapshot

기록:

- runtime root exists / missing
- metadata root exists / missing
- attachments root exists / missing
- runtime file list
- DB/SQLite unexpected file
- actual personal sample check

5. git 상태 확인

```text
git status --short
```

기대:

```text
source tree unexpected modification 없음
```

## J. Stop Criteria

다음 상황이 발생하면 즉시 중단하고 cleanup result review에 기록한다.

- unexpected source tree change 발견
- `git status --short`에 예상하지 않은 code/XAML/test 변경 발견
- project root `attachments/`에 파일 발견
- project root `data/local`에 파일 발견
- DB/SQLite unexpected file 발견
- actual personal sample 가능성 발견
- 삭제 대상이 `$policiesPath`, `$claimsPath` 외 경로로 확장될 위험 발견
- wildcard deletion이 필요해지는 상황 발생
- recursive deletion이 필요해지는 상황 발생
- directory deletion이 필요해지는 상황 발생
- do-not-delete path 삭제 위험 발견
- documents.json 삭제 위험 발견
- policy-documents.json 삭제 위험 발견
- claim-documents.json 삭제 위험 발견
- runtime attachment 삭제 위험 발견
- deletion command 실패 후 상태가 불명확한 경우
- app launch가 필요한 상황 발생
- OpenFileDialog 실행이 필요한 상황 발생
- Scenario 8 실행이 필요한 상황 발생

중단 시 원칙:

- 추가 cleanup을 수행하지 않는다.
- reset/checkout/clean으로 정리하지 않는다.
- 결과를 docs/143 result review 후보에 기록한다.

## K. Cleanup Result Review Requirement

cleanup 실행 후 result review 문서를 생성한다.

Expected document:

```text
docs/143_POLICY_CLAIM_RUNTIME_ARTIFACT_CLEANUP_RESULT_REVIEW.md
```

cleanup result review에 포함할 항목:

- Status Marker
- cleanup approval source
- cleanup option
- cleanup exact target paths
- pre-cleanup snapshot
- deleted exact paths
- deletion success / failure
- skipped target paths, if missing
- deletion error, if any
- post-cleanup snapshot
- do-not-delete path preservation
- project root `attachments/` / `data/local` safety
- DB/SQLite unexpected file check
- actual personal sample check
- cleanup performed: yes
- cleanup scope: targeted only
- Scenario 8 executed: no
- OpenFileDialog executed: no
- app launch executed: no
- code/XAML/ViewModel/test modified: no
- remaining risks
- next recommendation

## L. Explicit Non-Scope for This Documentation Task

이 docs/142 생성 작업에서 수행하지 않는 항목:

- cleanup 실행 없음
- runtime artifact 삭제 없음
- runtime JSON 수정 없음
- `%LOCALAPPDATA%\FamilyClaimRef` 삭제 없음
- `%LOCALAPPDATA%\FamilyClaimRef\data\local` 삭제 없음
- `%LOCALAPPDATA%\FamilyClaimRef\attachments` 삭제 없음
- policies.json 삭제 없음
- claims.json 삭제 없음
- documents.json 삭제 없음
- policy-documents.json 삭제 없음
- claim-documents.json 삭제 없음
- runtime attachment 삭제 없음
- project root cleanup 없음
- app launch 없음
- OpenFileDialog 실행 없음
- Scenario 8 실행 없음
- synthetic test document 생성 없음
- runtime_test_document.txt 생성 없음
- registration workflow 실행 없음
- 코드 수정 없음
- XAML 수정 없음
- ViewModel 수정 없음
- AppServices 수정 없음
- DocumentLinkCoordinator 수정 없음
- DocumentRegistrationWorkflow 수정 없음
- 테스트 수정 없음
- DB/SQLite/OCR/repository 구현 없음
- git add / commit / reset / checkout / clean 없음

## M. Verification for This Documentation Task

docs/142 생성 후 확인할 항목:

- `docs/142_POLICY_CLAIM_RUNTIME_ARTIFACT_CLEANUP_EXECUTION_INSTRUCTION.md` 생성
- 기존 docs/136~141 미수정
- code/XAML/ViewModel/test 미수정
- cleanup 미실행
- app launch 미실행
- OpenFileDialog 미실행
- Scenario 8 미실행
- runtime artifact 삭제 없음
- project root attachments/ files count
- project root data/local files count
- DB/SQLite unexpected file 없음
- `git diff --check` PASS
- `git status --short`가 docs/136~142 only 상태인지 확인

build/test:

- documentation-only change이므로 실행하지 않는다.

## N. Completion Report Format

완료 보고 형식:

```text
POLICY_CLAIM_RUNTIME_ARTIFACT_CLEANUP_EXECUTION_INSTRUCTION_CREATED

생성 문서:
- docs/142_POLICY_CLAIM_RUNTIME_ARTIFACT_CLEANUP_EXECUTION_INSTRUCTION.md

분석 대상:
- docs/136_POLICY_CLAIM_RUNTIME_MANUAL_VALIDATION_SCOPE_DESIGN.md
- docs/137_POLICY_CLAIM_RUNTIME_MANUAL_VALIDATION_PHASE3D_USER_DECISION_RECORD.md
- docs/138_POLICY_CLAIM_RUNTIME_MANUAL_VALIDATION_PHASE3D_EXECUTION_INSTRUCTION.md
- docs/139_POLICY_CLAIM_RUNTIME_MANUAL_VALIDATION_PHASE3D_RESULT_REVIEW.md
- docs/140_POLICY_CLAIM_RUNTIME_ARTIFACT_CLEANUP_SCOPE_DESIGN.md
- docs/141_POLICY_CLAIM_RUNTIME_ARTIFACT_CLEANUP_USER_DECISION_RECORD.md
- app/FamilyClaimRef.App/Composition/AppServices.cs
- app/FamilyClaimRef.App/Services/Storage/JsonPolicyClaimStorageService.cs
- app/FamilyClaimRef.App/Services/Storage/DocumentRegistrationWorkflow.cs
- app/FamilyClaimRef.App/Services/Storage/DocumentLinkCoordinator.cs

구현/실행 여부:
- 코드 수정 없음
- XAML 수정 없음
- ViewModel 수정 없음
- 테스트 수정 없음
- cleanup 실행 없음
- app launch 없음
- OpenFileDialog 실행 없음
- Scenario 8 실행 없음
- runtime workflow 실행 없음
- runtime artifact 삭제 없음
- %LOCALAPPDATA%\FamilyClaimRef 삭제 없음

execution instruction 요약:
- cleanup option:
- cleanup exact target paths:
- do-not-delete paths:
- pre-cleanup checklist:
- post-cleanup checklist:
- stop criteria:
- result review document:

검증 결과:
- git diff --check: PASS/FAIL
- git status --short: expected docs/136~142 only / unexpected
- project root attachments/: files=<count>
- project root data/local: files=<count>
- DB/SQLite unexpected file:
- build/test: not run, documentation-only change

수정하지 않은 항목:
- 기존 docs/136~141 수정 없음
- AppServices 수정 없음
- DocumentLinkCoordinator 수정 없음
- DocumentRegistrationWorkflow 수정 없음
- MainWindow 수정 없음
- XAML 수정 없음
- ViewModel 수정 없음
- tests 수정 없음
- runtime artifact 삭제 없음
- project root cleanup 없음
- DB/SQLite/OCR/repository 구현 없음
- git add/commit/reset/checkout/clean 사용 없음

다음 추천 작업:
- 사용자가 별도 승인하면 docs/142 기준으로 Option B targeted cleanup 실행
- cleanup 실행 후 docs/143_POLICY_CLAIM_RUNTIME_ARTIFACT_CLEANUP_RESULT_REVIEW.md 생성
```
