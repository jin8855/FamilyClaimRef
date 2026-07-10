# Policy Claim Product UI Shell Wireframe Screen Function Inventory

## A. Status

PRODUCT_UI_SHELL_WIREFRAME_SCREEN_FUNCTION_INVENTORY_ONLY

## B. Marker

POLICY_CLAIM_PRODUCT_UI_SHELL_WIREFRAME_SCREEN_FUNCTION_INVENTORY_READY

## C. 기준 Commit

`7d24fb1 docs(familyclaimref): consolidate storage decision track state`

## D. Inventory Rule

- source에서 확인한 wireframe 화면과 기능만 Confirmed로 기록한다.
- 사용자 발화로 확인한 최초 wireframe 전체 포함 원칙은 Product scope decision으로 기록한다.
- source에서 찾지 못한 화면과 기능은 Unknown / needs source로 기록한다.
- 화면과 기능을 임의로 invent하지 않는다.
- Source evidence only: source-confirmed items can be marked as product target candidates.
- Unknown / needs source: missing source evidence remains Unknown / needs source.
- No invented screen/function: do not invent screens or functions from assumptions.
- This inventory is planning only and is not approved for implementation.

## E. Screen Inventory

| Screen / Area | Source evidence | Product target status | MVP phase candidate | WPF mapping candidate | Notes |
|---|---|---|---|---|---|
| Home / dashboard | `docs/10_IA_MERMAID.md`, `docs/13_SCREEN_REVIEW_CHECKLIST.md`, `docs/33_UI_STATE_AND_CONFIRMATION_MESSAGE_GUIDE.md` | Final target scope | Phase 1 | Product home/dashboard view candidate | 4개 대메뉴 진입과 dashboard summary 후보 |
| Document registration | `docs/10_IA_MERMAID.md`, `docs/13_SCREEN_REVIEW_CHECKLIST.md`, `docs/33_UI_STATE_AND_CONFIRMATION_MESSAGE_GUIDE.md`, `docs/248_POLICY_CLAIM_FINAL_KOREAN_COPY_RESOURCE_VALUE_INVENTORY_PLAN.md` | MVP candidate | Phase 1 | Document registration product view candidate | 현재 harness 기능을 product flow로 재배치해야 함 |
| Document list | `docs/13_SCREEN_REVIEW_CHECKLIST.md`, `docs/22_WIREFRAME_V5_5_ACTION_ALIGNMENT_AND_DETAIL_REVIEW.md`, `docs/33_UI_STATE_AND_CONFIRMATION_MESSAGE_GUIDE.md` | Final target scope | Phase 1 | Document list view candidate | 문서함은 조회/관리 화면으로 유지 |
| Document detail | linked document evidence exists in policy/claim flows, standalone source not confirmed | Unknown / needs source | Unknown | Document detail view candidate | standalone detail screen source 확인 필요 |
| Policy contract list | `docs/10_IA_MERMAID.md`, `docs/13_SCREEN_REVIEW_CHECKLIST.md`, `docs/14_WIREFRAME_V2_REVIEW.md` | Final target scope | Phase 2 | Policy contract list view candidate | 보험 검색/보험 목록과 관리 목록을 구분해야 함 |
| Policy contract detail | `docs/10_IA_MERMAID.md`, `docs/13_SCREEN_REVIEW_CHECKLIST.md`, `docs/14_WIREFRAME_V2_REVIEW.md` | Final target scope | Phase 2 | Policy contract detail view candidate | 보험 상세와 연결 문서 후보 포함 |
| Claim case list | `docs/10_IA_MERMAID.md`, `docs/11_USER_FLOW_MERMAID.md` | Later phase | Phase 2 | Claim case list view candidate | 이력 보기와 청구 사건 후보 경계 확인 필요 |
| Claim case detail | `docs/14_WIREFRAME_V2_REVIEW.md`, `docs/33_UI_STATE_AND_CONFIRMATION_MESSAGE_GUIDE.md` | Final target scope | Phase 2 | Claim case detail view candidate | 청구 시작/청구 사건 입력과 detail 경계 결정 필요 |
| Claim preparation checklist | `docs/13_SCREEN_REVIEW_CHECKLIST.md`, `docs/33_UI_STATE_AND_CONFIRMATION_MESSAGE_GUIDE.md` | Final target scope | Phase 2 | Claim checklist view candidate | 청구 준비 완료와 지급 완료를 분리해야 함 |
| Settings | source not confirmed in inspected docs/app/tests | Unknown / needs source | Unknown | Settings view candidate | wireframe source 확인 전 구현 금지 |
| Validation harness | `docs/177_POLICY_CLAIM_UI_REDESIGN_DEFER_UNTIL_CORE_VALIDATION_DECISION.md`, `docs/217_POLICY_CLAIM_UI_PHASE_ENTRY_DECISION.md` | Validation harness only | Not MVP | Existing MainWindow validation harness | product shell과 혼동 금지 |
| Management harness | `docs/248_POLICY_CLAIM_FINAL_KOREAN_COPY_RESOURCE_VALUE_INVENTORY_PLAN.md`, `docs/253_POLICY_CLAIM_FINAL_KOREAN_COPY_CANDIDATE_TABLE.md` | Validation harness only | Not MVP | Existing management panel candidate | harness-only copy는 product copy로 승격하지 않음 |
| OCR candidate review | `docs/10_IA_MERMAID.md`, `docs/11_USER_FLOW_MERMAID.md`, `docs/13_SCREEN_REVIEW_CHECKLIST.md`, `docs/300_POLICY_CLAIM_OCR_BOUNDARY_PRIVACY_STORAGE_OPTIONS_AND_RECOMMENDATION.md` | Later phase | Phase 3 | OCR candidate review view candidate | OCR implementation/storage는 승인되지 않음 |
| Search/filter | `docs/10_IA_MERMAID.md`, `docs/17_WIREFRAME_V5_REVIEW.md`, `docs/33_UI_STATE_AND_CONFIRMATION_MESSAGE_GUIDE.md` | Final target scope | Phase 2 | Search/filter view candidate | 보험 검색, 이력 검색, 조건 필터 후보를 구분해야 함 |
| Product navigation shell | `docs/10_IA_MERMAID.md`, `docs/217_POLICY_CLAIM_UI_PHASE_ENTRY_DECISION.md` | Final target scope | Phase 1 | ProductShellWindow candidate / product navigation view model candidate | 구현 승인 전 candidate로만 유지 |

## F. Function Inventory

| Function | Source evidence | Product target status | MVP phase candidate | Depends on | Notes |
|---|---|---|---|---|---|
| create policy contract | `docs/10_IA_MERMAID.md`, `docs/13_SCREEN_REVIEW_CHECKLIST.md`, `docs/248_POLICY_CLAIM_FINAL_KOREAN_COPY_RESOURCE_VALUE_INVENTORY_PLAN.md` | Final target scope | Phase 2 | existing JSON storage, product UI shell | harness create policy와 product 보험 계약 생성은 분리 필요 |
| create claim case | `docs/10_IA_MERMAID.md`, `docs/11_USER_FLOW_MERMAID.md`, `docs/33_UI_STATE_AND_CONFIRMATION_MESSAGE_GUIDE.md` | Final target scope | Phase 2 | existing JSON storage, product UI shell | 청구 시작과 청구 사건 관리 경계 결정 필요 |
| register document | `docs/10_IA_MERMAID.md`, `docs/13_SCREEN_REVIEW_CHECKLIST.md`, `docs/248_POLICY_CLAIM_FINAL_KOREAN_COPY_RESOURCE_VALUE_INVENTORY_PLAN.md` | MVP candidate | Phase 1 | existing JSON storage, product UI shell | 현재 validation harness flow의 product port 후보 |
| link document to policy/claim | `docs/10_IA_MERMAID.md`, `docs/94_POLICY_CLAIM_DOCUMENT_LINK_WORKFLOW_SCOPE_DESIGN.md` | MVP candidate | Phase 1 | existing JSON storage, product UI shell | 보험 문서와 청구 서류 연결 흐름 분리 |
| edit document metadata | `docs/248_POLICY_CLAIM_FINAL_KOREAN_COPY_RESOURCE_VALUE_INVENTORY_PLAN.md`, `docs/253_POLICY_CLAIM_FINAL_KOREAN_COPY_CANDIDATE_TABLE.md` | MVP candidate | Phase 1 | existing JSON storage, product UI shell | display title, document type, reference date 후보 |
| document list filtering | `docs/22_WIREFRAME_V5_5_ACTION_ALIGNMENT_AND_DETAIL_REVIEW.md`, `docs/33_UI_STATE_AND_CONFIRMATION_MESSAGE_GUIDE.md` | Final target scope | Phase 2 | product UI shell | 문서함 조회/관리 중심 |
| claim checklist | `docs/13_SCREEN_REVIEW_CHECKLIST.md`, `docs/33_UI_STATE_AND_CONFIRMATION_MESSAGE_GUIDE.md` | Later phase | Phase 2 | final display model | 지급 가능 확정 표현 금지 |
| file attachment handling | existing document registration workflow docs and current app service boundary | MVP candidate | Phase 1 | existing JSON storage | 실제 file picker/product UI 실행은 별도 승인 필요 |
| OCR candidate extraction/review | `docs/10_IA_MERMAID.md`, `docs/300_POLICY_CLAIM_OCR_BOUNDARY_PRIVACY_STORAGE_OPTIONS_AND_RECOMMENDATION.md` | Later phase | Phase 3 | future OCR | OCR/privacy/storage approval 전 구현 금지 |
| search | `docs/10_IA_MERMAID.md`, `docs/17_WIREFRAME_V5_REVIEW.md` | Final target scope | Phase 2 | existing JSON storage, future repository candidate | DB/SQLite/repository가 Phase 1 필수는 아님 |
| settings | source not confirmed in inspected docs/app/tests | Unknown / needs source | Unknown | unknown | wireframe source 확인 필요 |
| cleanup/admin/debug | `docs/275_POLICY_CLAIM_POST_RESOURCE_COPY_CLEANUP_CURRENT_STATE_REVIEW.md`, `docs/284_POLICY_CLAIM_REMAINING_UNAPPROVED_WORK_GATE_DECISION_MATRIX.md` | Validation harness only | Not MVP | explicit cleanup approval | product feature로 승격하지 않음 |
| validation harness synthetic target management | `docs/248_POLICY_CLAIM_FINAL_KOREAN_COPY_RESOURCE_VALUE_INVENTORY_PLAN.md`, `docs/253_POLICY_CLAIM_FINAL_KOREAN_COPY_CANDIDATE_TABLE.md` | Validation harness only | Not MVP | validation harness | synthetic target management는 product shell 기능이 아님 |

## G. Inventory Judgment

- initial wireframe full scope is product target, but implementation phase must be explicit.
- validation harness-only screens must not be confused with final product screens.
- OCR candidate review remains future-only unless OCR/privacy/storage approval changes.
- DB/SQLite/repository is not required for Phase 1 unless user explicitly changes storage direction.
- Unknown / needs source items require wireframe source confirmation before implementation.
