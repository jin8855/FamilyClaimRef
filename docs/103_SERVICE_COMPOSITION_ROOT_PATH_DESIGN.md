# Service Composition / Root Path Design

## A. Goal

이 문서는 FamilyClaimRef WPF 런타임에서 서비스 그래프를 어떻게 조립할지와 production metadata root, attachment root를 어디에 둘지 결정하기 위한 설계 초안이다.

범위는 문서 설계에 한정한다. C# 구현, XAML 수정, 실제 파일 생성, 앱 실행, OpenFileDialog 실행은 포함하지 않는다.

정리 대상은 다음과 같다.

- WPF runtime에서 `DocumentRegistrationViewModel`에 필요한 서비스 그래프 구성 방식
- production metadata root 위치 후보
- production attachment root 위치 후보
- ViewModel과 MainWindow 연결 후보
- 후속 구현 전 결정이 필요한 항목

## B. Current State

현재 기준으로 다음 구성 요소는 구현되어 있다.

- `DocumentRegistrationViewModel`
- `WpfFilePickerService`
- `DocumentRegistrationWorkflow`
- `DocumentAttachmentCoordinator`
- `DocumentLinkCoordinator`
- `JsonDocumentStorageService`
- `LocalFileAttachmentService`
- lower-level model / storage / file / coordinator 계층

현재 자동화 테스트는 총 216개가 통과한 상태로 기록되어 있다.

아직 구현되지 않았거나 연결되지 않은 항목은 다음과 같다.

- runtime service composition
- XAML binding
- production root 실제 연결
- `AppServices` 같은 composition root
- App/MainWindow startup flow 연결
- Policy storage
- Claim storage

현재 App/MainWindow는 기본 상태이며, 실제 WPF 런타임에서 문서 등록 ViewModel과 하위 서비스 그래프가 연결되어 있지 않다.

## C. Problem Statement

ViewModel과 하위 서비스들은 구현되었지만, 실제 WPF 앱 실행 시 어떤 객체가 어떤 순서로 생성되고 공유되는지 아직 정해지지 않았다.

`DocumentRegistrationViewModel`은 다음 의존성이 필요하다.

- `DocumentRegistrationWorkflow`
- `IFilePickerService`

`DocumentRegistrationWorkflow`는 다음 의존성이 필요하다.

- `DocumentAttachmentCoordinator`
- `DocumentLinkCoordinator`
- `IDocumentStorageService`
- `IFileAttachmentService`

하위 서비스들은 metadata root와 attachment root가 필요하다.

결정이 필요한 핵심 문제는 다음과 같다.

- production metadata root를 project root, app base directory, user app data 중 어디에 둘 것인가
- production attachment root를 project root, app base directory, user app data 중 어디에 둘 것인가
- `attachments/`, `data/local` 실제 파일 생성 시점을 언제로 둘 것인가
- composition만 먼저 할 것인가, XAML 연결까지 같은 단계에서 할 것인가
- Policy/Claim storage가 없는 상태에서 target id 입력을 어떻게 임시 처리할 것인가

## D. Service Graph Candidate

후속 runtime composition 후보 그래프는 다음과 같다.

```text
DocumentRegistrationViewModel
 ├─ DocumentRegistrationWorkflow
 │  ├─ DocumentAttachmentCoordinator
 │  │  ├─ IDocumentStorageService -> JsonDocumentStorageService
 │  │  └─ IFileAttachmentService -> LocalFileAttachmentService
 │  ├─ DocumentLinkCoordinator
 │  │  └─ IDocumentStorageService -> JsonDocumentStorageService
 │  ├─ IDocumentStorageService -> JsonDocumentStorageService
 │  └─ IFileAttachmentService -> LocalFileAttachmentService
 └─ IFilePickerService -> WpfFilePickerService
```

결정이 필요한 사항은 다음과 같다.

- 동일한 `JsonDocumentStorageService` instance를 공유할 것인가
- 같은 root path를 사용하는 별도 `JsonDocumentStorageService` instance를 둘 것인가
- `LocalFileAttachmentService`는 동일한 attachment root를 공유할 것인가
- ViewModel이 lower-level service를 직접 생성하지 않도록 할 것인가

후보 기준으로는 ViewModel이 하위 서비스를 직접 생성하지 않고, composition root에서 그래프를 조립하는 편이 적절하다.

## E. Composition Location Candidate

### Candidate 1. `MainWindow.xaml.cs` 직접 조립

장점:

- 가장 단순하다.
- WPF 기본 구조를 크게 바꾸지 않는다.
- MVP 초기에 빠르게 연결할 수 있다.

단점:

- MainWindow code-behind가 서비스 생성 책임까지 갖는다.
- 테스트 가능한 composition 단위가 흐려진다.
- 화면이 늘어나면 MainWindow가 과도하게 커질 위험이 있다.

### Candidate 2. `App.xaml.cs`에서 조립

장점:

- 앱 startup 시점에 root path와 서비스 그래프를 한 곳에서 만들 수 있다.
- MainWindow는 ViewModel 주입 또는 DataContext 할당만 받게 할 수 있다.

단점:

- `StartupUri` 변경 또는 startup override가 필요할 수 있다.
- App startup flow 변경은 별도 검증이 필요하다.

### Candidate 3. 별도 `AppServices` factory

장점:

- manual composition root를 명확하게 둘 수 있다.
- DI container 없이도 생성 책임을 분리할 수 있다.
- unit test에서 composition 결과를 검증하기 쉽다.
- root path 계산 책임을 한 곳으로 모을 수 있다.

단점:

- 별도 파일이 추가된다.
- 관리하지 않으면 pseudo DI container처럼 커질 수 있다.

### Candidate Recommendation

후보 권장안은 별도 `AppServices` manual composition root를 두는 것이다.

단, 이 문서에서는 `AppServices.cs`를 생성하지 않는다. 후속 사용자 결정 이후 별도 구현 단계에서 생성 여부를 확정한다.

## F. Root Path Candidate

### Candidate 1. Project root

예시:

```text
C:\EtcProject\FamilyClaimRef\data\local
C:\EtcProject\FamilyClaimRef\attachments
```

장점:

- 개발 중 파일 위치를 확인하기 쉽다.
- 기존 문서의 `data/local`, `attachments` 경로와 직관적으로 맞다.

단점:

- repository root를 production runtime data로 오염시킬 수 있다.
- 권한, 백업, 배포 구조가 애매해질 수 있다.
- 실제 사용자 환경과 개발 환경의 경계가 흐려진다.

### Candidate 2. App base directory

예시:

```text
<AppBaseDirectory>\data\local
<AppBaseDirectory>\attachments
```

장점:

- 앱 배포 폴더 기준으로 상대 경로를 유지할 수 있다.
- 설치형 앱에서 구조가 단순하다.

단점:

- 설치 위치에 따라 쓰기 권한 문제가 생길 수 있다.
- app binary와 user data가 섞일 수 있다.

### Candidate 3. User app data

예시:

```text
%LOCALAPPDATA%\FamilyClaimRef\data\local
%LOCALAPPDATA%\FamilyClaimRef\attachments
```

장점:

- WPF desktop runtime의 사용자별 local data 저장 위치로 자연스럽다.
- repository root 오염을 피할 수 있다.
- 설치 위치 쓰기 권한 문제를 줄일 수 있다.
- production data와 source tree를 분리할 수 있다.

단점:

- 개발자가 파일 위치를 바로 보기 어렵다.
- manual runtime check 문서에 실제 위치를 명확히 기록해야 한다.

### Candidate Recommendation

MVP runtime production root는 user app data 기준을 후보 권장안으로 둔다.

Project root는 debug option 또는 문서 검토용 기준으로만 남기고, production runtime root로 사용하지 않는 방향을 우선 검토한다.

이 문서에서는 실제 디렉터리나 파일을 생성하지 않는다.

## G. Root Path Shape Candidate

production root 후보 구조는 다음과 같다.

```text
%LOCALAPPDATA%/FamilyClaimRef/data/local/
%LOCALAPPDATA%/FamilyClaimRef/attachments/
```

metadata 파일 후보:

```text
data/local/documents.json
data/local/policy-documents.json
data/local/claim-documents.json
```

attachment 파일 후보:

```text
attachments/documents/<physicalFileName>
```

정책 후보는 다음과 같다.

- metadata는 absolute path를 저장하지 않는다.
- `DocumentRecord.RelativePath`는 attachment-root-relative path로 유지한다.
- root path는 composition root에서만 결정한다.
- ViewModel은 root path를 알지 않는다.
- ViewModel은 file picker 결과와 registration input만 다룬다.

## H. Production File Creation Policy Candidate

production root 생성 시점 후보는 다음과 같다.

### Candidate 1. App startup에서 root directory 생성

장점:

- 앱 시작 직후 경로 오류를 빠르게 발견할 수 있다.
- runtime check가 단순하다.

단점:

- 사용자가 아무 작업을 하지 않아도 파일 시스템 흔적이 생긴다.

### Candidate 2. Constructor에서 root path만 normalize

장점:

- 객체 생성과 파일 생성의 책임을 분리할 수 있다.
- 앱 실행만으로는 디렉터리나 파일이 생기지 않는다.

단점:

- 실제 저장 시점까지 권한 문제를 발견하지 못할 수 있다.

### Candidate 3. First save/copy operation에서 필요한 directory/file 생성

장점:

- 실제 작업이 발생할 때만 파일 시스템을 변경한다.
- 현재 `JsonFileStore`, `LocalFileAttachmentService` 책임 경계와 잘 맞는다.

단점:

- 첫 작업 시점에 오류가 발생할 수 있다.

### Current Behavior Notes

- `JsonFileStore`는 save/load boundary에서 JSON 파일을 다룬다.
- `LocalFileAttachmentService`는 copy operation 시 target directory를 만든다.

### Candidate Recommendation

후보 권장안은 다음과 같다.

- App startup에서는 root path만 계산한다.
- 실제 directory/file 생성은 service operation 시점에 맡긴다.
- manual runtime check에서 root 생성 시점과 생성 위치를 별도 확인한다.

## I. MainWindow / DataContext Candidate

### Candidate 1. MainWindow 기본 생성 후 code-behind에서 DataContext 설정

장점:

- 변경 범위가 작다.
- 기본 WPF 흐름을 유지하기 쉽다.

단점:

- MainWindow가 composition 책임을 일부 가질 수 있다.

### Candidate 2. `MainWindow(DocumentRegistrationViewModel viewModel)` 생성자 추가

장점:

- MainWindow가 필요한 ViewModel을 명시적으로 받는다.
- 테스트와 추적이 쉬워진다.

단점:

- XAML designer 또는 기본 생성자 사용 흐름에 영향을 줄 수 있다.

### Candidate 3. App startup 또는 AppServices가 ViewModel을 만들고 MainWindow에 할당

장점:

- composition 책임이 App startup 또는 AppServices에 모인다.
- MainWindow는 DataContext 소비자로 남는다.

단점:

- `StartupUri` 변경 가능성이 있다.

### Candidate Recommendation

후보 권장안은 App startup 또는 AppServices가 `DocumentRegistrationViewModel`을 만들고 MainWindow에 전달하거나 DataContext로 할당하는 것이다.

이 문서는 연결 방식을 기록만 하며, MainWindow나 App 파일은 수정하지 않는다.

## J. Manual Runtime Check Candidate

후속 구현 이후 manual runtime check 후보는 다음과 같다.

- app starts without exception
- MainWindow shows
- DataContext assigned
- metadata root path computed
- attachment root path computed
- clicking file select opens OpenFileDialog
- cancel does not crash
- selected file display name appears
- registration with dummy policyId/claimId runs
- `%LOCALAPPDATA%/FamilyClaimRef` expected files created only after operation
- no project root pollution if user app data root chosen

이 문서 작성 단계에서는 앱 실행, OpenFileDialog 실행, production root 파일 생성을 수행하지 않는다.

## K. Test Scope Candidate

후속 unit test 후보는 다음과 같다.

- `AppServices` creates `DocumentRegistrationViewModel`
- `AppServices` uses configured metadata root
- `AppServices` uses configured attachment root
- ViewModel dependencies are non-null
- actual OpenFileDialog is not used in unit tests
- temp/custom root can be injected

제외 범위는 다음과 같다.

- actual WPF launch
- actual OpenFileDialog UI test
- visual automation
- production `%LOCALAPPDATA%` write test
- real user file import

## L. Needs Decision

1. service composition/root path 설계를 진행할 것인가?
2. 별도 `AppServices` manual composition root를 둘 것인가?
3. DI container를 사용하지 않고 manual factory로 둘 것인가?
4. production metadata root를 user app data 기준으로 둘 것인가?
5. production attachment root를 user app data 기준으로 둘 것인가?
6. project root `data/local`, `attachments`를 runtime production root로 쓰지 않을 것인가?
7. root path를 ViewModel이 알지 않게 할 것인가?
8. actual directory/file 생성은 service operation 시점에 맡길 것인가?
9. MainWindow DataContext 연결은 App startup 또는 composition root를 통해 둘 것인가?
10. `App.xaml`의 `StartupUri` 변경 가능성을 후속 구현 후보로 둘 것인가?
11. 후속 구현 때 `AppServices.cs`를 생성할 것인가?
12. 후속 구현 때 XAML UI는 아직 최소 연결만 할 것인가, 아니면 별도 단계로 보류할 것인가?
13. unit test에서는 actual OpenFileDialog와 production app data root를 사용하지 않을 것인가?
14. manual runtime check를 별도 기록 문서로 남길 것인가?

## M. Out of Scope

- C# 구현 없음
- `AppServices.cs` 생성 없음
- App/MainWindow 수정 없음
- XAML 수정 없음
- DataContext 연결 없음
- actual app launch 없음
- actual OpenFileDialog 실행 없음
- actual production root file 생성 없음
- ViewModel 수정 없음
- workflow/coordinator/storage/file service 수정 없음
- test code 수정 없음
- test file 생성 없음
- Policy/Claim storage 구현 없음
- OCR 구현 없음
- SQLite DB/package 추가 없음
- repository/data access/migration 구현 없음
- 실제 개인정보 샘플 없음
- Git commit/reset/checkout/add 없음

## N. Risks

- user app data path가 기존 project-root 중심 문서 표현과 다를 수 있다.
- project root를 production root로 쓰면 repository pollution과 권한 문제가 생길 수 있다.
- App startup 변경은 WPF initialization flow에 영향을 줄 수 있다.
- `AppServices`가 관리되지 않으면 pseudo DI container처럼 커질 수 있다.
- actual OpenFileDialog runtime은 unit test로 검증하기 어렵다.
- Policy/Claim storage가 없어 MVP 초기에는 target id manual input이 필요하다.

## O. Recommendation

권장 순서는 다음과 같다.

1. 이 문서를 기준으로 service composition/root path 결정을 먼저 확정한다.
2. 사용자 결정 후 `docs/104_SERVICE_COMPOSITION_ROOT_PATH_USER_DECISION_RECORD.md`를 생성한다.
3. 이후 별도 구현 단계에서 최소 `AppServices` / App startup / MainWindow DataContext 연결을 수행한다.
4. XAML UI는 최소 연결 이후 별도 layout design 단계로 분리한다.
5. 실제 앱 실행과 root file creation은 manual runtime check 문서로 따로 기록한다.

## P. Result

`SERVICE_COMPOSITION_ROOT_PATH_DESIGN_DRAFTED`
