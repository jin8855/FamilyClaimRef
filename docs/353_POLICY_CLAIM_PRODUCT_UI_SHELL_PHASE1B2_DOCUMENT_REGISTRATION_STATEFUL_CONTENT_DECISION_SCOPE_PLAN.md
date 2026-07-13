# Product UI Shell Phase 1B2 Document Registration Stateful Content Decision Scope Plan

## A. Status

- Marker: `POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1B2_DOCUMENT_REGISTRATION_STATEFUL_CONTENT_DECISION_SCOPE_READY`
- Task ID: `POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1B2_DOCUMENT_REGISTRATION_STATEFUL_CONTENT_EXACT_SCOPE_DECISION_DOCS_BATCH`
- Work type: documentation-only exact-scope decision candidate batch
- Implementation target now: 0

## B. Baseline

- Project: `C:\EtcProject\FamilyClaimRef`
- Full hash: `e79b4f8489f7c066abd0025fa856ce16bba8a6f5`
- Subject: `feat(familyclaimref): add title-only home content host`
- Initial working tree: clean
- Initial staged files: none
- Known full solution test result: PASS 351/351
- `UiStrings.xaml` `Ui.*` resources: 64
- `UiTextKeys.cs` `Ui.*` constants: 64
- `Ui.Product.*` resources/constants: 8/8

## C. Current Product Shell State

| Area | Current state | Evidence status |
|---|---|---|
| `ProductShellWindow` | tracked, compile-only | Source-confirmed |
| `ProductHomeView` | tracked, title-only | Source-confirmed |
| Home content mapping | `SelectedNavigationItem.Id == Home` XAML branch | Source-confirmed |
| `ProductDocumentRegistrationView` | absent | Source-confirmed |
| `ProductDocumentRegistrationViewModel` | absent | Source-confirmed |
| `ProductDocumentListView` | absent | Source-confirmed |
| ProductShell runtime entry | absent | Source-confirmed |
| Current app startup | `MainWindow` validation harness | Source-confirmed |
| ProductShell `AppServices` composition | absent | Source-confirmed |

## D. Purpose

Phase 1B2의 목적은 title-only placeholder를 추가하는 것이 아니다. 기존 document registration workflow와 상태를 ProductShell의 문서 등록 영역에서 재사용할 수 있는 최소 구조를 결정하는 것이다.

이번 문서 묶음은 다음을 결정 후보로 정리한다.

- 기존 `DocumentRegistrationViewModel` 직접 재사용 여부
- wrapper 또는 general current-content contract 필요 여부
- `ProductDocumentRegistrationView` DataContext 소유권
- `LoadTargetOptionsAsync`, `SelectFileAsync`, `RegisterAsync` interaction 경계
- ProductShell stateful content mapping 확장 방식
- product copy와 기존 validation-harness resource의 경계
- future implementation exact file list 후보
- future build/test gate

## E. In Scope

- ProductShell과 document registration source/test read-only reconciliation
- Candidate A~F architecture 비교
- direct reuse와 wrapper 판단
- view event forwarding 후보 판단
- target option load lifecycle 판단
- copy/resource conflict와 추가 key 후보 판단
- future implementation exact file list 후보 작성
- future validation command와 gate 작성

## F. Explicit Non-Scope

- source, test, XAML, ViewModel, resource, project file 수정
- `ProductDocumentRegistrationView` 생성
- `ProductDocumentRegistrationViewModel` 생성
- `ProductShellWindow` 또는 `ProductShellViewModel` 수정
- `DocumentRegistrationViewModel` 수정
- `AppServices` 또는 app startup 수정
- runtime entry 추가
- build/test/app launch/OpenFileDialog/manual workflow
- `ProductDocumentListView` 생성
- new command, router, service locator, DI container 구현
- DB/SQLite/repository/OCR/migration/backup/rollback 구현
- cleanup 또는 파일 삭제/이동/rename
- protected-path internal inspection
- Git stage/commit/push
- `docs/358_POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1B2_DOCUMENT_REGISTRATION_IMPLEMENTATION_RESULT_REVIEW.md` 생성

## G. Protected Boundaries

- `data/claimdoc/`는 ignore 상태만 확인하며 내부를 읽거나 나열하지 않는다.
- `docs/nightwork_20260706/`는 ignore 상태만 확인하며 내부를 읽거나 검색하지 않는다.
- 실제 개인정보, 보험사, 병원, 진단, 계약 또는 청구 문서 샘플을 사용하지 않는다.
- project root runtime artifact를 생성하거나 정리하지 않는다.

## H. Approval Matrix

| Approval item | Approved now |
|---|---|
| Candidate A architecture implementation | no |
| `ProductDocumentRegistrationView` creation | no |
| `ProductDocumentRegistrationViewModel` creation | no |
| `DocumentRegistrationViewModel` modification | no |
| `ProductShellViewModel` modification | no |
| `ProductShellWindow` modification | no |
| registration interaction implementation | no |
| registration lifecycle implementation | no |
| shared copy/resource reuse | no |
| new `Ui.Product.*` resource addition | no |
| runtime-message productization | no |
| `AppServices` modification | no |
| runtime entry | no |
| `MainWindow` replacement | no |
| App startup change | no |
| exact implementation file list | no |
| docs/358 creation | no |

## I. Phase Judgment

- Phase 1B2 remains a phased product-shell slice, not a scope deletion.
- A functional path must reuse the existing workflow boundary rather than duplicate or bypass it.
- A title-only registration placeholder is not recommended.
- Source evidence is sufficient to identify a preferred architecture candidate.
- Copy/resource, lifecycle, and composition approvals are not sufficient to authorize implementation.
- Final implementation readiness: `BLOCKED_PENDING_SEPARATE_APPROVALS`.
- Source/test/XAML/ViewModel/resource/project changes: none.
- Build/test: not run.
- Stage/commit: not run.

## J. Batch Boundary

This batch ends after `docs/353~357` are created and validated. It does not authorize implementation, build/test, staging, commit, or runtime execution.
