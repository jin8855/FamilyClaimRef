# Product UI Shell Phase 1B2 Document Registration Composition Lifecycle Approved Decision

## A. Status

- Marker: `POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1B2_DOCUMENT_REGISTRATION_COMPOSITION_LIFECYCLE_APPROVED_DECISION_READY`
- Decision scope: future compile-only Candidate A implementation contract
- Implemented now: no

## B. Selected Composition Contract

Selected architecture: **Candidate A, direct reuse through `ProductShellViewModel`**.

The future constructor candidate is:

```text
ProductShellViewModel(
    IUiTextProvider uiTextProvider,
    DocumentRegistrationViewModel documentRegistration)
```

The future read-only property identifier is:

```text
DocumentRegistration
```

`DocumentRegistration` is selected because the existing aggregate ViewModel uses the same concise ownership convention for its registration child. The property exposes the existing instance; it does not create or wrap it.

## C. Ownership Boundary

- `ProductShellViewModel` receives and exposes one existing `DocumentRegistrationViewModel` reference.
- `ProductShellViewModel` does not receive or create workflow, picker, or storage services.
- `ProductDocumentRegistrationViewModel` wrapper: none.
- `MainWindowViewModel` reuse: none.
- `AppServices` modification: none for compile-only scope.
- `ProductShellWindow.xaml.cs` modification: none.
- ProductShell runtime composition: deferred.
- Runtime entry: absent and not approved.

Composition blocker judgment for compile-only scope: **resolved**.

Runtime composition remains deferred until a separate runtime-entry decision.

## D. Interaction Forwarding Contract

| Product view event | Existing ViewModel call | Allowed code-behind responsibility |
|---|---|---|
| `Loaded` | `LoadTargetOptionsAsync()` | forward only |
| select-file click | `SelectFileAsync()` | forward only |
| register click | `RegisterAsync()` | forward only |

The product view code-behind must not create services, read storage, duplicate validation, normalize business values, call the workflow directly, or translate result state.

## E. Activation Lifecycle Decision

- Call `LoadTargetOptionsAsync()` on every `ProductDocumentRegistrationView.Loaded` event.
- Re-entering the page and loading the view again reads the current policy/claim storage snapshot again.
- Do not add a one-load-per-view-instance cache.
- Do not add a permanent initialized flag.
- Sequential calls must replace option collections rather than append duplicate options.
- Invalid prior policy or claim selections are cleared by existing ViewModel behavior.
- The view must not access storage directly.
- Do not introduce an async page-lifecycle interface, navigation router, command framework, or service locator.

Lifecycle blocker judgment for compile-only scope: **resolved**.

## F. Required Sequential Regression Test

Future modification target:

- `tests/FamilyClaimRef.App.Tests/DocumentRegistrationViewModelTests.cs`

Required assertions:

1. Load an initial active policy/claim snapshot.
2. Select targets from that snapshot.
3. Change the test-owned storage snapshot.
4. Call `LoadTargetOptionsAsync()` a second time.
5. Confirm the second snapshot is reflected.
6. Confirm disabled or removed targets are absent.
7. Confirm invalid prior selections are cleared.
8. Confirm no duplicate options are present.
9. Confirm the workflow and file picker were not called.

No production `DocumentRegistrationViewModel` change is required for this test contract.

## G. Async Error Boundary

- Preserve the current `LoadTargetOptionsAsync`, `SelectFileAsync`, and `RegisterAsync` behavior.
- Do not add a silent catch in the product view.
- Do not swallow storage or picker exceptions.
- `RegisterAsync` continues to own its existing status-message conversion.
- Any broader page-level async error policy requires a separate decision.

## H. Final Decision

- Candidate A composition future contract: approved.
- every-activation lifecycle future contract: approved.
- lifecycle regression test: required.
- source implementation in this batch: no.
- runtime composition: deferred.
- runtime entry: not approved.
