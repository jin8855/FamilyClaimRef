# Policy Claim Product UI Shell Gate8 Real Document Registration Attachment and Persistence Decision Scope Plan

## A. Status

- Status: `DECISION_PACKAGE_SCOPE_PLAN`
- Marker: `POLICY_CLAIM_PRODUCT_UI_SHELL_GATE8_REAL_DOCUMENT_REGISTRATION_ATTACHMENT_AND_PERSISTENCE_DECISION_SCOPE_READY`
- Project: `C:\EtcProject\FamilyClaimRef`
- Baseline branch: `main`
- Baseline commit: `79d8f1d5b76b22b0ef1a65fbf781c833bbcd7fff`
- Baseline subject: `docs(familyclaimref): close gate7 default startup transition`
- Baseline parent: `2ff924c846d2b5f7fad905afa5a7a90d93af31cf`
- Baseline parent subject: `feat(familyclaimref): make product shell the default startup`
- Baseline `docs/412` SHA-256: `021AEE4719B402E465EBC2E74B958668E6BF19DF37A72112370B8D16020CB4FA`
- Baseline worktree: clean

## B. Purpose

Gate8은 ProductShell의 문서 등록 화면을 실제 파일 첨부 및 metadata 영속화 흐름에 연결하기 전에 필요한 결정 경계를 확정하는 단계다.

이번 package의 목적은 다음 항목을 구현 없이 결정 후보로 고정하는 것이다.

1. 기존 `DocumentRegistrationWorkflow` 재사용 여부
2. 외부 원본 파일과 앱 관리 복사본의 authoritative payload 관계
3. 파일 검증, metadata, 중복, 취소, 재진입, 실패 복구 정책
4. ProductShell과 MainWindow validation harness 사이의 composition 경계
5. 향후 구현에 허용할 exact file list와 검증 gate

## C. Current Baseline

| Item | Current evidence |
|---|---|
| Default startup | ProductShell default startup transition complete |
| ProductShell navigation | Five destinations remain available |
| Registration UI | `ProductDocumentRegistrationView` is stateful and connected to `DocumentRegistrationViewModel` |
| Existing use case | `DocumentRegistrationWorkflow` coordinates attachment and policy/claim link creation |
| File storage | `LocalFileAttachmentService` copies into the app-managed attachment root |
| Metadata storage | `JsonDocumentStorageService` persists document and link JSON files |
| Runtime roots | `EnvironmentRuntimeRootProvider` supplies metadata and attachment roots |
| Allowed extensions | `pdf`, `jpg`, `jpeg`, `png` |
| Current resource baseline | resource/constants `91/91`, `Ui.Product.*` `35/35` |
| Current test baseline | latest known full test `436/436 PASS` |
| Gate7 evidence | default ProductShell startup complete; no Gate8 real document execution evidence |

## D. ALREADY_DONE

The following approved baseline remains protected and is not reimplemented or revalidated in this package.

- Phase 1 ProductShell structure
- Phase 1B Home content host
- Phase 1B2 stateful document registration UI
- Phase 2A policy and claim management
- Gate 6 user visual acceptance
- Gate 7 ProductShell default startup transition
- Gate 7 final user visual review and documentation closure
- no-argument default launch to `ProductShellWindow`
- `--product-shell-preview` compatibility path
- MainWindow source preservation
- default runtime MainWindow instance `0`
- top-level window `1`
- ProductShellWindow instance `1`
- navigation destinations `5`
  - Home
  - PolicyContracts
  - ClaimCases
  - DocumentRegistration
  - DocumentList
- navigation selected count `1`
- canonical ProductShell construction path
- policy creation and duplicate blocking
- claim creation and policy association
- input retention and screen-entry message reset
- registration target creation/removal
- stale target selection clear
- resource/constants `91/91`
- `Ui.Product.*` `35/35`
- full tests `436/436 PASS`
- build warnings/errors `0/0`
- UIA-targeted click `26/26`, `100%`
- forbidden UIA exposure `0`
- production root access/deletion `0/0`
- persistent environment mutation `0`
- deployment/production readiness not approved

## E. Exact Package Scope

이번 작업에서 생성하는 exact file list는 다음 여섯 문서뿐이다.

1. `docs/413_POLICY_CLAIM_PRODUCT_UI_SHELL_GATE8_REAL_DOCUMENT_REGISTRATION_ATTACHMENT_AND_PERSISTENCE_DECISION_SCOPE_PLAN.md`
2. `docs/414_POLICY_CLAIM_PRODUCT_UI_SHELL_GATE8_CURRENT_SOURCE_WORKFLOW_STORAGE_AND_COMPOSITION_RECONCILIATION.md`
3. `docs/415_POLICY_CLAIM_PRODUCT_UI_SHELL_GATE8_FILE_LIFECYCLE_METADATA_SECURITY_AND_FAILURE_POLICY_DECISION_CANDIDATE.md`
4. `docs/416_POLICY_CLAIM_PRODUCT_UI_SHELL_GATE8_PRODUCT_COPY_REENTRY_DUPLICATE_AND_CANCEL_POLICY_DECISION_CANDIDATE.md`
5. `docs/417_POLICY_CLAIM_PRODUCT_UI_SHELL_GATE8_EXACT_IMPLEMENTATION_FILE_LIST_AND_VALIDATION_TEST_GATE_PLAN.md`
6. `docs/418_POLICY_CLAIM_PRODUCT_UI_SHELL_GATE8_DECISION_PACKAGE_REVIEW.md`

## F. Decision Status Vocabulary

| Status | Meaning |
|---|---|
| `CURRENT` | 현재 source에서 확인된 동작 또는 구조 |
| `CANDIDATE` | Gate8 구현 전에 사용자 승인이 필요한 결정 후보 |
| `DEFERRED` | Gate8 이후 별도 승인으로 넘기는 항목 |
| `FORBIDDEN` | Gate8에서도 수집, 저장, 표시 또는 구현하지 않는 항목 |
| `PROTECTED` | 향후 Gate8 구현 후보에서도 변경하지 않을 파일 또는 경계 |

`CANDIDATE`는 구현 승인과 동일하지 않다. 이 package가 검토 가능한 상태가 되어도 source, test, resource 변경은 승인되지 않는다.

## G. In Scope

- current source call chain 정합성 확인
- current composition and lifetime 정합성 확인
- current file and JSON storage behavior 확인
- architecture Candidate A/B/C 비교
- authoritative payload 선택 후보
- file validation policy 후보
- metadata classification 후보
- duplicate semantics 후보
- cancel, replacement, reentry, success reset 후보
- atomicity, compensation, rollback 후보
- ProductShell copy/resource 후보
- future exact implementation file list
- future static, unit, integration, ProductShell contract, runtime UIA gates
- blocker count와 user decision item 정리

## H. Explicit Non-Scope

- source code 수정
- test code 수정
- XAML 수정
- ViewModel 수정
- resource 수정
- project file 수정
- app launch
- `OpenFileDialog` 실행
- 실제 document registration workflow 실행
- 실제 또는 synthetic 파일 생성/선택/복사
- production runtime root 접근
- runtime metadata 또는 attachment 접근/삭제
- cleanup 실행
- `data/claimdoc` 접근, 열람, 목록화, 사용, 이동, 삭제, stage 또는 commit
- DB, SQLite, OCR, repository 구현
- product deployment 또는 production release
- Git stage 또는 commit

## I. Architecture Candidates

| Candidate | Description | Scope judgment |
|---|---|---|
| A | 기존 `DocumentRegistrationViewModel` -> `DocumentRegistrationWorkflow` -> coordinator/storage 경로를 재사용하고 하위 validation/storage 정책만 확장 | `RECOMMENDED CANDIDATE` |
| B | ProductShell 전용 별도 등록 workflow와 storage 경로 생성 | `REJECT CANDIDATE`; 중복 동작과 정책 drift 위험 |
| C | View 또는 code-behind가 file/storage service를 직접 호출 | `REJECT`; UI I/O 결합과 rollback 책임 분산 |

Candidate A를 기본 recommendation으로 기록하지만 사용자 승인 전 구현은 금지한다.

### I1. Protected Boundaries

- `App.xaml` and `App.xaml.cs`
- default no-argument ProductShell startup
- `--product-shell-preview` compatibility
- MainWindow source and validation-harness composition
- ProductShell navigation count `5` and selected count `1`
- existing policy/claim management workflow
- `FileNamePolicyService` allowlist
- runtime root provider boundary
- DocumentList UI behavior
- Home expansion
- production runtime root and all runtime artifacts
- all files outside the future candidate list in docs/417

## J. Package-Wide Candidate Decisions

1. Architecture: Candidate A
2. Authoritative payload: successful registration 이후 앱 관리 복사본
3. Original external path: durable metadata 저장 금지
4. Duplicate key: active target scope의 `target kind + target ID + SHA-256`
5. Maximum file size: `25 MiB` (`26,214,400` bytes) candidate
6. Signature validation: PDF/JPEG/PNG required candidate
7. Success reset: file and document metadata reset, valid target selection retained
8. Failure behavior: retry 가능한 입력 유지
9. Resource addition: eight `Ui.Product.DocumentRegistration.*` keys candidate

## K. Decision Questions Requiring User Approval

| ID | Decision item | Candidate |
|---|---|---|
| D1 | Architecture | Reuse existing workflow and extend lower layers |
| D2 | Payload authority | App-managed copied file becomes authoritative |
| D3 | File policy | 25 MiB, non-zero, extension/signature agreement; create a read-only selection-time SHA-256 runtime snapshot and compare it with the staged payload SHA-256 at registration; mismatch requires reselection; length/last-write are auxiliary only; selection hash and source path are not durable metadata |
| D4 | Duplicate policy | Serialize duplicate query plus registration for the same target/SHA-256 inside one process so exactly one concurrent attempt succeeds; cross-process uniqueness is not provided |
| D5 | Reentry/reset/navigation | Preserve draft on cancel/failure; reset after success; allow navigation while busy |
| D6 | Metadata schema | Apply the `18/1/3/1/8` classification |
| D7 | Atomicity and recovery | Normal exceptions use compensation and the current contract guarantees successful-return consistency; crash windows after final move can leave an orphan final payload or a Document without a link while startup recovery is deferred |
| D8 | Product copy/resources | Add exactly eight candidate keys |
| D9 | Implementation/test scope | Use docs/417 exact 35-file candidate and 37 new automated scenarios; the file count stays 35 because the concurrency case uses an existing candidate test file |

## L. Blocking Rules

다음 중 하나라도 발생하면 implementation readiness는 `HOLD`다.

- D1~D9 중 사용자 승인 누락
- picker filter와 `FileNamePolicyService` allowlist 불일치 미해결
- metadata schema migration/compatibility 전략 미승인
- file signature, size, changed-after-selection policy 미승인
- duplicate query/atomicity/compensation test 계획 미확정
- ProductShell에 raw path, raw internal ID, CLR type 또는 exception 노출
- exact implementation file list 밖 변경 필요
- production runtime root나 실제 document 접근 필요

## M. Completion Rule

이 decision package 자체는 아래 조건에서 `PASS_USER_REVIEW_PENDING`으로 판정할 수 있다.

- 여섯 문서가 모두 생성됨
- current source와 candidate policy가 구분됨
- 모든 수치와 exact file list가 문서 간 일치함
- implementation, runtime execution, stage, commit이 수행되지 않음

구현 readiness는 별도다. 사용자 승인 전 상태는 `HOLD_IMPLEMENTATION_NOT_AUTHORIZED`다.

## N. Scope Result

`POLICY_CLAIM_PRODUCT_UI_SHELL_GATE8_REAL_DOCUMENT_REGISTRATION_ATTACHMENT_AND_PERSISTENCE_DECISION_SCOPE_READY`

## O. Package Consistency Register

| Item | Package-wide value |
|---|---|
| Baseline HEAD | `79d8f1d5b76b22b0ef1a65fbf781c833bbcd7fff` |
| Baseline subject | `docs(familyclaimref): close gate7 default startup transition` |
| Baseline parent | `2ff924c846d2b5f7fad905afa5a7a90d93af31cf` |
| `docs/412` SHA-256 | `021AEE4719B402E465EBC2E74B958668E6BF19DF37A72112370B8D16020CB4FA` |
| Architecture | Candidate A, reuse existing workflow |
| Workflow owner | `DocumentRegistrationWorkflow` |
| File storage owner | `IFileAttachmentService` / `LocalFileAttachmentService` |
| Metadata repository owner | `IDocumentStorageService` / `JsonDocumentStorageService` |
| Target repository owner | `IPolicyClaimStorageService` / `JsonPolicyClaimStorageService` |
| Composition owner | `AppServices`; ProductShell window-scoped child ViewModel |
| Authoritative payload | App-managed copy after complete success |
| Reentry | Refresh targets, preserve draft, clear stale target/transient copy |
| Duplicate key | active `target kind + target ID + SHA-256` |
| Selection integrity | Read-only selection SHA-256 runtime snapshot compared with staged payload SHA-256; mismatch requires reselection; length/last-write are auxiliary only; selection hash and source path are not durable |
| Concurrency boundary | Same-process duplicate query plus registration is serialized; concurrent same target/SHA-256 yields exactly one success; cross-process guarantee is excluded |
| Picker cancel | Preserve prior valid selection and draft |
| Consistency contract | Successful-return consistency with normal-exception compensation; crash consistency and startup recovery remain deferred |
| Crash residual risk | Orphan final payload and Document without a link can remain after a process crash following final move |
| Current source inventory files | `58` |
| Metadata items | `31` |
| Metadata classification | `18/1/3/1/8` |
| Future exact implementation files | `35` |
| New resource key candidates | `8` |
| New automated scenario candidates | `37` |
| Unresolved blockers | `16` |
| Implementation readiness | `HOLD_IMPLEMENTATION_NOT_AUTHORIZED` |
| Deployment/production readiness | `NOT_AUTHORIZED`; multi-process uniqueness and startup recovery remain on hold |
| Documentation commit | `NOT_AUTHORIZED` |
| Non-approval | No source/test/resource/runtime/commit/deployment approval |
| Package final marker | `POLICY_CLAIM_PRODUCT_UI_SHELL_GATE8_REAL_DOCUMENT_REGISTRATION_ATTACHMENT_AND_PERSISTENCE_DECISION_PACKAGE_PASS_USER_REVIEW_PENDING` |
