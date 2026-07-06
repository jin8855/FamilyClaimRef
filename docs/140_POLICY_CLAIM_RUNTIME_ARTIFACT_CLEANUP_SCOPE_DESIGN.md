# Policy / Claim Runtime Artifact Cleanup Scope Design

## A. Status Marker

POLICY_CLAIM_RUNTIME_ARTIFACT_CLEANUP_SCOPE_DESIGNED

## B. Background

Phase 3D base runtime manual validation은 Scenario 1~7만 실행했다.

Scenario 8 synthetic document registration은 실행하지 않았다.

`OpenFileDialog`는 실행하지 않았다.

document registration workflow는 실행하지 않았다.

`runtime_test_document.txt`는 생성하지 않았다.

cleanup은 승인되지 않아 수행하지 않았다.

`%LOCALAPPDATA%\FamilyClaimRef`에는 pre-existing runtime document metadata와 attachment가 이미 있었다.

이번 base execution으로 `policies.json`, `claims.json` synthetic disabled records가 생성되었다.

project root `attachments/`와 `data/local`은 사전/사후 모두 files=0으로 clean 상태다.

## C. Current Runtime Artifact State

`docs/139_POLICY_CLAIM_RUNTIME_MANUAL_VALIDATION_PHASE3D_RESULT_REVIEW.md` 기준 current state는 다음과 같다.

Pre-run runtime root:

- `%LOCALAPPDATA%\FamilyClaimRef` existed
- pre-existing:
  - `attachments\documents\policy-document_20260702_policy_001.png`
  - `data\local\documents.json`
  - `data\local\policy-documents.json`
- pre-run missing:
  - `policies.json`
  - `claims.json`
  - `claim-documents.json`

Post-run runtime root:

- `data\local\policies.json`
- `data\local\claims.json`
- pre-existing `data\local\documents.json`
- pre-existing `data\local\policy-documents.json`
- pre-existing attachment under `%LOCALAPPDATA%`

Post-run policy / claim state:

```text
policies total=1
policies active=0
claims total=1
claims active=0
```

Current runtime records:

```text
policies.json items=1
  id=policy_53839aaed23342568f346879599f7d0a
  title=policy_title_runtime_demo
  disabledAt=2026-07-03T08:38:53.648529+00:00

claims.json items=1
  id=claim_db67f71178274699889b9c573c951130
  title=claim_title_runtime_demo
  policyId=policy_53839aaed23342568f346879599f7d0a
  disabledAt=2026-07-03T08:38:52.5704075+00:00

documents.json items=1
  id=doc_ea8a2b89b3184dc3909c2cdd9fef99f2
  title=Dummy Policy Document

policy-documents.json items=1
  id=pdoc_f22fb58e800e4a49a7de7e0e4ae08b63
  policyId=POLICY-DEMO-001
  documentId=doc_ea8a2b89b3184dc3909c2cdd9fef99f2

claim-documents.json MISSING
```

Project root:

- `C:\EtcProject\FamilyClaimRef\attachments`: files=0
- `C:\EtcProject\FamilyClaimRef\data\local`: files=0

DB / SQLite unexpected file:

```text
none
```

## D. Problem Definition

Scenario 8을 진행하려면 active policy / claim target이 필요하다.

현재 runtime policies / claims는 disabled 상태라 active target으로 쓸 수 없다.

Scenario 8 전에 새 active policy / claim을 만들 수도 있지만 기존 runtime artifacts는 계속 남는다.

cleanup 없이 Scenario 8을 실행하면 pre-existing document metadata, previous attachment, new Scenario 8 metadata가 함께 존재한다.

cleanup을 수행하면 validation evidence가 사라질 수 있다.

full cleanup은 과거 runtime artifact까지 삭제할 수 있어 위험하다.

따라서 cleanup 범위와 보존 범위를 먼저 결정해야 한다.

## E. Explicit Non-Scope

이 문서 생성 작업에서 수행하지 않는 항목:

- cleanup 실행 없음
- `%LOCALAPPDATA%\FamilyClaimRef` 삭제 없음
- runtime file 삭제 없음
- runtime JSON 수정 없음
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

## F. Cleanup Options

### Option A: No Cleanup, Proceed With Recorded Runtime State

설명:

- `%LOCALAPPDATA%\FamilyClaimRef`를 그대로 둔다.
- Scenario 8 실행 전 새 active policy / claim을 다시 생성하거나 기존 상태를 기록하고 진행한다.

장점:

- validation evidence를 보존한다.
- 삭제 위험이 없다.
- 가장 안전한 보존 정책이다.

단점:

- Scenario 8 결과가 기존 runtime artifacts와 섞인다.
- pre-existing `documents.json`, `policy-documents.json`, attachment 때문에 Scenario 8 결과 분리가 어렵다.
- disabled synthetic policy / claim records가 계속 남는다.

판정 후보:

- evidence preservation 우선이면 적합하다.
- clean Scenario 8 검증에는 부적합하다.

### Option B: Targeted Cleanup of Phase 3D Base Policy / Claim Artifacts Only

설명:

- 이번 base execution으로 생성된 것으로 확인된 `policies.json`, `claims.json`만 cleanup 후보로 둔다.
- pre-existing `documents.json`, `policy-documents.json`, attachments는 보존한다.

Cleanup candidate files:

- `%LOCALAPPDATA%\FamilyClaimRef\data\local\policies.json`
- `%LOCALAPPDATA%\FamilyClaimRef\data\local\claims.json`

장점:

- Phase 3D base synthetic policy / claim residue를 제거할 수 있다.
- pre-existing document evidence를 보존한다.
- full cleanup보다 위험이 작다.

단점:

- pre-existing document metadata / attachment는 여전히 남아 Scenario 8 결과와 섞일 수 있다.
- policies / claims cleanup의 안전성은 pre-run snapshot 근거에 의존한다.
- cleanup execution instruction이 별도로 필요하다.

판정 후보:

- 가장 현실적인 최소 cleanup 후보다.
- 별도 user approval과 exact file list cleanup instruction이 필요하다.

### Option C: Full Runtime Root Cleanup

설명:

- `%LOCALAPPDATA%\FamilyClaimRef` 전체 또는 하위 `data\local`, `attachments`를 삭제한다.

장점:

- Scenario 8을 clean runtime 상태에서 검증할 수 있다.
- artifact 혼합 위험이 가장 작다.

단점:

- pre-existing document metadata와 attachment를 삭제한다.
- 이전 runtime evidence가 사라진다.
- 실제 사용자가 만든 runtime data가 있었으면 손상 위험이 있다.
- 현재 정책상 별도 cleanup scope와 explicit approval 없이는 금지된다.

판정 후보:

- 기본적으로 reject한다.
- clean-room validation이 꼭 필요하고 사용자가 명시 승인한 경우에만 별도 계획으로 다룬다.

### Option D: Preserve Runtime Root and Use Separate Temporary Runtime Root

설명:

- 기존 `%LOCALAPPDATA%\FamilyClaimRef`를 건드리지 않고 별도 temporary runtime root를 사용한다.

장점:

- 기존 runtime evidence를 보존한다.
- Scenario 8을 isolated clean state에서 수행할 수 있다.

단점:

- 현재 `AppServices.CreateDefault()`는 `%LOCALAPPDATA%\FamilyClaimRef` 고정 root를 사용한다.
- 현재 앱이 runtime root override를 지원하는지는 불명확하며 즉시 실행 후보가 아니다.
- root override를 지원하려면 code change 또는 launch environment 설계가 필요할 수 있다.

판정 후보:

- 장기적으로 유용할 수 있다.
- 현재 cleanup decision 범위를 넘어서는 별도 technical design이 필요하다.

## G. Recommended Cleanup Direction

추천:

- 즉시 cleanup 실행은 하지 않는다.
- 먼저 Option B targeted cleanup을 1차 후보로 둔다.
- targeted cleanup execution은 별도 user decision record와 exact-file cleanup instruction 후에만 수행한다.
- full runtime root cleanup인 Option C는 기본 reject한다.
- Scenario 8 진행 전에는 다음 중 하나를 선택해야 한다.
  1. cleanup 없이 existing runtime state를 기록하고 Scenario 8 진행
  2. Option B targeted cleanup 후 Scenario 8 진행
  3. Option C full cleanup 별도 승인 후 Scenario 8 진행
  4. Scenario 8을 보류하고 docs/136~140 commit candidate review 진행

## H. Cleanup Safety Rules

cleanup을 수행하는 경우 반드시 지킬 안전 규칙:

- cleanup은 exact path list 기준으로만 수행한다.
- wildcard deletion 금지
- directory recursive deletion 금지
- `%LOCALAPPDATA%\FamilyClaimRef` 전체 삭제 금지, 별도 명시 승인 전
- project root cleanup 금지
- git clean 금지
- git reset / checkout 금지
- cleanup 전 pre-cleanup snapshot 필수
- cleanup 후 post-cleanup snapshot 필수
- 삭제한 파일 목록 기록 필수
- cleanup result review 문서 필수
- cleanup 중 오류 발생 시 즉시 중단

## I. Candidate Targeted Cleanup Exact Paths

Option B 후보 exact path:

- `%LOCALAPPDATA%\FamilyClaimRef\data\local\policies.json`
- `%LOCALAPPDATA%\FamilyClaimRef\data\local\claims.json`

Do not delete:

- `%LOCALAPPDATA%\FamilyClaimRef\data\local\documents.json`
- `%LOCALAPPDATA%\FamilyClaimRef\data\local\policy-documents.json`
- `%LOCALAPPDATA%\FamilyClaimRef\data\local\claim-documents.json`
- `%LOCALAPPDATA%\FamilyClaimRef\attachments`
- `%LOCALAPPDATA%\FamilyClaimRef\attachments\documents\policy-document_20260702_policy_001.png`
- project root `attachments/`
- project root `data/local`

주의:

- `claim-documents.json`은 현재 missing으로 기록되어 있으나, 존재하더라도 targeted cleanup 후보가 아니다.
- documents / policy-documents / attachments는 pre-existing evidence로 본다.

## J. Scenario 8 Impact

No cleanup:

- 새 document metadata가 기존 `documents.json`에 추가될 수 있다.
- 새 link metadata가 기존 `policy-documents.json` 또는 `claim-documents.json`에 추가될 수 있다.
- 결과 분리가 어렵다.

Targeted cleanup:

- policy / claim active list를 더 깔끔하게 시작할 수 있다.
- 기존 documents / link / attachments는 남는다.
- Scenario 8 document metadata 분리는 pre/post snapshot에 의존한다.

Full cleanup:

- 가장 명확한 Scenario 8 결과를 얻을 수 있다.
- 하지만 기존 evidence 삭제 위험이 크다.

Recommendation:

- Scenario 8은 targeted cleanup 여부를 먼저 결정한 뒤 별도 approval gate로 진행한다.

## K. Required Decision Before Any Cleanup

다음 user decision record에서 결정해야 할 항목:

- cleanup을 수행할지 여부
- Option A / B / C / D 중 선택
- targeted cleanup exact path 승인 여부
- full cleanup reject 여부
- cleanup 전후 snapshot 방식
- cleanup result review 문서 생성 여부
- cleanup 후 Scenario 8 진행 여부
- docs/136~140 commit candidate review 선행 여부

## L. Verification for This Documentation Task

docs/140 생성 후 수행:

- `git diff --check`
- `git status --short`
- project root `attachments/` files count
- project root `data/local` files count

build/test:

- documentation-only change이므로 실행하지 않는다.

## M. Completion Report Format

완료 보고 형식:

```text
POLICY_CLAIM_RUNTIME_ARTIFACT_CLEANUP_SCOPE_DESIGNED

생성 문서:
- docs/140_POLICY_CLAIM_RUNTIME_ARTIFACT_CLEANUP_SCOPE_DESIGN.md

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

주요 설계:
- current runtime state:
- cleanup options:
- recommended cleanup direction:
- candidate targeted cleanup paths:
- do-not-delete paths:
- Scenario 8 impact:
- required next decision:

검증 결과:
- git diff --check: PASS/FAIL
- git status --short: expected docs/136~140 only / unexpected
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
Policy / Claim Runtime Artifact Cleanup User Decision Record 문서 생성
```
