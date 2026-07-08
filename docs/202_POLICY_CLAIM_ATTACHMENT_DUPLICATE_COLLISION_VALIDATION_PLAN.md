# Policy/Claim Attachment Duplicate Collision Validation Plan

## A. Status

Status: TEST_PLAN_ONLY

Marker:

POLICY_CLAIM_ATTACHMENT_DUPLICATE_COLLISION_VALIDATION_PLANNED

No code is modified by this document.

No test is implemented by this document.

No app launch is authorized by this document.

## B. Purpose

Record:
- negative validation covered rejection paths.
- duplicate/collision behavior needs separate scope because there are multiple meanings.
- UI redesign remains deferred.

## C. Definitions

Separate these concepts:

1. Physical filename collision:
   same generated destination filename already exists.

2. Duplicate index retry:
   service increments duplicate index to avoid overwrite.

3. Duplicate active link:
   same target/document active link already exists.

4. Business duplicate:
   same target + document type + display title or same source file registered multiple times.

## D. Candidate Test Cases

Plan tests for:

1. filename collision creates unique duplicate-indexed attachment without overwrite.
2. duplicate-index max limit rejects safely.
3. duplicate active policy link is rejected.
4. duplicate active claim link is rejected.
5. repeated registration of same source file is classified according to current product semantics.
6. unsupported duplicate/business semantics remain deferred if not defined.

## E. Safety Rules

- isolated runtime root only
- synthetic files only
- no data/claimdoc
- no real document data
- no app launch
- no UI automation
- no cleanup of default runtime evidence
- no FileNamePolicyService allowlist changes

## F. Acceptance Criteria

- no overwrite of existing attachment files
- duplicate-index behavior is deterministic
- rejected duplicate links do not create extra active links
- rollback behavior is verified where applicable
- project root remains clean
- dotnet build/test pass

## G. Planned Result Review

Future implementation batch must create:
- docs/205_POLICY_CLAIM_ATTACHMENT_DUPLICATE_COLLISION_VALIDATION_RESULT_REVIEW.md

## H. Planning Judgment

POLICY_CLAIM_ATTACHMENT_DUPLICATE_COLLISION_VALIDATION_PLAN_READY
