namespace GOATY.Contracts.Requests
{
    public sealed class RepairTaskRequest
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal TimeEstimated { get; set; }
        public decimal CostEstimated { get; set; }
        public List<RepairTaskDetailsRequest> RepairtTaskDetails { get; set; } = [];
    }
}
