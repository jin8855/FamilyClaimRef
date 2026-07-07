# Nightwork Instruction Pack Handling Decision

## A. Status Marker

NIGHTWORK_INSTRUCTION_PACK_HANDLING_DECISION_RECORDED

## B. Decision Context

- `docs/nightwork_20260706/`는 퇴근 후 사용자 판단 없는 작업을 분리하기 위해 생성된 운영 지시서 pack이다.
- 실제 Scenario 8B runtime execution은 수행하지 않았다.
- nightwork pack은 `docs/157~162` 생성/검토를 돕는 operational artifact였다.
- `docs/157~158`, `docs/160~162`는 이미 commit 완료되었다.
- 현재 남은 untracked item은 `docs/nightwork_20260706/`뿐이다.
- 이 폴더를 어떻게 처리할지 결정이 필요하다.

## C. Current State

- latest commit: `ff9e66c docs(familyclaimref): add scenario8b claim target plan`
- git status:

```text
?? docs/nightwork_20260706/
```

- `docs/nightwork_20260706/` exists.
- contents are operational instructions.
- not staged.
- not committed.
- not modified in this task.

## D. Options

### Option A: Commit nightwork pack

Pros:

- 당시 운영 지시 흐름이 보존된다.
- 추적 가능성이 높다.

Cons:

- evidence chain 문서라기보다 작업 운영용 문서다.
- 향후에도 날짜별 nightwork 폴더가 누적될 수 있다.
- repo 문서 노이즈가 늘어난다.

### Option B: Keep untracked locally

Pros:

- 삭제하지 않고 유지할 수 있다.
- commit history를 오염시키지 않는다.

Cons:

- git status noise가 계속 남는다.
- 후속 작업마다 expected-but-excluded item으로 관리해야 한다.

### Option C: Add ignore rule for docs/nightwork_*/

Pros:

- status noise를 줄인다.
- 운영 지시서 pack을 local-only로 유지할 수 있다.

Cons:

- `.gitignore` 정책 변경이 필요하다.
- `docs` 하위 ignore rule은 신중해야 한다.
- 향후 필요한 nightwork evidence까지 숨길 수 있다.

### Option D: Delete nightwork pack

Pros:

- status clean 가능.

Cons:

- 삭제는 현재 별도 승인 없이는 금지.
- 사용자가 아침에 보려는 작업지시서 evidence를 잃는다.

## E. Recommended Decision

Recommendation:

- Option A 또는 Option C 중 선택 필요.
- 냉정하게는 Option A는 단발성 evidence 보존에 적합하다.
- 그러나 장기 운영 관점에서는 Option C가 더 깨끗하다.
- 이번 단계에서는 삭제하지 말고, 사용자에게 commit 여부를 묻는다.

Recommended immediate path:

1. `docs/163` decision 문서만 생성한다.
2. 이후 사용자가 다음 중 선택한다.
   - nightwork pack commit
   - nightwork pack ignore rule 설계
   - nightwork pack local keep

## F. Explicit Non-Scope

- nightwork 폴더 수정 없음
- nightwork 폴더 삭제 없음
- nightwork 폴더 이동 없음
- nightwork 폴더 stage 없음
- `.gitignore` 수정 없음
- app launch 없음
- Scenario 8B 실행 없음
- cleanup 없음
- commit 없음

## G. Next Recommendation

사용자 결정 필요:

- `docs/nightwork_20260706/` commit 여부
- 또는 ignore rule 여부
- 또는 local keep 여부

Verification required:

- `git diff --check`
- `git status --short`
- `git check-ignore -v -- data/claimdoc/`
- project root `attachments/`: files=0
- project root `data/local`: files=0
- project root `runtime_test_document.*`: missing
- DB/SQLite unexpected file 없음
