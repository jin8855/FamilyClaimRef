# Policy / Claim data/claimdoc Gitignore Decision

## A. Status Marker

POLICY_CLAIM_DATA_CLAIMDOC_GITIGNORE_DECISION_RECORDED

## B. Decision Context

- Scenario 8A docs commit이 완료되었다.
- 현재 latest commit은 `6ddd3fe docs(familyclaimref): add scenario8 document registration review`이다.
- 현재 git status에는 `?? data/`가 남아 있다.
- 사용자는 `C:\EtcProject\FamilyClaimRef\data\claimdoc` 안에 약관/계약서가 있다고 보고했다.
- `docs/149_POLICY_CLAIM_LOCAL_DATA_CLAIMDOC_HANDLING_DECISION.md`에서 `data/claimdoc`는 known local real-document artifact로 취급하기로 했다.
- `data/claimdoc`는 inspect/use/stage/commit/delete/move 금지 대상이다.
- 현재 문제는 후속 작업마다 `?? data/`를 expected-but-excluded로 관리할 것인지, 또는 `.gitignore`에 exact ignore rule을 추가할 것인지 결정하는 것이다.
- 이 문서는 `.gitignore` exact rule 추가 여부를 결정하기 위한 문서다.
- 이 문서 생성 작업에서는 `.gitignore`를 수정하지 않는다.

## C. Current State

Latest commit:

```text
6ddd3fe docs(familyclaimref): add scenario8 document registration review
```

Observed git status:

```text
?? data/
```

Current `.gitignore` relevant rules:

```text
attachments/
data/local/
*.db
*.sqlite
*.sqlite3
```

Current `.gitignore` does not contain:

```text
/data/claimdoc/
```

Current interpretation:

- `data/` is untracked.
- `data/claimdoc` existence was checked only at path level.
- `data/claimdoc` contents were not inspected.
- `data/claimdoc` file list was not collected.
- `data/claimdoc` was not staged.
- `data/claimdoc` was not committed.
- `data/claimdoc` was not moved or deleted.
- project root `data/local` remains separate from real-document `data/claimdoc`.

## D. Risk Assessment

- `data/claimdoc` may contain actual contract or terms documents.
- If `git add .` or `git add -A` is used accidentally, real documents could be staged.
- Current operating rules forbid `git add .` and `git add -A`, but git status noise remains.
- An exact `.gitignore` rule can reduce accidental staging risk.
- A broad `.gitignore` rule can hide unrelated `data/` paths and reduce visibility.
- `.gitignore` modification is a project configuration change and should be handled as a separate implementation decision.
- `data/local` runtime artifact handling must remain separate from `data/claimdoc` real-document handling.

## E. Options

### Option A: Keep expected-but-excluded only

Description:

- Do not change `.gitignore`.
- Continue treating `?? data/` as a known local expected-but-excluded status item.

Pros:

- No project config change.
- Current policy remains unchanged.
- No risk of accidentally hiding other `data/` paths.

Cons:

- Every future task must explicitly allow `?? data/` as expected.
- Accidental staging risk remains if broad git add commands are used.
- Status noise remains.

Assessment:

- Acceptable short-term, but operationally noisy.

### Option B: Add exact `.gitignore` rule for `/data/claimdoc/`

Description:

- Add one exact `.gitignore` rule:

```text
/data/claimdoc/
```

Pros:

- Real-document path can disappear from git status.
- Accidental staging risk decreases.
- Root `data/local` and other `data/` paths are not broadly ignored.
- The rule documents the project boundary for local real-document artifacts.

Cons:

- `.gitignore` modification is required.
- Verification is required to ensure only `data/claimdoc` is ignored.
- If untracked `data/` contains other files outside `claimdoc`, they may still appear in status.

Assessment:

- Recommended as the next implementation candidate.
- Must be implemented in a separate patch, not in this decision document.

### Option C: Ignore whole `/data/`

Description:

- Add a broad ignore rule for all root `data/`.

Pros:

- Removes broad `data/` status noise.

Cons:

- Too broad.
- Could hide unintended files.
- Conflicts with the need to distinguish `data/local` runtime artifacts from `data/claimdoc` real documents.
- Reduces future visibility.

Assessment:

- Reject.

### Option D: Move or delete `data/claimdoc`

Description:

- Move or delete the local real-document folder.

Pros:

- Removes source tree noise.

Cons:

- Directly manipulates user-provided real documents.
- Current policy forbids delete/move.
- Evidence or user files could be lost.

Assessment:

- Reject.

## F. Recommended Decision

Recommended:

- Select Option B as the future implementation candidate.
- Add exact `/data/claimdoc/` ignore rule in a separate approved patch.
- Do not modify `.gitignore` in this decision task.
- Reject whole `/data/` ignore.
- Reject move/delete of `data/claimdoc`.
- Continue treating `?? data/` as expected-but-excluded until the exact rule is implemented and verified.

## G. Confirmed Decision

Confirmed:

- Selected option: Option B, future exact `.gitignore` rule for `/data/claimdoc/`.
- `.gitignore` is not modified in this task.
- `/data/` whole ignore is rejected.
- `data/claimdoc` move/delete/use is rejected.
- `data/claimdoc` remains a known local real-document artifact.
- `data/claimdoc` must not be included in any commit candidate exact file list.

Deferred:

- Actual `.gitignore` implementation.
- Verification that `data/claimdoc` disappears from status after exact ignore rule.
- Review document for the `.gitignore` implementation.

## H. Guardrails For Future .gitignore Implementation

Future implementation instruction must include:

- exact rule only:

```text
/data/claimdoc/
```

- no `/data/` whole ignore
- no `data/local` ignore change
- no `data/claimdoc` file open/listing
- no `data/claimdoc` file use
- no `data/claimdoc` delete/move
- no `git add .`
- no `git add -A`
- `.gitignore` only modification candidate
- implementation review document candidate: `docs/155_POLICY_CLAIM_DATA_CLAIMDOC_GITIGNORE_IMPLEMENTATION_REVIEW.md`

Required verification:

- git status before: `?? data/`
- git status after: `data/claimdoc` ignored 여부
- project root `attachments/`: files=0
- project root `data/local`: files=0
- project root `runtime_test_document.*`: missing
- DB/SQLite unexpected file: none
- tracked source diff only `.gitignore` plus review docs if approved

## I. Explicit Non-Scope

This task does not perform:

- `.gitignore` modification
- `data/claimdoc` file open
- `data/claimdoc` file listing
- `data/claimdoc` use
- `data/claimdoc` delete/move
- `data/claimdoc` stage/commit
- app launch
- OpenFileDialog
- Scenario 8B
- cleanup
- code/XAML/ViewModel/test modification
- `FileNamePolicyService` modification
- allowlist change
- DB/SQLite/OCR/repository implementation
- git add/commit/reset/checkout/clean

## J. Verification For This Documentation Task

After creating `docs/154`, verify:

- `git diff --check`
- `git status --short`
- expected:
  - `?? data/`
  - `?? docs/154_POLICY_CLAIM_DATA_CLAIMDOC_GITIGNORE_DECISION.md`
- `.gitignore` unchanged
- project root `attachments/`: files=0
- project root `data/local`: files=0
- project root `runtime_test_document.*`: missing
- DB/SQLite unexpected file: none
- build/test: not run, documentation-only change

## K. Next Recommendation

Next recommended task:

```text
data/claimdoc .gitignore exact rule implementation instruction 작성
```

Proposed implementation review document:

```text
docs/155_POLICY_CLAIM_DATA_CLAIMDOC_GITIGNORE_IMPLEMENTATION_REVIEW.md
```
