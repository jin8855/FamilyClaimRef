# Gate 8 Startup Instrumentation Reparse Ownership and Concurrency Repair

## A. Status Marker

`POLICY_CLAIM_PRODUCT_UI_SHELL_GATE8_STARTUP_INSTRUMENTATION_REPARSE_OWNERSHIP_AND_CONCURRENCY_REPAIR_IMPLEMENTED_AUTOMATED_PASS_RUNTIME_NOT_AUTHORIZED`

## B. Scope

이번 배치는 `docs/431`에서 확인된 startup diagnostic 경로 안전성, 소유권,
동시성 문제만 교정했다.

수정한 exact 파일:

- `app/FamilyClaimRef.App/Startup/StartupDiagnosticSession.cs`
- `tests/FamilyClaimRef.App.Tests/StartupDiagnosticSessionTests.cs`
- `tests/FamilyClaimRef.App.Tests/AppStartupObservabilityContractTests.cs`

생성한 exact 파일:

- `docs/432_POLICY_CLAIM_PRODUCT_UI_SHELL_GATE8_STARTUP_INSTRUMENTATION_REPARSE_OWNERSHIP_AND_CONCURRENCY_REPAIR.md`

변경하지 않은 주요 owner:

- `App.xaml.cs`
- `App.xaml`
- `ProductShellWindow.xaml.cs`
- `ProductShellWindow.xaml`
- `AppServices.cs`
- `EnvironmentRuntimeRootProvider.cs`
- `RuntimeRootPaths.cs`
- storage/repository/registration/persistence owner
- project/solution
- `docs/413~431`

## C. Starting Baseline

| Item | Observed |
|---|---|
| Branch | `main` |
| HEAD | `aecf7edfd43b4124ec5ff17d35687020cf4c0d90` |
| Subject | `docs(familyclaimref): record gate8 registration persistence decision package` |
| Tracked/staged/untracked | `29/0/23` |
| Status entries | `52` |
| Product process candidates | `0` |
| `docs/432` preexistence | `0` |

문서 identity:

| Document | Bytes | Lines | SHA-256 |
|---|---:|---:|---|
| `docs/430` | 15321 | 447 | `e56dc6a53d8bd58a325f3cb1ab973527bdc281b0fa4ca09a3bd1ecd806201db2` |
| `docs/431` | 18296 | 346 | `724b6701fb8bb8ce6e6dd624cce6f9463109c9d7fa069e16f82f03d7c64d0933` |

시작 52-path set SHA-256:

`2803d3965b9ea456e9a840b7d285d698b974dec1b5403c15c483427530bd215c`

허용된 3개 파일을 제외한 보호 49-path content manifest SHA-256:

`bf92b683544f143b220053e631170a27b167cd892451d1e5e5bc3a8b2ecfea70`

## D. `docs/431` Finding Disposition

| Finding | Severity | Disposition | Evidence |
|---|---|---|---|
| F-01 pathname check/use race | HIGH | `CLOSED_BY_REPAIR_AND_AUTOMATED_EVIDENCE` | TEMP root부터 requested leaf까지 directory handle lease를 유지하고 log handle final path를 검증했다. 실제 leaf/ancestor rename 차단 test가 통과했다. |
| F-02 pathname compensation ownership | HIGH | `CLOSED_BY_REPAIR_AND_AUTOMATED_EVIDENCE` | directory 생성·삭제와 pathname file 삭제를 제거했다. 실패 시 소유한 open handle만 닫고 residue를 보존한다. sentinel 회귀 test가 통과했다. |
| F-03 concurrent `Record`/`Dispose` evidence | MEDIUM | `CLOSED_BY_AUTOMATED_EVIDENCE` | 다중 writer와 `Record`/`Dispose` barrier 경쟁 test가 모두 통과했다. |
| F-04 reparse/race evidence | MEDIUM | `CLOSED_BY_AUTOMATED_EVIDENCE` | 실제 leaf symbolic link, ancestor symbolic link, leaf rename/replacement, ancestor rename test가 skip 없이 통과했다. |
| F-05 WPF runtime lifecycle evidence | LOW | `DEFERRED_NOT_AUTHORIZED` | source/static evidence는 유지했지만 실제 WPF runtime lifecycle은 실행하지 않았다. |

F-05 exact state:

- App/ProductShell 실제 runtime lifecycle: `NOT_EXECUTED`
- source/static evidence: `RETAINED`
- implementation defect: `NOT_CONFIRMED`
- independent runtime verification: `DEFERRED, NOT_AUTHORIZED`

## E. Pre-existing Root Contract

새 activation 계약:

- enable 값은 ordinal exact `1`이어야 한다.
- Windows가 아니면 diagnostics는 disabled다.
- configured root는 fully-qualified path여야 한다.
- configured root는 `%TEMP%\FamilyClaimRef\StartupDiagnostics`의 strict child여야 한다.
- configured root 전체 경로는 activation 전에 이미 존재해야 한다.
- root가 없거나 directory가 아니면 diagnostics는 disabled다.
- Product diagnostic code는 directory를 생성하지 않는다.
- Product diagnostic code는 directory 또는 file pathname을 삭제하지 않는다.
- invalid configuration과 setup 실패는 Product startup에 exception을 전파하지 않는다.

Product source 정적 결과:

| Forbidden operation | Count |
|---|---:|
| `Directory.CreateDirectory` | 0 |
| `Directory.Delete` | 0 |
| `File.Delete` | 0 |
| `FileShare.Delete` | 0 |

격리 root 준비는 향후 별도 승인된 실행 절차의 책임이다. 이번 배치에서는
root를 준비하거나 Product를 실행하지 않았다.

## F. `docs/430` Supersession

| Topic | `docs/430` state | Repair state |
|---|---|---|
| Root creation | session이 missing root를 생성 | pre-existing root 필수, Product 생성 0 |
| Reparse 검증 | pathname attribute 검사 | component별 open handle attribute와 final path 검사 |
| TOCTOU 경계 | 최종 pathname 검사 후 file 생성 | 모든 directory handle을 file 생성과 session lifecycle 동안 유지 |
| Setup compensation | session 생성 추정값을 이용한 pathname 삭제 | pathname 삭제 0, 소유한 handle dispose만 허용 |
| Setup failure artifact | best-effort 삭제 | 빈 파일 또는 부분 파일 residue 허용 및 보존 |
| Reparse evidence | stable leaf symlink | leaf/ancestor symlink와 rename/replacement behavioral test |
| Concurrency evidence | source lock 중심 | concurrent `Record`와 `Record`/`Dispose` behavioral test |
| WPF runtime evidence | 미실행 | 계속 미실행, 별도 승인 필요 |

이 표의 repair state가 startup diagnostic root와 file ownership에 대한 최신
계약이다.

## G. Windows Directory Handle Lease

Diagnostics ON 후보에서 다음 component를 순서대로 open한다.

1. normalized OS TEMP root
2. TEMP root 아래 configured path의 각 directory component
3. requested leaf root

각 component는 `CreateFileW`로 연다.

| Parameter | Value |
|---|---|
| desired access | `FILE_READ_ATTRIBUTES` |
| creation disposition | `OPEN_EXISTING` |
| flags | `FILE_FLAG_BACKUP_SEMANTICS | FILE_FLAG_OPEN_REPARSE_POINT` |
| share mode | `FILE_SHARE_READ | FILE_SHARE_WRITE` |
| delete sharing | 없음 |
| backup privilege | 요구하지 않음 |

각 `SafeFileHandle`에서 확인:

- `GetFileInformationByHandleEx(FileAttributeTagInfo)`
- directory attribute 존재
- reparse-point attribute 부재
- `GetFinalPathNameByHandleW`의 DOS final path
- expected normalized component path와 ordinal-ignore-case exact equality

UNC, SUBST, device path, unsupported filesystem 또는 예상하지 않은 final-path
형식처럼 identity를 입증할 수 없는 경우 diagnostics를 disabled한다.

부분 open 실패 시 이미 획득한 handle을 leaf 방향부터 역순으로 닫는다.
static/global mutable lease와 background keeper는 없다.

## H. Lease Lifetime and Dispose Order

directory lease는 다음 전체 기간 동안 유지된다.

- final root 검증
- `startup.ndjson` `CreateNew`
- log file handle final-path 검증
- 활성 session lifecycle
- final stream close

Dispose 순서:

1. event handler detach
2. `Record`와 동일한 synchronization boundary 획득
3. writer/file stream close
4. directory lease handle을 leaf부터 역순으로 close

검증 직후 handle을 조기 dispose하거나 마지막 검사 뒤 pathname만 신뢰하는
경로는 없다.

## I. Log File Identity

log filename과 overwrite 계약은 유지했다.

- filename: `startup.ndjson`
- creation: `FileMode.CreateNew`
- access: write
- share: live read를 위한 `FileShare.Read`
- 기존 file overwrite: 0

file 생성 직후 첫 record 전에 file handle을 검증한다.

- file attribute가 directory가 아님
- reparse-point attribute가 없음
- final DOS path가 `<requestedRoot>\startup.ndjson`와 exact equality

검증 실패 시 stream과 directory lease만 dispose한다. pathname delete는
수행하지 않는다.

## J. Ownership and Failure Residue

제거한 상태와 동작:

- `rootExisted`
- `logFileCreatedBySession`
- `TryDeleteNewEmptyDirectory`
- `TryDeleteOwnedLogFile`
- setup catch의 pathname compensation

소유권은 session이 가진 `SafeFileHandle`과 `FileStream`에만 한정한다.

setup이 file 생성 뒤 실패하면:

- open stream/handle은 no-throw로 닫는다.
- 빈 file 또는 부분 file residue가 남을 수 있다.
- 현재 pathname의 entry를 삭제하지 않는다.
- 경쟁자 소유 sentinel file/directory를 변경하지 않는다.

정상 Dispose에서도 log file은 증거로 보존한다.

## K. Concurrency and Lifecycle

동일 `sync` boundary 안에서 유지되는 동작:

1. disposed/write-stopped 상태 검사
2. 다음 sequence 계산
3. UTF-8 payload와 newline byte 계산
4. 128 KiB 상한 검사
5. payload와 newline write
6. disk flush
7. sequence commit

검증된 결과:

- concurrent caller exception 0
- partial/interleaved NDJSON record 0
- duplicate sequence 0
- strict monotonic sequence
- final file size `<= 131072`
- `Dispose` 이후 `Record` no-op
- `Dispose` 완료 뒤 file length 불변
- background retry 0

`RuntimeStartupDiagnosticEventRegistrar.Detach`는 task scheduler, dispatcher,
AppDomain unsubscribe를 각각 독립적인 no-throw 경계에서 시도한다. 첫
unsubscribe 실패가 뒤 detach 시도를 차단하지 않는다.

## L. Behavioral Test Evidence

추가 또는 보강한 핵심 case:

- valid missing root가 disabled이며 directory/file delta 0
- pre-existing valid root에서 session enabled
- 기존 `startup.ndjson` bytes 불변
- actual leaf symbolic link 거부
- actual ancestor symbolic link 거부
- live lease 중 leaf rename과 replacement sequence 차단
- live lease 중 ancestor rename 차단
- lease Dispose 후 test-owned rename/replacement 성공
- setup validation 실패 뒤 competitor sentinel bytes/path 불변
- setup 실패 log residue 보존
- 다중 task concurrent `Record`
- barrier 기반 `Record`/`Dispose` 경쟁
- dispose 후 additional `Record` no-op과 file length 불변

symbolic link, rename, concurrency test의 skipped case는 0이다.
Product EXE, `App.Run`, `Show`, `ShowDialog`, top-level WPF window는
호출하지 않았다.

## M. Automated Verification

초기 sandbox build는 Windows SDK discovery 경로 권한으로 실행되지
않았다. 같은 build를 승인된 elevated context에서 재실행했다.

| Verification | Result |
|---|---|
| Build | PASS |
| Build warnings/errors | `0/0` |
| Startup targeted tests | `37/37` |
| Targeted failed/skipped | `0/0` |
| Previous targeted baseline | `29` |
| Added targeted cases | `8` |
| Full solution tests | `523/523` |
| Full failed/skipped | `0/0` |
| Previous full baseline | `515` |
| Product process before/after | `0/0` |
| Product launch/window creation | `0/0` |
| Test TEMP residue entries | `0` |
| `git diff --check` | PASS |

## N. Static and Privacy Audit

| Contract | Result |
|---|---|
| default OFF | retained |
| ordinal exact `1` | retained |
| Windows gate | present |
| pre-existing root gate | present |
| component `SafeFileHandle` ownership | present |
| handle-based reparse attribute | present |
| handle-based directory final path | present |
| handle-based log final path | present |
| delete sharing | absent |
| pathname compensation | absent |
| 128 KiB bound | retained |
| environment write | absent |
| exception message/`ToString`/stack trace | absent |
| Product launch/background mechanism | absent |
| storage/registration owner reference | absent |

## O. Generated Binary Identity

Build 후 generated binary:

| Binary | Bytes | SHA-256 |
|---|---:|---|
| `FamilyClaimRef.App.exe` | 162816 | `bb3de775939bbeb06aa9abe42e9e93cee51881084b3e6f20e7293a2d23300c39` |
| `FamilyClaimRef.App.dll` | 318976 | `935c5c3c19db57deff3e109f912d4d3b48b80dfd9e81bb129130ea869bb4896c` |

이 identity는 automated build 증거이며 Product runtime 증거가 아니다.

## P. Protected-path and Git Audit

`docs/432` 생성 전 확인:

- 시작 52-path path-set SHA-256:
  `2803d3965b9ea456e9a840b7d285d698b974dec1b5403c15c483427530bd215c`
- 현재 기존 52-path path-set SHA-256: 동일
- 보호 49-path content manifest SHA-256:
  `bf92b683544f143b220053e631170a27b167cd892451d1e5e5bc3a8b2ecfea70`
- 보호 49-path content mismatch: 0
- 기존 52개 중 content delta: 허용된 3개만
- 삭제/rename: `0/0`
- 허용 목록 밖 신규 repository path: 0

수정 파일 identity:

| Path | Bytes | Lines | SHA-256 |
|---|---:|---:|---|
| `StartupDiagnosticSession.cs` | 30465 | 1022 | `c42a35f771ac22d1f17543af86124df4eb7f630094dcdb977d3bc2fadc129093` |
| `StartupDiagnosticSessionTests.cs` | 29045 | 876 | `5264fad93fa98c7977c41137acddb7113952d77a5a1e4da798c32929310544c9` |
| `AppStartupObservabilityContractTests.cs` | 11599 | 342 | `df3072a32a8316d220041a79452931514da7bccc49ce1dd0502dde359f2c4030` |

최종 기대 Git 상태:

- tracked modified: 29
- staged: 0
- untracked: 24
- status entries: 53
- 기존 52-path set: 불변
- 신규 path: `docs/432`만
- stage/commit/push: `0/0/0`

## Q. Runtime Boundary

이번 배치에서 실행하지 않은 항목:

- Product EXE
- diagnostic Product startup
- WPF top-level window
- preflight 및 R01~R09
- UIA/browser/screenshot
- registration/persistence workflow
- deployment

유지 상태:

- `docs/428` runtime cause: `UNRESOLVED`
- Guarded runtime functional review: `NOT_COMPLETED`
- Product runtime retry: `NOT_AUTHORIZED`
- Diagnostic Product startup: `NOT_AUTHORIZED`
- Deployment/production readiness: `NOT_AUTHORIZED`
- Stage/commit: `NOT_AUTHORIZED`

## R. Final Judgment

F-01과 F-02의 source-level 결함은 handle lease와 pathname compensation 제거로
교정되었다. F-03과 F-04는 실제 Windows filesystem behavioral test로
보강되었다. build, targeted test, full test와 exact Git scope가 통과했다.

판정:

`POLICY_CLAIM_PRODUCT_UI_SHELL_GATE8_STARTUP_INSTRUMENTATION_REPARSE_OWNERSHIP_AND_CONCURRENCY_REPAIR_IMPLEMENTED_AUTOMATED_PASS_RUNTIME_NOT_AUTHORIZED`

다음 행동은 새 Codex 세션에서 F-01~F-04 closure, handle lifetime,
actual rename/replace tests, concurrency tests와 exact Git scope를 독립
재검증하는 것이다.

독립 PASS 전에는 diagnostic Product startup을 승인하지 않는다.
