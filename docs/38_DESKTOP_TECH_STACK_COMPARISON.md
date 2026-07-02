# 38_DESKTOP_TECH_STACK_COMPARISON

## 1. Goal

`FamilyClaimRef` MVP의 데스크톱 앱 기술 스택 후보를 비교한다.

이 문서는 app scaffold 전 기술 설계 검토 문서다. 특정 기술 스택을 최종 확정하거나 실제 프로젝트 파일을 생성하지 않는다.

## 2. Checked Files / Paths

- `README.md`
- `docs/01_PRD.md`
- `docs/02_FEATURE_SPEC.md`
- `docs/03_USER_FLOW.md`
- `docs/04_SCREEN_LIST.md`
- `docs/05_WIREFRAME_SPEC.md`
- `docs/06_DATA_MODEL.md`
- `docs/13_SCREEN_REVIEW_CHECKLIST.md`
- `docs/23_SCREEN_TO_DATA_MODEL_MAPPING.md`
- `docs/24_DATA_MODEL_GAP_REVIEW.md`
- `docs/33_UI_STATE_AND_CONFIRMATION_MESSAGE_GUIDE.md`
- `docs/34_PRE_IMPLEMENTATION_READINESS_CHECKLIST.md`
- `docs/35_PRE_IMPLEMENTATION_DECISION_MATRIX.md`
- `docs/36_USER_DECISION_QUESTIONS_BEFORE_IMPLEMENTATION.md`
- `docs/37_USER_DECISION_Q1_Q6_ACCEPTANCE_RECORD.md`
- `design/wireframes/*.html`

## 3. Scope

포함 범위:

- WPF, WinForms, .NET MAUI, Avalonia, Electron 또는 WebView 기반 로컬 앱 비교
- V5.5 와이어프레임과 Q1~Q6 사용자 결정 기준의 적합도 평가
- MVP 1차 추천 후보 제시
- app scaffold 전 사용자 승인 필요 항목 분리

제외 범위:

- app scaffold 생성
- `app/`, `src/`, `package.json`, `tsconfig.json` 생성
- DB 테이블 구조 확정
- OCR 엔진 또는 OCR 구현 방식 확정
- HTML/CSS/JavaScript 수정
- runtime/build 설정 생성
- 실제 개인정보 샘플 작성

## 4. Project Requirements for Desktop App

`FamilyClaimRef`의 현재 기준은 다음과 같다.

- 개인용 로컬 중심 데스크톱 앱
- 가족 보험, 보험 문서, 병원 서류, 청구 이력, 지급 결과 관리
- 민감정보는 사용자 PC 밖으로 내보내지 않는 방향
- 외부 서버 전송, 클라우드 OCR, 외부 AI 분석, 보험사 API 연동은 기본 범위 제외
- 문서 파일은 로컬 연결이 필요하며 Git 추적 대상에서 제외
- OCR 후보값은 자동 확정하지 않고 사용자가 확인한 값만 업무 객체에 반영
- 청구 흐름은 5단계 기준으로 정리
- `ClaimCase`, `ClaimSubmission`, `ClaimPayment`는 분리
- 보험은 `유지`와 `해지`를 구분하고, 해지 보험은 청구 기본 목록에서 제외
- `사용중지`는 앱 관리 상태이고 `해지`는 보험 계약 상태로 분리
- `physicalFileName`, `displayTitle`, `originalFileName` 경계가 필요
- 사용자는 웹/TypeScript보다 C#/.NET 계열에 더 익숙하다는 전제를 둔다.

## 5. Candidate Stack Summary

| 후보 | 1차 판정 | 적합한 경우 | 주요 주의점 |
|---|---|---|---|
| WPF | MVP 1차 추천 | Windows 전용, C#/.NET 중심, 복잡한 화면 상태와 MVVM이 필요한 경우 | XAML 학습 부담, UI 품질은 설계 역량 필요 |
| WinForms | 빠른 CRUD 후보 | 단순 입력/목록/관리 화면을 빠르게 만들 때 | 화면 전환과 복잡한 상태가 늘면 유지보수 부담 |
| .NET MAUI | 현재 MVP에는 과함 | 모바일 또는 다중 플랫폼이 실제 목표일 때 | Windows 데스크톱만 목표라면 복잡도 대비 이점 약함 |
| Avalonia | cross-platform 조건부 후보 | Windows 외 macOS/Linux 배포 가능성을 열어둘 때 | 팀 경험, 컴포넌트, 자료 검토 필요 |
| Electron / WebView 로컬 앱 | 웹 스택 조건부 후보 | 기존 HTML/CSS/JS 자산을 실제 구현 자산으로 재사용해야 할 때 | 앱 크기, 런타임, 보안 설정, TypeScript 부담 |

## 6. Comparison Matrix

| 기준 | WPF | WinForms | .NET MAUI | Avalonia | Electron / WebView |
|---|---|---|---|---|---|
| Windows 데스크톱 적합성 | 높음 | 높음 | 중간 | 높음 | 중간 |
| C#/.NET 친화성 | 높음 | 높음 | 높음 | 높음 | 낮음~중간 |
| V5.5 패널/목록/상세 UI | 높음 | 중간 | 중간 | 높음 | 높음 |
| 5단계 청구 흐름 상태 관리 | 높음 | 중간 | 중간 | 높음 | 중간 |
| OCR 후보 확인 화면 확장 | 높음 | 중간 | 중간 | 높음 | 중간 |
| 로컬 파일 접근 | 높음 | 높음 | 중간 | 높음 | 중간 |
| 로컬 DB 연동 | 높음 | 높음 | 중간 | 높음 | 중간 |
| 민감정보 로컬 보관 통제 | 높음 | 높음 | 중간 | 높음 | 중간 |
| MVVM/구조화 가능성 | 높음 | 낮음~중간 | 중간 | 높음 | 프레임워크 선택 의존 |
| 초기 개발 속도 | 중간 | 높음 | 낮음~중간 | 중간 | 중간 |
| 장기 유지보수 | 높음 | 중간 | 중간 | 중간~높음 | 중간 |
| 배포 부담 | 중간 | 낮음~중간 | 중간~높음 | 중간 | 높음 |
| 앱 크기 | 중간 | 낮음 | 중간 | 중간 | 높음 |
| MVP 추천도 | 높음 | 중간 | 낮음 | 조건부 중간 | 조건부 낮음 |

## 7. WPF Review

### 적합한 점

- Windows 전용 데스크톱 앱에 적합하다.
- C#/.NET 중심 개발에 잘 맞는다.
- MVVM 구조로 화면 상태, 저장 상태, 확인 메시지, dirty-check를 분리하기 좋다.
- V5.5 와이어프레임의 좌우 패널, Top 3 후보 패널, 목록/상세/등록 화면을 구성하기 적합하다.
- 파일 선택, 로컬 파일 경로 처리, 로컬 DB 연동, 로컬 OCR 호출 후보와의 연결이 자연스럽다.
- `ClaimCase`, `ClaimSubmission`, `ClaimPayment`처럼 상태가 분리된 업무 객체를 화면 ViewModel로 나누기 좋다.
- 민감정보 표시명과 물리 파일명을 분리하는 UI/모델 경계를 명확히 두기 좋다.

### 리스크

- XAML과 바인딩 패턴 학습 부담이 있다.
- 좋은 화면 밀도와 정렬을 만들려면 UI 설계 기준이 필요하다.
- Windows 전용이다.
- 과도한 MVVM 추상화는 MVP 속도를 늦출 수 있다.

### 판단

WPF는 현재 프로젝트의 Windows 데스크톱, C#/.NET 친화성, 로컬 파일/DB/OCR 후보 연동, 복잡한 화면 상태 관리 요구와 가장 잘 맞는다.

판정: `WPF_RECOMMENDED_FOR_MVP`

## 8. WinForms Review

### 적합한 점

- 단순 CRUD 화면을 빠르게 만들기 쉽다.
- 가족, 보험, 분류, 문서 목록 같은 관리 화면을 빠르게 구성할 수 있다.
- 로컬 파일 선택과 로컬 DB 연동이 단순하다.
- 초기 학습 부담이 낮다.

### 리스크

- V5.5 화면은 단순 CRUD보다 복잡하다.
- 5단계 청구 흐름, OCR 후보/사용자 확정값 분리, 우측 후보 패널, 진행 현황, 이력 상세가 늘어나면 이벤트 중심 코드가 복잡해질 수 있다.
- 화면 상태와 확인 메시지 기준을 구조화하지 않으면 유지보수 위험이 크다.
- UI 현대성과 반응형 레이아웃 품질이 제한될 수 있다.

### 판단

WinForms는 빠른 CRUD MVP에는 유리하지만, FamilyClaimRef의 장기 화면 구조에는 WPF보다 불리하다.

판정: `WINFORMS_RECOMMENDED_FOR_FAST_CRUD_MVP`

## 9. .NET MAUI Review

### 적합한 점

- C#/.NET 기반이다.
- 장기적으로 모바일 또는 다중 플랫폼 확장이 명확하다면 후보 가치가 있다.

### 리스크

- 현재 사용자 결정은 데스크톱 앱 방향이며 모바일 요구가 없다.
- Windows 데스크톱만 목표라면 프로젝트 복잡도 대비 이점이 약하다.
- 플랫폼별 UI/파일 접근/배포 이슈를 고려해야 한다.
- 로컬 문서 관리와 OCR 후보 확인 중심 MVP에는 과한 선택일 수 있다.

### 판단

모바일 또는 명확한 cross-platform 요구가 생기기 전까지는 MVP 1차 후보로 두지 않는다.

판정: `MAUI_NOT_RECOMMENDED_UNLESS_MOBILE_REQUIRED`

## 10. Avalonia Review

### 적합한 점

- .NET 기반 cross-platform 데스크톱 후보로 의미가 있다.
- WPF와 유사한 XAML/MVVM 감각을 활용할 수 있다.
- Windows 외 macOS/Linux 가능성을 열어둘 수 있다.
- 복잡한 패널, 목록, 상세 화면을 구조화하기 좋다.

### 리스크

- Microsoft 기본 데스크톱 스택은 아니다.
- 팀 경험, 자료, 컴포넌트, 배포 방식 검토가 필요하다.
- Windows 전용 MVP라면 WPF보다 선택 근거가 약하다.
- OCR, 로컬 파일, 로컬 DB 연동은 가능하더라도 실제 라이브러리 조합 검토가 필요하다.

### 판단

Windows 외 플랫폼 확장이 명확한 요구가 되면 강한 후보가 될 수 있다. 현재 MVP 기준에서는 WPF 다음의 조건부 후보로 둔다.

판정: `AVALONIA_RECOMMENDED_IF_CROSS_PLATFORM_REQUIRED`

## 11. Electron / WebView Local App Review

### 적합한 점

- 기존 HTML Low-Fi 와이어프레임의 시각 구조를 실제 화면으로 옮기기 쉽다.
- 웹 UI 구현 경험이 충분하면 레이아웃 생산성이 높을 수 있다.
- WebView 기반이면 일부 웹 자산 재사용 가능성이 있다.

### 리스크

- JavaScript/TypeScript 부담이 커진다.
- 앱 크기와 런타임 무게가 커질 수 있다.
- 로컬 파일 접근과 민감정보 보안 설정이 복잡해진다.
- `attachments/`, 로컬 문서, 원본 파일명, OCR 후보값을 다룰 때 브라우저 런타임 경계와 권한 설계가 필요하다.
- 사용자의 C#/.NET 친화성 전제와 거리가 있다.
- 현재 와이어프레임은 검토용 정적 HTML이며, 구현 자산으로 확정된 것이 아니다.

### 판단

HTML/CSS/JS 재사용이 핵심 요구가 되거나 웹 스택이 명확히 승인되기 전까지는 MVP 1차 후보로 두지 않는다.

판정: `ELECTRON_NOT_RECOMMENDED_UNLESS_WEB_STACK_REQUIRED`

## 12. Recommended MVP Direction

1차 추천은 WPF다.

추천 이유:

- Windows 데스크톱 앱 방향과 잘 맞는다.
- C#/.NET 중심 개발에 적합하다.
- 로컬 파일, 로컬 DB 후보, 로컬 OCR 후보 연결이 자연스럽다.
- V5.5의 다단계 청구 흐름, 우측 후보 패널, 문서함, OCR 확인, 이력 상세를 구조화하기 좋다.
- `ClaimCase`, `ClaimSubmission`, `ClaimPayment`의 상태 분리를 MVVM 구조로 다루기 좋다.
- 민감정보 표시와 저장 경계를 ViewModel / domain model / local storage 계층으로 나누기 좋다.

다만 이 판단은 scaffold 확정이 아니다. WPF로 app scaffold를 생성하려면 사용자 승인과 별도 구현 지시가 필요하다.

## 13. Risks

공통 리스크:

- 기술 스택을 정해도 DB 테이블 구조와 OCR 엔진은 별도 결정이 필요하다.
- 파일명 마스킹, `displayTitle`, `originalFileName` 경계가 구현 전에 확정되어야 한다.
- 해지 보험의 보장기간 기반 예외 청구는 아직 `Needs Decision`이다.
- OCR 원문 저장 예외는 아직 승인되지 않았다.
- app scaffold를 기술 비교 문서만으로 바로 생성하면 범위 위반이다.

WPF 리스크:

- XAML/MVVM 구조를 과하게 잡으면 MVP가 늦어진다.
- 디자인 기준 없이 만들면 정적 와이어프레임의 화면 밀도를 잃을 수 있다.

WinForms 리스크:

- 빠르게 시작할 수 있지만 화면 수와 상태가 늘면 유지보수가 어려워질 수 있다.

MAUI 리스크:

- 현재 요구 대비 플랫폼 복잡도가 크다.

Avalonia 리스크:

- cross-platform이 필요하지 않으면 WPF 대비 선택 근거가 약하다.

Electron / WebView 리스크:

- 로컬 민감정보와 파일 접근 보안 설계가 무거워질 수 있다.

## 14. Questions Before App Scaffold

app scaffold 전에 다음 질문에 답해야 한다.

1. MVP는 Windows 전용으로 확정하는가?
2. WPF를 1차 스택으로 승인하는가?
3. MVVM 구조를 MVP 기본 구조로 둘 것인가?
4. 로컬 저장소는 어떤 후보를 먼저 검토할 것인가?
5. 파일 저장 루트와 `attachments/` 연동 방식은 어떻게 둘 것인가?
6. `physicalFileName`, `displayTitle`, `originalFileName`을 구현 전 어느 수준까지 확정할 것인가?
7. OCR은 MVP에서 실제 연동하지 않고 후보 확인 UI만 먼저 둘 것인가?
8. 해지 보험의 보장기간 기반 예외 청구를 MVP에 포함할 것인가?
9. WPF 선택 시 화면별 ViewModel 분리 기준을 먼저 문서화할 것인가?
10. scaffold 생성 승인 범위는 빈 프로젝트까지만인지, 첫 화면 골격까지인지 구분할 것인가?

## 15. Recommendation

MVP 기술 스택 1차 후보는 WPF로 둔다.

조건부 판단:

- 가장 빠른 단순 CRUD만 원하면 WinForms도 가능하지만 장기 유지보수 리스크가 있다.
- Windows 외 플랫폼이 필요하면 Avalonia를 재검토한다.
- 모바일이 필요하지 않으면 .NET MAUI는 보류한다.
- 웹 스택 재사용이 핵심이 아니면 Electron / WebView는 보류한다.

권장 순서:

1. WPF 승인 여부를 사용자에게 확인한다.
2. 승인 전까지 app scaffold는 생성하지 않는다.
3. WPF가 승인되면 DB/OCR 없이 빈 desktop scaffold 범위만 별도 지시로 분리한다.
4. scaffold 이후에도 DB 설계, OCR 설계, 파일 저장 설계는 별도 단계로 둔다.

## 16. Next Step

- 사용자가 WPF 1차 후보를 승인할지 결정한다.
- 승인 시에도 scaffold 생성 범위를 별도 문서 또는 별도 지시로 확정한다.
- DB 설계와 OCR 설계는 app scaffold 지시와 섞지 않는다.
- 파일명, 원본 파일명, 해지 보험 예외 정책은 구현 전 decision 문서로 이어간다.

## Result

WPF_RECOMMENDED_FOR_MVP
