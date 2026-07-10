# Product UI Shell Phase 1 Validation Test Gate Decision Plan

## A. Status

PRODUCT_UI_SHELL_PHASE1_VALIDATION_TEST_GATE_DECISION_PLAN_ONLY

## B. Marker

POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1_VALIDATION_TEST_GATE_DECISION_READY

## C. Baseline

- baseline commit: `574af1a docs(familyclaimref): plan product shell phase1 implementation preflight`
- current work type: documentation-only decision candidate planning

## D. Future Validation Gate

Future Phase 1 implementation must pass:

```powershell
dotnet build FamilyClaimRef.sln
dotnet test FamilyClaimRef.sln --filter FullyQualifiedName~ResourceUiTextProviderTests
dotnet test FamilyClaimRef.sln --filter FullyQualifiedName~DocumentRegistrationViewModel
dotnet test FamilyClaimRef.sln
```

No validation command is run in this documentation-only batch.

## E. Future Targeted Test Candidate Table

| Test target | Candidate file | Required condition | Approved now |
|---|---|---|---|
| ProductShellViewModel navigation state | `tests/FamilyClaimRef.App.Tests/ProductShellViewModelTests.cs` | `ProductShellViewModel` creation approved | no |
| ProductNavigationItemViewModel behavior | `tests/FamilyClaimRef.App.Tests/ProductNavigationItemViewModelTests.cs` | `ProductNavigationItemViewModel` creation approved | no |
| ResourceUiTextProviderTests for `Ui.Product.*` keys | `tests/FamilyClaimRef.App.Tests/Services/Localization/ResourceUiTextProviderTests.cs` | `Ui.Product.*` value table approved | no |
| DocumentRegistrationViewModel reuse regression | `tests/FamilyClaimRef.App.Tests/DocumentRegistrationViewModelTests.cs` | product registration view reuses or wraps existing ViewModel | no |
| Document list data-source behavior | future document list test candidate | document list ViewModel/data-source boundary approved | no |
| full regression | existing full test suite | any ProductShell implementation is approved | no |

## F. Implementation Blocker Checklist

Future implementation must confirm:

- exact file list approved
- entry strategy approved
- resource/copy table approved
- product terminology approved if shown
- `MainWindow` non-replacement confirmed
- App startup unchanged confirmed
- no `data/claimdoc` confirmed
- no DB/SQLite/repository/OCR/migration dependency confirmed
- tests approved

## G. Validation / Test Judgment

No validation command is run in this documentation-only batch.

Future implementation must include build/test.

App launch/manual workflow remains not approved.

If Windows SDK user-profile boundary occurs, distinguish environment boundary from actual build/test failure and record elevated rerun separately.
