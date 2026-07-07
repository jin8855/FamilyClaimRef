# Nightwork Instruction Pack Gitignore Commit Candidate Review

## A. Status Marker

NIGHTWORK_INSTRUCTION_PACK_GITIGNORE_COMMIT_CANDIDATE_READY

## B. Review Target

- `.gitignore`
- `docs/163_NIGHTWORK_INSTRUCTION_PACK_HANDLING_DECISION.md`
- `docs/164_NIGHTWORK_INSTRUCTION_PACK_GITIGNORE_IMPLEMENTATION_REVIEW.md`

## C. Scope Review

- `.gitignore` exact pattern change 확인
- `docs/163~164` review scope 확인
- source code diff 없음
- XAML diff 없음
- ViewModel diff 없음
- test diff 없음
- runtime cleanup 없음
- app launch/OpenFileDialog 없음
- Scenario 8B execution 없음

## D. .gitignore Review

- added exact pattern: `/docs/nightwork_*/`
- broad `/docs/` ignore: not added
- `docs/*.md` ignore: not added
- `/data/claimdoc/`: preserved
- `data/local/`: preserved
- `attachments/`: preserved
- DB/SQLite rules: preserved
- duplicate rule: none
- existing rule deletion: none

## E. Nightwork Safety Review

- `docs/nightwork_20260706/` ignored
- not staged
- not committed
- not modified
- not deleted
- operational instruction pack local-only

## F. data/claimdoc Safety Review

- `data/claimdoc` ignored
- contents not inspected
- files not listed
- not staged
- not committed
- not used
- not deleted/moved

## G. Verification Results

| Check | Result |
|---|---|
| `git diff --check` | PASS, LF/CRLF warning only |
| `git status --short` before this document | `M .gitignore`, `?? docs/163...`, `?? docs/164...` |
| `git status --short` after this document | expected `.gitignore`, `docs/163`, `docs/164`, `docs/165` only |
| `git check-ignore -v -- docs/nightwork_20260706/` | PASS, `.gitignore:9:/docs/nightwork_*/` |
| `git check-ignore -v -- data/claimdoc/` | PASS, `.gitignore:6:/data/claimdoc/` |
| tracked source diff | `.gitignore` only |
| project root `attachments/` | files=0 expected |
| project root `data/local` | files=0 expected |
| project root `runtime_test_document.*` | missing expected |
| DB/SQLite unexpected file | none expected |
| build/test | not run, `.gitignore`/docs-only review |

## H. Git Status Summary

Document creation before expected:

```text
 M .gitignore
?? docs/163_NIGHTWORK_INSTRUCTION_PACK_HANDLING_DECISION.md
?? docs/164_NIGHTWORK_INSTRUCTION_PACK_GITIGNORE_IMPLEMENTATION_REVIEW.md
```

Document creation after expected:

```text
 M .gitignore
?? docs/163_NIGHTWORK_INSTRUCTION_PACK_HANDLING_DECISION.md
?? docs/164_NIGHTWORK_INSTRUCTION_PACK_GITIGNORE_IMPLEMENTATION_REVIEW.md
?? docs/165_NIGHTWORK_INSTRUCTION_PACK_GITIGNORE_COMMIT_CANDIDATE_REVIEW.md
```

Unexpected:

- `docs/nightwork_20260706/` should not appear.
- `data/` should not appear.
- code/XAML/ViewModel/test changes should not appear.

## I. Commit Readiness

commit readiness:

```text
ready
```

reason:

- `.gitignore` and `docs/163~165` are the only commit candidates.
- `/docs/nightwork_*/` is an exact pattern.
- broad docs ignore was not added.
- `/data/claimdoc/` was preserved.
- `git diff --check` passed.
- check-ignore checks passed.
- project root `attachments/` and `data/local` remain clean.
- nightwork folder is ignored and not staged.
- `data/claimdoc` was not inspected, listed, staged, or used.
- no DB/SQLite unexpected file is expected.

## J. Commit Candidate Exact File List

Expected candidate:

- `.gitignore`
- `docs/163_NIGHTWORK_INSTRUCTION_PACK_HANDLING_DECISION.md`
- `docs/164_NIGHTWORK_INSTRUCTION_PACK_GITIGNORE_IMPLEMENTATION_REVIEW.md`
- `docs/165_NIGHTWORK_INSTRUCTION_PACK_GITIGNORE_COMMIT_CANDIDATE_REVIEW.md`

Do not include:

- `docs/nightwork_20260706/`
- `data/`
- `data/claimdoc`
- runtime files
- temp files
- code/XAML/ViewModel/test files

## K. Recommended Commit Message

```text
chore(familyclaimref): ignore nightwork instruction packs
```

## L. Remaining Risks / Follow-up

- nightwork pack remains local-only
- `.gitignore` reduces Git noise but does not preserve nightwork pack in commit history
- Scenario 8B claim target execution still requires explicit approval
- Scenario 8A/8B artifact cleanup remains deferred
