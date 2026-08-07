using System.Text;
using FamilyClaimRef.App.Models.Storage;
using FamilyClaimRef.App.Services.Storage;
using Xunit;

namespace FamilyClaimRef.App.Tests;

public sealed class ClaimSubmissionStorageServiceTests
{
    [Fact]
    public async Task Missing_file_returns_empty_without_creating_storage()
    {
        await UsingFixtureAsync(async fixture =>
        {
            var records = await fixture.Submissions.GetByClaimCaseAsync("claim_missing");

            Assert.Empty(records);
            Assert.False(File.Exists(fixture.SubmissionPath));
        });
    }

    [Fact]
    public async Task Preparing_submissions_allow_multiple_records_for_same_claim_and_policy_and_reload()
    {
        await UsingFixtureAsync(async fixture =>
        {
            var family = await fixture.CreateFamilyAsync();
            var policy = await fixture.CreatePolicyAsync(family.Id);
            var claim = await fixture.CreateSavedClaimAsync(family.Id);

            var first = await fixture.Submissions.CreateAsync(CreateDraft(claim.Id, policy.Id));
            var second = await fixture.Submissions.CreateAsync(CreateDraft(claim.Id, policy.Id));
            var reloaded = new JsonClaimSubmissionStorageService(
                fixture.RootPath,
                fixture.PolicyClaims,
                fixture.PolicyClaims,
                fixture.Documents);

            var records = await reloaded.GetByClaimCaseAsync(claim.Id);
            Assert.Equal(2, records.Count);
            Assert.Contains(records, record => record.Id == first.Id);
            Assert.Contains(records, record => record.Id == second.Id);
            Assert.All(records, record =>
            {
                Assert.Equal(ClaimSubmissionValues.StatusPreparing, record.Status);
                Assert.Equal(1, record.Revision);
            });
        });
    }

    [Fact]
    public async Task Create_rejects_non_preparing_and_invalid_values_without_write()
    {
        await UsingFixtureAsync(async fixture =>
        {
            var family = await fixture.CreateFamilyAsync();
            var policy = await fixture.CreatePolicyAsync(family.Id);
            var claim = await fixture.CreateSavedClaimAsync(family.Id);

            Assert.IsType<ClaimSubmissionTransitionException>(await Record.ExceptionAsync(() =>
                fixture.Submissions.CreateAsync(CreateDraft(claim.Id, policy.Id) with
                {
                    Status = ClaimSubmissionValues.StatusSubmitted,
                    CoverageDisplayName = "coverage",
                    SubmittedDate = new DateOnly(2026, 8, 8)
                })));
            Assert.IsType<ArgumentOutOfRangeException>(await Record.ExceptionAsync(() =>
                fixture.Submissions.CreateAsync(CreateDraft(claim.Id, policy.Id) with
                {
                    SubmittedAmount = -1
                })));
            Assert.IsType<ArgumentException>(await Record.ExceptionAsync(() =>
                fixture.Submissions.CreateAsync(CreateDraft(" ", policy.Id))));
            Assert.False(File.Exists(fixture.SubmissionPath));
        });
    }

    [Fact]
    public async Task Claimable_policy_requires_saved_claim_active_policy_and_matching_family()
    {
        await UsingFixtureAsync(async fixture =>
        {
            var firstFamily = await fixture.CreateFamilyAsync("first family");
            var secondFamily = await fixture.CreateFamilyAsync("second family");
            var active = await fixture.CreatePolicyAsync(firstFamily.Id, "active policy");
            _ = await fixture.CreatePolicyAsync(
                firstFamily.Id,
                "expired policy",
                InsurancePolicyValues.ContractStatusExpired);
            _ = await fixture.CreatePolicyAsync(secondFamily.Id, "other family policy");
            var savedClaim = await fixture.CreateSavedClaimAsync(firstFamily.Id);
            var draftClaim = await fixture.PolicyClaims.CreateClaimCaseAsync(
                CreateClaimDraft(firstFamily.Id, "draft claim"));

            var policies = await fixture.Submissions.GetClaimablePoliciesAsync(savedClaim.Id);

            Assert.Equal(active.Id, Assert.Single(policies).Id);
            Assert.IsType<ClaimSubmissionReferenceException>(await Record.ExceptionAsync(() =>
                fixture.Submissions.GetClaimablePoliciesAsync(draftClaim.Id)));
        });
    }

    [Fact]
    public async Task Create_rejects_cross_family_and_expired_policy_without_write()
    {
        await UsingFixtureAsync(async fixture =>
        {
            var claimFamily = await fixture.CreateFamilyAsync("claim family");
            var otherFamily = await fixture.CreateFamilyAsync("other family");
            var claim = await fixture.CreateSavedClaimAsync(claimFamily.Id);
            var otherPolicy = await fixture.CreatePolicyAsync(otherFamily.Id, "other policy");
            var expiredPolicy = await fixture.CreatePolicyAsync(
                claimFamily.Id,
                "expired policy",
                InsurancePolicyValues.ContractStatusExpired);

            Assert.IsType<ClaimSubmissionReferenceException>(await Record.ExceptionAsync(() =>
                fixture.Submissions.CreateAsync(CreateDraft(claim.Id, otherPolicy.Id))));
            Assert.IsType<ClaimSubmissionReferenceException>(await Record.ExceptionAsync(() =>
                fixture.Submissions.CreateAsync(CreateDraft(claim.Id, expiredPolicy.Id))));
            Assert.False(File.Exists(fixture.SubmissionPath));
        });
    }

    [Fact]
    public async Task Submitted_details_and_active_same_claim_documents_are_enforced()
    {
        await UsingFixtureAsync(async fixture =>
        {
            var family = await fixture.CreateFamilyAsync();
            var policy = await fixture.CreatePolicyAsync(family.Id);
            var claim = await fixture.CreateSavedClaimAsync(family.Id);
            var otherClaim = await fixture.CreateSavedClaimAsync(family.Id, "other claim");
            var activeLink = await fixture.CreateClaimDocumentAsync(claim.Id, "active document");
            var otherLink = await fixture.CreateClaimDocumentAsync(otherClaim.Id, "other document");
            var created = await fixture.Submissions.CreateAsync(CreateDraft(claim.Id, policy.Id));

            Assert.IsType<ArgumentException>(await Record.ExceptionAsync(() =>
                fixture.Submissions.UpdateAsync(
                    created.Id,
                    created.Revision,
                    CreateDraft(claim.Id, policy.Id) with
                    {
                        Status = ClaimSubmissionValues.StatusSubmitted
                    })));
            Assert.IsType<ArgumentException>(await Record.ExceptionAsync(() =>
                fixture.Submissions.UpdateAsync(
                    created.Id,
                    created.Revision,
                    SubmittedDraft(claim.Id, policy.Id) with
                    {
                        SubmittedAmount = null
                    })));
            Assert.IsType<ClaimSubmissionReferenceException>(await Record.ExceptionAsync(() =>
                fixture.Submissions.UpdateAsync(
                    created.Id,
                    created.Revision,
                    SubmittedDraft(claim.Id, policy.Id, otherLink.Id))));

            await fixture.Documents.DisableClaimDocumentAsync(otherLink.Id, DateTimeOffset.UtcNow);
            var submitted = await fixture.Submissions.UpdateAsync(
                created.Id,
                created.Revision,
                SubmittedDraft(claim.Id, policy.Id, activeLink.Id));

            Assert.Equal(ClaimSubmissionValues.StatusSubmitted, submitted.Status);
            Assert.Equal(activeLink.Id, Assert.Single(submitted.SubmittedClaimDocumentIds));
            Assert.Equal(2, submitted.Revision);
        });
    }

    [Fact]
    public async Task Update_targets_exact_id_preserves_references_and_requires_revision()
    {
        await UsingFixtureAsync(async fixture =>
        {
            var family = await fixture.CreateFamilyAsync();
            var firstPolicy = await fixture.CreatePolicyAsync(family.Id, "first policy");
            var secondPolicy = await fixture.CreatePolicyAsync(family.Id, "second policy");
            var claim = await fixture.CreateSavedClaimAsync(family.Id);
            var first = await fixture.Submissions.CreateAsync(CreateDraft(claim.Id, firstPolicy.Id));
            var second = await fixture.Submissions.CreateAsync(CreateDraft(claim.Id, firstPolicy.Id));

            var updated = await fixture.Submissions.UpdateAsync(
                first.Id,
                first.Revision,
                CreateDraft(claim.Id, firstPolicy.Id) with { Memo = "updated" });

            Assert.Equal(first.Id, updated.Id);
            Assert.Equal(2, updated.Revision);
            Assert.Equal("updated", updated.Memo);
            Assert.Equal(1, (await fixture.Submissions.GetAsync(second.Id))!.Revision);
            Assert.IsType<ClaimSubmissionConcurrencyException>(await Record.ExceptionAsync(() =>
                fixture.Submissions.UpdateAsync(first.Id, first.Revision, CreateDraft(claim.Id, firstPolicy.Id))));
            Assert.IsType<ClaimSubmissionReferenceException>(await Record.ExceptionAsync(() =>
                fixture.Submissions.UpdateAsync(
                    first.Id,
                    updated.Revision,
                    CreateDraft(claim.Id, secondPolicy.Id))));
        });
    }

    [Fact]
    public async Task State_machine_allows_contract_transitions_and_blocks_terminal_mutation()
    {
        await UsingFixtureAsync(async fixture =>
        {
            var family = await fixture.CreateFamilyAsync();
            var policy = await fixture.CreatePolicyAsync(family.Id);
            var claim = await fixture.CreateSavedClaimAsync(family.Id);
            var created = await fixture.Submissions.CreateAsync(CreateDraft(claim.Id, policy.Id));
            var submitted = await fixture.Submissions.UpdateAsync(
                created.Id,
                created.Revision,
                SubmittedDraft(claim.Id, policy.Id));

            Assert.IsType<ClaimSubmissionTransitionException>(await Record.ExceptionAsync(() =>
                fixture.Submissions.UpdateAsync(
                    submitted.Id,
                    submitted.Revision,
                    SubmittedDraft(claim.Id, policy.Id) with
                    {
                        Status = ClaimSubmissionValues.StatusPreparing
                    })));
            var reviewing = await fixture.Submissions.UpdateAsync(
                submitted.Id,
                submitted.Revision,
                SubmittedDraft(claim.Id, policy.Id) with
                {
                    Status = ClaimSubmissionValues.StatusReviewing
                });
            var completed = await fixture.Submissions.UpdateAsync(
                reviewing.Id,
                reviewing.Revision,
                SubmittedDraft(claim.Id, policy.Id) with
                {
                    Status = ClaimSubmissionValues.StatusCompleted
                });

            Assert.Equal(4, completed.Revision);
            Assert.IsType<ClaimSubmissionTransitionException>(await Record.ExceptionAsync(() =>
                fixture.Submissions.UpdateAsync(
                    completed.Id,
                    completed.Revision,
                    SubmittedDraft(claim.Id, policy.Id))));
        });
    }

    [Fact]
    public async Task Submitted_documents_can_be_added_but_not_removed()
    {
        await UsingFixtureAsync(async fixture =>
        {
            var family = await fixture.CreateFamilyAsync();
            var policy = await fixture.CreatePolicyAsync(family.Id);
            var claim = await fixture.CreateSavedClaimAsync(family.Id);
            var firstLink = await fixture.CreateClaimDocumentAsync(claim.Id, "first document");
            var secondLink = await fixture.CreateClaimDocumentAsync(claim.Id, "second document");
            var created = await fixture.Submissions.CreateAsync(CreateDraft(claim.Id, policy.Id));
            var submitted = await fixture.Submissions.UpdateAsync(
                created.Id,
                created.Revision,
                SubmittedDraft(claim.Id, policy.Id, firstLink.Id));
            var added = await fixture.Submissions.UpdateAsync(
                submitted.Id,
                submitted.Revision,
                SubmittedDraft(claim.Id, policy.Id, firstLink.Id, secondLink.Id));

            Assert.Equal(2, added.SubmittedClaimDocumentIds.Length);
            Assert.IsType<ClaimSubmissionReferenceException>(await Record.ExceptionAsync(() =>
                fixture.Submissions.UpdateAsync(
                    added.Id,
                    added.Revision,
                    SubmittedDraft(claim.Id, policy.Id, secondLink.Id))));
        });
    }

    [Fact]
    public async Task Concurrent_same_revision_update_has_exactly_one_success_and_one_conflict()
    {
        await UsingFixtureAsync(async fixture =>
        {
            var family = await fixture.CreateFamilyAsync();
            var policy = await fixture.CreatePolicyAsync(family.Id);
            var claim = await fixture.CreateSavedClaimAsync(family.Id);
            var created = await fixture.Submissions.CreateAsync(CreateDraft(claim.Id, policy.Id));

            var results = await Task.WhenAll(
                CaptureAsync(() => fixture.Submissions.UpdateAsync(
                    created.Id,
                    created.Revision,
                    CreateDraft(claim.Id, policy.Id) with { Memo = "first" })),
                CaptureAsync(() => fixture.Submissions.UpdateAsync(
                    created.Id,
                    created.Revision,
                    CreateDraft(claim.Id, policy.Id) with { Memo = "second" })));

            Assert.Single(results, result => result.Record is not null);
            Assert.Single(results, result => result.Exception is ClaimSubmissionConcurrencyException);
            Assert.Equal(2, (await fixture.Submissions.GetAsync(created.Id))!.Revision);
            Assert.Empty(Directory.GetFiles(fixture.RootPath, "claim-submissions.json.*.tmp"));
        });
    }

    [Fact]
    public async Task Malformed_and_unsupported_envelopes_fail_closed()
    {
        await UsingFixtureAsync(async fixture =>
        {
            await File.WriteAllTextAsync(fixture.SubmissionPath, "{ invalid json", Encoding.UTF8);
            Assert.IsType<InvalidOperationException>(await Record.ExceptionAsync(() =>
                fixture.Submissions.GetAsync("submission_any")));

            await File.WriteAllTextAsync(
                fixture.SubmissionPath,
                """
                {
                  "schemaVersion": 2,
                  "savedAt": "2026-08-08T00:00:00Z",
                  "items": []
                }
                """,
                Encoding.UTF8);
            Assert.IsType<InvalidOperationException>(await Record.ExceptionAsync(() =>
                fixture.Submissions.GetAsync("submission_any")));
        });
    }

    [Fact]
    public async Task Locked_write_preserves_original_revision_backup_and_cleans_temp()
    {
        await UsingFixtureAsync(async fixture =>
        {
            var family = await fixture.CreateFamilyAsync();
            var policy = await fixture.CreatePolicyAsync(family.Id);
            var claim = await fixture.CreateSavedClaimAsync(family.Id);
            var created = await fixture.Submissions.CreateAsync(CreateDraft(claim.Id, policy.Id));
            var submitted = await fixture.Submissions.UpdateAsync(
                created.Id,
                created.Revision,
                SubmittedDraft(claim.Id, policy.Id));
            var before = await File.ReadAllBytesAsync(fixture.SubmissionPath);
            var backupPath = fixture.SubmissionPath + ".bak";
            var backupBefore = await File.ReadAllBytesAsync(backupPath);

            Exception? exception;
            using (new FileStream(
                fixture.SubmissionPath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read))
            {
                exception = await Record.ExceptionAsync(() => fixture.Submissions.UpdateAsync(
                    submitted.Id,
                    submitted.Revision,
                    SubmittedDraft(claim.Id, policy.Id) with
                    {
                        Status = ClaimSubmissionValues.StatusReviewing
                    }));
            }

            Assert.NotNull(exception);
            Assert.Equal(before, await File.ReadAllBytesAsync(fixture.SubmissionPath));
            Assert.Equal(backupBefore, await File.ReadAllBytesAsync(backupPath));
            Assert.Equal(submitted.Revision, (await fixture.Submissions.GetAsync(submitted.Id))!.Revision);
            Assert.Empty(Directory.GetFiles(fixture.RootPath, "claim-submissions.json.*.tmp"));
        });
    }

    [Fact]
    public async Task Unresolved_legacy_claim_owner_returns_dedicated_error_without_submission_write()
    {
        await UsingFixtureAsync(async fixture =>
        {
            await File.WriteAllTextAsync(
                Path.Combine(fixture.RootPath, "claims.json"),
                """
                {
                  "schemaVersion": 1,
                  "savedAt": "2026-08-08T00:00:00Z",
                  "items": [
                    {
                      "id": "claim_legacy_unresolved",
                      "policyId": "policy_missing",
                      "displayTitle": "legacy claim",
                      "referenceDate": "2026-08-08",
                      "createdAt": "2026-08-08T00:00:00Z",
                      "updatedAt": "2026-08-08T00:00:00Z",
                      "disabledAt": null
                    }
                  ]
                }
                """,
                Encoding.UTF8);

            var exception = await Record.ExceptionAsync(() =>
                fixture.Submissions.GetClaimablePoliciesAsync("claim_legacy_unresolved"));

            Assert.IsType<ClaimSubmissionLegacyReviewRequiredException>(exception);
            Assert.False(File.Exists(fixture.SubmissionPath));
        });
    }

    private static ClaimSubmissionDraft CreateDraft(string claimCaseId, string policyId)
    {
        return new ClaimSubmissionDraft(
            claimCaseId,
            policyId,
            PolicyCoverageId: null,
            CoverageDisplayName: null,
            SubmittedDate: null,
            SubmittedAmount: null,
            SubmittedClaimDocumentIds: [],
            Status: ClaimSubmissionValues.StatusPreparing,
            Memo: null);
    }

    private static ClaimSubmissionDraft SubmittedDraft(
        string claimCaseId,
        string policyId,
        params string[] documentIds)
    {
        return CreateDraft(claimCaseId, policyId) with
        {
            CoverageDisplayName = "synthetic coverage",
            SubmittedDate = new DateOnly(2026, 8, 8),
            SubmittedAmount = 12_000,
            SubmittedClaimDocumentIds = documentIds,
            Status = ClaimSubmissionValues.StatusSubmitted,
            Memo = "synthetic memo"
        };
    }

    private static ClaimCaseDraft CreateClaimDraft(string familyMemberId, string title)
    {
        return new ClaimCaseDraft(
            title,
            familyMemberId,
            new DateOnly(2026, 8, 8),
            "synthetic hospital",
            "a12.3",
            "synthetic diagnosis",
            ClaimCaseValues.VisitTypeOutpatient,
            HasSurgery: false,
            HasPrescription: true,
            CoveredAmount: 1_000,
            NonCoveredAmount: 2_000,
            PrescriptionAmount: 3_000,
            Memo: "synthetic memo");
    }

    private static InsurancePolicyDraft CreatePolicyDraft(
        string familyMemberId,
        string title,
        string status)
    {
        return new InsurancePolicyDraft(
            title,
            familyMemberId,
            "synthetic insurer",
            status,
            new DateOnly(2026, 8, 1),
            "synthetic coverage",
            "20 years",
            12_000_000m,
            InsurancePolicyValues.RenewalTypeFixed,
            InsurancePolicyValues.RefundTypeRefundable,
            InsurancePolicyValues.BusinessTypeLife,
            InsurancePolicyValues.ProductCategoryMedicalExpense);
    }

    private static async Task<(ClaimSubmissionRecord? Record, Exception? Exception)> CaptureAsync(
        Func<Task<ClaimSubmissionRecord>> action)
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

    private static async Task UsingFixtureAsync(Func<TestFixture, Task> action)
    {
        await using var fixture = new TestFixture();
        await action(fixture);
    }

    private sealed class TestFixture : IAsyncDisposable
    {
        public TestFixture()
        {
            RootPath = Path.Combine(
                Path.GetTempPath(),
                "FamilyClaimRef.App.Tests",
                nameof(ClaimSubmissionStorageServiceTests),
                Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(RootPath);
            Families = new JsonFamilyMemberStorageService(RootPath);
            PolicyClaims = new JsonPolicyClaimStorageService(RootPath, Families);
            Documents = new JsonDocumentStorageService(RootPath);
            Submissions = new JsonClaimSubmissionStorageService(
                RootPath,
                PolicyClaims,
                PolicyClaims,
                Documents);
        }

        public string RootPath { get; }

        public string SubmissionPath => Path.Combine(RootPath, "claim-submissions.json");

        public JsonFamilyMemberStorageService Families { get; }

        public JsonPolicyClaimStorageService PolicyClaims { get; }

        public JsonDocumentStorageService Documents { get; }

        public JsonClaimSubmissionStorageService Submissions { get; }

        public Task<FamilyMemberRecord> CreateFamilyAsync(string title = "synthetic family")
        {
            return Families.CreateFamilyMemberAsync(new FamilyMemberDraft(
                title,
                FamilyMemberRelationValues.Self,
                null));
        }

        public Task<PolicyRecord> CreatePolicyAsync(
            string familyMemberId,
            string title = "synthetic policy",
            string status = InsurancePolicyValues.ContractStatusActive)
        {
            return PolicyClaims.CreateInsurancePolicyAsync(
                CreatePolicyDraft(familyMemberId, title, status));
        }

        public async Task<ClaimRecord> CreateSavedClaimAsync(
            string familyMemberId,
            string title = "synthetic claim")
        {
            var draft = CreateClaimDraft(familyMemberId, title);
            var created = await PolicyClaims.CreateClaimCaseAsync(draft);
            return await PolicyClaims.UpdateClaimCaseAsync(created.Id, created.Revision, draft);
        }

        public async Task<ClaimDocumentRecord> CreateClaimDocumentAsync(
            string claimId,
            string title)
        {
            var physicalName = $"claim-{Guid.NewGuid():N}.pdf";
            var document = await Documents.AddDocumentAsync(new DocumentDraft(
                physicalName,
                title,
                "pdf",
                $"claims/{claimId}/{physicalName}"));
            return await Documents.AddClaimDocumentAsync(new ClaimDocumentDraft(
                claimId,
                document.Id,
                "receipt"));
        }

        public ValueTask DisposeAsync()
        {
            if (Directory.Exists(RootPath))
            {
                Directory.Delete(RootPath, recursive: true);
            }

            return ValueTask.CompletedTask;
        }
    }
}
