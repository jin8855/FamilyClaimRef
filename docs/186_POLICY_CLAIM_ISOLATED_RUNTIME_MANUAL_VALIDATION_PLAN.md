# Policy/Claim Isolated Runtime Manual Validation Plan

## A. Status

Status: MANUAL_VALIDATION_PLAN_ONLY

Marker:

```text
POLICY_CLAIM_ISOLATED_RUNTIME_MANUAL_VALIDATION_PLANNED
```

No app launch is authorized by this document.

No OpenFileDialog is authorized by this document.

No workflow execution is authorized by this document.

No synthetic file creation is authorized by this document.

No cleanup is authorized by this document.

## B. Baseline

- latest commit:
  `442fa01 test(familyclaimref): validate document workflow in isolated runtime root`
- source docs reviewed:
  - `docs/182_POLICY_CLAIM_RUNTIME_ROOT_PROVIDER_IMPLEMENTATION_RESULT_REVIEW.md`
  - `docs/183_POLICY_CLAIM_ISOLATED_RUNTIME_AUTOMATED_VALIDATION_PLAN.md`
  - `docs/185_POLICY_CLAIM_ISOLATED_RUNTIME_AUTOMATED_VALIDATION_RESULT_REVIEW.md`
- current runtime evidence check:
  - default runtime metadata existence/count check only
  - default runtime attachment directory existence/count check only
  - metadata expected file count observed: 5
  - attachment file count observed: 3
  - no runtime metadata file content opened
  - no runtime attachment file content opened

## C. Purpose

- Automated isolated runtime validation passed.
- Next manual validation should prove the current validation harness can run under an isolated runtime root.
- The manual check should use an isolated runtime override and should not depend on existing default runtime evidence.
- UI redesign remains deferred.
- Current `MainWindow` remains a validation harness.
- Korean localization remains deferred.

## D. Manual Scenario

Scenario name:

```text
SCENARIO9_ISOLATED_RUNTIME_POLICY_CLAIM_DOCUMENT_REGISTRATION
```

Scenario goals:

1. Launch app with isolated runtime env vars.
2. Verify old `%LOCALAPPDATA%/FamilyClaimRef` evidence is not used for active validation state.
3. Create synthetic policy target.
4. Create synthetic claim target.
5. Register synthetic policy document.
6. Register synthetic claim document.
7. Verify isolated runtime root contains metadata/link/attachments.
8. Verify `%LOCALAPPDATA%/FamilyClaimRef` evidence is preserved.
9. Verify project root remains clean.

## E. Approval Marker For Future Execution

Required marker:

```text
POLICY_CLAIM_ISOLATED_RUNTIME_MANUAL_VALIDATION_APPROVED
```

This marker appearing in this document is only a definition, not approval.

Future execution must be separately approved by the user.

## F. Proposed Isolated Root

Use:

```text
%TEMP%/FamilyClaimRef-Isolated/scenario9_manual_validation
```

or timestamped equivalent:

```text
%TEMP%/FamilyClaimRef-Isolated/scenario9_manual_validation_<timestamp>
```

Do not use:

- `%LOCALAPPDATA%/FamilyClaimRef`
- project root
- `data/claimdoc`

## G. Deferred / Forbidden

Deferred:

- UI redesign
- Korean localization
- resource extraction
- wireframe port
- runtime metadata cleanup
- runtime attachment cleanup

Forbidden until explicit future approval:

- app launch
- OpenFileDialog execution
- manual document registration workflow execution
- synthetic file creation
- runtime metadata deletion
- runtime attachment deletion
- full runtime root cleanup
- project root cleanup
- `data/claimdoc` read/list/use/select/stage/commit/delete/move
- DB/SQLite/OCR/repository implementation

Full runtime root cleanup remains rejected.
