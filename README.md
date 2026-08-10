# FamilyClaimRef

FamilyClaimRef는 가족 보험, 약관, 병원 서류, 청구 이력, 지급 결과를 로컬에서 정리하고 보험금 청구 전에 참고할 수 있는 정보를 빠르게 조회하기 위한 개인용 프로젝트다.

이 저장소는 로컬 Product app 구현과 기능별 검증을 진행하는 단계다. FamilyMember,
InsurancePolicy, Category aggregate, FamilyMember 소유 ClaimCase, 보험사별 ClaimSubmission과
ClaimPayment MVP의 JSON 영속화는 구현·검증 중이다. OCR과 배포 준비도는 승인 또는 평가되지 않았다.

## 프로젝트 목적

- 가족 구성원별 보험 가입 내역을 한곳에서 정리한다.
- 보험 약관과 특약 정보를 청구 판단 참고 자료로 연결한다.
- 진단서, 진료비 영수증, 약제비 영수증 등 병원 서류의 입력 부담을 줄이는 방향을 검토한다.
- 과거 청구 이력과 실제 지급 결과를 함께 보관해 중복 확인과 누락 방지를 돕는다.
- 보험금 지급 가능 여부를 확정하는 시스템이 아니라, 사용자가 직접 확인할 참고 정보를 제공하는 도구로 유지한다.

## 보안 원칙

- 민감정보는 기본적으로 사용자 PC 밖으로 내보내지 않는다.
- 외부 서버 전송, 클라우드 OCR, 외부 AI 분석, 보험사 API 연동은 기본 범위에서 제외한다.
- 실제 주민등록번호, 계좌번호, 카드번호, 증권번호 전체값은 저장하지 않는다.
- 문서, 이미지, 로컬 추출 데이터는 Git 추적 대상에서 제외한다.
- 샘플 문서와 화면 설계에는 익명 데이터만 사용한다.
- 개발 전후 모든 기능은 민감정보 노출 가능성을 먼저 검토한다.

## 역할 분리

| 역할 | 책임 |
|---|---|
| ChatGPT | 기획 정리, 요구사항 구조화, 문서 초안 작성, 검토 질문 생성 |
| Figma / FigJam | IA, 사용자 흐름, Low-Fi wireframe, 화면 구조 시각화 |
| Codex | 승인된 문서와 시각화 결과를 기준으로 로컬 app 구현 및 검증 |
| 사용자 | 실제 업무 기준 판단, 민감정보 관리, 최종 기능 범위 승인 |

## 신규 고위험 기능의 사전 검토 기준

새로운 고위험 기능을 시작할 때 다음 조건을 먼저 확인한다. 이 기준은 이미 구현·검증된
app 전체를 미승인 상태로 되돌리는 게이트가 아니다.

1. 해당 기능의 `docs/` 기준 문서와 승인 범위가 최신 상태여야 한다.
2. 사용자 흐름이나 화면 구조가 바뀌면 FigJam 또는 Figma에서 필요한 범위를 검토한다.
3. 실제 개인정보나 실제 보험 문서를 시각화·검증 도구에 올리지 않는다.
4. 신규 입력값, 저장값, 조회값의 민감도와 Git 제외 범위를 확인한다.
5. DB, OCR, 외부 API, 실제 데이터 migration 등 고위험 경계를 구현하기 전에 사용자의 명시적 승인을 받는다.

현재 구현된 FamilyMember, InsurancePolicy, Category aggregate, ClaimCase, ClaimSubmission 기능은 이 기준으로
승인 상태가 취소되지 않는다. 승인되지 않은 신규 고위험 범위만 별도 검토한다.

## 문서 구조

```text
FamilyClaimRef/
  README.md
  .gitignore
  docs/
    00_PROJECT_BASE_GUIDE.md
    01_PRD.md
    02_FEATURE_SPEC.md
    03_USER_FLOW.md
    04_SCREEN_LIST.md
    05_WIREFRAME_SPEC.md
    06_DATA_MODEL.md
    07_FIGMA_TRANSFER_GUIDE.md
    08_CODEX_PROMPT_DRAFT.md
  design/
    figma/
    wireframes/
  attachments/
  data/
    local/
```

## Git 추적 제외

다음 경로는 실제 문서, 이미지, 로컬 추출 데이터가 들어갈 수 있으므로 Git에 올리지 않는다.

- `attachments/`
- `data/local/`

기준 문서와 시각화 산출물만 추적 대상으로 유지한다.
