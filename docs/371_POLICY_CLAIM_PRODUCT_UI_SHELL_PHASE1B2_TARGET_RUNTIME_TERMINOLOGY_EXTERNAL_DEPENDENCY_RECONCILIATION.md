# Policy Claim Product UI Shell Phase 1B2 Target Runtime Terminology External Dependency Reconciliation

## A. Status

- Marker: `POLICY_CLAIM_PRODUCT_UI_SHELL_PHASE1B2_TARGET_RUNTIME_TERMINOLOGY_EXTERNAL_DEPENDENCY_RECONCILIATION_READY`
- Audit scope: six exact old values in tracked `app/tests`
- Total exact old-value occurrences: 25
- Original candidate external occurrences: 2
- Dependencies beyond the revised candidate: 0
- Unresolved occurrences: 0
- Implementation target now: 0

## B. Dependency Matrix

| Old value | File | Symbol/helper | Resource key | Semantic role | Required future action | Status |
|---|---|---|---|---|---|---|
| `선택할 수 있는 활성 청구 대상이 없습니다.` | `UiStrings.xaml` | `ResourceDictionary` | `Ui.DocumentRegistration.Message.NoActiveClaim` | canonical resource value | change to Candidate A value | candidate, not approved |
| `선택할 수 있는 활성 보험 대상이 없습니다.` | `UiStrings.xaml` | `ResourceDictionary` | `Ui.DocumentRegistration.Message.NoActivePolicy` | canonical resource value | change to Candidate A value | candidate, not approved |
| `문서를 등록하기 전에 청구 대상을 선택해 주세요.` | `UiStrings.xaml` | `ResourceDictionary` | `Ui.DocumentRegistration.Validation.SelectClaimBeforeRegister` | canonical resource value | change to Candidate A value | candidate, not approved |
| `문서를 등록하기 전에 보험 대상을 선택해 주세요.` | `UiStrings.xaml` | `ResourceDictionary` | `Ui.DocumentRegistration.Validation.SelectPolicyBeforeRegister` | canonical resource value | change to Candidate A value | candidate, not approved |
| `저장할 대상을 선택해 주세요.` | `UiStrings.xaml` | `ResourceDictionary` | `Ui.DocumentRegistration.Validation.SelectTarget` | canonical resource value | change to Candidate A value | candidate, not approved |
| `저장할 대상 유형을 선택해 주세요.` | `UiStrings.xaml` | `ResourceDictionary` | `Ui.DocumentRegistration.Validation.SelectTargetKind` | canonical resource value | change to Candidate A value | candidate, not approved |
| `선택할 수 있는 활성 보험 대상이 없습니다.` | `DocumentRegistrationViewModelTests.cs` | `LoadTargetOptionsAsync_no_active_policy_shows_empty_state_message` | `DocumentRegistrationMessageNoActivePolicy` | exact user-visible assertion | update expected value | candidate, not approved |
| `선택할 수 있는 활성 청구 대상이 없습니다.` | `DocumentRegistrationViewModelTests.cs` | `LoadTargetOptionsAsync_no_active_claim_shows_empty_state_message` | `DocumentRegistrationMessageNoActiveClaim` | exact user-visible assertion | update expected value | candidate, not approved |
| `문서를 등록하기 전에 보험 대상을 선택해 주세요.` | `DocumentRegistrationViewModelTests.cs` | `RegisterAsync_without_selected_policy_target_is_blocked` | `DocumentRegistrationValidationSelectPolicyBeforeRegister` | exact user-visible assertion | update expected value | candidate, not approved |
| `문서를 등록하기 전에 청구 대상을 선택해 주세요.` | `DocumentRegistrationViewModelTests.cs` | `RegisterAsync_without_selected_claim_target_is_blocked` | `DocumentRegistrationValidationSelectClaimBeforeRegister` | exact user-visible assertion | update expected value | candidate, not approved |
| `저장할 대상을 선택해 주세요.` | `DocumentRegistrationViewModelTests.cs` | `RegisterAsync_missing_target_id_rejects` | `DocumentRegistrationValidationSelectTarget` | exact user-visible assertion | update expected value | candidate, not approved |
| `저장할 대상 유형을 선택해 주세요.` | `DocumentRegistrationViewModelTests.cs` | `RegisterAsync_invalid_target_kind_rejects` | `DocumentRegistrationValidationSelectTargetKind` | exact user-visible assertion | update expected value | candidate, not approved |
| `선택할 수 있는 활성 청구 대상이 없습니다.` | `DocumentRegistrationViewModelTests.cs` | `CreateUiTextProvider` | `DocumentRegistrationMessageNoActiveClaim` | test provider dictionary | update mirrored value | candidate, not approved |
| `선택할 수 있는 활성 보험 대상이 없습니다.` | `DocumentRegistrationViewModelTests.cs` | `CreateUiTextProvider` | `DocumentRegistrationMessageNoActivePolicy` | test provider dictionary | update mirrored value | candidate, not approved |
| `문서를 등록하기 전에 청구 대상을 선택해 주세요.` | `DocumentRegistrationViewModelTests.cs` | `CreateUiTextProvider` | `DocumentRegistrationValidationSelectClaimBeforeRegister` | test provider dictionary | update mirrored value | candidate, not approved |
| `문서를 등록하기 전에 보험 대상을 선택해 주세요.` | `DocumentRegistrationViewModelTests.cs` | `CreateUiTextProvider` | `DocumentRegistrationValidationSelectPolicyBeforeRegister` | test provider dictionary | update mirrored value | candidate, not approved |
| `저장할 대상을 선택해 주세요.` | `DocumentRegistrationViewModelTests.cs` | `CreateUiTextProvider` | `DocumentRegistrationValidationSelectTarget` | test provider dictionary | update mirrored value | candidate, not approved |
| `저장할 대상 유형을 선택해 주세요.` | `DocumentRegistrationViewModelTests.cs` | `CreateUiTextProvider` | `DocumentRegistrationValidationSelectTargetKind` | test provider dictionary | update mirrored value | candidate, not approved |
| `선택할 수 있는 활성 청구 대상이 없습니다.` | `ResourceUiTextProviderTests.cs` | `Approved_korean_copy_values_resolve_from_UiStrings` | `DocumentRegistrationMessageNoActiveClaim` | exact resource-value assertion | update expected value | candidate, not approved |
| `선택할 수 있는 활성 보험 대상이 없습니다.` | `ResourceUiTextProviderTests.cs` | `Approved_korean_copy_values_resolve_from_UiStrings` | `DocumentRegistrationMessageNoActivePolicy` | exact resource-value assertion | update expected value | candidate, not approved |
| `문서를 등록하기 전에 청구 대상을 선택해 주세요.` | `ResourceUiTextProviderTests.cs` | `Approved_korean_copy_values_resolve_from_UiStrings` | `DocumentRegistrationValidationSelectClaimBeforeRegister` | exact resource-value assertion | update expected value | candidate, not approved |
| `문서를 등록하기 전에 보험 대상을 선택해 주세요.` | `ResourceUiTextProviderTests.cs` | `Approved_korean_copy_values_resolve_from_UiStrings` | `DocumentRegistrationValidationSelectPolicyBeforeRegister` | exact resource-value assertion | update expected value | candidate, not approved |
| `저장할 대상을 선택해 주세요.` | `ResourceUiTextProviderTests.cs` | `Approved_korean_copy_values_resolve_from_UiStrings` | `DocumentRegistrationValidationSelectTarget` | exact resource-value assertion | update expected value | candidate, not approved |
| `저장할 대상 유형을 선택해 주세요.` | `AppServices.cs` | `CreateUiTextProvider` fallback dictionary | `DocumentRegistrationValidationSelectTargetKind` | executable fallback resource dictionary | update only this mirrored value | included in revised candidate, not approved |
| `저장할 대상 유형을 선택해 주세요.` | `PolicyClaimManagementViewModelTests.cs` | `CreateDocumentRegistrationUiTextProvider` | `DocumentRegistrationValidationSelectTargetKind` | document-registration provider fixture | update only this fixture value | included in revised candidate, not approved |

## C. AppServices Classification

- The occurrence is inside `CreateUiTextProvider()`.
- `Application.Current != null` returns a provider backed by `Application.Current.Resources`.
- `Application.Current == null` executes the fallback `Dictionary<string, string>` branch.
- The observed string is keyed by `UiTextKeys.DocumentRegistrationValidationSelectTargetKind`.
- It is executable fallback data, not a comment, dead constant, or composition rule.
- A future value-only update changes no constructor, dependency graph, runtime root, startup logic, MainWindow composition, or ProductShell composition.
- Inclusion judgment: include `AppServices.cs` in the revised future candidate.
- Production source-code modification required: yes, limited to this exact fallback dictionary value.

## D. PolicyClaimManagementViewModelTests Classification

- The occurrence is inside `CreateDocumentRegistrationUiTextProvider()`.
- That helper is passed to a `DocumentRegistrationViewModel` created by the test fixture.
- The key is `UiTextKeys.DocumentRegistrationValidationSelectTargetKind`.
- It is not a Policy/Claim management user-message assertion.
- It does not define or validate management behavior.
- Leaving it unchanged would preserve an obsolete mirrored document-registration fixture value.
- Inclusion judgment: include `PolicyClaimManagementViewModelTests.cs` in the revised future candidate.
- Future change boundary: update this fixture value only; do not change management assertions, methods, or production management code.

## E. Resource Test Coverage Note

`ResourceUiTextProviderTests.cs` has five direct exact-value rows for the six Candidate A keys. `DocumentRegistrationValidationSelectTargetKind` is present in `RuntimeMessageKeys`, but it has no direct exact-value `InlineData` row. A future implementation that requires direct exact assertions for all six values must add that one row and explain the evidence-backed test-count change.

## F. Audit Judgment

- Total exact old-value occurrences: 25.
- Occurrences covered by the original four-file candidate: 23.
- External occurrences missing from the original candidate: 2.
- Dependencies outside the revised six-file candidate: 0.
- Unresolved semantic roles: 0.
- Revised candidate readiness: `READY_AS_CANDIDATE_ONLY`.
- Implementation approval: no.
