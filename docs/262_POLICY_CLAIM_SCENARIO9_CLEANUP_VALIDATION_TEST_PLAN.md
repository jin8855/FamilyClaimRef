# Policy Claim Scenario 9 Cleanup Validation Test Plan

## 1. Status

CLEANUP_VALIDATION_TEST_PLAN_ONLY

## 2. Marker

POLICY_CLAIM_SCENARIO9_CLEANUP_VALIDATION_TEST_PLAN_PLANNED

## 3. Scope Boundary

- no cleanup executed by this document
- no code/test/XAML/ViewModel/resource modified by this document
- no runtime metadata deletion by this document
- no runtime attachment deletion by this document
- no `data/claimdoc/` access by this document
- no DB/SQLite/OCR/repository implementation by this document

## 4. Future Cleanup Validation Phases

1. pre-cleanup `git status --short` clean 확인
2. exact cleanup candidate path list 생성
3. dry-run report 생성
4. user approval 확인
5. exact cleanup command 실행
6. post-cleanup artifact count 확인
7. `data/claimdoc/` untouched 확인
8. build/test 재검증
9. result review 문서 생성

## 5. Future Allowed Validation Commands

- `git status --short`
- `git diff --check`
- `git check-ignore -v -- data/claimdoc/`
- `git check-ignore -v -- docs/nightwork_20260706/`
- `dotnet build FamilyClaimRef.sln`
- `dotnet test FamilyClaimRef.sln`

## 6. Future Cleanup Must Not Run

- `git clean`
- `git reset`
- `git checkout`
- recursive delete without exact path approval
- `data/claimdoc/` delete/move/list/read
- DB/SQLite/OCR/repository changes
- source-controlled docs/app/tests deletion
- runtime root deletion without exact path approval
- wildcard deletion

## 7. Forbidden Validation

- no app launch
- no OpenFileDialog
- no manual workflow
- no screenshot/visual automation
- no `data/claimdoc/`
- no DB/SQLite/OCR/repository
- no UI redesign
- no product UI shell
- no deferred diagnostic summary format extraction

## 8. Future Dry-Run Report Requirements

future cleanup dry-run report는 최소한 다음 항목을 포함해야 한다.

- cleanup candidate exact path list
- each path artifact class
- each path source of evidence
- whether each path is source-controlled
- whether each path is under project root
- whether each path is under isolated runtime root
- whether each path is under `data/claimdoc/`
- whether each path is user-approved for deletion
- command preview without deletion
- rollback or recovery note, if applicable

## 9. Future Exact Cleanup Command Rules

- command는 exact path만 대상으로 한다.
- wildcard cleanup은 금지한다.
- directory deletion은 exact directory approval 없이는 금지한다.
- recursive deletion은 exact path approval 없이는 금지한다.
- `data/claimdoc/`는 cleanup candidate list에 포함하지 않는다.
- command 실행 전후 `git status --short`와 root artifact count를 기록한다.

## 10. Future Result Review

future cleanup result review 문서 후보:

- `docs/264_POLICY_CLAIM_SCENARIO9_CLEANUP_POLICY_RESULT_REVIEW.md`

이 문서는 cleanup이 별도 승인되고 실행된 경우에만 생성한다.

## 11. Current Batch Validation Policy

이번 batch는 documentation-only cleanup policy review다.

- build/test는 실행하지 않는다.
- cleanup command는 실행하지 않는다.
- app launch는 실행하지 않는다.
- source/test/resource 변경은 수행하지 않는다.
- git add/stage/commit은 수행하지 않는다.

## 12. Final Marker

POLICY_CLAIM_SCENARIO9_CLEANUP_VALIDATION_TEST_PLAN_READY
