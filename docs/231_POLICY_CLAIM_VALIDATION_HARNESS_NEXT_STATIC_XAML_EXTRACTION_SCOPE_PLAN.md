# Policy/Claim Validation Harness Next Static XAML Extraction Scope Plan

## A. Status

Status: EXTRACTION_PLAN_ONLY

Marker:

POLICY_CLAIM_VALIDATION_HARNESS_NEXT_STATIC_XAML_EXTRACTION_SCOPE_PLANNED

This document plans the next static XAML extraction only.

No XAML is modified by this document.

No resource file is modified by this document.

No code is modified by this document.

No localization is implemented by this document.

No direct Korean replacement is authorized by this document.

No wireframe port is authorized by this document.

## B. Baseline

Record:

- latest commit:
  478e6cd refactor(familyclaimref): extract validation harness pilot strings

- source docs reviewed:
  - docs/218_POLICY_CLAIM_UI_STRING_INVENTORY_REVIEW.md
  - docs/222_POLICY_CLAIM_RESOURCE_KEY_NAMING_AND_STRING_OWNERSHIP_DECISION.md
  - docs/225_POLICY_CLAIM_RESOURCE_INFRASTRUCTURE_IMPLEMENTATION_RESULT_REVIEW.md
  - docs/230_POLICY_CLAIM_VALIDATION_HARNESS_PILOT_STRING_EXTRACTION_RESULT_REVIEW.md

## C. Completed Pilot Extraction

Record:

- Ui.App.Title applied to Window Title.
- Ui.App.Title applied to top app title.
- Ui.DevHarness.Warning.LocalMvpValidation applied to dev harness warning.
- Ui.Document.SourceFileSection applied to Source file header.
- Ui.Action.SelectFile applied to Select file button.
- Ui.Status.RegistrationSection applied to Registration status header.
- build/test passed after pilot extraction.
- no resource file was modified during pilot extraction.
- no code/test/ViewModel was modified during pilot extraction.

## D. Next Static XAML Extraction Decision

Recommended next scope:

Document registration form static XAML labels only.

Include these literals:

- Selected file
- Target selection
- Target kind
- Policy target
- Claim target
- Document metadata
- Document type
- Display title
- Reference date
- Register
- Validation
- Status
- Last registration summary

Do not include yet:

- Policy/Claim Management
- Create policy
- Disable policy
- Create claim
- Disable claim
- Management message
- all ViewModel validation/status messages
- all business duplicate warning/copy
- all wireframe product UI strings

## E. Rationale

Record:

- This scope continues the successful pilot pattern.
- These strings are static XAML labels/buttons/headers.
- The scope avoids ViewModel message provider injection.
- The scope avoids management panel churn.
- The scope avoids product UI shell or wireframe port.
- The scope avoids final Korean copy decisions.
- The scope is large enough to make progress but still small enough to review.

## F. Future Implementation Candidate

If separately approved later, implementation may modify only:

- app/FamilyClaimRef.App/Resources/UiStrings.xaml
- app/FamilyClaimRef.App/Services/Localization/UiTextKeys.cs
- app/FamilyClaimRef.App/MainWindow.xaml
- docs/235_POLICY_CLAIM_VALIDATION_HARNESS_NEXT_STATIC_XAML_EXTRACTION_RESULT_REVIEW.md

Implementation must:

- add only approved keys.
- replace only approved static XAML literals.
- keep values neutral/current, not final Korean copy.
- avoid direct Korean replacement.
- avoid layout/control hierarchy changes.
- avoid ViewModel changes.
- avoid workflow/storage behavior changes.

## G. Scope Judgment

POLICY_CLAIM_VALIDATION_HARNESS_NEXT_STATIC_XAML_EXTRACTION_SCOPE_READY
