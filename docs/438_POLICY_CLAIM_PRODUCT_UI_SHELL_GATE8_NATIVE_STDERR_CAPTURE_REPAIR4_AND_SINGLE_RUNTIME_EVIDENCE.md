# Gate 8 Native Stderr Capture Repair4 and Single Runtime Evidence

## A. Status

`POLICY_CLAIM_PRODUCT_UI_SHELL_GATE8_NATIVE_STDERR_CAPTURE_AND_DURABLE_OBSERVATION_REPAIR4_AND_SINGLE_DIAGNOSTIC_PRODUCT_STARTUP_PASS_CURRENT_RUNTIME_OBSERVED_GATE8_CLOSURE_PENDING_INDEPENDENT_RECHECK`

- Repair4 native Git capture: `PASS`
- `GitAuditProbeOnly`: `PASS`
- Repair4 Runtime: `PASS`
- Current Product runtime observation: `OBSERVED_PASS`
- F-05 current evidence gap: `CLOSED_FOR_REPAIR4_RUN`
- Gate 8 closure: `PENDING_INDEPENDENT_RECHECK`
- Historical defect fixed: `NOT_PROVEN`
- Deployment/production readiness: `NOT_AUTHORIZED`
- Stage/commit/push: `NOT_AUTHORIZED`

## B. Scope

이번 배치는 Repair3에서 확인된 native stderr handling과 runtime observation durability만 보정했다.

생성된 exact 경로:

1. `docs/438_POLICY_CLAIM_PRODUCT_UI_SHELL_GATE8_NATIVE_STDERR_CAPTURE_REPAIR4_AND_SINGLE_RUNTIME_EVIDENCE.md`
2. `docs/evidence/438_GATE8_NATIVE_STDERR_CAPTURE_REPAIR4_SINGLE_DIAGNOSTIC_STARTUP/diagnostic_run_harness_repair4.ps1`
3. `docs/evidence/438_GATE8_NATIVE_STDERR_CAPTURE_REPAIR4_SINGLE_DIAGNOSTIC_STARTUP/runtime_observation.json`
4. `docs/evidence/438_GATE8_NATIVE_STDERR_CAPTURE_REPAIR4_SINGLE_DIAGNOSTIC_STARTUP/startup.ndjson`

수정하지 않은 범위:

- docs/433~437와 Original/Repair1/Repair2/Repair3 harness
- Product source/test/XAML/resource/project/binary
- startup instrumentation
- production/default runtime data

실행하지 않은 범위:

- build/test
- file picker와 document registration workflow
- stage/commit/push
- Product retry 또는 두 번째 Repair4 시작

## C. Evidence Classification

| Classification | Meaning |
|---|---|
| Confirmed by artifact | 저장된 파일의 bytes, SHA-256, JSON/NDJSON 내용으로 확인 |
| Harness self-observed | Repair4 process 내부에서 관찰하고 durable observation에 기록 |
| Independently rechecked | harness 종료 후 별도 read-only 명령으로 재확인 |
| Unknown | 현재 artifact만으로 원인을 확정할 수 없음 |

## D. Starting 63-Path Baseline

| Item | Actual | Evidence | Result |
|---|---:|---|---|
| Branch | `main` | independently rechecked | PASS |
| HEAD | `aecf7edfd43b4124ec5ff17d35687020cf4c0d90` | independently rechecked | PASS |
| Tracked/staged/untracked | `29/0/34` | independently rechecked | PASS |
| Status entries | `63` | independently rechecked | PASS |
| Product process/window | `0/0` | independently rechecked | PASS |
| Protected path count | `63` | independently rechecked | PASS |
| Path-set SHA-256 | `5c46306e51fd39ffdc452ece07e3543b56e5e46f644513c5e340836bc03d1a1d` | independently rechecked | PASS |
| T0 aggregate SHA-256 | `611df5c4e49e99b7c37e3e0a777b88635524062bec738ca8a3140be8a35848a3` | independently rechecked | PASS |

Repair4 harness 생성 직후 상태는 `29/0/35`, status entries `64`, Repair4 신규 경로 `1`이었다.

## E. Repair3 Identity Gate

| Artifact | Bytes | Lines/records | SHA-256 | Result |
|---|---:|---:|---|---|
| docs/437 | 12297 | 291 LF | `2ec9e4f242c7889a11608d1500959526e34c9296dd8a6c3acf64ded44fca5fbb` | PASS |
| Repair3 harness | 49655 | 1271 | `75604046cfb904303bde292a0ec482c50dd3caa960af7d9df67d5d4cae824818` | PASS |
| Repair3 `startup.ndjson` | 7643 | 26 | `20e5dd5a0612b441643e0d5a6ae95eb5da22a54c35e489c43491778ffcfd3c31` | PASS |

docs/437의 `291`은 LF count다. PowerShell `Get-Content` logical count는 `286`이지만 bytes와 SHA-256은 authoritative identity와 일치한다.

기존 63-path aggregate가 exact match이므로 docs/433~436, prior harness, source/test/binary를 포함한 protected content도 불변이다.

## F. Repair3 Hold Cause

Repair3 Product와 cleanup은 완료됐지만 final `git diff --check`에서 발생한 기존 LF/CRLF warning 29건이 Windows PowerShell native stderr error record로 승격되어 observation write 전에 종료됐다.

Repair4가 확정한 범위:

- Git exit code와 stdout/stderr는 분리해야 한다.
- `git diff --check` exit `0`, whitespace output `0`, accepted LF/CRLF warning `29`는 PASS다.
- unclassified stderr가 있을 때만 HOLD한다.

확정하지 않은 범위:

- Repair3 이전 historical failure의 단일 원인
- docs/428 historical cause

Historical representation은 계속 `UNRESOLVED_HISTORICAL`이며 historical defect fixed는 `NOT_PROVEN`이다.

## G. Repair3 to Repair4 Delta

Repair4 harness:

- bytes: `66876`
- lines: `1765`
- SHA-256: `28600a785e74c7f998a8c6df00fbcdfac265c09c72c166edf091863206833bf2`

허용 delta:

- evidence path `437`에서 `438`로 변경
- starting protected set `60`에서 current exact `63`으로 변경
- docs/437, Repair3 harness, Repair3 log identity gate 추가
- `System.Diagnostics.Process` 기반 native Git helper 추가
- stdout/stderr/exit code 분리와 warning count classification
- `-GitAuditProbeOnly` mode
- Product 이전 runtime state 초기화
- outer `try/catch/finally`
- exact-owner final cleanup
- terminal phase와 fixed error code
- atomic UTF-8 no-BOM observation write
- Repair4와 누적 invocation/Product count

정적 검토:

| Check | Actual | Result |
|---|---:|---|
| PowerShell parser errors | 0 | PASS |
| Product `[Diagnostics.Process]::Start` sites | 1 | PASS |
| Native Git helper implementations | 1 | PASS |
| Native Git process start sites | 1 | PASS |
| Canonical snapshot implementations | 1 | PASS |
| `GitAuditProbeOnly` branches | 1 | PASS |
| Atomic observation writers | 1 | PASS |
| Raw `& git` invocation | 0 | PASS |
| `Start-Process`/`pwsh` | `0/0` | PASS |
| `safe.directory=*` | 0 | PASS |
| Persistent Git config mutation | 0 | PASS |
| Observation required fields | `40/40` | PASS |
| Trailing whitespace/merge markers | `0/0` | PASS |
| Local-profile literal | 0 | PASS |

Product executable, working directory, arguments, child environment, window/PID contract, lifecycle validation, graceful close, captured-PID fallback 및 exact-owner cleanup 의미는 유지됐다.

## H. Native Git Contract

단일 `Invoke-RepositoryGitRead` helper는 다음을 적용한다.

- `System.Diagnostics.Process`
- `UseShellExecute=false`
- `CreateNoWindow=true`
- stdout/stderr separate capture
- independent exit code
- command-local exact repository `safe.directory`
- `core.quotepath=false`
- raw stderr를 evidence에 저장하지 않고 accepted/unclassified count만 기록

`git diff --check` PASS 기준:

- exit code `0`
- stdout whitespace-error line `0`
- accepted LF/CRLF warning 이외 stderr `0`

## I. GitAuditProbeOnly

elevated Windows PowerShell 5.1에서 정확히 한 번 실행했다.

| Item | Expected | Actual | Result |
|---|---:|---:|---|
| Raw status count | 64 | 64 | PASS |
| Repair4 harness exclusion | 1 | 1 | PASS |
| Protected path count | 63 | 63 | PASS |
| Path-set SHA-256 | expected | exact match | PASS |
| T0 aggregate SHA-256 | expected | exact match | PASS |
| Git diff exit code | 0 | 0 | PASS |
| Whitespace-error count | 0 | 0 | PASS |
| Accepted warning count | record | 29 | PASS |
| Unclassified stderr count | 0 | 0 | PASS |
| Native Git invocation count | record | 7 | PASS |
| Product process before/after | 0/0 | 0/0 | PASS |
| Product start attempt | 0 | 0 | PASS |
| Diagnostic/runtime root creation | 0/0 | 0/0 | PASS |
| Repository mismatch | 0 | 0 | PASS |
| Git config mutation | 0 | 0 | PASS |

Probe output의 `fixedErrorCode`는 `null`, `pass`는 `true`였다.

## J. Invocation and Product Counts

| Item | Count |
|---|---:|
| Original harness invocations | 1 |
| Repair1 harness invocations | 1 |
| Repair2 harness invocations | 1 |
| Repair3 `BaselineProbeOnly` invocations | 1 |
| Repair3 Runtime invocations | 1 |
| Repair4 `GitAuditProbeOnly` invocations | 1 |
| Repair4 Runtime invocations | 1 |
| Prior cumulative Product starts | 1 |
| Repair4 Product starts | 1 |
| Cumulative Product starts | 2 |
| Repair4 retry/second start | 0 |

Repair4 runtime은 한 번만 호출했고 Product를 다시 시작하지 않았다.

## K. Repair4 Artifact Identities

| Artifact | Bytes | Lines/records | SHA-256 | Evidence |
|---|---:|---:|---|---|
| Repair4 harness | 66876 | 1765 | `28600a785e74c7f998a8c6df00fbcdfac265c09c72c166edf091863206833bf2` | confirmed by artifact |
| `runtime_observation.json` | 9644 | 172 logical | `e2c263b18e50485ead46dbd029baf4e84ae74d34e2440c40c67491fc6478d7b8` | confirmed by artifact |
| `startup.ndjson` | 7618 | 26 | `a86ddd6609fa9ef7922bccf5475eeea06201b0ce72b1e9a21ec5d025f7665141` | confirmed by artifact |

`runtime_observation.json.tmp` residue는 `0`이다.

## L. Durable Runtime Observation

| Field | Value | Evidence |
|---|---|---|
| `terminalPhase` | `COMPLETED` | confirmed by artifact |
| `pass` | `true` | confirmed by artifact |
| `fixedErrorCode` | `null` | confirmed by artifact |
| `observationWriteAttempted` | `true` | confirmed by artifact |
| `capturedProductPid` | `8488` | harness self-observed |
| `firstWindowHandle` | `11013844` | harness self-observed |
| `windowOwnedByCapturedPid` | `true` | harness self-observed |
| `gracefulCloseRequestCount` | `1` | harness self-observed |
| `gracefulCloseSucceeded` | `true` | harness self-observed |
| `fallbackTerminationCount` | `0` | harness self-observed |
| `productExitCode` | `0` | harness self-observed |
| `startupRecordCount` | `26` | confirmed by artifact |
| `gitProbeExitCode/warning/unclassified` | `0/29/0` | confirmed by artifact |
| `runtimeGitAuditExitCode/warning/unclassified` | `0/29/0` | confirmed by artifact |
| `runtimeGitWarningCountMatchedProbe` | `true` | confirmed by artifact |
| `diagnostic/runtime/unrelated residue` | `0/0/0` | harness self-observed |
| `safeDirectoryExactRepositoryMatch` | `true` | confirmed by artifact |
| `safeDirectoryWildcardUsed` | `false` | confirmed by artifact |
| `gitConfigMutationCount` | `0` | confirmed by artifact |
| `rawRepositoryGitInvocationCount` | `0` | confirmed by artifact |

## M. Lifecycle and Privacy

`startup.ndjson` 독립 재검증:

- JSON records: `26`
- parse failures: `0`
- sequence: `1..26`, contiguous
- `product_shell_window.content_rendered`: `1`
- `product_shell_window.closed`: `1`
- `app_on_exit` enter/return: `1/1`
- privacy findings: `0`
- local-profile findings: `0`
- observation/log identity match: `PASS`

Harness self-observed validation:

- expected ordered subsequence: `PASS`
- JSON/sequence/size/privacy: `PASS/PASS/PASS/PASS`
- allowlist/privacy finding count: `0/0`
- native top-level window observed: `true`
- window captured-PID ownership: `true`
- termination mode: `graceful_close`
- fallback termination: `0`
- Product exit code: `0`

## N. Cleanup and Independent Postflight

독립 재검증:

| Item | Actual | Result |
|---|---:|---|
| Product process/nonzero window after | `0/0` | PASS |
| Repair4 diagnostic/runtime leaf residue | `0/0` | PASS |
| Repair4 owner-token residue | `0` | PASS |
| Unrelated TEMP entries | `1`, unchanged by self-observation | PASS |
| `git diff --check` exit code | `0` | PASS |
| Accepted LF/CRLF warnings | `29` | PASS |
| Other diff output | `0` | PASS |
| Existing 63-path count | `63` | PASS |
| Existing 63 path-set SHA-256 | `5c46306e51fd39ffdc452ece07e3543b56e5e46f644513c5e340836bc03d1a1d` | PASS |
| Existing 63 T0 aggregate SHA-256 | `611df5c4e49e99b7c37e3e0a777b88635524062bec738ca8a3140be8a35848a3` | PASS |

production/default runtime root access/deletion은 `0/0`이며 broad cleanup은 수행하지 않았다.

## O. Final Git Scope

docs/438 생성 직전:

- tracked/staged/untracked: `29/0/37`
- status entries: `66`
- Repair4 paths: exact `3`

docs/438 생성 후 기대 및 최종 검증 대상:

- tracked/staged/untracked: `29/0/38`
- status entries: `67`
- Repair4 paths: exact `4`
- staged: `0`
- deletion/rename: `0/0`
- Product source/test/XAML/resource/project delta introduced by Repair4: `0/0/0/0/0`
- stage/commit/push: `0/0/0`

## P. Findings

| Severity | Count | Finding |
|---|---:|---|
| Blocking | 0 | 없음 |
| Major | 0 | 없음 |
| Minor | 0 | 없음 |

## Q. Decision

`POLICY_CLAIM_PRODUCT_UI_SHELL_GATE8_NATIVE_STDERR_CAPTURE_AND_DURABLE_OBSERVATION_REPAIR4_AND_SINGLE_DIAGNOSTIC_PRODUCT_STARTUP_PASS_CURRENT_RUNTIME_OBSERVED_GATE8_CLOSURE_PENDING_INDEPENDENT_RECHECK`

- F-05 current evidence gap: `CLOSED_FOR_REPAIR4_RUN`
- Current App/ProductShell lifecycle: `EXECUTED_AND_OBSERVED`
- Durable runtime observation: `PASS`
- docs/428 historical cause: `UNRESOLVED_HISTORICAL`
- Historical defect fixed: `NOT_PROVEN`
- Gate 8 closure: `PENDING_INDEPENDENT_RECHECK`
- Deployment/production readiness: `NOT_AUTHORIZED`
- Stage/commit: `NOT_AUTHORIZED`

다음 행동은 Product 재실행이 아닌 read-only independent recheck다. independent recheck PASS 전에는 Gate 8 closure, instrumentation 제거, deployment, stage/commit을 진행하지 않는다.
