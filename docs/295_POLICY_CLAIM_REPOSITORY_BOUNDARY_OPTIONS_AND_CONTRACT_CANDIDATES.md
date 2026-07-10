# Repository Boundary Options And Contract Candidates

## A. Status

REPOSITORY_BOUNDARY_OPTIONS_AND_CONTRACT_CANDIDATES_ONLY

## B. Marker

POLICY_CLAIM_REPOSITORY_BOUNDARY_OPTIONS_READY

## C. 기준 commit

`9c5fca4 docs(familyclaimref): plan db sqlite architecture decision`

## D. Option Matrix

| Option | Description | Implementation impact | Migration risk | Test impact | Pros | Cons | Recommendation |
|---|---|---|---|---|---|---|---|
| Keep existing storage services as current boundary | `IDocumentStorageService`와 `IPolicyClaimStorageService`를 현재 persistence boundary로 유지한다. | none now | low | current storage tests remain primary | 검증된 JSON baseline을 흔들지 않는다. | future query/search 요구가 커지면 재검토가 필요하다. | Current recommendation |
| Add repository facade over existing JSON services | 기존 JSON storage service 위에 facade repository를 둔다. | future interface/class addition | medium | repository contract tests required | storage 교체 후보를 숨길 수 있다. | 현재는 추상화 비용이 이득보다 클 수 있다. | Not now |
| Split repositories by aggregate, e.g. `PolicyRepository`, `ClaimRepository`, `DocumentRepository` | aggregate별 repository ownership을 분리한다. | high future refactor | medium to high | aggregate contract tests required | ownership이 명확해질 수 있다. | workflow와 repository 책임이 섞일 위험이 있다. | Needs future decision |
| Add query-only repository for future search/index use | command storage는 유지하고 read/search projection만 별도 repository로 둔다. | future read model addition | medium | query projection tests required | product search 요구에 대응하기 쉽다. | 현재 query/search requirement가 승인되지 않았다. | Defer |
| Introduce repository only after SQLite decision | SQLite source/index 방향이 확정된 뒤 repository contract를 정한다. | none now | lower than premature abstraction | future DB/storage equivalence tests required | 저장 방향과 contract를 함께 맞출 수 있다. | repository planning을 나중에 다시 열어야 한다. | Recommended secondary path |
| Defer repository until product UI shell/query requirements are approved | product UI shell, search, filtering, reporting 요구가 승인될 때까지 repository 판단을 미룬다. | none now | low | current tests remain primary | 과잉 설계를 피한다. | future planning debt가 남는다. | Recommended with current baseline |

Current recommendation:

- Keep existing storage services as current boundary.

Secondary recommendation:

- Do not add repository interface/class now.

Future repository planning may be reopened only after storage direction, query/search requirements, and migration strategy are explicit.

## E. Current Boundary Table

| Current boundary | Current role | Keep / Candidate for repository? | Notes |
|---|---|---|---|
| `IDocumentStorageService` | document metadata와 policy/claim document link 저장/조회/사용 중지 | Keep | 현재 document persistence interface다. |
| `IPolicyClaimStorageService` | policy/claim 저장/조회/사용 중지 및 존재 확인 | Keep | 현재 policy/claim persistence interface다. |
| `DocumentRegistrationWorkflow` | attachment import, document metadata, policy/claim link를 registration use case로 조합 | Keep as workflow | repository가 workflow behavior를 가져가면 안 된다. |
| `JsonDocumentStorageService` | `IDocumentStorageService`의 JSON 구현 | Keep as current implementation | repository가 아니라 concrete storage다. |
| `JsonPolicyClaimStorageService` | `IPolicyClaimStorageService`의 JSON 구현 | Keep as current implementation | repository가 아니라 concrete storage다. |
| `JsonFileStore` | JSON envelope load/save helper | Keep as low-level helper | repository boundary 후보가 아니다. |

## F. Candidate Repository Contract Table

| Candidate contract | Possible responsibility | Depends on | Approved now |
|---|---|---|---|
| `IPolicyRepository` | policy aggregate persistence/query | storage direction, aggregate ownership, migration plan | no |
| `IClaimRepository` | claim aggregate persistence/query | storage direction, aggregate ownership, migration plan | no |
| `IDocumentRepository` | document metadata persistence/query | document metadata ownership, link policy, migration plan | no |
| `IAttachmentMetadataRepository` | attachment metadata read model or persistence facade | document/attachment ownership decision | no |
| `IRegistrationSummaryReader` | registration summary read-only projection | product UI/query requirement | no |
| `IReadOnlyClaimSearchRepository` | claim search and filter projection | search/index requirement, possible SQLite projection decision | no |

## G. Decision Questions

- 어떤 use case가 repository abstraction을 요구하는가?
- 현재 storage service boundary로 테스트/검증이 충분한가?
- repository가 JSON storage를 감싸는가, SQLite를 감싸는가, 둘 다 추상화하는가?
- aggregate ownership은 policy/claim/document 중 어디에 있는가?
- repository가 workflow behavior를 가져가면 안 되는 이유는 무엇인가?
- query/search 요구가 없는 상태에서 repository를 추가하는 비용은 무엇인가?
- future migration test는 어느 contract를 기준으로 작성할 것인가?
- `data/claimdoc`를 사용하지 않고 synthetic-only fixture로 검증 가능한가?

## H. Boundary Recommendation

현재 단계에서는 repository abstraction을 추가하지 않는다. JSON storage service boundary를 유지하고, repository는 future option으로만 남긴다.

Repository boundary implementation은 explicit user approval, storage direction decision, query/search requirement, migration strategy, and validation plan이 모두 갖춰진 뒤에만 다시 검토한다.

