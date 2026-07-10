# Product UI Shell Phase 1 Entry Strategy Decision Candidate

## A. Status

PRODUCT_UI_SHELL_PHASE1_ENTRY_STRATEGY_DECISION_CANDIDATE_ONLY

## B. Marker

POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1_ENTRY_STRATEGY_DECISION_CANDIDATE_READY

## C. Baseline

- baseline commit: `574af1a docs(familyclaimref): plan product shell phase1 implementation preflight`
- current work type: documentation-only decision candidate planning

## D. Entry Strategy Decision Table

| Entry strategy | Candidate decision | Runtime exposure | MainWindow impact | App startup impact | Implementation approved now | Reason |
|---|---|---|---|---|---|---|
| Compile-only `ProductShellWindow`, no runtime entry yet | selected future implementation candidate | none | none | none | no | build/test validation 후보로만 적합하며 runtime 노출을 열지 않는다 |
| `ProductShellWindow` opened from validation harness command/button | rejected for Phase 1 | validation harness command/button would expose runtime entry | modifies validation harness | none or minor | no | validation harness UI change is not approved |
| `ProductShellWindow` as app startup replacement | rejected for Phase 1 | full runtime exposure | replaces `MainWindow` | startup replacement required | no | `MainWindow` replacement is not approved |
| Command-line or config-driven product shell startup | defer | conditional runtime exposure | no direct replacement if guarded | startup logic change required | no | startup policy and runtime mode selection are not approved |
| Separate future product executable/project | defer | separate runtime surface | none | project/solution change required | no | project structure change is not approved |

## E. Required Judgment

- implementation approved now: no for all rows
- selected future implementation candidate is not implementation approval
- `MainWindow` replacement: no
- App startup change: no
- validation harness command/button entry: no
- app launch/manual workflow: no
- compile-only shell is a candidate for future build/test validation only
- runtime entry exposure remains none until separately approved

## F. Entry Strategy Judgment

Recommended future entry strategy:

```text
Compile-only ProductShellWindow, no runtime entry yet.
```

This document does not approve `ProductShellWindow` creation.

This document does not approve App startup changes.

This document does not approve runtime exposure.
