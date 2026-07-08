# Policy/Claim Lifecycle Persistence Test Scope Review

## A. Status

Status: TEST_SCOPE_REVIEW_ONLY

This document records read-only findings for a future automated validation test.

No code is modified by this document.

No test is implemented by this document.

## B. Read-Only Source Findings

### Confirmed

- `IPolicyClaimStorageService` exposes policy creation through `AddPolicyAsync`.
- `IPolicyClaimStorageService` exposes claim creation through `AddClaimAsync`.
- `IPolicyClaimStorageService` exposes policy disable through `DisablePolicyAsync`.
- `IPolicyClaimStorageService` exposes claim disable through `DisableClaimAsync`.
- `IPolicyClaimStorageService` exposes active policy query through `GetPoliciesAsync`.
- `IPolicyClaimStorageService` exposes active claim query through `GetClaimsAsync`.
- `IPolicyClaimStorageService` exposes active claim-by-policy query through `GetClaimsByPolicyIdAsync`.
- `IPolicyClaimStorageService` exposes active lookup helpers through `GetPolicyAsync`, `GetClaimAsync`, `PolicyExistsAsync`, and `ClaimExistsAsync`.
- `JsonPolicyClaimStorageService.GetPoliciesAsync` returns records where `DisabledAt is null`.
- `JsonPolicyClaimStorageService.GetClaimsAsync` returns records where `DisabledAt is null`.
- `JsonPolicyClaimStorageService.GetClaimsByPolicyIdAsync` filters active claims by `PolicyId`.
- `JsonPolicyClaimStorageService.AddClaimAsync` requires an active policy through `EnsureActivePolicyExistsAsync`.
- `JsonPolicyClaimStorageService.DisablePolicyAsync` sets `UpdatedAt` and `DisabledAt`.
- `JsonPolicyClaimStorageService.DisableClaimAsync` sets `UpdatedAt` and `DisabledAt`.
- `JsonPolicyClaimStorageService` stores data in `policies.json` and `claims.json` under the configured metadata root path.
- Existing `JsonPolicyClaimStorageServiceTests` confirm policy creation persistence after recreating `JsonPolicyClaimStorageService`.
- Existing `JsonPolicyClaimStorageServiceTests` confirm claim creation persistence after recreating `JsonPolicyClaimStorageService`.
- Existing `JsonPolicyClaimStorageServiceTests` confirm active-only policy query behavior.
- Existing `JsonPolicyClaimStorageServiceTests` confirm active-only claim query behavior.
- Existing `AppServicesTests` confirm `AppServices.Create` accepts an `IRuntimeRootProvider` and uses its runtime, metadata, and attachment paths.
- Existing isolated runtime integration tests confirm `AppServices.CreateDefault` can run under environment runtime override and keep project root `attachments/`, `data/local/`, and `runtime_test_document.*` unchanged.

### Candidate

- A future lifecycle persistence test can recreate `AppServices` with the same isolated runtime root to verify reload behavior.
- A future lifecycle persistence test can use `JsonPolicyClaimStorageService` directly if the validation does not need ViewModel behavior.
- Disabled record persistence may need to be verified by reading the JSON envelope through `JsonFileStore<PolicyRecord>` and `JsonFileStore<ClaimRecord>` or by direct JSON deserialization in test code.
- Active-only filtering can be verified through the existing public query methods.
- Project root cleanliness can follow the snapshot pattern already used by `AppServicesTests` and `IsolatedRuntimeDocumentWorkflowTests`.

### Unknown

- No public service method was confirmed that returns disabled policy records through `IPolicyClaimStorageService`.
- No public service method was confirmed that returns disabled claim records through `IPolicyClaimStorageService`.
- The final test location is not yet decided: `tests/FamilyClaimRef.App.Tests/Integration/PolicyClaimLifecyclePersistenceTests.cs` is recommended, but repository-conventional equivalent remains acceptable.
- It is not yet decided whether the implementation should test only `JsonPolicyClaimStorageService` or include `AppServices` recreation.

## C. Risk Review

Record:

- API may not expose active-only filtering separately because the current public list and lookup methods are already active-only.
- disable semantics are service-level in `JsonPolicyClaimStorageService`, while UI behavior may require separate validation later.
- test may need to assert persisted disabled state by inspecting stored JSON rather than by public active-only lookup.
- if production code change is needed, implementation batch must STOP_AND_REPORT.

## D. Recommended Test Strategy

Record:

- prefer service-level tests for lifecycle persistence.
- use isolated runtime root env override only if the test intentionally validates `AppServices` composition.
- recreate `AppServices` or `JsonPolicyClaimStorageService` for reload validation.
- verify active-only filtering via `GetPoliciesAsync`, `GetClaimsAsync`, `GetClaimsByPolicyIdAsync`, `GetPolicyAsync`, `GetClaimAsync`, `PolicyExistsAsync`, and `ClaimExistsAsync`.
- verify disabled persisted state through a read-only storage inspection approach if public service methods continue to hide disabled records.
- avoid UI automation.
- avoid manual workflow.
