# Product UI Shell Phase 1 Ui.Product Copy Implementation Plan

## A. Status

PRODUCT_UI_SHELL_PHASE1_UI_PRODUCT_COPY_IMPLEMENTATION_PLAN_ONLY

## B. Marker

POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1_UI_PRODUCT_COPY_IMPLEMENTATION_PLAN_READY

## C. Baseline

- 기준 commit: `21c51ab docs(familyclaimref): plan product shell phase1 entry copy filelist decisions`
- 작업 유형: documentation-only future implementation planning

## D. Purpose

승인된 `Ui.Product.*` value table을 이후 implementation batch에서 어떻게 반영할지 구현 없이 계획한다.

This document does not approve source/resource/test modification.

Approved values are approved for future exact-file-list implementation only.

`Ui.Product.*` implementation candidate scope remains limited to a later separately approved source/resource/test batch.

Ui.Product.* implementation candidate values are documented here, but no implementation is performed now.

## E. Future Implementation Candidate Exact File List

If a later implementation batch is explicitly approved, the expected exact file list candidate is:

- `app/FamilyClaimRef.App/Resources/UiStrings.xaml`
- `app/FamilyClaimRef.App/Services/Localization/UiTextKeys.cs`
- `tests/FamilyClaimRef.App.Tests/Services/Localization/ResourceUiTextProviderTests.cs`
- `docs/340_POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1_UI_PRODUCT_COPY_IMPLEMENTATION_RESULT_REVIEW.md`

The list above is a future candidate only. It is not an approved implementation file list for this batch.

## E-1. Candidate List Boundary

- These 4 files are future `Ui.Product.*` copy source implementation candidate exact file list.
- This is not a ProductShell implementation file list.
- This is not a ProductShellWindow implementation file list.
- This does not mean exact ProductShell implementation file list approval.
- This docs/335~339 batch does not authorize any candidate file modification.
- Future implementation requires separate explicit approval and a separate exact implementation batch.

## F. Future Implementation Target

If separately approved:

- add 8 `Ui.Product.*` keys to `UiStrings.xaml`
- add 8 `Ui.Product.*` constants to `UiTextKeys.cs`
- add `ResourceUiTextProviderTests` assertions for 8 values
- create result review doc

## G. Future Implementation Non-Target

- ProductShellWindow creation
- XAML port
- ProductShellViewModel
- Product view
- MainWindow replacement
- App startup change
- DocumentRegistrationViewModel behavior change
- DB/SQLite/repository/OCR/migration
- data/claimdoc access

## H. Expected Future Count Checks

| Count item | Expected value |
|---|---:|
| existing `Ui.*` baseline before implementation | 56 |
| new `Ui.Product.*` keys if future implementation approved | 8 |
| expected `Ui.*` keys after future implementation | 64 |
| current `UiTextKeys.cs` `Ui.*` constants | 56 |
| future new `Ui.Product.*` constants | 8 |
| future `UiTextKeys.cs` `Ui.*` constants | 64 |
| deleted keys | 0 |
| renamed keys | 0 |

## I. Future Test Responsibilities

Future `ResourceUiTextProviderTests` candidate work must include:

- exact 8 key/value assertions
- total resource key count 64
- total `UiTextKeys.cs` constant count 64
- no duplicate resource key
- no missing constant
- no orphan constant
- existing 56 resource values unchanged
- `Ui.Policy.TargetLabel = 보험 대상` unchanged
- `Ui.Claim.TargetLabel = 청구 대상` unchanged

## J. Current Batch State

| Item | Current batch state |
|---|---|
| `UiStrings.xaml` modified | no |
| `UiTextKeys.cs` modified | no |
| `ResourceUiTextProviderTests.cs` modified | no |
| docs/340 created | no |
| `Ui.Product.*` keys added | no |
| build run | no |
| test run | no |

## K. Implementation Readiness Judgment

`Ui.Product.*` copy implementation may be considered after this approved table is committed.

ProductShell implementation remains blocked until separate exact implementation approval.

This document does not approve source/resource/test modification.
