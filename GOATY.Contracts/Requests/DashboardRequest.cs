namespace GOATY.Contracts.Requests
{
    public sealed record class DashboardRequest(
        DateOnly Day = default,
        string TimeZone = "UTC");
}
