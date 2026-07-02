# 42_WPF_TARGET_FRAMEWORK_DECISION

## 1. Goal

현재 WPF scaffold의 `TargetFramework`가 `net9.0-windows`인 상태를 기준으로, 실제 기능 구현 전에 TFM을 유지할지 .NET 10 LTS로 전환할지 판단할 수 있도록 선택지를 정리한다.

이 문서는 decision 문서다. `.csproj` 수정, SDK 설치, retarget, build 설정 변경, C# / XAML 코드 수정은 수행하지 않는다.

## 2. Checked Files / Paths

- `docs/38_DESKTOP_TECH_STACK_COMPARISON.md`
- `docs/39_WPF_STACK_AND_SCAFFOLD_SCOPE_DECISION.md`
- `docs/40_WPF_MINIMAL_SCAFFOLD_REVIEW.md`
- `docs/41_WPF_SCAFFOLD_STRUCTURE_AND_TFM_REVIEW.md`
- `FamilyClaimRef.sln`
- `app/FamilyClaimRef.App/FamilyClaimRef.App.csproj`

## 3. Scope

포함 범위:

- 현재 WPF project TFM 확인
- 현재 설치 SDK 확인
- `net9.0-windows` 유지 선택지 검토
- `net10.0-windows` 전환 선택지 검토
- TFM 결정 보류 선택지 검토
- 사용자 결정 질문 작성

제외 범위:

- `.csproj` 수정
- `.sln` 수정
- SDK 설치
- workload 설치
- target framework 변경
- NuGet package 추가
- C# / XAML 코드 수정
- DB/OCR/file service 구현
- sample/mock data 생성

## 4. Current Project TFM

현재 `app/FamilyClaimRef.App/FamilyClaimRef.App.csproj` 상태:

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>WinExe</OutputType>
    <TargetFramework>net9.0-windows</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <UseWPF>true</UseWPF>
  </PropertyGroup>

</Project>
```

확인 결과:

| 항목 | 현재 값 | 판정 |
|---|---|---|
| `TargetFramework` | `net9.0-windows` | 현재 scaffold 검증 기준 유지 가능 |
| `UseWPF` | `true` | WPF app 기준 정상 |
| `Nullable` | `enable` | 유지 가능 |
| `ImplicitUsings` | `enable` | 유지 가능 |
| `PackageReference` | 없음 | NuGet package 추가 없음 |

현재 `.csproj`는 수정하지 않았다.

## 5. Current SDK State

확인 명령:

```text
dotnet --list-sdks
```

현재 SDK:

```text
7.0.410 [C:\Program Files\dotnet\sdk]
9.0.313 [C:\Program Files\dotnet\sdk]
```

판정:

- 현재 환경에는 .NET 9 SDK가 설치되어 있다.
- 현재 확인 결과에는 .NET 10 SDK가 없다.
- SDK 설치는 시도하지 않았다.
- 인터넷 다운로드는 시도하지 않았다.
- .NET 10으로 전환하려면 사용자 승인 후 SDK 설치/확인 절차가 별도로 필요하다.

## 6. Option A: Keep net9.0-windows

선택:

```text
TargetFramework = net9.0-windows
```

장점:

- 현재 scaffold build가 이미 통과했다.
- 현재 설치된 SDK 9.0.313으로 계속 검토할 수 있다.
- 추가 SDK 설치가 필요 없다.
- `.csproj` retarget 작업이 없다.
- WPF scaffold 구조 검토, MVVM 설계 문서 작성, 파일 저장 정책 문서 작성은 계속 진행할 수 있다.

단점:

- 장기 구현 기준에서는 LTS보다 안정성 판단이 약하다.
- 실제 구현 기간이 길어지면 지원 기간과 유지보수 기준을 다시 검토해야 한다.
- DB/OCR/파일 저장/패키지 선택을 시작한 뒤 전환하면 retarget 비용이 커질 수 있다.

판정:

- scaffold 검증/임시 유지에는 가능
- 실제 기능 구현 장기 기준으로는 비추천
- 상태: `KEEP_NET9_TEMPORARILY`

## 7. Option B: Move to net10.0-windows

선택:

```text
TargetFramework = net10.0-windows
```

장점:

- LTS 기준으로 장기 개발에 더 적합하다.
- 실제 기능 구현 전에 전환하면 변경 비용이 작다.
- 이후 DB/OCR/파일 저장/패키지 선택 기준을 장기 지원 버전에 맞출 수 있다.
- MVP가 장기 유지될 가능성이 있으면 안정적인 기준선이 된다.

단점:

- 현재 SDK 목록에는 .NET 10 SDK가 없다.
- SDK 설치 또는 업데이트가 필요할 수 있다.
- `.csproj` retarget 작업이 필요하다.
- retarget 후 `dotnet build FamilyClaimRef.sln` 검증이 별도로 필요하다.
- Codex가 임의로 SDK 설치나 retarget을 수행할 수 없다.

판정:

- 실제 기능 구현 전 권장
- 사용자 승인과 SDK 설치/확인 필요
- 상태: `RECOMMEND_NET10_LTS_BEFORE_REAL_IMPLEMENTATION`

## 8. Option C: Defer TFM Decision

선택:

```text
TFM 결정을 기능 구현 전까지 보류
```

장점:

- 지금은 코드 수정 없이 문서 작업을 이어갈 수 있다.
- WPF MVVM 최소 구조 설계 문서, 파일 저장 정책 decision 문서, DB/OCR decision 문서는 계속 작성할 수 있다.
- SDK 설치나 retarget을 즉시 수행하지 않아도 된다.

단점:

- 첫 기능 C# 코드 작성 전에는 반드시 결정해야 한다.
- 너무 늦게 결정하면 `.csproj`, package, build, test 기준 변경 비용이 커진다.
- DB/OCR/파일 저장 관련 package 검토 전에 TFM이 흔들리면 설계 판단이 반복될 수 있다.

판정:

- MVVM 설계 문서 작성까지는 보류 가능
- 실제 C# 코드 작성 전에는 결정 필요
- 상태: `NEEDS_USER_DECISION_ON_TFM`

## 9. Comparison Matrix

| 기준 | A. `net9.0-windows` 유지 | B. `net10.0-windows` 전환 | C. 보류 |
|---|---|---|---|
| 현재 SDK로 가능 | 가능 | 현재 목록 기준 불가, SDK 필요 | 가능 |
| 현재 scaffold build 기준 | 이미 검증됨 | retarget 후 재검증 필요 | 기존 상태 유지 |
| `.csproj` 수정 | 없음 | 필요 | 없음 |
| SDK 설치/확인 | 불필요 | 필요 후보 | 나중에 필요 |
| 장기 유지보수 | 보통 | 유리 | 미정 |
| 실제 구현 전 적합성 | 임시 가능 | 권장 | 설계 문서까지 가능 |
| 리스크 | 장기 지원 기준 약함 | 설치/전환 작업 필요 | 결정 지연 비용 |

## 10. Recommendation

권장 판단:

- 현재 scaffold 검증 상태는 `net9.0-windows`로 유지 가능하다.
- WPF MVVM 최소 구조 설계 문서와 파일 저장 정책 decision 문서까지는 현재 TFM으로 계속 진행해도 된다.
- 실제 기능 C# 코드 작성 전에는 `net10.0-windows` 전환 여부를 사용자에게 확인한다.
- 장기 구현을 시작하기 전에는 .NET 10 LTS 전환을 권장한다.
- 단, Codex는 사용자 승인 없이 SDK 설치, workload 설치, retarget, build 설정 변경을 수행하지 않는다.

권장 선택:

```text
Q1 TFM 선택: B 또는 C
Q2 .NET 10 SDK 설치/확인: B 또는 C
Q3 retarget 범위: B 또는 C
```

실무적으로는 다음 순서가 안전하다.

1. MVVM 최소 구조 설계 문서까지는 현재 `net9.0-windows` 유지
2. 첫 기능 코드 작성 전 .NET 10 SDK 설치 여부 확인
3. 사용자 승인 후 `.csproj` retarget
4. retarget 후 `dotnet build FamilyClaimRef.sln` 검증

## 11. User Decision Questions

아래 형식으로 사용자가 답변하면 다음 작업 범위를 결정할 수 있다.

```text
Q1 TFM 선택:
- A. net9.0-windows 유지
- B. net10.0-windows 전환
- C. 기능 구현 전까지 보류

Q2 .NET 10 SDK 설치/확인:
- A. 이미 설치되어 있으면 확인만
- B. 설치 필요 시 사용자가 직접 설치
- C. 설치/전환 작업 보류

Q3 retarget 범위:
- A. .csproj TargetFramework만 net10.0-windows로 변경
- B. retarget + build 검증
- C. 보류
```

권장 답변 후보:

```text
Q1 TFM 선택: C
Q2 .NET 10 SDK 설치/확인: C
Q3 retarget 범위: C

추가 조건:
- MVVM 설계 문서까지는 현재 TFM 유지
- 첫 기능 코드 작성 전 TFM 재결정
```

장기 구현 착수 직전 권장 답변 후보:

```text
Q1 TFM 선택: B
Q2 .NET 10 SDK 설치/확인: B
Q3 retarget 범위: B

추가 조건:
- SDK 설치는 사용자가 직접 수행
- Codex는 설치 확인 후 .csproj retarget과 build 검증만 수행
```

## 12. Risks

- `net9.0-windows`를 장기 구현 기준으로 고정하면 LTS 기준보다 유지보수 판단이 약해질 수 있다.
- `net10.0-windows`로 전환하려면 현재 환경에 없는 SDK가 필요할 수 있다.
- SDK 설치 없이 `.csproj`만 retarget하면 build가 실패할 수 있다.
- DB/OCR/파일 저장 package를 고른 뒤 TFM을 바꾸면 재검토 범위가 커진다.
- TFM 결정을 너무 늦추면 첫 기능 코드 작성 시점에 일정 지연이 생길 수 있다.

## 13. Next Step

다음 후보:

- WPF MVVM 최소 구조 설계 문서 작성
- TFM 사용자 답변 수집
- .NET 10 SDK 설치 여부를 사용자가 직접 확인
- 사용자 승인 후 retarget 전용 작업 지시 작성
- 파일 저장 정책 decision 문서 작성

기능 구현, DB/OCR 구현, package 추가는 별도 승인 전까지 진행하지 않는다.

## Result

TFM_DECISION_READY
