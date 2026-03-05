namespace GOATY.Application.Features.Dashboards.DTOs
{
    public sealed class Dashboard
    {
        public int TotalOrders { get; set; }
        public int TotalScheduled { get; set; }
        public int TotalInProgress { get; set; }
        public int TotalCompleted { get; set; }
        public int TotalCancelled { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal TotalPartsCost { get; set; }
        public decimal TotalTechniciansCost { get; set; }
        public decimal NetProfit { get; set; }
        public int UniqueVehicles { get; set; }
        public int UniqueCustomers { get; set; }
        public decimal ProfitMargin { get; set; }
        public double CompletionRate { get; set; }
        public double CancellationRate { get; set; }
        public double AvgRevenuPerOrder { get; set; }
        public double OrdersPerVehicle { get; set; }
        public double PartsCostRatio { get; set; }
        public double LaborCostRatio { get; set; }
    }
}
