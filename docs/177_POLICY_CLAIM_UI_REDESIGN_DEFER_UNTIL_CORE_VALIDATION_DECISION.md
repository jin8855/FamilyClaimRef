# Policy/Claim UI Redesign Defer Until Core Validation Decision

## A. Status

Status: DECISION_ONLY

Marker:

```text
POLICY_CLAIM_UI_REDESIGN_DEFERRED_UNTIL_CORE_VALIDATION_COMPLETE
```

This document records a UI sequencing decision only.

No UI implementation is authorized by this document.

No XAML change is authorized by this document.

No Korean localization implementation is authorized by this document.

No resource extraction implementation is authorized by this document.

No wireframe port is authorized by this document.

## B. Baseline

- latest commit: `5f2e995 docs(familyclaimref): review isolated runtime root design`
- source docs reviewed:
  - `docs/173_POLICY_CLAIM_PHASE3D_RUNTIME_EVIDENCE_CLOSURE_REVIEW.md`
  - `docs/174_POLICY_CLAIM_PHASE3D_CLOSURE_DOCS_COMMIT_CANDIDATE_REVIEW.md`
  - `docs/175_POLICY_CLAIM_ISOLATED_RUNTIME_ROOT_DESIGN_REVIEW.md`
  - `docs/176_POLICY_CLAIM_ISOLATED_RUNTIME_ROOT_DESIGN_COMMIT_CANDIDATE_REVIEW.md`

## C. User Decision

- 기능 개발과 검증을 모두 마친 다음 화면을 바꾼다.
- 지금 화면을 바꾸면 기능 개발, runtime validation, localization, wireframe port가 서로 얽힌다.
- 현재 `MainWindow`는 product UI가 아니라 validation harness로 본다.
- 현재 validation harness는 기능 검증이 끝날 때까지 유지한다.
- UI redesign, Korean localization, resource extraction, wireframe port는 core feature validation 완료 후로 defer한다.

## D. Rationale

- Current UI is console-like / validation-tool-like.
- That is acceptable temporarily because current priority is function correctness.
- Reworking UI now would obscure whether future failures come from logic, storage, runtime root, or presentation changes.
- Korean localization should not be done by direct string replacement.
- Resource extraction should be designed and implemented after feature boundaries stabilize.
- Wireframe port should be treated as a separate product UI phase, not mixed with storage/runtime validation.

## E. UI Freeze Scope

Until core feature validation is complete, freeze these:

- MainWindow visual redesign
- product UI shell implementation
- wireframe port
- Korean label replacement
- resource dictionary or `.resx` implementation
- styling/theme work
- screen navigation redesign
- UX copy polish

Allowed only by separate explicit approval:

- minimal validation-only diagnostic text required for feature verification
- temporary status output needed for runtime validation
- documentation-only UI gap review

## F. Current MainWindow Classification

- Current MainWindow role: validation harness
- Not classified as: production product UI

Current validation harness may remain visually rough.

It should not be polished until core feature validation is complete.

It should not be treated as the final wireframe-based UI.

## G. Deferred UI Work

Deferred until after core feature validation:

1. Korean localization design.
2. Resource key inventory.
3. `.resx` or `ResourceDictionary` decision.
4. ViewModel message provider decision.
5. Wireframe-to-WPF screen mapping.
6. Product shell layout.
7. Home/dashboard screen.
8. Document box screen.
9. Policy document registration screen.
10. Claim document registration screen.
11. Management screens.
12. History/detail screens.

## H. Recommended Near-Term Sequence

Recommended next sequence:

1. RuntimeRootProvider / isolated runtime override implementation planning.
2. RuntimeRootProvider / isolated runtime override implementation after separate approval.
3. Tests for isolated runtime root after separate approval.
4. Synthetic-only runtime validation using isolated root.
5. Complete remaining core feature validation.
6. Then start Korean resource extraction plan.
7. Then start wireframe product UI port.

## I. Non-Execution Confirmations

| Item | Result |
|---|---|
| code modification | none |
| XAML modification | none |
| ViewModel modification | none |
| test modification | none |
| resource file creation | none |
| localization implementation | none |
| wireframe port | none |
| app launch | not run |
| OpenFileDialog | not run |
| document registration workflow | not run |
| cleanup execution | not run |
| runtime metadata deletion | not run |
| runtime attachment deletion | not run |
| RuntimeRootProvider implementation | none |
| isolated runtime override implementation | none |
| DB/SQLite/OCR/repository implementation | none |
| commit | not run |

## J. Decision Judgment

```text
POLICY_CLAIM_UI_REDESIGN_DEFER_DECISION_RECORDED
```

Meaning:

- UI redesign is intentionally deferred.
- Current validation UI remains acceptable for feature verification.
- Product UI work must wait until core feature validation is complete.
- Korean localization and resource extraction must not be started as incidental edits.
- Wireframe port must be a separate later phase.

## K. Next Recommended Work

1. Commit `docs/177~178` if validation passes.
2. Continue runtime metadata cleanup `DEFER`.
3. Continue runtime attachment cleanup `DEFER`.
4. Proceed to RuntimeRootProvider / isolated runtime override implementation planning docs.
5. Do not start UI implementation until core feature validation is complete.
