# 39_WPF_STACK_AND_SCAFFOLD_SCOPE_DECISION

## 1. Goal

WPF를 `FamilyClaimRef` MVP 1차 기술 스택으로 승인할지, 승인한다면 다음 단계의 app scaffold 범위를 어디까지 제한할지 결정할 수 있도록 질문을 정리한다.

이 문서는 구현 지시가 아니다. WPF 프로젝트, solution, app/src 폴더, DB 파일, OCR 파일, XAML, runtime/build 설정을 생성하지 않는다.

## 2. Checked Files / Paths

- `docs/34_PRE_IMPLEMENTATION_READINESS_CHECKLIST.md`
- `docs/35_PRE_IMPLEMENTATION_DECISION_MATRIX.md`
- `docs/36_USER_DECISION_QUESTIONS_BEFORE_IMPLEMENTATION.md`
- `docs/37_USER_DECISION_Q1_Q6_ACCEPTANCE_RECORD.md`
- `docs/38_DESKTOP_TECH_STACK_COMPARISON.md`

## 3. Scope

포함 범위:

- WPF 승인 여부 질문
- Windows 전용 MVP 여부 질문
- MVVM 구조 채택 여부 질문
- scaffold 범위 선택지 정리
- scaffold 제외 범위 명시
- DB/OCR 분리 원칙 명시
- 사용자 답변 양식 작성

제외 범위:

- WPF 프로젝트 생성
- `.sln`, `.csproj` 생성
- `app/`, `src/` 생성
- DB 설계 확정 또는 DB 파일 생성
- OCR 구현 방식 확정 또는 OCR 파일 생성
- HTML/CSS/JavaScript 수정
- 실제 개인정보 샘플 작성

## 4. WPF Approval Question

### Q1. MVP 1차 기술 스택을 WPF로 승인할 것인가?

선택지:

- A. WPF를 MVP 1차 스택으로 승인한다.
- B. WPF를 보류하고 WinForms를 재검토한다.
- C. WPF를 보류하고 Avalonia를 재검토한다.
- D. 기술 스택 결정을 보류한다.

권장 답변:

- A

근거:

- 사용자 결정 기록에서 데스크톱 앱 방향이 `Accepted`로 기록되었다.
- WPF는 Windows 데스크톱 앱과 C#/.NET 중심 개발에 적합하다.
- 로컬 파일 처리, 로컬 DB 후보, 로컬 OCR 후보와 연결하기 쉽다.
- V5.5 화면의 5단계 청구 흐름, 우측 후보 패널, 문서함, OCR 확인, 이력 상세를 구조화하기 좋다.
- `ClaimCase`, `ClaimSubmission`, `ClaimPayment`의 상태 분리를 MVVM으로 다루기 좋다.

주의:

- WPF 승인과 WPF scaffold 생성은 다르다.
- WPF 승인만으로 `.sln`, `.csproj`, `app/` 폴더를 만들지 않는다.
- scaffold 생성은 별도 사용자 지시가 있을 때만 가능하다.

## 5. Windows-Only MVP Question

### Q2. MVP는 Windows 전용으로 확정할 것인가?

선택지:

- A. MVP는 Windows 전용으로 확정한다.
- B. Windows 우선이나 cross-platform 가능성은 열어둔다.
- C. cross-platform을 필수로 본다.
- D. 보류한다.

권장 답변:

- A

근거:

- 현재 요구는 개인용 로컬 중심 데스크톱 앱이다.
- Windows 전용이면 WPF 선택 근거가 강해진다.
- Windows 외 플랫폼이 필수이면 Avalonia 재검토가 필요하다.

주의:

- cross-platform을 필수로 두면 WPF보다 Avalonia 후보가 강해진다.
- 모바일 요구가 생기면 .NET MAUI 검토가 필요하지만 현재 MVP 요구와는 거리가 있다.

## 6. MVVM Structure Question

### Q3. WPF MVP 기본 구조는 MVVM으로 둘 것인가?

선택지:

- A. 최소 MVVM 구조로 시작한다.
- B. code-behind 중심으로 빠르게 시작한다.
- C. 화면별로 혼합한다.
- D. 보류한다.

권장 답변:

- A

권장 MVP 구조 후보:

```text
Views
ViewModels
Models
Services
```

주의:

- 실제 폴더 생성은 아직 하지 않는다.
- 구조 후보만 기록한다.
- 과도한 추상화는 금지한다.
- MVP에서는 화면 상태, 저장 상태, 확인 메시지, 민감정보 표시 경계를 분리하는 수준의 최소 MVVM을 목표로 둔다.

## 7. Scaffold Scope Question

### Q4. scaffold 생성 범위는 어디까지 허용할 것인가?

선택지:

- A. 빈 WPF 프로젝트까지만
- B. 빈 WPF 프로젝트 + 기본 폴더 구조까지만
- C. 빈 WPF 프로젝트 + 첫 화면 shell 골격까지
- D. 첫 화면 + 주요 화면 navigation 골격까지
- E. 보류

권장 답변:

- B 또는 C

판단:

- B가 가장 안전하다.
- C는 화면 방향성을 빠르게 확인할 수 있지만 화면 구현으로 번질 위험이 있다.
- D는 첫 scaffold 범위로는 넓다.

강한 주의:

- DB/OCR/실제 저장 기능은 포함하지 않는다.
- 실제 개인정보 샘플은 포함하지 않는다.
- HTML 와이어프레임 전체를 WPF 화면으로 변환하지 않는다.
- scaffold 범위는 별도 구현 지시에서 다시 한 번 제한해야 한다.

## 8. Scaffold Exclusion Rules

다음 항목은 scaffold에 포함하지 않는다.

- DB 생성
- SQLite 등 저장소 확정
- OCR 엔진 연동
- OCR 실행
- 실제 파일 복사
- `attachments/` 내부 파일 생성
- `data/local/` 내부 파일 생성
- 실제 개인정보 샘플
- 실제 가족 실명 샘플
- 실제 보험사명 샘플
- 실제 병원명 샘플
- 실제 진단명 또는 실제 진단코드 기반 개인 사례 샘플
- `ClaimReferenceResult` 계산 구현
- `HistoryItem` projection 구현
- 실제 보험 청구 로직
- 운영 API 호출
- 외부 AI/OCR API 호출
- XAML 상세 화면 구현
- HTML 와이어프레임 일괄 변환

## 9. Scaffold Inclusion Options

### A. 빈 WPF 프로젝트까지만

포함 후보:

- solution / project 생성 후보
- 빌드 가능한 빈 `MainWindow`

제외:

- 화면 구조
- navigation
- sample data
- DB/OCR/파일 처리

판정:

- 가장 좁지만, MVP 화면 방향을 확인하기에는 정보가 부족할 수 있다.

### B. 빈 WPF 프로젝트 + 기본 폴더 구조

포함 후보:

- `Views`
- `ViewModels`
- `Models`
- `Services`
- `Resources`

제외:

- 실제 구현 코드
- DB/OCR/파일 처리
- 화면별 XAML 구현
- 문서 또는 화면 더미 데이터 생성

판정:

- 첫 scaffold로 가장 안전한 범위다.
- 기술 스택과 구조만 확인하고 기능 구현으로 번지는 위험을 줄인다.

### C. 빈 WPF 프로젝트 + 첫 화면 shell 골격

포함 후보:

- `MainWindow` shell
- 좌측 또는 상단 메뉴 shell
- 홈 대시보드 placeholder
- 상태 메시지 placeholder

제외:

- 실제 화면별 기능
- 실제 저장/조회
- DB/OCR 연동
- 실제 문서 파일 처리
- 실제 개인정보 샘플

판정:

- 화면 방향성을 빠르게 확인할 수 있다.
- 다만 첫 scaffold가 UI 구현으로 확대될 위험이 있다.

### D. 주요 화면 navigation 골격

포함 후보:

- 홈
- 청구 시작
- OCR 확인
- 보험 찾기
- 진행 현황
- 청구 완료
- 보험 검색
- 이력 보기
- 관리하기

주의:

- 이 범위는 scaffold치고 넓다.
- 첫 구현 지시로는 과할 수 있다.
- 별도 step으로 분리하는 것이 안전하다.

판정:

- 첫 scaffold 범위로는 비추천한다.

## 10. Recommended Scaffold Scope

권장 scaffold 범위:

- 1순위: B. 빈 WPF 프로젝트 + 기본 폴더 구조
- 2순위: C. 빈 WPF 프로젝트 + 첫 화면 shell 골격

권장 이유:

- B는 기술 스택과 구조만 확인하므로 DB/OCR/화면 구현으로 번질 위험이 가장 낮다.
- C는 사용자가 화면 방향을 빨리 볼 수 있지만, 첫 scaffold부터 XAML 구현 범위가 커질 수 있다.
- D는 첫 scaffold가 아니라 후속 UI shell 단계로 분리하는 편이 안전하다.

## 11. DB / OCR Separation Rules

반드시 유지할 원칙:

- WPF scaffold와 DB 설계는 분리한다.
- WPF scaffold와 OCR 설계는 분리한다.
- WPF scaffold와 파일 저장 정책 구현은 분리한다.
- WPF scaffold와 실제 문서 파일 복사는 분리한다.
- WPF scaffold와 실제 보험 청구 로직은 분리한다.
- scaffold 후 별도 문서로 DB 설계, 파일 저장 설계, OCR 설계를 진행한다.

아직 확정하지 않는 항목:

- 물리 DB 테이블 구조
- SQLite 등 저장소 선택
- OCR 엔진/라이브러리
- OCR 원문 저장 예외
- `ClaimReferenceResult` snapshot 저장 범위
- `HistoryItem` projection 구현 방식
- `physicalFileName`, `displayTitle`, `originalFileName`의 실제 저장 구조

## 12. User Answer Template

아래 형식으로 답변하면 다음 scaffold 지시 범위를 결정할 수 있다.

```text
Q1 WPF 승인:
Q2 Windows 전용:
Q3 MVVM 구조:
Q4 scaffold 범위:
추가 조건:
-
```

예시 답변 형식:

```text
Q1 WPF 승인: A
Q2 Windows 전용: A
Q3 MVVM 구조: A
Q4 scaffold 범위: B
추가 조건:
- DB/OCR/파일 처리 제외
- 첫 scaffold는 빌드 가능한 빈 구조까지만
```

## 13. Risks

- WPF 승인을 scaffold 생성 승인으로 오해하면 `.sln`, `.csproj`, `app/` 생성 범위가 열릴 수 있다.
- scaffold 범위가 C 또는 D로 커지면 화면 구현과 기능 구현이 섞일 수 있다.
- DB/OCR을 scaffold에 포함하면 구현 전 결정 문서의 경계를 위반한다.
- 실제 파일 처리나 원본 파일명 보존을 scaffold에 포함하면 민감정보 저장면이 커질 수 있다.
- MVVM 구조를 과하게 잡으면 MVP 속도가 늦어질 수 있다.
- code-behind 중심으로 시작하면 장기적으로 화면 상태 관리가 복잡해질 수 있다.

## 14. Recommendation

권장 답변:

- Q1 WPF 승인: A
- Q2 Windows 전용: A
- Q3 MVVM 구조: A
- Q4 scaffold 범위: B

권장 진행:

1. 사용자가 WPF와 Windows 전용 MVP를 승인한다.
2. 최소 MVVM 구조를 승인한다.
3. 첫 scaffold 범위는 `빈 WPF 프로젝트 + 기본 폴더 구조`로 제한한다.
4. DB/OCR/파일 저장/실제 화면 구현은 다음 단계로 분리한다.

## 15. Next Step

사용자 답변을 받은 뒤 다음 중 하나로 진행한다.

- Q1~Q4가 승인되면 `WPF scaffold 최소 범위 지시문`을 별도 작성한다.
- Q4가 C 이상이면 화면 구현 범위가 과도하지 않은지 재검토한다.
- Q1 또는 Q2가 보류되면 WinForms/Avalonia 재검토 문서를 먼저 작성한다.
- 어떤 경우에도 사용자 승인 전까지 WPF scaffold를 생성하지 않는다.

## Result

WPF_DECISION_QUESTIONS_READY
