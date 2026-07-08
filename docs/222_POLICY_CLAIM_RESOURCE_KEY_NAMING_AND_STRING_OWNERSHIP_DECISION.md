# Policy/Claim Resource Key Naming and String Ownership Decision

## A. Status

Status: DECISION_ONLY

Marker:

POLICY_CLAIM_RESOURCE_KEY_NAMING_AND_STRING_OWNERSHIP_DECIDED

No resource keys are implemented by this document.

No resource file is created by this document.

No string is changed by this document.

## B. Baseline

Record:

- latest commit:
  781e3ef docs(familyclaimref): plan ui phase entry and localization resources

- source docs reviewed:
  - docs/218_POLICY_CLAIM_UI_STRING_INVENTORY_REVIEW.md
  - docs/219_POLICY_CLAIM_KOREAN_RESOURCE_EXTRACTION_ARCHITECTURE_PLAN.md

## C. Key Naming Principles

Record:

1. Keys must describe product meaning, not current English text.
2. Keys must be stable across Korean copy changes.
3. Dev validation harness keys must be separate from product UI keys.
4. Status/validation messages must be separate from labels/buttons.
5. Business duplicate UX keys must not be created until the product duplicate decision is made.
6. Environment variable names, path placeholders, commit messages, and machine-readable markers are not localizable keys.

## D. Recommended Prefixes

Record approved prefix candidates:

- Ui.App.*
- Ui.Nav.*
- Ui.Document.*
- Ui.Target.*
- Ui.Policy.*
- Ui.Claim.*
- Ui.Management.*
- Ui.Action.*
- Ui.Status.*
- Ui.Validation.*
- Ui.Diagnostics.*
- Ui.DevHarness.*
- Ui.Product.*
- Ui.BusinessDuplicate.* only after separate product decision

## E. Ownership Categories

Record:

### Product-facing candidate strings

- app title
- document registration labels
- target selection labels
- policy/claim labels
- action buttons
- validation/status messages likely visible to users

### Validation-harness-only strings

- Local MVP warning
- Is busy diagnostic
- Last registration summary
- synthetic target management helper text
- dev-only management panel labels

### Non-localized strings

- machine-readable markers
- environment variable names
- path placeholders
- generated ids
- synthetic test titles
- exception type names
- commit messages
- internal class names

## F. Initial Pilot Key Set Recommendation

Recommend a small future pilot set.

Example pilot scope:

XAML labels/buttons:

- Ui.App.Title
- Ui.Document.SourceFileSection
- Ui.Action.SelectFile
- Ui.Document.SelectedFileLabel
- Ui.Target.SelectionSection
- Ui.Target.KindLabel
- Ui.Document.MetadataSection
- Ui.Document.TypeLabel
- Ui.Document.DisplayTitleLabel
- Ui.Document.ReferenceDateLabel
- Ui.Action.RegisterDocument
- Ui.Status.RegistrationSection

ViewModel/status messages:

- Ui.Validation.SelectFile
- Ui.Validation.SelectTargetKind
- Ui.Validation.SelectTarget
- Ui.Validation.SelectDocumentType
- Ui.Validation.EnterDisplayTitle
- Ui.Validation.SelectReferenceDate
- Ui.Status.DocumentRegistrationCompleted
- Ui.Status.DocumentRegistrationFailed
- Ui.Status.FileSelected

Do not implement this pilot in this batch.

## G. Direct Korean Replacement Decision

Record:

Direct Korean replacement:

REJECT

Reason:

- hides string ownership
- makes future copy changes harder
- mixes validation harness and product UI
- prevents resource key review
- makes tests fragile

## H. Decision Judgment

POLICY_CLAIM_RESOURCE_KEY_NAMING_READY
