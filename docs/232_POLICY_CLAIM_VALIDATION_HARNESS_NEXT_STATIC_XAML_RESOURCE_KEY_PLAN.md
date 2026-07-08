# Policy/Claim Validation Harness Next Static XAML Resource Key Plan

## A. Status

Status: RESOURCE_KEY_PLAN_ONLY

Marker:

POLICY_CLAIM_VALIDATION_HARNESS_NEXT_STATIC_XAML_RESOURCE_KEYS_PLANNED

No resource key is implemented by this document.

No resource file is modified by this document.

No XAML is modified by this document.

## B. Baseline

Record:

- latest commit:
  478e6cd refactor(familyclaimref): extract validation harness pilot strings

## C. Key Rules

Record:

- keys describe product meaning, not current English text.
- keys remain stable across future Korean copy changes.
- dev harness keys stay separate from product-facing candidate keys.
- status/validation ViewModel messages are not included.
- business duplicate UX keys are not included.
- no direct Korean replacement.

## D. Planned Key Additions

| Current literal | Planned key | Ownership | Resource value for first implementation | Notes |
|---|---|---|---|---|
| Selected file | Ui.Document.SelectedFileLabel | product-facing candidate | Selected file | Displays selected source file. |
| Target selection | Ui.Target.SelectionSection | product-facing candidate | Target selection | Registration target section header. |
| Target kind | Ui.Target.KindLabel | product-facing candidate | Target kind | Target kind selector label. |
| Policy target | Ui.Policy.TargetLabel | product-facing candidate | Policy target | Policy target selector label. |
| Claim target | Ui.Claim.TargetLabel | product-facing candidate | Claim target | Claim target selector label. |
| Document metadata | Ui.Document.MetadataSection | product-facing candidate | Document metadata | Document metadata section header. |
| Document type | Ui.Document.TypeLabel | product-facing candidate | Document type | Document type selector label. |
| Display title | Ui.Document.DisplayTitleLabel | product-facing candidate | Display title | Display title field label. |
| Reference date | Ui.Document.ReferenceDateLabel | product-facing candidate | Reference date | Reference date field label. |
| Register | Ui.Action.RegisterDocument | product-facing candidate | Register | Main document registration action. |
| Validation | Ui.Validation.SectionLabel | validation-harness-only for now | Validation | Validation message display label. |
| Status | Ui.Status.Label | product-facing candidate | Status | General status message display label. |
| Last registration summary | Ui.Status.LastRegistrationSummaryLabel | validation-harness-only | Last registration summary | Harness summary label. |

## E. Keys Not Included

Record:

- Ui.Target.Kind.Policy
- Ui.Target.Kind.Claim
- Ui.Status.IsBusyFormat
- Ui.Management.*
- Ui.DevHarness.ManagementWarning
- Ui.Validation.SelectFile and other ViewModel validation keys
- Ui.Status.DocumentRegistrationCompleted
- Ui.Status.DocumentRegistrationFailed
- Ui.BusinessDuplicate.*
- Ui.Product.*

Reason:

- These require separate scope or message-provider work.

## F. Resource Key Judgment

POLICY_CLAIM_VALIDATION_HARNESS_NEXT_STATIC_XAML_RESOURCE_KEYS_READY
