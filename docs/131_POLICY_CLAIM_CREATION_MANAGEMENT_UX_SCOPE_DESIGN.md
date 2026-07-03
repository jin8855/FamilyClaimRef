# Policy / Claim Creation Management UX Scope Design

## A. Status Marker

POLICY_CLAIM_CREATION_MANAGEMENT_UX_SCOPE_DESIGNED

## B. Background

Phase 1 added JSON storage for `Policy` and `Claim` records.

Phase 2 added `DocumentLinkCoordinator` validation so a document link can only target an existing active policy or claim.

Phase 3B added active `Policy` / `Claim` dropdown selection to the document registration UI. The document registration screen can now choose an active target instead of requiring direct target id input.

The remaining gap is that the application still has no UX for creating, editing, or disabling active `Policy` and `Claim` records. As a result, the document registration flow can select an existing target, but the user has no application-level path to create that target first.

The real MVP flow is therefore not closed yet.

## C. Problem Definition

- Document registration requires at least one active policy or claim before registration can complete.
- The current UI can select a target but cannot create a target.
- If the active policy or claim list is empty, document registration is blocked.
- Creating policy or claim records inside document registration would expand the responsibility of that screen too much.
- Policy and claim management should remain separate from document registration, even if the first implementation appears in the same `MainWindow`.

## D. Design Constraints

- Do not use real personal information samples.
- Do not use real family names.
- Do not use real policy numbers.
- Do not use real claim numbers.
- Do not use real insurer names.
- Do not use real hospital names.
- Do not use real diagnosis names or diagnosis codes.
- Keep JSON storage as the persistence direction.
- Do not introduce DB, SQLite, OCR, or repository implementation in this phase.
- Keep `DocumentLinkCoordinator` active target validation.
- Keep responsibilities separated:
  - Document Registration: attach/register a document and link it to an existing active target.
  - Policy/Claim Management: create, list, edit candidate, and disable policy/claim targets.

## E. Explicit Non-Scope

- No implementation.
- No XAML changes.
- No ViewModel changes.
- No tests.
- No app launch.
- No `OpenFileDialog` execution.
- No registration workflow runtime execution.
- No DB implementation.
- No SQLite implementation.
- No OCR implementation.
- No seed data implementation.
- No Git commit.
- No Git reset.
- No Git checkout.
- No Git clean.

Future implementation phases should keep the following as separate decision items or non-scope candidates:

- OCR-based automatic policy or claim creation.
- Real insurer sample data.
- Real hospital sample data.
- Real diagnosis sample data.
- Insurer API integration.
- Hospital or treatment data integration.
- Real family member name management.

## F. UX Architecture Options

### Option A: Add Policy/Claim Management Section to MainWindow

Add a separate management section to the existing `MainWindow`. The document registration area remains responsible for file registration and target selection. The new section manages active policy and claim records.

Pros:

- Smaller implementation than a new window.
- Fits the current single-window MVP structure.
- Allows MVP progress without navigation or dialog lifecycle work.
- Can reuse existing app composition and `IPolicyClaimStorageService`.

Cons:

- `MainWindow` can become crowded.
- Registration and management are visible on the same screen.
- UX separation depends on layout discipline.

Judgment:

- Recommended first for Phase 3C initial MVP.

### Option B: Separate Policy/Claim Management Window

Create a separate window or dialog for policy and claim management.

Pros:

- Clear responsibility split.
- Keeps the registration screen simple.
- Better long-term fit if policy/claim management grows.

Cons:

- Requires window or dialog lifecycle decisions.
- Requires broader navigation and composition work.
- Requires additional runtime verification.
- Likely too much for the immediate next phase.

Judgment:

- Good future direction, but not recommended for the next minimal phase.

### Option C: Quick Create from Document Registration

Add inline quick-create controls directly inside the document registration flow.

Pros:

- User can create a target without leaving the registration flow.
- Reduces empty-list friction.

Cons:

- Conflicts with the Phase 3B rejection of quick create in document registration.
- Tangles registration, creation, validation, and rollback responsibilities.
- Makes document registration harder to reason about.
- Unsuitable for the current MVP boundary.

Judgment:

- Continues to be rejected.

Recommendation:

- Use Option A first for Phase 3C.
- Keep Option B as a future refinement.
- Keep Option C rejected.

## G. Minimum Policy Management Scope

Minimum behavior candidates:

- Show active policy list.
- Create policy.
- Accept display title input.
- Generate an internal synthetic-safe id.
- Disable selected policy.
- Disabled policy disappears from document registration dropdown after reload.
- Decide whether edit is included in Phase 3C or deferred.

Decision items:

- Minimum policy fields.
- User-facing policy label.
- Id generation rule.
- Disable behavior when linked claims exist.
- Disable behavior when linked documents exist.
- Whether edit is included or deferred.

Privacy rule:

- Do not use real policy numbers or real insurer names in samples or tests.
- Use synthetic-safe values only, such as `policy_demo_001` and `policy_title_demo`.

## H. Minimum Claim Management Scope

Minimum behavior candidates:

- Show active claim list.
- Create claim.
- Require an active policy for claim creation.
- Accept claim display title input.
- Generate an internal synthetic-safe id.
- Disable selected claim.
- Disabled claim disappears from document registration dropdown after reload.
- Decide whether edit is included in Phase 3C or deferred.

Decision items:

- Whether an active policy is always required for claim creation.
- Behavior when the parent policy is disabled.
- Minimum claim fields.
- User-facing claim label.
- Disable behavior when linked documents exist.
- Whether edit is included or deferred.

Privacy rule:

- Do not use real claim numbers, hospital names, diagnosis names, or diagnosis codes in samples or tests.
- Use synthetic-safe values only, such as `claim_demo_001` and `claim_title_demo`.

## I. Disable / Relationship Policy Candidates

Policy disable candidates:

- Candidate A: Disable policy only and keep existing active claims.
- Candidate B: Cascade disable active claims under the policy.
- Candidate C: Block policy disable when active claims exist.

Recommendation candidate:

- Choose Candidate C or Candidate B for MVP.
- Candidate C is simpler and preserves data integrity without confirmation UX.
- Candidate B may be acceptable later, but it is riskier without explicit confirmation UI.
- Candidate A can leave active claims under a disabled policy and can harm relationship integrity.

Claim disable candidate:

- Disable the claim only.
- Do not delete linked document files.
- Do not delete existing document link metadata.
- Disabled claim cannot be selected as a new document registration target.
- Existing document links remain as history.

## J. Document Link Impact Policy

Policy or claim disable should not delete document files.

Policy or claim disable should not delete existing link metadata.

Disabled policy or claim records should not be selectable for new document registration.

Existing link display policy is deferred.

Phase 3C is about creation and management UX for targets. It is not a document link history viewer phase.

## K. ViewModel / Service Scope Candidates

Candidate ViewModels:

- `PolicyClaimManagementViewModel`
- `PolicyEditorViewModel`
- `ClaimEditorViewModel`

Simple MVP alternative:

- Add management state and commands directly to `DocumentRegistrationViewModel`.

Recommendation:

- Use a separate `PolicyClaimManagementViewModel`, even if the first UI is placed in `MainWindow`.
- This keeps document registration responsibilities smaller and makes future window extraction easier.

Service scope:

- Use existing `IPolicyClaimStorageService`.
- Use existing `AddPolicyAsync`.
- Use existing `AddClaimAsync`.
- Use existing `DisablePolicyAsync`.
- Use existing `DisableClaimAsync`.
- Do not introduce repository, DB, or SQLite.
- Extend the service only if a concrete implementation gap appears.

## L. XAML Scope Candidates

MVP section candidates:

- Policy Management active policy list.
- Policy display title input.
- Create policy button.
- Disable selected policy button.
- Claim Management active policy selector for claim creation.
- Active claim list.
- Claim display title input.
- Create claim button.
- Disable selected claim button.
- Message area.

Rules:

- Do not add quick create inside document registration.
- Do not use real-data-looking placeholders.
- Use synthetic-safe placeholders only.

## M. Test Scope Candidates

Candidate tests:

- Create policy adds an active policy.
- Disable policy removes the policy from active list.
- Create claim requires an active policy.
- Create claim adds an active claim.
- Disable claim removes the claim from active list.
- Disabled policy cannot be used for claim creation.
- Policy with active claim disable behavior follows the chosen policy.
- Document registration dropdown reflects newly created active policy/claim after reload.
- Actual document files are not created.
- Project root `attachments/` and `data/local/` remain clean in unit tests.
- Test data uses synthetic-safe ids and titles only.

## N. Privacy / Sample Data Policy

Forbidden sample data:

- Real family names.
- Real policy numbers.
- Real claim numbers.
- Real insurer names.
- Real hospital names.
- Real diagnosis names.
- Real diagnosis codes.
- OCR results from real documents.
- Real user file metadata.

Allowed synthetic-safe sample values:

- `policy_demo_001`
- `claim_demo_001`
- `policy_title_demo`
- `claim_title_demo`

## O. Recommended Phase Split

### Phase 3C-1: User Decision Record

Decide:

- Option A, B, or C.
- Policy disable relationship policy.
- Claim disable policy.
- Whether edit is included.
- Id generation rule.
- Minimum display title fields.
- `MainWindow` section versus separate window.

### Phase 3C-2: Implementation Plan

Write a concrete implementation plan after the user decision record.

### Phase 3C-3: Minimal Implementation

Implement only the approved minimum scope.

### Phase 3C-4: Commit Candidate Review

Review changed files, tests, safety boundaries, and commit readiness.

### Phase 3D: Runtime Manual Validation

Validate the runtime path separately after implementation.

## P. Risks

- `MainWindow` may become crowded if management and registration are not visually separated.
- Simplified policy/claim UX may differ from the real insurance domain model.
- Excluding real policy or claim numbers can reduce user distinction between records.
- Wrong disable or cascade policy can harm data integrity.
- Wrong linked document handling can affect rollback or metadata integrity.
- Runtime validation is separate and remains required after implementation.

## Q. Next Recommendation

Create a `Policy/Claim Creation Management Phase 3C user decision record` document.
