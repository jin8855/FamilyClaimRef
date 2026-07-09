# Remaining Unapproved Work Gate Decision Matrix

## A. Status Marker

POLICY_CLAIM_REMAINING_UNAPPROVED_WORK_GATE_DECISION_MATRIX_READY

## B. Purpose

This matrix records remaining unapproved work items and their execution gates for `FamilyClaimRef`.

No item in this matrix is authorized for implementation by this document.

## C. Decision Matrix

| Work item | Current state | Required approval | Required planning docs | Implementation allowed now | Risk |
|---|---|---|---|---|---|
| cleanup execution | Dry-run reported no project root cleanup candidates | explicit user approval | cleanup execution scope and exact target review | no | high |
| diagnostic summary extraction | Keep deferred until final display model and ownership are approved | explicit user approval | diagnostic display model and ownership decision | no | medium |
| DB/SQLite repository planning | Not approved for implementation; planning can be proposed separately | explicit user approval | DB/SQLite architecture planning docs | no | high |
| OCR planning | Not approved for implementation; planning can be proposed separately | explicit user approval | OCR boundary, privacy, and storage planning docs | no | high |
| repository implementation | Not approved; architecture and storage boundary must be decided first | explicit user approval | repository implementation scope and validation plan | no | high |
| business duplicate rule/copy | Boundary documented; runtime business rule not implemented | explicit user approval | business duplicate rule implementation plan | no | medium |
| product UI shell | Product UI shell not authorized | explicit user approval | product UI shell scope plan | no | high |
| UI redesign | Deferred until core validation closure and explicit product scope approval | explicit user approval | UI redesign scope and wireframe alignment plan | no | high |
| culture/dynamic language switching | Not authorized; resource infrastructure exists only for static copy extraction path | explicit user approval | culture switching architecture plan | no | medium |
| Ui.BusinessDuplicate.* | Candidate resource group only; implementation not authorized | explicit user approval | resource key ownership and UI usage plan | no | medium |
| Ui.Product.* | Candidate product shell copy group only; shell implementation not authorized | explicit user approval | product copy and product shell scope plan | no | high |
| Ui.ActionResult.* | Candidate result message group only; behavior integration not authorized | explicit user approval | action result behavior and display ownership plan | no | medium |

## D. Recommended Next Steps

| Work item group | Recommended next step |
|---|---|
| cleanup execution | No-op, because dry-run candidates are none. Keep execution deferred unless exact targets are later approved. |
| diagnostic summary extraction | Keep deferred until final display model and diagnostic ownership are explicitly approved. |
| DB/SQLite/OCR/repository | Separate architecture planning only. No implementation in the current state. |
| UI/product shell | Separate product scope planning only. No shell or redesign implementation in the current state. |
| dynamic language switching | Keep deferred until culture strategy and runtime switching ownership are approved. |
| resource candidate groups | Keep as candidate keys until the owning UI behavior is approved. |

## E. Protected Areas

- `data/claimdoc` remains protected.
- Runtime cleanup remains deferred.
- DB, SQLite, OCR, and repository implementation remain blocked.
- Product UI shell and UI redesign remain blocked.
- Diagnostic summary extraction remains blocked.

## F. Current Gate Result

All listed work items remain unapproved.

Implementation allowed now: no for all items.
