# Gate 8 Ordinal Path-Set Baseline Repair3 and Single Runtime Evidence

## A. Status

`POLICY_CLAIM_PRODUCT_UI_SHELL_GATE8_ORDINAL_PATH_SET_BASELINE_REPAIR3_AND_SINGLE_DIAGNOSTIC_PRODUCT_STARTUP_HOLD_RUNTIME_EVIDENCE_OR_ENVIRONMENT_FINDING`

- Repair3 baseline calculator: `PASS`
- `BaselineProbeOnly`: `PASS`
- Repair3 Runtime invocation: `EXECUTED_ONCE`
- Product start attempt: `EXECUTED_ONCE`
- Current diagnostic startup evidence: `PARTIAL_OBSERVED`
- Gate 8 final closure: `PENDING_INDEPENDENT_RECHECK`
- Deployment/production readiness: `NOT_AUTHORIZED`
- Stage/commit/push: `NOT_AUTHORIZED`

## B. Scope

이번 배치는 Repair2 내부 path-set mismatch를 explicit ordinal 계약으로 보정하고, 동일 Repair3 harness의 probe와 runtime 진입점을 각각 한 번만 실행하는 범위였다.

생성된 허용 경로:

- `docs/437_POLICY_CLAIM_PRODUCT_UI_SHELL_GATE8_ORDINAL_PATH_SET_BASELINE_REPAIR3_AND_SINGLE_RUNTIME_EVIDENCE.md`
- `docs/evidence/437_GATE8_ORDINAL_PATH_SET_BASELINE_REPAIR3_SINGLE_DIAGNOSTIC_STARTUP/diagnostic_run_harness_repair3.ps1`
- `docs/evidence/437_GATE8_ORDINAL_PATH_SET_BASELINE_REPAIR3_SINGLE_DIAGNOSTIC_STARTUP/startup.ndjson`

생성되지 않은 허용 경로:

- `docs/evidence/437_GATE8_ORDINAL_PATH_SET_BASELINE_REPAIR3_SINGLE_DIAGNOSTIC_STARTUP/runtime_observation.json`

수정하지 않은 범위:

- docs/433~436와 Original/Repair1/Repair2 harness
- source/test/XAML/resource/project/binary
- production/default runtime data

실행하지 않은 범위:

- build/test
- file picker와 document registration workflow
- stage/commit/push
- Product retry 또는 두 번째 시작

## C. Starting Baseline

| Item | Actual | Result |
|---|---:|---|
| Branch | `main` | PASS |
| HEAD | `aecf7edfd43b4124ec5ff17d35687020cf4c0d90` | PASS |
| Tracked/staged/untracked before Repair3 creation | `29/0/31` | PASS |
| Status entries before Repair3 creation | `60` | PASS |
| Product process/nonzero window | `0/0` | PASS |
| Protected path count | `60` | PASS |
| Protected path-set SHA-256 | `5249606957c72016190f5690923797d255a1f899e316675b2e47122be689d1a9` | PASS |
| Protected T0 aggregate SHA-256 | `96821b4243bdf53e31198e97eca05975eadae81b57ee04e2e23e2f19a09f6a03` | PASS |

Repair3 harness 생성 직후 상태는 `29/0/32`, status entries `61`, Repair3 신규 경로 `1`이었다.

## D. Protected Identity Review

| Artifact | Bytes | Lines | SHA-256 | Result |
|---|---:|---:|---|---|
| docs/433 | 40717 | 650 | `f049843a8a31d0c5211db86e7bb789a9d15bec47ec1b1a33c53efc0b2f07aad7` | PASS |
| docs/434 | 11528 | 264 | `18aed9e914fdf8722c4e127ea84881248cdadc2d25ee26bb78ae52a32cfc51a5` | PASS |
| docs/435 | 11772 | 302 | `a340d759c67b6efd124619a69cc439786902d745f1f09baafaec5b8810885b40` | PASS |
| docs/436 | 12783 | 354 | `dc01682d11c0c737c28c8cadda29cc805ba2320ba534986e60c4dff5bd24cdfb` | PASS |
| Original harness | 31210 | 842 | `c02707d1e9240eae9afeb3f5b235248b8751144215c9d9c04919c501ec51db08` | PASS |
| Repair1 harness | 34797 | 927 | `05db1731da76176cf298c0a3f84e9e327afeaab44eea168a096f37379f451383` | PASS |
| Repair2 harness | 39980 | 1055 | `7914d64ddcc4af4e44ee2017b14ccdb8a587ea38c3291286ab25db6a900a7868` | PASS |
| `StartupDiagnosticSession.cs` | 30465 | 1022 | `c42a35f771ac22d1f17543af86124df4eb7f630094dcdb977d3bc2fadc129093` | PASS |
| `StartupDiagnosticSessionTests.cs` | 29045 | 876 | `5264fad93fa98c7977c41137acddb7113952d77a5a1e4da798c32929310544c9` | PASS |
| `AppStartupObservabilityContractTests.cs` | 11599 | 342 | `df3072a32a8316d220041a79452931514da7bccc49ce1dd0502dde359f2c4030` | PASS |
| `FamilyClaimRef.App.exe` | 162816 | n/a | `bb3de775939bbeb06aa9abe42e9e93cee51881084b3e6f20e7293a2d23300c39` | PASS |
| `FamilyClaimRef.App.dll` | 318976 | n/a | `935c5c3c19db57deff3e109f912d4d3b48b80dfd9e81bb129130ea869bb4896c` | PASS |

docs/433의 authoritative identity는 bytes와 SHA-256이다. PowerShell `Get-Content`가 관찰한 logical line count는 `650`이며, 이전 기록의 `672`와 차이가 있지만 bytes/SHA-256은 일치한다.

## E. Repair2 to Repair3 Review

Repair3 harness identity:

- bytes: `49655`
- lines: `1271`
- SHA-256: `75604046cfb904303bde292a0ec482c50dd3caa960af7d9df67d5d4cae824818`

허용 delta:

- evidence path `436`에서 `437`로 변경
- Repair3 filename과 prior Repair2 identity gate 추가
- protected baseline `58`에서 `60`으로 변경
- strict Git porcelain parser 추가
- canonical path/state/bytes/SHA-256 snapshot 계산 통합
- `[StringComparer]::Ordinal` 정렬
- terminal LF와 UTF-8 without BOM SHA-256 계산
- `-BaselineProbeOnly` 조기 분기
- Repair3 probe/runtime invocation 필드 추가

정적 검토:

| Check | Actual | Result |
|---|---:|---|
| PowerShell parser errors | 0 | PASS |
| Product `Process.Start` call sites | 1 | PASS |
| Canonical calculator implementations | 1 | PASS |
| Old calculator references | 0 | PASS |
| Calculator 내부 `Sort-Object` | 0 | PASS |
| Calculator 내부 wildcard exclusion | 0 | PASS |
| Explicit ordinal comparer | present | PASS |
| UTF-8 without BOM | explicit | PASS |
| Terminal LF | explicit | PASS |
| Trailing whitespace/merge markers | `0/0` | PASS |
| `pwsh` invocation | 0 | PASS |
| Persistent Git config mutation | 0 | PASS |
| `safe.directory=*` | 0 | PASS |

Repair2의 `Sort-Object` 원인은 여전히 historical hypothesis다. Repair3 결과로 확정 가능한 표현은 `UNRESOLVED_ORDERING_OR_CALCULATION_DIFFERENCE`이다.

## F. Canonical Calculator Contract

단일 `Get-CanonicalRepositorySnapshot` 구현이 probe와 runtime에서 공유된다.

1. command-local `safe.directory`와 `core.quotepath=false`로 `git status --short --untracked-files=all`을 읽는다.
2. non-empty 행 길이, 2-character status code, index 2 공백을 검증한다.
3. rename/copy status를 거부한다.
4. index 3부터 행 끝까지를 경로로 사용하고 invalid/rooted/traversal 경로를 거부한다.
5. Repair3 harness exact path 하나만 ordinal equality로 제외한다.
6. path/state/bytes/SHA-256을 메모리에 보관한다.
7. `[Array]::Sort(..., [StringComparer]::Ordinal)`로 정렬한다.
8. path-set과 manifest를 terminal LF로 직렬화한다.
9. `UTF8Encoding(false)` bytes의 SHA-256 lowercase hex를 계산한다.

## G. BaselineProbeOnly Result

elevated Windows PowerShell 5.1에서 정확히 한 번 실행했다.

| Item | Expected | Actual | Result |
|---|---:|---:|---|
| Raw status count | 61 | 61 | PASS |
| Repair3 harness exact exclusion count | 1 | 1 | PASS |
| Protected path count | 60 | 60 | PASS |
| Path-set SHA-256 | expected | `5249606957c72016190f5690923797d255a1f899e316675b2e47122be689d1a9` | PASS |
| T0 aggregate SHA-256 | expected | `96821b4243bdf53e31198e97eca05975eadae81b57ee04e2e23e2f19a09f6a03` | PASS |
| Branch/HEAD | expected | match | PASS |
| Prior identities | PASS | PASS | PASS |
| Executable identity | PASS | PASS | PASS |
| Product process before/after | 0/0 | 0/0 | PASS |
| Diagnostic/runtime root creation | 0/0 | 0/0 | PASS |
| Repository content mismatch | 0 | 0 | PASS |
| Git config mutation | 0 | 0 | PASS |

Probe output의 `fixedErrorCode`는 `null`, `pass`는 `true`였다.

## H. Invocation Counts

| Item | Count |
|---|---:|
| Original harness invocations | 1 |
| Repair1 harness invocations | 1 |
| Repair2 harness invocations | 1 |
| Prior total harness invocations | 3 |
| Repair3 `BaselineProbeOnly` invocations | 1 |
| Repair3 Runtime invocations | 1 |
| Repair3 total process invocations | 2 |
| Prior cumulative Product start attempts | 0 |
| Repair3 Product start attempts | 1 |
| Cumulative Product start attempts | 1 |
| Product retry/second start | 0 |

Runtime은 한 번만 호출했고 실패 후 재호출하지 않았다.

## I. Runtime Evidence

Runtime mode는 격리 root와 opt-in startup diagnostics를 사용해 Product를 한 번 시작했다. `startup.ndjson`이 privacy/size/JSON/sequence/expected-milestone copy gate를 통과해 repository evidence로 복사되었다.

`startup.ndjson`:

- bytes: `7643`
- lines/records: `26/26`
- SHA-256: `20e5dd5a0612b441643e0d5a6ae95eb5da22a54c35e489c43491778ffcfd3c31`
- JSON parse failures: `0`
- sequence: `1..26`, contiguous
- owners: `App`, `ProductShellWindow`
- privacy findings: `0`
- `product_shell_window.content_rendered`: `1`
- `product_shell_window.closed`: `1`
- `app_on_exit` enter/return: present

현재 Product process/nonzero main-window handle은 `0/0`이다.

## J. Runtime Hold Finding

Product 종료와 exact-owner cleanup 뒤 최종 command-local `git diff --check`를 호출하는 과정에서 Git이 기존 working-copy LF/CRLF warning 29건을 stderr로 출력했다.

독립 재확인:

- `git diff --check` exit code: `0`
- warning count: `29`
- non-warning output count: `0`

Repair3 harness는 `$ErrorActionPreference = 'Stop'` 상태에서 native stderr를 terminating error로 처리했다. 따라서 final audit 변수 계산 뒤 `runtime_observation.json`을 쓰기 전에 harness가 exit code `1`로 종료되었다.

확정 가능한 내용:

- Product startup lifecycle log 생성과 privacy gate 통과
- ProductShell content rendered와 closed
- App exit 기록
- 현재 Product process/window `0/0`
- Repair3 diagnostic/runtime leaf residue `0/0`
- 기존 60-path content/state/bytes/SHA-256 불변

확정할 수 없는 필수 PASS evidence:

- captured Product PID
- native top-level window handle과 captured PID ownership
- first-window observation timestamp
- graceful-close request count
- fallback termination 변수
- harness 최종 `pass` 값
- pre/post unrelated TEMP entry set equality

필수 runtime observation이 없으므로 PASS로 승격하지 않는다.

## K. Cleanup and Residue

- Repair3 diagnostic leaf pattern residue: `0`
- Repair3 isolated runtime leaf pattern residue: `0`
- Repair3 owner-token residue: `0`
- broad cleanup: `0`
- production/default runtime root access/deletion: `0/0`
- unrelated TEMP entry: 기존 범위 밖 empty directory `1`, Repair3 leaf pattern 및 owner-token과 불일치

pre-run unrelated TEMP entry set은 `runtime_observation.json` 부재로 외부에서 복원할 수 없으므로 unrelated TEMP delta는 확정하지 않는다.

## L. Final Repository Audit

docs/437 생성 직전:

| Item | Actual |
|---|---:|
| Branch | `main` |
| HEAD | `aecf7edfd43b4124ec5ff17d35687020cf4c0d90` |
| Tracked/staged/untracked | `29/0/33` |
| Status entries | `62` |
| Repair3 evidence paths | `2` |

docs/437 생성 후 최종 상태:

| Item | Actual |
|---|---:|
| Tracked/staged/untracked | `29/0/34` |
| Status entries | `63` |
| Repair3 신규 경로 | `3` |
| `runtime_observation.json` | missing |

Repair3 신규 경로는 결과 문서, Repair3 harness, privacy 검증을 통과한 `startup.ndjson`의 exact 3개다. 그 외 Repair3 경로는 생성되지 않았다.

Repair3 harness와 copied log를 exact 제외한 기존 60개:

- protected count: `60`
- path-set SHA-256: `5249606957c72016190f5690923797d255a1f899e316675b2e47122be689d1a9`
- T0 aggregate SHA-256: `96821b4243bdf53e31198e97eca05975eadae81b57ee04e2e23e2f19a09f6a03`
- starting baseline match: `PASS`

Git:

- `git diff --check`: exit code `0`, LF/CRLF warnings `29`
- source/test/resource/runtime tracked delta introduced by this batch: `0`
- stage/commit/push: `0/0/0`
- deletion/rename: `0/0`

## M. Findings

| Severity | Count | Finding |
|---|---:|---|
| Blocking | 0 | 확인된 Product source/runtime contract defect 없음 |
| Major | 1 | native stderr warning으로 observation write가 차단되어 필수 runtime ownership evidence가 누락됨 |
| Minor | 0 | 없음 |

## N. Decision

`POLICY_CLAIM_PRODUCT_UI_SHELL_GATE8_ORDINAL_PATH_SET_BASELINE_REPAIR3_AND_SINGLE_DIAGNOSTIC_PRODUCT_STARTUP_HOLD_RUNTIME_EVIDENCE_OR_ENVIRONMENT_FINDING`

- BaselineRepair3: `PASS`
- Current Product startup log: `OBSERVED_PASS`
- Complete guarded runtime evidence: `HOLD`
- F-05 current evidence gap: `PARTIALLY_CLOSED`
- docs/428 historical cause: `UNRESOLVED_HISTORICAL`
- Historical defect fixed: `NOT_PROVEN`
- Gate 8 final closure: `PENDING_INDEPENDENT_RECHECK`
- Deployment/production readiness: `NOT_AUTHORIZED`

다음 작업은 별도 승인 전 자동 진행하지 않는다. 이번 Product를 다시 시작하거나 Repair3 runtime을 재호출해서는 안 된다.
