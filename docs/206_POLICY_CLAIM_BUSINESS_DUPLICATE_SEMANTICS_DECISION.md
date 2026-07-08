# Policy/Claim Business Duplicate Semantics Decision

## A. Status

Status: DECISION_ONLY

Marker:

POLICY_CLAIM_BUSINESS_DUPLICATE_SEMANTICS_DECISION_RECORDED

No code is modified by this document.

No test is implemented by this document.

No business duplicate rejection rule is implemented by this document.

## B. Baseline

Record:

- latest commit:
  `4d215d4 test(familyclaimref): validate attachment duplicate collision paths`
- source docs reviewed:
  - `docs/201_POLICY_CLAIM_DOCUMENT_REGISTRATION_NEGATIVE_VALIDATION_RESULT_REVIEW.md`
  - `docs/202_POLICY_CLAIM_ATTACHMENT_DUPLICATE_COLLISION_VALIDATION_PLAN.md`
  - `docs/203_POLICY_CLAIM_ATTACHMENT_DUPLICATE_COLLISION_SCOPE_REVIEW.md`
  - `docs/205_POLICY_CLAIM_ATTACHMENT_DUPLICATE_COLLISION_VALIDATION_RESULT_REVIEW.md`

## C. Problem

Record:

- Physical filename collision is already validated.
- Duplicate-index retry is already validated.
- Duplicate active policy/claim link rejection is already validated.
- Business duplicate semantics are not defined.
- Business duplicate is a product rule question, not a storage safety question.
- Current workflow allows repeated registration when each registration creates a distinct document id and physical filename.
- Core validation must not invent product semantics.

## D. Definitions

Record:

1. Physical filename collision:
   same generated destination filename would collide with an existing attachment file.
2. Duplicate-index retry:
   storage increments duplicate index to avoid overwrite.
3. Duplicate active link:
   same target/document id active link already exists.
4. Business duplicate:
   repeated registration of the same source file, or same target + document type + display title, as a new document identity.

## E. Decision Options

### Option A: Reject repeated same source file at workflow level

Assessment:

- Not selected now.
- Requires product semantics.
- May block legitimate re-upload or corrected scans.
- Needs UX copy and localization later.

### Option B: Allow repeated registration as distinct document records

Assessment:

- Selected as current core validation baseline.
- Matches current behavior.
- Safe because attachment collision and duplicate active link risks are already validated.
- Does not imply final product UX.

### Option C: Warn-only at product UI layer later

Assessment:

- Candidate for future product UI phase.
- Requires Korean copy/resource extraction.
- Must wait until UI redesign/localization phase.

### Option D: Add future BusinessDuplicatePolicyService

Assessment:

- Deferred.
- Requires separate design and product rule approval.
- Not required for current core validation.

## F. Current Decision

Record:

- Current core validation baseline:
  Option B.
- Meaning:
  repeated registration may create distinct document metadata, distinct attachment files, and distinct links, as long as each document id is different and storage safety rules pass.
- Not decided:
  final product UX warning/rejection policy.
- Deferred:
  - UI warning/copy/localization.
  - BusinessDuplicatePolicyService.
  - Same-source-file product duplicate rule.
  - Same target + document type + display title product duplicate rule.

## G. Guardrails

Record:

- Do not implement business duplicate rejection now.
- Do not modify workflow now.
- Do not modify FileNamePolicyService now.
- Do not modify allowlists now.
- Do not modify UI/ViewModel now.
- Do not add Korean copy now.
- Do not treat repeated registration as a bug in current core validation.

## H. Decision Judgment

POLICY_CLAIM_BUSINESS_DUPLICATE_SEMANTICS_DECISION_READY
