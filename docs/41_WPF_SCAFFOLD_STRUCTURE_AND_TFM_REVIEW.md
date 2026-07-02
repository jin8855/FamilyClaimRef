# 41_WPF_SCAFFOLD_STRUCTURE_AND_TFM_REVIEW

## 1. Goal

`WPF_MINIMAL_SCAFFOLD_CREATED` 결과를 기준으로 현재 WPF scaffold 구조와 target framework 상태를 검토한다.

이번 문서는 검토 문서다. `.sln`, `.csproj`, XAML, C# 파일은 수정하지 않는다.

## 2. Checked Files / Paths

- `docs/39_WPF_STACK_AND_SCAFFOLD_SCOPE_DECISION.md`
- `docs/40_WPF_MINIMAL_SCAFFOLD_REVIEW.md`
- `FamilyClaimRef.sln`
- `app/FamilyClaimRef.App/FamilyClaimRef.App.csproj`
- `app/FamilyClaimRef.App/App.xaml`
- `app/FamilyClaimRef.App/App.xaml.cs`
- `app/FamilyClaimRef.App/MainWindow.xaml`
- `app/FamilyClaimRef.App/MainWindow.xaml.cs`
- `app/FamilyClaimRef.App/Views/`
- `app/FamilyClaimRef.App/ViewModels/`
- `app/FamilyClaimRef.App/Models/`
- `app/FamilyClaimRef.App/Services/`
- `app/FamilyClaimRef.App/Resources/`

## 3. Scope

포함 범위:

- scaffold 구조 존재 여부 확인
- WPF template 기본 파일 상태 확인
- `.csproj`의 target framework와 WPF 설정 확인
- PackageReference 추가 여부 확인
- `bin/`, `obj/` 빌드 산출물 존재 여부 확인
- 금지 범위 위반 여부 확인

제외 범위:

- `.sln` 수정
- `.csproj` 수정
- C# 파일 수정
- XAML 파일 수정
- target framework 변경
- NuGet package 추가
- DB/OCR/file service class 생성
- ViewModelBase / RelayCommand / NavigationService 구현
- HTML wireframe 변환

## 4. Scaffold Structure Check

확인된 구조:

```text
FamilyClaimRef.sln
app/FamilyClaimRef.App/
app/FamilyClaimRef.App/FamilyClaimRef.App.csproj
app/FamilyClaimRef.App/App.xaml
app/FamilyClaimRef.App/App.xaml.cs
app/FamilyClaimRef.App/AssemblyInfo.cs
app/FamilyClaimRef.App/MainWindow.xaml
app/FamilyClaimRef.App/MainWindow.xaml.cs
app/FamilyClaimRef.App/Views/
app/FamilyClaimRef.App/ViewModels/
app/FamilyClaimRef.App/Models/
app/FamilyClaimRef.App/Services/
app/FamilyClaimRef.App/Resources/
```

판정:

- 승인된 B 범위인 `빈 WPF 프로젝트 + 기본 폴더 구조` 안에 있다.
- 기본 WPF template 파일 외 기능 코드가 추가되지 않았다.
- `Views`, `ViewModels`, `Models`, `Services`, `Resources` 폴더는 비어 있다.
- sample/mock data는 확인되지 않았다.
- DB/OCR/file service class는 확인되지 않았다.
- HTML wireframe 변환 파일은 확인되지 않았다.

주의:

- `MainWindow.xaml.cs`에는 WPF template 기본 using인 `System.Windows.Navigation`이 포함되어 있다.
- 별도 `NavigationService`, `Navigate(...)`, 화면 전환 구현은 확인되지 않았다.

## 5. Project File Check

`app/FamilyClaimRef.App/FamilyClaimRef.App.csproj` 확인 결과:

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

확인 항목:

| 항목 | 값 | 판정 |
|---|---|---|
| `OutputType` | `WinExe` | WPF app template 기준 정상 |
| `TargetFramework` | `net9.0-windows` | 현재 scaffold 검증 기준 유지 가능 |
| `Nullable` | `enable` | 유지 가능 |
| `ImplicitUsings` | `enable` | 유지 가능 |
| `UseWPF` | `true` | WPF app 기준 정상 |
| `PackageReference` | 없음 | NuGet package 추가 없음 |

DB/OCR/파일 관련 package는 `.csproj`에 없다.

## 6. Target Framework Review

현재 target framework:

```text
net9.0-windows
```

현재 설치 SDK:

```text
7.0.410
9.0.313
```

검토:

- 현재 scaffold는 설치된 .NET SDK 9.0.313 기준으로 생성된 `net9.0-windows` WPF project다.
- `dotnet build FamilyClaimRef.sln`은 `net9.0-windows` 상태에서 통과한 기록이 있다.
- 현재 단계는 scaffold 검토 단계이므로 임의 retarget은 하지 않는다.
- 실제 기능 구현 전에는 장기 지원 관점에서 .NET 10 LTS 전환 여부를 사용자에게 확인하는 것이 좋다.
- .NET 10 LTS로 전환하려면 사용자의 SDK 설치/업데이트 승인과 `.csproj` 변경 승인이 필요하다.
- Codex가 SDK 설치나 retarget을 임의 수행하지 않는다.

판정:

- 현재 scaffold 검증: `KEEP_NET9_TEMPORARILY`
- 실제 구현 전 검토: `RECOMMEND_NET10_LTS_BEFORE_REAL_IMPLEMENTATION`
- 최종 TFM 선택: `NEEDS_USER_DECISION_ON_TFM`

## 7. Build Artifact Check

확인된 빌드 산출물:

- `app/FamilyClaimRef.App/bin/`
- `app/FamilyClaimRef.App/obj/`

판정:

- `bin/`과 `obj/`는 이전 `dotnet build FamilyClaimRef.sln` 검증 과정에서 생성된 빌드 산출물이다.
- 이번 검토 작업에서는 삭제하지 않는다.
- Git 저장소가 아니므로 현재 Git 추적 상태는 검증할 수 없다.
- 추후 Git 적용 시 `bin/`, `obj/` 제외 정책이 필요하다.

주의:

- `obj/` 내부에는 NuGet restore 산출물과 generated build files가 있다.
- 이는 WPF build 산출물이며 수작업 구현 파일로 보지 않는다.

## 8. Forbidden Scope Check

확인 결과:

- DB 파일 없음
- OCR 관련 구현 파일 없음
- sample/mock data 없음
- 실제 개인정보 샘플 없음
- `PackageReference` 추가 없음
- `package.json` 없음
- `tsconfig.json` 없음
- `src/` 폴더 없음
- JavaScript/TypeScript 파일 없음
- `ViewModelBase` 구현 없음
- `RelayCommand` 구현 없음
- `NavigationService` 구현 없음
- model/service class 구현 없음
- XAML 상세 화면 구현 없음
- HTML wireframe 변환 없음

보충:

- `MainWindow.xaml`은 WPF template 기본 빈 `Grid` 상태다.
- `MainWindow.xaml.cs`는 template 기본 생성자와 `InitializeComponent()`만 포함한다.

## 9. Risks

- `net9.0-windows`는 현재 scaffold 검증에는 충분하지만, 실제 구현 전 장기 지원 TFM 결정이 필요하다.
- .NET 10 LTS 전환을 선택하면 SDK 설치/업데이트와 `.csproj` retarget이 별도 작업으로 필요하다.
- `bin/`, `obj/`가 생성되어 있으므로 Git 적용 전 제외 정책을 확인해야 한다.
- 현재 폴더 구조는 비어 있으므로 MVVM 구현 기준은 아직 없다.
- scaffold 구조가 있다고 해서 DB/OCR/파일 저장 정책이 결정된 것은 아니다.

## 10. Recommendation

현재 scaffold 구조는 승인된 최소 범위 안에 있다.

권장:

1. 현재 상태에서는 `net9.0-windows`를 임시 유지한다.
2. 실제 기능 코드 작성 전 `TargetFramework`를 유지할지, .NET 10 LTS로 전환할지 사용자 결정 문서를 작성한다.
3. Git 적용 전에 `bin/`, `obj/` 제외 정책을 확인한다.
4. 다음 코드 작업 전에 MVVM 최소 구조 설계 문서를 먼저 작성한다.
5. DB/OCR/파일 저장 구현은 별도 승인 전까지 시작하지 않는다.

## 11. Next Step

다음 후보:

- `TargetFramework` 결정 문서 작성
- WPF MVVM 최소 구조 설계 문서 작성
- `bin/`, `obj` Git 제외 정책 확인
- 파일 저장 정책 decision 문서 작성
- DB 설계 전 decision 문서 작성
- OCR 설계 전 decision 문서 작성

기능 구현은 사용자 별도 승인 전까지 진행하지 않는다.

## Result

WPF_SCAFFOLD_STRUCTURE_READY
