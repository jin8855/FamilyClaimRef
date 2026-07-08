# Policy/Claim Attachment Duplicate Collision Scope Review

## A. Status

Status: TEST_SCOPE_REVIEW_ONLY

## B. Read-Only Source Findings

### Confirmed

- `FileNamePolicyService` accepts `duplicateIndex` values from `1` through `999`.
- `FileNamePolicyService` rejects `duplicateIndex <= 0` and `duplicateIndex > 999`.
- `FileNamePolicyService` formats duplicate suffixes as three-digit suffixes such as `_001` and `_999`.
- `DocumentAttachmentCoordinator` starts duplicate index attempts at `1`.
- `DocumentAttachmentCoordinator` skips a generated physical filename when the same physical filename already exists in document metadata.
- `DocumentAttachmentCoordinator` skips a generated physical filename when the target attachment path already exists.
- `DocumentAttachmentCoordinator` retries the next duplicate index when final file copy fails with an existing target file before the max index.
- `DocumentAttachmentCoordinator` stops at max duplicate index `999` and throws when no duplicate index is available.
- `LocalFileAttachmentService` copies with overwrite disabled.
- `LocalFileAttachmentService` throws when a target attachment file already exists.
- `DocumentLinkCoordinator` rejects duplicate active policy links for the same target/document pair.
- `DocumentLinkCoordinator` rejects duplicate active claim links for the same target/document pair.
- `DocumentLinkCoordinator` excludes disabled links from duplicate active link checks.
- Existing tests already cover duplicate-index start, target-file collision increment, metadata physical filename collision increment, copy-time collision retry, duplicate active policy link rejection, duplicate active claim link rejection, and disabled-link exclusion.

### Candidate

- Add integration-level workflow tests that prove repeated physical filename collision produces unique attachment files under isolated runtime root.
- Add integration-level tests that prove the workflow does not overwrite an existing attachment when a collision is encountered.
- Add targeted tests for max duplicate index exhaustion if the helper setup can create the state without excessive filesystem work.
- Add workflow-level duplicate active link tests only if they can reuse an existing document/link setup without forcing new product behavior.
- Reuse lower-level coordinator tests where they already prove the intended behavior sufficiently.

### Unknown

- Business duplicate semantics are not defined.
- Current service behavior appears to allow repeated registration of the same source file as separate document metadata and separate physical attachments, as long as generated filenames differ.
- It is not yet decided whether repeated registration of the same source file should be rejected, allowed, or warned at UI level.
- It is not yet decided whether same target + document type + display title should be considered a business duplicate.
- It is not yet decided whether business duplicate validation belongs in workflow, storage, ViewModel, or a future product rule service.

## C. Risk Review

- collision and duplicate are not the same concept.
- business duplicate semantics may not yet be defined.
- current service may allow repeated registration as separate documents.
- tests must not force product semantics that are not defined.
- unsupported business duplicate should be deferred.
- max duplicate index exhaustion can be expensive if implemented by real filesystem population rather than focused test doubles.
- link duplicate tests should avoid implying a broader business duplicate policy.

## D. Recommended Strategy

- test physical filename collision and duplicate active link first.
- classify business duplicate semantics as product decision if not implemented.
- do not modify allowlists.
- do not modify UI.
- do not add localization copy.
- prefer focused service-level tests for max duplicate index exhaustion.
- use integration-level tests only for behavior that must prove actual isolated runtime attachment files are not overwritten.
