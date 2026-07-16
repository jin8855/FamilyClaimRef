# Product UI Shell Phase 1D2 Guarded Runtime Entry Decision Scope Plan

## A. Status

- Task ID: `POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1D2_GUARDED_RUNTIME_ENTRY_EXACT_SCOPE_DECISION_DOCS_BATCH`
- Marker: `POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1D2_GUARDED_RUNTIME_ENTRY_DECISION_SCOPE_READY`
- Work type: documentation-only exact-scope decision
- Implementation target now: `0`

## B. Baseline

- Hash: `ced4a00f16a55bbe1e76e0b016922983bf1aefd5`
- Subject: `feat(familyclaimref): compose product shell view model graph`
- Initial working tree: clean
- Initial staged files: none
- Latest full solution tests: PASS `382/382`
- Resources/constants: `68/68`
- `Ui.Product.*` resources/constants: `12/12`
- `AppServices.ProductShellViewModel`: committed and read-only
- Default startup Window: `MainWindow`
- ProductShell production runtime caller: absent
- Guarded ProductShell entry: absent
- Default ProductShell startup ready: no

## C. Decision Purpose

이 배치는 현재 default `MainWindow` startup을 보존하면서 명시적 command-line opt-in으로만 `ProductShellWindow`를 선택하는 향후 계약을 결정한다. 구현, build, test, app launch 및 runtime evidence 수집은 수행하지 않는다.

결정 대상은 다음과 같다.

1. `StartupEventArgs.Args` 기반 startup guard.
2. default 및 preview Window ownership.
3. side-effect 없는 startup-mode selector와 test visibility.
4. 향후 exact implementation file list.
5. automated validation과 별도 manual smoke의 경계.

## D. Exact Documentation Scope

이번 배치에서 생성하는 파일은 정확히 다음 5개다.

1. `docs/392_POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1D2_GUARDED_RUNTIME_ENTRY_DECISION_SCOPE_PLAN.md`
2. `docs/393_POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1D2_STARTUP_ARGUMENT_AND_WINDOW_OWNERSHIP_RECONCILIATION.md`
3. `docs/394_POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1D2_GUARDED_ENTRY_STRATEGY_AND_EXACT_FILE_LIST_DECISION_CANDIDATE.md`
4. `docs/395_POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1D2_GUARDED_ENTRY_VALIDATION_TEST_GATE_PLAN.md`
5. `docs/396_POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1D2_GUARDED_ENTRY_COMMIT_CANDIDATE_REVIEW.md`

예약된 구현 결과 문서 `docs/397_POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1D2_GUARDED_RUNTIME_ENTRY_IMPLEMENTATION_RESULT_REVIEW.md`는 생성하지 않는다.

## E. Non-Scope

- `App.xaml` 또는 `App.xaml.cs` 수정
- startup selector 또는 selector test 생성
- `AppServices`, MainWindow, ProductShellWindow 또는 ViewModel 수정
- resource, project, solution 또는 package 수정
- Window 생성, `Show`, `ShowDialog` 또는 runtime entry 추가
- build, test, app launch, OpenFileDialog, workflow 또는 manual smoke 실행
- staging, commit 또는 push

## F. Approval Matrix

| Area | Candidate state | Approved now |
|---|---|---|
| command-line guard | decision candidate | no |
| exact preview token | `--product-shell-preview` candidate | no |
| startup selector creation | future candidate | no |
| `App.xaml.cs` modification | future candidate | no |
| `ProductShellWindow` construction | future guarded branch candidate | no |
| guarded runtime entry | future candidate | no |
| manual preview launch | separate future approval | no |
| default startup replacement | excluded | no |
| environment/AppContext guard | excluded | no |
| exact implementation file list | planning candidate only | no |
| docs/397 creation | future implementation result only | no |

## G. Current Boundary

- Composition is complete; runtime entry is not.
- Guarded preview feasibility does not make ProductShell the default product startup.
- Policy contract management, claim case management, and fresh-root target creation remain default-startup blockers.
- Implementation must not start from this document batch.

## H. Execution Record

- Source/test/XAML/ViewModel/resource/project changes: none
- docs/397 created: no
- Build/test/app launch: not run
- Git add/stage/commit/push: not run
- Implementation target now: `0`
