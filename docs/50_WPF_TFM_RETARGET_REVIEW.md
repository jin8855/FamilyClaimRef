# 50_WPF_TFM_RETARGET_REVIEW

## 1. Goal

이 문서는 WPF project의 Target Framework retarget 재시도 결과를 기록한다.

이번 작업은 retarget 전용 작업이다. `.NET 10 SDK` 확인 후 `app/FamilyClaimRef.App/FamilyClaimRef.App.csproj`의 `TargetFramework` 한 줄만 `net10.0-windows`로 변경했고, solution build를 검증했다.

기능 구현, C# 파일 생성, XAML 파일 수정, NuGet package 추가, DB/OCR/metadata 구현, 파일 저장 구현은 수행하지 않았다.

## 2. Checked Files / Paths

| 구분 | 경로 | 확인 내용 |
|---|---|---|
| Target Framework Decision | `docs/42_WPF_TARGET_FRAMEWORK_DECISION.md` | `net10.0-windows` 전환 전 SDK 확인 필요 |
| Filename Policy Scope | `docs/48_FILENAME_POLICY_IMPLEMENTATION_SCOPE_DECISION.md` | `FileNamePolicyService` 구현 전 TFM 결정 필요 |
| TFM User Decision | `docs/49_TFM_USER_DECISION_RECORD_BEFORE_FILENAME_POLICY.md` | 사용자 결정: .NET 10 전환 후 코드 구현, SDK 설치는 사용자 직접 수행 |
| Solution | `FamilyClaimRef.sln` | 수정하지 않음 |
| WPF Project | `app/FamilyClaimRef.App/FamilyClaimRef.App.csproj` | `TargetFramework` 한 줄만 변경 |

## 3. Scope

이번 작업의 허용 범위는 다음이었다.

- `dotnet --list-sdks`로 SDK 설치 여부 확인
- `.NET 10 SDK`가 있으면 `TargetFramework` 한 줄만 변경
- `dotnet build FamilyClaimRef.sln`으로 build 검증
- retarget 결과 문서 갱신

이번 작업에서 수행하지 않은 항목은 다음과 같다.

- SDK 설치
- workload 설치
- `.sln` 수정
- NuGet package 추가
- C# 파일 생성 또는 수정
- XAML 파일 생성 또는 수정
- 기능 구현
- DB/OCR/metadata 구현
- `FileNamePolicyService` 구현

## 4. Previous Attempt

이전 시도 결과는 다음과 같다.

| 항목 | 이전 결과 |
|---|---|
| SDK 확인 | `7.0.410`, `9.0.313` |
| .NET 10 SDK | 확인되지 않음 |
| Retarget | 미수행 |
| Build | 미실행 |
| Result | `BLOCKED_BY_MISSING_NET10_SDK` |

이전 이력은 삭제하지 않고 이번 재시도 결과와 분리해 보존한다.

## 5. SDK Recheck

실행 명령:

```bat
dotnet --list-sdks
```

확인 결과:

```text
7.0.410 [C:\Program Files\dotnet\sdk]
9.0.313 [C:\Program Files\dotnet\sdk]
10.0.301 [C:\Program Files\dotnet\sdk]
```

판정:

- .NET 10 SDK `10.0.301`이 확인되었다.
- retarget 진행 조건이 충족되었다.

## 6. Retarget Change

Retarget은 수행했다.

변경 전:

```xml
<TargetFramework>net9.0-windows</TargetFramework>
```

변경 후:

```xml
<TargetFramework>net10.0-windows</TargetFramework>
```

변경 범위:

- `app/FamilyClaimRef.App/FamilyClaimRef.App.csproj`의 `TargetFramework` 한 줄만 변경
- 다른 속성 변경 없음
- NuGet package 추가 없음
- `.sln`, C# 파일, XAML 파일 수정 없음

## 7. Build Result

Retarget 후 build를 실행했다.

실행 명령:

```bat
dotnet build FamilyClaimRef.sln
```

첫 sandbox 실행 결과:

- 실패
- 원인: sandbox가 `C:\Users\jin8855\AppData\Local\Microsoft SDKs` 접근을 막아 `Access to the path` 오류 발생
- 조치: 동일한 build 명령을 권한 상승으로 재실행

권한 상승 build 결과:

```text
FamilyClaimRef.App -> C:\EtcProject\FamilyClaimRef\app\FamilyClaimRef.App\bin\Debug\net10.0-windows\FamilyClaimRef.App.dll

빌드했습니다.
    경고 0개
    오류 0개
```

판정:

- Build: PASS

## 8. Modified Files

수정 파일:

- `app/FamilyClaimRef.App/FamilyClaimRef.App.csproj`
- `docs/50_WPF_TFM_RETARGET_REVIEW.md`

수정하지 않은 파일:

- `FamilyClaimRef.sln`
- 모든 C# 파일
- 모든 XAML 파일
- 다른 기존 문서
- 모든 HTML/CSS/JS 파일

허용된 build 산출물:

- `bin/`
- `obj/`

## 9. Forbidden Scope Check

금지 범위 확인 결과는 다음과 같다.

| 항목 | 결과 |
|---|---|
| SDK 설치 | 수행하지 않음 |
| workload 설치 | 수행하지 않음 |
| `.sln` 수정 | 수행하지 않음 |
| C# 파일 생성/수정 | 수행하지 않음 |
| XAML 파일 생성/수정 | 수행하지 않음 |
| NuGet package 추가 | 수행하지 않음 |
| DB 파일 생성 | 수행하지 않음 |
| OCR 파일 생성 | 수행하지 않음 |
| metadata 파일 생성 | 수행하지 않음 |
| 실제 문서 파일 생성 | 수행하지 않음 |
| `attachments/` 내부 파일 생성 | 수행하지 않음 |
| `data/local/` 내부 파일 생성 | 수행하지 않음 |
| `FileNamePolicyService` 구현 | 수행하지 않음 |
| navigation 구현 | 수행하지 않음 |

## 10. Risks

남은 위험은 다음과 같다.

- `net10.0-windows` retarget은 완료되었지만 실제 기능 구현은 아직 시작하지 않았다.
- build 과정에서 `bin/Debug/net10.0-windows`와 `obj` 산출물이 생성되었다.
- 첫 sandbox build는 SDK 위치 접근 권한 문제로 실패했으므로, 향후 build 검증에서도 동일한 권한 문제가 반복될 수 있다.
- `FileNamePolicyService` 구현은 여전히 별도 승인 작업이다.

## 11. Recommendation

권장 순서는 다음과 같다.

1. `net10.0-windows` retarget 상태를 기준으로 다음 문서를 작성한다.
2. `FileNamePolicyService` 순수 정책 함수 구현 승인 범위를 별도 문서로 확정한다.
3. 구현 시에도 파일/DB/OCR/metadata 접근 금지를 유지한다.
4. build 검증이 필요하면 SDK 위치 접근 권한 이슈를 고려한다.

## 12. Next Step

다음 작업 후보:

```text
docs/51_FILENAME_POLICY_IMPLEMENTATION_APPROVAL.md
```

후속 작업 전까지 다음은 진행하지 않는다.

- C# 기능 코드 작성
- `FileNamePolicyService` 구현
- NuGet package 추가
- DB/OCR/metadata 구현
- 파일 저장/복사 구현

## Result

`TFM_RETARGETED_TO_NET10_BUILD_PASS`

