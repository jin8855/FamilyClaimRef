# Policy / Claim Runtime Artifact Cleanup User Decision Record

## A. Status Marker

POLICY_CLAIM_RUNTIME_ARTIFACT_CLEANUP_USER_DECISION_RECORDED

## B. Decision Context

Phase 3D base runtime manual validation에서 Scenario 1~7은 실행 완료되었다.

Scenario 8 synthetic document registration은 실행하지 않았다.

`OpenFileDialog`는 실행하지 않았다.

document registration workflow는 실행하지 않았다.

cleanup은 승인되지 않아 수행하지 않았다.

`%LOCALAPPDATA%\FamilyClaimRef`에는 pre-existing `documents.json`, `policy-documents.json`, attachment가 있었다.

Phase 3D base execution으로 `policies.json`, `claims.json`이 새로 생성되었다.

생성된 policy / claim records는 synthetic runtime validation records이며 현재 disabled 상태다.

project root `attachments/`와 `data/local`은 files=0으로 clean 상태다.

`docs/140_POLICY_CLAIM_RUNTIME_ARTIFACT_CLEANUP_SCOPE_DESIGN.md`에서 cleanup options를 비교했고 Option B targeted cleanup을 1차 후보로 정리했다.

이 문서는 cleanup 실행 전 사용자 결정을 고정하기 위한 문서다.

## C. Confirmed Decisions

### Decision 1: Cleanup Execution Timing

Confirmed:

- 이 user decision record 작성 중에는 cleanup을 실행하지 않는다.
- cleanup은 별도 execution instruction 작성 후에만 수행한다.

Reason:

- cleanup은 runtime validation evidence를 삭제할 수 있으므로 exact path와 snapshot 절차를 먼저 고정해야 한다.

### Decision 2: Cleanup Option

Confirmed:

- Option B targeted cleanup을 Phase 3D cleanup의 우선 실행 후보로 둔다.

Selected option:

- Option B: Targeted Cleanup of Phase 3D Base Policy / Claim Artifacts Only

Rejected by default:

- Option C: Full Runtime Root Cleanup

Deferred:

- Option A: No cleanup
- Option D: Temporary runtime root

Reason:

- `policies.json`과 `claims.json`은 Phase 3D base execution에서 새로 생성된 synthetic policy / claim residue다.
- pre-existing `documents.json`, `policy-documents.json`, runtime attachment는 이번 cleanup 대상이 아니다.
- full cleanup은 기존 evidence와 과거 runtime artifact를 삭제할 수 있어 위험하다.

### Decision 3: Targeted Cleanup Exact Paths

Confirmed cleanup candidate exact paths:

- `%LOCALAPPDATA%\FamilyClaimRef\data\local\policies.json`
- `%LOCALAPPDATA%\FamilyClaimRef\data\local\claims.json`

Implementation rule:

- cleanup execution instruction에서는 위 exact paths만 삭제 후보로 둔다.
- wildcard deletion 금지
- recursive deletion 금지
- directory deletion 금지

### Decision 4: Do-Not-Delete Paths

Confirmed do-not-delete paths:

- `%LOCALAPPDATA%\FamilyClaimRef\data\local\documents.json`
- `%LOCALAPPDATA%\FamilyClaimRef\data\local\policy-documents.json`
- `%LOCALAPPDATA%\FamilyClaimRef\data\local\claim-documents.json`
- `%LOCALAPPDATA%\FamilyClaimRef\attachments`
- `%LOCALAPPDATA%\FamilyClaimRef\attachments\documents\policy-document_20260702_policy_001.png`
- `C:\EtcProject\FamilyClaimRef\attachments`
- `C:\EtcProject\FamilyClaimRef\data\local`

Notes:

- `claim-documents.json`은 현재 missing으로 기록되어 있으나, 존재하더라도 cleanup 대상이 아니다.
- `documents.json`, `policy-documents.json`, attachments는 pre-existing evidence로 본다.

### Decision 5: Full Cleanup Policy

Confirmed:

- `%LOCALAPPDATA%\FamilyClaimRef` 전체 삭제는 reject한다.
- `%LOCALAPPDATA%\FamilyClaimRef\data\local` 전체 삭제는 reject한다.
- `%LOCALAPPDATA%\FamilyClaimRef\attachments` 전체 삭제는 reject한다.

Reason:

- pre-existing document metadata와 attachment를 삭제할 수 있다.
- 실제 사용자가 만든 runtime data가 있었으면 손상 위험이 있다.
- 이번 cleanup 목적은 Phase 3D base execution residue만 최소 제거하는 것이다.

### Decision 6: Snapshot Policy

Confirmed:

- cleanup execution 전 pre-cleanup snapshot을 반드시 기록한다.
- cleanup execution 후 post-cleanup snapshot을 반드시 기록한다.
- 삭제 전 `policies.json`과 `claims.json` 존재 여부, file size, item count, id / title / disabledAt sanity를 기록한다.
- 삭제 후 `policies.json`과 `claims.json` missing 여부를 기록한다.
- do-not-delete paths가 유지되는지 확인한다.

### Decision 7: Scenario 8 Relation

Confirmed:

- Scenario 8은 이 cleanup decision record에서 승인하지 않는다.
- Scenario 8은 cleanup 여부와 별개로 별도 explicit approval gate가 필요하다.
- Targeted cleanup을 수행하더라도 Scenario 8은 자동 승인되지 않는다.

Reason:

- Scenario 8은 `OpenFileDialog`, actual file selection, document registration workflow, copied attachment 생성을 포함하여 위험도가 다르다.

### Decision 8: Cleanup Result Review

Confirmed:

- cleanup 실행 후에는 cleanup result review 문서를 생성한다.

Expected document:

```text
docs/142_POLICY_CLAIM_RUNTIME_ARTIFACT_CLEANUP_RESULT_REVIEW.md
```

Cleanup result review에 포함할 항목:

- pre-cleanup snapshot
- deleted exact paths
- deletion success / failure
- post-cleanup snapshot
- do-not-delete path preservation
- project root `attachments` / `data/local` safety
- DB/SQLite unexpected file check
- actual personal sample check
- cleanup performed: yes
- cleanup scope: targeted only
- Scenario 8 executed: no

### Decision 9: Commit Timing

Confirmed:

- docs/136~141은 cleanup 실행 전에 commit candidate review를 할 수도 있다.
- cleanup result review까지 포함하려면 docs/136~142를 묶어 commit candidate review를 할 수도 있다.

Recommended:

- cleanup을 실제 수행할 계획이면 docs/142 cleanup result review까지 만든 뒤 docs/136~142를 함께 commit candidate review한다.

Reason:

- cleanup decision과 result를 같은 evidence chain에 두는 편이 추적성이 좋다.

## D. Explicit Non-Scope

이 user decision record 작성에서 하지 않는 항목:

- cleanup 실행 없음
- runtime artifact 삭제 없음
- runtime JSON 수정 없음
- `%LOCALAPPDATA%\FamilyClaimRef` 삭제 없음
- app launch 없음
- `OpenFileDialog` 실행 없음
- Scenario 8 실행 없음
- synthetic test document 생성 없음
- registration workflow 실행 없음
- 코드 수정 없음
- XAML 수정 없음
- ViewModel 수정 없음
- tests 수정 없음
- DB/SQLite/OCR/repository 구현 없음
- git add / commit / reset / checkout / clean 없음

## E. Guardrails for Cleanup Execution Instruction

다음 cleanup execution instruction 작성 시 반드시 포함할 guardrail:

- exact path cleanup only
- wildcard deletion 금지
- directory deletion 금지
- recursive deletion 금지
- `%LOCALAPPDATA%\FamilyClaimRef` 전체 삭제 금지
- project root cleanup 금지
- git clean 금지
- git reset / checkout 금지
- pre-cleanup snapshot
- post-cleanup snapshot
- `policies.json` / `claims.json`만 cleanup 후보
- `documents.json` / `policy-documents.json` / attachments 보존
- deletion failure 발생 시 즉시 중단
- result review 문서 생성
- app launch 금지
- `OpenFileDialog` 금지
- Scenario 8 금지

## F. Risks Accepted

Accepted risks:

- targeted cleanup을 수행하면 Phase 3D base synthetic policy / claim evidence 일부가 삭제된다.
- cleanup 후에는 `policies.json`, `claims.json`의 원본 contents를 직접 재검증하기 어렵다.
- pre-existing documents / link / attachment가 남기 때문에 Scenario 8 결과 혼합 위험은 완전히 사라지지 않는다.
- cleanup을 하지 않으면 disabled synthetic policy / claim records가 후속 runtime 실행에 계속 남는다.

Risk handling:

- docs/139와 docs/140에 cleanup 전 evidence가 기록되어 있다.
- cleanup execution 전에 pre-cleanup snapshot을 다시 기록한다.
- cleanup result review를 생성한다.
- Scenario 8은 별도 approval gate로 유지한다.

## G. Next Recommendation

다음 추천 작업:

```text
Policy / Claim Runtime Artifact Cleanup Execution Instruction 작성
```

실제 cleanup은 그 execution instruction의 별도 승인 후에만 수행한다.

## H. Verification for This Documentation Task

docs/141 생성 후 수행:

- `git diff --check`
- `git status --short`
- project root `attachments/` files count
- project root `data/local` files count

build/test:

- documentation-only change이므로 실행하지 않는다.

## I. Completion Report Format

완료 보고 형식:

```text
POLICY_CLAIM_RUNTIME_ARTIFACT_CLEANUP_USER_DECISION_RECORDED

생성 문서:
- docs/141_POLICY_CLAIM_RUNTIME_ARTIFACT_CLEANUP_USER_DECISION_RECORD.md

분석 대상:
- ...

구현/실행 여부:
- 코드 수정 없음
- XAML 수정 없음
- ViewModel 수정 없음
- 테스트 수정 없음
- cleanup 실행 없음
- app launch 없음
- OpenFileDialog 실행 없음
- runtime workflow 실행 없음
- runtime artifact 삭제 없음
- %LOCALAPPDATA%\FamilyClaimRef 삭제 없음

확정 결정:
- cleanup execution timing:
- selected cleanup option:
- targeted cleanup exact paths:
- do-not-delete paths:
- full cleanup policy:
- snapshot policy:
- Scenario 8 relation:
- cleanup result review:
- commit timing:

검증 결과:
- git diff --check: PASS/FAIL
- git status --short: expected docs/136~141 only / unexpected
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
- runtime artifact 삭제 없음
- %LOCALAPPDATA%\FamilyClaimRef 삭제 없음
- project root cleanup 없음
- DB/SQLite/OCR/repository 구현 없음
- git add/commit/reset/checkout/clean 사용 없음

다음 추천 작업:
Policy / Claim Runtime Artifact Cleanup Execution Instruction 작성
```
