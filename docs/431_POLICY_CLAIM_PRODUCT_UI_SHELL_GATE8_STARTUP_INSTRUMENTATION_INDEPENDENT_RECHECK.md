# Gate 8 Startup Instrumentation Independent Source and Automated Recheck

## 1. 역할과 최종 판정

이 문서는 `docs/430` 작성 및 구현에 참여하지 않은 독립 검토자가 수행한
source, test, document, build, Git 경계 재검증 결과다.

최종 판정은 `REJECT`다. 자동화 결과와 일반적인 startup/lifecycle 계약은
통과했지만, strict TEMP 경계가 마지막 reparse-point 검사와
`FileMode.CreateNew` 사이의 path swap을 원자적으로 차단하지 않는다.
directive의 `TEMP 경계 우회 가능` 판정 규칙에 해당하므로 repair가 필요하다.

Final marker:

`POLICY_CLAIM_PRODUCT_UI_SHELL_GATE8_STARTUP_INSTRUMENTATION_INDEPENDENT_RECHECK_REJECT_REPAIR_REQUIRED`

## 2. 시작 baseline

| Item | Observed | Result |
|---|---|---|
| Branch | `main` | 일치 |
| HEAD | `aecf7edfd43b4124ec5ff17d35687020cf4c0d90` | 일치 |
| Subject | `docs(familyclaimref): record gate8 registration persistence decision package` | 일치 |
| Tracked/staged/untracked | `29/0/22` | 일치 |
| Status entries | `51` | 일치 |
| `docs/431` preexistence | `0` | 일치 |
| Product process candidates | `0` | 일치 |

시작 identity:

| Artifact | Bytes | Lines | SHA-256 |
|---|---:|---:|---|
| `docs/429_POLICY_CLAIM_PRODUCT_UI_SHELL_GATE8_PRODUCT_STARTUP_OBSERVABILITY_STATIC_ANALYSIS_AND_DECISION.md` | 24569 | 434 | `8e0e1606f37ad9c1732d1d9259ebd137c40e2e10af928f9959f2e82aa2db2b9b` |
| `docs/430_POLICY_CLAIM_PRODUCT_UI_SHELL_GATE8_MINIMAL_OPT_IN_STARTUP_INSTRUMENTATION_IMPLEMENTATION_AND_STATIC_VERIFICATION.md` | 15321 | 447 | `e56dc6a53d8bd58a325f3cb1ab973527bdc281b0fa4ca09a3bd1ecd806201db2` |
| starting Product EXE | 162816 | N/A | `bb3de775939bbeb06aa9abe42e9e93cee51881084b3e6f20e7293a2d23300c39` |
| starting Product DLL | 315392 | N/A | `8654604b6a2e1715bf558735cefae0cdf26b9516b89b9dd899f269f5c5f9d0ff` |

Baseline 또는 `docs/430` transport identity mismatch는 없었다.

## 3. 검토한 exact 파일

Directive의 직접 검토 대상:

- `app/FamilyClaimRef.App/App.xaml.cs`
- `app/FamilyClaimRef.App/ProductShell/ProductShellWindow.xaml.cs`
- `app/FamilyClaimRef.App/Startup/StartupDiagnosticSession.cs`
- `tests/FamilyClaimRef.App.Tests/StartupDiagnosticSessionTests.cs`
- `tests/FamilyClaimRef.App.Tests/AppStartupObservabilityContractTests.cs`
- `docs/430_POLICY_CLAIM_PRODUCT_UI_SHELL_GATE8_MINIMAL_OPT_IN_STARTUP_INSTRUMENTATION_IMPLEMENTATION_AND_STATIC_VERIFICATION.md`

비교 및 owner 경계 확인 대상:

- `app/FamilyClaimRef.App/App.xaml`
- `app/FamilyClaimRef.App/ProductShell/ProductShellWindow.xaml`
- `app/FamilyClaimRef.App/Composition/AppServices.cs`
- `app/FamilyClaimRef.App/Services/Runtime/EnvironmentRuntimeRootProvider.cs`
- `app/FamilyClaimRef.App/Services/Runtime/RuntimeRootPaths.cs`
- `app/FamilyClaimRef.App/FamilyClaimRef.App.csproj`
- `tests/FamilyClaimRef.App.Tests/FamilyClaimRef.App.Tests.csproj`
- `FamilyClaimRef.sln`
- `docs/429_POLICY_CLAIM_PRODUCT_UI_SHELL_GATE8_PRODUCT_STARTUP_OBSERVABILITY_STATIC_ANALYSIS_AND_DECISION.md`
- 시작 `51`-path status manifest

## 4. Exact delta 및 기존 manifest 보존

독립 검토 시작 시 구현 delta는 다음 exact 6-path set과 일치했다.

Modified:

- `app/FamilyClaimRef.App/App.xaml.cs`
- `app/FamilyClaimRef.App/ProductShell/ProductShellWindow.xaml.cs`

Created:

- `app/FamilyClaimRef.App/Startup/StartupDiagnosticSession.cs`
- `tests/FamilyClaimRef.App.Tests/StartupDiagnosticSessionTests.cs`
- `tests/FamilyClaimRef.App.Tests/AppStartupObservabilityContractTests.cs`
- `docs/430_POLICY_CLAIM_PRODUCT_UI_SHELL_GATE8_MINIMAL_OPT_IN_STARTUP_INSTRUMENTATION_IMPLEMENTATION_AND_STATIC_VERIFICATION.md`

| Check | Measured result |
|---|---|
| Missing/extra implementation path | `0/0` |
| Allowed list 밖 source/test delta | `0` |
| `App.xaml` / `ProductShellWindow.xaml` delta | `0/0` |
| `AppServices` 및 runtime-root owner의 이 batch delta | `0` |
| Storage/repository/registration/persistence owner의 이 batch delta | `0` |
| Project/solution delta | `0/0` |
| `docs/413` through `docs/429`의 이 batch delta | `0` |
| Deletion/rename | `0/0` |
| 기존 45-path count | `45/45` |
| 기존 45-path path fingerprint | `ffebc26e9c13849c439a00045cc9a6d9d9411334c5f27846e41feed461721537` |
| 기존 45-path content fingerprint | `d5a2880c12c8c7efa696cd81edf558964fb0e111a10c9408d8062eb385ef6eeb` |
| 기존 45-path content mismatch | `0` |
| 시작 51-path path fingerprint | `db2945392402cf3ea9c3312bbc3709bbba3f4eea0eea00518c9f731719e4206b` |
| 시작 51-path content fingerprint | `848c35dbf9425998c334864338a64521a52199c95fe032eaffb2999481a740bb` |

## 5. Source contract 판정

| Area | Independent result | 근거 |
|---|---|---|
| Activation | 충족 | `StringComparison.Ordinal`로 exact `1`만 enable한다. Root 누락/invalid는 disabled session으로 종료한다. |
| Disabled behavior | 충족 | directory/file/handler/background work가 생성되지 않으며 Record는 no-op이다. Product runtime-root/storage owner 호출이 없다. |
| Normalized strict child | 충족 | `Path.GetFullPath`, exact shared-root 거부, separator를 포함한 `OrdinalIgnoreCase` child 비교로 prefix 오인을 막는다. |
| Stable existing reparse points | 충족 | candidate에서 TEMP root까지 기존 component를 검사하고 `Directory.CreateDirectory` 뒤 다시 검사한다. |
| Concurrent reparse/path swap | 위반 | 마지막 검사 뒤 `FileStream` open까지 checked directory를 pin하지 않아 path swap을 통한 TEMP 경계 이탈이 가능하다. |
| Existing-log overwrite | 충족 | `FileMode.CreateNew`가 안정된 pathname의 기존 파일 overwrite를 원자적으로 거부한다. |
| Compensation ownership | 위반 | ownership을 handle/file identity가 아닌 사전 `rootExisted` boolean과 pathname으로 판단한다. |
| NDJSON byte limit | 충족 | UTF-8 payload byte length와 LF 1 byte를 합산하고 최종 `Position`이 `131072`를 넘지 않게 한다. |
| Record concurrency | source로 충족 | sequence, size check, write, flush가 동일 `lock (sync)` 안에 있다. |
| Record/Dispose race | source로 충족 | 둘 다 동일 lock을 사용하고 disposed/writeStopped를 확인하며 외부로 예외를 전달하지 않는다. |
| Privacy | 충족 | owner/milestone/phase/result/method는 allowlist 정규화되고 exception type/HResult만 기록한다. raw args/env/message/stack/path는 기록하지 않는다. |
| Control characters | 충족 | 사용자 제공 문자열은 allowlist 밖에서 고정값 또는 null이 되고 JSON serializer가 record 구조를 보존한다. |
| No-throw logging | 충족 | setup/Record/Dispose/handler logging 실패를 Product exception으로 전달하지 않는다. |
| Handler lifecycle | 충족, residual risk 기록 | session당 attach 1회, Dispose idempotent, OnExit finally detach가 확인됐다. `Handled` 변경 및 `SetObserved()` 호출은 없다. |

### 5.1 Confirmed TEMP boundary defect

`StartupDiagnosticSession.CreateForConfiguration`의 순서는 다음과 같다.

1. `TryNormalizeDiagnosticRoot`에서 path 및 reparse-point 검사
2. `Directory.CreateDirectory(normalizedRoot)`
3. `ContainsReparsePoint(normalizedRoot)` 재검사
4. pathname 기반 `new FileStream(..., FileMode.CreateNew, ...)`

3과 4 사이에 checked leaf 또는 ancestor가 junction/symbolic link로
교체되면 4는 새 target을 따라갈 수 있다. `FileMode.CreateNew`는 기존
파일 overwrite는 막지만, 검증한 directory identity를 고정하거나
reparse traversal을 금지하지 않는다. 또한 1과 2 사이의 parent swap은
2가 dedicated TEMP 밖에 directory side effect를 만든 뒤 3에서 거부되는
경우를 허용한다. 따라서 post-create 재검사는 안정된 reparse point에는
유효하지만 strict boundary를 원자적으로 보장하지 않는다.

### 5.2 Compensation ownership defect

`rootExisted`는 `Directory.CreateDirectory` 전 pathname observation이다.
그 뒤 경쟁자가 directory를 먼저 만들면 session이 만들지 않은 빈
directory도 `TryDeleteNewEmptyDirectory` 대상이 될 수 있다.

`logFileCreatedBySession`도 file handle identity를 보존하지 않는다.
setup failure에서 stream dispose 후 pathname이 교체되면
`TryDeleteOwnedLogFile`이 session이 생성한 file이 아닌 현재
`startup.ndjson`을 삭제할 수 있다. reparse swap과 결합하면 삭제
경계도 dedicated TEMP 내부로 입증되지 않는다.

## 6. Handler 및 exception 의미

- `RegisterHandlers`의 `disposed || handlersRegistered` guard와 lock으로
  duplicate subscription을 막는다.
- Runtime registrar는 세 handler를 1회 attach하고 정상 Dispose에서
  detach한다.
- `App.OnExit`는 `base.OnExit` 또는 exit record 경로의 결과와 무관하게
  `finally`에서 Dispose를 호출한다.
- `DispatcherUnhandledExceptionEventArgs.Handled`를 변경하지 않는다.
- `UnobservedTaskExceptionEventArgs.SetObserved()`를 호출하지 않는다.
- `AppServices.CreateDefault`, outer startup, ProductShell constructor catch는
  record 후 `throw;`를 사용한다.
- fallback window, exception swallow, alternate startup flow는 없다.
- Logging callback은 `RecordCore`의 no-throw 경계 안에 있다.

Residual risk: `RuntimeStartupDiagnosticEventRegistrar.Detach`는 각
unsubscribe를 개별 `try/finally`로 격리하지 않는다. 일반적인 현재 event
remove 경로에서는 예외가 예상되지 않지만, 첫 unsubscribe가 예외를
던지는 비정상 상황의 나머지 detach는 behavioral test로 입증되지 않았다.

## 7. App startup order 비교

변경 전:

1. `base.OnStartup(e)`
2. `StartupWindowModeSelector.Select(e.Args)`
3. `AppServices.CreateDefault()`
4. `ProductShellWindow` construction
5. `MainWindow = selectedWindow`
6. `selectedWindow.Show()`

변경 후에도 Product operation 순서는 동일하다. Diagnostic Record 호출은
각 operation의 전후에 추가됐고 기존 selector argument `e.Args`는 그대로다.
원문 args는 기록하지 않고 allowlisted `default` 또는
`product_shell_preview`만 기록한다.

`AppServices.CreateDefault()` 내부는 이 batch에서 변경되지 않았다.
Custom `Main`, `StartupUri`, generated entrypoint replacement, project item
변경은 없다. Product-owned source는 generated `App.InitializeComponent()`
완료를 직접 계측하지 않고 `app_constructor.body_ready`와
`app_on_startup.enter` 사이로만 한정한다.

## 8. ProductShell post-Show 및 lifecycle

- Public one-argument constructor는 유지된다.
- 기존 `ArgumentNullException.ThrowIfNull`, `InitializeComponent`,
  `DataContext` assignment 순서는 유지된다.
- Diagnostics OFF에서는 `startupDiagnostics`가 null이므로 추가
  Loaded/ContentRendered/Closed handler와 dispatcher work가 `0`이다.
- Loaded 및 ContentRendered handler는 첫 event에서 자신을 detach한다.
- Closed는 남은 lifecycle handler를 정리한다.
- `App.OnStartup`은 `selectedWindow.Show()` 반환 record 뒤에만
  `ScheduleStartupDispatcherObservation()`을 호출한다.
- Schedule guard와 boolean으로 callback은 최대 1회 요청된다.
- `BeginInvoke` 실패와 callback Record 실패는 Product exception 의미를
  바꾸지 않는다.

따라서 `post-Show`는 source ordering 관점에서 정확하다. 실제 Product
process에서 callback 실행까지 도달했는지는 이번 batch에서 검증하지 않았다.

## 9. 신규 테스트 요구사항 매핑

Targeted discovered cases는 총 `29`다.

| Requirement | Test strength | Independent result |
|---|---|---|
| exact activation values | Behavioral, 5 theory cases | 충족 |
| missing/relative/non-TEMP/shared/file/occupied root | Behavioral | 충족 |
| normalized parent segment | Behavioral | 충족 |
| actual reparse point | Behavioral `Directory.CreateSymbolicLink`, skip 없음 | stable leaf reparse만 충족 |
| parseable, flushed NDJSON | Behavioral, shared live reader | 충족 |
| UTF-8 actual file byte bound | Behavioral `FileInfo.Length` | 충족 |
| privacy synthetic literals | Behavioral final-log absence check | 충족 |
| handler duplicate/detach observation | Behavioral fake registrar event | 충족 |
| Record after Dispose | Behavioral | 순차 case 충족 |
| concurrent Record | 없음 | `EVIDENCE_GAP`; source lock으로만 입증 |
| Record/Dispose race | 없음 | `EVIDENCE_GAP`; source lock으로만 입증 |
| ancestor/path-swap reparse race | 없음 | 결함을 탐지하지 못함 |
| App order and `throw;` | Static source string ordering | 취약한 contract test |
| ProductShell enabled-only/post-Show | Static source string search | 취약한 contract test; 독립 source review로 보강 |
| Handled/SetObserved semantics | Static negative string search | behavioral event-args 검증 없음 |
| public constructor | Reflection | 충족 |
| Product EXE/App.Run/Show/window creation 0 | Static scan plus executed test inventory | 충족 |
| environment restoration | 환경 변수를 변경하는 신규 test 없음 | restoration 대상 없음 |

실제 test TEMP root의 최종 recursive residue entry count는 `0`이다.

## 10. Automated recheck

Directive 순서대로 Product process precheck, build, targeted tests, full
tests, Product process postcheck, TEMP residue, `git diff --check`, exact path
delta를 확인했다.

첫 sandbox build와 첫 sandbox targeted test는
`C:\Users\jin8855\AppData\Local\Microsoft SDKs` read denial로
`ENVIRONMENT_OR_TOOLCHAIN_BLOCKED`였다. 동일 command의 승인된 재실행은
성공했으며 code/test failure로 분류하지 않는다.

| Validation | Final measured result |
|---|---|
| `dotnet build .\FamilyClaimRef.sln --nologo --verbosity minimal` | warning/error `0/0` |
| Targeted suites | total/passed/failed/skipped `29/29/0/0` |
| Full solution tests | total/passed/failed/skipped `515/515/0/0` |
| Product process before/after | `0/0` |
| Product launch | `0` |
| Top-level WPF window creation | `0` |
| Product nonzero main-window handle | `0` |
| Test TEMP residue entries | `0` |
| `git diff --check` | exit `0`; line-ending warnings only |
| Unexpected repository path | `0` |

Targeted raw summary:

`총 테스트 수: 29`, `통과: 29`; failed/skipped `0/0`.

Full raw summary:

`실패: 0, 통과: 515, 건너뜀: 0, 전체: 515`.

## 11. Generated binary identity

Final build 후 identity:

| Artifact | Bytes | SHA-256 | Classification |
|---|---:|---|---|
| `app/FamilyClaimRef.App/bin/Debug/net10.0-windows/FamilyClaimRef.App.exe` | 162816 | `bb3de775939bbeb06aa9abe42e9e93cee51881084b3e6f20e7293a2d23300c39` | generated output only |
| `app/FamilyClaimRef.App/bin/Debug/net10.0-windows/FamilyClaimRef.App.dll` | 315392 | `8654604b6a2e1715bf558735cefae0cdf26b9516b89b9dd899f269f5c5f9d0ff` | generated output only |

두 identity 모두 시작 값과 같다. Binary를 실행하거나 runtime evidence로
승격하지 않았다.

## 12. 독립 findings

| ID | Severity | Classification | Finding |
|---|---|---|---|
| F-01 | HIGH | `IMPLEMENTATION_FAILURE` | 마지막 reparse-point 검사와 pathname `FileMode.CreateNew` 사이의 check/use race로 strict TEMP boundary 이탈이 가능하다. |
| F-02 | HIGH | `IMPLEMENTATION_FAILURE` | setup compensation이 stable directory/file identity가 아니라 pathname과 `rootExisted` boolean에 의존해 session 비소유 entry 삭제 가능성을 배제하지 못한다. |
| F-03 | MEDIUM | `EVIDENCE_GAP` | concurrent Record 및 Record/Dispose race behavioral test가 없다. 현재 source lock은 계약을 지지하지만 회귀 검출 강도는 부족하다. |
| F-04 | MEDIUM | `EVIDENCE_GAP` | 실제 reparse test는 stable leaf symlink만 다루며 ancestor swap, post-check swap, boundary escape를 검증하지 않는다. |
| F-05 | LOW | `EVIDENCE_GAP` | App/ProductShell/handler exception semantics의 다수 test가 production source 문자열 검색이며 behavioral WPF/lifecycle 검증이 아니다. |

F-01과 F-02는 evidence 부재가 아니라 source에서 확인된 계약 위반이다.
따라서 build/test 성공을 독립 implementation 승인으로 승격하지 않는다.

## 13. Protected-path audit

| Item | Measured result |
|---|---|
| Product EXE invocation | `0` |
| Diagnostic Product startup | `0` |
| WPF top-level window creation | `0` |
| File picker/UIA/browser/screenshot | `0/0/0/0` |
| Preflight 및 R01-R09 execution | `0` |
| Production runtime root direct access | `0` |
| `data/claimdoc` direct access | `0` |
| Source/test/XAML/resource/project repair | `0/0/0/0/0` |
| Existing document modification | `0` |
| Stage/commit/push/tag/rebase/amend | `0/0/0/0/0/0` |
| Reset/checkout/clean/stash | `0/0/0/0` |

이번 독립 batch가 생성한 repository file은 이 `docs/431` 하나다.

## 14. Runtime 미검증 항목

- Product-owned `App` constructor 및 generated `App.InitializeComponent()` 도달
- 실제 `OnStartup` 및 `AppServices.CreateDefault()` 완료
- ProductShell construction, Show, Loaded, ContentRendered 도달
- post-Show dispatcher callback 실행
- 실제 Product diagnostic log 생성과 exception/exit record
- 실제 Product process에서의 timing 및 startup 원인
- docs/427의 ProductShell unavailable 원인

Runtime 실행 부재는 F-01/F-02의 source-level 결함을 해소하지 않는다.

## 15. 최종 Git gate

`docs/431` 생성 후 최종 기대 및 확인 대상:

| Item | Final value |
|---|---|
| Branch | `main` |
| HEAD | `aecf7edfd43b4124ec5ff17d35687020cf4c0d90` |
| Tracked/staged/untracked | `29/0/23` |
| Status entries | `52` |
| 기존 51-path set/content | 불변 |
| 새 repository path | 이 `docs/431` 1개 |
| Deletion/rename | `0/0` |
| `git diff --check` | exit `0` |
| Stage/commit/push | `0/0/0` |

Retained states:

- docs/428 runtime cause: `UNRESOLVED`
- Guarded runtime functional review: `NOT_COMPLETED`
- Product runtime retry: `NOT_AUTHORIZED`
- Diagnostic Product startup: `NOT_AUTHORIZED`
- Deployment/production readiness: `NOT_AUTHORIZED`
- Stage/commit: `NOT_AUTHORIZED`

다음 batch는 F-01/F-02를 지정된 범위에서 repair하고 concurrency 및
path-swap test를 추가한 뒤 독립 재검증해야 한다. Product diagnostic
startup은 별도 승인 전까지 허용되지 않는다.
