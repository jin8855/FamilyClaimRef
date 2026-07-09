# DB SQLite OCR Repository Decision Gate Scope Plan

## A. Status Marker

POLICY_CLAIM_DB_SQLITE_OCR_REPOSITORY_DECISION_GATE_SCOPE_READY

## B. Selected Candidate

Selected next candidate: `DB/SQLite/OCR/repository decision gate planning`.

This batch is documentation-only. It does not approve, design in detail, or implement DB, SQLite, OCR, or repository work.

## C. Purpose

This document defines the exact scope for a decision gate review before any DB, SQLite, OCR, or repository implementation is considered.

The goal is to separate planning questions from implementation work and to keep the current validated JSON-based core intact until explicit user approval changes the direction.

## D. Current Baseline

- Project: `C:\EtcProject\FamilyClaimRef`
- Latest known commit at batch start: `d73d5f3 docs(familyclaimref): document remaining unapproved work gates`
- `Ui.*` key count: 56
- approved Korean resource copy applied: 21
- latest known full test: PASS 331
- cleanup dry-run: no project root candidates
- `data/claimdoc`: Never cleanup
- diagnostic summary formats: Keep deferred
- all remaining work gates: implementation allowed now = no

## E. Non-Scope

The following work is not authorized in this batch:

- DB implementation
- SQLite implementation
- OCR implementation
- repository implementation
- repository package addition
- JSON storage replacement
- migration implementation
- production data access
- `data/claimdoc` read, list, use, select, stage, commit, delete, or move
- cleanup execution
- app launch
- workflow execution
- Git staging
- Git commit

## F. Output Documents

This batch creates only:

- `docs/286_POLICY_CLAIM_DB_SQLITE_OCR_REPOSITORY_DECISION_GATE_SCOPE_PLAN.md`
- `docs/287_POLICY_CLAIM_DB_SQLITE_OCR_REPOSITORY_DECISION_GATE_MATRIX.md`
- `docs/288_POLICY_CLAIM_DB_SQLITE_OCR_REPOSITORY_DECISION_GATE_COMMIT_CANDIDATE_REVIEW.md`

## G. Gate Principle

Planning may identify future options, but implementation remains blocked until a separate explicit user approval names exact target files, allowed runtime actions, and validation requirements.
