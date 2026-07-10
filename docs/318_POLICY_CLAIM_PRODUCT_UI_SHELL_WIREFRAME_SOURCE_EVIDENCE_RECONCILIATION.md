# Product UI Shell Wireframe Source Evidence Reconciliation

## A. Status

PRODUCT_UI_SHELL_WIREFRAME_SOURCE_EVIDENCE_RECONCILIATION_ONLY

## B. Marker

POLICY_CLAIM_PRODUCT_UI_SHELL_WIREFRAME_SOURCE_EVIDENCE_RECONCILIATION_READY

## C. Baseline Commit

`9e40fe5 docs(familyclaimref): plan product ui shell wireframe scope`

## D. Evidence Classification Rule

| Classification | Meaning |
|---|---|
| Source-confirmed | tracked docs/app/tests에서 직접 evidence 확인 |
| User-scope-confirmed | 사용자 발화로 final target scope에는 포함하지만 source detail evidence는 부족 |
| Unknown / needs source | source evidence와 사용자 구체 확인이 모두 부족 |
| Validation harness only | product screen으로 승격하지 않음 |
| Future-only | dependency approval 전 구현 금지 |
| Not product target | product target에서 제외 |

## E. Reconciliation Table

| Item | Previous status in docs/314 | New evidence | Reconciled status | Implementation gate | Notes |
|---|---|---|---|---|---|
| standalone Document detail | Unknown / needs source | `Document detail` direct hit is limited to docs/314; linked document evidence exists through policy/claim/document flows | User-scope-confirmed final target, needs source detail | implementation blocked | standalone detail screen source 확인 전 구현 금지 |
| Settings | Unknown / needs source | `Settings` hits are dev/test settings or docs/314/315 candidate references, not product settings screen evidence | User-scope-confirmed final target, needs source detail | implementation blocked | product settings source 확인 전 구현 금지 |
| Home / dashboard | Final target candidate | docs/177 records Home/dashboard; docs/315 places it in Phase 1 shell skeleton candidate | Source-confirmed final target | future product shell planning required | current `MainWindow`는 validation harness 유지 |
| Document registration | MVP candidate | app workflow and multiple docs confirm document registration flow and static UI/resource work | Source-confirmed final target | future product shell planning required | current harness flow를 product flow로 옮기는 것은 별도 승인 필요 |
| Document list | Final target scope | docs/314 records document list with checklist/state guide evidence | Source-confirmed final target | future product shell planning required | document list view candidate only |
| Policy contract list/detail | Final target scope | docs/10, docs/13, docs/14 references recorded in docs/314 | Source-confirmed final target | future Phase 2 product planning required | insurance search/list/detail boundaries remain to be refined |
| Claim case list/detail | Later phase / Final target scope | docs/10, docs/11, docs/14, docs/33 references recorded in docs/314 | Source-confirmed final target | future Phase 2 product planning required | claim start/history/detail boundaries remain to be refined |
| Claim preparation checklist | Final target scope | docs/13 and docs/33 references recorded in docs/314 | Source-confirmed final target | future Phase 2 product planning required | claim preparation and payment completion must stay separated |
| OCR candidate review | Later phase | docs/10, docs/11, docs/13, docs/300 references recorded; OCR storage/privacy remains unapproved | Future-only | future OCR/privacy approval required | no OCR implementation or storage approval |
| Search/filter | Final target scope | docs/10, docs/17, docs/33 references recorded in docs/314 | Source-confirmed final target | future DB/search approval may be required | exact search source and storage dependency must be decided later |
| Product navigation shell | Final target scope | docs/10 and docs/217 references recorded in docs/314 | Source-confirmed final target | future product shell planning required | `ProductShellWindow` candidate only |
| Validation harness | Validation harness only | docs/177 and docs/217 classify current `MainWindow` as validation harness | Validation harness only | no implementation, harness only | do not productize as final shell |
| Management harness | Validation harness only | docs/248 and docs/253 references recorded in docs/314 | Validation harness only | no implementation, harness only | synthetic target management is not product shell scope |

## F. Summary Counts

| Classification | Count |
|---|---:|
| Source-confirmed final target | 8 |
| User-scope-confirmed final target, needs source detail | 2 |
| Unknown / needs source | 0 |
| Validation harness only | 2 |
| Future-only | 1 |

## G. Reconciliation Judgment

The full wireframe scope remains accepted as final product target scope. That decision does not make every item source-confirmed or implementation-ready.

`standalone Document detail` and `Settings` must remain blocked for exact screen implementation until source detail is confirmed or the user explicitly provides the missing wireframe evidence.

`OCR candidate review` remains future-only. `Validation harness` and `Management harness` remain harness-only and must not be treated as final product screens.

`Ui.Product.*` is not added by this batch.
