# Policy Claim Product UI Shell Gate8 Product Copy Reentry Duplicate and Cancel Policy Decision Candidate

## A. Status

- Status: `PRODUCT_STATE_AND_COPY_DECISION_CANDIDATE`
- Marker: `POLICY_CLAIM_PRODUCT_UI_SHELL_GATE8_PRODUCT_COPY_REENTRY_DUPLICATE_AND_CANCEL_POLICY_DECISION_CANDIDATE_READY`
- Implementation authorization: `NO`
- Resource modification authorization: `NO`

## B. Current Product State Facts

- ProductShell owns a dedicated `DocumentRegistrationViewModel`.
- MainWindow owns a separate registration ViewModel.
- Both ViewModels use the same lower workflow and storage instances within one `AppServices` graph.
- ProductShell navigation preserves the ProductShell registration ViewModel instance.
- Returning to the registration destination raises `Loaded` and reloads active targets.
- Picker cancel currently preserves the prior selected file.
- Registration success currently does not reset input fields.
- Failure currently leaves user input available for retry.
- ProductShell does not display `LastRegistrationSummary`.
- ProductShell status and validation copy are resource-driven.

## C. Candidate State Ownership

| State | Owner | Lifetime | Persisted |
|---|---|---|---|
| Selected source snapshot | `DocumentRegistrationViewModel` | ProductShell window/draft | No |
| Target selection | `DocumentRegistrationViewModel` | ProductShell window/draft | No |
| Document type/title/reference date | `DocumentRegistrationViewModel` | ProductShell window/draft | No |
| Busy flag | `DocumentRegistrationViewModel` | One operation | No |
| Validation/status copy | `DocumentRegistrationViewModel` | Transient | No |
| Final document metadata | Storage service | Durable | Yes |
| Final target link | Storage service | Durable | Yes |
| Original absolute source path | Runtime selection only | Until reset/replacement | Forbidden durable |

## D. Candidate Reentry Policy

When the registration destination is loaded:

1. If no registration is running, reload active policy and claim options.
2. If the selected target no longer exists or is inactive, clear only that target selection.
3. Preserve the selected file snapshot, document type, display title, and reference date.
4. Clear stale transient validation/status messages.
5. Do not execute registration automatically.
6. Do not reopen the picker automatically.
7. If a registration is running, do not start a concurrent target reload; resume/refresh after completion.

This policy allows navigation without losing a draft while ensuring stale targets are not reused.

## E. Candidate Cancel and Replacement Policy

| User event | Selected file | Metadata inputs | Target | Status/validation | Storage side effect |
|---|---|---|---|---|---|
| Picker cancel with no prior file | Remains empty | Preserve | Preserve | Show optional canceled status | None |
| Picker cancel with prior file | Preserve prior snapshot | Preserve | Preserve | Show optional canceled status | None |
| Select a new valid file | Replace snapshot | Preserve | Preserve | Clear file-specific error/status | None |
| Select an invalid file | Do not replace prior valid snapshot until validation succeeds | Preserve | Preserve | Show safe validation | None |
| User navigates away | Preserve draft | Preserve | Preserve | Preserve operation state | None |
| User returns | Preserve draft | Preserve | Revalidate | Clear stale transient copy | Target read only |

Cancel is not a failure and must not clear a valid existing draft.

### E1. Required State Transition Matrix

| Start state | Event | Expected selected file | Metadata input | Result message | Storage side effect |
|---|---|---|---|---|---|
| empty | file picker cancel | empty | unchanged | optional canceled status | none |
| selected | picker cancel | prior valid snapshot retained | retained | canceled status | none |
| selected | select another file | new snapshot after validation | retained | file-selected or validation status | none |
| selected | navigation away/back | retained | retained | stale transient copy cleared on reentry | target read only |
| validation error | retry | retained unless user replaces it | retained | replaced by latest result | only on deliberate register |
| success | screen reentry | empty | reset to initial values | success retained until reentry, then cleared | none on reentry |
| target removed | register | retained | retained | target unavailable | staged/final artifact compensated |
| target list reload | stale selection | retained | retained | target guidance | target selection cleared only |

## F. Candidate Registration Transition Policy

| Transition | Candidate behavior |
|---|---|
| Register starts | Set busy, prevent second select/register operation |
| Validation fails before workflow | Retain all inputs and focus/review relevant field |
| Duplicate rejected | Retain all inputs; allow target/file change |
| File/storage failure | Retain inputs; expose retry-safe copy |
| Target becomes inactive | Clear invalid target after failure; preserve file/metadata |
| Registration succeeds | Clear selected file, document type, display title, and reference date |
| Registration succeeds | Retain target kind and target selection only if still active |
| Registration succeeds | Show safe success status without raw IDs |
| Navigation while busy | Allowed; operation continues under window-scoped VM |
| Return while busy | Show busy state; do not start another registration/load |
| Operation completes off-screen | Persist status in VM and show it when user returns |

No autosave draft is introduced. Closing the ProductShell window discards the in-memory draft.

Phase 2A compatibility:

- Policy/claim management input retention and screen-entry message reset remain unchanged.
- Product registration uses the same message-reset principle on reentry.
- Product registration resets file/document metadata after success because a completed payload is immutable input, while retaining a still-active target for efficient repeated registration.

## G. Candidate Duplicate Copy

| Condition | Product copy intent |
|---|---|
| Same target and same content | Explain that the same document is already registered |
| Same filename but different content | Do not call it a duplicate; registration may continue |
| Different target and same content | Registration may continue |
| Previous failed attempt | Explain retry is available |
| Target removed or disabled | Ask the user to select an available target |
| Concurrent same target and same content in one process | Exactly one attempt succeeds; the other receives the duplicate message after workflow-level serialization |
| Cross-process same target and same content | No Gate8 guarantee; do not claim production readiness |

The UI must not mention SHA-256, GUID, JSON, file system roots, suffix indices, or internal record types.

Changed-after-selection copy is also content-based: selection performs a read-only SHA-256 runtime snapshot, registration compares it with the staged payload SHA-256, and a mismatch uses the reselection copy. Length and last-write are only auxiliary checks. The selection hash and original source path are never durable Product metadata.

## H. Existing Copy Reuse

The following existing resource roles remain reusable:

- source file label
- selected file label
- policy/claim target labels
- document type label
- display title label
- reference date label
- select file action
- register action
- required-field validation
- no-active-target validation
- select-target validation
- registration success
- registration failure
- cleanup failure

Existing `Ui.DocumentRegistration.*` messages remain the shared runtime boundary where their wording is already safe. Gate8 does not create a ProductShell-only duplicate for every shared message.

### H1. Resource Ownership Classification

| Class | Current/candidate ownership |
|---|---|
| Approved Product keys | Existing `Ui.Product.DocumentRegistration.*` title/section/target labels |
| Shared Product-safe runtime keys | Existing `Ui.DocumentRegistration.Validation.*`, status completed/failed/cleanup, no-active messages |
| Validation-harness-only state | `LastRegistrationSummary` and raw internal ID diagnostics |
| MainWindow-only display | MainWindow summary binding and harness layout |
| Gate8 new Product candidates | The exact eight keys in section I |
| Internal diagnostic text | Exception types/messages, rollback failures, paths, hashes; never direct Product copy |

### H2. Required Message Mapping

| Product condition | Resource decision |
|---|---|
| File not selected | Reuse `Ui.DocumentRegistration.Validation.SelectFile` |
| Unsupported format | New `Ui.Product.DocumentRegistration.Validation.UnsupportedFileType` |
| Empty file | New `Ui.Product.DocumentRegistration.Validation.EmptyFile` |
| Maximum size exceeded | New `Ui.Product.DocumentRegistration.Validation.FileTooLarge` |
| Selected file missing/unreadable | New `Ui.Product.DocumentRegistration.Validation.SourceUnavailable` |
| File changed after selection | New `Ui.Product.DocumentRegistration.Validation.SourceChanged`, triggered by selection SHA-256 versus staged SHA-256 mismatch |
| Target no longer valid | Reuse select-policy/select-claim and no-active shared keys |
| Duplicate document | New `Ui.Product.DocumentRegistration.Validation.DuplicateDocument` |
| Registration succeeded | Reuse `Ui.DocumentRegistration.Status.Completed` |
| Registration failed | Reuse `Ui.DocumentRegistration.Status.Failed` |
| Temporary/final cleanup failed | Reuse `Ui.DocumentRegistration.Status.CleanupFailed` |
| Retry is possible | New `Ui.Product.DocumentRegistration.Status.RetryAvailable` |
| User canceled picker | New `Ui.Product.DocumentRegistration.Status.Canceled` |

## I. Candidate New Product Resource Keys

Exactly eight new keys are candidates. They are not implemented in this package.

| # | Key | Candidate Korean value | Trigger |
|---:|---|---|---|
| 1 | `Ui.Product.DocumentRegistration.Validation.UnsupportedFileType` | `지원하지 않는 파일 형식입니다.` | extension/signature rejection |
| 2 | `Ui.Product.DocumentRegistration.Validation.EmptyFile` | `빈 파일은 등록할 수 없습니다.` | zero-byte file |
| 3 | `Ui.Product.DocumentRegistration.Validation.FileTooLarge` | `파일 크기는 25MB 이하여야 합니다.` | above candidate maximum |
| 4 | `Ui.Product.DocumentRegistration.Validation.SourceUnavailable` | `선택한 파일을 읽을 수 없습니다. 다시 선택해 주세요.` | missing/unreadable/locked |
| 5 | `Ui.Product.DocumentRegistration.Validation.SourceChanged` | `선택 후 파일이 변경되었습니다. 다시 선택해 주세요.` | selected snapshot mismatch |
| 6 | `Ui.Product.DocumentRegistration.Validation.DuplicateDocument` | `같은 대상에 동일한 문서가 이미 등록되어 있습니다.` | target-scoped SHA duplicate |
| 7 | `Ui.Product.DocumentRegistration.Status.Canceled` | `파일 선택을 취소했습니다.` | picker cancel |
| 8 | `Ui.Product.DocumentRegistration.Status.RetryAvailable` | `입력 내용을 유지했습니다. 확인 후 다시 시도해 주세요.` | recoverable failure |

Candidate resource counts after approval and implementation:

- Resource values: `99`
- Key constants: `99`
- Resource/constants parity: `99/99`
- `Ui.Product.*`: `43/43`
- New resource key candidate count: `8`

## J. Copy Safety Rules

ProductShell-visible copy must not contain:

- any local user-profile path or absolute path
- runtime root or staging directory
- JSON filenames
- raw policy, claim, document, or link IDs
- SHA-256 values
- CLR type names
- exception messages or stack traces
- machine/user names
- actual personal, insurance, hospital, diagnosis, contract, or claim data

Internal diagnostics may retain structured error codes. Product copy must resolve those codes through `IUiTextProvider`.

## K. Current and Candidate Message Boundary

| Surface | Current | Gate8 candidate |
|---|---|---|
| ProductShell selected file | Leaf display name only | Keep |
| ProductShell success | Resource message | Keep; no ID summary |
| ProductShell failure | Resource message | Extend with safe categories |
| ProductShell cleanup failure | Shared safe resource | Keep |
| Process crash after final move | No ProductShell success claim is available | Startup recovery is deferred; orphan final payload or Document without a link remains an internal residual risk |
| MainWindow diagnostic summary | Raw internal IDs currently bound | Keep outside ProductShell; no Gate8 expansion |
| Logs | No new approved logger | Do not expose through UI; logging decision deferred |

## L. Product Lifecycle Invariants

- Cancel never causes a copy or metadata write.
- Selection never causes a copy or metadata write.
- Reentry never causes registration automatically.
- One ViewModel instance cannot start two registration operations concurrently.
- The workflow serializes duplicate query plus registration for the same target/SHA-256 inside one process, even across separate ViewModel command surfaces.
- Concurrent same-process attempts for the same active target/SHA-256 yield exactly one success.
- Cross-process uniqueness is not guaranteed, so multi-process and production readiness remain on hold.
- A stale target is cleared before retry.
- Success clears the file and document metadata draft.
- Success retains only a still-active target selection.
- Failure retains enough input for a deliberate retry.
- ProductShell never displays the original full path or internal IDs.
- ProductShell reports success only after the workflow returns success with payload, Document, and link consistent.
- Normal exceptions use compensation; a process crash after final move can leave an orphan final payload or a Document without a link until separately approved startup recovery exists.

## M. Product Policy Decisions Requiring Approval

1. Retain a valid target after success.
2. Reset file/document metadata after success.
3. Preserve prior file on picker cancel.
4. Preserve draft while navigating within ProductShell.
5. Allow navigation during busy operation while preventing concurrent commands, and approve workflow-level same-process serialization for the same target/SHA-256.
6. Clear transient messages on reentry.
7. Add the exact eight resource keys and candidate Korean values.
8. Keep MainWindow diagnostic summary outside ProductShell.

## N. Blocker Count

| Blocker class | Count | Items |
|---|---:|---|
| Lifecycle blockers | `3` | success reset; reentry transient-state reset; navigation/busy serialization |
| Product policy blockers | `3` | size/signature and selection-SHA policy; duplicate/concurrency semantics; successful-return/reset/crash-recovery boundary |
| Copy/resource blockers | `1` | eight keys and final values require user approval |

Counts intentionally match docs/414 and docs/415; they are not additive duplicates when calculating the package total.

## O. Candidate Result

`POLICY_CLAIM_PRODUCT_UI_SHELL_GATE8_PRODUCT_COPY_REENTRY_DUPLICATE_AND_CANCEL_POLICY_DECISION_CANDIDATE_READY`

## P. Package Consistency Register

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
