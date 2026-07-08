# Policy/Claim Validation Harness Pilot String Extraction Scope Plan

## A. Status

Status: EXTRACTION_PLAN_ONLY

Marker:

POLICY_CLAIM_VALIDATION_HARNESS_PILOT_STRING_EXTRACTION_SCOPE_PLANNED

This document plans pilot string extraction only.

No XAML is modified by this document.

No resource file is modified by this document.

No code is modified by this document.

No localization is implemented by this document.

No direct Korean replacement is authorized by this document.

No wireframe port is authorized by this document.

## B. Baseline

Record:

- latest commit:
  14f0541 feat(familyclaimref): add ui resource infrastructure

- source docs reviewed:
  - docs/217_POLICY_CLAIM_UI_PHASE_ENTRY_DECISION.md
  - docs/218_POLICY_CLAIM_UI_STRING_INVENTORY_REVIEW.md
  - docs/219_POLICY_CLAIM_KOREAN_RESOURCE_EXTRACTION_ARCHITECTURE_PLAN.md
  - docs/221_POLICY_CLAIM_RESOURCE_INFRASTRUCTURE_IMPLEMENTATION_SCOPE_PLAN.md
  - docs/222_POLICY_CLAIM_RESOURCE_KEY_NAMING_AND_STRING_OWNERSHIP_DECISION.md
  - docs/223_POLICY_CLAIM_RESOURCE_INFRASTRUCTURE_TEST_VALIDATION_PLAN.md
  - docs/225_POLICY_CLAIM_RESOURCE_INFRASTRUCTURE_IMPLEMENTATION_RESULT_REVIEW.md

## C. Current Infrastructure State

Record:

- ResourceDictionary exists:
  app/FamilyClaimRef.App/Resources/UiStrings.xaml

- App.xaml resource merge exists:
  yes

- UiTextKeys exists:
  app/FamilyClaimRef.App/Services/Localization/UiTextKeys.cs

- IUiTextProvider exists:
  app/FamilyClaimRef.App/Services/Localization/IUiTextProvider.cs

- ResourceUiTextProvider exists:
  app/FamilyClaimRef.App/Services/Localization/ResourceUiTextProvider.cs

- provider tests exist:
  tests/FamilyClaimRef.App.Tests/Services/Localization/ResourceUiTextProviderTests.cs

- full tests from docs/225:
  306 passed

## D. Pilot Extraction Decision

Recommended pilot scope:

Only extract already implemented pilot keys from MainWindow.xaml.

Pilot keys:

- Ui.App.Title
- Ui.Document.SourceFileSection
- Ui.Action.SelectFile
- Ui.Status.RegistrationSection
- Ui.DevHarness.Warning.LocalMvpValidation

Meaning:

- Use resource keys for a tiny, low-risk static XAML label set.
- Do not change layout.
- Do not change ViewModel behavior.
- Do not change workflow/storage behavior.
- Do not localize to Korean yet.
- Do not extract all MainWindow strings at once.
- Do not touch management panel strings yet.
- Do not touch validation/status ViewModel messages yet.

## E. Out of Scope For Pilot

Record:

- ViewModel validation/status message extraction
- PolicyClaimManagementViewModel message extraction
- MainWindowViewModel changes
- DocumentRegistrationViewModel constructor changes
- message provider injection
- business duplicate UX copy
- Korean copy finalization
- ResourceDictionary culture switching
- dynamic language switching
- product UI shell
- wireframe port
- UI redesign
- layout/styling changes
- DB/SQLite/OCR/repository
- data/claimdoc
- cleanup

## F. Future Implementation Candidate

If separately approved later, the pilot implementation may modify only:

- app/FamilyClaimRef.App/MainWindow.xaml

and only to replace the following literals with existing resource keys:

- Window Title / top app title:
  Ui.App.Title
- Local MVP warning text:
  Ui.DevHarness.Warning.LocalMvpValidation
- Source file GroupBox header:
  Ui.Document.SourceFileSection
- Select file button content:
  Ui.Action.SelectFile
- Registration status GroupBox header:
  Ui.Status.RegistrationSection

No new resource keys should be required in the first pilot implementation.

If a missing key is discovered, STOP_AND_REPORT instead of broadening scope.

## G. Acceptance Criteria For Future Pilot Implementation

A future pilot implementation is acceptable only if:

- build passes.
- full tests pass.
- ResourceUiTextProviderTests still pass.
- App.xaml resource merge remains valid.
- MainWindow layout is unchanged.
- visual text remains semantically unchanged.
- no direct Korean replacement occurs.
- no ViewModel production code changes occur.
- no workflow/storage behavior changes occur.
- no data/claimdoc access occurs.

## H. Scope Judgment

POLICY_CLAIM_VALIDATION_HARNESS_PILOT_STRING_EXTRACTION_SCOPE_READY
