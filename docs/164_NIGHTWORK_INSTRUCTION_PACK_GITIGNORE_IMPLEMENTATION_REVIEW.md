# Nightwork Instruction Pack Gitignore Implementation Review

## A. Status Marker

NIGHTWORK_INSTRUCTION_PACK_GITIGNORE_IMPLEMENTED

## B. Implementation Scope

- `.gitignore`에 `/docs/nightwork_*/` 추가
- `docs/164` 생성
- nightwork folder not modified
- nightwork folder not deleted
- nightwork folder not staged/committed

## C. Decision Basis

- `docs/163` 기준 Option C 적용
- operational instruction pack을 local-only로 유지
- evidence docs와 분리

## D. .gitignore Review

- added rule: `/docs/nightwork_*/`
- broad `/docs/` ignore 없음
- `docs/*.md` ignore 없음
- `/data/claimdoc/` 유지
- `data/local/` 유지
- `attachments/` 유지
- DB/SQLite rules 유지

## E. Verification Results

- `git diff --check`: PASS
- `git status --short`: expected `.gitignore`, `docs/163`, `docs/164`
- `git check-ignore -v -- docs/nightwork_20260706/`: PASS
- `git check-ignore -v -- data/claimdoc/`: PASS
- project root `attachments/`: files=0
- project root `data/local`: files=0
- project root `runtime_test_document.*`: missing
- DB/SQLite unexpected file: 없음
- build/test: not run, `.gitignore`/docs-only change

## F. Commit Candidate

Exact file list 후보:

- `.gitignore`
- `docs/163_NIGHTWORK_INSTRUCTION_PACK_HANDLING_DECISION.md`
- `docs/164_NIGHTWORK_INSTRUCTION_PACK_GITIGNORE_IMPLEMENTATION_REVIEW.md`

Recommended commit message:

```text
chore(familyclaimref): ignore nightwork instruction packs
```

## G. Remaining Risks

- nightwork pack remains local-only
- not part of committed evidence chain
- Scenario 8B execution still requires explicit approval
