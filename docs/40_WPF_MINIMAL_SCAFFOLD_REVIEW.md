# 40_WPF_MINIMAL_SCAFFOLD_REVIEW

## 1. Goal

사용자가 승인한 최소 범위 안에서 WPF app scaffold를 생성한 결과를 기록한다.

이번 작업은 WPF solution / project와 기본 폴더 구조 생성까지만 수행했다. DB, OCR, 파일 저장, navigation, 실제 화면, sample/mock data는 구현하지 않았다.

## 2. Approved Scope

사용자 승인 사항:

```text
Q1 WPF 승인: A. WPF를 MVP 1차 스택으로 승인
Q2 Windows 전용: A. MVP는 Windows 전용
Q3 MVVM 구조: A. 최소 MVVM 구조
Q4 scaffold 범위: B. 빈 WPF 프로젝트 + 기본 폴더 구조까지만
```

허용 생성 범위:

- `FamilyClaimRef.sln`
- `app/FamilyClaimRef.App/`
- WPF template 기본 파일
- 최소 MVVM 후보 폴더 구조

명시적 제외 범위:

- DB 생성
- OCR 구현
- 파일 저장 구현
- navigation 구현
- 실제 화면 구현
- HTML wireframe 변환
- sample/mock data 생성

## 3. Created Files / Folders

생성된 주요 scaffold:

- `FamilyClaimRef.sln`
- `app/`
- `app/FamilyClaimRef.App/`
- `app/FamilyClaimRef.App/FamilyClaimRef.App.csproj`
- `app/FamilyClaimRef.App/App.xaml`
- `app/FamilyClaimRef.App/App.xaml.cs`
- `app/FamilyClaimRef.App/AssemblyInfo.cs`
- `app/FamilyClaimRef.App/MainWindow.xaml`
- `app/FamilyClaimRef.App/MainWindow.xaml.cs`
- `app/FamilyClaimRef.App/Views/`
- `app/FamilyClaimRef.App/ViewModels/`
- `app/FamilyClaimRef.App/Models/`
- `app/FamilyClaimRef.App/Services/`
- `app/FamilyClaimRef.App/Resources/`

빌드 검증으로 생성된 산출물:

- `app/FamilyClaimRef.App/bin/`
- `app/FamilyClaimRef.App/obj/`

주의:

- `bin/`과 `obj/`는 `dotnet build FamilyClaimRef.sln` 검증 과정에서 생성된 빌드 산출물이다.
- 기능 코드, sample data, mock data, DB/OCR/file service class는 생성하지 않았다.

## 4. Modified Existing Files

없음.

이번 작업 전에는 `FamilyClaimRef.sln`과 `app/FamilyClaimRef.App/`가 없었다. 생성된 solution과 project는 이번 scaffold 범위 안의 신규 파일이다.

기존 기준 문서와 HTML/CSS/JS 파일은 수정하지 않았다.

## 5. Build Result

사전 확인:

- .NET SDK 확인: 성공
  - `7.0.410`
  - `9.0.313`
- WPF template 확인: 성공
  - `dotnet new list wpf`에서 `WPF 애플리케이션` template 확인

생성 및 연결:

- `dotnet new sln -n FamilyClaimRef`: 성공
- `dotnet new wpf -n FamilyClaimRef.App -o app\FamilyClaimRef.App`: template 파일 생성 성공
- template 생성 직후 restore는 sandbox 내부에서 `C:\Users\jin8855\AppData\Local\Microsoft SDKs` 접근 거부로 실패했다.
- `dotnet sln FamilyClaimRef.sln add app\FamilyClaimRef.App\FamilyClaimRef.App.csproj`: 권한 상승 후 성공

빌드 검증:

```text
dotnet build FamilyClaimRef.sln
```

결과:

```text
빌드했습니다.
경고 0개
오류 0개
```

판정:

- `PASS`

## 6. Forbidden Scope Check

확인 결과:

- `package.json` 미생성
- `tsconfig.json` 미생성
- `src/` 폴더 미생성
- DB 파일 미생성
- OCR 관련 파일 미생성
- `attachments/` 내부 파일 없음
- `data/local/` 내부 파일 없음
- JavaScript/TypeScript 파일 미생성
- 실제 개인정보 샘플 없음
- sample/mock data 없음
- navigation 구현 없음
- XAML 상세 화면 구현 없음
- HTML wireframe 변환 없음
- NuGet package 추가 없음

## 7. Excluded Items

이번 scaffold에서 제외한 항목:

- DB 설계 및 DB 파일
- SQLite 등 저장소 확정
- OCR 엔진 연동
- OCR 실행
- OCR 원문 저장
- 파일 업로드/복사/저장 구현
- `attachments/` 파일 생성
- `data/local/` 파일 생성
- 실제 보험 청구 로직
- `ClaimReferenceResult` 계산 구현
- `HistoryItem` projection 구현
- ViewModelBase, RelayCommand, NavigationService 구현
- 실제 model class 구현
- service class 구현
- sample/mock data
- HTML wireframe의 WPF 변환

## 8. Risks

- WPF template 기본 `MainWindow.xaml`은 빈 shell 수준이므로 실제 V5.5 화면 구조를 반영하지 않는다.
- `bin/`과 `obj/`는 빌드 검증 과정에서 생성되었으므로 추후 Git 추적 정책 확인이 필요하다.
- SDK 9.0.313 기준으로 `net9.0-windows` WPF project가 생성되었다. 장기 지원 대상 프레임워크를 별도 결정할 필요가 있다.
- scaffold만 생성되었으므로 DB/OCR/파일 저장 정책은 여전히 미결정이다.
- 최소 MVVM 폴더만 만들었고 실제 MVVM base class나 command 구현은 없다.

## 9. Recommendation

다음 작업은 기능 구현이 아니라 scaffold 구조 검토 또는 설계 문서화로 제한한다.

권장 순서:

1. WPF scaffold 구조 검토
2. WPF MVVM 최소 구조 설계 문서 작성
3. 파일 저장 정책 decision 문서 작성
4. DB 설계 전 decision 문서 작성
5. OCR 설계 전 decision 문서 작성

바로 DB/OCR 구현이나 화면 navigation 구현으로 넘어가지 않는다.

## 10. Next Step

후속 후보:

- `WPF MVVM 최소 구조 설계 문서` 작성
- `파일 저장 정책 decision 문서` 작성
- `DB 설계 전 decision 문서` 작성
- `OCR 설계 전 decision 문서` 작성

기능 구현은 사용자의 별도 승인 전까지 진행하지 않는다.

## Result

WPF_MINIMAL_SCAFFOLD_CREATED
