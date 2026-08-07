namespace FamilyClaimRef.App.Models.Storage;

public static class ClaimSubmissionValues
{
    public const string StatusPreparing = "preparing";
    public const string StatusSubmitted = "submitted";
    public const string StatusAdditionalDocumentsRequested = "additional_documents_requested";
    public const string StatusReviewing = "reviewing";
    public const string StatusCancelled = "cancelled";
    public const string StatusCompleted = "submission_completed";

    public static IReadOnlyList<string> Statuses { get; } =
    [
        StatusPreparing,
        StatusSubmitted,
        StatusAdditionalDocumentsRequested,
        StatusReviewing,
        StatusCancelled,
        StatusCompleted
    ];

    public static bool IsTerminal(string status)
    {
        return string.Equals(status, StatusCancelled, StringComparison.Ordinal)
            || string.Equals(status, StatusCompleted, StringComparison.Ordinal);
    }

    public static bool RequiresSubmittedDetails(string status)
    {
        return string.Equals(status, StatusSubmitted, StringComparison.Ordinal)
            || string.Equals(status, StatusAdditionalDocumentsRequested, StringComparison.Ordinal)
            || string.Equals(status, StatusReviewing, StringComparison.Ordinal)
            || string.Equals(status, StatusCompleted, StringComparison.Ordinal);
    }

    public static IReadOnlyList<string> GetAllowedTargets(string currentStatus)
    {
        return currentStatus switch
        {
            StatusPreparing => [StatusSubmitted, StatusCancelled],
            StatusSubmitted =>
            [
                StatusAdditionalDocumentsRequested,
                StatusReviewing,
                StatusCompleted,
                StatusCancelled
            ],
            StatusAdditionalDocumentsRequested =>
            [
                StatusSubmitted,
                StatusReviewing,
                StatusCancelled
            ],
            StatusReviewing =>
            [
                StatusAdditionalDocumentsRequested,
                StatusCompleted,
                StatusCancelled
            ],
            _ => []
        };
    }
}
