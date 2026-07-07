# Policy / Claim Scenario 8B Docs Commit Candidate Review

## A. Status Marker

COMMIT_CANDIDATE_READY

## B. Purpose

This document reviews the docs-only Scenario 8B planning set as a commit candidate.

This document does not stage or commit files.

## C. Reviewed Source State

Latest commit:

```text
5615736 chore(familyclaimref): ignore local claim documents
```

Expected current untracked planning documents:

```text
?? docs/157_POLICY_CLAIM_SCENARIO8B_CLAIM_TARGET_SCOPE_DECISION.md
?? docs/158_POLICY_CLAIM_SCENARIO8B_CLAIM_TARGET_EXECUTION_INSTRUCTION.md
?? docs/nightwork_20260706/
```

`docs/160_POLICY_CLAIM_SCENARIO8B_EXECUTION_INSTRUCTION_READY_REVIEW.md` is created by the current night work after the above baseline.

Tracked source diff before this commit review:

```text
none
```

## D. Primary Commit Candidate Exact File List

Recommended primary evidence commit candidate:

```text
docs/157_POLICY_CLAIM_SCENARIO8B_CLAIM_TARGET_SCOPE_DECISION.md
docs/158_POLICY_CLAIM_SCENARIO8B_CLAIM_TARGET_EXECUTION_INSTRUCTION.md
docs/160_POLICY_CLAIM_SCENARIO8B_EXECUTION_INSTRUCTION_READY_REVIEW.md
```

Rationale:

- `docs/157` records the Scenario 8B scope decision.
- `docs/158` records the future execution instruction.
- `docs/160` records the readiness and baseline review.

## E. Excluded From Primary Candidate

Excluded by default:

```text
docs/nightwork_20260706/*
docs/161_POLICY_CLAIM_SCENARIO8B_DOCS_COMMIT_CANDIDATE_REVIEW.md
docs/162_POLICY_CLAIM_SCENARIO8_ARTIFACT_CLEANUP_SCOPE_DESIGN.md
```

Reason:

- `docs/nightwork_20260706/*` is operational night-work guidance, not primary Scenario 8B evidence.
- `docs/161` is this commit candidate review and may be committed separately if the user wants commit review records included.
- `docs/162` is optional cleanup scope design and should be reviewed separately from the Scenario 8B plan commit.

Never include:

```text
data/
data/claimdoc
attachments/
data/local
runtime artifacts
temp synthetic files
```

## F. Safety Review

| Check | Result |
|---|---|
| `data/claimdoc` ignored by exact rule | PASS |
| `data/claimdoc` contents inspected | NO |
| `data/claimdoc` file list collected | NO |
| project root `attachments/` files | 0 |
| project root `data/local` files | 0 |
| project root `runtime_test_document.*` | missing |
| unexpected DB/SQLite file | none |
| tracked source diff | none |
| app launch | none |
| OpenFileDialog | none |
| Scenario 8B runtime execution | none |
| cleanup | none |
| commit | none |

## G. Check Results

`git check-ignore -v -- data/claimdoc/`:

```text
.gitignore:6:/data/claimdoc/	data/claimdoc/
```

`git diff --check`:

```text
PASS in final verification after creating docs/160~162.
```

Build/test:

```text
not run, docs-only commit candidate review
```

## H. Candidate Decision

Decision:

```text
COMMIT_CANDIDATE_READY
```

Scope:

- docs-only
- no code
- no XAML
- no ViewModel
- no tests
- no runtime execution
- no cleanup

Recommended commit message:

```text
docs(familyclaimref): add scenario8b claim target plan
```

## I. Remaining User Decisions

- Whether to commit only the primary candidate list.
- Whether to include `docs/161` as a review record in a separate docs commit.
- Whether to commit `docs/nightwork_20260706/*`.
- Whether to keep `docs/162` separate from the Scenario 8B plan commit.
