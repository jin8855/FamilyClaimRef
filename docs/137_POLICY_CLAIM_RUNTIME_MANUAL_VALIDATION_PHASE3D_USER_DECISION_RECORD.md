# Policy / Claim Runtime Manual Validation Phase 3D User Decision Record

## A. Status Marker

POLICY_CLAIM_RUNTIME_MANUAL_VALIDATION_PHASE3D_USER_DECISION_RECORDED

## B. Decision Context

Phase 1에서 Policy / Claim JSON storage가 추가되었다.

Phase 2에서 `DocumentLinkCoordinator` active target validation이 추가되었다.

Phase 3B에서 document registration target dropdown이 추가되었다.

Phase 3C에서 `MainWindow` 내 Policy / Claim Management UI가 추가되었다.

`docs/136_POLICY_CLAIM_RUNTIME_MANUAL_VALIDATION_SCOPE_DESIGN.md`에서 runtime manual validation scope가 설계되었다.

자동 build/test는 통과했지만 actual app launch, `OpenFileDialog`, runtime document registration workflow는 아직 수행하지 않았다.

이 문서는 Phase 3D 실행 전에 사용할 사용자 결정을 고정하기 위한 기록이다. 이 문서 작성 작업에서는 app launch, `OpenFileDialog`, runtime workflow, runtime artifact creation을 수행하지 않는다.

현재 기준 커밋:

```text
b58155d feat(familyclaimref): add policy claim management UI
```

현재 uncommitted 기준 문서:

```text
docs/136_POLICY_CLAIM_RUNTIME_MANUAL_VALIDATION_SCOPE_DESIGN.md
```

## C. Confirmed Decisions

### Decision 1: Runtime Manual Validation 진행 방식

Confirmed:

- Phase 3D runtime manual validation은 별도 execution instruction 작성 후에만 수행한다.
- 이 user decision record 문서 생성 중에는 app launch를 수행하지 않는다.
- 이 user decision record 문서 생성 중에는 runtime data를 만들지 않는다.

Reason:

- runtime validation은 `%LOCALAPPDATA%\FamilyClaimRef` artifact를 생성할 수 있으므로 실행 전 범위 고정이 필요하다.

### Decision 2: App Launch 허용 여부

Confirmed:

- 다음 Phase 3D execution instruction에서는 app launch를 허용하는 방향으로 진행한다.
- 단, app launch는 execution instruction에 명시된 pre-run checklist 완료 후에만 수행한다.

Guardrail:

- execution instruction 작성 전에는 app launch 금지.
- app startup crash 또는 binding failure 발생 시 즉시 중단.

### Decision 3: Scenario Execution Scope

Confirmed:

Phase 3D execution instruction에서 우선 수행할 시나리오는 다음으로 둔다.

Approved scenarios:

1. Startup / MainWindow Binding
2. Empty State
3. Runtime Policy Creation
4. Runtime Claim Creation
5. Policy Disable Block With Active Claim
6. Claim Disable
7. Policy Disable After Claim Disabled

Conditionally approved scenario:

8. Synthetic Document Registration

Decision:

- Scenario 8은 `OpenFileDialog`와 actual registration workflow를 포함하므로 별도 explicit approval gate를 둔다.
- execution instruction에서 Scenario 8은 별도 section으로 분리한다.
- 사용자가 Scenario 8까지 승인한 경우에만 synthetic test document 생성과 `OpenFileDialog` 실행을 허용한다.

### Decision 4: OpenFileDialog 허용 여부

Confirmed:

- Scenario 1~7에서는 `OpenFileDialog` 실행 금지.
- Scenario 8에서만 `OpenFileDialog` 실행 후보로 둔다.
- Scenario 8 실행 전 synthetic test document path를 먼저 확정한다.
- 실제 개인 / 보험 / 의료 / 가족 문서를 선택하지 않는다.

Guardrail:

- `OpenFileDialog`에서 실제 문서를 선택하려는 상황이 발생하면 즉시 중단한다.
- 실제 파일명에 개인정보, 보험, 병원, 진단 관련 값이 있으면 선택 금지.

### Decision 5: Synthetic Test Document

Confirmed:

- synthetic document registration을 실행하는 경우에만 `runtime_test_document.txt`를 생성한다.
- 이 user decision record 문서 작성 중에는 파일을 생성하지 않는다.
- 파일 내용은 harmless synthetic text만 허용한다.

Allowed file name:

```text
runtime_test_document.txt
```

Allowed file content:

```text
FamilyClaimRef runtime manual validation synthetic file.
No real personal, insurance, hospital, diagnosis, or claim data.
```

Forbidden:

- 실제 진단서
- 실제 병원 영수증
- 실제 보험 서류
- 실제 가족 관련 문서
- 실제 OCR 결과
- 실제 개인정보가 포함된 파일

### Decision 6: Runtime Synthetic Data

Confirmed:

Phase 3D에서 사용할 synthetic runtime 값은 아래 후보로 제한한다.

Allowed:

- `policy_title_runtime_demo`
- `claim_title_runtime_demo`
- `runtime_test_document.txt`
- `policy_runtime_demo_001`
- `claim_runtime_demo_001`
- `document_runtime_demo_001`

Forbidden:

- 실제 가족 실명
- 실제 보험계약 번호
- 실제 청구 번호
- 실제 보험사명
- 실제 병원명
- 실제 진단명
- 실제 진단코드
- 실제 OCR 결과
- 실제 사용자 문서 파일명

### Decision 7: Runtime Artifact Root

Confirmed:

- runtime artifact root는 `%LOCALAPPDATA%\FamilyClaimRef`로 본다.
- metadata root는 `%LOCALAPPDATA%\FamilyClaimRef\data\local`로 본다.
- attachment root는 `%LOCALAPPDATA%\FamilyClaimRef\attachments`로 본다.
- project root `attachments/`와 project root `data/local`은 runtime artifact 위치가 아니다.

Expected runtime metadata files:

- `%LOCALAPPDATA%\FamilyClaimRef\data\local\policies.json`
- `%LOCALAPPDATA%\FamilyClaimRef\data\local\claims.json`
- `%LOCALAPPDATA%\FamilyClaimRef\data\local\documents.json`
- `%LOCALAPPDATA%\FamilyClaimRef\data\local\policy-documents.json`
- `%LOCALAPPDATA%\FamilyClaimRef\data\local\claim-documents.json`

Expected copied attachment path candidate:

```text
%LOCALAPPDATA%\FamilyClaimRef\attachments\documents\<physicalFileName>
```

### Decision 8: Pre/Post Snapshot

Confirmed:

- Phase 3D execution instruction은 `%LOCALAPPDATA%\FamilyClaimRef` pre-run snapshot을 반드시 기록한다.
- Phase 3D execution instruction은 `%LOCALAPPDATA%\FamilyClaimRef` post-run snapshot을 반드시 기록한다.
- project root `attachments/`와 project root `data/local` 파일 수도 pre/post로 확인한다.

Snapshot should include:

- directory existence
- file list
- relevant JSON file existence
- copied attachment file path candidate
- no DB/SQLite file check

주의:

- snapshot은 확인과 기록만 수행한다.
- cleanup은 수행하지 않는다.

### Decision 9: Cleanup Policy

Confirmed:

- Phase 3D execution에서는 cleanup을 기본적으로 수행하지 않는다.
- `%LOCALAPPDATA%\FamilyClaimRef` 삭제 금지.
- runtime artifact cleanup은 별도 cleanup scope design 또는 explicit cleanup instruction 후에만 수행한다.
- cleanup이 필요하면 result review에 `cleanup needed`로 기록한다.

Reason:

- cleanup을 먼저 수행하면 runtime validation evidence가 사라질 수 있다.

### Decision 10: Project Root Safety

Confirmed:

- Phase 3D execution 전후에 project root `attachments/` files count를 확인한다.
- Phase 3D execution 전후에 project root `data/local` files count를 확인한다.
- project root에 runtime artifact가 생성되면 stop criteria로 본다.

Expected:

- project root `attachments/`: files=0
- project root `data/local`: files=0

### Decision 11: DB/SQLite Policy

Confirmed:

- Phase 3D runtime validation에서 DB/SQLite 파일이 생성되면 stop criteria로 본다.
- DB/SQLite/OCR/repository 구현은 계속 금지한다.

### Decision 12: Failure / Stop Criteria

Confirmed:

아래 상황이 발생하면 runtime validation을 중단하고 result review를 작성한다.

- app startup crash
- `MainWindow` 표시 실패
- `MainWindow.DataContext` binding failure
- `DocumentRegistrationViewModel` 연결 실패
- `PolicyClaimManagementViewModel` 연결 실패
- Policy / Claim Management section missing
- policy create failure
- claim create failure
- active claim이 있는 policy disable 허용
- disable action이 file metadata 또는 link metadata 삭제
- project root `attachments/` 파일 생성
- project root `data/local` 파일 생성
- DB/SQLite file 생성
- 실제 개인정보 샘플 포함
- 실제 보험 / 의료 / 가족 파일 선택 위험 발생
- source tree unexpected modification
- cleanup이 검증 증거를 삭제할 위험 발생

## D. Explicit Non-Scope

이 user decision record 작업에서 하지 않는 항목:

- app launch 없음
- `OpenFileDialog` 실행 없음
- actual file selection 없음
- actual registration workflow 없음
- runtime policy 생성 없음
- runtime claim 생성 없음
- runtime disable 실행 없음
- synthetic test document 생성 없음
- runtime artifact 생성 없음
- runtime artifact 삭제 없음
- `%LOCALAPPDATA%\FamilyClaimRef` 삭제 없음
- 코드 수정 없음
- XAML 수정 없음
- ViewModel 수정 없음
- tests 수정 없음
- DB/SQLite/OCR/repository 구현 없음
- git add / commit / reset / checkout / clean 없음

## E. Guardrails for Next Execution Instruction

Phase 3D execution instruction 작성 시 반드시 포함할 guardrail:

- `git status --short` 확인
- latest commit 확인
- build/test PASS 확인
- project root `attachments/` files=0 확인
- project root `data/local` files=0 확인
- DB/SQLite unexpected file 없음 확인
- actual personal sample 없음 확인
- `%LOCALAPPDATA%\FamilyClaimRef` pre-run snapshot
- app launch 허용 범위 명시
- Scenario 1~7 실행 순서 명시
- Scenario 8은 별도 explicit approval gate
- `OpenFileDialog`는 Scenario 8에서만 허용
- synthetic test document는 Scenario 8에서만 생성
- post-run snapshot
- cleanup 금지
- failure / stop criteria
- result review 문서 생성

## F. Risks Accepted

Accepted risks:

- runtime validation은 `%LOCALAPPDATA%\FamilyClaimRef`에 artifacts를 생성할 수 있다.
- `OpenFileDialog` 실행 시 실제 문서를 선택할 위험이 있다.
- runtime artifact에 synthetic data가 남을 수 있다.
- cleanup을 하지 않으면 후속 실행에 이전 artifact가 영향을 줄 수 있다.
- cleanup을 하면 validation evidence가 사라질 수 있다.
- manual validation은 재현성이 낮다.
- `MainWindow`가 커져 수동 UX 검증 항목이 많아진다.

Risk handling:

- pre/post snapshot을 반드시 기록한다.
- cleanup은 별도 승인 후 수행한다.
- 실제 파일 선택 위험 발생 시 즉시 중단한다.
- Scenario 8은 별도 approval gate로 분리한다.

## G. Next Recommendation

다음 추천 작업:

```text
Policy / Claim Runtime Manual Validation Phase 3D execution instruction 작성
```

해당 execution instruction은 Scenario 8 포함 여부를 명확히 나누어야 한다.

Recommended structure:

- Base execution: Scenario 1~7 only
- Optional gated execution: Scenario 8 synthetic document registration

이 문서 작성 작업은 documentation-only change이므로 build/test는 실행하지 않는다. 후속 execution instruction 또는 구현 단계에서 필요한 경우 별도로 실행한다.
