# Policy/Claim Resource Infrastructure Implementation Scope Plan

## A. Status

Status: IMPLEMENTATION_PLAN_ONLY

Marker:

POLICY_CLAIM_RESOURCE_INFRASTRUCTURE_IMPLEMENTATION_SCOPE_PLANNED

This document plans resource infrastructure implementation only.

No code is modified by this document.

No XAML is modified by this document.

No ViewModel is modified by this document.

No resource file is created by this document.

No localization is implemented by this document.

No wireframe port is authorized by this document.

## B. Baseline

Record:

- latest commit:
  781e3ef docs(familyclaimref): plan ui phase entry and localization resources

- source docs reviewed:
  - docs/214_POLICY_CLAIM_REMAINING_PRODUCT_UI_BOUNDARY_DECISION.md
  - docs/217_POLICY_CLAIM_UI_PHASE_ENTRY_DECISION.md
  - docs/218_POLICY_CLAIM_UI_STRING_INVENTORY_REVIEW.md
  - docs/219_POLICY_CLAIM_KOREAN_RESOURCE_EXTRACTION_ARCHITECTURE_PLAN.md
  - docs/220_POLICY_CLAIM_UI_PHASE_ENTRY_DOCS_COMMIT_CANDIDATE_REVIEW.md

## C. Current Findings

Record confirmed findings only:

- existing resource/localization source files:
  not found under app/FamilyClaimRef.App excluding bin/obj.
- App.xaml:
  has an Application.Resources block, but no dedicated UI resource dictionary is currently defined.
- current XAML literal string surface:
  MainWindow.xaml contains direct Title/Text/Content/Header literals for the validation harness.
- current ViewModel message literal surface:
  DocumentRegistrationViewModel and PolicyClaimManagementViewModel contain direct status, validation, target selection, and management message literals.
- current validation harness role:
  MainWindow remains a validation harness.
- current product UI status:
  product UI implementation is not authorized and has not started.

Use docs/218 as primary inventory source.

## D. Recommended First Implementation Scope

Recommend a minimal first implementation scope:

1. Add resource infrastructure only.
2. Do not redesign UI.
3. Do not port wireframes.
4. Do not change layout.
5. Do not replace all strings in one batch.
6. Add XAML label resource mechanism.
7. Add ViewModel/status message abstraction.
8. Add tests for resource lookup/message provider.
9. Extract only a small pilot string set after infrastructure exists.
10. Keep current validation harness behavior unchanged.

## E. Proposed Code Areas For Future Implementation

Record candidate future files, adjusted to repository convention after read-only inspection:

Candidate resource infrastructure:

- app/FamilyClaimRef.App/Resources/UiStrings.xaml
- app/FamilyClaimRef.App/Services/Localization/IUiTextProvider.cs
- app/FamilyClaimRef.App/Services/Localization/ResourceUiTextProvider.cs
- app/FamilyClaimRef.App/Services/Localization/UiTextKey.cs or equivalent constants class

Candidate composition updates:

- app/FamilyClaimRef.App/Composition/AppServices.cs
- app/FamilyClaimRef.App/App.xaml

Candidate tests:

- tests/FamilyClaimRef.App.Tests/Services/Localization/ResourceUiTextProviderTests.cs
- tests/FamilyClaimRef.App.Tests/ViewModels/... only after message provider injection is approved

Important:

- These are candidates, not implementation approval.
- If repository convention suggests different names, record the better candidate names.
- Do not create files in this batch.

## F. Recommended Architecture Boundary

Record:

- XAML visual labels/buttons/headers:
  ResourceDictionary or equivalent static resource lookup.

- ViewModel validation/status messages:
  IUiTextProvider or equivalent message provider.

- Dev harness diagnostic strings:
  separate key namespace such as Ui.DevHarness.*.

- Product UI strings:
  separate key namespace, not implemented yet.

- Business duplicate warning copy:
  deferred until product duplicate UX decision.

## G. Out of Scope

Record:

- direct Korean replacement
- full string extraction in one batch
- wireframe port
- product shell implementation
- XAML redesign
- ViewModel behavioral change
- storage/workflow behavior change
- DB/SQLite/OCR/repository
- data/claimdoc
- real document ingestion
- cleanup

## H. Future Implementation Acceptance Criteria

A future implementation is acceptable only if:

- build passes.
- tests pass.
- current validation harness behavior remains unchanged.
- no workflow/storage behavior changes.
- no direct bulk Korean replacement.
- resource lookup has deterministic fallback behavior.
- missing key behavior is tested.
- message provider can be unit tested.
- XAML uses resource keys only for approved pilot scope.
- ViewModel messages are not directly tied to fragile final product copy.
- data/claimdoc remains untouched.

## I. Scope Judgment

POLICY_CLAIM_RESOURCE_INFRASTRUCTURE_IMPLEMENTATION_SCOPE_READY
