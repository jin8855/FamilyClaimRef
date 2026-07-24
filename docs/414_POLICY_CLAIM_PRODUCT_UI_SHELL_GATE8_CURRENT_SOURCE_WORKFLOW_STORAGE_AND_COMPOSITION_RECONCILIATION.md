# Policy Claim Product UI Shell Gate8 Current Source Workflow Storage and Composition Reconciliation

## A. Status

- Status: `CURRENT_SOURCE_RECONCILIATION`
- Marker: `POLICY_CLAIM_PRODUCT_UI_SHELL_GATE8_CURRENT_SOURCE_WORKFLOW_STORAGE_AND_COMPOSITION_RECONCILIATION_READY`
- Baseline: `79d8f1d5b76b22b0ef1a65fbf781c833bbcd7fff`
- Inspection mode: read-only static inspection
- Current source inventory file count: `58`

## B. Reconciled Prior Decisions

| Evidence | Reconciled conclusion |
|---|---|
| Document 353 | ProductShell registration planning originated as a bounded UI entry |
| Document 354 | Candidate A, reuse of existing registration boundary, remains the least-duplication direction |
| Document 355 | Earlier copy/resource candidates are superseded where current source now provides implemented values |
| Document 356 | Validation must retain static, ViewModel, workflow, storage, resource, and runtime separation |
| Documents 368, 370, and 374 | Policy/claim target terminology must stay product-facing and must not expose raw internal identifiers |
| Documents 399 through 408 | ProductShell management and registration composition are present; lower storage services are shared |
| Documents 409 through 411 | ProductShell runtime/accessibility/visual evidence exists for prior gates, not for Gate8 real registration |
| Document 412 | Gate7 default startup transition is closed; Gate8 begins from a clean default ProductShell baseline |

## C. Current Actual Call Chain

```text
ProductShellWindow
  -> ProductShellViewModel.DocumentRegistration
  -> ProductDocumentRegistrationView.DataContext
  -> ProductDocumentRegistrationView.xaml.cs
     -> Loaded: DocumentRegistrationViewModel.LoadTargetOptionsAsync()
     -> Select file: DocumentRegistrationViewModel.SelectFileAsync()
        -> IFilePickerService
        -> WpfFilePickerService
     -> Register: DocumentRegistrationViewModel.RegisterAsync()
        -> DocumentRegistrationWorkflow
           -> DocumentAttachmentCoordinator
              -> FileNamePolicyService
              -> IFileAttachmentService
              -> LocalFileAttachmentService
              -> IDocumentStorageService.SaveDocumentAsync()
           -> DocumentLinkCoordinator
              -> IPolicyClaimStorageService
              -> IDocumentStorageService
```

The view and code-behind do not directly call file storage or JSON storage.

## D. Current Composition and Lifetime

| Component | Current lifetime/owner | Gate8 implication |
|---|---|---|
| `AppServices` | One composition graph per application startup | Central factory remains the owner of concrete services |
| `ProductShellWindow` | Window lifetime | Owns one `ProductShellViewModel` reference |
| `ProductShellViewModel` | ProductShell window lifetime | Owns one ProductShell registration ViewModel reference |
| ProductShell `DocumentRegistrationViewModel` | ProductShell window lifetime | Draft survives navigation within the same window |
| MainWindow `DocumentRegistrationViewModel` | Separate instance | Validation harness does not share UI state with ProductShell |
| Storage/workflow services | Shared within one `AppServices` graph | ProductShell and MainWindow use the same lower persistence boundary |
| `EnvironmentRuntimeRootProvider` | Composition-level | Both UI surfaces resolve the same guarded runtime roots |

## E. Current Source Inventory

| # | Exact path | Responsibility | Creation/lifetime | Input | Output | I/O | ProductShell connection | Gate8 impact/evidence |
|---:|---|---|---|---|---|---|---|---|
| 1 | `app/FamilyClaimRef.App/ViewModels/DocumentRegistrationViewModel.cs` | Registration UI state and orchestration | ProductShell/MainWindow instance | picker, target, metadata | validation/status/result | indirect | Direct DataContext | Main lifecycle owner |
| 2 | `app/FamilyClaimRef.App/Views/ProductDocumentRegistrationView.xaml` | Product registration form | View navigation lifetime | bound ViewModel | visual state | none | Direct view | Existing controls are sufficient for candidate messages |
| 3 | `app/FamilyClaimRef.App/Views/ProductDocumentRegistrationView.xaml.cs` | Loaded/select/register event forwarding | View lifetime | UI events | async VM calls | none | Direct | No direct storage call |
| 4 | `app/FamilyClaimRef.App/ViewModels/ProductShellViewModel.cs` | ProductShell destination and child VM ownership | Window lifetime | child VMs | selected content | none | Root VM | Registration VM persists across navigation |
| 5 | `app/FamilyClaimRef.App/ProductShell/ProductShellWindow.xaml` | Shell and DataTemplates | Window lifetime | ProductShell VM | destination content | none | Root view | Registration view is template-selected |
| 6 | `app/FamilyClaimRef.App/ProductShell/ProductShellWindow.xaml.cs` | Shell window initialization | Window lifetime | ProductShell VM | DataContext | none | Root view | No Gate8 storage responsibility |
| 7 | `app/FamilyClaimRef.App/Composition/AppServices.cs` | Concrete composition | Application lifetime | runtime provider | VMs/services | root creation | Direct factory | Central Gate8 wiring owner |
| 8 | `app/FamilyClaimRef.App/App.xaml.cs` | Default/preview startup selection | Application lifetime | startup args/env | selected window | window launch | Default entry | Protected; Gate7 already closed |
| 9 | `app/FamilyClaimRef.App/ViewModels/MainWindowViewModel.cs` | Validation harness root VM | MainWindow lifetime | child VMs | harness content | none | Separate | Product state must not be shared |
| 10 | `app/FamilyClaimRef.App/MainWindow.xaml` | Validation harness UI | MainWindow lifetime | bound VM | diagnostic UI | none | Not ProductShell | Binds raw registration summary; protected from Gate8 |
| 11 | `app/FamilyClaimRef.App/MainWindow.xaml.cs` | Harness window initialization | Window lifetime | MainWindow VM | DataContext | none | Not ProductShell | No Gate8 change |
| 12 | `app/FamilyClaimRef.App/Services/Storage/DocumentRegistrationWorkflow.cs` | Attachment plus target link use case | Composition-level | policy/claim request | registration result | indirect | Through VM | Reuse candidate |
| 13 | `app/FamilyClaimRef.App/Services/Storage/DocumentAttachmentCoordinator.cs` | Validate, name, copy, save Document | Composition-level | attachment request | Document record | file/JSON indirect | Through workflow | Needs lower policy extension |
| 14 | `app/FamilyClaimRef.App/Services/Storage/DocumentLinkCoordinator.cs` | Validate target and create link | Composition-level | target/document IDs | link record | JSON indirect | Through workflow | Current duplicate check is document-ID only |
| 15 | `app/FamilyClaimRef.App/Services/Storage/DocumentAttachmentRequest.cs` | Attachment request contract | Per call | source/type/title/date | coordinator input | none | Through workflow | Missing selected snapshot/hash fields |
| 16 | `app/FamilyClaimRef.App/Services/Storage/DocumentAttachmentResult.cs` | Attachment result contract | Per call | stored records | workflow result | none | Through workflow | Existing result can be extended only if needed |
| 17 | `app/FamilyClaimRef.App/Services/Storage/PolicyDocumentRegistrationRequest.cs` | Policy registration contract | Per call | UI values | workflow input | none | Through VM | Target kind is implicit |
| 18 | `app/FamilyClaimRef.App/Services/Storage/ClaimDocumentRegistrationRequest.cs` | Claim registration contract | Per call | UI values | workflow input | none | Through VM | Target kind is implicit |
| 19 | `app/FamilyClaimRef.App/Services/Storage/IFileAttachmentService.cs` | File storage abstraction | Composition-level | copy/delete/exists | copy result | file | Through coordinator | Needs staged-finalization contract candidate |
| 20 | `app/FamilyClaimRef.App/Services/Storage/LocalFileAttachmentService.cs` | Managed attachment file copy/delete | Composition-level | source, physical name | relative path and size | file | Through coordinator | Direct final copy; no staging/hash |
| 21 | `app/FamilyClaimRef.App/Services/Storage/FileAttachmentCopyResult.cs` | Copy result | Per call | file operation | path/name/size | none | Through coordinator | Missing hash and validated type |
| 22 | `app/FamilyClaimRef.App/Services/Storage/IDocumentStorageService.cs` | Document/link persistence abstraction | Composition-level | drafts/IDs | records | JSON | Through coordinators | Needs target/hash query candidate |
| 23 | `app/FamilyClaimRef.App/Services/Storage/JsonDocumentStorageService.cs` | Documents and links JSON persistence | Composition-level | drafts/links | stored records | JSON | Shared lower service | Separate JSON files; no cross-file transaction |
| 24 | `app/FamilyClaimRef.App/Services/Storage/JsonFileStore.cs` | JSON file read/write helper | Per storage service | item list | atomic file replacement | file/JSON | Indirect | Temp-plus-move per JSON file only |
| 25 | `app/FamilyClaimRef.App/Models/Storage/DocumentRecord.cs` | Persisted document metadata | Durable record | document draft | document state | none | List/workflow | Missing size/hash/type/reference metadata |
| 26 | `app/FamilyClaimRef.App/Models/Storage/DocumentDraft.cs` | New document metadata input | Per save | coordinator values | storage input | none | Through coordinator | Schema extension candidate |
| 27 | `app/FamilyClaimRef.App/Models/Storage/PolicyDocumentRecord.cs` | Policy-document link | Durable record | policy/document IDs | active/disabled link | none | Through workflow | Target association |
| 28 | `app/FamilyClaimRef.App/Models/Storage/ClaimDocumentRecord.cs` | Claim-document link | Durable record | claim/document IDs | active/disabled link | none | Through workflow | Target association |
| 29 | `app/FamilyClaimRef.App/Services/UI/IFilePickerService.cs` | Picker abstraction | Composition-level | picker request | selected file/cancel | dialog indirect | Through VM | Existing boundary is reusable |
| 30 | `app/FamilyClaimRef.App/Services/UI/WpfFilePickerService.cs` | WPF file picker | Composition-level | dialog options | source full path/display name | dialog | Through VM | Filter conflicts with actual allowlist |
| 31 | `app/FamilyClaimRef.App/Services/UI/FilePickerResult.cs` | Selected file snapshot | Per selection | dialog result | path/display name | none | Through VM | Missing size/last-write snapshot |
| 32 | `app/FamilyClaimRef.App/Services/FileNamePolicyService.cs` | Type/extension/filename policy | Composition-level | type/ext/index | normalized name | none | Through coordinator | Authoritative extension allowlist |
| 33 | `app/FamilyClaimRef.App/Services/Runtime/EnvironmentRuntimeRootProvider.cs` | Guarded runtime root selection | Composition-level | environment/default | root paths | environment/path | Through AppServices | Existing injection seam |
| 34 | `app/FamilyClaimRef.App/Services/Runtime/RuntimeRootPaths.cs` | Metadata/attachment roots | Value lifetime | root | derived paths | none | Through AppServices | Keeps payload and metadata under one runtime root |
| 35 | `app/FamilyClaimRef.App/Services/Runtime/IRuntimeRootProvider.cs` | Runtime root abstraction | Composition-level | none | root paths | none | Through AppServices | TEMP test injection supported |
| 36 | `app/FamilyClaimRef.App/ViewModels/ProductDocumentListViewModel.cs` | Active document title list | ProductShell lifetime | storage records | list items/status | JSON indirect | Document list destination | Read-only; no payload open |
| 37 | `app/FamilyClaimRef.App/ViewModels/ProductDocumentListItemViewModel.cs` | Document list projection | Per record | Document record | display properties | none | Product list | Must not expose source path |
| 38 | `app/FamilyClaimRef.App/Views/ProductDocumentListView.xaml` | Product document list UI | View lifetime | list VM | titles/status | none | Direct destination | Gate8 does not expand list behavior |
| 39 | `app/FamilyClaimRef.App/Views/ProductDocumentListView.xaml.cs` | Product list load forwarding | View lifetime | Loaded event | load call | none | Direct destination | Reentry list refresh remains separate |
| 40 | `app/FamilyClaimRef.App/Services/Localization/UiTextKeys.cs` | Resource key constants | Static | key names | constants | none | All UI | Eight future product keys candidate |
| 41 | `app/FamilyClaimRef.App/Resources/UiStrings.xaml` | UI resource values | Application resources | key lookup | localized copy | resource | Product view | Current baseline `91/91` |
| 42 | `app/FamilyClaimRef.App/Models/Storage/DocumentTypeSeeds.cs` | Approved document type seeds | Static | none | type codes | none | Target metadata | Type policy source |
| 43 | `app/FamilyClaimRef.App/Models/Storage/JsonFileEnvelope.cs` | JSON schema version, saved time, items envelope | Per JSON file load/save | schema version/items | validated envelope | none | Shared lower storage | Existing schema/version contract |
| 44 | `tests/FamilyClaimRef.App.Tests/DocumentRegistrationViewModelTests.cs` | Registration VM guards/state | Test lifetime | fakes | assertions | temp where needed | Product/harness shared VM | Current cancel/load/register coverage |
| 45 | `tests/FamilyClaimRef.App.Tests/DocumentRegistrationWorkflowTests.cs` | Workflow rollback | Test lifetime | fake coordinators | assertions | temp/fake | Lower use case | Existing link-failure compensation |
| 46 | `tests/FamilyClaimRef.App.Tests/DocumentAttachmentCoordinatorTests.cs` | Naming/copy/metadata behavior | Test lifetime | temp files/fakes | assertions | TEMP | Lower use case | Existing collision and metadata failure coverage |
| 47 | `tests/FamilyClaimRef.App.Tests/IFileAttachmentServiceTests.cs` | Local file service behavior | Test lifetime | TEMP source/root | assertions | TEMP | Lower service | Existing path/delete/copy coverage |
| 48 | `tests/FamilyClaimRef.App.Tests/JsonDocumentStorageServiceTests.cs` | JSON schema/CRUD/link behavior | Test lifetime | TEMP root | assertions | TEMP JSON | Shared storage | Schema compatibility coverage base |
| 49 | `tests/FamilyClaimRef.App.Tests/Integration/DocumentRegistrationNegativeValidationTests.cs` | Negative workflow integration | Test lifetime | TEMP root/files | assertions | TEMP | End-to-end lower path | Unsupported/missing/disabled rollback coverage |
| 50 | `tests/FamilyClaimRef.App.Tests/ProductShellViewModelTests.cs` | Navigation/child ownership | Test lifetime | child VMs | assertions | none | ProductShell | Must preserve five destinations |
| 51 | `tests/FamilyClaimRef.App.Tests/ProductDocumentListViewModelTests.cs` | Document list projection | Test lifetime | fake storage | assertions | none | Product list | Gate8 list regression check |
| 52 | `tests/FamilyClaimRef.App.Tests/Composition/AppServicesTests.cs` | Composition/root isolation | Test lifetime | TEMP provider | assertions | TEMP | Product/harness graph | Verifies injected root behavior |
| 53 | `tests/FamilyClaimRef.App.Tests/Services/Localization/ResourceUiTextProviderTests.cs` | Resource/key parity | Test lifetime | resources/constants | assertions | resource | Product UI | Future `99/99`, `43/43` candidate |
| 54 | `app/FamilyClaimRef.App/Services/Storage/IPolicyClaimStorageService.cs` | Policy/claim target persistence abstraction | Composition-level | target IDs/drafts | policy/claim records | JSON indirect | VM and link coordinator | Reuse for pre-attachment active-target recheck |
| 55 | `app/FamilyClaimRef.App/Services/Storage/JsonPolicyClaimStorageService.cs` | Policy/claim JSON persistence | Composition-level | target queries/drafts | active/disabled records | JSON | Shared lower service | Protected existing target repository |
| 56 | `app/FamilyClaimRef.App/Models/Storage/PolicyRecord.cs` | Policy target record | Durable record | policy draft | active/disabled target | none | Registration options | Target snapshot source |
| 57 | `app/FamilyClaimRef.App/Models/Storage/ClaimRecord.cs` | Claim target record | Durable record | claim draft | active/disabled target | none | Registration options | Target snapshot source |
| 58 | `tests/FamilyClaimRef.App.Tests/JsonPolicyClaimStorageServiceTests.cs` | Policy/claim target persistence contract | Test lifetime | TEMP root | assertions | TEMP JSON | Shared target storage | Active target existence evidence |

### E1. Member and Test Evidence

| Evidence area | Actual type/member/test |
|---|---|
| View load forwarding | `ProductDocumentRegistrationView.UserControl_Loaded` |
| View selection forwarding | `ProductDocumentRegistrationView.SelectFileButton_Click` |
| View registration forwarding | `ProductDocumentRegistrationView.RegisterButton_Click` |
| Target reload boundary | `DocumentRegistrationViewModel.LoadTargetOptionsAsync` |
| File picker boundary | `DocumentRegistrationViewModel.SelectFileAsync` |
| Registration boundary | `DocumentRegistrationViewModel.RegisterAsync` |
| Policy use case | `DocumentRegistrationWorkflow.RegisterPolicyDocumentAsync` |
| Claim use case | `DocumentRegistrationWorkflow.RegisterClaimDocumentAsync` |
| Workflow compensation | `DocumentRegistrationWorkflow.RollbackAttachmentAsync` |
| Attachment use case | `DocumentAttachmentCoordinator.AttachDocumentAsync` |
| Attachment cleanup | `DocumentAttachmentCoordinator.CleanupCopiedFileAsync` |
| Policy target validation | `DocumentLinkCoordinator.EnsureActivePolicyExistsAsync` |
| Claim target validation | `DocumentLinkCoordinator.EnsureActiveClaimExistsAsync` |
| Current ID duplicate checks | `EnsureNoActivePolicyDuplicateAsync`, `EnsureNoActiveClaimDuplicateAsync` |
| Physical copy | `LocalFileAttachmentService.CopyDocumentFileAsync` |
| Document persistence | `JsonDocumentStorageService.AddDocumentAsync` |
| Link persistence | `AddPolicyDocumentAsync`, `AddClaimDocumentAsync` |
| Per-file JSON atomic replacement | `JsonFileStore.SaveAsync` |
| Runtime root | `EnvironmentRuntimeRootProvider.GetRuntimeRootPaths` |
| Composition | `AppServices.CreateDefault`, `AppServices.Create` |
| ProductShell child ownership | `ProductShellViewModel.DocumentRegistration` |
| Cancel evidence | `SelectFileAsync_cancel_keeps_previous_state_and_does_not_set_error` |
| Repeated load evidence | `LoadTargetOptionsAsync_repeated_load_replaces_snapshot_and_clears_invalid_selections` |
| Workflow rollback evidence | `RegisterPolicyDocumentAsync_link_failure_deletes_copied_file_and_disables_document` |
| Metadata cleanup evidence | `AttachDocumentAsync_cleans_up_copied_file_when_metadata_save_fails` |
| Relative path evidence | `CopyDocumentFileAsync_returns_relative_path_not_absolute` |
| Schema envelope evidence | `Saved_json_contains_schemaVersion_and_savedAt` |
| Composition separation evidence | `Create_composes_separate_main_window_and_product_shell_view_model_graphs` |
| Resource parity evidence | `UiTextKeys_match_resource_keys_without_duplicates_or_gaps` |

### E2. Existing Abstraction Assessment

| Concern | Current source fact | Gate8 candidate |
|---|---|---|
| Hash provider | No hash provider abstraction was found | Add lower file validation/hash service |
| ID provider | `Guid.NewGuid` is called inline in storage/file helpers | Keep internal generation; no Gate8 provider abstraction required |
| Time provider | `DateTimeOffset.UtcNow` is called inline | Keep current boundary unless deterministic time becomes a test blocker |
| Atomic JSON helper | `JsonFileStore.SaveAsync` writes a temp file and moves it over one JSON file | Reuse for per-file JSON only |
| Cross-file transaction | None | Use compensation; do not claim transactionality |
| Target option model | Active `PolicyRecord` and `ClaimRecord` collections are exposed by the registration ViewModel | Reuse; no parallel Product option model |
| Validation copy | `UiTextKeys`, `UiStrings.xaml`, `IUiTextProvider` | Reuse safe shared keys; add only approved Product keys |
| DocumentList connection | `ProductDocumentListViewModel` queries active Document records | Preserve as read-only; UI expansion deferred |

### E3. Exact Creation Owner and Concrete Evidence

This table supplements the lifetime and Gate8-impact columns in E so that every inventory file has an explicit creation owner and evidence surface.

| # | Exact path | Creation owner | Concrete evidence |
|---:|---|---|---|
| 1 | `app/FamilyClaimRef.App/ViewModels/DocumentRegistrationViewModel.cs` | `AppServices.Create` | `LoadTargetOptionsAsync`, `SelectFileAsync`, `RegisterAsync` |
| 2 | `app/FamilyClaimRef.App/Views/ProductDocumentRegistrationView.xaml` | ProductShell DataTemplate/WPF | `DataContext` bindings and busy triggers |
| 3 | `app/FamilyClaimRef.App/Views/ProductDocumentRegistrationView.xaml.cs` | WPF view construction | `UserControl_Loaded`, `SelectFileButton_Click`, `RegisterButton_Click` |
| 4 | `app/FamilyClaimRef.App/ViewModels/ProductShellViewModel.cs` | `AppServices.Create` | `DocumentRegistration`, `NavigationItems`, `SelectedNavigationItem` |
| 5 | `app/FamilyClaimRef.App/ProductShell/ProductShellWindow.xaml` | `App.CreateProductShellWindow` | Product DataTemplates and content host |
| 6 | `app/FamilyClaimRef.App/ProductShell/ProductShellWindow.xaml.cs` | `App.CreateProductShellWindow` | `ProductShellWindow(ProductShellViewModel)` |
| 7 | `app/FamilyClaimRef.App/Composition/AppServices.cs` | `App.OnStartup` | `CreateDefault`, `Create` |
| 8 | `app/FamilyClaimRef.App/App.xaml.cs` | WPF application | `OnStartup`, `CreateProductShellWindow` |
| 9 | `app/FamilyClaimRef.App/ViewModels/MainWindowViewModel.cs` | `AppServices.Create` | separate MainWindow child graph |
| 10 | `app/FamilyClaimRef.App/MainWindow.xaml` | Not constructed by current default startup | validation-harness bindings |
| 11 | `app/FamilyClaimRef.App/MainWindow.xaml.cs` | Not constructed by current default startup | MainWindow constructor/DataContext boundary |
| 12 | `app/FamilyClaimRef.App/Services/Storage/DocumentRegistrationWorkflow.cs` | `AppServices.Create` | policy/claim register and rollback members |
| 13 | `app/FamilyClaimRef.App/Services/Storage/DocumentAttachmentCoordinator.cs` | `AppServices.Create` | `AttachDocumentAsync`, `CleanupCopiedFileAsync` |
| 14 | `app/FamilyClaimRef.App/Services/Storage/DocumentLinkCoordinator.cs` | `AppServices.Create` | policy/claim link and active-target checks |
| 15 | `app/FamilyClaimRef.App/Services/Storage/DocumentAttachmentRequest.cs` | `DocumentRegistrationWorkflow` per call | attachment request record |
| 16 | `app/FamilyClaimRef.App/Services/Storage/DocumentAttachmentResult.cs` | `DocumentAttachmentCoordinator` per call | attachment result record |
| 17 | `app/FamilyClaimRef.App/Services/Storage/PolicyDocumentRegistrationRequest.cs` | `DocumentRegistrationViewModel` per call | policy registration request |
| 18 | `app/FamilyClaimRef.App/Services/Storage/ClaimDocumentRegistrationRequest.cs` | `DocumentRegistrationViewModel` per call | claim registration request |
| 19 | `app/FamilyClaimRef.App/Services/Storage/IFileAttachmentService.cs` | `AppServices.Create` supplies implementation | copy/delete/exists contract |
| 20 | `app/FamilyClaimRef.App/Services/Storage/LocalFileAttachmentService.cs` | `AppServices.Create` | `CopyDocumentFileAsync` |
| 21 | `app/FamilyClaimRef.App/Services/Storage/FileAttachmentCopyResult.cs` | `LocalFileAttachmentService` per copy | relative path/name/extension/size |
| 22 | `app/FamilyClaimRef.App/Services/Storage/IDocumentStorageService.cs` | `AppServices.Create` supplies implementation | document and link CRUD contract |
| 23 | `app/FamilyClaimRef.App/Services/Storage/JsonDocumentStorageService.cs` | `AppServices.Create` | document/link add/get/disable members |
| 24 | `app/FamilyClaimRef.App/Services/Storage/JsonFileStore.cs` | `JsonDocumentStorageService` | `LoadAsync`, `SaveAsync`, `ValidateEnvelope` |
| 25 | `app/FamilyClaimRef.App/Models/Storage/DocumentRecord.cs` | `JsonDocumentStorageService.AddDocumentAsync` | durable Document record |
| 26 | `app/FamilyClaimRef.App/Models/Storage/DocumentDraft.cs` | `DocumentAttachmentCoordinator` | new Document input |
| 27 | `app/FamilyClaimRef.App/Models/Storage/PolicyDocumentRecord.cs` | `JsonDocumentStorageService.AddPolicyDocumentAsync` | durable policy link |
| 28 | `app/FamilyClaimRef.App/Models/Storage/ClaimDocumentRecord.cs` | `JsonDocumentStorageService.AddClaimDocumentAsync` | durable claim link |
| 29 | `app/FamilyClaimRef.App/Services/UI/IFilePickerService.cs` | `AppServices.Create` supplies implementation | `PickDocumentFileAsync` |
| 30 | `app/FamilyClaimRef.App/Services/UI/WpfFilePickerService.cs` | `AppServices.Create` | `PickDocumentFileAsync` and filter |
| 31 | `app/FamilyClaimRef.App/Services/UI/FilePickerResult.cs` | `WpfFilePickerService` per selection | source path and display name |
| 32 | `app/FamilyClaimRef.App/Services/FileNamePolicyService.cs` | static policy | `GetAllowedDocumentTypes`, `CreatePhysicalFileName` |
| 33 | `app/FamilyClaimRef.App/Services/Runtime/EnvironmentRuntimeRootProvider.cs` | `AppServices.CreateDefault` | `GetRuntimeRootPaths` |
| 34 | `app/FamilyClaimRef.App/Services/Runtime/RuntimeRootPaths.cs` | runtime root provider | metadata and attachment root values |
| 35 | `app/FamilyClaimRef.App/Services/Runtime/IRuntimeRootProvider.cs` | `AppServices.Create` parameter | runtime root abstraction |
| 36 | `app/FamilyClaimRef.App/ViewModels/ProductDocumentListViewModel.cs` | `AppServices.Create` | active Document load |
| 37 | `app/FamilyClaimRef.App/ViewModels/ProductDocumentListItemViewModel.cs` | Product document list projection | display item fields |
| 38 | `app/FamilyClaimRef.App/Views/ProductDocumentListView.xaml` | ProductShell DataTemplate/WPF | list bindings |
| 39 | `app/FamilyClaimRef.App/Views/ProductDocumentListView.xaml.cs` | WPF view construction | Loaded forwarding |
| 40 | `app/FamilyClaimRef.App/Services/Localization/UiTextKeys.cs` | static constants | Product and shared registration keys |
| 41 | `app/FamilyClaimRef.App/Resources/UiStrings.xaml` | WPF application resources | current resource values |
| 42 | `app/FamilyClaimRef.App/Models/Storage/DocumentTypeSeeds.cs` | static seed owner | `Claim`, `Policy`, `All` |
| 43 | `app/FamilyClaimRef.App/Models/Storage/JsonFileEnvelope.cs` | `JsonFileStore` per load/save | `SchemaVersion`, `SavedAt`, `Items` |
| 44 | `tests/FamilyClaimRef.App.Tests/DocumentRegistrationViewModelTests.cs` | xUnit | cancel, reload, validation, register tests |
| 45 | `tests/FamilyClaimRef.App.Tests/DocumentRegistrationWorkflowTests.cs` | xUnit | link failure and rollback tests |
| 46 | `tests/FamilyClaimRef.App.Tests/DocumentAttachmentCoordinatorTests.cs` | xUnit | copy, collision, metadata cleanup tests |
| 47 | `tests/FamilyClaimRef.App.Tests/IFileAttachmentServiceTests.cs` | xUnit | relative path, copy, traversal, delete tests |
| 48 | `tests/FamilyClaimRef.App.Tests/JsonDocumentStorageServiceTests.cs` | xUnit | schema, CRUD, link tests |
| 49 | `tests/FamilyClaimRef.App.Tests/Integration/DocumentRegistrationNegativeValidationTests.cs` | xUnit | unsupported/missing/disabled target integration |
| 50 | `tests/FamilyClaimRef.App.Tests/ProductShellViewModelTests.cs` | xUnit | navigation and child ownership |
| 51 | `tests/FamilyClaimRef.App.Tests/ProductDocumentListViewModelTests.cs` | xUnit | active Document projection |
| 52 | `tests/FamilyClaimRef.App.Tests/Composition/AppServicesTests.cs` | xUnit | roots and graph separation |
| 53 | `tests/FamilyClaimRef.App.Tests/Services/Localization/ResourceUiTextProviderTests.cs` | xUnit | parity and approved copy |
| 54 | `app/FamilyClaimRef.App/Services/Storage/IPolicyClaimStorageService.cs` | `AppServices.Create` supplies implementation | policy/claim query and existence contract |
| 55 | `app/FamilyClaimRef.App/Services/Storage/JsonPolicyClaimStorageService.cs` | `AppServices.Create` | `PolicyExistsAsync`, `ClaimExistsAsync` |
| 56 | `app/FamilyClaimRef.App/Models/Storage/PolicyRecord.cs` | `JsonPolicyClaimStorageService` | policy target identity and `DisabledAt` |
| 57 | `app/FamilyClaimRef.App/Models/Storage/ClaimRecord.cs` | `JsonPolicyClaimStorageService` | claim target identity and `DisabledAt` |
| 58 | `tests/FamilyClaimRef.App.Tests/JsonPolicyClaimStorageServiceTests.cs` | xUnit | active target CRUD/existence behavior |

## F. Current Workflow Behavior

### F1. Selection

- `SelectFileAsync` delegates to `IFilePickerService`.
- Picker cancel returns without replacing the previously selected file.
- The selected absolute source path exists only in runtime ViewModel state.
- The Product view displays the safe selected file name, not the full source path.

### F2. Target loading

- `LoadTargetOptionsAsync` reloads active policies and claims.
- Invalid policy or claim selections are cleared.
- Current registration target IDs are synchronized to the selected target.
- Reentry through view `Loaded` triggers another target load.

### F3. Registration

- The ViewModel validates target, file, document type, title, and reference date.
- `IsBusy` prevents duplicate select/register commands in the Product view.
- The workflow attaches the file first and creates the policy/claim link second.
- Target active-state validation currently happens in `DocumentLinkCoordinator`, after attachment creation.

### F4. Rollback

- A document metadata save failure causes copied-file deletion.
- A target link failure causes copied-file deletion and Document disable.
- Cleanup failure is surfaced as an `AggregateException`.
- JSON files are independently replaced; there is no transaction spanning payload, document metadata, and link metadata.

## G. Current Storage Facts

| Fact | Current behavior |
|---|---|
| Attachment root | `<runtime root>/attachments` |
| Metadata root | `<runtime root>/data/local` |
| Payload key | `documents/<physicalFileName>` |
| Document JSON | `documents.json` |
| Policy link JSON | `policy-documents.json` |
| Claim link JSON | `claim-documents.json` |
| Current filename collision | Suffix indices `1` through `999` |
| Extension policy | `pdf`, `jpg`, `jpeg`, `png`, normalized case-insensitively |
| Current copy mode | Direct `File.Copy` to final path, no overwrite |
| Current integrity | Byte length returned; no durable hash/signature |
| Current duplicate rule | Same document ID cannot have duplicate active link |

## H. Source Conflict

`WpfFilePickerService` currently presents `webp`, `bmp`, and an all-files choice in addition to the actual `FileNamePolicyService` allowlist. The registration workflow rejects unsupported extensions. This is one current source blocker because the picker promises choices that the use case does not accept.

Candidate resolution:

- Picker filter must expose only `pdf`, `jpg`, `jpeg`, `png`.
- The lower policy remains authoritative.
- Picker filtering is convenience, not security validation.

## I. Architecture Comparison

| Criterion | A: Reuse workflow | B: Product parallel workflow | C: View direct storage |
|---|---:|---:|---:|
| Reuses tested rollback | Yes | No | No |
| Single policy source | Yes | No | No |
| Maintains UI/I/O boundary | Yes | Partial | No |
| Composition change size | Low/medium | High | Medium |
| Drift risk | Low | High | High |
| Gate8 recommendation | `SELECT CANDIDATE` | Reject | Reject |

## J. Candidate Ownership

| Concern | Candidate owner |
|---|---|
| UI event forwarding | `ProductDocumentRegistrationView.xaml.cs` |
| Draft, target, busy, status, validation state | `DocumentRegistrationViewModel` |
| Registration use case, same-process serialization, and normal-exception compensation | `DocumentRegistrationWorkflow` |
| Target active-state and target-scoped duplicate decision | Workflow plus storage query, held in one same-process serialized critical section |
| File validation and staged file lifecycle | New lower storage validation service plus `LocalFileAttachmentService` |
| Metadata persistence | `IDocumentStorageService` / `JsonDocumentStorageService` |
| Concrete wiring and runtime roots | `AppServices` |
| ProductShell navigation/lifetime | Existing `ProductShellViewModel` and `ProductShellWindow` |

### J1. Gate8 Corrected Integrity and Consistency Boundary

| Concern | Current source | Gate8 corrected candidate |
|---|---|---|
| Changed after selection | No authoritative selection-time content snapshot | Open the selected source read-only, compute a SHA-256 runtime snapshot, then compare it with the staged payload SHA-256 during registration; mismatch requires reselection |
| Auxiliary file facts | Length and last-write can be observed but do not prove content identity | Length and last-write are early auxiliary checks only; SHA-256 equality is authoritative |
| Durable source data | Runtime source path is transient | Neither the selection hash nor original source path is stored in durable Document or link metadata |
| Concurrent duplicate registration | ViewModel busy state prevents only one UI command instance | Duplicate query plus registration for the same target/SHA-256 is serialized inside one process and exactly one concurrent attempt succeeds |
| Cross-process registration | No shared transaction or process lock | Not guaranteed in Gate8; multi-process and production readiness remain on hold |
| Normal exception consistency | Existing workflow provides compensation-oriented cleanup | Gate8 retains normal-exception compensation and defines the returned-success state as internally consistent |
| Crash consistency | No startup recovery coordinator | A crash after final move can leave an orphan final payload or a Document without a link; startup recovery remains deferred and production-ready claims are prohibited |

## K. Current Blocker Count

| Blocker class | Count | Items |
|---|---:|---|
| Source blockers | `1` | picker filter conflicts with actual allowlist |
| Composition blockers | `0` | reusable central composition already exists |
| Lifecycle blockers | `3` | success reset, reentry transient-state reset, navigation/busy serialization |

## L. Reconciliation Result

- Current source inventory file count: `58`
- Selected architecture candidate: `A`
- New parallel workflow: not recommended
- View direct storage: forbidden
- Implementation status: not authorized

`POLICY_CLAIM_PRODUCT_UI_SHELL_GATE8_CURRENT_SOURCE_WORKFLOW_STORAGE_AND_COMPOSITION_RECONCILIATION_READY`

## M. Package Consistency Register

| Item | Package-wide value |
|---|---|
| Baseline HEAD | `79d8f1d5b76b22b0ef1a65fbf781c833bbcd7fff` |
| Baseline subject | `docs(familyclaimref): close gate7 default startup transition` |
| Baseline parent | `2ff924c846d2b5f7fad905afa5a7a90d93af31cf` |
| `docs/412` SHA-256 | `021AEE4719B402E465EBC2E74B958668E6BF19DF37A72112370B8D16020CB4FA` |
| Architecture | Candidate A, reuse existing workflow |
| Workflow owner | `DocumentRegistrationWorkflow` |
| File storage owner | `IFileAttachmentService` / `LocalFileAttachmentService` |
| Metadata repository owner | `IDocumentStorageService` / `JsonDocumentStorageService` |
| Target repository owner | `IPolicyClaimStorageService` / `JsonPolicyClaimStorageService` |
| Composition owner | `AppServices`; ProductShell window-scoped child ViewModel |
| Authoritative payload | App-managed copy after complete success |
| Reentry | Refresh targets, preserve draft, clear stale target/transient copy |
| Duplicate key | active `target kind + target ID + SHA-256` |
| Selection integrity | Read-only selection SHA-256 runtime snapshot compared with staged payload SHA-256; mismatch requires reselection; length/last-write are auxiliary only; selection hash and source path are not durable |
| Concurrency boundary | Same-process duplicate query plus registration is serialized; concurrent same target/SHA-256 yields exactly one success; cross-process guarantee is excluded |
| Picker cancel | Preserve prior valid selection and draft |
| Consistency contract | Successful-return consistency with normal-exception compensation; crash consistency and startup recovery remain deferred |
| Crash residual risk | Orphan final payload and Document without a link can remain after a process crash following final move |
| Current source inventory files | `58` |
| Metadata items | `31` |
| Metadata classification | `18/1/3/1/8` |
| Future exact implementation files | `35` |
| New resource key candidates | `8` |
| New automated scenario candidates | `37` |
| Unresolved blockers | `16` |
| Implementation readiness | `HOLD_IMPLEMENTATION_NOT_AUTHORIZED` |
| Deployment/production readiness | `NOT_AUTHORIZED`; multi-process uniqueness and startup recovery remain on hold |
| Documentation commit | `NOT_AUTHORIZED` |
| Non-approval | No source/test/resource/runtime/commit/deployment approval |
| Package final marker | `POLICY_CLAIM_PRODUCT_UI_SHELL_GATE8_REAL_DOCUMENT_REGISTRATION_ATTACHMENT_AND_PERSISTENCE_DECISION_PACKAGE_PASS_USER_REVIEW_PENDING` |
