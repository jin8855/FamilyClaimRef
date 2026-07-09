# Policy Claim Scenario 9 Runtime Artifact Cleanup Policy

## 1. Status

RUNTIME_ARTIFACT_CLEANUP_POLICY_ONLY

## 2. Marker

POLICY_CLAIM_SCENARIO9_RUNTIME_ARTIFACT_CLEANUP_POLICY_PLANNED

## 3. 기준 Commit

- `1fd475a refactor(familyclaimref): apply approved korean resource copy`

## 4. Artifact Classification

| Artifact class | Example path or pattern | Current evidence | Cleanup decision | Reason | Future handling |
|---|---|---|---|---|---|
| project root attachments/ | `attachments/` | project root files count 0 | Not found | 현재 project root artifact가 없다. | 발견되더라도 future exact cleanup batch에서만 검토한다. |
| project root data/local/ | `data/local/` | project root files count 0 | Not found | 현재 project root local metadata artifact가 없다. | 발견되더라도 future exact cleanup batch에서만 검토한다. |
| project root runtime_test_document.* | `runtime_test_document.*` | project root files count 0 | Not found | 현재 root synthetic test document가 없다. | 발견되더라도 future exact cleanup batch에서만 검토한다. |
| DB/SQLite unexpected root files | `*.db`, `*.sqlite`, `*.sqlite3` | project root files count 0 | Not found | DB/SQLite implementation은 미승인이고 root file도 없다. | future DB decision 전까지 cleanup implementation과 분리한다. |
| isolated runtime root synthetic metadata | isolated runtime root `data/local` equivalent | docs record prior isolated runtime validation evidence; path was not inspected in this batch | Cleanup allowed only by future exact cleanup batch | evidence preservation이 우선이며 이번 batch는 cleanup 실행 문서가 아니다. | dry-run report, exact path list, user approval 후에만 검토한다. |
| isolated runtime root synthetic attachments | isolated runtime root `attachments` equivalent | docs record prior copied attachment evidence; path was not inspected in this batch | Cleanup allowed only by future exact cleanup batch | runtime attachment evidence는 validation result review 전후 보존 대상이다. | evidence가 commit/review된 뒤 exact path approval이 있어야 한다. |
| test-created temp/runtime artifacts | test-owned temp roots and `runtime_test_document.*` snapshots | source/test read-only grep confirms temp roots and project root snapshot checks | Cleanup allowed only by future exact cleanup batch | test cleanup is usually test-owned, but unexpected residue must be exact-path reviewed. | dry-run에서 test-owned residue와 project root residue를 분리한다. |
| data/claimdoc/ | `data/claimdoc/` | ignore check confirms protected local path; contents were not inspected | Never cleanup | known local real-document artifact이며 read/list/use/delete/move/stage/commit 금지 대상이다. | cleanup candidate list에 절대 포함하지 않는다. |
| docs/nightwork_*/ | `docs/nightwork_*/` | ignore check confirms local-only instruction pack pattern | Keep | operational instruction pack이며 이번 policy review에서는 내부 접근하지 않는다. | local-only 유지 또는 별도 사용자 결정이 필요하다. |
| source-controlled docs | `docs/*.md` tracked files | source-controlled evidence chain | Never cleanup | 문서 evidence chain이며 cleanup 대상이 아니다. | 변경은 별도 docs batch와 exact file list로만 수행한다. |
| source-controlled app/tests files | `app/`, `tests/` tracked files | source-controlled implementation and tests | Never cleanup | code/test는 cleanup 대상이 아니며 이번 batch에서 수정하지 않는다. | 변경은 별도 implementation/test batch에서만 수행한다. |

## 5. Mandatory Policy

- `data/claimdoc/`는 Never cleanup이다.
- `docs/nightwork_*/`는 ignore 대상이며 이번 cleanup policy review에서는 내부 접근하지 않는다.
- source-controlled docs/app/tests는 cleanup 대상이 아니다.
- isolated runtime root synthetic artifacts는 future exact cleanup batch에서만 cleanup 검토 가능하다.
- root-level unexpected artifacts가 발견되어도 이번 batch에서는 삭제하지 않는다.
- cleanup은 dry-run report 후 exact path approval이 있어야 한다.
- cleanup은 user approval 없이 자동 실행하지 않는다.
- evidence preservation이 필요한 validation artifact는 commit/review 전 삭제하지 않는다.
- recursive delete는 exact path approval 없이 실행하지 않는다.
- `git clean`, `git reset`, `git checkout`은 cleanup command로 사용하지 않는다.

## 6. Count Summary

| Item | Result |
|---|---:|
| project root attachments files | 0 |
| project root data/local files | 0 |
| project root runtime_test_document.* files | 0 |
| DB/SQLite unexpected root files | 0 |

## 7. Ignore Check Summary

- `git check-ignore -v -- data/claimdoc/`: ignored by `.gitignore` rule `/data/claimdoc/`
- `git check-ignore -v -- docs/nightwork_20260706/`: ignored by `.gitignore` rule `/docs/nightwork_*/`

## 8. Current Cleanup Decision

현재 cleanup decision은 다음과 같다.

- project root unexpected artifacts: Not found
- isolated runtime root synthetic artifacts: Cleanup allowed only by future exact cleanup batch
- `data/claimdoc/`: Never cleanup
- source-controlled docs/app/tests: Never cleanup
- docs/nightwork local instruction packs: Keep

## 9. Final Marker

POLICY_CLAIM_SCENARIO9_RUNTIME_ARTIFACT_CLEANUP_POLICY_READY
