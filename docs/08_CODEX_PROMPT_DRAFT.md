# Codex 지시문 초안: 개발 착수 전 문서 생성 및 검토

## 역할

너는 로컬 프로젝트를 읽고 지정된 범위 안에서 분석/문서화/구현을 수행하는 Codex 실행자다.

현재 단계에서는 실제 기능 구현이 아니라, 기획 문서와 화면 흐름을 기준으로 개발 착수 가능성을 검토하는 것이 목표다.

---

## 목표

`가족 보험 청구 참고 조회기` 프로젝트의 1차 MVP 개발 착수 전 기준 문서를 로컬 프로젝트 `docs` 폴더에 정리하고, 누락된 화면/데이터 흐름/보안 리스크를 점검한다.

---

## 기준 문서

다음 문서를 기준으로 한다.

- 00_PROJECT_BASE_GUIDE.md
- 01_PRD.md
- 02_FEATURE_SPEC.md
- 03_USER_FLOW.md
- 04_SCREEN_LIST.md
- 05_WIREFRAME_SPEC.md
- 06_DATA_MODEL.md
- 07_FIGMA_TRANSFER_GUIDE.md

---

## 절대 금지

- Git commit 금지
- Git reset 금지
- Git checkout 금지
- 외부 API 호출 금지
- 실제 개인정보 샘플 사용 금지
- 실제 보험증권/진단서/영수증 파일 사용 금지
- 파일 삭제 금지
- 기존 문서 overwrite 금지
- 지정 범위 밖 수정 금지
- 보험금 지급 확정 로직 구현 금지
- 클라우드 OCR 구현 금지
- 외부 AI 분석 구현 금지

---

## 작업 범위

허용 범위:

- docs 폴더에 기준 문서 복사 또는 신규 생성
- 문서 간 링크 정리
- 개발 착수 전 누락 체크리스트 작성
- 1차 MVP 구현 순서 제안
- 기술 후보 검토 문서 작성

금지 범위:

- 실제 앱 코드 구현
- OCR 라이브러리 설치
- DB 스키마 구현
- UI 구현
- 외부 통신 코드 작성

---

## 수행 지시

1. docs 폴더 구조를 제안한다.
2. 기준 문서를 docs 폴더에 정리한다.
3. PRD, 기능명세서, 유저플로우, 와이어프레임, 데이터 모델 사이의 누락을 점검한다.
4. 누락/불일치 항목은 `Unknown`, `Needs Decision`, `Risk`로 분리한다.
5. 개발 착수 전 확정해야 할 항목을 정리한다.
6. 1차 구현 순서를 제안한다.

---

## 생성 문서

- docs/PROJECT_BASE_GUIDE.md
- docs/PRD.md
- docs/FEATURE_SPEC.md
- docs/USER_FLOW.md
- docs/SCREEN_LIST.md
- docs/WIREFRAME_SPEC.md
- docs/DATA_MODEL.md
- docs/FIGMA_TRANSFER_GUIDE.md
- docs/IMPLEMENTATION_READINESS_REVIEW.md
- docs/MVP_IMPLEMENTATION_ORDER.md

---

## 완료 보고 형식

```md
## 생성 문서
- ...

## 변경 파일
- ...

## 코드 수정 여부
- 있음 / 없음

## 검증 결과
- ...

## 남은 위험
- ...

## 다음 추천 작업
- ...
```

