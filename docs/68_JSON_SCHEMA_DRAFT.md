# JSON Schema Draft

## A. Goal

이 문서는 JSON file storage 구현 전에 `Document`, `PolicyDocument`, `ClaimDocument` 중심의 JSON schema 초안을 정리한다.

초안 범위는 다음 JSON file 후보의 구조를 문서로만 정의하는 것이다.

- `documents.json`
- `policy-documents.json`
- `claim-documents.json`

이 문서는 구현 문서가 아니다. 실제 JSON file 생성, C# model 구현, C# interface 구현, storage service 구현, repository 구현, DB/OCR/metadata/file storage 구현은 수행하지 않는다.

## B. Current State

- WPF scaffold는 생성되어 있다.
- WPF app TargetFramework는 `net10.0-windows`이다.
- `FileNamePolicyService`는 구현되어 있다.
- `FileNamePolicyService` 테스트 프로젝트가 존재하며 최근 검토 기준 PASS 상태이다.
- JSON file storage 방향은 사용자 결정으로 정리되어 있다.
- `Document`, `PolicyDocument`, `ClaimDocument` 저장 구조는 사용자 결정으로 정리되어 있다.
- `CategoryItem`과 document type 연결 정책은 사용자 결정으로 정리되어 있다.
- storage service interface 방향은 사용자 결정으로 정리되어 있다.
- JSON schema 구현은 아직 없다.
- C# model/interface 구현은 아직 없다.
- 실제 JSON file 생성은 아직 없다.
- `data/local/`, `attachments/` 내부 파일 생성은 이 단계에서 수행하지 않는다.

## C. Schema Design Principles

- JSON files는 implementation detail이다.
- ViewModel은 JSON schema, JSON file path, JSON DTO를 직접 알지 않는다.
- JSON schema는 이후 SQLite migration 가능성을 고려한다.
- record 간 연결은 id-based reference를 사용한다.
- 실제 삭제 대신 `disabledAt`을 사용한다.
- raw `originalFileName`은 저장하지 않는다.
- OCR temporary result는 저장하지 않는다.
- `documentType`은 domain link record에만 저장한다.
- `Document.documentType`은 저장하지 않는다.
- `isDisabled`는 저장하지 않고 `disabledAt != null`에서 파생한다.
- `memo`는 MVP 1차에서 제외한다.
- 예시는 실제 개인정보, 실제 가족명, 실제 보험사명, 실제 병원명, 실제 진단명, 실제 진단코드를 사용하지 않는다.

## D. File Unit Candidate

실제 파일 생성 없이, 다음 file unit을 후보로 둔다.

- `data/local/documents.json`
- `data/local/policy-documents.json`
- `data/local/claim-documents.json`

주의: 이 문서는 위 경로를 schema 후보로만 기록한다. `data/local` 내부에 실제 JSON file을 생성하지 않는다.

## E. Common Metadata Envelope 후보

### Option A. Array Only

```json
[
  {
    "id": "doc_000001"
  }
]
```

장점:

- 구조가 단순하다.
- MVP 초기 구현이 빠르다.

단점:

- `schemaVersion`을 둘 위치가 없다.
- file-level `savedAt`을 둘 위치가 없다.
- 이후 migration 기준이 약하다.

### Option B. Envelope + Items

```json
{
  "schemaVersion": 1,
  "savedAt": "2026-06-29T00:00:00Z",
  "items": []
}
```

장점:

- `schemaVersion`을 명시할 수 있다.
- file-level `savedAt`을 기록할 수 있다.
- 이후 schema migration 기준을 만들기 쉽다.

단점:

- array only보다 구현이 조금 더 복잡하다.
- read/write service가 envelope를 처리해야 한다.

Candidate Recommendation:

- `Envelope + Items` 구조를 사용한다.
- 최초 `schemaVersion`은 `1`로 둔다.
- `savedAt`은 UTC ISO-8601 string으로 기록한다.

## F. `documents.json` Schema 후보

`DocumentRecord`는 실제 첨부 파일의 공통 metadata record 후보이다.

| Field | Status | Description |
|---|---|---|
| `id` | Required Candidate | document id |
| `physicalFileName` | Required Candidate | `FileNamePolicyService` 결과 파일명 |
| `displayTitle` | Required Candidate | 내부 표시용 이름 |
| `extension` | Required Candidate | `pdf`, `jpg`, `jpeg`, `png` |
| `relativePath` | Required Candidate | `attachments/` 기준 상대 경로 |
| `createdAt` | Required Candidate | UTC ISO-8601 |
| `updatedAt` | Required Candidate | UTC ISO-8601 |
| `disabledAt` | Nullable Candidate | 사용 중지 source of truth |
| `documentType` | Excluded | MVP 1차 persisted field에서 제외 |
| `originalFileName` | Excluded | raw original filename 저장 금지 |
| `isDisabled` | Excluded / Derived | `disabledAt != null`에서 파생 |
| `memo` | Excluded | MVP 1차 제외 |

Example:

```json
{
  "schemaVersion": 1,
  "savedAt": "2026-06-29T00:00:00Z",
  "items": [
    {
      "id": "doc_000001",
      "physicalFileName": "claim_20260629_doc_000001.pdf",
      "displayTitle": "청구 서류 A",
      "extension": "pdf",
      "relativePath": "2026/06/doc_000001.pdf",
      "createdAt": "2026-06-29T00:00:00Z",
      "updatedAt": "2026-06-29T00:00:00Z",
      "disabledAt": null
    }
  ]
}
```

## G. `policy-documents.json` Schema 후보

`PolicyDocumentRecord`는 `Policy`와 `Document`를 연결하는 domain link record 후보이다.

| Field | Status | Description |
|---|---|---|
| `id` | Required Candidate | policy-document id |
| `policyId` | Required Candidate | policy record reference |
| `documentId` | Required Candidate | `Document.id` reference |
| `documentType` | Required Candidate | policy document type source of truth |
| `createdAt` | Required Candidate | UTC ISO-8601 |
| `updatedAt` | Required Candidate | UTC ISO-8601 |
| `disabledAt` | Nullable Candidate | 사용 중지 source of truth |
| `memo` | Excluded | MVP 1차 제외 |

현재 policy scope document type allowlist:

- `policy`
- `terms`
- `contract`
- `capture`
- `etc`

Example:

```json
{
  "schemaVersion": 1,
  "savedAt": "2026-06-29T00:00:00Z",
  "items": [
    {
      "id": "pdoc_000001",
      "policyId": "policy_000001",
      "documentId": "doc_000001",
      "documentType": "policy",
      "createdAt": "2026-06-29T00:00:00Z",
      "updatedAt": "2026-06-29T00:00:00Z",
      "disabledAt": null
    }
  ]
}
```

## H. `claim-documents.json` Schema 후보

`ClaimDocumentRecord`는 `ClaimCase`와 `Document`를 연결하는 domain link record 후보이다.

| Field | Status | Description |
|---|---|---|
| `id` | Required Candidate | claim-document id |
| `claimId` | Required Candidate | claim record reference |
| `documentId` | Required Candidate | `Document.id` reference |
| `documentType` | Required Candidate | claim document type source of truth |
| `createdAt` | Required Candidate | UTC ISO-8601 |
| `updatedAt` | Required Candidate | UTC ISO-8601 |
| `disabledAt` | Nullable Candidate | 사용 중지 source of truth |
| `ocrConfirmedFieldsSnapshot` | Deferred | 별도 결정 전까지 저장하지 않음 |
| `memo` | Excluded | MVP 1차 제외 |

현재 claim scope document type allowlist:

- `receipt`
- `diagnosis`
- `medicine`
- `visit`
- `admission`
- `surgery`
- `etc`

New Candidate / Needs Implementation:

- `statement`
- `prescription`
- claim scope `capture`

주의:

- New Candidate는 현재 schema에서 자동 허용하지 않는다.
- New Candidate를 허용하려면 별도 정책 결정, `FileNamePolicyService` patch, 테스트 갱신이 필요하다.

Example:

```json
{
  "schemaVersion": 1,
  "savedAt": "2026-06-29T00:00:00Z",
  "items": [
    {
      "id": "cdoc_000001",
      "claimId": "claim_000001",
      "documentId": "doc_000001",
      "documentType": "receipt",
      "createdAt": "2026-06-29T00:00:00Z",
      "updatedAt": "2026-06-29T00:00:00Z",
      "disabledAt": null
    }
  ]
}
```

## I. Reference Validation Candidate

Candidate rules:

- `PolicyDocument.documentId`는 `documents.json`의 기존 `Document.id`를 참조해야 한다.
- `ClaimDocument.documentId`는 `documents.json`의 기존 `Document.id`를 참조해야 한다.
- `PolicyDocument.policyId`는 향후 `policies.json`의 `Policy.id`를 참조해야 한다.
- `ClaimDocument.claimId`는 향후 `claims.json`의 `Claim.id`를 참조해야 한다.
- disabled `Document`를 신규 `PolicyDocument` 또는 `ClaimDocument`에 연결할 수 있는지는 별도 결정 후보이다.
- disabled `PolicyDocument` 또는 `ClaimDocument`를 기본 조회에서 제외할지는 별도 결정 후보이다.
- 이 문서에서는 validation 구현을 수행하지 않는다.
- reference validation은 JSON implementation/storage service 승인 단계에서 결정한다.

## J. Schema Version / Migration Candidate

- 각 JSON file envelope는 `schemaVersion: 1`을 저장한다.
- `schemaVersion`이 없거나 예상 값과 다를 때의 load policy가 필요하다.
- MVP 1차에서는 migration을 구현하지 않는다.
- load failure는 error state/message policy가 필요하다.
- schema migration은 별도 문서에서 결정한다.

## K. Draft / Record Contract Mapping

| Contract | Role | Notes |
|---|---|---|
| `DocumentDraft` | save request input | caller가 저장 요청에 넘기는 입력 후보 |
| `DocumentRecord` | persisted state | `documents.json` item 후보 |
| `PolicyDocumentDraft` | save request input | `policyId`, `documentId`, `documentType` 입력 후보 |
| `PolicyDocumentRecord` | persisted state | `policy-documents.json` item 후보 |
| `ClaimDocumentDraft` | save request input | `claimId`, `documentId`, `documentType` 입력 후보 |
| `ClaimDocumentRecord` | persisted state | `claim-documents.json` item 후보 |

Criteria:

- Draft는 save request input이다.
- Record는 persisted state이다.
- `id`, `createdAt`, `updatedAt`을 service가 생성할지 caller가 전달할지는 Needs Decision이다.
- Record는 persisted shape에 가깝게 둔다.
- ViewModel은 Draft/Record에 직접 의존하지 않는 방향을 유지한다.
- JSON DTO는 implementation detail이다.

## L. Needs Decision

1. JSON files를 `documents.json`, `policy-documents.json`, `claim-documents.json`으로 분리할 것인가?
2. 각 JSON file은 envelope + items 구조를 사용할 것인가?
3. 각 JSON file에 `schemaVersion`을 둘 것인가?
4. 각 JSON envelope에 `savedAt`을 둘 것인가?
5. `DocumentRecord`는 `id`, `physicalFileName`, `displayTitle`, `extension`, `relativePath`, `createdAt`, `updatedAt`, `disabledAt`만 둘 것인가?
6. `DocumentRecord.documentType`을 제외할 것인가?
7. `DocumentRecord.originalFileName`을 제외할 것인가?
8. `isDisabled`는 저장하지 않고 파생 상태로 둘 것인가?
9. `PolicyDocumentRecord.documentType`을 source of truth로 둘 것인가?
10. `ClaimDocumentRecord.documentType`을 source of truth로 둘 것인가?
11. `ocrConfirmedFieldsSnapshot`은 별도 결정 전까지 제외할 것인가?
12. `memo`는 MVP 1차에서 제외할 것인가?
13. reference validation 세부 규칙은 JSON implementation design doc으로 미룰 것인가?
14. schema migration은 MVP 1차에서 구현하지 않고 deferred로 둘 것인가?
15. 이번 단계에서는 실제 JSON file을 생성하지 않는가?

## M. Out of Scope

- actual JSON file creation
- C# model implementation
- C# interface implementation
- JSON storage implementation
- SQLite DB creation
- SQLite package addition
- repository/data access/migration implementation
- CategoryItem implementation/storage
- storage service implementation
- actual file copy/storage
- `attachments/` 내부 파일 생성
- `data/local/` 내부 파일 생성
- DB/OCR/metadata/file storage implementation
- WPF UI/XAML/navigation implementation
- test code/test project/package work
- real privacy sample usage

## N. Risks

- `schemaVersion`이 없으면 이후 migration 비용이 커진다.
- reference validation을 늦게 정의하면 잘못된 `documentId`, `policyId`, `claimId`가 저장될 수 있다.
- Draft/Record 경계가 불명확하면 service 책임이 흔들릴 수 있다.
- `Document.documentType`을 제외하면 document type 조회는 domain link record join/lookup에 의존한다.
- envelope `savedAt`과 record-level `updatedAt`이 서로 어긋날 수 있다.
- ViewModel이 JSON schema를 알게 되면 SQLite migration이 어려워진다.

## O. Recommendation

1. 이 문서를 기준으로 JSON schema 방향에 대한 사용자 결정을 받는다.
2. 사용자 결정 후 `docs/69_JSON_SCHEMA_USER_DECISION_RECORD.md`를 생성한다.
3. 이후 C# model/interface 구현 범위 결정 문서를 생성한다.
4. document type seed constant 구현 승인을 별도로 받는다.
5. JSON file storage 구현 승인을 별도로 받는다.

## P. Result

`JSON_SCHEMA_DRAFTED`
