# Policy Claim Persistence Extension Scope Discovery and User Decision

## 1. 목적과 비승인 범위

### Status

`FAMILYCLAIMREF_PERSISTENCE_EXTENSION_SCOPE_DISCOVERED_USER_DECISION_REQUIRED`

이 문서는 화면 12, 13, 16, 19, 20에 표시된 명령을 현재 source, storage, schema, test에 연결하여 후속 persistence 구현 전에 필요한 사용자 결정을 분리한다.

- 현재 배치 위험도: `T2_MODERATE`
- 후속 persistence 구현 위험도: `T3_HIGH`
- 조사 방식: source 및 문서 read-only inspection
- Product source/test/schema/migration 변경: `0`
- DB/API/runtime/build/test 실행: `0`
- P03/R03/R07/R08 실행 또는 재도입: `0`
- stage/commit/push: `0`
- persistence 구현 승인: `NOT_AUTHORIZED`
- schema 변경 승인: `NOT_AUTHORIZED`
- production/deployment 승인: `NOT_AUTHORIZED`

이 문서는 기존 Product UI Shell, 승인된 21개 화면 구조, 화면 17/18의 문서 등록 기능, Gate 8 PASS를 다시 열거나 재설계하지 않는다. 비활성 명령은 사용자 결정과 별도 구현 지시가 있기 전까지 계속 비활성 상태를 유지한다.

## 2. 권위 기준과 조사 source

### 2.1 시작 기준

| 항목 | 확인값 |
|---|---|
| Repository | `C:\EtcProject\FamilyClaimRef` |
| Branch | `main` |
| HEAD | `aecf7edfd43b4124ec5ff17d35687020cf4c0d90` |
| HEAD subject | `docs(familyclaimref): record gate8 registration persistence decision package` |
| 시작 tracked/staged/untracked | `46/0/42` |
| 시작 status entries | `88` |
| `docs/439` | 존재, 변경하지 않음 |
| `docs/439` SHA-256 | `34B6BD6A26D3552DF0B3297C7D0FC8560EA4FE02969478D47E09F71C95F14306` |
| 다음 미사용 번호 | `440` |

Gate 8 권위 상태는 [docs/439](./439_POLICY_CLAIM_PRODUCT_UI_SHELL_GATE8_AUTHORITATIVE_STATUS_RECONCILIATION_AND_CLOSURE.md)를 따른다.

- `WIREFRAME_STRUCTURE_STATE = USER_ACCEPTED_21_OF_21`
- `PRODUCT_UI_STATE = USER_VISUAL_ACCEPTED`
- `POST_UI_RUNTIME_EVIDENCE_STATE = PASS_EXISTING_T2_EVIDENCE`
- `P03_REFERENCE_STATE = RETIRED_UNDEFINED_REFERENCE_NOT_APPLICABLE`
- `R07_REFERENCE_STATE = RETIRED_UNDEFINED_REFERENCE_NOT_APPLICABLE`
- `R08_REFERENCE_STATE = RETIRED_UNDEFINED_REFERENCE_NOT_APPLICABLE`
- `GATE8_STATE = PASS`
- `PERSISTENCE_EXTENSION_STATE = DEFERRED`

### 2.2 Source evidence index

| Evidence ID | 경로와 확인 범위 |
|---|---|
| `SRC-UI-01` | [ProductScreenRoutes.cs](../app/FamilyClaimRef.App/ViewModels/ProductScreenRoutes.cs): 화면 번호와 route |
| `SRC-UI-02` | [ProductScreenCatalog.cs](../app/FamilyClaimRef.App/ViewModels/ProductScreenCatalog.cs): `CommandSpecs`, `RouteActions`, command enablement |
| `SRC-UI-03` | [ProductWireframeScreenView.xaml](../app/FamilyClaimRef.App/Views/ProductWireframeScreenView.xaml): 공용 View와 `NavigateCommand` binding |
| `SRC-UI-04` | [ProductScreenContent.xaml](../app/FamilyClaimRef.App/Resources/ProductScreenContent.xaml): 실제 제목, 필드, 버튼 문구, 화면 전용 비영속화 안내 |
| `SRC-NAV-01` | [ProductRouteCommand.cs](../app/FamilyClaimRef.App/ViewModels/ProductRouteCommand.cs), [ProductShellViewModel.cs](../app/FamilyClaimRef.App/ViewModels/ProductShellViewModel.cs): enabled route 이동 |
| `SRC-POL-01` | [IPolicyClaimStorageService.cs](../app/FamilyClaimRef.App/Services/Storage/IPolicyClaimStorageService.cs): Policy/Claim storage abstraction |
| `SRC-POL-02` | [JsonPolicyClaimStorageService.cs](../app/FamilyClaimRef.App/Services/Storage/JsonPolicyClaimStorageService.cs): JSON provider, active query, add/disable |
| `SRC-POL-03` | [PolicyRecord.cs](../app/FamilyClaimRef.App/Models/Storage/PolicyRecord.cs), [ClaimRecord.cs](../app/FamilyClaimRef.App/Models/Storage/ClaimRecord.cs), [PolicyDraft.cs](../app/FamilyClaimRef.App/Models/Storage/PolicyDraft.cs), [ClaimDraft.cs](../app/FamilyClaimRef.App/Models/Storage/ClaimDraft.cs): 현재 persistence record/draft |
| `SRC-POL-04` | [PolicyClaimManagementViewModel.cs](../app/FamilyClaimRef.App/ViewModels/PolicyClaimManagementViewModel.cs): load/create/disable, duplicate, busy/error mapping |
| `SRC-JSON-01` | [JsonFileStore.cs](../app/FamilyClaimRef.App/Services/Storage/JsonFileStore.cs), [JsonFileEnvelope.cs](../app/FamilyClaimRef.App/Models/Storage/JsonFileEnvelope.cs): schema version 1, temp-save/final-move |
| `SRC-COMP-01` | [AppServices.cs](../app/FamilyClaimRef.App/Composition/AppServices.cs): shared storage provider와 ViewModel composition |
| `SRC-DOC-01` | [DocumentRegistrationViewModel.cs](../app/FamilyClaimRef.App/ViewModels/DocumentRegistrationViewModel.cs), [DocumentRegistrationWorkflow.cs](../app/FamilyClaimRef.App/Services/Storage/DocumentRegistrationWorkflow.cs), [DocumentAttachmentCoordinator.cs](../app/FamilyClaimRef.App/Services/Storage/DocumentAttachmentCoordinator.cs), [DocumentLinkCoordinator.cs](../app/FamilyClaimRef.App/Services/Storage/DocumentLinkCoordinator.cs): 화면 17/18 문서 등록 비교 기준 |
| `TEST-POL-01` | [PolicyClaimManagementViewModelTests.cs](../tests/FamilyClaimRef.App.Tests/PolicyClaimManagementViewModelTests.cs), [JsonPolicyClaimStorageServiceTests.cs](../tests/FamilyClaimRef.App.Tests/JsonPolicyClaimStorageServiceTests.cs), [PolicyClaimLifecyclePersistenceTests.cs](../tests/FamilyClaimRef.App.Tests/Integration/PolicyClaimLifecyclePersistenceTests.cs): 현재 Policy/Claim 계약 검증 |
| `TEST-DOC-01` | [DocumentRegistrationViewModelTests.cs](../tests/FamilyClaimRef.App.Tests/DocumentRegistrationViewModelTests.cs), [DocumentRegistrationWorkflowTests.cs](../tests/FamilyClaimRef.App.Tests/DocumentRegistrationWorkflowTests.cs), [DocumentRegistrationPersistenceGate8Tests.cs](../tests/FamilyClaimRef.App.Tests/Integration/DocumentRegistrationPersistenceGate8Tests.cs): 문서 등록 계약 검증 |

### 2.3 실제 계층 판정

| 계층 | 현재 사실 |
|---|---|
| Product View/ViewModel | 다섯 화면 모두 `ProductWireframeScreenView`와 `ProductScreenViewModel`을 사용한다. 화면별 write ViewModel은 없다. |
| Command handler | enabled command는 route navigation이다. disabled command는 `RouteId=null`, `IsEnabled=false`이며 write handler가 없다. |
| Application use case | Policy/Claim에는 `PolicyClaimManagementViewModel`이 있으나 화면 12와 연결되지 않는다. Family/Category use case는 없다. |
| Domain | 별도 Domain aggregate 계층은 없다. `Models/Storage`의 persistence record만 확인된다. |
| Repository | 별도 repository abstraction/implementation은 없다. |
| Storage abstraction/provider | Policy/Claim은 `IPolicyClaimStorageService`와 `JsonPolicyClaimStorageService`를 사용한다. |
| Schema/migration | JSON envelope `schemaVersion=1`만 존재한다. DB, SQLite, SQL schema, migration, seed, snapshot은 없다. |
| Transaction | 단일 JSON 파일 save는 temp file 후 final move다. 여러 파일을 묶는 transaction은 없다. |
| Concurrency | 관리 ViewModel instance별 `SemaphoreSlim`만 있다. provider, 다중 ViewModel, cross-process 동시성 보장은 없다. |

## 3. 화면 및 command inventory

### 3.1 화면 요약

| 화면 | Route | 실제 화면명 | View/ViewModel | 표시 명령 | Enabled | Disabled | Unbound |
|---:|---|---|---|---:|---:|---:|---:|
| 12 | `12_policy_register` | 보험 등록/편집 | `ProductWireframeScreenView` / `ProductScreenViewModel` | 7 | 3 | 4 | 0 |
| 13 | `13_family_register` | 가족 등록/편집 | 동일 공용 View/ViewModel | 5 | 2 | 3 | 0 |
| 16 | `16_category_manage` | 분류/태그 관리 | 동일 공용 View/ViewModel | 7 | 5 | 2 | 0 |
| 19 | `19_category_register` | 분류 등록/편집 | 동일 공용 View/ViewModel | 5 | 2 | 3 | 0 |
| 20 | `20_category_item_register` | 항목 등록/편집 | 동일 공용 View/ViewModel | 5 | 2 | 3 | 0 |
| **합계** |  |  |  | **29** | **14** | **15** | **0** |

`UNBOUND=0`은 XAML command property에 binding 자체가 없다는 의미다. 그러나 disabled 15개는 공용 `NavigateCommand`에 연결되어 있어도 `RouteId=null`이고 write handler가 없으므로 persistence 관점에서는 모두 `write-unbound`다.

### 3.2 Write 필요성 요약

- `REQUIRED`: 13개 (`CMD-PER-001`, `002`, `003`, `004`, `008`, `009`, `010`, `020`, `021`, `022`, `025`, `026`, `027`)
- `NOT_REQUIRED`: 14개 route navigation (`CMD-PER-005`~`007`, `011`~`014`, `017`~`019`, `023`, `024`, `028`, `029`)
- `UNKNOWN`: 2개 (`CMD-PER-015`, `016`). 화면 16에서 선택된 대상이 Category인지 CategoryItem인지 나타내는 identity와 command parameter가 없다.

## 4. Command-to-domain-to-storage 추적표

| ID | 화면/문구/목적 | 상태 | View/VM 및 현재 handler | Write | 대상 aggregate | Storage/schema | 재사용 | Validation 및 실패/취소 | 결정 | 위험/근거 |
|---|---|---|---|---|---|---|---|---|---|
| `CMD-PER-001` | 12 보험 등록/편집 / 저장 / 입력 저장 | DISABLED | `SRC-UI-02/03`; handler `NONE` | REQUIRED | 기존 `PolicyRecord` 부분 재사용, 전체 화면 owner `TBD` | `IPolicyClaimStorageService.AddPolicyAsync`; PARTIAL | PARTIAL | 현재 title/date 규칙만 확인됨; disabled라 현재 write/실패 없음 | `DEC-PER-001`, `002`, `008`, `009`, `010` | 전체 7필드 mapping과 update 없음; high; `SRC-UI-04`, `SRC-POL-01/04` |
| `CMD-PER-002` | 12 / 보류 / 편집 상태 보류 | DISABLED | 동일; handler `NONE` | REQUIRED | Policy lifecycle `TBD` | NONE/NONE | NO | 보류 상태·재진입 규칙 `TBD`; 현재 no-op | `DEC-PER-002` | 신규 상태 전이; high; `SRC-UI-02`, `SRC-POL-03` |
| `CMD-PER-003` | 12 / 삭제 / 보험 계약 제거 | DISABLED | 동일; handler `NONE` | REQUIRED | Policy | NONE/NONE | NO | active claim/document 참조와 soft/hard delete `TBD`; 현재 no-op | `DEC-PER-002` | 참조 무결성·복구; high; `SRC-POL-01/03` |
| `CMD-PER-004` | 12 / 사용 중지 / 활성 계약 비활성화 | DISABLED | 동일; handler `NONE` | REQUIRED | 기존 `PolicyRecord` | `DisablePolicyAsync`; FULL for current record | PARTIAL | active policy 선택과 active claim 차단은 기존 VM 규칙; 취소는 rethrow하며 save 후 refresh 취소 결과는 모호함 | `DEC-PER-002`, `009` | 화면 identity 미연결, 다중 VM 경쟁; medium-high; `SRC-POL-01/02/04`, `TEST-POL-01` |
| `CMD-PER-005` | 12 / 닫기 / 보험 관리 이동 | ENABLED | `SRC-UI-03`; `ProductRouteCommand.Execute` → `NavigateTo` | NOT_REQUIRED | N/A | NONE, write N/A | YES | route 유효성; 실패/취소 N/A | NONE | navigation only; low; `SRC-UI-01/02`, `SRC-NAV-01` |
| `CMD-PER-006` | 12 / 보험 문서 등록 / 화면 17 이동 | ENABLED | 동일 navigation handler | NOT_REQUIRED | N/A | NONE, write N/A | YES | 기존 선택 context/route 계약; write는 화면 17 소유 | NONE | navigation only; low; `SRC-UI-02`, `SRC-DOC-01` |
| `CMD-PER-007` | 12 / 보험 관리 / 화면 11 이동 | ENABLED | 동일 navigation handler | NOT_REQUIRED | N/A | NONE, write N/A | YES | route 유효성; 실패/취소 N/A | NONE | navigation only; low; `SRC-UI-02`, `SRC-NAV-01` |
| `CMD-PER-008` | 13 가족 등록/편집 / 저장 | DISABLED | `SRC-UI-02/03`; handler `NONE` | REQUIRED | `FamilyMember` candidate, 현재 없음 | NONE/NONE | NO | pseudonymous display name, relation, active, memo 규칙 결정 필요 | `DEC-PER-003`, `004`, `008`, `009`, `010` | 개인정보·identity·중복; high; `SRC-UI-04` |
| `CMD-PER-009` | 13 / 삭제 / 가족 후보 제거 | DISABLED | 동일; handler `NONE` | REQUIRED | `FamilyMember` candidate | NONE/NONE | NO | 참조 여부·soft/hard delete·복구 `TBD`; 현재 no-op | `DEC-PER-004` | 개인정보와 향후 참조; high; `SRC-UI-04` |
| `CMD-PER-010` | 13 / 사용 중지 / 가족 후보 비활성화 | DISABLED | 동일; handler `NONE` | REQUIRED | `FamilyMember` candidate | NONE/NONE | NO | active 상태 전이와 재선택 규칙 필요 | `DEC-PER-004`, `009` | stale selection; medium-high; `SRC-UI-04` |
| `CMD-PER-011` | 13 / 닫기 / 가족 목록 이동 | ENABLED | navigation handler | NOT_REQUIRED | N/A | NONE, write N/A | YES | route 유효성; 실패/취소 N/A | NONE | low; `SRC-UI-02`, `SRC-NAV-01` |
| `CMD-PER-012` | 13 / 가족 관리 / 화면 02 이동 | ENABLED | navigation handler | NOT_REQUIRED | N/A | NONE, write N/A | YES | route 유효성; 실패/취소 N/A | NONE | low; `SRC-UI-02`, `SRC-NAV-01` |
| `CMD-PER-013` | 16 분류/태그 관리 / 등록 / 화면 19 이동 | ENABLED | navigation handler | NOT_REQUIRED | N/A | NONE, write N/A | YES | route 유효성; 실제 save는 화면 19 소유 | NONE | low; `SRC-UI-02`, `SRC-NAV-01` |
| `CMD-PER-014` | 16 / 등록 / 화면 20 이동 | ENABLED | navigation handler | NOT_REQUIRED | N/A | NONE, write N/A | YES | route 유효성; 실제 save는 화면 20 소유 | NONE | low; `SRC-UI-02`, `SRC-NAV-01` |
| `CMD-PER-015` | 16 / 수정 / 선택 대상 편집 | DISABLED | `SRC-UI-02/03`; handler·parameter `NONE` | UNKNOWN | Category 또는 CategoryItem `TBD` | NONE/UNKNOWN | NO | 선택 row type/id가 선행되어야 함; 현재 no-op | `DEC-PER-007` | 잘못된 aggregate 수정; high; `SRC-UI-04` |
| `CMD-PER-016` | 16 / 삭제 / 선택 대상 제거 | DISABLED | 동일; handler·parameter `NONE` | UNKNOWN | Category 또는 CategoryItem `TBD` | NONE/UNKNOWN | NO | 선택 row type/id와 참조 정책 필요; 현재 no-op | `DEC-PER-006`, `007` | 잘못된 대상·참조 손실; high; `SRC-UI-04` |
| `CMD-PER-017` | 16 / 분류 등록/편집 / 화면 19 이동 | ENABLED | navigation handler | NOT_REQUIRED | N/A | NONE, write N/A | YES | route 유효성 | NONE | low; `SRC-UI-02` |
| `CMD-PER-018` | 16 / 항목 등록/편집 / 화면 20 이동 | ENABLED | navigation handler | NOT_REQUIRED | N/A | NONE, write N/A | YES | route 유효성 | NONE | low; `SRC-UI-02` |
| `CMD-PER-019` | 16 / 관리하기 / 화면 15 이동 | ENABLED | navigation handler | NOT_REQUIRED | N/A | NONE, write N/A | YES | route 유효성 | NONE | low; `SRC-UI-02` |
| `CMD-PER-020` | 19 분류 등록/편집 / 저장 | DISABLED | `SRC-UI-02/03`; handler `NONE` | REQUIRED | Category candidate | NONE/NONE | NO | name/code/active/sort/description/system-default 규칙 필요 | `DEC-PER-005`, `006`, `008`, `009`, `010` | code uniqueness·system default; high; `SRC-UI-04` |
| `CMD-PER-021` | 19 / 삭제 / 분류 제거 | DISABLED | 동일; handler `NONE` | REQUIRED | Category candidate | NONE/NONE | NO | active child/reference·soft/hard delete·복구 `TBD` | `DEC-PER-006` | child/reference integrity; high; `SRC-UI-04` |
| `CMD-PER-022` | 19 / 사용 중지 / 분류 비활성화 | DISABLED | 동일; handler `NONE` | REQUIRED | Category candidate | NONE/NONE | NO | active child 처리와 selection repair 필요 | `DEC-PER-006`, `009` | cascade ambiguity; high; `SRC-UI-04` |
| `CMD-PER-023` | 19 / 닫기 / 화면 16 이동 | ENABLED | navigation handler | NOT_REQUIRED | N/A | NONE, write N/A | YES | route 유효성 | NONE | low; `SRC-UI-02` |
| `CMD-PER-024` | 19 / 분류/태그 관리 / 화면 16 이동 | ENABLED | navigation handler | NOT_REQUIRED | N/A | NONE, write N/A | YES | route 유효성 | NONE | low; `SRC-UI-02` |
| `CMD-PER-025` | 20 항목 등록/편집 / 저장 | DISABLED | `SRC-UI-02/03`; handler `NONE` | REQUIRED | CategoryItem candidate | NONE/NONE | NO | parent/name/code/active/sort/scope validation 필요 | `DEC-PER-005`, `006`, `008`, `009`, `010` | parent lifetime·per-parent uniqueness; high; `SRC-UI-04` |
| `CMD-PER-026` | 20 / 삭제 / 항목 제거 | DISABLED | 동일; handler `NONE` | REQUIRED | CategoryItem candidate | NONE/NONE | NO | reference·soft/hard delete·복구 `TBD` | `DEC-PER-006` | future search reference; high; `SRC-UI-04` |
| `CMD-PER-027` | 20 / 사용 중지 / 항목 비활성화 | DISABLED | 동일; handler `NONE` | REQUIRED | CategoryItem candidate | NONE/NONE | NO | active state와 search exclusion 규칙 필요 | `DEC-PER-006`, `009` | stale consumer state; medium-high; `SRC-UI-04` |
| `CMD-PER-028` | 20 / 닫기 / 화면 16 이동 | ENABLED | navigation handler | NOT_REQUIRED | N/A | NONE, write N/A | YES | route 유효성 | NONE | low; `SRC-UI-02` |
| `CMD-PER-029` | 20 / 분류/태그 관리 / 화면 16 이동 | ENABLED | navigation handler | NOT_REQUIRED | N/A | NONE, write N/A | YES | route 유효성 | NONE | low; `SRC-UI-02` |

명령 누락 판정: `0`. `CommandSpecs`와 `RouteActions`에서 다섯 화면에 표시되는 29개 항목을 모두 포함했다.

## 5. 데이터 소유권과 생명주기

### 5.1 현재 사실과 candidate 경계

| 데이터 | 현재 root/child 사실 | Candidate ownership | 생성·조회·수정 | 상태 전이 및 참조 | 감사·민감도 |
|---|---|---|---|---|---|
| Policy | `PolicyRecord`가 `policies.json`에 독립 record로 존재한다. Domain aggregate라는 명시는 없다. | 기존 record를 유지하되 화면 12 전체 field owner는 `TBD_USER_DECISION` | Add, active query, disable만 존재. update/hold/delete/restore 없음. | `ClaimRecord.PolicyId`로 연결. 기존 VM은 active claim이 있으면 disable을 차단하지만 storage 자체는 강제하지 않는다. | CreatedAt/UpdatedAt/DisabledAt만 존재. 보험 정보 포함. |
| Claim | `claims.json`의 별도 record이며 PolicyId를 가진다. | 이번 구현 대상 아님. Policy reference 검증 기준으로만 사용. | 기존 add/active query/disable 유지. | Policy의 child aggregate라고 단정하지 않는다. | 보험 청구 정보 포함. |
| FamilyMember | source/storage model 없음. | root candidate. 최소 pseudonymous display name, controlled relation, active, memo 후보. | 모두 `TBD_USER_DECISION`. | 향후 Policy 연결 key 여부와 deactivate 시 영향 미정. | 개인정보 가능성이 있으므로 실명·고유식별정보를 기본 계약에 넣지 않는다. |
| Category | source/storage model 없음. | root aggregate candidate. | create/update/deactivate 후보. | active items가 있을 때 parent deactivate 정책 필요. | 일반 분류 정보. timestamps 후보. |
| CategoryItem | source/storage model 없음. | Category child candidate이나 아직 승인되지 않음. | parent 범위 query/update/deactivate 후보. | Category reference와 검색 consumer는 아직 없다. | 일반 분류 정보. timestamps 후보. |

### 5.2 생명주기 보호 원칙

- `삭제`를 hard delete로 해석하지 않는다.
- `사용 중지`는 `DisabledAt` 또는 candidate active state 전이이며 삭제와 같지 않다.
- restore 계약은 현재 Policy/Claim active-only query로는 지원되지 않는다.
- 여러 JSON 파일 save를 하나의 transaction으로 간주하지 않는다.
- 저장 실패의 자동 재시도는 현재 계약에 없다.
- FamilyMember, Category, CategoryItem identity는 display name이 아니라 generated internal ID가 소유하는 candidate가 안전하다.
- 사용자 UI, durable log, error message에 raw ID, 절대 경로, stack trace, SHA를 노출하지 않는다.
- target command가 파일을 다루지 않으므로 aggregate 삭제와 physical file 삭제를 연결하지 않는다.

## 6. 기존 기능 재사용 가능성

### 6.1 Policy/Claim 관리 계약

재사용 가능한 부분:

- `PolicyDraft(DisplayTitle, ReferenceDate)` 기반 create
- active policy query
- `DisabledAt` 기반 disable
- trim 및 required title/date normalization
- Product management ViewModel의 active title case-insensitive duplicate 차단
- per-ViewModel busy/reentry gate와 제품용 safe error message
- isolated JSON persistence unit/integration test pattern

재사용할 수 없는 부분:

- 화면 12의 가족, 보험사, 계약 상태, 가입일, 보험기간, 등록 출처 persistence
- 기존 Policy update, hold, delete, restore
- 여러 ViewModel/process 사이의 concurrency guarantee
- Family/Category/CategoryItem storage

화면 12의 `가입일` 또는 `보험기간` 중 무엇을 현재 `ReferenceDate`로 취급할지는 source에 정의되어 있지 않다. 이를 임의 mapping하지 않는다.

### 6.2 화면 17/18 문서 등록 계약

재사용 가능한 패턴:

- 선택 target validation과 stale-target 차단
- busy/reentry 방지
- source selection SHA와 staged SHA 비교
- 동일 process duplicate critical section
- staging → final 이동과 link 실패 보상
- safe product error mapping과 민감 정보 비노출
- unit/integration test 분리

직접 재사용 불가 판정:

- 화면 12/13/16/19/20의 write 후보는 파일을 입력하지 않는다.
- 문서 등록의 transaction boundary는 attachment, Document metadata, target link다.
- 이번 후보의 transaction boundary는 Policy, FamilyMember, Category aggregate persistence다.
- 따라서 attachment coordinator나 file validation을 복사하는 것은 범위 확장이다. 보상과 동시성 원칙만 설계 참고로 사용할 수 있다.

## 7. Persistence 계약 후보

아래 계약은 모두 `CANDIDATE`다. 사용자 결정 전 승인된 구현 계약이 아니다.

### 7.1 Core contract 후보

| Command | Operation candidate | 입력/출력 | 필수값·정규화 | 중복·idempotency | Concurrency·transaction |
|---|---|---|---|---|---|
| `001` | `CREATE` 또는 `UPDATE` | 화면 12 field set → Policy result | field mapping 결정 전 TBD | active title 규칙은 기존 create에만 정의 | process-scoped aggregate gate + version/UpdatedAt 비교 후보 |
| `002` | `DEFER/HOLD_STATE` | Policy ID + hold reason 후보 → state result | 상태 enum/사유 TBD | 동일 상태 요청 idempotent 후보 | 단일 Policy aggregate save 후보 |
| `003` | `SOFT_DELETE` 후보 | Policy ID → delete result | active reference 검증 | repeated delete idempotent 후보 | Policy/Claim/document reference boundary 결정 필요 |
| `004` | `DEACTIVATE` | active Policy ID → disabled Policy | normalized ID, active claims 없음 | 이미 disabled면 structured no-success | Policy query부터 save까지 직렬화 필요 |
| `008` | `CREATE` 또는 `UPDATE` | display name/relation/active/memo → FamilyMember | trim, controlled relation; 실명/identifier 금지 | display name duplicate 허용 후보, ID identity | 단일 FamilyMember store save 후보 |
| `009` | `SOFT_DELETE` 후보 | FamilyMember ID → result | reference 검증 | repeated delete idempotent 후보 | reference consumer 결정 필요 |
| `010` | `DEACTIVATE` | active FamilyMember ID → result | valid active ID | already inactive no-success | query-to-save gate 후보 |
| `015` | `UPDATE_NAVIGATION` 또는 update | selected type/id → edit target/result | explicit row type/id 필수 | N/A until target known | 잘못된 aggregate 방지를 위해 현재 disabled 유지 |
| `016` | `SOFT_DELETE` 또는 `DEACTIVATE` | selected type/id → result | explicit row type/id, reference 검증 | target별 idempotency TBD | Category/Item boundary 결정 필요 |
| `020` | `CREATE` 또는 `UPDATE` | name/code/active/sort/description/system default → Category | trim, code normalization, sort range | normalized code global unique 후보 | Category aggregate atomic save 후보 |
| `021` | `SOFT_DELETE` 후보 | Category ID → result | active child/reference 검증 | repeated delete idempotent 후보 | child 처리 정책과 같은 boundary 필요 |
| `022` | `DEACTIVATE` | Category ID → result | active child policy | already inactive no-success | Category aggregate atomic save 후보 |
| `025` | `CREATE` 또는 `UPDATE` | parent/name/code/active/sort/description/search scopes → CategoryItem | active parent, code normalization, boolean scope | code unique per parent 후보 | Category root + item atomic save 후보 |
| `026` | `SOFT_DELETE` 후보 | Category ID + Item ID → result | search/reference 검증 | repeated delete idempotent 후보 | Category aggregate atomic save 후보 |
| `027` | `DEACTIVATE` | Category ID + Item ID → result | active parent/item | already inactive no-success | Category aggregate atomic save 후보 |

### 7.2 Failure, message, test 후보

| Command group | 저장 순서/부분 실패 | 취소·재시도 | Product message candidate | 로그 허용/금지 | 필요한 tests |
|---|---|---|---|---|---|
| `001`~`004` | load/validate/conflict check/save/refresh. 성공 save 후 refresh 실패 또는 취소의 결과를 별도 명시해야 한다. | mutation 전 cancellation은 no-success. mutation 후 refresh cancellation은 persisted result를 구분해야 한다. 자동 retry 없음. | 성공: 보험 계약 처리 완료. Validation: 필수 정보를 확인해 주세요. Conflict: 다른 변경이 반영되었습니다. 다시 불러와 주세요. Failure: 보험 계약을 처리하지 못했습니다. 다시 시도해 주세요. | command type와 safe error code만 허용. raw ID/path/exception 금지. | create/update/deactivate lifecycle, duplicate, conflict, repeated load, post-mutation refresh failure/cancellation integration |
| `008`~`010` | FamilyMember 한 aggregate save. 향후 Policy link와 같은 transaction이라고 가정하지 않음. | cancellation no-success. 사용자 명시 retry만. | 성공/validation/conflict/failure를 가족 후보 업무용 한국어로 분리. | 실명, 메모 원문, raw ID 금지. | privacy, duplicate-display, deactivate/reference, concurrency, persistence reload |
| `015`~`016` | target type/id 결정 전 persistence operation 없음. | N/A until decision. | 선택 대상을 확인해 주세요. | selected raw ID 노출 금지. | category/item row selection와 wrong-target non-write |
| `020`~`022` | Category root 단위 validate/save/refresh 후보. | cancellation no-success. 자동 retry 없음. | 분류 저장/사용 중지/충돌/실패 문구 분리. | name/code의 필요 최소 진단만 검토; raw ID/exception 금지. | code duplicate, active child block, system-default rule, conflict, reload |
| `025`~`027` | Category root와 child item을 한 aggregate 파일에서 atomic save하는 후보. | cancellation no-success. 자동 retry 없음. | 항목 저장/사용 중지/충돌/실패 문구 분리. | raw ID/exception 금지. | per-parent code duplicate, inactive parent, scope flags, conflict, reload |

Runtime 확인은 구현 후 guarded Product preview에서 synthetic/non-personal fixture만 사용해야 하며 현재는 `NOT_AUTHORIZED`다.

## 8. 파일 저장 경계

`FILE_STORAGE_BOUNDARY = NOT_APPLICABLE`

화면 12, 13, 16, 19, 20의 persistence 후보에는 source file, attachment payload, file replacement command가 없다.

| 항목 | 판정 |
|---|---|
| 원본 파일 위치 | N/A |
| DB file content/metadata | N/A; 현재 DB도 없음 |
| extension/size/signature/SHA | N/A |
| staging/final move | N/A |
| path traversal | N/A |
| logical/physical file delete | N/A |
| orphan file cleanup | N/A |

화면 12에서 화면 17로 이동하는 command는 navigation일 뿐이며 실제 attachment persistence는 기존 화면 17 계약이 소유한다. 따라서 이번 extension에서 파일 저장 요구를 추가하지 않는다.

## 9. Schema 및 migration 영향

### 9.1 현재 schema

- 실제 provider: JSON files
- Policy store: `policies.json`
- Claim store: `claims.json`
- envelope: `JsonFileEnvelope<T>`
- current schema version: `1`
- save: 같은 파일에 대한 temp write 후 `File.Move(..., overwrite: true)`
- incompatible schema version: read 거부
- DB/SQLite/schema migration framework: 없음

### 9.2 영향 분류

| 후보 | 기존 schema | 영향 | Migration/backfill | Rollback/compatibility |
|---|---|---|---|---|
| Policy create with current `DisplayTitle` + `ReferenceDate` | FULL | record shape 변경 없음 | 없음 | 낮음, 단 UI field mapping 결정 필요 |
| Policy disable with current `DisabledAt` | FULL | record shape 변경 없음 | 없음 | 낮음, reference/concurrency 계약 필요 |
| Policy update limited to current fields | PARTIAL | interface/provider method 추가, record shape는 유지 가능 | 없음 | lost update 방지 token/version 결정 필요 |
| Policy hold/delete/restore | NONE 또는 PARTIAL | state semantics와 query contract 확장 | 기존 row backfill 여부는 선택 state에 따라 다름 | active-only query와 하위 호환 위험 |
| 화면 12의 7개 field 전체 저장 | PARTIAL | `PolicyRecord`/JSON schema 확장 | 기존 policy backfill/default 필요 | `schemaVersion` 전환 및 rollback 계획 필요 |
| FamilyMember | NONE | 신규 model/contract/provider와 versioned JSON store candidate | 신규 store면 기존 데이터 backfill 없음 | file introduction rollback은 가능하나 소비자 계약 필요 |
| Category + CategoryItem | NONE | 신규 aggregate/model/contract/provider와 versioned JSON store candidate | 신규 store면 기존 데이터 backfill 없음 | parent/item 저장 topology가 compatibility를 좌우 |
| SQLite/repository 도입 | NONE | 전면 신규 architecture | 별도 import/migration plan 필요 | 현재 배치에서 권고하지 않으며 승인되지 않음 |

`MIGRATION_SOURCE_STATE = NOT_CREATED`이며 실제 DB 실행 전 별도 architecture, migration, backup, rollback 승인 gate가 필요하다.

## 10. 사용자 결정표

모든 결정 상태는 `PENDING_USER_DECISION`이다.

| 결정 ID | 관련 화면/명령 | 결정 질문 | Option A | Option B | Option C | 권고안과 근거 | 미결정 시 영향 | 상태 |
|---|---|---|---|---|---|---|---|---|
| `DEC-PER-001` | 12 / `001` | 화면 12에서 어떤 Policy field를 persistence할 것인가? | 현재 `DisplayTitle` + 명시적으로 mapping한 `ReferenceDate`만 저장. 낮은 schema 영향. | 7개 field 전체를 model/schema에 추가. UX 충족, migration/backfill 필요. | write를 계속 disabled. | **C 권고.** 가입일/보험기간 중 `ReferenceDate` mapping과 가족/보험사 owner가 source에 없으므로 먼저 field contract가 필요하다. | `001` 구현 불가. | PENDING_USER_DECISION |
| `DEC-PER-002` | 12 / `001`~`004` | Policy lifecycle 범위를 어디까지 열 것인가? | create/update-current-fields + deactivate만. hold/delete/restore disabled 유지. | hold/soft-delete/restore까지 신규 상태 모델 도입. | hard delete 허용. | **A 권고.** 현재 Add/Disable 계약과 가장 가깝고 Claim/document reference 손실을 피한다. | `002`, `003`은 disabled, `001/004`만 제한 구현 가능. | PENDING_USER_DECISION |
| `DEC-PER-003` | 13 / `008` | FamilyMember identity와 중복 기준은 무엇인가? | generated ID identity, pseudonymous display name/relation/active/memo, 동일 display name 허용. | display name을 case-insensitive unique key로 사용. | Family persistence 계속 defer. | **A 권고.** 표시명은 안정 identity가 아니며 개인정보 최소화를 유지한다. | Family model/storage 설계 불가. | PENDING_USER_DECISION |
| `DEC-PER-004` | 13 / `009`, `010` | Family lifecycle과 참조 중 처리 방식은 무엇인가? | deactivate만, hard delete/restore deferred; 참조 중 deactivate는 명시 정책 전 차단. | soft delete + restore. | hard delete. | **A 권고.** 아직 Policy 연결 계약이 없어 비가역 삭제는 위험하다. | `009` disabled, `010`도 reference rule 확정 전 제한. | PENDING_USER_DECISION |
| `DEC-PER-005` | 16/19/20 / `015`, `016`, `020`, `025` | Category/Item aggregate와 uniqueness는 무엇인가? | Category root에 items 포함. category code global unique, item code parent 범위 unique, display name은 identity 아님. | Category와 Item을 별도 store/aggregate로 관리. | 전부 defer. | **A 권고.** parent/item을 한 save boundary로 묶어 partial write를 줄인다. | Category persistence 계약 불가. | PENDING_USER_DECISION |
| `DEC-PER-006` | 16/19/20 / `016`, `021`, `022`, `026`, `027` | Category lifecycle과 참조 중 parent 처리 방식은 무엇인가? | item deactivate 지원, active item이 있으면 parent deactivate 차단, delete/restore deferred. | parent cascade deactivate/restore. | hard delete/cascade delete. | **A 권고.** 현재 consumer/reference inventory가 없어 cascade나 hard delete를 정당화할 수 없다. | delete는 disabled, deactivate도 제한 구현 불가. | PENDING_USER_DECISION |
| `DEC-PER-007` | 16 / `015`, `016` | 수정/삭제가 어느 row type을 대상으로 하는가? | explicit selected row type + ID가 생길 때까지 disabled 유지. | 마지막 route/context로 target 추론. | 전체 목록 단위 작업. | **A 권고.** 현재 command parameter가 없어 추론은 wrong-target write 위험을 만든다. | `015`, `016` 구현 금지 유지. | PENDING_USER_DECISION |
| `DEC-PER-008` | 모든 write 후보 | persistence provider를 무엇으로 할 것인가? | 현재 JSON architecture에 versioned store 추가. DB 없음. | SQLite + repository architecture 도입. | session-only state. | **A 권고.** 현재 provider/test/runtime root와 일치하며 별도 DB migration을 피한다. | exact implementation layer/file list 불가. | PENDING_USER_DECISION |
| `DEC-PER-009` | 모든 write 후보 | 동시 수정과 transaction/retry 경계는 무엇인가? | process-scoped aggregate gate + optimistic `UpdatedAt`/version; 한 aggregate file 단위 save; 자동 retry 없음. | 현재 per-ViewModel gate만 유지. | cross-process lock/transaction 도입. | **A 권고.** 같은 process의 여러 ViewModel 경쟁을 막고 silent lost update를 줄인다. Cross-process readiness는 별도 gate다. | correctness test와 failure contract 불가. | PENDING_USER_DECISION |
| `DEC-PER-010` | 모든 write 후보 | 감사 이력 수준은 무엇인가? | CreatedAt/UpdatedAt/DisabledAt만 유지/확장. | append-only business event log 추가. | 감사 timestamp 없음. | **A 권고.** 현재 모델과 일치하고 민감한 payload log를 늘리지 않는다. | model field와 test expectation 불가. | PENDING_USER_DECISION |

### 10.1 Already defined

| ID | 정의된 사항 | 근거 |
|---|---|---|
| `ALREADY-DEFINED-PER-001` | 기존 Product policy create의 active display title 중복은 trim 후 case-insensitive로 차단하고 disabled title 재사용을 허용한다. | `SRC-POL-04`, `TEST-POL-01` |
| `ALREADY-DEFINED-PER-002` | 기존 Product policy disable은 active claim이 있으면 application layer에서 차단한다. Storage provider 단독 호출은 같은 규칙을 강제하지 않는다. | `SRC-POL-02/04`, `TEST-POL-01` |
| `ALREADY-DEFINED-PER-003` | 화면 17/18의 file validation, SHA comparison, duplicate section, compensation은 기존 문서 등록 계약이 소유한다. | `SRC-DOC-01`, `TEST-DOC-01` |
| `ALREADY-DEFINED-PER-004` | 대상 다섯 화면의 persistence 후보에는 file replacement, physical file deletion, orphan attachment cleanup이 적용되지 않는다. | `SRC-UI-02/04` |

## 11. 구현 배치 후보 분할

모든 배치는 `NOT_AUTHORIZED`다.

| Batch | 포함 Command | 선행 결정 | 예상 변경 계층 | Schema 영향 | Tests/runtime evidence | Rollback/보상 | 위험 |
|---|---|---|---|---|---|---|---|
| `T3-PER-A` Policy bounded integration | `001`, `004` | `001`, `002`, `008`, `009`, `010` | 화면 12 전용 VM/adapter, 기존 policy service extension, composition, tests | current fields면 낮음; 7-field면 version/backfill 필요 | VM/unit, provider, lifecycle integration, guarded synthetic runtime | single-file rollback; refresh failure contract 필요 | T3_HIGH |
| `T3-PER-B` Family persistence | `008`, `010`; `009`는 disabled 유지 후보 | `003`, `004`, `008`, `009`, `010` | Family model/service/JSON provider, screen VM, composition, tests | 신규 versioned JSON store | privacy/duplicate/deactivate/reload/concurrency tests | single aggregate file; no file compensation | T3_HIGH |
| `T3-PER-C` Category aggregate persistence | `020`, `022`, `025`, `027`; `015`, `016`은 explicit selection 후 | `005`, `006`, `007`, `008`, `009`, `010` | Category aggregate/service/JSON provider, selection-aware VM, composition, tests | 신규 versioned Category aggregate store | parent/item uniqueness, lifecycle, conflict, reload, wrong-target non-write | one aggregate file rollback | T3_HIGH |
| `T3-PER-D` Optional lifecycle expansion | `002`, `003`, `009`, `021`, `026`, 필요 시 restore | 별도 lifecycle 재승인 | state model/query/service/UI/tests | 기존/new schema state 확장 가능 | reference integrity, restore, idempotency tests | migration rollback와 recovery 필요 | T3_HIGH |

배치 파일 목록은 사용자 결정과 실제 구조 선택 뒤 별도 exact-scope planning에서 계산해야 한다. 현재 파일 수나 CREATE/MODIFY 목록을 확정하지 않는다.

## 12. 위험과 보호 범위

### 12.1 Blocking risks before implementation

1. 화면 12의 7개 field와 현재 `PolicyRecord` 2개 업무 field 사이 mapping이 없다.
2. Family/Category/CategoryItem model, storage, identity, lifecycle이 없다.
3. 화면 16의 수정/삭제에는 selected aggregate type/id가 없다.
4. 현재 JSON provider에는 multi-ViewModel/process concurrency와 optimistic conflict가 없다.
5. hard delete, restore, cascade, hold 의미가 정의되지 않았다.
6. 여러 파일 write를 묶는 transaction은 없으며 그렇게 간주할 수 없다.
7. DB/SQLite/repository/migration은 존재하지도 승인되지도 않았다.

### 12.2 Protected scope

- 사용자 승인 Product UI 21/21 유지
- disabled command 상태 유지
- 화면 17/18 문서 등록 기능 유지
- Gate 8 PASS 유지
- `docs/439`와 역사적 Gate 8 문서 불변
- `data/claimdoc` 접근 금지
- 실제 사용자 보험·청구·문서 파일 접근 금지
- raw ID, local path, SHA, exception detail 비노출
- source/test/schema/migration/runtime 변경 및 실행 금지

### 12.3 추정 방지 감사

- Category root/child topology는 `CANDIDATE`이며 현재 source 사실이 아니다.
- FamilyMember field와 Policy 연결은 `CANDIDATE`다.
- JSON 신규 file name, schema version, model type은 아직 정하지 않았다.
- transaction, retry, restore, delete semantics는 `PENDING_USER_DECISION`이다.
- 미결정 사항은 defect가 아니라 후속 구현 gate 입력이다.

## 13. 최종 상태

`FAMILYCLAIMREF_PERSISTENCE_EXTENSION_SCOPE_DISCOVERED_USER_DECISION_REQUIRED`

| State | Value |
|---|---|
| `PERSISTENCE_SCOPE_DISCOVERY_STATE` | `PASS` |
| `PERSISTENCE_CONTRACT_STATE` | `CANDIDATE_PENDING_USER_DECISION` |
| `PERSISTENCE_IMPLEMENTATION_STATE` | `NOT_AUTHORIZED` |
| `SCHEMA_CHANGE_STATE` | `NOT_AUTHORIZED` |
| `DB_EXECUTION_STATE` | `NOT_AUTHORIZED` |
| `GATE8_STATE` | `PASS` |

다음 최소 작업은 사용자가 `DEC-PER-001`부터 `DEC-PER-010`까지 선택하는 것이다. 사용자 결정 전에는 persistence 구현, schema 변경, migration 생성, DB 실행, runtime 검증으로 진행하지 않는다.

## 14. T3-PER-C 사용자 결정 및 구현 상태 (2026-08-07)

이 절은 1~13절의 조사 시점 `PENDING_USER_DECISION` 기록을 삭제하지 않고, 이후 사용자가 확정한 T3-PER-C 계약과 구현 결과를 기록한다. 아래 결정은 Category/Item 범위에만 적용하며 T3-PER-D, hard delete, restore, DB, cross-process locking 승인이 아니다.

### 14.1 Superseding user decisions

| Decision | Selected contract | State |
|---|---|---|
| `DEC-PER-005` | Option A: Category aggregate root가 Item child를 소유한다. Category code는 aggregate 전역, Item code는 parent 범위에서 `OrdinalIgnoreCase` unique다. | `USER_APPROVED_T3_PER_C` |
| `DEC-PER-006` | Option A: Item deactivate/reactivate와 Category reactivate를 지원한다. active Item이 있으면 Category deactivate를 차단한다. hard delete, restore, cascade는 구현하지 않는다. | `USER_APPROVED_T3_PER_C` |
| `DEC-PER-007` | 화면 16/19/20은 selected Category/Item `RowId`와 읽은 `aggregateVersion`을 전달한다. list index, sort order, code, first row를 write target으로 사용하지 않는다. | `USER_APPROVED_T3_PER_C` |
| `DEC-PER-008` | Option A: 기존 runtime-root JSON architecture에 신규 versioned store를 추가한다. DB/SQLite/repository migration은 없다. | `USER_APPROVED_T3_PER_C` |
| `DEC-PER-009` | Option A를 구체화하여 canonical JSON path별 process-scoped gate와 exact `expectedAggregateVersion`을 사용한다. 같은 version 동시 write는 하나만 성공한다. cross-process 보장은 제공하지 않는다. | `USER_APPROVED_T3_PER_C` |
| `DEC-PER-010` | Option A: `CreatedAt`, `UpdatedAt`, `DisabledAt`만 저장한다. business event log를 추가하지 않는다. | `USER_APPROVED_T3_PER_C` |

### 14.2 Implemented persistence contract

- Store file: runtime metadata root의 `categories.json`
- Envelope: `schemaVersion`, `aggregateVersion`, `savedAt`, `categories[]`, `categories[].items[]`
- Schema version: `1`
- Missing store: file을 만들지 않고 empty aggregate version `0` 반환
- Successful mutation: aggregate version exactly `+1`
- Identity: Category/Item immutable generated `Guid RowId`; Item은 explicit `ParentCategoryId` 소유
- Normalization: name/code/description trim, blank required value 거부, negative sort order 거부
- Uniqueness: Category code global, Item code per parent, 모두 `OrdinalIgnoreCase`
- Wrong target: missing RowId, parent mismatch, item reparent 요청은 structured no-success 및 no-write
- Lifecycle: create active, item create/reactivate는 active parent에서만 허용, active item은 parent deactivate 차단
- Concurrency: canonical store path별 process-scoped gate + optimistic aggregate version; 자동 retry 없음
- Atomic persistence: same-directory unique temp write, flush-to-disk, deserialize/schema validation, replace/rename, 직전 정상본 `.bak`, temp best-effort cleanup
- Failure policy: malformed/unsupported schema fail closed; 자동 migration/recovery 없음
- Privacy/UI: 제품 메시지에 raw ID, 절대 경로, SHA, 예외 전문을 노출하지 않는다. DataGrid 접근성 이름도 display name만 사용한다.

### 14.3 Exact implementation file list

CREATE 12:

- `app/FamilyClaimRef.App/Models/Storage/CategoryAggregateModels.cs`
- `app/FamilyClaimRef.App/Services/Storage/ICategoryAggregateStorageService.cs`
- `app/FamilyClaimRef.App/Services/Storage/JsonCategoryAggregateStorageService.cs`
- `app/FamilyClaimRef.App/ViewModels/CategoryManagementViewModel.cs`
- `app/FamilyClaimRef.App/Views/ProductCategoryManagementView.xaml`
- `app/FamilyClaimRef.App/Views/ProductCategoryManagementView.xaml.cs`
- `app/FamilyClaimRef.App/Views/ProductCategoryEditorView.xaml`
- `app/FamilyClaimRef.App/Views/ProductCategoryEditorView.xaml.cs`
- `app/FamilyClaimRef.App/Views/ProductCategoryItemEditorView.xaml`
- `app/FamilyClaimRef.App/Views/ProductCategoryItemEditorView.xaml.cs`
- `tests/FamilyClaimRef.App.Tests/CategoryManagementViewModelTests.cs`
- `tests/FamilyClaimRef.App.Tests/JsonCategoryAggregateStorageServiceTests.cs`

MODIFY 10:

- `app/FamilyClaimRef.App/Composition/AppServices.cs`
- `app/FamilyClaimRef.App/ProductShell/ProductShellWindow.xaml`
- `app/FamilyClaimRef.App/Resources/UiStrings.xaml`
- `app/FamilyClaimRef.App/Services/Localization/UiTextKeys.cs`
- `app/FamilyClaimRef.App/ViewModels/ProductShellViewModel.cs`
- `tests/FamilyClaimRef.App.Tests/Composition/AppServicesTests.cs`
- `tests/FamilyClaimRef.App.Tests/DocumentRegistrationLifecycleGate8Tests.cs`
- `tests/FamilyClaimRef.App.Tests/ProductShellViewModelTests.cs`
- `tests/FamilyClaimRef.App.Tests/Services/Localization/ResourceUiTextProviderTests.cs`
- `docs/440_POLICY_CLAIM_PERSISTENCE_EXTENSION_SCOPE_DISCOVERY_AND_USER_DECISION.md`

Exact changed file count: `22` (`CREATE 12`, `MODIFY 10`).

### 14.4 Validation and independent review evidence

- Release build: PASS, warnings/errors `0/0`
- Category focused tests: `26/26`, failed/skipped `0/0`
- Full Release regression: `767/767`, failed/skipped `0/0` (`740` baseline + `27` contract tests)
- Resource/constants parity: `214/214`
- `Ui.Product.*` parity: `157/157`
- Runtime screen 16/19/20: Category create, Item create, reload persistence PASS
- Runtime duplicate: trim/case-insensitive duplicate message PASS; aggregate version/count unchanged
- Runtime parent deactivate block: active Item 존재 시 safe message PASS; aggregate version unchanged
- Runtime stale-version conflict: synthetic external version advance 후 conflict message PASS; attempted code write `0`
- Runtime accessibility: DataGrid UI Automation raw RowId/ParentCategoryId/GUID exposure `0`
- Runtime evidence: project 밖의 신규 TEMP roots만 사용; 실제 사용자 data root와 `data/claimdoc` 미접근
- Independent review repaired findings: inactive parent + active item malformed JSON acceptance, production test fallback constructor, record default `ToString()` accessibility identifier exposure
- Independent review final Blocking/Major/Minor: `0/0/0`
- `git diff --check`: PASS

stale-version runtime fixture 준비 중 Windows PowerShell 기본 인코딩으로 첫 synthetic JSON이 유효하지 않게 된 실행자 오류가 있었으며, 직전 synthetic 값과 고정 RowId로 UTF-8 복원한 뒤 격리 root에서만 검증을 완료했다. source, 실제 사용자 data, production runtime root에는 영향이 없다.

### 14.5 Current state and non-authorization

`FAMILYCLAIMREF_T3_PER_C_CATEGORY_AGGREGATE_PERSISTENCE_IMPLEMENTED_VERIFIED_PENDING_PR`

| State | Value |
|---|---|
| `T3_PER_C_DECISION_STATE` | `USER_APPROVED` |
| `T3_PER_C_IMPLEMENTATION_STATE` | `IMPLEMENTED` |
| `PERSISTENCE_STATE` | `VERIFIED` |
| `CONCURRENCY_STATE` | `VERIFIED_PROCESS_SCOPE_ONLY` |
| `RUNTIME_UI_STATE` | `VERIFIED` |
| `INDEPENDENT_REVIEW_STATE` | `PASS` |
| `PR_STATE_AT_DOCUMENT_COMMIT` | `PENDING` |
| `PR_CI_STATE_AT_DOCUMENT_COMMIT` | `PENDING` |
| `PRODUCTION_READINESS_STATE` | `NOT_EVALUATED` |
| `DEPLOYMENT_STATE` | `NOT_AUTHORIZED` |

Hard delete, restore, cascade, cross-process locking, automatic migration/recovery, DB/SQLite, production readiness, deployment는 이번 결정과 구현에 포함되지 않는다.
