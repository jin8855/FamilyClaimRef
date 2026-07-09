# Policy Claim Deferred Diagnostic Summary Format Ownership Decision Plan

## 1. Status

DIAGNOSTIC_SUMMARY_FORMAT_OWNERSHIP_DECISION_PLAN_ONLY

## 2. Marker

POLICY_CLAIM_DEFERRED_DIAGNOSTIC_SUMMARY_FORMAT_OWNERSHIP_DECISION_PLANNED

## 3. Baseline Commit

- `b131255 docs(familyclaimref): add scenario9 cleanup dry-run report`

## 4. Current Format Inventory

| Format | Source location | Placeholder contract | Current purpose | Current exposure | Extraction decision | Reason |
|---|---|---|---|---|---|---|
| `policy:{policyId}; document:{documentId}` | `DocumentRegistrationViewModel.CreatePolicySummary` | `{policyId}`, `{documentId}` | diagnostic policy registration summary format | `LastRegistrationSummary` validation harness display | Keep deferred | diagnostic summary format, final display model 미확정, id placeholder display policy 미확정 |
| `claim:{claimId}; document:{documentId}` | `DocumentRegistrationViewModel.CreateClaimSummary` | `{claimId}`, `{documentId}` | diagnostic claim registration summary format | `LastRegistrationSummary` validation harness display | Keep deferred | diagnostic summary format, final display model 미확정, id placeholder display policy 미확정 |

## 5. Extraction Decision Vocabulary

이 문서의 extraction decision 값은 다음 중 하나만 사용한다.

- Keep deferred
- Diagnostic resource candidate, not approved
- Product display candidate, needs final display model
- Exclude
- Unknown

## 6. Default Judgment

두 format은 현재 `Keep deferred`로 둔다.

이유:

- diagnostic summary format이다.
- final display model이 아직 확정되지 않았다.
- id placeholder display policy가 아직 확정되지 않았다.
- product UI shell이 아직 미승인 상태다.
- 사용자 친화 copy로 확정하기에는 이르다.
- current source behavior를 이번 batch에서 변경하지 않는다.

## 7. Future Decision Options

1. Keep as diagnostic hard-coded format.
2. Extract as diagnostic resource key.
3. Replace with final product-facing display model.
4. Move to structured summary object rather than localized string.
5. Defer until product UI shell.

## 8. Candidate Resource Keys

아래 key는 Candidate only다. 구현 승인이나 naming 확정이 아니다.

| Format | Candidate only resource key | Status |
|---|---|---|
| `policy:{policyId}; document:{documentId}` | `Ui.DocumentRegistration.Summary.PolicyDiagnosticFormat` | Candidate only, not approved |
| `claim:{claimId}; document:{documentId}` | `Ui.DocumentRegistration.Summary.ClaimDiagnosticFormat` | Candidate only, not approved |

## 9. Recommended Decision

Current batch recommendation:

- Keep deferred until final display model decision.
- Do not extract these two formats in the next implementation batch unless user explicitly approves diagnostic resource extraction.
- If product UI shell starts first, prefer product-facing structured display design before resource key implementation.
- If validation harness remains the target, diagnostic ownership can be approved separately.

## 10. Non-Scope Confirmation

- `UiStrings.xaml` key 추가 없음.
- `UiTextKeys.cs` constant 추가 없음.
- `DocumentRegistrationViewModel` format 변경 없음.
- `DocumentRegistrationViewModelTests` assertion 변경 없음.
- `ResourceUiTextProviderTests` assertion 변경 없음.
- final Korean copy 작성 없음.
- product UI shell 승인 없음.
- cleanup 실행 없음.

## 11. Final Marker

POLICY_CLAIM_DEFERRED_DIAGNOSTIC_SUMMARY_FORMAT_OWNERSHIP_DECISION_READY
