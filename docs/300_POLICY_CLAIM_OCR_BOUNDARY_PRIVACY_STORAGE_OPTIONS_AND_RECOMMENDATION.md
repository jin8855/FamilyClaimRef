# OCR Boundary Privacy Storage Options And Recommendation

## A. Status

OCR_BOUNDARY_PRIVACY_STORAGE_OPTIONS_AND_RECOMMENDATION_ONLY

## B. Marker

POLICY_CLAIM_OCR_BOUNDARY_PRIVACY_STORAGE_OPTIONS_READY

## C. 기준 commit

`23d417b docs(familyclaimref): plan repository boundary decision`

## D. Option Matrix

| Option | Description | Implementation impact | Privacy risk | Storage risk | Test impact | Pros | Cons | Recommendation |
|---|---|---|---|---|---|---|---|---|
| Keep OCR out of current MVP | 현재 MVP에서는 OCR을 구현하지 않는다. | none now | low | low | current tests remain primary | 현재 검증된 core를 흔들지 않는다. | OCR 편의 기능은 나중으로 밀린다. | Current recommendation |
| OCR planning only, no storage | OCR boundary와 privacy/storage 정책만 문서로 검토한다. | docs only | low | low | planning review only | 정책 결정을 구현과 분리한다. | 실제 OCR 검증은 수행하지 않는다. | Recommended now |
| OCR extraction with no raw text persistence | future OCR 실행 시 raw text는 저장하지 않고 사용자 확인 흐름만 검토한다. | future OCR provider boundary | medium | low | synthetic OCR behavior tests required | privacy surface를 줄인다. | 재검토와 audit에 필요한 원문 evidence가 제한된다. | Future option only |
| OCR candidate snapshot under approved runtime root | 사용자 검토 후보값만 승인된 runtime root에 제한 저장한다. | future storage contract | medium | medium | retention/masking tests required | 후보값 검토 이력을 남길 수 있다. | masking, retention, deletion 정책이 필요하다. | Not now |
| Persist raw OCR text under approved runtime root | raw OCR text를 승인된 runtime root에 저장한다. | high future storage work | high | high | privacy, retention, deletion tests required | 재검토 evidence가 넓어진다. | 민감정보 저장 위험이 크다. | Not recommended |
| OCR with SQLite/search index | OCR 결과를 SQLite/search index와 연결한다. | high future DB/index work | high | high | DB/index consistency tests required | 검색 기능 확장 후보가 된다. | DB/SQLite/OCR risk가 동시에 커진다. | Not now |
| Defer OCR until product UI shell and privacy policy are approved | product UI shell과 privacy policy가 명시될 때까지 OCR 판단을 미룬다. | none now | low | low | current tests remain primary | UI/보관 정책 없이 OCR을 시작하지 않는다. | 향후 planning을 다시 열어야 한다. | Recommended with current baseline |

## E. Recommendation

Current recommendation:

- Keep OCR out of current MVP implementation.

Secondary recommendation:

- OCR boundary/privacy/storage planning can continue, but no OCR package, provider, API, or storage should be added now.

Raw text recommendation:

- Do not persist raw OCR text by default.

Candidate recommendation:

- Do not persist OCR candidate snapshots until retention, masking, and approval workflow are decided.

SQLite relation:

- OCR must not be used as a reason to introduce SQLite until query/search requirements and privacy policy are explicit.

`data/claimdoc` relation:

- `data/claimdoc` remains protected and must not be used as OCR input.

## F. Privacy/Storage Decision Questions

- Is OCR in MVP scope?
- What documents can be OCR inputs?
- Are OCR inputs synthetic-only during validation?
- Is raw OCR text stored, discarded, or masked?
- Are OCR candidate values stored separately from raw OCR text?
- What confidence score or provenance metadata is retained?
- How long are OCR artifacts retained?
- Where are OCR artifacts stored under runtime root?
- How are OCR artifacts deleted or reviewed?
- What masking/redaction rules are required before storage?
- How are tests designed without `data/claimdoc`?
- Does OCR require DB/SQLite or can it remain memory-only?
- What UI shows OCR candidates, and is product UI shell approved?

## G. Artifact Policy Table

| Artifact | Store now | Future condition | Privacy risk | Notes |
|---|---|---|---|---|
| uploaded source file | existing behavior only | current attachment workflow remains non-OCR | medium | OCR input expansion is not approved. |
| copied attachment | existing behavior only | current document attachment workflow remains source of truth | medium | This is not OCR artifact storage. |
| raw OCR text | no | explicit privacy, masking, retention, and storage approval | high | Do not persist by default. |
| OCR candidate fields | no | exact-scope approval for candidate value storage and user confirmation workflow | medium | Separate from raw OCR text. |
| OCR confidence score | no | provenance policy and UI ownership approval | medium | Do not add now. |
| OCR provenance metadata | no | provider/runtime boundary approval | medium | Provider is not selected. |
| OCR error log | no | error retention and redaction policy approval | medium | Avoid accidental sensitive text capture. |
| masked OCR text | no | masking/redaction rules and tests approved | medium | Masking policy is not defined. |
| synthetic test OCR fixture | no | future test implementation approval | low | Must not use `data/claimdoc`. |

## H. Planning Judgment

OCR is a future planning track only. No OCR implementation, package, provider, raw text storage, candidate snapshot storage, DB/SQLite dependency, or product UI dependency is approved by this document.

