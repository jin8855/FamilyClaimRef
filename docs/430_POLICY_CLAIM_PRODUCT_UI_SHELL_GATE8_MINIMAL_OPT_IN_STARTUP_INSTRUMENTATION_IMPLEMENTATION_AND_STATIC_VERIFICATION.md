# Gate 8 Minimal Opt-In Startup Instrumentation Implementation and Static Verification

## 1. Status

- Marker:
  `POLICY_CLAIM_PRODUCT_UI_SHELL_GATE8_MINIMAL_OPT_IN_STARTUP_INSTRUMENTATION_IMPLEMENTED_STATIC_AND_AUTOMATED_VERIFICATION_PASS_RUNTIME_NOT_AUTHORIZED`
- Selected decision source:
  `docs/429`, option
  `O2. MINIMAL_OPT_IN_STARTUP_INSTRUMENTATION_REQUIRED`
- Implementation scope: exact minimal startup instrumentation
- Product launch: `0`
- Top-level WPF window creation by this batch: `0`
- Diagnostic Product runtime execution: `0`
- Stage/commit/push: `0/0/0`

The implementation is diagnostic instrumentation, not a Product startup defect
repair. The existing runtime cause remains unresolved.

## 2. Starting Baseline

| Item | Observed value | Result |
|---|---|---|
| Branch | `main` | PASS |
| HEAD | `aecf7edfd43b4124ec5ff17d35687020cf4c0d90` | PASS |
| Subject | `docs(familyclaimref): record gate8 registration persistence decision package` | PASS |
| Tracked/staged/untracked | `27/0/18` | PASS |
| Status entries | `45` | PASS |
| Existing exact path set | `45/45` | PASS |
| Missing/extra | `0/0` | PASS |
| Existing 45-path path-set fingerprint | `ffebc26e9c13849c439a00045cc9a6d9d9411334c5f27846e41feed461721537` | recorded |
| Existing 45-path content fingerprint | `d5a2880c12c8c7efa696cd81edf558964fb0e111a10c9408d8062eb385ef6eeb` | recorded |
| docs/430 preexistence | `0` | PASS |

Protected starting identities:

| Artifact | Bytes | SHA-256 |
|---|---:|---|
| `docs/429_POLICY_CLAIM_PRODUCT_UI_SHELL_GATE8_PRODUCT_STARTUP_OBSERVABILITY_STATIC_ANALYSIS_AND_DECISION.md` | 24569 | `8e0e1606f37ad9c1732d1d9259ebd137c40e2e10af928f9959f2e82aa2db2b9b` |
| starting Product EXE | 162816 | `bb3de775939bbeb06aa9abe42e9e93cee51881084b3e6f20e7293a2d23300c39` |
| starting Product DLL | 294912 | `4534420c13f4f01b80263a73d3a3c71bbe3ce1c0c01836e3b069a6ce218e1f3f` |

Product process count before implementation/build/test: `0`.

## 3. Exact Repository Delta

Modified:

- `app/FamilyClaimRef.App/App.xaml.cs`
- `app/FamilyClaimRef.App/ProductShell/ProductShellWindow.xaml.cs`

Created:

- `app/FamilyClaimRef.App/Startup/StartupDiagnosticSession.cs`
- `tests/FamilyClaimRef.App.Tests/StartupDiagnosticSessionTests.cs`
- `tests/FamilyClaimRef.App.Tests/AppStartupObservabilityContractTests.cs`
- `docs/430_POLICY_CLAIM_PRODUCT_UI_SHELL_GATE8_MINIMAL_OPT_IN_STARTUP_INSTRUMENTATION_IMPLEMENTATION_AND_STATIC_VERIFICATION.md`

No repository file outside this exact six-file set was created or modified by
this batch.

Explicitly unchanged:

- `App.xaml`
- `ProductShellWindow.xaml`
- all Resources owners
- `AppServices.cs`
- `EnvironmentRuntimeRootProvider.cs`
- `RuntimeRootPaths.cs`
- all storage, repository, registration, and persistence owners
- all project and solution files
- all preexisting test files
- docs/413 through docs/429

SDK default compile inclusion is used. No project-file item was added.

## 4. Activation Contract

Diagnostics are enabled only when both conditions are met:

1. `FAMILYCLAIMREF_ENABLE_STARTUP_DIAGNOSTICS` is exactly `1`;
2. `FAMILYCLAIMREF_STARTUP_DIAGNOSTIC_ROOT` resolves to a valid isolated
   child directory under
   `%TEMP%\FamilyClaimRef\StartupDiagnostics`.

The following enable values are disabled:

- missing;
- empty;
- `0`;
- `true`;
- `01`;
- any value other than exact ordinal `1`.

OFF or invalid configuration behavior:

- directory creation: `0`;
- file creation: `0`;
- handler registration: `0`;
- timer/thread/task creation: `0`;
- Product storage/runtime-root access: `0`;
- all Record APIs: no-op and no-throw;
- Product startup exception replacement: `0`.

The implementation reads the two environment variables. It does not write or
persist any environment value.

## 5. Diagnostic Root Safety

Validation occurs before output creation:

- nonempty fully qualified path required;
- normalized path must be a strict child of the dedicated diagnostic TEMP
  area;
- relative path rejected;
- exact shared diagnostic area rejected because it is not an isolated run
  root;
- non-TEMP path rejected;
- existing file path rejected;
- normalized parent segments are accepted only when the final path remains
  inside the allowed area;
- existing path components are checked for reparse points;
- a reparse-point root disables diagnostics;
- an existing `startup.ndjson` is never overwritten;
- directory or log preparation failure disables diagnostics without changing
  Product startup.

Compensation is ownership-bounded. A log is deleted after setup failure only
when this session conclusively created it. A competing process's preexisting
file is not deleted.

Actual symlink-capable automated validation used
`Directory.CreateSymbolicLink` and passed. No platform guard or skipped test
was used.

## 6. Output Contract

- format: NDJSON;
- file count per enabled session root: exactly one;
- file name: `startup.ndjson`;
- maximum size: 131072 bytes;
- writes beyond the limit: dropped without exception or retry;
- flush: after every written record;
- sequence: process-local monotonic increment;
- wall clock: UTC timestamp;
- monotonic time: `Stopwatch.ElapsedTicks`;
- process ID: included;
- managed thread ID: included;
- owner, milestone, phase, and result: allowlisted values only;
- exception type and HResult: allowed;
- Product-owned method identifier: exact allowlist only.

Forbidden output:

- raw command-line arguments;
- raw environment values;
- raw exception message;
- `Exception.ToString()`;
- stack file paths or line numbers;
- absolute source or user-profile paths;
- original document or attachment paths;
- policy, claim, document, link, or attachment data.

All logging and disposal methods are no-throw. No background thread, task,
timer, worker, or retry loop exists.

## 7. Lifecycle and Exception Contract

The enabled session registers at most one set of:

- `AppDomain.CurrentDomain.UnhandledException`;
- `Application.DispatcherUnhandledException`;
- `TaskScheduler.UnobservedTaskException`.

Repeated registration calls add no handler. Normal `OnExit` disposes the
session and detaches the registered set.

Exception semantics are preserved:

- `DispatcherUnhandledExceptionEventArgs.Handled` is not changed;
- `UnobservedTaskExceptionEventArgs.SetObserved()` is not called;
- startup catches record only normalized type/HResult/owner data;
- startup catches immediately use `throw;`;
- logging failures do not replace the Product exception;
- no fallback window or alternate startup flow was introduced.

## 8. Actual Milestones

### 8.1 App

- `app_constructor.body_enter`
- `startup_diagnostics.handler_registration`, on registration failure only
- `app_constructor.body_ready`
- `app_on_startup.enter`
- `base_on_startup`, begin/end
- `startup_mode.selection`, allowlisted default or preview classification
- `app_services_create_default`, begin/end/exception
- `product_shell_window.construction`, begin/end
- `application.main_window_assignment`
- `product_shell_window.show`, begin/return
- `app_on_startup.exception`
- `app_on_exit`, enter/return/exception

The explicit Product `App` constructor starts after the framework base
constructor. It does not claim to observe entry into the base constructor.

Generated `App.InitializeComponent()` remains between:

- `app_constructor.body_ready`;
- `app_on_startup.enter`.

No custom `Main`, generated entrypoint replacement, or project-file change was
introduced.

### 8.2 ProductShellWindow

- constructor body enter/return/exception;
- `InitializeComponent`, begin/end;
- DataContext assignment, begin/end;
- first `Loaded`;
- first `ContentRendered`;
- one post-Show dispatcher callback, scheduled/executed/failure;
- `Closed`.

The public one-argument constructor remains available. App uses an
assembly-internal two-argument overload to pass the diagnostic session.

When diagnostics are OFF:

- additional UI event handlers: `0`;
- dispatcher callback scheduling: `0`;
- window behavior/order change: `0`.

## 9. docs/429 Interpretation Correction

`AppServices.CreateDefault()` and its internal implementation remain
unchanged. The implementation records only the outer interval:

- `app_services_create_default.begin`;
- `app_services_create_default.end`;
- `app_services_create_default` with failed result and normalized exception.

This interval contains both runtime-root resolution and composition. It does
not claim separate internal milestones for either operation.

## 10. Automated Verification

### 10.1 New targeted suites

Command scope:

- `StartupDiagnosticSessionTests`;
- `AppStartupObservabilityContractTests`.

Final result:

| Item | Result |
|---|---:|
| Total | 29 |
| Passed | 29 |
| Failed | 0 |
| Skipped | 0 |

Covered contracts:

- unset/empty/`0`/`true`/`01` enable values;
- missing, relative, empty, non-TEMP, non-isolated, file, and occupied-log
  roots;
- normalized parent segment inside the allowed area;
- actual reparse-point root rejection;
- exactly one parseable and immediately readable NDJSON file;
- required fields and valid PID/thread identity;
- monotonic sequence and elapsed ticks;
- bounded 128 KiB output;
- privacy inputs and raw exception message absent from log;
- idempotent handler registration and detach;
- no observation after detach;
- no-throw Record after disposal;
- existing startup order;
- preserved `throw;` semantics;
- no custom Main, StartupUri, project item, Product launch, or background work;
- no storage/registration owner reference in the diagnostic session;
- public one-argument ProductShell constructor compatibility.

### 10.2 Full suite

Final result:

| Item | Result |
|---|---:|
| Previous baseline | 486 |
| New tests | 29 |
| Total | 515 |
| Passed | 515 |
| Failed | 0 |
| Skipped | 0 |

### 10.3 Intermediate failures and corrections

1. Initial sandboxed build was blocked by Windows SDK path access:
   `ENVIRONMENT_OR_TOOLCHAIN_BLOCKED`.
2. First elevated compile found one implementation import omission:
   `IMPLEMENTATION_FAILURE`; `System.IO` was added in the allowed new source.
3. First targeted run was `22/27`, with five live-log read failures caused by
   the test reader's sharing mode:
   `TEST_CONTRACT_FAILURE`.
4. Test live readers were corrected to explicit `FileShare.ReadWrite`.
5. Final build, targeted tests, and full tests all passed.

Two exact failed-test TEMP leaf roots were inspected and removed. This was
test-output compensation only, not Product runtime cleanup. Final test TEMP
residue entry count is `0`.

## 11. Build and Generated Output

Final build:

- warning: `0`;
- error: `0`;
- result: PASS.

Final generated binary identity:

| Artifact | Bytes | SHA-256 | Classification |
|---|---:|---|---|
| Product EXE | 162816 | `bb3de775939bbeb06aa9abe42e9e93cee51881084b3e6f20e7293a2d23300c39` | generated build output; unchanged apphost identity |
| Product DLL | 315392 | `8654604b6a2e1715bf558735cefae0cdf26b9516b89b9dd899f269f5c5f9d0ff` | generated build output; changed as expected |

The generated binary is not runtime review evidence. It was not launched,
loaded through a custom harness, or used to claim Product milestone reach.

## 12. Protected Boundary Audit

| Item | Result |
|---|---|
| Product EXE invocation | `0` |
| Product process before/after | `0/0` |
| Top-level WPF window creation by tests | `0` |
| Preflight execution | `0` |
| R01-R09 execution | `0` |
| UIA/browser/screenshot/Evidence 06 | `0/0/0/0` |
| Diagnostic Product runtime execution | `0` |
| Production runtime root access/deletion | `0/0` |
| `data/claimdoc` access | `0` |
| Storage/registration/persistence owner delta | `0` |
| XAML/resource/project/solution delta | `0/0/0/0` |
| Existing test file delta | `0` |
| docs/413-429 delta | `0` |
| Local profile literal in exact changed files | `0` |
| Trailing whitespace/merge markers | `0/0` |
| `git diff --check` | PASS |
| Stage/commit/push | `0/0/0` |

## 13. Existing 45-Path Preservation

The two allowed modified source files were clean at the starting baseline and
therefore were not members of the starting 45-path status set. All starting
45 paths remained byte-identical.

| Item | Final comparison |
|---|---|
| Existing path count | `45/45` |
| Existing path-set fingerprint | unchanged `ffebc26e9c13849c439a00045cc9a6d9d9411334c5f27846e41feed461721537` |
| Existing content fingerprint | unchanged `d5a2880c12c8c7efa696cd81edf558964fb0e111a10c9408d8062eb385ef6eeb` |
| Existing hash mismatch count | `0` |
| Existing deletion/rename | `0/0` |

The starting-count forecast of tracked `27` assumed the two allowed source
owners were already modified. They were not. Path equality therefore takes
precedence:

- starting tracked modified: `27`;
- newly modified allowed tracked owners: `2`;
- final tracked modified: `29`.

## 14. Final Git Gate

Required final repository delta is the exact six-file set in section 3.

| Item | Final value |
|---|---|
| Branch | `main` |
| HEAD | `aecf7edfd43b4124ec5ff17d35687020cf4c0d90` |
| Tracked/staged/untracked | `29/0/22` |
| Status entries | `51` |
| Existing exact 45-path set | unchanged |
| Existing 45-path content mismatch | `0` |
| New allowed status paths before docs/430 | `5/5` |
| docs/430 | this new file only |
| Unexpected repository path | `0` |
| `git diff --check` | PASS |
| Stage/commit/push | `0/0/0` |

## 15. Runtime Items Still Unverified

This batch does not verify:

- Product-owned `App` constructor milestone in a real Product process;
- generated `App.InitializeComponent()` completion;
- `OnStartup` reachability in the prior failing environment;
- actual `AppServices.CreateDefault()` completion;
- ProductShell construction, Show, Loaded, ContentRendered, or dispatcher
  callback in a Product process;
- actual isolated diagnostic log creation by Product startup;
- normal or unhandled Product exit record;
- diagnostic timing impact in a real Product run;
- the cause of docs/427 ProductShell unavailability.

## 16. Final Judgment

PASS:

- exact implementation scope preserved;
- default OFF contract verified;
- invalid configuration fail-open verified;
- isolated and bounded NDJSON contract verified;
- privacy literal exposure `0`;
- exception propagation semantics unchanged;
- Product launch/window creation `0/0`;
- build PASS;
- targeted tests `29/29`;
- full tests `515/515`;
- existing 45-path content delta `0`;
- protected boundary PASS;
- Git gate PASS.

Final marker:

`POLICY_CLAIM_PRODUCT_UI_SHELL_GATE8_MINIMAL_OPT_IN_STARTUP_INSTRUMENTATION_IMPLEMENTED_STATIC_AND_AUTOMATED_VERIFICATION_PASS_RUNTIME_NOT_AUTHORIZED`

Retained states:

- docs/428 classification:
  `C - HOLD_PRODUCT_WINDOW_UNAVAILABLE_CAUSE_UNRESOLVED`;
- Guarded runtime functional review: `NOT_COMPLETED`;
- Final Gate 8 implementation: `HOLD_RUNTIME_REVIEW_REQUIRED`;
- User visual acceptance: `NOT_AVAILABLE_AS_NEXT_GATE`;
- Deployment/production readiness: `NOT_AUTHORIZED`;
- Product runtime retry: `NOT_AUTHORIZED`;
- Diagnostic runtime execution: `NOT_AUTHORIZED`;
- Stage/commit: `NOT_AUTHORIZED`.

## 17. Single Next Recommendation

An independent Codex session should read-only recheck the exact six-file delta,
the new tests, generated binary identity, privacy contract, Product non-launch,
and Git gate. No diagnostic Product startup run is authorized before that
independent recheck.
