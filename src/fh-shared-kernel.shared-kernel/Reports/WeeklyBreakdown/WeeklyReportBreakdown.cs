namespace FamilyHubs.SharedKernel.Reports.WeeklyBreakdown;

public class WeeklyReportBreakdown
{
    public IEnumerable<WeeklyReport> WeeklyReports { get; init; } = null!;

    public int TotalSearchCount { get; init; }
}
