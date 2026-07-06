# Policy / Claim data/claimdoc Gitignore Commit Candidate Review

## A. Status Marker

POLICY_CLAIM_DATA_CLAIMDOC_GITIGNORE_COMMIT_CANDIDATE_READY

## B. Review Target

Review targets:

- `.gitignore`
- `docs/154_POLICY_CLAIM_DATA_CLAIMDOC_GITIGNORE_DECISION.md`
- `docs/155_POLICY_CLAIM_DATA_CLAIMDOC_GITIGNORE_IMPLEMENTATION_REVIEW.md`

This review document adds:

- `docs/156_POLICY_CLAIM_DATA_CLAIMDOC_GITIGNORE_COMMIT_CANDIDATE_REVIEW.md`

## C. Scope Review

- `.gitignore` exact rule change only: PASS
- docs/154~155 reviewed as commit candidates: PASS
- source code diff: none
- XAML diff: none
- ViewModel diff: none
- test diff: none
- runtime cleanup: not performed
- app launch: not performed
- OpenFileDialog: not performed
- Scenario 8B: not run
- runtime workflow: not run

## D. .gitignore Review

Confirmed:

- added exact rule: `/data/claimdoc/`
- whole `/data/` ignore: not added
- `data/local/` rule: preserved
- `attachments/` rule: preserved
- DB/SQLite rules: preserved
- duplicate `/data/claimdoc/` rule: none
- existing rule deletion: none

Relevant `.gitignore` rules:

```text
attachments/
data/local/
/data/claimdoc/
*.db
*.sqlite
*.sqlite3
```

Reviewed diff:

```diff
 # Local sensitive or user-provided files
 attachments/
 data/local/
+
+# Local real-document artifacts
+/data/claimdoc/

 # .NET build outputs
 bin/
 obj/
```

## E. data/claimdoc Safety Review

Confirmed:

- contents not inspected
- files not listed
- filenames not collected
- not staged
- not committed
- not deleted
- not moved
- not used
- `git check-ignore` used only on `data/claimdoc/` path itself
- child file paths under `data/claimdoc` not used for verification

## F. Verification Results

`git diff --check`:

```text
PASS
```

Note:

```text
warning: in the working copy of '.gitignore', LF will be replaced by CRLF the next time Git touches it
```

`git status --short` before this document:

```text
 M .gitignore
?? docs/154_POLICY_CLAIM_DATA_CLAIMDOC_GITIGNORE_DECISION.md
?? docs/155_POLICY_CLAIM_DATA_CLAIMDOC_GITIGNORE_IMPLEMENTATION_REVIEW.md
```

`git check-ignore -v -- data/claimdoc/`:

```text
.gitignore:6:/data/claimdoc/	data/claimdoc/
```

Tracked diff:

```text
M	.gitignore
```

Tracked source diff:

```text
none
```

Project root safety:

```text
attachments/: files=0
data/local: files=0
runtime_test_document.*: missing
```

DB/SQLite unexpected file:

```text
NONE
```

Actual personal sample targeted scan:

```text
none in .gitignore and docs/154~155 review targets
```

Build/test:

```text
not run, .gitignore/docs-only review
```

## G. Git Status Summary

Before this review document:

```text
 M .gitignore
?? docs/154_POLICY_CLAIM_DATA_CLAIMDOC_GITIGNORE_DECISION.md
?? docs/155_POLICY_CLAIM_DATA_CLAIMDOC_GITIGNORE_IMPLEMENTATION_REVIEW.md
```

Expected after this review document:

```text
 M .gitignore
?? docs/154_POLICY_CLAIM_DATA_CLAIMDOC_GITIGNORE_DECISION.md
?? docs/155_POLICY_CLAIM_DATA_CLAIMDOC_GITIGNORE_IMPLEMENTATION_REVIEW.md
?? docs/156_POLICY_CLAIM_DATA_CLAIMDOC_GITIGNORE_COMMIT_CANDIDATE_REVIEW.md
```

Unexpected and not observed:

- `?? data/`
- code changes
- XAML changes
- ViewModel changes
- test changes
- runtime artifact changes

## H. Commit Readiness

Commit readiness:

```text
ready
```

Reason:

- `.gitignore` and docs/154~156 are the only commit candidates.
- `.gitignore` adds only the exact `/data/claimdoc/` rule.
- Whole `/data/` ignore was not added.
- `data/local/` and `attachments/` rules are preserved.
- DB/SQLite ignore rules are preserved.
- `git diff --check` passed with LF/CRLF warning only.
- `git check-ignore` confirms `data/claimdoc/` is ignored by the exact rule.
- Project root `attachments/` and `data/local` are empty.
- Project root `runtime_test_document.*` is absent.
- `data/claimdoc` was not inspected, listed, staged, committed, moved, deleted, or used.
- No DB/SQLite unexpected file was reported.
- No actual personal sample was found in the reviewed targets.
- Build/test is not required for this .gitignore/docs-only review.

## I. Commit Candidate Exact File List

Include only:

- `.gitignore`
- `docs/154_POLICY_CLAIM_DATA_CLAIMDOC_GITIGNORE_DECISION.md`
- `docs/155_POLICY_CLAIM_DATA_CLAIMDOC_GITIGNORE_IMPLEMENTATION_REVIEW.md`
- `docs/156_POLICY_CLAIM_DATA_CLAIMDOC_GITIGNORE_COMMIT_CANDIDATE_REVIEW.md`

Do not include:

- `data/`
- `data/claimdoc`
- runtime files
- temp files
- code files
- XAML files
- ViewModel files
- test files

## J. Recommended Commit Message

```text
chore(familyclaimref): ignore local claim documents
```

## K. Remaining Risks / Follow-up

- `data/claimdoc` remains a local real-document artifact.
- `.gitignore` reduces Git status noise but does not secure files outside Git.
- If `data/` contains other untracked child paths, future status may show `data/` again.
- Scenario 8B claim target remains untested.
- Scenario 8A artifact cleanup remains deferred.
- Temp `.txt` and `.png` cleanup remains deferred.
- Runtime artifacts under `%LOCALAPPDATA%` remain unless separately cleaned.
