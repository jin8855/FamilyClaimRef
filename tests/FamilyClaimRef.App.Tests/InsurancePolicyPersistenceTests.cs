using System.Text.Json;
using FamilyClaimRef.App.Models.Storage;
using FamilyClaimRef.App.Services.Storage;
using Xunit;

namespace FamilyClaimRef.App.Tests;

public sealed class InsurancePolicyPersistenceTests
{
    private static readonly DateOnly EnrollmentDate = new(2026, 8, 4);

    [Fact]
    public async Task Create_round_trips_twelve_user_fields_and_system_source_as_camel_case_json()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var familyStorage = new JsonFamilyMemberStorageService(rootPath);
            var family = await CreateFamilyAsync(familyStorage, "synthetic family");
            var service = new JsonPolicyClaimStorageService(rootPath, familyStorage);

            var created = await service.CreateInsurancePolicyAsync(CreateDraft(
                family.Id,
                displayTitle: "  synthetic policy  "));

            Assert.Equal("synthetic policy", created.DisplayTitle);
            Assert.Equal(family.Id, created.FamilyMemberId);
            Assert.Equal("synthetic insurer", created.InsurerName);
            Assert.Equal(InsurancePolicyValues.ContractStatusActive, created.ContractStatus);
            Assert.Equal(EnrollmentDate, created.EnrollmentDate);
            Assert.Null(created.ReferenceDate);
            Assert.Equal("2026-2027", created.CoveragePeriod);
            Assert.Equal("20년납", created.PremiumPaymentPeriod);
            Assert.Equal(12_000_000m, created.TotalPlannedPremiumAmount);
            Assert.Equal(InsurancePolicyValues.RenewalTypeFixed, created.RenewalType);
            Assert.Equal(InsurancePolicyValues.RefundTypeRefundable, created.RefundType);
            Assert.Equal(InsurancePolicyValues.BusinessTypeLife, created.InsuranceBusinessType);
            Assert.Equal(InsurancePolicyValues.ProductCategoryCancer, created.ProductCategory);
            Assert.Equal(InsurancePolicyValues.RegistrationSourceDirectInput, created.RegistrationSource);

            var reloaded = await new JsonPolicyClaimStorageService(rootPath).GetPolicyAsync(created.Id);
            Assert.Equal(created, reloaded);

            using var json = JsonDocument.Parse(await File.ReadAllTextAsync(PolicyPath(rootPath)));
            var item = Assert.Single(json.RootElement.GetProperty("items").EnumerateArray());
            Assert.Equal("synthetic policy", item.GetProperty("displayTitle").GetString());
            Assert.Equal(family.Id, item.GetProperty("familyMemberId").GetString());
            Assert.Equal("synthetic insurer", item.GetProperty("insurerName").GetString());
            Assert.Equal(InsurancePolicyValues.ContractStatusActive, item.GetProperty("contractStatus").GetString());
            Assert.Equal(EnrollmentDate.ToString("yyyy-MM-dd"), item.GetProperty("enrollmentDate").GetString());
            Assert.Equal("2026-2027", item.GetProperty("coveragePeriod").GetString());
            Assert.Equal("20년납", item.GetProperty("premiumPaymentPeriod").GetString());
            Assert.Equal(12_000_000m, item.GetProperty("totalPlannedPremiumAmount").GetDecimal());
            Assert.Equal(InsurancePolicyValues.RenewalTypeFixed, item.GetProperty("renewalType").GetString());
            Assert.Equal(InsurancePolicyValues.RefundTypeRefundable, item.GetProperty("refundType").GetString());
            Assert.Equal(InsurancePolicyValues.BusinessTypeLife, item.GetProperty("insuranceBusinessType").GetString());
            Assert.Equal(InsurancePolicyValues.ProductCategoryCancer, item.GetProperty("productCategory").GetString());
            Assert.Equal(InsurancePolicyValues.RegistrationSourceDirectInput, item.GetProperty("registrationSource").GetString());
        });
    }

    [Fact]
    public async Task Update_preserves_policy_and_family_identity_and_round_trips_changes()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var familyStorage = new JsonFamilyMemberStorageService(rootPath);
            var family = await CreateFamilyAsync(familyStorage, "synthetic family");
            var service = new JsonPolicyClaimStorageService(rootPath, familyStorage);
            var created = await service.CreateInsurancePolicyAsync(CreateDraft(family.Id));
            var updatedDate = new DateOnly(2026, 9, 1);

            var updated = await service.UpdateInsurancePolicyAsync(
                created.Id,
                CreateDraft(family.Id) with
                {
                    DisplayTitle = "updated policy",
                    InsurerName = "updated insurer",
                    ContractStatus = InsurancePolicyValues.ContractStatusPremiumWaived,
                    EnrollmentDate = updatedDate,
                    CoveragePeriod = "2027-2028",
                    PremiumPaymentPeriod = "80세납",
                    TotalPlannedPremiumAmount = null,
                    RenewalType = InsurancePolicyValues.RenewalTypePartiallyRenewable,
                    RefundType = InsurancePolicyValues.RefundTypeNoSurrenderValue,
                    InsuranceBusinessType = InsurancePolicyValues.BusinessTypeNonLife,
                    ProductCategory = InsurancePolicyValues.ProductCategoryDriver
                });

            Assert.Equal(created.Id, updated.Id);
            Assert.Equal(family.Id, updated.FamilyMemberId);
            Assert.Equal("updated policy", updated.DisplayTitle);
            Assert.Equal("updated insurer", updated.InsurerName);
            Assert.Equal(InsurancePolicyValues.ContractStatusPremiumWaived, updated.ContractStatus);
            Assert.Equal(updatedDate, updated.EnrollmentDate);
            Assert.Null(updated.ReferenceDate);
            Assert.Equal("2027-2028", updated.CoveragePeriod);
            Assert.Equal("80세납", updated.PremiumPaymentPeriod);
            Assert.Null(updated.TotalPlannedPremiumAmount);
            Assert.Equal(InsurancePolicyValues.RenewalTypePartiallyRenewable, updated.RenewalType);
            Assert.Equal(InsurancePolicyValues.RefundTypeNoSurrenderValue, updated.RefundType);
            Assert.Equal(InsurancePolicyValues.BusinessTypeNonLife, updated.InsuranceBusinessType);
            Assert.Equal(InsurancePolicyValues.ProductCategoryDriver, updated.ProductCategory);
            Assert.Equal(InsurancePolicyValues.RegistrationSourceDirectInput, updated.RegistrationSource);
            Assert.Equal(updated, await new JsonPolicyClaimStorageService(rootPath).GetPolicyAsync(created.Id));
        });
    }

    [Fact]
    public async Task Restart_and_multiple_records_preserve_every_policy_without_title_uniqueness()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var familyStorage = new JsonFamilyMemberStorageService(rootPath);
            var family = await CreateFamilyAsync(familyStorage, "synthetic family");
            var service = new JsonPolicyClaimStorageService(rootPath, familyStorage);

            var first = await service.CreateInsurancePolicyAsync(CreateDraft(family.Id, "same title"));
            var second = await service.CreateInsurancePolicyAsync(CreateDraft(family.Id, "same title"));
            var reloaded = await new JsonPolicyClaimStorageService(rootPath).GetPoliciesAsync();

            Assert.Equal(2, reloaded.Count);
            Assert.Contains(reloaded, policy => policy.Id == first.Id);
            Assert.Contains(reloaded, policy => policy.Id == second.Id);
            Assert.All(reloaded, policy => Assert.Equal("same title", policy.DisplayTitle));
        });
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(6)]
    [InlineData(7)]
    [InlineData(8)]
    [InlineData(9)]
    [InlineData(10)]
    public async Task Create_rejects_each_missing_required_field_without_writing(int fieldIndex)
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var familyStorage = new JsonFamilyMemberStorageService(rootPath);
            var family = await CreateFamilyAsync(familyStorage, "synthetic family");
            var service = new JsonPolicyClaimStorageService(rootPath, familyStorage);
            var draft = CreateDraftWithMissingField(family.Id, fieldIndex);

            await Assert.ThrowsAsync<ArgumentException>(() =>
                service.CreateInsurancePolicyAsync(draft));

            Assert.False(File.Exists(PolicyPath(rootPath)));
        });
    }

    [Theory]
    [InlineData(nameof(InsurancePolicyDraft.ContractStatus))]
    [InlineData(nameof(InsurancePolicyDraft.RenewalType))]
    [InlineData(nameof(InsurancePolicyDraft.RefundType))]
    [InlineData(nameof(InsurancePolicyDraft.InsuranceBusinessType))]
    [InlineData(nameof(InsurancePolicyDraft.ProductCategory))]
    public async Task Create_rejects_arbitrary_selection_values_at_storage_boundary(string field)
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var familyStorage = new JsonFamilyMemberStorageService(rootPath);
            var family = await CreateFamilyAsync(familyStorage, "synthetic family");
            var service = new JsonPolicyClaimStorageService(rootPath, familyStorage);
            var draft = CreateDraft(family.Id);
            draft = field switch
            {
                nameof(InsurancePolicyDraft.ContractStatus) => draft with { ContractStatus = "arbitrary" },
                nameof(InsurancePolicyDraft.RenewalType) => draft with { RenewalType = "arbitrary" },
                nameof(InsurancePolicyDraft.RefundType) => draft with { RefundType = "arbitrary" },
                nameof(InsurancePolicyDraft.InsuranceBusinessType) => draft with { InsuranceBusinessType = "arbitrary" },
                _ => draft with { ProductCategory = "arbitrary" }
            };

            await Assert.ThrowsAsync<ArgumentException>(() => service.CreateInsurancePolicyAsync(draft));
            Assert.False(File.Exists(PolicyPath(rootPath)));
        });
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(1.5)]
    public async Task Create_rejects_negative_or_fractional_planned_premium(decimal amount)
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var familyStorage = new JsonFamilyMemberStorageService(rootPath);
            var family = await CreateFamilyAsync(familyStorage, "synthetic family");
            var service = new JsonPolicyClaimStorageService(rootPath, familyStorage);

            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() =>
                service.CreateInsurancePolicyAsync(CreateDraft(family.Id) with
                {
                    TotalPlannedPremiumAmount = amount
                }));
            Assert.False(File.Exists(PolicyPath(rootPath)));
        });
    }

    [Fact]
    public async Task Create_rejects_missing_family_reference_without_writing()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var familyStorage = new JsonFamilyMemberStorageService(rootPath);
            var service = new JsonPolicyClaimStorageService(rootPath, familyStorage);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                service.CreateInsurancePolicyAsync(CreateDraft("family_missing")));

            Assert.False(File.Exists(PolicyPath(rootPath)));
        });
    }

    [Fact]
    public async Task Create_rejects_inactive_family_reference_without_writing()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var familyStorage = new JsonFamilyMemberStorageService(rootPath);
            var family = await CreateFamilyAsync(familyStorage, "synthetic family");
            await familyStorage.DeactivateFamilyMemberAsync(family.Id, family.Version);
            var service = new JsonPolicyClaimStorageService(rootPath, familyStorage);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                service.CreateInsurancePolicyAsync(CreateDraft(family.Id)));

            Assert.False(File.Exists(PolicyPath(rootPath)));
        });
    }

    [Fact]
    public async Task Linked_family_deactivate_update_same_reference_and_reactivate_preserve_identity()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var familyStorage = new JsonFamilyMemberStorageService(rootPath);
            var family = await CreateFamilyAsync(familyStorage, "synthetic family");
            var service = new JsonPolicyClaimStorageService(rootPath, familyStorage);
            var policy = await service.CreateInsurancePolicyAsync(CreateDraft(family.Id));

            var inactive = await familyStorage.DeactivateFamilyMemberAsync(family.Id, family.Version);
            var preserved = Assert.Single(await service.GetPoliciesAsync());
            Assert.Equal(family.Id, preserved.FamilyMemberId);

            var updated = await service.UpdateInsurancePolicyAsync(
                policy.Id,
                CreateDraft(family.Id, "updated while inactive"));
            Assert.Equal(family.Id, updated.FamilyMemberId);

            var active = await familyStorage.ReactivateFamilyMemberAsync(inactive.Id, inactive.Version);
            Assert.Equal(family.Id, active.Id);
            Assert.Equal(family.Id, (await service.GetPolicyAsync(policy.Id))!.FamilyMemberId);
        });
    }

    [Fact]
    public async Task Update_rejects_switch_to_inactive_family_and_preserves_existing_json()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var familyStorage = new JsonFamilyMemberStorageService(rootPath);
            var activeFamily = await CreateFamilyAsync(familyStorage, "active family");
            var inactiveFamily = await CreateFamilyAsync(familyStorage, "inactive family");
            await familyStorage.DeactivateFamilyMemberAsync(inactiveFamily.Id, inactiveFamily.Version);
            var service = new JsonPolicyClaimStorageService(rootPath, familyStorage);
            var policy = await service.CreateInsurancePolicyAsync(CreateDraft(activeFamily.Id));
            var before = await File.ReadAllBytesAsync(PolicyPath(rootPath));

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                service.UpdateInsurancePolicyAsync(
                    policy.Id,
                    CreateDraft(inactiveFamily.Id, "invalid update")));

            Assert.Equal(before, await File.ReadAllBytesAsync(PolicyPath(rootPath)));
            Assert.Equal(activeFamily.Id, (await service.GetPolicyAsync(policy.Id))!.FamilyMemberId);
        });
    }

    [Fact]
    public async Task Orphan_reference_loads_without_rewrite_and_requires_explicit_valid_relink()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var policyId = "policy_orphan";
            await WritePolicyEnvelopeAsync(rootPath, $$"""
                {
                  "id": "{{policyId}}",
                  "displayTitle": "orphan policy",
                  "referenceDate": "2026-08-04",
                  "createdAt": "2026-08-04T00:00:00Z",
                  "updatedAt": "2026-08-04T00:00:00Z",
                  "disabledAt": null,
                  "familyMemberId": "family_missing",
                  "insurerName": "synthetic insurer",
                  "contractStatus": "active",
                  "enrollmentDate": "2026-08-04",
                  "coveragePeriod": "2026-2027",
                  "registrationSource": "manual"
                }
                """);
            var before = await File.ReadAllBytesAsync(PolicyPath(rootPath));
            var familyStorage = new JsonFamilyMemberStorageService(rootPath);
            var service = new JsonPolicyClaimStorageService(rootPath, familyStorage);

            var loaded = await service.GetPolicyAsync(policyId);

            Assert.NotNull(loaded);
            Assert.Equal("family_missing", loaded.FamilyMemberId);
            Assert.Equal(before, await File.ReadAllBytesAsync(PolicyPath(rootPath)));
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                service.UpdateInsurancePolicyAsync(policyId, CreateDraft("family_missing")));

            var replacement = await CreateFamilyAsync(familyStorage, "replacement family");
            var relinked = await service.UpdateInsurancePolicyAsync(
                policyId,
                CreateDraft(replacement.Id, "relinked policy"));
            Assert.Equal(replacement.Id, relinked.FamilyMemberId);
        });
    }

    [Fact]
    public async Task Legacy_policy_loads_with_nullable_new_fields_without_read_time_rewrite()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            await WritePolicyEnvelopeAsync(rootPath, """
                {
                  "id": "policy_legacy",
                  "displayTitle": "legacy policy",
                  "referenceDate": "2026-07-01",
                  "createdAt": "2026-07-01T00:00:00Z",
                  "updatedAt": "2026-07-01T00:00:00Z",
                  "disabledAt": null
                }
                """);
            var before = await File.ReadAllBytesAsync(PolicyPath(rootPath));

            var policy = await new JsonPolicyClaimStorageService(rootPath).GetPolicyAsync("policy_legacy");

            Assert.NotNull(policy);
            Assert.Equal(new DateOnly(2026, 7, 1), policy.ReferenceDate);
            Assert.Null(policy.FamilyMemberId);
            Assert.Null(policy.InsurerName);
            Assert.Null(policy.ContractStatus);
            Assert.Null(policy.EnrollmentDate);
            Assert.Null(policy.CoveragePeriod);
            Assert.Null(policy.RegistrationSource);
            Assert.Null(policy.PremiumPaymentPeriod);
            Assert.Null(policy.TotalPlannedPremiumAmount);
            Assert.Null(policy.RenewalType);
            Assert.Null(policy.RefundType);
            Assert.Null(policy.InsuranceBusinessType);
            Assert.Null(policy.ProductCategory);
            Assert.Equal(before, await File.ReadAllBytesAsync(PolicyPath(rootPath)));
        });
    }

    [Fact]
    public async Task Previous_seven_field_policy_loads_losslessly_without_defaults_or_rewrite()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            await WritePolicyEnvelopeAsync(rootPath, """
                {
                  "id": "policy_previous_seven",
                  "displayTitle": "previous policy",
                  "referenceDate": "2026-07-01",
                  "createdAt": "2026-07-01T00:00:00Z",
                  "updatedAt": "2026-07-01T00:00:00Z",
                  "disabledAt": null,
                  "familyMemberId": "family_previous",
                  "insurerName": "previous insurer",
                  "contractStatus": "사용 중",
                  "enrollmentDate": "2026-06-15",
                  "coveragePeriod": "2026-2036",
                  "registrationSource": "직접 입력"
                }
                """);
            var before = await File.ReadAllBytesAsync(PolicyPath(rootPath));

            var policy = await new JsonPolicyClaimStorageService(rootPath)
                .GetPolicyAsync("policy_previous_seven");

            Assert.NotNull(policy);
            Assert.Equal("family_previous", policy.FamilyMemberId);
            Assert.Equal("2026-2036", policy.CoveragePeriod);
            Assert.Equal(InsurancePolicyValues.LegacyContractStatusActive, policy.ContractStatus);
            Assert.Equal(InsurancePolicyValues.RegistrationSourceDirectInput, policy.RegistrationSource);
            Assert.Null(policy.PremiumPaymentPeriod);
            Assert.Null(policy.TotalPlannedPremiumAmount);
            Assert.Null(policy.RenewalType);
            Assert.Null(policy.RefundType);
            Assert.Null(policy.InsuranceBusinessType);
            Assert.Null(policy.ProductCategory);
            Assert.Equal(before, await File.ReadAllBytesAsync(PolicyPath(rootPath)));
        });
    }

    [Fact]
    public async Task Failed_atomic_replace_preserves_previous_policy_file()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var familyStorage = new JsonFamilyMemberStorageService(rootPath);
            var family = await CreateFamilyAsync(familyStorage, "synthetic family");
            var service = new JsonPolicyClaimStorageService(rootPath, familyStorage);
            var policy = await service.CreateInsurancePolicyAsync(CreateDraft(family.Id));
            var before = await File.ReadAllBytesAsync(PolicyPath(rootPath));

            Exception? exception;
            await using (var lockStream = new FileStream(
                PolicyPath(rootPath),
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read))
            {
                exception = await Record.ExceptionAsync(() =>
                    service.UpdateInsurancePolicyAsync(
                        policy.Id,
                        CreateDraft(family.Id, "should not replace")));
            }

            Assert.NotNull(exception);
            Assert.Equal(before, await File.ReadAllBytesAsync(PolicyPath(rootPath)));
            Assert.Empty(Directory.GetFiles(rootPath, "policies.json.*.tmp"));
        });
    }

    private static InsurancePolicyDraft CreateDraft(
        string familyMemberId,
        string displayTitle = "synthetic policy")
    {
        return new InsurancePolicyDraft(
            displayTitle,
            familyMemberId,
            "synthetic insurer",
            InsurancePolicyValues.ContractStatusActive,
            EnrollmentDate,
            "2026-2027",
            "20년납",
            12_000_000m,
            InsurancePolicyValues.RenewalTypeFixed,
            InsurancePolicyValues.RefundTypeRefundable,
            InsurancePolicyValues.BusinessTypeLife,
            InsurancePolicyValues.ProductCategoryCancer);
    }

    private static InsurancePolicyDraft CreateDraftWithMissingField(
        string familyMemberId,
        int fieldIndex)
    {
        return new InsurancePolicyDraft(
            fieldIndex == 0 ? " " : "synthetic policy",
            fieldIndex == 1 ? " " : familyMemberId,
            fieldIndex == 2 ? " " : "synthetic insurer",
            fieldIndex == 3 ? " " : InsurancePolicyValues.ContractStatusActive,
            fieldIndex == 4 ? default : EnrollmentDate,
            fieldIndex == 5 ? " " : "2026-2027",
            fieldIndex == 6 ? " " : "20년납",
            12_000_000m,
            fieldIndex == 7 ? " " : InsurancePolicyValues.RenewalTypeFixed,
            fieldIndex == 8 ? " " : InsurancePolicyValues.RefundTypeRefundable,
            fieldIndex == 9 ? " " : InsurancePolicyValues.BusinessTypeLife,
            fieldIndex == 10 ? " " : InsurancePolicyValues.ProductCategoryCancer);
    }

    private static Task<FamilyMemberRecord> CreateFamilyAsync(
        JsonFamilyMemberStorageService storage,
        string displayName)
    {
        return storage.CreateFamilyMemberAsync(new FamilyMemberDraft(
            displayName,
            FamilyMemberRelationValues.Self,
            null));
    }

    private static async Task WritePolicyEnvelopeAsync(string rootPath, string itemJson)
    {
        Directory.CreateDirectory(rootPath);
        await File.WriteAllTextAsync(
            PolicyPath(rootPath),
            $$"""
            {
              "schemaVersion": 1,
              "savedAt": "2026-08-04T00:00:00Z",
              "items": [
                {{itemJson}}
              ]
            }
            """);
    }

    private static string PolicyPath(string rootPath) => Path.Combine(rootPath, "policies.json");

    private static async Task UsingTempRootAsync(Func<string, Task> action)
    {
        var rootPath = Path.Combine(
            Path.GetTempPath(),
            "FamilyClaimRef.App.Tests",
            nameof(InsurancePolicyPersistenceTests),
            Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(rootPath);

        try
        {
            await action(rootPath);
        }
        finally
        {
            if (Directory.Exists(rootPath))
            {
                Directory.Delete(rootPath, recursive: true);
            }
        }
    }
}
