# Gate 8 Command-Local Git Trust Environment Repair1 and Single Runtime Evidence

## A. Status

- Status: `HOLD`
- Marker:
  `POLICY_CLAIM_PRODUCT_UI_SHELL_GATE8_COMMAND_LOCAL_GIT_TRUST_ENVIRONMENT_REPAIR1_AND_SINGLE_DIAGNOSTIC_PRODUCT_STARTUP_HOLD_RUNTIME_EVIDENCE_OR_ENVIRONMENT_FINDING`
- Elevated read-only Git trust probe: `PASS`
- Repair1 harness pre-launch: `FAILED`
- Product runtime evidence: `NOT_EXECUTED`
- F-05: `OPEN`
- Gate 8 final closure: `NOT_APPROVED`
- Deployment/production readiness: `NOT_AUTHORIZED`
- Stage/commit/push: `0/0/0`

The elevated command-local Git trust probe succeeded. The Repair1 harness was
then invoked exactly once, but failed before its first repository read and
before Product `Process.Start` because the elevated Windows PowerShell runtime
does not expose `System.IO.Path.TrimEndingDirectorySeparator`.

The no-retry contract was applied. The Repair1 harness was not modified or
invoked again after the failure.

## B. Starting Baseline

| Item | Expected | Observed | Result |
|---|---|---|---|
| Branch | `main` | `main` | PASS |
| HEAD | `aecf7edfd43b4124ec5ff17d35687020cf4c0d90` | same | PASS |
| Tracked/staged/untracked | `29/0/27` | `29/0/27` | PASS |
| Status entries | `56` | `56` | PASS |
| Product process candidates | `0` | `0` | PASS |
| Product nonzero windows | `0` | `0` | PASS |
| `docs/435_*` preexistence | `0` | `0` | PASS |
| Evidence 435 directory preexistence | `0` | `0` | PASS |

Existing filtered Gate 8 path set:

- count: `54`
- LF-terminated sorted path-set SHA-256:
  `08c04ac187923eb2b983dc94412c26705aff10e16b6f11627419a4bf23f98aba`

Protected existing status set after including `docs/434` and its harness:

- count: `56`
- LF-terminated sorted path-set SHA-256:
  `20d7acae305a559c66a7562d29b5cc13f009dd28db510d2ccf0bc8b5cdbe58e3`

## C. Protected Evidence Identity

| Artifact | Bytes | Lines | SHA-256 | Result |
|---|---:|---:|---|---|
| `docs/433_POLICY_CLAIM_PRODUCT_UI_SHELL_GATE8_STARTUP_INSTRUMENTATION_REPAIR_INDEPENDENT_RECHECK.md` | 40,717 | 672 | `f049843a8a31d0c5211db86e7bb789a9d15bec47ec1b1a33c53efc0b2f07aad7` | PASS |
| `docs/434_POLICY_CLAIM_PRODUCT_UI_SHELL_GATE8_SINGLE_ISOLATED_DIAGNOSTIC_PRODUCT_STARTUP_AND_RUNTIME_EVIDENCE.md` | 11,528 | 264 | `18aed9e914fdf8722c4e127ea84881248cdadc2d25ee26bb78ae52a32cfc51a5` | PASS |
| `docs/evidence/434_GATE8_SINGLE_DIAGNOSTIC_STARTUP/diagnostic_run_harness.ps1` | 31,210 | 842 | `c02707d1e9240eae9afeb3f5b235248b8751144215c9d9c04919c501ec51db08` | PASS |

Neither protected file from `docs/434` was modified.

## D. Source, Test, and Binary Identity

| Artifact | Bytes | Lines | SHA-256 | Result |
|---|---:|---:|---|---|
| `StartupDiagnosticSession.cs` | 30,465 | 1,022 | `c42a35f771ac22d1f17543af86124df4eb7f630094dcdb977d3bc2fadc129093` | PASS |
| `StartupDiagnosticSessionTests.cs` | 29,045 | 876 | `5264fad93fa98c7977c41137acddb7113952d77a5a1e4da798c32929310544c9` | PASS |
| `AppStartupObservabilityContractTests.cs` | 11,599 | 342 | `df3072a32a8316d220041a79452931514da7bccc49ce1dd0502dde359f2c4030` | PASS |
| `FamilyClaimRef.App.exe` | 162,816 | N/A | `bb3de775939bbeb06aa9abe42e9e93cee51881084b3e6f20e7293a2d23300c39` | PASS |
| `FamilyClaimRef.App.dll` | 318,976 | N/A | `935c5c3c19db57deff3e109f912d4d3b48b80dfd9e81bb129130ea869bb4896c` | PASS |

Source, test, XAML, resource, project, EXE, and DLL were not modified.

## E. Repair1 Harness Identity

Created:

`docs/evidence/435_GATE8_COMMAND_LOCAL_GIT_TRUST_REPAIR1_SINGLE_DIAGNOSTIC_STARTUP/diagnostic_run_harness_repair1.ps1`

| Item | Value |
|---|---|
| Bytes | `34,797` |
| Lines | `927` |
| SHA-256 | `05db1731da76176cf298c0a3f84e9e327afeaab44eea168a096f37379f451383` |

Permitted delta from the protected harness:

1. evidence root and harness filename changed to the 435 Repair1 paths,
2. protected baseline changed from filtered `54` paths to existing `56` paths,
3. prior `docs/434` and harness identities were added as pre-launch gates,
4. `Invoke-RepositoryGitRead` was added for command-local exact-repository
   `safe.directory`,
5. repository Git reads were routed through that helper,
6. final branch/HEAD/status/diff checks were added,
7. Repair1 environment-evidence fields were added to the observation schema.

Product start, window observation, diagnostic validation, graceful close,
captured-process fallback, exact-owner cleanup, and privacy gates were copied
without intentional behavior expansion.

## F. Repair1 Static Review

| Check | Result |
|---|---|
| PowerShell parser errors | `0` |
| Product `[Diagnostics.Process]::Start(...)` call sites | `1` |
| Retry loop / second-start fallback | `0/0` |
| `Start-Process` / `dotnet run` | `0/0` |
| Captured-process `.Kill()` implementations | `1` |
| Broad process-name kill | `0` |
| Exact-owned-root `Remove-Item` implementations | `1` |
| Broad parent cleanup | `0` |
| Local-profile path literals | `0` |
| Git mutation commands | `0` |
| Git config mutation commands | `0` |
| Wildcard `safe.directory` form | `0` |
| Exact repository literal | `1` |
| Git helper implementation | `1` |
| Raw `git -C` repository reads outside helper | `0` |
| Helper references | `7` |
| Product argument configuration | `0` |
| Trailing whitespace / merge markers | `0/0` |

The static review did not detect the Windows PowerShell runtime compatibility
problem in `System.IO.Path.TrimEndingDirectorySeparator`. Parser success did
not prove that the referenced framework method exists at runtime.

## G. Elevated Read-Only Git Trust Probe

The probe was executed exactly once in an elevated context before invoking the
Repair1 harness.

Every repository Git command used:

`git -c "safe.directory=C:/EtcProject/FamilyClaimRef" -C "C:/EtcProject/FamilyClaimRef" <read-only command>`

Probe result:

| Item | Actual |
|---|---|
| Branch command exit code | `0` |
| Branch | `main` |
| HEAD command exit code | `0` |
| HEAD | `aecf7edfd43b4124ec5ff17d35687020cf4c0d90` |
| Status command exit code | `0` |
| Total status entries after Repair1 harness creation | `57` |
| Protected existing status entries | `56` |
| Repair1 harness status entries | `1` |
| Product process/window | `0/0` |
| Probe result | `PASS` |

Git trust boundary:

- `safe.directory` mode: `command_local_exact_repository`
- wildcard used: `false`
- persistent Git config mutation: `0`
- repository parent, TEMP root, and drive root trust: `0`
- ownership/ACL mutation: `0`

## H. Repair1 Harness Invocation

| Counter or result | Actual |
|---|---|
| Prior harness invocation count | `1` |
| Prior Product start attempt count | `0` |
| Repair1 harness invocation count | `1` |
| Repair1 Product start attempt count | `0` |
| Cumulative Product start attempt count | `0` |
| Cumulative Product process creation count | `0` |
| Second Product start attempt count | `0` |

Failure category:

`PRE_LAUNCH_WINDOWS_POWERSHELL_PATH_API_COMPATIBILITY`

The first `Invoke-RepositoryGitRead` call attempted to normalize the repository
path with `System.IO.Path.TrimEndingDirectorySeparator`. That method was not
available in the elevated Windows PowerShell runtime. The helper failed before
executing its internal Git command and the harness terminated.

Control-flow review confirms the failure occurred before:

1. repository branch/HEAD baseline completion,
2. cryptographic run ID and nonce generation,
3. diagnostic or runtime TEMP leaf creation,
4. child environment assignment,
5. Product `[Diagnostics.Process]::Start(...)`.

No Repair1 harness modification, second invocation, alternate launcher, or
direct Product startup followed this failure.

## I. Expected and Actual Runtime Lifecycle

| Lifecycle group | Expected source-derived evidence | Actual |
|---|---|---|
| App construction | constructor enter/ready | not observed |
| Startup entry | `app_on_startup.enter`, base begin/end | not observed |
| Mode selection | `startup_mode.selection/default` | not observed |
| Service composition | create-default begin/end | not observed |
| ProductShell construction | constructor, initialize, DataContext | not observed |
| Main window | assignment, Show, loaded, content rendered | not observed |
| Dispatcher | scheduled and executed | not observed |
| Shutdown | window closed, app exit enter/return | not observed |

Runtime lifecycle result: `NOT_EXECUTED`.

## J. Runtime Evidence and Cleanup

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

No runtime observation was manually fabricated.

## K. Repository Verification

Immediately after the failed Repair1 harness invocation and before this
document was created:

- tracked/staged/untracked: `29/0/28`
- status entries: `57`
- existing protected paths: `56`
- new path: Repair1 harness only
- protected `docs/434` and harness identities: unchanged
- source/test/binary identities: unchanged
- deletion/rename: `0/0`
- build/test: not run
- Product/file picker/workflow: not run
- stage/commit/push: `0/0/0`

Expected final HOLD state after this document:

- tracked/staged/untracked: `29/0/29`
- status entries: `58`
- existing protected paths: `56`
- approved new paths: `2`
- `runtime_observation.json` and `startup.ndjson`: absent

## L. Findings

| Severity | Count | Finding |
|---|---:|---|
| Blocking | 1 | Repair1 harness uses a path API unavailable in the elevated Windows PowerShell runtime and therefore cannot reach its command-local Git read or Product launch. |
| Major | 0 | None |
| Minor | 0 | None |

The elevated command-local Git trust approach itself was proven by the external
probe. The remaining blocker is harness runtime compatibility, not repository
trust and not Product behavior.

## M. Disposition

- Current diagnostic startup: `NOT_EXECUTED`
- Current App/ProductShell lifecycle: `NOT_OBSERVED`
- F-05 actual runtime evidence gap: `OPEN`
- `docs/428` historical failure cause: `UNRESOLVED_HISTORICAL`
- Gate 8 final closure: `NOT_APPROVED`
- Deployment/production readiness: `NOT_AUTHORIZED`
- Stage/commit/push: `NOT_AUTHORIZED`

Exact result marker:

`POLICY_CLAIM_PRODUCT_UI_SHELL_GATE8_COMMAND_LOCAL_GIT_TRUST_ENVIRONMENT_REPAIR1_AND_SINGLE_DIAGNOSTIC_PRODUCT_STARTUP_HOLD_RUNTIME_EVIDENCE_OR_ENVIRONMENT_FINDING`

## N. Exact Created Scope

Created:

- `docs/435_POLICY_CLAIM_PRODUCT_UI_SHELL_GATE8_COMMAND_LOCAL_GIT_TRUST_ENVIRONMENT_REPAIR1_AND_SINGLE_RUNTIME_EVIDENCE.md`
- `docs/evidence/435_GATE8_COMMAND_LOCAL_GIT_TRUST_REPAIR1_SINGLE_DIAGNOSTIC_STARTUP/diagnostic_run_harness_repair1.ps1`

Not created:

- `docs/evidence/435_GATE8_COMMAND_LOCAL_GIT_TRUST_REPAIR1_SINGLE_DIAGNOSTIC_STARTUP/runtime_observation.json`
- `docs/evidence/435_GATE8_COMMAND_LOCAL_GIT_TRUST_REPAIR1_SINGLE_DIAGNOSTIC_STARTUP/startup.ndjson`

No existing repository path was modified.

## O. Next Action

No additional Product startup is authorized by this batch.

A separately approved Repair2 batch may create a new harness that performs
exact repository path normalization using APIs available in the actual elevated
Windows PowerShell runtime. It must preserve:

- command-local exact-repository `safe.directory`,
- no persistent Git configuration mutation,
- protected `docs/434`, `docs/435`, and both prior harness identities,
- one Product start maximum,
- no retry,
- isolated test-owned roots,
- privacy and exact cleanup gates.
