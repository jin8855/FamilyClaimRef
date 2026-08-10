namespace FamilyClaimRef.App.Models.Storage;

public static class ClaimPaymentValues
{
    public const string StatusPending = "pending";
    public const string StatusPaid = "paid";
    public const string StatusPartiallyPaid = "partially_paid";
    public const string StatusDenied = "denied";
    public const string StatusCancelled = "cancelled";

    public static IReadOnlyList<string> Statuses { get; } =
    [
        StatusPending,
        StatusPaid,
        StatusPartiallyPaid,
        StatusDenied,
        StatusCancelled
    ];

    public static IReadOnlyList<string> TerminalStatuses { get; } =
    [
        StatusPaid,
        StatusPartiallyPaid,
        StatusDenied,
        StatusCancelled
    ];

    public static bool IsTerminal(string status)
    {
        return TerminalStatuses.Contains(status, StringComparer.Ordinal);
    }

    public static IReadOnlyList<string> GetAllowedTargets(string currentStatus)
    {
        return string.Equals(currentStatus, StatusPending, StringComparison.Ordinal)
            ? TerminalStatuses
            : [];
    }
}
