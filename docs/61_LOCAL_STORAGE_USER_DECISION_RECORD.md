# Local Storage User Decision Record

## A. Goal

이 문서는 `docs/60_LOCAL_STORAGE_STRATEGY_DECISION.md`의 Needs Decision Q1~Q8에 대한 사용자 결정 기록이다.

목적은 FamilyClaimRef MVP의 로컬 저장 방식 방향을 확정하고, 이후 storage service interface 설계와 JSON file storage 구현 승인 범위를 분리하기 위한 것이다.

이 문서는 구현 문서가 아니다. JSON 저장 구현, SQLite 구현, storage service 구현, repository 구현, DB/OCR/metadata/file storage 구현, WPF UI/XAML 구현, navigation 구현은 수행하지 않는다.

## B. Checked Files / Paths

| 대상 | 확인 목적 | 비고 |
|---|---|---|
| `docs/60_LOCAL_STORAGE_STRATEGY_DECISION.md` | 로컬 저장 방식 비교와 Q1~Q8 Needs Decision 확인 | 읽기 전용 |
| `docs/44_FILE_STORAGE_AND_DOCUMENT_METADATA_DECISION.md` | `attachments/`, `data/local/`, `originalFileName`, OCR 임시 결과 기준 확인 | 읽기 전용 |
| `docs/45_FILE_STORAGE_USER_DECISION_RECORD.md` | 파일 저장 사용자 결정 기록 확인 | 읽기 전용 |
| `docs/46_FILE_STORAGE_DETAIL_POLICY_DECISION.md` | 파일 저장 세부 정책 후보 확인 | 읽기 전용 |
| `docs/47_FILE_STORAGE_DETAIL_POLICY_USER_DECISION_RECORD.md` | 파일 저장 세부 정책 사용자 결정 확인 | 읽기 전용 |
| `docs/59_FILENAME_POLICY_TEST_PROJECT_IMPLEMENTATION_REVIEW.md` | 현재 구현/테스트 상태와 미구현 범위 확인 | 읽기 전용 |
| `FamilyClaimRef.sln` | solution 존재 기준 확인 | 수정 없음 |
| `app/FamilyClaimRef.App/FamilyClaimRef.App.csproj` | WPF app project와 Target Framework 기준 확인 | 수정 없음 |

## C. User Decision Summary

| ID | Question | Decision | Recorded Direction |
|---|---|---|---|
| Q1 | MVP 1차 저장 방식을 JSON으로 시작할 것인가 | Accepted | MVP 1차 저장 방식은 JSON file storage로 시작한다. SQLite는 즉시 구현하지 않는다. |
| Q2 | SQLite를 MVP 이후 확장 후보로 보류할 것인가 | Accepted | SQLite는 MVP 이후 확장 후보로 보류한다. 청구 이력, 검색, 필터, 데이터량 증가가 실제로 필요해질 때 전환을 검토한다. |
| Q3 | `data/local/` 아래에 JSON metadata를 저장할 것인가 | Accepted | metadata 저장 root 후보는 `data/local/`로 유지한다. 실제 JSON metadata 파일 생성은 별도 구현 승인 전까지 하지 않는다. |
| Q4 | 실제 첨부 파일을 `attachments/` 아래에 저장하는 구조로 갈 것인가 | Accepted | 실제 첨부 파일 저장 root 후보는 `attachments/`로 유지한다. 실제 파일 저장/복사 구현은 별도 승인 전까지 하지 않는다. |
| Q5 | storage service interface를 먼저 만들고 JSON 구현체를 붙일 것인가 | Accepted | storage service interface를 먼저 설계하고, MVP 1차 구현체는 JSON implementation 후보로 둔다. SQLite 전환 가능성을 고려해 interface와 model 경계를 분리한다. |
| Q6 | OCR 임시 결과를 MVP에서 저장하지 않는 결정을 유지할 것인가 | Accepted | OCR 임시 결과 저장은 MVP에서 보류한다. 사용자 확정값만 업무 객체에 반영하는 기준을 유지한다. |
| Q7 | raw `originalFileName` 저장 보류 결정을 유지할 것인가 | Accepted | raw `originalFileName` 저장은 MVP에서 보류한다. 화면 표시용 `displayTitle`과 저장용 `physicalFileName` 분리 기준을 유지한다. |
| Q8 | 실제 개인정보 샘플 없이 dummy data만 사용할 것인가 | Accepted | 실제 개인정보 샘플을 사용하지 않는다. 테스트, 문서, 샘플은 dummy data만 사용한다. |

## D. Accepted Direction

- MVP 1차 저장 방식은 JSON file storage로 확정한다.
- SQLite는 MVP 이후 확장 후보로 보류한다.
- metadata root 후보는 `data/local/`로 확정한다.
- actual file root 후보는 `attachments/`로 확정한다.
- storage service interface를 먼저 설계한다.
- JSON implementation은 interface 뒤에 붙이는 MVP 1차 구현체 후보로 둔다.
- OCR 임시 결과 저장은 MVP에서 보류한다.
- raw `originalFileName` 저장은 MVP에서 보류한다.
- 실제 개인정보 샘플 사용은 금지한다.
- 실제 가족 실명, 실제 보험사명, 실제 병원명, 실제 진단명, 실제 진단코드 사례는 사용하지 않는다.

## E. Still Not Implemented

아래 항목은 아직 구현하지 않았다.

- JSON 저장 구현 없음
- SQLite DB 생성 없음
- SQLite package 추가 없음
- storage service interface 구현 없음
- JSON implementation 구현 없음
- repository 구현 없음
- data access 구현 없음
- migration 구현 없음
- DB 구현 없음
- OCR 구현 없음
- metadata 저장 구현 없음
- file storage 구현 없음
- file copy/storage 구현 없음
- WPF UI/XAML 구현 없음
- navigation 구현 없음
- `attachments/` 내부 파일 생성 없음
- `data/local/` 내부 파일 생성 없음

## F. Next Decision Needed

다음 항목은 이후 구현 착수 전 별도 결정이 필요하다.

1. `Document` / `PolicyDocument` / `ClaimDocument` 저장 구조 결정
2. `CategoryItem`과 document type 연결 정책 결정
3. storage service interface 설계 범위 결정
4. JSON 파일 단위 결정
5. metadata schema 초안 결정
6. 저장 구현 테스트 범위 결정

## G. Recommendation

다음 순서로 진행하는 것이 적절하다.

1. `Document` / `PolicyDocument` / `ClaimDocument` 저장 구조 결정 문서를 생성한다.
2. 그 다음 `CategoryItem`과 document type 연결 정책 문서를 생성한다.
3. 그 다음 storage service interface 설계 문서를 생성한다.
4. 그 다음 JSON file storage 구현 여부를 별도 승인받는다.

## H. Result

`LOCAL_STORAGE_USER_DECISION_RECORDED`
