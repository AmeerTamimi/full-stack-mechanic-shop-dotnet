using GOATY.Domain.Common.Enums;

namespace GOATY.Contracts.Requests
{
    public sealed class RepairTaskRequest
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public TimeStamps TimeEstimated { get; set; }
        public decimal CostEstimated { get; set; }
        public decimal TechnicianCost { get; set; }
        public List<PartRequirementsRequest> Parts { get; set; } = [];
    }
}
