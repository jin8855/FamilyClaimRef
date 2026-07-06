# Policy / Claim Local Data Claimdoc Handling Decision

## A. Status Marker

POLICY_CLAIM_LOCAL_DATA_CLAIMDOC_HANDLING_DECISION_RECORDED

## B. Decision Context

Scenario 8A retry execution instruction 작성이 시작 전 git status check에서 중단되었다.

중단 원인은 expected docs/145~148 외에 `?? data/`가 나타났기 때문이다.

사용자는 `C:\EtcProject\FamilyClaimRef\data\claimdoc` 안에 약관/계약서가 있다고 밝혔다.

`data/claimdoc` 하위 파일은 열람/분석/삭제/정리하지 않았다.

docs/149 retry instruction은 생성되지 않았다.

`FileNamePolicyService` allowlist는 `pdf`, `jpg`, `jpeg`, `png`로 확인되었다.

Scenario 8A retry는 allowed synthetic PNG로 진행하는 방향이지만, `data/` untracked 처리 결정이 먼저 필요하다.

## C. Current Source Tree State

Latest commit:

```text
58f891a docs(familyclaimref): add runtime validation cleanup review
```

Observed `git status --short`:

```text
?? data/
?? docs/145_POLICY_CLAIM_SCENARIO8_SYNTHETIC_DOCUMENT_REGISTRATION_DECISION.md
?? docs/146_POLICY_CLAIM_SCENARIO8_SYNTHETIC_DOCUMENT_REGISTRATION_EXECUTION_INSTRUCTION.md
?? docs/147_POLICY_CLAIM_SCENARIO8_SYNTHETIC_DOCUMENT_REGISTRATION_RESULT_REVIEW.md
?? docs/148_POLICY_CLAIM_SCENARIO8A_RETRY_POLICY_DECISION.md
```

Current interpretation:

- `data/` is untracked.
- docs/145~148 are untracked.
- docs/149 retry instruction does not exist yet.
- code/XAML/ViewModel/tests tracked diff 없음.

주의:

- `data/claimdoc` contents were not inspected.
- `data/claimdoc` files were not listed.
- `data/claimdoc` was not staged.
- `data/claimdoc` was not deleted.

## D. Data / Claimdoc Risk Assessment

`data/claimdoc`는 실제 약관/계약서 보관 경로로 취급한다.

실제 약관/계약서는 synthetic validation에 사용할 수 없다.

실제 계약서/약관은 실제 보험사명, 계약 정보, 개인정보, 민감한 문서명을 포함할 가능성이 있다.

따라서 Scenario 8A retry에서는 절대 사용하지 않는다.

OpenFileDialog에서 해당 경로가 보이거나 선택 후보로 나타나면 선택하지 않는다.

해당 파일은 git commit 대상이 아니다.

해당 파일은 cleanup 대상도 아니다.

해당 파일은 project root `data/local` runtime artifact와 다르게 취급한다.

## E. Decision Options

### Option A: Keep Blocking Until data/ Is Removed

설명:

- `?? data/`가 사라질 때까지 모든 후속 문서 작업과 Scenario 8A retry instruction을 중단한다.

장점:

- git status 기준이 가장 엄격하다.
- accidental commit 위험이 낮다.

단점:

- 사용자가 실제 약관/계약서를 보관 중이라면 삭제/이동을 요구하게 된다.
- cleanup/delete/move가 금지된 현재 흐름과 충돌한다.
- 프로젝트 진행이 불필요하게 막힌다.

판정 후보:

- 보안은 강하지만 운영상 과도하다.

### Option B: Treat data/claimdoc As Known Local Real-Document Artifact And Continue

설명:

- `?? data/`를 known local out-of-scope artifact로 기록한다.
- 후속 작업의 expected git status에 `?? data/`를 명시적으로 포함한다.
- 단, `data/` 하위 파일은 stage/inspect/use/delete 하지 않는다.

장점:

- 실제 문서 보관 상태를 건드리지 않는다.
- 프로젝트 진행이 가능하다.
- accidental use를 문서화된 금지 항목으로 막을 수 있다.

단점:

- git status가 clean하지 않다.
- 매 작업마다 `?? data/`를 known exception으로 관리해야 한다.
- 실수로 `git add data`를 할 위험이 남는다.

판정:

- 현 단계 추천안.

### Option C: Add Exact .gitignore Rule For data/claimdoc/

설명:

- `.gitignore`에 `data/claimdoc/` 또는 유사 exact path rule을 추가한다.

장점:

- git status noise를 줄인다.
- accidental staging 위험을 줄일 수 있다.

단점:

- `.gitignore` 수정은 project config change다.
- 현재 작업 범위보다 넓다.
- 어떤 `data/` 경로를 ignore할지 별도 설계가 필요하다.
- root `data/local`과 구분해야 한다.

판정:

- 후속 별도 user decision / implementation 후보.
- 이번 decision 문서에서는 실행하지 않는다.

### Option D: Move Or Delete data/claimdoc

설명:

- 실제 약관/계약서 폴더를 source tree 밖으로 옮기거나 삭제한다.

장점:

- source tree status를 clean하게 만들 수 있다.

단점:

- 사용자의 실제 파일에 대한 위험한 조작이다.
- 삭제/이동은 현재 명시 금지다.
- evidence 손실 위험이 있다.

판정:

- reject.

## F. Confirmed Decision

Confirmed:

- Option B를 채택한다.
- `data/claimdoc`는 known local real-document artifact로 취급한다.
- `?? data/`는 후속 Scenario 8A retry instruction 작성과 execution instruction에서 expected-but-excluded status item으로 허용한다.
- 단, commit candidate exact file list에는 절대 포함하지 않는다.
- `data/claimdoc` 파일은 열람/분석/사용/삭제/이동하지 않는다.
- Scenario 8A retry에서 사용할 파일은 계속 `%TEMP%\FamilyClaimRef\runtime_test_document.png` synthetic PNG뿐이다.

Rejected:

- `data/claimdoc` 삭제
- `data/claimdoc` 이동
- `data/claimdoc` 사용
- `data/claimdoc` git add
- `data/claimdoc` cleanup
- `.gitignore` 즉시 수정

Deferred:

- `.gitignore`에 `data/claimdoc/` exact ignore rule을 추가할지 여부는 후속 별도 decision 후보로 둔다.

## G. Guardrails For Subsequent Scenario 8A Retry Instruction

후속 docs/150 Scenario 8A retry execution instruction 작성 시 반드시 포함한다.

- git status expected items:
  - `?? data/`
  - `?? docs/145...`
  - `?? docs/146...`
  - `?? docs/147...`
  - `?? docs/148...`
  - `?? docs/149...`
- `?? data/` is known local real-document artifact.
- do not inspect `data/claimdoc` contents.
- do not select any file under `data/claimdoc`.
- do not stage `data/`.
- do not commit `data/`.
- do not delete/move `data/`.
- allowed retry file remains `%TEMP%\FamilyClaimRef\runtime_test_document.png`.
- project root `runtime_test_document.*` must remain missing.
- project root `attachments/` files=0.
- project root `data/local` files=0.

## H. Commit Policy

- docs/145~150 commit candidate review에서는 `data/`를 포함하지 않는다.
- `git add` exact file list only.
- `data/` must never be staged.
- `git add .` / `git add -A` / `git add --all` remain forbidden.
- If `data/` appears in staged diff, commit is blocked.
- If `data/` is accidentally staged, do not use reset/checkout/clean without separate instruction; stop and report.

## I. Explicit Non-Scope

이번 decision 문서 생성에서 하지 않는 항목:

- `data/claimdoc` inspection 없음
- `data/claimdoc` file listing 없음
- `data/claimdoc` cleanup 없음
- `data/claimdoc` deletion 없음
- `data/claimdoc` move 없음
- `data/claimdoc` gitignore 추가 없음
- Scenario 8A retry instruction 생성 없음
- app launch 없음
- OpenFileDialog 없음
- synthetic PNG 생성 없음
- document registration workflow 실행 없음
- code/XAML/ViewModel/test 수정 없음
- `FileNamePolicyService` 수정 없음
- DB/SQLite/OCR/repository 구현 없음
- git add/commit/reset/checkout/clean 없음

## J. Verification For This Documentation Task

docs/149 생성 후 확인:

- `git diff --check`
- `git status --short`
- expected:
  - `?? data/`
  - `?? docs/145...`
  - `?? docs/146...`
  - `?? docs/147...`
  - `?? docs/148...`
  - `?? docs/149...`
- tracked source diff 없음
- project root `attachments/`: files=0
- project root `data/local`: files=0
- project root `runtime_test_document.*`: missing
- temp `runtime_test_document.png`: missing
- build/test: not run, documentation-only change

## K. Next Recommendation

다음 추천 작업:

```text
Scenario 8A allowed-extension retry execution instruction 문서 생성
```

후속 문서:

```text
docs/150_POLICY_CLAIM_SCENARIO8A_ALLOWED_EXTENSION_RETRY_EXECUTION_INSTRUCTION.md
```
