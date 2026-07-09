# Policy Claim Thread Continuation Guide

Status: THREAD_CONTINUATION_GUIDE

Marker:
POLICY_CLAIM_THREAD_CONTINUATION_GUIDE_READY

## 1. Project

프로젝트 경로:

`C:\EtcProject\FamilyClaimRef`

## 2. Role Split

| Role | Responsibility |
|---|---|
| ChatGPT | next-step decision, scope approval, question answering, and handoff review |
| Codex | exact-scope local file read/write, validation commands, and evidence-based reporting |

## 3. Latest Commit

`046c5fc docs(familyclaimref): refresh current validation baseline`

## 4. Current Baseline

| Area | Current state |
|---|---|
| `Ui.*` key count | 56 |
| approved Korean resource copy | 21 applied |
| latest full test | PASS 331 |
| cleanup dry-run | no project root candidates |
| diagnostic summary formats | Keep deferred |

Diagnostic summary formats:

- `policy:{policyId}; document:{documentId}`: Keep deferred
- `claim:{claimId}; document:{documentId}`: Keep deferred

## 5. Absolute Prohibitions

- `data/claimdoc` access
- cleanup execution
- DB/SQLite/OCR/repository
- UI redesign/product UI shell
- diagnostic summary extraction
- `git add .`
- `git add -A`
- `git add --all`
- `git clean`
- broad stage/cleanup/reset commands

## 6. Current Safe Defaults

- Treat current MainWindow/UI as validation harness, not product UI shell.
- Keep diagnostic summary formats deferred unless final display model or diagnostic ownership is explicitly approved.
- Keep cleanup execution deferred because project root cleanup candidates are none.
- Keep `data/claimdoc/` as protected local real-document artifact.
- Continue using exact file lists for docs/code commits.
- Use command-local `safe.directory` only if plain git status/log fails with dubious ownership.

## 7. Next Candidate Paths

1. stop / handoff
2. DB/SQLite/OCR/repository decision gate planning, if explicitly approved
3. product UI shell planning, if explicitly approved
4. diagnostic summary extraction, only if final display model or diagnostic ownership explicitly approved
5. cleanup execution, not recommended because no root candidates

## 8. Suggested Continuation Prompt

```text
FamilyClaimRef 이어서 진행.

프로젝트:
C:\EtcProject\FamilyClaimRef

최신 기준:
046c5fc docs(familyclaimref): refresh current validation baseline

현재 baseline:
- Ui.* key count 56
- approved Korean resource copy 21 applied
- latest full test PASS 331
- cleanup dry-run no project root candidates
- diagnostic summary formats Keep deferred

절대 금지:
- data/claimdoc 접근
- cleanup execution
- DB/SQLite/OCR/repository
- UI redesign/product UI shell
- diagnostic summary extraction
- git add . / git add -A / git clean

다음 후보 중 하나만 선택해서 exact scope로 진행.
```

## 9. Handoff Judgment

The project is ready for a new chat handoff. The next step should be explicitly selected before any implementation, cleanup, DB, OCR, repository, diagnostic summary extraction, or product UI work starts.
