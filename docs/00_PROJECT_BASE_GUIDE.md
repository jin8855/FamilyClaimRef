# 가족 보험 청구 참고 조회기 기준 지침서

## 공통 운영 기준

이 프로젝트는 아래 공통 문서를 따른다.

- `C:\DevKnowledgeVault\00_Common\COMMON_OPERATION_GUIDE.md`
- `C:\DevKnowledgeVault\00_Common\COMMON_FORBIDDEN_ACTIONS.md`
- `C:\DevKnowledgeVault\00_Common\MARKDOWN_DOCUMENT_RULES.md`
- `C:\DevKnowledgeVault\00_Common\CODEX_PROMPT_TEMPLATE.md`
- `C:\DevKnowledgeVault\00_Common\PRE_DEV_ARTIFACTS_TEMPLATE.md`
- `C:\DevKnowledgeVault\00_Common\UI_COMMON_GUIDE.md`
- `C:\DevKnowledgeVault\00_Common\CODEX_RESULT_REVIEW_CHECKLIST.md`
- `C:\DevKnowledgeVault\00_Common\THREAD_CONTINUATION_TEMPLATE.md`

공통 규칙은 위 문서를 기준으로 하며, 이 프로젝트 문서에는 공통 규칙의 상세 내용을 반복 복사하지 않는다.

Common 문서가 갱신되면 이 프로젝트에도 최신 공통 기준을 적용한다.

이 프로젝트 문서에는 프로젝트 고유 목적, 경계, 금지 사항, 예외와 강화 기준만 기록한다.

## 작업 규모와 검증

작업 등급, 문서 생성 여부, 사용자 확인 여부, 개발 전 산출물 적용 여부와 검증 수준은 `C:\DevKnowledgeVault\00_Common\COMMON_OPERATION_GUIDE.md`의 L0~L3 기준을 따른다.

이 프로젝트에서 공통보다 강화해야 하는 사항만 `프로젝트 고유 기준` 또는 `프로젝트 예외`에 기록한다.

## 개발 전 산출물 기준

개발 전 산출물은 `C:\DevKnowledgeVault\00_Common\PRE_DEV_ARTIFACTS_TEMPLATE.md`를 따른다.

모든 작업에 모든 산출물을 요구하지 않는다.

L2·L3 또는 실제로 필요한 작업에만 적용 가능한 산출물을 선택해 작성한다.

L0·L1 작업에는 원칙적으로 별도 개발 전 산출물을 요구하지 않는다.

이 프로젝트는 개인정보·의료·보험 정보 취급 위험이 크므로, 새 기능·화면·저장 구조·OCR 흐름 변경은 개발 전 산출물과 사용자 검토를 우선한다.

## UI 및 프로토타입 기준

UI 정의, 화면 설계와 HTML 프로토타입은 `C:\DevKnowledgeVault\00_Common\UI_COMMON_GUIDE.md`를 따른다.

HTML 프로토타입은 실제 구현 예정 화면과 최대한 유사하게 작성한다.

기존 프로젝트가 있는 경우 현재 shell, typography, spacing, button, modal, tab, list, table 패턴을 참조한다.

실제 API, DB, 운영 데이터, 실제 영속 저장, 실제 인증과 세션은 연결하지 않는다.

프로젝트별 실제 디자인 시스템 또는 shell 경로가 확인되면 `프로젝트 고유 기준`에 그 경로만 기록한다.

## 프로젝트 고유 기준

### 프로젝트 목적

- 가족의 보험, 약관, 병원 서류, 청구 이력, 지급 결과를 로컬에서 관리한다.
- 병원비 청구 전 참고할 수 있는 담보 후보를 조회하는 개인용 프로그램을 목표로 한다.
- 보험금 자동 판정 도구, 고객 관리 서비스, 외부 API 기반 보험 조회 서비스가 아니다.

### 프로젝트 성격

- 분석
- 구현
- 검증
- 문서화
- 개인용 로컬 프로그램
- 가족 보험 관리 도구
- 병원 청구 참고 조회 도구

### 실제 프로젝트 경로

- `C:\EtcProject\FamilyClaimRef`

### 프로젝트 문서 경로

- `C:\EtcProject\FamilyClaimRef\docs`

### 허용 범위

- 확정된 문서와 지시문 기준의 로컬 개발
- 로컬 OCR, 로컬 PDF 텍스트 추출, 로컬 DOCX 텍스트 추출 검토
- 로컬 DB 또는 암호화 JSON 저장 구조 검토
- 사용자가 확인한 데이터 기준의 규칙 매칭 설계
- 익명 샘플 데이터 기반 시각화

### 고유 경계

- 민감정보는 PC 밖으로 나가지 않는다.
- OCR 결과는 확정 데이터가 아니다.
- 사용자 확인 데이터만 확정 데이터로 취급한다.
- 앱은 보험금 지급 여부를 확정하지 않는다.
- 실제 보험·의료 문서를 외부 기획 도구에 업로드하지 않는다.

### 고유 금지 사항

- 외부 서버 전송 금지
- 클라우드 OCR 금지
- 생성형 AI 분석 금지
- AI 학습 데이터 사용 금지
- 외부 보험 API 직접 연동 금지
- 실제 개인정보 샘플 사용 금지
- 주민번호 저장 금지
- 계좌번호 저장 금지
- 카드번호 저장 금지
- 증권번호 전체 저장 지양, 마스킹 우선
- 보험금 지급 확정, 청구 가능 확정, 보장 불가 확정, 예상 보험금 확정 표현 금지

### 고유 검증 기준

- OCR 후보값과 사용자 확정값의 구분을 확인한다.
- 청구 사건과 보험사별 청구 이력의 구분을 확인한다.
- 청구 이력과 지급 결과의 연결을 확인한다.
- 개인정보 저장 제외 항목을 확인한다.
- 외부 API 없음, 클라우드 저장 없음, 외부 AI 분석 없음 기준을 확인한다.

### 현재 상태 기준

- 현재 단계: `로컬 제품 구현 및 기능별 검증 진행 단계`
- 보험 영속화·문서 이력(T3-PER-A-R1): `PASS`
- 가족 구성원 JSON 영속화(T3-PER-B): `PASS`
- 분류 aggregate JSON 영속화(T3-PER-C): `PASS`
- ClaimCase MVP 영속화(T3-CLAIM-A): `IMPLEMENTED_AND_LOCALLY_VALIDATED`
  - owner: `FamilyMember`
  - 신규 Product ClaimCase의 `PolicyId`: 기록하지 않음
  - legacy `PolicyId`: nullable 호환 값으로 보존
  - mutation concurrency: process-scoped gate + `expectedRevision`
  - 저장: temp write, flush-to-disk, 재검증, atomic replace와 `.bak` 보존
  - 최종 로컬 검증: focused `23/23`, full regression `790/790`, build warning/error `0/0`
- 프로덕션 준비도: `평가하지 않음`
- 배포: `승인되지 않음`
- 검토 대상 문서 후보:
  - `C:\EtcProject\FamilyClaimRef\docs\01_PRD.md`
  - `C:\EtcProject\FamilyClaimRef\docs\02_FEATURE_SPEC.md`
  - `C:\EtcProject\FamilyClaimRef\docs\03_USER_FLOW.md`
  - `C:\EtcProject\FamilyClaimRef\docs\04_SCREEN_LIST.md`
  - `C:\EtcProject\FamilyClaimRef\docs\05_WIREFRAME_SPEC.md`
  - `C:\EtcProject\FamilyClaimRef\docs\06_DATA_MODEL.md`
  - `C:\EtcProject\FamilyClaimRef\docs\07_FIGMA_TRANSFER_GUIDE.md`

## 프로젝트 예외

- 예외 대상 공통 기준: 개발 전 산출물 적용 강도
- 프로젝트별 적용 방식: 개인정보·의료·보험 정보 취급 위험이 있는 새 기능·화면·저장 구조·OCR 흐름 변경은 공통 L2·L3 기준보다 보수적으로 사용자 검토를 우선한다.
- 예외 근거: 민감정보 로컬 보안 원칙
- 사용자 승인 또는 기준 문서: 이 기준 지침서

