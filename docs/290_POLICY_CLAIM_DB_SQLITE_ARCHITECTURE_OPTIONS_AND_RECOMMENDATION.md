# DB SQLite Architecture Options And Recommendation

## A. Status

DB_SQLITE_ARCHITECTURE_OPTIONS_AND_RECOMMENDATION_ONLY

## B. Marker

POLICY_CLAIM_DB_SQLITE_ARCHITECTURE_OPTIONS_READY

## C. 기준 Commit

`3a621b2 docs(familyclaimref): document db sqlite ocr repository gates`

## D. Option Matrix

| Option | Description | Implementation impact | Data/migration risk | Privacy risk | Test impact | Pros | Cons | Recommendation |
|---|---|---|---|---|---|---|---|---|
| Keep JSON as current source of truth | 현재 검증된 JSON storage를 source of truth로 유지한다. | none now | low | low | current tests remain primary | 현재 baseline을 흔들지 않는다. | query/search 요구가 커지면 한계가 생길 수 있다. | Current recommendation |
| Add repository abstraction over current JSON storage | 기존 JSON storage 위에 repository boundary를 추가하는 선택지다. | future code boundary work | medium | low | contract tests required | storage 교체 여지를 만든다. | 지금은 추상화 비용이 실제 이득보다 클 수 있다. | Later planning only |
| Add SQLite as read/query projection only | JSON은 source of truth로 유지하고 SQLite는 조회용 index/projection으로 둔다. | future projection and sync work | medium | medium | projection consistency tests required | query/search 성능과 flexible filtering을 얻을 수 있다. | 동기화/재생성 정책이 필요하다. | Defer |
| Replace JSON with SQLite primary storage | SQLite를 primary storage로 전환한다. | high | high | medium | migration and full regression tests required | storage querying과 schema control이 명확해질 수 있다. | migration, backup, rollback, privacy surface가 커진다. | Not recommended now |
| Hybrid JSON source-of-truth plus SQLite index | JSON을 원본으로 두고 SQLite index를 보조로 유지한다. | high | high | medium | source/index drift tests required | 원본 보존과 검색 성능을 함께 노릴 수 있다. | 복잡도가 가장 높고 drift 위험이 있다. | Not recommended now |
| Defer SQLite until product UI shell or OCR boundary is approved | product UI, 검색, OCR boundary가 명확해질 때까지 SQLite 판단을 미룬다. | none now | low | low | current tests remain primary | 과잉 설계를 피한다. | 나중에 architecture decision을 다시 열어야 한다. | Recommended with current baseline |

## E. Current Recommendation

Current recommendation:

- Keep JSON as current source of truth.

Secondary planning recommendation:

- Repository boundary planning may be considered later, but no implementation now.

SQLite recommendation:

- Do not introduce SQLite until query/search requirements, migration strategy, backup/rollback, privacy policy, and test strategy are approved.

OCR relation:

- OCR privacy/storage planning must be separate from DB/SQLite architecture.
- OCR raw text storage and OCR candidate snapshot storage are not approved by this document.

`data/claimdoc` relation:

- `data/claimdoc` remains protected.
- `data/claimdoc` must not be used as migration/input data.
- No read, list, use, select, stage, commit, delete, or move action is allowed for `data/claimdoc`.

## F. Decision Questions

| Question | Required before implementation |
|---|---|
| What query/search requirements justify SQLite? | yes |
| Is JSON storage sufficient for current MVP validation? | yes |
| What data becomes source of truth after SQLite? | yes |
| Where is DB file located under runtime root? | yes |
| How is backup/rollback handled? | yes |
| How are migrations versioned? | yes |
| How is synthetic-only test data separated from real local artifacts? | yes |
| What repository interface owns policy, claim, document, attachment metadata? | yes |
| Are OCR raw text and OCR candidates in scope or out of scope? | yes |
| How will tests avoid `data/claimdoc`? | yes |

## G. Decision Gate

- architecture planning only
- implementation remains blocked
- explicit user approval required before any DB/SQLite/repository/migration/OCR implementation
- package reference addition remains blocked
- DB file creation remains blocked
- cleanup execution remains blocked

POLICY_CLAIM_DB_SQLITE_ARCHITECTURE_OPTIONS_READY
