using System.Globalization;
using FamilyClaimRef.App.Models.Matching;
using FamilyClaimRef.App.Models.Storage;
using FamilyClaimRef.App.Services.Matching;
using Xunit;

namespace FamilyClaimRef.App.Tests;

public sealed class ClaimReferenceMatchingEngineTests
{
    private static readonly DateTimeOffset BaseTime =
        new(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);

    private readonly ClaimReferenceMatchingEngine engine = new();

    [Fact]
    public void TestPcs002_Selected_claim_must_be_saved_active_and_owned_by_an_active_family()
    {
        var baseline = CreateRequest();

        AssertSelectedClaimFailure(baseline with
        {
            ClaimCases = [baseline.ClaimCases![0] with { CaseStatus = ClaimCaseValues.StatusDraft }]
        });
        AssertSelectedClaimFailure(baseline with
        {
            ClaimCases = [baseline.ClaimCases![0] with
            {
                DisabledAt = BaseTime.AddDays(2),
                UpdatedAt = BaseTime.AddDays(2)
            }]
        });
        AssertSelectedClaimFailure(baseline with
        {
            FamilyMembers = [baseline.FamilyMembers![0] with
            {
                DisabledAt = BaseTime.AddDays(2),
                UpdatedAt = BaseTime.AddDays(2)
            }]
        });
        AssertMatchingFailure(baseline with { FamilyMembers = [] });
        AssertMatchingFailure(baseline with
        {
            ClaimCases = [baseline.ClaimCases![0] with { VisitType = "unknown" }]
        });
    }

    [Fact]
    public void TestPcs003_004_Valid_unrelated_family_is_excluded_after_global_validation()
    {
        var request = CreateRequest();
        var otherFamily = CreateFamily("family-b", "Synthetic B");
        var otherPolicy = CreatePolicy("policy-b", otherFamily.Id, "Policy B");
        var otherCoverage = CreateCoverage("coverage-b", otherPolicy.Id, "Coverage B");

        var result = engine.BuildProjection(request with
        {
            FamilyMembers = [.. request.FamilyMembers!, otherFamily],
            Policies = [.. request.Policies!, otherPolicy],
            PolicyCoverages = [.. request.PolicyCoverages!, otherCoverage]
        });

        var coverage = Assert.Single(result.CoverageResults);
        Assert.Equal("coverage-a", coverage.PolicyCoverageId);
    }

    [Fact]
    public void TestPcs003_027_Corruption_outside_selected_family_fails_the_whole_graph()
    {
        var request = CreateRequest();
        var invalidPolicy = CreatePolicy("policy-b", "family-missing", "Policy B");

        AssertMatchingFailure(request with { Policies = [.. request.Policies!, invalidPolicy] });
    }

    [Fact]
    public void TestPcs005_009_Only_active_user_confirmed_coverages_produce_results()
    {
        var request = CreateRequest();
        var coverages = new[]
        {
            CreateCoverage("coverage-confirmed", "policy-a", "Confirmed"),
            CreateCoverage("coverage-candidate", "policy-a", "Candidate") with
            {
                ReviewStatus = PolicyCoverageValues.ReviewStatusCandidate
            },
            CreateCoverage("coverage-review", "policy-a", "Review") with
            {
                ReviewStatus = PolicyCoverageValues.ReviewStatusNeedsReview
            },
            CreateCoverage("coverage-ignored", "policy-a", "Ignored") with
            {
                ReviewStatus = PolicyCoverageValues.ReviewStatusIgnored
            },
            CreateCoverage("coverage-disabled", "policy-a", "Disabled") with
            {
                DisabledAt = BaseTime.AddDays(2),
                UpdatedAt = BaseTime.AddDays(2)
            }
        };

        var result = engine.BuildProjection(request with { PolicyCoverages = coverages });

        Assert.Equal("coverage-confirmed", Assert.Single(result.CoverageResults).PolicyCoverageId);
        Assert.True(result.HasExcludedUnconfirmedCoverages);

        var ignoredOnly = engine.BuildProjection(request with
        {
            PolicyCoverages = [coverages[3]]
        });
        Assert.Empty(ignoredOnly.CoverageResults);
        Assert.False(ignoredOnly.HasExcludedUnconfirmedCoverages);
    }

    [Fact]
    public void TestPcs010_Eligible_policy_statuses_are_included_and_other_policies_are_excluded()
    {
        var request = CreateRequest();
        var otherFamily = CreateFamily("family-b", "Synthetic B");
        var policies = new[]
        {
            CreatePolicy("policy-active", "family-a", "Active", InsurancePolicyValues.ContractStatusActive),
            CreatePolicy("policy-waived", "family-a", "Waived", InsurancePolicyValues.ContractStatusPremiumWaived),
            CreatePolicy("policy-legacy", "family-a", "Legacy", InsurancePolicyValues.LegacyContractStatusActive),
            CreatePolicy("policy-expired", "family-a", "Expired", InsurancePolicyValues.ContractStatusExpired),
            CreatePolicy("policy-disabled", "family-a", "Disabled") with
            {
                DisabledAt = BaseTime.AddDays(2),
                UpdatedAt = BaseTime.AddDays(2)
            },
            CreatePolicy("policy-other", "family-b", "Other")
        };
        var coverages = policies.Select(policy =>
            CreateCoverage($"coverage-{policy.Id}", policy.Id, policy.DisplayTitle)).ToArray();

        var result = engine.BuildProjection(request with
        {
            FamilyMembers = [.. request.FamilyMembers!, otherFamily],
            Policies = policies,
            PolicyCoverages = coverages
        });

        Assert.Equal(
            ["policy-active", "policy-legacy", "policy-waived"],
            result.CoverageResults.Select(item => item.PolicyId).Order(StringComparer.Ordinal));
    }

    [Fact]
    public void TestPcs011_013_Treatment_date_uses_enrollment_and_inclusive_effective_bounds()
    {
        var request = CreateRequest();
        var claim = request.ClaimCases![0] with { ReferenceDate = new DateOnly(2026, 2, 10) };
        var inclusive = request.PolicyCoverages![0] with
        {
            PolicyCoverageId = "coverage-inclusive",
            EffectiveFrom = claim.ReferenceDate,
            EffectiveTo = claim.ReferenceDate
        };
        var beforeEnrollment = request.Policies![0] with
        {
            Id = "policy-future",
            DisplayTitle = "Future",
            EnrollmentDate = claim.ReferenceDate.AddDays(1)
        };
        var beforeEnrollmentCoverage = CreateCoverage(
            "coverage-future",
            beforeEnrollment.Id,
            "Future Coverage") with
        {
            EffectiveFrom = null,
            EffectiveTo = null
        };

        var result = engine.BuildProjection(request with
        {
            Policies = [request.Policies[0], beforeEnrollment],
            PolicyCoverages = [inclusive, beforeEnrollmentCoverage],
            ClaimCases = [claim]
        });

        Assert.Equal(
            ClaimReferenceMatchingValues.OutcomePassed,
            Evidence(result, "coverage-inclusive", ClaimReferenceMatchingValues.RuleTreatmentDate));
        Assert.Equal(
            ClaimReferenceMatchingValues.OutcomeMismatch,
            Evidence(result, "coverage-future", ClaimReferenceMatchingValues.RuleTreatmentDate));
    }

    [Theory]
    [InlineData("2026-02-11", "2026-12-31")]
    [InlineData("2025-01-01", "2026-02-09")]
    public void TestPcs012_Treatment_date_outside_effective_range_is_a_mismatch(
        string effectiveFrom,
        string effectiveTo)
    {
        var request = CreateRequest();
        var coverage = request.PolicyCoverages![0] with
        {
            EffectiveFrom = DateOnly.ParseExact(effectiveFrom, "yyyy-MM-dd", CultureInfo.InvariantCulture),
            EffectiveTo = DateOnly.ParseExact(effectiveTo, "yyyy-MM-dd", CultureInfo.InvariantCulture)
        };

        var result = engine.BuildProjection(request with { PolicyCoverages = [coverage] });

        Assert.Equal(
            ClaimReferenceMatchingValues.OutcomeMismatch,
            Evidence(result, coverage.PolicyCoverageId, ClaimReferenceMatchingValues.RuleTreatmentDate));
    }

    [Fact]
    public void TestPcs013_Free_text_coverage_period_is_ignored_and_unknown_lower_bound_needs_confirmation()
    {
        var request = CreateRequest();
        var policy = request.Policies![0] with
        {
            EnrollmentDate = null,
            CoveragePeriod = "synthetic text that must not be parsed"
        };
        var coverage = request.PolicyCoverages![0] with
        {
            EffectiveFrom = null,
            EffectiveTo = null
        };

        var result = engine.BuildProjection(request with
        {
            Policies = [policy],
            PolicyCoverages = [coverage]
        });

        Assert.Equal(
            ClaimReferenceMatchingValues.OutcomeNeedsConfirmation,
            Evidence(result, coverage.PolicyCoverageId, ClaimReferenceMatchingValues.RuleTreatmentDate));
        Assert.Equal(
            ClaimReferenceMatchingValues.ResultGroupNeedsConfirmation,
            Assert.Single(result.CoverageResults).ResultGroup);
    }

    [Theory]
    [InlineData("any", "outpatient", "passed")]
    [InlineData("outpatient", "outpatient", "passed")]
    [InlineData("inpatient", "outpatient", "mismatch")]
    public void TestPcs014_Visit_type_rule_is_exact_and_ordinal(
        string rule,
        string visitType,
        string expected)
    {
        var request = CreateRequest();
        var coverage = request.PolicyCoverages![0] with { VisitTypeRule = rule };
        var claim = request.ClaimCases![0] with { VisitType = visitType };

        var result = engine.BuildProjection(request with
        {
            PolicyCoverages = [coverage],
            ClaimCases = [claim]
        });

        Assert.Equal(expected, Evidence(
            result,
            coverage.PolicyCoverageId,
            ClaimReferenceMatchingValues.RuleVisitType));
    }

    [Theory]
    [InlineData("any", false, "passed")]
    [InlineData("required", true, "passed")]
    [InlineData("required", false, "mismatch")]
    [InlineData("excluded", false, "passed")]
    [InlineData("excluded", true, "mismatch")]
    public void TestPcs015_Surgery_rule_uses_only_the_explicit_boolean(
        string rule,
        bool actual,
        string expected)
    {
        AssertBooleanRule(
            coverage => coverage with { SurgeryRule = rule },
            claim => claim with { HasSurgery = actual, CoveredAmount = 999_999 },
            ClaimReferenceMatchingValues.RuleSurgery,
            expected);
    }

    [Theory]
    [InlineData("any", false, "passed")]
    [InlineData("required", true, "passed")]
    [InlineData("required", false, "mismatch")]
    [InlineData("excluded", false, "passed")]
    [InlineData("excluded", true, "mismatch")]
    public void TestPcs016_Prescription_rule_uses_only_the_explicit_boolean(
        string rule,
        bool actual,
        string expected)
    {
        AssertBooleanRule(
            coverage => coverage with { PrescriptionRule = rule },
            claim => claim with { HasPrescription = actual, PrescriptionAmount = 999_999 },
            ClaimReferenceMatchingValues.RulePrescription,
            expected);
    }

    [Fact]
    public void TestPcs017_019_Diagnosis_rules_normalize_case_and_preserve_punctuation()
    {
        var request = CreateRequest();
        var any = request.PolicyCoverages![0] with
        {
            PolicyCoverageId = "coverage-any",
            DiagnosisRuleMode = PolicyCoverageValues.DiagnosisRuleAny,
            DiagnosisCodePrefixes = []
        };
        var prefix = request.PolicyCoverages[0] with
        {
            PolicyCoverageId = "coverage-prefix",
            DiagnosisRuleMode = PolicyCoverageValues.DiagnosisRulePrefixList,
            DiagnosisCodePrefixes = ["A00.1-Z"]
        };
        var mismatch = prefix with
        {
            PolicyCoverageId = "coverage-mismatch",
            DiagnosisCodePrefixes = ["A001Z"]
        };
        var claim = request.ClaimCases![0] with { DiagnosisCode = "  a00.1-z9  " };

        var result = engine.BuildProjection(request with
        {
            PolicyCoverages = [any, prefix, mismatch],
            ClaimCases = [claim]
        });

        Assert.Equal("passed", Evidence(result, any.PolicyCoverageId, "diagnosis_code"));
        Assert.Equal("passed", Evidence(result, prefix.PolicyCoverageId, "diagnosis_code"));
        Assert.Equal("mismatch", Evidence(result, mismatch.PolicyCoverageId, "diagnosis_code"));

        var noDiagnosis = engine.BuildProjection(request with
        {
            PolicyCoverages = [prefix],
            ClaimCases = [request.ClaimCases[0] with { DiagnosisCode = null }]
        });
        Assert.Equal("needs_confirmation", Evidence(
            noDiagnosis,
            prefix.PolicyCoverageId,
            "diagnosis_code"));
    }

    [Fact]
    public void TestPcs020_021_Mismatch_precedes_confirmation_and_score_excludes_source_document()
    {
        var request = CreateRequest();
        var document = CreatePolicyDocument("pdoc-a", "policy-a");
        var coverage = request.PolicyCoverages![0] with
        {
            EffectiveFrom = null,
            EffectiveTo = null,
            VisitTypeRule = PolicyCoverageValues.VisitTypeInpatient,
            SourceKind = PolicyCoverageValues.SourcePolicyDocument,
            SourcePolicyDocumentId = document.Id
        };
        var policy = request.Policies![0] with { EnrollmentDate = null };

        var result = engine.BuildProjection(request with
        {
            Policies = [policy],
            PolicyCoverages = [coverage],
            PolicyDocuments = [document]
        });
        var item = Assert.Single(result.CoverageResults);

        Assert.Equal(ClaimReferenceMatchingValues.ResultGroupCurrentInputMismatch, item.ResultGroup);
        Assert.Equal(4, item.PassedRuleCount);
        Assert.True(item.HasSourcePolicyDocument);
        Assert.Equal("passed", Evidence(result, coverage.PolicyCoverageId, "source_document"));

        var allPass = engine.BuildProjection(request);
        var allPassItem = Assert.Single(allPass.CoverageResults);
        Assert.Equal(ClaimReferenceMatchingValues.ResultGroupConditionMatch, allPassItem.ResultGroup);
        Assert.Equal(6, allPassItem.PassedRuleCount);
    }

    [Fact]
    public void TestPcs021_Result_order_is_group_score_display_name_and_ordinal_id()
    {
        var request = CreateRequest();
        var policies = new[]
        {
            CreatePolicy("policy-z", "family-a", "Same"),
            CreatePolicy("policy-a", "family-a", "Same")
        };
        var conditionZ = CreateCoverage("coverage-z", "policy-z", "Same");
        var conditionA = CreateCoverage("coverage-a", "policy-a", "Same");
        var needs = CreateCoverage("coverage-needs", "policy-a", "Needs") with
        {
            EffectiveFrom = null,
            EffectiveTo = null
        };
        var mismatch = CreateCoverage("coverage-mismatch", "policy-a", "Mismatch") with
        {
            VisitTypeRule = PolicyCoverageValues.VisitTypeInpatient
        };
        policies[1] = policies[1] with { EnrollmentDate = null };

        var result = engine.BuildProjection(request with
        {
            Policies = policies,
            PolicyCoverages = [mismatch, conditionZ, needs, conditionA]
        });

        Assert.Equal(
            ["coverage-a", "coverage-z", "coverage-needs", "coverage-mismatch"],
            result.CoverageResults.Select(item => item.PolicyCoverageId));
    }

    [Fact]
    public void TestPcs021_Result_is_stable_across_input_order_and_current_culture()
    {
        var request = CreateRequest();
        var policies = new[]
        {
            CreatePolicy("policy-i", "family-a", "I"),
            CreatePolicy("policy-dotless", "family-a", "ı")
        };
        var coverages = new[]
        {
            CreateCoverage("coverage-i", policies[0].Id, "I"),
            CreateCoverage("coverage-dotless", policies[1].Id, "ı")
        };
        var originalCulture = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("tr-TR");
            var forward = engine.BuildProjection(request with
            {
                Policies = policies,
                PolicyCoverages = coverages
            });
            var reverse = engine.BuildProjection(request with
            {
                Policies = policies.Reverse().ToArray(),
                PolicyCoverages = coverages.Reverse().ToArray()
            });

            Assert.Equal(
                forward.CoverageResults.Select(ResultFingerprint),
                reverse.CoverageResults.Select(ResultFingerprint));
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
        }
    }

    [Fact]
    public void TestPcs022_023_Similar_claims_use_tiers_order_top_three_and_nested_terminal_facts()
    {
        var request = CreateRequest(anchorCoverageId: "coverage-a");
        var claims = new[]
        {
            request.ClaimCases![0],
            CreateClaim("claim-tier-a", "family-a", new DateOnly(2025, 1, 1), "X99", "inpatient"),
            CreateClaim("claim-tier-b", "family-a", new DateOnly(2025, 4, 1), "A00.1", "outpatient"),
            CreateClaim("claim-tier-c", "family-a", new DateOnly(2025, 3, 1), "A00", "outpatient"),
            CreateClaim("claim-tier-c-old", "family-a", new DateOnly(2025, 2, 1), "A00.1-Z", "outpatient")
        };
        var submissions = new[]
        {
            CreateSubmission("submission-a", claims[1].Id, "policy-a", "coverage-a", BaseTime.AddDays(1)),
            CreateSubmission("submission-b", claims[2].Id, "policy-a", null, BaseTime.AddDays(4)),
            CreateSubmission("submission-c", claims[3].Id, "policy-a", null, BaseTime.AddDays(3)),
            CreateSubmission("submission-c-old", claims[4].Id, "policy-a", null, BaseTime.AddDays(2))
        };
        var payments = new[]
        {
            CreatePayment("payment-a-paid", submissions[0].Id, ClaimPaymentValues.StatusPaid, BaseTime.AddDays(2)),
            CreatePayment("payment-a-denied", submissions[0].Id, ClaimPaymentValues.StatusDenied, BaseTime.AddDays(3)),
            CreatePayment("payment-a-pending", submissions[0].Id, ClaimPaymentValues.StatusPending, BaseTime.AddDays(4)),
            CreatePayment("payment-b", submissions[1].Id, ClaimPaymentValues.StatusPaid, BaseTime.AddDays(5)),
            CreatePayment("payment-c", submissions[2].Id, ClaimPaymentValues.StatusPartiallyPaid, BaseTime.AddDays(4)),
            CreatePayment("payment-c-old", submissions[3].Id, ClaimPaymentValues.StatusDenied, BaseTime.AddDays(3))
        };

        var result = engine.BuildProjection(request with
        {
            ClaimCases = claims,
            ClaimSubmissions = submissions,
            ClaimPayments = payments
        });

        Assert.Equal(
            ["submission-a", "submission-b", "submission-c"],
            result.SimilarClaims.Select(item => item.ClaimSubmissionId));
        Assert.Equal(["A", "B", "C"], result.SimilarClaims.Select(item => item.SimilarityTier));
        Assert.Equal(2, result.SimilarClaims[0].TerminalPaymentFacts.Count);
        Assert.DoesNotContain(
            result.SimilarClaims[0].TerminalPaymentFacts,
            item => item.Status == ClaimPaymentValues.StatusPending);
    }

    [Fact]
    public void TestPcs023_Null_anchor_does_not_create_tier_a()
    {
        var request = CreateRequest();
        var otherClaim = CreateClaim(
            "claim-other",
            "family-a",
            new DateOnly(2025, 1, 1),
            "X99",
            ClaimCaseValues.VisitTypeInpatient);
        var submission = CreateSubmission(
            "submission-other",
            otherClaim.Id,
            "policy-a",
            "coverage-a",
            BaseTime.AddDays(1));
        var payment = CreatePayment(
            "payment-other",
            submission.Id,
            ClaimPaymentValues.StatusPaid,
            BaseTime.AddDays(2));

        var result = engine.BuildProjection(request with
        {
            ClaimCases = [request.ClaimCases![0], otherClaim],
            ClaimSubmissions = [submission],
            ClaimPayments = [payment]
        });

        Assert.Empty(result.SimilarClaims);
    }

    [Fact]
    public void TestPcs022_Current_pending_cancelled_and_incomplete_records_are_not_similar_claims()
    {
        var request = CreateRequest();
        var otherClaim = CreateClaim(
            "claim-other",
            "family-a",
            new DateOnly(2025, 1, 1),
            "A00.1",
            "outpatient");
        var currentSubmission = CreateSubmission(
            "submission-current",
            "claim-current",
            "policy-a",
            null,
            BaseTime.AddDays(1));
        var pendingOnly = CreateSubmission(
            "submission-pending",
            otherClaim.Id,
            "policy-a",
            null,
            BaseTime.AddDays(2));
        var cancelled = CreateSubmission(
            "submission-cancelled",
            otherClaim.Id,
            "policy-a",
            null,
            BaseTime.AddDays(3)) with
        { Status = ClaimSubmissionValues.StatusCancelled };
        var incomplete = CreateSubmission(
            "submission-incomplete",
            otherClaim.Id,
            "policy-a",
            null,
            BaseTime.AddDays(4)) with
        { Status = ClaimSubmissionValues.StatusReviewing };
        var completedCancelledPayment = CreateSubmission(
            "submission-cancelled-payment",
            otherClaim.Id,
            "policy-a",
            null,
            BaseTime.AddDays(5));
        var submissions = new[]
        {
            currentSubmission,
            pendingOnly,
            cancelled,
            incomplete,
            completedCancelledPayment
        };
        var payments = new[]
        {
            CreatePayment("payment-current", currentSubmission.Id, ClaimPaymentValues.StatusPaid, BaseTime.AddDays(2)),
            CreatePayment("payment-pending", pendingOnly.Id, ClaimPaymentValues.StatusPending, BaseTime.AddDays(3)),
            CreatePayment("payment-cancelled-submission", cancelled.Id, ClaimPaymentValues.StatusPaid, BaseTime.AddDays(4)),
            CreatePayment("payment-incomplete", incomplete.Id, ClaimPaymentValues.StatusPaid, BaseTime.AddDays(5)),
            CreatePayment("payment-cancelled", completedCancelledPayment.Id, ClaimPaymentValues.StatusCancelled, BaseTime.AddDays(6))
        };

        var result = engine.BuildProjection(request with
        {
            ClaimCases = [request.ClaimCases![0], otherClaim],
            ClaimSubmissions = submissions,
            ClaimPayments = payments
        });

        Assert.Empty(result.SimilarClaims);
    }

    [Fact]
    public void TestPcs022_Historical_payment_facts_do_not_change_current_coverage_classification()
    {
        var request = CreateRequest();
        var otherClaim = CreateClaim(
            "claim-history",
            "family-a",
            new DateOnly(2025, 1, 1),
            "A00.1",
            "outpatient");
        var submission = CreateSubmission(
            "submission-history",
            otherClaim.Id,
            "policy-a",
            null,
            BaseTime.AddDays(1));
        var denied = CreatePayment(
            "payment-denied",
            submission.Id,
            ClaimPaymentValues.StatusDenied,
            BaseTime.AddDays(2));

        var withoutHistory = engine.BuildProjection(request);
        var withHistory = engine.BuildProjection(request with
        {
            ClaimCases = [request.ClaimCases![0], otherClaim],
            ClaimSubmissions = [submission],
            ClaimPayments = [denied]
        });

        Assert.Equal(
            ResultFingerprint(Assert.Single(withoutHistory.CoverageResults)),
            ResultFingerprint(Assert.Single(withHistory.CoverageResults)));
        Assert.Single(withHistory.SimilarClaims);
    }

    [Theory]
    [InlineData(ClaimSubmissionValues.StatusSubmitted)]
    [InlineData(ClaimSubmissionValues.StatusAdditionalDocumentsRequested)]
    [InlineData(ClaimSubmissionValues.StatusReviewing)]
    [InlineData(ClaimSubmissionValues.StatusCompleted)]
    public void TestPcs027_Submission_states_requiring_details_reject_missing_date(string status)
    {
        var request = CreateRequest();
        var submission = CreateSubmission(
            "submission-a",
            "claim-current",
            "policy-a",
            null,
            BaseTime) with
        {
            Status = status,
            SubmittedDate = null
        };

        AssertMatchingFailure(request with { ClaimSubmissions = [submission] });
    }

    [Theory]
    [InlineData(ClaimSubmissionValues.StatusSubmitted)]
    [InlineData(ClaimSubmissionValues.StatusAdditionalDocumentsRequested)]
    [InlineData(ClaimSubmissionValues.StatusReviewing)]
    [InlineData(ClaimSubmissionValues.StatusCompleted)]
    public void TestPcs027_Submission_states_requiring_details_reject_invalid_coverage(
        string status)
    {
        var request = CreateRequest();
        var submission = CreateSubmission(
            "submission-a",
            "claim-current",
            "policy-a",
            null,
            BaseTime) with { Status = status };

        foreach (var invalidCoverage in new string?[] { null, " ", " Synthetic coverage " })
        {
            AssertMatchingFailure(request with
            {
                ClaimSubmissions = [submission with { CoverageDisplayName = invalidCoverage }]
            });
        }
    }

    [Theory]
    [InlineData(" ")]
    [InlineData(" Synthetic memo ")]
    public void TestPcs027_Submission_memo_must_be_normalized(string invalidMemo)
    {
        var request = CreateRequest();
        var submission = CreateSubmission(
            "submission-a",
            "claim-current",
            "policy-a",
            null,
            BaseTime) with { Memo = invalidMemo };

        AssertMatchingFailure(request with { ClaimSubmissions = [submission] });
    }

    [Theory]
    [InlineData(0L)]
    [InlineData(-1L)]
    public void TestPcs027_Payment_amount_must_be_positive_when_present(long invalidAmount)
    {
        var request = CreateRequest();
        var submission = CreateSubmission(
            "submission-a",
            "claim-current",
            "policy-a",
            null,
            BaseTime);
        var payment = CreatePayment(
            "payment-a",
            submission.Id,
            ClaimPaymentValues.StatusPaid,
            BaseTime) with { PaidAmount = invalidAmount };

        AssertMatchingFailure(request with
        {
            ClaimSubmissions = [submission],
            ClaimPayments = [payment]
        });
    }

    [Theory]
    [InlineData(ClaimPaymentValues.StatusPaid, "missing_paid_date")]
    [InlineData(ClaimPaymentValues.StatusPaid, "missing_paid_amount")]
    [InlineData(ClaimPaymentValues.StatusPaid, "missing_paid_coverage")]
    [InlineData(ClaimPaymentValues.StatusPaid, "deny_reason")]
    [InlineData(ClaimPaymentValues.StatusPaid, "reduction_reason")]
    [InlineData(ClaimPaymentValues.StatusPartiallyPaid, "missing_paid_date")]
    [InlineData(ClaimPaymentValues.StatusPartiallyPaid, "missing_paid_amount")]
    [InlineData(ClaimPaymentValues.StatusPartiallyPaid, "missing_paid_coverage")]
    [InlineData(ClaimPaymentValues.StatusPartiallyPaid, "missing_reduction_reason")]
    [InlineData(ClaimPaymentValues.StatusPartiallyPaid, "deny_reason")]
    [InlineData(ClaimPaymentValues.StatusDenied, "missing_deny_reason")]
    [InlineData(ClaimPaymentValues.StatusDenied, "paid_date")]
    [InlineData(ClaimPaymentValues.StatusDenied, "paid_amount")]
    [InlineData(ClaimPaymentValues.StatusDenied, "paid_coverage")]
    [InlineData(ClaimPaymentValues.StatusDenied, "reduction_reason")]
    [InlineData(ClaimPaymentValues.StatusCancelled, "paid_date")]
    [InlineData(ClaimPaymentValues.StatusCancelled, "paid_amount")]
    [InlineData(ClaimPaymentValues.StatusCancelled, "paid_coverage")]
    [InlineData(ClaimPaymentValues.StatusCancelled, "deny_reason")]
    [InlineData(ClaimPaymentValues.StatusCancelled, "reduction_reason")]
    [InlineData(ClaimPaymentValues.StatusCancelled, "additional_documents_memo")]
    public void TestPcs027_Payment_status_fields_must_match_storage_contract(
        string status,
        string invalidField)
    {
        var request = CreateRequest();
        var submission = CreateSubmission(
            "submission-a",
            "claim-current",
            "policy-a",
            null,
            BaseTime);
        var payment = MakePaymentStatusFieldsInvalid(
            CreatePayment("payment-a", submission.Id, status, BaseTime),
            invalidField);

        AssertMatchingFailure(request with
        {
            ClaimSubmissions = [submission],
            ClaimPayments = [payment]
        });
    }

    [Theory]
    [InlineData("paid_coverage")]
    [InlineData("deny_reason")]
    [InlineData("reduction_reason")]
    [InlineData("additional_documents_memo")]
    [InlineData("memo")]
    public void TestPcs027_Payment_optional_strings_must_be_normalized(string field)
    {
        var request = CreateRequest();
        var submission = CreateSubmission(
            "submission-a",
            "claim-current",
            "policy-a",
            null,
            BaseTime);

        foreach (var invalidValue in new[] { " ", " Synthetic value " })
        {
            var payment = CreatePaymentWithInvalidNormalizedField(
                "payment-a",
                submission.Id,
                field,
                invalidValue);
            AssertMatchingFailure(request with
            {
                ClaimSubmissions = [submission],
                ClaimPayments = [payment]
            });
        }
    }

    [Theory]
    [InlineData(ClaimPaymentValues.StatusPending)]
    [InlineData(ClaimPaymentValues.StatusPaid)]
    [InlineData(ClaimPaymentValues.StatusPartiallyPaid)]
    [InlineData(ClaimPaymentValues.StatusDenied)]
    [InlineData(ClaimPaymentValues.StatusCancelled)]
    public void TestPcs027_Valid_payment_status_contracts_are_accepted(string status)
    {
        var request = CreateRequest();
        var submission = CreateSubmission(
            "submission-a",
            "claim-current",
            "policy-a",
            null,
            BaseTime);

        var result = engine.BuildProjection(request with
        {
            ClaimSubmissions = [submission],
            ClaimPayments = [CreatePayment("payment-a", submission.Id, status, BaseTime)]
        });

        Assert.Empty(result.SimilarClaims);
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
    [InlineData(11)]
    [InlineData(12)]
    [InlineData(13)]
    [InlineData(14)]
    [InlineData(15)]
    [InlineData(16)]
    [InlineData(17)]
    [InlineData(18)]
    [InlineData(19)]
    [InlineData(20)]
    [InlineData(21)]
    [InlineData(22)]
    public void TestPcs027_Whole_graph_fail_closed_rejects_integrity_and_value_failures(int scenario)
    {
        var request = CreateInvalidGraphScenario(scenario);

        var exception = Assert.Throws<ClaimReferenceMatchingException>(() =>
            engine.BuildProjection(request));

        Assert.Equal(ClaimReferenceMatchingErrorCode.InvalidGraph, exception.ErrorCode);
    }

    [Fact]
    public void TestPcs031_Policy_document_source_requires_a_same_policy_reference()
    {
        var request = CreateRequest();
        var document = CreatePolicyDocument("pdoc-a", "policy-a");
        var coverage = request.PolicyCoverages![0] with
        {
            SourceKind = PolicyCoverageValues.SourcePolicyDocument,
            SourcePolicyDocumentId = document.Id
        };

        var result = engine.BuildProjection(request with
        {
            PolicyCoverages = [coverage],
            PolicyDocuments = [document]
        });

        Assert.True(Assert.Single(result.CoverageResults).HasSourcePolicyDocument);

        var invalidManual = coverage with { SourceKind = PolicyCoverageValues.SourceManual };
        AssertMatchingFailure(request with
        {
            PolicyCoverages = [invalidManual],
            PolicyDocuments = [document]
        });
    }

    [Fact]
    public void TestPcs032_Reversed_effective_dates_fail_closed()
    {
        var request = CreateRequest();
        var coverage = request.PolicyCoverages![0] with
        {
            EffectiveFrom = new DateOnly(2026, 3, 1),
            EffectiveTo = new DateOnly(2026, 2, 1)
        };

        AssertMatchingFailure(request with { PolicyCoverages = [coverage] });
    }

    [Fact]
    public void TestPcs024_Engine_is_read_only_and_failure_messages_do_not_expose_sensitive_input()
    {
        var request = CreateRequest();
        var familySnapshot = request.FamilyMembers!.ToArray();
        var policySnapshot = request.Policies!.ToArray();
        var coverageSnapshot = request.PolicyCoverages!.ToArray();
        var claimSnapshot = request.ClaimCases!.ToArray();

        _ = engine.BuildProjection(request);

        Assert.Equal(familySnapshot, request.FamilyMembers);
        Assert.Equal(policySnapshot, request.Policies);
        Assert.Equal(coverageSnapshot, request.PolicyCoverages);
        Assert.Equal(claimSnapshot, request.ClaimCases);

        const string sensitiveId = "raw-sensitive-id";
        const string sensitivePath = "C:\\synthetic-private\\fixture.json";
        var exception = Assert.Throws<ClaimReferenceMatchingException>(() =>
            engine.BuildProjection(request with { SelectedClaimCaseId = sensitiveId }));
        Assert.DoesNotContain(sensitiveId, exception.Message, StringComparison.Ordinal);
        Assert.DoesNotContain(sensitivePath, exception.Message, StringComparison.Ordinal);
        Assert.Null(exception.InnerException);
    }

    private static ClaimReferenceMatchingRequest CreateInvalidGraphScenario(int scenario)
    {
        var request = CreateRequest();
        return scenario switch
        {
            0 => request with { Policies = [request.Policies![0], request.Policies[0]] },
            1 => request with
            {
                PolicyCoverages = [request.PolicyCoverages![0] with { PolicyId = "policy-missing" }]
            },
            2 => request with
            {
                PolicyCoverages = [request.PolicyCoverages![0] with
                {
                    SourceKind = PolicyCoverageValues.SourcePolicyDocument,
                    SourcePolicyDocumentId = "pdoc-missing"
                }]
            },
            3 => CreateCrossPolicySourceRequest(request),
            4 => request with
            {
                ClaimSubmissions = [CreateSubmission(
                    "submission-a",
                    "claim-missing",
                    "policy-a",
                    null,
                    BaseTime)]
            },
            5 => CreateCrossFamilySubmissionRequest(request),
            6 => CreateCoveragePolicyMismatchSubmissionRequest(request),
            7 => request with
            {
                ClaimPayments = [CreatePayment(
                    "payment-a",
                    "submission-missing",
                    ClaimPaymentValues.StatusPaid,
                    BaseTime)]
            },
            8 => request with
            {
                Policies = [request.Policies![0] with { ContractStatus = "unknown" }]
            },
            9 => request with
            {
                PolicyCoverages = [request.PolicyCoverages![0] with { ReviewStatus = "unknown" }]
            },
            10 => request with
            {
                PolicyCoverages = [request.PolicyCoverages![0] with { VisitTypeRule = "unknown" }]
            },
            11 => request with
            {
                PolicyCoverages = [request.PolicyCoverages![0] with { SourceKind = "unknown" }]
            },
            12 => request with
            {
                PolicyCoverages = [request.PolicyCoverages![0] with
                {
                    DiagnosisCodePrefixes = [" a00"]
                }]
            },
            13 => request with
            {
                Policies = [request.Policies![0] with { FamilyMemberId = null }]
            },
            14 => request with { ClaimPayments = null },
            15 => request with
            {
                PolicyDocuments = [CreatePolicyDocument("pdoc-a", "policy-missing")]
            },
            16 => request with
            {
                ClaimSubmissions = [CreateSubmission(
                    "submission-a",
                    "claim-current",
                    "policy-a",
                    null,
                    BaseTime) with { Status = "unknown" }]
            },
            17 => CreateUnknownPaymentStatusRequest(request),
            18 => request with
            {
                PolicyCoverages = [request.PolicyCoverages![0] with
                {
                    DiagnosisRuleMode = PolicyCoverageValues.DiagnosisRulePrefixList,
                    DiagnosisCodePrefixes = []
                }]
            },
            19 => request with
            {
                PolicyCoverages = [request.PolicyCoverages![0] with { SurgeryRule = "unknown" }]
            },
            20 => request with
            {
                PolicyCoverages = [request.PolicyCoverages![0] with { PrescriptionRule = "unknown" }]
            },
            21 => request with
            {
                PolicyCoverages = [request.PolicyCoverages![0] with
                {
                    DiagnosisRuleMode = PolicyCoverageValues.DiagnosisRulePrefixList,
                    DiagnosisCodePrefixes = ["A00", "A00"]
                }]
            },
            22 => request with
            {
                ClaimCases = [request.ClaimCases![0] with { CaseStatus = "unknown" }]
            },
            _ => throw new ArgumentOutOfRangeException(nameof(scenario))
        };
    }

    private static ClaimReferenceMatchingRequest CreateUnknownPaymentStatusRequest(
        ClaimReferenceMatchingRequest request)
    {
        var submission = CreateSubmission(
            "submission-a",
            "claim-current",
            "policy-a",
            null,
            BaseTime);
        return request with
        {
            ClaimSubmissions = [submission],
            ClaimPayments = [CreatePayment(
                "payment-a",
                submission.Id,
                "unknown",
                BaseTime)]
        };
    }

    private static ClaimReferenceMatchingRequest CreateCrossPolicySourceRequest(
        ClaimReferenceMatchingRequest request)
    {
        var policyB = CreatePolicy("policy-b", "family-a", "Policy B");
        var documentB = CreatePolicyDocument("pdoc-b", policyB.Id);
        return request with
        {
            Policies = [.. request.Policies!, policyB],
            PolicyDocuments = [documentB],
            PolicyCoverages = [request.PolicyCoverages![0] with
            {
                SourceKind = PolicyCoverageValues.SourcePolicyDocument,
                SourcePolicyDocumentId = documentB.Id
            }]
        };
    }

    private static ClaimReferenceMatchingRequest CreateCrossFamilySubmissionRequest(
        ClaimReferenceMatchingRequest request)
    {
        var familyB = CreateFamily("family-b", "Synthetic B");
        var policyB = CreatePolicy("policy-b", familyB.Id, "Policy B");
        return request with
        {
            FamilyMembers = [.. request.FamilyMembers!, familyB],
            Policies = [.. request.Policies!, policyB],
            ClaimSubmissions = [CreateSubmission(
                "submission-a",
                "claim-current",
                policyB.Id,
                null,
                BaseTime)]
        };
    }

    private static ClaimReferenceMatchingRequest CreateCoveragePolicyMismatchSubmissionRequest(
        ClaimReferenceMatchingRequest request)
    {
        var policyB = CreatePolicy("policy-b", "family-a", "Policy B");
        var coverageB = CreateCoverage("coverage-b", policyB.Id, "Coverage B");
        return request with
        {
            Policies = [.. request.Policies!, policyB],
            PolicyCoverages = [.. request.PolicyCoverages!, coverageB],
            ClaimSubmissions = [CreateSubmission(
                "submission-a",
                "claim-current",
                "policy-a",
                coverageB.PolicyCoverageId,
                BaseTime)]
        };
    }

    private void AssertBooleanRule(
        Func<PolicyCoverageRecord, PolicyCoverageRecord> coverageMutation,
        Func<ClaimRecord, ClaimRecord> claimMutation,
        string ruleName,
        string expected)
    {
        var request = CreateRequest();
        var coverage = coverageMutation(request.PolicyCoverages![0]);
        var claim = claimMutation(request.ClaimCases![0]);

        var result = engine.BuildProjection(request with
        {
            PolicyCoverages = [coverage],
            ClaimCases = [claim]
        });

        Assert.Equal(expected, Evidence(result, coverage.PolicyCoverageId, ruleName));
    }

    private void AssertSelectedClaimFailure(ClaimReferenceMatchingRequest request)
    {
        var exception = Assert.Throws<ClaimReferenceMatchingException>(() =>
            engine.BuildProjection(request));
        Assert.Equal(ClaimReferenceMatchingErrorCode.SelectedClaimUnavailable, exception.ErrorCode);
    }

    private void AssertMatchingFailure(ClaimReferenceMatchingRequest request)
    {
        var exception = Assert.Throws<ClaimReferenceMatchingException>(() =>
            engine.BuildProjection(request));
        Assert.Equal(ClaimReferenceMatchingErrorCode.InvalidGraph, exception.ErrorCode);
    }

    private static ClaimPaymentRecord MakePaymentStatusFieldsInvalid(
        ClaimPaymentRecord payment,
        string invalidField)
    {
        return invalidField switch
        {
            "missing_paid_date" => payment with { PaidDate = null },
            "missing_paid_amount" => payment with { PaidAmount = null },
            "missing_paid_coverage" => payment with { PaidCoverageDisplayName = null },
            "missing_deny_reason" => payment with { DenyReason = null },
            "missing_reduction_reason" => payment with { ReductionReason = null },
            "paid_date" => payment with { PaidDate = new DateOnly(2025, 1, 3) },
            "paid_amount" => payment with { PaidAmount = 10_000 },
            "paid_coverage" => payment with { PaidCoverageDisplayName = "Synthetic coverage" },
            "deny_reason" => payment with { DenyReason = "Synthetic denial" },
            "reduction_reason" => payment with { ReductionReason = "Synthetic reduction" },
            "additional_documents_memo" => payment with
            {
                AdditionalDocumentsMemo = "Synthetic additional documents"
            },
            _ => throw new ArgumentOutOfRangeException(nameof(invalidField))
        };
    }

    private static ClaimPaymentRecord CreatePaymentWithInvalidNormalizedField(
        string id,
        string submissionId,
        string field,
        string invalidValue)
    {
        return field switch
        {
            "paid_coverage" => CreatePayment(
                id,
                submissionId,
                ClaimPaymentValues.StatusPaid,
                BaseTime) with { PaidCoverageDisplayName = invalidValue },
            "deny_reason" => CreatePayment(
                id,
                submissionId,
                ClaimPaymentValues.StatusDenied,
                BaseTime) with { DenyReason = invalidValue },
            "reduction_reason" => CreatePayment(
                id,
                submissionId,
                ClaimPaymentValues.StatusPartiallyPaid,
                BaseTime) with { ReductionReason = invalidValue },
            "additional_documents_memo" => CreatePayment(
                id,
                submissionId,
                ClaimPaymentValues.StatusPending,
                BaseTime) with { AdditionalDocumentsMemo = invalidValue },
            "memo" => CreatePayment(
                id,
                submissionId,
                ClaimPaymentValues.StatusPending,
                BaseTime) with { Memo = invalidValue },
            _ => throw new ArgumentOutOfRangeException(nameof(field))
        };
    }

    private static string Evidence(
        ClaimReferenceProjection projection,
        string coverageId,
        string ruleName)
    {
        return projection.CoverageResults
            .Single(item => item.PolicyCoverageId == coverageId)
            .RuleEvidence
            .Single(item => item.RuleName == ruleName)
            .Outcome;
    }

    private static string ResultFingerprint(ClaimReferenceCoverageResult result)
    {
        return string.Join(
            "|",
            result.PolicyId,
            result.PolicyCoverageId,
            result.PolicyDisplayName,
            result.CoverageDisplayName,
            result.ResultGroup,
            result.PassedRuleCount,
            result.HasSourcePolicyDocument,
            string.Join(",", result.RuleEvidence.Select(item => $"{item.RuleName}:{item.Outcome}")));
    }

    private static ClaimReferenceMatchingRequest CreateRequest(string? anchorCoverageId = null)
    {
        var family = CreateFamily("family-a", "Synthetic A");
        var policy = CreatePolicy("policy-a", family.Id, "Policy A");
        var coverage = CreateCoverage("coverage-a", policy.Id, "Coverage A");
        var claim = CreateClaim(
            "claim-current",
            family.Id,
            new DateOnly(2026, 2, 10),
            "A00.1",
            ClaimCaseValues.VisitTypeOutpatient);
        return new ClaimReferenceMatchingRequest(
            claim.Id,
            anchorCoverageId,
            [family],
            [policy],
            [coverage],
            [claim],
            [],
            [],
            []);
    }

    private static FamilyMemberRecord CreateFamily(string id, string displayName)
    {
        return new FamilyMemberRecord(
            id,
            displayName,
            "Synthetic relation",
            null,
            BaseTime,
            BaseTime,
            null,
            1);
    }

    private static PolicyRecord CreatePolicy(
        string id,
        string familyId,
        string displayTitle,
        string contractStatus = InsurancePolicyValues.ContractStatusActive)
    {
        return new PolicyRecord(
            id,
            displayTitle,
            new DateOnly(2025, 1, 1),
            BaseTime,
            BaseTime,
            null,
            familyId,
            "Synthetic insurer",
            contractStatus,
            new DateOnly(2025, 1, 1));
    }

    private static PolicyCoverageRecord CreateCoverage(
        string id,
        string policyId,
        string displayName)
    {
        return new PolicyCoverageRecord(
            id,
            policyId,
            displayName,
            PolicyCoverageValues.ReviewStatusUserConfirmed,
            new DateOnly(2025, 1, 1),
            new DateOnly(2026, 12, 31),
            PolicyCoverageValues.VisitTypeAny,
            PolicyCoverageValues.ConditionAny,
            PolicyCoverageValues.ConditionAny,
            PolicyCoverageValues.DiagnosisRuleAny,
            [],
            PolicyCoverageValues.SourceManual,
            null,
            null,
            null,
            1,
            BaseTime,
            BaseTime,
            null);
    }

    private static ClaimRecord CreateClaim(
        string id,
        string familyId,
        DateOnly treatmentDate,
        string? diagnosisCode,
        string visitType)
    {
        return new ClaimRecord(
            id,
            null,
            $"Synthetic {id}",
            treatmentDate,
            BaseTime,
            BaseTime,
            null,
            familyId,
            null,
            diagnosisCode,
            null,
            visitType,
            false,
            false,
            null,
            null,
            null,
            null,
            ClaimCaseValues.StatusSaved,
            1);
    }

    private static ClaimSubmissionRecord CreateSubmission(
        string id,
        string claimCaseId,
        string policyId,
        string? policyCoverageId,
        DateTimeOffset updatedAt)
    {
        return new ClaimSubmissionRecord(
            id,
            claimCaseId,
            policyId,
            policyCoverageId,
            "Synthetic coverage",
            new DateOnly(2025, 1, 2),
            null,
            [],
            ClaimSubmissionValues.StatusCompleted,
            null,
            1,
            BaseTime,
            updatedAt);
    }

    private static ClaimPaymentRecord CreatePayment(
        string id,
        string submissionId,
        string status,
        DateTimeOffset updatedAt)
    {
        var payment = new ClaimPaymentRecord(
            id,
            submissionId,
            status,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            1,
            BaseTime,
            updatedAt);

        return status switch
        {
            ClaimPaymentValues.StatusPaid => payment with
            {
                PaidDate = new DateOnly(2025, 1, 3),
                PaidAmount = 10_000,
                PaidCoverageDisplayName = "Synthetic coverage"
            },
            ClaimPaymentValues.StatusPartiallyPaid => payment with
            {
                PaidDate = new DateOnly(2025, 1, 3),
                PaidAmount = 10_000,
                PaidCoverageDisplayName = "Synthetic coverage",
                ReductionReason = "Synthetic reduction"
            },
            ClaimPaymentValues.StatusDenied => payment with
            {
                DenyReason = "Synthetic denial"
            },
            _ => payment
        };
    }

    private static PolicyDocumentRecord CreatePolicyDocument(string id, string policyId)
    {
        return new PolicyDocumentRecord(
            id,
            policyId,
            $"document-{id}",
            "policy_terms",
            BaseTime,
            BaseTime,
            null);
    }
}
