# Product UI Shell Phase 1 Exact File Candidate and Entry Strategy

## A. Status

PRODUCT_UI_SHELL_PHASE1_EXACT_FILE_CANDIDATE_AND_ENTRY_STRATEGY_ONLY

## B. Marker

POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1_EXACT_FILE_CANDIDATE_AND_ENTRY_STRATEGY_READY

## C. Baseline

- baseline commit: `6cee3a9 docs(familyclaimref): plan product shell phase1 scope`
- current work type: documentation-only implementation preflight planning

## D. Future Implementation Candidate Files

All rows are future candidates only. No file is created or modified by this document.

| Candidate file or path | Candidate purpose | Required approval before creation/modification | Risk | Notes |
|---|---|---|---|---|
| `app/FamilyClaimRef.App/ProductShell/ProductShellWindow.xaml` | Product shell window layout candidate | explicit ProductShellWindow creation approval | high | must not replace `MainWindow` without separate approval |
| `app/FamilyClaimRef.App/ProductShell/ProductShellWindow.xaml.cs` | Product shell window code-behind candidate | explicit ProductShellWindow creation approval | medium | should remain minimal if later approved |
| `app/FamilyClaimRef.App/ViewModels/ProductShellViewModel.cs` | Product shell state and navigation owner candidate | explicit ViewModel creation approval | high | should not reuse `MainWindowViewModel` directly |
| `app/FamilyClaimRef.App/ViewModels/ProductNavigationItemViewModel.cs` | Product navigation item candidate | explicit ViewModel creation approval | medium | navigation state model only |
| `app/FamilyClaimRef.App/Views/ProductHomeView.xaml` | Home/dashboard product view candidate | explicit XAML view approval | high | copy/resource boundary must be approved first |
| `app/FamilyClaimRef.App/Views/ProductHomeView.xaml.cs` | Home/dashboard code-behind candidate | explicit XAML view approval | medium | should remain view-only if later approved |
| `app/FamilyClaimRef.App/Views/ProductDocumentRegistrationView.xaml` | Document registration product view candidate | explicit XAML view approval | high | may reuse or wrap existing document registration ViewModel only if approved |
| `app/FamilyClaimRef.App/Views/ProductDocumentRegistrationView.xaml.cs` | Document registration view code-behind candidate | explicit XAML view approval | medium | should avoid workflow bypass |
| `app/FamilyClaimRef.App/Views/ProductDocumentListView.xaml` | Document list product view candidate | explicit XAML view approval | high | data source must stay JSON/storage-service based unless later approved |
| `app/FamilyClaimRef.App/Views/ProductDocumentListView.xaml.cs` | Document list view code-behind candidate | explicit XAML view approval | medium | should remain view-only if later approved |
| `app/FamilyClaimRef.App/Composition/AppServices.cs` | future composition hook candidate | explicit composition change approval | high | current `MainWindow` composition must remain stable |
| `app/FamilyClaimRef.App/Resources/UiStrings.xaml` | future product copy resource candidate | explicit `Ui.Product.*` copy approval | high | no product keys are approved now |
| `app/FamilyClaimRef.App/Services/Localization/UiTextKeys.cs` | future product key constants candidate | explicit `Ui.Product.*` key approval | high | no key addition is approved now |
| `tests/FamilyClaimRef.App.Tests/ProductShellViewModelTests.cs` | product shell ViewModel test candidate | explicit test implementation approval | medium | should cover navigation state if later approved |
| `tests/FamilyClaimRef.App.Tests/ProductNavigationItemViewModelTests.cs` | navigation item test candidate | explicit test implementation approval | low | only needed if navigation item model is created |
| `tests/FamilyClaimRef.App.Tests/Services/Localization/ResourceUiTextProviderTests.cs` | resource provider regression candidate | explicit resource/copy test update approval | medium | only needed if `Ui.Product.*` keys are approved |

## E. Entry / Startup Strategy Options

| Entry strategy | Description | MainWindow impact | App.xaml/App startup impact | Recommended now | Reason |
|---|---|---|---|---|---|
| Compile-only `ProductShellWindow`, no runtime entry yet | Create shell in a future code batch but do not expose runtime startup | none | none | yes, if implementation is later approved | build/test validation can happen before runtime exposure |
| `ProductShellWindow` opened from validation harness command/button | Add a harness entry to open product shell | modifies validation harness | likely none | no | validation harness UI change is not approved |
| `ProductShellWindow` as app startup replacement | Start app directly in product shell | replaces validation harness entry | App startup change required | no | `MainWindow` replacement is not approved |
| Command-line or config-driven product shell startup | Select shell by runtime option | no direct replacement if guarded | App startup logic change required | no | startup option policy is not approved |
| Separate future product executable/project | Keep validation harness and product app separated | none | project/solution change required | no | project structure change is not approved |

Recommended now: compile-only `ProductShellWindow`, no runtime entry yet, if implementation is later approved.

Reasons:

- `MainWindow` replacement is not approved.
- validation harness is stable and should remain separated.
- app launch/manual workflow is not approved in this batch.
- product shell can be build/test validated before runtime entry is exposed.

## F. Composition Strategy Options

| Composition strategy | Description | Candidate judgment | Risk |
|---|---|---|---|
| `ProductShellWindow` directly composes view model | Window creates dependencies itself | not recommended | high |
| `AppServices` creates `ProductShellWindow` | composition root owns ProductShell construction | candidate only | medium |
| Separate `ProductShellComposition` service | isolated product shell composition boundary | candidate only | medium |
| Reuse `MainWindowViewModel` | make current harness ViewModel drive product shell | not recommended | high |
| Wrap existing `DocumentRegistrationViewModel` inside `ProductShellViewModel` | product shell owns navigation while reusing document registration boundary | candidate only | medium |

Recommendations:

- Do not reuse `MainWindowViewModel` as `ProductShellViewModel`.
- Prefer a separate `ProductShellViewModel` candidate if implementation is later approved.
- `DocumentRegistrationViewModel` reuse or wrapping remains a candidate, not an approval.
- ProductShell composition should not hard-code product shell behavior into validation `MainWindow`.

## G. File Candidate Judgment

Phase 1 implementation is not ready until the resource/copy strategy and entry strategy are explicitly approved.

No exact implementation file list is approved by this document.

