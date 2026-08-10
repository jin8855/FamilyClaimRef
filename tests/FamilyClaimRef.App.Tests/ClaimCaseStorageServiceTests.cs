using System.Text;
using FamilyClaimRef.App.Models.Storage;
using FamilyClaimRef.App.Services.Storage;
using Xunit;

namespace FamilyClaimRef.App.Tests;

public sealed class ClaimCaseStorageServiceTests
{
    [Fact]
    public async Task Legacy_v1_load_projects_family_without_rewriting_file()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var familyStore = new JsonFamilyMemberStorageService(rootPath);
            var service = new JsonPolicyClaimStorageService(rootPath, familyStore);
            var family = await CreateFamilyAsync(familyStore);
            var policy = await CreatePolicyAsync(service, family.Id);
            var claimsPath = await WriteLegacyClaimAsync(rootPath, policy.Id);
            var bytesBefore = await File.ReadAllBytesAsync(claimsPath);

            var claim = Assert.Single(await service.GetClaimCasesAsync());

            Assert.Equal("claim_legacy", claim.Id);
            Assert.Equal(policy.Id, claim.PolicyId);
            Assert.Equal(family.Id, claim.FamilyMemberId);
            Assert.Equal(ClaimCaseValues.StatusSaved, claim.CaseStatus);
            Assert.Equal(0, claim.Revision);
            Assert.Equal(bytesBefore, await File.ReadAllBytesAsync(claimsPath));
        });
    }
    [Fact]
    public async Task History_read_preserves_raw_legacy_owner_and_does_not_rewrite_file()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var familyStore = new JsonFamilyMemberStorageService(rootPath);
            var service = new JsonPolicyClaimStorageService(rootPath, familyStore);
            var family = await CreateFamilyAsync(familyStore);
            var policy = await CreatePolicyAsync(service, family.Id);
            var claimsPath = await WriteLegacyClaimAsync(rootPath, policy.Id);
            var bytesBefore = await File.ReadAllBytesAsync(claimsPath);
            var historyReader = (IClaimHistoryStorageReader)service;

            var claim = Assert.Single(await historyReader.GetAllClaimCasesForHistoryAsync());

            Assert.Equal("claim_legacy", claim.Id);
            Assert.Equal(policy.Id, claim.PolicyId);
            Assert.Null(claim.FamilyMemberId);
            Assert.Equal(bytesBefore, await File.ReadAllBytesAsync(claimsPath));
        });
    }

    [Fact]
    public async Task Unresolved_legacy_owner_blocks_update_without_write()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var familyStore = new JsonFamilyMemberStorageService(rootPath);
            var service = new JsonPolicyClaimStorageService(rootPath, familyStore);
            var family = await CreateFamilyAsync(familyStore);
            var claimsPath = await WriteLegacyClaimAsync(rootPath, "policy_missing");
            var bytesBefore = await File.ReadAllBytesAsync(claimsPath);

            await Assert.ThrowsAsync<ClaimCaseLegacyReviewRequiredException>(() =>
                service.UpdateClaimCaseAsync(
                    "claim_legacy",
                    0,
                    CreateDraft(family.Id),
                    CancellationToken.None));

            Assert.Equal(bytesBefore, await File.ReadAllBytesAsync(claimsPath));
            Assert.Empty(FindClaimTempFiles(rootPath));
        });
    }

    [Fact]
    public async Task Unresolved_legacy_owner_blocks_both_disable_paths_without_write_or_residue()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var familyStore = new JsonFamilyMemberStorageService(rootPath);
            var service = new JsonPolicyClaimStorageService(rootPath, familyStore);
            var legacyService = (IPolicyClaimStorageService)service;
            var claimsPath = await WriteLegacyClaimAsync(rootPath, "policy_missing");
            var backupPath = $"{claimsPath}.bak";
            var backupBytes = Encoding.UTF8.GetBytes("synthetic existing backup");
            await File.WriteAllBytesAsync(backupPath, backupBytes);
            var bytesBefore = await File.ReadAllBytesAsync(claimsPath);

            await Assert.ThrowsAsync<ClaimCaseLegacyReviewRequiredException>(() =>
                service.DisableClaimCaseAsync("claim_legacy", 0));

            Assert.Equal(bytesBefore, await File.ReadAllBytesAsync(claimsPath));
            Assert.Equal(backupBytes, await File.ReadAllBytesAsync(backupPath));
            Assert.Empty(FindClaimTempFiles(rootPath));

            await Assert.ThrowsAsync<ClaimCaseLegacyReviewRequiredException>(() =>
                legacyService.DisableClaimAsync("claim_legacy", 0));

            Assert.Equal(bytesBefore, await File.ReadAllBytesAsync(claimsPath));
            Assert.Equal(backupBytes, await File.ReadAllBytesAsync(backupPath));
            Assert.Empty(FindClaimTempFiles(rootPath));
        });
    }

    [Fact]
    public async Task Resolved_legacy_owner_can_be_disabled_without_overblocking()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var familyStore = new JsonFamilyMemberStorageService(rootPath);
            var service = new JsonPolicyClaimStorageService(rootPath, familyStore);
            var family = await CreateFamilyAsync(familyStore);
            var policy = await CreatePolicyAsync(service, family.Id);
            var claimsPath = await WriteLegacyClaimAsync(rootPath, policy.Id);
            var bytesBefore = await File.ReadAllBytesAsync(claimsPath);

            var disabled = await service.DisableClaimCaseAsync("claim_legacy", 0);

            Assert.NotNull(disabled.DisabledAt);
            Assert.Equal(1, disabled.Revision);
            Assert.Equal(bytesBefore, await File.ReadAllBytesAsync($"{claimsPath}.bak"));
            Assert.Empty(FindClaimTempFiles(rootPath));
        });
    }

    [Fact]
    public async Task Family_owned_create_and_reload_persists_normalized_fields_without_policy()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var familyStore = new JsonFamilyMemberStorageService(rootPath);
            var service = new JsonPolicyClaimStorageService(rootPath, familyStore);
            var family = await CreateFamilyAsync(familyStore);
            var draft = CreateDraft(family.Id) with
            {
                DisplayTitle = "  synthetic claim  ",
                HospitalName = "  synthetic hospital  ",
                DiagnosisCode = "  a12.3  ",
                DiagnosisName = "  synthetic diagnosis  ",
                Memo = "  synthetic memo  "
            };

            var created = await service.CreateClaimCaseAsync(draft);
            var reloaded = Assert.Single(await service.GetClaimCasesAsync());

            Assert.Equal(created.Id, reloaded.Id);
            Assert.Null(reloaded.PolicyId);
            Assert.Equal(family.Id, reloaded.FamilyMemberId);
            Assert.Equal("synthetic claim", reloaded.DisplayTitle);
            Assert.Equal("synthetic hospital", reloaded.HospitalName);
            Assert.Equal("A12.3", reloaded.DiagnosisCode);
            Assert.Equal("synthetic diagnosis", reloaded.DiagnosisName);
            Assert.Equal("synthetic memo", reloaded.Memo);
            Assert.Equal(ClaimCaseValues.StatusDraft, reloaded.CaseStatus);
            Assert.Equal(1, reloaded.Revision);
        });
    }

    [Fact]
    public async Task Missing_or_inactive_family_is_rejected_without_claim_file()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var familyStore = new JsonFamilyMemberStorageService(rootPath);
            var service = new JsonPolicyClaimStorageService(rootPath, familyStore);
            var family = await CreateFamilyAsync(familyStore);
            await familyStore.DeactivateFamilyMemberAsync(family.Id, family.Version);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                service.CreateClaimCaseAsync(CreateDraft("family_missing")));
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                service.CreateClaimCaseAsync(CreateDraft(family.Id)));

            Assert.False(File.Exists(Path.Combine(rootPath, "claims.json")));
        });
    }

    [Fact]
    public async Task Update_targets_exact_id_and_increments_revision_once()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var familyStore = new JsonFamilyMemberStorageService(rootPath);
            var service = new JsonPolicyClaimStorageService(rootPath, familyStore);
            var family = await CreateFamilyAsync(familyStore);
            var first = await service.CreateClaimCaseAsync(CreateDraft(family.Id, "first"));
            var second = await service.CreateClaimCaseAsync(CreateDraft(family.Id, "second"));

            var updated = await service.UpdateClaimCaseAsync(
                second.Id,
                second.Revision,
                CreateDraft(family.Id, "second updated"));
            var claims = await service.GetClaimCasesAsync();

            Assert.Equal(2, claims.Count);
            Assert.Equal("first", Assert.Single(claims, item => item.Id == first.Id).DisplayTitle);
            Assert.Equal("second updated", Assert.Single(claims, item => item.Id == second.Id).DisplayTitle);
            Assert.Equal(second.Revision + 1, updated.Revision);
            Assert.Equal(ClaimCaseValues.StatusSaved, updated.CaseStatus);
        });
    }

    [Fact]
    public async Task Stale_revision_conflict_preserves_file_bytes()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var familyStore = new JsonFamilyMemberStorageService(rootPath);
            var service = new JsonPolicyClaimStorageService(rootPath, familyStore);
            var family = await CreateFamilyAsync(familyStore);
            var claim = await service.CreateClaimCaseAsync(CreateDraft(family.Id));
            var current = await service.UpdateClaimCaseAsync(
                claim.Id,
                claim.Revision,
                CreateDraft(family.Id, "current"));
            var claimsPath = Path.Combine(rootPath, "claims.json");
            var bytesBefore = await File.ReadAllBytesAsync(claimsPath);

            await Assert.ThrowsAsync<ClaimCaseConcurrencyException>(() =>
                service.UpdateClaimCaseAsync(
                    claim.Id,
                    claim.Revision,
                    CreateDraft(family.Id, "stale")));

            Assert.Equal(bytesBefore, await File.ReadAllBytesAsync(claimsPath));
            Assert.Equal(current.Revision, (await service.GetClaimCaseAsync(claim.Id))!.Revision);
        });
    }

    [Fact]
    public async Task Concurrent_same_revision_update_has_one_success_and_no_lost_update()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var familyStore = new JsonFamilyMemberStorageService(rootPath);
            var firstService = new JsonPolicyClaimStorageService(rootPath, familyStore);
            var secondService = new JsonPolicyClaimStorageService(rootPath, familyStore);
            var family = await CreateFamilyAsync(familyStore);
            var claim = await firstService.CreateClaimCaseAsync(CreateDraft(family.Id));

            var attempts = await Task.WhenAll(
                CaptureAsync(() => firstService.UpdateClaimCaseAsync(
                    claim.Id,
                    claim.Revision,
                    CreateDraft(family.Id, "writer one"))),
                CaptureAsync(() => secondService.UpdateClaimCaseAsync(
                    claim.Id,
                    claim.Revision,
                    CreateDraft(family.Id, "writer two"))));

            Assert.Single(attempts, result => result.Record is not null);
            Assert.Single(attempts, result => result.Exception is ClaimCaseConcurrencyException);
            var stored = await firstService.GetClaimCaseAsync(claim.Id);
            Assert.NotNull(stored);
            Assert.Equal(claim.Revision + 1, stored.Revision);
            Assert.Contains(stored.DisplayTitle, new[] { "writer one", "writer two" });
        });
    }

    [Theory]
    [InlineData(" ", "synthetic hospital", ClaimCaseValues.VisitTypeOutpatient, 0)]
    [InlineData("synthetic", " ", ClaimCaseValues.VisitTypeOutpatient, 0)]
    [InlineData("synthetic", "synthetic hospital", "unsupported", 0)]
    [InlineData("synthetic", "synthetic hospital", ClaimCaseValues.VisitTypeOutpatient, -1)]
    public async Task Invalid_required_visit_or_amount_values_are_rejected(
        string title,
        string hospital,
        string visitType,
        long amount)
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var familyStore = new JsonFamilyMemberStorageService(rootPath);
            var service = new JsonPolicyClaimStorageService(rootPath, familyStore);
            var family = await CreateFamilyAsync(familyStore);
            var draft = CreateDraft(family.Id) with
            {
                DisplayTitle = title,
                HospitalName = hospital,
                VisitType = visitType,
                CoveredAmount = amount
            };

            await Assert.ThrowsAnyAsync<ArgumentException>(() =>
                service.CreateClaimCaseAsync(draft));
        });
    }

    [Fact]
    public async Task Disable_increments_revision_and_repeated_disable_does_not_write()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var familyStore = new JsonFamilyMemberStorageService(rootPath);
            var service = new JsonPolicyClaimStorageService(rootPath, familyStore);
            var family = await CreateFamilyAsync(familyStore);
            var claim = await service.CreateClaimCaseAsync(CreateDraft(family.Id));

            var disabled = await service.DisableClaimCaseAsync(claim.Id, claim.Revision);
            var claimsPath = Path.Combine(rootPath, "claims.json");
            var bytesBeforeRepeat = await File.ReadAllBytesAsync(claimsPath);

            Assert.NotNull(disabled.DisabledAt);
            Assert.Equal(claim.Revision + 1, disabled.Revision);
            Assert.Empty(await service.GetClaimCasesAsync());
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                service.DisableClaimCaseAsync(claim.Id, disabled.Revision));
            Assert.Equal(bytesBeforeRepeat, await File.ReadAllBytesAsync(claimsPath));
        });
    }

    [Fact]
    public async Task Stale_disable_revision_is_conflict_and_does_not_write()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var familyStore = new JsonFamilyMemberStorageService(rootPath);
            var service = new JsonPolicyClaimStorageService(rootPath, familyStore);
            var family = await CreateFamilyAsync(familyStore);
            var claim = await service.CreateClaimCaseAsync(CreateDraft(family.Id));
            var updated = await service.UpdateClaimCaseAsync(
                claim.Id,
                claim.Revision,
                CreateDraft(family.Id, "updated before stale disable"));
            var claimsPath = Path.Combine(rootPath, "claims.json");
            var bytesBefore = await File.ReadAllBytesAsync(claimsPath);

            await Assert.ThrowsAsync<ClaimCaseConcurrencyException>(() =>
                service.DisableClaimCaseAsync(claim.Id, claim.Revision));

            Assert.Equal(bytesBefore, await File.ReadAllBytesAsync(claimsPath));
            var reloaded = await service.GetClaimCaseAsync(claim.Id);
            Assert.NotNull(reloaded);
            Assert.Null(reloaded.DisabledAt);
            Assert.Equal(updated.Revision, reloaded.Revision);
        });
    }

    [Fact]
    public async Task Write_failure_preserves_original_revision_and_cleans_temp()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var familyStore = new JsonFamilyMemberStorageService(rootPath);
            var service = new JsonPolicyClaimStorageService(rootPath, familyStore);
            var family = await CreateFamilyAsync(familyStore);
            var claim = await service.CreateClaimCaseAsync(CreateDraft(family.Id));
            var claimsPath = Path.Combine(rootPath, "claims.json");
            var bytesBefore = await File.ReadAllBytesAsync(claimsPath);

            await using (var lockStream = new FileStream(
                claimsPath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read))
            {
                await Assert.ThrowsAnyAsync<IOException>(() =>
                    service.UpdateClaimCaseAsync(
                        claim.Id,
                        claim.Revision,
                        CreateDraft(family.Id, "must not persist")));
            }

            Assert.Equal(bytesBefore, await File.ReadAllBytesAsync(claimsPath));
            Assert.Equal(claim.Revision, (await service.GetClaimCaseAsync(claim.Id))!.Revision);
            Assert.Empty(FindClaimTempFiles(rootPath));
        });
    }

    [Fact]
    public async Task Successful_replace_preserves_previous_backup_and_cleans_temp()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var familyStore = new JsonFamilyMemberStorageService(rootPath);
            var service = new JsonPolicyClaimStorageService(rootPath, familyStore);
            var family = await CreateFamilyAsync(familyStore);
            var claim = await service.CreateClaimCaseAsync(CreateDraft(family.Id));
            var claimsPath = Path.Combine(rootPath, "claims.json");
            var bytesBefore = await File.ReadAllBytesAsync(claimsPath);

            await service.UpdateClaimCaseAsync(
                claim.Id,
                claim.Revision,
                CreateDraft(family.Id, "updated"));

            Assert.Equal(bytesBefore, await File.ReadAllBytesAsync($"{claimsPath}.bak"));
            Assert.Empty(FindClaimTempFiles(rootPath));
        });
    }

    [Fact]
    public async Task Claim_document_link_file_is_unchanged_by_claim_mutation()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var familyStore = new JsonFamilyMemberStorageService(rootPath);
            var service = new JsonPolicyClaimStorageService(rootPath, familyStore);
            var family = await CreateFamilyAsync(familyStore);
            var claim = await service.CreateClaimCaseAsync(CreateDraft(family.Id));
            var linkPath = Path.Combine(rootPath, "claim-documents.json");
            var linkBytes = Encoding.UTF8.GetBytes("synthetic claim document link sentinel");
            await File.WriteAllBytesAsync(linkPath, linkBytes);

            await service.UpdateClaimCaseAsync(
                claim.Id,
                claim.Revision,
                CreateDraft(family.Id, "updated"));

            Assert.Equal(linkBytes, await File.ReadAllBytesAsync(linkPath));
        });
    }

    private static ClaimCaseDraft CreateDraft(
        string familyMemberId,
        string title = "synthetic claim")
    {
        return new ClaimCaseDraft(
            title,
            familyMemberId,
            new DateOnly(2026, 8, 7),
            "synthetic hospital",
            "a12.3",
            "synthetic diagnosis",
            ClaimCaseValues.VisitTypeOutpatient,
            HasSurgery: false,
            HasPrescription: true,
            CoveredAmount: 1000,
            NonCoveredAmount: 2000,
            PrescriptionAmount: 3000,
            Memo: "synthetic memo");
    }

    private static Task<FamilyMemberRecord> CreateFamilyAsync(
        JsonFamilyMemberStorageService storage)
    {
        return storage.CreateFamilyMemberAsync(new FamilyMemberDraft(
            "synthetic family",
            FamilyMemberRelationValues.Self,
            null));
    }

    private static Task<PolicyRecord> CreatePolicyAsync(
        JsonPolicyClaimStorageService service,
        string familyMemberId)
    {
        return service.CreateInsurancePolicyAsync(new InsurancePolicyDraft(
            "synthetic policy",
            familyMemberId,
            "synthetic insurer",
            InsurancePolicyValues.ContractStatusActive,
            new DateOnly(2026, 8, 1),
            "synthetic coverage",
            "synthetic payment",
            1000,
            InsurancePolicyValues.RenewalTypeRenewable,
            InsurancePolicyValues.RefundTypeRefundable,
            InsurancePolicyValues.BusinessTypeLife,
            InsurancePolicyValues.ProductCategoryMedicalExpense));
    }

    private static async Task<string> WriteLegacyClaimAsync(
        string rootPath,
        string policyId)
    {
        var claimsPath = Path.Combine(rootPath, "claims.json");
        var json = $$"""
            {
              "schemaVersion": 1,
              "savedAt": "2026-08-07T00:00:00+00:00",
              "items": [
                {
                  "id": "claim_legacy",
                  "policyId": "{{policyId}}",
                  "displayTitle": "legacy claim",
                  "referenceDate": "2026-08-01",
                  "createdAt": "2026-08-01T00:00:00+00:00",
                  "updatedAt": "2026-08-01T00:00:00+00:00",
                  "disabledAt": null
                }
              ]
            }
            """;
        await File.WriteAllTextAsync(claimsPath, json, Encoding.UTF8);
        return claimsPath;
    }

    private static string[] FindClaimTempFiles(string rootPath)
    {
        return Directory.GetFiles(rootPath, "claims.json.*.tmp", SearchOption.TopDirectoryOnly);
    }

    private static async Task<(ClaimRecord? Record, Exception? Exception)> CaptureAsync(
        Func<Task<ClaimRecord>> action)
    {
        try
        {
            return (await action(), null);
        }
        catch (Exception exception)
        {
            return (null, exception);
        }
    }

    private static async Task UsingTempRootAsync(Func<string, Task> action)
    {
        var rootPath = Path.Combine(
            Path.GetTempPath(),
            "FamilyClaimRef.App.Tests",
            nameof(ClaimCaseStorageServiceTests),
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
