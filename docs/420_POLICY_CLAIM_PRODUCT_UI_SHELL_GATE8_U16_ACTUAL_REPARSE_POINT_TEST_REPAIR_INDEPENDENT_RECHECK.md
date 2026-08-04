# Gate 8 U16 Actual Reparse-Point Test Repair Independent Recheck

## A. Status

- Marker:
  `POLICY_CLAIM_PRODUCT_UI_SHELL_GATE8_U16_ACTUAL_REPARSE_POINT_TEST_REPAIR_INDEPENDENT_RECHECK_PASS_RUNTIME_REVIEW_PENDING`
- Judgment: `PASS`
- Findings: Blocking `0`, Major `0`, Minor `0`
- Independent repair recheck: `PASS`
- Final Gate 8 implementation: `HOLD_RUNTIME_REVIEW_REQUIRED`
- Runtime review: `NOT_EXECUTED_NOT_AUTHORIZED_IN_THIS_BATCH`
- Deployment/production readiness: `NOT_AUTHORIZED`
- Stage/commit: `NOT_AUTHORIZED`

## B. Reviewer Role and Independence Boundary

This review independently reconstructed the U16 and production validator
contracts from source and independently executed the authorized build and test
commands. The PASS text and historical test counts in `docs/419` were not used
as execution evidence for this recheck.

Reviewed production source, tests, and `docs/413~419` remained read-only.
The only repository content created by this batch is this result document.
No runtime UI, file picker, UIA, screenshot, deployment, or Git mutation was
performed.

## C. Baseline and Exact Scope

| Item | Result |
|---|---|
| Project | `C:\EtcProject\FamilyClaimRef` |
| Branch | `main` |
| HEAD | `aecf7edfd43b4124ec5ff17d35687020cf4c0d90` |
| Subject | `docs(familyclaimref): record gate8 registration persistence decision package` |
| Parent | `79d8f1d5b76b22b0ef1a65fbf781c833bbcd7fff` |
| Starting tracked/staged/untracked | `27/0/8` |
| Starting status entries | `35` |
| Existing Gate 8 path set | `35/35` exact equality |
| Missing/extra | `0/0` |
| Starting Gate 8 TEMP roots/files | `0/0` |
| `docs/420` before review | absent |

## D. Baseline SHA-256

| File | SHA-256 |
|---|---|
| `tests/FamilyClaimRef.App.Tests/DocumentFileValidationServiceTests.cs` | `ec55a7e3d1ebc9e8f5625ed628ea90914057d3fe8bab08a2772047ac8ff37431` |
| `docs/419_POLICY_CLAIM_PRODUCT_UI_SHELL_GATE8_REAL_DOCUMENT_REGISTRATION_IMPLEMENTATION_RESULT_REVIEW.md` | `b81b76fe43bef81142db1beb30c930b939773993a26a3a83d24a500d97a73506` |
| `docs/413` | `8f8a5717085ea3f08745e3ae16b8226897af0b127bfff55fbba6fc595650dabd` |
| `docs/414` | `522d1e9518cf2d4314f9cf3214d57d22be06c4f3b8b0f77fddf1cd4044c0141f` |
| `docs/415` | `04db1ba9dbb606a8ed2c429c447834294f2a407ee0d2714bb8369d0274e7727a` |
| `docs/416` | `e62e2cc9cb49b8fe090db49f608ef0c3ed76014bc336ea986a1a321b58b58b28` |
| `docs/417` | `2b6ff910b6699f8fcdb38344494472f34ed9c942a9916b8f0972a3dcbf6488c1` |
| `docs/418` | `e458f808079d07f8418072f31304ba10b74d28b84dcd4d30a4ffc326783c6363` |

## E. Independent Static Reconstruction

### E1. U16 executable contract

Direct source inspection established the following executable path:

1. `UsingGate8RootAsync` creates one unique test-owned root under the logical
   `%TEMP%\FamilyClaimRef\Gate8\gate8-validation-{actual-guid}` boundary.
2. `WriteBytesAsync` creates a synthetic PDF regular target.
3. `File.GetAttributes(targetPath)` must succeed and the test asserts that the
   target does not have `FileAttributes.ReparsePoint`.
4. `File.CreateSymbolicLink(linkPath, targetPath)` directly creates a file
   symbolic link.
5. The test asserts that the link exists and has
   `FileAttributes.ReparsePoint`.
6. `File.ResolveLinkTarget(linkPath, false)` must return a non-null target
   whose full path equals the synthetic target.
7. The returned link object must identify the expected link path.
8. The exact `linkPath`, not `targetPath`, is passed to
   `DocumentFileValidationService.ValidateSourceAsync`.
9. Validation success is prohibited by requiring a
   `DocumentRegistrationException`.
10. The exception must classify the result as
    `DocumentRegistrationErrorCode.SourceUnavailable`.
11. The exception message must not contain the test-owned TEMP root.
12. No runtime storage directory may be created.
13. The `finally` block deletes only the exact unique test-owned root.

The valid PDF target creation plus successful `File.GetAttributes` call is
executable existence evidence; a missing target could not reach the following
attribute assertion or symbolic-link call.

### E2. Production rejection contract

`DocumentFileValidationService.ValidateFileAsync` refreshes `FileInfo` for the
actual input path and rejects the input before content reading when:

- the file does not exist;
- the path identifies a directory; or
- `file.Attributes` contains `FileAttributes.ReparsePoint`.

The reparse branch throws `DocumentRegistrationException` with
`DocumentRegistrationErrorCode.SourceUnavailable` and the safe message
`Source must be an existing regular file.` It does not retain the raw path or
an inner filesystem exception.

The separate presentation boundary maps `SourceUnavailable` to
`Ui.Product.DocumentRegistration.Validation.SourceUnavailable`. U16 directly
asserts the storage error code; the resource-key mapping was reconstructed
from `DocumentRegistrationViewModel` and was not misrepresented as part of
the exact U16 execution.

### E3. Forbidden substitutes

| Substitute | Count |
|---|---:|
| Production source string search | 0 |
| Reflection-only guard | 0 |
| Mock file attributes/filesystem | 0 |
| Regular file treated as reparse | 0 |
| Hard link | 0 |
| Directory symlink substitute | 0 |
| Conditional skip or `Skip` attribute | 0 |
| Platform return | 0 |
| Catch-and-pass | 0 |
| Privilege failure converted to PASS | 0 |
| Assertion-free early return | 0 |
| External PowerShell output used as PASS evidence | 0 |

## F. Raw Environment Observations

These fields are recorded separately and are not treated as the capability
proof:

| Field | Observation |
|---|---|
| Registry product-name field | `Windows 10 Pro` |
| Registry display-version field | `25H2` |
| Registry build field | `26200` |
| Registry UBR field | `8875` |
| Process elevation | non-elevated |
| `SeCreateSymbolicLinkPrivilege` | not listed in the process token |
| Developer Mode observation | `AllowDevelopmentWithoutDevLicense=1` |
| TEMP volume | `C:` |
| TEMP filesystem | `NTFS` |
| Batch-internal persistent environment mutation | `0` |

The actual capability proof is the independently executed U16 test reaching
and passing `File.CreateSymbolicLink`, the reparse attribute assertion,
`ResolveLinkTarget`, and the production rejection assertion.

## G. Independent Execution Results

| Execution | Passed | Failed | Skipped | Total |
|---|---:|---:|---:|---:|
| Solution build warnings/errors | 0 | 0 | n/a | n/a |
| Exact U16 method | 1 | 0 | 0 | 1 |
| `DocumentFileValidationServiceTests` | 9 | 0 | 0 | 9 |
| New Gate 8 three suites | 37 | 0 | 0 | 37 |

The exact U16 execution independently proved:

- regular target creation and non-reparse state: PASS;
- actual `File.CreateSymbolicLink`: PASS;
- link `FileAttributes.ReparsePoint`: PASS;
- `File.ResolveLinkTarget`: PASS;
- exact link-path delivery to the production validator: PASS;
- structured `SourceUnavailable` rejection: PASS;
- raw TEMP path non-exposure: PASS;
- runtime/storage side-effect absence: PASS;
- exact test-owned root cleanup: PASS.

The modified existing eight-suite `199/199` and full solution `486/486`
results were intentionally not rerun in this independent recheck. They remain
repair automated-validation evidence recorded in `docs/419`, not independent
execution evidence in this document.

## H. TEMP, Side Effects, and Protected State

| Check | Result |
|---|---|
| Gate 8 TEMP roots/files after build | `0/0` |
| Gate 8 TEMP roots/files after exact U16 | `0/0` |
| Gate 8 TEMP roots/files after nine-test suite | `0/0` |
| Gate 8 TEMP roots/files after 37-test suites | `0/0` |
| Cleanup outside test-owned root | `0` |
| Staging residue | `0` |
| Project attachments files | `0` |
| Project `data/local` files | `0` |
| Project `runtime_test_document.*` | `0` |
| Project-root payload artifacts | `0` |
| Production runtime root access/deletion | `0/0` |
| `data/claimdoc` access | `0` |
| Actual user-file use | `0` |
| Actual personal/insurance/medical sample use | `0` |
| Persistent environment mutation | `0` |
| Existing 35-file content delta | `0` |
| Production/test delta | `0/0` |
| `docs/413~419` modification | `0` |
| Protected-file delta | `0` |

Actual GUID values, account identifiers, profile paths, and expanded TEMP
paths are intentionally not retained.

## I. Git and Document Quality Gates

- `git diff --check`: PASS
- Existing line-ending conversion warnings: informational only
- Merge markers: `0`
- Temporary work markers: `0`
- Trailing whitespace: `0`
- Staged files: `0`
- Git stage/commit/push: `0/0/0`
- App/file picker/UIA/screenshot/runtime review: `0/0/0/0/0`

## J. Final Judgment

All independent PASS conditions for the U16 actual reparse-point repair are
satisfied.

- Blocking: `0`
- Major: `0`
- Minor: `0`
- Independent repair recheck: `PASS`
- Final Gate 8 implementation: `HOLD_RUNTIME_REVIEW_REQUIRED`
- Runtime review: `NOT_EXECUTED_NOT_AUTHORIZED_IN_THIS_BATCH`
- Deployment/production readiness: `NOT_AUTHORIZED`
- Stage/commit: `NOT_AUTHORIZED`

Final marker:

`POLICY_CLAIM_PRODUCT_UI_SHELL_GATE8_U16_ACTUAL_REPARSE_POINT_TEST_REPAIR_INDEPENDENT_RECHECK_PASS_RUNTIME_REVIEW_PENDING`

## K. Next Recommendation

Prepare a guarded runtime UIA/manual visual review instruction in a separately
approved batch.
