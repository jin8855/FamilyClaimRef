# Product UI Shell Phase 1 Exact File List Decision Candidate

## A. Status

PRODUCT_UI_SHELL_PHASE1_EXACT_FILE_LIST_DECISION_CANDIDATE_ONLY

## B. Marker

POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1_EXACT_FILE_LIST_DECISION_CANDIDATE_READY

## C. Baseline

- baseline commit: `574af1a docs(familyclaimref): plan product shell phase1 implementation preflight`
- current work type: documentation-only decision candidate planning

## D. Exact File List Candidate Table

| File path | Candidate action | Phase 1 role | Required prerequisite | Implementation approved now | Notes |
|---|---|---|---|---|---|
| `app/FamilyClaimRef.App/ProductShell/ProductShellWindow.xaml` | create candidate | compile-only product shell window | explicit ProductShellWindow creation approval | no | future exact implementation batch 전까지 생성 금지 |
| `app/FamilyClaimRef.App/ProductShell/ProductShellWindow.xaml.cs` | create candidate | minimal window code-behind | explicit ProductShellWindow creation approval | no | runtime entry를 열지 않는 조건 필요 |
| `app/FamilyClaimRef.App/ViewModels/ProductShellViewModel.cs` | create candidate | product shell navigation state owner | ViewModel creation approval | no | `MainWindowViewModel` 재사용 금지 후보 |
| `app/FamilyClaimRef.App/ViewModels/ProductNavigationItemViewModel.cs` | create candidate | navigation item state | navigation model approval | no | navigation behavior test 후보 |
| `app/FamilyClaimRef.App/Views/ProductHomeView.xaml` | create candidate | Home/dashboard product view | XAML view approval and copy boundary approval | no | product-facing copy hard-code 금지 |
| `app/FamilyClaimRef.App/Views/ProductHomeView.xaml.cs` | create candidate | Home/dashboard view code-behind | XAML view approval | no | view-only 유지 후보 |
| `app/FamilyClaimRef.App/Views/ProductDocumentRegistrationView.xaml` | create candidate | document registration product view | XAML view approval and ViewModel reuse decision | no | workflow bypass 금지 |
| `app/FamilyClaimRef.App/Views/ProductDocumentRegistrationView.xaml.cs` | create candidate | document registration view code-behind | XAML view approval | no | existing `DocumentRegistrationViewModel` reuse/wrap decision 필요 |
| `app/FamilyClaimRef.App/Views/ProductDocumentListView.xaml` | create candidate | document list product view | document list data-source boundary approval | no | DB/repository 의존 금지 |
| `app/FamilyClaimRef.App/Views/ProductDocumentListView.xaml.cs` | create candidate | document list view code-behind | XAML view approval | no | view-only 유지 후보 |
| `app/FamilyClaimRef.App/Composition/AppServices.cs` | modify candidate | future composition hook | composition strategy approval | no | future composition hook candidate only |
| `app/FamilyClaimRef.App/Resources/UiStrings.xaml` | update candidate | future `Ui.Product.*` values | approved `Ui.Product.*` value table | no | `Ui.Product.*` 승인 전 수정 금지 |
| `app/FamilyClaimRef.App/Services/Localization/UiTextKeys.cs` | update candidate | future `Ui.Product.*` key constants | approved `Ui.Product.*` key table | no | key addition 승인 전 수정 금지 |
| `tests/FamilyClaimRef.App.Tests/ProductShellViewModelTests.cs` | test candidate | shell navigation state tests | ProductShellViewModel implementation approval | no | implementation batch에서 다시 승인 필요 |
| `tests/FamilyClaimRef.App.Tests/ProductNavigationItemViewModelTests.cs` | test candidate | navigation item behavior tests | navigation item model approval | no | implementation batch에서 다시 승인 필요 |
| `tests/FamilyClaimRef.App.Tests/Services/Localization/ResourceUiTextProviderTests.cs` | update candidate | `Ui.Product.*` resource resolution tests | approved `Ui.Product.*` value table | no | `Ui.Product.*` 승인 전 수정 금지 |

## E. Required Judgment

- all `Implementation approved now` values are no
- `AppServices.cs` is a future composition hook candidate only
- `UiStrings.xaml` and `UiTextKeys.cs` must not change until `Ui.Product.*` is approved
- `ResourceUiTextProviderTests` must not change until `Ui.Product.*` is approved
- ProductShell code files must not be created before a future exact implementation batch
- `ProductDocumentListView` data source must not require DB/repository

## F. Exact File List Judgment

Candidate list is complete enough for future review.

Exact implementation file list is not approved by this document.

Future implementation batch must restate exact file list and approval.
