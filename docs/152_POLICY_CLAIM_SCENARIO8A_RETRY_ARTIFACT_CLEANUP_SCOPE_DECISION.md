# Policy / Claim Scenario 8A Retry Artifact Cleanup Scope Decision

## A. Status Marker

POLICY_CLAIM_SCENARIO8A_RETRY_ARTIFACT_CLEANUP_SCOPE_DECISION_RECORDED

## B. Decision Context

- Scenario 8A initial `.txt` execution was BLOCKED due to extension policy.
- Scenario 8A allowed-extension PNG retry was EXECUTED successfully.
- app launch: PASS.
- synthetic PNG creation: PASS.
- OpenFileDialog selected approved PNG only.
- policy target registration: PASS.
- UI status: `문서 등록이 완료되었습니다.`
- `documents.json` was updated with the new document.
- `policy-documents.json` was updated with the new policy-document link.
- copied attachment was created under runtime attachment root.
- `data/claimdoc` was not used, inspected, listed, or selected.
- `FileNamePolicyService` and allowlist were not changed.
- cleanup was not performed.
- runtime/temp artifacts remain and require separate cleanup decision.

## C. Current Artifact State

`docs/151_POLICY_CLAIM_SCENARIO8A_ALLOWED_EXTENSION_RETRY_RESULT_REVIEW.md` 기준 current artifacts를 정리한다.

Temp artifacts:

- `%TEMP%\FamilyClaimRef\runtime_test_document.txt`
- `%TEMP%\FamilyClaimRef\runtime_test_document.png`

Runtime artifacts:

- `%LOCALAPPDATA%\FamilyClaimRef\data\local\policies.json`
- `%LOCALAPPDATA%\FamilyClaimRef\data\local\documents.json`
- `%LOCALAPPDATA%\FamilyClaimRef\data\local\policy-documents.json`
- `%LOCALAPPDATA%\FamilyClaimRef\attachments\documents\policy-document_20260706_capture_001.png`

Runtime missing:

- `%LOCALAPPDATA%\FamilyClaimRef\data\local\claims.json`
- `%LOCALAPPDATA%\FamilyClaimRef\data\local\claim-documents.json`

Project root:

- `C:\EtcProject\FamilyClaimRef\attachments`: files=0
- `C:\EtcProject\FamilyClaimRef\data\local`: files=0
- `C:\EtcProject\FamilyClaimRef\runtime_test_document.*`: missing

Known local excluded:

- `C:\EtcProject\FamilyClaimRef\data\claimdoc`
- not inspected
- not used
- not staged

## D. Problem Definition

- Scenario 8A success evidence is now recorded in `docs/151`.
- Remaining artifacts are useful evidence but may affect future runtime validation.
- temp `.txt` and `.png` files remain outside project root.
- runtime policies/documents/link/attachment remain under `%LOCALAPPDATA%`.
- cleanup could remove validation evidence.
- not cleaning up could affect future Scenario 8B or UI validation.
- cleanup scope must distinguish Scenario 8A-created artifacts from pre-existing runtime artifacts.
- runtime root was not clean-room before Scenario 8A; pre-existing documents/link/attachment existed before earlier phases.

## E. Cleanup Options

### Option A: No Cleanup, Commit Evidence First

Description:

- Cleanup을 수행하지 않고 `docs/145`~`docs/152` evidence chain을 먼저 commit candidate review로 묶는다.

Pros:

- Scenario 8A success evidence를 보존한다.
- cleanup 전 상태를 그대로 유지한다.
- 삭제 위험이 없다.

Cons:

- future runtime validation에 artifacts가 남는다.
- temp files가 계속 남는다.

Assessment:

- evidence preservation 우선이면 적합하다.

### Option B: Targeted Cleanup of Scenario 8A Retry Artifacts Only

Description:

- Scenario 8A retry가 만든 artifact만 exact path 기준으로 삭제한다.

Candidate cleanup targets:

- `%TEMP%\FamilyClaimRef\runtime_test_document.png`
- `%LOCALAPPDATA%\FamilyClaimRef\data\local\policies.json`
- `%LOCALAPPDATA%\FamilyClaimRef\data\local\documents.json`
- `%LOCALAPPDATA%\FamilyClaimRef\data\local\policy-documents.json`
- `%LOCALAPPDATA%\FamilyClaimRef\attachments\documents\policy-document_20260706_capture_001.png`

Optional candidate:

- `%TEMP%\FamilyClaimRef\runtime_test_document.txt`

Pros:

- Scenario 8A retry residue를 줄일 수 있다.
- future validation을 더 깨끗하게 시작할 수 있다.

Cons:

- `documents.json` / `policy-documents.json`에는 pre-existing evidence가 있었던 이력이 있으므로 전체 파일 삭제는 pre-existing metadata까지 삭제할 수 있다.
- exact record removal은 현재 storage tooling이 없으면 JSON 수동 수정이 필요하며 위험하다.
- attachment file name은 Scenario 8A-created로 확인되지만 삭제는 evidence를 제거한다.
- temp `.txt`는 initial blocked scenario evidence다.

Assessment:

- 파일 단위 cleanup은 위험하다.
- JSON record-level cleanup은 별도 tooling 없이 비권장이다.
- 즉시 실행 후보로는 부적합하다.

### Option C: Targeted Cleanup of Temp Synthetic Files Only

Description:

- `%TEMP%\FamilyClaimRef\runtime_test_document.txt`
- `%TEMP%\FamilyClaimRef\runtime_test_document.png`

위 temp files만 cleanup 후보로 둔다.

Pros:

- source/runtime metadata evidence는 보존한다.
- temp folder residue만 줄인다.
- 가장 낮은 위험의 cleanup이다.

Cons:

- runtime policies/documents/link/attachment는 남는다.
- future runtime validation은 사전 existing runtime artifacts를 고려해야 한다.

Assessment:

- 낮은 위험의 후속 cleanup 후보다.
- cleanup result review가 필요하다.

### Option D: Full Runtime Root Cleanup

Description:

- `%LOCALAPPDATA%\FamilyClaimRef` 전체 또는 `data/local`, `attachments`를 삭제한다.

Pros:

- clean-room runtime에 가까워진다.

Cons:

- pre-existing evidence와 Scenario 8A evidence를 모두 삭제한다.
- 실제 사용자 runtime data가 있었다면 위험하다.
- 현재 정책상 reject한다.

Assessment:

- reject.

### Option E: Runtime Root Override / Isolated Future Validation

Description:

- 기존 runtime root는 보존하고, future validation은 isolated temporary runtime root를 사용하도록 설계한다.

Pros:

- evidence 보존과 clean validation을 동시에 달성할 수 있다.

Cons:

- 현재 앱은 runtime root override를 지원하지 않을 가능성이 높다.
- code/config 설계가 필요하다.
- 이번 cleanup decision 범위 밖이다.

Assessment:

- future technical design 후보.

## F. Recommended Decision

Recommended:

- Option A를 우선 선택한다.
- 즉시 cleanup은 수행하지 않는다.
- `docs/145`~`docs/152` evidence chain을 먼저 commit candidate review로 묶는다.
- cleanup은 별도 후속으로 둔다.
- cleanup을 한다면 Option C temp synthetic files cleanup부터 별도 decision/instruction으로 수행한다.
- Scenario 8A-created runtime JSON/link/attachment cleanup은 지금 하지 않는다.
- Full runtime root cleanup은 reject한다.
- Scenario 8B를 진행하려면 current runtime artifact state를 pre-run snapshot으로 다시 기록한다.

## G. Confirmed Decision

Confirmed:

- No cleanup now.
- Commit evidence first.
- Temp file cleanup may be considered later.
- Runtime metadata/link/attachment cleanup is deferred.
- Full runtime root cleanup is rejected.
- `data/claimdoc` remains expected-but-excluded and untouched.

## H. Cleanup Safety Rules For Future

- exact path cleanup only
- no wildcard deletion
- no recursive deletion
- no directory deletion
- no `%LOCALAPPDATA%\FamilyClaimRef` full deletion
- no project root cleanup
- no git clean/reset/checkout
- pre-cleanup snapshot required
- post-cleanup snapshot required
- cleanup result review required
- `data/claimdoc` is never cleanup target
- JSON record-level cleanup requires separate tooling/design; do not manually edit JSON records

## I. Commit Policy

- `docs/145`~`docs/152` are eligible for exact-file-list commit candidate review.
- `data/` is not eligible.
- temp files are not eligible.
- runtime files are not eligible.
- code/XAML/ViewModel/test are not eligible unless separately changed.
- `git add .` / `git add -A` remain forbidden.
- exact docs file list only.

## J. Scenario 8B Impact

- Scenario 8B claim target remains untested.
- If Scenario 8B proceeds, current runtime artifacts from Scenario 8A remain.
- Scenario 8B pre-run snapshot must record existing policy/document/link/attachment state.
- Scenario 8B may create `claims.json`, `claim-documents.json`, additional document record, and additional copied attachment.
- Scenario 8B should remain a separate approval decision.

## K. Explicit Non-Scope

이번 문서 생성에서 하지 않는 항목:

- cleanup 실행 없음
- temp file deletion 없음
- runtime artifact deletion 없음
- JSON editing 없음
- app launch 없음
- OpenFileDialog 없음
- Scenario 8B 없음
- document registration workflow 없음
- `data/claimdoc` use/inspection/listing 없음
- code/XAML/ViewModel/test 수정 없음
- `FileNamePolicyService` 수정 없음
- DB/SQLite/OCR/repository 구현 없음
- git add/commit/reset/checkout/clean 없음

## L. Verification For This Documentation Task

`docs/152` 생성 후 수행:

- `git diff --check`
- `git status --short`
- expected:
  - `?? data/`
  - `?? docs/145...` through `docs/152`
- project root `attachments/`: files=0
- project root `data/local`: files=0
- project root `runtime_test_document.*`: missing
- build/test: not run, documentation-only change

## M. Completion Report Format

```md
POLICY_CLAIM_SCENARIO8A_RETRY_ARTIFACT_CLEANUP_SCOPE_DECISION_RECORDED

생성 문서:
- docs/152_POLICY_CLAIM_SCENARIO8A_RETRY_ARTIFACT_CLEANUP_SCOPE_DECISION.md

구현/실행 여부:
- code 수정 없음
- XAML 수정 없음
- ViewModel 수정 없음
- tests 수정 없음
- cleanup 실행 없음
- temp file deletion 없음
- runtime artifact deletion 없음
- app launch 없음
- OpenFileDialog 없음
- Scenario 8B 없음

결정 요약:
- selected cleanup policy:
- deferred cleanup:
- rejected cleanup:
- commit policy:
- Scenario 8B impact:
- data/claimdoc:

검증 결과:
- git diff --check: PASS/FAIL
- git status --short: expected docs/145~152 plus excluded data/ / unexpected
- project root attachments/: files=<count>
- project root data/local: files=<count>
- project root runtime_test_document.*: missing/exists
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
- runtime artifact 삭제 없음
- project root cleanup 없음
- data/claimdoc 파일 사용 없음
- DB/SQLite/OCR/repository 구현 없음
- git add/commit/reset/checkout/clean 사용 없음

다음 추천 작업:
- docs/145~152 commit candidate review 생성
```
