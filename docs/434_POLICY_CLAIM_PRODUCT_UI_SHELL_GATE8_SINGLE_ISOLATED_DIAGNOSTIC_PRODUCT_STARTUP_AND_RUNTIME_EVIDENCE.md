# Gate 8 Single Isolated Diagnostic Product Startup and Runtime Evidence

## A. Status

- Status: `HOLD`
- Marker:
  `POLICY_CLAIM_PRODUCT_UI_SHELL_GATE8_SINGLE_ISOLATED_DIAGNOSTIC_PRODUCT_STARTUP_HOLD_RUNTIME_EVIDENCE_OR_ENVIRONMENT_FINDING`
- Product runtime evidence: `NOT_EXECUTED`
- Gate 8 closure: `HOLD_RUNTIME_REVIEW_REQUIRED`
- Deployment/production readiness: `NOT_AUTHORIZED`
- Stage/commit/push: `0/0/0`

이번 배치는 승인된 harness를 정확히 한 번 호출했지만, 권한 상승 실행
컨텍스트에서 Git repository ownership preflight가 실패하여 Product
`Process.Start` 이전에 중단되었다. 지시된 no-retry 계약에 따라 harness 수정,
재실행, 우회 실행을 수행하지 않았다.

## B. Starting Baseline

| Item | Observed |
|---|---|
| Branch | `main` |
| HEAD | `aecf7edfd43b4124ec5ff17d35687020cf4c0d90` |
| Initial tracked/staged/untracked | `29/0/25` |
| Initial status entries | `54` |
| Product process candidates | `0` |
| Product nonzero main-window handles | `0` |
| `docs/434` preexistence | `0` |
| Evidence directory preexistence | `0` |
| Existing 54-path set SHA-256 | `08c04ac187923eb2b983dc94412c26705aff10e16b6f11627419a4bf23f98aba` |

The 54-path set was checked again after the failed invocation. The path count and
LF-terminated sorted path-set SHA-256 remained unchanged.

## C. Protected Identity Review

| Artifact | Bytes | Lines | SHA-256 | Result |
|---|---:|---:|---|---|
| `docs/433_POLICY_CLAIM_PRODUCT_UI_SHELL_GATE8_STARTUP_INSTRUMENTATION_REPAIR_INDEPENDENT_RECHECK.md` | 40,717 | 672 | `f049843a8a31d0c5211db86e7bb789a9d15bec47ec1b1a33c53efc0b2f07aad7` | PASS |
| `StartupDiagnosticSession.cs` | 30,465 | 1,022 | `c42a35f771ac22d1f17543af86124df4eb7f630094dcdb977d3bc2fadc129093` | PASS |
| `StartupDiagnosticSessionTests.cs` | 29,045 | 876 | `5264fad93fa98c7977c41137acddb7113952d77a5a1e4da798c32929310544c9` | PASS |
| `AppStartupObservabilityContractTests.cs` | 11,599 | 342 | `df3072a32a8316d220041a79452931514da7bccc49ce1dd0502dde359f2c4030` | PASS |
| `FamilyClaimRef.App.exe` | 162,816 | N/A | `bb3de775939bbeb06aa9abe42e9e93cee51881084b3e6f20e7293a2d23300c39` | PASS |
| `FamilyClaimRef.App.dll` | 318,976 | N/A | `935c5c3c19db57deff3e109f912d4d3b48b80dfd9e81bb129130ea869bb4896c` | PASS |

Source, test, XAML, resource, project, generated binary contents were not
modified by this batch.

## D. Source-Derived Launch Contract

The pre-launch read-only source review established the following child-only
environment contract:

| Purpose | Environment variable | Source owner |
|---|---|---|
| Enable diagnostics | `FAMILYCLAIMREF_ENABLE_STARTUP_DIAGNOSTICS` | `StartupDiagnosticSession.CreateFromEnvironment` |
| Diagnostic root | `FAMILYCLAIMREF_STARTUP_DIAGNOSTIC_ROOT` | `StartupDiagnosticSession.CreateFromEnvironment` |
| Enable isolated runtime override | `FAMILYCLAIMREF_ENABLE_DEV_RUNTIME_ROOT_OVERRIDE` | `EnvironmentRuntimeRootProvider` |
| Isolated runtime root | `FAMILYCLAIMREF_RUNTIME_ROOT` | `EnvironmentRuntimeRootProvider` |

- Enable values use ordinal exact `1`.
- Runtime override requires an absolute path.
- No Product command-line argument was configured.
- Fixed executable:
  `app/FamilyClaimRef.App/bin/Debug/net10.0-windows/FamilyClaimRef.App.exe`
- Fixed working directory:
  `app/FamilyClaimRef.App/bin/Debug/net10.0-windows`
- The child-only runtime override would prevent use of the production/default
  runtime root if Product startup were reached.

## E. Source-Derived Expected Milestone Matrix

The harness was prepared to require the following ordered subsequence while
allowing only the reviewed owner/milestone/phase/result/method allowlists.

| Order | Owner | Milestone | Phase | Result |
|---:|---|---|---|---|
| 1 | `App` | `app_constructor.body_enter` | `enter` | `started` |
| 2 | `App` | `app_constructor.body_ready` | `return` | `completed` |
| 3 | `App` | `app_on_startup.enter` | `enter` | `started` |
| 4 | `App` | `base_on_startup` | `begin` | `started` |
| 5 | `App` | `base_on_startup` | `end` | `completed` |
| 6 | `App` | `startup_mode.selection` | `decision` | `default` |
| 7 | `App` | `app_services_create_default` | `begin` | `started` |
| 8 | `App` | `app_services_create_default` | `end` | `completed` |
| 9 | `App` | `product_shell_window.construction` | `begin` | `started` |
| 10 | `ProductShellWindow` | `product_shell_window.constructor` | `enter` | `started` |
| 11 | `ProductShellWindow` | `product_shell_window.initialize_component` | `begin` | `started` |
| 12 | `ProductShellWindow` | `product_shell_window.initialize_component` | `end` | `completed` |
| 13 | `ProductShellWindow` | `product_shell_window.data_context_assignment` | `begin` | `started` |
| 14 | `ProductShellWindow` | `product_shell_window.data_context_assignment` | `end` | `completed` |
| 15 | `ProductShellWindow` | `product_shell_window.constructor` | `return` | `completed` |
| 16 | `App` | `product_shell_window.construction` | `end` | `completed` |
| 17 | `App` | `application.main_window_assignment` | `end` | `completed` |
| 18 | `App` | `product_shell_window.show` | `begin` | `started` |
| 19 | `ProductShellWindow` | `product_shell_window.loaded` | `event` | `observed` |
| 20 | `App` | `product_shell_window.show` | `return` | `completed` |
| 21 | `ProductShellWindow` | `product_shell_window.dispatcher_callback` | `callback` | `scheduled` |
| 22 | `ProductShellWindow` | `product_shell_window.content_rendered` | `event` | `observed` |
| 23 | `ProductShellWindow` | `product_shell_window.dispatcher_callback` | `callback` | `executed` |
| 24 | `ProductShellWindow` | `product_shell_window.closed` | `event` | `observed` |
| 25 | `App` | `app_on_exit` | `enter` | `started` |
| 26 | `App` | `app_on_exit` | `return` | `completed` |

No actual milestone comparison was possible because Product startup was not
reached.

## F. Harness Static Review

Created harness:

`docs/evidence/434_GATE8_SINGLE_DIAGNOSTIC_STARTUP/diagnostic_run_harness.ps1`

| Check | Result |
|---|---|
| Bytes / lines | `31,210 / 842` |
| SHA-256 | `c02707d1e9240eae9afeb3f5b235248b8751144215c9d9c04919c501ec51db08` |
| PowerShell parser errors | `0` |
| Actual `[Diagnostics.Process]::Start(...)` call sites | `1` |
| `Start-Process` commands | `0` |
| Captured-process fallback `.Kill()` implementations | `1` |
| Broad `Stop-Process` / `taskkill` / process-name kill | `0` |
| `Remove-Item` implementations | `1`, exact owned root only |
| Product argument configuration | `0` |
| `dotnet run/build/test` | `0` |
| Git mutation commands | `0` |
| Local-profile path literals | `0` |
| Required child environment variables | `4/4` |
| Win32 window PID ownership check | present |
| Win32 window class read | present |
| Evidence copy byte/hash equality gate | present |
| Unrelated TEMP sibling delta gate | present |
| Trailing whitespace / merge markers | `0/0` |

The harness excludes its own approved evidence paths when rechecking the
existing 54-path baseline. Before invocation that filtered baseline was
`54` paths with the expected path-set SHA-256.

## G. Single Invocation Result

| Item | Actual |
|---|---|
| Harness invocation count | `1` |
| Product start attempt count | `0` |
| Product process created count | `0` |
| Second Product start attempt count | `0` |
| Captured PID | `null` |
| Main window observed | `false` |
| Diagnostic log observed | `false` |
| Graceful close requests | `0` |
| Fallback termination used | `false` |
| Exit code | `null` |

The elevated harness process reached its first branch/HEAD Git preflight.
Git rejected repository access with `detected dubious ownership`. The harness
therefore raised `Branch or HEAD baseline mismatch.` and stopped.

Control-flow review confirms this failure occurred before:

1. cryptographic run ID and nonce generation,
2. diagnostic/runtime TEMP leaf creation,
3. child environment assignment,
4. `[Diagnostics.Process]::Start(...)`.

No command-local or global `safe.directory` mutation was attempted. The harness
was not edited or invoked again after this failure.

## H. Runtime Evidence and Cleanup

| Evidence or state | Result |
|---|---|
| `runtime_observation.json` | not created |
| `startup.ndjson` TEMP original | not created |
| repository `startup.ndjson` copy | not created |
| Diagnostic run root | not created |
| Isolated runtime root | not created |
| Cleanup execution | not required, not executed |
| Product process after | `0` |
| Product nonzero main-window handles after | `0` |
| Production/default runtime root access | `0`, Product not started |
| `data/claimdoc` access | `0` |

The absence of runtime evidence is not converted into PASS evidence. No
`runtime_observation.json` was manually fabricated.

## I. Repository Verification

Immediately after the failed invocation and before this document was created:

- tracked/staged/untracked: `29/0/26`
- status entries: `55`
- the one new path was the approved harness
- existing baseline paths: `54`
- existing path-set SHA-256:
  `08c04ac187923eb2b983dc94412c26705aff10e16b6f11627419a4bf23f98aba`
- deletion/rename: `0/0`
- protected identities: unchanged
- generated EXE/DLL identities: unchanged
- `git diff --check`: exit code `0`
- build/test: not run
- App/file picker/workflow: not run
- stage/commit/push: `0/0/0`

`git diff --check` emitted the repository's existing LF-to-CRLF conversion
warnings but no whitespace error.

Final state after creating this HOLD document:

- tracked/staged/untracked: `29/0/27`
- status entries: `56`
- approved new repository paths: `2`
- existing baseline paths: `54`, unchanged path set
- `runtime_observation.json`: absent
- repository `startup.ndjson`: absent
- Product process/nonzero window handles: `0/0`

## J. Findings

| Severity | Count | Finding |
|---|---:|---|
| Blocking | 1 | The elevated execution context could not read Git baseline state because repository ownership was not trusted in that context. |
| Major | 0 | None |
| Minor | 0 | None |

This is an execution-environment finding. It is not evidence that Product
startup succeeds or fails.

## K. Disposition

- Current diagnostic startup: `NOT_EXECUTED`
- Current App/ProductShell runtime lifecycle: `NOT_OBSERVED`
- F-05 actual runtime evidence gap: `OPEN`
- `docs/428` historical failure cause: `UNRESOLVED_HISTORICAL`
- Historical defect fixed: `NOT_PROVEN`
- Gate 8 final closure: `HOLD_RUNTIME_REVIEW_REQUIRED`
- Deployment/production readiness: `NOT_AUTHORIZED`
- Documentation/source commit: `NOT_AUTHORIZED`

Exact result marker:

`POLICY_CLAIM_PRODUCT_UI_SHELL_GATE8_SINGLE_ISOLATED_DIAGNOSTIC_PRODUCT_STARTUP_HOLD_RUNTIME_EVIDENCE_OR_ENVIRONMENT_FINDING`

## L. Exact Created Scope

- `docs/434_POLICY_CLAIM_PRODUCT_UI_SHELL_GATE8_SINGLE_ISOLATED_DIAGNOSTIC_PRODUCT_STARTUP_AND_RUNTIME_EVIDENCE.md`
- `docs/evidence/434_GATE8_SINGLE_DIAGNOSTIC_STARTUP/diagnostic_run_harness.ps1`

Not created:

- `docs/evidence/434_GATE8_SINGLE_DIAGNOSTIC_STARTUP/runtime_observation.json`
- `docs/evidence/434_GATE8_SINGLE_DIAGNOSTIC_STARTUP/startup.ndjson`

No existing repository file was modified.

## M. Next Action

No additional Product startup is authorized by this batch.

A separately approved future batch may prepare a fresh one-shot harness whose
Git preflight uses command-local
`-c safe.directory=C:/EtcProject/FamilyClaimRef` consistently in the elevated
context. Global Git configuration must remain unchanged. That future batch
must repeat all one-start, isolated-root, privacy, exact cleanup, and
no-retry gates.
