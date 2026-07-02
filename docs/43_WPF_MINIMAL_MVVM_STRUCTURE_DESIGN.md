# 43_WPF_MINIMAL_MVVM_STRUCTURE_DESIGN

## 1. Goal

이 문서는 `FamilyClaimRef` WPF MVP의 최소 MVVM 구조를 설계한다.

현재 단계의 목표는 구현 착수 전에 View, ViewModel, Model, Service 후보의 책임 경계를 정리하는 것이다. 이 문서는 코드 생성 지시서가 아니며, C# 파일, XAML 파일, DB, OCR, 파일 저장 구현을 생성하지 않는다.

## 2. Checked Files / Paths

검토 기준 파일과 경로는 다음과 같다.

| 구분 | 경로 | 확인 내용 |
|---|---|---|
| Data Model | `docs/06_DATA_MODEL.md` | 핵심 객체, Candidate, Needs Decision 경계 |
| Screen Mapping | `docs/23_SCREEN_TO_DATA_MODEL_MAPPING.md` | 화면별 입력, 저장, 조회 객체 |
| Gap Review | `docs/24_DATA_MODEL_GAP_REVIEW.md` | 미결정 객체와 위험 |
| UI State | `docs/33_UI_STATE_AND_CONFIRMATION_MESSAGE_GUIDE.md` | Empty, Loading, Error, Confirm, Dirty 상태 기준 |
| Readiness | `docs/34_PRE_IMPLEMENTATION_READINESS_CHECKLIST.md` | 구현 착수 전 점검 항목 |
| User Decisions | `docs/37_USER_DECISION_Q1_Q6_ACCEPTANCE_RECORD.md` | 사용자 승인 기록 |
| Tech Stack | `docs/38_DESKTOP_TECH_STACK_COMPARISON.md` | 데스크톱 기술 후보 |
| WPF Decision | `docs/39_WPF_STACK_AND_SCAFFOLD_SCOPE_DECISION.md` | WPF 선택 및 scaffold 범위 |
| Scaffold Review | `docs/40_WPF_MINIMAL_SCAFFOLD_REVIEW.md` | 최소 scaffold 검토 |
| Scaffold Structure | `docs/41_WPF_SCAFFOLD_STRUCTURE_AND_TFM_REVIEW.md` | 현재 WPF 폴더와 Target Framework 검토 |
| Target Framework | `docs/42_WPF_TARGET_FRAMEWORK_DECISION.md` | TFM 결정 후보 |
| Solution | `FamilyClaimRef.sln` | 수정하지 않음 |
| WPF Project | `app/FamilyClaimRef.App/FamilyClaimRef.App.csproj` | 수정하지 않음 |
| WPF App Folders | `app/FamilyClaimRef.App/Views`, `ViewModels`, `Models`, `Services`, `Resources` | 책임 후보만 정리 |

## 3. Scope

이번 문서의 범위는 다음으로 제한한다.

- WPF MVP의 최소 MVVM 책임 분리 기준을 정리한다.
- 기존 와이어프레임 화면을 ViewModel 후보로 매핑한다.
- 데이터 모델 문서의 Confirmed, Candidate, Needs Decision 경계를 유지한다.
- UI 상태와 확인 메시지의 책임 위치를 ViewModel 후보 관점으로 정리한다.
- Navigation, Service, File, OCR, DB 관련 요소는 후보 경계만 정리한다.

범위 밖 항목은 다음과 같다.

- C# 파일 생성 또는 수정
- XAML 파일 생성 또는 수정
- `.csproj`, `.sln` 수정
- Target Framework 변경
- NuGet package 추가
- `ViewModelBase`, `RelayCommand`, `NavigationService` 구현
- DB, OCR, 파일 저장, 문서 파서, runtime 구현
- sample/mock data 생성

## 4. Minimal MVVM Principles

WPF MVP의 최소 MVVM 원칙은 다음과 같다.

| 원칙 | 기준 | 구현 여부 |
|---|---|---|
| 외부 MVVM package 미사용 | 초기 MVP에서는 CommunityToolkit.Mvvm 등 외부 패키지 도입을 보류한다. | 구현하지 않음 |
| View 책임 | 화면 배치, binding, 시각 상태 표현만 담당한다. | 구현하지 않음 |
| ViewModel 책임 | 입력값, 조회 결과, 상태 플래그, 버튼 command, 메시지 상태를 가진다. | 구현하지 않음 |
| Model 책임 | 업무 데이터 구조를 표현한다. DB entity로 확정하지 않는다. | 구현하지 않음 |
| Service 책임 | 파일, OCR, 조회, 저장소, 메시지 같은 외부 경계 후보를 표현한다. | 구현하지 않음 |
| Code-behind 최소화 | 화면 초기화 외 로직은 ViewModel 후보로 이동하는 방향을 둔다. 단, shell event는 후보로 남긴다. | 구현하지 않음 |
| Navigation 후보화 | `NavigationService`는 후보로만 둔다. 실제 navigation 구현은 별도 승인 후 진행한다. | 구현하지 않음 |

## 5. Folder Responsibility

현재 scaffold 폴더의 책임 후보는 다음과 같다.

| 폴더 | 책임 후보 | 들어갈 수 있는 후보 | 현재 금지 |
|---|---|---|---|
| `Views/` | 화면 XAML 후보 | `HomeView`, `ClaimCaseView`, `OcrReviewView`, `PolicyManageView`, `HistoryView` | XAML 생성 금지 |
| `ViewModels/` | 화면 상태와 command 후보 | `MainWindowViewModel`, `HomeDashboardViewModel`, `ClaimCaseViewModel`, `OcrReviewViewModel` | C# 생성 금지 |
| `Models/` | 업무 객체 후보 | `FamilyMember`, `Policy`, `ClaimCase`, `ClaimSubmission`, `ClaimPayment` | DB entity 확정 금지 |
| `Services/` | 외부 경계 후보 | `FileNamePolicyService`, `LocalDocumentService`, `OcrCandidateService`, `ClaimReferenceService` | 파일 저장, OCR, DB 구현 금지 |
| `Resources/` | UI 리소스 후보 | 색상, 스타일, 문자열, 민감정보 경고 문구 후보 | 실제 개인정보 샘플 생성 금지 |

`Models/`의 객체는 업무 모델 후보이며 물리 DB 테이블 구조를 의미하지 않는다.

## 6. Screen to ViewModel Candidate Mapping

화면별 ViewModel 후보는 다음과 같다. 이 표는 class 생성 지시가 아니라 구현 전 책임 분리 후보이다.

| 화면 | ViewModel 후보 | 우선순위 | 주요 상태 후보 | 비고 |
|---|---|---|---|---|
| MainWindow shell | `MainWindowViewModel` | MVP First | 현재 화면, title, global message | Navigation 후보와 연결 가능 |
| `index.html` / 홈 | `HomeDashboardViewModel` | MVP First | 진행 현황 요약, 최근 이력, 주요 메뉴 | 초기 shell 대체 후보 |
| 가족 구성원 | `FamilyMembersViewModel` | MVP Later | 가족 목록, 선택 항목, 사용 상태 | 편집/삭제/사용 중지 정책 필요 |
| 가족 등록/편집 | `FamilyMembersViewModel` 또는 `FamilyRegisterViewModel` | MVP Later | 입력값, Dirty, Confirm | 별도 ViewModel 여부 Needs Decision |
| 보험 관리 | `PolicyManageViewModel` | MVP First | 보험 목록, 검색 조건, 행별 action | 행별 문서 추가 진입 포함 |
| 보험 등록/편집 | `PolicyRegisterViewModel` | MVP First | 보험 입력값, 임시저장 상태, 보류 상태 | 삭제/사용 중지 정책 필요 |
| 보험 문서 등록 | `PolicyDocumentRegisterViewModel` | MVP Later | 문서 메타, 파일 선택 상태, 연결 보험 | 파일 저장 구현 제외 |
| 문서함 | `DocumentBoxViewModel` | MVP Later | 문서 목록, 문서 유형 필터, OCR 후보 여부 | 등록 화면이 아닌 조회/관리 화면 |
| 청구 시작 | `ClaimCaseViewModel` | MVP First | 청구 시작 입력, 연결 가족, 문서 후보 | 청구 시작과 서류/이미지 추가 통합 |
| 청구 서류 등록 | `ClaimDocumentRegisterViewModel` | MVP Later | 문서 목적, 파일 선택 상태, 연결 청구 | 보조 화면 |
| OCR 확인 | `OcrReviewViewModel` | MVP First | 문서 후보, OCR 후보값, 사용자 확정값 | 후보값 자동 반영 금지 |
| 보험 찾기 | `ClaimReferenceResultViewModel` | MVP First | 검색 조건, Top 3, 더보기, 선택 보험 후보 | `ClaimReferenceResult` 저장 범위 Candidate |
| 진행 현황 | `ClaimSubmissionViewModel` | MVP First | 보험사별 제출 상태, 지급 상태, 더보기 | `ClaimCase` 완료와 분리 필요 |
| 청구 완료 | `ClaimCompleteViewModel` | MVP First | 완료 요약, 다음 행동, confirmation | 완료 기준 Needs Decision |
| 보험 검색 | `PolicySearchViewModel` | MVP Later | 진단명, 기간, 태그, 진료상황 조건 | 보험 관리 링크 제외 기준 유지 |
| 보험 상세 | `PolicyDetailViewModel` | MVP Later | 담보/특약, 문서 연결, 유사 청구 | `PolicyCoverage` 별도 객체 결정 필요 |
| 이력 보기 | `HistoryViewModel` | MVP Later | 이력 목록, 검색 조건, 더보기 | `HistoryItem`은 projection Candidate |
| 이력 상세 | `HistoryDetailViewModel` | MVP Later | 제출, 지급, 문서 연결, 메모 후보 | `HistoryMemo` Candidate |
| 분류/태그 관리 | `CategoryManageViewModel` | MVP Later | 분류 목록, 태그 목록, 사용 상태 | 일반 태그만 사용 |
| 분류 등록/편집 | `CategoryRegisterViewModel` | MVP Later | 분류명, 상태, Dirty | 코드 생성 없음 |
| 분류 항목 등록/편집 | `CategoryItemRegisterViewModel` | MVP Later | 항목명, 설명, 사용 상태 | `Tag` 분리 여부 Needs Decision |

MVP First 후보는 실제 코드 생성 순서를 확정하지 않는다. Target Framework와 첫 구현 범위 승인 후 다시 축소해야 한다.

## 7. Domain Model Candidate Boundary

데이터 객체 경계는 기존 문서의 Confirmed, Candidate, Needs Decision 기준을 따른다.

| 객체 | 현재 경계 | MVVM 관점 |
|---|---|---|
| `FamilyMember` | 핵심 업무 객체 | 가족 목록과 연결 대상 선택에 사용 |
| `Policy` | 핵심 업무 객체 | 보험 관리, 등록, 검색, 상세에 사용 |
| `PolicyCoverage` | 별도 객체 여부 결정 필요 | 담보/특약 목록 표현 후보 |
| `PolicyDocument` | `Policy` 연결 문서 도메인 명칭 | 물리 분리 여부 Needs Decision |
| `ClaimDocument` | `ClaimCase` 연결 문서 도메인 명칭 | 물리 분리 여부 Needs Decision |
| `Document` | 단일 물리 저장 후보 | `PolicyDocument` / `ClaimDocument`의 기반 후보 |
| `OcrCandidate` | OCR 후보값 | 사용자 확정 전 업무 객체 반영 금지 |
| `OcrExtraction` | OCR 실행 기록 Candidate | 유지 여부 Needs Decision |
| `ClaimCase` | 청구 사건 | 청구 시작과 완료 기준 분리 필요 |
| `ClaimReferenceResult` | 유사 청구 검색 결과 Candidate | snapshot 저장 범위 Needs Decision |
| `ClaimSubmission` | 보험사별 청구 제출 | 진행 현황의 중심 후보 |
| `ClaimPayment` | 지급 결과 | `ClaimSubmission` 종속 여부 결정 필요 |
| `HistoryItem` | 조회 projection Candidate | 저장 객체 전환 여부 Needs Decision |
| `Category` | 관리 분류 | 분류/태그 관리 화면 후보 |
| `CategoryItem` | 관리 항목 | 일반 태그 예시와 연결 후보 |
| `Tag` | 별도 객체 Candidate | `CategoryItem`과 분리 여부 Needs Decision |
| `ClaimMemo` | 메모 Candidate | 별도 객체 여부 Needs Decision |
| `HistoryMemo` | 메모 Candidate | 별도 객체 여부 Needs Decision |

`Policy.appStatus`, `Policy.contractStatus`는 화면 상태와 계약 상태를 분리하는 후보로 유지한다. 확정 enum, DB column, 저장 정책은 아직 구현하지 않는다.

`physicalFileName`, `displayTitle`, `originalFileName` 경계는 파일명 마스킹과 표시명 분리를 위한 후보이다. 실제 저장 위치, 파일명 규칙, 메타데이터 구조는 구현하지 않는다.

## 8. UI State / Message Responsibility

UI 상태와 메시지 책임 후보는 다음과 같다.

| 상태/메시지 | ViewModel 책임 후보 | View 책임 후보 | Service 책임 후보 |
|---|---|---|---|
| Empty | 목록 없음 상태 플래그와 안내 문구 후보 | 빈 상태 화면 표시 | 없음 |
| Loading | 조회 또는 저장 진행 상태 플래그 | 버튼 비활성, loading 표시 | 향후 async 결과 반환 |
| Error | 오류 메시지, 재시도 가능 여부 | 오류 패널 표시 | 향후 실패 결과 반환 |
| Success | 저장/완료 메시지 후보 | 성공 안내 표시 | 향후 처리 결과 반환 |
| Warning | 민감정보, OCR 후보값, 삭제 제한 경고 | 경고 문구 표시 | 정책 결과 후보 |
| Confirm | 삭제, 사용 중지, 보류, 저장 전 확인 상태 | 확인 메시지 표시 | `DialogService` 후보 |
| Dirty state | 입력 변경 여부, 이탈 경고 여부 | 저장 전 경고 표시 | 없음 |
| 민감정보 경고 | 개인정보 입력/표시 화면의 경고 후보 | 경고 문구 표시 | 마스킹 정책 후보 |
| OCR 후보값 경고 | 후보값 자동 반영 금지 안내 | 후보값과 확정값 분리 표시 | OCR 후보 결과 후보 |
| 청구 완료 메시지 | 완료 요약과 다음 행동 상태 | 다음 행동 버튼 표시 | 없음 |

메시지 service, toast, modal, dialog 구현은 하지 않는다. 초기 MVP에서는 ViewModel의 단순 상태 값과 View 표시 후보로만 둔다.

## 9. Navigation Candidate Boundary

Navigation은 후보로만 둔다.

| 항목 | 후보 기준 | 현재 결정 |
|---|---|---|
| `NavigationService` | 화면 전환을 중앙에서 관리하는 후보 | 구현하지 않음 |
| `MainWindowViewModel.CurrentView` | shell 내부 화면 전환 후보 | 구현하지 않음 |
| View-first navigation | 버튼이 View를 직접 전환하는 후보 | 구현하지 않음 |
| ViewModel-first navigation | command가 navigation service를 호출하는 후보 | 구현하지 않음 |
| D scope major navigation | 전체 navigation 구조 구현 | 승인 전 진행 금지 |

MVP 첫 구현은 Home shell 또는 단일 화면에서 시작하는 것이 안전하다. 다중 화면 navigation은 `D scope`로 커질 수 있으므로 별도 승인 전 구현하지 않는다.

`18_claim_document_register.html`에 해당하는 청구 서류 등록 화면은 단계바 독립 단계가 아니라 청구 시작 단계의 보조 화면으로 유지한다.

## 10. Service Candidate Boundary

Service 후보는 다음과 같다. 모두 후보이며 구현하지 않는다.

| Service 후보 | 책임 후보 | 의존 후보 | 현재 금지 |
|---|---|---|---|
| `FileNamePolicyService` | 표시명, 원본 파일명, 물리 파일명 마스킹 정책 | 문서 메타데이터 결정 | 파일명 규칙 구현 금지 |
| `LocalDocumentService` | 로컬 문서 저장/조회 경계 후보 | 파일 저장 위치 결정 | 파일 복사/저장 구현 금지 |
| `DocumentMetadataService` | 문서 메타데이터 후보 관리 | `Document`, `PolicyDocument`, `ClaimDocument` 결정 | DB/파일 메타 저장 구현 금지 |
| `OcrCandidateService` | OCR 후보값 조회/확정 경계 후보 | OCR 도구 결정 | OCR 구현 금지 |
| `ClaimReferenceService` | 과거 유사 청구 Top 3와 더보기 조회 후보 | `ClaimReferenceResult` 저장 범위 결정 | 검색 알고리즘 구현 금지 |
| `HistoryProjectionService` | 이력 조회 projection 후보 | `HistoryItem` 저장 여부 결정 | projection 구현 금지 |
| `SettingsService` | 로컬 설정 후보 | 설정 파일 정책 결정 | 설정 파일 생성 금지 |
| `DialogService` 또는 `MessageService` | 확인/경고 메시지 표시 경계 후보 | UI message 정책 결정 | dialog 구현 금지 |

Service 후보는 ViewModel을 가볍게 유지하기 위한 경계 설계일 뿐이다. 지금은 interface, class, mock 구현을 만들지 않는다.

## 11. Risks

구현 전 남은 위험은 다음과 같다.

- Target Framework 최종 결정 전 code scaffold를 확장하면 이후 retarget 비용이 생길 수 있다.
- `PolicyDocument` / `ClaimDocument`의 물리 분리 여부가 미정이면 문서 관련 Service 경계가 흔들릴 수 있다.
- `OcrCandidate`와 사용자 확정값 저장 경계가 모호하면 OCR 후보가 업무 객체에 잘못 반영될 수 있다.
- `ClaimCase` 완료와 `ClaimSubmission` 완료가 섞이면 청구 완료 화면과 진행 현황 화면의 상태가 충돌할 수 있다.
- `HistoryItem`이 저장 객체인지 projection인지 확정되지 않으면 이력 화면 ViewModel 책임이 과해질 수 있다.
- `Tag`와 `CategoryItem` 분리 여부가 미정이면 분류/태그 관리 화면의 저장 구조가 바뀔 수 있다.
- 삭제와 사용 중지 정책이 확정되지 않으면 가족, 보험, 분류 관리 command가 위험해질 수 있다.
- 파일명 마스킹 기준이 확정되지 않으면 민감정보 노출 위험이 생길 수 있다.

## 12. Recommendation

첫 코드 작업 후보는 다음 중 하나로 축소하는 것을 권장한다.

| 선택지 | 내용 | 장점 | 위험 |
|---|---|---|---|
| Option A | MVVM infrastructure only: `ViewModelBase`, `RelayCommand`, `MainWindowViewModel` | UI 기능 없이 MVVM 기반만 작게 시작 가능 | 구현 착수로 간주되므로 승인 필요 |
| Option B | MainWindow shell only | 화면 구조와 topbar 기준을 확인하기 쉬움 | navigation 범위가 커질 수 있음 |
| Option C | 파일 저장/문서 메타 정책 결정 먼저 | 민감정보와 문서 연결 위험을 먼저 줄임 | 코드 진행은 늦어짐 |

현재 권장 순서는 Option C를 먼저 결정한 뒤, 코드가 필요하면 Option A를 최소 범위로 승인받는 것이다. Target Framework 결정이 완전히 끝나기 전에는 XAML 화면 확장과 Service 구현을 보류한다.

## 13. Next Step

다음 작업은 구현이 아니라 결정 문서 또는 승인 문서로 진행하는 것이 적절하다.

- Target Framework 최종 승인 여부 확인
- 첫 코드 작업 범위가 `MVVM infrastructure only`인지 확인
- `PolicyDocument` / `ClaimDocument` 물리 저장 구조 결정
- OCR 후보값과 사용자 확정값 저장 경계 결정
- 삭제와 사용 중지 정책 결정
- 파일명 마스킹과 문서 메타데이터 저장 위치 결정

위 항목이 정리되기 전에는 ViewModel, Model, Service class를 생성하지 않는다.

## Result

`MVVM_STRUCTURE_DESIGN_READY`

