# Product UI Shell Phase 1B Home Content Host Source Dependency Reconciliation

## A. Status

- Marker: `POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1B_HOME_CONTENT_HOST_SOURCE_DEPENDENCY_RECONCILIATION_READY`
- Baseline: `c53cc53f82413973d0d897e6fa18b2bf95f24730`
- Work type: read-only source and document reconciliation

## B. Evidence Matrix

| Area | Actual evidence | Phase 1B1 implication | Status |
|---|---|---|---|
| Current ProductShellWindow content placeholder | The right-side region displays `SelectedNavigationItem.DisplayText` in a `TextBlock`. | Replace that region with a future `ContentControl` candidate while preserving the shell grid and navigation. | Source-confirmed |
| ProductShellWindow DataContext contract | Constructor injection assigns `ProductShellViewModel` as DataContext. | Compile-only Home content does not require a new composition boundary. | Source-confirmed |
| ProductShellViewModel public state | Exposes shell title, three navigation items, and selected navigation item only. | Current selection state can drive a small XAML template switch. | Source-confirmed |
| SelectedNavigationItem behavior | Home is initial; null is ignored; foreign items are rejected. | Existing guards remain sufficient when the ViewModel is unchanged. | Source-confirmed |
| Navigation Id contract | Stable IDs include `Home`, `DocumentRegistration`, and `DocumentList`. | `Home` can be the smallest template-selection key, with expansion risk noted. | Candidate supported |
| Current ProductShell tests | ProductShellViewModelTests pass 9/9 and ProductNavigationItemViewModelTests pass 8/8 in the known baseline. | Existing ViewModel tests remain the targeted regression gate if no ViewModel changes occur. | Source-confirmed |
| Existing Views/UserControl convention | No tracked product `Views/` UserControl implementation exists. | ProductHomeView would establish the first view-only convention and needs explicit implementation approval. | Needs explicit implementation approval |
| ContentControl/DataTemplate convention | No current product-shell ContentControl or DataTemplate switching convention exists. | The first content host must remain narrowly scoped and compile-only. | Candidate supported |
| IUiTextProvider convention | ViewModels use resource-backed text providers; XAML also consumes application resources. | A static title-only view can use the approved resource directly without a new Home ViewModel. | Source-confirmed |
| Ui.Product.Home.Title | `Ui.Product.Home.Title` and `UiTextKeys.ProductHomeTitle` exist and are aligned. | No new resource is required for a title-only Home candidate. | Source-confirmed |
| Home wireframe/source evidence | Home/dashboard is a Phase 1 product entry candidate; detailed dashboard contents remain broader candidates. | Implement only a structural title landing view; do not invent dashboard content. | Source-confirmed |
| AppServices composition | ProductShell is not composed by AppServices. | AppServices is not required for compile-only Home XAML. | Not required |
| MainWindow/App startup | App startup still opens MainWindow; ProductShellWindow is not instantiated. | Runtime entry remains absent before and after any future Phase 1B1 compile-only work. | Source-confirmed |
| Project default WPF inclusion | SDK-style WPF project uses default XAML/C# inclusion. | A future view pair should not require a project-file modification. | Source-confirmed |
| Resource impact | Product resources/constants are 8/8; overall resources/constants are 64/64. | Keep both counts unchanged for the minimal title-only candidate. | Not required |
| Test convention | Existing tests exercise ViewModel guards; no visual or XAML runtime test convention is present. | Build validates XAML; existing shell tests validate unchanged selection behavior. | Source-confirmed |

## C. Required Questions

| Question | Answer | Status |
|---|---|---|
| 1. Which files are needed to replace the placeholder with actual content? | Candidate files are `Views/ProductHomeView.xaml`, `Views/ProductHomeView.xaml.cs`, and a modification to `ProductShell/ProductShellWindow.xaml`. | Candidate supported |
| 2. Is ProductHomeViewModel required? | No. The minimal Home candidate has no state beyond the approved static title resource. | Not required |
| 3. Is ProductShellViewModel modification required? | No. Existing `SelectedNavigationItem` and stable `Home` Id can drive the minimal XAML switch. | Not required |
| 4. Is shell XAML-only switching possible? | Yes. A `ContentControl` and XAML template/data-trigger candidate can map the selected `Home` item to ProductHomeView. | Candidate supported |
| 5. Is code-behind switching required? | No. Event-driven view creation would add state ownership and reduce testability. | Not required |
| 6. Is compile-only Home possible without AppServices modification? | Yes. The shell already receives its DataContext, and the view has no service dependency. | Candidate supported |
| 7. Is an additional resource required? | No for a title-only view using `Ui.Product.Home.Title`. Richer content would need separate copy/resource approval. | Not required |
| 8. What Home content is source-confirmed? | Home is a product entry/dashboard candidate and has approved navigation/title copy. Detailed metrics and dashboard content are not confirmed for this slice. | Source-confirmed |
| 9. Can runtime entry remain blocked before Home implementation? | Yes. ProductShellWindow remains unreferenced from startup, MainWindow, and AppServices. | Source-confirmed |
| 10. Does the candidate block future registration/list extension? | No. Further templates can be added later, but the string-Id approach should be reconsidered if the shell gains richer page state. | Deferred |

## D. Architecture Reconciliation

- ViewModel-driven current content is more scalable and testable but adds state and files not justified by a static title-only Home view.
- SelectedNavigationItem-based XAML switching is the smallest compile-only candidate and preserves current ViewModel behavior.
- Window code-behind switching is not required and is not recommended.
- A navigation/router service is disproportionate for Phase 1B1 and remains deferred.

## E. Blockers and Conditions

- Source blocker for a title-only Home view: none.
- Copy/resource blocker for a title-only Home view: none.
- Rich dashboard content: deferred and requires explicit source, copy, data, and implementation approval.
- Any implementation: still requires explicit user approval and an approved exact file list.
