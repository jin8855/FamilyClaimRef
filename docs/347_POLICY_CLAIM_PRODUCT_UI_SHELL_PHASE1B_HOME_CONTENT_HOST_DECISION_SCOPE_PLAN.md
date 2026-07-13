# Product UI Shell Phase 1B Home Content Host Decision Scope Plan

## A. Status

- Task ID: `POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1B_HOME_CONTENT_HOST_EXACT_SCOPE_DECISION_DOCS_BATCH`
- Marker: `POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1B_HOME_CONTENT_HOST_DECISION_SCOPE_READY`
- Work type: documentation-only exact-scope decision candidate
- Implementation target now: 0

## B. Baseline

- Full hash: `c53cc53f82413973d0d897e6fa18b2bf95f24730`
- Subject: `feat(familyclaimref): add compile-only product shell skeleton`
- Initial working tree: clean
- Initial staged files: none
- Full solution tests: PASS 351/351, known committed baseline
- `Ui.*` resources/constants: 64/64
- `Ui.Product.*` resources/constants: 8/8
- Phase 1A compile-only skeleton: committed
- Initial selected navigation item: Home
- ProductShell runtime entry: none
- ProductHomeView: absent
- ProductHomeViewModel: absent

## C. Purpose

Phase 1B1 defines the smallest candidate boundary for a compile-only Home landing view and a ProductShell content host. It also defines how the current Home navigation selection could map to product content without enabling runtime entry.

Phase 1B1 is phased delivery, not deletion of the final Home/dashboard scope. Dashboard summaries, metrics, alerts, recent activity, and action cards remain deferred until their data and copy are separately approved.

## D. In Scope

- Inspect the committed ProductShell placeholder and selection contract.
- Compare content-host architecture candidates.
- Decide whether a title-only Home view needs a dedicated ViewModel.
- Identify a future exact implementation file list candidate.
- Define future build and test gates.
- Keep all implementation approvals closed.

## E. Out of Scope

- Source, test, XAML, ViewModel, resource, or project-file modification
- Runtime entry, `MainWindow` replacement, App startup change, or AppServices composition
- Document registration workflow wiring or document list data-source wiring
- ProductDocumentRegistrationView or ProductDocumentListView creation
- Invented dashboard data, counts, amounts, activity, alerts, cards, or calls to action
- New `Ui.Product.*`, `Ui.BusinessDuplicate.*`, or `Ui.ActionResult.*` resources
- DB, SQLite, repository, OCR, migration, backup, rollback, or storage changes
- App launch, workflow execution, screenshot, or visual automation
- Cleanup or protected-path access
- Build, test, stage, commit, or push

## F. Home Content Boundary

The current approved Home-facing copy is limited to:

- `Ui.Product.Navigation.Home`
- `Ui.Product.Home.Title`

These keys support a title-only Home landing view candidate. They do not authorize any metric, subtitle, summary card, empty-state message, alert, recent activity, or action copy.

## G. Protected Boundary

- `data/claimdoc/`: Never read, list, search, use, select, stage, commit, delete, or move.
- `docs/nightwork_*`: Do not read or search internally.
- No real personal, insurance, hospital, diagnosis, or claim sample data is allowed.

## H. Approval Matrix

| Approval item | Approved now |
|---|---|
| ProductHomeView creation | no |
| ProductHomeViewModel creation | no |
| ProductShellWindow modification | no |
| ProductShellViewModel modification | no |
| content-host implementation | no |
| new resource addition | no |
| AppServices modification | no |
| runtime entry | no |
| MainWindow replacement | no |
| App startup change | no |
| exact implementation file list | no |

## I. Execution Record

- Documentation-only: yes
- Build/test: not run
- Git stage/commit: not run
- Source implementation: not started
- `docs/352` creation: no
