# Policy/Claim Validation Harness Pilot Resource Key Mapping Review

## A. Status

Status: MAPPING_REVIEW_ONLY

Marker:

POLICY_CLAIM_VALIDATION_HARNESS_PILOT_RESOURCE_KEY_MAPPING_RECORDED

This document records mapping only.

No XAML is modified by this document.

No resource file is modified by this document.

No string is changed by this document.

## B. Baseline

Record:

- latest commit:
  14f0541 feat(familyclaimref): add ui resource infrastructure

## C. Mapping Rules

Record:

- Map only existing pilot keys.
- Do not introduce new keys in pilot extraction implementation.
- Do not localize values to Korean in pilot extraction implementation.
- Do not change layout.
- Do not map ViewModel runtime messages in this pilot.
- Do not map management panel strings in this pilot.
- Do not map business duplicate UX strings in this pilot.

## D. Pilot Mapping Table

| Source file | Current literal | Current element / usage | Existing resource key | Resource file | Product-facing or dev-harness-only | Future implementation action |
|---|---|---|---|---|---|---|
| app/FamilyClaimRef.App/MainWindow.xaml | FamilyClaimRef | Window Title and/or top app title if present | Ui.App.Title | app/FamilyClaimRef.App/Resources/UiStrings.xaml | product-facing candidate | replace literal binding with StaticResource or equivalent resource lookup only in approved implementation batch |
| app/FamilyClaimRef.App/MainWindow.xaml | Local MVP validation screen. Do not use real personal, insurer, hospital, diagnosis, policy number, or claim number samples. | Validation harness warning text | Ui.DevHarness.Warning.LocalMvpValidation | app/FamilyClaimRef.App/Resources/UiStrings.xaml | validation-harness-only | replace literal with resource lookup only in approved implementation batch |
| app/FamilyClaimRef.App/MainWindow.xaml | Source file | GroupBox Header literal | Ui.Document.SourceFileSection | app/FamilyClaimRef.App/Resources/UiStrings.xaml | product-facing candidate | replace GroupBox Header literal with resource lookup only in approved implementation batch |
| app/FamilyClaimRef.App/MainWindow.xaml | Select file | Button Content literal | Ui.Action.SelectFile | app/FamilyClaimRef.App/Resources/UiStrings.xaml | product-facing candidate | replace Button Content literal with resource lookup only in approved implementation batch |
| app/FamilyClaimRef.App/MainWindow.xaml | Registration status | GroupBox Header literal | Ui.Status.RegistrationSection | app/FamilyClaimRef.App/Resources/UiStrings.xaml | product-facing candidate | replace GroupBox Header literal with resource lookup only in approved implementation batch |

## E. Deferred Mapping

Record:

Deferred strings include:

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
- Policy/Claim Management
- Create policy
- Disable policy
- Create claim
- Disable claim
- all ViewModel validation/status messages
- all business duplicate warning/copy
- all wireframe product UI strings

Reason:

- Need small pilot first.
- Need avoid broad XAML churn.
- Need preserve validation harness behavior.
- Need separate ViewModel message provider injection plan.

## F. Mapping Judgment

POLICY_CLAIM_VALIDATION_HARNESS_PILOT_RESOURCE_KEY_MAPPING_READY
