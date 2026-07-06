# Policy / Claim Scenario 8A Retry Policy Decision

## A. Status Marker

POLICY_CLAIM_SCENARIO8A_RETRY_POLICY_DECISION_RECORDED

## B. Decision Context

Scenario 8A policy target synthetic document registration은 실행되었다.

확인 결과:

- app launch는 PASS였다.
- MainWindow 표시는 PASS였다.
- temp synthetic document 생성은 PASS였다.
- runtime synthetic policy 생성은 PASS였다.
- OpenFileDialog 실행은 PASS였다.
- 승인 파일 선택은 PASS였다.
- policy target selection은 PASS였다.
- document registration workflow는 실행되었다.
- registration result는 BLOCKED였다.
- UI status는 `문서 등록에 실패했습니다.`였다.
- likely cause는 승인 파일이 `.txt`였고, 현재 `FileNamePolicyService` allowlist가 `pdf`, `jpg`, `jpeg`, `png`만 허용한다는 점이다.
- no retry was performed.
- `documents.json`과 `policy-documents.json`은 unchanged였다.
- copied attachment는 생성되지 않았다.
- project root `attachments/`와 `data/local`은 files=0으로 clean이었다.
- runtime `policies.json`과 temp `runtime_test_document.txt`가 remaining artifact로 남아 있다.

## C. Current Runtime State After Blocked Scenario 8A

docs/147 기준 current state:

- `%LOCALAPPDATA%\FamilyClaimRef` exists
- `policies.json` exists
- `policies.json` contains one active synthetic policy:
  - displayTitle: `policy_title_scenario8_demo`
  - disabledAt: null
- `documents.json` unchanged
- `policy-documents.json` unchanged
- `claims.json` missing
- `claim-documents.json` missing
- no new copied attachment
- `%TEMP%\FamilyClaimRef\runtime_test_document.txt` exists
- project root `attachments/`: files=0
- project root `data/local`: files=0
- project root `runtime_test_document.txt` missing
- DB/SQLite unexpected file: none
- actual personal sample: none

주의:

- `policies.json`은 Scenario 8A retry에서 reuse할 수도 있지만, retry 전 snapshot으로 반드시 확인해야 한다.
- temp `.txt` file은 evidence로 남아 있으며 cleanup은 아직 승인되지 않았다.

## D. Cause Assessment

docs/147의 likely cause:

- selected file: `%TEMP%\FamilyClaimRef\runtime_test_document.txt`
- observed UI status: `문서 등록에 실패했습니다.`
- registration did not create a new `DocumentRecord`.
- registration did not create a new `PolicyDocumentRecord`.
- no copied attachment was created.

Source check:

- `FileNamePolicyService` allowlist is:
  - `pdf`
  - `jpg`
  - `jpeg`
  - `png`

Assessment:

- `.txt` failure is expected policy rejection under current file extension policy.
- The blocked result is consistent with current file extension policy.
- This is not evidence that policy target linking is broken.
- Scenario 8A success path remains unverified because registration did not reach metadata/link/copy success.

UX note:

- UI showed only generic failure message: `문서 등록에 실패했습니다.`
- Detailed cause was not surfaced to the user.
- Extension rejection detail is a candidate for later UI/error-message hardening.

## E. Decision Options

### Option A: Accept `.txt` Blocked Result As Final Scenario 8A Evidence

설명:

- `.txt` failure를 file policy validation evidence로 인정하고 retry하지 않는다.
- Scenario 8A success path는 미검증 상태로 남긴다.

장점:

- 추가 runtime artifact를 만들지 않는다.
- cleanup 범위를 줄일 수 있다.
- 현재 failure 원인이 합리적으로 설명된다.

단점:

- copied attachment, `documents.json` update, `policy-documents.json` update 성공 경로는 검증되지 않는다.
- Scenario 8A 목적을 완전히 달성하지 못한다.

판정 후보:

- 보수적이지만 기능 검증을 닫기에는 부족하다.

### Option B: Retry Scenario 8A With Allowed Extension Synthetic PNG

설명:

- allowed extension인 `.png` synthetic file을 temp 경로에 생성해 policy target registration을 재시도한다.

Recommended synthetic path:

```text
%TEMP%\FamilyClaimRef\runtime_test_document.png
```

Recommended file type:

- minimal valid PNG binary
- no embedded personal/insurance/hospital/diagnosis text

장점:

- 현재 allowlist를 변경하지 않는다.
- source tree를 오염시키지 않는다.
- successful document copy / `documents.json` / `policy-documents.json` path를 검증할 수 있다.
- `.txt` failure와 success path를 분리해 검증할 수 있다.

단점:

- 추가 runtime artifacts가 생성된다.
- 기존 `policies.json` active policy를 reuse할지 새 policy를 만들지 결정해야 한다.
- temp `.txt`와 temp `.png` cleanup 정책이 필요해진다.

추천:

- Option B를 1차 추천한다.

### Option C: Retry Scenario 8A With Allowed Extension Synthetic PDF

설명:

- allowed extension인 `.pdf` synthetic file을 temp 경로에 생성해 재시도한다.

장점:

- document registration 현실성은 PNG보다 높을 수 있다.
- allowlist 변경이 필요 없다.

단점:

- valid minimal PDF 생성이 PNG보다 더 까다롭다.
- PDF 내부 text가 privacy scan에서 혼선을 만들 수 있다.
- binary/source evidence 관리가 더 복잡하다.

판정 후보:

- 가능하지만 PNG보다 우선순위 낮음.

### Option D: Change FileNamePolicyService To Allow `.txt`

설명:

- `.txt`를 allowlist에 추가하고 retry한다.

장점:

- 기존 approved synthetic text file을 그대로 쓸 수 있다.

단점:

- runtime validation을 위해 production policy를 바꾸는 꼴이 된다.
- 문서 파일 허용 정책이 넓어져 별도 설계와 테스트가 필요하다.
- 이번 Scenario 8A retry 범위를 벗어난다.

판정:

- reject for Scenario 8A retry.

### Option E: Add UI/Error Message Hardening Before Retry

설명:

- generic failure 대신 extension rejection cause를 사용자에게 보여주도록 개선한 뒤 재시도한다.

장점:

- 실제 UX 개선 효과가 있다.
- 이번 failure의 user-facing 문제를 해결한다.

단점:

- 코드 수정이 필요하다.
- Scenario 8A success path 검증과 UX hardening이 섞인다.
- 별도 implementation plan/review/commit이 필요하다.

판정 후보:

- follow-up hardening으로 분리한다.
- retry policy 자체를 막지는 않는다.

## F. Recommended Decision

Recommended:

- Option B, allowed extension synthetic PNG로 Scenario 8A를 retry한다.
- `FileNamePolicyService`는 수정하지 않는다.
- `.txt` failure는 valid file extension policy rejection evidence로 기록한다.
- UI/error message hardening은 후속으로 둔다.
- Scenario 8B claim target registration은 여전히 별도 후보로 둔다.

## G. Retry Scope Recommendation

Scenario 8A retry scope:

- policy target only
- claim target registration 없음
- no Scenario 8B
- no code changes
- no allowlist changes
- no cleanup before retry unless separately approved
- use allowed extension synthetic file
- OpenFileDialog에서 approved `.png` file only 선택
- registration success path 검증

Recommended approved retry file:

```text
%TEMP%\FamilyClaimRef\runtime_test_document.png
```

Recommended retry policy:

- reuse existing active runtime policy if `policies.json` contains exactly one active policy with displayTitle `policy_title_scenario8_demo`
- if no active policy exists, create `policy_title_scenario8_retry_demo` during retry execution
- do not create claim
- do not cleanup before retry
- cleanup needed 여부는 retry result review에 기록

## H. Synthetic PNG Policy

Decision candidate:

Allowed:

- minimal valid PNG binary
- no real personal/insurance/hospital/diagnosis content
- no actual screenshot
- no actual document image
- no generated image with personal/medical/insurance context

Recommended:

- use a tiny synthetic PNG fixture generated by script during execution instruction
- file name:

```text
runtime_test_document.png
```

- path:

```text
%TEMP%\FamilyClaimRef\runtime_test_document.png
```

주의:

- 이 decision 문서 생성 중에는 PNG를 만들지 않는다.
- retry execution instruction에서만 생성한다.
- project root에 PNG를 만들지 않는다.

## I. Existing `.txt` Artifact Policy

현재 temp `.txt` artifact:

```text
%TEMP%\FamilyClaimRef\runtime_test_document.txt
```

Decision:

- retry decision 단계에서는 cleanup하지 않는다.
- retry execution 중에도 `.txt` cleanup은 하지 않는다.
- retry result review에서 cleanup needed로 기록한다.
- temp cleanup은 별도 cleanup decision/instruction 후에만 수행한다.

## J. Runtime Policy Artifact Policy

현재 runtime `policies.json`:

- exists
- contains active synthetic policy created by blocked Scenario 8A

Decision:

- retry execution may reuse this policy if sanity check passes.
- if reuse is not safe, retry execution may create a new synthetic policy title:

```text
policy_title_scenario8_retry_demo
```

- retry execution must record which path was used.
- cleanup is not performed during retry.

## K. Expected Retry Artifacts

If Option B retry succeeds:

- `%TEMP%\FamilyClaimRef\runtime_test_document.png` exists
- `policies.json` exists
- `documents.json` updated with new document record
- `policy-documents.json` updated with new link record
- copied attachment created under `%LOCALAPPDATA%\FamilyClaimRef\attachments\documents`
- project root `attachments/`: files=0
- project root `data/local`: files=0
- DB/SQLite unexpected file: none
- actual personal sample: none

If retry fails:

- result review records failure point
- no cleanup
- project root remains clean

## L. Stop Criteria For Retry

Confirmed retry stop criteria:

- unexpected source tree change
- build/test failure not attributable to known Windows SDK permission issue
- project root `attachments/` files > 0
- project root `data/local` files > 0
- project root `runtime_test_document.*` created
- temp PNG cannot be created
- temp PNG invalid or not at approved path
- app startup crash
- policy target unavailable and policy creation fails
- OpenFileDialog selects anything except approved temp PNG
- document registration fails again
- copied attachment created under project root
- metadata created under project root
- DB/SQLite unexpected file created
- actual personal sample detected

## M. Explicit Non-Scope

이번 decision 문서 생성에서 하지 않는 항목:

- app launch 없음
- OpenFileDialog 없음
- retry 실행 없음
- PNG 생성 없음
- PDF 생성 없음
- `.txt` cleanup 없음
- runtime artifact cleanup 없음
- code 수정 없음
- `FileNamePolicyService` 수정 없음
- allowlist 변경 없음
- tests 수정 없음
- DB/SQLite/OCR/repository 구현 없음
- git add/commit/reset/checkout/clean 없음

## N. Next Recommendation

다음 추천 작업:

```text
Scenario 8A allowed-extension retry execution instruction 문서 생성
```

예상 문서:

```text
docs/149_POLICY_CLAIM_SCENARIO8A_ALLOWED_EXTENSION_RETRY_EXECUTION_INSTRUCTION.md
```

검증:

- `git diff --check`
- `git status --short`
- project root `attachments/` files count
- project root `data/local` files count
- project root `runtime_test_document.*` absence

build/test:

- documentation-only change이므로 실행하지 않는다.
