# Product UI Shell Phase 1 Resource Copy Terminology Candidate Table

## A. Status

PRODUCT_UI_SHELL_PHASE1_RESOURCE_COPY_TERMINOLOGY_CANDIDATE_TABLE_ONLY

## B. Marker

POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1_RESOURCE_COPY_TERMINOLOGY_CANDIDATE_TABLE_READY

## C. Baseline

- baseline commit: `574af1a docs(familyclaimref): plan product shell phase1 implementation preflight`
- current work type: documentation-only decision candidate planning

## D. Resource / Copy Rule

- existing `Ui.*` 56 baseline remains unchanged
- `Ui.Product.*` remains candidate only
- no `Ui.Product.*` key addition now
- product terminology remains candidate only
- final product copy is not approved
- validation-harness-only copy must not be productized

## E. Product Terminology Candidate Table

| Concept | Current/harness term | Product terminology candidate | Decision status | Implementation approved now | Notes |
|---|---|---|---|---|---|
| Policy target | `Policy target` / `보험 대상` | 보험 계약 | Candidate | no | generic term only, not insurer or policy number |
| Claim target | `Claim target` / `청구 대상` | 청구 건 | Candidate | no | generic claim unit only |
| Document registration | document registration / 문서 등록 | 문서 등록 | Candidate | no | current approved copy may remain usable if later approved |
| Document list | document list | 문서 목록 | Candidate | no | product list surface 후보 |
| Home / dashboard | Home / dashboard | 홈 | Candidate | no | product entry screen 후보 |
| Product navigation | navigation | navigation / 메뉴 후보 | Candidate | no | final menu wording not approved |
| Last registration summary | last registration summary | 최근 등록 요약 후보 | Candidate | no | runtime display model not finalized |
| Target selection | target selection | 대상 선택 또는 연결 대상 선택 후보 | Candidate | no | product flow wording decision needed |
| Document metadata | document metadata | 문서 정보 | Candidate | no | display title/type/reference date grouping 후보 |

## F. Ui.Product.* Candidate Key Table

| Candidate key | Candidate Korean copy | Purpose | Required before implementation | Implementation approved now |
|---|---|---|---|---|
| `Ui.Product.Navigation.Home` | 홈 | product navigation home label | approved `Ui.Product.*` value table | no |
| `Ui.Product.Navigation.DocumentRegistration` | 문서 등록 | product navigation document registration label | approved `Ui.Product.*` value table | no |
| `Ui.Product.Navigation.DocumentList` | 문서 목록 | product navigation document list label | approved `Ui.Product.*` value table | no |
| `Ui.Product.Home.Title` | 홈 | product home title | approved `Ui.Product.*` value table | no |
| `Ui.Product.DocumentRegistration.Title` | 문서 등록 | product document registration title | approved `Ui.Product.*` value table | no |
| `Ui.Product.DocumentList.Title` | 문서 목록 | product document list title | approved `Ui.Product.*` value table | no |
| `Ui.Product.DocumentList.EmptyMessage` | 등록된 문서가 없습니다. | product document list empty state candidate | approved `Ui.Product.*` value table | no |
| `Ui.Product.Shell.Title` | FamilyClaimRef | product shell title candidate | approved `Ui.Product.*` value table | no |

## G. Required Judgment

- Candidate key names are proposals only.
- Candidate Korean copy is proposal only.
- No key is added by this document.
- No copy is final by this document.
- Any `Ui.Product.*` implementation requires a later approved value table.

## H. Copy / Table Judgment

Product copy is not implementation-ready until the user approves final product terminology and a `Ui.Product.*` value table.

Phase 1 implementation should not hard-code product-facing copy.

If implementation proceeds without `Ui.Product.*` keys, it must be compile-only/non-runtime and avoid product-facing hard-coded copy.
