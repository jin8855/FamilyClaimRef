# Policy Claim Product UI Shell Gate8 File Lifecycle Metadata Security and Failure Policy Decision Candidate

## A. Status

- Status: `DECISION_CANDIDATE`
- Marker: `POLICY_CLAIM_PRODUCT_UI_SHELL_GATE8_FILE_LIFECYCLE_METADATA_SECURITY_AND_FAILURE_POLICY_DECISION_CANDIDATE_READY`
- Implementation authorization: `NO`
- User review required: `YES`

## B. Selected Authoritative Payload Candidate

### B1. Candidate

`Candidate B: app-managed copy authoritative`

성공한 등록 이후 authoritative payload는 앱 관리 attachment root 아래의 복사본이다. 외부 원본 파일은 input일 뿐이며 durable record의 authoritative 위치가 아니다.

### B2. Consequences

- 성공 후 외부 원본이 이동 또는 삭제되어도 등록된 document는 유효해야 한다.
- 외부 원본 absolute path는 durable metadata에 저장하지 않는다.
- 외부 원본 파일명은 control character와 경로 정보를 제거한 display-only metadata로만 저장할 수 있다.
- payload key는 runtime root 기준 상대 경로만 저장한다.
- target ID를 directory 또는 physical file name에 포함하지 않는다.
- physical file name은 앱이 생성하고 extension은 lower-case normalization을 적용한다.
- ProductShell UI에는 full source path와 managed absolute path를 표시하지 않는다.

### B3. Rejected Candidates

| Candidate | Judgment | Reason |
|---|---|---|
| External original authoritative | Reject | 원본 이동/삭제, 권한 변경, removable media에 취약 |
| Dual authoritative copies | Reject | divergence와 conflict owner가 불명확 |
| App-managed copy authoritative | Select candidate | 기존 `LocalFileAttachmentService`와 일치하며 offline durability가 명확 |

## C. Candidate Storage Key

```text
<runtime root>/
  attachments/
    staging/
      <registration-operation-id>.tmp
    documents/
      <generated-document-file-name>.<normalized-extension>
  data/
    local/
      documents.json
      policy-documents.json
      claim-documents.json
```

Rules:

- `staging/` and `documents/` must resolve under the injected attachment root.
- A staging path must never be persisted as a document path.
- The final persisted key remains runtime-root-relative.
- A physical file name must not contain policy ID, claim ID, user name, machine name, original directory, or actual personal data.
- No folder-per-target layout is introduced in Gate8.

## D. Metadata Classification Matrix

The matrix covers normalized Document metadata plus its policy/claim link record. Target association stays in link records rather than being duplicated into `DocumentRecord`.

| # | Metadata item | Classification | Candidate storage/derivation | Reason |
|---:|---|---|---|---|
| 1 | Document ID | `REQUIRED NOW` | `DocumentRecord.Id` | Stable identity |
| 2 | Link record ID | `REQUIRED NOW` | Policy/claim link record `Id` | Stable association identity |
| 3 | Target kind | `REQUIRED NOW` | Link record type | Distinguishes policy and claim |
| 4 | Target ID | `REQUIRED NOW` | Link record `PolicyId` or `ClaimId` | Association owner |
| 5 | Original display file name | `REQUIRED NOW` | Sanitized leaf name only | User recognition without path |
| 6 | Document display title | `REQUIRED NOW` | `DocumentRecord.DisplayTitle` | Product-facing title |
| 7 | Physical file name | `REQUIRED NOW` | `DocumentRecord.PhysicalFileName` | Existing storage contract |
| 8 | Relative payload key | `REQUIRED NOW` | `DocumentRecord.RelativePath` | Durable app-managed location |
| 9 | Normalized extension | `REQUIRED NOW` | lower-case extension | Allowlist audit |
| 10 | Validated file type | `REQUIRED NOW` | PDF/JPEG/PNG | Signature result |
| 11 | Byte length | `REQUIRED NOW` | copied payload length | Size and integrity evidence |
| 12 | SHA-256 | `REQUIRED NOW` | lower-case hex digest | Target-scoped duplicate key |
| 13 | Reference date | `REQUIRED NOW` | request value | Existing user metadata |
| 14 | Document type/category | `REQUIRED NOW` | approved type code | Existing policy boundary |
| 15 | Created at UTC | `REQUIRED NOW` | `CreatedAt` | Audit time |
| 16 | Updated at UTC | `REQUIRED NOW` | `UpdatedAt` | State transition time |
| 17 | Disabled at UTC | `OPTIONAL NOW` | nullable `DisabledAt` | Active document has no value |
| 18 | Declared content type | `DERIVED` | From validated file type | Avoid conflicting duplicate state |
| 19 | Is disabled | `DERIVED` | `DisabledAt != null` | Existing source-of-truth rule |
| 20 | Registration status | `DERIVED` | Payload plus document/link active state | Avoid persisted UI state |
| 21 | Schema version | `REQUIRED NOW` | `JsonFileEnvelope.SchemaVersion` | Existing validated envelope contract |
| 22 | Saved at UTC | `REQUIRED NOW` | `JsonFileEnvelope.SavedAt` | Existing validated envelope contract |
| 23 | Free-form description/memo | `DEFERRED` | None | No approved ProductShell field |
| 24 | External source absolute path | `FORBIDDEN` | Never persisted | Local profile/privacy leak |
| 25 | Temporary staging absolute path | `FORBIDDEN` | Never persisted | Internal implementation detail |
| 26 | CLR type/exception type | `FORBIDDEN` | Never persisted or shown | Diagnostic leak |
| 27 | Stack trace/raw exception message | `FORBIDDEN` | Internal diagnostic only | Path/implementation leak |
| 28 | Machine name or user profile name | `FORBIDDEN` | Never persisted | Environment/privacy leak |
| 29 | Managed absolute OS path | `FORBIDDEN` | Relative key only | Environment portability |
| 30 | Raw internal GUID in ProductShell copy | `FORBIDDEN` | Internal state only | Product copy boundary |
| 31 | Unrelated personal/insurance/medical data | `FORBIDDEN` | Never collected | Data minimization |

Classification counts:

- `REQUIRED NOW`: `18`
- `OPTIONAL NOW`: `1`
- `DERIVED`: `3`
- `DEFERRED`: `1`
- `FORBIDDEN`: `8`
- Total metadata items: `31`

## E. Current-to-Candidate Schema Mapping

| Current field | Candidate treatment |
|---|---|
| `Id` | Keep |
| `PhysicalFileName` | Keep |
| `DisplayTitle` | Keep |
| `Extension` | Keep normalized |
| `RelativePath` | Keep relative |
| `CreatedAt` | Keep UTC |
| `UpdatedAt` | Keep UTC |
| `DisabledAt` | Keep nullable source of truth |
| Original display file name | Add candidate |
| Validated file type | Add candidate |
| Byte length | Add candidate |
| SHA-256 | Add candidate |
| Reference date | Add candidate |
| Document type | Add candidate |

Compatibility decision still required:

- Existing envelope `schemaVersion` and `savedAt` validation remains intact.
- Candidate additive Document fields may load as explicitly legacy/unverified nullable values, but every new Gate8 registration must write all new required fields.
- Silent fabricated SHA-256, byte length, or validated type is forbidden.
- Gate8 implementation must not rewrite all existing JSON merely by reading it.

## F. Candidate File Validation Policy

| Validation | Candidate decision | Failure |
|---|---|---|
| Source exists | Required at registration | Safe validation message |
| Source is a regular file | Required | Reject directory/special entry |
| Reparse point/symlink | Reject | Prevent boundary bypass |
| Read access | Required | Reject unreadable/locked file |
| Non-zero length | Required | Reject zero-byte file |
| Maximum size | `25 MiB` (`26,214,400` bytes) | Reject above maximum |
| Allowed extension | `pdf`, `jpg`, `jpeg`, `png` | Reject others |
| Extension normalization | lower-case, case-insensitive input | Persist normalized value |
| File signature | PDF/JPEG/PNG required | Reject malformed or mismatch |
| Double extension | Validate final extension and signature | Reject if final extension unsupported/mismatched |
| Selected snapshot | Open the source read-only and compute a SHA-256 runtime snapshot at selection time; retain leaf name plus auxiliary length/last-write UTC | Snapshot exists only in runtime state |
| Changed after selection | Compare the staged payload SHA-256 with the selection-time SHA-256; reject mismatch and require reselection | SHA-256 equality is authoritative; length/last-write are auxiliary only |
| Original display name | sanitized leaf, max 255 chars | Reject invalid/control-only name |
| Hash | SHA-256 at read-only selection and again from staged payload bytes | Required for changed-source comparison before duplicate decision; selection hash is not durable |

The picker filter is not a security boundary. The lower file validation service remains authoritative.

The selection-time SHA-256 and original source path are transient runtime validation data. Neither value is persisted in Document, policy link, claim link, JSON envelope, or ProductShell-visible evidence.

### F1. Policy Decision Detail

| Policy | Current source fact | Minimum safe candidate | User impact | Implementation complexity | Approval |
|---|---|---|---|---|---|
| Extension | Lower allowlist is `pdf/jpg/jpeg/png`; picker is broader | Match picker and lower allowlist | Fewer misleading picker choices | Low | Required |
| Case | Lower normalization is case-insensitive | Persist lower-case | No visible restriction | Low | Required |
| MIME | No durable content type | Derive from validated signature | Stable type display later | Low | Required |
| Signature | No signature check | Validate PDF/JPEG/PNG bytes | Malformed or renamed files rejected | Medium | Required |
| Maximum size | No approved maximum | `25 MiB` | Large files rejected with clear copy | Low | Required |
| Zero byte | Currently copyable | Reject | Empty file cannot be registered | Low | Required |
| Directory/missing | Existing file existence check | Require regular existing file | Clear reselection path | Low | Required |
| Unreadable/locked | Copy failure is generic | Preflight read and fail safely | Clear retry/reselection | Medium | Required |
| Changed/deleted after selection | No authoritative selected content snapshot | Compute a read-only selection SHA-256 and compare it with the staged payload SHA-256; length/last-write are auxiliary only | Requires reselection on mismatch | Medium | Required |
| Reparse point | No explicit rule | Reject | Prevents indirect path escape | Medium | Required |
| Long/control/reserved name | Physical name generated; display leaf unbounded | Sanitized leaf, max 255 chars | Safe display-only name | Low | Required |
| Double extension | Final extension only is normalized | Final extension plus signature must agree | Renamed executable-like inputs rejected | Low | Required |
| Same name/different bytes | Filename collision suffix only | Allow as a separate document | No forced rename prompt | Medium | Required |
| Different name/same bytes | No content duplicate check | Reject only in the same active target | Prevents accidental duplicate registration | Medium | Required |

## G. Candidate Duplicate Matrix

Duplicate key:

```text
active target kind + active target ID + SHA-256
```

| Scenario | Candidate result | Reason |
|---|---|---|
| Same target, same bytes, same name | Reject duplicate | Same target content |
| Same target, same bytes, different name | Reject duplicate | Filename does not change content identity |
| Same target, different bytes, same name | Allow | Generated physical name prevents collision |
| Same target, different bytes, different name | Allow | Separate document |
| Different target, same bytes | Allow | No global deduplication in Gate8 |
| Previous attempt failed before active link | Retry allowed | No successful active registration |
| Previous document/link disabled | Fresh registration allowed | Active-only duplicate rule |
| Target disabled/removed before commit | Reject and compensate | Target no longer valid |
| Physical generated-name collision | Advance suffix `1..999` | Storage collision, not business duplicate |
| Suffix space exhausted | Reject safely | No overwrite |

No content version chain is introduced in Gate8. Versioning remains a separate product decision.

### G1. Compared Duplicate Keys

| Candidate key | Judgment | Reason |
|---|---|---|
| target + original filename | Reject | Same bytes can be renamed; different bytes can share a name |
| target + case-insensitive filename | Reject | Reduces case drift but not content ambiguity |
| target + SHA-256 | Partial | Target kind must be explicit |
| global SHA-256 | Reject | Same document may legitimately belong to different targets |
| target kind + target ID + SHA-256 | Select candidate | Exact active business scope and content identity |
| metadata equality | Reject | User-editable metadata is not payload identity |

### G2. Same-Process Concurrency Boundary

- After the staged SHA-256 is known, the duplicate query and registration finalization for the same `target kind + target ID + SHA-256` run inside one process-local serialized critical section.
- The critical section is held through target recheck, duplicate query, final move, Document save, and link save.
- Two concurrent attempts for the same active target and staged SHA-256 must produce exactly one successful registration. The losing attempt is rejected as a duplicate and its staging payload is compensated.
- A ViewModel `IsBusy` flag is not the business concurrency boundary; it protects only one command surface.
- Gate8 provides no cross-process lock or transaction. Multi-process and production readiness remain `HOLD` / `NOT_AUTHORIZED`.
- The critical section can be implemented inside the existing `DocumentRegistrationWorkflow.cs` candidate, so this decision does not add a file to the future exact 35-file scope.

## H. Candidate Atomicity Sequence

1. Validate the target snapshot, request fields, and presence of the transient selection-time SHA-256.
2. Open the source read-only and copy into same-root `attachments/staging`.
3. Validate actual length, signature, and SHA-256 from staged payload bytes.
4. Compare the staged SHA-256 with the selection-time SHA-256. On mismatch, delete staging and require reselection.
5. Enter the same-process serialized critical section for `target kind + target ID + staged SHA-256`.
6. Recheck target active state.
7. Query active target-scoped SHA-256 duplicate.
8. Atomically move the staged file to the final `attachments/documents` key.
9. Persist the Document record.
10. Persist the policy/claim link record.
11. Return success and release the critical section only when payload, Document, and link are all valid.

The staged-to-final move must stay on the same filesystem/root so the rename can be atomic.

The current Gate8 contract is **successful-return consistency**, not crash-atomic consistency across the payload and three JSON stores.

## I. Normal Exception Failure and Compensation Matrix

| Failure point | Required compensation | User-visible result | Retry |
|---|---|---|---|
| Request validation | None | Correct field-specific message | Yes |
| Source missing/unreadable | None | Reselect file | Yes |
| Size/signature mismatch | Delete staging file | Unsupported/invalid file message | Yes |
| Source changes during copy | Delete staging file | Reselect changed file | Yes |
| SHA duplicate found | Delete staging file | Duplicate message | After different file/target |
| Target becomes inactive | Delete staging file | Target unavailable message | After valid target selection |
| Final rename fails | Delete staging file where possible | Registration failed safely | Yes |
| Document metadata save fails | Delete final payload | Registration failed safely | Yes |
| Link save fails | Delete final payload and disable Document | Registration failed safely | Yes |
| Payload cleanup fails | Preserve failure evidence internally; no success | Safe cleanup failure message | After operator review |
| Document disable compensation fails | Aggregate internally; no success | Safe cleanup failure message | After operator review |
| Cancellation before finalization | Delete staging file; no metadata/link | Canceled message | Yes |
| Cancellation after finalization starts | Complete all steps or compensate; never return ambiguous partial success | Completed or safe failure | Depends on outcome |
| UI navigation during operation | Operation continues under VM; no duplicate start | Busy state on return | After completion |

### I1. Crash Consistency Matrix After Final Move

| Crash window | Possible durable residue | Contract judgment | Required follow-up |
|---|---|---|---|
| Final move completed, before Document save | Orphan final payload; no Document; no link | Not a successful registration | Startup orphan-payload recovery is required before production readiness |
| Document save committed, before link save starts | Final payload plus active Document without a policy/claim link | Not a successful registration | Startup linkless-Document reconciliation or quarantine is required |
| Link save started, before its atomic file replacement completes | Final payload plus active Document; link is either absent or remains at the last valid envelope | Not known successful to the caller | Startup reconciliation must identify and repair/quarantine the linkless state |
| Link save committed, before successful return reaches the caller | Final payload, active Document, and one active link are durable; caller may not have received success | Durable state is internally consistent but operation outcome is ambiguous to the caller | Idempotent duplicate detection must prevent a second active link on retry |
| Normal-exception compensation started, then process crashes | Orphan final payload and/or active Document without a link can remain, depending on the completed compensation step | Not a successful registration | Startup recovery must inspect payload, Document, and link ownership together |

Crash-window rows are residual-risk documentation, not a claim that startup recovery exists.

## J. Consistency Invariants

- Success requires one final payload, one active Document record, and one active target link.
- The consistency guarantee applies when the workflow returns success.
- Durable metadata must never point to a known missing payload.
- Staging paths must never appear in durable metadata.
- A failed registration must not leave an active target link.
- A cleanup failure must never be reported as success.
- Metadata present with a known missing payload is forbidden.
- A final payload without metadata is not an allowed success state; it may exist only as an explicitly recorded cleanup-failure residue.
- A retry must not overwrite an existing payload.
- ProductShell copy must not include path, GUID, exception type, or stack trace.
- A file with the same target-scoped SHA-256 must not create another active link.
- Concurrent same-process attempts for the same active target/SHA-256 must yield exactly one success.
- A crash can violate the normal-exception compensation outcome until an explicitly approved startup recovery gate reconciles residual state.

## K. Recovery Boundary

Gate8 candidate includes synchronous compensation for normal exceptions only. It does not provide crash-atomic persistence across the final payload, Document metadata, and target link.

Deferred:

- startup orphan scan
- scheduled integrity scan
- quarantine UI
- payload repair UI
- historical record migration
- hash backfill for legacy records
- global content-addressed storage
- cross-process file lock coordination

These items require a separate recovery/migration decision and are not silently added to Gate8.

Residual risks after the candidate implementation:

- Process termination can leave a staging residue, an orphan final payload, or an active Document without a link.
- The three JSON stores are not one transaction.
- Legacy records may lack Gate8 hash/type/length metadata and must remain explicitly legacy/unverified.
- Same-process duplicate query plus registration requires a workflow-level serialized critical section; `IsBusy` alone is insufficient.
- Cross-process concurrent registration is not guaranteed by Gate8.
- No background integrity scan verifies payloads after registration.

Until startup recovery and cross-process coordination receive separate approval and validation, deployment/production readiness is `NOT_AUTHORIZED` and no production-ready claim is permitted.

## L. Security Boundary

- Canonicalize and verify every managed path remains under the configured runtime root.
- Do not trust picker display name as a path.
- Reject rooted relative keys, traversal segments, alternate data stream syntax, reparse points, and device/special paths.
- Use generated physical names and no target identifier in the filename.
- Do not store or log external absolute paths in ProductShell-visible evidence.
- Do not expose raw exception or internal IDs.
- Never inspect or use `data/claimdoc`.

## M. Blocker Count

| Blocker class | Count | Items |
|---|---:|---|
| Storage blockers | `3` | no staged selection-SHA/finalization contract; no same-process serialized duplicate-query/registration critical section; no startup recovery across final payload, Document, and link stores |
| Metadata blockers | `1` | current schema lacks required Gate8 fields and compatibility decision |
| Security blockers | `2` | no authoritative selection-SHA versus staged-SHA guard; raw path/selection hash/diagnostic/internal identity must remain non-durable and outside ProductShell |
| Product policy blockers | `3` | maximum size/signature approval; duplicate and concurrency semantics approval; successful-return/crash-recovery boundary approval |

## N. User Decisions Required

- Approve or reject app-managed authoritative copy.
- Approve or change `25 MiB`.
- Approve signature validation and selected-file snapshot rules.
- Approve target-scoped SHA-256 duplicate semantics and same-process serialization with exactly one concurrent success.
- Approve metadata classification `18/1/3/1/8`.
- Approve same-root staging, normal-exception compensation, successful-return consistency, and deferred startup recovery residual risk.
- Decide legacy metadata compatibility before implementation.

## O. Candidate Result

`POLICY_CLAIM_PRODUCT_UI_SHELL_GATE8_FILE_LIFECYCLE_METADATA_SECURITY_AND_FAILURE_POLICY_DECISION_CANDIDATE_READY`

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
