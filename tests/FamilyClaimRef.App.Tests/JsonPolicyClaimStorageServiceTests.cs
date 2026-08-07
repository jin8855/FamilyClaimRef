using FamilyClaimRef.App.Models.Storage;
using FamilyClaimRef.App.Services.Storage;
using Xunit;

namespace FamilyClaimRef.App.Tests;

public sealed class JsonPolicyClaimStorageServiceTests
{
    private static readonly DateOnly ReferenceDate = new(2026, 7, 3);

    [Fact]
    public async Task Missing_json_files_return_empty_active_lists()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var service = new JsonPolicyClaimStorageService(rootPath);

            var policies = await service.GetPoliciesAsync();
            var claims = await service.GetClaimsAsync();
            var policyClaims = await service.GetClaimsByPolicyIdAsync("policy_missing");

            Assert.Empty(policies);
            Assert.Empty(claims);
            Assert.Empty(policyClaims);
        });
    }

    [Fact]
    public async Task AddPolicyAsync_creates_record_and_persists_to_json_file()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var service = new JsonPolicyClaimStorageService(rootPath);
            var draft = CreatePolicyDraft();

            var policy = await service.AddPolicyAsync(draft);

            Assert.StartsWith("policy_", policy.Id, StringComparison.Ordinal);
            Assert.Equal(draft.DisplayTitle, policy.DisplayTitle);
            Assert.Equal(draft.ReferenceDate, policy.ReferenceDate);
            Assert.NotEqual(default, policy.CreatedAt);
            Assert.NotEqual(default, policy.UpdatedAt);
            Assert.Null(policy.DisabledAt);
            Assert.True(File.Exists(Path.Combine(rootPath, "policies.json")));

            var reloadedService = new JsonPolicyClaimStorageService(rootPath);
            var reloaded = await reloadedService.GetPolicyAsync(policy.Id);

            Assert.Equal(policy, reloaded);
        });
    }

    [Fact]
    public async Task AddPolicyAsync_rejects_missing_display_title()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var service = new JsonPolicyClaimStorageService(rootPath);

            var exception = await Record.ExceptionAsync(() => service.AddPolicyAsync(new PolicyDraft(
                " ",
                ReferenceDate)));

            Assert.NotNull(exception);
            Assert.IsType<ArgumentException>(exception);
        });
    }

    [Fact]
    public async Task AddPolicyAsync_rejects_default_reference_date()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var service = new JsonPolicyClaimStorageService(rootPath);

            var exception = await Record.ExceptionAsync(() => service.AddPolicyAsync(new PolicyDraft(
                "Policy A",
                default)));

            Assert.NotNull(exception);
            Assert.IsType<ArgumentException>(exception);
        });
    }

    [Fact]
    public async Task Policy_queries_are_active_only()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var service = new JsonPolicyClaimStorageService(rootPath);
            var activePolicy = await service.AddPolicyAsync(CreatePolicyDraft("Policy Active"));
            var disabledPolicy = await service.AddPolicyAsync(CreatePolicyDraft("Policy Disabled"));

            await service.DisablePolicyAsync(disabledPolicy.Id);

            var policies = await service.GetPoliciesAsync();
            var activeLookup = await service.GetPolicyAsync(activePolicy.Id);
            var disabledLookup = await service.GetPolicyAsync(disabledPolicy.Id);
            var activeExists = await service.PolicyExistsAsync(activePolicy.Id);
            var disabledExists = await service.PolicyExistsAsync(disabledPolicy.Id);

            var policy = Assert.Single(policies);
            Assert.Equal(activePolicy.Id, policy.Id);
            Assert.Equal(activePolicy, activeLookup);
            Assert.Null(disabledLookup);
            Assert.True(activeExists);
            Assert.False(disabledExists);
        });
    }

    [Fact]
    public async Task DisablePolicyAsync_sets_disabledAt_and_updatedAt()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var service = new JsonPolicyClaimStorageService(rootPath);
            var policy = await service.AddPolicyAsync(CreatePolicyDraft());

            var disabledPolicy = await service.DisablePolicyAsync(policy.Id);

            Assert.NotNull(disabledPolicy.DisabledAt);
            Assert.Equal(disabledPolicy.DisabledAt, disabledPolicy.UpdatedAt);
        });
    }

    [Fact]
    public async Task DisablePolicyAsync_rejects_missing_policy()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var service = new JsonPolicyClaimStorageService(rootPath);

            var exception = await Record.ExceptionAsync(() => service.DisablePolicyAsync("policy_missing"));

            Assert.NotNull(exception);
            Assert.IsType<InvalidOperationException>(exception);
        });
    }

    [Fact]
    public async Task DisablePolicyAsync_rejects_already_disabled_policy()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var service = new JsonPolicyClaimStorageService(rootPath);
            var policy = await service.AddPolicyAsync(CreatePolicyDraft());
            await service.DisablePolicyAsync(policy.Id);

            var exception = await Record.ExceptionAsync(() => service.DisablePolicyAsync(policy.Id));

            Assert.NotNull(exception);
            Assert.IsType<InvalidOperationException>(exception);
        });
    }

    [Fact]
    public async Task AddClaimAsync_creates_record_for_active_policy_and_persists_to_json_file()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var service = new JsonPolicyClaimStorageService(rootPath);
            var policy = await service.AddPolicyAsync(CreatePolicyDraft());
            var draft = CreateClaimDraft(policy.Id);

            var claim = await service.AddClaimAsync(draft);

            Assert.StartsWith("claim_", claim.Id, StringComparison.Ordinal);
            Assert.Equal(policy.Id, claim.PolicyId);
            Assert.Equal(draft.DisplayTitle, claim.DisplayTitle);
            Assert.Equal(draft.ReferenceDate, claim.ReferenceDate);
            Assert.NotEqual(default, claim.CreatedAt);
            Assert.NotEqual(default, claim.UpdatedAt);
            Assert.Null(claim.DisabledAt);
            Assert.True(File.Exists(Path.Combine(rootPath, "claims.json")));

            var reloadedService = new JsonPolicyClaimStorageService(rootPath);
            var reloaded = await reloadedService.GetClaimAsync(claim.Id);

            Assert.Equal(claim, reloaded);
        });
    }

    [Fact]
    public async Task AddClaimAsync_rejects_missing_policy()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var service = new JsonPolicyClaimStorageService(rootPath);

            var exception = await Record.ExceptionAsync(() => service.AddClaimAsync(CreateClaimDraft("policy_missing")));

            Assert.NotNull(exception);
            Assert.IsType<InvalidOperationException>(exception);
        });
    }

    [Fact]
    public async Task AddClaimAsync_rejects_disabled_policy()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var service = new JsonPolicyClaimStorageService(rootPath);
            var policy = await service.AddPolicyAsync(CreatePolicyDraft());
            await service.DisablePolicyAsync(policy.Id);

            var exception = await Record.ExceptionAsync(() => service.AddClaimAsync(CreateClaimDraft(policy.Id)));

            Assert.NotNull(exception);
            Assert.IsType<InvalidOperationException>(exception);
        });
    }

    [Fact]
    public async Task AddClaimAsync_rejects_missing_policy_id()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var service = new JsonPolicyClaimStorageService(rootPath);

            var exception = await Record.ExceptionAsync(() => service.AddClaimAsync(new ClaimDraft(
                " ",
                "Claim A",
                ReferenceDate)));

            Assert.NotNull(exception);
            Assert.IsType<ArgumentException>(exception);
        });
    }

    [Fact]
    public async Task AddClaimAsync_rejects_missing_display_title()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var service = new JsonPolicyClaimStorageService(rootPath);
            var policy = await service.AddPolicyAsync(CreatePolicyDraft());

            var exception = await Record.ExceptionAsync(() => service.AddClaimAsync(new ClaimDraft(
                policy.Id,
                " ",
                ReferenceDate)));

            Assert.NotNull(exception);
            Assert.IsType<ArgumentException>(exception);
        });
    }

    [Fact]
    public async Task AddClaimAsync_rejects_default_reference_date()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var service = new JsonPolicyClaimStorageService(rootPath);
            var policy = await service.AddPolicyAsync(CreatePolicyDraft());

            var exception = await Record.ExceptionAsync(() => service.AddClaimAsync(new ClaimDraft(
                policy.Id,
                "Claim A",
                default)));

            Assert.NotNull(exception);
            Assert.IsType<ArgumentException>(exception);
        });
    }

    [Fact]
    public async Task Claim_queries_are_active_only()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var service = new JsonPolicyClaimStorageService(rootPath);
            var policy = await service.AddPolicyAsync(CreatePolicyDraft());
            var activeClaim = await service.AddClaimAsync(CreateClaimDraft(policy.Id, "Claim Active"));
            var disabledClaim = await service.AddClaimAsync(CreateClaimDraft(policy.Id, "Claim Disabled"));

            await service.DisableClaimAsync(disabledClaim.Id, disabledClaim.Revision);

            var claims = await service.GetClaimsAsync();
            var policyClaims = await service.GetClaimsByPolicyIdAsync(policy.Id);
            var activeLookup = await service.GetClaimAsync(activeClaim.Id);
            var disabledLookup = await service.GetClaimAsync(disabledClaim.Id);
            var activeExists = await service.ClaimExistsAsync(activeClaim.Id);
            var disabledExists = await service.ClaimExistsAsync(disabledClaim.Id);

            var claim = Assert.Single(claims);
            var policyClaim = Assert.Single(policyClaims);
            Assert.Equal(activeClaim.Id, claim.Id);
            Assert.Equal(activeClaim.Id, policyClaim.Id);
            Assert.Equal(activeClaim, activeLookup);
            Assert.Null(disabledLookup);
            Assert.True(activeExists);
            Assert.False(disabledExists);
        });
    }

    [Fact]
    public async Task GetClaimsByPolicyIdAsync_filters_active_claims_by_policy_id()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var service = new JsonPolicyClaimStorageService(rootPath);
            var policyA = await service.AddPolicyAsync(CreatePolicyDraft("Policy A"));
            var policyB = await service.AddPolicyAsync(CreatePolicyDraft("Policy B"));
            var claimA = await service.AddClaimAsync(CreateClaimDraft(policyA.Id, "Claim A"));
            await service.AddClaimAsync(CreateClaimDraft(policyB.Id, "Claim B"));

            var policyAClaims = await service.GetClaimsByPolicyIdAsync(policyA.Id);

            var policyAClaim = Assert.Single(policyAClaims);
            Assert.Equal(claimA.Id, policyAClaim.Id);
        });
    }

    [Fact]
    public async Task DisableClaimAsync_sets_disabledAt_and_updatedAt()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var service = new JsonPolicyClaimStorageService(rootPath);
            var policy = await service.AddPolicyAsync(CreatePolicyDraft());
            var claim = await service.AddClaimAsync(CreateClaimDraft(policy.Id));

            var disabledClaim = await service.DisableClaimAsync(claim.Id, claim.Revision);

            Assert.NotNull(disabledClaim.DisabledAt);
            Assert.Equal(disabledClaim.DisabledAt, disabledClaim.UpdatedAt);
        });
    }

    [Fact]
    public async Task DisableClaimAsync_rejects_missing_claim()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var service = new JsonPolicyClaimStorageService(rootPath);

            var exception = await Record.ExceptionAsync(() =>
                service.DisableClaimAsync("claim_missing", expectedRevision: 0));

            Assert.NotNull(exception);
            Assert.IsType<InvalidOperationException>(exception);
        });
    }

    [Fact]
    public async Task DisableClaimAsync_rejects_already_disabled_claim()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var service = new JsonPolicyClaimStorageService(rootPath);
            var policy = await service.AddPolicyAsync(CreatePolicyDraft());
            var claim = await service.AddClaimAsync(CreateClaimDraft(policy.Id));
            var disabled = await service.DisableClaimAsync(claim.Id, claim.Revision);

            var exception = await Record.ExceptionAsync(() =>
                service.DisableClaimAsync(claim.Id, disabled.Revision));

            Assert.NotNull(exception);
            Assert.IsType<InvalidOperationException>(exception);
        });
    }

    [Fact]
    public async Task Invalid_policies_json_load_fails_with_invalidOperationException()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            await File.WriteAllTextAsync(Path.Combine(rootPath, "policies.json"), "{ invalid json");
            var service = new JsonPolicyClaimStorageService(rootPath);

            var exception = await Record.ExceptionAsync(() => service.GetPoliciesAsync());

            Assert.NotNull(exception);
            Assert.IsType<InvalidOperationException>(exception);
        });
    }

    [Fact]
    public async Task Invalid_claims_json_load_fails_with_invalidOperationException()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            await File.WriteAllTextAsync(Path.Combine(rootPath, "claims.json"), "{ invalid json");
            var service = new JsonPolicyClaimStorageService(rootPath);

            var exception = await Record.ExceptionAsync(() => service.GetClaimsAsync());

            Assert.NotNull(exception);
            Assert.IsType<InvalidOperationException>(exception);
        });
    }

    [Fact]
    public async Task Policy_schema_version_mismatch_fails_with_invalidOperationException()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            await WriteEnvelopeAsync(rootPath, "policies.json", schemaVersion: 2, itemsJson: "[]");
            var service = new JsonPolicyClaimStorageService(rootPath);

            var exception = await Record.ExceptionAsync(() => service.GetPoliciesAsync());

            Assert.NotNull(exception);
            Assert.IsType<InvalidOperationException>(exception);
        });
    }

    [Fact]
    public async Task Claim_schema_version_mismatch_fails_with_invalidOperationException()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            await WriteEnvelopeAsync(rootPath, "claims.json", schemaVersion: 2, itemsJson: "[]");
            var service = new JsonPolicyClaimStorageService(rootPath);

            var exception = await Record.ExceptionAsync(() => service.GetClaimsAsync());

            Assert.NotNull(exception);
            Assert.IsType<InvalidOperationException>(exception);
        });
    }

    [Fact]
    public async Task Null_policy_items_fails_with_invalidOperationException()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            await WriteEnvelopeAsync(rootPath, "policies.json", schemaVersion: 1, itemsJson: "null");
            var service = new JsonPolicyClaimStorageService(rootPath);

            var exception = await Record.ExceptionAsync(() => service.GetPoliciesAsync());

            Assert.NotNull(exception);
            Assert.IsType<InvalidOperationException>(exception);
        });
    }

    [Fact]
    public async Task Null_claim_items_fails_with_invalidOperationException()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            await WriteEnvelopeAsync(rootPath, "claims.json", schemaVersion: 1, itemsJson: "null");
            var service = new JsonPolicyClaimStorageService(rootPath);

            var exception = await Record.ExceptionAsync(() => service.GetClaimsAsync());

            Assert.NotNull(exception);
            Assert.IsType<InvalidOperationException>(exception);
        });
    }

    [Fact]
    public async Task Storage_files_are_created_only_under_configured_root()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var service = new JsonPolicyClaimStorageService(rootPath);
            var policy = await service.AddPolicyAsync(CreatePolicyDraft());
            await service.AddClaimAsync(CreateClaimDraft(policy.Id));

            var files = Directory
                .GetFiles(rootPath, "*", SearchOption.AllDirectories)
                .Select(Path.GetFileName)
                .Order(StringComparer.Ordinal)
                .ToList();

            Assert.Equal(["claims.json", "policies.json"], files);
        });
    }

    private static PolicyDraft CreatePolicyDraft(string displayTitle = "Policy A")
    {
        return new PolicyDraft(displayTitle, ReferenceDate);
    }

    private static ClaimDraft CreateClaimDraft(string policyId, string displayTitle = "Claim A")
    {
        return new ClaimDraft(policyId, displayTitle, ReferenceDate);
    }

    private static async Task WriteEnvelopeAsync(
        string rootPath,
        string fileName,
        int schemaVersion,
        string itemsJson)
    {
        await File.WriteAllTextAsync(
            Path.Combine(rootPath, fileName),
            $$"""
            {
              "schemaVersion": {{schemaVersion}},
              "savedAt": "2026-01-01T00:00:00Z",
              "items": {{itemsJson}}
            }
            """);
    }

    private static async Task UsingTempRootAsync(Func<string, Task> action)
    {
        var rootPath = Path.Combine(Path.GetTempPath(), "FamilyClaimRef.App.Tests", Guid.NewGuid().ToString("N"));
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
