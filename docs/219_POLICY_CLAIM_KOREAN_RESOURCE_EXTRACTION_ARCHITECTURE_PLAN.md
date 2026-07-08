# Policy/Claim Korean Resource Extraction Architecture Plan

## A. Status

Status: ARCHITECTURE_PLAN_ONLY

Marker:

POLICY_CLAIM_KOREAN_RESOURCE_EXTRACTION_ARCHITECTURE_PLANNED

This document plans resource extraction only.

No resource file is created by this document.

No code is modified by this document.

No XAML is modified by this document.

No Korean localization is implemented by this document.

## B. Baseline

Record:

- latest commit:
  893311f docs(familyclaimref): close core validation status review

- source docs reviewed:
  - docs/214_POLICY_CLAIM_REMAINING_PRODUCT_UI_BOUNDARY_DECISION.md
  - docs/217_POLICY_CLAIM_UI_PHASE_ENTRY_DECISION.md
  - docs/218_POLICY_CLAIM_UI_STRING_INVENTORY_REVIEW.md

## C. Architecture Options

### Option A: .resx-based strongly typed resources

Assessment:

- good for stable application strings
- testable
- common .NET pattern
- may require binding helper for WPF XAML

### Option B: WPF ResourceDictionary string resources

Assessment:

- natural for XAML
- easy static resource lookup
- less ideal for ViewModel validation/status strings

### Option C: Hybrid ResourceDictionary for XAML labels + message provider for ViewModel/status strings

Assessment:

- separates visual labels from runtime messages
- supports ViewModel unit tests
- better for validation harness and product UI separation
- recommended direction

### Option D: Direct Korean string replacement

Assessment:

- rejected
- creates maintenance debt
- blocks later localization/resource key review

## D. Recommended Direction

Recommend:

- Option C as initial architecture direction.
- XAML labels/buttons/headers should use resource keys.
- ViewModel validation/status messages should use a message provider or resource service abstraction.
- Current validation harness strings should be extracted only after resource infrastructure is approved.
- Product UI wireframe strings should be mapped after key naming rules are committed.

## E. Proposed Future Implementation Stages

Stage 1:

- create resource key inventory and key naming doc

Stage 2:

- create resource infrastructure implementation plan

Stage 3:

- implement minimal resource infrastructure

Stage 4:

- extract current validation harness labels/status strings

Stage 5:

- create wireframe-to-resource key mapping

Stage 6:

- start product shell implementation

## F. Guardrails

Record:

- no direct Korean replacement
- no UI redesign in resource infrastructure batch
- no product shell implementation before resource strategy
- no business duplicate warning copy until business duplicate UX decision
- no DB/SQLite/OCR/repository
- no data/claimdoc
- no real document/personal sample data

## G. Acceptance Criteria For Future Implementation

A future resource implementation is acceptable only if:

- XAML strings are no longer directly hardcoded for selected scope.
- ViewModel messages do not directly embed final Korean copy without resource/message provider.
- tests do not assert fragile product copy unless intentionally testing resource keys.
- validation harness remains functionally unchanged.
- no workflow/storage behavior changes.
- build/test pass.

## H. Architecture Judgment

POLICY_CLAIM_KOREAN_RESOURCE_EXTRACTION_ARCHITECTURE_READY_FOR_REVIEW
