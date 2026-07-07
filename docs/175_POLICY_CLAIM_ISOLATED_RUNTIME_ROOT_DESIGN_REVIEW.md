# Policy/Claim Isolated Runtime Root Design Review

## A. Status

Status: DESIGN_REVIEW_ONLY

Marker:

```text
POLICY_CLAIM_ISOLATED_RUNTIME_ROOT_DESIGN_REVIEW_RECORDED
```

This document reviews design options only.

No code is modified by this document.

No cleanup is executed by this document.

No runtime metadata cleanup is authorized by this document.

No runtime attachment cleanup is authorized by this document.

No DB/SQLite/OCR/repository implementation is authorized by this document.

## B. Baseline

- latest commit: `0421717 docs(familyclaimref): close phase3d runtime evidence review`
- git status before this document: clean
- source docs reviewed:
  - `docs/168_POLICY_CLAIM_SCENARIO8_RUNTIME_ARTIFACT_CLEANUP_SCOPE_DECISION.md`
  - `docs/169_POLICY_CLAIM_SCENARIO8_TEMP_FILE_CLEANUP_EXECUTION_INSTRUCTION.md`
  - `docs/170_POLICY_CLAIM_SCENARIO8_TEMP_FILE_CLEANUP_READY_REVIEW.md`
  - `docs/171_POLICY_CLAIM_SCENARIO8_CLEANUP_DOCS_COMMIT_CANDIDATE_REVIEW.md`
  - `docs/172_POLICY_CLAIM_SCENARIO8_TEMP_FILE_CLEANUP_RESULT_REVIEW.md`
  - `docs/173_POLICY_CLAIM_PHASE3D_RUNTIME_EVIDENCE_CLOSURE_REVIEW.md`
  - `docs/174_POLICY_CLAIM_PHASE3D_CLOSURE_DOCS_COMMIT_CANDIDATE_REVIEW.md`

## C. Problem Statement

- Scenario 8 runtime evidence remains preserved under `%LOCALAPPDATA%\FamilyClaimRef`.
- Runtime metadata cleanup remains `DEFER`.
- Runtime attachment cleanup remains `DEFER`.
- Full runtime root cleanup remains `REJECT`.
- Deleting preserved evidence is not the right strategy for future validation.
- Future manual validation needs clean-room runtime state without deleting existing evidence.
- Therefore, a design for isolated runtime root is needed before new runtime validation batches.

## D. Current Evidence State

Temp synthetic cleanup:

```text
COMPLETED
```

Current temp paths:

- `%TEMP%\FamilyClaimRef\runtime_test_document.txt`: missing
- `%TEMP%\FamilyClaimRef\runtime_test_document.png`: missing
- `%TEMP%\FamilyClaimRef\runtime_test_document_claim.png`: missing

Temp directory:

- `%TEMP%\FamilyClaimRef`: preserved / not deleted

Runtime metadata existence only:

| Runtime metadata | Exists |
|---|---:|
| `%LOCALAPPDATA%\FamilyClaimRef\data\local\policies.json` | true |
| `%LOCALAPPDATA%\FamilyClaimRef\data\local\claims.json` | true |
| `%LOCALAPPDATA%\FamilyClaimRef\data\local\documents.json` | true |
| `%LOCALAPPDATA%\FamilyClaimRef\data\local\policy-documents.json` | true |
| `%LOCALAPPDATA%\FamilyClaimRef\data\local\claim-documents.json` | true |

Runtime attachments:

| Runtime attachment directory | Exists | File count |
|---|---:|---:|
| `%LOCALAPPDATA%\FamilyClaimRef\attachments\documents` | true | 3 |

This document does not include local user-profile absolute paths.

This document does not include metadata file contents.

This document does not include attachment file contents.

## E. Read-Only Code Inspection Summary

Searched keywords:

- `FamilyClaimRef`
- `LocalApplicationData`
- `%LOCALAPPDATA%`
- `AppData`
- `data\local`
- `data/local`
- `attachments`
- `documents.json`
- `policies.json`
- `claims.json`
- `policy-documents.json`
- `claim-documents.json`
- `Environment.SpecialFolder`
- `Path.Combine`
- `File.WriteAllText`
- `File.ReadAllText`
- `Directory.CreateDirectory`
- `RuntimeRoot`
- `RootProvider`
- `GetEnvironmentVariable`
- `CommandLine`

Candidate files/areas observed:

- `app/FamilyClaimRef.App/Composition/AppServices.cs`
- `app/FamilyClaimRef.App/App.xaml.cs`
- `app/FamilyClaimRef.App/Services/Storage/JsonFileStore.cs`
- `app/FamilyClaimRef.App/Services/Storage/JsonDocumentStorageService.cs`
- `app/FamilyClaimRef.App/Services/Storage/JsonPolicyClaimStorageService.cs`
- `app/FamilyClaimRef.App/Services/Storage/LocalFileAttachmentService.cs`
- `app/FamilyClaimRef.App/Services/Storage/DocumentRegistrationWorkflow.cs`
- related storage/workflow tests under `tests/FamilyClaimRef.App.Tests`

Current runtime root construction hypothesis:

- Confirmed default app composition currently starts from `Environment.SpecialFolder.LocalApplicationData`.
- Confirmed default app root uses the `FamilyClaimRef` folder name.
- Confirmed default metadata root is built as app data root plus `data/local`.
- Confirmed default attachment root is built as app data root plus `attachments`.
- Confirmed `JsonDocumentStorageService` and `JsonPolicyClaimStorageService` receive the metadata root path.
- Confirmed `JsonFileStore` combines a provided root path with JSON file names and creates the target directory during save.
- Confirmed `LocalFileAttachmentService` receives the attachment root path and stores document files under a `documents` relative folder.
- Confirmed `DocumentRegistrationWorkflow` composes attachment, link, document storage, and file attachment services without selecting a root itself.

Unknowns:

- No existing `RuntimeRootProvider` abstraction was identified.
- No existing environment variable runtime root override was identified.
- No existing command-line runtime root override was identified.
- No active runtime root diagnostic surface was identified for manual validation.
- The source inspection was read-only and did not test runtime behavior.

## F. Design Requirements

The isolated runtime root design must satisfy:

1. Existing `%LOCALAPPDATA%\FamilyClaimRef` evidence must remain untouched.
2. Future validation must be able to use a separate runtime root.
3. Default production/runtime behavior must remain unchanged unless an explicit dev/test override is enabled.
4. Override must be explicit and visible in validation logs/docs.
5. Override must not silently redirect real user data.
6. Override must not use `data/claimdoc`.
7. Override must not require deleting old runtime evidence.
8. Attachments and JSON metadata must be rooted under the same isolated root.
9. The design must support pre/post snapshot reporting.
10. The design must avoid wildcard/recursive cleanup as a normal workflow.
11. The design must not implement DB/SQLite/OCR/repository.
12. The design must be testable with synthetic data only.

## G. Design Options

### Option A: Continue Current Runtime Root With Manual Cleanup

Summary:

- Keep using `%LOCALAPPDATA%\FamilyClaimRef`.
- Clean artifacts manually between validations.

Assessment:

- Not recommended.
- Deletion risk remains.
- Evidence and new validation state can mix.
- Manual cleanup is error-prone.

### Option B: Environment Variable Runtime Root Override

Example concept:

- `FAMILYCLAIMREF_RUNTIME_ROOT`
- optional guard: `FAMILYCLAIMREF_ENABLE_DEV_RUNTIME_ROOT_OVERRIDE=1`

Assessment:

- Useful for Codex/manual validation.
- Easy to document.
- Risk: accidental environment leakage.
- Needs strong guardrails.

### Option C: Command-Line Runtime Root Override

Example concept:

- `--runtime-root <path>`

Assessment:

- Explicit at launch time.
- Good for scripted validation.
- Requires app launch path discipline.
- Still must be guarded against production use.

### Option D: Dev/Test Settings File Override

Example concept:

- local untracked dev settings file
- ignored by git

Assessment:

- Convenient for developer machine.
- Risk: hidden state.
- Risk: accidental commit if ignore rule is weak.
- Not preferred as primary mechanism.

### Option E: RuntimeRootProvider Abstraction

Example concept:

- central provider resolves default root and optional dev/test isolated root
- all metadata and attachment services depend on this provider
- default provider returns `%LOCALAPPDATA%\FamilyClaimRef`
- dev/test override provider returns explicit isolated root only under approved conditions

Assessment:

- Recommended design direction.
- Reduces path scattering.
- Makes validation setup auditable.
- Requires code implementation in a later separately approved batch.

## H. Recommended Design Direction

Recommend:

- Option E as the long-term design direction.
- Option B or C as the explicit override mechanism under the provider.
- Default behavior remains `%LOCALAPPDATA%\FamilyClaimRef`.
- Isolated validation root should use a deterministic, synthetic-only path such as:
  - `%TEMP%\FamilyClaimRef-Isolated\<scenario-id>`
  - `%TEMP%\FamilyClaimRef-Isolated\<timestamped-run-id>`

Do not recommend:

- deleting `%LOCALAPPDATA%\FamilyClaimRef`
- JSON record-level manual cleanup
- wildcard cleanup
- recursive cleanup
- using `data/claimdoc`
- using real policy/claim/hospital documents

## I. Proposed Future Implementation Boundary

This document does not authorize implementation.

If separately approved later, implementation should be small and staged:

1. Add runtime root resolution abstraction.
2. Keep default root unchanged.
3. Add explicit dev/test override guard.
4. Route policy/claim/document metadata paths through the root abstraction.
5. Route attachment document paths through the same root abstraction.
6. Add diagnostics/status text for active runtime root during validation.
7. Add synthetic-only manual validation instructions.
8. Add tests only after separate approval.

Implementation must not include:

- DB/SQLite/OCR/repository implementation
- real document ingestion
- real personal sample data
- broad cleanup
- migration of existing runtime evidence
- deletion of existing runtime evidence

## J. Validation Acceptance Criteria For Future Implementation

A future implementation should be considered acceptable only if:

- default runtime root remains unchanged without override.
- isolated runtime root can be selected explicitly.
- active runtime root can be verified before validation.
- policy metadata, claim metadata, document metadata, policy links, claim links, and attachments all use the same selected root.
- project root `attachments/` remains files=0.
- project root `data/local` remains files=0.
- `data/claimdoc` remains untouched.
- no DB/SQLite files are created.
- no OCR/repository features are introduced.
- synthetic document registration works in isolated root.
- existing `%LOCALAPPDATA%\FamilyClaimRef` evidence remains untouched.

## K. Risks

- Path resolution may be scattered across services.
- A partial override could split metadata and attachments into different roots.
- Environment variable override can leak across sessions.
- Command-line override can be missed by manual app launch.
- Dev settings file can become hidden state.
- Tests may accidentally rely on local machine state.
- Existing evidence under `%LOCALAPPDATA%` still remains and should not be deleted.

## L. Non-Execution Confirmations

| Item | Result |
|---|---|
| cleanup execution | not run |
| temp deletion rerun | not run |
| `Remove-Item` | not run |
| runtime metadata deletion | not run |
| runtime attachment deletion | not run |
| `%TEMP%\FamilyClaimRef` directory deletion | not run |
| `%LOCALAPPDATA%\FamilyClaimRef` deletion | not run |
| app launch | not run |
| OpenFileDialog | not run |
| Scenario 8A/8B rerun | not run |
| synthetic file creation | not run |
| document registration workflow | not run |
| code/XAML/ViewModel/test modification | none |
| `FileNamePolicyService` modification | none |
| allowlist modification | none |
| DB/SQLite/OCR/repository implementation | none |
| commit | not run |

## M. Design Judgment

```text
POLICY_CLAIM_ISOLATED_RUNTIME_ROOT_DESIGN_READY_FOR_REVIEW
```

Meaning:

- Isolated runtime root design can proceed to user review.
- Runtime metadata cleanup remains deferred.
- Runtime attachment cleanup remains deferred.
- Full runtime root cleanup remains rejected.
- Implementation remains blocked until separate approval.

## N. Next Recommended Work

1. Commit `docs/175~176` if validation passes.
2. Keep runtime metadata cleanup `DEFER`.
3. Keep runtime attachment cleanup `DEFER`.
4. If approved later, create implementation planning docs for RuntimeRootProvider / explicit isolated runtime override.
5. Do not implement code until a separate implementation approval is given.
