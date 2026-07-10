# Product UI Shell Phase 1 Implementation Preflight Scope Plan

## A. Status

PRODUCT_UI_SHELL_PHASE1_IMPLEMENTATION_PREFLIGHT_SCOPE_PLAN_ONLY

## B. Marker

POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1_IMPLEMENTATION_PREFLIGHT_SCOPE_READY

## C. Baseline

- baseline commit: `6cee3a9 docs(familyclaimref): plan product shell phase1 scope`
- project: `FamilyClaimRef`
- current work type: documentation-only implementation preflight planning

## D. Purpose

This document records the preflight scope before any Product UI Shell Phase 1 code batch.

The goal is to clarify exact future file candidates, entry/startup strategy, composition boundary candidates, resource/copy prerequisites, validation gates, and risk classification before implementation is approved.

This document does not approve implementation.

## E. Current Baseline

| Item | Current state |
|---|---|
| Phase 1 source-confirmed/core screen scope | planned |
| Product navigation shell | future candidate |
| Home/dashboard | future candidate |
| Document registration product view | future candidate |
| Document list view | future candidate |
| standalone Document detail | excluded from Phase 1 exact target |
| Settings | excluded from Phase 1 exact target |
| current `MainWindow` | remains validation harness |
| ProductShell implementation | not approved |
| `ProductShellWindow` addition | not approved |
| `MainWindow` replacement | not approved |
| `Ui.Product.*` addition | not approved |
| product terminology finalization | not approved |
| latest known full test | PASS 331 |
| storage source of truth | JSON retained |
| `data/claimdoc` | protected / no operational use |

## F. Included In This Preflight

- future exact implementation file candidates
- ProductShell entry/startup strategy options
- ProductShell composition boundary candidates
- resource/copy prerequisite assessment
- validation/test gate planning
- risk and blocker classification

## G. Explicitly Excluded

- product shell implementation
- `ProductShellWindow` creation
- XAML port
- `MainWindow` replacement
- App startup change
- code/test/resource/project changes
- `Ui.Product.*` addition
- DB/SQLite/OCR/repository/migration implementation
- `data/claimdoc` access
- cleanup execution
- app launch
- OpenFileDialog execution
- manual workflow execution
- screenshot or visual automation
- diagnostic summary extraction
- git staging or commit

## H. Scope Judgment

This batch is preflight planning only.

Implementation remains blocked. If blockers remain after this preflight, the next step is additional planning or explicit user decision, not implementation.

