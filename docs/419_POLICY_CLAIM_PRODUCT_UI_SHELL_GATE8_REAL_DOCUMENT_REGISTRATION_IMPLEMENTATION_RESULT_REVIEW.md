# Gate 8 Real Document Registration Implementation Result Review

## A. Status

- Status: `IMPLEMENTATION_RESULT_REVIEW_WITH_U16_REPAIR_AUTOMATED_VALIDATION_PASS`
- Current marker: `POLICY_CLAIM_PRODUCT_UI_SHELL_GATE8_U16_ACTUAL_REPARSE_POINT_TEST_REPAIR_STATIC_AND_AUTOMATED_VALIDATION_PASS_INDEPENDENT_RECHECK_REQUIRED`
- Original implementation-batch marker: `POLICY_CLAIM_PRODUCT_UI_SHELL_GATE8_REAL_DOCUMENT_REGISTRATION_IMPLEMENTATION_STATIC_AND_AUTOMATED_VALIDATION_PASS_RUNTIME_REVIEW_PENDING`
- Source implementation: `IMPLEMENTED; U16 ACTUAL REPARSE EXECUTION VALIDATED`
- Independent source/test recheck: `HISTORICAL_HOLD; Blocking 1 / Major 0 / Minor 0`
- Independent repair recheck: `REQUIRED_NOT_YET_EXECUTED`
- Runtime visual review: `PENDING_NOT_AUTHORIZED_IN_THIS_BATCH`
- Final Gate 8 implementation: `HOLD_RUNTIME_REVIEW_REQUIRED`
- Deployment/production readiness: `NOT_AUTHORIZED`

## B. Baseline and Preflight

| Item | Result |
|---|---|
| Branch | `main` |
| Start HEAD | `aecf7edfd43b4124ec5ff17d35687020cf4c0d90` |
| Start subject | `docs(familyclaimref): record gate8 registration persistence decision package` |
| Start parent | `79d8f1d5b76b22b0ef1a65fbf781c833bbcd7fff` |
| Initial tracked/staged/untracked | `0/0/0` |
| MODIFY pre-existence | `27/27` |
| CREATE absence | `8/8` |
| `docs/419` or higher pre-existence | `0` |
| Unrelated user changes | `0` |

Protected decision documents remained read-only.

| Document | SHA-256 | Match |
|---|---|---|
| `docs/413` | `8f8a5717085ea3f08745e3ae16b8226897af0b127bfff55fbba6fc595650dabd` | PASS |
| `docs/414` | `522d1e9518cf2d4314f9cf3214d57d22be06c4f3b8b0f77fddf1cd4044c0141f` | PASS |
| `docs/415` | `04db1ba9dbb606a8ed2c429c447834294f2a407ee0d2714bb8369d0274e7727a` | PASS |
| `docs/416` | `e62e2cc9cb49b8fe090db49f608ef0c3ed76014bc336ea986a1a321b58b58b28` | PASS |
| `docs/417` | `2b6ff910b6699f8fcdb38344494472f34ed9c942a9916b8f0972a3dcbf6488c1` | PASS |
| `docs/418` | `e458f808079d07f8418072f31304ba10b74d28b84dcd4d30a4ffc326783c6363` | PASS |

## C. Exact 35-File Result

### C1. Production MODIFY: 19

1. `app/FamilyClaimRef.App/Composition/AppServices.cs`
2. `app/FamilyClaimRef.App/ViewModels/DocumentRegistrationViewModel.cs`
3. `app/FamilyClaimRef.App/Views/ProductDocumentRegistrationView.xaml.cs`
4. `app/FamilyClaimRef.App/Services/UI/WpfFilePickerService.cs`
5. `app/FamilyClaimRef.App/Services/UI/FilePickerResult.cs`
6. `app/FamilyClaimRef.App/Services/Storage/PolicyDocumentRegistrationRequest.cs`
7. `app/FamilyClaimRef.App/Services/Storage/ClaimDocumentRegistrationRequest.cs`
8. `app/FamilyClaimRef.App/Services/Storage/DocumentRegistrationWorkflow.cs`
9. `app/FamilyClaimRef.App/Services/Storage/DocumentAttachmentCoordinator.cs`
10. `app/FamilyClaimRef.App/Services/Storage/DocumentAttachmentRequest.cs`
11. `app/FamilyClaimRef.App/Services/Storage/IFileAttachmentService.cs`
12. `app/FamilyClaimRef.App/Services/Storage/LocalFileAttachmentService.cs`
13. `app/FamilyClaimRef.App/Services/Storage/FileAttachmentCopyResult.cs`
14. `app/FamilyClaimRef.App/Services/Storage/IDocumentStorageService.cs`
15. `app/FamilyClaimRef.App/Services/Storage/JsonDocumentStorageService.cs`
16. `app/FamilyClaimRef.App/Models/Storage/DocumentDraft.cs`
17. `app/FamilyClaimRef.App/Models/Storage/DocumentRecord.cs`
18. `app/FamilyClaimRef.App/Services/Localization/UiTextKeys.cs`
19. `app/FamilyClaimRef.App/Resources/UiStrings.xaml`

### C2. Production CREATE: 4

20. `app/FamilyClaimRef.App/Services/Storage/DocumentFileValidationService.cs`
21. `app/FamilyClaimRef.App/Services/Storage/DocumentFileValidationResult.cs`
22. `app/FamilyClaimRef.App/Services/Storage/StagedFileAttachment.cs`
23. `app/FamilyClaimRef.App/Services/Storage/DocumentRegistrationException.cs`

### C3. Test MODIFY: 8

24. `tests/FamilyClaimRef.App.Tests/DocumentRegistrationViewModelTests.cs`
25. `tests/FamilyClaimRef.App.Tests/DocumentRegistrationWorkflowTests.cs`
26. `tests/FamilyClaimRef.App.Tests/DocumentAttachmentCoordinatorTests.cs`
27. `tests/FamilyClaimRef.App.Tests/IFileAttachmentServiceTests.cs`
28. `tests/FamilyClaimRef.App.Tests/JsonDocumentStorageServiceTests.cs`
29. `tests/FamilyClaimRef.App.Tests/Integration/DocumentRegistrationNegativeValidationTests.cs`
30. `tests/FamilyClaimRef.App.Tests/Composition/AppServicesTests.cs`
31. `tests/FamilyClaimRef.App.Tests/Services/Localization/ResourceUiTextProviderTests.cs`

### C4. Test CREATE: 3

32. `tests/FamilyClaimRef.App.Tests/DocumentFileValidationServiceTests.cs`
33. `tests/FamilyClaimRef.App.Tests/DocumentRegistrationLifecycleGate8Tests.cs`
34. `tests/FamilyClaimRef.App.Tests/Integration/DocumentRegistrationPersistenceGate8Tests.cs`

### C5. Result Document CREATE: 1

35. `docs/419_POLICY_CLAIM_PRODUCT_UI_SHELL_GATE8_REAL_DOCUMENT_REGISTRATION_IMPLEMENTATION_RESULT_REVIEW.md`

Totals:

- Production: `23`
- Tests: `11`
- Result document: `1`
- MODIFY: `27`
- CREATE: `8`
- Total: `35`
- Extra or missing path: `0/0`

## D. D1-D9 Implementation Judgment

| Decision | Implementation result |
|---|---|
| D1 Architecture | Existing `DocumentRegistrationWorkflow` and lower storage/coordinator graph reused. No Product-only parallel workflow or storage was added. |
| D2 Payload authority | Successful Gate 8 registration uses the app-managed final copy as authoritative payload. Original input remains transient. |
| D3 File policy | Non-zero, 25 MiB maximum, four extensions, signature agreement, read-only selection SHA-256 snapshot, staged SHA-256 recomputation, and selection/staged comparison implemented. Mismatch requires reselection. |
| D4 Duplicate/concurrency | Active target kind + target ID + staged SHA-256 implemented. Duplicate query through link save is serialized by one same-process workflow critical section. Cross-process uniqueness is not provided. |
| D5 Product state | Cancel, replacement, reentry, inactive-target clear, success reset, retry retention, and busy duplicate prevention implemented and tested. |
| D6 Metadata | `18/1/3/1/8` classification applied. Gate 8 records require a complete metadata set while legacy records remain nullable/unverified. |
| D7 Atomicity/recovery | Same-root staging, atomic move, normal-exception compensation, and successful-return consistency implemented. Startup crash recovery remains deferred. |
| D8 Product resources | Exactly eight approved keys and Korean values added. Final parity is `99/99`; `Ui.Product.*` parity is `43/43`. |
| D9 Scope/tests | Exact 35-file scope maintained. New scenario coverage is `37/37`. |

## E. Architecture and Ownership

- Composition owner: `AppServices`
- Workflow owner: `DocumentRegistrationWorkflow`
- Selection and staged-byte validation owner: `DocumentFileValidationService`
- Staging/final payload owner: `IFileAttachmentService` and `LocalFileAttachmentService`
- Document/link metadata owner: `IDocumentStorageService` and `JsonDocumentStorageService`
- Target active-state owner: `IPolicyClaimStorageService` and `JsonPolicyClaimStorageService`
- Link owner: existing `DocumentLinkCoordinator`
- MainWindow and ProductShell use distinct ViewModels but share the same lower workflow/storage graph.
- Product UI and code-behind do not directly call filesystem or JSON APIs.

The actual WPF picker path always supplies a validated selection snapshot. The existing synthetic legacy harness path that directly injects both source path and display name without using the picker remains on the legacy nullable-metadata workflow for backward compatibility; it is not the Product picker path.

## F. File Validation and SHA Contract

- Allowed input extensions: `pdf`, `jpg`, `jpeg`, `png`
- Extension comparison: case-insensitive
- Durable extension: lower-case
- Zero-byte files: rejected
- Maximum allowed size: `26,214,400` bytes
- Above maximum: rejected
- Required signatures: PDF, JPEG, PNG
- Extension/signature mismatch: rejected
- Reparse point: rejected
- Unavailable or locked input: rejected
- Safe display name: leaf-only, invalid/control characters removed, maximum 255 characters
- Physical filename: generated by existing `FileNamePolicyService`
- Physical collision index: `1..999`

SHA relationship:

1. Selection reads the source and creates a runtime-only SHA-256 snapshot.
2. Registration copies the source to same-root staging.
3. Staged bytes are revalidated and rehashed.
4. Selection SHA-256 must equal staged SHA-256.
5. Atomic move preserves the staged payload bytes as final payload bytes.
6. Durable `Document.Sha256` is the validated staged/final payload hash.

The following are not persisted:

- Original absolute source path
- Selection snapshot object or separately classified selection hash
- Staging absolute path
- Managed absolute OS path

## G. Storage, Metadata, and Legacy Compatibility

Runtime-relative layout:

```text
<runtime root>/
  attachments/
    staging/
    documents/
  data/
    local/
      documents.json
      policy-documents.json
      claim-documents.json
```

Gate 8 metadata stores the required document/link/envelope values, including IDs, target relation, sanitized display filename, generated physical filename, relative payload key, normalized extension, validated type, byte length, durable SHA-256, reference date, document type, timestamps, schema version, and saved timestamp.

- REQUIRED NOW: `18`
- OPTIONAL NOW: `1`
- DERIVED: `3`
- DEFERRED: `1`
- FORBIDDEN: `8`
- Total: `31`

`DisabledAt` remains the nullable source of truth. `IsDisabled` and declared content type are derived and excluded from JSON. Existing records with no Gate 8 fields load with null metadata; reads do not fabricate values or rewrite the JSON file.

## H. Duplicate and Same-Process Concurrency

Duplicate key:

```text
active target kind + active target ID + staged payload SHA-256
```

Verified behavior:

- Same target and same bytes: rejected regardless of filename
- Same target and different bytes: allowed
- Different target and same bytes: allowed
- Disabled document/link: not treated as an active duplicate
- Failed attempt before active link: retry allowed
- Global deduplication: not implemented
- Version chain: not implemented
- Concurrent identical same-process requests: exactly one success and one structured duplicate
- Losing concurrent attempt staging residue: `0`
- Cross-process lock/uniqueness: not implemented

## I. Successful-Return Consistency and Compensation

Success requires:

- Final payload count: `1`
- Active Document count: `1`
- Active target link count: `1`
- Staging residue: `0`

Normal-exception behavior:

| Failure point | Result |
|---|---|
| Request/source validation | No storage side effect |
| Stage validation or selection/staged SHA mismatch | Staging removed |
| Target unavailable or duplicate | Staging removed |
| Final move failure | Staging cleanup attempted; no success |
| Document save failure | Final payload removed |
| Link save failure | Final payload removed and Document disabled |
| Cleanup failure | Structured/aggregate failure; no success return |
| Document-disable compensation failure | Aggregate failure; no success return |

The implemented contract is successful-return consistency, not a filesystem-plus-multiple-JSON crash transaction.

Deferred crash windows and residual risks:

| Crash window | Possible residue |
|---|---|
| After final move, before Document save | Orphan final payload |
| After Document save, before link save | Linkless Document |
| After link commit, before caller receives success | Ambiguous caller outcome |
| During compensation | Orphan payload or linkless/disabled metadata residue |

Startup recovery and cross-process uniqueness remain deferred. Production-ready claims are prohibited until those decisions and runtime review are completed.

## J. Product State and Copy

- Picker cancel with no previous file preserves empty state.
- Picker cancel with a previous valid file preserves that selection.
- Invalid replacement does not overwrite a previous valid selection.
- Reentry reloads active targets and preserves file/type/title/reference date draft.
- Reentry clears only an inactive selected target.
- Success clears file/type/title and resets reference date.
- Success retains the still-active target.
- Recoverable failure retains retry inputs.
- Busy state prevents duplicate load/register execution.
- Navigation itself remains allowed while the window-scoped operation continues.
- Product-visible copy uses resource messages and does not expose raw ID, path, SHA-256, GUID, exception, stack, type, runtime root, or JSON filename.

## K. Resource Result

Added exact keys:

1. `Ui.Product.DocumentRegistration.Validation.UnsupportedFileType`
2. `Ui.Product.DocumentRegistration.Validation.EmptyFile`
3. `Ui.Product.DocumentRegistration.Validation.FileTooLarge`
4. `Ui.Product.DocumentRegistration.Validation.SourceUnavailable`
5. `Ui.Product.DocumentRegistration.Validation.SourceChanged`
6. `Ui.Product.DocumentRegistration.Validation.DuplicateDocument`
7. `Ui.Product.DocumentRegistration.Status.Canceled`
8. `Ui.Product.DocumentRegistration.Status.RetryAvailable`

Verification:

- Resource values: `99`
- Constants: `99`
- Resource/constants delta: `0`
- `Ui.Product.*` resources: `43`
- `Ui.Product.*` constants: `43`
- New production Korean literal outside `UiStrings.xaml`: `0`

## L. Scenario-to-Test Mapping

### L1. Unit U01-U18

| ID | Exact test method |
|---|---|
| U01 | `U01_cancel_without_prior_file_preserves_empty_state` |
| U02 | `U02_cancel_with_prior_file_preserves_previous_snapshot` |
| U03 | `U03_valid_replacement_changes_only_file_snapshot` |
| U04 | `U04_reentry_preserves_draft_and_refreshes_targets` |
| U05 | `U05_reentry_clears_only_inactive_target` |
| U06 | `U06_success_resets_document_draft_and_retains_active_target` |
| U07 | `U07_recoverable_duplicate_failure_retains_retry_inputs` |
| U08 | `U08_busy_state_prevents_duplicate_command_execution` |
| U09 | `U09_allowed_extensions_are_accepted_case_insensitively` |
| U10 | `U10_unsupported_extension_is_rejected` |
| U11 | `U11_zero_byte_file_is_rejected` |
| U12 | `U12_exact_25_mib_boundary_is_accepted` |
| U13 | `U13_above_25_mib_boundary_is_rejected` |
| U14 | `U14_pdf_jpeg_and_png_signatures_are_validated` |
| U15 | `U15_extension_and_signature_mismatch_is_rejected` |
| U16 | `U16_missing_locked_and_reparse_boundaries_are_rejected_or_guarded` |
| U17 | `U17_selection_sha_and_staged_sha_mismatch_requires_reselection` |
| U18 | `U18_product_registration_copy_exposes_no_forbidden_internal_values` |

### L2. Integration I01-I13

| ID | Exact test method |
|---|---|
| I01 | `I01_managed_copy_survives_source_deletion` |
| I02 | `I02_relative_key_length_type_and_sha_persist` |
| I03 | `I03_same_target_and_same_sha_is_rejected` |
| I04 | `I04_same_name_and_different_bytes_is_allowed` |
| I05 | `I05_different_targets_and_same_bytes_are_allowed` |
| I06 | `I06_document_metadata_failure_deletes_final_payload` |
| I07 | `I07_link_failure_deletes_payload_and_disables_document` |
| I08 | `I08_compensation_failure_never_returns_success` |
| I09 | `I09_success_leaves_zero_staging_residue` |
| I10 | `I10_failure_has_no_active_link_to_missing_payload` |
| I11 | `I11_legacy_document_loads_without_fabricated_metadata_or_rewrite` |
| I12 | `I12_registration_uses_only_injected_temp_runtime_root` |
| I13 | `I13_concurrent_identical_registration_has_one_success_and_one_duplicate` |

### L3. Contract C01-C06

| ID | Exact test method |
|---|---|
| C01 | `C01_app_services_reuses_one_lower_registration_workflow` |
| C02 | `C02_product_view_and_code_behind_have_no_direct_file_or_json_calls` |
| C03 | `C03_product_shell_remains_default_startup_and_main_window_default_count_is_zero` |
| C04 | `C04_navigation_has_five_destinations_and_one_selection` |
| C05 | `C05_resource_constants_have_99_99_and_product_43_43_parity` |
| C06 | `C06_picker_extensions_equal_file_name_policy_allowlist` |

Coverage:

- Unit: `18/18`
- Integration: `13/13`
- Contract: `6/6`
- Total: `37/37`

## M. Build and Test Evidence

Final build command:

```text
dotnet build FamilyClaimRef.sln
```

Final build result:

- Restore: up to date
- Warning: `0`
- Error: `0`
- Result: PASS

New Gate 8 targeted command:

```text
dotnet test tests/FamilyClaimRef.App.Tests/FamilyClaimRef.App.Tests.csproj --no-build --filter "FullyQualifiedName~DocumentFileValidationServiceTests|FullyQualifiedName~DocumentRegistrationLifecycleGate8Tests|FullyQualifiedName~DocumentRegistrationPersistenceGate8Tests" --logger "console;verbosity=minimal"
```

Result: passed `37`, failed `0`, skipped `0`, total `37`.

Modified existing eight-suite targeted command:

```text
dotnet test tests/FamilyClaimRef.App.Tests/FamilyClaimRef.App.Tests.csproj --no-build --filter "FullyQualifiedName~DocumentRegistrationViewModelTests|FullyQualifiedName~DocumentRegistrationWorkflowTests|FullyQualifiedName~DocumentAttachmentCoordinatorTests|FullyQualifiedName~IFileAttachmentServiceTests|FullyQualifiedName~JsonDocumentStorageServiceTests|FullyQualifiedName~DocumentRegistrationNegativeValidationTests|FullyQualifiedName~AppServicesTests|FullyQualifiedName~ResourceUiTextProviderTests" --logger "console;verbosity=minimal"
```

Result: passed `199`, failed `0`, skipped `0`, total `199`.

Full command:

```text
dotnet test FamilyClaimRef.sln --no-build --logger "console;verbosity=minimal"
```

Result:

- Discovered: `486`
- Passed: `486`
- Failed: `0`
- Skipped: `0`
- Existing minimum threshold `436`: satisfied

### M1. Transparent Remediation History

The final PASS followed these observed and corrected failures:

1. First build: production assembly built; lifecycle test object-initializer syntax caused `22` compile errors.
2. Second build: errors `0`; four nullable/xUnit analyzer warnings remained.
3. First modified-suite targeted run: `197/199` passed; two expectations still represented the pre-Gate-8 source-unavailable contract.
4. First full run: `485/486` passed; one legacy isolated harness injected a non-signature synthetic file directly without a picker snapshot.

Corrections stayed inside the approved exact files. Final validation was rerun after every correction. The actual Product picker snapshot path remains strict; the isolated direct-injection legacy harness remains backward compatible.

## N. TEMP and Safety Evidence

- Logical Gate 8 test root: `%TEMP%\FamilyClaimRef\Gate8\gate8-validation-{actual-guid}\`
- Synthetic payloads only: yes
- Gate 8 test roots remaining after execution: `0`
- Gate 8 test files remaining after execution: `0`
- Existing isolated-workflow test roots remaining: `0`
- Staging residue: `0`
- Project-root `attachments/` files: `0`
- Project-root `data/local/` files: `0`
- Project-root `runtime_test_document.*` files: `0`
- Production runtime root access/deletion: `0/0`
- `data/claimdoc` access: `0`
- Persistent environment mutation: `0`
- Actual personal/insurance/hospital/diagnosis/claim sample use: `0`

No expanded user profile TEMP path or actual GUID is retained in this document.

## O. Static and Protected-Boundary Evidence

- Exact changed path set before this document: `34/34`
- Exact changed path set after this document: `35/35`
- Missing/extra path: `0/0`
- Product-only parallel workflow/storage: `0`
- Product workflow bypass: `0`
- Product view/code-behind direct filesystem/JSON call: `0`
- Durable source absolute path findings: `0`
- Durable staging or managed absolute path findings: `0`
- New production runtime-root literal: `0`
- Cross-process guarantee claim: `0`
- Startup recovery implementation claim: `0`
- Product raw exception/stack/type/path/ID exposure: `0`
- Navigation destinations/selected: `5/1`
- Default MainWindow runtime instance: `0`
- Startup file changes: `0`
- Runtime provider file changes: `0`
- DocumentList feature expansion: `0`
- Local profile path findings: `0`
- Personal identifier sample findings: `0`
- Merge markers: `0`
- Provisional implementation markers: `0`
- Trailing whitespace: `0`
- `git diff --check`: PASS
- Protected file changes: `0`
- Protected decision-document SHA-256 matches: `6/6`

## P. Execution and Git Boundaries

Not executed:

- App launch
- Actual file picker
- UIA/manual runtime smoke
- Screenshot capture
- Cleanup outside exact test-owned TEMP roots
- Deployment, installer, or packaging

Git mutation counts:

- Stage: `0`
- Commit: `0`
- Push: `0`
- Tag: `0`
- Rebase: `0`
- Amend: `0`

Expected final worktree:

- Tracked modified: `27`
- Staged: `0`
- Untracked: `8`
- Status entries: `35`
- Unrelated path: `0`

## Q. Original Implementation-Batch Judgment

All authorized source, static, build, targeted-test, full-test, TEMP-safety, and exact-scope gates passed.

The following remain explicitly unresolved:

- Guarded runtime UIA/manual visual review
- Cross-process uniqueness
- Startup crash recovery
- Deployment/production readiness

Therefore:

- Source implementation: `IMPLEMENTED_STATIC_AND_AUTOMATED_VALIDATION_PASS`
- Runtime visual review: `PENDING_NOT_AUTHORIZED_IN_THIS_BATCH`
- Final Gate 8 implementation: `HOLD_RUNTIME_REVIEW_REQUIRED`
- Deployment/production readiness: `NOT_AUTHORIZED`

Final marker:

`POLICY_CLAIM_PRODUCT_UI_SHELL_GATE8_REAL_DOCUMENT_REGISTRATION_IMPLEMENTATION_STATIC_AND_AUTOMATED_VALIDATION_PASS_RUNTIME_REVIEW_PENDING`

## R. Next Recommendation

Submit this exact 35-file diff and this result review for user/ChatGPT source and test review. Only after that review passes should a separate guarded runtime UIA/screenshot batch be considered. Staging, commit, and production-readiness claims remain prohibited in this batch.

## S. Independent Recheck Finding and U16 Repair Attempt

### S1. Independent recheck result

The independent read-only source/test recheck did not accept the original U16
reparse evidence. U16 executed the missing and locked cases, but its reparse
case only asserted that the production source contained
`FileAttributes.ReparsePoint`.

- Findings: Blocking `1`, Major `0`, Minor `0`
- Independent recheck judgment: `HOLD`
- Independent recheck marker:
  `POLICY_CLAIM_PRODUCT_UI_SHELL_GATE8_REAL_DOCUMENT_REGISTRATION_INDEPENDENT_RECHECK_HOLD`
- Production reparse guard inspection: present
- Actual reparse-point execution evidence before repair: absent

The original implementation-batch build and test results in section M remain
historical evidence. They are not rewritten as failures. The independent
recheck held only the strength of U16's reparse execution evidence.

### S2. Authorized repair scope

- Production files changed by repair: `0`
- Test files changed by repair: `1`
  - `tests/FamilyClaimRef.App.Tests/DocumentFileValidationServiceTests.cs`
- Result documents changed by repair: `1`
  - `docs/419_POLICY_CLAIM_PRODUCT_UI_SHELL_GATE8_REAL_DOCUMENT_REGISTRATION_IMPLEMENTATION_RESULT_REVIEW.md`
- New files: `0`
- Existing Gate 8 status entries retained: `35`

### S3. Actual reparse test design

U16 now uses `File.CreateSymbolicLink` inside its unique test-owned logical
root `%TEMP%\FamilyClaimRef\Gate8\gate8-validation-{actual-guid}\`.

The strengthened test contains executable assertions for:

1. a synthetic PDF target exists and is not a reparse point;
2. the file symbolic link exists and has `FileAttributes.ReparsePoint`;
3. `File.ResolveLinkTarget` returns the expected target;
4. the exact link path, not the target path, is passed to
   `DocumentFileValidationService`;
5. the validator returns structured `SourceUnavailable`;
6. the structured message does not contain the TEMP root;
7. no runtime storage directory is created.

The previous production-source string assertion was removed. No skip,
platform return, catch-and-pass, mock attribute, hard-link substitute, or
static guard substitute remains.

### S4. Repair execution evidence

Actual API invoked:

```text
File.CreateSymbolicLink(linkPath, targetPath)
```

Observed sequence:

- Missing-file execution: PASS, structured `SourceUnavailable`
- Locked-file execution: PASS, structured `SourceUnavailable`
- Regular target existence assertion: PASS
- Regular target `ReparsePoint` absence assertion: PASS
- File symbolic-link creation: FAILED before link creation
- Link `ReparsePoint` assertion: NOT REACHED
- `ResolveLinkTarget` assertion: NOT REACHED
- Production validator link-path rejection: NOT REACHED
- Skip count: `0`
- Static source-string substitute count: `0`
- Catch-and-pass count: `0`

The Windows filesystem call returned:

```text
System.IO.IOException: 클라이언트가 필요한 권한을 가지고 있지 않습니다.
```

The expanded TEMP path and actual GUID are intentionally not retained in this
document.

### S5. Current repair validation

| Check | Result |
|---|---|
| Solution build | PASS; warning `0`, error `0` |
| `DocumentFileValidationServiceTests` | passed `8`, failed `1`, skipped `0`, total `9` |
| Failure owner | U16 symbolic-link creation environment precondition |
| New Gate 8 three-suite rerun | NOT RUN after blocking U16 environment failure |
| Modified existing eight-suite rerun | NOT RUN after blocking U16 environment failure |
| Full test rerun | NOT RUN after blocking U16 environment failure |
| Test-owned Gate 8 root residue | roots `0`, files `0` |
| Production runtime root access/deletion | `0/0` |
| `data/claimdoc` access | `0` |
| App/file picker/UIA/screenshot | `0/0/0/0` |
| Git stage/commit/push | `0/0/0` |

The remaining suites were not presented as passing repair evidence after the
blocking U16 precondition failed. The last complete pre-repair independent
run remains build `0/0`, new Gate 8 `37/37`, modified suites `199/199`, and
full `486/486`, with the documented U16 evidence limitation.

### S6. Current judgment

- U16 repair code readiness: `READY_FOR_PRIVILEGED_OR_DEVELOPER_MODE_RECHECK`
- U16 actual reparse execution result:
  `HOLD_ACTUAL_REPARSE_POINT_TEST_ENVIRONMENT_UNAVAILABLE`
- Repair automated validation: `HOLD`
- Independent repair recheck: `REQUIRED_NOT_YET_EXECUTED`
- Final Gate 8 implementation: `HOLD_RUNTIME_REVIEW_REQUIRED`
- Runtime review: `NOT_AUTHORIZED`
- Deployment/production readiness: `NOT_AUTHORIZED`
- Stage/commit: `NOT_AUTHORIZED`

Current marker:

`POLICY_CLAIM_PRODUCT_UI_SHELL_GATE8_U16_ACTUAL_REPARSE_POINT_TEST_REPAIR_HOLD`

At that HOLD point, the recommendation was to rerun the actual reparse-point
test in a Windows environment where `File.CreateSymbolicLink` is permitted,
then perform a read-only independent repair recheck. Runtime UIA/screenshot
and Git stage/commit remained unauthorized.

## T. Symlink-Capable Environment Qualification and Automated Validation Rerun

### T1. Preflight

- Branch: `main`
- HEAD: `aecf7edfd43b4124ec5ff17d35687020cf4c0d90`
- Subject:
  `docs(familyclaimref): record gate8 registration persistence decision package`
- Parent: `79d8f1d5b76b22b0ef1a65fbf781c833bbcd7fff`
- Starting tracked/staged/untracked: `27/0/8`
- Starting status entries: `35`
- Approved Gate 8 path set equality: PASS, missing `0`, extra `0`
- Starting U16 test SHA-256:
  `ec55a7e3d1ebc9e8f5625ed628ea90914057d3fe8bab08a2772047ac8ff37431`
- Starting result document SHA-256:
  `e4dc6ae72df8960ec296e5508c4b42a9769260b69e3572a7b43af3bd3f47a227`
- `docs/413~418` approved SHA-256: `6/6` retained
- Starting Gate 8 TEMP residue: roots `0`, files `0`

### T2. Environment qualification

- Windows: Windows 10 Pro, version `25H2`, build `26200.8875`
- Actual probe process: non-elevated
- `SeCreateSymbolicLinkPrivilege`: not listed in the process token
- Developer Mode read-only observation:
  `AllowDevelopmentWithoutDevLicense=1`
- TEMP volume: `C:`, `NTFS`
- Relevant organization-policy values: not observed in the inspected policy
  keys
- Pre-batch operator environment preparation: Developer Mode was enabled
  outside this batch and Codex/terminal processes were restarted
- Batch-internal persistent environment mutation: `0`
- Qualified capability path: `A`, non-elevated Developer Mode execution
- Final capability evidence: actual `File.CreateSymbolicLink` execution, not
  registry or privilege observation alone

### T3. Historical failure and successful U16 re-execution

The initial repair execution in section S remains historical evidence:

- actual `File.CreateSymbolicLink` privilege failure;
- targeted result passed `8`, failed `1`, skipped `0`, total `9`;
- judgment `HOLD_ACTUAL_REPARSE_POINT_TEST_ENVIRONMENT_UNAVAILABLE`.

After the external environment preparation and full process restart, U16
executed the complete reparse contract:

1. regular synthetic PDF target creation: PASS;
2. target `ReparsePoint` absence assertion: PASS;
3. actual file symbolic-link creation: PASS;
4. link `FileAttributes.ReparsePoint` assertion: PASS;
5. `File.ResolveLinkTarget` non-null and expected target assertion: PASS;
6. exact link path delivered to `DocumentFileValidationService`: PASS;
7. structured `SourceUnavailable` rejection: PASS;
8. raw TEMP root absence from the structured message: PASS;
9. exact test-owned TEMP root cleanup: PASS.

`DocumentFileValidationServiceTests` result:

- passed `9`;
- failed `0`;
- skipped `0`;
- total `9`.

### T4. Full automated and static revalidation

| Check | Result |
|---|---|
| New Gate 8 three suites | passed `37`, failed `0`, skipped `0`, total `37` |
| Modified existing eight suites | passed `199`, failed `0`, skipped `0`, total `199` |
| Solution build | PASS; warning `0`, error `0` |
| Full solution tests | passed `486`, failed `0`, skipped `0`, total `486` |
| Resource/constants parity | `99/99`, missing `0/0` |
| `Ui.Product.*` parity | `43/43` |
| U16 forbidden substitutes | skip/return/catch-and-pass/source-string/mock/hardlink `0` |
| `git diff --check` | PASS |
| Gate 8 TEMP residue | roots `0`, files `0` |
| Project attachments/data-local/runtime-test residue | `0/0/0` |
| Project-root payload artifacts | `0` |
| Production runtime root access/deletion | `0/0` |
| `data/claimdoc` access | `0` |
| Batch-internal persistent environment mutation | `0` |

### T5. Current scope and judgment

- Current-batch production content delta: `0`
- Current-batch test content delta: `0`
- Current-batch document content delta: exactly this `docs/419` update
- New files: `0`
- `docs/413~418` and protected file changes: `0`
- Findings for this automated rerun: Blocking `0`, Major `0`, Minor `0`
- U16 repair automated validation: `PASS`
- Independent repair recheck: `REQUIRED_NOT_YET_EXECUTED`
- Final Gate 8 implementation: `HOLD_RUNTIME_REVIEW_REQUIRED`
- Runtime review: `NOT_AUTHORIZED`
- Deployment/production readiness: `NOT_AUTHORIZED`
- Stage/commit: `NOT_AUTHORIZED`

Current marker:

`POLICY_CLAIM_PRODUCT_UI_SHELL_GATE8_U16_ACTUAL_REPARSE_POINT_TEST_REPAIR_STATIC_AND_AUTOMATED_VALIDATION_PASS_INDEPENDENT_RECHECK_REQUIRED`

The next authorized recommendation is a read-only independent repair recheck
of the actual symbolic-link creation and production rejection evidence.
