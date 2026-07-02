# 48_FILENAME_POLICY_IMPLEMENTATION_SCOPE_DECISION

## 1. Goal

이 문서는 후속 승인 시 `FileNamePolicyService`를 구현할 경우 첫 구현 범위를 어디까지 제한할지 정리한다.

이번 작업은 구현 작업이 아니다. `FileNamePolicyService` class, interface, unit test, 파일 저장, 파일 복사, metadata 저장, DB, OCR, `attachments/`, `data/local/` 내부 파일은 생성하지 않는다.

## 2. Checked Files / Paths

| 구분 | 경로 | 확인 내용 |
|---|---|---|
| Target Framework Decision | `docs/42_WPF_TARGET_FRAMEWORK_DECISION.md` | 현재 `net9.0-windows`, .NET 10 SDK 없음, 기능 코드 전 TFM 재결정 필요 |
| MVVM Structure | `docs/43_WPF_MINIMAL_MVVM_STRUCTURE_DESIGN.md` | `FileNamePolicyService` 후보와 구현 금지 경계 |
| File Decision | `docs/44_FILE_STORAGE_AND_DOCUMENT_METADATA_DECISION.md` | `physicalFileName`, `displayTitle`, `originalFileName` 경계 |
| File User Decision | `docs/45_FILE_STORAGE_USER_DECISION_RECORD.md` | 파일 저장 사용자 결정과 service 구현 보류 |
| Detail Policy Decision | `docs/46_FILE_STORAGE_DETAIL_POLICY_DECISION.md` | 순수 정책 함수 후보, 입력/출력 후보 |
| Detail User Decision | `docs/47_FILE_STORAGE_DETAIL_POLICY_USER_DECISION_RECORD.md` | `claim-{id}`, `policy-{id}`, suffix 정책 사용자 결정 |
| Solution | `FamilyClaimRef.sln` | 수정하지 않음 |
| WPF Project | `app/FamilyClaimRef.App/FamilyClaimRef.App.csproj` | 수정하지 않음 |
| File Root Candidate | `attachments/` | 내부 파일 생성하지 않음 |
| Local Data Candidate | `data/local/` | 내부 파일 생성하지 않음 |

## 3. Scope

이 문서가 하는 일은 다음과 같다.

- `FileNamePolicyService`의 순수 정책 함수 후보 범위를 정의한다.
- 입력값 후보를 정의한다.
- 출력값 후보를 정의한다.
- 허용 가능한 validation 범위를 정의한다.
- 구현 금지 범위를 명확히 한다.
- 구현 전 Target Framework 조건을 확인한다.
- 후속 구현 지시 조건을 정리한다.

이 문서가 하지 않는 일은 다음과 같다.

- C# class 생성
- interface 생성
- unit test 생성
- XAML 생성 또는 수정
- `.csproj`, `.sln`, Target Framework 변경
- NuGet package 추가
- 파일 저장 또는 파일 복사
- metadata 저장
- DB 파일 생성
- OCR 구현 또는 OCR 실행
- `attachments/`, `data/local/` 내부 파일 생성

## 4. Implementation Candidate

후속 승인 시 구현 가능한 최소 후보는 다음으로 제한한다.

| 항목 | 범위 |
|---|---|
| 후보 이름 | `FileNamePolicyService` |
| 구현 성격 | 순수 파일명 생성 정책 함수 |
| 입력 | document scope, id, date, document type, extension, duplicate index 후보 |
| 출력 | safe `physicalFileName` 문자열 후보 |
| 파일 접근 | 없음 |
| DB 접근 | 없음 |
| OCR 접근 | 없음 |
| metadata 저장 | 없음 |
| `attachments/` 파일 생성 | 없음 |
| `data/local/` 파일 생성 | 없음 |

후보 함수 책임:

- 입력값을 받아 안전한 `physicalFileName` 문자열 후보를 생성한다.
- 실제 파일 존재 여부는 확인하지 않는다.
- 중복 suffix는 입력값으로 받은 `duplicateIndex`만 반영한다.
- 실제 중복 탐색은 하지 않는다.
- `displayTitle`이나 raw `originalFileName`을 물리 파일명으로 사용하지 않는다.

## 5. Input Contract Candidate

후속 구현 시 입력 후보는 다음과 같다. 이 문서에서는 실제 type, enum, class를 만들지 않는다.

```text
documentScope:
- claim
- policy

id:
- 내부 식별자 후보
- 실제 DB id 아님
- 문자열 또는 숫자 후보

date:
- yyyyMMdd 포맷으로 변환 가능한 날짜 후보

documentType:
- 허용 문서유형 코드 후보

extension:
- 원본 확장자 후보
- 확장자 허용 목록은 별도 결정

duplicateIndex:
- 없음 또는 1 이상 숫자 후보
```

주의:

- `id` 생성 방식은 아직 확정하지 않는다.
- 날짜 기준은 아직 확정하지 않는다.
- 허용 확장자 목록은 아직 확정하지 않는다.
- `duplicateIndex`는 실제 파일 검색 결과가 아니라 호출자가 넘긴 후보값이다.

## 6. Output Contract Candidate

출력 후보는 다음 형식이다.

```text
claim-000001_20260626_receipt.pdf
claim-000001_20260626_receipt_001.pdf
policy-000003_20260626_terms.pdf
policy-000003_20260626_terms_001.pdf
```

주의:

- 위 예시는 구조 예시다.
- 실제 개인정보, 실제 보험사명, 실제 병원명, 실제 진단명, 실제 진단코드는 포함하지 않는다.
- 출력은 `physicalFileName` 후보일 뿐 실제 파일 생성이 아니다.
- 출력 문자열은 디스크에 쓰지 않는다.

## 7. Document Type Code Candidate

문서유형 코드 후보는 다음과 같다. 최종 목록 확정은 별도 결정이다.

### 보험 문서 코드 후보

| 코드 | 의미 |
|---|---|
| `policy` | 보험 문서 일반 |
| `terms` | 약관 |
| `contract` | 계약서 |
| `capture` | 화면 캡처 |
| `etc` | 기타 |

### 청구 문서 코드 후보

| 코드 | 의미 |
|---|---|
| `receipt` | 영수증 |
| `diagnosis` | 진단 관련 문서 |
| `medicine` | 약제비 관련 문서 |
| `visit` | 통원 관련 문서 |
| `admission` | 입퇴원 관련 문서 |
| `surgery` | 수술 관련 문서 |
| `etc` | 기타 |

주의:

- 최종 목록은 아직 확정하지 않는다.
- MVP 초기 상수 후보로만 둘 수 있다.
- `CategoryItem`과 연결 여부는 DB 설계 전까지 확정하지 않는다.
- 실제 진단명이나 병원명은 document type code에 넣지 않는다.

## 8. Validation Boundary

후속 구현 시 허용 가능한 최소 validation 후보는 다음과 같다.

### 허용 후보

- `documentScope`가 `claim` 또는 `policy`인지 확인
- `id`가 비어 있지 않은지 확인
- `date`가 유효한지 확인
- `documentType`이 허용 후보 안에 있는지 확인
- `extension`이 비어 있지 않은지 확인
- 파일명 금지 문자가 제거 또는 치환되는지 확인
- `duplicateIndex`가 없거나 1 이상 숫자인지 확인

### 금지 후보

- 실제 파일 존재 여부 확인
- 디스크 접근
- DB 조회
- OCR 상태 조회
- 문서 metadata 조회
- 중복 파일 자동 탐색
- 파일 hash 계산
- `attachments/` 경로 생성
- `data/local/` 경로 생성

validation은 안전한 파일명 구조를 만들기 위한 문자열 수준 검증으로 제한한다.

## 9. Security / Sensitive Data Boundary

파일명 생성 함수의 최소 보안 기준은 다음과 같다.

- 실제 가족 실명 금지
- 실제 보험사명 금지
- 실제 병원명 금지
- 실제 진단명 금지
- 실제 진단코드 기반 개인 사례 금지
- 주민번호, 증권번호, 계좌번호, 카드번호 전체값 금지
- `displayTitle`을 `physicalFileName`으로 사용 금지
- raw `originalFileName`을 `physicalFileName`으로 사용 금지

주의:

- 함수가 민감정보를 완전히 판별할 수 있다고 가정하지 않는다.
- 사용자 입력값 검증과 경고 메시지는 별도 UI 정책이다.
- 함수는 안전한 구조를 강제하는 수준으로 제한한다.
- 민감정보 탐지 엔진을 구현하지 않는다.

## 10. Target Framework Condition

`docs/42_WPF_TARGET_FRAMEWORK_DECISION.md` 기준으로 현재 상태는 다음과 같다.

| 항목 | 현재 상태 |
|---|---|
| 현재 `TargetFramework` | `net9.0-windows` |
| 현재 확인된 SDK | .NET 9 SDK |
| .NET 10 SDK | 현재 확인 결과 없음 |
| 실제 기능 코드 전 조건 | `net10.0-windows` 전환 여부 사용자 확인 권장 |

이 문서의 TFM 기준:

- 이 문서는 구현 지시가 아니므로 TFM을 변경하지 않는다.
- 후속 C# 코드 구현 전에 TFM을 먼저 결정해야 한다.
- `net10.0-windows` 전환을 선택하면 SDK 설치/확인, `.csproj` retarget, build 검증이 별도 작업으로 필요하다.
- TFM 미결정 상태에서 `FileNamePolicyService` 구현으로 바로 넘어가는 것은 비추천이다.

판정:

- `FileNamePolicyService`의 순수 정책 함수 범위는 정리되었다.
- 그러나 C# 구현 착수 전에는 TFM 결정을 먼저 닫아야 한다.

## 11. Conditions Before Implementation

후속 구현으로 넘어가려면 아래 조건이 필요하다.

1. TFM 결정
2. `FileNamePolicyService` 구현 승인
3. 구현 범위가 순수 정책 함수로 제한됨
4. 파일 접근 금지
5. DB 접근 금지
6. OCR 접근 금지
7. metadata 저장 금지
8. `attachments/`, `data/local` 내부 파일 생성 금지
9. sample/mock data 생성 금지
10. build 검증 범위 지정

후속 구현 지시에는 최소한 다음이 포함되어야 한다.

- 대상 파일 경로
- 생성할 class 또는 함수명
- 허용 입력/출력 계약
- 금지할 의존성
- 테스트 생성 여부
- build 또는 compile 검증 명령

## 12. User Decision Questions

구현 전 사용자 결정 질문은 다음과 같다.

```text
Q1 TFM을 먼저 결정할 것인가?
- A. net10.0-windows 전환 후 코드 구현
- B. net9.0-windows 상태로 순수 정책 함수만 구현
- C. 코드 구현 보류

Q2 FileNamePolicyService 구현을 승인할 것인가?
- A. 순수 정책 함수만 승인
- B. 구현 보류
- C. 범위 재검토

Q3 구현 시 허용 범위:
- A. Service class + 순수 함수만
- B. Service class + 간단한 테스트 코드까지
- C. 문서만 유지

Q4 금지 범위 확인:
- A. 파일/DB/OCR/metadata 접근 모두 금지
- B. 일부 파일 접근 허용
- C. 보류
```

## 13. Recommended Answers

권장 답변은 다음과 같다.

```text
Q1: A 또는 C
Q2: A
Q3: A
Q4: A
```

해석:

- 가장 안전한 순서는 Q1=A, 즉 TFM을 먼저 닫고 구현하는 것이다.
- TFM을 아직 닫지 못하면 Q1=C로 코드 구현을 보류한다.
- `net9.0-windows` 상태에서 코드를 시작하는 Q1=B는 가능하지만 비추천이다.
- 구현이 승인되더라도 Q2=A, Q3=A, Q4=A 기준으로 순수 정책 함수에만 제한한다.

## 14. Risks

남은 위험은 다음과 같다.

- TFM 결정 없이 C# 코드를 만들면 이후 `net10.0-windows` 전환 시 재검증 범위가 커질 수 있다.
- 순수 정책 함수 범위를 넘기면 파일 저장, metadata 저장, DB, OCR 경계로 쉽게 확장될 수 있다.
- document type code 최종 목록이 확정되지 않아 구현 시 상수 후보가 바뀔 수 있다.
- 날짜 기준이 확정되지 않아 호출자가 넘기는 date의 의미가 화면마다 달라질 수 있다.
- 허용 확장자 목록이 확정되지 않아 extension validation 범위가 흔들릴 수 있다.
- suffix는 단순하지만 실제 중복 탐색은 하지 않으므로 호출자가 duplicate index를 책임져야 한다.
- 민감정보 판별은 완전 자동화할 수 없으므로 UI 경고와 입력 정책이 별도로 필요하다.

## 15. Recommendation

추천 순서는 다음과 같다.

1. TFM을 먼저 결정한다.
2. TFM이 확정되면 `FileNamePolicyService` 순수 정책 함수 구현 여부를 별도 승인한다.
3. 첫 구현 범위는 문자열 정책 함수에만 제한한다.
4. 파일 접근, DB 접근, OCR 접근, metadata 저장은 명시적으로 금지한다.
5. document type code, 날짜 기준, 확장자 허용 목록은 구현 전 또는 구현 중 상수 후보로만 둔다.
6. 실제 파일 복사와 `attachments/` 저장은 별도 implementation scope 문서 이후로 미룬다.

현재 판단:

- 파일명 정책 구현 범위는 문서상 정리되었다.
- 코드 착수는 TFM 결정 전에는 진행하지 않는 것이 안전하다.

## 16. Next Step

다음 작업 후보:

```text
docs/49_TFM_USER_DECISION_RECORD_BEFORE_FILENAME_POLICY.md
```

또는 사용자가 TFM을 이미 확정할 경우:

```text
docs/49_FILENAME_POLICY_IMPLEMENTATION_USER_APPROVAL.md
```

다음 작업 전에는 아래를 수행하지 않는다.

- C# service class 생성
- unit test 생성
- `.csproj` 수정
- `.sln` 수정
- SDK 설치
- Target Framework 변경
- 파일 저장/복사 구현
- metadata 저장 구현
- DB/OCR 구현

## Result

`NEEDS_TFM_DECISION_BEFORE_CODE`

