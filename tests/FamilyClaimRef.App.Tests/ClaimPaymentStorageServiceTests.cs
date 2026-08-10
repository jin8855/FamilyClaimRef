using System.Text;
using System.Text.Json.Nodes;
using FamilyClaimRef.App.Models.Storage;
using FamilyClaimRef.App.Services.Storage;
using Xunit;

namespace FamilyClaimRef.App.Tests;

public sealed class ClaimPaymentStorageServiceTests
{
    [Fact]
    public async Task Missing_file_returns_empty_without_creating_storage()
    {
        await UsingFixtureAsync(async fixture =>
        {
            Assert.Empty(await fixture.Payments.GetBySubmissionAsync("submission_missing"));
            Assert.False(File.Exists(fixture.PaymentPath));
        });
    }

    [Fact]
    public async Task Pending_payments_allow_multiple_records_for_same_submission_and_reload()
    {
        await UsingFixtureAsync(async fixture =>
        {
            var submission = await fixture.CreateSubmissionAsync(ClaimSubmissionValues.StatusSubmitted);
            var first = await fixture.Payments.CreateAsync(PendingDraft(submission.Id));
            var second = await fixture.Payments.CreateAsync(PendingDraft(submission.Id));
            var reloaded = fixture.CreatePaymentStorage();

            var records = await reloaded.GetBySubmissionAsync(submission.Id);

            Assert.Equal(2, records.Count);
            Assert.Contains(records, record => record.Id == first.Id);
            Assert.Contains(records, record => record.Id == second.Id);
            Assert.All(records, record =>
            {
                Assert.Equal(ClaimPaymentValues.StatusPending, record.Status);
                Assert.Equal(1, record.Revision);
            });
        });
    }

    [Fact]
    public async Task Exact_id_update_increments_revision_and_stale_revision_is_no_write()
    {
        await UsingFixtureAsync(async fixture =>
        {
            var submission = await fixture.CreateSubmissionAsync(ClaimSubmissionValues.StatusSubmitted);
            var first = await fixture.Payments.CreateAsync(PendingDraft(submission.Id));
            var second = await fixture.Payments.CreateAsync(PendingDraft(submission.Id));
            var updated = await fixture.Payments.UpdateAsync(
                first.Id,
                first.Revision,
                PendingDraft(submission.Id) with { Memo = " updated " });
            var before = await File.ReadAllBytesAsync(fixture.PaymentPath);
            var backupBefore = await File.ReadAllBytesAsync(fixture.PaymentPath + ".bak");

            var exception = await Record.ExceptionAsync(() => fixture.Payments.UpdateAsync(
                first.Id,
                first.Revision,
                PendingDraft(submission.Id) with { Memo = "stale" }));

            Assert.Equal(first.Id, updated.Id);
            Assert.Equal(2, updated.Revision);
            Assert.Equal("updated", updated.Memo);
            Assert.Equal(1, (await fixture.Payments.GetAsync(second.Id))!.Revision);
            Assert.IsType<ClaimPaymentConcurrencyException>(exception);
            Assert.Equal(before, await File.ReadAllBytesAsync(fixture.PaymentPath));
            Assert.Equal(backupBefore, await File.ReadAllBytesAsync(fixture.PaymentPath + ".bak"));
            Assert.Empty(fixture.PaymentTempFiles());
        });
    }

    [Fact]
    public async Task Concurrent_same_revision_update_has_one_success_and_one_conflict()
    {
        await UsingFixtureAsync(async fixture =>
        {
            var submission = await fixture.CreateSubmissionAsync(ClaimSubmissionValues.StatusSubmitted);
            var created = await fixture.Payments.CreateAsync(PendingDraft(submission.Id));

            var results = await Task.WhenAll(
                CaptureAsync(() => fixture.Payments.UpdateAsync(
                    created.Id,
                    created.Revision,
                    PendingDraft(submission.Id) with { Memo = "first" })),
                CaptureAsync(() => fixture.Payments.UpdateAsync(
                    created.Id,
                    created.Revision,
                    PendingDraft(submission.Id) with { Memo = "second" })));

            Assert.Single(results, result => result.Record is not null);
            Assert.Single(results, result => result.Exception is ClaimPaymentConcurrencyException);
            Assert.Equal(2, (await fixture.Payments.GetAsync(created.Id))!.Revision);
            Assert.Empty(fixture.PaymentTempFiles());
        });
    }

    [Theory]
    [InlineData(ClaimPaymentValues.StatusPaid)]
    [InlineData(ClaimPaymentValues.StatusPartiallyPaid)]
    [InlineData(ClaimPaymentValues.StatusDenied)]
    [InlineData(ClaimPaymentValues.StatusCancelled)]
    public async Task Pending_allows_each_terminal_transition_and_terminal_records_are_locked(
        string terminalStatus)
    {
        await UsingFixtureAsync(async fixture =>
        {
            var submission = await fixture.CreateSubmissionAsync(ClaimSubmissionValues.StatusCompleted);
            var created = await fixture.Payments.CreateAsync(PendingDraft(submission.Id));
            var terminal = await fixture.Payments.UpdateAsync(
                created.Id,
                created.Revision,
                ResultDraft(submission.Id, terminalStatus));

            var exception = await Record.ExceptionAsync(() => fixture.Payments.UpdateAsync(
                terminal.Id,
                terminal.Revision,
                ResultDraft(submission.Id, terminalStatus) with { Memo = "blocked" }));

            Assert.Equal(2, terminal.Revision);
            Assert.IsType<ClaimPaymentTransitionException>(exception);
            Assert.Equal(terminal.Revision, (await fixture.Payments.GetAsync(terminal.Id))!.Revision);
        });
    }

    [Fact]
    public async Task Invalid_transitions_and_result_fields_are_rejected_without_write()
    {
        await UsingFixtureAsync(async fixture =>
        {
            var submission = await fixture.CreateSubmissionAsync(ClaimSubmissionValues.StatusCompleted);
            var created = await fixture.Payments.CreateAsync(PendingDraft(submission.Id));
            var before = await File.ReadAllBytesAsync(fixture.PaymentPath);

            Assert.IsType<ClaimPaymentTransitionException>(await Record.ExceptionAsync(() =>
                fixture.Payments.CreateAsync(ResultDraft(submission.Id, ClaimPaymentValues.StatusPaid))));
            Assert.IsType<ArgumentException>(await Record.ExceptionAsync(() => fixture.Payments.UpdateAsync(
                created.Id,
                created.Revision,
                PendingDraft(submission.Id) with { Status = "unsupported" })));
            Assert.IsType<ArgumentException>(await Record.ExceptionAsync(() => fixture.Payments.UpdateAsync(
                created.Id,
                created.Revision,
                ResultDraft(submission.Id, ClaimPaymentValues.StatusPaid) with { PaidDate = null })));
            Assert.IsType<ArgumentOutOfRangeException>(await Record.ExceptionAsync(() =>
                fixture.Payments.UpdateAsync(
                    created.Id,
                    created.Revision,
                    PendingDraft(submission.Id) with { PaidAmount = 0 })));
            Assert.IsType<ArgumentOutOfRangeException>(await Record.ExceptionAsync(() =>
                fixture.Payments.UpdateAsync(
                    created.Id,
                    created.Revision,
                    PendingDraft(submission.Id) with { PaidAmount = -1 })));
            Assert.IsType<ArgumentException>(await Record.ExceptionAsync(() => fixture.Payments.UpdateAsync(
                created.Id,
                created.Revision,
                ResultDraft(submission.Id, ClaimPaymentValues.StatusPartiallyPaid) with
                {
                    ReductionReason = null
                })));
            Assert.IsType<ArgumentException>(await Record.ExceptionAsync(() => fixture.Payments.UpdateAsync(
                created.Id,
                created.Revision,
                ResultDraft(submission.Id, ClaimPaymentValues.StatusDenied) with
                {
                    PaidAmount = 1
                })));
            Assert.IsType<ArgumentException>(await Record.ExceptionAsync(() => fixture.Payments.UpdateAsync(
                created.Id,
                created.Revision,
                ResultDraft(submission.Id, ClaimPaymentValues.StatusCancelled) with
                {
                    AdditionalDocumentsMemo = "not allowed"
                })));

            Assert.Equal(before, await File.ReadAllBytesAsync(fixture.PaymentPath));
            Assert.Equal(created.Revision, (await fixture.Payments.GetAsync(created.Id))!.Revision);
        });
    }

    [Fact]
    public async Task Strings_are_trimmed_and_blank_optional_values_normalize_to_null()
    {
        await UsingFixtureAsync(async fixture =>
        {
            var submission = await fixture.CreateSubmissionAsync(ClaimSubmissionValues.StatusSubmitted);

            var record = await fixture.Payments.CreateAsync(PendingDraft($"  {submission.Id}  ") with
            {
                PaidCoverageDisplayName = " coverage ",
                DenyReason = " ",
                ReductionReason = " reduction ",
                AdditionalDocumentsMemo = " documents ",
                Memo = " memo "
            });

            Assert.Equal(submission.Id, record.ClaimSubmissionId);
            Assert.Equal("coverage", record.PaidCoverageDisplayName);
            Assert.Null(record.DenyReason);
            Assert.Equal("reduction", record.ReductionReason);
            Assert.Equal("documents", record.AdditionalDocumentsMemo);
            Assert.Equal("memo", record.Memo);
        });
    }

    [Theory]
    [InlineData(ClaimSubmissionValues.StatusPreparing)]
    [InlineData(ClaimSubmissionValues.StatusCancelled)]
    public async Task Preparing_or_cancelled_submission_rejects_payment_create_without_write(
        string submissionStatus)
    {
        await UsingFixtureAsync(async fixture =>
        {
            var submission = await fixture.CreateSubmissionAsync(submissionStatus);

            var exception = await Record.ExceptionAsync(() =>
                fixture.Payments.CreateAsync(PendingDraft(submission.Id)));

            Assert.IsType<ClaimPaymentReferenceException>(exception);
            Assert.False(File.Exists(fixture.PaymentPath));
            Assert.False(File.Exists(fixture.PaymentPath + ".bak"));
            Assert.Empty(fixture.PaymentTempFiles());
        });
    }

    [Fact]
    public async Task Missing_submission_rejects_create_without_write()
    {
        await UsingFixtureAsync(async fixture =>
        {
            Assert.IsType<ClaimPaymentReferenceException>(await Record.ExceptionAsync(() =>
                fixture.Payments.CreateAsync(PendingDraft("submission_missing"))));
            Assert.False(File.Exists(fixture.PaymentPath));
        });
    }

    [Theory]
    [InlineData(ClaimPaymentValues.StatusPaid)]
    [InlineData(ClaimPaymentValues.StatusPartiallyPaid)]
    [InlineData(ClaimPaymentValues.StatusDenied)]
    public async Task Result_finalization_requires_completed_submission(string resultStatus)
    {
        await UsingFixtureAsync(async fixture =>
        {
            var submission = await fixture.CreateSubmissionAsync(ClaimSubmissionValues.StatusReviewing);
            var payment = await fixture.Payments.CreateAsync(PendingDraft(submission.Id));
            var before = await File.ReadAllBytesAsync(fixture.PaymentPath);

            var exception = await Record.ExceptionAsync(() => fixture.Payments.UpdateAsync(
                payment.Id,
                payment.Revision,
                ResultDraft(submission.Id, resultStatus)));

            Assert.IsType<ClaimPaymentReferenceException>(exception);
            Assert.Equal(before, await File.ReadAllBytesAsync(fixture.PaymentPath));
            Assert.Equal(payment.Revision, (await fixture.Payments.GetAsync(payment.Id))!.Revision);
        });
    }

    [Fact]
    public async Task Disabled_claim_or_policy_blocks_existing_payment_mutation_without_write()
    {
        await UsingFixtureAsync(async fixture =>
        {
            var context = await fixture.CreateSubmissionContextAsync(ClaimSubmissionValues.StatusSubmitted);
            var payment = await fixture.Payments.CreateAsync(PendingDraft(context.Submission.Id));
            _ = await fixture.PolicyClaims.DisableClaimCaseAsync(
                context.Claim.Id,
                context.Claim.Revision);
            var before = await File.ReadAllBytesAsync(fixture.PaymentPath);

            Assert.IsType<ClaimPaymentReferenceException>(await Record.ExceptionAsync(() =>
                fixture.Payments.UpdateAsync(
                    payment.Id,
                    payment.Revision,
                    PendingDraft(context.Submission.Id) with { Memo = "blocked" })));
            Assert.Equal(before, await File.ReadAllBytesAsync(fixture.PaymentPath));

            var secondContext = await fixture.CreateSubmissionContextAsync(
                ClaimSubmissionValues.StatusSubmitted,
                "second");
            var secondPayment = await fixture.Payments.CreateAsync(
                PendingDraft(secondContext.Submission.Id));
            _ = await fixture.PolicyClaims.DisablePolicyAsync(secondContext.Policy.Id);
            var secondBefore = await File.ReadAllBytesAsync(fixture.PaymentPath);

            Assert.IsType<ClaimPaymentReferenceException>(await Record.ExceptionAsync(() =>
                fixture.Payments.UpdateAsync(
                    secondPayment.Id,
                    secondPayment.Revision,
                    PendingDraft(secondContext.Submission.Id) with { Memo = "blocked" })));
            Assert.Equal(secondBefore, await File.ReadAllBytesAsync(fixture.PaymentPath));
            Assert.Empty(fixture.PaymentTempFiles());
        });
    }

    [Fact]
    public async Task Unknown_submission_status_blocks_existing_payment_mutation_without_write()
    {
        await UsingFixtureAsync(async fixture =>
        {
            var context = await fixture.CreateSubmissionContextAsync(ClaimSubmissionValues.StatusSubmitted);
            var payment = await fixture.Payments.CreateAsync(PendingDraft(context.Submission.Id));
            payment = await fixture.Payments.UpdateAsync(
                payment.Id,
                payment.Revision,
                PendingDraft(context.Submission.Id) with { Memo = "baseline" });
            var before = await File.ReadAllBytesAsync(fixture.PaymentPath);
            var backupBefore = await File.ReadAllBytesAsync(fixture.PaymentPath + ".bak");
            var storage = new JsonClaimPaymentStorageService(
                fixture.RootPath,
                new FixedClaimSubmissionStorage(context.Submission with { Status = "unknown" }),
                fixture.PolicyClaims,
                fixture.PolicyClaims);

            var exception = await Record.ExceptionAsync(() => storage.UpdateAsync(
                payment.Id,
                payment.Revision,
                PendingDraft(context.Submission.Id) with { Memo = "blocked" }));

            Assert.IsType<ClaimPaymentReferenceException>(exception);
            Assert.Equal(before, await File.ReadAllBytesAsync(fixture.PaymentPath));
            Assert.Equal(backupBefore, await File.ReadAllBytesAsync(fixture.PaymentPath + ".bak"));
            Assert.Equal(payment.Revision, (await fixture.Payments.GetAsync(payment.Id))!.Revision);
            Assert.Empty(fixture.PaymentTempFiles());
        });
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Other_family_or_legacy_policy_owner_blocks_mutation_without_write(
        bool legacyOwner)
    {
        await UsingFixtureAsync(async fixture =>
        {
            var context = await fixture.CreateSubmissionContextAsync(ClaimSubmissionValues.StatusSubmitted);
            var payment = await fixture.Payments.CreateAsync(PendingDraft(context.Submission.Id));
            payment = await fixture.Payments.UpdateAsync(
                payment.Id,
                payment.Revision,
                PendingDraft(context.Submission.Id) with { Memo = "baseline" });
            var before = await File.ReadAllBytesAsync(fixture.PaymentPath);
            var backupBefore = await File.ReadAllBytesAsync(fixture.PaymentPath + ".bak");
            var policy = context.Policy with
            {
                FamilyMemberId = legacyOwner ? null : "family_other"
            };
            var storage = new JsonClaimPaymentStorageService(
                fixture.RootPath,
                fixture.Submissions,
                fixture.PolicyClaims,
                new FixedPolicyClaimStorage(policy));

            var exception = await Record.ExceptionAsync(() => storage.UpdateAsync(
                payment.Id,
                payment.Revision,
                PendingDraft(context.Submission.Id) with { Memo = "blocked" }));

            if (legacyOwner)
            {
                Assert.IsType<ClaimPaymentLegacyReviewRequiredException>(exception);
            }
            else
            {
                Assert.IsType<ClaimPaymentReferenceException>(exception);
            }

            Assert.Equal(before, await File.ReadAllBytesAsync(fixture.PaymentPath));
            Assert.Equal(backupBefore, await File.ReadAllBytesAsync(fixture.PaymentPath + ".bak"));
            Assert.Equal(payment.Revision, (await fixture.Payments.GetAsync(payment.Id))!.Revision);
            Assert.Empty(fixture.PaymentTempFiles());
        });
    }

    [Fact]
    public async Task Legacy_claim_owner_returns_dedicated_error_without_payment_write()
    {
        await UsingFixtureAsync(async fixture =>
        {
            var context = await fixture.CreateSubmissionContextAsync(ClaimSubmissionValues.StatusSubmitted);
            var unresolved = context.Claim with { FamilyMemberId = null };
            var storage = new JsonClaimPaymentStorageService(
                fixture.RootPath,
                fixture.Submissions,
                new FixedClaimCaseStorage(unresolved),
                fixture.PolicyClaims);

            var exception = await Record.ExceptionAsync(() =>
                storage.CreateAsync(PendingDraft(context.Submission.Id)));

            Assert.IsType<ClaimPaymentLegacyReviewRequiredException>(exception);
            Assert.False(File.Exists(fixture.PaymentPath));
            Assert.Empty(fixture.PaymentTempFiles());
        });
    }

    [Fact]
    public async Task History_reader_returns_all_raw_payments_without_modifying_file()
    {
        await UsingFixtureAsync(async fixture =>
        {
            var submission = await fixture.CreateSubmissionAsync(ClaimSubmissionValues.StatusSubmitted);
            _ = await fixture.Payments.CreateAsync(PendingDraft(submission.Id));
            var envelope = JsonNode.Parse(await File.ReadAllTextAsync(fixture.PaymentPath))!
                .AsObject();
            var items = envelope["items"]!.AsArray();
            var orphan = items[0]!.DeepClone().AsObject();
            orphan["id"] = "payment_orphan";
            orphan["claimSubmissionId"] = "submission_missing";
            orphan["status"] = "unexpected_payment_status";
            items.Add(orphan);
            await File.WriteAllTextAsync(
                fixture.PaymentPath,
                envelope.ToJsonString(),
                new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
            var before = await File.ReadAllBytesAsync(fixture.PaymentPath);

            var records = await ((IClaimPaymentHistoryStorageReader)fixture.Payments)
                .GetAllPaymentsForHistoryAsync();

            Assert.Equal(2, records.Count);
            Assert.Contains(records, record => record.Id == "payment_orphan"
                && record.ClaimSubmissionId == "submission_missing"
                && record.Status == "unexpected_payment_status");
            Assert.Equal(before, await File.ReadAllBytesAsync(fixture.PaymentPath));
        });
    }

    [Fact]
    public async Task Malformed_and_unsupported_envelopes_fail_closed()
    {
        await UsingFixtureAsync(async fixture =>
        {
            await File.WriteAllTextAsync(fixture.PaymentPath, "{ invalid json", Encoding.UTF8);
            Assert.IsType<InvalidOperationException>(await Record.ExceptionAsync(() =>
                fixture.Payments.GetAsync("payment_any")));

            await File.WriteAllTextAsync(
                fixture.PaymentPath,
                """
                {
                  "schemaVersion": 2,
                  "savedAt": "2026-08-10T00:00:00Z",
                  "items": []
                }
                """,
                Encoding.UTF8);
            Assert.IsType<InvalidOperationException>(await Record.ExceptionAsync(() =>
                fixture.Payments.GetAsync("payment_any")));
        });
    }

    [Fact]
    public async Task Locked_replace_preserves_original_backup_revision_and_cleans_temp()
    {
        await UsingFixtureAsync(async fixture =>
        {
            var submission = await fixture.CreateSubmissionAsync(ClaimSubmissionValues.StatusSubmitted);
            var created = await fixture.Payments.CreateAsync(PendingDraft(submission.Id));
            var updated = await fixture.Payments.UpdateAsync(
                created.Id,
                created.Revision,
                PendingDraft(submission.Id) with { Memo = "first" });
            var before = await File.ReadAllBytesAsync(fixture.PaymentPath);
            var backupBefore = await File.ReadAllBytesAsync(fixture.PaymentPath + ".bak");

            Exception? exception;
            using (new FileStream(
                fixture.PaymentPath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read))
            {
                exception = await Record.ExceptionAsync(() => fixture.Payments.UpdateAsync(
                    updated.Id,
                    updated.Revision,
                    PendingDraft(submission.Id) with { Memo = "blocked" }));
            }

            Assert.NotNull(exception);
            Assert.Equal(before, await File.ReadAllBytesAsync(fixture.PaymentPath));
            Assert.Equal(backupBefore, await File.ReadAllBytesAsync(fixture.PaymentPath + ".bak"));
            Assert.Equal(updated.Revision, (await fixture.Payments.GetAsync(updated.Id))!.Revision);
            Assert.Empty(fixture.PaymentTempFiles());
        });
    }

    [Fact]
    public async Task Payment_mutations_leave_claim_policy_document_and_submission_files_unchanged()
    {
        await UsingFixtureAsync(async fixture =>
        {
            var context = await fixture.CreateSubmissionContextAsync(ClaimSubmissionValues.StatusCompleted);
            _ = await fixture.Documents.AddDocumentAsync(new DocumentDraft(
                "synthetic.pdf",
                "synthetic document",
                "pdf",
                "synthetic/synthetic.pdf"));
            var protectedPaths = new[]
            {
                Path.Combine(fixture.RootPath, "claims.json"),
                Path.Combine(fixture.RootPath, "policies.json"),
                Path.Combine(fixture.RootPath, "documents.json"),
                fixture.SubmissionPath
            };
            var before = protectedPaths.ToDictionary(
                path => path,
                File.ReadAllBytes,
                StringComparer.OrdinalIgnoreCase);

            var payment = await fixture.Payments.CreateAsync(PendingDraft(context.Submission.Id));
            _ = await fixture.Payments.UpdateAsync(
                payment.Id,
                payment.Revision,
                ResultDraft(context.Submission.Id, ClaimPaymentValues.StatusPaid));

            foreach (var path in protectedPaths)
            {
                Assert.Equal(before[path], await File.ReadAllBytesAsync(path));
            }
        });
    }

    private static ClaimPaymentDraft PendingDraft(string submissionId)
    {
        return new ClaimPaymentDraft(
            submissionId,
            ClaimPaymentValues.StatusPending,
            PaidDate: null,
            PaidAmount: null,
            PaidCoverageDisplayName: null,
            DenyReason: null,
            ReductionReason: null,
            AdditionalDocumentsMemo: null,
            Memo: null);
    }

    private static ClaimPaymentDraft ResultDraft(string submissionId, string status)
    {
        return status switch
        {
            ClaimPaymentValues.StatusPaid => PendingDraft(submissionId) with
            {
                Status = status,
                PaidDate = new DateOnly(2026, 8, 10),
                PaidAmount = 100_000,
                PaidCoverageDisplayName = "medical coverage"
            },
            ClaimPaymentValues.StatusPartiallyPaid => PendingDraft(submissionId) with
            {
                Status = status,
                PaidDate = new DateOnly(2026, 8, 10),
                PaidAmount = 60_000,
                PaidCoverageDisplayName = "medical coverage",
                ReductionReason = "contract limit"
            },
            ClaimPaymentValues.StatusDenied => PendingDraft(submissionId) with
            {
                Status = status,
                DenyReason = "not covered"
            },
            ClaimPaymentValues.StatusCancelled => PendingDraft(submissionId) with
            {
                Status = status
            },
            _ => throw new ArgumentOutOfRangeException(nameof(status))
        };
    }

    private static async Task<(ClaimPaymentRecord? Record, Exception? Exception)> CaptureAsync(
        Func<Task<ClaimPaymentRecord>> action)
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
                nameof(ClaimPaymentStorageServiceTests),
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
            Payments = CreatePaymentStorage();
        }

        public string RootPath { get; }

        public string PaymentPath => Path.Combine(RootPath, "claim-payments.json");

        public string SubmissionPath => Path.Combine(RootPath, "claim-submissions.json");

        public JsonFamilyMemberStorageService Families { get; }

        public JsonPolicyClaimStorageService PolicyClaims { get; }

        public JsonDocumentStorageService Documents { get; }

        public JsonClaimSubmissionStorageService Submissions { get; }

        public JsonClaimPaymentStorageService Payments { get; }

        public JsonClaimPaymentStorageService CreatePaymentStorage()
        {
            return new JsonClaimPaymentStorageService(
                RootPath,
                Submissions,
                PolicyClaims,
                PolicyClaims);
        }

        public string[] PaymentTempFiles()
        {
            return Directory.GetFiles(RootPath, "claim-payments.json.*.tmp");
        }

        public async Task<ClaimSubmissionRecord> CreateSubmissionAsync(string status)
        {
            return (await CreateSubmissionContextAsync(status)).Submission;
        }

        public async Task<(FamilyMemberRecord Family, PolicyRecord Policy, ClaimRecord Claim, ClaimSubmissionRecord Submission)>
            CreateSubmissionContextAsync(string status, string suffix = "primary")
        {
            var family = await Families.CreateFamilyMemberAsync(new FamilyMemberDraft(
                $"synthetic family {suffix}",
                FamilyMemberRelationValues.Self,
                null));
            var policy = await PolicyClaims.CreateInsurancePolicyAsync(new InsurancePolicyDraft(
                $"synthetic policy {suffix}",
                family.Id,
                "synthetic insurer",
                InsurancePolicyValues.ContractStatusActive,
                new DateOnly(2026, 8, 1),
                "synthetic coverage",
                "20 years",
                12_000_000m,
                InsurancePolicyValues.RenewalTypeFixed,
                InsurancePolicyValues.RefundTypeRefundable,
                InsurancePolicyValues.BusinessTypeLife,
                InsurancePolicyValues.ProductCategoryMedicalExpense));
            var claimDraft = new ClaimCaseDraft(
                $"synthetic claim {suffix}",
                family.Id,
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
                Memo: null);
            var claimDraftRecord = await PolicyClaims.CreateClaimCaseAsync(claimDraft);
            var claim = await PolicyClaims.UpdateClaimCaseAsync(
                claimDraftRecord.Id,
                claimDraftRecord.Revision,
                claimDraft);
            var submission = await Submissions.CreateAsync(new ClaimSubmissionDraft(
                claim.Id,
                policy.Id,
                PolicyCoverageId: null,
                CoverageDisplayName: null,
                SubmittedDate: null,
                SubmittedAmount: null,
                SubmittedClaimDocumentIds: [],
                ClaimSubmissionValues.StatusPreparing,
                Memo: null));

            if (!string.Equals(status, ClaimSubmissionValues.StatusPreparing, StringComparison.Ordinal))
            {
                submission = await Submissions.UpdateAsync(
                    submission.Id,
                    submission.Revision,
                    SubmissionDraft(claim.Id, policy.Id, ClaimSubmissionValues.StatusSubmitted));
            }

            if (string.Equals(status, ClaimSubmissionValues.StatusAdditionalDocumentsRequested, StringComparison.Ordinal)
                || string.Equals(status, ClaimSubmissionValues.StatusReviewing, StringComparison.Ordinal)
                || string.Equals(status, ClaimSubmissionValues.StatusCompleted, StringComparison.Ordinal))
            {
                submission = await Submissions.UpdateAsync(
                    submission.Id,
                    submission.Revision,
                    SubmissionDraft(claim.Id, policy.Id, ClaimSubmissionValues.StatusReviewing));
            }

            if (string.Equals(status, ClaimSubmissionValues.StatusAdditionalDocumentsRequested, StringComparison.Ordinal))
            {
                submission = await Submissions.UpdateAsync(
                    submission.Id,
                    submission.Revision,
                    SubmissionDraft(
                        claim.Id,
                        policy.Id,
                        ClaimSubmissionValues.StatusAdditionalDocumentsRequested));
            }
            else if (string.Equals(status, ClaimSubmissionValues.StatusCompleted, StringComparison.Ordinal))
            {
                submission = await Submissions.UpdateAsync(
                    submission.Id,
                    submission.Revision,
                    SubmissionDraft(claim.Id, policy.Id, ClaimSubmissionValues.StatusCompleted));
            }
            else if (string.Equals(status, ClaimSubmissionValues.StatusCancelled, StringComparison.Ordinal))
            {
                submission = await Submissions.UpdateAsync(
                    submission.Id,
                    submission.Revision,
                    SubmissionDraft(claim.Id, policy.Id, ClaimSubmissionValues.StatusCancelled));
            }

            return (family, policy, claim, submission);
        }

        public ValueTask DisposeAsync()
        {
            if (Directory.Exists(RootPath))
            {
                Directory.Delete(RootPath, recursive: true);
            }

            return ValueTask.CompletedTask;
        }

        private static ClaimSubmissionDraft SubmissionDraft(
            string claimId,
            string policyId,
            string status)
        {
            return new ClaimSubmissionDraft(
                claimId,
                policyId,
                PolicyCoverageId: null,
                CoverageDisplayName: "synthetic coverage",
                SubmittedDate: new DateOnly(2026, 8, 8),
                SubmittedAmount: null,
                SubmittedClaimDocumentIds: [],
                status,
                Memo: null);
        }
    }

    private sealed class FixedClaimSubmissionStorage(ClaimSubmissionRecord record)
        : IClaimSubmissionStorageService
    {
        public Task<IReadOnlyList<ClaimSubmissionRecord>> GetByClaimCaseAsync(
            string claimCaseId,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<ClaimSubmissionRecord>>([record]);

        public Task<ClaimSubmissionRecord?> GetAsync(
            string id,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<ClaimSubmissionRecord?>(record);

        public Task<IReadOnlyList<PolicyRecord>> GetClaimablePoliciesAsync(
            string claimCaseId,
            CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<ClaimSubmissionRecord> CreateAsync(
            ClaimSubmissionDraft draft,
            CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<ClaimSubmissionRecord> UpdateAsync(
            string id,
            int expectedRevision,
            ClaimSubmissionDraft draft,
            CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();
    }

    private sealed class FixedPolicyClaimStorage(PolicyRecord record)
        : IPolicyClaimStorageService
    {
        public Task<IReadOnlyList<PolicyRecord>> GetPoliciesAsync(
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<PolicyRecord>>([record]);

        public Task<PolicyRecord?> GetPolicyAsync(
            string id,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<PolicyRecord?>(record);

        public Task<PolicyRecord> AddPolicyAsync(
            PolicyDraft draft,
            CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<PolicyRecord> CreateInsurancePolicyAsync(
            InsurancePolicyDraft draft,
            CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<PolicyRecord> UpdateInsurancePolicyAsync(
            string id,
            InsurancePolicyDraft draft,
            CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<PolicyRecord> DisablePolicyAsync(
            string id,
            CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<IReadOnlyList<ClaimRecord>> GetClaimsAsync(
            CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<IReadOnlyList<ClaimRecord>> GetClaimsByPolicyIdAsync(
            string policyId,
            CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<ClaimRecord?> GetClaimAsync(
            string id,
            CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<ClaimRecord> AddClaimAsync(
            ClaimDraft draft,
            CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<ClaimRecord> DisableClaimAsync(
            string id,
            int expectedRevision,
            CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<bool> PolicyExistsAsync(
            string id,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(string.Equals(id, record.Id, StringComparison.Ordinal));

        public Task<bool> ClaimExistsAsync(
            string id,
            CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();
    }

    private sealed class FixedClaimCaseStorage(ClaimRecord record) : IClaimCaseStorageService
    {
        public Task<IReadOnlyList<ClaimRecord>> GetClaimCasesAsync(
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IReadOnlyList<ClaimRecord>>([record]);
        }

        public Task<ClaimRecord?> GetClaimCaseAsync(
            string id,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult<ClaimRecord?>(record);
        }

        public Task<ClaimRecord> CreateClaimCaseAsync(
            ClaimCaseDraft draft,
            CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public Task<ClaimRecord> UpdateClaimCaseAsync(
            string id,
            int expectedRevision,
            ClaimCaseDraft draft,
            CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public Task<ClaimRecord> DisableClaimCaseAsync(
            string id,
            int expectedRevision,
            CancellationToken cancellationToken = default) => throw new NotSupportedException();
    }
}
