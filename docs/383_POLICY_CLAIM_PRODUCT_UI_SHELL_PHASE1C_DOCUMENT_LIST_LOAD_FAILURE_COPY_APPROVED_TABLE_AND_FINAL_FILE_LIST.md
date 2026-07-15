# Product UI Shell Phase 1C Document List Load-Failure Copy Approved Table And Final File List

## A. Status Marker

`POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1C_DOCUMENT_LIST_LOAD_FAILURE_COPY_APPROVED_TABLE_AND_FINAL_FILE_LIST_READY`

## B. Approved Load-Failure Copy

| Contract item | Approved future value | Implemented now |
|---|---|---|
| Resource key | `Ui.Product.DocumentList.LoadFailedMessage` | no |
| `UiTextKeys` identifier | `ProductDocumentListLoadFailedMessage` | no |
| Korean value | `문서 목록을 불러오지 못했습니다.` | no |

Copy approval state: approved for the future exact candidate only. No resource or constant is added in this documentation batch.

## C. Value Rationale

- It is distinct from the successful empty state, `등록된 문서가 없습니다.`.
- It does not expose an exception type, exception message, path, identifier, or storage detail.
- It does not imply a retry action because the basic list has no retry control.
- The future implementation must resolve it through `IUiTextProvider`; direct Korean XAML or C# literals remain prohibited.

## D. Resource Count Contract

| Count | Current | Future candidate |
|---|---:|---:|
| All `Ui.*` resources | 67 | 68 |
| All `UiTextKeys` constants | 67 | 68 |
| `Ui.Product.*` resources | 11 | 12 |
| `Ui.Product.*` constants | 11 | 12 |
| New keys | 0 | 1 |
| Deleted keys | 0 | 0 |
| Renamed keys | 0 | 0 |
| Existing value changes | 0 | 0 |
| Resource/constant mismatch | 0 | 0 |

## E. Final Future Exact Implementation Candidate

| # | File | Classification | Approved now |
|---:|---|---|---|
| 1 | `app/FamilyClaimRef.App/Views/ProductDocumentListView.xaml` | production create | no |
| 2 | `app/FamilyClaimRef.App/Views/ProductDocumentListView.xaml.cs` | production create | no |
| 3 | `app/FamilyClaimRef.App/ViewModels/ProductDocumentListViewModel.cs` | production create | no |
| 4 | `app/FamilyClaimRef.App/ViewModels/ProductDocumentListItemViewModel.cs` | production create | no |
| 5 | `app/FamilyClaimRef.App/ViewModels/ProductShellViewModel.cs` | production modify | no |
| 6 | `app/FamilyClaimRef.App/ProductShell/ProductShellWindow.xaml` | production modify | no |
| 7 | `app/FamilyClaimRef.App/Resources/UiStrings.xaml` | resource modify | no |
| 8 | `app/FamilyClaimRef.App/Services/Localization/UiTextKeys.cs` | constant modify | no |
| 9 | `tests/FamilyClaimRef.App.Tests/ProductDocumentListViewModelTests.cs` | test create | no |
| 10 | `tests/FamilyClaimRef.App.Tests/ProductShellViewModelTests.cs` | test modify | no |
| 11 | `tests/FamilyClaimRef.App.Tests/Services/Localization/ResourceUiTextProviderTests.cs` | test modify | no |
| 12 | `docs/380_POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1C_DOCUMENT_LIST_IMPLEMENTATION_RESULT_REVIEW.md` | result document create | no |

Candidate counts:

| Category | Count |
|---|---:|
| Production create | 4 |
| Production modify, including resource/constant files | 4 |
| Test create | 1 |
| Test modify | 2 |
| Storage modify | 0 |
| Result document create | 1 |
| Total | 12 |

## F. Excluded Files And Boundaries

Not included:

- `IDocumentStorageService.cs`
- `JsonDocumentStorageService.cs`
- `JsonDocumentStorageServiceTests.cs`
- `AppServices.cs`
- `App.xaml` / `App.xaml.cs`
- `MainWindow*`
- `ProductShellWindow.xaml.cs`
- `DocumentRecord` or other storage models
- Policy/claim link storage
- Project, solution, or package files
- Command, router, converter, repository, or service files

Runtime composition remains a separate decision. The exact 12-file candidate is approved as a contract only after the source audit, but every file remains `Approved now = no` for implementation. Implementation target now: `0`.
