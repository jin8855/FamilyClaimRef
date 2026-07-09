# Policy Claim Scenario 9 Cleanup Policy Review Scope Plan

## 1. Status

SCENARIO9_CLEANUP_POLICY_REVIEW_PLAN_ONLY

## 2. Marker

POLICY_CLAIM_SCENARIO9_CLEANUP_POLICY_REVIEW_SCOPE_PLANNED

## 3. 기준 Commit

- `1fd475a refactor(familyclaimref): apply approved korean resource copy`

## 4. 목적

Scenario 9 isolated runtime artifact cleanup의 정책과 제외 범위를 문서화한다.

이번 문서는 cleanup implementation 문서가 아니다. cleanup 실행 승인 문서도 아니며, 파일 삭제/이동, runtime metadata deletion, runtime attachment deletion을 수행하지 않는다.

## 5. Current State

- UI resource/copy baseline closed.
- latest known full test: PASS 331, recorded in `docs/251_POLICY_CLAIM_FINAL_KOREAN_COPY_STRATEGY_RESULT_REVIEW.md`.
- Scenario 9 cleanup remains deferred.
- `data/claimdoc/`는 known local real-document artifact로 보호한다.
- DB/SQLite/OCR/repository implementation은 여전히 미승인 상태다.
- UI redesign/product UI shell 작업은 이 문서 범위가 아니다.

## 6. Recent Commit Flow 확인

다음 흐름을 read-only로 확인했다.

- `1fd475a refactor(familyclaimref): apply approved korean resource copy`
- `2350160 docs(familyclaimref): approve final korean copy table`
- `1036fba docs(familyclaimref): draft final korean copy candidate table`
- `01aeffe docs(familyclaimref): plan final korean copy strategy`
- `a8a2407 refactor(familyclaimref): extract viewmodel runtime messages`
- `ee0c7b5 docs(familyclaimref): plan viewmodel runtime message extraction`
- `687bc26 docs(familyclaimref): consolidate ui resource current state`
- `26b031f refactor(familyclaimref): extract management static xaml strings`
- `a570d9a refactor(familyclaimref): extract document registration static xaml strings`
- `478e6cd refactor(familyclaimref): extract validation harness pilot strings`
- `14f0541 feat(familyclaimref): add ui resource infrastructure`

## 7. 포함 후보

- runtime artifact class 분류
- cleanup allowed/not-allowed boundary
- runtime evidence retention policy
- future cleanup dry-run 조건
- future exact cleanup implementation 조건
- cleanup 실행 전후 검증 기준

## 8. 제외 범위

- cleanup 실행
- 파일 삭제/이동
- directory deletion
- runtime metadata deletion
- runtime attachment deletion
- root artifact deletion
- `data/claimdoc/` 접근
- DB/SQLite/OCR/repository
- app launch/manual workflow
- OpenFileDialog
- screenshot/visual automation
- UI redesign/product UI shell
- deferred diagnostic summary format extraction
- `policy:{policyId}; document:{documentId}` extraction
- `claim:{claimId}; document:{documentId}` extraction

## 9. Read-Only Evidence Baseline

- `docs/251_POLICY_CLAIM_FINAL_KOREAN_COPY_STRATEGY_RESULT_REVIEW.md`는 latest UI copy implementation과 full test PASS 331을 기록한다.
- `docs/246_POLICY_CLAIM_VIEWMODEL_RUNTIME_MESSAGE_EXTRACTION_RESULT_REVIEW.md`는 runtime message extraction 결과와 cleanup/runtime deletion 미실행을 기록한다.
- `docs/241_POLICY_CLAIM_UI_RESOURCE_CURRENT_STATE_REVIEW.md`는 UI resource current-state 기준과 `data/claimdoc` 접근 없음, cleanup 없음, DB/SQLite/OCR/repository 없음 기준을 기록한다.
- `git grep -n -- "Scenario 9" docs`에서 Scenario 9 isolated runtime manual validation과 artifact cleanup 관련 기존 문서가 확인되었다.
- `git grep -n -- "isolated runtime" docs`에서 RuntimeRootProvider와 isolated runtime validation 흐름이 확인되었다.
- `git grep -n -- "runtime metadata" docs`와 `git grep -n -- "runtime attachment" docs`에서 기존 evidence preservation 및 cleanup DEFER 흐름이 확인되었다.
- `git grep -n -- "data/claimdoc" docs`에서 `data/claimdoc/` 보호 정책이 확인되었다.
- source/test read-only grep에서 `RuntimeRootProvider`, `RuntimeRootPaths`, `attachments`, `metadata`, `runtime_test_document` snapshot 검증이 확인되었다.

## 10. Policy Question

- 현재 cleanup을 계속 DEFER할지 결정해야 한다.
- future cleanup을 승인하려면 어떤 evidence와 exact path가 필요한지 결정해야 한다.
- cleanup을 수행한다면 dry-run report와 exact path approval이 선행되어야 한다.
- `data/claimdoc/`는 cleanup 후보가 아니라 영구 보호 대상인지 재확인해야 한다.

## 11. Scope Judgment

현재 단계의 판단은 다음과 같다.

- Scenario 9 isolated runtime artifact cleanup은 계속 DEFER한다.
- 이 문서는 future cleanup의 정책과 검증 조건만 정리한다.
- cleanup implementation은 별도 exact cleanup batch와 별도 사용자 승인 없이는 시작하지 않는다.

## 12. Final Marker

POLICY_CLAIM_SCENARIO9_CLEANUP_POLICY_REVIEW_SCOPE_READY
