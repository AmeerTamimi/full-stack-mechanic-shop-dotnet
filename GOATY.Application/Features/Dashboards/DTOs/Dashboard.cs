namespace GOATY.Application.Features.Dashboards.DTOs
{
    public sealed class Dashboard
    {
        public DateOnly Day { get; set; }
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
        public decimal CompletionRate { get; set; }
        public decimal CancellationRate { get; set; }
        public decimal AvgRevenuPerOrder { get; set; }
        public decimal OrdersPerVehicle { get; set; }
        public decimal PartsCostRatio { get; set; }
        public decimal LaborCostRatio { get; set; }
    }
}