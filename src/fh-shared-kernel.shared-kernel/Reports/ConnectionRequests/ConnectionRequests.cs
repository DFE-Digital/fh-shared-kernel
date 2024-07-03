namespace FamilyHubs.SharedKernel.Reports.ConnectionRequests;

public class ConnectionRequests
{
    public string? Date { get; set; }

    public int Made { get; init; }

    public int Accepted { get; init; }

    public int Declined { get; init; }
}
