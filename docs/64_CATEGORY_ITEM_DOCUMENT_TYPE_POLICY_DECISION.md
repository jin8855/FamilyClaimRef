# CategoryItem Document Type Policy Decision

## A. Goal

이 문서는 `CategoryItem`과 document type의 관계를 결정하기 위한 문서다.

목적은 `FileNamePolicyService`의 documentType allowlist와 저장 모델의 `Document.documentType`, `PolicyDocument.documentType`, `ClaimDocument.documentType`을 어떻게 연결할지 정리하는 것이다.

이 문서는 구현 문서가 아니다. C# enum/constant/model 구현, JSON 저장 구현, category storage 구현, storage service 구현, repository 구현, DB/OCR/metadata/file storage 구현은 수행하지 않는다.

## B. Current State

| 항목 | 현재 상태 |
|---|---|
| `FileNamePolicyService` | 구현 완료 |
| `FileNamePolicyService` 자동화 테스트 | PASS 기록 존재 |
| scope | `claim`, `policy` |
| documentType 검증 | scope별 allowlist로 검증됨 |
| `Document.documentType` | 저장 구조 후보 |
| `PolicyDocument.documentType` | 저장 구조 후보 |
| `ClaimDocument.documentType` | 저장 구조 후보 |
| `CategoryItem`과 document type 연결 | 미결정 |
| C# 모델 구현 | 없음 |
| JSON 저장 구현 | 없음 |
| CategoryItem storage 구현 | 없음 |

현재 선행 결정:

- MVP 1차 저장 방식은 JSON file storage다.
- metadata root는 `data/local/`이다.
- actual file root는 `attachments/`다.
- `Document`는 실제 파일 metadata 공통 record다.
- `PolicyDocument`는 `policyId + documentId` 연결 record다.
- `ClaimDocument`는 `claimId + documentId` 연결 record다.
- 파일 metadata는 `Document`에만 저장한다.
- JSON metadata는 `documents.json`, `policy-documents.json`, `claim-documents.json` 분리 파일 구조 후보다.
- 사용 중지 source of truth는 `disabledAt`이다.
- `isDisabled`는 저장하지 않고 파생 상태로 둔다.
- `displayTitle`, `relativePath`는 `Document`에 저장한다.
- raw `originalFileName`은 MVP에서 저장하지 않는다.
- OCR 임시 결과는 MVP에서 저장하지 않는다.
- OCR 확정값 snapshot은 별도 결정으로 보류한다.
- `memo`는 MVP 1차에서 보류한다.

## C. Problem Statement

document type은 파일명 생성, 문서 저장, 화면 표시, 문서함 필터, 보험 문서/청구 문서 구분에 동시에 쓰인다.

결정해야 할 문제:

- `documentType`을 단순 string constant로 둘지, `CategoryItem` 기반으로 관리할지 결정해야 한다.
- `FileNamePolicyService`의 allowlist와 실제 저장 schema의 documentType 기준이 어긋나면 저장, 파일명, 화면 표시가 불일치할 수 있다.
- `Document`, `PolicyDocument`, `ClaimDocument`에 모두 `documentType`이 있으면 중복 또는 역할 충돌 위험이 있다.
- 청구 문서 type과 보험 문서 type은 서로 다른 scope를 가져야 한다.
- 화면 표시용 label과 저장용 code를 분리해야 한다.

## D. Candidate Policy

### Candidate 1: Hardcoded string constants only

내용:

- document type code를 C# constant 또는 static class로 관리한다.
- `CategoryItem` 없이 MVP를 진행한다.

장점:

- 단순하다.
- 구현이 빠르다.
- 테스트 작성이 쉽다.
- `FileNamePolicyService` allowlist와 바로 연결하기 쉽다.

단점:

- 화면 label 관리가 불편하다.
- 추후 사용자 정의, 정렬, 비활성화가 어렵다.
- JSON 저장된 code와 표시 label 매핑을 별도로 관리해야 한다.

### Candidate 2: CategoryItem-driven document type

내용:

- document type을 `CategoryItem` record로 관리한다.
- `CategoryItem`은 `code`, `label`, `scope`, `sortOrder`, `disabledAt` 등을 가진다.

장점:

- 화면 표시, 정렬, 비활성화에 유리하다.
- 향후 사용자 정의 category 확장이 가능하다.
- JSON metadata와 UI label을 연결하기 쉽다.
- `CategoryItem` 관리 화면과 연결하기 좋다.

단점:

- MVP 초기 구현 범위가 커진다.
- category storage와 seed 정책이 필요하다.
- `FileNamePolicyService` allowlist와 동기화 규칙이 필요하다.

### Candidate 3: Hybrid fixed seed CategoryItem

내용:

- MVP에서는 고정 seed category를 사용한다.
- 사용자가 직접 document type category를 추가/수정/삭제하지 않는다.
- code는 고정 string으로 유지한다.
- label, sortOrder, scope는 `CategoryItem` seed 후보로 문서화한다.
- 구현 시점에는 constant 기반으로 시작할 수 있으나, 저장/화면 설계는 `CategoryItem` 전환 가능하게 둔다.

장점:

- MVP 구현 부담을 줄인다.
- code 안정성과 label 표시를 동시에 고려할 수 있다.
- SQLite/JSON 전환에도 유리하다.
- 사용자 정의 category를 나중으로 미룰 수 있다.

단점:

- constant와 seed category의 기준 문서가 어긋날 위험이 있다.
- seed 관리 정책이 필요하다.
- `FileNamePolicyService` allowlist와 seed 기준을 검증하는 테스트가 나중에 필요할 수 있다.

## E. Recommended Direction

### Candidate Recommendation

MVP 1차는 Hybrid fixed seed CategoryItem 방향이 가장 현실적이다.

추천 후보:

- document type code는 안정적인 string code로 둔다.
- 화면 label, sortOrder, scope는 `CategoryItem` seed 후보로 문서화한다.
- 사용자 정의 category 추가/수정/삭제는 MVP 1차에서 제외한다.
- `FileNamePolicyService` allowlist는 seed category와 동일 기준을 가져야 한다.
- `claim` scope와 `policy` scope document type은 분리한다.
- 저장되는 값은 label이 아니라 code다.
- label은 화면 표시용이다.
- `disabledAt`은 `CategoryItem`에도 적용 가능한 후보로 둔다.
- 실제 구현 전에는 `CategoryItem` model/storage 구현을 하지 않는다.

이 추천은 구현 확정이 아니라 `Candidate Recommendation`이다. 사용자 결정 기록 전까지는 구현하지 않는다.

## F. Scope별 Document Type 후보

현재 allowlist는 `FileNamePolicyService.cs` 기준을 우선한다. 테스트 파일은 일부 정상/오류 대표 케이스를 검증하고 있으며 전체 allowlist 항목을 모두 정상 케이스로 검증하지는 않는다.

### Claim document type 후보

| Code | Label Candidate | Status | Note |
|---|---|---|---|
| `receipt` | 영수증 | Current Allowlist | claim scope |
| `diagnosis` | 진단서 | Current Allowlist | claim scope |
| `medicine` | 약제비 서류 | Current Allowlist | claim scope |
| `visit` | 통원 확인 서류 | Current Allowlist | claim scope |
| `admission` | 입퇴원 확인 서류 | Current Allowlist | claim scope |
| `surgery` | 수술 확인 서류 | Current Allowlist | claim scope |
| `etc` | 기타 | Current Allowlist | claim scope |
| `statement` | 진료비 세부내역서 | New Candidate / Needs Implementation | claim scope, 현재 allowlist 없음 |
| `prescription` | 처방전 | New Candidate / Needs Implementation | claim scope, 현재 allowlist 없음 |
| `capture` | 캡처 | New Candidate / Needs Implementation | claim scope, 현재 allowlist 없음 |

### Policy document type 후보

| Code | Label Candidate | Status | Note |
|---|---|---|---|
| `policy` | 보험증권 | Current Allowlist | policy scope |
| `terms` | 약관 | Current Allowlist | policy scope |
| `contract` | 계약서 | Current Allowlist | policy scope |
| `capture` | 캡처 | Current Allowlist | policy scope |
| `etc` | 기타 | Current Allowlist | policy scope |

### Extension allowlist 기준

document type과 별개로 현재 파일 확장자 allowlist는 다음 네 가지다.

- `pdf`
- `jpg`
- `jpeg`
- `png`

확장자 allowlist는 document type seed와 별도 정책이다.

## G. DocumentType 저장 위치 정리

`Document`, `PolicyDocument`, `ClaimDocument`의 documentType 역할은 다음처럼 볼 수 있다.

- `Document.documentType`
  - physical file name 생성과 파일 metadata 분류에 사용한다.
  - `FileNamePolicyService` 결과와 연결된다.
- `PolicyDocument.documentType`
  - 보험 도메인에서 해당 문서의 업무 type을 나타낸다.
  - MVP에서는 `Document.documentType`과 동일 code를 유지하는 후보가 있다.
- `ClaimDocument.documentType`
  - 청구 도메인에서 해당 문서의 업무 type을 나타낸다.
  - MVP에서는 `Document.documentType`과 동일 code를 유지하는 후보가 있다.

주의할 점:

- documentType을 3곳에 모두 저장하면 정합성 위험이 있다.
- 장기적으로는 `PolicyDocument` / `ClaimDocument`의 type을 source of truth로 두고, `Document.documentType`은 공통 파일 분류 또는 제거 후보로 볼 수 있다.
- MVP에서는 중복 저장을 최소화하는 방향을 평가해야 한다.

### Option A: `Document.documentType`만 저장

장점:

- 중복이 없다.
- `Document`만으로 파일명 재생성과 파일 분류를 이해하기 쉽다.

단점:

- 도메인별 type 의미가 약해진다.
- 하나의 `Document`가 여러 도메인에 재사용될 경우 의미 충돌 가능성이 있다.
- `claim` scope와 `policy` scope 분리 근거가 약해질 수 있다.

### Option B: 도메인 연결 record에만 documentType 저장

장점:

- 업무 의미가 명확하다.
- `PolicyDocument`와 `ClaimDocument` scope 분리가 가능하다.
- `Document`는 실제 파일 metadata 공통 record라는 책임을 유지하기 쉽다.

단점:

- 파일명 생성 시 document type을 별도로 전달해야 한다.
- `Document`만 봤을 때 파일 type을 알기 어렵다.
- file metadata 조회 화면에서 도메인 연결 record join 또는 lookup이 필요하다.

### Option C: MVP에서는 양쪽 저장, 검증으로 일치 강제

장점:

- 파일명, 표시, 도메인 처리가 단순하다.
- 문서함에서 `Document`만 조회해도 기본 분류를 표시하기 쉽다.
- 초기 구현 속도가 빠를 수 있다.

단점:

- 중복 저장으로 정합성 관리가 필요하다.
- document type 변경 시 두 위치를 함께 갱신해야 한다.
- SQLite 전환 시 source of truth를 다시 정리해야 할 수 있다.

### Recommendation 후보

- MVP에서는 Option B 또는 Option C를 비교한다.
- 중복을 줄이려면 Option B가 더 정합적이다.
- 다만 `FileNamePolicyService`와 파일 metadata 추적을 단순화하려면 Option C가 빠르다.
- 최종 결정은 사용자 결정 문서에서 받는다.

## H. Needs Decision

| ID | Question | Status |
|---|---|---|
| Q1 | MVP document type 관리는 Hybrid fixed seed CategoryItem 방향으로 갈 것인가? | Needs Decision |
| Q2 | 사용자 정의 document type 추가/수정/삭제는 MVP 1차에서 제외할 것인가? | Needs Decision |
| Q3 | 저장되는 document type 값은 label이 아니라 code로 할 것인가? | Needs Decision |
| Q4 | claim scope와 policy scope의 document type allowlist를 분리할 것인가? | Needs Decision |
| Q5 | CategoryItem seed에는 `code`, `label`, `scope`, `sortOrder`, `disabledAt` 후보를 둘 것인가? | Needs Decision |
| Q6 | CategoryItem 자체 JSON 저장 구현은 MVP 1차 storage 구현 이후로 보류할 것인가? | Needs Decision |
| Q7 | `FileNamePolicyService` allowlist와 CategoryItem seed 기준은 반드시 일치해야 하는가? | Needs Decision |
| Q8 | 현재 코드 allowlist에 없는 document type 후보는 `New Candidate / Needs Implementation`으로 분리할 것인가? | Needs Decision |
| Q9 | `Document.documentType`, `PolicyDocument.documentType`, `ClaimDocument.documentType` 중 source of truth를 어디에 둘 것인가? | Needs Decision |
| Q10 | MVP에서는 documentType 중복 저장을 허용할 것인가, 아니면 도메인 연결 record에만 둘 것인가? | Needs Decision |
| Q11 | document type 변경 시 기존 저장 문서의 처리 정책은 별도 결정으로 보류할 것인가? | Needs Decision |

## I. Out of Scope

이번 문서에서 제외한다.

- C# enum/constant 구현 없음
- CategoryItem C# 모델 구현 없음
- CategoryItem JSON 저장 구현 없음
- document type allowlist 코드 수정 없음
- `FileNamePolicyService` 수정 없음
- 테스트 코드 수정 없음
- JSON 저장 구현 없음
- SQLite DB 생성 없음
- storage service interface 구현 없음
- repository/data access 구현 없음
- DB/OCR/metadata/file storage 구현 없음
- WPF UI/XAML 구현 없음
- navigation 구현 없음
- 실제 개인정보 샘플 없음
- 실제 가족 실명 없음
- 실제 보험사명 없음
- 실제 병원명 없음
- 실제 진단명/진단코드 사례 없음

## J. Risks

| 위험 | 설명 | 완화 후보 |
|---|---|---|
| code/label/scope 혼합 | code, label, scope가 분리되지 않으면 UI와 저장값이 섞인다. | 저장값은 code, 화면은 label로 분리한다. |
| MVP 범위 확대 | CategoryItem을 너무 빨리 구현하면 MVP 범위가 커진다. | fixed seed 문서화 후 구현은 보류한다. |
| 표시/정렬 불편 | CategoryItem을 전혀 고려하지 않으면 이후 화면 표시, 정렬, 비활성화가 불편하다. | Hybrid fixed seed 후보를 유지한다. |
| allowlist 불일치 | `FileNamePolicyService` allowlist와 CategoryItem seed가 어긋나면 파일명 생성과 저장 검증이 충돌한다. | seed와 allowlist 일치 검증을 후속 테스트 후보로 둔다. |
| source of truth 불명확 | documentType source of truth가 불명확하면 `Document`, `PolicyDocument`, `ClaimDocument` 간 정합성이 깨진다. | 사용자 결정 문서에서 Option B/C 중 하나를 고른다. |
| 변경/비활성화 정책 부재 | document type 변경 또는 비활성화 시 기존 문서 처리 정책이 필요하다. | 기존 문서 처리 정책은 별도 Needs Decision으로 둔다. |

## K. Recommendation

다음 순서를 추천한다.

1. 이 문서를 기준으로 CategoryItem/document type 정책 결정을 받는다.
2. 사용자 결정 후 `docs/65_CATEGORY_ITEM_DOCUMENT_TYPE_USER_DECISION_RECORD.md`를 생성한다.
3. 그 다음 storage service interface 설계 문서를 생성한다.
4. 그 다음 JSON schema 초안 문서를 생성한다.
5. 그 다음 C# model 구현 여부를 별도 승인받는다.

## L. Result

`CATEGORY_ITEM_DOCUMENT_TYPE_POLICY_DECISION_DRAFTED`
