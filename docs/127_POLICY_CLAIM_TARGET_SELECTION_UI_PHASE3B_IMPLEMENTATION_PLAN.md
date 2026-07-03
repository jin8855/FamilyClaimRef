# Policy / Claim Target Selection UI Phase 3B Implementation Plan

## A. Status Marker

POLICY_CLAIM_TARGET_SELECTION_UI_PHASE3B_IMPLEMENTATION_PLAN_CREATED

## B. Background

- Phase 1: `Policy` / `Claim` storage was added.
- Phase 2: `DocumentLinkCoordinator` active target validation was added.
- `docs/126_MAINWINDOW_TARGET_SELECTION_UI_SCOPE_DESIGN.md` recommends Option B, active `Policy` / `Claim` dropdown.
- The current `MainWindow` has a minimal `TargetKind` / `TargetId` direct input area, but it does not provide a user-friendly target selection UI.
- The current document registration flow still requires users to know or type a target id manually.

## C. Implementation Goal

Phase 3B should implement the minimum UI and ViewModel changes needed to select an active document registration target.

Goals:

- Show active policy / active claim options in the UI.
- Let the user select the target used for document registration.
- Convert the selected target into the existing document registration request target kind/id contract.
- Do not show disabled policy/claim options in the UI.
- Keep `DocumentLinkCoordinator` as the final validation boundary.
- Provide an empty state message when there is no selectable active target.
- Add focused tests for target option loading, selection, validation, and existing registration behavior.

## D. Explicit Non-Scope

- No policy creation UI.
- No claim creation UI.
- No policy edit UI.
- No claim edit UI.
- No policy disable UI.
- No claim disable UI.
- No quick create inside document registration.
- No seed data implementation.
- No DB/SQLite/OCR/repository implementation.
- No runtime manual validation in this phase.
- No app launch in this phase.
- No OpenFileDialog execution in this phase.
- No actual registration workflow execution outside automated tests.
- No commit in this planning task.

## E. Required Decisions

### 1. UI Method

Recommended:

- Use Option B: active `Policy` / `Claim` dropdown.

Rejected in Phase 3B:

- Option C: quick create inside registration.

Fallback candidate:

- Option A: direct target id input may remain as a temporary or advanced fallback if Phase 3B implementation needs it.

### 2. Direct Target Id Input Retention

Candidate A: Hide or remove the current direct `TargetKind` / `TargetId` input UI.

Pros:

- Reduces user-facing confusion.
- Pushes the screen toward the intended MVP UX.

Cons:

- Reduces dev fallback when no policy/claim creation UX or seed data exists.

Candidate B: Keep direct input as a dev fallback or advanced section.

Pros:

- Allows validation testing even when active target lists are empty.
- Preserves the current minimal contract during transition.

Cons:

- Can confuse real users.
- Makes the UI look less finished.

Recommendation:

- For Phase 3B, prefer replacing the visible direct id input with dropdown-based selection.
- If fallback is kept, it should be visually secondary and explicitly marked as non-primary development fallback.
- Final fallback behavior should be confirmed in a user decision record before implementation.

### 3. Active List Load Timing

Options:

- Load during ViewModel construction.
- Load through explicit `LoadTargetOptionsAsync`.
- Load from a `MainWindow` loaded event.
- Lazy load during registration command.

Recommendation:

- Add an explicit `LoadTargetOptionsAsync` candidate and invoke it from the UI lifecycle in a controlled way during implementation.
- Avoid doing async work directly in the constructor.
- Avoid lazy loading only during registration because the user needs target availability before pressing Register.

Note:

- This planning task does not run the app or any runtime event.

### 4. Register Button Policy

Options:

- Disable Register when the selected target is missing.
- Keep Register enabled and block through validation message.

Recommendation:

- Use validation message as the minimum Phase 3B requirement.
- Button disable can be added if the existing binding structure supports it without broad command refactoring.
- Coordinator validation remains mandatory either way.

### 5. Target Display Label

The display label must not use real insurer names, hospital names, diagnosis names, diagnosis codes, real policy numbers, or real claim numbers.

Candidate display:

- Policy: internal id + display title candidate.
- Claim: internal id + parent policy id candidate + display title candidate.

Examples for docs/tests only:

- `policy_demo_001`
- `claim_demo_001`
- `document_demo_001`

Final display label should be determined from available `PolicyRecord` and `ClaimRecord` fields, not from personal or external real-world data.

## F. IPolicyClaimStorageService API Review

Current API:

- `GetPoliciesAsync`
- `GetPolicyAsync`
- `AddPolicyAsync`
- `DisablePolicyAsync`
- `GetClaimsAsync`
- `GetClaimsByPolicyIdAsync`
- `GetClaimAsync`
- `AddClaimAsync`
- `DisableClaimAsync`
- `PolicyExistsAsync`
- `ClaimExistsAsync`

Observed behavior:

- `JsonPolicyClaimStorageService.GetPoliciesAsync` returns only records where `DisabledAt is null`.
- `JsonPolicyClaimStorageService.GetClaimsAsync` returns only records where `DisabledAt is null`.
- `GetClaimsByPolicyIdAsync` also starts from active claims.
- `PolicyExistsAsync` and `ClaimExistsAsync` use active-only lookup.

Assessment:

- API sufficient for initial Phase 3B active dropdown implementation.
- No immediate service API extension is required for active-only policy and claim lists.
- If the UI later needs inactive record visibility, that should be a separate management phase decision.

Rejected for Phase 3B:

- Repository abstraction addition.
- DB/SQLite implementation.
- Broad storage API redesign.

## G. ViewModel Implementation Plan

Potential ViewModel changes:

- Inject `IPolicyClaimStorageService` into `DocumentRegistrationViewModel`, or introduce a small target option provider if direct injection becomes too broad.
- Add active policy / claim option collections.
- Add selected policy / claim state.
- Preserve the final workflow contract as target kind/id.
- Add empty state and target validation messages.

Candidate properties:

- `AvailablePolicies`
- `AvailableClaims`
- `SelectedTargetKind`
- `SelectedPolicyId`
- `SelectedClaimId`
- `TargetSelectionMessage`
- `HasAvailablePolicies`
- `HasAvailableClaims`

Candidate command/flow:

- `LoadTargetOptionsAsync`
- Target kind change switches the visible selection list.
- Selected policy updates the policy target id equivalent.
- Selected claim updates the claim target id equivalent.
- Register validation checks target selection before invoking workflow.

Existing contract to preserve:

- Policy registration must still call `RegisterPolicyDocumentAsync`.
- Claim registration must still call `RegisterClaimDocumentAsync`.
- The request still needs a target id.
- `DocumentLinkCoordinator` validation must remain in place.

Naming note:

- The exact property and method names should follow the current `DocumentRegistrationViewModel` style.
- Names in this document are implementation candidates, not final API commitments.

## H. XAML Implementation Plan

Candidate UI changes:

- Rework the current `Target` group in `MainWindow.xaml`.
- Keep a target type selector.
- Add policy dropdown for active policy selection.
- Add claim dropdown for active claim selection.
- Add empty state text for no active policy / claim.
- Add target selection validation message if needed.

Policy:

- Disabled targets must not appear in dropdowns.
- Create buttons must not be added in Phase 3B.
- Quick create links must not be added in Phase 3B.
- Empty state should show a short message only.

Candidate empty state text:

- `No active policy is available for selection.`
- `No active claim is available for selection.`

Note:

- The current `MainWindow.xaml.cs` has only button click handlers. Phase 3B should avoid changing it if binding and existing handlers are enough.

## I. Test Plan

Candidate tests for Phase 3B:

1. ViewModel initializes active policy/claim options.
2. Disabled policy/claim records are not exposed in available options.
3. Selecting a policy sets the target kind/id used for registration.
4. Selecting a claim sets the target kind/id used for registration.
5. No active policy shows an empty state message.
6. No active claim shows an empty state message.
7. Registration without selected target is blocked or shows validation message.
8. Existing registration tests remain passing.
9. Existing target validation and rollback tests remain passing.

Test data rules:

- Use synthetic ids only.
- Allowed examples:
  - `policy_demo_001`
  - `claim_demo_001`
  - `document_demo_001`
- Do not use real family names, real policy numbers, real claim numbers, real insurer names, real hospital names, real diagnosis names, diagnosis codes, OCR output, or real file metadata.

Execution scope for Phase 3B implementation:

- Unit tests only.
- No app launch.
- No OpenFileDialog runtime execution.
- No manual registration workflow execution.

## J. File Change Plan for Phase 3B

Expected modified files:

- `app/FamilyClaimRef.App/ViewModels/DocumentRegistrationViewModel.cs`
- `app/FamilyClaimRef.App/MainWindow.xaml`
- `tests/FamilyClaimRef.App.Tests/DocumentRegistrationViewModelTests.cs`

Conditional modified files:

- `app/FamilyClaimRef.App/Services/Storage/IPolicyClaimStorageService.cs`
- `app/FamilyClaimRef.App/Services/Storage/JsonPolicyClaimStorageService.cs`
- `tests/FamilyClaimRef.App.Tests/JsonPolicyClaimStorageServiceTests.cs`

Current assessment:

- The existing storage list API appears sufficient for Phase 3B.
- Conditional service changes should be avoided unless implementation discovers a concrete gap.

Files expected to remain unchanged:

- `app/FamilyClaimRef.App/Composition/AppServices.cs`, unless ViewModel constructor injection requires composition update.
- `app/FamilyClaimRef.App/Services/Storage/DocumentLinkCoordinator.cs`
- `app/FamilyClaimRef.App/Services/Storage/DocumentRegistrationWorkflow.cs`
- `app/FamilyClaimRef.App/MainWindow.xaml.cs`, if existing button handlers remain enough.

## K. Privacy / Sample Data Policy

Forbidden:

- Real family names.
- Real policy numbers.
- Real claim numbers.
- Real insurer names.
- Real hospital names.
- Real diagnosis names.
- Real diagnosis codes.
- Real OCR result.
- Real user file metadata.

Allowed:

- Synthetic ids:
  - `policy_demo_001`
  - `claim_demo_001`
  - `document_demo_001`
- Generic display titles that do not identify a real person, organization, medical provider, or diagnosis.

## L. Verification Plan for Phase 3B

Required verification for the future implementation task:

- `git diff --check`
- `dotnet build FamilyClaimRef.sln`
- `dotnet test FamilyClaimRef.sln`
- project root `attachments/` files count
- project root `data/local` files count
- DB/SQLite unexpected file check
- actual personal sample check

Prohibited in Phase 3B implementation unless separately approved:

- app launch.
- OpenFileDialog runtime execution.
- runtime manual registration workflow.

This planning document does not run build/test because this is a documentation-only change.

## M. Risks

- Active target list can be empty, blocking the user without a policy/claim creation UX.
- Actual usability remains limited until policy/claim creation and management UX exists.
- If direct id input remains visible, the target UI can feel inconsistent.
- If direct id input is removed, dev fallback decreases.
- Existing service list API appears sufficient, but the UI may need a display-friendly projection later.
- Display labels may be too technical if they expose only internal ids.
- Runtime validation remains a separate phase.

## N. Recommended Phase 3B Direction

Recommendation:

- Implement Option B active dropdown as the Phase 3B default.
- Exclude Option C quick create.
- Do not remove coordinator validation.
- Prefer no storage service API expansion unless implementation finds a concrete gap.
- Keep `MainWindow.xaml.cs` unchanged if existing click handlers are enough.
- Keep the final workflow input contract as target kind/id.
- Use ViewModel binding as the main implementation path.
- Add focused ViewModel tests using synthetic ids only.

Direct id fallback:

- Do not treat direct id input as the primary UI.
- Decide in a user decision record whether to hide, remove, or keep it as a dev fallback.

Service API sufficiency:

- Current API is sufficient for active dropdown MVP because existing list methods return active-only records.

## O. Next Recommendation

Next recommended task:

- Create `Policy/Claim Target Selection UI Phase 3B user decision record`.

Alternative next task after user approval:

- Write `Phase 3B minimal implementation instruction`.

Do not proceed directly to implementation from this document without recording the user decision or writing an explicit implementation instruction.
