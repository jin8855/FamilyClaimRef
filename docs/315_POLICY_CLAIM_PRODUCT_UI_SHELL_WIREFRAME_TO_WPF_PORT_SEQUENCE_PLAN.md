# Policy Claim Product UI Shell Wireframe To WPF Port Sequence Plan

## A. Status

PRODUCT_UI_SHELL_WIREFRAME_TO_WPF_PORT_SEQUENCE_PLAN_ONLY

## B. Marker

POLICY_CLAIM_PRODUCT_UI_SHELL_WIREFRAME_TO_WPF_PORT_SEQUENCE_READY

## C. 기준 Commit

`7d24fb1 docs(familyclaimref): consolidate storage decision track state`

## D. 목적

full wireframe scope를 실제 WPF/XAML port 순서로 옮기기 전에, 구현 없이 planning 단계에서 후보 순서를 정리한다.

## E. Recommended Shell Strategy

- keep current MainWindow as validation harness until explicit product shell implementation approval
- introduce separate ProductShellWindow or equivalent product shell only in a later exact implementation batch
- do not replace MainWindow in the first product planning batch
- keep product shell and validation harness separated until a deliberate migration plan exists

## F. Phase Sequence

| Phase | Goal | Candidate screens | Candidate functions | Implementation allowed now | Required approval |
|---|---|---|---|---|---|
| Phase 0 | wireframe inventory and mapping only | docs/313~316 planning set | screen/function inventory, WPF mapping candidates | docs only | none beyond this planning batch |
| Phase 1 | product shell skeleton and core document flow | Product navigation shell, Home/dashboard, Document registration, Document list | register document, link document to policy/claim, edit document metadata | no | explicit product shell skeleton implementation approval |
| Phase 2 | policy/claim case management and checklist | Policy contract list/detail, Claim case list/detail, Claim preparation checklist, Search/filter | create policy contract, create claim case, claim checklist, document filtering | no | explicit Phase 2 product screen implementation approval |
| Phase 3 | search/OCR/DB-dependent extensions | OCR candidate review, extended search/filter | OCR candidate extraction/review, advanced search | no | explicit OCR/privacy/storage and DB/repository approval if needed |
| Phase 4 | polish, UI redesign, dynamic language/culture, product hardening | all product shell screens | UI redesign, culture strategy, accessibility and hardening | no | explicit UI redesign/product hardening approval |

## G. WPF Mapping Candidates

| Candidate | Current status | Implementation approved now | Notes |
|---|---|---|---|
| ProductShellWindow candidate | future candidate only | no | 별도 exact implementation batch 필요 |
| Product navigation view model candidate | future candidate only | no | navigation ownership 결정 필요 |
| Document registration product view candidate | future candidate only | no | current MainWindow harness와 분리 필요 |
| Document list view candidate | future candidate only | no | 문서함 조회/관리 기준 반영 |
| Policy contract view candidate | future candidate only | no | 보험 검색/관리/상세 경계 결정 필요 |
| Claim case view candidate | future candidate only | no | 청구 시작, 이력, 상세 경계 결정 필요 |
| Checklist view candidate | future candidate only | no | 청구 준비 완료와 지급 완료 표현 분리 |
| Settings view candidate | Unknown / needs source | no | source evidence 확인 전 구현 금지 |

## H. Resource / Copy Policy

- Ui.Product.* key family remains future candidate only.
- existing Ui.* 56 baseline remains unchanged.
- validation-harness-only management copy should not be productized without product shell copy table.
- product terminology must be decided before copy implementation.
- suggested product terminology candidate:
  - Policy target -> 보험 계약
  - Claim target -> 청구 건
- 이번 batch에서는 위 terminology candidate를 확정하지 않는다.
- Ui.Product.* addition is not approved for implementation.
- Product terminology candidates are planning only.
- Resource/copy implementation requires explicit user approval.

## I. Dependency Policy

- JSON source of truth remains current baseline.
- DB/SQLite/repository implementation is not required for Phase 1 planning.
- OCR remains future-only.
- `data/claimdoc` remains protected and never used for product shell validation.

## J. Port Judgment

- WPF/XAML port is not started by this document.
- MainWindow replacement is not approved.
- product shell implementation remains blocked.
- UI redesign implementation remains blocked.
- explicit user approval is required before any product shell code batch.
