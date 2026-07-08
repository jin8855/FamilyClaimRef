# Policy/Claim Next Core Validation Sequence Decision

## A. Status

Status: DECISION_ONLY

Marker:

```text
POLICY_CLAIM_NEXT_CORE_VALIDATION_SEQUENCE_DECIDED
```

## B. Baseline

Record:

- latest commit:
  `3fd316e docs(familyclaimref): review isolated runtime manual validation`

## C. Decision

Recommended next sequence:

1. Policy/Claim lifecycle + persistence automated validation.
2. Policy/Claim lifecycle + persistence commit.
3. Document registration negative validation plan.
4. Document registration negative validation tests.
5. Isolated runtime cleanup policy execution only after separate approval.
6. UI redesign remains deferred until core validations complete.

## D. Rationale

Record:

- isolated runtime root now allows clean-room automated/manual validation.
- lifecycle and persistence are core feature risks.
- disabled target behavior must be verified before product UI work.
- negative validation must be verified before final UI copy/localization.
- UI work now would obscure functional defects.

## E. Immediate Next Work

Immediate next work after committing docs/190~193:

- create Policy/Claim lifecycle + persistence automated validation plan.

## F. Decision Judgment

```text
POLICY_CLAIM_NEXT_CORE_VALIDATION_SEQUENCE_READY
```
