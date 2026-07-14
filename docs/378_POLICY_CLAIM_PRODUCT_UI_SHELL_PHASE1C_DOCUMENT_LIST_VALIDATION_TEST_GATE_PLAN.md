# Policy Claim Product UI Shell Phase 1C Document List Validation And Test Gate Plan

## A. Status Marker

`POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1C_DOCUMENT_LIST_VALIDATION_TEST_GATE_PLAN_READY`

## B. Execution Boundary

This document defines commands and gates for a separately approved implementation batch. No build or test command is run in this documentation-only batch.

## C. Future Command Candidate

Run in this order only after the future exact implementation scope is approved:

```powershell
dotnet build FamilyClaimRef.sln
dotnet test FamilyClaimRef.sln --filter FullyQualifiedName~ProductDocumentListViewModelTests
dotnet test FamilyClaimRef.sln --filter FullyQualifiedName~ProductShellViewModelTests
dotnet test FamilyClaimRef.sln --filter FullyQualifiedName~JsonDocumentStorageServiceTests
dotnet test FamilyClaimRef.sln --filter FullyQualifiedName~ResourceUiTextProviderTests
dotnet test FamilyClaimRef.sln
```

The `JsonDocumentStorageServiceTests` run is a regression gate only. Storage source and tests are not implementation candidates.

## D. ProductDocumentListViewModel Test Gate

- Null storage and text-provider dependencies are rejected.
- Title and empty-state copy resolve through `IUiTextProvider`.
- Missing JSON/empty source produces an empty successful state.
- Non-empty load projects `DisplayTitle` only.
- Disabled documents are excluded through `DisabledAt is null`.
- Source-return order is preserved.
- Repeated load replaces items and does not duplicate rows.
- Load failure is distinct from a successful empty state.
- Approved load-failure resource is used when storage throws.
- Item projection exposes no raw path, physical file name, extension, or visible technical ID.
- Document type and reference date are not invented or displayed.

## E. ProductShell Test Gate

- Existing navigation count, order, IDs, and selection behavior remain unchanged.
- Existing `DocumentRegistration` injection/property behavior remains unchanged.
- Future list ViewModel constructor dependency is exact and null-guarded.
- Future `DocumentList` property exposes the injected instance.
- Home and registration mappings remain intact.
- DocumentList no longer uses fallback text after implementation.

## F. XAML And Lifecycle Gate

- XAML compiles under the existing SDK-style WPF project without project-file edits.
- `ProductDocumentListView` DataContext comes from `ProductShellViewModel.DocumentList`.
- View code-behind only forwards Loaded to `LoadAsync()`.
- Empty, loading, error, and non-empty states do not overlap.
- No local path, raw ID, or raw document-type code is visible.
- There are no detail/open/edit/delete/unlink/search/filter/sort controls.
- Every activation reloads and replaces items.

## G. Resource Gate

If the load-failure key is separately approved and added:

- Resources/constants must move from `67/67` to `68/68`.
- `Ui.Product.*` resources/constants must move from `11/11` to `12/12`.
- Resource/constant mismatch, duplicate, missing, and orphan counts must remain `0`.
- Existing resource values must remain unchanged except for the one approved addition.
- Direct exact-value coverage must include the new key.
- No direct Korean production literal is added outside the resource dictionary.

## H. Storage And Privacy Regression Gate

- `IDocumentStorageService` and `JsonDocumentStorageService` remain unchanged.
- Missing-file, persistence, disabled-state, and invalid JSON/schema tests continue to pass.
- JSON remains the source of truth.
- Policy/claim link storage is not queried by the basic list.
- Protected local document content is not read, listed, searched, or used.
- No runtime artifacts or sample documents are created.

## I. Full Regression Gate

- Latest known baseline: PASS `358/358`.
- Future full-test count must be recorded, not assumed.
- Existing tests deleted: `0`.
- Existing assertions weakened: `0`.
- Runtime entry remains absent.
- MainWindow, App startup, and AppServices remain unchanged unless a later explicit approval changes the boundary.
- App launch, OpenFileDialog, manual workflow, and screenshot automation are not part of the implementation validation batch unless separately approved.
