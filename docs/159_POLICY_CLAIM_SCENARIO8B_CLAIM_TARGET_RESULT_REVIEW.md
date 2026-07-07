# Policy / Claim Scenario 8B Claim Target Result Review

## A. Status Marker

POLICY_CLAIM_SCENARIO8B_CLAIM_TARGET_EXECUTED

## B. Execution Scope

- Scenario: 8B claim target synthetic PNG registration
- Approved synthetic source file: `%TEMP%\FamilyClaimRef\runtime_test_document_claim.png`
- Approved synthetic policy title: `policy_title_scenario8b_demo`
- Approved synthetic claim title: `claim_title_scenario8b_demo`
- Approved document display title: `scenario8b_claim_document_png_demo`
- Runtime execution target: WPF `FamilyClaimRef.App`
- Result document scope: runtime result review only

## C. Explicit Non-Scope

- Scenario 8A repeat 없음
- policy target registration을 primary goal로 수행하지 않음
- 실제 개인정보, 보험사명, 병원명, 진단명, 진단코드, 실제 약관/계약/청구 문서 사용 없음
- `data/claimdoc` 파일 열람, 목록화, 사용, 선택, stage, commit, 삭제, 이동 없음
- `FileNamePolicyService` 수정 없음
- allowlist 변경 없음
- cleanup 없음
- `%LOCALAPPDATA%\FamilyClaimRef` 삭제 없음
- project root cleanup 없음
- code/XAML/ViewModel/test 수정 없음
- DB/SQLite/OCR/repository 구현 없음
- git add/commit/reset/checkout/clean 없음

## D. Runtime Input

| 항목 | 값 |
|---|---|
| source file | `%TEMP%\FamilyClaimRef\runtime_test_document_claim.png` |
| source extension | `png` |
| source file size | `68` bytes |
| policy title | `policy_title_scenario8b_demo` |
| claim title | `claim_title_scenario8b_demo` |
| target kind | `claim` |
| document type | `etc` |
| display title | `scenario8b_claim_document_png_demo` |
| reference date | `2026-07-07` |

## E. Execution Steps

1. WPF app launch 수행.
2. fresh synthetic policy 생성.
3. fresh synthetic claim 생성.
4. target kind를 `claim`으로 선택.
5. claim target을 `claim_title_scenario8b_demo`로 선택.
6. OpenFileDialog를 실행.
7. 승인된 `%TEMP%\FamilyClaimRef\runtime_test_document_claim.png`만 선택.
8. document display title을 `scenario8b_claim_document_png_demo`로 입력.
9. 초기 시도에서 document type `capture`로 등록을 시도했으나 실패.
10. source allowlist 기준 검토 결과, `capture`는 policy scope 문서 유형이고 claim scope 허용 유형이 아님을 확인.
11. 코드 또는 allowlist 변경 없이 claim scope 허용 문서 유형인 `etc`로 변경.
12. reference date `2026-07-07` 입력.
13. claim target document registration workflow 재실행.
14. 등록 성공 메시지와 last registration summary 확인.

## F. Created Runtime Records

| 객체 | 확인값 |
|---|---|
| Policy | `policy_55816736e1634402a7ca0ce31d819d13` |
| Claim | `claim_74868dcd8717402dbe9db19492c5a13b` |
| Document | `doc_d5266cad2e6345d4bdb7c10a09cbb9f6` |
| ClaimDocument link | `claim_74868dcd8717402dbe9db19492c5a13b` + `doc_d5266cad2e6345d4bdb7c10a09cbb9f6` |

## G. UI Result

- Status message: `문서 등록이 완료되었습니다.`
- Last registration summary: `claim:claim_74868dcd8717402dbe9db19492c5a13b; document:doc_d5266cad2e6345d4bdb7c10a09cbb9f6`
- Initial failed status was observed before correction: `문서 등록에 실패했습니다.`
- Final result after claim-compatible `etc` document type and reference date input: success

## H. Runtime Metadata Sanity Check

| 확인 항목 | 결과 |
|---|---|
| `%LOCALAPPDATA%\FamilyClaimRef\data\local\policies.json` | exists |
| `%LOCALAPPDATA%\FamilyClaimRef\data\local\claims.json` | exists |
| `%LOCALAPPDATA%\FamilyClaimRef\data\local\documents.json` | exists |
| `%LOCALAPPDATA%\FamilyClaimRef\data\local\policy-documents.json` | exists |
| `%LOCALAPPDATA%\FamilyClaimRef\data\local\claim-documents.json` | exists |
| synthetic policy found | true |
| synthetic claim found | true |
| scenario8B document found | true |
| claim document link found | true |
| policy link to scenario8B document | false |

Runtime counts:

| 파일 | count |
|---|---:|
| policies | 2 |
| claims | 1 |
| documents | 2 |
| policy-documents | 1 |
| claim-documents | 1 |

## I. Copied Attachment Sanity Check

| 항목 | 값 |
|---|---|
| physicalFileName | `claim-document_20260707_etc_001.png` |
| relativePath | `documents/claim-document_20260707_etc_001.png` |
| extension | `png` |
| copied attachment exists | true |
| copied attachment size | `68` bytes |

## J. Project Root Safety Check

| 항목 | 결과 |
|---|---|
| project root `attachments/` file count | 0 |
| project root `data/local/` file count | 0 |
| project root `runtime_test_document.*` file count | 0 |
| DB/SQLite unexpected file count | 0 |
| `data/claimdoc/` ignore rule | `.gitignore:6:/data/claimdoc/` |

## K. Git / Diff Check

- `git diff --check`: PASS
- `git status --short` before result document creation: clean
- after this document creation, expected untracked item: `docs/159_POLICY_CLAIM_SCENARIO8B_CLAIM_TARGET_RESULT_REVIEW.md`
- git add/commit/reset/checkout/clean: not run

## L. Privacy Check

- 실제 개인정보 샘플 사용 없음
- 실제 가족 실명 사용 없음
- 실제 보험사명 사용 없음
- 실제 병원명 사용 없음
- 실제 진단명/진단코드 기반 개인 사례 사용 없음
- synthetic-only values:
  - `policy_title_scenario8b_demo`
  - `claim_title_scenario8b_demo`
  - `scenario8b_claim_document_png_demo`

## M. Remaining Risks

- runtime artifacts remain under `%LOCALAPPDATA%\FamilyClaimRef` and `%TEMP%\FamilyClaimRef`.
- cleanup was explicitly not approved in this run.
- Scenario 8A artifacts still exist in runtime metadata.
- initial `capture` selection failure is recorded as execution evidence; final successful registration uses claim-compatible `etc`.

## N. Next Recommendation

1. 별도 승인 후 Scenario 8B runtime artifact cleanup scope를 결정한다.
2. `docs/159_POLICY_CLAIM_SCENARIO8B_CLAIM_TARGET_RESULT_REVIEW.md` commit candidate review를 생성한다.
3. 필요 시 claim document type UX에서 claim target 선택 시 policy-only document type을 숨기는 후속 범위를 검토한다.
