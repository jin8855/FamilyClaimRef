# Family Insurance Persistence T3-PER-A Implementation and Evidence

## A. Status

`FAMILYCLAIMREF_T3_PER_A_INSURANCE_PERSISTENCE_IMPLEMENTED_EVIDENCE_PASS`

- `T3_PER_A_IMPLEMENTATION_STATE = IMPLEMENTED`
- `T3_PER_A_AUTOMATED_VALIDATION_STATE = PASS`
- `T3_PER_A_INDEPENDENT_REVIEW_STATE = PASS`
- `T3_PER_A_USER_RUNTIME_STATE = READY_FOR_REVIEW`
- `T3_PER_A_COMMIT_STATE = NOT_CREATED`
- `T3_PER_C_D_STATE = NOT_STARTED`
- `PRODUCTION_READINESS_STATE = NOT_EVALUATED`
- `DEPLOYMENT_STATE = NOT_AUTHORIZED`

## B. Baseline

- Repository: `C:\EtcProject\FamilyClaimRef`
- Branch: `main`
- HEAD: `65b557c181b395a440210d7ca61eda85d34c4189`
- Parent: `aecf7edfd43b4124ec5ff17d35687020cf4c0d90`
- 시작 staged path: `0`
- 시작 시 기존 dirty path: `docs/00_PROJECT_BASE_GUIDE.md` 1개
- `docs/00_PROJECT_BASE_GUIDE.md` 시작 SHA-256: `c2decac7d8f833417f331af010115a6a49b769ba3352e31401a53db0982acf87`
- 보호 문서는 이 작업에서 수정, stage, 복구하지 않았다.

## C. Canonical Seven-Field Contract

| No. | Canonical field | Type | Required | Persisted JSON key | Screen 11/12 behavior |
|---:|---|---|---|---|---|
| 1 | `DisplayTitle` | `string` | yes | `displayTitle` | 목록 및 편집 제목 |
| 2 | `FamilyMemberId` | `string` | yes | `familyMemberId` | active 가족 선택 및 표시명 projection |
| 3 | `InsurerName` | `string` | yes | `insurerName` | 보험사 |
| 4 | `ContractStatus` | `string` | yes | `contractStatus` | 계약 상태 |
| 5 | `EnrollmentDate` | `DateOnly` | yes | `enrollmentDate` | 가입일 |
| 6 | `CoveragePeriod` | `string` | yes | `coveragePeriod` | 보험기간 |
| 7 | `RegistrationSource` | `string` | yes | `registrationSource` | 등록 출처 |

`FamilyMemberId`는 7개 업무 필드에 포함되며 별도의 여덟 번째 필드가 아니다. 문자열은 trim하고 공백 값은 거부하며 `EnrollmentDate`의 default 값은 거부한다. 현재 권위 자료에 없는 최대 길이, enum 후보, 보험번호, uniqueness 규칙은 추가하지 않았다.

기존 `ReferenceDate`는 JSON 하위 호환을 위해 유지한다. 신규 보험 저장과 수정에서는 `EnrollmentDate`와 같은 값으로 기록하지만 7개 업무 필드 수에는 포함하지 않는다.

## D. FamilyMemberId Reference Integrity

- 신규 보험은 존재하는 active `FamilyMemberId`만 저장할 수 있다.
- 기존 보험이 동일한 inactive 가족을 참조하는 경우 해당 참조를 유지한 수정은 허용한다.
- 가족 참조를 변경하는 경우 새 대상은 반드시 active 상태여야 한다.
- 가족 사용 중지는 보험 레코드를 삭제, null 변환, 자동 재연결하지 않는다.
- 가족 다시 사용은 동일 `FamilyMemberId`를 복원하므로 보험 연결도 그대로 유지된다.
- orphan 참조는 읽을 때 삭제하거나 재작성하지 않는다.
- orphan 보험을 편집할 때 raw ID를 표시하지 않고 유효한 가족을 명시적으로 다시 선택해야 저장할 수 있다.
- 참조 검증은 UI뿐 아니라 `JsonPolicyClaimStorageService` 저장 경계에서도 수행한다.

## E. Implementation Result

- 화면 11은 active 보험 목록에 가족 표시명, 보험사, 계약 상태, 가입일을 표시한다.
- 화면 12는 7개 필드의 create/edit 입력과 validation, 저장, 닫기를 제공한다.
- create/edit 저장 성공 후 화면 11로 돌아가며 최신 active 목록을 다시 표시한다.
- 보험 전용 새로고침은 기존 `AvailablePolicies`, `SelectedPolicyId`, `SelectedPolicyForClaimId`도 함께 보정한다.
- 보험 사용 중지는 active 청구 건이 있으면 기존 계약대로 차단한다.
- `DisplayTitle`은 identity가 아니므로 같은 표시명의 여러 보험 레코드를 허용한다.
- 저장 실패 시 화면 상태를 유지하고 제품용 안전 메시지를 사용한다. 경로, 내부 ID, stack trace, 예외 전문은 표시하지 않는다.
- 화면 11/12에 남아 있던 과거의 미등록/미영속화 안내 영역은 실제 구현과 모순되어 제거했다.
- 기존 Product 21-screen route, document registration, FamilyMember lifecycle, MainWindow 기본 생성 경로는 유지했다.

## F. Exact Dependency Closure

### CREATE (7)

- `app/FamilyClaimRef.App/Models/Storage/InsurancePolicyDraft.cs`
- `app/FamilyClaimRef.App/ViewModels/InsurancePolicyListItemViewModel.cs`
- `app/FamilyClaimRef.App/Views/ProductInsurancePolicyEditorView.xaml`
- `app/FamilyClaimRef.App/Views/ProductInsurancePolicyEditorView.xaml.cs`
- `tests/FamilyClaimRef.App.Tests/InsurancePolicyManagementViewModelTests.cs`
- `tests/FamilyClaimRef.App.Tests/InsurancePolicyPersistenceTests.cs`
- `docs/442_FAMILY_INSURANCE_PERSISTENCE_T3_PER_A_IMPLEMENTATION_AND_EVIDENCE.md`

### MODIFY (21)

- `app/FamilyClaimRef.App/Composition/AppServices.cs`
- `app/FamilyClaimRef.App/Models/Storage/PolicyRecord.cs`
- `app/FamilyClaimRef.App/ProductShell/ProductShellWindow.xaml`
- `app/FamilyClaimRef.App/Resources/UiStrings.xaml`
- `app/FamilyClaimRef.App/Services/Localization/UiTextKeys.cs`
- `app/FamilyClaimRef.App/Services/Storage/IPolicyClaimStorageService.cs`
- `app/FamilyClaimRef.App/Services/Storage/JsonPolicyClaimStorageService.cs`
- `app/FamilyClaimRef.App/ViewModels/PolicyClaimManagementViewModel.cs`
- `app/FamilyClaimRef.App/ViewModels/ProductShellViewModel.cs`
- `app/FamilyClaimRef.App/Views/ProductPolicyContractsView.xaml`
- `app/FamilyClaimRef.App/Views/ProductPolicyContractsView.xaml.cs`
- `tests/FamilyClaimRef.App.Tests/DocumentLinkCoordinatorTests.cs`
- `tests/FamilyClaimRef.App.Tests/DocumentRegistrationLifecycleGate8Tests.cs`
- `tests/FamilyClaimRef.App.Tests/DocumentRegistrationViewModelTests.cs`
- `tests/FamilyClaimRef.App.Tests/DocumentRegistrationWorkflowTests.cs`
- `tests/FamilyClaimRef.App.Tests/PolicyClaimManagementViewModelTests.cs`
- `tests/FamilyClaimRef.App.Tests/ProductPolicyClaimAccessibilityLayoutContractTests.cs`
- `tests/FamilyClaimRef.App.Tests/ProductPolicyClaimManagementIntegrationTests.cs`
- `tests/FamilyClaimRef.App.Tests/ProductShellViewModelTests.cs`
- `tests/FamilyClaimRef.App.Tests/ProductWireframeRouteCoverageTests.cs`
- `tests/FamilyClaimRef.App.Tests/Services/Localization/ResourceUiTextProviderTests.cs`

T3-PER-A dependency closure는 총 28개다. 기존 dirty 보호 문서 `docs/00_PROJECT_BASE_GUIDE.md`는 위 범위에 포함하지 않는다.

## G. JSON Compatibility and Write Safety

- 기존 `policies.json` envelope와 schema version 1을 유지한다.
- 신규 필드는 camelCase JSON으로 저장한다.
- 기존 레코드에 신규 필드가 없으면 nullable 기본값으로 읽고 read-time rewrite하지 않는다.
- 저장은 고유 임시 파일에 전체 envelope를 기록한 뒤 final path로 이동한다.
- write/replace 실패 시 기존 정상 파일을 보존하고 임시 파일을 정리한다.
- 손상 JSON은 안전하게 실패하며 자동 초기화, 묵음 삭제, 임의 migration을 하지 않는다.
- DB, API, schema migration, hard delete, cascade delete를 구현하지 않았다.

## H. Automated Evidence

- Solution build: `PASS`, warnings/errors `0/0`
- T3-PER-A focused tests: `151/151`, failed/skipped `0/0`
- 관련 기존 storage/ViewModel/composition/Gate 8 회귀 묶음: `60/60`, failed/skipped `0/0`
- Full test suite: `632/632`, failed/skipped `0/0`
- 기존 baseline tests: `592`
- 신규 tests: `40`
- 기존 test loss: `0`
- Resource parity: `129/129`
- `Ui.Product.*` parity: `73/73`
- `git diff --check`: `PASS` (기존 LF/CRLF warning만 있음)
- trailing whitespace / merge marker: `0/0`
- local profile path / visible raw ID-path-SHA-exception binding: `0/0`

테스트 프로젝트의 `T3PerA` configuration으로 최종 빌드와 테스트를 수행했다. 실행 중인 기존 Product의 Debug 출력 파일을 건드리지 않기 위해 solution build는 고유 `%TEMP%` `OutDir`를 사용했고 해당 build output은 종료 후 제거했다.

## I. Independent Review

1회 독립 source/test 검토에서 다음을 발견하고 승인 범위 안에서 수리했다.

- Major 1: 보험 전용 refresh가 기존 policy collection과 선택 상태를 갱신하지 않아 사용 중지 후 stale selection이 남을 수 있었다. 공용 정책 상태 보정과 회귀 테스트를 추가했다.
- Minor 1: 화면 11/12에 실제 7필드 영속화와 모순되는 과거 안내 영역이 남아 있었다. 해당 표시 영역을 제거했다.
- Minor 1: 기존 ProductShell 가족 편집 테스트가 자신의 fixture root를 정리하지 않았다. `try/finally` 정리 계약을 추가했다.

수리 후 최종 findings:

- Blocking: `0`
- Major: `0`
- Minor: `0`

최종 테스트 실행 전 공용 ProductShell test fixture 잔여는 files/directories `24/72`였다. 출처가 확정되지 않은 기존 잔여를 임의 삭제하지 않았다. 정리 계약 보강 후 집중·전체 테스트를 다시 실행했으며 최종 값도 `24/72`, 최신 잔여 시각도 변경 없음으로 신규 residue `0`을 확인했다.

## J. Runtime and Protected State

- Product App launch: `0`
- 사용자 runtime validation: `NOT_EXECUTED`
- 사용자 runtime next state: `READY_FOR_REVIEW`
- 기존 PID `35692`: 종료, 재사용, 조작하지 않음
- 기존 synthetic/evidence root: 접근, 변경, 정리하지 않음
- 실제 사용자 data root: 접근하지 않음
- `data/claimdoc`: 접근하지 않음
- disposable runtime/data root: App runtime `0`; 자동 테스트 고유 fixture만 사용
- solution build TEMP residue: `0`
- stage/commit/push: `0/0/0`
- T3-PER-C/D: `NOT_STARTED`

## K. Final Boundary

이 문서는 source 및 자동 검증 완료 증거이며 사용자 runtime acceptance를 선언하지 않는다. Production readiness는 평가하지 않았고 deployment는 승인하지 않는다. 다음 단계는 새 disposable runtime/data root에서 화면 11/12 create, edit, restart, inactive/reactivate/orphan reference 동작을 사용자 검토하는 것이다.

## L. 2026-08-05 R1 User-Driven Contract Revision

### L.1 Superseded State and Final Marker

위 A-K는 당시 7필드 계약에 대한 역사적 구현 및 검증 기록으로 보존한다. 2026-08-05 사용자 요구에 따라 해당 업무 계약은 다음 상태로 대체되었다.

- `T3_PER_A_ORIGINAL_7_FIELD_CONTRACT_STATE = SUPERSEDED_BY_USER_REQUIREMENT_2026_08_05`
- `T3_PER_A_USER_RUNTIME_STATE = REOPENED_FOR_REQUIREMENT_REVISION`
- `CREATE_EDIT_OBSERVED = NOT_ACCEPTED_DUE_TO_REVISED_REQUIREMENTS`
- `HOLD_T3_PER_A_R1_VALIDATION_ENVIRONMENT_BLOCKED`
- `T3_PER_A_R1_IMPLEMENTATION_STATE = IMPLEMENTED`
- `T3_PER_A_R1_AUTOMATED_VALIDATION_STATE = PASS`
- `T3_PER_A_R1_INDEPENDENT_REVIEW_STATE = PASS`
- `T3_PER_A_R1_USER_RUNTIME_STATE = HOLD_WINDOWLESS_PROCESS_RESIDUAL`
- `T3_PER_A_R1_COMMIT_STATE = NOT_CREATED`
- `T3_PER_C_D_STATE = NOT_STARTED`
- `PRODUCTION_READINESS_STATE = NOT_EVALUATED`
- `DEPLOYMENT_STATE = NOT_AUTHORIZED`

### L.2 Revised Insurance Contract

| No. | User-editable field | Persisted representation | Rule |
|---:|---|---|---|
| 1 | `DisplayTitle` | trimmed `string` | required |
| 2 | `FamilyMemberId` | existing family ID `string` | required; active target required for create or reference change |
| 3 | `InsurerName` | trimmed `string` | required; classification is not inferred from this value |
| 4 | `ContractStatus` | `유지`, `만기`, `보험료 납입면제` | required exact selection; legacy `사용 중` is presented as `유지` |
| 5 | `EnrollmentDate` | ISO `DateOnly` | required; screen 11 displays `yyyy-MM-dd` |
| 6 | `CoveragePeriod` | trimmed `string` | required |
| 7 | `PremiumPaymentPeriod` | trimmed `string` | required free text; separate from coverage period |
| 8 | `TotalPlannedPremiumAmount` | nullable whole-number `decimal`, KRW | optional; non-negative; no automatic calculation; list displays `N0원` |
| 9 | `RenewalType` | `갱신형`, `비갱신형(고정형)`, `일부 갱신형` | required exact selection |
| 10 | `RefundType` | `환급형`, `해약환급금 미지급형` | required exact selection |
| 11 | `InsuranceBusinessType` | `생명보험`, `손해보험` | required exact selection; UI label `보험사 구분` |
| 12 | `ProductCategory` | `실손보험`, `운전자보험`, `암보험`, `종합보험` | required exact selection |

`RegistrationSource`는 사용자 편집 필드가 아닌 시스템 메타데이터다. 화면 12 직접 생성은 `직접 입력`을 저장하고, 편집은 최초 값을 유지한다. `보험 문서 등록` 연계는 가장하지 않으며 화면에서는 읽기 전용으로만 표시한다.

화면 12는 `기본정보`, `보장·납입정보`, `보험 분류`, `등록정보` 네 영역으로 구성한다. 화면 11은 보험 계약 이름, 가족, 보험사, 상품 구분, 계약 상태, 가입일, 총 납입예정액을 표시한다. 저장 성공은 화면 11로 복귀해 active 목록을 다시 불러오고, 저장 실패는 화면 12와 입력값을 유지한다.

### L.3 ReferenceDate Ownership

`ReferenceDate`는 보험 계약의 12개 사용자 편집 필드가 아니라 문서 메타데이터다. 신규 보험 계약에는 `null`로 저장하며 `EnrollmentDate` 또는 오늘 날짜를 복사하지 않는다.

- UI label: `문서 발급·조회 기준일`
- UI help: `문서에 표시된 발급일 또는 보험정보 조회 기준일입니다. 보험 가입일과는 다릅니다. 문서에 날짜가 없으면 비워두세요.`
- document metadata: nullable `DateOnly`
- durable JSON: 날짜가 없으면 `null`; 가입일 또는 오늘 날짜로 대체하지 않음
- physical attachment file name: 날짜가 없을 때 내부 전용 `00010101` 토큰만 사용하며 durable metadata에는 기록하지 않음
- display: 값이 있으면 culture와 무관하게 `yyyy-MM-dd`

실제 `JsonDocumentStorageService` 경로를 사용하는 회귀 테스트로 null 기준일 등록, durable null 보존, 내부 파일명 토큰을 확인했다.

### L.4 Compatibility and Storage Boundary

- 이전 7필드 JSON은 새 필드가 없어도 손실 없이 로드하고 read-time rewrite하지 않는다.
- legacy ID, `FamilyMemberId`, `CoveragePeriod`, `RegistrationSource`를 유지한다.
- legacy `사용 중`은 UI에서 `유지`로 인식하지만 arbitrary legacy status는 `기존 값 확인 필요`로 표시한다.
- 누락된 새 선택값은 `미등록` 또는 `선택 필요`로 표시하며 저장 전에 사용자가 명시적으로 선택해야 한다.
- 선택형 다섯 그룹과 금액의 정수, 음수 규칙은 UI와 `JsonPolicyClaimStorageService` 경계에서 모두 검증한다.
- 동일한 inactive 가족 참조를 유지하는 편집, reactivate, orphan 재연결의 기존 정책을 유지한다.
- 저장 실패 시 기존 정상 JSON을 보존한다.
- DB, API, migration, ledger, OCR, hard delete, cascade delete를 추가하지 않았다.

### L.5 Exact Combined Dependency Closure

R1 완료 시 T3-PER-A combined dependency closure는 `CREATE 9`, `MODIFY 29`, 합계 `38`개다. 별도 기존 사용자 변경 `docs/00_PROJECT_BASE_GUIDE.md`는 보호 상태로 유지하며 dependency closure에 포함하지 않는다.

#### CREATE (9)

- `app/FamilyClaimRef.App/Models/Storage/InsurancePolicyDraft.cs`
- `app/FamilyClaimRef.App/Models/Storage/InsurancePolicyValues.cs`
- `app/FamilyClaimRef.App/ViewModels/InsurancePolicyListItemViewModel.cs`
- `app/FamilyClaimRef.App/Views/ProductInsurancePolicyEditorView.xaml`
- `app/FamilyClaimRef.App/Views/ProductInsurancePolicyEditorView.xaml.cs`
- `tests/FamilyClaimRef.App.Tests/InsurancePolicyManagementViewModelTests.cs`
- `tests/FamilyClaimRef.App.Tests/InsurancePolicyPersistenceTests.cs`
- `tests/FamilyClaimRef.App.Tests/InsurancePolicyRevisionContractTests.cs`
- `docs/442_FAMILY_INSURANCE_PERSISTENCE_T3_PER_A_IMPLEMENTATION_AND_EVIDENCE.md`

#### MODIFY (29)

- `app/FamilyClaimRef.App/Composition/AppServices.cs`
- `app/FamilyClaimRef.App/Models/Storage/PolicyRecord.cs`
- `app/FamilyClaimRef.App/ProductShell/ProductShellWindow.xaml`
- `app/FamilyClaimRef.App/Resources/UiStrings.xaml`
- `app/FamilyClaimRef.App/Services/Localization/UiTextKeys.cs`
- `app/FamilyClaimRef.App/Services/Storage/ClaimDocumentRegistrationRequest.cs`
- `app/FamilyClaimRef.App/Services/Storage/DocumentAttachmentCoordinator.cs`
- `app/FamilyClaimRef.App/Services/Storage/DocumentAttachmentRequest.cs`
- `app/FamilyClaimRef.App/Services/Storage/IPolicyClaimStorageService.cs`
- `app/FamilyClaimRef.App/Services/Storage/JsonDocumentStorageService.cs`
- `app/FamilyClaimRef.App/Services/Storage/JsonPolicyClaimStorageService.cs`
- `app/FamilyClaimRef.App/Services/Storage/PolicyDocumentRegistrationRequest.cs`
- `app/FamilyClaimRef.App/ViewModels/DocumentRegistrationViewModel.cs`
- `app/FamilyClaimRef.App/ViewModels/PolicyClaimManagementViewModel.cs`
- `app/FamilyClaimRef.App/ViewModels/ProductShellViewModel.cs`
- `app/FamilyClaimRef.App/Views/ProductDocumentRegistrationView.xaml`
- `app/FamilyClaimRef.App/Views/ProductPolicyContractsView.xaml`
- `app/FamilyClaimRef.App/Views/ProductPolicyContractsView.xaml.cs`
- `tests/FamilyClaimRef.App.Tests/DocumentAttachmentCoordinatorTests.cs`
- `tests/FamilyClaimRef.App.Tests/DocumentLinkCoordinatorTests.cs`
- `tests/FamilyClaimRef.App.Tests/DocumentRegistrationLifecycleGate8Tests.cs`
- `tests/FamilyClaimRef.App.Tests/DocumentRegistrationViewModelTests.cs`
- `tests/FamilyClaimRef.App.Tests/DocumentRegistrationWorkflowTests.cs`
- `tests/FamilyClaimRef.App.Tests/PolicyClaimManagementViewModelTests.cs`
- `tests/FamilyClaimRef.App.Tests/ProductPolicyClaimAccessibilityLayoutContractTests.cs`
- `tests/FamilyClaimRef.App.Tests/ProductPolicyClaimManagementIntegrationTests.cs`
- `tests/FamilyClaimRef.App.Tests/ProductShellViewModelTests.cs`
- `tests/FamilyClaimRef.App.Tests/ProductWireframeRouteCoverageTests.cs`
- `tests/FamilyClaimRef.App.Tests/Services/Localization/ResourceUiTextProviderTests.cs`

허용 dependency closure 밖 신규 변경은 `0`이다.

### L.6 R1 Automated Evidence and Independent Review

- Solution build: `PASS`, warnings/errors `0/0`
- R1 focused impact tests: `361/361`, failed/skipped `0/0`
- Full test suite: `671/671`, failed/skipped `0/0`
- 기존 632 tests loss: `0`
- 신규 tests: `39`
- Resource/constants parity: `144/144`
- `Ui.Product.*` parity: `87/87`
- `git diff --check`: `PASS` with existing LF/CRLF conversion notices only
- trailing whitespace / merge marker / missing final LF: `0/0/0`
- changed-path local profile / actual personal data scan: `0/0`

독립 spec/code-quality 검토에서 다음 두 항목을 발견해 승인 범위 안에서 수리하고 build, focused tests, full tests를 모두 다시 실행했다.

- Major 1: UI와 coordinator가 null `ReferenceDate`를 허용해도 실제 `JsonDocumentStorageService`가 complete metadata 검증에서 이를 거부했다. 날짜를 선택 메타데이터로 분리하고 실제 JSON 저장 경로 테스트를 추가했다.
- Minor 1: `InsuranceBusinessType` 라벨이 exact 계약 `보험사 구분`이 아니라 `보험업 구분`이었다. 두 리소스 소스와 exact contract test를 교정했다.

최종 independent findings는 Blocking/Major/Minor `0/0/0`이다.

### L.7 Runtime and Protected State

- 기존 PID `43100`: 정상 종료를 요청했으나 top-level window가 없어 `CloseMainWindow()`가 `false`; 강제 종료하지 않았고 현재 windowless process로 남아 있다.
- 기존 PID `35692`: 조회, 종료, 재사용, 변경하지 않았다.
- 기존 runtime root `C:\Users\jin8855\AppData\Local\Temp\FamilyClaimRef\T3_PER_A_USER_RUNTIME_20260804_180144_471`: 삭제하거나 재사용하지 않고 보존했다.
- 첫 R1 launch PID `6512`: 최초 window handle `13701922`를 확인했으나 이후 windowless residual로 전환됨; 강제 종료하거나 root를 정리하지 않음
- 첫 R1 launch root: `C:\Users\jin8855\AppData\Local\Temp\FamilyClaimRef\T3_PER_A_R1_USER_RUNTIME_20260805_101332_004_265`
- 최종 disposable runtime root: `C:\Users\jin8855\AppData\Local\Temp\FamilyClaimRef\T3_PER_A_R1_USER_RUNTIME_RETRY1_20260805_101801_563_616`
- 최종 Product PID: `35996`
- 재시도 PID `35996`: 최초 window handle `529506`를 15초 동안 유지하고 responding 상태였으나 이후에도 windowless residual로 전환됨; 강제 종료하거나 root를 정리하지 않음
- 최종 Product state: 열린 top-level window 없음; 사용자 runtime 검토 준비 미완료
- 실제 사용자 data root: 접근 `0`
- `data/claimdoc`: 접근 `0`
- stage/commit/push: `0/0/0`
- T3-PER-C/D: 시작하지 않음

### L.8 User Runtime Review Procedure

runtime 환경의 windowless residual 원인을 별도 승인 범위에서 해소한 뒤 새 Product 창에서 화면 11과 12를 사용해 다음을 확인한다.

1. 계약 상태 exact 선택값 3개와 legacy 상태 안내
2. 보험료 납입기간 입력, 저장, 편집 복원
3. 총 납입예정액 optional 입력, 천 단위 `원` 표시, 재시작 복원
4. 갱신 유형, 환급 유형, 보험사 구분, 상품 구분 exact 선택값
5. 등록 출처 `직접 입력` 표시와 수정 불가 상태
6. 가입일의 `yyyy-MM-dd` 목록 표시
7. 문서 등록 화면의 `문서 발급·조회 기준일` 라벨과 도움말, 날짜 미입력 허용
8. create, edit, cancel, restart
9. inactive, reactivate, orphan reference 기존 동작

현재 runtime은 `HOLD_WINDOWLESS_PROCESS_RESIDUAL`이며 사용자 검토를 시작할 수 없다. 구현 및 자동 검증 PASS는 유지하지만 필수 새 runtime 준비가 완료되지 않았으므로 R1 전체 판정은 환경 차단 HOLD다. 사용자 확인 전 `USER_ACCEPTED`로 승격하지 않는다.

## M. 2026-08-05 R1-UI Approved Wireframe Structural Conformance Repair

### M.1 User Observation and Authority

사용자 런타임 관찰에 따라 create/edit 동작 PASS는 유지하고 화면 12의 와이어프레임 준수 판정만 다시 열었다.

- `CREATE_EDIT_BEHAVIOR_OBSERVED = PASS`
- `WIREFRAME_CONFORMANCE_OBSERVED = FAIL`
- `T3_PER_A_USER_RUNTIME_STATE = REOPENED_FOR_UI_REPAIR`
- 결함 증거: `C:\Users\jin8855\AppData\Local\Temp\codex-clipboard-dec874e0-23c8-4702-9948-10061d9e9055.png`
- 사용자 제공 기준 이미지: `C:\Users\jin8855\AppData\Local\Temp\codex-clipboard-79e6a4b4-2cda-41ae-a1ad-6b7aea0f90f7.png`
- 승인 원본: `design/wireframes/12_policy_register.html`
- 화면 ID: `12_policy_register`

원본 HTML의 상단 명령 영역, 좌측 보험 요약 정보, 우측 연결 문서, 하단 전체 폭 담보 후보 확인 구조가 서로 일치함을 read-only로 확인했다. 화면 12 authority 충돌은 없었다.

### M.2 Structural Repair

| Area | Final implementation | Follow-up boundary |
|---|---|---|
| 상단 명령 영역 | 우측 정렬 명령 모음으로 이동 | `임시저장`, `보류`, `삭제`, `사용 중지`는 승인된 저장 계약이 없어 비활성 |
| 보험 요약 정보 | 데스크톱 좌측 카드, 좁은 폭 첫 번째 적층 영역 | T3-PER-A-R1의 12개 편집 필드와 읽기 전용 `RegistrationSource`만 사용 |
| 이 보험에 연결할 문서 | 데스크톱 우측 카드, 좁은 폭 두 번째 적층 영역 | create mode에서는 비활성, edit mode에서 기존 화면 17 보험 문서 등록 route만 사용 |
| 담보 후보 확인 | 상단 두 카드 아래 전체 폭 영역 | 분석·판정은 미구현이며 명시적 빈 상태만 표시 |

새 보험 필드 배치는 다음과 같다.

| Subsection | Fields |
|---|---|
| 기본정보 | 가족, 보험 계약 이름, 보험사, 가입일 |
| 보장·납입정보 | 보험기간, 보험료 납입기간, 총 납입예정액 |
| 보험 분류 | 계약 상태, 갱신 유형, 환급 유형, 보험사 구분, 상품 구분 |
| 등록정보 | 등록 출처 읽기 전용 |

데스크톱에서는 보험 요약과 연결 문서를 같은 행의 2열로 표시한다. 사용 가능한 폭이 `1100` 미만이면 보험 요약, 연결 문서, 담보 후보 순으로 적층한다. 화면 12의 가로 스크롤은 비활성이다.

### M.3 Existing Route and Safety Contract

- 보험 문서 추가는 새 저장소나 새 문서 workflow를 만들지 않는다.
- edit mode에서 현재 보험의 `SelectedPolicyId`를 유지하고 기존 `17_policy_document_register` route를 사용한다.
- create mode의 문서 추가 버튼은 비활성이다.
- 문서 유형 세 행은 보험 조회 캡처, 보험증권/계약서, 약관 PDF/DOCX다.
- 문서 상태는 이 화면에서 추정하지 않고 기존 보험 문서 등록 화면에서 확인하도록 안내한다.
- 담보 후보 자동 분석, OCR, 자동 판정, 가짜 후보 데이터는 추가하지 않았다.
- `RegistrationSource`는 읽기 전용 `OneWay` 바인딩으로 유지한다.

### M.4 Exact UI Repair Scope

이번 R1-UI 보정에서 변경한 exact path는 다음 11개다.

#### Production and resources (6)

- `app/FamilyClaimRef.App/Composition/AppServices.cs`
- `app/FamilyClaimRef.App/Resources/UiStrings.xaml`
- `app/FamilyClaimRef.App/Services/Localization/UiTextKeys.cs`
- `app/FamilyClaimRef.App/ViewModels/PolicyClaimManagementViewModel.cs`
- `app/FamilyClaimRef.App/Views/ProductInsurancePolicyEditorView.xaml`
- `app/FamilyClaimRef.App/Views/ProductInsurancePolicyEditorView.xaml.cs`

#### Tests (4)

- `tests/FamilyClaimRef.App.Tests/DocumentRegistrationLifecycleGate8Tests.cs`
- `tests/FamilyClaimRef.App.Tests/InsurancePolicyRevisionContractTests.cs`
- `tests/FamilyClaimRef.App.Tests/ProductShellViewModelTests.cs`
- `tests/FamilyClaimRef.App.Tests/Services/Localization/ResourceUiTextProviderTests.cs`

#### Evidence (1)

- `docs/442_FAMILY_INSURANCE_PERSISTENCE_T3_PER_A_IMPLEMENTATION_AND_EVIDENCE.md`

신규 UI 리소스는 20개다. 최종 resource/constants parity는 `164/164`, `Ui.Product.*` parity는 `107/107`이다.

### M.5 Automated Validation

- Release solution build: `PASS`, warnings/errors `0/0`
- 화면 12 구조·반응형 contract tests: `14/14`, failed/skipped `0/0`
- 보험 persistence/ViewModel, route, ProductShell, resource focused regression: `204/204`, failed/skipped `0/0`
- focused 합계: `218/218`, failed/skipped `0/0`
- full test suite: `699/699`, failed/skipped `0/0`
- 기존 R1 full tests `671` 대비 신규 tests: `8`
- 기존 test loss: `0`

Debug output build는 기존 windowless PID `6512`, `35996`가 Debug 실행 파일을 잠가 중단됐다. 해당 프로세스를 승인 없이 종료하지 않았고, 동일 소스를 별도 Release output으로 빌드하여 최종 `0/0`을 확인했다.

### M.6 Runtime Repair and Visual Evidence

첫 disposable runtime에서 화면 12 진입 시 읽기 전용 `InsuranceRegistrationSource`의 기본 TwoWay 바인딩 때문에 WPF `InvalidOperationException`이 발생했다.

- first PID: `41996`
- first runtime root: `C:\Users\jin8855\AppData\Local\Temp\FamilyClaimRef\T3_PER_A_R1_UI_Runtime_20260805_104129_501`
- first result: 화면 12 진입 시 process 종료
- repair: `Text="{Binding InsuranceRegistrationSource, Mode=OneWay}"` 및 회귀 assertion 추가

수리 후 새 disposable runtime에서 화면 12 route와 시각 구조를 다시 검증했다.

- final runtime root: `C:\Users\jin8855\AppData\Local\Temp\FamilyClaimRef\T3_PER_A_R1_UI_Runtime_20260805_104456_415`
- evidence root: `C:\Users\jin8855\AppData\Local\Temp\FamilyClaimRef\T3_PER_A_R1_UI_Evidence_20260805_104456_415`
- final PID: `42348`
- final window handle: `6886658`
- final Product state: 화면 12 데스크톱 상단 상태로 열어 둠

Evidence files:

- `01_after_desktop_1600x1000.png`
- `02_after_desktop_1800x1152.png`
- `03_after_desktop_bottom_1800x1152.png`
- `04_after_narrow_960x1000_top.png`
- `05_after_narrow_960x1000_stacked.png`
- `06_after_narrow_960x1000_bottom.png`

실제 화면 검토 결과:

- desktop upper panels: 좌우 2열, 겹침·가로 잘림 없음
- desktop lower panel: 상단 두 카드 아래 전체 폭 담보 후보 영역 확인
- narrow layout: 보험 요약, 연결 문서, 담보 후보 순서 확인
- narrow horizontal scroll: `false`
- create mode document actions: `3/3 disabled`
- top command state: 임시저장/보류/삭제/사용 중지 disabled, 저장은 필수값 미입력 상태에서 disabled, 닫기 enabled
- raw ID, 경로, SHA, 예외 전문의 Product UI 노출: `0`

### M.7 Independent Review and Final State

와이어프레임 계약과 code quality를 분리해 재검토했다. 첫 runtime 바인딩 결함을 수리한 뒤 build, 구조 tests, full tests, runtime route, desktop/narrow screenshot을 다시 확인했다.

- Blocking: `0`
- Major: `0`
- Minor: `0`
- Product/source files staged: `0`
- commit/push: `0/0`
- 실제 사용자 data root 접근: `0`
- `data/claimdoc` 접근: `0`
- DB/API/migration/OCR/담보 자동 판정: `0`
- T3-PER-C/D: `NOT_STARTED`

최종 상태:

- `T3_PER_A_R1_UI_IMPLEMENTATION_STATE = IMPLEMENTED`
- `T3_PER_A_R1_UI_AUTOMATED_VALIDATION_STATE = PASS`
- `T3_PER_A_R1_UI_VISUAL_REVIEW_STATE = AWAITING_USER`
- `T3_PER_A_R1_COMMIT_STATE = NOT_CREATED`
- `DEPLOYMENT_STATE = NOT_AUTHORIZED`

`FAMILYCLAIMREF_T3_PER_A_R1_UI_WIREFRAME_REPAIR_READY_FOR_USER_REVIEW`

## N. 2026-08-05 FU-01 Amount Input and Screen 17 Structural Repair

### N.1 User Observation

기능 사용자 검토의 `FU-01` 단계에서 사용자가 다음 결함을 직접 관찰했다.

- 금액 필드 명칭을 `총 납입예정액`에서 `납입액`으로 변경해야 한다.
- 금액은 숫자만 입력하고 우측 정렬하며 3자리 콤마를 자동 표시해야 한다.
- 화면 17의 실제 Product 화면이 승인된 `design/wireframes/17_policy_document_register.html` 구조와 다르다.
- 결함 증거: `C:\Users\jin8855\AppData\Local\Temp\codex-clipboard-e93b91d4-f20e-4f38-8627-45013145abd4.png`
- 승인 와이어프레임 비교 증거: `C:\Users\jin8855\AppData\Local\Temp\codex-clipboard-244ebbe7-4708-4003-8263-27c7b9d08cda.png`

따라서 `FU-01`은 PASS로 처리하지 않았고 기능 검토를 수리 범위로 분리했다.

### N.2 Root Cause and Repair

- `ProductInsurancePolicyEditorView`의 금액 입력은 일반 `TextBox`였고 명칭, 입력 제한, 정렬, 자동 형식 계약이 없었다.
- `ProductDocumentRegistrationView`는 실제 등록 workflow를 제공했지만 화면 17의 선택 보험 요약과 좌우 업무 영역을 구현하지 않은 세로형 화면이었다.

적용한 최소 수리는 다음과 같다.

- 금액 명칭을 `납입액`으로 변경했다.
- 금액 입력은 숫자만 허용하고 우측 정렬하며 입력·붙여넣기 시 3자리 콤마를 자동 표시한다.
- 화면 17을 상단 명령, 선택된 보험 요약, 안내, `문서 이미지/파일 연결`과 `사용자 문서 내용 확인` 2열 구조로 재배치했다.
- 좁은 화면에서는 두 업무 영역을 순서대로 적층한다.
- 선택된 보험의 가족 표시명과 보험사를 요약에 표시하되 raw ID는 표시하지 않는다.
- 기존 파일 선택, 대상 선택, `ReferenceDate`, 등록, 중복·busy·retry 계약은 유지했다.
- 화면 17의 `닫기`는 승인 와이어프레임과 같이 화면 12 보험 등록/편집으로 돌아간다.
- OCR, 자동 문서 분석, T3-PER-C/D 기능은 확장하지 않았다.

### N.3 Exact Repair Scope

Production and resources:

- `app/FamilyClaimRef.App/Composition/AppServices.cs`
- `app/FamilyClaimRef.App/Resources/ProductScreenContent.xaml`
- `app/FamilyClaimRef.App/Resources/UiStrings.xaml`
- `app/FamilyClaimRef.App/ViewModels/DocumentRegistrationViewModel.cs`
- `app/FamilyClaimRef.App/Views/ProductDocumentRegistrationView.xaml`
- `app/FamilyClaimRef.App/Views/ProductDocumentRegistrationView.xaml.cs`
- `app/FamilyClaimRef.App/Views/ProductInsurancePolicyEditorView.xaml`
- `app/FamilyClaimRef.App/Views/ProductInsurancePolicyEditorView.xaml.cs`

Tests:

- `tests/FamilyClaimRef.App.Tests/InsurancePolicyRevisionContractTests.cs`
- `tests/FamilyClaimRef.App.Tests/ProductWireframeRouteCoverageTests.cs`
- `tests/FamilyClaimRef.App.Tests/Services/Localization/ResourceUiTextProviderTests.cs`

Evidence:

- `docs/442_FAMILY_INSURANCE_PERSISTENCE_T3_PER_A_IMPLEMENTATION_AND_EVIDENCE.md`

### N.4 Automated Validation

- Release solution build: `PASS`, warnings/errors `0/0`
- 금액 입력 및 화면 17 구조 focused tests: `27/27`, failed/skipped `0/0`
- 보험, 문서 등록, Product navigation, resource focused regression: `248/248`, failed/skipped `0/0`
- full test suite: `711/711`, failed/skipped `0/0`
- 이전 full test `699` 대비 신규 tests: `12`
- `git diff --check`: `PASS`
- staged/commit/push: `0/0/0`

### N.5 Runtime Recheck State

- 기존 FU Product PID `42348`: 정상 창 종료 완료
- 보존 중인 Debug PID `35996`: 접근·종료하지 않음
- 새 disposable runtime root: `C:\Users\jin8855\AppData\Local\Temp\FamilyClaimRef\T3_PER_A_R1_FU_Repair_20260805_141507_584`
- 새 Product PID: `37360`
- 새 window handle: `5574352`
- 기존 synthetic 가족 fixture만 새 root로 복사했다.
- 실제 사용자 data root 접근: `0`
- `data/claimdoc` 접근: `0`
- 기존 runtime/evidence root 정리: `0`

현재 상태:

- `T3_PER_A_R1_UI_VISUAL_REVIEW_STATE = REOPENED_FOR_SCREEN_17_RECHECK`
- `T3_PER_A_R1_FUNCTIONAL_USER_REVIEW_STATE = AWAITING_USER`
- `T3_PER_A_R1_COMMIT_STATE = NOT_CREATED`
- `T3_PER_C_D_STATE = NOT_STARTED`
- `PRODUCTION_READINESS_STATE = NOT_EVALUATED`
- `DEPLOYMENT_STATE = NOT_AUTHORIZED`

`FAMILYCLAIMREF_T3_PER_A_R1_FU_REPAIR_READY_FOR_USER_REVIEW`

## O. 2026-08-05 FU-02 Registered Document Projection Repair

### O.1 User Observation

사용자가 화면 17에서 파일을 선택하고 등록한 뒤 다음 결함을 직접 관찰했다.

- 등록 상태에는 `문서 등록이 완료되었습니다.`가 표시됐다.
- 등록 성공 후 화면 17에 그대로 남았다.
- 해당 보험의 연결 문서 영역에서 등록 결과를 확인할 수 없었다.
- 결함 증거: `C:\Users\jin8855\AppData\Local\Temp\codex-clipboard-758253be-0adf-4f43-b739-79a51087bdda.png`

이 관찰로 기능 사용자 검토를 다시 열었으며, 완료 메시지만으로 PASS 처리하지 않았다.

### O.2 Persistence Inspection and Root Cause

승인된 synthetic root만 읽기 전용으로 검사했다.

- inspected root: `C:\Users\jin8855\AppData\Local\Temp\FamilyClaimRef\T3_PER_A_R1_FU_Repair_20260805_141507_584`
- `documents.json`: active document `1`
- `policy-documents.json`: active policy link `1`
- managed attachment: `1`
- linked policy display title: `실손보험`
- registered document display title: `보험약관`
- document/link target consistency: `PASS`

따라서 원인은 저장 실패가 아니었다. 기존 구현은 다음 두 표시 계약이 누락되어 있었다.

1. 등록 성공 후 해당 보험 편집 화면으로 복귀하는 navigation 계약
2. 보험 편집 화면에서 `IDocumentStorageService`의 policy-document link와 document title을 읽는 projection 계약

### O.3 Minimal Repair

- `DocumentRegistrationViewModel.RegisterAsync`가 실제 성공 여부를 반환하도록 변경했다.
- 보험 대상 등록이 성공하면 화면 17에서 해당 보험 편집 화면으로 복귀한다.
- `PolicyClaimManagementViewModel`의 Product 구성에 기존 `IDocumentStorageService`를 주입했다.
- 보험 편집 진입 시 active policy-document link와 active document를 조회한다.
- 보험 조회 캡처, 보험증권/계약서, 약관 PDF/DOCX 행에 등록된 실제 표시 제목을 투영한다.
- 미등록 행은 `미등록`, 조회 실패는 `문서 상태를 불러오지 못했습니다.`로 표시한다.
- raw ID, 원본 경로, SHA, 예외 전문은 Product UI에 투영하지 않는다.
- 새 저장소, 스키마, DB, API, OCR, 자동 분석은 추가하지 않았다.

Exact repair delta:

Production and resources:

- `app/FamilyClaimRef.App/Composition/AppServices.cs`
- `app/FamilyClaimRef.App/Resources/UiStrings.xaml`
- `app/FamilyClaimRef.App/Services/Localization/UiTextKeys.cs`
- `app/FamilyClaimRef.App/ViewModels/DocumentRegistrationViewModel.cs`
- `app/FamilyClaimRef.App/ViewModels/PolicyClaimManagementViewModel.cs`
- `app/FamilyClaimRef.App/Views/ProductDocumentRegistrationView.xaml.cs`
- `app/FamilyClaimRef.App/Views/ProductInsurancePolicyEditorView.xaml`

Tests:

- `tests/FamilyClaimRef.App.Tests/DocumentRegistrationLifecycleGate8Tests.cs`
- `tests/FamilyClaimRef.App.Tests/DocumentRegistrationViewModelTests.cs`
- `tests/FamilyClaimRef.App.Tests/InsurancePolicyManagementViewModelTests.cs`
- `tests/FamilyClaimRef.App.Tests/InsurancePolicyRevisionContractTests.cs`
- `tests/FamilyClaimRef.App.Tests/Services/Localization/ResourceUiTextProviderTests.cs`

Evidence:

- `docs/442_FAMILY_INSURANCE_PERSISTENCE_T3_PER_A_IMPLEMENTATION_AND_EVIDENCE.md`

### O.4 Automated Validation

- focused contract/resource tests: `128/128`, failed/skipped `0/0`
- related registration/insurance/list/shell/resource regression: `253/253`, failed/skipped `0/0`
- Release solution build: `PASS`, warnings/errors `0/0`
- full test suite: `715/715`, failed/skipped `0/0`
- resource/constants parity: `166/166`
- `Ui.Product.*` parity: `109/109`
- `git diff --check`: `PASS`
- staged/commit/push: `0/0/0`

기존 Product PID `37360`은 Release output lock을 해제하기 위해 `CloseMainWindow()`로 정상 종료했다. 보호 중인 Debug PID는 종료하지 않았다.

### O.5 User Recheck Runtime

- runtime root: `C:\Users\jin8855\AppData\Local\Temp\FamilyClaimRef\T3_PER_A_R1_DocumentProjection_Recheck_20260805_144251_894`
- Product PID: `47556`
- window handle: `5967714`
- process state: open for user review
- copied synthetic files: `5`
- source synthetic root cleanup: `0`
- actual user data root access: `0`
- `data/claimdoc` access: `0`

사용자 확인 대기 항목:

1. `실손보험` 편집 화면의 약관 행에 `보험약관`이 표시되는지
2. 새 문서 등록 성공 후 해당 보험 편집 화면으로 자동 복귀하는지
3. 복귀한 화면에서 새 문서 표시 제목이 즉시 보이는지

현재 상태:

- `T3_PER_A_R1_DOCUMENT_PERSISTENCE_STATE = PASS`
- `T3_PER_A_R1_DOCUMENT_PROJECTION_REPAIR_STATE = IMPLEMENTED`
- `T3_PER_A_R1_DOCUMENT_PROJECTION_USER_REVIEW_STATE = AWAITING_USER`
- `T3_PER_A_R1_COMMIT_STATE = NOT_CREATED`
- `T3_PER_C_D_STATE = NOT_STARTED`
- `PRODUCTION_READINESS_STATE = NOT_EVALUATED`
- `DEPLOYMENT_STATE = NOT_AUTHORIZED`

`FAMILYCLAIMREF_T3_PER_A_R1_DOCUMENT_PROJECTION_REPAIR_READY_FOR_USER_REVIEW`

## P. 2026-08-05 FU-03 Policy Document Lifecycle and Compact Editor Repair

### P.1 User Decision

사용자는 보험 등록/편집 화면의 다음 변경을 승인했다.

- 하단의 별도 명령 패널을 제거하고 저장·보류·삭제·사용 중지·닫기 명령을 타이틀 영역 오른쪽으로 이동한다.
- 세로 여백과 표 행 높이를 줄여 불필요한 스크롤을 완화한다.
- 보험 문서는 동일 보험과 동일 문서 유형마다 활성 연결을 정확히 1건만 유지한다.
- 다시 등록하거나 연결을 해제해도 이전 연결, 문서 metadata, managed file은 이력으로 보존한다.
- 등록된 문서는 `문서 열기`, `다시 등록`, `연결 해제`로 관리하고 hard delete는 추가하지 않는다.

### P.2 Implementation Contract

보험 등록/편집 화면은 데스크톱 폭에서 페이지 제목과 명령을 한 행에 배치한다. 좁은 폭에서는 명령이 제목 아래로 줄바꿈되며, 보험 요약, 연결 문서, 담보 후보 순서의 적층 계약은 유지한다. 루트 여백, 섹션 간격, 안내 영역, 문서 표 행과 empty-state 높이를 줄였고 기존 전체 폭과 세 업무 영역은 유지했다.

문서 행의 제품 동작은 다음과 같다.

| 상태 | 기본 동작 | 추가 동작 | 저장 계약 |
|---|---|---|---|
| 미등록 | `문서 등록` | 없음 | 선택한 유형을 미리 설정하여 화면 17로 이동 |
| 등록 | `문서 열기` | `다시 등록`, `연결 해제` | managed relative path만 해석하여 기본 viewer로 열기 |
| 다시 등록 성공 | `문서 열기` | `다시 등록`, `연결 해제` | 기존 동일 유형 active link를 비활성화하고 신규 link 1건만 활성화 |
| 연결 해제 성공 | `문서 등록` | 없음 | active link만 비활성화하고 Document와 managed file은 유지 |

`문서 열기`는 attachment root 밖의 경로, 절대 경로, traversal, missing file, reparse point를 거부한다. 실패 시 경로와 예외 전문을 노출하지 않고 제품용 메시지만 표시한다. `연결 해제`는 사용자 확인 후 수행하며 파일 삭제 의미가 아님을 안내한다.

### P.3 Exact Incremental Scope

Production and resources:

- `app/FamilyClaimRef.App/Composition/AppServices.cs`
- `app/FamilyClaimRef.App/Resources/UiStrings.xaml`
- `app/FamilyClaimRef.App/Services/Localization/UiTextKeys.cs`
- `app/FamilyClaimRef.App/Services/Storage/DocumentLinkCoordinator.cs`
- `app/FamilyClaimRef.App/Services/Storage/DocumentRegistrationWorkflow.cs`
- `app/FamilyClaimRef.App/Services/Storage/IDocumentStorageService.cs`
- `app/FamilyClaimRef.App/Services/Storage/JsonDocumentStorageService.cs`
- `app/FamilyClaimRef.App/Services/UI/IManagedDocumentOpener.cs`
- `app/FamilyClaimRef.App/Services/UI/ManagedDocumentOpener.cs`
- `app/FamilyClaimRef.App/ViewModels/PolicyClaimManagementViewModel.cs`
- `app/FamilyClaimRef.App/ViewModels/ProductShellViewModel.cs`
- `app/FamilyClaimRef.App/Views/ProductInsurancePolicyEditorView.xaml`
- `app/FamilyClaimRef.App/Views/ProductInsurancePolicyEditorView.xaml.cs`

Tests:

- `tests/FamilyClaimRef.App.Tests/DocumentRegistrationLifecycleGate8Tests.cs`
- `tests/FamilyClaimRef.App.Tests/InsurancePolicyManagementViewModelTests.cs`
- `tests/FamilyClaimRef.App.Tests/InsurancePolicyRevisionContractTests.cs`
- `tests/FamilyClaimRef.App.Tests/Integration/AttachmentDuplicateCollisionValidationTests.cs`
- `tests/FamilyClaimRef.App.Tests/Integration/DocumentRegistrationPersistenceGate8Tests.cs`
- `tests/FamilyClaimRef.App.Tests/JsonDocumentStorageServiceTests.cs`
- `tests/FamilyClaimRef.App.Tests/ManagedDocumentOpenerTests.cs`
- `tests/FamilyClaimRef.App.Tests/ProductShellViewModelTests.cs`
- `tests/FamilyClaimRef.App.Tests/Services/Localization/ResourceUiTextProviderTests.cs`

Evidence:

- `docs/442_FAMILY_INSURANCE_PERSISTENCE_T3_PER_A_IMPLEMENTATION_AND_EVIDENCE.md`

### P.4 Automated Validation

- solution build: `PASS`, warnings/errors `0/0`
- focused storage, workflow, ViewModel, Product UI, navigation and resource tests: `247/247`, failed/skipped `0/0`
- full test suite: `732/732`, failed/skipped `0/0`
- resource/constants parity: `175/175`
- `Ui.Product.*` parity: `118/118`
- `git diff --check`: `PASS`
- staged/commit/push: `0/0/0`

자동 검증은 다음을 직접 포함한다.

- 동일 보험과 동일 유형의 재등록 후 active link `1`, inactive history `1`
- 연결 해제 후 active link `0`, inactive link와 Document metadata 유지
- 문서 열기 시 managed relative path만 사용하고 root 탈출·missing path를 launch 전에 거부
- 등록 및 다시 등록 진입 시 policy target과 document type preset 유지
- 데스크톱 타이틀 오른쪽 명령 배치와 좁은 폭 줄바꿈 계약

### P.5 Runtime State

- reused disposable synthetic runtime root: `C:\Users\jin8855\AppData\Local\Temp\FamilyClaimRef\T3_PER_A_R1_DocumentProjection_Recheck_20260805_144251_894`
- Product PID: `11092`
- window handle: `30610110`
- process state: open and responding for user review
- actual user data root access: `0`
- `data/claimdoc` access: `0`
- synthetic/evidence root cleanup: `0`

Windows Computer Use가 해당 handle을 다른 app owner로 잘못 귀속하여 입력을 중단했다. 이후 좌표 클릭이나 입력은 수행하지 않았다. handle 기반 OS 캡처는 black frame이어서 시각 PASS 증거로 사용하지 않는다. 따라서 자동 검증은 PASS지만 최종 UI 배치와 문서 관리 동작의 사용자 시각 확인은 계속 필요하다.

현재 상태:

- `T3_PER_A_R1_POLICY_DOCUMENT_ACTIVE_ONE_HISTORY_STATE = IMPLEMENTED`
- `T3_PER_A_R1_POLICY_DOCUMENT_MANAGEMENT_STATE = IMPLEMENTED`
- `T3_PER_A_R1_COMPACT_EDITOR_AUTOMATED_VALIDATION_STATE = PASS`
- `T3_PER_A_R1_COMPACT_EDITOR_VISUAL_REVIEW_STATE = AWAITING_USER`
- `T3_PER_A_R1_COMMIT_STATE = NOT_CREATED`
- `PRODUCTION_READINESS_STATE = NOT_EVALUATED`
- `DEPLOYMENT_STATE = NOT_AUTHORIZED`

`FAMILYCLAIMREF_T3_PER_A_R1_POLICY_DOCUMENT_LIFECYCLE_AND_COMPACT_EDITOR_READY_FOR_USER_REVIEW`

## Q. 2026-08-05 FU-04 Policy Document History Visibility Repair

### Q.1 User Observation and Decision

사용자는 보험 문서의 `다시 등록`과 `연결 해제` 동작은 정상임을 확인했지만,
보존된 이전 문서를 Product UI에서 확인할 수 없는 결함을 관찰했다.

사용자 결정에 따라 보험 등록/편집 화면의 `이 보험에 연결할 문서` 영역에
문서 이력을 추가한다. 화면의 세로 길이를 불필요하게 늘리지 않도록 이력은
기본 접힘 상태로 제공하며, 다음 정보만 표시한다.

- 문서 유형
- 문서 제목
- 등록일시
- 상태: `현재` 또는 `이력`
- 동작: `문서 열기`

이력 행에는 삭제 또는 재활성화 동작을 추가하지 않는다. 기존 활성 1건,
이력 보존, 다시 등록, 연결 해제 계약은 변경하지 않는다.

### Q.2 Root Cause and Minimal Repair

저장소는 비활성화된 policy-document link와 Document metadata 및 managed file을
이미 보존하고 있었다. 결함 원인은 `PolicyClaimManagementViewModel`이 active link만
조회하여 현재 문서 제목만 투영하고, 화면에 이력을 표시하는 projection이 없었던 것이다.

최소 수리는 다음과 같다.

- 선택한 보험의 모든 policy-document link와 연결 Document를 읽는다.
- 문서 유형별 최신 active link 1건만 `현재`로 분류한다.
- 나머지 link는 저장값을 변경하지 않고 `이력`으로 투영한다.
- raw link/document ID, SHA, 절대 경로는 이력 ViewModel에 노출하지 않는다.
- 이력 열기는 내부 managed relative path만 `IManagedDocumentOpener`로 전달한다.
- 이력 `Expander`는 기본 접힘이며, 이력이 있을 때만 표시하고 목록 높이는 `220`으로 제한한다.

Exact incremental files:

Production and resources:

- `app/FamilyClaimRef.App/Composition/AppServices.cs`
- `app/FamilyClaimRef.App/Resources/UiStrings.xaml`
- `app/FamilyClaimRef.App/Services/Localization/UiTextKeys.cs`
- `app/FamilyClaimRef.App/ViewModels/InsurancePolicyDocumentHistoryItemViewModel.cs`
- `app/FamilyClaimRef.App/ViewModels/PolicyClaimManagementViewModel.cs`
- `app/FamilyClaimRef.App/Views/ProductInsurancePolicyEditorView.xaml`
- `app/FamilyClaimRef.App/Views/ProductInsurancePolicyEditorView.xaml.cs`

Tests:

- `tests/FamilyClaimRef.App.Tests/DocumentRegistrationLifecycleGate8Tests.cs`
- `tests/FamilyClaimRef.App.Tests/InsurancePolicyManagementViewModelTests.cs`
- `tests/FamilyClaimRef.App.Tests/InsurancePolicyRevisionContractTests.cs`
- `tests/FamilyClaimRef.App.Tests/Services/Localization/ResourceUiTextProviderTests.cs`

Evidence:

- `docs/442_FAMILY_INSURANCE_PERSISTENCE_T3_PER_A_IMPLEMENTATION_AND_EVIDENCE.md`

### Q.3 Automated Validation

- isolated output solution build: `PASS`, warnings/errors `0/0`
- focused history, insurance, resource and Gate 8 tests: `196/196`, failed/skipped `0/0`
- full test suite: `739/739`, failed/skipped `0/0`
- resource/constants parity: `180/180`
- `Ui.Product.*` parity: `123/123`
- `git diff --check`: `PASS`
- staged/commit/push: `0/0/0`

기존 PID `11092`는 top-level window 없이 응답 중이며, `CloseMainWindow()`가
종료 요청을 전달하지 못했다. 강제 종료하지 않았다. 이 프로세스가 기존 Debug
output을 잠그므로 별도의 TEMP 및 ignored test `bin` output에서 빌드와 테스트를
수행했다. 첫 TEMP test 실행의 project-root 탐색 실패는 테스트 결함 판정에
사용하지 않았고, 저장소 하위 ignored output에서 같은 binary를 실행하여 전체
검증을 완료했다.

### Q.4 User Recheck Runtime

- source synthetic root preserved:
  `C:\Users\jin8855\AppData\Local\Temp\FamilyClaimRef\T3_PER_A_R1_DocumentProjection_Recheck_20260805_144251_894`
- new disposable synthetic runtime root:
  `C:\Users\jin8855\AppData\Local\Temp\FamilyClaimRef\T3_PER_A_R1_DocumentHistory_Recheck_20260805_160734_521`
- Product PID: `10420`
- initial window handle: `4265456`
- final observed window handle: `0`
- process state: responding without a top-level window
- old synthetic root cleanup: `0`
- new synthetic root cleanup: `0`
- actual user data root access: `0`
- `data/claimdoc` access: `0`

PID `10420`은 시작 직후 top-level handle을 생성했지만 최종 검사 시 handle이
`0`으로 바뀌었다. 강제 종료 또는 자동 재시도하지 않았다. 따라서 아래 항목의
사용자 시각 검토를 실행 가능한 상태로 과장하지 않으며 visible runtime 재실행이
별도로 필요하다.

사용자 확인 대기 항목:

1. 연결 문서 영역 아래에 `문서 이력 보기 (건수)`가 표시되는지
2. 기본 상태가 접힘인지
3. 펼쳤을 때 현재 문서와 다시 등록 전 문서가 `현재`/`이력`으로 구분되는지
4. 과거 이력의 `문서 열기`가 정상인지
5. 이력에 삭제 또는 재활성화 동작이 없는지

현재 상태:

- `T3_PER_A_R1_POLICY_DOCUMENT_HISTORY_VISIBILITY_STATE = IMPLEMENTED`
- `T3_PER_A_R1_POLICY_DOCUMENT_HISTORY_AUTOMATED_VALIDATION_STATE = PASS`
- `T3_PER_A_R1_POLICY_DOCUMENT_HISTORY_USER_REVIEW_STATE = VISIBLE_RUNTIME_RELAUNCH_REQUIRED`
- `T3_PER_A_R1_COMMIT_STATE = NOT_CREATED`
- `PRODUCTION_READINESS_STATE = NOT_EVALUATED`
- `DEPLOYMENT_STATE = NOT_AUTHORIZED`

`FAMILYCLAIMREF_T3_PER_A_R1_POLICY_DOCUMENT_HISTORY_AUTOMATED_VALIDATION_PASS_VISIBLE_RUNTIME_RELAUNCH_REQUIRED`

## R. Current Authoritative Status Reconciliation

Q절의 `VISIBLE_RUNTIME_RELAUNCH_REQUIRED`는 당시 자동화 검증 직후의 역사적 상태다.
후속 사용자 검토와 집중 회귀 및 독립 테스트가 완료되었으므로 현재 authoritative
상태는 다음과 같다. 이 상태는 production readiness 또는 deployment 승격을 의미하지 않는다.

- `VR-01~VR-05 = PRESERVED`
- `VR-05 = USER_ACCEPTED`
- FU-07 related regression: `68/68 PASS`
- FU-08 independent test: `1/1 PASS`
- `T3_PER_A_R1_FU_06_MINOR_TEST_GAP_STATE = REPAIRED_AND_INDEPENDENTLY_VERIFIED`
- `T3_PER_A_R1_POLICY_DOCUMENT_HISTORY_FEATURE_STATE = PASS`
- `T3_PER_A_R1_COMMIT_STATE = NOT_CREATED`
- `PRODUCTION_READINESS_STATE = NOT_EVALUATED`
- `DEPLOYMENT_STATE = NOT_AUTHORIZED`
