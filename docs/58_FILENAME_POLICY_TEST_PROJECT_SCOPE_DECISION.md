# 58_FILENAME_POLICY_TEST_PROJECT_SCOPE_DECISION

## 1. Goal

이 문서는 `FileNamePolicyService` 자동화 테스트 전환 전에 test project 생성 여부와 첫 자동화 테스트 범위를 결정하기 위한 초안이다.

이번 작업은 문서 생성만 수행한다. test project 생성, C# 테스트 파일 생성, `.sln` 수정, `.csproj` 수정, NuGet package 추가, production code 수정, DB/OCR/metadata 구현, 파일 저장 구현은 수행하지 않는다.

## 2. Checked Files / Paths

| 구분 | 경로 | 확인 내용 |
|---|---|---|
| Test Scope Decision | `docs/52_FILENAME_POLICY_SERVICE_TEST_SCOPE_DECISION.md` | 기존 테스트 방식 후보와 test project 주의사항 |
| Test Cases | `docs/53_FILENAME_POLICY_TEST_CASES.md` | patch 이후 N1-N7, E1-E22, B1-B5 자동화 후보 |
| Manual Review | `docs/54_FILENAME_POLICY_MANUAL_REVIEW_RECORD.md` | patch 이후 수동 검토 기록 |
| Patch Review | `docs/57_FILENAME_POLICY_PATCH_REVIEW.md` | `FileNamePolicyService` 정책 patch 결과와 build PASS 기록 |
| Service Source | `app/FamilyClaimRef.App/Services/FileNamePolicyService.cs` | 현재 production code 상태 |
| Solution | `FamilyClaimRef.sln` | app project만 포함, test project 없음 |
| App Project | `app/FamilyClaimRef.App/FamilyClaimRef.App.csproj` | `net10.0-windows`, WPF app project |

## 3. Current State

현재 상태는 다음과 같다.

- WPF scaffold는 생성되어 있다.
- Target Framework는 `net10.0-windows`이다.
- `FamilyClaimRef.sln`에는 현재 app project만 포함되어 있다.
- `dotnet build FamilyClaimRef.sln`은 이전 기준 PASS 상태로 기록되어 있다.
- `FileNamePolicyService` production code는 구현 및 정책 patch가 완료되어 있다.
- `duplicateIndex`는 `1`부터 `999`까지만 허용한다.
- `duplicateIndex <= 0` 또는 `duplicateIndex > 999`는 `ArgumentOutOfRangeException`이다.
- extension allowlist는 `pdf`, `jpg`, `jpeg`, `png`이다.
- allowlist 밖 extension은 `ArgumentException`이다.
- `docs/53_FILENAME_POLICY_TEST_CASES.md`는 patch 이후 기준으로 갱신되어 있다.
- `docs/54_FILENAME_POLICY_MANUAL_REVIEW_RECORD.md`도 patch 이후 기준으로 갱신되어 있다.
- 아직 test project는 없다.
- 아직 자동화 테스트 코드는 없다.
- `dotnet test`는 아직 실행하지 않았다.

## 4. Decision Targets

자동화 테스트 전환 전에 분리해서 결정해야 할 항목은 다음과 같다.

1. test project를 생성할 것인가?
2. 생성한다면 프로젝트명은 무엇으로 할 것인가?
3. test framework는 무엇을 사용할 것인가?
4. `.sln`에 test project를 추가할 것인가?
5. app project를 test project에서 참조할 것인가?
6. NuGet package 추가가 필요한가?
7. 자동화 테스트 첫 범위는 어디까지인가?
8. 자동화하지 않고 문서 기반으로 남길 정책 미결정 항목은 무엇인가?

## 5. Candidate Recommendation

아래 추천안은 확정이 아니라 Candidate이다. 실제 test project 생성 전 사용자 승인이 필요하다.

| 항목 | Candidate |
|---|---|
| test project 생성 | 생성 후보 |
| test project 경로 | `tests/FamilyClaimRef.App.Tests/` |
| test project 이름 | `FamilyClaimRef.App.Tests` |
| test framework | `xUnit` |
| 첫 자동화 대상 | `FileNamePolicyService` |
| app project 참조 | test project에서 app project 참조 후보 |
| `.sln` 추가 | 별도 승인 후 추가 후보 |
| NuGet package | test project에만 추가 후보 |

첫 자동화 대상은 `FileNamePolicyService.CreatePhysicalFileName(...)`로 제한하는 것을 권장한다.

## 6. First Automation Scope Candidate

첫 자동화 테스트 범위 후보는 다음과 같다.

| Case Range | Automation Status | 비고 |
|---|---|---|
| N1-N7 | Auto Candidate | 정상 입력/출력 검증 |
| E1-E22 | Auto Candidate | 예외 타입 검증 |
| B1-B2 | Auto Candidate | suffix 없음, `_999` 경계 검증 |
| B3 | Moved to Error Case / E19 | `duplicateIndex=1000`은 E19로 검증 |
| B4-B5 | Auto Candidate | extension dot 정규화 검증 |
| Undecided Policy Cases | Needs Policy Decision | 자동화 보류 |

첫 자동화 범위에 포함하는 항목:

- N1-N7 정상 케이스
- E1-E22 오류 케이스
- B1-B2 경계 케이스
- B4-B5 경계 케이스
- B3은 E19로 검증

첫 자동화 범위에 포함하지 않는 항목:

- 정책 미결정 항목
- DB/OCR/metadata/file storage/navigation 관련 테스트
- WPF UI/XAML 테스트
- 실제 파일 접근 테스트
- 실제 개인정보 또는 실제 기관명 기반 테스트

## 7. Deferred / Forbidden Scope

이번 결정 문서에서 아래 항목은 보류 또는 금지로 명시한다.

- test project 생성은 아직 하지 않는다.
- C# 테스트 파일 생성은 아직 하지 않는다.
- `.sln` 수정은 아직 하지 않는다.
- `.csproj` 수정은 아직 하지 않는다.
- NuGet package 추가는 아직 하지 않는다.
- `dotnet test` 실행은 아직 하지 않는다.
- DB/OCR/metadata/file storage/navigation 테스트는 범위 밖이다.
- WPF UI/XAML 테스트는 범위 밖이다.
- 실제 개인정보 샘플은 사용하지 않는다.
- 실제 가족 실명, 실제 보험사명, 실제 병원명, 실제 진단명, 실제 진단코드 기반 개인 사례는 사용하지 않는다.

## 8. Test Project Creation Impact

test project를 실제로 생성하면 다음 변경이 필요할 수 있다.

| 항목 | 예상 영향 | 승인 필요 여부 |
|---|---|---|
| `tests/FamilyClaimRef.App.Tests/` 생성 | 신규 폴더 및 test project 생성 | 필요 |
| `FamilyClaimRef.App.Tests.csproj` 생성 | test project 파일 생성 | 필요 |
| test framework package 추가 | xUnit 관련 NuGet package 추가 가능성 | 필요 |
| app project 참조 추가 | test project에서 app project 참조 | 필요 |
| `FamilyClaimRef.sln` 수정 | solution에 test project 등록 | 필요 |
| C# test file 생성 | N/E/B 케이스 자동화 | 필요 |

주의:

- production app project에는 test package를 추가하지 않는다.
- test package는 test project에만 추가하는 방향을 후보로 둔다.
- `.sln` 수정은 자동화 테스트 실행 편의를 위해 필요할 수 있지만, 별도 승인 전에는 하지 않는다.

## 9. Still Document-Based / Needs Decision

아래 항목은 자동화하지 않고 문서 기반 또는 후속 결정 대상으로 남긴다.

| 항목 | 상태 | 이유 |
|---|---|---|
| MVP 이후 extension allowlist 확장 | Needs Decision | 현재 MVP는 `pdf`, `jpg`, `jpeg`, `png`로 제한 |
| document type final list | Needs Decision | 현재 후보 상수 기준, `CategoryItem` 연결 미결정 |
| 호출자의 날짜 기준 선택 방식 | Needs Decision | service는 `yyyyMMdd` 포맷만 담당 |
| id source 선택 | Needs Decision | DB id, 화면 임시 id, document id 중 선택 필요 |
| MVP 이후 `duplicateIndex` 1000 이상 확장 | Needs Decision | 현재 MVP는 999까지만 허용 |
| DB/OCR/metadata/file storage 테스트 | Deferred | 현재 `FileNamePolicyService` 범위 밖 |
| navigation 테스트 | Deferred | 현재 구현 범위 밖 |
| WPF UI/XAML 테스트 | Deferred | 현재 자동화 첫 범위 밖 |

## 10. User Decision Needed

후속 작업 전 사용자 결정 질문은 다음과 같다.

1. test project 생성을 승인할 것인가?
2. test framework는 xUnit으로 진행해도 되는가?
3. test project 경로는 `tests/FamilyClaimRef.App.Tests/`로 할 것인가?
4. test project 이름은 `FamilyClaimRef.App.Tests`로 할 것인가?
5. `.sln`에 test project를 추가해도 되는가?
6. test project에서 app project를 참조해도 되는가?
7. NuGet package 추가를 test project에만 허용할 것인가?
8. 첫 자동화 범위는 `FileNamePolicyService`만으로 제한할 것인가?
9. N1-N7, E1-E22, B1-B2, B4-B5를 첫 자동화 범위로 승인할 것인가?
10. B3은 E19로 검증하는 기준을 승인할 것인가?
11. DB/OCR/metadata/file storage/navigation 테스트는 MVP 이후로 보류할 것인가?
12. WPF UI/XAML 테스트는 이번 자동화 범위 밖으로 둘 것인가?

## 11. Risks

남은 위험은 다음과 같다.

- test project가 아직 없으므로 자동 회귀 검증은 아직 없다.
- `dotnet test`는 아직 실행하지 않았다.
- test project 생성 시 `.sln`, test `.csproj`, NuGet package 변경이 필요할 수 있다.
- xUnit 선택은 Candidate이며 아직 사용자 확정이 아니다.
- document type final list가 바뀌면 N/E 케이스 일부도 갱신될 수 있다.
- 호출자의 날짜 기준과 id source는 service 밖 책임으로 남아 있다.
- MVP 이후 extension allowlist 또는 `duplicateIndex` 상한이 바뀌면 테스트 케이스도 갱신해야 한다.

## 12. Recommendation

다음 순서를 권장한다.

1. 이 문서를 기준으로 test project 생성 여부를 승인받는다.
2. 승인 시 test framework를 xUnit으로 확정할지 결정한다.
3. `tests/FamilyClaimRef.App.Tests/` 경로와 `FamilyClaimRef.App.Tests` 이름을 확정한다.
4. `.sln` 추가, app project 참조, NuGet package 추가 범위를 명시적으로 승인받는다.
5. 첫 자동화 범위는 `FileNamePolicyService`의 순수 함수 테스트로 제한한다.
6. DB/OCR/metadata/file storage/navigation/WPF UI 테스트는 별도 승인 전까지 보류한다.

## 13. Next Step

사용자가 승인할 경우 다음 작업 후보:

```text
FamilyClaimRef.App.Tests test project creation
```

후속 작업 전까지 다음은 진행하지 않는다.

- test project 생성
- C# 테스트 파일 생성
- `.sln` 수정
- `.csproj` 수정
- NuGet package 추가
- `dotnet test` 실행
- production code 수정
- DB/OCR/metadata 구현
- 파일 저장 또는 복사 구현
- `attachments/`, `data/local` 내부 파일 생성

## Result

`TEST_PROJECT_SCOPE_DECISION_DRAFTED`
