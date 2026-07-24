# Policy Claim Product UI Shell Gate8 Exact Implementation File List and Validation Test Gate Plan

## A. Status

- Status: `IMPLEMENTATION_SCOPE_AND_VALIDATION_PLAN`
- Marker: `POLICY_CLAIM_PRODUCT_UI_SHELL_GATE8_EXACT_IMPLEMENTATION_FILE_LIST_AND_VALIDATION_TEST_GATE_PLAN_READY`
- Implementation authorization: `NO`
- Test execution authorization in this package: `NO`
- App/runtime execution authorization in this package: `NO`

## B. Scope Rule

This document defines a candidate exact implementation scope only. It does not authorize editing, staging, committing, building, testing, launching, selecting, copying, or registering any file.

Any future implementation must stop if:

- a required change falls outside the candidate exact list,
- a protected file must be modified,
- a user decision in docs/413~416 remains unresolved,
- real or production data is required,
- current baseline has unrelated changes.

## C. Future Exact Implementation Candidate File List

### C1. Production Files

| # | Action | Exact path | Candidate purpose |
|---:|---|---|---|
| 1 | `MODIFY` | `app/FamilyClaimRef.App/Composition/AppServices.cs` | Compose validation service and extended workflow |
| 2 | `MODIFY` | `app/FamilyClaimRef.App/ViewModels/DocumentRegistrationViewModel.cs` | Reentry, selected snapshot, reset, safe status |
| 3 | `MODIFY` | `app/FamilyClaimRef.App/Views/ProductDocumentRegistrationView.xaml.cs` | Serialize Loaded/select/register forwarding |
| 4 | `MODIFY` | `app/FamilyClaimRef.App/Services/UI/WpfFilePickerService.cs` | Align filter to `pdf/jpg/jpeg/png` |
| 5 | `MODIFY` | `app/FamilyClaimRef.App/Services/UI/FilePickerResult.cs` | Carry selected size/last-write snapshot |
| 6 | `MODIFY` | `app/FamilyClaimRef.App/Services/Storage/PolicyDocumentRegistrationRequest.cs` | Carry selected snapshot and sanitized name |
| 7 | `MODIFY` | `app/FamilyClaimRef.App/Services/Storage/ClaimDocumentRegistrationRequest.cs` | Carry selected snapshot and sanitized name |
| 8 | `MODIFY` | `app/FamilyClaimRef.App/Services/Storage/DocumentRegistrationWorkflow.cs` | Target recheck, duplicate decision, compensation |
| 9 | `MODIFY` | `app/FamilyClaimRef.App/Services/Storage/DocumentAttachmentCoordinator.cs` | Staging/finalization and validated metadata |
| 10 | `MODIFY` | `app/FamilyClaimRef.App/Services/Storage/DocumentAttachmentRequest.cs` | Add validation snapshot inputs |
| 11 | `MODIFY` | `app/FamilyClaimRef.App/Services/Storage/IFileAttachmentService.cs` | Add staged copy/finalize/abort boundary |
| 12 | `MODIFY` | `app/FamilyClaimRef.App/Services/Storage/LocalFileAttachmentService.cs` | Same-root staging, hash, atomic final rename |
| 13 | `MODIFY` | `app/FamilyClaimRef.App/Services/Storage/FileAttachmentCopyResult.cs` | Return length/hash/type/final key |
| 14 | `MODIFY` | `app/FamilyClaimRef.App/Services/Storage/IDocumentStorageService.cs` | Add target-scoped active hash query |
| 15 | `MODIFY` | `app/FamilyClaimRef.App/Services/Storage/JsonDocumentStorageService.cs` | Persist new metadata and duplicate query |
| 16 | `MODIFY` | `app/FamilyClaimRef.App/Models/Storage/DocumentDraft.cs` | Add approved Gate8 metadata |
| 17 | `MODIFY` | `app/FamilyClaimRef.App/Models/Storage/DocumentRecord.cs` | Add approved durable metadata |
| 18 | `MODIFY` | `app/FamilyClaimRef.App/Services/Localization/UiTextKeys.cs` | Add eight approved constants |
| 19 | `MODIFY` | `app/FamilyClaimRef.App/Resources/UiStrings.xaml` | Add eight approved Korean values |
| 20 | `CREATE` | `app/FamilyClaimRef.App/Services/Storage/DocumentFileValidationService.cs` | Lower authoritative validation |
| 21 | `CREATE` | `app/FamilyClaimRef.App/Services/Storage/DocumentFileValidationResult.cs` | Validated type/length/hash contract |
| 22 | `CREATE` | `app/FamilyClaimRef.App/Services/Storage/StagedFileAttachment.cs` | Staging lifecycle contract |
| 23 | `CREATE` | `app/FamilyClaimRef.App/Services/Storage/DocumentRegistrationException.cs` | Stable internal failure codes without raw UI copy |

Production candidate counts:

- `MODIFY`: `19`
- `CREATE`: `4`
- Production subtotal: `23`
- Resource files included in production subtotal: `2`
- Storage schema change files: `3`
  - `DocumentDraft.cs`
  - `DocumentRecord.cs`
  - `JsonDocumentStorageService.cs`

### C2. Test Files

| # | Action | Exact path | Candidate purpose |
|---:|---|---|---|
| 24 | `MODIFY` | `tests/FamilyClaimRef.App.Tests/DocumentRegistrationViewModelTests.cs` | Lifecycle and safe-copy state tests |
| 25 | `MODIFY` | `tests/FamilyClaimRef.App.Tests/DocumentRegistrationWorkflowTests.cs` | Duplicate/target/rollback tests |
| 26 | `MODIFY` | `tests/FamilyClaimRef.App.Tests/DocumentAttachmentCoordinatorTests.cs` | Staging and validated metadata tests |
| 27 | `MODIFY` | `tests/FamilyClaimRef.App.Tests/IFileAttachmentServiceTests.cs` | Same-root staging/finalization tests |
| 28 | `MODIFY` | `tests/FamilyClaimRef.App.Tests/JsonDocumentStorageServiceTests.cs` | Schema, compatibility, hash query tests |
| 29 | `MODIFY` | `tests/FamilyClaimRef.App.Tests/Integration/DocumentRegistrationNegativeValidationTests.cs` | Extended invalid-file/failure paths |
| 30 | `MODIFY` | `tests/FamilyClaimRef.App.Tests/Composition/AppServicesTests.cs` | Composition and root isolation |
| 31 | `MODIFY` | `tests/FamilyClaimRef.App.Tests/Services/Localization/ResourceUiTextProviderTests.cs` | `99/99` and `43/43` parity |
| 32 | `CREATE` | `tests/FamilyClaimRef.App.Tests/DocumentFileValidationServiceTests.cs` | File validation policy |
| 33 | `CREATE` | `tests/FamilyClaimRef.App.Tests/DocumentRegistrationLifecycleGate8Tests.cs` | Reentry/cancel/reset/busy state |
| 34 | `CREATE` | `tests/FamilyClaimRef.App.Tests/Integration/DocumentRegistrationPersistenceGate8Tests.cs` | TEMP-root real file/metadata/link integration |

Test candidate counts:

- `MODIFY`: `8`
- `CREATE`: `3`
- Test file subtotal: `11`

### C3. Result Review Document

| # | Action | Exact path | Candidate purpose |
|---:|---|---|---|
| 35 | `CREATE` | `docs/419_POLICY_CLAIM_PRODUCT_UI_SHELL_GATE8_REAL_DOCUMENT_REGISTRATION_IMPLEMENTATION_RESULT_REVIEW.md` | Record implementation and validation evidence |

### C4. Exact Candidate Totals

| Category | Count |
|---|---:|
| Production files | `23` |
| Test files | `11` |
| Result review document | `1` |
| Total future implementation candidate files | `35` |
| Resource files modified | `2` |
| New resource keys | `8` |
| Storage schema change files | `3` |
| New automated test scenario candidates | `37` |

The future exact file count remains `35` only because the corrected D4 concurrency implementation is assigned to the existing `DocumentRegistrationWorkflow.cs` candidate and the new concurrency scenario is assigned to the existing `DocumentRegistrationPersistenceGate8Tests.cs` candidate. If implementation requires another production or test file, the 35-file exact scope must be revised and re-approved before that file is created.

### C5. Per-File Evidence, Test, and Approval Matrix

The action and responsibility columns are defined in C1 through C3. This table supplies the remaining required evidence, test connection, and approval state for every candidate file.

| # | Exact path | Change evidence | Test connection | Approval state |
|---:|---|---|---|---|
| 1 | `app/FamilyClaimRef.App/Composition/AppServices.cs` | Current central construction lacks a file validation service | `AppServicesTests` | `CANDIDATE_NOT_APPROVED` |
| 2 | `app/FamilyClaimRef.App/ViewModels/DocumentRegistrationViewModel.cs` | Current success reset/reentry/busy policy is incomplete | VM and lifecycle Gate8 tests | `CANDIDATE_NOT_APPROVED` |
| 3 | `app/FamilyClaimRef.App/Views/ProductDocumentRegistrationView.xaml.cs` | Three async event forwarders can overlap on Loaded/command reentry | lifecycle and direct-bypass contract tests | `CANDIDATE_NOT_APPROVED` |
| 4 | `app/FamilyClaimRef.App/Services/UI/WpfFilePickerService.cs` | Picker advertises `webp`, `bmp`, and all files outside the allowlist | picker/allowlist contract test | `CANDIDATE_NOT_APPROVED` |
| 5 | `app/FamilyClaimRef.App/Services/UI/FilePickerResult.cs` | Current result has path/name but no read-only selection SHA-256 runtime snapshot | VM and validation tests | `CANDIDATE_NOT_APPROVED` |
| 6 | `app/FamilyClaimRef.App/Services/Storage/PolicyDocumentRegistrationRequest.cs` | Current request cannot carry the transient selection SHA-256 needed for staged comparison | workflow policy tests | `CANDIDATE_NOT_APPROVED` |
| 7 | `app/FamilyClaimRef.App/Services/Storage/ClaimDocumentRegistrationRequest.cs` | Current request cannot carry the transient selection SHA-256 needed for staged comparison | workflow claim tests | `CANDIDATE_NOT_APPROVED` |
| 8 | `app/FamilyClaimRef.App/Services/Storage/DocumentRegistrationWorkflow.cs` | Current target validation occurs after attachment, no SHA duplicate decision exists, and duplicate query plus registration is not one process-local serialized critical section | workflow and persistence Gate8 tests | `CANDIDATE_NOT_APPROVED` |
| 9 | `app/FamilyClaimRef.App/Services/Storage/DocumentAttachmentCoordinator.cs` | Current coordinator copies directly and stores limited metadata | coordinator and persistence Gate8 tests | `CANDIDATE_NOT_APPROVED` |
| 10 | `app/FamilyClaimRef.App/Services/Storage/DocumentAttachmentRequest.cs` | Current request lacks original display name and validation snapshot | coordinator tests | `CANDIDATE_NOT_APPROVED` |
| 11 | `app/FamilyClaimRef.App/Services/Storage/IFileAttachmentService.cs` | Current abstraction has final copy/delete/exists only | file service contract tests | `CANDIDATE_NOT_APPROVED` |
| 12 | `app/FamilyClaimRef.App/Services/Storage/LocalFileAttachmentService.cs` | Current implementation uses direct `File.Copy` to final path | file service and persistence Gate8 tests | `CANDIDATE_NOT_APPROVED` |
| 13 | `app/FamilyClaimRef.App/Services/Storage/FileAttachmentCopyResult.cs` | Current result lacks SHA-256 and validated file type | file service/coordinator tests | `CANDIDATE_NOT_APPROVED` |
| 14 | `app/FamilyClaimRef.App/Services/Storage/IDocumentStorageService.cs` | No active target-scoped hash lookup contract exists | JSON storage/workflow tests | `CANDIDATE_NOT_APPROVED` |
| 15 | `app/FamilyClaimRef.App/Services/Storage/JsonDocumentStorageService.cs` | Current Document schema and queries lack Gate8 metadata/hash | JSON storage and persistence Gate8 tests | `CANDIDATE_NOT_APPROVED` |
| 16 | `app/FamilyClaimRef.App/Models/Storage/DocumentDraft.cs` | Current draft lacks required Gate8 metadata | JSON storage/coordinator tests | `CANDIDATE_NOT_APPROVED` |
| 17 | `app/FamilyClaimRef.App/Models/Storage/DocumentRecord.cs` | Current record lacks original name/type/length/hash/date/type code | JSON storage/list regression tests | `CANDIDATE_NOT_APPROVED` |
| 18 | `app/FamilyClaimRef.App/Services/Localization/UiTextKeys.cs` | Eight candidate keys do not exist | resource parity tests | `CANDIDATE_NOT_APPROVED` |
| 19 | `app/FamilyClaimRef.App/Resources/UiStrings.xaml` | Eight candidate Korean values do not exist | resource parity/copy tests | `CANDIDATE_NOT_APPROVED` |
| 20 | `app/FamilyClaimRef.App/Services/Storage/DocumentFileValidationService.cs` | No lower signature/size or read-only selection-SHA versus staged-SHA validation owner exists | new validation service tests | `CANDIDATE_NOT_APPROVED` |
| 21 | `app/FamilyClaimRef.App/Services/Storage/DocumentFileValidationResult.cs` | No validated type/length/hash result contract exists | new validation service tests | `CANDIDATE_NOT_APPROVED` |
| 22 | `app/FamilyClaimRef.App/Services/Storage/StagedFileAttachment.cs` | No staged-file lifecycle contract exists | file service and persistence tests | `CANDIDATE_NOT_APPROVED` |
| 23 | `app/FamilyClaimRef.App/Services/Storage/DocumentRegistrationException.cs` | Current generic exception mapping cannot distinguish safe Product categories | VM safe-copy tests | `CANDIDATE_NOT_APPROVED` |
| 24 | `tests/FamilyClaimRef.App.Tests/DocumentRegistrationViewModelTests.cs` | Existing suite lacks Gate8 reset/reentry/snapshot/error categories | targeted VM tests | `CANDIDATE_NOT_APPROVED` |
| 25 | `tests/FamilyClaimRef.App.Tests/DocumentRegistrationWorkflowTests.cs` | Existing suite lacks hash duplicate and early target recheck cases | targeted workflow tests | `CANDIDATE_NOT_APPROVED` |
| 26 | `tests/FamilyClaimRef.App.Tests/DocumentAttachmentCoordinatorTests.cs` | Existing suite lacks staged finalization and extended metadata cases | targeted coordinator tests | `CANDIDATE_NOT_APPROVED` |
| 27 | `tests/FamilyClaimRef.App.Tests/IFileAttachmentServiceTests.cs` | Existing suite lacks stage/finalize/abort/hash cases | targeted file service tests | `CANDIDATE_NOT_APPROVED` |
| 28 | `tests/FamilyClaimRef.App.Tests/JsonDocumentStorageServiceTests.cs` | Existing suite lacks additive Gate8 field and hash query cases | targeted JSON tests | `CANDIDATE_NOT_APPROVED` |
| 29 | `tests/FamilyClaimRef.App.Tests/Integration/DocumentRegistrationNegativeValidationTests.cs` | Existing negative suite lacks size/signature/reparse/change cases | targeted negative integration tests | `CANDIDATE_NOT_APPROVED` |
| 30 | `tests/FamilyClaimRef.App.Tests/Composition/AppServicesTests.cs` | Existing composition test must include the new validator and preserve graph separation | targeted composition tests | `CANDIDATE_NOT_APPROVED` |
| 31 | `tests/FamilyClaimRef.App.Tests/Services/Localization/ResourceUiTextProviderTests.cs` | Current parity is `91/91`, `Ui.Product.*` `35/35` | candidate `99/99`, `43/43` tests | `CANDIDATE_NOT_APPROVED` |
| 32 | `tests/FamilyClaimRef.App.Tests/DocumentFileValidationServiceTests.cs` | New lower policy needs direct boundary tests | U09 through U17 | `CANDIDATE_NOT_APPROVED` |
| 33 | `tests/FamilyClaimRef.App.Tests/DocumentRegistrationLifecycleGate8Tests.cs` | New Product lifecycle policy needs focused tests | U01 through U08 and U18 | `CANDIDATE_NOT_APPROVED` |
| 34 | `tests/FamilyClaimRef.App.Tests/Integration/DocumentRegistrationPersistenceGate8Tests.cs` | New payload/metadata/link, same-process concurrency, and successful-return consistency invariants need TEMP integration | I01 through I13 | `CANDIDATE_NOT_APPROVED` |
| 35 | `docs/419_POLICY_CLAIM_PRODUCT_UI_SHELL_GATE8_REAL_DOCUMENT_REGISTRATION_IMPLEMENTATION_RESULT_REVIEW.md` | Future evidence requires one bounded result review | all static/build/test/runtime evidence | `CANDIDATE_NOT_APPROVED` |

### C6. Change Decision Summary

| Decision | Candidate |
|---|---|
| `App.xaml.cs` modification | `NO`, protected |
| `AppServices` modification | `YES`, one file |
| `ProductShellViewModel` modification | `NO`, protected |
| `ProductShellWindow` modification | `NO`, protected |
| `ProductDocumentRegistrationView.xaml` modification | `NO`, protected |
| `ProductDocumentRegistrationView.xaml.cs` modification | `YES`, one file |
| `DocumentRegistrationViewModel` modification | `YES`, one file |
| Existing tests modification | `YES`, eight files |
| New tests only sufficient | `NO`, existing behavior contracts also change |
| MainWindow modification count | `0` |
| Startup modification count | `0` |
| Storage schema change file count | `3` |

## D. Protected and Read-Only Reference Files

These files may be inspected during implementation but are not implementation candidates.

| Action | Exact path | Protection reason |
|---|---|---|
| `PROTECT` | `app/FamilyClaimRef.App/App.xaml` | Startup resource boundary already established |
| `PROTECT` | `app/FamilyClaimRef.App/App.xaml.cs` | Gate7 default startup is closed |
| `PROTECT` | `app/FamilyClaimRef.App/ViewModels/ProductShellViewModel.cs` | Existing window-scoped ownership is sufficient |
| `PROTECT` | `app/FamilyClaimRef.App/ProductShell/ProductShellWindow.xaml` | No new navigation or visual layout required |
| `PROTECT` | `app/FamilyClaimRef.App/ProductShell/ProductShellWindow.xaml.cs` | No composition bypass |
| `PROTECT` | `app/FamilyClaimRef.App/Views/ProductDocumentRegistrationView.xaml` | Existing controls/status bindings are sufficient |
| `PROTECT` | `app/FamilyClaimRef.App/ViewModels/MainWindowViewModel.cs` | Validation harness remains separate |
| `PROTECT` | `app/FamilyClaimRef.App/MainWindow.xaml` | No ProductShell feature backport |
| `PROTECT` | `app/FamilyClaimRef.App/MainWindow.xaml.cs` | No startup/harness change |
| `PROTECT` | `app/FamilyClaimRef.App/Services/FileNamePolicyService.cs` | Existing allowlist remains authoritative |
| `PROTECT` | `app/FamilyClaimRef.App/Services/Storage/DocumentLinkCoordinator.cs` | Existing ID-link boundary retained |
| `READ_ONLY_REFERENCE` | `app/FamilyClaimRef.App/Services/Runtime/EnvironmentRuntimeRootProvider.cs` | Existing guarded root injection |
| `READ_ONLY_REFERENCE` | `app/FamilyClaimRef.App/Services/Runtime/IRuntimeRootProvider.cs` | Existing test seam |
| `READ_ONLY_REFERENCE` | `app/FamilyClaimRef.App/Services/Runtime/RuntimeRootPaths.cs` | Existing root layout |
| `READ_ONLY_REFERENCE` | `app/FamilyClaimRef.App/ViewModels/ProductDocumentListViewModel.cs` | List expansion deferred |
| `READ_ONLY_REFERENCE` | `app/FamilyClaimRef.App/Views/ProductDocumentListView.xaml` | No list UI expansion |
| `READ_ONLY_REFERENCE` | `app/FamilyClaimRef.App/Views/ProductDocumentListView.xaml.cs` | Existing load behavior |
| `READ_ONLY_REFERENCE` | `app/FamilyClaimRef.App/Services/Storage/IPolicyClaimStorageService.cs` | Existing active-target query contract is sufficient |
| `READ_ONLY_REFERENCE` | `app/FamilyClaimRef.App/Services/Storage/JsonPolicyClaimStorageService.cs` | Existing target persistence remains unchanged |
| `READ_ONLY_REFERENCE` | `app/FamilyClaimRef.App/Models/Storage/PolicyRecord.cs` | Existing target record remains unchanged |
| `READ_ONLY_REFERENCE` | `app/FamilyClaimRef.App/Models/Storage/ClaimRecord.cs` | Existing target record remains unchanged |
| `READ_ONLY_REFERENCE` | `tests/FamilyClaimRef.App.Tests/JsonPolicyClaimStorageServiceTests.cs` | Existing target repository regression reference |

Protected/reference count: `22`

## E. New Automated Test Scenario Candidates

The minimum new automated scenario candidate count is `37`. The count is a planning minimum, not a claim that tests have been implemented or discovered.

### E1. Unit Scenarios: 18

| ID | Scenario |
|---|---|
| U01 | Picker cancel with no previous file preserves empty state |
| U02 | Picker cancel with a previous file preserves the previous snapshot |
| U03 | Valid replacement changes only file snapshot and clears file error |
| U04 | Reentry preserves draft fields and refreshes targets |
| U05 | Reentry clears an inactive target |
| U06 | Success resets file/metadata and retains a still-active target |
| U07 | Recoverable failure retains retry inputs |
| U08 | Busy state prevents duplicate command/load execution across navigation |
| U09 | Each `pdf/jpg/jpeg/png` extension is accepted case-insensitively |
| U10 | Unsupported extension is rejected |
| U11 | Zero-byte file is rejected |
| U12 | File exactly `26,214,400` bytes is accepted |
| U13 | File above `26,214,400` bytes is rejected |
| U14 | Matching PDF/JPEG/PNG signature is accepted |
| U15 | Extension/signature mismatch is rejected |
| U16 | Missing, unreadable, locked, or reparse source is rejected safely |
| U17 | Read-only selection SHA-256 is compared with staged payload SHA-256; mismatch is rejected and requires reselection while length/last-write remain auxiliary only |
| U18 | Product status contains no path, GUID, CLR type, or raw exception |

### E2. Integration Scenarios: 13

| ID | Scenario |
|---|---|
| I01 | App-managed copy remains usable after the source is removed |
| I02 | Relative key, byte length, validated type, and SHA-256 persist correctly |
| I03 | Same target and same SHA-256 is rejected |
| I04 | Same target, same name, different bytes is allowed |
| I05 | Different target and same SHA-256 is allowed |
| I06 | Document metadata failure deletes finalized payload |
| I07 | Link failure deletes payload and disables Document |
| I08 | Compensation failure never returns success |
| I09 | Successful registration leaves no staging residue |
| I10 | Failed registration leaves no active link to a missing payload |
| I11 | Legacy metadata follows the approved compatibility decision |
| I12 | Injected TEMP root is used and production root writes remain zero |
| I13 | Two concurrent same-process registrations for the same active target and staged SHA-256 yield exactly one success and one compensated duplicate result |

### E3. Contract Scenarios: 6

| ID | Scenario |
|---|---|
| C01 | `AppServices` composes one reusable workflow path and no Product-only parallel storage |
| C02 | Product view/code-behind contains no direct file or JSON service call |
| C03 | ProductShell remains default startup and MainWindow startup count remains zero |
| C04 | Navigation count remains five and registration destination remains selectable |
| C05 | Resource/constants parity is `99/99` and `Ui.Product.*` parity is `43/43` |
| C06 | Picker extension set equals `FileNamePolicyService` allowlist |

## F. Static Validation Gates

| Gate | PASS condition |
|---|---|
| Exact scope | Only approved 35 future files changed/created |
| Composition single path | One shared workflow/storage path and Product-only parallel workflow files `0` |
| Workflow bypass | ProductShell workflow bypass findings `0` |
| Direct filesystem | Product view and code-behind direct filesystem/service calls `0` |
| Production root literal | New production runtime-root literal findings `0` |
| Source path persistence | External absolute source path persistence findings `0` |
| Selection snapshot durability | Selection SHA-256 and original source path durable metadata findings `0` |
| Changed-source authority | Selection SHA-256 versus staged SHA-256 comparison exists; length/last-write are auxiliary only |
| Same-process concurrency | Duplicate query plus registration is one serialized workflow critical section; same target/SHA-256 concurrent success count is exactly `1` |
| Cross-process claim | Cross-process uniqueness or production-readiness claims `0` |
| Crash recovery claim | Startup recovery implementation claims `0`; production readiness stays `NOT_AUTHORIZED` |
| Raw exception UI | Raw exception/stack/type Product binding findings `0` |
| Navigation regression | Navigation destinations `5`, selected count `1` |
| MainWindow default | Default runtime MainWindow instance `0` |
| Preview compatibility | `--product-shell-preview` path remains valid |
| Startup drift | `App.xaml.cs` and default ProductShell selection unchanged |
| Runtime root drift | Runtime provider files changed `0` |
| Korean literal policy | New production Korean literals outside `UiStrings.xaml` `0` |
| Resource parity | Values/constants `99/99`; `Ui.Product.*` `43/43` |
| Privacy scan | Absolute local profile, actual personal/sample, raw ID copy findings `0` |
| Whitespace/patch | `git diff --check` PASS |

## G. Build and Test Gates

Future approved implementation must run:

1. `dotnet build FamilyClaimRef.sln`
2. Targeted `DocumentFileValidationServiceTests`
3. Targeted `DocumentRegistrationLifecycleGate8Tests`
4. Targeted `DocumentRegistrationPersistenceGate8Tests`
5. Targeted existing modified suites
6. `dotnet test FamilyClaimRef.sln`

PASS thresholds:

- build error: `0`
- build warning: `0`
- targeted test failed: `0`
- targeted test skipped: `0`
- full test failed: `0`
- full test skipped: `0`
- full discovered tests: at least current `436` plus all actually added discovered tests

No fixed future discovered count is claimed before implementation.

## H. TEMP-Only Integration Plan

All integration file operations must use an injected root under:

```text
%TEMP%\FamilyClaimRef\Gate8\gate8-validation-{GuidN}\
```

Required synthetic payloads:

- minimal valid PDF signature sample
- minimal valid JPEG signature sample
- minimal valid PNG signature sample
- malformed signature sample
- zero-byte sample
- exact-boundary size sample generated within TEMP only
- same-byte/different-name duplicate samples
- same-name/different-byte samples

Rules:

- Synthetic content only.
- No actual policy, claim, insurance, hospital, diagnosis, contract, or personal data.
- No `data/claimdoc` access.
- No production `%LOCALAPPDATA%\FamilyClaimRef` write.
- No Process/User/Machine persistent environment-variable mutation.
- Every test receives a unique injected root.
- Evidence/log output, when required, stays outside the runtime payload root.
- Cleanup may remove only the exact injected TEMP test root created by that test.
- Successful and failed tests must leave test-process residue count `0`.
- Production root access/deletion count must remain `0/0`.

## I. Runtime UIA and Screenshot Gate

Runtime evidence is a later separately approved step. Minimum required scenarios:

1. Default no-argument ProductShell launch, five navigation items, selected count one, MainWindow instance zero
2. DocumentRegistration entry and approved synthetic file selection
3. Picker cancel with an existing draft retained
4. Unsupported/invalid file safe rejection
5. Successful registration without path/internal IDs
6. Same-target duplicate rejection
7. Busy state plus navigation away/back without duplicate command
8. Stale target removal and selection clear
9. Forbidden identifier/path exposure count zero and blind/coordinate click count zero
10. Normal exit with unexpected dialog and process residue counts zero

Screenshot requirements:

- initial registration view
- cancel/reentry retained draft
- validation or duplicate message
- busy state
- success/reset state

Screenshots must use synthetic data and must not expose absolute paths or internal identifiers.

## J. Persistence Evidence Gate

Future result review must record:

- injected runtime root exact value
- source file synthetic identifier
- final relative payload key
- copied byte length
- SHA-256 equality between selection snapshot and staged payload
- SHA-256 equality between staged and final payload
- one active Document record
- one active policy or claim link
- duplicate rejection evidence
- concurrent same-process same-target/SHA-256 result counts: success `1`, compensated duplicate `1`
- successful-return consistency evidence for payload, Document, and link
- normal-exception compensation evidence separated from crash-window residual-risk review
- staging residue count `0`
- production runtime write count `0`
- project-root attachment/data artifact count `0`

The result review must not include the production absolute local profile path.

## K. Candidate Implementation Marker

PASS marker after all future gates:

`POLICY_CLAIM_PRODUCT_UI_SHELL_GATE8_REAL_DOCUMENT_REGISTRATION_ATTACHMENT_AND_PERSISTENCE_IMPLEMENTATION_PASS`

HOLD marker if any gate or approval is missing:

`POLICY_CLAIM_PRODUCT_UI_SHELL_GATE8_REAL_DOCUMENT_REGISTRATION_ATTACHMENT_AND_PERSISTENCE_IMPLEMENTATION_HOLD`

Current marker:

`POLICY_CLAIM_PRODUCT_UI_SHELL_GATE8_REAL_DOCUMENT_REGISTRATION_ATTACHMENT_AND_PERSISTENCE_IMPLEMENTATION_HOLD`

Reason: decision package awaits user review; implementation is not authorized.

## L. Validation Plan Result

`POLICY_CLAIM_PRODUCT_UI_SHELL_GATE8_EXACT_IMPLEMENTATION_FILE_LIST_AND_VALIDATION_TEST_GATE_PLAN_READY`

## M. Package Consistency Register

| Item | Package-wide value |
|---|---|
| Baseline HEAD | `79d8f1d5b76b22b0ef1a65fbf781c833bbcd7fff` |
| Baseline subject | `docs(familyclaimref): close gate7 default startup transition` |
| Baseline parent | `2ff924c846d2b5f7fad905afa5a7a90d93af31cf` |
| `docs/412` SHA-256 | `021AEE4719B402E465EBC2E74B958668E6BF19DF37A72112370B8D16020CB4FA` |
| Architecture | Candidate A, reuse existing workflow |
| Workflow owner | `DocumentRegistrationWorkflow` |
| File storage owner | `IFileAttachmentService` / `LocalFileAttachmentService` |
| Metadata repository owner | `IDocumentStorageService` / `JsonDocumentStorageService` |
| Target repository owner | `IPolicyClaimStorageService` / `JsonPolicyClaimStorageService` |
| Composition owner | `AppServices`; ProductShell window-scoped child ViewModel |
| Authoritative payload | App-managed copy after complete success |
| Reentry | Refresh targets, preserve draft, clear stale target/transient copy |
| Duplicate key | active `target kind + target ID + SHA-256` |
| Selection integrity | Read-only selection SHA-256 runtime snapshot compared with staged payload SHA-256; mismatch requires reselection; length/last-write are auxiliary only; selection hash and source path are not durable |
| Concurrency boundary | Same-process duplicate query plus registration is serialized; concurrent same target/SHA-256 yields exactly one success; cross-process guarantee is excluded |
| Picker cancel | Preserve prior valid selection and draft |
| Consistency contract | Successful-return consistency with normal-exception compensation; crash consistency and startup recovery remain deferred |
| Crash residual risk | Orphan final payload and Document without a link can remain after a process crash following final move |
| Current source inventory files | `58` |
| Metadata items | `31` |
| Metadata classification | `18/1/3/1/8` |
| Future exact implementation files | `35` |
| New resource key candidates | `8` |
| New automated scenario candidates | `37` |
| Unresolved blockers | `16` |
| Implementation readiness | `HOLD_IMPLEMENTATION_NOT_AUTHORIZED` |
| Deployment/production readiness | `NOT_AUTHORIZED`; multi-process uniqueness and startup recovery remain on hold |
| Documentation commit | `NOT_AUTHORIZED` |
| Non-approval | No source/test/resource/runtime/commit/deployment approval |
| Package final marker | `POLICY_CLAIM_PRODUCT_UI_SHELL_GATE8_REAL_DOCUMENT_REGISTRATION_ATTACHMENT_AND_PERSISTENCE_DECISION_PACKAGE_PASS_USER_REVIEW_PENDING` |
