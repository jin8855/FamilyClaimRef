# 49_TFM_USER_DECISION_RECORD_BEFORE_FILENAME_POLICY

## 1. Goal

이 문서는 `FileNamePolicyService` 구현 전에 필요한 Target Framework 사용자 결정값을 기록한다.

이번 작업은 사용자 결정 기록 문서 작성이다. `.csproj` 수정, `.sln` 수정, SDK 설치, target framework 변경, C# 파일 생성, XAML 파일 생성, NuGet package 추가, build 설정 변경은 수행하지 않는다.

## 2. Checked Files / Paths

| 구분 | 경로 | 확인 내용 |
|---|---|---|
| Target Framework Decision | `docs/42_WPF_TARGET_FRAMEWORK_DECISION.md` | `net9.0-windows` 현 상태, `.NET 10 SDK` 필요 후보, retarget 조건 |
| Filename Policy Scope | `docs/48_FILENAME_POLICY_IMPLEMENTATION_SCOPE_DECISION.md` | `FileNamePolicyService` 구현 전 TFM 결정 필요 |
| WPF Project | `app/FamilyClaimRef.App/FamilyClaimRef.App.csproj` | 현재 `TargetFramework` 확인, 수정하지 않음 |
| Solution | `FamilyClaimRef.sln` | 수정하지 않음 |
| SDK Check | `dotnet --list-sdks` | 설치 SDK 목록 확인, 설치하지 않음 |

## 3. Scope

이 문서의 범위는 다음으로 제한한다.

- TFM 사용자 결정값 기록
- 현재 `TargetFramework`와 SDK 상태 기록
- 아직 수행하지 않은 작업 분리
- retarget 전 조건 정리
- 후속 retarget 지시의 허용 범위와 금지 범위 정리
- 사용자 할 일 정리

범위 밖 항목은 다음과 같다.

- `.csproj` retarget
- `.sln` 수정
- SDK 설치
- workload 설치
- build 설정 변경
- C# 파일 생성 또는 수정
- XAML 파일 생성 또는 수정
- NuGet package 추가
- `FileNamePolicyService` 구현
- DB/OCR/metadata 구현

## 4. Current State

현재 상태는 다음과 같다.

| 항목 | 상태 |
|---|---|
| 현재 `TargetFramework` | `net9.0-windows` |
| 현재 확인된 SDK | `7.0.410`, `9.0.313` |
| 현재 확인 결과 .NET 10 SDK | 없음 |
| 현재 WPF scaffold build | `net9.0-windows`에서 PASS 이력 있음 |
| 현재 코드 구현 | 아직 시작하지 않음 |
| 현재 retarget | 수행하지 않음 |

현재 `.csproj` 기준:

```xml
<TargetFramework>net9.0-windows</TargetFramework>
```

현재 SDK 확인 결과:

```text
7.0.410
9.0.313
```

## 5. User Decision

사용자 결정값은 다음과 같다.

| 질문 | 사용자 결정 | 판정 |
|---|---|---|
| Q1 TFM 선택 | A. `net10.0-windows` 전환 후 코드 구현 | Accepted |
| Q2 .NET 10 SDK 설치/확인 | B. 설치 필요 시 사용자가 직접 설치 | Accepted |
| Q3 retarget 범위 | B. retarget + build 검증 | Accepted |

Accepted Decision:

- 실제 C# 기능 코드 구현 전 `net10.0-windows`로 전환하는 방향을 승인한다.
- .NET 10 SDK 설치는 사용자가 직접 수행한다.
- Codex는 SDK 설치를 하지 않는다.
- SDK 설치 또는 확인 후 별도 retarget 작업에서 `.csproj`의 `TargetFramework`를 `net10.0-windows`로 변경하고 build 검증한다.
- `FileNamePolicyService` 구현은 retarget 이후로 미룬다.

## 6. Not Yet Performed

다음 작업은 아직 수행하지 않았다.

- SDK 설치
- `.csproj` retarget
- build 재검증
- C# 파일 생성
- XAML 파일 수정
- NuGet package 추가
- DB 구현
- OCR 구현
- metadata 구현
- `FileNamePolicyService` 구현

## 7. Conditions Before Retarget

retarget 전 조건은 다음과 같다.

- 사용자가 .NET 10 SDK를 설치했거나 설치 여부를 확인해야 한다.
- Codex는 `dotnet --list-sdks`로 설치 여부 확인만 수행할 수 있다.
- .NET 10 SDK가 없으면 retarget하지 않는다.
- SDK 확인 후에만 `.csproj` 변경이 가능하다.
- 변경 범위는 `TargetFramework`만으로 제한한다.
- 변경 후 `dotnet build FamilyClaimRef.sln`을 실행한다.
- build 실패 시 임의 패키지 추가나 설정 변경을 하지 않는다.
- build 실패 분석 문서는 별도 작업으로 분리한다.

## 8. Allowed Retarget Scope Later

후속 retarget 전용 작업에서 허용 가능한 범위는 다음과 같다.

- `dotnet --list-sdks`
- `app/FamilyClaimRef.App/FamilyClaimRef.App.csproj`의 `TargetFramework`를 `net9.0-windows`에서 `net10.0-windows`로 변경
- `dotnet build FamilyClaimRef.sln`
- `docs/50_WPF_TFM_RETARGET_REVIEW.md` 생성

허용 범위의 의미:

- retarget 작업은 TFM 변경과 build 검증만 다룬다.
- 기능 구현은 포함하지 않는다.
- 패키지 추가는 포함하지 않는다.

## 9. Forbidden Scope Later

후속 retarget 전용 작업에서도 아래 항목은 금지한다.

- SDK 설치
- workload 설치
- NuGet package 추가
- C# 파일 생성 또는 수정
- XAML 파일 생성 또는 수정
- DB 파일 생성
- OCR 파일 생성
- metadata 파일 생성
- 기능 구현
- scaffold 구조 변경
- `FileNamePolicyService` 구현
- `ViewModelBase`, `RelayCommand`, `NavigationService` 구현
- `attachments/` 내부 파일 생성
- `data/local/` 내부 파일 생성

## 10. User Action Required

사용자에게 필요한 작업은 다음과 같다.

```text
1. .NET 10 SDK를 직접 설치하거나 설치 여부를 확인한다.
2. 설치 후 Codex에게 retarget 전용 지시를 실행시킨다.
3. retarget이 성공하면 FileNamePolicyService 순수 정책 함수 구현 승인으로 넘어간다.
```

사용자가 확인해야 할 사항:

- .NET 10 SDK 설치 여부
- `net10.0-windows`를 실제 구현 기준으로 사용할지 최종 확인
- retarget 작업을 기능 구현과 분리할지 확인

## 11. Risks

남은 위험은 다음과 같다.

- .NET 10 SDK가 없는 상태에서 `.csproj`만 retarget하면 build가 실패할 수 있다.
- retarget과 기능 구현을 한 번에 진행하면 실패 원인을 분리하기 어렵다.
- build 실패 시 임의 패키지 추가나 설정 변경을 하면 retarget 범위가 넓어진다.
- `FileNamePolicyService` 구현을 retarget 전에 시작하면 이후 TFM 변경 시 재검증 범위가 커진다.
- .NET 10 SDK 설치는 사용자 환경 변경이므로 Codex가 수행하지 않는다.

## 12. Recommendation

권장 순서는 다음과 같다.

1. 사용자가 .NET 10 SDK를 직접 설치하거나 설치 여부를 확인한다.
2. Codex는 `dotnet --list-sdks`로 설치 여부만 확인한다.
3. .NET 10 SDK가 확인되면 retarget 전용 작업을 수행한다.
4. retarget 작업은 `.csproj`의 `TargetFramework` 변경과 `dotnet build FamilyClaimRef.sln` 검증으로 제한한다.
5. retarget 결과는 `docs/50_WPF_TFM_RETARGET_REVIEW.md`에 기록한다.
6. retarget 성공 후에만 `FileNamePolicyService` 순수 정책 함수 구현 승인으로 넘어간다.

현재 권장 판정:

- TFM 사용자 결정은 기록되었다.
- 아직 retarget은 수행하지 않는다.
- `FileNamePolicyService` 구현도 아직 수행하지 않는다.

## 13. Next Step

다음 작업 후보:

```text
docs/50_WPF_TFM_RETARGET_REVIEW.md
```

단, 이 작업은 사용자가 .NET 10 SDK 설치 또는 설치 여부 확인을 마친 뒤 retarget 전용 지시를 줄 때만 진행한다.

후속 retarget 전용 지시 전에는 다음을 수행하지 않는다.

- `.csproj` 수정
- `.sln` 수정
- SDK 설치
- build 설정 변경
- C# 파일 생성
- XAML 파일 생성
- `FileNamePolicyService` 구현

## Result

`TFM_USER_DECISION_RECORDED`

