# Policy / Claim Scenario 8 Synthetic Document Registration Decision

## A. Status Marker

POLICY_CLAIM_SCENARIO8_SYNTHETIC_DOCUMENT_REGISTRATION_DECISION_RECORDED

## B. Decision Context

Phase 3D base runtime validation Scenario 1~7은 완료되었다.

Scenario 8 synthetic document registration은 아직 실행되지 않았다.

OpenFileDialog는 아직 실행되지 않았다.

actual file selection은 수행되지 않았다.

document registration workflow 실제 실행은 아직 없다.

docs/136~144는 commit 완료되었다.

targeted cleanup으로 `policies.json`, `claims.json`은 삭제되었다.

runtime root에는 pre-existing `documents.json`, `policy-documents.json`, attachment가 남아 있다.

Scenario 8을 실행하려면 active policy/claim target이 다시 필요하다.

Scenario 8은 OpenFileDialog, synthetic file 생성, file copy, document metadata, link metadata 생성을 포함하므로 별도 승인 gate가 필요하다.

## C. Current Runtime Baseline

docs/143, docs/144 기준 current runtime state:

- `%LOCALAPPDATA%\FamilyClaimRef` exists
- `%LOCALAPPDATA%\FamilyClaimRef\data\local` exists
- `%LOCALAPPDATA%\FamilyClaimRef\attachments` exists
- `policies.json` missing
- `claims.json` missing
- `documents.json` exists
- `policy-documents.json` exists
- `claim-documents.json` missing
- known runtime attachment exists
- project root `attachments/`: files=0
- project root `data/local`: files=0
- DB/SQLite unexpected file: 없음

주의:

- runtime root는 clean-room 상태가 아니다.
- existing documents/link/attachment evidence가 남아 있다.
- Scenario 8 결과는 pre/post snapshot으로 분리해야 한다.

## D. Confirmed Decisions

### Decision 1: Scenario 8 Execution Status

Confirmed:

- Scenario 8은 아직 실행하지 않는다.
- 이 decision 문서 작성 중에는 app launch, OpenFileDialog, synthetic file 생성, document registration workflow를 실행하지 않는다.

Next:

- Scenario 8 execution instruction 문서를 별도로 작성한 뒤, 사용자가 별도 승인하면 실행한다.

### Decision 2: Scenario 8 Execution Candidate

Confirmed:

- Scenario 8 execution은 다음 단계의 후보로 둔다.
- 단, 실행은 별도 execution instruction과 explicit approval 후에만 허용한다.

Reason:

- Scenario 8은 OpenFileDialog와 actual registration workflow를 포함하므로 Scenario 1~7보다 위험도가 높다.

### Decision 3: Active Target Preparation

Confirmed:

- targeted cleanup 이후 `policies.json`, `claims.json`은 missing 상태다.
- Scenario 8 실행 전 active policy 또는 active claim을 runtime에서 새로 생성해야 한다.

Execution candidate:

- app launch 후 Policy/Claim Management section에서 synthetic policy 생성
- 필요 시 synthetic claim 생성
- document registration target dropdown에서 active policy 또는 active claim 선택

Allowed synthetic runtime titles:

- `policy_title_scenario8_demo`
- `claim_title_scenario8_demo`

Forbidden:

- 실제 보험계약 번호
- 실제 청구 번호
- 실제 보험사명
- 실제 병원명
- 실제 진단명/진단코드
- 실제 가족 실명

### Decision 4: Scenario 8 Target Type

Recommended candidate:

- 우선 policy target registration으로 Scenario 8을 수행한다.

Reason:

- policy target만으로 `DocumentRegistrationWorkflow`, document metadata, policy-document link, file copy 경로를 검증할 수 있다.
- claim target까지 검증하면 runtime artifact가 늘어나고 해석이 복잡해진다.
- claim registration은 후속 optional scenario로 분리 가능하다.

Alternative:

- policy target registration과 claim target registration을 둘 다 수행하려면 Scenario 8A / 8B로 분리한다.

Confirmed recommendation:

- Scenario 8A: policy target registration only
- Scenario 8B: claim target registration optional, 별도 approval 후보

### Decision 5: Synthetic Test Document Location

Option A: project root `runtime_test_document.txt`

장점:

- 찾기 쉽다.
- OpenFileDialog에서 선택하기 쉽다.

단점:

- source tree에 untracked file이 생긴다.
- cleanup 또는 commit 제외 관리가 필요하다.

Option B: temp directory outside project root

예:

```text
%TEMP%\FamilyClaimRef\runtime_test_document.txt
```

장점:

- source tree를 오염시키지 않는다.
- git status에 나타나지 않는다.

단점:

- OpenFileDialog에서 경로 확인이 다소 번거롭다.
- temp cleanup 정책이 필요하다.

Recommended:

- Option B, temp directory outside project root를 우선 선택한다.

Reason:

- project root pollution을 최소화한다.
- runtime validation source tree clean 원칙과 맞다.

Confirmed candidate path:

```text
%TEMP%\FamilyClaimRef\runtime_test_document.txt
```

주의:

- 이 decision 문서 생성 중에는 해당 파일을 만들지 않는다.
- execution instruction에서만 생성한다.

### Decision 6: Synthetic Test Document Content

Allowed content:

```text
FamilyClaimRef runtime manual validation synthetic file.
No real personal, insurance, hospital, diagnosis, or claim data.
```

Forbidden:

- 실제 개인정보
- 실제 가족 실명
- 실제 보험계약 번호
- 실제 청구 번호
- 실제 보험사명
- 실제 병원명
- 실제 진단명/진단코드
- 실제 OCR 결과
- 실제 사용자 문서 내용

### Decision 7: OpenFileDialog Policy

Confirmed:

- OpenFileDialog는 Scenario 8 execution에서만 허용한다.
- OpenFileDialog에서는 approved synthetic file만 선택한다.
- 실제 문서가 보이거나 실제 문서를 선택할 위험이 있으면 즉시 중단한다.

Approved synthetic file:

```text
%TEMP%\FamilyClaimRef\runtime_test_document.txt
```

Stop criteria:

- 실제 개인/보험/의료/가족 문서를 선택하려는 상황
- 승인된 synthetic file 외 파일 선택 필요성 발생
- file name에 개인정보/보험/병원/진단 관련 단어가 있는 경우

### Decision 8: Expected Runtime Artifacts

Scenario 8 execution 후 expected runtime artifacts:

- `policies.json` 생성 또는 갱신
- optional `claims.json` 생성 또는 갱신, claim target을 사용하는 경우
- `documents.json` 갱신
- `policy-documents.json` 갱신, policy target 사용 시
- `claim-documents.json` 생성 또는 갱신, claim target 사용 시
- copied attachment under `%LOCALAPPDATA%\FamilyClaimRef\attachments\documents`

Project root expected:

- project root `attachments/`: files=0
- project root `data/local`: files=0
- project root source files unchanged
- temp synthetic file may exist outside project root

### Decision 9: Cleanup Policy After Scenario 8

Confirmed:

- Scenario 8 execution 중 cleanup은 수행하지 않는다.
- Scenario 8 result review에서 cleanup needed 여부를 기록한다.
- runtime artifact cleanup은 별도 cleanup decision / instruction 후에만 수행한다.
- temp synthetic file cleanup도 별도 승인 전 자동 수행하지 않는다.

Reason:

- result evidence 보존이 우선이다.

### Decision 10: Stop Criteria

Confirmed stop criteria:

- app startup crash
- MainWindow binding failure
- active policy creation failure
- approved synthetic file creation failure
- OpenFileDialog에서 approved synthetic file 선택 불가
- actual real document selection risk
- document registration failure
- copied attachment가 project root `attachments/`에 생성됨
- metadata가 project root `data/local`에 생성됨
- DB/SQLite unexpected file 생성
- actual personal sample 포함
- source tree unexpected modification, temp file outside project root 제외
- cleanup 필요 상황 발생

## E. Explicit Non-Scope

이번 decision 문서 생성에서 하지 않는 항목:

- app launch 없음
- OpenFileDialog 실행 없음
- synthetic test document 생성 없음
- runtime policy 생성 없음
- runtime claim 생성 없음
- actual file selection 없음
- document registration workflow 실행 없음
- runtime artifact 생성 없음
- runtime artifact 삭제 없음
- temp file 생성 없음
- temp file cleanup 없음
- code/XAML/ViewModel/test 수정 없음
- DB/SQLite/OCR/repository 구현 없음
- git add/commit/reset/checkout/clean 없음

## F. Guardrails for Scenario 8 Execution Instruction

다음 execution instruction 작성 시 반드시 포함한다.

- git status clean 확인
- latest commit 확인
- build/test 확인
- project root attachments/data/local files=0 확인
- runtime pre-run snapshot
- temp synthetic file path 생성
- synthetic file content 확인
- app launch 허용
- active policy 생성
- optional active claim 생성 여부 명시
- OpenFileDialog 허용 범위 명시
- Select File button 허용
- Register button 허용
- document type 선택 기준
- target policy/claim 선택 기준
- post-run snapshot
- `documents.json` sanity
- `policy-documents.json` or `claim-documents.json` sanity
- copied attachment path sanity
- project root pollution check
- DB/SQLite check
- actual personal sample scan
- cleanup 금지
- result review 문서 생성

## G. Risks Accepted

Accepted risks:

- Scenario 8는 runtime root에 new artifacts를 생성한다.
- runtime root는 clean-room 상태가 아니므로 기존 documents/link/attachment와 결과가 섞일 수 있다.
- OpenFileDialog는 실제 파일 선택 위험을 가진다.
- temp synthetic file이 남을 수 있다.
- cleanup을 하지 않으면 Scenario 8 artifact가 후속 실행에 남는다.
- cleanup을 하면 evidence가 사라질 수 있다.
- active policy/claim을 다시 생성해야 하므로 `policies.json`/`claims.json`이 다시 생긴다.

Risk handling:

- temp file은 project root 밖에 둔다.
- pre/post snapshot을 기록한다.
- OpenFileDialog 선택 파일을 approved path로 제한한다.
- cleanup은 별도 approval 후에만 수행한다.

## H. Next Recommendation

다음 추천 작업:

```text
Scenario 8 synthetic document registration execution instruction 문서 생성
```

예상 문서:

```text
docs/146_POLICY_CLAIM_SCENARIO8_SYNTHETIC_DOCUMENT_REGISTRATION_EXECUTION_INSTRUCTION.md
```

검증:

- git diff --check
- git status --short
- project root attachments/ files count
- project root data/local files count

build/test:

- documentation-only change이므로 실행하지 않는다.
