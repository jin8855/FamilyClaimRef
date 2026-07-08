# Policy/Claim Document Registration Negative Validation Plan

## A. Status

Status: TEST_PLAN_ONLY

Marker:

POLICY_CLAIM_DOCUMENT_REGISTRATION_NEGATIVE_VALIDATION_PLANNED

No code is modified by this document.

No test is implemented by this document.

No app launch is authorized by this document.

## B. Purpose

Record:
- positive document registration works under isolated runtime root.
- negative cases must be validated before product UI/localization.
- validation should be service/workflow-level first.
- UI redesign remains deferred.

## C. Candidate Negative Cases

Plan tests for:

1. missing source file path
2. nonexistent source file
3. empty display title
4. unsupported document type
5. missing target id
6. target kind mismatch
7. disabled policy target registration rejection, if supported
8. disabled claim target registration rejection, if supported
9. source file extension not allowlisted, if allowlist exists
10. duplicate registration / filename collision behavior, if applicable

## D. Safety Rules

- isolated runtime root only
- synthetic files only
- no data/claimdoc
- no real personal/institution/diagnosis data
- no app launch
- no UI automation
- no cleanup of default runtime evidence

## E. Acceptance Criteria

- tests prove rejected cases do not create metadata/link/attachments.
- success paths remain unaffected.
- project root remains clean.
- dotnet build passes.
- dotnet test passes.

## F. Planned Result Review

Future implementation batch must create:
- docs/201_POLICY_CLAIM_DOCUMENT_REGISTRATION_NEGATIVE_VALIDATION_RESULT_REVIEW.md

## G. Planning Judgment

POLICY_CLAIM_DOCUMENT_REGISTRATION_NEGATIVE_VALIDATION_PLAN_READY
