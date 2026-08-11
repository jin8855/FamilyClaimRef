using System.Text.Json;
using System.Text.Json.Nodes;
using FamilyClaimRef.App.Models.Storage;
using FamilyClaimRef.App.Services.Storage;
using Xunit;

namespace FamilyClaimRef.App.Tests;

public sealed class JsonPolicyCoverageStorageServiceTests
{
    [Fact]
    public async Task Missing_store_loads_empty_without_creating_a_file()
    {
        await UsingFixtureAsync(async fixture =>
        {
            Assert.Empty(await fixture.Coverages.GetPolicyCoveragesAsync());
            Assert.False(File.Exists(fixture.CoveragePath));
        });
    }

    [Fact]
    public async Task Create_normalizes_and_round_trips_schema_one_record()
    {
        await UsingFixtureAsync(async fixture =>
        {
            var policy = await fixture.CreateActivePolicyAsync();

            var created = await fixture.Coverages.CreatePolicyCoverageAsync(
                CreateDraft(policy.Id) with
                {
                    DisplayName = "  coverage A  ",
                    DiagnosisCodePrefixes = [" a00.1 ", "A00.1", " b-2 "],
                    SourceLocator = "  private-locator  ",
                    Memo = "  memo  "
                });

            Assert.Equal(1, created.Revision);
            Assert.Equal("coverage A", created.DisplayName);
            Assert.Equal(["A00.1", "B-2"], created.DiagnosisCodePrefixes);
            Assert.Equal("private-locator", created.SourceLocator);
            Assert.Equal("memo", created.Memo);
            Assert.Null(created.DisabledAt);

            var reloaded = await fixture.CreateCoverageService().GetPolicyCoverageAsync(
                created.PolicyCoverageId);
            AssertRecordEqual(created, reloaded);

            using var json = JsonDocument.Parse(await File.ReadAllTextAsync(fixture.CoveragePath));
            Assert.Equal(1, json.RootElement.GetProperty("schemaVersion").GetInt32());
            Assert.Single(json.RootElement.GetProperty("items").EnumerateArray());
        });
    }

    [Theory]
    [InlineData(PolicyCoverageValues.ReviewStatusUserConfirmed)]
    [InlineData(PolicyCoverageValues.ReviewStatusIgnored)]
    public async Task Create_rejects_non_initial_review_status_without_write(string reviewStatus)
    {
        await UsingFixtureAsync(async fixture =>
        {
            var policy = await fixture.CreateActivePolicyAsync();

            var exception = await Assert.ThrowsAsync<PolicyCoverageStorageException>(() =>
                fixture.Coverages.CreatePolicyCoverageAsync(
                    CreateDraft(policy.Id) with { ReviewStatus = reviewStatus }));

            Assert.Equal(PolicyCoverageStorageErrorCode.InvalidTransition, exception.ErrorCode);
            Assert.False(File.Exists(fixture.CoveragePath));
        });
    }

    [Theory]
    [InlineData("unknown", PolicyCoverageValues.ConditionAny, PolicyCoverageValues.DiagnosisRuleAny)]
    [InlineData(PolicyCoverageValues.VisitTypeAny, "unknown", PolicyCoverageValues.DiagnosisRuleAny)]
    [InlineData(PolicyCoverageValues.VisitTypeAny, PolicyCoverageValues.ConditionAny, "unknown")]
    public async Task Create_rejects_unknown_rule_values_without_write(
        string visitTypeRule,
        string surgeryRule,
        string diagnosisRuleMode)
    {
        await UsingFixtureAsync(async fixture =>
        {
            var policy = await fixture.CreateActivePolicyAsync();

            var exception = await Assert.ThrowsAsync<PolicyCoverageStorageException>(() =>
                fixture.Coverages.CreatePolicyCoverageAsync(CreateDraft(policy.Id) with
                {
                    VisitTypeRule = visitTypeRule,
                    SurgeryRule = surgeryRule,
                    DiagnosisRuleMode = diagnosisRuleMode,
                    DiagnosisCodePrefixes = diagnosisRuleMode == PolicyCoverageValues.DiagnosisRulePrefixList
                        ? ["A00"]
                        : []
                }));

            Assert.Equal(PolicyCoverageStorageErrorCode.IntegrityViolation, exception.ErrorCode);
            Assert.False(File.Exists(fixture.CoveragePath));
        });
    }

    [Fact]
    public async Task Create_rejects_empty_prefix_list_and_reversed_dates_without_write()
    {
        await UsingFixtureAsync(async fixture =>
        {
            var policy = await fixture.CreateActivePolicyAsync();
            var prefixException = await Assert.ThrowsAsync<PolicyCoverageStorageException>(() =>
                fixture.Coverages.CreatePolicyCoverageAsync(CreateDraft(policy.Id) with
                {
                    DiagnosisCodePrefixes = []
                }));
            var dateException = await Assert.ThrowsAsync<PolicyCoverageStorageException>(() =>
                fixture.Coverages.CreatePolicyCoverageAsync(CreateDraft(policy.Id) with
                {
                    EffectiveFrom = new DateOnly(2027, 1, 1),
                    EffectiveTo = new DateOnly(2026, 1, 1)
                }));

            Assert.Equal(PolicyCoverageStorageErrorCode.IntegrityViolation, prefixException.ErrorCode);
            Assert.Equal(PolicyCoverageStorageErrorCode.IntegrityViolation, dateException.ErrorCode);
            Assert.False(File.Exists(fixture.CoveragePath));
        });
    }

    [Fact]
    public async Task Create_rejects_orphan_and_disabled_policy_without_write()
    {
        await UsingFixtureAsync(async fixture =>
        {
            var orphanException = await Assert.ThrowsAsync<PolicyCoverageStorageException>(() =>
                fixture.Coverages.CreatePolicyCoverageAsync(CreateDraft("policy_missing")));
            var policy = await fixture.CreateActivePolicyAsync();
            await fixture.Policies.DisablePolicyAsync(policy.Id);
            var disabledException = await Assert.ThrowsAsync<PolicyCoverageStorageException>(() =>
                fixture.Coverages.CreatePolicyCoverageAsync(CreateDraft(policy.Id)));

            Assert.Equal(PolicyCoverageStorageErrorCode.ReferenceInvalid, orphanException.ErrorCode);
            Assert.Equal(PolicyCoverageStorageErrorCode.ReferenceInvalid, disabledException.ErrorCode);
            Assert.False(File.Exists(fixture.CoveragePath));
        });
    }

    [Fact]
    public async Task Policy_document_source_accepts_same_policy_history_and_rejects_other_references()
    {
        await UsingFixtureAsync(async fixture =>
        {
            var policyA = await fixture.CreateActivePolicyAsync("policy A");
            var policyB = await fixture.CreateActivePolicyAsync("policy B");
            var linkA = await fixture.CreatePolicyDocumentAsync(policyA.Id);
            var linkB = await fixture.CreatePolicyDocumentAsync(policyB.Id);
            await fixture.Documents.DisablePolicyDocumentAsync(
                linkA.Id,
                DateTimeOffset.UtcNow);

            var valid = await fixture.Coverages.CreatePolicyCoverageAsync(
                CreateDraft(policyA.Id) with
                {
                    SourceKind = PolicyCoverageValues.SourcePolicyDocument,
                    SourcePolicyDocumentId = linkA.Id
                });
            Assert.Equal(linkA.Id, valid.SourcePolicyDocumentId);

            foreach (var invalidSourceId in new[] { linkB.Id, "pdoc_missing" })
            {
                var before = await File.ReadAllBytesAsync(fixture.CoveragePath);
                var exception = await Assert.ThrowsAsync<PolicyCoverageStorageException>(() =>
                    fixture.Coverages.CreatePolicyCoverageAsync(
                        CreateDraft(policyA.Id) with
                        {
                            SourceKind = PolicyCoverageValues.SourcePolicyDocument,
                            SourcePolicyDocumentId = invalidSourceId
                        }));
                Assert.Equal(PolicyCoverageStorageErrorCode.ReferenceInvalid, exception.ErrorCode);
                Assert.Equal(before, await File.ReadAllBytesAsync(fixture.CoveragePath));
            }
        });
    }

    [Fact]
    public async Task Manual_source_allows_null_or_same_policy_document_reference()
    {
        await UsingFixtureAsync(async fixture =>
        {
            var policy = await fixture.CreateActivePolicyAsync();
            var otherPolicy = await fixture.CreateActivePolicyAsync("policy B");
            var link = await fixture.CreatePolicyDocumentAsync(policy.Id);
            var otherLink = await fixture.CreatePolicyDocumentAsync(otherPolicy.Id);

            var withoutDocument = await fixture.Coverages.CreatePolicyCoverageAsync(
                CreateDraft(policy.Id) with
                {
                    DiagnosisRuleMode = PolicyCoverageValues.DiagnosisRuleAny,
                    DiagnosisCodePrefixes = [],
                    SourcePolicyDocumentId = null
                });
            var withDocument = await fixture.Coverages.CreatePolicyCoverageAsync(
                CreateDraft(policy.Id) with { SourcePolicyDocumentId = link.Id });

            Assert.Null(withoutDocument.SourcePolicyDocumentId);
            Assert.Equal(link.Id, withDocument.SourcePolicyDocumentId);

            var before = await File.ReadAllBytesAsync(fixture.CoveragePath);
            await AssertErrorAsync(
                PolicyCoverageStorageErrorCode.ReferenceInvalid,
                () => fixture.Coverages.CreatePolicyCoverageAsync(
                    CreateDraft(policy.Id) with { SourcePolicyDocumentId = otherLink.Id }));
            Assert.Equal(before, await File.ReadAllBytesAsync(fixture.CoveragePath));
        });
    }

    [Fact]
    public async Task Update_preserves_identity_and_demotes_confirmed_after_rule_change()
    {
        await UsingFixtureAsync(async fixture =>
        {
            var policy = await fixture.CreateActivePolicyAsync();
            var created = await fixture.Coverages.CreatePolicyCoverageAsync(CreateDraft(policy.Id));
            var confirmed = await fixture.Coverages.ChangePolicyCoverageReviewStatusAsync(
                created.PolicyCoverageId,
                created.Revision,
                PolicyCoverageValues.ReviewStatusUserConfirmed);

            var updated = await fixture.Coverages.UpdatePolicyCoverageAsync(
                confirmed.PolicyCoverageId,
                confirmed.Revision,
                CreateUpdateDraft(confirmed) with
                {
                    VisitTypeRule = PolicyCoverageValues.VisitTypeInpatient
                });

            Assert.Equal(confirmed.PolicyCoverageId, updated.PolicyCoverageId);
            Assert.Equal(confirmed.PolicyId, updated.PolicyId);
            Assert.Equal(confirmed.CreatedAt, updated.CreatedAt);
            Assert.Equal(confirmed.DisabledAt, updated.DisabledAt);
            Assert.Equal(confirmed.Revision + 1, updated.Revision);
            Assert.Equal(PolicyCoverageValues.ReviewStatusNeedsReview, updated.ReviewStatus);
        });
    }

    [Fact]
    public async Task Update_keeps_confirmed_when_only_display_name_and_memo_change()
    {
        await UsingFixtureAsync(async fixture =>
        {
            var policy = await fixture.CreateActivePolicyAsync();
            var created = await fixture.Coverages.CreatePolicyCoverageAsync(CreateDraft(policy.Id));
            var confirmed = await fixture.Coverages.ChangePolicyCoverageReviewStatusAsync(
                created.PolicyCoverageId,
                created.Revision,
                PolicyCoverageValues.ReviewStatusUserConfirmed);

            var updated = await fixture.Coverages.UpdatePolicyCoverageAsync(
                confirmed.PolicyCoverageId,
                confirmed.Revision,
                CreateUpdateDraft(confirmed) with
                {
                    DisplayName = "renamed",
                    Memo = "updated memo"
                });

            Assert.Equal(PolicyCoverageValues.ReviewStatusUserConfirmed, updated.ReviewStatus);
            Assert.Equal("renamed", updated.DisplayName);
            Assert.Equal("updated memo", updated.Memo);
        });
    }

    [Theory]
    [InlineData(PolicyCoverageValues.ReviewStatusCandidate, PolicyCoverageValues.ReviewStatusNeedsReview)]
    [InlineData(PolicyCoverageValues.ReviewStatusCandidate, PolicyCoverageValues.ReviewStatusUserConfirmed)]
    [InlineData(PolicyCoverageValues.ReviewStatusCandidate, PolicyCoverageValues.ReviewStatusIgnored)]
    [InlineData(PolicyCoverageValues.ReviewStatusNeedsReview, PolicyCoverageValues.ReviewStatusUserConfirmed)]
    [InlineData(PolicyCoverageValues.ReviewStatusNeedsReview, PolicyCoverageValues.ReviewStatusIgnored)]
    [InlineData(PolicyCoverageValues.ReviewStatusUserConfirmed, PolicyCoverageValues.ReviewStatusNeedsReview)]
    [InlineData(PolicyCoverageValues.ReviewStatusUserConfirmed, PolicyCoverageValues.ReviewStatusIgnored)]
    [InlineData(PolicyCoverageValues.ReviewStatusIgnored, PolicyCoverageValues.ReviewStatusNeedsReview)]
    public async Task Review_status_allows_only_contract_transitions(
        string currentStatus,
        string targetStatus)
    {
        await UsingFixtureAsync(async fixture =>
        {
            var policy = await fixture.CreateActivePolicyAsync();
            var current = await CreateInStatusAsync(fixture, policy.Id, currentStatus);

            var changed = await fixture.Coverages.ChangePolicyCoverageReviewStatusAsync(
                current.PolicyCoverageId,
                current.Revision,
                targetStatus);

            Assert.Equal(targetStatus, changed.ReviewStatus);
            Assert.Equal(current.Revision + 1, changed.Revision);
        });
    }

    [Theory]
    [InlineData(PolicyCoverageValues.ReviewStatusCandidate, PolicyCoverageValues.ReviewStatusCandidate)]
    [InlineData(PolicyCoverageValues.ReviewStatusNeedsReview, PolicyCoverageValues.ReviewStatusCandidate)]
    [InlineData(PolicyCoverageValues.ReviewStatusUserConfirmed, PolicyCoverageValues.ReviewStatusCandidate)]
    [InlineData(PolicyCoverageValues.ReviewStatusIgnored, PolicyCoverageValues.ReviewStatusUserConfirmed)]
    public async Task Review_status_rejects_same_or_forbidden_transition_without_write(
        string currentStatus,
        string targetStatus)
    {
        await UsingFixtureAsync(async fixture =>
        {
            var policy = await fixture.CreateActivePolicyAsync();
            var current = await CreateInStatusAsync(fixture, policy.Id, currentStatus);
            var before = await File.ReadAllBytesAsync(fixture.CoveragePath);

            var exception = await Assert.ThrowsAsync<PolicyCoverageStorageException>(() =>
                fixture.Coverages.ChangePolicyCoverageReviewStatusAsync(
                    current.PolicyCoverageId,
                    current.Revision,
                    targetStatus));

            Assert.Equal(PolicyCoverageStorageErrorCode.InvalidTransition, exception.ErrorCode);
            Assert.Equal(before, await File.ReadAllBytesAsync(fixture.CoveragePath));
        });
    }

    [Fact]
    public async Task Disable_and_restore_preserve_review_status_and_increment_revision()
    {
        await UsingFixtureAsync(async fixture =>
        {
            var policy = await fixture.CreateActivePolicyAsync();
            var created = await fixture.Coverages.CreatePolicyCoverageAsync(CreateDraft(policy.Id));
            var confirmed = await fixture.Coverages.ChangePolicyCoverageReviewStatusAsync(
                created.PolicyCoverageId,
                created.Revision,
                PolicyCoverageValues.ReviewStatusUserConfirmed);

            var disabled = await fixture.Coverages.DisablePolicyCoverageAsync(
                confirmed.PolicyCoverageId,
                confirmed.Revision);
            Assert.NotNull(disabled.DisabledAt);
            Assert.Equal(disabled.DisabledAt, disabled.UpdatedAt);
            Assert.Equal(confirmed.ReviewStatus, disabled.ReviewStatus);
            Assert.DoesNotContain(
                await fixture.Coverages.GetActivePolicyCoveragesAsync(),
                item => item.PolicyCoverageId == disabled.PolicyCoverageId);

            var restored = await fixture.Coverages.RestorePolicyCoverageAsync(
                disabled.PolicyCoverageId,
                disabled.Revision);
            Assert.Null(restored.DisabledAt);
            Assert.Equal(disabled.Revision + 1, restored.Revision);
            Assert.Equal(disabled.ReviewStatus, restored.ReviewStatus);
        });
    }

    [Fact]
    public async Task Disabled_parent_allows_query_and_disable_but_blocks_update_status_and_restore()
    {
        await UsingFixtureAsync(async fixture =>
        {
            var policy = await fixture.CreateActivePolicyAsync();
            var created = await fixture.Coverages.CreatePolicyCoverageAsync(CreateDraft(policy.Id));
            await fixture.Policies.DisablePolicyAsync(policy.Id);

            var queried = await fixture.Coverages.GetPolicyCoverageAsync(created.PolicyCoverageId);
            Assert.NotNull(queried);
            Assert.Null(queried.DisabledAt);
            await AssertErrorAsync(
                PolicyCoverageStorageErrorCode.ReferenceInvalid,
                () => fixture.Coverages.UpdatePolicyCoverageAsync(
                    created.PolicyCoverageId,
                    created.Revision,
                    CreateUpdateDraft(created)));
            await AssertErrorAsync(
                PolicyCoverageStorageErrorCode.ReferenceInvalid,
                () => fixture.Coverages.ChangePolicyCoverageReviewStatusAsync(
                    created.PolicyCoverageId,
                    created.Revision,
                    PolicyCoverageValues.ReviewStatusUserConfirmed));

            var disabled = await fixture.Coverages.DisablePolicyCoverageAsync(
                created.PolicyCoverageId,
                created.Revision);
            await AssertErrorAsync(
                PolicyCoverageStorageErrorCode.ReferenceInvalid,
                () => fixture.Coverages.RestorePolicyCoverageAsync(
                    disabled.PolicyCoverageId,
                    disabled.Revision));
        });
    }

    [Fact]
    public async Task Stale_revision_is_no_write_version_conflict()
    {
        await UsingFixtureAsync(async fixture =>
        {
            var policy = await fixture.CreateActivePolicyAsync();
            var created = await fixture.Coverages.CreatePolicyCoverageAsync(CreateDraft(policy.Id));
            var updated = await fixture.Coverages.UpdatePolicyCoverageAsync(
                created.PolicyCoverageId,
                created.Revision,
                CreateUpdateDraft(created) with { DisplayName = "first update" });
            var before = await File.ReadAllBytesAsync(fixture.CoveragePath);

            await AssertErrorAsync(
                PolicyCoverageStorageErrorCode.VersionConflict,
                () => fixture.Coverages.UpdatePolicyCoverageAsync(
                    updated.PolicyCoverageId,
                    created.Revision,
                    CreateUpdateDraft(updated) with { DisplayName = "stale update" }));

            Assert.Equal(before, await File.ReadAllBytesAsync(fixture.CoveragePath));
            AssertRecordEqual(
                updated,
                await fixture.Coverages.GetPolicyCoverageAsync(updated.PolicyCoverageId));
        });
    }

    [Fact]
    public async Task Two_service_instances_prevent_lost_update_for_same_revision()
    {
        await UsingFixtureAsync(async fixture =>
        {
            var policy = await fixture.CreateActivePolicyAsync();
            var created = await fixture.Coverages.CreatePolicyCoverageAsync(CreateDraft(policy.Id));
            var first = fixture.CreateCoverageService();
            var second = fixture.CreateCoverageService();

            var attempts = await Task.WhenAll(
                CaptureAsync(() => first.UpdatePolicyCoverageAsync(
                    created.PolicyCoverageId,
                    created.Revision,
                    CreateUpdateDraft(created) with { DisplayName = "writer one" })),
                CaptureAsync(() => second.UpdatePolicyCoverageAsync(
                    created.PolicyCoverageId,
                    created.Revision,
                    CreateUpdateDraft(created) with { DisplayName = "writer two" })));

            Assert.Single(attempts, attempt => attempt.Record is not null);
            var conflict = Assert.Single(attempts, attempt => attempt.Exception is not null);
            Assert.Equal(
                PolicyCoverageStorageErrorCode.VersionConflict,
                conflict.Exception!.ErrorCode);
            Assert.Equal(
                2,
                (await fixture.Coverages.GetPolicyCoverageAsync(created.PolicyCoverageId))!.Revision);
            Assert.Empty(Directory.GetFiles(fixture.RootPath, "*.tmp", SearchOption.TopDirectoryOnly));
        });
    }

    [Fact]
    public async Task Duplicate_coverage_id_load_fails_closed_without_rewrite()
    {
        await UsingFixtureAsync(async fixture =>
        {
            var policy = await fixture.CreateActivePolicyAsync();
            var created = await fixture.Coverages.CreatePolicyCoverageAsync(CreateDraft(policy.Id));
            var root = JsonNode.Parse(await File.ReadAllTextAsync(fixture.CoveragePath))!.AsObject();
            var items = root["items"]!.AsArray();
            items.Add(items[0]!.DeepClone());
            await File.WriteAllTextAsync(fixture.CoveragePath, root.ToJsonString(JsonOptions));
            var before = await File.ReadAllBytesAsync(fixture.CoveragePath);

            await AssertErrorAsync(
                PolicyCoverageStorageErrorCode.IntegrityViolation,
                () => fixture.Coverages.GetPolicyCoverageAsync(created.PolicyCoverageId));

            Assert.Equal(before, await File.ReadAllBytesAsync(fixture.CoveragePath));
        });
    }

    [Theory]
    [InlineData("malformed")]
    [InlineData("schema")]
    public async Task Malformed_or_unknown_schema_load_fails_closed_without_rewrite(string caseName)
    {
        await UsingFixtureAsync(async fixture =>
        {
            Directory.CreateDirectory(fixture.RootPath);
            if (caseName == "malformed")
            {
                await File.WriteAllTextAsync(fixture.CoveragePath, "{ not-json");
            }
            else
            {
                await File.WriteAllTextAsync(
                    fixture.CoveragePath,
                    """
                    {
                      "schemaVersion": 2,
                      "savedAt": "2026-08-11T00:00:00Z",
                      "items": []
                    }
                    """);
            }

            var before = await File.ReadAllBytesAsync(fixture.CoveragePath);
            await AssertErrorAsync(
                PolicyCoverageStorageErrorCode.IntegrityViolation,
                () => fixture.Coverages.GetPolicyCoveragesAsync());
            Assert.Equal(before, await File.ReadAllBytesAsync(fixture.CoveragePath));
            Assert.False(File.Exists(fixture.CoveragePath + ".bak"));
        });
    }

    [Fact]
    public async Task Missing_items_property_fails_closed_without_rewrite_backup_or_temp_file()
    {
        await UsingFixtureAsync(async fixture =>
        {
            Directory.CreateDirectory(fixture.RootPath);
            await File.WriteAllTextAsync(
                fixture.CoveragePath,
                """
                {
                  "schemaVersion": 1,
                  "savedAt": "2026-08-11T00:00:00Z"
                }
                """);
            var before = await File.ReadAllBytesAsync(fixture.CoveragePath);

            await AssertErrorAsync(
                PolicyCoverageStorageErrorCode.IntegrityViolation,
                () => fixture.Coverages.GetPolicyCoveragesAsync());

            Assert.Equal(before, await File.ReadAllBytesAsync(fixture.CoveragePath));
            Assert.False(File.Exists(fixture.CoveragePath + ".bak"));
            Assert.Empty(Directory.GetFiles(fixture.RootPath, "*.tmp", SearchOption.TopDirectoryOnly));
        });
    }

    [Fact]
    public async Task Future_updated_timestamp_remains_monotonic_through_all_mutations_and_reloads()
    {
        await UsingFixtureAsync(async fixture =>
        {
            var policy = await fixture.CreateActivePolicyAsync();
            var created = await fixture.Coverages.CreatePolicyCoverageAsync(CreateDraft(policy.Id));
            var futureTimestamp = DateTimeOffset.UtcNow.AddDays(30);
            var root = JsonNode.Parse(await File.ReadAllTextAsync(fixture.CoveragePath))!.AsObject();
            var item = root["items"]![0]!.AsObject();
            item["createdAt"] = futureTimestamp.AddMinutes(-1).ToString("O");
            item["updatedAt"] = futureTimestamp.ToString("O");
            await File.WriteAllTextAsync(fixture.CoveragePath, root.ToJsonString(JsonOptions));

            var futureRecord = await fixture.Coverages.GetPolicyCoverageAsync(created.PolicyCoverageId);
            Assert.NotNull(futureRecord);
            var updated = await fixture.Coverages.UpdatePolicyCoverageAsync(
                futureRecord.PolicyCoverageId,
                futureRecord.Revision,
                CreateUpdateDraft(futureRecord) with { DisplayName = "future update" });
            Assert.True(updated.UpdatedAt >= futureRecord.UpdatedAt);
            AssertRecordEqual(
                updated,
                await fixture.CreateCoverageService().GetPolicyCoverageAsync(updated.PolicyCoverageId));

            var statusChanged = await fixture.Coverages.ChangePolicyCoverageReviewStatusAsync(
                updated.PolicyCoverageId,
                updated.Revision,
                PolicyCoverageValues.ReviewStatusNeedsReview);
            Assert.True(statusChanged.UpdatedAt >= updated.UpdatedAt);
            AssertRecordEqual(
                statusChanged,
                await fixture.CreateCoverageService().GetPolicyCoverageAsync(statusChanged.PolicyCoverageId));

            var disabled = await fixture.Coverages.DisablePolicyCoverageAsync(
                statusChanged.PolicyCoverageId,
                statusChanged.Revision);
            Assert.True(disabled.UpdatedAt >= statusChanged.UpdatedAt);
            Assert.Equal(disabled.UpdatedAt, disabled.DisabledAt);
            AssertRecordEqual(
                disabled,
                await fixture.CreateCoverageService().GetPolicyCoverageAsync(disabled.PolicyCoverageId));

            var restored = await fixture.Coverages.RestorePolicyCoverageAsync(
                disabled.PolicyCoverageId,
                disabled.Revision);
            Assert.True(restored.UpdatedAt >= disabled.UpdatedAt);
            Assert.Null(restored.DisabledAt);
            AssertRecordEqual(
                restored,
                await fixture.CreateCoverageService().GetPolicyCoverageAsync(restored.PolicyCoverageId));
        });
    }

    [Theory]
    [InlineData("unknown_status")]
    [InlineData("unknown_rule")]
    [InlineData("unknown_source")]
    [InlineData("orphan_policy")]
    [InlineData("orphan_document")]
    [InlineData("reversed_dates")]
    public async Task Invalid_stored_record_fails_closed_without_rewrite(string caseName)
    {
        await UsingFixtureAsync(async fixture =>
        {
            var policy = await fixture.CreateActivePolicyAsync();
            _ = await fixture.Coverages.CreatePolicyCoverageAsync(CreateDraft(policy.Id));
            var root = JsonNode.Parse(await File.ReadAllTextAsync(fixture.CoveragePath))!.AsObject();
            var item = root["items"]![0]!.AsObject();
            switch (caseName)
            {
                case "unknown_status":
                    item["reviewStatus"] = "future";
                    break;
                case "unknown_rule":
                    item["visitTypeRule"] = "future";
                    break;
                case "unknown_source":
                    item["sourceKind"] = "future";
                    break;
                case "orphan_policy":
                    item["policyId"] = "policy_missing";
                    break;
                case "orphan_document":
                    item["sourceKind"] = PolicyCoverageValues.SourcePolicyDocument;
                    item["sourcePolicyDocumentId"] = "pdoc_missing";
                    break;
                case "reversed_dates":
                    item["effectiveFrom"] = "2027-01-01";
                    item["effectiveTo"] = "2026-01-01";
                    break;
            }

            await File.WriteAllTextAsync(fixture.CoveragePath, root.ToJsonString(JsonOptions));
            var before = await File.ReadAllBytesAsync(fixture.CoveragePath);
            await AssertErrorAsync(
                PolicyCoverageStorageErrorCode.IntegrityViolation,
                () => fixture.Coverages.GetPolicyCoveragesAsync());
            Assert.Equal(before, await File.ReadAllBytesAsync(fixture.CoveragePath));
        });
    }

    [Fact]
    public async Task Failed_atomic_replace_preserves_original_and_removes_temp_file()
    {
        await UsingFixtureAsync(async fixture =>
        {
            var policy = await fixture.CreateActivePolicyAsync();
            var created = await fixture.Coverages.CreatePolicyCoverageAsync(CreateDraft(policy.Id));
            var firstUpdate = await fixture.Coverages.UpdatePolicyCoverageAsync(
                created.PolicyCoverageId,
                created.Revision,
                CreateUpdateDraft(created) with { DisplayName = "first update" });
            var before = await File.ReadAllBytesAsync(fixture.CoveragePath);
            var backupBefore = await File.ReadAllBytesAsync(fixture.CoveragePath + ".bak");

            await using (File.Open(
                fixture.CoveragePath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read))
            {
                await AssertErrorAsync(
                    PolicyCoverageStorageErrorCode.IntegrityViolation,
                    () => fixture.Coverages.UpdatePolicyCoverageAsync(
                        firstUpdate.PolicyCoverageId,
                        firstUpdate.Revision,
                        CreateUpdateDraft(firstUpdate) with { DisplayName = "blocked update" }));
            }

            Assert.Equal(before, await File.ReadAllBytesAsync(fixture.CoveragePath));
            Assert.Equal(
                backupBefore,
                await File.ReadAllBytesAsync(fixture.CoveragePath + ".bak"));
            Assert.Empty(Directory.GetFiles(fixture.RootPath, "*.tmp", SearchOption.TopDirectoryOnly));
        });
    }

    [Fact]
    public async Task Second_save_preserves_previous_valid_envelope_as_backup()
    {
        await UsingFixtureAsync(async fixture =>
        {
            var policy = await fixture.CreateActivePolicyAsync();
            var created = await fixture.Coverages.CreatePolicyCoverageAsync(CreateDraft(policy.Id));
            var before = await File.ReadAllBytesAsync(fixture.CoveragePath);

            _ = await fixture.Coverages.UpdatePolicyCoverageAsync(
                created.PolicyCoverageId,
                created.Revision,
                CreateUpdateDraft(created) with { DisplayName = "updated" });

            Assert.Equal(before, await File.ReadAllBytesAsync(fixture.CoveragePath + ".bak"));
            Assert.NotEqual(before, await File.ReadAllBytesAsync(fixture.CoveragePath));
        });
    }

    [Fact]
    public async Task Pre_cancelled_create_has_zero_durable_write()
    {
        await UsingFixtureAsync(async fixture =>
        {
            var policy = await fixture.CreateActivePolicyAsync();
            using var cancellation = new CancellationTokenSource();
            cancellation.Cancel();

            await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
                fixture.Coverages.CreatePolicyCoverageAsync(
                    CreateDraft(policy.Id),
                    cancellation.Token));

            Assert.False(File.Exists(fixture.CoveragePath));
            Assert.Empty(Directory.GetFiles(fixture.RootPath, "*.tmp", SearchOption.TopDirectoryOnly));
        });
    }

    [Fact]
    public async Task Coverage_mutations_do_not_change_policy_or_claim_submission_files()
    {
        await UsingFixtureAsync(async fixture =>
        {
            var policy = await fixture.CreateActivePolicyAsync();
            var policiesBefore = await File.ReadAllBytesAsync(
                Path.Combine(fixture.RootPath, "policies.json"));
            var submissionPath = Path.Combine(fixture.RootPath, "claim-submissions.json");
            await File.WriteAllTextAsync(submissionPath, "synthetic sentinel");
            var submissionsBefore = await File.ReadAllBytesAsync(submissionPath);

            var created = await fixture.Coverages.CreatePolicyCoverageAsync(CreateDraft(policy.Id));
            _ = await fixture.Coverages.DisablePolicyCoverageAsync(
                created.PolicyCoverageId,
                created.Revision);

            Assert.Equal(
                policiesBefore,
                await File.ReadAllBytesAsync(Path.Combine(fixture.RootPath, "policies.json")));
            Assert.Equal(submissionsBefore, await File.ReadAllBytesAsync(submissionPath));
        });
    }

    [Fact]
    public async Task Structured_errors_do_not_expose_payload_or_path_values()
    {
        await UsingFixtureAsync(async fixture =>
        {
            var policy = await fixture.CreateActivePolicyAsync();
            const string displayName = "private-display-name";
            const string sourceLocator = "C:\\private\\coverage.json";

            var exception = await Assert.ThrowsAsync<PolicyCoverageStorageException>(() =>
                fixture.Coverages.CreatePolicyCoverageAsync(CreateDraft(policy.Id) with
                {
                    DisplayName = displayName,
                    SourceKind = PolicyCoverageValues.SourcePolicyDocument,
                    SourcePolicyDocumentId = "private-document-id",
                    SourceLocator = sourceLocator,
                    Memo = "private memo"
                }));

            Assert.Equal(PolicyCoverageStorageErrorCode.ReferenceInvalid, exception.ErrorCode);
            Assert.DoesNotContain(displayName, exception.ToString(), StringComparison.Ordinal);
            Assert.DoesNotContain(sourceLocator, exception.ToString(), StringComparison.Ordinal);
            Assert.DoesNotContain("private-document-id", exception.ToString(), StringComparison.Ordinal);
            Assert.DoesNotContain("private memo", exception.ToString(), StringComparison.Ordinal);
            Assert.DoesNotContain(fixture.RootPath, exception.ToString(), StringComparison.OrdinalIgnoreCase);
        });
    }

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    private static PolicyCoverageCreateDraft CreateDraft(string policyId)
    {
        return new PolicyCoverageCreateDraft(
            PolicyId: policyId,
            DisplayName: "coverage A",
            ReviewStatus: PolicyCoverageValues.ReviewStatusCandidate,
            EffectiveFrom: new DateOnly(2026, 1, 1),
            EffectiveTo: new DateOnly(2027, 12, 31),
            VisitTypeRule: PolicyCoverageValues.VisitTypeAny,
            SurgeryRule: PolicyCoverageValues.ConditionRequired,
            PrescriptionRule: PolicyCoverageValues.ConditionExcluded,
            DiagnosisRuleMode: PolicyCoverageValues.DiagnosisRulePrefixList,
            DiagnosisCodePrefixes: ["A00.1"],
            SourceKind: PolicyCoverageValues.SourceManual,
            SourcePolicyDocumentId: null,
            SourceLocator: null,
            Memo: null);
    }

    private static PolicyCoverageUpdateDraft CreateUpdateDraft(PolicyCoverageRecord record)
    {
        return new PolicyCoverageUpdateDraft(
            DisplayName: record.DisplayName,
            EffectiveFrom: record.EffectiveFrom,
            EffectiveTo: record.EffectiveTo,
            VisitTypeRule: record.VisitTypeRule,
            SurgeryRule: record.SurgeryRule,
            PrescriptionRule: record.PrescriptionRule,
            DiagnosisRuleMode: record.DiagnosisRuleMode,
            DiagnosisCodePrefixes: record.DiagnosisCodePrefixes,
            SourceKind: record.SourceKind,
            SourcePolicyDocumentId: record.SourcePolicyDocumentId,
            SourceLocator: record.SourceLocator,
            Memo: record.Memo);
    }

    private static async Task<PolicyCoverageRecord> CreateInStatusAsync(
        Fixture fixture,
        string policyId,
        string status)
    {
        if (status == PolicyCoverageValues.ReviewStatusNeedsReview)
        {
            return await fixture.Coverages.CreatePolicyCoverageAsync(
                CreateDraft(policyId) with { ReviewStatus = status });
        }

        var created = await fixture.Coverages.CreatePolicyCoverageAsync(CreateDraft(policyId));
        return status == PolicyCoverageValues.ReviewStatusCandidate
            ? created
            : await fixture.Coverages.ChangePolicyCoverageReviewStatusAsync(
                created.PolicyCoverageId,
                created.Revision,
                status);
    }

    private static async Task AssertErrorAsync(
        PolicyCoverageStorageErrorCode expectedCode,
        Func<Task> action)
    {
        var exception = await Assert.ThrowsAsync<PolicyCoverageStorageException>(action);
        Assert.Equal(expectedCode, exception.ErrorCode);
    }

    private static void AssertRecordEqual(
        PolicyCoverageRecord expected,
        PolicyCoverageRecord? actual)
    {
        Assert.NotNull(actual);
        Assert.Equal(expected.DiagnosisCodePrefixes, actual.DiagnosisCodePrefixes);
        Assert.Equal(
            expected with { DiagnosisCodePrefixes = actual.DiagnosisCodePrefixes },
            actual);
    }

    private static async Task<MutationAttempt> CaptureAsync(
        Func<Task<PolicyCoverageRecord>> action)
    {
        try
        {
            return new MutationAttempt(await action(), null);
        }
        catch (PolicyCoverageStorageException exception)
        {
            return new MutationAttempt(null, exception);
        }
    }

    private static async Task UsingFixtureAsync(Func<Fixture, Task> action)
    {
        var fixture = new Fixture();
        try
        {
            await action(fixture);
        }
        finally
        {
            fixture.Dispose();
        }
    }

    private sealed record MutationAttempt(
        PolicyCoverageRecord? Record,
        PolicyCoverageStorageException? Exception);

    private sealed class Fixture : IDisposable
    {
        private int documentSequence;

        public Fixture()
        {
            RootPath = Path.Combine(
                Path.GetTempPath(),
                "FamilyClaimRef.App.Tests",
                "policy-coverage",
                Guid.NewGuid().ToString("N"));
            Families = new JsonFamilyMemberStorageService(RootPath);
            Policies = new JsonPolicyClaimStorageService(RootPath, Families);
            Documents = new JsonDocumentStorageService(RootPath);
            Coverages = CreateCoverageService();
        }

        public string RootPath { get; }

        public string CoveragePath => Path.Combine(
            RootPath,
            JsonPolicyCoverageStorageService.StoreFileName);

        public JsonFamilyMemberStorageService Families { get; }

        public JsonPolicyClaimStorageService Policies { get; }

        public JsonDocumentStorageService Documents { get; }

        public JsonPolicyCoverageStorageService Coverages { get; }

        public JsonPolicyCoverageStorageService CreateCoverageService()
        {
            return new JsonPolicyCoverageStorageService(
                RootPath,
                (IClaimHistoryStorageReader)Policies,
                Documents);
        }

        public async Task<PolicyRecord> CreateActivePolicyAsync(
            string displayTitle = "policy A")
        {
            var family = await Families.CreateFamilyMemberAsync(new FamilyMemberDraft(
                $"family {Guid.NewGuid():N}",
                FamilyMemberRelationValues.Self,
                null));
            return await Policies.CreateInsurancePolicyAsync(new InsurancePolicyDraft(
                DisplayTitle: displayTitle,
                FamilyMemberId: family.Id,
                InsurerName: "insurer A",
                ContractStatus: InsurancePolicyValues.ContractStatusActive,
                EnrollmentDate: new DateOnly(2026, 1, 1),
                CoveragePeriod: "2026-2027",
                PremiumPaymentPeriod: "20 years",
                TotalPlannedPremiumAmount: 12_000_000m,
                RenewalType: InsurancePolicyValues.RenewalTypeFixed,
                RefundType: InsurancePolicyValues.RefundTypeRefundable,
                InsuranceBusinessType: InsurancePolicyValues.BusinessTypeLife,
                ProductCategory: InsurancePolicyValues.ProductCategoryCancer));
        }

        public async Task<PolicyDocumentRecord> CreatePolicyDocumentAsync(string policyId)
        {
            var sequence = Interlocked.Increment(ref documentSequence);
            var document = await Documents.AddDocumentAsync(new DocumentDraft(
                PhysicalFileName: $"document-{sequence}.pdf",
                DisplayTitle: $"document {sequence}",
                Extension: "pdf",
                RelativePath: $"managed/document-{sequence}.pdf",
                OriginalDisplayFileName: $"source-{sequence}.pdf",
                ValidatedFileType: "pdf",
                ByteLength: 10,
                Sha256: new string((char)('a' + sequence), 64),
                ReferenceDate: new DateOnly(2026, 1, 1),
                DocumentType: "terms"));
            return await Documents.AddPolicyDocumentAsync(new PolicyDocumentDraft(
                policyId,
                document.Id,
                "terms"));
        }

        public void Dispose()
        {
            if (Directory.Exists(RootPath))
            {
                Directory.Delete(RootPath, recursive: true);
            }
        }
    }
}
