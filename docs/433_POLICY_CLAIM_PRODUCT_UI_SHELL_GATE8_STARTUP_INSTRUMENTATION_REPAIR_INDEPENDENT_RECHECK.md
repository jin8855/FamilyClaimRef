# POLICY CLAIM PRODUCT UI SHELL GATE8 STARTUP INSTRUMENTATION REPAIR INDEPENDENT RECHECK

Task ID:

`POLICY_CLAIM_PRODUCT_UI_SHELL_GATE8_STARTUP_INSTRUMENTATION_REPARSE_OWNERSHIP_AND_CONCURRENCY_REPAIR_INDEPENDENT_RECHECK`

## 1. 역할, 범위, 실행 경계

이 문서는 `docs/432` 작성 및 repair 구현에 참여하지 않은 독립 검토 결과다.
`docs/432`의 self-report를 독립 증거로 승격하지 않고 실제 source, test,
filesystem behavior, build/test output 및 exact Git 상태를 다시 확인했다.

이번 검토에서 실행하지 않은 항목:

- Product EXE, `App.Run`, diagnostic Product startup
- WPF top-level window, `Show`, `ShowDialog`
- file picker, UIA, browser, screenshot
- preflight, R01-R09, registration/persistence workflow
- production runtime root, `data/claimdoc` 접근
- source/test/XAML/resource/project repair
- stage/commit/push/reset/checkout/clean/stash/rebase/amend/tag

생성한 repository path는 이 `docs/433` 하나다.

## 2. 시작 baseline

| Item | 독립 실측 | Expected | Result |
|---|---|---|---|
| Branch | `main` | `main` | 일치 |
| HEAD | `aecf7edfd43b4124ec5ff17d35687020cf4c0d90` | same | 일치 |
| Subject | `docs(familyclaimref): record gate8 registration persistence decision package` | same | 일치 |
| Tracked/staged/untracked | `29/0/24` | `29/0/24` | 일치 |
| Status entries | `53` | `53` | 일치 |
| `docs/433` preexistence | `0` | `0` | 일치 |
| Product process candidates | `0` | `0` | 일치 |
| Product nonzero main-window handles | `0` | `0` | 일치 |

Baseline mismatch는 없었으므로 구현 검토를 진행했다.

문서 identity:

| Path | Bytes | Lines | SHA-256 |
|---|---:|---:|---|
| `docs/430_POLICY_CLAIM_PRODUCT_UI_SHELL_GATE8_MINIMAL_OPT_IN_STARTUP_INSTRUMENTATION_IMPLEMENTATION_AND_STATIC_VERIFICATION.md` | 15321 | 447 | `e56dc6a53d8bd58a325f3cb1ab973527bdc281b0fa4ca09a3bd1ecd806201db2` |
| `docs/431_POLICY_CLAIM_PRODUCT_UI_SHELL_GATE8_STARTUP_INSTRUMENTATION_INDEPENDENT_RECHECK.md` | 18296 | 346 | `724b6701fb8bb8ce6e6dd624cce6f9463109c9d7fa069e16f82f03d7c64d0933` |
| `docs/432_POLICY_CLAIM_PRODUCT_UI_SHELL_GATE8_STARTUP_INSTRUMENTATION_REPARSE_OWNERSHIP_AND_CONCURRENCY_REPAIR.md` | 14058 | 382 | `9956485ba995b5301766c239aae44899460e190a54d5966729eb13c205c3ca71` |

Repair 대상 source/test identity:

| Path | Bytes | Lines | SHA-256 |
|---|---:|---:|---|
| `app/FamilyClaimRef.App/Startup/StartupDiagnosticSession.cs` | 30465 | 1022 | `c42a35f771ac22d1f17543af86124df4eb7f630094dcdb977d3bc2fadc129093` |
| `tests/FamilyClaimRef.App.Tests/StartupDiagnosticSessionTests.cs` | 29045 | 876 | `5264fad93fa98c7977c41137acddb7113952d77a5a1e4da798c32929310544c9` |
| `tests/FamilyClaimRef.App.Tests/AppStartupObservabilityContractTests.cs` | 11599 | 342 | `df3072a32a8316d220041a79452931514da7bccc49ce1dd0502dde359f2c4030` |

## 3. 검토한 exact 파일

직접 source/test review:

- `app/FamilyClaimRef.App/Startup/StartupDiagnosticSession.cs`
- `tests/FamilyClaimRef.App.Tests/StartupDiagnosticSessionTests.cs`
- `tests/FamilyClaimRef.App.Tests/AppStartupObservabilityContractTests.cs`
- `app/FamilyClaimRef.App/App.xaml.cs`
- `app/FamilyClaimRef.App/ProductShell/ProductShellWindow.xaml.cs`

계약 및 scope review:

- `app/FamilyClaimRef.App/App.xaml`
- `app/FamilyClaimRef.App/ProductShell/ProductShellWindow.xaml`
- `app/FamilyClaimRef.App/Composition/AppServices.cs`
- `app/FamilyClaimRef.App/Services/Runtime/EnvironmentRuntimeRootProvider.cs`
- `app/FamilyClaimRef.App/Services/Runtime/RuntimeRootPaths.cs`
- `app/FamilyClaimRef.App/FamilyClaimRef.App.csproj`
- `tests/FamilyClaimRef.App.Tests/FamilyClaimRef.App.Tests.csproj`
- `FamilyClaimRef.sln`
- `docs/430_POLICY_CLAIM_PRODUCT_UI_SHELL_GATE8_MINIMAL_OPT_IN_STARTUP_INSTRUMENTATION_IMPLEMENTATION_AND_STATIC_VERIFICATION.md`
- `docs/431_POLICY_CLAIM_PRODUCT_UI_SHELL_GATE8_STARTUP_INSTRUMENTATION_INDEPENDENT_RECHECK.md`
- `docs/432_POLICY_CLAIM_PRODUCT_UI_SHELL_GATE8_STARTUP_INSTRUMENTATION_REPARSE_OWNERSHIP_AND_CONCURRENCY_REPAIR.md`
- 시작 53-path manifest와 독립 재구성한 보호 49-path identity manifest

## 4. 시작 53-path manifest

아래 목록은 `git -c core.quotepath=false status --porcelain=v1 -uall`에서
독립 재구성했다. `docs/432`를 제외한 52-path LF-terminated path-set
SHA-256은 directive와 같은
`2803d3965b9ea456e9a840b7d285d698b974dec1b5403c15c483427530bd215c`다.
현재 53-path LF-terminated path-set SHA-256은
`2a99daacb2e33b4336a1992d3bc23ffa12dcacd6f50a6feebbd5ff978540621d`다.

```text
 M app/FamilyClaimRef.App/App.xaml.cs
 M app/FamilyClaimRef.App/Composition/AppServices.cs
 M app/FamilyClaimRef.App/Models/Storage/DocumentDraft.cs
 M app/FamilyClaimRef.App/Models/Storage/DocumentRecord.cs
 M app/FamilyClaimRef.App/ProductShell/ProductShellWindow.xaml.cs
 M app/FamilyClaimRef.App/Resources/UiStrings.xaml
 M app/FamilyClaimRef.App/Services/Localization/UiTextKeys.cs
 M app/FamilyClaimRef.App/Services/Storage/ClaimDocumentRegistrationRequest.cs
 M app/FamilyClaimRef.App/Services/Storage/DocumentAttachmentCoordinator.cs
 M app/FamilyClaimRef.App/Services/Storage/DocumentAttachmentRequest.cs
 M app/FamilyClaimRef.App/Services/Storage/DocumentRegistrationWorkflow.cs
 M app/FamilyClaimRef.App/Services/Storage/FileAttachmentCopyResult.cs
 M app/FamilyClaimRef.App/Services/Storage/IDocumentStorageService.cs
 M app/FamilyClaimRef.App/Services/Storage/IFileAttachmentService.cs
 M app/FamilyClaimRef.App/Services/Storage/JsonDocumentStorageService.cs
 M app/FamilyClaimRef.App/Services/Storage/LocalFileAttachmentService.cs
 M app/FamilyClaimRef.App/Services/Storage/PolicyDocumentRegistrationRequest.cs
 M app/FamilyClaimRef.App/Services/UI/FilePickerResult.cs
 M app/FamilyClaimRef.App/Services/UI/WpfFilePickerService.cs
 M app/FamilyClaimRef.App/ViewModels/DocumentRegistrationViewModel.cs
 M app/FamilyClaimRef.App/Views/ProductDocumentRegistrationView.xaml.cs
 M tests/FamilyClaimRef.App.Tests/Composition/AppServicesTests.cs
 M tests/FamilyClaimRef.App.Tests/DocumentAttachmentCoordinatorTests.cs
 M tests/FamilyClaimRef.App.Tests/DocumentRegistrationViewModelTests.cs
 M tests/FamilyClaimRef.App.Tests/DocumentRegistrationWorkflowTests.cs
 M tests/FamilyClaimRef.App.Tests/IFileAttachmentServiceTests.cs
 M tests/FamilyClaimRef.App.Tests/Integration/DocumentRegistrationNegativeValidationTests.cs
 M tests/FamilyClaimRef.App.Tests/JsonDocumentStorageServiceTests.cs
 M tests/FamilyClaimRef.App.Tests/Services/Localization/ResourceUiTextProviderTests.cs
?? app/FamilyClaimRef.App/Services/Storage/DocumentFileValidationResult.cs
?? app/FamilyClaimRef.App/Services/Storage/DocumentFileValidationService.cs
?? app/FamilyClaimRef.App/Services/Storage/DocumentRegistrationException.cs
?? app/FamilyClaimRef.App/Services/Storage/StagedFileAttachment.cs
?? app/FamilyClaimRef.App/Startup/StartupDiagnosticSession.cs
?? docs/419_POLICY_CLAIM_PRODUCT_UI_SHELL_GATE8_REAL_DOCUMENT_REGISTRATION_IMPLEMENTATION_RESULT_REVIEW.md
?? docs/420_POLICY_CLAIM_PRODUCT_UI_SHELL_GATE8_U16_ACTUAL_REPARSE_POINT_TEST_REPAIR_INDEPENDENT_RECHECK.md
?? docs/421_POLICY_CLAIM_PRODUCT_UI_SHELL_GATE8_GUARDED_RUNTIME_UIA_MANUAL_VISUAL_REVIEW_RESULT_REVIEW.md
?? docs/422_POLICY_CLAIM_PRODUCT_UI_SHELL_GATE8_TRANSIENT_FILE_PICKER_UIA_SEMANTIC_HARNESS_REPAIR_AND_FULL_RUNTIME_RECHECK.md
?? docs/423_POLICY_CLAIM_PRODUCT_UI_SHELL_GATE8_NATIVE_PICKER_ACCESSIBILITY_ACTION_FALLBACK_AND_FULL_RUNTIME_RECHECK.md
?? docs/424_POLICY_CLAIM_PRODUCT_UI_SHELL_GATE8_DIALOG_ROOT_MSAA_DEFAULT_ACTION_AND_FULL_RUNTIME_RECHECK.md
?? docs/425_POLICY_CLAIM_PRODUCT_UI_SHELL_GATE8_TRANSIENT_BUSY_READ_ONLY_EVENT_TRACE_AND_REMAINING_RUNTIME_RECHECK.md
?? docs/426_POLICY_CLAIM_PRODUCT_UI_SHELL_GATE8_R07_OBSERVER_CAPABILITY_PREFLIGHT_AND_FINAL_RUNTIME_RECHECK.md
?? docs/427_POLICY_CLAIM_PRODUCT_UI_SHELL_GATE8_R07_PREFLIGHT_LIFETIME_REPAIR_AND_CONDITIONAL_FINAL_RUNTIME_RECHECK.md
?? docs/428_POLICY_CLAIM_PRODUCT_UI_SHELL_GATE8_R07_PRODUCT_STARTUP_RESIDUE_RECOVERY_AND_WINDOW_AVAILABILITY_DIAGNOSIS.md
?? docs/429_POLICY_CLAIM_PRODUCT_UI_SHELL_GATE8_PRODUCT_STARTUP_OBSERVABILITY_STATIC_ANALYSIS_AND_DECISION.md
?? docs/430_POLICY_CLAIM_PRODUCT_UI_SHELL_GATE8_MINIMAL_OPT_IN_STARTUP_INSTRUMENTATION_IMPLEMENTATION_AND_STATIC_VERIFICATION.md
?? docs/431_POLICY_CLAIM_PRODUCT_UI_SHELL_GATE8_STARTUP_INSTRUMENTATION_INDEPENDENT_RECHECK.md
?? docs/432_POLICY_CLAIM_PRODUCT_UI_SHELL_GATE8_STARTUP_INSTRUMENTATION_REPARSE_OWNERSHIP_AND_CONCURRENCY_REPAIR.md
?? tests/FamilyClaimRef.App.Tests/AppStartupObservabilityContractTests.cs
?? tests/FamilyClaimRef.App.Tests/DocumentFileValidationServiceTests.cs
?? tests/FamilyClaimRef.App.Tests/DocumentRegistrationLifecycleGate8Tests.cs
?? tests/FamilyClaimRef.App.Tests/Integration/DocumentRegistrationPersistenceGate8Tests.cs
?? tests/FamilyClaimRef.App.Tests/StartupDiagnosticSessionTests.cs
```

## 5. 보호 49-path identity manifest

재구성 규칙은 시작 53 paths에서 `docs/432`를 제외해 repair 시작
52 paths를 만든 뒤, repair 허용 3 files를 제외하는 것이다.

- `app/FamilyClaimRef.App/Startup/StartupDiagnosticSession.cs`
- `tests/FamilyClaimRef.App.Tests/StartupDiagnosticSessionTests.cs`
- `tests/FamilyClaimRef.App.Tests/AppStartupObservabilityContractTests.cs`

결과는 정확히 49 paths다. Directive reference content-manifest SHA-256은
`bf92b683544f143b220053e631170a27b167cd892451d1e5e5bc3a8b2ecfea70`다.
독립 검토에서는 `docs/432`의 mismatch count를 신뢰하지 않고 아래 각 파일을
직접 SHA-256 처리했다. 시작과 자동화 후 재검사 사이 per-path mismatch는
`0`이다.

| Protected path | SHA-256 |
|---|---|
| `app/FamilyClaimRef.App/App.xaml.cs` | `8bde248c6ac2650711e43c91a462d66d615509c14e4ca33b912b78835bfd78f9` |
| `app/FamilyClaimRef.App/Composition/AppServices.cs` | `320ed790af679468db2a6be7587447861ef90b7ca60b655fc25ff2d4578f3745` |
| `app/FamilyClaimRef.App/Models/Storage/DocumentDraft.cs` | `ed672d74c9b3b69e21128e71f4ec3f9cd93e6cd9ad842e618495c4cef8777ce6` |
| `app/FamilyClaimRef.App/Models/Storage/DocumentRecord.cs` | `6d91761679fa75f20c87b1aadfe5cc8fb55eca0a0e5171eaf0bc89600d177002` |
| `app/FamilyClaimRef.App/ProductShell/ProductShellWindow.xaml.cs` | `52c8a83cb639d73b38c0ad4064a3d976bdffec3d506ace39c4e4ff3f12028dfc` |
| `app/FamilyClaimRef.App/Resources/UiStrings.xaml` | `1a980c34a1257bf5f306894e2d289e2a6e0b9a91d4ff3048a4afc5f83565f3ee` |
| `app/FamilyClaimRef.App/Services/Localization/UiTextKeys.cs` | `79255f817884164f86852d843e825da515ca6df917b87c35cd034ae3e5643be8` |
| `app/FamilyClaimRef.App/Services/Storage/ClaimDocumentRegistrationRequest.cs` | `8ba87412b7f366325dd8483bd9bbdc424eeb4624bb35110fc5af621a3a6a2d81` |
| `app/FamilyClaimRef.App/Services/Storage/DocumentAttachmentCoordinator.cs` | `98d3f70f702275f042ab973a25e070279957b7cd374107f02336975ba4b207e2` |
| `app/FamilyClaimRef.App/Services/Storage/DocumentAttachmentRequest.cs` | `b26c103c4a2ddb92173e2a8944b99ab982ec51f51bdbd11d9c93028c7f1039c7` |
| `app/FamilyClaimRef.App/Services/Storage/DocumentFileValidationResult.cs` | `5969bdacebf3bec8f06eee88fbd572b751086acdd4de482ae70aac86c362e6f2` |
| `app/FamilyClaimRef.App/Services/Storage/DocumentFileValidationService.cs` | `230cfc80863f1a9e567505bb292b74a26443b5bb5cb90e7e728e7e5aadcf4a47` |
| `app/FamilyClaimRef.App/Services/Storage/DocumentRegistrationException.cs` | `5c9df0a8a4c40c50d3be81038b3019c244840f0148de3debe2496b1771fb1d59` |
| `app/FamilyClaimRef.App/Services/Storage/DocumentRegistrationWorkflow.cs` | `4a998f9dac19f86617a86560130ca83fd9396831b095f412f29dee5530ea0c07` |
| `app/FamilyClaimRef.App/Services/Storage/FileAttachmentCopyResult.cs` | `ca10d7c6fd9cd046fcc33ac6a6bb9a72c293ffb2c503e8a97232ba0d5c2bc498` |
| `app/FamilyClaimRef.App/Services/Storage/IDocumentStorageService.cs` | `f2aa7a9b5fdcefd2a8b2dcee4fe50aa22e948086e6860667e064e3028822bf43` |
| `app/FamilyClaimRef.App/Services/Storage/IFileAttachmentService.cs` | `cc570c306392753d6f8688a708976d2919cacea913a0aa250947e276eb150846` |
| `app/FamilyClaimRef.App/Services/Storage/JsonDocumentStorageService.cs` | `3b07e307b7fc0225139930d296979266a2d881f39fb5e39aff6bcb4f7344a986` |
| `app/FamilyClaimRef.App/Services/Storage/LocalFileAttachmentService.cs` | `558788bbff12bfdb78183d53430117592fe78d49254df4e65167df649d9fe14b` |
| `app/FamilyClaimRef.App/Services/Storage/PolicyDocumentRegistrationRequest.cs` | `9c562164cac34c7974e9ea04f7d5b6a003f25ceaa353a8bd500a00e197b95aaa` |
| `app/FamilyClaimRef.App/Services/Storage/StagedFileAttachment.cs` | `4b4d976e1e4269a506cfd42cfacd64a86f7a2931141e906237fd7c33af600449` |
| `app/FamilyClaimRef.App/Services/UI/FilePickerResult.cs` | `b855da4cf8e7e00f4d92045c403c9336dbaf998017f3e4acd9181b6fbfb46ee6` |
| `app/FamilyClaimRef.App/Services/UI/WpfFilePickerService.cs` | `1d51770ea16c26e9f684e8bb9bfaaa31e82ce63c0e0072cf8165f3725e888c11` |
| `app/FamilyClaimRef.App/ViewModels/DocumentRegistrationViewModel.cs` | `b6b49c2d330c59bb4e2173a0441243a8d98e394525657b136e40f01fecd9e4c0` |
| `app/FamilyClaimRef.App/Views/ProductDocumentRegistrationView.xaml.cs` | `bd8ff5f6e862d7e65e573fd0c20e03f6dff1e6d737dc886939a4cab1ef3f7ee6` |
| `docs/419_POLICY_CLAIM_PRODUCT_UI_SHELL_GATE8_REAL_DOCUMENT_REGISTRATION_IMPLEMENTATION_RESULT_REVIEW.md` | `b81b76fe43bef81142db1beb30c930b939773993a26a3a83d24a500d97a73506` |
| `docs/420_POLICY_CLAIM_PRODUCT_UI_SHELL_GATE8_U16_ACTUAL_REPARSE_POINT_TEST_REPAIR_INDEPENDENT_RECHECK.md` | `fc7101ce4347d19178edbfbb0920e42eedfdbb7ba30848516a0b688ff8d24001` |
| `docs/421_POLICY_CLAIM_PRODUCT_UI_SHELL_GATE8_GUARDED_RUNTIME_UIA_MANUAL_VISUAL_REVIEW_RESULT_REVIEW.md` | `1056ad68f56cd1e89b618a9ba3207f36181eb781e81db79cff1ad306346533db` |
| `docs/422_POLICY_CLAIM_PRODUCT_UI_SHELL_GATE8_TRANSIENT_FILE_PICKER_UIA_SEMANTIC_HARNESS_REPAIR_AND_FULL_RUNTIME_RECHECK.md` | `c287436daee816ae6aab61c36593a6f7c2956a26ed89d1ede4280a43068b023a` |
| `docs/423_POLICY_CLAIM_PRODUCT_UI_SHELL_GATE8_NATIVE_PICKER_ACCESSIBILITY_ACTION_FALLBACK_AND_FULL_RUNTIME_RECHECK.md` | `f14220edc2a23742055ef5fa00fe560a1278f87f1d5fe6a36298bc14f6c3a51c` |
| `docs/424_POLICY_CLAIM_PRODUCT_UI_SHELL_GATE8_DIALOG_ROOT_MSAA_DEFAULT_ACTION_AND_FULL_RUNTIME_RECHECK.md` | `20fb193928947611e4be143632b9c14becdfdca23348a08e21e0823a7feec222` |
| `docs/425_POLICY_CLAIM_PRODUCT_UI_SHELL_GATE8_TRANSIENT_BUSY_READ_ONLY_EVENT_TRACE_AND_REMAINING_RUNTIME_RECHECK.md` | `b57500a3d9c70e25c5254359fbf36dbabcf7e0bda119fc3c8f8a653c7e984192` |
| `docs/426_POLICY_CLAIM_PRODUCT_UI_SHELL_GATE8_R07_OBSERVER_CAPABILITY_PREFLIGHT_AND_FINAL_RUNTIME_RECHECK.md` | `7ee4b05f86159f1bc1a0d75bc9044cfa6e3037fa66e224c87491ae76b0b10f13` |
| `docs/427_POLICY_CLAIM_PRODUCT_UI_SHELL_GATE8_R07_PREFLIGHT_LIFETIME_REPAIR_AND_CONDITIONAL_FINAL_RUNTIME_RECHECK.md` | `b118ff961f144c2e0c41aef83467d858f307a2f63a9aa3496d72cccdc4caf702` |
| `docs/428_POLICY_CLAIM_PRODUCT_UI_SHELL_GATE8_R07_PRODUCT_STARTUP_RESIDUE_RECOVERY_AND_WINDOW_AVAILABILITY_DIAGNOSIS.md` | `a62b8ff361bde2173277efbd1c2aaf17766ffd3b312ba2ba4d8cc93c68418485` |
| `docs/429_POLICY_CLAIM_PRODUCT_UI_SHELL_GATE8_PRODUCT_STARTUP_OBSERVABILITY_STATIC_ANALYSIS_AND_DECISION.md` | `8e0e1606f37ad9c1732d1d9259ebd137c40e2e10af928f9959f2e82aa2db2b9b` |
| `docs/430_POLICY_CLAIM_PRODUCT_UI_SHELL_GATE8_MINIMAL_OPT_IN_STARTUP_INSTRUMENTATION_IMPLEMENTATION_AND_STATIC_VERIFICATION.md` | `e56dc6a53d8bd58a325f3cb1ab973527bdc281b0fa4ca09a3bd1ecd806201db2` |
| `docs/431_POLICY_CLAIM_PRODUCT_UI_SHELL_GATE8_STARTUP_INSTRUMENTATION_INDEPENDENT_RECHECK.md` | `724b6701fb8bb8ce6e6dd624cce6f9463109c9d7fa069e16f82f03d7c64d0933` |
| `tests/FamilyClaimRef.App.Tests/Composition/AppServicesTests.cs` | `9a5905425cfe7a036a5b3625c13c7b752242adbda767b3fb3e10ab9acab7b4d1` |
| `tests/FamilyClaimRef.App.Tests/DocumentAttachmentCoordinatorTests.cs` | `e798b6064249ec6731a4edacd69f0c552c96765d528bd087c54cf87b79d7a6ee` |
| `tests/FamilyClaimRef.App.Tests/DocumentFileValidationServiceTests.cs` | `ec55a7e3d1ebc9e8f5625ed628ea90914057d3fe8bab08a2772047ac8ff37431` |
| `tests/FamilyClaimRef.App.Tests/DocumentRegistrationLifecycleGate8Tests.cs` | `3ce0166d83eb26703041a349d6f2f838cb249f32fcc453a914dcade26bdcf49a` |
| `tests/FamilyClaimRef.App.Tests/DocumentRegistrationViewModelTests.cs` | `21833fadd785d15f10cce4a27f96b46a0cec3fa6ca20db18266500b2eafc5f17` |
| `tests/FamilyClaimRef.App.Tests/DocumentRegistrationWorkflowTests.cs` | `77df19e5eb8c6e9f4f4d7e651547265491a3c40a86886e42802b1542f46c152d` |
| `tests/FamilyClaimRef.App.Tests/IFileAttachmentServiceTests.cs` | `a4d1e0a39be6ef417559f47d3851f173a8d11d32dc63ace38b1759856a3b1fb3` |
| `tests/FamilyClaimRef.App.Tests/Integration/DocumentRegistrationNegativeValidationTests.cs` | `96dd8ade7a5c11fc31f3ec73a59e319bc47af2192cdece87982dd572095fbb93` |
| `tests/FamilyClaimRef.App.Tests/Integration/DocumentRegistrationPersistenceGate8Tests.cs` | `c03015b1b61f7664d1321223fa57d0651f33bc642857e1c3bb316fcb8c2a6337` |
| `tests/FamilyClaimRef.App.Tests/JsonDocumentStorageServiceTests.cs` | `a3044d9727f110ffc3c9c1942cf0a9bbd55b904e6ece953b9ffa0897158a1064` |
| `tests/FamilyClaimRef.App.Tests/Services/Localization/ResourceUiTextProviderTests.cs` | `5ee63e2f7392d73d1148945f25ac7a439e976a9ca59b8c2f8ea3a5176fe226ae` |

## 6. Pre-existing root 및 activation

| Contract | Source/test evidence | Disposition |
|---|---|---|
| Default OFF, ordinal exact `1` | `string.Equals(enableValue, "1", StringComparison.Ordinal)`; 5 disabled theory cases | 충족 |
| Windows gate | `OperatingSystem.IsWindows()` 이전에는 file open 없음 | 충족 |
| Fully-qualified path | `Path.IsPathFullyQualified` | 충족 |
| Strict child | normalized `%TEMP%\FamilyClaimRef\StartupDiagnostics` exact path는 거부하고 separator boundary child만 허용 | 충족 |
| Missing root | `Directory.Exists` false이면 disabled; `Valid_missing_root_disables_without_creating_directory_or_file` | 충족 |
| File root | `Directory.Exists` false; `Existing_file_root_disables_diagnostics_and_preserves_file` | 충족 |
| Prefix-like/non-TEMP/different drive | ordinal-ignore-case separator boundary가 false; non-TEMP behavioral case | 충족 |
| Relative/UNC/SUBST/device/unsupported final form | normalization 또는 final-handle equality를 입증하지 못하면 disabled | fail-closed |
| Product directory/file mutation | `Directory.CreateDirectory`, `Directory.Delete`, `File.Delete` source count `0/0/0` | 충족 |
| Raw environment value logging | environment values는 activation/root 입력으로만 사용하고 record field로 전달하지 않음 | 충족 |
| Invalid/setup failure no-throw | setup 전체 catch가 disabled session을 반환 | 충족 |

`Path.GetFullPath`와 pathname `Directory.Exists`는 단독 승인 근거가 아니다.
최종 승인은 아래 component handle validation 및 log handle validation에 의존한다.

## 7. Win32 선언, numeric constant, 실제 인자

| Item | Actual declaration/value | 검토 결과 |
|---|---|---|
| `FILE_READ_ATTRIBUTES` | `0x00000080` | 정확 |
| `OPEN_EXISTING` | `3` | 정확 |
| `FILE_FLAG_BACKUP_SEMANTICS` | `0x02000000` | 정확 |
| `FILE_FLAG_OPEN_REPARSE_POINT` | `0x00200000` | 정확 |
| `FileAttributeTagInfo` class | `9` | 정확 |
| `FILE_SHARE_READ` | `1` | 정확 |
| `FILE_SHARE_WRITE` | `2` | 정확 |
| directory share mask | `1 | 2 = 3` | delete bit `4` 없음 |
| `GetFinalPathNameByHandleW` flags | `0` | normalized DOS path 요청 |
| `CreateFileW` | `kernel32.dll`, Unicode, `ExactSpelling=true`, `SetLastError=true`, `SafeFileHandle` return | 정확 |
| `GetFileInformationByHandleEx` | `kernel32.dll`, `SetLastError=true`, no string marshaling, method-name entrypoint | 정확 |
| `GetFinalPathNameByHandleW` | `kernel32.dll`, Unicode, `ExactSpelling=true`, `SetLastError=true` | 정확 |
| `FILE_ATTRIBUTE_TAG_INFO` layout | sequential `FileAttributes` + `uint ReparseTag`, size passed by `Marshal.SizeOf` | 정확 |

각 directory component의 실제 `CreateFileW` 인자:

| Parameter | Actual |
|---|---|
| `fileName` | 현재 normalized component path |
| `desiredAccess` | `0x00000080` |
| `shareMode` | `FileShare.Read | FileShare.Write`, numeric `3` |
| `securityAttributes` | `IntPtr.Zero` |
| `creationDisposition` | `3` |
| `flagsAndAttributes` | `0x02000000 | 0x00200000 = 0x02200000` |
| `templateFile` | `IntPtr.Zero` |

`FILE_SHARE_DELETE`, 관리자 권한, backup privilege, retry/background keeper는
사용하지 않는다. 반환 `SafeFileHandle`은 P/Invoke marshaller가 소유 handle로
생성하고 invalid handle은 즉시 `Dispose`한다.

## 8. Component pinning, validation, lifetime

| 순서/component | Acquisition | Handle verification | Lifetime |
|---|---|---|---|
| 1. normalized OS TEMP root | `CreateFileW` | directory bit set, reparse bit clear, final DOS path exact | session `Dispose`까지 |
| 2. `FamilyClaimRef` | same | same | session `Dispose`까지 |
| 3. `StartupDiagnostics` | same | same | session `Dispose`까지 |
| 4. strict-child의 각 segment | TEMP 기준 relative path를 순서대로 누적 open | same | session `Dispose`까지 |
| 5. requested leaf | 마지막 누적 component | same | log stream close 이후까지 |

`TryBuildComponentPaths`는 `List<string> { tempRoot }`로 시작하고
`Path.GetRelativePath(tempRoot, normalizedRoot)`의 모든 non-empty segment를
하나씩 추가한다. 따라서 TEMP root부터 requested leaf까지 누락 component는
`0`이다. 각 handle은 `FILE_FLAG_OPEN_REPARSE_POINT`로 component 자체를 열고,
pathname `File.GetAttributes`로 승인하지 않는다.

`TryNormalizeNativeFinalPath`는 `\\?\` DOS prefix만 받아들이고 `\\?\UNC\`는
거부한다. prefix 제거 후 fully-qualified DOS path를 요구한다. Volume GUID,
device, UNC, SUBST alias처럼 expected normalized component와 final path가
ordinal-ignore-case exact equality가 아닌 형식은 disabled다.

소유권 흐름:

1. local `acquiredHandles`
2. 성공 시 `DirectoryHandleLease`
3. `StartupDiagnosticSession.directoryLease`
4. log `CreateNew`
5. log handle final-path 검증
6. 전체 active session 및 모든 `Record`
7. `Dispose`에서 log stream close
8. directory handles leaf-to-TEMP reverse close

writer/stream보다 directory lease를 먼저 닫는 경로는 없다.

## 9. Partial-failure dispose 분석

| Failure point | Current handle | Previously acquired handles | Stream | Result |
|---|---|---|---|---|
| component path build 실패 | 없음 | 없음 | 없음 | false |
| `CreateFileW` invalid | invalid handle 즉시 close | `finally`에서 reverse close | 없음 | false |
| directory attribute/final-path 실패 | current close | `finally`에서 reverse close | 없음 | false |
| lease 완성 | list ownership을 lease로 이동하고 local을 null | lease 소유 | 없음 | success |
| log `CreateNew` 실패 | N/A | lease catch close | 생성 실패 | disabled |
| log validator false | N/A | stream close 후 lease close | 빈/부분 residue 허용 | disabled |
| session constructor 예외 | N/A | catch close | catch close | disabled |
| 정상 `Dispose` | N/A | stream 이후 reverse close | 먼저 close | no-throw |

`acquiredHandles = null`, `stream = null`, `lease = null` ownership transfer로
성공 및 명시 실패 경로의 중복 cleanup을 피한다. 실패 시 pathname을 다시
찾아 삭제하지 않으며 열린 session-owned handle만 닫는다. static/global
mutable lease와 `SafeFileHandle` leak 경로는 확인되지 않았다.

## 10. Log file identity 및 ownership

| Contract | Actual source | Result |
|---|---|---|
| Filename | `startup.ndjson` | 충족 |
| Create mode | `FileMode.CreateNew` | existing overwrite 0 |
| Access/share | `FileAccess.Write`, `FileShare.Read` | write/delete sharing 없음 |
| Handle checked | 생성된 `FileStream.SafeFileHandle` | 실제 handle |
| Attribute | directory/reparse bit 모두 거부 | 충족 |
| Final identity | `<requestedRoot>\startup.ndjson`와 ordinal-ignore-case exact final DOS path | 충족 |
| Validation timing | 첫 record 전에 완료 | 충족 |
| Failure cleanup | stream/lease close만 수행 | pathname delete 0 |
| Normal dispose | log 보존 | evidence file 삭제 0 |

`rootExisted`, `logFileCreatedBySession`, `TryDeleteNewEmptyDirectory`,
`TryDeleteOwnedLogFile`, `TryDelete` 계열은 source에 없다.
setup이 log 생성 뒤 실패하면 빈 파일 또는 부분 파일 residue를 보존한다.
이는 source와 `docs/432` 최신 계약이 일치한다.

`Setup_failure_preserves_competitor_sentinel_and_owned_log_residue`는 instance-local
validator `(_, _) => false`로 log 생성 이후 validation failure를 유도한다.
competitor directory/file path와 exact bytes를 유지하고 session-created
`startup.ndjson` 길이 `0` residue를 확인한다. test-only cleanup은 `finally`의
`DeleteExactTestRoot`이며 Product compensation이 아니다. process-global mutable
test hook은 없다.

Pathname compensation static search:

| Pattern | Product instrumentation source count |
|---|---:|
| `Directory.CreateDirectory` | 0 |
| `Directory.Delete` | 0 |
| `File.Delete` | 0 |
| `FileShare.Delete` | 0 |
| `TryDelete` | 0 |
| `rootExisted` | 0 |
| `logFileCreatedBySession` | 0 |

## 11. Behavioral test 이름과 요구사항 매핑

Targeted suites는 exact 37 discovered cases다. `Skip =` count는 `0`,
`Thread.Sleep`/`Task.Delay` count는 `0`이다. 두 reparse case는 실제
`Directory.CreateSymbolicLink`를 호출한다.

| Requirement | Exact test name | Evidence |
|---|---|---|
| ordinal exact activation, default OFF | `Disabled_enable_values_create_no_directory_file_or_handlers` | 5 theory cases |
| null root no-throw | `Missing_root_disables_diagnostics_without_throwing` | behavioral |
| valid missing root, directory/file delta 0 | `Valid_missing_root_disables_without_creating_directory_or_file` | behavioral |
| relative/empty fail-closed | `Relative_or_empty_root_disables_diagnostics_without_artifacts` | 2 theory cases |
| non-TEMP fail-closed | `Non_temp_root_disables_diagnostics_without_creating_it` | behavioral |
| shared allowed root reject | `Allowed_area_itself_is_not_an_isolated_run_root` | behavioral |
| file root preserve | `Existing_file_root_disables_diagnostics_and_preserves_file` | content 및 single-entry 확인 |
| existing log no overwrite | `Existing_log_file_disables_diagnostics_without_overwrite` | content 및 single-entry 확인 |
| normalized child accepted | `Normalized_parent_segment_inside_allowed_area_is_accepted` | pre-existing root enabled |
| actual leaf symbolic link reject | `Reparse_point_root_disables_diagnostics` | actual Windows filesystem |
| actual ancestor symbolic link reject | `Ancestor_reparse_point_disables_diagnostics` | actual Windows filesystem |
| live leaf rename/replacement sequence block, post-dispose control | `Directory_lease_blocks_leaf_rename_and_replacement_until_dispose` | actual Windows filesystem |
| live ancestor rename block, post-dispose control | `Directory_lease_blocks_ancestor_rename_until_dispose` | actual Windows filesystem |
| post-create validation failure ownership | `Setup_failure_preserves_competitor_sentinel_and_owned_log_residue` | actual residue/sentinel |
| enabled, parseable, immediate flush | `Enabled_session_creates_one_parseable_immediately_flushed_log` | shared live reader |
| privacy | `Privacy_sensitive_inputs_and_exception_message_are_not_logged` | synthetic path/document/claim/message absence |
| 128 KiB bound | `Log_is_bounded_and_all_written_records_remain_parseable` | actual bytes and JSON |
| concurrent Record | `Concurrent_record_calls_produce_parseable_monotonic_bounded_ndjson` | 8 tasks, 160 calls |
| Record/Dispose race | `Record_and_dispose_race_is_no_throw_parseable_and_length_stable` | barrier, 8 writers + disposer |
| handler idempotence/detach | `Handler_registration_is_idempotent_and_dispose_detaches` | fake registrar behavioral |
| post-dispose no-op | `Record_after_dispose_is_no_throw_and_creates_no_new_record` | length/content stable |
| startup order/rethrow | `App_preserves_existing_startup_order_and_rethrows_exceptions` | static source contract, independent review 보강 |
| one activation/register owner | `App_constructor_owns_activation_and_one_handler_registration_call` | static source contract |
| public constructor retained | `Product_shell_retains_public_one_argument_constructor` | reflection |
| enabled-only lifecycle/dispatcher | `Product_shell_diagnostic_events_and_dispatcher_are_enabled_only` | static source contract |
| exception observation semantics | `Runtime_handlers_observe_without_changing_exception_semantics` | static negative contract |
| no background/Product launch | `Instrumentation_has_no_background_or_product_launch_mechanism` | static negative contract |
| no storage/registration coupling | `Instrumentation_does_not_reference_storage_or_registration_owners` | static negative contract |
| generated WPF entrypoint | `App_xaml_and_project_keep_generated_entrypoint_contract` | XAML/project contract |
| `AppServices` isolation | `App_services_owner_remains_unmodified_by_instrumentation` | static negative contract |
| environment read-only | `Diagnostic_configuration_reads_environment_without_writing_it` | exact read/write source count |
| handle-pinned pre-existing storage | `Diagnostic_storage_uses_preexisting_handle_pinned_root_without_pathname_compensation` | static contract, independent source review 보강 |

## 12. Rename/replacement causal analysis

`Directory_lease_blocks_leaf_rename_and_replacement_until_dispose`는 test-owned
`root`, `displacedRoot`, `competitorRoot`를 먼저 생성한다.

- active session에서 `Directory.Move(root, displacedRoot)`가 실패한다.
- source에서 leaf directory handle은 share mask `3`으로 열려 있고 delete-share
  bit `4`가 없다.
- `session.Dispose()` 후 동일 `Directory.Move(root, displacedRoot)`가 성공한다.
- 이어서 `Directory.Move(competitorRoot, root)`가 성공하고 competitor sentinel이
  새 root에서 그대로 확인된다.

따라서 permission 부족 또는 본래 invalid source path가 원인이 아니다.
다만 active session의 두 번째 단독 assertion
`Directory.Move(competitorRoot, root)`는 첫 rename이 lease로 차단되어
`root`가 계속 존재하므로 target-exists 조건도 갖는다. Replacement sequence의
실패 원인은 leased root를 먼저 displaced할 수 없다는 점이며, 두 번째 move
자체를 별도의 lease-block 증거로 과대 해석하지 않았다. Post-dispose의 같은
test-owned full sequence 성공이 대조군이다.

`Directory_lease_blocks_ancestor_rename_until_dispose`는 active session에서
test-owned ancestor move가 실패하고 `Dispose` 뒤 같은 move가 성공한다.
두 test 모두 sleep, retry-until-success, skip이 없다. Actual source review로
TEMP root부터 leaf까지 각 directory handle의 delete-share 부재와 session
lifetime 소유를 별도로 확인했다.

## 13. Concurrent Record 및 Record/Dispose

Source의 단일 `lock (sync)` 안에 다음 전체 sequence가 있다.

1. `disposed || writeStopped` 검사
2. `nextSequence` 계산
3. allowlist normalization 및 UTF-8 JSON payload 생성
4. payload + LF byte 계산
5. `MaximumLogFileBytes` 검사
6. payload/LF write
7. `Flush(flushToDisk: true)`
8. sequence commit

`Dispose`도 같은 lock 안에서 detach, `disposed = true`, log stream close,
directory lease close를 수행한다. 진행 중인 `Record`가 lock을 먼저 얻으면
flush/commit 후 dispose되고, `Dispose`가 먼저 얻으면 이후 `Record`는 no-op이다.

Behavioral evidence:

| Check | Result |
|---|---|
| Concurrent workers | 8 tasks x 20 calls, 동일 session |
| Concurrent caller exception | 0 |
| Records | 160 |
| JSON parse failure | 0 |
| Sequence | `1..160`, strict consecutive, duplicate 0 |
| File bytes | `<= 131072` |
| Record/Dispose start | shared `ManualResetEventSlim` barrier |
| Race writers/disposer | 8 writers x 40 calls + 1 disposer |
| Race caller exception/ODE propagation | `0/0` |
| Race final NDJSON parse | 통과 |
| Race sequence | starts at 1, consecutive, duplicate 0 |
| Post-dispose Record | no-op |
| Post-dispose file length | unchanged |
| Timing sleep/retry race loop | 0 |
| Background retry/write | source 0 |

## 14. Detach, privacy, size, startup semantics

`RuntimeStartupDiagnosticEventRegistrar.Detach`는 다음 순서로 호출하지만 각
method가 독립 `try/catch/finally` 경계다.

1. `TryDetachTaskScheduler`
2. `TryDetachDispatcher`
3. `TryDetachAppDomain`

각 method는 own attached flag/handler만 처리하고 실패해도 finally에서 state를
clear한다. 첫 unsubscribe failure가 다음 detach call을 차단하지 않는다.

Exception semantics:

- `DispatcherUnhandledException.Handled` 변경 0
- `UnobservedTaskException.SetObserved()` 호출 0
- `AppServices.CreateDefault`, outer `OnStartup`, `OnExit`,
  `ProductShellWindow` constructor catch는 `throw;`
- fallback/suppression/replacement startup 0

Privacy:

- raw `e.Args`는 selector 입력으로만 사용하고 log하지 않음
- raw environment value 기록 0
- `exception.Message`, `ToString`, stack trace 기록 0
- filesystem path, local profile, document/attachment/claim-like input 기록 0
- owner/milestone/phase/result/method는 allowlist 밖에서 fixed value 또는 null
- exception type 및 HResult만 exception에서 기록
- JSON serializer가 제어문자/newline을 구조적으로 escape

Size:

- UTF-8 serialized payload byte length + LF 1 byte로 계산
- actual stream position 기준 `131072` 초과 write를 시작하지 않음
- limit 도달 후 `writeStopped = true`, retry/background write 0

Startup order:

1. `base.OnStartup(e)`
2. `StartupWindowModeSelector.Select(e.Args)`
3. `AppServices.CreateDefault()`
4. `ProductShellWindow` construction
5. `MainWindow = selectedWindow`
6. `selectedWindow.Show()`
7. post-Show dispatcher observation schedule

Diagnostics OFF에서는 disabled session의 `Record`가 즉시 return하고
`ProductShellWindow`가 diagnostics field를 보유하지 않는다. 추가 runtime
handler, directory/file, dispatcher/background work는 `0`이다.

## 15. 독립 automation 원문 요약

Directive 순서로 Product process precheck, build, targeted, full, Product
process postcheck, TEMP residue, `git diff --check`, exact scope/content audit를
실행했다.

첫 sandbox build와 첫 sandbox targeted/full test는 다음 toolchain discovery
read denial이었다.

```text
error MSB4184: "[Microsoft.Build.Utilities.ToolLocationHelper]::GetPlatformSDKDisplayName(Windows, 7.0)" 식을 계산할 수 없습니다.
Access to the path 'C:\Users\jin8855\AppData\Local\Microsoft SDKs' is denied.
```

Product 실행으로 대체하지 않았다. 동일한 permitted command를 Windows SDK
discovery path를 읽을 수 있는 실행 경계에서 재실행했다.

Build command:

```text
dotnet build .\FamilyClaimRef.sln --nologo --verbosity minimal
```

Final raw summary:

```text
빌드했습니다.
    경고 0개
    오류 0개
```

Targeted command:

```text
dotnet test .\tests\FamilyClaimRef.App.Tests\FamilyClaimRef.App.Tests.csproj --no-build --nologo --verbosity minimal --filter "FullyQualifiedName~StartupDiagnosticSessionTests|FullyQualifiedName~AppStartupObservabilityContractTests"
```

Targeted raw summary:

```text
통과!  - 실패: 0, 통과: 37, 건너뜀: 0, 전체: 37
```

Full command:

```text
dotnet test .\FamilyClaimRef.sln --no-build --nologo --verbosity minimal
```

Full raw summary:

```text
통과!  - 실패: 0, 통과: 523, 건너뜀: 0, 전체: 523
```

| Validation | Final result |
|---|---|
| Build warnings/errors | `0/0` |
| Targeted passed/total | `37/37` |
| Targeted failed/skipped | `0/0` |
| Full passed/total | `523/523` |
| Full failed/skipped | `0/0` |
| Product process before/after | `0/0` |
| Product launch | `0` |
| WPF top-level window creation | `0` |
| Product nonzero main-window handles before/after | `0/0` |
| Test TEMP recursive residue entries | `0` |
| `git diff --check` | exit `0`; line-ending warnings only |

Test TEMP audit path:

`C:\Users\jin8855\AppData\Local\Temp\FamilyClaimRef\StartupDiagnostics\tests`

Directory 자체는 존재했고 recursive child entry count는 `0`이었다.

## 16. Generated binary identity

Final build 산출물:

| Artifact | Bytes | SHA-256 | Classification |
|---|---:|---|---|
| `app/FamilyClaimRef.App/bin/Debug/net10.0-windows/FamilyClaimRef.App.exe` | 162816 | `bb3de775939bbeb06aa9abe42e9e93cee51881084b3e6f20e7293a2d23300c39` | generated build output only |
| `app/FamilyClaimRef.App/bin/Debug/net10.0-windows/FamilyClaimRef.App.dll` | 318976 | `935c5c3c19db57deff3e109f912d4d3b48b80dfd9e81bb129130ea869bb4896c` | generated build output only |

두 binary를 실행하지 않았고 runtime evidence로 승격하지 않았다.

## 17. F-01 - F-05 disposition

| Finding | Severity | Independent disposition | Evidence |
|---|---|---|---|
| F-01 pathname check/use race | HIGH | `CLOSED_CONFIRMED` | 모든 component handle pin, handle attribute/final path, log handle final identity, lease lifetime |
| F-02 pathname compensation ownership | HIGH | `CLOSED_CONFIRMED` | pathname create/delete compensation 제거, open handle ownership 및 residue preservation |
| F-03 concurrent Record/Dispose evidence | MEDIUM | `CLOSED_CONFIRMED` | 8-task Record 및 barrier Record/Dispose behavioral tests, source lock review |
| F-04 reparse/race evidence | MEDIUM | `CLOSED_CONFIRMED` | actual leaf/ancestor symlink, leaf/ancestor rename controls, post-dispose success |
| F-05 WPF runtime lifecycle evidence | LOW | `DEFERRED_NOT_AUTHORIZED` | source/static evidence retained; Product runtime 미실행 |

F-05 exact retained state:

- App/ProductShell actual runtime lifecycle: `NOT_EXECUTED`
- source/static evidence: `RETAINED`
- implementation defect: `NOT_CONFIRMED`
- independent runtime verification: `DEFERRED, NOT_AUTHORIZED`

## 18. 독립 findings와 severity

| ID | Severity | Classification | Finding |
|---|---|---|---|
| R-01 | INFO | `EVIDENCE_INTERPRETATION` | Replacement test의 두 번째 live move는 target-exists 조건도 있으므로 단독 lease 증거로 사용하지 않았다. 첫 leased-root rename failure, source delete-share 검토, post-dispose 동일 full sequence success를 합쳐 원인을 판정했다. |
| R-02 | LOW | `AUTHORIZED_EVIDENCE_GAP` | F-05 actual WPF/Product runtime lifecycle은 이번 directive에서 금지되어 계속 미검증이다. 구현 결함으로 확인되지 않았고 독립 runtime 승인이 아니다. |

HIGH/MEDIUM repair defect, component omission, early lease dispose, delete sharing,
pathname compensation, ownership violation, malformed concurrency output,
size/privacy/detach/startup-order regression은 확인되지 않았다.

## 19. Exact scope 및 최종 Git gate

`docs/433` 생성 전 자동화 후 audit:

- tracked/staged/untracked: `29/0/24`
- status entries: `53`
- starting 53-path set SHA-256: unchanged
  `2a99daacb2e33b4336a1992d3bc23ffa12dcacd6f50a6feebbd5ff978540621d`
- protected 49 per-path hash mismatch: `0`
- repair 3 identities: 시작 expected와 동일
- deletion/rename: `0/0`
- project/solution status entry: `0`
- `App.xaml` status entry: `0`
- `ProductShellWindow.xaml` status entry: `0`

`docs/433` 생성 후 최종 상태:

| Item | Final |
|---|---|
| Branch/HEAD | `main` / `aecf7edfd43b4124ec5ff17d35687020cf4c0d90` |
| Tracked/staged/untracked | `29/0/25` |
| Status entries | `54` |
| Existing starting paths | `53/53`, content mismatch `0` |
| New repository path | `docs/433_POLICY_CLAIM_PRODUCT_UI_SHELL_GATE8_STARTUP_INSTRUMENTATION_REPAIR_INDEPENDENT_RECHECK.md` only |
| Source/test/XAML/resource/project additional delta | `0/0/0/0/0` |
| App/ProductShell additional delta | `0/0` |
| Runtime-root/storage/registration/persistence additional delta | `0` |
| docs/413-432 additional delta | `0` |
| Deletion/rename | `0/0` |
| Stage/commit/push | `0/0/0` |
| `git diff --check` | PASS |

## 20. Retained boundary

- docs/428 runtime cause: `UNRESOLVED`
- Guarded runtime functional review: `NOT_COMPLETED`
- Product runtime retry: `NOT_AUTHORIZED`
- Diagnostic Product startup: `NOT_AUTHORIZED`
- Deployment/production readiness: `NOT_AUTHORIZED`
- Stage/commit: `NOT_AUTHORIZED`

다음 단계는 별도 새 directive에서 격리 root 준비, 정확히 1회 diagnostic
Product startup, process/window/log 수집, 종료 및 residue 경계를 검토한 뒤
명시적으로 승인하거나 HOLD해야 한다. 이번 독립 판정은 그 실행을 승인하지
않는다.

## 21. 판정

`POLICY_CLAIM_PRODUCT_UI_SHELL_GATE8_STARTUP_INSTRUMENTATION_REPAIR_INDEPENDENT_RECHECK_PASS_DIAGNOSTIC_RUNTIME_STILL_NOT_AUTHORIZED`
