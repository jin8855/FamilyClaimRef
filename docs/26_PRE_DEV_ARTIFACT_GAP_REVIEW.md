# Pre-Dev Artifact Gap Review

## 1. 목적

변경된 공통 지침 기준으로 `FamilyClaimRef`의 개발 전 산출물이 구현 지시로 넘어가기에 충분한지 검토한다.

이 문서는 구현 지시가 아니다. HTML, CSS, 앱, DB, OCR, runtime scaffold를 생성하거나 수정하지 않는다.

## 2. 공통 기준 요약

- Markdown 문서는 한국어 본문을 기본으로 작성하고, 파일명과 객체명 같은 raw identifier는 원문을 유지한다.
- 확인된 사실과 후보, 미결정 사항을 분리한다.
- 기존 문서는 무조건 overwrite하지 않는다.
- 개발 전에는 요구사항 정의서, 기능 명세서, 사용자 프로세스, UI 정의서, HTML 와이어프레임이 먼저 검토되어야 한다.
- `index.html`은 전체 화면 자가검토 인덱스 역할을 해야 하며, 사용자의 첫 진입 화면은 별도 화면으로 분리하는 것이 권장된다.
- 구현 지시로 넘어가기 전에 사용자의 명시적 승인 문구가 필요하다.
- 화면에는 빈 상태, 오류 상태, 로딩 상태, 확인 메시지, 권한/상태 차이 같은 검토 상태가 충분히 표현되어야 한다.

## 3. 현재 산출물 확인

| 산출물 | 현재 파일 | 상태 | 비고 |
|---|---|---|---|
| 프로젝트 목적/보안 원칙 | `README.md` | Documentation Candidate | 개발 전 시각화 게이트와 보안 원칙이 있음 |
| 요구사항 정의서 | `docs/01_PRD.md` | Documentation Candidate | 존재 확인 대상. 이번 작업에서는 내용 수정 없음 |
| 기능 명세서 | `docs/02_FEATURE_SPEC.md` | Documentation Candidate | 존재 확인 대상. 이번 작업에서는 내용 수정 없음 |
| 사용자 프로세스 | `docs/03_USER_FLOW.md`, `docs/11_USER_FLOW_MERMAID.md` | Documentation Candidate | V5.5 흐름은 Mermaid 문서에 반영됨 |
| 화면 목록 | `docs/04_SCREEN_LIST.md`, `docs/13_SCREEN_REVIEW_CHECKLIST.md` | Documentation Candidate | V5.5 화면 검토 체크리스트 존재 |
| 와이어프레임 기준 | `docs/05_WIREFRAME_SPEC.md`, `docs/09_VISUALIZATION_BASELINE.md` | Documentation Candidate | 정적 검토 기준 존재 |
| 데이터 모델 초안 | `docs/06_DATA_MODEL.md` | Gap | 기존 명칭과 V5.5 화면 기준 명칭의 차이가 있음 |
| 화면-데이터 매핑 | `docs/23_SCREEN_TO_DATA_MODEL_MAPPING.md` | Documentation Candidate | 화면별 입력/저장/조회 객체 정리됨 |
| 데이터 모델 간극 검토 | `docs/24_DATA_MODEL_GAP_REVIEW.md` | Documentation Candidate | 결정 필요 사항이 분리됨 |
| 명칭 결정 초안 | `docs/25_DATA_MODEL_NAMING_DECISION_DRAFT.md` | Documentation Candidate | 이번 작업에서 생성 |
| HTML 와이어프레임 | `design/wireframes/*.html` | Documentation Candidate | 정적 Low-Fi HTML 존재 |
| 전체 화면 인덱스 | `design/wireframes/index.html` | Documentation Candidate | 전체 화면 접근 인덱스 역할 |
| 사용자 첫 진입 화면 | `design/wireframes/01_home_dashboard.html` | Documentation Candidate | `index.html`과 분리된 홈 대시보드 존재 |
| 구현 승인 문구 | 없음 | Gap | 사용자 명시 승인 전 구현 지시로 넘어가면 안 됨 |

## 4. 공통 기준 대비 Gap

| 기준 | 현재 상태 | 판단 | 보완 필요 여부 |
|---|---|---|---|
| 요구사항 정의서 존재 | `docs/01_PRD.md` 존재 | Documentation Candidate | 내용 최신성 검토 필요 |
| 기능 명세서 존재 | `docs/02_FEATURE_SPEC.md` 존재 | Documentation Candidate | V5.5 데이터 객체와 연결성 검토 필요 |
| 사용자 프로세스 존재 | `docs/03_USER_FLOW.md`, `docs/11_USER_FLOW_MERMAID.md` 존재 | Documentation Candidate | 5단계 청구 흐름 기준 유지 확인 필요 |
| UI 정의서 존재 | `docs/05_WIREFRAME_SPEC.md`, `docs/13_SCREEN_REVIEW_CHECKLIST.md` 존재 | Documentation Candidate | 빈/오류/로딩/확인 메시지 상태 보강 필요 |
| HTML 와이어프레임 존재 | `design/wireframes/*.html` 존재 | Documentation Candidate | 정적 검토 가능 |
| 기능-화면 연결표 존재 | `docs/23_SCREEN_TO_DATA_MODEL_MAPPING.md`에서 일부 연결됨 | Gap | 기능 ID 기준 연결표는 별도 보강 필요 |
| 사용자 프로세스 시각화 존재 | `docs/10_IA_MERMAID.md`, `docs/11_USER_FLOW_MERMAID.md`, `docs/12_STATE_FLOW_MERMAID.md` 존재 | Documentation Candidate | V5.5 이후 상태 흐름 최신성 확인 필요 |
| 화면 목록 존재 | `docs/04_SCREEN_LIST.md`, `docs/13_SCREEN_REVIEW_CHECKLIST.md`, `index.html` 존재 | Documentation Candidate | `index.html`의 V5.5 표기 최신성 확인 필요 |
| `index.html` 전체 화면 자가검토 인덱스 역할 | 존재 | Documentation Candidate | 공통 기준상 적절함 |
| 사용자 첫 진입 화면 분리 | `01_home_dashboard.html` 존재 | Documentation Candidate | 파일명이 `home.html`은 아니지만 역할 분리는 되어 있음 |
| 모든 주요 화면 접근 가능성 | `index.html`, 관리 홈, 흐름 링크 존재 | Documentation Candidate | 실제 브라우저 클릭 검증 필요 |
| 빈 상태 / 오류 상태 / 로딩 상태 | 명시 전용 화면 없음 | Gap | 구현 전 상태 예시 또는 체크 문서 보강 필요 |
| 확인 메시지 | 삭제/저장 확인 메시지 기준은 문서상 일부만 있음 | Gap | 구현 전 메시지 기준 필요 |
| 구현 전 사용자 승인 문구 | 없음 | Pre-Implementation Required | 구현 지시 전 명시 승인 필요 |
| 데이터 모델 명칭 정합성 | `06_DATA_MODEL.md`와 V5.5 명칭 차이 존재 | Pre-Implementation Required | `25_DATA_MODEL_NAMING_DECISION_DRAFT.md` 검토 후 결정 필요 |

## 5. 화면 검토 산출물 Gap

- Confirmed: 정적 HTML 와이어프레임은 `design/wireframes/` 아래 존재한다.
- Confirmed: `index.html`은 전체 화면 인덱스 역할을 한다.
- Confirmed: 사용자의 첫 진입 화면 역할은 `01_home_dashboard.html`로 분리되어 있다.
- Confirmed: 주요 청구 흐름은 `청구 시작(서류/이미지 추가) -> OCR 확인 -> 보험 찾기 -> 진행 현황 -> 청구 완료` 5단계 기준이다.
- Candidate: 전체 주요 화면은 `index.html` 또는 관리 화면 목록에서 접근 가능하도록 보인다.
- Gap: 빈 상태, 오류 상태, 로딩 상태, 저장/삭제 확인 메시지를 별도 상태 화면이나 체크리스트로 충분히 검토했는지는 아직 부족하다.
- Gap: 기능 ID와 화면 ID를 직접 연결하는 표는 아직 약하다.
- Gap: V5.5 화면 기준과 `docs/06_DATA_MODEL.md`의 객체명 차이가 남아 있다.

## 6. 구현 전 승인 가능 여부

현재 상태는 `PARTIAL_READY_WITH_GAPS`로 판단한다.

구현 지시로 바로 넘어가면 안 되는 이유는 다음과 같다.

- `docs/06_DATA_MODEL.md`의 기존 명칭과 V5.5 화면 기준 명칭이 아직 최종 정합화되지 않았다.
- `HistoryItem`, `ClaimReferenceResult`, `Tag`, `ClaimMemo`, `HistoryMemo`가 저장 객체인지 후보인지 미정이다.
- `PolicyDocument` / `ClaimDocument`를 물리 분리할지 단일 `Document` alias로 둘지 결정되지 않았다.
- 빈 상태, 오류 상태, 로딩 상태, 확인 메시지의 시각화 또는 명세가 부족하다.
- 사용자의 명시적 구현 승인 문구가 아직 없다.

구현 지시로 넘어가기 전 필요한 승인 문구 후보는 다음과 같다.

```text
위 산출물을 기준으로 Codex 구현 지시문을 작성해도 된다.
```

## 7. 보완 작업 후보

| 우선순위 | 보완 작업 | 판단 | 이유 |
|---|---|---|---|
| 1 | `25_DATA_MODEL_NAMING_DECISION_DRAFT.md` 검토 후 객체명 rename/alias 결정 | Pre-Implementation Required | 데이터 모델 명칭이 구현 지시의 기준이 되기 때문 |
| 2 | `docs/06_DATA_MODEL.md` 수정 여부 결정 | Pre-Implementation Required | 기존 데이터 모델과 V5.5 산출물의 불일치를 해소해야 함 |
| 3 | 문서 객체 구조 결정 | Pre-Implementation Required | `PolicyDocument` / `ClaimDocument` / 단일 `Document` 선택 필요 |
| 4 | `HistoryItem`과 `ClaimReferenceResult` 저장 여부 결정 | Pre-Implementation Required | 이력 조회와 보험 찾기 저장 경계가 구현에 직접 영향 |
| 5 | 빈 상태 / 오류 상태 / 로딩 상태 / 확인 메시지 검토 문서 보강 | Gap | UI 구현 전 상태별 표시 기준 필요 |
| 6 | 기능 ID와 화면 ID 연결표 보강 | Gap | 구현 태스크 분해 시 누락 방지 |
| 7 | 사용자 구현 승인 문구 확보 | Blocker | 명시 승인 전 app 개발로 넘어가면 안 됨 |

## 8. 다음 추천 작업

1. `docs/25_DATA_MODEL_NAMING_DECISION_DRAFT.md`를 검토해 rename / alias 방향을 결정한다.
2. `docs/06_DATA_MODEL.md` 수정 여부와 수정 범위를 별도 지시문으로 확정한다.
3. 화면 상태 검토 문서 또는 체크리스트에 빈 상태, 오류 상태, 로딩 상태, 저장/삭제 확인 메시지 항목을 추가한다.
4. 기능 ID, 화면 ID, 데이터 객체를 연결하는 구현 전 traceability 표를 작성한다.
5. 사용자가 구현 진행을 명시적으로 승인하기 전까지 `app/`, `src/`, DB, OCR, runtime 작업은 시작하지 않는다.
