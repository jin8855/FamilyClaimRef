# Policy / Claim Scenario 8A Allowed Extension Retry Result Review

## A. Status Marker

POLICY_CLAIM_SCENARIO8A_ALLOWED_EXTENSION_RETRY_EXECUTED

## B. Approval Marker

```text
PHASE3D_SCENARIO8A_ALLOWED_EXTENSION_SYNTHETIC_PNG_RETRY_APPROVED
```

## C. Purpose

Scenario 8A policy target synthetic document registration을 allowed-extension synthetic PNG로 재시도한 결과를 기록한다.

이번 실행은 `.txt` retry가 아니라 `FileNamePolicyService` allowlist 안의 `.png` synthetic file을 사용한 retry다.

## D. Execution Boundary

실행한 항목:

- app launch
- allowed-extension synthetic PNG creation
- existing active policy sanity check
- synthetic retry policy creation
- OpenFileDialog execution
- approved synthetic PNG selection only
- policy target selection
- document registration workflow execution
- runtime copied attachment verification
- `documents.json` sanity check
- `policy-documents.json` sanity check
- project root safety check
- result review document creation

실행하지 않은 항목:

- Scenario 8B claim target registration
- claim target registration
- `FileNamePolicyService` 수정
- allowlist 변경
- `.txt` retry
- PDF retry
- 실제 약관/계약서 사용
- `C:\EtcProject\FamilyClaimRef\data\claimdoc` 파일 사용/열람/목록화/선택
- actual personal/insurance/hospital/diagnosis document use
- cleanup
- `%LOCALAPPDATA%\FamilyClaimRef` deletion
- project root cleanup
- code/XAML/ViewModel/test modification
- DB/SQLite/OCR/repository implementation
- git add/reset/checkout/clean/commit

## E. Source Tree Pre-Status

Pre-run source status:

```text
?? data/
?? docs/145_POLICY_CLAIM_SCENARIO8_SYNTHETIC_DOCUMENT_REGISTRATION_DECISION.md
?? docs/146_POLICY_CLAIM_SCENARIO8_SYNTHETIC_DOCUMENT_REGISTRATION_EXECUTION_INSTRUCTION.md
?? docs/147_POLICY_CLAIM_SCENARIO8_SYNTHETIC_DOCUMENT_REGISTRATION_RESULT_REVIEW.md
?? docs/148_POLICY_CLAIM_SCENARIO8A_RETRY_POLICY_DECISION.md
?? docs/149_POLICY_CLAIM_LOCAL_DATA_CLAIMDOC_HANDLING_DECISION.md
?? docs/150_POLICY_CLAIM_SCENARIO8A_ALLOWED_EXTENSION_RETRY_EXECUTION_INSTRUCTION.md
```

`data/`는 known local excluded artifact로만 확인했다.

## F. Build Baseline

`dotnet build FamilyClaimRef.sln`:

```text
PASS
warning: 0
error: 0
```

`dotnet test`:

```text
not run
```

사유:

- 이번 승인 범위는 Scenario 8A allowed-extension synthetic PNG retry 실행이다.
- build는 최신 WPF 산출물과 source/runtime mismatch 가능성을 줄이기 위해 실행했다.
- test는 이번 승인 항목에 포함되지 않아 실행하지 않았다.

## G. Synthetic PNG

Approved synthetic file:

```text
%TEMP%\FamilyClaimRef\runtime_test_document.png
```

확인 결과:

- file created: PASS
- extension: `.png`
- file size: 68 bytes
- source path: `C:\Users\jin8855\AppData\Local\Temp\FamilyClaimRef\runtime_test_document.png`
- project root에 생성되지 않음
- 실제 개인정보, 보험사, 병원, 진단, 청구 데이터 없음
- 실제 약관/계약서 이미지 아님

기존 `.txt` evidence:

```text
%TEMP%\FamilyClaimRef\runtime_test_document.txt
```

상태:

- remains
- not retried
- not deleted

## H. data/claimdoc Exclusion

`C:\EtcProject\FamilyClaimRef\data\claimdoc`는 이번 retry에서 사용하지 않았다.

확인 기준:

- 파일 사용 없음
- 파일 열람 없음
- 파일 목록화 없음
- 파일명 수집 없음
- OpenFileDialog 선택 없음
- document registration input으로 사용 없음
- git staging 없음

`?? data/`는 expected-but-excluded 상태로만 기록한다.

## I. Active Policy Handling

초기 기대:

- 기존 active policy `policy_title_scenario8_demo` 재사용 가능 여부 확인

실제 진행:

- app UI에서 기존 active policy가 registration target으로 바로 선택되지 않았다.
- 승인 범위의 “필요 시 synthetic retry policy 생성” 경로를 사용했다.
- synthetic retry policy를 생성한 뒤 최신 WPF build 산출물 기준으로 다시 로드했다.

사용한 policy:

```text
displayTitle: policy_title_scenario8_retry_demo
policyId: policy_6d78f89825874c199292c2edf5c60eae
```

`policies.json` sanity:

```text
PASS
```

확인 내용:

- schemaVersion: 1
- item count: 1
- active policy count: 1
- displayTitle: `policy_title_scenario8_retry_demo`
- disabledAt: null

## J. OpenFileDialog Result

OpenFileDialog execution:

```text
PASS
```

Selected file:

```text
C:\Users\jin8855\AppData\Local\Temp\FamilyClaimRef\runtime_test_document.png
```

확인:

- approved temp PNG only
- `data/claimdoc` not selected
- actual document not selected
- `.txt` not selected
- PDF not selected

## K. Document Registration Result

Target:

```text
policy
```

Document type:

```text
capture
```

Display title:

```text
scenario8_policy_document_png_retry_demo
```

UI status:

```text
문서 등록이 완료되었습니다.
```

Last registration summary:

```text
policy:policy_6d78f89825874c199292c2edf5c60eae; document:doc_e867532a8fca4f13845289ce818bbc3f
```

Registration result:

```text
PASS
```

## L. Runtime Copied Attachment Verification

Runtime attachment:

```text
C:\Users\jin8855\AppData\Local\FamilyClaimRef\attachments\documents\policy-document_20260706_capture_001.png
```

확인 결과:

- copied attachment created: PASS
- extension: `.png`
- size: 68 bytes
- location: runtime attachment root
- project root attachment 생성 없음

## M. documents.json Sanity

확인 파일:

```text
%LOCALAPPDATA%\FamilyClaimRef\data\local\documents.json
```

확인 결과:

```text
PASS
```

확인 내용:

- schemaVersion: 1
- item count: 1
- document id: `doc_e867532a8fca4f13845289ce818bbc3f`
- physicalFileName: `policy-document_20260706_capture_001.png`
- displayTitle: `scenario8_policy_document_png_retry_demo`
- extension: `png`
- relativePath: `documents/policy-document_20260706_capture_001.png`
- disabledAt: null

## N. policy-documents.json Sanity

확인 파일:

```text
%LOCALAPPDATA%\FamilyClaimRef\data\local\policy-documents.json
```

확인 결과:

```text
PASS
```

확인 내용:

- schemaVersion: 1
- item count: 1
- policyDocument id: `pdoc_726bcb3bd3414e55b6663d908f1f4439`
- policyId: `policy_6d78f89825874c199292c2edf5c60eae`
- documentId: `doc_e867532a8fca4f13845289ce818bbc3f`
- documentType: `capture`
- disabledAt: null

## O. Claim Runtime Status

`claims.json`:

```text
missing
```

`claim-documents.json`:

```text
missing
```

판정:

```text
PASS
```

사유:

- 이번 retry target은 policy only다.
- claim target registration은 승인 범위 밖이다.

## P. Project Root Safety

Project root checks:

- `C:\EtcProject\FamilyClaimRef\attachments`: files=0
- `C:\EtcProject\FamilyClaimRef\data\local`: files=0
- `C:\EtcProject\FamilyClaimRef\runtime_test_document.*`: missing
- project root synthetic PNG created: no
- project root synthetic TXT created: no

판정:

```text
PASS
```

## Q. DB / SQLite / OCR Check

DB/SQLite unexpected file:

```text
NONE
```

OCR:

```text
not run
```

Repository/SQLite implementation:

```text
not implemented
```

## R. FileNamePolicyService / Allowlist Check

`FileNamePolicyService` changed:

```text
no
```

allowlist changed:

```text
no
```

Retry가 성공한 이유:

- `.txt`가 아니라 allowlist에 포함된 `.png` synthetic file을 사용했다.

## S. Read Context Note

초기 확인 중 일반 shell context에서 이전 runtime JSON snapshot이 보이는 현상이 있었다.

최종 metadata sanity는 app launch와 동일한 상승 실행 context에서 다시 확인했으며, `policies.json`, `documents.json`, `policy-documents.json` 모두 retry 결과와 일치했다.

이 항목은 cleanup 또는 구현 수정 없이 review note로만 기록한다.

## T. Cleanup Status

Cleanup performed:

```text
no
```

Remaining runtime artifacts:

- `%TEMP%\FamilyClaimRef\runtime_test_document.txt`
- `%TEMP%\FamilyClaimRef\runtime_test_document.png`
- `%LOCALAPPDATA%\FamilyClaimRef\data\local\policies.json`
- `%LOCALAPPDATA%\FamilyClaimRef\data\local\documents.json`
- `%LOCALAPPDATA%\FamilyClaimRef\data\local\policy-documents.json`
- `%LOCALAPPDATA%\FamilyClaimRef\attachments\documents\policy-document_20260706_capture_001.png`

Cleanup needed:

```text
yes, separate approval required
```

## U. Final Judgment

Scenario 8A allowed-extension synthetic PNG retry:

```text
PASS
```

최종 판정:

- `.txt` failure는 extension policy rejection 경로로 보는 판단을 유지한다.
- `.png` synthetic retry는 policy target document registration success path를 확인했다.
- OpenFileDialog, policy target selection, document registration workflow, copied attachment, documents metadata, policy-document link가 모두 확인됐다.
- claim target / Scenario 8B는 여전히 미실행이다.

## V. Remaining Risks

- runtime artifact cleanup이 아직 수행되지 않았다.
- 일반 shell context와 상승 execution context의 runtime JSON read 결과가 일시적으로 다르게 보인 이력이 있다.
- UI automation 기반 확인이므로 사람이 직접 화면을 보는 manual confirmation은 별도 수행 가능하다.
- claim target document registration은 아직 검증하지 않았다.

## W. Next Recommendation

다음 작업 후보:

1. Scenario 8A runtime artifacts targeted cleanup 범위 결정
2. Scenario 8B claim target synthetic PNG registration scope decision
3. 일반 shell context와 app execution context의 runtime JSON read mismatch 원인 검토
4. document registration success 이후 UI message / metadata sanity를 한 화면에서 확인할 수 있는 review helper 설계
