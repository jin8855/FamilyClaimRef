# Policy/Claim RuntimeRootProvider Implementation Scope Plan

## A. Status

Status: IMPLEMENTATION_PLAN_ONLY

Marker:

```text
POLICY_CLAIM_RUNTIME_ROOT_PROVIDER_IMPLEMENTATION_SCOPE_PLANNED
```

This document plans implementation only.

No code is modified by this document.

No UI/XAML/resource work is authorized by this document.

No cleanup is authorized by this document.

No DB/SQLite/OCR/repository implementation is authorized by this document.

## B. Baseline

- latest commit: `3eeb89e docs(familyclaimref): defer ui redesign until core validation`
- git status before this document: clean
- source docs reviewed:
  - `docs/175_POLICY_CLAIM_ISOLATED_RUNTIME_ROOT_DESIGN_REVIEW.md`
  - `docs/176_POLICY_CLAIM_ISOLATED_RUNTIME_ROOT_DESIGN_COMMIT_CANDIDATE_REVIEW.md`
  - `docs/177_POLICY_CLAIM_UI_REDESIGN_DEFER_UNTIL_CORE_VALIDATION_DECISION.md`
  - `docs/178_POLICY_CLAIM_UI_REDESIGN_DEFER_COMMIT_CANDIDATE_REVIEW.md`

## C. Current Design Decision

- RuntimeRootProvider abstraction is the recommended direction.
- Default behavior must remain `%LOCALAPPDATA%\FamilyClaimRef`.
- Existing runtime evidence must remain untouched.
- Future validation should use isolated runtime root.
- UI redesign/localization/wireframe port remains deferred.

## D. Read-Only Source Findings

### Confirmed

- `AppServices.CreateDefault()` currently resolves `Environment.SpecialFolder.LocalApplicationData`.
- `AppServices.CreateDefault()` currently appends the `FamilyClaimRef` folder name.
- Metadata root is currently composed as selected app data root + `data/local`.
- Attachment root is currently composed as selected app data root + `attachments`.
- `JsonDocumentStorageService` receives the metadata root path.
- `JsonPolicyClaimStorageService` receives the metadata root path.
- `LocalFileAttachmentService` receives the attachment root path.
- `DocumentRegistrationWorkflow` composes attachment, link, document storage, and file attachment services without resolving the runtime root itself.
- `JsonFileStore` combines a provided root path with JSON file names and creates the target directory during save.
- Existing tests already use synthetic temp roots for storage, attachment, and workflow validation patterns.

### Unknown

- No existing `RuntimeRootProvider` abstraction was identified in the inspected source.
- No existing environment variable runtime root override was identified.
- No existing command-line runtime root override was identified.
- No existing dev settings file runtime root override was identified.
- No active runtime root diagnostic surface was identified.

### Candidate

- `AppServices.CreateDefault()` is the primary candidate integration point.
- A new service area under `app/FamilyClaimRef.App/Services/Runtime/` is a candidate location for runtime root resolution abstractions.
- Future tests can follow the current temp-root pattern under `tests/FamilyClaimRef.App.Tests`.

## E. Implementation Scope

Recommended first implementation scope:

1. Add runtime root resolution abstraction.
2. Keep default root unchanged.
3. Add explicit dev/test environment-variable override:
   - `FAMILYCLAIMREF_ENABLE_DEV_RUNTIME_ROOT_OVERRIDE=1`
   - `FAMILYCLAIMREF_RUNTIME_ROOT=<absolute path>`
4. Override must be ignored unless guard variable is explicitly enabled.
5. Override path must be absolute.
6. Metadata root must be selected root + `data/local`.
7. Attachment root must be selected root + `attachments`.
8. `AppServices.CreateDefault()` must use the provider.
9. No UI diagnostic surface in this implementation batch.
10. No command-line override in this implementation batch.
11. No dev settings file override in this implementation batch.

## F. Out of Scope

- XAML/UI changes
- Korean localization
- `ResourceDictionary` / `.resx`
- wireframe port
- app launch/manual workflow execution
- DB/SQLite/OCR/repository
- `data/claimdoc`
- cleanup/deletion
- migration of existing runtime evidence
- JSON record-level cleanup
- real document ingestion
- real personal sample data

## G. Expected Candidate Code Areas

Candidate areas only. This document does not modify them.

- new runtime root provider files under `app/FamilyClaimRef.App/Services/Runtime/`
- `app/FamilyClaimRef.App/Composition/AppServices.cs`
- tests under `tests/FamilyClaimRef.App.Tests/Services/Runtime/`
- tests under `tests/FamilyClaimRef.App.Tests/Composition/` if needed

## H. Implementation Acceptance Criteria

A future implementation is acceptable only if:

- default root remains unchanged when no override guard is set.
- override root is used only when guard is enabled and path is valid.
- metadata and attachment roots share the selected runtime root.
- no project root `attachments/` or `data/local` files are created.
- no `data/claimdoc` access occurs.
- no DB/SQLite/OCR/repository features are introduced.
- tests cover default and override behavior.
- existing `%LOCALAPPDATA%\FamilyClaimRef` evidence is untouched.

## I. Implementation Judgment

```text
POLICY_CLAIM_RUNTIME_ROOT_PROVIDER_IMPLEMENTATION_SCOPE_READY
```
