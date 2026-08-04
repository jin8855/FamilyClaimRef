# Gate 8 Windows PowerShell Path Compatibility Repair2 and Single Runtime Evidence

## A. Status

- Status: `HOLD`
- Marker:
  `POLICY_CLAIM_PRODUCT_UI_SHELL_GATE8_WINDOWS_POWERSHELL_PATH_COMPATIBILITY_REPAIR2_AND_SINGLE_DIAGNOSTIC_PRODUCT_STARTUP_HOLD_RUNTIME_EVIDENCE_OR_ENVIRONMENT_FINDING`
- Windows PowerShell compatibility probe: `PASS`
- Repair2 harness pre-launch: `FAILED`
- Product runtime evidence: `NOT_EXECUTED`
- F-05: `OPEN`
- Gate 8 final closure: `NOT_APPROVED`
- Deployment/production readiness: `NOT_AUTHORIZED`
- Stage/commit/push: `0/0/0`

Repair2 removed the incompatible path API and passed the required elevated
Windows PowerShell compatibility probe. The Repair2 harness was then invoked
exactly once, but its internal 58-path baseline gate reported
`Repository path-set baseline mismatch.` before Product startup.

The harness was not modified or invoked again after that failure.

## B. Starting Baseline

| Item | Expected | Observed | Result |
|---|---|---|---|
| Branch | `main` | `main` | PASS |
| HEAD | `aecf7edfd43b4124ec5ff17d35687020cf4c0d90` | same | PASS |
| Tracked/staged/untracked | `29/0/29` | `29/0/29` | PASS |
| Status entries | `58` | `58` | PASS |
| Product process candidates | `0` | `0` | PASS |
| Product nonzero windows | `0` | `0` | PASS |
| `docs/436_*` preexistence | `0` | `0` | PASS |
| Evidence 436 directory preexistence | `0` | `0` | PASS |

Existing protected path set:

- count: `58`
- LF-terminated sorted path-set SHA-256:
  `9b15b71d81a7f76d512b359c46d91223bb9446f70673f6fb5eaa2277d7b2dbbe`

## C. T0 Content Manifest

The T0 manifest was calculated in memory before creating Repair2 files. No
manifest file was written.

Each LF-terminated line used:

`relativePath<TAB>state<TAB>bytes<TAB>sha256`

Rules:

- repository-relative path,
- state is `tracked` or `untracked`,
- path ordering performed before aggregation,
- one line for each existing status path.

T0 result:

| Item | Value |
|---|---|
| Entry count | `58` |
| Aggregate SHA-256 | `8ab50ce002109abb81f841f057a4d2aaff87d90c1d3c96c07041eeefd4590f0f` |

The post-failure external read-only audit reproduced the same 58 entries and
the same T0 aggregate.

## D. Protected Evidence Identity

| Artifact | Bytes | Lines | SHA-256 | Result |
|---|---:|---:|---|---|
| `docs/433_POLICY_CLAIM_PRODUCT_UI_SHELL_GATE8_STARTUP_INSTRUMENTATION_REPAIR_INDEPENDENT_RECHECK.md` | 40,717 | 672 | `f049843a8a31d0c5211db86e7bb789a9d15bec47ec1b1a33c53efc0b2f07aad7` | PASS |
| `docs/434_POLICY_CLAIM_PRODUCT_UI_SHELL_GATE8_SINGLE_ISOLATED_DIAGNOSTIC_PRODUCT_STARTUP_AND_RUNTIME_EVIDENCE.md` | 11,528 | 264 | `18aed9e914fdf8722c4e127ea84881248cdadc2d25ee26bb78ae52a32cfc51a5` | PASS |
| Original harness | 31,210 | 842 | `c02707d1e9240eae9afeb3f5b235248b8751144215c9d9c04919c501ec51db08` | PASS |
| `docs/435_POLICY_CLAIM_PRODUCT_UI_SHELL_GATE8_COMMAND_LOCAL_GIT_TRUST_ENVIRONMENT_REPAIR1_AND_SINGLE_RUNTIME_EVIDENCE.md` | 11,772 | 302 | `a340d759c67b6efd124619a69cc439786902d745f1f09baafaec5b8810885b40` | PASS |
| Repair1 harness | 34,797 | 927 | `05db1731da76176cf298c0a3f84e9e327afeaab44eea168a096f37379f451383` | PASS |

No prior evidence file was modified.

## E. Source, Test, and Binary Identity

| Artifact | Bytes | Lines | SHA-256 | Result |
|---|---:|---:|---|---|
| `StartupDiagnosticSession.cs` | 30,465 | 1,022 | `c42a35f771ac22d1f17543af86124df4eb7f630094dcdb977d3bc2fadc129093` | PASS |
| `StartupDiagnosticSessionTests.cs` | 29,045 | 876 | `5264fad93fa98c7977c41137acddb7113952d77a5a1e4da798c32929310544c9` | PASS |
| `AppStartupObservabilityContractTests.cs` | 11,599 | 342 | `df3072a32a8316d220041a79452931514da7bccc49ce1dd0502dde359f2c4030` | PASS |
| `FamilyClaimRef.App.exe` | 162,816 | N/A | `bb3de775939bbeb06aa9abe42e9e93cee51881084b3e6f20e7293a2d23300c39` | PASS |
| `FamilyClaimRef.App.dll` | 318,976 | N/A | `935c5c3c19db57deff3e109f912d4d3b48b80dfd9e81bb129130ea869bb4896c` | PASS |

Source, test, XAML, resource, project, EXE, and DLL were not modified.

## F. Repair2 Harness Identity

Created:

`docs/evidence/436_GATE8_WINDOWS_POWERSHELL_PATH_COMPATIBILITY_REPAIR2_SINGLE_DIAGNOSTIC_STARTUP/diagnostic_run_harness_repair2.ps1`

| Item | Value |
|---|---|
| Bytes | `39,980` |
| Lines | `1,055` |
| SHA-256 | `7914d64ddcc4af4e44ee2017b14ccdb8a587ea38c3291286ab25db6a900a7868` |

Permitted Repair1-to-Repair2 delta:

1. evidence paths changed from 435 to 436,
2. protected baseline changed from 56 to 58 paths,
3. T0 manifest identity gate was added,
4. `docs/435` and Repair1 harness identities were added,
5. the unsupported path API was removed,
6. Windows PowerShell-compatible path normalization was added,
7. Repair2 compatibility and cumulative invocation evidence fields were added.

Product start, child environment, window observation, NDJSON validation,
graceful close, captured-process fallback, exact-owner cleanup, and privacy
logic were not intentionally expanded.

## G. Compatible Path Normalizer

The Repair2 normalizer applies:

1. null, empty, and whitespace rejection,
2. `Path.IsPathRooted`,
3. `Path.GetFullPath`,
4. `Path.GetPathRoot`,
5. drive/share root rejection,
6. explicit separator `char[]`,
7. `String.TrimEnd(char[])`,
8. `OrdinalIgnoreCase` exact repository comparison,
9. separator conversion to `/` for Git.

Expected command-local Git path:

`C:/EtcProject/FamilyClaimRef`

The unsupported method reference count in the Repair2 harness is `0`.

## H. Repair2 Static Review

| Check | Result |
|---|---|
| PowerShell parser errors | `0` |
| Unsupported path API references | `0` |
| `pwsh` references | `0` |
| `Resolve-Path` references | `0` |
| Product `[Diagnostics.Process]::Start(...)` call sites | `1` |
| `Start-Process` / `dotnet run` | `0/0` |
| Retry loop / second-start fallback | `0/0` |
| Captured-process `.Kill()` implementations | `1` |
| Broad process-name kill | `0` |
| Exact-owned-root cleanup implementations | `1` |
| Broad parent cleanup | `0` |
| Wildcard safe-directory form | `0` |
| Persistent Git configuration mutation | `0` |
| Raw `git -C` repository reads outside helper | `0` |
| Repository Git helper implementations | `1` |
| Git helper references | `8` |
| Local-profile literals | `0` |
| Product argument configuration | `0` |
| Source/test/build invocation | `0` |
| Trailing whitespace / merge markers | `0/0` |

## I. Elevated Windows PowerShell Compatibility Probe

The probe was executed exactly once using:

- `powershell.exe`
- `-NoProfile`
- `-NonInteractive`
- elevated Windows PowerShell runtime

Runtime identity:

| Item | Actual |
|---|---|
| PowerShell edition | `Desktop` |
| PowerShell version | `5.1.26100.8875` |
| CLR version | `4.0.30319.42000` |

Probe checks:

| Check | Result |
|---|---|
| `Path.GetFullPath` | PASS |
| `Path.GetPathRoot` | PASS |
| `String.TrimEnd(char[])` | PASS |
| Plain repository path normalization | PASS |
| Trailing backslash normalization | PASS |
| Trailing slash normalization | PASS |
| Exact safe-directory result | PASS |
| Command-local Git branch | `main`, exit `0` |
| Command-local Git HEAD | expected HEAD, exit `0` |
| Command-local Git status | exit `0` |
| Status entries after harness creation | `59` |
| Protected existing status entries | `58` |
| Repository delta | `0` |
| TEMP root creation | `0` |
| Persistent Git config mutation | `0` |
| Product process/window | `0/0` |

Compatibility probe result: `PASS`.

## J. Repair2 Harness Invocation

| Counter | Actual |
|---|---:|
| Original harness invocation count | 1 |
| Repair1 harness invocation count | 1 |
| Prior total harness invocation count | 2 |
| Prior cumulative Product start attempts | 0 |
| Repair2 harness invocation count | 1 |
| Repair2 Product start attempts | 0 |
| Cumulative Product start attempts | 0 |
| Cumulative Product process creations | 0 |
| Second Product start attempts | 0 |

Failure:

`Repository path-set baseline mismatch.`

The failure occurred at the Repair2 internal path-set preflight before:

1. T0 content map capture,
2. run ID and nonce generation,
3. diagnostic/runtime TEMP leaf creation,
4. child environment assignment,
5. Product `Process.Start`.

Post-failure external read-only recomputation produced:

- protected path count: `58`
- protected path-set SHA-256:
  `9b15b71d81a7f76d512b359c46d91223bb9446f70673f6fb5eaa2277d7b2dbbe`
- protected T0 aggregate SHA-256:
  `8ab50ce002109abb81f841f057a4d2aaff87d90c1d3c96c07041eeefd4590f0f`

These values match the starting baseline. The mismatch is therefore limited to
the elevated harness's internal path-set calculation or ordering. The harness
did not persist its internal path list, so the exact ordering difference is
`UNRESOLVED`.

No Repair2 modification, second invocation, repeated compatibility probe,
alternate launcher, or direct Product execution followed.

## K. Expected and Actual Runtime Lifecycle

| Lifecycle group | Expected | Actual |
|---|---|---|
| App construction | constructor enter/ready | not observed |
| Startup entry | app startup and base begin/end | not observed |
| Mode selection | default Product mode | not observed |
| Service composition | create-default begin/end | not observed |
| ProductShell construction | constructor, initialize, DataContext | not observed |
| Main window | assignment, Show, loaded, content rendered | not observed |
| Dispatcher | scheduled and executed | not observed |
| Shutdown | window closed and app exit | not observed |

Runtime lifecycle result: `NOT_EXECUTED`.

## L. Runtime Evidence and Cleanup

| Evidence or state | Actual |
|---|---|
| Captured Product PID | `null` |
| Main window observed | `false` |
| `runtime_observation.json` | not created |
| TEMP `startup.ndjson` | not created |
| Repository `startup.ndjson` | not created |
| Diagnostic run root | not created |
| Isolated runtime root | not created |
| Graceful close requests | `0` |
| Fallback termination | `0` |
| Cleanup execution | not required, not executed |
| Product process/window after | `0/0` |
| Production/default runtime root access | `0` |
| `data/claimdoc` access | `0` |

No runtime observation was manually created.

## M. Repository Verification

Immediately after the failed Repair2 harness invocation and before this
document:

- tracked/staged/untracked: `29/0/30`
- status entries: `59`
- protected existing paths: `58`
- new path: Repair2 harness only
- protected path-set identity: unchanged
- protected T0 aggregate: unchanged
- prior evidence identities: unchanged
- source/test/binary identities: unchanged
- deletion/rename: `0/0`
- build/test: not run
- Product/file picker/workflow: not run
- stage/commit/push: `0/0/0`

Expected final HOLD state after this document:

- tracked/staged/untracked: `29/0/31`
- status entries: `60`
- existing protected paths: `58`
- approved new paths: `2`
- Repair2 runtime observation/log: absent

## N. Findings

| Severity | Count | Finding |
|---|---:|---|
| Blocking | 1 | Elevated Repair2 harness path-set calculation did not equal the externally reproduced 58-path baseline despite identical external path-set and T0 hashes. |
| Major | 0 | None |
| Minor | 0 | None |

The Windows PowerShell path API compatibility defect from Repair1 is corrected
and independently probed. Product behavior remains unobserved.

## O. Disposition

- Current diagnostic startup: `NOT_EXECUTED`
- Current App/ProductShell lifecycle: `NOT_OBSERVED`
- F-05 evidence gap: `OPEN`
- `docs/428` historical cause: `UNRESOLVED_HISTORICAL`
- Historical defect fixed: `NOT_PROVEN`
- Gate 8 final closure: `NOT_APPROVED`
- Deployment/production readiness: `NOT_AUTHORIZED`
- Stage/commit/push: `NOT_AUTHORIZED`

Exact result marker:

`POLICY_CLAIM_PRODUCT_UI_SHELL_GATE8_WINDOWS_POWERSHELL_PATH_COMPATIBILITY_REPAIR2_AND_SINGLE_DIAGNOSTIC_PRODUCT_STARTUP_HOLD_RUNTIME_EVIDENCE_OR_ENVIRONMENT_FINDING`

## P. Exact Created Scope

Created:

- `docs/436_POLICY_CLAIM_PRODUCT_UI_SHELL_GATE8_WINDOWS_POWERSHELL_PATH_COMPATIBILITY_REPAIR2_AND_SINGLE_RUNTIME_EVIDENCE.md`
- `docs/evidence/436_GATE8_WINDOWS_POWERSHELL_PATH_COMPATIBILITY_REPAIR2_SINGLE_DIAGNOSTIC_STARTUP/diagnostic_run_harness_repair2.ps1`

Not created:

- `docs/evidence/436_GATE8_WINDOWS_POWERSHELL_PATH_COMPATIBILITY_REPAIR2_SINGLE_DIAGNOSTIC_STARTUP/runtime_observation.json`
- `docs/evidence/436_GATE8_WINDOWS_POWERSHELL_PATH_COMPATIBILITY_REPAIR2_SINGLE_DIAGNOSTIC_STARTUP/startup.ndjson`

No existing repository file was modified.

## Q. Next Action

No additional Product startup is authorized by this batch.

A separately approved Repair3 batch may replace cross-host `Sort-Object`
dependency in protected path ordering with explicit
`StringComparer.Ordinal`, while preserving all prior evidence identities and
the one-start/no-retry rule. Repair3 must not proceed without explicit user
approval.
