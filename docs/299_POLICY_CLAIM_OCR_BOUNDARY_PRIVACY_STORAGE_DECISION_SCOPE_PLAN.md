# OCR Boundary Privacy Storage Decision Scope Plan

## A. Status

OCR_BOUNDARY_PRIVACY_STORAGE_DECISION_SCOPE_PLAN_ONLY

## B. Marker

POLICY_CLAIM_OCR_BOUNDARY_PRIVACY_STORAGE_DECISION_SCOPE_READY

## C. 기준 commit

`23d417b docs(familyclaimref): plan repository boundary decision`

## D. 목적

OCR boundary, privacy, storage, retention, test strategy를 구현 없이 정리한다.

이 문서는 OCR boundary/privacy/storage decision planning 문서다. OCR implementation 문서가 아니며, OCR package/API/provider addition 또는 OCR artifact storage 승인 문서가 아니다.

## E. Current Baseline

| 항목 | 현재 기준 |
|---|---|
| latest known full test | PASS 331 |
| current storage baseline | JSON source of truth 유지 |
| repository boundary implementation | 승인 없음 |
| SQLite implementation | 승인 없음 |
| OCR implementation | 승인 없음 |
| OCR raw text/candidate storage | 승인 없음 |
| `data/claimdoc` | protected / no operational use |
| cleanup execution | 승인 없음 |
| diagnostic summary extraction | 승인 없음 |
| UI redesign/product UI shell | 승인 없음 |

## F. Read-Only Inspection Summary

| 대상 | 확인 결과 |
|---|---|
| 기준 문서 | OCR planning은 가능하지만 implementation은 승인되지 않은 gate로 기록되어 있다. |
| `app/tests` OCR search | `OCR`, `Optical`, `Tesseract`, `Image`, `Pdf`, `raw text`, `confidence`, `privacy`, `retention` 구현 match 없음 |
| `app/tests` PDF search | extension/file-name 관련 test에서 `PDF` 문자열만 확인된다. |
| OCR package/provider | app/tests 기준 implementation 또는 package usage 확인 없음 |
| OCR raw text/candidate storage | app/tests 기준 implementation 확인 없음 |
| image/PDF parsing | app/tests 기준 implementation 확인 없음 |
| `data/claimdoc` | protected local real-document artifact이며 OCR input으로 사용하지 않는다. |

## G. 포함 후보

- OCR in/out of MVP decision
- OCR input artifact boundary
- raw OCR text retention policy
- OCR candidate value storage policy
- confidence score storage policy
- privacy masking/redaction policy
- synthetic-only fixture policy
- future provider boundary
- future test strategy

## H. 제외 범위

- OCR implementation
- OCR package/API/provider addition
- OCR raw text storage implementation
- OCR candidate snapshot storage implementation
- image/PDF parsing implementation
- DB/SQLite/repository/migration implementation
- `data/claimdoc` access
- real document sample use
- cleanup execution
- product UI shell

## I. Scope Judgment

- OCR planning only
- implementation remains blocked
- current recommendation is to keep OCR out of MVP implementation until privacy/storage boundaries are approved
- raw OCR text persistence is not a default behavior
- OCR candidate snapshot persistence requires separate retention, masking, and user confirmation decisions

