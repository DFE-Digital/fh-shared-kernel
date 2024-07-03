namespace FamilyHubs.SharedKernel.Reports.ConnectionRequests;

public class ConnectionRequestsBreakdown
{
    public ConnectionRequests Totals { get; init; } = null!;

    public IEnumerable<ConnectionRequests> WeeklyReports { get; init; } = null!;
}
